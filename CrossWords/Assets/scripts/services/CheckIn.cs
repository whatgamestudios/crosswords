// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using System;

namespace CrossWords {
    public class CheckIn : MonoBehaviour {

        CheckInProcessor contract;

        string status;
        private bool isProcessing = false;

        public void Start() {
            AuditLog.Log("Checkin start");
            contract = new CheckInProcessor();
            StartCheckinProcess();
        }

        private async void StartCheckinProcess() {
            if (isProcessing) {
                return;
            }
            if (!PassportStore.IsLoggedIn()) {
                return;
            }

            // Check network connectivity
            if (Application.internetReachability == NetworkReachability.NotReachable) {
                AuditLog.Log("Checkin: No network connectivity available");
                return;
            }

            isProcessing = true;

            try {
                await PassportLogin.InitAndLogin();
                uint gameDay = Timeline.GameDay();
                AuditLog.Log("Checkin transaction");
                var checkInSuccess = await contract.SubmitCheckIn(gameDay);
                AuditLog.Log("Checkin: " + checkInSuccess.ToString());
            }
            catch (Exception ex) {
                AuditLog.Log($"Exception in checkin process: {ex.Message}");
            }
            finally {
                isProcessing = false;
            }
        }
    }
}