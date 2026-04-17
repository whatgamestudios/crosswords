// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {WorcadianWordListV1} from "./WorcadianWordListV1.sol";


/**
 * Word list used by Worcadian.
 *
 * This contract is designed to be upgradeable.
 */
contract WorcadianWordListV2 is WorcadianWordListV1 {
    /// @notice Version 2 version number
    uint256 internal constant _VERSION2 = 2;


    /**
     * @notice Function to be called when upgrading this contract.
     * @dev Call this function as part of upgradeToAndCall().
     * @dev This function does not need access control. Only allow upgrade from previous 
     *      version to this version.
     * @ param _data ABI encoded data to be used as part of the contract storage upgrade.
     */
    function upgradeStorage(bytes memory /* _data */) external virtual override {
        if (version == _VERSION1) {
            version = _VERSION2;
        }
        else {
            revert CanNotUpgradeToLowerOrSameVersion(version);
        }
    }

    /**
     * @notice Check if a word is in the word list.
     * @param _word The word to check.
     * @return true if the word is in the list.
     */
    function inWordList(bytes calldata _word) external view override returns (bool) {
        bytes memory lower = _toLower(_word);
        return wordList[keccak256(lower)];
    }

    /**
     * @notice Check if multiple words are in the word list.
     * @param _words The words to check.
     * @return bool[]: true if each word is in the list.
     */
    function inWordListBulk(bytes[] calldata _words) external view override returns (bool[] memory) {
        bool[] memory result = new bool[](_words.length);
        for (uint256 i = 0; i < _words.length; i++) {
            bytes memory lower = _toLower(_words[i]);
            result[i] = wordList[keccak256(lower)];
        }
        return result;
    }


    /**
     * @dev Converts an ASCII string to lowercase.
     */
    function _toLower(bytes memory _str) internal pure returns (bytes memory) {
        bytes memory result = new bytes(_str.length);
        for (uint256 i = 0; i < _str.length; i++) {
            bytes1 b = _str[i];
            if (b >= 0x41 && b <= 0x5A) {
                result[i] = bytes1(uint8(b) + 32);
            } else {
                result[i] = b;
            }
        }
        return result;
    }
}
