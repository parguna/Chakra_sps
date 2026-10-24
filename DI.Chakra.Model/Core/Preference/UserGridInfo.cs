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
    /// Table used to persist RadGrid Page Size,Index,Filter expression values selected by user. 
    /// Created by Harihara : 16/6/2015
    /// </summary>
    [Serializable]
    [TableInfo("DI_M_UserGridInfo", "DI_M_UserGridInfo")]
    [Table("DI_M_UserGridInfo")]
    public class UserGridInfo
    {
        [Key]
        public long Id { get; set; }
        public string PagePath { get; set; }
        public string GridId { get; set; }
        public long UserId { get; set; }
        public string SortDirection { get; set; }
        public string SortColumnName { get; set; }
        public string FilterExpression { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        [ForeignKey("UserId")]
        public virtual User Users { get; set; }
    }
}
