using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AttestationSnapshotAction")]
    public class AttestationSnapshotActionController : BaseController<DomainModel.AttestationSnapshotAction, EntityModel.AttestationSnapshotAction, Dto.AttestationSnapshotAction>
    {
        [HttpGet]
        [Route("UpdateReviewerDetail")]
        public void UpdateReviewerDetail(long attestationId, long attestationCampaignId, long taskId, long assignedTo, string currentUser)
        {
            DomainModel.AttestationSnapshotAction.UpdateReviewerDetail(attestationId, attestationCampaignId, taskId, assignedTo, currentUser);
        }
        [HttpGet]
        [Route("CheckAllPositionSelected")]
        public bool CheckAllPositionSelected(long snapShotActionId,string accountName,long endpointSystemId, long campaignId)
        {
            return DomainModel.AttestationSnapshotAction.CheckAllPositionSelected(snapShotActionId, accountName, endpointSystemId, campaignId);
        }
    }
}