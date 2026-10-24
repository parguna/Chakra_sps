using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Table("DI_M_QuickLink")]
    public class QuickLink : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public long QuickLinkId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

    }
}
