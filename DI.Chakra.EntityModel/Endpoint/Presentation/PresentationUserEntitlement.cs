using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationUserEntitlement
    {
        public long EndpointUserId { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; } 

        public string Remarks1 { get; set; }

        public EndpointType EndPointSystemType { get; set; }

        public string EndpointSystemName { get; set; }
        public string EndpointSystemCode { get; set; }
        public long EndpointSystemId { get; set; }

        public DateTime? ReviewDate { get; set; }

        public bool? CanRequest { get; set; }

        public string UserDefinedDesc { get; set; }

        public long EntitlementId { get; set; }

        public EntitlementType EntitlementType { get; set; }

        public long? ParentEntitlementId { get; set; }

        public bool IsSODConflict { get; set; }

        public bool IsSensitiveAccess { get; set; }

        public Nullable<long> WogUserId { get; set; }
        public string CanRequestString
        {
            get { return CanRequest != null ? Convert.ToBoolean(CanRequest) ? "Yes" : "No" : "Not defined"; }
        }
        
        public string UserDefinedDisplayName { get; set; }

        public Nullable<long> Level1Approver { get; set; }

        public Nullable<long> Level2Approver { get; set; }

        public Nullable<bool> IsLevel1Group { get; set; }

        public Nullable<bool> IsLevel2Group { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public bool IsValid { get; set; }

        public string ValidationMessage { get; set; }

        public long Id { get; set; }

        public string Approver1Name { get; set; }
        public string Approver2Name { get; set; }
        public string Remarks2 { get; set; }
        public string Remarks3 { get; set; }
        public string Remarks4 { get; set; }
        public string Remarks5 { get; set; }
        public string Remarks6 { get; set; }
        public string Remarks7 { get; set; }
        public string Remarks8 { get; set; }
        public string Remarks9 { get; set; }
        public string Remarks10 { get; set; }
        public AccountType AccountType { get; set; }
        public string AccountTypeString { get; set; }
        public Nullable<DateTime> LastAttestedOn { get; set; }
    }

    public class PresentationBusinessRoleDetail
    {
        public long Id { get; set; }
        public long EndpointSystemId { get; set; }
        public long EndpointUserId { get; set; }
        public string BusinessRole { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string Module { get; set; }
        public string SubModule { get; set; }
        public string Remarks { get; set; }
        public DateTime? ValidToDate { get; set; }
    }
    [NotMapped]
    public class PresentationFlinsIndirectRole
    {
        public long Id { get; set; }
        public long EntitlementId { get; set; }
        public long EndpointUserId { get; set; }
        public string Entitlement{ get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public DateTime? ValidToDate { get; set; }
        public DateTime? LastAttestedOn { get; set; }
    }
}
