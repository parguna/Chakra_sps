using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [TableInfo("DI_M_UnMappedUsers", "DI_M_UnMappedUsers")]
    [Table("DI_M_UnMappedUsers")]
    public class UnMappedUsers : IAggregateRoot
    {
        [Key]
        public long UnMappedUserId { get; set; }
        public long UserId { get; set; }
        public long EndPointSystemId { get; set; }
        public int ProcessId { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public int? MappedUserId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        //public ADSUserInfo AdUsername { get; set; }
        public string UserName;// { get; set; }
        public string DisplayName;
        public string MuserName;// { get; set; }
        public string MDisplayName;
    }

}
