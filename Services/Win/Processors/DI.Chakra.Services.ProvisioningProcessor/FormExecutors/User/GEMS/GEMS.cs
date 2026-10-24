using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common;

namespace DI.Chakra.Services.ProvisioningProcessor.FormExecutors.User
{
    public class GEMS : GEMSBase
    {
        public GEMS(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger idmsLogger) : base(endpoint, currentRequest,idmsLogger)
        {
            Endpoint = endpoint;
            CurrentRequest = currentRequest;
        }

        public override bool Execute(ProvisioningParamCollection fields, DomainModel.User user)
        {
            UserName = user.UserName;
            bool result = base.Execute(fields, user);
            return result;
        }

        public override bool OffBoard(DomainModel.User user, ProvisioningParamCollection fields)
        {
            var gmUsr = GEMSUserInfo.GetPrimaryByDIUserId(user.Id, Endpoint.Id);

            bool isRemovedSuccessfully = false;

            if (gmUsr != null)
            {
                TakeOUSnapshot(user.Id, gmUsr, EmployeeEvents.Resignation, (EndpointType)Endpoint.EndpointSystemTypeId.Value, EntitlementType.Role);
                isRemovedSuccessfully = DeleteGEMSUserAccount(gmUsr, LockScenario.OffBoard);
                if (!isRemovedSuccessfully)
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format(ProvisioningConstants.MSG_ProvisioningGEMSUserDeletionFailed, gmUsr.USER_NAME, gmUsr.EndPointSystemId), null);
                }
            }

            return isRemovedSuccessfully;
        }

    }
}
