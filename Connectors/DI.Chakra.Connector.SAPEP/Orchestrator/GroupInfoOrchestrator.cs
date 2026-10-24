using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using SapCEWebservice;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Connector.SAPEP
{
    public class GroupInfoOrchestrator
    {
        public long EndpointSystemId { get; set; }

        public int HistoryId { get; set; }
        private IdmsLogger _logger = null;

        public GroupInfoOrchestrator(IdmsLogger logger)
        {
            _logger = logger;
        }

        public List<SAPEPGroupInfo> GetGroups(ZZPrincipalInformation[] zPrincipalList) // IList<SAPEPGroupInfoHistory> 
        {
            List<SAPEPGroupInfo> groups = new List<SAPEPGroupInfo>();

            SAPEPGroupInfo obj = null;
            try
            {
                _logger.WriteLog(LogLevelType.Info, "Started discovering Groups");
                foreach (ZZPrincipalInformation zp in zPrincipalList)
                {
                    obj = new SAPEPGroupInfo();

                    obj.EndPointSystemId = EndpointSystemId;
                    obj.GroupId = zp.strPrincipalID;
                    obj.GroupName = zp.strPrincipalName;
                    obj.GroupDescription = zp.strPrincipalDesc;
                    obj.GroupUniqueName = zp.strPrincipalUniqueName;

                    //obj.GroupUniqueName = string.IsNullOrWhiteSpace(zp.strPrincipalUniqueName) ? obj.GroupName : zp.strPrincipalUniqueName;
                    //obj.IsXLUploaded = true; // xl uploaded feature no longer required

                    obj.Version = Convert.ToInt16(VersionNo.Available);
                    if (!groups.Contains(obj))
                        groups.Add(obj);
                }
                _logger.WriteLog(LogLevelType.Info, "Completed discovering Groups");
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Exception, "Exception occured during GroupInfoOrchestrator:GetGroups", ex);
                throw ex;
            }

            return groups;
        }

        public IList<SAPEPGroupInfo> GetUserGroups(string userName, ZZPrincipalInformation[] zPrincipalList) // IList<SAPEPUserGroupHistory> 
        {
            IList<SAPEPGroupInfo> list = new List<SAPEPGroupInfo>();

            try
            {
                SAPEPGroupInfo usergroup = null;

                foreach (ZZPrincipalInformation zp in zPrincipalList)
                {
                    usergroup = new SAPEPGroupInfo();
                    usergroup.GroupName = userName;
                    usergroup.GroupId = zp.strPrincipalName;
                    usergroup.EndPointSystemId = EndpointSystemId;
                    list.Add(usergroup);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Exception, "Exception occured during GroupInfoOrchestrator:GetUserGroups", ex);
                throw;
            }

            return list;
        }
    }
}

