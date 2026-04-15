// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {WorcadianGameV1} from "./WorcadianGameV1.sol";


/**
 * Add a better current day situation getter.
 */
contract WorcadianGameV2 is WorcadianGameV1 {

    uint256 internal constant _VERSION2 = 2;

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
        if (version == _VERSION1) {
            version = _VERSION2;
        }
        else {
            revert CanNotUpgradeToLowerOrSameVersion(version);
        }
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
}
