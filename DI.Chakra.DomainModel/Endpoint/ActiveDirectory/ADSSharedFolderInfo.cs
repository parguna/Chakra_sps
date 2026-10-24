using DI.Chakra.Logger;
using DI.Chakra.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ADSSharedFolderInfo : DomainModelBase<ADSSharedFolderInfo, EntityModel.ADSSharedFolderInfo>
    {
        
        public long EndPointSystemId { get; set; }

        public string cn { get; set; }

        public string description { get; set; }

        public string distinguishedName { get; set; }

        public string instanceType { get; set; }

        public DateTime? whenCreated { get; set; }

        public DateTime? whenChanged { get; set; }

        public Int64 uSNCreated { get; set; }

        public Int64 uSNChanged { get; set; }

        public string name { get; set; }

        public string objectGUID { get; set; }

        public string uNCName { get; set; }

        public string managedBy { get; set; }

        public string objectCategory { get; set; }

        public string nTSecurityDescriptor { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public int? Version { get; set; }


        public static void AddADSSharedFolderInfoBulk(IList<ADSSharedFolderInfo> list)
        {
            ADSUserInfo.DeleteAll<ADSSharedFolderInfo>(list.First().EndPointSystemId);
            SQLBulkUploader<ADSSharedFolderInfo> sql = null;
            string tableName = "DI_ADS_SharedFolderInfo";
            sql = new SQLBulkUploader<ADSSharedFolderInfo>(tableName);
            sql.Flush(list);
            sql.Close();
            DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Added {0} AD SharedFolder objects", list.Count.ToString()));
        }
    }
}
