// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

namespace CrossWords {

    public class BackgroundSelector : MonoBehaviour {
        public GameObject panelScreenBackground;

        public GameObject panelFreeType1;
        public GameObject panelFreeType2;
        public GameObject panelFreeType3;

        public GameObject panelOwnedContent;

        public GameObject panelOwned;

        // Claim button
        public Button claimButton;
        public TextMeshProUGUI claimButtonText;


        private GameObject[] panelOwnedNfts;
        private ScrollRect scrollRect;

        private FourteenNumbersSolutionsContract fourteenNumbersContracts = new FourteenNumbersSolutionsContract();
        private FourteenNumbersClaimContract fourteenNumbersClaimContracts = new FourteenNumbersClaimContract();
        private Coroutine loadRoutineDaysPlayed;
        private Coroutine loadRoutineDaysClaimed;

        private const int NOT_SET = -1;
        private volatile int DaysPlayed = NOT_SET;
        private volatile int DaysClaimed = NOT_SET;

        public void Start()
        {
            AuditLog.Log("Scene screen");

            // Force NFT sync now. 
            AssetSyncManager.Instance.SyncNow();

            // Get the ScrollRect component
            scrollRect = panelOwned.GetComponentInParent<ScrollRect>();
            if (scrollRect == null)
            {
                AuditLog.Log("ERROR: No ScrollRect found in parent of panelOwned");
                return;
            }

            drawOwnedPanel();

            int selected = SceneStore.GetBackground();
            setSelected(selected);

            if (PassportStore.IsLoggedIn())
            {
                claimButton.interactable = false;
            }
            else
            {
                claimButtonText.text = "Sign in to Claim";
                claimButtonText.fontSize = 80;
            }
        }

        public void OnEnable() {
            if (PassportStore.IsLoggedIn()) {
                StartLoaders();
            }
        }

        public void OnDisable() {
        }



        public void OnButtonClick(string buttonText) {
            if (buttonText == "Claim") {
                if (PassportStore.IsLoggedIn()) {
                    SceneStack.Instance().PushScene();
                    SceneManager.LoadScene("ClaimScene", LoadSceneMode.Additive);
                }
                else {
                    SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
                }
            }
            else if (buttonText == "Prob") {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("ClaimProbScene", LoadSceneMode.Additive);
            }
            else if (buttonText == "Sync") {
                // To re-sync the NFT collection, reload this scene. 
                SceneManager.LoadScene("BackgroundsScene", LoadSceneMode.Single);
            }
            else {
                // One of the image buttons has been pressed.
                int option = BackgroundsMetadata.ButtonTextToOption(buttonText);
                int alreadySelectedOption = SceneStore.GetBackground();
                if (option == alreadySelectedOption) {
                    SceneStack.Instance().PushScene();
                    SceneManager.LoadScene("SceneDetailScene", LoadSceneMode.Additive);
                }
                else {
                    SceneStore.SetBackground(option);
                    setSelected(option);
                    ScreenBackgroundSetter.SetPanelBackground(panelScreenBackground);
                }
            }
        }

        private void setSelected(int selected) {
            AuditLog.Log("Background Selector: select: " + selected);

            // Handle free panels
            Image img1 = panelFreeType1.GetComponent<Image>();
            Image img2 = panelFreeType2.GetComponent<Image>();
            Image img3 = panelFreeType3.GetComponent<Image>();
            
            img1.color = UnityEngine.Color.black;
            img2.color = UnityEngine.Color.black;
            img3.color = UnityEngine.Color.black;

            // Handle owned NFT panels
            if (panelOwnedNfts != null) {
                foreach (GameObject panel in panelOwnedNfts) {
                    if (panel != null) {
                        Image panelImage = panel.GetComponent<Image>();
                        if (panelImage != null) {
                            panelImage.color = UnityEngine.Color.black;
                        }
                    }
                }
            }

            // Set the selected panel to red
            if (selected >= 1 && selected <= 3) {
                switch (selected) {
                    case 1:
                        img1.color = UnityEngine.Color.red;
                        break;
                    case 2:
                        img2.color = UnityEngine.Color.red;
                        break;
                    case 3:
                        img3.color = UnityEngine.Color.red;
                        break;
                }
            } else if (selected >= 100) {
                // Find the owned NFT panel with this ID
                if (panelOwnedNfts != null) {
                    foreach (GameObject panel in panelOwnedNfts) {
                        if (panel != null) {
                            Button button = panel.GetComponentInChildren<Button>();
                            if (button != null && 
                                (button.name == $"button_Gen1_{selected}") ||
                                (button.name == $"button_Gen2_{selected}")
                            ){
                                Image panelImage = panel.GetComponent<Image>();
                                if (panelImage != null) {
                                    panelImage.color = UnityEngine.Color.red;
                                }
                                else {
                                    AuditLog.Log("Background Selector: Panel image is null");
                                }
                                break;
                            }
                            else {
                                if (button == null) {
                                    AuditLog.Log("Background Selector: Button is null");
                                }
                                else {
                                    // This is expected - no need to log
                                    // Debug.Log("Background Selector: Incorrect button name: " + button.name);
                                }
                            }
                        }
                        else {
                            AuditLog.Log("Background Selector: Panel is null");
                        }
                    }
                }
            } else {
                AuditLog.Log("Background Selector: Unknown option: " + selected);
            }
        }

