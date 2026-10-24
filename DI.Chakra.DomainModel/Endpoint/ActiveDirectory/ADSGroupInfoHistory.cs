using DI.Chakra.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ADSGroupInfoHistory : DomainModelBase<ADSGroupInfoHistory, EntityModel.ADSGroupInfoHistory>
    {
        public int GroupId { get; set; }

        public int HistoryId { get; set; }

        

        public long EndPointSystemId { get; set; }

        public string cn { get; set; }

        public string description { get; set; }

        public string member { get; set; }

        public string distinguishedName { get; set; }

        public string instanceType { get; set; }

        public DateTime? whenCreated { get; set; }

        public DateTime? whenChanged { get; set; }

        public Int64 uSNCreated { get; set; }

        public Int64 uSNChanged { get; set; }

        public string name { get; set; }

        public string objectGUID { get; set; }

        public string objectSid { get; set; }

        public string sAMAccountName { get; set; }

        public string sAMAccountType { get; set; }

        public string groupType { get; set; }

        public string groupTypeDesc { get; set; }

        public string objectCategory { get; set; }

        public string nTSecurityDescriptor { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public string RecordStatus { get; set; }

        public int? Version { get; set; }

        public static void AddADSGroupInfoHistoryBulk(IList<ADSGroupInfoHistory> groups)
        {
            SQLBulkUploader<ADSGroupInfoHistory> sql = null;
            string tableName = "DI_ADS_GroupInfoHistory";
            sql = new SQLBulkUploader<ADSGroupInfoHistory>(tableName);
            sql.Flush(groups);
            sql.Close();
            
        }
    }
}
