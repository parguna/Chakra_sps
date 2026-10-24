using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [TableInfo("DI_Oracle11gUM_UserTable", "Oracle11gUMUserTable")]
    [Table("DI_Oracle11gUM_UserTable")]
    public class OracleUMUserTable
    {
        [Key]
        public long Id { get; set; }
        public int UserId { get; set; }
        public int TableId { get; set; }
    }
}
