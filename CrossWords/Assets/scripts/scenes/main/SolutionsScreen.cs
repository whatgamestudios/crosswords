// Copyright (c) Whatgame Studios 2024 - 2025
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

        // Output
        public TextMeshProUGUI targetText;

        public TextMeshProUGUI bestInput1Text;
        public TextMeshProUGUI bestCalculated1Text;
        public TextMeshProUGUI bestPoints1Text;
        public TextMeshProUGUI bestInput2Text;
        public TextMeshProUGUI bestCalculated2Text;
        public TextMeshProUGUI bestPoints2Text;
        public TextMeshProUGUI bestInput3Text;
        public TextMeshProUGUI bestCalculated3Text;
        public TextMeshProUGUI bestPoints3Text;
        public TextMeshProUGUI bestPointsTotalText;
        public TextMeshProUGUI bestPlayerText;


        public TextMeshProUGUI playerInput1Text;
        public TextMeshProUGUI playerCalculated1Text;
        public TextMeshProUGUI playerPoints1Text;
        public TextMeshProUGUI playerInput2Text;
        public TextMeshProUGUI playerCalculated2Text;
        public TextMeshProUGUI playerPoints2Text;
        public TextMeshProUGUI playerInput3Text;
        public TextMeshProUGUI playerCalculated3Text;
        public TextMeshProUGUI playerPoints3Text;
        public TextMeshProUGUI playerPointsTotalText;



        private uint gameDayToday = 0;

        private uint gameDayDisplaying = 0;

        private int indexDisplaying = 0;

