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
    public class PresentationSAPTCodeReport
    {
        public long TCODE_Id { get; set; }

        public string TCODE_Code { get; set; }

        public string TCODE_Desc { get; set; }

        public string Institution { get; set; }

        public string HostName { get; set; }

        public static PresentationSAPTCodeReport ORMap(EntityModel.Presentation.PresentationSAPTCodeReport presentationSAPUserRoleTCodeAuth)
        {
            if (presentationSAPUserRoleTCodeAuth == null)
                return null;
            return Mapper.Map<EntityModel.Presentation.PresentationSAPTCodeReport, PresentationSAPTCodeReport>(presentationSAPUserRoleTCodeAuth);
        }

        public static List<PresentationSAPTCodeReport> ORMapList(IList<EntityModel.Presentation.PresentationSAPTCodeReport> presentationSAPUserRoleTCodeAuths)
        {
            return presentationSAPUserRoleTCodeAuths.Select(ORMap).ToList();
        }

        public static IList<PresentationSAPTCodeReport> GetAllSapTCodesReportData(long endPointSystemId)
        {
            using (DataAccessTransaction dataTransaction=new DataAccessTransaction (false))
            {
                return ORMapList(new SAPTCodeInfoRepository(dataTransaction).GetAllSapTCodesReportData(endPointSystemId));
            }
        }
        public static IList<PresentationSAPTCodeReport> GetTcodeInfo(long endPointSystemId, string roleCode)
        {
            using (DataAccessTransaction dataTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPTCodeInfoRepository(dataTransaction).GetTcodeInfo(endPointSystemId, roleCode));
            }
        }
    }

}
