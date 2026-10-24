using DI.Chakra.DBConfigHelper;
using DI.Chakra.Facade;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Services.HouseKeepingProcessor
{
    public class PurgeAgent
    {
        public void StartProcess()
        {
            DILog.HouseKeepingLogger.WriteLog(LogLevelType.Info, "Housekeeping Engine Started");
            int HouseKeeping_NumberOfDaysAttestation = MasterConfigHelper.HouseKeepingNumberOfDaysAttestation;
            int HouseKeeping_NumberOfDaysTasks = MasterConfigHelper.HouseKeepingNumberOfDaysTasks;
            int HouseKeeping_NumberOfDaysLog = MasterConfigHelper.HouseKeepingNumberOfDaysLog;
            HouseKeepingFacade housekeepingFacade = new HouseKeepingFacade();

            if (HouseKeeping_NumberOfDaysAttestation >= 30)
            {
                int TotalAttestationDataPurged = housekeepingFacade.PurgeAllAttestationData(HouseKeeping_NumberOfDaysAttestation);
                DILog.HouseKeepingLogger.WriteLog(LogLevelType.Info, string.Format("Housekeeping Engine Purged {0} Items From Attestation Data", TotalAttestationDataPurged));
            }

            if (HouseKeeping_NumberOfDaysTasks >= 30)
            {
                int TotalTasksDataPurged = housekeepingFacade.PurgeAllTasksData(HouseKeeping_NumberOfDaysTasks);
                DILog.HouseKeepingLogger.WriteLog(LogLevelType.Info, string.Format("Housekeeping Engine Purged {0} Items From Tasks Data", TotalTasksDataPurged));
            }

            if (HouseKeeping_NumberOfDaysLog >= 30)
            {
                int TotalLogsDataPurged = housekeepingFacade.PurgeAllLogsData(HouseKeeping_NumberOfDaysLog);
            }
        }

    }
}
