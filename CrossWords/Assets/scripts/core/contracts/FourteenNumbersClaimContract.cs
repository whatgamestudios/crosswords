// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Linq;
using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.Encoders;
using Nethereum.ABI;

using Immutable.Passport;
using Immutable.Passport.Model;

namespace CrossWords {

    public class FourteenNumbersClaimContract : FourteenNumbersContract {
        private FourteenNumbersClaimService service;
        public const string FOURTEEN_NUMBERS_CLAIM_CONTRACT = "0xb427336d725943BA4300EEC219587E207ad21146";

        public FourteenNumbersClaimContract() : base(FOURTEEN_NUMBERS_CLAIM_CONTRACT) {
            var web3 = new Web3(RPC_URL);
            service = new FourteenNumbersClaimService(web3, contractAddress);
        }

        public async Task<uint> GetDaysClaimed(string address) {
            ClaimedDayOutputDTO claimedDaysObj = await service.ClaimedDayQueryAsync(address);
            BigInteger daysClaimedInt = claimedDaysObj.DaysClaimed;
            if (daysClaimedInt < 0 || daysClaimedInt > uint.MaxValue) {
                AuditLog.Log($"ERROR: DaysClaimedInt {daysClaimedInt} is outside uint range");
                // Use 7 to indicate an error.
                return 7;
            }
            else {
                return (uint) claimedDaysObj.DaysClaimed;
            }
        }

        public async Task<List<ClaimableTokenDTO>> GetClaimableNfts() {
            try {
                var func = new GetClaimableNftsFunction();
                var result = await service.GetClaimableNftsQueryAsync(func);
                return result.ClaimableTokens;
            }
            catch (Exception ex) {
                AuditLog.Log($"ERROR: Error getting claimable NFTs: {ex.Message}");
                return new List<ClaimableTokenDTO>();
            }
        }


        // This function is no longer used in the claim contract.
        public async Task<bool> PrepareForClaim(int salt) {
            var func = new PrepareForClaimFunction() {
                Salt = new BigInteger(salt)
            };
            var (success, _) = await executeTransaction(func.GetCallData());
            return success;
        }

        public async Task<(bool success, BigInteger tokenId)> Claim() {
            int salt = 0;
            var func = new ClaimFunction() {
                Salt = new BigInteger(salt)
            };
            var (success, response) = await executeTransaction(func.GetCallData());
            if (!success) {
                return (false, BigInteger.Zero);
            }

            var tokenId = await GetClaimEventTokenId(response);
            if (tokenId == BigInteger.Zero) {
                AuditLog.Log("No Claim event found in transaction receipt");
                return (false, BigInteger.Zero);
            }

            return (true, tokenId);
        }

        public async Task<BigInteger> GetClaimEventTokenId(TransactionReceiptResponse response) {
            if (response == null) {
                AuditLog.Log("Transaction response is null");
                return BigInteger.Zero;
            }

            try {
                var web3 = new Web3(RPC_URL);
                var fetchedReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(response.hash);
                
                if (fetchedReceipt == null) {
                    AuditLog.Log("Failed to fetch transaction receipt from RPC");
                    return BigInteger.Zero;
                }

                var transferEventOutputs = fetchedReceipt.DecodeAllEvents<ClaimEventDTO>();
                if (transferEventOutputs == null) {
                    AuditLog.Log("Transaction logs are null");
                    return BigInteger.Zero;
                }
                if (transferEventOutputs.Count == 0) {
                    AuditLog.Log("No claim logs found in transaction");
                    return BigInteger.Zero;
                }
                var transferEventOutput = transferEventOutputs[0];
                return transferEventOutput.Event.TokenId;
            }
            catch (Exception ex) {
                AuditLog.Log($"Error processing Claim event: {ex.Message}");
                return BigInteger.Zero;
            }
        }
    }
} 