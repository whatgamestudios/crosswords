// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

namespace CrossWords {
    /**
     * Storage for holding scene related information.
    **/
    public class SceneStore {
        public const string BG_OPTION = "OPTION_BACKGROUND";

        public const int BG_DEFAULT = 1;

        public const string BG_OWNED = "BACKGROUND_OWNED";
        public const string BG_BALANCES = "BACKGROUND_BALANCES";
        public const string BG_LAST_CHECKED = "BACKGROUND_LAST_CHECKED";

        // Interval between rechecking whether NFTs are owned, in hours.
        private const int RECHECK_INTERVAL = 24;

        /**
        * Set the background used by all scenes.
        */
        public static void SetBackground(int option) {
            PlayerPrefs.SetInt(BG_OPTION, option);
            PlayerPrefs.Save();
        }

        /**
        * Get the background option to be used.
        *
        * @return the background option number to use.
        */
        public static int GetBackground() {
            return PlayerPrefs.GetInt(BG_OPTION, BG_DEFAULT);
        }

        /**
         * Save the list of owned backgrounds.
         *
         * @param owned Array of owned background ids.
         */
        public static void SetOwned(int[] owned, int[] balances) {
            string ownedString = string.Join(",", owned);
            PlayerPrefs.SetString(BG_OWNED, ownedString);
            string balancesString = string.Join(",", balances);
            PlayerPrefs.SetString(BG_BALANCES, balancesString);
            PlayerPrefs.Save();
        }

        /**
         * Get the list of owned backgrounds.
         * 
         * @return array of owned background ids.
         */
        public static int[] GetOwned() {
            string ownedString = PlayerPrefs.GetString(BG_OWNED, "");
            if (string.IsNullOrEmpty(ownedString)) {
                return new int[0];
            }
            return Array.ConvertAll(ownedString.Split(','), int.Parse);
        }

        public static int[] GetBalances() {
            string balancesString = PlayerPrefs.GetString(BG_BALANCES, "");
            if (string.IsNullOrEmpty(balancesString)) {
                return new int[0];
            }
            return Array.ConvertAll(balancesString.Split(','), int.Parse);
        }

        public static int GetBalanceFor(int tokenId) {
            int[] owned = GetOwned();
            int[] balances = GetBalances();
            for (int i = 0; i < owned.Length; i++) {
                if (owned[i] == tokenId) {
                    return balances[i];
                }
            }
            return 0;
        }

        /**
         * Determine whether the NFTs owned by the player has been checked recently.
         * 
         * @return true if the NFTs owned by the player should be checked.
         */
        public static bool DoINeedToCheckOwnedNfts() {
            DateTime now = DateTime.Now;
            int currentTimeInHours = ((now.Year * 12 + now.Month) * 31 + now.Day) * 24 + now.Hour;

            int lastChecked = PlayerPrefs.GetInt(BG_LAST_CHECKED, 0);
            if (currentTimeInHours > lastChecked + RECHECK_INTERVAL) {
                return true;
            }
            return false;
        }

        /**
         * Determine whether the NFTs owned by the player has been checked recently.
         * 
         * @return true if the NFTs owned by the player should be checked.
         */
        public static void SetNftsSynced() {
            DateTime now = DateTime.Now;
            int currentTimeInHours = ((now.Year * 12 + now.Month) * 31 + now.Day) * 24 + now.Hour;
            PlayerPrefs.SetInt(BG_LAST_CHECKED, currentTimeInHours);
        }
    }
}