using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using SapEpOldWebservice;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Connector.SAPIP
{
    public class RoleInfoOrchestrator
    {
        public long EndpointSystemId { get; set; }
        public string TenantId { get; set; }
        public int HistoryId { get; set; }

        public IList<SAPEPRoleInfo> GetRoles(ZZPrincipalInformation[] zPrincipalList) // IList<SAPEPRoleInfoHistory> 
        {
            IList<SAPEPRoleInfo> roles = new List<SAPEPRoleInfo>();

            SAPEPRoleInfo obj = null;
            try
            {
                foreach (ZZPrincipalInformation zp in zPrincipalList)
                {
                    obj = new SAPEPRoleInfo();


                    obj.TenantId = TenantId;
                    //obj.HistoryId = HistoryId;
                    obj.EndPointSystemId = EndpointSystemId;
                    obj.RoleId = zp.strPrincipalID;
                    obj.RoleName = zp.strPrincipalName;
                    obj.RoleDescription = zp.strPrincipalDesc;
                    obj.UniqueRoleName = string.Empty;
                    obj.CreatedBy = Constants.CreatedBy;
                    obj.CreatedDate = DateTime.Now;
                    obj.LastModifiedBy = "System";
                    obj.LastModifiedDate = DateTime.Now;

                    if (!roles.Contains(obj))
                        roles.Add(obj);
                }
            }
            catch (Exception)
            {
                
                throw;
            }

            return roles;
        }

        public IList<SAPEPUserRole> GetUserRoles(string userName, ZZPrincipalInformation[] zPrincipalList) // IList<SAPEPUserRoleHistory> 
        {
            IList<SAPEPUserRole> list = new List<SAPEPUserRole>();

            try
            {
                SAPEPUserRole userrole = null;

                foreach (ZZPrincipalInformation zp in zPrincipalList)
                {
                    userrole = new SAPEPUserRole();
                    userrole.UserName = userName;
                    userrole.RoleId = zp.strPrincipalID;
                    userrole.TenantId = TenantId;
                    userrole.EndPointSystemId = EndpointSystemId;
                    userrole.CreatedBy = Constants.CreatedBy;
                    userrole.CreatedDate = DateTime.Now;
                    userrole.LastModifiedBy = "System";
                    userrole.LastModifiedDate = DateTime.Now;

                    list.Add(userrole);
                }
            }
            catch (Exception)
            {
                
                throw;
            }

            return list;
        }
    }
}
