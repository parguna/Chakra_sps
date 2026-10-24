using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_LNS_MailQuotaConfig")]
    public class LNSMailQuotaConfig : IXLData
    {
        [Key]
        public long Id { get; set; }
        public string Position { get; set; }
        public int MailQuotaSizeInMB { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }
    }
}
