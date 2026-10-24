
using System;
using System.Runtime.Serialization;

namespace DI.Chakra.Infrastructure
{
    /// <summary>
    /// The exception represents that the entity was not found.
    /// </summary>
    public class EntityNotFoundException : DomainException
    {
        #region Constructors
        public EntityNotFoundException()
            : base()
        { }

        public EntityNotFoundException(string _message)
            : base(_message)
        { }

        public EntityNotFoundException(string _message, Exception _innerException)
            : base(_message, _innerException)
        { }

        public EntityNotFoundException(SerializationInfo _info, StreamingContext _context)
            : base(_info, _context)
        { }

        public EntityNotFoundException(string _format, params object[] _args)
            : base(_format, _args)
        { }

        #endregion
    }
}
