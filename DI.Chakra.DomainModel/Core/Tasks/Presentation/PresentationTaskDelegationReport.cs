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
    public class PresentationTaskDelegationReport : TaskDelegation
    {
        public long DelegationId { get; set; }

        public string ActionUserName { get; set; }

        public string DelegatedFromUserName { get; set; }

        public string DelegatedToUserName { get; set; }

        public bool Active { get; set; }

        public string DelegationCreatedBy { get; set; }

        public long AccessRequestId { get; set; }

        public DateTime DelegationDate { get; set; }

        public string DelegateAttestationTasks { get; set; }

        public string DelegateProvisioningTasks { get; set; }

        public static List<PresentationTaskDelegationReport> GetTaskDelegationsForReport()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new TaskDelegationRepository(dataAccessTransaction).GetTaskDelegationsForReport());
            }
        }

        public static PresentationTaskDelegationReport ORMap(EntityModel.Presentation.PresentationTaskDelegationReport presentationTaskDelegationReportEntityModel)
        {
            if (presentationTaskDelegationReportEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationTaskDelegationReport, PresentationTaskDelegationReport>(presentationTaskDelegationReportEntityModel);
        }

        public static List<PresentationTaskDelegationReport> ORMapList(IList<EntityModel.Presentation.PresentationTaskDelegationReport> presentationTaskDelegationReportEntityModel)
        {
            return presentationTaskDelegationReportEntityModel.Select(ORMap).ToList();
        }
    }
}
