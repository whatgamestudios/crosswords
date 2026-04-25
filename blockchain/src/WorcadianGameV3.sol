// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {WorcadianGameV2} from "./WorcadianGameV2.sol";
import {IWordListSeed, IWorcadianWordList} from "./WorcadianGameV1.sol";


/**
 * Add a better current day situation getter.
 */
contract WorcadianGameV3 is WorcadianGameV2 {

    uint256 internal constant _VERSION3 = 3;

    /**
     * @notice Function to be called when upgrading this contract.
     * @dev Call this function as part of upgradeToAndCall().
     * @dev This function does not need access control. Only allow upgrade from previous 
     *      version to this version.
     * @ param _data ABI encoded data to be used as part of the contract storage upgrade.
     */
    function upgradeStorage(
        bytes memory /* _data */
    )
        external
        override virtual
    {
        if (version == _VERSION2) {
            version = _VERSION3;
        }
        else {
            revert CanNotUpgradeToLowerOrSameVersion(version);
        }
    }


    /**
     * @notice Submit a completed board for a game day.
     * @dev See contract-level NatSpec for the full validation flow.
     * @param _gameDay  The game day the board was played on.
     * @param _score    Score computed client-side (must match on-chain calculation).
     * @param _board    121-character flat board string, row-major (uppercase A–Z / space).
     */
    function submitBoard(
        uint32 _gameDay,
        uint256 _score,
        string calldata _board
    ) external virtual override {
        // Game players must be using Passport wallet.
        if (!isPassport(msg.sender)) {
            revert NotPassportWallet(msg.sender);
        }

        // Only allow games to be played on the game day. This is to stop people
        // submitting games for the past or the future.
        checkGameDay(_gameDay);

        // Early exit: reject if submitted score is worse than today's best.
        // Lower score is better; 0 is optimal.
        uint256 dayCount = submissionCount[_gameDay];
        if (dayCount > 0 && _score > bestScoreByDay[_gameDay]) {
            // Emit an event rather than revert because this is a possible 
            // happy code path.
            emit ScoreNotCompetitive(_score, bestScoreByDay[_gameDay]);
            return;
        }

        // Validate board dimensions.
        bytes memory boardBytes = bytes(_board);
        if (boardBytes.length != BOARD_SIZE * BOARD_SIZE) {
            revert InvalidBoardSize(boardBytes.length);
        }

        // Fetch seed word and verify it is at the expected position:
        //   row    = BOARD_SIZE / 2
        //   startX = (BOARD_SIZE - wordLen) / 2
        // This is to ensure people don't submit a different day's game board with 
        // a different seed word.
        string memory seedWord = IWordListSeed(wordListSeed).getSeedWord(_gameDay);
        if (!_isSeedWordOnBoard(boardBytes, bytes(seedWord))) {
            revert SeedWordNotFound(seedWord);
        }

        // Extract all connected words from the board.
        string[] memory words = analyse(_board);

        // Look up all words in the word list.
        bool[] memory inDictionary = _checkWords(words);

        // Calculate the actual score and verify it matches the submitted value.
        uint256 calculatedScore = score(words, inDictionary);
        if (calculatedScore != _score) {
            // There could be a mismatch if the word list is not the same in the contract and
            // the game player's app. This could happen when the word list in the contract is
            // updated and the game player's app hasn't yet been updated to match.
            emit ScoreMismatch(_score, calculatedScore);

            if (dayCount > 0 && calculatedScore > bestScoreByDay[_gameDay]) {
                // Emit an event rather than revert because this is a possible 
                // happy code path.
                emit ScoreNotCompetitive(_score, bestScoreByDay[_gameDay]);
                return;
            }
        }

        // Record the submission.
        //   calculatedScore is guaranteed ≤ bestScoreByDay[_gameDay] (or first submission)
        //   because we verified calculatedScore == _score and _score ≤ bestScoreByDay in step 3.
        bestScoreByDay[_gameDay] = calculatedScore;
        _submissions[_gameDay][calculatedScore].push(
            Submission({player: msg.sender, board: _board})
        );
        submissionCount[_gameDay]++;
        emit BoardSubmitted(_gameDay, calculatedScore, msg.sender);
    }

    /**
     * @notice Submit a completed board and return the analysis of the board.
     * @param _board    121-character flat board string, row-major (uppercase A–Z / space).
     * @return the calculated score, the words derived from the board, and which are in the wordlist.
     */
    function analyseBoard(
        string calldata _board
    ) external virtual view returns (uint256, string[] memory, bool[] memory) {
        // Extract all connected words from the board.
        string[] memory words = analyse(_board);

        // Look up all words in the word list.
        bool[] memory inDictionary = _checkWords(words);

        // Calculate the actual score and verify it matches the submitted value.
        uint256 calculatedScore = score(words, inDictionary);

        return (calculatedScore, words, inDictionary);
    }
}
