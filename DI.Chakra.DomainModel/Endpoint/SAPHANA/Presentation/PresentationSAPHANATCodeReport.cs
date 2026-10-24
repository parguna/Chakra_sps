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
    public class PresentationSAPHANATCodeReport
    {
        public long TCODE_Id { get; set; }

        public string TCODE_Code { get; set; }

        public string TCODE_Desc { get; set; }

        public string Institution { get; set; }

        public string HostName { get; set; }

        public static PresentationSAPHANATCodeReport ORMap(EntityModel.Presentation.PresentationSAPHANATCodeReport presentationSAPUserRoleTCodeAuth)
        {
            if (presentationSAPUserRoleTCodeAuth == null)
                return null;
            return Mapper.Map<EntityModel.Presentation.PresentationSAPHANATCodeReport, PresentationSAPHANATCodeReport>(presentationSAPUserRoleTCodeAuth);
        }

        public static List<PresentationSAPHANATCodeReport> ORMapList(IList<EntityModel.Presentation.PresentationSAPHANATCodeReport> presentationSAPUserRoleTCodeAuths)
        {
            return presentationSAPUserRoleTCodeAuths.Select(ORMap).ToList();
        }

        public static IList<PresentationSAPHANATCodeReport> GetAllSapTCodesReportData(long endPointSystemId)
        {
            using (DataAccessTransaction dataTransaction=new DataAccessTransaction (false))
            {
                return ORMapList(new SAPHANATCodeInfoRepository(dataTransaction).GetAllSapTCodesReportData(endPointSystemId));
            }
        }
        public static IList<PresentationSAPHANATCodeReport> GetTcodeInfo(long endPointSystemId, string roleCode)
        {
            using (DataAccessTransaction dataTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPHANATCodeInfoRepository(dataTransaction).GetTcodeInfo(endPointSystemId, roleCode));
            }
        }
    }

}
