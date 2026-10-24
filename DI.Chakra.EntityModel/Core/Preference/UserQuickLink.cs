using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_UserQuickLink")]
    public  class UserQuickLink:EntityBase,ICreatable,IUpdatable
    {

        public long UserId { get; set; }

        public string Name { get; set; }

        public long QuickLinkId { get; set; }

        public int Position { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        [ForeignKey("UserId")]
        public  User User { get; set; }

        [ForeignKey("QuickLinkId")]
        public  QuickLink QuickLink { get; set; }
    }
}
