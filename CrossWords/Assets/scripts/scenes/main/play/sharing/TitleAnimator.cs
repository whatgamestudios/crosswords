// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

namespace CrossWords {

    public class TitleAnimator : MonoBehaviour {
        public GameObject titlePanel;

        private GameObject floatingImageW;
        private GameObject floatingImageO;
        private GameObject floatingImageR;
        private GameObject floatingImageC;
        private GameObject floatingImageA;
        private GameObject floatingImageD;
        private GameObject floatingImageI;
        private GameObject floatingImageA2;
        private GameObject floatingImageN;

        private float targetLocationW;
        private float targetLocationO;
        private float targetLocationR;
        private float targetLocationC;
        private float targetLocationA;
        private float targetLocationD;
        private float targetLocationI;
        private float targetLocationA2;
        private float targetLocationN;

        private float perLetterWidth;

        private DateTime start;
        private bool started;

        private const int TIME_FOR_START = 2000;
        private const int NUM_LETTERS = 9;

        public void Start() {
            started = false;
        }

        public void Init() {
            AuditLog.Log("Title Animator init");
            started = true;
            start = DateTime.Now;
            
            floatingImageW = new GameObject("FloatingImageW");
            loadImage(floatingImageW, 'w');
            floatingImageO = new GameObject("FloatingImageO");
            loadImage(floatingImageO, 'o');
            floatingImageR = new GameObject("FloatingImageR");
            loadImage(floatingImageR, 'r');
            floatingImageC = new GameObject("FloatingImageC");
            loadImage(floatingImageC, 'c');
            floatingImageA = new GameObject("FloatingImageA");
            loadImage(floatingImageA, 'a');
            floatingImageD = new GameObject("FloatingImageD");
            loadImage(floatingImageD, 'd');
            floatingImageI = new GameObject("FloatingImageI");
            loadImage(floatingImageI, 'i');
            floatingImageA2 = new GameObject("FloatingImageA2");
            loadImage(floatingImageA2, 'a');
            floatingImageN = new GameObject("FloatingImageN");
            loadImage(floatingImageN, 'n');

            float panelWidth = titlePanel.GetComponent<RectTransform>().rect.width;
            perLetterWidth = panelWidth / (NUM_LETTERS + 1);
            targetLocationW = perLetterWidth * -4;
            targetLocationO = perLetterWidth * -3;
            targetLocationR = perLetterWidth * -2;
            targetLocationC = perLetterWidth;
            targetLocationA = 0;
            targetLocationD = perLetterWidth * 1;
            targetLocationI = perLetterWidth * 2;
            targetLocationA2 = perLetterWidth * 3;
            targetLocationN = perLetterWidth * 4;
        }

        public void Update() {
            if (!started)
            {
                return;
            }

            DateTime now = DateTime.Now;
            double msSinceStart = (now - start).TotalMilliseconds;
            double relativeProgress = msSinceStart / TIME_FOR_START * NUM_LETTERS;
            int animationStage = (int)relativeProgress;
            float animationProgress = (float) (relativeProgress - animationStage);
            bool animationDone = msSinceStart > TIME_FOR_START;

            AuditLog.Log($"Title Animator: {animationStage}, {animationProgress}, {animationDone}");

            if (animationDone)
            {
                return;
            }

            switch (animationStage)
            {
                case 0:
                    animateLetter(floatingImageW, targetLocationW, animationProgress);
                    break;
                case 1:
                    animateLetter(floatingImageO, targetLocationO, animationProgress);
                    break;
                case 2:
                    animateLetter(floatingImageR, targetLocationR, animationProgress);
                    break;
                case 3:
                    animateLetter(floatingImageC, targetLocationC, animationProgress);
                    break;
                case 4:
                    animateLetter(floatingImageA, targetLocationA, animationProgress);
                    break;
                case 5:
                    animateLetter(floatingImageD, targetLocationD, animationProgress);
                    break;
                case 6:
                    animateLetter(floatingImageI, targetLocationI, animationProgress);
                    break;
                case 7:
                    animateLetter(floatingImageA2, targetLocationA2, animationProgress);
                    break;
                case 8:
                    animateLetter(floatingImageN, targetLocationN, animationProgress);
                    break;
            }
        }

        private void animateLetter(GameObject floatingImage, float targetLocation, float progress)
        {
            floatingImage.SetActive(true);

            // Calculate animation progress
            RectTransform imageRect = floatingImage.GetComponent<RectTransform>();

            // Keep the image centered during size changes
           float locationX = Mathf.Lerp(0, targetLocation, progress);
           imageRect.anchoredPosition = new Vector2(locationX , 0);

            float size = Mathf.Lerp(0, perLetterWidth, progress);
            imageRect.sizeDelta = new Vector2(size, size);
        }


        private void loadImage(GameObject imageGameObject, char letter) {
            imageGameObject.transform.SetParent(titlePanel.transform, false);
            
            RectTransform imageRect = imageGameObject.AddComponent<RectTransform>();
            imageRect.anchorMin = new Vector2(0.5f, 0.5f);
            imageRect.anchorMax = new Vector2(0.5f, 0.5f);
            imageRect.anchoredPosition = Vector2.zero;
            imageRect.sizeDelta = new Vector2(300, 300);
            
            imageGameObject.SetActive(false);

            Image imageComponent = imageGameObject.AddComponent<Image>();
            string resource = "letters/letter-" + letter + "-rounded";

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