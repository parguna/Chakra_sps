#region Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Model;
using DeepIdentity.Infrastructure;
using System.ComponentModel;
using System.Reflection;
using System.Collections;

using DeepIdentity.DataAccess;
#endregion

namespace DeepIdentity.Facade
{
    public class ORCLUMFacade
    {

        #region Variable Declaration
        private bool m_populate = false;


        private ORCLUMDA OracleDA = null;
        #endregion

        #region Constructor
        public ORCLUMFacade()
        {
            OracleDA = new ORCLUMDA();

        }
        #endregion
        
        public IList<EndpointObjects> GetORACLEObjects()
        {
            try
            {
                return OracleDA.GetORACLEObjects();
            }
            catch (Exception)
            {
                throw;
            }
        }

       


        public IList<OracleUMUserInfo> GetAllUsers()
        {
            try
            {
                return OracleDA.GetAllUsersInfo();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<OracleUMUserInfo> GetUsers(int id)
        {
            try
            {
                return OracleDA.GetUsers(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AddOracleRoleInfo(OracleUMRoleInfo RoleInfo)
        {
            try
            {
                OracleDA.AddOracleRoleInfo(RoleInfo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AddOracleTableInfo(OracleUMTableInfo TableInfo)
        {
            try
            {
                OracleDA.AddOracleTableInfo(TableInfo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public OracleUMConfiguration GetConfiguration(int _systemid)
        {
            return OracleDA.GetConfiguration(_systemid);
        }

        public void AddOracleUserInfo(OracleUMUserInfo UserInfo, bool IsNewlyAdded)
        {
            try
            {
                OracleDA.AddOracleUserInfo(UserInfo);
                new ReconcilationFacade().AddOrUpdateAccountInReconTable(UserInfo, IsNewlyAdded);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ChangeOracleUserInfo(OracleUMUserInfo UserInfo)
        {
            try
            {
                OracleDA.ChangeOracleUserInfo(UserInfo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public OracleUMUserInfo GetOracle11gUserByUserId(int userId)
        //{
        //    return OracleDA.GetOracle11gUserByUserId(userId);
        //}

        public OracleUMUserInfo GetOracleUserByDIUserId(int diUserId)
        {
            return OracleDA.GetOracleUserByDIUserId(diUserId);
        }

        public OracleUMUserInfo GetLockedOracleUserByDIUserId(int DIUserId, int endpointSystemId)
        {
            return OracleDA.GetLockedOracleUserByDIUserId(DIUserId, endpointSystemId);
        }

        public List<OracleUMUserInfo> GetPrimaryOracleUserByDIUserId(int DIUserId, int endpointSystemId)
        {
            return OracleDA.GetPrimaryOracleUserByDIUserId(DIUserId, endpointSystemId);
        }

        public int AddOracleUser(OracleUMUserInfo _oraUser)
        {
            try
            {
                return OracleDA.AddOracleUser(_oraUser);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<OracleUMRoleInfo> GetOracleRolesByUserId(int userId, int endpointSystemId)
        {
            return OracleDA.GetOracleRoleByUserId(userId, endpointSystemId);
        }

        public bool CheckOracleUserNameExists(string userName, int endpointSystemId)
        {
            try
            {
                return OracleDA.CheckOracleUserNameExists(userName, endpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        public IList<OracleUMRoleInfo> GetAllOracleUMRoles()
        {
            try
            {
                return OracleDA.GetOracleUMRoles();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<OracleUMUserRoleReport> GetOracleUMUserToRoleReportData()
        {
            try
            {
                return OracleDA.GetOracleUMUserToRoleReportData();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public IList<OracleUMUserRoleReport> GetOracleUMRoletoUserReportData()
        {
            try
            {
                return OracleDA.GetOracleUMRoleToUserReportData();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveUserRole(IList<OracleUMUserRole> userroles, int endpointSystemid)
        {
            try
            {
                OracleDA.AddOracleUserRole(userroles, endpointSystemid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public OracleUMUserInfo GetOracleUserByUserId(int userId)
        {
            return OracleDA.GetOracleUserByUserId(userId);
        }
    }
}