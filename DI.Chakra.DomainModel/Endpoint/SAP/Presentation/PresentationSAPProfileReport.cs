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

    public class PresentationSAPProfileReport
    {
        public int SAP_PROFILEID { get; set; }

        public string PROFILE_CODE { get; set; }

        public string PROFILE_DESC { get; set; }

        public string Institution { get; set; }

        public string HostName { get; set; }



        public static PresentationSAPProfileReport ORMap(EntityModel.Presentation.PresentationSAPProfileReport presentationSAPProfileReport)
        {
            if (presentationSAPProfileReport == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationSAPProfileReport, PresentationSAPProfileReport>(presentationSAPProfileReport);
        }

        public static List<PresentationSAPProfileReport> ORMapList(IList<EntityModel.Presentation.PresentationSAPProfileReport> presentationSAPProfileReports)
        {
            return presentationSAPProfileReports.Select(ORMap).ToList();
        }


        public static List<PresentationSAPProfileReport> GetSAPUserToProfilesReportData(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPProfileInfoRepository(dataAccessTransaction).GetSAPUserToProfilesReportData(endpointSystemId));
            }
        }
    }
}
