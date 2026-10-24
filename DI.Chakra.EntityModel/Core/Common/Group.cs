using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_Group")]
    public class Group : EntityBase, ICreatable, IUpdatable
    {      
        public string GroupName { get; set; }

        public string GroupDescription { get; set; }

        public string EmailId { get; set; }

        public long? GroupOwner { get; set; }

        public bool IsDefaultGroup { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public List<User> Users { get; set; }

        [ForeignKey("GroupOwner")]
        public User Owner { get; set; }
    }
}
