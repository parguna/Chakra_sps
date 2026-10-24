using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SAPEPRoleInfoTempRepository:RepositoryBase<SAPEPRoleInfoTemp>
    {

        public SAPEPRoleInfoTempRepository(DataAccessTransaction transaction) : base(transaction) { }


        public Dictionary<string, string> GetSAPEPRoleUniqueNamesDictionary(long endPointId)
        {
            Dictionary<string, string> roleDict = new Dictionary<string, string>();
            //string query = "DI_spGetGetSAPEPRoleUniqueNames";
            string tmpRoleId;
            string tmpRoleFullName;

            IList<SAPEPRoleInfoTemp> SAPEPRoleInfoTempList = new List<SAPEPRoleInfoTemp>();
            SAPEPRoleInfoTempList = CurrentContext.SAPEPRoleInfoTemps.ToList();

            foreach (SAPEPRoleInfoTemp listItem in SAPEPRoleInfoTempList)
            {
                tmpRoleId = (DBNull.Value.Equals(listItem.RoleUniqueName)) ? string.Empty : (listItem.RoleUniqueName == null ? string.Empty : listItem.RoleUniqueName);
                tmpRoleFullName = (DBNull.Value.Equals(listItem.RoleFullName)) ? string.Empty : (listItem.RoleFullName == null ? string.Empty : listItem.RoleFullName);
                if (!string.IsNullOrEmpty(tmpRoleId) && !roleDict.ContainsKey(tmpRoleId))
                {
                    roleDict.Add(tmpRoleId, tmpRoleFullName);
                }
            }

            return roleDict;
        }
    }
}
