
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System; 

namespace DI.Chakra.Workflow.Processor
{
    public class UpdateEscalationFlag
    {
        public void Execute(Task task)
        {
            RequestType requestType = RequestType.None;
            try
            {
                
                requestType = (RequestType)task.RequestTypeId;

                task.IsEscalated = true;

                task.Save(MasterConfigHelper.ServiceUser);
            }
            catch (Exception ex)
            {
                string message = "Error while executing the UpdateEscalationFlag activity.";
                if (requestType == RequestType.Attestation)
                {
                    DILog.AttestationLogger.WriteLog(null,LogLevelType.Exception, message, ex);
                }
                else if (requestType == RequestType.Provisioning)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null,  LogLevelType.Exception, message, ex);
                }
                else
                {
                    DILog.WorkflowLogger.WriteLog(LogLevelType.Exception, message, ex);
                }
            }
        }
    }
}
