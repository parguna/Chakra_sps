using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class InterimStoreRepository : RepositoryBase<InterimStore>
    {
        public InterimStoreRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<InterimStore> GetInterimStoreDays(int days)
        {
            List<InterimStore> resultinterimList = new List<InterimStore>();
            List<InterimStore> interimList = CurrentContext.InterimStores.Where(x=>x.InterimStoreItemType == InterimStoreItemType.AccessRequest).ToList();

            foreach (InterimStore intrm in interimList)
            {
                if (DateTime.Now.Subtract(intrm.CreatedDate).Days >= days)
                {
                    resultinterimList.Add(intrm);
                }
            }
            return resultinterimList;
        }

        public List<PresentationInterimStore> GetInterimStoreAccessRequestForUser(long userId, bool isSaved, InterimStoreItemType itemType)
        {
            List<PresentationInterimStore> resultinterimList = new List<PresentationInterimStore>();

            resultinterimList = (from interim in CurrentContext.InterimStores
                                 join attachment in CurrentContext.InterimStores on interim.Id equals attachment.TaskId into HasAttachment
                                 from attachment in HasAttachment.DefaultIfEmpty()
                                 join createdUser in CurrentContext.User on interim.CreatedBy equals createdUser.UserName
                                 
                                 where interim.UserId == userId && interim.IsSaved == isSaved && interim.InterimStoreItemType == itemType
                                 select new PresentationInterimStore
                                 {
                                     Id = interim.Id,
                                     InterimStoreItemType = interim.InterimStoreItemType,
                                     TaskId = interim.TaskId,
                                     UserId = interim.UserId,
                                     XmlData = interim.XmlData,
                                     IsSaved = interim.IsSaved,
                                     IsActive = interim.IsActive,
                                     CreatedBy = createdUser.DisplayName,
                                     CreatedDate = interim.CreatedDate,
                                     LastModifiedBy = interim.LastModifiedBy,
                                     LastModifiedDate = interim.LastModifiedDate,
                                     HasAttachment = attachment == null ? false : true,
                                     AttachmentFileName = "",
                                 }).ToList();

            return resultinterimList;
        }


        public IList<InterimStore> GetFromInterimStoreByUserId(long userId, bool isSaved)
        {
            try
            {
                 IList<InterimStore> interim = new List<InterimStore>();
                    if (isSaved)
                    {
                        interim = CurrentContext.InterimStores.Where(item => (userId != 0 ? item.UserId == userId : true) && item.TaskId == null && item.IsSaved == true).ToList();
                    }
                    else
                    {
                        interim = CurrentContext.InterimStores.Where(item => (userId != 0 ? item.UserId == userId : true) && item.TaskId == null && item.IsSaved == false).ToList();
                    }
                    return interim; 
            }
            catch
            {
                throw;
            }
        }
    }
}
