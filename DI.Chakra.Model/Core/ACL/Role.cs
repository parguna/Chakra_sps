using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_Role")]
    public class Role : IAggregateRoot, IComparable<Role>,IEquatable<Role>
    {
        public Role()
        {
            this.Users = new List<User>();
        }

        [Key]
        public long Id { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public long? RoleOwner { get; set; }
        public bool IsDefaultRole { get; set; }
        public virtual List<User> Users { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

     

        [ForeignKey("RoleOwner")]
        public virtual User Owner { get; set; }
       
        public override string ToString()
        {
            return RoleName.ToString();
        }
        public static Role CreateRole()
        {
            Role _role = new Role();

            _role.CreatedDate = DateTime.Now;
            _role.LastModifiedDate = DateTime.Now;

            return _role;

        }

         

        #region IComparable<Role> Members

        public int CompareTo(Role other)
        {
            if (other == null)
                return -1;

            return this.RoleName.CompareTo(other.RoleName);
        }

        #endregion

        #region IEquatable<Role> Members

        public bool Equals(Role other)
        {
            if (other == null)
                return false;

            if (!string.IsNullOrEmpty(this.RoleName) && !string.IsNullOrEmpty(other.RoleName))
            {
                return this.RoleName.ToUpper().Trim().Equals(other.RoleName.ToUpper().Trim());
            }
            else
                return false;
            
        }

        #endregion
    }
}
