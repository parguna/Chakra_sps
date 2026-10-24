using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class IDMSHttpResponse<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
