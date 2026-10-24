using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto.Presentation;
using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using DI.Chakra.Dto.Core;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationTask")]
    public class PresentationTaskController : TaskController
    {
        [HttpGet]
        [Route("GetAll")]
        public IList<PresentationTask> GetAll(string FromDate, string ToDate,long requestFor=0,long endpointSystemId = 0, StatusType statusType = StatusType.None)
        {
            DateTime? dtFrom = UtilityMethods.GetDateFromString(FromDate, Constants.DateFormat);
            DateTime? dtTo = UtilityMethods.GetDateFromString(ToDate, Constants.DateFormat);
            return DtoMapList(DomainModel.Presentation.PresentationTask.GetAll(dtFrom, dtTo, requestFor, endpointSystemId, statusType));
        }

        [HttpGet]
        [Route("GetAllExecutorTask")]
        public List<PresentationTask> GetAllExecutorTask()
        {
            return DtoMapList(DomainModel.Presentation.PresentationTask.GetAllExecutorTask());
        }

        [HttpGet]
        [Route("GetAllExecutorTaskReport")]
        public List<PresentationTask> GetAllExecutorTaskReport(DateTime? fromDate, DateTime? toDate, StatusType statusType)
        {
            return DtoMapList(DomainModel.Presentation.PresentationTask.GetAllExecutorTaskReport(fromDate,toDate,statusType));
        }


        [HttpGet]
        [Route("GetByStatus")]
        public List<PresentationTask> GetByStatus(StatusType taskStatus, string FromDate, string ToDate, long requestFor = 0, ApplicationType applicationType = ApplicationType.None)
        {
            DateTime? dtFrom = UtilityMethods.GetDateFromString(FromDate, Constants.DateFormat);
            DateTime? dtTo = UtilityMethods.GetDateFromString(ToDate, Constants.DateFormat);
            return DtoMapList(DomainModel.Presentation.PresentationTask.GetByStatus(taskStatus, dtFrom, dtTo, requestFor, applicationType));
        }

        [HttpGet]
        [Route("GetByActionUserId")]
        public  List<PresentationTask> GetByActionUserId(long actionUserId, string FromDate, string ToDate, long endpointSystemId=0)
        {
            DateTime? dtFrom = UtilityMethods.GetDateFromString(FromDate, Constants.DateFormat);
            DateTime? dtTo = UtilityMethods.GetDateFromString(ToDate, Constants.DateFormat);
            return DtoMapList(DomainModel.Presentation.PresentationTask.GetByActionUserId(actionUserId, dtFrom, dtTo, endpointSystemId));
        }

        [HttpGet]
        [Route("GetTaskDetailsByTaskStatus")]
        public  List<PresentationTask> GetTaskDetailsByTaskStatus(long actionUserId, StatusType taskStatus, string FromDate, string ToDate, long endpointSystemId = 0)
        {
            DateTime? dtFrom = UtilityMethods.GetDateFromString(FromDate, Constants.DateFormat);
            DateTime? dtTo = UtilityMethods.GetDateFromString(ToDate, Constants.DateFormat);
            return DtoMapList(DomainModel.Presentation.PresentationTask.GetTaskDetailsByTaskStatus(actionUserId, taskStatus, dtFrom, dtTo, endpointSystemId));
        }

        [HttpGet]
        [Route("GetGroupTaskDetailsByActionUserId")]
        public  List<PresentationTask> GetGroupTaskDetailsByActionUserId(long actionUserId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationTask.GetGroupTaskDetailsByActionUserId(actionUserId));
        }
        [HttpPost]
        [Route("GetGroupTaskDetailsByActionUserId")]
        public Dto.Core.PresentationGridPagedData<PresentationTask> GetGroupTaskDetailsByActionUserId(StatusType? taskStatus, long actionUserId, PresentationGridPagedDataParameter param,long requestFor=0,long endpointId=0)
        {
            var response = new Dto.Core.PresentationGridPagedData<PresentationTask>();
            var dmRsult = DomainModel.Presentation.PresentationTask.GetGroupTaskDetailsByActionUserId(actionUserId,taskStatus, param.StartRowIndex, param.MaximumRows, param.FilterExpression, param.SortExpression, requestFor, param.FromDate, param.ToDate, endpointId);
            response.TotalCount = dmRsult.TotalCount;
            response.Data = DtoMapList(dmRsult.Data);
            return response;
        }
        [HttpGet]
        [Route("GetAllGroupTask")]
        public List<PresentationTask> GetAllGroupTask(long actionUserId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationTask.GetAllGroupTask(actionUserId));
        }
        [HttpGet]
        [Route("GetGroupTaskDetailsByTaskStatus")]
        public  List<PresentationTask> GetGroupTaskDetailsByTaskStatus(long actionUserId, StatusType taskStatus)
        {
            return DtoMapList(DomainModel.Presentation.PresentationTask.GetGroupTaskDetailsByTaskStatus(actionUserId, taskStatus));
        }

        public  PresentationTask DtoMap(DomainModel.Presentation.PresentationTask tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationTask, PresentationTask>(tModel);
        }
        public  List<PresentationTask> DtoMapList(IList<DomainModel.Presentation.PresentationTask> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        public PagedResults<Dto.Presentation.PresentationTask> DtoMapListPaging(PagedResults<DomainModel.Presentation.PresentationTask> tModelList)
        {
            var results = tModelList.Results;
            var mappedResults = results.Select(DtoMap).ToList();

            return new PagedResults<PresentationTask>()
            {
                PageNumber = tModelList.PageNumber,
                PageSize = tModelList.PageSize,
                TotalNumberOfPages = tModelList.TotalNumberOfPages,
                TotalNumberOfRecords = tModelList.TotalNumberOfRecords,
                Results = mappedResults
            };
        }

        [HttpGet]
        [Route("GetCampaignTaskDetail")]
        public List<PresentationTask> GetCampaignTaskDetail(long AttestationCampaignId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationTask.GetCampaignTaskDetail(AttestationCampaignId));
        }
        [HttpGet]
        [Route("GetTaskDetailsByActionUserIdAndTaskStatusForBulkAssign")]
        public List<PresentationTask> GetTaskDetailsByActionUserIdAndTaskStatusForBulkAssign(long actionUserId, StatusType taskStatus, long groupId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationTask.GetTaskDetailsByActionUserIdAndTaskStatusForBulkAssign(actionUserId, taskStatus, groupId));
        }

        [HttpPost]
        [Route("GetTaskPagedResult")]
        public Dto.Core.PresentationGridPagedData<Dto.Presentation.PresentationTask> GetTaskPagedResult(PresentationGridPagedDataParameter presentationGridPagedDataParameter, long requestFor = 0, long endpointSystemId = 0, StatusType statusType = StatusType.None, long actionUserId = 0)
        {
            DateTime? dtFrom = presentationGridPagedDataParameter.FromDate;
            DateTime? dtTo = presentationGridPagedDataParameter.ToDate;
            var data = DomainModel.Presentation.PresentationTask.GetPagedResult(dtFrom, dtTo, presentationGridPagedDataParameter.StartRowIndex, presentationGridPagedDataParameter.MaximumRows, presentationGridPagedDataParameter.FilterExpression, presentationGridPagedDataParameter.SortExpression, requestFor, endpointSystemId, statusType, actionUserId);
            Dto.Core.PresentationGridPagedData<Dto.Presentation.PresentationTask> presentationGridPagedData = new PresentationGridPagedData<PresentationTask>();
            presentationGridPagedData.Data = DtoMapList(data.Data);
            presentationGridPagedData.TotalCount = data.TotalCount;
            return presentationGridPagedData;
        }
        [HttpPost]
        [Route("WitHDrawHangingtask")]
        public string TaskWithDraw(PresentationTask task)
        {
            return DomainModel.Presentation.PresentationTask.TaskWithDraw(task.TaskId, task.DelegatedComment);
        }
        [HttpGet]
        [Route("ReinitiateRequest")]
        public string ReinitiateRequest(RequestType requestType, long requestId)
        {
            return DomainModel.Presentation.PresentationTask.ReinitiateRequest(requestType, requestId);
        }
    }
}
