using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using Xunit;

namespace DI.Chakra.Connector.SuccessFactor.Tests
{
    public class SamlAssertionTests
    {
        const string Saml = "urn:oasis:names:tc:SAML:2.0:assertion";
        static readonly DateTime Now = new DateTime(2026, 10, 28, 8, 30, 15, DateTimeKind.Utc);

        public static RSA NewKey()
        {
            return new RSACng(2048);
        }

        public static bool SignatureIsValid(XmlDocument doc, RSA key)
        {
            var signedXml = new SignedXml(doc);
            var signature = (XmlElement)doc.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)[0];
            signedXml.LoadXml(signature);
            return signedXml.CheckSignature(key);
        }

        static XmlDocument Build(SF_AuthenticationSettings settings, RSA key, DateTime now)
        {
            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(SF_OAuthTokenProvider.BuildSignedAssertion(settings, key, now));
            return doc;
        }

        static XmlNamespaceManager Ns(XmlDocument doc)
        {
            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("saml2", Saml);
            ns.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
            return ns;
        }

        [Fact]
        public void Assertion_ContainsSuccessFactorsElements()
        {
            var settings = TestSupport.OAuthSettings();
            using (var key = NewKey())
            {
                var doc = Build(settings, key, Now);
                var ns = Ns(doc);

                Assert.Equal(settings.ClientId, doc.SelectSingleNode("/saml2:Assertion/saml2:Issuer", ns).InnerText);
                var nameId = (XmlElement)doc.SelectSingleNode("/saml2:Assertion/saml2:Subject/saml2:NameID", ns);
                Assert.Equal("sfapi_user", nameId.InnerText);
                Assert.Equal("urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified", nameId.GetAttribute("Format"));
                var confirmation = (XmlElement)doc.SelectSingleNode("//saml2:SubjectConfirmation", ns);
                Assert.Equal("urn:oasis:names:tc:SAML:2.0:cm:bearer", confirmation.GetAttribute("Method"));
                var confirmationData = (XmlElement)doc.SelectSingleNode("//saml2:SubjectConfirmationData", ns);
                Assert.Equal("https://api.example.test/oauth/token", confirmationData.GetAttribute("Recipient"));
                Assert.Equal("www.successfactors.com", doc.SelectSingleNode("//saml2:AudienceRestriction/saml2:Audience", ns).InnerText);
                Assert.Equal(settings.ClientId, doc.SelectSingleNode("//saml2:Attribute[@Name='api_key']/saml2:AttributeValue", ns).InnerText);
            }
        }

        [Fact]
        public void Assertion_IsSignedWithRsaSha256_AndSignatureFollowsIssuer()
        {
            using (var key = NewKey())
            {
                var doc = Build(TestSupport.OAuthSettings(), key, Now);
                var ns = Ns(doc);

                var issuer = doc.SelectSingleNode("/saml2:Assertion/saml2:Issuer", ns);
                Assert.Equal("Signature", issuer.NextSibling.LocalName);
                Assert.Equal("http://www.w3.org/2001/04/xmldsig-more#rsa-sha256",
                             ((XmlElement)doc.SelectSingleNode("//ds:SignatureMethod", ns)).GetAttribute("Algorithm"));
                Assert.Equal("#" + doc.DocumentElement.GetAttribute("ID"),
                             ((XmlElement)doc.SelectSingleNode("//ds:Reference", ns)).GetAttribute("URI"));
                Assert.True(SignatureIsValid(doc, key));
            }
        }

        [Fact]
        public void Assertion_TamperedContent_FailsSignatureCheck()
        {
            using (var key = NewKey())
            {
                var doc = Build(TestSupport.OAuthSettings(), key, Now);

                doc.SelectSingleNode("//saml2:NameID", Ns(doc)).InnerText = "someone_else";

                Assert.False(SignatureIsValid(doc, key));
            }
        }

        [Fact]
        public void Assertion_WrongKey_FailsSignatureCheck()
        {
            using (var key = NewKey())
            using (var otherKey = NewKey())
            {
                var doc = Build(TestSupport.OAuthSettings(), key, Now);

                Assert.False(SignatureIsValid(doc, otherKey));
            }
        }

        [Fact]
        public void Assertion_ValidityWindow()
        {
            using (var key = NewKey())
            {
                var doc = Build(TestSupport.OAuthSettings(), key, Now);
                var ns = Ns(doc);
                var conditions = (XmlElement)doc.SelectSingleNode("//saml2:Conditions", ns);

                Assert.Equal("2026-10-28T08:30:15Z", doc.DocumentElement.GetAttribute("IssueInstant"));
                Assert.Equal("2026-10-28T08:29:15Z", conditions.GetAttribute("NotBefore"));
                Assert.Equal("2026-10-28T08:40:15Z", conditions.GetAttribute("NotOnOrAfter"));
                Assert.Equal("2026-10-28T08:40:15Z", ((XmlElement)doc.SelectSingleNode("//saml2:SubjectConfirmationData", ns)).GetAttribute("NotOnOrAfter"));
            }
        }

        [Theory]
        [InlineData("th-TH")]   // Buddhist calendar: culture-sensitive formatting would give year 2569
        [InlineData("ar-SA")]   // Um Al Qura calendar
        [InlineData("de-DE")]
        [InlineData("fa-IR")]
        public void Assertion_TimestampsUseInvariantCulture(string culture)
        {
            using (TestSupport.UseCulture(culture))
            using (var key = NewKey())
            {
                var doc = Build(TestSupport.OAuthSettings(), key, Now);

                Assert.Equal("2026-10-28T08:30:15Z", doc.DocumentElement.GetAttribute("IssueInstant"));
                Assert.True(SignatureIsValid(doc, key));
            }
        }
    }

    public class DateFormattingTests
    {
        [Theory]
        [InlineData("en-US")]
        [InlineData("en-GB")]
        [InlineData("de-DE")]   // "." date separator
        [InlineData("th-TH")]   // Buddhist calendar
        [InlineData("ar-SA")]   // Um Al Qura calendar
        public void NotificationDate_IsDayMonthYearWithSlashes_InEveryCulture(string culture)
        {
            using (TestSupport.UseCulture(culture))
            {
                Assert.Equal("05/03/2026", SF_Helper.FormatNotificationDate(new DateTime(2026, 3, 5)));
            }
        }

        [Fact]
        public void SamlDate_IsUtcIso8601()
        {
            using (TestSupport.UseCulture("th-TH"))
            {
                Assert.Equal("2026-03-05T14:07:09Z",
                             SF_OAuthTokenProvider.FormatSamlDate(new DateTime(2026, 3, 5, 14, 7, 9, DateTimeKind.Utc)));
            }
        }
    }
}
