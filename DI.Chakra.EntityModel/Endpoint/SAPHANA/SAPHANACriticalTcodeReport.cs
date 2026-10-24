using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAPHANA_CriticalTcodeReport")]
    public class SAPHANACriticalTcodeReport : EntityBase, ICreatable, IUpdatable
    {
        public long EndPointSystemId { get; set; }
        public string EndpointSystemName { get; set; }
        public string AccountName { get; set; }
        public string Displayname { get; set; }
        public string Transaction { get; set; }
        public string Description { get; set; }
        public string BusinessRole { get; set; }
        public string TechnicalRole { get; set; }
        public string FromPosition { get; set; }
        public AccountType AccountType { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }
        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPointSystem { get; set; }
        public long? AccountOwnerId { get; set; }
        public string TransactionOwner { get; set; }
        public string UserName { get; set; }
        public string UserDisplayName { get; set; }
    }
}
