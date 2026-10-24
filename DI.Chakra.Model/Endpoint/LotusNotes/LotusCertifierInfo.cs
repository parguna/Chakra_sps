using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_LNS_Certifiers")]
    public class LotusCertifierInfo : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string CertifierType { get; set; }
        public string Owner { get; set; }
        public string FullName { get; set; }
        public string FormIssuedBy { get; set; }
        public string MailDomain { get; set; }
        public int CertificateType { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int? Version { get; set; }
    }
}
