// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text;


namespace CrossWords {

    public class ShareManager : MonoBehaviour {
        public GameObject panelShare;

        public void Start()
        {
            panelShare.SetActive(true);
        }


        // public void Update()
        // {
        //     GameState gameState = GameState.Instance();
        //     if (GameState.Instance().IsPlayerStateDone())
        //     {
        //         panelShare.SetActive(true);
        //     }
        // }

        public void OnButtonClick(string buttonText)
        {
            if (buttonText == "Share")
            {
                (bool exists, Solution sol) = Stats.GetCurrent();
                if (!exists)
                {
                    return;
                }

                uint gameDay = sol.GameDay;
                uint score = sol.Score;
                string board = sol.BoardString;

                string msg =
                    "Worcadian\n" +
                    "Game day " + gameDay + "\n";
                for (int i = 0; i < Board.BOARD_SIZE; i++)
                {
                    msg += board.Substring(i * Board.BOARD_SIZE, Board.BOARD_SIZE) + "\n";
                }
                msg += "Score: " + score;
                AuditLog.Log("Share: \n" + msg);

                msg = translate(msg);
                AuditLog.Log("Share2: \n" + msg);
                SunShineNativeShare.instance.ShareText(msg, msg);
            }
            else
            {
                AuditLog.Log($"Share Manager: Unknown button: {buttonText}");
            }
        }

        private string translate(string solution) {
            StringBuilder sb = new StringBuilder(solution);
            char fullWidthA = '\uFF21';
            for (int i = 0; i < solution.Length; i++)
            {
                char c = sb[i];
                if (c == ' ')
                {
                    c = '\u2005';
                }
                else
                {
                    c = (char) (c - 'A' + fullWidthA);
                }
                sb[i] = c;
            }
            return sb.ToString();
        }
    }
}