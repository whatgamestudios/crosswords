// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

namespace CrossWords {

    public class ClaimProbabilitiesScreen : MonoBehaviour {
        public GameObject panelOwned;
        public GameObject panelOwnedContent;

        private GameObject[] panelAvailableNfts;
        private ScrollRect scrollRect;
        private List<ClaimableTokenDTO> claimableNfts = new List<ClaimableTokenDTO>();
        private bool loaded = false;
        private bool displayed = false;

        private FourteenNumbersClaimContract fourteenNumbersClaimContracts = new FourteenNumbersClaimContract();

        private Coroutine claimableNftsRoutine;
        public TextMeshProUGUI loadingText;

        public void Start() {
            AuditLog.Log("Claim Probabilities screen");
            // Get the ScrollRect component
            scrollRect = panelOwned.GetComponentInParent<ScrollRect>();
            if (scrollRect == null) {
                AuditLog.Log("ERROR: No ScrollRect found in parent of panelOwned");
                return;
            }

            StartLoaders();
        }

        public void Update() {
            if (loaded && !displayed) {
                displayed = true;
                loadingText.gameObject.SetActive(false);
                drawAvailablePanel();
            }
        }

        private void drawAvailablePanel() {
            limitClaimableNftsTo100Percent();
            int numClaimable = claimableNfts.Count;
            int height = 280;
            
            // Clear existing panels if any
            if (panelAvailableNfts != null) {
                foreach (GameObject panel in panelAvailableNfts) {
                    Destroy(panel);
                }
            }
            
            // Reset scroll position to top
            if (scrollRect != null) {
                scrollRect.normalizedPosition = new Vector2(0, 1);
            }
            
            // Resize panelOwned based on number of NFTs
            RectTransform panelOwnedRect = panelOwned.GetComponent<RectTransform>();
            int topOfScreen = 180;
            float panelHeight = topOfScreen + (numClaimable * height) + 20; // 225 initial offset + (220 per panel) + 20 padding at bottom
            panelOwnedRect.sizeDelta = new Vector2(panelOwnedRect.sizeDelta.x, panelHeight);

            RectTransform panelOwnedContentRect = panelOwnedContent.GetComponent<RectTransform>();
            panelOwnedContentRect.sizeDelta = new Vector2(panelOwnedContentRect.sizeDelta.x, panelHeight);

            // Create new array for panels
            panelAvailableNfts = new GameObject[numClaimable];
            
            // Create panels for each background
            int i = 0;
            foreach (var nft in claimableNfts) {
                int tokenId = (int) nft.TokenId;
                SceneInfo sceneInfo = BackgroundsMetadata.GetInfo(tokenId);
                if (sceneInfo.resource == null) {
                    AuditLog.Log("No resource found for NFT id: " + tokenId);
                    continue;
                }

                // Create outer container panel
                GameObject holderContainer = new GameObject();
                holderContainer.transform.SetParent(panelOwned.transform, false);

                // Add RectTransform to outer container
                RectTransform buttonRect = holderContainer.AddComponent<RectTransform>();
                buttonRect.anchorMin = new Vector2(0.5f, 1f);
                buttonRect.anchorMax = new Vector2(0.5f, 1f);
                buttonRect.anchoredPosition = new Vector2(0, -topOfScreen - (i * height));
                buttonRect.sizeDelta = new Vector2(750, height - 10); // Width of parent less a bit
 
                // Add Image component for background
                Image buttonImage = holderContainer.AddComponent<Image>();
                buttonImage.color = Color.black;
                buttonImage.sprite = Resources.Load<Sprite>("UI/Sprites/UI_Background");

                // Create button container (left side)
                GameObject imageContainer = new GameObject();
                imageContainer.transform.SetParent(holderContainer.transform, false);

                // Add RectTransform to button container
                RectTransform imageContainerRect = imageContainer.AddComponent<RectTransform>();
                imageContainerRect.anchorMin = new Vector2(0, 0.5f);
                imageContainerRect.anchorMax = new Vector2(0, 0.5f);
                imageContainerRect.anchoredPosition = new Vector2(105, 0); // Half of button width
                imageContainerRect.sizeDelta = new Vector2(210, 210);

                // Create button
                GameObject button = new GameObject();
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
                    AuditLog.Log("Resource not found: " + sceneInfo.resource);
                }
                Rect size = new Rect(0.0f, 0.0f, tex.width, tex.height);
                Vector2 pivot = new Vector2(0.0f, 0.0f);
                Sprite s = Sprite.Create(tex, size, pivot);
                imageImage.sprite = s;
                                
                // Create text container (right side)
                GameObject textContainer = new GameObject();
                textContainer.transform.SetParent(holderContainer.transform, false);

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
                int available = (int) nft.Balance;
                double dropRate = ((double) nft.Percentage) / 100.0;

                string text = sceneInfo.name + "\n" +
                                sceneInfo.series + " " + sceneInfo.rarity + "\n" +
                                "Artist: " + sceneInfo.artist + "\n" +
                                "Available: " + available + "\n" + 
                                "Drop Rate: " + dropRate + "%";
                textMesh.text = text;
                
                // Store panel reference
                panelAvailableNfts[i] = holderContainer;
                i++;
            }
        }

