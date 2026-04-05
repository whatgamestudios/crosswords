using System;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections.Generic;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;

namespace CrossWords {
    public partial class FourteenNumbersClaimService: FourteenNumbersClaimServiceBase {
        public FourteenNumbersClaimService(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress) {
        }
    }

    public partial class FourteenNumbersClaimServiceBase: ContractWeb3ServiceBase {
        public FourteenNumbersClaimServiceBase(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress) {
        }

        public virtual Task<ClaimedDayOutputDTO> ClaimedDayQueryAsync(ClaimedDayFunction claimedDayFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ClaimedDayFunction, ClaimedDayOutputDTO>(claimedDayFunction, blockParameter);
        }

        public virtual Task<ClaimedDayOutputDTO> ClaimedDayQueryAsync(string account, BlockParameter blockParameter = null)
        {
            var claimedDayFunction = new ClaimedDayFunction();
            claimedDayFunction.Account = account;
            return ContractHandler.QueryAsync<ClaimedDayFunction, ClaimedDayOutputDTO>(claimedDayFunction, blockParameter);
        }

        public virtual Task<GetClaimableNftsOutputDTO> GetClaimableNftsQueryAsync(GetClaimableNftsFunction getClaimableNftsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetClaimableNftsFunction, GetClaimableNftsOutputDTO>(getClaimableNftsFunction, blockParameter);
        }



        public override List<Type> GetAllFunctionTypes()
        {
            return new List<Type>
            {
                typeof(ClaimedDayFunction),
            };
        }

        public override List<Type> GetAllEventTypes()
        {
            return new List<Type>
            {
                typeof(ClaimEventDTO),
            };
        }

        public override List<Type> GetAllErrorTypes()
        {
            return new List<Type>
            {
            };
        }        
    }





    public partial class ClaimedDayFunction : ClaimedDayFunctionBase { }

    [Function("claimedDay", typeof(ClaimedDayOutputDTO))]
    public class ClaimedDayFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string Account { get; set; }
    }

    public partial class ClaimedDayOutputDTO : ClaimedDayOutputDTOBase { }

    [FunctionOutput]
    public class ClaimedDayOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger DaysClaimed { get; set; }
    }

    public partial class PrepareForClaimFunction : PrepareForClaimFunctionBase { }

    [Function("prepareForClaim")]
    public class PrepareForClaimFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "_salt", 1)]
        public virtual BigInteger Salt { get; set; }

    }

    public partial class ClaimFunction : ClaimFunctionBase { }

    [Function("claim")]
    public class ClaimFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "_salt", 1)]
        public virtual BigInteger Salt { get; set; }

    }

    public partial class ClaimEventDTO : ClaimEventDTOBase { }

    [Event("Claimed")]
    public class ClaimEventDTOBase : IEventDTO
    {
        [Parameter("address", "_gamePlayer", 1, false)]
        public virtual string GamePlayer { get; set; }

        [Parameter("address", "_erc1155Contract", 2, false)]
        public virtual string Erc1155Contract { get; set; }

        [Parameter("uint256", "_tokenId", 3, false)]
        public virtual BigInteger TokenId { get; set; }

        [Parameter("uint256", "_daysPlayed", 4, false)]
        public virtual BigInteger DaysPlayed { get; set; }

        [Parameter("uint256", "_claimedSoFar", 5, false)]
        public virtual BigInteger ClaimedSoFar { get; set; }
    }

    public partial class GetClaimableNftsFunction : GetClaimableNftsFunctionBase { }

    [Function("getClaimableNfts", typeof(GetClaimableNftsOutputDTO))]
    public class GetClaimableNftsFunctionBase : FunctionMessage
    {
    }

    public partial class GetClaimableNftsOutputDTO : GetClaimableNftsOutputDTOBase { }

    [FunctionOutput]
    public class GetClaimableNftsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("tuple[]", "", 1)]
        public virtual List<ClaimableTokenDTO> ClaimableTokens { get; set; }
    }

    public class ClaimableTokenDTO
    {
        [Parameter("address", "erc1155Contract", 1)]
        public virtual string Erc1155Contract { get; set; }

        [Parameter("uint256", "tokenId", 2)]
        public virtual BigInteger TokenId { get; set; }

        [Parameter("uint256", "balance", 3)]
        public virtual BigInteger Balance { get; set; }

        [Parameter("uint256", "percentage", 4)]
        public virtual BigInteger Percentage { get; set; }
    }
} 
