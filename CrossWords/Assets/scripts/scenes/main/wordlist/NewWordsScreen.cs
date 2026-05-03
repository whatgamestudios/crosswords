using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CrossWords { 

    public class NewWordsScreen : MonoBehaviour
    {
        public TextMeshProUGUI InfoText;
        public RectTransform scrollContent;

        void Start()
        {
            AuditLog.Log($"New Words screen");
            InfoText.text = 
                "\n" +
                "Words added in Worcadian version 1.4 (May 5, 2026):\n" +
                "bluray, bromances, btw, csv, devops, dox, fatbergs, gf, " +
                "goji, imao, ip, jp, luv, pdf, podcast, podcast, rofl, " +
                "sexting, stablecoin, ty, uv, vape, vaped, vaper, vapes, " +
                "vaping, vlog\n" +
                "\n";
        }
    }
}
