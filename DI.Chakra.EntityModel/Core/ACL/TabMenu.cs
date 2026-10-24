using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_TabMenu")]
    public class TabMenu : EntityBase, ICreatable, IUpdatable
    {
        public string UniqueName { get; set; }

        public string MenuName { get; set; }

        public long? ParentTabMenuId { get; set; }

        public long? PageId { get; set; }

        [ForeignKey("PageId")]
        public WebPage WebPage { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
