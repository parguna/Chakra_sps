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
using DI.Chakra.Model.SAP;
using DI.Chakra.DataAccess;
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model.GridData;

namespace DI.Chakra.Facade
{
    public class SAPFacade : IEndpointFacade
    {


        public SAPConnectorDA SAPDA { get; set; }
        private EndpointObjectDA EndpointObjectDA = null;
        private ReconcilationFacade reconcilationFacade = new ReconcilationFacade();



        public SAPFacade()
        {
            SAPDA = new SAPConnectorDA();
            EndpointObjectDA = new EndpointObjectDA();
        }
        public IEndpointUserInfo GetEndpointUserInfo(long EndpointUserId)
        {
            return GetSAPUserInfo(EndpointUserId);
        }
        public void AddSAPUserinfo_OLD(IList<SAPUserinfo> list)
        {
            try
            {
                SAPDA.AddSAPUserinfoBulk(list);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<SAPUserHistoryInfo> GetSAPUserhistoryinfo(long Endpointsystemid)
        {
            try
            {
                return SAPDA.getSAPUserHistoryInfo(Endpointsystemid);

                // return  .GetSAPUserHistoryInfoDA(Endpointsystemid);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SaveUsers(IList<SAPUserinfo> list, long EndpointSystemId)
        {
            try
            {

                SAPUserinfo sapexistingUser = null;
                foreach (SAPUserinfo usr in list)
                {

                    sapexistingUser = SAPDA.GetUserByUserLogonName(usr.UserLogonName, EndpointSystemId);
                    if (sapexistingUser == null)
                    {
                        usr.Version = Convert.ToInt16(VersionNo.Available);
                        SAPDA.AddSAPUser(usr);
                        DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Warning, string.Format("Reconciliation Exception: SAP User {0} has been ADDED without any request made in the system", usr.UserLogonName));

                    }

                    else
                    {
                        usr.IsLocked = sapexistingUser.IsLocked;
                        usr.LockScenario = sapexistingUser.LockScenario;
                        usr.TenantId = sapexistingUser.TenantId;
                        usr.Id = sapexistingUser.Id;
                        usr.DIUserId = sapexistingUser.DIUserId;
                        usr.MappingType = sapexistingUser.MappingType;
                        usr.AccountType = sapexistingUser.AccountType;
                        usr.IfUnmappedMapTo = sapexistingUser.IfUnmappedMapTo;
                        usr.CreatedDate = sapexistingUser.CreatedDate;
                        usr.CreatedBy = sapexistingUser.CreatedBy;
                        usr.Version = sapexistingUser.Version;
                        SAPDA.UpdateSAPUser(usr);
                    }

                    reconcilationFacade.AddOrUpdateAccountInReconTable(usr, sapexistingUser == null ? true : false);

                }


            }
            catch (Exception)
            {
                throw;
            }
        }


        public void UpdateSAPUser(SAPUserinfo user)
        {
            SAPDA.UpdateSAPUser(user);
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


        public void AddSAPProfileInfo(IList<SAPProfileInfo> list)
        {
            try
            {


            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<SAPSingleRoleInfo> GetSapSingleRoles(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetSapSingleRoles(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SAPCompositeRoleInfo> GetSapCompositeRoles(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetSapCompositeRoles(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SAPConfiguration GetSAPConfigurations()
        {
            return SAPDA.GetSAPConfiguration();

        }



        public IList<SAPUsersToRolesReportData> GetAllSapUsersToRole(long EndpointSystemId, List<int> SelectedUsers)
        {
            try
            {
                return SAPDA.GetSAPUserToRolesReportData(EndpointSystemId, SelectedUsers);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPUsersToProfilesReportData> GetAllSapUsersToProfiles(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetSAPUserToProfilesReportData(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<SAPUserRoleTCodeAuth> GetAllSapUserRoleTCodeAuthDT(int UserId, long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetSapUserRoleTCodeAuthReport(EndpointSystemId, UserId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<SAPUserRoleTCodeAuth> GetAllSapUserTCodeDT(int UserId, long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetSapUserTCodeReport(UserId, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<SAPUserRoleTCodeAuth> GetAllSapUserTCodeDT(int UserId)
        {
            try
            {
                return SAPDA.GetSapUserTCodeReport(UserId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<SAPUserRoleTCodeAuth> GetAllSapUserRoleTCodeAuthDT(int UserId, long EndpointSystemId, bool _withoutAuthorization)
        {
            try
            {
                return SAPDA.GetSapUserRoleTCodeAuthReport(UserId, EndpointSystemId, _withoutAuthorization);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IList<SAPGroupInfo> GetAllSAPEPGroupsByEndpointsystemid(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetAllSAPGroupsByEndpointsystemid(EndpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public IList<SAPUserRoleTCodeAuth> GetAllSapUserRoleTCodeAuthDT(int UserId)
        {
            try
            {
                return SAPDA.GetSapUserRoleTCodeAuthReport(UserId);
            }
            catch (Exception)
            {
                throw;
            }
        }




        public IList<SAPUsersToGroupsReportData> GetAllSapUsersToGroupsDT()
        {
            try
            {
                return SAPDA.GetSAPUserToGroupsReportData();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SAPGroupInfo getGroupInfo(SAPGroupHistoryInfo SAPGroupInfoHistory)
        {

            SAPGroupInfo SAPGroupInfo = new SAPGroupInfo();

            try
            {
                SAPGroupInfo.GROUP_CODE = SAPGroupInfoHistory.GROUP_CODE;
                SAPGroupInfo.TenantId = SAPGroupInfoHistory.TenantId;
                SAPGroupInfo.EndPointSystemId = SAPGroupInfoHistory.EndPointSystemId.Value;
                SAPGroupInfo.GROUP_DESC = SAPGroupInfoHistory.GROUP_DESC;
                SAPGroupInfo.TargetSysId = SAPGroupInfoHistory.TargetSysId;
                SAPGroupInfo.CreatedBy = SAPGroupInfoHistory.CreatedBy;
                SAPGroupInfo.CreatedDate = SAPGroupInfoHistory.CreatedDate;
                SAPGroupInfo.ModifiedBy = SAPGroupInfoHistory.ModifiedBy;
                SAPGroupInfo.ModifiedDate = SAPGroupInfoHistory.ModifiedDate;


                return SAPGroupInfo;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddSAPGroupInfo(IList<SAPGroupInfo> list)
        {
            try
            {

                SAPDA.AddSAPGroupInfoBulk(list);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddSAPGroupInfo(IList<SAPGroupHistoryInfo> list)
        {
            try
            {

                SAPDA.AddSAPGroupInfoHistoryBulk(list);

                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Added {0} SAP GroupHistory objects", list.Count.ToString()));

                var listGroups = list.IterateConvert<SAPGroupHistoryInfo, SAPGroupInfo>(
                   delegate(SAPGroupHistoryInfo _group)
                   {
                       return this.getGroupInfo(_group);
                   });

                SAPDA.DeleteAll<SAPGroupInfo>(list.First().TenantId, list.First().EndPointSystemId);


                SAPDA.AddSAPGroupInfoBulk(listGroups);
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Added {0} SAP Group objects", list.Count.ToString()));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<SAPRoleInfo> GetSAPRoles(EndPoint endPoint)
        {
            return SAPDA.GetSAPRoles(endPoint);
        }

        public IList<SAPPosition> GetSAPPositions(EndPoint endPoint)
        {
            return SAPDA.GetSAPPositions(endPoint);
        }

        public IList<SAPParameterHistoryInfo> GetSAPParameters(EndPoint endPoint, string searchText)
        {
            return SAPDA.GetSAPParameters(endPoint, searchText);
        }

        public IList<SAPParameterHistoryInfo> GetSAPParameters(EndPoint endPoint)
        {
            return SAPDA.GetSAPParameters(endPoint);
        }

        public void AddSAPParameterInfo(IList<SAPParameterHistoryInfo> lstParameters)
        {
            try
            {

                SAPDA.AddSAPParameterHistoryInfoBulk(lstParameters);

                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Added {0} SAP Parameter History objects", lstParameters.Count.ToString()));

                var listParameters = lstParameters.IterateConvert<SAPParameterHistoryInfo, SAPParameterInfo>(
                   delegate(SAPParameterHistoryInfo _Parameter)
                   {
                       return this.getParameterInfo(_Parameter);
                   });

                SAPDA.DeleteAll<SAPParameterInfo>(lstParameters.First().TenantId, lstParameters.First().EndPointSystemId);
                SAPDA.AddSAPParameterInfoBulk(listParameters);
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Added {0} SAP Parameter objects", lstParameters.Count.ToString()));

            }
            catch (Exception)
            {

                throw;
            }
        }

        private SAPParameterInfo getParameterInfo(SAPParameterHistoryInfo _Parameter)
        {
            SAPParameterInfo SAPParameterInfo = new SAPParameterInfo();

            try
            {
                SAPParameterInfo.PARAM_CODE = _Parameter.PARAM_CODE;
                SAPParameterInfo.TenantId = _Parameter.TenantId;
                SAPParameterInfo.EndPointSystemId = _Parameter.EndPointSystemId;
                SAPParameterInfo.PARAM_DESC = _Parameter.PARAM_DESC;
                SAPParameterInfo.PARAM_VALUE = _Parameter.PARAM_VALUE;
                SAPParameterInfo.TargetSysId = _Parameter.TargetSysId;
                SAPParameterInfo.CreatedBy = _Parameter.CreatedBy;
                SAPParameterInfo.CreatedDate = _Parameter.CreatedDate;
                SAPParameterInfo.ModifiedBy = _Parameter.ModifiedBy;
                SAPParameterInfo.ModifiedDate = _Parameter.ModifiedDate;


                return SAPParameterInfo;
            }
            catch (Exception)
            {

                throw;
            }
        }



        public void AddSAPProfileInfo(IList<SAPProfileHistoryInfo> list)
        {
            try
            {
                try
                {

                    SAPDA.AddSAPProfileHistoryInfoBulk(list);

                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Added {0} SAP profileHistory objects", list.Count.ToString()));

                    var listprofiles = list.IterateConvert<SAPProfileHistoryInfo, SAPProfileInfo>(
                       delegate(SAPProfileHistoryInfo _profile)
                       {
                           return this.getProfileInfo(_profile);
                       });

                    SAPDA.DeleteAll<SAPProfileInfo>(list.First().TenantId, list.First().EndPointSystemId);
                    SAPDA.AddSAPProfileInfoBulk(listprofiles);
                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Added {0} SAP profile objects", list.Count.ToString()));

                }
                catch (Exception)
                {

                    throw;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private SAPProfileInfo getProfileInfo(SAPProfileHistoryInfo SAPProfileInfoHistory)
        {
            SAPProfileInfo SAPProfileInfo = new SAPProfileInfo();

            try
            {
                SAPProfileInfo.PROFILE_CODE = SAPProfileInfoHistory.PROFILE_CODE;
                SAPProfileInfo.TenantId = SAPProfileInfoHistory.TenantId;
                SAPProfileInfo.EndPointSystemId = SAPProfileInfoHistory.EndPointSystemId;
                SAPProfileInfo.PROFILE_DESC = SAPProfileInfoHistory.PROFILE_DESC;
                SAPProfileInfo.TargetSysId = SAPProfileInfoHistory.TargetSysId;
                SAPProfileInfo.CreatedBy = SAPProfileInfoHistory.CreatedBy;
                SAPProfileInfo.CreatedDate = SAPProfileInfoHistory.CreatedDate;
                SAPProfileInfo.ModifiedBy = SAPProfileInfoHistory.ModifiedBy;
                SAPProfileInfo.ModifiedDate = SAPProfileInfoHistory.ModifiedDate;


                return SAPProfileInfo;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<SAPUserinfo> GetAllSAPUsers()
        {
            try
            {
                IList<SAPUserinfo> SAPUsers = SAPDA.GetAllSAPUserInfo();
                return SAPUsers;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPUserinfo> GetAllSAPUsers(long EndpointSystemId, string FilterText)
        {
            try
            {
                return SAPDA.GetAllSAPUserInfo(EndpointSystemId, FilterText);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CheckSAPUserNameExists(string userName, long endpointSystemId)
        {
            try
            {
                return SAPDA.CheckSAPUserNameExists(userName, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }
        public IList<SAPTCodeInfo> GetAllSapTCodes(long EndpointSystemId)
        {
            try
            {

                IList<SAPTCodeInfo> SAPTCodes = SAPDA.GetAllTCodes(EndpointSystemId);


                return SAPTCodes;
            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public Dictionary<long, string> GetAllSapTCodesForCompliance(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetAllSapTCodes(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IList<SAPTCodeInfo> GetAllTCodesWithoutTrackingCodes(long EndpointSystemId)
        {
            try
            {

                return SAPDA.GetAllTCodesWithoutTrackingCodes(EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public SAPUserinfo Dessemble(SAPUserinfo _aggregateRoot)
        {
            throw new NotImplementedException();
        }

        public IList<SAPUserinfo> GetAllUnMappedSAPUsers()
        {
            try
            {
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void UpdateSAPUserInfoOnAttributeMap(System.Data.DataTable dt)
        {
            try
            {
                SAPDA.UpdateSAPUserInfoOnAttributeMap(dt);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddSAPRoleInfo(IList<SAPRoleHistoryInfo> lstRoles)
        {
            try
            {
                try
                {
                    SAPDA.AddSAPRoleHistoryInfoBulk(lstRoles);

                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Added {0} SAP RoleHistory objects", lstRoles.Count.ToString()));

                    var listRoles = lstRoles.IterateConvert<SAPRoleHistoryInfo, SAPRoleInfo>(
                       delegate(SAPRoleHistoryInfo _Role)
                       {
                           return this.getRoleInfo(_Role);
                       });

                    SAPDA.DeleteAll<SAPRoleInfo>(lstRoles.First().TenantId, lstRoles.First().EndPointSystemId);
                    SAPDA.AddSAPRoleInfoBulk(listRoles);
                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Added {0} SAP Role objects", lstRoles.Count.ToString()));

                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private SAPRoleInfo getRoleInfo(SAPRoleHistoryInfo SAPRoleInfoHistory)
        {
            SAPRoleInfo SAPRoleInfo = new SAPRoleInfo();
            try
            {
                SAPRoleInfo.ROLE_CODE = SAPRoleInfoHistory.ROLE_CODE;
                SAPRoleInfo.TenantId = SAPRoleInfoHistory.TenantId;
                SAPRoleInfo.EndPointSystemId = SAPRoleInfoHistory.EndPointSystemId;
                SAPRoleInfo.ROLE_DESC = SAPRoleInfoHistory.ROLE_DESC;
                SAPRoleInfo.TargetSysId = SAPRoleInfoHistory.TargetSysId;
                SAPRoleInfo.CreatedBy = SAPRoleInfoHistory.CreatedBy;
                SAPRoleInfo.CreatedDate = SAPRoleInfoHistory.CreatedDate;
                SAPRoleInfo.ModifiedBy = SAPRoleInfoHistory.ModifiedBy;
                SAPRoleInfo.ModifiedDate = SAPRoleInfoHistory.ModifiedDate;


                return SAPRoleInfo;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public IList<SAPUserinfo> GetSAPUserInfoByDIUserId(long DIUserId)
        {
            return SAPDA.GetSAPUserInfoByDIUserId(DIUserId);
        }

        public List<SAPUserinfo> GetPrimarySAPUserByDIUserId(long DIUserId, long endpointSystemId)
        {
            return SAPDA.GetPrimarySAPUserByDIUserId(DIUserId, endpointSystemId);
        }

        public List<SAPUserinfo> GetActivePrimarySAPUserByDIUserId(int DIUserId, long endpointSystemId)
        {
            try
            {
                return SAPDA.GetActivePrimarySAPUserByDIUserId(DIUserId, endpointSystemId);
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Error Occured while getting All sap DIUserId={0}", DIUserId.ToString()), ex);
                throw ex;
            }
        }

        public SAPUserinfo GetLockedSAPUserByDIUserId(long DIUserId, long endpointSystemId)
        {
            return SAPDA.GetLockedSAPUserByDIUserId(DIUserId, endpointSystemId);
        }

        public List<SAPUserinfo> GetLockedSAPUsersByDIUserId(int DIUserId, long endpointSystemId)
        {
            return SAPDA.GetLockedSAPUsersByDIUserId(DIUserId, endpointSystemId);
        }

        public SAPUserinfo GetSAPUserByUserName(string userName, long endpointSystemId)
        {
            return SAPDA.GetSAPUserByUserName(userName, endpointSystemId);
        }

        public SAPUserinfo GetSAPUserByUserId(long userId)
        {
            return SAPDA.GetSAPUserByUserId(userId);
        }

        public void GetSAPUserGroupRoleProfile(int UserId, out IList<SAPGroupInfo> groupList, out IList<SAPCompositeRoleInfo> roleList, out IList<SAPProfileInfo> profileList, out IList<SAPSingleRoleInfo> singleRoleList, out IList<SAPRoleInfo> sapRoleList)
        {
            SAPDA.GetSAPUserGroupRoleProfile(UserId, out groupList, out roleList, out profileList, out singleRoleList, out sapRoleList);
        }

        public IList<SAPTCodeInfo> GetTcodeByUserId(int sapUserId)
        {
            return SAPDA.GetTcodeByUserId(sapUserId);
        }

        public DataSet GetObjectsByUserId(int sapUserId)
        {
            return SAPDA.GetObjectsByUserId(sapUserId);
        }

        public DataSet GetAccessByUserId(int sapUserId)
        {
            return SAPDA.GetAccessByUserId(sapUserId);
        }

        public DataSet GetObjectsByTCode(string TCode)
        {
            return SAPDA.GetObjectsByTCode(TCode);
        }

        public IList<SAPRoleTCode> GetTcodeAndRoleByUserId(int sapUserId, long EndpointSystemId)
        {
            return SAPDA.GetTcodeAndRoleByUserId(sapUserId, EndpointSystemId);
        }


        public IList<SAPUserinfo> GetAllUnMappedAdsUsers(EndPoint EndPoint)
        {
            try
            {
                return SAPDA.GetAllUnMappedSAPUsers(EndPoint);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataSet GetTCodeByRole(string Role_Code)
        {
            try
            {
                return SAPDA.GetTCodeByRole(Role_Code);
            }
            catch (Exception)
            {

                throw;
            }
        }




        public List<string> getChildRolesByCompositeRoleCode(List<string> compositeRolesList)
        {
            return SAPDA.getChildRolesByCompositeRoleCode(compositeRolesList);
        }
        public void DeleteAll<T>()
        {
            try
            {
                //ActiveDirectoryDA.DeleteAll<T>();
                SAPDA.DeleteAll<T>();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteAll<T>(string _tenantId, long _endpointSystemId)
        {
            try
            {
                SAPDA.DeleteAll<T>(_tenantId, _endpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //Newly Added

        public void ExecuteQry(string cmd)
        {
            EndpointObjectDA EndpointObjectDA = new EndpointObjectDA();
            EndpointObjectDA.ExecuteQry(cmd);

        }

        public IList<SAPUserinfo> GetUsersFromDB()
        {
            try
            {
                return SAPDA.GetUsersFromDB();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<SAPUserinfo> GetUsersFromDB(long endpointId)
        {
            try
            {
                return SAPDA.GetUsersFromDB(endpointId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<SAPUserinfo> GetDeletedUsers(long EndpointId, Process discoveryProcess)
        {
            try
            {

                return SAPDA.GetDeletedUsers(EndpointId, discoveryProcess);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RemoveSAPuser(IList<SAPUserinfo> listDelUser)
        {
            try
            {
                foreach (SAPUserinfo user in listDelUser)
                {
                    SAPDA.RemoveSAPuser(user);
                    reconcilationFacade.DeleteAccount((int)user.EndPointSystemId, user.Id);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SaveGroups(IList<SAPGroupInfo> list, long EndpointSystemId)
        {
            try
            {
                SAPDA.SaveGroups(list, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SAPGroupInfo GetgroupByGROUP_CODE(string groupCode, long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetgroupByGROUP_CODE(groupCode, EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<SAPGroupInfo> GetDeletedGroups(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedGroups(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RemoveSAPGroups(IList<SAPGroupInfo> listDelGroups)
        {
            try
            {
                foreach (SAPGroupInfo group in listDelGroups)
                {
                    SAPDA.RemoveSAPGroups(group);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SAPProfileInfo GetProfileByPROFILE_CODE(string profileCode, long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetProfileByPROFILE_CODE(profileCode, EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SaveProfiles(IList<SAPProfileInfo> list, long EndpointSystemId)
        {
            try
            {

                SAPProfileInfo sapexistingProfile = null;

                foreach (SAPProfileInfo profile in list)
                {

                    sapexistingProfile = SAPDA.GetProfileByPROFILE_CODE(profile.PROFILE_CODE, EndpointSystemId);
                    if (sapexistingProfile == null)
                    {
                        SAPDA.AddProfile(profile);
                    }
                    else
                    {
                        profile.Id = sapexistingProfile.Id;
                        profile.PROFILE_CODE = sapexistingProfile.PROFILE_CODE;
                        SAPDA.UpdateProfile(profile);
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SavePositions(IList<SAPPosition> list, long EndpointSystemId)
        {
            try
            {

                SAPPosition sapexistingPositions = null;

                foreach (SAPPosition pos in list)
                {

                    sapexistingPositions = SAPDA.GetPositionByPosCode(pos.Code, EndpointSystemId);
                    if (sapexistingPositions == null)
                    {
                        SAPDA.AddPosition(pos);
                    }
                    else
                    {
                        SAPDA.UpdatePosition(sapexistingPositions);
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        //SaveCompositeProfiles
        public void SaveCompositeProfiles(IList<SAPCompositeProfileInfo> list, long EndpointSystemId)
        {
            try
            {

                SAPCompositeProfileInfo sapexistingProfile = null;

                foreach (SAPCompositeProfileInfo profile in list)
                {

                    sapexistingProfile = SAPDA.GetCompositeProfileByPROFILE_CODE(profile.PROFILE_CODE, profile.SUBProfile_Code, EndpointSystemId);
                    if (sapexistingProfile == null)
                    {
                        SAPDA.AddCompositeProfile(profile);
                    }
                    else
                    {
                        SAPDA.UpdateCompositeProfile(sapexistingProfile);
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }



        public IList<SAPProfileInfo> GetDeletedProfiles(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedProfiles(EndpointId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void RemoveSAPProfiles(IList<SAPProfileInfo> listprofile)
        {
            try
            {
                foreach (SAPProfileInfo profile in listprofile)
                {
                    SAPDA.RemoveSAPProfiles(profile);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<SAPCompositeProfileInfo> GetDeletedCompositeProfiles(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedCompositeProfiles(EndpointId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void RemoveSAPCompositeProfiles(IList<SAPCompositeProfileInfo> listprofile)
        {
            try
            {
                foreach (SAPCompositeProfileInfo profile in listprofile)
                {
                    SAPDA.RemoveSAPCompositeProfiles(profile);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SaveAuthorizations(IList<SAPAuthorizationProfile> list, long EndpointSystemId)
        {
            try
            {
                SAPDA.SaveAuthorizations(list, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPAuthorizationProfile> GetDeletedAuthorizations(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedAuthorizations(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RemoveSAPAuthorizations(IList<SAPAuthorizationProfile> listDelAuthorizations)
        {
            try
            {
                foreach (SAPAuthorizationProfile auth in listDelAuthorizations)
                {
                    SAPDA.RemoveSAPAuthorization(auth);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveSAPObjectsAuthMap(IList<SAPAuthorizationToObject> list, long EndpointSystemId)
        {
            try
            {

                SAPDA.SaveSAPObjectsAuthMap(list, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPAuthorizationToObject> GetDeletedSAPObjectsAuthMap(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedSapObjectsAuthMap(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IList<SAPAuthorizationToObject> GetSapObjectsAuthMap(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedSapObjectsAuthMap(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoveSapObjectsAuthMap(IList<SAPAuthorizationToObject> listSapObjects)
        {
            try
            {
                foreach (SAPAuthorizationToObject sObject in listSapObjects)
                {
                    SAPDA.RemoveSAPObjectsAuthMap(sObject);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SaveSAPFieldValueMap(IList<SAPFieldValueMapping> list, long EndpointSystemId)
        {
            try
            {

                SAPDA.SaveSAPFieldValueMap(list, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPFieldValueMapping> GetDeletedSAPFieldValueMap(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedSAPFieldValueMap(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IList<SAPFieldValueMapping> GetSapFieldValueMap(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedSAPFieldValueMap(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoveSapFieldValueMap(IList<SAPFieldValueMapping> listSapObjects)
        {
            try
            {
                foreach (SAPFieldValueMapping sObject in listSapObjects)
                {
                    SAPDA.RemoveSAPFieldValueMap(sObject);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveSAPObjectValue(IList<SAPObjectValues> list, long EndpointSystemId)
        {
            try
            {
                SAPDA.SaveSAPObjectValue(list, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPObjectValues> GetDeletedSAPObjectValues(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedObjectValue(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IList<SAPObjectValues> GetSapObjectValue(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedObjectValue(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoveSapObjectValue(IList<SAPObjectValues> listSapObjects)
        {
            try
            {
                foreach (SAPObjectValues sObject in listSapObjects)
                {
                    SAPDA.RemoveSAPObjectValue(sObject);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveSAPTCodeObjectMap(IList<SAPTCodeObjectMap> list, long EndpointSystemId)
        {
            try
            {
                SAPDA.SaveSAPTCodeObjectMap(list, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPTCodeObjectMap> GetDeletedSAPTCodeObjectMap(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedSAPTCodeObjectMap(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoveSapTCodeObjectMap(IList<SAPTCodeObjectMap> listSapObjects)
        {
            try
            {
                foreach (SAPTCodeObjectMap sObject in listSapObjects)
                {
                    SAPDA.RemoveSAPTCodeObjectMap(sObject);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SaveSapObject(IList<SAPObjectInfo> list, long EndpointSystemId)
        {
            try
            {
                SAPDA.SaveSapObject(list, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPObjectInfo> GetDeletedSAPObjects(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedSAPObject(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }




        public void RemoveSapObject(IList<SAPObjectInfo> listSapObjects)
        {
            try
            {
                foreach (SAPObjectInfo sObject in listSapObjects)
                {
                    SAPDA.RemoveSAPObject(sObject);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SaveSapRoleAuthMap(IList<SAPRoleToAuthMap> list, int EndpointSystemId)
        {
            try
            {
                SAPDA.SaveSapRoleAuthMap(list, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPRoleToAuthMap> GetDeletedSAPRoleAuthMap(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedSAPRoleToAuthMap(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoveSapRoleAuthMap(IList<SAPRoleToAuthMap> listSapObjects)
        {
            try
            {
                foreach (SAPRoleToAuthMap sObject in listSapObjects)
                {
                    SAPDA.RemoveSAPRoleToAuthMap(sObject);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SaveActivtyInfo(IList<SAPActivityInfo> list, long EndpointSystemId)
        {
            try
            {
                SAPDA.SaveSAPSaveActivityInfo(list, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SaveSAPUserProfile(IList<SapUserProfile> list, long EndpointSystemId)
        {
            try
            {

                SAPDA.SaveSapUserProfile(list, EndpointSystemId);



            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveSAPRoleProfile(IList<SapRoleProfileMapping> list, long EndpointSystemId)
        {
            try
            {

                SAPDA.SaveSapRoleProfile(list, EndpointSystemId);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SapUserProfile> GetDeletedSAPUserProfile(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedSAPUserProfile(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IList<SapUserProfile> GetSapUserProfile(int EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedSAPUserProfile(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoveSAPUserProfile(IList<SapUserProfile> listSapObjects)
        {
            try
            {
                foreach (SapUserProfile sObject in listSapObjects)
                {
                    SAPDA.RemoveSAPUserProfile(sObject);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SAPParameterInfo GetParameterByPARAM_CODE(string param_code, long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetParameterByPARAM_CODE(param_code, EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void ClearParameters(long EndpointSystemId)
        {
            try
            {
                SAPDA.ClearParameters(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }


        }
        public void SaveParameter(IList<SAPParameterInfo> list, long EndpointSystemId)
        {
            try
            {

                SAPParameterInfo sapexistingParameter = null;
                SAPDA.SaveParameter(list, EndpointSystemId);



            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveSapPositionsBulk(List<SAPPosition> SapPositions)
        {
            try
            {
                SAPDA.SaveSapPositionsBulk(SapPositions);
            }

            catch (Exception)
            {
                throw;
            }

        }

        public void ClearSapPositions(long EndpointSystemId)
        {
            try
            {
                SAPDA.ClearALLSapPositions(EndpointSystemId);
            }

            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPParameterInfo> GetDeletedSAPParameters(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedSAPParameters(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RemoveSAPParameter(IList<SAPParameterInfo> listDelParameter)
        {
            try
            {
                foreach (SAPParameterInfo parameter in listDelParameter)
                {
                    SAPDA.RemoveSAPParameter(parameter);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SaveRoles(IList<SAPRoleInfo> list, long EndpointSystemId)
        {
            SAPDA.SaveRoles(list, EndpointSystemId);

        }

        public void UpdateRolesAsSingleIfNull(long EndpointSystemId)
        {
            SAPDA.UpdateRolesAsSingleIfNull(EndpointSystemId);
        }



        public SAPRoleInfo GetRoleByROLE_CODE(string roleCode, long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetRoleByROLE_CODE(roleCode, EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<SAPRoleInfo> GetRolesFromDB(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetRolesFromDB(EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public IList<SAPProfileInfo> GetProfilesFromDB(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetProfilesFromDB(EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public IList<SAPRoleInfo> GetDeletedSapRoles(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedSapRoles(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RemoveSapRoles(IList<SAPRoleInfo> listDelSapRoles)
        {
            try
            {
                foreach (SAPRoleInfo role in listDelSapRoles)
                {
                    SAPDA.RemoveSapRoles(role);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SaveSAPCompositeRoles(IList<SAPCompositeRoleInfo> list, long EndpointSystemId)
        {
            try
            {

                SAPCompositeRoleInfo sapexistingCompositeRole = null;

                foreach (SAPCompositeRoleInfo compositerole in list)
                {

                    sapexistingCompositeRole = SAPDA.GetSAPCompositeRoleByCompositeRole_Code(compositerole.CompositeRole_Code, EndpointSystemId);
                    if (sapexistingCompositeRole == null)
                    {
                        SAPDA.AddSAPCompositeRole(compositerole);
                    }
                    else
                    {
                        compositerole.CompositeRole_Code = sapexistingCompositeRole.CompositeRole_Code;
                        compositerole.Id = sapexistingCompositeRole.Id;
                        compositerole.CreatedBy = sapexistingCompositeRole.CreatedBy;
                        compositerole.CreatedDate = sapexistingCompositeRole.CreatedDate;
                        SAPDA.UpdateSAPCompositeRole(compositerole);
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public SAPCompositeRoleInfo GetSAPCompositeRoleByCompositeRole_Code(string CompositeRole_Code, long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetSAPCompositeRoleByCompositeRole_Code(CompositeRole_Code, EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<SAPCompositeRoleInfo> SAPCompositeRolesFromDB(long EndpointSystemId)
        {
            try
            {
                return SAPDA.SAPCompositeRolesFromDB(EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<SAPCompositeRoleInfo> GetDeletedSAPCompositeRoleInfo(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedSAPCompositeRoleInfo(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RemoveSAPCompositeRole(IList<SAPCompositeRoleInfo> listDelCompositeRoles)
        {
            try
            {
                foreach (SAPCompositeRoleInfo compRole in listDelCompositeRoles)
                {
                    SAPDA.RemoveSAPCompositeRole(compRole);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }



        public void SaveSAPSingleRoles(IList<SAPSingleRoleInfo> list, long EndpointSystemId)
        {
            try
            {
                SAPDA.SaveSingleRoles(list, EndpointSystemId);


            }
            catch (Exception)
            {
                throw;
            }
        }
        public SAPSingleRoleInfo GetSAPSingleRoleBySingleRole_Code(string SingleRole_Code, long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetSAPSingleRoleBySingleRole_Code(SingleRole_Code, EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<SAPSingleRoleInfo> GetSingleRolesFromDB(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetSingleRolesFromDB(EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<SAPSingleRoleInfo> GetDeletedSAPSingleRole(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedSAPSingleRole(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RemoveSapSingleRole(IList<SAPSingleRoleInfo> listDelSapSingleRoles)
        {
            try
            {
                foreach (SAPSingleRoleInfo singleRole in listDelSapSingleRoles)
                {
                    SAPDA.RemoveSapSingleRole(singleRole);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }



        public void SaveTheTCode(SAPTCodeInfo SapTCode)
        {
            SAPTCodeInfo sapexistingTcode = null;
            try
            {
                sapexistingTcode = SAPDA.GetTcodeByTCODE_Code(SapTCode.TCODE_Code.Trim());
                if (sapexistingTcode == null)
                {
                    SAPDA.AddTcode(SapTCode);
                }
                else
                {
                    SapTCode.Id = sapexistingTcode.Id;
                    SapTCode.TCODE_Code = sapexistingTcode.TCODE_Code;
                    SapTCode.CreatedBy = sapexistingTcode.CreatedBy;
                    SapTCode.CreatedDate = sapexistingTcode.CreatedDate;

                    SAPDA.UpdateTcode(SapTCode);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveTcodes(IList<SAPTCodeInfo> list, IList<SAPTCodeInfo> lstSapTCodes, long EndpointSystemId)
        {
            try
            {
                SAPDA.SaveTCodes(list, lstSapTCodes, EndpointSystemId);
                //SAPTCodeInfo sapexistingTcode = null;

                //foreach (SAPTCodeInfo tcode in list)
                //{

                //    sapexistingTcode = SAPDA.GetTcodeByTCODE_Code(tcode.TCODE_Code.Trim());
                //    if (sapexistingTcode == null)
                //    {
                //        SAPDA.AddTcode(tcode);
                //    }
                //    else
                //    {
                //        tcode.TCODE_Id = sapexistingTcode.TCODE_Id;
                //        tcode.TCODE_Code = sapexistingTcode.TCODE_Code;
                //        tcode.CreatedBy = sapexistingTcode.CreatedBy;
                //        tcode.CreatedDate = sapexistingTcode.CreatedDate;

                //        SAPDA.UpdateTcode(tcode);
                //    }
                //}

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveSapRoleTCodeMapping(IList<SapRoleTCodeMapping> lstSapRoleTCodeMapping, long EndpointSystemId)
        {
            try
            {
                SAPDA.SaveSapRoleTCodeMapping(lstSapRoleTCodeMapping, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SAPTCodeInfo GetTcodeByTCODE_Code(string tCode_Code)
        {
            try
            {
                return SAPDA.GetTcodeByTCODE_Code(tCode_Code);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<SAPTCodeInfo> GetDeletedSAPTCodes(long EndpointId)
        {
            try
            {
                return SAPDA.GetDeletedSAPTCodes(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RemoveSAPTCodes(IList<SAPTCodeInfo> listDelTCodes)
        {
            try
            {
                foreach (SAPTCodeInfo tcode in listDelTCodes)
                {
                    SAPDA.RemoveSAPTCode(tcode);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveSAPUserGroupMap(IList<SAPConnectorDA.SAPUserGroup> listSAPUserGroup, long EndpointSystemId)               //IList<SAPUserGroup> listSAPUserGroup, int EndpointSystemId)
        {
            try
            {
                SAPDA.SaveSAPUserGroupMap(listSAPUserGroup, EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void ClearSAPUserGroupMap(long EndpointSystemId)
        {
            try
            {
                SAPDA.ClearSAPUserGroupMap(EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void ClearSAPUserGroupMap(long EndpointSystemId, long UserId)
        {
            try
            {
                SAPDA.ClearSAPUserGroupMap(EndpointSystemId, UserId);
            }
            catch (Exception)
            {

                throw;
            }
        }



        public void SaveSAPUserProfileMap(IList<SAPConnectorDA.SAPUserProfile> listSAPUserProfile, long EndpointSystemId)
        {
            try
            {
                SAPDA.SaveSAPUserProfileMap(listSAPUserProfile, EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void ClearSAPUserProfileMap(long EndpointSystemId)
        {
            try
            {
                SAPDA.ClearSAPUserProfileMap(EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void ClearSAPUserProfileMap(long EndpointSystemId, long UserId)
        {
            try
            {
                SAPDA.ClearSAPUserProfileMap(EndpointSystemId, UserId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SaveSAPUserParameterMap(IList<SAPConnectorDA.SAPUserParameter> listSAPUserParameter, long EndpointSystemId)
        {
            try
            {
                SAPDA.SaveSAPUserParameterMap(listSAPUserParameter, EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SaveCompositeChildRoleMap(IList<SAPConnectorDA.SAPCompositeChildRoleMap> listCompositeChildRoleMap, long EndpointSystemId)
        {
            try
            {
                SAPDA.SaveCompositeChildRoleMap(listCompositeChildRoleMap, EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }



        public void SaveSAPUserRoleMap(IList<SAPConnectorDA.SAPUserRole> listSAPUserRole, long EndpointSystemId)
        {
            try
            {
                SAPDA.SaveSAPUserRoleMap(listSAPUserRole, EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void ClearSAPUserRoleMap(long EndpointSystemId)
        {
            try
            {
                SAPDA.ClearSAPUserRoleMap(EndpointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void ClearSAPUserRoleMap(long EndpointSystemId, long UserId)
        {
            try
            {
                SAPDA.ClearSAPUserRoleMap(EndpointSystemId, UserId);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public void SaveSAPSingleRoleTcodeMap(IList<SAPConnectorDA.SapSingleRoleTcodes> listSAPSingleRoleTcodes, long EndpointSystemId, bool ClearAllFirst)
        {
            try
            {
                SAPDA.SaveSAPSingleRoleTcodeMap(listSAPSingleRoleTcodes, EndpointSystemId, ClearAllFirst);
            }
            catch (Exception)
            {

                throw;
            }
        }



        public void AddSingleRoleTCodes(DataTable dtAlltcode, long EndpointSystemId, IList<string> DeltaRoles, bool IsFirstTimeDiscovery)
        {
            SAPDA.AddSingleRoleTCodes(dtAlltcode, EndpointSystemId, DeltaRoles, IsFirstTimeDiscovery);
        }




        public SAPPosition GetSAPPositionByPositionCode(string code, long endpointSystemId)
        {
            return SAPDA.GetSAPPositionByPositionCode(code, endpointSystemId);
        }

        public IList<SAPUserHistoryInfo> GetAllSAPHistoryUsers()
        {
            try
            {
                return SAPDA.GetAllSAPHistoryUsers();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<EndpointObjects> GetSAPObjects()
        {
            try
            {
                return SAPDA.GetSAPObjects();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPUserReportData> GetAllSAPUsersReportData(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetAllSAPUsersReportData(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPProfileReportData> GetAllSAPProfilesReportData(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetAllSAPProfilesReportData(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public IList<SAPGroupReportData> GetAllSAPGroupsReportData(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetAllSAPGroupsReportData(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public IList<SAPRoleReportData> GetAllSAPRolesReportData(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetAllSAPRolesReportData(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public IList<SAPTCodeReportData> GetAllSapTCodesReportData(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetAllSapTCodesReportData(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SapRoleToAuthorizationReportData> GetAllSapRolesToAuthorizationData(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetAllSapRolesToAuthorizationData(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SapRoleTCodeMapping> GetAllSapRolesToTCodeData(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetAllSapRolesToTCodeData(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SAPGroupInfo> GetSAPGroupsByUserId(long UserId)
        {
            try
            {
                return SAPDA.GetSAPGroupsByUserId(UserId);
            }
            catch
            {
                throw;
            }
        }

        public List<SAPProfileInfo> GetSAPProfilesByUserId(long UserId)
        {
            try
            {
                return SAPDA.GetSAPProfilesByUserId(UserId);
            }
            catch
            {
                throw;
            }
        }

        public List<SAPRoleInfo> GetSAPRolesByUserId(long UserId)
        {
            try
            {
                return SAPDA.GetSAPRolesByUserId(UserId);
            }
            catch
            {
                throw;
            }
        }

        public List<SAPRoleToAuthMap> GetSapRoleToAuthBasedOnRoles(List<string> DistinctRoles, long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetSapRoleToAuthBasedOnRoles(DistinctRoles, EndpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public List<SAPTCodeInfo> GetSapRolesToTCodeData(string RoleCode, long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetSapRolesToTCodeData(RoleCode, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<string> GetTCodesBySAPRole(string RoleCode, long EndpointSystemId)
        {
            try
            {
                List<string> tcodes = new List<string>();
                var roleTCodes = SAPDA.GetTCodesBySAPRole(RoleCode, EndpointSystemId);

                foreach (SAPRoleToAuthMap item in roleTCodes)
                {
                    if (string.IsNullOrEmpty(item.ToValue))
                    {
                        if (item.FromValue.Contains("*"))
                        {
                            var tcodesFromRange = SAPDA.GetTCodesByPattern(item.FromValue, EndpointSystemId);
                            tcodes.AddRange(tcodesFromRange);
                        }
                        else
                        {
                            tcodes.Add(item.FromValue);
                        }
                    }
                    else
                    {
                        var tcodesFromRange = SAPDA.GetTCodesByRange(item.FromValue, item.ToValue, EndpointSystemId);
                        tcodes.AddRange(tcodesFromRange);
                    }
                }

                return tcodes.Distinct().ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SAPTCodeInfo> GetSapRolesToTCodeDataWithRoles(string RoleCode, long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetSapRolesToTCodeDataWithRoles(RoleCode, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<AttestationReviewPageSAP_Level1GridData> GetAuthorization(string RoleCode, long EndPointSystemId)
        {
            try
            {
                return SAPDA.GetAuthorization(RoleCode, EndPointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void DeleteAuthorizations(long EndpointId)
        {
            try
            {
                SAPDA.DeleteAuthorizations(EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteSAPTCodeObjectMap(long EndpointSystemId)
        {
            try
            {
                SAPDA.DeleteSAPTCodeObjectMap(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteSAPObjectsAuthMap(long EndpointSystemId)
        {
            try
            {

                SAPDA.DeleteSAPObjectsAuthMap(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteSapRoleAuthMap(long EndpointSystemId)
        {
            try
            {
                SAPDA.DeleteSapRoleAuthMap(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ClearHRP1001Dump(long endpointSystemId)
        {
            try
            {
                SAPDA.ClearHRP1001Dump(endpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AddHRP1001Dump(SAPHRP1001 hr1001)
        {
            try
            {
                SAPDA.AddHRP1001Dump(hr1001);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateHRP1001Dump(SAPHRP1001 hr1001)
        {
            try
            {
                SAPDA.UpdateHRP1001Dump(hr1001);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SAPHRP1001 GetHRP1001Dump(SAPHRP1001 hr1001)
        {
            try
            {
                return SAPDA.GetHRP1001Dump(hr1001);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteHRP1001Dump(SAPHRP1001 hr1001)
        {
            try
            {
                SAPDA.DeleteHRP1001Dump(hr1001);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<string> GetRolesByPosition(string position, long endpointSystemId)
        {
            try
            {
                return SAPDA.GetRolesByPosition(position, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public List<string> GetPositionsByUser(string logonName, long endpointSystemId)
        {
            try
            {
                return SAPDA.GetPositionsByUser(logonName, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetPositionTextByUser(string logonName, long endpointSystemId)
        {
            try
            {
                return SAPDA.GetPositionTextByUser(logonName, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }


        public SAPEPUserInfo GetSAPEPUserInfoByUserId(int UserId)
        {
            try
            {
                return SAPDA.GetSAPEPUserInfoByUserId(UserId);
            }
            catch
            {
                throw;
            }
        }

        public List<SAPUserinfo> GetSAPUsersToAssignEPGroup(long endpointSystemId)
        {
            try
            {
                return SAPDA.GetSAPUsersToAssignEPGroup(endpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public IList<SAPRoleInfo> GetSAPRoles(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetAllSapRoles(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SAPAuthorizationToObject GetSapObjectByObjectCode(string ObjectCode)
        {
            try
            {
                return SAPDA.GetSapObjectByObjectCode(ObjectCode);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public SAPAuthorizationToObject GetSapObjectByObjectCodeAndEndpointId(string ObjectCode, long EndpointId)
        {
            try
            {
                return SAPDA.GetSapObjectByObjectCodeAndEndpointId(ObjectCode, EndpointId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<SAPUserinfo> GetAllUsersInARole(string RoleCode, long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetAllUsersInARole(RoleCode, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SAPUserinfo> GetAllUsersInProfile(string ProfileCode, long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetAllUsersInProfile(ProfileCode, EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPUserinfo> GetAllUsersInObject(string TCode, string ObjectCode, string FieldCode, string Value)
        {
            try
            {
                return SAPDA.GetAllUsersInObject(TCode, ObjectCode, FieldCode, Value);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<SAPUserinfo> GetAllUsersInATCode(string TCode, long EndpointId)
        {
            try
            {

                return SAPDA.GetAllUsersInATCode(TCode, EndpointId);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<string> GetSapFields(long EndpointId, string ObjCode)
        {
            try
            {
                return SAPDA.GetSapFields(EndpointId, ObjCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<long, string> GetAllAssignedTCodes(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetAllAssignedTCodes(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IList<SAPProfileInfo> GetSAPProfiles(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetProfilesFromDB(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, string> GetSAPAuthorizationObjects(string searchString, long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetSAPAuthorizationObjects(searchString, EndpointSystemId);

            }
            catch
            {
                throw;
            }
        }
        public List<string> GetSapFields(string searchString, long EndpointSystemId, string ObjectCode)
        {
            try
            {
                return SAPDA.GetSapFields(searchString, EndpointSystemId, ObjectCode);

            }
            catch
            {
                throw;
            }
        }
        public IList<SAPObjectInfo> GetSAPAuthorizationObjects(long EndpointSystemId)
        {
            try
            {
                return SAPDA.GetSAPAuthorizationObjects(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public SAPUserinfo GetSAPUserInfo(long EndpointUserId)
        {
            return SAPDA.GetSAPUserInfo(EndpointUserId);
        }

        public void GetAllMasterUsers(IList<SAPUserinfo> listsapUser, bool mapAll)
        {
            SAPDA.GetAllMasterUsersSAP(listsapUser, mapAll);



        }

    }

}
