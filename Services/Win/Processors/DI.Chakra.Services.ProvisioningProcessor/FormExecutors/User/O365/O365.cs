
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User
{
    public class O365 : O365Base
    {
        public O365(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest, idmsLogger)
        {
            Endpoint = endpoint;
            CurrentRequest = currentRequest;
        }

        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            bool result = base.Execute(fields, user);
            return result;
        }
        public override bool AssignO365License(DomainModel.User user, ProvisioningParamCollection fields)
        {
            bool result = base.AssignO365License(user, fields);
            return result;
        }
        public override bool UpdateO365MailBoxSettings(DomainModel.User user, ProvisioningParamCollection fields)
        {
            bool result = base.UpdateO365MailBoxSettings(user, fields);
            return result;
        }

        public override bool UndoEvent(DomainModel.User user, ProvisioningParamCollection fields)
        {
            bool isUpdated = false;
            try
            {
                if (this.CurrentRequest.EventUndoRequest.EmployeeEventType == EmployeeEvents.Resignation)
                {
                    isUpdated = base.EnableUser(user, null, fields);
                }
                return isUpdated;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
            }
            return isUpdated;
        }
    }
}
