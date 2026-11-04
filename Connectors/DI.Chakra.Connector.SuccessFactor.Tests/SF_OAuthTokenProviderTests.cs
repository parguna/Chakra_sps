using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using Xunit;

namespace DI.Chakra.Connector.SuccessFactor.Tests
{
    public class SF_OAuthTokenProviderTests
    {
        const string TokenResponse = "{\"access_token\":\"abc123\",\"token_type\":\"Bearer\",\"expires_in\":86400}";

        [Fact]
        public void GetAccessToken_PostsSignedSamlBearerRequest()
        {
            var settings = TestSupport.OAuthSettings();
            var handler = FakeTokenHandler.Returning(HttpStatusCode.OK, TokenResponse);
            using (var key = new RSACng(2048))
            {
                var provider = new SF_OAuthTokenProvider(settings, () => ExportedCopy(key), handler);

                Assert.Equal("abc123", provider.GetAccessToken());

                Assert.Equal(new Uri("https://api.example.test/oauth/token"), handler.LastRequestUri);
                var form = HttpUtility.ParseQueryString(handler.LastRequestBody);
                Assert.Equal("ACMECORP", form["company_id"]);
                Assert.Equal(settings.ClientId, form["client_id"]);
                Assert.Equal("urn:ietf:params:oauth:grant-type:saml2-bearer", form["grant_type"]);

                var assertion = new XmlDocument { PreserveWhitespace = true };
                assertion.LoadXml(Encoding.UTF8.GetString(Convert.FromBase64String(form["assertion"])));
                Assert.True(SamlAssertionTests.SignatureIsValid(assertion, key));
            }
        }

        [Fact]
        public void GetAccessToken_ReusesCachedTokenUntilShortlyBeforeExpiry()
        {
            var settings = TestSupport.OAuthSettings();
            var handler = FakeTokenHandler.Returning(HttpStatusCode.OK, "{\"access_token\":\"abc123\",\"expires_in\":3600}");
            var now = new DateTime(2026, 10, 28, 8, 0, 0, DateTimeKind.Utc);
            var provider = new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey, handler, utcNow: () => now);

            provider.GetAccessToken();
            now = now.AddMinutes(54);
            provider.GetAccessToken();
            Assert.Equal(1, handler.RequestCount);

            // Refreshed 5 minutes before the 60 minute lifetime ends.
            now = now.AddMinutes(1);
            provider.GetAccessToken();
            Assert.Equal(2, handler.RequestCount);
        }

        [Fact]
        public void GetAccessToken_ShortLifetime_RefreshesAtHalfLife()
        {
            var settings = TestSupport.OAuthSettings();
            var handler = FakeTokenHandler.Returning(HttpStatusCode.OK, "{\"access_token\":\"abc123\",\"expires_in\":\"120\"}");
            var now = new DateTime(2026, 10, 28, 8, 0, 0, DateTimeKind.Utc);
            var provider = new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey, handler, utcNow: () => now);

            provider.GetAccessToken();
            now = now.AddSeconds(59);
            provider.GetAccessToken();
            now = now.AddSeconds(1);
            provider.GetAccessToken();

