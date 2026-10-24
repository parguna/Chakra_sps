using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.Collections;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Model.SAP
{
    [Table("DI_SAP_ROLEINFO")]
    [Serializable]
    public class SAPRoleInfo : IAggregateRoot, IEndpointEntitlementInfo
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string ROLE_CODE { get; set; }
        public string ROLE_DESC { get; set; }
        public string TargetSysId { get; set; }
        public string RoleType { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual List<SAPUserinfo> SAPUserinfo { get; set; }
        public int? Version { get; set; }

        public string EntitlementName
        {
            get { return ROLE_CODE; }
        }

        public string EntitlementDesc
        {
            get { return ROLE_DESC; }
        }

        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Role; }
        }
    }
}
