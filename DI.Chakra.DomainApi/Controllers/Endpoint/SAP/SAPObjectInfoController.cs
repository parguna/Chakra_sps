using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPObjectInfo")]
    public class SAPObjectInfoController : BaseController<DomainModel.SAPObjectInfo, EntityModel.SAPObjectInfo, Dto.SAPObjectInfo>
    {
        [HttpGet]
        [Route("GetSapFields")]
        public List<string> GetSapFields(long endPointId, string objCode)
        {
            return DomainModel.SAPObjectInfo.GetSapFields(endPointId, objCode);
        }

        [HttpGet]
        [Route("GetDictionaryByObjCode")]
        public  Dictionary<long, string> GetDictionaryByObjCode(long endPointId, string objCode)
        {
            return DomainModel.SAPObjectInfo.GetDictionaryByObjCode(endPointId, objCode);
        }
    }
}
