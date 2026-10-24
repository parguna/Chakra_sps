using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using DI.Chakra.Infrastructure.Enums;
using SapEpOldWebservice;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Connector.SAPIP
{
    public class GroupInfoOrchestrator
    {
        public long EndpointSystemId { get; set; }
        public string TenantId { get; set; }
        public int HistoryId { get; set; }

        public IList<SAPEPGroupInfo> GetGroups(ZZPrincipalInformation[] zPrincipalList) // IList<SAPEPGroupInfoHistory> 
        {
            IList<SAPEPGroupInfo> groups = new List<SAPEPGroupInfo>();

            SAPEPGroupInfo obj = null;
            try
            {
                foreach (ZZPrincipalInformation zp in zPrincipalList)
                {
                    obj = new SAPEPGroupInfo();


                    obj.TenantId = TenantId;
                    //obj.HistoryId = HistoryId;
                    obj.EndPointSystemId = EndpointSystemId;
                    obj.GroupId = zp.strPrincipalID;
                    obj.GroupName = zp.strPrincipalName;
                    obj.GroupDescription = zp.strPrincipalDesc;
                    obj.GroupUniqueName = string.Empty;
                    obj.CreatedBy = Constants.CreatedBy;
                    obj.CreatedDate = DateTime.Now;
                    obj.LastModifiedBy = "System";
                    obj.LastModifiedDate = DateTime.Now;
                    obj.Version = Convert.ToInt16(VersionNo.Available);
                    if (!groups.Contains(obj))
                        groups.Add(obj);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return groups;
        }

        public IList<SAPEPUserGroup> GetUserGroups(string userName, ZZPrincipalInformation[] zPrincipalList) // IList<SAPEPUserGroupHistory> 
        {
            IList<SAPEPUserGroup> list = new List<SAPEPUserGroup>();

            try
            {
                SAPEPUserGroup usergroup = null;

                foreach (ZZPrincipalInformation zp in zPrincipalList)
                {
                    usergroup = new SAPEPUserGroup();
                    usergroup.UserName = userName;
                    usergroup.GroupId = zp.strPrincipalName;
                    usergroup.TenantId = TenantId;
                    usergroup.EndPointSystemId = EndpointSystemId;
                    usergroup.CreatedBy = Constants.CreatedBy;
                    usergroup.CreatedDate = DateTime.Now;
                    usergroup.LastModifiedBy = "System";
                    usergroup.LastModifiedDate = DateTime.Now;

                    list.Add(usergroup);
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

