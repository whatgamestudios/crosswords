using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CrossWords { 

    public class HowToPlayScene : MonoBehaviour
    {
        Board board;

        public TextMeshProUGUI ScoreText;

        public TextMeshProUGUI StatusText1;
        public TextMeshProUGUI StatusText2;
        public TextMeshProUGUI StatusText3;
        public TextMeshProUGUI StatusText4;

        public GameObject InfoPanel;
        public TextMeshProUGUI InfoText;


        private bool dictionaryLoaded = false;

        public uint Score = 26;


        private uint TimeInShow;

        private uint GameDay;

        private const int TIME_MOVE = 250;
        private DateTime timeOfLastMove = DateTime.Now;


        private uint Delay;
        private uint NextMove;

        void Awake()
        {
            if (board == null)
            {
                board = FindFirstObjectByType<Board>();
            }
        }

        void Start()
        {
            AuditLog.Log($"How to Play screen");
            TimeInShow = 0;
            GameDay = 12;
            InfoText.text = "";
            InfoPanel.SetActive(false);
            Delay = 0;
            NextMove = 1;

            resetBoard();


            Invoke("DelayedScoreUpdate", 0.1f);
        }
        
        void Update()
        {
            if (board != null)
            {
                board.UpdateBoard();
            }
            showStatus();


            DateTime now = DateTime.Now;
            if ((now - timeOfLastMove).TotalMilliseconds < TIME_MOVE)
            {
                return;
            }
            timeOfLastMove = now;

            AuditLog.Log($"Delay: {Delay}, NextMove: {NextMove}");

            if (Delay++ < NextMove)
            {
                AuditLog.Log($"zzDelay: {Delay}, NextMove: {NextMove}");
                return;
            }
            Delay = 0;

            AuditLog.Log($"Time in show: {TimeInShow}");
            switch (TimeInShow++)
            {
                case 0:
                    resetBoard();
                    NextMove = 0;
                    break;
                case 1:
                    InfoText.text = "Buid words off the seed word";
                    InfoPanel.SetActive(true);
                    NextMove = 8;
                    break;
                case 2:
                    InfoPanel.SetActive(false);
                    NextMove = 1;
                    break;
                case 3:
                    InfoText.text = "Select a cell";
                    InfoPanel.SetActive(true);
                    NextMove = 1;
                    break;
                case 4:
                    SelectCell(4,4);
                    NextMove = 4;
                    break;
                case 5:
                    InfoPanel.SetActive(false);
                    NextMove = 1;
                    break;
                case 6:
                    InfoText.text = "Then choose a letter";
                    InfoPanel.SetActive(true);
                    NextMove = 4;
                    break;
                case 7:
                    OnLetterButton('L');
                    NextMove = 8;
                    break;
                case 8:
                    InfoPanel.SetActive(false);
                    NextMove = 1;
                    break;
                case 9:
                    InfoText.text = "Create words";
                    InfoPanel.SetActive(true);
                    NextMove = 3;
                    break;

                case 10:
                    SelectCell(4,6);
                    NextMove = 0;
                    break;
                case 11:
                    OnLetterButton('M');
                    NextMove = 0;
                    break;
                case 12:
                    SelectCell(4,7);
                    NextMove = 0;
                    break;
                case 13:
                    OnLetterButton('P');
                    NextMove = 0;
                    InfoPanel.SetActive(false);
                    break;

                case 14:
                    SelectCell(0,7);
                    NextMove = 0;
                    break;
                case 15:
                    OnLetterButton('C');
                    NextMove = 0;
                    break;
                case 16:
                    SelectCell(1,7);
                    NextMove = 0;
                    break;
                case 17:
                    OnLetterButton('H');
                    NextMove = 0;
                    break;
                case 18:
                    SelectCell(2,7);
                    NextMove = 0;
                    break;
                case 19:
                    OnLetterButton('I');
                    NextMove = 0;
                    break;
                case 20:
                    SelectCell(3,7);
                    NextMove = 0;
                    break;
                case 21:
                    OnLetterButton('R');
                    NextMove = 0;
                    break;
                case 22:
                    SelectCell(2,6);
                    NextMove = 0;
                    break;
                case 23:
                    OnLetterButton('V');
                    NextMove = 0;
                    break;
                case 24:
                    SelectCell(2,8);
                    NextMove = 0;
                    break;
                case 25:
                    OnLetterButton('Z');
                    NextMove = 0;
                    break;
                case 26:
                    SelectCell(2,9);
                    NextMove = 0;
                    break;
                case 27:
                    OnLetterButton('Y');
                    NextMove = 0;
                    break;

                case 28:
                    InfoText.text = "Words not in the wordlist";
                    InfoPanel.SetActive(true);
                    NextMove = 8;
                    break;
                case 29:
                    InfoText.text = "are shown in red";
                    NextMove = 8;
                    break;
                case 30:
                    InfoPanel.SetActive(false);
                    NextMove = 0;
                    break;
                case 31:
                    InfoText.text = "Use backspace to remove";
                    InfoPanel.SetActive(true);
                    NextMove = 4;
                    break;
                case 32:
                    InfoText.text = "the last letter";
                    NextMove = 4;
                    break;
                case 33:
                    InfoPanel.SetActive(false);
                    NextMove = 8;
                    ResetLetterButton(2,9);
                    break;
                case 34:
                    InfoText.text = "The more letters";
                    InfoPanel.SetActive(true);
                    NextMove = 4;
                    break;
                case 35:
                    InfoText.text = "you use";
                    NextMove = 4;
                    break;
                case 36:
                    InfoText.text = "the lower your score";
                    NextMove = 4;
                    break;
                case 37:
                    InfoPanel.SetActive(false);
                    NextMove = 4;
                    break;

                case 38:
                    InfoText.text = "Use the clear button ¢";
                    InfoPanel.SetActive(true);
                    NextMove = 4;
                    break;
                case 39:
                    InfoText.text = "to reset the board";
                    NextMove = 4;
                    break;
                case 40:
                    resetBoard();
                    NextMove = 4;
                    break;
                case 41:
                    InfoPanel.SetActive(false);
                    NextMove = 4;
                    break;

                case 42:
                    InfoText.text = "There is a different";
                    InfoPanel.SetActive(true);
                    NextMove = 4;
                    break;
                case 43:
                    InfoText.text = "seed word each day";
                    NextMove = 4;
                    GameDay = 13;
                    resetBoard();
                    break;
                case 44:
                    NextMove = 4;
                    GameDay = 14;
                    resetBoard();
                    break;
                case 45:
                    NextMove = 4;
                    GameDay = 15;
                    resetBoard();
                    break;
                case 46:
                    NextMove = 4;
                    GameDay = 16;
                    resetBoard();
                    break;

                case 47:
                    InfoPanel.SetActive(false);
                    NextMove = 0;
                    break;



                case 48:
                    TimeInShow = 0;
                    NextMove = 10;
                    InfoPanel.SetActive(false);

                    GameDay = 12;
                    resetBoard();

                    break;

                default:
                    // Do nothing.
                    break;
            }
        }


        void DelayedScoreUpdate()
        {
            if (!dictionaryLoaded)
            {
                analyseBoardAndUpdateScore();
                Invoke("DelayedScoreUpdate", 0.1f);
            }
        }

        void DisableLetterButtonsForUsedLetters()
        {
            if (board == null)
                return;

            string letters = board.GetCells();

            var buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var button in buttons)
            {
                string n = button.gameObject.name;
                if (n.Length != 4 || !n.StartsWith("But", StringComparison.Ordinal))
                {
                    continue;
                }

                char c = n[3];
                if (c >= 'A' && c <= 'Z')
                {
                    button.interactable = (letters.Contains(c) == false);
                }
            }
        }

        void SelectCell(int x, int y)
        {
            board.FakeOnCellPointerDown(x, y);
        }

        void OnLetterButton(char letter)
        {
            board.TryGetSelectedCell(out int x, out int y);
            board.SetCell(x, y, letter);

            analyseBoardAndUpdateScore();
            DisableLetterButtonsForUsedLetters();
        }

        public void ResetLetterButton(int x, int y) {
            board.SetCell(x, y, '\0');

            analyseBoardAndUpdateScore();
            DisableLetterButtonsForUsedLetters();
        }



        private void resetBoard()
        {
            string starterWord = WordListSeed.GetSeedWord(GameDay);
            AuditLog.Log($"Loaded word: {starterWord}");
            if (board != null)
            {
                board.ResetAllCells();
                board.SetStarterWord(starterWord);
            }        
            analyseBoardAndUpdateScore();
            DisableLetterButtonsForUsedLetters();
        }

        void analyseBoardAndUpdateScore()
        {

            List<WordOnBoard> words = AnalyseBoard.Analyse(board);

            Score = 26;
            WordListDictionary wordListDictionary = GetComponent<WordListDictionary>();
            if (wordListDictionary == null)
            {
                AuditLog.Log("ERROR: No dictionary");
                ScoreText.text = "?";
            }
            else if (!wordListDictionary.DictionaryLoaded)
            {
                AuditLog.Log("ERROR: Dictionary not loaded");
                ScoreText.text = "?";
            }
            else
            {
                dictionaryLoaded = true;

                board.RestoreAllCellsVisual();
                int i = 0;
                bool[] inDictionary = new bool[100];
                foreach (WordOnBoard word in words)
                {
                    bool inDic = wordListDictionary.IsInDictionary(word.Word);
                    //AuditLog.Log($"Words: {word.Word} in dic: {inDic}");
                    inDictionary[i++] = inDic;
                    if (!inDic)
                    {
                        board.HighlightNotInDictionaryCells(word.StartX, word.StartY, word.Length(), word.IsHorizontal());
                    }
                }
                Score = ScoreCalculator.Score(inDictionary, words);
                ScoreText.text = Score.ToString();

                string b = board.GetCells();
                uint gameDay = Timeline.GameDay();
                Stats.SetCurrent(gameDay, Score, b);
            }
        }


        void showStatus()
        {
            // if not scrolling

            Vector2 pos = StatusText1.rectTransform.anchoredPosition;
            pos.y = -35;
            StatusText1.rectTransform.anchoredPosition = pos;
            StatusText1.color = Color.black;

            pos = StatusText2.rectTransform.anchoredPosition;
            pos.y = -75;
            StatusText2.rectTransform.anchoredPosition = pos;
            StatusText2.color = Color.black;

            StatusText3.gameObject.SetActive(false);
            StatusText4.gameObject.SetActive(false);

            string status1 = "";
            string status2 = "";
            if (Score >= 20)
            {
                status1 = "Start from the";
                status2 = "seed word";
            }
            else
            {
                switch (Score)
                {
                    // case 20:
                    //     status1 = "Good start";
                    //     break;
                    case 19:
                        status1 = "Great start";
                        break;
                    case 18:
                        status1 = "Doing well";
                        break;
                    case 17:
                        status1 = "What next?";
                        break;
                    case 16:
                        status1 = "Getting there";
                        break;
                    case 15:
                        status1 = "What is a";
                        status2 = "word?";
                        break;
                    case 14:
                        status1 = "You can do";
                        status2 = "this";
                        break;
                    case 13:
                        status1 = "Great";
                        break;
                    case 12:
                        status1 = "You have got";
                        status2 = "this";
                        break;
                    case 11:
                        status1 = "What next";
                        break;
                    case 10:
                        status1 = "Very good!";
                        break;
                    case 9:
                        status1 = "How to use";
                        status2 = "last letters?";
                        break;
                    case 8:
                        status1 = "Doing well!";
                        break;
                    case 7:
                        status1 = "Riding high";
                        break;
                    case 6:
                        status1 = "Today is a";
                        status2 = "good day";
                        break;
                    case 5:
                        status1 = "Today is a";
                        status2 = "great day";
                        break;
                    case 4:
                        status1 = "Awesome";
                        break;
                    case 3:
                        status1 = "Exceptional";
                        break;
                    case 2:
                        status1 = "You are a";
                        status2 = "star!";
                        break;
                    case 1:
                        status1 = "You are a";
                        status2 = "super star!";
                        break;
                    case 0:
                        status1 = "You have";
                        status2 = "arrived.";
                        break;
                }
            }


            
            StatusText1.text = status1;
            StatusText2.text = status2;
        }

    }
}
