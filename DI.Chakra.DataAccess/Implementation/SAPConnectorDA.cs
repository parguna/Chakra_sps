using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using System.Data;
using System.Data.SqlClient;

using System.Configuration;
using DI.Chakra.Infrastructure;
using System.Collections;
using System.ComponentModel;
using DI.Chakra.Model.SAP;
using System.Data.Entity;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Model.GridData;

namespace DI.Chakra.DataAccess
{

    public class SAPConnectorDA//:ISAPConnectorDA 
    {
        #region EF Conjunction Table Classes
        public class SAPUserGroup : IAggregateRoot
        {
            public long SAPGroupInfo_Id { get; set; }
            public long SAPUserinfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }
        public class SAPUserProfile : IAggregateRoot
        {
            public long SAPUserinfo_Id { get; set; }
            public long SAPProfileInfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }
        public class SAPUserParameter : IAggregateRoot
        {
            public int SAPUserInfo_Id { get; set; }
            public long SAPParameterInfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }
        public class SAPCompositeChildRoleMap : IAggregateRoot
        {
            public long SAPSingleRoleInfo_Id { get; set; }
            public long SAPCompositeRoleInfo_CompositeRole_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }
        public class SAPUserRole : IAggregateRoot
        {
            public long SAPRoleInfo_Id { get; set; }
            public long SAPUserinfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
            public bool? IsDirectlyAttached { get; set; }
            public string Role_FromDate { get; set; }
            public string Role_ToDate { get; set; }
        }
        public class SapSingleRoleTcodes : IAggregateRoot
        {
            public long SAPTCodeInfo_Id { get; set; }
            public long SAPSingleRoleInfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }

    
        #endregion

        private IACMDbOperator m_iacmOperator;

        QueryExecutor executor;
        private QueryExecutor m_executor;
        private IACMDBContext _context;

        #region ISAPConnectorDA Members

        public void AddSAPUserinfoBulk(IList<SAPUserinfo> SAPUserinfo)
        {
            SQLBulkUploader<SAPUserinfo> SAPUser = null;
            try
            {
                string tablename = "DI_SAP_UserInfo";
                SAPUser = new SQLBulkUploader<SAPUserinfo>(tablename);
                SAPUser.Flush(SAPUserinfo);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                SAPUser.Close();
            }
        }


