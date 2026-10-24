using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [TableInfo("DI_Oracle11gUM_Configuration", "DI_Oracle11gUM_Configuration")]
    [Table("DI_Oracle11gUM_Configuration")]
    public class OracleUMConfiguration : IAggregateRoot, IEndpointConfiguration
    {
        [Key]
        public long Id { get; set; }
        public long EndpointSystemId { get; set; }
        public string HostName { get; set; }
        public string IPAddress { get; set; }
        public string DomainName { get; set; }
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string InstanceName { get; set; }
        public int PortNo { get; set; }
        public bool ImportUsers { get; set; }
        public bool ImportRoles { get; set; }
        public bool ImportTables { get; set; }
        public bool? MapAll { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        [ForeignKey("EndpointSystemId")]
        public virtual EndPoint EndpointSystem { get; set; }
    }
}
