using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class OrganisationReportData
    {
        public long organisationId { get; set; }
        public string OrganisationName { get; set; }
        public string ParentOrganisation { get; set; }
        public string ParentOrganisationId { get; set; }
        public string Scope { get; set; }
        public string HeadofDepartmentUserName { get; set; }
        public string HeadofDepartmentName { get; set; }
        public string HeadofDepartmentEmployeeID { get; set; }
        public string RepresentativeUserName { get; set; }
        public string RepresentativeName { get; set; }
        public string OrgUnitPrefix { get; set; }
    }
}

