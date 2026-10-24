using DI.Chakra.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Connector
{
    public class UserMailBoxQuotaInfoOrchastrator
    {
        public UserMailBoxQuotaInfoOrchastrator()
        {
        }
        public long EndpointSystemId { get; set; }

        public int HistoryId { get; set; }
        public ExUserMailBoxQuotaInfo UserMailBoxQuotaInfo(PSObject directoryEntry)
        {
            if (null == directoryEntry)
                return null;
            ExUserMailBoxQuotaInfo excgroupInfo = new ExUserMailBoxQuotaInfo();
            try
            {
                excgroupInfo.EndPointSystemId = EndpointSystemId;
                excgroupInfo.UserId = Convert.ToString(ExHelper.GetProperty(directoryEntry, ExProperties.sAMAccountName));
                excgroupInfo.IssueWarningQuota = Convert.ToString(ExHelper.GetProperty(directoryEntry, ExProperties.IssueWarningQuota));
                excgroupInfo.ProhibitSendQuota = Convert.ToString(ExHelper.GetProperty(directoryEntry, ExProperties.ProhibitSendQuota));
                excgroupInfo.ProhibitSendReceiveQuota = Convert.ToString(ExHelper.GetProperty(directoryEntry, ExProperties.ProhibitSendReceiveQuota));
                excgroupInfo.PrimarySmtpAddress = Convert.ToString(ExHelper.GetProperty(directoryEntry, ExProperties.PrimarySmtpAddress));
                if (Convert.ToString(ExHelper.GetProperty(directoryEntry, ExProperties.UseDatabaseQuotaDefaults)).ToLower() == "true" || Convert.ToString(ExHelper.GetProperty(directoryEntry, ExProperties.UseDatabaseQuotaDefaults)).ToLower() == "$true" || Convert.ToString(ExHelper.GetProperty(directoryEntry, ExProperties.UseDatabaseQuotaDefaults)).ToLower() == "1")
                    excgroupInfo.UseDatabaseQuotaDefaults = true;
                else
                    excgroupInfo.UseDatabaseQuotaDefaults = false;
            }
            catch (Exception ex)
            {

                //Log.Discovery.WriteMessage(LogLevel.ERROR,
                //               string.Format("Error occured in accessing the path {0}",
                //               directoryEntry.Path), ex);
                return null;
            }
            return excgroupInfo;
        }
    }
}
