// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text;


namespace CrossWords {

    public class ShareManager : MonoBehaviour {
        public GameObject DebugPanel;
        public TextMeshProUGUI DebugText;

        public void Start()
        {
            DebugPanel.SetActive(false);
        }


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
                string[] rows = new string[Board.BOARD_SIZE];
                for (int i = 0; i < Board.BOARD_SIZE; i++)
                {
                    rows[i] = board.Substring(i * Board.BOARD_SIZE, Board.BOARD_SIZE);
                }


                board = translate(board);

                string msg =
                    "Worcadian\n" +
                    "Game day " + gameDay + "\n";
                for (int i = 0; i < Board.BOARD_SIZE; i++)
                {
                    string row = rows[i];
                    if (row == "           ")
                    {
                        // Ignore
                    }
                    else
                    {
                        msg += translate(row) + "\n";    
                    }
                }
                msg += "Score: " + score;
                // DebugPanel.SetActive(true);
                // DebugText.text = msg;
                SunShineNativeShare.instance.ShareText(msg, msg);
            }
            else
            {
                AuditLog.Log($"Share Manager: Unknown button: {buttonText}");
            }
        }

        private string translate(string solution) {
            StringBuilder sbi = new StringBuilder(solution);
            StringBuilder sbo = new StringBuilder();
            
            // Implement Mathematical Monospace A (\U0001D670) as a Unicode Surrogate Pair.
            // See: https://www.russellcottrell.com/greek/utilities/SurrogatePairCalculator.htm
            // char mathematicalMonospaceAHigh = '\uD835';
            // char mathematicalMonospaceALow = '\uDE70';
            char smallSpace = '\u2009';
            char figureSpace = '\u2007';
            for (int i = 0; i < solution.Length; i++)
            {
                char c = sbi[i];
                if (c == ' ')
                {
                    sbo.Append(figureSpace);
                }
                else
                {
                    // int charVal = c - 'A';
                    // char low = (char) (mathematicalMonospaceALow + charVal);
                    // string mathematicalMonospace = new string(new char[] { mathematicalMonospaceAHigh, low });
                    // sbo.Append(mathematicalMonospace);
                    sbo.Append(c);
                    if (c == 'I')
                    {
                        sbo.Append(smallSpace);
                    }
                }
            }
            return sbo.ToString();
        }
    }
}