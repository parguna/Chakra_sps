using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI;
using System.Net;

namespace DI.Chakra.Web
{
    public abstract class WebApiClientBase
    {
        public static TReturnType GetByUrl<TReturnType>(string baseUrl, string actionRoot, string actionMethod, Dictionary<string, string> parameterList, object complexObject = null)
        {
            string routeUrl = string.Empty;

            if (parameterList != null && parameterList.Count >= 1)
                routeUrl = String.Format("{0}/{1}?{2}", actionRoot, actionMethod, GetQueryString(parameterList));
            else
                routeUrl = String.Format("{0}/{1}", actionRoot, actionMethod);

            return InvokeApi<TReturnType>(baseUrl, routeUrl, complexObject);
        }
        public static TReturnType InvokeApi<TReturnType>(string baseUrl, string routeUrl, object complexObject, bool hasExtendedTime)
        {
            try
            {
                using (var client = GetHttpClient(baseUrl))
                {
                    HttpResponseMessage response;

                    if (complexObject == null)
                        response = client.GetAsync(routeUrl).Result;
                    else
                    {
                        HttpContent content = GetJsonSerializedContent(complexObject);
                        if (hasExtendedTime)
                            client.Timeout = new TimeSpan(0, 20, 0);
                        response = client.PostAsync(routeUrl, content).Result;
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static TReturnType InvokeApi<TReturnType>(string baseUrl, string routeUrl, object complexObject)
        {
            using (var client = GetHttpClient(baseUrl))
            {
                HttpResponseMessage response;

                if (complexObject == null)
                    response = client.GetAsync(routeUrl).Result;
                else
                {
                    HttpContent content = GetJsonSerializedContent(complexObject);
                    response = client.PostAsync(routeUrl, content).Result;
                }

                ValidateResponse(response);

                if (typeof(TReturnType) == typeof(System.Object))
                    return default(TReturnType);

                string json = response.Content.ReadAsStringAsync()?.Result;

                if (json == null || json == "null")
                    return default(TReturnType);

                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new DtoJsonConverter());

                return serializer.Deserialize<TReturnType>(new JsonTextReader(new StringReader(json)));

                //if ( typeof(TReturnType) == typeof(Dto.IEndpointConfiguration)
                //    ||
                //    typeof(TReturnType) == typeof(Dto.IProvisioningForm)
                //    ||
                //    typeof(TReturnType) == typeof(Dto.IEndpointEntitlementInfo)
                //    )
                //{

                //string jsonContent = response.Content.ReadAsStringAsync().Result;

                //if (jsonContent == "null" || jsonContent.Length <= 3) return default(TReturnType);

                //JObject jsonObject = JObject.Parse(jsonContent);
                //var assemblyQualifiedTypeName = jsonObject["$type"].ToString();
                //Type type = Type.GetType(assemblyQualifiedTypeName);
                //return (TReturnType)response.Content.ReadAsAsync(type).Result;

                //}

                //return response.Content.ReadAsAsync<TReturnType>().Result;

            }
        }
        public async static Task<TReturnType> InvokeApiAsync<TReturnType>(string baseUrl, string routeUrl, object complexObject)
        {
            HttpResponseMessage response = await SendRequestAsync(baseUrl, routeUrl, complexObject);

            string json = response.Content.ReadAsStringAsync().Result;

            if (json == null || json == "null")
                return default(TReturnType);

            dynamic obj = JsonConvert.DeserializeObject(json);

            if (obj.success != "true")
                return default(TReturnType);

            string jsonString = JsonConvert.SerializeObject(obj.result);

            JsonSerializer serializer = new JsonSerializer();

            JsonTextReader read = new JsonTextReader(new StringReader(jsonString));

            return serializer.Deserialize<TReturnType>(read);
        }
        public async static Task<HttpResponseMessage> SendRequestAsync(string baseUrl, string adaptiveUri, object complexObject)
        {
            using (var httpClient = GetHttpClient(baseUrl))
            {

                HttpResponseMessage responseMessage = null;
                try
                {
                    if (complexObject == null)
                        responseMessage = await httpClient.PostAsync(adaptiveUri, null);
                    else
                    {
                        HttpContent content = GetJsonSerializedContent(complexObject);
                        responseMessage = await httpClient.PostAsync(adaptiveUri, content);
                    }

                }
                catch (Exception ex)
                {
                    if (responseMessage == null)
                    {
                        responseMessage = new HttpResponseMessage();
                    }
                    responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                    responseMessage.ReasonPhrase = string.Format("RestHttpClient.SendRequest failed: {0}", ex);
                }
                return responseMessage;
            }
        }
        public static HttpContent GetJsonSerializedContent(object data)
        {
            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            
            
            
            settings.StringEscapeHandling = StringEscapeHandling.EscapeHtml;
            string jsonString = JsonConvert.SerializeObject(data, settings);

            return new StringContent(jsonString, Encoding.UTF8, "application/json");

            //return new ObjectContent(data.GetType(), data, formatter);
        }

        public static TReturnType GetObjectFromHttpResponse<TReturnType>(HttpResponseMessage response)
        {
            if (typeof(TReturnType) == typeof(System.Object))
                return default(TReturnType);

            string json = response.Content.ReadAsStringAsync().Result;
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new DtoJsonConverter());

            return serializer.Deserialize<TReturnType>(new JsonTextReader(new StringReader(json)));
        }

        public static HttpClient GetHttpClient(string baseUrl)
        {
            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Comment this out
            //var cred = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "IACMAdmin", "di"));
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));
            //return httpClient;
            // till this

            if (System.Web.HttpContext.Current.Session != null)
            {
                if (System.Web.HttpContext.Current.Session["LoginUserName"] != null && System.Web.HttpContext.Current.Session["LoginPassword"] != null)
                {
                    var credentials = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", System.Web.HttpContext.Current.Session["LoginUserName"],
                        System.Web.HttpContext.Current.Session["LoginPassword"]));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
                }
            }

