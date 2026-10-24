using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AttestationLog")]
    public class AttestationLogController : BaseController<DomainModel.AttestationLog, EntityModel.AttestationLog, AttestationLog>
    {
        //[HttpGet]
        //[Route("GetByLogleveltype")]
        //public IList<ServiceLog> GetByLogleveltype(LogLevelType logleveltype, int pageSize, int pageNumber)
        //{
        //    return DtoMapList(DomainModel.DiscoveryLog.GetByLogleveltype(logleveltype, pageSize, pageNumber));
        //}

        //[HttpGet]
        //[Route("GetByLogleveltype")]
        //public IList<ServiceLog> GetByLogleveltype(LogLevelType logleveltype)
        //{
        //    return DtoMapList(DomainModel.DiscoveryLog.GetByLogleveltype(logleveltype));
        //}

        //[HttpGet]
        //[Route("GetCountByLogleveltype")]
        //public long GetCountByLogleveltype(LogLevelType logleveltype)
        //{
        //    return DomainModel.DiscoveryLog.GetCountByLogleveltype(logleveltype);
        //}

        //[HttpGet]
        //[Route("GetByReferenceType")]
        //public IList<ServiceLog> GetByReferenceType(LogReferenceType logReferenceType, LogLevelType logLevelType)
        //{
        //    return DtoMapList(DomainModel.DiscoveryLog.GetByReferenceType(logReferenceType, logLevelType));
        //}

        [HttpGet]
        [Route("GetByLogDate")]
        public IList<AttestationLog> GetByLogDate(LogReferenceType? logReferenceType, DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType, int? pageSize, int? pageNumber, string FilterExpression, string sortExpression)
        {

            FilterExpression = UtilityMethods.StripLogForgingCharacters(FilterExpression);
            sortExpression = UtilityMethods.StripLogForgingCharacters(sortExpression);
            return DtoMapList(DomainModel.AttestationLog.GetAttestationLogByLogDate(logReferenceType, dtFrom, dtTo, logLevelType, pageSize, pageNumber, FilterExpression, sortExpression));

        }

        [HttpGet]
        [Route("GetCountByLogDate")]
        public long GetCountByLogDate(LogReferenceType? logReferenceType, DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType, string FilterExpression, string sortExpression)
        {
            FilterExpression = UtilityMethods.StripLogForgingCharacters(FilterExpression);
            sortExpression = UtilityMethods.StripLogForgingCharacters(sortExpression);
            return DomainModel.AttestationLog.GetCountByLogDate( dtFrom, dtTo, logLevelType, FilterExpression, sortExpression);
        }

        //[HttpGet]
        //[Route("GetLoginDetail")]
        //public IList<ServiceLog> GetLoginDetail()
        //{
        //    return DtoMapList(DomainModel.DiscoveryLog.GetLoginDetail());
        //}
    }
}
