using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AttestationCampaign")]
    public class AttestationCampaignController : BaseController<DomainModel.AttestationCampaign, EntityModel.AttestationCampaign, Dto.AttestationCampaign>
    {
        [HttpGet]
        [Route("GetByWorkflowInstanceId")]
        public AttestationCampaign GetByWorkflowInstanceId(Guid workflowInstanceId)
        {
            return DtoMap(DomainModel.AttestationCampaign.GetByWorkflowInstanceID(workflowInstanceId));
        }

        [HttpGet]
        [Route("GetByCampaignId")]
        public AttestationCampaign GetByCampaignId(long campaignId)
        {
            return DtoMap(DomainModel.AttestationCampaign.GetByCampaignId(campaignId));
        }
        [HttpGet]
        [Route("GetAllChildCampaignByCampaignId")]
        public List<AttestationCampaign> GetAllChildCampaignByCampaignId(long campaignId)
        {
            return DtoMapList(DomainModel.AttestationCampaign.GetAllByCampaignId(campaignId));
        }
    }
}