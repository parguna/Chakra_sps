#region Namespace

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Model;

using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
#endregion

namespace DI.Chakra.DataAccess
{
    public class ORCLUMDA
    {
        private QueryExecutor m_executor = null;

        private IACMDbOperator m_iacmOperator = null;
        private IACMDBContext _context = null;

        #region Private Variables

        QueryExecutor executor = null;
        private IACMDbOperator iacmOperator = null;
        #endregion

        #region Constructor
        /// <summary>
        /// trusted da
        /// </summary>
        public ORCLUMDA()
        {
            _context = new IACMDBContext();
        }
        #endregion


        public class OracleUserRoleT : IAggregateRoot
        {
            public long OracleUMUserInfo_Id { get; set; }
            public long OracleUMRoleInfo_RoleId { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }


        #region Add to OracleUserManagement History Table
        /// <summary>
        /// To add to Trusted Source History Table
        /// </summary>
        /// <param name="users"></param>
        public void AddOracleUserManagementHistoryBulk(IList<OracleUMUserInfoHistory> users)
        {

            //SQLBulkUploader<OUMLoginInfo> sqlUserHistory = null;
            //try
            //{
            //    string tableName = "DI_M_OUM_LoginInfo";

            //    sqlUserHistory = new SQLBulkUploader<OUMLoginInfo>(tableName);

            //    sqlUserHistory.Flush(users);

            //}
            //catch (Exception)
            //{

            //    throw;
            //}
            //finally
            //{

            //}
        }
        #endregion

        #region Add to OracleUserManagement Live Table
        /// <summary>
        /// Add to Trusted Source Live Table
        /// </summary>
        /// <param name="users"></param>
        public void AddOracleUserManagementInfoBulk(IList<OracleUMUserInfo> users)
        {
            //SQLBulkUploader<OUMLoginInfo> sql = null;
            //try
            //{
            //    string tableName = "DI_M_OUM_LoginInfo";

            //    sql = new SQLBulkUploader<OUMLoginInfo>(tableName);
            //    sql.Flush(users);

            //}
            //catch (Exception)
            //{

            //    throw;
            //}
            //finally
            //{

            //}
        }
        #endregion

        public bool AddOracleUserManaagementConfiguration(OracleUMConfiguration _OracleUMConfiguration)
        {
            try
            {
                string query = "DI_spAddORAUMConfiguration";
                int noAffected;
                List<SqlParameter> parameters = new List<SqlParameter>();


                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Endpointsystemid";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = _OracleUMConfiguration.EndpointSystemId;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@HostName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OracleUMConfiguration.HostName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@IPAddress";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OracleUMConfiguration.IPAddress;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@DomainName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OracleUMConfiguration.DomainName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@UserName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OracleUMConfiguration.UserName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Password";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OracleUMConfiguration.Password;
                parameters.Add(sqlParam);



                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ServerName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OracleUMConfiguration.ServerName;
                parameters.Add(sqlParam);


                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@InstanceName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50; ;
                sqlParam.Value = _OracleUMConfiguration.InstanceName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@PortNo";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = _OracleUMConfiguration.PortNo;
                parameters.Add(sqlParam);


                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ImportUsers";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _OracleUMConfiguration.ImportUsers;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ImportRoles";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _OracleUMConfiguration.ImportRoles;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ImportTables";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _OracleUMConfiguration.ImportTables;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@MapAll";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _OracleUMConfiguration.MapAll;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@CreatedBy";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OracleUMConfiguration.CreatedBy;
                parameters.Add(sqlParam);


                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@LastModifiedBy";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OracleUMConfiguration.LastModifiedBy;
                parameters.Add(sqlParam);





                IACMDbOperator iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                executor = new QueryExecutor(iacmOperator);

                noAffected = executor.ExecuteQuery();

                if (noAffected <= 0)
                {
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Inserting the Oracle Um Configuration Details", ex);
                throw ex;
            }
            finally
            {
                executor.IACMDbOperator.Close();
            }
        }

        public OracleUMConfiguration GetConfiguration(long _systemid)
        {
            return _context.OracleUMConfiguration.FirstOrDefault(s => s.EndpointSystem.Id == _systemid);
        }

        //public IList<SQLUMDatabaseinfo> GetOracleUMDatabaseInfo()
        //{

        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            return oIACMDBContext.SQLUMDatabaseInfos.ToList();
        //        }
        //    }

        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //}

        public IList<EndpointObjects> GetORACLEObjects()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    EndPointSystemType ep = _context.EndpointSystemTypes.Where(e => Convert.ToString(e.EndpointSystemTypeName) == Constants.OracleUM).FirstOrDefault();
                    return _context.EndpointObjects.Where(e => e.EndpointSystemTypeId == ep.Id).ToList<EndpointObjects>();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All Oracle EndpointObjects ", ex);
                throw ex;
            }
        }

