// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {UUPSUpgradeable} from "@openzeppelin/contracts-upgradeable/proxy/utils/UUPSUpgradeable.sol";
import {
    AccessControlEnumerableUpgradeable
} from "@openzeppelin/contracts-upgradeable/access/extensions/AccessControlEnumerableUpgradeable.sol";


/**
 * Word list used by Worcadian.
 *
 * This contract is designed to be upgradeable.
 */
contract WorcadianWordListV1 is AccessControlEnumerableUpgradeable, UUPSUpgradeable {
    /// @notice Error: Attempting to upgrade contract storage.
    error CanNotUpgradeToLowerOrSameVersion(uint256 _storageVersion);

    error BadAddress(address _admin, address _owner, address _upgrade, address _wordSmithAdmin);

    error WordNotInList(string _word);

    /// @notice One or more words has been added.
    event WordsAdded(uint256 _number);

    /// @notice A word has been removed.
    event WordRemoved(string _word);

    /// @notice Only UPGRADE_ROLE can upgrade the contract
    bytes32 public constant UPGRADE_ROLE = keccak256("UPGRADE_ROLE");

    /// @notice The first Owner role is returned as the owner of the contract.
    bytes32 public constant OWNER_ROLE = keccak256("OWNER_ROLE");

    /// @notice Only WORDSMITH_ROLE can add and remove words from the word list.
    bytes32 public constant WORDSMITH_ROLE = keccak256("WORDSMITH_ROLE");

    /// @notice Version 1 version number
    uint256 internal constant _VERSION1 = 1;
    /// @notice Version 2 version number
    //uint256 private constant _VERSION2 = 2;
    /// @notice Version 3 version number
    //uint256 private constant _VERSION3 = 3;
    /// @notice Version 4 version number
    //uint256 private constant _VERSION4 = 4;

    /// @notice version number of the storage variable layout.
    uint256 public version;

    mapping(bytes32 => bool) internal wordList;


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
     * @param _wordSmithAdmin the address to grant `WORDSMITH_ROLE` to.
     */
    function initialize(address _roleAdmin, address _owner, address _upgradeAdmin, address _wordSmithAdmin) public virtual initializer {
        require(
            _roleAdmin != address(0) && _owner != address(0) && _upgradeAdmin != address(0) && _wordSmithAdmin != (address(0)),
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
     * @dev This function does not need access control. Only allow upgrade from previous 
     *      version to this version.
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
     * @notice Add words to the word list.
     * @param _words Words to add to the word list.
     */
    function addWords(bytes[] calldata _words) external virtual onlyRole(WORDSMITH_ROLE) {
        for (uint256 i = 0; i < _words.length; i++) {
            wordList[keccak256(_words[i])] = true;
        }
        emit WordsAdded(_words.length);
    }

    /**
     * @notice Remove a word from the word list.
     * @param _word Word to remove from the word list.
     */
    function removeWord(bytes calldata _word) external virtual onlyRole(WORDSMITH_ROLE) {
        bytes32 wordHash = keccak256(_word);
        require(wordList[wordHash], WordNotInList(string(_word)));
        wordList[wordHash] = false;
        emit WordRemoved(string(_word));
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
     * @notice Check if a word is in the word list.
     * @param _word The word to check.
     * @return true if the word is in the list.
     */
    function inWordList(bytes calldata _word) external virtual view returns (bool) {
        return wordList[keccak256(_word)];
    }

    /**
     * @notice Check if multiple words are in the word list.
     * @param _words The words to check.
     * @return bool[]: true if each word is in the list.
     */
    function inWordListBulk(bytes[] calldata _words) external virtual view returns (bool[] memory) {
        bool[] memory result = new bool[](_words.length);
        for (uint256 i = 0; i < _words.length; i++) {
            result[i] = wordList[keccak256(_words[i])];
        }
        return result;
    }


    // Override the _authorizeUpgrade function
    function _authorizeUpgrade(address newImplementation) internal override onlyRole(UPGRADE_ROLE) {}

    /// @notice storage gap for additional variables for upgrades
    /// forge-lint: disable-next-line(mixed-case-variable)
    uint256[50] private __WorcadianWordListGap;
}
