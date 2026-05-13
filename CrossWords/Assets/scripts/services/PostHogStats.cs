// Copyright (c) Whatgame Studios 2024 - 2026
using PostHogUnity;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace CrossWords {

    // Follows documnetation here: https://posthog.com/docs/libraries/unity
    public class PostHogStats {

        private static PostHogStats Instance;

        public static PostHogStats GetInstance()
        {
            if (Instance == null)
            {
                Instance = new PostHogStats();
            }
            return Instance;
        }


        private PostHogStats()
        {
            AuditLog.Log("PostHogStats start");
            PostHog.Setup(new PostHogConfig
            {
                ApiKey = "phc_CYyiYxpYf6rgE7DDWqSQajJ7mwxUT58bUHVJygfdAceV",
                Host = "https://us.i.posthog.com" // usually 'https://us.i.posthog.com' or 'https://eu.i.posthog.com'
            });

            // Register a super property
            bool firstTime;
            string userId;
            (firstTime, userId) = UserId.GetUserId();
            PostHog.Register("user_id", userId);
            if (firstTime)
            {
                LogFirstTime();
            }
        }


        private void LogFirstTime()
        {
            string device = SystemInfo.deviceModel;
            string operatingSystem = SystemInfo.operatingSystem;
            string screenWidth = Screen.width.ToString();
            string screenHeight = Screen.height.ToString();
            
            PostHog.Capture("user_init");
        }

        public void LogWelcome()
        {
            PostHog.Capture("user_welcome");
        }

        public void LogPlayingGame()
        {
            uint gameDay = Timeline.GameDay();

            PostHog.Capture("user_play", new Dictionary<string, object>
            {
                { "game day", gameDay }
            });            
        }    

        public void LogPublishingError(string error) 
        {
            uint gameDay = Timeline.GameDay();
            (bool available, Solution solution) = Stats.GetSolution(gameDay);
            string avail = available ? solution.ToString() : "Not available";
            PostHog.Capture("publish_error", new Dictionary<string, object>
            {
                { "game day", gameDay },
                { "solution", avail},
                { "error", error}
            });
        }    

   }
}