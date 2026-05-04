// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text;


namespace CrossWords {

    public class TextShare : MonoBehaviour {

        public void OnButtonClick(string buttonText)
        {
            if (buttonText != "round" && buttonText != "square" && buttonText != "inverse")
            {
                AuditLog.Log($"Share Text: Unknown button: {buttonText}");
            }

            string style = buttonText;

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
                    msg += translate(style, row) + "\n";    
                }
            }
            msg += "Score: " + score;

            AuditLog.Log($"Shared: {msg}");
            SunShineNativeShare.instance.ShareText(msg, msg);
        }

        private string translate(string style, string solution) {
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
                    string mathMonoSpaceC;
                    if (style == "square") 
                    {
                        // Squared Latin Capital Letter
                        mathMonoSpaceC = char.ConvertFromUtf32(0x1F130 + charVal);
                    }
                    else if (style == "round")
                    {
                        // Negative Circled Latin Capital Letter
                        mathMonoSpaceC = char.ConvertFromUtf32(0x1F150 + charVal);
                    }
                    else if (style == "inverse")
                    {
                        // Negative Squared Latin Capital Letter
                        mathMonoSpaceC = char.ConvertFromUtf32(0x1F170 + charVal);
                    }
                    else
                    {
                        // Regional Indicator Symbol Letter
                        mathMonoSpaceC = char.ConvertFromUtf32(0x1F1E6 + charVal);
                    }
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
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    int charVal = i;
                    string mathMonoSpaceC = char.ConvertFromUtf32(0x1F1E6 + charVal);
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