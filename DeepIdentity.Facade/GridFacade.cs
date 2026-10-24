using DeepIdentity.DataAccess.Implementation;
using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIdentity.Facade
{
    public class GridFacade
    {
        public GridDA gridDA { get; set; }
        public GridFacade()
        {
            gridDA = new GridDA();
        }
        public UserGridInfo GetUserGridInfo(int userId, string GridId, string PagePath)
        {
            try
            {
                return gridDA.GetUserGridInfo(userId, GridId, PagePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserFilterInfo> GetUserFilterInfo(int userId, string GridId, string PagePath)
        {
            try
            {
                return gridDA.GetUserFilterInfo(userId, GridId, PagePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SaveGridSortAndPageInformation(UserGridInfo info)
        {
            try
            {
                gridDA.SaveGridSortAndPageInformation(info);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SaveGridFilterInformation(UserFilterInfo info)
        {
            try
            {
                gridDA.SaveGridFilterInformation(info);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DeleteGridFilterInformation(UserFilterInfo info)
        {
            try
            {
                gridDA.DeleteGridFilterInformation(info);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
