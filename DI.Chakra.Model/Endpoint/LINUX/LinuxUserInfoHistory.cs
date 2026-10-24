using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_Linux_UserInfoHistory")]
    public class LinuxUserInfoHistory : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public int HistoryId { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string LinuxUserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string GroupId { get; set; }
        public string UserIdInfo { get; set; }
        public string HomeDirectory { get; set; }
        public string CommandShell { get; set; }
        public int? DIUserId { get; set; }
        public bool SpecialAccount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int? Version { get; set; }
        public AccountType AccountType { get; set; }

    }
}
