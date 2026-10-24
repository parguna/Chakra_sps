using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class InterimStore : DomainModelBase<InterimStore, EntityModel.InterimStore>
    {
        public InterimStoreItemType InterimStoreItemType { get; set; }

        public long? TaskId { get; set; }

        public long? UserId { get; set; }

        public string XmlData { get; set; }

        public bool IsSaved { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static InterimStore GetByTaskId(long taskId, bool isSaved, InterimStoreItemType itemType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InterimStore>(dataAccessTransaction).Get(Is => Is.TaskId == taskId && Is.IsSaved == isSaved && Is.InterimStoreItemType == itemType).FirstOrDefault());
            }
        }

        public static IList<InterimStore> GetByUserId(long userId, bool isSaved, InterimStoreItemType itemType)
        {
            IList<InterimStore> interim = new List<InterimStore>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                if (isSaved)
                    interim = ORMapList(new RepositoryBase<EntityModel.InterimStore>(dataAccessTransaction).Get(Is => Is.UserId == userId && Is.IsSaved == isSaved && Is.InterimStoreItemType == itemType).ToList());
                else
                    interim = ORMapList(new RepositoryBase<EntityModel.InterimStore>(dataAccessTransaction).Get(Is => Is.UserId == userId && Is.IsSaved == false && Is.InterimStoreItemType == itemType).ToList());
            }
            return interim;
        }

        public static IList<InterimStore> GetForSpecialAccounts(bool isSaved, InterimStoreItemType itemType)
        {
            IList<InterimStore> interim = new List<InterimStore>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                interim = ORMapList(new RepositoryBase<EntityModel.InterimStore>(dataAccessTransaction).Get(Is => Is.IsSaved == isSaved && Is.InterimStoreItemType == itemType).ToList());
            }
            return interim;
        }

        public static IList<InterimStore> GetFromInterimStoreByUserId(long userId, bool isSaved)
        {
            IList<InterimStore> interim = new List<InterimStore>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                interim = ORMapList(new InterimStoreRepository(dataAccessTransaction).GetFromInterimStoreByUserId(userId, isSaved));
            }
            return interim;
        }

        public static IList<InterimStore> GetByType(InterimStoreItemType itemType)
        {
            IList<InterimStore> interim = new List<InterimStore>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                interim = ORMapList(new RepositoryBase<EntityModel.InterimStore>(dataAccessTransaction).Get(Is => Is.IsSaved == true && Is.InterimStoreItemType == itemType).ToList());
            }
            return interim;
        }

        public static IList<InterimStore> GetAttachmentByIds(List<long> ids)
        {
            IList<InterimStore> interim = new List<InterimStore>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                interim = ORMapList(new RepositoryBase<EntityModel.InterimStore>(dataAccessTransaction).Get(Is => Is.InterimStoreItemType == InterimStoreItemType.Attachment && Is.TaskId != null && ids.Contains(Is.TaskId.Value)).ToList());
            }
            return interim;
        }

        public static void DeleteByUserId(long userId, InterimStoreItemType itemType)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                List<InterimStore> dbInterimStore = InterimStore.GetByUserIdAndItemType(userId, itemType);
                if (itemType == InterimStoreItemType.AccessRequest)
                {
                    List<InterimStore> dbInterimAttachment = InterimStore.GetByUserIdAndItemType(userId, InterimStoreItemType.Attachment);
                    InterimStore.DeleteList(dbInterimAttachment);
                }
                InterimStore.DeleteList(dbInterimStore);
            }
        }
         
        public static void DeleteByInterimId(List<long> interimIds, InterimStoreItemType itemType)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                foreach (long interimId in interimIds)
                {
                    List<InterimStore> dbInterimStore = InterimStore.GetByInterimIdAndItemType(interimId, itemType);
                    if (itemType == InterimStoreItemType.AccessRequest)
                    {
                        List<InterimStore> dbInterimAttachment = InterimStore.GetByInterimIdAndItemType(interimId, InterimStoreItemType.Attachment);
                        InterimStore.DeleteList(dbInterimAttachment);
                    }
                    InterimStore.DeleteList(dbInterimStore);
                }
       
                
            }
        }

        
        public static void DeleteByUserId(long userId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                List<InterimStore> dbInterimStore = InterimStore.GetAll(true).Where(Is => Is.UserId == userId && Is.InterimStoreItemType == InterimStoreItemType.Task).ToList();
                InterimStore.DeleteList(dbInterimStore);
            }
        }
        public static List<InterimStore> GetByUserIdAndItemType(long userId, InterimStoreItemType itemType)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.InterimStore>(transaction).Get(Is => Is.UserId == userId && Is.InterimStoreItemType == itemType).ToList());
            }
        }

        public static List<InterimStore> GetByInterimIdAndItemType(long interimId, InterimStoreItemType itemType)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.InterimStore>(transaction).Get(Is => Is.Id == interimId && Is.InterimStoreItemType == itemType).ToList());
            }
        }

        public static IList<InterimStore> GetInterimStoresByTaskId(long taskId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.InterimStore>(dataAccessTransaction)
                    .Get(Is => Is.TaskId == taskId).ToList());
            }
        }

        public static List<InterimStore> GetInterimStoreDays(int days)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMapList(new InterimStoreRepository(transaction).GetInterimStoreDays(days).ToList());
            }
        }
        public static void DeleteInterimStoreByTaskId(long taskId)
        {

            //using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                var interimStores = GetInterimStoresByTaskId(taskId);
                foreach (InterimStore interimStore in interimStores)
                {
                    interimStore.Delete();
                }
            }
        }

        public static bool DoesPendingRequestExists(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.InterimStore>(dataAccessTransaction).Any(item => item.UserId == userId && item.IsSaved && item.InterimStoreItemType == InterimStoreItemType.AccessRequest);
            }
        }

        public static void AddToInterimStore(Task task, string currentUser)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                InterimStore interimStore = new InterimStore()
                {
                    InterimStoreItemType = InterimStoreItemType.Task,
                    IsActive = true,
                    IsSaved = true,
                    TaskId = task.Id,
                    XmlData = UtilityMethods.Serialize<Task>(task),
                };
                interimStore.Save(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }
    }
}
