using System.Collections.Generic;
using System.Linq; // Required for the .AsEnumerable() extension method


namespace CrossWords {

    public class ScoreCalculator
    {
        public const uint LETTERS_IN_ALPHABET = 26;

        public static uint Score(bool[] inDictionary, List<WordOnBoard> words)
        {
            bool[] used = new bool[LETTERS_IN_ALPHABET];
            uint score = LETTERS_IN_ALPHABET;

            int i = 0;
            foreach (WordOnBoard word in words) {
                bool inDic = inDictionary[i++];
                string wordStr = word.Word;
                foreach (char ch in wordStr.ToCharArray())
                {
                    int chInt = ch - 'A';
                    if (!used[chInt])
                    {
                        if (inDic)
                        {
                            score--;
                        }
                        else
                        {
                            score++;
                        }
                        used[chInt] = true;
                    }
                }
            }
            return score;            
        }
    }
}