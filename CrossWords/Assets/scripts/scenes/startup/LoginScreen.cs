// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using Immutable.Passport;
using System.Threading.Tasks;

namespace CrossWords {

    public class LoginScreen : MonoBehaviour {

        public Button loginButton;

        public Button skipButton;
        private Coroutine loginCheckRoutine;
        private Coroutine buttonAnimationRoutine;
        private Coroutine skipButtonRoutine;
        private bool isRunning = false;
        private Image buttonImage;
        private int currentButtonFrame = 1;

        private string help = "Sign in to claim Scenes and publish best scores.\n\n" +
            "14Numbers uses Passport which uses Google, Apple, Facebook or email to sign in";

        public async Task Start() {
            AuditLog.Log("Login screen");
           PassportStore.SetLoggedIn(false);
            SceneStack.Instance().Reset();
           await PassportLogin.Init();
            
            // Hide skip button initially
            skipButton.gameObject.SetActive(false);
            
            startCoroutine();
            startButtonAnimation();
            startSkipButtonTimer();
        }

        public void OnDisable() {
            stopCoroutine();
            stopButtonAnimation();
            stopSkipButtonTimer();
        }

        private void startSkipButtonTimer() {
            if (skipButtonRoutine == null) {
                skipButtonRoutine = StartCoroutine(SkipButtonTimerRoutine());
            }
        }

        private void stopSkipButtonTimer() {
            if (skipButtonRoutine != null) {
                StopCoroutine(skipButtonRoutine);
                skipButtonRoutine = null;
            }
        }

        IEnumerator SkipButtonTimerRoutine() {
            yield return new WaitForSeconds(2f);
            skipButton.gameObject.SetActive(true);
        }

        private void startButtonAnimation() {
            if (buttonAnimationRoutine == null) {
                // Ensure we have an Image component
                buttonImage = loginButton.GetComponent<Image>();
                if (buttonImage == null) {
                    // If we can't find the button image, then don't try to animate it.
                    return;
                }

                buttonAnimationRoutine = StartCoroutine(ButtonAnimationRoutine());
            }
        }

        private void stopButtonAnimation() {
            if (buttonAnimationRoutine != null) {
                StopCoroutine(buttonAnimationRoutine);
                buttonAnimationRoutine = null;
            }
        }

        IEnumerator ButtonAnimationRoutine() {
            while (true)
            {
                try
                {
//. Maybe how to do it
// RectTransform rt = GetComponent<RectTransform>();
// // Increases current width and height by 10%
// rt.sizeDelta = new Vector2(rt.sizeDelta.x * 1.1f, rt.sizeDelta.y * 1.1f);                    

                    // buttonImage = loginButton.GetComponent<Image>();


                    // // Load and set the next button frame
                    // string resource = $"buttons/animated_button{currentButtonFrame}";
                    // Texture2D tex = Resources.Load<Texture2D>(resource);
                    // if (tex != null)
                    // {
                    //     Rect size = new Rect(0.0f, 0.0f, tex.width, tex.height);
                    //     Vector2 pivot = new Vector2(0.0f, 0.0f);
                    //     Sprite s = Sprite.Create(tex, size, pivot);
                    //     buttonImage.sprite = s;
                    // }
                    // else
                    // {
                    //     AuditLog.Log($"ERROR: Failed to load button texture: {resource}");
                    // }

                    // // Increment frame counter and reset if needed
                    // currentButtonFrame++;
                    // if (currentButtonFrame > 6)
                    // {
                    //     currentButtonFrame = 1;
                    // }
                }
                catch (Exception ex)
                {
                    AuditLog.Log($"Exception during login animation: {ex.Message}");
                    throw ex;
                }


                // Wait for 125ms before next frame
                yield return new WaitForSeconds(0.125f);
            }
        }

        public async void OnButtonClick(string buttonText) {
            if (buttonText == "Help") {
                MessagePass.SetMsg(help);
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("BigHelpContextScene", LoadSceneMode.Additive);
            }
            else if (buttonText == "Login") {
                //Debug.Log("LoginPKCE start");
// #if (UNITY_ANDROID && !UNITY_EDITOR_WIN) || (UNITY_IPHONE && !UNITY_EDITOR_WIN) || UNITY_STANDALONE_OSX
//                await Passport.Instance.LoginPKCE();
// #else
               await Passport.Instance.Login();
//#endif
//                Debug.Log("LoginPKCE done");
            }
            else if (buttonText == "Skip") {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("SkipSigninScene", LoadSceneMode.Additive);
            }
            else {
                AuditLog.Log("Login Screen: Unknown button");
            }
        }

        private void startCoroutine() {
            if (!isRunning) {
                loginCheckRoutine = StartCoroutine(LoginCheckRoutine());
                isRunning = true;
            }
        }

        public void stopCoroutine() {
            if (isRunning && loginCheckRoutine != null) {
                StopCoroutine(loginCheckRoutine);
                isRunning = false;
            }
        }

        IEnumerator LoginCheckRoutine() {
            while (true) {
                CheckLogin();
                yield return new WaitForSeconds(1f);
            }
        }

        private async void CheckLogin() {
            bool loggedIn = await Passport.Instance.HasCredentialsSaved();
            AuditLog.Log("CheckLogin: Loggedin: " + loggedIn);
            if (loggedIn) {
                PassportStore.SetLoggedIn(true);
                PassportStore.SetLoggedInChecked();
                DeepLinkManager.Instance.LoginPath = DeepLinkManager.LOGIN_THREAD;
                SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
                stopCoroutine();
            }
        }
    }
}