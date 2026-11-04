using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Xunit;

namespace DI.Chakra.Connector.SuccessFactor.Tests
{
    /// <summary>Diagnostic logging that shows which authentication is used, without ever logging secrets.</summary>
    public class SF_DiagnosticLoggingTests
    {
        const string AccessToken = "SECRET-ACCESS-TOKEN-0123456789";

        [Fact]
        public void OAuth_NewToken_LogsCertificateAssertionAndTokenSteps_WithoutSecrets()
        {
            var settings = TestSupport.OAuthSettings();
            var logs = new List<string>();
            var handler = FakeTokenHandler.Returning(HttpStatusCode.OK, "{\"access_token\":\"" + AccessToken + "\",\"expires_in\":86400}");
            var provider = new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey, handler, logInfo: logs.Add);

            provider.GetAccessToken();

            Assert.Equal(3, logs.Count);
            Assert.StartsWith("SuccessFactors OAuth certificate 0123...4567 loaded (RSA 2048 bit)", logs[0]);
            Assert.StartsWith("SuccessFactors OAuth SAML assertion generated and signed (RSA-SHA256,", logs[1]);
            Assert.Equal("SuccessFactors OAuth token request successful (HTTP 200, expires in 86400 s).", logs[2]);

            string assertion = HttpUtility.ParseQueryString(handler.LastRequestBody)["assertion"];
            string assertionXml = Encoding.UTF8.GetString(Convert.FromBase64String(assertion));
            AssertNoSecrets(logs, AccessToken, assertion, assertionXml);
        }

        [Fact]
        public void OAuth_CachedToken_IsNotLoggedAgain()
        {
            var settings = TestSupport.OAuthSettings();
            var logs = new List<string>();
            var handler = FakeTokenHandler.Returning(HttpStatusCode.OK, "{\"access_token\":\"" + AccessToken + "\",\"expires_in\":86400}");
            var provider = new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey, handler, logInfo: logs.Add);

            provider.GetAccessToken();
            provider.GetAccessToken();

            Assert.Equal(3, logs.Count);
        }

        [Fact]
        public void OAuth_FailedTokenRequest_DoesNotLogSuccess()
        {
            var settings = TestSupport.OAuthSettings();
            var logs = new List<string>();
            var provider = new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey,
                                                     FakeTokenHandler.Returning(HttpStatusCode.Unauthorized, "{\"errorMessage\":\"bad signature\"}"),
                                                     logInfo: logs.Add);

            Assert.Throws<SF_AuthenticationException>(() => provider.GetAccessToken());

            Assert.DoesNotContain(logs, l => l.Contains("token request successful"));
        }

        [Fact]
        public void SelectedLine_OAuth_NamesOAuth_AndShortensClientIdAndThumbprint()
        {
            var settings = TestSupport.OAuthSettings();
            settings.ClientId = "NjI1MzU2YzUzODY3ZTk3ZGQ4NjM3MTViMjQyNQ";

            string line = settings.DescribeForLog();

            Assert.StartsWith("SuccessFactors authentication: OAuth (certificate) selected.", line);
            Assert.Contains("token URL https://api.example.test/oauth/token", line);
            Assert.Contains("client ID ...MjQyNQ", line);
            Assert.DoesNotContain(settings.ClientId, line);
            Assert.Contains("certificate 0123...4567", line);
            Assert.DoesNotContain("Basic", line);
        }

        [Fact]
        public void SelectedLine_Basic_NamesBasic_AndNeverThePassword()
        {
            var config = new Dictionary<EndpointKey, string>
            {
                { EndpointKey.UserName, "jdoe" },
                { EndpointKey.SF_Company, "ACMECORP" },
                { EndpointKey.Password, RSAHelper.EncryptData("S3cret!") },
            };
            var settings = SF_AuthenticationSettings.FromConfiguration(TestSupport.Config(config));

            string line = settings.DescribeForLog();

            Assert.Equal("SuccessFactors authentication: Basic selected. User jdoe@ACMECORP.", line);
            AssertNoSecrets(new[] { line }, "S3cret!", config[EndpointKey.Password], "amRvZUBBQ01FQ09SUDpTM2NyZXQh");
        }

        [Fact]
        public void SelectedLine_RemovesLineBreaksFromConfiguredValues()
        {
            var settings = TestSupport.OAuthSettings();
            settings.UserId = "user\r\nFAKE LOG LINE";

            Assert.DoesNotContain("\n", settings.DescribeForLog());
        }

        [Theory]
        [InlineData("Bearer abc.def.ghi", "Bearer")]
        [InlineData("Basic amRvZUBBQ01FQ09SUDpTM2NyZXQh", "Basic")]
        [InlineData("", "no")]
        [InlineData(null, "no")]
        [InlineData("garbage", "unknown")]
        public void GetScheme_ReturnsOnlyTheScheme(string header, string expected)
        {
            Assert.Equal(expected, SF_Authenticator.GetScheme(header));
        }

        [Fact]
        public void GetScheme_FromRealHeaders_NeverContainsCredentials()
        {
            var basic = new SF_Authenticator(SF_AuthenticationSettings.FromConfiguration(TestSupport.Config(new Dictionary<EndpointKey, string>
            {
                { EndpointKey.UserName, "jdoe" },
                { EndpointKey.SF_Company, "ACMECORP" },
                { EndpointKey.Password, RSAHelper.EncryptData("S3cret!") },
            })));
            var settings = TestSupport.OAuthSettings();
            var oauth = new SF_Authenticator(settings, new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey,
                FakeTokenHandler.Returning(HttpStatusCode.OK, "{\"access_token\":\"" + AccessToken + "\",\"expires_in\":86400}")));

            Assert.Equal("Basic", SF_Authenticator.GetScheme(basic.GetAuthorizationHeader()));
            Assert.Equal("Bearer", SF_Authenticator.GetScheme(oauth.GetAuthorizationHeader()));
        }

        static void AssertNoSecrets(IEnumerable<string> logs, params string[] secrets)
        {
            foreach (string line in logs)
            {
                foreach (string secret in secrets.Where(s => !string.IsNullOrEmpty(s)))
                    Assert.DoesNotContain(secret, line);
                Assert.DoesNotContain("saml2:", line);
                Assert.DoesNotContain("SignatureValue", line);
                Assert.DoesNotContain("PRIVATE KEY", line);
            }
        }
    }
}
