using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;

namespace CrossWords {



    public partial class DefaultAdminRoleFunction : DefaultAdminRoleFunctionBase { }

    [Function("DEFAULT_ADMIN_ROLE", "bytes32")]
    public class DefaultAdminRoleFunctionBase : FunctionMessage
    {

    }

    public partial class OwnerRoleFunction : OwnerRoleFunctionBase { }

    [Function("OWNER_ROLE", "bytes32")]
    public class OwnerRoleFunctionBase : FunctionMessage
    {

    }

    public partial class UpgradeInterfaceVersionFunction : UpgradeInterfaceVersionFunctionBase { }

    [Function("UPGRADE_INTERFACE_VERSION", "string")]
    public class UpgradeInterfaceVersionFunctionBase : FunctionMessage
    {

    }

    public partial class UpgradeRoleFunction : UpgradeRoleFunctionBase { }

    [Function("UPGRADE_ROLE", "bytes32")]
    public class UpgradeRoleFunctionBase : FunctionMessage
    {

    }

    public partial class CalcFunction : CalcFunctionBase { }

    [Function("calc", typeof(CalcOutputDTO))]
    public class CalcFunctionBase : FunctionMessage
    {
        [Parameter("bytes", "_input", 1)]
        public virtual byte[] Input { get; set; }
    }

    public partial class CalcPointsFunction : CalcPointsFunctionBase { }

