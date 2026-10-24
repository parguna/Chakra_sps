using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_QuickLink")]
    public class QuickLink : EntityBase,ICreatable,IUpdatable
    {
        public string Name { get; set; }

        public string Category { get; set; }

        public long QuickLinkId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string LastModifiedBy { get; set; }


    }
}
