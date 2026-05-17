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
                "Words added in Worcadian version 2.3 (May 18, 2026):\n" +
                "pj, pjs, fx, vj, oj\n" +
                "\n" +
                "Words added in Worcadian version 2.0 (May 10, 2026):\n" +
                "karens, unfriends, wip\n" +
                "\n" +
                "Words added in Worcadian version 1.4 (May 5, 2026):\n" +
                "bluray, bromance, bromances, btw, csv, devops, dox, fatbergs, gf, " +
                "goji, imao, ip, jp, luv, pdf, podcast, podcast, rofl, " +
                "sexting, stablecoin, ty, uv, vape, vaped, vaper, vapes, " +
                "vaping, vlog\n" +
                "\n";
        }
    }
}
