// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {Test} from "forge-std/Test.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";
import {WorcadianWordListV1} from "../src/WorcadianWordListV1.sol";

contract WorcadianWordListV1Test is Test {
    WorcadianWordListV1 wordList;

    address admin = makeAddr("admin");
    address ownerAddr = makeAddr("owner");
    address upgradeAdmin = makeAddr("upgradeAdmin");
    address wordSmithAdmin = makeAddr("wordSmithAdmin");
    address stranger = makeAddr("stranger");

    function deployProxy(address _admin, address _owner, address _upgradeAdmin, address _wordSmithAdmin)
        internal
        returns (WorcadianWordListV1)
    {
        WorcadianWordListV1 impl = new WorcadianWordListV1();
        ERC1967Proxy proxy = new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WorcadianWordListV1.initialize, (_admin, _owner, _upgradeAdmin, _wordSmithAdmin))
        );
        return WorcadianWordListV1(address(proxy));
    }

    function setUp() public {
        wordList = deployProxy(admin, ownerAddr, upgradeAdmin, wordSmithAdmin);
    }

    // ── initialize ────────────────────────────────────────────────────────────

    function test_Initialize_RolesAssigned() public view {
        assertTrue(wordList.hasRole(wordList.DEFAULT_ADMIN_ROLE(), admin));
        assertTrue(wordList.hasRole(wordList.OWNER_ROLE(), ownerAddr));
        assertTrue(wordList.hasRole(wordList.UPGRADE_ROLE(), upgradeAdmin));
        assertTrue(wordList.hasRole(wordList.WORDSMITH_ROLE(), wordSmithAdmin));
    }

    function test_Initialize_VersionIsOne() public view {
        assertEq(wordList.version(), 1);
    }

    function test_Initialize_OwnerReturnsCorrectAddress() public view {
        assertEq(wordList.owner(), ownerAddr);
    }

    function test_Initialize_ZeroAdmin_Reverts() public {
        WorcadianWordListV1 impl = new WorcadianWordListV1();
        vm.expectRevert(
            abi.encodeWithSelector(
                WorcadianWordListV1.BadAddress.selector, address(0), ownerAddr, upgradeAdmin, wordSmithAdmin
            )
        );
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WorcadianWordListV1.initialize, (address(0), ownerAddr, upgradeAdmin, wordSmithAdmin))
        );
    }

    function test_Initialize_ZeroOwner_Reverts() public {
        WorcadianWordListV1 impl = new WorcadianWordListV1();
        vm.expectRevert(
            abi.encodeWithSelector(
                WorcadianWordListV1.BadAddress.selector, admin, address(0), upgradeAdmin, wordSmithAdmin
            )
        );
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WorcadianWordListV1.initialize, (admin, address(0), upgradeAdmin, wordSmithAdmin))
        );
    }

    function test_Initialize_ZeroUpgradeAdmin_Reverts() public {
        WorcadianWordListV1 impl = new WorcadianWordListV1();
        vm.expectRevert(
            abi.encodeWithSelector(
                WorcadianWordListV1.BadAddress.selector, admin, ownerAddr, address(0), wordSmithAdmin
            )
        );
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WorcadianWordListV1.initialize, (admin, ownerAddr, address(0), wordSmithAdmin))
        );
    }

    function test_Initialize_ZeroWordSmithAdmin_Reverts() public {
        WorcadianWordListV1 impl = new WorcadianWordListV1();
        vm.expectRevert(
            abi.encodeWithSelector(
                WorcadianWordListV1.BadAddress.selector, admin, ownerAddr, upgradeAdmin, address(0)
            )
        );
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WorcadianWordListV1.initialize, (admin, ownerAddr, upgradeAdmin, address(0)))
        );
    }

    function test_Initialize_CannotReinitialize() public {
        vm.expectRevert();
        wordList.initialize(admin, ownerAddr, upgradeAdmin, wordSmithAdmin);
    }

    function test_Initialize_ImplementationDirectly_Reverts() public {
        WorcadianWordListV1 impl = new WorcadianWordListV1();
        vm.expectRevert();
        impl.initialize(admin, ownerAddr, upgradeAdmin, wordSmithAdmin);
    }

    // ── addWords ──────────────────────────────────────────────────────────────

    function test_AddWords_WordSmith_SingleWord() public {
        bytes[] memory words = new bytes[](1);
        words[0] = bytes("CRANE");

        vm.prank(wordSmithAdmin);
        wordList.addWords(words);

        assertTrue(wordList.inWordList(bytes("CRANE")));
    }

    function test_AddWords_WordSmith_MultipleWords() public {
        bytes[] memory words = new bytes[](3);
        words[0] = bytes("CRANE");
        words[1] = bytes("SLATE");
        words[2] = bytes("AUDIO");

        vm.prank(wordSmithAdmin);
        wordList.addWords(words);

        assertTrue(wordList.inWordList(bytes("CRANE")));
        assertTrue(wordList.inWordList(bytes("SLATE")));
        assertTrue(wordList.inWordList(bytes("AUDIO")));
    }

    function test_AddWords_EmitsWordsAddedEvent() public {
        bytes[] memory words = new bytes[](2);
        words[0] = bytes("CRANE");
        words[1] = bytes("SLATE");

        vm.expectEmit(true, true, true, true, address(wordList));
        emit WorcadianWordListV1.WordsAdded(2);

        vm.prank(wordSmithAdmin);
        wordList.addWords(words);
    }

    function test_AddWords_EmitsWordsAddedWithCorrectCount() public {
        bytes[] memory words = new bytes[](5);
        words[0] = bytes("CRANE");
        words[1] = bytes("SLATE");
        words[2] = bytes("AUDIO");
        words[3] = bytes("ORATE");
        words[4] = bytes("RAISE");

        vm.expectEmit(true, true, true, true, address(wordList));
        emit WorcadianWordListV1.WordsAdded(5);

        vm.prank(wordSmithAdmin);
        wordList.addWords(words);
    }

    function test_AddWords_DuplicateWord_Idempotent() public {
        bytes[] memory words = new bytes[](1);
        words[0] = bytes("CRANE");

        vm.startPrank(wordSmithAdmin);
        wordList.addWords(words);
        wordList.addWords(words); // add same word again
        vm.stopPrank();

        assertTrue(wordList.inWordList(bytes("CRANE")));
    }

    function test_AddWords_Stranger_Reverts() public {
        bytes[] memory words = new bytes[](1);
        words[0] = bytes("CRANE");

        vm.prank(stranger);
        vm.expectRevert();
        wordList.addWords(words);
    }

    function test_AddWords_Admin_Reverts() public {
        bytes[] memory words = new bytes[](1);
        words[0] = bytes("CRANE");

        vm.prank(admin);
        vm.expectRevert();
        wordList.addWords(words);
    }

    function test_AddWords_EmptyArray_EmitsZeroCount() public {
        bytes[] memory words = new bytes[](0);

        vm.expectEmit(true, true, true, true, address(wordList));
        emit WorcadianWordListV1.WordsAdded(0);

        vm.prank(wordSmithAdmin);
        wordList.addWords(words);
    }

    // ── removeWord ────────────────────────────────────────────────────────────

    function test_RemoveWord_WordSmith_RemovesWord() public {
        bytes[] memory words = new bytes[](1);
        words[0] = bytes("CRANE");

        vm.startPrank(wordSmithAdmin);
        wordList.addWords(words);
        wordList.removeWord(bytes("CRANE"));
        vm.stopPrank();

        assertFalse(wordList.inWordList(bytes("CRANE")));
    }

    function test_RemoveWord_EmitsWordRemovedEvent() public {
        bytes[] memory words = new bytes[](1);
        words[0] = bytes("CRANE");

        vm.prank(wordSmithAdmin);
        wordList.addWords(words);

        vm.expectEmit(true, true, true, true, address(wordList));
        emit WorcadianWordListV1.WordRemoved("CRANE");

        vm.prank(wordSmithAdmin);
        wordList.removeWord(bytes("CRANE"));
    }

    function test_RemoveWord_NonExistentWord_Reverts() public {
        vm.prank(wordSmithAdmin);
        vm.expectRevert(
            abi.encodeWithSelector(WorcadianWordListV1.WordNotInList.selector, "CRANE")
        );
        wordList.removeWord(bytes("CRANE"));
    }

    function test_RemoveWord_AfterRemoval_CannotRemoveAgain() public {
        bytes[] memory words = new bytes[](1);
        words[0] = bytes("CRANE");

        vm.startPrank(wordSmithAdmin);
        wordList.addWords(words);
        wordList.removeWord(bytes("CRANE"));

        vm.expectRevert(
            abi.encodeWithSelector(WorcadianWordListV1.WordNotInList.selector, "CRANE")
        );
        wordList.removeWord(bytes("CRANE"));
        vm.stopPrank();
    }

    function test_RemoveWord_OnlyRemovesTargetWord() public {
        bytes[] memory words = new bytes[](2);
        words[0] = bytes("CRANE");
        words[1] = bytes("SLATE");

        vm.startPrank(wordSmithAdmin);
        wordList.addWords(words);
        wordList.removeWord(bytes("CRANE"));
        vm.stopPrank();

        assertFalse(wordList.inWordList(bytes("CRANE")));
        assertTrue(wordList.inWordList(bytes("SLATE")));
    }

    function test_RemoveWord_Stranger_Reverts() public {
        bytes[] memory words = new bytes[](1);
        words[0] = bytes("CRANE");

        vm.prank(wordSmithAdmin);
        wordList.addWords(words);

        vm.prank(stranger);
        vm.expectRevert();
        wordList.removeWord(bytes("CRANE"));
    }

    function test_RemoveWord_CanReAddAfterRemoval() public {
        bytes[] memory words = new bytes[](1);
        words[0] = bytes("CRANE");

        vm.startPrank(wordSmithAdmin);
        wordList.addWords(words);
        wordList.removeWord(bytes("CRANE"));
        wordList.addWords(words);
        vm.stopPrank();

        assertTrue(wordList.inWordList(bytes("CRANE")));
    }

    // ── inWordList ────────────────────────────────────────────────────────────

    function test_InWordList_WordPresent_ReturnsTrue() public {
        bytes[] memory words = new bytes[](1);
        words[0] = bytes("CRANE");

        vm.prank(wordSmithAdmin);
        wordList.addWords(words);

        assertTrue(wordList.inWordList(bytes("CRANE")));
    }

    function test_InWordList_WordAbsent_ReturnsFalse() public view {
        assertFalse(wordList.inWordList(bytes("CRANE")));
    }

    function test_InWordList_CaseSensitive() public {
        bytes[] memory words = new bytes[](1);
        words[0] = bytes("CRANE");

        vm.prank(wordSmithAdmin);
        wordList.addWords(words);

        assertTrue(wordList.inWordList(bytes("CRANE")));
        assertFalse(wordList.inWordList(bytes("crane")));
        assertFalse(wordList.inWordList(bytes("Crane")));
    }

    function test_InWordList_EmptyBytes_NotInList() public view {
        assertFalse(wordList.inWordList(bytes("")));
    }

    // ── inWordListBulk ────────────────────────────────────────────────────────

    function test_InWordListBulk_AllPresent_ReturnsAllTrue() public {
        bytes[] memory words = new bytes[](3);
        words[0] = bytes("CRANE");
        words[1] = bytes("SLATE");
        words[2] = bytes("AUDIO");

        vm.prank(wordSmithAdmin);
        wordList.addWords(words);

        bool[] memory result = wordList.inWordListBulk(words);
        assertEq(result.length, 3);
        assertTrue(result[0]);
        assertTrue(result[1]);
        assertTrue(result[2]);
    }

    function test_InWordListBulk_NonePresent_ReturnsAllFalse() public view {
        bytes[] memory words = new bytes[](2);
        words[0] = bytes("CRANE");
        words[1] = bytes("SLATE");

        bool[] memory result = wordList.inWordListBulk(words);
        assertEq(result.length, 2);
        assertFalse(result[0]);
        assertFalse(result[1]);
    }

    function test_InWordListBulk_MixedPresence_ReturnsCorrectFlags() public {
        bytes[] memory toAdd = new bytes[](1);
        toAdd[0] = bytes("CRANE");

        vm.prank(wordSmithAdmin);
        wordList.addWords(toAdd);

        bytes[] memory toCheck = new bytes[](2);
        toCheck[0] = bytes("CRANE");
        toCheck[1] = bytes("SLATE");

        bool[] memory result = wordList.inWordListBulk(toCheck);
        assertEq(result.length, 2);
        assertTrue(result[0]);
        assertFalse(result[1]);
    }

    function test_InWordListBulk_EmptyInput_ReturnsEmptyArray() public view {
        bytes[] memory words = new bytes[](0);
        bool[] memory result = wordList.inWordListBulk(words);
        assertEq(result.length, 0);
    }

    // ── owner ─────────────────────────────────────────────────────────────────

    function test_Owner_ReturnsOwnerRoleMember() public view {
        assertEq(wordList.owner(), ownerAddr);
    }

    function test_Owner_NoMembers_ReturnsAddressZero() public {
        vm.startPrank(admin);
        wordList.revokeRole(wordList.OWNER_ROLE(), ownerAddr);
        vm.stopPrank();
        assertEq(wordList.owner(), address(0));
    }

    function test_Owner_AfterRoleTransfer() public {
        address newOwner = makeAddr("newOwner");
        vm.startPrank(admin);
        wordList.grantRole(wordList.OWNER_ROLE(), newOwner);
        wordList.revokeRole(wordList.OWNER_ROLE(), ownerAddr);
        vm.stopPrank();
        assertEq(wordList.owner(), newOwner);
    }

    // ── upgradeStorage ────────────────────────────────────────────────────────

    function test_UpgradeStorage_AlwaysReverts() public {
        vm.expectRevert(
            abi.encodeWithSelector(WorcadianWordListV1.CanNotUpgradeToLowerOrSameVersion.selector, uint256(1))
        );
        wordList.upgradeStorage("");
    }

    function test_UpgradeStorage_AnyCallerReverts() public {
        vm.prank(stranger);
        vm.expectRevert(
            abi.encodeWithSelector(WorcadianWordListV1.CanNotUpgradeToLowerOrSameVersion.selector, uint256(1))
        );
        wordList.upgradeStorage("");
    }

    // ── _authorizeUpgrade ─────────────────────────────────────────────────────

    function test_Upgrade_WithUpgradeRole_Succeeds() public {
        WorcadianWordListV1 newImpl = new WorcadianWordListV1();
        vm.prank(upgradeAdmin);
        wordList.upgradeToAndCall(address(newImpl), "");
    }

    function test_Upgrade_PreservesStorageAfterUpgrade() public {
        bytes[] memory words = new bytes[](1);
        words[0] = bytes("CRANE");

        vm.prank(wordSmithAdmin);
        wordList.addWords(words);

        WorcadianWordListV1 newImpl = new WorcadianWordListV1();
        vm.prank(upgradeAdmin);
        wordList.upgradeToAndCall(address(newImpl), "");

        assertEq(wordList.version(), 1);
        assertTrue(wordList.inWordList(bytes("CRANE")));
        assertTrue(wordList.hasRole(wordList.OWNER_ROLE(), ownerAddr));
        assertTrue(wordList.hasRole(wordList.WORDSMITH_ROLE(), wordSmithAdmin));
    }

    function test_Upgrade_WithoutUpgradeRole_Reverts() public {
        WorcadianWordListV1 newImpl = new WorcadianWordListV1();
        vm.prank(stranger);
        vm.expectRevert();
        wordList.upgradeToAndCall(address(newImpl), "");
    }

    function test_Upgrade_ByAdmin_WithoutUpgradeRole_Reverts() public {
        // DEFAULT_ADMIN_ROLE does not grant upgrade rights
        WorcadianWordListV1 newImpl = new WorcadianWordListV1();
        vm.prank(admin);
        vm.expectRevert();
        wordList.upgradeToAndCall(address(newImpl), "");
    }
}
