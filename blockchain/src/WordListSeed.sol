// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {UUPSUpgradeable} from "@openzeppelin/contracts-upgradeable/proxy/utils/UUPSUpgradeable.sol";
import {
    AccessControlEnumerableUpgradeable
} from "@openzeppelin/contracts-upgradeable/access/extensions/AccessControlEnumerableUpgradeable.sol";


/**
 * Seed word list used by the Worcadian to determine the daily puzzle word.
 * Mirrors WordListSeed.cs from the Unity client.
 *
 * getSeedWord(gameDay) returns seedWords[gameDay % seedWords.length] in uppercase,
 * matching the C# behaviour of GetSeedWord(uint gameDay).
 *
 * The initial word list (added via addSeedWords after deployment) must match
 * the seedWords array in WordListSeed.cs exactly and in order:
 *   "gipsy", "piles", "posed", "flows", "scrap", "pleas", "knead", "tired",
 *   "sped", "scared", "party", "desk", "take", "strike", ...
 *
 * This contract is designed to be upgradeable.
 */
contract WordListSeed is AccessControlEnumerableUpgradeable, UUPSUpgradeable {
    /// @notice Error: Attempting to downgrade contract storage version.
    error CanNotUpgradeToLowerOrSameVersion(uint256 _storageVersion);

    error BadAddress(address _admin, address _owner, address _upgrade, address _wordSmithAdmin);

    /// @notice Error: getSeedWord called before any words have been added.
    error EmptyWordList();

    /// @notice One or more seed words have been added.
    event SeedWordsAdded(uint256 _number);

    /// @notice Only UPGRADE_ROLE can upgrade the contract.
    bytes32 public constant UPGRADE_ROLE = keccak256("UPGRADE_ROLE");

    /// @notice The first Owner role is returned as the owner of the contract.
    bytes32 public constant OWNER_ROLE = keccak256("OWNER_ROLE");

    /// @notice Only WORDSMITH_ROLE can add seed words.
    bytes32 public constant WORDSMITH_ROLE = keccak256("WORDSMITH_ROLE");

    /// @notice Version 1 version number.
    uint256 internal constant _VERSION1 = 1;

    /// @notice Version number of the storage variable layout.
    uint256 public version;

    /// @notice Ordered list of seed words. Index is used to select the daily word.
    string[] private _seedWords;


    /**
     * @dev Don't allow the implementation contract to be initialised.
     */
    constructor() {
        _disableInitializers();
    }

    /**
     * @notice Initialises the upgradeable contract, setting up admin accounts.
     * @param _roleAdmin   Address to grant DEFAULT_ADMIN_ROLE.
     * @param _owner       Address to grant OWNER_ROLE.
     * @param _upgradeAdmin Address to grant UPGRADE_ROLE.
     * @param _wordSmithAdmin Address to grant WORDSMITH_ROLE.
     */
    function initialize(
        address _roleAdmin,
        address _owner,
        address _upgradeAdmin,
        address _wordSmithAdmin
    ) public virtual initializer {
        require(
            _roleAdmin != address(0) && _owner != address(0) &&
            _upgradeAdmin != address(0) && _wordSmithAdmin != address(0),
            BadAddress(_roleAdmin, _owner, _upgradeAdmin, _wordSmithAdmin)
        );

        __UUPSUpgradeable_init();
        __AccessControlEnumerable_init();
        _grantRole(DEFAULT_ADMIN_ROLE, _roleAdmin);
        _grantRole(OWNER_ROLE, _owner);
        _grantRole(UPGRADE_ROLE, _upgradeAdmin);
        _grantRole(WORDSMITH_ROLE, _wordSmithAdmin);
        version = _VERSION1;
    }

    /**
     * @notice Function to be called when upgrading this contract.
     * @dev Call this function as part of upgradeToAndCall().
     */
    function upgradeStorage(
        bytes memory /* _data */
    ) external virtual {
        revert CanNotUpgradeToLowerOrSameVersion(version);
    }

    /**
     * @notice Add seed words to the ordered list.
     * @dev Call in batches to stay within block gas limits.
     *      Words must be appended in the same order as seedWords in WordListSeed.cs.
     * @param _words Words to append to the seed word list.
     */
    function addSeedWords(string[] calldata _words) external virtual onlyRole(WORDSMITH_ROLE) {
        for (uint256 i = 0; i < _words.length; i++) {
            _seedWords.push(_words[i]);
        }
        emit SeedWordsAdded(_words.length);
    }

    /**
     * @notice Returns the seed word for a given game day.
     * @dev Mirrors WordListSeed.cs GetSeedWord(uint gameDay):
     *      returns seedWords[gameDay % seedWords.Length].ToUpper()
     * @param _gameDay The game day number.
     * @return The seed word in uppercase.
     */
    function getSeedWord(uint256 _gameDay) external view returns (string memory) {
        uint256 len = _seedWords.length;
        require(len > 0, EmptyWordList());
        return _toUpper(_seedWords[_gameDay % len]);
    }

    /**
     * @notice Returns the number of seed words currently in the list.
     */
    function seedWordCount() external view returns (uint256) {
        return _seedWords.length;
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

    /**
     * @dev Converts an ASCII string to uppercase.
     */
    function _toUpper(string memory _str) internal pure returns (string memory) {
        bytes memory src = bytes(_str);
        bytes memory result = new bytes(src.length);
        for (uint256 i = 0; i < src.length; i++) {
            bytes1 b = src[i];
            if (b >= 0x61 && b <= 0x7A) {
                result[i] = bytes1(uint8(b) - 32);
            } else {
                result[i] = b;
            }
        }
        return string(result);
    }

    // Override the _authorizeUpgrade function
    function _authorizeUpgrade(address newImplementation) internal override onlyRole(UPGRADE_ROLE) {}

    /// @notice Storage gap for additional variables in future upgrades.
    /// forge-lint: disable-next-line(mixed-case-variable)
    uint256[49] private __WordListSeedGap;
}
