using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DI.Chakra.Web
{
    public class DomainServiceAction
    {

        public static class ActiveDirectoryService
        {
            public const string RootName = "ActiveDirectoryService";
            public const string AuthenticateADUser = "AuthenticateADUser";
            public const string UnlockAccount = "UnlockAccount";

            //public const string DownloadSpecificUserFromHR = "DownloadSpecificUserFromHR";
        }

        public static class EndPointService
        {
            public const string RootName = "EndPointService";
            public const string ResetPassword = "ResetPassword";
            public const string ChangePassword = "ChangePassword";
            public const string IsEndpointConnected = "IsEndpointConnected";
        }

        public static class AppWinService
        {
            public const string RootName = "AppWinService";
            public const string StartStopWinService = "StartStopWinService";
            public const string GetServicesStatus = "GetServicesStatus";
        }

        //public static class InhouseService
        //{
        //    public const string RootName = "InhouseService";
        //    public const string CreateUserPostAction = "CreateUserPostAction";
        //    public const string AddEntitlement = "AddEntitlement";
        //    public const string RemoveEntitlement = "RemoveEntitlement";
        //    public const string DisableUser = "DisableUser";
        //}
    }
}