            return httpClient;
        }

        public static string GetQueryString(Dictionary<string, string> parameterList)
        {
            StringBuilder parameterString = new StringBuilder();

            foreach (string key in parameterList.Keys)
            {
                if (parameterString.Length > 0)
                    parameterString.Append("&");

                parameterString.AppendFormat("{0}={1}",
                        HttpContext.Current.Server.UrlEncode(key),
                        HttpContext.Current.Server.UrlEncode(parameterList[key]));

            }

            return parameterString.ToString();
        }      

        public static void ValidateResponse(HttpResponseMessage response)
        {
            try
            {
                if (response.IsSuccessStatusCode)
                    return;

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    HttpContext.Current.Session.Clear();
                    HttpContext.Current.Session.Abandon();
                    HttpContext.Current.Response.Redirect("~/Login.aspx");
                }
                response.EnsureSuccessStatusCode();

            }
            catch (Exception ex)
            {
                long loggedInUserId = new SessionStatusImpl().User?.Id ?? 0;
                DILog.UILogger.WriteLog(loggedInUserId, LogLevelType.Exception, "WebApi", ex);
                if (response.StatusCode.ToString() == "422")
                    ShowExceptionAlert(response.ReasonPhrase);
                else
                    throw;
            }
        }

        public static void ShowExceptionAlert(string exceptionMessage)
        {
            string encodedMessage = HttpUtility.JavaScriptStringEncode(exceptionMessage)
                .Replace("<", "\\u003C")
                .Replace(">", "\\u003E");
            exceptionMessage = "alert('" + encodedMessage + "');";
            if (HttpContext.Current.CurrentHandler is Page)
            {
                Page currentPage = (Page)HttpContext.Current.CurrentHandler;
                if (ScriptManager.GetCurrent(currentPage) != null)
                {
                    ScriptManager.RegisterStartupScript(currentPage, typeof(Page), "Message", exceptionMessage, true);
                    //currentPage.AutoPostBackControl.Dispose();
                    currentPage.AutoPostBackControl?.Dispose();
                }
                else
                {
                    currentPage.ClientScript.RegisterStartupScript(typeof(Page), "Message", exceptionMessage, true);
                }
            }
        }
    }
}