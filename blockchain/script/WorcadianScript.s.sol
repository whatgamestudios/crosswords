// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import "forge-std/Script.sol";
import "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";
import {WorcadianCheckInV1} from "../src/WorcadianCheckInV1.sol";
import {WorcadianCheckInV2} from "../src/WorcadianCheckInV2.sol";
import {WorcadianWordListV1} from "../src/WorcadianWordListV1.sol";
import {WorcadianWordListV2} from "../src/WorcadianWordListV2.sol";
import {WordListSeed} from "../src/WordListSeed.sol";
import {WordListSeedV2} from "../src/WordListSeedV2.sol";
import {WorcadianGameV1} from "../src/WorcadianGameV1.sol";
import {WorcadianGameV2} from "../src/WorcadianGameV2.sol";

contract WorcadianScript is Script {
    function deployCheckInV1() public {
        address deployer = vm.envAddress("DEPLOYER_ADDRESS");
        address roleAdmin = deployer;
        address upgradeAdmin = deployer;
        address owner = deployer;

        vm.broadcast();
        WorcadianCheckInV1 impl = new WorcadianCheckInV1();
        bytes memory initData =
            abi.encodeWithSelector(WorcadianCheckInV1.initialize.selector, roleAdmin, owner, upgradeAdmin);

        vm.broadcast();
        ERC1967Proxy proxy = new ERC1967Proxy(address(impl), initData);

        console.log("Proxy address: ", address(proxy));
        console.log("Implementation address: ", address(impl));
    }

    function deployCheckInV2() public {
        vm.broadcast();
        WorcadianCheckInV2 implV2 = new WorcadianCheckInV2();

        WorcadianCheckInV2 proxy = WorcadianCheckInV2(address(0x7E70b51A3090753593AAAfEdf26536ed2cbC26e8));
        vm.broadcast();
        proxy.upgradeToAndCall(address(implV2), 
            abi.encodeWithSelector(WorcadianCheckInV2.upgradeStorage.selector, bytes("")));

        console.log("Implementation V2 address: ", address(implV2));
    }

    /**
     * @notice Deploy WorcadianWordListV1, WordListSeed, and WorcadianGameV1 together
     *         with their ERC-1967 proxy contracts.
     *
     * Required environment variables:
     *   DEPLOYER_ADDRESS  – tx sender; receives all admin / owner / upgrade roles.
     *
     * Deployment order:
     *   1. WorcadianWordListV1  implementation & proxy
     *   2. WordListSeed         implementation & proxy
     *   3. WorcadianGameV1      implementation & proxy
     */
    function deployGame() public {
        address deployer = vm.envAddress("DEPLOYER_ADDRESS");
        // A randomly selected passport wallet on mainnet.
        address passportWallet = 0xDa77D416bb4238c9424b8d27A7f90fA2Bdf4911E;
        address roleAdmin = deployer;
        address owner = deployer;
        address upgradeAdmin = deployer;
        address wordSmithAdmin = deployer;

        // ── WorcadianWordListV1 ───────────────────────────────────────────────

        vm.broadcast();
        WorcadianWordListV1 wordListImpl = new WorcadianWordListV1();

        vm.broadcast();
        ERC1967Proxy wordListProxy = new ERC1967Proxy(
            address(wordListImpl),
            abi.encodeCall(WorcadianWordListV1.initialize,
                (roleAdmin, owner, upgradeAdmin, wordSmithAdmin))
        );

        console.log("WorcadianWordListV1 implementation:", address(wordListImpl));
        console.log("WorcadianWordListV1 proxy:         ", address(wordListProxy));

        // ── WordListSeed ──────────────────────────────────────────────────────

        vm.broadcast();
        WordListSeed seedImpl = new WordListSeed();

        vm.broadcast();
        ERC1967Proxy seedProxy = new ERC1967Proxy(
            address(seedImpl),
            abi.encodeCall(WordListSeed.initialize,
                (roleAdmin, owner, upgradeAdmin, wordSmithAdmin))
        );

        console.log("WordListSeed implementation:       ", address(seedImpl));
        console.log("WordListSeed proxy:                ", address(seedProxy));

        // ── WorcadianGameV1 ───────────────────────────────────────────────────

        vm.broadcast();
        WorcadianGameV1 gameImpl = new WorcadianGameV1();

        vm.broadcast();
        ERC1967Proxy gameProxy = new ERC1967Proxy(
            address(gameImpl),
            abi.encodeCall(WorcadianGameV1.initialize,
                (roleAdmin, owner, upgradeAdmin,
                 passportWallet, address(seedProxy), address(wordListProxy)))
        );

        console.log("WorcadianGameV1 implementation:    ", address(gameImpl));
        console.log("WorcadianGameV1 proxy:             ", address(gameProxy));
    }

    function deploySeedV2() public {
        vm.broadcast();
        WordListSeedV2 implV2 = new WordListSeedV2();

        WordListSeedV2 proxy = WordListSeedV2(address(0xaf14a6EC2a10E7c193e3ef0762Ca320267A6571D));
        vm.broadcast();
        proxy.upgradeToAndCall(address(implV2), 
            abi.encodeWithSelector(WorcadianCheckInV2.upgradeStorage.selector, bytes("")));

        console.log("Implementation V2 address: ", address(implV2));
    }

    function deployGameV2() public {
        vm.broadcast();
        WorcadianGameV2 implV2 = new WorcadianGameV2();

        WorcadianGameV2 proxy = WorcadianGameV2(address(0xBe3558861DE7BB699b9a929d1eA5503dCcb329cD));
        vm.broadcast();
        proxy.upgradeToAndCall(address(implV2), 
            abi.encodeWithSelector(WorcadianGameV2.upgradeStorage.selector, bytes("")));

        console.log("Implementation V2 address: ", address(implV2));
    }


    function deployWordListV2() public {
        vm.broadcast();
        WorcadianWordListV2 implV2 = new WorcadianWordListV2();

        WorcadianWordListV2 proxy = WorcadianWordListV2(address(0x6b17A8D0eD442a234df46342B52C49f4247e119d));
        vm.broadcast();
        proxy.upgradeToAndCall(address(implV2), 
            abi.encodeWithSelector(WorcadianWordListV2.upgradeStorage.selector, bytes("")));

        console.log("Implementation V2 address: ", address(implV2));
    }

}
