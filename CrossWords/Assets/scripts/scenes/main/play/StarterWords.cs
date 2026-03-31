namespace CrossWords {

    public class StarterWords
    {
        public const int STARTER_WORD_LENGTH = 5;


        static readonly string[] Words =
        {
            "MOUSE",
            "WHOLE",
            "PEARL",
            "BRICK",
            "QUICK",
            "CEDAR",
            "SOUTH",
            "IMAGE",
        };

        public static string GetStarterWord(uint gameDay)
        {
            long i = gameDay % Words.Length;
            if (i < 0)
                i += Words.Length;

            return Words[i];
        }
    }
}
