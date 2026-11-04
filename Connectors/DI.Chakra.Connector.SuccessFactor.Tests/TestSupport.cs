using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.SuccessFactor.Tests
{
    static class TestSupport
    {
        public const string BaseCompanyUrl = "https://api.example.test";

        /// <summary>Mirrors SF_Helper.Config: missing keys come back as "".</summary>
        public static Func<EndpointKey, string> Config(Dictionary<EndpointKey, string> values, List<EndpointKey> accessed = null)
        {
            return key =>
            {
                accessed?.Add(key);
                string value;
                return values.TryGetValue(key, out value) ? value : "";
            };
        }

        /// <summary>OAuth settings with a unique client ID, so the shared token cache does not leak between tests.</summary>
        public static SF_AuthenticationSettings OAuthSettings()
        {
            return new SF_AuthenticationSettings
            {
                AuthenticationType = SF_AuthenticationType.OAuth,
                BaseCompanyUrl = BaseCompanyUrl,
                CompanyId = "ACMECORP",
                UserId = "sfapi_user",
                ClientId = "client-" + Guid.NewGuid().ToString("N"),
                CertificateThumbprint = "0123456789ABCDEF0123456789ABCDEF01234567",
                GrantType = SF_OAuthTokenProvider.SamlBearerGrantType,
            };
        }

        public static IDisposable UseCulture(string name)
        {
            return new CultureScope(new CultureInfo(name));
        }

        sealed class CultureScope : IDisposable
        {
            readonly CultureInfo _culture;
            readonly CultureInfo _uiCulture;

            public CultureScope(CultureInfo culture)
            {
                _culture = Thread.CurrentThread.CurrentCulture;
                _uiCulture = Thread.CurrentThread.CurrentUICulture;
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }

            public void Dispose()
            {
                Thread.CurrentThread.CurrentCulture = _culture;
                Thread.CurrentThread.CurrentUICulture = _uiCulture;
            }
        }
    }

    /// <summary>Records token requests and replies with a canned response.</summary>
    class FakeTokenHandler : HttpMessageHandler
    {
        readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

        public FakeTokenHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
        {
            _respond = respond;
        }

        public static FakeTokenHandler Returning(HttpStatusCode status, string body)
        {
            return new FakeTokenHandler(_ => new HttpResponseMessage(status) { Content = new StringContent(body) });
        }

        public int RequestCount { get; private set; }

        public Uri LastRequestUri { get; private set; }

        public string LastRequestBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            LastRequestUri = request.RequestUri;
            LastRequestBody = request.Content == null ? null : await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            return _respond(request);
        }
    }
}
