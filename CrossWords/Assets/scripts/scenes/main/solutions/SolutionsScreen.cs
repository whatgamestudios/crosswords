// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

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

        private BoardResultsResult todaysResult = null;

        private Coroutine loadingAnimation;

        public void Start() {
            AuditLog.Log("Solutions screen");
            uint gameDay = (uint) Timeline.GameDay();
            gameDayToday = gameDay;
            Invoke("DelayedInitialLoad", 0.1f);
            buttonUp.interactable = false;
            buttonRight.interactable = false;
            buttonLeft.interactable = false;
        }

        // Run the initial load slightly after the scene loads to give the dictionary long enough to load. 
        private void DelayedInitialLoad()
        {
            showNewDay(gameDayToday);
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
                if (todaysResult == null || todaysResult.Submissions == null) {
                    return;
                }
                int submissionCount = todaysResult.Submissions.Length;
                if (submissionCount > 0) {
                    int newIndex = indexDisplaying + 1;
                    if (newIndex == submissionCount - 1) {
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
        IEnumerator LoadingAnimationRoutine() {
            string[] frames = { "LOADING.", "LOADING..", "LOADING..." };
            int i = 0;
            while (true) {
                bestSolutionScoreText.text = frames[i % 3];
                i++;
                yield return new WaitForSeconds(1f);
            }
        }

        async void GetResult()
        {
            if (loadingAnimation != null) {
                StopCoroutine(loadingAnimation);
            }
            loadingAnimation = StartCoroutine(LoadingAnimationRoutine());
            BoardServerProcessor boardProcessor = new BoardServerProcessor();
            todaysResult = await boardProcessor.GetResults((int)gameDayDisplaying);
            StopCoroutine(loadingAnimation);
            loadingAnimation = null;
            showCached(gameDayDisplaying, indexDisplaying);

            if (todaysResult.Submissions != null && todaysResult.Submissions.Length > 1)
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
            if (submissions == null || submissions.Length == 0) {
                board.ResetAllCells();
                string seedWord = WordListSeed.GetSeedWord(gameDay);
                board.SetStarterWord(seedWord);
                board.BlockInteraction();
                int score = 26 - seedWord.Length;
                bestSolutionScoreText.text = $"Best Score: {score}";
                return;
            }

            if (index < 0 || index >= submissions.Length) {
                AuditLog.Log($"ERROR: showCached: index {index} out of range (count {submissions.Length})");
                return;
            }

            BoardSubmission sub = submissions[index];
            if (!string.IsNullOrEmpty(sub.Board)) {
                board.SetCells(sub.Board);
            }
            string starterWord = WordListSeed.GetSeedWord(gameDay);
            // Ignore return values.
            BoardHighlighting.Highlight((Board)board, starterWord);
            board.BlockInteraction();
            bestSolutionScoreText.text = $"Best Score: {todaysResult.BestScore?.ToString() ?? "?"}";
        }

        void DisplayMyResult(uint gameDay) {
            MyBoard board = FindFirstObjectByType<MyBoard>();
            (bool exists, Solution sol) = Stats.GetSolution(gameDay);           
            string starterWord = WordListSeed.GetSeedWord(gameDay);
            uint score;
            AuditLog.Log($"My result exists: {exists}");

            if (exists)
            {
                AuditLog.Log($"My result: {sol.BoardString}");
                board.SetCells(sol.BoardString);
                score = sol.Score;
            }
            else 
            {
                board.RestoreAllCellsVisual();
                score = (uint)(26 - starterWord.Length);
            }

            // Ignore return values.
            BoardHighlighting.Highlight((Board)board, starterWord);
            board.BlockInteraction();
            mySolutionScoreText.text = $"My Score: {score}";
        }
    }
}