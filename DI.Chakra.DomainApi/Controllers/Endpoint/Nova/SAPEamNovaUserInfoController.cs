using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPEamNovaUserInfo")]
    public class SAPEamNovaUserInfoController : BaseController<DomainModel.SAPEamNovaUserInfo, EntityModel.SAPEamNovaUserInfo, Dto.SAPEamNovaUserInfo>
    {
        [HttpGet]
        [Route("GetNovaJobByMatrixType")]
        public Dictionary<long, string> GetNovaJobByMatrixType(string searchString, AdHocActionType adhocActionType,long userId,long endpointId,NovaMatrixType matrixType,string SAPSectionCode, string DepartmentCode=null)
        {
            return DomainModel.SAPEamNovaUserInfo.GetByMatrixType(searchString, adhocActionType, userId, endpointId, matrixType,SAPSectionCode, DepartmentCode);
        }
        [HttpGet]
        [Route("GetUser")]
        public  User GetUser(long id)
        {
            return AutoMapper.Mapper.Map<Dto.User>( DomainModel.SAPEamNovaUserInfo.GetUser(id)); 
        }
    }
}