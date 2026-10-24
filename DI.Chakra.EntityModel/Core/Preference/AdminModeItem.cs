using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_AdminModeItem")]
    public class AdminModeItem : EntityBase,ICreatable,IUpdatable
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public long TabMenuId { get; set; }

        public string Icon { get; set; }

        public string Information { get; set; }

        public AdminModeItemType Type { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string TableName { get; set; }

        [ForeignKey("TabMenuId")]
        public TabMenu TabMenu { get; set; }
    }
}
