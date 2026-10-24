using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Model;
using System.Data;
using System.Data.SqlClient;

using System.Configuration;
using DeepIdentity.Infrastructure;
using System.Collections;
using System.ComponentModel;
using DeepIdentity.Model.LotusNotes;
using System.Data.Entity;
using DeepIdentity.Logger;
using DeepIdentity.Infrastructure.Enums;

namespace DeepIdentity.DataAccess
{
    //*UserGroupMap 
    public class LNSUserGroup : IAggregateRoot
    {


        public int LotusUserInfo_UserId { get; set; }
        public int LotusGroupInfo_Id { get; set; }
        public int EndpointSystemId { get; set; }
        public int? Version { get; set; }
    }


    public class LotusNotesDA
    {
        private IACMDbOperator m_iacmOperator;

        QueryExecutor executor = null;
        private QueryExecutor m_executor;
        public IACMDBContext _context;


        #region ILotusNotesDA Members

        public void AddLotusUserInfoBulk(IList<LotusUserInfo> lotusUserInfo)
        {
            using (_context = new IACMDBContext())
            {
                _context.Database.ExecuteSqlCommand("Truncate table DI_LNS_Users");
            }

            SQLBulkUploader<LotusUserInfo> lnsUser = null;
            try
            {
                string tablename = "DI_LNS_Users";
                lnsUser = new SQLBulkUploader<LotusUserInfo>(tablename);
                lnsUser.Flush(lotusUserInfo);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                lnsUser.Close();
            }
        }



        #endregion

        #region ILotusNotesDA Members

        public void UpdateLnsUserInfoOnAttributeMap(DataTable dtLnsUser)
        {

            int newRowCount = -1;


            SqlConnection conn = null;
            SqlCommand selectCmd = null;
            SqlCommand updateCmd = null;
            SqlCommand insertCommand = null;
            SqlCommand deletecommand = null;
            SqlDataAdapter da = null;

            StringBuilder strInsert = new StringBuilder();
            StringBuilder strUpdate = new StringBuilder();
            StringBuilder strDelete = new StringBuilder();
            try
            {
                m_iacmOperator = new IACMDbOperator();

                conn = m_iacmOperator.GetConnection();

                selectCmd = conn.CreateCommand();


                selectCmd.CommandText = "SELECT UserId,InternetAddress,Name, DIUserId FROM DI_LNS_Users";
                updateCmd = conn.CreateCommand();
                updateCmd.CommandText = "UPDATE DI_LNS_Users Set DIUserId=@DIUserId Where UserId=@UserId";
                updateCmd.CommandType = CommandType.Text;


                //selectCmd.CommandText = "DI_spGetAllLNSUserInfo";

                //updateCmd = conn.CreateCommand();
                //updateCmd.CommandText = "DI_spUpdateLNSUserInfo";
                //updateCmd.CommandType = CommandType.StoredProcedure;

                List<SqlParameter> parameters = null;
                parameters = GetUserParams();
                updateCmd.Parameters.AddRange(parameters.ToArray());

                updateCmd.UpdatedRowSource = UpdateRowSource.None;

                insertCommand = conn.CreateCommand();
                insertCommand.CommandText = "DI_spAddLNSUserInfo";
                insertCommand.CommandType = CommandType.StoredProcedure;

                List<SqlParameter> insertParams = GetUserParams();

                insertCommand.Parameters.AddRange(insertParams.ToArray());
                insertCommand.UpdatedRowSource = UpdateRowSource.None;

                deletecommand = conn.CreateCommand();
                deletecommand.CommandText = "Delete DI_LNS_Users where Id=@Id";
                deletecommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int, 4, "Id"));
                updateCmd.UpdatedRowSource = UpdateRowSource.None;

                da = new SqlDataAdapter(selectCmd);
                da.UpdateCommand = updateCmd;

                da.UpdateBatchSize = 100;

                da.AcceptChangesDuringUpdate = true;

                DataSet ds = new DataSet();
                da.Fill(ds, "LNSUserInfo");

                DataRow[] drowsMax = ds.Tables[0].Select("UserId = max(UserId)");

                if (drowsMax.Count() <= 0)
                    newRowCount = 0;
                else
                    newRowCount = Convert.ToInt32(drowsMax[0]["UserId"]);

                //DataTable dtNew = dtAdsUser;

                if (ds.Tables[0].Rows.Count <= 0)
                {

                    ds.Tables[0].Merge(dtLnsUser, false);
                    da.Update(ds, "LNSUserInfo");
                    return;
                }

                DataRow drow = null;

