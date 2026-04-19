using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CrossWords {

    public class Board : MonoBehaviour
    {
        public const int BOARD_SIZE = 11;

        static readonly Color CellNormalColor = new Color(0.96f, 0.96f, 0.94f, 1f);
        static readonly Color CellSelectedColor = new Color(0.28f, 0.28f, 0.28f, 1f);
        static readonly Color CellBorderColor = Color.black;
        static readonly Color StarterCellInnerColor = Color.black;
        static readonly Color StarterCellBorderColor = Color.white;
        static readonly Color StarterCellTextColor = Color.white;
        static readonly Color NotInDictionaryCellTextColor = Color.red;
        static readonly Color NormalCellTextColor = Color.black;
        const float CellBorderPx = 3f;
        const float GridSpacingPx = 0f;

        const float BorderWidthScreenPx = 5f;
        const string BorderChildName = "BoardBorder";
        const string CorrectChildName = "BoardCorrect";
        const string FillChildName = "BoardFill";
        const string GridChildName = "BoardGrid";

        readonly char[,] _characters = new char[BOARD_SIZE, BOARD_SIZE];
        readonly bool[,] _isStarterCell = new bool[BOARD_SIZE, BOARD_SIZE];
        CellView[,] _cellViews;

        Image _borderImage;
        Image _correctImage;
        Image _fillImage;
        RectTransform _gridRect;
        GridLayoutGroup _gridLayout;

        int? _selectedX;
        int? _selectedY;
        bool interactionBlocked = false;

        RectTransform PanelRect => (RectTransform)transform;

        void Awake()
        {
            EnsureClipMask();
            EnsureBorderGraphic();
            EnsureFillGraphic();
            EnsureGrid();
        }

        void Start()
        {
            RefreshGridCellSizes();
            SyncAllCellLabels();
        }

        void OnRectTransformDimensionsChange()
        {
            ApplyFillLayout();
            RefreshGridCellSizes();
        }

        void EnsureClipMask()
        {
            if (GetComponent<RectMask2D>() == null)
                gameObject.AddComponent<RectMask2D>();
        }

        void EnsureBorderGraphic()
        {
            Transform existing = transform.Find(BorderChildName);
            GameObject borderGo;
            if (existing != null)
            {
                borderGo = existing.gameObject;
                _borderImage = borderGo.GetComponent<Image>();
                if (_borderImage == null)
                    _borderImage = borderGo.AddComponent<Image>();
            }
            else
            {
                borderGo = new GameObject(BorderChildName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                borderGo.transform.SetParent(transform, false);
                _borderImage = borderGo.GetComponent<Image>();
            }

            _borderImage.sprite = UiSpriteUtility.WhiteSprite;
            _borderImage.type = Image.Type.Simple;
            _borderImage.color = Color.black;

            var borderRect = borderGo.GetComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.offsetMin = Vector2.zero;
            borderRect.offsetMax = Vector2.zero;
            borderRect.pivot = new Vector2(0.5f, 0.5f);
            borderRect.anchoredPosition = Vector2.zero;
            borderRect.localScale = Vector3.one;

            borderGo.transform.SetSiblingIndex(0);
        }

        void EnsureFillGraphic()
        {
            Transform existing = transform.Find(FillChildName);
            GameObject fillGo;
            if (existing != null)
            {
                fillGo = existing.gameObject;
                _fillImage = fillGo.GetComponent<Image>();
                if (_fillImage == null)
                    _fillImage = fillGo.AddComponent<Image>();
            }
            else
            {
                fillGo = new GameObject(FillChildName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                fillGo.transform.SetParent(transform, false);
                _fillImage = fillGo.GetComponent<Image>();
            }

            _fillImage.sprite = UiSpriteUtility.WhiteSprite;
            _fillImage.type = Image.Type.Simple;
            _fillImage.color = Color.white;

            var fillRect = fillGo.GetComponent<RectTransform>();
            fillRect.pivot = new Vector2(0.5f, 0.5f);
            fillRect.anchoredPosition = Vector2.zero;
            fillRect.localScale = Vector3.one;

            fillGo.transform.SetSiblingIndex(1);
            ApplyFillLayout();
        }

        void EnsureGrid()
        {
            if (_fillImage == null || _fillImage.transform.Find(GridChildName) != null)
                return;

            var gridGo = new GameObject(GridChildName, typeof(RectTransform));
            gridGo.transform.SetParent(_fillImage.transform, false);
            _gridRect = gridGo.GetComponent<RectTransform>();
            _gridRect.anchorMin = Vector2.zero;
            _gridRect.anchorMax = Vector2.one;
            _gridRect.offsetMin = Vector2.zero;
            _gridRect.offsetMax = Vector2.zero;
            _gridRect.pivot = new Vector2(0.5f, 0.5f);
            _gridRect.localScale = Vector3.one;

            _gridLayout = gridGo.AddComponent<GridLayoutGroup>();
            _gridLayout.childAlignment = TextAnchor.MiddleCenter;
            _gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            _gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            //_gridLayout.childControlWidth = true;
            //_gridLayout.childControlHeight = true;
            //_gridLayout.childForceExpandWidth = true;
            //_gridLayout.childForceExpandHeight = true;
            _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayout.constraintCount = BOARD_SIZE;
            _gridLayout.spacing = new Vector2(GridSpacingPx, GridSpacingPx);

            _cellViews = new CellView[BOARD_SIZE, BOARD_SIZE];
            var font = UiFontUtility.DefaultUIFont;

            for (int y = 0; y < BOARD_SIZE; y++)
            {
                for (int x = 0; x < BOARD_SIZE; x++)
                {
                    _cellViews[y, x] = CellView.Create(this, _gridRect, x, y, font);
                }
            }

            gridGo.transform.SetAsLastSibling();
        }

        void RefreshGridCellSizes()
        {
            if (_gridLayout == null || _fillImage == null)
                return;

            Rect r = _fillImage.rectTransform.rect;
            float spacing = GridSpacingPx;
            float innerW = Mathf.Max(0f, r.width - spacing * (BOARD_SIZE - 1));
            float innerH = Mathf.Max(0f, r.height - spacing * (BOARD_SIZE - 1));
            float cw = innerW / BOARD_SIZE;
            float ch = innerH / BOARD_SIZE;
            _gridLayout.cellSize = new Vector2(cw, ch);
        }

        void ApplyFillLayout()
        {
            if (_fillImage == null)
                return;

            float inset = ComputeBorderInset();
            var fillRect = _fillImage.rectTransform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(inset, inset);
            fillRect.offsetMax = new Vector2(-inset, -inset);
        }

        float ComputeBorderInset()
        {
            float sx = transform.lossyScale.x;
            if (sx < 1e-4f)
                sx = 1f;
            float inset = BorderWidthScreenPx / sx;

            float w = PanelRect.rect.width;
            float h = PanelRect.rect.height;
            if (w > 1e-4f && h > 1e-4f)
            {
                float half = Mathf.Min(w, h) * 0.5f;
                float maxInset = Mathf.Max(0f, half - 0.5f);
                inset = Mathf.Min(inset, maxInset);
            }

            return Mathf.Max(0f, inset);
        }

        void SyncAllCellLabels()
        {
            if (_cellViews == null)
                return;
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                for (int x = 0; x < BOARD_SIZE; x++)
                {
                    _cellViews[y, x].SetDisplayedCharacter(_characters[y, x]);
                    if (_isStarterCell[y, x])
                        _cellViews[y, x].SetStarterWordAppearance();
                    else
                        _cellViews[y, x].SetNormalAppearance();
                }
            }
        }

        internal void OnCellPointerDown(int x, int y)
        {
            if (!CanSelectCell(x, y))
                return;

            // If an cell is already selected, then switch the colour
            // back to its unselected colour.
            if (_selectedX.HasValue && _selectedY.HasValue)
            {
                RestoreCellBackground(_selectedX.Value, _selectedY.Value);
            }

            _selectedX = x;
            _selectedY = y;
            _cellViews[y, x].SetBackgroundColor(CellSelectedColor);
        }

        public void FakeOnCellPointerDown(int x, int y)
        {
            OnCellPointerDown(x, y);
        }


        /// <summary>
        /// Sets the character at grid position (x, y): x is column (0 = left), y is row (0 = top).
        /// </summary>
        public void SetCell(int x, int y, char value)
        {
            if ((uint)x >= BOARD_SIZE || (uint)y >= BOARD_SIZE)
                return;

            _characters[y, x] = value;
            if (_cellViews != null)
            {
                _cellViews[y, x].SetDisplayedCharacter(value);

                if (IsCellOccupied(x, y))
                {
                    RestoreCellVisual(x, y);
                    if (_selectedX.HasValue && _selectedY.HasValue &&
                        _selectedX.Value == x && _selectedY.Value == y)
                    {
                        _selectedX = null;
                        _selectedY = null;
                    }
                }
            }
        }

        public void SetCells(string all)
        {
            int i = 0;
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                for (int x = 0; x < BOARD_SIZE; x++)
                {
                    SetCell(x, y, all[i++]);
                }
            }
        }

        public void ResetCell(int x, int y)
        {
            SetCell(x, y, '\0');
        }

        public void ResetAllCells()
        {
            for (int yy = 0; yy < BOARD_SIZE; yy++)
            {
                for (int xx = 0; xx < BOARD_SIZE; xx++)
                {
                    SetCell(xx, yy, '\0');
                }
            }
        }

        public void HighlightNotInDictionaryCells(int startX, int startY, int length, bool horizontal)
        {
            if (horizontal)
            {
                for (int i = 0; i < length; i++)
                {
                    _cellViews[startY, startX + i].SetDisplayedColourNotInDictionary();
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    _cellViews[startY + i, startX].SetDisplayedColourNotInDictionary();
                }
            }
        }


        static bool IsEmptyChar(char c) => c == '\0' || char.IsWhiteSpace(c);

        public bool IsCellOccupied(int x, int y)
        {
            return !IsEmptyChar(_characters[y, x]);
        }

        bool CanSelectCell(int x, int y)
        {
            if (interactionBlocked)
            {
                return false;
            }

            if (IsStarterCell(x, y))
            {
                return false;
            }
            return true;
        }

        public void BlockInteraction()
        {
            interactionBlocked = true;
        }

        bool HasAnyCharacterOnBoard()
        {
            for (int yy = 0; yy < BOARD_SIZE; yy++)
            {
                for (int xx = 0; xx < BOARD_SIZE; xx++)
                {
                    if (IsCellOccupied(xx, yy))
                        return true;
                }
            }
            return false;
        }

        public char GetCell(int x, int y)
        {
            if ((uint)x >= BOARD_SIZE || (uint)y >= BOARD_SIZE)
                return '\0';
            return _characters[y, x];
        }

        public string GetCells()
        {
            string s = "";
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                for (int x = 0; x < BOARD_SIZE; x++)
                {
                    char c = _characters[y, x];
                    c = (c == '\0') ? ' ' : c;
                    s += c;
                }
            }
            return s;
        }

        /**
         * Indicate which cell if any has been selected.
         */
        public bool TryGetSelectedCell(out int x, out int y)
        {
            if (!_selectedX.HasValue || !_selectedY.HasValue)
            {
                x = 0;
                y = 0;
                return false;
            }

            x = _selectedX.Value;
            y = _selectedY.Value;
            return true;
        }

        /// <summary>
        /// Places the starter word horizontally on the middle row, centered. Starter cells use a black fill,
        /// white text, and white grid borders.
        /// </summary>
        public void SetStarterWord(string word)
        {
            if (_cellViews == null || string.IsNullOrEmpty(word))
                return;

            ClearStarterWordCells();

            word = word.Trim().ToUpperInvariant();
            int n = word.Length;
            if (n == 0 || n > BOARD_SIZE)
                return;

            int row = BOARD_SIZE / 2;
            int startX = (BOARD_SIZE - n) / 2;

            for (int i = 0; i < n; i++)
            {
                int x = startX + i;
                char c = word[i];
                if (c < 'A' || c > 'Z')
                    continue;

                _characters[row, x] = c;
                _isStarterCell[row, x] = true;
                _cellViews[row, x].SetDisplayedCharacter(c);
                _cellViews[row, x].SetStarterWordAppearance();
            }
        }

        void ClearStarterWordCells()
        {
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                for (int x = 0; x < BOARD_SIZE; x++)
                {
                    if (!_isStarterCell[y, x])
                        continue;

                    _isStarterCell[y, x] = false;
                    _characters[y, x] = '\0';
                    _cellViews[y, x].SetDisplayedCharacter('\0');
                    _cellViews[y, x].SetNormalAppearance();
                }
            }
        }

        internal bool IsStarterCell(int x, int y)
        {
            return _isStarterCell[y, x];
        }


        void RestoreCellBackground(int x, int y)
        {
            if (_isStarterCell[y, x])
                _cellViews[y, x].SetStarterWordBackground();
            else
                _cellViews[y, x].SetNormalBackground();
        }

        void RestoreCellVisual(int x, int y)
        {
            if (_isStarterCell[y, x])
                _cellViews[y, x].SetStarterWordAppearance();
            else
                _cellViews[y, x].SetNormalAppearance();
        }

        public void RestoreAllCellsVisual()
        {
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                for (int x = 0; x < BOARD_SIZE; x++)
                {
                    RestoreCellVisual(x, y);
                }
            }
        }

        public void UpdateBoard()
        {
        }

        sealed class CellView : MonoBehaviour, IPointerDownHandler
        {
            Board _board;
            int _gx;
            int _gy;
            Image _borderImage;
            Image _innerImage;
            Text _text;

            public static CellView Create(Board board, RectTransform gridParent, int x, int y, Font font)
            {
                var root = new GameObject($"Cell_{x}_{y}", typeof(RectTransform));
                root.transform.SetParent(gridParent, false);

                var borderImg = root.AddComponent<Image>();
                borderImg.sprite = UiSpriteUtility.WhiteSprite;
                borderImg.type = Image.Type.Simple;
                borderImg.color = CellBorderColor;
                borderImg.raycastTarget = true;

                var innerGo = new GameObject("Inner", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                innerGo.transform.SetParent(root.transform, false);
                var innerRt = innerGo.GetComponent<RectTransform>();
                var innerImg = innerGo.GetComponent<Image>();
                innerImg.sprite = UiSpriteUtility.WhiteSprite;
                innerImg.type = Image.Type.Simple;
                innerImg.color = CellNormalColor;
                innerImg.raycastTarget = false;

                float inset = CellBorderPx;
                innerRt.anchorMin = Vector2.zero;
                innerRt.anchorMax = Vector2.one;
                innerRt.offsetMin = new Vector2(inset, inset);
                innerRt.offsetMax = new Vector2(-inset, -inset);

                var textGo = new GameObject("Char", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
                textGo.transform.SetParent(innerGo.transform, false);
                var textRt = textGo.GetComponent<RectTransform>();
                textRt.anchorMin = Vector2.zero;
                textRt.anchorMax = Vector2.one;
                textRt.offsetMin = Vector2.zero;
                textRt.offsetMax = Vector2.zero;

                var text = textGo.GetComponent<Text>();
                text.font = font;
                text.fontStyle = FontStyle.Bold;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 8;
                text.resizeTextMaxSize = 64;
                text.alignment = TextAnchor.MiddleCenter;
                text.color = NormalCellTextColor;
                text.raycastTarget = false;

                var cell = root.AddComponent<CellView>();
                cell._board = board;
                cell._gx = x;
                cell._gy = y;
                cell._borderImage = borderImg;
                cell._innerImage = innerImg;
                cell._text = text;
                return cell;
            }

            public void SetDisplayedCharacter(char c)
            {
                _text.text = (c == '\0' || char.IsWhiteSpace(c)) ? string.Empty : c.ToString();
                _text.color = _board.IsStarterCell(_gx, _gy) ? StarterCellTextColor : NormalCellTextColor;
            }

            public void SetDisplayedColourNotInDictionary()
            {
                _text.color = NotInDictionaryCellTextColor;
            }

            public void SetBackgroundColor(Color c)
            {
                _innerImage.color = c;
            }

            public void SetNormalAppearance()
            {
                SetNormalBackground();
                _text.color = NormalCellTextColor;
            }

            public void SetNormalBackground()
            {
                _borderImage.color = CellBorderColor;
                _innerImage.color = CellNormalColor;
            }

            public void SetStarterWordAppearance()
            {
                SetStarterWordBackground();
                _text.color = StarterCellTextColor;
            }
            
            public void SetStarterWordBackground()
            {
                _borderImage.color = StarterCellBorderColor;
                _innerImage.color = StarterCellInnerColor;
            }

            public void OnPointerDown(PointerEventData eventData)
            {
                _board.OnCellPointerDown(_gx, _gy);
            }
        }
    }

    static class UiSpriteUtility
    {
        static Sprite _whiteSprite;

        public static Sprite WhiteSprite
        {
            get
            {
                if (_whiteSprite == null)
                {
                    var tex = Texture2D.whiteTexture;
                    _whiteSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect);
                }
                return _whiteSprite;
            }
        }
    }

    static class UiFontUtility
    {
        static Font _uiFont;

        public static Font DefaultUIFont
        {
            get
            {
                if (_uiFont == null)
                {
                    _uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    if (_uiFont == null)
                        _uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    if (_uiFont == null)
                        _uiFont = Font.CreateDynamicFontFromOSFont("Arial", 24);
                }
                return _uiFont;
            }
        }
    }
}