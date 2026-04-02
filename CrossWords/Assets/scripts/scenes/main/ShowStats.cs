// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

namespace CrossWords {

    public class ShowStats : MonoBehaviour {
        public TextMeshProUGUI pointsAveText;
        public TextMeshProUGUI daysPlayedText;
        public TextMeshProUGUI daysPublishedText;
        public TextMeshProUGUI firstDayPlayedText;
        public TextMeshProUGUI firstDatePlayedText;
        public TextMeshProUGUI lastDayPlayedText;
        public TextMeshProUGUI lastDatePlayedText;

        public TextMeshProUGUI silverText;
        public TextMeshProUGUI silverLongestText;
        public TextMeshProUGUI goldText;
        public TextMeshProUGUI goldLongestText;
        public TextMeshProUGUI diamondText;
        public TextMeshProUGUI diamondLongestText;
        public TextMeshProUGUI blueDiamondText;
        public TextMeshProUGUI blueDiamondLongestText;

        private string help = "" +
            "Streaks are sequential days played.";

        public void Start() {
            AuditLog.Log("Stats screen");

            int firstPlayed = Stats.GetFirstDayPlayed();
            int lastPlayed = (int) Stats.GetLastGameDay();
            int timesPlayed = Stats.GetNumDaysPlayed();
            int timesPublished = Stats.GetNumTimesPublished();
            int averageScore = Stats.GetAverageScore();


            string firstPlayedS;
            string lastPlayedS;

            if (firstPlayed == 0) {
                firstPlayedS = "Never Played";
                lastPlayedS = "Never Played";
            }
            else {
                DateTime firstPlayedDate = Timeline.GetRelativeDate(firstPlayed);
                firstPlayedS = firstPlayedDate.ToString("D");

                DateTime lastPlayedDate = Timeline.GetRelativeDate(lastPlayed);
                lastPlayedS = lastPlayedDate.ToString("D");
            }

            pointsAveText.text = averageScore.ToString();
            daysPlayedText.text = timesPlayed.ToString();
            daysPublishedText.text = timesPublished.ToString();
            firstDayPlayedText.text = firstPlayed.ToString();
            firstDatePlayedText.text = firstPlayedS;
            lastDayPlayedText.text = lastPlayed.ToString();
            lastDatePlayedText.text = lastPlayedS;


            int silverLen;
            int goldLen;
            int diamondLen;
            int bdiamondLen;
            (silverLen, goldLen, diamondLen, bdiamondLen) = Stats.GetStreaksLengths();
            silverText.text = silverLen.ToString();
            goldText.text = goldLen.ToString();
            diamondText.text = diamondLen.ToString();
            blueDiamondText.text = bdiamondLen.ToString();

            (silverLen, goldLen, diamondLen, bdiamondLen) = Stats.GetLongestStreaksLengths();
            silverLongestText.text = silverLen.ToString();
            goldLongestText.text = goldLen.ToString();
            diamondLongestText.text = diamondLen.ToString();
            blueDiamondLongestText.text = bdiamondLen.ToString();
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Help") {
                // MessagePass.SetMsg(help);
                // SceneStack.Instance().PushScene();
                // SceneManager.LoadScene("HelpContextScene", LoadSceneMode.Additive);
            }
            else {
                AuditLog.Log($"Show Stats: Unknown button: {buttonText}");
            }
        }

    }
}