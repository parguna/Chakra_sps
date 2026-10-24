using AutoMapper;
using DI.Chakra.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NPACLevelInfo")]
    public class NPACLevelInfoController : BaseController<DomainModel.NPACLevelInfo, EntityModel.NPACLevelInfo, Dto.NPACLevelInfo>
    {
        [HttpGet]
        [Route("GetNPACLevelInfoList")]
        public Dictionary<int, string> GetNPACLevelInfoList(long RequesteeId)
        {
            DomainModel.User user = DomainModel.User.GetById(RequesteeId);

            if (user != null && (user.FullTimeEmployee || user.EmployeeType == Infrastructure.Enums.EmployeeType.TempStaff))
            {
                return DomainModel.NPACLevelInfo.GetAll().Where(level => level.Level != null).Select(level => level).ToDictionary(key => key.Level.Value, value => value.Description);
            }
            else
            {
                var level = DomainModel.NPACLevelInfo.GetByLevel(3);
                if (level == null) return new Dictionary<int, string>();
                return new Dictionary<int, string> { { level.Level.Value, level.Description } };
            }
        }

        [HttpGet]
        [Route("GetPrimaryByDIUserId")]
        public NPACUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            var result = DomainModel.NPACUserInfo.GetPrimaryByDIUserId(dIUserId, endPointSystemId);
            if (result == null)
                return null;

            return Mapper.Map<DomainModel.NPACUserInfo, NPACUserInfo>(result);
        }
    }
}