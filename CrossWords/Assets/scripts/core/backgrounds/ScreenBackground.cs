// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

namespace CrossWords {
    public class ScreenBackground {
        private const string SCENES_ERC1155 = "0x29c3A209d8423f9A53Bf8AD39bBb85087a2A938B";

        public static bool Syncing = false;

        public static bool FakeOwnership = false;        

        /**
         * Get the list of owned backgrounds.
         * 
         * @return array of owned background ids.
         */
        public static int[] GetOwned() {
            if (FakeOwnership) {
                return BackgroundsMetadata.GetAllOwnedNftIds();
            }
            return SceneStore.GetOwned();
        }

        public static bool IsOwned(int tokenId) {
            if (FakeOwnership) {
                return true;
            }
            int[] owned = SceneStore.GetOwned();
            foreach (int tokenIdOwned in owned) {
                if (tokenIdOwned == tokenId) {
                    return true;
                }
            }
            return false;
        }

        /**
         * Call the FetchImmutableNfts function and process the response.
         */
        public static void FetchAndProcessNfts(MonoBehaviour mono) {
            if (Syncing) {
                AuditLog.Log("Already syncing");
                return;
            }
            AuditLog.Log("Sync started");

            // Kick off the syncing process.
            string accountAddress = PassportStore.GetPassportAccount();
            mono.StartCoroutine(ScreenBackground.FetchImmutableNfts(
                accountAddress, SCENES_ERC1155, ProcessNftResponse));
        }


        /**
         * Fetch NFTs for a specific account and contract from Immutable X testnet.
         *
         * @param accountAddress The account address to fetch NFTs for.
         * @param contractAddress The contract address to fetch NFTs for.
         * @param callback Function to receive the JSON response string.
         */
        public static IEnumerator FetchImmutableNfts(string accountAddress, string contractAddress, System.Action<string> callback) {
            Syncing = true;
            string url = $"https://api.immutable.com/v1/chains/imtbl-zkevm-mainnet/accounts/{accountAddress}/nfts?contract_address={contractAddress}";
            //Debug.Log("url: " + url);
            using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(url)) {
                request.SetRequestHeader("Accept", "application/json");
                yield return request.SendWebRequest();

                if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success) {
                    callback?.Invoke(request.downloadHandler.text);
                } else {
                    AuditLog.Log($"ERROR: Error fetching NFTs: {request.error}");
                    callback?.Invoke(null);
                }
            }
        }

        /**
         * Process the JSON response by parsing it and logging the NFT data.
         *
         * @param json The JSON response string.
         */
        public static void ProcessNftResponse(string json) {
            //Debug.Log("json response: " + json);
            if (json != null) {
                try {
                    var nftData = JsonUtility.FromJson<NftResponse>(json);
                    if (nftData == null) {
                        AuditLog.Log("ERROR: NFT data null");
                    }
                    else if (nftData.result == null) {
                        AuditLog.Log("ERROR: nftData.result null");
                    }
                    else {
                        var tokenIds = new int[nftData.result.Length];
                        var balances = new int[nftData.result.Length];
                        for (int i = 0; i < nftData.result.Length; i++) {
                            tokenIds[i] = Int32.Parse(nftData.result[i].token_id);
                            balances[i] = Int32.Parse(nftData.result[i].balance);
                            //Debug.Log($"NFT ID: {tokenIds[i]}, Balance: {balances[i]}");
                        }
                        SceneStore.SetOwned(tokenIds, balances);
                        //Debug.Log("Token IDs: " + string.Join(", ", tokenIds));
                    }
                } catch (Exception e) {
                    AuditLog.Log($"ERROR: Error parsing NFT data: {e.Message}");
                }
                SceneStore.SetNftsSynced();
                AuditLog.Log("Synced NFT collection");
            } else {
                AuditLog.Log("Failed to fetch NFTs.");
            }
            Syncing = false;
        }

        [Serializable]
        private class NftResponse {
            public NftData[] result;
            //public PageData page;
        }

        [Serializable]
        private class NftData {
            //public Chain chain;
            public string token_id;
            // public string contract_address;
            // public string contract_type;
            // public string indexed_at;
            // public string updated_at;
            // public string metadata_synced_at;
            // public string metadata_id;
            // public string name;
            // public string description;
            // public string image;
            // public string external_link;
            // public string animation_url;
            // public string youtube_url;
            // public Attribute[] attributes;
            public string balance;
        }

        [Serializable]
        private class Chain {
            public string id;
            public string name;
        }

        [Serializable]
        private class Attribute {
            public string display_type;
            public string trait_type;
            public string value;
        }

        [Serializable]
        private class PageData {
            public string next_cursor;
            public string previous_cursor;
        }
    }
}
