using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{

    [Serializable]
    [Table("DI_M_Permission")]
    public class Permission
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string IconURL { get; set; }
        public string ClientScript { get; set; }
        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
