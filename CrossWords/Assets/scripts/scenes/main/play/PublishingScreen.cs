// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using System;
using System.Numerics;
using System.Collections.Generic;

namespace CrossWords {

    public class PublishingScreen : MonoBehaviour {
        public Canvas backgroundCanvas;

        public Button backButton;
                    
        private const int TIME_PER_DOT = 1000;
        DateTime timeOfLastDot = DateTime.Now;

        GameProcessor contract;

        private bool isProcessing = false;
        private bool hasError = false;
        private string errorMessage = "";

        private TextMeshProUGUI infoText;
        private GameObject infoObj;


        private List<FallingNumber> fallingNumbers = new List<FallingNumber>();
        private Color[] colors = new Color[] {
            new Color(0.678f, 0.847f, 0.902f), // Light blue
            new Color(0.576f, 0.439f, 0.859f), // Purple
            new Color(1f, 0.753f, 0.796f)      // Pink
        };
        private string[] numbers = new string[] {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
        };

        private RectTransform canvasRect;
        private float baseFontSize = 110f;
        private float pulseAmplitude = 10f;
        private float pulseSpeed = 1f * Mathf.PI; // One complete cycle per second

        public void Start() {
            AuditLog.Log("Publishing screen");
            contract = new GameProcessor();
            timeOfLastDot = DateTime.Now;
            canvasRect = backgroundCanvas.GetComponent<RectTransform>();
            
            // Set the background canvas to render behind other UI
            backgroundCanvas.sortingOrder = 0;
            
            StartPublishProcess();
            InitializeFallingNumbers();
            InitializeInfo();

            // Move backButton to be definitely on top of the background.
            if (backButton != null) {
                backButton.transform.SetParent(backgroundCanvas.transform, false);
            }
        }

        private void InitializeFallingNumbers()
        {
            // Create 50 falling numbers
            for (int i = 0; i < 50; i++)
            {
                CreateFallingNumber();
            }
        }

        private void InitializeInfo() {
            infoObj = new GameObject("Info2");
            infoObj.transform.SetParent(backgroundCanvas.transform, false);

            RectTransform rect = infoObj.AddComponent<RectTransform>();
            rect.sizeDelta = new UnityEngine.Vector2(canvasRect.rect.width * 0.9f, canvasRect.rect.height * 0.8f);
            //            rect.sizeDelta = new UnityEngine.Vector2(600, 400);
            var yPos = canvasRect.rect.width * 0.2f;
            var xPos = 0.0f;
            rect.anchoredPosition = new UnityEngine.Vector2(xPos, yPos);

            infoText = infoObj.AddComponent<TextMeshProUGUI>();
            infoText.alignment = TextAlignmentOptions.Center;
            infoText.fontSize = 100f;
            infoText.fontSize = baseFontSize;
            infoText.color = Color.white;
            infoText.fontStyle = FontStyles.Bold;
            infoText.text = "Publishing";
        }


        private void CreateFallingNumber() {
            GameObject numberObj = new GameObject("FallingNumber");
            numberObj.transform.SetParent(backgroundCanvas.transform, false);

            TextMeshProUGUI numberText = numberObj.AddComponent<TextMeshProUGUI>();
            numberText.alignment = TextAlignmentOptions.Center;
            numberText.fontSize = UnityEngine.Random.Range(60, 120);
            numberText.color = colors[UnityEngine.Random.Range(0, colors.Length)];

            RectTransform rect = numberObj.GetComponent<RectTransform>();
            float canvasWidth = canvasRect.rect.width;
            float canvasHeight = canvasRect.rect.height;

            // Set initial position within canvas bounds
            float xPos = UnityEngine.Random.Range(-canvasWidth / 2f + 50f, canvasWidth / 2f - 50f);
            float yPos = UnityEngine.Random.Range(-canvasHeight / 2f, canvasHeight / 2f);
            rect.anchoredPosition = new UnityEngine.Vector2(xPos, yPos);

            FallingNumber fallingNumber = new FallingNumber {
                GameObject = numberObj,
                Text = numberText,
                Speed = UnityEngine.Random.Range(50f, 150f),
                Number = numbers[UnityEngine.Random.Range(0, numbers.Length)]
            };

            fallingNumbers.Add(fallingNumber);
        }

        private void UpdateFallingNumbers() {
            float canvasWidth = canvasRect.rect.width;
            float canvasHeight = canvasRect.rect.height;

            foreach (var number in fallingNumbers) {
                RectTransform rect = number.GameObject.GetComponent<RectTransform>();
                UnityEngine.Vector2 pos = rect.anchoredPosition;
                pos.y -= number.Speed * Time.deltaTime;

                // Check if number has gone below the canvas
                if (pos.y < -canvasHeight/2f + 30f) {
                    // Reset to top of canvas with random x position
                    pos.y = canvasHeight/2f - 20f;
                    pos.x = UnityEngine.Random.Range(-canvasWidth/2f + 50f, canvasWidth/2f - 50f);
                    number.Number = numbers[UnityEngine.Random.Range(0, numbers.Length)];
                }

                // Ensure x position stays within bounds
                pos.x = Mathf.Clamp(pos.x, -canvasWidth/2f + 50f, canvasWidth/2f - 50f);

                rect.anchoredPosition = pos;
                number.Text.text = number.Number;
            }
        }

        private async void StartPublishProcess() {
            if (isProcessing) {
                return;
            }
            isProcessing = true;
            hasError = false;
            errorMessage = "";

            try {
                AuditLog.Log("Publish transaction");
                GamePlayScene gamePlayScene = FindFirstObjectByType<GamePlayScene>();
                if (gamePlayScene == null) {
                    return;
                }
                uint gameDay = gamePlayScene.GameDayBeingPlayed;
                (bool available, Solution solution) = Stats.GetSolution(gameDay);
                if (!available) {
                    return;
                }
                bool publishSuccess = false;
                uint retry = 0;
                while (!publishSuccess)
                {
                    publishSuccess = await contract.SubmitBoard(solution.GameDay, (int) solution.Score, solution.BoardString);
                    if (publishSuccess)
                    {
                        Stats.SetPublished(solution.Score);
                    }
                    else
                    {
                        retry++;
                        if (retry > 3)
                        {
                            AuditLog.Log("Failed to publish");
                            hasError = true;
                            errorMessage = "Failed to publish. Please try again later";
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) {
                hasError = true;
                errorMessage = "Error during publish process. Please try again later";
                AuditLog.Log($"Exception in publish process: {ex.Message}");
            }
            finally {
                isProcessing = false;
            }
        }

        public void Update() {
            UpdateFallingNumbers();

            if (isProcessing) {
                // Update info text size with pulsating effect
                float pulseValue = Mathf.Sin(Time.time * pulseSpeed);
                infoText.fontSize = baseFontSize + (pulseValue * pulseAmplitude);
                infoText.text = "Publishing";
            }
            else {
                infoText.fontSize = 100f;
                if (hasError) {
                    infoText.text = errorMessage;
                }
                else {
                    infoText.text = "Published your solution!";
                }
                // Get the best score for the day to be reloaded.
                BestScoreLoader.RestartLoadRoutine();
            }
        }

        private void OnDestroy() {
            foreach (var number in fallingNumbers) {
                if (number.GameObject != null) {
                    Destroy(number.GameObject);
                }
            }
            fallingNumbers.Clear();

            if (infoObj != null)
            {
                Destroy(infoObj);
            }
        }
    }

    public class FallingNumber {
        public GameObject GameObject;
        public TextMeshProUGUI Text;
        public float Speed;
        public string Number;
    }
}
