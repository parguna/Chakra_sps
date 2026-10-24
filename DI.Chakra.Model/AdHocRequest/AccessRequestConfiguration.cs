using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Table("DI_M_AccessRequestConfig")]
    public class AccessRequestConfiguration
    {
        [Key]
        public long Id { get; set; }
        public long EndpointSystemId { get; set; }
        public bool EnableNewAccount { get; set; }
        public bool EnableAmendAdd { get; set; }
        public bool EnableAmendRemove { get; set; }
        public bool EnableDeleteAccount { get; set; }
        public bool EnableUserAccountTypeRequest { get; set; }
        public bool EnableSecondaryAccountTypeRequest { get; set; }
        public bool EnableSharedAccountTypeRequest { get; set; }
        public bool EnableServiceAccountTypeRequest { get; set; }
        public EndPoint EndPointSystem { get; set; }
    }
}
