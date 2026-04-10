// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import "forge-std/Script.sol";
import "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";
import {WorcadianCheckInV1} from "../src/WorcadianCheckInV1.sol";

contract WorcadianScript is Script {
    function deployV1() public {
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
}
