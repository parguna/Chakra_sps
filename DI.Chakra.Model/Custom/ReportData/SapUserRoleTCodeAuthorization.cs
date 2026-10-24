using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Serializable]
    public class SapUserRoleTCodeAuthorization: IAggregateRoot
    {
        public string UserName { get; set; }
        public string RoleCode { get; set; }
        public string TCode { get; set; }
        public string AuthorizationCode { get; set; }
        public string ObjectCode { get; set; }
        public string FieldCode { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }
        public long EndpointSystemId { get; set; }
    }
}