//TODO        private GetAllSolutionsOutputDTO todaysResult = null;

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
                // int newIndex = indexDisplaying + 1;
                // if (newIndex == todaysResult.Solutions.Count - 1) {
                //     buttonRight.interactable = false;
                // }
                // buttonLeft.interactable = true;
                // showSolution(newIndex);
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

            targetText.text = "TODO";
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
            // FourteenNumbersSolutionsContract fourteenNumbersContracts = new FourteenNumbersSolutionsContract();
            // todaysResult = await fourteenNumbersContracts.GetAllSolutions(gameDayDisplaying);
            // showCached(gameDayDisplaying, indexDisplaying);

            // if (todaysResult.Solutions.Count > 1)
            // {
            //     buttonRight.interactable = true;
            // }
        }

        public void showCached(uint gameDay, int index) {
            // if (todaysResult == null) {
            //     AuditLog.Log("Todays result is null");
            //     return;
            // }

            // if (todaysResult.Solutions.Count != 0)
            // {
            //     string player = todaysResult.Solutions[index].Player;
            //     bestPlayerText.text = player.Substring(0, 6) + "...." + player.Substring(player.Length - 4, 4);
            // }
            // else
            // {
            //     bestPlayerText.text = "";
            // }
            // bestPointsTotalText.text = todaysResult.Points.ToString();

            // byte[] combinedSolutionBytes = {};
            // if (todaysResult.Solutions.Count != 0) {
            //     combinedSolutionBytes = todaysResult.Solutions[index].CombinedSolution;
            // }
            // var combinedSolution = System.Text.Encoding.Default.GetString(combinedSolutionBytes);
            // string sol1 = "";
            // string sol2 = "";
            // string sol3 = "";
            // if (combinedSolution.Length != 0) {
            //     int indexOfEquals = combinedSolution.IndexOf('=');
            //     sol1 = combinedSolution.Substring(0, indexOfEquals);
            //     combinedSolution = combinedSolution.Substring(indexOfEquals+1);
            //     indexOfEquals = combinedSolution.IndexOf('=');
            //     sol2 = combinedSolution.Substring(0, indexOfEquals);
            //     sol3 = combinedSolution.Substring(indexOfEquals+1);
            // }

            // if (gameDayDisplaying == gameDayToday) {
            //     bestInput1Text.text = replace(sol1);
            //     bestInput2Text.text = replace(sol2);
            //     bestInput3Text.text = replace(sol3);
            // }
            // else {
            //     bestInput1Text.text = replace(sol1, true);
            //     bestInput2Text.text = replace(sol2, true);
            //     bestInput3Text.text = replace(sol3, true);
            // }

            // uint points1 = 0;
            // uint points2 = 0;
            // uint points3 = 0;
            // CalcProcessor processor = new CalcProcessor();
            // uint targetValue = TargetValue.GetTarget(gameDayDisplaying);
            // int errorCode;
            // int res1 = 0;
            // int res2 = 0;
            // int res3 = 0;
            // if (sol1.Length != 0) {
            //     (res1, errorCode) = processor.Calc(sol1);
            //     if (errorCode == CalcProcessor.ERR_NO_ERROR) {
            //         points1 = Points.CalcPoints((uint) res1, targetValue);
            //     }

            // }
            // if (sol2.Length != 0) {
            //     (res2, errorCode) = processor.Calc(sol2);
            //     if (errorCode == CalcProcessor.ERR_NO_ERROR) {
            //         points2 = Points.CalcPoints((uint) res2, targetValue);
            //     }
            // }
            // if (sol3.Length != 0) {
            //     (res3, errorCode) = processor.Calc(sol3);
            //     if (errorCode == CalcProcessor.ERR_NO_ERROR) {
            //         points3 = Points.CalcPoints((uint) res3, targetValue);
            //     }
            // }
            // bestCalculated1Text.text = res1.ToString();
            // bestCalculated2Text.text = res2.ToString();
            // bestCalculated3Text.text = res3.ToString();

            // bestPoints1Text.text = points1.ToString();
            // bestPoints2Text.text = points2.ToString();
            // bestPoints3Text.text = points3.ToString();
        }

        void DisplayMyResult(uint gameDay) {

            // var combinedSolution = Stats.GetCombinedSolution(gameDay);
            // string sol1 = "";
            // string sol2 = "";
            // string sol3 = "";
            // if (combinedSolution.Length != 0) {
            //     int indexOfEquals = combinedSolution.IndexOf('=');
            //     sol1 = combinedSolution.Substring(0, indexOfEquals);
            //     combinedSolution = combinedSolution.Substring(indexOfEquals+1);
            //     indexOfEquals = combinedSolution.IndexOf('=');
            //     sol2 = combinedSolution.Substring(0, indexOfEquals);
            //     sol3 = combinedSolution.Substring(indexOfEquals+1);
            // }

            // playerInput1Text.text = replace(sol1, true);
            // playerInput2Text.text = replace(sol2, true);
            // playerInput3Text.text = replace(sol3, true);

            // uint points1 = 0;
            // uint points2 = 0;
            // uint points3 = 0;
            // CalcProcessor processor = new CalcProcessor();
            // uint targetValue = TargetValue.GetTarget(gameDay);
            // int errorCode;
            // int res1 = 0;
            // int res2 = 0;
            // int res3 = 0;
            // if (sol1.Length != 0) {
            //     (res1, errorCode) = processor.Calc(sol1);
            //     if (errorCode == CalcProcessor.ERR_NO_ERROR) {
            //         points1 = Points.CalcPoints((uint) res1, targetValue);
            //     }

            // }
            // if (sol2.Length != 0) {
            //     (res2, errorCode) = processor.Calc(sol2);
            //     if (errorCode == CalcProcessor.ERR_NO_ERROR) {
            //         points2 = Points.CalcPoints((uint) res2, targetValue);
            //     }
            // }
            // if (sol3.Length != 0) {
            //     (res3, errorCode) = processor.Calc(sol3);
            //     if (errorCode == CalcProcessor.ERR_NO_ERROR) {
            //         points3 = Points.CalcPoints((uint) res3, targetValue);
            //     }
            // }
            // playerCalculated1Text.text = res1.ToString();
            // playerCalculated2Text.text = res2.ToString();
            // playerCalculated3Text.text = res3.ToString();

            // playerPoints1Text.text = points1.ToString();
            // playerPoints2Text.text = points2.ToString();
            // playerPoints3Text.text = points3.ToString();

            // playerPointsTotalText.text = (points1 + points2 + points3).ToString();
        }
    }
}