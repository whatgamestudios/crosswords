// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace CrossWords {

    public class StatsGraph : MonoBehaviour {
        [SerializeField] private RectTransform graphContainer;
        [SerializeField] private Sprite dotSprite;
        public Font labelFont;

        private const uint MAX_SCORE = 20;
        private const uint MAX_NUM_Y_TICKS = 5;

        private const uint MAX_SCORE_LABEL_WIDTH = 50;
        private const uint MAX_SCORE_LABEL_HEIGHT = 20;

        private const uint X_LEFT_OFFSET = 80;
        private const uint X_RIGHT_OFFSET = 50;
        private const uint Y_TOP_OFFSET = 70;
        private const uint Y_BOTTOM_OFFSET = 40;

        private const float BAR_LINE_WIDTH = 15f;

        private uint[] scoreDistribution;


        public void Start() {
            AuditLog.Log("Stats graph screen");
            createScoreDistribution();
            //createDummyScoreDistribution();


            showGraph();
        }


        private void createScoreDistribution() {
            uint[] scoreDist = new uint[MAX_SCORE];
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
                        if (score < MAX_SCORE)
                        {
                            scoreDist[score]++;
                        }
                    }
                }
            }
            scoreDistribution = scoreDist;
        }

        private void createDummyScoreDistribution() {
            uint[] scoreDist = new uint[MAX_SCORE];
            scoreDist[0] = 10;
            scoreDist[1] = 20;
            scoreDist[2] = 15;
            scoreDist[3] = 10;
            scoreDist[5] = 5;
            scoreDist[10] = 1;
            scoreDist[15] = 5;
            scoreDist[MAX_SCORE - 1] = 1;
            scoreDistribution = scoreDist;
        }


        private void showGraph()
        {
            float graphHeight = graphContainer.sizeDelta.y - Y_TOP_OFFSET - Y_BOTTOM_OFFSET;
            float graphWidth = graphContainer.sizeDelta.x - X_LEFT_OFFSET - X_RIGHT_OFFSET;

            AuditLog.Log($"Stats: h: {graphHeight}, w:{graphWidth}");


            // Auto-scale Y: Find the max value in your list
            int yMax = 0;
            foreach (int val in scoreDistribution) {
                if (val > yMax) yMax = val;
            }
            yMax = Mathf.Max(yMax, 1); // Avoid division by zero
            AuditLog.Log($"Stats: ymax: {yMax}");

            float xSize = graphWidth / MAX_SCORE;
            AuditLog.Log($"Stats: xSize: {xSize}");


            // 1. Draw Y-Axis Labels
            // Separator count is the y axis lines
            uint separatorCount = MAX_NUM_Y_TICKS;
            if (yMax < MAX_NUM_Y_TICKS)
            {
                separatorCount = (uint) yMax;
            }
            for (int i = 0; i <= separatorCount; i++) {
                float normalizedValue = i / (float)separatorCount;
                float yPos = normalizedValue * graphHeight + Y_TOP_OFFSET;
                string labelText = Mathf.RoundToInt(normalizedValue * yMax).ToString();
                CreateLabel(new Vector2(MAX_SCORE_LABEL_HEIGHT, yPos), labelText, TextAlignmentOptions.Right);
            }

            // 2. Draw X-Axis Labels (0 to 21)
            for (int i = 0; i <= MAX_SCORE; i++) {
                float xPos = i * xSize + X_LEFT_OFFSET;
                // Only draw every 3rd or 5th label if it looks cluttered
                if (i % 1 == 0) { 
                    CreateLabel(new Vector2(xPos, MAX_SCORE_LABEL_WIDTH), i.ToString(), TextAlignmentOptions.Top);
                }
            }


            for (int i = 0; i < MAX_SCORE && i <= MAX_SCORE; i++)
            {
                float xPosition = i * xSize + X_LEFT_OFFSET;
                // Scale Y relative to the container height and max value
                float yPosition = (scoreDistribution[i] / (float)yMax) * graphHeight + Y_TOP_OFFSET;
                
                CreateDot(new Vector2(xPosition, yPosition));
            }
        }

        private void CreateLabel(Vector2 anchoredPosition, string text, TextAlignmentOptions anchor)
        {
            AuditLog.Log($"Stats: create label: {text}");

            // GameObject labelObj = new GameObject("Label", typeof(Text));
            // labelObj.transform.SetParent(graphContainer, false);
            
            // TextMeshProUGUI txt = labelObj.GetComponent<TextMeshProUGUI>();
            // txt.text = text;
            // //txt.font = labelFont;
            // txt.fontSize = 100;
            // txt.color = Color.black;
            // //txt.alignment = anchor;

            // RectTransform rect = labelObj.GetComponent<RectTransform>();
            // rect.anchoredPosition = anchoredPosition;
            // rect.sizeDelta = new Vector2(50, 20); // Large enough for the number
            // rect.anchorMin = rect.anchorMax = new Vector2(0, 0);


            // 1. Create a new GameObject for the text
            GameObject textObj = new GameObject("DynamicText");

            // 2. Make the Image the parent of this text
            textObj.transform.SetParent(graphContainer.transform, false);

            // 3. Add the TextMeshPro component
            TextMeshProUGUI myText = textObj.AddComponent<TextMeshProUGUI>();
            
            // 4. Set the text and styling
            myText.text = text;
            myText.fontSize = 30;
            myText.alignment = anchor; //TextAlignmentOptions.Center;
            myText.color = Color.black;

            // 5. Make the text fill the parent image area
            RectTransform rt = textObj.GetComponent<RectTransform>();
            // rt.anchorMin = new Vector2(0, 0);
            // rt.anchorMax = new Vector2(1, 1);
            // rt.offsetMin = Vector2.zero;
            // rt.offsetMax = Vector2.zero;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = new Vector2(50, 20); // Large enough for the number
            rt.anchorMin = rt.anchorMax = new Vector2(0, 0);

        }

        private void CreateDot(Vector2 anchoredPosition)
        {
            float xPosition = anchoredPosition.x;
            float yPosition = anchoredPosition.y;
            float lineHeight = yPosition - Y_TOP_OFFSET;
            if (lineHeight > 0f)
            {
                GameObject lineObj = new GameObject("barLine", typeof(Image));
                lineObj.transform.SetParent(graphContainer, false);
                Image lineImage = lineObj.GetComponent<Image>();
                lineImage.sprite = dotSprite;
                lineImage.color = new Color(0.6f, 0.6f, 0.6f, 0.85f);
                RectTransform lineRect = lineObj.GetComponent<RectTransform>();
                lineRect.anchorMin = lineRect.anchorMax = new Vector2(0, 0);
                lineRect.pivot = new Vector2(0.5f, 0f);
                lineRect.anchoredPosition = new Vector2(xPosition, Y_TOP_OFFSET);
                lineRect.sizeDelta = new Vector2(BAR_LINE_WIDTH, lineHeight);
            }

            GameObject dot = new GameObject("dot", typeof(Image));
            dot.transform.SetParent(graphContainer, false);
            dot.GetComponent<Image>().sprite = dotSprite;
            RectTransform rect = dot.GetComponent<RectTransform>();
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(8, 8);
            rect.anchorMin = rect.anchorMax = new Vector2(0, 0);
        }





        // old private void CreateDot(Vector2 anchoredPosition)
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