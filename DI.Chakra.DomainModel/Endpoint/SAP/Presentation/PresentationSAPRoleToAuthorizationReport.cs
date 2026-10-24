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
     public class PresentationSAPRoleToAuthorizationReport
    {


        public int SAPRoleToAuthMapId { get; set; }
        public string RoleName { get; set; }
        public string ObjectCode { get; set; }
        public string AuthCode { get; set; }
        public string FieldCode { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }

        public static PresentationSAPRoleToAuthorizationReport ORMap(EntityModel.Presentation.PresentationSAPRoleToAuthorizationReport presentationSAPRoleToAuthorizationReport)
        {
            if (presentationSAPRoleToAuthorizationReport == null)
                return null;
            return Mapper.Map<EntityModel.Presentation.PresentationSAPRoleToAuthorizationReport, PresentationSAPRoleToAuthorizationReport>(presentationSAPRoleToAuthorizationReport);
        }

        public static List<PresentationSAPRoleToAuthorizationReport> ORMapList(IList<EntityModel.Presentation.PresentationSAPRoleToAuthorizationReport> presentationSAPRoleToAuthorizationReport)
        {
            return presentationSAPRoleToAuthorizationReport.Select(ORMap).ToList();
        }




        public static List<PresentationSAPRoleToAuthorizationReport> GetSapRolesToAuthorizationData(string roleCode, long endPointSystemId)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMapList(new SAPRoleToAuthMapRepository(dataAccessTransaction).GetSapRolesToAuthorizationData(roleCode, endPointSystemId));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
