// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

namespace CrossWords {

    public class AnimatorWorcadianPurple : MonoBehaviour {
        public GameObject titlePanel;

        private GameObject floatingImage;
        private TextMeshProUGUI gameDayText;

        private double scale;


        private DateTime start;
        private bool started;

        private const int TIME_FOR_START = 2000;

        private const int SIZE_X_BASE = 1000;
        private const int SIZE_Y_BASE = 1200;

        private const int SIZE_Y_OFS = 300;
        private const int TEXT_Y_OFS = -120;


        public void Start() {
            started = false;
        }

        public void Init() {
            AuditLog.Log("Title Animator init");
            
            floatingImage = new GameObject("FloatingImage");
            loadImage(floatingImage);

            uint gameDay = Timeline.GameDay();
            gameDayText = createGameDayText(floatingImage, gameDay);

            scale = 1.0;

            started = true;
            start = DateTime.Now;
        }

        public void End()
        {
            if (floatingImage != null) floatingImage.SetActive(false);
        }

        public void Update() {
            if (!started)
            {
                return;
            }

            DateTime now = DateTime.Now;
            double msSinceStart = (now - start).TotalMilliseconds;
            double animationProgress = msSinceStart / TIME_FOR_START;
            bool animationDone = msSinceStart > TIME_FOR_START;

            AuditLog.Log($"Title Animator: {animationProgress}, {animationDone}");

            if (animationDone)
            {
                return;
            }

            floatingImage.SetActive(true);

            // Calculate animation progress
            RectTransform imageRect = floatingImage.GetComponent<RectTransform>();
            // Keep the image centered during size changes
            imageRect.anchoredPosition = new Vector2(0, SIZE_Y_OFS);

            // Transparency changes:
            // * First 20% of time goes from 0->1
            // * Middle 60% stays at 1
            // * Last 20% of time goes from 1->0.
            float transparency = Mathf.Lerp(0, 5, (float) animationProgress);
            if (transparency > 4.0f) 
            {
                transparency = 5.0f - transparency;
            }
            else if (transparency > 1.0f) 
            {
                transparency = 1.0f;
            }

            Image img = floatingImage.GetComponent<Image>();
            Color c = img.color;
            c.a = transparency;
            img.color = c;

            Color tc = gameDayText.color;
            tc.a = transparency;
            gameDayText.color = tc;

            // float locationX = Mathf.Lerp(0, targetLocation, progress);
            // imageRect.anchoredPosition = new Vector2(locationX , 0);

            // float size = Mathf.Lerp(0, perLetterWidth, animationProgress) * (float) scale;
            // imageRect.sizeDelta = new Vector2(size, size);
        }


        private TextMeshProUGUI createGameDayText(GameObject parent, uint gameDay) {
            GameObject textObj = new GameObject("GameDayText");
            textObj.transform.SetParent(parent.transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = new Vector2(0, TEXT_Y_OFS);
            textRect.sizeDelta = new Vector2(SIZE_X_BASE, SIZE_Y_BASE);

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = $"Game Day {gameDay}";
            tmp.color = Color.black;
            tmp.fontSize = 100;
            tmp.alignment = TextAlignmentOptions.Center;

            return tmp;
        }

        private void loadImage(GameObject imageGameObject) {
            imageGameObject.transform.SetParent(titlePanel.transform, false);
            
            RectTransform imageRect = imageGameObject.AddComponent<RectTransform>();
            imageRect.anchorMin = new Vector2(0.5f, 0.5f);
            imageRect.anchorMax = new Vector2(0.5f, 0.5f);
            imageRect.anchoredPosition = new Vector2(0, SIZE_Y_OFS);
            imageRect.sizeDelta = new Vector2(SIZE_X_BASE, SIZE_Y_BASE);
            
            imageGameObject.SetActive(false);

            Image imageComponent = imageGameObject.AddComponent<Image>();
            string resource = "letters/worcadian-purple";

            // Update the existing Image component
            Texture2D tex = Resources.Load<Texture2D>(resource);
            if (tex != null) {
                Rect size = new Rect(0.0f, 0.0f, tex.width, tex.height);
                Vector2 pivot = new Vector2(0.0f, 0.0f);
                Sprite s = Sprite.Create(tex, size, pivot);
                imageComponent.sprite = s;
            }
        }

    }
}