using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;

namespace DI.Chakra.MockedTests
{
    /// <summary>One recorded connector call.</summary>
    public sealed class ConnectorCall
    {
        public string Method { get; set; }
        public long EndpointId { get; set; }
        public long? RequestId { get; set; }
        public string Account { get; set; }
        public string Detail { get; set; }
        public override string ToString() => $"{Method}({Account}{(string.IsNullOrEmpty(Detail) ? "" : ", " + Detail)}) ep={EndpointId} req={RequestId}";
    }

    /// <summary>
    /// Stands in for every real connector. It never contacts a target system: each operation is
    /// recorded, then (for the operations that have one) the base class's database-only
    /// reconciliation update runs, exactly as a real connector does after a successful target call.
    /// Tests can make any method fail (return false) or throw.
    /// </summary>
    public class MockConnector : ConnectorBase
    {
        public static readonly ConcurrentQueue<ConnectorCall> Calls = new ConcurrentQueue<ConnectorCall>();
        /// <summary>Methods that should return false (simulated target failure).</summary>
        public static readonly HashSet<string> FailOn = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>Methods that should throw (simulated connector crash).</summary>
        public static readonly HashSet<string> ThrowOn = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public static void Reset()
        {
            while (Calls.TryDequeue(out _)) { }
            FailOn.Clear();
            ThrowOn.Clear();
            OnlineAccountExists = false;
            MailBoxExistsAnswer = false;
        }

        public static IList<ConnectorCall> CallsFor(long endpointId) => Calls.Where(c => c.EndpointId == endpointId).ToList();

        /// <summary>Routes every ConnectorFactory.GetConnector call to a MockConnector.</summary>
        public static void Install()
        {
            // Null-safe: some callers ask for a connector with no request (SAP offboarding's HR write-back)
            // or for an endpoint that may not exist (e.g. no trusted-source endpoint configured).
            ConnectorFactory.TestConnectorOverride = (endpoint, request, logger) =>
                new MockConnector(endpoint, request, logger ?? new ProvisioningLogger(request?.Id, endpoint?.Id, endpoint?.EndpointSystemTypeId, endpoint?.DisplayName));
        }

        public static void Uninstall() => ConnectorFactory.TestConnectorOverride = null;

        public MockConnector(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger logger) : base(logger)
        {
            Endpoint = endpoint;
            CurrentRequest = currentRequest;
        }

        private bool Record(string method, IEndpointUserInfo user = null, string detail = null)
        {
            Calls.Enqueue(new ConnectorCall
            {
                Method = method,
                EndpointId = Endpoint?.Id ?? 0,
                RequestId = CurrentRequest?.Id,
                Account = user?.AccountName,
                Detail = detail,
            });
            if (ThrowOn.Contains(method))
                throw new InvalidOperationException($"MockConnector: simulated failure in {method}");
            return !FailOn.Contains(method);
        }

