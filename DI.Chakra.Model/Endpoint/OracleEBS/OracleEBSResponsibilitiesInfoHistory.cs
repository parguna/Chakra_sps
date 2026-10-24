#region NAMESPACE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DI.Chakra.Infrastructure;

#endregion

namespace DI.Chakra.Model
{
    [TableInfo("DI_OEBS_ResponsibilitiesInfoHistory", "ORACLE EBS Responsibilities Info")]
    [Table("DI_OEBS_ResponsibilitiesInfoHistory")]
    public class OracleEBSResponsibilitiesInfoHistory : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }

        public int ResponsibilityId { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string ResponsibilityName { get; set; }
        //public string PasswordRequired { get; set; }
        //public string AuthenticationType { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
