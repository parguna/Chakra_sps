using System;
using System.Runtime.Serialization;

namespace DI.Chakra.Infrastructure
{
    /// <summary>
    /// Exception definition for domain exceptions.
    /// </summary>
    public class ScheduleException : ApplicationException
    {

        #region Public Constants
        /// <summary>
        /// General domain exception message.
        /// </summary>
        public const string TEXT_DOMAIN_EXCEPTION = @"Exception raised in Scheduler.";
        #endregion

        #region Constructors
        public ScheduleException()
            : base()
        {
        }

        public ScheduleException(string _message)
            : base(_message)
        {
        }

        public ScheduleException(string _message, Exception _innerException)
            : base(_message, _innerException)
        {
        }

        public ScheduleException(SerializationInfo _info, StreamingContext _context)
            : base(_info, _context)
        {
        }

        public ScheduleException(string _format, params object[] _args)
            : base(string.Format(_format, _args))
        { }

        #endregion
    }
}
