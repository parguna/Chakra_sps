#region NAMESPACE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion


namespace DI.Chakra.Model
{
    [TableInfo("DI_Oracle11gUM_UserTableMap", "Oracle11gUMUMUserTableMap")]
    [Table("DI_Oracle11gUM_UserTableMap")]
    public class OracleUMUserTableMap : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public int UserId { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string UserName { get; set; }
        public string TableName { get; set; }
        public string TableSpaceName { get; set; }
        public string ClusterName { get; set; }
        public string IOTName { get; set; }
        public string Status { get; set; }
        public DateTime? LastAnalyzed { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

    }
}
