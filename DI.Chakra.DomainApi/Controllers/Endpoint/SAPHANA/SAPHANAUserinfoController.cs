using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPHANAUserinfo")]
    public class SAPHANAUserinfoController : BaseController<DomainModel.SAPHANAUserinfo, EntityModel.SAPHANAUserinfo, Dto.SAPHANAUserinfo>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string userName, long endpointsystemId)
        {
            return DomainModel.SAPHANAUserinfo.DoesExists(userName, endpointsystemId);
        }

        [HttpGet]
        [Route("GetSAPUserByUserId")]
        public SAPHANAUserinfo GetSAPUserByUserId(long userId, string UserAccountName)
        {
            return DtoMap(DomainModel.SAPHANAUserinfo.GetSAPUserByUserId(userId, UserAccountName));
        }
        [HttpGet]
        [Route("GetPositionTextByUser")]
        public Dictionary<long, string> GetPositionTextByUser(long endpointUserId, long endpointSystemId)
        {
            return DomainModel.SAPHANAUserinfo.GetPositionTextByUser(endpointUserId, endpointSystemId);
        }

        [HttpGet]
        [Route("GetActivePrimarySAPUserByDIUserId")]
        public Dictionary<string, string> GetActivePrimarySAPUserByDIUserId(long dIUserId, ApplicationType applicationType, AccountType accountType)
        {
            return DomainModel.SAPHANAUserinfo.GetActivePrimarySAPUserByDIUserId(dIUserId, applicationType, accountType);
        }


        [HttpGet]
        [Route("GetSAPSODConflicts")]
        public List<string> GetSAPSODConflicts(long dIUserId, long requestId, bool isBulkRequest, string applicationType)
        {
            return DomainModel.SAPHANAUserinfo.GetSAPSODConflicts(dIUserId, requestId, isBulkRequest, applicationType);
        }
        [HttpGet]
        [Route("GetSAPSODConflictsShowingrid")]
        public List<PresentationConflictRemarks> GetSAPSODConflictsShowingrid(long dIUserId, long requestId, bool isBulkRequest, string applicationType)
        {
            List<PresentationConflictRemarks> fResults = new List<PresentationConflictRemarks>();
            var results = (DomainModel.SAPHANAUserinfo.GetSAPSODConflictsShowingrid(dIUserId, requestId, isBulkRequest, applicationType));

            if (results != null && results.Count > 0)
            {
                foreach (var item in results)
                {
                    fResults.Add(new PresentationConflictRemarks()
                    {
                        EndpointName = item.EndpointName,
                        ConflictedRole = item.ConflictedRole,
                        ConflictRemarks = item.ConflictRemarks,
                        RequestedRole = item.RequestedRole,
                        Source = item.Source
                    });
                }
            }
            return fResults;
        }
    }
}

