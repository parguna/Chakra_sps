using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using DeepIdentity.Infrastructure;
using System.Collections;
using DeepIdentity.Model;

using DeepIdentity.Model.SAP;
using System.Reflection;
using DeepIdentity.Infrastructure.Enums;
using System.Data.Entity;
using DeepIdentity.Model.ReportData;

namespace DeepIdentity.DataAccess
{
    public class AccessRequestDA
    {
        #region Private Members
        private QueryExecutor executor;
        private IACMDbOperator iacmOperator;
        private IACMDBContext oIACMDBContext = new IACMDBContext();
        #endregion

        #region ADD

        public bool AddAccessRequest(AccessRequest newAR)
        {
            bool isDataSaved = false;
            if (newAR == null)
                return isDataSaved;

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    newAR.ActivationDate = newAR.ActivationDate == DateTime.MinValue ? null : newAR.ActivationDate;
                    newAR.DeactivationDate = newAR.DeactivationDate == DateTime.MinValue ? null : newAR.DeactivationDate;
                    newAR.ExecutionTime = newAR.ExecutionTime == DateTime.MinValue ? null : newAR.ExecutionTime;

                    oIACMDBContext.AccessRequests.Add(newAR);
                    oIACMDBContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while adding new AccessRequest.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        public int AddADSUserInfo(ADSUserInfo adUsr)
        {
            int userID = -1;
            if (adUsr == null)
                return userID;

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    adUsr.whenCreated = adUsr.whenCreated == DateTime.MinValue ? null : adUsr.whenCreated;
                    adUsr.whenChanged = adUsr.whenChanged == DateTime.MinValue ? null : adUsr.whenChanged;
                    adUsr.PasswordExpirationDate = adUsr.PasswordExpirationDate == DateTime.MinValue ? null : adUsr.PasswordExpirationDate;
                    adUsr.badPasswordTime = adUsr.badPasswordTime == DateTime.MinValue ? null : adUsr.badPasswordTime;
                    adUsr.lastLogoff = adUsr.lastLogoff == DateTime.MinValue ? null : adUsr.lastLogoff;
                    adUsr.lastLogon = adUsr.lastLogon == DateTime.MinValue ? null : adUsr.lastLogon;
                    adUsr.pwdLastSet = adUsr.pwdLastSet == DateTime.MinValue ? null : adUsr.pwdLastSet;

                    oIACMDBContext.ADSUserInfos.Add(adUsr);
                    oIACMDBContext.SaveChanges();
                    userID = adUsr.UserId;
                }
            }
            catch (Exception ex)
            {
                userID = -1;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while creating new AD user from Provisioning Manager.", ex);
                throw;
            }
            return userID;
        }

