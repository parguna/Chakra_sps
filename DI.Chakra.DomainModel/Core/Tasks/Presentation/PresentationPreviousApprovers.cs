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
    public class PresentationPreviousApprovers : Task
    {
        public string UserName { get; set; }

        public DateTime ApprovedDate { get; set; }

        public static List<PresentationPreviousApprovers> GetByWorkflowInstanceId(Guid workflowInstanceId, int workflowStateId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new TaskRepository(dataAccessTransaction).GetByWorkflowInstanceId(workflowInstanceId, workflowStateId));
            }
        }

        public static PresentationPreviousApprovers ORMap(EntityModel.Presentation.PresentationPreviousApprovers presentationPreviousApproversEntityModel)
        {
            if (presentationPreviousApproversEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationPreviousApprovers, PresentationPreviousApprovers>(presentationPreviousApproversEntityModel);
        }

        public static List<PresentationPreviousApprovers> ORMapList(IList<EntityModel.Presentation.PresentationPreviousApprovers> presentationPreviousApproversEntityModel)
        {
            return presentationPreviousApproversEntityModel.Select(ORMap).ToList();
        }
    }
}
