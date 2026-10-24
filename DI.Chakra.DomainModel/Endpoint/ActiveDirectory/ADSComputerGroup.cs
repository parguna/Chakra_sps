using DI.Chakra.Logger;
using DI.Chakra.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ADSComputerGroup : DomainModelBase<ADSComputerGroup, EntityModel.ADSComputerGroup>
    {
        public long EndpointSystemId { get; set; }

        public string ComputerCN { get; set; }

        public string ComputerDistinguishedName { get; set; }

        public string GroupCN { get; set; }

        public string GroupDistinguishedName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public static void AddADSComputerGroupBulk(IList<ADSComputerGroup> computerGroup)
        {
            SQLBulkUploader<ADSComputerGroup> sqlUserHistory = null;
            try
            {
                ADSUserInfo.DeleteAll<ADSComputerGroup>(computerGroup.First().EndpointSystemId);
                string tableName = "DI_ADS_ComputerGroup";
                sqlUserHistory = new SQLBulkUploader<ADSComputerGroup>(tableName);
                sqlUserHistory.Flush(computerGroup);
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Added {0} AD ComputerGroup objects", computerGroup.Count.ToString()));
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlUserHistory.Close();
            }
        }
    }
}
