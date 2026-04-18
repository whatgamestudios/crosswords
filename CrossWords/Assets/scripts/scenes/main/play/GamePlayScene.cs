using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CrossWords { 

    public class GamePlayScene : MonoBehaviour
    {
        Board board;

        public TextMeshProUGUI TimeToNextText;
        public TextMeshProUGUI GameDayText;

        public TextMeshProUGUI ScoreText;

        public TextMeshProUGUI StatusText1;
        public TextMeshProUGUI StatusText2;
        public TextMeshProUGUI StatusText3;
        public TextMeshProUGUI StatusText4;

        readonly MoveStack _moveStack = new MoveStack();

        private bool dictionaryLoaded = false;

        public uint Score = 26;

        public uint GameDayBeingPlayed = 0;

        private const int TIME_STATUS_MOVE = 40;
        private DateTime timeOfStatusMove = DateTime.Now;

        void Awake()
        {
            if (board == null)
            {
                board = FindFirstObjectByType<Board>();
            }
        }

        async void Start()
        {
            uint gameDay = Timeline.GameDay();
            GameDayBeingPlayed = gameDay;
            AuditLog.Log($"Game Play screen for day {gameDay}");

            WireLetterButtons();

            uint lastPlayedGameDay = Stats.GetLastGameDay();
            if (lastPlayedGameDay != gameDay)
            {
                AuditLog.Log($"Last game day {lastPlayedGameDay}");
                resetBoard();
            }
            else
            {
                (bool exists, Solution sol) = Stats.GetCurrent();
                if (!exists)
                {
                    AuditLog.Log($"Solution for today does not exist {gameDay}");
                    resetBoard();
                }
                else
                {
                    AuditLog.Log($"Loaded today's solution: {sol.BoardString}");
                    board.ResetAllCells();
                    board.SetCells(sol.BoardString);
                    string starterWord = WordListSeed.GetSeedWord(gameDay);
                    board.SetStarterWord(starterWord);
                    analyseBoardAndUpdateScore();
                    _moveStack.LoadFromStorage();        
                    DisableLetterButtonsForUsedLetters();
                }
            }

            await PassportLogin.InitAndLogin();

            Invoke("DelayedScoreUpdate", 0.1f);
        }
        
        void DelayedScoreUpdate()
        {
            if (!dictionaryLoaded)
            {
                analyseBoardAndUpdateScore();
                Invoke("DelayedScoreUpdate", 0.1f);
            }
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

        void OnLetterButton(Button button, char letter)
        {
            if (board == null || !board.TryGetSelectedCell(out int x, out int y))
            {
                return;
            }

            if (board.IsCellOccupied(x, y))
            {
                char previousChar = board.GetCell(x, y);
                bool found = _moveStack.Remove(previousChar);
                if (!found)
                {
                    AuditLog.Log($"Replacing character <{previousChar}> not found");
                } 
                else
                {
                    GameObject btnObject = GameObject.Find($"But{previousChar}");
                    if (btnObject != null)
                    {
                        Button prevButton = btnObject.GetComponent<Button>();
                        prevButton.interactable = true;
                    }
                }
            }

            board.SetCell(x, y, letter);
            _moveStack.Add(letter, x, y);
            button.interactable = false;

            analyseBoardAndUpdateScore();
        }

        public void OnBackSpaceButton()
        {
            bool successful = _moveStack.TryRemoveTop(out MoveEntry entry);
            if (successful)
            {
                board.ResetCell(entry.X, entry.Y);
            }
            analyseBoardAndUpdateScore();
            DisableLetterButtonsForUsedLetters();
        }


        public void OnClearButton()
        {
            resetBoard();
        }        

        void Update()
        {
            GameDayText.text = Timeline.GameDayStr();
            TimeToNextText.text = Timeline.TimeToNextDayStr();

            if (board != null)
                board.UpdateBoard();

            showStatus();
        }

        private void resetBoard()
        {
            uint gameDay = Timeline.GameDay();
            string starterWord = WordListSeed.GetSeedWord(gameDay);
            AuditLog.Log($"Loaded word: {starterWord}");
            if (board != null)
            {
                board.ResetAllCells();
                board.SetStarterWord(starterWord);
            }        
            _moveStack.Clear();                  
            analyseBoardAndUpdateScore();
            DisableLetterButtonsForUsedLetters();
        }

        void analyseBoardAndUpdateScore()
        {

            List<WordOnBoard> words = AnalyseBoard.Analyse(board);
            // AuditLog.Log("Words:");
            // foreach (WordOnBoard word in words)
            // {
            //     AuditLog.Log(word.Word);
            // }

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


        // Use code below for scrolling text

            // DateTime now = DateTime.Now;
            // if ((now - timeOfStatusMove).TotalMilliseconds > TIME_STATUS_MOVE) {
            //     timeOfStatusMove = now;

                // updateStatusText(StatusText1);
                // updateStatusText(StatusText2);
                // updateStatusText(StatusText3);
                // updateStatusText(StatusText4);


        // private void updateStatusText(TextMeshProUGUI tmp)
        // {
        //     bool rollover = false;

        //     Vector2 pos = tmp.rectTransform.anchoredPosition;
        //     float ypos = pos.y + 1;
        //     if (ypos > 20)
        //     {
        //         ypos = -120;
        //         rollover = true;
        //     }
        //     pos.y = ypos;
        //     tmp.rectTransform.anchoredPosition = pos;

        //     if (rollover)
        //     {
        //         if (words.Count != 0)
        //         {
        //             statusWordOffset = (statusWordOffset + 1) % words.Count;
        //         }
        //         tmp.text = words[statusWordOffset].Word;
        //         tmp.color = inDictionary[statusWordOffset] ? new Color(0.13f, 0.55f, 0.13f) : Color.red;
        //     }
        // }
    }
}
