using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_SAP_XLCriticalTCodeMatrix")]
    public class SAPCriticalTCodeMatrix : IXLData
    {
        [Key]
        public long Id { get; set; }
        public string Transaction { get; set; }
        public string TransactionDescription { get; set; }
        public string TransactionOwner { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }
    }
}
