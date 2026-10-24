using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DataAccess
{
    public class UserQuickLinkDA
    {
        public IList<QuickLink> GetCategoryAndName(long RoleId)
        {
            IList<QuickLink> menult = new List<QuickLink>();
            try
            {
                if (RoleId != -1)
                {
                    using (IACMDBContext oIACMDBContext = new IACMDBContext())
                    {
                        var query = from QR in oIACMDBContext.QuickLinkAccessRights
                                    join r in oIACMDBContext.Roles
                                    on QR.RoleId equals r.Id
                                    join Q in oIACMDBContext.QuickLinks
                                    on QR.QuickLinkId equals Q.Id
                                    where r.Id == RoleId
                                    select Q;
                        menult = query.ToList();
                    }
                }
                return menult;
            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Geting GetQuickLinkByUserRole");
                throw;
            }
        }

        public QuickLink GetUserLinks(int menuId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    QuickLink link = oIACMDBContext.QuickLinks.Include("DefaultRoleQuickLink").Where(QU => QU.QuickLinkId == menuId).FirstOrDefault();
                    return link;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Geting GetUserLinks");
                throw;
            }
        }

        public QuickLink Getlinkid(string id)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    QuickLink link = oIACMDBContext.QuickLinks.Where(QU=>QU.Name==id).FirstOrDefault();
                    return link;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Geting GetLinks");
                throw;
            }
        }

        public IList<UserQuickLink> SaveLinks(UserQuickLink selectedmenu)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                oIACMDBContext.UserQuickLinks.Attach(selectedmenu);
                oIACMDBContext.Entry(selectedmenu).State = EntityState.Added;
                oIACMDBContext.SaveChanges();
            }
            return null;
        }
    }
}

 
