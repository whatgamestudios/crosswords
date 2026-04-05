using System;

namespace CrossWords
{

    public class Solution
    {
        public uint GameDay;
        public uint Score;
        public string BoardString;

        public Solution(uint gameDay, uint score, string board)
        {
            GameDay = gameDay;
            Score = score;
            BoardString = board;
        }

        public static Solution FromSerialised(string serialised)
        {
            string gameDayS = serialised.Substring(0,5);
            uint gameDay = UInt32.Parse(gameDayS);
            string scoreS = serialised.Substring(5, 2);
            uint score = UInt32.Parse(scoreS);
            int numCells = Board.BOARD_SIZE * Board.BOARD_SIZE;
            string board = serialised.Substring(7, numCells);

            return new Solution(gameDay, score, board);
        }

        public string Serialise()
        {
            return String.Format("{0,5}{1,2}{2}", GameDay, Score, BoardString);
        }
    }
}
