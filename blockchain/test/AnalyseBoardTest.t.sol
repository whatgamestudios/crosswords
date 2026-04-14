// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {Test} from "forge-std/Test.sol";
import {AnalyseBoard} from "../src/AnalyseBoard.sol";

/**
 * Concrete harness to expose AnalyseBoard's internal functions for testing.
 */
contract AnalyseBoardHarness is AnalyseBoard {
    function exposedAnalyse(string memory _board) external view returns(string[] memory){
        return analyse(_board);
    }
}

contract AnalyseTest is Test {
    AnalyseBoardHarness harness;


    function setUp() public {
        harness = new AnalyseBoardHarness();
    }

    function test_Scenario1() public {
        string memory board = string.concat(
            "      Z    ",
            "     WACK  ", 
            "       H   ", 
            "   BF GIMP ", 
            "    R  V   ",
            "    ONLY   ", 
            "  J S X    ",
            " QUET      ",
            "  D        ",
            "           ",
            "           ");
        string[] memory expected = new string[](10);
        expected[0] = "ONLY";
        expected[1] = "FROST";
        expected[2] = "LX";
        expected[3] = "CHIVY";
        expected[4] = "BF";
        expected[5] = "QUET";
        expected[6] = "WACK";
        expected[7] = "GIMP";
        expected[8] = "JUD";
        expected[9] = "ZA";
        string[] memory res = harness.exposedAnalyse(board);
        check(res, expected);
    }

    function check(string[] memory _actual, string[] memory _expected) internal {
        assertEq(_actual.length, _expected.length, "Length mismatch");

        for (uint256 i = 0; i < _expected.length; i++) {
            //console.log("Actual: ", _actual[i], "Expected: ", _expected[i]);
            assertEq(_actual[i], _expected[i], "Did not match");
        }
    }
}