        public IList<SAPParameterHistoryInfo> GetSAPParameters(EndPoint endPoint)
        {
            //string query = "DI_spGetAllSAPParametersByEndPointID";
            SAPParameterHistoryInfo _SAPParameter = null;
            IList<SAPParameterHistoryInfo> _list = new List<SAPParameterHistoryInfo>();

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var _list1 = (from saph in _context.SAPParameterInfos // SAPParameterHistoryInfos
                                  where saph.EndPointSystemId == endPoint.Id
                                  select saph).Distinct();

                    foreach (var tLst in _list1)
                    {
                        _SAPParameter = SAPParameterHistoryInfo.CreateSAPParameterHistoryInfo();

                        _SAPParameter.PARAM_CODE = CheckNull.AgainstString(tLst.PARAM_CODE);
                        _SAPParameter.PARAM_DESC = CheckNull.AgainstString(tLst.PARAM_DESC);
                        _SAPParameter.PARAM_VALUE = CheckNull.AgainstString(tLst.PARAM_VALUE);
                        _SAPParameter.Id = CheckNull.AgainstInt(tLst.Id); ;
                        _SAPParameter.TenantId = CheckNull.AgainstString(tLst.TenantId);
                        _list.Add(_SAPParameter);
                    }
                }
                return _list;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null,  LogLevelType.Info, "Error Occured while getting All SAP Parameters ", ex);
                throw ex;
            }
        }




        public IList<SAPGroupInfo> GetAllSAPGroupsByEndpointsystemid(long EndpointSystemId)
        {        
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from groups in _context.SAPGroupInfos
                            where groups.EndPointSystemId == EndpointSystemId
                            select groups).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IList<SAPParameterHistoryInfo> GetSAPParameters(EndPoint endPoint, string searchText)
        {
            //string query = "DI_spGetAllSAPParametersByEndPointID";
            SAPParameterHistoryInfo _SAPParameter = null;
            IList<SAPParameterHistoryInfo> _list = new List<SAPParameterHistoryInfo>();

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var _list1 = (from saph in _context.SAPParameterInfos // SAPParameterHistoryInfos
                                  where saph.EndPointSystemId == endPoint.Id && saph.PARAM_CODE.Contains(searchText)
                                  select saph).Distinct();

                    foreach (var tLst in _list1)
                    {
                        _SAPParameter = SAPParameterHistoryInfo.CreateSAPParameterHistoryInfo();

                        _SAPParameter.PARAM_CODE = CheckNull.AgainstString(tLst.PARAM_CODE);
                        _SAPParameter.PARAM_DESC = CheckNull.AgainstString(tLst.PARAM_DESC);
                        _SAPParameter.PARAM_VALUE = CheckNull.AgainstString(tLst.PARAM_VALUE);
                        _SAPParameter.Id = CheckNull.AgainstInt(tLst.Id); ;
                        _SAPParameter.TenantId = CheckNull.AgainstString(tLst.TenantId);
                        _list.Add(_SAPParameter);
                    }
                }
                return _list;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null,  LogLevelType.Info, "Error Occured while getting All SAP Parameters ", ex);
                throw ex;
            }
        }

        public IList<SAPRoleInfo> GetSAPRoles(EndPoint endPoint)
        {
            //string query = "DI_spGetAllSAPRolesByEndPointID";
            SAPRoleInfo _SAPRole = null;
            IList<SAPRoleInfo> _list = new List<SAPRoleInfo>();

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    // TODO: //ORDER BY ROLE_CODE COLLATE Latin1_General_BIN
                    var _list1 = (from sr in _context.SAPRoleInfos
                                  where sr.EndPointSystemId == endPoint.Id
                                  orderby sr.ROLE_CODE
                                  select new
                                      {
                                          SAP_ROLEID = sr.Id,
                                          sr.ROLE_DESC,
                                          sr.ROLE_CODE,
                                          sr.TenantId
                                      }).Distinct();

                    foreach (var tLst in _list1)
                    {
                        _SAPRole = new SAPRoleInfo();
                        _SAPRole.Id = CheckNull.AgainstInt(tLst.SAP_ROLEID);
                        _SAPRole.ROLE_CODE = CheckNull.AgainstString(tLst.ROLE_CODE);
                        _SAPRole.ROLE_DESC = CheckNull.AgainstString(tLst.ROLE_DESC);
                        _SAPRole.TenantId = CheckNull.AgainstString(tLst.TenantId);
                        _list.Add(_SAPRole);
                    }
                }
                return _list;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All SAP Roles ", ex);
                throw ex;
            }
        }

        public IList<SAPPosition> GetSAPPositions(EndPoint endPoint)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPPositions.Where(item => item.EndPointSystemId == endPoint.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All SAP positions ", ex);
                throw ex;
            }
        }

        public void AddSAPUserinfoHistoryBulk(IList<SAPUserHistoryInfo> SAPUserinfo)
        {
            SQLBulkUploader<SAPUserHistoryInfo> sqlUserHistory = null;
            try
            {
                string tableName = "DI_SAP_UserHistoryInfo";
                sqlUserHistory = new SQLBulkUploader<SAPUserHistoryInfo>(tableName);
                sqlUserHistory.Flush(SAPUserinfo);
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

        public void AddSAPParameterHistoryInfoBulk(IList<SAPParameterHistoryInfo> lstParameters)
        {
            SQLBulkUploader<SAPParameterHistoryInfo> sqlParameterHistory = null;
            try
            {
                string tableName = "DI_SAP_PARAMETERHISTORYINFO";
                sqlParameterHistory = new SQLBulkUploader<SAPParameterHistoryInfo>(tableName);
                sqlParameterHistory.Flush(lstParameters);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlParameterHistory.Close();
            }
        }

        public void AddSAPParameterInfoBulk(IList<SAPParameterInfo> listParameters)
        {
            SQLBulkUploader<SAPParameterInfo> SAPParameter = null;
            try
            {
                string tablename = "DI_SAP_ParameterInfo";
                SAPParameter = new SQLBulkUploader<SAPParameterInfo>(tablename);
                SAPParameter.Flush(listParameters);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                SAPParameter.Close();
            }
        }

        public void AddSAPProfileInfoBulk(IList<SAPProfileInfo> SAPProfileInfo)
        {
            SQLBulkUploader<SAPProfileInfo> SAPProfile = null;
            try
            {
                string tablename = "DI_SAP_ProfileInfo";
                SAPProfile = new SQLBulkUploader<SAPProfileInfo>(tablename);
                SAPProfile.Flush(SAPProfileInfo);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                SAPProfile.Close();
            }
        }

        public void AddSAPProfileInfoHistoryBulk(IList<SAPProfileHistoryInfo> SAPProfileInfo)
        {
            SQLBulkUploader<SAPProfileHistoryInfo> sqlUserHistory = null;
            try
            {
                string tableName = "DI_SAP_ProfileHistoryInfo";
                sqlUserHistory = new SQLBulkUploader<SAPProfileHistoryInfo>(tableName);
                sqlUserHistory.Flush(SAPProfileInfo);
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

        #endregion


        #region ISAPConnectorDA Members

        public void UpdateRolesAsSingleIfNull(long EndpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    string query = string.Format("Update DI_SAP_RoleInfo set RoleType = '{0}'  where EndpointSystemId={1} and RoleType IS Null", Constants.SINGLE, EndpointSystemId);
                    _context.Database.ExecuteSqlCommand(query);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveRoles(IList<SAPRoleInfo> list, long EndpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    SAPRoleInfo sapexistingRole = null;

                    foreach (SAPRoleInfo role in list)
                    {

                        sapexistingRole = _context.SAPRoleInfos.FirstOrDefault(t => t.ROLE_CODE == role.ROLE_CODE && t.EndPointSystemId == EndpointSystemId);
                        if (sapexistingRole == null)
                        {
                            _context.SAPRoleInfos.Add(role);
                            _context.SaveChanges();
                        }
                        else
                        {
                            sapexistingRole.ModifiedBy = role.ModifiedBy;
                            sapexistingRole.ModifiedDate = role.ModifiedDate;
                            sapexistingRole.ROLE_CODE = role.ROLE_CODE;
                            sapexistingRole.ROLE_DESC = role.ROLE_DESC;
                            sapexistingRole.Version = role.Version;
                            sapexistingRole.TargetSysId = role.TargetSysId;
                            sapexistingRole.RoleType = role.RoleType;
                            _context.SAPRoleInfos.Attach(sapexistingRole);
                            _context.Entry(sapexistingRole).State = EntityState.Modified;
                            _context.SaveChanges();

                        }
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateSAPUserInfoOnAttributeMap(DataTable dtSAPUser)
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


                selectCmd.CommandText = "SELECT UserId, UserLogonName, DIUserId FROM DI_SAP_UserInfo";
                updateCmd = conn.CreateCommand();
                updateCmd.CommandText = "UPDATE DI_SAP_UserInfo Set DIUserId=@DIUserId Where UserId=@UserId";
                updateCmd.CommandType = CommandType.Text;


                //selectCmd.CommandText = "DI_spGetAllSAPUserInfo";

                //updateCmd = conn.CreateCommand();
                //updateCmd.CommandText = "DI_spUpdateSAPUserInfo";
                //updateCmd.CommandType = CommandType.StoredProcedure;

                List<SqlParameter> parameters = null;
                parameters = GetUserParams();
                updateCmd.Parameters.AddRange(parameters.ToArray());

                updateCmd.UpdatedRowSource = UpdateRowSource.None;

                insertCommand = conn.CreateCommand();
                insertCommand.CommandText = "DI_spAddSAPUserInfo";
                insertCommand.CommandType = CommandType.StoredProcedure;

                List<SqlParameter> insertParams = GetUserParams();

                insertCommand.Parameters.AddRange(insertParams.ToArray());
                insertCommand.UpdatedRowSource = UpdateRowSource.None;

                deletecommand = conn.CreateCommand();
                deletecommand.CommandText = "Delete DI_SAP_UserInfo where UserId=@Id";
                deletecommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int, 4, "Id"));
                updateCmd.UpdatedRowSource = UpdateRowSource.None;

                da = new SqlDataAdapter(selectCmd);
                da.UpdateCommand = updateCmd;

                da.UpdateBatchSize = 100;

                da.AcceptChangesDuringUpdate = true;

                DataSet ds = new DataSet();
                da.Fill(ds, "SAPUserInfo");

                DataRow[] drowsMax = ds.Tables[0].Select("UserId = max(UserId)");

                if (drowsMax.Count() <= 0)
                    newRowCount = 0;
                else
                    newRowCount = Convert.ToInt32(drowsMax[0]["UserId"]);

                //DataTable dtNew = dtAdsUser;

                if (ds.Tables[0].Rows.Count <= 0)
                {

                    ds.Tables[0].Merge(dtSAPUser, false);
                    da.Update(ds, "SAPUserInfo");
                    return;
                }

                DataRow drow = null;

                try
                {
                    foreach (DataRow dr in dtSAPUser.Rows)
                    {
                        DataRow[] dRows = ds.Tables[0].Select("UserLogonName='" + dr["UserLogonName"].ToString().Replace("'", "''") + "'");




                        if (dRows.Count() <= 0)
                        {


                            drow = ds.Tables[0].NewRow();
                            drow["UserId"] = ++newRowCount;
                            drow["DIUserId"] = dr["DIUserId"];

                            ds.Tables[0].Rows.Add(drow);
                        }

                        else
                        {

                            foreach (DataRow dRowTemp in dRows)
                            {
                                //drow = dRows[0];

                                //drow["DIUserId"] = dr["DIUserId"];
                                dRowTemp["DIUserId"] = dr["DIUserId"];
                                strUpdate = strUpdate.AppendFormat(",{0}", dRowTemp["UserId"]);
                            }
                        }






                    }
                }
                catch (Exception)
                {
                }




                if (strUpdate.ToString() != string.Empty)
                    //  Log.WriteMessage(LogLevel.INFO, "Updated userId(s):" + strUpdate.ToString());


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
        private List<SqlParameter> GetUserParams()
        {
            SqlParameter sqlParam = null;
            List<SqlParameter> parameters = new List<SqlParameter>();
            SqlParameter sqlparam = new SqlParameter();
            sqlparam = new SqlParameter();
            sqlparam.ParameterName = "@UserId";
            sqlparam.SqlDbType = SqlDbType.Int;
            sqlparam.SourceColumn = "UserId";
            sqlparam.Value = 1;
            parameters.Add(sqlparam);

            sqlparam = new SqlParameter();
            sqlparam.ParameterName = "@DIUserId";
            sqlparam.SqlDbType = SqlDbType.Int;
            sqlparam.SourceColumn = "DIUserId";
            parameters.Add(sqlparam);



            return parameters;
        }
        #endregion


        #region Get SAP Configuration

        public SAPConfiguration GetConfiguration(long _systemid)
        {
            using (_context = new IACMDBContext())
            {
                SAPConfiguration config = null;
                config = _context.SapConfiguration.FirstOrDefault(s => s.EndPointSystemId == _systemid);
                config.Password = RSAHelper.DecryptData(config.Password);
                return config;
            }
        }

        public SAPConfiguration GetConfigurationByEndpointID(long _endpointid)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SapConfiguration.First(s => s.EndPointSystemId == _endpointid);
            }
        }

        #endregion

        private DataTable getUserInfoTable()
        {
            DataTable table = new DataTable();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(SAPUserinfo));

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

        #region SAPConnector Configuration



        public SAPConfiguration GetSAPConfiguration()
        {
            string query = "select EP.HostName as clientno,EP.DomainName as systemno,EP.ipaddress,ADS.UserName,ADS.Password from DI_M_EndpointSystem EP join DI_ADS_Configuration ADS on EP.EndPointSystemId = ADS.EndPointSystemId where EP.EndPointSystemTypeId = 4";
            SAPConfiguration SAPConfiguration = new SAPConfiguration();
            try
            {
                m_iacmOperator = new IACMDbOperator(query, CommandType.Text, null);

                executor = new QueryExecutor(m_iacmOperator);
                DataSet ds = executor.ExecuteQueryDS();
                if (ds == null)
                    return null;
                if (ds.Tables.Count == 0)
                    return null;
                DataTable dtSAPConfiguration = ds.Tables[0];
                foreach (DataRow dtrow in dtSAPConfiguration.Rows)
                {
                    SAPConfiguration = new SAPConfiguration();
                    SAPConfiguration.UserName = CheckNull.AgainstString(dtrow["UserName"]);
                    SAPConfiguration.Password = CheckNull.AgainstString(dtrow["Password"]);
                    //  SAPConfiguration.DomainName = CheckNull.AgainstString(dtrow["DomainName"]);
                    //  SAPConfiguration.HostName = CheckNull.AgainstString(dtrow["HostName"]);
                    SAPConfiguration.IPAddress = CheckNull.AgainstString(dtrow["ipaddress"]);
                    SAPConfiguration.ClientNumber = Convert.ToInt16(CheckNull.AgainstInt(dtrow["clientno"]));
                    SAPConfiguration.SystemNumber = Convert.ToInt16(CheckNull.AgainstInt(dtrow["systemno"]));
                    //   SAPConfiguration.BPopIACM = CheckNull.AgainstBit(dtrow["BPopIACM"]);
                    //  SAPConfiguration.BImpDefPriviledge = CheckNull.AgainstBit(dtrow["BImpDefPriviledge"]);
                    //  SAPConfiguration.CreatedBy = CheckNull.AgainstString(dtrow["CreatedBy"]);
                    //   SAPConfiguration.CreatedDate = CheckNull.AgainstDatetime(dtrow["CreatedDate"]);
                    //  SAPConfiguration.LastModifiedBy = CheckNull.AgainstString(dtrow["LastModifiedBy"]);
                    //   SAPConfiguration.LastModifiedDate = CheckNull.AgainstDatetime(dtrow["LastModifiedDate"]);

                }
                return SAPConfiguration;

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

        #region ISAPConnectorDA Members


        public void AddSAPGroupInfoBulk(IList<SAPGroupInfo> groups)
        {
            SQLBulkUploader<SAPGroupInfo> SAPGroup = null;
            try
            {
                string tablename = "DI_SAP_GroupInfo";
                SAPGroup = new SQLBulkUploader<SAPGroupInfo>(tablename);
                SAPGroup.Flush(groups);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                SAPGroup.Close();
            }
        }

        public void AddSAPGroupInfoHistoryBulk(IList<SAPGroupHistoryInfo> groups)
        {
            SQLBulkUploader<SAPGroupHistoryInfo> sqlGroupHistory = null;
            try
            {
                string tableName = "DI_SAP_GroupHistoryInfo";
                sqlGroupHistory = new SQLBulkUploader<SAPGroupHistoryInfo>(tableName);
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
                    case "SAPUserInfo":
                        strTableName = "DI_SAP_UserInfo";
                        break;
                    case "SAPParameterInfo":
                        strTableName = "DI_SAP_PARAMETERINFO";
                        break;
                    case "SAPGroupInfo":
                        strTableName = "DI_SAP_GroupInfo";
                        break;
                    case "SAPProfileInfo":
                        strTableName = "DI_SAP_ProfileInfo";
                        break;
                    case "SAPRoleInfo":
                        strTableName = "DI_SAP_RoleInfo";
                        break;
                    case "SAPDomainInfo":
                        strTableName = "DI_SAP_DomainInfo";
                        break;
                    case "SAPUserRoleMapInfo":
                        strTableName = "DI_SAP_USER_ROLE_MAP";
                        break;

                    case "SAPUserGroupMapInfo":
                        strTableName = "DI_SAP_USER_GROUP_MAP_INFO";
                        break;
                    case "SAPUserProfileMapInfo":
                        strTableName = "DI_SAP_USER_PROFILE_MAP_INFO";
                        break;

                    case "SAPUserRoleTCodeMapInfo":
                        strTableName = "DI_SAP_USER_ROLETCODE_MAP_INFO";
                        break;

                    case "SAPCompositeChildRoleMapInfo":
                        strTableName = "DI_SAP_COMPOSITE_CHILD_ROLEMAP_INFO";
                        break;
                    case "SAPUserParameterMapInfo":
                        strTableName = "DI_SAP_USER_PARAMETER_MAP";
                        break;



                    default:
                        break;
                }

                string strQuery = string.Format("DELETE {0}", strTableName);

                if (_tenantId != null && _endpointSystemId != null)
                {
                    strQuery = string.Format("{0} WHERE TenantId = '{1}' AND EndpointSystemId={2}", strQuery, _tenantId, _endpointSystemId);
                }

                m_iacmOperator = new IACMDbOperator(strQuery, CommandType.Text, null);

                m_executor = new QueryExecutor(m_iacmOperator);

                noAffected = m_executor.ExecuteQuery();

                if (noAffected <= 0)
                {
                }

                //Log.WriteMessage(LogLevel.DEBUG, string.Format("Deleted {0} records from table {1}",
                //                                                noAffected.ToString(),
                //                                                strTableName));



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



        public IList<SAPUserinfo> GetAllSAPUserInfo()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from sapUser in _context.SAPUserinfos
                            .Include("EndPointSystem")
                            .Include("EndPointSystem.EndPointSystemType")
                            select sapUser).ToList();

                }
            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Fetching All Users", ex);
                throw ex;
            }

        }

        public bool CheckSAPUserNameExists(string userName, long endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPUserinfos.Any(item => item.UserLogonName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)
                        && item.EndPointSystemId == endpointSystemId);
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<SAPUserinfo> GetAllSAPUserInfo(long EndpointSystemId, string FilterText)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var SapUsers = (from sapUser in _context.SAPUserinfos
                            .Include("EndPointSystem")
                            .Include("EndPointSystem.EndPointSystemType")
                                    where sapUser.EndPointSystemId == EndpointSystemId
                                    select sapUser
                            );

                    if (FilterText == "*")
                    {
                        return SapUsers.ToList();
                    }
                    return SapUsers.Where(sapUser => sapUser.UserLogonName.Contains(FilterText)).ToList();
                }
            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Fetching All Users", ex);
                throw ex;
            }

        }

        public IList<SAPRoleInfo> GetAllSapRoles(long EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from sapRole in _context.SAPRoleInfos
                            where sapRole.EndPointSystemId == EndpointSystemId
                            select sapRole).ToList();

                }
            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Fetching Sap Roles", ex);
                throw ex;
            }
        }

        public Dictionary<long, string> GetAllAssignedTCodes(long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return (from authObject in _context.SapRoleToAuthMap
                        where authObject.EndPointSystemId == EndpointSystemId && authObject.ObjectCode == "S_TCODE" && authObject.FieldCode == "TCD"
                        join tCodes in _context.SAPTCodeInfos on authObject.FromValue equals tCodes.TCODE_Code
                        select tCodes)
                        .Select(tCode => new { key = tCode.Id, value = tCode.TCODE_Code })
                        .Take(1000)
                        .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);

            }
        }

        public IList<SAPUserinfo> GetAllUnMappedSAPUsers()
        {
            IList<SAPUserinfo> listSAPs = new List<SAPUserinfo>();

            string query = "DI_spGetAllUnMappedSAPUserInfo";

            SAPUserinfo SAPUserInfo = null;
            try
            {
                m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure);

                m_executor = new QueryExecutor(m_iacmOperator);

                IDataReader reader = m_executor.ExecuteQueryDR();


                while (reader.Read())
                {
                    SAPUserInfo = SAPUserinfo.CreateSAPUserinfo();
                    SAPUserInfo.Id = CheckNull.AgainstInt(reader["UserId"]);
                    SAPUserInfo.Firstname = CheckNull.AgainstString(reader["FIRSTNAME"]);
                    SAPUserInfo.Lastname = CheckNull.AgainstString(reader["LASTNAME"]);
                    SAPUserInfo.Initials = CheckNull.AgainstString(reader["INITIALS"]);
                    SAPUserInfo.Department = CheckNull.AgainstString(reader["DEPARTMENT"]);
                    SAPUserInfo.E_Mail = CheckNull.AgainstString(reader["EMAIL"]);
                    SAPUserInfo.Pers_No = CheckNull.AgainstString(reader["PERSONNUM"]);
                    SAPUserInfo.Tel1_Numbr = CheckNull.AgainstString(reader["TELEPHONENUM"]);
                    SAPUserInfo.Function0 = CheckNull.AgainstString(reader["FUNCTION"]);
                    SAPUserInfo.Title = CheckNull.AgainstString(reader["TITLE"]);
                    SAPUserInfo.City = CheckNull.AgainstString(reader["CITY"]);
                    SAPUserInfo.Namcountry = CheckNull.AgainstString(reader["NAMEFORMATCOUNTRY"]);
                    SAPUserInfo.Postl_Cod1 = CheckNull.AgainstString(reader["CITYPOSTALCODE"]);
                    SAPUserInfo.Pboxcit_No = CheckNull.AgainstString(reader["POBOXCITYCODE"]);
                    SAPUserInfo.Street = CheckNull.AgainstString(reader["STREET"]);
                    SAPUserInfo.ActivityGroup = CheckNull.AgainstString(reader["ACTIVITYGROUPS"]);
                    SAPUserInfo.LastLoginTime = CheckNull.AgainstDatetime(reader["LASTLOGINTIME"]);
                    SAPUserInfo.UserLogonName = CheckNull.AgainstString(reader["USERLOGONNAME"]);
                    SAPUserInfo.AccntLocDurPwd = CheckNull.AgainstString(reader["ACCNTLOCKEDDURINGPWD"]);
                    SAPUserInfo.AccntLockNoPwd = CheckNull.AgainstString(reader["ACCNTLOCKEDNOPWD"]);
                    SAPUserInfo.LOCKEDBYINTRUDER = CheckNull.AgainstString(reader["LOCKEDBYINTRUDER"]);
                    SAPUserInfo.EMPLOYEEID = CheckNull.AgainstString(reader["EMPLOYEEID"]);
                    SAPUserInfo.EMPLOYEETYPE = CheckNull.AgainstString(reader["EMPLOYEETYPE"]);
                    SAPUserInfo.PASSWORDEXPTIME = CheckNull.AgainstDatetime(reader["PASSWORDEXPTIME"]);
                    SAPUserInfo.LOGINDISABLED = CheckNull.AgainstString(reader["LOGINDISABLED"]);
                    SAPUserInfo.IsActive = CheckNull.AgainstBit(reader["ISACTIVE"]);
                    SAPUserInfo.CreatedDate = CheckNull.AgainstDatetime(reader["CreatedDate"]);
                    SAPUserInfo.LastModifiedDate = CheckNull.AgainstDatetime(reader["LastModifiedDate"]);


                    listSAPs.Add(SAPUserInfo);
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                m_iacmOperator.Close();
            }

            return listSAPs;
        }

        public SAPPosition GetSAPPositionByPositionCode(string code, long endpointSystemId)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                return _context.SAPPositions.FirstOrDefault(item => item.Code == code && item.EndPointSystemId == endpointSystemId);
            }
        }

        public IList<UserAccount> GetAccountDetail(int UserID)
        {
            IList<UserAccount> Listtobereturn = new List<UserAccount>();
            IList<SAPUserinfo> listSAPUserInfo = new List<SAPUserinfo>();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                listSAPUserInfo = oIACMDBContext.SAPUserinfos.Where(sapusr => sapusr.DIUserId == UserID && sapusr.IsLocked == false).ToList();
                foreach (SAPUserinfo sapusr in listSAPUserInfo)
                {
                    if (sapusr != null)
                    {
                        UserAccount UserAccount = new UserAccount
                        {
                            EndpointUserId = sapusr.Id,
                            UserName = sapusr.AccountName,
                            FullName = sapusr.Firstname + " " + sapusr.Lastname,
                            AccountType = sapusr.AccountType,
                            LastLogonDate = sapusr.LastLoginTime,
                            EndpointSystemId = sapusr.EndPointSystemId,
                            EndPointSystemType = sapusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = sapusr.EndPointSystem.DisplayName
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
            IList<SAPUserinfo> listSAPUserInfo = new List<SAPUserinfo>();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                listSAPUserInfo = oIACMDBContext.SAPUserinfos.Include(lq => lq.EndPointSystem).Include(lq => lq.EndPointSystem.EndPointSystemType).Where(sapusr => sapusr.EndPointSystemId == ApplicationID && sapusr.IsLocked == false).ToList();
                foreach (SAPUserinfo sapusr in listSAPUserInfo)
                {
                    if (sapusr != null)
                    {
                        UserAccount UserAccount = new UserAccount
                        {
                            EndpointUserId = sapusr.Id,
                            UserName = sapusr.AccountName,
                            FullName = sapusr.Firstname + " " + sapusr.Lastname,
                            AccountType = sapusr.AccountType,
                            LastLogonDate = sapusr.LastLoginTime,
                            EndpointSystemId = sapusr.EndPointSystemId,
                            EndPointSystemType = sapusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                            EndpointSystemName = sapusr.EndPointSystem.DisplayName
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
            SAPUserinfo SAPUserinfo = new SAPUserinfo();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                SAPUserinfo = oIACMDBContext.SAPUserinfos.Include(lq => lq.EndPointSystem).Include(lq => lq.EndPointSystem.EndPointSystemType).Include(lq => lq.SAPGroupInfo).Include(lq => lq.SAPProfileInfos).Include(lq => lq.SAPRoleInfo).Where(sapusr => sapusr.Id == EndpointUserId).FirstOrDefault();
                if (SAPUserinfo == null)
                    return Listtobereturn;

                foreach (SAPGroupInfo grp in SAPUserinfo.SAPGroupInfo)
                {
                    UserEntitlement UserEntitlement = new UserEntitlement();
                    UserEntitlement.EndpointUserId = SAPUserinfo.Id;
                    UserEntitlement.UserName = SAPUserinfo.AccountName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Group);
                    UserEntitlement.EndPointSystemType = SAPUserinfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = SAPUserinfo.EndPointSystem.DisplayName;
                    if (grp != null)
                    {
                        UserEntitlement.Name = grp.GROUP_CODE;
                        UserEntitlement.Description = grp.GROUP_DESC;
                        Listtobereturn.Add(UserEntitlement);
                    }
                }
                foreach (SAPProfileInfo profile in SAPUserinfo.SAPProfileInfos)
                {
                    UserEntitlement UserEntitlement = new UserEntitlement();
                    UserEntitlement.EndpointUserId = SAPUserinfo.Id;
                    UserEntitlement.UserName = SAPUserinfo.AccountName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Profile);
                    UserEntitlement.EndPointSystemType = SAPUserinfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = SAPUserinfo.EndPointSystem.DisplayName;
                    if (profile != null)
                    {
                        UserEntitlement.Name = profile.PROFILE_CODE;
                        UserEntitlement.Description = profile.PROFILE_DESC;
                        Listtobereturn.Add(UserEntitlement);
                    }
                }
                foreach (SAPRoleInfo role in SAPUserinfo.SAPRoleInfo)
                {
                    UserEntitlement UserEntitlement = new UserEntitlement();
                    UserEntitlement.EndpointUserId = SAPUserinfo.Id;
                    UserEntitlement.UserName = SAPUserinfo.AccountName;
                    UserEntitlement.Category = Convert.ToString(EntitlementType.Role);
                    UserEntitlement.EndPointSystemType = SAPUserinfo.EndPointSystem.EndPointSystemType.EndpointSystemTypeName;
                    UserEntitlement.EndpointSystemName = SAPUserinfo.EndPointSystem.DisplayName;
                    UserEntitlement.EndpointSystemId = SAPUserinfo.EndPointSystemId;
                    if (role != null)
                    {
                        UserEntitlement.Name = role.ROLE_CODE;
                        UserEntitlement.Description = role.ROLE_DESC;
                        Listtobereturn.Add(UserEntitlement);
                    }
                }
            }
            return Listtobereturn;
        }

        public AccountDetail GetAccountDetailByEndPointUserID(int EndpointUserId)
        {
            AccountDetail AccountDetail = new AccountDetail();
            SAPUserinfo SAPUserinfo = new SAPUserinfo();
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                SAPUserinfo = oIACMDBContext.SAPUserinfos.Where(sapusr => sapusr.Id == EndpointUserId).FirstOrDefault();
                if (SAPUserinfo == null)
                    return AccountDetail;
                AccountDetail.LoginName = SAPUserinfo.AccountName;
                AccountDetail.DisplayName = SAPUserinfo.Firstname + " " + SAPUserinfo.Lastname;
                AccountDetail.PhoneNo = SAPUserinfo.Pers_No;
                AccountDetail.Email = SAPUserinfo.E_Mail;
                AccountDetail.AccountType = SAPUserinfo.AccountType;
                AccountDetail.IsMappedAccount = SAPUserinfo.DIUserId == null ? false : true;
                AccountDetail.LastLogonDate = SAPUserinfo.LastLoginTime;
                AccountDetail.Version = Convert.ToString(SAPUserinfo.Version);
                AccountDetail.DIUserID = SAPUserinfo.DIUserId;
                AccountDetail.EndpointSystemId = SAPUserinfo.EndPointSystemId;
                AccountDetail.EndpointUserId = EndpointUserId;
            }
            return AccountDetail;
        }
        public void GetAllMasterUsersSAP(IList<SAPUserinfo> listsapUser, bool mapAll)
        {
            try
            {
                DataRow[] dRowsMaster = null;

                DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Started processing Master user(s) objects");
                IList<User> users = new List<User>();
                IList<ConfigGeneralTrustedSource> listTrustedConfig = null;
                IList<ConfigGeneralTrustedSource> list = null; //new DiConfigurationDA().GetAllConfigTrustedSource();


                listTrustedConfig = list.Where(li => li.TrustedSource == EndpointType.SAP).ToList();


                if (listTrustedConfig.Count <= 0)
                {
                    DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Populate User: No attribute is configured from SAP");
                    return;
                }


                string tenantId = string.Empty;
                if (listsapUser.Count > 0)
                {
                    tenantId = listsapUser.First().TenantId;
                }

                IList<User> listUsers = new List<User>();

                if (tenantId != string.Empty)
                    listUsers = new UserDA().GetAllUsers(tenantId);
                else
                    listUsers = new UserDA().GetAllUsers();

                foreach (SAPUserinfo sapuser in listsapUser)
                {

                    IList<User> existingMasteruser = (from usr in listUsers where usr.Id == sapuser.DIUserId select usr).ToList();
                    IList<SAPUserinfo> sapusr = (from sapu in listsapUser where sapu.UserLogonName == sapuser.UserLogonName select sapu).ToList();


                    if (existingMasteruser != null && existingMasteruser.Count() > 0)
                    {



                        DataTable dtMaster1 = CollectionHelper.ConvertToNormalTable(existingMasteruser);
                        DataTable dtSapUser1 = CollectionHelper.ConvertToNormalTable(sapusr);


                        foreach (DataRow adsRow in dtSapUser1.Rows)
                        {

                            dRowsMaster = dtMaster1.Select(string.Format("UserId={0}", sapuser.DIUserId));

                            if (dRowsMaster.Count() > 0) //modify existing user
                            {

                                foreach (DataRow dRowM in dRowsMaster)
                                {

                                    foreach (ConfigGeneralTrustedSource trustedSource in listTrustedConfig)
                                    {
                                        if (string.IsNullOrEmpty(dRowM[trustedSource.AttributeName].ToString()))
                                            dRowM[trustedSource.AttributeName] = adsRow[trustedSource.TrustedSourceAttribute];

                                        else if (!trustedSource.SelfUpdate)
                                            dRowM[trustedSource.AttributeName] = adsRow[trustedSource.TrustedSourceAttribute];
                                    }

                                    dRowM["LastModifiedBy"] = "Scheduler";
                                    dRowM["LastModifiedDate"] = DateTime.Now;

                                }



                            }
                        }


                        IList<User> masterusr = CollectionHelper.ConvertTo<User>(dtMaster1);

                        using (IACMDBContext oIACMDBContext = new IACMDBContext())
                        {

                            var original = oIACMDBContext.Users.Find(masterusr.FirstOrDefault().Id);

                            if (original != null)
                            {
                                masterusr.FirstOrDefault().CreatedDate = original.CreatedDate;
                                oIACMDBContext.Entry(original).CurrentValues.SetValues(masterusr.FirstOrDefault());
                                oIACMDBContext.SaveChanges();
                            }

                        }

                    }

                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        #region BULK INSERT OLD

        public void AddSAPProfileHistoryInfoBulk(IList<SAPProfileHistoryInfo> list)
        {
            SQLBulkUploader<SAPProfileHistoryInfo> sqlGroupHistory = null;
            try
            {
                string tableName = "DI_SAP_PROFILEHISTORYINFO";
                sqlGroupHistory = new SQLBulkUploader<SAPProfileHistoryInfo>(tableName);
                sqlGroupHistory.Flush(list);
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
        public void AddSAPRoleHistoryInfoBulk(IList<SAPRoleHistoryInfo> lstRoles)
        {
            SQLBulkUploader<SAPRoleHistoryInfo> sqlRoleHistory = null;
            try
            {
                string tableName = "DI_SAP_ROLEHISTORYINFO";
                sqlRoleHistory = new SQLBulkUploader<SAPRoleHistoryInfo>(tableName);
                sqlRoleHistory.Flush(lstRoles);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlRoleHistory.Close();
            }
        }
        public void AddSAPRoleInfoBulk(IList<SAPRoleInfo> list)
        {
            SQLBulkUploader<SAPRoleInfo> SAPRoles = null;
            try
            {
                string tablename = "DI_SAP_ROLEINFO";
                SAPRoles = new SQLBulkUploader<SAPRoleInfo>(tablename);
                SAPRoles.Flush(list);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                SAPRoles.Close();
            }
        }


        #endregion

        #region UserListing Methods
        public IList<SAPUserinfo> GetSAPUserInfoByDIUserId(long DIUserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from sapUser in _context.SAPUserinfos
                                   .Include("EndPointSystem")
                                   .Include("EndPointSystem.EndPointSystemType")
                            where sapUser.DIUserId == DIUserId && sapUser.IsLocked == false
                            orderby sapUser.CreatedDate
                            select sapUser).ToList();

                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting SAPUser By DIUserId ", ex);
                throw ex;
            }
        }

        public IList<SAPUserinfo> GetSAPUserInfoByDIUserId(long DIUserId, AccountType AccountType)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from sapUser in _context.SAPUserinfos
                                   .Include("EndPointSystem")
                                   .Include("EndPointSystem.EndPointSystemType")
                            where sapUser.DIUserId == DIUserId && sapUser.IsLocked == false && sapUser.AccountType == AccountType
                            orderby sapUser.CreatedDate
                            select sapUser).ToList();

                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting SAPUser By DIUserId ", ex);
                throw ex;
            }
        }

        public List<SAPUserinfo> GetPrimarySAPUserByDIUserId(long DIUserId, long endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPUserinfos.Where(item => item.DIUserId == DIUserId && item.AccountType == AccountType.User
                                && item.EndPointSystemId == endpointSystemId && !item.IsLocked).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Error Occured while getting All sap DIUserId={0}", DIUserId), ex);
                throw ex;
            }
        }

        public List<SAPUserinfo> GetActivePrimarySAPUserByDIUserId(int DIUserId, long endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPUserinfos.Where(item => item.DIUserId == DIUserId && item.AccountType == AccountType.User
                                && item.EndPointSystemId == endpointSystemId && item.IsActive && !item.IsLocked).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Error Occured while getting All sap DIUserId={0}", DIUserId), ex);
                throw ex;
            }
        }

        public SAPUserinfo GetLockedSAPUserByDIUserId(long DIUserId, long endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPUserinfos.FirstOrDefault(item => item.DIUserId == DIUserId && item.EndPointSystemId == endpointSystemId
                            && item.AccountType == AccountType.User && item.IsLocked);
                }
            }
            catch
            {
                throw;
            }
        }

        public List<SAPUserinfo> GetLockedSAPUsersByDIUserId(int DIUserId, long endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPUserinfos.Where(item => item.DIUserId == DIUserId && item.EndPointSystemId == endpointSystemId
                            && item.AccountType == AccountType.User && item.IsLocked).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public SAPUserinfo GetSAPUserByUserId(long userId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPUserinfos.Include(lq => lq.SAPRoleInfo).FirstOrDefault(item => item.Id == userId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SAPUserinfo GetSAPUserByUserName(string userName, long endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPUserinfos.FirstOrDefault(item => item.UserLogonName == userName && item.EndPointSystemId == endpointSystemId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SAPUserinfo GetSAPUserInfoByDIUserIdAndEndpoint(int diUserId, long endpointId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from sapUser in _context.SAPUserinfos
                                   .Include("EndPointSystem")
                                   .Include("EndPointSystem.EndPointSystemType")
                            where sapUser.DIUserId == diUserId && sapUser.EndPointSystemId == endpointId
                            orderby sapUser.CreatedDate
                            select sapUser).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting SAPUser By DIUserId ", ex);
                throw ex;
            }
        }

        //public List<SAPUserRoleMapInfo> GetAllSapUsersByRole(string rolename)
        //{
        //    return _context.SapUserRoleMapInfo.Where(s => s.SAP_ROLE_CODE == rolename).ToList();

        //}

        //public List<SAPUserRoleMapInfo> GetAllSapUsersByTCode(string TCode)
        //{
        //    List<SAPUserRoleTCodeMapInfo> RoleTCode = _context.SAPUserRoleTCodeMap.Where(s => s.TCode == TCode).ToList();

        //    List<SAPUserRoleMapInfo> UsrsTCode = new List<SAPUserRoleMapInfo>();

        //    foreach (SAPUserRoleTCodeMapInfo role in RoleTCode)
        //    {
        //        List<SAPUserRoleMapInfo> usrsrole = GetAllSapUsersByRole(role.Role_Code);
        //        UsrsTCode = UsrsTCode.Union(usrsrole).ToList();
        //    }

        //    return UsrsTCode;

        //}

        //public void GetSAPUserGroupRoleProfile(int UserId, out IList<SAPGroupInfo> groupList, out IList<SAPRoleInfo> roleList, out IList<SAPProfileInfo> profileList, out IList<SAPCompositeChildRoleMapInfo> singleRoleList)
        //{
        //    string query = "DI_spGetSAPUserGRPByDIUserId";
        //    string tmpValue;
        //    string tmpType;

        //    List<SqlParameter> parameters = new List<SqlParameter>();

        //    try
        //    {
        //        groupList = new List<SAPGroupInfo>();
        //        roleList = new List<SAPRoleInfo>();
        //        profileList = new List<SAPProfileInfo>();
        //        singleRoleList = new List<SAPCompositeChildRoleMapInfo>();

        //        SqlParameter sqlParam = new SqlParameter();
        //        sqlParam.ParameterName = "@UserId";
        //        sqlParam.SqlDbType = SqlDbType.Int;
        //        sqlParam.Value = UserId;
        //        parameters.Add(sqlParam);

        //        m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);
        //        m_executor = new QueryExecutor(m_iacmOperator);
        //        IDataReader reader = m_executor.ExecuteQueryDR();

        //        SAPGroupInfo grpinfo;
        //        SAPRoleInfo roleinfo;
        //        SAPProfileInfo Profileinfo;
        //        SAPCompositeChildRoleMapInfo SingleRoleInfo;
        //        int endPtSystemId;

        //        while (reader.Read())
        //        {
        //            tmpValue = CheckNull.AgainstString(reader["GrpupRoleProfileName"]);
        //            tmpType = CheckNull.AgainstString(reader["GRPType"]);
        //            endPtSystemId = CheckNull.AgainstInt(reader["EndPointSystemId"]);

        //            switch (tmpType)
        //            {
        //                case "Group":
        //                    grpinfo = new SAPGroupInfo();
        //                    grpinfo.GROUP_CODE = tmpValue;
        //                    grpinfo.GROUP_DESC = CheckNull.AgainstString(reader["GroupDesc"]);
        //                    grpinfo.EndPointSystemId = endPtSystemId;
        //                    groupList.Add(grpinfo);
        //                    break;
        //                case "Role":
        //                    roleinfo = new SAPRoleInfo();
        //                    roleinfo.ROLE_CODE = tmpValue;
        //                    roleinfo.ROLE_DESC = CheckNull.AgainstString(reader["GroupDesc"]);
        //                    roleinfo.EndPointSystemId = endPtSystemId;
        //                    roleList.Add(roleinfo);
        //                    break;
        //                case "Profile":
        //                    Profileinfo = new SAPProfileInfo();
        //                    Profileinfo.PROFILE_CODE = tmpValue;
        //                    Profileinfo.PROFILE_DESC = CheckNull.AgainstString(reader["GroupDesc"]);
        //                    Profileinfo.EndPointSystemId = endPtSystemId;
        //                    profileList.Add(Profileinfo); ;
        //                    break;

        //                case "SingleRole":
        //                    SingleRoleInfo = new SAPCompositeChildRoleMapInfo();
        //                    SingleRoleInfo.CHILD_ROLE_CODE = tmpValue;
        //                    SingleRoleInfo.CHILD_ROLE_CODE_DESCRIPTION = CheckNull.AgainstString(reader["GroupDesc"]);
        //                    SingleRoleInfo.EndPointSystemId = endPtSystemId;
        //                    singleRoleList.Add(SingleRoleInfo);
        //                    break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting SAPUser By DIUserId ", ex);
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (m_executor != null)
        //            m_executor.IACMDbOperator.Close();
        //    }
        //}

        public void GetSAPUserGroupRoleProfile(int UserId, out IList<SAPGroupInfo> groupList, out IList<SAPCompositeRoleInfo> sapCompRoleList, out IList<SAPProfileInfo> profileList, out IList<SAPSingleRoleInfo> sapsingleRoleList, out IList<SAPRoleInfo> sapRoleList)
        {
            //string query = "DI_spGetSAPUserGRPByDIUserId";
            string tmpValue, tmpType;

            try
            {
                groupList = new List<SAPGroupInfo>();
                sapCompRoleList = new List<SAPCompositeRoleInfo>();
                profileList = new List<SAPProfileInfo>();
                sapsingleRoleList = new List<SAPSingleRoleInfo>();
                sapRoleList = new List<SAPRoleInfo>();
                IList<SAPSingleRoleInfo> tempSapsingleRoleList = new List<SAPSingleRoleInfo>();

                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    SAPUserinfo aSAPUser = oIACMDBContext.SAPUserinfos.FirstOrDefault(usr => usr.Id == UserId);
                    if (aSAPUser != null)
                    {
                        //---Group infos
                        groupList = aSAPUser.SAPGroupInfo.ToList();

                        //---Composite role info
                        //    sapCompRoleList = aSAPUser.SAPCompositeRoleInfos.ToList();



                        sapRoleList = aSAPUser.SAPRoleInfo.ToList();

                        foreach (SAPRoleInfo SapRole in sapRoleList)
                        {
                            if (SapRole.RoleType == "SINGLE")
                            {
                                SAPSingleRoleInfo SingleRole = oIACMDBContext.SAPSingleRoleInfos.FirstOrDefault(s => s.SingleRole_Code == SapRole.ROLE_CODE);

                                if (SingleRole != null)
                                    sapsingleRoleList.Add(SingleRole);
                            }
                            else
                            {
                                SAPCompositeRoleInfo CompositeRole = oIACMDBContext.SAPCompositeRoleInfos.FirstOrDefault(s => s.CompositeRole_Code == SapRole.ROLE_CODE);
                                if (CompositeRole != null)
                                    sapCompRoleList.Add(CompositeRole);
                            }

                        }

                        // Commented out to implement composite and single role
                        ////---singlerole  info from composite info
                        //foreach (SAPCompositeRoleInfo compRoleItem in sapCompRoleList)
                        //{
                        //    tempSapsingleRoleList = compRoleItem.SAPSingleRoleInfos.ToList();
                        //    foreach (SAPSingleRoleInfo singleRoleItem in tempSapsingleRoleList)
                        //    {
                        //        if (sapsingleRoleList.Contains(singleRoleItem) == false)
                        //            sapsingleRoleList.Add(singleRoleItem);
                        //    }
                        //}

                        ////---single role info from user
                        ////tempSapsingleRoleList = aSAPUser.SAPSingleRoleInfos.ToList();
                        ////foreach (SAPSingleRoleInfo singleRoleItem in tempSapsingleRoleList)
                        ////{
                        ////    if (sapsingleRoleList.Contains(singleRoleItem) == false)
                        ////        sapsingleRoleList.Add(singleRoleItem);
                        ////}

                        //---Profile
                        //List<SapUserProfile> lstUserProfiles = oIACMDBContext.SapUserProfile.Where(user => user.UserName == aSAPUser.UserLogonName).ToList();
                        //foreach (SapUserProfile UserProfile in lstUserProfiles)
                        //{
                        //    SAPProfileInfo SapUserProfile = oIACMDBContext.SAPProfileInfos.FirstOrDefault(profile => profile.PROFILE_CODE == UserProfile.ProfileName);
                        //    if (SapUserProfile != null)
                        //        profileList.Add(SapUserProfile);
                        //}

                        profileList = aSAPUser.SAPProfileInfos.ToList();

                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting SAPUser By DIUserId ", ex);
                throw ex;
            }
        }

        public IList<SAPTCodeInfo> GetTcodeByUserId(int sapUserId)
        {
            IList<SAPTCodeInfo> tempTCodeList = new List<SAPTCodeInfo>();
            IList<SAPTCodeInfo> retunTCodeList = new List<SAPTCodeInfo>();
            IList<SAPSingleRoleInfo> tempSapSingleRoleList = new List<SAPSingleRoleInfo>();
            IList<SAPSingleRoleInfo> sapSingleRoleList = new List<SAPSingleRoleInfo>();
            IList<SAPCompositeRoleInfo> sapCompRoleList = new List<SAPCompositeRoleInfo>();

            List<SAPTCodeInfo> lstSapTCodeInfo = new List<SAPTCodeInfo>();


            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                SAPUserinfo aSAPUser = oIACMDBContext.SAPUserinfos.FirstOrDefault(usr => usr.Id == sapUserId);
                if (aSAPUser != null)
                {
                    //sapCompRoleList = aSAPUser.SAPCompositeRoleInfos.ToList();

                    List<SAPRoleInfo> lstSapRoles = aSAPUser.SAPRoleInfo.ToList();

                    foreach (SAPRoleInfo RoleInfo in lstSapRoles)
                    {
                        List<SAPRoleToAuthMap> lstRoleAuth = oIACMDBContext.SapRoleToAuthMap.Where(Role => Role.RoleName == RoleInfo.ROLE_CODE && Role.ObjectCode == "S_TCODE" && Role.FieldCode == "TCD").ToList();

                        foreach (SAPRoleToAuthMap RoleAuth in lstRoleAuth)
                        {
                            SAPTCodeInfo TCodeInfo = oIACMDBContext.SAPTCodeInfos.FirstOrDefault(tcode => tcode.TCODE_Code == RoleAuth.FromValue);
                            if (TCodeInfo != null)
                                lstSapTCodeInfo.Add(TCodeInfo);
                        }

                    }



                }
            }

            return lstSapTCodeInfo;

        }

        public DataSet GetAccessByUserId(int sapUserId)
        {
            DataSet ds = null;


            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                SAPUserinfo aSAPUser = oIACMDBContext.SAPUserinfos.FirstOrDefault(usr => usr.Id == sapUserId);
                if (aSAPUser != null)
                {
                    try
                    {
                        string query = "DI_spGetUserAccess";

                        SqlParameter sqlParamUserLogon = new SqlParameter();
                        sqlParamUserLogon.ParameterName = "@userid";
                        sqlParamUserLogon.SqlDbType = SqlDbType.Int;
                        sqlParamUserLogon.Value = sapUserId;

                        SqlParameter sqlParamEndpoinSystemId = new SqlParameter();
                        sqlParamEndpoinSystemId.ParameterName = "@EndpointSystemId";
                        sqlParamEndpoinSystemId.SqlDbType = SqlDbType.Int;
                        sqlParamEndpoinSystemId.Value = aSAPUser.EndPointSystemId;

                        List<SqlParameter> parameters = new List<SqlParameter>();

                        parameters.Add(sqlParamUserLogon);
                        parameters.Add(sqlParamEndpoinSystemId);

                        m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                        m_executor = new QueryExecutor(m_iacmOperator);
                        ds = m_executor.ExecuteQueryDS();
                    }
                    catch (Exception)
                    {
                        throw;
                    }



                }
            }
            return ds;
        }

        public DataSet GetObjectsByUserId(int sapUserId)
        {
            DataSet ds = null;


            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                SAPUserinfo aSAPUser = oIACMDBContext.SAPUserinfos.FirstOrDefault(usr => usr.Id == sapUserId);
                if (aSAPUser != null)
                {
                    try
                    {
                        string query = "DI_spGetSapObjectFieldData";

                        SqlParameter sqlParamUserLogon = new SqlParameter();
                        sqlParamUserLogon.ParameterName = "@UserLogonName";
                        sqlParamUserLogon.SqlDbType = SqlDbType.NVarChar;
                        sqlParamUserLogon.Value = aSAPUser.UserLogonName;

                        SqlParameter sqlParamEndpoinSystemId = new SqlParameter();
                        sqlParamEndpoinSystemId.ParameterName = "@EndpointSystemId";
                        sqlParamEndpoinSystemId.SqlDbType = SqlDbType.NVarChar;
                        sqlParamEndpoinSystemId.Value = aSAPUser.EndPointSystemId;

                        List<SqlParameter> parameters = new List<SqlParameter>();

                        parameters.Add(sqlParamUserLogon);
                        parameters.Add(sqlParamEndpoinSystemId);

                        m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                        m_executor = new QueryExecutor(m_iacmOperator);
                        ds = m_executor.ExecuteQueryDS();
                    }
                    catch (Exception)
                    {
                        throw;
                    }



                }
            }
            return ds;


        }

        public DataSet GetObjectsByTCode(string TCode)
        {
            DataSet ds = null;


            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                try
                {
                    string query = "DI_spGetTCodeObjectFieldData";

                    SqlParameter sqlParam = new SqlParameter();
                    sqlParam.ParameterName = "@TCode";
                    sqlParam.SqlDbType = SqlDbType.NVarChar;
                    sqlParam.Value = TCode;

                    List<SqlParameter> parameters = new List<SqlParameter>();

                    parameters.Add(sqlParam);

                    m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                    m_executor = new QueryExecutor(m_iacmOperator);
                    ds = m_executor.ExecuteQueryDS();



                }
                catch (Exception)
                {
                    throw;
                }




            }
            return ds;


        }


        public IList<SAPRoleTCode> GetTcodeAndRoleByUserId(int sapUserId, long EndpointSystemId)
        {
            IList<SAPRoleTCode> roletcodelist = new List<SAPRoleTCode>();
            SAPRoleTCode roletcode = new SAPRoleTCode();

            IList<SAPTCodeInfo> tempTCodeList = new List<SAPTCodeInfo>();
            IList<SAPTCodeInfo> retunTCodeList = new List<SAPTCodeInfo>();
            IList<SAPSingleRoleInfo> tempSapSingleRoleList = new List<SAPSingleRoleInfo>();
            IList<SAPSingleRoleInfo> sapSingleRoleList = new List<SAPSingleRoleInfo>();
            IList<SAPCompositeRoleInfo> sapCompRoleList = new List<SAPCompositeRoleInfo>();

            IList<string[]> lstSapTCodeInfo = new List<string[]>();
            string res = "";
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                SAPUserinfo aSAPUser = oIACMDBContext.SAPUserinfos.FirstOrDefault(usr => usr.Id == sapUserId && usr.EndPointSystemId == EndpointSystemId);
                if (aSAPUser != null)
                {
                    //sapCompRoleList = aSAPUser.SAPCompositeRoleInfos.ToList();

                    List<SAPRoleInfo> lstSapRoles = aSAPUser.SAPRoleInfo.ToList();

                    foreach (SAPRoleInfo RoleInfo in lstSapRoles)
                    {
                        List<SAPRoleToAuthMap> lstRoleAuth = oIACMDBContext.SapRoleToAuthMap.Where(Role => Role.RoleName == RoleInfo.ROLE_CODE && Role.ObjectCode == "S_TCODE" && Role.FieldCode == "TCD").ToList();

                        foreach (SAPRoleToAuthMap RoleAuth in lstRoleAuth)
                        {
                            roletcode = new SAPRoleTCode();
                            ////SAPTCodeInfo TCodeInfo = oIACMDBContext.SAPTCodeInfos.FirstOrDefault(tcode => tcode.TCODE_Code == RoleAuth.FromValue);
                            ////if (TCodeInfo != null)
                            // res += string.Format("{0} (Role: {1})\r\n", RoleAuth.FromValue, RoleAuth.RoleName);
                            ////lstSapTCodeInfo.Add(string.Format("{0} (Role: {1})", RoleAuth.FromValue, RoleAuth.RoleName));
                            // string[] tcode = { RoleAuth.RoleName, RoleAuth.FromValue};
                            // lstSapTCodeInfo.Add(tcode);
                            roletcode.Role = RoleAuth.RoleName;
                            roletcode.TCode = RoleAuth.FromValue;
                            roletcodelist.Add(roletcode);
                        }

                    }



                }
            }

            //lstSapTCodeInfo.Sort();
            //return lstSapTCodeInfo;
            return roletcodelist;

        }

        public List<SAPGroupInfo> GetSAPGroupsByUserId(long UserId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.SAPUserinfos.FirstOrDefault(usr => usr.Id == UserId).SAPGroupInfo.ToList();
                }
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
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.SAPUserinfos.FirstOrDefault(usr => usr.Id == UserId).SAPProfileInfos.ToList();
                }
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
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.SAPUserinfos.FirstOrDefault(usr => usr.Id == UserId).SAPRoleInfo.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<SAPRoleInfo> GetSAPRolesByDIUserId(long DIUserId)
        {
            List<SAPRoleInfo> sapgroupsList = new List<SAPRoleInfo>();
            try
            {

                SAPUserinfo sapuser = null;
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {

                    sapuser = oIACMDBContext.SAPUserinfos.Include(lq => lq.SAPRoleInfo).FirstOrDefault(usr => usr.DIUserId == DIUserId);
                    if (sapuser != null)
                    {
                        sapgroupsList = sapuser.SAPRoleInfo.ToList();
                    }
                }
            }
            catch
            {
                throw;
            }
            return sapgroupsList;
        }

        public List<SAPRoleInfo> GetSAPRolesByDIUserId(long DIUserId, AccountType AccountType)
        {
            List<SAPRoleInfo> sapgroupsList = new List<SAPRoleInfo>();
            try
            {

                SAPUserinfo sapuser = null;
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {

                    sapuser = oIACMDBContext.SAPUserinfos.Include(lq => lq.SAPRoleInfo).FirstOrDefault(usr => usr.DIUserId == DIUserId && usr.AccountType == AccountType);
                    if (sapuser != null)
                    {
                        sapgroupsList = sapuser.SAPRoleInfo.ToList();
                    }
                }
            }
            catch
            {
                throw;
            }
            return sapgroupsList;
        }

        #endregion

        #region GET ALL UNMAPPED USERS
        public IList<SAPUserinfo> GetAllUnMappedSAPUsers(EndPoint EndPoint)
        {
            IList<SAPUserinfo> listSAP = new List<SAPUserinfo>();

            string query = "DI_spGetAllUnMappedSAPUserinfo";

            SAPUserinfo SAPUserInfo = null;
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
                    SAPUserInfo = SAPUserinfo.CreateSAPUserinfo();
                    SAPUserInfo.Id = CheckNull.AgainstInt(reader["UserId"]);
                    SAPUserInfo.EndPointSystemId = CheckNull.AgainstInt(reader["EndPointSystemId"]);
                    SAPUserInfo.Firstname = CheckNull.AgainstString(reader["FIRSTNAME"]);
                    SAPUserInfo.Lastname = CheckNull.AgainstString(reader["LASTNAME"]);
                    SAPUserInfo.Initials = CheckNull.AgainstString(reader["INITIALS"]);
                    SAPUserInfo.Department = CheckNull.AgainstString(reader["DEPARTMENT"]);
                    SAPUserInfo.E_Mail = CheckNull.AgainstString(reader["E_Mail"]);
                    SAPUserInfo.Pers_No = CheckNull.AgainstString(reader["Pers_No"]);
                    SAPUserInfo.Tel1_Numbr = CheckNull.AgainstString(reader["Tel1_Numbr"]);
                    SAPUserInfo.Function0 = CheckNull.AgainstString(reader["Function0"]);
                    SAPUserInfo.Title = CheckNull.AgainstString(reader["TITLE"]);
                    SAPUserInfo.City = CheckNull.AgainstString(reader["CITY"]);
                    SAPUserInfo.Namcountry = CheckNull.AgainstString(reader["Namcountry"]);
                    SAPUserInfo.Postl_Cod1 = CheckNull.AgainstString(reader["Postl_Cod1"]);
                    SAPUserInfo.Pboxcit_No = CheckNull.AgainstString(reader["Pboxcit_No"]);
                    SAPUserInfo.Street = CheckNull.AgainstString(reader["STREET"]);
                    SAPUserInfo.ActivityGroup = CheckNull.AgainstString(reader["ActivityGroup"]);
                    SAPUserInfo.LastLoginTime = CheckNull.AgainstDatetime(reader["LastLoginTime"]);
                    SAPUserInfo.UserLogonName = CheckNull.AgainstString(reader["UserLogonName"]);
                    SAPUserInfo.AccntLocDurPwd = CheckNull.AgainstString(reader["AccntLocDurPwd"]);
                    SAPUserInfo.AccntLockNoPwd = CheckNull.AgainstString(reader["AccntLockNoPwd"]);
                    SAPUserInfo.LOCKEDBYINTRUDER = CheckNull.AgainstString(reader["LOCKEDBYINTRUDER"]);
                    SAPUserInfo.EMPLOYEEID = CheckNull.AgainstString(reader["EMPLOYEEID"]);
                    SAPUserInfo.EMPLOYEETYPE = CheckNull.AgainstString(reader["EMPLOYEETYPE"]);
                    SAPUserInfo.PASSWORDEXPTIME = CheckNull.AgainstDatetime(reader["PASSWORDEXPTIME"]);
                    SAPUserInfo.LOGINDISABLED = CheckNull.AgainstString(reader["LOGINDISABLED"]);
                    SAPUserInfo.IsActive = CheckNull.AgainstBit(reader["IsActive"]);
                    SAPUserInfo.CreatedDate = CheckNull.AgainstDatetime(reader["CreatedDate"]);
                    SAPUserInfo.LastModifiedDate = CheckNull.AgainstDatetime(reader["LastModifiedDate"]);


                    listSAP.Add(SAPUserInfo);
                }
            }
            catch (Exception)
            {

                throw;
            }

            finally
            {
                m_iacmOperator.Close();

            }

            return listSAP;
        }
        #endregion

        #region TCODES BY ROLE

        public DataSet GetTCodeByRole(string Role_Code)
        {
            DataSet ds = null;
            List<SqlParameter> parameters = new List<SqlParameter>();


            try
            {
                string query = "DI_spGetTCodesByRole";

                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Role_Code";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Value = Role_Code;
                parameters.Add(sqlParam);


                m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                executor = new QueryExecutor(m_iacmOperator);

                ds = executor.ExecuteQueryDS();


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                m_iacmOperator.Close();
            }

            return ds;
        }

        //public List<SAPUserRoleTCodeMapInfo> GetAllTCodes(int EndpointSystemId)
        //{
        //    return _context.SAPUserRoleTCodeMap.Where
        //        ( s => s.EndPointSystemId == EndpointSystemId).ToList();
        //}

        public List<SAPTCodeInfo> GetAllTCodes(long EndpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    if (EndpointSystemId != -1)
                        return _context.SAPTCodeInfos.Where(s => s.EndPointSystemId == EndpointSystemId)
                            .OrderBy(e => e.TCODE_Code).ToList();
                    return _context.SAPTCodeInfos.OrderBy(e => e.TCODE_Code).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, string> GetAllSapTCodes(long EndpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    return _context.SAPTCodeInfos.Where(s => s.EndPointSystemId == EndpointSystemId)
                            .OrderBy(e => e.TCODE_Code)
                             .Select(x => new { Key = x.Id, Value = x.TCODE_Code })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SAPTCodeInfo> GetAllTCodesWithoutTrackingCodes(long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPTCodeInfos
                    .Where(s => s.EndPointSystemId == EndpointSystemId)
                    .AsNoTracking()
                    .ToList();

            }
        }

        public List<SAPUserinfo> GetAllUsersInARole(string RoleCode)
        {
            using (_context = new IACMDBContext())
            {
                SAPRoleInfo role = _context.SAPRoleInfos.FirstOrDefault(s => s.ROLE_CODE == RoleCode);
                List<SAPUserinfo> usrList = null;
                if (role != null) usrList = role.SAPUserinfo.ToList();
                return usrList;


            }
        }

        public List<SAPUserinfo> GetAllUsersInARole(string RoleCode, long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                SAPRoleInfo role = _context.SAPRoleInfos.Include("SAPUserinfo").FirstOrDefault(s => s.ROLE_CODE == RoleCode && s.EndPointSystemId == EndpointSystemId);
                List<SAPUserinfo> usrList = null;
                if (role != null) usrList = role.SAPUserinfo.ToList();
                return usrList;


            }
        }
        public List<SAPUserinfo> GetAllUsersInATCode(string TCode, long EndpointId)
        {
            List<SAPUserinfo> usrList = new List<SAPUserinfo>();
            List<SapUserProfile> usrProfileList = new List<SapUserProfile>();

            //List<SapUserProfile> profiles = _context.prof

            List<string> AuthorizationCodes = GetListOfAuthorizations(TCode, EndpointId);
            using (_context = new IACMDBContext())
            {
                if (AuthorizationCodes != null)
                {
                    foreach (string AuthCode in AuthorizationCodes)
                    {
                        List<SAPAuthorizationProfile> AuthProfileLst = _context.SAPAuthorizationInfos.Where(s => s.AuthorizationCode == AuthCode).ToList();
                        foreach (SAPAuthorizationProfile AuthProfile in AuthProfileLst)
                        {
                            List<SapUserProfile> SapUsrProfile = _context.SapUserProfile.Where(s => s.ProfileName == AuthProfile.ProfileCode).ToList();
                            usrProfileList = usrProfileList.Union(SapUsrProfile).ToList();
                        }
                    }
                }

                foreach (SapUserProfile usrinfo in usrProfileList)
                {
                    SAPUserinfo user = _context.SAPUserinfos.FirstOrDefault(s => s.UserLogonName == usrinfo.UserName);
                    if (user != null)
                    {
                        usrList.Add(user);
                    }
                }

                return usrList;
            }
        }

        public List<SAPUserinfo> GetAllUsersInATCodeRole(string TCode)
        {
            using (_context = new IACMDBContext())
            {
                SAPTCodeInfo tcode = _context.SAPTCodeInfos.FirstOrDefault(s => s.TCODE_Code == TCode);

                List<SAPUserinfo> usrList = new List<SAPUserinfo>();

                if (tcode.SAPSingleRoleInfos != null && tcode.SAPSingleRoleInfos.Count > 0)
                {
                    foreach (SAPSingleRoleInfo roleinfo in tcode.SAPSingleRoleInfos)
                    {
                        SAPRoleInfo role = _context.SAPRoleInfos.FirstOrDefault(s => s.ROLE_CODE == roleinfo.SingleRole_Code);
                        if (role != null)
                        {
                            if (role.SAPUserinfo != null)
                            {
                                usrList = usrList.Union(role.SAPUserinfo).ToList();
                            }
                        }
                    }
                }
                return usrList;
            }
        }


        #region Provisioning Methods
        public List<string> getChildRolesByCompositeRoleCode(List<string> compositeRolesList)
        {
            if (!(compositeRolesList != null && compositeRolesList.Count > 0))
                return null;

            //string queryFormatter = "SELECT DISTINCT CHILD_ROLE_CODE FROM DI_SAP_COMPOSITE_CHILD_ROLEMAP_INFO WHERE COMPOSITE_ROLE_CODE IN ({0})";
            List<string> sapChildRoles = new List<string>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    IList<SAPCompositeRoleInfo> SAPCompositeRoleInfoList = new List<SAPCompositeRoleInfo>();
                    SAPCompositeRoleInfoList = _context.SAPCompositeRoleInfos.Where(cri => compositeRolesList.Contains(cri.CompositeRole_Code)).ToList();
                    foreach (SAPCompositeRoleInfo listItem in SAPCompositeRoleInfoList)
                    {
                        IList<SAPSingleRoleInfo> SAPsingleRoleInfoList = listItem.SAPSingleRoleInfos.ToList();
                        foreach (SAPSingleRoleInfo listSingleRoleItem in SAPsingleRoleInfoList)
                        {
                            if (!string.IsNullOrEmpty(listSingleRoleItem.SingleRole_Code))
                            {
                                if (sapChildRoles.Contains(listSingleRoleItem.SingleRole_Code) == false)
                                    sapChildRoles.Add(listSingleRoleItem.SingleRole_Code);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while getting child roles by composite role codes.", ex);
                throw;
            }
            return sapChildRoles;
        }

        public bool UpdateSAPUserInfoFromProvisioning(SAPUserinfo sapUserInfo)
        {
            bool isDataSaved = false;
            if (sapUserInfo == null)
                return isDataSaved;

            //string query = "DI_spUpdateSAPUserInfoFromProvisioning";
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    IList<SAPUserinfo> userList = _context.SAPUserinfos.Where(usr => usr.Id == sapUserInfo.Id).ToList();

                    foreach (SAPUserinfo userItem in userList)
                    {
                        userItem.Firstname = sapUserInfo.Firstname;
                        userItem.Lastname = sapUserInfo.Lastname;
                        userItem.Title = sapUserInfo.Title;
                        userItem.E_Mail = sapUserInfo.E_Mail;
                        userItem.Tel1_Numbr = sapUserInfo.Tel1_Numbr;
                        userItem.Department = sapUserInfo.Department;
                        userItem.LastModifiedBy = string.IsNullOrEmpty(sapUserInfo.LastModifiedBy) ? "System" : sapUserInfo.LastModifiedBy;
                        userItem.LastModifiedDate = sapUserInfo.LastModifiedDate;
                    }
                    _context.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                isDataSaved = false;
                DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while updating  SAPUserInfo From Provisioning.", ex);
                throw;
            }

            isDataSaved = true;
            return isDataSaved;
        }
        #endregion
        public IList<SAPUserinfo> GetAllUsersInObject(string TCode, string ObjectCode, string FieldCode, string Value)
        {
            using (_context = new IACMDBContext())
            {
                SAPAuthorizationToObject authobject = _context.SapAuthorizationToObject.FirstOrDefault(s => s.ObjectCode == ObjectCode
                                                        && s.FieldCode == FieldCode && s.FromValue == Value);


                SAPAuthorizationProfile AuthCompositeProfile = _context.SAPAuthorizationInfos.FirstOrDefault(s => s.AuthorizationCode == authobject.AuthorizationCode);

                SAPCompositeProfileInfo compositeprofile = _context.SAPCompositeProfileInfos.FirstOrDefault(s => s.SUBProfile_Code == AuthCompositeProfile.ProfileCode);

                SAPProfileInfo profile = _context.SAPProfileInfos.FirstOrDefault(s => s.PROFILE_CODE == compositeprofile.PROFILE_CODE);


                return profile.SAPUserinfos;
            }
        }



        #endregion


        #region NEW METHODS EF

        #region USER

        public SAPUserinfo GetUserByUserLogonName(string UserLogonName, long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPUserinfos.FirstOrDefault(t => t.UserLogonName == UserLogonName && t.EndPointSystemId == EndpointSystemId);
            }
        }
        public void AddSAPUser(SAPUserinfo user)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPUserinfos.Add(user);
                _context.SaveChanges();
            }
        }
        public void UpdateSAPUser(SAPUserinfo user)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                var accReqEntry = _context.Entry(user);
                if (accReqEntry.State == EntityState.Detached)
                {
                    var set = _context.Set<SAPUserinfo>();
                    SAPUserinfo attachedAccReq = set.Find(user.Id);

                    if (attachedAccReq != null)
                    {
                        var attachedEntry = _context.Entry(attachedAccReq);
                        attachedEntry.CurrentValues.SetValues(user);
                    }
                }
                _context.SaveChanges();
            }
        }
        public IList<SAPUserinfo> GetUsersFromDB()
        {
            using (_context = new IACMDBContext())
            {
                IList<SAPUserinfo> list = new List<SAPUserinfo>();
                SAPUserinfo obj = new SAPUserinfo();


                return _context.SAPUserinfos.Include("SAPRoleInfo").Include("SAPGroupInfo").Include("SAPParameterInfo").ToList();

            }

        }
        public IList<SAPUserinfo> GetUsersFromDB(long endpointId)
        {
            using (_context = new IACMDBContext())
            {
                IList<SAPUserinfo> listSAPUsers = new List<SAPUserinfo>();
                var allSAPusers = _context.SAPUserinfos.Where(endpoint => endpoint.EndPointSystemId == endpointId);
                listSAPUsers = allSAPusers.ToList();
                return listSAPUsers;

            }
        }
        public IList<SAPUserinfo> GetDeletedUsers(long EndpointId, Process discoveryProcess)
        {
            using (_context = new IACMDBContext())
            {
                IList<SAPUserinfo> listDeletedUsers = new List<SAPUserinfo>();
                if (discoveryProcess != null)
                {
                    listDeletedUsers = _context.SAPUserinfos.Where(d => d.EndPointSystemId == EndpointId && d.LastModifiedDate < discoveryProcess.ProcessStartAt).ToList();
                }
                return listDeletedUsers;
            }
        }
        public void RemoveSAPuser(SAPUserinfo user)
        {
            using (_context = new IACMDBContext())
            {
                SAPUserinfo sapuser = new SAPUserinfo();
                sapuser = _context.SAPUserinfos.First(u => u.Id == user.Id);
                _context.SAPUserinfos.Remove(sapuser);
                _context.SaveChanges();
            }
        }

        #endregion

        #region  GROUP
        public void SaveGroups(IList<SAPGroupInfo> list, long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                IList<SAPGroupInfo> listExistingSAPgroups = new List<SAPGroupInfo>();
                SAPGroupInfo sapexistingGroups = null;
                listExistingSAPgroups = _context.SAPGroupInfos.ToList();
                foreach (SAPGroupInfo grp in list)
                {
                    sapexistingGroups = listExistingSAPgroups.Where(t => t.GROUP_CODE == grp.GROUP_CODE && t.EndPointSystemId == EndpointSystemId).FirstOrDefault();
                    if (sapexistingGroups == null)
                    {
                        _context.SAPGroupInfos.Add(grp);
                        _context.SaveChanges();

                    }

                    else
                    {
                        grp.Id = sapexistingGroups.Id;
                        grp.GROUP_DESC = sapexistingGroups.GROUP_DESC;
                        grp.CreatedDate = sapexistingGroups.CreatedDate;
                        grp.CreatedBy = sapexistingGroups.CreatedBy;
                        _context.Entry(sapexistingGroups).CurrentValues.SetValues(grp);
                    }

                }
                _context.SaveChanges();
            }
        }
        public SAPGroupInfo GetgroupByGROUP_CODE(string groupCode, long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPGroupInfos.FirstOrDefault(group => group.GROUP_CODE == groupCode && group.EndPointSystemId == EndpointSystemId);
            }
        }
        public IList<SAPGroupInfo> GetDeletedGroups(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPGroupInfo> listDeletedGroups = new List<SAPGroupInfo>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedGroups = _context.SAPGroupInfos.Where(d => d.EndPointSystemId == EndpointId && d.ModifiedDate < process.ProcessStartAt).ToList();
                    }
                }

                return listDeletedGroups;

            }
        }
        public void RemoveSAPGroups(SAPGroupInfo group)
        {
            using (_context = new IACMDBContext())
            {
                SAPGroupInfo sapgroup = new SAPGroupInfo();
                sapgroup = _context.SAPGroupInfos.First(u => u.Id == group.Id);
                _context.SAPGroupInfos.Remove(sapgroup);
                _context.SaveChanges();
            }
        }
        #endregion

        #region PROFILES

        public SAPProfileInfo GetProfileByPROFILE_CODE(string profileCode, long EndPointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPProfileInfos.FirstOrDefault(t => t.PROFILE_CODE == profileCode && t.EndPointSystemId == EndPointSystemId);
            }
        }

        public SAPCompositeProfileInfo GetCompositeProfileByPROFILE_CODE(string profileCode, string subprofilecode, long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPCompositeProfileInfos.FirstOrDefault(t => t.PROFILE_CODE == profileCode && t.SUBProfile_Code == subprofilecode && t.EndPointSystemId == EndpointSystemId);
            }
        }


        public void AddProfile(SAPProfileInfo profile)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPProfileInfos.Add(profile);
                _context.SaveChanges();
            }
        }

        public void AddCompositeProfile(SAPCompositeProfileInfo profile)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPCompositeProfileInfos.Add(profile);
                _context.SaveChanges();
            }
        }

        public void UpdateProfile(SAPProfileInfo profile)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPProfileInfos.Attach(profile);
                _context.Entry(profile).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public void UpdateCompositeProfile(SAPCompositeProfileInfo profile)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPCompositeProfileInfos.Attach(profile);
                _context.Entry(profile).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public IList<SAPProfileInfo> GetDeletedProfiles(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPProfileInfo> listDeletedProfiles = new List<SAPProfileInfo>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedProfiles = _context.SAPProfileInfos.Where(d => d.EndPointSystemId == EndpointId && d.ModifiedDate < process.ProcessStartAt).ToList();
                    }
                }

                return listDeletedProfiles;

            }
        }

        public IList<SAPCompositeProfileInfo> GetDeletedCompositeProfiles(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPCompositeProfileInfo> listDeletedProfiles = new List<SAPCompositeProfileInfo>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedProfiles = _context.SAPCompositeProfileInfos.Where(d => d.EndPointSystemId == EndpointId && d.ModifiedDate < process.ProcessStartAt).ToList();
                    }
                }

                return listDeletedProfiles;

            }
        }


        public void RemoveSAPProfiles(SAPProfileInfo profile)
        {
            using (_context = new IACMDBContext())
            {
                SAPProfileInfo sapProfile = new SAPProfileInfo();
                sapProfile = _context.SAPProfileInfos.First(u => u.Id == profile.Id);
                _context.SAPProfileInfos.Remove(sapProfile);
                _context.SaveChanges();
            }
        }

        public void RemoveSAPCompositeProfiles(SAPCompositeProfileInfo profile)
        {
            using (_context = new IACMDBContext())
            {
                SAPCompositeProfileInfo sapProfile = new SAPCompositeProfileInfo();
                sapProfile = _context.SAPCompositeProfileInfos.First(u => u.Id == profile.Id);
                _context.SAPCompositeProfileInfos.Remove(sapProfile);
                _context.SaveChanges();
            }
        }

        public List<SAPProfileInfo> GetAllProfiles(EndPoint endpoint)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPProfileInfos.Where(s => s.EndPointSystemId == endpoint.Id).ToList();
            }
        }

        public List<SAPProfileInfo> GetAllProfiles()
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    return _context.SAPProfileInfos.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SAPGroupInfo> GetAllSapGroups()
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    return _context.SAPGroupInfos.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SAPCompositeProfileInfo> GetAllCompositeProfiles()
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPCompositeProfileInfos.ToList();
            }
        }

        public List<SAPUserinfo> GetAllUsersInProfile(string ProfileCode, long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                SAPProfileInfo profile = _context.SAPProfileInfos.Include("SAPUserinfos").FirstOrDefault(s => s.PROFILE_CODE == ProfileCode && s.EndPointSystemId == EndpointSystemId);
                return profile.SAPUserinfos.ToList();
            }
        }



        #endregion

        #region Authorization

        public SAPAuthorizationProfile GetAuthorizationByProfileCode(SAPAuthorizationProfile AuthProfile)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPAuthorizationInfos.FirstOrDefault(t => t.AuthorizationCode == AuthProfile.AuthorizationCode
                                                                        && t.ObjectCode == AuthProfile.ObjectCode && t.ProfileCode == AuthProfile.ProfileCode);
            }
        }

        public void AddAuthorization(SAPAuthorizationProfile AuthInfo)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPAuthorizationInfos.Add(AuthInfo);
                _context.SaveChanges();
            }
        }

        public void UpdateAuthorization(SAPAuthorizationProfile AuthInfo)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPAuthorizationInfos.Attach(AuthInfo);
                _context.Entry(AuthInfo).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }



        public IList<SAPAuthorizationProfile> GetDeletedAuthorizations(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPAuthorizationProfile> listDeletedAuthorizations = new List<SAPAuthorizationProfile>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedAuthorizations = _context.SAPAuthorizationInfos.Where(d => d.EndPointSystemId == EndpointId && d.ModifiedDate < process.ProcessStartAt).ToList();
                    }
                }

                return listDeletedAuthorizations;

            }
        }
        public void RemoveSAPAuthorization(SAPAuthorizationProfile authinfo)
        {
            using (_context = new IACMDBContext())
            {
                SAPAuthorizationProfile sapauth = new SAPAuthorizationProfile();
                sapauth = _context.SAPAuthorizationInfos.First(u => u.AuthorizationCode == authinfo.AuthorizationCode);
                _context.SAPAuthorizationInfos.Remove(sapauth);
                _context.SaveChanges();
            }
        }

        public void DeleteAuthorizations(long EndpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    string query = string.Format("Delete from DI_SAP_AuthorizationProfile where EndpointSystemId={0}", EndpointSystemId);
                    _context.Database.ExecuteSqlCommand(query);
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

                string tableName = "DI_SAP_AuthorizationProfile";

                SQLBulkUploader<SAPAuthorizationProfile> sqlSAPUserProfileMap = new SQLBulkUploader<SAPAuthorizationProfile>(tableName);

                sqlSAPUserProfileMap.Flush(list);


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
                using (_context = new IACMDBContext())
                {
                    string query = string.Format("Delete from DI_SAP_Objects where EndpointSystemId={0}", EndpointSystemId);
                    _context.Database.ExecuteSqlCommand(query);
                }

                string tableName = "DI_SAP_Objects";

                SQLBulkUploader<SAPObjectInfo> sqlSAPUserProfileMap = new SQLBulkUploader<SAPObjectInfo>(tableName);

                sqlSAPUserProfileMap.Flush(list);
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
                using (_context = new IACMDBContext())
                {
                    string query = string.Format("Delete from DI_SAP_AuthObjectMap where EndpointSystemId={0}", EndpointSystemId);
                    _context.Database.ExecuteSqlCommand(query);
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

                string tableName = "DI_SAP_AuthObjectMap";

                SQLBulkUploader<SAPAuthorizationToObject> sqlSAPUserProfileMap = new SQLBulkUploader<SAPAuthorizationToObject>(tableName);

                sqlSAPUserProfileMap.Flush(list);


            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SAPRoleToAuthMap> GetSapRoleToAuthBasedOnRoles(List<string> DistinctRoles, long EndpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    //var SapRoleToAuthorizations = from Roles in DistinctRoles 
                    //                              join RoleToAuth in _context.SapRoleToAuthMap on Roles equals RoleToAuth.RoleName
                    //                              where RoleToAuth.EndPointSystemId == EndpointSystemId
                    //                              select RoleToAuth;

                    var SapRoleToAuthorizations = from RoleToAuth in _context.SapRoleToAuthMap
                                                  where DistinctRoles.Contains(RoleToAuth.RoleName)
                                                  select RoleToAuth;

                    return SapRoleToAuthorizations.ToList();

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
                using (_context = new IACMDBContext())
                {
                    string query = string.Format("Delete from DI_SAP_ObjectValues where EndpointSystemId={0}", EndpointSystemId);
                    _context.Database.ExecuteSqlCommand(query);
                }
                string tableName = "DI_SAP_ObjectValues";

                SQLBulkUploader<SAPObjectValues> sqlSAPUserProfileMap = new SQLBulkUploader<SAPObjectValues>(tableName);

                sqlSAPUserProfileMap.Flush(list);


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
                using (_context = new IACMDBContext())
                {
                    string query = string.Format("Delete from DI_SAP_RoleToAuthMap where EndpointSystemId={0}", EndpointSystemId);
                    _context.Database.ExecuteSqlCommand(query);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveSapRoleAuthMap(IList<SAPRoleToAuthMap> list, long EndpointSystemId)
        {
            try
            {
                string tableName = "DI_SAP_RoleToAuthMap";

                SQLBulkUploader<SAPRoleToAuthMap> sqlSAPUserProfileMap = new SQLBulkUploader<SAPRoleToAuthMap>(tableName);

                sqlSAPUserProfileMap.Flush(list);


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
                using (_context = new IACMDBContext())
                {
                    string query = string.Format("Delete from DI_SAP_ROLETCODEMAPPING where EndpointSystemId={0}", EndpointSystemId);
                    _context.Database.ExecuteSqlCommand(query);
                }

                string tableName = "DI_SAP_ROLETCODEMAPPING";

                SQLBulkUploader<SapRoleTCodeMapping> sqlSAPUserProfileMap = new SQLBulkUploader<SapRoleTCodeMapping>(tableName);

                sqlSAPUserProfileMap.Flush(lstSapRoleTCodeMapping);
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
                using (_context = new IACMDBContext())
                {
                    string query = string.Format("Delete from DI_SAP_FIELDVALUEMAPPING where EndpointSystemId={0}", EndpointSystemId);
                    _context.Database.ExecuteSqlCommand(query);
                }
                string tableName = "DI_SAP_FIELDVALUEMAPPING";

                SQLBulkUploader<SAPFieldValueMapping> sqlSAPUserProfileMap = new SQLBulkUploader<SAPFieldValueMapping>(tableName);

                sqlSAPUserProfileMap.Flush(list);

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
                using (_context = new IACMDBContext())
                {
                    string query = string.Format("Delete from DI_SAP_TCodeObject where EndpointSystemId={0}", EndpointSystemId);
                    _context.Database.ExecuteSqlCommand(query);

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

                string tableName = "DI_SAP_TCodeObject";

                SQLBulkUploader<SAPTCodeObjectMap> sqlSAPUserProfileMap = new SQLBulkUploader<SAPTCodeObjectMap>(tableName);

                sqlSAPUserProfileMap.Flush(list);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveSAPSaveActivityInfo(IList<SAPActivityInfo> list, long EndpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    string query = string.Format("Delete from DI_SAP_ActivityInfo where EndpointSystemId={0}", EndpointSystemId);
                    _context.Database.ExecuteSqlCommand(query);

                }
                string tableName = "DI_SAP_ActivityInfo";

                SQLBulkUploader<SAPActivityInfo> sqlSAPActivityInfo = new SQLBulkUploader<SAPActivityInfo>(tableName);

                sqlSAPActivityInfo.Flush(list);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SAPAuthorizationToObject GetSapObjectByObjectCode(string ObjectCode)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SapAuthorizationToObject.FirstOrDefault(t => t.ObjectCode == ObjectCode);
            }
        }
        
        public SAPAuthorizationToObject GetSapObjectByObjectCodeAndEndpointId(string ObjectCode, long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SapAuthorizationToObject.FirstOrDefault(t => t.ObjectCode == ObjectCode && t.EndPointSystemId == EndpointId);
            }
        }


        public SAPAuthorizationProfile GetProfileByAuthCode(string AuthCode)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPAuthorizationInfos.FirstOrDefault(s => s.AuthorizationCode == AuthCode);
            }
        }

        public SAPRoleToAuthMap GetRoleByAuthCode(string AuthCode)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SapRoleToAuthMap.FirstOrDefault(s => s.AuthCode == AuthCode);
            }
        }



        public void AddSapObjectAuthMap(SAPAuthorizationToObject sapObject)
        {
            using (_context = new IACMDBContext())
            {
                _context.SapAuthorizationToObject.Add(sapObject);
                _context.SaveChanges();
            }
        }

        public void UpdateSapObjectAuthMap(SAPAuthorizationToObject sapObject)
        {
            using (_context = new IACMDBContext())
            {
                _context.SapAuthorizationToObject.Attach(sapObject);
                _context.Entry(sapObject).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public IList<SAPAuthorizationToObject> GetDeletedSapObjectsAuthMap(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPAuthorizationToObject> listDeletedSapObjects = new List<SAPAuthorizationToObject>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedSapObjects = _context.SapAuthorizationToObject.Where(d => d.EndPointSystemId == EndpointId && d.ModifiedDate < process.ProcessStartAt).ToList();
                    }
                }

                return listDeletedSapObjects;

            }
        }
        public void RemoveSAPObjectsAuthMap(SAPAuthorizationToObject authinfo)
        {
            using (_context = new IACMDBContext())
            {
                SAPAuthorizationToObject sapObject = new SAPAuthorizationToObject();
                sapObject = _context.SapAuthorizationToObject.First(u => u.ObjectCode == authinfo.ObjectCode);
                _context.SapAuthorizationToObject.Remove(sapObject);
                _context.SaveChanges();
            }
        }



        public List<SAPObjectInfo> GetSapObjects(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                var ObjectCodes = _context.SapObject.Where(s => s.EndPointSystemId == EndpointId);
                return ObjectCodes.ToList();
            }

        }

        public SAPObjectInfo GetSAPObject(int ObjectId)
        {
            using (_context = new IACMDBContext())
            {
                var sapObject = _context.SapObject.Where(s => s.Id == ObjectId).FirstOrDefault();
                return sapObject;
            }
        }

        public List<string> GetDistinctSapFields(long EndpointId, string ObjCode)
        {
            using (_context = new IACMDBContext())
            {
                var ObjectCodes = _context.SapAuthorizationToObject.Where(s => s.EndPointSystemId == EndpointId && s.ObjectCode == ObjCode)
                                                    .Select(e => e.FieldCode);
                return ObjectCodes.Distinct().ToList();
            }

        }

        public List<string> GetSapFields(long EndpointId, string ObjCode)
        {
            using (_context = new IACMDBContext())
            {
                SAPObjectInfo sapobject = _context.SapObject.FirstOrDefault(s => s.SapObjectCode == ObjCode && s.EndPointSystemId == EndpointId);

                List<string> FieldList = new List<string>();

                if (sapobject != null)
                {
                    if (sapobject.FieldCode0 != null && sapobject.FieldCode0 != string.Empty)
                        FieldList.Add(sapobject.FieldCode0);

                    if (sapobject.FieldCode1 != null && sapobject.FieldCode1 != string.Empty)
                        FieldList.Add(sapobject.FieldCode1);

                    if (sapobject.FieldCode2 != null && sapobject.FieldCode2 != string.Empty)
                        FieldList.Add(sapobject.FieldCode2);

                    if (sapobject.FieldCode3 != null && sapobject.FieldCode3 != string.Empty)
                        FieldList.Add(sapobject.FieldCode3);

                    if (sapobject.FieldCode4 != null && sapobject.FieldCode4 != string.Empty)
                        FieldList.Add(sapobject.FieldCode4);

                    if (sapobject.FieldCode5 != null && sapobject.FieldCode5 != string.Empty)
                        FieldList.Add(sapobject.FieldCode5);

                    if (sapobject.FieldCode6 != null && sapobject.FieldCode6 != string.Empty)
                        FieldList.Add(sapobject.FieldCode6);

                    if (sapobject.FieldCode7 != null && sapobject.FieldCode7 != string.Empty)
                        FieldList.Add(sapobject.FieldCode7);

                    if (sapobject.FieldCode8 != null && sapobject.FieldCode8 != string.Empty)
                        FieldList.Add(sapobject.FieldCode8);

                    if (sapobject.FieldCode9 != null && sapobject.FieldCode9 != string.Empty)
                        FieldList.Add(sapobject.FieldCode9);



                }


                return FieldList;
            }

        }



        public List<string> GetListOfAuthorizations(string TCode, long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                if (EndpointId != -1)
                {
                    var AuthorizationCodes = _context.SapAuthorizationToObject.Where(s => s.EndPointSystemId == EndpointId && s.FieldCode == "TCD" && s.FromValue == TCode)
                                                                          .Select(e => e.AuthorizationCode);
                    return AuthorizationCodes.Distinct().ToList();
                }
                else
                {
                    var AuthorizationCodes = _context.SapAuthorizationToObject.Where(s => s.FieldCode == "TCD" && s.FromValue == TCode)
                                                      .Select(e => e.AuthorizationCode);
                    return AuthorizationCodes.Distinct().ToList();
                }



            }
        }

        public List<SAPAuthorizationToObject> GetListOfAuthorizationObjects(long EndpointId, string AuthCode, string ObjectCode, string FieldCode)
        {
            using (_context = new IACMDBContext())
            {
                var AuthorizationCodes = _context.SapAuthorizationToObject.Where(s => s.EndPointSystemId == EndpointId && s.AuthorizationCode == AuthCode
                                                                                    && s.ObjectCode == ObjectCode && s.FieldCode == FieldCode);
                return AuthorizationCodes.ToList();

            }
        }

        public List<SAPAuthorizationToObject> GetListOfAuthorizationObjects(long EndpointId, string AuthCode)
        {
            using (_context = new IACMDBContext())
            {
                var AuthorizationCodes = _context.SapAuthorizationToObject.Where(s => s.EndPointSystemId == EndpointId && s.AuthorizationCode == AuthCode
                                                                                    );
                return AuthorizationCodes.ToList();

            }
        }
        public List<string> GetDistinctSapAuthorizations(long EndpointId, string AuthCode)
        {
            using (_context = new IACMDBContext())
            {
                var ObjectCodes = _context.SapAuthorizationToObject.Where(s => s.EndPointSystemId == EndpointId & s.FromValue == AuthCode)
                                                    .Select(e => e.AuthorizationCode);
                return ObjectCodes.Distinct().ToList();
            }

        }

        public SAPAuthorizationToObject GetAuthorizationAndObject(string AuthCode, string ObjectCode)
        {
            using (_context = new IACMDBContext())
            {
                SAPAuthorizationToObject AuthToObject = _context.SapAuthorizationToObject.FirstOrDefault(s => s.AuthorizationCode == AuthCode && s.ObjectCode == ObjectCode);

                return AuthToObject;
            }
        }

        #region Sap Field Value Mapping

        // field mapping
        public SAPFieldValueMapping GetSapFieldValueMap(string RoleCode, string FieldCode)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPFieldValueMap.FirstOrDefault(t => t.FieldCode == FieldCode && t.RoleCode == RoleCode);
            }
        }

        public void AddSapFieldValueMap(SAPFieldValueMapping sapObject)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPFieldValueMap.Add(sapObject);
                _context.SaveChanges();
            }
        }

        public void UpdateSapFieldValueMap(SAPFieldValueMapping sapObject)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPFieldValueMap.Attach(sapObject);
                _context.Entry(sapObject).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public IList<SAPFieldValueMapping> GetDeletedSAPFieldValueMap(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPFieldValueMapping> listDeletedSapObjects = new List<SAPFieldValueMapping>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedSapObjects = _context.SAPFieldValueMap.Where(d => d.EndPointSystemId == EndpointId && d.ModifiedDate < process.ProcessStartAt).ToList();
                    }
                }

                return listDeletedSapObjects;

            }
        }
        public void RemoveSAPFieldValueMap(SAPFieldValueMapping authinfo)
        {
            using (_context = new IACMDBContext())
            {
                SAPFieldValueMapping sapObject = new SAPFieldValueMapping();
                sapObject = _context.SAPFieldValueMap.First(u => u.RoleCode == authinfo.RoleCode && u.FieldCode == authinfo.FieldCode && u.EndPointSystemId == authinfo.EndPointSystemId);
                _context.SAPFieldValueMap.Remove(sapObject);
                _context.SaveChanges();
            }
        }

        #endregion


        #region Sap User Profiles

        // field mapping
        public SapUserProfile GetSAPUserProfile(string ProfileCode, string UserName)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SapUserProfile.FirstOrDefault(t => t.ProfileName == ProfileCode && t.UserName == UserName);
            }
        }

        public void ClearUserProfile(int EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                string query = string.Format("Delete from DI_SAP_UserProfile where EndpointSystemId={0}", EndpointSystemId);
                _context.Database.ExecuteSqlCommand(query);
            }
        }

        public void AddSAPUserProfile(SapUserProfile sapProfile)
        {
            using (_context = new IACMDBContext())
            {
                _context.SapUserProfile.Add(sapProfile);
                _context.SaveChanges();
            }
        }

        public void UpdateSapUserProfile(SapUserProfile sapProfile)
        {
            using (_context = new IACMDBContext())
            {
                _context.SapUserProfile.Attach(sapProfile);
                _context.Entry(sapProfile).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public IList<SapUserProfile> GetDeletedSAPUserProfile(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SapUserProfile> listDeletedSAPUserProfile = new List<SapUserProfile>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedSAPUserProfile = _context.SapUserProfile.Where(d => d.EndPointSystemId == EndpointId && d.ModifiedDate < process.ProcessStartAt).ToList();
                    }
                }

                return listDeletedSAPUserProfile;

            }
        }
        public void RemoveSAPUserProfile(SapUserProfile authinfo)
        {
            using (_context = new IACMDBContext())
            {
                SapUserProfile sapObject = new SapUserProfile();
                sapObject = _context.SapUserProfile.First(u => u.ProfileName == authinfo.ProfileName && u.UserName == authinfo.UserName && u.EndPointSystemId == authinfo.EndPointSystemId);
                _context.SapUserProfile.Remove(sapObject);
                _context.SaveChanges();
            }
        }

        #endregion

        #region SapObjectValues

        // Sap Object Values
        public SAPObjectValues GetSapObjectValue(string ObjectCode, string ObjectType)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SapObjectValues.FirstOrDefault(t => t.SapObjectCode == ObjectCode && t.SapObjectType == ObjectType);
            }
        }

        public void AddSapObjectValue(SAPObjectValues sapObject)
        {
            using (_context = new IACMDBContext())
            {
                _context.SapObjectValues.Add(sapObject);
                _context.SaveChanges();
            }
        }

        public void UpdateSapObjectValue(SAPObjectValues sapObject)
        {
            using (_context = new IACMDBContext())
            {
                _context.SapObjectValues.Attach(sapObject);
                _context.Entry(sapObject).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public IList<SAPObjectValues> GetDeletedObjectValue(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPObjectValues> listDeletedSapObjects = new List<SAPObjectValues>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedSapObjects = _context.SapObjectValues.Where(d => d.EndPointSystemId == EndpointId && d.ModifiedDate < process.ProcessStartAt).ToList();
                    }
                }

                return listDeletedSapObjects;

            }
        }
        public void RemoveSAPObjectValue(SAPObjectValues authinfo)
        {
            using (_context = new IACMDBContext())
            {
                SAPObjectValues sapObject = new SAPObjectValues();
                sapObject = _context.SapObjectValues.First(u => u.SapObjectCode == authinfo.SapObjectCode);
                _context.SapObjectValues.Remove(sapObject);
                _context.SaveChanges();
            }
        }

        #endregion

        #region TCode - Object Mapping

        public SAPTCodeObjectMap GetSapTCodeObjectMap(SAPTCodeObjectMap TCodeInfo)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SapTCodeObjectMap.FirstOrDefault(t => t.ObjectCode == TCodeInfo.ObjectCode && t.FieldCode == TCodeInfo.FieldCode && t.EndPointSystemId == TCodeInfo.EndPointSystemId);
            }
        }



        public void AddSapTCodeObjectMap(SAPTCodeObjectMap sapObject)
        {
            using (_context = new IACMDBContext())
            {
                _context.SapTCodeObjectMap.Add(sapObject);
                _context.SaveChanges();
            }
        }

        public void UpdateSapTCodeObjectMap(SAPTCodeObjectMap sapObject)
        {
            using (_context = new IACMDBContext())
            {
                _context.SapTCodeObjectMap.Attach(sapObject);
                _context.Entry(sapObject).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public IList<SAPTCodeObjectMap> GetDeletedSAPTCodeObjectMap(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPTCodeObjectMap> listDeletedSapTCodeObject = new List<SAPTCodeObjectMap>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedSapTCodeObject = _context.SapTCodeObjectMap.Where(d => d.EndPointSystemId == EndpointId && d.ModifiedDate < process.ProcessStartAt).ToList();
                    }
                }

                return listDeletedSapTCodeObject;

            }
        }
        public void RemoveSAPTCodeObjectMap(SAPTCodeObjectMap MapInfo)
        {
            using (_context = new IACMDBContext())
            {
                SAPTCodeObjectMap sapObject = new SAPTCodeObjectMap();
                sapObject = _context.SapTCodeObjectMap.First(u => u.FieldCode == MapInfo.FieldCode && u.ObjectCode == MapInfo.ObjectCode && u.EndPointSystemId == MapInfo.EndPointSystemId);
                _context.SapTCodeObjectMap.Remove(sapObject);
                _context.SaveChanges();
            }
        }

        #endregion


        #region Sap Objects
        public SAPObjectInfo GetSapObject(SAPObjectInfo TCodeInfo)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SapObject.FirstOrDefault(t => t.SapObjectCode == TCodeInfo.SapObjectCode && t.EndPointSystemId == TCodeInfo.EndPointSystemId);
            }
        }



        public void AddSapObject(SAPObjectInfo sapObject)
        {
            using (_context = new IACMDBContext())
            {
                _context.SapObject.Add(sapObject);
                _context.SaveChanges();
            }
        }

        public void UpdateSapObject(SAPObjectInfo sapObject)
        {
            using (_context = new IACMDBContext())
            {
                _context.SapObject.Attach(sapObject);
                _context.Entry(sapObject).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public IList<SAPObjectInfo> GetDeletedSAPObject(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPObjectInfo> listDeletedSapTCodeObject = new List<SAPObjectInfo>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedSapTCodeObject = _context.SapObject.Where(d => d.EndPointSystemId == EndpointId && d.ModifiedDate < process.ProcessStartAt).ToList();
                    }
                }

                return listDeletedSapTCodeObject;

            }
        }

        public void RemoveSAPObject(SAPObjectInfo ObjInfo)
        {
            using (_context = new IACMDBContext())
            {
                SAPObjectInfo sapObject = new SAPObjectInfo();
                sapObject = _context.SapObject.First(u => u.SapObjectCode == ObjInfo.SapObjectCode && u.EndPointSystemId == ObjInfo.EndPointSystemId);
                _context.SapObject.Remove(sapObject);
                _context.SaveChanges();
            }
        }
        #endregion

        #region Sap Role To Auth Map
        public SAPRoleToAuthMap GetSAPRoleToAuthMap(SAPRoleToAuthMap RoleToAuthMap)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SapRoleToAuthMap.FirstOrDefault(t => t.ObjectCode == RoleToAuthMap.ObjectCode && t.EndPointSystemId == RoleToAuthMap.EndPointSystemId);
            }
        }



        public void AddSAPRoleToAuthMap(SAPRoleToAuthMap RoleToAuthMap)
        {
            using (_context = new IACMDBContext())
            {
                _context.SapRoleToAuthMap.Add(RoleToAuthMap);
                _context.SaveChanges();
            }
        }

        public void UpdateSAPRoleToAuthMap(SAPRoleToAuthMap RoleToAuthMap)
        {
            using (_context = new IACMDBContext())
            {
                _context.SapRoleToAuthMap.Attach(RoleToAuthMap);
                _context.Entry(RoleToAuthMap).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public IList<SAPRoleToAuthMap> GetDeletedSAPRoleToAuthMap(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPRoleToAuthMap> listDeletedSapRoleToAuthMap = new List<SAPRoleToAuthMap>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedSapRoleToAuthMap = _context.SapRoleToAuthMap.Where(d => d.EndPointSystemId == EndpointId && d.ModifiedDate < process.ProcessStartAt).ToList();
                    }
                }

                return listDeletedSapRoleToAuthMap;

            }
        }

        public void RemoveSAPRoleToAuthMap(SAPRoleToAuthMap RoleToAuthMap)
        {
            using (_context = new IACMDBContext())
            {
                SAPRoleToAuthMap sapRoleToAuthMap = new SAPRoleToAuthMap();
                sapRoleToAuthMap = _context.SapRoleToAuthMap.First(u => u.ObjectCode == RoleToAuthMap.ObjectCode && u.EndPointSystemId == RoleToAuthMap.EndPointSystemId);
                _context.SapRoleToAuthMap.Remove(sapRoleToAuthMap);
                _context.SaveChanges();
            }
        }
        #endregion

        #endregion

        #region ROLES
        public SAPRoleInfo GetRoleByROLE_CODE(string ROLE_CODE, long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPRoleInfos.FirstOrDefault(role => role.ROLE_CODE == ROLE_CODE && role.EndPointSystemId == EndpointSystemId);
            }
        }
        public void AddSAPRole(SAPRoleInfo role)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPRoleInfos.Add(role);
                _context.SaveChanges();
            }
        }
        public void UpdateSAPRole(SAPRoleInfo role)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPRoleInfos.Attach(role);
                _context.Entry(role).State = EntityState.Modified;
                _context.SaveChanges();
            }

            //using ( _context = new IACMDBContext())
            //{
            //    var accReqEntry = _context.Entry<SAPRoleInfo>(role);
            //    if (accReqEntry.State == EntityState.Detached)
            //    {
            //        var set = _context.Set<SAPRoleInfo>();
            //        SAPRoleInfo attachedAccReq = set.Find(role.SAP_ROLEID);

            //        if (attachedAccReq != null)
            //        {
            //            var attachedEntry = _context.Entry(attachedAccReq);
            //            attachedEntry.CurrentValues.SetValues(role);
            //        }
            //    }
            //    _context.SaveChanges();
            //}
        }
        public IList<SAPRoleInfo> GetRolesFromDB(long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPRoleInfos.Where(endpoint => endpoint.EndPointSystemId == EndpointSystemId).ToList();

            }

        }

        public IList<SAPProfileInfo> GetProfilesFromDB(long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPProfileInfos.Where(endpoint => endpoint.EndPointSystemId == EndpointSystemId).ToList();

            }

        }
        public Dictionary<long, string> GetSAPAuthorizationObjects(string searchString, long EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SapObject.Where(s => s.EndPointSystemId == EndpointSystemId && s.SapObjectCode.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.SapObjectCode })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }
        public List<string> GetSapFields(string searchString, long EndpointSystemId, string ObjectCode)
        {
            List<string> FieldList = new List<string>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    //return _context.SapObject.Where(s => s.EndPointSystemId == EndpointSystemId && s.SapObjectCode == ObjectCode)
                    //    .Select(x => new { Key = x.SapObjectId, Value = x.SapObjectCode })
                    //    .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);

                    SAPObjectInfo sapobject = _context.SapObject.FirstOrDefault(s => s.SapObjectCode == ObjectCode && s.EndPointSystemId == EndpointSystemId);
                    if (sapobject != null)
                    {
                        if (sapobject.FieldCode0 != null && sapobject.FieldCode0 != string.Empty)
                            FieldList.Add(sapobject.FieldCode0);

                        if (sapobject.FieldCode1 != null && sapobject.FieldCode1 != string.Empty)
                            FieldList.Add(sapobject.FieldCode1);

                        if (sapobject.FieldCode2 != null && sapobject.FieldCode2 != string.Empty)
                            FieldList.Add(sapobject.FieldCode2);

                        if (sapobject.FieldCode3 != null && sapobject.FieldCode3 != string.Empty)
                            FieldList.Add(sapobject.FieldCode3);

                        if (sapobject.FieldCode4 != null && sapobject.FieldCode4 != string.Empty)
                            FieldList.Add(sapobject.FieldCode4);

                        if (sapobject.FieldCode5 != null && sapobject.FieldCode5 != string.Empty)
                            FieldList.Add(sapobject.FieldCode5);

                        if (sapobject.FieldCode6 != null && sapobject.FieldCode6 != string.Empty)
                            FieldList.Add(sapobject.FieldCode6);

                        if (sapobject.FieldCode7 != null && sapobject.FieldCode7 != string.Empty)
                            FieldList.Add(sapobject.FieldCode7);

                        if (sapobject.FieldCode8 != null && sapobject.FieldCode8 != string.Empty)
                            FieldList.Add(sapobject.FieldCode8);

                        if (sapobject.FieldCode9 != null && sapobject.FieldCode9 != string.Empty)
                            FieldList.Add(sapobject.FieldCode9);
                    }
                }
            }
            catch
            {
                throw;
            }
            return FieldList;
        }
        public List<SAPObjectInfo> GetSAPAuthorizationObjects(long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                var ObjectCodes = _context.SapObject.Where(s => s.EndPointSystemId == EndpointSystemId);
                return ObjectCodes.ToList();
            }

        }


        public IList<SAPRoleInfo> GetDeletedSapRoles(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPRoleInfo> listDeletedSapRoles = new List<SAPRoleInfo>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedSapRoles = _context.SAPRoleInfos.Where(d => d.EndPointSystemId == EndpointId && d.ModifiedDate < process.ProcessStartAt).ToList();
                    }
                }

                return listDeletedSapRoles;

            }
        }
        public void RemoveSapRoles(SAPRoleInfo saprole)
        {
            using (_context = new IACMDBContext())
            {
                SAPRoleInfo role = new SAPRoleInfo();
                role = _context.SAPRoleInfos.First(u => u.Id == saprole.Id);
                _context.SAPRoleInfos.Remove(role);
                _context.SaveChanges();
            }
        }

        public IList<SAPUsersToRolesReportData> GetSAPUserToRolesReportData(long EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    string sqlQuery = string.Format("select 0, sapuser.UserLogonName as username, sapuser.firstname + sapuser.lastname as DisplayName, sapuser.Department, sapuser.IsActive, saprole.ROLE_CODE as RoleCode, saprole.ROLE_DESC as RoleDescription, userrole.Role_FromDate, userrole.Role_ToDate  from SAPRoleInfoSAPUserinfo userrole inner join di_sap_userinfo sapuser on userrole.SAPUserinfo_Id = sapuser.userid and sapuser.EndPointSystemId = {0} inner join di_sap_roleinfo saprole on userrole.SAPRoleInfo_Id = saprole.Id and saprole.EndPointSystemId = {0}", EndpointSystemId);
                    var userstoroles = _context.Database.SqlQuery<SAPUsersToRolesReportData>
                                    (sqlQuery, 0);


                    return userstoroles.ToList();

                    //return (from sapUser in _context.SAPUserinfos
                    //        where sapUser.EndPointSystemId == EndpointSystemId
                    //        from sapRole in sapUser.SAPRoleInfo
                    //        where sapRole.EndPointSystemId == EndpointSystemId
                    //        select new SAPUsersToRolesReportData
                    //        {
                    //            UserId = sapUser.UserId,
                    //            UserName = sapUser.UserLogonName,
                    //            DisplayName = sapUser.Firstname + sapUser.Lastname,
                    //            RoleCode = sapRole.ROLE_CODE,
                    //            RoleDescription = sapRole.ROLE_DESC,
                    //            IsActive = sapUser.IsActive ? "YES" : "NO",
                    //            Department = sapUser.Department


                    //        }
                    //        )
                    //         .ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPUsersToRolesReportData> GetSAPUserToRolesReportData(long EndpointSystemId, List<int> UserList)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    List<SAPUsersToRolesReportData> UsersRoles = new List<SAPUsersToRolesReportData>();
                    foreach (int userid in UserList)
                    {
                        string sqlQuery = string.Format("select 0, sapuser.UserLogonName as username, sapuser.firstname + sapuser.lastname as DisplayName, sapuser.Department, sapuser.IsActive, saprole.ROLE_CODE as RoleCode, saprole.ROLE_DESC as RoleDescription, userrole.Role_FromDate, userrole.Role_ToDate  from SAPRoleInfoSAPUserinfo userrole inner join di_sap_userinfo sapuser on userrole.SAPUserinfo_Id = sapuser.Id and sapuser.EndPointSystemId = {0} inner join di_sap_roleinfo saprole on userrole.SAPRoleInfo_Id = saprole.Id and saprole.EndPointSystemId = {0} and sapuser.Id = {1}", EndpointSystemId, userid);

                        List<SAPUsersToRolesReportData> userstoroles = _context.Database.SqlQuery<SAPUsersToRolesReportData>
                                        (sqlQuery, 0).ToList();

                        UsersRoles = userstoroles.Union(UsersRoles).ToList();
                    }


                    return UsersRoles;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPUsersToRolesReportData> GetSAPUserToRolesReportData(int UserId, long EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return (from sapUser in _context.SAPUserinfos
                            where sapUser.Id == UserId && sapUser.EndPointSystemId == EndpointSystemId
                            from sapRole in sapUser.SAPRoleInfo
                            where sapRole.EndPointSystemId == EndpointSystemId
                            select new SAPUsersToRolesReportData
                            {
                                UserId = sapUser.Id,
                                UserName = sapUser.UserLogonName,
                                DisplayName = sapUser.Firstname + sapUser.Lastname,
                                RoleCode = sapRole.ROLE_CODE,
                                RoleDescription = sapRole.ROLE_DESC,
                                IsActive = sapUser.IsActive,
                                Department = sapUser.Department


                            }
                            ).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPUserRoleTCodeAuth> GetSapUserRoleTCodeAuthReport(long EndpointSystemId, int UserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    List<SAPUsersToRolesReportData> lstUsersRoles = (List<SAPUsersToRolesReportData>)GetSAPUserToRolesReportData(UserId);

                    List<SAPUserRoleTCodeAuth> lstSapUserRoleTCodeAuth = new List<SAPUserRoleTCodeAuth>();


                    foreach (SAPUsersToRolesReportData SapUserRole in lstUsersRoles)
                    {
                        List<string> lstSapRoleTCode = _context.SapRoleToAuthMap.Where(role => role.RoleName == SapUserRole.RoleCode
                                                        && role.ObjectCode == "S_TCODE" && role.FieldCode == "TCD")
                                                        .Select(tcode => tcode.FromValue).ToList();


                        foreach (string SapRoleAuth in lstSapRoleTCode)
                        {
                            List<string> lstRoleToAuth = _context.SapRoleToAuthMap.Where(role => role.RoleName == SapUserRole.RoleCode
                                                        && role.ObjectCode != "S_TCODE")
                                                        .Select(auth => auth.AuthCode).Distinct().ToList();

                            foreach (string SapRoleTCodeAuth in lstRoleToAuth)
                            {

                                SAPUserRoleTCodeAuth SapUserRoleTCodeAuth = new SAPUserRoleTCodeAuth();
                                SapUserRoleTCodeAuth.UserId = SapUserRole.UserId;
                                SapUserRoleTCodeAuth.UserName = SapUserRole.UserName;
                                SapUserRoleTCodeAuth.IsActive = SapUserRole.IsActive;
                                SapUserRoleTCodeAuth.DisplayName = SapUserRole.DisplayName;
                                SapUserRoleTCodeAuth.Department = SapUserRole.Department;
                                SapUserRoleTCodeAuth.RoleCode = SapUserRole.RoleCode;
                                SapUserRoleTCodeAuth.TCode = SapRoleAuth;
                                SapUserRoleTCodeAuth.AuthorizationCode = SapRoleTCodeAuth;

                                if (SapUserRoleTCodeAuth.ProfileCode == string.Empty)
                                {
                                    //SAPAuthorizationProfile 
                                }

                                lstSapUserRoleTCodeAuth.Add(SapUserRoleTCodeAuth);
                            }


                        }


                    }

                    return lstSapUserRoleTCodeAuth;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public IList<SAPUserRoleTCodeAuth> GetSapUserRoleTCodeAuthReport(int UserId, long EndpointSystemId, bool withoutAuthrization)
        {
            try
            {
                SAPUserRoleTCodeAuth SapUserRoleTCodeAuth = null;
                using (IACMDBContext _context = new IACMDBContext())
                {

                    List<SAPUsersToRolesReportData> lstUsersRoles = (List<SAPUsersToRolesReportData>)GetSAPUserToRolesReportData(UserId, EndpointSystemId);

                    List<SAPUserRoleTCodeAuth> lstSapUserRoleTCodeAuth = new List<SAPUserRoleTCodeAuth>();


                    foreach (SAPUsersToRolesReportData SapUserRole in lstUsersRoles)
                    {
                        List<string> lstSapRoleTCode = _context.SapRoleToAuthMap.Where(role => role.RoleName == SapUserRole.RoleCode
                                                        && role.ObjectCode == "S_TCODE" && role.FieldCode == "TCD")
                                                        .Select(tcode => tcode.FromValue).ToList();


                        foreach (string tCode in lstSapRoleTCode)
                        {

                            SapUserRoleTCodeAuth = new SAPUserRoleTCodeAuth();
                            SapUserRoleTCodeAuth.UserId = SapUserRole.UserId;
                            SapUserRoleTCodeAuth.UserName = SapUserRole.UserName;
                            SapUserRoleTCodeAuth.IsActive = SapUserRole.IsActive;
                            SapUserRoleTCodeAuth.DisplayName = SapUserRole.DisplayName;
                            SapUserRoleTCodeAuth.Department = SapUserRole.Department;
                            SapUserRoleTCodeAuth.RoleCode = SapUserRole.RoleCode;
                            SapUserRoleTCodeAuth.TCode = tCode;
                            lstSapUserRoleTCodeAuth.Add(SapUserRoleTCodeAuth);



                        }


                    }

                    return lstSapUserRoleTCodeAuth;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public IList<SAPUserRoleTCodeAuth> GetSapUserTCodeReport(int UserId, long EndpointSystemId)
        {
            try
            {
                SAPUserRoleTCodeAuth SapUserRoleTCodeAuth = null;
                using (IACMDBContext _context = new IACMDBContext())
                {

                    List<SAPUsersToRolesReportData> lstUsersRoles = (List<SAPUsersToRolesReportData>)GetSAPUserToRolesReportData(UserId);

                    List<SAPUserRoleTCodeAuth> lstSapUserRoleTCodeAuth = new List<SAPUserRoleTCodeAuth>();


                    foreach (SAPUsersToRolesReportData SapUserRole in lstUsersRoles)
                    {
                        List<string> lstSapRoleTCode = _context.SapRoleToAuthMap.Where(role => role.RoleName == SapUserRole.RoleCode
                                                        && role.ObjectCode == "S_TCODE" && role.FieldCode == "TCD")
                                                        .Select(tcode => tcode.FromValue).ToList();


                        foreach (string SapRoleAuth in lstSapRoleTCode)
                        {
                            List<string> lstRoleToAuth = _context.SapRoleToAuthMap.Where(role => role.RoleName == SapUserRole.RoleCode
                                                        && role.ObjectCode != "S_TCODE")
                                                        .Select(auth => auth.AuthCode).Distinct().ToList();

                            SapUserRoleTCodeAuth = new SAPUserRoleTCodeAuth();
                            SapUserRoleTCodeAuth.UserId = SapUserRole.UserId;
                            SapUserRoleTCodeAuth.UserName = SapUserRole.UserName;
                            SapUserRoleTCodeAuth.IsActive = SapUserRole.IsActive;
                            SapUserRoleTCodeAuth.DisplayName = SapUserRole.DisplayName;
                            SapUserRoleTCodeAuth.Department = SapUserRole.Department;
                            SapUserRoleTCodeAuth.RoleCode = SapUserRole.RoleCode;
                            SapUserRoleTCodeAuth.TCode = SapRoleAuth;
                            lstSapUserRoleTCodeAuth.Add(SapUserRoleTCodeAuth);



                        }


                    }

                    return lstSapUserRoleTCodeAuth;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public IList<SAPUserRoleTCodeAuth> GetSapUserRoleTCodeAuthReport(long EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    List<SAPUsersToRolesReportData> lstUsersRoles = (List<SAPUsersToRolesReportData>)GetSAPUserToRolesReportData(EndpointSystemId);

                    List<SAPUserRoleTCodeAuth> lstSapUserRoleTCodeAuth = new List<SAPUserRoleTCodeAuth>();


                    foreach (SAPUsersToRolesReportData SapUserRole in lstUsersRoles)
                    {
                        List<string> lstSapRoleTCode = _context.SapRoleToAuthMap.Where(role => role.RoleName == SapUserRole.RoleCode
                                                        && role.ObjectCode == "S_TCODE" && role.FieldCode == "TCD")
                                                        .Select(tcode => tcode.FromValue).ToList();


                        foreach (string SapRoleAuth in lstSapRoleTCode)
                        {
                            List<string> lstRoleToAuth = _context.SapRoleToAuthMap.Where(role => role.RoleName == SapUserRole.RoleCode
                                                        && role.ObjectCode != "S_TCODE")
                                                        .Select(auth => auth.AuthCode).Distinct().ToList();

                            foreach (string SapRoleTCodeAuth in lstRoleToAuth)
                            {

                                SAPUserRoleTCodeAuth SapUserRoleTCodeAuth = new SAPUserRoleTCodeAuth();
                                SapUserRoleTCodeAuth.UserId = SapUserRole.UserId;
                                SapUserRoleTCodeAuth.UserName = SapUserRole.UserName;
                                SapUserRoleTCodeAuth.IsActive = SapUserRole.IsActive;
                                SapUserRoleTCodeAuth.DisplayName = SapUserRole.DisplayName;
                                SapUserRoleTCodeAuth.Department = SapUserRole.Department;
                                SapUserRoleTCodeAuth.RoleCode = SapUserRole.RoleCode;
                                SapUserRoleTCodeAuth.TCode = SapRoleAuth;
                                SapUserRoleTCodeAuth.AuthorizationCode = SapRoleTCodeAuth;

                                if (SapUserRoleTCodeAuth.ProfileCode == string.Empty)
                                {
                                    //SAPAuthorizationProfile 
                                }

                                lstSapUserRoleTCodeAuth.Add(SapUserRoleTCodeAuth);
                            }


                        }


                    }

                    return lstSapUserRoleTCodeAuth;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public IList<SAPUserRoleTCodeAuth> GetSapUserTCodeReport(long EndpointSystemId)
        {
            SAPUserRoleTCodeAuth SapUserRoleTCodeAuth = null;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    List<SAPUsersToRolesReportData> lstUsersRoles = (List<SAPUsersToRolesReportData>)GetSAPUserToRolesReportData(EndpointSystemId);

                    List<SAPUserRoleTCodeAuth> lstSapUserRoleTCodeAuth = new List<SAPUserRoleTCodeAuth>();


                    foreach (SAPUsersToRolesReportData SapUserRole in lstUsersRoles)
                    {
                        List<string> lstSapRoleTCode = _context.SapRoleToAuthMap.Where(role => role.RoleName == SapUserRole.RoleCode
                                                        && role.ObjectCode == "S_TCODE" && role.FieldCode == "TCD")
                                                        .Select(tcode => tcode.FromValue).ToList();


                        foreach (string SapRoleAuth in lstSapRoleTCode)
                        {

                            SapUserRoleTCodeAuth = new SAPUserRoleTCodeAuth();
                            SapUserRoleTCodeAuth.UserId = SapUserRole.UserId;
                            SapUserRoleTCodeAuth.UserName = SapUserRole.UserName;
                            SapUserRoleTCodeAuth.IsActive = SapUserRole.IsActive;
                            SapUserRoleTCodeAuth.DisplayName = SapUserRole.DisplayName;
                            SapUserRoleTCodeAuth.Department = SapUserRole.Department;
                            SapUserRoleTCodeAuth.RoleCode = SapUserRole.RoleCode;
                            SapUserRoleTCodeAuth.TCode = SapRoleAuth;

                            lstSapUserRoleTCodeAuth.Add(SapUserRoleTCodeAuth);

                            if (lstSapUserRoleTCodeAuth.Count() % 100 == 0)
                                break;

                        }


                    }

                    return lstSapUserRoleTCodeAuth;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }


        public IList<SAPUsersToGroupsReportData> GetSAPUserToGroupsReportData()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return (from sapUser in _context.SAPUserinfos
                            from sapGroup in sapUser.SAPGroupInfo
                            select new SAPUsersToGroupsReportData
                            {
                                UserId = sapUser.Id,
                                UserName = sapUser.UserLogonName,
                                DisplayName = sapUser.Firstname + sapUser.Lastname,
                                GroupCode = sapGroup.GROUP_CODE,
                                GroupDescription = sapGroup.GROUP_DESC,
                                IsActive = sapUser.IsActive ? "YES" : "NO",
                                Department = sapUser.Department


                            }
                            ).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region COMPOSITE_ROLES
        public SAPCompositeRoleInfo GetSAPCompositeRoleByCompositeRole_Code(string CompositeRole_Code, long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPCompositeRoleInfos.Include("SAPSingleRoleInfos").FirstOrDefault(t => t.CompositeRole_Code.Trim() == CompositeRole_Code.Trim() && t.EndPointSystemId == EndpointSystemId);
            }
        }
        public void AddSAPCompositeRole(SAPCompositeRoleInfo sapCompositeRole)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPCompositeRoleInfos.Add(sapCompositeRole);
                _context.SaveChanges();
            }
        }
        public void UpdateSAPCompositeRole(SAPCompositeRoleInfo sapCompositeRole)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPCompositeRoleInfos.Attach(sapCompositeRole);
                _context.Entry(sapCompositeRole).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }
        public IList<SAPCompositeRoleInfo> SAPCompositeRolesFromDB(long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                IList<SAPCompositeRoleInfo> list = new List<SAPCompositeRoleInfo>();

                // return _context.SAPCompositeRoleInfos.Include("SAPCompositeRoleInfos").ToList();
                return _context.SAPCompositeRoleInfos.Where(endpoint => endpoint.EndPointSystemId == EndpointSystemId).ToList();

            }

        }
        public IList<SAPCompositeRoleInfo> GetDeletedSAPCompositeRoleInfo(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPCompositeRoleInfo> listDeletedSAPCompositeRoles = new List<SAPCompositeRoleInfo>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedSAPCompositeRoles = _context.SAPCompositeRoleInfos.Where(d => d.EndPointSystemId == EndpointId && d.ModifiedDate < process.ProcessStartAt).ToList();
                    }
                }

                return listDeletedSAPCompositeRoles;

            }
        }
        public void RemoveSAPCompositeRole(SAPCompositeRoleInfo CompositeRole)
        {
            using (_context = new IACMDBContext())
            {
                SAPCompositeRoleInfo compRole = new SAPCompositeRoleInfo();
                compRole = _context.SAPCompositeRoleInfos.First(u => u.Id == CompositeRole.Id);
                _context.SAPCompositeRoleInfos.Remove(compRole);
                _context.SaveChanges();
            }
        }



        #endregion

        #region CHILD_ROLES/SINGLE_ROLES
        public SAPSingleRoleInfo GetSAPSingleRoleBySingleRole_Code(string SingleRole_Code, long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPSingleRoleInfos.FirstOrDefault(t => t.SingleRole_Code == SingleRole_Code && t.EndPointSystemId == EndpointSystemId);
            }
        }
        public void AddSAPSingleRole(SAPSingleRoleInfo sapSingleRole)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPSingleRoleInfos.Add(sapSingleRole);
                _context.SaveChanges();
            }
        }
        public void UpdateSAPSingleRole(SAPSingleRoleInfo sapSingleRole)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPSingleRoleInfos.Attach(sapSingleRole);
                _context.Entry(sapSingleRole).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }
        public IList<SAPSingleRoleInfo> GetSingleRolesFromDB(long EndPointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPSingleRoleInfos.Where(endpoint => endpoint.EndPointSystemId == EndPointSystemId).ToList();

            }

        }

        public void SaveSingleRoles(IList<SAPSingleRoleInfo> list, long EndpointSystemId)
        {
            SAPSingleRoleInfo sapexistingSingleRole = null;


            try
            {
                using (_context = new IACMDBContext())
                {
                    List<SAPSingleRoleInfo> lstSapSingleRoles = _context.SAPSingleRoleInfos
                                                                .Where(singlerole => singlerole.EndPointSystemId == EndpointSystemId)
                                                                .AsNoTracking()
                                                                .ToList();


                    foreach (SAPSingleRoleInfo singlerole in list)
                    {

                        sapexistingSingleRole = lstSapSingleRoles.FirstOrDefault(sRole => sRole.SingleRole_Code == singlerole.SingleRole_Code && sRole.EndPointSystemId == singlerole.EndPointSystemId);

                        if (sapexistingSingleRole == null)
                        {
                            _context.SAPSingleRoleInfos.Add(singlerole);
                            _context.SaveChanges();
                            //SAPDA.AddSAPSingleRole(singlerole);
                        }
                        else
                        {
                            singlerole.Id = sapexistingSingleRole.Id;
                            singlerole.SingleRole_Code = sapexistingSingleRole.SingleRole_Code;
                            singlerole.CreatedBy = sapexistingSingleRole.CreatedBy;
                            singlerole.CreatedDate = sapexistingSingleRole.CreatedDate;
                            _context.SAPSingleRoleInfos.Attach(singlerole);
                            _context.Entry(singlerole).State = EntityState.Modified;
                            _context.SaveChanges();
                            //SAPDA.UpdateSAPSingleRole(singlerole);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SAPSingleRoleInfo> GetDeletedSAPSingleRole(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPSingleRoleInfo> listDeletedSingleRole = new List<SAPSingleRoleInfo>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedSingleRole = _context.SAPSingleRoleInfos.Where(d => d.EndPointSystemId == EndpointId && d.ModifiedDate < process.ProcessStartAt).ToList();
                    }
                }

                return listDeletedSingleRole;

            }
        }
        public void RemoveSapSingleRole(SAPSingleRoleInfo SingleRole)
        {
            using (_context = new IACMDBContext())
            {
                SAPSingleRoleInfo singrole = new SAPSingleRoleInfo();
                singrole = _context.SAPSingleRoleInfos.First(u => u.Id == SingleRole.Id);
                _context.SAPSingleRoleInfos.Remove(singrole);
                _context.SaveChanges();
            }
        }

        #endregion

        #region PARAMETERS

        public SAPParameterInfo GetParameterByPARAM_CODE(string param_code, long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPParameterInfos.FirstOrDefault(t => t.PARAM_CODE == param_code && t.EndPointSystemId == EndpointSystemId);
            }
        }

        public void ClearParameters(long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                _context.Database.ExecuteSqlCommand(string.Format("Delete From DI_SAP_ParameterInfo WHere EndpointSystemId = {0}", EndpointSystemId));
            }
        }

        public void SaveParameter(IList<SAPParameterInfo> list, long EndpointSystemId)
        {

            using (_context = new IACMDBContext())
            {
                foreach (SAPParameterInfo parameter in list)
                {

                    _context.SAPParameterInfos.Add(parameter);
                    _context.SaveChanges();
                }
            }


        }


        public void ClearALLSapPositions(long EndpointSystemId)
        {

            using (_context = new IACMDBContext())
            {
                _context.Database.ExecuteSqlCommand(string.Format("Delete From DI_SAP_Position WHere EndpointSystemId = {0}", EndpointSystemId));
            }
        }

        public void SaveSapPositionsBulk(List<SAPPosition> SapPositions)
        {
            string tableName = "DI_SAP_Position";

            SQLBulkUploader<SAPPosition> sqlSAPUserProfileMap = new SQLBulkUploader<SAPPosition>(tableName);

            sqlSAPUserProfileMap.Flush(SapPositions);
        }

        public void AddParameter(SAPParameterInfo parameter)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPParameterInfos.Add(parameter);
                _context.SaveChanges();
            }
        }
        public void UpdateParameter(SAPParameterInfo parameter)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPParameterInfos.Attach(parameter);
                _context.Entry(parameter).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }
        public IList<SAPParameterInfo> GetDeletedSAPParameters(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPParameterInfo> listDeletedSAPParameters = new List<SAPParameterInfo>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedSAPParameters = _context.SAPParameterInfos.Where(d => d.EndPointSystemId == EndpointId && d.ModifiedDate < process.ProcessStartAt).ToList();
                    }
                }

                return listDeletedSAPParameters;

            }
        }
        public void RemoveSAPParameter(SAPParameterInfo parameter)
        {
            using (_context = new IACMDBContext())
            {
                SAPParameterInfo sapparameter = new SAPParameterInfo();
                sapparameter = _context.SAPParameterInfos.First(u => u.Id == parameter.Id);
                _context.SAPParameterInfos.Remove(sapparameter);
                _context.SaveChanges();
            }
        }

        #endregion

        #region TCODES
        public SAPTCodeInfo GetTcodeByTCODE_Code(string tCode_Code)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPTCodeInfos.FirstOrDefault(t => t.TCODE_Code == tCode_Code);
            }
        }
        public void AddTcode(SAPTCodeInfo tcode)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPTCodeInfos.Add(tcode);
                _context.SaveChanges();
            }
        }

        public void UpdateTcode(SAPTCodeInfo tcode)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPTCodeInfos.Attach(tcode);
                _context.Entry(tcode).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public void SaveTCodes(IList<SAPTCodeInfo> list, IList<SAPTCodeInfo> lstSapTCodes, long EndpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    SAPTCodeInfo sapexistingTcode = null;

                    foreach (SAPTCodeInfo tcode in list)
                    {

                        sapexistingTcode = lstSapTCodes.FirstOrDefault(TCode => TCode.TCODE_Code == tcode.TCODE_Code && TCode.EndPointSystemId == EndpointSystemId);
                        if (sapexistingTcode == null)
                        {
                            _context.SAPTCodeInfos.Add(tcode);
                            _context.SaveChanges();
                        }
                        else
                        {
                            //tcode.TCODE_Id = sapexistingTcode.TCODE_Id;
                            //tcode.TCODE_Code = sapexistingTcode.TCODE_Code;
                            //tcode.CreatedBy = sapexistingTcode.CreatedBy;
                            //tcode.CreatedDate = sapexistingTcode.CreatedDate;

                            sapexistingTcode.TCODE_Desc = tcode.TCODE_Desc;
                            sapexistingTcode.ModifiedDate = tcode.ModifiedDate;
                            sapexistingTcode.ModifiedBy = tcode.ModifiedBy;

                            _context.SAPTCodeInfos.Attach(sapexistingTcode);
                            _context.Entry(sapexistingTcode).State = EntityState.Modified;
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public IList<SAPTCodeInfo> GetDeletedSAPTCodes(long EndpointId)
        {
            using (_context = new IACMDBContext())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPTCodeInfo> listDeletedSAPTCodes = new List<SAPTCodeInfo>();
                objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedSAPTCodes = _context.SAPTCodeInfos.Where(d => d.EndPointSystemId == EndpointId && d.ModifiedDate < process.ProcessStartAt).ToList();
                    }
                }
                return listDeletedSAPTCodes;

            }
        }
        public void RemoveSAPTCode(SAPTCodeInfo tCode)
        {
            using (_context = new IACMDBContext())
            {
                SAPTCodeInfo sapTcode = new SAPTCodeInfo();
                sapTcode = _context.SAPTCodeInfos.First(u => u.Id == tCode.Id);
                _context.SAPTCodeInfos.Remove(sapTcode);
                _context.SaveChanges();
            }
        }

        #endregion

        #region MAPPING TABLES
        #region GROUPS
        public void ClearSAPUserGroupMap(long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                string query = string.Format("Delete from SAPUserinfoSAPGroupInfo where EndpointSystemId={0}", EndpointSystemId);
                _context.Database.ExecuteSqlCommand(query);
            }
        }

        public void ClearSAPUserGroupMap(long EndpointSystemId, long UserId)
        {
            using (_context = new IACMDBContext())
            {
                string query = string.Format("Delete from SAPUserinfoSAPGroupInfo where EndpointSystemId={0} and SAPUserinfo_Id={1}", EndpointSystemId, UserId);
                _context.Database.ExecuteSqlCommand(query);
            }
        }

        public void SaveSAPUserGroupMap(IList<SAPUserGroup> listSAPUserGroup, long EndpointSystemId)
        {
            //using (_context = new IACMDBContext())
            //{
            //    string query = string.Format("Delete from SAPUserinfoSAPGroupInfo where EndpointSystemId={0}", EndpointSystemId);
            //    _context.Database.ExecuteSqlCommand(query);
            //}


            SQLBulkUploader<SAPUserGroup> sqlSAPUserGroupMap = null;
            try
            {
                string tableName = "SAPUserinfoSAPGroupInfo";

                sqlSAPUserGroupMap = new SQLBulkUploader<SAPUserGroup>(tableName);

                sqlSAPUserGroupMap.Flush(listSAPUserGroup);

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlSAPUserGroupMap.Close();
            }

        }
        #endregion

        #region PROFILES

        public void ClearSAPUserProfileMap(long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                string query = string.Format("Delete from SAPProfileInfoSAPUserinfo where EndpointSystemId={0}", EndpointSystemId);
                _context.Database.ExecuteSqlCommand(query);
            }
        }

        public void SaveSapUserProfile(IList<SapUserProfile> list, long EndpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    string query = string.Format("Delete from DI_SAP_UserProfile where EndpointSystemId={0}", EndpointSystemId);
                    _context.Database.ExecuteSqlCommand(query);
                }
                string tableName = "DI_SAP_UserProfile";

                SQLBulkUploader<SapUserProfile> sqlSAPUserProfileMap = new SQLBulkUploader<SapUserProfile>(tableName);

                sqlSAPUserProfileMap.Flush(list);


            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveSapRoleProfile(IList<SapRoleProfileMapping> list, long EndpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    string query = string.Format("Delete from DI_SAP_ROLEPROFILE where EndpointSystemId={0}", EndpointSystemId);
                    _context.Database.ExecuteSqlCommand(query);
                }
                string tableName = "DI_SAP_ROLEPROFILE";

                SQLBulkUploader<SapRoleProfileMapping> sqlSAPUserProfileMap = new SQLBulkUploader<SapRoleProfileMapping>(tableName);

                sqlSAPUserProfileMap.Flush(list);


            }
            catch (Exception)
            {
                throw;
            }
        }


        public void ClearSAPUserProfileMap(long EndpointSystemId, long UserId)
        {
            using (_context = new IACMDBContext())
            {
                string query = string.Format("Delete from SAPProfileInfoSAPUserinfo where EndpointSystemId={0} and SAPUserinfo_Id={1}", EndpointSystemId, UserId);
                _context.Database.ExecuteSqlCommand(query);
            }
        }

        public void SaveSAPUserProfileMap(IList<SAPUserProfile> listSAPUserProfile, long EndpointSystemId)
        {
            //using (_context = new IACMDBContext())
            //{
            //    string query = string.Format("Delete from SAPProfileInfoSAPUserinfo where EndpointSystemId={0}", EndpointSystemId);
            //    _context.Database.ExecuteSqlCommand(query);
            //}


            SQLBulkUploader<SAPUserProfile> sqlSAPUserProfileMap = null;
            try
            {
                string tableName = "SAPProfileInfoSAPUserinfo";

                sqlSAPUserProfileMap = new SQLBulkUploader<SAPUserProfile>(tableName);

                sqlSAPUserProfileMap.Flush(listSAPUserProfile);

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlSAPUserProfileMap.Close();
            }

        }
        #endregion

        #region PARAMETERS
        public void SaveSAPUserParameterMap(IList<SAPUserParameter> listSAPUserParameter, long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                string query = string.Format("Delete from SAPParameterInfoSAPUserinfo where EndpointSystemId={0}", EndpointSystemId);
                _context.Database.ExecuteSqlCommand(query);
            }


            SQLBulkUploader<SAPUserParameter> sqlSAPUserParameterMap = null;
            try
            {
                string tableName = "SAPParameterInfoSAPUserinfo";

                sqlSAPUserParameterMap = new SQLBulkUploader<SAPUserParameter>(tableName);

                sqlSAPUserParameterMap.Flush(listSAPUserParameter);

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlSAPUserParameterMap.Close();
            }

        }
        #endregion

        #region CompositeChildRoleMAP

        public List<SAPSingleRoleInfo> GetSapSingleRoles(long EndpointSystemId)
        {
            try
            {
                using (_context = new IACMDBContext())
                {
                    return _context.SAPSingleRoleInfos.Where(singlerole => singlerole.EndPointSystemId == EndpointSystemId).ToList();
                }
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
                using (_context = new IACMDBContext())
                {
                    return _context.SAPCompositeRoleInfos.Where(singlerole => singlerole.EndPointSystemId == EndpointSystemId).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SaveCompositeChildRoleMap(IList<SAPCompositeChildRoleMap> listCompositeChildRoleMap, long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                string query = string.Format("Delete from SAPSingleRoleInfoSAPCompositeRoleInfo where EndpointSystemId={0}", EndpointSystemId);
                _context.Database.ExecuteSqlCommand(query);
            }


            SQLBulkUploader<SAPCompositeChildRoleMap> sqlSAPCompositeChildRoleMap = null;
            try
            {
                string tableName = "SAPSingleRoleInfoSAPCompositeRoleInfo";

                sqlSAPCompositeChildRoleMap = new SQLBulkUploader<SAPCompositeChildRoleMap>(tableName);

                sqlSAPCompositeChildRoleMap.Flush(listCompositeChildRoleMap);

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlSAPCompositeChildRoleMap.Close();
            }

        }
        #endregion

        #region ROLES

        public void ClearSAPUserRoleMap(long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                string query = string.Format("Delete from SAPRoleInfoSAPUserinfo where EndpointSystemId={0}", EndpointSystemId);
                _context.Database.ExecuteSqlCommand(query);
            }
        }

        public void ClearSAPUserRoleMap(long EndpointSystemId, long UserId)
        {
            using (_context = new IACMDBContext())
            {
                string query = string.Format("Delete from SAPRoleInfoSAPUserinfo where EndpointSystemId={0} and SAPUserinfo_Id={1}", EndpointSystemId, UserId);
                _context.Database.ExecuteSqlCommand(query);
            }
        }

        public void SaveSAPUserRoleMap(IList<SAPUserRole> listSAPUserRole, long EndpointSystemId)
        {
            //using (_context = new IACMDBContext())
            //{
            //    string query = string.Format("Delete from SAPRoleInfoSAPUserinfo where EndpointSystemId={0}", EndpointSystemId);
            //    _context.Database.ExecuteSqlCommand(query);
            //}


            SQLBulkUploader<SAPUserRole> sqlSAPUserRoleMap = null;
            try
            {
                string tableName = "SAPRoleInfoSAPUserinfo";

                sqlSAPUserRoleMap = new SQLBulkUploader<SAPUserRole>(tableName);

                sqlSAPUserRoleMap.Flush(listSAPUserRole);

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlSAPUserRoleMap.Close();
            }

        }



        public void SaveSAPSingleRoleTcodeMap(IList<SapSingleRoleTcodes> listSAPSingleRoleTcodes, long EndpointSystemId, bool ClearAllFirst)
        {
            using (_context = new IACMDBContext())
            {
                if (ClearAllFirst)
                {
                    string query = string.Format("Delete from SAPTCodeInfoSAPSingleRoleInfo where EndpointSystemId={0}", EndpointSystemId);
                    _context.Database.ExecuteSqlCommand(query);
                }
                else
                {
                    var UniqueSingleRoles =
                                    from roletcode in listSAPSingleRoleTcodes
                                    join roleinfo in _context.SAPSingleRoleInfos
                                    on roletcode.SAPSingleRoleInfo_Id equals roleinfo.Id
                                    select roleinfo;

                    UniqueSingleRoles = UniqueSingleRoles.Distinct().ToList();
                    foreach (SAPSingleRoleInfo singlerole in UniqueSingleRoles)
                    {
                        string query = string.Format("Delete from SAPTCodeInfoSAPSingleRoleInfo where EndpointSystemId={0} and SAPSingleRoleInfo_Id={1}", EndpointSystemId, singlerole.Id);
                        _context.Database.ExecuteSqlCommand(query);

                    }

                }
            }

            SQLBulkUploader<SapSingleRoleTcodes> sqlSAPSingleRoleTcodeMap = null;
            try
            {
                string tableName = "SAPTCodeInfoSAPSingleRoleInfo";

                sqlSAPSingleRoleTcodeMap = new SQLBulkUploader<SapSingleRoleTcodes>(tableName);

                sqlSAPSingleRoleTcodeMap.Flush(listSAPSingleRoleTcodes);

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlSAPSingleRoleTcodeMap.Close();
            }

        }

        #endregion

        public void AddSingleRoleTCodes(DataTable dtAlltcode, long EndpointSystemId, IList<string> DeltaRoles, bool IsFirstTimeDiscovery)
        {
            IList<SapSingleRoleTcodes> listSingleRoleTcodes = new List<SapSingleRoleTcodes>();
            try
            {
                using (_context = new IACMDBContext())
                {
                    SAPTCodeInfo objSAPTCodeInfo = null;
                    SAPSingleRoleInfo objSingleRoleInfo = null;
                    SapSingleRoleTcodes objSapSingleRoleTcodeMap = null;
                    List<SAPTCodeInfo> lstTCodes = _context.SAPTCodeInfos.Where(tcode => tcode.EndPointSystemId == EndpointSystemId).ToList();
                    List<SAPSingleRoleInfo> lstSapRoles = _context.SAPSingleRoleInfos.Where(role => role.EndPointSystemId == EndpointSystemId).ToList();

                    for (int RowNo = 0; RowNo < dtAlltcode.Rows.Count; RowNo++)
                    {
                        string TCode = dtAlltcode.Rows[RowNo]["LOW"].ToString().Trim();
                        string RoleName = dtAlltcode.Rows[RowNo]["AGR_Name"].ToString().Trim();

                        bool DiscoverRoleTCode = true;
                        if (!IsFirstTimeDiscovery)
                        {
                            DiscoverRoleTCode = DeltaRoles.FirstOrDefault(deltarole => deltarole.Trim() == RoleName.Trim()) != null;
                        }
                        if (DiscoverRoleTCode)
                        {
                            objSAPTCodeInfo = new SAPTCodeInfo();
                            objSAPTCodeInfo = lstTCodes.FirstOrDefault(tcode => tcode.TCODE_Code == TCode && tcode.EndPointSystemId == EndpointSystemId);

                            objSingleRoleInfo = new SAPSingleRoleInfo();
                            objSingleRoleInfo = lstSapRoles.FirstOrDefault(role => role.SingleRole_Code == RoleName && role.EndPointSystemId == EndpointSystemId);


                            if (objSingleRoleInfo != null && objSAPTCodeInfo != null)
                            {
                                SapSingleRoleTcodes objSingleRoleTCode = listSingleRoleTcodes.FirstOrDefault
                                                (
                                                  roletcode => roletcode.SAPSingleRoleInfo_Id == objSingleRoleInfo.Id
                                                  && roletcode.SAPTCodeInfo_Id == objSAPTCodeInfo.Id
                                                  && roletcode.EndpointSystemId == EndpointSystemId
                                                );
                                if (objSingleRoleTCode == null)
                                {
                                    objSapSingleRoleTcodeMap = new SapSingleRoleTcodes();
                                    objSapSingleRoleTcodeMap.SAPTCodeInfo_Id = objSAPTCodeInfo.Id;
                                    objSapSingleRoleTcodeMap.SAPSingleRoleInfo_Id = objSingleRoleInfo.Id;
                                    objSapSingleRoleTcodeMap.EndpointSystemId = EndpointSystemId;
                                    objSapSingleRoleTcodeMap.Version = Convert.ToInt16(VersionNo.Available);
                                    listSingleRoleTcodes.Add(objSapSingleRoleTcodeMap);
                                }
                            }
                        }

                    }
                }
                SaveSAPSingleRoleTcodeMap(listSingleRoleTcodes, EndpointSystemId, IsFirstTimeDiscovery);

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion


        #region Positions

        public SAPPosition GetPositionByPosCode(string posCode, long EndpointSystemId)
        {
            using (_context = new IACMDBContext())
            {
                return _context.SAPPositions.FirstOrDefault(t => t.Code == posCode && t.EndPointSystemId == EndpointSystemId);
            }
        }

        public void AddPosition(SAPPosition pos)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPPositions.Add(pos);
                _context.SaveChanges();
            }
        }
        public void UpdatePosition(SAPPosition pos)
        {
            using (_context = new IACMDBContext())
            {
                _context.SAPPositions.Attach(pos);
                _context.Entry(pos).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        #endregion


        public IList<SAPUserHistoryInfo> GetAllSAPHistoryUsers()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPUserHistoryinfos.ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Fetching All SAP History Users", ex);
                throw ex;
            }
        }

        public IList<EndpointObjects> GetSAPObjects()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    EndPointSystemType ep = _context.EndpointSystemTypes.Where(e => e.EndpointSystemTypeName == EndpointType.SAP).FirstOrDefault();
                    return _context.EndpointObjects.Where(e => e.EndpointSystemTypeId == ep.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All SAP EndpointObjects ", ex);
                throw ex;
            }
        }

        public IList<SAPUserHistoryInfo> getSAPUserHistoryInfo(long endpointSystemId)
        {

            IList<SAPUserHistoryInfo> sapUsers = new List<SAPUserHistoryInfo>();
            using (IACMDBContext _context = new IACMDBContext())
            {
                return _context.SAPUserHistoryinfos.Where(item => item.EndPointSystemId == endpointSystemId).ToList();
            }
        }

        public IList<SAPUserReportData> GetAllSAPUsersReportData(long EndpointSystemId)
        {
            try
            {
                IList<SAPUserReportData> sapUserReportData = new List<SAPUserReportData>();
                SAPUserReportData sapUserReportItem = new SAPUserReportData();
                IList<SAPUserinfo> sapUsers = new List<SAPUserinfo>();

                using (IACMDBContext _context = new IACMDBContext())
                {
                    sapUsers = GetAllSAPUserInfo(EndpointSystemId, "*");

                    foreach (SAPUserinfo usr in sapUsers)
                    {
                        User user = new User();
                        string displayname = "";
                        user = _context.Users.FirstOrDefault(usrs => usrs.Id == usr.DIUserId);
                        if (user != null)
                            displayname = user.DisplayName;
                        sapUserReportItem = new SAPUserReportData();
                        sapUserReportItem.UserId = usr.Id;
                        sapUserReportItem.UserLogonName = usr.UserLogonName;
                        sapUserReportItem.DisplayName = string.Format("{0} {1}", usr.Firstname, usr.Lastname);
                        sapUserReportItem.Firstname = usr.Firstname;
                        sapUserReportItem.Lastname = usr.Lastname;
                        sapUserReportItem.Department = usr.Department;
                        sapUserReportItem.costcenter = usr.CostCenter;
                        sapUserReportItem.IsLocked = usr.IsLocked ? "Yes" : "No";
                        sapUserReportItem.LockScenario = usr.LockScenario;
                        sapUserReportItem.costcenter = usr.CostCenter;
                        sapUserReportItem.Pers_No = usr.Pers_No;
                        sapUserReportItem.PasswordExpirationDate = usr.PASSWORDEXPTIME.HasValue ? usr.PASSWORDEXPTIME.Value.ToString() : string.Empty;
                        sapUserReportItem.Function0 = usr.Function0;
                        if (usr.LastLoginTime != null)
                        {
                            sapUserReportItem.lastLogon = usr.LastLoginTime.Value;
                        }
                        sapUserReportItem.IsActive = usr.IsActive ? "Yes" : "No";
                        sapUserReportItem.E_Mail = usr.E_Mail;
                        sapUserReportItem.HostName = usr.EndPointSystem.DisplayName;
                        sapUserReportItem.Role = "";
                        sapUserReportItem.IsUserLocked = usr.IsUserLocked;
                        sapUserReportItem.UserType = usr.SapUserType;
                        if (usr.validFromDate != null)
                            sapUserReportItem.StartDate = usr.validFromDate.Value;
                        if (usr.validToDate != null)
                            sapUserReportItem.EndDate = usr.validToDate.Value;
                        sapUserReportItem.IsMapped = (usr.DIUserId != null) ? "Yes" : "No";
                        sapUserReportItem.Owner = (usr.DIUserId != null) ? displayname : "";
                        sapUserReportData.Add(sapUserReportItem);
                    }
                    return sapUserReportData;
                }
            }

            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Fetching All Users", ex);
                throw ex;
            }
        }

        public IList<SAPUsersToProfilesReportData> GetSAPUserToProfilesReportData(long EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return (from sapUserprofile in _context.SapUserProfile
                            where sapUserprofile.EndPointSystemId == EndpointSystemId
                            select new SAPUsersToProfilesReportData
                            {
                                UserName = sapUserprofile.UserName,
                                ProfileCode = sapUserprofile.ProfileName


                            }
                            )
                             .ToList();



                }


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


                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from sapProfile in _context.SAPProfileInfos
                            where sapProfile.EndPointSystemId == EndpointSystemId
                            select new SAPProfileReportData

                            {
                                PROFILE_CODE = sapProfile.PROFILE_CODE,
                                PROFILE_DESC = sapProfile.PROFILE_DESC

                            }

                    )
                    .ToList();




                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<SAPGroupReportData> GetAllSAPGroupsReportData(long EndpointSystemId)
        {

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from sapGroup in _context.SAPGroupInfos
                            where sapGroup.EndPointSystemId == EndpointSystemId
                            select new SAPGroupReportData

                            {
                                GROUP_CODE = sapGroup.GROUP_CODE,
                                GROUP_DESC = sapGroup.GROUP_DESC
                            }
                    )
                    .ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from sapRole in _context.SAPRoleInfos
                            where sapRole.EndPointSystemId == EndpointSystemId
                            select new SAPRoleReportData

                            {
                                ROLE_CODE = sapRole.ROLE_CODE,
                                ROLE_DESC = sapRole.ROLE_DESC
                            }
                    )
                    .ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    string query = string.Format("select distinct roleauth.SAPRoleToAuthMapId, roleauth.RoleName, roleauth.AuthCode,  roleauth.ObjectCode, roleauth.FieldCode, case when fieldvalue.FromValue is not null then fieldvalue.FromValue else roleauth.fromvalue end as FromValue, roleauth.ToValue from DI_SAP_RoleToAuthMap roleauth  left join DI_SAP_FIELDVALUEMAPPING fieldvalue on roleauth.FromValue = fieldvalue.FieldCode and roleauth.rolename = fieldvalue.RoleCode and left(roleauth.fromvalue, 1) = '$' and fieldvalue.EndpointSystemId={0}	where roleauth.endpointsystemid = {0} and roleauth.ProfileGeneratorVariant != 'X'", EndpointSystemId);
                    var RoleAuth = _context.Database.SqlQuery<SapRoleToAuthorizationReportData>(query);

                    return RoleAuth.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SapRoleToAuthorizationReportData> GetSapRolesToAuthorizationData(string RoleCode, long EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    string query = string.Format("select distinct roleauth.SAPRoleToAuthMapId, roleauth.RoleName, roleauth.AuthCode,  roleauth.ObjectCode, roleauth.FieldCode, case when fieldvalue.FromValue is not null then fieldvalue.FromValue else roleauth.fromvalue end as FromValue, roleauth.ToValue from DI_SAP_RoleToAuthMap roleauth  left join DI_SAP_FIELDVALUEMAPPING fieldvalue on roleauth.FromValue = fieldvalue.FieldCode and roleauth.rolename = fieldvalue.RoleCode and left(roleauth.fromvalue, 1) = '$'	where roleauth.endpointsystemid = {0} and roleauth.RoleName = '{1}' and roleauth.ObjectCode != 'S_TCODE' and roleauth.ProfileGeneratorVariant != 'X'", EndpointSystemId, RoleCode);
                    var RoleAuth = _context.Database.SqlQuery<SapRoleToAuthorizationReportData>(query);

                    return RoleAuth.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SAPTCodeInfo> GetSapRolesToTCodeData(string RoleCode, long EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from RoleToAuth in _context.SapRoleToAuthMap
                            join TCodes in _context.SAPTCodeInfos on RoleToAuth.FromValue equals TCodes.TCODE_Code
                            where (RoleToAuth.RoleName == RoleCode && RoleToAuth.ObjectCode == "S_TCODE"
                                && RoleToAuth.FieldCode == "TCD" && TCodes.EndPointSystemId == EndpointSystemId
                                && RoleToAuth.EndPointSystemId == EndpointSystemId)
                            select TCodes
                    ).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SAPRoleToAuthMap> GetTCodesBySAPRole(string RoleCode, long EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from RoleToAuth in _context.SapRoleToAuthMap
                            where (RoleToAuth.RoleName == RoleCode && RoleToAuth.ObjectCode == "S_TCODE"
                                && RoleToAuth.FieldCode == "TCD" && RoleToAuth.EndPointSystemId == EndpointSystemId && RoleToAuth.ProfileGeneratorVariant != "X")
                            select RoleToAuth
                    ).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<string> GetTCodesByPattern(string pattern, long endpointSystemId)
        {
            try
            {
                pattern = pattern.Replace("*", "%");
                //SELECT * FROM DI_SAP_TCODEINFO WHERE EndPointSystemId = 16 AND TCODE_Code LIKE 'pattern'
                string query = string.Format("SELECT * FROM [dbo].[DI_SAP_TCODEINFO] WHERE EndPointSystemId = {0} AND TCODE_Code LIKE '{1}'", endpointSystemId, pattern);
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Database.SqlQuery<SAPTCodeInfo>(query).Select(item => item.TCODE_Code).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<string> GetTCodesByRange(string fromValue, string toValue, long endpointSystemId)
        {
            try
            {
                //SELECT * FROM DI_SAP_TCODEINFO WHERE EndPointSystemId = 16 AND TCODE_Code BETWEEN 'SU51' AND 'SU56' ORDER BY TCODE_Code
                string query = string.Format("SELECT * FROM [dbo].[DI_SAP_TCODEINFO] WHERE EndPointSystemId = {0} AND TCODE_Code BETWEEN '{1}' AND '{2}'", endpointSystemId, fromValue, toValue);
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Database.SqlQuery<SAPTCodeInfo>(query).Select(item => item.TCODE_Code).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<SAPTCodeInfo> GetSapRolesToTCodeDataWithRoles(string RoleCode, long EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    List<SAPTCodeInfo> SAPTCodeInfo = (from RoleToAuth in _context.SapRoleToAuthMap
                                                       join TCodes in _context.SAPTCodeInfos on RoleToAuth.FromValue equals TCodes.TCODE_Code
                                                       where (RoleToAuth.RoleName == RoleCode && RoleToAuth.ObjectCode == "S_TCODE"
                                                           && RoleToAuth.FieldCode == "TCD" && TCodes.EndPointSystemId == EndpointSystemId)
                                                       select TCodes
                    ).ToList();
                    List<SAPTCodeInfo> ResultTCodeInfo = new List<SAPTCodeInfo>();
                    foreach (SAPTCodeInfo tcode in SAPTCodeInfo)
                    {
                        tcode.Role = RoleCode;
                        ResultTCodeInfo.Add(tcode);
                    }

                    return ResultTCodeInfo;
                }

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
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SapRoleTCodeMap
                        .Where(roletcode => roletcode.EndPointSystemId == EndpointSystemId)
                        .ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from sapTcode in _context.SAPTCodeInfos
                            where sapTcode.EndPointSystemId == EndpointSystemId
                            select new SAPTCodeReportData

                            {
                                TCODE_Code = sapTcode.TCODE_Code,
                                TCODE_Desc = sapTcode.TCODE_Desc

                            }
                    )
                    .ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    string query = string.Format("Delete From DI_SAP_SAPHRP1001 WHere EndpointSystemId = {0}", endpointSystemId);
                    _context.Database.ExecuteSqlCommand(query);
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.SAPHRP1001Entries.Add(hr1001);
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var taskEntry = _context.Entry(hr1001);
                    if (taskEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<SAPHRP1001>();
                        SAPHRP1001 attachedTask = set.Find(hr1001.Id);

                        if (attachedTask != null)
                        {
                            var attachedEntry = _context.Entry(attachedTask);
                            attachedEntry.CurrentValues.SetValues(hr1001);
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

        public SAPHRP1001 GetHRP1001Dump(SAPHRP1001 hr1001)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPHRP1001Entries.FirstOrDefault(item => item.ActualObjectCode == hr1001.ActualObjectCode &&
                        item.EndpointSystemId == hr1001.EndpointSystemId && item.RelatedObjectCode == hr1001.RelatedObjectCode);
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var itemToRemove = _context.SAPHRP1001Entries.FirstOrDefault(item => item.ActualObjectCode == hr1001.ActualObjectCode &&
                        item.EndpointSystemId == hr1001.EndpointSystemId && item.RelatedObjectCode == hr1001.RelatedObjectCode);

                    if (itemToRemove != null)
                    {
                        _context.SAPHRP1001Entries.Remove(itemToRemove);
                        _context.SaveChanges();
                    }
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPHRP1001Entries.Where(item => item.EndpointSystemId == endpointSystemId && item.ActualObjectCode == position &&
                                    item.RelatedObjectType == "AG").Select(role => role.RelatedObjectCode).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPHRP1001Entries.Where(item => item.EndpointSystemId == endpointSystemId && item.RelatedObjectCode == logonName &&
                                    item.RelatedObjectType == "US").Select(role => role.ActualObjectCode).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return (from hrp001 in _context.SAPHRP1001Entries
                            join hrp001Text in _context.SAPPositionsMatrices on hrp001.ActualObjectCode equals hrp001Text.PositionCode
                            where hrp001.EndpointSystemId == endpointSystemId && hrp001.RelatedObjectCode == logonName &&
                                    hrp001.RelatedObjectType == "US" && hrp001Text.ValidFrom <= DateTime.Now && hrp001Text.ValidTo >= DateTime.Now
                            select hrp001Text)
                            .Select(position => new { key = position.Id, value = position.PositionText })
                            .Distinct().ToDictionary(dictionary => (long)dictionary.key, dictionary => dictionary.value);
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPEPUserInfos.FirstOrDefault(sapepuser => sapepuser.Id == UserId);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SAPUserinfo> GetSAPUsersToAssignEPGroup(long endpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPUserinfos.Where(item => item.NeedEPGroupAssignment && item.EndPointSystemId == endpointSystemId).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateAccountDetail(AccountDetail AccountDetail)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                SAPUserinfo SAPUInfo = Context.SAPUserinfos.FirstOrDefault(ENDPT => ENDPT.Id == AccountDetail.EndpointUserId && ENDPT.EndPointSystemId == AccountDetail.EndpointSystemId);
                if (SAPUInfo != null)
                {
                    SAPUInfo.DIUserId = AccountDetail.DIUserID; ;
                    SAPUInfo.LastModifiedDate = DateTime.Today;
                    SAPUInfo.MappingType = "Manual";
                    SAPUInfo.AccountType = AccountDetail.AccountType;
                    SAPUInfo.IfUnmappedMapTo = AccountDetail.UnMappedMapTo;
                    Context.SaveChanges();
                }
            }
        }

        public SAPUserinfo GetSAPUserInfo(long EndpointUserId)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                return Context.SAPUserinfos.Include("SAPRoleInfo").FirstOrDefault(sapuser => sapuser.Id == EndpointUserId);
            }
        }

        public IList<AttestationReviewPageSAP_Level1GridData> GetAuthorization(string RoleCode, long EndPointSystemId)
        {
            IList<AttestationReviewPageSAP_Level1GridData> returnValue = new List<AttestationReviewPageSAP_Level1GridData>();
            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {

                    List<SapRoleToAuthorizationReportData> RoleToAuthorizationData = new SAPConnectorDA().GetSapRolesToAuthorizationData(RoleCode, EndPointSystemId);
                    if (RoleToAuthorizationData.Count > 0)
                    {
                        returnValue = (from Authorization in RoleToAuthorizationData
                                       select new AttestationReviewPageSAP_Level1GridData
                                       {
                                           sapLevel1DetailId = "Authorization;" + Authorization.AuthCode + ";" + EndPointSystemId,
                                           sapLevel1Detail = string.Format("Authorization Code: {0} Object: {1}", Authorization.AuthCode, Authorization.ObjectCode),
                                           sapLevel1DetailDesc = string.Format("Field: {0} From Value: {1}", Authorization.FieldCode, Authorization.FromValue),
                                           sapLevel1DetailType = "Auhtorization"
                                       }).ToList();
                    }
                }
            }
            catch (Exception)
            {

            }
            return returnValue;
        }

        //public IList<AccessRequestItem> GetAccessRequestItem(string searchString, AccessRequestEntitlementType entType)
        //{

        //}
        public List<AccessRequestItem> GetAccessRequestItem(string searchString, bool isCatalog)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                if (isCatalog)
                {
                    return (from catalogItem in Context.SAPRoleCatalogs
                            where catalogItem.BusinessRole.Contains(searchString)
                            orderby catalogItem.BusinessRole
                            select new AccessRequestItem
                            {
                                AccessRequestItemType = AccessRequestItemType.Entitlement,
                                AccessItemType = Model.AccessItemType.Catalog,
                                EndpointType = EndpointType.ActiveDirectory,
                                KeyId = catalogItem.Id,
                                EndpointSystemId = catalogItem.EndpointSystemId,
                                EndpointSystemTypeId = (int)EndpointType.SAP,
                                ItemName = catalogItem.BusinessRole,
                                ItemDetail = catalogItem.TechnicalRole,
                                ItemDescription = catalogItem.Description
                            }).ToList();
                }

                return (from roles in Context.SAPRoleInfos
                 where roles.ROLE_CODE.Contains(searchString)
                        orderby roles.ROLE_CODE
                 select new AccessRequestItem
                 {
                     AccessRequestItemType = AccessRequestItemType.Entitlement,
                     EntitlementType = Infrastructure.Enums.EntitlementType.Role,
                     EndpointType = EndpointType.SAP,
                     KeyId = roles.Id,
                     EndpointSystemId = roles.EndPointSystemId,
                     EndpointSystemTypeId = (int)EndpointType.SAP,
                     ItemName = roles.ROLE_CODE,
                     ItemDescription = roles.ROLE_DESC,
                     ItemDetail=roles.ROLE_DESC
                 }).ToList();
            }
        }

        public bool IsSecondartAccount(long EndpointUserID)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                return _context.SAPUserinfos.Where(item => item.DIUserId == EndpointUserID && item.AccountType == AccountType.User).Count() > 0 ? true : false;
            }
        }

        public List<AccessRequestItem> GetAccessRequestItemForRemoveRequest(string searchString, long? DIUserId)
        {
            using (IACMDBContext Context = new IACMDBContext())
            {
                if (DIUserId == null)
                    return null;

                var listApp = GetSAPUserInfoByDIUserId(DIUserId.Value);

                var listEnt = GetSAPRolesByDIUserId(DIUserId.Value);


                var listARApp = (from apps in listApp
                                 where apps.UserLogonName.Contains(searchString)
                                 orderby apps.UserLogonName
                                 select new AccessRequestItem
                                 {
                                     AccessRequestItemType = AccessRequestItemType.Application,
                                     EndpointType = EndpointType.SAP,
                                     KeyId = apps.EndPointSystemId,
                                     EndpointSystemId = apps.EndPointSystemId,
                                     EndpointSystemTypeId = (int)EndpointType.SAP,
                                     ItemName = apps.UserLogonName,
                                     ItemDescription = apps.Lastname
                                 }).ToList();

                var listAREnt = (from groups in listEnt
                                 where groups.ROLE_CODE.Contains(searchString)
                                 orderby groups.ROLE_CODE
                                 select new AccessRequestItem
                                 {
                                     AccessRequestItemType = AccessRequestItemType.Entitlement,
                                     EndpointType = EndpointType.SAP,
                                     KeyId = groups.Id,
                                     EndpointSystemId = groups.EndPointSystemId,
                                     EndpointSystemTypeId = (int)EndpointType.SAP,
                                     ItemName = groups.ROLE_CODE,
                                     ItemDescription = groups.ROLE_DESC
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

                var listApp = GetSAPUserInfoByDIUserId(DIUserId.Value, Criteria.AccountType);

                var listEnt = GetSAPRolesByDIUserId(DIUserId.Value, Criteria.AccountType);


                var listARApp = (from apps in listApp
                                 where apps.UserLogonName.Contains(Criteria.SearchKey)
                                 orderby apps.UserLogonName
                                 select new AccessRequestItem
                                 {
                                     AccessRequestItemType = AccessRequestItemType.Application,
                                     EndpointType = EndpointType.SAP,
                                     KeyId = apps.EndPointSystemId,
                                     EndpointSystemId = apps.EndPointSystemId,
                                     EndpointSystemTypeId = (int)EndpointType.SAP,
                                     ItemName = apps.UserLogonName,
                                     ItemDescription = apps.Lastname
                                 }).ToList();

                var listAREnt = (from groups in listEnt
                                 where groups.ROLE_CODE.Contains(Criteria.SearchKey)
                                 orderby groups.ROLE_CODE
                                 select new AccessRequestItem
                                 {
                                     AccessRequestItemType = AccessRequestItemType.Entitlement,
                                     EndpointType = EndpointType.SAP,
                                     KeyId = groups.Id,
                                     EndpointSystemId = groups.EndPointSystemId,
                                     EndpointSystemTypeId = (int)EndpointType.SAP,
                                     ItemName = groups.ROLE_CODE,
                                     ItemDescription = groups.ROLE_DESC
                                 }).ToList();

                return listARApp.Union(listAREnt).ToList();

            }
        }

        public List<AccountDetail> GetPwdExpiryUsers()
        {
            List<AccountDetail> Returnvalue = new List<AccountDetail>();
            using (IACMDBContext _context = new IACMDBContext())
            {
                var ADSUserInfo = _context.SAPUserinfos.Include(l => l.EndPointSystem).Where(item => item.PASSWORDEXPTIME <= DateTime.Now && item.DIUserId > 0).ToList();
                ADSUserInfo.ForEach(lq =>
                {
                    Returnvalue.Add(new AccountDetail()
                    {
                        LoginName = lq.AccountName,
                        DisplayName = lq.Firstname + " " + lq.Lastname,
                        PhoneNo = lq.Pers_No,
                        Email = lq.E_Mail,
                        AccountType = lq.AccountType,
                        IsMappedAccount = lq.DIUserId == null ? false : true,
                        LastLogonDate = lq.LastLoginTime,
                        Version = Convert.ToString(lq.Version),
                        DIUserID = lq.DIUserId,
                        EndpointSystemId = lq.EndPointSystemId,
                        EndpointUserId = lq.Id,
                        ApplicationType = Convert.ToString(EndpointType.SAP),
                        Application = lq.EndPointSystem.DisplayName
                    });
                });
            }
            return Returnvalue;
        }
    }
}
        #endregion

