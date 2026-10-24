using DI.Chakra.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using DI.Chakra.Dto;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DI.Chakra.Web
{
    public class DomainApiClient<TDto> : WebApiClientBase
    { 
        public static List<TDto> GetAll()
        {
            string routeUrl = typeof(TDto).Name;

            using (var client = GetHttpClient(AppSettingsHelper.DomainApiBaseUrl))
            {
                HttpResponseMessage response = client.GetAsync(routeUrl).Result;

                ValidateResponse(response);

                return GetObjectFromHttpResponse<IEnumerable<TDto>>(response).ToList();
            }
        }

        public static PagedResults<TDto> GetAllPaging(int pageSize, int pageNumber)
        {
            string routeUrl = String.Format("{0}/{1}/{2}", typeof(TDto).Name, pageSize, pageNumber);

            using (var client = GetHttpClient(AppSettingsHelper.DomainApiBaseUrl))
            {
                HttpResponseMessage response = client.GetAsync(routeUrl).Result;

                ValidateResponse(response);

                return GetObjectFromHttpResponse<PagedResults<TDto>>(response);
            }
        }

        public static List<TDto> GetAll(bool eagerLoadAllProperties, int pageSize, int pageNumber)
        {
            string routeUrl = String.Format("{0}/{1}/{2}/{3}", typeof(TDto).Name, eagerLoadAllProperties, pageSize, pageNumber);

            using (var client = GetHttpClient(AppSettingsHelper.DomainApiBaseUrl))
            {
                HttpResponseMessage response = client.GetAsync(routeUrl).Result;

                ValidateResponse(response);

                return GetObjectFromHttpResponse<IEnumerable<TDto>>(response).ToList();
            }

        }

        public static PagedResults<TDto> GetAllPaging(bool eagerLoadAllProperties, int pageSize, int pageNumber, int args)
        {
            string routeUrl = String.Format("{0}/{1}/{2}/{3}/{4}", typeof(TDto).Name, eagerLoadAllProperties, pageSize, pageNumber, args);

            using (var client = GetHttpClient(AppSettingsHelper.DomainApiBaseUrl))
            {
                HttpResponseMessage response = client.GetAsync(routeUrl).Result;

                ValidateResponse(response);

                return GetObjectFromHttpResponse<PagedResults<TDto>>(response);
            }
        }

        internal static object GetByAction<T>(object getStandingOrders, Dictionary<string, string> parameter)
        {
            throw new NotImplementedException();
        }

        public static List<TDto> Get(Expression<Func<TDto, bool>> filter, bool eagerLoadAllProperties, int pageSize, int pageNumber)
        {
            string routeUrl = String.Format("{0}/{1}/{2}/{3}/{4}", typeof(TDto).Name, filter, eagerLoadAllProperties, pageSize, pageNumber);

            using (var client = GetHttpClient(AppSettingsHelper.DomainApiBaseUrl))
            {
                HttpResponseMessage response = client.GetAsync(routeUrl).Result;

                ValidateResponse(response);

                return GetObjectFromHttpResponse<IEnumerable<TDto>>(response).ToList();
            }
        }
        public static TDto GetById(long id)
        {
            string routeUrl = String.Format("{0}/{1}", typeof(TDto).Name, id);

            using (var client = GetHttpClient(AppSettingsHelper.DomainApiBaseUrl))
            {
                HttpResponseMessage response = client.GetAsync(routeUrl).Result;

                ValidateResponse(response);

                return GetObjectFromHttpResponse<TDto>(response);
            }
        }

        public static TDto GetById(long id, bool eagerload)
        {
            string routeUrl = String.Format("{0}/{1}/{2}", typeof(TDto).Name, id, eagerload);

            using (var client = GetHttpClient(AppSettingsHelper.DomainApiBaseUrl))
            {
                HttpResponseMessage response = client.GetAsync(routeUrl).Result;

                ValidateResponse(response);

                return GetObjectFromHttpResponse<TDto>(response);
            }
        }

        public static void Save(TDto dto)
        {
            string routeUrl = typeof(TDto).Name;

            using (var client = GetHttpClient(AppSettingsHelper.DomainApiBaseUrl))
            {
                HttpContent content = GetJsonSerializedContent(dto);
                HttpResponseMessage response = client.PostAsync(routeUrl, content).Result;

                ValidateResponse(response);
            }
        }

        public static long SaveAndReturnId(TDto dto)
        {
            long tempId = 0;
            string routeUrl = typeof(TDto).Name;

            using (var client = GetHttpClient(AppSettingsHelper.DomainApiBaseUrl))
            {
                HttpContent content = GetJsonSerializedContent(dto);
                HttpResponseMessage response = client.PostAsync(routeUrl, content).Result;

                ValidateResponse(response);

                if(response != null && response.Content != null)
                {
                    var obj = response.Content.ReadAsStringAsync();
                    if (obj != null)
                        tempId = Convert.ToInt64(obj.Result);
                }
            }
            return tempId;
        }

        public static void SaveList(List<TDto> dtoList)
        {
            string routeUrl = typeof(TDto).Name + "/SaveList";

            using (var client = GetHttpClient(AppSettingsHelper.DomainApiBaseUrl))
            {
                HttpContent content = GetJsonSerializedContent(dtoList);
                HttpResponseMessage response = client.PostAsync(routeUrl, content).Result;

                ValidateResponse(response);
            }
        }

        public static void DeleteById(long id)
        {
            string routeUrl = String.Format("{0}/{1}", typeof(TDto).Name, id);

            using (var client = GetHttpClient(AppSettingsHelper.DomainApiBaseUrl))
            {
                HttpResponseMessage response = client.DeleteAsync(routeUrl).Result;

                ValidateResponse(response);
            }
        }

        public static TReturnType GetByAction<TReturnType>(string actionMethod, Dictionary<string, string> parameterList)
        {
            string routeUrl = string.Empty;

            if (parameterList != null && parameterList.Count >= 1)
                routeUrl = String.Format("{0}/{1}?{2}", typeof(TDto).Name, actionMethod, GetQueryString(parameterList));
            else
                routeUrl = String.Format("{0}/{1}", typeof(TDto).Name, actionMethod);

            return InvokeApi<TReturnType>(AppSettingsHelper.DomainApiBaseUrl, routeUrl, null);
        }

        public static TReturnType GetByAction<TReturnType>(string actionMethod, Dictionary<string, string> parameterList, object complexObject)
        {
            string routeUrl = string.Empty;

            if (parameterList != null && parameterList.Count >= 1)
                routeUrl = String.Format("{0}/{1}?{2}", typeof(TDto).Name, actionMethod, GetQueryString(parameterList));
            else
                routeUrl = String.Format("{0}/{1}", typeof(TDto).Name, actionMethod);

            return InvokeApi<TReturnType>(AppSettingsHelper.DomainApiBaseUrl, routeUrl, complexObject);
        }

        public static TReturnType GetByAction<TReturnType>(string actionRoot, string actionMethod, Dictionary<string, string> parameterList, object complexObject = null)
        {
            return WebApiClientBase.GetByUrl<TReturnType>(AppSettingsHelper.DomainApiBaseUrl, actionRoot, actionMethod, parameterList, complexObject);
        }

        public async static Task<TReturnType> GetByActionAsync<TReturnType>(string actionMethod, Dictionary<string, string> parameterList, object complexObject)
        {
            string routeUrl = string.Empty;

            if (parameterList != null && parameterList.Count >= 1)
                routeUrl = String.Format("{0}/{1}?{2}", typeof(TDto).Name, actionMethod, GetQueryString(parameterList));
            else
                routeUrl = String.Format("{0}/{1}", typeof(TDto).Name, actionMethod);

            return await InvokeApiAsync<TReturnType>(AppSettingsHelper.DomainApiBaseUrl, routeUrl, complexObject);
        }
        public async static Task<TReturnType> GetByActionAsync<TReturnType>(string controller, string actionMethod, Dictionary<string, string> parameterList, object complexObject)
        {
            string routeUrl = string.Empty;

            if (parameterList != null && parameterList.Count >= 1)
                routeUrl = String.Format("{0}/{1}?{2}", controller, actionMethod, GetQueryString(parameterList));
            else
                routeUrl = String.Format("{0}/{1}", controller, actionMethod);

            return await InvokeApiAsync<TReturnType>(AppSettingsHelper.DomainApiBaseUrl, routeUrl, complexObject);
        }
        public async static Task<TReturnType> GetByActionAsync<TReturnType>(string actionMethod, Dictionary<string, string> parameterList)
        {
            string routeUrl = string.Empty;

            if (parameterList != null && parameterList.Count >= 1)
                routeUrl = String.Format("{0}/{1}?{2}", typeof(TDto).Name, actionMethod, GetQueryString(parameterList));
            else
                routeUrl = String.Format("{0}/{1}", typeof(TDto).Name, actionMethod);

            return await InvokeApiAsync<TReturnType>(AppSettingsHelper.DomainApiBaseUrl, routeUrl, null);
        }

        public static string GetUrl(string actionMethod, Dictionary<string, string> parameterList)
        {
            string routeUrl = string.Empty;

            if (parameterList != null && parameterList.Count >= 1)
                routeUrl = String.Format("{0}/{1}?{2}", typeof(TDto).Name, actionMethod, GetQueryString(parameterList));
            else
                routeUrl = String.Format("{0}/{1}", typeof(TDto).Name, actionMethod);


            return routeUrl;

        }
        public static TReturnType Execute<TReturnType>(string url, object complexObject = null)
        {
            return InvokeApi<TReturnType>(AppSettingsHelper.DomainApiBaseUrl, url, complexObject);

        }
    }
}