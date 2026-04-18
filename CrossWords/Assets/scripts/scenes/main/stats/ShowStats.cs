// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace CrossWords {

    public class ShowStats : MonoBehaviour {
        public TextMeshProUGUI pointsAveText;
        public TextMeshProUGUI daysPlayedText;
        public TextMeshProUGUI daysPublishedText;
        public TextMeshProUGUI firstDayPlayedText;
        public TextMeshProUGUI firstDatePlayedText;

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
        }
    }
}