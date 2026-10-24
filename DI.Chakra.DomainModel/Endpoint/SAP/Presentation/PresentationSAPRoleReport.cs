using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationSAPRoleReport
    {
        public long SAP_ROLEID { get; set; }

        public string ROLE_CODE { get; set; }

        public string ROLE_DESC { get; set; }

        public string ROLETYPE { get; set; }

        public string Institution { get; set; }

        public string HostName { get; set; }

        public DateTime? ValidToDate { get; set; }
        public static PresentationSAPRoleReport ORMap(EntityModel.Presentation.PresentationSAPRoleReport presentationSAPRoleReport)
        {
            if (presentationSAPRoleReport == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationSAPRoleReport, PresentationSAPRoleReport>(presentationSAPRoleReport);
        }

        public static List<PresentationSAPRoleReport> ORMapList(IList<EntityModel.Presentation.PresentationSAPRoleReport> presentationSAPRoleReports)
        {
            return presentationSAPRoleReports.Select(ORMap).ToList();
        }

        public static List<PresentationSAPRoleReport> GetAllSAPRolesReportData(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPRoleInfoRepository(dataAccessTransaction).GetAllSAPRolesReportData(endpointSystemId));
            }
        }

    }
}
