using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("UserGridColumnInfo")]
    public class UserGridColumnInfoController : BaseController<DomainModel.UserGridColumnInfo, EntityModel.UserGridColumnInfo, Dto.UserGridColumnInfo>
    {
        [HttpGet]
        [Route("GetcolumnsByUserId")]
        public List<Dto.UserGridColumnInfo> GetcolumnsByUserId(long userId, string gridId, string pagePath)
        {
            return DtoMapList(DomainModel.UserGridColumnInfo.GetcolumnsByUserId(userId, gridId, pagePath));
        }

        [HttpGet]
        [Route("Delete")]
        public void Delete(long userId, string pagePath, string gridId)
        {
            DomainModel.UserGridColumnInfo.Delete(userId, pagePath, gridId);
        }
    }
}