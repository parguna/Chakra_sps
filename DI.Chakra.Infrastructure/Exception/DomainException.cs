using System;
using System.Runtime.Serialization;

namespace DI.Chakra.Infrastructure
{
    /// <summary>
    /// Exception definition for domain exceptions.
    /// </summary>
    public class DomainException : Exception
    {

        #region Public Constants
        /// <summary>
        /// General domain exception message.
        /// </summary>
        public const string TEXT_DOMAIN_EXCEPTION = @"Exception raised from domain layer.";
        #endregion

        #region Constructors
        public DomainException()
            : base()
        {
        }

        public DomainException(string _message)
            : base(_message)
        {
        }

        public DomainException(string _message, Exception _innerException)
            : base(_message, _innerException)
        {
        }

        public DomainException(SerializationInfo _info, StreamingContext _context)
            : base(_info, _context)
        {
        }

        public DomainException(string _format, params object[] _args)
            : base(string.Format(_format, _args))
        { }

        #endregion
    }
    public class SuccessFactorAPIReadException : Exception
    {
        public SuccessFactorAPIReadException()
            : base()
        {
        }

        public SuccessFactorAPIReadException(string _message)
            : base(_message)
        {
        }

        public SuccessFactorAPIReadException(string _message, Exception _innerException)
            : base(_message, _innerException)
        {
        }

        public SuccessFactorAPIReadException(SerializationInfo _info, StreamingContext _context)
            : base(_info, _context)
        {
        }

        public SuccessFactorAPIReadException(string _format, params object[] _args)
            : base(string.Format(_format, _args))
        { }
    }
}
