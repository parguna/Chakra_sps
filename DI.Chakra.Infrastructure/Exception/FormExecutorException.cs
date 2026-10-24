using System;
using System.Runtime.Serialization;

namespace DI.Chakra.Infrastructure
{

    public class FormExecutorException : Exception
    {

        #region Public Constants
        /// <summary>
        /// General domain exception message.
        /// </summary>
        public const string TEXT_DOMAIN_EXCEPTION = @"Exception raised from domain layer.";
        #endregion

        #region Constructors
        public FormExecutorException()
            : base()
        {
        }

        public FormExecutorException(string _message)
            : base(_message)
        {
        }

        public FormExecutorException(string _message, Exception _innerException)
            : base(_message, _innerException)
        {
        }

        public FormExecutorException(SerializationInfo _info, StreamingContext _context)
            : base(_info, _context)
        {
        }

        public FormExecutorException(string _format, params object[] _args)
            : base(string.Format(_format, _args))
        { }

        #endregion
    }

    public class FormExecutorValidationFailedException : FormExecutorException
    {
        #region Public Constants
        /// <summary>
        /// General domain exception message.
        /// </summary>
        public const string TEXT_DOMAIN_EXCEPTION = @"Exception raised from domain layer.";
        #endregion

        #region Constructors
        public FormExecutorValidationFailedException()
            : base()
        {
        }

        public FormExecutorValidationFailedException(string _message)
            : base(_message)
        {
        }

        public FormExecutorValidationFailedException(string _message, Exception _innerException)
            : base(_message, _innerException)
        {
        }

        public FormExecutorValidationFailedException(SerializationInfo _info, StreamingContext _context)
            : base(_info, _context)
        {
        }

        public FormExecutorValidationFailedException(string _format, params object[] _args)
            : base(string.Format(_format, _args))
        { }

        #endregion
    }


    public class FormExecutorInActiveUserException : FormExecutorException
    {
        #region Public Constants
        /// <summary>
        /// General domain exception message.
        /// </summary>
        public const string TEXT_DOMAIN_EXCEPTION = @"Exception raised from domain layer.";
        #endregion

        #region Constructors
        public FormExecutorInActiveUserException()
            : base()
        {
        }

        public FormExecutorInActiveUserException(string _message)
            : base(_message)
        {
        }

        public FormExecutorInActiveUserException(string _message, Exception _innerException)
            : base(_message, _innerException)
        {
        }

        public FormExecutorInActiveUserException(SerializationInfo _info, StreamingContext _context)
            : base(_info, _context)
        {
        }

        public FormExecutorInActiveUserException(string _format, params object[] _args)
            : base(string.Format(_format, _args))
        { }

        #endregion
    }
    public class FormExecutorAlreadyDeletedAccountException : FormExecutorException
    {
        #region Public Constants
        /// <summary>
        /// General domain exception message.
        /// </summary>
        public const string TEXT_DOMAIN_EXCEPTION = @"Exception raised from domain layer.";
        #endregion

        #region Constructors
        public FormExecutorAlreadyDeletedAccountException()
            : base()
        {
        }

        public FormExecutorAlreadyDeletedAccountException(string _message)
            : base(_message)
        {
        }

        public FormExecutorAlreadyDeletedAccountException(string _message, Exception _innerException)
            : base(_message, _innerException)
        {
        }

        public FormExecutorAlreadyDeletedAccountException(SerializationInfo _info, StreamingContext _context)
            : base(_info, _context)
        {
        }

        public FormExecutorAlreadyDeletedAccountException(string _format, params object[] _args)
            : base(string.Format(_format, _args))
        { }

        #endregion
    }
}
