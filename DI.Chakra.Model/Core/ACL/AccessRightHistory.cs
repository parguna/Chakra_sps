using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_M_AccessRightHistory")]
    public class AccessRightHistory
    {
        [Key]
        public long Id { get; set; }
        public int AccessRightsId { get; set; }
        public Nullable<long> RoleId { get; set; }
        public Nullable<int> TabMenuId { get; set; }
        public string UserAction { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
