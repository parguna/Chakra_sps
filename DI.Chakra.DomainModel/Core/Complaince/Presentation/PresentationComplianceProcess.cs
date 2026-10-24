using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationComplianceProcess : ComplianceProcess
    {
        public long ComplianceProcessId { get; set; }
        public Occurence occurence { get; set; }
        public string Status { get; set; }
        public string StartDateTime { get; set; }
        public string PolicyName { get; set; }

         public string SchedulerDateTime { get; set; }


        public static List<PresentationComplianceProcess> GetComplianceProcessJobs(int startRow, int PageSize, string filter)
        {
            List<PresentationComplianceProcess> lstComp = new List<PresentationComplianceProcess>();            
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new ComplianceProcessRepository(dataAccessTransaction).GetComplianceProcessJobs(startRow,PageSize,filter));
            }
        }
        public static PresentationComplianceProcess ORMap(EntityModel.Presentation.PresentationComplianceProcess presentationPagePermissionEntityModel)
        {
            if (presentationPagePermissionEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationComplianceProcess, PresentationComplianceProcess>(presentationPagePermissionEntityModel);
        }

        public static List<PresentationComplianceProcess> ORMapList(IList<EntityModel.Presentation.PresentationComplianceProcess> presentationPagePermissionEntityModel)
        {
            return presentationPagePermissionEntityModel.Select(ORMap).ToList();
        }
    }
}
