using System.Collections.Generic;

namespace CrossWords {
    public readonly struct MoveEntry
    {
        public readonly char Letter;
        public readonly int X;
        public readonly int Y;

        public MoveEntry(char letter, int x, int y)
        {
            Letter = letter;
            X = x;
            Y = y;
        }
    }

    public class MoveStack
    {
        readonly Stack<MoveEntry> _stack = new Stack<MoveEntry>();

        public int Count => _stack.Count;

        public void Add(char letter, int x, int y)
        {
            _stack.Push(new MoveEntry(letter, x, y));
        }

        public bool TryRemoveTop(out MoveEntry entry)
        {
            if (_stack.Count == 0)
            {
                entry = default;
                return false;
            }

            entry = _stack.Pop();
            return true;
        }

        public MoveEntry? RemoveTopOrNull()
        {
            if (_stack.Count == 0)
                return null;
            return _stack.Pop();
        }
    }
}