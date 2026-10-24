using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationSAPHANARoleWithNoUsers
    {
        public int SAP_ROLEID { get; set; }

        public string ROLE_CODE { get; set; }

        public string ROLE_DESC { get; set; }

        public string ROLETYPE { get; set; }

        public string Institution { get; set; }

        public string HostName { get; set; }
    }
}
