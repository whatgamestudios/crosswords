using UnityEngine;

namespace CrossWords
{

    public class MoveStackStore
    {
        public const string MOVE_STACK = "MOVE_STACK";

        public static void Store(string moveStack)
        {
            PlayerPrefs.SetString(MOVE_STACK, moveStack);
            PlayerPrefs.Save();
        }

        public static string Load()
        {
            return PlayerPrefs.GetString(MOVE_STACK, "");
        }
    }
}
