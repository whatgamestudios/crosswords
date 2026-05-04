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

        private double scale;


        private DateTime start;
        private bool started;

        private const int TIME_FOR_START = 2000;

        public void Start() {
            started = false;
        }

        public void Init() {
            AuditLog.Log("Title Animator init");
            
            floatingImage = new GameObject("FloatingImage");
            loadImage(floatingImage);

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
            imageRect.anchoredPosition = new Vector2(0 , 0);
            // float locationX = Mathf.Lerp(0, targetLocation, progress);
            // imageRect.anchoredPosition = new Vector2(locationX , 0);

            // float size = Mathf.Lerp(0, perLetterWidth, animationProgress) * (float) scale;
            // imageRect.sizeDelta = new Vector2(size, size);
        }


        private void loadImage(GameObject imageGameObject) {
            imageGameObject.transform.SetParent(titlePanel.transform, false);
            
            RectTransform imageRect = imageGameObject.AddComponent<RectTransform>();
            imageRect.anchorMin = new Vector2(0.5f, 0.5f);
            imageRect.anchorMax = new Vector2(0.5f, 0.5f);
            imageRect.anchoredPosition = Vector2.zero;
            imageRect.sizeDelta = new Vector2(300, 300);
            
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