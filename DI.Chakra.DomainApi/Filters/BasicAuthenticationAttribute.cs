
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Filters;

namespace DI.Chakra.DomainApi
{
    public class BasicAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }

        public async System.Threading.Tasks.Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = context.Request;
            AuthenticationHeaderValue authorization = request.Headers.Authorization;

            if (authorization == null)
            {
                // No authentication was attempted (for this authentication method).
                // Do not set either Principal (which would indicate success) or ErrorResult (indicating an error).
                return;
            }

            if (authorization.Scheme != "Basic")
            {
                // No authentication was attempted (for this authentication method).
                // Do not set either Principal (which would indicate success) or ErrorResult (indicating an error).
                return;
            }

            if (String.IsNullOrEmpty(authorization.Parameter))
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                //context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
                return;
            }

            Tuple<string, string> userNameAndPasword = ExtractUserNameAndPassword(authorization.Parameter);

            if (userNameAndPasword == null)
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                //context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
                return;
            }

            string userName = userNameAndPasword.Item1;
            string password = userNameAndPasword.Item2;

            IPrincipal principal = await AuthenticateAsync(userName, password, cancellationToken);

            if (principal == null)
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                //context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);
            }
            else
            {
                // Authentication was attempted and succeeded. Set Principal to the authenticated user.
                context.Principal = principal;
            }
        }

        private async System.Threading.Tasks.Task<IPrincipal> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken)
        {

            //Suresh - below code duplicates LoginHelper in Web project .. try to put in some common place
            #region

            AuthenticationMode authenticationMode = (AuthenticationMode)Enum.Parse(typeof(AuthenticationMode), MasterConfigHelper.AuthenticationMode);
            var user = DomainModel.User.GetByUserName(userName);
            if (user != null &&  string.Compare(userName, Constants.IACMADMIN, true) == 0 && user.AuthenticationMode != "AD")
            {
                if (DomainModel.User.Authenticate(userName, password) == false)
                    return null;
            }
            else if (authenticationMode == AuthenticationMode.IACM)
            {
                if (DomainModel.User.Authenticate(userName, password) == false)
                    return null;
            }
            else if (authenticationMode == AuthenticationMode.AD)
            {
                if (AdAuthendication(userName, password,MasterConfigHelper.AuthenticationDomain) == false)
                    return null;
            }
            else if (authenticationMode == AuthenticationMode.MixedMode)
            {
                if (string.Compare(DomainModel.User.GetByUserName(userName).AuthenticationMode, "AD", true) == 0)
                {
                    if (AdAuthendication(userName, password, MasterConfigHelper.AuthenticationDomain) == false)
                        return null;
                }
                else
                {
                    if (DomainModel.User.Authenticate(userName, password) == false)
                        return null;
                }
            }
            #endregion

            return new ClaimsPrincipal(new GenericIdentity(userName));
        }

        private bool AdAuthendication(string userName, string password,long domainId)
        {

            return true;

            /** TODO: To be fixed for HTTPS authentication **/
            /**
            string baseUrl = AppSettingsHelper.DomainServiceBaseUrl;

            //TODO - Move DomainServiceClient in Web project to seperate Library and replace DomainApiClient for below code.
            string routeUrl = "ActiveDirectoryService/AuthenticateADUser?userName=" + System.Uri.EscapeDataString(userName) + "&password=" + System.Uri.EscapeDataString(password)+ "&domainId="+ domainId;

            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = httpClient.GetAsync(routeUrl).Result;

            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<LoginResult>().Result.Success;

            **/
        }

        private static Tuple<string, string> ExtractUserNameAndPassword(string authorizationParameter)
        {
            byte[] credentialBytes;

            try
            {
                credentialBytes = Convert.FromBase64String(authorizationParameter);
            }
            catch (FormatException)
            {
                return null;
            }

            // The currently approved HTTP 1.1 specification says characters here are ISO-8859-1.
            // However, the current draft updated specification for HTTP 1.1 indicates this encoding is infrequently
            // used in practice and defines behavior only for ASCII.
            Encoding encoding = Encoding.ASCII;
            // Make a writable copy of the encoding to enable setting a decoder fallback.
            encoding = (Encoding)encoding.Clone();
            // Fail on invalid bytes rather than silently replacing and continuing.
            encoding.DecoderFallback = DecoderFallback.ExceptionFallback;
            string decodedCredentials;

            try
            {
                decodedCredentials = encoding.GetString(credentialBytes);
            }
            catch (DecoderFallbackException)
            {
                return null;
            }

            if (String.IsNullOrEmpty(decodedCredentials))
            {
                return null;
            }

            int colonIndex = decodedCredentials.IndexOf(':');

            if (colonIndex == -1)
            {
                return null;
            }

            string userName = decodedCredentials.Substring(0, colonIndex);
            string password = decodedCredentials.Substring(colonIndex + 1);
            return new Tuple<string, string>(userName, password);
        }

        public System.Threading.Tasks.Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return System.Threading.Tasks.Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            //string parameter;

            //if (String.IsNullOrEmpty(Realm))
            //{
            //    parameter = null;
            //}
            //else
            //{
            //    // A correct implementation should verify that Realm does not contain a quote character unless properly
            //    // escaped (precededed by a backslash that is not itself escaped).
            //    parameter = "realm=\"" + Realm + "\"";
            //}

            //context.ChallengeWith("Basic", parameter);
        }

        public bool AllowMultiple
        {
            get { return false; }
        }

    }
}