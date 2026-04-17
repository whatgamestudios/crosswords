// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

using CrossWords.WorcadianGameV1.ContractDefinition;

namespace CrossWords {

    public class SolutionsScreen : MonoBehaviour {
        // Control
        public TextMeshProUGUI gameDayText;
        public TextMeshProUGUI gameDateText;

        public Button buttonUp;
        public Button buttonDown;
        public Button buttonLeft;
        public Button buttonRight;


        private uint gameDayToday = 0;

        private uint gameDayDisplaying = 0;

        private int indexDisplaying = 0;

        public TextMeshProUGUI mySolutionScoreText;

        public TextMeshProUGUI bestSolutionScoreText;
        public TextMeshProUGUI bestPlayerText;

        private GetResultsOutputDTO todaysResult = null;

        public void Start() {
            AuditLog.Log("Solutions screen");
            uint gameDay = (uint) Timeline.GameDay();
            gameDayToday = gameDay;
            showNewDay(gameDay);
            buttonUp.interactable = false;

            buttonRight.interactable = false;
            buttonLeft.interactable = false;
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Up") {
                uint newDay = gameDayDisplaying + 1;
                buttonDown.interactable = true;
                if (newDay >= gameDayToday) {
                    buttonUp.interactable = false;
                }
                buttonRight.interactable = false;
                buttonLeft.interactable = false;
                showNewDay(newDay);
            }
            else if (buttonText == "Down") {
                uint newDay = gameDayDisplaying - 1;
                buttonUp.interactable = true;
                if (newDay == 0) {
                    buttonDown.interactable = false;
                }
                buttonRight.interactable = false;
                buttonLeft.interactable = false;
                showNewDay(newDay);
            }
            else if (buttonText == "Left") {
                int newIndex = indexDisplaying - 1;
                if (newIndex == 0) {
                    buttonLeft.interactable = false;
                }
                buttonRight.interactable = true;
                showSolution(newIndex);
            }
            else if (buttonText == "Right") {
                if (todaysResult == null) {
                    return;
                }
                if (todaysResult.NumSubmissions > 0) {
                    int newIndex = indexDisplaying + 1;
                    if (newIndex == todaysResult.NumSubmissions - 1) {
                        buttonRight.interactable = false;
                    }
                    buttonLeft.interactable = true;
                    showSolution(newIndex);
                }
            }
            else {
                AuditLog.Log($"Unknown button: {buttonText}");
            }
        }


        public void showNewDay(uint gameDay) {
            gameDayDisplaying = gameDay;
            indexDisplaying = 0;
            gameDayText.text = "" + gameDay;

            gameDateText.text = Timeline.GetRelativeDateString((int) gameDay);

            DisplayMyResult(gameDay);

            StartCoroutine(GetResultRoutine());
        }

        public void showSolution(int index) {
            indexDisplaying = index;
            showCached(gameDayDisplaying, indexDisplaying);
        }

        IEnumerator GetResultRoutine() {
            GetResult();
            yield return new WaitForSeconds(0f);
        }
        async void GetResult()
        {

            GameProcessor gameProcessor = new GameProcessor();
            todaysResult = await gameProcessor.GetResults(gameDayDisplaying);
            showCached(gameDayDisplaying, indexDisplaying);

            if (todaysResult.Submissions != null && todaysResult.Submissions.Count > 1)
            {
                buttonRight.interactable = true;
            }
        }

        public void showCached(uint gameDay, int index) {
            if (todaysResult == null) {
                AuditLog.Log("Todays result is null");
                return;
            }

            BestBoard board = FindFirstObjectByType<BestBoard>();
            if (board == null) {
                AuditLog.Log("BestBoard not found");
                return;
            }

            var submissions = todaysResult.Submissions;
            if (submissions == null || submissions.Count == 0) {
                if (bestPlayerText != null) {
                    bestPlayerText.text = "";
                }
                board.ResetAllCells();
                string seedWord = WordListSeed.GetSeedWord(gameDay);
                board.SetStarterWord(seedWord);
                board.BlockInteraction();
                int score = 26 - seedWord.Length;
                bestSolutionScoreText.text = $"Best Score: {score}";
                return;
            }

            if (index < 0 || index >= submissions.Count) {
                AuditLog.Log($"ERROR: showCached: index {index} out of range (count {submissions.Count})");
                return;
            }

            Submission sub = submissions[index];
            if (bestPlayerText != null) {
                string player = sub.Player ?? "";
                if (player.Length >= 10) {
                    bestPlayerText.text = player.Substring(0, 6) + "...." + player.Substring(player.Length - 4, 4);
                } else {
                    bestPlayerText.text = player;
                }
            }

            board.ResetAllCells();
            if (!string.IsNullOrEmpty(sub.Board)) {
                board.SetCells(sub.Board);
            }
            string starterWord = WordListSeed.GetSeedWord(gameDay);
            board.SetStarterWord(starterWord);
            board.BlockInteraction();
            bestSolutionScoreText.text = $"Best Score: {todaysResult.BestScore}";
        }

        void DisplayMyResult(uint gameDay) {
            MyBoard board = FindFirstObjectByType<MyBoard>();

            board.ResetAllCells();
            (bool exists, Solution sol) = Stats.GetSolution(gameDay);
            uint score = 21;
            if (exists)
            {
                board.SetCells(sol.BoardString);
                score = sol.Score;
            }
            string starterWord = WordListSeed.GetSeedWord(gameDay);
            board.SetStarterWord(starterWord);
            board.BlockInteraction();
            mySolutionScoreText.text = $"My Score: {score}";
        }
    }
}