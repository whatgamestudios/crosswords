// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
//using Immutable.Passport;
using System.Threading.Tasks;

namespace CrossWords {

    public class SkipScreen : MonoBehaviour {

        public void Start() {
            AuditLog.Log("Skip screen");
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Skip")
            {
                SceneStack.Instance().Reset();
                //DeepLinkManager.Instance.LoginPath = DeepLinkManager.LOGIN_SKIP;
                SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
            }
            else
            {
                AuditLog.Log("Skip Screen: Unknown button");
            }
        }
    }
}