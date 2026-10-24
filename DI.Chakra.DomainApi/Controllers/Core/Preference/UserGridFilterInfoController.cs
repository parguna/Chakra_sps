using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("UserGridFilterInfo")]
    public class UserFilterInfoController : BaseController<DomainModel.UserGridFilterInfo, EntityModel.UserGridFilterInfo, Dto.UserGridFilterInfo>
    {
        [HttpGet]
        [Route("Delete")]
        public void Delete(long userId, string pagePath, string gridId, string columnName)
        {
           DomainModel.UserGridFilterInfo.Delete(userId,pagePath,gridId,columnName);
        }

        [HttpGet]
        [Route("GetFilterByUserId")]
        public Dto.UserGridFilterInfo GetFilterByUserId(long userId, string gridId, string pagePath,string columnName)
        {
            return DtoMap(DomainModel.UserGridFilterInfo.GetFilterByUserId(userId, gridId, pagePath, columnName));
        }

        [HttpGet]
        [Route("GetFilterByUserId")]
        public List<Dto.UserGridFilterInfo> GetFilterByUserId(long userId, string gridId, string pagePath)
        {
            return DtoMapList(DomainModel.UserGridFilterInfo.GetFilterByUserId(userId, gridId, pagePath));
        }

        [HttpGet]
        [Route("Getfiltervalue")]
        public string Getfiltervalue(long userId, string gridId, string pagePath)
        {
            return DomainModel.UserGridFilterInfo.Getfiltervalue(userId, gridId, pagePath);
        }
    }
}
