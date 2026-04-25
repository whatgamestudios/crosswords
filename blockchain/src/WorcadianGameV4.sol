// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {UUPSUpgradeable} from "@openzeppelin/contracts-upgradeable/proxy/utils/UUPSUpgradeable.sol";
import {
    AccessControlEnumerableUpgradeable
} from "@openzeppelin/contracts-upgradeable/access/extensions/AccessControlEnumerableUpgradeable.sol";

import {GameDayCheckV2} from "./GameDayCheckV2.sol";
import {PassportCheck} from "./PassportCheck.sol";
import {AnalyseBoard} from "./AnalyseBoard.sol";
import {ScoreV4} from "./ScoreV4.sol";


/// @dev Minimal interface for the WordListSeed contract.
interface IWordListSeed {
    function getSeedWord(uint256 _gameDay) external view returns (string memory);
}

/// @dev Minimal interface for the WorcadianWordListV1 contract.
interface IWorcadianWordList {
    function inWordListBulk(bytes[] calldata _words) external view returns (bool[] memory);
}


/**
 * Import updated scoring algorithm.
 */
contract WorcadianGameV4 is
    AccessControlEnumerableUpgradeable,
    UUPSUpgradeable,
    GameDayCheckV2,
    PassportCheck,
    AnalyseBoard,
    ScoreV4 {

    // ── Errors ────────────────────────────────────────────────────────────────

    /// @notice Error: Attempting to downgrade contract storage version.
    error CanNotUpgradeToLowerOrSameVersion(uint256 _storageVersion);

    error BadAddress(
        address _admin,
        address _owner,
        address _upgrade,
        address _passportWallet,
        address _wordListSeed,
        address _wordList
    );

    /// @notice Caller is not an allowlisted Passport Wallet.
    error NotPassportWallet(address _caller);

    /// @notice Board string is not the expected BOARD_SIZE × BOARD_SIZE length.
    error InvalidBoardSize(uint256 _size);

    /// @notice The seed word is not present at its expected position on the board.
    error SeedWordNotFound(string _seedWord);

    // ── Events ────────────────────────────────────────────────────────────────

    /// @notice Submitted score is worse than today's current best.
    event ScoreNotCompetitive(uint256 _submitted, uint256 _bestScore);

    /// @notice A valid board was stored at the current best score (or tied).
    event BoardSubmitted(uint32 indexed _gameDay, uint256 _score, address indexed _player);

    /// @notice Submitted score does not match the score calculated from the board.
    event ScoreMismatch(uint256 _submitted, uint256 _calculated);

    // ── Roles ─────────────────────────────────────────────────────────────────

    /// @notice Only UPGRADE_ROLE can upgrade the contract.
    bytes32 public constant UPGRADE_ROLE = keccak256("UPGRADE_ROLE");

    /// @notice The first OWNER_ROLE member is returned as owner().
    bytes32 public constant OWNER_ROLE = keccak256("OWNER_ROLE");

    // ── Constants ─────────────────────────────────────────────────────────────

    uint256 internal constant _VERSION3 = 3;
    uint256 internal constant _VERSION4 = 4;

    // ── Storage ───────────────────────────────────────────────────────────────

    /// @notice Version number of the storage variable layout.
    uint256 public version;

    /// @notice Address of the deployed WordListSeed contract.
    address public wordListSeed;

    /// @notice Address of the deployed WorcadianWordListV1 contract.
    address public wordList;

    /// @notice Current best (lowest) score for each game day.
    ///         Only meaningful when submissionCount[gameDay] > 0.
    mapping(uint32 => uint256) public bestScoreByDay;

    /// @notice Total number of stored submissions for each game day (all score tiers).
    ///         Use this to distinguish "no submissions yet" from a genuine score of 0.
    mapping(uint32 => uint256) public submissionCount;

    /// @dev A single player submission.
    struct Submission {
        address player;
        string board;
    }

    /// @dev All submissions, indexed by game day and then by score.
    mapping(uint32 => mapping(uint256 => Submission[])) internal _submissions;

    // ── Constructor ───────────────────────────────────────────────────────────

    /**
     * @dev Disable direct initialisation of the implementation contract.
     */
    constructor() {
        _disableInitializers();
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
    ) external virtual {
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

    // ── Passport allowlist management ─────────────────────────────────────────

    /**
     * @notice Add a Passport Wallet proxy to the allowlist.
     * @param _wallet A deployed Passport Wallet proxy address.
     */
    function addPassportWallet(address _wallet) external onlyRole(OWNER_ROLE) {
        _addWalletToAllowlist(_wallet);
    }

    /**
     * @notice Remove a Passport Wallet proxy from the allowlist.
     * @param _wallet The Passport Wallet proxy address to remove.
     */
    function removePassportWallet(address _wallet) external onlyRole(OWNER_ROLE) {
        _removeWalletFromAllowlist(_wallet);
    }

    // ── Upgrade ───────────────────────────────────────────────────────────────

    /**
     * @notice Function to be called when upgrading this contract.
     * @dev Call this function as part of upgradeToAndCall().
     * @dev This function does not need access control. Only allow upgrade from previous 
     *      version to this version.
     * @ param _data ABI encoded data to be used as part of the contract storage upgrade.
     */
    function upgradeStorage(bytes memory /* _data */) external virtual {
        if (version == _VERSION3) {
            version = _VERSION4;
        }
        else {
            revert CanNotUpgradeToLowerOrSameVersion(version);
        }
    }


    function _authorizeUpgrade(address newImplementation) internal override onlyRole(UPGRADE_ROLE) {
        // Nothing here
    }


    // ── View functions ────────────────────────────────────────────────────────

    /**
     * @notice Returns a paginated slice of submissions stored at a given score tier.
     * @param _gameDay     The game day to query.
     * @param _score       The score tier to query.
     * @param _startIndex  Index of the first submission to return.
     * @param _count       Maximum number of submissions to return.
     */
    function getSubmissions(
        uint32 _gameDay,
        uint256 _score,
        uint256 _startIndex,
        uint256 _count
    ) external view returns (Submission[] memory result) {
        Submission[] storage all = _submissions[_gameDay][_score];
        uint256 total = all.length;
        if (_startIndex >= total) return new Submission[](0);
        uint256 available = total - _startIndex;
        if (_count > available) _count = available;
        result = new Submission[](_count);
        for (uint256 i = 0; i < _count; i++) {
            result[i] = all[_startIndex + i];
        }
    }

    /**
     * @notice Returns the number of submissions stored at a specific score tier for a game day.
     */
    function getSubmissionCountAtScore(uint32 _gameDay, uint256 _score)
        external view returns (uint256)
    {
        return _submissions[_gameDay][_score].length;
    }

    /**
     * @notice Returns the submissions at today's best score for a game day.
     * @dev Returns an empty array if no submissions exist for the day.
     */
    function getBestSubmissions(uint32 _gameDay)
        external view returns (Submission[] memory)
    {
        if (submissionCount[_gameDay] == 0) {
            return new Submission[](0);
        }
        return _submissions[_gameDay][bestScoreByDay[_gameDay]];
    }

    /**
     * @dev Returns the address of the current owner.
     */
    function owner() public view virtual returns (address) {
        if (getRoleMemberCount(OWNER_ROLE) == 0) return address(0);
        return getRoleMember(OWNER_ROLE, 0);
    }

    /**
     * @notice Returns the submissions at today's best score for a game day.
     * @dev Returns an empty array if no submissions exist for the day.
     */
    function getResults(uint32 _gameDay)
        external view returns (uint256 numSubmissions, uint256 bestScore, Submission[] memory submissions)
    {
        uint256 count = submissionCount[_gameDay];
        uint256 best = bestScoreByDay[_gameDay];
        if (count == 0) {
            return (0, 0, new Submission[](0));
        }
        return (count, best, _submissions[_gameDay][best]);
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


    // ── Private helpers ───────────────────────────────────────────────────────

    /**
     * @dev Check that the seed word occupies the correct horizontal position on the board.
     *
     *      The seed word is always placed on the centre row and horizontally centred:
     *        row    = BOARD_SIZE / 2          (= 5 for 11×11)
     *        startX = (BOARD_SIZE - len) / 2
     *
     *      Mirrors the placement logic in the Unity client's GamePlayScene.cs.
     *
     * @param _boardBytes  Board as a flat byte array.
     * @param _seedBytes   Expected seed word as bytes (uppercase).
     * @return True if every character of the seed word matches the board at the expected position.
     */
    function _isSeedWordOnBoard(
        bytes memory _boardBytes,
        bytes memory _seedBytes
    ) internal pure returns (bool) {
        uint256 wordLen = _seedBytes.length;
        if (wordLen == 0 || wordLen > BOARD_SIZE) {
            return false;
        }
        uint256 startX = (BOARD_SIZE - wordLen) / 2;
        uint256 row    = BOARD_SIZE / 2;
        for (uint256 i = 0; i < wordLen; i++) {
            if (_boardBytes[row * BOARD_SIZE + startX + i] != _seedBytes[i]) {
                return false;
            }
        }
        return true;
    }

    /**
     * @dev Convert the string word array to bytes[] and query WorcadianWordListV1
     *      in a single batched external call.
     */
    function _checkWords(string[] memory _words) internal view returns (bool[] memory) {
        uint256 len = _words.length;
        bytes[] memory wordBytes = new bytes[](len);
        for (uint256 i = 0; i < len; i++) {
            wordBytes[i] = bytes(_words[i]);
        }
        return IWorcadianWordList(wordList).inWordListBulk(wordBytes);
    }

    /// @notice Storage gap for additional variables in future upgrades.
    /// forge-lint: disable-next-line(mixed-case-variable)
    uint256[50] private __WorcadianGameGap;
}
