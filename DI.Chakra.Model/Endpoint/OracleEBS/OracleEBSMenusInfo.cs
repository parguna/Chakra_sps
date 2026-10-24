
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DI.Chakra.Infrastructure;

namespace DI.Chakra.Model
{
    [TableInfo("DI_OEBS_MenusInfo", "ORACLE EBS Menus Info")]
    [Table("DI_OEBS_MenusInfo")]
   public class OracleEBSMenusInfo
    {
        [Key]
        public long Id { get; set; }
        public double? MENU_ID { get; set; }
        public string MENU_NAME { get; set; }
       
        public double? LAST_UPDATED_BY { get; set; }
        public DateTime? LAST_UPDATE_DATE { get; set; }
        public double? LAST_UPDATE_LOGIN { get; set; }
        public double CREATED_BY { get; set; }
        public DateTime? CREATION_DATE { get; set; }
       // public string TYPE { get; set; }
        public int? SECURITY_GROUP_ID { get; set; }

        public string LANGUAGE { get; set; }
        public string SOURCE_LANG { get; set; }
        public string DESCRIPTION { get; set; }

        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

    }
}
