// Copyright (c) Whatgame Studios 2024 - 2026
using PostHogUnity;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace CrossWords {

    // Follows documnetation here: https://posthog.com/docs/libraries/unity
    public class PostHogStats : MonoBehaviour {


        void Start()
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


        void LogFirstTime()
        {
            string device = SystemInfo.deviceModel;
            string operatingSystem = SystemInfo.operatingSystem;
            string screenWidth = Screen.width.ToString();
            string screenHeight = Screen.height.ToString();
            
            bool isLoggedIn = PassportStore.IsLoggedIn();
            bool choseNotToLogin = PassportStore.ChoseToNotLogin();
            string passportAccount = PassportStore.GetPassportAccount();

            PostHog.Capture("user_init");
        }

        public void LogWelcome()
        {
            PostHog.Capture("user_welcome");
        }

        public void LogPressedLogin()
        {
            string passportAccount = PassportStore.GetPassportAccount();
            PostHog.Capture("user_login", new Dictionary<string, object>
            {
                { "passport", passportAccount}
            });
        }

        public void LogPressedSkip()
        {
            PostHog.Capture("user_skip");
        }

        public void LogPlayingGame()
        {
            uint gameDay = Timeline.GameDay();
            bool choseNotToLogin = PassportStore.ChoseToNotLogin();
            string passportAccount = PassportStore.GetPassportAccount();

            PostHog.Capture("user_play", new Dictionary<string, object>
            {
                { "game day", gameDay },
                { "chose not to", choseNotToLogin},
                { "passport", passportAccount}
            });            
        }        

   }
}