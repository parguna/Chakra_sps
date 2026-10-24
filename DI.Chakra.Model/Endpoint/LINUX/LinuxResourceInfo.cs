using DI.Chakra.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [TableInfo("DI_Linux_ResourceInfo", "LinuxResourceInfo")]
    [Table("DI_Linux_ResourceInfo")]
    public class LinuxResourceInfo : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string Resource_Name { get; set; }
        public string Resource_Type { get; set; }
        public string Resource_User { get; set; }
        public string Resource_Group { get; set; }
        public string Resource_Parent { get; set; }
        public int Resource_Level { get; set; }
        public string Resouce_AccessRights { get; set; }
        public string Resouce_Size { get; set; }
        public string Resouce_LastModified { get; set; }
        public string MountDrive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? Version { get; set; }

        public string Mount;

    }
}

