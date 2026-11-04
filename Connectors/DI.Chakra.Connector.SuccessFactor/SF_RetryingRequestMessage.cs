using Microsoft.Data.OData;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.Net;

namespace DI.Chakra.Connector
{
    /// <summary>
    /// OData request message used by the SuccessFactors EntityContainer. With OAuth, when SuccessFactors answers 401
    /// (token revoked or expired early) the cached token is dropped and the request is sent once more with a new token.
    /// Only requests without a body are resent; the connector only reads through OData, writes go through upsert.
    /// </summary>
    public class SF_RetryingRequestMessage : DataServiceClientRequestMessage
    {
        readonly DataServiceClientRequestMessageArgs _args;
        readonly SF_Authenticator _authenticator;
        readonly Func<DataServiceClientRequestMessageArgs, DataServiceClientRequestMessage> _createMessage;
        readonly List<KeyValuePair<string, string>> _headersSet = new List<KeyValuePair<string, string>>();
        DataServiceClientRequestMessage _inner;
        bool _hasBody;
        bool _retried;

        public SF_RetryingRequestMessage(DataServiceClientRequestMessageArgs args,
                                         SF_Authenticator authenticator,
                                         Func<DataServiceClientRequestMessageArgs, DataServiceClientRequestMessage> createMessage = null)
            : base(args.ActualMethod)
        {
            _args = args;
            _authenticator = authenticator;
            _createMessage = createMessage ?? (a => new HttpWebRequestMessage(a));
            _inner = _createMessage(args);
        }

        public override IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { return _inner.Headers; }
        }

        public override Uri Url
        {
            get { return _inner.Url; }
            set { _inner.Url = value; }
        }

        public override string Method
        {
            get { return _inner.Method; }
            set { _inner.Method = value; }
        }

        public override ICredentials Credentials
        {
            get { return _inner.Credentials; }
            set { _inner.Credentials = value; }
        }

        public override int Timeout
        {
            get { return _inner.Timeout; }
            set { _inner.Timeout = value; }
        }

        public override bool SendChunked
        {
            get { return _inner.SendChunked; }
            set { _inner.SendChunked = value; }
        }

        public override string GetHeader(string headerName)
        {
            return _inner.GetHeader(headerName);
        }

        /// <summary>Headers set after the message is created (e.g. Authorization in SendingRequest2) are replayed on a retry.</summary>
        public override void SetHeader(string headerName, string headerValue)
        {
            _inner.SetHeader(headerName, headerValue);
            _headersSet.Add(new KeyValuePair<string, string>(headerName, headerValue));
        }

        public override Stream GetStream()
        {
            _hasBody = true;
            return _inner.GetStream();
        }

        public override void Abort()
        {
            _inner.Abort();
        }

        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            _hasBody = true;
            return _inner.BeginGetRequestStream(callback, state);
        }

        public override Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            return _inner.EndGetRequestStream(asyncResult);
        }

        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            return _inner.BeginGetResponse(callback, state);
        }

        public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult)
        {
            return _inner.EndGetResponse(asyncResult);
        }

        /// <summary>Sends the request; on a 401 that may be retried, sends it once more (the second call cannot retry again).</summary>
        public override IODataResponseMessage GetResponse()
        {
            try
            {
                IODataResponseMessage response = _inner.GetResponse();
                if (!IsUnauthorized(response) || !PrepareRetry(response))
                    return response;
            }
            catch (DataServiceTransportException ex) when (IsUnauthorized(ex.Response) && PrepareRetry(ex.Response))
            {
            }

            return GetResponse();
        }

        /// <summary>
        /// Called for every 401: drops the rejected token, then, when a retry is allowed, swaps in a new message
        /// with the same headers and a new token. Returns false when the 401 must be passed on to the caller.
        /// </summary>
        bool PrepareRetry(IODataResponseMessage unauthorizedResponse)
        {
            _authenticator.OnUnauthorized(_inner.GetHeader("Authorization"));
            if (_retried || _hasBody || !_authenticator.CanRetryUnauthorized)
                return false;

            (unauthorizedResponse as IDisposable)?.Dispose();
            _retried = true;
            _authenticator.LogRetry(_inner.Method, _inner.Url);

            DataServiceClientRequestMessage retry = _createMessage(_args);
            retry.Timeout = _inner.Timeout;
            retry.SendChunked = _inner.SendChunked;
            if (_inner.Credentials != null)
                retry.Credentials = _inner.Credentials;
            foreach (var header in _headersSet)
                retry.SetHeader(header.Key, header.Value);
            retry.SetHeader("Authorization", _authenticator.GetAuthorizationHeader());

            _inner = retry;
            return true;
        }

        static bool IsUnauthorized(IODataResponseMessage response)
        {
            return response != null && response.StatusCode == (int)HttpStatusCode.Unauthorized;
        }
    }
}
