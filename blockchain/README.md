## Worcadian's Contracts

This directory contains contract code used as part of Wordcadian.

### Build and test:

```
forge build
forge test -vvv
forge coverage
```

To format the code:

```
forge fmt
```

### Deployment:

Rename '.env.example' to '.env' and fill out values. The values are:

```
export DEPLOYER_ADDRESS=<address>
export BLOCKSCOUT_APIKEY=<blockscout API key>
export PRIVATE_KEY=<private key if used>
export LEDGER_HD_PATH="m/44'/60'/0'/0/1"
export USE_LEDGER=<0 for private key, 1 for ledger>
export USE_MAINNETuseMainNet=<1 for mainnet, 0 for testnet>
```

Then execute the script:

```
sh script/deployCheckInV1.sh  
```

### Deployed Addresses

*WorcadianCheckIn*:

|Contract                | Address                   |
|------------------------|---------------------------|
| CheckIn Proxy.         | [0x7e70b51a3090753593aaafedf26536ed2cbc26e8](https://explorer.immutable.com/address/0x7e70b51a3090753593aaafedf26536ed2cbc26e8) |
| WorcadianCheckInV1     | [0xdde657951a32e75a0e11a7763c029202fe314a50](https://explorer.immutable.com/address/0xdde657951a32e75a0e11a7763c029202fe314a50) |
| WorcadianCheckInV2     | [0xeb6223ed5e8b1e815a70e64f587fae4e6733a2c3](https://explorer.immutable.com/address/0xeb6223ed5e8b1e815a70e64f587fae4e6733a2c3) |
| WordList Proxy         | [0x6b17A8D0eD442a234df46342B52C49f4247e119d](https://explorer.immutable.com/address/0x6b17A8D0eD442a234df46342B52C49f4247e119d) |
| WorcadianWordListV1    | [0x8cC43512a60CAF3a19930D3DeA4db8A2F355Ab9d](https://explorer.immutable.com/address/0x8cC43512a60CAF3a19930D3DeA4db8A2F355Ab9d) |
| Seed Word Proxy         | [0xaf14a6EC2a10E7c193e3ef0762Ca320267A6571D](https://explorer.immutable.com/address/0xaf14a6EC2a10E7c193e3ef0762Ca320267A6571D) |
| WordListSeed            | [0x5Cbdc8858770643c54F0AaFe55B509b1aDe4aAAE](https://explorer.immutable.com/address/0x5Cbdc8858770643c54F0AaFe55B509b1aDe4aAAE) |
| WordListSeedV2          | [0x22e6ba1f880ecfdac8c9489811473898d64a64ec](https://explorer.immutable.com/address/0x22e6ba1f880ecfdac8c9489811473898d64a64ec) |
| Game      Proxy         | [0xBe3558861DE7BB699b9a929d1eA5503dCcb329cD](https://explorer.immutable.com/address/0xBe3558861DE7BB699b9a929d1eA5503dCcb329cD) |
| WorcadianGameV1         | [0x438c62f2C1b882A9e08740009525EF0ed9a63b97](https://explorer.immutable.com/address/0x438c62f2C1b882A9e08740009525EF0ed9a63b97) |
| WorcadianGameV2         | [0x22e46e7b4f98f38f1be96f833e488148727f72ad](https://explorer.immutable.com/address/0x22e46e7b4f98f38f1be96f833e488148727f72ad) |


### C# Code Generation:

Following the instructions here: [https://docs.nethereum.com/en/latest/nethereum-codegen-vscodesolidity/#step-2-single-contract](https://docs.nethereum.com/en/latest/nethereum-codegen-vscodesolidity/#step-2-single-contract). That is:

* Build the code using `forge build`.
* Open Visual Studio Code and select `./out/WorcadianCheckInV1.sol/WorcadianCheckInV1.json
* In Visual Studio Code, use Command-Shift-P to open the command pallete, and choose `Solidity: Code generate CSharp from contract definition`.
* Files will be written to `./WorcadianCheckInV1`.


# Lib installation

To install Immutable's contracts:

```
forge install https://github.com/GNSPS/solidity-bytes-utils.git --no-commit
```

