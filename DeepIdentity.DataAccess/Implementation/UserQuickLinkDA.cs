using DeepIdentity.Infrastructure;
using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIdentity.DataAccess
{
    public class UserQuickLinkDA
    {
        public IList<QuickLink> GetCategoryAndName(int RoleId)
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
                                    on QR.RoleId equals r.RoleId
                                    join Q in oIACMDBContext.QuickLinks
                                    on QR.QuickLinkId equals Q.Id
                                    where r.RoleId==RoleId
                                    select Q;
                        menult = query.ToList();
                    }
                }
                return menult;
            }

            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Geting GetQuickLinkByUserRole");
                throw;
            }
        }
    }
}

 
