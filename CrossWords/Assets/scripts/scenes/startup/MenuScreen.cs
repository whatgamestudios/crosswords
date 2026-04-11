// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Immutable.Passport;

namespace CrossWords {

    public class MenuScreen : MonoBehaviour {
        public TextMeshProUGUI loggedIn;


        public async void Start() {
            AuditLog.Log("Menu screen");
            loggedIn.text = "Loading";

            bool isLoggedIn = PassportStore.IsLoggedIn();
            if (isLoggedIn) {
                await PassportLogin.Init();
                await PassportLogin.Login();

                // Set up wallet (includes creating a wallet for new players)
                List<string> accounts = await Passport.Instance.ZkEvmRequestAccounts();
                if (accounts.Count ==0) {
                    loggedIn.text = "Logged In";
                }
                else {
                    string account = accounts[0];
                    loggedIn.text = "Logged In (" + 
                                    DeepLinkManager.Instance.LoginPath + 
                                    ") as\n" + 
                                    account;
                }
            }
            else {
                loggedIn.text = "Not Logged In";
            }
        }


        public void OnButtonClick(string buttonText)
        {
            if (buttonText == "Play")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("GamePlayScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Stats")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("StatsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Solutions")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("SolutionsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Backgrounds")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("BackgroundsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Help")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("HelpScene", LoadSceneMode.Single);
            }
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
            else if (buttonText == "WordList") {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("WordListScene", LoadSceneMode.Single);
            }
            else
            {
                AuditLog.Log($"Menu: Unknown button {buttonText}");
                return;
            }
        }
    }
}
