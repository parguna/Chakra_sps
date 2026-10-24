using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DBConfigHelper;
using System.Net.Http;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System.Net;
using System.IO;

using System.Reflection;


namespace DI.Chakra.Services.AdHocJobProcessor
{
    public class AdHocJobService
    {
        private const string KEY_SERVICENAME = @"System";

        private void ChangeAdHocJobStatus(Process process, SchedulerStatus status)
        {
            if (status == SchedulerStatus.InProgress)
            {
                process.ProcessStartAt = DateTime.Now;
                process.ProcessEndAt = null;
                process.Status = "InProgress";

            }
            else if (status == SchedulerStatus.Failed)
            {
                process.ProcessEndAt = DateTime.Now;
                process.Status = "Failed";
                //Need  to write logic to send email for failed Job
            }
            else if (status == SchedulerStatus.Pending)
            {
                process.LastModifiedBy = KEY_SERVICENAME;
                process.LastModifiedDate = DateTime.Now;
                process.Status = "Pending";

            }
            else
            {
                process.ProcessEndAt = DateTime.Now;
                process.LastRunOn = DateTime.Now;
                process.Status = "Completed";
            }
            process.Save(MasterConfigHelper.ServiceUser);

        }
        public void Start(Process process)
        {
            AdhocJobProcess adhocJobProcess = null;
            string responseString = string.Empty;
            string result = string.Empty;
            try
            {
                adhocJobProcess = AdhocJobProcess.GetActiveJobById(process.ProcessTypeKeyId,true);
                if (adhocJobProcess == null)
                {
                    DILog.ReportingLogger.WriteLog((int)process.Id, LogLevelType.Error, string.Format("AdHocJob process not found for ProcessId - {0}", Convert.ToString(process.ProcessTypeKeyId)));
                    throw new EntityNotFoundException(string.Format("AdHocJob process not found for ProcessId - {0}", Convert.ToString(process.ProcessTypeKeyId)));
                }

                var JobName = adhocJobProcess.JobName;

                ConfigurationMasterIM AdhocJobAssemblyPath = ConfigurationMasterIM.GetByConfigKey("AdhocJobAssemblyPath");
                if (AdhocJobAssemblyPath != null)
                {
                    Assembly asm = Assembly.LoadFile(AdhocJobAssemblyPath.ConfigValue);
                    string fullyQualifiedTypeName = "DI.Chakra.AdhocJobs.Jobs." + JobName;
                    // Get the type contained in the name string
                    Type type = asm.GetTypes().Where(m => m.FullName == fullyQualifiedTypeName).FirstOrDefault();

                    // create an instance of that type
                    object instance = asm.CreateInstance(type.FullName);
                    if (instance != null)
                    {
                        MethodInfo PreConditionResultSetMethodInfo = type.GetMethod("BuildPreConditionResultSet");
                        PreConditionResultSetMethodInfo.Invoke(instance, null);
                        MethodInfo ExecuteMethodInfo = type.GetMethod("Execute");
                        ExecuteMethodInfo.Invoke(instance, null);
                    }

                    ChangeAdHocJobStatus(process, SchedulerStatus.Completed);
                    ProcessHistory history = GetHistory(process.Id, "Success", "");
                    history.Save(MasterConfigHelper.ServiceUser);
                    DILog.DiscoveryLogger.WriteLog((int)adhocJobProcess.Id, LogLevelType.Info, string.Format("Completed ProcessId:{0}", adhocJobProcess.Id.ToString()));
                }
            }
            catch (Exception ex)
            {
                process.ErrorMessage = ex.Message;
                ChangeAdHocJobStatus(process, SchedulerStatus.Failed);
                // I need to write a code for saving error message and status in to ProcessHistroy table.
                DILog.DiscoveryLogger.WriteLog((int)adhocJobProcess.Id, LogLevelType.Error, "ErrorTraced", ex);
            }

        }


        private ProcessHistory GetHistory(long processId, string status, string errMessage)
        {
            ProcessHistory history = new ProcessHistory();
            history.ProcessType = ProcessType.Reporting;
            history.ProcessId = processId;
            history.Status = status;
            history.ErrorMessage = errMessage;
            return history;
        }
    }
}
