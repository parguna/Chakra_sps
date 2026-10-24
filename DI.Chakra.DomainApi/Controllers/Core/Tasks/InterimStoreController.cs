using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("InterimStore")]
    public class InterimStoreController : BaseController<DomainModel.InterimStore, EntityModel.InterimStore, Dto.InterimStore>
    {
        [HttpGet]
        [Route("GetByUserId")]
        public IList<InterimStore> GetByUserId(long userId, bool isSaved, InterimStoreItemType itemType) 
        {
            return DtoMapList(DomainModel.InterimStore.GetByUserId(userId, isSaved, itemType));
        }

        [HttpGet]
        [Route("GetForSpecialAccounts")]
        public IList<InterimStore> GetForSpecialAccounts(bool isSaved, InterimStoreItemType itemType)
        {
            return DtoMapList(DomainModel.InterimStore.GetForSpecialAccounts(isSaved, itemType));
        }

        [HttpGet]
        [Route("GetFromInterimStoreByUserId")]
        public IList<InterimStore> GetFromInterimStoreByUserId(long userId, bool isSaved)
        {
            return DtoMapList(DomainModel.InterimStore.GetFromInterimStoreByUserId(userId, isSaved));
        }


        [HttpGet]
        [Route("GetByType")]
        public IList<InterimStore> GetByType(InterimStoreItemType itemType)
        {
            return DtoMapList(DomainModel.InterimStore.GetByType(itemType));
        }

        [HttpGet]
        [Route("GetByTaskId")]
        public InterimStore GetByTaskId(long taskId, bool isSaved, InterimStoreItemType itemType)
        {
            return DtoMap(DomainModel.InterimStore.GetByTaskId(taskId, isSaved, itemType));
        }

        [HttpGet]
        [Route("GetByUserIdAndItemType")]
        public IList<InterimStore> GetByUserIdAndItemType([FromUri]long userId,[FromBody] InterimStoreItemType itemType)
        {
            return DtoMapList(DomainModel.InterimStore.GetByUserIdAndItemType(userId, itemType));
        }

        [HttpGet]
        [Route("DeleteByUserId")]
        public void DeleteByUserId( long userId,InterimStoreItemType itemType)
        {
            DomainModel.InterimStore.DeleteByUserId(userId, itemType);
        }
        [HttpGet]
        [Route("DeleteByUserId")]
        public void DeleteByUserId(long userId)
        {
            DomainModel.InterimStore.DeleteByUserId(userId);
        }
        
        [HttpPost]
        [Route("DeleteByInterimId")]
        public void DeleteByInterimId(List<long> interimId, InterimStoreItemType itemType)
        {
            DomainModel.InterimStore.DeleteByInterimId(interimId , itemType);
        }
    }
}