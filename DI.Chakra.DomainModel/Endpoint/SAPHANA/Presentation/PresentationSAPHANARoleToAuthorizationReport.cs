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
     public class PresentationSAPHANARoleToAuthorizationReport
    {


        public int SAPRoleToAuthMapId { get; set; }
        public string RoleName { get; set; }
        public string ObjectCode { get; set; }
        public string AuthCode { get; set; }
        public string FieldCode { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }

        public static PresentationSAPHANARoleToAuthorizationReport ORMap(EntityModel.Presentation.PresentationSAPHANARoleToAuthorizationReport presentationSAPRoleToAuthorizationReport)
        {
            if (presentationSAPRoleToAuthorizationReport == null)
                return null;
            return Mapper.Map<EntityModel.Presentation.PresentationSAPHANARoleToAuthorizationReport, PresentationSAPHANARoleToAuthorizationReport>(presentationSAPRoleToAuthorizationReport);
        }

        public static List<PresentationSAPHANARoleToAuthorizationReport> ORMapList(IList<EntityModel.Presentation.PresentationSAPHANARoleToAuthorizationReport> presentationSAPRoleToAuthorizationReport)
        {
            return presentationSAPRoleToAuthorizationReport.Select(ORMap).ToList();
        }




        public static List<PresentationSAPHANARoleToAuthorizationReport> GetSapRolesToAuthorizationData(string roleCode, long endPointSystemId)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMapList(new SAPHANARoleToAuthMapRepository(dataAccessTransaction).GetSapRolesToAuthorizationData(roleCode, endPointSystemId));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
