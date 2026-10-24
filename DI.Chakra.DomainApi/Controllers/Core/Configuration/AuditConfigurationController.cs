using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using TrackerEnabledDbContext.Common.Models;
using System.Data;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AuditConfiguration")]
    public class AuditConfigurationController : BaseController<DomainModel.AuditConfiguration, EntityModel.AuditConfiguration, Dto.AuditConfiguration>
    {
        [HttpGet]
        [Route("GetByTableName")]
        public AuditConfiguration GetByTableName(string tableName)
        {
            return DtoMap(DomainModel.AuditConfiguration.GetByTableName(tableName));
        }
        [HttpGet]
        [Route("GetEntitiesToAudit")]
        [AllowAnonymous]
        public Dictionary<string, List<string>> GetEntitiesToAudit()
        {
            return DomainModel.AuditConfiguration.GetEntitiesToAudit();
        }
        [HttpGet]
        [Route("GetLogs")]
        public IList<AuditLog> GetLogs(string entityName, DateTime? filterDate, string userName)
        {
            return DomainModel.AuditConfiguration.GetLogs(entityName, filterDate, userName);
        }
        [HttpGet]
        [Route("GetTableList")]
        public IList<string> GetTableList(string tableName, DateTime? filterDate, string userName)
        {
            return DomainModel.AuditConfiguration.GetTableList(tableName, filterDate, userName);
        }
        [HttpGet]
        [Route("GetAuditLogDetail")]
        public List<AuditLogDetail> GetAuditLogDetail(long auditLogId)
        {
            return DomainModel.AuditConfiguration.GetAuditLogDetail(auditLogId);
        }
        [HttpGet]
        [Route("GetAllTableList")]
        public IList<string> GetAllTableList()
        {
            return DomainModel.AuditConfiguration.GetAllTableList();
        }
        [HttpGet]
        [Route("GetAuditLogRecordDetail")]
        public DataTable GetAuditLogRecordDetail(long auditLogId)
        {
            return DomainModel.AuditConfiguration.GetAuditLogRecordDetail(auditLogId);
        }
    }
}