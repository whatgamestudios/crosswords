// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;


namespace CrossWords {
    public class AdminScene : MonoBehaviour {

        public TextMeshProUGUI OutputText;
        public RectTransform ScrollContent;
        bool isProcessing = false;

        System.Text.StringBuilder logs = new System.Text.StringBuilder();

        public void Start() {
            AuditLog.Log("Admin start");
        }

        public void OnButtonClick(string buttonText) {
            if (isProcessing) {
                log("Busy");
                return;
            }

            if (buttonText == "SeedStatus")
            {
                Invoke("RunSeedProcess", 0.1f);
            }
            else if (buttonText == "LoadSeed") 
            {
                Invoke("LoadSeedWordsProcess", 0.1f);
            }
            else
            {
                AuditLog.Log("Admin: Unknown button: " + buttonText);
            }
        }


        private async Task RunSeedProcess() {
            isProcessing = true;
            try {
                resetLog();
                log("RunSeedProcess: started");

                SeedWordProcessor seedProcessorContract = new SeedWordProcessor();
                uint seedWordCount = await seedProcessorContract.SeedWordCount();
                log($"Seed word count is: {seedWordCount}");

                log("RunSeedProcess: done");
            }
            catch (Exception ex) {
                log($"Exception during admin process: {ex.Message}");
            }
            finally {
                isProcessing = false;
            }
        }

        private async Task LoadSeedWordsProcess() {
            isProcessing = true;
            try {
                resetLog();
                log("LoadSeedWordsProcess: started");

                if (!PassportStore.IsLoggedIn()) {
                    log("Passport not logged in");
                    return;
                }

                // Check network connectivity
                if (Application.internetReachability == NetworkReachability.NotReachable) {
                    log("No network connectivity available");
                    return;
                }

                await PassportLogin.InitAndLogin();
                log("Passport init done");

                SeedWordProcessor seedProcessorContract = new SeedWordProcessor();

                int groupSize = 300;

                uint seedWordCount = await seedProcessorContract.SeedWordCount();;
                int numSeedWords = WordListSeed.GetNumSeedWords();
                while (seedWordCount < numSeedWords) {
                    log($"Loading {seedWordCount} of {numSeedWords}");
                    List<string> words = WordListSeed.GetOrderedWords(seedWordCount, groupSize);
                    bool ok = await seedProcessorContract.AddSeedWords(words);
                    if (!ok) {
                        break;
                    }
                    log($"Pausing 2 seconds");
                    await Task.Delay(2000);
                    seedWordCount = await seedProcessorContract.SeedWordCount();;
                }
                log($"End {seedWordCount} of {numSeedWords}");

                log("LoadSeedWordsProcess: done");
            }
            catch (Exception ex) {
                log($"Exception during admin process: {ex.Message}");
            }
            finally {
                isProcessing = false;
            }
        }



        // private async Task StartCheckinProcess() {
        //     if (!PassportStore.IsLoggedIn()) {
        //         return;
        //     }

        //     // Check network connectivity
        //     if (Application.internetReachability == NetworkReachability.NotReachable) {
        //         AuditLog.Log("Checkin: No network connectivity available");
        //         return;
        //     }

        //     await PassportLogin.InitAndLogin();
        //     uint gameDay = Timeline.GameDay();
        //     AuditLog.Log("Checkin transaction");
        //     var checkInSuccess = await contract.SubmitCheckIn(gameDay);
        //     AuditLog.Log("Checkin: " + checkInSuccess.ToString());
        // }

        private void resetLog() {
            logs = new System.Text.StringBuilder();
        }

        private void log(string str) {
            AuditLog.Log(str);
            logs.Append(str + '\n');

            OutputText.text = logs.ToString();

            // 1. Force TMP to calculate its heights immediately
            OutputText.ForceMeshUpdate();
            // 2. Get the preferred height (the actual space the letters take)
            float textHeight = OutputText.preferredHeight;
            // 3. Apply that height to the Scroll View's Content container
            ScrollContent.sizeDelta = new Vector2(ScrollContent.sizeDelta.x, textHeight);                

        }
    }
}