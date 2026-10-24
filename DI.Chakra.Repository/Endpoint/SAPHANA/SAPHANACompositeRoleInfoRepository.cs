using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SAPHANACompositeRoleInfoRepository : RepositoryBase<SAPCompositeRoleInfo>
    {
        public SAPHANACompositeRoleInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<string> GetChildRolesByCompositeRoleCode(List<string> compositeRolesList)
        {
            if (!(compositeRolesList != null && compositeRolesList.Count > 0))
                return null;

            //string queryFormatter = "SELECT DISTINCT CHILD_ROLE_CODE FROM DI_SAP_COMPOSITE_CHILD_ROLEMAP_INFO WHERE COMPOSITE_ROLE_CODE IN ({0})";
            List<string> sapChildRoles = new List<string>();
            
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    IList<SAPHANACompositeRoleInfo> SAPCompositeRoleInfoList = new List<SAPHANACompositeRoleInfo>();
                    SAPCompositeRoleInfoList = CurrentContext.SAPHANACompositeRoleInfo.Where(roleInfo => compositeRolesList.Contains(roleInfo.CompositeRole_Code)).ToList();
                    foreach (SAPHANACompositeRoleInfo listItem in SAPCompositeRoleInfoList)
                    {
                        IList<SAPHANASingleRoleInfo> SAPsingleRoleInfoList = listItem.SAPSingleRoleInfos.ToList();
                        foreach (SAPHANASingleRoleInfo listSingleRoleItem in SAPsingleRoleInfoList)
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
