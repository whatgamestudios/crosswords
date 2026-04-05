// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System;

namespace CrossWords {

    /**
    * Game time class to determine which game day it is and how long until the next game day.
    */
    public class Timeline{
        /**
        * Return the number of days since the start of game development. This is
        * to generate a new random seed for each day of game play.
        */
        public static uint GameDay() {
            DateTime now = getTimeNow();
            DateTime dawnOfTime = new DateTime(2026, 3, 30, 0, 0, 0);
            TimeSpan diff = now.Subtract(dawnOfTime);
            return (uint)diff.Days;
        }

        public static DateTime GetRelativeDate(int gameDay) {
            DateTime dawnOfTime = new DateTime(2026, 3, 30, 0, 0, 0);
            return dawnOfTime.AddDays((double) gameDay);
        }

        public static string GetRelativeDateString(int gameDay) {
            DateTime dawnOfTime = new DateTime(2026, 3, 30, 0, 0, 0);
            DateTime relativeDate = dawnOfTime.AddDays((double) gameDay);
            return relativeDate.ToString("dddd MMMM dd, yyyy");
        }


        /**
        * Return the time until the next day of game play.
        */
        public static TimeSpan TimeToNextDay() {
            DateTime now = getTimeNow();
            DateTime nextDay = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            return nextDay.Subtract(now);
        }

        public static string GameDayStr() {
            uint gameDay = GameDay();
            return gameDay.ToString();
        }

        /**
        * Return the hours, minutes, and seconds until the next day.
        */
        public static string TimeToNextDayStr() {
            TimeSpan diff = TimeToNextDay();
            string format = "{0,2:D2}:{1,2:D2}:{2,2:D2}";
            return string.Format(format, diff.Hours, diff.Minutes, diff.Seconds);
        }

        public static string TimeToNextDayStrShort() {
            TimeSpan diff = TimeToNextDay();
            string format = "{0,2:D2} hours {1,2:D2} minutes";
            return string.Format(format, diff.Hours, diff.Minutes);
        }

        public static string TimeOfDayStr() {
            DateTime now = getTimeNow();
            DateTime startOfDay = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            TimeSpan diff = now.Subtract(startOfDay);
            string format = "{0,2:D2}:{1,2:D2}:{2,2:D2}";
            return string.Format(format, diff.Hours, diff.Minutes, diff.Seconds);
        }

        /**
        * Return the time now in either local time zone or GMT+14 time zone.
        */
        private static DateTime getTimeNow() {
            if (TimezoneStore.UseLocalTimeZone()) {
                return DateTime.Now;
            } else {
                // Get UTC time and add 14 hours for GMT+14
                return DateTime.UtcNow.AddHours(14);
            }
        }
    }
}