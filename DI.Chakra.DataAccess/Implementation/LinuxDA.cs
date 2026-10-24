using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using System.Data;
using System.Data.SqlClient;

using DI.Chakra.Infrastructure;
using System.Data.Entity;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;


namespace DI.Chakra.DataAccess
{
    public class LinuxDA
    {
        QueryExecutor m_executor;

        IACMDbOperator m_iacmOperator;

        private IACMDBContext _context;

        public LinuxDA()
        {
            _context = new IACMDBContext();
        }

        public IList<EndpointObjects> GetLinuxObjects()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    EndPointSystemType ep = _context.EndpointSystemTypes.Where(dhf => dhf.EndpointSystemTypeName == EndpointType.Linux).FirstOrDefault();
                    return _context.EndpointObjects.Where(e => e.EndpointSystemTypeId == ep.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while getting All Solaris / Linux EndpointObjects ", ex);
                throw ex;
            }
        }

        public LinuxConfiguration GetConfiguration(long _systemid)
        {
            LinuxConfiguration config = null;
            config = _context.linuxConfigurations.Include("EndpointSystem").FirstOrDefault(s => s.EndpointSystem.Id == _systemid);
            config.Password = RSAHelper.DecryptData(config.Password);
            return config;
        }

        #region Linux Configuration
        public bool AddLinuxConfiguration(LinuxConfiguration _LinuxConfiguration)
        {
            try
            {
                string query = "DI_spAddLinuxConfiguration";
                int noAffected;
                List<SqlParameter> parameters = new List<SqlParameter>();


                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Endpointsystemid";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = _LinuxConfiguration.EndpointSystem.Id;
                parameters.Add(sqlParam);



                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@IPAddress";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _LinuxConfiguration.IPAddress;
                parameters.Add(sqlParam);






                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@UserName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _LinuxConfiguration.UserName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Password";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _LinuxConfiguration.Password;
                parameters.Add(sqlParam);


                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@MapAll";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _LinuxConfiguration.MapAll.Value;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@CreatedBy";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _LinuxConfiguration.CreatedBy;
                parameters.Add(sqlParam);


                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@LastModifiedBy";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _LinuxConfiguration.LastModifiedBy;
                parameters.Add(sqlParam);

                m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                m_executor = new QueryExecutor(m_iacmOperator);

                noAffected = m_executor.ExecuteQuery();

                if (noAffected <= 0)
                {
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while Inserting the Linux Configuration Details", ex);
                throw ex;
            }
            finally
            {
                m_executor.IACMDbOperator.Close();
            }
        }

        /// <summary>
        /// Update LinuxConfiguration Details
        /// </summary>
        /// <param name="_LinuxConfiguration"></param>
        public void UpdateLinuxConfiguration(LinuxConfiguration _LinuxConfiguration)
        {
            try
            {
                string query = "DI_spUpdateLinuxConfiguration";
                int recordsAffected;
                List<SqlParameter> parameters = new List<SqlParameter>();


                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Endpointsystemid";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = _LinuxConfiguration.EndpointSystem.Id;
                parameters.Add(sqlParam);



                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@IPAddress";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _LinuxConfiguration.IPAddress;
                parameters.Add(sqlParam);


                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@UserName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _LinuxConfiguration.UserName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Password";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _LinuxConfiguration.Password;
                parameters.Add(sqlParam);




                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@MapAll";
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = _LinuxConfiguration.MapAll;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@LastModifiedBy";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = _LinuxConfiguration.LastModifiedBy;
                parameters.Add(sqlParam);



                m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                m_executor = new QueryExecutor(m_iacmOperator);

                recordsAffected = m_executor.ExecuteQuery();
                if (recordsAffected <= 0)
                {

                }
                else
                {
                    throw new Exception("Linux Configuration Details are not updated");
                }


            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while Updating the Linux Configuration Details", ex);
                throw ex;
            }
            finally
            {
                m_executor.IACMDbOperator.Close();
            }
        }

        #endregion

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


        public void AddLinuxUserInfoHistoryBulk(IList<LinuxUserInfoHistory> users)
        {

            SQLBulkUploader<LinuxUserInfoHistory> sqlUserHistory = null;
            try
            {
                string tableName = "DI_Linux_UserInfoHistory";

                sqlUserHistory = new SQLBulkUploader<LinuxUserInfoHistory>(tableName);

                sqlUserHistory.Flush(users);



            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }
        }

        public LinuxUserInfo GetLinuxUserByUserId(long userId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LinuxUserInfos.FirstOrDefault(item => item.Id == userId && !item.IsLocked);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }








        public void AddLinuxUserInfoBulk(IList<LinuxUserInfo> users, EndPoint endpoint)
        {



            //SQLBulkUploader<LinuxUserInfo> sqlUserHistory = null;
            try
            {
                using (IACMDBContext context = new IACMDBContext())
                {
                    List<LinuxUserInfo> PreviousDiscoveryUsers = _context.LinuxUserInfos.Where(c => c.EndPointSystemId == endpoint.Id && c.IsLocked == false).ToList();
                    List<LinuxUserInfo> existingUsers = getExistingUsers(PreviousDiscoveryUsers, users);
                    List<LinuxUserInfo> DeleteUsers = getDeletedUsers(PreviousDiscoveryUsers, users);
                    List<LinuxUserInfo> NewUsers = getNewUsers(PreviousDiscoveryUsers, users);

                    new EndPointDA().DiscoveryChanges(existingUsers, NewUsers, DeleteUsers);

                    context.Database.ExecuteSqlCommand(string.Format("update DI_Linux_UserInfo set LastModifiedDate={0} where EndPointSystemId={1}", "GetDate()", endpoint.Id));

                    context.SaveChanges();
                }
                //string tableName = "DI_Linux_UserInfo";

                //sqlUserHistory = new SQLBulkUploader<LinuxUserInfo>(tableName);

                //sqlUserHistory.Flush(users);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }
        }


        public IList<UserEntitlement> GetEntitlementDetail(long EndpointUserId)
        {
            IList<UserEntitlement> Listtobereturn = new List<UserEntitlement>();
            LinuxUserInfo linuxUserInfo = new LinuxUserInfo();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                linuxUserInfo = oIACMDBContext.LinuxUserInfos.Include(lq => lq.EndPointSystem).Include(lq => lq.EndPointSystem.EndPointSystemType).Include(lq => lq.LinuxGroup).Where(linuxusr => linuxusr.Id == EndpointUserId).FirstOrDefault();
                if (linuxUserInfo == null)
                    return Listtobereturn;

                foreach (LinuxGroupInfo grp in linuxUserInfo.LinuxGroup)
                {
                    UserEntitlement UserEntitlement = new UserEntitlement();
                    UserEntitlement.EndpointUserId = linuxUserInfo.Id;
                    UserEntitlement.UserName = linuxUserInfo.AccountName;
                    UserEntitlement.EndPointSystemType = linuxUserInfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = linuxUserInfo.EndPointSystem.DisplayName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                    if (grp != null)
                    {
                        UserEntitlement.Name = grp.GroupName;
                        UserEntitlement.Description = grp.GroupName;
                        Listtobereturn.Add(UserEntitlement);
                    }
                }

            }
            return Listtobereturn;
        }



        public IList<UserAccount> GetUserAccountDetailByApplication(int ApplicationID)
        {
            IList<UserAccount> Listtobereturn = new List<UserAccount>();
            IList<LinuxUserInfo> listLinuxUserInfo = new List<LinuxUserInfo>();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                listLinuxUserInfo = oIACMDBContext.LinuxUserInfos.Include(lq => lq.EndPointSystem).Include(lq => lq.EndPointSystem.EndPointSystemType).Where(linuxusr => linuxusr.EndPointSystemId == ApplicationID && linuxusr.IsLocked == false).ToList();
                foreach (LinuxUserInfo linuxusr in listLinuxUserInfo)
                {
                    if (linuxusr != null)
                    {
                        UserAccount UserAccount = new UserAccount
                        {
                            EndpointUserId = linuxusr.Id,
                            UserName = linuxusr.AccountName,
                            FullName = linuxusr.UserName,
                            AccountType = linuxusr.AccountType,
                            LastLogonDate = linuxusr.LastModifiedDate,
                            EndpointSystemId = linuxusr.EndPointSystemId,
                            EndPointSystemType = linuxusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = linuxusr.EndPointSystem.DisplayName
                        };
                        Listtobereturn.Add(UserAccount);
                    }
                }
            }
            return Listtobereturn;
        }


