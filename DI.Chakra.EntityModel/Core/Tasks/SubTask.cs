using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_SubTask")]
    public class SubTask : EntityBase,ICreatable,IUpdatable
    {
        public string RoleName { get; set; }

        public string TCode { get; set; }

        public string TCodeDescription { get; set; }

        public string Remarks { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public long OwnerId;
    }
}
