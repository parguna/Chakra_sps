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
   public class PresentationSAPEPGroupToUsersReport
   {
       public long GroupId { get; set; }
       public string GroupName { get; set; }
       public string GroupDescription { get; set; }

       public long UserId { get; set; }
       public string UserName { get; set; }
       public string DisplayName { get; set; }
       public string InternetAddress { get; set; }
       public string JobTitle { get; set; }
       public Nullable<DateTime> IACMPasswordChangeDate { get; set; }
       public string Department { get; set; }

       public static List<PresentationSAPEPGroupToUsersReport> GetByEndpointSystemId(long endpointSystemId)
       {
           using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
           {
               return ORMapList(new SAPEPGroupInfoRepository(dataAccessTransaction).GetByEndpointSystemId(endpointSystemId).ToList());
           }
       }

       public static PresentationSAPEPGroupToUsersReport ORMap(EntityModel.Presentation.PresentationSAPEPGroupToUsersReport presentationSAPEPGroupToUsersReport)
       {
           if (presentationSAPEPGroupToUsersReport == null)
               return null;

           return Mapper.Map<EntityModel.Presentation.PresentationSAPEPGroupToUsersReport, PresentationSAPEPGroupToUsersReport>(presentationSAPEPGroupToUsersReport);
       }

       public static List<PresentationSAPEPGroupToUsersReport> ORMapList(IList<EntityModel.Presentation.PresentationSAPEPGroupToUsersReport> presentationSAPEPGroupToUsersReports)
       {
           return presentationSAPEPGroupToUsersReports.Select(ORMap).ToList();
       }
    }
}
