using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AttestationSnapshotActionHistory")]
    public class AttestationSnapshotActionHistoryController : BaseController<DomainModel.AttestationSnapshotActionHistory, EntityModel.AttestationSnapshotActionHistory, Dto.AttestationSnapshotActionHistory>
    {
        [HttpGet]
        [Route("GetAttestationActionHistory")]
        public List<AttestationSnapshotActionHistory> GetAttestationActionHistory(long attestationActionId, long taskId)
        {
            var task = DomainModel.Task.GetById(taskId);
            bool loadAllHistroy = true;
            long versionTillToLoad = 0;
            if (task != null && !task.IsCompleted)
            {
                loadAllHistroy = false;
                versionTillToLoad = task.WorkflowStateId;
            }
            return DtoMapList(DomainModel.AttestationSnapshotActionHistory.GetAttestationActionHistory(attestationActionId, loadAllHistroy, versionTillToLoad)).ToList();
        }

        [HttpGet]
        [Route("GetWithValidComments")]
        public List<AttestationSnapshotActionHistory> GetWithValidComments(long snapshotActionId, long taskId)
        {
            var task = DomainModel.Task.GetById(taskId);
            bool loadAllHistroy = true;
            long versionTillToLoad = 0;
            if (task != null && !task.IsCompleted)
            {
                loadAllHistroy = false;
                versionTillToLoad = task.WorkflowStateId;
            }

            return DtoMapList(DomainModel.AttestationSnapshotActionHistory.GetAttestationActionReviewHistory(snapshotActionId, loadAllHistroy, versionTillToLoad)).ToList();
        }
    }
}