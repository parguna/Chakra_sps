using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationSAPRoleReport
    {
        public long SAP_ROLEID { get; set; }

        public string ROLE_CODE { get; set; }

        public string ROLE_DESC { get; set; }

        public string ROLETYPE { get; set; }

        public string Institution { get; set; }

        public string HostName { get; set; }

        public DateTime? ValidToDate { get; set; }
    }
    [NotMapped]
    public class PresentationFlinsTechnicalRole
    {
        public string TechnicalRule { get; set; }
        public string Type { get; set; }
        public string BusinessRole { get; set; }
        public string Entitlement { get; set; }
        public string EntitlementDescription { get; set; }
        public DateTime? ValidToDate { get; set; }
        public DateTime? LastAttestedOn { get; set; }
    }
}
