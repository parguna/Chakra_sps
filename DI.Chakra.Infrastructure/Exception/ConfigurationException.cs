using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DI.Chakra.Infrastructure
{
    public class ConfigurationException : ApplicationException
    {
        #region Public Constants
        /// <summary>
        /// General domain exception message.
        /// </summary>
        public const string TEXT_EXCEPTION = @"Exception raised while getting configuration.";
        #endregion

        #region Constructors
        public ConfigurationException()
            : base()
        {
        }

        public ConfigurationException(string _message)
            : base(_message)
        {
        }

        public ConfigurationException(string _message, Exception _innerException)
            : base(_message, _innerException)
        {
        }

        public ConfigurationException(SerializationInfo _info, StreamingContext _context)
            : base(_info, _context)
        {
        }

        public ConfigurationException(string _format, params object[] _args)
            : base(string.Format(_format, _args))
        { }

        #endregion
    }
}
