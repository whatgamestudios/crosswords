using System;
using UnityEngine;
using System.Collections;

namespace CrossWords
{

    public class Stats
    {
        public const string STATS_SOLUTIONS = "STATS_SOLUTIONS_";
        public const string STATS_CURRENT = "STATS_CURRENT";
        public const string STATS_FIRST_PLAYED = "STATS_FIRST_PLAYED";
        public const string STATS_LAST_PLAYED = "STATS_LAST_PLAYED";
        public const string STATS_DAYS_PLAYED = "STATS_DAYS_PLAYED";
        public const string STATS_TOTAL_POINTS = "STATS_TOTAL_POINTS";

        public const string STATS_DAYS_CLAIMED = "STATS_DAYS_CLAIMED";

        public const string STATS_NUM_TIMES_PUBLISHED = "STATS_NUM_TIMES_PUBLISHED";
        public const string STATS_MOST_RECENT_PUBLISHED_DAY = "STATS_MOST_RECENT_PUBLISHED_DAY";
        public const string STATS_PUBLISHED_SCORE = "STATS_PUBLISHED_SCORE";
        public const string STATS_LAST_PUBLISHED = "STATS_LAST_PUBLISHED";

        public const int NEVER_PLAYED = -1;



        public static void SetCurrent(uint gameDay, uint score, string solution)
        {
            // Update days played stats
            int first = PlayerPrefs.GetInt(STATS_FIRST_PLAYED, 0);
            if (first == 0)
            {
                PlayerPrefs.SetInt(STATS_FIRST_PLAYED, (int) gameDay);
            }
            int last = PlayerPrefs.GetInt(STATS_LAST_PLAYED, 0);
            if (last != gameDay)
            {
                int daysPlayed = PlayerPrefs.GetInt(STATS_DAYS_PLAYED, 0);
                PlayerPrefs.SetInt(STATS_DAYS_PLAYED, daysPlayed + 1);
                PlayerPrefs.SetInt(STATS_LAST_PLAYED, (int) gameDay);
            }


            Solution current = new Solution(gameDay, score, solution);
            string currentSerialisd = current.Serialise();
            PlayerPrefs.SetString(STATS_CURRENT, current.Serialise());

            // Don't update score or stats if an existing score from today is better than the current score.
            (bool exists, Solution sol) = GetSolution(gameDay);
            if (!exists || sol.Score > score)
            {
                // Store new best solution for today.
                string key = STATS_SOLUTIONS + gameDay.ToString();
                PlayerPrefs.SetString(key, current.Serialise());

                int totalPoints = PlayerPrefs.GetInt(STATS_TOTAL_POINTS, 0);
                totalPoints += (int) score;
                if (exists)
                {
                    totalPoints -= (int) sol.Score;
                }
                PlayerPrefs.SetInt(STATS_TOTAL_POINTS, totalPoints);
            }
            PlayerPrefs.Save();
        }

        public static (bool, Solution) GetSolution(uint gameDay)
        {
            string key = STATS_SOLUTIONS + gameDay.ToString();
            string serialised = PlayerPrefs.GetString(key, "");
            if (serialised == "")
            {
                return (false, null);
            }
            else
            {
                return (true, Solution.FromSerialised(serialised));
            }
        }

        public static (bool, Solution) GetCurrent()
        {
            string serialised = PlayerPrefs.GetString(STATS_CURRENT, "");
            if (serialised == "")
            {
                return (false, null);
            }
            else
            {
                return (true, Solution.FromSerialised(serialised));
            }
        }


        public static int GetFirstDayPlayed()
        {
            return PlayerPrefs.GetInt(STATS_FIRST_PLAYED, 0);
        }

        public static uint GetLastGameDay()
        {
            return (uint) PlayerPrefs.GetInt(STATS_LAST_PLAYED, 0);
        }

        public static int GetNumDaysPlayed()
        {
            return PlayerPrefs.GetInt(STATS_DAYS_PLAYED, 1);
        }

        public static int GetAverageScore()
        {
            int total = PlayerPrefs.GetInt(STATS_TOTAL_POINTS, 26);
            int daysPlayed = GetNumDaysPlayed();
            return total / daysPlayed;
        }


        public static void SetPublished(uint score)
        {
            uint gameDay = GetLastGameDay();
            if (gameDay != GetMostRecentDayPublished())
            {
                PlayerPrefs.SetInt(STATS_MOST_RECENT_PUBLISHED_DAY, (int) gameDay);
                int timesPlayed = GetNumTimesPublished();
                PlayerPrefs.SetInt(STATS_NUM_TIMES_PUBLISHED, timesPlayed + 1);
            }
            PlayerPrefs.SetInt(STATS_PUBLISHED_SCORE, (int) score);
        }

        public static uint GetMostRecentDayPublished() {
            return (uint) PlayerPrefs.GetInt(STATS_MOST_RECENT_PUBLISHED_DAY, 0);
        }

        public static uint GetPublishedScore() {
            return (uint) PlayerPrefs.GetInt(STATS_PUBLISHED_SCORE, 0);
        }

        public static bool HasPublishedToday()
        {
            return GetLastGameDay() == GetMostRecentDayPublished();
        }

        public static int GetNumTimesPublished()
        {
            return PlayerPrefs.GetInt(STATS_NUM_TIMES_PUBLISHED, 0);
        }
    }    
}