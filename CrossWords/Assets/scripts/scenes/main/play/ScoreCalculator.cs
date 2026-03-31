using System.Collections.Generic;

namespace CrossWords {

    public class ScoreCalculator
    {
        public const int LETTERS_IN_ALPHABET = 26;

        public static int Score(List<WordOnBoard> words)
        {
            bool[] used = new bool[LETTERS_IN_ALPHABET];
            int score = LETTERS_IN_ALPHABET;

            foreach (WordOnBoard word in words) {
                string wordStr = word.Word;
                foreach (char ch in wordStr.ToCharArray())
                {
                    int chInt = ch - 'A';
                    if (!used[chInt])
                    {
                        score--;
                        used[chInt] = true;
                    }
                }
            }
            return score;            
        }
    }
}