        private void drawOwnedPanel() {
            // Get the owned backgrounds
            int[] owned = ScreenBackground.GetOwned();
            
            // Clear existing panels if any
            if (panelOwnedNfts != null) {
                foreach (GameObject panel in panelOwnedNfts) {
                    Destroy(panel);
                }
            }
            
            // Reset scroll position to top
            if (scrollRect != null) {
                scrollRect.normalizedPosition = new Vector2(0, 1);
            }
            
            // Resize panelOwned based on number of NFTs
            RectTransform panelOwnedRect = panelOwned.GetComponent<RectTransform>();
            int topOfScreen = 225+220+20;
            float panelHeight = topOfScreen + (owned.Length * 220) + 20; // 225 initial offset + (220 per panel) + 20 padding at bottom
            panelOwnedRect.sizeDelta = new Vector2(panelOwnedRect.sizeDelta.x, panelHeight);

            RectTransform panelOwnedContentRect = panelOwnedContent.GetComponent<RectTransform>();
            panelOwnedContentRect.sizeDelta = new Vector2(panelOwnedContentRect.sizeDelta.x, panelHeight);
            
            // Create new array for panels
            panelOwnedNfts = new GameObject[owned.Length];
            
            // Create panels for each owned background
            for (int i = 0; i < owned.Length; i++) {
                SceneInfo sceneInfo = BackgroundsMetadata.GetInfo(owned[i]);
                if (sceneInfo.resource == null) {
                    AuditLog.Log("No resource found for NFT id: " + owned[i]);
                    continue;
                }
                string buttonId = "button_" + sceneInfo.series + "_" + owned[i];

                // Create outer container panel
                GameObject buttonContainer = new GameObject(buttonId);
                buttonContainer.transform.SetParent(panelOwned.transform, false);

                // Add RectTransform to outer container
                RectTransform buttonRect = buttonContainer.AddComponent<RectTransform>();
                buttonRect.anchorMin = new Vector2(0.5f, 1f);
                buttonRect.anchorMax = new Vector2(0.5f, 1f);
                buttonRect.anchoredPosition = new Vector2(0, -topOfScreen - (i * 220));
                buttonRect.sizeDelta = new Vector2(750, 210); // Width of parent less a bit
 
                // Add Image component for background
                Image buttonImage = buttonContainer.AddComponent<Image>();
                buttonImage.color = Color.black;
                buttonImage.sprite = Resources.Load<Sprite>("UI/Sprites/UI_Background");

                // Create button container (left side)
                GameObject imageContainer = new GameObject($"imageContainer_{i}");
                imageContainer.transform.SetParent(buttonContainer.transform, false);

                // Add RectTransform to button container
                RectTransform imageContainerRect = imageContainer.AddComponent<RectTransform>();
                imageContainerRect.anchorMin = new Vector2(0, 0.5f);
                imageContainerRect.anchorMax = new Vector2(0, 0.5f);
                imageContainerRect.anchoredPosition = new Vector2(105, 0); // Half of button width
                imageContainerRect.sizeDelta = new Vector2(210, 210);

                // Create button
                GameObject button = new GameObject($"OwnedButton_{i}");
                button.transform.SetParent(imageContainer.transform, false);
                
                // Add RectTransform to button
                RectTransform imageRect = button.AddComponent<RectTransform>();
                imageRect.anchorMin = new Vector2(0.5f, 0.5f);
                imageRect.anchorMax = new Vector2(0.5f, 0.5f);
                imageRect.anchoredPosition = Vector2.zero;
                imageRect.sizeDelta = new Vector2(190, 190);
                
                // Add Image component to button
                Image imageImage = button.AddComponent<Image>();

                Texture2D tex = Resources.Load<Texture2D>(sceneInfo.resource);
                if (tex == null) {
                    AuditLog.Log("ERROR: Resource not found: " + sceneInfo.resource);
                }
                Rect size = new Rect(0.0f, 0.0f, tex.width, tex.height);
                Vector2 pivot = new Vector2(0.0f, 0.0f);
                Sprite s = Sprite.Create(tex, size, pivot);
                imageImage.sprite = s;
                
                // Add Button component
                Button buttonComponent = buttonContainer.AddComponent<Button>();
                
                int offset = BackgroundsMetadata.OptionToType(owned[i]);
                int gen = BackgroundsMetadata.OptionToGeneration(owned[i]);
                button.name = $"gen{gen}_{offset}"; // Set the button name to match the format
                buttonComponent.onClick.AddListener(() => OnButtonClick(button.name));
                
                // Create text container (right side)
                GameObject textContainer = new GameObject($"TextContainer_{i}");
                textContainer.transform.SetParent(buttonContainer.transform, false);

                // Add RectTransform to text container
                RectTransform textContainerRect = textContainer.AddComponent<RectTransform>();
                textContainerRect.anchorMin = new Vector2(0, 0);
                textContainerRect.anchorMax = new Vector2(1, 1);
                textContainerRect.offsetMin = new Vector2(220, 0); // Start after button
                textContainerRect.offsetMax = new Vector2(-20, 0); // Padding on right

                // Add TextMeshProUGUI component
                TextMeshProUGUI textMesh = textContainer.AddComponent<TextMeshProUGUI>();
                textMesh.fontSize = 40;
                textMesh.alignment = TextAlignmentOptions.Left;
                textMesh.color = Color.white;
                textMesh.textWrappingMode = TextWrappingModes.Normal;
                string text = sceneInfo.name + "\n" +
                                sceneInfo.series + " " + sceneInfo.rarity + "\n" +
                                sceneInfo.artist;
                if (sceneInfo.artist == "Unknown") {
                    text = text + " Artist";
                }
                textMesh.text = text;
                
                // Store panel reference
                panelOwnedNfts[i] = buttonContainer;
            }
        }

