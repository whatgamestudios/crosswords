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

        private float sizeX;
        private float sizeY;

        void Start()
        {
            AuditLog.Log($"Word List Scene");
            WireLetterButtons();

            (checkTextSelected, checkText, startsWithText) = WordListStore.Load();

            sizeX = CheckEntryBackground.rectTransform.sizeDelta.x + 500f;
            sizeY = CheckEntryBackground.rectTransform.sizeDelta.y + 25f;

            SetImageSprite(CheckEntryBackground, checkTextSelected);
            SetImageSprite(StartsWithBackground, !checkTextSelected);
            DisableLetterButtonsForUsedLetters(checkText);
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
            if (checkTextSelected)
            {
                if (checkText.Length <= 11) {
                    checkText += letter;
                    button.interactable = false;
                }
            }
            else
            {
                if (startsWithText.Length <= 11)
                {
                    startsWithText += letter;
                    button.interactable = false;
                }
            }
            WordListStore.Store(checkTextSelected, checkText, startsWithText);
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
                DisableLetterButtonsForUsedLetters(checkText);
            }
            else
            {
                int lenLessOne = startsWithText.Length - 1;
                //c = startsWithText[lenLessOne];
                if (lenLessOne >= 0)
                {
                    startsWithText = startsWithText.Substring(0, lenLessOne);
                    DisableLetterButtonsForUsedLetters(startsWithText);
                }
            }
            WordListStore.Store(checkTextSelected, checkText, startsWithText);
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
            DisableLetterButtonsForUsedLetters("");
            WordListStore.Store(checkTextSelected, checkText, startsWithText);
        }        

        public void OnButtonClick(string buttonText) {
            AuditLog.Log("WordList: button: " + buttonText);
            if (buttonText == "StartsWith")
            {
                MessagePass.SetMsg(startsWithText);
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("StartsWithScene", LoadSceneMode.Single);
            }
            else if (buttonText == "TwoLetters")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("TwoLetterWordsScene", LoadSceneMode.Single);
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
            DisableLetterButtonsForUsedLetters(checkText);
            WordListStore.Store(checkTextSelected, checkText, startsWithText);
        }

        public void OnStartsWithTextSelected()
        {
            checkTextSelected = false;
            SetImageSprite(CheckEntryBackground, false);
            SetImageSprite(StartsWithBackground, true);
            DisableLetterButtonsForUsedLetters(startsWithText);
            WordListStore.Store(checkTextSelected, checkText, startsWithText);
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

            // Bodge the size of the image to approximately fit where the image
            // should be.
            RectTransform rt = img.rectTransform;
            img.sprite = s;
            rt.sizeDelta = new Vector2(sizeX, sizeY);
        }

        private void DisableLetterButtonsForUsedLetters(string str)
        {
            var buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var button in buttons)
            {
                string n = button.gameObject.name;
                if (n.Length != 4 || !n.StartsWith("But", StringComparison.Ordinal))
                {
                    continue;
                }

                char c = n[3];
                if (c >= 'A' && c <= 'Z')
                {
                    button.interactable = (str.Contains(c) == false);
                }
            }
        }

    }
}