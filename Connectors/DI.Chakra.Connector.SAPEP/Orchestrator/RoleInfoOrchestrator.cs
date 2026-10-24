using DI.Chakra.DomainModel;
using DI.Chakra.Logger;
using SapCEWebservice;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Connector.SAPEP
{
    public class RoleInfoOrchestrator
    {
        public long EndpointSystemId { get; set; }
        
        public int HistoryId { get; set; }
        private IdmsLogger _logger = null;
        public RoleInfoOrchestrator(IdmsLogger logger)
        {
            _logger = logger;
        }

        public List<SAPEPRoleInfo> GetRoles(ZZPrincipalInformation[] zPrincipalList) // IList<SAPEPRoleInfoHistory> 
        {
            List<SAPEPRoleInfo> roles = new List<SAPEPRoleInfo>();

            SAPEPRoleInfo obj = null;
            try
            {
                _logger.WriteLog(LogLevelType.Info, "Started discovering Roles");
                foreach (ZZPrincipalInformation zp in zPrincipalList)
                {
                    obj = new SAPEPRoleInfo();

                    obj.EndPointSystemId = EndpointSystemId;
                    obj.RoleId = zp.strPrincipalID;
                    obj.RoleName = zp.strPrincipalName;
                    obj.RoleDescription = zp.strPrincipalDesc;
                    obj.UniqueRoleName = zp.strPrincipalUniqueName;
                    obj.CreatedBy = "System";
                    obj.CreatedDate = DateTime.Now;
                    obj.LastModifiedBy = "System";
                    obj.LastModifiedDate = DateTime.Now;

                    obj.CanRequest = true;

                    if (!roles.Contains(obj))
                        roles.Add(obj);
                }
                _logger.WriteLog(LogLevelType.Info, "Completed discovering Roles");

            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Exception, "Exception occured during RoleInfoOrchestrator:GetRoles", ex);
                throw ex;
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
                    userrole.EndPointSystemId = EndpointSystemId;
                    
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
