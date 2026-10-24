using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Services.MiscProcessor
{
    public class PurgeAgent
    {
        private static readonly string[] AllowedTables = { "DI_LOG_SERVICELOG", "DI_LOG_UI", "DI_LOG_ATTESTATION", "DI_LOG_DISCOVERY", "DI_LOG_EMAIL", "DI_LOG_PROVISIONING", "DI_M_EMAILHISTORY", "DI_M_EMAIL"};
        public void StartProcess()
        {
            AutoMapperHelper.BootStrap();
            List<PurgeConfig> currentJobs = PurgeConfig.GetAll();
            
            
            try
            {
                foreach (PurgeConfig job in currentJobs)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(job.TableName) && job.PurgeMode == PurgeMode.Delete)
                        {
                            var temp = AllowedTables.FirstOrDefault(table => job.TableName.Trim().ToUpper() == table.Trim().ToUpper());
                            if (temp != null)
                            {
                                DILog.ProvisioningLogger?.WriteLog(LogLevelType.Info, $"The Log Table:{job?.TableName} DaysToKeep:{job.DaysToKeep}.");
                                PurgeConfig.PurgeTable(job.TableName, job.DaysToKeep);
                            }
                            else
                            {
                                DILog.ProvisioningLogger?.WriteLog(LogLevelType.Warning, $"Purge is not allowed for this table:{job?.TableName}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger?.WriteLog(LogLevelType.Exception, $"Error while deleting the Log Table:{job?.TableName} DaysToKeep:{job.DaysToKeep}.", ex);
                    }
                }
            }
            catch(Exception ex)
            {

                DILog.ProvisioningLogger?.WriteLog(LogLevelType.Exception, "Error while StartProcess.", ex);
            }
        }
    }
}
