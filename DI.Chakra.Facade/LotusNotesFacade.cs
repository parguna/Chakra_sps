using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using DI.Chakra.Infrastructure.;
using DI.Chakra.Model;
using DI.Chakra.Infrastructure;
using System.ComponentModel;
using System.Reflection;
using System.Collections;

using System.Data;
using DI.Chakra.Model.LotusNotes;
using DI.Chakra.DataAccess;
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Facade
{
    public class LotusNotesFacade : IEndpointFacade
    {
        private LotusNotesDA LotusNotesDA { get; set; }
        private EndpointObjectDA EndpointObjectDA = null;
        private ReconcilationFacade reconcilationFacade = new ReconcilationFacade();
     
        public LotusNotesFacade()
        {
            LotusNotesDA = new LotusNotesDA();
            EndpointObjectDA = new EndpointObjectDA();
        }
        public IEndpointUserInfo GetEndpointUserInfo(long EndpointUserId)
        {
            return GetLNSUserInfo(EndpointUserId);
        }

      

        public void AddLotusUserInfo(IList<LotusUserInfo> list)
        {
            try
            {
                LotusNotesDA.AddLotusUserInfoBulk(list);

            }
            catch (Exception)
            {

                throw;
            }
        }
        public long AddLotusUserInfo(LotusUserInfo _lnsUser)
        {
            try
            {
                return LotusNotesDA.AddLNSUser(_lnsUser);

            }
            catch (Exception)
            {

                throw;
            }
        }



        public bool PopulateUserIntoDB
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void AddLotusServerMailUsers(IList<LotusServerMailUsersInfo> list)
        {
            try
            {


            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddLotusProfileInfo(IList<LotusProfileInfo> list)
        {
            try
            {


            }
            catch (Exception)
            {

                throw;
            }
        }


        public void AddLotusUserInfo(IList<LotusUserInfo> list, EndPoint _endpoint)
        {
            try
            {

                LotusNotesDA.AddLotusUserInfoBulk(list);

            }
            catch (Exception)
            {

                throw;
            }
        }
        public void AddLotusGroupInfo(IList<LotusGroupInfo> list)
        {
            try
            {

                LotusNotesDA.AddLNSGroupInfoBulk(list);
            }
            catch (Exception)
            {

                throw;
            }
        }



        public void AddLotusServerMailUsers(IList<LotusServerMailUsersHistoryInfo> list)
        {
            try
            {


            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddLotusProfileInfo(IList<LotusProfileHistoryInfo> list)
        {
            try
            {


            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddLotusDomainInfo(IList<LotusDomainInfo> list)
        {
            try
            {

                LotusNotesDA.AddLNSDomainInofBulk(list);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<LotusUserInfo> GetAllLNSUsers()
        {
            try
            {
                IList<LotusUserInfo> lnsUsers = LotusNotesDA.GetAllLNSUserInfo();
                return lnsUsers;
            }
            catch
            {
                throw;
            }
        }
        public bool CheckLNSEmailExists(string email, int endpointSystemId)
        {
            try
            {
                return LotusNotesDA.CheckLNSEmailExists(email, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }
        public bool CheckLNSUserNameExists(string userName, int endpointSystemId)
        {
            try
            {
                return LotusNotesDA.CheckLNSUserNameExists(userName, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }
        public bool CheckLNSLastNameExists(string userName, int endpointSystemId)
        {
            try
            {
                return LotusNotesDA.CheckLNSLastNameExists(userName, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }
        public LotusUserInfo Dessemble(LotusUserInfo _aggregateRoot)
        {
            throw new NotImplementedException();
        }

        public IList<LotusUserInfo> GetAllUnMappedLNSUsers()
        {
            try
            {
                return LotusNotesDA.GetAllUnMappedLNSUsers();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void UpdateLnsUserInfoOnAttributeMap(System.Data.DataTable dt)
        {
            try
            {
                LotusNotesDA.UpdateLnsUserInfoOnAttributeMap(dt);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddLotusUserGroupMapInfo(IList<LotusUserGroupMap> lstgroupmap, EndPoint EndPoint)
        {
            try
            {

                LotusNotesDA.AddLotusUserGroupMapInfoBulk(lstgroupmap);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public LotusUserInfo GetLotusNotesUserByUserName(string FullName)
        {
            try
            {
                return LotusNotesDA.GetLotusNotesUserByUserName(FullName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public LotusGroupInfo GetLotusNotesGroupByGroupName(string GroupName)
        {
            try
            {
                return LotusNotesDA.GetLotusNotesGroupByGroupName(GroupName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<long, string> GetUserIdsWithOwner(long EndpointId)
        {
            return LotusNotesDA.GetUserIdsWithOwner(EndpointId);
        }
        public Dictionary<long, string> GetGroupIdsWithOwner(long EndpointId)
        {
            return LotusNotesDA.GetGroupIdsWithOwner(EndpointId);
        }
        public void SaveUserGroupMap(Dictionary<long, string> dicUsers, Dictionary<long, string> dicGroups, Dictionary<string, List<object>> dicArray, long endpointSystemid)
        {
            try
            {
                LotusNotesDA.SaveUserGroupMap(dicUsers, dicGroups, dicArray, endpointSystemid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveUsers(IList<LotusUserInfo> list, EndPoint Endpoint)
        {
            try
            {
                LotusUserInfo lnsExistingUser = null;
                //  listExistingADusers = _context.ADSUserInfos.ToList();
                foreach (LotusUserInfo usr in list)
                {

                    if (string.IsNullOrEmpty(usr.ShortName))
                        continue;

                    lnsExistingUser = LotusNotesDA.GetUserByShortName(usr.ShortName, Endpoint.Id);
                    if (lnsExistingUser == null)
                    {
                        LotusNotesDA.AddLNSUser(usr);

                    }

                    else
                    {
                        usr.Version = lnsExistingUser.Version;
                        usr.IsLocked = lnsExistingUser.IsLocked;
                        usr.LockScenario = lnsExistingUser.LockScenario;
                        usr.TenantId = lnsExistingUser.TenantId;
                        usr.Id = lnsExistingUser.Id;
                        usr.DIUserId = lnsExistingUser.DIUserId;
                        usr.MappingType = lnsExistingUser.MappingType;
                        usr.AccountType = lnsExistingUser.AccountType;
                        usr.IfUnmappedMapTo = lnsExistingUser.IfUnmappedMapTo;
                        usr.CreatedDate = lnsExistingUser.CreatedDate;
                        usr.CreatedBy = lnsExistingUser.CreatedBy;

                        LotusNotesDA.UpdateLNSUser(usr);
                    }
                    reconcilationFacade.AddOrUpdateAccountInReconTable(usr, lnsExistingUser == null ? true : false);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveGroups(IList<LotusGroupInfo> list, EndPoint Endpoint)
        {
            try
            {
                LotusGroupInfo lnsExistingGroup = null;
                DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Info, list.Count.ToString());
                foreach (LotusGroupInfo group in list)
                {

                    lnsExistingGroup = LotusNotesDA.GetLNSGroup(group);

                    if (lnsExistingGroup == null)
                    {
                        group.CanRequest = true;
                        LotusNotesDA.AddLNSGroup(group);
                    }
                    else
                    {
                        group.Id = lnsExistingGroup.Id;
                        group.CanRequest = lnsExistingGroup.CanRequest;
                        group.CreatedBy = lnsExistingGroup.CreatedBy;
                        group.CreatedDate = lnsExistingGroup.CreatedDate;

                        LotusNotesDA.UpdateLNSGroup(group);

                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<LotusCertifierInfo> GetAllLNSCertifiers()
        {
            try
            {
                return LotusNotesDA.GetAllLNSCertifiers();
            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting Lotus Notes Certifiers ", ex);
                throw ex;
            }
        }

        public void SaveCertifiers(IList<LotusCertifierInfo> list, EndPoint Endpoint)
        {
            try
            {
                LotusCertifierInfo lnsExistingCertifier = null;
                DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Info, list.Count.ToString());
                foreach (LotusCertifierInfo certifier in list)
                {

                    lnsExistingCertifier = LotusNotesDA.GetLNSCertifier(certifier);

                    if (lnsExistingCertifier == null)
                    {
                        LotusNotesDA.AddLNSCertifier(certifier);
                    }
                    else
                    {
                        certifier.Id = lnsExistingCertifier.Id;
                        certifier.CreatedBy = lnsExistingCertifier.CreatedBy;
                        certifier.CreatedDate = lnsExistingCertifier.CreatedDate;

                        LotusNotesDA.UpdateLNSCertifier(certifier);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<LotusGroupInfo> GetLnsGroupsByDIUserId(int? endPointSystemId, int userId)
        {
            return LotusNotesDA.GetLnsGroupsByDIUserId(endPointSystemId, userId);
        }

        

        public IList<LotusUserInfo> GetLNSUserInfoByDIUserId(long DIUserId)
        {
            return LotusNotesDA.GetLNSUserInfoByDIUserId(DIUserId);
        }

        public List<LotusUserInfo> GetPrimaryLNSUserByDIUserId(long DIUserId, int endpointSystemId)
        {
            return LotusNotesDA.GetPrimaryLNSUserByDIUserId(DIUserId, endpointSystemId);
        }

        public LotusUserInfo GetLockedLNSUserByDIUserId(long DIUserId, long endpointSystemId)
        {
            return LotusNotesDA.GetLockedLNSUserByDIUserId(DIUserId, endpointSystemId);
        }

        public LotusUserInfo GetLNSUserByUserId(long userId)
        {
            return LotusNotesDA.GetLNSUserByUserId(userId);
        }

        public LotusUserInfo GetLNSUserByMailAddress(string mailAddress)
        {
            return LotusNotesDA.GetLNSUserByMailAddress(mailAddress);
        }

        public LotusUserInfo GetLNSUserByShortName(string shortName)
        {
            return LotusNotesDA.GetLNSUserByShortName(shortName);
        }

        public bool IsMailFileNameExist(string mailFileName)
        {
            return LotusNotesDA.IsMailFileNameExist(mailFileName);
        }
        public bool IsUserFileNameExist(string userfileName)
        {
            return LotusNotesDA.IsMailFileNameExist(userfileName);
        }

        public bool IsInternetAddressExist(string internetAddress)
        {
            try
            {
                return LotusNotesDA.IsInternetAddressExist(internetAddress);
            }
            catch
            {
                throw;
            }
        }

        public bool IsLastNameExist(string lastName)
        {
            return LotusNotesDA.IsLastNameExist(lastName);
        }

        //Newly Added

        public void ExecuteQry(string cmd)
        {
            EndpointObjectDA EndpointObjectDA = new EndpointObjectDA();
            EndpointObjectDA.ExecuteQry(cmd);

        }
        public Dictionary<long, string> GetAllLNSGroupInfo(string searchString)
        {
            try
            {
                return LotusNotesDA.GetAllLNSGroupInfo(searchString);

            }
            catch
            {
                throw;
            }
        }
        public IList<LotusGroupInfo> GetAllLNSGroupInfo()
        {
            try
            {
                return LotusNotesDA.GetAllLNSGroupInfo();
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting Lotus Notes Groups ", ex);
                throw ex;
            }
        }

        public IList<LNSUserToGroupsReportData> GetLNSUserToGroupsReportData()
        {
            try
            {
                return LotusNotesDA.GetLNSUserToGroupsReportData();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<LNSGroupToUsersReportData> GetLNSGroupToUsersReportData()
        {
            try
            {
                return LotusNotesDA.GetLNSGroupToUsersReportData();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<LotusUserHistoryInfo> GetAllLNSHistoryUsers()
        {
            try
            {
                return LotusNotesDA.GetAllLNSHistoryUsers();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<LotusUserHistoryInfo> GetAllLNSusersbyendpointID(int EnpointID)
        {
            try
            {
                return LotusNotesDA.GetLNShistoryUsersbyEndpointID(EnpointID);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public IList<EndpointObjects> GetLNSObjects()
        {
            try
            {
                return LotusNotesDA.GetLNSObjects();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<LotusNotesUsersReportData> GetAllLotusNotesUsersReportData()
        {
            try
            {
                return LotusNotesDA.GetAllLotusNotesUsersReportData();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<LotusNotesGroupsReportData> GetAllLotusNotesGroupsReportData()
        {
            try
            {
                return LotusNotesDA.GetAllLotusNotesGroupsReportData();
            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting Lotus Notes Groups ", ex);
                throw ex;
            }
        }

        public List<LotusGroupInfo> GetLNSGroupsByUserId(long userId, int endpointSystemId)
        {
            return LotusNotesDA.GetLNSGroupsByUserId(userId, endpointSystemId);
        }

        public List<LotusUserInfo> GetUsersExternalEmailAccess()
        {

            try
            {
                return LotusNotesDA.GetUsersExternalEmailAccess();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateUser(LotusUserInfo _user)
        {
            try
            {
                LotusNotesDA.UpdateUser(_user);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public LotusGroupInfo GetGroupByGroupName(string groupName)
        {
            try
            {
                return LotusNotesDA.GetGroupByGroupName(groupName);
            }
            catch
            {
                throw;
            }
        }

        public List<long> GetOwnerByGroupName(string groupName)
        {
            try
            {
                string ownerName = string.Empty;

                List<long> ownerIds = new List<long>();
                var group = LotusNotesDA.GetGroupByGroupName(groupName);
                if (group != null)
                {
                    if (!string.IsNullOrEmpty(group.Owner))
                    {
                        List<string> owners = group.Owner.Split(';').ToList();

                        foreach (string owner in owners)
                        {
                            if (!string.IsNullOrEmpty(owner))
                            {
                                ownerName = owner;

                                //ownerName = ownerName.Replace("CN=", string.Empty);
                                //ownerName = ownerName.Replace("OU=", string.Empty);
                                //ownerName = ownerName.Replace("O=", string.Empty);

                                var user = LotusNotesDA.GetLotusNotesUserByOwner(ownerName);
                                if (user != null)
                                {
                                    long ownerId = user.DIUserId.HasValue ? user.DIUserId.Value : 0;

                                    if (ownerId != 0)
                                    {
                                        ownerIds.Add(ownerId);
                                    }
                                }
                            }
                        }
                    }
                }
                return ownerIds;
            }
            catch
            {
                throw;
            }
        }

        public IList<LotusUserInfo> GetDeletedUsers(long EndpointId, Process discoveryProcess)
        {
            try
            {
                return LotusNotesDA.GetDeletedUsers(EndpointId, discoveryProcess);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoveLNSUser(IList<LotusUserInfo> listDelUser)
        {
            try
            {
                foreach (LotusUserInfo user in listDelUser)
                {
                    LotusNotesDA.RemoveLNSUser(user);

                    reconcilationFacade.DeleteAccount((int)user.EndPointSystemId, user.Id);

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<LotusUserInfo> GetLotusNotesUsersByGroupName(string GroupName, long EndpointSystemId)
        {
            try
            {
                return LotusNotesDA.GetLotusNotesUsersByGroupName(GroupName, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public LotusUserInfo GetLNSUserInfo(long EndpointUserID)
        {
            return LotusNotesDA.GetLNSUserInfo(EndpointUserID);
        }

        public void GetAllMasterUsers(IList<LotusUserInfo> listLnsUsers, bool mapAll)
        {
            try
            {
                int count = -1;
                int newRowCount = -1;
                DataRow[] dRowsMaster = null;

                string clauseType = "AND";
                if (!mapAll)
                    clauseType = "OR";


                StringBuilder bldr = new StringBuilder();

                IList<User> users = new List<User>();

                object obj = null;

                IList<ConfigGeneralTrustedSource> listTrustedConfig = null;
                IList<ConfigGeneralTrustedSource> list = null;// new DiConfigurationDA().GetAllConfigTrustedSource();

                listTrustedConfig = list.Where(li => li.TrustedSource == EndpointType.LotusNotes).ToList();

                IList<AttributeMappingColumns> listAttributes = new DiConfigurationDA().GetAttributeMappingColumns(listLnsUsers.FirstOrDefault().EndPointSystemId);


                if (listTrustedConfig.Count <= 0)
                {
                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Populate User: No attribute is configured from LOTUSNOTES"));
                    return;
                }

                if (listAttributes.Count <= 0)
                {
                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Populate User: Attribute Mapping is NOT configured for LOTUSNOTES"));
                    return;
                }

                Type usertype = typeof(User);

                IList<PropertyInfo> userProperties = usertype.GetProperties().ToList();

                string tenantId = string.Empty;

                if (listLnsUsers.Count > 0)
                {
                    tenantId = listLnsUsers.First().TenantId;
                }
                IList<User> listUsers = new List<User>();

                if (tenantId != string.Empty)
                    listUsers = new UserDA().GetAllUsers(tenantId);
                else
                    listUsers = new UserDA().GetAllUsers();

                DataTable dtMaster = CollectionHelper.ConvertToNormalTable<User>(listUsers);

                DataTable dtExUser = CollectionHelper.ConvertToNormalTable<LotusUserInfo>(listLnsUsers);


                GeneralDA genDA = new GeneralDA();


                newRowCount = genDA.GetMaxId("DI_M_User", "UserId");

                foreach (DataRow adsRow in dtExUser.Rows)
                {
                    count = 0;
                    bldr = new StringBuilder();

                    foreach (AttributeMappingColumns att in listAttributes)
                    {

                        bldr.AppendFormat(string.Format("{0}='{1}'", att.DiColumn, adsRow[att.Tgcolumn].ToString().Replace("'", "''")));

                        count++;

                        if (count < listAttributes.Count)
                            bldr.AppendFormat(" {0} ", clauseType);

                    }

                    dRowsMaster = dtMaster.Select(bldr.ToString());

                    if (dRowsMaster.Count() > 0) //modify existing user
                    {

                        foreach (ConfigGeneralTrustedSource trustedSource in listTrustedConfig)
                        {

                            if (string.IsNullOrEmpty(dRowsMaster[0][trustedSource.AttributeName].ToString()))
                                dRowsMaster[0][trustedSource.AttributeName] = adsRow[trustedSource.TrustedSourceAttribute];

                            else if (!trustedSource.SelfUpdate)
                                dRowsMaster[0][trustedSource.AttributeName] = adsRow[trustedSource.TrustedSourceAttribute];
                        }

                        dRowsMaster[0]["LastModifiedBy"] = "Scheduler";
                        dRowsMaster[0]["LastModifiedDate"] = DateTime.Now;

                    }


                }



                new UserDA().UpdateMasterLatest(dtMaster, tenantId);

                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Global users populated"));
            }
            catch (Exception ec)
            {

            }
        }
    }

}
