// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;


namespace CrossWords {

    public class UnexpectedErrorScreen : MonoBehaviour {

        public void Start() {
            AuditLog.Log("Unexpected Error screen");
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Share") {
                string msg = AuditLog.GetLogs();
                SunShineNativeShare.instance.ShareText(msg, msg);
            }
            else if (buttonText == "GoToGame") {
                SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
            }
            else {
                AuditLog.Log($"Unexpected E Screen: Unknwon Button: {buttonText}");
            }
        }
    }
}