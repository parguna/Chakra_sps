using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.ServiceHelpers.PwdResetSvcRef;
using System;
using System.ServiceProcess;
using DI.Chakra.Infrastructure;
namespace DI.Chakra.ServiceHelpers
{
    /*
    public class PasswordResetServiceHelper
    {
        PasswordResetServiceClient client = null;

        public PasswordResetServiceHelper()
        {
            client = new PasswordResetServiceClient();
        }

        public LoginResult AuthenticateADUser(string userName, string password)
        {
            try
            {
                return client.AuthenticateADUser(userName, password);
            }
            catch
            {
                throw;
            }
        }

        public bool IsActiveAccount(string userName)
        {
            try
            {
                return client.IsActiveAccount(userName);
            }
            catch
            {
                throw;
            }
        }

        public bool UnlockAccount(string userName)
        {
            try
            {
                return client.UnlockAccount(userName);
            }
            catch
            {
                throw;
            }
        }

        public bool ResetPassword(string userName, string password, long userId, long endpointSystemId)
        {
            try
            {
                return client.ResetPassword(userName, password, userId, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public bool ChangePassword(string userName, string password, string oldPassword, long userId, long endpointSystemId)
        {
            try
            {
                return client.ChangePassword(userName, password, oldPassword, userId, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public bool IsEndpointConnected(long endpointSystemId)
        {
            try
            {
                return client.IsEndpointConnected(endpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public void CreateUserPostAction(InhouseUserInfo userInfo)
        {
            try
            {

                client.CreateUserPostAction(userInfo);
            }
            catch
            {
                throw;
            }
        }

        public void AddEntitlement(InhouseUserInfo userInfo, EntitlementType entitlementType)
        {
            try
            {

                client.AddEntitlement(userInfo, entitlementType);
            }
            catch
            {
                throw;
            }
        }

        public void RemoveEntitlement(InhouseUserInfo userInfo, EntitlementType entitlementType)
        {
            try
            {
                client.RemoveEntitlement(userInfo, entitlementType);
            }
            catch
            {
                throw;
            }
        }

        public void DisableUser(InhouseUserInfo userInfo)
        {
            try
            {
                client.DisableUser(userInfo);
            }
            catch
            {
                throw;
            }
        }

        public long DownloadAllUsersFromHR(DateTime scanDate)
        {
            try
            {
                return client.DownloadAllUsersFromHR(scanDate);
            }
            catch
            {
                throw;
            }
        }

        public void DownloadSpecificUserFromHR(string pernr)
        {
            try
            {
                client.DownloadSpecificUserFromHR(pernr);
            }
            catch
            {
                throw;
            }
        }

        public void StartStopWinService(string toStartStop, string serviceName, int timeoutMilliseconds)
        {
            try
            {
                client.StartStopWinService( toStartStop,  serviceName,  timeoutMilliseconds);
            }
            catch
            {
                throw;
            }
        }

        public ServiceControllerStatus GetServicesStatus(string servicename)
        {
            try
            {
                return client.GetServicesStatus(servicename);
            }
            catch
            {
                throw;
            }
        }
    }
    */
}