    [Function("calcPoints", "uint256")]
    public class CalcPointsFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "_target", 1)]
        public virtual BigInteger Target { get; set; }
        [Parameter("uint256", "_res1", 2)]
        public virtual BigInteger Res1 { get; set; }
        [Parameter("uint256", "_res2", 3)]
        public virtual BigInteger Res2 { get; set; }
        [Parameter("uint256", "_res3", 4)]
        public virtual BigInteger Res3 { get; set; }
    }

    public partial class CalcPointsSingleFunction : CalcPointsSingleFunctionBase { }

    [Function("calcPointsSingle", "uint256")]
    public class CalcPointsSingleFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "_target", 1)]
        public virtual BigInteger Target { get; set; }
        [Parameter("uint256", "_res", 2)]
        public virtual BigInteger Res { get; set; }
    }

    public partial class CheckInFunction : CheckInFunctionBase { }

    [Function("checkIn")]
    public class CheckInFunctionBase : FunctionMessage
    {
        [Parameter("uint32", "_gameDay", 1)]
        public virtual uint GameDay { get; set; }
    }

    public partial class DetermineCurrentGameDaysFunction : DetermineCurrentGameDaysFunctionBase { }

    [Function("determineCurrentGameDays", typeof(DetermineCurrentGameDaysOutputDTO))]
    public class DetermineCurrentGameDaysFunctionBase : FunctionMessage
    {

    }

    public partial class GetRoleAdminFunction : GetRoleAdminFunctionBase { }

    [Function("getRoleAdmin", "bytes32")]
    public class GetRoleAdminFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
    }

    public partial class GetRoleMemberFunction : GetRoleMemberFunctionBase { }

    [Function("getRoleMember", "address")]
    public class GetRoleMemberFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
        [Parameter("uint256", "index", 2)]
        public virtual BigInteger Index { get; set; }
    }

    public partial class GetRoleMemberCountFunction : GetRoleMemberCountFunctionBase { }

    [Function("getRoleMemberCount", "uint256")]
    public class GetRoleMemberCountFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
    }

    public partial class GetRoleMembersFunction : GetRoleMembersFunctionBase { }

    [Function("getRoleMembers", "address[]")]
    public class GetRoleMembersFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
    }

    public partial class GetTargetValueFunction : GetTargetValueFunctionBase { }

    [Function("getTargetValue", "uint256")]
    public class GetTargetValueFunctionBase : FunctionMessage
    {
        [Parameter("uint32", "_gameDay", 1)]
        public virtual uint GameDay { get; set; }
    }

    public partial class GrantRoleFunction : GrantRoleFunctionBase { }

    [Function("grantRole")]
    public class GrantRoleFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 2)]
        public virtual string Account { get; set; }
    }

    public partial class HasRoleFunction : HasRoleFunctionBase { }

    [Function("hasRole", "bool")]
    public class HasRoleFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 2)]
        public virtual string Account { get; set; }
    }

    public partial class InitializeFunction : InitializeFunctionBase { }

    [Function("initialize")]
    public class InitializeFunctionBase : FunctionMessage
    {
        [Parameter("address", "_roleAdmin", 1)]
        public virtual string RoleAdmin { get; set; }
        [Parameter("address", "_owner", 2)]
        public virtual string Owner { get; set; }
        [Parameter("address", "_upgradeAdmin", 3)]
        public virtual string UpgradeAdmin { get; set; }
    }

    public partial class OwnerFunction : OwnerFunctionBase { }

    [Function("owner", "address")]
    public class OwnerFunctionBase : FunctionMessage
    {

    }

    public partial class ProxiableUUIDFunction : ProxiableUUIDFunctionBase { }

    [Function("proxiableUUID", "bytes32")]
    public class ProxiableUUIDFunctionBase : FunctionMessage
    {

    }

    public partial class RenounceRoleFunction : RenounceRoleFunctionBase { }

    [Function("renounceRole")]
    public class RenounceRoleFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "callerConfirmation", 2)]
        public virtual string CallerConfirmation { get; set; }
    }

    public partial class RevokeRoleFunction : RevokeRoleFunctionBase { }

    [Function("revokeRole")]
    public class RevokeRoleFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 2)]
        public virtual string Account { get; set; }
    }

    public partial class GetAllSolutionsFunction : GetAllSolutionsFunctionBase { }

    [Function("getAllSolutions", typeof(GetAllSolutionsOutputDTO))]
    public class GetAllSolutionsFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger GameDay { get; set; }
    }

    public partial class SolutionsFunction : SolutionsFunctionBase { }

    [Function("solutions", typeof(SolutionsOutputDTO))]
    public class SolutionsFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger GameDay { get; set; }
    }

    public partial class StatsFunction : StatsFunctionBase { }

    [Function("stats", typeof(StatsOutputDTO))]
    public class StatsFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class StoreResultsFunction : StoreResultsFunctionBase { }

    [Function("storeResults")]
    public class StoreResultsFunctionBase : FunctionMessage
    {
        [Parameter("uint32", "_gameDay", 1)]
        public virtual uint GameDay { get; set; }
        [Parameter("bytes", "_sol1", 2)]
        public virtual byte[] Sol1 { get; set; }
        [Parameter("bytes", "_sol2", 3)]
        public virtual byte[] Sol2 { get; set; }
        [Parameter("bytes", "_sol3", 4)]
        public virtual byte[] Sol3 { get; set; }
        [Parameter("bool", "_store", 5)]
        public virtual bool Store { get; set; }
    }

    public partial class SupportsInterfaceFunction : SupportsInterfaceFunctionBase { }

    [Function("supportsInterface", "bool")]
    public class SupportsInterfaceFunctionBase : FunctionMessage
    {
        [Parameter("bytes4", "interfaceId", 1)]
        public virtual byte[] InterfaceId { get; set; }
    }

    public partial class UpgradeStorageFunction : UpgradeStorageFunctionBase { }

    [Function("upgradeStorage")]
    public class UpgradeStorageFunctionBase : FunctionMessage
    {
        [Parameter("bytes", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class UpgradeToAndCallFunction : UpgradeToAndCallFunctionBase { }

    [Function("upgradeToAndCall")]
    public class UpgradeToAndCallFunctionBase : FunctionMessage
    {
        [Parameter("address", "newImplementation", 1)]
        public virtual string NewImplementation { get; set; }
        [Parameter("bytes", "data", 2)]
        public virtual byte[] Data { get; set; }
    }

    public partial class VersionFunction : VersionFunctionBase { }

    [Function("version", "uint256")]
    public class VersionFunctionBase : FunctionMessage
    {

    }

    public partial class CongratulationsEventDTO : CongratulationsEventDTOBase { }

    [Event("Congratulations")]
    public class CongratulationsEventDTOBase : IEventDTO
    {
        [Parameter("address", "player", 1, false )]
        public virtual string Player { get; set; }
        [Parameter("bytes", "solution1", 2, false )]
        public virtual byte[] Solution1 { get; set; }
        [Parameter("bytes", "solution2", 3, false )]
        public virtual byte[] Solution2 { get; set; }
        [Parameter("bytes", "solution3", 4, false )]
        public virtual byte[] Solution3 { get; set; }
        [Parameter("uint256", "points", 5, false )]
        public virtual BigInteger Points { get; set; }
    }

    public partial class InitializedEventDTO : InitializedEventDTOBase { }

    [Event("Initialized")]
    public class InitializedEventDTOBase : IEventDTO
    {
        [Parameter("uint64", "version", 1, false )]
        public virtual ulong Version { get; set; }
    }

    public partial class NextTimeEventDTO : NextTimeEventDTOBase { }

    [Event("NextTime")]
    public class NextTimeEventDTOBase : IEventDTO
    {
        [Parameter("address", "player", 1, false )]
        public virtual string Player { get; set; }
        [Parameter("bytes", "solution1", 2, false )]
        public virtual byte[] Solution1 { get; set; }
        [Parameter("bytes", "solution2", 3, false )]
        public virtual byte[] Solution2 { get; set; }
        [Parameter("bytes", "solution3", 4, false )]
        public virtual byte[] Solution3 { get; set; }
        [Parameter("uint256", "points", 5, false )]
        public virtual BigInteger Points { get; set; }
        [Parameter("uint256", "bestPoints", 6, false )]
        public virtual BigInteger BestPoints { get; set; }
    }

    public partial class RoleAdminChangedEventDTO : RoleAdminChangedEventDTOBase { }

    [Event("RoleAdminChanged")]
    public class RoleAdminChangedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "role", 1, true )]
        public virtual byte[] Role { get; set; }
        [Parameter("bytes32", "previousAdminRole", 2, true )]
        public virtual byte[] PreviousAdminRole { get; set; }
        [Parameter("bytes32", "newAdminRole", 3, true )]
        public virtual byte[] NewAdminRole { get; set; }
    }

    public partial class RoleGrantedEventDTO : RoleGrantedEventDTOBase { }

    [Event("RoleGranted")]
    public class RoleGrantedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "role", 1, true )]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 2, true )]
        public virtual string Account { get; set; }
        [Parameter("address", "sender", 3, true )]
        public virtual string Sender { get; set; }
    }

    public partial class RoleRevokedEventDTO : RoleRevokedEventDTOBase { }

    [Event("RoleRevoked")]
    public class RoleRevokedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "role", 1, true )]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 2, true )]
        public virtual string Account { get; set; }
        [Parameter("address", "sender", 3, true )]
        public virtual string Sender { get; set; }
    }

    public partial class UpgradedEventDTO : UpgradedEventDTOBase { }

    [Event("Upgraded")]
    public class UpgradedEventDTOBase : IEventDTO
    {
        [Parameter("address", "implementation", 1, true )]
        public virtual string Implementation { get; set; }
    }

    public partial class AccessControlBadConfirmationError : AccessControlBadConfirmationErrorBase { }
    [Error("AccessControlBadConfirmation")]
    public class AccessControlBadConfirmationErrorBase : IErrorDTO
    {
    }

    public partial class AccessControlUnauthorizedAccountError : AccessControlUnauthorizedAccountErrorBase { }

    [Error("AccessControlUnauthorizedAccount")]
    public class AccessControlUnauthorizedAccountErrorBase : IErrorDTO
    {
        [Parameter("address", "account", 1)]
        public virtual string Account { get; set; }
        [Parameter("bytes32", "neededRole", 2)]
        public virtual byte[] NeededRole { get; set; }
    }

    public partial class AddressEmptyCodeError : AddressEmptyCodeErrorBase { }

    [Error("AddressEmptyCode")]
    public class AddressEmptyCodeErrorBase : IErrorDTO
    {
        [Parameter("address", "target", 1)]
        public virtual string Target { get; set; }
    }

    public partial class CanNotUpgradeToLowerOrSameVersionError : CanNotUpgradeToLowerOrSameVersionErrorBase { }

    [Error("CanNotUpgradeToLowerOrSameVersion")]
    public class CanNotUpgradeToLowerOrSameVersionErrorBase : IErrorDTO
    {
        [Parameter("uint256", "_storageVersion", 1)]
        public virtual BigInteger StorageVersion { get; set; }
    }

    public partial class DivideByZeroError : DivideByZeroErrorBase { }
    [Error("DivideByZero")]
    public class DivideByZeroErrorBase : IErrorDTO
    {
    }

    public partial class ERC1967InvalidImplementationError : ERC1967InvalidImplementationErrorBase { }

    [Error("ERC1967InvalidImplementation")]
    public class ERC1967InvalidImplementationErrorBase : IErrorDTO
    {
        [Parameter("address", "implementation", 1)]
        public virtual string Implementation { get; set; }
    }

    public partial class ERC1967NonPayableError : ERC1967NonPayableErrorBase { }
    [Error("ERC1967NonPayable")]
    public class ERC1967NonPayableErrorBase : IErrorDTO
    {
    }

    public partial class EmptyInputError : EmptyInputErrorBase { }
    [Error("EmptyInput")]
    public class EmptyInputErrorBase : IErrorDTO
    {
    }

    public partial class EndedOnInvalidCharacterError : EndedOnInvalidCharacterErrorBase { }

    [Error("EndedOnInvalidCharacter")]
    public class EndedOnInvalidCharacterErrorBase : IErrorDTO
    {
        [Parameter("uint256", "_unknownChar", 1)]
        public virtual BigInteger UnknownChar { get; set; }
    }

    public partial class FailedCallError : FailedCallErrorBase { }
    [Error("FailedCall")]
    public class FailedCallErrorBase : IErrorDTO
    {
    }

    public partial class GameDayInvalidError : GameDayInvalidErrorBase { }

    [Error("GameDayInvalid")]
    public class GameDayInvalidErrorBase : IErrorDTO
    {
        [Parameter("uint32", "_requestedGameDay", 1)]
        public virtual uint RequestedGameDay { get; set; }
        [Parameter("uint32", "_minGameDay", 2)]
        public virtual uint MinGameDay { get; set; }
        [Parameter("uint32", "_maxGameDay", 3)]
        public virtual uint MaxGameDay { get; set; }
    }

    public partial class InvalidInitializationError : InvalidInitializationErrorBase { }
    [Error("InvalidInitialization")]
    public class InvalidInitializationErrorBase : IErrorDTO
    {
    }

    public partial class InvalidNumber1Error : InvalidNumber1ErrorBase { }
    [Error("InvalidNumber1")]
    public class InvalidNumber1ErrorBase : IErrorDTO
    {
    }

    public partial class InvalidNumber3Error : InvalidNumber3ErrorBase { }
    [Error("InvalidNumber3")]
    public class InvalidNumber3ErrorBase : IErrorDTO
    {
    }

    public partial class InvalidNumber4Error : InvalidNumber4ErrorBase { }
    [Error("InvalidNumber4")]
    public class InvalidNumber4ErrorBase : IErrorDTO
    {
    }

    public partial class InvalidStartError : InvalidStartErrorBase { }
    [Error("InvalidStart")]
    public class InvalidStartErrorBase : IErrorDTO
    {
    }

    public partial class LeadingZeroError : LeadingZeroErrorBase { }
    [Error("LeadingZero")]
    public class LeadingZeroErrorBase : IErrorDTO
    {
    }

    public partial class LeftBracketAfterNumericError : LeftBracketAfterNumericErrorBase { }
    [Error("LeftBracketAfterNumeric")]
    public class LeftBracketAfterNumericErrorBase : IErrorDTO
    {
    }

    public partial class LessThanZeroError : LessThanZeroErrorBase { }
    [Error("LessThanZero")]
    public class LessThanZeroErrorBase : IErrorDTO
    {
    }

    public partial class NoMatchingRightBracketError : NoMatchingRightBracketErrorBase { }
    [Error("NoMatchingRightBracket")]
    public class NoMatchingRightBracketErrorBase : IErrorDTO
    {
    }

    public partial class NotDivisibleError : NotDivisibleErrorBase { }
    [Error("NotDivisible")]
    public class NotDivisibleErrorBase : IErrorDTO
    {
    }

    public partial class NotInitializingError : NotInitializingErrorBase { }
    [Error("NotInitializing")]
    public class NotInitializingErrorBase : IErrorDTO
    {
    }

    public partial class NumberPreviouslyUsedError : NumberPreviouslyUsedErrorBase { }

    [Error("NumberPreviouslyUsed")]
    public class NumberPreviouslyUsedErrorBase : IErrorDTO
    {
        [Parameter("uint256", "_repeatedNumber", 1)]
        public virtual BigInteger RepeatedNumber { get; set; }
    }

    public partial class NumbersRepeatedError : NumbersRepeatedErrorBase { }
    [Error("NumbersRepeated")]
    public class NumbersRepeatedErrorBase : IErrorDTO
    {
    }

    public partial class OperationAfterNonNumericError : OperationAfterNonNumericErrorBase { }
    [Error("OperationAfterNonNumeric")]
    public class OperationAfterNonNumericErrorBase : IErrorDTO
    {
    }

    public partial class RightBracketAfterNonNumericError : RightBracketAfterNonNumericErrorBase { }
    [Error("RightBracketAfterNonNumeric")]
    public class RightBracketAfterNonNumericErrorBase : IErrorDTO
    {
    }

    public partial class RightBracketBeforeLeftError : RightBracketBeforeLeftErrorBase { }
    [Error("RightBracketBeforeLeft")]
    public class RightBracketBeforeLeftErrorBase : IErrorDTO
    {
    }

    public partial class TooManyLeftBracketsError : TooManyLeftBracketsErrorBase { }
    [Error("TooManyLeftBrackets")]
    public class TooManyLeftBracketsErrorBase : IErrorDTO
    {
    }

    public partial class TooManyNumbersError : TooManyNumbersErrorBase { }
    [Error("TooManyNumbers")]
    public class TooManyNumbersErrorBase : IErrorDTO
    {
    }

    public partial class UUPSUnauthorizedCallContextError : UUPSUnauthorizedCallContextErrorBase { }
    [Error("UUPSUnauthorizedCallContext")]
    public class UUPSUnauthorizedCallContextErrorBase : IErrorDTO
    {
    }

    public partial class UUPSUnsupportedProxiableUUIDError : UUPSUnsupportedProxiableUUIDErrorBase { }

    [Error("UUPSUnsupportedProxiableUUID")]
    public class UUPSUnsupportedProxiableUUIDErrorBase : IErrorDTO
    {
        [Parameter("bytes32", "slot", 1)]
        public virtual byte[] Slot { get; set; }
    }

    public partial class UnknownSymbolError : UnknownSymbolErrorBase { }
    [Error("UnknownSymbol")]
    public class UnknownSymbolErrorBase : IErrorDTO
    {
    }

    public partial class DefaultAdminRoleOutputDTO : DefaultAdminRoleOutputDTOBase { }

    [FunctionOutput]
    public class DefaultAdminRoleOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class OwnerRoleOutputDTO : OwnerRoleOutputDTOBase { }

    [FunctionOutput]
    public class OwnerRoleOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class UpgradeInterfaceVersionOutputDTO : UpgradeInterfaceVersionOutputDTOBase { }

    [FunctionOutput]
    public class UpgradeInterfaceVersionOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class UpgradeRoleOutputDTO : UpgradeRoleOutputDTOBase { }

    [FunctionOutput]
    public class UpgradeRoleOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class CalcOutputDTO : CalcOutputDTOBase { }

    [FunctionOutput]
    public class CalcOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
        [Parameter("uint256", "", 2)]
        public virtual BigInteger ReturnValue2 { get; set; }
    }

    public partial class CalcPointsOutputDTO : CalcPointsOutputDTOBase { }

    [FunctionOutput]
    public class CalcPointsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class CalcPointsSingleOutputDTO : CalcPointsSingleOutputDTOBase { }

    [FunctionOutput]
    public class CalcPointsSingleOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }



    public partial class DetermineCurrentGameDaysOutputDTO : DetermineCurrentGameDaysOutputDTOBase { }

    [FunctionOutput]
    public class DetermineCurrentGameDaysOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint32", "", 1)]
        public virtual uint ReturnValue1 { get; set; }
        [Parameter("uint32", "", 2)]
        public virtual uint ReturnValue2 { get; set; }
    }

    public partial class GetRoleAdminOutputDTO : GetRoleAdminOutputDTOBase { }

    [FunctionOutput]
    public class GetRoleAdminOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class GetRoleMemberOutputDTO : GetRoleMemberOutputDTOBase { }

    [FunctionOutput]
    public class GetRoleMemberOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class GetRoleMemberCountOutputDTO : GetRoleMemberCountOutputDTOBase { }

    [FunctionOutput]
    public class GetRoleMemberCountOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class GetRoleMembersOutputDTO : GetRoleMembersOutputDTOBase { }

    [FunctionOutput]
    public class GetRoleMembersOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address[]", "", 1)]
        public virtual List<string> ReturnValue1 { get; set; }
    }

    public partial class GetTargetValueOutputDTO : GetTargetValueOutputDTOBase { }

    [FunctionOutput]
    public class GetTargetValueOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }



    public partial class HasRoleOutputDTO : HasRoleOutputDTOBase { }

    [FunctionOutput]
    public class HasRoleOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }



    public partial class OwnerOutputDTO : OwnerOutputDTOBase { }

    [FunctionOutput]
    public class OwnerOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class ProxiableUUIDOutputDTO : ProxiableUUIDOutputDTOBase { }

    [FunctionOutput]
    public class ProxiableUUIDOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }





    public partial class SolutionsOutputDTO : SolutionsOutputDTOBase { }

    [FunctionOutput]
    public class SolutionsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes", "combinedSolution", 1)]
        public virtual byte[] CombinedSolution { get; set; }
        [Parameter("uint256", "points", 2)]
        public virtual BigInteger Points { get; set; }
        [Parameter("address", "player", 3)]
        public virtual string Player { get; set; }
    }

    public partial class GetAllSolutionsOutputDTO : GetAllSolutionsOutputDTOBase { }

    [FunctionOutput]
    public class GetAllSolutionsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "points", 1)]
        public virtual BigInteger Points { get; set; }

        [Parameter("tuple[]", "", 2)]
        public virtual List<ExtraSolutionDTO> Solutions { get; set; }
    }

    public class ExtraSolutionDTO
    {
        [Parameter("bytes", "combinedSolution", 1)]
        public virtual byte[] CombinedSolution { get; set; }
        [Parameter("address", "player", 2)]
        public virtual string Player { get; set; }
    }


    public partial class StatsOutputDTO : StatsOutputDTOBase { }

    [FunctionOutput]
    public class StatsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint32", "firstGameDay", 1)]
        public virtual uint FirstGameDay { get; set; }
        [Parameter("uint32", "mostRecentGameDay", 2)]
        public virtual uint MostRecentGameDay { get; set; }
        [Parameter("uint256", "totalPoints", 3)]
        public virtual BigInteger TotalPoints { get; set; }
        [Parameter("uint256", "daysPlayed", 4)]
        public virtual BigInteger DaysPlayed { get; set; }
    }



    public partial class SupportsInterfaceOutputDTO : SupportsInterfaceOutputDTOBase { }

    [FunctionOutput]
    public class SupportsInterfaceOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }





    public partial class VersionOutputDTO : VersionOutputDTOBase { }

    [FunctionOutput]
    public class VersionOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }
}
