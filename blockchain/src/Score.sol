// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

/**
 * @notice Scoring algorithm for the Worcadian crossword game.
 * Mirrors ScoreCalculator.cs from the Unity client.
 *
 * Algorithm:
 *   Start with score = 26 (one point per letter of the alphabet).
 *   For each word, iterate its characters. The first time a letter is seen:
 *     - If the word is in the dictionary: score--
 *     - If the word is NOT in the dictionary: score++
 *   Duplicate letters (already seen in any prior word) are ignored.
 */
abstract contract Score {
    uint256 internal constant LETTERS_IN_ALPHABET = 26;

    /**
     * @notice Calculate the score for a set of words.
     * @dev Mirrors ScoreCalculator.Score(bool[] inDictionary, List<WordOnBoard> words).
     *      Words must be uppercase ASCII (A-Z).
     * @param _words   The words found on the board.
     * @param _inDictionary  Whether each word is in the dictionary (parallel array).
     * @return The calculated score.
     */
    function score(
        string[] memory _words,
        bool[] memory _inDictionary
    ) internal pure returns (uint256) {
        bool[LETTERS_IN_ALPHABET] memory used;
        uint256 currentScore = LETTERS_IN_ALPHABET;

        for (uint256 i = 0; i < _words.length; i++) {
            bool inDic = _inDictionary[i];
            bytes memory wordBytes = bytes(_words[i]);
            for (uint256 j = 0; j < wordBytes.length; j++) {
                uint8 ch = uint8(wordBytes[j]);
                // Expect uppercase A-Z (0x41–0x5A)
                if (ch >= 0x41 && ch <= 0x5A) {
                    uint256 idx = ch - 0x41;
                    if (!used[idx]) {
                        if (inDic) {
                            currentScore--;
                        } else {
                            currentScore++;
                        }
                        used[idx] = true;
                    }
                }
            }
        }

        return currentScore;
    }
}
