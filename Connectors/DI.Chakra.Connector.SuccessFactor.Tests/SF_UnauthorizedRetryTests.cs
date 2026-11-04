using Microsoft.Data.OData;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DI.Chakra.Connector.SuccessFactor.Tests
{
    public class SF_UnauthorizedRetryTests
    {
        const string EmployeeId = "10045678";

        /// <summary>OAuth authenticator whose token endpoint hands out token-1, token-2, ...; warnings are collected.</summary>
        static SF_Authenticator OAuthAuthenticator(out FakeTokenHandler tokenHandler, List<string> warnings = null)
        {
            var settings = TestSupport.OAuthSettings();
            int call = 0;
            tokenHandler = new FakeTokenHandler(_ =>
            {
                call++;
                return new System.Net.Http.HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new System.Net.Http.StringContent($"{{\"access_token\":\"token-{call}\",\"expires_in\":86400}}")
                };
            });
            return new SF_Authenticator(settings, new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey, tokenHandler), Collect(warnings));
        }

        static SF_Authenticator BasicAuthenticator(List<string> warnings = null)
        {
            return new SF_Authenticator(new SF_AuthenticationSettings
            {
                AuthenticationType = SF_AuthenticationType.Basic,
                UserId = "jdoe",
                CompanyId = "ACMECORP",
                Password = "S3cret!",
            }, null, Collect(warnings));
        }

        static Action<string> Collect(List<string> warnings)
        {
            if (warnings == null)
                return null;
            return message => { lock (warnings) warnings.Add(message); };
        }

        static DataServiceClientRequestMessageArgs GetArgs()
        {
            return new DataServiceClientRequestMessageArgs("GET", new Uri($"https://api.example.test/odata/v2/User?$filter=userId eq '{EmployeeId}'"),
                                                           false, false, new Dictionary<string, string>());
        }

        /// <summary>Creates the wrapper the way the EntityContainer does, then sets headers like SendingRequest2.</summary>
        static SF_RetryingRequestMessage NewODataRequest(SF_Authenticator authenticator, List<FakeRequestMessage> sent,
                                                         params Func<FakeRequestMessage, IODataResponseMessage>[] responses)
        {
            var message = new SF_RetryingRequestMessage(GetArgs(), authenticator, args =>
            {
                var fake = new FakeRequestMessage(args, responses[sent.Count]);
                sent.Add(fake);
                return fake;
            });
            message.SetHeader("Authorization", authenticator.GetAuthorizationHeader());
            message.SetHeader("Pragma", "no-cache");
            return message;
        }

        static Func<FakeRequestMessage, IODataResponseMessage> Respond(HttpStatusCode status)
        {
            return _ => new FakeResponseMessage((int)status);
        }

        static Func<FakeRequestMessage, IODataResponseMessage> Throw(HttpStatusCode status)
        {
            return _ => { throw new DataServiceTransportException(new FakeResponseMessage((int)status), new WebException("transport")); };
        }

        /// <summary>The retry warning names the method and path only: no token, no query string (employee IDs), no credentials.</summary>
        static void AssertSingleSafeRetryWarning(List<string> warnings, string method, string path)
        {
            string warning = Assert.Single(warnings);
            Assert.Contains("401", warning);
            Assert.Contains($"{method} {path}", warning);
            Assert.DoesNotContain("token-", warning);
            Assert.DoesNotContain("Bearer", warning);
            Assert.DoesNotContain("?", warning);
            Assert.DoesNotContain(EmployeeId, warning);
            Assert.DoesNotContain("purgeType", warning);
        }

        #region OData

        [Fact]
        public void OData_401_IsResentOnceWithNewToken_AndSameHeaders()
        {
            FakeTokenHandler tokens;
            var warnings = new List<string>();
            var authenticator = OAuthAuthenticator(out tokens, warnings);
            var sent = new List<FakeRequestMessage>();
            var message = NewODataRequest(authenticator, sent, Respond(HttpStatusCode.Unauthorized), Respond(HttpStatusCode.OK));

            Assert.Equal(200, message.GetResponse().StatusCode);
            Assert.Equal(2, sent.Count);
            Assert.Equal("Bearer token-1", sent[0].GetHeader("Authorization"));
            Assert.Equal("Bearer token-2", sent[1].GetHeader("Authorization"));
            Assert.Equal("no-cache", sent[1].GetHeader("Pragma"));
            Assert.Equal(2, tokens.RequestCount);
            AssertSingleSafeRetryWarning(warnings, "GET", "https://api.example.test/odata/v2/User");
        }

        [Fact]
        public void OData_401_ThrownAsTransportException_IsResent()
        {
            FakeTokenHandler tokens;
            var authenticator = OAuthAuthenticator(out tokens);
            var sent = new List<FakeRequestMessage>();
            var message = NewODataRequest(authenticator, sent, Throw(HttpStatusCode.Unauthorized), Respond(HttpStatusCode.OK));

            Assert.Equal(200, message.GetResponse().StatusCode);
            Assert.Equal("Bearer token-2", sent[1].GetHeader("Authorization"));
        }

        [Fact]
        public void OData_401_OnRetryToo_IsNotResentAgain_AndDropsTheRetryToken()
        {
            FakeTokenHandler tokens;
            var warnings = new List<string>();
            var authenticator = OAuthAuthenticator(out tokens, warnings);
            var sent = new List<FakeRequestMessage>();
            var message = NewODataRequest(authenticator, sent, Respond(HttpStatusCode.Unauthorized), Throw(HttpStatusCode.Unauthorized));

            Assert.Throws<DataServiceTransportException>(() => message.GetResponse());
            Assert.Equal(2, sent.Count);
            Assert.Single(warnings);
            Assert.Equal("Bearer token-3", authenticator.GetAuthorizationHeader());
        }

        [Fact]
        public void OData_Basic_401_IsNotResent_AndNotLogged()
        {
            var warnings = new List<string>();
            var sent = new List<FakeRequestMessage>();
            var message = NewODataRequest(BasicAuthenticator(warnings), sent, Throw(HttpStatusCode.Unauthorized));

            Assert.Throws<DataServiceTransportException>(() => message.GetResponse());
            Assert.Single(sent);
            Assert.Empty(warnings);
        }

        [Fact]
        public void OData_RequestWithBody_IsNotResent()
        {
            FakeTokenHandler tokens;
            var warnings = new List<string>();
            var authenticator = OAuthAuthenticator(out tokens, warnings);
            var sent = new List<FakeRequestMessage>();
            var message = NewODataRequest(authenticator, sent, Respond(HttpStatusCode.Unauthorized));
            message.GetStream();

            Assert.Equal(401, message.GetResponse().StatusCode);
            Assert.Single(sent);
            Assert.Empty(warnings);
        }

        [Theory]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public void OData_OtherErrors_AreNotResent(HttpStatusCode status)
        {
            FakeTokenHandler tokens;
            var warnings = new List<string>();
            var authenticator = OAuthAuthenticator(out tokens, warnings);
            var sent = new List<FakeRequestMessage>();
            var message = NewODataRequest(authenticator, sent, Throw(status));

            Assert.Throws<DataServiceTransportException>(() => message.GetResponse());
            Assert.Single(sent);
            Assert.Equal(1, tokens.RequestCount);
            Assert.Empty(warnings);
        }

        [Fact]
        public void OData_NetworkFailure_WithoutResponse_IsNotResent()
        {
            FakeTokenHandler tokens;
            var warnings = new List<string>();
            var authenticator = OAuthAuthenticator(out tokens, warnings);
            var sent = new List<FakeRequestMessage>();
            var message = NewODataRequest(authenticator, sent, _ =>
            {
                throw new DataServiceTransportException(null, new WebException("connection reset", WebExceptionStatus.ConnectionClosed));
            });

            Assert.Throws<DataServiceTransportException>(() => message.GetResponse());
            Assert.Single(sent);
            Assert.Equal(1, tokens.RequestCount);
            Assert.Empty(warnings);
        }

        [Fact]
        public void OData_RealHttpMessage_401_IsResentWithReplayedHeaders()
        {
            FakeTokenHandler tokens;
            var authenticator = OAuthAuthenticator(out tokens);
            using (var server = new FakeUpsertServer(HttpStatusCode.Unauthorized, HttpStatusCode.OK))
            {
                var args = new DataServiceClientRequestMessageArgs("GET", new Uri(server.Url + "odata/v2/User"), false, false,
                                                                   new Dictionary<string, string>());
                var message = new SF_RetryingRequestMessage(args, authenticator);
                // Headers the EntityContainer and SendingRequest2 set, including ones HttpWebRequest restricts.
                message.SetHeader("Accept", "application/atom+xml,application/xml");
                message.SetHeader("User-Agent", "Microsoft ADO.NET Data Services");
                message.SetHeader("DataServiceVersion", "2.0;NetFx");
                message.SetHeader("Authorization", authenticator.GetAuthorizationHeader());
                message.SetHeader("Pragma", "no-cache");
                message.Timeout = 30000;

                IODataResponseMessage response = message.GetResponse();
                (response as IDisposable)?.Dispose();

                Assert.Equal(200, response.StatusCode);
                Assert.Equal(new[] { "Bearer token-1", "Bearer token-2" }, server.Authorizations);
            }
        }

        #endregion OData

        #region Token cache race

        /// <summary>
        /// Requests A and B were both sent with token-1. B gets its 401 first and fetches token-2.
        /// A's 401 arrives afterwards: it must not throw away token-2 (which would fetch token-3 for nothing).
        /// </summary>
        [Fact]
        public void LateUnauthorizedForOldToken_KeepsNewerTokenFetchedByAnotherRequest()
        {
            FakeTokenHandler tokens;
            var authenticator = OAuthAuthenticator(out tokens);
            string sentByAAndB = authenticator.GetAuthorizationHeader();

            authenticator.OnUnauthorized(sentByAAndB);                          // B: 401
            Assert.Equal("Bearer token-2", authenticator.GetAuthorizationHeader()); // B: retry fetches token-2

            authenticator.OnUnauthorized(sentByAAndB);                          // A: late 401 for token-1

            Assert.Equal("Bearer token-2", authenticator.GetAuthorizationHeader());
            Assert.Equal(2, tokens.RequestCount);
        }

        [Fact]
        public void LateUnauthorizedForOldToken_FromAnotherConnectorInstance_KeepsNewerToken()
        {
            var settings = TestSupport.OAuthSettings();
            int call = 0;
            var handler = new FakeTokenHandler(_ =>
            {
                call++;
                return new System.Net.Http.HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new System.Net.Http.StringContent($"{{\"access_token\":\"token-{call}\",\"expires_in\":86400}}")
                };
            });
            // Two connectors for the same endpoint share the static token cache.
            var connectorA = new SF_Authenticator(settings, new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey, handler));
            var connectorB = new SF_Authenticator(settings, new SF_OAuthTokenProvider(settings, SamlAssertionTests.NewKey, handler));
            string sentByA = connectorA.GetAuthorizationHeader();
            string sentByB = connectorB.GetAuthorizationHeader();
            Assert.Equal(sentByA, sentByB);

            connectorB.OnUnauthorized(sentByB);
            Assert.Equal("Bearer token-2", connectorB.GetAuthorizationHeader());
            connectorA.OnUnauthorized(sentByA);

            Assert.Equal("Bearer token-2", connectorA.GetAuthorizationHeader());
            Assert.Equal(2, handler.RequestCount);
        }

        /// <summary>
        /// Concurrency check: many requests rejected at the same moment share one new token and each retries once.
        /// The exact race (a late 401 removing a newer token) is reproduced deterministically by the two tests above;
        /// this one does not reliably hit that window, it guards against deadlocks and extra fetches under load.
        /// </summary>
        [Fact]
        public async Task ConcurrentRequests_401OnSameToken_FetchExactlyOneNewToken()
        {
            const int requestCount = 8;
            FakeTokenHandler tokens;
            var warnings = new List<string>();
            var authenticator = OAuthAuthenticator(out tokens, warnings);
            Func<DataServiceClientRequestMessageArgs, DataServiceClientRequestMessage> createMessage =
                args => new FakeRequestMessage(args,
                                               m => new FakeResponseMessage(m.GetHeader("Authorization") == "Bearer token-1" ? 401 : 200));

            // Every request is sent with token-1 before any 401 comes back, then all of them get their 401 at the same time.
            var messages = Enumerable.Range(0, requestCount).Select(_ =>
            {
                var message = new SF_RetryingRequestMessage(GetArgs(), authenticator, createMessage);
                message.SetHeader("Authorization", authenticator.GetAuthorizationHeader());
                return message;
            }).ToList();

            using (var start = new Barrier(requestCount))
            {
                var results = messages.Select(message => Task.Factory.StartNew(() =>
                {
                    start.SignalAndWait();
                    return message.GetResponse().StatusCode;
                }, TaskCreationOptions.LongRunning)).ToArray();

                Task<int[]> all = Task.WhenAll(results);
                Assert.Same(all, await Task.WhenAny(all, Task.Delay(TimeSpan.FromSeconds(30))));
                Assert.All(await all, status => Assert.Equal(200, status));
            }

            Assert.Equal(2, tokens.RequestCount);   // token-1, then a single token-2 shared by every retry
            Assert.Equal(requestCount, warnings.Count);
        }

        #endregion Token cache race

        #region Upsert

        /// <summary>
        /// Builds the request the same way SuccessFactorConnector.GetHttpPostMethod does.
        /// The short default timeout makes a stuck local server fail the test quickly instead of stalling the run.
        /// </summary>
        static Func<HttpWebRequest> UpsertRequest(string url, SF_Authenticator authenticator, string body, Action onCreate = null, int timeoutMs = 10000)
        {
            return () =>
            {
                onCreate?.Invoke();
                HttpWebRequest request = WebRequest.CreateHttp(url + "upsert?purgeType=full");
                request.Proxy = null;
                request.Timeout = timeoutMs;
                request.ContentType = "application/json";
                request.Method = "POST";
                request.Headers.Set("Authorization", authenticator.GetAuthorizationHeader());
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(body);
                }
                return request;
            };
        }

        [Fact]
        public void Upsert_401_IsResentOnceWithNewTokenAndSameBody()
        {
            FakeTokenHandler tokens;
            var warnings = new List<string>();
            var authenticator = OAuthAuthenticator(out tokens, warnings);
            string body = $"{{\"__metadata\":{{\"uri\":\"User('{EmployeeId}')\"}},\"username\":\"JDOE\"}}";
            using (var server = new FakeUpsertServer(HttpStatusCode.Unauthorized, HttpStatusCode.OK))
            using (var response = authenticator.Send(UpsertRequest(server.Url, authenticator, body)))
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(new[] { "Bearer token-1", "Bearer token-2" }, server.Authorizations);
                Assert.Equal(new[] { body, body }, server.Bodies);
                AssertSingleSafeRetryWarning(warnings, "POST", server.Url + "upsert");
            }
        }

        [Fact]
        public void Upsert_401_OnRetryToo_Throws_AndDropsTheRetryToken()
        {
            FakeTokenHandler tokens;
            var warnings = new List<string>();
            var authenticator = OAuthAuthenticator(out tokens, warnings);
            using (var server = new FakeUpsertServer(HttpStatusCode.Unauthorized, HttpStatusCode.Unauthorized, HttpStatusCode.OK))
            {
                var ex = Assert.Throws<WebException>(() => authenticator.Send(UpsertRequest(server.Url, authenticator, "{}")));
                Assert.Equal(HttpStatusCode.Unauthorized, ((HttpWebResponse)ex.Response).StatusCode);
                Assert.Equal(2, server.Authorizations.Count);
                Assert.Single(warnings);
                Assert.Equal("Bearer token-3", authenticator.GetAuthorizationHeader());
            }
        }

        [Fact]
        public void Upsert_Basic_401_IsNotResent_AndNotLogged()
        {
            var warnings = new List<string>();
            var authenticator = BasicAuthenticator(warnings);
            using (var server = new FakeUpsertServer(HttpStatusCode.Unauthorized, HttpStatusCode.OK))
            {
                Assert.Throws<WebException>(() => authenticator.Send(UpsertRequest(server.Url, authenticator, "{}")));
                Assert.Single(server.Authorizations);
                Assert.Empty(warnings);
            }
        }

        [Theory]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public void Upsert_OtherErrors_AreNotResent(HttpStatusCode status)
        {
            FakeTokenHandler tokens;
            var warnings = new List<string>();
            var authenticator = OAuthAuthenticator(out tokens, warnings);
            using (var server = new FakeUpsertServer(status, HttpStatusCode.OK))
            {
                var ex = Assert.Throws<WebException>(() => authenticator.Send(UpsertRequest(server.Url, authenticator, "{}")));
                Assert.Equal(status, ((HttpWebResponse)ex.Response).StatusCode);
                Assert.Single(server.Authorizations);
                Assert.Equal(1, tokens.RequestCount);
                Assert.Empty(warnings);
            }
        }

        [Fact]
        public void Upsert_Timeout_IsNotResent()
        {
            FakeTokenHandler tokens;
            var warnings = new List<string>();
            var authenticator = OAuthAuthenticator(out tokens, warnings);
            int created = 0;
            // Accepts the connection (OS backlog) but never answers, so the request times out with no response.
            var silent = new TcpListener(IPAddress.Loopback, 0);
            silent.Start();
            try
            {
                string url = $"http://127.0.0.1:{((IPEndPoint)silent.LocalEndpoint).Port}/";
                var ex = Assert.Throws<WebException>(() => authenticator.Send(UpsertRequest(url, authenticator, "{}", () => created++, timeoutMs: 1000)));

                Assert.Equal(WebExceptionStatus.Timeout, ex.Status);
                Assert.Equal(1, created);
                Assert.Equal(1, tokens.RequestCount);
                Assert.Empty(warnings);
            }
            finally
            {
                silent.Stop();
            }
        }

        [Fact]
        public void Upsert_NetworkFailure_IsNotResent()
        {
            FakeTokenHandler tokens;
            var warnings = new List<string>();
            var authenticator = OAuthAuthenticator(out tokens, warnings);
            int created = 0;
            string url = $"http://127.0.0.1:{FakeUpsertServer.FreePort()}/";   // nothing listening

            var ex = Assert.Throws<WebException>(() => authenticator.Send(UpsertRequest(url, authenticator, "{}", () => created++)));

            Assert.Equal(WebExceptionStatus.ConnectFailure, ex.Status);
            Assert.Equal(1, created);
            Assert.Equal(1, tokens.RequestCount);
            Assert.Empty(warnings);
        }

        #endregion Upsert
    }

    class FakeRequestMessage : DataServiceClientRequestMessage
    {
        readonly Func<FakeRequestMessage, IODataResponseMessage> _respond;
        readonly Dictionary<string, string> _headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Takes Method and Url from the args, like HttpWebRequestMessage.</summary>
        public FakeRequestMessage(DataServiceClientRequestMessageArgs args, Func<FakeRequestMessage, IODataResponseMessage> respond) : base(args.Method)
        {
            _respond = respond;
            Method = args.Method;
            Url = args.RequestUri;
        }

        public override IEnumerable<KeyValuePair<string, string>> Headers { get { return _headers; } }
        public override Uri Url { get; set; }
        public override string Method { get; set; }
        public override ICredentials Credentials { get; set; }
        public override int Timeout { get; set; }
        public override bool SendChunked { get; set; }

        public override string GetHeader(string headerName)
        {
            string value;
            return _headers.TryGetValue(headerName, out value) ? value : null;
        }

        public override void SetHeader(string headerName, string headerValue) { _headers[headerName] = headerValue; }
        public override Stream GetStream() { return new MemoryStream(); }
        public override void Abort() { }
        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state) { throw new NotSupportedException(); }
        public override Stream EndGetRequestStream(IAsyncResult asyncResult) { throw new NotSupportedException(); }
        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state) { throw new NotSupportedException(); }
        public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult) { throw new NotSupportedException(); }
        public override IODataResponseMessage GetResponse() { return _respond(this); }
    }

    class FakeResponseMessage : IODataResponseMessage
    {
        public FakeResponseMessage(int statusCode) { StatusCode = statusCode; }

        public IEnumerable<KeyValuePair<string, string>> Headers { get { return new KeyValuePair<string, string>[0]; } }
        public int StatusCode { get; set; }
        public string GetHeader(string headerName) { return null; }
        public void SetHeader(string headerName, string headerValue) { }
        public Stream GetStream() { return new MemoryStream(); }
    }

    /// <summary>Local HTTP server that answers the n-th request with the n-th status and records what was sent.</summary>
    sealed class FakeUpsertServer : IDisposable
    {
        readonly HttpListener _listener = new HttpListener();
        readonly HttpStatusCode[] _statuses;
        readonly object _sync = new object();
        readonly List<string> _authorizations = new List<string>();
        readonly List<string> _bodies = new List<string>();

        public FakeUpsertServer(params HttpStatusCode[] statuses)
        {
            _statuses = statuses;
            Url = $"http://localhost:{FreePort()}/";
            _listener.Prefixes.Add(Url);
            _listener.Start();
            Task.Run(Serve);
        }

        public string Url { get; private set; }

        public List<string> Authorizations { get { lock (_sync) return new List<string>(_authorizations); } }

        public List<string> Bodies { get { lock (_sync) return new List<string>(_bodies); } }

        async Task Serve()
        {
            while (true)
            {
                HttpListenerContext context;
                try
                {
                    context = await _listener.GetContextAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    return;
                }

                int index;
                using (var reader = new StreamReader(context.Request.InputStream))
                {
                    string body = reader.ReadToEnd();
                    lock (_sync)
                    {
                        index = _authorizations.Count;
                        _authorizations.Add(context.Request.Headers["Authorization"]);
                        _bodies.Add(body);
                    }
                }

                context.Response.StatusCode = (int)_statuses[Math.Min(index, _statuses.Length - 1)];
                byte[] reply = Encoding.UTF8.GetBytes("{\"d\":[]}");
                context.Response.OutputStream.Write(reply, 0, reply.Length);
                context.Response.Close();
            }
        }

        public static int FreePort()
        {
            var probe = new TcpListener(IPAddress.Loopback, 0);
            probe.Start();
            int port = ((IPEndPoint)probe.LocalEndpoint).Port;
            probe.Stop();
            return port;
        }

        public void Dispose()
        {
            _listener.Close();
        }
    }
}
