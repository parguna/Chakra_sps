using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DataAccess.Implementation
{
    public class GridDA
    {
        public UserGridInfo GetUserGridInfo(int userId,string GridId,string PagePath)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    UserGridInfo objGridInfo = new UserGridInfo();
                    objGridInfo = oIACMDBContext.UserGridInfos.Where(m => m.UserId == userId && m.PagePath == PagePath && m.GridId == GridId).FirstOrDefault();
                    return objGridInfo;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<UserFilterInfo> GetUserFilterInfo(int userId, string GridId, string PagePath)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.UserFilterInfos.Where(m => m.UserId == userId && m.PagePath == PagePath && m.GridId == GridId).ToList();

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void SaveGridSortAndPageInformation(UserGridInfo info)
        {
            try
            {
                DeleteGridSortAndPageInformation(info);
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    oIACMDBContext.UserGridInfos.Attach(info);
                    oIACMDBContext.Entry(info).State = EntityState.Added;
                    oIACMDBContext.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void SaveGridFilterInformation(UserFilterInfo info)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    oIACMDBContext.UserFilterInfos.Attach(info);
                    oIACMDBContext.Entry(info).State = EntityState.Added;
                    oIACMDBContext.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void DeleteGridSortAndPageInformation(UserGridInfo info)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    UserGridInfo userGrid = oIACMDBContext.UserGridInfos.Where(m => m.UserId == info.UserId && m.PagePath == info.PagePath && m.GridId == info.GridId).FirstOrDefault();
                    if (userGrid != null)
                    {
                        oIACMDBContext.UserGridInfos.Remove(userGrid);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void DeleteGridFilterInformation(UserFilterInfo info)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    UserFilterInfo userFilter = oIACMDBContext.UserFilterInfos.Where(m => m.UserId == info.UserId && m.PagePath == info.PagePath && m.GridId == info.GridId && m.ColumnName == info.ColumnName).FirstOrDefault();
                    if (userFilter != null)
                    {
                        oIACMDBContext.UserFilterInfos.Remove(userFilter);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