        public void UpdateOracleUserManaagementConfiguration(OracleUMConfiguration _OracleUMConfiguration)
        {

            try
            {
                string query = "DI_spUpdateORAUMConfiguration";
                int noAffected;
                List<SqlParameter> parameters = new List<SqlParameter>();


                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Endpointsystemid";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = _OracleUMConfiguration.EndpointSystemId;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@HostName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OracleUMConfiguration.HostName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@IPAddress";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OracleUMConfiguration.IPAddress;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ServerName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OracleUMConfiguration.ServerName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@DomainName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OracleUMConfiguration.DomainName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@UserName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OracleUMConfiguration.UserName;
                parameters.Add(sqlParam);



                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Password";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Value = _OracleUMConfiguration.Password;
                parameters.Add(sqlParam);


                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@InstanceName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OracleUMConfiguration.InstanceName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@PortNo";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = _OracleUMConfiguration.PortNo;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ImportUsers";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _OracleUMConfiguration.ImportUsers;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ImportRoles";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _OracleUMConfiguration.ImportRoles;
                parameters.Add(sqlParam);


                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ImportTables";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _OracleUMConfiguration.ImportTables;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@MapAll";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _OracleUMConfiguration.MapAll;
                parameters.Add(sqlParam);



                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@LastModifiedBy";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _OracleUMConfiguration.LastModifiedBy;
                parameters.Add(sqlParam);





                IACMDbOperator iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                executor = new QueryExecutor(iacmOperator);

                noAffected = executor.ExecuteQuery();



            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Inserting the ORA UM Configuration Details", ex);
                throw ex;
            }
            finally
            {
                executor.IACMDbOperator.Close();
            }
        }

        public IList<OracleUMUserInfo> GetOracleUserInfo(int DIUserId)
        {
            string query = "DI_spGetORAUserInfo";

            OracleUMUserInfo _orclUser = null;

            IList<OracleUMUserInfo> _list = new List<OracleUMUserInfo>();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                SqlParameter sqlParam = null;

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@UserId";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = DIUserId;
                parameters.Add(sqlParam);

                m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                m_executor = new QueryExecutor(m_iacmOperator);

                IDataReader reader = m_executor.ExecuteQueryDR();


                while (reader.Read())
                {
                    _orclUser = new OracleUMUserInfo();

                    _orclUser.Id = CheckNull.AgainstInt(reader["UserId"]);
                    _orclUser.TenantId = CheckNull.AgainstString(reader["TenantId"]);
                    _orclUser.EndPointSystemId = CheckNull.AgainstInt(reader["EndPointSystemId"]);
                    _orclUser.UserName = CheckNull.AgainstString(reader["UserName"]);
                    _orclUser.OraUserId = CheckNull.AgainstInt(reader["OraUserId"]);
                    _orclUser.AccountStatus = CheckNull.AgainstString(reader["AccountStatus"]);
                    _orclUser.CreatedAt = CheckNull.AgainstDatetime(reader["CreatedAt"]);
                    _orclUser.LockAt = CheckNull.AgainstDatetime(reader["LockAt"]);
                    _orclUser.ExpiryAt = CheckNull.AgainstDatetime(reader["ExpiryAt"]);
                    _orclUser.DefaultTableSpace = CheckNull.AgainstString(reader["DefaultTableSpace"]);
                    _orclUser.Profile = CheckNull.AgainstString(reader["Profile"]);
                    _orclUser.InitialRSRCConsumerGroup = CheckNull.AgainstString(reader["InitialRSRCConsumerGroup"]);
                    _orclUser.AuthenticationType = CheckNull.AgainstString(reader["AuthenticationType"]);
                    _orclUser.DIUserId = CheckNull.AgainstInt(reader["DIUserId"]);
                    _orclUser.SpecialAccount = CheckNull.AgainstBit(reader["SpecialAccount"]);
                    _orclUser.CreatedBy = CheckNull.AgainstString(reader["CreatedBy"]);
                    _orclUser.CreatedDate = CheckNull.AgainstDatetime(reader["CreatedDate"]);
                    _orclUser.LastModifiedBy = CheckNull.AgainstString(reader["LastModifiedBy"]);
                    _orclUser.LastModifiedDate = CheckNull.AgainstDatetime(reader["LastModifiedDate"]);
                    _list.Add(_orclUser);

                }
                return _list;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All ADUsers ", ex);
                throw ex;
            }
            finally
            {
                m_executor.IACMDbOperator.Close();
            }
        }


