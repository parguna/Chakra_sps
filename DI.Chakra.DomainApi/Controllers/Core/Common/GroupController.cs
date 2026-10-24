using DI.Chakra.Dto;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("Group")]
    public class GroupController : BaseController<DomainModel.Group, EntityModel.Group, Dto.Group>
    {
        [HttpGet]
        [Route("GetByGroupName")]
        public Group GetByGroupName(string groupName)
        {
            return DtoMap(DomainModel.Group.GetByGroupName(groupName));
        }

        [HttpDelete]
        [Route("DeleteUserFromGroup")]
        public void DeleteUserFromGroup([FromUri]long groupId, [FromBody]User userToRemove)
        {
            DomainModel.Group.DeleteUserFromGroup(groupId, AutoMapperHelper.DtoUnMap<Dto.User, DomainModel.User>(userToRemove));
        }

        [HttpGet]
        [Route("DoesExist")]
        public bool DoesExist(string groupName, long groupID)
        {
            return DomainModel.Group.DoesExist(groupName, groupID);
        }

        [HttpGet]
        [Route("GetAllGroups")]
        public Dictionary<long, string> GetAllGroups(string searchString)
        {
            return DomainModel.Group.GetAllGroups(searchString);
        }

        [HttpGet]
        [Route("GetAllGroupsByEndpointID")]
        public List<Group> GetAllGroupsByEndpointID(long endpointSystemTypeId, long endPointID, string searchString)
        {
            return DtoMapList(DomainModel.Group.GetAllGroupsByEndpointID(endpointSystemTypeId, endPointID, searchString));
        }

        [HttpGet]
        [Route("GetByIdIncludingUser")]
        public Group GetByIdIncludingUser(long id)
        {
            return DtoMap(DomainModel.Group.GetByIdIncludingUser(id));
        }

        [HttpGet]
        [Route("DeleteGroupByID")]
        public string DeleteGroupByID(long id)
        {
            return DomainModel.Group.DeleteGroupById(id);
        }


        [HttpGet]
        [Route("CheckUserGroup")]
        public bool CheckUserGroup(long userId, long GroupID)
        {
            var group = DomainModel.Group.GetByIdIncludingUsers(GroupID);
            return group != null && group.Users != null && group.Users.Any(u => u.Id == userId);
        }

        //[HttpGet]
        //[Route("GetByIdIncludingOwner")]
        //public Group GetByIdIncludingOwner(long id)
        //{
        //    return DtoMap(DomainModel.Group.GetByIdIncludingOwner(id));
        //}

        [HttpGet]
        [Route("GetGroupsByUserId")]
        public Dictionary<long, string> GetGroupsByUserId(long userId)
        {
            return DomainModel.Group.GetGroupsByUserId(userId);
        }
    }
}
