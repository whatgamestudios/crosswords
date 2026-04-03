// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace CrossWords {

    public class CreditsScreen : MonoBehaviour {

        public TextMeshProUGUI VersionText;

        public void Start()
        {
            AuditLog.Log("Credits screen");
            VersionText.text = Application.version;
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Website") {
                Application.OpenURL("https://whatgamestudios.com/worcadian");
            }
            else if (buttonText == "Privacy") {
                Application.OpenURL("https://whatgamestudios.com/worcadian/privacy-policy/");
            }

            else
            {
                AuditLog.Log("Credits screen: Unknown button: " + buttonText);
            }
        }

    }
}