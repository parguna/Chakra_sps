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
    [RoutePrefix("EntitlementReconcilation")]
    public class EntitlementReconcilationController : BaseController<DomainModel.EntitlementReconcilation, EntityModel.EntitlementReconcilation, EntitlementReconcilation>
    {
        [HttpGet]
        [Route("GetByUser")]
        public EntitlementReconcilation GetByUser(long endpointUserId, long endpointSystemId, string entitlement, EntitlementType entitlementType)
        {
            return DtoMap(DomainModel.EntitlementReconcilation.GetByUser(endpointUserId, endpointSystemId, entitlement, entitlementType));
        } 

        [HttpGet]
        [Route("GetByAccountReconcilationId")]
        public EntitlementReconcilation GetByAccountReconcilationId(long accountReconcilationId)
        {
            return DtoMap(DomainModel.EntitlementReconcilation.GetByAccountReconcilationId(accountReconcilationId));
        }

        [HttpGet]
        [Route("GetByReconcilationId")]
        public List<EntitlementReconcilation> GetByReconcilationId(long endpointSystemId, long reconcilationId)
        {
            return DtoMapList(DomainModel.EntitlementReconcilation.GetByReconcilationId(endpointSystemId, reconcilationId));
        }
        [HttpGet]
        [Route("GetByUserEntitlement")]
        public EntitlementReconcilation GetByUserEntitlement(long endpointUserId, long endpointSystemId, string entitlement)
        {
            return DtoMap(DomainModel.EntitlementReconcilation.GetByUserEntitlement(endpointUserId, endpointSystemId, entitlement));
        }
        [HttpGet]
        [Route("GetByUserEntitlementByType")]
        public EntitlementReconcilation GetByUserEntitlementByType(long endpointUserId, long endpointSystemId, string entitlement)
        {
            return DtoMap(DomainModel.EntitlementReconcilation.GetByUserEntitlementByType(endpointUserId, endpointSystemId, entitlement));
        }
        [HttpGet]
        [Route("GetByUserEntitlementByExceptGroup")]
        public List<EntitlementReconcilation> GetByUserEntitlementByExceptGroup(long endpointUserId, long endpointSystemId)
        {
            return DtoMapList(DomainModel.EntitlementReconcilation.GetByUserEntitlementByType(endpointUserId, endpointSystemId));
        }
        [HttpGet]
        [Route("ExecuteCriticalReportByUser")]
        public bool ExecuteCriticalReportByUser(string accountName, long endpointSystemId)
        {
            if (!string.IsNullOrWhiteSpace(accountName))
            {
                return DomainModel.EntitlementReconcilation.GetActiveAccountsForCriticalReport(endpointSystemId, accountName);
            }
            return false;
        }

    }
}
