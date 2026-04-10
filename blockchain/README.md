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

