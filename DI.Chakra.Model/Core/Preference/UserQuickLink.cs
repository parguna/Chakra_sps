using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DI.Chakra.Model
{
    [Table("DI_M_UserQuickLink")]
    public class UserQuickLink : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public long QuickLinkId { get; set; }
        public int Position { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("QuickLinkId")]
        public virtual QuickLink QuickLink { get; set; }
    }
}
