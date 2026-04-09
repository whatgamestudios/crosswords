// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: UNLICENSED
pragma solidity ^0.8.26;

import {Test} from "forge-std/Test.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";
import {WorcadianCheckInV1} from "../src/WorcadianCheckInV1.sol";

contract WorcadianCheckInV1Test is Test {
    WorcadianCheckInV1 checkin;

    address admin        = makeAddr("admin");
    address ownerAddr    = makeAddr("owner");
    address upgradeAdmin = makeAddr("upgradeAdmin");
    address player1      = makeAddr("player1");
    address player2      = makeAddr("player2");
    address stranger     = makeAddr("stranger");

    uint256 constant GAME_START = 1743292800; // March 30, 2026 00:00:00 UTC
    uint256 constant DAY        = 86400;
    uint256 constant MINUS_12   = 43200;

    // Returns noon UTC on _gameDay — unambiguously valid (minDay = _gameDay, maxDay = _gameDay + 1)
    function noonOn(uint32 gameDay) internal pure returns (uint256) {
        return GAME_START + uint256(gameDay) * DAY + DAY / 2;
    }

    function deployProxy(
        address _admin,
        address _owner,
        address _upgradeAdmin
    ) internal returns (WorcadianCheckInV1) {
        WorcadianCheckInV1 impl = new WorcadianCheckInV1();
        ERC1967Proxy proxy = new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WorcadianCheckInV1.initialize, (_admin, _owner, _upgradeAdmin))
        );
        return WorcadianCheckInV1(address(proxy));
    }

    function setUp() public {
        checkin = deployProxy(admin, ownerAddr, upgradeAdmin);
    }

    // ── initialize ────────────────────────────────────────────────────────────

    function test_Initialize_RolesAssigned() public view {
        assertTrue(checkin.hasRole(checkin.DEFAULT_ADMIN_ROLE(), admin));
        assertTrue(checkin.hasRole(checkin.OWNER_ROLE(), ownerAddr));
        assertTrue(checkin.hasRole(checkin.UPGRADE_ROLE(), upgradeAdmin));
    }

    function test_Initialize_VersionIsOne() public view {
        assertEq(checkin.version(), 1);
    }

    function test_Initialize_OwnerReturnsCorrectAddress() public view {
        assertEq(checkin.owner(), ownerAddr);
    }

    function test_Initialize_ZeroAdmin_Reverts() public {
        WorcadianCheckInV1 impl = new WorcadianCheckInV1();
        vm.expectRevert(abi.encodeWithSelector(
            WorcadianCheckInV1.BadAddress.selector, address(0), ownerAddr, upgradeAdmin
        ));
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WorcadianCheckInV1.initialize, (address(0), ownerAddr, upgradeAdmin))
        );
    }

    function test_Initialize_ZeroOwner_Reverts() public {
        WorcadianCheckInV1 impl = new WorcadianCheckInV1();
        vm.expectRevert(abi.encodeWithSelector(
            WorcadianCheckInV1.BadAddress.selector, admin, address(0), upgradeAdmin
        ));
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WorcadianCheckInV1.initialize, (admin, address(0), upgradeAdmin))
        );
    }

    function test_Initialize_ZeroUpgradeAdmin_Reverts() public {
        WorcadianCheckInV1 impl = new WorcadianCheckInV1();
        vm.expectRevert(abi.encodeWithSelector(
            WorcadianCheckInV1.BadAddress.selector, admin, ownerAddr, address(0)
        ));
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WorcadianCheckInV1.initialize, (admin, ownerAddr, address(0)))
        );
    }

    function test_Initialize_CannotReinitialize() public {
        vm.expectRevert();
        checkin.initialize(admin, ownerAddr, upgradeAdmin);
    }

    function test_Initialize_ImplementationDirectly_Reverts() public {
        // _disableInitializers() in the constructor prevents direct init of the impl
        WorcadianCheckInV1 impl = new WorcadianCheckInV1();
        vm.expectRevert();
        impl.initialize(admin, ownerAddr, upgradeAdmin);
    }

    // ── checkIn ───────────────────────────────────────────────────────────────

    function test_CheckIn_FirstPlayer_AllCountersUpdated() public {
        uint32 gameDay = 10;
        vm.warp(noonOn(gameDay));

        vm.prank(player1);
        checkin.checkIn(gameDay);

        assertEq(checkin.getDaysPlayed(player1), 1);
        assertEq(checkin.numberOfPlayers(gameDay), 1);
        assertEq(checkin.numberOfSessions(gameDay), 1);
    }

    function test_CheckIn_EmitsCheckInEvent() public {
        uint32 gameDay = 10;
        vm.warp(noonOn(gameDay));

        vm.expectEmit(true, true, true, true, address(checkin));
        emit WorcadianCheckInV1.CheckIn(gameDay, player1, 1);

        vm.prank(player1);
        checkin.checkIn(gameDay);
    }

    function test_CheckIn_SameDay_SecondSession_OnlyIncrementsSession() public {
        uint32 gameDay = 10;
        vm.warp(noonOn(gameDay));

        vm.startPrank(player1);
        checkin.checkIn(gameDay);
        checkin.checkIn(gameDay);
        vm.stopPrank();

        assertEq(checkin.getDaysPlayed(player1), 1);       // not double-counted
        assertEq(checkin.numberOfPlayers(gameDay), 1);     // not double-counted
        assertEq(checkin.numberOfSessions(gameDay), 2);    // both sessions recorded
    }

    function test_CheckIn_SameDay_NoSecondEvent() public {
        uint32 gameDay = 10;
        vm.warp(noonOn(gameDay));

        vm.startPrank(player1);
        checkin.checkIn(gameDay);

        // Second check-in on the same day must not emit CheckIn
        vm.recordLogs();
        checkin.checkIn(gameDay);
        vm.stopPrank();

        assertEq(vm.getRecordedLogs().length, 0);
    }

    function test_CheckIn_MultipleDays_IncreasesDaysPlayed() public {
        for (uint32 day = 10; day < 15; day++) {
            vm.warp(noonOn(day));
            vm.prank(player1);
            checkin.checkIn(day);
        }

        assertEq(checkin.getDaysPlayed(player1), 5);
        for (uint32 day = 10; day < 15; day++) {
            assertEq(checkin.numberOfPlayers(day), 1);
            assertEq(checkin.numberOfSessions(day), 1);
        }
    }

    function test_CheckIn_MultiplePlayers_SameDay() public {
        uint32 gameDay = 10;
        vm.warp(noonOn(gameDay));

        vm.prank(player1); checkin.checkIn(gameDay);
        vm.prank(player2); checkin.checkIn(gameDay);

        assertEq(checkin.numberOfPlayers(gameDay), 2);
        assertEq(checkin.numberOfSessions(gameDay), 2);
        assertEq(checkin.getDaysPlayed(player1), 1);
        assertEq(checkin.getDaysPlayed(player2), 1);
    }

    function test_CheckIn_DayZero_Succeeds() public {
        // The daysPlayed == 0 guard in checkIn handles the day-0 edge case
        vm.warp(GAME_START + MINUS_12);

        vm.prank(player1);
        checkin.checkIn(0);

        assertEq(checkin.getDaysPlayed(player1), 1);
        assertEq(checkin.numberOfPlayers(0), 1);
        assertEq(checkin.numberOfSessions(0), 1);
    }

    function test_CheckIn_DayZero_SecondSession_NotDoubleCountedAsPlayer() public {
        vm.warp(GAME_START + MINUS_12);

        vm.startPrank(player1);
        checkin.checkIn(0);
        checkin.checkIn(0);
        vm.stopPrank();

        assertEq(checkin.getDaysPlayed(player1), 1);
        assertEq(checkin.numberOfPlayers(0), 1);
        assertEq(checkin.numberOfSessions(0), 2);
    }

    function test_CheckIn_DayZeroThenDayOne_BothRecorded() public {
        // Player plays day 0, then day 1 — both should be counted
        vm.warp(GAME_START + MINUS_12);
        vm.prank(player1);
        checkin.checkIn(0);

        vm.warp(noonOn(1));
        vm.prank(player1);
        checkin.checkIn(1);

        assertEq(checkin.getDaysPlayed(player1), 2);
        assertEq(checkin.numberOfPlayers(0), 1);
        assertEq(checkin.numberOfPlayers(1), 1);
    }

    function test_CheckIn_FutureDay_Reverts() public {
        vm.warp(noonOn(10)); // valid: [10, 11]
        vm.prank(player1);
        vm.expectRevert();
        checkin.checkIn(13);
    }

    function test_CheckIn_PastDay_Reverts() public {
        vm.warp(noonOn(10)); // valid: [10, 11]
        vm.prank(player1);
        vm.expectRevert();
        checkin.checkIn(8);
    }

    // ── getDaysPlayed ─────────────────────────────────────────────────────────

    function test_GetDaysPlayed_NewPlayer_ReturnsZero() public view {
        assertEq(checkin.getDaysPlayed(player1), 0);
    }

    function test_GetDaysPlayed_AccumulatesAcrossDays() public {
        for (uint32 day = 20; day < 23; day++) {
            vm.warp(noonOn(day));
            vm.prank(player1);
            checkin.checkIn(day);
        }
        assertEq(checkin.getDaysPlayed(player1), 3);
    }

    function test_GetDaysPlayed_MultipleSessionsSameDay_CountsOnce() public {
        uint32 gameDay = 20;
        vm.warp(noonOn(gameDay));

        vm.startPrank(player1);
        checkin.checkIn(gameDay);
        checkin.checkIn(gameDay);
        checkin.checkIn(gameDay);
        vm.stopPrank();

        assertEq(checkin.getDaysPlayed(player1), 1);
    }

    function test_GetDaysPlayed_IndependentPerPlayer() public {
        vm.warp(noonOn(10));
        vm.prank(player1); checkin.checkIn(10);

        vm.warp(noonOn(11));
        vm.prank(player1); checkin.checkIn(11);
        vm.prank(player2); checkin.checkIn(11);

        assertEq(checkin.getDaysPlayed(player1), 2);
        assertEq(checkin.getDaysPlayed(player2), 1);
    }

    // ── getNumPlayers ─────────────────────────────────────────────────────────

    function test_GetNumPlayers_ZeroLength_ReturnsEmptyArray() public view {
        uint256[] memory result = checkin.getNumPlayers(10, 0);
        assertEq(result.length, 0);
    }

    function test_GetNumPlayers_CorrectCountsPerDay() public {
        // day 10: player1 + player2; day 11: player1 only; day 12: nobody
        vm.warp(noonOn(10));
        vm.prank(player1); checkin.checkIn(10);
        vm.prank(player2); checkin.checkIn(10);

        vm.warp(noonOn(11));
        vm.prank(player1); checkin.checkIn(11);

        uint256[] memory result = checkin.getNumPlayers(10, 3);
        assertEq(result.length, 3);
        assertEq(result[0], 2); // day 10
        assertEq(result[1], 1); // day 11
        assertEq(result[2], 0); // day 12
    }

    function test_GetNumPlayers_CapsAtFiveYearsOfDays() public view {
        uint256[] memory result = checkin.getNumPlayers(0, 9999);
        assertEq(result.length, 1826); // FIVE_YEARS_OF_DAYS
    }

    function test_GetNumPlayers_ExactlyFiveYears_NotCapped() public view {
        uint256[] memory result = checkin.getNumPlayers(0, 1826);
        assertEq(result.length, 1826);
    }

    function test_GetNumPlayers_StartOffset_CorrectSlice() public {
        vm.warp(noonOn(5));
        vm.prank(player1); checkin.checkIn(5);

        vm.warp(noonOn(6));
        vm.prank(player1); checkin.checkIn(6);
        vm.prank(player2); checkin.checkIn(6);

        uint256[] memory result = checkin.getNumPlayers(5, 3);
        assertEq(result[0], 1); // day 5
        assertEq(result[1], 2); // day 6
        assertEq(result[2], 0); // day 7
    }

    // ── getNumSessions ────────────────────────────────────────────────────────

    function test_GetNumSessions_ZeroLength_ReturnsEmptyArray() public view {
        uint256[] memory result = checkin.getNumSessions(10, 0);
        assertEq(result.length, 0);
    }

    function test_GetNumSessions_SessionsExceedPlayers() public {
        uint32 gameDay = 10;
        vm.warp(noonOn(gameDay));

        // player1: 3 sessions; player2: 1 session
        vm.startPrank(player1);
        checkin.checkIn(gameDay);
        checkin.checkIn(gameDay);
        checkin.checkIn(gameDay);
        vm.stopPrank();
        vm.prank(player2); checkin.checkIn(gameDay);

        uint256[] memory sessions = checkin.getNumSessions(gameDay, 2);
        uint256[] memory players  = checkin.getNumPlayers(gameDay, 2);

        assertEq(sessions[0], 4); // 4 total sessions
        assertEq(players[0],  2); // 2 unique players
        assertEq(sessions[1], 0);
        assertEq(players[1],  0);
    }

    function test_GetNumSessions_CapsAtFiveYearsOfDays() public view {
        uint256[] memory result = checkin.getNumSessions(0, 9999);
        assertEq(result.length, 1826);
    }

    // ── getTotalPlayers / getPlayers ──────────────────────────────────────────

    function test_GetTotalPlayers_NoPlayers_ReturnsZero() public view {
        assertEq(checkin.getTotalPlayers(), 0);
    }

    function test_GetTotalPlayers_AfterCheckIns() public {
        vm.warp(noonOn(10));
        vm.prank(player1); checkin.checkIn(10);
        vm.prank(player2); checkin.checkIn(10);
        assertEq(checkin.getTotalPlayers(), 2);
    }

    function test_GetTotalPlayers_MultipleSessionsSamePlayer_CountsOnce() public {
        uint32 gameDay = 10;
        vm.warp(noonOn(gameDay));

        vm.startPrank(player1);
        checkin.checkIn(gameDay);
        checkin.checkIn(gameDay); // second session same day
        vm.stopPrank();

        vm.warp(noonOn(gameDay + 1));
        vm.prank(player1); checkin.checkIn(gameDay + 1); // new day, same player

        assertEq(checkin.getTotalPlayers(), 1);
    }

    function test_GetTotalPlayers_NewPlayerOnEachDay() public {
        vm.warp(noonOn(10));
        vm.prank(player1); checkin.checkIn(10);

        vm.warp(noonOn(11));
        vm.prank(player2); checkin.checkIn(11);

        assertEq(checkin.getTotalPlayers(), 2);
    }

    function test_GetPlayers_ReturnsPlayersInRegistrationOrder() public {
        vm.warp(noonOn(10));
        vm.prank(player1); checkin.checkIn(10);
        vm.prank(player2); checkin.checkIn(10);

        address[] memory players = checkin.getPlayers(0, 10);
        assertEq(players.length, 2);
        assertEq(players[0], player1);
        assertEq(players[1], player2);
    }

    function test_GetPlayers_StartIndexBeyondEnd_ReturnsEmpty() public {
        vm.warp(noonOn(10));
        vm.prank(player1); checkin.checkIn(10);

        address[] memory players = checkin.getPlayers(5, 10);
        assertEq(players.length, 0);
    }

    function test_GetPlayers_CountExceedsAvailable_ClampsToAvailable() public {
        vm.warp(noonOn(10));
        vm.prank(player1); checkin.checkIn(10);
        vm.prank(player2); checkin.checkIn(10);

        // Ask for 100 but only 2 exist
        address[] memory players = checkin.getPlayers(0, 100);
        assertEq(players.length, 2);
    }

    function test_GetPlayers_Pagination() public {
        address player3 = makeAddr("player3");
        address player4 = makeAddr("player4");
        vm.warp(noonOn(10));
        vm.prank(player1); checkin.checkIn(10);
        vm.prank(player2); checkin.checkIn(10);
        vm.prank(player3); checkin.checkIn(10);
        vm.prank(player4); checkin.checkIn(10);

        // Page 1
        address[] memory page1 = checkin.getPlayers(0, 2);
        assertEq(page1.length, 2);
        assertEq(page1[0], player1);
        assertEq(page1[1], player2);

        // Page 2
        address[] memory page2 = checkin.getPlayers(2, 2);
        assertEq(page2.length, 2);
        assertEq(page2[0], player3);
        assertEq(page2[1], player4);

        // Page 3 (empty)
        address[] memory page3 = checkin.getPlayers(4, 2);
        assertEq(page3.length, 0);
    }

    function test_GetPlayers_DayZeroPlayer_Included() public {
        vm.warp(GAME_START + MINUS_12);
        vm.prank(player1); checkin.checkIn(0);

        address[] memory players = checkin.getPlayers(0, 10);
        assertEq(players.length, 1);
        assertEq(players[0], player1);
    }

    function test_GetPlayers_ZeroCount_ReturnsEmpty() public {
        vm.warp(noonOn(10));
        vm.prank(player1); checkin.checkIn(10);

        address[] memory players = checkin.getPlayers(0, 0);
        assertEq(players.length, 0);
    }

    // ── owner ─────────────────────────────────────────────────────────────────

    function test_Owner_ReturnsOwnerRoleMember() public view {
        assertEq(checkin.owner(), ownerAddr);
    }

    function test_Owner_NoMembers_ReturnsAddressZero() public {
        vm.startPrank(admin);
        checkin.revokeRole(checkin.OWNER_ROLE(), ownerAddr);
        vm.stopPrank();
        assertEq(checkin.owner(), address(0));
    }

    function test_Owner_AfterRoleTransfer() public {
        address newOwner = makeAddr("newOwner");
        vm.startPrank(admin);
        checkin.grantRole(checkin.OWNER_ROLE(), newOwner);
        checkin.revokeRole(checkin.OWNER_ROLE(), ownerAddr);
        vm.stopPrank();
        assertEq(checkin.owner(), newOwner);
    }

    // ── upgradeStorage ────────────────────────────────────────────────────────

    function test_UpgradeStorage_AlwaysReverts() public {
        vm.expectRevert(abi.encodeWithSelector(
            WorcadianCheckInV1.CanNotUpgradeToLowerOrSameVersion.selector, uint256(1)
        ));
        checkin.upgradeStorage("");
    }

    function test_UpgradeStorage_AnyCallerReverts() public {
        vm.prank(stranger);
        vm.expectRevert(abi.encodeWithSelector(
            WorcadianCheckInV1.CanNotUpgradeToLowerOrSameVersion.selector, uint256(1)
        ));
        checkin.upgradeStorage("");
    }

    // ── _authorizeUpgrade ─────────────────────────────────────────────────────

    function test_Upgrade_WithUpgradeRole_Succeeds() public {
        WorcadianCheckInV1 newImpl = new WorcadianCheckInV1();
        vm.prank(upgradeAdmin);
        checkin.upgradeToAndCall(address(newImpl), "");
    }

    function test_Upgrade_PreservesStorageAfterUpgrade() public {
        // Check in some data before upgrading
        uint32 gameDay = 10;
        vm.warp(noonOn(gameDay));
        vm.prank(player1);
        checkin.checkIn(gameDay);

        // Upgrade to a new implementation
        WorcadianCheckInV1 newImpl = new WorcadianCheckInV1();
        vm.prank(upgradeAdmin);
        checkin.upgradeToAndCall(address(newImpl), "");

        // Storage must be intact
        assertEq(checkin.version(), 1);
        assertEq(checkin.getDaysPlayed(player1), 1);
        assertEq(checkin.numberOfPlayers(gameDay), 1);
        assertTrue(checkin.hasRole(checkin.OWNER_ROLE(), ownerAddr));
    }

    function test_Upgrade_WithoutUpgradeRole_Reverts() public {
        WorcadianCheckInV1 newImpl = new WorcadianCheckInV1();
        vm.prank(stranger);
        vm.expectRevert();
        checkin.upgradeToAndCall(address(newImpl), "");
    }

    function test_Upgrade_ByAdmin_WithoutUpgradeRole_Reverts() public {
        // DEFAULT_ADMIN_ROLE does not grant upgrade rights
        WorcadianCheckInV1 newImpl = new WorcadianCheckInV1();
        vm.prank(admin);
        vm.expectRevert();
        checkin.upgradeToAndCall(address(newImpl), "");
    }
}
