// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

namespace CrossWords {

    public class WordListScreen : MonoBehaviour {

        public TextMeshProUGUI CheckEntryText;
        public TextMeshProUGUI StartsWithEntryText;

        public Image CheckEntryBackground;
        public Image StartsWithBackground;


        private bool checkTextSelected;
        private string checkText;
        private string startsWithText;

        
        void Start()
        {
            AuditLog.Log($"Word List Scene");
            WireLetterButtons();
            checkTextSelected = true;
            checkText = "";
            startsWithText = "";
        }

        void WireLetterButtons()
        {
            var buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var button in buttons)
            {
                string n = button.gameObject.name;
                if (n.Length != 4 || !n.StartsWith("But", StringComparison.Ordinal))
                    continue;

                char c = n[3];
                if (c < 'A' || c > 'Z')
                    continue;

                button.onClick.RemoveAllListeners();
                char letter = c;
                button.onClick.AddListener(() => OnLetterButton(button, letter));
            }
        }

        void OnLetterButton(Button button, char letter)
        {
            button.interactable = false;

            if (checkTextSelected)
            {
                checkText += letter;
            }
            else
            {
                startsWithText += letter;
            }
        }

        public void OnBackSpaceButton()
        {
            if (checkTextSelected)
            {
                int lenLessOne = checkText.Length - 1;
                //c = checkText[lenLessOne];
                if (lenLessOne >= 0)
                {
                    checkText = checkText.Substring(0, lenLessOne);
                }
            }
            else
            {
                int lenLessOne = startsWithText.Length - 1;
                //c = startsWithText[lenLessOne];
                if (lenLessOne >= 0)
                {
                    startsWithText = startsWithText.Substring(0, lenLessOne);
                }
            }
        }


        public void OnClearButton()
        {
            if (checkTextSelected)
            {
                checkText = "";
            }
            else
            {
                startsWithText = "";
            }
        }        

        public void OnButtonClick(string buttonText) {
            if (buttonText == "ShareLogs") {
            }
            else
            {
                AuditLog.Log("WordList: Unknown button: " + buttonText);
            }
        }


        public void OnCheckTextSelected()
        {
            checkTextSelected = true;
            SetImageSprite(CheckEntryBackground, true);
            SetImageSprite(StartsWithBackground, false);
        }

        public void OnStartsWithTextSelected()
        {
            checkTextSelected = false;
            SetImageSprite(CheckEntryBackground, false);
            SetImageSprite(StartsWithBackground, true);
        }

        void Update()
        {
            CheckEntryText.text = checkText;
            StartsWithEntryText.text = startsWithText;
        }


        private void SetImageSprite(Image img, bool useWhite)
        {
            string resource = useWhite ? "backgrounds/white" : "backgrounds/lightgrey";
            Texture2D tex = Resources.Load<Texture2D>(resource);
            if (tex == null) {
                AuditLog.Log("ERROR: Resource not found: " + resource);
                return;
            }

            Rect size = new Rect(0.0f, 0.0f, tex.width, tex.height);
            Vector2 pivot = new Vector2(0.0f, 0.0f);
            Sprite s = Sprite.Create(tex, size, pivot);
            img.sprite = s;
        }
    }
}