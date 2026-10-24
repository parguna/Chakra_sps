using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector
{
    public class SkypeForBusinessProperties
    {
        #region UnAssigned Property
        public string AnnouncementServiceId { get; set; } = "AnnouncementServiceId";
        public string AnnouncementServerFqdn { get; set; } = "AnnouncementServerFqdn";
        public string AnnouncementName { get; set; } = "AnnouncementName";
        public string AnnouncementId { get; set; } = "AnnouncementId";
        public string NumberRangeStart { get; set; } = "NumberRangeStart";
        public string NumberRangeEnd { get; set; } = "NumberRangeEnd";
        public string Priority { get; set; } = "Priority";

        #endregion
        #region User Property
        public string SamAccountName { get; set; } = "SamAccountName";
        public string UserPrincipalName { get; set; } = "UserPrincipalName";
        public string FirstName { get; set; } = "FirstName";
        public string LastName { get; set; } = "LastName";
        public string WindowsEmailAddress { get; set; } = "WindowsEmailAddress";
        public string Sid { get; set; } = "Sid";
        public string LineServerURI { get; set; } = "LineServerURI";
        //public string OriginatorSid { get; set; } = "OriginatorSid";
        //public string AudioVideoDisabled { get; set; } = "AudioVideoDisabled";
        //public string IPPBXSoftPhoneRoutingEnabled { get; set; } = "IPPBXSoftPhoneRoutingEnabled";
        //public string RemoteCallControlTelephonyEnabled { get; set; } = "RemoteCallControlTelephonyEnabled";
        //public string PrivateLine { get; set; } = "PrivateLine";
        //public string AcpInfo { get; set; } = "AcpInfo";
        //public string HostedVoiceMail { get; set; } = "HostedVoiceMail";
        public string DisplayName { get; set; } = "DisplayName";
        //public string ProxyAddresses { get; set; } = "ProxyAddresses";
        //public string HomeServer { get; set; } = "HomeServer";
        //public string TargetServerIfMoving { get; set; } = "TargetServerIfMoving";
        //public string EnabledForFederation { get; set; } = "EnabledForFederation";
        //public string EnabledForInternetAccess { get; set; } = "EnabledForInternetAccess";
        //public string PublicNetworkEnabled { get; set; } = "PublicNetworkEnabled";
        public string EnterpriseVoiceEnabled { get; set; } = "EnterpriseVoiceEnabled";
        //public string EnabledForRichPresence { get; set; } = "EnabledForRichPresence";
        //public string ExchangeArchivingPolicy { get; set; } = "ExchangeArchivingPolicy";
        public string LineURI { get; set; } = "LineURI";
        public string SipAddress { get; set; } = "SipAddress";
        public string Enabled { get; set; } = "Enabled";
        public string TenantId { get; set; } = "TenantId";
        //public string UserRoutingGroupId { get; set; } = "UserRoutingGroupId";
        public string TargetRegistrarPool { get; set; } = "TargetRegistrarPool";
        public string VoicePolicy { get; set; } = "VoicePolicy";
        //public string MobilityPolicy { get; set; } = "MobilityPolicy";
        public string ConferencingPolicy { get; set; } = "ConferencingPolicy";
        //public string PresencePolicy { get; set; } = "PresencePolicy";
        public string VoiceRoutingPolicy { get; set; } = "VoiceRoutingPolicy";
        public string RegistrarPool { get; set; } = "RegistrarPool";
        public string DialPlan { get; set; } = "DialPlan";
        //public string LocationPolicy { get; set; } = "LocationPolicy";
        //public string ClientPolicy { get; set; } = "ClientPolicy";
        //public string ClientVersionPolicy { get; set; } = "ClientVersionPolicy";
        //public string ArchivingPolicy { get; set; } = "ArchivingPolicy";
        //public string LegalInterceptPolicy { get; set; } = "LegalInterceptPolicy";
        //public string PinPolicy { get; set; } = "PinPolicy";
        //public string CallViaWorkPolicy { get; set; } = "CallViaWorkPolicy";
        //public string GraphPolicy { get; set; } = "GraphPolicy";
        public string ExternalAccessPolicy { get; set; } = "ExternalAccessPolicy";
        //public string HostedVoicemailPolicy { get; set; } = "HostedVoicemailPolicy";
        //public string PersistentChatPolicy { get; set; } = "PersistentChatPolicy";
        //public string UserServicesPolicy { get; set; } = "UserServicesPolicy";
        //public string ExperiencePolicy { get; set; } = "ExperiencePolicy";
        //public string AddressBookPolicy { get; set; } = "AddressBookPolicy";
        //public string ThirdPartyVideoSystemPolicy { get; set; } = "ThirdPartyVideoSystemPolicy";
        public string HostingProvider { get; set; } = "HostingProvider";
        public string ObjectId { get; set; } = "ObjectId";
        //public string ExUmEnabled { get; set; } = "ExUmEnabled";
        public string Name { get; set; } = "Name";
        public string DistinguishedName { get; set; } = "DistinguishedName";
        public string Identity { get; set; } = "Identity";
        public string Guid { get; set; } = "Guid";
        public string ObjectCategory { get; set; } = "ObjectCategory";
        public string ObjectClass { get; set; } = "ObjectClass";
        public string WhenChanged { get; set; } = "WhenChanged";
        public string WhenCreated { get; set; } = "WhenCreated";
        public string OriginatingServer { get; set; } = "OriginatingServer";
        //public string IsByPassValidation { get; set; } = "IsByPassValidation";
        public string IsValid { get; set; } = "IsValid";
        public string ObjectState { get; set; } = "ObjectState";
        //For Teams
        public string TenantDialPlan { get; set; } = "TenantDialPlan";
        public string OnlineVoiceRoutingPolicy { get; set; } = "OnlineVoiceRoutingPolicy";
        public string TeamsUpgradePolicy { get; set; } = "TeamsUpgradePolicy";
        public string OnPremLineURI { get; set; } = "OnPremLineURI";

        #endregion
        #region ResponseGroup Property
        public string GroupName { get; set; } = "Name";
        public string OwnerPool { get; set; } = "OwnerPool";
        public string AgentsByUri { get; set; } = "Agents";
        public string Description { get; set; } = "Description";
        public string ParticipationPolicy { get; set; } = "ParticipationPolicy";
        public string AgentAlertTime { get; set; } = "AgentAlertTime";
        public string RoutingMethod { get; set; } = "RoutingMethod";
        public string DistributionGroupAddress { get; set; } = "DistributionLists";
        #endregion

    }
    public class SkypeForBusinessScriptBlock
    {
        public string Script { get; set; }
        public bool IncludeCredential { get; set; }
        public bool IncludeObject { get; set; }
    }
}
