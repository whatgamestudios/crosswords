// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

//import {Initializable} from "@openzeppelin/contracts-upgradeable/proxy/utils/Initializable.sol";
import {UUPSUpgradeable} from "@openzeppelin/contracts-upgradeable/proxy/utils/UUPSUpgradeable.sol";
import {
    AccessControlEnumerableUpgradeable
} from "@openzeppelin/contracts-upgradeable/access/extensions/AccessControlEnumerableUpgradeable.sol";

import {GameDayCheck} from "./GameDayCheck.sol";

/**
 * Manage players submitting solutions for the 14Numbers game.
 *
 * This contract is designed to be upgradeable.
 */
contract WorcadianCheckInV1 is AccessControlEnumerableUpgradeable, GameDayCheck, UUPSUpgradeable {
    /// @notice Error: Attempting to upgrade contract storage to version 0.
    error CanNotUpgradeToLowerOrSameVersion(uint256 _storageVersion);

    error BadAddress(address _admin, address _owner, address _upgrade);

    event CheckIn(uint32 _gameDay, address _player, uint32 _numDaysPlayed);

    /// @notice Only UPGRADE_ROLE can upgrade the contract
    bytes32 public constant UPGRADE_ROLE = keccak256("UPGRADE_ROLE");

    /// @notice The first Owner role is returned as the owner of the contract.
    bytes32 public constant OWNER_ROLE = keccak256("OWNER_ROLE");

    /// @notice Version 1 version number
    uint256 internal constant _VERSION1 = 1;
    /// @notice Version 2 version number
    //uint256 private constant _VERSION2 = 2;
    /// @notice Version 3 version number
    //uint256 private constant _VERSION3 = 3;
    /// @notice Version 4 version number
    //uint256 private constant _VERSION4 = 4;

    // Days in five years is approximately 365 x 4 + 366.
    uint32 private constant FIVE_YEARS_OF_DAYS = 1826;

    /// @notice version number of the storage variable layout.
    uint256 public version;

    // Holds a player's stats.
    struct Stats {
        uint32 mostRecentGameDay;
        uint32 daysPlayed;
    }

    // Map: player address => player's stats.
    mapping(address => Stats) private allPlayerStats;

    mapping(uint32 gameDay => uint256 numPlayers) public numberOfPlayers;
    mapping(uint32 gameDay => uint256 numSessions) public numberOfSessions;

    // Every address that has ever played the game, in order of first check-in.
    address[] private allPlayers;

    /**
     * @dev Don't allow the implementation contract to be initialised.
     */
    constructor() {
        _disableInitializers();
    }

    /**
     * @notice Initialises the upgradeable contract, setting up admin accounts.
     * @param _roleAdmin the address to grant `DEFAULT_ADMIN_ROLE` to.
     * @param _owner the address to grant `OWNER_ROLE` to.
     * @param _upgradeAdmin the address to grant `UPGRADE_ROLE` to.
     */
    function initialize(address _roleAdmin, address _owner, address _upgradeAdmin) public virtual initializer {
        require(
            _roleAdmin != address(0) && _owner != address(0) && _upgradeAdmin != address(0),
            BadAddress(_roleAdmin, _owner, _upgradeAdmin)
        );

        __UUPSUpgradeable_init();
        __AccessControlEnumerable_init();
        _grantRole(DEFAULT_ADMIN_ROLE, _roleAdmin);
        _grantRole(OWNER_ROLE, _owner);
        _grantRole(UPGRADE_ROLE, _upgradeAdmin);
        version = _VERSION1;
    }

    /**
     * @notice Function to be called when upgrading this contract.
     * @dev Call this function as part of upgradeToAndCall().
     * @dev This function does not need access control. Future versions should be written to
     *      only update from the previous version to the current version.
     * @ param _data ABI encoded data to be used as part of the contract storage upgrade.
     */
    function upgradeStorage(
        bytes memory /* _data */
    )
        external
        virtual
    {
        revert CanNotUpgradeToLowerOrSameVersion(version);
    }

    /**
     * @notice Check in when player opens the game play screen.
     * @param _gameDay The day since the game epoch start.
     */
    function checkIn(uint32 _gameDay) external virtual {
        // Reverts if game day is in the future or in the past.
        checkGameDay(_gameDay);

        numberOfSessions[_gameDay]++;

        Stats storage playerStats = allPlayerStats[msg.sender];
        // Note: checking for daysPlayed = 0 is for the edge case of the game
        // day being zero.
        if (playerStats.daysPlayed == 0 || _gameDay > playerStats.mostRecentGameDay) {
            numberOfPlayers[_gameDay]++;

            if (playerStats.daysPlayed == 0) {
                allPlayers.push(msg.sender);
            }

            playerStats.mostRecentGameDay = _gameDay;
            uint32 daysPlayed = playerStats.daysPlayed + 1;
            playerStats.daysPlayed = daysPlayed;
            emit CheckIn(_gameDay, msg.sender, daysPlayed);
        }
    }

    /**
     * @dev Returns the address of the current owner, for use by systems that need an "owner".
     */
    function owner() public view virtual returns (address) {
        if (getRoleMemberCount(OWNER_ROLE) == 0) {
            return address(0);
        }
        return getRoleMember(OWNER_ROLE, 0);
    }

    function getDaysPlayed(address _player) external view returns (uint256 _days) {
        _days = allPlayerStats[_player].daysPlayed;
    }

    function getNumPlayers(uint32 _startGameDay, uint32 _numDays) external view returns (uint256[] memory _numPlayers) {
        _numDays = _numDays > FIVE_YEARS_OF_DAYS ? FIVE_YEARS_OF_DAYS : _numDays;
        _numPlayers = new uint256[](_numDays);
        for (uint32 i = 0; i < _numDays; i++) {
            _numPlayers[i] = numberOfPlayers[_startGameDay + i];
        }
    }

    function getNumSessions(uint32 _startGameDay, uint32 _numDays)
        external
        view
        returns (uint256[] memory _numSessions)
    {
        _numDays = _numDays > FIVE_YEARS_OF_DAYS ? FIVE_YEARS_OF_DAYS : _numDays;
        _numSessions = new uint256[](_numDays);
        for (uint32 i = 0; i < _numDays; i++) {
            _numSessions[i] = numberOfSessions[_startGameDay + i];
        }
    }

    /**
     * @notice Returns the total number of unique players who have ever played.
     */
    function getTotalPlayers() external view returns (uint256) {
        return allPlayers.length;
    }

    /**
     * @notice Returns a slice of the all-time player list.
     * @param _startIndex Index of the first player to return.
     * @param _count Maximum number of players to return.
     */
    function getPlayers(uint256 _startIndex, uint256 _count) external view returns (address[] memory _players) {
        uint256 total = allPlayers.length;
        if (_startIndex >= total) {
            return new address[](0);
        }
        uint256 available = total - _startIndex;
        if (_count > available) {
            _count = available;
        }
        _players = new address[](_count);
        for (uint256 i = 0; i < _count; i++) {
            _players[i] = allPlayers[_startIndex + i];
        }
    }

    // Override the _authorizeUpgrade function
    function _authorizeUpgrade(address newImplementation) internal override onlyRole(UPGRADE_ROLE) {}

    /// @notice storage gap for additional variables for upgrades
    /// forge-lint: disable-next-line(mixed-case-variable)
    uint256[49] private __WorcadianCheckInGap;
}
