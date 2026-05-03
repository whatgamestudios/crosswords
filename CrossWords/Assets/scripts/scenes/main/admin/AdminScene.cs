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
            else if (buttonText == "CheckSeed") 
            {
                Invoke("CheckSeedWordsProcess", 0.1f);
            }
            else if (buttonText == "AddWords") 
            {
                Invoke("AddWordsProcess", 0.1f);
            }
            else if (buttonText == "CheckWords")
            {
                Invoke("CheckWords", 0.1f);
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

        private async Task CheckSeedWordsProcess() {
            isProcessing = true;
            try {
                resetLog();
                log("CheckSeedWordsProcess: started");

                SeedWordProcessor seedProcessorContract = new SeedWordProcessor();

                int numSeedWords = WordListSeed.GetNumSeedWords();
                int numIncorrect = 0;
                for (int i = 0; i < numSeedWords; i++) {
                    log($"Checking {i} of {numSeedWords}. Wrong: {numIncorrect}");
                    string word = await seedProcessorContract.GetSeedWord(i);
                    string appWord = WordListSeed.GetSeedWord((uint)i);
                    if (word != appWord) {
                        numIncorrect++;
                        log($" app {appWord}, contract {word}");
                    }
                }

                log("CheckSeedWordsProcess: done");
            }
            catch (Exception ex) {
                log($"Exception during admin process: {ex.Message}");
            }
            finally {
                isProcessing = false;
            }
        }

        private async Task AddWordsProcess() {
            isProcessing = true;
            try {
                resetLog();
                log("AddWordsProcess: started");

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

                WordListProcessor processorContract = new WordListProcessor();

                NewWordsLoader newWordsLoader = GetComponent<NewWordsLoader>();
                if (newWordsLoader == null)
                {
                    log("ERROR: No new words");
                    return;
                }
                else if (!newWordsLoader.DictionaryLoaded)
                {
                    log("ERROR: newWordsLoader not loaded");
                    return;
                }

                HashSet<string> dict = newWordsLoader.GetDict();

                int groupSize = 300;
                List<string> words = new List<string>();

                int size = dict.Count;
                int num = 0;

                foreach (string word in dict)
                {
                    int ofs = (num % groupSize);
                    num++;
                    words.Add(word);
                    if (ofs + 1 == groupSize) {
                        log($"Loading {num} of {size}");
                        bool ok = await processorContract.AddWords(words);
                        if (!ok) {
                            log("AddWords returned no OK");
                            break;
                        }
                        words = new List<string>();
                    }
                } 

                // Send off the final set of words
                log($"Loading {num} of {size}");
                bool ok2 = await processorContract.AddWords(words);
                if (!ok2) {
                    log("AddWords returned no OK");
                }

                log("AddWordsProcess: done");
            }
            catch (Exception ex) {
                log($"Exception during admin process: {ex.Message}");
            }
            finally {
                isProcessing = false;
            }
        }


        private async Task CheckWords() {
            isProcessing = true;
            const int batchSize = 1000;
            try {
                resetLog();
                log("CheckWords: started");

                WordListDictionary wordListDictionary = GetComponent<WordListDictionary>();
                if (wordListDictionary == null) {
                    log("ERROR: No dictionary");
                    return;
                }
                if (!wordListDictionary.DictionaryLoaded) {
                    log("ERROR: Dictionary not loaded");
                    return;
                }

                HashSet<string> dict = wordListDictionary.GetDict();
                List<string> allWords = new List<string>(dict);
                int total = allWords.Count;
                WordListProcessor processorContract = new WordListProcessor();

                int numMissing = 0;
                for (int i = 0; i < total; i += batchSize) {
                    int count = Math.Min(batchSize, total - i);
                    List<string> batch = allWords.GetRange(i, count);
                    log($"Checking words {i} to {i + count - 1} of {total}. Missing so far: {numMissing}");

                    List<bool> present = await processorContract.InWordListBulk(batch);
                    for (int j = 0; j < present.Count; j++) {
                        if (!present[j]) {
                            numMissing++;
                            log($" not on contract: {batch[j]}");
                        }
                    }
                }

                log($"CheckWords: done. Total missing on contract: {numMissing}");
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