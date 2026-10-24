using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DI.Chakra.Model
{
    //[TableInfo("DI_SAPEP_RoleInfo", "DI_SAPEP_RoleInfo")]
    [Table("DI_SAPEP_RoleInfo")]
    public class SAPEPRoleInfo : IAggregateRoot, IComparer<SAPEPRoleInfo>, IXLData
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public Nullable<long> EndPointSystemId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public string UniqueRoleName { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public int? Version { get; set; }
        public bool IsXLUploaded { get; set; }

        public virtual IList<SAPEPUserInfo> SAPEPUserInfos { get; set; }

        public override string ToString()
        {
            return this.RoleId.ToString();
        }
        public static SAPEPRoleInfo CreateSAPEPRoleInfo()
        {
            SAPEPRoleInfo sapEPRoleInfo = new SAPEPRoleInfo();
            return sapEPRoleInfo;
        }

        public override bool Equals(object obj)
        {
            SAPEPRoleInfo other = obj as SAPEPRoleInfo;
            if (obj != null)
            {
                return this.RoleId.Equals(other.RoleId);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.RoleId.GetHashCode() ^ this.RoleName.GetHashCode();
        }
        public int Compare(SAPEPRoleInfo x, SAPEPRoleInfo y)
        {
            int cmprValue = -1;
            if (x != null && y != null)
            {

                cmprValue = string.Compare(y.RoleName, x.RoleName);
            }

            return cmprValue;
        }

        [NotMapped]
        public DateTime? ValidFrom { get; set; }
        [NotMapped]
        public DateTime? ValidTo { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }

    }
}
