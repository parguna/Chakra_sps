using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class Role : DtoBase 
    {
        //public Role()
        //{
        //    this.Users = new List<User>();
        //}

        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public long? RoleOwner { get; set; }
        public bool IsDefaultRole { get; set; }
        public List<User> Users { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }             
        public User Owner { get; set; }
    }
}
