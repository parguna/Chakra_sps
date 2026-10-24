using AutoMapper;
//using DI.Chakra.DomainModel;
using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{

    [RoutePrefix("PresentationWorkflowApproverGridData")]
    public class PresentationWorkflowApproverGridDataController : WorkflowConfigurationController
    {
        [HttpPost]
        [Route("GetApproversList")]
        public List<PresentationWorkflowApproverGridData> GetApproversList([FromBody]List<Dto.WorkflowConfigurationDetail> workflowConfigurationDetailList)
        {
            return DtoMapList(DomainModel.Presentation.PresentationWorkflowApproverGridData.GetApproversList(AutoMapperHelper.DtoUnMapList<Dto.WorkflowConfigurationDetail, DomainModel.WorkflowConfigurationDetail>(workflowConfigurationDetailList)));

        }


        public PresentationWorkflowApproverGridData DtoMap(DomainModel.Presentation.PresentationWorkflowApproverGridData tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationWorkflowApproverGridData, PresentationWorkflowApproverGridData>(tModel);
        }

        public List<PresentationWorkflowApproverGridData> DtoMapList(IList<DomainModel.Presentation.PresentationWorkflowApproverGridData> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
