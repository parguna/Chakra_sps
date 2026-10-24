using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("WorkflowConfigurationDetail")]
    public class WorkflowConfigurationDetailController : BaseController<DomainModel.WorkflowConfigurationDetail, EntityModel.WorkflowConfigurationDetail, Dto.WorkflowConfigurationDetail>
    {
        [HttpGet]
        [Route("GetByWorkflowName")]
        public List<WorkflowConfigurationDetail> GetByWorkflowConfigurationId(long workflowConfigurationId)
        {
            return DtoMapList(DomainModel.WorkflowConfigurationDetail.GetByWorkflowConfigurationId(workflowConfigurationId));
        }
    }
}
