// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {Test} from "forge-std/Test.sol";
import {AnalyseBoard} from "../src/AnalyseBoard.sol";

/**
 * Concrete harness to expose AnalyseBoard's internal functions for testing.
 */
contract AnalyseBoardHarness is AnalyseBoard {
    function exposedAnalyse(string memory _board) external pure returns(string[] memory){
        return analyse(_board);
    }
}

contract AnalyseTest is Test {
    AnalyseBoardHarness harness;


    function setUp() public {
        harness = new AnalyseBoardHarness();
    }

    function test_Scenario1() public view {
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

    function test_Scenario2() public view {
        string memory board = string.concat(
            "      Z    ",
            "     WACK  ", 
            "       H   ", 
            "   BF GIMP ", 
            "           ",
            "    ONLY   ", 
            "  J        ",
            " QUET      ",
            "  D        ",
            "           ",
            "           ");
        string[] memory expected = new string[](1);
        expected[0] = "ONLY";
        string[] memory res = harness.exposedAnalyse(board);
        check(res, expected);
    }

    function test_Scenario3() public view {
        string memory board = string.concat(
            "A     Z    ",
            "B    WACK  ", 
            "C      H   ", 
            "D  BF GIMP ", 
            "E          ",
            "ABCDONLYEFG", 
            "F          ",
            "G UET      ",
            "H D        ",
            "I          ",
            "J          ");
        string[] memory expected = new string[](2);
        expected[0] = "ABCDONLYEFG";
        expected[1] = "ABCDEAFGHIJ";
        string[] memory res = harness.exposedAnalyse(board);
        check(res, expected);
    }

    function test_Scenario4() public view {
        string memory board = string.concat(
            "A     Z   A",
            "B    WACK B", 
            "C      H  C", 
            "D  BF GIM D", 
            "          E",
            "ABCDONLYEFG", 
            "          F",
            "G UET     G",
            "H D       H",
            "I         I",
            "J         J");
        string[] memory expected = new string[](2);
        expected[0] = "ABCDONLYEFG";
        expected[1] = "ABCDEGFGHIJ";
        string[] memory res = harness.exposedAnalyse(board);
        check(res, expected);
    }

    function test_Scenario5() public view {
        string memory board = string.concat(
            "ABCDEFGHIJK",
            "     A     ", 
            "C    B H   ", 
            "D  B C IM  ", 
            "     D     ",
            "ABC ONLY   ", 
            "     E     ",
            "G UE F     ",
            "H D  G     ",
            "     H     ",
            "MNOPQRSTUVW");
        string[] memory expected = new string[](4);
        expected[0] = "ONLY";
        expected[1] = "FABCDNEFGHR";
        expected[2] = "ABCDEFGHIJK";
        expected[3] = "MNOPQRSTUVW";
        string[] memory res = harness.exposedAnalyse(board);
        check(res, expected);
    }

    function test_Scenario6() public view {
        string memory board = string.concat(
            "AA AA AA AA",
            "A AA AA AAA", 
            "AAA AA AA  ", 
            "  AAA AA AA", 
            "A AA AA AA ",
            "AAAAAAAAAAA", 
            "AA AA AA AA", 
            "A AA AA AAA", 
            " AA AA AA A", 
            "AA AA AA AA", 
            "A AA AA AA ");
        string[] memory res = harness.exposedAnalyse(board);
        assertEq(res.length, 64, "Length mismatch");
    }



    function check(string[] memory _actual, string[] memory _expected) internal pure {
        assertEq(_actual.length, _expected.length, "Length mismatch");

        for (uint256 i = 0; i < _expected.length; i++) {
            //console.log("Actual: ", _actual[i], "Expected: ", _expected[i]);
            assertEq(_actual[i], _expected[i], "Did not match");
        }
    }
}
