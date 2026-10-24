using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAPHANA_XLCriticalTCodeMatrix")]
    public class SAPHANAXLCriticalTCodeMatrix: EntityBase, ICreatable, IUpdatable
    {
        public long EndpointSystemId { get; set; }
        public string Application { get; set; }
        public string Transaction { get; set; }
        public string TransactionDescription { get; set; }
        public string TransactionOwner { get; set; }
        public DateTime? ValidFrom
        {
            get;
            set;
        }
        public DateTime? ValidTo
        {
            get;
            set;
        }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
