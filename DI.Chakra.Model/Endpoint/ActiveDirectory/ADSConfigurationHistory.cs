using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DI.Chakra.Model
{
    [Table("DI_ADS_ConfigurationHistory")]
    public class ADSConfigurationHistory : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }

        public long ADSConfigurationId { get; set; }       
        public string IPAddress { get; set; }
        public string DomainName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Ou { get; set; }
        public Nullable<bool> MapAll { get; set; }

        public string AuditAction { get; set; }
        public string AuditUser { get; set; }
        public DateTime AuditDate { get; set; }
        [ForeignKey("ADSConfigurationId")]
        public virtual ADSConfiguration ADSConfiguration { get; set; }
    }
}