        private List<LinuxUserInfo> getNewUsers(IList<LinuxUserInfo> PreviousDiscoveryUsers, IList<LinuxUserInfo> Discoveredusers)
        {
            try
            {
                return Discoveredusers.Where(c => !(PreviousDiscoveryUsers.Any(m => m.UserName == c.UserName))).ToList();
            }
            catch (Exception)
            { return null; }
        }

        private List<LinuxUserInfo> getDeletedUsers(IList<LinuxUserInfo> PreviousDiscoveryUsers, IList<LinuxUserInfo> Discoveredusers)
        {
            try
            {

                return PreviousDiscoveryUsers.Where(c => !(Discoveredusers.Any(m => c.UserName == m.UserName))).ToList();
            }
            catch (Exception)
            { return null; }
        }

        private List<LinuxUserInfo> getExistingUsers(IList<LinuxUserInfo> PreviousDiscoveryUsers, IList<LinuxUserInfo> Discoveredusers)
        {
            try
            {
                return PreviousDiscoveryUsers.Where(c => Discoveredusers.Any(m => c.UserName == m.UserName)).ToList();
            }
            catch (Exception)
            { return null; }
        }

        private List<LinuxGroupInfo> getNewGroups(IList<LinuxGroupInfo> PreviousDiscoveryGroups, IList<LinuxGroupInfo> DiscoveredGroups)
        {
            try
            {
                return DiscoveredGroups.Where(c => !PreviousDiscoveryGroups.Any(m => c.GroupName == m.GroupName)).ToList();
            }
            catch (Exception)
            { return null; }
        }

