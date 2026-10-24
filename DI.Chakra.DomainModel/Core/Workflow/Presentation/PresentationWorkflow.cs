using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel;
using AutoMapper;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationWorkflow
    {
        public long WorkflowConfigurationId { get; set; }
        public string WorkflowName { get; set; }
        public string Description { get; set; }
        public string RequestType { get; set; }
        public string ApplicationType { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public static PresentationWorkflow ORMap(EntityModel.Presentation.PresentationWorkflow PresentationWorkflowEntityModel)
        {
            if (PresentationWorkflowEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationWorkflow, PresentationWorkflow>(PresentationWorkflowEntityModel);
        }

        public static List<PresentationWorkflow> ORMapList(IList<EntityModel.Presentation.PresentationWorkflow> PresentationWorkflowEntityModels)
        {
            return PresentationWorkflowEntityModels.Select(ORMap).ToList();
        }

        public static IList<PresentationWorkflow> GetWorkflowList()
        {
            IList<PresentationWorkflow> workflowList = new List<PresentationWorkflow>();
            PresentationWorkflow workflow;
            RequestType rqstType;

            IList<WorkflowConfiguration> _wfconfigList = DomainModel.WorkflowConfiguration.GetByIncludeEndpoint();
            if (_wfconfigList != null && _wfconfigList.Count > 0)
            {
                foreach (WorkflowConfiguration cnfg in _wfconfigList)
                {
                    workflow = new PresentationWorkflow();
                    workflow.WorkflowConfigurationId = cnfg.Id;
                    workflow.WorkflowName = cnfg.WorkflowName;
                    workflow.Description = cnfg.Description;
                    rqstType = (RequestType)cnfg.RequestTypeId;
                    workflow.RequestType = UtilityMethods.GetEnumAttribute<Description>(rqstType);
                    workflow.ApplicationType = cnfg.Endpoint == null ? string.Empty : cnfg.Endpoint.DisplayName;
                    workflow.IsActive = cnfg.IsActive;
                    workflow.CreatedBy = cnfg.CreatedBy;
                    workflow.CreatedDate = cnfg.CreatedDate;
                    workflow.LastModifiedBy = cnfg.LastModifiedBy;
                    workflow.LastModifiedDate = cnfg.LastModifiedDate;

                    workflowList.Add(workflow);
                }
            }
            return workflowList.OrderByDescending(leWorkflow => leWorkflow.CreatedDate).ToList() ;

        }
    }
}
