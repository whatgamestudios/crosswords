using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CrossWords {

    public class ReplayManager : MonoBehaviour
    {
        Board board;

        public TextMeshProUGUI ScoreText;

        public TextMeshProUGUI StatusText1;
        public TextMeshProUGUI StatusText2;
        public TextMeshProUGUI StatusText3;
        public TextMeshProUGUI StatusText4;

        public GameObject ReplayPanel;
        public GameObject ReplayBlockInteractionPanel;

        public uint Score = 26;



        private List<MoveEntry> _replayMoves;
        private int _replayIndex;
        private DateTime _timeOfLastMove = DateTime.Now;
        private bool _selectCell;

        private const int TIME_MOVE = 400;

        public bool IsReplaying { get; private set; }

        void Awake()
        {
            if (board == null)
            {
                board = FindFirstObjectByType<Board>();
            }
        }


        public void OnReplayButtonPress() {
            GamePlayScene gamePlay = FindFirstObjectByType<GamePlayScene>();
            StartReplay(gamePlay.moveStack);
            
        }

        public void StartReplay(MoveStack moveStack)
        {
            _replayMoves = moveStack.GetEntriesBottomToTop();
            _replayIndex = 0;
            _selectCell = true;
            IsReplaying = true;
            _timeOfLastMove = DateTime.Now;
            ReplayPanel.SetActive(false);
            resetBoard();
        }

        void Update()
        {
            ReplayBlockInteractionPanel.SetActive(IsReplaying);

            if (!IsReplaying) {
                GamePlayScene gamePlay = FindFirstObjectByType<GamePlayScene>();
                bool shouldBeActive = gamePlay.moveStack.Count > 2;
                ReplayPanel.SetActive(shouldBeActive);
                return;
            }

            DateTime now = DateTime.Now;
            if ((now - _timeOfLastMove).TotalMilliseconds < TIME_MOVE) {
                return;
            }
            _timeOfLastMove = now;

            if (_replayIndex >= _replayMoves.Count)
            {
                IsReplaying = false;
                ReplayPanel.SetActive(true);
                return;
            }

            MoveEntry entry = _replayMoves[_replayIndex];
            if (_selectCell) {
                board.FakeOnCellPointerDown(entry.X, entry.Y);
                _selectCell = false;
            }
            else {
                board.SetCell(entry.X, entry.Y, entry.Letter);
                _replayIndex++;
                _selectCell = true;

                analyseBoardAndUpdateScore();
                showStatus();
                DisableLetterButtonsForUsedLetters();
            }
        }


        private void resetBoard()
        {
            uint gameDay = Timeline.GameDay();
            string starterWord = WordListSeed.GetSeedWord(gameDay);
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

    }

}
