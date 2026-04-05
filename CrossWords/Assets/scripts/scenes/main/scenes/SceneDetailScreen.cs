// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace CrossWords {
    /**
     * Screen shown when play presses on a scene in the scene selector screen, when the 
     * scene is already selected.
     */
    public class CrossWords : MonoBehaviour {

        public TextMeshProUGUI sceneName;
        public TextMeshProUGUI sceneMetadata;


        public void Start() {
            int tokenId = SceneStore.GetBackground();
            AuditLog.Log("Scene detail screen: " + tokenId.ToString());
            SceneInfo sceneInfo = BackgroundsMetadata.GetInfo(tokenId);
            string maxSupply = sceneInfo.maxSupply == 0 ? "Infinite" : sceneInfo.maxSupply.ToString();
            sceneName.text = sceneInfo.name;
            int balance = SceneStore.GetBalanceFor(tokenId);
            if (sceneInfo.series == "Free") {
                balance = 1;
            }
            sceneMetadata.text = 
                sceneInfo.series + "\n" +
                sceneInfo.rarity + "\n" +
                maxSupply + "\n" + 
                sceneInfo.artist + "\n" +
                balance;
        }
    }
}