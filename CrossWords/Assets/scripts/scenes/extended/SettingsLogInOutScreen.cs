// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Immutable.Passport;
using System.Collections;
using System.Collections.Generic;


namespace CrossWords {

    public class SettingsLogInOutScreen : MonoBehaviour {

        public TextMeshProUGUI LogInOutButtonText;

        private bool isLoginCoroutineRunning;
        private bool isLogoutCoroutineRunning;
        private Coroutine loginCheckRoutine;
        private Coroutine logoutCheckRoutine;

        public void Start() {
            AuditLog.Log("Settings screen");

            if (PassportStore.IsLoggedIn())
            {
                LogInOutButtonText.text = "Logout";
            }
            else
            {
                LogInOutButtonText.text = "Login";
            }
        }

        public void OnDisable() {
            stopLoginCoroutine();
            stopLogoutCoroutine();
        }

        public async void OnButtonClick(string buttonText) {
            if (buttonText != "LogInOut")
            {
                AuditLog.Log("SettingsLoginOut: Unknown button: " + buttonText);
            }

            if (PassportStore.IsLoggedIn())
            {
                PostHogStats.GetInstance().LogSettingsLogout();

                PassportStore.SetLoggedIn(false);
                await PassportLogin.Init();
                await Passport.Instance.Logout();
                startLogoutCoroutine();
            }
            else
            {
                PostHogStats.GetInstance().LogSettingsLogin();

                PassportStore.SetLoggedIn(false);
                PassportStore.SetChoseToNotLogin(false);
                SceneStack.Instance().Reset();
                await PassportLogin.Init();                
                startLoginCoroutine();

                //Debug.Log("LoginPKCE start");
// #if (UNITY_ANDROID && !UNITY_EDITOR_WIN) || (UNITY_IPHONE && !UNITY_EDITOR_WIN) || UNITY_STANDALONE_OSX
//                await Passport.Instance.LoginPKCE();
// #else
               await Passport.Instance.Login();
//#endif
//                Debug.Log("LoginPKCE done");
            }
        }


        private void startLoginCoroutine() {
            if (!isLoginCoroutineRunning) {
                loginCheckRoutine = StartCoroutine(LoginCheckRoutine());
                isLoginCoroutineRunning = true;
            }
        }

        public void stopLoginCoroutine() {
            if (isLoginCoroutineRunning && loginCheckRoutine != null) {
                StopCoroutine(loginCheckRoutine);
                isLoginCoroutineRunning = false;
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
                stopLoginCoroutine();
            }
        }        

        private void startLogoutCoroutine() {
            if (!isLogoutCoroutineRunning) {
                logoutCheckRoutine = StartCoroutine(LogoutCheckRoutine());
                isLogoutCoroutineRunning = true;
            }
        }

        public void stopLogoutCoroutine() {
            if (isLogoutCoroutineRunning && logoutCheckRoutine != null) {
                StopCoroutine(logoutCheckRoutine);
                isLogoutCoroutineRunning = false;
            }
        }

        IEnumerator LogoutCheckRoutine() {
            while (true) {
                CheckLogout();
                yield return new WaitForSeconds(1f);
            }
        }

        private async void CheckLogout() {
            bool loggedIn = await Passport.Instance.HasCredentialsSaved();
            AuditLog.Log("CheckLogout: Loggedin: " + loggedIn);
            if (!loggedIn) {
                PassportStore.SetLoggedIn(false);
                PassportStore.SetLoggedInChecked();
                DeepLinkManager.Instance.LoginPath = DeepLinkManager.SETTINGS;
                SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
                stopLogoutCoroutine();
            }
        }        

    }
}