// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text;


namespace CrossWords {

    public class ShareManager : MonoBehaviour {

        public void OnButtonClick(string buttonText)
        {
            if (buttonText == "Share")
            {
                AuditLog.Log("Go to Share Scene");
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("ShareScene", LoadSceneMode.Single);
            }
            else
            {
                AuditLog.Log($"Share Manager: Unknown button: {buttonText}");
            }
        }
    }
}