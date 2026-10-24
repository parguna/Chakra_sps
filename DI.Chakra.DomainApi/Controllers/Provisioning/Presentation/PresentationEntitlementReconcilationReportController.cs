using AutoMapper;
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
    [RoutePrefix("PresentationEntitlementReconcilationReport")]
    public class PresentationEntitlementReconcilationReportController : EntitlementReconcilationController
    {
        [HttpGet]
        [Route("GetExceptions")]
        public List<PresentationEntitlementReconcilationReport> GetExceptions(int endpointSystemId, ExceptionTypes exceptionType)
        {
            return DtoMapList(DomainModel.Presentation.PresentationEntitlementReconcilationReport.GetExceptions(endpointSystemId, exceptionType));
        }

        public  PresentationEntitlementReconcilationReport DtoMap(DomainModel.Presentation.PresentationEntitlementReconcilationReport tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationEntitlementReconcilationReport, PresentationEntitlementReconcilationReport>(tModel);
        }

        public  List<PresentationEntitlementReconcilationReport> DtoMapList(IList<DomainModel.Presentation.PresentationEntitlementReconcilationReport> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
