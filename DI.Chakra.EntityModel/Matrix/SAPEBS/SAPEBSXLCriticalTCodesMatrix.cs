using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_EBS_XL_CriticalTCodesMatrix")]
    public class SAPEBSXLCriticalTCodesMatrix : EntityBase, ICreatable, IUpdatable
    {
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
