// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
//using Immutable.Passport;

namespace CrossWords {

    public class OtherMenuScreen : MonoBehaviour {

        public void Start()
        {
            AuditLog.Log("Menu (secondary) screen");
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Passport") {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("PassportScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Socials") {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("SocialsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Settings") {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("SettingsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Credits") {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("CreditsScene", LoadSceneMode.Single);
            }
            else {
                AuditLog.Log("Other Menu: Unknown button" + buttonText);
            }
        }
    }
}