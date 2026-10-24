using AutoMapper;
using DI.Chakra.DomainModel;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Services.EmailingProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationWorkflowCriteriaBase")]
    public class PresentationWorkflowCriteriaBaseController:ApiController
    {
        [HttpGet]
        [Route("GetWorkFlowCriteria")]
        public PresentationWorkflowCriteriaBase GetWorkFlowCriteria(AccessType accessType,string UserName, long TaskId,string Comments, string AccountCreatedNotificationTO, string conflictRemarks)
        {
            //Task task = Task.GetProvisioningTask(TaskId);

            var returnDetails = DtoMap(DomainModel.Presentation.PresentationWorkflowCriteriaBase.GetWorkFlowCriteria(accessType,UserName,TaskId,Comments,User.Identity.Name, conflictRemarks));
            //if(accessType == AccessType.Revoke) --Commented because system triggers mail before the task gets saved , So email with wrong value is sent in mail
            //new EmailComposer().Compose(EmailTemplate.AccessRequestRejected, (long)task.AccessRequestId); 
            return returnDetails;
        }
        [HttpPost]
        [Route("GetWorkFlowCriteria")]
        public PresentationWorkflowCriteriaBase GetWorkFlowCriteria(PresentationWorkFlowCriteria criteria)
        {
            //Task task = Task.GetProvisioningTask(TaskId);

            var returnDetails = DtoMap(DomainModel.Presentation.PresentationWorkflowCriteriaBase.GetWorkFlowCriteria(criteria.AccessType, criteria.UserName, criteria.TaskId, criteria.Comments, User.Identity.Name, criteria.ConflictRemarks));
            //if(accessType == AccessType.Revoke) --Commented because system triggers mail before the task gets saved , So email with wrong value is sent in mail
            //new EmailComposer().Compose(EmailTemplate.AccessRequestRejected, (long)task.AccessRequestId); 
            return returnDetails;
        }
        public PresentationWorkflowCriteriaBase DtoMap(DomainModel.Presentation.PresentationWorkflowCriteriaBase tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationWorkflowCriteriaBase, PresentationWorkflowCriteriaBase>(tModel);
        }

        public  List<PresentationWorkflowCriteriaBase> DtoMapList(IList<DomainModel.Presentation.PresentationWorkflowCriteriaBase> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