                try
                {
                    foreach (DataRow dr in dtLnsUser.Rows)
                    {
                        DataRow[] dRows = ds.Tables[0].Select("InternetAddress='" + dr["InternetAddress"].ToString().Replace("'", "''") + "'");




                        if (dRows.Count() <= 0)
                        {


                            drow = ds.Tables[0].NewRow();
                            drow["UserId"] = ++newRowCount;
                            drow["DIUserId"] = dr["DIUserId"];

                            ds.Tables[0].Rows.Add(drow);
                        }

                        else
                        {

                            //foreach (DataRow dRowTemp in dRows)
                            //{
                            drow = dRows[0];

                            drow["DIUserId"] = dr["DIUserId"];
                            //dRowTemp["DIUserId"] = dr["DIUserId"];
                            strUpdate = strUpdate.AppendFormat(",{0}", drow["UserId"]);
                            //}
                        }






                    }
                }
                catch (Exception)
                {
                }




                if (strUpdate.ToString() != string.Empty)
                    Log.General.WriteMessage(LogLevel.INFO, "Updated userId(s):" + strUpdate.ToString().Split(',').Count());


                try
                {
                    da.Update(ds.Tables[0].Select(null, null, DataViewRowState.ModifiedCurrent));

                }
                catch (Exception ex)
                {
                    Log.General.WriteMessage(LogLevel.ERROR, "UserInfo update Error", ex);

                }


            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "UserInfo Error", ex);
                throw;
            }
            finally
            {
                m_iacmOperator.Close();

                selectCmd.Dispose();

                updateCmd.Dispose();
                deletecommand.Dispose();

            }
        }

        private List<SqlParameter> GetUserParams()
        {
            SqlParameter sqlParam = null;
            List<SqlParameter> parameters = new List<SqlParameter>();
            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@DIUserId";
            sqlParam.SqlDbType = SqlDbType.Int;
            sqlParam.SourceColumn = "DIUserId";
            parameters.Add(sqlParam);

            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@UserId";
            sqlParam.SqlDbType = SqlDbType.Int;
            sqlParam.SourceColumn = "UserId";
            sqlParam.Value = 1;
            parameters.Add(sqlParam);

            return parameters;
        }
        #endregion

        public LNSConfiguration GetConfiguration(int _systemid)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    LNSConfiguration config = null;
                    config = _context.lnsConfiguration.FirstOrDefault(s => s.EndPointSystemId == _systemid);
                    config.Password = RSAHelper.DecryptData(config.Password);
                    return config;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SaveUserGroupMap(Dictionary<int, string> dicUsers, Dictionary<int, string> dicGroups, Dictionary<string, List<object>> dicGroupUsers, int endpointSystemid)
        {
            SQLBulkUploader<LNSUserGroup> sqlUserGroupMap = null;

            try
            {
                using (_context = new IACMDBContext())
                {
                    _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM LotusUserInfoLotusGroupInfo WHERE EndpointSystemId = {0}", endpointSystemid));
                }

                IList<LNSUserGroup> listUG = new List<LNSUserGroup>();

                LNSUserGroup userGroup = null;

                foreach (var group in dicGroupUsers)
                {

                    foreach (var user in group.Value)
                    {
                        if (user == null)
                            continue;

                        userGroup = new LNSUserGroup();

                        userGroup.LotusGroupInfo_Id = dicGroups.FirstOrDefault(x => x.Value == group.Key).Key;

                        try
                        {
                            string usr = user.ToString().ToUpper();
                            var usrs = dicUsers.FirstOrDefault(x => x.Value != null && x.Value.ToUpper() == usr);

                            userGroup.LotusUserInfo_UserId = usrs.Key;

                            userGroup.EndpointSystemId = endpointSystemid;
                            userGroup.Version = Convert.ToInt16(VersionNo.Available);

                            if (userGroup.LotusUserInfo_UserId != 0)
                            {
                                var usrGrp = listUG.FirstOrDefault(item => item.LotusGroupInfo_Id == userGroup.LotusGroupInfo_Id && item.LotusUserInfo_UserId == userGroup.LotusUserInfo_UserId);
                                if (usrGrp == null)
                                {
                                    listUG.Add(userGroup);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Error, "Error while saving user group mapping.", ex, DiscoveryObjectType.User, 0);
                        }

                    }
                }

                string tableName = "LotusUserInfoLotusGroupInfo";

                sqlUserGroupMap = new SQLBulkUploader<LNSUserGroup>(tableName);

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

        public int AddLNSUser(LotusUserInfo lnsUserInfo)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    _context.LotusUserInfos.Add(lnsUserInfo);
                    _context.SaveChanges();
                    return lnsUserInfo.UserId;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateLNSUser(LotusUserInfo lnsUserInfo)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var accReqEntry = _context.Entry(lnsUserInfo);
                    if (accReqEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<LotusUserInfo>();
                        LotusUserInfo attachedAccReq = set.Find(lnsUserInfo.UserId);

                        if (attachedAccReq != null)
                        {
                            var attachedEntry = _context.Entry(attachedAccReq);
                            attachedEntry.CurrentValues.SetValues(lnsUserInfo);
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

        public LotusUserInfo GetUserByInternetAddress(string InternetAddress, int EndpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    return _context.LotusUserInfos.FirstOrDefault(user => user.InternetAddress == InternetAddress && user.EndPointSystemId == EndpointSystemId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public LotusUserInfo GetUserByShortName(string shortName, int EndpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    return _context.LotusUserInfos.FirstOrDefault(user => user.ShortName == shortName && user.EndPointSystemId == EndpointSystemId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public LotusGroupInfo GetLNSGroup(LotusGroupInfo ExistingLNSGroup)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    return _context.LotusGroupInfos.FirstOrDefault(group => group.Group == ExistingLNSGroup.Group);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AddLNSGroup(LotusGroupInfo NewLNSGroup)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    _context.LotusGroupInfos.Add(NewLNSGroup);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateLNSGroup(LotusGroupInfo existingLnsGroup)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    _context.LotusGroupInfos.Attach(existingLnsGroup);
                    _context.Entry(existingLnsGroup).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Exception, ex.InnerException.StackTrace, ex, DiscoveryObjectType.Group, 0);
                throw;
            }
        }

        public IList<LotusCertifierInfo> GetAllLNSCertifiers()
        {
            IList<LotusCertifierInfo> _list = new List<LotusCertifierInfo>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                    _list = _context.LotusCertifierInfos.ToList();
                return _list;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while getting Lotus Notes Certifiers ", ex);
                throw ex;
            }
        }

        public LotusCertifierInfo GetLNSCertifier(LotusCertifierInfo existingLNSCertifier)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    return _context.LotusCertifierInfos.FirstOrDefault(certifier => certifier.FullName == existingLNSCertifier.FullName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<int, string> GetUserIdsWithOwner(int EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.LotusUserInfos.Where(ep => ep.EndPointSystemId == EndpointId).ToDictionary(item => item.UserId, item => item.Owner);
            }
        }
        public Dictionary<int, string> GetGroupIdsWithOwner(int EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.LotusGroupInfos.Where(ep => ep.EndPointSystemId == EndpointId).ToDictionary(item => item.Id, item => item.Group);
            }
        }
        public void AddLNSCertifier(LotusCertifierInfo newLNSCertifier)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    _context.LotusCertifierInfos.Add(newLNSCertifier);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateLNSCertifier(LotusCertifierInfo existingLnsCertifier)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    _context.LotusCertifierInfos.Attach(existingLnsCertifier);
                    _context.Entry(existingLnsCertifier).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Exception, ex.InnerException.StackTrace, ex, DiscoveryObjectType.Certifier, 0);
                throw;
            }
        }

        private DataTable getUserInfoTable()
        {
            DataTable table = new DataTable();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(LotusUserInfo));

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

            DataColumn dcol = table.Columns["Id"];
            dcol.AutoIncrement = true;
            dcol.AutoIncrementSeed = 1;
            dcol.AutoIncrementStep = 1;

            dcol = table.Columns["DIUserId"];
            dcol.AutoIncrement = true;
            dcol.AutoIncrementSeed = 1;
            dcol.AutoIncrementStep = 1;

            return table;

        }


        #region ILotusNotesDA Members


        public void AddLNSGroupInfoBulk(IList<LotusGroupInfo> groups)
        {
            SQLBulkUploader<LotusGroupInfo> lnsGroup = null;
            try
            {
                string tablename = "DI_LNS_Groups";
                lnsGroup = new SQLBulkUploader<LotusGroupInfo>(tablename);
                lnsGroup.Flush(groups);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                lnsGroup.Close();
            }
        }

        public void AddLNSGroupInfoHistoryBulk(IList<LotusGroupHistoryInfo> groups)
        {
            SQLBulkUploader<LotusGroupHistoryInfo> sqlGroupHistory = null;
            try
            {
                string tableName = "DI_LNS_GroupsHistory";
                sqlGroupHistory = new SQLBulkUploader<LotusGroupHistoryInfo>(tableName);
                sqlGroupHistory.Flush(groups);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlGroupHistory.Close();
            }
        }
        public void DeleteAll<T>()
        {
            int noAffected = -1;

            string strTableName = string.Empty;

            try
            {
                Type type = typeof(T);

                switch (type.Name)
                {
                    case "LNSUserInfo":
                        strTableName = "DI_LNS_Users";
                        break;
                    case "LotusGroupInfo":
                        strTableName = "DI_LNS_Groups";
                        break;
                    case "LotusDomainInfo":
                        strTableName = "DI_LNS_Domains";
                        break;
                    case "LotusUserGroupMap":
                        strTableName = "DI_LNS_UserGroupMap";
                        break;
                    default:
                        break;
                }

                string strQuery = string.Format("Truncate Table {0}", strTableName);

                m_iacmOperator = new IACMDbOperator(strQuery, CommandType.Text, null);

                m_executor = new QueryExecutor(m_iacmOperator);

                noAffected = m_executor.ExecuteQuery();

                if (noAffected <= 0)
                {
                    return;
                }

                Log.General.WriteMessage(LogLevel.DEBUG, string.Format("Deleted {0} records from table {1}",
                                                                noAffected,
                                                                strTableName));



            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                m_iacmOperator.Close();
            }


        }

        #endregion

        #region ILotusNotesDA Members


        public void AddLNSDomainInofBulk(IList<LotusDomainInfo> domains)
        {

            SQLBulkUploader<LotusDomainInfo> lnsDomain = null;
            try
            {
                using (_context = new IACMDBContext())
                {
                    _context.Database.ExecuteSqlCommand("Truncate table DI_LNS_Domains");
                }

                string tablename = "DI_LNS_Domains";
                lnsDomain = new SQLBulkUploader<LotusDomainInfo>(tablename);
                lnsDomain.Flush(domains);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                lnsDomain.Close();
            }
        }

        #endregion

        #region ILotusNotesDA Members



        public IList<LotusUserInfo> GetAllLNSUserInfo()
        {
            //string query = "DI_spGetAllLNSUserInfo";
            IList<LotusUserInfo> _list = new List<LotusUserInfo>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                    _list = _context.LotusUserInfos.ToList();
                return _list;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while getting  LotusUsers ", ex);
                throw ex;
            }
        }

        public bool CheckLNSUserNameExists(string userName, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LotusUserInfos.Any(item => item.ShortName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)
                        && item.EndPointSystemId == endpointSystemId);
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LotusUserInfos.Any(item => item.InternetAddress.Equals(email, StringComparison.CurrentCultureIgnoreCase)
                        && item.EndPointSystemId == endpointSystemId);
                }
            }
            catch
            {
                throw;
            }
        }
        public bool CheckLNSLastNameExists(string lastName, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LotusUserInfos.Any(item => item.LastName.Equals(lastName, StringComparison.CurrentCultureIgnoreCase)
                        && item.EndPointSystemId == endpointSystemId);
                }
            }
            catch
            {
                throw;
            }
        }
        public IList<LotusUserInfo> GetLNSUserInfoByDIUserId(int DIUserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from user in _context.LotusUserInfos
                                 .Include("EndPointSystem")
                                 .Include("EndPointSystem.EndPointSystemType")
                            where user.DIUserId == DIUserId
                            orderby user.CreatedDate
                            select user).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, string.Format("Error Occured while getting All LotusUser DIUserId={0}", DIUserId), ex);
                throw ex;
            }
        }

        public List<LotusUserInfo> GetPrimaryLNSUserByDIUserId(int DIUserId, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LotusUserInfos.Where(item => item.DIUserId == DIUserId && item.AccountType == AccountType.User
                                && item.EndPointSystemId == endpointSystemId && !item.IsLocked).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, string.Format("Error Occured while getting All LotusUser DIUserId={0}", DIUserId), ex);
                throw ex;
            }
        }

        public LotusUserInfo GetLockedLNSUserByDIUserId(int DIUserId, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LotusUserInfos.FirstOrDefault(item => item.DIUserId == DIUserId && item.EndPointSystemId == endpointSystemId
                            && item.AccountType == AccountType.User && item.IsLocked);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LotusUserInfo GetLNSUserByUserId(int userId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LotusUserInfos.FirstOrDefault(item => item.UserId == userId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LotusUserInfo GetLNSUserByMailAddress(string mailAddress)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LotusUserInfos.FirstOrDefault(item => item.InternetAddress == mailAddress);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LotusUserInfo GetLNSUserByShortName(string shortName)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LotusUserInfos.FirstOrDefault(item => item.ShortName == shortName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IList<LotusUserInfo> GetAllUnMappedLNSUsers()
        {
            try
            {
                IList<LotusUserInfo> lstLotusUserInfo = new List<LotusUserInfo>();
                return lstLotusUserInfo;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion



        public void AddLotusUserGroupMapInfo(IList<LotusUserGroupMapHistory> lstgroupmap)
        {
            SQLBulkUploader<LotusUserGroupMapHistory> lnsGroupMap = null;
            try
            {
                string tablename = "DI_LNS_UserGroupMapHistory";
                lnsGroupMap = new SQLBulkUploader<LotusUserGroupMapHistory>(tablename);
                lnsGroupMap.Flush(lstgroupmap);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                lnsGroupMap.Close();
            }
        }



        public void AddLotusUserGroupMapInfoBulk(IList<LotusUserGroupMap> listUserGroupMap)
        {
            DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Info, "Before Bulk Update", DiscoveryObjectType.Group, listUserGroupMap.Count);

            SQLBulkUploader<LotusUserGroupMap> lnsGroupMap = null;
            try
            {
                using (_context = new IACMDBContext())
                {
                    _context.Database.ExecuteSqlCommand("Truncate table LotusUserInfoLotusGroupInfo");
                }

                string tablename = "LotusUserInfoLotusGroupInfo";
                lnsGroupMap = new SQLBulkUploader<LotusUserGroupMap>(tablename);
                lnsGroupMap.Flush(listUserGroupMap);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                lnsGroupMap.Close();
            }
        }

        public LotusUserInfo GetLotusNotesUserByUserName(string fullName)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    return _context.LotusUserInfos.FirstOrDefault(user => user.FullName.Equals(fullName, StringComparison.CurrentCultureIgnoreCase));
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public LotusUserInfo GetLotusNotesUserByOwner(string owner)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    return _context.LotusUserInfos.FirstOrDefault(user => user.Owner.Equals(owner, StringComparison.CurrentCultureIgnoreCase));
                }

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
                using (_context = new IACMDBContext())
                {
                    return _context.LotusGroupInfos.FirstOrDefault(group => group.Group == GroupName);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<LotusGroupInfo> GetLnsGroupsByDIUserId(int? endPointSystemId, int userId)
        {
            IList<LotusGroupInfo> lotusgroupsList = new List<LotusGroupInfo>();

            try
            {
                LotusUserInfo lotusUser = null;

                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (endPointSystemId == null)
                        lotusUser = oIACMDBContext.LotusUserInfos.FirstOrDefault(lnsUser => lnsUser.UserId == userId);
                    else
                    {
                        lotusUser = oIACMDBContext.LotusUserInfos.FirstOrDefault(lnsUser => lnsUser.UserId == userId && lnsUser.EndPointSystemId == endPointSystemId.Value);

                    }
                    if (lotusUser != null)
                    {
                        lotusgroupsList = lotusUser.LotusGroupInfos.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while getting Groups By DIUserId ", ex);
                throw ex;
            }
            return lotusgroupsList;
        }
        public Dictionary<int, string> GetAllLNSGroupInfo(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LotusGroupInfos.Where(item => item.Group.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.Group })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }
        public IList<LotusGroupInfo> GetAllLNSGroupInfo()
        {
            IList<LotusGroupInfo> _list = new List<LotusGroupInfo>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                    _list = _context.LotusGroupInfos.ToList();
                return _list;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while getting Lotus Notes Groups ", ex);
                throw ex;
            }
        }

        public bool IsMailFileNameExist(string mailFileName)
        {
            bool isNameExist = false;
            string tmpMailFileName = string.Empty;
            try
            {
                tmpMailFileName = string.Format(@"mail\{0}", mailFileName);

                using (IACMDBContext _context = new IACMDBContext())
                {
                    int cnt = _context.LotusUserInfos.Where(ui => ui.MailFile == tmpMailFileName).Count();
                    isNameExist = (cnt > 0) ? true : false;
                }
                return isNameExist;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while checking mail file name in Lotus Notes.", ex);
                throw;
            }
        }

        public bool IsInternetAddressExist(string internetAddress)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LotusUserInfos.Any(item => item.InternetAddress.StartsWith(internetAddress));
                }
            }
            catch
            {
                throw;
            }
        }

        public bool IsLastNameExist(string lastName)
        {
            bool isNameExist = false;
            try
            {

                using (IACMDBContext _context = new IACMDBContext())
                {
                    int cnt = _context.LotusUserInfos.Where(ui => ui.LastName == lastName).Count();
                    isNameExist = (cnt > 0) ? true : false;
                }
                return isNameExist;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while checking Last Name in Lotus Notes.", ex);
                throw;
            }
        }

        public IList<LNSUserToGroupsReportData> GetLNSUserToGroupsReportData()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return (from lnsUser in _context.LotusUserInfos
                            from lnsGrp in lnsUser.LotusGroupInfos
                            where (lnsGrp.Group != null && lnsGrp.Group != string.Empty)
                            select new LNSUserToGroupsReportData
                            {
                                UserId = lnsUser.UserId,
                                UserName = lnsUser.FullName,
                                DisplayName = lnsUser.ShortName,
                                InternetAddress = lnsUser.InternetAddress,
                                Department = lnsUser.Department,

                                GroupId = lnsGrp.Id,
                                GroupName = lnsGrp.Group,
                                GroupDescription = lnsGrp.Description,
                                Category = lnsGrp.Category
                            }
                            ).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return (from lnsGrp in _context.LotusGroupInfos
                            from lnsUser in lnsGrp.LotusUserInfos
                            where (lnsUser.FullName != null && lnsUser.FullName != string.Empty)
                            select new LNSGroupToUsersReportData
                            {
                                GroupId = lnsGrp.Id,
                                GroupName = lnsGrp.Group,
                                GroupDescription = lnsGrp.Description,
                                Category = lnsGrp.Category,

                                UserId = lnsUser.UserId,
                                UserName = lnsUser.FullName,
                                DisplayName = lnsUser.ShortName,
                                InternetAddress = lnsUser.InternetAddress,
                                Department = lnsUser.Department,
                                HTTPPasswordChangeDate = lnsUser.HTTPPasswordChangeDate
                            }
                            ).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Provisioning Methods

        public bool UpdateLNUserInfoFromProvisioning(LotusUserInfo lnsUserInfo)
        {
            bool isDataSaved = false;
            if (lnsUserInfo == null)
                return isDataSaved;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var reviewEPEntry = _context.Entry(lnsUserInfo);
                    if (reviewEPEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<LotusUserInfo>();
                        LotusUserInfo attachedreviewEP = set.Find(lnsUserInfo.UserId);

                        if (attachedreviewEP != null)
                        {
                            var attachedEntry = _context.Entry(attachedreviewEP);
                            attachedEntry.CurrentValues.SetValues(lnsUserInfo);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while updating  LotusUserInfo From Provisioning.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        #endregion

        public IList<LotusUserHistoryInfo> GetAllLNSHistoryUsers()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                    return _context.LotusUserHistoryInfos.ToList();
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while getting Lotus Notes History Users ", ex);
                throw ex;
            }
        }
        public IList<LotusUserHistoryInfo> GetLNShistoryUsersbyEndpointID(int EndpointId)
        {

            IList<LotusUserHistoryInfo> lnsusers = new List<LotusUserHistoryInfo>();
            using (IACMDBContext _context = new IACMDBContext())
            {
                return _context.LotusUserHistoryInfos.Where(item => item.EndPointSystemId == EndpointId).ToList();
            }
        }

        public IList<EndpointObjects> GetLNSObjects()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    EndPointSystemType ep = _context.EndpointSystemTypes.Where(e => e.EndpointSystemTypeName == EndpointType.LotusNotes).FirstOrDefault();
                    return _context.EndpointObjects.Where(e => e.EndpointSystemTypeId == ep.EndpointSystemTypeId).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while getting All Lotus Notes EndpointObjects ", ex);
                throw ex;
            }
        }

        public IList<LotusNotesUsersReportData> GetAllLotusNotesUsersReportData()
        {
            IList<LotusUserInfo> _usrlist = new List<LotusUserInfo>();
            IList<LotusNotesUsersReportData> _usrReportDatalist = new List<LotusNotesUsersReportData>();
            LotusNotesUsersReportData _usrReportDataItem = new LotusNotesUsersReportData();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _usrlist = GetAllLNSUserInfo();
                    foreach (LotusUserInfo _usr in _usrlist)
                    {
                        _usrReportDataItem = new LotusNotesUsersReportData();
                        _usrReportDataItem.UserId = _usr.UserId;
                        _usrReportDataItem.ShortName = _usr.ShortName;
                        _usrReportDataItem.Name = _usr.Name;
                        _usrReportDataItem.LastName = _usr.LastName;
                        _usrReportDataItem.FullName = _usr.FullName;
                        _usrReportDataItem.InternetAddress = _usr.InternetAddress;
                        _usrReportDataItem.Department = _usr.Department;
                        _usrReportDataItem.CompanyName = _usr.CompanyName;
                        _usrReportDataItem.JobTitle = _usr.JobTitle;
                        _usrReportDataItem.HostName = _context.EndpointSystems.FirstOrDefault(ep => ep.EndpointSystemId == _usr.EndPointSystemId).DisplayName; // _usr.EndPointSystem.HostName; //
                        _usrReportDataItem.Institute = _usr.TenantId;
                        _usrReportDataItem.IsMapped = (_usr.DIUserId != null) ? "Yes" : "No";
                        _usrReportDataItem.Owner = (_usr.DIUserId != null) ? _context.Users.FirstOrDefault(usrs => usrs.UserId == _usr.DIUserId).DisplayName : "";
                        _usrReportDatalist.Add(_usrReportDataItem);
                    }
                }
                return _usrReportDatalist;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while getting  LotusUsers ", ex);
                throw ex;
            }
        }

        public IList<LotusNotesGroupsReportData> GetAllLotusNotesGroupsReportData()
        {
            IList<LotusGroupInfo> _grplist = new List<LotusGroupInfo>();
            IList<LotusNotesGroupsReportData> _grpReportDatalist = new List<LotusNotesGroupsReportData>();
            LotusNotesGroupsReportData _grpReportDataItem = new LotusNotesGroupsReportData();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _grplist = GetAllLNSGroupInfo();
                    foreach (LotusGroupInfo _grp in _grplist)
                    {
                        _grpReportDataItem = new LotusNotesGroupsReportData();
                        _grpReportDataItem.Group = _grp.Group;
                        _grpReportDataItem.Description = _grp.Description;
                        _grpReportDataItem.Category = _grp.Category;
                        _grpReportDataItem.Institution = _grp.TenantId;
                        _grpReportDataItem.HostName = _context.EndpointSystems.FirstOrDefault(ep => ep.EndpointSystemId == _grp.EndPointSystemId).DisplayName;
                        _grpReportDatalist.Add(_grpReportDataItem);
                    }
                }
                return _grpReportDatalist;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while getting Lotus Notes Groups ", ex);
                throw ex;
            }
        }

        public List<LotusGroupInfo> GetLNSGroupsByUserId(int userId, int endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LotusUserInfos.FirstOrDefault(item => item.UserId == userId && item.EndPointSystemId == endpointSystemId).LotusGroupInfos.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<LotusUserInfo> GetUsersExternalEmailAccess()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from lnsUsers in _context.LotusUserInfos
                            where lnsUsers.MailExternalAccess
                            select lnsUsers).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateUser(LotusUserInfo _lnsUser)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    _context.LotusUserInfos.Attach(_lnsUser);
                    _context.Entry(_lnsUser).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public LotusGroupInfo GetGroupByGroupName(string groupName)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    return _context.LotusGroupInfos.FirstOrDefault(item => item.Group == groupName);
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<LotusUserInfo> GetDeletedUsers(int EndpointId, Process objDiscoveryProcess)
        {
            using (_context = new IACMDBContext())
            {
                IList<LotusUserInfo> listDeletedUsers = new List<LotusUserInfo>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedUsers = _context.LotusUserInfos.Where(d => d.EndPointSystemId == EndpointId && d.LastModifiedDate < objDiscoveryProcess.ProcessStartAt).ToList();
                }

                return listDeletedUsers;
            }
        }

        public void RemoveLNSUser(LotusUserInfo user)
        {
            using (_context = new IACMDBContext())
            {
                LotusUserInfo lnsuser = new LotusUserInfo();
                lnsuser = _context.LotusUserInfos.First(u => u.UserId == user.UserId);
                _context.LotusUserInfos.Remove(lnsuser);
                _context.SaveChanges();
            }
        }

        public IList<LotusUserInfo> GetLotusNotesUsersByGroupName(string GroupName, int EndpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    LotusGroupInfo LotusGroup = _context.LotusGroupInfos.FirstOrDefault(group => group.Group == GroupName && group.EndPointSystemId == EndpointSystemId);
                    IList<LotusUserInfo> lotusUsers = new List<LotusUserInfo>();
                    if (LotusGroup != null)
                        lotusUsers = LotusGroup.LotusUserInfos;
                    return lotusUsers;
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
            IList<LotusUserInfo> listLNSUserInfo = new List<LotusUserInfo>();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                listLNSUserInfo = oIACMDBContext.LotusUserInfos.Where(lnsusr => lnsusr.DIUserId == UserID && lnsusr.IsLocked == false).ToList();
                foreach (LotusUserInfo lnsusr in listLNSUserInfo)
                {
                    if (lnsusr != null)
                    {
                        UserAccount UserAccount = new UserAccount
                        {
                            EndpointUserId = lnsusr.UserId,
                            UserName = lnsusr.AccountName,
                            FullName = lnsusr.FullName,
                            AccountType = lnsusr.AccountType,
                            LastLogonDate = lnsusr.LastModifiedDate,
                            EndpointSystemId = lnsusr.EndPointSystemId,
                            EndPointSystemType = lnsusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = lnsusr.EndPointSystem.DisplayName
                        };
                        Listtobereturn.Add(UserAccount);
                    }
                }
            }
            return Listtobereturn;
        }

        public IList<UserAccount> GetUserAccountDetailByApplication(int ApplicationID)
        {
            IList<UserAccount> Listtobereturn = new List<UserAccount>();
            IList<LotusUserInfo> listLNSUserInfo = new List<LotusUserInfo>();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                listLNSUserInfo = oIACMDBContext.LotusUserInfos.Where(lnsusr => lnsusr.EndPointSystemId == ApplicationID && lnsusr.IsLocked == false).ToList();
                foreach (LotusUserInfo lnsusr in listLNSUserInfo)
                {
                    if (lnsusr != null)
                    {
                        UserAccount UserAccount = new UserAccount
                        {
                            EndpointUserId = lnsusr.UserId,
                            UserName = lnsusr.AccountName,
                            FullName = lnsusr.FullName,
                            AccountType = lnsusr.AccountType,
                            LastLogonDate = lnsusr.LastModifiedDate,
                            EndpointSystemId = lnsusr.EndPointSystemId,
                            EndPointSystemType = lnsusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = lnsusr.EndPointSystem.DisplayName
                        };
                        Listtobereturn.Add(UserAccount);
                    }
                }
            }
            return Listtobereturn;
        }


        public IList<UserEntitlement> GetEntitlementDetail(int EndpointUserId)
        {
            IList<UserEntitlement> Listtobereturn = new List<UserEntitlement>();
            LotusUserInfo LotusUserInfo = new LotusUserInfo();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                LotusUserInfo = oIACMDBContext.LotusUserInfos.Where(lnsusr => lnsusr.UserId == EndpointUserId).FirstOrDefault();
                if (LotusUserInfo == null)
                    return Listtobereturn;

                foreach (LotusGroupInfo grp in LotusUserInfo.LotusGroupInfos)
                {
                    UserEntitlement UserEntitlement = new UserEntitlement();
                    UserEntitlement.EndpointUserId = LotusUserInfo.UserId;
                    UserEntitlement.UserName = LotusUserInfo.AccountName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                    UserEntitlement.EndPointSystemType = LotusUserInfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = LotusUserInfo.EndPointSystem.DisplayName;
                    if (grp != null)
                    {
                        UserEntitlement.Name = grp.Group;
                        UserEntitlement.Description = grp.Description;
                        Listtobereturn.Add(UserEntitlement);
                    }
                }
            }
            return Listtobereturn;
        }

        public AccountDetail GetAccountDetailByEndPointUserID(int EndpointUserId)
        {
            AccountDetail AccountDetail = new AccountDetail();
            LotusUserInfo LotusUserInfo = new LotusUserInfo();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                LotusUserInfo = oIACMDBContext.LotusUserInfos.Where(lnsusr => lnsusr.UserId == EndpointUserId).FirstOrDefault();
                if (LotusUserInfo == null)
                    return AccountDetail;
                AccountDetail.LoginName = LotusUserInfo.AccountName;
                AccountDetail.DisplayName = LotusUserInfo.FullName;
                AccountDetail.PhoneNo = LotusUserInfo.OfficePhoneNumber;
                //AccountDetail.Email = LotusUserInfo.EmailID;
                AccountDetail.AccountType = LotusUserInfo.AccountType;
                AccountDetail.IsMappedAccount = LotusUserInfo.DIUserId == null ? false : true;
                AccountDetail.LastLogonDate = LotusUserInfo.LastModifiedDate;
                AccountDetail.Version = Convert.ToString(LotusUserInfo.Version);
                AccountDetail.DIUserID = LotusUserInfo.DIUserId;
                AccountDetail.EndpointSystemId = LotusUserInfo.EndPointSystemId;
                AccountDetail.EndpointUserId = EndpointUserId;
            }
            return AccountDetail;
        }

        public void UpdateAccountDetail(AccountDetail AccountDetail)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                LotusUserInfo LOTUSUInfo = Context.LotusUserInfos.FirstOrDefault(ENDPT => ENDPT.UserId == AccountDetail.EndpointUserId && ENDPT.EndPointSystemId == AccountDetail.EndpointSystemId);
                if (LOTUSUInfo != null)
                {
                    LOTUSUInfo.DIUserId = AccountDetail.DIUserID; ;
                    LOTUSUInfo.LastModifiedDate = DateTime.Today;
                    LOTUSUInfo.MappingType = "Manual";
                    LOTUSUInfo.AccountType = AccountDetail.AccountType;
                    LOTUSUInfo.IfUnmappedMapTo = AccountDetail.UnMappedMapTo;
                    Context.SaveChanges();
                }
            }
        }

        public LotusUserInfo GetLNSUserInfo(int EndpointUserID)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                return Context.LotusUserInfos.Include("LotusGroupInfos").Where(lnsusr => lnsusr.UserId == EndpointUserID).FirstOrDefault();
            }
        }

        public List<AccessRequestItem> GetAccessRequestItem(string searchString)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                return (from groups in Context.LotusGroupInfos
                        where groups.Group.Contains(searchString) && groups.CanRequest == true
                        orderby groups.Group
                        select new AccessRequestItem
                        {
                            AccessRequestItemType = AccessRequestItemType.Entitlement,
                            EndpointType = EndpointType.LotusNotes,
                            KeyId = groups.Id,
                            EndpointSystemId = groups.EndPointSystemId,
                            EndpointSystemTypeId = (int)EndpointType.LotusNotes,
                            ItemName = groups.Group,
                            ItemDescription = groups.Description
                        }).ToList();
            }
        }

        public bool IsSecondartAccount(int EndpointUserID)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                return _context.SAPUserinfos.Where(item => item.DIUserId == EndpointUserID && item.AccountType == AccountType.User).Count() > 0 ? true : false;
            }
        }
    }
}
