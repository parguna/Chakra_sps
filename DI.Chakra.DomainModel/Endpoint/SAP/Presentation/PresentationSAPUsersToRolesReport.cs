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
     public class PresentationSAPUsersToRolesReport
    {
        public long UserId { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string Department { get; set; }

        public bool? IsActive { get; set; }

        public string RoleCode { get; set; }

        public string RoleDescription { get; set; }


        public static PresentationSAPUsersToRolesReport ORMap(EntityModel.Presentation.PresentationSAPUsersToRolesReport presentationSAPUsersToRolesReport)
        {
            if (presentationSAPUsersToRolesReport == null)
                return null;
            return Mapper.Map<EntityModel.Presentation.PresentationSAPUsersToRolesReport, PresentationSAPUsersToRolesReport>(presentationSAPUsersToRolesReport);
        }

        public static List<PresentationSAPUsersToRolesReport> ORMapList(IList<EntityModel.Presentation.PresentationSAPUsersToRolesReport> presentationSAPUsersToRolesReports)
        {
            return presentationSAPUsersToRolesReports.Select(ORMap).ToList();
        }

        public static IList<PresentationSAPUsersToRolesReport> GetSAPUserToRolesReportData(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList( new SAPRoleToAuthMapRepository(dataAccessTransaction).GetSAPUserToRolesReportData(endPointSystemId));
            }

        }


        public static IList<PresentationSAPUsersToRolesReport> GetSAPUserToRolesReportDataIncludeUserId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPRoleToAuthMapRepository(dataAccessTransaction).GetSAPUserToRolesReportDataIncludeUserId(endPointSystemId));
            }

        }
        public static IList<PresentationSAPUsersToRolesReport> GetSAPUserToRolesReportData(int userId, long endpointSystemId)
        {
            
             using(DataAccessTransaction dataAccessTransaction=new DataAccessTransaction ())
             {
                 return ORMapList(new SAPRoleToAuthMapRepository(dataAccessTransaction).GetSAPUserToRolesReportData(userId, endpointSystemId));

             }
                
           
        }

        public static IList<PresentationSAPUsersToRolesReport> GetSAPUserToTCodeReport(int userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList( new SAPRoleToAuthMapRepository(dataAccessTransaction).GetSAPUserToRolesReportData(userId));
            }

        } 
    }
}
