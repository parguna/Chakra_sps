using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


using DI.Chakra.Infrastructure;


namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_Group")]
    public class Group : IAggregateRoot
    {
        public Group()
        {
            this.Users = new List<User>();
        }
        [Key]
        public long Id { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }        
        public long? GroupOwner { get; set; }
        public bool IsDefaultGroup { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public virtual List<User> Users { get; set; }

        [ForeignKey("GroupOwner")]
        public virtual User Owner { get; set; }
        
    }
}