        public bool AddADSUserIntoGroup(ADSUserInfo adUsr)
        {
            bool isDataSaved = false;
            if (!(adUsr != null && adUsr.Groups != null))
                return isDataSaved;

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    ADSUserInfo currentADUser = oIACMDBContext.ADSUserInfos.FirstOrDefault(item => item.UserId == adUsr.UserId);

                    foreach (string grpStr in adUsr.Groups)
                    {
                        IList<ADSGroupInfo> grpList = oIACMDBContext.ADSGroupInfos.Where(gi => gi.cn == grpStr && gi.EndPointSystemId == currentADUser.EndPointSystemId).ToList();
                        foreach (ADSGroupInfo grpItem in grpList)
                        {
                            ADSGroupInfo tmpgrpItem = currentADUser.ADSGroups.Where(g => g.cn == grpItem.cn).FirstOrDefault();
                            if (tmpgrpItem == null)
                            {
                                currentADUser.ADSGroups.Add(grpItem);
                            }
                        }
                        oIACMDBContext.SaveChanges();
                    }

                    string query = "UPDATE ADSUserInfoADSGroupInfo SET EndpointSystemId = " + adUsr.EndPointSystemId + " WHERE ADSUserInfo_UserId = " + adUsr.UserId;

                    oIACMDBContext.Database.ExecuteSqlCommand(query);

                }
                isDataSaved = true;
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while mapping AD User to Group from Provisioning Manager.", ex);
                throw;
            }
            return isDataSaved;
        }

        public bool AddLotusNoteUserIntoGroup(LotusUserInfo lnUsr)
        {
            bool isDataSaved = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    LotusUserInfo currentLNSUser = oIACMDBContext.LotusUserInfos.FirstOrDefault(item => item.UserId == lnUsr.UserId);
                    foreach (string grpStr in lnUsr.Groups)
                    {
                        IList<LotusGroupInfo> grpList = oIACMDBContext.LotusGroupInfos.Where(gi => gi.Group == grpStr && gi.EndPointSystemId == lnUsr.EndPointSystemId).ToList();
                        foreach (LotusGroupInfo grpItem in grpList)
                        {
                            LotusGroupInfo tmpgrpItem = currentLNSUser.LotusGroupInfos.Where(g => g.Group == grpItem.Group).FirstOrDefault();
                            if (tmpgrpItem == null)
                            {
                                currentLNSUser.LotusGroupInfos.Add(grpItem);
                            }
                        }
                        oIACMDBContext.SaveChanges();
                    }
                    string query = "UPDATE LotusUserInfoLotusGroupInfo SET EndpointSystemId = " + lnUsr.EndPointSystemId + " WHERE LotusUserInfo_UserId = " + lnUsr.UserId;

                    oIACMDBContext.Database.ExecuteSqlCommand(query);
                }

                isDataSaved = true;
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while mapping LN User to Group from Provisioning Manager.", ex);
                throw;
            }
            return isDataSaved;
        }

        public List<AccessRequest> GetAccessRequestsByTransactionId(Guid transactionId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.AccessRequests.Where(item => item.TransactionId == transactionId &&
                        item.RequestStatus == AccessRequestStatus.Requested &&
                        item.ProgressStatus == AccessRequestProgress.WaitingforApproval).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        #region SAP
        public int AddSAPUserInfo(SAPUserinfo sapUsr)
        {
            int userID = -1;
            if (sapUsr == null)
                return userID;

            //string query = "DI_spAddNewUserintoSAPUserInfo";
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    sapUsr.PASSWORDEXPTIME = sapUsr.PASSWORDEXPTIME == DateTime.MinValue ? null : sapUsr.PASSWORDEXPTIME;
                    oIACMDBContext.SAPUserinfos.Add(sapUsr);
                    oIACMDBContext.SaveChanges();
                    userID = sapUsr.UserId;
                }
            }
            catch (Exception ex)
            {
                userID = -1;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while creating new SAP user from Provisioning Manager.", ex);
                throw;
            }
            return userID;
        }

        public bool AddSAPUserIntoRoles(SAPUserinfo sapUsr)
        {
            bool isDataSaved = false;
            if (!(sapUsr != null && sapUsr.Roles != null))
                return isDataSaved;

            //string query = "DI_spAddSAPUserIntoRolesMap";
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {

                    SAPUserinfo newSapUser = oIACMDBContext.SAPUserinfos.FirstOrDefault(user => user.UserId == sapUsr.UserId);
                    if (newSapUser != null)
                    {
                        foreach (string strRole in sapUsr.Roles)
                        {
                            SAPRoleInfo roleinfo = oIACMDBContext.SAPRoleInfos.FirstOrDefault(role => role.ROLE_CODE == strRole && role.EndPointSystemId == sapUsr.EndPointSystemId);
                            if (roleinfo != null)
                            {
                                SAPRoleInfo existingrole = newSapUser.SAPRoleInfo.FirstOrDefault(role => role.ROLE_CODE == roleinfo.ROLE_CODE);
                                if (existingrole == null)
                                {
                                    newSapUser.SAPRoleInfo.Add(roleinfo);
                                }
                            }
                        }
                    }
                    oIACMDBContext.SAPUserinfos.Attach(newSapUser);
                    oIACMDBContext.Entry(newSapUser).State = EntityState.Modified;
                    oIACMDBContext.SaveChanges();

                    string query = "UPDATE [SAPRoleInfoSAPUserinfo] SET EndpointSystemId = " + sapUsr.EndPointSystemId + " WHERE SAPUserinfo_UserId = " + sapUsr.UserId;
                    oIACMDBContext.Database.ExecuteSqlCommand(query);
                }
                isDataSaved = true;
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while mapping SAP User to Role from Provisioning Manager.", ex);
                throw;
            }
            return isDataSaved;
        }

        public bool AddSAPUserIntoGroups(SAPUserinfo sapUsr)
        {
            bool isDataSaved = false;
            if (!(sapUsr != null && sapUsr.Groups != null))
                return isDataSaved;

            //string query = "DI_spAddSAPUserIntoRolesMap";
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {

                    SAPUserinfo newSapUser = oIACMDBContext.SAPUserinfos.FirstOrDefault(user => user.UserId == sapUsr.UserId);
                    if (newSapUser != null)
                    {
                        foreach (string strGroup in sapUsr.Groups)
                        {
                            SAPGroupInfo groupInfo = oIACMDBContext.SAPGroupInfos.FirstOrDefault(group => group.GROUP_CODE == strGroup && group.EndPointSystemId == sapUsr.EndPointSystemId);
                            if (groupInfo != null)
                            {
                                SAPGroupInfo existingGroup = newSapUser.SAPGroupInfo.FirstOrDefault(group => group.GROUP_CODE == groupInfo.GROUP_CODE);
                                if (existingGroup == null)
                                {
                                    newSapUser.SAPGroupInfo.Add(groupInfo);
                                }
                            }
                        }
                    }
                    oIACMDBContext.SAPUserinfos.Attach(newSapUser);
                    oIACMDBContext.Entry(newSapUser).State = EntityState.Modified;
                    oIACMDBContext.SaveChanges();

                    string query = "UPDATE [SAPUserinfoSAPGroupInfo] SET EndpointSystemId = " + sapUsr.EndPointSystemId + " WHERE SAPUserinfo_UserId = " + sapUsr.UserId;
                    oIACMDBContext.Database.ExecuteSqlCommand(query);
                }
                isDataSaved = true;
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while mapping SAP User to Role from Provisioning Manager.", ex);
                throw;
            }
            return isDataSaved;
        }

        #endregion

        #region SAP EP
        public int AddSAPEPUserInfo(SAPEPUserInfo sapEPUsr)
        {
            int userID = -1;
            if (sapEPUsr == null)
                return userID;

            //string query = "DI_spAddNewUserintoSAPEPUserInfo";
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (!string.IsNullOrEmpty(sapEPUsr.Password))
                    {
                        sapEPUsr.Password = RSAHelper.EncryptData(sapEPUsr.Password);
                    }
                    sapEPUsr.UserLastLogOnTime = sapEPUsr.UserLastLogOnTime == DateTime.MinValue ? null : sapEPUsr.UserLastLogOnTime;

                    oIACMDBContext.SAPEPUserInfos.Add(sapEPUsr);
                    oIACMDBContext.SaveChanges();
                    userID = sapEPUsr.UserId;
                }
            }
            catch (Exception ex)
            {
                userID = -1;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while creating new SAPEP user from Provisioning Manager.", ex);
                throw;
            }
            return userID;
        }

        public bool AddUserIntoSAPEPUserIntoRoles(SAPEPUserInfo sapEPUsr)
        {
            bool isDataSaved = false;
            if (!(sapEPUsr != null && sapEPUsr.Roles != null))
                return isDataSaved;

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {

                    SAPEPUserInfo newSapEPUser = oIACMDBContext.SAPEPUserInfos.FirstOrDefault(user => user.UserId == sapEPUsr.UserId);
                    if (newSapEPUser != null)
                    {
                        foreach (string role in sapEPUsr.Roles)
                        {
                            SAPEPRoleInfo roleInfo = oIACMDBContext.SAPEPRoleInfos.FirstOrDefault(item => item.UniqueRoleName == role && item.EndPointSystemId == sapEPUsr.EndPointSystemId);
                            if (roleInfo != null)
                            {
                                SAPEPRoleInfo existingRole = newSapEPUser.SAPEPRoles.FirstOrDefault(item => item.UniqueRoleName == roleInfo.UniqueRoleName);
                                if (existingRole == null)
                                {
                                    newSapEPUser.SAPEPRoles.Add(roleInfo);
                                }
                            }
                        }
                    }
                    oIACMDBContext.SAPEPUserInfos.Attach(newSapEPUser);
                    oIACMDBContext.Entry(newSapEPUser).State = EntityState.Modified;
                    oIACMDBContext.SaveChanges();

                    string query = "UPDATE [SAPEPRoleInfoSAPEPUserInfo] SET EndpointSystemId = " + sapEPUsr.EndPointSystemId + " WHERE SAPEPUserInfo_UserId = " + sapEPUsr.UserId;
                    oIACMDBContext.Database.ExecuteSqlCommand(query);
                }
                isDataSaved = true;
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while mapping SAPEP User to Role from Provisioning Manager.", ex);
                throw;
            }
            return isDataSaved;
        }

        public bool AddUserIntoSAPEPUserIntoGroups(SAPEPUserInfo sapEPUsr)
        {
            bool isDataSaved = false;
            if (!(sapEPUsr != null && sapEPUsr.Groups != null))
                return isDataSaved;

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {

                    SAPEPUserInfo newSapEPUser = oIACMDBContext.SAPEPUserInfos.FirstOrDefault(user => user.UserId == sapEPUsr.UserId);
                    if (newSapEPUser != null)
                    {
                        foreach (string group in sapEPUsr.Groups)
                        {
                            SAPEPGroupInfo groupInfo = oIACMDBContext.SAPEPGroupInfos.FirstOrDefault(item => item.GroupUniqueName == group && item.EndPointSystemId == newSapEPUser.EndPointSystemId);
                            if (groupInfo != null)
                            {
                                SAPEPGroupInfo existingGroup = newSapEPUser.SAPEPGroups.FirstOrDefault(item => item.GroupUniqueName == groupInfo.GroupUniqueName);
                                if (existingGroup == null)
                                {
                                    newSapEPUser.SAPEPGroups.Add(groupInfo);
                                }
                            }
                        }
                    }
                    oIACMDBContext.SAPEPUserInfos.Attach(newSapEPUser);
                    oIACMDBContext.Entry(newSapEPUser).State = EntityState.Modified;
                    oIACMDBContext.SaveChanges();

                    string query = "UPDATE [SAPEPUserInfoSAPEPGroupInfo] SET EndpointSystemId = " + sapEPUsr.EndPointSystemId + " WHERE SAPEPUserInfo_UserId = " + sapEPUsr.UserId;
                    oIACMDBContext.Database.ExecuteSqlCommand(query);
                }
                isDataSaved = true;
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while mapping SAPEP User to Group from Provisioning Manager.", ex);
                throw;
            }
            return isDataSaved;
        }

        #endregion

        #endregion

        #region EDIT

        public bool UpdateAccessRequest(AccessRequest accessRequest)
        {
            bool isDataSaved = false;
            if (accessRequest == null)
                return isDataSaved;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        var reviewEPEntry = _context.Entry(accessRequest);
                        if (reviewEPEntry.State == EntityState.Detached)
                        {
                            var set = _context.Set<AccessRequest>();
                            AccessRequest attachedreviewEP = set.Find(accessRequest.AccessRequestId);

                            if (attachedreviewEP != null)
                            {
                                var attachedEntry = _context.Entry(attachedreviewEP);
                                attachedEntry.CurrentValues.SetValues(accessRequest);
                            }
                        }
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while updating AccessRequest.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        #endregion

        #region DELETE
        public bool DeleteADUser(int ADUserId)
        {
            bool isDataSaved = false;
            //string query = "DI_spDeleteADUserByUserId";
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    ADSUserInfo delUsrInfo = null;
                    delUsrInfo = oIACMDBContext.ADSUserInfos.Where(ui => ui.UserId == ADUserId).FirstOrDefault();
                    if (delUsrInfo != null)
                    {
                        oIACMDBContext.ADSUserInfos.Remove(delUsrInfo);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting AD User.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        public bool DeleteADUserGroupMap(ADSUserInfo adUser)
        {
            bool isDataSaved = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    ADSUserInfo currentADUser = oIACMDBContext.ADSUserInfos.FirstOrDefault(item => item.UserId == adUser.UserId);

                    foreach (string group in adUser.Groups)
                    {
                        currentADUser.ADSGroups.Remove(oIACMDBContext.ADSGroupInfos.FirstOrDefault(item => group.Equals(item.cn, StringComparison.CurrentCultureIgnoreCase)));
                    }
                    //currentADUser.ADSGroups.Clear();
                    oIACMDBContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting AD User Group Map.", ex);
                throw ex;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        public bool DeleteSAPUser(int sapUserId)
        {
            bool isDataSaved = false;
            //string query = "DI_spDeleteSAPUserByUserId";
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    SAPUserinfo delUsrInfo = null;
                    delUsrInfo = oIACMDBContext.SAPUserinfos.Where(ui => ui.UserId == sapUserId).FirstOrDefault();
                    if (delUsrInfo != null)
                    {
                        oIACMDBContext.SAPUserinfos.Remove(delUsrInfo);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting SAP User.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }
        public bool DeleteSAPUserRoleMap(SAPUserinfo sapUser)
        {
            bool isDataSaved = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    SAPUserinfo currentSapUser = oIACMDBContext.SAPUserinfos.FirstOrDefault(item => item.UserId == sapUser.UserId);


                    foreach (string SapRole in sapUser.Roles)
                    {
                        SAPRoleInfo saproleinfo = currentSapUser.SAPRoleInfo.FirstOrDefault(role => role.ROLE_CODE == SapRole);
                        if (saproleinfo != null)
                        {
                            currentSapUser.SAPRoleInfo.Remove(saproleinfo);
                        }
                    }

                    //currentSapUser.SAPRoleInfo.Clear();


                    oIACMDBContext.SaveChanges();
                    isDataSaved = true;
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting SAP User Role Map.", ex);
                throw ex;
            }

            return isDataSaved;
        }

        public bool DeleteSAPUserGroupMap(SAPUserinfo sapUser)
        {
            bool isDataSaved = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    SAPUserinfo currentSapUser = oIACMDBContext.SAPUserinfos.FirstOrDefault(item => item.UserId == sapUser.UserId);

                    foreach (string sapGroup in sapUser.Groups)
                    {
                        SAPGroupInfo sapGroupInfo = currentSapUser.SAPGroupInfo.FirstOrDefault(group => group.GROUP_CODE == sapGroup);
                        if (sapGroupInfo != null)
                        {
                            currentSapUser.SAPGroupInfo.Remove(sapGroupInfo);
                        }
                    }

                    oIACMDBContext.SaveChanges();
                    isDataSaved = true;
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting SAP User Group Map.", ex);
                throw ex;
            }

            return isDataSaved;
        }

        public bool DeleteSAEPPUser(int sapEPUserId)
        {
            bool isDataSaved = false;
            //string query = "DI_spDeleteSAPEPUserByUserId";

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    SAPEPUserInfo delUsrInfo = null;
                    delUsrInfo = oIACMDBContext.SAPEPUserInfos.Where(ui => ui.UserId == sapEPUserId).FirstOrDefault();
                    if (delUsrInfo != null)
                    {
                        oIACMDBContext.SAPEPUserInfos.Remove(delUsrInfo);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting SAP EP User.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        public bool DeleteSAPEPUserRoleMap(SAPEPUserInfo sapEPUsr)
        {
            bool isDataSaved = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    SAPEPUserInfo currentSapEPUser = oIACMDBContext.SAPEPUserInfos.FirstOrDefault(item => item.UserId == sapEPUsr.UserId);


                    foreach (string SapepRole in sapEPUsr.Roles)
                    {
                        SAPEPRoleInfo sapeproleinfo = currentSapEPUser.SAPEPRoles.FirstOrDefault(role => role.UniqueRoleName == SapepRole);
                        if (sapeproleinfo != null)
                        {
                            currentSapEPUser.SAPEPRoles.Remove(sapeproleinfo);
                        }
                    }

                    oIACMDBContext.SaveChanges();
                    isDataSaved = true;
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting SAPEP User Role Map.", ex);
                throw ex;
            }

            return isDataSaved;
        }

        public bool DeleteSAPEPUserGroupMap(SAPEPUserInfo sapEPUsr)
        {
            bool isDataSaved = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    SAPEPUserInfo currentSapEPUser = oIACMDBContext.SAPEPUserInfos.FirstOrDefault(item => item.UserId == sapEPUsr.UserId);


                    foreach (string SapepGroup in sapEPUsr.Groups)
                    {
                        SAPEPGroupInfo sapepgroupinfo = currentSapEPUser.SAPEPGroups.FirstOrDefault(role => role.GroupUniqueName == SapepGroup);
                        if (sapepgroupinfo != null)
                        {
                            currentSapEPUser.SAPEPGroups.Remove(sapepgroupinfo);
                        }
                    }

                    oIACMDBContext.SaveChanges();
                    isDataSaved = true;
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting SAPEP User Group Map.", ex);
                throw ex;
            }

            return isDataSaved;
        }

        public bool DeleteLotusNotesUser(int LNUserId)
        {
            bool isDataSaved = false;
            //string query = "DI_spDeleteLotusNotesUserByUserId";

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    LotusUserInfo delUsrInfo = null;
                    delUsrInfo = oIACMDBContext.LotusUserInfos.Where(ui => ui.UserId == LNUserId).FirstOrDefault();
                    if (delUsrInfo != null)
                    {
                        oIACMDBContext.LotusUserInfos.Remove(delUsrInfo);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting LotusNotes User.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }
        public bool DeleteLotusNotesUserGroupMap(LotusUserInfo lnsUser)
        {
            bool isDataSaved = false;

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    LotusUserInfo currentLNSUser = oIACMDBContext.LotusUserInfos.FirstOrDefault(item => item.UserId == lnsUser.UserId);

                    foreach (string group in lnsUser.Groups)
                    {
                        currentLNSUser.LotusGroupInfos.Remove(oIACMDBContext.LotusGroupInfos.FirstOrDefault(item => group.Equals(item.Group, StringComparison.CurrentCultureIgnoreCase)));
                    }

                    //currentLNSUser.LotusGroupInfos.Clear();
                    oIACMDBContext.SaveChanges();
                }
                isDataSaved = true;
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting LotusNotes User Group Map.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        #endregion

        #region GET

        public IList<AutoProvisioningDetailsReportData> getAllAutoProvisioningDetailsBtnStartAndEndTimePeriod(DateTime startDate, DateTime endDate)
        {
            IList<AutoProvisioningDetailsReportData> retList = new List<AutoProvisioningDetailsReportData>();

            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    retList = (from AR in _iacmdbcontext.AccessRequests
                               //join PRL in _iacmdbcontext.ProvisioningRoles
                               //on AR.ProvisioningRoleId equals PRL.ProvisioningRoleId
                               join usr in _iacmdbcontext.Users
                               on AR.UserId equals usr.UserId
                               where AR.ExecutionTime >= startDate && AR.ExecutionTime <= endDate
                               select new AutoProvisioningDetailsReportData
                               {
                                   accessRequestId = AR.AccessRequestId,
                                   activationDate = AR.ActivationDate.Value,
                                   description = AR.Description,
                                   executionTime = AR.ExecutionTime,
                                   progressStatusENUM = AR.ProgressStatus,
                                   remarks = AR.Remarks,
                                   requestStatusENUM = AR.RequestStatus,
                                   userName = usr.UserName
                               }).ToList();

                    foreach (AutoProvisioningDetailsReportData item in retList)
                    {
                        switch (item.progressStatusENUM)
                        {
                            case AccessRequestProgress.Completed:
                                item.progressStatus = "COMPLETED";
                                break;
                            case AccessRequestProgress.Executing:
                                item.progressStatus = "EXECUTING";
                                break;
                            case AccessRequestProgress.ExecutionFailed:
                                item.progressStatus = "EXECUTION FAILED";
                                break;
                            case AccessRequestProgress.ReadytoExecute:
                                item.progressStatus = "READY TO EXECUTE";
                                break;
                            case AccessRequestProgress.RequestRejected:
                                item.progressStatus = "REQUEST REJECTED";
                                break;
                            case AccessRequestProgress.WaitingforApproval:
                                item.progressStatus = "WAITING FOR APPROVAL";
                                break;
                        }

                        switch (item.requestStatusENUM)
                        {
                            case AccessRequestStatus.Approved:
                                item.requestStatus = "APPROVED";
                                break;
                            case AccessRequestStatus.Granted:
                                item.requestStatus = "GRANTED";
                                break;
                            case AccessRequestStatus.Rejected:
                                item.requestStatus = "REJECTED";
                                break;
                            case AccessRequestStatus.Requested:
                                item.requestStatus = "REQUESTED";
                                break;
                        }
                    }

                    return retList;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public AccessRequest GetAccessRequestById(long accessRequestId, bool deepLoad)
        {
            //string query = "DI_spGetAccessRequestById";
            AccessRequest accessRequest = null;

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (deepLoad)
                    {
                        accessRequest = oIACMDBContext.AccessRequests.Include("AdHocAccessRequest")
                            .Include("AdHocAccessRequest.Endpoint").FirstOrDefault(ar => ar.AccessRequestId == accessRequestId);
                    }
                    else
                    {
                        accessRequest = oIACMDBContext.AccessRequests.Include("AdHocAccessRequest").Where(ar => ar.AccessRequestId == accessRequestId).FirstOrDefault();
                    }
                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getting AccessRequest By Id", ex);
                throw;
            }
            return accessRequest;
        }

        public AccessRequest GetAccessRequestByAdHocRequestId(long adhocAccessRequestId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.AccessRequests.FirstOrDefault(ar => ar.AdHocAccessRequestId == adhocAccessRequestId);
                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getting AccessRequest By Id", ex);
                throw;
            }
        }

        public IList<AccessRequest> GetallAccessRequestByUserId(int UserId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.AccessRequests.Where(ar => ar.UserId == UserId).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getting AccessRequest By UserId", ex);
                throw;
            }
        }

        public IList<AdHocAccessRequest> GetallAdHocAccessRequestByUserId(int UserId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.AdHocAccessRequests.Where(ar => ar.RequesteeUserId == UserId).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getting AccessRequest By UserId", ex);
                throw;
            }
        }

        public bool CheckIfSameRequestsPending(AdHocAccessRequest AdhocrequestObj, out int RequesterID, out long AccessRequestID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {

                    var List = (from accessRequests in oIACMDBContext.AccessRequests
                                join adhocAccessRequests in oIACMDBContext.AdHocAccessRequests
                                on accessRequests.AdHocAccessRequestId equals adhocAccessRequests.AdHocAccessRequestId
                                where adhocAccessRequests.RequesteeUserId == AdhocrequestObj.RequesteeUserId &&
                                    adhocAccessRequests.AdhocActionType == AdhocrequestObj.AdhocActionType &&
                                    adhocAccessRequests.EndpointSystemType == AdhocrequestObj.EndpointSystemType &&
                                    adhocAccessRequests.AccountType == AdhocrequestObj.AccountType &&
                                    adhocAccessRequests.EndpointSystemId == AdhocrequestObj.EndpointSystemId &&
                                    (accessRequests.ProgressStatus == AccessRequestProgress.WaitingforApproval || accessRequests.ProgressStatus == AccessRequestProgress.Executing || accessRequests.ProgressStatus == AccessRequestProgress.ReadytoExecute)
                                select new
                                {
                                    accessRequests.AccessRequestId,
                                    requesterid = accessRequests.RequesterId

                                }).ToList();

                    if (List != null && List.Count > 0)
                    {
                        AccessRequestID = List[0].AccessRequestId;
                        RequesterID = List[0].requesterid;
                    }

                    else
                    {
                        AccessRequestID = 0;
                        RequesterID = 0;
                    }

                    if (List == null || List.Count == 0)
                        return false;
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getting CheckIfSameRequestsPending", ex);
                throw;
            }
            return true;
        }

        public IList<AccessRequest> GetallAccessRequests()
        {
            IList<AccessRequest> list = new List<AccessRequest>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    list = oIACMDBContext.AccessRequests.ToList();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getting AccessRequests", ex);
                throw;
            }
            return list;
        }

        public IList<AccessRequest> GetaAccessRequestsByProgressStatus(AccessRequestProgress Status)
        {
            IList<AccessRequest> list = new List<AccessRequest>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    list = oIACMDBContext.AccessRequests.Include(lq => lq.AdHocAccessRequest).Include(lq => lq.User).Where(lq => Status == AccessRequestProgress.None ? 1 == 1 : lq.ProgressStatus == Status).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getting AccessRequests", ex);
                throw;
            }
            return list;
        }


        public IList<AccessRequestGridData> GetallAccessRequestsGridData()
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    IList<AccessRequestGridData> accessRequestlist = new List<AccessRequestGridData>();
                    AccessRequestGridData _tmpAccessRequestGridData;

                    //list = oIACMDBContext.AccessRequests.ToList();

                    var List = (from accessRequests in oIACMDBContext.AccessRequests
                                join usr in oIACMDBContext.Users
                                on accessRequests.UserId equals usr.UserId
                                //join role in oIACMDBContext.ProvisioningRoles
                                //on accessRequests.ProvisioningRoleId equals role.ProvisioningRoleId
                                select new // AccessRequestGridData()
                                {
                                    accessRequests.AccessRequestId, accessRequests.TenantId, accessRequests.RequesterId, accessRequests.UserId, usr.DisplayName, accessRequests.Description, accessRequests.RequestStatus, //.ToString(), // AccessRequestStatus
                                    accessRequests.ProgressStatus, //.ToString(), // AccessRequestProgress
                                    accessRequests.RequestSource, //.ToString(), // AccessRequestSource
                                    accessRequests.ActivationDate,
                                    accessRequests.DeactivationDate,
                                    accessRequests.ExecutionTime,
                                    accessRequests.Remarks,
                                    accessRequests.ApproverType,
                                    accessRequests.IsCurrentLevelApproved
                                }).ToList();

                    foreach (var listItem in List)
                    {
                        _tmpAccessRequestGridData = new AccessRequestGridData();

                        _tmpAccessRequestGridData.AccessRequestId = listItem.AccessRequestId;
                        _tmpAccessRequestGridData.TenantId = listItem.TenantId;

                        _tmpAccessRequestGridData.RequesterId = listItem.RequesterId;
                        if (listItem.RequesterId != null)
                        {
                            if (listItem.RequesterId >= 0)
                            {
                                User usr = new UserDA().GetUser(listItem.RequesterId);
                                if (usr != null)
                                    _tmpAccessRequestGridData.Requester = usr.UserName;
                            }
                            else
                                _tmpAccessRequestGridData.Requester = string.Empty;
                        }
                        else
                            _tmpAccessRequestGridData.NewUserName = string.Empty;

                        _tmpAccessRequestGridData.UserId = listItem.UserId;
                        _tmpAccessRequestGridData.DisplayName = listItem.DisplayName;
                        _tmpAccessRequestGridData.Description = listItem.Description;

                        _tmpAccessRequestGridData.RequestStatus = listItem.RequestStatus.ToString();
                        _tmpAccessRequestGridData.ProgressStatus = listItem.ProgressStatus.ToString();
                        _tmpAccessRequestGridData.RequestSource = listItem.RequestSource.ToString();
                        _tmpAccessRequestGridData.ActivationDate = listItem.ActivationDate;
                        _tmpAccessRequestGridData.DeactivationDate = listItem.DeactivationDate;
                        _tmpAccessRequestGridData.ExecutionTime = listItem.ExecutionTime;
                        _tmpAccessRequestGridData.Remarks = listItem.Remarks;
                        _tmpAccessRequestGridData.ApproverType = listItem.ApproverType;

                        _tmpAccessRequestGridData.IsCurrentLevelApproved = listItem.IsCurrentLevelApproved;
                        accessRequestlist.Add(_tmpAccessRequestGridData);
                    }
                    return accessRequestlist;

                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getting AccessRequests", ex);
                throw;
            }
            //return list;
        }

        public IList<AccessRequestGridData> GetallAccessRequestsReportData(DateTime startDate, DateTime endDate, AccessRequestStatus approvalStatus, AccessRequestProgress provisioningStatus)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    IList<AccessRequestGridData> accessRequestlist = new List<AccessRequestGridData>();
                    AccessRequestGridData _tmpAccessRequestGridData;

                    //list = oIACMDBContext.AccessRequests.ToList();

                    var List = (from accessRequests in oIACMDBContext.AccessRequests
                                join usr in oIACMDBContext.Users
                                on accessRequests.UserId equals usr.UserId
                                //join role in oIACMDBContext.ProvisioningRoles
                                //on accessRequests.ProvisioningRoleId equals role.ProvisioningRoleId
                                where accessRequests.CreatedDate >= startDate && accessRequests.CreatedDate <= endDate
                                && (approvalStatus == AccessRequestStatus.None || accessRequests.RequestStatus == approvalStatus)
                                && (provisioningStatus == AccessRequestProgress.None || accessRequests.ProgressStatus == provisioningStatus)
                                select new // AccessRequestGridData()
                                {
                                    accessRequests.AccessRequestId,
                                    accessRequests.TenantId,
                                    accessRequests.RequesterId,
                                    accessRequests.UserId,
                                    usr.DisplayName,
                                    accessRequests.Description,
                                    accessRequests.RequestStatus, //.ToString(), // AccessRequestStatus
                                    accessRequests.ProgressStatus, //.ToString(), // AccessRequestProgress
                                    accessRequests.RequestSource, //.ToString(), // AccessRequestSource
                                    accessRequests.ActivationDate,
                                    accessRequests.DeactivationDate,
                                    accessRequests.ExecutionTime,
                                    accessRequests.Remarks,
                                    accessRequests.ApproverType,
                                    accessRequests.IsCurrentLevelApproved
                                }).ToList();

                    foreach (var listItem in List)
                    {
                        _tmpAccessRequestGridData = new AccessRequestGridData();

                        _tmpAccessRequestGridData.AccessRequestId = listItem.AccessRequestId;
                        _tmpAccessRequestGridData.TenantId = listItem.TenantId;

                        _tmpAccessRequestGridData.RequesterId = listItem.RequesterId;
                        if (listItem.RequesterId != null)
                        {
                            if (listItem.RequesterId >= 0)
                            {
                                User usr = new UserDA().GetUser(listItem.RequesterId);
                                if (usr != null)
                                    _tmpAccessRequestGridData.Requester = usr.UserName;
                            }
                            else
                                _tmpAccessRequestGridData.Requester = string.Empty;
                        }
                        else
                            _tmpAccessRequestGridData.NewUserName = string.Empty;

                        _tmpAccessRequestGridData.UserId = listItem.UserId;
                        _tmpAccessRequestGridData.DisplayName = listItem.DisplayName;
                        _tmpAccessRequestGridData.Description = listItem.Description;

                        _tmpAccessRequestGridData.RequestStatus = listItem.RequestStatus.ToString();
                        _tmpAccessRequestGridData.ProgressStatus = listItem.ProgressStatus.ToString();
                        _tmpAccessRequestGridData.RequestSource = listItem.RequestSource.ToString();
                        _tmpAccessRequestGridData.ActivationDate = listItem.ActivationDate;
                        _tmpAccessRequestGridData.DeactivationDate = listItem.DeactivationDate;
                        _tmpAccessRequestGridData.ExecutionTime = listItem.ExecutionTime;
                        _tmpAccessRequestGridData.Remarks = listItem.Remarks;
                        _tmpAccessRequestGridData.ApproverType = listItem.ApproverType;

                        _tmpAccessRequestGridData.IsCurrentLevelApproved = listItem.IsCurrentLevelApproved;
                        accessRequestlist.Add(_tmpAccessRequestGridData);
                    }
                    return accessRequestlist;

                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getting AccessRequests", ex);
                throw;
            }
            //return list;
        }
        
        #endregion

        #region Provisioning Engine Methods
        public List<AccessRequest> GetApprovedAccessRequestBatch()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    //First preference: Onboarding & AdhocRequest.
                    var request = _context.AccessRequests.FirstOrDefault(item => item.RequestStatus == AccessRequestStatus.Granted &&
                        item.ProgressStatus == AccessRequestProgress.ReadytoExecute && item.ExecutionTime.Value <= DateTime.Now &&
                        (item.RequestSource == AccessRequestSource.AdhocRequest || item.RequestSource == AccessRequestSource.OnBoarding));

                    if (request != null)
                    {
                        return _context.AccessRequests.Include("AdHocAccessRequest").Where(item => item.RequestStatus == AccessRequestStatus.Granted &&
                                  item.ProgressStatus == AccessRequestProgress.ReadytoExecute && item.ExecutionTime.Value <= DateTime.Now
                                  && item.TransactionId == request.TransactionId).ToList();
                    }
                    //Next preference: OffBoarding & OUChange & ReOrganisation.
                    request = _context.AccessRequests.FirstOrDefault(item => item.RequestStatus == AccessRequestStatus.Granted &&
                                                                             item.ProgressStatus == AccessRequestProgress.ReadytoExecute && item.ExecutionTime.Value <= DateTime.Now &&
                                                                             (item.RequestSource == AccessRequestSource.OffBoarding || item.RequestSource == AccessRequestSource.OUChange ||
                                                                              item.RequestSource == AccessRequestSource.ReOrganisation));
                    if (request != null)
                    {
                        return _context.AccessRequests.Include("AdHocAccessRequest").Where(item => item.RequestStatus == AccessRequestStatus.Granted &&
                                                                                                   item.ProgressStatus == AccessRequestProgress.ReadytoExecute && item.ExecutionTime.Value <= DateTime.Now
                                                                                                   && item.TransactionId == request.TransactionId).ToList();
                    }
                    //Next preference: Rest.
                    request = _context.AccessRequests.FirstOrDefault(item => item.RequestStatus == AccessRequestStatus.Granted &&
                                                                             item.ProgressStatus == AccessRequestProgress.ReadytoExecute && item.ExecutionTime.Value <= DateTime.Now);
                    if (request != null)
                    {
                        return _context.AccessRequests.Include("AdHocAccessRequest").Where(item => item.RequestStatus == AccessRequestStatus.Granted &&
                                                                                                   item.ProgressStatus == AccessRequestProgress.ReadytoExecute && item.ExecutionTime.Value <= DateTime.Now
                                                                                                   && item.TransactionId == request.TransactionId).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getting all AccessRequests.", ex);
                throw;
            }

            return null;
        }
        public bool isUserIdExist(int[] endPointIds, string usrLogInName, int DIUserId, out string ADUserLogOnName)
        {
            bool isIdExist = false;
            ADUserLogOnName = string.Empty;
            if (!(endPointIds != null && endPointIds.Length == 4))
                return isIdExist;

            //string query = "DI_spIsUserIdExistCheckByEndPoints";
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    ADSUserInfo tmpADSUserInfo = oIACMDBContext.ADSUserInfos.Where(ui => ui.DIUserId == DIUserId && ui.EndPointSystemId == endPointIds[0]).FirstOrDefault();
                    if (tmpADSUserInfo != null)
                        ADUserLogOnName = tmpADSUserInfo.sAMAccountName;

                    var adsUser = oIACMDBContext.ADSUserInfos.Where(ui => ui.sAMAccountName == usrLogInName && ui.EndPointSystemId == endPointIds[0]).Select(ui => ui.UserId).ToList();
                    var sapUser = oIACMDBContext.SAPUserinfos.Where(ui => ui.UserLogonName == usrLogInName && ui.EndPointSystemId == endPointIds[1]).Select(ui => ui.UserId).ToList();
                    var sapepUser = oIACMDBContext.SAPEPUserInfos.Where(ui => ui.UserName == usrLogInName && ui.EndPointSystemId == endPointIds[2]).Select(ui => ui.UserId).ToList();
                    var lnsUser = oIACMDBContext.LotusUserInfos.Where(ui => ui.InternetAddress.Substring(1, ui.InternetAddress.IndexOf("@")) == usrLogInName && ui.EndPointSystemId == endPointIds[3]).Select(ui => ui.UserId).ToList();

                    var listTotal1 = adsUser.Union(sapUser);
                    var listTotal2 = listTotal1.Union(sapepUser);
                    var listTotal = listTotal2.Union(lnsUser);

                    if (listTotal.Count() > 0)
                        isIdExist = true;
                    else
                        isIdExist = false;
                }

            }
            catch (Exception ex)
            {
                isIdExist = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while checking common user ID for Auto Provisioning.", ex);
                throw;
            }
            return isIdExist;
        }




        #endregion

        #region Private Helper Methods


        #endregion

        public IList<MyRequestsGridData> GetRequestsByActionUserId(int actionUserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    IList<MyRequestsGridData> requestList = new List<MyRequestsGridData>();
                    requestList = (from rqst in _context.AdHocAccessRequests.Include("EndPoint")
                                   join acssrqst in _context.AccessRequests
                                   on rqst.AdHocAccessRequestId equals acssrqst.AdHocAccessRequestId
                                   join rusr in _context.Users
                                   on rqst.RequesteeUserId equals rusr.UserId
                                   join rqust in _context.Users
                                   on rqst.RequesterUserId equals rqust.UserId
                                   where rqst.RequesterUserId == actionUserId && !acssrqst.IsDependentRequest
                                   select new MyRequestsGridData
                                   {
                                       AdHocAccessRequestId = rqst.AdHocAccessRequestId,
                                       Name = rqst.EndPoint.DisplayName,
                                       RequestedDate = rqst.CreatedDate,
                                       ExecutionDate = acssrqst.ExecutionTime,
                                       RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                       Comments = rqst.Comments,
                                       RequestStatus = acssrqst.RequestStatus,
                                       AccessRequestID = acssrqst.AccessRequestId,
                                       RequesteeName = rusr.DisplayName,
                                       RequesterName = rqust.DisplayName,
                                       AccountType = rqst.AccountType,
                                       ValidFrom = rqst.ValidFrom,
                                       ValidTo = rqst.ValidTo
                                   }).ToList();

                    foreach (MyRequestsGridData _rqst in requestList)
                    {
                        _rqst.Status = UtilityMethods.GetEnumAttribute<DisplayAs>(_rqst.RequestStatus);
                    }
                    return requestList;
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<ApproversGridData> GetApproveStatusListByRequestId(long rqstId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    IList<ApproversGridData> approveStatusList = new List<ApproversGridData>();
                    approveStatusList = (from adhocrqst in _context.AdHocAccessRequests
                                         join acssrqst in _context.AccessRequests
                                         on adhocrqst.AdHocAccessRequestId equals acssrqst.AdHocAccessRequestId
                                         join tsk in _context.Tasks
                                         on acssrqst.AccessRequestId equals tsk.AccessRequestId
                                         join usr in _context.Users
                                         on tsk.ActionUserId equals usr.UserId
                                         where adhocrqst.AdHocAccessRequestId == rqstId && (tsk.IsCompleted || tsk.Action == StatusType.Rejected)
                                         select new ApproversGridData
                                         {
                                             ApproverName = usr.DisplayName,
                                             ApproverType = tsk.Approver,
                                             WorkflowState = tsk.WorkflowStateId,
                                             RequestStatus = acssrqst.RequestStatus,
                                             Action = tsk.Action,
                                             ActionDate = tsk.LastModifiedDate != null ? tsk.LastModifiedDate.Value : tsk.CreatedDate,
                                             Comments = tsk.Comments
                                         }).ToList();

                    foreach (ApproversGridData _approver in approveStatusList)
                    {
                        _approver.Status = UtilityMethods.GetEnumAttribute<Description>(_approver.Action);
                    }

                    return approveStatusList;
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<MyRequestsGridData> GetRequestsByRequester(List<int> RequesterIDs)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    IList<MyRequestsGridData> requestList = new List<MyRequestsGridData>();
                    IList<MyRequestsGridData> requestListAll = new List<MyRequestsGridData>();
                    if (RequesterIDs != null && RequesterIDs.Count > 0)
                    {
                        foreach (int userid in RequesterIDs)
                        {
                            requestList = (from rqst in _context.AdHocAccessRequests.Include("EndPoint")
                                           join acssrqst in _context.AccessRequests
                                           on rqst.AdHocAccessRequestId equals acssrqst.AdHocAccessRequestId
                                           join usr in _context.Users
                                           on acssrqst.RequesterId equals usr.UserId
                                           join rusr in _context.Users
                                           on rqst.RequesteeUserId equals rusr.UserId
                                           where rqst.RequesterUserId == userid && !acssrqst.IsDependentRequest
                                           select new MyRequestsGridData
                                           {
                                               AdHocAccessRequestId = rqst.AdHocAccessRequestId,
                                               Name = rqst.EndPoint.DisplayName,
                                               RequestedDate = rqst.CreatedDate,
                                               ExecutionDate = acssrqst.ExecutionTime,
                                               RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                               Comments = rqst.Comments,
                                               RequestStatus = acssrqst.RequestStatus,
                                               AccessRequestID = acssrqst.AccessRequestId,
                                               RequesteeName = rusr.DisplayName,
                                               RequesterName = usr.DisplayName,
                                               AccountType = rqst.AccountType,
                                               ValidFrom = rqst.ValidFrom,
                                               ValidTo = rqst.ValidTo
                                           }).ToList();

                            foreach (MyRequestsGridData _rqst in requestList)
                            {
                                _rqst.Status = UtilityMethods.GetEnumAttribute<DisplayAs>(_rqst.RequestStatus);
                            }
                            foreach (MyRequestsGridData listitem in requestList)
                            {
                                requestListAll.Add(listitem);
                            }
                        }
                    }
                    else
                    {
                        requestListAll = (from rqst in _context.AdHocAccessRequests.Include("EndPoint")
                                          join acssrqst in _context.AccessRequests
                                          on rqst.AdHocAccessRequestId equals acssrqst.AdHocAccessRequestId
                                          join usr in _context.Users
                                          on acssrqst.RequesterId equals usr.UserId
                                          join rusr in _context.Users
                                          on rqst.RequesteeUserId equals rusr.UserId
                                          where !acssrqst.IsDependentRequest
                                          select new MyRequestsGridData
                                          {
                                              AdHocAccessRequestId = rqst.AdHocAccessRequestId,
                                              Name = rqst.EndPoint.DisplayName,
                                              RequestedDate = rqst.CreatedDate,
                                              ExecutionDate = acssrqst.ExecutionTime,
                                              RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                              Comments = rqst.Comments,
                                              RequestStatus = acssrqst.RequestStatus,
                                              AccessRequestID = acssrqst.AccessRequestId,
                                              RequesteeName = rusr.DisplayName,
                                              RequesterName = usr.DisplayName,
                                              AccountType = rqst.AccountType,
                                              ValidFrom = rqst.ValidFrom,
                                              ValidTo = rqst.ValidTo
                                          }).ToList();

                        foreach (MyRequestsGridData _rqst in requestListAll)
                        {
                            _rqst.Status = UtilityMethods.GetEnumAttribute<DisplayAs>(_rqst.RequestStatus);
                        }
                    }
                    requestListAll = requestListAll.OrderByDescending(rqst => rqst.RequestedDate).ToList();
                    return requestListAll;
                }
            }
            catch
            {
                throw;
            }
        }

        public AdHocAccessRequest GetAdhocRequest(int AdhocRequestID)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                return Context.AdHocAccessRequests.Include("EndPoint").Where(lq => lq.AdHocAccessRequestId == AdhocRequestID).FirstOrDefault();
            }
        }

        #region OracleEBS

        public bool DeleteOracleEBSUser(string oraUserName)
        {
            bool isDataSaved = false;
            //string query = "DI_spDeleteLotusNotesUserByUserId";

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    OracleEBSUserInfo delUsrInfo = oIACMDBContext.OracleEBSUserInfo.Where(ui => ui.USER_NAME == oraUserName).FirstOrDefault();
                    if (delUsrInfo != null)
                    {
                        // oIACMDBContext.OracleUMUserInfo.Remove(delUsrInfo);
                        delUsrInfo.LastModifiedDate = DateTime.Now;
                        delUsrInfo.IsLocked = true;
                        oIACMDBContext.Entry(delUsrInfo);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting LotusNotes User.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        public bool AddOracleEBSUserIntoRole(OracleEBSUserInfo oraEBSUsr, EntitlementType Entitlement)
        {
            bool isDataSaved = false;
            //string query = "DI_spAddLNSUserGroupMap";
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    OracleEBSUserInfo currentOracleEBSUser = oIACMDBContext.OracleEBSUserInfo.Include("RolesInfo").Include("ResponsibilitiesInfo").FirstOrDefault(item => item.USER_NAME == oraEBSUsr.USER_NAME && !oraEBSUsr.IsLocked);
                    if (Entitlement == EntitlementType.Role)
                    {

                        foreach (string roleStr in oraEBSUsr.Roles)
                        {

                            IList<OracleEBSRoleInfo> roleList = oIACMDBContext.OracleEBSRoleInfo.Where(gi => gi.NAME == roleStr && gi.EndPointSystemId == oraEBSUsr.EndPointSystemId).ToList();
                            foreach (OracleEBSRoleInfo roleItem in roleList)
                            {
                                OracleEBSRoleInfo tmproleItem = currentOracleEBSUser.RolesInfo.Where(r => r.NAME == roleItem.NAME).FirstOrDefault();
                                if (tmproleItem == null)
                                {
                                    currentOracleEBSUser.RolesInfo.Add(roleItem);
                                }
                            }
                        }
                        oIACMDBContext.SaveChanges();
                        string query = "UPDATE OracleEBSRoleInfoOracleEBSUserInfo SET EndpointSystemId = " + oraEBSUsr.EndPointSystemId + " WHERE OracleEBSUserInfo_UserId = " + oraEBSUsr.UserId;
                        oIACMDBContext.Database.ExecuteSqlCommand(query);
                    }
                    else
                    {
                        foreach (string roleStr in oraEBSUsr.Responsibilities)
                        {
                            IList<OracleEBSResponsibilitiesInfo> responsibilityList = oIACMDBContext.OracleEBSResponsibilitiesInfo.Where(gi => gi.RESPONSIBILITY_KEY == roleStr && gi.EndPointSystemId == oraEBSUsr.EndPointSystemId).ToList();
                            OracleEBSResponsibilitiesInfo responsibilityItem = responsibilityList.FirstOrDefault();
                            if (responsibilityItem != null)
                            {
                                OracleEBSResponsibilitiesInfo tmproleItem = currentOracleEBSUser.ResponsibilitiesInfo.Where(r => r.RESPONSIBILITY_NAME == responsibilityItem.RESPONSIBILITY_NAME).FirstOrDefault();
                                if (tmproleItem == null)
                                {
                                    currentOracleEBSUser.ResponsibilitiesInfo.Add(responsibilityItem);
                                }
                            }

                        }
                        oIACMDBContext.SaveChanges();
                        string query = "UPDATE OracleEBSUserInfoOracleEBSResponsibilitiesInfo SET EndpointSystemId = " + oraEBSUsr.EndPointSystemId + " WHERE OracleEBSUserInfo_UserId = " + oraEBSUsr.UserId;
                        oIACMDBContext.Database.ExecuteSqlCommand(query);
                    }



                }

                isDataSaved = true;
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while mapping Oracle User to Role from Provisioning Manager.", ex);
                throw;
            }
            return isDataSaved;
        }

        public bool DeleteOracleEBSUserRoleMap(OracleEBSUserInfo OracleEBSUser, EntitlementType Entitlement)
        {
            bool isDataSaved = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    OracleEBSUserInfo currentOracleEBSUser = oIACMDBContext.OracleEBSUserInfo.Include("RolesInfo").Include("ResponsibilitiesInfo").FirstOrDefault(item => item.USER_NAME == OracleEBSUser.USER_NAME && !OracleEBSUser.IsLocked);

                    if (Entitlement == EntitlementType.Role)
                    {
                        foreach (string OracleRole in OracleEBSUser.Roles)
                        {

                            OracleEBSRoleInfo oracleEBSroleinfo = currentOracleEBSUser.RolesInfo.FirstOrDefault(role => role.NAME == OracleRole);
                            if (oracleEBSroleinfo != null)
                            {
                                currentOracleEBSUser.RolesInfo.Remove(oracleEBSroleinfo);
                                oIACMDBContext.SaveChanges();
                                //  oIACMDBContext.Database.ExecuteSqlCommand(string.Format("update OracleEBSRoleInfoOracleEBSUserInfo set EndpointSystemId={0},Version='{1}' where OracleEBSRoleInfo_RoleId={2} and OracleEBSUserInfo_UserId={3}", OracleEBSUser.EndPointSystemId, oracleEBSroleinfo.version, oracleEBSroleinfo.RoleId, OracleEBSUser.UserId));
                            }
                        }
                    }
                    else
                    {
                        foreach (string OracleResponsibilities in OracleEBSUser.Responsibilities)
                        {
                            OracleEBSResponsibilitiesInfo oracleEBSresponsibilitiesinfo = currentOracleEBSUser.ResponsibilitiesInfo.FirstOrDefault(role => role.RESPONSIBILITY_KEY == OracleResponsibilities);
                            if (oracleEBSresponsibilitiesinfo != null)
                            {
                                currentOracleEBSUser.ResponsibilitiesInfo.Remove(oracleEBSresponsibilitiesinfo);
                                oIACMDBContext.SaveChanges();
                                //   oIACMDBContext.Database.ExecuteSqlCommand(string.Format("update OracleEBSUserInfoOracleEBSResponsibilitiesInfo set EndpointSystemId={0},Version='{1}' where OracleEBSResponsibilitiesInfo_ResponsibilityId={2} and OracleEBSUserInfo_UserId={3}", OracleEBSUser.EndPointSystemId, OracleEBSUser.Version, oracleEBSresponsibilitiesinfo.ResponsibilityId, OracleEBSUser.UserId));
                            }

                        }
                    }

                    oIACMDBContext.SaveChanges();

                    isDataSaved = true;
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting GTSS User Role Map.", ex);
                throw ex;
            }

            return isDataSaved;
        }





        public bool AddLinuxUserIntoGroup(LinuxUserInfo linuxUsr, EntitlementType Entitlement)
        {
            bool isDataSaved = false;
            //string query = "DI_spAddLNSUserGroupMap";
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    LinuxUserInfo currentLinuxUser = oIACMDBContext.LinuxUserInfos.Include("LinuxGroup").FirstOrDefault(item => item.UserName == linuxUsr.UserName &&
                        !linuxUsr.IsLocked);
                    if (Entitlement == EntitlementType.Group)
                    {

                        foreach (string roleStr in linuxUsr.Groups)
                        {

                            IList<LinuxGroupInfo> groupList = oIACMDBContext.LinuxGroupInfos.Where(gi => gi.GroupName == roleStr && gi.EndPointSystemId == linuxUsr.EndPointSystemId).ToList();
                            foreach (LinuxGroupInfo groupItem in groupList)
                            {
                                LinuxGroupInfo tmproleItem = currentLinuxUser.LinuxGroup.Where(r => r.GroupName == groupItem.GroupName).FirstOrDefault();
                                if (tmproleItem == null)
                                {
                                    currentLinuxUser.LinuxGroup.Add(groupItem);
                                }
                            }
                        }
                        oIACMDBContext.SaveChanges();
                        string query = "UPDATE LinuxUserInfoLinuxGroupInfo SET EndpointSystemId = " + linuxUsr.EndPointSystemId + " WHERE LinuxUserInfo_UserId = " + linuxUsr.UserId;
                        oIACMDBContext.Database.ExecuteSqlCommand(query);
                    }


                }

                isDataSaved = true;
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while mapping Oracle User to Role from Provisioning Manager.", ex);
                throw;
            }
            return isDataSaved;
        }




        #endregion

        public bool DeleteLinuxUser(string LinuxUser)
        {
            bool isDataSaved = false;
            //string query = "DI_spDeleteLotusNotesUserByUserId";

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    LinuxUserInfo delUsrInfo = oIACMDBContext.LinuxUserInfos.Where(ui => ui.UserName == LinuxUser).FirstOrDefault();
                    if (delUsrInfo != null)
                    {
                        // oIACMDBContext.OracleUMUserInfo.Remove(delUsrInfo);
                        delUsrInfo.LastModifiedDate = DateTime.Now;
                        delUsrInfo.IsLocked = true;
                        oIACMDBContext.Entry(delUsrInfo);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting Linux User.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        public void DeleteLinuxUserGroupMap(LinuxUserInfo LinuxUser, EntitlementType EntitlementType)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    oIACMDBContext.Database.ExecuteSqlCommand("Delete LinuxUserInfoLinuxGroupInfo where LinuxUserInfo_UserId =" + LinuxUser.UserId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //OracleUM//
        public bool DeleteOracleUserRoleMap(OracleUMUserInfo OracleUser, EntitlementType EntitlementType = EntitlementType.Role)
        {
            bool isDataSaved = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    OracleUMUserInfo currentOracleUser = oIACMDBContext.OracleUMUserInfo.FirstOrDefault(item => item.UserName == OracleUser.UserName);

                    foreach (string OracleRole in OracleUser.Roles)
                    {
                        OracleUMRoleInfo oracleroleinfo = currentOracleUser.OracleRoles.FirstOrDefault(role => role.RoleName == OracleRole);
                        if (oracleroleinfo != null)
                        {
                            currentOracleUser.OracleRoles.Remove(oracleroleinfo);
                        }
                    }
                    oIACMDBContext.SaveChanges();
                    isDataSaved = true;
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting GTSS User Role Map.", ex);
                throw ex;
            }

            return isDataSaved;
        }

        public bool DeleteOracleUser(string oraUserName)
        {
            bool isDataSaved = false;
            //string query = "DI_spDeleteLotusNotesUserByUserId";

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    OracleUMUserInfo delUsrInfo = null;
                    delUsrInfo = oIACMDBContext.OracleUMUserInfo.Where(ui => ui.UserName == oraUserName).FirstOrDefault();
                    if (delUsrInfo != null)
                    {
                        // oIACMDBContext.OracleUMUserInfo.Remove(delUsrInfo);
                        delUsrInfo.LastModifiedDate = DateTime.Now;
                        delUsrInfo.IsLocked = true;
                        oIACMDBContext.Entry(delUsrInfo);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while deleting LotusNotes User.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        public bool AddOracleUserIntoRole(OracleUMUserInfo oraUsr, EntitlementType Entitlement)
        {
            bool isDataSaved = false;
            //string query = "DI_spAddLNSUserGroupMap";
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    OracleUMUserInfo currentOracleUser = oIACMDBContext.OracleUMUserInfo.FirstOrDefault(item => item.UserName == oraUsr.UserName);
                    foreach (string roleStr in oraUsr.Roles)
                    {
                        IList<OracleUMRoleInfo> roleList = oIACMDBContext.OracleUMRoleInfo.Where(gi => gi.RoleName == roleStr && gi.EndPointSystemId == oraUsr.EndPointSystemId).ToList();
                        foreach (OracleUMRoleInfo roleItem in roleList)
                        {
                            OracleUMRoleInfo tmproleItem = currentOracleUser.OracleRoles.Where(r => r.RoleName == roleItem.RoleName).FirstOrDefault();
                            if (tmproleItem == null)
                            {
                                currentOracleUser.OracleRoles.Add(roleItem);
                            }
                        }
                        oIACMDBContext.SaveChanges();
                    }
                    string query = "UPDATE OracleUMUserInfoOracleUMRoleInfo SET EndpointSystemId = " + oraUsr.EndPointSystemId + " WHERE OracleUMUserInfo_UserId = " + oraUsr.UserId;

                    oIACMDBContext.Database.ExecuteSqlCommand(query);
                }

                isDataSaved = true;
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while mapping Oracle User to Role from Provisioning Manager.", ex);
                throw;
            }
            return isDataSaved;
        }
    }
}
