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
    [RoutePrefix("ProvisioningLog")]
    public class ProvisioningLogController : BaseController<DomainModel.ProvisioningLog, EntityModel.ProvisioningLog, ProvisioningLog>
    {
        [HttpGet]
        [Route("GetByLogleveltype")]
        public IList<ProvisioningLog> GetByLogleveltype(LogLevelType logleveltype, int pageSize, int pageNumber)
        {
            return DtoMapList(DomainModel.ProvisioningLog.GetByLogleveltype(logleveltype, pageSize, pageNumber));
        }

        [HttpGet]
        [Route("GetByLogleveltype")]
        public IList<ProvisioningLog> GetByLogleveltype(LogLevelType logleveltype)
        {
            return DtoMapList(DomainModel.ProvisioningLog.GetByLogleveltype(logleveltype));
        }

        [HttpGet]
        [Route("GetCountByLogleveltype")]
        public long GetCountByLogleveltype(LogLevelType logleveltype)
        {
            return DomainModel.ProvisioningLog.GetCountByLogleveltype(logleveltype);
        }

        //[HttpGet]
        //[Route("GetByReferenceType")]
        //public IList<ProvisioningLog> GetByReferenceType(LogReferenceType logReferenceType, LogLevelType logLevelType)
        //{
        //    return DtoMapList(DomainModel.ProvisioningLog.GetByReferenceType(logReferenceType, logLevelType));
        //}

        [HttpPost]
        [Route("GetByLogDate")]
        public IList<ProvisioningLog> GetByLogDate(LogParameter param)
        {
            var logs = DomainModel.ProvisioningLog.GetByLogDate(param.dtFrom, param.dtTo, param.logLevelType, param.pageSize, param.pageNumber, param.FilterExpression, param.sortExpression, param.RequestId);
            var returnValues = DtoMapList(logs);
            return returnValues;
        }

        [HttpPost]
        [Route("GetCountByLogDate")]
        public long GetCountByLogDate(LogParameter param)//LogReferenceType? logReferenceType, DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType, string FilterExpression, string sortExpression)
        {
            return DomainModel.ProvisioningLog.GetCountByLogDate(param.dtFrom, param.dtTo, param.logLevelType, param.FilterExpression, param.sortExpression,param.RequestId);
        }

        //[HttpGet]
        //[Route("GetLoginDetail")]
        //public IList<ProvisioningLog> GetLoginDetail()
        //{
        //    return DtoMapList(DomainModel.ProvisioningLog.GetLoginDetail());
        //}
    }
}