            Assert.Equal(2, handler.RequestCount);
        }

        [Fact]
        public void GetAccessToken_NoExpiry_IsNotCached()
        {
            var settings = TestSupport.OAuthSettings();
            var handler = FakeTokenHandler.Returning(HttpStatusCode.OK, "{\"access_token\":\"abc123\"}");
            var provider = new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey, handler);

            provider.GetAccessToken();
            provider.GetAccessToken();

            Assert.Equal(2, handler.RequestCount);
        }

        [Fact]
        public void GetAccessToken_CacheIsSharedAcrossConnectorInstancesForSameEndpoint()
        {
            var settings = TestSupport.OAuthSettings();
            var handler = FakeTokenHandler.Returning(HttpStatusCode.OK, TokenResponse);

            new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey, handler).GetAccessToken();
            new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey, handler).GetAccessToken();

            Assert.Equal(1, handler.RequestCount);
        }

        [Fact]
        public void TokenError_IncludesStatusAndSanitizedBody()
        {
            var settings = TestSupport.OAuthSettings();
            var handler = FakeTokenHandler.Returning(HttpStatusCode.Unauthorized,
                "{\"errorMessage\":\"Unable to validate the SAML assertion\"}\r\nFAKE LOG LINE");
            var provider = new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey, handler);

            var ex = Assert.Throws<SF_AuthenticationException>(() => provider.GetAccessToken());

            Assert.Contains("401", ex.Message);
            Assert.Contains("Unable to validate the SAML assertion", ex.Message);
            Assert.DoesNotContain("\n", ex.Message);
            Assert.DoesNotContain("\r", ex.Message);
        }

        [Fact]
        public void TokenError_LongBodyIsTruncated()
        {
            var settings = TestSupport.OAuthSettings();
            var handler = FakeTokenHandler.Returning(HttpStatusCode.BadRequest, new string('x', 5000));
            var provider = new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey, handler);

            var ex = Assert.Throws<SF_AuthenticationException>(() => provider.GetAccessToken());

            Assert.True(ex.Message.Length < 700);
        }

        [Theory]
        [InlineData("<html>Service Unavailable</html>", "not valid JSON")]
        [InlineData("{\"token_type\":\"Bearer\",\"expires_in\":3600}", "did not contain an access token")]
        [InlineData("{\"access_token\":\"\"}", "did not contain an access token")]
        public void TokenError_BadSuccessResponse(string body, string expectedMessage)
        {
            var settings = TestSupport.OAuthSettings();
            var provider = new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey, FakeTokenHandler.Returning(HttpStatusCode.OK, body));

            var ex = Assert.Throws<SF_AuthenticationException>(() => provider.GetAccessToken());

            Assert.Contains(expectedMessage, ex.Message);
        }

        [Fact]
        public void TokenError_NetworkFailureIsWrapped()
        {
            var settings = TestSupport.OAuthSettings();
            var handler = new FakeTokenHandler(_ => { throw new HttpRequestException("connection refused"); });
            var provider = new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey, handler);

            var ex = Assert.Throws<SF_AuthenticationException>(() => provider.GetAccessToken());

            Assert.Contains("connection refused", ex.Message);
            Assert.IsType<HttpRequestException>(ex.InnerException);
        }

        [Fact]
        public void TokenError_FailedRequestIsNotCached()
        {
            var settings = TestSupport.OAuthSettings();
            int call = 0;
            var handler = new FakeTokenHandler(_ => ++call == 1
                ? new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("boom") }
                : new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(TokenResponse) });
            var provider = new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey, handler);

            Assert.Throws<SF_AuthenticationException>(() => provider.GetAccessToken());
            Assert.Equal("abc123", provider.GetAccessToken());
        }

        [Fact]
        public void MissingCertificate_FailsBeforeAnyHttpCall()
        {
            var settings = TestSupport.OAuthSettings();
            settings.CertificateThumbprint = string.Concat(Enumerable.Repeat("FE", 20));
            var handler = FakeTokenHandler.Returning(HttpStatusCode.OK, TokenResponse);
            var provider = new SF_OAuthTokenProvider(settings, httpHandler: handler);

            var ex = Assert.Throws<SF_AuthenticationException>(() => provider.GetAccessToken());

            Assert.Contains("was not found", ex.Message);
            Assert.Equal(0, handler.RequestCount);
        }

        [Fact]
        public void CertificateStore_EmptyThumbprint_IsReported()
        {
            var ex = Assert.Throws<SF_AuthenticationException>(() => SF_CertificateStore.GetSigningKey("  "));
            Assert.Contains("not configured", ex.Message);
        }

        [Fact]
        public void CertificateStore_NormalizesThumbprintCopiedFromMmc()
        {
            Assert.Equal("ABCDEF0123", SF_CertificateStore.NormalizeThumbprint("‎ab cd ef 01 23"));
            Assert.Equal("", SF_CertificateStore.NormalizeThumbprint(null));
        }

        // The provider disposes the key after signing, so hand it a copy and keep the original for verification.
        static RSA ExportedCopy(RSA key)
        {
            var copy = new RSACng();
            copy.ImportParameters(key.ExportParameters(true));
            return copy;
        }
    }
}
