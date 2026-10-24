using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DI.Chakra.Web
{
    public class DomainServiceClient : WebApiClientBase
    {
        public static TReturnType GetByAction<TReturnType>(string actionRoot, string actionMethod, Dictionary<string, string> parameterList, object complexObject = null)
        {
            //DILog.UILogger.WriteLog(null, LogLevelType.Info, $"Url:  {AppSettingsHelper.DomainServiceBaseUrl}/{actionRoot}/{actionMethod}");
            return WebApiClientBase.GetByUrl<TReturnType>(AppSettingsHelper.DomainServiceBaseUrl, actionRoot, actionMethod, parameterList, complexObject);
            
        }
        public async static Task<TReturnType> GetByActionAsync<TReturnType>(string method, Dictionary<string, string> parameterList)
        {
            string routeUrl = string.Empty;

            if (parameterList != null && parameterList.Count >= 1)
                routeUrl = String.Format("{0}?{1}", method, GetQueryString(parameterList));
            else
                routeUrl = String.Format("{0}", method);
            DILog.UILogger.WriteLog(null, LogLevelType.Info, "Url: " + AppSettingsHelper.DomainServiceBaseUrl+routeUrl);
            return await InvokeApiAsync<TReturnType>(AppSettingsHelper.DomainServiceBaseUrl, routeUrl, null);
        }

        public async static Task<TReturnType> GetByActionAsync<TReturnType>(string actionRoot, string method, Dictionary<string, string> parameterList)
        {
            string routeUrl = string.Empty;

            if (parameterList != null && parameterList.Count >= 1)
                routeUrl = String.Format("{0}/{1}?{2}", actionRoot, method, GetQueryString(parameterList));
            else
                routeUrl = String.Format("{0}/{1}", actionRoot, method);
            //DILog.UILogger.WriteLog(null, LogLevelType.Info, "Url: " + AppSettingsHelper.DomainServiceBaseUrl + routeUrl);
            return await InvokeApiAsync<TReturnType>(AppSettingsHelper.DomainServiceBaseUrl, routeUrl, null);
        }
        public async static Task<TReturnType> GetByActionAsync<TReturnType>(string method, Dictionary<string, string> parameterList, object complexObject)
        {
            string routeUrl = string.Empty;


            if (parameterList != null && parameterList.Count >= 1)
                routeUrl = String.Format("{0}?{1}", method, GetQueryString(parameterList));
            else
                routeUrl = String.Format("{0}", method);

            //DILog.UILogger.WriteLog(null, LogLevelType.Info, "Url: " + AppSettingsHelper.DomainServiceBaseUrl + routeUrl);
            return await InvokeApiAsync<TReturnType>(AppSettingsHelper.DomainServiceBaseUrl, routeUrl, complexObject);
        }
    }
}