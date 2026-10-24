using DI.Chakra.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPGroupHistoryInfo : DomainModelBase<SAPGroupHistoryInfo, EntityModel.SAPGroupHistoryInfo>
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

        public void AddSAPGroupInfoHistoryBulk(IList<SAPGroupHistoryInfo> groups)
        {
            SQLBulkUploader<SAPGroupHistoryInfo> sqlGroupHistory = null;
            try
            {
                string tableName = "DI_SAP_GroupHistoryInfo";
                sqlGroupHistory = new SQLBulkUploader<SAPGroupHistoryInfo>(tableName);
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