        private List<LinuxGroupInfo> getDeletedGroups(IList<LinuxGroupInfo> PreviousDiscoveryGroups, IList<LinuxGroupInfo> DiscoveredGroups)
        {
            try
            {
                return PreviousDiscoveryGroups.Where(c => !(DiscoveredGroups.Any(m => c.GroupName == m.GroupName))).ToList();
            }
            catch (Exception)
            { return null; }
        }

        private List<LinuxGroupInfo> getExistingGroups(IList<LinuxGroupInfo> PreviousDiscoveryGroups, IList<LinuxGroupInfo> DiscoveredGroups)
        {
            try
            {
                return DiscoveredGroups.Where(c => PreviousDiscoveryGroups.Any(m => c.GroupName == m.GroupName)).ToList();
            }
            catch (Exception)
            { return null; }
        }

        public void AddLinuxResourceInfoHistoryBulk(IList<LinuxResourceInfoHistory> users)
        {

            SQLBulkUploader<LinuxResourceInfoHistory> sqlUserHistory = null;
            try
            {
                string tableName = "DI_Linux_ResourceInfoHistory";

                sqlUserHistory = new SQLBulkUploader<LinuxResourceInfoHistory>(tableName);

                sqlUserHistory.Flush(users);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }
        }

        public void AddLinuxUserGroupBulk(IList<LinuxUserGroup> userGroup)
        {

            SQLBulkUploader<LinuxUserGroup> sqlUserHistory = null;
            try
            {
                string tableName = "DI_Linux_UserGroupMap";

                sqlUserHistory = new SQLBulkUploader<LinuxUserGroup>(tableName);

                sqlUserHistory.Flush(userGroup);



            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }
        }


        public void AddLinuxUserGroupBulk(LinuxUserInfo linuxUserValue)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    LinuxUserInfo currentLINUXUser = oIACMDBContext.LinuxUserInfos.FirstOrDefault(item => item.Id == linuxUserValue.Id);

                    foreach (string grpStr in linuxUserValue.Groups)
                    {
                        IList<LinuxGroupInfo> grpList = oIACMDBContext.LinuxGroupInfos.Where(gi => gi.GroupName == grpStr && gi.EndPointSystemId == currentLINUXUser.EndPointSystemId).ToList();
                        foreach (LinuxGroupInfo grpItem in grpList)
                        {
                            LinuxGroupInfo tmpgrpItem = currentLINUXUser.LinuxGroup.Where(g => g.GroupName == grpItem.GroupName).FirstOrDefault();
                            if (tmpgrpItem == null)
                            {
                                currentLINUXUser.LinuxGroup.Add(grpItem);
                            }
                        }
                        oIACMDBContext.SaveChanges();
                    }

                    string query = "UPDATE LinuxUserInfoLinuxGroupInfo SET EndpointSystemId = " + currentLINUXUser.EndPointSystemId + " WHERE LinuxUserInfo_Id = " + currentLINUXUser.Id;

                    oIACMDBContext.Database.ExecuteSqlCommand(query);

                    oIACMDBContext.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

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

            int newRowCount = -1;

