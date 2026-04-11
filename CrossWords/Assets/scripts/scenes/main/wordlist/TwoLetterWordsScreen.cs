using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CrossWords { 

    public class TwoLetterWordsScreen : MonoBehaviour
    {
        public TextMeshProUGUI InfoText;
        public RectTransform scrollContent;

        void Start()
        {
            AuditLog.Log($"Two Letter Words screen");
            InfoText.text = "Loading";
            Invoke("DelayedLoad", 0.1f);
        }
        
        void DelayedLoad()
        {
            WordListDictionary wordListDictionary = GetComponent<WordListDictionary>();
            if (wordListDictionary == null)
            {
                AuditLog.Log("Two Letter Word screen: No dictionary");
            }
            else if (!wordListDictionary.DictionaryLoaded)
            {
                Invoke("DelayedScoreUpdate", 0.1f);
            }
            else
            {
                string words = wordListDictionary.GetTwoLetterWords();
                InfoText.text = words;

                // 1. Force TMP to calculate its heights immediately
                InfoText.ForceMeshUpdate();
                // 2. Get the preferred height (the actual space the letters take)
                float textHeight = InfoText.preferredHeight;
                // 3. Apply that height to the Scroll View's Content container
                scrollContent.sizeDelta = new Vector2(scrollContent.sizeDelta.x, textHeight);                
            }
        }
    }
}
