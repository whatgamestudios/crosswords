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
        public TextMeshProUGUI lastDayPlayedText;
        public TextMeshProUGUI lastDatePlayedText;

        [SerializeField] private RectTransform graphContainer;
        [SerializeField] private Sprite dotSprite;
        [SerializeField] private Font labelFont; // Assign Arial or similar in Inspector


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

            showGraph();
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Help") {
                MessagePass.SetMsg(help);
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("HelpContextScene", LoadSceneMode.Additive);
            }
            else {
                AuditLog.Log($"Show Stats: Unknown button: {buttonText}");
            }
        }


        private void showGraph()
        {
            uint maxScore = 20;
            uint[] scoreDistribution = new uint[maxScore];

            int firstPlayed = Stats.GetFirstDayPlayed();
            int lastPlayed = (int) Stats.GetLastGameDay();
            if ((firstPlayed != 0) && (lastPlayed != 0))
            {
                for (int i = firstPlayed; i <= lastPlayed; i++)
                {
                    (bool exists, Solution sol) = Stats.GetSolution((uint)i);
                    if (exists)
                    {
                        uint score = sol.Score;
                        if (score < maxScore)
                        {
                            scoreDistribution[score]++;
                        }
                    }
                }
            }

            // RectTransform graphContainer;
            // Sprite dotSprite;

            float graphHeight = graphContainer.sizeDelta.y;
            float graphWidth = graphContainer.sizeDelta.x;
            
            // Auto-scale Y: Find the max value in your list
            int yMax = 0;
            foreach (int val in scoreDistribution) {
                if (val > yMax) yMax = val;
            }
            yMax = Mathf.Max(yMax, 1); // Avoid division by zero

            float xSize = graphWidth / maxScore;


            // 1. Draw Y-Axis Labels (e.g., 5 separators)
            int separatorCount = 5;
            for (int i = 0; i <= separatorCount; i++) {
                float normalizedValue = i / (float)separatorCount;
                float yPos = normalizedValue * graphHeight;
                string labelText = Mathf.RoundToInt(normalizedValue * yMax).ToString();
                CreateLabel(new Vector2(-20, yPos), labelText, TextAnchor.MiddleRight);
            }

            // 2. Draw X-Axis Labels (0 to 21)
            for (int i = 0; i <= maxScore; i++) {
                float xPos = i * xSize;
                // Only draw every 3rd or 5th label if it looks cluttered
                if (i % 3 == 0) { 
                    CreateLabel(new Vector2(xPos, -20), i.ToString(), TextAnchor.UpperCenter);
                }
            }


            for (int i = 0; i < maxScore && i <= maxScore; i++)
            {
                float xPosition = i * xSize;
                // Scale Y relative to the container height and max value
                float yPosition = (scoreDistribution[i] / (float)yMax) * graphHeight;
                
                CreateDot(new Vector2(xPosition, yPosition));
            }
        }

        private void CreateLabel(Vector2 anchoredPosition, string text, TextAnchor anchor)
        {
            GameObject labelObj = new GameObject("Label", typeof(Text));
            labelObj.transform.SetParent(graphContainer, false);
            
            Text txt = labelObj.GetComponent<Text>();
            txt.text = text;
            txt.font = labelFont;
            txt.fontSize = 12;
            txt.color = Color.white;
            txt.alignment = anchor;

            RectTransform rect = labelObj.GetComponent<RectTransform>();
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(50, 20); // Large enough for the number
            rect.anchorMin = rect.anchorMax = new Vector2(0, 0);
        }

        private void CreateDot(Vector2 anchoredPosition)
        {
            GameObject dot = new GameObject("dot", typeof(Image));
            dot.transform.SetParent(graphContainer, false);
            dot.GetComponent<Image>().sprite = dotSprite;
            RectTransform rect = dot.GetComponent<RectTransform>();
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(8, 8);
            rect.anchorMin = rect.anchorMax = new Vector2(0, 0);
        }

        // private void CreateDot(Vector2 anchoredPosition)
        // {
        //     GameObject gameObject = new GameObject("dot", typeof(Image));
        //     gameObject.transform.SetParent(graphContainer, false);
        //     gameObject.GetComponent<Image>().sprite = dotSprite;
            
        //     RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        //     rectTransform.anchoredPosition = anchoredPosition;
        //     rectTransform.sizeDelta = new Vector2(10, 10);
        //     rectTransform.anchorMin = new Vector2(0, 0);
        //     rectTransform.anchorMax = new Vector2(0, 0);
        // }
    }
}