        public IList<UserEntitlement> GetEntitlementDetail(long EndpointUserId)
        {
            IList<UserEntitlement> Listtobereturn = new List<UserEntitlement>();
            OracleUMUserInfo OrclumUserInfo = new OracleUMUserInfo();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                OrclumUserInfo = oIACMDBContext.OracleUMUserInfo.Include(lq => lq.EndPointSystem).Include(lq => lq.EndPointSystem.EndPointSystemType).Include(lq => lq.OracleRoles).Where(Oracleumusr => Oracleumusr.Id == EndpointUserId).FirstOrDefault();
                if (OrclumUserInfo == null)
                    return Listtobereturn;

                foreach (OracleUMRoleInfo grp in OrclumUserInfo.OracleRoles)
                {
                    UserEntitlement UserEntitlement = new UserEntitlement();
                    UserEntitlement.EndpointUserId = OrclumUserInfo.Id;
                    UserEntitlement.UserName = OrclumUserInfo.AccountName;
                    UserEntitlement.EndPointSystemType = OrclumUserInfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = OrclumUserInfo.EndPointSystem.DisplayName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                    if (grp != null)
                    {
                        UserEntitlement.Name = grp.RoleName;
                        UserEntitlement.Description = grp.Description;
                        Listtobereturn.Add(UserEntitlement);
                    }
                }

            }
            return Listtobereturn;
        }







        public IList<string> GetOracleRoles(int DIUserId, int endPointSystemId)
        {
            string query = "DI_spGetORAUserRolesByDIUserId";
            string tmpValue;
            IList<string> roleList = new List<string>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            try
            {


                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@DIUserId";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = DIUserId;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@EndPointSystemId";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = endPointSystemId;
                parameters.Add(sqlParam);

                m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);
                m_executor = new QueryExecutor(m_iacmOperator);
                IDataReader reader = m_executor.ExecuteQueryDR();

                while (reader.Read())
                {
                    tmpValue = CheckNull.AgainstString(reader["RoleName"]);
                    if (!string.IsNullOrEmpty(tmpValue) && !roleList.Contains(tmpValue))
                    {
                        roleList.Add(tmpValue);
                    }

                }

                return roleList;
            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting ORAUser By DIUserId ", ex);
                throw ex;
            }
            finally
            {
                if (m_executor != null)
                    m_executor.IACMDbOperator.Close();
            }
        }

        public IList<OracleUMUserInfo> GetAllUsersInfo()
        {
            IList<OracleUMUserInfo> OracleUMUserInfo = new List<OracleUMUserInfo>();

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    OracleUMUserInfo = (from usrinfo in _context.OracleUMUserInfo select usrinfo).ToList();
                }

                return OracleUMUserInfo;
            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All Oracle User Information", ex);
                throw ex;
            }
        }


        public IList<UserAccount> GetAccountDetail(int UserID)
        {
            IList<UserAccount> Listtobereturn = new List<UserAccount>();
            IList<OracleUMUserInfo> listoracleumUserInfo = new List<OracleUMUserInfo>();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                listoracleumUserInfo = oIACMDBContext.OracleUMUserInfo.Include("EndPointSystem").Where(usr => usr.DIUserId == UserID && usr.IsLocked == false).ToList();
                foreach (OracleUMUserInfo user in listoracleumUserInfo)
                {
                    if (user != null)
                    {
                        UserAccount UserAccount = new UserAccount
                        {
                            EndpointUserId = user.Id,
                            UserName = user.AccountName,
                            AccountType = user.AccountType,
                            LastLogonDate = user.LastModifiedDate,
                            EndpointSystemId = user.EndPointSystemId,
                            EndPointSystemType = user.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = user.EndPointSystem.DisplayName
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
            OracleUMUserInfo OrclumUserInfo = new OracleUMUserInfo();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                OrclumUserInfo = oIACMDBContext.OracleUMUserInfo.Where(Orclumusr => Orclumusr.Id == EndpointUserId).FirstOrDefault();
                if (OrclumUserInfo == null)
                    return AccountDetail;
                AccountDetail.LoginName = OrclumUserInfo.AccountName;
                AccountDetail.AccountType = OrclumUserInfo.AccountType;
                AccountDetail.IsMappedAccount = OrclumUserInfo.DIUserId == null ? false : true;
                AccountDetail.LastLogonDate = OrclumUserInfo.LastModifiedDate;
                AccountDetail.Version = Convert.ToString(OrclumUserInfo.Version);
                AccountDetail.DIUserID = OrclumUserInfo.DIUserId;
                AccountDetail.EndpointSystemId = OrclumUserInfo.EndPointSystemId;
                AccountDetail.EndpointUserId = EndpointUserId;
            }
            return AccountDetail;
        }




        public IList<OracleUMUserInfo> GetUsers(int id)
        {
            IList<OracleUMUserInfo> OracleUMUserInfo = new List<OracleUMUserInfo>();

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    OracleUMUserInfo = (from usrinfo in _context.OracleUMUserInfo where usrinfo.DIUserId == id && usrinfo.IsLocked == false select usrinfo).ToList();
                }

                return OracleUMUserInfo;
            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All Oracle User Information", ex);
                throw ex;
            }
        }

        public void AddOracleRoleInfo(OracleUMRoleInfo RoleInfo)
        {
            using (_context = new IACMDBContext())
            {
                _context.OracleUMRoleInfo.Add(RoleInfo);
                _context.SaveChanges();
            }
        }

        public void AddOracleTableInfo(OracleUMTableInfo TableInfo)
        {
            using (_context = new IACMDBContext())
            {
                _context.OracleUMTableInfo.Add(TableInfo);
                _context.SaveChanges();
            }
        }

        public void AddOracleUserInfo(OracleUMUserInfo UserInfo)
        {
            using (_context = new IACMDBContext())
            {
                _context.OracleUMUserInfo.Add(UserInfo);
                _context.SaveChanges();
            }
        }

        public void ChangeOracleUserInfo(OracleUMUserInfo UserInfo)
        {
            using (_context = new IACMDBContext())
            {
                _context.OracleUMUserInfo.Attach(UserInfo);
                _context.Entry(UserInfo).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public IList<OracleUMUserInfo> GetOracleUMLoginInfoByDIUserId(int DIUserId)
        {
            //string query = "DI_spGetSQLUMLoginInfoByDIUserId";
            IList<OracleUMUserInfo> list = new List<OracleUMUserInfo>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                    list = _context.OracleUMUserInfo
                        .Include("EndPointSystem")
                        .Include("EndPointSystem.EndPointSystemType")
                        .Where(oracleUI => oracleUI.DIUserId == DIUserId && oracleUI.IsLocked == false)
                        .ToList();
                return list;
            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while getting SQLUM Login Info By DIUserId", ex);
                throw;
            }
        }

        public IList<OracleUMUserInfo> GetOracleUMUserRoleByUserId(int UserId)
        {
            //string query = "DI_spGetSQLUMLoginInfoByDIUserId";
            IList<OracleUMUserInfo> list = new List<OracleUMUserInfo>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                    list = _context.OracleUMUserInfo
                         .Include("OracleUMUserRole")
                       .Where(oracleUI => oracleUI.Id == UserId)
                        .ToList();
                return list;
            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while getting SQLUM Login Info By DIUserId", ex);
                throw;
            }
        }





        public OracleUMUserInfo GetOracleUserByDIUserId(long diUserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleUMUserInfo.FirstOrDefault(item => item.DIUserId == diUserId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<OracleUMUserInfo> GetOracleUserInfoByDIUserId(int diUserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleUMUserInfo.Where(item => item.DIUserId == diUserId).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Provisioning Methods

        public bool UpdateOracleUserInfoFromProvisioning(OracleUMUserInfo oraUserInfo)
        {
            bool isDataSaved = false;
            if (oraUserInfo == null)
                return isDataSaved;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var reviewEPEntry = _context.Entry<OracleUMUserInfo>(oraUserInfo);
                    if (reviewEPEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<OracleUMUserInfo>();
                        OracleUMUserInfo attachedreviewEP = set.Find(oraUserInfo.Id);

                        if (attachedreviewEP != null)
                        {
                            var attachedEntry = _context.Entry(attachedreviewEP);
                            attachedEntry.CurrentValues.SetValues(oraUserInfo);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while updating  OracleUMUserInfo From Provisioning.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        #endregion

        public OracleUMUserInfo GetLockedOracleUserByDIUserId(long DIUserId, long endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleUMUserInfo.FirstOrDefault(item => item.DIUserId == DIUserId && item.EndPointSystemId == endpointSystemId
                            && item.IsUserAccount == true && item.IsLocked);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<OracleUMUserInfo> GetPrimaryOracleUserByDIUserId(long DIUserId, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleUMUserInfo.Where(item => item.DIUserId == DIUserId && item.IsUserAccount == true
                                && item.EndPointSystemId == endpointSystemId && !item.IsLocked).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Error Occured while getting All OracleUser DIUserId={0}", DIUserId.ToString()), ex);
                throw ex;
            }
        }

        public void AddOracleUserInfoBulk(IList<OracleUMUserInfo> lotusUserInfo)
        {
            using (_context = new IACMDBContext())
            {
                _context.Database.ExecuteSqlCommand("Truncate table DI_ORA_UserInfo");
            }

            SQLBulkUploader<OracleUMUserInfo> oraUser = null;
            try
            {
                string tablename = "DI_ORA_UserInfo";
                oraUser = new SQLBulkUploader<OracleUMUserInfo>(tablename);
                oraUser.Flush(lotusUserInfo);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                oraUser.Close();
            }
        }

        public long AddOracleUser(OracleUMUserInfo oraUserInfo)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    _context.OracleUMUserInfo.Add(oraUserInfo);
                    _context.SaveChanges();
                    return oraUserInfo.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<OracleUMRoleInfo> GetOracleRoleByUserId(long userId, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleUMUserInfo.FirstOrDefault(item => item.DIUserId == userId && item.EndPointSystemId == endpointSystemId).OracleRoles.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CheckOracleUserNameExists(string userName, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleUMUserInfo.Any(item => item.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)
                        && item.EndPointSystemId == endpointSystemId);
                }
            }
            catch
            {
                throw;
            }
        }

        public void DelUserRole()
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                //_context.Database.ExecuteSqlCommand(string.Format("DELETE FROM OracleUMUserInfoOracleUMRoleInfo WHERE EndpointSystemId = {0}", endpointSystemid));
                _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM OracleUMUserInfoOracleUMRoleInfo"));
            }
        }

        public void AddOracleUserRole(IList<OracleUMUserRole> userroles, int endpointSystemid)
        {


            IList<OracleUserRoleT> listUR = new List<OracleUserRoleT>();

            OracleUserRoleT userRole = null;

            foreach (var group in userroles)
            {

                userRole = new OracleUserRoleT();

                userRole.OracleUMUserInfo_Id = group.UserId;
                userRole.OracleUMRoleInfo_RoleId = group.RoleId;
                userRole.EndpointSystemId = endpointSystemid;
                userRole.Version = Convert.ToInt16(VersionNo.Available);

                if (userRole.OracleUMUserInfo_Id != 0)
                {
                    listUR.Add(userRole);
                }
            }

            SQLBulkUploader<OracleUserRoleT> sqlUserRoleMap = null;
            try
            {
                string tableName = "OracleUMUserInfoOracleUMRoleInfo";
                sqlUserRoleMap = new SQLBulkUploader<OracleUserRoleT>(tableName);
                //sqlUserRoleMap.Flush(userroles);
                sqlUserRoleMap.Flush(listUR);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlUserRoleMap.Close();
            }
        }

        public IList<OracleUMRoleInfo> GetOracleUMRoles()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleUMRoleInfo.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<OracleUMUserRoleReport> GetOracleUMUserToRoleReportData()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from Users in _context.OracleUMUserInfo
                            from Roles in Users.OracleRoles
                            select new OracleUMUserRoleReport
                            {
                                UserName = Users.UserName,
                                RoleName = Roles.RoleName,
                                CreatedAt = Users.CreatedAt

                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<OracleUMUserRoleReport> GetOracleUMRoleToUserReportData()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    //return _context.GEMSRoleInfos.Include("GEMSUserInfos").ToList();

                    return (from Roles in _context.OracleUMRoleInfo
                            from Users in Roles.OracleUsers
                            select new OracleUMUserRoleReport
                            {
                                UserName = Users.UserName,
                                RoleName = Roles.RoleName,
                                CreatedAt = Users.CreatedAt

                            }).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IList<UserAccount> GetUserAccountDetailByApplication(int ApplicationID)
        {
            IList<UserAccount> Listtobereturn = new List<UserAccount>();
            IList<OracleUMUserInfo> listOracleUMUserInfo = new List<OracleUMUserInfo>();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                listOracleUMUserInfo = oIACMDBContext.OracleUMUserInfo.Include(lq => lq.EndPointSystem).Include(lq => lq.EndPointSystem.EndPointSystemType).Where(Orclumusr => Orclumusr.EndPointSystemId == ApplicationID && Orclumusr.IsLocked == false).ToList();
                foreach (OracleUMUserInfo Orclumusr in listOracleUMUserInfo)
                {
                    if (Orclumusr != null)
                    {
                        UserAccount UserAccount = new UserAccount
                        {
                            EndpointUserId = Orclumusr.Id,
                            UserName = Orclumusr.AccountName,
                            FullName = Orclumusr.UserName,
                            AccountType = Orclumusr.AccountType,
                            LastLogonDate = Orclumusr.LastModifiedDate,
                            EndpointSystemId = Orclumusr.EndPointSystemId,
                            EndPointSystemType = Orclumusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = Orclumusr.EndPointSystem.DisplayName
                        };
                        Listtobereturn.Add(UserAccount);
                    }
                }
            }
            return Listtobereturn;
        }


        public void UpdateAccountDetail(AccountDetail AccountDetail)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                OracleUMUserInfo ORCLUMInfo = Context.OracleUMUserInfo.FirstOrDefault(ENDPT => ENDPT.Id == AccountDetail.EndpointUserId && ENDPT.EndPointSystemId == AccountDetail.EndpointSystemId);
                if (ORCLUMInfo != null)
                {
                    ORCLUMInfo.DIUserId = AccountDetail.DIUserID; ;
                    ORCLUMInfo.LastModifiedDate = DateTime.Today;
                    ORCLUMInfo.MappingType = "Manual";
                    ORCLUMInfo.AccountType = AccountDetail.AccountType;

                    Context.SaveChanges();
                }
            }
        }


        //AccessRequest


        public List<AccessRequestItem> GetAccessRequestItem(string searchString)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                return (from roles in Context.OracleUMRoleInfo
                        where roles.RoleName.Contains(searchString)
                        orderby roles.RoleName
                        select new AccessRequestItem
                        {
                            AccessRequestItemType = AccessRequestItemType.Entitlement,
                            EntitlementType = Infrastructure.Enums.EntitlementType.Role,
                            EndpointType = EndpointType.ORCLUM,
                            KeyId = roles.Id,
                            EndpointSystemId = (int)EndpointType.ORCLUM,
                            EndpointSystemTypeId = (int)EndpointType.ORCLUM,
                            ItemName = roles.RoleName,
                            ItemDescription=roles.Description,
                            ItemDetail=roles.Description
                        }).ToList();
            }
        }

        public OracleUMUserInfo GetOracleUserByUserId(long userId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleUMUserInfo.FirstOrDefault(item => item.Id == userId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}