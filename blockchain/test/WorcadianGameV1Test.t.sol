// Copyright (c) Whatgame Studios 2026
// SPDX-License-Identifier: PROPRIETARY
pragma solidity ^0.8.26;

import {Test} from "forge-std/Test.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";
import {WorcadianGameV1} from "../src/WorcadianGameV1.sol";
import {PassportCheck} from "../src/PassportCheck.sol";

/**
 * Minimal stand-in for a Passport Wallet implementation contract.
 * Only needs to have deployed bytecode (so extcodehash is non-zero).
 */
contract MockPassportImpl {
    // No functions needed; presence of bytecode is all that matters.
}

/**
 * Minimal stand-in for a Passport Wallet proxy contract.
 * Must implement PROXY_getImplementation() as the real proxy does.
 * Runtime bytecode is identical for every instance, so all instances
 * share the same extcodehash — mirroring how real passport proxies work.
 */
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

contract WorcadianGameV1Test is Test {
    WorcadianGameV1 game;

    MockPassportImpl   passportImpl;
    MockPassportWallet passportWallet;

    address admin            = makeAddr("admin");
    address ownerAddr        = makeAddr("owner");
    address upgradeAdmin     = makeAddr("upgradeAdmin");
    address wordListSeedAddr = makeAddr("wordListSeed");
    address wordListAddr     = makeAddr("wordList");
    address stranger         = makeAddr("stranger");

    // ── Helpers ───────────────────────────────────────────────────────────────

    function deployProxy(
        address _admin,
        address _owner,
        address _upgradeAdmin,
        address _passportWallet,
        address _wordListSeed,
        address _wordList
    ) internal returns (WorcadianGameV1) {
        WorcadianGameV1 impl = new WorcadianGameV1();
        ERC1967Proxy proxy = new ERC1967Proxy(
            address(impl),
            abi.encodeCall(
                WorcadianGameV1.initialize,
                (_admin, _owner, _upgradeAdmin, _passportWallet, _wordListSeed, _wordList)
            )
        );
        return WorcadianGameV1(address(proxy));
    }

    function setUp() public {
        passportImpl   = new MockPassportImpl();
        passportWallet = new MockPassportWallet(address(passportImpl));
        game = deployProxy(
            admin, ownerAddr, upgradeAdmin,
            address(passportWallet), wordListSeedAddr, wordListAddr
        );
    }

    // ── initialize ────────────────────────────────────────────────────────────

    function test_Initialize_RolesAssigned() public view {
        assertTrue(game.hasRole(game.DEFAULT_ADMIN_ROLE(), admin));
        assertTrue(game.hasRole(game.OWNER_ROLE(), ownerAddr));
        assertTrue(game.hasRole(game.UPGRADE_ROLE(), upgradeAdmin));
    }

    function test_Initialize_VersionIsOne() public view {
        assertEq(game.version(), 1);
    }

    function test_Initialize_AddressesStored() public view {
        assertEq(game.wordListSeed(), wordListSeedAddr);
        assertEq(game.wordList(), wordListAddr);
    }

    function test_Initialize_ZeroAdmin_Reverts() public {
        WorcadianGameV1 impl = new WorcadianGameV1();
        vm.expectRevert(
            abi.encodeWithSelector(
                WorcadianGameV1.BadAddress.selector,
                address(0), ownerAddr, upgradeAdmin, address(passportWallet), wordListSeedAddr, wordListAddr
            )
        );
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WorcadianGameV1.initialize,
                (address(0), ownerAddr, upgradeAdmin, address(passportWallet), wordListSeedAddr, wordListAddr))
        );
    }

    function test_Initialize_ZeroOwner_Reverts() public {
        WorcadianGameV1 impl = new WorcadianGameV1();
        vm.expectRevert(
            abi.encodeWithSelector(
                WorcadianGameV1.BadAddress.selector,
                admin, address(0), upgradeAdmin, address(passportWallet), wordListSeedAddr, wordListAddr
            )
        );
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WorcadianGameV1.initialize,
                (admin, address(0), upgradeAdmin, address(passportWallet), wordListSeedAddr, wordListAddr))
        );
    }

    function test_Initialize_ZeroUpgradeAdmin_Reverts() public {
        WorcadianGameV1 impl = new WorcadianGameV1();
        vm.expectRevert(
            abi.encodeWithSelector(
                WorcadianGameV1.BadAddress.selector,
                admin, ownerAddr, address(0), address(passportWallet), wordListSeedAddr, wordListAddr
            )
        );
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WorcadianGameV1.initialize,
                (admin, ownerAddr, address(0), address(passportWallet), wordListSeedAddr, wordListAddr))
        );
    }

    function test_Initialize_ZeroPassportWallet_Reverts() public {
        WorcadianGameV1 impl = new WorcadianGameV1();
        vm.expectRevert(
            abi.encodeWithSelector(
                WorcadianGameV1.BadAddress.selector,
                admin, ownerAddr, upgradeAdmin, address(0), wordListSeedAddr, wordListAddr
            )
        );
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WorcadianGameV1.initialize,
                (admin, ownerAddr, upgradeAdmin, address(0), wordListSeedAddr, wordListAddr))
        );
    }

    function test_Initialize_ZeroWordListSeed_Reverts() public {
        WorcadianGameV1 impl = new WorcadianGameV1();
        vm.expectRevert(
            abi.encodeWithSelector(
                WorcadianGameV1.BadAddress.selector,
                admin, ownerAddr, upgradeAdmin, address(passportWallet), address(0), wordListAddr
            )
        );
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WorcadianGameV1.initialize,
                (admin, ownerAddr, upgradeAdmin, address(passportWallet), address(0), wordListAddr))
        );
    }

    function test_Initialize_ZeroWordList_Reverts() public {
        WorcadianGameV1 impl = new WorcadianGameV1();
        vm.expectRevert(
            abi.encodeWithSelector(
                WorcadianGameV1.BadAddress.selector,
                admin, ownerAddr, upgradeAdmin, address(passportWallet), wordListSeedAddr, address(0)
            )
        );
        new ERC1967Proxy(
            address(impl),
            abi.encodeCall(WorcadianGameV1.initialize,
                (admin, ownerAddr, upgradeAdmin, address(passportWallet), wordListSeedAddr, address(0)))
        );
    }

    function test_Initialize_CannotReinitialize() public {
        vm.expectRevert();
        game.initialize(admin, ownerAddr, upgradeAdmin, address(passportWallet), wordListSeedAddr, wordListAddr);
    }

    function test_Initialize_ImplementationDirectly_Reverts() public {
        WorcadianGameV1 impl = new WorcadianGameV1();
        vm.expectRevert();
        impl.initialize(admin, ownerAddr, upgradeAdmin, address(passportWallet), wordListSeedAddr, wordListAddr);
    }

    // ── owner ─────────────────────────────────────────────────────────────────

    function test_Owner_ReturnsOwnerRoleMember() public view {
        assertEq(game.owner(), ownerAddr);
    }

    function test_Owner_NoMembers_ReturnsAddressZero() public {
        vm.startPrank(admin);
        game.revokeRole(game.OWNER_ROLE(), ownerAddr);
        vm.stopPrank();
        assertEq(game.owner(), address(0));
    }

    function test_Owner_AfterRoleTransfer() public {
        address newOwner = makeAddr("newOwner");
        vm.startPrank(admin);
        game.grantRole(game.OWNER_ROLE(), newOwner);
        game.revokeRole(game.OWNER_ROLE(), ownerAddr);
        vm.stopPrank();
        assertEq(game.owner(), newOwner);
    }

    // ── isPassport ────────────────────────────────────────────────────────────

    function test_IsPassport_InitialWallet_ReturnsTrue() public view {
        assertTrue(game.isPassport(address(passportWallet)));
    }

    function test_IsPassport_EOA_ReturnsFalse() public view {
        assertFalse(game.isPassport(stranger));
    }

    function test_IsPassport_SameBytecodeAndImpl_ReturnsTrue() public {
        // A second MockPassportWallet pointing to the already-allowlisted impl has
        // the same bytecode hash and the same implementation address, so it is
        // already a passport without any extra call to addPassportWallet.
        MockPassportWallet anotherWallet = new MockPassportWallet(address(passportImpl));
        assertTrue(game.isPassport(address(anotherWallet)));
    }

    function test_IsPassport_UnregisteredImpl_ReturnsFalse() public {
        // Same proxy bytecode but a brand-new implementation address that has
        // never been allowlisted.
        MockPassportImpl newImpl   = new MockPassportImpl();
        MockPassportWallet newWallet = new MockPassportWallet(address(newImpl));
        assertFalse(game.isPassport(address(newWallet)));
    }

    // ── addPassportWallet ─────────────────────────────────────────────────────

    function test_AddPassportWallet_ByOwner_NewImplBecomesPassport() public {
        MockPassportImpl newImpl     = new MockPassportImpl();
        MockPassportWallet newWallet = new MockPassportWallet(address(newImpl));

        assertFalse(game.isPassport(address(newWallet)));

        vm.prank(ownerAddr);
        game.addPassportWallet(address(newWallet));

        assertTrue(game.isPassport(address(newWallet)));
    }

    function test_AddPassportWallet_EmitsEvent() public {
        MockPassportImpl newImpl     = new MockPassportImpl();
        MockPassportWallet newWallet = new MockPassportWallet(address(newImpl));

        address walletAddr = address(newWallet);
        bytes32 codeHash;
        // solhint-disable-next-line no-inline-assembly
        assembly { codeHash := extcodehash(walletAddr) }

        vm.expectEmit(true, true, false, true, address(game));
        emit PassportCheck.WalletAllowlistChanged(codeHash, address(newWallet), true);

        vm.prank(ownerAddr);
        game.addPassportWallet(address(newWallet));
    }

    function test_AddPassportWallet_ByStranger_Reverts() public {
        MockPassportImpl newImpl     = new MockPassportImpl();
        MockPassportWallet newWallet = new MockPassportWallet(address(newImpl));

        vm.prank(stranger);
        vm.expectRevert();
        game.addPassportWallet(address(newWallet));
    }

    function test_AddPassportWallet_ByAdmin_WithoutOwnerRole_Reverts() public {
        // DEFAULT_ADMIN_ROLE does not grant the right to manage the passport allowlist.
        MockPassportImpl newImpl     = new MockPassportImpl();
        MockPassportWallet newWallet = new MockPassportWallet(address(newImpl));

        vm.prank(admin);
        vm.expectRevert();
        game.addPassportWallet(address(newWallet));
    }

    // ── removePassportWallet ──────────────────────────────────────────────────

    function test_RemovePassportWallet_ByOwner_WalletNoLongerPassport() public {
        assertTrue(game.isPassport(address(passportWallet)));

        vm.prank(ownerAddr);
        game.removePassportWallet(address(passportWallet));

        assertFalse(game.isPassport(address(passportWallet)));
    }

    function test_RemovePassportWallet_EmitsEvent() public {
        address walletAddr = address(passportWallet);
        bytes32 codeHash;
        // solhint-disable-next-line no-inline-assembly
        assembly { codeHash := extcodehash(walletAddr) }

        vm.expectEmit(true, true, false, true, address(game));
        emit PassportCheck.WalletAllowlistChanged(codeHash, address(passportWallet), false);

        vm.prank(ownerAddr);
        game.removePassportWallet(address(passportWallet));
    }

    function test_RemovePassportWallet_ByStranger_Reverts() public {
        vm.prank(stranger);
        vm.expectRevert();
        game.removePassportWallet(address(passportWallet));
    }

    function test_RemovePassportWallet_ByAdmin_WithoutOwnerRole_Reverts() public {
        vm.prank(admin);
        vm.expectRevert();
        game.removePassportWallet(address(passportWallet));
    }

    // ── upgradeStorage ────────────────────────────────────────────────────────

    function test_UpgradeStorage_AlwaysReverts() public {
        vm.expectRevert(
            abi.encodeWithSelector(WorcadianGameV1.CanNotUpgradeToLowerOrSameVersion.selector, uint256(1))
        );
        game.upgradeStorage("");
    }

    function test_UpgradeStorage_AnyCallerReverts() public {
        vm.prank(stranger);
        vm.expectRevert(
            abi.encodeWithSelector(WorcadianGameV1.CanNotUpgradeToLowerOrSameVersion.selector, uint256(1))
        );
        game.upgradeStorage("");
    }

    // ── _authorizeUpgrade ─────────────────────────────────────────────────────

    function test_Upgrade_WithUpgradeRole_Succeeds() public {
        WorcadianGameV1 newImpl = new WorcadianGameV1();
        vm.prank(upgradeAdmin);
        game.upgradeToAndCall(address(newImpl), "");
    }

    function test_Upgrade_PreservesStorageAfterUpgrade() public {
        WorcadianGameV1 newImpl = new WorcadianGameV1();
        vm.prank(upgradeAdmin);
        game.upgradeToAndCall(address(newImpl), "");

        assertEq(game.version(), 1);
        assertEq(game.wordListSeed(), wordListSeedAddr);
        assertEq(game.wordList(), wordListAddr);
        assertTrue(game.hasRole(game.OWNER_ROLE(), ownerAddr));
        assertTrue(game.isPassport(address(passportWallet)));
    }

    function test_Upgrade_WithoutUpgradeRole_Reverts() public {
        WorcadianGameV1 newImpl = new WorcadianGameV1();
        vm.prank(stranger);
        vm.expectRevert();
        game.upgradeToAndCall(address(newImpl), "");
    }

    function test_Upgrade_ByAdmin_WithoutUpgradeRole_Reverts() public {
        // DEFAULT_ADMIN_ROLE does not grant upgrade rights.
        WorcadianGameV1 newImpl = new WorcadianGameV1();
        vm.prank(admin);
        vm.expectRevert();
        game.upgradeToAndCall(address(newImpl), "");
    }
}
