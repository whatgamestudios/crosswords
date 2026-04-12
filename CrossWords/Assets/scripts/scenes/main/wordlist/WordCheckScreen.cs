using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CrossWords { 

    public class WordCheckScreen : MonoBehaviour
    {
        public TextMeshProUGUI WordCheckResult;
        public TextMeshProUGUI SuggestButtonText;
        public Button SuggestButton;
        public GameObject SuggestBackground;


        void Start()
        {
            AuditLog.Log($"Check Word screen");
            WordCheckResult.text = "Loading";
            SuggestBackground.SetActive(false);
            SuggestButton.gameObject.SetActive(false);
            Invoke("DelayedLoad", 0.1f);
        }
        
        void DelayedLoad()
        {
            WordListDictionary wordListDictionary = GetComponent<WordListDictionary>();
            if (wordListDictionary == null)
            {
                AuditLog.Log("Check Word screen: No dictionary");
            }
            else if (!wordListDictionary.DictionaryLoaded)
            {
                Invoke("DelayedLoad", 0.1f);
            }
            else
            {
                string word = MessagePass.GetMsg();
                bool inDic = wordListDictionary.IsInDictionary(word);
                if (inDic)
                {
                    WordCheckResult.text = word + " is in the word list";
                    SuggestButtonText.text = "Suggest word be REMOVED from the word list";
                }
                else
                {
                    WordCheckResult.text = word + " is not in the word list";
                    SuggestButtonText.text = "Suggest word be ADDED to the word list";
                }
                SuggestBackground.SetActive(true);
                SuggestButton.gameObject.SetActive(true);
            }
        }
    }
}
