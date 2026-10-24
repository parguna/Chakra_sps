using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using DI.Chakra.Model.CustomEntities;
using DI.Chakra.Model.SAP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using DI.Chakra.Model.ReportData;
using DI.Chakra.Logger;
using System.Collections;
using System.Globalization;
using DI.Chakra.Model.Core;


namespace DI.Chakra.DataAccess
{
    public class UserDA// : IUserDA
    {

        IACMDbOperator iacmOperator;
        IACMDBContext oIACMDBContext = new IACMDBContext();//---*G TenantMgmt

        QueryExecutor executor;

        #region INSERT DATA INTO 2nd LEVEL ATTESTATION
        #region IUserDA Members

        public User GetUser(long _userId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.Users.Include("BusinessRole").Include("Organisation").FirstOrDefault(usr => usr.Id == _userId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public User GetUserwithEndpoint(int _userId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.Users.Include("EndpointSystems").FirstOrDefault(usr => usr.Id == _userId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// proxy disabled user object
        /// </summary>
        /// <param name="_userId"></param>
        /// <returns></returns>
        public User GetUserById(int _userId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    oIACMDBContext.Configuration.ProxyCreationEnabled = false;
                    return oIACMDBContext.Users.FirstOrDefault(usr => usr.Id == _userId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public User GetUserByEmployeeId(string employeeid)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    oIACMDBContext.Configuration.ProxyCreationEnabled = false;
                    return oIACMDBContext.Users.FirstOrDefault(usr => usr.EmployeeId == employeeid);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public User GetUserByEmployeeDisplayName(string displayname)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.Users.FirstOrDefault(usr => usr.DisplayName == displayname);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public User GetUserByNRIC(string nric)
        {
            try
            {
                using (oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.Users.FirstOrDefault(item => item.NRIC == nric);
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Getting the Get User By NRIC", ex);
                throw ex;
            }
        }

        public User GetUserByPositionCode(string positionCode)
        {
            try
            {
                using (oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.Users.FirstOrDefault(item => item.UserExtensionObj.PositionCode == positionCode);
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Getting User By position code", ex);
                throw ex;
            }
        }

        public User GetUserByName(string _userName, string _tenantId)
        {
            try
            {
                using (oIACMDBContext = new IACMDBContext())
                {
                    if (!string.IsNullOrEmpty(_tenantId))
                    {
                        return oIACMDBContext.Users.Include("BusinessRole").Include("Roles").FirstOrDefault(usr => usr.UserName == _userName && usr.TenantId == _tenantId);
                    }
                    return oIACMDBContext.Users.Include("BusinessRole").Include("Roles").FirstOrDefault(usr => usr.UserName.Equals(_userName, StringComparison.CurrentCultureIgnoreCase));
                }
            }
            catch (Exception ex)
            {
                //TODO UI/Prov Logger
                // DILog.SchedulerLogger.WriteLog(LogLevelType.Error, "Error Occured while Getting the Get User By Name", ex);
                throw ex;
            }
        }

        public User GetUserByName(string _userName)
        {
            return GetUserByName(_userName, null);
        }

        public IList<string> GetAllBusinessRole(string _tenantId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (string.IsNullOrEmpty(_tenantId))
                    {
                        //temp fix
                        //return _context.Users.Where(user => !string.IsNullOrEmpty(user.BusinessRole)).Select(item => item.BusinessRole).Distinct().ToList();
                        return _context.Designations.Select(item => item.DesignationDesc).ToList();
                    }
                    //temp fix
                    //return _context.Users.Where(user => user.TenantId == _tenantId && !string.IsNullOrEmpty(user.BusinessRole)).Select(item => item.BusinessRole).Distinct().ToList();
                    return _context.Designations.Select(item => item.DesignationDesc).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured in GetAllBusinessRole ", ex);
                throw ex;
            }
        }

        public IList<User> GetAllManagers(string _tenantId)
        {
            IList<User> userList = new List<User>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (null != _tenantId)
                        userList = oIACMDBContext.Users.Where(usr => usr.TenantId == _tenantId &&
                                                                                    usr.ManagerId == usr.Id).ToList();
                    else
                        userList = oIACMDBContext.Users.Where(usr => usr.ManagerId == usr.Id).ToList();
                }
                return userList;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while All Users ", ex);
                throw ex;
            }
        }

        // Change for senoko attestaion usecase
        public IList<User> GetAllManagersForAttestation(string _tenantId)
        {
            //string query = "DI_spGetManagers";
            IList<User> userList = new List<User>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {

                    //from usr in oIACMDBContext.Users
                    //                                join endUser in oIACMDBContext.SAPUserinfos on usr.UserId equals endUser.DIUserId
                    //                                where endUser.EndPointSystemId == endPtId && endUser.DIUserId != null && endUser.DIUserId != -1
                    //                                select new
                    //                                {
                    //                                    usr.UserId,
                    //                                    usr.FirstName,
                    //                                    usr.LastName,
                    //                                    UserLogOnName = endUser.UserLogonName,
                    //                                    usr.EmailId,
                    //                                    usr.DisplayName
                    //                                }).ToList();

                    if (null != _tenantId)
                    {
                        var usrs = from usr1 in oIACMDBContext.Users
                                   join usr2 in oIACMDBContext.Users on usr1.ManagerId equals usr2.Id
                                   where usr1.TenantId == _tenantId && usr1.ManagerId != null && usr1.UserName != ""
                                   orderby usr2.UserName
                                   select usr2;

                        var sectionHeads = from usr1 in oIACMDBContext.Users
                                           join sectionHead in oIACMDBContext.Organisations on usr1.Id equals sectionHead.Head
                                           select usr1;
                        userList = usrs.Union(sectionHeads).Distinct().ToList();



                        //userList = oIACMDBContext.Users.Where(usr => usr.TenantId == _tenantId &&
                        //                                                            usr.ManagerId == usr.UserId)

                        //          .ToList();
                    }
                    else
                    {
                        var usrs = from usr1 in oIACMDBContext.Users
                                   join usr2 in oIACMDBContext.Users on usr1.ManagerId equals usr2.Id
                                   where usr1.ManagerId != null && usr1.UserName != ""
                                   orderby usr2.UserName
                                   select usr2;

                        var sectionHeads = from usr1 in oIACMDBContext.Users
                                           join sectionHead in oIACMDBContext.Organisations on usr1.Id equals sectionHead.Head
                                           select usr1;

                        userList = usrs.Union(sectionHeads).Distinct().ToList();
                    }
                }
                return userList;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while All Users ", ex);
                throw ex;
            }
        }

        public IList<User> GetAllManagers()
        {

            try
            {
                return GetAllManagers(null);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<User> GetAllUsers(string _tenantId)
        {
            //string query = "DI_spGetAllUsers";
            IList<User> userList = new List<User>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (_tenantId != null)
                        userList = oIACMDBContext.Users.Where(usr => usr.TenantId == _tenantId).ToList();
                    else
                        userList = oIACMDBContext.Users.ToList();
                }
                return userList;
            }
            catch
            {
                throw;
            }
        }

        public IList<User> GetAllUsers()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Users.Include("BusinessRole").Where(c => c.FullTimeEmployee != true).ToList();
                }
            }
            catch
            {
                throw;
            }
        }


        public IList<User> GetAllUsers(bool fullTimeEmployee)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (fullTimeEmployee == false)
                        return _context.Users.Include("BusinessRole").Where(c => c.FullTimeEmployee != true).ToList();
                    return _context.Users.Include("BusinessRole").Where(c => c.FullTimeEmployee == true).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<User> GetFullTimeEmployees()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Users.Include("BusinessRole").Where(c => c.FullTimeEmployee == true).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<AllUsersReportData> GetAllUsersReport(string ipSelectType)
        {
            IList<AllUsersReportData> returnList = new List<AllUsersReportData>();

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (ipSelectType == "All")
                    {
                        returnList = (from allUser in _context.Users
                                      where allUser.Id != 1
                                      select new AllUsersReportData
                                      {
                                          UserId = allUser.Id,
                                          UserName = allUser.UserName,
                                          DisplayName = allUser.DisplayName,
                                          OrganisationName = allUser.Organisation.OrganisationName,
                                          EmailId = allUser.EmailId,
                                          Position = allUser.UserExtensionObj.PositionName,
                                          JoinDate = allUser.JoinDate.Value,
                                          firstName = allUser.FirstName,
                                          lastName = allUser.LastName,
                                          tenantId = allUser.TenantId,
                                          IsActive = allUser.IsActive,
                                          TeminaionDate = allUser.TerminationDate
                                      }).OrderBy(usr => usr.firstName).ToList();
                    }
                    else if (ipSelectType == "Employee")
                    {
                        returnList = (from allUser in _context.Users
                                      where allUser.Id != 1 && allUser.FullTimeEmployee.Value
                                      select new AllUsersReportData
                                      {
                                          UserId = allUser.Id,
                                          UserName = allUser.UserName,
                                          DisplayName = allUser.DisplayName,
                                          OrganisationName = allUser.Organisation.OrganisationName,
                                          EmailId = allUser.EmailId,
                                          Position = allUser.UserExtensionObj.PositionName,
                                          JoinDate = allUser.JoinDate.Value,
                                          firstName = allUser.FirstName,
                                          lastName = allUser.LastName,
                                          tenantId = allUser.TenantId,
                                          IsActive = allUser.IsActive,
                                          TeminaionDate = allUser.TerminationDate
                                      }).OrderBy(usr => usr.firstName).ToList();
                    }
                    else if (ipSelectType == "Non Employee")
                    {
                        returnList = (from allUser in _context.Users
                                      where allUser.Id != 1 && allUser.FullTimeEmployee.Value == false
                                      select new AllUsersReportData
                                      {
                                          UserId = allUser.Id,
                                          UserName = allUser.UserName,
                                          DisplayName = allUser.DisplayName,
                                          OrganisationName = allUser.Organisation.OrganisationName,
                                          EmailId = allUser.EmailId,
                                          Position = allUser.UserExtensionObj.PositionName,
                                          JoinDate = allUser.JoinDate.Value,
                                          firstName = allUser.FirstName,
                                          lastName = allUser.LastName,
                                          tenantId = allUser.TenantId,
                                          IsActive = allUser.IsActive,
                                          TeminaionDate = allUser.TerminationDate
                                      }).OrderBy(usr => usr.firstName).ToList();
                    }

                    return returnList;
                }
            }
            catch
            {
                throw;
            }
        }


