// Copyright (c)  Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace CrossWords {
    public class SetPanelBackground : MonoBehaviour {
        public GameObject panel;

        public void Start() {
            if (AssetSyncManager.Instance != null) {
                AssetSyncManager.Instance.SyncIfNeeded();
            }

            // Set panel width to screen width
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(Screen.width * 3 / 2, panelRect.sizeDelta.y);
            ScreenBackgroundSetter.SetPanelBackground(panel);
        }
    }
}