// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.ABI;
using Nethereum.ABI.Encoders;

using CrossWords.WorcadianGameV1;
using CrossWords.WorcadianGameV1.ContractDefinition;

namespace CrossWords {

    public class GameProcessor : ContractExecution {

        private WorcadianGameV1Service service;

        public const string GAME_CONTRACT = "0xBe3558861DE7BB699b9a929d1eA5503dCcb329cD";

        public GameProcessor() : base(GAME_CONTRACT) {
            var web3 = new Web3(RPC_URL);
            service = new WorcadianGameV1Service(web3, contractAddress);
        }

        public async Task<bool> SubmitBoard(uint gameDay, int score, string board) {
            var func = new SubmitBoardFunction() {
                GameDay = gameDay,
                Score = (BigInteger)score,
                Board = board,
            };
            var (success, _) = await executeTransaction(func.GetCallData());
            return success;
        }

        public async Task<List<Submission>> GetSubmissions(uint gameDay, int score, int startIndex, int count) {
            GetSubmissionsOutputDTO dto = await service.GetSubmissionsQueryAsync(gameDay, (BigInteger)score, (BigInteger)startIndex, (BigInteger)count);
            return dto.Result ?? new List<Submission>();
        }

        public async Task<BigInteger> GetSubmissionsCountAtScore(uint gameDay, BigInteger score) {
            return await service.GetSubmissionCountAtScoreQueryAsync(gameDay, score);
        }

        public async Task<List<Submission>> GetBestSubmissions(uint gameDay) {
            GetBestSubmissionsOutputDTO dto = await service.GetBestSubmissionsQueryAsync(gameDay);
            return dto.ReturnValue1 ?? new List<Submission>();
        }

        public async Task<uint> BestScoreByDay(uint gameDay) {
            BigInteger val = await service.BestScoreByDayQueryAsync(gameDay);
            if (val < 0 || val > uint.MaxValue) {
                AuditLog.Log($"ERROR: BestScoreByDay: value {val} is outside uint range");
                // Use an illegal value to indicate an error.
                return 53;
            }
            else {
                return (uint) val;
            }
        }


        public async Task<BigInteger> SubmissionCount(uint gameDay) {
            return await service.SubmissionCountQueryAsync(gameDay);
        }

        /// WorcadianGameV2 <c>getResults</c>: total submissions for the day, best score, and all submissions at that score.
        /// Requires the game proxy to be upgraded to V2+.
        public async Task<GetResultsOutputDTO> GetResults(uint gameDay) {
            return await service.GetResultsQueryAsync(gameDay);
        }
    }
}
