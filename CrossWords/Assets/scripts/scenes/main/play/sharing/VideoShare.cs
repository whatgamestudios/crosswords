// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text;


namespace CrossWords {

    public class VideoShare : MonoBehaviour {

        public void Start()
        {
            // Get message and then share.
        }


        public void OnButtonClick(string buttonText)
        {
            if (buttonText != "purple" && buttonText != "letters")
            {
                AuditLog.Log($"Share Video: Unknown button: {buttonText}");
                return;
            }

            if (buttonText == "letters")
            {
                MessagePass.SetMsg("video-letters");
            }
            else
            {
                MessagePass.SetMsg("video-worcadian-purple");
            }

            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("GamePlayScene", LoadSceneMode.Single);


            //SunShineNativeShare.instance.ShareText(msg, msg);
        }


    }
}