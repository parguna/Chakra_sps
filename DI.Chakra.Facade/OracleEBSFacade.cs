#region Namespace

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Data;

using DI.Chakra.Model;
using DI.Chakra.Infrastructure;


using DI.Chakra.DataAccess;

#endregion

namespace DI.Chakra.Facade
{
    
    public class OracleEBSFacade
    {

        #region Variable Declaration

        private bool m_populate = false;
        private ReconcilationFacade reconcilationFacade = new ReconcilationFacade();

      
        private OracleEBSDA OEBSDA = null;

        #endregion

        #region Constructor

        public OracleEBSFacade()
        {
            OEBSDA = new OracleEBSDA();
           
        }

        #endregion


     

        public IList<EndpointObjects> GetORACLEObjects()
        {
            try
            {
                return OEBSDA.GetORACLEObjects();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<OracleEBSUserInfo> GetAllUsers()
        {
            try
            {
                return OEBSDA.GetAllUsersInfo();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<OracleEBSHRMSUserInfo> GetAllHRMSUsers()
        {
            try
            {
                return OEBSDA.GetAllHRMSUsersInfo();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<OracleEBSRoleInfo> GetOEBSRoles()
        {
            try
            {
                return OEBSDA.GetOEBSRoles();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<OracleEBSResponsibilitiesInfo> GetOEBSResponsibilities()
        {
            try
            {
                return OEBSDA.GetOEBSResponsibilities();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<OracleEBSUserInfo> GetUsers(int id)
        {
            try
            {
                return OEBSDA.GetUsers(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<OracleEBSRoleInfo> GetOEBSRole(int id)
        {
            try
            {
                return OEBSDA.GetOEBSRole(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<OracleEBSResponsibilitiesInfo> GetOEBSResponsibility(int id)
        {
            try
            {
                return OEBSDA.GetOEBSResponsibility(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public IList<OracleEBSHRMSUserInfo> GetAllHRMSUsers()
        //{
        //    try
        //    {
        //        return OEBSDA.GetAllHRMSUsersInfo();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public void AddOracleEBSRoleInfo(OracleEBSRoleInfo RoleInfo)
        {
            try
            {
                OEBSDA.AddOracleEBSRoleInfo(RoleInfo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AddOracleEBSResponsibilitiesInfo(OracleEBSResponsibilitiesInfo ResponsibilitiesInfo)
        {
            try
            {
                OEBSDA.AddOracleEBSResponsibilitiesInfo(ResponsibilitiesInfo);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void AddMenus(IList<OracleEBSMenusInfo> menus)
        {
            try
            {
                OEBSDA.AddOracleEBSMenus(menus);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddsubMenus(IList<OracleEBSSubmenusInfo> submenus)
        {
            try
            {
                OEBSDA.AddOracleEBSSubmenus(submenus);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public OracleEBSConfiguration GetConfiguration(int _systemid)
        {
            return OEBSDA.GetConfiguration(_systemid);
        }

        public void AddOracleEBSUserInfo(OracleEBSUserInfo UserInfo, bool IsNewlyAdded)
        {
            try
            {
                OEBSDA.AddOracleEBSUserInfo(UserInfo);
                reconcilationFacade.AddOrUpdateAccountInReconTable(UserInfo, IsNewlyAdded);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ChangeOracleEBSUserInfo(OracleEBSUserInfo UserInfo, bool IsNewlyAdded)
        {
            try
            {
                OEBSDA.ChangeOracleEBSUserInfo(UserInfo);
                reconcilationFacade.AddOrUpdateAccountInReconTable(UserInfo, IsNewlyAdded);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public void DeleteOracleEBSUserInfo(IList<OracleEBSUserInfo> UserInfo)
        {
            try
            {
                OEBSDA.DeleteOracleEBSUserInfo(UserInfo);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddOracleEBSHRMSUserInfo(OracleEBSHRMSUserInfo HRMSUserInfo)
        {
            try
            {
                OEBSDA.AddOracleEBSHRMSUserInfo(HRMSUserInfo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ChangeOracleEBSHRMSUserInfo(OracleEBSHRMSUserInfo HRMSUserInfo)
        {
            try
            {
                //OEBSDA.ChangeOracleEBSHRMSUserInfo(HRMSUserInfo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public OracleEBSUserInfo GetOracleEBSUserByUserId(long userId)
        {
            return OEBSDA.GetOracleEBSUserByUserId(userId);
        }

        public OracleEBSUserInfo GetOracleEBSUserByDIUserId(long diUserId)
        {
            return OEBSDA.GetOracleEBSUserByDIUserId(diUserId);
        }

        public OracleEBSUserInfo GetLockedOracleEBSUserByDIUserId(long DIUserId, long endpointSystemId)
        {
            return OEBSDA.GetLockedOracleEBSUserByDIUserId(DIUserId, endpointSystemId);
        }

        public List<OracleEBSUserInfo> GetPrimaryOracleEBSUserByDIUserId(long DIUserId, long endpointSystemId)
        {
            return OEBSDA.GetPrimaryOracleEBSUserByDIUserId(DIUserId, endpointSystemId);
        }

        public long AddOracleEBSUser(OracleEBSUserInfo _oraUser)
        {
            try
            {
                return OEBSDA.AddOracleEBSUser(_oraUser);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<OracleEBSRoleInfo> GetOracleEBSRolesByUserId(int userId, int endpointSystemId)
        {
            return OEBSDA.GetOracleEBSRoleByUserId(userId, endpointSystemId);
        }
        public List<OracleEBSRoleInfo> GetOracleEBSRolesByEndpointUserId(long userId, int endpointSystemId)
        {
            return OEBSDA.GetOracleEBSRolesByEndpointUserId(userId, endpointSystemId);
        }

        public List<OracleEBSResponsibilitiesInfo> GetOracleEBSResponsilbilitiesByUserId(long userId, int endpointSystemId)
        {
            return OEBSDA.GetOracleEBSResponsilbilitiesByUserId(userId, endpointSystemId);
        }

        public bool CheckOracleEBSUserNameExists(string userName, int endpointSystemId)
        {
            try
            {
                return OEBSDA.CheckOracleEBSUserNameExists(userName, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public IList<OracleEBSRoleInfo> GetAllOracleEBSRoles()
        {
            try
            {
                return OEBSDA.GetOracleEBSRoles();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<OracleEBSResponsibilitiesInfo> GetAllOracleEBSResponsibilitiesInfo()
        {
            try
            {
                return OEBSDA.GetOracleEBSResponsibilitiesInfo();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<OracleEBSUserRoleReport> GetOracleEBSUserToRoleReportData()
        {
            try
            {
                return OEBSDA.GetOracleEBSUserToRoleReportData();
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        public IList<OracleEBSUserRoleReport> GetOracleEBSRoletoUserReportData()
        {
            try
            {
                return OEBSDA.GetOracleEBSRoleToUserReportData();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<OracleEBSUserRoleReport> GetOracleEBSResponsibilitiesToUser()
        {
            try
            {
                return OEBSDA.GetOracleEBSResponsibilitiesToUser();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<OracleEBSUserInfo>OracleEBSUserToResponsibilities()
        {
            try
            {
                return OEBSDA.OracleEBSUserToResponsibilities();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<OracleEBSUserRoleReport> getoracleebsusertoresponsibilitiesreportdata()
        {
            try
            {
                return OEBSDA.GetOracleEBSUserToResponsibilitiesReportData();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /*
        public void SaveUserRole(IList<OracleEBSUserRole> userroles, int endpointSystemid)
        {
            try
            {
                OEBSDA.AddOracleEBSUserRole(userroles, endpointSystemid);
            }
            catch (Exception)
            {
                throw;
            }
        }
        */

        //public void SaveUserRoleMap(IList<OracleEBSUserInfo> _oebsUser, IList<OracleEBSRoleInfo> _roleInfo, DataSet oebsUserRole, int endpointSystemid)
        public void SaveUserRoleMap(IDictionary<string, long> _oebsUser, IDictionary<string, long> _roleInfo, DataSet oebsUserRole, long endpointSystemid)
        {
            try
            {
                OEBSDA.SaveUserRoleMap(_oebsUser, _roleInfo, oebsUserRole, endpointSystemid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveUserResponsibilitiesMap(IList<OracleEBSUserInfo> _oebsUser, IList<OracleEBSResponsibilitiesInfo> _responsibilitiesInfo, DataSet oebsUserResponsibilities, int endpointSystemid)
        {
            try
            {
                OEBSDA.SaveUserResponsibilitiesMap(_oebsUser, _responsibilitiesInfo, oebsUserResponsibilities, endpointSystemid);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

}
