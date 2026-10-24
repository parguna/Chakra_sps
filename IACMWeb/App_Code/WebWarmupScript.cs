using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Web
{
    /// <summary>
    /// Summary description for PreWarmUpScript
    /// </summary>
    public class WebWarmUpScript : System.Web.Hosting.IProcessHostPreloadClient
    {

        public void Preload(string[] parameters)
        {
            try
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, "Web Warm Up Script started...");

                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("endpointSystemId", 1.ToString());
                bool isAuthenticated = DomainServiceClient.GetByAction<bool>
                    ("EndPointService", DomainServiceAction.EndPointService.IsEndpointConnected, param);

                string validationMessage = string.Empty;
                bool isValid = false;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("name", "IACMAdmin");
                var user =  DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.GetByUserName, parameter);

                var password = RSAHelper.DecryptData(user.Password);

                LoginHelper loginHelper = new LoginHelper(user.UserName, password);
                isValid = loginHelper.IsValid(false, out validationMessage);

                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, "Web Warm Up Script ended...");
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null,  LogLevelType.Info, "Error while warming up Web.", ex);
            }
        }
    }
}