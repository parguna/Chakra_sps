using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [TableInfo("DI_ADS_ComputerGroup", "DI_ADS_ComputerGroup")]
    [Table("DI_ADS_ComputerGroup")]
    public class ADSComputerGroup : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndpointSystemId { get; set; }
        public string ComputerCN { get; set; }
        public string ComputerDistinguishedName { get; set; }
        public string GroupCN { get; set; }
        public string GroupDistinguishedName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int? Version { get; set; }

    }
}
