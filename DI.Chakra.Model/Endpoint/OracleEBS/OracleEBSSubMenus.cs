using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DI.Chakra.Infrastructure;

namespace DI.Chakra.Model
{
    [TableInfo("DI_OEBS_SubmenusInfo", "ORACLE EBS Submenus Info")]
    [Table("DI_OEBS_SubmenusInfo")]
    public class OracleEBSSubmenusInfo
    {
        [Key]
        public long Id { get; set; }
        public string LANGUAGE { get; set; }
        [Description("Parent  Menu Id")]
        public Double? MENU_ID { get; set; }
        public int? ENTRY_SEQUENCE { get; set; }
        public DateTime? LAST_UPDATE_DATE { get; set; }
        public Double? LAST_UPDATE_BY { get; set; }
        public Double? LAST_UPDATE_LOGIN { get; set; }
        public DateTime? CREATION_DATE { get; set; }
        public Double? CREATED_BY { get; set; }
        public string PROMPT { get; set; }
        public string DESCRIPTION { get; set; }
        public string SOURCE_LANG { get; set; }
        public int? SECURITY_GROUP_ID { get; set; }

        public long EndPointSystemId { get; set; }
        public string TenantId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

    }
}
