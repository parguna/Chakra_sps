#region NAMESPACE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using DI.Chakra.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace DI.Chakra.Model
{
    [Table("DI_Oracle11gUM_UserRole")]
    public class OracleUMUserRole:IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public DateTime? DateModified { get; set; }
        public string LastUser { get; set; }

        public int EndpointTypeSystemId { get; set; }
        public int Version { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("UserId")]
        public virtual OracleUMUserInfo OracleUMUserInfo { get; set; }
       

    }
}
