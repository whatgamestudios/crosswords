// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {Test} from "forge-std/Test.sol";
import {ScoreV4} from "../src/ScoreV4.sol";

/**
 * Concrete harness to expose Score's internal function for testing.
 */
contract ScoreHarnessV4 is ScoreV4 {
    function exposedScore(
        string[] memory _words,
        bool[] memory _inDictionary
    ) external pure returns (uint256) {
        return score(_words, _inDictionary);
    }
}

contract ScoreTest is Test {
    ScoreHarnessV4 harness;

    function setUp() public {
        harness = new ScoreHarnessV4();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    function _single(string memory _word, bool _inDict)
        internal pure
        returns (string[] memory words, bool[] memory dicts)
    {
        words = new string[](1);
        words[0] = _word;
        dicts = new bool[](1);
        dicts[0] = _inDict;
    }

    function _pair(string memory _w0, bool _d0, string memory _w1, bool _d1)
        internal pure
        returns (string[] memory words, bool[] memory dicts)
    {
        words = new string[](2);
        words[0] = _w0;
        words[1] = _w1;
        dicts = new bool[](2);
        dicts[0] = _d0;
        dicts[1] = _d1;
    }

    // ── Empty input ───────────────────────────────────────────────────────────

    function test_Score_EmptyWordList_Returns26() public view {
        string[] memory words = new string[](0);
        bool[] memory dicts = new bool[](0);
        assertEq(harness.exposedScore(words, dicts), 26);
    }

    // ── Single word in dictionary ─────────────────────────────────────────────

    function test_Score_SingleWord_AllInDict_DeductsUniqueLetters() public view {
        // "ABC" in dict: A, B, C each seen for first time → 26 - 3 = 23
        (string[] memory w, bool[] memory d) = _single("ABC", true);
        assertEq(harness.exposedScore(w, d), 23);
    }

    function test_Score_SingleWord_NotInDict_AddsUniqueLetters() public view {
        // "ABC" not in dict: A, B, C each seen for first time → 26 + 3 = 29
        (string[] memory w, bool[] memory d) = _single("ABC", false);
        assertEq(harness.exposedScore(w, d), 29);
    }

    function test_Score_SingleLetter_InDict() public view {
        // "A" in dict → 26 - 1 = 25
        (string[] memory w, bool[] memory d) = _single("A", true);
        assertEq(harness.exposedScore(w, d), 25);
    }

    function test_Score_SingleLetter_NotInDict() public view {
        // "A" not in dict → 26 + 1 = 27
        (string[] memory w, bool[] memory d) = _single("A", false);
        assertEq(harness.exposedScore(w, d), 27);
    }

    // ── Duplicate letters ─────────────────────────────────────────────────────

    function test_Score_DuplicateLetters_SameWord_InDict() public view {
        // "AAAA" in dict: only the FIRST occurrence of A matters → 26 - 1 = 25
        (string[] memory w, bool[] memory d) = _single("AAAA", true);
        assertEq(harness.exposedScore(w, d), 25);
    }

    function test_Score_DuplicateLetters_SameWord_NotInDict() public view {
        // "AAAA" not in dict: all four As matter: 26 + 4 = 30
        (string[] memory w, bool[] memory d) = _single("AAAA", false);
        assertEq(harness.exposedScore(w, d), 30);
    }

    function test_Score_DuplicateLetters_AcrossWords_InDict() public view {
        // "ABC" + "ABD" both in dict:
        //   ABC → A(1), B(2), C(3) → 26 - 3 = 23
        //   ABD → A seen, B seen, D(4) → 23 - 1 = 22
        (string[] memory w, bool[] memory d) = _pair("ABC", true, "ABD", true);
        assertEq(harness.exposedScore(w, d), 22);
    }

    function test_Score_DuplicateLetters_AcrossWords_NotInDict() public view {
        // "ABC" + "ABD" both NOT in dict: 
        //   ABC: 26 + 3 = 29
        //   ABD: 29 + 3 = 32
        (string[] memory w, bool[] memory d) = _pair("ABC", false, "ABD", false);
        assertEq(harness.exposedScore(w, d), 32);
    }

    // ── Mixed dictionary status ───────────────────────────────────────────────

    function test_Score_Mixed_DictFirst_NotDictSecond() public view {
        // "ABC" in dict + "DEF" not in dict:
        //   ABC → -3 → 23
        //   DEF → +3 → 26
        (string[] memory w, bool[] memory d) = _pair("ABC", true, "DEF", false);
        assertEq(harness.exposedScore(w, d), 26);
    }

    function test_Score_Mixed_NotDictFirst_DictSecond() public view {
        // "ABC" not in dict + "DEF" in dict:
        //   ABC → +3 → 29
        //   DEF → -3 → 26
        (string[] memory w, bool[] memory d) = _pair("ABC", false, "DEF", true);
        assertEq(harness.exposedScore(w, d), 26);
    }

    function test_Score_Mixed_SharedLetters_DictFirstThenNot() public view {
        // "ABC" in dict (−3 → 23) then "ADE" not in dict (+3 → 26)
        (string[] memory w, bool[] memory d) = _pair("ABC", true, "ADE", false);
        assertEq(harness.exposedScore(w, d), 26);
    }

    function test_Score_Mixed_SharedLetters_NotDictFirstThenDict() public view {
        // "ABC" not in dict (+3 → 29) then "ADE" in dict (−3 → 26)
        (string[] memory w, bool[] memory d) = _pair("ABC", false, "ADE", true);
        assertEq(harness.exposedScore(w, d), 26);
    }

    // ── All 26 letters ────────────────────────────────────────────────────────

    function test_Score_AllAlphabet_InDict_ScoreZero() public view {
        // All 26 unique letters in a single dictionary word → 26 - 26 = 0
        (string[] memory w, bool[] memory d) = _single("ABCDEFGHIJKLMNOPQRSTUVWXYZ", true);
        assertEq(harness.exposedScore(w, d), 0);
    }

    function test_Score_AllAlphabet_NotInDict_ScoreFiftyTwo() public view {
        // All 26 unique letters in a single non-dictionary word → 26 + 26 = 52
        (string[] memory w, bool[] memory d) = _single("ABCDEFGHIJKLMNOPQRSTUVWXYZ", false);
        assertEq(harness.exposedScore(w, d), 52);
    }

    // ── Non-alphabetic characters ─────────────────────────────────────────────

    function test_Score_NonAlphaChars_Ignored() public view {
        // Space and digits are not A-Z (0x41-0x5A) and must be silently skipped.
        // "A 1" in dict → only A counts → 26 - 1 = 25
        (string[] memory w, bool[] memory d) = _single("A 1", true);
        assertEq(harness.exposedScore(w, d), 25);
    }

    // ── Real-game scenario ────────────────────────────────────────────────────

    // Words found by AnalyseBoard in Scenario 1 (see AnalyseBoardTest.t.sol):
    //   ONLY, FROST, LX, CHIVY, BF, QUET, WACK, GIMP, JUD, ZA
    //
    // Unique letters covered (in discovery order):
    //   ONLY  → O N L Y
    //   FROST → F R S T   (O already seen)
    //   LX    → X         (L already seen)
    //   CHIVY → C H I V   (Y already seen)
    //   BF    → B         (F already seen)
    //   QUET  → Q U E     (T already seen)
    //   WACK  → W A K     (C already seen)
    //   GIMP  → G M P     (I already seen)
    //   JUD   → J D       (U already seen)
    //   ZA    → Z         (A already seen)
    //
    // Total: 26 unique letters (full alphabet). Each seen once.

    function _scenario1Words() internal pure
        returns (string[] memory words)
    {
        words = new string[](10);
        words[0] = "ONLY";
        words[1] = "FROST";
        words[2] = "LX";
        words[3] = "CHIVY";
        words[4] = "BF";
        words[5] = "QUET";
        words[6] = "WACK";
        words[7] = "GIMP";
        words[8] = "JUD";
        words[9] = "ZA";
    }

    function test_Score_Scenario1_AllInDict_ScoreZero() public view {
        // Every unique letter (all 26) appears in a dictionary word → 26 - 26 = 0.
        string[] memory words = _scenario1Words();
        bool[] memory dicts = new bool[](10);
        for (uint256 i = 0; i < 10; i++) dicts[i] = true;
        assertEq(harness.exposedScore(words, dicts), 0);
    }

    function test_Score_Scenario1_NoneInDict_ScoreFiftyTwo() public view {
        // Every unique letter (all 26) appears in a non-dictionary word → 26 + 26 = 52.
        string[] memory words = _scenario1Words();
        bool[] memory dicts = new bool[](10);
        // all false by default
        assertEq(harness.exposedScore(words, dicts), 61);
    }

    function test_Score_Scenario1_PartialDict() public view {
        string[] memory words = _scenario1Words();
        bool[] memory dicts = new bool[](10);
        dicts[0] = true; // ONLY in dict; rest false
        assertEq(harness.exposedScore(words, dicts), 53);
    }

    function test_Score_Scenario2() public view {
        string[] memory words  = new string[](4);
        words[0] = "ONLY";
        words[1] = "XO";
        words[2] = "SKY";
        words[3] = "AX";

        bool[] memory dicts = new bool[](4);
        dicts[0] = true;
        dicts[1] = false;
        dicts[2] = true;
        dicts[3] = true;

        //                           ONLY  XO   SKY   XO
        uint256 expectedScore = 26 - 4   + 2  - 2   - 2; // = 20

        assertEq(harness.exposedScore(words, dicts), expectedScore);
    }
}
