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

            // if (buttonText == "SeedStatus")
            // {
            //     Invoke("RunSeedProcess", 0.1f);
            // }
            // else if (buttonText == "LoadSeed") 
            // {
            //     Invoke("LoadSeedWordsProcess", 0.1f);
            // }
            //else 
            if (buttonText == "CheckSeed") 
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



        // private async Task LoadSeedWordsProcess() {
        //     isProcessing = true;
        //     try {
        //         resetLog();
        //         log("LoadSeedWordsProcess: started");

        //         if (!PassportStore.IsLoggedIn()) {
        //             log("Passport not logged in");
        //             return;
        //         }

        //         // Check network connectivity
        //         if (Application.internetReachability == NetworkReachability.NotReachable) {
        //             log("No network connectivity available");
        //             return;
        //         }

        //         await PassportLogin.InitAndLogin();
        //         log("Passport init done");

        //         SeedWordProcessor seedProcessorContract = new SeedWordProcessor();

        //         int groupSize = 300;

        //         uint seedWordCount = await seedProcessorContract.SeedWordCount();;
        //         int numSeedWords = WordListSeed.GetNumSeedWords();
        //         while (seedWordCount < numSeedWords) {
        //             log($"Loading {seedWordCount} of {numSeedWords}");
        //             List<string> words = WordListSeed.GetOrderedWords(seedWordCount, groupSize);
        //             bool ok = await seedProcessorContract.AddSeedWords(words);
        //             if (!ok) {
        //                 break;
        //             }
        //             log($"Pausing 2 seconds");
        //             await Task.Delay(2000);
        //             seedWordCount = await seedProcessorContract.SeedWordCount();;
        //         }
        //         log($"End {seedWordCount} of {numSeedWords}");

        //         log("LoadSeedWordsProcess: done");
        //     }
        //     catch (Exception ex) {
        //         log($"Exception during admin process: {ex.Message}");
        //     }
        //     finally {
        //         isProcessing = false;
        //     }
        // }

        private async Task CheckSeedWordsProcess() {
            isProcessing = true;
            const int batchSize = 100;
            try {
                resetLog();
                log("CheckSeedWordsProcess: started");

                GameDayServerProcessor gameDayProcessor = new GameDayServerProcessor();

                int numSeedWords = WordListSeed.GetNumSeedWords();
                int numIncorrect = 0;
                for (int i = 0; i < numSeedWords; i += batchSize) {
                    int count = Math.Min(batchSize, numSeedWords - i);
                    int[] days = new int[count];
                    for (int j = 0; j < count; j++) days[j] = i + j;

                    log($"Checking {i} to {i + count - 1} of {numSeedWords}. Wrong: {numIncorrect}");
                    Dictionary<int, string> words = await gameDayProcessor.GetSeedWords(days);

                    for (int j = 0; j < count; j++) {
                        int day = i + j;
                        string serverWord = words.TryGetValue(day, out string w) ? w : null;
                        string appWord = WordListSeed.GetSeedWord((uint)day);
                        if (serverWord != appWord) {
                            numIncorrect++;
                            log($" day {day}: app={appWord}, server={serverWord}");
                        }
                    }
                }

                log($"CheckSeedWordsProcess: done. Incorrect: {numIncorrect}");
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

                if (Application.internetReachability == NetworkReachability.NotReachable) {
                    log("No network connectivity available");
                    return;
                }

                WordListServerProcessor processorContract = new WordListServerProcessor();

                NewWordsLoader newWordsLoader = GetComponent<NewWordsLoader>();
                if (newWordsLoader == null) {
                    log("ERROR: No new words");
                    return;
                }
                else if (!newWordsLoader.DictionaryLoaded) {
                    log("ERROR: newWordsLoader not loaded");
                    return;
                }

                HashSet<string> dict = newWordsLoader.GetDict();

                int groupSize = 300;
                List<string> words = new List<string>();

                int size = dict.Count;
                int num = 0;

                foreach (string word in dict) {
                    int ofs = (num % groupSize);
                    num++;
                    words.Add(word);
                    if (ofs + 1 == groupSize) {
                        log($"Loading {num} of {size}");
                        AddWordsResult result = await processorContract.AddWords(words.ToArray());
                        log($"  added: {result.Added.Length}, already existed: {result.AlreadyExists.Length}");
                        words = new List<string>();
                    }
                }

                // Send off the final batch
                if (words.Count > 0) {
                    log($"Loading {num} of {size}");
                    AddWordsResult result = await processorContract.AddWords(words.ToArray());
                    log($"  added: {result.Added.Length}, already existed: {result.AlreadyExists.Length}");
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

                WordListDictionary newWordsLoader = GetComponent<WordListDictionary>();
                if (newWordsLoader == null) {
                    log("ERROR: No dictionary");
                    return;
                }
                if (!newWordsLoader.DictionaryLoaded) {
                    log("ERROR: Dictionary not loaded");
                    return;
                }

                HashSet<string> dict = newWordsLoader.GetDict();
                List<string> allWords = new List<string>(dict);
                int total = allWords.Count;
                WordListServerProcessor processorContract = new WordListServerProcessor();

                int numMissing = 0;
                for (int i = 0; i < total; i += batchSize) {
                    int count = Math.Min(batchSize, total - i);
                    List<string> batch = allWords.GetRange(i, count);
                    log($"Checking words {i} to {i + count - 1} of {total}. Missing so far: {numMissing}");

                    Dictionary<string, bool> present = await processorContract.CheckWords(batch.ToArray());
                    foreach (string word in batch) {
                        if (!present.TryGetValue(word.ToUpper(), out bool found) || !found) {
                            numMissing++;
                            log($" not on server: {word}");
                        }
                    }
                }

                log($"CheckWords: done. Total missing on server: {numMissing}");
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