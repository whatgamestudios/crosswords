using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CrossWords { 

    public class GamePlayScene : MonoBehaviour
    {
        [SerializeField] Board board;

        public Button ButtonA;
        public Button ButtonB;
        public Button ButtonC;
        public Button ButtonD;
        public Button ButtonE;
        public Button ButtonF;
        public Button ButtonG;
        public Button ButtonH;
        public Button ButtonI;
        public Button ButtonJ;
        public Button ButtonK;
        public Button ButtonL;
        public Button ButtonM;
        public Button ButtonN;
        public Button ButtonO;
        public Button ButtonP;
        public Button ButtonQ;
        public Button ButtonR;
        public Button ButtonS;
        public Button ButtonT;
        public Button ButtonU;
        public Button ButtonV;
        public Button ButtonW;
        public Button ButtonX;
        public Button ButtonY;
        public Button ButtonZ;

        readonly MoveStack _moveStack = new MoveStack();

        void Awake()
        {
            if (board == null)
                board = FindFirstObjectByType<Board>();
        }

        void Start()
        {
            int gameDay = (int)Timeline.GameDay();
            string targetWord = TargetWords.GetTargetWord(gameDay);
            if (board != null)
                board.SetTargetWord(targetWord);

            WireLetterButtons();
            DisableLetterButtonsForTargetWord(targetWord);
        }

        void WireLetterButtons()
        {
            var buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var button in buttons)
            {
                string n = button.gameObject.name;
                if (n.Length != 4 || !n.StartsWith("But", StringComparison.Ordinal))
                    continue;

                char c = n[3];
                if (c < 'A' || c > 'Z')
                    continue;

                button.onClick.RemoveAllListeners();
                char letter = c;
                button.onClick.AddListener(() => OnLetterButton(button, letter));
            }
        }

        void DisableLetterButtonsForTargetWord(string word)
        {
            if (string.IsNullOrEmpty(word))
                return;

            var letters = new HashSet<char>();
            foreach (char c in word.ToUpperInvariant())
            {
                if (c >= 'A' && c <= 'Z')
                    letters.Add(c);
            }

            if (letters.Count == 0)
                return;

            var buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var button in buttons)
            {
                string n = button.gameObject.name;
                if (n.Length != 4 || !n.StartsWith("But", StringComparison.Ordinal))
                    continue;

                char c = n[3];
                if (c >= 'A' && c <= 'Z' && letters.Contains(c))
                    button.interactable = false;
            }
        }

        void OnLetterButton(Button button, char letter)
        {
            if (board == null || !board.TryGetSelectedCell(out int x, out int y))
                return;

            board.SetCell(x, y, letter);
            _moveStack.Add(letter, x, y);
            button.interactable = false;
        }

        public void OnBackSpaceButton()
        {
            bool successful = _moveStack.TryRemoveTop(out MoveEntry entry);
            if (successful)
            {
                switch(entry.Letter)
                {
                    case 'A': 
                        ButtonA.interactable = true;
                        break;
                    case 'B': 
                        ButtonB.interactable = true;
                        break;
                    case 'C': 
                        ButtonC.interactable = true;
                        break;
                    case 'D': 
                        ButtonD.interactable = true;
                        break;
                    case 'E': 
                        ButtonE.interactable = true;
                        break;
                    case 'F': 
                        ButtonF.interactable = true;
                        break;
                    case 'G': 
                        ButtonG.interactable = true;
                        break;
                    case 'H': 
                        ButtonH.interactable = true;
                        break;
                    case 'I': 
                        ButtonI.interactable = true;
                        break;
                    case 'J': 
                        ButtonJ.interactable = true;
                        break;
                    case 'K': 
                        ButtonK.interactable = true;
                        break;
                    case 'L': 
                        ButtonL.interactable = true;
                        break;
                    case 'M': 
                        ButtonM.interactable = true;
                        break;
                    case 'N': 
                        ButtonN.interactable = true;
                        break;
                    case 'O': 
                        ButtonO.interactable = true;
                        break;
                    case 'P': 
                        ButtonP.interactable = true;
                        break;
                    case 'Q': 
                        ButtonQ.interactable = true;
                        break;
                    case 'R': 
                        ButtonR.interactable = true;
                        break;
                    case 'S': 
                        ButtonS.interactable = true;
                        break;
                    case 'T': 
                        ButtonT.interactable = true;
                        break;
                    case 'U': 
                        ButtonU.interactable = true;
                        break;
                    case 'V': 
                        ButtonV.interactable = true;
                        break;
                    case 'W': 
                        ButtonW.interactable = true;
                        break;
                    case 'X': 
                        ButtonX.interactable = true;
                        break;
                    case 'Y': 
                        ButtonY.interactable = true;
                        break;
                    case 'Z': 
                        ButtonZ.interactable = true;
                        break;
                }
                board.ResetCell(entry.X, entry.Y);
            }

        }


        public void OnClearButton()
        {
            int size = _moveStack.Count;
            for (int i = 0; i < size; i++)
            {
                OnBackSpaceButton();
            }
        }        

        void Update()
        {
            if (board != null)
                board.UpdateBoard();
        }
    }
}
