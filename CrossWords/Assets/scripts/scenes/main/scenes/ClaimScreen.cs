// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using System;
using System.Numerics;

namespace CrossWords {

    public class ClaimScreen : MonoBehaviour {
        public TextMeshProUGUI info;
                    
        private const int TIME_PER_DOT = 1000;
        DateTime timeOfLastDot;

        FourteenNumbersClaimContract claimContract;

        string status;
        private bool isProcessing = false;
        private bool hasError = false;
        private string errorMessage = "";
        private BigInteger tokenId = BigInteger.Zero;

        public void Start() {
            AuditLog.Log("Claim screen");
            status = "Claiming ";
            claimContract = new FourteenNumbersClaimContract();
            timeOfLastDot = DateTime.Now;
            StartClaimProcess();
        }

        private async void StartClaimProcess() {
            if (isProcessing) {
                return;
            }
            isProcessing = true;
            hasError = false;
            errorMessage = "";
            tokenId = BigInteger.Zero;

            try {
                AuditLog.Log("Claim transaction");
                var (claimSuccess, claimedTokenId) = await claimContract.Claim();
                if (!claimSuccess) {
                    hasError = true;
                    errorMessage = "Error during claim: type 1";
                }
                else {
                    tokenId = claimedTokenId;
                }
            }
            catch (Exception ex) {
                hasError = true;
                errorMessage = "Error during claim: type 2";
                AuditLog.Log($"Exception in claim process: {ex.Message}");
            }
            finally {
                isProcessing = false;
            }
        }

        public void Update() {
            if (hasError) {
                info.text = errorMessage;
                return;
            }

            if (isProcessing) {
                DateTime now = DateTime.Now;
                if ((now - timeOfLastDot).TotalMilliseconds > TIME_PER_DOT) {
                    timeOfLastDot = now;
                    status = status + ".";
                }
                info.text = status;
            }
            else {
                status = $"Claim process complete! Token ID: {tokenId}";
                info.text = status;
            }
        }
    }
}