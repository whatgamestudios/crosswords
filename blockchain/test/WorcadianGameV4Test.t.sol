// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {Test} from "forge-std/Test.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";
import {WorcadianGameV1} from "../src/WorcadianGameV1.sol";
import {WorcadianGameV2} from "../src/WorcadianGameV2.sol";
import {WorcadianGameV3} from "../src/WorcadianGameV3.sol";
import {WorcadianGameV4} from "../src/WorcadianGameV4.sol";

contract MockPassportImpl {}

contract MockPassportWallet {
    address private _impl;

    constructor(address impl_) {
        _impl = impl_;
    }

    /// forge-lint: disable-next-line(mixed-case-function)
    function PROXY_getImplementation() external view returns (address) {
        return _impl;
    }
}

contract MockWordListSeed {
    mapping(uint256 => string) private _words;

    function setSeedWord(uint256 gameDay, string calldata word) external {
        _words[gameDay] = word;
    }

    function getSeedWord(uint256 gameDay) external view returns (string memory) {
        return _words[gameDay];
    }
}

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

contract WorcadianGameV4Test is Test {
    WorcadianGameV4 game;

    MockPassportImpl passportImpl;
    MockPassportWallet passportWallet;
    MockWordListSeed mockSeed;
    MockWordList mockWordList;

    address admin = makeAddr("admin");
    address ownerAddr = makeAddr("owner");
    address upgradeAdmin = makeAddr("upgradeAdmin");

    uint256 constant GAME_START = 1774828800;
    uint256 constant DAY = 86400;
    uint32 constant GAME_DAY = 15;
    uint256 constant SCORE_IN_DICT = 22;

    function noonOn(uint32 gameDay) internal pure returns (uint256) {
        return GAME_START + uint256(gameDay) * DAY + DAY / 2;
    }

    function _onlyBoard() internal pure returns (string memory) {
        return string.concat(
            "           ",
            "           ",
            "           ",
            "           ",
            "           ",
            "   ONLY    ",
            "           ",
            "           ",
            "           ",
            "           ",
            "           "
        );
    }

    function _setAllInDict(bool value) internal {
        bool[] memory r = new bool[](1);
        r[0] = value;
        mockWordList.setResults(r);
    }

    function _submitOnly(uint256 score_) internal {
        vm.prank(address(passportWallet));
        game.submitBoard(GAME_DAY, score_, _onlyBoard());
    }

    function _deployProxy(
        address _admin, address _owner, address _upgradeAdmin,
        address _passportWallet, address _wordListSeed, address _wordList
    ) internal returns (WorcadianGameV4) {
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

        WorcadianGameV4 implV4 = new WorcadianGameV4();
        vm.prank(upgradeAdmin);
        WorcadianGameV1(address(proxy)).upgradeToAndCall(
            address(implV4), abi.encodeCall(WorcadianGameV2.upgradeStorage, (bytes("")))
        );

        return WorcadianGameV4(address(proxy));
    }

    function setUp() public {
        passportImpl = new MockPassportImpl();
        passportWallet = new MockPassportWallet(address(passportImpl));
        mockSeed = new MockWordListSeed();
        mockWordList = new MockWordList();

        game = _deployProxy(
            admin, ownerAddr, upgradeAdmin,
            address(passportWallet), address(mockSeed), address(mockWordList)
        );

        mockSeed.setSeedWord(GAME_DAY, "ONLY");
        _setAllInDict(true);
        vm.warp(noonOn(GAME_DAY));
    }

    function test_GetResults_NoSubmissions_ReturnsZerosAndEmptyArray() public view {
        (uint256 numSubmissions, uint256 bestScore, WorcadianGameV4.Submission[] memory submissions) =
            game.getResults(GAME_DAY);
        assertEq(numSubmissions, 0);
        assertEq(bestScore, 0);
        assertEq(submissions.length, 0);
    }

    function test_GetResults_AfterOneSubmission_MatchesStoredState() public {
        _submitOnly(SCORE_IN_DICT);

        (uint256 numSubmissions, uint256 bestScore, WorcadianGameV4.Submission[] memory submissions) =
            game.getResults(GAME_DAY);

        assertEq(numSubmissions, 1);
        assertEq(bestScore, SCORE_IN_DICT);
        assertEq(submissions.length, 1);
        assertEq(submissions[0].player, address(passportWallet));
        assertEq(submissions[0].board, _onlyBoard());

        WorcadianGameV4.Submission[] memory bestViaV1 = game.getBestSubmissions(GAME_DAY);
        assertEq(bestViaV1.length, submissions.length);
        assertEq(bestViaV1[0].player, submissions[0].player);
        assertEq(bestViaV1[0].board, submissions[0].board);
    }

    function test_GetResults_TwoSubmissionsAtBestScore_ReturnsBoth() public {
        _submitOnly(SCORE_IN_DICT);
        _submitOnly(SCORE_IN_DICT);

        (uint256 numSubmissions, uint256 bestScore, WorcadianGameV4.Submission[] memory submissions) =
            game.getResults(GAME_DAY);

        assertEq(numSubmissions, 2);
        assertEq(bestScore, SCORE_IN_DICT);
        assertEq(submissions.length, 2);
        assertEq(submissions[0].player, address(passportWallet));
        assertEq(submissions[1].player, address(passportWallet));
    }
}