            try
            {
                m_iacmOperator = new IACMDbOperator();

                conn = m_iacmOperator.GetConnection();

                selectCmd = conn.CreateCommand();
                selectCmd.CommandText = string.Format("SELECT UserId, UserName, DIUserId FROM DI_Linux_UserInfo where TenantId='{0}' and EndPointSystemId={1}", EndPoint.TenantId, EndPoint.Id);
                //selectCmd.CommandText = "SELECT UserId, DistinguishedName, DIUserId FROM DI_Linux_UserInfo";
                updateCmd = conn.CreateCommand();
                updateCmd.CommandText = string.Format("UPDATE DI_Linux_UserInfo Set DIUserId=@DIUserId Where UserId=@UserId and TenantId='{0}' and EndPointSystemId={1}", EndPoint.TenantId, EndPoint.Id);
                //updateCmd.CommandText = "UPDATE DI_Linux_UserInfo Set DIUserId=@DIUserId Where UserId=@UserId";
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
                da.Fill(ds, "LinuxUserInfo");


                DataRow[] drowsMax = ds.Tables[0].Select("UserId = max(UserId)");

                if (drowsMax.Count() <= 0)
                    newRowCount = 0;
                else
                    newRowCount = Convert.ToInt32(drowsMax[0]["UserId"]);

                //DataTable dtNew = dtAdsUser;

                if (ds.Tables[0].Rows.Count <= 0)
                {

                    ds.Tables[0].Merge(dtAdsUser, false);
                    da.Update(ds, "LinuxUserInfo");
                    return;
                }

                DataRow drow = null;

                foreach (DataRow dr in dtAdsUser.Rows)
                {
                    DataRow[] dRows = ds.Tables[0].Select("UserName='" + dr["UserName"].ToString().Replace("'", "''") + "'");




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




                if (strUpdate.ToString() != string.Empty)
                    DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Updated userId(s):" + strUpdate.ToString().Split(',').Count());


                try
                {
                    da.Update(ds.Tables[0].Select(null, null, DataViewRowState.ModifiedCurrent));

                }
                catch (Exception ex)
                {
                   DILog.GeneralLogger.WriteLog(LogLevelType.Error,"UserInfo update Error", ex);

                }


            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"UserInfo Error", ex);
                throw;
            }
            finally
            {
                m_iacmOperator.Close();

                selectCmd.Dispose();

                updateCmd.Dispose();


            }
        }


        public void AddLinuxGroupInfoBulk(IList<LinuxGroupInfo> groups, EndPoint endpoint)
        {

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    IList<LinuxGroupInfo> PreviousDiscoveryGroups = _context.LinuxGroupInfos.Where(c => c.EndPointSystemId == endpoint.Id).ToList();
                    IList<LinuxGroupInfo> existingGroups = getExistingGroups(PreviousDiscoveryGroups, groups);
                    IList<LinuxGroupInfo> DeleteGroups = getDeletedGroups(PreviousDiscoveryGroups, groups);
                    IList<LinuxGroupInfo> NewGroups = getNewGroups(PreviousDiscoveryGroups, groups);

                    if (DeleteGroups != null)
                        if (DeleteGroups != null)
                        {
                            foreach (var items in DeleteGroups)
                            {
                                _context.LinuxGroupInfos.RemoveRange(_context.LinuxGroupInfos.Where(c => c.Id == items.Id));
                            }
                        }

                    if (NewGroups != null)
                        _context.LinuxGroupInfos.AddRange(NewGroups);

                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // sqlBulk.Close();
            }
        }
        public void AddLinuxResourceInfoBulk(IList<LinuxResourceInfo> resources)
        {
            SQLBulkUploader<LinuxResourceInfo> sqlBulk = null;
            try
            {
                string tableName = "DI_Linux_ResourceInfo";

                sqlBulk = new SQLBulkUploader<LinuxResourceInfo>(tableName);

                sqlBulk.Flush(resources);
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
        public void AddLinuxGroupInfoHistoryBulk(IList<LinuxGroupInfoHistory> groups, EndPoint endpoint)
        {
            SQLBulkUploader<LinuxGroupInfoHistory> sql = null;
            try
            {
                string tableName = "DI_Linux_GroupInfoHistory";

                sql = new SQLBulkUploader<LinuxGroupInfoHistory>(tableName);

                sql.Flush(groups);


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }
        }


        //public void AddLinuxUserGroupBulk(IList<LinuxUserGroup> userGroup)
        //{

        //    SQLBulkUploader<LinuxUserGroup> sqlUserHistory = null;
        //    try
        //    {
        //        string tableName = "DI_Linux_UserGroup";

        //        sqlUserHistory = new SQLBulkUploader<LinuxUserGroup>(tableName);

        //        sqlUserHistory.Flush(userGroup);



        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    finally
        //    {

        //    }
        //}

        public IList<LinuxUserInfo> GetAllUnMappedLinuxUsers(EndPoint EndPoint)
        {
            IList<LinuxUserInfo> listAds = new List<LinuxUserInfo>();

            string query = "DI_spGetAllUnMappedLinuxUserInfo";

            LinuxUserInfo adsUserInfo = null;
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                SqlParameter sqlParam = null;

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@TenantId";
                sqlParam.SqlDbType = SqlDbType.VarChar;
                sqlParam.Value = EndPoint.TenantId;
                parameters.Add(sqlParam);
                m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                m_executor = new QueryExecutor(m_iacmOperator);

                IDataReader reader = m_executor.ExecuteQueryDR();


                while (reader.Read())
                {
                    adsUserInfo = LinuxUserInfo.CreateLinuxUserInfo();

                    adsUserInfo.Id = 1;
                    adsUserInfo.TenantId = CheckNull.AgainstString(reader["TenantId"]);
                    adsUserInfo.EndPointSystemId = CheckNull.AgainstInt(reader["EndpointSystemId"]);
                    adsUserInfo.UserName = CheckNull.AgainstString(reader["UserName"]);
                    adsUserInfo.Password = CheckNull.AgainstString(reader["Password"]);
                    adsUserInfo.LinuxUserId = CheckNull.AgainstString(reader["LinuxUserId"]);
                    adsUserInfo.GroupId = CheckNull.AgainstString(reader["GroupId"]);
                    adsUserInfo.UserIdInfo = CheckNull.AgainstString(reader["UserIdInfo"]);
                    adsUserInfo.HomeDirectory = CheckNull.AgainstString(reader["HomeDirectory"]);
                    adsUserInfo.CommandShell = CheckNull.AgainstString(reader["CommandShell"]);
                    adsUserInfo.DIUserId = CheckNull.AgainstInt(reader["DIUserId"]);
                    adsUserInfo.CreatedBy = CheckNull.AgainstString(reader["CreatedBy"]);
                    adsUserInfo.CreatedDate = CheckNull.AgainstDatetime(reader["CreatedDate"]);
                    adsUserInfo.LastModifiedBy = CheckNull.AgainstString(reader["LastModifiedBy"]);
                    adsUserInfo.LastModifiedDate = CheckNull.AgainstDatetime(reader["LastModifiedDate"]);

                    listAds.Add(adsUserInfo);
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                m_executor.IACMDbOperator.Close();
            }
            return listAds;
        }


        public IList<LinuxUserInfo> GetLinuxUserInfoByDIUserId(long DIUserId, int EndpointId = 0)
        {
            //string query = "DI_spGetLinuxUserInfo";
            IList<LinuxUserInfo> listLinux = new List<LinuxUserInfo>();

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from user in _context.LinuxUserInfos
                                 .Include("EndPointSystem")
                                 .Include("EndPointSystem.EndPointSystemType")
                                 .Include("LinuxGroup")
                            where user.DIUserId == DIUserId && user.IsLocked == false
                            && (user.EndPointSystemId == EndpointId || EndpointId == 0)
                            orderby user.CreatedDate
                            select user).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return listLinux;
        }



        public IList<AttributeMappingColumns> GetAttributeMappingColumns(long EndPointSystemId)
        {
            try
            {
                return new EndPointDA().GetAttributeMappingColumns(EndPointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //  m_executor.IACMDbOperator.Close();
            }

        }

        #region UserListing methods
        public IList<LinuxUserInfo> GetLinuxUsersByDIUserID(int DIUserID)
        {
            string query = "DI_spGetLinuxUsersByDIUserId";
            LinuxUserInfo _linuxUser = null;

            IList<LinuxUserInfo> _list = new List<LinuxUserInfo>();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                SqlParameter sqlParam = null;

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@DIUserID";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = DIUserID;
                parameters.Add(sqlParam);

                m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);
                m_executor = new QueryExecutor(m_iacmOperator);
                IDataReader reader = m_executor.ExecuteQueryDR();

                while (reader.Read())
                {
                    _linuxUser = LinuxUserInfo.CreateLinuxUserInfo();

                    _linuxUser.Id = CheckNull.AgainstInt(reader["UserId"]);
                    _linuxUser.TenantId = CheckNull.AgainstString(reader["TenantId"]);
                    _linuxUser.EndPointSystemId = CheckNull.AgainstInt(reader["EndPointSystemId"]);
                    _linuxUser.UserName = CheckNull.AgainstString(reader["UserName"]);
                    _linuxUser.HomeDirectory = CheckNull.AgainstString(reader["HomeDirectory"]);
                    _linuxUser.CommandShell = CheckNull.AgainstString(reader["CommandShell"]);
                    _linuxUser.CreatedDate = CheckNull.AgainstDatetime(reader["CreatedDate"]);
                    _linuxUser.LastModifiedDate = CheckNull.AgainstDatetime(reader["LastModifiedDate"]);

                    _list.Add(_linuxUser);

                }
                return _list;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while getting Linux users for UserListing ", ex);
                throw ex;
            }
            finally
            {
                if (m_executor != null)
                    m_executor.IACMDbOperator.Close();
            }
        }

        public IList<string> GetLinuxUserGroupsByDIUserID(int DIUserID, int endPointSystemId)
        {
            string query = "DI_spGetLinuxUserGroupsByDIUserId";
            IList<string> usrGrps = new List<string>();
            string grpStr;
            IACMDbOperator iacmOperator = null;
            QueryExecutor executor = null;

            try
            {

                List<SqlParameter> parameters = new List<SqlParameter>();
                SqlParameter sqlParam;

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@DIUserId";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = DIUserID;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@EndPointSystemId";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = endPointSystemId;
                parameters.Add(sqlParam);

                iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);
                executor = new QueryExecutor(iacmOperator);
                IDataReader reader = executor.ExecuteQueryDR();

                if (reader == null)
                    return null;


                while (reader.Read())
                {
                    grpStr = CheckNull.AgainstString(reader["GroupName"]);

                    if (!string.IsNullOrEmpty(grpStr) && !usrGrps.Contains(grpStr))
                    {
                        usrGrps.Add(grpStr);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while Getting the Get User Linux Groups in UserListing.", ex);
                throw ex;
            }
            finally
            {
                executor.IACMDbOperator.Close();
            }

            return usrGrps;
        }
        #endregion

        public LinuxUserInfo GetPrimaryLINUXUserByIdorDIID(int UserId, int EndpointId)
        {
            LinuxUserInfo LinuxUser = new LinuxUserInfo();

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (EndpointId != 0)
                    {
                        LinuxUser = (from user in _context.LinuxUserInfos
                 .Include("EndPointSystem")
                 .Include("EndPointSystem.EndPointSystemType")
                                     where user.DIUserId == UserId && !user.IsLocked && user.EndPointSystemId == EndpointId
                                     select user).FirstOrDefault();
                    }
                    else
                        LinuxUser = (from user in _context.LinuxUserInfos
                .Include("EndPointSystem")
                .Include("EndPointSystem.EndPointSystemType")
                                     where user.Id == UserId
                                     select user).FirstOrDefault();


                }
            }
            catch (Exception)
            {

                // throw;
            }
            return LinuxUser;
        }


        public AccountDetail GetAccountDetailByEndPointUserID(int EndpointUserId)
        {
            AccountDetail AccountDetail = new AccountDetail();
            LinuxUserInfo LinuxUserInfo = new LinuxUserInfo();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                LinuxUserInfo = oIACMDBContext.LinuxUserInfos.Where(lnxusr => lnxusr.Id == EndpointUserId).FirstOrDefault();
                if (LinuxUserInfo == null)
                    return AccountDetail;
                AccountDetail.LoginName = LinuxUserInfo.AccountName;
                AccountDetail.AccountType = LinuxUserInfo.AccountType;
                AccountDetail.IsMappedAccount = LinuxUserInfo.DIUserId == null ? false : true;
                AccountDetail.LastLogonDate = LinuxUserInfo.LastModifiedDate;
                AccountDetail.Version = Convert.ToString(LinuxUserInfo.Version);
                AccountDetail.DIUserID = LinuxUserInfo.DIUserId;
                AccountDetail.EndpointSystemId = LinuxUserInfo.EndPointSystemId;
                AccountDetail.EndpointUserId = EndpointUserId;
            }
            return AccountDetail;
        }


        public void UpdateAccountDetail(AccountDetail AccountDetail)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                LinuxUserInfo LinuxInfo = Context.LinuxUserInfos.FirstOrDefault(ENDPT => ENDPT.Id == AccountDetail.EndpointUserId && ENDPT.EndPointSystemId == AccountDetail.EndpointSystemId);
                if (LinuxInfo != null)
                {
                    LinuxInfo.DIUserId = AccountDetail.DIUserID; ;
                    LinuxInfo.LastModifiedDate = DateTime.Today;
                    LinuxInfo.MappingType = "Manual";
                    LinuxInfo.AccountType = AccountDetail.AccountType;

                    Context.SaveChanges();
                }
            }
        }



        public void UpdateUser(LinuxUserInfo endpointUser)
        {
            try
            {

                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(endpointUser);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<LinuxUserInfo>();
                        LinuxUserInfo attached = set.Find(endpointUser.Id);

                        if (attached != null)
                        {
                            var attachedEntry = _context.Entry(attached);
                            attachedEntry.CurrentValues.SetValues(endpointUser);
                        }
                    }
                    _context.SaveChanges();
                }

            }
            catch (Exception)
            {

                // throw;
            }
        }

        public bool ChecklinuxUserNameExists(string userName, int EndPointId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (_context.LinuxUserInfos.Count(user => user.UserName == userName && user.EndPointSystemId == EndPointId && user.IsLocked == false) > 0);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void DeleteLinuxUserGroup(int EndpointId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Database.ExecuteSqlCommand(string.Format("Delete from LinuxUserInfoLinuxGroupInfo Where EndpointSYstemId={0}", EndpointId));
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<LinuxUserInfo> GetLinuxUsersByEndpointId(long EndpointId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LinuxUserInfos.Where(c => c.EndPointSystemId == EndpointId && c.IsLocked == false).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveUserGroup(IList<LinuxUserGroup> listUserGroups, int EndpointId)
        {

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Database.ExecuteSqlCommand(string.Format("delete from LinuxUserInfoLinuxGroupInfo where EndpointSystemId={0}", EndpointId));
                    //_context.LinuxUserGroup.AddRange(listUserGroups);
                    foreach (LinuxUserGroup user in listUserGroups)
                    {
                        LinuxUserInfo UserInfo = _context.LinuxUserInfos.Where(c => c.UserName == user.UserName && c.IsLocked == false && c.EndPointSystemId == EndpointId).FirstOrDefault();
                        foreach (string str in user.GroupNumber.Split(','))
                        {
                            LinuxGroupInfo GroupInfo = _context.LinuxGroupInfos.Where(c => c.GroupName == str && c.EndPointSystemId == EndpointId).FirstOrDefault();
                            UserInfo.LinuxGroup.Add(GroupInfo);
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

        public List<LinuxGroupInfo> GetAllGroups()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LinuxGroupInfos.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<LinuxUserInfo> GetAllusers(int UserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    // return _context.LinuxUserInfos.Include("LinuxGroup").Where(c => !c.IsLocked && c.DIUserId == UserId).ToList();
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static IList<LinuxUserInfo> GetAllLinuxUsers()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LinuxUserInfos.Where(c => !c.IsLocked).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static IList<LinuxUserInfo> GetAllLinuxUsersReport()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LinuxUserInfos.Where(c => !c.IsLocked).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static object GetLinuxUserToGroupsReportData()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from Users in _context.LinuxUserInfos
                            from Groups in Users.LinuxGroup
                            select new LinuxUserGroup
                            {
                                UserName = Users.UserName,
                                GroupNumber = Groups.GroupName,
                                EndpointSystemId = Users.EndPointSystemId,
                                CreatedBy = Users.CreatedBy,
                                CreatedDate = Users.CreatedDate,
                                LastModifiedBy = Users.LastModifiedBy,
                                LastModifiedDate = Users.LastModifiedDate,
                                Id = Users.Id

                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static object GetLinuxGrouptoUserReportData()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from LinuxGroup in _context.LinuxGroupInfos.Include("LinuxGroup")
                            from LinuxUser in LinuxGroup.LinuxUserInfo
                            select new LinuxUserGroup
                            {
                                UserName = LinuxUser.UserName,
                                GroupNumber = LinuxGroup.GroupName,
                                EndpointSystemId = LinuxGroup.EndPointSystemId,
                                CreatedBy = LinuxGroup.CreatedBy,
                                CreatedDate = LinuxGroup.CreatedDate,
                                LastModifiedBy = LinuxGroup.LastModifiedBy,
                                LastModifiedDate = LinuxGroup.LastModifiedDate
                            }
                            ).ToList();
                }
            }

            catch (Exception)
            {
                throw;
            }

        }





        public IList<UserAccount> GetAccountDetail(int UserID)
        {
            IList<UserAccount> Listtobereturn = new List<UserAccount>();
            IList<LinuxUserInfo> listNPACUserInfo = new List<LinuxUserInfo>();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                listNPACUserInfo = oIACMDBContext.LinuxUserInfos.Include("EndPointSystem").Where(usr => usr.DIUserId == UserID && usr.IsLocked == false).ToList();
                foreach (LinuxUserInfo user in listNPACUserInfo)
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
















        public List<LinuxUserInfo> GetLINUXGroupsByUserId(long endpointUserId, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LinuxUserInfos.Include("LinuxGroup").Where(c => c.DIUserId == endpointUserId && c.IsLocked == false && c.EndPointSystemId == endpointSystemId).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void DeleteLinuxUserGroup(LinuxUserInfo linuxUserValue)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    LinuxUserInfo currentLINUXUser = oIACMDBContext.LinuxUserInfos.FirstOrDefault(item => item.Id == linuxUserValue.Id);

                    foreach (string group in linuxUserValue.Groups)
                    {
                        currentLINUXUser.LinuxGroup.RemoveAll(item => group.Equals(item.GroupName, StringComparison.CurrentCultureIgnoreCase));
                    }
                    oIACMDBContext.SaveChanges();

                    foreach (LinuxGroupInfo group in linuxUserValue.LinuxGroup)
                    {
                        oIACMDBContext.Database.ExecuteSqlCommand(string.Format("update LinuxUserInfoLinuxGroupInfo set EndpointSystemId={0}, Version={1} where LinuxGroupInfo_Id={2} and LinuxUserInfo_Id={3}", group.EndPointSystemId, group.Version,
                            group.Id, linuxUserValue.Id));
                        oIACMDBContext.SaveChanges();
                    }

                }



            }
            catch (Exception)
            {

            }
        }

        public IList<LinuxGroupInfo> GetLINUXGroups(int userId, int EndPointSystemId)
        {
            try
            {
                using (IACMDBContext context = new IACMDBContext())
                {
                    var User = _context.LinuxUserInfos.Where(c => c.DIUserId == userId && c.EndPointSystemId == EndPointSystemId).Select(c => c.LinuxGroup).FirstOrDefault();
                    return User;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool UpdateLinuxUserInfoFromProvisioning(LinuxUserInfo linuxUserValue)
        {
            try
            {

                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(linuxUserValue);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<LinuxUserInfo>();
                        LinuxUserInfo attached = set.Find(linuxUserValue.Id);

                        if (attached != null)
                        {
                            var attachedEntry = _context.Entry(attached);
                            attachedEntry.CurrentValues.SetValues(linuxUserValue);
                        }
                    }
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long AddLinuxUser(LinuxUserInfo LinuxUser)
        {
            try
            {

                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.LinuxUserInfos.Add(LinuxUser);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return LinuxUser.Id;
        }

        public LinuxUserInfo GetLockedLinuxUserByDIUserId(long UserId, long EndpointSystemId)
        {
            try
            {

                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LinuxUserInfos.Where(c => c.DIUserId == UserId && c.EndPointSystemId == EndpointSystemId && c.IsLocked).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long GetLinuxUserbyNameEndpoint(string UserName, long EndpointSystemId)
        {
            try
            {

                using (IACMDBContext _context = new IACMDBContext())
                {
                    var Item = _context.LinuxUserInfos.Where(c => c.UserName == UserName && c.EndPointSystemId == EndpointSystemId).FirstOrDefault();
                    return (Item == null) ? 0 : Item.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long GetLinuxGroupbyNameEndpoint(string GroupName, long EndpointSystemId)
        {
            try
            {

                using (IACMDBContext _context = new IACMDBContext())
                {
                    var Item = _context.LinuxGroupInfos.Where(c => c.GroupName == GroupName && c.EndPointSystemId == EndpointSystemId).FirstOrDefault();
                    return (Item == null) ? 0 : Item.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //AccessRequest
        public List<AccessRequestItem> GetAccessRequestItem(string searchString)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                return (from groups in Context.LinuxGroupInfos
                        where groups.GroupName.Contains(searchString) == true
                        orderby groups.GroupName
                        select new AccessRequestItem
                        {
                            AccessRequestItemType = AccessRequestItemType.Entitlement,
                            EntitlementType = Infrastructure.Enums.EntitlementType.Group,
                            EndpointType = EndpointType.Linux,
                            KeyId = groups.Id,
                      //   EndpointSystemId = groups.EndPointSystemId.,
                            EndpointSystemTypeId = (int)EndpointType.Linux,
                            ItemName = groups.GroupName,
                            ItemDescription=groups.GroupName,
                            ItemDetail=groups.GroupName
                           
                        }).ToList();
            }
        }
    }
}


