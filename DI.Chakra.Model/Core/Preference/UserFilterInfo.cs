using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    /// <summary>
    /// Table used to persist RadGrid  Filter Selection values selected by user. 
    /// Created by Harihara : 16/6/2015
    /// </summary>
    [Serializable]
    [TableInfo("DI_M_UserFilterInfo", "DI_M_UserFilterInfo")]
    [Table("DI_M_UserFilterInfo")]
    public class UserFilterInfo
    {
        [Key]
        public long Id { get; set; }
        public string PagePath { get; set; }
        public string  GridId { get; set; }
        public long UserId { get; set; }
        public string Operand { get; set; }
        public string ColumnName { get; set; }
        public string FilterValue { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        [ForeignKey("UserId")]
        public virtual User Users { get; set; }

    }
}