        public IList<User> GetAllUserOfManager(int managerId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Users.Where(item => item.ManagerId.Value == managerId).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while All Users ", ex);
                throw ex;
            }
        }


        public IList<User> GetAllUserOfOrganisation(object _orgUnit, UserType _userType)
        {
            //string query = "DI_spGetAllUserByOrganisation";
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    IList<User> retList = new List<User>();
                    int orgUnitId = Convert.ToInt32(_orgUnit);
                    if (_userType == UserType.User)
                    {
                        retList = oIACMDBContext.Users.Where(usr => usr.OrganisationId == orgUnitId).ToList();
                    }
                    else
                    {//in case userType = Manager
                        retList = oIACMDBContext.Users.Where(usr => usr.OrganisationId == orgUnitId &&
                                                                                             usr.ManagerId == usr.Id).ToList();
                    }
                    return retList;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IList<User> GetAllUsersOfCompany(string CompanyCode)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.Users.Where(user => user.UserExtensionObj.CompanyCode == CompanyCode).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<User> GetAllUsersOfSection(string SectionCode)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.Users.Where(user => user.UserExtensionObj.SectionCode == SectionCode).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<User> GetAllUserOfGroup(long _groupId, UserType _userType)
        {
            //string query = "DI_spGetGlobalUsersByGroup";            
            IList<User> userList = new List<User>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    userList = oIACMDBContext.Groups.FirstOrDefault(grp => grp.Id == _groupId).Users.ToList();
                    return userList;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while All Users ", ex);
                throw ex;
            }
        }
        private IList<User> getUsers(IDataReader _reader)
        {
            IList<User> _list = new List<User>();

            User user = null;

            try
            {
                if (_reader == null)
                    return null;


                while (_reader.Read())
                {
                    user = new User();

                    user.Id = CheckNull.AgainstInt(_reader["UserId"]);
                    user.NRIC = CheckNull.AgainstString(_reader["NRIC"]);
                    user.FIN = CheckNull.AgainstString(_reader["FIN"]);
                    user.PassportNo = CheckNull.AgainstString(_reader["PassportNo"]);
                    user.UserName = CheckNull.AgainstString(_reader["UserName"]);
                    user.Password = CheckNull.AgainstString(_reader["Password"]);
                    user.TenantId = CheckNull.AgainstString(_reader["TenantId"]);
                    user.FirstName = CheckNull.AgainstString(_reader["FirstName"]);
                    user.MiddleName = CheckNull.AgainstString(_reader["MiddleName"]);
                    user.LastName = CheckNull.AgainstString(_reader["LastName"]);
                    user.DisplayName = CheckNull.AgainstString(_reader["DisplayName"]);
                    user.EmployeeId = CheckNull.AgainstString(_reader["EmployeeId"]);
                    user.OrganisationId = CheckNull.AgainstInt(_reader["organisationId"]);
                    user.FullTimeEmployee = CheckNull.AgainstBit(_reader["FullTimeEmployee"]);
                    user.EmailId = CheckNull.AgainstString(_reader["EmailId"]);
                    user.Country = CheckNull.AgainstString(_reader["Country"]);
                    user.Ethinicity = CheckNull.AgainstString(_reader["Ethinicity"]);
                    user.TelephoneNumber = CheckNull.AgainstString(_reader["TelephoneNumber"]);
                    user.HomephoneNumber = CheckNull.AgainstString(_reader["HomephoneNumber"]);
                    user.MobileNumber = CheckNull.AgainstString(_reader["MobileNumber"]);
                    user.JoinDate = CheckNull.AgainstNullableDatetime(_reader["JoinDate"]);
                    user.RehireDate = CheckNull.AgainstNullableDatetime(_reader["RehireDate"]);
                    user.IsActive = CheckNull.AgainstBit(_reader["IsActive"]);
                    //user.EmployeeStatus = CheckNull.AgainstString(_reader["EmployeeStatus"]);
                    user.TerminationDate = CheckNull.AgainstNullableDatetime(_reader["TerminationDate"]);
                    user.CreatedBy = CheckNull.AgainstString(_reader["CreatedBy"]);
                    user.CreatedDate = CheckNull.AgainstDatetime(_reader["CreatedDate"]);
                    user.LastModifiedBy = CheckNull.AgainstString(_reader["LastModifiedBy"]);
                    user.LastModifiedDate = CheckNull.AgainstDatetime(_reader["LastModifiedDate"]);
                    _list.Add(user);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return _list;
        }


        #region Master BulkOperation

        public void AddMasterUsersBulk(IList<User> users)
        {
            SQLBulkUploader<User> sql = null;
            try
            {
                string tableName = "DI_M_User";

                sql = new SQLBulkUploader<User>(tableName);

                sql.Flush(users);


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }
        }

        public void UpdateMasterLatest(DataTable dtMaster, string tenantId)
        {
            try
            {
                int noAffected = -1;
                string strQuery = string.Empty;

                using (TransactionScope tran = new TransactionScope())
                {

                    if (null != tenantId)
                    {
                        strQuery = string.Format("DELETE {0} WHERE TenantId='{1}'", "DI_M_User", tenantId);
                    }
                    else
                        strQuery = string.Format("DELETE {0}", "DI_M_User");

                    iacmOperator = new IACMDbOperator(strQuery, CommandType.Text, null);

                    executor = new QueryExecutor(iacmOperator);

                    noAffected = executor.ExecuteQuery();

                    iacmOperator.Close();

                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Deleted {0} records from table {1}",
                                                                    noAffected,
                                                                    "DI_M_User"));



                    SQLBulkUploader<User> sqlUser = null;

                    string tableName = "DI_M_User";

                    sqlUser = new SQLBulkUploader<User>(tableName);

                    sqlUser.Flush(dtMaster);

                    tran.Complete();

                    tran.Dispose();
                }


            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion



        #region Tenant
        //public List<Tenant> GetAllTenant()
        //{
        //    List<Tenant> Tenantlist = new List<Tenant>();

        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //            Tenantlist = oIACMDBContext.Tenants.ToList();
        //        return Tenantlist;
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while Getting the Get All Teanant", ex);
        //        throw ex;
        //    }
        //}
        #endregion

        public void UpdateUser(User user)
        {
            //string query = "DI_spUpdateUser";
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var taskEntry = _context.Entry(user);
                    if (taskEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<User>();
                        User attachedTask = set.Find(user.Id);

                        if (attachedTask != null)
                        {
                            var attachedEntry = _context.Entry(attachedTask);
                            attachedEntry.CurrentValues.SetValues(user);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Updating the Update User");
                throw;
            }
        }


        //public void SaveUserImage(UserImage _UserImage)
        //{
        //    try
        //    {
        //        using (IACMDBContext Context = new IACMDBContext())
        //        {
        //            UserImage editImg = Context.UserImages.Where(umg => umg.Id == _UserImage.Id).FirstOrDefault();
        //            if (editImg == null)
        //            {
        //                Context.UserImages.Attach(_UserImage);
        //                Context.Entry(_UserImage).State = EntityState.Added;
        //                Context.SaveChanges();
        //            }
        //            else
        //            {
        //                Context.Entry(editImg).CurrentValues.SetValues(_UserImage);
        //                Context.SaveChanges();
        //            }                    
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while Saving the User Profile Image", ex);
        //        throw ex;
        //    }
        //}
        //public UserImage GetProfileByUserId(int _userId)
        //{
        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            return oIACMDBContext.UserImages.FirstOrDefault(usr => usr.UserId == _userId);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        public void DeleteUser(User user)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var userToDelete = _context.Users.FirstOrDefault(item => item.Id == user.Id);
                    _context.Users.Remove(userToDelete);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateManagerForUsers()
        {
            try
            {
                string query = "UPDATE usr SET ManagerId = usr1.Id FROM DI_M_User usr JOIN DI_M_User usr1 ON usr.Supervisor = usr1.EmployeeId";

                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Database.ExecuteSqlCommand(query);
                }
            }
            catch
            {
                throw;
            }
        }

        #region Private Methods

        private List<SqlParameter> GetUserParams()
        {
            SqlParameter sqlParam = null;
            List<SqlParameter> parameters = new List<SqlParameter>();
            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@UserId";
            sqlParam.SqlDbType = SqlDbType.Int;
            sqlParam.SourceColumn = "UserId";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@NRIC";
            sqlParam.SqlDbType = SqlDbType.NChar;
            sqlParam.Size = 9;
            sqlParam.SourceColumn = "NRIC";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@FIN";
            sqlParam.SqlDbType = SqlDbType.NChar;
            sqlParam.Size = 15;
            sqlParam.SourceColumn = "FIN";
            parameters.Add(sqlParam);

            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@PassportNo";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = 50;
            sqlParam.SourceColumn = "PassportNo";
            parameters.Add(sqlParam);

            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@UserName";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "UserName";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@Password";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "Password";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@TenantId";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = 50;
            sqlParam.SourceColumn = "TenantId";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@FirstName";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "FirstName";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@MiddleName";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "MiddleName";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@LastName";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "LastName";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@DisplayName";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "DisplayName";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@EmployeeId";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "EmployeeId";
            parameters.Add(sqlParam);

            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@FullTimeEmployee";
            sqlParam.SqlDbType = SqlDbType.Bit;
            sqlParam.SourceColumn = "FullTimeEmployee";
            parameters.Add(sqlParam);

            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@organisationId";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "organisationId";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@EmailId";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "EmailId";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@ManagerId";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "ManagerId";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@Location";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "Location";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@Address";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "Address";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@Country";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "Country";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@City";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "City";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@State";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "State";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@TelephoneNumber";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "TelephoneNumber";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@HomephoneNumber";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "HomephoneNumber";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@MobileNumber";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "MobileNumber";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@BusinessRole";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "BusinessRole";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@Company";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "Company";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@JoinDate";
            sqlParam.SqlDbType = SqlDbType.DateTime;
            sqlParam.SourceColumn = "JoinDate";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@RehireDate";
            sqlParam.SqlDbType = SqlDbType.DateTime;
            sqlParam.SourceColumn = "RehireDate";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@TerminationDate";
            sqlParam.SqlDbType = SqlDbType.DateTime;
            sqlParam.SourceColumn = "TerminationDate";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@Remarks1";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "Remarks1";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@Remarks2";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "Remarks2";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@Remarks3";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "Remarks3";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@IsActive";
            sqlParam.SqlDbType = SqlDbType.Bit;
            sqlParam.SourceColumn = "IsActive";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@DeepIdentityRole";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "DeepIdentityRole";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@EmployeeStatus";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = 6;
            sqlParam.SourceColumn = "EmployeeStatus";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@WorkAddress1";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "WorkAddress1";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@WorkAddress2";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "WorkAddress2";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@WorkAddress3";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "WorkAddress3";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@WorkAddress4";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "WorkAddress4";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@WorkPostalCode";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "WorkPostalCode";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@Country2Char";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = 2;
            sqlParam.SourceColumn = "Country2Char";
            parameters.Add(sqlParam);


            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@Building";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.Size = -1;
            sqlParam.SourceColumn = "Building";
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
        private DataTable getUserTable()
        {
            DataTable table = new DataTable();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(User));

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

            DataColumn dcol = table.Columns["UserId"];
            dcol.AutoIncrement = true;
            dcol.AutoIncrementSeed = 1;
            dcol.AutoIncrementStep = 1;



            return table;

        }

        #endregion

        #endregion

        #region ACTIVE DIRECTORY

        public IList<ADSGroupInfo> GetAllADSGroupsByUserId(int UserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSUserInfos.Include("ADSGroups")
                        .FirstOrDefault(item => item.Id == UserId).ADSGroups.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion


        #region SAP
        public IList<SAPRoleInfo> GetAllSAPRoleCodesByDIUserId(int userId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var user = _context.SAPUserinfos.FirstOrDefault(item => item.Id == userId);
                    if (user != null)
                    {
                        return user.SAPRoleInfo.ToList();
                    }
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Used For  SAP  Attestation 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        public IList<SAPRoleInfo> GetAllSAPRoleCodesByDIUserIdForAttestation(long userId)
        {
            try
            {
                IList<SAPRoleInfo> listSApRole = new List<SAPRoleInfo>();
                string query = string.Format("Select  rl.* from DI_SAP_ROLEINFO rl INNER JOIN   SAPRoleInfoSAPUserinfo userrole ON rl.Id=userrole.SAPRoleInfo_Id where userrole.SAPUserinfo_Id={0} and IsDirectlyAttached ={1} and convert(datetime, role_todate, 105) >= convert(datetime, {2}, 131)  ", userId, 1, DateTime.Now.Date.ToShortDateString());
                using (IACMDBContext _context = new IACMDBContext())
                {
                    listSApRole = _context.Database.SqlQuery<SAPRoleInfo>(query).ToList();
                }

                return listSApRole;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<SAPRoleInfo> GetAllSAPSingleRolesByDIUserId(int userId)
        {
            try
            {

                using (IACMDBContext _context = new IACMDBContext())
                {
                    var user = _context.SAPUserinfos.FirstOrDefault(item => item.Id == userId);
                    if (user != null)
                    {
                        return user.SAPRoleInfo.Where(item => item.RoleType == Constants.SINGLE).ToList();
                    }
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        #endregion

        #region LOTUSNOTES
        public IList<LotusGroupInfo> GetAllLNSGroupsByDIUserId(int DIUserId)
        {
            IList<LotusGroupInfo> list = new List<LotusGroupInfo>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    LotusUserInfo LNSUserInfo = oIACMDBContext.LotusUserInfos.FirstOrDefault(user => user.DIUserId == DIUserId);

                    if (LNSUserInfo != null)
                    {
                        list = LNSUserInfo.LotusGroupInfos;
                    }

                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Getting the Get User Groups and OU.", ex);
                throw ex;
            }

            return list;
        }

        public IList<LotusGroupInfo> GetAllLNSGroupsByUserId(int UserId)
        {
            IList<LotusGroupInfo> list = new List<LotusGroupInfo>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    LotusUserInfo LNSUserInfo = oIACMDBContext.LotusUserInfos.FirstOrDefault(user => user.Id == UserId);

                    if (LNSUserInfo != null)
                    {
                        list = LNSUserInfo.LotusGroupInfos;
                    }

                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Getting the Get User Groups and OU.", ex);
                throw ex;
            }

            return list;
        }


        public IList<SAPEPRoleInfo> GetAllSAPEPRolesByUserId(int UserId)
        {
            IList<SAPEPRoleInfo> list = new List<SAPEPRoleInfo>();

            try
            {
                //string query = "DI_spGetSAPEPRolesByUserId";
                using (IACMDBContext _context = new IACMDBContext())
                {
                    list = _context.SAPEPUserInfos.Include("SAPEPRoles")
                        .FirstOrDefault(item => item.Id == UserId).SAPEPRoles.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return list;
        }

        #endregion

        #endregion

        public IList<string> GetAllADGroupsAndOUByDIUserId(int DIUserID, int EndpointUserId, int endPointSystemId, out IList<string> usrOUList, out IList<string[]> securityGroup, out IList<string> distributionLists)
        {
            //string query = "DI_spGetADGroupsAndOUByDIUserId";
            IList<string> usrGrps = new List<string>();
            securityGroup = new List<string[]>();
            distributionLists = new List<string>();
            usrOUList = new List<string>();
            string grpStr;
            string secgrpStr;
            string distributiongrpStr;
            string grpDscStr;
            string OUStr;
            string[] OUOuter;
            string[] OUInner;

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var User = (from usr in oIACMDBContext.ADSUserInfos
                                where (usr.DIUserId == DIUserID && usr.EndPointSystemId == endPointSystemId && usr.Id == EndpointUserId)
                                select usr);
                    var list = User.FirstOrDefault().ADSGroups;

                    if (list == null || list.Count == 0)
                        return null;

                    foreach (var listIndex in list)
                    {
                        grpStr = CheckNull.AgainstString(listIndex.cn);
                        grpDscStr = CheckNull.AgainstString(listIndex.description);
                        // OUStr = CheckNull.AgainstString(listIndex.distinguishedName);

                        string grp = string.Empty;
                        if (grpDscStr != "")
                            //grp = grpStr + " - " + "(" + grpDscStr + ")";
                            grp = grpStr;
                        else
                            grp = grpStr;

                        if (!string.IsNullOrEmpty(grpStr) && !usrGrps.Contains(grpStr))
                            usrGrps.Add(grp);

                        //if (!string.IsNullOrEmpty(OUStr))
                        //{
                        //    OUOuter = OUStr.Split(',');
                        //    if (OUOuter != null && OUOuter.Length > 0)
                        //    {
                        //        foreach (string tmpValue in OUOuter)
                        //        {
                        //            OUInner = tmpValue.Split('=');
                        //            if (OUInner != null && OUInner.Length == 2)
                        //            {
                        //                if (!string.IsNullOrEmpty(OUInner[0]) && OUInner[0].ToUpper().Trim().Equals("OU") && !usrOUList.Contains(OUInner[1]))
                        //                {
                        //                    usrOUList.Add(OUInner[1]);
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        //New 

                        if (listIndex.groupTypeDesc == Constants.SECURITY_GROUP)
                        {
                            secgrpStr = CheckNull.AgainstString(listIndex.cn);
                            grpDscStr = CheckNull.AgainstString(listIndex.description);
                            string[] secgrp = new string[2];
                            if (grpDscStr != "")
                                //secgrp = secgrpStr + " - " + "(" + grpDscStr + ")";
                                secgrp[0] = secgrpStr;
                            else
                                secgrp[0] = secgrpStr;

                            secgrp[1] = grpDscStr;
                            // if (!string.IsNullOrEmpty(secgrpStr) && !securityGroup.Contains(secgrpStr))
                            if (!string.IsNullOrEmpty(secgrpStr))
                                securityGroup.Add(secgrp);
                        }
                        if (listIndex.groupTypeDesc == Constants.DISTRIBUTION_GROUP)
                        {
                            distributiongrpStr = CheckNull.AgainstString(listIndex.cn);
                            grpDscStr = CheckNull.AgainstString(listIndex.description);
                            string distgrp = string.Empty;
                            if (grpDscStr != "")
                                //distgrp = distributiongrpStr + " - " + "(" + grpDscStr + ")";
                                distgrp = distributiongrpStr;
                            else
                                distgrp = distributiongrpStr;

                            if (!string.IsNullOrEmpty(distributiongrpStr) && !distributionLists.Contains(distributiongrpStr))
                                distributionLists.Add(distgrp);
                        }


                    }
                    OUStr = CheckNull.AgainstString(User.FirstOrDefault().ou);
                    if (!string.IsNullOrEmpty(OUStr))
                    {
                        //OUOuter = OUStr.Split(',');
                        //if (OUOuter != null && OUOuter.Length > 0)
                        //{
                        //foreach (string tmpValue in OUOuter)
                        //{
                        //    OUInner = tmpValue.Split('=');
                        //    if (OUInner != null && OUInner.Length == 2)
                        //    {
                        //        if (!string.IsNullOrEmpty(OUInner[0]) && OUInner[0].ToUpper().Trim().Equals("OU") && !usrOUList.Contains(OUInner[1]))
                        //        {
                        //            usrOUList.Add(OUInner[1]);
                        //        }
                        //    }
                        //}

                        //}
                        usrOUList.Add(OUStr);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Getting the Get User Groups and OU.", ex);
                throw ex;
            }
            return usrGrps;
        }

        public void IsAdminUser(out bool isAdminGroup, out bool isIACMAdminGroup, long userID)
        {
            isAdminGroup = false;
            isIACMAdminGroup = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    User aUser = oIACMDBContext.Users.FirstOrDefault(usr => usr.Id == userID);
                    if (aUser != null)
                    {
                        foreach (Group grpItem in aUser.Groups)
                        {
                            if (grpItem.GroupName == "Administrator" || grpItem.GroupName == "IT Security")
                            {
                                isAdminGroup = true;
                                isIACMAdminGroup = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while checking is current user is in Admin Group", ex);
                throw ex;
            }

        }

        public bool IsHOSorAdminUser(int UserId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    bool IsAdminGroup = true, isIACMAdminGroup = true;
                    IsAdminUser(out IsAdminGroup, out isIACMAdminGroup, UserId);
                    bool IsHOS = oIACMDBContext.Organisations.FirstOrDefault(org => org.Head == UserId) != null;

                    return IsAdminGroup || isIACMAdminGroup || IsHOS;

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<User> GetAllUsersDataList(string _tenantId, string _OrgId)
        {
            //    string query = "DI_spGetUsersData";
            try
            {
                List<User> tempUserList = new List<User>();
                Organisation aOrganisation = new Organisation();
                long orgId = -1;
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (!string.IsNullOrEmpty(_OrgId) && _OrgId.ToLower() != "all")
                    {
                        aOrganisation = oIACMDBContext.Organisations.FirstOrDefault(org => org.OrganisationName == _OrgId);
                        if (aOrganisation != null)
                            orgId = aOrganisation.Id;
                        else
                            orgId = -1;
                    }

                    if (!string.IsNullOrEmpty(_tenantId) && orgId != -1)
                        tempUserList = oIACMDBContext.Users.Where(usr => usr.TenantId == _tenantId && usr.OrganisationId == orgId).ToList();
                    else if (string.IsNullOrEmpty(_tenantId) && orgId == -1)
                        tempUserList = oIACMDBContext.Users.ToList();
                    else if (!string.IsNullOrEmpty(_tenantId) && orgId == -1)
                        tempUserList = oIACMDBContext.Users.Where(usr => usr.TenantId == _tenantId).ToList();
                    else if (string.IsNullOrEmpty(_tenantId) && orgId != -1)
                        tempUserList = oIACMDBContext.Users.Where(usr => usr.OrganisationId == orgId).ToList();
                }
                return tempUserList;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataSet GetAllUsersData(string _tenantId, string _OrgId)
        {
            //    string query = "DI_spGetUsersData";
            try
            {
                List<User> tempUserList = new List<User>();
                Organisation aOrganisation = new Organisation();
                long orgId = -1;
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (!string.IsNullOrEmpty(_OrgId) && _OrgId.ToLower() != "all")
                    {
                        aOrganisation = oIACMDBContext.Organisations.FirstOrDefault(org => org.OrganisationName == _OrgId);
                        if (aOrganisation != null)
                            orgId = aOrganisation.Id;
                        else
                            orgId = -1;
                    }

                    if (!string.IsNullOrEmpty(_tenantId) && orgId != -1)
                        tempUserList = oIACMDBContext.Users.Where(usr => usr.TenantId == _tenantId && usr.OrganisationId == orgId).ToList();
                    else if (string.IsNullOrEmpty(_tenantId) && orgId == -1)
                        tempUserList = oIACMDBContext.Users.ToList();
                    else if (!string.IsNullOrEmpty(_tenantId) && orgId == -1)
                        tempUserList = oIACMDBContext.Users.Where(usr => usr.TenantId == _tenantId).ToList();
                    else if (string.IsNullOrEmpty(_tenantId) && orgId != -1)
                        tempUserList = oIACMDBContext.Users.Where(usr => usr.OrganisationId == orgId).ToList();
                }
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                if (tempUserList.Count() > 0)
                {
                    ds.Tables.Add(dt);
                    dt.Columns.Add("UserId", typeof(int));
                    dt.Columns.Add("UserName", typeof(string));
                    dt.Columns.Add("OrganisationId", typeof(int));
                    dt.Columns.Add("TenantId", typeof(string));
                    dt.Columns.Add("DisplayName", typeof(string));

                    var resultList = tempUserList.Select(usr => new { UserId = usr.Id, usr.UserName, usr.ManagerId, usr.OrganisationId, usr.TenantId, usr.DisplayName });

                    foreach (var item in resultList)
                    {
                        DataRow dr = dt.NewRow();
                        dr["UserId"] = item.UserId;
                        dr["UserName"] = item.UserName;
                        dr["OrganisationId"] = (item.OrganisationId == null) ? -1 : item.OrganisationId;
                        dr["TenantId"] = item.TenantId;
                        dr["DisplayName"] = item.DisplayName;
                        dt.Rows.Add(dr);
                    }
                }

                if (null == ds)
                    return null;

                if (0 == ds.Tables.Count)
                    return null;

                return ds;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataSet GetAllUsersData(string _tenantId)
        {

            try
            {
                if (!string.IsNullOrEmpty(_tenantId))
                {
                    return GetAllUsersData(_tenantId, null);

                }
                return GetAllUsersData(null, null);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataSet GetAllUsersData()
        {
            try
            {
                return GetAllUsersData(null);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<User> GetAllUsersIUFL(UserType usrType)
        {
            //string query = "DI_spGetAllUsersIUFL";
            IList<User> userList = new List<User>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (usrType == UserType.Manager)
                    {
                        IList<User> managerList = new List<User>();

                        var mList = (from usr in oIACMDBContext.Users
                                     //  join mgr in oIACMDBContext.Users on
                                     // usr.ManagerId.Value equals mgr.UserId
                                     where usr.Id == usr.ManagerId && usr.ManagerId != null
                                     orderby usr.UserName
                                     select new
                                     {
                                         ManagerUId = usr.Id,
                                         usr.UserName,
                                         usr.FirstName,
                                         usr.LastName,
                                         usr.EmailId,
                                         usr.ManagerId,
                                         usr.TenantId,
                                         usr.OrganisationId,
                                         usr.DisplayName,
                                         usr.EmployeeId
                                     }).Distinct().ToList();

                        User tmpusr;
                        for (int i = 0; i < mList.Count(); i++)
                        {
                            tmpusr = new User();
                            tmpusr.Id = mList[i].ManagerUId; // userIDClmnName; //.UserId;
                            tmpusr.UserName = mList[i].UserName;
                            tmpusr.FirstName = mList[i].FirstName;
                            tmpusr.LastName = mList[i].LastName;
                            tmpusr.EmailId = mList[i].EmailId;
                            tmpusr.ManagerId = mList[i].ManagerId;
                            tmpusr.TenantId = mList[i].TenantId;
                            tmpusr.OrganisationId = mList[i].OrganisationId;
                            tmpusr.EmployeeId = mList[i].EmployeeId;
                            managerList.Add(tmpusr);
                            userList.Add(tmpusr);
                        }
                    }
                    else if (usrType == UserType.User)
                    {
                        var uList = (from usr in oIACMDBContext.Users
                                     orderby usr.UserName
                                     select new
                                     {
                                         UserId = usr.Id,
                                         usr.UserName,
                                         usr.FirstName,
                                         usr.LastName,
                                         usr.EmailId,
                                         usr.ManagerId,
                                         usr.TenantId,
                                         usr.OrganisationId,
                                         usr.DisplayName,
                                         usr.EmployeeId
                                     }).ToList();

                        userList = new List<User>();
                        User tmpusr;
                        for (int i = 0; i < uList.Count(); i++)
                        {
                            tmpusr = new User();
                            tmpusr.Id = uList[i].UserId;
                            tmpusr.UserName = uList[i].UserName;
                            tmpusr.FirstName = uList[i].FirstName;
                            tmpusr.LastName = uList[i].LastName;
                            tmpusr.EmailId = uList[i].EmailId;
                            tmpusr.ManagerId = uList[i].ManagerId;
                            tmpusr.TenantId = uList[i].TenantId;
                            tmpusr.OrganisationId = uList[i].OrganisationId;
                            tmpusr.DisplayName = uList[i].DisplayName;
                            tmpusr.EmployeeId = uList[i].EmployeeId;
                            userList.Add(tmpusr);
                        }
                    }
                }
                return userList;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All Users.", ex);
                throw ex;
            }
        }

        public IList<TabMenu> GetMenusByUserRole(int RoleId)
        {
            //IList<TabMenu> menuList = new List<TabMenu>();
            //try
            //{
            //    if (RoleId != -1)
            //    {
            //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
            //        {
            //            var query = from AR in oIACMDBContext.AccessRights
            //                        join r in oIACMDBContext.Roles
            //                    on AR.RoleId equals r.RoleId
            //                        join TM in oIACMDBContext.TabMenu.Include("Page").Include("PageUrl")
            //                    on AR.PagePermissionId equals TM.per
            //                        where r.RoleId == RoleId && TM.TargetUrl != null
            //                        select TM;

            //            menuList = query.ToList();
            //        }
            //    }
            //    return menuList;
            //}
            //catch (Exception ex)
            //{
            //    DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while Geting GetMenusByUserRole");
            //    throw;
            //}
            return null;
        }



        public IList<User> GetMangersByUserId(int UserId)
        {
            //string query = "DI_spGetMangersByUserId";
            IList<User> managerList = new List<User>();
            User aUser = new User();
            try
            {
                if (UserId != -1)
                {
                    using (IACMDBContext oIACMDBContext = new IACMDBContext())
                    {
                        aUser = oIACMDBContext.Users.FirstOrDefault(usr => usr.Id == UserId);
                        managerList = oIACMDBContext.Users.Where(mgr => mgr.ManagerId == aUser.ManagerId).ToList();
                    }
                }
                return managerList;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Geting GetMangersByUserId");
                throw;
            }
        }

        public IList<User> GetAllImplementers()
        {
            IList<User> UserList = new List<User>();
            try
            {
                //string query = "DI_spGetAllImplementers";
                using (IACMDBContext _context = new IACMDBContext())
                {
                    IList<Group> groupList = new List<Group>();
                    IList<User> tmpUserList = new List<User>();
                    groupList = _context.Groups.Where(grp => grp.GroupName == "AdminGroup" || grp.GroupName == "ImplementerGroup").Distinct().ToList();

                    foreach (Group grpListItem in groupList)
                    {
                        tmpUserList = grpListItem.Users.Where(usr => usr.EmailId != null && usr.EmailId != "").Distinct().ToList();

                        foreach (User tmpUsrListItem in tmpUserList)
                        {
                            tmpUsrListItem.EmailId = !string.IsNullOrEmpty(tmpUsrListItem.EmailId) ? tmpUsrListItem.EmailId.Trim().ToLower() : tmpUsrListItem.EmailId;
                            UserList.Add(tmpUsrListItem);
                        }
                    }
                }
                return UserList;
            }
            catch (Exception)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Geting all Implementers");
                throw;
            }
        }

        #region UserListing Methods

        //public Dictionary<string, string> GetAllTenantList()
        //{
        //    Dictionary<string, string> tenantList = new Dictionary<string, string>();
        //    //string query = "DI_spGetAllTenantList";
        //    string tenantId;
        //    string tenantName;

        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            var tenantListNew = (from tnt in oIACMDBContext.Tenants
        //                                 orderby tnt.TenantName
        //                                 select new
        //                                 {
        //                                     tnt.TenantId,
        //                                     tnt.TenantName
        //                                 }).Distinct();

        //            foreach (var tmpobj in tenantListNew)
        //            {
        //                tenantId = CheckNull.AgainstString(tmpobj.TenantId);
        //                tenantName = CheckNull.AgainstString(tmpobj.TenantName);

        //                if (!string.IsNullOrEmpty(tenantId) && !tenantList.ContainsKey(tenantId))
        //                {
        //                    tenantList.Add(tenantId, tenantName);
        //                }
        //            }

        //            return tenantList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Error, "Error occur while getting all Tenant List.", ex);
        //        throw;
        //    }
        //}


        public IList<UserListGridData> GetUsersByTenantDeptAndUserName(int? organisationId, int? managerId, IDictionary<string, List<string>> Constraints = null)
        {
            IList<UserListGridData> userList = new List<UserListGridData>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (managerId.HasValue)
                    {
                        userList = (from users in _context.Users
                                    join org in _context.Organisations on users.OrganisationId equals org.Id
                                    where users.ManagerId == managerId && users.FullTimeEmployee == true && users.IsActive.HasValue && users.IsActive.Value
                                    select new UserListGridData
                                    {
                                        UserId = users.Id,
                                        TenantId = users.TenantId,
                                        UserName = users.UserName,
                                        DisplayName = users.DisplayName,
                                        EmployeeId = users.EmployeeId,
                                        OrganisationName = org.OrganisationName,
                                        OrganisationId = org.Id,
                                        FirstName = users.FirstName,
                                        EmployeeType = users.EmployeeType,
                                        CompanyId = users.UserExtensionObj.CompanyId,
                                        DivisionId = users.UserExtensionObj.DivisionId,
                                        DepartmentId = users.UserExtensionObj.DepartmentId,
                                        CostCenter = users.UserExtensionObj.CostCenterText,
                                        //DepartmentName = users.Department.DepartmentText,
                                        //DivisionName = users.Division.DivisionText,
                                        //CompanyName = users.Company.CompanyName,
                                        BusinessRole = users.BusinessRole.DesignationDesc,
                                        IsActive = users.IsActive.HasValue ? users.IsActive.Value : false,
                                        FullTimeEmployee = users.FullTimeEmployee.HasValue ? users.FullTimeEmployee.Value : false,
                                        SectionCode = users.UserExtensionObj.SectionCode,
                                        SectionName = users.UserExtensionObj.SectionName
                                    }).ToList();
                    }
                    else
                    {
                        userList = (from users in _context.Users
                                    where users.FullTimeEmployee == true && users.IsActive.HasValue && users.IsActive.Value

                                    select new UserListGridData
                                    {
                                        UserId = users.Id,
                                        TenantId = users.TenantId,
                                        UserName = users.UserName,
                                        DisplayName = users.DisplayName,
                                        EmployeeId = users.EmployeeId,
                                        OrganisationName = users.UserExtensionObj.SectionName != null ? users.UserExtensionObj.SectionName : string.Empty,
                                        OrganisationId = users.OrganisationId.HasValue ? users.OrganisationId.Value : 0,
                                        FirstName = users.FirstName,
                                        EmployeeType = users.EmployeeType,
                                        CompanyId = users.UserExtensionObj.CompanyId,
                                        DivisionId = users.UserExtensionObj.DivisionId,
                                        DepartmentId = users.UserExtensionObj.DepartmentId,
                                        CostCenter = users.UserExtensionObj.CostCenterText,
                                        //DepartmentName = users.Department.DepartmentText,
                                        //DivisionName = users.Division.DivisionText,
                                        //CompanyName = users.Company.CompanyName,
                                        BusinessRole = users.BusinessRole.DesignationDesc,
                                        IsActive = users.IsActive.HasValue ? users.IsActive.Value : false,
                                        FullTimeEmployee = users.FullTimeEmployee.HasValue ? users.FullTimeEmployee.Value : false,
                                        SectionCode = users.UserExtensionObj.SectionCode,
                                        SectionName = users.UserExtensionObj.SectionName
                                    }).ToList();

                        //if (tenantId != string.Empty)
                        //{
                        //    userList = userList.Where(usr => usr.TenantId == tenantId).OrderBy(us => us.UserName).ToList();
                        //}
                        if (organisationId.HasValue)
                        {
                            userList = userList.Where(usr => usr.OrganisationId == organisationId).OrderBy(us => us.UserName).ToList();
                        }



                        if (Constraints != null && Constraints.Count > 0)
                        {
                            if (Constraints.ContainsKey("Companies"))
                            {
                                List<string> Company = Constraints["Companies"];

                                userList = userList.Where(c => Company.Contains(c.CompanyId.ToString())).Select(c => c).ToList();

                            }
                            if (Constraints.ContainsKey("CostCenter"))
                            {
                                List<string> CostCenter = Constraints["CostCenter"];
                                userList = userList.Where(c => CostCenter.Contains(c.CostCenter)).Select(c => c).ToList();
                            }
                            if (Constraints.ContainsKey("Department"))
                            {
                                List<string> Department = Constraints["Department"];
                                userList = userList.Where(c => Department.Contains(c.DepartmentId.ToString())).Select(c => c).ToList();
                            }
                            if (Constraints.ContainsKey("Division"))
                            {
                                List<string> Division = Constraints["Division"];
                                userList = userList.Where(c => Division.Contains(c.DivisionId.ToString())).Select(c => c).ToList();
                            }

                            if (Constraints.ContainsKey("Section"))
                            {
                                List<string> Section = Constraints["Section"];
                                userList = userList.Where(c => Section.Contains(c.SectionName.ToString())).Select(c => c).ToList();
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All Users.", ex);
                throw ex;
            }
            return userList;
        }

        #region DBTC UserListing method
        private string RemoveSpecialChar(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return inputString;

            StringBuilder sb = new StringBuilder(inputString.Length);
            foreach (char inptStrChar in inputString)
            {
                if (inptStrChar != ' '
                    && inptStrChar != '\''
                    && inptStrChar != '.'
                    && inptStrChar != '\"'
                    && inptStrChar != ':'
                    && inptStrChar != ';'
                    && inptStrChar != '~'
                    && inptStrChar != '`'
                    && inptStrChar != '!'
                    && inptStrChar != '@'
                    && inptStrChar != '#'
                    && inptStrChar != '$'
                    && inptStrChar != '%'
                    && inptStrChar != '^'
                    && inptStrChar != '&'
                    && inptStrChar != '*'
                    && inptStrChar != '('
                    && inptStrChar != ')'
                    && inptStrChar != '|'
                    && inptStrChar != '\\'
                    && inptStrChar != ','
                    && inptStrChar != '-')
                {
                    sb.Append(inptStrChar);
                }
            }
            return sb.ToString();
        }

        public DataTable GetDBTCUsersByDIUserID(string hostName, int DIUserId)
        {
            DataTable dt = null;
            IACMDbOperator m_dbOperator = null;
            QueryExecutor executor;
            DataSet ds;

            if (string.IsNullOrEmpty(hostName))
                return dt;

            string tableName = "DI_" + RemoveSpecialChar(hostName) + "_UserInfo";
            string queryFrmt = @"IF EXISTS (SELECT OBJECT_ID FROM sys.tables WHERE name = '{0}') 
                                BEGIN 
                                IF EXISTS  (SELECt clmn.OBJECT_ID FROM sys.columns clmn JOIN sys.tables tbls ON clmn.OBJECT_ID = tbls.OBJECT_ID WHERE clmn.name = 'DIUserId' AND tbls.name = '{0}')
                                BEGIN
                                    SELECT * FROM {0} WHERE DIUserId = {1}
                                END
                                END";

            string query = string.Format(queryFrmt, tableName, DIUserId);
            try
            {
                m_dbOperator = new IACMDbOperator(query, CommandType.Text, null);
                executor = new QueryExecutor(m_dbOperator);
                ds = executor.ExecuteQueryDS();

                if (ds != null && ds.Tables != null && ds.Tables.Count == 1)
                    dt = ds.Tables[0];

                return dt;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Error, "Error occur while getting DBTC Users by DIUserID.", ex);
                throw;
            }
            finally
            {
                if (m_dbOperator != null)
                    m_dbOperator.Close();
            }
        }
        #endregion

        #endregion

        public void UpdateLastLogonInformation(User User)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    User objUser = (from usr in oIACMDBContext.Users.Where(usr => usr.Id == User.Id) select usr).FirstOrDefault();
                    if (objUser != null)
                    {
                        objUser.LastLogonDate = DateTime.Now;
                        oIACMDBContext.Entry(objUser);
                        oIACMDBContext.Entry(objUser).State = EntityState.Modified;

                    }

                    oIACMDBContext.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Updating User Details", ex);
                throw ex;
            }
        }


        public Organisation GetOrganisationByUserID(int userID) // TRSOrganisation 
        {
            //string query = "DI_spGetOrganisationByUserID";
            Organisation organisation = null; // TRSOrganisation 
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    organisation = (from usr in oIACMDBContext.Users
                                    join org in oIACMDBContext.Organisations
                                    on SqlFunctions.StringConvert((double)usr.OrganisationId).Trim() equals org.OrgUnitPrefix
                                    where usr.Id == userID
                                    select org).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting Organisation By UserId ", ex);
                throw ex;
            }
            return organisation;
        }

        public IList<User> GetAllUsersByEndPointName(EndpointType endPtName, int endPtId)
        {
            IList<User> UserList = new List<User>();

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    User UserDetails = null;
                    //string query = "DI_spGetAllUsersByEndPointName";

                    switch (endPtName)
                    {
                        case EndpointType.ActiveDirectory:
                            var ADSUserListQuery = (from usr in oIACMDBContext.Users
                                                    join endUser in oIACMDBContext.ADSUserInfos on usr.Id equals endUser.DIUserId
                                                    where endUser.EndPointSystemId == endPtId && endUser.DIUserId != null && endUser.DIUserId != -1
                                                    select new
                                                    {
                                                        UserId = usr.Id,
                                                        usr.FirstName,
                                                        usr.LastName,
                                                        UserLogOnName = endUser.sAMAccountName,
                                                        usr.EmailId,
                                                        usr.DisplayName
                                                    }).ToList();

                            foreach (var usr1 in ADSUserListQuery)
                            {
                                UserDetails = new User();

                                UserDetails.Id = CheckNull.AgainstInt(usr1.UserId);
                                UserDetails.UserName = CheckNull.AgainstString(usr1.UserLogOnName);
                                UserDetails.FirstName = CheckNull.AgainstString(usr1.FirstName);
                                UserDetails.LastName = CheckNull.AgainstString(usr1.LastName);
                                UserDetails.EmailId = CheckNull.AgainstString(usr1.EmailId);
                                UserDetails.DisplayName = CheckNull.AgainstString(usr1.DisplayName);

                                UserList.Add(UserDetails);
                            }

                            break;
                        case EndpointType.SAP:
                            var SAPUserListQuery = (from usr in oIACMDBContext.Users
                                                    join endUser in oIACMDBContext.SAPUserinfos on usr.Id equals endUser.DIUserId
                                                    where endUser.EndPointSystemId == endPtId && endUser.DIUserId != null && endUser.DIUserId != -1
                                                    select new
                                                    {
                                                        UserId = usr.Id,
                                                        usr.FirstName,
                                                        usr.LastName,
                                                        UserLogOnName = endUser.UserLogonName,
                                                        usr.EmailId,
                                                        usr.DisplayName
                                                    }).ToList();

                            foreach (var usr1 in SAPUserListQuery)
                            {
                                UserDetails = new User();

                                UserDetails.Id = CheckNull.AgainstInt(usr1.UserId);
                                UserDetails.UserName = CheckNull.AgainstString(usr1.UserLogOnName);
                                UserDetails.FirstName = CheckNull.AgainstString(usr1.FirstName);
                                UserDetails.LastName = CheckNull.AgainstString(usr1.LastName);
                                UserDetails.EmailId = CheckNull.AgainstString(usr1.EmailId);
                                UserDetails.DisplayName = CheckNull.AgainstString(usr1.DisplayName);

                                UserList.Add(UserDetails);
                            }
                            break;
                        case EndpointType.LotusNotes:
                            var LotusUserListQuery = (from usr in oIACMDBContext.Users
                                                      join endUser in oIACMDBContext.LotusUserInfos on usr.Id equals endUser.DIUserId
                                                      where endUser.EndPointSystemId == endPtId && endUser.DIUserId != null && endUser.DIUserId != -1
                                                      select new
                                                      {
                                                          UserId = usr.Id,
                                                          usr.FirstName,
                                                          usr.LastName,
                                                          UserLogOnName = endUser.InternetAddress,
                                                          usr.EmailId,
                                                          usr.DisplayName
                                                      }).ToList();

                            foreach (var usr1 in LotusUserListQuery)
                            {
                                UserDetails = new User();

                                UserDetails.Id = CheckNull.AgainstInt(usr1.UserId);
                                UserDetails.UserName = CheckNull.AgainstString(usr1.UserLogOnName);
                                UserDetails.FirstName = CheckNull.AgainstString(usr1.FirstName);
                                UserDetails.LastName = CheckNull.AgainstString(usr1.LastName);
                                UserDetails.EmailId = CheckNull.AgainstString(usr1.EmailId);
                                UserDetails.DisplayName = CheckNull.AgainstString(usr1.DisplayName);

                                UserList.Add(UserDetails);
                            }
                            break;
                        case EndpointType.SAPEnterprisePortal:
                            var SAPEPUserListQuery = (from usr in oIACMDBContext.Users
                                                      join endUser in oIACMDBContext.SAPEPUserInfos on usr.Id equals endUser.DIUserId
                                                      where endUser.EndPointSystemId == endPtId && endUser.DIUserId != null && endUser.DIUserId != -1
                                                      select new
                                                      {
                                                          UserId = usr.Id,
                                                          usr.FirstName,
                                                          usr.LastName,
                                                          UserLogOnName = endUser.UserName,
                                                          usr.EmailId,
                                                          usr.DisplayName
                                                      }).ToList();

                            foreach (var usr1 in SAPEPUserListQuery)
                            {
                                UserDetails = new User();

                                UserDetails.Id = CheckNull.AgainstInt(usr1.UserId);
                                UserDetails.UserName = CheckNull.AgainstString(usr1.UserLogOnName);
                                UserDetails.FirstName = CheckNull.AgainstString(usr1.FirstName);
                                UserDetails.LastName = CheckNull.AgainstString(usr1.LastName);
                                UserDetails.EmailId = CheckNull.AgainstString(usr1.EmailId);
                                UserDetails.DisplayName = CheckNull.AgainstString(usr1.DisplayName);

                                UserList.Add(UserDetails);
                            }
                            break;
                    }
                }
                return UserList;
            }
            catch (Exception)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Geting All Users By Endpoint Name.");
                throw;
            }
        }


        #region Provisioning Methods

        public List<UsersEndPointSystemDetailsGridData> GetUsersOfEndpointsByDIUserId(int UserId)
        {
            DataTable dt = new DataTable();
            try
            {
                //string query = "DI_spGetUsersOfEndpointsBy_DIUserId";

                List<UsersEndPointSystemDetailsGridData> endPointSystemList = new List<UsersEndPointSystemDetailsGridData>();
                UsersEndPointSystemDetailsGridData endPointSystemListItem;

                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var ADSquery = (from usr in oIACMDBContext.Users
                                    join endUser in oIACMDBContext.ADSUserInfos on usr.Id equals endUser.DIUserId
                                    join endPoin in oIACMDBContext.EndpointSystems on endUser.EndPointSystemId equals endPoin.Id
                                    join endpointType in oIACMDBContext.EndpointSystemTypes on endPoin.EndpointSystemTypeId equals endpointType.Id
                                    where usr.Id == UserId
                                    select new
                                    {
                                        endpointType.EndpointSystemTypeName,
                                        HostName = endPoin.DisplayName,
                                        endUser.EndPointSystemId, // DI_END_UserInfoEndPointSystemId =
                                        UserName = endUser.sAMAccountName,
                                        endUser.TenantId,
                                        UserId = endUser.Id, // DI_M_EndpointUserId = 
                                        usr.DisplayName
                                    }).Distinct();

                    var Lotusquery = (from usr in oIACMDBContext.Users
                                      join endUser in oIACMDBContext.LotusUserInfos on usr.Id equals endUser.DIUserId
                                      join endPoin in oIACMDBContext.EndpointSystems on endUser.EndPointSystemId equals endPoin.Id
                                      join endpointType in oIACMDBContext.EndpointSystemTypes on endPoin.EndpointSystemTypeId equals endpointType.Id
                                      where usr.Id == UserId
                                      select new
                                      {
                                          endpointType.EndpointSystemTypeName,
                                          HostName = endPoin.DisplayName,
                                          endUser.EndPointSystemId, // DI_END_UserInfoEndPointSystemId =
                                          UserName = endUser.Name,
                                          endUser.TenantId,
                                          UserId = endUser.Id, // DI_M_EndpointUserId = 
                                          usr.DisplayName
                                      }).Distinct();


                    var SAPquery = (from usr in oIACMDBContext.Users
                                    join endUser in oIACMDBContext.SAPUserinfos on usr.Id equals endUser.DIUserId
                                    join endPoin in oIACMDBContext.EndpointSystems on endUser.EndPointSystemId equals endPoin.Id
                                    join endpointType in oIACMDBContext.EndpointSystemTypes on endPoin.EndpointSystemTypeId equals endpointType.Id
                                    where usr.Id == UserId
                                    select new
                                    {
                                        endpointType.EndpointSystemTypeName,
                                        HostName = endPoin.DisplayName,
                                        endUser.EndPointSystemId, // DI_END_UserInfoEndPointSystemId =
                                        UserName = endUser.UserLogonName,
                                        endUser.TenantId,
                                        UserId = endUser.Id, // DI_M_EndpointUserId = 
                                        usr.DisplayName
                                    }).Distinct();

                    var SAPEPquery = (from usr in oIACMDBContext.Users
                                      join endUser in oIACMDBContext.SAPEPUserInfos on usr.Id equals endUser.DIUserId
                                      join endPoin in oIACMDBContext.EndpointSystems on endUser.EndPointSystemId equals endPoin.Id
                                      join endpointType in oIACMDBContext.EndpointSystemTypes on endPoin.EndpointSystemTypeId equals endpointType.Id
                                      where usr.Id == UserId
                                      select new
                                      {
                                          endpointType.EndpointSystemTypeName,
                                          HostName = endPoin.DisplayName,
                                          endUser.EndPointSystemId, // DI_END_UserInfoEndPointSystemId =
                                          UserName = endUser.DisplayName,
                                          endUser.TenantId,
                                          UserId = endUser.Id, // DI_M_EndpointUserId = 
                                          usr.DisplayName
                                      }).Distinct();

                    //var listTotal = ADSquery.Union(Lotusquery).Union(winquery).Union(SAPquery).Union(SAPEPquery);
                    var listTotal1 = ADSquery.Union(Lotusquery);
                    var listTotal3 = listTotal1.Union(SAPquery);
                    var listTotal = listTotal3.Union(SAPEPquery);

                    if (listTotal.Count() > 0)
                    {
                        foreach (var tmpList in listTotal)
                        {
                            endPointSystemListItem = new UsersEndPointSystemDetailsGridData();
                            endPointSystemListItem.EndpointSystemTypeName = CheckNull.AgainstString(tmpList.EndpointSystemTypeName);
                            endPointSystemListItem.HostName = CheckNull.AgainstString(tmpList.HostName);
                            endPointSystemListItem.EndPointSystemId = CheckNull.AgainstInt(tmpList.EndPointSystemId); // DI_END_UserInfoEndPointSystemId
                            endPointSystemListItem.UserName = CheckNull.AgainstString(tmpList.UserName);
                            endPointSystemListItem.TenantId = CheckNull.AgainstString(tmpList.TenantId);
                            endPointSystemListItem.DisplayName = CheckNull.AgainstString(tmpList.DisplayName);
                            endPointSystemListItem.UserId = CheckNull.AgainstInt(tmpList.UserId); // DI_M_EndpointUserId 
                            endPointSystemList.Add(endPointSystemListItem);
                        }
                    }
                }

                //if (winquery != null)
                //{
                //    if (query.Count() > 0) winUserList = query.ToList();
                //}
                //return list;
                //

                return endPointSystemList; // dt;
            }
            catch (Exception)
            {

                throw;
            }
        }








        #endregion

        public long AddUserManually(User user)
        {
            long returnValue = -1;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    oIACMDBContext.Users.Add(user);
                    oIACMDBContext.SaveChanges();
                    returnValue = user.Id;
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Adding  the User", ex);
                throw ex;
            }
        }

        public bool UpdateUserName(User usr)
        {
            if (!(usr != null && !string.IsNullOrEmpty(usr.UserName)))
                return false;

            try
            {
                // string query = "DI_spUpdateUserNameByUserId";
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    User editUser = oIACMDBContext.Users.Where(u => u.Id == usr.Id).FirstOrDefault();
                    if (editUser != null)
                    {
                        editUser.UserName = usr.UserName;
                        editUser.LastModifiedBy = usr.LastModifiedBy;
                        editUser.LastModifiedDate = usr.LastModifiedDate;

                        oIACMDBContext.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Updating UserName in UpdateUserName method.");
                throw;
            }
        }


        # region FSP



        #endregion
        public IList<UserToEndPointsReportData> getUserToEndPointsDetails(List<int> UserID, IDictionary<string, List<string>> Constraints)
        {
            IList<UserToEndPointsReportData> retList = new List<UserToEndPointsReportData>();
            IList<User> userList = new List<User>();
            UserToEndPointsReportData aEPDetails = new UserToEndPointsReportData();
            Organisation orgDetail = new Organisation();

            IList<ADSUserInfo> listADSUserInfo = new List<ADSUserInfo>();
            IList<SAPUserinfo> listSAPUserInfo = new List<SAPUserinfo>();
            IList<LotusUserInfo> listLNSUserInfo = new List<LotusUserInfo>();
            IList<SAPEPUserInfo> listSAPEPUserInfo = new List<SAPEPUserInfo>();
            IList<InhouseUserInfo> listINHUserInfo = new List<InhouseUserInfo>();
            User DIUserInfo = new User();



            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {

                    if (Constraints != null && Constraints.Count > 0)
                    {
                        userList = oIACMDBContext.Users.ToList();

                        if (Constraints.ContainsKey("Companies"))
                        {
                            List<string> Company = Constraints["Companies"];

                            userList = userList.Where(c => Company.Contains(c.UserExtensionObj.CompanyId.ToString())).Select(c => c).ToList();

                        }
                        if (Constraints.ContainsKey("CostCenter"))
                        {
                            List<string> CostCenter = Constraints["CostCenter"];
                            userList = userList.Where(c => CostCenter.Contains(c.UserExtensionObj.CostCenter)).Select(c => c).ToList();
                        }
                        if (Constraints.ContainsKey("Department"))
                        {
                            List<string> Department = Constraints["Department"];
                            userList = userList.Where(c => Department.Contains(c.UserExtensionObj.DepartmentId.ToString())).Select(c => c).ToList();
                        }
                        if (Constraints.ContainsKey("Division"))
                        {
                            List<string> Division = Constraints["Division"];
                            userList = userList.Where(c => Division.Contains(c.UserExtensionObj.DivisionId.ToString())).Select(c => c).ToList();
                        }

                        if (Constraints.ContainsKey("Section"))
                        {
                            List<string> Section = Constraints["Section"];
                            userList = userList.Where(c => Section.Contains(c.UserExtensionObj.SectionName.ToString())).Select(c => c).ToList();
                        }
                        if (Constraints.ContainsKey("User"))
                        {
                            List<string> user = Constraints["User"];
                            userList = userList.Where(c => user.Contains(c.Id.ToString())).Select(c => c).ToList();
                        }
                    }

                    foreach (User objUser in userList)
                    {
                        if (objUser.Id != 1)
                        {
                            DIUserInfo = oIACMDBContext.Users.FirstOrDefault(diusr => diusr.Id == objUser.Id);
                            listADSUserInfo = oIACMDBContext.ADSUserInfos.Where(adsusr => adsusr.DIUserId == objUser.Id && adsusr.IsLocked == false).ToList();
                            listSAPUserInfo = oIACMDBContext.SAPUserinfos.Where(sapusr => sapusr.DIUserId == objUser.Id && sapusr.IsLocked == false).ToList();
                            listLNSUserInfo = oIACMDBContext.LotusUserInfos.Where(lnsusr => lnsusr.DIUserId == objUser.Id && lnsusr.IsLocked == false).ToList();
                            listSAPEPUserInfo = oIACMDBContext.SAPEPUserInfos.Where(sapepusr => sapepusr.DIUserId == objUser.Id && sapepusr.IsLocked == false).ToList();
                            listINHUserInfo = oIACMDBContext.InhouseUserInfos.Where(INHusr => INHusr.DIUserId == objUser.Id && INHusr.IsLocked == false && INHusr.IsDeleted == false).ToList();


                            if (DIUserInfo.OrganisationId != null)
                            {
                                orgDetail = oIACMDBContext.Organisations.FirstOrDefault(user => user.Id == DIUserInfo.OrganisationId);
                            }

                            foreach (ADSUserInfo adusr in listADSUserInfo)
                            {
                                if (adusr != null)
                                {
                                    aEPDetails = new UserToEndPointsReportData();
                                    aEPDetails.userName = DIUserInfo.DisplayName;
                                    aEPDetails.EndpointHostName = oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.Id == adusr.EndPointSystemId).DisplayName;
                                    aEPDetails.EndpointSystemName = oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.Id == adusr.EndPointSystemId).EndPointSystemType.EndpointSystemTypeName.ToString();
                                    if (!string.IsNullOrEmpty(adusr.sAMAccountName))
                                    {
                                        aEPDetails.EndpointUserName = adusr.sAMAccountName;
                                    }
                                    if (orgDetail != null)
                                    {
                                        aEPDetails.OrganisationName = orgDetail.OrganisationName;
                                    }
                                    retList.Add(aEPDetails);
                                }
                            }

                            foreach (SAPUserinfo sapusr in listSAPUserInfo)
                            {
                                if (sapusr != null)
                                {
                                    aEPDetails = new UserToEndPointsReportData();
                                    aEPDetails.userName = DIUserInfo.DisplayName;
                                    aEPDetails.EndpointHostName = oIACMDBContext.EndpointSystems.FirstOrDefault(eps => eps.Id == sapusr.EndPointSystemId).DisplayName;
                                    aEPDetails.EndpointSystemName = oIACMDBContext.EndpointSystems.FirstOrDefault(eps => eps.Id == sapusr.EndPointSystemId).EndPointSystemType.EndpointSystemTypeName.ToString();
                                    if (!string.IsNullOrEmpty(sapusr.UserLogonName))
                                    {
                                        aEPDetails.EndpointUserName = sapusr.UserLogonName;
                                    }
                                    if (orgDetail != null)
                                    {
                                        aEPDetails.OrganisationName = orgDetail.OrganisationName;
                                    }
                                    retList.Add(aEPDetails);
                                }
                            }

                            foreach (LotusUserInfo lnsusr in listLNSUserInfo)
                            {
                                if (lnsusr != null)
                                {
                                    aEPDetails = new UserToEndPointsReportData();
                                    aEPDetails.userName = DIUserInfo.DisplayName;
                                    aEPDetails.EndpointHostName = oIACMDBContext.EndpointSystems.FirstOrDefault(eps => eps.Id == lnsusr.EndPointSystemId).DisplayName;
                                    aEPDetails.EndpointSystemName = oIACMDBContext.EndpointSystems.FirstOrDefault(eps => eps.Id == lnsusr.EndPointSystemId).EndPointSystemType.EndpointSystemTypeName.ToString();
                                    if (!string.IsNullOrEmpty(lnsusr.ShortName))
                                    {
                                        aEPDetails.EndpointUserName = lnsusr.ShortName; // InternetAddress;
                                    }
                                    if (orgDetail != null)
                                    {
                                        aEPDetails.OrganisationName = orgDetail.OrganisationName;
                                    }
                                    retList.Add(aEPDetails);
                                }
                            }

                            foreach (SAPEPUserInfo sapepusr in listSAPEPUserInfo)
                            {
                                if (sapepusr != null)
                                {
                                    aEPDetails = new UserToEndPointsReportData();
                                    aEPDetails.userName = DIUserInfo.DisplayName;
                                    aEPDetails.EndpointHostName = oIACMDBContext.EndpointSystems.FirstOrDefault(eps => eps.Id == sapepusr.EndPointSystemId).DisplayName;
                                    aEPDetails.EndpointSystemName = oIACMDBContext.EndpointSystems.FirstOrDefault(eps => eps.Id == sapepusr.EndPointSystemId).EndPointSystemType.EndpointSystemTypeName.ToString();
                                    if (!string.IsNullOrEmpty(sapepusr.UserName))
                                    {
                                        aEPDetails.EndpointUserName = sapepusr.UserName;
                                    }
                                    if (orgDetail != null)
                                    {
                                        aEPDetails.OrganisationName = orgDetail.OrganisationName;
                                    }
                                    retList.Add(aEPDetails);
                                }
                            }


                            foreach (InhouseUserInfo inhusr in listINHUserInfo)
                            {
                                if (inhusr != null)
                                {
                                    aEPDetails = new UserToEndPointsReportData();
                                    aEPDetails.userName = DIUserInfo.DisplayName;
                                    aEPDetails.EndpointHostName = oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.Id == inhusr.EndPointSystemId).DisplayName;
                                    aEPDetails.EndpointSystemName = oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.Id == inhusr.EndPointSystemId).EndPointSystemType.EndpointSystemTypeName.ToString();
                                    if (!string.IsNullOrEmpty(inhusr.UserName))
                                    {
                                        aEPDetails.EndpointUserName = inhusr.UserName;
                                    }
                                    if (orgDetail != null)
                                    {
                                        aEPDetails.OrganisationName = orgDetail.OrganisationName;
                                    }
                                    retList.Add(aEPDetails);
                                }
                            }

                        }
                    }
                }
                return retList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<OrganisationReportData> GetAllOrganisationReport()
        {
            IList<Organisation> orgList = new List<Organisation>();
            User aUser = new User();
            IList<OrganisationReportData> retList = new List<OrganisationReportData>();
            OrganisationReportData reportData = new OrganisationReportData();

            try
            {

                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return (from org in oIACMDBContext.Organisations
                            join usr in oIACMDBContext.Users
                           on org.Head equals usr.Id into diuser
                            from diusr in diuser.DefaultIfEmpty()
                            select new OrganisationReportData
                            {
                                organisationId = org.Id,
                                OrganisationName = org.OrganisationName,
                                Scope = org.Scope,
                                ParentOrganisation = org.ParentOrganisation,
                                HeadofDepartmentEmployeeID = diusr.EmployeeId,
                                HeadofDepartmentName = diusr.DisplayName,
                                HeadofDepartmentUserName = diusr.UserName,
                                OrgUnitPrefix = org.OrgUnitPrefix
                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<User> GetDeletedUsers(int EndpointId)
        {
            IList<User> listDeletedUsers = new List<User>();
            try
            {


                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                using (IACMDBContext _context = new IACMDBContext())
                {

                    objDiscoveryProcess = _context.DiscoveryProcess.Where(e => e.Endpointsystemid == EndpointId).FirstOrDefault();
                    if (objDiscoveryProcess != null)
                    {
                        Process process = _context.Process.FirstOrDefault(item => item.ProcessTypeKeyId == objDiscoveryProcess.Id);
                        if (process != null)
                        {
                            listDeletedUsers = _context.Users.Where(d => d.LastModifiedDate < process.ProcessStartAt).ToList();
                        }
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            return listDeletedUsers;


        }

        public List<AccessRequest> GetValidationFailedRequests()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.AccessRequests.Include("User").Include("AdHocAccessRequest")
                        .Where(item => item.ProgressStatus == AccessRequestProgress.ValidationFailed).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<FailedRequestGridData> GetFailedSystemRequests(AccessRequestProgress progressStatus)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from request in _context.AccessRequests.Include("User")
                            join ep in _context.EndpointSystems
                                on request.EndpointSystemId equals ep.Id into detail
                            from ep in detail.DefaultIfEmpty()
                            where request.ProgressStatus == progressStatus && request.RequestSource != AccessRequestSource.AdhocRequest
                            select new FailedRequestGridData
                            {
                                UserId = request.UserId,
                                DisplayName = request.User.DisplayName,
                                Company = request.User.UserExtensionObj.CompanyCode + " - " + request.User.UserExtensionObj.CompanyName,
                                CostCenter = request.User.UserExtensionObj.CostCenter,
                                Department = request.User.UserExtensionObj.DepartmentCode + " - " + request.User.UserExtensionObj.DepartmentName,
                                Division = request.User.UserExtensionObj.DivisionCode + " - " + request.User.UserExtensionObj.DivisionName,
                                Section = request.User.UserExtensionObj.SectionCode + " - " + request.User.UserExtensionObj.SectionName,
                                AccessRequestId = request.Id,
                                SystemName = ep.DisplayName,
                                Remarks = request.Remarks,
                                EmployeeID = request.User.EmployeeId,
                                RequestDate = request.ExecutionTime.Value,
                                RequestSource = request.RequestSource
                            }
                                ).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<User> GetOnBoardingUsers(bool forAdminCheck)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (forAdminCheck)
                    {
                        return (from item in _context.Users
                                join trig in _context.EmployeeEventTriggers
                                    on item.Id equals trig.UserId
                                where trig.IsAdminCheckRequired && trig.IsRequestCreated == false && trig.IsActive &&
                                      trig.EventType == EmployeeEvents.NewJoinee && trig.EffectiveDate <= DateTime.Now
                                select item).ToList();
                    }
                    return (from item in _context.Users
                            join trig in _context.EmployeeEventTriggers
                                on item.Id equals trig.UserId
                            where trig.IsAdminCheckRequired == false && trig.IsRequestCreated == false && trig.IsActive &&
                                  trig.EventType == EmployeeEvents.NewJoinee && trig.EffectiveDate <= DateTime.Now
                            select item).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<User> GetOffBoardingUsers()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from item in _context.Users
                            join trig in _context.EmployeeEventTriggers
                                on item.Id equals trig.UserId
                            where trig.IsRequestCreated == false && trig.IsActive &&
                                  trig.EventType == EmployeeEvents.Resignation &&
                                  trig.EffectiveDate <= DateTime.Now
                            select item).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<User> GetUsersForDelete()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from item in _context.Users
                            join trig in _context.EmployeeEventTriggers
                                on item.Id equals trig.UserId
                            where trig.IsRequestCreated == false && trig.IsActive &&
                                  trig.EventType == EmployeeEvents.PostResignation &&
                                  trig.EffectiveDate <= DateTime.Now
                            select item).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<UserAccessDetailMatrixGridData> GetUserAccessDetailMatrixGridData(long userId)
        {
            IList<UserAccessDetailMatrixGridData> retList = new List<UserAccessDetailMatrixGridData>();
            UserAccessDetailMatrixGridData aEPDetails = new UserAccessDetailMatrixGridData();

            IList<ADSUserInfo> listADSUserInfo = new List<ADSUserInfo>();
            IList<SAPUserinfo> listSAPUserInfo = new List<SAPUserinfo>();
            IList<LotusUserInfo> listLNSUserInfo = new List<LotusUserInfo>();
            IList<SAPEPUserInfo> listSAPEPUserInfo = new List<SAPEPUserInfo>();
            IList<InhouseUserInfo> listINHUserInfo = new List<InhouseUserInfo>();

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    User usr = oIACMDBContext.Users.Where(_usr => _usr.Id == userId).FirstOrDefault();
                    if (usr != null)
                    {
                        if (usr.Id != 1)
                        {
                            listADSUserInfo = oIACMDBContext.ADSUserInfos.Where(adsusr => adsusr.DIUserId == usr.Id && adsusr.IsLocked == false).ToList();
                            listSAPUserInfo = oIACMDBContext.SAPUserinfos.Where(sapusr => sapusr.DIUserId == usr.Id && sapusr.IsLocked == false).ToList();
                            listLNSUserInfo = oIACMDBContext.LotusUserInfos.Where(lnsusr => lnsusr.DIUserId == usr.Id && lnsusr.IsLocked == false).ToList();
                            listSAPEPUserInfo = oIACMDBContext.SAPEPUserInfos.Where(sapepusr => sapepusr.DIUserId == usr.Id && sapepusr.IsLocked == false).ToList();
                            listINHUserInfo = oIACMDBContext.InhouseUserInfos.Where(INHusr => INHusr.DIUserId == usr.Id && INHusr.IsDeleted == false && INHusr.IsLocked == false).ToList();

                            //aEPDetails = new UserAccessDetailMatrixGridData();
                            //aEPDetails.Id = 1;
                            //aEPDetails.Name = usr.UserName;
                            //retList.Add(aEPDetails);

                            foreach (ADSUserInfo adusr in listADSUserInfo)
                            {
                                if (adusr != null)
                                {
                                    aEPDetails = new UserAccessDetailMatrixGridData();

                                    //aEPDetails.ParentId = 1; // usr.UserId;
                                    aEPDetails.Id = retList.Count + 1; // adusr.UserId;

                                    //aEPDetails.userName = usr.UserName;
                                    aEPDetails.EndpointHostName = oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.Id == adusr.EndPointSystemId).DisplayName;
                                    aEPDetails.EndpointSystemName = oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.Id == adusr.EndPointSystemId).EndPointSystemType.EndpointSystemTypeName.ToString();
                                    aEPDetails.Name = adusr.sAMAccountName;

                                    aEPDetails.EndpointSystemID = adusr.EndPointSystemId;
                                    aEPDetails.FullName = usr.DisplayName;
                                    aEPDetails.LastLogonDate = adusr.lastLogon;
                                    aEPDetails.Type = "User"; //Constants.SECURITY_GROUP; // "Groups";
                                    aEPDetails.userName = usr.UserName;
                                    aEPDetails.AccountType = adusr.AccountType.ToString();

                                    retList.Add(aEPDetails);

                                    var adGrpList = (from adsGrp in adusr.ADSGroups
                                                     where adsGrp.groupTypeDesc == Constants.SECURITY_GROUP
                                                     select new UserAccessDetailMatrixGridData
                                                     {
                                                         ParentId = aEPDetails.Id,
                                                         Name = adsGrp.cn, // adsGrp.description
                                                         Description = adsGrp.description,
                                                         Type = "Group"
                                                     }
                                                    ).ToList();

                                    foreach (var adgrp in adGrpList)
                                    {
                                        aEPDetails.EndpointSystemID = adusr.EndPointSystemId;

                                        aEPDetails = new UserAccessDetailMatrixGridData();
                                        aEPDetails.ParentId = adgrp.ParentId;
                                        aEPDetails.Id = retList.Count + 1;
                                        aEPDetails.Name = adgrp.Name;
                                        aEPDetails.Description = adgrp.Description;
                                        aEPDetails.Type = adgrp.Type;
                                        retList.Add(aEPDetails);
                                    }
                                }
                            }

                            foreach (SAPUserinfo sapusr in listSAPUserInfo)
                            {
                                if (sapusr != null)
                                {
                                    aEPDetails = new UserAccessDetailMatrixGridData();

                                    //aEPDetails.ParentId = 1; // usr.UserId;
                                    aEPDetails.Id = retList.Count + 1; // sapusr.UserId;

                                    //aEPDetails.userName = usr.UserName;
                                    aEPDetails.EndpointHostName = oIACMDBContext.EndpointSystems.FirstOrDefault(eps => eps.Id == sapusr.EndPointSystemId).DisplayName;
                                    aEPDetails.EndpointSystemName = oIACMDBContext.EndpointSystems.FirstOrDefault(eps => eps.Id == sapusr.EndPointSystemId).EndPointSystemType.EndpointSystemTypeName.ToString();
                                    aEPDetails.Name = sapusr.UserLogonName;
                                    aEPDetails.FullName = usr.DisplayName;
                                    aEPDetails.EndpointSystemID = sapusr.EndPointSystemId;
                                    aEPDetails.LastLogonDate = sapusr.LastLoginTime;
                                    aEPDetails.userName = usr.UserName;
                                    aEPDetails.Type = "User"; // "Roles";
                                    aEPDetails.AccountType = sapusr.AccountType.ToString();
                                    retList.Add(aEPDetails);
                                    int pId = aEPDetails.Id;
                                    var sapRoleList = (from sapRole in sapusr.SAPRoleInfo
                                                       select new UserAccessDetailMatrixGridData
                                                       {
                                                           ParentId = aEPDetails.Id,
                                                           Name = sapRole.ROLE_CODE,
                                                           Description = sapRole.ROLE_DESC,
                                                           Type = "Role",
                                                           EndpointSystemID = sapusr.EndPointSystemId
                                                       }
                                                      ).ToList();

                                    //IList<SAPUserRoleTCodeAuth> UserRoleTCodeAuth = new SAPConnectorDA().GetSapUserRoleTCodeAuthReport(sapusr.UserId, sapusr.EndPointSystemId, true);

                                    foreach (var saprole in sapRoleList)
                                    {
                                        aEPDetails = new UserAccessDetailMatrixGridData();
                                        aEPDetails.ParentId = saprole.ParentId;
                                        aEPDetails.Id = retList.Count + 1;
                                        aEPDetails.Name = saprole.Name;
                                        aEPDetails.Description = saprole.Description;
                                        aEPDetails.Type = saprole.Type;
                                        aEPDetails.EndpointSystemID = sapusr.EndPointSystemId;
                                        aEPDetails.userName = usr.UserName;
                                        retList.Add(aEPDetails);
                                    }

                                    EndPoint Endpoint = new EndPointDA().GetEndPointSystemByKey(sapusr.EndPointSystemId);
                                    var sapPositionList = new SAPConnectorDA().GetPositionTextByUser(sapusr.UserLogonName, sapusr.EndPointSystemId);
                                    if (sapPositionList != null)
                                    {
                                        int ParentId = aEPDetails.Id;
                                        foreach (var sapPosition in sapPositionList)
                                        {
                                            aEPDetails = new UserAccessDetailMatrixGridData();
                                            aEPDetails.ParentId = pId;
                                            aEPDetails.Id = retList.Count + 1;
                                            aEPDetails.Name = sapPosition.Value;
                                            aEPDetails.Type = "Position";
                                            aEPDetails.EndpointSystemID = sapusr.EndPointSystemId;
                                            aEPDetails.userName = usr.UserName;
                                            retList.Add(aEPDetails);
                                        }
                                    }
                                }
                            }

                            foreach (LotusUserInfo lnsusr in listLNSUserInfo)
                            {
                                if (lnsusr != null)
                                {
                                    aEPDetails = new UserAccessDetailMatrixGridData();

                                    //aEPDetails.ParentId = 1; // usr.UserId;
                                    aEPDetails.Id = retList.Count + 1; // lnsusr.UserId;

                                    //aEPDetails.userName = usr.UserName;
                                    aEPDetails.EndpointHostName = oIACMDBContext.EndpointSystems.FirstOrDefault(eps => eps.Id == lnsusr.EndPointSystemId).DisplayName;
                                    aEPDetails.EndpointSystemName = oIACMDBContext.EndpointSystems.FirstOrDefault(eps => eps.Id == lnsusr.EndPointSystemId).EndPointSystemType.EndpointSystemTypeName.ToString();
                                    aEPDetails.Name = lnsusr.ShortName; // InternetAddress; // 
                                    aEPDetails.FullName = usr.DisplayName;
                                    //aEPDetails.LastLogonDate = lnsusr.lastLogon;
                                    aEPDetails.EndpointSystemID = lnsusr.EndPointSystemId;
                                    aEPDetails.userName = usr.UserName;
                                    aEPDetails.Type = "User"; // "Groups";
                                    aEPDetails.AccountType = lnsusr.AccountType.ToString();

                                    retList.Add(aEPDetails);

                                    var lnsGrpList = (from lnsGrp in lnsusr.LotusGroupInfos
                                                      where (!(string.IsNullOrEmpty(lnsGrp.Group)))
                                                      select new UserAccessDetailMatrixGridData
                                                      {
                                                          ParentId = aEPDetails.Id,
                                                          Name = lnsGrp.Group,
                                                          Description = lnsGrp.Description,
                                                          Type = "Group",
                                                          EndpointSystemID = lnsusr.EndPointSystemId
                                                      }
                                                      ).ToList();

                                    foreach (var lnsgrp in lnsGrpList)
                                    {
                                        aEPDetails = new UserAccessDetailMatrixGridData();
                                        aEPDetails.ParentId = lnsgrp.ParentId;
                                        aEPDetails.Id = retList.Count + 1;
                                        aEPDetails.Name = lnsgrp.Name;
                                        aEPDetails.Description = lnsgrp.Description;
                                        aEPDetails.Type = lnsgrp.Type;
                                        aEPDetails.EndpointSystemID = lnsusr.EndPointSystemId;
                                        retList.Add(aEPDetails);
                                    }
                                }
                            }

                            foreach (SAPEPUserInfo sapepusr in listSAPEPUserInfo)
                            {
                                if (sapepusr != null)
                                {
                                    aEPDetails = new UserAccessDetailMatrixGridData();

                                    aEPDetails.Id = retList.Count + 1; // sapusr.UserId;

                                    //aEPDetails.userName = usr.UserName;
                                    aEPDetails.EndpointHostName = oIACMDBContext.EndpointSystems.FirstOrDefault(eps => eps.Id == sapepusr.EndPointSystemId).DisplayName;
                                    aEPDetails.EndpointSystemName = oIACMDBContext.EndpointSystems.FirstOrDefault(eps => eps.Id == sapepusr.EndPointSystemId).EndPointSystemType.EndpointSystemTypeName.ToString();
                                    aEPDetails.Name = sapepusr.UserName;
                                    aEPDetails.FullName = usr.DisplayName;
                                    aEPDetails.LastLogonDate = sapepusr.UserLastLogOnTime;
                                    aEPDetails.Type = "User"; // "Roles";
                                    aEPDetails.EndpointSystemID = sapepusr.EndPointSystemId;
                                    aEPDetails.userName = usr.UserName;
                                    aEPDetails.AccountType = sapepusr.AccountType.ToString();

                                    retList.Add(aEPDetails);
                                    int _parentId = aEPDetails.Id;

                                    var sapepRoleList = (from sapepRole in sapepusr.SAPEPRoles
                                                         select new UserAccessDetailMatrixGridData
                                                         {
                                                             ParentId = aEPDetails.Id,
                                                             Name = sapepRole.RoleName,
                                                             Type = "Role",
                                                             Description = sapepRole.RoleDescription,
                                                             EndpointSystemID = sapepusr.EndPointSystemId
                                                         }
                                                      ).ToList();

                                    foreach (var sapeprole in sapepRoleList)
                                    {
                                        aEPDetails = new UserAccessDetailMatrixGridData();
                                        aEPDetails.ParentId = sapeprole.ParentId;
                                        aEPDetails.Id = retList.Count + 1;
                                        aEPDetails.Name = sapeprole.Name;
                                        aEPDetails.Description = sapeprole.Description;
                                        aEPDetails.Type = sapeprole.Type;
                                        aEPDetails.EndpointSystemID = sapepusr.EndPointSystemId;
                                        retList.Add(aEPDetails);
                                    }

                                    var sapepGroupList = (from sapepGroup in sapepusr.SAPEPGroups
                                                          select new UserAccessDetailMatrixGridData
                                                          {
                                                              ParentId = _parentId,
                                                              Name = sapepGroup.GroupName,
                                                              Type = "Group",
                                                              EndpointSystemID = sapepusr.EndPointSystemId
                                                          }
                                                      ).ToList();

                                    foreach (var sapepgroup in sapepGroupList)
                                    {
                                        aEPDetails = new UserAccessDetailMatrixGridData();
                                        aEPDetails.ParentId = sapepgroup.ParentId;
                                        aEPDetails.Id = retList.Count + 1;
                                        aEPDetails.Name = sapepgroup.Name;
                                        aEPDetails.Type = sapepgroup.Type;
                                        aEPDetails.EndpointSystemID = sapepusr.EndPointSystemId;
                                        retList.Add(aEPDetails);
                                    }
                                }
                            }

                            foreach (InhouseUserInfo inhusr in listINHUserInfo)
                            {
                                if (inhusr != null)
                                {
                                    aEPDetails = new UserAccessDetailMatrixGridData();

                                    aEPDetails.Id = retList.Count + 1; // adusr.UserId;

                                    aEPDetails.EndpointHostName = oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.Id == inhusr.EndPointSystemId).DisplayName;
                                    aEPDetails.EndpointSystemName = oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.Id == inhusr.EndPointSystemId).EndPointSystemType.EndpointSystemTypeName.ToString();
                                    //aEPDetails.ApplicationType = Convert.ToString((ApplicationType)(oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.EndpointSystemId == inhusr.EndPointSystemId).EntitlementDetailTypeId));
                                    aEPDetails.EndpointSystemID = inhusr.EndPointSystemId;
                                    aEPDetails.Name = inhusr.UserName;
                                    aEPDetails.FullName = usr.DisplayName;
                                    aEPDetails.LastLogonDate = inhusr.LastLoginDate;
                                    aEPDetails.Type = "User";
                                    aEPDetails.userName = usr.UserName;
                                    aEPDetails.AccountType = aEPDetails.AccountType;

                                    retList.Add(aEPDetails);

                                    var permissions = inhusr.Permission.Split(',');
                                    var permissionList = (from permlist in permissions
                                                          select new UserAccessDetailMatrixGridData
                                                          {
                                                              ParentId = aEPDetails.Id,
                                                              Name = permlist,
                                                              Description = aEPDetails.Description,
                                                              Type = "Permission",
                                                              EndpointSystemID = inhusr.EndPointSystemId
                                                          }
                                                      ).ToList();

                                    foreach (var perm in permissionList)
                                    {

                                        aEPDetails = new UserAccessDetailMatrixGridData();
                                        aEPDetails.ParentId = perm.ParentId;
                                        aEPDetails.Id = retList.Count + 1;
                                        aEPDetails.Name = perm.Name;
                                        aEPDetails.Description = perm.Description;
                                        aEPDetails.Type = perm.Type;
                                        aEPDetails.EndpointSystemID = inhusr.EndPointSystemId;
                                        retList.Add(aEPDetails);
                                    }
                                }
                            }
                        }
                    }
                }
                return retList;
            }
            catch (Exception)
            {

                throw;
            }
        }



        public IList<UsersByGroupReportData> getUsersByGroupReportList()
        {
            IList<UsersByGroupReportData> returnList = new List<UsersByGroupReportData>();
            UsersByGroupReportData aData = new UsersByGroupReportData();
            User usr = new User();
            IList<Group> grps = new List<Group>();

            int index = 0;

            try
            {
                using (IACMDBContext _iacmdbContext = new IACMDBContext())
                {
                    grps = _iacmdbContext.Groups.Include("Users").ToList();

                    foreach (Group grp in grps)
                    {
                        if (grp.Users.Count() > 0)
                        {
                            for (index = 0; index < grp.Users.Count(); index++)
                            {
                                usr = grp.Users[index];

                                aData = new UsersByGroupReportData();
                                aData.grpName = "Group Name: " + grp.GroupName + ";  Group Owner: " + grp.GroupOwner + ";  Group Description: " + grp.GroupDescription;
                                aData.userDisplayName = usr.DisplayName;
                                aData.userId = usr.Id;
                                aData.userMailId = usr.EmailId;
                                aData.userEmployeeId = usr.EmployeeId;
                                returnList.Add(aData);
                            }
                        }
                        else
                        {
                            aData = new UsersByGroupReportData();
                            aData.grpName = grp.GroupName;
                            returnList.Add(aData);
                        }
                    }
                    return returnList;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<UsersByDepartmentReportData> getUsersByDepartmentReportList()
        {
            IList<UsersByDepartmentReportData> returnList = new List<UsersByDepartmentReportData>();

            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    returnList = (from usr in _iacmdbcontext.Users
                                  join org in _iacmdbcontext.Organisations
                                  on usr.OrganisationId equals org.Id
                                  where usr.OrganisationId == org.Id
                                  orderby usr.UserName ascending
                                  select new UsersByDepartmentReportData
                                  {
                                      deptName = org.OrganisationName,
                                      userDisplayName = usr.DisplayName,
                                      userEmployeeId = usr.EmployeeId,
                                      userId = usr.Id,
                                      userMailId = usr.EmailId,
                                      userUserName = usr.UserName
                                  }).ToList();

                    return returnList;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<UsersByManagerReportData> getUsersByManagerReportList()
        {
            IList<UsersByManagerReportData> returnList = new List<UsersByManagerReportData>();
            IList<UsersByManagerReportData> tempList = new List<UsersByManagerReportData>();
            IList<User> managersList = new List<User>();

            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    managersList = _iacmdbcontext.Users.Where(usr => usr.ManagerId == usr.Id).ToList();

                    foreach (User mgr in managersList)
                    {
                        tempList = (from usrs in _iacmdbcontext.Users
                                    where usrs.ManagerId == mgr.Id &&
                                    usrs.Id != mgr.Id
                                    select new UsersByManagerReportData
                                    {
                                        managerName = mgr.UserName,
                                        userDisplayName = usrs.DisplayName,
                                        userEmployeeId = usrs.EmployeeId,
                                        userFirstName = usrs.FirstName,
                                        userId = usrs.Id,
                                        userInstitution = usrs.TenantId,
                                        userLastName = usrs.LastName,
                                        userMailId = usrs.EmailId,
                                        userUserName = usrs.UserName
                                    }).ToList();


                        returnList = returnList.Concat(tempList).ToList();
                    }

                    return returnList;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<UsersByManagerReportData> getUsersByWithoutManagerReportList()
        {
            IList<UsersByManagerReportData> returnList = new List<UsersByManagerReportData>();

            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    returnList = (from usrs in _iacmdbcontext.Users
                                  where usrs.ManagerId == null &&
                                  usrs.Id != 1 &&
                                  usrs.IsActive == true
                                  select new UsersByManagerReportData
                                  {
                                      userDisplayName = usrs.DisplayName,
                                      userEmployeeId = usrs.EmployeeId,
                                      userFirstName = usrs.FirstName,
                                      userId = usrs.Id,
                                      userInstitution = usrs.TenantId,
                                      userLastName = usrs.LastName,
                                      userMailId = usrs.EmailId,
                                      userUserName = usrs.UserName
                                  }).ToList();

                    return returnList;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public IList<UserPasswordCountGridData> getUserPasswordCountReportData(string userName)
        {
            try
            {
                List<UserPasswordCountGridData> _userPasswordCountList = new List<UserPasswordCountGridData>();
                UserPasswordCountGridData UserPasswordCountItem;
                List<UserPasswordCountGridData> ADSquery = new List<UserPasswordCountGridData>();
                List<UserPasswordCountGridData> SAPquery = new List<UserPasswordCountGridData>();
                List<UserPasswordCountGridData> SAPEPquery = new List<UserPasswordCountGridData>();
                List<UserPasswordCountGridData> Lotusquery = new List<UserPasswordCountGridData>();

                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    ADSquery = (from usr in oIACMDBContext.ADSUserInfos
                                join es in oIACMDBContext.EndpointSystems on usr.EndPointSystemId equals es.Id
                                join esType in oIACMDBContext.EndpointSystemTypes on es.EndpointSystemTypeId equals esType.Id
                                join musr in oIACMDBContext.Users on usr.DIUserId equals musr.Id
                                where ((musr.UserName.Contains(userName) || userName == "All") && usr.DIUserId != null)
                                select new UserPasswordCountGridData
                                {
                                    UserName = musr.UserName,
                                    EndpointSystemTypeName = esType.DisplayName,
                                    LoginID = usr.sAMAccountName,
                                    EndpointSystem = es.DisplayName,
                                    PasswordCount = usr.PwdResetCnt
                                }).ToList();

                    SAPquery = (from usr in oIACMDBContext.SAPUserinfos
                                join es in oIACMDBContext.EndpointSystems on usr.EndPointSystemId equals es.Id
                                join esType in oIACMDBContext.EndpointSystemTypes on es.EndpointSystemTypeId equals esType.Id
                                join musr in oIACMDBContext.Users on usr.DIUserId equals musr.Id
                                where ((musr.UserName.Contains(userName) || userName == "All") && usr.DIUserId != null)
                                select new UserPasswordCountGridData
                                {
                                    UserName = musr.UserName,
                                    EndpointSystemTypeName = esType.DisplayName,
                                    LoginID = usr.UserLogonName,
                                    EndpointSystem = es.DisplayName,
                                    PasswordCount = usr.PwdResetCnt
                                }).ToList();

                    SAPEPquery = (from usr in oIACMDBContext.SAPEPUserInfos
                                  join es in oIACMDBContext.EndpointSystems on usr.EndPointSystemId equals es.Id
                                  join esType in oIACMDBContext.EndpointSystemTypes on es.EndpointSystemTypeId equals esType.Id
                                  join musr in oIACMDBContext.Users on usr.DIUserId equals musr.Id
                                  where ((musr.UserName.Contains(userName) || userName == "All") && usr.DIUserId != null)
                                  select new UserPasswordCountGridData
                                  {
                                      UserName = musr.UserName,
                                      EndpointSystemTypeName = esType.DisplayName,
                                      LoginID = usr.UserName,
                                      EndpointSystem = es.DisplayName,
                                      PasswordCount = usr.PwdResetCnt
                                  }).ToList();

                    Lotusquery = (from usr in oIACMDBContext.LotusUserInfos
                                  join es in oIACMDBContext.EndpointSystems on usr.EndPointSystemId equals es.Id
                                  join esType in oIACMDBContext.EndpointSystemTypes on es.EndpointSystemTypeId equals esType.Id
                                  join musr in oIACMDBContext.Users on usr.DIUserId equals musr.Id
                                  where ((musr.UserName.Contains(userName) || userName == "All") && usr.DIUserId != null)
                                  select new UserPasswordCountGridData
                                  {
                                      UserName = musr.UserName,
                                      EndpointSystemTypeName = esType.DisplayName,
                                      LoginID = usr.InternetAddress,
                                      EndpointSystem = es.DisplayName,
                                      PasswordCount = usr.PwdResetCnt
                                  }).ToList();

                    /* var EXquery = (from usr in oIACMDBContext.ExUserInfos
                                    join es in oIACMDBContext.EndpointSystems on usr.EndPointSystemId equals es.EndpointSystemId
                                    join esType in oIACMDBContext.EndpointSystemTypes on es.EndpointSystemTypeId equals esType.EndpointSystemTypeId
                                    join musr in oIACMDBContext.Users on usr.DIUserId equals musr.UserId
                                    where ((musr.UserName.Contains(userName) || userName == "All") && usr.DIUserId != null)
                                    select new UserPasswordCountGridData()
                                    {
                                        UserName = musr.UserName,
                                        EndpointSystemTypeName = esType.EndpointSystemTypeName,
                                        LoginID = usr.sAMAccountName,
                                        EndpointSystem = es.HostName,
                                        PasswordCount = usr.PwdResetCnt
                                    }).ToList();
                     
                    var WinSoftquery = (from usr in oIACMDBContext.WindowsUserInfos
                                       join es in oIACMDBContext.EndpointSystems on usr.EndPointSystemId equals es.EndpointSystemId
                                       join esType in oIACMDBContext.EndpointSystemTypes on es.EndpointSystemTypeId equals esType.EndpointSystemTypeId
                                       join musr in oIACMDBContext.Users on usr.DIUserId equals musr.UserId
                                       where ((musr.UserName.Contains(userName) || userName == "All") && usr.DIUserId != null)
                                       select new UserPasswordCountGridData()
                                       {
                                           UserName = musr.UserName,
                                           EndpointSystemTypeName = esType.EndpointSystemTypeName,
                                           LoginID = usr.Name,
                                           EndpointSystem = es.HostName,
                                           PasswordCount = usr.PwdResetCnt
                                       }).ToList();

                    var SISquery = (from usr in oIACMDBContext.SISUserInfos
                                        join es in oIACMDBContext.EndpointSystems on usr.EndPointSystemId equals es.EndpointSystemId
                                        join esType in oIACMDBContext.EndpointSystemTypes on es.EndpointSystemTypeId equals esType.EndpointSystemTypeId
                                        join musr in oIACMDBContext.Users on usr.DIUserId equals musr.UserId
                                        where ((musr.UserName.Contains(userName) || userName == "All") && usr.DIUserId != null)
                                        select new UserPasswordCountGridData()
                                        {
                                            UserName = musr.UserName,
                                            EndpointSystemTypeName = esType.EndpointSystemTypeName,
                                            LoginID = usr.Email,
                                            EndpointSystem = es.HostName,
                                            PasswordCount = usr.PwdResetCnt
                                        }).ToList();

                    var sqlumquery = (from usr in oIACMDBContext.SQLUMUserInfos
                                    join es in oIACMDBContext.EndpointSystems on usr.EndPointSystemId equals es.EndpointSystemId
                                    join esType in oIACMDBContext.EndpointSystemTypes on es.EndpointSystemTypeId equals esType.EndpointSystemTypeId
                                    join musr in oIACMDBContext.Users on usr.DIUserId equals musr.UserId
                                    where ((musr.UserName.Contains(userName) || userName == "All") && usr.DIUserId != null)
                                    select new UserPasswordCountGridData()
                                    {
                                        UserName = musr.UserName,
                                        EndpointSystemTypeName = esType.EndpointSystemTypeName,
                                        LoginID = usr.Name,
                                        EndpointSystem = es.HostName,
                                        PasswordCount = usr.PwdResetCnt
                                    }).ToList(); */

                    var listTotal1 = ADSquery.Union(SAPquery);
                    var listTotal2 = listTotal1.Union(SAPEPquery);
                    var listTotal = listTotal2.Union(Lotusquery);

                    if (listTotal.Count() > 0)
                    {
                        foreach (var tmpList in listTotal)
                        {
                            UserPasswordCountItem = new UserPasswordCountGridData();
                            UserPasswordCountItem.UserName = CheckNull.AgainstString(tmpList.UserName);
                            UserPasswordCountItem.EndpointSystemTypeName = CheckNull.AgainstString(tmpList.EndpointSystemTypeName);
                            UserPasswordCountItem.LoginID = CheckNull.AgainstString(tmpList.LoginID);
                            UserPasswordCountItem.EndpointSystem = CheckNull.AgainstString(tmpList.EndpointSystem);
                            UserPasswordCountItem.PasswordCount = CheckNull.AgainstInt(tmpList.PasswordCount);
                            _userPasswordCountList.Add(UserPasswordCountItem);
                        }
                    }

                }

                return _userPasswordCountList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<UserListGridData> getUsersListByAttestation(int attestationId)
        {
            try
            {
                IList<UserListGridData> _userList = new List<UserListGridData>();

                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    _userList = (from attstn in oIACMDBContext.Attestations
                                 join tsk in oIACMDBContext.Tasks on (int)attstn.Id equals tsk.AttestationId
                                 join tskrvw in oIACMDBContext.TaskReviews on tsk.Id equals tskrvw.TaskId
                                 join attrvw in oIACMDBContext.AttestationReviews on tskrvw.ReviewId equals attrvw.Id
                                 join usr in oIACMDBContext.Users on attrvw.UserId equals usr.Id
                                 join org in oIACMDBContext.Organisations on usr.OrganisationId equals org.Id
                                 where (attstn.Id == attestationId)
                                 select new UserListGridData
                                 {
                                     UserName = usr.UserName,
                                     FirstName = usr.FirstName,
                                     MiddleName = usr.MiddleName,
                                     LastName = usr.LastName,
                                     DisplayName = usr.DisplayName,
                                     EmployeeId = usr.EmployeeId,
                                     OrganisationName = org.OrganisationName, // OrganisationId = usr.OrganisationId,
                                     EmailId = usr.EmailId
                                 }).ToList();
                }
                return _userList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Designation getDesignationByDesignationDesc(string _designationDesc)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Designations.FirstOrDefault(dsegn => dsegn.DesignationDesc == _designationDesc);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<Designation> GetAllBusinessRolesList(string _tenantId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (string.IsNullOrEmpty(_tenantId))
                        return _context.Designations.ToList();
                    return _context.Designations.ToList(); // TODO: TenantId Info // _context.Designations.Where(item => item.TenantId == _tenantId).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured in GetAllBusinessRole ", ex);
                throw ex;
            }
        }

        public string getBusinessRoleById(long _businessRoleId)
        {
            try
            {
                string _businessRole = "";
                using (IACMDBContext _context = new IACMDBContext())
                {
                    Designation _designation = _context.Designations.FirstOrDefault(dsegn => dsegn.Id == _businessRoleId);
                    if (_designation != null) _businessRole = _designation.DesignationDesc;
                }
                return _businessRole;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }







        public List<DeprovisioningUserDetailsGridData> GetDeprovisioningUserDetailsGridDataByUserId(int UserId)
        {
            try
            {
                List<DeprovisioningUserDetailsGridData> userList = new List<DeprovisioningUserDetailsGridData>();

                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    userList = (from usr in oIACMDBContext.Users.Include("Organisation").Include("BusinessRole")
                                //join busrole in oIACMDBContext.Designations on usr.BusinessRoleId equals busrole.BusinessRoleId
                                //join org in oIACMDBContext.Organisations on usr.OrganisationId equals org.OrganisationId
                                where usr.Id == UserId
                                select new DeprovisioningUserDetailsGridData
                               {
                                   UserId = usr.Id,
                                   UserName = usr.UserName,
                                   DisplayName = usr.DisplayName,
                                   Department = usr.Organisation != null ? usr.Organisation.OrganisationName : string.Empty, //org.OrganisationName,
                                   BusinessRole = usr.BusinessRole != null ? usr.BusinessRole.DesignationDesc : string.Empty, //busrole.DesignationDesc,
                                   TenantId = usr.TenantId
                               }).ToList();
                }
                return userList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<UserAccessDetailMatrixGridData> getUserAccessDetailsReport(int _endPointSystemTypeId, int _endPointSystemId, int _managerId)
        {
            IList<UserAccessDetailMatrixGridData> retList = new List<UserAccessDetailMatrixGridData>();
            IList<User> usersList = new List<User>();
            UserAccessDetailMatrixGridData aEPDetails = new UserAccessDetailMatrixGridData();
            Organisation orgDetail = new Organisation();

            IList<ADSUserInfo> listADSUserInfo = new List<ADSUserInfo>();
            IList<SAPUserinfo> listSAPUserInfo = new List<SAPUserinfo>();
            IList<LotusUserInfo> listLNSUserInfo = new List<LotusUserInfo>();
            IList<SAPEPUserInfo> listSAPEPUserInfo = new List<SAPEPUserInfo>();

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (_endPointSystemTypeId == -1 || _endPointSystemTypeId == (int)EndpointType.ActiveDirectory)
                    {
                        //listADSUserInfo = oIACMDBContext.ADSUserInfos.Where(adsusr => adsusr.DIUserId == usr.UserId).ToList();
                        retList = retList.Union(from adsusr in oIACMDBContext.ADSUserInfos
                                                join usr in oIACMDBContext.Users on adsusr.DIUserId equals usr.Id
                                                join ep in oIACMDBContext.EndpointSystems on adsusr.EndPointSystemId equals ep.Id
                                                //join epsType in oIACMDBContext.EndpointSystems on ep.EndpointSystemTypeId equals epsType.EndpointSystemTypeId
                                                where usr.ManagerId == _managerId && (ep.Id == _endPointSystemId || _endPointSystemId == -1)
                                                //select adsusr).ToList();
                                                select new UserAccessDetailMatrixGridData
                                                {
                                                    userName = usr.UserName,
                                                    Institution = usr.TenantId,
                                                    EndpointHostName = ep.DisplayName, // oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.EndpointSystemId == adusr.EndPointSystemId).HostName,
                                                    EndpointSystemName = EndpointType.ActiveDirectory.ToString(), // oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.EndpointSystemId == adusr.EndPointSystemId).EndPointSystemType.EndpointSystemTypeName.ToString();
                                                    Name = (!string.IsNullOrEmpty(adsusr.sAMAccountName)) ? adsusr.sAMAccountName : string.Empty,
                                                    LastLogonDate = adsusr.lastLogon
                                                }).ToList();
                    }


                    if (_endPointSystemTypeId == -1 || _endPointSystemTypeId == (int)EndpointType.SAP)
                    {
                        //listSAPUserInfo = oIACMDBContext.SAPUserinfos.Where(sapusr => sapusr.DIUserId == usr.UserId).ToList();
                        retList = retList.Union(from sapusr in oIACMDBContext.SAPUserinfos
                                                join usr in oIACMDBContext.Users on sapusr.DIUserId equals usr.Id
                                                join ep in oIACMDBContext.EndpointSystems on sapusr.EndPointSystemId equals ep.Id
                                                where usr.ManagerId == _managerId && (ep.Id == _endPointSystemId || _endPointSystemId == -1)
                                                select new UserAccessDetailMatrixGridData
                                                {
                                                    userName = usr.UserName,
                                                    Institution = usr.TenantId,
                                                    EndpointHostName = ep.DisplayName,
                                                    EndpointSystemName = EndpointType.SAP.ToString(),
                                                    Name = (!string.IsNullOrEmpty(sapusr.UserLogonName)) ? sapusr.UserLogonName : string.Empty,
                                                    LastLogonDate = sapusr.LastLoginTime
                                                }).ToList();
                    }

                    if (_endPointSystemTypeId == -1 || _endPointSystemTypeId == (int)EndpointType.LotusNotes)
                    {
                        //listLNSUserInfo = oIACMDBContext.LotusUserInfos.Where(lnsusr => lnsusr.DIUserId == usr.UserId).ToList();
                        retList = retList.Union(from lnsusr in oIACMDBContext.LotusUserInfos
                                                join usr in oIACMDBContext.Users on lnsusr.DIUserId equals usr.Id
                                                join ep in oIACMDBContext.EndpointSystems on lnsusr.EndPointSystemId equals ep.Id
                                                where usr.ManagerId == _managerId && (ep.Id == _endPointSystemId || _endPointSystemId == -1)
                                                select new UserAccessDetailMatrixGridData
                                                {
                                                    userName = usr.UserName,
                                                    Institution = usr.TenantId,
                                                    EndpointHostName = ep.DisplayName,
                                                    EndpointSystemName = EndpointType.LotusNotes.ToString(),
                                                    Name = (!string.IsNullOrEmpty(lnsusr.ShortName)) ? lnsusr.ShortName : string.Empty
                                                    //LastLogonDate = lnsusr.LastModifiedDate
                                                }).ToList();
                    }

                    if (_endPointSystemTypeId == -1 || _endPointSystemTypeId == (int)EndpointType.SAPEnterprisePortal)
                    {
                        //listSAPEPUserInfo = oIACMDBContext.SAPEPUserInfos.Where(sapepusr => sapepusr.DIUserId == usr.UserId).ToList();
                        retList = retList.Union(from sapepusr in oIACMDBContext.SAPEPUserInfos
                                                join usr in oIACMDBContext.Users on sapepusr.DIUserId equals usr.Id
                                                join ep in oIACMDBContext.EndpointSystems on sapepusr.EndPointSystemId equals ep.Id
                                                where usr.ManagerId == _managerId && (ep.Id == _endPointSystemId || _endPointSystemId == -1)
                                                select new UserAccessDetailMatrixGridData
                                                {
                                                    userName = usr.UserName,
                                                    Institution = usr.TenantId,
                                                    EndpointHostName = ep.DisplayName,
                                                    EndpointSystemName = EndpointType.SAPEnterprisePortal.ToString(),
                                                    Name = (!string.IsNullOrEmpty(sapepusr.UserName)) ? sapepusr.UserName : string.Empty,
                                                    LastLogonDate = sapepusr.UserLastLogOnTime
                                                }).ToList();
                    }


                }
                return retList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<UserAccessDetailMatrixGridData> getUserGroupRoleAccessMatrixDetailsReport(int _endPointSystemTypeId, int _endPointSystemId, int _managerId)
        {
            IList<UserAccessDetailMatrixGridData> retList = new List<UserAccessDetailMatrixGridData>();
            IList<User> usersList = new List<User>();
            UserAccessDetailMatrixGridData aEPDetails = new UserAccessDetailMatrixGridData();
            Organisation orgDetail = new Organisation();

            IList<ADSUserInfo> listADSUserInfo = new List<ADSUserInfo>();
            IList<SAPUserinfo> listSAPUserInfo = new List<SAPUserinfo>();
            IList<LotusUserInfo> listLNSUserInfo = new List<LotusUserInfo>();
            IList<SAPEPUserInfo> listSAPEPUserInfo = new List<SAPEPUserInfo>();

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (_endPointSystemTypeId == -1 || _endPointSystemTypeId == (int)EndpointType.ActiveDirectory)
                    {
                        //listADSUserInfo = oIACMDBContext.ADSUserInfos.Where(adsusr => adsusr.DIUserId == usr.UserId).ToList();
                        listADSUserInfo = (from adsusr in oIACMDBContext.ADSUserInfos
                                           join usr in oIACMDBContext.Users on adsusr.DIUserId equals usr.Id
                                           join ep in oIACMDBContext.EndpointSystems on adsusr.EndPointSystemId equals ep.Id
                                           //join epsType in oIACMDBContext.EndpointSystems on ep.EndpointSystemTypeId equals epsType.EndpointSystemTypeId
                                           where usr.ManagerId == _managerId && (ep.Id == _endPointSystemId || _endPointSystemId == -1)
                                           select adsusr).ToList();

                        foreach (ADSUserInfo adusr in listADSUserInfo)
                        {
                            retList = retList.Union(from adsGrp in adusr.ADSGroups
                                                    select new UserAccessDetailMatrixGridData
                                                    {
                                                        userName = adusr.sAMAccountName,
                                                        EndpointHostName = oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.Id == adusr.EndPointSystemId).DisplayName,
                                                        Type = "Group",
                                                        Name = adsGrp.sAMAccountName,
                                                        Description = adsGrp.description
                                                    }
                                            ).ToList();
                        }
                    }

                    if (_endPointSystemTypeId == -1 || _endPointSystemTypeId == (int)EndpointType.SAP)
                    {
                        //listSAPUserInfo = oIACMDBContext.SAPUserinfos.Where(sapusr => sapusr.DIUserId == usr.UserId).ToList();
                        listSAPUserInfo = (from sapusr in oIACMDBContext.SAPUserinfos
                                           join usr in oIACMDBContext.Users on sapusr.DIUserId equals usr.Id
                                           join ep in oIACMDBContext.EndpointSystems on sapusr.EndPointSystemId equals ep.Id
                                           //join epsType in oIACMDBContext.EndpointSystems on ep.EndpointSystemTypeId equals epsType.EndpointSystemTypeId
                                           where usr.ManagerId == _managerId && (ep.Id == _endPointSystemId || _endPointSystemId == -1)
                                           select sapusr).ToList();

                        foreach (SAPUserinfo sapusr in listSAPUserInfo)
                        {
                            retList = retList.Union(from sapRole in sapusr.SAPRoleInfo
                                                    select new UserAccessDetailMatrixGridData
                                                    {
                                                        userName = sapusr.UserLogonName,
                                                        EndpointHostName = oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.Id == sapusr.EndPointSystemId).DisplayName,
                                                        Type = "Role",
                                                        Name = sapRole.ROLE_CODE,
                                                        Description = sapRole.ROLE_DESC
                                                    }
                                            ).ToList();

                            retList = retList.Union(from sapGroup in sapusr.SAPGroupInfo
                                                    select new UserAccessDetailMatrixGridData
                                                    {
                                                        userName = sapusr.UserLogonName,
                                                        EndpointHostName = oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.Id == sapusr.EndPointSystemId).DisplayName,
                                                        Type = "Group",
                                                        Name = sapGroup.GROUP_CODE,
                                                        Description = sapGroup.GROUP_DESC
                                                    }
                                            ).ToList();
                        }
                    }

                    if (_endPointSystemTypeId == -1 || _endPointSystemTypeId == (int)EndpointType.LotusNotes)
                    {
                        //listLNSUserInfo = oIACMDBContext.LotusUserInfos.Where(lnsusr => lnsusr.DIUserId == usr.UserId).ToList();
                        listLNSUserInfo = (from lnsusr in oIACMDBContext.LotusUserInfos
                                           join usr in oIACMDBContext.Users on lnsusr.DIUserId equals usr.Id
                                           join ep in oIACMDBContext.EndpointSystems on lnsusr.EndPointSystemId equals ep.Id
                                           where usr.ManagerId == _managerId && (ep.Id == _endPointSystemId || _endPointSystemId == -1)
                                           select lnsusr).ToList();

                        foreach (LotusUserInfo lnsusr in listLNSUserInfo)
                        {
                            retList = retList.Union(from lnsGrp in lnsusr.LotusGroupInfos
                                                    select new UserAccessDetailMatrixGridData
                                                    {
                                                        userName = lnsusr.ShortName,
                                                        EndpointHostName = oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.Id == lnsusr.EndPointSystemId).DisplayName,
                                                        Type = "Group",
                                                        Name = lnsGrp.Group,
                                                        Description = lnsGrp.Description
                                                    }
                                            ).ToList();
                        }
                    }

                    if (_endPointSystemTypeId == -1 || _endPointSystemTypeId == (int)EndpointType.SAPEnterprisePortal)
                    {
                        //listSAPEPUserInfo = oIACMDBContext.SAPEPUserInfos.Where(sapepusr => sapepusr.DIUserId == usr.UserId).ToList();
                        listSAPEPUserInfo = (from sapepusr in oIACMDBContext.SAPEPUserInfos
                                             join usr in oIACMDBContext.Users on sapepusr.DIUserId equals usr.Id
                                             join ep in oIACMDBContext.EndpointSystems on sapepusr.EndPointSystemId equals ep.Id
                                             where usr.ManagerId == _managerId && (ep.Id == _endPointSystemId || _endPointSystemId == -1)
                                             select sapepusr).ToList();

                        foreach (SAPEPUserInfo sapepusr in listSAPEPUserInfo)
                        {
                            retList = retList.Union(from sapepRole in sapepusr.SAPEPRoles
                                                    select new UserAccessDetailMatrixGridData
                                                    {
                                                        userName = sapepusr.UserName,
                                                        EndpointHostName = oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.Id == sapepusr.EndPointSystemId).DisplayName,
                                                        Type = "Role",
                                                        Name = sapepRole.RoleName,
                                                        Description = sapepRole.RoleDescription
                                                    }
                                            ).ToList();

                            retList = retList.Union(from sapepGroup in sapepusr.SAPEPGroups
                                                    select new UserAccessDetailMatrixGridData
                                                    {
                                                        userName = sapepusr.UserName,
                                                        EndpointHostName = oIACMDBContext.EndpointSystems.FirstOrDefault(s => s.Id == sapepusr.EndPointSystemId).DisplayName,
                                                        Type = "Group",
                                                        Name = sapepGroup.GroupName,
                                                        Description = sapepGroup.GroupDescription
                                                    }
                                            ).ToList();
                        }
                    }



                }
                return retList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public long AddUserFromDiscovery(User user)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    oIACMDBContext.Users.Add(user);
                    oIACMDBContext.SaveChanges();
                    return user.Id;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Adding  the User", ex);
                throw ex;
            }
        }

        public void UpdateUserFromDiscovery(User user)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(user);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<User>();
                        User attached = set.Find(user.Id);

                        if (attached != null)
                        {
                            var attachedEntry = _context.Entry(attached);
                            attachedEntry.CurrentValues.SetValues(user);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while updating  the User", ex);
                throw ex;
            }
        }

        public IList<User> GetUserDetails()
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    IList<User> uList = new List<User>();

                    uList = (from usr in oIACMDBContext.Users
                             select usr
                                 ).Distinct().ToList();
                    return uList;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting User Details.", ex);
                throw ex;
            }
        }

        public IList<User> GetAllActiveEmployees()
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.Users.Where(item => item.FullTimeEmployee.Value && item.IsActive.Value).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting User Details.", ex);
                throw ex;
            }
        }

        public string GetEmployeeId(string DefaultEmployeeKey)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {

                    Dictionary<long, string> data = oIACMDBContext.Users.Where(lq => lq.EmployeeId.StartsWith(DefaultEmployeeKey))
                                   .Select(lq => new { Key = lq.Id, Value = lq.EmployeeId }).OrderByDescending(lq => lq.Key).ToDictionary(o => o.Key, o => o.Value);
                    foreach (var items in data)
                    {
                        return items.Value;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting User Details.", ex);
                throw ex;
            }
        }
        public long InsertNewUser(User usr)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    oIACMDBContext.Users.Add(usr);
                    oIACMDBContext.SaveChanges();
                    return usr.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public User GetCompanyDetails(string prefix)
        {
            try
            {
                User uList = new User();
                int userid = Convert.ToInt32(prefix);
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {


                    var res1 = from usr in oIACMDBContext.Users
                               join org in oIACMDBContext.Organisations on usr.OrganisationId equals org.Id into detail
                               from org in detail.DefaultIfEmpty()
                               where usr.Id.Equals(userid)
                               select new
                               {
                                   usr.NRIC,
                                   usr.EmployeeId,
                                   usr.FirstName,
                                   usr.MiddleName,
                                   usr.LastName,
                                   usr.JoinDate,
                                   usr.TerminationDate,
                                   usr.EmployeeType,
                                   usr.Reserved1,
                                   usr.TelephoneNumber,
                                   usr.Country,
                                   usr.Ethinicity,
                                   usr.DisplayName,
                                   usr.UserExtensionObj.CompanyName,
                                   usr.UserExtensionObj.CompanyId,
                                   usr.UserExtensionObj.CompanyCode,
                                   usr.UserExtensionObj.CostCenter,
                                   usr.UserExtensionObj.DepartmentName,
                                   usr.UserExtensionObj.DepartmentId,
                                   usr.UserExtensionObj.DepartmentCode,
                                   usr.UserExtensionObj.SectionCode,
                                   usr.UserExtensionObj.DivisionName,
                                   usr.UserExtensionObj.DivisionId,
                                   usr.UserExtensionObj.DivisionCode,
                                   usr.UserExtensionObj.PositionName,
                                   usr.UserExtensionObj.PositionCode,
                                   usr.UserExtensionObj.Supervisor,
                                   org.OrganisationName,
                                   usr.OrganisationId,
                                   usr.BusinessRoleId,
                                   usr.ManagerId,
                                   usr.UserName,
                                   usr.Password
                               };
                    var record1 = res1.FirstOrDefault();
                    if (record1 != null)
                    {
                        uList.NRIC = record1.NRIC;
                        uList.Password = record1.Password;
                        uList.EmployeeId = record1.EmployeeId;
                        uList.FirstName = record1.FirstName;
                        uList.MiddleName = record1.MiddleName;
                        uList.LastName = record1.LastName;
                        uList.JoinDate = record1.JoinDate;
                        uList.TerminationDate = record1.TerminationDate;
                        uList.EmployeeType = record1.EmployeeType;
                        uList.Reserved1 = record1.Reserved1;
                        uList.TelephoneNumber = record1.TelephoneNumber;
                        uList.Country = record1.Country;
                        uList.Ethinicity = record1.Ethinicity;
                        uList.DisplayName = record1.DisplayName;
                        uList.UserExtensionObj.CompanyName = record1.CompanyName;
                        uList.UserExtensionObj.CompanyId = record1.CompanyId;
                        uList.UserExtensionObj.CompanyCode = record1.CompanyCode;
                        uList.UserExtensionObj.CostCenter = record1.CostCenter;
                        uList.UserExtensionObj.DepartmentName = record1.DepartmentName;
                        uList.UserExtensionObj.DepartmentId = record1.DepartmentId;
                        uList.UserExtensionObj.DepartmentCode = record1.DepartmentCode;
                        uList.UserExtensionObj.SectionCode = record1.SectionCode;
                        uList.UserExtensionObj.DivisionName = record1.DivisionName;
                        uList.UserExtensionObj.DivisionId = record1.DivisionId;
                        uList.UserExtensionObj.DivisionCode = record1.DivisionCode;
                        uList.UserExtensionObj.OrgCompany = record1.OrganisationName;
                        uList.OrganisationId = record1.OrganisationId;
                        uList.ManagerId = record1.ManagerId;
                        uList.UserExtensionObj.PositionName = record1.PositionName;
                        uList.UserExtensionObj.PositionCode = record1.PositionCode;
                        uList.UserExtensionObj.Supervisor = record1.Supervisor;
                        uList.UserName = record1.UserName;

                        int managerid = Convert.ToInt32(record1.ManagerId);

                        var res3 = from usr in oIACMDBContext.Users
                                   where usr.Id == managerid
                                   select new { usr.DisplayName };
                        var record3 = res3.FirstOrDefault();
                        if (record3 != null)
                            uList.Reserved10 = record3.DisplayName;

                        var res4 = from des in oIACMDBContext.Designations
                                   where des.DesignationCode == record1.PositionCode
                                   select new { BusinessRoleId = des.Id };
                        var record4 = res4.FirstOrDefault();
                        if (record4 != null)
                            uList.BusinessRoleId = record4.BusinessRoleId;
                    }
                    return uList;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting User Details.", ex);
                throw ex;
            }
        }
        //public string GetTenantId()
        //{
        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            return oIACMDBContext.Tenants.FirstOrDefault().TenantId;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while getting User Details.", ex);
        //        throw ex;
        //    }
        //}

        public Dictionary<long, string> GetReportingManager(string prefix)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var mList = (from usr in oIACMDBContext.Users where usr.EmployeeType == EmployeeType.Permanent && usr.IsActive == true && usr.DisplayName.Contains(prefix) select new { usr.DisplayName, UserId = usr.Id }).Distinct();
                    return mList.ToDictionary(c => c.UserId, c => c.DisplayName);
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting User Details.", ex);
                throw ex;
            }
        }
        public Dictionary<string, string> GetPosition(string prefix)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var pList = (from usr in oIACMDBContext.Users join des in oIACMDBContext.Designations on usr.UserExtensionObj.PositionCode equals des.DesignationCode where usr.EmployeeType == EmployeeType.Permanent && usr.IsActive == true && usr.UserExtensionObj.PositionName.Contains(prefix) select new { usr.UserExtensionObj.PositionName, usr.UserExtensionObj.PositionCode, BusinessRoleId = des.Id }).Distinct();
                    return pList.ToDictionary(c => c.BusinessRoleId + "-" + c.PositionCode, c => c.PositionName);
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting User Details.", ex);
                throw ex;
            }
        }
        public Dictionary<string, string> GetReferenceUser(string prefix)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var uList = (from usr in oIACMDBContext.Users where usr.EmployeeType == EmployeeType.Permanent && usr.IsActive == true && usr.DisplayName.Contains(prefix) select new { usr.DisplayName, UserId = usr.Id, usr.UserExtensionObj.Supervisor }).Distinct();//.ToDictionary(c => c.DisplayName, c => c.UserId);
                    //return uList.ToDictionary(c => c.UserId + "-" + c.Supervisor, c => c.DisplayName);
                    return uList.ToDictionary(c => c.UserId.ToString(), c => c.DisplayName);
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting User Details.", ex);
                throw ex;
            }
        }
        public Dictionary<string, string> GetCompanyName(string prefix)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var uList = (from usr in oIACMDBContext.Companies
                                 where usr.IsActive && usr.CompanyName.Contains(prefix) && usr.Id != 0
                                 select new { usr.CompanyName, CompanyId = usr.Id, usr.CompanyCode }).ToDictionary(c => c.CompanyId + "-" + c.CompanyCode, c => c.CompanyName);
                    return uList;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error occured while getting User Details.", ex);
                throw ex;
            }
        }
        public Dictionary<string, string> GetDepartmentName(string prefix)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var uList = (from usr in oIACMDBContext.Departments
                                 where usr.IsActive && usr.DepartmentText.Contains(prefix) && usr.Id != 0
                                 select new { usr.DepartmentText, DepartmentId = usr.Id, usr.DepartmentCode }).ToDictionary(c => c.DepartmentId + "-" + c.DepartmentCode, c => c.DepartmentText);
                    return uList;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error occured while getting User Details.", ex);
                throw ex;
            }
        }
        public Dictionary<string, string> GetDivisionName(string prefix, int departmentId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var dList = (from usr in oIACMDBContext.Divisions
                                 where usr.IsActive && usr.DivisionText.Contains(prefix) && usr.DepartmentId == departmentId && usr.Id != 0
                                 select new { usr.DivisionText, DivisionId = usr.Id, usr.DivisionCode }).ToDictionary(c => c.DivisionId + "-" + c.DivisionCode, c => c.DivisionText);
                    return dList;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error occured while getting User Details.", ex);
                throw ex;
            }
        }
        public Dictionary<long, string> GetOrganisations(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Organisations.Where(item => item.OrganisationName.Contains(searchString) && item.Id != 0)
                        .Select(x => new { Key = x.Id, Value = x.OrganisationName })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }
        public Dictionary<string, string> GetOrganisationsWithPrefix(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Organisations.Where(item => item.OrganisationName.Contains(searchString) && item.Id != 0)
                        .Select(x => new { OrganisationId = x.Id, x.OrgUnitPrefix, x.OrganisationName })
                        .Distinct().ToDictionary(o => o.OrganisationId + "-" + o.OrgUnitPrefix, o => o.OrganisationName);
                }
            }
            catch
            {
                throw;
            }
        }
        public Dictionary<long, string> GetCostCenters(string searchString, int companyId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.CostCenters.Where(item => item.CostCenterCode.Contains(searchString) && item.Id != 0)
                   .Select(x => new { Key = x.Id, Value = x.CostCenterCode })
                   .Distinct().ToDictionary(o => o.Key, o => o.Value);

                }
            }
            catch
            {
                throw;
            }
        }
        //public string[] EmployeeTerm()
        //{
        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            var empTermList = (from empTerm in oIACMDBContext.EmployeeTerms
        //                               where empTerm.EmployeeTermDesc != "Permanent"
        //                               select new { empTerm.EmployeeTermDesc }).ToArray();
        //            // return empTermList;
        //            string[] arryEmpTermList = new string[empTermList.Count() + 1];
        //            int inc = 1;
        //            arryEmpTermList[0] = "--Select--";
        //            foreach (var item in empTermList)
        //            {
        //                arryEmpTermList[inc] = item.EmployeeTermDesc;
        //                inc++;
        //            }

        //            return arryEmpTermList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while getting User Details.", ex);
        //        throw ex;
        //    }
        //}

        public List<DeptChangedUserGridData> GetDeptChangedUserGridData(List<User> CrntUserList)
        {
            try
            {
                if (CrntUserList == null)
                    return null;

                List<DeptChangedUserGridData> deptChangedUserGridData = new List<DeptChangedUserGridData>();
                DeptChangedUserGridData _tmpDeptChangedUserGridData;

                foreach (User usr in CrntUserList)
                {
                    _tmpDeptChangedUserGridData = new DeptChangedUserGridData();

                    _tmpDeptChangedUserGridData.UserId = usr.Id;
                    //_tmpDeptChangedUserGridData.CreateAcnt = false;
                    _tmpDeptChangedUserGridData.FirstName = usr.FirstName;
                    _tmpDeptChangedUserGridData.LastName = usr.LastName;
                    _tmpDeptChangedUserGridData.JobTitle = usr.BusinessRole != null ? usr.BusinessRole.DesignationDesc : string.Empty;

                    _tmpDeptChangedUserGridData.RequestSource = AccessRequestSource.OUChange.ToString();
                    if (usr.JoinDate != null)
                    {
                        _tmpDeptChangedUserGridData.JoiningDate = usr.JoinDate;
                    }
                    //_tmpDeptChangedUserGridData.TerminationDate = usr.TerminationDate != null ? usr.TerminationDate.Value.ToShortDateString() : string.Empty;
                    _tmpDeptChangedUserGridData.DepartmentName = usr.UserExtensionObj.DepartmentName;
                    if (usr.OrganisationId.HasValue && usr.OrganisationId >= 0)
                    {
                        Organisation _organisation = new OrganisationDA().GetOrganisationById(usr.OrganisationId.Value);
                        if (_organisation != null)
                            _tmpDeptChangedUserGridData.Organisation = _organisation.OrganisationName;
                    }
                    if (usr.BusinessRoleId.HasValue && usr.BusinessRoleId >= 0)
                    {
                        _tmpDeptChangedUserGridData.JobTitle = getBusinessRoleById(usr.BusinessRoleId.Value);
                    }

                    deptChangedUserGridData.Add(_tmpDeptChangedUserGridData);
                }

                return deptChangedUserGridData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Dictionary<long, string> GetNonReportingUsersOfThisManagerID(long ManagerID, string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Users.Where(item => item.DisplayName.Contains(searchString) && item.ManagerId != ManagerID && item.TerminationDate >= DateTime.Now && item.IsActive == true)
                        .Select(x => new { Key = x.Id, Value = x.DisplayName + " [" + x.UserName + "]" })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetUsersByGroup(string searchString, string groupName)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (!string.IsNullOrEmpty(groupName))
                    {
                        Group group = _context.Groups.Include("Users").Where(grp => grp.GroupName == groupName).FirstOrDefault();
                        IList<User> usrList = group.Users.ToList();
                        return usrList.Where(item => item.DisplayName.ToUpper().Contains(searchString.ToUpper()))
                        .Select(x => new { Key = x.Id, Value = x.DisplayName })
                       .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                    return _context.Users.Where(item => item.DisplayName.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.DisplayName })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<User> GetUserListByGroupId(string groupName)
        {
            try
            {
                IList<User> userList = new List<User>();
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (!string.IsNullOrEmpty(groupName))
                    {
                        List<Group> group = _context.Groups.Where(grp => grp.GroupName == groupName).ToList();

                        foreach (Group UserListByGroup in group)
                        {
                            userList = UserListByGroup.Users;
                        }
                    }
                    return userList;
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetADUsersByDIUserId(long userid, int endpointsystemid, AccountType accountType)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (endpointsystemid == 0)
                    {
                        if (accountType == AccountType.User || accountType == AccountType.Secondary)
                        {
                            return _context.ADSUserInfos.Where(item => item.IsLocked == false && item.AccountType == accountType && item.DIUserId == userid)
                                                    .Select(x => new { Key = x.Id, Value = x.sAMAccountName })
                                                    .Distinct().ToDictionary(o => o.Key, o => o.Value);
                        }
                        if (accountType == AccountType.Service)
                        {
                            string ouOfRequestee = _context.Users.Where(lq => lq.Id == userid).Select(lq => lq.UserExtensionObj.SectionCode).FirstOrDefault();
                            List<long> listofUser = _context.Users.Where(lq => lq.UserExtensionObj.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
                            var listofitems = _context.ADSUserInfos.Where(item => item.DIUserId != null && item.IsLocked == false && (item.AccountType == accountType));
                            return listofitems.Where(lq => listofUser.Contains((int)(lq.DIUserId)))
                                .Select(x => new { Key = x.Id, Value = x.sAMAccountName })
                                .Distinct().ToDictionary(o => o.Key, o => o.Value);
                        }
                        else
                        {
                            string ouOfRequestee = _context.Users.Where(lq => lq.Id == userid).Select(lq => lq.UserExtensionObj.SectionCode).FirstOrDefault();
                            List<long> listofUser = _context.Users.Where(lq => lq.UserExtensionObj.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
                            var listofitems = _context.ADSUserInfos.Where(item => item.DIUserId != null && item.IsLocked == false && (item.AccountType == accountType));
                            return listofitems.Where(lq => listofUser.Contains((int)(lq.DIUserId)))
                                .Select(x => new { Key = x.Id, Value = x.sAMAccountName })
                                .Distinct().ToDictionary(o => o.Key, o => o.Value);
                        }
                    }
                    if (accountType == AccountType.User || accountType == AccountType.Secondary)
                    {
                        return _context.ADSUserInfos.Where(item => item.IsLocked == false && item.EndPointSystemId == endpointsystemid && item.AccountType == accountType && item.DIUserId == userid)
                            .Select(x => new { Key = x.Id, Value = x.sAMAccountName })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                    if (accountType == AccountType.Service)
                    {
                        string ouOfRequestee = _context.Users.Where(lq => lq.Id == userid).Select(lq => lq.UserExtensionObj.SectionCode).FirstOrDefault();
                        List<long> listofUser = _context.Users.Where(lq => lq.UserExtensionObj.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
                        var listofitems = _context.ADSUserInfos.Where(item => item.DIUserId != null && item.IsLocked == false && item.EndPointSystemId == endpointsystemid && (item.AccountType == accountType));
                        return listofitems.Where(lq => listofUser.Contains((int)(lq.DIUserId)))
                            .Select(x => new { Key = x.Id, Value = x.sAMAccountName })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                    else
                    {
                        string ouOfRequestee = _context.Users.Where(lq => lq.Id == userid).Select(lq => lq.UserExtensionObj.SectionCode).FirstOrDefault();
                        List<long> listofUser = _context.Users.Where(lq => lq.UserExtensionObj.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
                        var listofitems = _context.ADSUserInfos.Where(item => item.DIUserId != null && item.IsLocked == false && item.EndPointSystemId == endpointsystemid && (item.AccountType == accountType));
                        return listofitems.Where(lq => listofUser.Contains((int)(lq.DIUserId)))
                            .Select(x => new { Key = x.Id, Value = x.sAMAccountName })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetLNSOwnerByDIUserID(int UserID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LotusUserInfos.Where(item => item.DIUserId == UserID).Select(item => item.Owner).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }
        public Dictionary<long, string> GetLNSUsersByDIUserId(long userid, int endpointsystemid, AccountType accountType)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (accountType == AccountType.User || accountType == AccountType.Secondary)
                    {
                        return _context.LotusUserInfos.Where(item => item.IsLocked == false && item.EndPointSystemId == endpointsystemid && (item.AccountType == accountType && item.DIUserId == userid))
                                                .Select(x => new { Key = x.Id, Value = x.InternetAddress })
                                                .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                    if (accountType == AccountType.Service)
                    {
                        string ouOfRequestee = _context.Users.Where(lq => lq.Id == userid).Select(lq => lq.UserExtensionObj.SectionCode).FirstOrDefault();
                        List<long> listofUser = _context.Users.Where(lq => lq.UserExtensionObj.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
                        var listofitems = _context.LotusUserInfos.Where(item => item.IsLocked == false && item.DIUserId != null && item.EndPointSystemId == endpointsystemid && (item.AccountType == accountType));
                        return listofitems.Where(lq => listofUser.Contains((int)(lq.DIUserId)))
                            .Select(x => new { Key = x.Id, Value = x.InternetAddress })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                    else
                    {
                        string ouOfRequestee = _context.Users.Where(lq => lq.Id == userid).Select(lq => lq.UserExtensionObj.SectionCode).FirstOrDefault();
                        List<long> listofUser = _context.Users.Where(lq => lq.UserExtensionObj.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
                        var listofitems = _context.LotusUserInfos.Where(item => item.DIUserId != null && item.IsLocked == false && item.EndPointSystemId == endpointsystemid && (item.AccountType == accountType));
                        return listofitems.Where(lq => listofUser.Contains((int)(lq.DIUserId)))
                            .Select(x => new { Key = x.Id, Value = x.InternetAddress })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<long, string> GetSAPUsersByDIUserId(long userid, int endpointsystemid, AccountType accountType)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (accountType == AccountType.User || accountType == AccountType.Secondary)
                    {
                        return _context.SAPUserinfos.Where(item => item.IsLocked == false && item.EndPointSystemId == endpointsystemid && (item.AccountType == accountType && item.DIUserId == userid))
                                                .Select(x => new { Key = x.Id, Value = x.UserLogonName })
                                                .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                    if (accountType == AccountType.Service)
                    {
                        string ouOfRequestee = _context.Users.Where(lq => lq.Id == userid).Select(lq => lq.UserExtensionObj.SectionCode).FirstOrDefault();
                        List<long> listofUser = _context.Users.Where(lq => lq.UserExtensionObj.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
                        var listofitems = _context.SAPUserinfos.Where(item => item.DIUserId != null && item.IsLocked == false && item.EndPointSystemId == endpointsystemid && (item.AccountType == accountType));
                        return listofitems.Where(lq => listofUser.Contains((int)(lq.DIUserId)))
                            .Select(x => new { Key = x.Id, Value = x.UserLogonName })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                    else
                    {
                        string ouOfRequestee = _context.Users.Where(lq => lq.Id == userid).Select(lq => lq.UserExtensionObj.SectionCode).FirstOrDefault();
                        List<long> listofUser = _context.Users.Where(lq => lq.UserExtensionObj.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
                        var listofitems = _context.SAPUserinfos.Where(item => item.DIUserId != null && item.IsLocked == false && item.EndPointSystemId == endpointsystemid && (item.AccountType == accountType));
                        return listofitems.Where(lq => listofUser.Contains((int)(lq.DIUserId)))
                            .Select(x => new { Key = x.Id, Value = x.UserLogonName })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<long, string> GetSAPEPUsersByDIUserId(int userid, int endpointsystemid)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPEPUserInfos.Where(item => item.DIUserId == userid && item.EndPointSystemId == endpointsystemid)
                                         .Select(x => new { Key = x.Id, Value = x.AccountName })
                                         .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ADSUserInfo GetADUserInfoByDIUserId(int userid, int endpointsystemid)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSUserInfos.FirstOrDefault(item => item.DIUserId == userid && item.EndPointSystemId == endpointsystemid && item.AccountType == AccountType.User);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Dictionary<long, string> GetLinuxUsersByDIUserId(long UserId, int EndPointId, AccountType accountType)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (accountType == AccountType.User || accountType == AccountType.Secondary)
                    {
                        return _context.LinuxUserInfos.Where(item => item.IsLocked == false && item.EndPointSystemId == EndPointId && (item.AccountType == accountType && item.DIUserId == UserId))
                                                .Select(x => new { Key = x.Id, Value = x.UserName })
                                                .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                    if (accountType == AccountType.Service)
                    {
                        string ouOfRequestee = _context.Users.Where(lq => lq.Id == UserId).Select(lq => lq.UserExtensionObj.SectionCode).FirstOrDefault();
                        List<long> listofUser = _context.Users.Where(lq => lq.UserExtensionObj.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
                        var listofitems = _context.LinuxUserInfos.Where(item => item.DIUserId != null && item.IsLocked == false && item.EndPointSystemId == EndPointId && (item.AccountType == accountType));
                        return listofitems.Where(lq => listofUser.Contains((int)(lq.DIUserId)))
                            .Select(x => new { Key = x.Id, Value = x.UserName })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                    else
                    {
                        string ouOfRequestee = _context.Users.Where(lq => lq.Id == UserId).Select(lq => lq.UserExtensionObj.SectionCode).FirstOrDefault();
                        List<long> listofUser = _context.Users.Where(lq => lq.UserExtensionObj.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
                        var listofitems = _context.LinuxUserInfos.Where(item => item.DIUserId != null && item.IsLocked == false && item.EndPointSystemId == EndPointId && (item.AccountType == accountType));
                        return listofitems.Where(lq => listofUser.Contains((int)(lq.DIUserId)))
                            .Select(x => new { Key = x.Id, Value = x.UserName })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<long, string> GetOEBSUsersByDIUserId(long UserId, int EndPointId, AccountType accountType)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (accountType == AccountType.User || accountType == AccountType.Secondary)
                    {
                        return _context.OracleEBSUserInfo.Where(item => item.IsLocked == false && item.EndPointSystemId == EndPointId && (item.AccountType == accountType && item.DIUserId == UserId))
                                                .Select(x => new { Key = x.Id, Value = x.USER_NAME })
                                                .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                    if (accountType == AccountType.Service)
                    {
                        string ouOfRequestee = _context.Users.Where(lq => lq.Id == UserId).Select(lq => lq.UserExtensionObj.SectionCode).FirstOrDefault();
                        List<long> listofUser = _context.Users.Where(lq => lq.UserExtensionObj.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
                        var listofitems = _context.OracleEBSUserInfo.Where(item => item.DIUserId != null && item.IsLocked == false && item.EndPointSystemId == EndPointId && (item.AccountType == accountType));
                        return listofitems.Where(lq => listofUser.Contains((int)(lq.DIUserId)))
                            .Select(x => new { Key = x.Id, Value = x.USER_NAME })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                    else
                    {
                        string ouOfRequestee = _context.Users.Where(lq => lq.Id == UserId).Select(lq => lq.UserExtensionObj.SectionCode).FirstOrDefault();
                        List<long> listofUser = _context.Users.Where(lq => lq.UserExtensionObj.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
                        var listofitems = _context.OracleEBSUserInfo.Where(item => item.DIUserId != null && item.IsLocked == false && item.EndPointSystemId == EndPointId && (item.AccountType == accountType));
                        return listofitems.Where(lq => listofUser.Contains((int)(lq.DIUserId)))
                            .Select(x => new { Key = x.Id, Value = x.USER_NAME })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        public User GetCIO(string cioFlag)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Users.FirstOrDefault(item => item.UserExtensionObj.PositionName.Equals(cioFlag, StringComparison.CurrentCultureIgnoreCase));
                }
            }
            catch
            {
                throw;
            }
        }

        public List<UserInternetAndEmailAccessReportData> GetUsersInterentAndExternalEmailAccess()
        {
            using (IACMDBContext _context = new IACMDBContext())
            {


                var internetUsers = (from adsUser in _context.ADSUserInfos
                                     join user in _context.Users on adsUser.DIUserId equals user.Id
                                     where adsUser.ADSGroups.FirstOrDefault(grp => grp.name == "InternetUsers") != null
                                     select user);

                var externalEmailUsers = (from lnsUser in _context.LotusUserInfos
                                          join user in _context.Users on lnsUser.DIUserId equals user.Id
                                          where lnsUser.MailExternalAccess
                                          select user);


                return (from user in _context.Users
                        select new UserInternetAndEmailAccessReportData
                        {
                            DepartmentName = user.UserExtensionObj.DepartmentName,
                            InternetAccess = internetUsers.FirstOrDefault(usr => usr.Id == user.Id) == null ? "No" : "Yes",
                            EmailAccess = externalEmailUsers.FirstOrDefault(usr => usr.Id == user.Id) == null ? "No" : "Yes",
                            IsExceutiveEmployee = user.UserExtensionObj.IsExecutiveEmployee ? "Yes" : "No",
                            DisplayName = user.DisplayName,
                            UserName = user.UserName,
                            UserId = user.Id
                        }
                        ).ToList();

            }
        }


        public bool IsAccountLocked(int userID, int endPointSystemID, AccountType adhocAccountType, EndpointType endpointSystemType)
        {
            var result = false;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (endpointSystemType == EndpointType.SAP)
                    {
                        var data = _context.SAPUserinfos.Where
                            (item =>
                                adhocAccountType == AccountType.User ? (item.EndPointSystemId == endPointSystemID && (item.AccountType == AccountType.User || item.AccountType == AccountType.Secondary) && item.DIUserId == userID) :
                                adhocAccountType == AccountType.Shared ? (item.AccountType == AccountType.Shared && item.EndPointSystemId == endPointSystemID) :
                                (item.EndPointSystemId == endPointSystemID && item.AccountType == AccountType.Service)
                            )
                            .Select(x => x).ToList();
                        if (data.Count > 0)
                        {
                            var userAccountIsAvaliable = data.Where(lq => lq.IsLocked == false).Select(lq => lq);
                            result = userAccountIsAvaliable.Count() > 0 ? false : true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else if (endpointSystemType == EndpointType.ActiveDirectory)
                    {
                        var data = _context.ADSUserInfos.Where
                            (item =>
                                adhocAccountType == AccountType.User ? (item.EndPointSystemId == endPointSystemID && (item.AccountType == AccountType.User || item.AccountType == AccountType.Secondary) && item.DIUserId == userID) :
                                adhocAccountType == AccountType.Shared ? (item.AccountType == AccountType.Shared && item.EndPointSystemId == endPointSystemID) :
                                (item.EndPointSystemId == endPointSystemID && item.AccountType == AccountType.Service)
                            )
                            .Select(x => x).ToList();
                        if (data.Count > 0)
                        {
                            var userAccountIsAvaliable = data.Where(lq => lq.IsLocked == false).Select(lq => lq);
                            result = userAccountIsAvaliable.Count() > 0 ? false : true;
                        }
                        else
                        {
                            result = false;
                        }
                    }

                    else if (endpointSystemType == EndpointType.LotusNotes)
                    {
                        var data = _context.LotusUserInfos.Where
                            (item =>
                               adhocAccountType == AccountType.User ? (item.EndPointSystemId == endPointSystemID && (item.AccountType == AccountType.User || item.AccountType == AccountType.Secondary) && item.DIUserId == userID) :
                                adhocAccountType == AccountType.Shared ? (item.AccountType == AccountType.Shared && item.EndPointSystemId == endPointSystemID) :
                                (item.EndPointSystemId == endPointSystemID && item.AccountType == AccountType.Service)
                            )
                            .Select(x => x).ToList();
                        if (data.Count > 0)
                        {
                            var userAccountIsAvaliable = data.Where(lq => lq.IsLocked == false).Select(lq => lq);
                            result = userAccountIsAvaliable.Count() > 0 ? false : true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        public static bool IsUserAccount(long userID, int endPointSystemID, EndpointType EndpointSystemType)
        {
            var result = false;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (EndpointSystemType == EndpointType.InhouseSystems)
                    {
                        Dictionary<long, string> data = _context.InhouseUserInfos.Where(item => item.EndPointSystemId == endPointSystemID && item.DIUserId == userID && item.AccountType == AccountType.User && item.IsLocked == false)
                                                    .Select(x => new { Key = x.Id, Value = x.UserName })
                                                    .Distinct().ToDictionary(o => o.Key, o => o.Value);
                        result = data.Count > 0 ? true : false;
                    }
                    else if (EndpointSystemType == EndpointType.SAP)
                    {
                        Dictionary<long, string> data = _context.SAPUserinfos.Where(item => item.EndPointSystemId == endPointSystemID && item.DIUserId == userID && item.AccountType == AccountType.User && item.IsLocked == false)
                                                    .Select(x => new { Key = x.Id, Value = x.UserLogonName })
                                                    .Distinct().ToDictionary(o => o.Key, o => o.Value);
                        result = data.Count > 0 ? true : false;
                    }
                    else if (EndpointSystemType == EndpointType.SAPEnterprisePortal)
                    {
                        Dictionary<long, string> data = _context.SAPEPUserInfos.Where(item => item.EndPointSystemId == endPointSystemID && item.DIUserId == userID && item.AccountType == AccountType.User && item.IsLocked == false)
                                                    .Select(x => new { Key = x.Id, Value = x.UserName })
                                                    .Distinct().ToDictionary(o => o.Key, o => o.Value);
                        result = data.Count > 0 ? true : false;
                    }
                    else if (EndpointSystemType == EndpointType.ActiveDirectory)
                    {
                        Dictionary<long, string> data = _context.ADSUserInfos.Where(item => item.EndPointSystemId == endPointSystemID && item.DIUserId == userID && item.AccountType == AccountType.User && item.IsLocked == false)
                                                    .Select(x => new { Key = x.Id, Value = x.sAMAccountName })
                                                    .Distinct().ToDictionary(o => o.Key, o => o.Value);
                        result = data.Count > 0 ? true : false;
                    }
                    else if (EndpointSystemType == EndpointType.LotusNotes)
                    {
                        Dictionary<long, string> data = _context.LotusUserInfos.Where(item => item.EndPointSystemId == endPointSystemID && item.DIUserId == userID && item.AccountType == AccountType.User && item.IsLocked == false)
                                                    .Select(x => new { Key = x.Id, Value = x.InternetAddress })
                                                    .Distinct().ToDictionary(o => o.Key, o => o.Value);
                        result = data.Count > 0 ? true : false;
                    }
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        public void InsetUserGroup(int UserID, int GroupID)
        {
            try
            {
                QueryExecutor m_executor = null;
                IACMDbOperator m_iacmOperator = null;

                string query = "INSERT INTO UserGroup (User_UserId,Group_GroupId) VALUES(" + UserID + "," + GroupID + ")";
                m_iacmOperator = new IACMDbOperator(query, CommandType.Text, null);

                m_executor = new QueryExecutor(m_iacmOperator);

                m_executor.ExecuteQuery();
            }
            catch
            {
                throw;
            }
        }

        public void InsetUserRole(long UserID, int RoleId)
        {
            try
            {
                QueryExecutor m_executor = null;
                IACMDbOperator m_iacmOperator = null;

                string query = "INSERT INTO userRole (User_UserId,Role_RoleId) VALUES(" + UserID + "," + RoleId + ")";
                m_iacmOperator = new IACMDbOperator(query, CommandType.Text, null);

                m_executor = new QueryExecutor(m_iacmOperator);

                m_executor.ExecuteQuery();
            }
            catch
            {
                throw;
            }
        }
        public Dictionary<long, string> GetUsersBySearchString(string searchString)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.Users.Where(usr => !string.IsNullOrEmpty(usr.DisplayName) && usr.DisplayName.Contains(searchString)).Select(x => new { key = x.Id, value = x.DisplayName + " [" + x.UserName + "]" }).ToDictionary(o => o.key, o => o.value);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<long, string> GetActiveAndNonTerminatedUsers(string searchString)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.Users
                        .Where(usr => usr.IsActive == true &&
                            (usr.TerminationDate == null || (usr.TerminationDate != null && usr.TerminationDate > DateTime.Now))
                            && usr.DisplayName.Contains(searchString))
                        .Select(x => new { key = x.Id, value = x.DisplayName + " [" + x.UserName + "]" })
                        .ToDictionary(o => o.key, o => o.value);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<string, string> GetAllDepartmentsList(string tenantID)
        {
            Dictionary<string, string> departmentsList = new Dictionary<string, string>();
            IACMDbOperator m_dbOperator = null;
            string query = "DI_spGetAllOrganisationList";
            string deptId;
            string deptName;

            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                SqlParameter sqlParam = null;

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@TenantID";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Value = tenantID;
                parameters.Add(sqlParam);

                m_dbOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);
                executor = new QueryExecutor(m_dbOperator);
                IDataReader reader = executor.ExecuteQueryDR();

                while (reader.Read())
                {
                    deptId = CheckNull.AgainstString(reader["OrganisationId"]);
                    deptName = CheckNull.AgainstString(reader["OrganisationName"]);

                    if (!string.IsNullOrEmpty(deptId) && !departmentsList.ContainsKey(deptId))
                    {
                        departmentsList.Add(deptId, deptName);
                    }
                }

                return departmentsList;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Error, "Error occur while getting all Departments List.", ex);
                throw;
            }
            finally
            {
                if (m_dbOperator != null)
                    m_dbOperator.Close();
            }
        }
        public DataSet GetAllUsersByGroup(string groupName, int endpointId)
        {
            try
            {
                string query = "DI_spGetGroupUsers";



                List<SqlParameter> parameters = new List<SqlParameter>();

                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@GroupName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Value = groupName;
                parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@EndpointId";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = endpointId;
                parameters.Add(sqlParam);


                iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);



                executor = new QueryExecutor(iacmOperator);

                DataSet ds = executor.ExecuteQueryDS();

                if (null == ds)
                    return null;

                if (0 == ds.Tables.Count)
                    return null;

                return ds;

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                executor.IACMDbOperator.Close();
            }

        }

        public IList<User> GetAllDIUsers()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Users.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetReportingUsersOfThisManagerIDandAdmin(long ManagerID, string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    bool IsAdmin = false;
                    bool IsIACMAdmin = false;
                    IsAdminUser(out IsAdmin, out IsIACMAdmin, ManagerID);
                    if (IsAdmin && IsIACMAdmin)
                        return _context.Users.Where(item => item.DisplayName.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.DisplayName + " [" + x.UserName + "]" })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    return _context.Users.Where(item => item.DisplayName.Contains(searchString) && (item.ManagerId == ManagerID || item.Id == ManagerID))
                        .Select(x => new { Key = x.Id, Value = x.DisplayName + " [" + x.UserName + "]" })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public static string GetOrganisationUnitPrefixByOrgId(long? OrgID)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                if (OrgID.HasValue)
                {
                    var Organization = _context.Organisations.Find(OrgID);
                    return (Organization != null) ? Organization.OrgUnitPrefix : "";
                }
                return "";
            }

        }
        public string GetEndPointUserDetailByEndpointUserID(long EndpointUserID, string EndpointSystemName)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                if (EndpointSystemName == "AD")
                {
                    return (from source in _context.ADSUserInfos where source.Id == EndpointUserID select source.sAMAccountName).FirstOrDefault();
                }
                if (EndpointSystemName == "SAP")
                {
                    return (from source in _context.SAPUserinfos where source.Id == EndpointUserID select source.UserLogonName).FirstOrDefault();
                }
                if (EndpointSystemName == "SAPEP")
                {
                    return (from source in _context.SAPEPUserInfos where source.Id == EndpointUserID select source.UserName).FirstOrDefault();
                }
                if (EndpointSystemName == "LNS")
                {
                    return (from source in _context.LotusUserInfos where source.Id == EndpointUserID select source.InternetAddress).FirstOrDefault();
                }
                if (EndpointSystemName == "INH")
                {
                    return (from source in _context.InhouseUserInfos where source.Id == EndpointUserID select source.UserName).FirstOrDefault();
                }
                if (EndpointSystemName == "DIUser")
                {
                    return (from source in _context.Users where source.Id == EndpointUserID select source.DisplayName).FirstOrDefault();
                }

                return null;
            }
        }
        public Dictionary<string, string> GetDisticntSectionFromUsers(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Organisations.Where(item => item.OrganisationName.Contains(searchString))
                        .Select(x => new { Key = x.OrgUnitPrefix, Value = x.OrgUnitPrefix + " - " + x.OrganisationName })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CheckUserGroup(long UserID, int GroupID)
        {
            try
            {
                QueryExecutor m_executor = null;
                IACMDbOperator m_iacmOperator = null;

                string query = "select * from UserGroup where User_UserId=" + UserID + " and Group_GroupId=" + GroupID;
                m_iacmOperator = new IACMDbOperator(query, CommandType.Text, null);

                m_executor = new QueryExecutor(m_iacmOperator);

                DataSet ds = m_executor.ExecuteQueryDS();
                if (ds.Tables[0].Rows.Count > 0)
                    return true;
                return false;
            }
            catch
            {
                throw;
            }
        }

        public bool GetCoordinator(int userid, int roleid, int groupid)
        {
            try
            {
                //using (IACMDBContext _context = new IACMDBContext())
                //{

                //    bool IsCoordinatorRoleAssigned = _context.RoleAssignments.Any(usr => usr.UserId == userid && usr.RoleId == roleid);
                //    User user = _context.Users.FirstOrDefault(usr => usr.UserId == userid);

                //    bool IsCoordinatorGroupAssigned = user.Groups.Any(group => group.GroupId == groupid);
                //    return (IsCoordinatorRoleAssigned || IsCoordinatorGroupAssigned);
                //}

                return true;
            }
            catch
            {
                throw;
            }
        }
        public IList<User> GetAllNonEmployees(int userId, bool IsCoOrdinator, int DepartmentId, IDictionary<string, List<string>> Constraints = null)
        {

            try
            {
                IList<User> userList = new List<User>();
                bool IsAdmin = false;
                bool IsIACMAdmin = false;
                IsAdminUser(out IsAdmin, out IsIACMAdmin, userId);
                if (IsAdmin)
                {
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        userList = _context.Users.Where(item => item.FullTimeEmployee != true && item.UserName != "IACMAdmin").ToList();
                    }
                }
                else if (IsCoOrdinator)
                {
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        userList = _context.Users.Where(item => item.FullTimeEmployee != true && item.UserExtensionObj.DepartmentId == DepartmentId && item.UserName != "IACMAdmin").ToList();
                    }
                }
                else
                {
                    var users = GetAllSubordinateByUserID(userId, string.Empty);

                    userList = users.Where(usr => usr.FullTimeEmployee != true && usr.UserName != "IACMAdmin").ToList();


                }
                if (Constraints != null && Constraints.Count > 0)
                {
                    if (Constraints.ContainsKey("Companies"))
                    {
                        List<string> Company = Constraints["Companies"];

                        userList = userList.Where(c => Company.Contains(c.UserExtensionObj.CompanyId.ToString())).Select(c => c).ToList();

                    }
                    if (Constraints.ContainsKey("CostCenter"))
                    {
                        List<string> CostCenter = Constraints["CostCenter"];
                        userList = userList.Where(c => CostCenter.Contains(c.UserExtensionObj.CostCenter)).Select(c => c).ToList();
                    }
                    if (Constraints.ContainsKey("Department"))
                    {
                        List<string> Department = Constraints["Department"];
                        userList = userList.Where(c => Department.Contains(c.UserExtensionObj.DepartmentId.ToString())).Select(c => c).ToList();
                    }
                    if (Constraints.ContainsKey("Division"))
                    {
                        List<string> Division = Constraints["Division"];
                        userList = userList.Where(c => Division.Contains(c.UserExtensionObj.DivisionId.ToString())).Select(c => c).ToList();
                    }

                    if (Constraints.ContainsKey("Section"))
                    {
                        List<string> Section = Constraints["Section"];
                        userList = userList.Where(c => Section.Contains(c.UserExtensionObj.SectionName.ToString())).Select(c => c).ToList();
                    }
                }
                return userList.OrderBy(item => item.DisplayName).ToList();
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All Non-Employees.", ex);
                throw ex;
            }

        }

        public IList<User> GetAllEmployees(int userId, bool IsCoOrdinator, IDictionary<string, List<string>> Constraints = null)
        {

            try
            {
                IList<User> userList = new List<User>();
                bool IsAdmin = false;
                bool IsIACMAdmin = false;
                IsAdminUser(out IsAdmin, out IsIACMAdmin, userId);
                if (IsCoOrdinator || IsAdmin)
                {
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        userList = _context.Users.Where(item => item.FullTimeEmployee.Value && item.UserName != "IACMAdmin").ToList();
                    }
                }
                else
                {
                    var users = GetAllSubordinateByUserID(userId, string.Empty);

                    userList = users.Where(usr => usr.FullTimeEmployee == true && usr.UserName != "IACMAdmin").ToList();
                }

                if (Constraints != null && Constraints.Count > 0)
                {
                    if (Constraints.ContainsKey("Companies"))
                    {
                        List<string> Company = Constraints["Companies"];

                        userList = userList.Where(c => Company.Contains(c.UserExtensionObj.CompanyId.ToString())).Select(c => c).ToList();
                    }

                    if (Constraints.ContainsKey("CostCenter"))
                    {
                        List<string> CostCenter = Constraints["CostCenter"];
                        userList = userList.Where(c => CostCenter.Contains(c.UserExtensionObj.CostCenter)).Select(c => c).ToList();
                    }
                    if (Constraints.ContainsKey("Department"))
                    {
                        List<string> Department = Constraints["Department"];
                        userList = userList.Where(c => Department.Contains(c.UserExtensionObj.DepartmentId.ToString())).Select(c => c).ToList();
                    }
                    if (Constraints.ContainsKey("Division"))
                    {
                        List<string> Division = Constraints["Division"];
                        userList = userList.Where(c => Division.Contains(c.UserExtensionObj.DivisionId.ToString())).Select(c => c).ToList();
                    }

                    if (Constraints.ContainsKey("Section"))
                    {
                        List<string> Section = Constraints["Section"];
                        userList = userList.Where(c => Section.Contains(c.UserExtensionObj.SectionName.ToString())).Select(c => c).ToList();
                    }
                }
                return userList.OrderBy(item => item.DisplayName).ToList();
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while getting All Non-Employees.", ex);
                throw ex;
            }

        }

        public static IList<User> GetAllSubordinateByUserID(long UserID, string searchString)
        {
            IList<User> User = new List<User>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var OrgUnitPrefixUser = (from orgunitprefixuser in _context.Organisations where orgunitprefixuser.Head == UserID select orgunitprefixuser.OrgUnitPrefix).ToList();
                    foreach (var org in OrgUnitPrefixUser)
                    {
                        var orgList = GetChildOrgs(org);
                        foreach (var orgunit in orgList)
                        {
                            IList<User> UserofOrgUnit = (from user in _context.Users where user.DisplayName.Contains(searchString) && user.UserExtensionObj.SectionCode == orgunit select user).ToList();
                            foreach (User user in UserofOrgUnit)
                            {
                                User.Add(user);
                            }
                        }
                    }
                }
                return User;
            }
            catch
            {
                throw;
            }
        }

        private static List<string> GetChildOrgs(string parentOrg)
        {
            List<string> orgList = new List<string>();

            OrganisationDA orgDA = new OrganisationDA();

            orgList.Add(parentOrg);
            var childs = orgDA.GetOrganisationsByParent(parentOrg);

            if (childs != null && childs.Count != 0)
            {
                foreach (Organisation org in childs)
                {
                    orgList.Add(org.OrgUnitPrefix);
                }
            }

            return orgList;
        }

        public Dictionary<long, string> GetReportingofSubordinate(long UserID, string searchString, List<long> CompanyId, List<long> DepartmentId, List<long> DivisionId, List<long> OrganisationId, List<string> SectionCode)
        {
            Dictionary<long, string> data = new Dictionary<long, string>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    IList<User> User = new List<User>();
                    bool IsAdmin = false;
                    bool IsIACMAdmin = false;
                    IsAdminUser(out IsAdmin, out IsIACMAdmin, UserID);
                    if (IsAdmin || IsIACMAdmin)
                    {

                        data = _context.Users.Where(item => item.DisplayName.Contains(searchString)
                            && (CompanyId.Count == 0 || (item.UserExtensionObj.CompanyId.HasValue && CompanyId.Contains((int)item.UserExtensionObj.CompanyId.Value)))
                            && (DepartmentId.Count == 0 || (item.UserExtensionObj.DepartmentId.HasValue && DepartmentId.Contains(item.UserExtensionObj.DepartmentId.Value)))
                            && (DivisionId.Count == 0 || (item.UserExtensionObj.DivisionId.HasValue && DivisionId.Contains(item.UserExtensionObj.DivisionId.Value)))
                             && (SectionCode.Count == 0 || (!string.IsNullOrEmpty(item.UserExtensionObj.SectionCode) && SectionCode.Contains(item.UserExtensionObj.SectionCode)))
                           && (OrganisationId.Count == 0 || (item.OrganisationId.HasValue && OrganisationId.Contains(item.OrganisationId.Value)))
                            )
                        .Select(x => new { Key = x.Id, Value = x.DisplayName + " [" + x.UserName + "]" })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                    }
                    else
                    {
                        User = GetAllSubordinateByUserID(UserID, searchString);
                        if (User.Count > 0)
                        {
                            data = User.Select(x => new { Key = x.Id, Value = x.DisplayName + " [" + x.UserName + "]" })
                           .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                        }
                    }
                }
                return data;
            }
            catch
            {
                throw;
            }
        }

        public bool CheckIfUserPermanent(int UserID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Users.Any(user => user.Id == UserID && user.FullTimeEmployee == true);
                }
            }
            catch
            {
                throw;
            }
        }

        public List<User> GetUsersToUpdateEmailIdInHR()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Users.Where(user => user.UserExtensionObj.NeedEmailUpdateToHR).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<User> GetUsersToUpdateFLINSLoginIdInHR()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Users.Where(user => user.UserExtensionObj.NeedFLINSLoginIdUpdateToHR).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<string> GetDesignations()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Designations.Select(item => item.DesignationDesc).Distinct().ToList();

                    //return _context.Designations
                    //    .Select(x => new { Key = x.BusinessRoleId, Value = x.DesignationDesc })
                    //    .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CheckUserNameExist(string username)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Users.Any(item => item.UserName.Equals(username, StringComparison.CurrentCultureIgnoreCase));
                }
            }
            catch
            {
                throw;
            }
        }

        public List<TaskDelegationReportData> GetTaskDelegationsForReport()
        {
            try
            {

                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from delegation in _context.TaskDelegations
                            join actionUser in _context.Users
                            on delegation.ActionUser equals actionUser.Id
                            select new
                          {
                              ActionUserName = actionUser.UserName,
                              DelegatedToUserName = delegation.DelegatedToUser.UserName,
                              Active = delegation.IsActive,
                              DelegateAttestationTasks = delegation.DelegateAttestationTask,
                              DelegateProvisioningTasks = delegation.DelegateProvisioningTask,
                              DelegationId = delegation.Id,
                              delegation.StartDate,
                              delegation.EndDate,
                              DelegationDate = delegation.CreatedDate,
                              DelegationCreatedBy = delegation.CreatedBy,
                              delegation.DelegationComments
                          }).ToList()
                           .Select(x => new TaskDelegationReportData
                            {
                                ActionUserName = x.ActionUserName,
                                DelegatedToUserName = x.DelegatedToUserName,
                                Active = x.Active,
                                DelegateAttestationTasks = x.DelegateAttestationTasks ? "Yes" : "No",
                                DelegateProvisioningTasks = x.DelegateProvisioningTasks ? "Yes" : "No",
                                DelegationId = x.DelegationId,
                                StartDate = x.StartDate == null ? DateTime.MinValue : x.StartDate.Value,
                                EndDate = x.EndDate == null ? DateTime.MinValue : x.EndDate.Value,
                                DelegationDate = x.DelegationDate,
                                DelegationCreatedBy = x.DelegationCreatedBy,
                                DelegationComments = string.IsNullOrEmpty(x.DelegationComments) ? string.Empty : x.DelegationComments.Replace("\n", "<br>")

                            }).ToList();
                }
            }
            catch
            {
                throw;
            }
        }


        public List<TaskDelegationReportData> GetCurrentTaskDelegationsForReport()
        {
            try
            {

                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from task in _context.Tasks
                            join accessRequest in _context.AccessRequests
                            on task.AccessRequestId equals accessRequest.Id
                            join actionUser in _context.Users
                            on (long)task.ActionUserId equals actionUser.Id
                            join delegateUser in _context.Users
                            on (long)task.DelegatedFrom equals delegateUser.Id
                            where task.IsDelegated
                            select new
                            {
                                AccessRequestId = accessRequest.Id,
                                ActionUserName = actionUser.UserName,
                                DelegatedFromUser = delegateUser.UserName,
                                Active = task.IsActive,
                                DelegationDate = task.DelegatedDate,
                                DelegationCreatedBy = task.DelegatedBy,
                                task.DelegationComments
                            }).ToList()
                           .Select(x => new TaskDelegationReportData
                           {
                               AccessRequestId = x.AccessRequestId,
                               ActionUserName = x.ActionUserName,
                               DelegatedFromUserName = x.DelegatedFromUser,
                               Active = x.Active,
                               DelegationDate = x.DelegationDate.Value,
                               DelegationCreatedBy = x.DelegationCreatedBy,
                               DelegationComments = string.IsNullOrEmpty(x.DelegationComments) ? string.Empty : x.DelegationComments.Replace("\n", "<br>")

                           }).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<Company> GetAllDICompanies()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Companies.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<Division> GetAllDIDivisions()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Divisions.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<Department> GetAllDIDepartment()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Departments.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<MyRequestsGridData> GetAdhocRequestsByProgressStatus(AccessRequestProgress progressStatus)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    IList<MyRequestsGridData> requestList = new List<MyRequestsGridData>();
                    DateTime CurrentDate = DateTime.Now;
                    requestList = (from rqst in _context.AdHocAccessRequests
                                   join acssrqst in _context.AccessRequests.Include("User").Include("AdHocAccessRequest")
                                   on rqst.Id equals acssrqst.AdHocAccessRequestId
                                   join rusr in _context.Users
                                   on rqst.RequesteeUserId equals rusr.Id into allusr
                                   from alusr in allusr.DefaultIfEmpty()
                                   join rqust in _context.Users
                                   on rqst.RequesterUserId equals rqust.Id into allrqst
                                   from alrqst in allrqst.DefaultIfEmpty()
                                   where acssrqst.ProgressStatus == progressStatus && acssrqst.RequestSource == AccessRequestSource.AdhocRequest
                                   select new MyRequestsGridData
                                   {
                                       AdHocAccessRequestId = rqst.Id,
                                       Name = rqst.EndPoint.DisplayName,
                                       RequestedDate = rqst.CreatedDate,
                                       ExecutionDate = acssrqst.ExecutionTime,
                                       RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                       Comments = rqst.Comments,
                                       RequestStatus = acssrqst.RequestStatus,
                                       AccessRequestID = acssrqst.Id,
                                       RequesteeName = alusr.DisplayName,
                                       RequesterName = alrqst.DisplayName,
                                       AccountType = rqst.AccountType,
                                       ValidFrom = rqst.ValidFrom,
                                       ValidTo = rqst.ValidTo,
                                       UserID = acssrqst.UserId
                                   }).ToList();
                    if (progressStatus == AccessRequestProgress.Executing)
                    {
                        requestList = requestList.Where(item => (CurrentDate - item.ExecutionDate.Value).TotalMinutes > 30).ToList();
                    }

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

        public IList<MyRequestsGridData> GetSystemRequestsByProgressStatus(AccessRequestProgress progressStatus)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    IList<MyRequestsGridData> requestList = new List<MyRequestsGridData>();
                    DateTime CurrentDate = DateTime.Now;
                    requestList = (from rqst in _context.AdHocAccessRequests
                                   join acssrqst in _context.AccessRequests.Include("User").Include("AdHocAccessRequest")
                                   on rqst.Id equals acssrqst.AdHocAccessRequestId
                                   join rusr in _context.Users
                                   on rqst.RequesteeUserId equals rusr.Id into allusr
                                   from alusr in allusr.DefaultIfEmpty()
                                   join rqust in _context.Users
                                   on rqst.RequesterUserId equals rqust.Id into allrqst
                                   from alrqst in allrqst.DefaultIfEmpty()
                                   where acssrqst.ProgressStatus == progressStatus && acssrqst.RequestSource != AccessRequestSource.AdhocRequest
                                   select new MyRequestsGridData
                                   {
                                       AdHocAccessRequestId = rqst.Id,
                                       Name = rqst.EndPoint.DisplayName,
                                       RequestedDate = rqst.CreatedDate,
                                       ExecutionDate = acssrqst.ExecutionTime,
                                       RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                       Comments = rqst.Comments,
                                       RequestStatus = acssrqst.RequestStatus,
                                       AccessRequestID = acssrqst.Id,
                                       RequesteeName = alusr.DisplayName,
                                       RequesterName = alrqst.DisplayName,
                                       AccountType = rqst.AccountType,
                                       ValidFrom = rqst.ValidFrom,
                                       ValidTo = rqst.ValidTo,
                                       UserID = acssrqst.UserId
                                   }).ToList();
                    if (progressStatus == AccessRequestProgress.Executing)
                    {
                        requestList = requestList.Where(item => (CurrentDate - item.ExecutionDate.Value).TotalMinutes > 30).ToList();
                    }

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

        public int GetAutoGeneratedNRICCount()
        {
            try
            {
                using (IACMDBContext _iacmDBcontext = new IACMDBContext())
                {
                    return _iacmDBcontext.Users.Count(item => item.NRIC.Contains("Z00"));
                }
            }
            catch
            {
                throw;
            }
        }




        //public bool CheckSelfUpdateField(string FieldName)
        //{
        //    using (IACMDBContext _iacmDBcontext = new IACMDBContext())
        //    {
        //        return _iacmDBcontext.TrustedSources.Where(l => l.AttributeName == FieldName && l.SelfUpdate).Count() > 0 ? true : false;
        //    }
        //}


        #region AddedByConnectorTeam

        public void CreateEmployeeEventTrigger(EmployeeEventTrigger trigger)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    oIACMDBContext.EmployeeEventTriggers.Add(trigger);
                    oIACMDBContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateEmployeeEventTrigger(EmployeeEventTrigger trigger)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(trigger);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<EmployeeEventTrigger>();
                        EmployeeEventTrigger attached = set.Find(trigger.Id);

                        if (attached != null)
                        {
                            var attachedEntry = _context.Entry(attached);
                            attachedEntry.CurrentValues.SetValues(trigger);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while updating  the User", ex);
                throw ex;
            }
        }

        public EmployeeEventTrigger GetEmployeeEventTrigger(long userId, EmployeeEvents evnt)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.EmployeeEventTriggers.FirstOrDefault(item => item.UserId == userId &&
                        item.EventType == evnt && item.IsRequestCreated == false && item.IsActive);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeEventTrigger GetEmployeeEventTriggerToExecute(int userId, EmployeeEvents evnt)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.EmployeeEventTriggers.FirstOrDefault(item => item.UserId == userId &&
                        item.EventType == evnt && item.IsRequestCreated && item.IsActive);  //&& item.IsExecuted == false
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeEventTrigger GetEmployeeEventTrigger(long userId, EmployeeEvents evnt, DateTime date)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.EmployeeEventTriggers.FirstOrDefault(item => item.UserId == userId &&
                        item.EventType == evnt && item.IsRequestCreated && item.IsActive && item.CreatedDate >= date);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ORACLE
        public IList<OracleUMRoleInfo> GetAllOracleRolesByUserId(int UserId)
        {
            IList<OracleUMRoleInfo> list = new List<OracleUMRoleInfo>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    OracleUMUserInfo OraUserInfo = oIACMDBContext.OracleUMUserInfo.Include("OracleRoles").FirstOrDefault(user => user.Id == UserId && !user.IsLocked);
                    if (OraUserInfo.OracleRoles != null)
                        list = OraUserInfo.OracleRoles;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return list;
        }
        #endregion


        #region LINUX
        public IList<LinuxGroupInfo> GetAllUnixGroupInfoByUserId(int UserId)
        {
            IList<LinuxGroupInfo> list = new List<LinuxGroupInfo>();

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var listLinuxUsers = _context.LinuxUserInfos.Include("LinuxGroupInfo")
                        .FirstOrDefault(item => item.Id == UserId && !item.IsLocked);//.UnixGroup.ToList();
                    list = (listLinuxUsers == null) ? new List<LinuxGroupInfo>() : (listLinuxUsers.LinuxGroup == null) ? new List<LinuxGroupInfo>() : listLinuxUsers.LinuxGroup.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return list;
        }
        #endregion

        //#region Dashboard
        public IList<User> GetAssigneeInfo(long? userId)
        {
            using (IACMDBContext context = new IACMDBContext())
            {
                var lqAssigneeInfo = (from user in context.Users.Where(leUser => leUser.Id == userId)
                                       select new
                                      {
                                          DisplayName = user.DisplayName,
                                          MobileNumber = user.MobileNumber,
                                          EmailId = user.EmailId,
                                          DepartmentName = user.UserExtensionObj.DepartmentName,
                                          PositionName = user.UserExtensionObj.PositionName,
                                          SectionName = user.UserExtensionObj.SectionName
                                      }).ToList().Select(leUser => new User
                                      {

                                          DisplayName = leUser.DisplayName,
                                          MobileNumber = leUser.MobileNumber,
                                          EmailId = leUser.EmailId,
                                          //DepartmentName = leUser.DepartmentName,
                                          //PositionName = leUser.PositionName,
                                          //SectionName = leUser.SectionName
                                      }).ToList();
                return lqAssigneeInfo;
            }

        }
        //#endregion


    }
}
