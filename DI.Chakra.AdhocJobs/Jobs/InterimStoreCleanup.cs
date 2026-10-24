using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel;
using DI.Chakra.Logger;
using DI.Chakra.Connector;
using DI.Chakra.Services.ProvisioningProcessor;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.AdHocJobs;

namespace DI.Chakra.AdhocJobs.Jobs
{
   public class InterimStoreCleanup : IAdhocJob
    {
        private object JobConditionResultSet;
        public Object Result
        {
            get
            {
                return JobConditionResultSet;
            }

            set
            {
                JobConditionResultSet = value;
            }
        }

        public void BuildPreConditionResultSet()
        {

            List<InterimStore> InterimStores = new List<InterimStore>();

            try
            {

                ConfigurationMasterIM InterimStoreCleanUpConfig = ConfigurationMasterIM.GetByConfigKey("InterimStoreCleanUpDays");
                if (InterimStoreCleanUpConfig != null)
                {
                    int days = Convert.ToInt32(InterimStoreCleanUpConfig.ConfigValue);
                    InterimStores = InterimStore.GetInterimStoreDays(days);

                }

            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, "Error while executing BuildPreConditionResultSet - InterimStoreCleanUp Job PreCondition Query", ex);
            }
            
            Result = InterimStores;
        }

        public void Execute()
        {
            List<InterimStore> interimList = (List<InterimStore>)Result;
            if (interimList != null && interimList.Count > 0)
            {
                var interimAttachment = InterimStore.GetAttachmentByIds(interimList.Select(a => a.Id).ToList()).ToList();
                if (interimAttachment != null && interimAttachment.Count > 0)
                {
                    InterimStore.DeleteList(interimAttachment);
                }
                InterimStore.DeleteList(interimList);
            }
        }
     }
  }
