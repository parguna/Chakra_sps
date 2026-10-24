using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_AccessRequestConfig")]
    public class AccessRequestConfiguration : EntityBase,ICreatable,IUpdatable
    {
        public long EndpointSystemId { get; set; }
        public DI.Chakra.Infrastructure.Enums.AccountType AccountType { get; set; }
        public DI.Chakra.Infrastructure.Enums.AdHocActionType EnableActionType { get; set; }
        public EndPoint EndPointSystem { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
