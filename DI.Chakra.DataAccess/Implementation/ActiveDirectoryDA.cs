using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using System.Data;
using System.Data.SqlClient;

using DI.Chakra.Infrastructure;
using System.Collections;
using System.ComponentModel;
using System.Data.Entity;
using DI.Chakra.Model.SAP;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;


namespace DI.Chakra.DataAccess
{
    public class ActiveDirectoryDA
    {
        private QueryExecutor m_executor = null;

        private IACMDbOperator m_iacmOperator;

        IACMDBContext _context;
        public ActiveDirectoryDA()
        {

            _context = new IACMDBContext();

        }

        public ADSConfiguration GetConfiguration(int _systemId)
        {
            ADSConfiguration config = null;
            config = _context.ADSConfiguration.FirstOrDefault(s => s.EndpointSystem.Id == _systemId);
            config.Password = RSAHelper.DecryptData(config.Password);
            return config;
        }

        //*UserGroupMap 
        public class ADUserGroup : IAggregateRoot
        {
            public long ADSGroupInfo_Id { get; set; }
            public long ADSUserInfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }

        #region ADObjects BulkAdd
        public List<ADSUserInfo> GetHomeFolderAccessAssignmentFailedUsers()
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.ADSUserInfos.Where(usr => usr.IsHomeFolderPermissionSetFailed).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void AddADSUserInfoHistoryBulk(IList<ADSUserInfoHistory> users)
        {

            SQLBulkUploader<ADSUserInfoHistory> sqlUserHistory = null;
            try
            {
                string tableName = "DI_ADS_UserInfoHistory";

                sqlUserHistory = new SQLBulkUploader<ADSUserInfoHistory>(tableName);

                sqlUserHistory.Flush(users);



            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlUserHistory.Close();
            }
        }


        public void AddADSUserInfoBulk(IList<ADSUserInfo> users)
        {
            SQLBulkUploader<ADSUserInfo> sql = null;
            try
            {
                string tableName = "DI_ADS_UserInfo";

                sql = new SQLBulkUploader<ADSUserInfo>(tableName);

                sql.Flush(users);


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sql.Close();

            }
        }

        public void AddADSUserGroupBulk(IList<ADUserGroup> userGroup)
        {

            SQLBulkUploader<ADUserGroup> sqlUserHistory = null;
            try
            {
                string tableName = "DI_ADS_UserGroup";

                sqlUserHistory = new SQLBulkUploader<ADUserGroup>(tableName);

                sqlUserHistory.Flush(userGroup);



            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlUserHistory.Close();
            }
        }
        public void AddADSComputerGroupBulk(IList<ADSComputerGroup> computerGroup)
        {

            SQLBulkUploader<ADSComputerGroup> sqlUserHistory = null;
            try
            {
                string tableName = "DI_ADS_ComputerGroup";

                sqlUserHistory = new SQLBulkUploader<ADSComputerGroup>(tableName);

                sqlUserHistory.Flush(computerGroup);



            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlUserHistory.Close();
            }
        }
        public void AddADSGroupInfoBulk(IList<ADSGroupInfo> groups)
        {
            SQLBulkUploader<ADSGroupInfo> sqlBulk = null;
            try
            {
                string tableName = "DI_ADS_GroupInfo";

                sqlBulk = new SQLBulkUploader<ADSGroupInfo>(tableName);

                sqlBulk.Flush(groups);


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlBulk.Close();
            }
        }
        public void AddADSGroupInfoHistoryBulk(IList<ADSGroupInfoHistory> groups)
        {
            SQLBulkUploader<ADSGroupInfoHistory> sql = null;
            try
            {
                string tableName = "DI_ADS_GroupInfoHistory";

                sql = new SQLBulkUploader<ADSGroupInfoHistory>(tableName);

                sql.Flush(groups);


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sql.Close();
            }
        }



        public void AddADSOuInfoBulk(IList<ADSOuInfo> ouList)
        {
            SQLBulkUploader<ADSOuInfo> sql = null;
            try
            {
                string tableName = "DI_ADS_OuInfo";

                sql = new SQLBulkUploader<ADSOuInfo>(tableName);

                sql.Flush(ouList);


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sql.Close();
            }
        }
        public void AddADSOuInfo(IList<ADSOuInfo> ouList)
        {
            SQLBulkUploader<ADSOuInfo> sql = null;
            try
            {
                string tableName = "DI_ADS_OuInfo";

                sql = new SQLBulkUploader<ADSOuInfo>(tableName);

                sql.Flush(ouList);


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sql.Close();
            }
        }

        public void AddADSOuInfoHistoryBulk(IList<ADSOuInfoHistory> ouList)
        {
            SQLBulkUploader<ADSOuInfoHistory> sql = null;
            try
            {
                string tableName = "DI_ADS_OuInfoHistory";

                sql = new SQLBulkUploader<ADSOuInfoHistory>(tableName);

                sql.Flush(ouList);


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sql.Close();
            }
        }
        public void AddADSComputerInfoBulk(IList<ADSComputerInfo> computerList)
        {
            SQLBulkUploader<ADSComputerInfo> sql = null;
            try
            {
                string tableName = "DI_ADS_ComputerInfo";

                sql = new SQLBulkUploader<ADSComputerInfo>(tableName);

                sql.Flush(computerList);


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sql.Close();
            }
        }





        public void AddADSPrinterInfoBulk(IList<ADSPrinterInfo> _list)
        {
            SQLBulkUploader<ADSPrinterInfo> sql = null;
            try
            {
                string tableName = "DI_ADS_PrinterInfo";

                sql = new SQLBulkUploader<ADSPrinterInfo>(tableName);

                sql.Flush(_list);


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sql.Close();

            }
        }
        public void AddADSSharedFolderInfoBulk(IList<ADSSharedFolderInfo> _list)
        {
            SQLBulkUploader<ADSSharedFolderInfo> sql = null;
            try
            {
                string tableName = "DI_ADS_SharedFolderInfo";

                sql = new SQLBulkUploader<ADSSharedFolderInfo>(tableName);

                sql.Flush(_list);


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sql.Close();
            }
        }

        public List<ADSUserInfo> GetAdsUserInfo(long DIUserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (DIUserId > 0)
                    {
                        return (from user in _context.ADSUserInfos
                                      .Include("EndPointSystem")
                                      .Include("EndPointSystem.EndPointSystemType")
                                where user.DIUserId == DIUserId && user.IsLocked == false
                                orderby user.CreatedDate
                                select user).ToList();
                    }
                    return (from user in _context.ADSUserInfos
                        .Include("EndPointSystem")
                        .Include("EndPointSystem.EndPointSystemType")
                        where user.IsLocked == false
                        orderby user.CreatedDate
                        select user).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public ADSUserInfo GetADUserByUserId(long userId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSUserInfos.Include(lq => lq.ADSGroups).FirstOrDefault(item => item.Id == userId);
                }
            }
            catch
            {
                throw;
            }
        }

