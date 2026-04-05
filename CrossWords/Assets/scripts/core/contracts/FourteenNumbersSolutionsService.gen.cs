using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;

namespace CrossWords {
    public partial class FourteenNumbersSolutionsService: FourteenNumbersSolutionsServiceBase {
        public FourteenNumbersSolutionsService(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress) {
        }
    }


    public partial class FourteenNumbersSolutionsServiceBase: ContractWeb3ServiceBase {
        public FourteenNumbersSolutionsServiceBase(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress) {
        }

        public virtual Task<string> CheckInRequestAsync(uint gameDay)
        {
            var checkInFunction = new CheckInFunction();
                checkInFunction.GameDay = gameDay;
            
             return ContractHandler.SendRequestAsync(checkInFunction);
        }

        public virtual Task<TransactionReceipt> CheckInRequestAndWaitForReceiptAsync(uint gameDay, CancellationTokenSource cancellationToken = null)
        {
            var checkInFunction = new CheckInFunction();
                checkInFunction.GameDay = gameDay;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(checkInFunction, cancellationToken);
        }


        public virtual Task<SolutionsOutputDTO> SolutionsQueryAsync(SolutionsFunction solutionsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<SolutionsFunction, SolutionsOutputDTO>(solutionsFunction, blockParameter);
        }

        public virtual Task<SolutionsOutputDTO> SolutionsQueryAsync(BigInteger gameDay, BlockParameter blockParameter = null)
        {
            var solutionsFunction = new SolutionsFunction();
                solutionsFunction.GameDay = gameDay;
            
            return ContractHandler.QueryDeserializingToObjectAsync<SolutionsFunction, SolutionsOutputDTO>(solutionsFunction, blockParameter);
        }

        public virtual Task<GetAllSolutionsOutputDTO> GetAllSolutionsQueryAsync(BigInteger gameDay, BlockParameter blockParameter = null)
        {
            var solutionsFunction = new GetAllSolutionsFunction();
                solutionsFunction.GameDay = gameDay;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetAllSolutionsFunction, GetAllSolutionsOutputDTO>(solutionsFunction, blockParameter);
        }

        public virtual Task<StatsOutputDTO> StatsQueryAsync(StatsFunction statsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<StatsFunction, StatsOutputDTO>(statsFunction, blockParameter);
        }

        public virtual Task<StatsOutputDTO> StatsQueryAsync(string returnValue1, BlockParameter blockParameter = null)
        {
            var statsFunction = new StatsFunction();
                statsFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryDeserializingToObjectAsync<StatsFunction, StatsOutputDTO>(statsFunction, blockParameter);
        }

        public virtual Task<string> StoreResultsRequestAsync(StoreResultsFunction storeResultsFunction)
        {
             return ContractHandler.SendRequestAsync(storeResultsFunction);
        }

        public virtual Task<TransactionReceipt> StoreResultsRequestAndWaitForReceiptAsync(StoreResultsFunction storeResultsFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(storeResultsFunction, cancellationToken);
        }

        public virtual Task<string> StoreResultsRequestAsync(uint gameDay, byte[] sol1, byte[] sol2, byte[] sol3, bool store)
        {
            var storeResultsFunction = new StoreResultsFunction();
                storeResultsFunction.GameDay = gameDay;
                storeResultsFunction.Sol1 = sol1;
                storeResultsFunction.Sol2 = sol2;
                storeResultsFunction.Sol3 = sol3;
                storeResultsFunction.Store = store;
            
             return ContractHandler.SendRequestAsync(storeResultsFunction);
        }

        public virtual Task<TransactionReceipt> StoreResultsRequestAndWaitForReceiptAsync(uint gameDay, byte[] sol1, byte[] sol2, byte[] sol3, bool store, CancellationTokenSource cancellationToken = null)
        {
            var storeResultsFunction = new StoreResultsFunction();
                storeResultsFunction.GameDay = gameDay;
                storeResultsFunction.Sol1 = sol1;
                storeResultsFunction.Sol2 = sol2;
                storeResultsFunction.Sol3 = sol3;
                storeResultsFunction.Store = store;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(storeResultsFunction, cancellationToken);
        }

        public Task<BigInteger> VersionQueryAsync(VersionFunction versionFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VersionFunction, BigInteger>(versionFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> VersionQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VersionFunction, BigInteger>(null, blockParameter);
        }

        public override List<Type> GetAllFunctionTypes()
        {
            return new List<Type>
            {
                typeof(DefaultAdminRoleFunction),
                typeof(OwnerRoleFunction),
                typeof(UpgradeInterfaceVersionFunction),
                typeof(UpgradeRoleFunction),
                typeof(CalcFunction),
                typeof(CalcPointsFunction),
                typeof(CalcPointsSingleFunction),
                typeof(CheckInFunction),
                typeof(DetermineCurrentGameDaysFunction),
                typeof(GetRoleAdminFunction),
                typeof(GetRoleMemberFunction),
                typeof(GetRoleMemberCountFunction),
                typeof(GetRoleMembersFunction),
                typeof(GetTargetValueFunction),
                typeof(GrantRoleFunction),
                typeof(HasRoleFunction),
                typeof(InitializeFunction),
                typeof(OwnerFunction),
                typeof(ProxiableUUIDFunction),
                typeof(RenounceRoleFunction),
                typeof(RevokeRoleFunction),
                typeof(SolutionsFunction),
                typeof(StatsFunction),
                typeof(StoreResultsFunction),
                typeof(SupportsInterfaceFunction),
                typeof(UpgradeStorageFunction),
                typeof(UpgradeToAndCallFunction),
                typeof(VersionFunction)
            };
        }

        public override List<Type> GetAllEventTypes()
        {
            return new List<Type>
            {
                typeof(CongratulationsEventDTO),
                typeof(InitializedEventDTO),
                typeof(NextTimeEventDTO),
                typeof(RoleAdminChangedEventDTO),
                typeof(RoleGrantedEventDTO),
                typeof(RoleRevokedEventDTO),
                typeof(UpgradedEventDTO)
            };
        }

        public override List<Type> GetAllErrorTypes()
        {
            return new List<Type>
            {
                typeof(AccessControlBadConfirmationError),
                typeof(AccessControlUnauthorizedAccountError),
                typeof(AddressEmptyCodeError),
                typeof(CanNotUpgradeToLowerOrSameVersionError),
                typeof(DivideByZeroError),
                typeof(ERC1967InvalidImplementationError),
                typeof(ERC1967NonPayableError),
                typeof(EmptyInputError),
                typeof(EndedOnInvalidCharacterError),
                typeof(FailedCallError),
                typeof(GameDayInvalidError),
                typeof(InvalidInitializationError),
                typeof(InvalidNumber1Error),
                typeof(InvalidNumber3Error),
                typeof(InvalidNumber4Error),
                typeof(InvalidStartError),
                typeof(LeadingZeroError),
                typeof(LeftBracketAfterNumericError),
                typeof(LessThanZeroError),
                typeof(NoMatchingRightBracketError),
                typeof(NotDivisibleError),
                typeof(NotInitializingError),
                typeof(NumberPreviouslyUsedError),
                typeof(NumbersRepeatedError),
                typeof(OperationAfterNonNumericError),
                typeof(RightBracketAfterNonNumericError),
                typeof(RightBracketBeforeLeftError),
                typeof(TooManyLeftBracketsError),
                typeof(TooManyNumbersError),
                typeof(UUPSUnauthorizedCallContextError),
                typeof(UUPSUnsupportedProxiableUUIDError),
                typeof(UnknownSymbolError)
            };
        }
    }
}
