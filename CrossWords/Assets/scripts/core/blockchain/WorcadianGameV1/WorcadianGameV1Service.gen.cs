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
using CrossWords.WorcadianGameV1.ContractDefinition;

namespace CrossWords.WorcadianGameV1
{
    public partial class WorcadianGameV1Service: WorcadianGameV1ServiceBase
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.IWeb3 web3, WorcadianGameV1Deployment worcadianGameV1Deployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<WorcadianGameV1Deployment>().SendRequestAndWaitForReceiptAsync(worcadianGameV1Deployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.IWeb3 web3, WorcadianGameV1Deployment worcadianGameV1Deployment)
        {
            return web3.Eth.GetContractDeploymentHandler<WorcadianGameV1Deployment>().SendRequestAsync(worcadianGameV1Deployment);
        }

        public static async Task<WorcadianGameV1Service> DeployContractAndGetServiceAsync(Nethereum.Web3.IWeb3 web3, WorcadianGameV1Deployment worcadianGameV1Deployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, worcadianGameV1Deployment, cancellationTokenSource);
            return new WorcadianGameV1Service(web3, receipt.ContractAddress);
        }

        public WorcadianGameV1Service(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

    }


    public partial class WorcadianGameV1ServiceBase: ContractWeb3ServiceBase
    {

        public WorcadianGameV1ServiceBase(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

        public Task<byte[]> DefaultAdminRoleQueryAsync(DefaultAdminRoleFunction defaultAdminRoleFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DefaultAdminRoleFunction, byte[]>(defaultAdminRoleFunction, blockParameter);
        }

        
        public virtual Task<byte[]> DefaultAdminRoleQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DefaultAdminRoleFunction, byte[]>(null, blockParameter);
        }

        public Task<byte[]> OwnerRoleQueryAsync(OwnerRoleFunction ownerRoleFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerRoleFunction, byte[]>(ownerRoleFunction, blockParameter);
        }

        
        public virtual Task<byte[]> OwnerRoleQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerRoleFunction, byte[]>(null, blockParameter);
        }

        public Task<string> UpgradeInterfaceVersionQueryAsync(UpgradeInterfaceVersionFunction upgradeInterfaceVersionFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<UpgradeInterfaceVersionFunction, string>(upgradeInterfaceVersionFunction, blockParameter);
        }

        
        public virtual Task<string> UpgradeInterfaceVersionQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<UpgradeInterfaceVersionFunction, string>(null, blockParameter);
        }

        public Task<byte[]> UpgradeRoleQueryAsync(UpgradeRoleFunction upgradeRoleFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<UpgradeRoleFunction, byte[]>(upgradeRoleFunction, blockParameter);
        }

        
        public virtual Task<byte[]> UpgradeRoleQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<UpgradeRoleFunction, byte[]>(null, blockParameter);
        }

        public virtual Task<string> AddPassportWalletRequestAsync(AddPassportWalletFunction addPassportWalletFunction)
        {
             return ContractHandler.SendRequestAsync(addPassportWalletFunction);
        }

        public virtual Task<TransactionReceipt> AddPassportWalletRequestAndWaitForReceiptAsync(AddPassportWalletFunction addPassportWalletFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addPassportWalletFunction, cancellationToken);
        }

        public virtual Task<string> AddPassportWalletRequestAsync(string wallet)
        {
            var addPassportWalletFunction = new AddPassportWalletFunction();
                addPassportWalletFunction.Wallet = wallet;
            
             return ContractHandler.SendRequestAsync(addPassportWalletFunction);
        }

        public virtual Task<TransactionReceipt> AddPassportWalletRequestAndWaitForReceiptAsync(string wallet, CancellationTokenSource cancellationToken = null)
        {
            var addPassportWalletFunction = new AddPassportWalletFunction();
                addPassportWalletFunction.Wallet = wallet;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addPassportWalletFunction, cancellationToken);
        }

        public Task<BigInteger> BestScoreByDayQueryAsync(BestScoreByDayFunction bestScoreByDayFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BestScoreByDayFunction, BigInteger>(bestScoreByDayFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> BestScoreByDayQueryAsync(uint returnValue1, BlockParameter blockParameter = null)
        {
            var bestScoreByDayFunction = new BestScoreByDayFunction();
                bestScoreByDayFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<BestScoreByDayFunction, BigInteger>(bestScoreByDayFunction, blockParameter);
        }

        public virtual Task<GetBestSubmissionsOutputDTO> GetBestSubmissionsQueryAsync(GetBestSubmissionsFunction getBestSubmissionsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetBestSubmissionsFunction, GetBestSubmissionsOutputDTO>(getBestSubmissionsFunction, blockParameter);
        }

        public virtual Task<GetBestSubmissionsOutputDTO> GetBestSubmissionsQueryAsync(uint gameDay, BlockParameter blockParameter = null)
        {
            var getBestSubmissionsFunction = new GetBestSubmissionsFunction();
                getBestSubmissionsFunction.GameDay = gameDay;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetBestSubmissionsFunction, GetBestSubmissionsOutputDTO>(getBestSubmissionsFunction, blockParameter);
        }

        public Task<byte[]> GetRoleAdminQueryAsync(GetRoleAdminFunction getRoleAdminFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetRoleAdminFunction, byte[]>(getRoleAdminFunction, blockParameter);
        }

        
        public virtual Task<byte[]> GetRoleAdminQueryAsync(byte[] role, BlockParameter blockParameter = null)
        {
            var getRoleAdminFunction = new GetRoleAdminFunction();
                getRoleAdminFunction.Role = role;
            
            return ContractHandler.QueryAsync<GetRoleAdminFunction, byte[]>(getRoleAdminFunction, blockParameter);
        }

        public Task<string> GetRoleMemberQueryAsync(GetRoleMemberFunction getRoleMemberFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetRoleMemberFunction, string>(getRoleMemberFunction, blockParameter);
        }

        
        public virtual Task<string> GetRoleMemberQueryAsync(byte[] role, BigInteger index, BlockParameter blockParameter = null)
        {
            var getRoleMemberFunction = new GetRoleMemberFunction();
                getRoleMemberFunction.Role = role;
                getRoleMemberFunction.Index = index;
            
            return ContractHandler.QueryAsync<GetRoleMemberFunction, string>(getRoleMemberFunction, blockParameter);
        }

        public Task<BigInteger> GetRoleMemberCountQueryAsync(GetRoleMemberCountFunction getRoleMemberCountFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetRoleMemberCountFunction, BigInteger>(getRoleMemberCountFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> GetRoleMemberCountQueryAsync(byte[] role, BlockParameter blockParameter = null)
        {
            var getRoleMemberCountFunction = new GetRoleMemberCountFunction();
                getRoleMemberCountFunction.Role = role;
            
            return ContractHandler.QueryAsync<GetRoleMemberCountFunction, BigInteger>(getRoleMemberCountFunction, blockParameter);
        }

        public Task<List<string>> GetRoleMembersQueryAsync(GetRoleMembersFunction getRoleMembersFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetRoleMembersFunction, List<string>>(getRoleMembersFunction, blockParameter);
        }

        
        public virtual Task<List<string>> GetRoleMembersQueryAsync(byte[] role, BlockParameter blockParameter = null)
        {
            var getRoleMembersFunction = new GetRoleMembersFunction();
                getRoleMembersFunction.Role = role;
            
            return ContractHandler.QueryAsync<GetRoleMembersFunction, List<string>>(getRoleMembersFunction, blockParameter);
        }

        public Task<BigInteger> GetSubmissionCountAtScoreQueryAsync(GetSubmissionCountAtScoreFunction getSubmissionCountAtScoreFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetSubmissionCountAtScoreFunction, BigInteger>(getSubmissionCountAtScoreFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> GetSubmissionCountAtScoreQueryAsync(uint gameDay, BigInteger score, BlockParameter blockParameter = null)
        {
            var getSubmissionCountAtScoreFunction = new GetSubmissionCountAtScoreFunction();
                getSubmissionCountAtScoreFunction.GameDay = gameDay;
                getSubmissionCountAtScoreFunction.Score = score;
            
            return ContractHandler.QueryAsync<GetSubmissionCountAtScoreFunction, BigInteger>(getSubmissionCountAtScoreFunction, blockParameter);
        }

        public virtual Task<GetSubmissionsOutputDTO> GetSubmissionsQueryAsync(GetSubmissionsFunction getSubmissionsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetSubmissionsFunction, GetSubmissionsOutputDTO>(getSubmissionsFunction, blockParameter);
        }

        public virtual Task<GetSubmissionsOutputDTO> GetSubmissionsQueryAsync(uint gameDay, BigInteger score, BigInteger startIndex, BigInteger count, BlockParameter blockParameter = null)
        {
            var getSubmissionsFunction = new GetSubmissionsFunction();
                getSubmissionsFunction.GameDay = gameDay;
                getSubmissionsFunction.Score = score;
                getSubmissionsFunction.StartIndex = startIndex;
                getSubmissionsFunction.Count = count;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetSubmissionsFunction, GetSubmissionsOutputDTO>(getSubmissionsFunction, blockParameter);
        }

        public virtual Task<string> GrantRoleRequestAsync(GrantRoleFunction grantRoleFunction)
        {
             return ContractHandler.SendRequestAsync(grantRoleFunction);
        }

        public virtual Task<TransactionReceipt> GrantRoleRequestAndWaitForReceiptAsync(GrantRoleFunction grantRoleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(grantRoleFunction, cancellationToken);
        }

        public virtual Task<string> GrantRoleRequestAsync(byte[] role, string account)
        {
            var grantRoleFunction = new GrantRoleFunction();
                grantRoleFunction.Role = role;
                grantRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAsync(grantRoleFunction);
        }

        public virtual Task<TransactionReceipt> GrantRoleRequestAndWaitForReceiptAsync(byte[] role, string account, CancellationTokenSource cancellationToken = null)
        {
            var grantRoleFunction = new GrantRoleFunction();
                grantRoleFunction.Role = role;
                grantRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(grantRoleFunction, cancellationToken);
        }

        public Task<bool> HasRoleQueryAsync(HasRoleFunction hasRoleFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<HasRoleFunction, bool>(hasRoleFunction, blockParameter);
        }

        
        public virtual Task<bool> HasRoleQueryAsync(byte[] role, string account, BlockParameter blockParameter = null)
        {
            var hasRoleFunction = new HasRoleFunction();
                hasRoleFunction.Role = role;
                hasRoleFunction.Account = account;
            
            return ContractHandler.QueryAsync<HasRoleFunction, bool>(hasRoleFunction, blockParameter);
        }

        public virtual Task<string> InitializeRequestAsync(InitializeFunction initializeFunction)
        {
             return ContractHandler.SendRequestAsync(initializeFunction);
        }

        public virtual Task<TransactionReceipt> InitializeRequestAndWaitForReceiptAsync(InitializeFunction initializeFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(initializeFunction, cancellationToken);
        }

        public virtual Task<string> InitializeRequestAsync(string roleAdmin, string owner, string upgradeAdmin, string passportWallet, string wordListSeed, string wordList)
        {
            var initializeFunction = new InitializeFunction();
                initializeFunction.RoleAdmin = roleAdmin;
                initializeFunction.Owner = owner;
                initializeFunction.UpgradeAdmin = upgradeAdmin;
                initializeFunction.PassportWallet = passportWallet;
                initializeFunction.WordListSeed = wordListSeed;
                initializeFunction.WordList = wordList;
            
             return ContractHandler.SendRequestAsync(initializeFunction);
        }

        public virtual Task<TransactionReceipt> InitializeRequestAndWaitForReceiptAsync(string roleAdmin, string owner, string upgradeAdmin, string passportWallet, string wordListSeed, string wordList, CancellationTokenSource cancellationToken = null)
        {
            var initializeFunction = new InitializeFunction();
                initializeFunction.RoleAdmin = roleAdmin;
                initializeFunction.Owner = owner;
                initializeFunction.UpgradeAdmin = upgradeAdmin;
                initializeFunction.PassportWallet = passportWallet;
                initializeFunction.WordListSeed = wordListSeed;
                initializeFunction.WordList = wordList;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(initializeFunction, cancellationToken);
        }

        public Task<bool> IsPassportQueryAsync(IsPassportFunction isPassportFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsPassportFunction, bool>(isPassportFunction, blockParameter);
        }

        
        public virtual Task<bool> IsPassportQueryAsync(string target, BlockParameter blockParameter = null)
        {
            var isPassportFunction = new IsPassportFunction();
                isPassportFunction.Target = target;
            
            return ContractHandler.QueryAsync<IsPassportFunction, bool>(isPassportFunction, blockParameter);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction ownerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(ownerFunction, blockParameter);
        }

        
        public virtual Task<string> OwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, blockParameter);
        }

        public Task<byte[]> ProxiableUUIDQueryAsync(ProxiableUUIDFunction proxiableUUIDFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ProxiableUUIDFunction, byte[]>(proxiableUUIDFunction, blockParameter);
        }

        
        public virtual Task<byte[]> ProxiableUUIDQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ProxiableUUIDFunction, byte[]>(null, blockParameter);
        }

        public virtual Task<string> RemovePassportWalletRequestAsync(RemovePassportWalletFunction removePassportWalletFunction)
        {
             return ContractHandler.SendRequestAsync(removePassportWalletFunction);
        }

        public virtual Task<TransactionReceipt> RemovePassportWalletRequestAndWaitForReceiptAsync(RemovePassportWalletFunction removePassportWalletFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removePassportWalletFunction, cancellationToken);
        }

        public virtual Task<string> RemovePassportWalletRequestAsync(string wallet)
        {
            var removePassportWalletFunction = new RemovePassportWalletFunction();
                removePassportWalletFunction.Wallet = wallet;
            
             return ContractHandler.SendRequestAsync(removePassportWalletFunction);
        }

        public virtual Task<TransactionReceipt> RemovePassportWalletRequestAndWaitForReceiptAsync(string wallet, CancellationTokenSource cancellationToken = null)
        {
            var removePassportWalletFunction = new RemovePassportWalletFunction();
                removePassportWalletFunction.Wallet = wallet;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removePassportWalletFunction, cancellationToken);
        }

        public virtual Task<string> RenounceRoleRequestAsync(RenounceRoleFunction renounceRoleFunction)
        {
             return ContractHandler.SendRequestAsync(renounceRoleFunction);
        }

        public virtual Task<TransactionReceipt> RenounceRoleRequestAndWaitForReceiptAsync(RenounceRoleFunction renounceRoleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(renounceRoleFunction, cancellationToken);
        }

        public virtual Task<string> RenounceRoleRequestAsync(byte[] role, string callerConfirmation)
        {
            var renounceRoleFunction = new RenounceRoleFunction();
                renounceRoleFunction.Role = role;
                renounceRoleFunction.CallerConfirmation = callerConfirmation;
            
             return ContractHandler.SendRequestAsync(renounceRoleFunction);
        }

        public virtual Task<TransactionReceipt> RenounceRoleRequestAndWaitForReceiptAsync(byte[] role, string callerConfirmation, CancellationTokenSource cancellationToken = null)
        {
            var renounceRoleFunction = new RenounceRoleFunction();
                renounceRoleFunction.Role = role;
                renounceRoleFunction.CallerConfirmation = callerConfirmation;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(renounceRoleFunction, cancellationToken);
        }

        public virtual Task<string> RevokeRoleRequestAsync(RevokeRoleFunction revokeRoleFunction)
        {
             return ContractHandler.SendRequestAsync(revokeRoleFunction);
        }

        public virtual Task<TransactionReceipt> RevokeRoleRequestAndWaitForReceiptAsync(RevokeRoleFunction revokeRoleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeRoleFunction, cancellationToken);
        }

        public virtual Task<string> RevokeRoleRequestAsync(byte[] role, string account)
        {
            var revokeRoleFunction = new RevokeRoleFunction();
                revokeRoleFunction.Role = role;
                revokeRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAsync(revokeRoleFunction);
        }

        public virtual Task<TransactionReceipt> RevokeRoleRequestAndWaitForReceiptAsync(byte[] role, string account, CancellationTokenSource cancellationToken = null)
        {
            var revokeRoleFunction = new RevokeRoleFunction();
                revokeRoleFunction.Role = role;
                revokeRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeRoleFunction, cancellationToken);
        }

        public Task<BigInteger> SubmissionCountQueryAsync(SubmissionCountFunction submissionCountFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SubmissionCountFunction, BigInteger>(submissionCountFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> SubmissionCountQueryAsync(uint returnValue1, BlockParameter blockParameter = null)
        {
            var submissionCountFunction = new SubmissionCountFunction();
                submissionCountFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<SubmissionCountFunction, BigInteger>(submissionCountFunction, blockParameter);
        }

        public virtual Task<string> SubmitBoardRequestAsync(SubmitBoardFunction submitBoardFunction)
        {
             return ContractHandler.SendRequestAsync(submitBoardFunction);
        }

        public virtual Task<TransactionReceipt> SubmitBoardRequestAndWaitForReceiptAsync(SubmitBoardFunction submitBoardFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(submitBoardFunction, cancellationToken);
        }

        public virtual Task<string> SubmitBoardRequestAsync(uint gameDay, BigInteger score, string board)
        {
            var submitBoardFunction = new SubmitBoardFunction();
                submitBoardFunction.GameDay = gameDay;
                submitBoardFunction.Score = score;
                submitBoardFunction.Board = board;
            
             return ContractHandler.SendRequestAsync(submitBoardFunction);
        }

        public virtual Task<TransactionReceipt> SubmitBoardRequestAndWaitForReceiptAsync(uint gameDay, BigInteger score, string board, CancellationTokenSource cancellationToken = null)
        {
            var submitBoardFunction = new SubmitBoardFunction();
                submitBoardFunction.GameDay = gameDay;
                submitBoardFunction.Score = score;
                submitBoardFunction.Board = board;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(submitBoardFunction, cancellationToken);
        }

        public Task<bool> SupportsInterfaceQueryAsync(SupportsInterfaceFunction supportsInterfaceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SupportsInterfaceFunction, bool>(supportsInterfaceFunction, blockParameter);
        }

        
        public virtual Task<bool> SupportsInterfaceQueryAsync(byte[] interfaceId, BlockParameter blockParameter = null)
        {
            var supportsInterfaceFunction = new SupportsInterfaceFunction();
                supportsInterfaceFunction.InterfaceId = interfaceId;
            
            return ContractHandler.QueryAsync<SupportsInterfaceFunction, bool>(supportsInterfaceFunction, blockParameter);
        }

        public virtual Task<string> UpgradeStorageRequestAsync(UpgradeStorageFunction upgradeStorageFunction)
        {
             return ContractHandler.SendRequestAsync(upgradeStorageFunction);
        }

        public virtual Task<TransactionReceipt> UpgradeStorageRequestAndWaitForReceiptAsync(UpgradeStorageFunction upgradeStorageFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(upgradeStorageFunction, cancellationToken);
        }

        public virtual Task<string> UpgradeStorageRequestAsync(byte[] returnValue1)
        {
            var upgradeStorageFunction = new UpgradeStorageFunction();
                upgradeStorageFunction.ReturnValue1 = returnValue1;
            
             return ContractHandler.SendRequestAsync(upgradeStorageFunction);
        }

        public virtual Task<TransactionReceipt> UpgradeStorageRequestAndWaitForReceiptAsync(byte[] returnValue1, CancellationTokenSource cancellationToken = null)
        {
            var upgradeStorageFunction = new UpgradeStorageFunction();
                upgradeStorageFunction.ReturnValue1 = returnValue1;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(upgradeStorageFunction, cancellationToken);
        }

        public virtual Task<string> UpgradeToAndCallRequestAsync(UpgradeToAndCallFunction upgradeToAndCallFunction)
        {
             return ContractHandler.SendRequestAsync(upgradeToAndCallFunction);
        }

        public virtual Task<TransactionReceipt> UpgradeToAndCallRequestAndWaitForReceiptAsync(UpgradeToAndCallFunction upgradeToAndCallFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(upgradeToAndCallFunction, cancellationToken);
        }

        public virtual Task<string> UpgradeToAndCallRequestAsync(string newImplementation, byte[] data)
        {
            var upgradeToAndCallFunction = new UpgradeToAndCallFunction();
                upgradeToAndCallFunction.NewImplementation = newImplementation;
                upgradeToAndCallFunction.Data = data;
            
             return ContractHandler.SendRequestAsync(upgradeToAndCallFunction);
        }

        public virtual Task<TransactionReceipt> UpgradeToAndCallRequestAndWaitForReceiptAsync(string newImplementation, byte[] data, CancellationTokenSource cancellationToken = null)
        {
            var upgradeToAndCallFunction = new UpgradeToAndCallFunction();
                upgradeToAndCallFunction.NewImplementation = newImplementation;
                upgradeToAndCallFunction.Data = data;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(upgradeToAndCallFunction, cancellationToken);
        }

        public Task<BigInteger> VersionQueryAsync(VersionFunction versionFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VersionFunction, BigInteger>(versionFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> VersionQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VersionFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> WordListQueryAsync(WordListFunction wordListFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<WordListFunction, string>(wordListFunction, blockParameter);
        }

        
        public virtual Task<string> WordListQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<WordListFunction, string>(null, blockParameter);
        }

        public Task<string> WordListSeedQueryAsync(WordListSeedFunction wordListSeedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<WordListSeedFunction, string>(wordListSeedFunction, blockParameter);
        }

        
        public virtual Task<string> WordListSeedQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<WordListSeedFunction, string>(null, blockParameter);
        }

        public override List<Type> GetAllFunctionTypes()
        {
            return new List<Type>
            {
                typeof(DefaultAdminRoleFunction),
                typeof(OwnerRoleFunction),
                typeof(UpgradeInterfaceVersionFunction),
                typeof(UpgradeRoleFunction),
                typeof(AddPassportWalletFunction),
                typeof(BestScoreByDayFunction),
                typeof(GetBestSubmissionsFunction),
                typeof(GetRoleAdminFunction),
                typeof(GetRoleMemberFunction),
                typeof(GetRoleMemberCountFunction),
                typeof(GetRoleMembersFunction),
                typeof(GetSubmissionCountAtScoreFunction),
                typeof(GetSubmissionsFunction),
                typeof(GrantRoleFunction),
                typeof(HasRoleFunction),
                typeof(InitializeFunction),
                typeof(IsPassportFunction),
                typeof(OwnerFunction),
                typeof(ProxiableUUIDFunction),
                typeof(RemovePassportWalletFunction),
                typeof(RenounceRoleFunction),
                typeof(RevokeRoleFunction),
                typeof(SubmissionCountFunction),
                typeof(SubmitBoardFunction),
                typeof(SupportsInterfaceFunction),
                typeof(UpgradeStorageFunction),
                typeof(UpgradeToAndCallFunction),
                typeof(VersionFunction),
                typeof(WordListFunction),
                typeof(WordListSeedFunction)
            };
        }

        public override List<Type> GetAllEventTypes()
        {
            return new List<Type>
            {
                typeof(BoardSubmittedEventDTO),
                typeof(InitializedEventDTO),
                typeof(RoleAdminChangedEventDTO),
                typeof(RoleGrantedEventDTO),
                typeof(RoleRevokedEventDTO),
                typeof(ScoreMismatchEventDTO),
                typeof(ScoreNotCompetitiveEventDTO),
                typeof(UpgradedEventDTO),
                typeof(WalletAllowlistChangedEventDTO)
            };
        }

        public override List<Type> GetAllErrorTypes()
        {
            return new List<Type>
            {
                typeof(AccessControlBadConfirmationError),
                typeof(AccessControlUnauthorizedAccountError),
                typeof(AddressEmptyCodeError),
                typeof(BadAddressError),
                typeof(CanNotUpgradeToLowerOrSameVersionError),
                typeof(ERC1967InvalidImplementationError),
                typeof(ERC1967NonPayableError),
                typeof(FailedCallError),
                typeof(GameDayInvalidError),
                typeof(InvalidBoardSizeError),
                typeof(InvalidInitializationError),
                typeof(NotInitializingError),
                typeof(NotPassportWalletError),
                typeof(SeedWordNotFoundError),
                typeof(UUPSUnauthorizedCallContextError),
                typeof(UUPSUnsupportedProxiableUUIDError)
            };
        }
    }
}
