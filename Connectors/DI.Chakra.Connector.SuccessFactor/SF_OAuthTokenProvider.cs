using DI.Chakra.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace DI.Chakra.Connector
{
    /// <summary>
    /// SuccessFactors OAuth 2.0 SAML bearer flow:
    /// 1. Build a SAML 2.0 assertion locally and sign it with the private key of the certificate registered for the OAuth client.
    ///    (SAP's /oauth/idp endpoint is deprecated because it needs the private key to be sent over the wire.)
    /// 2. POST company_id, client_id, grant_type and the base64 assertion to {Base Company URL}/oauth/token.
    /// 3. Send "Authorization: Bearer {access_token}" on the OData requests. Tokens are cached until shortly before they expire.
    /// The private key never leaves the Windows certificate store, and the assertion and token are never logged.
    /// </summary>
    public class SF_OAuthTokenProvider
    {
        public const string SamlBearerGrantType = "urn:ietf:params:oauth:grant-type:saml2-bearer";
        public const string Audience = "www.successfactors.com";

        const string SamlNamespace = "urn:oasis:names:tc:SAML:2.0:assertion";
        const string RsaSha256SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        const string Sha256DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";
        const string SamlDateFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";
        const int MaxErrorBodyLength = 500;

        static readonly TimeSpan AssertionLifetime = TimeSpan.FromMinutes(10);
        static readonly TimeSpan ClockSkew = TimeSpan.FromMinutes(1);
        static readonly TimeSpan RefreshBeforeExpiry = TimeSpan.FromMinutes(5);

        static readonly ConcurrentDictionary<string, CachedToken> TokenCache = new ConcurrentDictionary<string, CachedToken>();
        static readonly ConcurrentDictionary<string, object> TokenLocks = new ConcurrentDictionary<string, object>();

        readonly SF_AuthenticationSettings _settings;
        readonly Func<RSA> _signingKeyProvider;
        readonly HttpMessageHandler _httpHandler;
        readonly TimeSpan _timeout;
        readonly Func<DateTime> _utcNow;
        readonly string _cacheKey;

        public SF_OAuthTokenProvider(SF_AuthenticationSettings settings,
                                     Func<RSA> signingKeyProvider = null,
                                     HttpMessageHandler httpHandler = null,
                                     TimeSpan? timeout = null,
                                     Func<DateTime> utcNow = null)
        {
            _settings = settings;
            _signingKeyProvider = signingKeyProvider ?? (() => SF_CertificateStore.GetSigningKey(settings.CertificateThumbprint));
            _httpHandler = httpHandler;
            _timeout = timeout ?? TimeSpan.FromSeconds(100);
            _utcNow = utcNow ?? (() => DateTime.UtcNow);
            _cacheKey = string.Join("|", settings.TokenUrl, settings.CompanyId, settings.ClientId, settings.UserId,
                                    SF_CertificateStore.NormalizeThumbprint(settings.CertificateThumbprint));
        }

        public string GetAccessToken()
        {
            CachedToken token;
            if (TryGetCachedToken(out token))
                return token.AccessToken;

            lock (TokenLocks.GetOrAdd(_cacheKey, _ => new object()))
            {
                if (TryGetCachedToken(out token))
                    return token.AccessToken;

                token = RequestToken();
                if (token.RefreshAtUtc.HasValue)
                    TokenCache[_cacheKey] = token;
                else
                    TokenCache.TryRemove(_cacheKey, out _);

                return token.AccessToken;
            }
        }

        /// <summary>
        /// Drops the cached token, but only if it is still the one SuccessFactors rejected.
        /// A newer token already fetched by a concurrent request is kept, so it is not thrown away and fetched again.
        /// </summary>
        public void InvalidateToken(string rejectedAccessToken)
        {
            CachedToken cached;
            if (TokenCache.TryGetValue(_cacheKey, out cached)
                && string.Equals(cached.AccessToken, rejectedAccessToken, StringComparison.Ordinal))
            {
                // Atomic compare-and-remove: removes the entry only if it is still this exact CachedToken instance.
                ((ICollection<KeyValuePair<string, CachedToken>>)TokenCache).Remove(new KeyValuePair<string, CachedToken>(_cacheKey, cached));
            }
        }

        bool TryGetCachedToken(out CachedToken token)
        {
            return TokenCache.TryGetValue(_cacheKey, out token)
                   && token.RefreshAtUtc.HasValue
                   && _utcNow() < token.RefreshAtUtc.Value;
        }

        CachedToken RequestToken()
        {
            DateTime now = _utcNow();
            string assertion;
            using (RSA signingKey = _signingKeyProvider())
            {
                try
                {
                    assertion = BuildSignedAssertion(_settings, signingKey, now);
                }
                catch (CryptographicException ex)
                {
                    throw new SF_AuthenticationException("SuccessFactors OAuth assertion could not be signed with RSA-SHA256. If the key was imported into a legacy CSP, re-import the PFX into the Microsoft Software Key Storage Provider.", ex);
                }
            }

            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("company_id", _settings.CompanyId),
                new KeyValuePair<string, string>("client_id", _settings.ClientId),
                new KeyValuePair<string, string>("grant_type", _settings.GrantType),
                new KeyValuePair<string, string>("assertion", Convert.ToBase64String(Encoding.UTF8.GetBytes(assertion))),
            });

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            HttpStatusCode statusCode;
            string body;
            try
            {
                using (var client = _httpHandler == null ? new HttpClient() : new HttpClient(_httpHandler, false))
                {
                    client.Timeout = _timeout;
                    using (HttpResponseMessage response = client.PostAsync(_settings.TokenUrl, form).ConfigureAwait(false).GetAwaiter().GetResult())
                    {
                        statusCode = response.StatusCode;
                        body = response.Content == null
                            ? string.Empty
                            : response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                }
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is OperationCanceledException)
            {
                throw new SF_AuthenticationException($"SuccessFactors OAuth token request to {_settings.TokenUrl} failed: {ex.Message}", ex);
            }

            if ((int)statusCode < 200 || (int)statusCode > 299)
                throw new SF_AuthenticationException($"SuccessFactors OAuth token request failed with HTTP {(int)statusCode} ({statusCode}): {SanitizeErrorBody(body)}");

            JObject json;
            try
            {
                json = JObject.Parse(body);
            }
            catch (JsonException)
            {
                throw new SF_AuthenticationException("SuccessFactors OAuth token response was not valid JSON.");
            }

            string accessToken = (string)json["access_token"];
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new SF_AuthenticationException("SuccessFactors OAuth token response did not contain an access token.");

            int expiresIn;
            DateTime? refreshAt = null;
            if (int.TryParse(Convert.ToString(json["expires_in"], CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out expiresIn)
                && expiresIn > 0)
            {
                var lifetime = TimeSpan.FromSeconds(expiresIn);
                var margin = lifetime < RefreshBeforeExpiry + RefreshBeforeExpiry ? TimeSpan.FromTicks(lifetime.Ticks / 2) : RefreshBeforeExpiry;
                refreshAt = now + lifetime - margin;
            }

            return new CachedToken { AccessToken = accessToken, RefreshAtUtc = refreshAt };
        }

        static string SanitizeErrorBody(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return "(empty response)";

            string trimmed = body.Length > MaxErrorBodyLength ? body.Substring(0, MaxErrorBodyLength) + "..." : body;
            return UtilityMethods.StripLogForgingCharacters(trimmed);
        }

        /// <summary>
        /// Builds and signs (enveloped, RSA-SHA256, exclusive C14N) the SAML 2.0 bearer assertion sent to /oauth/token.
        /// </summary>
        public static string BuildSignedAssertion(SF_AuthenticationSettings settings, RSA signingKey, DateTime utcNow)
        {
            string id = "_" + Guid.NewGuid().ToString("N");
            string issueInstant = FormatSamlDate(utcNow);
            string notBefore = FormatSamlDate(utcNow - ClockSkew);
            string notOnOrAfter = FormatSamlDate(utcNow + AssertionLifetime);

            var doc = new XmlDocument { PreserveWhitespace = true };
            XmlElement assertion = doc.CreateElement("saml2", "Assertion", SamlNamespace);
            assertion.SetAttribute("ID", id);
            assertion.SetAttribute("IssueInstant", issueInstant);
            assertion.SetAttribute("Version", "2.0");
            doc.AppendChild(assertion);

            XmlElement issuer = AppendSamlElement(assertion, "Issuer", settings.ClientId);

            XmlElement subject = AppendSamlElement(assertion, "Subject");
            XmlElement nameId = AppendSamlElement(subject, "NameID", settings.UserId);
            nameId.SetAttribute("Format", "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified");
            XmlElement subjectConfirmation = AppendSamlElement(subject, "SubjectConfirmation");
            subjectConfirmation.SetAttribute("Method", "urn:oasis:names:tc:SAML:2.0:cm:bearer");
            XmlElement subjectConfirmationData = AppendSamlElement(subjectConfirmation, "SubjectConfirmationData");
            subjectConfirmationData.SetAttribute("NotOnOrAfter", notOnOrAfter);
            subjectConfirmationData.SetAttribute("Recipient", settings.TokenUrl);

            XmlElement conditions = AppendSamlElement(assertion, "Conditions");
            conditions.SetAttribute("NotBefore", notBefore);
            conditions.SetAttribute("NotOnOrAfter", notOnOrAfter);
            AppendSamlElement(AppendSamlElement(conditions, "AudienceRestriction"), "Audience", Audience);

            XmlElement authnStatement = AppendSamlElement(assertion, "AuthnStatement");
            authnStatement.SetAttribute("AuthnInstant", issueInstant);
            authnStatement.SetAttribute("SessionIndex", "_" + Guid.NewGuid().ToString("N"));
            AppendSamlElement(AppendSamlElement(authnStatement, "AuthnContext"), "AuthnContextClassRef",
                              "urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport");

            XmlElement attribute = AppendSamlElement(AppendSamlElement(assertion, "AttributeStatement"), "Attribute");
            attribute.SetAttribute("Name", "api_key");
            AppendSamlElement(attribute, "AttributeValue", settings.ClientId);

            var signedXml = new SignedXml(doc) { SigningKey = signingKey };
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signedXml.SignedInfo.SignatureMethod = RsaSha256SignatureMethod;
            var reference = new Reference("#" + id) { DigestMethod = Sha256DigestMethod };
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());
            signedXml.AddReference(reference);
            signedXml.ComputeSignature();

            // SAML schema requires the Signature immediately after the Issuer.
            assertion.InsertAfter(doc.ImportNode(signedXml.GetXml(), true), issuer);

            return doc.OuterXml;
        }

        public static string FormatSamlDate(DateTime utc)
        {
            return utc.ToUniversalTime().ToString(SamlDateFormat, CultureInfo.InvariantCulture);
        }

        static XmlElement AppendSamlElement(XmlElement parent, string name, string text = null)
        {
            XmlElement element = parent.OwnerDocument.CreateElement("saml2", name, SamlNamespace);
            if (text != null)
                element.InnerText = text;
            parent.AppendChild(element);
            return element;
        }

        class CachedToken
        {
            public string AccessToken { get; set; }

            public DateTime? RefreshAtUtc { get; set; }
        }
    }

    /// <summary>
    /// Loads the OAuth signing key from the Windows certificate store (LocalMachine\My, then CurrentUser\My) by thumbprint.
    /// Only the thumbprint is stored in the endpoint configuration; the private key stays in the store.
    /// </summary>
    public static class SF_CertificateStore
    {
        public static RSA GetSigningKey(string thumbprint)
        {
            string normalized = NormalizeThumbprint(thumbprint);
            if (string.IsNullOrEmpty(normalized))
                throw new SF_AuthenticationException("SuccessFactors OAuth certificate thumbprint is not configured.");

            foreach (StoreLocation location in new[] { StoreLocation.LocalMachine, StoreLocation.CurrentUser })
            {
                X509Certificate2Collection found;
                var store = new X509Store(StoreName.My, location);
                try
                {
                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                    found = store.Certificates.Find(X509FindType.FindByThumbprint, normalized, false);
                }
                catch (CryptographicException)
                {
                    continue;
                }
                finally
                {
                    store.Close();
                }

                if (found.Count == 0)
                    continue;

                X509Certificate2 certificate = found[0];
                if (!certificate.HasPrivateKey)
                    throw new SF_AuthenticationException($"SuccessFactors OAuth certificate {normalized} in {location}\\My has no private key.");

                RSA key;
                try
                {
                    key = certificate.GetRSAPrivateKey();
                }
                catch (CryptographicException ex)
                {
                    throw new SF_AuthenticationException($"SuccessFactors OAuth certificate {normalized} private key could not be opened. Check that the service account has read access to the private key.", ex);
                }

                if (key == null)
                    throw new SF_AuthenticationException($"SuccessFactors OAuth certificate {normalized} does not have an RSA private key.");

                return key;
            }

            throw new SF_AuthenticationException($"SuccessFactors OAuth certificate {normalized} was not found in LocalMachine\\My or CurrentUser\\My.");
        }

        /// <summary>Removes spaces and hidden characters copied from the certificate MMC, and upper-cases the result.</summary>
        public static string NormalizeThumbprint(string thumbprint)
        {
            return new string((thumbprint ?? string.Empty).Where(Uri.IsHexDigit).ToArray()).ToUpperInvariant();
        }
    }
}
