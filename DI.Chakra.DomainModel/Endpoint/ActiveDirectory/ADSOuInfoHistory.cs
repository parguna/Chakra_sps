using DI.Chakra.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ADSOuInfoHistory : DomainModelBase<ADSOuInfoHistory, EntityModel.ADSOuInfoHistory>
    {
        public long OuId { get; set; }

        public int HistoryId { get; set; }

        

        public long EndPointSystemId { get; set; }

        public string ou { get; set; }

        public string description { get; set; }

        public string distinguishedName { get; set; }

        public string instanceType { get; set; }

        public DateTime? whenCreated { get; set; }

        public DateTime? whenChanged { get; set; }

        public Int64 uSNCreated { get; set; }

        public Int64 uSNChanged { get; set; }

        public string showInAdvancedViewOnly { get; set; }

        public string name { get; set; }

        public string objectGUID { get; set; }

        public string systemFlags { get; set; }

        public string objectCategory { get; set; }

        public string isCriticalSystemObject { get; set; }

        public string gPLink { get; set; }

        public string nTSecurityDescriptor { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public string RecordStatus { get; set; }

        public int? Version { get; set; }

        public ADSOuInfo ADSOuInfo { get; set; }


        public static void AddADSOuInfoHistoryBulk(IList<ADSOuInfoHistory> ouList)
        {
            SQLBulkUploader<ADSOuInfoHistory> sql = null;
            try
            {
                string tableName = "DI_ADS_OuInfoHistory";

                sql = new SQLBulkUploader<ADSOuInfoHistory>(tableName);

                sql.Flush(ouList);


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sql.Close();
            }
        }
    }
}
