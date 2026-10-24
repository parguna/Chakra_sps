using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web.Http;


namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("WorkflowConfiguration")]
    public class WorkflowConfigurationController : BaseController<DomainModel.WorkflowConfiguration, EntityModel.WorkflowConfiguration, Dto.WorkflowConfiguration>
    {

        [HttpGet]
        [Route("GetByIdIncludeEndpoint")]
        public WorkflowConfiguration GetByIdIncludeEndpoint(long id)
        {
            return DtoMap(DomainModel.WorkflowConfiguration.GetByIdIncludeEndpoint(id));
        }

        [HttpGet]
        [Route("GetWorkflowConfigurationDetailById")]
        public WorkflowConfiguration GetWorkflowConfigurationDetailById(long id)
        {
            return DtoMap(DomainModel.WorkflowConfiguration.GetWorkflowConfigurationDetailById(id));
        }

        [HttpGet]
        [Route("GetByWorkflowName")]
        public List<WorkflowConfiguration> GetByWorkflowName(string workflowName)
        {
            return DtoMapList(DomainModel.WorkflowConfiguration.GetByWorkflowName(workflowName));
        }



        [HttpGet]
        [Route("GetByRequestTypeId")]
        public List<WorkflowConfiguration> GetByRequestTypeId(RequestType requestTypeId)
        {
            return DtoMapList(DomainModel.WorkflowConfiguration.GetByRequestTypeId((byte)requestTypeId));
        }

        [HttpGet]
        [Route("GetByEndpointSystemId")]
        public WorkflowConfiguration GetByEndpointSystemId(long requestTypeId, long endpointSystemId, string userType, string accountType, string actionType, DateTime requestDate)
        {
            return DtoMap(DomainModel.WorkflowConfiguration.GetByEndpointSystemId(requestTypeId, endpointSystemId, userType, accountType, actionType, requestDate));
        }

        [HttpGet]
        [Route("GetBySearchString")]
        public  Dictionary<long, string> GetBySearchString(string searchString)
        {
            return DomainModel.WorkflowConfiguration.GetBySearchString(searchString);
        }
        [HttpPost]
        [Route("DoesExists")]
        public bool DoesExists([FromBody]WorkflowConfiguration workflow)
        {
            return DomainModel.WorkflowConfiguration.DoesExists(DtoUnMap( workflow));
        }
    }
}
