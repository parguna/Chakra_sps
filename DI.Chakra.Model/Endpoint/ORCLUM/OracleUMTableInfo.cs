using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [TableInfo("DI_Oracle11gUM_TableInfo", "Oracle11gUMTableInfo")]
    [Table("DI_Oracle11gUM_TableInfo")]
    public class OracleUMTableInfo : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string Owner { get; set; }
        public string TableName { get; set; }
        public string TableSpaceName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public virtual IList<OracleUMUserInfo> Users { get; set; }


    }
}
