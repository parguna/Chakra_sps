using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_EndPointAutoExpiry")]
    public class EndPointAutoExpiry : EntityBase, ICreatable, IUpdatable
    {
        public long EndpointSystemTypeId { get; set; }

        public Nullable<long>EndPointSystemId { get; set; }

        public bool IsAccountAutoExpirySupported { get; set; }

        public bool IsEntitlementAutoExpirySupported { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public bool IsAutoExpiryApprovalRequired { get; set; }
    }
}
