using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Infrastructure.Enums;
using AutoMapper;
using DI.Chakra.Dto.Core;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers.Attestation
{
    [RoutePrefix("FilterData")]
    public class FilterDataController : ApiController
    {
        [HttpPost]
        [Route("GetAttestationUserComparisionData")]
        public PresentationGridPagedData<ViewUserEntitlementData> GetAttestationUserComparisionData(APIAttestationSearch search)
        {
            PresentationGridPagedData<ViewUserEntitlementData> result = new PresentationGridPagedData<ViewUserEntitlementData>();
            var epIds = new List<long>();
            if(search != null && search.EndpointIds != null && search.EndpointIds.Count > 0)
            {
                epIds = search.EndpointIds;
            }
            var tSearch = search?.Value;
            var apiResult = DomainModel.PresentationUserEntitlementData.GetAttestationUserComparisionData(search.TaskId, search.Draw, search.Start, search.Length, epIds, search.AccountType, tSearch);
            result.TotalCount = apiResult.TotalCount;
            result.Data = DtoMapList(apiResult.Data).ToList();
            return result;
        }
        public static List<ViewUserEntitlementData> DtoMapList(IList<DomainModel.ViewUserEntitlementData> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
        public static ViewUserEntitlementData DtoMap(DomainModel.ViewUserEntitlementData tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.ViewUserEntitlementData, Dto.ViewUserEntitlementData>(tModel);
        }
        [HttpPost]
        [Route("GetFilterDataByType")]
        public List<FilterData> GetFilterDataByType(TaskFilter taskFilter)
        {
            return ConvertToDto(DomainModel.PresentationUserEntitlementData.GetFilterDataByType(taskFilter.TaskId, taskFilter.FilterType, taskFilter.EpIds, taskFilter.UserIds,taskFilter.AccountType));
        }
        [HttpPost]
        [Route("GetEmployeeInformation")]
        public Dictionary<string, List<FilterData>> GetEmployeeInformation(TaskFilter taskFilter)
        {
            var Result = new Dictionary<string, List<FilterData>>();
            var data = DomainModel.PresentationUserEntitlementData.GetEmployeeInformation(taskFilter.TaskId, taskFilter.AccountType, taskFilter.EpIds);
            foreach (var item in data)
            {
                Result.Add(item.Key, ConvertToDto(item.Value));
            }
            return Result;
        }
        public static List<FilterData> ConvertToDto(IList<DomainModel.FilterData> tModelList)
        {
            List<FilterData> filterDatas = new List<FilterData>();
            foreach (var item in tModelList)
            {
                filterDatas.Add(new FilterData { Id = item.Id, Name = item.Name,EndpointId=item.EndpointId, AdditionalInfo = item.AdditionalInfo, EndpointName = item.EndpointName, UserName = item.UserName, AccountStatus = item.AccountStatus, IsActionable = item.IsActionable, EntitlementType = item.EntitlementType });
            }
            return filterDatas;
        }
        [HttpPost]
        [Route("UpdateAttestationTaskActionByUser")]
        public bool UpdateAttestationTaskActionByUser(DTORequestParameter dTORequest)
        {
            EntitlementType entType = (EntitlementType)Enum.Parse(typeof(EntitlementType), dTORequest.EntitlementType);
            return DomainModel.PresentationUserEntitlementData.UpdateAttestationTaskActionByUser(dTORequest.TaskId, dTORequest.EndpointId, dTORequest.UserId, dTORequest.Entitlement, dTORequest.UserName, dTORequest.AccountType, dTORequest.AccessType, dTORequest.ActionUserId, dTORequest.Level, entType);
        }

        [HttpGet]
        [Route("GetAccountTypes")]
        public List<AccountType> GetAccountTypes(long taskId)
        {
            return DomainModel.PresentationUserEntitlementData.GetAccountTypes(taskId);
        }
        [HttpPost]
        [Route("GetAccountStatusInformation")]
        public List<FilterData> GetAccountStatusInformation(TaskFilter taskFilter)
        {
            var Result = new Dictionary<string, List<FilterData>>();
            var data = DomainModel.PresentationUserEntitlementData.GetAccountStatusInformation(taskFilter.TaskId, taskFilter.AccountType, taskFilter.UserId, taskFilter.AccountName);
            return ConvertToDto(data);
        }
    }
}