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
using CrossWords.WorcadianCheckInV1.ContractDefinition;

namespace CrossWords.WorcadianCheckInV1 {
    public partial class WorcadianCheckInV1Service: WorcadianCheckInV1ServiceBase
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.IWeb3 web3, WorcadianCheckInV1Deployment worcadianCheckInV1Deployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<WorcadianCheckInV1Deployment>().SendRequestAndWaitForReceiptAsync(worcadianCheckInV1Deployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.IWeb3 web3, WorcadianCheckInV1Deployment worcadianCheckInV1Deployment)
        {
            return web3.Eth.GetContractDeploymentHandler<WorcadianCheckInV1Deployment>().SendRequestAsync(worcadianCheckInV1Deployment);
        }

        public static async Task<WorcadianCheckInV1Service> DeployContractAndGetServiceAsync(Nethereum.Web3.IWeb3 web3, WorcadianCheckInV1Deployment worcadianCheckInV1Deployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, worcadianCheckInV1Deployment, cancellationTokenSource);
            return new WorcadianCheckInV1Service(web3, receipt.ContractAddress);
        }

        public WorcadianCheckInV1Service(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

    }


    public partial class WorcadianCheckInV1ServiceBase: ContractWeb3ServiceBase
    {

        public WorcadianCheckInV1ServiceBase(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
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

        public virtual Task<string> CheckInRequestAsync(CheckInFunction checkInFunction)
        {
             return ContractHandler.SendRequestAsync(checkInFunction);
        }

        public virtual Task<TransactionReceipt> CheckInRequestAndWaitForReceiptAsync(CheckInFunction checkInFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(checkInFunction, cancellationToken);
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

        public Task<BigInteger> GetDaysPlayedQueryAsync(GetDaysPlayedFunction getDaysPlayedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetDaysPlayedFunction, BigInteger>(getDaysPlayedFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> GetDaysPlayedQueryAsync(string player, BlockParameter blockParameter = null)
        {
            var getDaysPlayedFunction = new GetDaysPlayedFunction();
                getDaysPlayedFunction.Player = player;
            
            return ContractHandler.QueryAsync<GetDaysPlayedFunction, BigInteger>(getDaysPlayedFunction, blockParameter);
        }

        public Task<List<BigInteger>> GetNumPlayersQueryAsync(GetNumPlayersFunction getNumPlayersFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetNumPlayersFunction, List<BigInteger>>(getNumPlayersFunction, blockParameter);
        }

        
        public virtual Task<List<BigInteger>> GetNumPlayersQueryAsync(uint startGameDay, uint numDays, BlockParameter blockParameter = null)
        {
            var getNumPlayersFunction = new GetNumPlayersFunction();
                getNumPlayersFunction.StartGameDay = startGameDay;
                getNumPlayersFunction.NumDays = numDays;
            
            return ContractHandler.QueryAsync<GetNumPlayersFunction, List<BigInteger>>(getNumPlayersFunction, blockParameter);
        }

        public Task<List<BigInteger>> GetNumSessionsQueryAsync(GetNumSessionsFunction getNumSessionsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetNumSessionsFunction, List<BigInteger>>(getNumSessionsFunction, blockParameter);
        }

        
        public virtual Task<List<BigInteger>> GetNumSessionsQueryAsync(uint startGameDay, uint numDays, BlockParameter blockParameter = null)
        {
            var getNumSessionsFunction = new GetNumSessionsFunction();
                getNumSessionsFunction.StartGameDay = startGameDay;
                getNumSessionsFunction.NumDays = numDays;
            
            return ContractHandler.QueryAsync<GetNumSessionsFunction, List<BigInteger>>(getNumSessionsFunction, blockParameter);
        }

        public Task<List<string>> GetPlayersQueryAsync(GetPlayersFunction getPlayersFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetPlayersFunction, List<string>>(getPlayersFunction, blockParameter);
        }

        
        public virtual Task<List<string>> GetPlayersQueryAsync(BigInteger startIndex, BigInteger count, BlockParameter blockParameter = null)
        {
            var getPlayersFunction = new GetPlayersFunction();
                getPlayersFunction.StartIndex = startIndex;
                getPlayersFunction.Count = count;
            
            return ContractHandler.QueryAsync<GetPlayersFunction, List<string>>(getPlayersFunction, blockParameter);
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

        public Task<BigInteger> GetTotalPlayersQueryAsync(GetTotalPlayersFunction getTotalPlayersFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetTotalPlayersFunction, BigInteger>(getTotalPlayersFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> GetTotalPlayersQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetTotalPlayersFunction, BigInteger>(null, blockParameter);
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

        public virtual Task<string> InitializeRequestAsync(string roleAdmin, string owner, string upgradeAdmin)
        {
            var initializeFunction = new InitializeFunction();
                initializeFunction.RoleAdmin = roleAdmin;
                initializeFunction.Owner = owner;
                initializeFunction.UpgradeAdmin = upgradeAdmin;
            
             return ContractHandler.SendRequestAsync(initializeFunction);
        }

        public virtual Task<TransactionReceipt> InitializeRequestAndWaitForReceiptAsync(string roleAdmin, string owner, string upgradeAdmin, CancellationTokenSource cancellationToken = null)
        {
            var initializeFunction = new InitializeFunction();
                initializeFunction.RoleAdmin = roleAdmin;
                initializeFunction.Owner = owner;
                initializeFunction.UpgradeAdmin = upgradeAdmin;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(initializeFunction, cancellationToken);
        }

        public Task<BigInteger> NumberOfPlayersQueryAsync(NumberOfPlayersFunction numberOfPlayersFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NumberOfPlayersFunction, BigInteger>(numberOfPlayersFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> NumberOfPlayersQueryAsync(uint gameDay, BlockParameter blockParameter = null)
        {
            var numberOfPlayersFunction = new NumberOfPlayersFunction();
                numberOfPlayersFunction.GameDay = gameDay;
            
            return ContractHandler.QueryAsync<NumberOfPlayersFunction, BigInteger>(numberOfPlayersFunction, blockParameter);
        }

        public Task<BigInteger> NumberOfSessionsQueryAsync(NumberOfSessionsFunction numberOfSessionsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NumberOfSessionsFunction, BigInteger>(numberOfSessionsFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> NumberOfSessionsQueryAsync(uint gameDay, BlockParameter blockParameter = null)
        {
            var numberOfSessionsFunction = new NumberOfSessionsFunction();
                numberOfSessionsFunction.GameDay = gameDay;
            
            return ContractHandler.QueryAsync<NumberOfSessionsFunction, BigInteger>(numberOfSessionsFunction, blockParameter);
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

        public override List<Type> GetAllFunctionTypes()
        {
            return new List<Type>
            {
                typeof(DefaultAdminRoleFunction),
                typeof(OwnerRoleFunction),
                typeof(UpgradeInterfaceVersionFunction),
                typeof(UpgradeRoleFunction),
                typeof(CheckInFunction),
                typeof(GetDaysPlayedFunction),
                typeof(GetNumPlayersFunction),
                typeof(GetNumSessionsFunction),
                typeof(GetPlayersFunction),
                typeof(GetRoleAdminFunction),
                typeof(GetRoleMemberFunction),
                typeof(GetRoleMemberCountFunction),
                typeof(GetRoleMembersFunction),
                typeof(GetTotalPlayersFunction),
                typeof(GrantRoleFunction),
                typeof(HasRoleFunction),
                typeof(InitializeFunction),
                typeof(NumberOfPlayersFunction),
                typeof(NumberOfSessionsFunction),
                typeof(OwnerFunction),
                typeof(ProxiableUUIDFunction),
                typeof(RenounceRoleFunction),
                typeof(RevokeRoleFunction),
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
                typeof(CheckInEventDTO),
                typeof(InitializedEventDTO),
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
                typeof(BadAddressError),
                typeof(CanNotUpgradeToLowerOrSameVersionError),
                typeof(ERC1967InvalidImplementationError),
                typeof(ERC1967NonPayableError),
                typeof(FailedCallError),
                typeof(GameDayInvalidError),
                typeof(InvalidInitializationError),
                typeof(NotInitializingError),
                typeof(UUPSUnauthorizedCallContextError),
                typeof(UUPSUnsupportedProxiableUUIDError)
            };
        }
    }
}
