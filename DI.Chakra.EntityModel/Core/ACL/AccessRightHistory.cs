using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_AccessRightHistory")]
    public class AccessRightHistory : EntityBase, ICreatable, IUpdatable 
    {
        public int AccessRightsId { get; set; }
        public Nullable<long> RoleId { get; set; }
        public Nullable<int> TabMenuId { get; set; }
        public string UserAction { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