        private void StartLoaders() {
            // Only load the days played once per day.
            // uint statsGameDay = (uint) Stats.GetLastGameDay();
            // uint gameDay = (uint) Timeline.GameDay();
            // if (statsGameDay == gameDay) {
            //     int daysPlayed = Stats.GetDaysPlayed();
            //     int daysClaimed = Stats.GetDaysClaimed();
            //     daysPlayedLoaded(daysPlayed, daysClaimed);
            // }
            // else {
                AuditLog.Log("Scene Screen: Loading days played and days claimed");
                loadRoutineDaysPlayed = StartCoroutine(LoadRoutineDaysPlayed());
                loadRoutineDaysClaimed = StartCoroutine(LoadRoutineDaysClaimed());
            // }
        }

        IEnumerator LoadRoutineDaysPlayed() {
            FetchDaysPlayed();
            yield return new WaitForSeconds(0.1f);
        }
        IEnumerator LoadRoutineDaysClaimed() {
            FetchDaysClaimed();
            yield return new WaitForSeconds(0.1f);
        }

        private async void FetchDaysPlayed() {
            string account = PassportStore.GetPassportAccount();
            if (account == null || account == "") {
                return;
            }
            DaysPlayed = (int) (await fourteenNumbersContracts.GetDaysPlayed(account));
            //Stats.SetDaysPlayed(DaysPlayed);
            if (DaysClaimed != NOT_SET) {
                daysPlayedLoaded(DaysPlayed, DaysClaimed);
            } 
            else {
                AuditLog.Log("DaysPlayed loaded before DaysClaimed");
            }
        }

        private async void FetchDaysClaimed() {
            string account = PassportStore.GetPassportAccount();
            DaysClaimed = (int) (await fourteenNumbersClaimContracts.GetDaysClaimed(account));
            //Stats.SetDaysClaimed(DaysClaimed);
            if (DaysPlayed != NOT_SET) {
                daysPlayedLoaded(DaysPlayed, DaysClaimed);
            } 
            else {
                AuditLog.Log("DaysClaimed loaded before DaysPlayed");
            }
        }

        public void daysPlayedLoaded(int daysPlayed, int daysClaimed) {
            AuditLog.Log("Days played: " + daysPlayed.ToString() + ", days claimed: " + daysClaimed.ToString());
            if (daysClaimed > daysPlayed) {
                AuditLog.Log("Days claimed greater than days played");
            }
            int net = daysPlayed - daysClaimed;

            if (net >= 30) {
                claimButton.interactable = true;
            }
            else {
                int daysBeforeClaim = 30 - net;
                claimButtonText.text = daysBeforeClaim.ToString() + " days until claim";
                claimButtonText.fontSize = 60;
            }
        }
    }
}