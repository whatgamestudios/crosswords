// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

namespace CrossWords {

    // Check if it has gone from 11:59pm to midnight, and a new day has begun.
    // At the start of the day, re-load the game play scene.
    // In this way, if a game player has the game opened to the game play scene
    // at midnight, it will move to the new day.
    public class NewDayManager : MonoBehaviour
    {
        private float lastCheckTime = 0f;
        private const float CHECK_INTERVAL = 0.5f; // 500ms in seconds

        public void Update()
        {
            // Check if enough time has passed since last check
            if (Time.time - lastCheckTime < CHECK_INTERVAL)
            {
                return;
            }
            
            lastCheckTime = Time.time;

            GameState gameState = GameState.Instance();
            if (gameState.IsPlayerStateUnknown())
            {
                return;
            }

            uint today = Timeline.GameDay();
            uint gameDayBeingPlayed = gameState.GameDayBeingPlayed();
            if (today != gameDayBeingPlayed)
            {
                // It has just passed midnight!
                AuditLog.Log($"New day detected. Switching from {gameDayBeingPlayed} to {today}");
                SceneManager.LoadScene("GamePlayScene", LoadSceneMode.Single);
            }
        }
    }
}