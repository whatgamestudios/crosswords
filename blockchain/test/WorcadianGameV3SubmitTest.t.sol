// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {Test} from "forge-std/Test.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";
import {WorcadianGameV1} from "../src/WorcadianGameV1.sol";
import {WorcadianGameV2} from "../src/WorcadianGameV2.sol";
import {WorcadianGameV3} from "../src/WorcadianGameV3.sol";

// ── External contract mocks ───────────────────────────────────────────────────

/// @dev Stand-in passport implementation contract — just needs bytecode.
contract MockPassportImpl {}

/// @dev Stand-in passport proxy. Runtime bytecode is identical for every instance,
///      so every instance shares the same extcodehash.
contract MockPassportWallet {
    address private _impl;
    constructor(address impl_) { _impl = impl_; }
    /// forge-lint: disable-next-line(mixed-case-function)
    function PROXY_getImplementation() external view returns (address) { return _impl; }
}

/// @dev Configurable mock for IWordListSeed.getSeedWord.
contract MockWordListSeed {
    mapping(uint256 => string) private _words;

    function setSeedWord(uint256 gameDay, string calldata word) external {
        _words[gameDay] = word;
    }

    function getSeedWord(uint256 gameDay) external view returns (string memory) {
        return _words[gameDay];
    }
}

/// @dev Configurable mock for IWorcadianWordList.inWordListBulk.
///      Stores a result array; unset positions default to false.
contract MockWordList {
    bool[] private _results;

    function setResults(bool[] calldata results) external {
        delete _results;
        for (uint256 i = 0; i < results.length; i++) {
            _results.push(results[i]);
        }
    }

    function inWordListBulk(bytes[] calldata words) external view returns (bool[] memory results) {
        results = new bool[](words.length);
        for (uint256 i = 0; i < words.length && i < _results.length; i++) {
            results[i] = _results[i];
        }
    }
}

// ── Test contract ─────────────────────────────────────────────────────────────