        public ADSUserInfo GetADUserByCN(string cn, long endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSUserInfos.FirstOrDefault(item => item.cn == cn && item.EndPointSystemId == endpointSystemId);
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ADSGroupInfo> GetADGroupsByUserId(long userId, long? endpointSystemId)
        {
            try
            {
                ADSUserInfo adUserInfo = null;

                using (IACMDBContext _context = new IACMDBContext())
                {
                    List<ADSGroupInfo> adGroups = new List<ADSGroupInfo>();
                    if (endpointSystemId == null)
                        adUserInfo = _context.ADSUserInfos.Include(lq => lq.ADSGroups).FirstOrDefault(item => item.Id == userId);
                    else
                        adUserInfo =
                            _context.ADSUserInfos.Include(lq => lq.ADSGroups).FirstOrDefault(
                                item => item.Id == userId && item.EndPointSystemId == endpointSystemId);
                    if (adUserInfo != null)
                    {
                        adGroups = (List<ADSGroupInfo>)adUserInfo.ADSGroups;
                    }
                    return adGroups;
                }
            }
            catch
            {
                throw;
            }
        }
        public List<ADSGroupInfo> GetADGroupsByDIUserId(long userId)
        {
            try
            {
                ADSUserInfo adUserInfo = null;

                using (IACMDBContext _context = new IACMDBContext())
                {
                    List<ADSGroupInfo> adGroups = new List<ADSGroupInfo>();
                        adUserInfo =
                            _context.ADSUserInfos.Include(lq => lq.ADSGroups).FirstOrDefault(
                                item => item.DIUserId == userId );
                    if (adUserInfo != null)
                    {
                        adGroups = (List<ADSGroupInfo>)adUserInfo.ADSGroups;
                    }
                    return adGroups;
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ADSGroupInfo> GetADGroupsByDIUserId(long userId, AccountType AccountType)
        {
            try
            {
                ADSUserInfo adUserInfo = null;

                using (IACMDBContext _context = new IACMDBContext())
                {
                    List<ADSGroupInfo> adGroups = new List<ADSGroupInfo>();
                    adUserInfo =
                        _context.ADSUserInfos.Include(lq => lq.ADSGroups).FirstOrDefault(
                            item => item.DIUserId == userId && item.AccountType == AccountType);
                    if (adUserInfo != null)
                    {
                        adGroups = (List<ADSGroupInfo>)adUserInfo.ADSGroups;
                    }
                    return adGroups;
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ADSUserInfo> GetPrimaryADUserByDIUserId(long DIUserId, long endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSUserInfos.Where(item => item.DIUserId == DIUserId && item.EndPointSystemId == endpointSystemId
                            && (item.AccountType == AccountType.User || item.AccountType == AccountType.Secondary) && !item.IsAccountDisabled && !item.IsLocked).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public ADSUserInfo GetPrimaryADAccountByDIUserId(long DIUserId, long endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSUserInfos.FirstOrDefault(item => item.DIUserId == DIUserId && item.EndPointSystemId == endpointSystemId
                            && item.AccountType == AccountType.User && !item.IsAccountDisabled && !item.IsLocked);
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ADSUserInfo> GetADUserByDIUserId(long DIUserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSUserInfos.Where(item => item.DIUserId == DIUserId
                            && (item.AccountType == AccountType.User || item.AccountType == AccountType.Secondary) && !item.IsAccountDisabled && !item.IsLocked).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ADSUserInfo> GetADUserByDIUserId(long DIUserId, AccountType AccountType)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSUserInfos.Where(item => item.DIUserId == DIUserId
                            && (item.AccountType == AccountType) && !item.IsAccountDisabled && !item.IsLocked).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public ADSUserInfo GetLockedADUserByDIUserId(int DIUserId, int endpointSystemId, bool primaryAccount)
        {
            try
            {
                if (primaryAccount)
                {
                    return _context.ADSUserInfos.FirstOrDefault(item => item.DIUserId == DIUserId && item.EndPointSystemId == endpointSystemId
                            && item.AccountType == AccountType.User && (item.IsAccountDisabled || item.IsLocked));
                }
                return _context.ADSUserInfos.FirstOrDefault(item => item.DIUserId == DIUserId && item.EndPointSystemId == endpointSystemId
                                                                    && item.AccountType == AccountType.Secondary && (item.IsAccountDisabled || item.IsLocked));
            }
            catch
            {
                throw;
            }
        }

        public ADSUserInfo GetADSUserInfoByName(string userName)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSUserInfos.FirstOrDefault(item => item.sAMAccountName.Equals(userName, StringComparison.CurrentCultureIgnoreCase));
                }
            }
            catch
            {
                throw;
            }
        }

        public ADSUserInfo GetADSPrimaryUserInfoByName(string userName)
        {
            try
            {
                ADSUserInfo adsUserInfo = null;
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSUserInfos.FirstOrDefault(item => item.sAMAccountName == userName);
                    }
                }
            catch
            {
                throw;
            }
        }

        public IList<ADSUserInfo> GetAllADSUserInfo()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSUserInfos.ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while getting All ADUsers ", ex);
                throw ex;
            }
        }


        public IList<ADSUserInfo> GetADSUserInfoByEndpointSystem(int EndpointSystemID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSUserInfos.Where(lq => lq.EndPointSystemId == EndpointSystemID).Select(lq => lq).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while getting All ADUsers ", ex);
                throw ex;
            }
        }

        public bool CheckADUserNameExists(string samaAccountName, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSUserInfos.Any(item => item.sAMAccountName.Equals(samaAccountName, StringComparison.CurrentCultureIgnoreCase)
                        && item.EndPointSystemId == endpointSystemId);
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<ADSUserInfo> GetAllADSUserInfoBasedOnLastLogon()
        {
            IList<ADSUserInfo> _list = new List<ADSUserInfo>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _list = new List<ADSUserInfo>();
                    _list = _context.ADSUserInfos.Where(usr => usr.lastLogon.Value != null).ToList();
                }
                return _list;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while getting All ADUsers ", ex);
                throw ex;
            }
        }

        public IList<ADSGroupInfo> GetAllADSGroupInfo()
        {
            return _context.ADSGroupInfos.ToList();
           

        }

        public IList<ADSOuInfo> GetAllADSOUInfo()
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.ADSOuInfos.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<ADSOuInfo> GetADSOUInfoByEndPointSystemID(int EndpointSystemID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.ADSOuInfos.Where(lq => lq.EndPointSystemId == EndpointSystemID).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<ADSGroupInfo> GetAllADSGroupsByEndpointSystem(long EndpointSystemId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return (from groups in oIACMDBContext.ADSGroupInfos 
                            where groups.EndPointSystemId == EndpointSystemId 
                            orderby groups.name
                            select groups).ToList();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }


        public Dictionary<long, string> GetADSGroupInfo(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSGroupInfos.Where(item => item.name.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.name})
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => (long)o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }
        public IList<ADSGroupInfo> GetAllADSGroupInfo(string paramgroupType)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.ADSGroupInfos.Where(grp => grp.groupTypeDesc == paramgroupType).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<ADSGroupInfo> GetADSGroupInfoByEndpointSystemID(string paramgroupType, int EndpointSystemID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.ADSGroupInfos.Where(grp => grp.groupTypeDesc == paramgroupType && grp.EndPointSystemId == EndpointSystemID).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region UpdateADSUser

        public void UpdateDiUserId(int UserId, int DiUserId, int EndPointSystemId, EndpointType EndPointSystemType)
        {
            //string query = "DI_spUpdateDiUserId";
            User itemOfUser;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (EndPointSystemType == EndpointType.ActiveDirectory)
                    {
                        //FOR AD
                        ADSUserInfo ADSUInfo = _context.ADSUserInfos.FirstOrDefault(ENDPT => ENDPT.Id == UserId &&
                                                                                               ENDPT.EndPointSystemId == EndPointSystemId);
                        // itemOfUser = _context.Users.FirstOrDefault(usr => usr.UserId == DiUserId);
                        if (ADSUInfo != null)
                        {
                            ADSUInfo.DIUserId = DiUserId;
                            ADSUInfo.LastModifiedDate = DateTime.Today;
                            _context.SaveChanges();
                        }
                    }
                    else if (EndPointSystemType == EndpointType.LotusNotes)
                    {
                        LotusUserInfo LOTUSUInfo = _context.LotusUserInfos.FirstOrDefault(ENDPT => ENDPT.Id == UserId &&
                                                                                            ENDPT.EndPointSystemId == EndPointSystemId);
                        if (LOTUSUInfo != null)
                        {
                            LOTUSUInfo.DIUserId = DiUserId;
                            LOTUSUInfo.LastModifiedDate = DateTime.Today;
                            _context.SaveChanges();
                        }
                    }
                   
                    else if (EndPointSystemType == EndpointType.SAP)
                    {
                        SAPUserinfo SAPUInfo = _context.SAPUserinfos.FirstOrDefault(ENDPT => ENDPT.Id == UserId &&
                                                                                            ENDPT.EndPointSystemId == EndPointSystemId);
                        if (SAPUInfo != null)
                        {
                            SAPUInfo.DIUserId = DiUserId;
                            SAPUInfo.LastModifiedDate = DateTime.Today;
                            _context.SaveChanges();
                        }
                    }
                    else if (EndPointSystemType == EndpointType.SAPEnterprisePortal)
                    {
                        SAPEPUserInfo SAPEPUInfo = _context.SAPEPUserInfos.FirstOrDefault(ENDPT => ENDPT.Id == UserId &&
                                                                                            ENDPT.EndPointSystemId == EndPointSystemId);
                        if (SAPEPUInfo != null)
                        {
                            SAPEPUInfo.DIUserId = DiUserId;
                            SAPEPUInfo.LastModifiedDate = DateTime.Today;
                            _context.SaveChanges();
                        }
                    }
                    else if (EndPointSystemType == EndpointType.InhouseSystems)
                    {
                        InhouseUserInfo INHUInfo = _context.InhouseUserInfos.FirstOrDefault(ENDPT => ENDPT.Id == UserId &&
                                                                                            ENDPT.EndPointSystemId == EndPointSystemId);
                        if (INHUInfo != null)
                        {
                            INHUInfo.DIUserId = DiUserId;
                            INHUInfo.LastModifiedDate = DateTime.Today;
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while Updating The DiUserId ", ex);
                throw;
            }
        }

        public void UpdateDiUserIdWithMappingInfo(int UserId, int DiUserId, string MapTo, int EndPointSystemId, EndpointType EndPointSystemType, string UserName, AccountType accountType)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    User DIUser = _context.Users.FirstOrDefault(diusr => diusr.Id == DiUserId);
                    if (EndPointSystemType == EndpointType.ActiveDirectory)
                    {
                        //FOR AD
                        ADSUserInfo ADSUInfo = _context.ADSUserInfos.FirstOrDefault(ENDPT => ENDPT.Id == UserId &&
                                                                                               ENDPT.EndPointSystemId == EndPointSystemId);
                        if (ADSUInfo != null)
                        {
                            ADSUInfo.MappingType = "Manual";
                            ADSUInfo.DIUserId = DiUserId;
                            ADSUInfo.AccountType = accountType;
                            ADSUInfo.IfUnmappedMapTo = MapTo;
                            ADSUInfo.LastModifiedDate = DateTime.Today;
                            ADSUInfo.LastModifiedBy = UserName;

                            if (accountType == AccountType.User)
                                DIUser.UserName = ADSUInfo.sAMAccountName;

                            _context.SaveChanges();
                        }
                    }
                    else if (EndPointSystemType == EndpointType.LotusNotes)
                    {
                        LotusUserInfo LOTUSUInfo = _context.LotusUserInfos.FirstOrDefault(ENDPT => ENDPT.Id == UserId &&
                                                                                            ENDPT.EndPointSystemId == EndPointSystemId);
                        if (LOTUSUInfo != null)
                        {
                            LOTUSUInfo.DIUserId = DiUserId;
                            LOTUSUInfo.LastModifiedDate = DateTime.Today;
                            LOTUSUInfo.MappingType = "Manual";
                            LOTUSUInfo.AccountType = accountType;
                            LOTUSUInfo.IfUnmappedMapTo = MapTo;
                            LOTUSUInfo.LastModifiedBy = UserName;
                            _context.SaveChanges();
                           

                        }
                    }

                    else if (EndPointSystemType == EndpointType.SAP)
                    {
                        SAPUserinfo SAPUInfo = _context.SAPUserinfos.FirstOrDefault(ENDPT => ENDPT.Id == UserId &&
                                                                                            ENDPT.EndPointSystemId == EndPointSystemId);
                        if (SAPUInfo != null)
                        {
                            SAPUInfo.DIUserId = DiUserId;
                            SAPUInfo.LastModifiedDate = DateTime.Today;
                            SAPUInfo.MappingType = "Manual";
                            SAPUInfo.AccountType = accountType;
                            SAPUInfo.IfUnmappedMapTo = MapTo;
                            SAPUInfo.LastModifiedBy = UserName;
                            _context.SaveChanges();
                        }
                    }
                    else if (EndPointSystemType == EndpointType.SAPEnterprisePortal)
                    {
                        SAPEPUserInfo SAPEPUInfo = _context.SAPEPUserInfos.FirstOrDefault(ENDPT => ENDPT.Id == UserId &&
                                                                                            ENDPT.EndPointSystemId == EndPointSystemId);
                        if (SAPEPUInfo != null)
                        {

                            SAPEPUInfo.DIUserId = DiUserId;
                            SAPEPUInfo.LastModifiedDate = DateTime.Today;
                            SAPEPUInfo.MappingType = "Manual";
                            SAPEPUInfo.AccountType = accountType;
                            SAPEPUInfo.IfUnmappedMapTo = MapTo;
                            SAPEPUInfo.LastModifiedBy = UserName;
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while Updating The DiUserId ", ex);
                throw;
            }
        }

        public void UpdateUserInfoBatchOnAttributeMap(IList<ADSUserInfo> _adsUserInfoList)
        {

            SqlConnection conn = null;
            SqlCommand selectCmd = null;
            SqlCommand updateCmd = null;
            SqlDataAdapter da = null;

            StringBuilder strInsert = new StringBuilder();
            StringBuilder strUpdate = new StringBuilder();
            StringBuilder strDelete = new StringBuilder();

            int newRowCount = -1;

            try
            {
                m_iacmOperator = new IACMDbOperator();

                conn = m_iacmOperator.GetConnection();

                selectCmd = conn.CreateCommand();
                //TODO: check this code by srinivas
                selectCmd.CommandText = "SELECT Id, DistinguishedName, DIUserId FROM DI_ADS_UserInfo";
                // selectCmd.CommandText = string.Format("SELECT UserId, DistinguishedName, DIUserId FROM DI_ADS_UserInfo where TenantId='{0}' and EndPointSystemId={1}", endpoint.TenantId, endpoint.Endpointsystemid);
                updateCmd = conn.CreateCommand();
                updateCmd.CommandText = "UPDATE DI_ADS_UserInfo Set DIUserId=@DIUserId Where Id=@UserId";
                updateCmd.CommandType = CommandType.Text;

                List<SqlParameter> parameters = null;
                parameters = GetUserParamsForAttributeMap();
                updateCmd.Parameters.AddRange(parameters.ToArray());

                updateCmd.UpdatedRowSource = UpdateRowSource.None;


                updateCmd.UpdatedRowSource = UpdateRowSource.None;

                da = new SqlDataAdapter(selectCmd);
                da.UpdateCommand = updateCmd;

                da.UpdateBatchSize = 100;

                da.AcceptChangesDuringUpdate = true;

                DataSet ds = new DataSet();
                da.Fill(ds, "ADSUserInfo");

                DataRow[] drowsMax = ds.Tables[0].Select("UserId = max(UserId)");

                if (drowsMax.Count() <= 0)
                    newRowCount = 0;
                else
                    newRowCount = Convert.ToInt32(drowsMax[0]["UserId"]);

                DataTable dtNew = CollectionHelper.ConvertToUserInfo(_adsUserInfoList, getUserInfoTable());

                if (ds.Tables[0].Rows.Count <= 0)
                {

                    ds.Tables[0].Merge(dtNew, false);
                    da.Update(ds, "ADSUserInfo");
                    return;
                }

                DataRow drow = null;

                foreach (DataRow dr in dtNew.Rows)
                {
                    DataRow[] dRows = ds.Tables[0].Select("distinguishedName='" + dr["distinguishedName"].ToString().Replace("'", "''") + "'");

                    if (dRows.Count() <= 0)
                    {
                        drow = ds.Tables[0].NewRow();
                        drow["UserId"] = ++newRowCount;
                        drow["DIUserId"] = dr["DIUserId"];

                        ds.Tables[0].Rows.Add(drow);
                    }
                    else
                    {
                        drow = dRows[0];

                        //drow["UserId"] = dr["UserId"];
                        drow["DIUserId"] = dr["DIUserId"];
                        strUpdate = strUpdate.AppendFormat(",{0}", drow["UserId"]);
                    }
                }

                //if (strUpdate.ToString() != string.Empty)
                //    DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Updated userId(s):" + strUpdate.ToString());

                try
                {
                    da.Update(ds.Tables[0].Select(null, null, DataViewRowState.ModifiedCurrent));
                }
                catch (Exception ex)
                {
                    DILog.GeneralLogger.WriteLog(LogLevelType.Error, "UserInfo update Error", ex);
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Error, "UserInfo Error", ex);
                throw;
            }
            finally
            {
                m_iacmOperator.Close();
                selectCmd.Dispose();
                updateCmd.Dispose();
            }
        }

        public void UpdateUserInfoBatchOnAttributeMap(DataTable dtAdsUser, EndPoint EndPoint)
        {

            SqlConnection conn = null;
            SqlCommand selectCmd = null;
            SqlCommand updateCmd = null;

            SqlDataAdapter da = null;

            StringBuilder strInsert = new StringBuilder();
            StringBuilder strUpdate = new StringBuilder();
            StringBuilder strDelete = new StringBuilder();

            int count = 0;

            try
            {
                m_iacmOperator = new IACMDbOperator();

                conn = m_iacmOperator.GetConnection();

                selectCmd = conn.CreateCommand();
                //selectCmd.CommandText = string.Format("SELECT UserId, sAMAccountName, DIUserId FROM DI_ADS_UserInfo where TenantId='{0}' and EndPointSystemId={1}", EndPoint.TenantId, EndPoint.Endpointsystemid);
                selectCmd.CommandText = string.Format("SELECT Id, sAMAccountName, DIUserId FROM DI_ADS_UserInfo where (DIUserId=-1 OR DIUserId IS NULL)", EndPoint.TenantId, EndPoint.Id);

                updateCmd = conn.CreateCommand();
                updateCmd.CommandText = string.Format("UPDATE DI_ADS_UserInfo Set DIUserId=@DIUserId Where Id=@UserId", EndPoint.TenantId, EndPoint.Id);

                updateCmd.CommandType = CommandType.Text;

                List<SqlParameter> parameters = null;
                parameters = GetUserParamsForAttributeMap();
                updateCmd.Parameters.AddRange(parameters.ToArray());

                updateCmd.UpdatedRowSource = UpdateRowSource.None;


                da = new SqlDataAdapter(selectCmd);
                da.UpdateCommand = updateCmd;

                da.UpdateBatchSize = 100;

                da.AcceptChangesDuringUpdate = true;

                DataSet ds = new DataSet();
                da.Fill(ds, "ADSUserInfo");


                //DataRow[] drowsMax = ds.Tables[0].Select("UserId = max(UserId)");

                //if (drowsMax.Count() <= 0)
                //    newRowCount = 0;
                //else
                //    newRowCount = Convert.ToInt32(drowsMax[0]["UserId"]);

                //DataTable dtNew = dtAdsUser;

                if (ds.Tables[0].Rows.Count <= 0)
                {

                    ds.Tables[0].Merge(dtAdsUser, false);
                    da.Update(ds, "ADSUserInfo");
                    return;
                }

                DataRow drow = null;
                DataRow[] rows = dtAdsUser.Select("DIUserId<>-1");
                if (rows.Count() > 0)
                {
                    DataTable dtUnmappedADS = rows.CopyToDataTable();

                    foreach (DataRow dr in dtUnmappedADS.Rows)
                    {
                        DataRow[] dRows = ds.Tables[0].Select("sAMAccountName='" + dr["sAMAccountName"].ToString().Replace("'", "''") + "'");

                        if (dRows.Count() > 0)
                        {
                            drow = dRows[0];

                            drow["DIUserId"] = dr["DIUserId"];

                            count++;

                            if (count % 1000 == 0)
                               DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("MAP IACM User: Processed {0} ADUser objects", count));
                        }
                    }
                }

                //if (strUpdate.ToString() != string.Empty)
                //    DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Updated userId(s):" + strUpdate.Length.ToString());


                try
                {
                   DILog.GeneralLogger.WriteLog(LogLevelType.Info, "MAP IACM User: Start updating AD user(s) objects");

                    da.Update(ds.Tables[0].Select(null, null, DataViewRowState.ModifiedCurrent));

                   DILog.GeneralLogger.WriteLog(LogLevelType.Info, "MAP IACM User: Finished updating AD user(s) objects");

                }
                catch (Exception ex)
                {
                    DILog.GeneralLogger.WriteLog(LogLevelType.Error, "UserInfo update Error", ex);
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Error, "UserInfo Error", ex);
                throw;
            }
            finally
            {
                m_iacmOperator.Close();

                selectCmd.Dispose();

                updateCmd.Dispose();
            }
        }

        #endregion

        public void DeleteAll<T>()
        {
            try
            {
                DeleteAll<T>(null, null);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void DeleteAll<T>(string _tenantId, long? _endpointSystemId)
        {
            int noAffected = -1;

            string strTableName = string.Empty;

            try
            {
                Type type = typeof(T);

                switch (type.Name)
                {
                    case "ADSUserInfo":
                        strTableName = "DI_ADS_UserInfo";
                        break;
                    case "ADSGroupInfo":
                        strTableName = "DI_ADS_GroupInfo";
                        break;
                    case "ADSOuInfo":
                        strTableName = "DI_ADS_OuInfo";
                        break;
                    case "ADSComputerInfo":
                        strTableName = "DI_ADS_ComputerInfo";
                        break;
                    case "ADSPrinterInfo":
                        strTableName = "DI_ADS_PrinterInfo";
                        break;
                    case "ADSSharedFolderInfo":
                        strTableName = "DI_ADS_SharedFolderInfo";
                        break;
                    case "User":
                        strTableName = "DI_M_User";
                        break;
                    case "ADSUserGroup":
                        strTableName = "DI_ADS_UserGroup";
                        break;
                    case "ADSComputerGroup":
                        strTableName = "DI_ADS_ComputerGroup";
                        break;
                    case "LinuxUserGroup":
                        strTableName = "DI_Linux_UserGroupMap";
                        break;
                    case "LinuxGroupInfo":
                        strTableName = "DI_Linux_GroupInfo";
                        break;
                    case "LinuxResourceInfo":
                        strTableName = "DI_Linux_ResourceInfo";
                        break;
                    case "":
                    default:
                        break;
                }

                string strQuery = string.Format("DELETE {0}", strTableName);

                if (_tenantId != null && _endpointSystemId != null)
                {
                    strQuery = string.Format("{0} WHERE TenantId = '{1}' AND EndpointSystemId={2}", strQuery, _tenantId, _endpointSystemId);
                }

                DILog.GeneralLogger.WriteLog(LogLevelType.Info,string.Format("Query {0}",
                                                                strQuery));

                using (_context = new IACMDBContext())
                {
                    _context.Database.ExecuteSqlCommand(strQuery);
                }


                DILog.GeneralLogger.WriteLog(LogLevelType.Debug, string.Format("Deleted {0} records from table {1}",
                                                              noAffected,
                                                              strTableName));

                if (noAffected <= 0)
                {
                }

            }
            catch (Exception)
            {

                throw;
            }


        }

       
        private static List<EdmProperty> PopulateColumns(Type entityType)
        {
            IACMDBContext dbcontext = new IACMDBContext();


            var _objectContext = ((IObjectContextAdapter)dbcontext.ADSConfiguration).ObjectContext;

            var mdw = _objectContext.MetadataWorkspace.GetItems(DataSpace.CSpace);

            return mdw
                .Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .SelectMany(meta => ((EntityType)meta).Properties)
                .Where(p => p.DeclaringType.Name == entityType.Name)
                .ToList();
        }


        public IList GetColumnsList(string TableName)
        {
            IACMDBContext dbcontext = new IACMDBContext();

            ObjectContext ctx = ((IObjectContextAdapter)dbcontext).ObjectContext;
            var cols = from meta in ctx.MetadataWorkspace.GetItems(DataSpace.CSpace)
                       .Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                       from p in (meta as EntityType).Properties
                          .Where(p => p.DeclaringType.Name == TableName)
                       select new
                       {
                           ColumnName = p.Name,
                           DataType = p.TypeUsage.EdmType.Name
                       };

            return cols.ToList();
        }
       

     
       

        #region Private Methods
        private List<SqlParameter> GetUserParamsForAttributeMap()
        {
            SqlParameter sqlParam = null;
            List<SqlParameter> parameters = new List<SqlParameter>();

            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@UserId";
            sqlParam.SqlDbType = SqlDbType.Int;
            sqlParam.SourceColumn = "UserId";
            sqlParam.Value = 1;
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@DIUserId";
            sqlParam.SqlDbType = SqlDbType.Int;
            sqlParam.SourceColumn = "DIUserId";
            parameters.Add(sqlParam);

            return parameters;
        }
        private List<SqlParameter> GetUserParams()
        {
            SqlParameter sqlParam = null;
            List<SqlParameter> parameters = new List<SqlParameter>();

            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@UserId";
            sqlParam.SqlDbType = SqlDbType.Int;
            sqlParam.SourceColumn = "UserId";
            sqlParam.Value = 1;
            parameters.Add(sqlParam);

            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@TenantId";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.SourceColumn = "TenantId";
            parameters.Add(sqlParam);

            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@EndPointSystemId";
            sqlParam.SqlDbType = SqlDbType.Int;
            sqlParam.SourceColumn = "EndPointSystemId";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@cn";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "cn";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@sn";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "sn";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@st";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "st";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@title";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "title";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@description";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "description";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@postalAddress";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "postalAddress";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@postalCode";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "postalCode";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@postOfficeBox";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "postOfficeBox";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@physicalDeliveryOfficeName";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "physicalDeliveryOfficeName";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@telephoneNumber";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "telephoneNumber";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@facsimileTelephoneNumber";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "facsimileTelephoneNumber";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@givenName";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "givenName";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@initials";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "initials";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@distinguishedName";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "distinguishedName";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@instanceType";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "instanceType";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@whenCreated";
            sqlParam.SqlDbType = SqlDbType.DateTime;
            sqlParam.SourceColumn = "whenCreated";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@whenChanged";
            sqlParam.SqlDbType = SqlDbType.DateTime;
            sqlParam.SourceColumn = "whenChanged";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@displayName";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "displayName";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@uSNCreated";
            sqlParam.SqlDbType = SqlDbType.BigInt;
            sqlParam.SourceColumn = "uSNCreated";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@info";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "info";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@uSNChanged";
            sqlParam.SqlDbType = SqlDbType.BigInt;
            sqlParam.SourceColumn = "uSNChanged";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@co";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "co";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@department";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "department";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@company";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "company";
            parameters.Add(sqlParam);

            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@manager";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "manager";
            parameters.Add(sqlParam);

            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@streetAddress";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "streetAddress";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@wWWHomePage";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "wWWHomePage";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@name";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "name";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@objectGUID";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "objectGUID";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@userAccountControl";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "userAccountControl";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@IsAccountDisabled";
            sqlParam.SqlDbType = SqlDbType.Bit;
            sqlParam.SourceColumn = "IsAccountDisabled";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@IsAccountLocked";
            sqlParam.SqlDbType = SqlDbType.Bit;
            sqlParam.SourceColumn = "IsAccountLocked";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@IsPasswordRequired";
            sqlParam.SqlDbType = SqlDbType.Bit;
            sqlParam.SourceColumn = "IsPasswordRequired";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@PasswordExpirationDate";
            sqlParam.SqlDbType = SqlDbType.DateTime;
            sqlParam.SourceColumn = "PasswordExpirationDate";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@badPwdCount";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "badPwdCount";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@codePage";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "codePage";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@countryCode";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "countryCode";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@badPasswordTime";
            sqlParam.SqlDbType = SqlDbType.DateTime;
            sqlParam.SourceColumn = "badPasswordTime";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@lastLogoff";
            sqlParam.SqlDbType = SqlDbType.DateTime;
            sqlParam.SourceColumn = "lastLogoff";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@lastLogon";
            sqlParam.SqlDbType = SqlDbType.DateTime;
            sqlParam.SourceColumn = "lastLogon";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@pwdLastSet";
            sqlParam.SqlDbType = SqlDbType.DateTime;
            sqlParam.SourceColumn = "pwdLastSet";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@primaryGroupID";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "primaryGroupID";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@objectSid";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "objectSid";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@accountExpires";
            sqlParam.SqlDbType = SqlDbType.DateTime;
            sqlParam.SourceColumn = "accountExpires";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@logonCount";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "logonCount";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@sAMAccountName";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "sAMAccountName";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@sAMAccountType";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "sAMAccountType";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@managedObjects";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "managedObjects";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@ipPhone";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "ipPhone";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@objectCategory";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "objectCategory";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@mail";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "mail";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@homePhone";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "homePhone";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@mobile";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "mobile";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@pager";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "pager";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@nTSecurityDescriptor";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "nTSecurityDescriptor";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@DIUserId";
            sqlParam.SqlDbType = SqlDbType.Int;
            sqlParam.SourceColumn = "DIUserId";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@CreatedBy";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = 50;
            sqlParam.SourceColumn = "CreatedBy";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@CreatedDate";
            sqlParam.SqlDbType = SqlDbType.DateTime;
            sqlParam.SourceColumn = "CreatedDate";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@LastModifiedBy";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = 50;
            sqlParam.SourceColumn = "LastModifiedBy";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@LastModifiedDate";
            sqlParam.SqlDbType = SqlDbType.DateTime;
            sqlParam.SourceColumn = "LastModifiedDate";
            parameters.Add(sqlParam);

            return parameters;
        }

        private DataTable getUserInfoTable()
        {
            DataTable table = new DataTable();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(ADSUserInfo));

            foreach (PropertyDescriptor prop1 in properties)
            {
                if (prop1.PropertyType.Namespace == "System")
                {
                    table.Columns.Add(prop1.Name, Nullable.GetUnderlyingType(prop1.PropertyType) ?? prop1.PropertyType);
                }
                else
                {

                    Type entityType1 = prop1.PropertyType;
                    PropertyDescriptorCollection properties1 = TypeDescriptor.GetProperties(entityType1);

                    foreach (PropertyDescriptor prop11 in properties1)
                    {
                        table.Columns.Add(string.Format("{0}.{1}", prop1.Name, prop11.Name), Nullable.GetUnderlyingType(prop11.PropertyType) ?? prop11.PropertyType);
                    }

                }
            }

            //DataColumn dcol = table.Columns["UserId"];
            //dcol.AutoIncrement = true;
            //dcol.AutoIncrementSeed = 1;
            //dcol.AutoIncrementStep = 1;

            //dcol = table.Columns["DIUserId"];
            //dcol.AutoIncrement = true;
            //dcol.AutoIncrementSeed = 1;
            //dcol.AutoIncrementStep = 1;

            return table;

        }

        #endregion

        #region Provisioning Methods

        public bool UpdateADSUserInfoFromProvisioning(ADSUserInfo adsUserInfo)
        {
            bool isDataSaved = false;
            if (adsUserInfo == null)
                return isDataSaved;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var reviewEPEntry = _context.Entry(adsUserInfo);
                    if (reviewEPEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<ADSUserInfo>();
                        ADSUserInfo attachedreviewEP = set.Find(adsUserInfo.Id);

                        if (attachedreviewEP != null)
                        {
                            var attachedEntry = _context.Entry(attachedreviewEP);
                            attachedEntry.CurrentValues.SetValues(adsUserInfo);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                DILog.GeneralLogger.WriteLog(LogLevelType.Error, "Error occur while updating  ADSUserInfo From Provisioning.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }
        #endregion

        #region  NEW
        public IList<ADSUserInfo> GetAllADUsers(int endpointId)
        {
            using (_context = new IACMDBContext())
            {
                IList<ADSUserInfo> listAdUsers = new List<ADSUserInfo>();
                var allAdsusers = _context.ADSUserInfos.ToList();
                listAdUsers = allAdsusers.Where(e => e.EndPointSystemId == endpointId).ToList();
                return listAdUsers;

            }
        }

        public void SaveUsers(IList<ADSUserInfo> list)
        {
            using (_context = new IACMDBContext())
            {
                IList<ADSUserInfo> listExistingADusers = new List<ADSUserInfo>();
                ADSUserInfo adexistingUser = null;
                listExistingADusers = _context.ADSUserInfos.ToList();
                foreach (ADSUserInfo usr in list)
                {
                    adexistingUser = listExistingADusers.Where(t => t.sAMAccountName == usr.sAMAccountName).FirstOrDefault();
                    if (adexistingUser == null)
                    {
                        _context.ADSUserInfos.Add(usr);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.ADSUserInfos.Attach(usr);

                        var entry = _context.Entry(usr);
                        entry.State = EntityState.Modified;

                        entry.Property(e => e.Id).IsModified = false;
                        entry.Property(e => e.DIUserId).IsModified = false;
                        entry.Property(e => e.CreatedDate).IsModified = false;
                        entry.Property(e => e.CreatedBy).IsModified = false;
                    }
                }
                _context.SaveChanges();

            }
        }

        public Dictionary<long, string> GetADSGroupInfo(long EndPointID)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    return _context.ADSGroupInfos.Where(ep => ep.EndPointSystemId == EndPointID).ToDictionary(item => (long)item.Id, item => item.distinguishedName);
                }
            }
            catch
            {
                throw;
            }
        }

        public void SaveGroups(IList<ADSGroupInfo> list)
        {
            try
            {


                using (_context = new IACMDBContext())
                {
                    IList<ADSGroupInfo> listExistingADgroups = new List<ADSGroupInfo>();
                    ADSGroupInfo adexistingGroups = null;
                    listExistingADgroups = _context.ADSGroupInfos.ToList();
                    foreach (ADSGroupInfo grp in list)
                    {
                        adexistingGroups = listExistingADgroups.Where(t => t.sAMAccountName == grp.sAMAccountName).FirstOrDefault();
                        if (adexistingGroups == null)
                        {
                            _context.ADSGroupInfos.Add(grp);
                            _context.SaveChanges();

                        }

                        else
                        {
                            grp.Id = adexistingGroups.Id;
                            grp.description = adexistingGroups.description;
                            grp.CreatedDate = adexistingGroups.CreatedDate;
                            grp.CreatedBy = adexistingGroups.CreatedBy;
                            _context.Entry(adexistingGroups).CurrentValues.SetValues(grp);
                        }

                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ADSUserInfo getADUserByObjectSID(string paramObjectSID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.ADSUserInfos.FirstOrDefault(usr => usr.objectSid == paramObjectSID);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ADSGroupInfo getADGroupByObjectSID(string paramObjectSID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.ADSGroupInfos.FirstOrDefault(grp => grp.objectSid == paramObjectSID);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion


        public void SaveUserGroupMap(Dictionary<long, string> dicUsers, Dictionary<long, string> dicGroups, Dictionary<string, ArrayList> dicGroupUsers, long endpointSystemid)
        {
            using (_context = new IACMDBContext())
            {
                _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM ADSUserInfoADSGroupInfo WHERE EndpointSystemId = {0}", endpointSystemid));
            }

            IList<ADUserGroup> listUG = new List<ADUserGroup>();

            ADUserGroup userGroup = null;

            foreach (var group in dicGroupUsers)
            {

                foreach (var user in group.Value)
                {
                    userGroup = new ADUserGroup();

                    userGroup.ADSGroupInfo_Id = (int)dicGroups.FirstOrDefault(x => x.Value == group.Key).Key;
                    userGroup.ADSUserInfo_Id = (int)dicUsers.FirstOrDefault(x => x.Value.ToString().ToUpper() == user.ToString().ToUpper()).Key;
                    userGroup.EndpointSystemId = (int)endpointSystemid;
                    userGroup.Version = Convert.ToInt16(VersionNo.Available);

                    if (userGroup.ADSUserInfo_Id != 0)
                    {
                        var usrGrp = listUG.FirstOrDefault(item => item.ADSGroupInfo_Id == userGroup.ADSGroupInfo_Id && item.ADSUserInfo_Id == userGroup.ADSUserInfo_Id);
                        if (usrGrp == null)
                        {
                            listUG.Add(userGroup);
                        }
                    }
                }
            }

            SQLBulkUploader<ADUserGroup> sqlUserGroupMap = null;
            try
            {
                string tableName = "ADSUserInfoADSGroupInfo";

                sqlUserGroupMap = new SQLBulkUploader<ADUserGroup>(tableName);

                sqlUserGroupMap.Flush(listUG);



            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlUserGroupMap.Close();
            }

        }

        public ADSUserInfo GetUserBysAMAccountName(string sAMAccountName)
        {
            using (_context = new IACMDBContext())
            {
                return _context.ADSUserInfos.FirstOrDefault(t => t.sAMAccountName == sAMAccountName);
            }
        }

        public ADSUserInfo GetUserBysAMAccountNameAndEndpoint(string sAMAccountName, long endpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.ADSUserInfos.FirstOrDefault(t => t.sAMAccountName == sAMAccountName && t.EndPointSystemId == endpointSystemId);
            }
        }

        public void AddADsUser(ADSUserInfo user)
        {
            using (_context = new IACMDBContext())
            {
                _context.ADSUserInfos.Add(user);
                _context.SaveChanges();
            }
        }
        public void AddADsGroup(ADSGroupInfo group)
        {
            using (_context = new IACMDBContext())
            {
                _context.ADSGroupInfos.Add(group);
                _context.SaveChanges();
            }
        }
        public void UpdateADsUser(ADSUserInfo user)
        {
            using (_context = new IACMDBContext())
            {
                _context.ADSUserInfos.Attach(user);
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public void UpdateADsGroup(ADSGroupInfo group)
        {
            using (_context = new IACMDBContext())
            {
                _context.ADSGroupInfos.Attach(group);
                _context.Entry(group).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public Dictionary<long, string> getUserIds(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.ADSUserInfos.Where(ep => ep.EndPointSystemId == EndpointId).ToDictionary(item => item.Id, item => item.distinguishedName);
            }
        }

        public void RemoveADsUser(ADSUserInfo user)
        {
            using (_context = new IACMDBContext())
            {
                ADSUserInfo adsuser = new ADSUserInfo();
                adsuser = _context.ADSUserInfos.First(u => u.Id == user.Id);
                _context.ADSUserInfos.Remove(adsuser);
                _context.SaveChanges();
            }
        }

        public IList<ADSUserInfo> GetDeletedUsers(long EndpointId, Process objDiscoveryProcess)
        {
            using (_context = new IACMDBContext())
            {
                IList<ADSUserInfo> listDeletedUsers = new List<ADSUserInfo>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedUsers = _context.ADSUserInfos.Where(d => d.EndPointSystemId == EndpointId && d.LastModifiedDate < objDiscoveryProcess.ProcessStartAt).ToList();
                }

                return listDeletedUsers;

            }
        }


        public List<ADSUserInfo> GetAllUsersByGroup(string GroupCN)
        {
            List<ADSUserInfo> users = null;// _context.ADSUserGroups.Where(s => s.GroupCN == GroupCN).ToList();

            ADSGroupInfo group = _context.ADSGroupInfos.First(s => s.name == GroupCN);

            if (group != null)
                users = group.ADSUsers;

            return users;
        }

        public List<ADSUserInfo> GetAllUsersByGroup(int GroupId)
        {
            List<ADSUserInfo> users = null;// _context.ADSUserGroups.Where(s => s.GroupCN == GroupCN).ToList();

            ADSGroupInfo group = _context.ADSGroupInfos.Find(GroupId);

            if (group != null)
                users = group.ADSUsers;

            return users;
        }        

        public List<ADSUserInfo> GetAllUsersByGroup(string GroupCN, long EndpointSystemId)
        {
            List<ADSUserInfo> users = null;

            ADSGroupInfo group = _context.ADSGroupInfos.Include("ADSUsers").First(s => s.name == GroupCN && s.EndPointSystemId == EndpointSystemId);

            if (group != null)
                users = group.ADSUsers;

            return users;
        }


        public void RemoveADsGroup(ADSGroupInfo adGroup)
        {
            using (_context = new IACMDBContext())
            {
                ADSGroupInfo adsGroup = new ADSGroupInfo();
                adsGroup = _context.ADSGroupInfos.First(u => u.Id == adGroup.Id);
                _context.ADSGroupInfos.Remove(adsGroup);
                _context.SaveChanges();
            }
        }

        public IList<ADSGroupInfo> GetDeletedGroups(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<ADSGroupInfo> listDeletedGroups = new List<ADSGroupInfo>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                     Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                     if (process != null)
                     {
                         listDeletedGroups = _context.ADSGroupInfos.Where(d => d.EndPointSystemId == EndpointId && d.LastModifiedDate < process.ProcessStartAt).ToList();
                     }
                }

                return listDeletedGroups;

            }
        }


        public ADSGroupInfo GetGroupBysAMAccountName(string sAMAccountName)
        {
            using (_context = new IACMDBContext())
            {
                return _context.ADSGroupInfos.FirstOrDefault(t => t.sAMAccountName == sAMAccountName);
            }
        }

        public ADSGroupInfo GetGroupBycn(string cn)
        {
            using (_context = new IACMDBContext())
            {
                return _context.ADSGroupInfos.FirstOrDefault(t => t.cn == cn);
            }
        }

        public ADSGroupInfo GetGroupBycnAndEndpoint(string cn, long endpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.ADSGroupInfos.FirstOrDefault(t => t.cn == cn && t.EndPointSystemId == endpointSystemId);
            }
        }

        public IList<ADSUserToGroupsReportData> GetADSUserToGroupsReportData(int EndPointSystemID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from adsUser in _context.ADSUserInfos
                            from adsGrp in adsUser.ADSGroups
                            where adsGrp.groupTypeDesc == Constants.SECURITY_GROUP && adsGrp.EndPointSystemId == EndPointSystemID
                            select new ADSUserToGroupsReportData
                            {
                                UserName = adsUser.sAMAccountName,
                                UserOU = adsUser.ou,
                                DisplayName = adsUser.displayName,
                                AccountCreatedDate = adsUser.whenCreated.Value,
                                PasswordExpirationDate = adsUser.PasswordExpirationDate.Value,
                                LastLoginDate = adsUser.lastLogon.Value,
                                Disabled = adsUser.IsAccountDisabled ? "YES" : "NO",
                                Locked = adsUser.IsLocked ? "YES" : "NO",
                                Email = adsUser.mail,
                                Department = adsUser.department,
                                GroupName = adsGrp.sAMAccountName,
                                GroupDescription = adsGrp.description,
                                GroupOU = adsGrp.distinguishedName
                            }
                            ).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateUser(ADSUserInfo _adsUser)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    _context.ADSUserInfos.Attach(_adsUser);
                    _context.Entry(_adsUser).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<ADSUserInfoHistory> GetADSHistoryUsersEndpointSystemID(int EndPointSystemID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSUserInfoHistorys.Where(lq => lq.EndPointSystemId == EndPointSystemID).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while getting All AD History Users ", ex);
                throw ex;
            }
        }

        public IList<EndpointObjects> GetADObjects()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    EndPointSystemType ep = _context.EndpointSystemTypes.Where(e => e.EndpointSystemTypeName == EndpointType.ActiveDirectory).FirstOrDefault();
                    return _context.EndpointObjects.Where(e => e.EndpointSystemTypeId == ep.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while getting All AD EndpointObjects ", ex);
                throw ex;
            }
        }

        public IList<ADSUsersReportData> GetADSUsersReportDataByEndpointSystem(int EndpointsystemID)
        {
            IList<ADSUserInfo> _usrlist = new List<ADSUserInfo>();
            IList<ADSUsersReportData> _usrReportDatalist = new List<ADSUsersReportData>();
            ADSUsersReportData _usrReportDataItem = new ADSUsersReportData();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _usrlist = GetADSUserInfoByEndpointSystem(EndpointsystemID);
                    foreach (ADSUserInfo _usr in _usrlist)
                    {
                        _usrReportDataItem = new ADSUsersReportData();
                        _usrReportDataItem.UserId = _usr.Id;
                        _usrReportDataItem.sAMAccountName = _usr.sAMAccountName;
                        _usrReportDataItem.displayName = _usr.displayName;
                        _usrReportDataItem.physicalDeliveryOfficeName = _usr.physicalDeliveryOfficeName;
                        _usrReportDataItem.company = _usr.company;
                        _usrReportDataItem.pager = _usr.pager;
                        _usrReportDataItem.businessCategory = _usr.businessCategory;
                        _usrReportDataItem.division = _usr.division;
                        _usrReportDataItem.mail = _usr.mail;
                        _usrReportDataItem.title = _usr.title;
                        _usrReportDataItem.IsLocked = _usr.IsLocked ? "Yes" : "No";
                        _usrReportDataItem.LockScenario = _usr.LockScenario;
                        _usrReportDataItem.whenCreated = _usr.whenCreated.HasValue ? _usr.whenCreated.Value.ToString() : string.Empty;
                        _usrReportDataItem.PasswordExpirationDate = _usr.PasswordExpirationDate.HasValue ? _usr.PasswordExpirationDate.Value.ToString() : string.Empty;
                        _usrReportDataItem.lastLogon = _usr.lastLogon.HasValue ? _usr.lastLogon.Value.ToString() : string.Empty;
                        _usrReportDataItem.pwdLastSet = _usr.pwdLastSet.HasValue ? _usr.pwdLastSet.Value.ToString() : string.Empty;
                        _usrReportDataItem.accountExpires = _usr.accountExpires.HasValue ? _usr.accountExpires.Value.ToString() : string.Empty;
                        _usrReportDataItem.logonCount = _usr.logonCount;
                        _usrReportDataItem.IsAccountDisabled = _usr.IsAccountDisabled;
                        _usrReportDataItem.IsAccountLocked = _usr.IsLocked;
                        _usrReportDataItem.cn = _usr.cn;
                        _usrReportDataItem.OU = _usr.ou;
                        _usrReportDataItem.HostName = _context.EndpointSystems.FirstOrDefault(ep => ep.Id == _usr.EndPointSystemId).DisplayName;
                        _usrReportDataItem.Institution = _usr.TenantId;
                        _usrReportDataItem.IsMapped = (_usr.DIUserId != null) ? "Yes" : "No";
                        _usrReportDataItem.Owner = (_usr.DIUserId != null) ? _context.Users.FirstOrDefault(ep => ep.Id == _usr.DIUserId).DisplayName : "";
                        _usrReportDatalist.Add(_usrReportDataItem);
                    }
                }
                return _usrReportDatalist;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while getting All ADUsers ", ex);
                throw ex;
            }
        }



        public IList<ADSOUsReportData> GetADSOUInfoReportDataByEndPointSystemID(int EndpointSystemID)
        {
            IList<ADSOuInfo> _oulist = new List<ADSOuInfo>();
            IList<ADSOUsReportData> _ouReportDatalist = new List<ADSOUsReportData>();
            ADSOUsReportData _ouReportDataItem = new ADSOUsReportData();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _oulist = GetADSOUInfoByEndPointSystemID(EndpointSystemID);
                    foreach (ADSOuInfo _ou in _oulist)
                    {
                        _ouReportDataItem = new ADSOUsReportData();
                        _ouReportDataItem.OuId = _ou.Id;
                        _ouReportDataItem.name = _ou.name;
                        _ouReportDataItem.description = _ou.description;
                        _ouReportDataItem.whenCreated = _ou.whenCreated.HasValue ? _ou.whenCreated.Value.ToString() : string.Empty;
                        _ouReportDataItem.whenChanged = _ou.whenChanged.HasValue ? _ou.whenChanged.Value.ToString() : string.Empty;
                        _ouReportDataItem.HostName = _context.EndpointSystems.FirstOrDefault(ep => ep.Id == _ou.EndPointSystemId).DisplayName;
                        _ouReportDataItem.Institution = _ou.TenantId;
                        _ouReportDatalist.Add(_ouReportDataItem);
                    }
                }
                return _ouReportDatalist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ADSUserInfo> GetUsersInternetAccess()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from adsUser in _context.ADSUserInfos
                            from adsGroup in adsUser.ADSGroups
                            where adsGroup.name == "InternetUsers"
                            select adsUser).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

      

        public bool CheckADUserNameExist(string samaAccountName)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSUserInfos.Any(item => item.sAMAccountName.Equals(samaAccountName, StringComparison.CurrentCultureIgnoreCase));
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<TableProperties> GetColumnList(string TableName)
        {
            IACMDBContext dbcontext = new IACMDBContext();

            ObjectContext ctx = ((IObjectContextAdapter)dbcontext).ObjectContext;
            var cols = from meta in ctx.MetadataWorkspace.GetItems(DataSpace.CSpace)
                       .Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                       from p in (meta as EntityType).Properties
                          .Where(p => p.DeclaringType.Name == TableName)
                       select new TableProperties
                       {
                           ColumName = p.Name,
                           DataType = p.TypeUsage.EdmType.Name
                       };


            return cols.ToList();
        }

        public ADSUserInfo GetPrimaryADUserByDIUserId(long diuserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSUserInfos.FirstOrDefault(item => item.DIUserId == diuserId && item.AccountType == AccountType.User);
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<UserAccount> GetAccountDetail(int UserID)
        {
            IList<UserAccount> Listtobereturn = new List<UserAccount>();
            IList<ADSUserInfo> listADSUserInfo = new List<ADSUserInfo>();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                listADSUserInfo = oIACMDBContext.ADSUserInfos.Where(adsusr => adsusr.DIUserId == UserID && adsusr.IsLocked == false).ToList();
                foreach (ADSUserInfo adusr in listADSUserInfo)
                {
                    if (adusr != null)
                    {
                        UserAccount UserAccount = new UserAccount
                        {
                            EndpointUserId = adusr.Id,
                            UserName = adusr.AccountName,
                            FullName = adusr.displayName,
                            AccountType = adusr.AccountType,
                            LastLogonDate = adusr.lastLogon,
                            EndpointSystemId = adusr.EndPointSystemId,
                            EndPointSystemType = adusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = adusr.EndPointSystem.DisplayName
                        };
                        Listtobereturn.Add(UserAccount);
                    }
                }
            }
            return Listtobereturn;
        }


        public IList<UserEntitlement> GetEntitlementDetail(long EndpointUserId)
        {
            IList<UserEntitlement> Listtobereturn = new List<UserEntitlement>();
            ADSUserInfo ADSUserInfo = new ADSUserInfo();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                ADSUserInfo = oIACMDBContext.ADSUserInfos.Include(lq => lq.EndPointSystem).Include(lq => lq.EndPointSystem.EndPointSystemType).Include(lq => lq.ADSGroups).Where(adsusr => adsusr.Id == EndpointUserId).FirstOrDefault();
                if (ADSUserInfo == null)
                    return Listtobereturn;
                
                foreach (ADSGroupInfo grp in ADSUserInfo.ADSGroups)
                {
                    UserEntitlement UserEntitlement = new UserEntitlement();
                    UserEntitlement.EndpointUserId = ADSUserInfo.Id;
                    UserEntitlement.UserName = ADSUserInfo.AccountName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                    UserEntitlement.EndPointSystemType = ADSUserInfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = ADSUserInfo.EndPointSystem.DisplayName;
                    if (grp != null)
                    {
                        UserEntitlement.Name = grp.name;
                        UserEntitlement.Description = grp.description;
                        Listtobereturn.Add(UserEntitlement);
                    }
                }
            }
            return Listtobereturn;
        }

        public IList<UserAccount> GetUserAccountDetailByApplication(int ApplicationID)
        {
            IList<UserAccount> Listtobereturn = new List<UserAccount>();
            List<ADSUserInfo> listADSUserInfo = new List<ADSUserInfo>();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                listADSUserInfo = oIACMDBContext.ADSUserInfos.Include(lq => lq.EndPointSystem).Include(lq => lq.EndPointSystem.EndPointSystemType).Where(adsusr => adsusr.EndPointSystemId == ApplicationID && adsusr.IsLocked == false).ToList();
                foreach (ADSUserInfo adusr in listADSUserInfo)
                {
                    if (adusr != null)
                    {
                        UserAccount UserAccount = new UserAccount
                        {
                            EndpointUserId = adusr.Id,
                            UserName = adusr.AccountName,
                            FullName = adusr.displayName,
                            AccountType = adusr.AccountType,
                            LastLogonDate = adusr.lastLogon,
                            EndpointSystemId = adusr.EndPointSystemId,
                            EndPointSystemType = adusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = adusr.EndPointSystem.DisplayName
                        };
                        Listtobereturn.Add(UserAccount);
                    }
                }
            }
            return Listtobereturn;
        }

        public AccountDetail GetAccountDetailByEndPointUserID(int EndpointUserId)
        {
            AccountDetail AccountDetail = new AccountDetail();
            ADSUserInfo ADSUserInfo = new ADSUserInfo();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                ADSUserInfo = oIACMDBContext.ADSUserInfos.Where(adsusr => adsusr.Id == EndpointUserId).FirstOrDefault();
                if (ADSUserInfo == null)
                        return AccountDetail;
                    AccountDetail.LoginName = ADSUserInfo.AccountName;
                    AccountDetail.DisplayName = ADSUserInfo.displayName;
                    AccountDetail.Description = ADSUserInfo.description;
                    AccountDetail.PhoneNo = ADSUserInfo.homePhone;
                    AccountDetail.Email = ADSUserInfo.mail;
                    AccountDetail.AccountType = ADSUserInfo.AccountType;
                    AccountDetail.IsMappedAccount = ADSUserInfo.DIUserId == null ? false : true;
                    AccountDetail.LastLogonDate = ADSUserInfo.lastLogon;
                    AccountDetail.Version = Convert.ToString(ADSUserInfo.Version);
                    AccountDetail.DIUserID = ADSUserInfo.DIUserId;
                    AccountDetail.EndpointSystemId = ADSUserInfo.EndPointSystemId;
                    AccountDetail.EndpointUserId = EndpointUserId;
            }
            return AccountDetail;
        }

        public void UpdateAccountDetail(AccountDetail AccountDetail)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                User DIUser = null;
                ADSUserInfo ADSUInfo = Context.ADSUserInfos.FirstOrDefault(ENDPT => ENDPT.Id == AccountDetail.EndpointUserId && ENDPT.EndPointSystemId == AccountDetail.EndpointSystemId);
                if (AccountDetail.DIUserID != null)
                    DIUser = Context.Users.FirstOrDefault(diusr => diusr.Id == AccountDetail.DIUserID);
                if (ADSUInfo != null)
                {
                    ADSUInfo.MappingType = "Manual";
                    ADSUInfo.DIUserId = AccountDetail.DIUserID;
                    ADSUInfo.AccountType = AccountDetail.AccountType;
                    ADSUInfo.IfUnmappedMapTo = AccountDetail.UnMappedMapTo;
                    ADSUInfo.LastModifiedDate = DateTime.Today;
                    if (AccountDetail.AccountType == AccountType.User && DIUser != null)
                        DIUser.UserName = ADSUInfo.sAMAccountName;
                    Context.SaveChanges();
                }
            }
        }

        public ADSUserInfo GetADSUserInfo(long EndpointUserId)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                return Context.ADSUserInfos.Include("ADSGroups").Where(adsusr => adsusr.Id == EndpointUserId).FirstOrDefault();
            }
        }

        public List<AccessRequestItem>  GetAccessRequestItem(string searchString, bool isCatalog)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {

                if (isCatalog)
                {
                    return (from catalogItem in Context.ADSCatalogs
                            where catalogItem.BusinessRole.Contains(searchString)
                            orderby catalogItem.BusinessRole
                            select new AccessRequestItem
                            {
                                AccessRequestItemType = AccessRequestItemType.Entitlement,
                                AccessItemType = Model.AccessItemType.Catalog,
                                EndpointType = EndpointType.ActiveDirectory,
                                KeyId = catalogItem.Id,
                                EndpointSystemId = catalogItem.EndpointSystemId,
                                EndpointSystemTypeId = (int)EndpointType.ActiveDirectory,
                                ItemName = catalogItem.BusinessRole,
                                ItemDetail = catalogItem.TechnicalRole,
                                ItemDescription = catalogItem.Description
                            }).ToList();
                }
                return (from groups in Context.ADSGroupInfos
                                                 where groups.sAMAccountName.Contains(searchString)
                                                 orderby groups.name
                                                 select new AccessRequestItem
                                                 {
                                                     AccessRequestItemType = AccessRequestItemType.Entitlement,
                                                     EntitlementType = Infrastructure.Enums.EntitlementType.Group,
                                                     EndpointType = EndpointType.ActiveDirectory,
                                                     KeyId = groups.Id,
                                                     EndpointSystemId = groups.EndPointSystemId,
                                                     EndpointSystemTypeId = (int)EndpointType.ActiveDirectory,
                                                     ItemName = groups.sAMAccountName,
                                                     ItemDescription = groups.description,
                                                     ItemDetail = groups.description
                                                 }).ToList();

                
            }
        }
        public List<AccessRequestItem> GetAccessRequestItemForRemoveRequest(string searchString, long? DIUserId)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                if (DIUserId == null)
                    return null;

                var listApp = GetADUserByDIUserId(DIUserId.Value);

                var listEnt = GetADGroupsByDIUserId(DIUserId.Value);


                var listARApp = (from apps in listApp
                                 where apps.sAMAccountName.Contains(searchString)
                                 orderby apps.sAMAccountName
                                 select new AccessRequestItem
                                 {
                                     AccessRequestItemType = AccessRequestItemType.Application,
                                     EndpointType = EndpointType.ActiveDirectory,
                                     KeyId = apps.EndPointSystemId,
                                     EndpointSystemId = apps.EndPointSystemId,
                                     EndpointSystemTypeId = (int)EndpointType.ActiveDirectory,
                                     ItemName = apps.sAMAccountName,
                                     ItemDescription = apps.displayName
                                 }).ToList();

                var listAREnt = (from groups in listEnt
                        where groups.sAMAccountName.Contains(searchString)
                        orderby groups.name
                        select new AccessRequestItem
                        {
                            AccessRequestItemType = AccessRequestItemType.Entitlement,
                            EndpointType = EndpointType.ActiveDirectory,
                            KeyId = groups.Id,
                            EndpointSystemId = groups.EndPointSystemId,
                            EndpointSystemTypeId = (int)EndpointType.ActiveDirectory,
                            ItemName = groups.sAMAccountName,
                            ItemDescription = groups.description
                        }).ToList();

                return listARApp.Union(listAREnt).ToList();

            }
        }
        public List<AccessRequestItem> GetAccessRequestItemForRemoveRequestSpecailACR(AccessRequestFilterCriteria Criteria, long? DIUserId)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                if (DIUserId == null)
                    return null;

                var listApp = GetADUserByDIUserId(DIUserId.Value, Criteria.AccountType);

                var listEnt = GetADGroupsByDIUserId(DIUserId.Value, Criteria.AccountType);


                var listARApp = (from apps in listApp
                                 where apps.sAMAccountName.Contains(Criteria.SearchKey)
                                 orderby apps.sAMAccountName
                                 select new AccessRequestItem
                                 {
                                     AccessRequestItemType = AccessRequestItemType.Application,
                                     EndpointType = EndpointType.ActiveDirectory,
                                     KeyId = apps.EndPointSystemId,
                                     EndpointSystemId = apps.EndPointSystemId,
                                     EndpointSystemTypeId = (int)EndpointType.ActiveDirectory,
                                     ItemName = apps.sAMAccountName,
                                     ItemDescription = apps.displayName
                                 }).ToList();

                var listAREnt = (from groups in listEnt
                                 where groups.sAMAccountName.Contains(Criteria.SearchKey)
                                 orderby groups.name
                                 select new AccessRequestItem
                                 {
                                     AccessRequestItemType = AccessRequestItemType.Entitlement,
                                     EndpointType = EndpointType.ActiveDirectory,
                                     KeyId = groups.Id,
                                     EndpointSystemId = groups.EndPointSystemId,
                                     EndpointSystemTypeId = (int)EndpointType.ActiveDirectory,
                                     ItemName = groups.sAMAccountName,
                                     ItemDescription = groups.description
                                 }).ToList();

                return listARApp.Union(listAREnt).ToList();

            }
        }
        public bool IsSecondartAccount(long EndpointUserID)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                return _context.ADSUserInfos.Where(item => item.DIUserId == EndpointUserID && item.AccountType == AccountType.User).Count() > 0 ? true : false;
            }
        }

        public List<AccountDetail> GetPwdExpiryUsers()
        {
            List<AccountDetail> Returnvalue = new List<AccountDetail>();
            using (IACMDBContext _context = new IACMDBContext())
            {
                var ADSUserInfo = _context.ADSUserInfos.Include(l => l.EndPointSystem).Where(item => item.PasswordExpirationDate <= DateTime.Now && item.DIUserId > 0).ToList();
                ADSUserInfo.ForEach(lq =>
                    {
                        Returnvalue.Add(new AccountDetail()
                        {
                            LoginName = lq.AccountName,
                            DisplayName = lq.displayName,
                            Description = lq.description,
                            PhoneNo = lq.homePhone,
                            Email = lq.mail,
                            AccountType = lq.AccountType,
                            IsMappedAccount = lq.DIUserId == null ? false : true,
                            LastLogonDate = lq.lastLogon,
                            Version = Convert.ToString(lq.Version),
                            DIUserID = lq.DIUserId,
                            EndpointSystemId = lq.EndPointSystemId,
                            EndpointUserId = lq.Id,
                            ApplicationType = Convert.ToString(EndpointType.ActiveDirectory),
                            Application = lq.EndPointSystem.DisplayName
                        });
                    });
            }
            return Returnvalue;
        }
    }
}