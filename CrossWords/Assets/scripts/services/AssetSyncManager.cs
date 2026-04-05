// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;

namespace CrossWords
{
    public class AssetSyncManager
    {
        public static AssetSyncManager Instance { get; private set; }

        private MonoBehaviour mono;

        private AssetSyncManager(MonoBehaviour monoBehaviour)
        {
            mono = monoBehaviour;
            SyncIfNeeded();
        }

        public static void StartInstance(MonoBehaviour monoBehaviour)
        {
            if (Instance == null)
            {
                Instance = new AssetSyncManager(monoBehaviour);
            }
        }

        public void SyncIfNeeded()
        {
            bool needToSync = SceneStore.DoINeedToCheckOwnedNfts();
            AuditLog.Log($"AssetSync: Need to sync: {needToSync}");
            if (needToSync)
            {
                ScreenBackground.FetchAndProcessNfts(mono);
            }
        }

        public void SyncNow()
        {
            ScreenBackground.FetchAndProcessNfts(mono);
        }
    }
}