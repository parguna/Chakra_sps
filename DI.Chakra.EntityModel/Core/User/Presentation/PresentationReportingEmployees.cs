using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationReportingEmployees
    {
        public long UserID { get; set; }
        public string SearchString { get; set; }
        public List<long> CompanyId { get; set; }
        public List<long> DepartmentId { get; set; }
        public List<long> DivisionId { get; set; }
        public List<long> OrganisationId { get; set; }
        public List<string> SectionCode { get; set; }
    }
}
