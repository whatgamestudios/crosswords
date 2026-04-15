// Copyright (c) Whatgame Studios 2024 - 2026
// WorcadianGameV2.getResults — hand-maintained alongside generated V1 ABI until codegen includes V2.
using System.Collections.Generic;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Contracts.CQS;

namespace CrossWords.WorcadianGameV1.ContractDefinition {

    public partial class GetResultsFunction : GetResultsFunctionBase { }

    [Function("getResults", typeof(GetResultsOutputDTO))]
    public class GetResultsFunctionBase : FunctionMessage {
        [Parameter("uint32", "_gameDay", 1)]
        public virtual uint GameDay { get; set; }
    }

    public partial class GetResultsOutputDTO : GetResultsOutputDTOBase { }

    [FunctionOutput]
    public class GetResultsOutputDTOBase : IFunctionOutputDTO {
        [Parameter("uint256", "numSubmissions", 1)]
        public virtual BigInteger NumSubmissions { get; set; }

        [Parameter("uint256", "bestScore", 2)]
        public virtual BigInteger BestScore { get; set; }

        [Parameter("tuple[]", "submissions", 3)]
        public virtual List<Submission> Submissions { get; set; }
    }
}
