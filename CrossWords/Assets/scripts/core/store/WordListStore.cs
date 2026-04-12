using UnityEngine;

namespace CrossWords
{

    public class WordListStore
    {
        public const string WORDLIST_IS_WORD = "WORDLIST_IS_WORD";
        public const string WORDLIST_STARTS_WITH = "WORDLIST_STARTS_WITH";
        public const string WORDLIST_HIGHLIGHT = "WORDLIST_HIGHLIGHT";

        public static void Store(bool highlightIsWord, string isWord, string startsWith)
        {
            PlayerPrefs.SetInt(WORDLIST_HIGHLIGHT, highlightIsWord ? 1 : 0);
            PlayerPrefs.SetString(WORDLIST_IS_WORD, isWord);
            PlayerPrefs.SetString(WORDLIST_STARTS_WITH, startsWith);
            PlayerPrefs.Save();
        }

        public static (bool, string, string) Load()
        {
            return (PlayerPrefs.GetInt(WORDLIST_HIGHLIGHT, 1) == 1,
                    PlayerPrefs.GetString(WORDLIST_IS_WORD, ""), 
                    PlayerPrefs.GetString(WORDLIST_STARTS_WITH, ""));
        }
    }
}
