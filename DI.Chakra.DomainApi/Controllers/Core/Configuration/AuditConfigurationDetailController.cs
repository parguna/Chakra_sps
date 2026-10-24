using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AuditConfigurationDetail")]
    public class AuditConfigurationDetailController : BaseController<DomainModel.AuditConfigurationDetail, EntityModel.AuditConfigurationDetail, Dto.AuditConfigurationDetail>
    {
        [HttpGet]
        [Route("GetPropertiesByTableName")]
        public List<string> GetPropertiesByTableName(string tableName)
        {
             return DomainModel.AuditConfigurationDetail.GetPropertiesByTableName(tableName);
        }
        [HttpGet]
        [Route("GetByTableProperties")]
        public long GetByTableProperties(string tableName, string property)
        {
            return DomainModel.AuditConfigurationDetail.GetByTableProperties(tableName, property);
        }
    }
}