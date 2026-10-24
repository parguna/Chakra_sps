using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("Task")]
    public class TaskController : BaseController<DomainModel.Task, EntityModel.Task, Dto.Task>
    {
        [HttpGet]
        [Route("DoesGroupTaskExistsForUser")]
        public bool DoesGroupTaskExistsForUser(long userId)
        {
            return DomainModel.Task.DoesGroupTaskExistsForUser(userId);
        }

        [HttpGet]
        [Route("DoesAttestationTasksAlreadyExists")]
        public bool DoesAttestationTasksAlreadyExists(long attestationId)
        {
            return DomainModel.Task.DoesAttestationTasksAlreadyExists(attestationId);
        }

        [HttpGet]
        [Route("GetTaskInfo")]
        public List<Dto.Task> GetTaskInfo(string attestationName)
        {
            return DtoMapList(DomainModel.Task.GetTaskInfo(attestationName));
        }

        [HttpGet]
        [Route("GetTaskInfoByTriggerId")]
        public Dto.Task GetTaskInfoByTriggerId(long triggerId)
        {
            return DtoMap(DomainModel.Task.GetTaskInfoByTriggerId(triggerId));
        }

        [HttpGet]
        [Route("GetProvisioningTask")]
        public Dto.Task GetProvisioningTask(long id)
        {
            return DtoMap(DomainModel.Task.GetProvisioningTask(id));
        }

        [HttpGet]
        [Route("GetBulkRequestTask")]
        public Dto.Task GetBulkRequestTask(long id)
        {
            return DtoMap(DomainModel.Task.GetBulkRequestTask(id));
        }

        

        [HttpGet]
        [Route("GetTaskCompleteness")]
        public Infrastructure.TaskCompleteness  GetTaskCompleteness(string attestationName)
        {
            return DomainModel.Task.GetTaskCompleteness(attestationName);
        }

        [HttpGet]
        [Route("GetCampaignTaskDetail")]
        public Infrastructure.CampaignTaskDetail GetCampaignTaskDetail(string campaignName)
        {
            return DomainModel.Task.GetCampaignTaskDetail(campaignName);
        }


        [HttpGet]
        [Route("GetPendingTaskByAccessRequestId")]
        public List<Task> GetPendingTaskByAccessRequestId(long accessRequestId)
        {
            return DtoMapList(DomainModel.Task.GetPendingTaskByAccessRequestId(accessRequestId));
        }

        [HttpPost]
        [Route("Serialize")]
        public String Serialize([FromBody] Task task)
        {
            return DomainModel.Task.Serialize(AutoMapperHelper.DtoUnMap<Task,
                DomainModel.Task>(task));
        }

        [HttpPost]
        [Route("DeSerialize")]
        public Task DeSerialize([FromBody]Infrastructure.TransformXmlData xmlData)
        {
            return DtoMap(DomainModel.Task.DeSerialize(xmlData.XmlData));
        }

        [HttpGet]
        [Route("GekByAccessRequestId")]
        public List<Task> GekByAccessRequestId(long accessRequestId)
        {
            return DtoMapList(DomainModel.Task.GekByAccessRequestId(accessRequestId));
        }

        [HttpGet]
        [Route("DoesExistsByWorkflowConfigurationId")]
        public bool DoesExistsByWorkflowConfigurationId(long workflowid) 
        {
            return DomainModel.Task.DoesExistsByWorkflowConfigurationId(workflowid);
        }

        [HttpGet]
        [Route("GetTaskByBulkRequestId")]
        public Task GetTaskByBulkRequestId(long bulkRequestId)
        {
            return DtoMap(DomainModel.Task.GetTaskByBulkRequestId(bulkRequestId));
        }
        [HttpGet]
        [Route("GetTaskByEmployeeEventId")]
        public List<Task> GetTaskByEmployeeEventId(long employeeEventId)
        {
            return DtoMapList(DomainModel.Task.GetByEventTriggerIdForWithDraw(employeeEventId));
        }
        
    }
}