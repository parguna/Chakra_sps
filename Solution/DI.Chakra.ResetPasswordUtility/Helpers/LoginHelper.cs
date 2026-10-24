
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;


namespace DI.Chakra.PasswordReset
{
    public class LoginHelper
    {
        private readonly string mUserName = string.Empty;
        private readonly string mPassword = string.Empty;
        public LoginHelper()
        { }
        public LoginHelper(string userName, string password)
        {
            mUserName = userName;
            mPassword = password;
        }

        public string ResetPassword(out string mMessage)
        {
            string bReturn = "";
            mMessage = string.Empty;

            try
            {

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("name", mUserName);

                Dto.User mUser = ApiCallPost<Dto.User>("User", DomainApiAction.User.ForgotUser, parameter, null);

                if (mUser == null)
                {
                    mMessage = "User Name not found";
                    return mMessage;
                }

                if (!mUser.IsActive.HasValue || mUser.IsActive.Value == false)
                {
                    mMessage = "User is not active";
                    return mMessage;
                }


                if (!string.IsNullOrEmpty(mUser.AuthenticationMode) && (mUser.AuthenticationMode.ToLower() == AuthenticationMode.AD.ToString().ToLower() || mUser.AuthenticationMode.ToLower() == AuthenticationMode.MixedMode.ToString().ToLower()))
                {
                    mMessage = string.Format("Can't reset the password for 'AD' atuhentication mode. User - '{0}' authentication mode is 'AD'.", mUser.UserName);
                    return mMessage;
                }

                mUser.Password = RSAHelper.EncryptData(mPassword);
                mUser.LastModifiedBy = "System";
                mUser.LastModifiedDate = DateTime.Now;
                var res = ApiCallPost<Dto.User>("User", "SaveUser", null, mUser);

                return "Password Reset successfully done.";
            }
            catch (Exception ex)
            {
                throw;
            }
            return bReturn;
        }

        private TReturnType ApiCallPost<TReturnType>(string actionName, string actionMethod, Dictionary<string, string> parameter, object complexObject)
        {
            string routeUrl;
            HttpResponseMessage response = null;
            string baseurl = string.Empty;

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DomainApiBaseUrl"]))
            {
                baseurl = ConfigurationManager.AppSettings["DomainApiBaseUrl"];
            }

           
            //var handler = new WebRequestHandler();
            //handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            //handler.UseProxy = false;

            //var certificate = new X509Certificate2(@"C:\Divakara\cert\dummycert.pfx");
            //handler.ClientCertificates.Add(certificate);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseurl);

                if (complexObject == null)
                {
                    var param = GetQueryString(parameter);
                    routeUrl = String.Format("{0}/{1}?{2}", actionName, actionMethod, param);

                    // Add an Accept header for JSON format.
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    response = client.GetAsync(routeUrl).Result;
                }
                else
                {
                    routeUrl = String.Format("{0}/{1}", actionName, actionMethod);

                    // Add an Accept header for JSON format.
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    HttpContent content = GetJsonSerializedContent(complexObject);
                    response = client.PostAsync(routeUrl, content).Result;
                }
            }

            ValidateResponse(response);


            if (typeof(TReturnType) == typeof(System.Object))
                return default(TReturnType);

            string json = response.Content.ReadAsStringAsync().Result;

            if (json == null || json == "null")
                return default(TReturnType);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new DtoJsonConverter());

            return serializer.Deserialize<TReturnType>(new JsonTextReader(new StringReader(json)));
        }

        public static HttpContent GetJsonSerializedContent(object data)
        {
            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            string jsonString = JsonConvert.SerializeObject(data, settings);

            return new StringContent(jsonString, Encoding.UTF8, "application/json");

            //return new ObjectContent(data.GetType(), data, formatter);
        }


        public static string GetQueryString(Dictionary<string, string> parameterList)
        {
            StringBuilder parameterString = new StringBuilder();

            foreach (string key in parameterList.Keys)
            {
                if (parameterString.Length > 0)
                    parameterString.Append("&");

                parameterString.AppendFormat("{0}={1}",
                      key,
                        parameterList[key]);

            }

            return parameterString.ToString();
        }

        public static void ValidateResponse(HttpResponseMessage response)
        {
            try
            {
                if (response.IsSuccessStatusCode)
                    return;
                response.EnsureSuccessStatusCode();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}