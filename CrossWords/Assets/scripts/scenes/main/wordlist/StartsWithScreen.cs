using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CrossWords { 

    public class StartsWithScreen : MonoBehaviour
    {
        public TextMeshProUGUI InfoText;
        public RectTransform scrollContent;

        void Start()
        {
            AuditLog.Log($"Starts With screen");
            InfoText.text = "Loading";
            Invoke("DelayedLoad", 0.1f);
        }
        
        void DelayedLoad()
        {
            WordListDictionary wordListDictionary = GetComponent<WordListDictionary>();
            if (wordListDictionary == null)
            {
                AuditLog.Log("Starts With screen: No dictionary");
            }
            else if (!wordListDictionary.DictionaryLoaded)
            {
                Invoke("DelayedLoad", 0.1f);
            }
            else
            {
                string prefix = MessagePass.GetMsg();
                (bool max, string words) = wordListDictionary.StartsWith(prefix);
                if (words.Length == 0)
                {
                    InfoText.text = "No words found";
                }
                else if (max)
                {
                    InfoText.text = "The first 1000 words are:\n" + words;
                }
                else
                {
                    InfoText.text = words;
                }

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
