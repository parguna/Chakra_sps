using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_Page")]
    public class WebPage : EntityBase, ICreatable, IUpdatable
    {
        public string UniqueName { get; set; }

        public string Name { get; set; }

        public long? PageUrlId { get; set; }

        [ForeignKey("PageUrlId")]
        public PageUrl PageUrl { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
