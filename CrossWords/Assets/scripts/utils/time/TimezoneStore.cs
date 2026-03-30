// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System;

namespace CrossWords {
    // Audit log to store an audit of activity in the game.
    public class TimezoneStore {
        public const string TIMEZONE = "TIMEZONE";

        public static void SetTimeZone(bool useLocal) {
            int useLocalInt = useLocal ? 0 : 1;
            PlayerPrefs.SetInt(TIMEZONE, useLocalInt);
        }

        public static bool UseLocalTimeZone() {
            int useLocalInt = PlayerPrefs.GetInt(TIMEZONE, 0);
            return useLocalInt == 0;
        }
    }
}