        // ---- abstract members ----
        public override bool Connect() => Record(nameof(Connect));
        public override void Disconnect() => Record(nameof(Disconnect));
        public override void GetData() => Record(nameof(GetData));
        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation) => Record(nameof(ChangePassword), null, UserName);
        public override bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields) => Record(nameof(CreateAccount), _userInfo);
        public override bool UpdateAccount(IEndpointUserInfo _userInfo) => Record(nameof(UpdateAccount), _userInfo);
        public override bool ChangeContainer(IEndpointUserInfo _userInfo) => Record(nameof(ChangeContainer), _userInfo, OuOf(_userInfo));
        public override bool ExtendValidity(IEndpointUserInfo _userInfo) => Record(nameof(ExtendValidity), _userInfo);

        // ---- account state: record, then keep the base DB reconciliation behaviour ----
        public override bool LockAccount(IEndpointUserInfo _userInfo) => Record(nameof(LockAccount), _userInfo) && base.LockAccount(_userInfo);
        public override bool UnLockAccount(IEndpointUserInfo _userInfo) => Record(nameof(UnLockAccount), _userInfo) && base.UnLockAccount(_userInfo);
        public override bool DisableAccount(IEndpointUserInfo _userInfo) => Record(nameof(DisableAccount), _userInfo) && base.DisableAccount(_userInfo);
        public override bool EnableAccount(IEndpointUserInfo _userInfo) => Record(nameof(EnableAccount), _userInfo) && base.EnableAccount(_userInfo);
        public override bool DeleteAccount(IEndpointUserInfo _userInfo) => Record(nameof(DeleteAccount), _userInfo) && base.DeleteAccount(_userInfo);
        public override bool ResetPassword(string UserName, string NewPassword, string OldPassword) => Record(nameof(ResetPassword), null, UserName);
        public override void MoveOU(IEndpointUserInfo _userInfo, string _ouName) => Record(nameof(MoveOU), _userInfo, _ouName);

        // ---- entitlements ----
        public override bool AddEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType) => Record(nameof(AddEntitlement), _userInfo, _entitlementType.ToString()) && base.AddEntitlement(_userInfo, _entitlementType);
        public override bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType) =>
            Record(nameof(RemoveEntitlement), _userInfo, _entitlementType + (_userInfo is SAPUserinfo ? ":" + RolesOf(_userInfo) : "")) && base.RemoveEntitlement(_userInfo, _entitlementType);
        public override bool AddEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType, List<string> entitlement) => Record(nameof(AddEntitlement), _userInfo, string.Join(";", entitlement ?? new List<string>()));
        public override bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType, List<string> entitlement) => Record(nameof(RemoveEntitlement), _userInfo, string.Join(";", entitlement ?? new List<string>()));
        public override bool RemoveAllEntitlements(IEndpointUserInfo _userInfo) => Record(nameof(RemoveAllEntitlements), _userInfo) && base.RemoveAllEntitlements(_userInfo);

        // ---- AD-style checks ----
        /// <summary>
        /// "Does this OU exist?" is answered yes (the target is assumed correctly configured); any other
        /// property (mail, account name, ...) is answered "not taken", so name generation never loops.
        /// Add "IsUserPropertyExists" to FailOn to simulate a missing OU.
        /// </summary>
        public override bool IsUserPropertyExists(string propertyName, string propertyValue, string accountName)
        {
            bool ok = Record(nameof(IsUserPropertyExists), null, propertyName + "=" + propertyValue);
            return string.Equals(propertyName, "OU", StringComparison.OrdinalIgnoreCase) && ok;
        }
        public override bool PostReactivationProcedure(IEndpointUserInfo _userInfo) => Record(nameof(PostReactivationProcedure), _userInfo);
        public override bool UpdatePOBoxAttribute(IEndpointUserInfo _userInfo, bool add) => Record(nameof(UpdatePOBoxAttribute), _userInfo, add ? "add" : "remove");
        public override bool UpdateExterEmailAccecss(string accountName, string access) => Record(nameof(UpdateExterEmailAccecss), null, accountName + "=" + access);
        public override bool UpdateSSOAttributes(IEndpointUserInfo userInfo, string samAccountName, AccountType accountType) => Record(nameof(UpdateSSOAttributes), userInfo, samAccountName);
        public override bool UpdateAccountAttribute(IEndpointUserInfo _userInfo) => Record(nameof(UpdateAccountAttribute), _userInfo);
        public override bool IsActiveAccount(IEndpointUserInfo _userInfo) => Record(nameof(IsActiveAccount), _userInfo);

        // ---- SAP ----
        /// <summary>4-argument SAP role add: record, then keep the base DB-only behaviour.</summary>
        public override bool AddEntitlement(IEndpointUserInfo _userInfo, string Validfrom, string ValidTo, EntitlementType entitlementType) =>
            Record(nameof(AddEntitlement), _userInfo, entitlementType + ":" + RolesOf(_userInfo)) && base.AddEntitlement(_userInfo, Validfrom, ValidTo, entitlementType);
        public override bool AssignPosition(IEndpointUserInfo _userInfo, string positionCode, string positionText, DateTime? startDate, DateTime? endDate, bool hasHRInfo) =>
            Record(nameof(AssignPosition), _userInfo, positionCode);
        public override bool RemovePosition(IEndpointUserInfo _userInfo, string positionCode, DateTime? startDate, DateTime? endDate, bool hasHRInfo) =>
            Record(nameof(RemovePosition), _userInfo, positionCode);
        /// <summary>HR write-back during SAP offboarding; called with the trusted-source endpoint and no request.</summary>
        public override bool DelimitFLINSLogonIdInHR(string employeeId, DateTime delimitDate) => Record(nameof(DelimitFLINSLogonIdInHR), null, employeeId);

        // ---- O365 ----
        public static bool OnlineAccountExists { get; set; }
        public static bool MailBoxExistsAnswer { get; set; }
        public override bool AssignLicense(string userPrincipalName, Dictionary<string, string> licenses) => Record(nameof(AssignLicense), null, userPrincipalName);
        public override bool RemoveLicense(string userPrincipalName, Dictionary<string, string> licenses, List<string> licensesToRemove) => Record(nameof(RemoveLicense), null, userPrincipalName);
        public override bool UpdateMailBoxSettings(string userPrincipalName, ProvisioningParamCollection fields) => Record(nameof(UpdateMailBoxSettings), null, userPrincipalName);
        public override bool DownloadUser(string userPrincipalName) => Record(nameof(DownloadUser), null, userPrincipalName);
        public override bool MailBoxExists(string accountName) { Record(nameof(MailBoxExists), null, accountName); return MailBoxExistsAnswer; }
        public override bool ConvertSharedToRegularMailBox(string emailAddress) => Record(nameof(ConvertSharedToRegularMailBox), null, emailAddress);
        public override bool ConvertRegularToSharedMailBox(string emailAddress) => Record(nameof(ConvertRegularToSharedMailBox), null, emailAddress);
        public override bool AssignMembers(string emailAddress, Dictionary<string, string> members) => Record(nameof(AssignMembers), null, emailAddress);
        public override bool RemoveMembers(string emailAddress, Dictionary<string, string> members) => Record(nameof(RemoveMembers), null, emailAddress);

        /// <summary>The OU an AD account carries (for asserting MoveOU / ChangeContainer targets).</summary>
        private static string OuOf(IEndpointUserInfo u) => (u as ADSUserInfo)?.ou;
        /// <summary>The roles an SAP account carries in the call (what would be sent to SAP).</summary>
        private static string RolesOf(IEndpointUserInfo u) =>
            u is SAPUserinfo sap && sap.Roles != null ? string.Join(";", sap.Roles)
            : u?.Entitlements == null ? "" : string.Join(";", u.Entitlements.Select(e => e?.ToString()));

        // ---- lookups: the mock target has no pre-existing accounts ----
        public override bool CheckEndPointAccountExists(string accountName) { Record(nameof(CheckEndPointAccountExists), null, accountName); return false; }
        public override bool CheckOnlineAccountExists(string accountName) { Record(nameof(CheckOnlineAccountExists), null, accountName); return OnlineAccountExists; }
        public override bool IsUserExists(string userName) { Record(nameof(IsUserExists), null, userName); return false; }
    }
}
