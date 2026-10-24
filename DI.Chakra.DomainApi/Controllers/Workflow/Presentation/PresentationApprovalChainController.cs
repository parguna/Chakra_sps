using AutoMapper;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Workflow.Processor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationApprovalChain")]
    public class PresentationApprovalChainController : AccessRequestController
    {
        [HttpGet]
        [Route("GetByRequestId")]
        public List<Dto.Presentation.PresentationApprovalChain> GetByRequestId(long accessRequestId)
        {
            ApprovalChainFinder workflowHelper = new ApprovalChainFinder();
            return DtoMapList(workflowHelper.Execute(accessRequestId, false));

        }

        [HttpGet]
        [Route("GetByAdhocRequestId")]
        public List<Dto.Presentation.PresentationApprovalChain> GetByAdhocRequestId(long adhocAccessRequestId)
        {
            DomainModel.AccessRequest accessRequest = DomainModel.AccessRequest.GetByAdHocAccessRequestId(adhocAccessRequestId);
            if (accessRequest == null) return new List<Dto.Presentation.PresentationApprovalChain>();

            ApprovalChainFinder workflowHelper = new ApprovalChainFinder();
            return DtoMapList(workflowHelper.Execute(accessRequest.Id, false));

        }

        [HttpGet]
        [Route("GetByBulkRequestId")]
        public List<Dto.Presentation.PresentationApprovalChain> GetByBulkRequestId(long bulkRequestId)
        {
            ApprovalChainFinder workflowHelper = new ApprovalChainFinder();
            return DtoMapList(workflowHelper.Execute(bulkRequestId, true));

        }

        [HttpGet]
        [Route("GetByEmployeeRequestId")]
        public List<Dto.Presentation.PresentationApprovalChain> GetByEmployeeRequestId(long triggerId)
        {
            ApprovalChainFinder workflowHelper = new ApprovalChainFinder();
            return DtoMapList(workflowHelper.Execute(triggerId));

        }


        public List<Dto.Presentation.PresentationUserEntitlement> DtoMapList2(IList<DomainModel.Presentation.PresentationUserEntitlement> tModelList)
        {
            return tModelList.Select(DtoMap2).ToList();
        }


        public List<Dto.Presentation.PresentationApprovalChain> DtoMapList(IList<DomainModel.Presentation.PresentationApprovalChain> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
        public Dto.Presentation.PresentationApprovalChain DtoMap(DomainModel.Presentation.PresentationApprovalChain tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationApprovalChain, Dto.Presentation.PresentationApprovalChain>(tModel);
        }

        public Dto.Presentation.PresentationUserEntitlement DtoMap2(DomainModel.Presentation.PresentationUserEntitlement tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationUserEntitlement, Dto.Presentation.PresentationUserEntitlement>(tModel);
        }
    }
}
