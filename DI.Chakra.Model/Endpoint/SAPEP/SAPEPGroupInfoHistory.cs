using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DI.Chakra.Model
{
    //[TableInfo("DI_SAPEP_GroupInfoHistory", "SAPEPGroupInfoHistory")]
    [Table("DI_SAPEP_GroupInfoHistory")]
    public class SAPEPGroupInfoHistory : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public int HistoryId { get; set; }
        public string TenantId { get; set; }
        public Nullable<int> EndPointSystemId { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public string Extar1 { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public int? Version { get; set; }

        public string RecordStatus { get; set; }

        public override string ToString()
        {
            return this.GroupId.ToString();
        }
        public static SAPEPGroupInfoHistory CreateSAPEPGroupInfoHistory()
        {
            SAPEPGroupInfoHistory sapEPGroupInfoHistory = new SAPEPGroupInfoHistory();
            return sapEPGroupInfoHistory;
        }

        public override bool Equals(object obj)
        {
            SAPEPGroupInfoHistory other = obj as SAPEPGroupInfoHistory;
            if (obj != null)
            {
                return this.GroupId.Equals(other.GroupId);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.GroupId.GetHashCode() ^ this.GroupName.GetHashCode();
        }
    }
}
