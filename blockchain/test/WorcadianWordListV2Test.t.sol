// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {Test} from "forge-std/Test.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";
import {WorcadianWordListV1} from "../src/WorcadianWordListV1.sol";
import {WorcadianWordListV2} from "../src/WorcadianWordListV2.sol";

contract WorcadianWordListV2Test is Test {
    WorcadianWordListV1 wordList;

    address admin = makeAddr("admin");
    address ownerAddr = makeAddr("owner");
    address upgradeAdmin = makeAddr("upgradeAdmin");
    address wordSmithAdmin = makeAddr("wordSmithAdmin");


    function deployProxy(address _admin, address _owner, address _upgradeAdmin, address _wordSmithAdmin)
        internal
        returns (WorcadianWordListV1)
    {
        WorcadianWordListV1 impl = new WorcadianWordListV1();
        ERC1967Proxy proxy = new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WorcadianWordListV1.initialize, (_admin, _owner, _upgradeAdmin, _wordSmithAdmin))
        );

        WorcadianWordListV2 implV2 = new WorcadianWordListV2();
        vm.prank(_upgradeAdmin);
        WorcadianWordListV2(address(proxy)).upgradeToAndCall(address(implV2), 
            abi.encodeWithSelector(WorcadianWordListV2.upgradeStorage.selector, bytes(""))
        );
        return WorcadianWordListV1(address(proxy));
    }

    function setUp() public {
        wordList = deployProxy(admin, ownerAddr, upgradeAdmin, wordSmithAdmin);
    }

    function test_CheckVersion() public view {
        assertEq(wordList.version(), 2);
    }

    function test_AddWords_WordSmith_SingleLowercaseWord() public {
        bytes[] memory words = new bytes[](1);
        words[0] = bytes("crane");

        vm.prank(wordSmithAdmin);
        wordList.addWords(words);

        assertTrue(wordList.inWordList(bytes("CRANE")));
    }

    function test_AddWords_WordSmith_MultipleLowercaseWords() public {
        bytes[] memory words = new bytes[](3);
        words[0] = bytes("crane");
        words[1] = bytes("slate");
        words[2] = bytes("audio");

        vm.prank(wordSmithAdmin);
        wordList.addWords(words);

        assertTrue(wordList.inWordList(bytes("CRANE")));
        assertTrue(wordList.inWordList(bytes("slate"))); // lower case
        assertTrue(wordList.inWordList(bytes("Audio"))); // mixed case
    }
}
