using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace DeepIdentity.Facade
{

    public class InterimStoreFacade
    {
        InterimStoreDA _interimStoreDA;

        public InterimStoreFacade()
        {
            _interimStoreDA = new InterimStoreDA();
        }

        public void AddToInterimStore(InterimStore interimStore)
        {
            try
            {
                _interimStoreDA.AddToInterimStore(interimStore);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateInterimStore(InterimStore interimStore)
        {
            try
            {
                _interimStoreDA.UpdateInterimStore(interimStore);
            }
            catch
            {
                throw;
            }
        }

        public void DeleteInterimStoreByTaskId(long taskId)
        {
            try
            {
                _interimStoreDA.DeleteInterimStoreByTaskId(taskId);
            }
            catch
            {
                throw;
            }
        }

        public InterimStore GetFromInterimStoreByTaskId(long taskId, bool isSaved, InterimStoreItemType itemType)
        {
            try
            {
                return _interimStoreDA.GetFromInterimStoreByTaskId(taskId, isSaved, itemType);
            }
            catch
            {
                throw;
            }
        }

        public IList<InterimStore> GetFromInterimStoreByUserId(int userid, bool isSaved, InterimStoreItemType itemType)
        {
            try
            {
                return _interimStoreDA.GetFromInterimStoreByUserId(userid, isSaved, itemType);
            }
            catch
            {
                throw;
            }

        }

        public void DeleteInterimStoreByUserId(int userId, InterimStoreItemType itemType)
        {
            try
            {
                _interimStoreDA.DeleteInterimStoreByUserId(userId, itemType);
            }
            catch
            {
                throw;
            }
        }

        public InterimStore GetFromInterimStoreByInterimStoreId(long interimstoreid)
        {
            try
            {
                return _interimStoreDA.GetFromInterimStoreByInterimStoreID(interimstoreid);
            }
            catch
            {
                throw;
            }
        }

        public void DeleteInterimStoreById(long interimStoreId)
        {
            try
            {
                _interimStoreDA.DeleteInterimStoreById(interimStoreId);
            }
            catch
            {
                throw;
            }
        }


        public bool ExistsPendingRequests(int UserId)
        {
            try
            {
                return _interimStoreDA.ExistsPendingRequests(UserId);
            }
            catch
            {
                throw;
            }
        }
    }
}