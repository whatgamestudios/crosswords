// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Threading.Tasks;
using System.Collections;


namespace CrossWords {

    public class VideoOrchestrator : MonoBehaviour {

        public GameObject BlockInteractionPanel;

        private Coroutine coroutine;

        

        private bool isRunning = false;

        private const int STYLE_WORCADIAN_PURPLE = 0;
        private const int STYLE_LETTERS = 1;
        private int style;

        public void Start() {
            string msg = MessagePass.GetMsg();
            AuditLog.Log($"Video Orchestrator: {msg}");
            if (msg == null) {
                return;
            }
            else if (msg == "video-worcadian-purple")
            {
                style = STYLE_WORCADIAN_PURPLE;
            }
            else if (msg == "video-letters")
            {
                style = STYLE_LETTERS;
            }
            else
            {
                return;
            }
            MessagePass.SetMsg("video");
            startCoroutine();
        }

        public void OnDisable() {
            stopCoroutine();
        }


        private void startCoroutine() {
            if (!isRunning) {
                coroutine = StartCoroutine(PlaySequenceRoutine());
                isRunning = true;
            }
        }

        public void stopCoroutine() {
            if (isRunning && coroutine != null) {
                StopCoroutine(coroutine);
                isRunning = false;
            }
        }

        private IEnumerator PlaySequenceRoutine()
        {
            AuditLog.Log("Play Sequence start");
            BlockInteractionPanel.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            BlockInteractionPanel.SetActive(true);

            if (style == STYLE_LETTERS) {
                AnimatorWorcadianPurple animatorWorcadianPurple = FindFirstObjectByType<AnimatorWorcadianPurple>();
                animatorWorcadianPurple.Init();
                yield return new WaitForSeconds(3f);
                animatorWorcadianPurple.End();
            }
            else
            {
                AnimatorTitle animatorTitle = FindFirstObjectByType<AnimatorTitle>();
                animatorTitle.Init();
                yield return new WaitForSeconds(3f);
                animatorTitle.End();
            }

            ReplayManager replayManager = FindFirstObjectByType<ReplayManager>();
            replayManager.OnReplayButtonPress();
            yield return new WaitForSeconds(0.1f);
            while (replayManager.IsReplaying)
            {
                yield return new WaitForSeconds(0.1f);
            }
            BlockInteractionPanel.SetActive(true);

            yield return new WaitForSeconds(1.5f);
            BlockInteractionPanel.SetActive(false);

            BackButtonHandler.GoBack();
            AuditLog.Log("Play Sequence end");

        }

    }     
}