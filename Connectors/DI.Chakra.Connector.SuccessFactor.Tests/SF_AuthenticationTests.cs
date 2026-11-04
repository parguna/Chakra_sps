using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace DI.Chakra.Connector.SuccessFactor.Tests
{
    public class SF_AuthenticationTests
    {
        static Dictionary<EndpointKey, string> BasicConfig()
        {
            return new Dictionary<EndpointKey, string>
            {
                { EndpointKey.UserName, "jdoe" },
                { EndpointKey.SF_Company, "ACMECORP" },
                { EndpointKey.Password, RSAHelper.EncryptData("S3cret!") },
                { EndpointKey.WSDLURL, "https://api.example.test/odata/v2" },
            };
        }

        [Fact]
        public void Basic_ExistingEndpointWithoutOAuthKey_ProducesSameHeaderAsBefore()
        {
            var settings = SF_AuthenticationSettings.FromConfiguration(TestSupport.Config(BasicConfig()));
            var authenticator = new SF_Authenticator(settings);

            Assert.Equal(SF_AuthenticationType.Basic, authenticator.AuthenticationType);
            // base64("jdoe@ACMECORP:S3cret!"), the value the connector produced before OAuth was added.
            Assert.Equal("Basic amRvZUBBQ01FQ09SUDpTM2NyZXQh", authenticator.GetAuthorizationHeader());
        }

        [Theory]
        [InlineData("False")]
        [InlineData("false")]
        [InlineData("")]
        [InlineData("not-a-bool")]
        public void Basic_OAuthFlagNotTrue_StaysBasic(string flag)
        {
            var config = BasicConfig();
            config[EndpointKey.SF_EnableOAuth] = flag;

            var settings = SF_AuthenticationSettings.FromConfiguration(TestSupport.Config(config));

            Assert.Equal(SF_AuthenticationType.Basic, settings.AuthenticationType);
            Assert.Equal("S3cret!", settings.Password);
        }

        [Fact]
        public void Basic_MissingValues_DoesNotThrow_SoExistingEndpointsBehaveAsBefore()
        {
            var settings = SF_AuthenticationSettings.FromConfiguration(TestSupport.Config(new Dictionary<EndpointKey, string>()));

            var authenticator = new SF_Authenticator(settings);

            Assert.StartsWith("Basic ", authenticator.GetAuthorizationHeader());
        }

        [Fact]
        public void OAuth_Enabled_ReadsOAuthKeys_AndNeverReadsPassword()
        {
            var accessed = new List<EndpointKey>();
            var config = new Dictionary<EndpointKey, string>
            {
                { EndpointKey.SF_EnableOAuth, "True" },
                { EndpointKey.UserName, "sfapi_user" },
                { EndpointKey.SF_Company, "ACMECORP" },
                { EndpointKey.SF_BaseCompanyUrl, "https://api.example.test/" },
                { EndpointKey.SF_ClientId, "client-1" },
                { EndpointKey.SF_CertificateThumbprint, "AB CD" },
            };

            var settings = SF_AuthenticationSettings.FromConfiguration(TestSupport.Config(config, accessed));

            Assert.Equal(SF_AuthenticationType.OAuth, settings.AuthenticationType);
            Assert.DoesNotContain(EndpointKey.Password, accessed);
            Assert.Null(settings.Password);
            Assert.Equal("sfapi_user", settings.UserId);
            Assert.Equal("ACMECORP", settings.CompanyId);
            Assert.Equal("client-1", settings.ClientId);
            Assert.Equal("https://api.example.test/oauth/token", settings.TokenUrl);
            Assert.Equal(SF_OAuthTokenProvider.SamlBearerGrantType, settings.GrantType);
        }

        [Fact]
        public void OAuth_MissingSettings_ListsEverySettingWithoutValues()
        {
            var settings = new SF_AuthenticationSettings
            {
                AuthenticationType = SF_AuthenticationType.OAuth,
                UserId = "secret-user-value",
                GrantType = SF_OAuthTokenProvider.SamlBearerGrantType,
            };

            var ex = Assert.Throws<SF_AuthenticationException>(() => settings.Validate());

            Assert.Contains("Base Company URL", ex.Message);
            Assert.Contains("Company ID", ex.Message);
            Assert.Contains("Client ID", ex.Message);
            Assert.Contains("Certificate Thumbprint", ex.Message);
            Assert.DoesNotContain("User ID", ex.Message);
            Assert.DoesNotContain("secret-user-value", ex.Message);
        }

        [Fact]
        public void OAuth_MissingSettings_FailsWhenAuthenticatorIsCreated()
        {
            var settings = TestSupport.OAuthSettings();
            settings.ClientId = " ";

            Assert.Throws<SF_AuthenticationException>(() => new SF_Authenticator(settings));
        }

        [Theory]
        [InlineData("http://api.example.test")]
        [InlineData("api.example.test")]
        public void OAuth_BaseCompanyUrlMustBeAbsoluteHttps(string url)
        {
            var settings = TestSupport.OAuthSettings();
            settings.BaseCompanyUrl = url;

            var ex = Assert.Throws<SF_AuthenticationException>(() => settings.Validate());
            Assert.Contains("https", ex.Message);
        }

        [Fact]
        public void OAuth_UnsupportedGrantType_IsRejected()
        {
            var settings = TestSupport.OAuthSettings();
            settings.GrantType = "client_credentials";

            var ex = Assert.Throws<SF_AuthenticationException>(() => settings.Validate());
            Assert.Contains("Grant Type", ex.Message);
        }

        [Fact]
        public void OAuth_HeaderIsBearerToken_And401InvalidatesCachedToken()
        {
            var settings = TestSupport.OAuthSettings();
            int call = 0;
            var handler = new FakeTokenHandler(_ =>
            {
                call++;
                return new System.Net.Http.HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new System.Net.Http.StringContent($"{{\"access_token\":\"token-{call}\",\"token_type\":\"Bearer\",\"expires_in\":86400}}")
                };
            });
            var provider = new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey, handler);
            var authenticator = new SF_Authenticator(settings, provider);

            Assert.Equal("Bearer token-1", authenticator.GetAuthorizationHeader());
            Assert.Equal("Bearer token-1", authenticator.GetAuthorizationHeader());

            authenticator.OnUnauthorized("Bearer token-1");

            Assert.Equal("Bearer token-2", authenticator.GetAuthorizationHeader());
            Assert.Equal(2, handler.RequestCount);
        }
    }
}
