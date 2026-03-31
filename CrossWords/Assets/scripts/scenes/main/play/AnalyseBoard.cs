using System.Collections.Generic;


namespace CrossWords {

    public readonly struct WordOnBoard
    {
        public readonly string Word;
        public readonly int StartX;
        public readonly int StartY;

        public readonly int EndX;
        public readonly int EndY;

        public WordOnBoard(string word, int startX, int startY, int endX, int endY)
        {
            Word = word;
            StartX = startX;
            StartY = startY;
            EndX = endX;
            EndY = endY;
        }

        public int Length()
        {
            return EndY - StartY + EndX - StartX;
        }

        public override string ToString()
        {
            return $"Word: {Word}, Start: {StartX},{StartY}, End: {EndX},{EndY}";
        }
    }


    public class AnalyseBoard
    {
        const int BOARD_SIZE = Board.BOARD_SIZE;
        const int STARTER_WORD_LENGTH = StarterWords.STARTER_WORD_LENGTH;

        Board _board;

        bool[,] _analysedH;
        bool[,] _analysedV;

        AnalyseBoard(Board board)
        {
            _board = board;
        }

        public static List<WordOnBoard> Analyse(Board board)
        {
            return (new AnalyseBoard(board)).analyse();
        }

        List<WordOnBoard> analyse() 
        {
            _analysedH = new bool[BOARD_SIZE, BOARD_SIZE];
            _analysedV = new bool[BOARD_SIZE, BOARD_SIZE];

            List<WordOnBoard> wordList = new List<WordOnBoard>();

            int startX = BOARD_SIZE / 2;
            int startY = startX;

            if (!_board.IsCellOccupied(startX, startY))
            {
                AuditLog.Log("Analyse: Middle cell empty");
                return new List<WordOnBoard>();
            }

            WordOnBoard first = findHorizontalWord(startX, startY);
            wordList.Add(first);
            List<WordOnBoard> verticalWords = findVerticalWords(first);
            wordList.AddRange(verticalWords);

            return wordList;

        }

        private WordOnBoard findHorizontalWord(int x, int y)
        {
            //AuditLog.Log($"findHorizontalWord: {x}, {y}; occupied: {_board.IsCellOccupied(x, y)}, char: {_board.GetCell(x, y)}");
            while (x != 0 && _board.IsCellOccupied(x-1, y)) {
                x--;
            }
            int startX = x;
            while (x != BOARD_SIZE-1 && _board.IsCellOccupied(x+1, y)) 
            {
                x++;    
            }
            int endX = x;

            int length = endX - startX + 1;
            string w = "";
            for (int i=0; i < length; i++)
            {
                w += _board.GetCell(startX + i, y);
                _analysedH[startX + i, y] = true;
            }

            //AuditLog.Log($"findHorizontalWord: {new WordOnBoard(w, startX, y, endX, y)}");
            return new WordOnBoard(w, startX, y, endX, y);
        }

        WordOnBoard findVerticalWord(int x, int y)
        {
            //AuditLog.Log($"findVericalWord: {x}, {y}; occupied: {_board.IsCellOccupied(x, y)}, char: {_board.GetCell(x, y)}");

            while (y != 0 && _board.IsCellOccupied(x, y-1)) {
                y--;
            }
            int startY = y;

            while (y != BOARD_SIZE-1 && _board.IsCellOccupied(x, y+1)) 
            {
                y++;    
            }
            int endY = y;

            int length = endY - startY + 1;
            string w = "";
            for (int i=0; i < length; i++)
            {
                w += _board.GetCell(x, startY + i);
                _analysedV[x, startY + i] = true;
            }

            //AuditLog.Log($"findVerticalWord: {new WordOnBoard(w, x, startY, x, endY)}");
            return new WordOnBoard(w, x, startY, x, endY);
        }

        List<WordOnBoard> findHorizontalWords(WordOnBoard seedWord)
        {
            List<WordOnBoard> horizontalWordList = new List<WordOnBoard>();

            for (int i = 0; i <= seedWord.Length(); i++)
            {
                bool exists = false;
                int yofs = seedWord.StartY + i;
                if (seedWord.StartX != 0)
                {
                    if (_board.IsCellOccupied(seedWord.StartX - 1, yofs) && 
                        !_analysedH[seedWord.StartX - 1, yofs]) {
                        exists = true;
                    }
                }
                if (seedWord.StartX != BOARD_SIZE - 1)
                {
                    if (_board.IsCellOccupied(seedWord.StartX + 1, yofs) &&
                        !_analysedH[seedWord.StartX + 1, yofs]) {
                        exists = true;
                    }
                }
                if (exists)
                {
                    WordOnBoard horizontalWord = findHorizontalWord(seedWord.StartX, yofs);
                    horizontalWordList.Add(horizontalWord);
                    List<WordOnBoard> verticalWords = findVerticalWords(horizontalWord);
                    horizontalWordList.AddRange(verticalWords);
                }
            }
            return horizontalWordList;
        }

        List<WordOnBoard> findVerticalWords(WordOnBoard seedWord)
        {
            List<WordOnBoard> verticalWordList = new List<WordOnBoard>();

            for (int i = 0; i <= seedWord.Length(); i++)
            {
                int xofs = seedWord.StartX + i;
                bool exists = false;
                if (seedWord.StartY != 0)
                {
                    if (_board.IsCellOccupied(xofs, seedWord.StartY - 1) && 
                        !_analysedV[xofs, seedWord.StartY - 1]) {
                        exists = true;
                    }
                }
                if (seedWord.StartY != BOARD_SIZE - 1)
                {
                    if (_board.IsCellOccupied(xofs, seedWord.StartY + 1) &&
                        !_analysedV[xofs, seedWord.StartY + 1]) {
                        exists = true;
                    }
                }
                if (exists)
                {
                    WordOnBoard verticalWord = findVerticalWord(xofs, seedWord.StartY);
                    verticalWordList.Add(verticalWord);
                    List<WordOnBoard> horizontalWords = findHorizontalWords(verticalWord);
                    verticalWordList.AddRange(horizontalWords);
                }
            }
            return verticalWordList;
        }

    }
}