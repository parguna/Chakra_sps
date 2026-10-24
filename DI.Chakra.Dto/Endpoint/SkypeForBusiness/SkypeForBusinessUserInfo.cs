using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SkypeForBusinessUserInfo :DtoBase, IEndpointUserInfo
    {
        public SkypeForBusinessUserInfo()
        {
            this.SkypeForBusinessResponseGroup = new List<SkypeForBusinessResponseGroupInfo>();
        }
        public string SamAccountName { get; set; }
        public string UserPrincipalName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string WindowsEmailAddress { get; set; }
        public string Sid { get; set; }
        public string LineServerURI { get; set; }
        public string OriginatorSid { get; set; }
        public string AudioVideoDisabled { get; set; }
        public string IPPBXSoftPhoneRoutingEnabled { get; set; }
        public string RemoteCallControlTelephonyEnabled { get; set; }
        public string PrivateLine { get; set; }
        public string AcpInfo { get; set; }
        public string HostedVoiceMail { get; set; }
        public string DisplayName { get; set; }
        public string ProxyAddresses { get; set; }
        public string HomeServer { get; set; }
        public string TargetServerIfMoving { get; set; }
        public string EnabledForFederation { get; set; }
        public string EnabledForInternetAccess { get; set; }
        public string PublicNetworkEnabled { get; set; }
        public string EnterpriseVoiceEnabled { get; set; }
        public string EnabledForRichPresence { get; set; }
        public string ExchangeArchivingPolicy { get; set; }
        public string LineURI { get; set; }
        public string SipAddress { get; set; }
        public bool Enabled { get; set; }
        public string TenantId { get; set; }
        public string UserRoutingGroupId { get; set; }
        public string TargetRegistrarPool { get; set; }
        public string VoicePolicy { get; set; }
        public string MobilityPolicy { get; set; }
        public string ConferencingPolicy { get; set; }
        public string PresencePolicy { get; set; }
        public string VoiceRoutingPolicy { get; set; }
        public string RegistrarPool { get; set; }
        public string DialPlan { get; set; }
        public string LocationPolicy { get; set; }
        public string ClientPolicy { get; set; }
        public string ClientVersionPolicy { get; set; }
        public string ArchivingPolicy { get; set; }
        public string LegalInterceptPolicy { get; set; }
        public string PinPolicy { get; set; }
        public string CallViaWorkPolicy { get; set; }
        public string GraphPolicy { get; set; }
        public string ExternalAccessPolicy { get; set; }
        public string HostedVoicemailPolicy { get; set; }
        public string PersistentChatPolicy { get; set; }
        public string UserServicesPolicy { get; set; }
        public string ExperiencePolicy { get; set; }
        public string AddressBookPolicy { get; set; }
        public string ThirdPartyVideoSystemPolicy { get; set; }
        public string HostingProvider { get; set; }
        public string ObjectId { get; set; }
        public string ExUmEnabled { get; set; }
        public string Name { get; set; }
        public string DistinguishedName { get; set; }
        public string Identity { get; set; }
        public string Guid { get; set; }
        public string ObjectCategory { get; set; }
        public string ObjectClass { get; set; }
        public DateTime? WhenChanged { get; set; }
        public DateTime? WhenCreated { get; set; }
        public string OriginatingServer { get; set; }
        public string IsByPassValidation { get; set; }
        public bool IsValid { get; set; }
        public string ObjectState { get; set; }
        public int? LineURIExtension { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public long EndPointSystemId { get; set; }
        public EndPoint EndPointSystem { get; set; }
        public EntitlementType UserType { get; set; }
        public Nullable<bool> WaitingAccountSync { get; set; }
        public Nullable<long> AccessRequestId { get; set; }
        public string TenantDialPlan { get; set; }
        public string OnlineVoiceRoutingPolicy { get; set; }
        public string TeamsUpgradePolicy { get; set; }
        public List<SkypeForBusinessResponseGroupInfo> SkypeForBusinessResponseGroup { get; set; }
        public string AccountName
        {
            get
            {
                return this.UserPrincipalName;
            }
        }
        public string AccountPassword
        {
            get
            {
                return null;
            }
        }
        public List<IEndpointEntitlementInfo> Entitlements { get; set; }
    }
}
