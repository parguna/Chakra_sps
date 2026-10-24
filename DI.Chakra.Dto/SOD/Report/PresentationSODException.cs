using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class PresentationSODException
    {
        public string ExceptionCode { get; set; }
        public string ExceptionDescription { get; set; }
        public string ExceptionOwner { get; set; }
        public string User { get; set; }
        public string Function1 { get; set; }
        public string FunctionValue1 { get; set; }
        public string Function2 { get; set; }
        public string FunctionValue2 { get; set; }
    }
}
