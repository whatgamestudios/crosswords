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
                "Words added in Worcadian version 2.7 (June 25, 2026):\n" +
                "bling\n" +
                "\n" +
                "Words added in Worcadian version 2.6 (June 13, 2026):\n" +
                "zine, throuple\n" +
                "\n" +
                "\n";
        }
    }
}
