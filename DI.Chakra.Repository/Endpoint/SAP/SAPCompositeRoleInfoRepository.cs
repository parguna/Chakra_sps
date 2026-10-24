using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SAPCompositeRoleInfoRepository : RepositoryBase<SAPCompositeRoleInfo>
    {
        public SAPCompositeRoleInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<string> GetChildRolesByCompositeRoleCode(List<string> compositeRolesList)
        {
            if (!(compositeRolesList != null && compositeRolesList.Count > 0))
                return null;

            //string queryFormatter = "SELECT DISTINCT CHILD_ROLE_CODE FROM DI_SAP_COMPOSITE_CHILD_ROLEMAP_INFO WHERE COMPOSITE_ROLE_CODE IN ({0})";
            List<string> sapChildRoles = new List<string>();
            
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    IList<SAPCompositeRoleInfo> SAPCompositeRoleInfoList = new List<SAPCompositeRoleInfo>();
                    SAPCompositeRoleInfoList = CurrentContext.SAPCompositeRoleInfo.Where(roleInfo => compositeRolesList.Contains(roleInfo.CompositeRole_Code)).ToList();
                    foreach (SAPCompositeRoleInfo listItem in SAPCompositeRoleInfoList)
                    {
                        IList<SAPSingleRoleInfo> SAPsingleRoleInfoList = listItem.SAPSingleRoleInfos.ToList();
                        foreach (SAPSingleRoleInfo listSingleRoleItem in SAPsingleRoleInfoList)
                        {
                            if (!string.IsNullOrEmpty(listSingleRoleItem.SingleRole_Code))
                            {
                                if (sapChildRoles.Contains(listSingleRoleItem.SingleRole_Code) == false)
                                    sapChildRoles.Add(listSingleRoleItem.SingleRole_Code);
                            }
                        }
                    }
                }      
            return sapChildRoles;
        }
    }
}
