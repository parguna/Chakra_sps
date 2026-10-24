using DI.Chakra.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPHANAGroupHistoryInfo : DomainModelBase<SAPHANAGroupHistoryInfo, EntityModel.SAPHANAGroupHistoryInfo>
    {
        public Nullable<int> HistoryId { get; set; }

        

        public Nullable<int> EndPointSystemId { get; set; }

        public string GROUP_CODE { get; set; }

        public string GROUP_DESC { get; set; }

        public Nullable<int> TargetSysId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public void AddSAPGroupInfoHistoryBulk(IList<SAPHANAGroupHistoryInfo> groups)
        {
            SQLBulkUploader<SAPHANAGroupHistoryInfo> sqlGroupHistory = null;
            try
            {
                string tableName = "DI_SAPHANA_GroupHistoryInfo";
                sqlGroupHistory = new SQLBulkUploader<SAPHANAGroupHistoryInfo>(tableName);
                sqlGroupHistory.Flush(groups);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlGroupHistory.Close();
            }
        }
    }
}
