using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.GridSettings
{
    [Serializable]
    [Table("DI_M_UserGridFilterSettings")]
    public class UserGridFilterSettings
    {
        [Key]
        public long ID { get; set; }
        public long UserId { get; set; }
        public string PagePath { get; set; }
        public string GridID { get; set; }
        public string Operand { get; set; }
        public string ColumnName { get; set; }
        public string FilterValue { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
