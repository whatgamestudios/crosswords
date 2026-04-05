// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;

namespace CrossWords {

    /**
    * Manage data upgrades.
    */
    public class UpgradeStorage {
        // Storage version keys.
        public const string STORAGE_VERSION = "STORAGE_VERSION";

        public const int STORAGE_VERSION_0 = 0;
        public const int STORAGE_VERSION_1 = 1;


        /**
        * Check the storage version. If the app has been updated and the storage
        * layout has changed, upgrade the storage data.
        */
        public static void UpgradeStorageIfNecessary() {
            int currentStorageVersion = PlayerPrefs.GetInt(STORAGE_VERSION, STORAGE_VERSION_0);

            // if (currentStorageVersion == STORAGE_VERSION_1) {
            //     // Latest version - do nothing.
            //     return;
            // }

            // switch (currentStorageVersion) {
            //     case STORAGE_VERSION_0:
            //         // Upgrade from 0 to latest.
            //         AuditLog.Log("Upgrading storage from " + STORAGE_VERSION_0 + " to " + STORAGE_VERSION_1);
            //         PlayerPrefs.SetInt(STORAGE_VERSION, STORAGE_VERSION_1);

            //         SceneStore.SetBackground(SceneStore.BG_DEFAULT);

            //         PlayerPrefs.Save();
            //         break;
            //     default:
            //         AuditLog.Log("Unknown storage version: " + currentStorageVersion);
            //         break;            
            // }
        }
    }
}