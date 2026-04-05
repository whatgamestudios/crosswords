// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System;
using System.Collections;
using System.Numerics;
using System.Threading.Tasks;
using System.Text;

using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.Encoders;
using Nethereum.ABI;

using Immutable.Passport;
using Immutable.Passport.Model;


namespace CrossWords {

    public class FourteenNumbersSolutionsContract : FourteenNumbersContract {

        private FourteenNumbersSolutionsService service;

        public const string FOURTEEN_NUMBERS_SOLUTIONS_CONTRACT = "0xe2E762770156FfE253C49Da6E008b4bECCCf2812";

        public FourteenNumbersSolutionsContract() : base(FOURTEEN_NUMBERS_SOLUTIONS_CONTRACT) {
            var web3 = new Web3(RPC_URL);
            service = new FourteenNumbersSolutionsService(web3, contractAddress);
        }


        public async Task<uint> GetBestScore(uint gameDay) {
            SolutionsOutputDTO solution = await service.SolutionsQueryAsync(gameDay);
            BigInteger bestScoreBigInt = solution.Points;
            if (bestScoreBigInt < 0 || bestScoreBigInt > uint.MaxValue) {
                AuditLog.Log($"ERROR: Number {bestScoreBigInt} is outside uint range");
                // Use 7 to indicate an error.
                return 7;
            }
            else {
                return (uint) solution.Points;
            }
        }

        public async Task<SolutionsOutputDTO> GetSolution(uint gameDay) {
            return await service.SolutionsQueryAsync(gameDay);
        }

        public async Task<GetAllSolutionsOutputDTO> GetAllSolutions(uint gameDay) {
            return await service.GetAllSolutionsQueryAsync(gameDay);
        }

        public async Task<uint> GetDaysPlayed(string address) {
            StatsOutputDTO stats = await service.StatsQueryAsync(address);
            BigInteger daysPlayedInt = stats.DaysPlayed;
            if (daysPlayedInt < 0 || daysPlayedInt > uint.MaxValue) {
                AuditLog.Log($"ERROR: Number {daysPlayedInt} is outside uint range");
                // Use 7 to indicate an error.
                return 7;
            }
            else {
                return (uint) stats.DaysPlayed;
            }
        }


        public async Task<bool> SubmitBestScore(uint gameDay, string sol1, string sol2, string sol3) {
            StoreResultsFunction func = new StoreResultsFunction() {
                GameDay = gameDay,
                Sol1 = Encoding.UTF8.GetBytes(sol1),
                Sol2 = Encoding.UTF8.GetBytes(sol2),
                Sol3 = Encoding.UTF8.GetBytes(sol3),
                Store = false,
            };
            var (success, _) = await executeTransaction(func.GetCallData(), 1);
            return success;
        }

        public async Task<bool> SubmitCheckIn(uint gameDay) {
            var func = new CheckInFunction() {
                GameDay = gameDay,
            };
            var (success, _) = await executeTransaction(func.GetCallData());
            return success;
        }
    }
}



