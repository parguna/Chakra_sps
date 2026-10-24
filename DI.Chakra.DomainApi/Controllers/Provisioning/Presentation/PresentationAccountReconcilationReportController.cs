using AutoMapper;
using DI.Chakra.Dto.Core;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationAccountReconcilationReport")]
    public class PresentationAccountReconcilationReportController : AccountReconcilationController
    {
        [HttpGet]
        [Route("GetExceptions")]
        public List<PresentationAccountReconcilationReport> GetExceptions(int endpointSystemId, byte exceptionType, ReconcilationType reconcilationType, bool? isResolved = null, bool? isDeleted = null)
        {
            return DtoMapList(DomainModel.Presentation.PresentationAccountReconcilationReport.GetExceptions(endpointSystemId, exceptionType, reconcilationType, isResolved, isDeleted));
        }


        [HttpGet]
        [Route("GetResolvedReconcilations")]
        public List<PresentationAccountReconcilationReport> GetResolvedReconcilations(DateTime? FromDate, DateTime? ToDate, ResolvedStatus status, bool IsResolved)
        {
            return DtoMapList(DomainModel.Presentation.PresentationAccountReconcilationReport.GetResolvedReconcilations(FromDate, ToDate, status, IsResolved));
        }

        [HttpGet]
        [Route("GetEntitlementDetailsForAccessRequest")]
        public List<PresentationAccountReconcilationReport> GetEntitlementDetailsForAccessRequest(long accessRequestId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationAccountReconcilationReport.GetEntitlementDetailsForAccessRequest(accessRequestId));
        }

        [HttpGet]
        [Route("GetBySystem")]
        public List<PresentationAccountReconcilationReport> GetBySystem(long systemId, Nullable<AccountType> accType, MappingType mappingTypes = MappingType.None, bool employeeHasMoreAccounts = false, bool eagerLoadAllProperties = true, bool isLocked = false, bool isDeletedBoolean = false)
        {
            return DtoMapList(DomainModel.Presentation.PresentationAccountReconcilationReport.GetBySystem(systemId, accType, mappingTypes, employeeHasMoreAccounts, eagerLoadAllProperties, isLocked, isDeletedBoolean));
        }
        [HttpPost]
        [Route("GetLazyLoadAllSystem")]
        public PresentationGridPagedData<PresentationAccountReconcilationReport> GetLazyLoadAllSystem(PresentationGridPagedDataParameter param, long systemId, Nullable<AccountType> accType, long mappingTypes)
        {
            PresentationGridPagedData<PresentationAccountReconcilationReport> respResult = new PresentationGridPagedData<PresentationAccountReconcilationReport>();
            var dmResult = DomainModel.Presentation.PresentationAccountReconcilationReport.GetLazyLoadAllSystem(param.StartRowIndex, param.MaximumRows, param.FilterExpression, param.SortExpression, systemId, accType, mappingTypes);
            respResult.TotalCount = dmResult.TotalCount;
            respResult.Data = DtoMapList(dmResult.Data);
            return respResult;
        }
        public PresentationAccountReconcilationReport DtoMap(DomainModel.Presentation.PresentationAccountReconcilationReport tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationAccountReconcilationReport, PresentationAccountReconcilationReport>(tModel);
        }

        public List<PresentationAccountReconcilationReport> DtoMapList(IList<DomainModel.Presentation.PresentationAccountReconcilationReport> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