contract WorcadianGameV3SubmitTest is Test {
    WorcadianGameV3    game;
    MockPassportImpl   passportImpl;
    MockPassportWallet passportWallet;
    MockPassportWallet passportWallet2;
    MockWordListSeed   mockSeed;
    MockWordList       mockWordList;

    address admin        = makeAddr("admin");
    address ownerAddr    = makeAddr("owner");
    address upgradeAdmin = makeAddr("upgradeAdmin");
    address stranger     = makeAddr("stranger");

    // Game epoch: Monday March 30, 2026 00:00:00 UTC (matches GameDayCheckV2).
    uint256 constant GAME_START = 1774828800;
    uint256 constant DAY        = 86400;

    // Day used across most tests.  Noon on day 15 is unambiguously within the
    // valid window [minDay, maxDay] returned by determineCurrentGameDays().
    uint32 constant GAME_DAY = 15;

    // Scores for the simple ONLY board:
    //   "ONLY" in dict     → 26 - 4 unique letters = 22
    //   "ONLY" not in dict → 26 + 4 unique letters = 30
    uint256 constant SCORE_IN_DICT     = 22;
    uint256 constant SCORE_NOT_IN_DICT = 30;
    //   "ONLY" and "SKY" in dict → 26 - 4 - 2 unique letters = 20
    uint256 constant SCORE_GOOD_BOARD  = 20;

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// @dev Noon UTC on _gameDay — valid for both minDay and maxDay windows.
    function noonOn(uint32 gameDay) internal pure returns (uint256) {
        return GAME_START + uint256(gameDay) * DAY + DAY / 2;
    }

    /**
     * @dev Minimal valid 11×11 board containing only the word "ONLY".
     *
     *  Seed-word placement for "ONLY" (len 4):
     *    startX = (11 - 4) / 2 = 3   →  columns 3-6
     *    row    = 11 / 2        = 5
     *
     *  Row 5 = "   ONLY    " (3 leading spaces, ONLY, 4 trailing spaces).
     *  All other rows are blank.
     *
     *  AnalyseBoard starts at centre cell (5,5) = board[60] = 'L' (occupied).
     *  It finds the single horizontal word "ONLY"; no other letters → ["ONLY"].
     */
    function _onlyBoard() internal pure returns (string memory) {
        return string.concat(
            "           ",  // row 0
            "           ",  // row 1
            "           ",  // row 2
            "           ",  // row 3
            "           ",  // row 4
            "   ONLY    ",  // row 5 – seed word centred at columns 3-6
            "           ",  // row 6
            "           ",  // row 7
            "           ",  // row 8
            "           ",  // row 9
            "           "   // row 10
        );
    }

    function _goodBoard() internal pure returns (string memory) {
        return string.concat(
            "           ",  // row 0
            "           ",  // row 1
            "           ",  // row 2
            "      S    ",  // row 3
            "      K    ",  // row 4
            "   ONLY    ",  // row 5 – seed word centred at columns 3-6
            "           ",  // row 6
            "           ",  // row 7
            "           ",  // row 8
            "           ",  // row 9
            "           "   // row 10
        );
    }


    /// @dev Set the mock word list so every word lookup returns `value`.
    function _setAllInDict(bool value) internal {
        bool[] memory r = new bool[](1);
        r[0] = value;
        mockWordList.setResults(r);
    }

    /// @dev Set the mock word list so every word lookup returns true`.
    function _setTwoInDict() internal {
        bool[] memory r = new bool[](2);
        r[0] = true;
        r[1] = true;
        mockWordList.setResults(r);
    }


    /// @dev Call submitBoard from passportWallet on GAME_DAY with the ONLY board.
    function _submitOnly(uint256 score_) internal {
        vm.prank(address(passportWallet));
        game.submitBoard(GAME_DAY, score_, _onlyBoard());
    }

    function _deployProxy(
        address _admin, address _owner, address _upgradeAdmin,
        address _passportWallet, address _wordListSeed, address _wordList
    ) internal returns (WorcadianGameV3) {
        WorcadianGameV1 implV1 = new WorcadianGameV1();
        ERC1967Proxy proxy = new ERC1967Proxy(
            address(implV1),
            abi.encodeCall(WorcadianGameV1.initialize,
                (_admin, _owner, _upgradeAdmin, _passportWallet, _wordListSeed, _wordList))
        );
        WorcadianGameV2 implV2 = new WorcadianGameV2();
        vm.prank(upgradeAdmin);
        WorcadianGameV1(address(proxy)).upgradeToAndCall(
            address(implV2), abi.encodeCall(WorcadianGameV2.upgradeStorage, (bytes("")))
        );

        WorcadianGameV3 implV3 = new WorcadianGameV3();
        vm.prank(upgradeAdmin);
        WorcadianGameV1(address(proxy)).upgradeToAndCall(
            address(implV3), abi.encodeCall(WorcadianGameV2.upgradeStorage, (bytes("")))
        );

        return WorcadianGameV3(address(proxy));
    }

    function setUp() public {
        passportImpl    = new MockPassportImpl();
        passportWallet  = new MockPassportWallet(address(passportImpl));
        passportWallet2 = new MockPassportWallet(address(passportImpl));
        mockSeed        = new MockWordListSeed();
        mockWordList    = new MockWordList();

        game = _deployProxy(
            admin, ownerAddr, upgradeAdmin,
            address(passportWallet), address(mockSeed), address(mockWordList)
        );

        // Default seed word for GAME_DAY.
        mockSeed.setSeedWord(GAME_DAY, "ONLY");

        // Default: ONLY is in the dictionary → calculated score = 22.
        _setAllInDict(true);

        vm.warp(noonOn(GAME_DAY));
    }

    // ── Guard: passport check ─────────────────────────────────────────────────

    function test_SubmitBoard_NotPassport_Reverts() public {
        vm.prank(stranger);
        vm.expectRevert(
            abi.encodeWithSelector(WorcadianGameV1.NotPassportWallet.selector, stranger)
        );
        game.submitBoard(GAME_DAY, SCORE_IN_DICT, _onlyBoard());
    }

    // ── Guard: game day ───────────────────────────────────────────────────────

    function test_SubmitBoard_FutureGameDay_Reverts() public {
        vm.prank(address(passportWallet));
        vm.expectRevert();
        game.submitBoard(GAME_DAY + 3, SCORE_IN_DICT, _onlyBoard());
    }

    function test_SubmitBoard_PastGameDay_Reverts() public {
        vm.prank(address(passportWallet));
        vm.expectRevert();
        game.submitBoard(GAME_DAY - 3, SCORE_IN_DICT, _onlyBoard());
    }

    // ── Guard: score not competitive ──────────────────────────────────────────

    function test_SubmitBoard_ScoreNotCompetitive_EmitsEvent() public {
        // First submission establishes bestScore = 22.
        _submitOnly(SCORE_IN_DICT);

        // A higher (worse) score triggers the early-exit event.
        vm.expectEmit(false, false, false, true, address(game));
        emit WorcadianGameV1.ScoreNotCompetitive(SCORE_NOT_IN_DICT, SCORE_IN_DICT);

        vm.prank(address(passportWallet));
        game.submitBoard(GAME_DAY, SCORE_NOT_IN_DICT, _onlyBoard());
    }

    function test_SubmitBoard_ScoreNotCompetitive_DoesNotRecordSubmission() public {
        _submitOnly(SCORE_IN_DICT); // submissionCount becomes 1

        vm.prank(address(passportWallet));
        game.submitBoard(GAME_DAY, SCORE_NOT_IN_DICT, _onlyBoard()); // rejected early

        assertEq(game.submissionCount(GAME_DAY), 1);
    }

    function test_SubmitBoard_EqualScore_IsNotRejected() public {
        // The early exit fires only when _score > best.  Equal score should proceed.
        _submitOnly(SCORE_IN_DICT);
        _submitOnly(SCORE_IN_DICT); // same score — must not revert or early-exit
        assertEq(game.submissionCount(GAME_DAY), 2);
    }

    function test_SubmitBoard_FirstSubmission_NoEarlyExit() public {
        // When submissionCount == 0 the early-exit guard is not reached, so any
        // score is accepted (the score-mismatch path still records it).
        _setAllInDict(true);
        // Submit with a very high score — first-ever submission must go through.
        vm.prank(address(passportWallet));
        game.submitBoard(GAME_DAY, 999, _onlyBoard()); // _score != calculatedScore=22
        assertEq(game.submissionCount(GAME_DAY), 1);
    }

    // ── Guard: board size ─────────────────────────────────────────────────────

    function test_SubmitBoard_BoardTooShort_Reverts() public {
        string memory shortBoard = "ABCDE"; // 5 chars instead of 121
        vm.prank(address(passportWallet));
        vm.expectRevert(
            abi.encodeWithSelector(WorcadianGameV1.InvalidBoardSize.selector, uint256(5))
        );
        game.submitBoard(GAME_DAY, SCORE_IN_DICT, shortBoard);
    }

    function test_SubmitBoard_BoardTooLong_Reverts() public {
        string memory longBoard = string.concat(_onlyBoard(), " "); // 122 chars
        vm.prank(address(passportWallet));
        vm.expectRevert(
            abi.encodeWithSelector(WorcadianGameV1.InvalidBoardSize.selector, uint256(122))
        );
        game.submitBoard(GAME_DAY, SCORE_IN_DICT, longBoard);
    }

    // ── Guard: seed word ──────────────────────────────────────────────────────

    function test_SubmitBoard_SeedWordNotFound_Reverts() public {
        // Mock returns "FROST" but the board only contains "ONLY".
        mockSeed.setSeedWord(GAME_DAY, "FROST");
        vm.prank(address(passportWallet));
        vm.expectRevert(
            abi.encodeWithSelector(WorcadianGameV1.SeedWordNotFound.selector, "FROST")
        );
        game.submitBoard(GAME_DAY, SCORE_IN_DICT, _onlyBoard());
    }

    function test_SubmitBoard_SeedWordEmptyString_Reverts() public {
        // _isSeedWordOnBoard returns false when wordLen == 0.
        mockSeed.setSeedWord(GAME_DAY, "");
        vm.prank(address(passportWallet));
        vm.expectRevert(
            abi.encodeWithSelector(WorcadianGameV1.SeedWordNotFound.selector, "")
        );
        game.submitBoard(GAME_DAY, SCORE_IN_DICT, _onlyBoard());
    }

    function test_SubmitBoard_SeedWordTooLong_Reverts() public {
        // A word longer than BOARD_SIZE (11) can never be centred on the board.
        mockSeed.setSeedWord(GAME_DAY, "ABCDEFGHIJKL"); // 12 chars
        vm.prank(address(passportWallet));
        vm.expectRevert(); // SeedWordNotFound
        game.submitBoard(GAME_DAY, SCORE_IN_DICT, _onlyBoard());
    }

    function test_SubmitBoard_SeedWordAtWrongColumn_Reverts() public {
        // Board has "ONLY" offset by one column from the expected centred position.
        // Row 5 should be "   ONLY    " (startX=3) but here it starts at column 4.
        string memory offsetBoard = string.concat(
            "           ",
            "           ",
            "           ",
            "           ",
            "           ",
            "    ONLY   ",  // ONLY at col 4-7, not the required col 3-6
            "           ",
            "           ",
            "           ",
            "           ",
            "           "
        );
        vm.prank(address(passportWallet));
        vm.expectRevert(
            abi.encodeWithSelector(WorcadianGameV1.SeedWordNotFound.selector, "ONLY")
        );
        game.submitBoard(GAME_DAY, SCORE_IN_DICT, offsetBoard);
    }

    // ── Valid submission: events ──────────────────────────────────────────────

    function test_SubmitBoard_ValidBoard_EmitsBoardSubmitted() public {
        vm.expectEmit(true, true, false, true, address(game));
        emit WorcadianGameV1.BoardSubmitted(GAME_DAY, SCORE_IN_DICT, address(passportWallet));

        _submitOnly(SCORE_IN_DICT);
    }

    // ── Valid submission: state ───────────────────────────────────────────────

    function test_SubmitBoard_ValidBoard_UpdatesSubmissionCount() public {
        assertEq(game.submissionCount(GAME_DAY), 0);
        _submitOnly(SCORE_IN_DICT);
        assertEq(game.submissionCount(GAME_DAY), 1);
    }

    function test_SubmitBoard_ValidBoard_UpdatesBestScoreByDay() public {
        _submitOnly(SCORE_IN_DICT);
        assertEq(game.bestScoreByDay(GAME_DAY), SCORE_IN_DICT);
    }

    function test_SubmitBoard_ValidBoard_StoresPlayerAndBoard() public {
        _submitOnly(SCORE_IN_DICT);

        WorcadianGameV1.Submission[] memory subs =
            game.getSubmissions(GAME_DAY, SCORE_IN_DICT, 0, 10);
        assertEq(subs.length, 1);
        assertEq(subs[0].player, address(passportWallet));
        assertEq(subs[0].board, _onlyBoard());
    }

    // ── Score calculation ─────────────────────────────────────────────────────

    function test_SubmitBoard_AllWordsInDict_ScoreDecrements() public {
        // ONLY in dict: 4 unique letters → 26 - 4 = 22.
        _setAllInDict(true);
        _submitOnly(SCORE_IN_DICT);
        assertEq(game.bestScoreByDay(GAME_DAY), SCORE_IN_DICT);
    }

    function test_SubmitBoard_NoWordsInDict_ScoreIncrements() public {
        // ONLY not in dict: 4 unique letters → 26 + 4 = 30.
        _setAllInDict(false);
        _submitOnly(SCORE_NOT_IN_DICT);
        assertEq(game.bestScoreByDay(GAME_DAY), SCORE_NOT_IN_DICT);
    }

    function test_SubmitBoard_ScoreMismatch_EmitsEvent() public {
        // ONLY in dict → calculatedScore = 22, but client submits 99.
        _setAllInDict(true);
        uint256 wrongScore = 99;

        vm.expectEmit(false, false, false, true, address(game));
        emit WorcadianGameV1.ScoreMismatch(wrongScore, SCORE_IN_DICT);

        vm.prank(address(passportWallet));
        game.submitBoard(GAME_DAY, wrongScore, _onlyBoard());
    }

    function test_SubmitBoard_ScoreMismatch_StoresCalculatedScore() public {
        // Even when the submitted score is wrong, the on-chain calculated score
        // is what gets recorded in bestScoreByDay and the submission index.
        _setAllInDict(true);
        vm.prank(address(passportWallet));
        game.submitBoard(GAME_DAY, 99, _onlyBoard());

        assertEq(game.bestScoreByDay(GAME_DAY), SCORE_IN_DICT); // 22, not 99
        assertEq(game.getSubmissionCountAtScore(GAME_DAY, SCORE_IN_DICT), 1);
        assertEq(game.getSubmissionCountAtScore(GAME_DAY, 99), 0);
    }

    function test_SubmitBoard_ScoreMismatch_DoesntStoreIfBetter() public {
        // Even when the submitted score is wrong, the on-chain calculated score
        // is what gets used. If there is already a better solution, does not 
        // get stored as best score.
        _setTwoInDict();
        vm.prank(address(passportWallet));
        game.submitBoard(GAME_DAY, 20, _goodBoard());
        assertEq(game.bestScoreByDay(GAME_DAY), SCORE_GOOD_BOARD); 

        vm.prank(address(passportWallet));
        game.submitBoard(GAME_DAY, 19, _onlyBoard()); // will be caculated as 22.
        // Best score should not change.
        assertEq(game.bestScoreByDay(GAME_DAY), SCORE_GOOD_BOARD, "Best score"); 
        assertEq(game.getSubmissionCountAtScore(GAME_DAY, SCORE_IN_DICT), 0, "Count at 22");
        assertEq(game.getSubmissionCountAtScore(GAME_DAY, SCORE_GOOD_BOARD), 1, "Count at 20");
        assertEq(game.getSubmissionCountAtScore(GAME_DAY, 19), 0, "Count at 19");
    }


    // ── Multi-submission: better score ────────────────────────────────────────

    function test_SubmitBoard_BetterScore_UpdatesBestScoreByDay() public {
        _setAllInDict(false);
        _submitOnly(SCORE_NOT_IN_DICT); // bestScore = 30
        assertEq(game.bestScoreByDay(GAME_DAY), SCORE_NOT_IN_DICT);

        _setAllInDict(true);
        _submitOnly(SCORE_IN_DICT); // better: bestScore → 22
        assertEq(game.bestScoreByDay(GAME_DAY), SCORE_IN_DICT);
    }

    function test_SubmitBoard_BetterScore_SubmissionCountIncrements() public {
        _setAllInDict(false);
        _submitOnly(SCORE_NOT_IN_DICT);

        _setAllInDict(true);
        _submitOnly(SCORE_IN_DICT);

        assertEq(game.submissionCount(GAME_DAY), 2);
    }

    function test_SubmitBoard_BetterScore_OldTierPreserved() public {
        // The original score-30 submission is still retrievable after a better
        // submission is recorded; only getBestSubmissions changes.
        _setAllInDict(false);
        _submitOnly(SCORE_NOT_IN_DICT);

        _setAllInDict(true);
        vm.prank(address(passportWallet2));
        game.submitBoard(GAME_DAY, SCORE_IN_DICT, _onlyBoard());

        assertEq(game.getSubmissionCountAtScore(GAME_DAY, SCORE_NOT_IN_DICT), 1);
        assertEq(game.getSubmissionCountAtScore(GAME_DAY, SCORE_IN_DICT), 1);
    }

    // ── Multi-submission: tied score ─────────────────────────────────────────

    function test_SubmitBoard_TiedScore_BothStoredAtSameTier() public {
        _submitOnly(SCORE_IN_DICT);

        vm.prank(address(passportWallet2));
        game.submitBoard(GAME_DAY, SCORE_IN_DICT, _onlyBoard());

        assertEq(game.getSubmissionCountAtScore(GAME_DAY, SCORE_IN_DICT), 2);
        assertEq(game.submissionCount(GAME_DAY), 2);
    }

    function test_SubmitBoard_TiedScore_BestScoreUnchanged() public {
        _submitOnly(SCORE_IN_DICT);

        vm.prank(address(passportWallet2));
        game.submitBoard(GAME_DAY, SCORE_IN_DICT, _onlyBoard());

        assertEq(game.bestScoreByDay(GAME_DAY), SCORE_IN_DICT);
    }

    // ── Multi-submission: worse score ─────────────────────────────────────────

    function test_SubmitBoard_WorseScore_BestScoreByDayUnchanged() public {
        _submitOnly(SCORE_IN_DICT); // bestScore = 22

        vm.prank(address(passportWallet));
        game.submitBoard(GAME_DAY, SCORE_NOT_IN_DICT, _onlyBoard()); // rejected

        assertEq(game.bestScoreByDay(GAME_DAY), SCORE_IN_DICT); // still 22
    }

    function test_SubmitBoard_WorseScore_SubmissionCountUnchanged() public {
        _submitOnly(SCORE_IN_DICT); // submissionCount = 1

        vm.prank(address(passportWallet));
        game.submitBoard(GAME_DAY, SCORE_NOT_IN_DICT, _onlyBoard()); // rejected

        assertEq(game.submissionCount(GAME_DAY), 1);
    }

    // ── getBestSubmissions ────────────────────────────────────────────────────

    function test_GetBestSubmissions_BeforeAnySubmission_ReturnsEmpty() public view {
        WorcadianGameV1.Submission[] memory subs = game.getBestSubmissions(GAME_DAY);
        assertEq(subs.length, 0);
    }

    function test_GetBestSubmissions_ReturnsCurrentBestTier() public {
        _submitOnly(SCORE_IN_DICT);

        WorcadianGameV1.Submission[] memory subs = game.getBestSubmissions(GAME_DAY);
        assertEq(subs.length, 1);
        assertEq(subs[0].player, address(passportWallet));
    }

    function test_GetBestSubmissions_UpdatesAfterBetterSubmission() public {
        // First player submits a worse score.
        _setAllInDict(false);
        _submitOnly(SCORE_NOT_IN_DICT);

        // Second player submits a better score.
        _setAllInDict(true);
        vm.prank(address(passportWallet2));
        game.submitBoard(GAME_DAY, SCORE_IN_DICT, _onlyBoard());

        WorcadianGameV1.Submission[] memory best = game.getBestSubmissions(GAME_DAY);
        assertEq(best.length, 1);
        assertEq(best[0].player, address(passportWallet2));
    }

    function test_GetBestSubmissions_TiedScores_ReturnsAllAtBestTier() public {
        _submitOnly(SCORE_IN_DICT);

        vm.prank(address(passportWallet2));
        game.submitBoard(GAME_DAY, SCORE_IN_DICT, _onlyBoard());

        WorcadianGameV1.Submission[] memory best = game.getBestSubmissions(GAME_DAY);
        assertEq(best.length, 2);
    }

    // ── getSubmissions ────────────────────────────────────────────────────────

    function test_GetSubmissions_StartBeyondEnd_ReturnsEmpty() public {
        _submitOnly(SCORE_IN_DICT); // 1 submission stored

        WorcadianGameV1.Submission[] memory subs =
            game.getSubmissions(GAME_DAY, SCORE_IN_DICT, 5, 10);
        assertEq(subs.length, 0);
    }

    function test_GetSubmissions_CountExceedsAvailable_Clamped() public {
        _submitOnly(SCORE_IN_DICT);

        WorcadianGameV1.Submission[] memory subs =
            game.getSubmissions(GAME_DAY, SCORE_IN_DICT, 0, 100);
        assertEq(subs.length, 1);
    }

    function test_GetSubmissions_Pagination() public {
        // Three separate wallets submit at the same score.
        address[3] memory wallets;
        for (uint256 i = 0; i < 3; i++) {
            wallets[i] = address(new MockPassportWallet(address(passportImpl)));
            vm.prank(wallets[i]);
            game.submitBoard(GAME_DAY, SCORE_IN_DICT, _onlyBoard());
        }

        WorcadianGameV1.Submission[] memory page1 =
            game.getSubmissions(GAME_DAY, SCORE_IN_DICT, 0, 2);
        WorcadianGameV1.Submission[] memory page2 =
            game.getSubmissions(GAME_DAY, SCORE_IN_DICT, 2, 2);

        assertEq(page1.length, 2);
        assertEq(page1[0].player, wallets[0]);
        assertEq(page1[1].player, wallets[1]);
        assertEq(page2.length, 1);
        assertEq(page2[0].player, wallets[2]);
    }

    // ── getSubmissionCountAtScore ─────────────────────────────────────────────

    function test_GetSubmissionCountAtScore_ReturnsCorrectCount() public {
        assertEq(game.getSubmissionCountAtScore(GAME_DAY, SCORE_IN_DICT), 0);
        _submitOnly(SCORE_IN_DICT);
        assertEq(game.getSubmissionCountAtScore(GAME_DAY, SCORE_IN_DICT), 1);
        vm.prank(address(passportWallet2));
        game.submitBoard(GAME_DAY, SCORE_IN_DICT, _onlyBoard());
        assertEq(game.getSubmissionCountAtScore(GAME_DAY, SCORE_IN_DICT), 2);
    }

    function test_GetSubmissionCountAtScore_UnknownScore_ReturnsZero() public view {
        assertEq(game.getSubmissionCountAtScore(GAME_DAY, 999), 0);
    }


    // -- analyseBoard --------------------------------------------------
    function test_AnalyseBoard_Only() public {
        _setAllInDict(true);
        vm.prank(address(passportWallet));
        (uint256 score, string[] memory words, bool[] memory inDict) = game.analyseBoard(_onlyBoard());
        assertEq(score, SCORE_IN_DICT, "score");
        assertEq(words.length, 1, "words len");
        assertEq(words[0], "ONLY", "first word");
        assertEq(inDict.length, 1, "in dict length");
        assertEq(inDict[0], true, "in dict value");
    }
}
