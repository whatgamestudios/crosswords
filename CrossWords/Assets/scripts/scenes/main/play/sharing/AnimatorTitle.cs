// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

namespace CrossWords {

    public class AnimatorTitle : MonoBehaviour {
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

        private double scaleW;
        private double scaleO;
        private double scaleR;
        private double scaleC;
        private double scaleA;
        private double scaleD;
        private double scaleI;
        private double scaleA2;
        private double scaleN;


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
            targetLocationW = perLetterWidth * (float) -4.3;
            targetLocationO = perLetterWidth * (float) -2.95;
            targetLocationR = perLetterWidth * (float) -1.9;
            targetLocationC = perLetterWidth * (float) -0.85;
            targetLocationA = perLetterWidth * (float) 0.2;
            targetLocationD = perLetterWidth * (float) 1.25;
            targetLocationI = perLetterWidth * (float) 2.3;
            targetLocationA2 = perLetterWidth * (float) 3.35;
            targetLocationN = perLetterWidth * (float) 4.4;

            scaleW = 1.5;
            scaleO = 1.0;
            scaleR = 0.9;
            scaleC = 0.9;
            scaleA = 0.9;
            scaleD = 1.0;
            scaleI = 0.9;
            scaleA2 = 0.9;
            scaleN = 1.0;
        }

        public void End()
        {
            if (floatingImageW != null) floatingImageW.SetActive(false);
            if (floatingImageO != null) floatingImageO.SetActive(false);
            if (floatingImageR != null) floatingImageR.SetActive(false);
            if (floatingImageC != null) floatingImageC.SetActive(false);
            if (floatingImageA != null) floatingImageA.SetActive(false);
            if (floatingImageD != null) floatingImageD.SetActive(false);
            if (floatingImageI != null) floatingImageI.SetActive(false);
            if (floatingImageA2 != null) floatingImageA2.SetActive(false);
            if (floatingImageN != null) floatingImageN.SetActive(false);
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
                    animateLetter(floatingImageW, targetLocationW, scaleW, animationProgress);
                    break;
                case 1:
                    animateLetter(floatingImageO, targetLocationO, scaleO, animationProgress);
                    break;
                case 2:
                    animateLetter(floatingImageR, targetLocationR, scaleR, animationProgress);
                    break;
                case 3:
                    animateLetter(floatingImageC, targetLocationC, scaleC, animationProgress);
                    break;
                case 4:
                    animateLetter(floatingImageA, targetLocationA, scaleA, animationProgress);
                    break;
                case 5:
                    animateLetter(floatingImageD, targetLocationD, scaleD, animationProgress);
                    break;
                case 6:
                    animateLetter(floatingImageI, targetLocationI, scaleI, animationProgress);
                    break;
                case 7:
                    animateLetter(floatingImageA2, targetLocationA2, scaleA2, animationProgress);
                    break;
                case 8:
                    animateLetter(floatingImageN, targetLocationN, scaleN, animationProgress);
                    break;
            }
        }

        private void animateLetter(GameObject floatingImage, float targetLocation, double scale, float progress)
        {
            floatingImage.SetActive(true);

            // Calculate animation progress
            RectTransform imageRect = floatingImage.GetComponent<RectTransform>();

            // Keep the image centered during size changes
           float locationX = Mathf.Lerp(0, targetLocation, progress);
           imageRect.anchoredPosition = new Vector2(locationX , 0);

            float size = Mathf.Lerp(0, perLetterWidth, progress) * (float) scale;
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