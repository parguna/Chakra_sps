using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace DI.Chakra.Services.EmailingProcessor
{
    public class EMailServerNotWorkingException  : ApplicationException
    {
        public EMailServerNotWorkingException()
            : base()
        {
        }

        public EMailServerNotWorkingException(string _message)
            : base(_message)
        {
        }

        public EMailServerNotWorkingException(string _message, Exception _innerException)
            : base(_message, _innerException)
        {
        }

        public EMailServerNotWorkingException(SerializationInfo _info, StreamingContext _context)
            : base(_info, _context)
        {
        }

        public EMailServerNotWorkingException(string _format, params object[] _args)
            : base(string.Format(_format, _args))
        { }
    
    }
}