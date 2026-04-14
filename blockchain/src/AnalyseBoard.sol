// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {GameConstants} from "./GameConstants.sol";

/**
 * @notice Board analysis for the Worcadian crossword game.
 * Mirrors AnalyseBoard.cs from the Unity client.
 *
 * The board is an 11×11 grid encoded as a flat 121-character string, row-major:
 *   cell (x, y) is at index (y × BOARD_SIZE + x).
 * Occupied cells hold an uppercase letter (A–Z); empty cells hold a space (0x20).
 *
 * Analysis starts at the centre cell (5, 5), finds the horizontal word there,
 * then iteratively discovers all connected vertical and horizontal words.
 * The iterative BFS is semantically equivalent to the mutual recursion in the
 * C# original (findVerticalWords ↔ findHorizontalWords) and finds the same set
 * of words.
 */
abstract contract AnalyseBoard is GameConstants {

    /// @dev Hard upper bound on words discoverable on an 11×11 board.
    uint256 private constant MAX_WORDS = 64;

    /// @dev Compact on-stack representation of a word's bounding box.
    struct WordPos {
        uint8 startX;
        uint8 startY;
        uint8 endX;
        uint8 endY;
    }

    /**
     * @dev Per-call analysis state threaded through the helpers as a memory
     *      struct (passed by reference) so that analysed flags written inside
     *      _findHorizontalWord / _findVerticalWord are visible to the caller.
     *
     *      analysedH[x][y] – cell (x,y) has already been counted in a horizontal word.
     *      analysedV[x][y] – cell (x,y) has already been counted in a vertical word.
     *      Mirrors the bool[BOARD_SIZE, BOARD_SIZE] fields in AnalyseBoard.cs.
     */
    struct AnalysisState {
        // [x][y] indexing: outer dimension = x (0-10), inner = y (0-10)
        bool[11][11] analysedH;
        bool[11][11] analysedV;
    }

    // ── Public entry point ────────────────────────────────────────────────────

    /**
     * @notice Extract all connected words from a board string.
     * @dev Mirrors AnalyseBoard.Analyse(Board board) / analyse().
     *      Returns an empty array when the centre cell is empty.
     * @param _board 121-character flat board string (row-major, uppercase A–Z / space).
     * @return words All words found, in discovery order (first horizontal at centre,
     *               then branching depth-first as in the C# original).
     */
    function analyse(string memory _board) internal pure returns (string[] memory words) {
        bytes memory b = bytes(_board);

        uint256 center = BOARD_SIZE / 2; // 5 for 11×11

        // Mirror: "if (!_board.IsCellOccupied(startX, startY)) return new List<WordOnBoard>();"
        if (!_isOccupied(b, center, center)) {
            return new string[](0);
        }

        AnalysisState memory state;

        // Pending queues (indexed from the front, heads advance, never shrink).
        WordPos[MAX_WORDS] memory pendingH;
        uint256 pendingHCount;
        WordPos[MAX_WORDS] memory pendingV;
        uint256 pendingVCount;

        string[MAX_WORDS] memory foundWords;
        uint256 wordCount;

        // Mirror: "WordOnBoard first = findHorizontalWord(startX, startY);"
        WordPos memory first = _findHorizontalWord(b, center, center, state);
        foundWords[wordCount++] = _extractWord(b, first);
        pendingH[pendingHCount++] = first;

        uint256 hIdx;
        uint256 vIdx;

        while (hIdx < pendingHCount || vIdx < pendingVCount) {

            // ── findVerticalWords(hw) ─────────────────────────────────────────
            // For each horizontal word in the pending queue, check every column
            // of that word for an unanalysed vertical branch above or below.
            while (hIdx < pendingHCount) {
                WordPos memory hw = pendingH[hIdx++];
                uint256 hLen = uint256(hw.endX) - uint256(hw.startX) + 1;
                for (uint256 i = 0; i < hLen; i++) {
                    uint256 x = uint256(hw.startX) + i;
                    uint256 y = uint256(hw.startY);
                    // Mirror: "if (seedWord.StartY != 0) if (occupied(xofs, y-1) && !analysedV[xofs,y-1]) exists=true;"
                    bool exists = false;
                    if (y > 0 && _isOccupied(b, x, y - 1) && !state.analysedV[x][y - 1]) {
                        exists = true;
                    }
                    // Mirror: "if (seedWord.StartY != BOARD_SIZE-1) if (occupied(xofs,y+1) && !analysedV[xofs,y+1]) exists=true;"
                    if (!exists && y < BOARD_SIZE - 1 && _isOccupied(b, x, y + 1) && !state.analysedV[x][y + 1]) {
                        exists = true;
                    }
                    if (exists && wordCount < MAX_WORDS) {
                        WordPos memory vw = _findVerticalWord(b, x, y, state);
                        foundWords[wordCount++] = _extractWord(b, vw);
                        if (pendingVCount < MAX_WORDS) pendingV[pendingVCount++] = vw;
                    }
                }
            }

            // ── findHorizontalWords(vw) ───────────────────────────────────────
            // For each vertical word in the pending queue, check every row of
            // that word for an unanalysed horizontal branch to the left or right.
            while (vIdx < pendingVCount) {
                WordPos memory vw = pendingV[vIdx++];
                uint256 vLen = uint256(vw.endY) - uint256(vw.startY) + 1;
                for (uint256 i = 0; i < vLen; i++) {
                    uint256 x = uint256(vw.startX);
                    uint256 y = uint256(vw.startY) + i;
                    // Mirror: "if (seedWord.StartX != 0) if (occupied(x-1,yofs) && !analysedH[x-1,yofs]) exists=true;"
                    bool exists = false;
                    if (x > 0 && _isOccupied(b, x - 1, y) && !state.analysedH[x - 1][y]) {
                        exists = true;
                    }
                    // Mirror: "if (seedWord.StartX != BOARD_SIZE-1) if (occupied(x+1,yofs) && !analysedH[x+1,yofs]) exists=true;"
                    if (!exists && x < BOARD_SIZE - 1 && _isOccupied(b, x + 1, y) && !state.analysedH[x + 1][y]) {
                        exists = true;
                    }
                    if (exists && wordCount < MAX_WORDS) {
                        WordPos memory hw = _findHorizontalWord(b, x, y, state);
                        foundWords[wordCount++] = _extractWord(b, hw);
                        if (pendingHCount < MAX_WORDS) pendingH[pendingHCount++] = hw;
                    }
                }
            }
        }

        words = new string[](wordCount);
        for (uint256 i = 0; i < wordCount; i++) {
            words[i] = foundWords[i];
        }
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    /// @dev Returns true when cell (x,y) holds an uppercase letter (A–Z).
    function _isOccupied(bytes memory _b, uint256 _x, uint256 _y) private pure returns (bool) {
        uint8 ch = uint8(_b[_y * BOARD_SIZE + _x]);
        return ch >= 0x41 && ch <= 0x5A;
    }

    /**
     * @dev Find the full horizontal word that passes through cell (x, y).
     *      Walks left to the start, right to the end, marks each cell in
     *      _state.analysedH, and returns the bounding WordPos.
     *      Mirrors AnalyseBoard.findHorizontalWord().
     */
    function _findHorizontalWord(
        bytes memory _b,
        uint256 _x,
        uint256 _y,
        AnalysisState memory _state
    ) private pure returns (WordPos memory wp) {
        while (_x > 0 && _isOccupied(_b, _x - 1, _y)) _x--;
        uint256 startX = _x;
        while (_x < BOARD_SIZE - 1 && _isOccupied(_b, _x + 1, _y)) _x++;
        uint256 endX = _x;
        for (uint256 i = startX; i <= endX; i++) {
            _state.analysedH[i][_y] = true;
        }
        // safe: startX, endX, _y are all bounded by BOARD_SIZE (11)
        // forge-lint: disable-next-line(unsafe-typecast)
        wp = WordPos({ startX: uint8(startX), startY: uint8(_y), endX: uint8(endX), endY: uint8(_y) });
    }

    /**
     * @dev Find the full vertical word that passes through cell (x, y).
     *      Walks up to the start, down to the end, marks each cell in
     *      _state.analysedV, and returns the bounding WordPos.
     *      Mirrors AnalyseBoard.findVerticalWord().
     */
    function _findVerticalWord(
        bytes memory _b,
        uint256 _x,
        uint256 _y,
        AnalysisState memory _state
    ) private pure returns (WordPos memory wp) {
        while (_y > 0 && _isOccupied(_b, _x, _y - 1)) _y--;
        uint256 startY = _y;
        while (_y < BOARD_SIZE - 1 && _isOccupied(_b, _x, _y + 1)) _y++;
        uint256 endY = _y;
        for (uint256 i = startY; i <= endY; i++) {
            _state.analysedV[_x][i] = true;
        }
        // safe: _x, startY, endY are all bounded by BOARD_SIZE (11)
        // forge-lint: disable-next-line(unsafe-typecast)
        wp = WordPos({ startX: uint8(_x), startY: uint8(startY), endX: uint8(_x), endY: uint8(endY) });
    }

    /**
     * @dev Read the characters of a word from the board bytes using its WordPos.
     *      Horizontal words read left-to-right; vertical words read top-to-bottom.
     */
    function _extractWord(bytes memory _b, WordPos memory _pos) private pure returns (string memory) {
        bool isH = _pos.startY == _pos.endY;
        uint256 len = isH
            ? uint256(_pos.endX) - uint256(_pos.startX) + 1
            : uint256(_pos.endY) - uint256(_pos.startY) + 1;
        bytes memory result = new bytes(len);
        for (uint256 i = 0; i < len; i++) {
            uint256 x = isH ? uint256(_pos.startX) + i : uint256(_pos.startX);
            uint256 y = isH ? uint256(_pos.startY) : uint256(_pos.startY) + i;
            result[i] = _b[y * BOARD_SIZE + x];
        }
        return string(result);
    }
}
