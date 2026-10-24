using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure 
{
   public enum NovaMatrixType : byte
    {
        [Description("All")]
        All =0,
        [Description("User")]
        User =1,
        [Description("Management")]
        Management =2,
        [Description("Exceptional")]
        Exceptional = 3
    }

    public class NovaException : Exception
    {
        public NovaException() : base() { }
        
        public NovaException(string message) : base(message) { }

        public NovaException(string message, Exception innerException) : base(message, innerException) { }
    }
}
