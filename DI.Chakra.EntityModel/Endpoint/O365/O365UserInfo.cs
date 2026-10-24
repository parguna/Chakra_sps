using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_Office365_UserInfo")]
    public class Office365UserInfo : EntityBase, ICreatable, IUpdatable
    {
        public Office365UserInfo()
        {
            this.O365Groups = new List<Office365GroupInfo>();
        }

        public long EndPointSystemId { get; set; }
        public string City { get; set; }
        public string Company { get; set; }
        public string ConsumerNetID { get; set; }
        public string CountryOrRegion { get; set; }
        public string Department { get; set; }
        public string DisplayName { get; set; }
        public string DistinguishedName { get; set; }
        public string ExchangeVersion { get; set; }
        public string Fax { get; set; }
        public string FirstName { get; set; }
        public string HomePhone { get; set; }
        public string Identity { get; set; }
        public bool? IsLinked { get; set; }
        public bool? IsSecurityPrincipal { get; set; }
        public bool? IsValid { get; set; }
        public string LastName { get; set; }
        public string LinkedMasterAccount { get; set; }
        public string Manager { get; set; }
        public string MicrosoftOnlineServicesID { get; set; }
        public string MobilePhone { get; set; }
        public string Name { get; set; }
        public string NetID { get; set; }
        public string Office { get; set; }
        public string OrganizationalUnit { get; set; }
        public string OrganizationId { get; set; }
        public string Phone { get; set; }
        public string SamAccountName { get; set; }
        public string Sid { get; set; }
        public string SimpleDisplayName { get; set; }
        public string SKUAssigned { get; set; }
        public string Title { get; set; }
        public string UserAccountControl { get; set; }
        public string UserPrincipalName { get; set; }
        public DateTime? WhenCreated { get; set; }
        public string WindowsEmailAddress { get; set; }
        public string WindowsLiveID { get; set; }
        public int PwdResetCnt { get; set; }
        public string mail { get; set; }
        public string MailboxUsage { get; set; }
        public DateTime? LastLogon { get; set; }
        public string ValidationStatus { get; set; }
        public bool? LicenseReconciliationNeeded { get; set; }
        public string UsageLocation { get; set; }
        public string Alias { get; set; }
        public string RetentionPolicy { get; set; }
        public bool? LitigationHoldEnabled { get; set; }
        public bool? RetentionHoldEnabled { get; set; }
        public string ForwardingSmtpAddress { get; set; }
        public string LitigationHoldDuration { get; set; }
        public bool IsMailboxEnabled { get; set; }
        public string Languages { get; set; }
        public string RoleAssignmentPolicy { get; set; }
        public string SharingPolicy { get; set; }
        public string ArchiveName { get; set; }
        public string ArchiveStatus { get; set; }
        public string RecipientType { get; set; }
        public string SendAs { get; set; }
        public string SendonBehalf { get; set; }
        public string FullAccess { get; set; }
        public string AccountSkuId { get; set; }
        public string ServicePlanInfo { get; set; }
        public bool IsSkypeOnPrem { get; set; }
        public bool IsShared { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public List<Office365GroupInfo> O365Groups { get; set; }

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
    }
}
