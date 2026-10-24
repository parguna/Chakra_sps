using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model.SAP
{
    [TableInfo("DI_SAP_Configuration", "DI_SAP_Configuration")]
    [Table("DI_SAP_Configuration")]
    public partial class SAPConfiguration : IEndpointConfiguration
    {
        [Key]
        public long Id { get; set; }
        public long EndPointSystemId { get; set; }
        public string HostName { get; set; }
        public string IPAddress { get; set; }
        public string DomainName { get; set; }
        public string ServerName { get; set; }
        public string DataSourceName { get; set; }
        public Nullable<int> ClientNumber { get; set; }
        public Nullable<int> SystemNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Nullable<bool> MapAll { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("EndPointSystemId")]
        public virtual EndPoint EndpointSystem { get; set; }

    }
}
