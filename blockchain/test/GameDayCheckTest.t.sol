// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {Test} from "forge-std/Test.sol";
import {GameDayCheck} from "../src/GameDayCheck.sol";

/**
 * Concrete harness to expose GameDayCheck's internal functions for testing.
 */
contract GameDayCheckHarness is GameDayCheck {
    function exposedCheckGameDay(uint32 _gameDay) external view {
        checkGameDay(_gameDay);
    }

    function exposedDetermineCurrentGameDays() external view returns (uint32, uint32) {
        return determineCurrentGameDays();
    }
}

contract GameDayCheckTest is Test {
    GameDayCheckHarness harness;

    // Mirror constants from GameDayCheck
    uint256 constant GAME_START = 1743292800; // Monday March 30, 2026 00:00:00 UTC
    uint256 constant DAY = 86400;
    uint256 constant MINUS_12 = 43200; // GMT-12 offset in seconds
    uint256 constant PLUS_14 = 50400; // GMT+14 offset in seconds

    function setUp() public {
        harness = new GameDayCheckHarness();
    }

    // ── determineCurrentGameDays ──────────────────────────────────────────────
    //
    // At timestamp T, for a given game day D:
    //   maxDay = (T + PLUS_14  - GAME_START) / DAY
    //   minDay = (T - MINUS_12 - GAME_START) / DAY
    //
    // At noon UTC on day D (T = GAME_START + D*DAY + DAY/2):
    //   minDay = D, maxDay = D+1
    //
    // Just after midnight UTC on day D (T = GAME_START + D*DAY + 1):
    //   minDay = D-1, maxDay = D

    function test_DetermineCurrentGameDays_NoonOnDay10() public {
        vm.warp(GAME_START + 10 * DAY + DAY / 2);
        (uint32 min, uint32 max) = harness.exposedDetermineCurrentGameDays();
        assertEq(min, 10);
        assertEq(max, 11);
    }

    function test_DetermineCurrentGameDays_JustAfterMidnight() public {
        // 1 second past midnight UTC on day 10: GMT-12 is still on day 9
        vm.warp(GAME_START + 10 * DAY + 1);
        (uint32 min, uint32 max) = harness.exposedDetermineCurrentGameDays();
        assertEq(min, 9);
        assertEq(max, 10);
    }

    function test_DetermineCurrentGameDays_ExactMinBoundary() public {
        // Exact moment GMT-12 transitions into day D: minDay becomes D
        uint32 D = 5;
        vm.warp(GAME_START + uint256(D) * DAY + MINUS_12);
        (uint32 min, uint32 max) = harness.exposedDetermineCurrentGameDays();
        assertEq(min, D);
        assertEq(max, D + 1);
    }

    function test_DetermineCurrentGameDays_ExactMaxBoundary() public {
        // Exact moment GMT+14 transitions into day D+1: maxDay becomes D+1
        uint32 D = 5;
        vm.warp(GAME_START + uint256(D + 1) * DAY - PLUS_14);
        (, uint32 max) = harness.exposedDetermineCurrentGameDays();
        assertEq(max, D + 1);
    }

    function test_DetermineCurrentGameDays_DayZero_FirstValidMoment() public {
        // GAME_START + MINUS_12 is the earliest timestamp without underflow
        vm.warp(GAME_START + MINUS_12);
        (uint32 min, uint32 max) = harness.exposedDetermineCurrentGameDays();
        assertEq(min, 0);
        assertEq(max, 1);
    }

    function test_DetermineCurrentGameDays_Underflow_Reverts() public {
        // One second before the safe window: unsigned subtraction underflows
        vm.warp(GAME_START + MINUS_12 - 1);
        vm.expectRevert();
        harness.exposedDetermineCurrentGameDays();
    }

    function test_DetermineCurrentGameDays_FarFuture() public {
        vm.warp(GAME_START + 1000 * DAY + DAY / 2);
        (uint32 min, uint32 max) = harness.exposedDetermineCurrentGameDays();
        assertEq(min, 1000);
        assertEq(max, 1001);
    }

    function testFuzz_DetermineCurrentGameDays_MinAlwaysLeqMax(uint256 timestamp) public {
        timestamp = bound(timestamp, GAME_START + MINUS_12, GAME_START + 10_000 * DAY);
        vm.warp(timestamp);
        (uint32 min, uint32 max) = harness.exposedDetermineCurrentGameDays();
        assertLe(min, max);
    }

    function testFuzz_DetermineCurrentGameDays_WindowIsAtMostTwoDays(uint256 timestamp) public {
        timestamp = bound(timestamp, GAME_START + MINUS_12, GAME_START + 10_000 * DAY);
        vm.warp(timestamp);
        (uint32 min, uint32 max) = harness.exposedDetermineCurrentGameDays();
        assertLe(max - min, 2);
    }

    // ── checkGameDay ──────────────────────────────────────────────────────────

    function test_CheckGameDay_MinDay_Passes() public {
        vm.warp(GAME_START + 10 * DAY + DAY / 2); // minDay = 10
        harness.exposedCheckGameDay(10);
    }

    function test_CheckGameDay_MaxDay_Passes() public {
        vm.warp(GAME_START + 10 * DAY + DAY / 2); // maxDay = 11
        harness.exposedCheckGameDay(11);
    }

    function test_CheckGameDay_FutureDay_Reverts() public {
        vm.warp(GAME_START + 10 * DAY + DAY / 2); // valid: [10, 11]
        vm.expectRevert(
            abi.encodeWithSelector(GameDayCheck.GameDayInvalid.selector, uint32(12), uint32(10), uint32(11))
        );
        harness.exposedCheckGameDay(12);
    }

    function test_CheckGameDay_PastDay_Reverts() public {
        vm.warp(GAME_START + 10 * DAY + DAY / 2); // valid: [10, 11]
        vm.expectRevert(abi.encodeWithSelector(GameDayCheck.GameDayInvalid.selector, uint32(9), uint32(10), uint32(11)));
        harness.exposedCheckGameDay(9);
    }

    function test_CheckGameDay_DayZero_Passes() public {
        vm.warp(GAME_START + MINUS_12); // earliest moment day 0 is valid
        harness.exposedCheckGameDay(0);
    }

    function testFuzz_CheckGameDay_AnyDayInValidRange_NeverReverts(uint256 timestamp, uint8 offset) public {
        timestamp = bound(timestamp, GAME_START + MINUS_12, GAME_START + 10_000 * DAY);
        vm.warp(timestamp);
        (uint32 min, uint32 max) = harness.exposedDetermineCurrentGameDays();
        // Pick any day within [min, max]
        uint32 range = max - min + 1; // always 1 or 2 within the safe range
        uint32 day = min + (uint32(offset) % range);
        harness.exposedCheckGameDay(day); // must not revert
    }

    function testFuzz_CheckGameDay_DayBeyondMax_AlwaysReverts(uint256 timestamp, uint16 excess) public {
        timestamp = bound(timestamp, GAME_START + MINUS_12, GAME_START + 10_000 * DAY);
        excess = uint16(bound(excess, 1, 1000));
        vm.warp(timestamp);
        (, uint32 max) = harness.exposedDetermineCurrentGameDays();
        vm.expectRevert();
        harness.exposedCheckGameDay(max + uint32(excess));
    }
}
