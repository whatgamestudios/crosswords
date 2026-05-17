// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace CrossWords {
    public class BoardHighlighting {

        public static (bool, uint) Highlight(Board board, string seedWord)
        {
            List<WordOnBoard> words = AnalyseBoard.Analyse(board);
            // AuditLog.Log("Words:");
            // foreach (WordOnBoard word in words)
            // {
            //     AuditLog.Log(word.Word);
            // }


            WordListDictionary wordListDictionary = WordListDictionary.Instance;
            if (wordListDictionary == null)
            {
                AuditLog.Log("ERROR: No dictionary");
                return (false, 0);
            }
            if (!wordListDictionary.DictionaryLoaded)
            {
                AuditLog.Log("ERROR: Dictionary not loaded");
                return (false, 0);
            }

            //board.RestoreAllCellsVisual();
            board.SetStarterWord(seedWord);

            bool first = true;
            foreach (WordOnBoard word in words)
            {
                if (!first)
                {
                    board.HighlightInDictionaryCells(word.StartX, word.StartY, word.Length(), word.IsHorizontal());
                }
                first = false;
            }
            int i = 0;
            bool[] inDictionary = new bool[100];
            foreach (WordOnBoard word in words)
            {
                bool inDic = wordListDictionary.IsInDictionary(word.Word);
                //AuditLog.Log($"Words: {word.Word} in dic: {inDic}");
                inDictionary[i++] = inDic;
                if (!inDic)
                {
                    board.HighlightNotInDictionaryCells(word.StartX, word.StartY, word.Length(), word.IsHorizontal());
                }
            }


            //board.DumpBoard();

            uint score = ScoreCalculator.Score(inDictionary, words);
            return (true, score);
        }
    }
}