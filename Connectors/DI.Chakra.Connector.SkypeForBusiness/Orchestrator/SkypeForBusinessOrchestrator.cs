using DI.Chakra.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure;
namespace DI.Chakra.Connector
{
    public static class SkypeForBusinessOrchestrator
    {
        public static SkypeForBusinessUserInfo PopulateUser(PSObject psMemberInfo, long endpointId, SkypeForBusinessProperties SkypeForBusinessProperties, string DCCPolicyName)
        {
            var userInfo = new SkypeForBusinessUserInfo();
            try
            {
                userInfo.EndPointSystemId = endpointId;
                userInfo.SamAccountName = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.SamAccountName);
                userInfo.EnterpriseVoiceEnabled = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.EnterpriseVoiceEnabled);

                userInfo.UserPrincipalName = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.UserPrincipalName);
                //Currently SamAccountname not same as UPN.Hence extracting the details.
                userInfo.SamAccountName = userInfo.AccountNameFromUPN;
                userInfo.FirstName = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.FirstName);

                userInfo.LastName = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.LastName);

                userInfo.WindowsEmailAddress = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.WindowsEmailAddress);

                userInfo.Sid = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.Sid);

                userInfo.LineServerURI = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.LineServerURI);
                
                userInfo.DisplayName = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.DisplayName);
                
                userInfo.LineURI = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.LineURI);

                userInfo.SipAddress = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.SipAddress);

                userInfo.Enabled = SkypeForBusinessHelper.GetBooleanProperty(psMemberInfo, SkypeForBusinessProperties.Enabled);

                userInfo.TenantId = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.TenantId);
                
                userInfo.TargetRegistrarPool = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.TargetRegistrarPool);

                userInfo.VoicePolicy = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.VoicePolicy);
                
                userInfo.ConferencingPolicy = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.ConferencingPolicy);
                
                userInfo.VoiceRoutingPolicy = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.VoiceRoutingPolicy);

                userInfo.RegistrarPool = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.RegistrarPool);

                userInfo.DialPlan = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.DialPlan);
                
                userInfo.ExternalAccessPolicy = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.ExternalAccessPolicy);

                userInfo.ConferencingPolicy = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.ConferencingPolicy);

                userInfo.HostingProvider = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.HostingProvider);

                userInfo.ObjectId = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.ObjectId);
                
                userInfo.Name = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.Name);

                userInfo.DistinguishedName = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.DistinguishedName);

                userInfo.Identity = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.Identity);

                userInfo.Guid = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.Guid);

                userInfo.ObjectCategory = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.ObjectCategory);

                userInfo.ObjectClass = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.ObjectClass);

                userInfo.WhenChanged = SkypeForBusinessHelper.GetProperty_DateTime(psMemberInfo, SkypeForBusinessProperties.WhenChanged);

                userInfo.WhenCreated = SkypeForBusinessHelper.GetProperty_DateTime(psMemberInfo, SkypeForBusinessProperties.WhenCreated);

                userInfo.OriginatingServer = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.OriginatingServer);
                

                userInfo.ObjectState = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.ObjectState);

                userInfo.OnPremLineURI = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.OnPremLineURI);

                if (!string.IsNullOrWhiteSpace(userInfo.LineURI) && userInfo.LineURI.Length > 3)
                {
                    var num = userInfo.LineURI.GetLastDigit(4);
                    int newnum;
                    if (int.TryParse(num, out newnum))
                    {
                        userInfo.LineURIExtension = newnum;
                    }
                    else
                    {
                        userInfo.LineURIExtension = null;
                    }
                }
                else
                {
                    userInfo.LineURIExtension = null;
                }

                userInfo.TeamsUpgradePolicy = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.TeamsUpgradePolicy);

                userInfo.OnlineVoiceRoutingPolicy = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.OnlineVoiceRoutingPolicy);
                if (!string.IsNullOrWhiteSpace(userInfo.OnlineVoiceRoutingPolicy))
                {
                    userInfo.OnlineVoiceRoutingPolicy = userInfo.OnlineVoiceRoutingPolicy.Trim();
                }

                userInfo.TenantDialPlan = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.TenantDialPlan);

                if(!string.IsNullOrWhiteSpace(userInfo.OnlineVoiceRoutingPolicy) && DCCPolicyName.Trim().ToUpper() == userInfo.OnlineVoiceRoutingPolicy.ToUpper())
                {
                    userInfo.IsValid = true;
                }
                else
                {
                    //Valid only LineURI exists
                    userInfo.IsValid = userInfo.LineURIExtension.HasValue;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("PopulateUser failed", ex);
            }
            return userInfo;
        }
        public static SkypeForBusinessUserInfo PopulateOnPremUser(PSObject psMemberInfo, long endpointId, SkypeForBusinessProperties SkypeForBusinessProperties)
        {
            var userInfo = new SkypeForBusinessUserInfo();
            try
            {
                userInfo.EndPointSystemId = endpointId;
                userInfo.SamAccountName = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.SamAccountName);
                userInfo.UserPrincipalName = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.UserPrincipalName);
                userInfo.FirstName = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.FirstName);
                userInfo.LastName = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.LastName);
                userInfo.WindowsEmailAddress = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.WindowsEmailAddress);

                userInfo.DisplayName = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.DisplayName);

                userInfo.LineURI = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.LineURI);
                userInfo.SipAddress = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.SipAddress);
                userInfo.Enabled = SkypeForBusinessHelper.GetBooleanProperty(psMemberInfo, SkypeForBusinessProperties.Enabled);
                userInfo.TenantId = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.TenantId);

                userInfo.TargetRegistrarPool = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.TargetRegistrarPool);
                userInfo.VoicePolicy = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.VoicePolicy);

                userInfo.ConferencingPolicy = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.ConferencingPolicy);

                userInfo.VoiceRoutingPolicy = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.VoiceRoutingPolicy);
                userInfo.RegistrarPool = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.RegistrarPool);
                userInfo.DialPlan = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.DialPlan);

                userInfo.ExternalAccessPolicy = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.ExternalAccessPolicy);
                userInfo.ConferencingPolicy = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.ConferencingPolicy);
                userInfo.HostingProvider = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.HostingProvider);

                userInfo.Name = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.Name);
                userInfo.DistinguishedName = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.DistinguishedName);
                userInfo.Identity = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.Identity);

                //Valid only LineURI exists
                

                if (!string.IsNullOrWhiteSpace(userInfo.LineURI) && userInfo.LineURI.Length > 3)
                {
                    var num = userInfo.LineURI.GetLastDigit(4);
                    int newnum;
                    if (int.TryParse(num, out newnum))
                    {
                        userInfo.LineURIExtension = newnum;
                    }
                    else
                    {
                        userInfo.LineURIExtension = null;
                    }
                }
                else
                {
                    userInfo.LineURIExtension = null;
                }
                userInfo.IsValid = userInfo.LineURIExtension.HasValue;
            }
            catch (Exception ex)
            {
                throw new Exception("PopulateUser failed", ex);
            }
            return userInfo;
        }
        public static SkypeForBusinessResponseGroupInfo PopulateResponseGroup(PSObject psMemberInfo, long endpointId, SkypeForBusinessProperties SkypeForBusinessProperties)
        {
            SkypeForBusinessResponseGroupInfo grpInfo = null;
            try
            {
                grpInfo = new SkypeForBusinessResponseGroupInfo();
                grpInfo.EndPointSystemId = endpointId;
                grpInfo.Name = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.Name);
                grpInfo.Identity = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.Identity);
                grpInfo.OwnerPool = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.OwnerPool);
                grpInfo.AgentsByUri = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.AgentsByUri);
                grpInfo.Description = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.Description);
                if (string.IsNullOrWhiteSpace(grpInfo.Description))
                {
                    grpInfo.Description = grpInfo.Name;
                }
                grpInfo.ParticipationPolicy = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.ParticipationPolicy);
                grpInfo.AgentAlertTime = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.AgentAlertTime);
                grpInfo.RoutingMethod = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.RoutingMethod);
                grpInfo.DistributionGroupAddress = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.DistributionGroupAddress);
            }
            catch (Exception ex)
            {
                throw new Exception("PopulateResponseGroup failed", ex);
            }
            return grpInfo;
        }
        public static SkypeForBusinessUnAssignedNumber PopulateUnAssignedNumber(PSObject psMemberInfo, long endpointId, SkypeForBusinessProperties SkypeForBusinessProperties)
        {
            SkypeForBusinessUnAssignedNumber unAssignedNumber = null;
            try
            {
                unAssignedNumber = new SkypeForBusinessUnAssignedNumber();
                unAssignedNumber.EndPointSystemId = endpointId;
                //unAssignedNumber.Identity = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.Identity);
                //unAssignedNumber.AnnouncementServiceId = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.AnnouncementServiceId);
                //unAssignedNumber.AnnouncementServerFqdn = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.AnnouncementServerFqdn);
                //unAssignedNumber.AnnouncementName = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.AnnouncementName);
                //unAssignedNumber.AnnouncementId = SkypeForBusinessHelper.GetGuidProperty(psMemberInfo, SkypeForBusinessProperties.AnnouncementId);
                //unAssignedNumber.NumberRangeStart = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.NumberRangeStart);
                //unAssignedNumber.NumberRangeEnd = SkypeForBusinessHelper.GetProperty(psMemberInfo, SkypeForBusinessProperties.NumberRangeEnd);
                unAssignedNumber.Priority = SkypeForBusinessHelper.GetIntegerProperty(psMemberInfo, SkypeForBusinessProperties.Priority);
            }
            catch(Exception ex)
            {
                throw new Exception("PopulateUnAssignedNumber failed", ex);
            }
            return unAssignedNumber;
        }
    }
}
