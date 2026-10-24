using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("DiscoveryLog")]
    public class DiscoveryLogController : BaseController<DomainModel.DiscoveryLog, EntityModel.DiscoveryLog, DiscoveryLog>
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

        [HttpPost]
        [Route("GetByLogDate")]
        public IList<DiscoveryLog> GetByLogDate(LogParameter logParameter)  //(LogReferenceType? logReferenceType, DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType, int? pageSize, int? pageNumber, string FilterExpression, string sortExpression,long? endpointSystemId, DiscoveryObjectType objectType)
        {

            return DtoMapList(DomainModel.DiscoveryLog.GetDiscoveryLogByLogDate( logParameter.dtFrom, logParameter.dtTo, logParameter.logLevelType, logParameter.pageSize, logParameter.pageNumber, logParameter.FilterExpression, logParameter.sortExpression, logParameter.endpointSystemId, logParameter.objectType));

        }

        [HttpPost]
        [Route("GetCountByLogDate")]
        public long GetCountByLogDate(LogParameter logParameter)  //(LogReferenceType? logReferenceType, DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType, string FilterExpression, string sortExpression, long? endpointSystemId, DiscoveryObjectType objectType)
        {
            return DomainModel.DiscoveryLog.GetCountByLogDate(logParameter.dtFrom, logParameter.dtTo, logParameter.logLevelType, logParameter.FilterExpression, logParameter.sortExpression, logParameter.endpointSystemId, logParameter.objectType);
        }

        //[HttpGet]
        //[Route("GetLoginDetail")]
        //public IList<ServiceLog> GetLoginDetail()
        //{
        //    return DtoMapList(DomainModel.DiscoveryLog.GetLoginDetail());
        //}
    }
}
