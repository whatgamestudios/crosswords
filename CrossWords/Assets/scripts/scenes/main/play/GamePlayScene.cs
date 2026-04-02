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

        readonly MoveStack _moveStack = new MoveStack();

        private bool dictionaryLoaded = false;

        void Awake()
        {
            if (board == null)
            {
                board = FindFirstObjectByType<Board>();
            }
        }

        void Start()
        {
            uint gameDay = Timeline.GameDay();
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
                    string starterWord = WordListTarget.GetTargetWord(gameDay);
                    board.SetStarterWord(starterWord);
                    analyseBoardAndUpdateScore();
                    _moveStack.LoadFromStorage();        
                    DisableLetterButtonsForUsedLetters();
                }
            }

            setGameState(gameDay);
            //await PassportLogin.InitAndLogin();

            Invoke("DelayedScoreUpdate", 0.1f);
        }
        
        public void OnDisable()
        {
            GameState.Instance().SetPlayerState(GameState.PlayerState.Unknown);
        }

        void DelayedScoreUpdate()
        {
            if (!dictionaryLoaded)
            {
                analyseBoardAndUpdateScore();
                Invoke("DelayedScoreUpdate", 0.1f);
            }
        }

        private void setGameState(uint todaysGameDay) {
            GameState gameState = GameState.Instance();
            gameState.SetGameDayBeingPlayed(todaysGameDay);
            //gameState.SetPointsEarnedTotal(pointsEarnedTotalToday());
            if (gameState.IsPlayerStateUnknown())
            {
                gameState.SetPlayerState(GameState.PlayerState.Playing);
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
                return;

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
        }

        private void resetBoard()
        {
            uint gameDay = Timeline.GameDay();
            string starterWord = WordListTarget.GetTargetWord(gameDay);
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

            uint score = 26;
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

                bool[] inDictionary = new bool[100];
                int i = 0;
                foreach (WordOnBoard word in words)
                {
                    bool inDic = wordListDictionary.IsInDictionary(word.Word);
                    AuditLog.Log($"Words: {word.Word} in dic: {inDic}");
                    inDictionary[i++] = inDic;
                }
                score = ScoreCalculator.Score(inDictionary, words);
                ScoreText.text = score.ToString();
            }

            string b = board.GetCells();
            uint gameDay = Timeline.GameDay();
            Stats.SetCurrent(gameDay, score, b);
        }

    }

}