        private void StartLoaders() {
            AuditLog.Log("Claim Probabilities Screen: Loading");
            claimableNftsRoutine = StartCoroutine(LoadClaimableNfts());
        }

        IEnumerator LoadClaimableNfts() {
            FetchClaimableNfts();
            yield return new WaitForSeconds(0.1f);
        }

        private async void FetchClaimableNfts() {
            try {
                claimableNfts = await fourteenNumbersClaimContracts.GetClaimableNfts();
                AuditLog.Log($"Fetched {claimableNfts.Count} claimable NFTs");
                loaded = true;
                
                // // Log details of each NFT for debugging
                // foreach (var nft in claimableNfts) {
                //     AuditLog.Log($"NFT Contract: {nft.Erc1155Contract}");
                //     AuditLog.Log($"Token ID: {nft.TokenId}");
                //     AuditLog.Log($"Balance: {nft.Balance}");
                //     AuditLog.Log($"Percentage: {nft.Percentage / 100.0}%");
                // }
            }
            catch (Exception ex) {
                AuditLog.Log($"Error fetching claimable NFTs: {ex.Message}");
                claimableNfts = new List<ClaimableTokenDTO>();
            }
        }


        private void limitClaimableNftsTo100Percent() {
            List<ClaimableTokenDTO> claimableNftsLimited = new List<ClaimableTokenDTO>();

            int numClaimable = claimableNfts.Count;
            double totalPercentage = 0.0;
            int defaultOffset = -1;
            int i = 0;
            foreach (var nft in claimableNfts) {
                double dropRate = ((double) nft.Percentage) / 100.0;
                if (dropRate == 0.0) {
                    // This is a default token
                    if (defaultOffset == -1) {
                        defaultOffset = i;
                    }
                }
                else if (totalPercentage < 100.0) {
                    double newTotal = totalPercentage + dropRate;
                    if (newTotal > 100) {
                        dropRate = 100 - totalPercentage;
                        nft.Percentage = (uint) dropRate * 100;
                        // Assign 100.0 rather than totalPercentage + dropRate just in case there is a rounding / quantaization issue.
                        totalPercentage = 100.0; 
                    }
                    else {
                        totalPercentage = newTotal;
                    }
                    claimableNftsLimited.Add(nft);
                }
                i++;
            }

            if (totalPercentage < 100.0 && defaultOffset != -1) {
                claimableNfts[defaultOffset].Percentage = (uint)((100.0 - totalPercentage) * 100.0);
                claimableNftsLimited.Add(claimableNfts[defaultOffset]);
            }
            claimableNfts = claimableNftsLimited;
        }
    }
}