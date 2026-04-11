using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq; // Required for the .AsEnumerable() extension method

namespace CrossWords {

    public class WordListDictionary : MonoBehaviour
    {
        public TextAsset dictionaryFile; 
        private HashSet<string> dictionarySet = new HashSet<string>();
        public bool DictionaryLoaded;

        void Start()
        {
            DictionaryLoaded = false;
            if (dictionaryFile != null)
            {
                LoadDictionary();
                AuditLog.Log($"Loaded dictionary: {dictionarySet.Count} items.");
                DictionaryLoaded = true;
            }
            else
            {
                AuditLog.Log($"ERROR: Failed to load dictionary.");
            }
        }

        void LoadDictionary()
        {
            // Read the entire text asset content as a single string
            string fileContents = dictionaryFile.text;

            // Split the string by line breaks and add to the HashSet
            // Note: Use the specific line ending character(s) your file uses. '\n' is common.
            string[] lines = fileContents.Split('\n');

            // Add each line to the HashSet
            foreach (string line in lines)
            {
                // Optional: Trim any leading/trailing whitespace, including carriage returns
                string trimmedLine = line.Trim(); 
                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    dictionarySet.Add(trimmedLine);
                }
            }
        }

        public bool IsInDictionary(string word)
        {
            return dictionarySet.Contains(word.ToLower());
        }

        public string GetTwoLetterWords()
        {
            System.Text.StringBuilder res = new System.Text.StringBuilder();

            foreach (string word in dictionarySet)
            {
                if (word.Length == 2)
                {
                    res.Append(word);
                    res.Append('\n');
                }
            }
            return res.ToString();
        }
    }
}
