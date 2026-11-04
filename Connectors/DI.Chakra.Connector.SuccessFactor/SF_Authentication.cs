using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DI.Chakra.Connector
{
    public enum SF_AuthenticationType
    {
        Basic,
        OAuth
    }

    public class SF_AuthenticationException : Exception
    {
        public SF_AuthenticationException(string message) : base(message) { }

        public SF_AuthenticationException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// SuccessFactors connection settings read from the endpoint configuration.
    /// OAuth is opt-in through SF_EnableOAuth; when it is not set the endpoint keeps using Basic authentication.
    /// </summary>
    public class SF_AuthenticationSettings
    {
        public SF_AuthenticationType AuthenticationType { get; set; }

        public string UserId { get; set; }

        public string CompanyId { get; set; }

        /// <summary>Decrypted password. Only populated for Basic authentication.</summary>
        public string Password { get; set; }

        public string BaseCompanyUrl { get; set; }

        public string ClientId { get; set; }

        public string CertificateThumbprint { get; set; }

        public string GrantType { get; set; }

        public string TokenUrl
        {
            get { return $"{(BaseCompanyUrl ?? string.Empty).Trim().TrimEnd('/')}/oauth/token"; }
        }

        public static SF_AuthenticationSettings FromConfiguration(Func<EndpointKey, string> config)
        {
            bool enableOAuth;
            bool.TryParse(config(EndpointKey.SF_EnableOAuth), out enableOAuth);

            var settings = new SF_AuthenticationSettings
            {
                AuthenticationType = enableOAuth ? SF_AuthenticationType.OAuth : SF_AuthenticationType.Basic,
                UserId = config(EndpointKey.UserName),
                CompanyId = config(EndpointKey.SF_Company),
            };

            if (enableOAuth)
            {
                settings.BaseCompanyUrl = config(EndpointKey.SF_BaseCompanyUrl);
                settings.ClientId = config(EndpointKey.SF_ClientId);
                settings.CertificateThumbprint = config(EndpointKey.SF_CertificateThumbprint);
                settings.GrantType = string.IsNullOrWhiteSpace(config(EndpointKey.SF_GrantType))
                    ? SF_OAuthTokenProvider.SamlBearerGrantType
                    : config(EndpointKey.SF_GrantType).Trim();
            }
            else
            {
                settings.Password = RSAHelper.DecryptData(config(EndpointKey.Password));
            }

            return settings;
        }

        /// <summary>
        /// Validates the OAuth settings. Basic authentication is not validated here so existing endpoints behave as before.
        /// The message names the missing settings only, never their values.
        /// </summary>
        public void Validate()
        {
            if (AuthenticationType != SF_AuthenticationType.OAuth)
                return;

            var missing = new List<string>();
            if (string.IsNullOrWhiteSpace(BaseCompanyUrl)) missing.Add("Base Company URL");
            if (string.IsNullOrWhiteSpace(CompanyId)) missing.Add("Company ID");
            if (string.IsNullOrWhiteSpace(UserId)) missing.Add("User ID");
            if (string.IsNullOrWhiteSpace(ClientId)) missing.Add("Client ID");
            if (string.IsNullOrWhiteSpace(CertificateThumbprint)) missing.Add("Certificate Thumbprint");

            if (missing.Count > 0)
                throw new SF_AuthenticationException($"SuccessFactors OAuth is enabled but the following settings are missing: {string.Join(", ", missing)}.");

            Uri baseUri;
            if (!Uri.TryCreate(BaseCompanyUrl.Trim(), UriKind.Absolute, out baseUri) || baseUri.Scheme != Uri.UriSchemeHttps)
                throw new SF_AuthenticationException("SuccessFactors OAuth Base Company URL must be an absolute https URL of the API server.");

            if (!string.Equals(GrantType, SF_OAuthTokenProvider.SamlBearerGrantType, StringComparison.Ordinal))
                throw new SF_AuthenticationException($"SuccessFactors OAuth Grant Type is not supported. Only '{SF_OAuthTokenProvider.SamlBearerGrantType}' is supported.");
        }
    }

    /// <summary>
    /// Produces the Authorization header for SuccessFactors requests (OData and upsert).
    /// </summary>
    public class SF_Authenticator
    {
        readonly SF_AuthenticationSettings _settings;
        readonly SF_OAuthTokenProvider _tokenProvider;
        readonly string _basicHeader;
        readonly Action<string> _logWarning;

        public SF_Authenticator(SF_AuthenticationSettings settings, SF_OAuthTokenProvider tokenProvider = null, Action<string> logWarning = null)
        {
            _settings = settings;
            _settings.Validate();
            _logWarning = logWarning ?? (_ => { });

            if (_settings.AuthenticationType == SF_AuthenticationType.OAuth)
                _tokenProvider = tokenProvider ?? new SF_OAuthTokenProvider(settings);
            else
                _basicHeader = $"Basic {GetBaseCredential(settings)}";
        }

        public SF_AuthenticationType AuthenticationType
        {
            get { return _settings.AuthenticationType; }
        }

        public string GetAuthorizationHeader()
        {
            if (_tokenProvider == null)
                return _basicHeader;

            return $"Bearer {_tokenProvider.GetAccessToken()}";
        }

        /// <summary>
        /// Called when SuccessFactors answers 401 with the given Authorization header, so the next request fetches a new token.
        /// Only that token is dropped; a newer one fetched meanwhile by a concurrent request is kept.
        /// </summary>
        public void OnUnauthorized(string rejectedAuthorizationHeader)
        {
            const string bearerPrefix = "Bearer ";
            if (_tokenProvider == null || rejectedAuthorizationHeader == null
                || !rejectedAuthorizationHeader.StartsWith(bearerPrefix, StringComparison.Ordinal))
                return;

            _tokenProvider.InvalidateToken(rejectedAuthorizationHeader.Substring(bearerPrefix.Length));
        }

        /// <summary>
        /// Logs that a request answered with 401 is sent once more with a new token.
        /// Only the method and the URL path are logged: never the token, the assertion or the query string (it can hold employee IDs).
        /// </summary>
        internal void LogRetry(string method, Uri url)
        {
            string path = url == null ? "(unknown)" : url.GetLeftPart(UriPartial.Path);
            _logWarning(UtilityMethods.StripLogForgingCharacters(
                $"SuccessFactors answered HTTP 401 (Unauthorized) to {method} {path}. The OAuth access token was discarded and the request is being sent once more with a new token."));
        }

        /// <summary>
        /// True when a 401 can be fixed by fetching a new token (OAuth).
        /// With Basic authentication a 401 means the password is wrong, so resending would only risk locking the account.
        /// </summary>
        public bool CanRetryUnauthorized
        {
            get { return _tokenProvider != null; }
        }

        /// <summary>
        /// Sends the request built by createRequest. When SuccessFactors answers 401 and OAuth is used,
        /// the rejected token is dropped and the request is built and sent once more with a new token.
        /// Only one retry; timeouts, network failures and other HTTP errors are never retried.
        /// </summary>
        public HttpWebResponse Send(Func<HttpWebRequest> createRequest)
        {
            HttpWebRequest request = createRequest();
            try
            {
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex) when (IsUnauthorized(ex.Response as HttpWebResponse))
            {
                OnUnauthorized(request.Headers["Authorization"]);
                if (!CanRetryUnauthorized)
                    throw;

                ex.Response.Dispose();
                LogRetry(request.Method, request.RequestUri);
            }

            HttpWebRequest retry = createRequest();
            try
            {
                return (HttpWebResponse)retry.GetResponse();
            }
            catch (WebException ex) when (IsUnauthorized(ex.Response as HttpWebResponse))
            {
                // The new token was rejected too: drop it so the next request starts with a fresh one.
                OnUnauthorized(retry.Headers["Authorization"]);
                throw;
            }
        }

        static bool IsUnauthorized(HttpWebResponse response)
        {
            return response != null && response.StatusCode == HttpStatusCode.Unauthorized;
        }

        static string GetBaseCredential(SF_AuthenticationSettings settings)
        {
            string creds = $"{settings.UserId}@{settings.CompanyId}:{settings.Password}";
            byte[] bcreds = Encoding.ASCII.GetBytes(creds);
            return Convert.ToBase64String(bcreds);
        }
    }
}
