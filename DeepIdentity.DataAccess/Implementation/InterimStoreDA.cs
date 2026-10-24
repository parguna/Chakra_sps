using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace DeepIdentity.DataAccess
{
    public class InterimStoreDA
    {

        public void AddToInterimStore(InterimStore interimStore)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.InterimStores.Add(interimStore);
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(interimStore);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<InterimStore>();
                        InterimStore attachedEntity = set.Find(interimStore.InterimStoreId);

                        if (attachedEntity != null)
                        {
                            var attachedEntry = _context.Entry(attachedEntity);
                            attachedEntry.CurrentValues.SetValues(interimStore);
                        }
                    }
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var interimStores = _context.InterimStores.Where(item => item.TaskId == taskId);
                    foreach (InterimStore interimStore in interimStores)
                    {
                        _context.InterimStores.Remove(interimStore);
                    }
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (isSaved)
                    {
                        return _context.InterimStores.FirstOrDefault(item => item.TaskId == taskId && item.IsSaved && item.InterimStoreItemType == itemType);
                    }
                    return _context.InterimStores.FirstOrDefault(item => item.TaskId == taskId && item.IsSaved == false && item.InterimStoreItemType == itemType);
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<InterimStore> GetFromInterimStoreByUserId(int userId, bool isSaved, InterimStoreItemType itemType)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    IList<InterimStore> interim = new List<InterimStore>();
                    if (isSaved)
                    {
                        interim = _context.InterimStores.Where(item => item.UserId == userId && item.IsSaved && item.InterimStoreItemType == itemType).ToList();
                    }
                    else
                    {
                        interim = _context.InterimStores.Where(item => item.UserId == userId && item.IsSaved == false && item.InterimStoreItemType == itemType).ToList();
                    }
                    return interim;
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var interimStores = _context.InterimStores.Where(item => item.UserId == userId && item.InterimStoreItemType == itemType);
                    foreach (InterimStore interimStore in interimStores)
                    {
                        _context.InterimStores.Remove(interimStore);
                    }
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var interimStores = _context.InterimStores.Where(item => item.InterimStoreId == interimStoreId);
                    foreach (InterimStore interimStore in interimStores)
                    {
                        _context.InterimStores.Remove(interimStore);
                    }
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public InterimStore GetFromInterimStoreByInterimStoreID(long interimstoreid)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.InterimStores.FirstOrDefault(item => item.InterimStoreId == interimstoreid);
                }
            }
            catch
            {
                throw;
            }
        }

        public bool ExistsPendingRequests(int userId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    IList<InterimStore> interim = new List<InterimStore>();
                    int CountofReq = 0;
                    CountofReq = _context.InterimStores.Count(item => item.UserId == userId && item.IsSaved && item.InterimStoreItemType == InterimStoreItemType.AccessRequest);
                    return (CountofReq > 0) ? true : false;
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
