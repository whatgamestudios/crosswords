// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace CrossWords {

    public class BestScoreLoader : MonoBehaviour {

        // The best score so far today.
        public static uint BestScore { get; private set; }

        public static bool LoadedBestScore { get; private set; }

        private readonly GameProcessor gameProcessor = new GameProcessor();
        private Coroutine loadRoutine;

        public static void RestartLoadRoutine() {
            BestScoreLoader loader = FindFirstObjectByType<BestScoreLoader>();
            if (loader == null) {
                AuditLog.Log("BestScoreLoader.RestartLoadRoutine: no instance found");
                return;
            }
            if (!loader.isActiveAndEnabled) {
                AuditLog.Log("BestScoreLoader.RestartLoadRoutine: instance inactive or disabled");
                return;
            }
            if (loader.loadRoutine != null) {
                loader.StopCoroutine(loader.loadRoutine);
                loader.loadRoutine = null;
            }
            LoadedBestScore = false;
            loader.loadRoutine = loader.StartCoroutine(loader.LoadRoutine());
        }

        public void OnStart() {
            LoadedBestScore = false;
            BestScore = 53;
        }

        public void OnEnable() {
            loadRoutine = StartCoroutine(LoadRoutine());
        }

        // Force the best score to be reloaded each time the game screen is entered.
        public void OnDisable()
        {
            LoadedBestScore = false;
            if (loadRoutine != null) {
                StopCoroutine(loadRoutine);
                loadRoutine = null;
            }
        }


        IEnumerator LoadRoutine() {
            uint gameDay = Timeline.GameDay();
            Task<int> task = gameProcessor.GetBestScore(gameDay);
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.IsFaulted) {
                AuditLog.Log($"BestScoreLoader: GetBestScore failed: {task.Exception}");
            } else {
                BestScore = (uint)task.Result;
            }
            LoadedBestScore = true;
        }
    }
}