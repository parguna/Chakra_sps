using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationSAPHANARoleToAuthorizationReport
    {
        public int SAPRoleToAuthMapId { get; set; }
        public string RoleName { get; set; }
        public string ObjectCode { get; set; }
        public string AuthCode { get; set; }
        public string FieldCode { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }

    }
}
