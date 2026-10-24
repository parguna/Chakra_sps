using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.ServiceModel;
using System.ServiceProcess;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Services
{
    [ServiceContract]
    public interface IPasswordResetService
    {
        //[OperationContract]
        //LoginResult AuthenticateADUser(string userName, string password);

        //[OperationContract]
        //bool IsActiveAccount(string userName);

        //[OperationContract]
        //bool ResetPassword(string userName, string password, long userId, long endpointSystemId);

        //[OperationContract]
        //bool ChangePassword(string userName, string password, string oldPassword, long userId, long endpointSystemId);

        //[OperationContract]
        //bool UnlockAccount(string userName);

        //[OperationContract]
        //bool IsEndpointConnected(long endpointSystemId);

        [OperationContract]
        void CreateUserPostAction(InhouseUserInfo userInfo);

        [OperationContract]
        void AddEntitlement(InhouseUserInfo userInfo, EntitlementType entitlementType);

        [OperationContract]
        void RemoveEntitlement(InhouseUserInfo userInfo, EntitlementType entitlementType);

        [OperationContract]
        void DisableUser(InhouseUserInfo userInfo);

        [OperationContract]
        int DownloadAllUsersFromHR(DateTime scanDate);

        [OperationContract]
        void DownloadSpecificUserFromHR(string pernr);

        [OperationContract]
        void StartStopWinService(string toStartStop, string serviceName, int timeoutMilliseconds);

        [OperationContract]
        ServiceControllerStatus GetServicesStatus(string servicename);
    }
}
