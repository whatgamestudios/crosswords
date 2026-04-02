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
        public const string STATS_LAST_PUBLISHED = "STATS_LAST_PUBLISHED";

        public const string STATS_SILVER_STREAK_LENGTH = "STATS_SSTREAK_LEN";
        public const string STATS_SILVER_STREAK_LONGEST_LENGTH = "STATS_SSTREAK_LONGEST_LEN";
        public const string STATS_SILVER_STREAK_LAST_DAY = "STATS_SSTREAK_LAST_DAY";
        public const uint STATS_SILVER_STREAK_THRESHOLD = 15;

        public const string STATS_GOLD_STREAK_LENGTH = "STATS_GSTREAK_LEN";
        public const string STATS_GOLD_STREAK_LONGEST_LENGTH = "STATS_GSTREAK_LONGEST_LEN";
        public const string STATS_GOLD_STREAK_LAST_DAY = "STATS_GSTREAK_LAST_DAY";
        public const uint STATS_GOLD_STREAK_THRESHOLD = 10;

        public const string STATS_DIAMOND_STREAK_LENGTH = "STATS_DSTREAK_LEN";
        public const string STATS_DIAMOND_STREAK_LONGEST_LENGTH = "STATS_DSTREAK_LONGEST_LEN";
        public const string STATS_DIAMOND_STREAK_LAST_DAY = "STATS_DSTREAK_LAST_DAY";
        public const uint STATS_DIAMOND_STREAK_THRESHOLD = 5;

        public const string STATS_BDIAMOND_STREAK_LENGTH = "STATS_BDSTREAK_LEN";
        public const string STATS_BDIAMOND_STREAK_LONGEST_LENGTH = "STATS_BDSTREAK_LONGEST_LEN";
        public const string STATS_BDIAMOND_STREAK_LAST_DAY = "STATS_BDSTREAK_LAST_DAY";
        public const uint STATS_BDIAMOND_STREAK_THRESHOLD = 3;

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

                updateStreaks(gameDay, score, true);
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




        private static void updateStreaks(uint gameDay, uint pointsToday, bool finalSolutionForToday = false)
        {
            updateStreak(gameDay, pointsToday, finalSolutionForToday,
                STATS_SILVER_STREAK_THRESHOLD, STATS_SILVER_STREAK_LAST_DAY,
                STATS_SILVER_STREAK_LENGTH, STATS_SILVER_STREAK_LONGEST_LENGTH);
            updateStreak(gameDay, pointsToday, finalSolutionForToday,
                STATS_GOLD_STREAK_THRESHOLD, STATS_GOLD_STREAK_LAST_DAY,
                STATS_GOLD_STREAK_LENGTH, STATS_GOLD_STREAK_LONGEST_LENGTH);
            updateStreak(gameDay, pointsToday, finalSolutionForToday,
                STATS_DIAMOND_STREAK_THRESHOLD, STATS_DIAMOND_STREAK_LAST_DAY,
                STATS_DIAMOND_STREAK_LENGTH, STATS_DIAMOND_STREAK_LONGEST_LENGTH);
            updateStreak(gameDay, pointsToday, finalSolutionForToday,
                STATS_BDIAMOND_STREAK_THRESHOLD, STATS_BDIAMOND_STREAK_LAST_DAY,
                STATS_BDIAMOND_STREAK_LENGTH, STATS_BDIAMOND_STREAK_LONGEST_LENGTH);
        }
        private static void updateStreak(uint gameDay, uint pointsToday, bool finalSolutionForToday,
            uint threshold, string lastDayKey, string lenKey, string longestLenKey)
        {
            if (pointsToday >= (int)threshold)
            {
                int lastDay = PlayerPrefs.GetInt(lastDayKey, 0);
                if (lastDay != gameDay)
                {
                    // Need to update stats for today
                    PlayerPrefs.SetInt(lastDayKey, (int)gameDay);
                    int streakLen;
                    if (lastDay == gameDay - 1)
                    {
                        // Extension of streak
                        streakLen = PlayerPrefs.GetInt(lenKey, 0);
                        streakLen++;
                    }
                    else
                    {
                        // Start of new streak
                        streakLen = 1;
                    }
                    PlayerPrefs.SetInt(lenKey, streakLen);
                    int longestStreakLen = PlayerPrefs.GetInt(longestLenKey, 0);
                    if (streakLen > longestStreakLen)
                    {
                        PlayerPrefs.SetInt(longestLenKey, streakLen);
                    }
                }
            }
            else
            {
                // Less than threshold
                if (finalSolutionForToday)
                {
                    // If this is the third solution for today, and the points scored is
                    // less than the threshold for this type of streak, then any existing 
                    // streak is ended, and the streak length is now 0.
                    PlayerPrefs.SetInt(lenKey, 0);
                }
            }
        }

        public static (int, int, int, int) GetStreaksLengths()
        {
            int silverLen = PlayerPrefs.GetInt(STATS_SILVER_STREAK_LENGTH, 0);
            int goldLen = PlayerPrefs.GetInt(STATS_GOLD_STREAK_LENGTH, 0);
            int diamondLen = PlayerPrefs.GetInt(STATS_DIAMOND_STREAK_LENGTH, 0);
            int bdiamondLen = PlayerPrefs.GetInt(STATS_BDIAMOND_STREAK_LENGTH, 0);
            return (silverLen, goldLen, diamondLen, bdiamondLen);
        }

        public static (int, int, int, int) GetLongestStreaksLengths()
        {
            int silverLen = PlayerPrefs.GetInt(STATS_SILVER_STREAK_LONGEST_LENGTH, 0);
            int goldLen = PlayerPrefs.GetInt(STATS_GOLD_STREAK_LONGEST_LENGTH, 0);
            int diamondLen = PlayerPrefs.GetInt(STATS_DIAMOND_STREAK_LONGEST_LENGTH, 0);
            int bdiamondLen = PlayerPrefs.GetInt(STATS_BDIAMOND_STREAK_LONGEST_LENGTH, 0);
            return (silverLen, goldLen, diamondLen, bdiamondLen);
        }

        // public static void SetPublished()
        // {
        //     int gameDay = GetLastGameDay();
        //     if (gameDay != GetMostRecentDayPublished())
        //     {
        //         //PlayerPrefs.SetInt(STATS_MOST_RECENT_PUBLISHED_DAY, gameDay);
        //         int timesPlayed = GetNumTimesPublished();
        //         PlayerPrefs.SetInt(STATS_NUM_TIMES_PUBLISHED, timesPlayed + 1);
        //     }
        // }

        // public static int GetMostRecentDayPublished() {
        //     return PlayerPrefs.GetInt(STATS_MOST_RECENT_PUBLISHED_DAY, 0);
        // }

        // public static bool HasPublishedToday()
        // {
        //     return GetLastGameDay() == GetMostRecentDayPublished();
        // }

        public static int GetNumTimesPublished()
        {
            return PlayerPrefs.GetInt(STATS_NUM_TIMES_PUBLISHED, 0);
        }
    }    
}