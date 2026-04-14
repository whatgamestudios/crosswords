// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {Test} from "forge-std/Test.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";
import {WordListSeed} from "../src/WordListSeed.sol";

contract WordListSeedTest is Test {
    WordListSeed wordListSeed;

    address admin = makeAddr("admin");
    address ownerAddr = makeAddr("owner");
    address upgradeAdmin = makeAddr("upgradeAdmin");
    address wordSmithAdmin = makeAddr("wordSmithAdmin");
    address stranger = makeAddr("stranger");

    function deployProxy(address _admin, address _owner, address _upgradeAdmin, address _wordSmithAdmin)
        internal
        returns (WordListSeed)
    {
        WordListSeed impl = new WordListSeed();
        ERC1967Proxy proxy = new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WordListSeed.initialize, (_admin, _owner, _upgradeAdmin, _wordSmithAdmin))
        );
        return WordListSeed(address(proxy));
    }

    function setUp() public {
        wordListSeed = deployProxy(admin, ownerAddr, upgradeAdmin, wordSmithAdmin);
    }

    // ── initialize ────────────────────────────────────────────────────────────

    function test_Initialize_RolesAssigned() public view {
        assertTrue(wordListSeed.hasRole(wordListSeed.DEFAULT_ADMIN_ROLE(), admin));
        assertTrue(wordListSeed.hasRole(wordListSeed.OWNER_ROLE(), ownerAddr));
        assertTrue(wordListSeed.hasRole(wordListSeed.UPGRADE_ROLE(), upgradeAdmin));
        assertTrue(wordListSeed.hasRole(wordListSeed.WORDSMITH_ROLE(), wordSmithAdmin));
    }

    function test_Initialize_VersionIsOne() public view {
        assertEq(wordListSeed.version(), 1);
    }

    function test_Initialize_OwnerReturnsCorrectAddress() public view {
        assertEq(wordListSeed.owner(), ownerAddr);
    }

    function test_Initialize_SeedWordCountIsZero() public view {
        assertEq(wordListSeed.seedWordCount(), 0);
    }

    function test_Initialize_ZeroAdmin_Reverts() public {
        WordListSeed impl = new WordListSeed();
        vm.expectRevert(
            abi.encodeWithSelector(
                WordListSeed.BadAddress.selector, address(0), ownerAddr, upgradeAdmin, wordSmithAdmin
            )
        );
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WordListSeed.initialize, (address(0), ownerAddr, upgradeAdmin, wordSmithAdmin))
        );
    }

    function test_Initialize_ZeroOwner_Reverts() public {
        WordListSeed impl = new WordListSeed();
        vm.expectRevert(
            abi.encodeWithSelector(
                WordListSeed.BadAddress.selector, admin, address(0), upgradeAdmin, wordSmithAdmin
            )
        );
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WordListSeed.initialize, (admin, address(0), upgradeAdmin, wordSmithAdmin))
        );
    }

    function test_Initialize_ZeroUpgradeAdmin_Reverts() public {
        WordListSeed impl = new WordListSeed();
        vm.expectRevert(
            abi.encodeWithSelector(
                WordListSeed.BadAddress.selector, admin, ownerAddr, address(0), wordSmithAdmin
            )
        );
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WordListSeed.initialize, (admin, ownerAddr, address(0), wordSmithAdmin))
        );
    }

    function test_Initialize_ZeroWordSmithAdmin_Reverts() public {
        WordListSeed impl = new WordListSeed();
        vm.expectRevert(
            abi.encodeWithSelector(
                WordListSeed.BadAddress.selector, admin, ownerAddr, upgradeAdmin, address(0)
            )
        );
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WordListSeed.initialize, (admin, ownerAddr, upgradeAdmin, address(0)))
        );
    }

    function test_Initialize_CannotReinitialize() public {
        vm.expectRevert();
        wordListSeed.initialize(admin, ownerAddr, upgradeAdmin, wordSmithAdmin);
    }

    function test_Initialize_ImplementationDirectly_Reverts() public {
        WordListSeed impl = new WordListSeed();
        vm.expectRevert();
        impl.initialize(admin, ownerAddr, upgradeAdmin, wordSmithAdmin);
    }

    // ── addSeedWords ──────────────────────────────────────────────────────────

    function test_AddSeedWords_WordSmith_SingleWord() public {
        string[] memory words = new string[](1);
        words[0] = "crane";

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);

        assertEq(wordListSeed.seedWordCount(), 1);
    }

    function test_AddSeedWords_WordSmith_MultipleWords() public {
        string[] memory words = new string[](3);
        words[0] = "crane";
        words[1] = "slate";
        words[2] = "audio";

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);

        assertEq(wordListSeed.seedWordCount(), 3);
    }

    function test_AddSeedWords_AppendedInOrder() public {
        string[] memory batch1 = new string[](2);
        batch1[0] = "crane";
        batch1[1] = "slate";

        string[] memory batch2 = new string[](2);
        batch2[0] = "audio";
        batch2[1] = "raise";

        vm.startPrank(wordSmithAdmin);
        wordListSeed.addSeedWords(batch1);
        wordListSeed.addSeedWords(batch2);
        vm.stopPrank();

        assertEq(wordListSeed.seedWordCount(), 4);
        assertEq(wordListSeed.getSeedWord(0), "CRANE");
        assertEq(wordListSeed.getSeedWord(1), "SLATE");
        assertEq(wordListSeed.getSeedWord(2), "AUDIO");
        assertEq(wordListSeed.getSeedWord(3), "RAISE");
    }

    function test_AddSeedWords_EmitsEvent() public {
        string[] memory words = new string[](2);
        words[0] = "crane";
        words[1] = "slate";

        vm.expectEmit(true, true, true, true, address(wordListSeed));
        emit WordListSeed.SeedWordsAdded(2);

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);
    }

    function test_AddSeedWords_EmitsEventWithCorrectCount() public {
        string[] memory words = new string[](5);
        words[0] = "crane";
        words[1] = "slate";
        words[2] = "audio";
        words[3] = "raise";
        words[4] = "orate";

        vm.expectEmit(true, true, true, true, address(wordListSeed));
        emit WordListSeed.SeedWordsAdded(5);

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);
    }

    function test_AddSeedWords_EmptyArray_EmitsZeroCount() public {
        string[] memory words = new string[](0);

        vm.expectEmit(true, true, true, true, address(wordListSeed));
        emit WordListSeed.SeedWordsAdded(0);

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);
    }

    function test_AddSeedWords_EmptyArray_CountUnchanged() public {
        string[] memory words = new string[](0);

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);

        assertEq(wordListSeed.seedWordCount(), 0);
    }

    function test_AddSeedWords_Stranger_Reverts() public {
        string[] memory words = new string[](1);
        words[0] = "crane";

        vm.prank(stranger);
        vm.expectRevert();
        wordListSeed.addSeedWords(words);
    }

    function test_AddSeedWords_Admin_Reverts() public {
        string[] memory words = new string[](1);
        words[0] = "crane";

        vm.prank(admin);
        vm.expectRevert();
        wordListSeed.addSeedWords(words);
    }

    function test_AddSeedWords_Owner_Reverts() public {
        string[] memory words = new string[](1);
        words[0] = "crane";

        vm.prank(ownerAddr);
        vm.expectRevert();
        wordListSeed.addSeedWords(words);
    }

    // ── getSeedWord ───────────────────────────────────────────────────────────

    function test_GetSeedWord_EmptyList_Reverts() public {
        vm.expectRevert(abi.encodeWithSelector(WordListSeed.EmptyWordList.selector));
        wordListSeed.getSeedWord(0);
    }

    function test_GetSeedWord_DayZero_ReturnsFirstWord() public {
        string[] memory words = new string[](3);
        words[0] = "crane";
        words[1] = "slate";
        words[2] = "audio";

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);

        assertEq(wordListSeed.getSeedWord(0), "CRANE");
    }

    function test_GetSeedWord_DayOne_ReturnsSecondWord() public {
        string[] memory words = new string[](3);
        words[0] = "crane";
        words[1] = "slate";
        words[2] = "audio";

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);

        assertEq(wordListSeed.getSeedWord(1), "SLATE");
    }

    function test_GetSeedWord_LastIndex_ReturnsLastWord() public {
        string[] memory words = new string[](3);
        words[0] = "crane";
        words[1] = "slate";
        words[2] = "audio";

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);

        assertEq(wordListSeed.getSeedWord(2), "AUDIO");
    }

    function test_GetSeedWord_WrapsAroundWithModulo() public {
        string[] memory words = new string[](3);
        words[0] = "crane";
        words[1] = "slate";
        words[2] = "audio";

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);

        // Day 3 should wrap back to index 0
        assertEq(wordListSeed.getSeedWord(3), "CRANE");
        // Day 4 → index 1
        assertEq(wordListSeed.getSeedWord(4), "SLATE");
        // Day 5 → index 2
        assertEq(wordListSeed.getSeedWord(5), "AUDIO");
        // Day 6 → index 0 again
        assertEq(wordListSeed.getSeedWord(6), "CRANE");
    }

    function test_GetSeedWord_ReturnsUppercase() public {
        string[] memory words = new string[](1);
        words[0] = "gipsy";

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);

        assertEq(wordListSeed.getSeedWord(0), "GIPSY");
    }

    function test_GetSeedWord_AlreadyUppercase_Unchanged() public {
        string[] memory words = new string[](1);
        words[0] = "CRANE";

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);

        assertEq(wordListSeed.getSeedWord(0), "CRANE");
    }

    function test_GetSeedWord_MixedCase_FullyUppercased() public {
        string[] memory words = new string[](1);
        words[0] = "cRaNe";

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);

        assertEq(wordListSeed.getSeedWord(0), "CRANE");
    }

    function test_GetSeedWord_SingleWordList_AlwaysReturnsSameWord() public {
        string[] memory words = new string[](1);
        words[0] = "crane";

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);

        assertEq(wordListSeed.getSeedWord(0), "CRANE");
        assertEq(wordListSeed.getSeedWord(1), "CRANE");
        assertEq(wordListSeed.getSeedWord(999), "CRANE");
    }

    function test_GetSeedWord_MatchesCsharpBehaviour_FirstEightWords() public {
        // Mirrors the first 8 entries of WordListSeed.cs seedWords array
        string[] memory words = new string[](8);
        words[0] = "gipsy";
        words[1] = "piles";
        words[2] = "posed";
        words[3] = "flows";
        words[4] = "scrap";
        words[5] = "pleas";
        words[6] = "knead";
        words[7] = "tired";

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);

        assertEq(wordListSeed.getSeedWord(0), "GIPSY");
        assertEq(wordListSeed.getSeedWord(1), "PILES");
        assertEq(wordListSeed.getSeedWord(2), "POSED");
        assertEq(wordListSeed.getSeedWord(3), "FLOWS");
        assertEq(wordListSeed.getSeedWord(4), "SCRAP");
        assertEq(wordListSeed.getSeedWord(5), "PLEAS");
        assertEq(wordListSeed.getSeedWord(6), "KNEAD");
        assertEq(wordListSeed.getSeedWord(7), "TIRED");
    }

    function test_GetSeedWord_LargeGameDay_WrapsCorrectly() public {
        string[] memory words = new string[](3);
        words[0] = "crane";
        words[1] = "slate";
        words[2] = "audio";

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);

        uint256 largeDay = 1_000_000;
        assertEq(wordListSeed.getSeedWord(largeDay), wordListSeed.getSeedWord(largeDay % 3));
    }

    // ── seedWordCount ─────────────────────────────────────────────────────────

    function test_SeedWordCount_InitiallyZero() public view {
        assertEq(wordListSeed.seedWordCount(), 0);
    }

    function test_SeedWordCount_IncreasesWithEachAdd() public {
        string[] memory batch1 = new string[](2);
        batch1[0] = "crane";
        batch1[1] = "slate";

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(batch1);
        assertEq(wordListSeed.seedWordCount(), 2);

        string[] memory batch2 = new string[](3);
        batch2[0] = "audio";
        batch2[1] = "raise";
        batch2[2] = "orate";

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(batch2);
        assertEq(wordListSeed.seedWordCount(), 5);
    }

    // ── owner ─────────────────────────────────────────────────────────────────

    function test_Owner_ReturnsOwnerRoleMember() public view {
        assertEq(wordListSeed.owner(), ownerAddr);
    }

    function test_Owner_NoMembers_ReturnsAddressZero() public {
        vm.startPrank(admin);
        wordListSeed.revokeRole(wordListSeed.OWNER_ROLE(), ownerAddr);
        vm.stopPrank();
        assertEq(wordListSeed.owner(), address(0));
    }

    function test_Owner_AfterRoleTransfer() public {
        address newOwner = makeAddr("newOwner");
        vm.startPrank(admin);
        wordListSeed.grantRole(wordListSeed.OWNER_ROLE(), newOwner);
        wordListSeed.revokeRole(wordListSeed.OWNER_ROLE(), ownerAddr);
        vm.stopPrank();
        assertEq(wordListSeed.owner(), newOwner);
    }

    // ── upgradeStorage ────────────────────────────────────────────────────────

    function test_UpgradeStorage_AlwaysReverts() public {
        vm.expectRevert(
            abi.encodeWithSelector(WordListSeed.CanNotUpgradeToLowerOrSameVersion.selector, uint256(1))
        );
        wordListSeed.upgradeStorage("");
    }

    function test_UpgradeStorage_AnyCallerReverts() public {
        vm.prank(stranger);
        vm.expectRevert(
            abi.encodeWithSelector(WordListSeed.CanNotUpgradeToLowerOrSameVersion.selector, uint256(1))
        );
        wordListSeed.upgradeStorage("");
    }

    // ── _authorizeUpgrade ─────────────────────────────────────────────────────

    function test_Upgrade_WithUpgradeRole_Succeeds() public {
        WordListSeed newImpl = new WordListSeed();
        vm.prank(upgradeAdmin);
        wordListSeed.upgradeToAndCall(address(newImpl), "");
    }

    function test_Upgrade_PreservesStorageAfterUpgrade() public {
        string[] memory words = new string[](2);
        words[0] = "crane";
        words[1] = "slate";

        vm.prank(wordSmithAdmin);
        wordListSeed.addSeedWords(words);

        WordListSeed newImpl = new WordListSeed();
        vm.prank(upgradeAdmin);
        wordListSeed.upgradeToAndCall(address(newImpl), "");

        assertEq(wordListSeed.version(), 1);
        assertEq(wordListSeed.seedWordCount(), 2);
        assertEq(wordListSeed.getSeedWord(0), "CRANE");
        assertEq(wordListSeed.getSeedWord(1), "SLATE");
        assertTrue(wordListSeed.hasRole(wordListSeed.OWNER_ROLE(), ownerAddr));
        assertTrue(wordListSeed.hasRole(wordListSeed.WORDSMITH_ROLE(), wordSmithAdmin));
    }

    function test_Upgrade_WithoutUpgradeRole_Reverts() public {
        WordListSeed newImpl = new WordListSeed();
        vm.prank(stranger);
        vm.expectRevert();
        wordListSeed.upgradeToAndCall(address(newImpl), "");
    }

    function test_Upgrade_ByAdmin_WithoutUpgradeRole_Reverts() public {
        // DEFAULT_ADMIN_ROLE does not grant upgrade rights
        WordListSeed newImpl = new WordListSeed();
        vm.prank(admin);
        vm.expectRevert();
        wordListSeed.upgradeToAndCall(address(newImpl), "");
    }
}
