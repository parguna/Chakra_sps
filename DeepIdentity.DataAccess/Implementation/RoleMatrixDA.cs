using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using DeepIdentity.Model.SAP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepIdentity.DataAccess
{
    public class RoleMatrixDA
    {

        public Dictionary<int, string> GetSAPRoleCatalog(int EndpointUserId)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                List<string> SapRoles = (from user in _context.SAPUserinfos
                                         where user.UserId == EndpointUserId
                                         from role in user.SAPRoleInfo
                                         select role.ROLE_CODE).ToList();


                return (from businessRole in _context.SAPRoleCatalogs
                        join tRoles in SapRoles on businessRole.TechnicalRole equals tRoles
                        select businessRole)
                       .Select(role => new { key = role.Id, value = role.BusinessRoleName }).OrderBy(item => item.value)
                       .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
            }
        }

    }

}
