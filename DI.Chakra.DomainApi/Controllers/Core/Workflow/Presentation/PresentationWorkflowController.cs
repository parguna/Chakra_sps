using AutoMapper;

using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{

    [RoutePrefix("PresentationWorkflow")]
    public class PresentationWorkflowController : WorkflowConfigurationController
    {
        [HttpGet]
        [Route("GetWorkflowList")]
        public List<PresentationWorkflow> GetWorkflowList()
        {
            return DtoMapList(DomainModel.Presentation.PresentationWorkflow.GetWorkflowList());
        }


        public  PresentationWorkflow DtoMap(DomainModel.Presentation.PresentationWorkflow tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationWorkflow, PresentationWorkflow>(tModel);
        }

        public  List<PresentationWorkflow> DtoMapList(IList<DomainModel.Presentation.PresentationWorkflow> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
