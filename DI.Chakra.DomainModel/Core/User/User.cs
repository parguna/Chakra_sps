using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.DomainModel.UserHelper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml.Serialization;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class User : DomainModelBase<User, EntityModel.User>, IComparer<User>, IEndpointUserInfo, IOrgChangeTrack
    {

        public long EndPointSystemId { get; set; }
        public string IdentificationNo { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string KnownAs { get; set; }
        public string EmployeeId { get; set; }
        public bool FullTimeEmployee { get; set; }
        public string EEGroup { get; set; }
        public string EmployeeStatus { get; set; }
        public bool IsExecutiveEmployee { get; set; }

        public Nullable<long> CompanyId { get; set; }
        public Nullable<long> DepartmentId { get; set; }
        public Nullable<long> DivisionId { get; set; }
        public Nullable<long> NOrganisationId { get; set; }
        public Nullable<long> NBusinessRoleId { get; set; }
        public Nullable<long> ManagerId { get; set; }
        public string EmailId { get; set; }
        private string _homephoneNumber;
        public string HomephoneNumber
        {
            get
            {
                return _homephoneNumber;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _homephoneNumber = value.Trim();
                _homephoneNumber = value;
            }
        }
        private string _telephoneNumber;
        public string TelephoneNumber
        {
            get
            {
                return _telephoneNumber;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _telephoneNumber = value.Trim();
                _telephoneNumber = value;
            }
        }
        private string _mobileNumber;
        public string MobileNumber
        {
            get
            {
                return _mobileNumber;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _mobileNumber = value.Trim();
                _mobileNumber = value;
            }
        }

        public string Fax { get; set; }

        public string PostalCode { get; set; }
        public string PostalAddress { get; set; }

        public Nullable<DateTime> JoinDate { get; set; }
        public Nullable<DateTime> RehireDate { get; set; }
        public Nullable<DateTime> TerminationDate { get; set; }

        public bool IsActive { get; set; }
        public string Ethinicity { get; set; }
        public string Country { get; set; }
        public EmployeeType EmployeeType { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }

        public string CostCenter { get; set; }
        public string CostCenterText { get; set; }
        public string CostDistribution { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string SectionCode { get; set; }
        public string SectionName { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public string Supervisor { get; set; }
        public string AuthenticationMode { get; set; }

        public string OrgCompany { get; set; }
        public string OrgDepartment { get; set; }
        public string OrgDivision { get; set; }
        public string OrgBranch { get; set; }
        public string OrgSection { get; set; }
        public string OrgUnit { get; set; }

        public string OneKeyTokenId { get; set; }
        public string FLINSLoginId { get; set; }

        public string ROEmployeeId { get; set; }
        public string COEmployeeId { get; set; }
        public string ProjectDescription { get; set; }
        public long ReferenceUserId { get; set; }

        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public string Reserved4 { get; set; }
        public string Reserved5 { get; set; }
        public string Reserved6 { get; set; }
        public string Reserved7 { get; set; }
        public string Reserved8 { get; set; }
        public string Reserved9 { get; set; }
        public string Reserved10 { get; set; }
        public bool NeedEmailUpdateToHR { get; set; }

        public bool NeedFLINSLoginIdUpdateToHR { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public Nullable<DateTime> LastLogonDate { get; set; }
        public List<Group> Groups { get; set; }
        public List<Role> Roles { get; set; }

        public NOrganisation NOrganisation { get; set; }
        public NDesignation NBusinessRole { get; set; }
        public string OrgChartLevels { get; set; }
        public string NCompany { get; set; }

        public int SortDirection;
        public bool NeedTelephoneUpdateToHR { get; set; }
        public string BranchCode { get; set; }

        public string UnitCode { get; set; }
        public string OrganisationCode { get; set; }
        public string AccountName
        {
            get
            {
                return UserName;
            }

        }

        public string AccountPassword
        {
            get
            {
                return "";
            }
        }
        [XmlIgnore]
        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public override string ToString()
        {
            return this.FirstName + this.LastName;
        }

        public static User GetDummyUser()
        {
            User user = new User();

            user.JoinDate = null;
            user.RehireDate = null;
            user.TerminationDate = null;
            user.CreatedDate = DateTime.Now;
            user.LastModifiedDate = DateTime.Now;
            return user;
        }

        /// <summary>
        /// Checks if the given instance is equals to the current one.
        /// </summary>
        /// <param name="obj">The object instance to check.</param>
        /// <returns>True if they equals, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is User)
            {
                User other = (User)obj;
                return this.Id.Equals(other.Id);
            }
            else
                return false;
        }

        /// <summary>
        /// Gets the hash code of the category.
        /// </summary>
        /// <returns>The category hash code.</returns>
        public override int GetHashCode()
        {
            if (this.DisplayName == null)
                return this.Id.GetHashCode();

            return this.Id.GetHashCode() ^ this.DisplayName.GetHashCode();
        }

        #region IComparer<Role> Members

        public int Compare(User x, User y)
        {
            int cmprValue = -1;
            if (x != null && y != null)
            {
                if (SortDirection == 0)
                {
                    cmprValue = string.Compare(x.UserName, y.UserName);

                }
                else
                {
                    cmprValue = string.Compare(y.UserName, x.UserName);
                }
            }

            return cmprValue;
        }

        #endregion

        public static User GetROByUser(long id)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new UserRepository(transaction).GetReportingOfficer(id));
            }
        }

        public static List<long> GetSameOUUserIDs(long diUserId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetSameOUUserIDs(diUserId);
            }
        }

        public static Dictionary<long, string> GetUsersForBulkRequest(string searchString, bool IsFulltimeEmployee, string rosEmpId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                var priorOnBoardDays = ConfigurationMaster.GetConfigKey("OnBoardDaysBeforeJoining", Product.IACM);


                DateTime joinDate = DateTime.Today.AddDays(Convert.ToInt32(priorOnBoardDays));

                if (!string.IsNullOrEmpty(rosEmpId))
                {


                    var result = new UserRepository(transaction).Get(item => item.ROEmployeeId == rosEmpId
                                                                    && item.JoinDate <= joinDate
                                                                    && ((item.TerminationDate == null || (item.TerminationDate != null && item.TerminationDate > DateTime.Now)))
                                                                    && item.FullTimeEmployee == IsFulltimeEmployee
                                                                    && item.IsActive == true
                                                                    && ((string.IsNullOrEmpty(searchString) ? true : (item.DisplayName.StartsWith(searchString) || item.DisplayName.Contains(searchString))))
                                                                    //&& (item.DisplayName.StartsWith(searchString) || item.DisplayName.Contains(searchString)) 
                                                                    ).OrderBy(i => i.DisplayName).ToDictionary(key => key.Id, value => value.DisplayName + " [" + value.UserName + "]");
                    return result;
                }
                else
                {
                    var result = new UserRepository(transaction).Get(item => item.JoinDate <= joinDate && ((item.TerminationDate == null || (item.TerminationDate != null
                                                                    && item.TerminationDate > DateTime.Now)))
                                                                    && item.FullTimeEmployee == IsFulltimeEmployee
                                                                    && item.IsActive == true
                                                                    && item.ROEmployeeId != null
                                                                    //&& ((string.IsNullOrEmpty(searchString) ? true : item.UserName.ToLower().Contains(searchString.ToLower()))
                                                                    && ((string.IsNullOrEmpty(searchString) ? true : (item.DisplayName.StartsWith(searchString) || item.DisplayName.Contains(searchString))))
                                                                    ).OrderBy(i => i.DisplayName).ToDictionary(key => key.Id, value => value.DisplayName + " [" + value.UserName + "]");
                    return result;
                }
            }
        }

        public static User GetCOByUser(long id)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new UserRepository(transaction).GetCounterSigningOfficer(id));
            }
        }

        public static bool AddEntitlement(long epUserId, EntitlementType _entitlementType, string entitlements)
        {
            try
            {
                User currentIDASUser = User.GetByIdIncludeRoles(epUserId);

                if (currentIDASUser == null)
                    return false;
                if (entitlements != null)
                {
                    foreach (var item in entitlements.Split(','))
                    {
                        var role = currentIDASUser.Roles.Find(g => g.RoleName.ToUpper() == item.ToUpper());

                        if (role == null)
                        {
                            var newRoleToAdd = Role.GetByRoleName(item);
                            if (newRoleToAdd != null)
                                InsertUserRole(epUserId, newRoleToAdd.Id);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }
        public static bool RemoveEntitlement(long epUserId, EntitlementType _entitlementType, string entitlements)
        {
            try
            {
                using (var transaction = new DataAccessTransaction())
                {
                    User currentIDASUser = User.GetByIdIncludeRoles(epUserId);

                    if (currentIDASUser == null)
                        return false;

                    foreach (var item in entitlements.Split(','))
                    {
                        var role = currentIDASUser.Roles.Find(g => g.RoleName.ToUpper() == item.ToUpper());

                        if (null != role)
                            new UserRepository(transaction).DeleteRole(currentIDASUser.Id, role.Id);
                    }
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }
        public static User GetByIdIncludeBusinessRoleAndOrganisation(long id)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.User>(transaction).GetById(id, User => User.Roles, user => user.NOrganisation, user => user.NBusinessRole));
            }
        }
        public static User GetByIdIncludeNBusinessRoleAndNOrganisation(long id)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.User>(transaction).GetById(id,
                    user => user.NBusinessRole,
                    user => user.NOrganisation));
            }
        }
        public static User GetByIdIncludeGroup(long id)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.User>(transaction).GetById(id, user => user.Groups));
            }
        }

        public static bool IsAdminUser(long userId)
        {
            var isAdmin = false;
            var user = GetByIdIncludeGroup(userId);
            if (user == null)
                return isAdmin;

            if (user.Groups == null && user.Groups.Count == 0)
                return isAdmin;

            var itGroupId = MasterConfigHelper.ITSecurityGroupId;
            isAdmin = user.Groups.Any(p => p.GroupName == "Administrator" || p.Id == itGroupId);
            return isAdmin;
        }
        public static List<User> GetUnlockedAccountsByDIUserId(long userId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new UserRepository(transaction).GetUnlockedAccountsByDIUserId(userId));
            }
        }
        public static User GetByIdIncludeRoles(long id)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.User>(transaction).GetById(id, user => user.Roles));
            }
        }

        public static List<User> GetAllIncludeRolesAndGroups()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new UserRepository(transaction).GetAllActiveUserWithRoleAndGroup().ToList());
            }
        }

        public static User GetByIdUserExtensionIncluded(int id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return ORMap(new RepositoryBase<EntityModel.User>(dataAccessTransaction).
                        Get(usr =>
                            usr.Id == id
                            ).
                            FirstOrDefault());
            }
        }

        public static bool IsInternal(long userId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                var data = new RepositoryBase<EntityModel.User>(transaction).Get(user => user.Id == userId).FirstOrDefault().FullTimeEmployee;
                return data;

            }
        }

        public static User GetByDisplayName(string displayname)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.DisplayName == displayname).FirstOrDefault());
            }

        }

        public static User GetByADSUserName(string userName)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.UserName == userName).FirstOrDefault());
            }

        }

        public static User GetByNRIC(string nric)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.IdentificationNo == nric && usr.FullTimeEmployee,
                    user => user.NOrganisation).FirstOrDefault());
            }

        }

        public static User GetByNRICByEmployeeType(string nric, int employeeType)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.IdentificationNo == nric && (int)usr.EmployeeType == employeeType,
                    user => user.NBusinessRole,
                    user => user.NOrganisation).FirstOrDefault());
            }

        }

        public static User GetByPositionCode(string positionCode)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.NBusinessRole.Code == positionCode).FirstOrDefault());
            }

        }
        public static bool IsPostalCodeExist(string postalCode)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.User>(transaction).Any(usr => usr.PostalCode == postalCode);
            }

        }

        private static void IsAdmin(out bool isAdminGroup, out bool isIACMAdminGroup, long userID)
        {
            //using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                isAdminGroup = isIACMAdminGroup = false;
                User aUser = User.GetById(userID);
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

        public static bool IsHOSorAdmin(long userID)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                bool IsAdminGroup = true;
                bool isIACMAdminGroup = true;

                IsAdmin(out IsAdminGroup, out isIACMAdminGroup, userID);
                bool IsHOS = NOrganisation.IsUserOrgHead(userID);

                return IsAdminGroup || isIACMAdminGroup || IsHOS;
            }
        }

        public static User GetByUserName(string name)
        {

            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                User user = ORMap(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.UserName.Equals(name, StringComparison.CurrentCultureIgnoreCase), true).FirstOrDefault());
                if (user != null)
                {
                    user.Password = RSAHelper.DecryptData(user.Password);
                    return user;
                }
                else
                    return null;
            }

        }
        public static User GetActiveByUserName(string name)
        {

            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                User user = ORMap(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.UserName.Equals(name, StringComparison.CurrentCultureIgnoreCase) && usr.IsActive, true).FirstOrDefault());
                if (user != null)
                {
                    user.Password = RSAHelper.DecryptData(user.Password);
                    return user;
                }
                else
                    return null;
            }

        }
        public static User GetByUserNameFromIMProcessor(string name)
        {

            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                User user = ORMap(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.UserName.Equals(name, StringComparison.CurrentCultureIgnoreCase), true).FirstOrDefault());
                if (user != null)
                {
                    return user;
                }
                else
                    return null;
            }

        }


        public static User GetByUserNric(string userName, string nric, DateTime dob)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                User user = ORMap(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.IdentificationNo == nric && usr.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase), true).FirstOrDefault());
                if (user != null)
                {
                    user.Password = RSAHelper.DecryptData(user.Password);
                    return user;
                }
                else
                    return null;
            }
        }


        public static Boolean Authenticate(string userName, string password)
        {
            User user = GetActiveByUserName(userName);

            if (user == null)
                return false;

            if (user.IsActive == false)
                return false;

            if (user.EmployeeType != EmployeeType.Permanent && user.EmployeeType != EmployeeType.TempStaff)
                return false;

            if (!user.Password.Equals(password, StringComparison.InvariantCulture))
                return false;

            if (user.TerminationDate.HasValue && user.TerminationDate.Value < DateTime.Now)
                return false;

            return true;
        }

        public static User GetByName(string userName)
        {
            return GetByUserName(userName);
        }

        //public static List<User> GetAllManagers(string tenantId)
        //{
        //    using (DataAccessTransaction transaction = new DataAccessTransaction())
        //    {
        //        if (!string.IsNullOrEmpty(tenantId))
        //            return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.TenantId == tenantId && usr.ManagerId == usr.Id).ToList());
        //        else
        //            return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.ManagerId == usr.Id).ToList());
        //    }

        //}

        public static bool IsValid(string value, ApprovalUserType accType)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                if (string.IsNullOrWhiteSpace(value) && accType == ApprovalUserType.None)
                    return true;

                if (string.IsNullOrWhiteSpace(value) && accType != ApprovalUserType.None)
                    return false;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (accType == ApprovalUserType.User)
                        return new RepositoryBase<EntityModel.User>(transaction).Any(cmp => cmp.EmployeeId == value && cmp.IsActive);
                    else if (accType == ApprovalUserType.Group)
                        return new RepositoryBase<EntityModel.Group>(transaction).Any(cmp => cmp.GroupName == value);
                    else
                        return false;
                }
                return false;
            }

        }

        public static void UpdateLastLogonInformation(User user)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(user.UserName))
            {
                User oUser = User.GetById(user.Id);

                if (oUser != null)
                {
                    oUser.LastLogonDate = DateTime.Now;
                    oUser.Save(transaction);
                }

                UserGridFilterInfo.ClearFilterByUserId(user.Id);
                transaction.Commit();
            }
        }

        public static void WriteLoginResult(string userName, string message)
        {
            if (message == Boolean.TrueString)
            {
                DILog.AuthenticationLogger.WriteLog(LogLevelType.Success, string.Format("IACM Authentication Successful for the User: {0}", userName));
            }
            else
            {
                DILog.AuthenticationLogger.WriteLog(LogLevelType.Error, string.Format("IACM Authentication Failed for the User: {0}", userName));
            }
        }


        public static void DeleteByUserRole(int roleId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new UserRepository(dataAccessTransaction).DeleteByUserRole(roleId);
            }
        }


        public static List<User> GetOnBoardingUsers(bool forAdminCheck)
        {
            DateTime dueCutoff;
            if (!EmployeeEventTrigger.IsEventProcessWindowOpen(DateTime.Now, out dueCutoff))
                return new List<User>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new UserRepository(dataAccessTransaction).GetOnBoardingUsers(forAdminCheck, dueCutoff).ToList());
            }
        }

        public static List<User> GetEmployeeConversionUsers(bool forAdminCheck)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new UserRepository(dataAccessTransaction).GetEmployeeConversionUsers(forAdminCheck).ToList());
            }
        }

        public static List<User> GetRehireUsers(bool forAdminCheck)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new UserRepository(dataAccessTransaction).GetRehireUsers(forAdminCheck).ToList());
            }
        }

        public static List<User> GetBackkDatedExceptions(int onboardBackdatedWindowperiod, DateTime? fromDate, DateTime? toDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new UserRepository(dataAccessTransaction).GetBackkDatedExceptions(onboardBackdatedWindowperiod, fromDate, toDate).ToList());
            }
        }

        public static List<User> GetOffBoardingUsers()
        {
            DateTime dueCutoff;
            if (!EmployeeEventTrigger.IsEventProcessWindowOpen(DateTime.Now, out dueCutoff))
                return new List<User>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new UserRepository(dataAccessTransaction).GetOffBoardingUsers(dueCutoff).ToList());
            }
        }

        public static List<User> GetNotifiableUsers(long taskId, long requestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new UserRepository(dataAccessTransaction).GetNotifiableUsers(taskId, requestId).ToList());
            }
        }

        //public static List<User> GetAllByTenantId(string tenantId)
        //{
        //    using (DataAccessTransaction transaction = new DataAccessTransaction())
        //    {
        //        if (!string.IsNullOrEmpty(tenantId))
        //            return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.TenantId == tenantId).ToList());
        //        else
        //            return User.GetAll();
        //    }

        //}


        public static List<User> GetFullTimeEmployees(bool fullTimeEmp, int startRowIndex, int maximumRows, string FilterExpression, string sortExpression)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                var userList = new List<User>();
                if (FilterExpression != null && sortExpression != null)
                {
                    userList = ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.FullTimeEmployee == fullTimeEmp, usr => usr.NOrganisation).AsQueryable().Where(FilterExpression).OrderBy(sortExpression).ToList());
                }
                else if (FilterExpression != null)
                {
                    userList = ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.FullTimeEmployee == fullTimeEmp, usr => usr.NOrganisation).AsQueryable().Where(FilterExpression).ToList());

                }
                else if (sortExpression != null)
                {
                    userList = ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.FullTimeEmployee == fullTimeEmp, usr => usr.NOrganisation).OrderBy(sortExpression).Skip(startRowIndex).Take(maximumRows).ToList());
                }
                else
                {
                    userList = ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.FullTimeEmployee == fullTimeEmp, usr => usr.NOrganisation).Skip(startRowIndex).Take(maximumRows).ToList());
                }
                if (userList != null && userList.Count > 0 && userList.Any(a => a.NOrganisation == null))
                {
                    userList.Where(a => a.NOrganisation == null).ToList().ForEach(a => { a.NOrganisation = new NOrganisation(); });
                }
                //foreach (var item in userList)
                //{
                //    item.NOrganisation = item.NOrganisationId.HasValue ? NOrganisation.GetById(item.NOrganisationId.Value) : null;
                //}
                return userList;
            }

        }

        public static Dictionary<long, string> GetGroupMembersbyUserId(string searchString, long userId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                var CoordinatorRole = MasterConfigHelper.BulkAssignTaskRoleToExclude;
                List<long> roles = new List<long>();
                if (!string.IsNullOrWhiteSpace(CoordinatorRole))
                {
                    char[] delimiter = { ',' };
                    var tempRoles = CoordinatorRole.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (tempRoles.Count > 0)
                    {
                        foreach (var tempRole in tempRoles)
                        {
                            long roleId;
                            if (Int64.TryParse(tempRole, out roleId))
                            {
                                roles.Add(roleId);
                            }
                        }
                    }
                }

                return new UserRepository(transaction).GetGroupMembersbyUserId(searchString, userId, roles);
            }
        }

        public static long GetFullTimeEmployeesCount(bool fullTimeEmp)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.User>(transaction).Count(usr => usr.FullTimeEmployee == fullTimeEmp);
            }

        }

        public static bool IsPermanent(long userID)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.User>(transaction).Any(user => user.Id == userID && user.FullTimeEmployee == true);
            }

        }

        public static List<User> GetActiveEmployees()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.IsActive).ToList());
            }
        }

        public static string GetEmployeeId(string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetEmployeeId(searchString);
            }
        }

        public static User GetCompanyDetails(string prefix)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new UserRepository(transaction).GetCompanyDetails(prefix));
            }
        }

        public static Dictionary<long, string> GetReportingManager(string prefix)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetReportingManager(CheckNull.AsteriskToString(prefix));
            }
        }

        public static Dictionary<string, string> GetPosition(string prefix)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetPosition(CheckNull.AsteriskToString(prefix));
            }
        }

        public static Dictionary<string, string> GetReferenceUser(string prefix)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetReferenceUser(CheckNull.AsteriskToString(prefix));
            }
        }
        public static Dictionary<string, string> GetReferencePermanentUser(string prefix)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetReferencePermanentUser(CheckNull.AsteriskToString(prefix));
            }
        }
        public static Dictionary<long, string> GetNonReportersOfManager(long managerID, string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetNonReportersOfManager(managerID, searchString);
            }
        }


        public static List<User> GetByOrganization(long orgUnit, UserType userType)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                IList<User> retList = new List<User>();
                if (userType == UserType.User)
                    return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.NOrganisationId == orgUnit).ToList());
                else //in case userType = Manager
                    return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.NOrganisationId == orgUnit && usr.ROEmployeeId == usr.EmployeeId).ToList());
            }
        }

        public static Dictionary<long, string> GetBySearchString(string searchString, bool hidden)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetBySearchString(CheckNull.AsteriskToString(searchString), hidden);
            }
        }

        public static Dictionary<long, string> GetROUsersBySearchString(string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetROUsersBySearchString(CheckNull.AsteriskToString(searchString));
            }
        }

        public static Dictionary<long, string> GetBySearchStringAndConsiderJoinDate(string searchString, bool hidden, bool considerJoinDate)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetBySearchString(CheckNull.AsteriskToString(searchString), hidden, considerJoinDate);
            }
        }

        public static Dictionary<long, string> GetActiveAndNonTerminaters(string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetActiveAndNonTerminaters(CheckNull.AsteriskToString(searchString));
            }
        }

        public static List<User> GetByCompany(string companyCode)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).GetAll().Where(user => user.NOrganisation.Code == companyCode).ToList());
            }
        }

        public static Dictionary<long, string> GetReportingUsersOfManagerAndAdmin(long managerId, string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                bool IsAdmin = false;
                bool IsIACMAdmin = false;
                User.IsAdmin(out IsAdmin, out IsIACMAdmin, managerId);

                if (IsAdmin && IsIACMAdmin)
                    return new UserRepository(transaction).GetReportingUsersOfManagerAndAdmin(true, managerId, searchString);
                else
                    return new UserRepository(transaction).GetReportingUsersOfManagerAndAdmin(false, managerId, searchString);
            }
        }

        public static string GetEndPointDetailByID(long endpointUserID, string endpointSystemName)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                if (endpointSystemName == "AD")
                    return new RepositoryBase<EntityModel.ADSUserInfo>(transaction).GetById(endpointUserID).sAMAccountName;
                else if (endpointSystemName == "SAP")
                    return new RepositoryBase<EntityModel.SAPUserinfo>(transaction).GetById(endpointUserID).UserLogonName;
                else if (endpointSystemName == "SAPEP")
                    return new RepositoryBase<EntityModel.SAPEPUserInfo>(transaction).GetById(endpointUserID).UserName;
                else if (endpointSystemName == "INH")
                    return new RepositoryBase<EntityModel.InhouseUserInfo>(transaction).GetById(endpointUserID).UserName;
                else if (endpointSystemName == "DIUser")
                    return new RepositoryBase<EntityModel.User>(transaction).GetById(endpointUserID).DisplayName;
                else
                    return null;


            }
        }

        public static List<User> GetBySection(string sectionCode)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).GetAll().Where(user => user.NOrganisation.Code == sectionCode).ToList());
            }
        }

        public static List<User> GetByGroup(List<Group> group)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                List<User> retList = new List<User>();
                if (group != null)
                {
                    List<User> allList = new List<User>();
                    foreach (Group grp in group)
                    {
                        retList = ORMapList(new RepositoryBase<EntityModel.Group>(transaction).GetById(grp.Id).Users.ToList());

                        if (retList.Count > 0)
                            allList = (List<User>)allList.Union(retList).ToList();

                    }
                    return allList;

                }
                return null;
            }
        }

        public static List<User> GetReportingUsers(long userID, string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                List<User> userList = new List<User>();

                var OrgUnitPrefixUser = (new RepositoryBase<EntityModel.NOrganisationHead>(transaction).Get(o => o.HeadId == userID)).Select(p => p.Organisation.Code);

                foreach (var org in OrgUnitPrefixUser)
                {
                    var orgList = NOrganisation.GetChildOrgs(org);
                    foreach (var orgunit in orgList)
                    {
                        List<User> UserofOrgUnit = ORMapList(new RepositoryBase<EntityModel.User>(transaction).GetAll().Where(user => user.DisplayName.Contains(searchString) && user.NOrganisation.Code == orgunit).ToList());
                        foreach (User user in UserofOrgUnit)
                        {
                            userList.Add(user);
                        }
                    }
                }

                return userList;
            }
        }

        public static List<User> GetByUpdateEmailIdToHR()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                var AllUser = ORMapList(new RepositoryBase<EntityModel.User>(transaction).GetAll().ToList());
                //return AllUser.Where(user => user.UserExtensionObj != null && user.UserExtensionObj.NeedEmailUpdateToHR != null && user.UserExtensionObj.NeedEmailUpdateToHR).ToList();
                return null;
            }
        }

        public static List<User> GetByUpdateFLINSLoginToHR()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                var AllUser = ORMapList(new RepositoryBase<EntityModel.User>(transaction).GetAll().ToList());
                //return AllUser.Where(user => user.UserExtensionObj != null && user.UserExtensionObj.NeedFLINSLoginIdUpdateToHR != null && user.UserExtensionObj.NeedFLINSLoginIdUpdateToHR).ToList();
                return null;
            }
        }

        public static bool DoesExists(string username)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.User>(transaction).Any(user => user.UserName.Equals(username, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        public static bool DoesEmployeeExists(string employeeId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.User>(transaction).Any(user => user.EmployeeId.Equals(employeeId, StringComparison.CurrentCultureIgnoreCase));
            }
        }
        public static bool DoesIdentificationNumberExists(string identificationNumber, int employeeType)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.User>(transaction).Any(user => user.IdentificationNo.Equals(identificationNumber, StringComparison.CurrentCultureIgnoreCase) && (int)user.EmployeeType == employeeType);
            }
        }

        public static long GetAutoGeneratedNRICCount()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.User>(transaction).Count(user => user.IdentificationNo.Contains("Z00"));
            }
        }

        public static List<User> GetAssigneeInfo(long? userId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new UserRepository(transaction).GetAssigneeInfo(userId));
            }
        }

        public static User GetByEmployeeId(string employeeId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.EmployeeId == employeeId).FirstOrDefault());
            }
        }
        public static User CheckDisplayNameDuplicate(long userId, string displayName)
        {
            User userObj = null;
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                if (userId > 0)
                {
                    userObj = ORMap(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.FullTimeEmployee == false && user.Id != userId && user.DisplayName == displayName).FirstOrDefault());
                }
                else
                {
                    userObj = ORMap(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.FullTimeEmployee == false && user.DisplayName == displayName).FirstOrDefault());
                }
                return userObj;
            }
        }
        public static List<User> GetAllByEndPointId(long endPointId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.EndPointSystemId == endPointId).ToList());
            }
        }

        public static User GetByEmployeeId(string employeeId, long endpointSystemId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.EmployeeId == employeeId
                                                                                            && user.EndPointSystemId == endpointSystemId).FirstOrDefault());
            }
        }

        public static long GetIdByUserName(string userName)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.UserName == userName).FirstOrDefault()).Id;
            }
        }

        public static User GetUserByUserNameorIdNo(string userName, string identificatioNo)
        {
            User usr = null;

            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {

                usr = ORMap(new RepositoryBase<EntityModel.User>(transaction).
                  Get(user => (user.IdentificationNo == identificatioNo)).FirstOrDefault());

                if (usr == null && !string.IsNullOrEmpty(userName))
                {
                    usr = ORMap(new RepositoryBase<EntityModel.User>(transaction).
                        Get(user => (user.UserName == userName)).FirstOrDefault());
                }
            }

            return usr;
        }

        public static string GetDisplayName(long usrId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return (new RepositoryBase<EntityModel.User>(transaction).Get(user => user.Id == usrId).FirstOrDefault()).DisplayName;
            }
        }

        public static List<User> GetAllImplementers()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                List<User> userList = new List<User>();
                List<Group> groupList = Group.GetByGroupName();
                List<User> tmpUserList = ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.EmailId != null && usr.EmailId != "").Distinct().ToList());

                foreach (Group grpListItem in groupList)
                {
                    foreach (User tmpUsrListItem in tmpUserList)
                    {
                        tmpUsrListItem.EmailId = !string.IsNullOrEmpty(tmpUsrListItem.EmailId) ? tmpUsrListItem.EmailId.Trim().ToLower() : tmpUsrListItem.EmailId;
                        userList.Add(tmpUsrListItem);
                    }
                }

                return userList;

            }
        }


        public static Dictionary<long, string> GetReportingEmployees(Presentation.PresentationReportingEmployees reportingEmployees)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                List<User> user = new List<User>();
                bool isAdmin = false;
                bool isIACMAdmin = false;
                IsAdmin(out isAdmin, out isIACMAdmin, reportingEmployees.UserID);

                if (isAdmin || isIACMAdmin)
                {
                    user = ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(
                        item => item.DisplayName.Contains(reportingEmployees.SearchString)
                            && (reportingEmployees.SectionCode.Contains(item.NOrganisation.Code))
                           ).ToList());
                }
                else

                    user = User.GetReportingUsers(reportingEmployees.UserID, reportingEmployees.SearchString);

                if (user.Count > 0)
                {
                    return user.Select(x => new { Key = x.Id, Value = x.DisplayName + " [" + x.UserName + "]" })
                                               .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
                else
                    return null;
            }
        }

        public static List<long> GetUserGroups(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.User>(dataAccessTransaction).Get(us => us.Id == id, includeProperties: u => u.Groups).FirstOrDefault().Groups.Select(x => x.Id).ToList();
            }
        }

        public static void UpdateManagerForUsers()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new UserRepository(dataAccessTransaction).UpdateManagerForUsers();
            }
        }

        public static void UpdateMasterLatest(DataTable dtMaster)
        {

            string strQuery = string.Empty;

            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    new UserRepository(transaction).UpdateMasterLatest(strQuery);

                    SQLBulkUploader<User> sqlUser = null;
                    string tableName = "DI_M_User";
                    sqlUser = new SQLBulkUploader<User>(tableName);
                    sqlUser.Flush(dtMaster);
                    tran.Complete();
                    tran.Dispose();
                }
                transaction.Commit();
            }
        }

        public static List<User> GetOUChangeUsers()
        {
            return GetUsers(EmployeeEventTrigger.GetOUChangeUsers(false));
        }

        public static List<User> GetRehireChangeUsers()
        {
            return GetUsers(EmployeeEventTrigger.GetRehireChangeUsers(false));
        }

        public static List<User> GetReOrgChangeUsers()
        {
            return GetUsers(EmployeeEventTrigger.GetReOrgChangeUsers(false));
        }

        public static List<User> GetReOrgUsers()
        {
            return GetUsers(EmployeeEventTrigger.GetReOrgUsers());
        }

        // Each due trigger paired with the user deserialized from that trigger's own UserDetails, so the
        // event loops process every trigger with its own data even when a user has several pending triggers.
        public static List<KeyValuePair<EmployeeEventTrigger, User>> GetOUChangeTriggerUsers()
        {
            return GetTriggerUsers(EmployeeEventTrigger.GetOUChangeUsers(false));
        }

        public static List<KeyValuePair<EmployeeEventTrigger, User>> GetRehireTriggerUsers()
        {
            return GetTriggerUsers(EmployeeEventTrigger.GetRehireChangeUsers(false));
        }

        public static List<KeyValuePair<EmployeeEventTrigger, User>> GetReOrgTriggerUsers()
        {
            return GetTriggerUsers(EmployeeEventTrigger.GetReOrgUsers());
        }

        private static List<User> GetUsers(List<EmployeeEventTrigger> triggers)
        {
            return GetTriggerUsers(triggers).Select(pair => pair.Value).ToList();
        }

        private static List<KeyValuePair<EmployeeEventTrigger, User>> GetTriggerUsers(List<EmployeeEventTrigger> triggers)
        {
            var triggerUsers = new List<KeyValuePair<EmployeeEventTrigger, User>>();

            foreach (EmployeeEventTrigger trigger in triggers)
            {
                var user = UtilityMethods.Deserialize<User>(trigger.UserDetails);
                if (user == null)
                {
                    // A null entry makes the event loops throw and abort for every remaining trigger, so skip just this one.
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, $"Skipping EmployeeEventTrigger Id:[{trigger.Id}], Event:[{trigger.EventType}], UserId:[{trigger.UserId}] - UserDetails is missing or could not be deserialized.");
                    continue;
                }
                if (user.Id != trigger.UserId)
                {
                    // Processing would apply this data to a different user than the trigger belongs to.
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, $"Skipping EmployeeEventTrigger Id:[{trigger.Id}], Event:[{trigger.EventType}], UserId:[{trigger.UserId}] - UserDetails belongs to UserId:[{user.Id}].");
                    continue;
                }
                triggerUsers.Add(new KeyValuePair<EmployeeEventTrigger, User>(trigger, user));
            }

            return triggerUsers;
        }

        public static void UpdateUserFromDiscovery(User user, List<string> propagationFields, bool canRunPropagationTrigger, string currentUser, bool canSaveUser = true)
        {
            try
            {
                EmployeeEventTrigger.InitiatePropagateTrigger(user, propagationFields, canRunPropagationTrigger, currentUser);
                if (canSaveUser)
                {
                    user.Save(currentUser);
                }
            }
            catch
            {
                throw;
            }
        }



        public static long GetNManagerFromOrg(long userId)
        {
            User user = User.GetById(userId);

            return GetNManagerFromOrg(user);
        }

        public static long GetNManagerFromOrg(User user)
        {
            string approver = ApproverException.CheckForApproverInException(user.EmployeeId, 1);

            if (!string.IsNullOrEmpty(approver))
            {
                var appUser = User.GetByEmployeeId(approver);
                if (appUser != null)
                    return appUser.Id;
                else
                    return 0;
            }
            else
            {
                var immediateMgr = NOrganisationHead.GetImmediateSupervisor(user.Id);

                if (immediateMgr != null)
                    return immediateMgr.Id;
            }

            return 0;
        }

        public static long GetManagerFromOrgScope(User user, string scope)
        {
            string approver = ApproverException.CheckForApproverInException(user.EmployeeId, 1);

            if (!string.IsNullOrEmpty(approver))
            {
                var appUser = User.GetByEmployeeId(approver);
                if (appUser != null)
                    return appUser.Id;
                else
                    return 0;
            }
            else
            {
                var immediateMgr = NOrganisationHead.GetImmediateSupervisor(user.Id);

                if (immediateMgr != null)
                    return immediateMgr.Id;
            }

            return 0;
        }

        //public static void DeactivateUsers(List<User> deletedUsers)
        //{
        //    foreach (User usr in deletedUsers)
        //    {
        //        usr.IsActive = false;
        //        usr.TerminationDate = DateTime.Now;
        //        usr.Save();
        //        DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format(" User {0} has been terminated", usr.UserName));                
        //    }
        //}

        public static void InsertUserRole(long userID, long roleId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                new UserRepository(transaction).InsertUserRole(userID, roleId);
                transaction.Commit();
            }
        }

        public static void DoAfterSaveUser(User user, string currentUser)
        {
            try
            {
                EndPoint ep = EndPoint.GetByApplicationType(ApplicationType.IDMS);
                IACMUserInfo iACMUserInfo = IacmUserHelper.CreateAndSaveIacmUserInfo(user, null, ep, null);
                if (iACMUserInfo != null)
                {
                    long endpointId = 0;
                    if (ep != null)
                        endpointId = ep.Id;
                    IACMUserInfo.SaveUsers(new List<IACMUserInfo> { iACMUserInfo }, endpointId, currentUser);
                }
                var accRec = AccountReconcilation.GetByAccountName(user.UserName, ep.Id);
                if (accRec != null)
                {
                    accRec.UserId = user.Id;
                    accRec.IsDeleted = !user.IsActive;
                    accRec.ValidToDate = user.TerminationDate.HasValue ? user.TerminationDate.Value : DateTime.MaxValue;
                    accRec.Save(currentUser);
                }


                //IacmUserHelper.MapUserIacmUser(ep, true, 0, user.Id.ToString());
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
            }
        }

        public static bool CheckUserGroup(long userID, long groupID)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).CheckUserGroup(userID, groupID);
            }
        }

        public static List<User> GetOrganisationByUserType(UserType _userType, IList<NOrganisation> _list) // should remove '_' underscore
        {

            List<User> retList = null;

            if (null == _list)
                return null;
            retList = new List<User>();

            // Question: Why this variable?
            List<User> allList = new List<User>();

            foreach (NOrganisation organisation in _list)
            {
                retList = GetByOrganization(Convert.ToInt32(organisation.Id), _userType);
                if (retList.Count > 0)
                    allList = (List<User>)allList.Union(retList).ToList(); // Question: Why this union
            }
            return allList;

        }


        public static bool IsUserAccount(long userID, long endPointSystemID, long EndpointSystemType, AccountType accountType)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).IsUserAccount(userID, endPointSystemID, EndpointSystemType, accountType);
            }
        }
        public static void GetAllMasterUsers(IList<SAPUserinfo> listSAPUsers, bool p, string currentUser)
        {
            try
            {
                DataRow[] dRowsMaster = null;

                IList<User> users = new List<User>();
                IList<ConfigGeneralTrustedSource> listTrustedConfig = null;
                IList<ConfigGeneralTrustedSource> list = null; //new DiConfigurationDA().GetAllConfigTrustedSource();
                listTrustedConfig = list.Where(li => li.TrustedSource == EndpointType.SAP).ToList();
                if (listTrustedConfig.Count <= 0)
                {
                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Populate User: No attribute is configured from SAP");
                    return;
                }

                IList<User> listUsers = new List<User>();

                foreach (SAPUserinfo sapuser in listSAPUsers)
                {
                    AccountReconcilation useracc = AccountReconcilation.GetByUser(sapuser.Id, sapuser.EndPointSystemId);
                    IList<User> existingMasteruser = (from usr in listUsers where usr.Id == useracc.UserId select usr).ToList();
                    IList<SAPUserinfo> sapusr = (from sapu in listSAPUsers where sapu.UserLogonName == sapuser.UserLogonName select sapu).ToList();
                    if (existingMasteruser != null && existingMasteruser.Count() > 0)
                    {
                        DataTable dtMaster1 = CollectionHelper.ConvertToNormalTable(existingMasteruser);
                        DataTable dtSapUser1 = CollectionHelper.ConvertToNormalTable(sapusr);
                        foreach (DataRow adsRow in dtSapUser1.Rows)
                        {
                            dRowsMaster = dtMaster1.Select(string.Format("UserId={0}", useracc.UserId));
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
                        var original = GetById(masterusr.FirstOrDefault().Id);
                        if (original != null)
                        {
                            masterusr.FirstOrDefault().CreatedDate = original.CreatedDate;
                            masterusr.FirstOrDefault().Save(currentUser);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<AccountReconcilation> HandleServiceAccount(User user, string currentUser, long? triggerId)
        {


            var adAccountRecon = new List<AccountReconcilation>();
            try
            {
                var sectionHeadUser = GetROByUser(user.Id);
                if (sectionHeadUser == null || sectionHeadUser.IsActive == false)
                {
                    sectionHeadUser = GetCOByUser(user.Id);
                    if (sectionHeadUser == null || sectionHeadUser.IsActive == false)
                    {
                        sectionHeadUser = null;
                    }
                }

                var listAcs = AccountReconcilation.GetAllByUser(user.Id, false);
                var adEndpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                var adEndpointSAP = EndPoint.GetByApplicationType(ApplicationType.SAPLocal);
                if (listAcs != null && listAcs.Count > 0)
                {
                    listAcs = listAcs.Where(t => t.IsDeleted == false && (t.AccountType == AccountType.Service || t.AccountType == AccountType.Shared)).ToList();
                    Guid transactionId = Guid.NewGuid();
                    foreach (var listAc in listAcs)
                    {
                        try
                        {
                            if (sectionHeadUser != null && sectionHeadUser.IsActive)
                            {
                                listAc.OffboardingAccountAction = OffboardingAccountActionType.Supervisor;
                                listAc.UserId = sectionHeadUser.Id;
                                listAc.Save(currentUser);
                                var entitlementRecon = EntitlementReconcilation.GetAllActiveByAccountReconcilationId(listAc.Id);
                                entitlementRecon.ForEach(i => { i.UserId = listAc.UserId; i.Save(currentUser); });
                                if (listAc.EndpointSystemId == adEndpoint.Id || listAc.EndpointSystemId == adEndpointSAP.Id)
                                {
                                    adAccountRecon.Add(listAc);
                                }
                                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, string.Format("Endpoint system Id -{0}, Account {1}  reassigned to Manager of {1}", listAc.EndpointSystemId, listAc.AccountName, sectionHeadUser.DisplayName));
                            }
                            else
                            {
                                listAc.OffboardingAccountAction = OffboardingAccountActionType.Unmapped;
                                listAc.UserId = null;
                                listAc.Save(currentUser);
                                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, $"sectionHeadUser not found for Account:{user?.DisplayName}");
                            }
                        }
                        catch (Exception ex)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"HandleServiceAccount Exception:{ex.Message} StackTrace:{ex.StackTrace} Account:{listAc?.AccountName}", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"HandleServiceAccount Exception:{ex.Message} StackTrace:{ex.StackTrace}", ex);
            }
            return adAccountRecon;
        }

        static void setRequestItemDetail(AdHocAccessRequest adhocRequest, AccountReconcilation accountReconcilation)
        {
            dynamic detail = new ExpandoObject();

            detail = ExpandoHelper.AddProperty(detail, "AccountName", accountReconcilation == null ? "" : accountReconcilation.AccountName);
            string detailString = JsonConvert.SerializeObject(detail);

            PresentationAccessRequestItem arItem = new PresentationAccessRequestItem
            {
                EndpointSystemId = adhocRequest.EndpointSystemId.Value,
                AccessRequestItemType = AccessRequestItemType.Application,
                AdditionalInfo = detailString,
            };
            adhocRequest.SetRequestedItems = new List<PresentationAccessRequestItem>() { arItem };
        }
        public static string ConstructUserName(string preferredName, string firstName, string lastName, string employeeType = "")
        {
            //return retValue;
            if (string.IsNullOrEmpty(preferredName) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName)) return string.Empty;
            preferredName = User.RemoveSpecialCharacters(preferredName.Trim(), false, false).ToLower();
            firstName = User.RemoveSpecialCharacters(firstName.Trim(), false, false).ToLower();
            lastName = User.RemoveSpecialCharacters(lastName.Trim(), false, false).ToLower();
            string userName = string.Empty;
            string PreferedNameInitials = string.Empty;
            string LastNameInitials = string.Empty;
            string FirstNameInitials = string.Empty;
            string LastNameWithoutSpace = string.Empty;
            string FirstNameWithoutSpace = string.Empty;
            string PreferredNameWithoutSpace = string.Empty;

            if (!string.IsNullOrEmpty(preferredName))
            {
                var PreferedNameInitialsList = preferredName.Split(' ').Select(y => (y.Count() > 0 ? y[0] : ' ')).ToList();
                PreferedNameInitials = string.Join(string.Empty, PreferedNameInitialsList);
                PreferedNameInitials = PreferedNameInitials.Replace(" ", string.Empty);
                PreferredNameWithoutSpace = preferredName.Replace(" ", string.Empty);
            }
            if (!string.IsNullOrEmpty(lastName))
            {
                var LastNameInitialsList = lastName.Split(' ').Select(y => (y.Count() > 0 ? y[0] : ' ')).ToList();
                LastNameInitials = string.Join(string.Empty, LastNameInitialsList);
                LastNameInitials = LastNameInitials.Replace(" ", string.Empty);
                LastNameWithoutSpace = lastName.Replace(" ", string.Empty);
            }
            if (!string.IsNullOrEmpty(firstName))
            {
                var FirstNameInitialsList = firstName.Split(' ').Select(y => (y.Count() > 0 ? y[0] : ' ')).ToList();
                FirstNameInitials = string.Join(string.Empty, FirstNameInitialsList);
                FirstNameInitials = FirstNameInitials.Replace(" ", string.Empty);
                FirstNameWithoutSpace = firstName.Replace(" ", string.Empty);
            }

            if (!string.IsNullOrEmpty(userName))
            {
                var prefixStartString = ConfigurationMasterIM.GetByConfigKey("UserNamePrefix");
                if (prefixStartString != null && !string.IsNullOrEmpty(prefixStartString.ConfigValue))
                {
                    userName = Convert.ToString(prefixStartString.ConfigValue).Trim() + userName;
                }
            }

            return userName;
        }


        public static string RemoveSpecialCharacters(string inPutStr, bool isRemoveSpace, bool isFirstName)
        {
            char[] spChars = new char[] { '~', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '{', '}', '[', ']', '|', '\\', '<', '>', '?', ',', '.', '/', '\\', '"', ':', '\'', ';' };
            //string[] tmpStr = inPutStr.Trim().Split(' ');
            //inPutStr = tmpStr != null && tmpStr.Length > 0 ? tmpStr[0] : inPutStr;


            string ethinicNames = ConfigurationMasterIM.GetByConfigKey("EthinicNamesToReplace").ConfigValue;

            var val = ConfigurationMasterIM.GetByConfigKey("ApplyOnlyonWholeWord");
            string applyOnlyonWholeWord = val == null ? "0" : val.ConfigValue;

            string[] ethinicNamesArray = !string.IsNullOrEmpty(ethinicNames) ? ethinicNames.Split(',') : null;

            if (ethinicNamesArray != null && ethinicNamesArray.Count() > 0)
            {
                foreach (string ename in ethinicNamesArray)
                {
                    string pattern = string.Format(@"{0}", ename);
                    if (!string.IsNullOrEmpty(applyOnlyonWholeWord) && applyOnlyonWholeWord.Trim() == "1")
                        pattern = string.Format(@"\b{0}\b", ename);

                    inPutStr = Regex.Replace(inPutStr, pattern, "", RegexOptions.IgnoreCase);
                }
            }

            foreach (char spChar in spChars)
            {
                inPutStr = inPutStr.Replace(spChar, ' ');
            }

            inPutStr = inPutStr.Replace(",", "");

            if (isRemoveSpace)
            {
                inPutStr.Replace(" ", string.Empty);
            }
            else
            {
                inPutStr.Replace("   ", string.Empty);
                inPutStr.Replace("  ", string.Empty);
            }

            return inPutStr.ToString().Trim();
        }

        public static Dictionary<long, string> GetCostCenters(string searchString, long companyId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new UserRepository(dataAccessTransaction).GetCostCenters(CheckNull.AsteriskToString(searchString), companyId);
            }
        }

        public static Dictionary<string, string> GetOrganisationsWithPrefix(string searchString)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new UserRepository(dataAccessTransaction).GetOrganisationsWithPrefix(CheckNull.AsteriskToString(searchString));
            }
        }

        public static Dictionary<long, string> GetNonReportingUsersOfManagerID(long managerID, string searchString)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new UserRepository(dataAccessTransaction).GetNonReportingUsersOfManagerID(managerID, CheckNull.AsteriskToString(searchString));
            }
        }

        public static Dictionary<long, string> GetUsersByGroup(string searchString, string groupName)
        {
            using (DataAccessTransaction dataAccessTransation = new DataAccessTransaction(false))
            {
                if (!string.IsNullOrEmpty(groupName))
                {
                    return new UserRepository(dataAccessTransation).GetByGroupName(CheckNull.AsteriskToString(searchString), groupName);
                }
                else
                {
                    return new UserRepository(dataAccessTransation).SearchGroup(CheckNull.AsteriskToString(searchString), groupName);
                }
            }
        }


        public static Dictionary<string, string> GetDisticntSectionFromUsers(string searchString)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new UserRepository(dataAccessTransaction).GetDisticntSectionFromUsers(CheckNull.AsteriskToString(searchString));
            }
        }

        public static User GetByIdReferenceUser(long id, bool eagerLoadAllProperties = true)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.User>(transaction).
                    Get(user => user.Id == id && user.IsActive == true, eagerLoadAllProperties).FirstOrDefault());
            }
        }

        public static List<User> GetByNOrganisationId(long organisationId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.NOrganisationId == organisationId, user => user.Groups).ToList());
            }
        }

        public static List<User> GetByCompanyId(long companyId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.CompanyId == companyId && user.IsActive).ToList());
            }
        }

        public static List<User> GetByDepartmentId(long departmentId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.DepartmentId == departmentId && user.IsActive).ToList());
            }
        }
        public static List<User> GetByBranchCode(string branchCode)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.BranchCode == branchCode && user.IsActive).ToList());
            }
        }
        public static List<User> GetByDepartmentCode(string code)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.DepartmentCode == code && user.IsActive).ToList());
            }
        }
        public static List<User> GetBySectionCode(string code)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.SectionCode == code && user.IsActive).ToList());
            }
        }
        public static List<User> GetByUnitCode(string unitCode)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.UnitCode == unitCode && user.IsActive).ToList());
            }
        }
        //public override void Save()
        //{
        //    using (DataAccessTransaction transaction = new DataAccessTransaction())
        //    {
        //        if (this.Id == 0)
        //        {
        //            this.Id = new UserRepository(transaction).Insert(ORUnMap());
        //        }
        //        else
        //        {
        //            new UserRepository(transaction).Update(ORUnMap());
        //        }

        //        transaction.Commit();
        //    }
        //}
        public override long Save(string currentUser)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                if (this.Id == 0)
                {
                    this.Id = new UserRepository(transaction).Insert(ORUnMap());
                }
                else
                {
                    new UserRepository(transaction).Update(ORUnMap());
                }

                transaction.Commit();
            }
            return 0;
        }

        public static bool IsApprover(long UserId)
        {
            var inWorkflow = WorkflowConfigurationDetail.IsApprover(UserId);

            //var inRoleOwnerApproval = NRoleGroupApproval.IsApprover(UserId);

            return inWorkflow;//|| inRoleOwnerApproval;
        }

        public static bool IsGroupMember(long UserId)
        {
            var inRoleGroup = User.GetByIdIncludeGroup(UserId);

            return inRoleGroup.Groups.Any();
        }

        public static bool IsEndpointOwner(long UserId)
        {
            var isEndpointOwner = EndPoint.IsUserEndpointOwner(UserId);

            return isEndpointOwner;
        }

        public static void SaveUser(User usr, string currentUser)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                usr.Save(transaction);
                transaction.Commit();
            }
        }

        public static User GetByUserId(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.User>(dataAccessTransaction).GetById(userId));
            }
        }

        public static List<long> GetGroupId(long actionUserId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new TaskRepository(transaction).GetGroupId(actionUserId);
            }
        }

        public static void UpdateUserADAccount(long userId, string sAMAccountName, long endpointsystemId)
        {
            try
            {
                User user = User.GetById(userId);

                if (null == user)
                    return;

                string applicationAuthMode = ConfigurationMaster.GetByConfigKey("AuthenticationMode", Product.IACM).ConfigValue;

                var defaultLoginDomain = ConfigurationMaster.GetByConfigKey("AuthenticationDomain", Product.IACM).ConfigValue;

                if (!string.IsNullOrEmpty(defaultLoginDomain) && defaultLoginDomain == endpointsystemId.ToString())
                {
                    //[11/03/22] 
                    //For non employee, if there is already inactive fulltime employee with the same user name, update its user name with  employee ID so that there won't be any duplicated user records
                    if (!user.FullTimeEmployee)
                    {
                        User fulltimeEmployeeWithSameName = User.GetByUserName(sAMAccountName);
                        if (fulltimeEmployeeWithSameName != null && fulltimeEmployeeWithSameName.FullTimeEmployee && !string.IsNullOrWhiteSpace(fulltimeEmployeeWithSameName.EmployeeId) && !fulltimeEmployeeWithSameName.IsActive)
                        {
                            fulltimeEmployeeWithSameName.UserName = fulltimeEmployeeWithSameName.EmployeeId;
                            fulltimeEmployeeWithSameName.LastModifiedBy = "ServiceUser";
                            fulltimeEmployeeWithSameName.LastModifiedDate = DateTime.Now;
                            fulltimeEmployeeWithSameName.Save("ServiceUser");

                            //update IDMS userinfo and account recon
                            var idmsEndpoint = EndPoint.GetByEndpointSystemTypeId((long)EndpointType.IDMS).ToList();
                            if (idmsEndpoint != null && idmsEndpoint.Count > 0)
                            {
                                var idmsEndpointId = idmsEndpoint[0].Id;
                                AccountReconcilation fullTimeUserRecon = AccountReconcilation.GetByDIUserId(fulltimeEmployeeWithSameName.Id, idmsEndpointId);
                                if (fullTimeUserRecon != null)
                                {
                                    fullTimeUserRecon.AccountName = fulltimeEmployeeWithSameName.UserName;
                                    fullTimeUserRecon.LastModifiedBy = "ServiceUser";
                                    fullTimeUserRecon.LastModifiedDate = DateTime.Now;
                                    fullTimeUserRecon.Save("ServiceUser");
                                }

                                if (fullTimeUserRecon == null) return;

                                var fullTimeUserEntitlementList = EntitlementReconcilation.GetByAccountReconcilationId(fullTimeUserRecon.Id, idmsEndpointId);
                                if (fullTimeUserEntitlementList != null)
                                {
                                    fullTimeUserEntitlementList.ForEach(a =>
                                    {
                                        a.AccountName = fulltimeEmployeeWithSameName.UserName;
                                        a.LastModifiedBy = "ServiceUser";
                                        a.LastModifiedDate = DateTime.Now;
                                        a.Save("ServiceUser");
                                    });
                                }

                                if (fullTimeUserRecon.EndpointUserId > 0)
                                {
                                    var iacmFullTimeUserInfo = IACMUserInfo.GetById(fullTimeUserRecon.EndpointUserId);
                                    if (iacmFullTimeUserInfo != null)
                                    {
                                        iacmFullTimeUserInfo.UserName = fulltimeEmployeeWithSameName.UserName;
                                        iacmFullTimeUserInfo.ADID = string.Empty;
                                        iacmFullTimeUserInfo.LastModifiedBy = "ServiceUser";
                                        iacmFullTimeUserInfo.LastModifiedDate = DateTime.Now;
                                        iacmFullTimeUserInfo.Save("ServiceUser");
                                    }
                                }
                            }
                        }
                    }

                    user.AuthenticationMode = (applicationAuthMode == DI.Chakra.Infrastructure.Enums.AuthenticationMode.MixedMode.ToString() || applicationAuthMode == DI.Chakra.Infrastructure.Enums.AuthenticationMode.AD.ToString()) ?
                    DI.Chakra.Infrastructure.Enums.AuthenticationMode.AD.ToString() : DI.Chakra.Infrastructure.Enums.AuthenticationMode.IACM.ToString();

                    user.IsActive = true;
                    user.UserName = sAMAccountName;
                    user.LastModifiedBy = "ServiceUser";
                    user.LastModifiedDate = DateTime.Now;
                    user.Save("ServiceUser");

                    //update IACM userinfo and account recon
                    var iacmEndpoint = EndPoint.GetByEndpointSystemTypeId((long)EndpointType.IDMS).ToList();
                    if (iacmEndpoint != null && iacmEndpoint.Count > 0)
                    {
                        var iacmEndpointId = iacmEndpoint[0].Id;
                        AccountReconcilation recon = AccountReconcilation.GetActiveByDIUserId(user.Id, iacmEndpointId);
                        if (recon != null)
                        {
                            recon.AccountName = user.UserName;
                            recon.AccountType = AccountType.User;
                            recon.Save("ServiceUser");
                        }

                        if (recon == null) return;

                        var eRecon = EntitlementReconcilation.GetByAccountReconcilationId(recon.Id, iacmEndpointId);
                        if (eRecon != null)
                        {
                            eRecon.ForEach(a =>
                            {
                                a.AccountName = user.UserName;
                                a.Save("ServiceUser");
                            });
                        }

                        if (recon.EndpointUserId > 0)
                        {
                            var iacmUserInfo = IACMUserInfo.GetById(recon.EndpointUserId);
                            if (iacmUserInfo != null)
                            {
                                iacmUserInfo.UserName = sAMAccountName;
                                iacmUserInfo.ADID = sAMAccountName;
                                iacmUserInfo.Save("ServiceUser");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static Dictionary<long, string> GetFullTimeUsers(string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetFullTimeUsers(CheckNull.AsteriskToString(searchString));
            }
        }

        public static Dictionary<long, string> GetVendorUsers(string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetVendorUsers(CheckNull.AsteriskToString(searchString));
            }
        }

        public static Dictionary<long, string> GetUsersForSupervisor(long managerId, string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetUsersForSupervisor(managerId, CheckNull.AsteriskToString(searchString));
            }
        }

        public static List<string> GetRequesteeInfo(long userId, long endpointId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                List<string> info = new List<string>();
                try
                {
                    //exclusionlist
                    var exclusionlist = UserExclusion.GetAllUserExclusionByUserIdWithDate(userId);
                    if (exclusionlist != null && exclusionlist.Count > 0)
                    {
                        info.AddRange(exclusionlist.Select(a => string.Format("Excluded from event {0} till {1}", a.EventType, a.EffectiveDate?.ToString("dd-MM-yyyy"))));
                    }
                    //upcoming events
                    var eventlist = EmployeeEventTrigger.GetAllUserEventsToRun(userId);
                    if (eventlist != null && eventlist.Count > 0)
                    {
                        info.AddRange(eventlist.Select(a => string.Format("The user's event type will take place on {1}:{0}", a.EventType, a.EffectiveDate.ToString("dd-MM-yyyy"))));
                    }
                    return info;
                }
                catch (Exception ex)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while getting requestee info for task", ex);
                }
                return info;
            }
        }

        public static List<User> GetExclusionUsersByEventType(EmployeeEvents eventType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new UserRepository(dataAccessTransaction).GetExclusionUsersByEventType(eventType).ToList());
            }
        }

        public static void UpdateUserPAInfo()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new UserRepository(dataAccessTransaction).UpdateUserPAInfo();
            }
        }

        public static void UpdateROCOForReferenceUsers(long referenceUserId, string ro, string co)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new UserRepository(dataAccessTransaction).UpdateROCOForReferenceUsers(referenceUserId, ro, co);
            }
        }

        public static void DeriveCOEmployeeId(long DIUserId)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    var val = new UserRepository(dataAccessTransaction).DeriveCOEmployeeId(DIUserId);
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"Error while DeriveCOEmployeeId for DIUserId - {DIUserId}", ex);
            }
        }
        public static List<long> GetReferenceUsersIds(long userId)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                {
                    return new UserRepository(dataAccessTransaction).GetReferenceUsersIds(userId);
                }
            }
            catch
            {
                throw;
            }
        }

        public static List<string> GetReferenceUsers(long userId)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                {
                    return new UserRepository(dataAccessTransaction).GetReferenceUsers(userId);
                }
            }
            catch
            {
                throw;
            }
        }
        public static PersonnelArea GetPersonnelAreaById(long id)
        {
            try
            {
                return PersonnelArea.GetById(id);
            }
            catch
            {
                throw;
            }
        }
        public static Company GetCompanyById(long id)
        {
            try
            {
                return Company.GetById(id);
            }
            catch
            {
                throw;
            }
        }
        public static Dictionary<long, string> GetCompanies(string searchString)
        {
            Dictionary<long, string> companies = new Dictionary<long, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                searchString = searchString?.Replace("*", string.Empty);
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    companies = new RepositoryBase<EntityModel.Company>(dataAccessTransaction).Get(item => item.Code.ToLower() != "all" && item.Text.ToLower().Contains(searchString.ToLower())).Select(x => new { Key = x.Id, Value = x.Code + " - " + x.Text })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
                else
                {
                    companies = new RepositoryBase<EntityModel.Company>(dataAccessTransaction).Get(item => item.Code.ToLower() != "all").Select(x => new { Key = x.Id, Value = x.Code + " - " + x.Text })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            return companies;
        }
        public static PersonnelSubArea GetPersonnelSubAreaById(long id)
        {
            try
            {
                return PersonnelSubArea.GetById(id);
            }
            catch
            {
                throw;
            }
        }
        public static NOrganisation GetOrganisationByCode(string code)
        {
            try
            {
                return NOrganisation.GetByOrgCode(code);
            }
            catch
            {
                throw;
            }
        }
        public static List<NOrganisation> GetAllOrganisations()
        {
            try
            {
                return NOrganisation.GetAllOrganisations();
            }
            catch
            {
                throw;
            }
        }


        public static Dictionary<string, string> GetGroupsById(long Id, AdHocActionType actionType, string searchString)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new UserRepository(dataAccessTransaction).GetGroupsById(Id, actionType, searchString);
            }

        }

        public static Dictionary<string, string> GetRolesById(long Id, AdHocActionType actionType, string searchString)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new UserRepository(dataAccessTransaction).GetRolesById(Id, actionType, searchString);
            }

        }
        public static List<User> GetOneKeyTokenEmployees()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.OneKeyTokenId != null && usr.OneKeyTokenId != "").ToList());
            }
        }

        public static List<User> GetExpiringNonEmployees()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                DateTime today = DateTime.Today;
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(user => !user.FullTimeEmployee && user.TerminationDate.Value >= today && user.IsActive).ToList());
            }

        }
        public static List<User> GetUsersToUpdateEmailIdInHR()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.NeedEmailUpdateToHR).ToList());
            }
        }
        public static List<User> GetUsersToUpdateTelephoneNoInHR()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.NeedTelephoneUpdateToHR && user.FullTimeEmployee).ToList());
            }
        }
        public static List<User> GetUsersToUpdateFLINSLoginIdInHR()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.NeedFLINSLoginIdUpdateToHR).ToList());
            }
        }

        public static Dictionary<string, string> GetOrgCompanies(string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetOrgCompanies(searchString);
            }
        }

        public static Dictionary<string, string> GetOrgDepartments(string searchString, List<string> CompanyNameFilter)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetOrgDepartments(searchString, CompanyNameFilter);
            }
        }

        public static Dictionary<string, string> GetOrgDivisions(string searchString, List<string> CompanyNameFilter, List<string> DepartNameFilter)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetOrgDivisions(searchString, CompanyNameFilter, DepartNameFilter);
            }
        }

        public static Dictionary<string, string> GetOrgBranches(string searchString, List<string> CompanyNameFilter, List<string> DepartNameFilter)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetOrgBranches(searchString, CompanyNameFilter, DepartNameFilter);
            }
        }

        public static Dictionary<string, string> GetOrgSections(string searchString, List<string> CompanyNameFilter, List<string> DepartNameFilter, List<string> BranchNameFilter)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetOrgSections(searchString, CompanyNameFilter, DepartNameFilter, BranchNameFilter);
            }
        }

        public static Dictionary<string, string> GetOrgUnits(string searchString, List<string> CompanyNameFilter, List<string> DepartNameFilter, List<string> BranchNameFilter, List<string> SectioNameFilter)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetOrgUnits(searchString, CompanyNameFilter, DepartNameFilter,
                    BranchNameFilter, SectioNameFilter);
            }
        }

        public static Dictionary<long, string> GetReportingofSubordinate(long UserID, string searchString, List<string> CompanyNameFilter, List<string> DepartNameFilter, List<string> BranchNameFilter, List<string> SectioNameFilter, List<string> UnitName)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                Dictionary<long, string> returnvalue = new UserRepository(transaction).GetReportingofSubordinate(UserID, searchString, CompanyNameFilter, DepartNameFilter, BranchNameFilter, SectioNameFilter, UnitName, MasterConfigHelper.ExcludedUsers);

                return returnvalue;
            }
        }

        public static Dictionary<long, string> GetAllEmployeesUnderRO(string roEmployeeId, string searchString)
        {
            try
            {
                string query = $@"WITH Emp_CTE AS (
                                SELECT Id, EmployeeID, ROEmployeeId, DisplayName + ' ['+UserName+']' DisplayName, 1 as Level
                                FROM DI_M_User
                                WHERE  IsActive = 1 AND  ROEmployeeId = '{roEmployeeId}'
                                UNION ALL
                                SELECT e.Id, e.EmployeeID, e.ROEmployeeId, e.DisplayName + ' ['+UserName+']' DisplayName, ecte.Level + 1 as Level
                                FROM DI_M_User e
                                INNER JOIN Emp_CTE ecte ON ecte.EmployeeID = e.ROEmployeeId WHERE  e.IsActive = 1
                                )
                                SELECT * FROM Emp_CTE T where T.DisplayName like '%{searchString}%' ORDER BY T.DisplayName ASC ";

                Dictionary<long, string> result = new Dictionary<long, string>();

                var dtResult = GetDataTableByQuery(query);
                if (dtResult != null && dtResult.Rows.Count > 0)
                {
                    foreach (DataRow row in dtResult.Rows)
                    {
                        result.Add(Convert.ToInt64(row["Id"]), Convert.ToString(row["DisplayName"]));
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"Error while GetAllEmployeesUnderRO", ex);
                throw;
            }
        }

        public static string[] GetEmailIdsByGroupId(long groupId)
        {
            List<string> lstEmailIds = new List<string>();
            //using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                string query = $"select U.EmailId from UserGroup UG inner join di_M_User U on UG.User_UserId = U.Id where Group_GroupId = {groupId} and isnull(U.EmailId,'') != ''";
                var dtResult = GetDataTableByQuery(query);
                if (dtResult != null && dtResult.Rows.Count > 0)
                {
                    foreach (DataRow row in dtResult.Rows)
                    {
                        lstEmailIds.Add(Convert.ToString(row[0]));
                    }
                }
            }
            return lstEmailIds.ToArray();
        }
        public static string[] GetUsetEmailIdsByGroupId(long groupId)
        {
            List<string> lstEmailIds = new List<string>();
            //using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                string query = $"select U.EmailId from UserGroup UG inner join di_M_User U on UG.User_UserId = U.Id where Group_GroupId = {groupId} and isnull(U.EmailId,'') != ''";
                var dtResult = GetDataTableByQuery(query);
                if (dtResult != null && dtResult.Rows.Count > 0)
                {
                    foreach (DataRow row in dtResult.Rows)
                    {
                        lstEmailIds.Add(Convert.ToString(row[0]));
                    }
                }
            }
            return lstEmailIds.ToArray();
        }
        public static string GetEmailIdsByGroupName(string groupName)
        {
            List<string> lstEmailIds = new List<string>();
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                string query = $"SELECT U.EmailId FROM UserGroup UG INNER JOIN di_M_User U ON UG.User_UserId = U.Id INNER JOIN DI_M_Group grp ON ug.Group_GroupId =grp.Id WHERE grp.GroupName='{groupName}' AND ISNULL(U.EmailId,'') != ''";
                var dtResult = GetDataTableByQuery(query);
                if (dtResult != null && dtResult.Rows.Count > 0)
                {
                    foreach (DataRow row in dtResult.Rows)
                    {
                        lstEmailIds.Add(Convert.ToString(row[0]));
                    }
                }
            }
            return string.Join(",", lstEmailIds);
        }

        public static void AssignCoordinatorRolesIfRequiredAndDefaultRolesIfNotExists()
        {
            try
            {
                using (DataAccessTransaction transaction = new DataAccessTransaction())
                {
                    new UserRepository(transaction).AssignCoordinatorRolesIfRequiredAndDefaultRolesIfNotExists();
                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Dictionary<long, string> GetUsersByGroupId(string searchString, long groupId)
        {
            var excludeRoleIds = MasterConfigHelper.IACMCoordinatorRoles;

            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetUsersByGroupId(searchString, groupId, excludeRoleIds);
            }
        }

        public static void PerformLogoutCleanup(long userId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                new UserRepository(transaction).PerformLogoutCleanup(userId);
                transaction.Commit();
            }
        }

        public static List<string> GetActiveEmployeesPerno()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetAllEmployeePerNo().ToList();
            }
        }

        public OrgChangeTrack GetOrgChangeTrack(EmployeeEvents employeeEvent, string EventReason, string EventCode, DateTime EffectiveDate, DateTime? EndDate = null, int? eventSequence = null, string retainAccessForTransfer = "")
        {
            var user = new OrgChangeTrack(employeeEvent, EventReason, EventCode, EffectiveDate, EndDate, eventSequence, retainAccessForTransfer);
            user.EmployeeId = this.EmployeeId;
            user.CompanyId = CompanyId;
            user.DepartmentId = DepartmentId;
            user.DivisionId = DivisionId;
            user.CompanyCode = CompanyCode;
            user.DepartmentCode = DepartmentCode;
            user.DivisionCode = DivisionCode;
            user.SectionCode = SectionCode;
            user.CompanyName = CompanyName;
            user.DepartmentName = DepartmentName;
            user.DivisionName = DivisionName;
            user.SectionName = SectionName;
            user.OrgCompany = OrgCompany;
            user.OrgDepartment = OrgDepartment;
            user.OrgDivision = OrgDivision;
            user.OrgBranch = OrgBranch;
            user.OrgSection = OrgSection;
            user.OrgUnit = OrgUnit;
            user.BranchCode = BranchCode;
            user.UnitCode = UnitCode;
            user.OrganisationCode = OrganisationCode;
            user.CostCenter = CostCenter;
            user.CostCenterText = CostCenterText;
            user.PositionCode = PositionCode;
            user.PositionName = PositionName;
            user.IsActive = IsActive;
            user.EEGroup = EEGroup;
            user.EmployeeStatus = EmployeeStatus;
            user.IsExecutiveEmployee = IsExecutiveEmployee;
            user.ROEmployeeId = this.ROEmployeeId;
            user.JoinDate = JoinDate;
            user.RehireDate = RehireDate;
            user.TerminationDate = TerminationDate;
            user.ROEmployeeId = ROEmployeeId;
            user.FirstName = FirstName;
            user.MiddleName = MiddleName;
            user.LastName = LastName;
            user.DisplayName = DisplayName;
            user.KnownAs = KnownAs;
            user.EmailId = EmailId;
            user.HomephoneNumber = HomephoneNumber;
            user.TelephoneNumber = TelephoneNumber;
            user.MobileNumber = MobileNumber;
            user.Fax = Fax;
            user.PostalCode = PostalCode;
            user.PostalAddress = PostalAddress;
            user.Ethinicity = Ethinicity;
            user.Country = Country;
            user.EmployeeType = EmployeeType;
            return user;
        }
        public static string SendOTP(long userId)
        {
            var usr = User.GetById(userId);
            if (string.IsNullOrWhiteSpace(usr.MobileNumber))
            {
                return "User Mobile Number does not exists in the system. Please contact IACM Administrator.";
            }
            string message = $"Please enter the following OTP to reset your Active Directory Password ";
            return message;
        }
        public static List<User> GetAllUserUnderReportingOfficer(Nullable<long> roUserId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new UserRepository(tran).GetAllUserUnderReportingOfficer(roUserId).ToList());
            }
        }
        public static List<User> GetByCompanyCode(string code)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new UserRepository(tran).GetUserByCompanyCode(code).ToList());
            }
        }
        public static List<User> GetAllActiveUser()
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(tran).Get(user => user.IsActive).ToList());
            }
        }
        public static Dictionary<long, string> GetCustomReportUserBySearchString(string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetCustomReportUserBySearchString(CheckNull.AsteriskToString(searchString));
            }
        }
        public static List<User> GetAllRO()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new UserRepository(transaction).GetAllRO());
            }
        }
        public static Dictionary<long, string> GetAllActiveRO(string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetAllActiveRO(CheckNull.AsteriskToString(searchString));
            }
        }
        public static User GetReportingOfficerByUser(long id)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new UserRepository(transaction).GetReportingOfficerByUser(id));
            }
        }

        public static string GetByUserActive(string userName)
        {
            var user = GetByUserName(userName);

            if (user != null)
            {
                if (user.IsActive == false)
                {
                    return "User is not active";
                }
                else
                {
                    var adEndpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    var adUserInfo = ADSUserInfo.GetAccountDisabledByEndpoint(user.Id, adEndpoint.Id);
                    if (adUserInfo == null)
                    {
                        return "AD Does not exist for the user";

                    }

                    if (adUserInfo.IsAccountDisabled == true)
                        return "AD Account is disabled for the User";

                    var office365Endpoint = EndPoint.GetByApplicationType(ApplicationType.Office365);
                    var o365User = Office365UserInfo.GetPrimaryByDIUserId(user.Id, office365Endpoint.Id);
                    if (o365User == null)
                    {
                        return "Office365 User Account does not exists for the User";
                    }

                }
            }

            string message = "";
            return message;
        }

        public static List<long> GetIACMUserForAttestation()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                var excludeList = MasterConfigHelper.IDMSAttestationRoleToExclude;
                return new UserRepository(transaction).GetIACMUserForAttestation(excludeList);
            }
        }
        public static string CheckUserVPNToken(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.User>(dataAccessTransaction).GetById(userId)?.OneKeyTokenId;
            }
        }
        public static string IsSameTokenExists(string tokenId)
        {
            var users = string.Empty;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var objs = new RepositoryBase<EntityModel.User>(dataAccessTransaction).Get(i => i.OneKeyTokenId == tokenId);
                if (objs != null && objs.Any())
                {
                    users = string.Join(",", objs.Select(i => i.DisplayName));
                }
            }
            return users;
        }
        public static string GetUserMobileNumber(string employeeId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new UserRepository(transaction).GetUserMobileNumber(employeeId);
            }
        }
        public static List<User> GetActiveFullTimeEmployees()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.User>(transaction).Get(usr => usr.FullTimeEmployee && usr.IsActive).ToList());
            }
        }
        public static User IsEmailIdAlreadyExist(long userId, string emailId)
        {
            User userObj = null;
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                if (!string.IsNullOrWhiteSpace(emailId))
                {
                    var domain = MasterConfigHelper.ADUPNDomainName.Trim();
                    if (!string.IsNullOrWhiteSpace(domain) && !emailId.EndsWith(domain, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return userObj;
                    }
                    var ep = EndPoint.GetByApplicationType(ApplicationType.Office365);
                    if (ep != null)
                    {
                        var accountRecon = AccountReconcilation.GetByAccountNameByAccountType(emailId, ep.Id, AccountType.Shared);
                        if (accountRecon != null)
                        {
                            return userObj;
                        }
                    }

                    if (userId > 0)
                    {
                        userObj = ORMap(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.Id != userId && user.EmailId == emailId).FirstOrDefault());

                        if (userObj != null)
                        {
                            var usr = User.GetById(userId);
                            if (usr != null && userObj != null && userObj.DisplayName == usr.DisplayName)
                            {
                                userObj = null;
                            }
                        }
                    }
                    else
                    {
                        userObj = ORMap(new RepositoryBase<EntityModel.User>(transaction).Get(user => user.EmailId == emailId).FirstOrDefault());
                    }
                }
                return userObj;
            }
        }
        public static List<long> GetOneCardUserForAttestation(long endPointId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                var usr = new List<long>();
                var field = NProvisioningField.GetByEndpointSystemIdAndName(endPointId, "DefaultAccessLevels");
                if (field != null && !string.IsNullOrWhiteSpace(field.Value))
                {
                    var excludeList = field.Value.SplitBySeparator(",", StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (excludeList != null && excludeList.Count > 0)
                    {
                        usr = new UserRepository(transaction).GetOneCardUserForAttestation(excludeList);
                    }
                }
                return usr;
            }
        }
    }
}


