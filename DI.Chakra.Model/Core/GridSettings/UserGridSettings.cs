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
    [Table("DI_M_UserGridSettings")]
    public class UserGridSettings
    {
        [Key]
        public long ID { get; set; }
        public string PagePath { get; set; }
        public string GridID { get; set; }
        public string SortColumnName { get; set; }
        public string SortDirection { get; set; }
        public string FilterCondition { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
