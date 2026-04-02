// Copyright (c) Whatgame Studios 2026
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace CrossWords {

    public class DeleteScreen : MonoBehaviour {

        public void Start()
        {
            AuditLog.Log("Delete screen");
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "DeletePassport") {
                Application.OpenURL("https://support.immutable.com/docs/immutable-passport/security-and-privacy/deleting-my-passport-account");
            }
            else
            {
                AuditLog.Log("Delete screen: Unknown button: " + buttonText);
            }
        }

    }
}