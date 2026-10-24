using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Table("DI_M_UserDashboardPreference")]
    public class UserDashboardPreference : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public long DashboardId { get; set; }
        public int Position { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("DashboardId")]
        public virtual Dashboard Dashboard { get; set; }
    }
}
