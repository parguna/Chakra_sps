using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DI.Chakra.Model
{
    //[TableInfo("DI_SAPEP_RoleInfoHistory", "SAPEPRoleInfoHistory")]
    [Table("DI_SAPEP_RoleInfoHistory")]
    public class SAPEPRoleInfoHistory : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public int HistoryId { get; set; }
        public string TenantId { get; set; }
        public Nullable<int> EndPointSystemId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public string Extar1 { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string RecordStatus { get; set; }
        public int? Version { get; set; }


        public override string ToString()
        {
            return this.RoleId.ToString();
        }
        public static SAPEPRoleInfoHistory CreateSAPEPRoleInfoHistory()
        {
            SAPEPRoleInfoHistory sapEPRoleInfoHistory = new SAPEPRoleInfoHistory();
            return sapEPRoleInfoHistory;
        }

        public override bool Equals(object obj)
        {
            SAPEPRoleInfoHistory other = obj as SAPEPRoleInfoHistory;
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
    }
}
