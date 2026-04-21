using System;
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
            StoreToStorage();
        }

        public bool TryRemoveTop(out MoveEntry entry)
        {
            if (_stack.Count == 0)
            {
                entry = default;
                return false;
            }

            entry = _stack.Pop();
            StoreToStorage();
            return true;
        }

        public MoveEntry? RemoveTopOrNull()
        {
            if (_stack.Count == 0)
                return null;
            MoveEntry? maybe = _stack.Pop();
            StoreToStorage();
            return maybe;
        }

        public bool Remove(char target)
        {
            if (_stack.Count == 0)
            {
                return false;
            }

            Stack<MoveEntry> tempStack = new Stack<MoveEntry>();

            bool found = false;
            while (_stack.Count > 0)
            {
                MoveEntry current = _stack.Pop();
                if (current.Letter == target) 
                {
                    found = true;
                    break; // Found it, don't push it to temp
                }
                tempStack.Push(current);
            }

            // Push everything back in the original order
            while (tempStack.Count > 0)
            {
                _stack.Push(tempStack.Pop());
            }
            StoreToStorage();
            return found;
        }

        public void LoadFromStorage()
        {
            _stack.Clear();

            int size = 5;
            string serialised = MoveStackStore.Load();
            int len = serialised.Length / size;
            for (int i = len - 1; i >= 0; i--)
            {
                int startOfs = i * size;
                string xS = serialised.Substring(startOfs, 2);
                string yS = serialised.Substring(startOfs + 2, 2);
                string cS = serialised.Substring(startOfs + 4, 1);
                int x = Int32.Parse(xS);
                int y = Int32.Parse(yS);
                char c = cS[0];
                Add(c, x, y);              
            }
        }

        private void StoreToStorage()
        {
            string all = "";
            foreach (var item in _stack)
            {
                string s = String.Format("{0,2}{1,2}{2}", item.X, item.Y, item.Letter);
                all += s;               
            }
            MoveStackStore.Store(all);
        }

        public void Clear()
        {
            _stack.Clear();
            StoreToStorage();
        }

        public List<MoveEntry> GetEntriesBottomToTop()
        {
            var list = new List<MoveEntry>(_stack); // Stack enumerates top-to-bottom
            list.Reverse();
            return list;
        }

    }
}