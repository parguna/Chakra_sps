using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [NotMapped]
    public class PresentationSAPHANARoleTCode
    {
        public string EndpointName { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public string TCODE_Code { get; set; }
        public string TCODE_Desc { get; set; }
        public string Remarks { get; set; }
    }
}
