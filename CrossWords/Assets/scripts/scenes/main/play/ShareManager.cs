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

msg += check();

                // DebugPanel.SetActive(true);
                // DebugText.text = msg;
                AuditLog.Log($"Shared: {msg}");
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
            
            for (int i = 0; i < solution.Length; i++)
            {
                char c = sbi[i];
                if (c == ' ')
                {
                    sbo.Append('\u3000');
                }
                else
                {
                    int charVal = c - 'A';
//                    string mathMonoSpaceC = char.ConvertFromUtf32(0x1F130 + charVal);
//                    string mathMonoSpaceC = char.ConvertFromUtf32(0x1F150 + charVal);
                    string mathMonoSpaceC = char.ConvertFromUtf32(0x1F170 + charVal);
                    sbo.Append(mathMonoSpaceC);
                }
            }


            return sbo.ToString();
        }

        private string check() {
            StringBuilder sbo = new StringBuilder();
            sbo.Append("\n");

            int num = 11;
            
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    int charVal = i;
                    string mathMonoSpaceC = char.ConvertFromUtf32(0x1F130 + charVal);
                    sbo.Append(mathMonoSpaceC);
                }
                sbo.Append("X\n");
            }
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    int charVal = i;
                    string mathMonoSpaceC = char.ConvertFromUtf32(0x1F150 + charVal);
                    sbo.Append(mathMonoSpaceC);
                }
                sbo.Append("X\n");
            }
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    int charVal = i;
                    string mathMonoSpaceC = char.ConvertFromUtf32(0x1F150 + charVal);
                    sbo.Append(mathMonoSpaceC);
                }
                sbo.Append("X\n");
            }

            for (int j = 0; j < num; j++)
            {
                sbo.Append('\u3000');
            }
            sbo.Append("X3000\n");

            for (int j = 0; j < num; j++)
            {
                sbo.Append('\u205F');
            }
            sbo.Append("X205F\n");

            for (int j = 0; j < num; j++)
            {
                sbo.Append('\u200A');
            }
            sbo.Append("X200A\n");
            for (int j = 0; j < num; j++)
            {
                sbo.Append('\u2009');
            }
            sbo.Append("X2009\n");
            for (int j = 0; j < num; j++)
            {
                sbo.Append('\u2008');
            }
            sbo.Append("X2008\n");

            for (int j = 0; j < num; j++)
            {
                sbo.Append("\u2007");
            }
            sbo.Append("X2007\n");
            for (int j = 0; j < num; j++)
            {
                sbo.Append('\u2006');
            }
            sbo.Append("X2006\n");
            for (int j = 0; j < num; j++)
            {
                sbo.Append('\u2005');
            }
            sbo.Append("X2005\n");

            for (int j = 0; j < num; j++)
            {
                sbo.Append('\u2003');
            }
            sbo.Append("X2003\n");
            for (int j = 0; j < num; j++)
            {
                sbo.Append('\u2002');
            }
            sbo.Append("X2002\n");
            for (int j = 0; j < num; j++)
            {
                sbo.Append('\u2001');
            }
            sbo.Append("X2001\n");
            for (int j = 0; j < num; j++)
            {
                sbo.Append('\u2000');
            }
            sbo.Append("X2000\n");

            return sbo.ToString();
        }

    }
}