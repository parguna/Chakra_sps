using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto.Presentation;
using AutoMapper;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationPreviousApprovers")]
    public class PresentationPreviousApproversController : TaskController
    {
        [HttpGet]
        [Route("GetByWorkflowInstanceId")]
        public List<DI.Chakra.Dto.Presentation.PresentationPreviousApprovers> GetByWorkflowInstanceId(Guid workflowInstanceId, int workflowStateId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationPreviousApprovers.GetByWorkflowInstanceId(workflowInstanceId, workflowStateId));
        }
        public  DI.Chakra.Dto.Presentation.PresentationPreviousApprovers DtoMap(DomainModel.Presentation.PresentationPreviousApprovers tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationPreviousApprovers, DI.Chakra.Dto.Presentation.PresentationPreviousApprovers>(tModel);
        }

        public  List<DI.Chakra.Dto.Presentation.PresentationPreviousApprovers> DtoMapList(IList<DomainModel.Presentation.PresentationPreviousApprovers> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

    }
}