namespace CrossWords {

    public class TargetWords
    {
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

        public static string GetTargetWord(uint gameDay)
        {
            long i = gameDay % Words.Length;
            if (i < 0)
                i += Words.Length;

            return Words[i];
        }
    }
}
