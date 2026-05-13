// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System;

namespace CrossWords {
    public class CheckIn : MonoBehaviour {

        private readonly CheckInServerProcessor checkInProcessor = new CheckInServerProcessor();
        private bool isProcessing = false;

        public void Start() {
            AuditLog.Log("Checkin start");
            StartCheckinProcess();
        }

        private async void StartCheckinProcess() {
            if (isProcessing) {
                return;
            }

            if (Application.internetReachability == NetworkReachability.NotReachable) {
                AuditLog.Log("Checkin: No network connectivity available");
                return;
            }

            isProcessing = true;

            try {
                (_, string player) = UserId.GetUserId();
                uint gameDay = Timeline.GameDay();
                AuditLog.Log("Checkin transaction");
                CheckInResult result = await checkInProcessor.CheckIn((int)gameDay, player);
                AuditLog.Log($"Checkin: days_played={result.DaysPlayed}, is_new_day={result.IsNewDay}");
            }
            catch (Exception ex) {
                AuditLog.Log($"Exception in checkin process: {ex.Message}");
                PostHogStats.GetInstance().LogCheckinError(ex.Message);
            }
            finally {
                isProcessing = false;
            }
        }
    }
}