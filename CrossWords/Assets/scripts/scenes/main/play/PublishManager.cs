// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

namespace CrossWords {

    // Manage the publish button
    public class PublishManager : MonoBehaviour
    {
        public GameObject panelPublish;
        public TextMeshProUGUI buttonPublishText;

        private bool publishButtonPressed = false;

        private const int TIME_PER_FLASH = 500;
        DateTime timeOfLastFlash = DateTime.Now;
        bool cursorOn = false;


        public void Start()
        {
            panelPublish.SetActive(false);
        }

        public void OnButtonClick(string buttonText)
        {
            if (buttonText == "Publish")
            {
                AuditLog.Log("Publish");
                publishButtonPressed = true;
                panelPublish.SetActive(false);
                if (PassportStore.IsLoggedIn())
                {
                    SceneStack.Instance().PushScene();
                    SceneManager.LoadScene("PublishScene", LoadSceneMode.Additive);
                }
                else
                {
                    SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
                }
            }
            else
            {
                AuditLog.Log("PublishManager: Unknown button: " + buttonText);
            }
        }

        public void Update()
        {
            GamePlayScene gamePlayScene = FindFirstObjectByType<GamePlayScene>();
            if (gamePlayScene == null) {
                return;
            }
            uint score = gamePlayScene.Score;
            if (!BestScoreLoader.LoadedBestScore ||
                publishButtonPressed ||
                score > BestScoreLoader.BestScore)
            {
                panelPublish.SetActive(false);
                return;
            }
            if (Stats.HasPublishedToday() && Stats.GetPublishedScore() == score) {
                panelPublish.SetActive(false);
                return;
            }
            if (!PassportStore.IsLoggedIn())
            {
                buttonPublishText.text = "Sign in to Publish";
                buttonPublishText.fontSize = 50;
            }

            // Flash the colour of the publish panel.
            // Note: the publish panel may not be active.
            // DateTime now = DateTime.Now;
            // if ((now - timeOfLastFlash).TotalMilliseconds > TIME_PER_FLASH)
            // {
            //     timeOfLastFlash = now;

            //     Image img = panelPublish.GetComponent<Image>();
            //     if (cursorOn)
            //     {
            //         img.color = UnityEngine.Color.green;
            //         cursorOn = false;
            //     }
            //     else
            //     {
            //         img.color = UnityEngine.Color.red;
            //         cursorOn = true;
            //     }
            // }

            panelPublish.SetActive(true);
        }
    }
}