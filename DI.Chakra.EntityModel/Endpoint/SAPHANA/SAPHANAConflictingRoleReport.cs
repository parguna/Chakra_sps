using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAPHANA_ConflictingRoleReport")]
    public class SAPHANAConflictingRoleReport : EntityBase, ICreatable, IUpdatable
    {
        public long EndPointSystemId { get; set; }
        public string EndpointSystemName { get; set; }
        public string AccountName { get; set; }
        public string Displayname { get; set; }
        public string BusinessRole { get; set; }
        public string TechnicalRole { get; set; }
        public string From_Position { get; set; }
        public string ConflictingPosition { get; set; }
        public string ConflictingRole { get; set; }
        public string ConflictingTechnicalRole  { get; set; }
        public string SoDRemarks { get; set; }
        public AccountType AccountType { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }
        public long? AccountOwnerId { get; set; }
        public string SODOwner { get; set; }
        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPointSystem { get; set; }
    }
}
