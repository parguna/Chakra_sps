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
    [TableInfo("DI_OEBS_RoleInfo", "ORACLE EBS Role Info")]
    [Table("DI_OEBS_RoleInfo")]
    public class OracleEBSRoleInfo:IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        /*
         NAME, DISPLAY_NAME, DESCRIPTION, NOTIFICATION_PREFERENCE, LANGUAGE, TERRITORY, EMAIL_ADDRESS, FAX, 
                            ORIG_SYSTEM, ORIG_SYSTEM_ID, START_DATE, STATUS, EXPIRATION_DATE, SECURITY_GROUP_ID, USER_FLAG, PARTITION_ID, CREATED_BY,
                            CREATION_DATE, LAST_UPDATED_BY, LAST_UPDATE_DATE, LAST_UPDATE_LOGIN, OWNER_TAG, PARENT_ORIG_SYSTEM, PARENT_ORIG_SYSTEM_ID
        */
        public string NAME { get; set; }
        public string DISPLAY_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string NOTIFICATION_PREFERENCE { get; set; }
        public string LANGUAGE { get; set; }
        public string TERRITORY { get; set; }
        public string EMAIL_ADDRESS { get; set; }
        public string FAX { get; set; }
        public string ORIG_SYSTEM { get; set; }
        public double? ORIG_SYSTEM_ID { get; set; }
        public DateTime? START_DATE { get; set; }
        public string STATUS { get; set; }
        public DateTime? EXPIRATION_DATE { get; set; }
        public double? SECURITY_GROUP_ID { get; set; }
        public string USER_FLAG { get; set; }
        public double? PARTITION_ID { get; set; }
        public double? CREATED_BY { get; set; }
        public DateTime? CREATION_DATE { get; set; }
        public double? LAST_UPDATED_BY { get; set; }
        public DateTime? LAST_UPDATE_DATE { get; set; }
        public double? LAST_UPDATE_LOGIN { get; set; }
        public string OWNER_TAG { get; set; }
        public string PARENT_ORIG_SYSTEM { get; set; }
        public double? PARENT_ORIG_SYSTEM_ID { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? version { get; set; }

        public virtual IList<OracleEBSUserInfo> OracleEBSUsers { get; set; }

    }
}
