using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DI.Chakra.Model
{
    //[TableInfo("DI_SAPEP_GroupInfo", "SAPEPGroupInfo")]
    [Table("DI_SAPEP_GroupInfo")]
    public class SAPEPGroupInfo : IAggregateRoot, IComparer<SAPEPGroupInfo>,IXLData
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public Nullable<long> EndPointSystemId { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public string GroupUniqueName { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public int? Version { get; set; }
        public bool IsXLUploaded { get; set; }


        public virtual IList<SAPEPUserInfo> SAPEPUserInfos { get; set; }


        public SAPEPGroupInfo()
        {
            SAPEPUserInfos = new List<SAPEPUserInfo>();
        }

        public override string ToString()
        {
            return this.GroupId.ToString();
        }
        public static SAPEPGroupInfo CreateSAPEPGroupInfo()
        {
            SAPEPGroupInfo sapEPGroupInfo = new SAPEPGroupInfo();
            return sapEPGroupInfo;
        }

        public override bool Equals(object obj)
        {
            SAPEPGroupInfo other = obj as SAPEPGroupInfo;
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


        public int Compare(SAPEPGroupInfo x, SAPEPGroupInfo y)
        {
            int cmprValue = -1;
            if (x != null && y != null)
            {
               
                    cmprValue = string.Compare(y.GroupName, x.GroupName);
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
