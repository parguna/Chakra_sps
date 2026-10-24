using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DI.Chakra.Infrastructure
{
    public class EndpointNotConnectedException: ApplicationException
    {
        #region Public Constants
        /// <summary>
        /// General domain exception message.
        /// </summary>
        public const string TEXT_EXCEPTION = @"Exception raised while Connecting to AD";
        #endregion

        #region Constructors
        public EndpointNotConnectedException()
            : base()
        {
        }

        public EndpointNotConnectedException(string _message)
            : base(_message)
        {
        }

        public EndpointNotConnectedException(string _message, System.Exception _innerException)
            : base(_message, _innerException)
        {
        }

        public EndpointNotConnectedException(SerializationInfo _info, StreamingContext _context)
            : base(_info, _context)
        {
        }

        public EndpointNotConnectedException(string _format, params object[] _args)
            : base(string.Format(_format, _args))
        { }

        #endregion
    }
}

