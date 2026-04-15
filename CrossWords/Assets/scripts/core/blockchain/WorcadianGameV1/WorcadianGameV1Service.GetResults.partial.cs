// Copyright (c) Whatgame Studios 2024 - 2026
// Partial extension for WorcadianGameV2.getResults (not in generated V1 service yet).
using System.Threading.Tasks;
using CrossWords.WorcadianGameV1.ContractDefinition;
using Nethereum.RPC.Eth.DTOs;

namespace CrossWords.WorcadianGameV1 {

    public partial class WorcadianGameV1ServiceBase {
        public virtual Task<GetResultsOutputDTO> GetResultsQueryAsync(
            GetResultsFunction getResultsFunction,
            BlockParameter blockParameter = null
        ) {
            return ContractHandler.QueryDeserializingToObjectAsync<GetResultsFunction, GetResultsOutputDTO>(
                getResultsFunction,
                blockParameter
            );
        }

        public virtual Task<GetResultsOutputDTO> GetResultsQueryAsync(uint gameDay, BlockParameter blockParameter = null) {
            var f = new GetResultsFunction { GameDay = gameDay };
            return ContractHandler.QueryDeserializingToObjectAsync<GetResultsFunction, GetResultsOutputDTO>(f, blockParameter);
        }
    }
}
