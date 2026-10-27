using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace DI.Chakra.DomainApi 
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            string exceptionMessage = string.Empty;
            HttpStatusCode httpStatusCode = (HttpStatusCode)422;
            var sqlException = actionExecutedContext?.Exception?.InnerException?.InnerException as SqlException; 
            if (actionExecutedContext?.Exception?.InnerException == null)
            {
                exceptionMessage = actionExecutedContext?.Exception?.Message;
            }
            else
            {
                if ((sqlException != null) && (sqlException.Number == 547))
                {
                    exceptionMessage = "Unable to proceed due to child data exists!";
                }
            }
            // ReasonPhrase must be a single line and at most 512 chars, or IIS throws while writing the response
            var reasonPhrase = (exceptionMessage ?? string.Empty).Replace("\r", " ").Replace("\n", " ");
            if (reasonPhrase.Length > 500)
                reasonPhrase = reasonPhrase.Substring(0, 500);
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("WepApi Exception"),
                StatusCode = httpStatusCode,
                ReasonPhrase = reasonPhrase
            };
            actionExecutedContext.Response = response;

        }
       
    }
}