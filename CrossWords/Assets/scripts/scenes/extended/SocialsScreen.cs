// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace CrossWords {

    public class SocialsScreen : MonoBehaviour {

        public void Start() {
            AuditLog.Log("Socials screen");
        }
    
        public void OnButtonClick(string buttonText) {
            if (buttonText == "Instagram") {
                Application.OpenURL("https://www.instagram.com/worcadian/");
            }
            else if (buttonText == "Discord") {
                Application.OpenURL("https://discord.gg/V6nbs2grws");
            }
            else if (buttonText == "YouTube") {
                Application.OpenURL("https://www.youtube.com/@WorcadianOfficial");
            }
            else if (buttonText == "Twitter") {
                Application.OpenURL("https://x.com/worcadian");
            }
            else {
                AuditLog.Log($"Socials Screen: Unknown button: {buttonText}");
            }
        }
    }
}