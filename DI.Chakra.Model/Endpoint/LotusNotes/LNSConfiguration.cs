using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
 
    [Table("DI_LNS_Configuration")]
    public partial class LNSConfiguration : IEndpointConfiguration
    {
        [Key]
        public long Id { get; set; }
        public long EndPointSystemId { get; set; }
        public string HostName { get; set; }
        public string IPAddress { get; set; }
        public string DomainName { get; set; }
        public string ServerName { get; set; }
        public string Password { get; set; }
        public string RootCertificate { get; set; }
        public string DenyAllAccessGroupName { get; set; }
        public string IDVaultAdmin { get; set; }
        public string IDVaultAdminFile { get; set; }
        public string MailDomain { get; set; }
        public string MailServer { get; set; }
        
        public Nullable<bool> MapAll { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public virtual EndPoint EndpointSystem { get; set; }

    }
}
