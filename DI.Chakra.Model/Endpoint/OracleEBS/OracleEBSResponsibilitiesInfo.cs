#region NAMESPACE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DI.Chakra.Infrastructure;

#endregion

namespace DI.Chakra.Model
{
    [TableInfo("DI_OEBS_ResponsibilitiesInfo", "ORACLE EBS Responsibilities Info")]
    [Table("DI_OEBS_ResponsibilitiesInfo")]
    public class OracleEBSResponsibilitiesInfo : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        //public string ResponsibilityName { get; set; }

       
        public double APPLICATION_ID { get; set; }
        public double RESPONSIBILITY_ID { get; set; }
        public double? LAST_UPDATED_BY { get; set; }
        public DateTime? LAST_UPDATE_DATE { get; set; }
        public double CREATED_BY { get; set; }
        public DateTime? CREATION_DATE { get; set; }
        public double DATA_GROUP_APPLICATION_ID { get; set; }
        public double DATA_GROUP_ID { get; set; }
        public double MENU_ID { get; set; }
        public DateTime? START_DATE { get; set; }
        public DateTime? END_DATE { get; set; }
        public int GROUP_APPLICATION_ID { get; set; }
        public int REQUEST_GROUP_ID { get; set; }
        public string OEBS_VERSION { get; set; }
        public string WEB_HOST_NAME { get; set; }
        public string WEB_AGENT_NAME { get; set; }
        public string RESPONSIBILITY_KEY { get; set; }
        public int? SECURITY_GROUP_ID { get; set; }

        public string LANGUAGE { get; set; }
        public string RESPONSIBILITY_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public double LAST_UPDATE_LOGIN { get; set; }

        public string APPLICATION_SHORT_NAME { get; set; }

        //public double ROWID { get; set; }
       
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        //public int Version { get; set; }

        public virtual IList<OracleEBSUserInfo> OracleEBSUsers { get; set; }


    }
}
