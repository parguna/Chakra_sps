using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using DI.Chakra.Logger;


namespace DI.Chakra.Repository
{
    public class UserRepository : RepositoryBase<User>
    {
        public UserRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<User> GetSectionHeads()
        {
            return (from usr1 in CurrentContext.User
                    join sectionHead in CurrentContext.NOrganisationHeads on usr1.Id equals sectionHead.HeadId
                    select usr1).ToList();
        }

        public List<long> GetSameOUUserIDs(long diUserId)
        {
            return CurrentContext.User.Join(CurrentContext.User, otheruser => otheruser.OrganisationCode, usrid => usrid.OrganisationCode, (otheruser, usrid) => new { UserList = otheruser, currentUser = usrid })
                                        .Where(bothuser => bothuser.currentUser.Id == diUserId).Select(bothuser => bothuser.UserList.Id).ToList();
        }
        public User GetReportingOfficer(long UserId)
        {
            return CurrentContext.User.Join(CurrentContext.User,
                                            roUser => roUser.EmployeeId,
                                            userRO => userRO.ROEmployeeId,
                                            (roUser, userRO) => new { ReportingOfficerUser = roUser, UserEmployee = userRO })
                                            .Where(both => both.UserEmployee.Id == UserId)
                                            .Select(both => both.ReportingOfficerUser).FirstOrDefault();
        }



        public User GetCounterSigningOfficer(long UserId)
        {
            return CurrentContext.User.Join(CurrentContext.User,
                                            coUser => coUser.EmployeeId,
                                            userCO => userCO.COEmployeeId,
                                            (coUser, userCO) => new { COUser = coUser, UserEmployee = userCO })
                                            .Where(both => both.UserEmployee.Id == UserId)
                                            .Select(both => both.COUser).FirstOrDefault();
        }


        public void DeleteRole(long userid, long roleId)
        {
            string cmd = "DELETE FROM [dbo].[UserRole] WHERE User_UserId = " + userid + " AND  Role_RoleId = " + roleId;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        // dueCutoff comes from EmployeeEventTrigger.IsEventProcessWindowOpen; callers skip this query while the window is closed.
        public List<User> GetOnBoardingUsers(bool forAdminCheck, DateTime dueCutoff)
        {
            return (from item in CurrentContext.User
                    join trig in CurrentContext.EmployeeEventTriggers
                        on item.Id equals trig.UserId
                    where trig.IsAdminCheckRequired == forAdminCheck && trig.IsRequestCreated == false && trig.IsActive &&
                          (trig.EventType == EmployeeEvents.NewJoinee) && trig.EffectiveDate < dueCutoff
                    select item).ToList();
        }

        public List<User> GetEmployeeConversionUsers(bool forAdminCheck)
        {
            return (from item in CurrentContext.User
                    join trig in CurrentContext.EmployeeEventTriggers
                        on item.Id equals trig.UserId
                    where trig.IsAdminCheckRequired == forAdminCheck && trig.IsRequestCreated == false && trig.IsActive &&
                          (trig.EventType == EmployeeEvents.EmployeeConversion) && trig.EffectiveDate <= DateTime.Now
                    select item).ToList();
        }
        public List<User> GetRehireUsers(bool forAdminCheck)
        {
            return (from item in CurrentContext.User
                    join trig in CurrentContext.EmployeeEventTriggers
                        on item.Id equals trig.UserId
                    where trig.IsAdminCheckRequired == forAdminCheck && trig.IsRequestCreated == false && trig.IsActive &&
                          trig.EventType == EmployeeEvents.Rehire && trig.EffectiveDate <= DateTime.Now
                    select item).ToList();
        }

        public List<User> GetBackkDatedExceptions(int onboardBackdatedWindowperiod, DateTime? fromDate, DateTime? toDate)
        {
            List<User> users = new List<User>();
            List<User> ExceptionUsers = new List<User>();
            if (fromDate.HasValue && toDate.HasValue)
            {
                users = (from usr in CurrentContext.User
                         where usr.JoinDate != null && usr.CreatedDate != null
                && usr.CreatedDate >= fromDate && usr.CreatedDate <= toDate && DbFunctions.DiffDays(usr.JoinDate, usr.CreatedDate) > onboardBackdatedWindowperiod
                         select usr).ToList();
            }
            else
            {
                users = (from usr in CurrentContext.User
                         where usr.JoinDate != null && usr.CreatedDate != null
                         && usr.CreatedDate > usr.JoinDate.Value
                         && DbFunctions.DiffDays(usr.JoinDate, usr.CreatedDate) > onboardBackdatedWindowperiod
                         select usr).ToList();
            }

            return users;
        }

        public List<User> GetAllActiveUserWithRoleAndGroup()
        {
            return (from user in CurrentContext.User
                    where user.IsActive == true
                    select user).ToList();
        }

        public List<User> GetUnlockedAccountsByDIUserId(long userId)
        {
            return (from user in CurrentContext.User.Include("Roles")
                    join accountRecon in CurrentContext.AccountReconcilations on user.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == userId && accountRecon.IsLocked == false && user.EndPointSystemId == accountRecon.EndpointSystemId
                    select user).ToList();
        }

        // dueCutoff comes from EmployeeEventTrigger.IsEventProcessWindowOpen; callers skip this query while the window is closed.
        public List<User> GetOffBoardingUsers(DateTime dueCutoff)
        {
            return (from item in CurrentContext.User
                    join trig in CurrentContext.EmployeeEventTriggers
                        on item.Id equals trig.UserId
                    where trig.IsRequestCreated == false && trig.IsActive &&
                          trig.EventType == EmployeeEvents.Resignation &&
                          trig.EffectiveDate < dueCutoff
                    select item).ToList();
        }

        public List<User> GetNotifiableUsers(long taskId, long requestId)
        {
            return ((from tsk in CurrentContext.Tasks
                     join acr in CurrentContext.AccessRequest.Include("Requester")
                     on tsk.AccessRequestId equals acr.Id
                     //where tsk.Id ==taskId 
                     select acr.Requester).Union(from tsk in CurrentContext.Tasks
                                                 join usr in CurrentContext.User
                                                 on tsk.ActionUserId equals usr.Id
                                                 //where (tsk.AccessRequestId == requestId &&
                                                 //tsk.IsActive==true && tsk.IsCompleted==true)
                                                 select usr))
                    .Distinct().ToList();
        }

        public string GetEmployeeId(string searchString)
        {
            Dictionary<long, string> data = CurrentContext.User.Where(lq => lq.EmployeeId.StartsWith(searchString))
                                   .Select(lq => new { Key = lq.Id, Value = lq.EmployeeId }).OrderByDescending(lq => lq.Key).ToDictionary(o => o.Key, o => o.Value);
            foreach (var items in data)
            {
                return items.Value;
            }
            return null;
        }

        public User GetCompanyDetails(string prefix)
        {
            User uList = new User();
            int userid = Convert.ToInt32(prefix);


            var res1 = from usr in CurrentContext.User
                       join org in CurrentContext.NOrganisations on usr.NOrganisationId equals org.Id into detail
                       join businessRole in CurrentContext.NDesignations on usr.NBusinessRoleId equals businessRole.Id into role
                       from org in detail.DefaultIfEmpty()
                       from businessRole in role.DefaultIfEmpty()
                       where usr.Id.Equals(userid)
                       select new
                       {
                           usr.IdentificationNo,
                           usr.EmployeeId,
                           usr.FirstName,
                           usr.MiddleName,
                           usr.LastName,
                           usr.JoinDate,
                           usr.TerminationDate,
                           usr.EmployeeType,
                           usr.Reserved1,
                           usr.TelephoneNumber,
                           usr.MobileNumber,
                           usr.Country,
                           usr.Ethinicity,
                           usr.DisplayName,
                           org,
                           org.Name,
                           usr.NOrganisationId,
                           usr.NBusinessRoleId,
                           businessRole,
                           usr.UserName,
                           usr.Password,
                           usr.CompanyName,
                           usr.CompanyId,
                           usr.CompanyCode,
                           usr.CostCenter,
                           usr.DepartmentName,
                           usr.DepartmentId,
                           usr.DepartmentCode,
                           usr.SectionCode,
                           usr.DivisionName,
                           usr.DivisionId,
                           usr.DivisionCode,
                           usr.PositionName,
                           usr.PositionCode,
                           usr.Supervisor,
                           usr.ManagerId,
                           usr.OrgUnit,
                           usr.SectionName,
                           usr.OrgCompany,
                           usr.OrgDepartment,
                           usr.OrgBranch,
                           usr.OrgDivision,
                           usr.OrgSection,
                           usr.OrganisationCode
                       };
            var record1 = res1.FirstOrDefault();
            if (record1 != null)
            {
                uList.IdentificationNo = record1.IdentificationNo;
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
                uList.MobileNumber = record1.MobileNumber;
                uList.Country = record1.Country;
                uList.Ethinicity = record1.Ethinicity;
                uList.DisplayName = record1.DisplayName;
                uList.NOrganisation = record1.org;
                uList.NOrganisationId = record1.NOrganisationId;
                uList.NBusinessRoleId = record1.NBusinessRoleId;
                uList.NBusinessRole = record1.businessRole;
                uList.UserName = record1.UserName;
                uList.CompanyName = record1.CompanyName;
                uList.CompanyCode = record1.CompanyCode;
                uList.CostCenter = record1.CostCenter;
                uList.DepartmentName = record1.DepartmentName;
                uList.DepartmentCode = record1.DepartmentCode;
                uList.SectionCode = record1.SectionCode;
                uList.DivisionName = record1.DivisionName;
                uList.DivisionCode = record1.DivisionCode;
                uList.PositionName = record1.PositionName;
                uList.PositionCode = record1.PositionCode;
                uList.Supervisor = record1.Supervisor;
                uList.OrgUnit = record1.OrgUnit;
                uList.SectionName = record1.SectionName;
                uList.OrgCompany = record1.OrgCompany;
                uList.OrgDepartment = record1.OrgDepartment;
                uList.OrgBranch = record1.OrgBranch;
                uList.OrgDivision = record1.OrgDivision;
                uList.OrgSection = record1.OrgSection;
                uList.OrganisationCode = record1.OrganisationCode;
                var res4 = from des in CurrentContext.NDesignations
                           where des.Id == record1.NBusinessRoleId
                           select new { BusinessRoleId = des.Id };
                var record4 = res4.FirstOrDefault();
                if (record4 != null)
                    uList.NBusinessRoleId = record4.BusinessRoleId;
            }
            return uList;
        }
        public Dictionary<long, string> GetReportingManager(string prefix)
        {
            var mList = (from usr in CurrentContext.User where usr.EmployeeType == EmployeeType.Permanent && usr.IsActive == true && (usr.DisplayName.Contains(prefix) || usr.UserName.Contains(prefix)) select new { usr.DisplayName, UserId = usr.Id }).Distinct();
            return mList.ToDictionary(c => c.UserId, c => c.DisplayName);
        }

        public Dictionary<string, string> GetPosition(string prefix)
        {
            var pList = CurrentContext.NDesignations.ToDictionary(c => c.Id + "-" + c.Code, c => c.Description);

            return pList;
        }

        public Dictionary<string, string> GetReferenceUser(string searchString)
        {
            var uList = CurrentContext.User.Where(usr => usr.IsActive == true && (string.IsNullOrEmpty(searchString) ? true : (usr.DisplayName.StartsWith(searchString) || usr.DisplayName.Contains(searchString))) && usr.JoinDate <= DateTime.Now).
                Take(200).
                Select(a => new { a.DisplayName, UserId = a.Id, UserName = a.UserName }).Distinct();
            return uList.ToDictionary(c => c.UserId.ToString(), c => c.DisplayName + " [" + c.UserName + "]");
        }
        public Dictionary<string, string> GetReferencePermanentUser(string searchString)
        {
            var uList = CurrentContext.User.Where(usr => usr.FullTimeEmployee && usr.IsActive == true && (string.IsNullOrEmpty(searchString) ? true : (usr.DisplayName.StartsWith(searchString) || usr.DisplayName.Contains(searchString))) && usr.JoinDate <= DateTime.Now).
                Take(200).
                Select(a => new { a.DisplayName, UserId = a.Id, UserName = a.UserName }).Distinct();
            return uList.ToDictionary(c => c.UserId.ToString(), c => c.DisplayName + " [" + c.UserName + "]");
        }
        public Dictionary<long, string> GetNonReportersOfManager(long managerID, string searchString)
        {
            return CurrentContext.User.Where(item => (string.IsNullOrEmpty(searchString) ? true : (item.DisplayName.StartsWith(searchString) || item.DisplayName.Contains(searchString))) && item.TerminationDate >= DateTime.Now && item.IsActive == true)
                       .Select(x => new { Key = x.Id, Value = x.DisplayName + " [" + x.UserName + "]" })
                       .Distinct().ToDictionary(o => o.Key, o => o.Value);
        }

        public Dictionary<long, string> GetBySearchString(string searchString, bool hidden, bool considerJoinDate = false)
        {
            if (hidden)
            {
                var Users = CurrentContext.User.Where(usr => !string.IsNullOrEmpty(usr.DisplayName) && usr.IsActive == hidden &&
                                                    (usr.DisplayName.StartsWith(searchString) || usr.DisplayName.Contains(searchString)));
                if (considerJoinDate)
                {
                    var priorOnBoardDays = this.CurrentContext.ConfigurationMastersIACM
                                                            .Where(conf => conf.ConfigKey == "OnBoardDaysBeforeJoining")
                                                            .Select(conf => conf.ConfigValue).FirstOrDefault();
                    DateTime joinDate = DateTime.Today.AddDays(Convert.ToInt32(priorOnBoardDays));
                    Users = Users.Where(usr => usr.JoinDate <= joinDate &&
                                                      (usr.TerminationDate == null || (usr.TerminationDate != null && usr.TerminationDate > DateTime.Now)));
                }
                return Users.Take(100).
                Select(x => new { key = x.Id, value = x.DisplayName + " [" + x.UserName + "]" }).
                ToDictionary(o => o.key, o => o.value);
            }
            else
            {
                return CurrentContext.User.Where(usr => !string.IsNullOrEmpty(usr.DisplayName) &&
                                                                  (usr.DisplayName.StartsWith(searchString) || usr.DisplayName.Contains(searchString))).Take(100).
                                                                  Select(x => new { key = x.Id, value = x.DisplayName + " [" + x.UserName + "]" }).
                                                                  ToDictionary(o => o.key, o => o.value);
            }
        }

        public Dictionary<long, string> GetROUsersBySearchString(string searchString)
        {
            var roUsers = CurrentContext.User.Where(usr => !string.IsNullOrEmpty(usr.ROEmployeeId)).Select(p => p.ROEmployeeId).Distinct().ToList();
            return CurrentContext.User.Where(usr => !string.IsNullOrEmpty(usr.DisplayName) && roUsers.Contains(usr.EmployeeId) &&
                                                              (usr.DisplayName.Contains(searchString) || usr.UserName.Contains(searchString))).Take(100).
                                                              Select(x => new { key = x.Id, value = x.DisplayName + " [" + x.UserName + "]" }).
                                                              ToDictionary(o => o.key, o => o.value);
        }

        public Dictionary<long, string> GetActiveAndNonTerminaters(string searchString)
        {
            return CurrentContext.User
                       .Where(usr => usr.IsActive == true && usr.JoinDate <= DateTime.Now &&
                           (usr.TerminationDate == null || (usr.TerminationDate != null && usr.TerminationDate > DateTime.Now))
                           && (usr.DisplayName.Contains(searchString) || usr.UserName.Contains(searchString))).Take(100)
                       .Select(x => new { key = x.Id, value = x.DisplayName + " [" + x.UserName + "]" })
                       .ToDictionary(o => o.key, o => o.value);
        }

        public Dictionary<long, string> GetReportingUsersOfManagerAndAdmin(bool isAdmin, long managerId, string searchString)
        {
            if (isAdmin)
                return CurrentContext.User.Where(usr => (usr.DisplayName.Contains(searchString) || usr.UserName.Contains(searchString)))
                .Select(x => new { Key = x.Id, Value = x.DisplayName + " [" + x.UserName + "]" })
                .Distinct().ToDictionary(o => o.Key, o => o.Value);
            else
                return CurrentContext.User.Where(item => (item.DisplayName.Contains(searchString) || item.UserName.Contains(searchString)))
                 .Select(x => new { Key = x.Id, Value = x.DisplayName + " [" + x.UserName + "]" })
                 .Distinct().ToDictionary(o => o.Key, o => o.Value);
        }

        public List<User> GetAssigneeInfo(long? userId)
        {
            return (from user in CurrentContext.User.Where(leUser => leUser.Id == userId)
                    select user).ToList();
            //select new
            //{
            //    DisplayName = user.DisplayName,
            //    MobileNumber = user.MobileNumber,
            //    EmailId = user.EmailId

            //}).ToList().Select(leUser => new User
            //{

            //    DisplayName = leUser.DisplayName,
            //    MobileNumber = leUser.MobileNumber,
            //    EmailId = leUser.EmailId

            //}).ToList();

        }


        public string GetQuery()
        {
            return string.Format("DELETE {0}", "DI_M_User");
        }

        public void DeleteByUserRole(int roleId)
        {
            string query = "DELETE FROM UserRole where Role_RoleId=" + roleId + "";
            this.CurrentContext.Database.ExecuteSqlCommand(query);
            CurrentContext.SaveChanges();
        }

        public void UpdateMasterLatest(string strQuery)
        {

            IACMDbOperator iacmOperator;
            QueryExecutor executor;
            int noAffected = -1;

            iacmOperator = new IACMDbOperator(strQuery, CommandType.Text, null);
            executor = new QueryExecutor(iacmOperator);
            noAffected = executor.ExecuteQuery();
            iacmOperator.Close();

        }

        public void UpdateManagerForUsers()
        {
            string query = "UPDATE usr SET ManagerId = usr1.Id FROM DI_M_User usr JOIN DI_M_User usr1 ON usr.Supervisor = usr1.EmployeeId";
            this.CurrentContext.Database.ExecuteSqlCommand(query);
        }

        public void InsertUserRole(long userID, long roleId)
        {
            try
            {
                string query = $"if not exists(select 1 from  [dbo].[UserRole] where Role_RoleId = {roleId} and User_UserId = {userID}) " +
                    $"INSERT INTO [dbo].[UserRole] (Role_RoleId,User_UserId) VALUES({roleId},{userID})";
                CurrentContext.Database.ExecuteSqlCommand(query);
                CurrentContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            //m_iacmOperator = new IACMDbOperator(query, CommandType.Text, null);

            //m_executor = new QueryExecutor(m_iacmOperator);

            //m_executor.ExecuteQuery();

        }
        public override void Update(User user)
        {
            //TODO:Need to verify this with Murugan

            //User dbRole = this.GetById(user.Id, r => r.Roles);

            //List<Role> deleteRoles = dbRole.Roles.Except(user.Roles, new IdBaseEqualityComparer<Role>()).ToList();

            //List<Role> addedRoles = user.Roles.Except(dbRole.Roles, new IdBaseEqualityComparer<Role>()).ToList();

            //deleteRoles.ForEach(role => dbRole.Roles.Remove(role));

            //foreach (Role role in addedRoles)
            //{
            //    if (CurrentContext.Entry(role).State == System.Data.Entity.EntityState.Detached)
            //        CurrentContext.Roles.Attach(role);

            //    dbRole.Roles.Add(role);
            //}

            base.Update(user);
        }

        public bool CheckUserGroup(long userID, long groupID)
        {
            QueryExecutor m_executor = null;
            IACMDbOperator m_iacmOperator = null;

            string query = "select * from UserGroup where User_UserId=" + userID + " and Group_GroupId=" + groupID;
            m_iacmOperator = new IACMDbOperator(query, CommandType.Text, null);

            m_executor = new QueryExecutor(m_iacmOperator);

            DataSet ds = m_executor.ExecuteQueryDS();
            if (ds.Tables[0].Rows.Count > 0)
                return true;
            return false;

        }

        public Dictionary<long, string> GetCostCenters(string searchString, long companyId)
        {
            return null;
        }

        public Dictionary<string, string> GetOrganisationsWithPrefix(string searchString)
        {
            return CurrentContext.NOrganisations.Where(item => item.Name.Contains(searchString) && item.Id != 0)
                       .Select(x => new { OrganisationId = x.Id, x.Code, x.Name })
                       .Distinct().ToDictionary(o => o.OrganisationId + "-" + o.Code, o => o.Name);
        }


        public Dictionary<long, string> GetNonReportingUsersOfManagerID(long managerID, string searchString)
        {
            return CurrentContext.User.Where(item => (item.DisplayName.Contains(searchString) || item.UserName.Contains(searchString)) && item.TerminationDate >= DateTime.Now && item.IsActive == true)
                        .Select(x => new { Key = x.Id, Value = x.DisplayName + " [" + x.UserName + "]" })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
        }


        public Dictionary<long, string> SearchGroup(string searchString, string groupName)
        {

            return CurrentContext.User.Where(item => (item.DisplayName.Contains(searchString) || item.UserName.Contains(searchString)))
                .Select(x => new { Key = x.Id, Value = x.DisplayName })
                .Distinct().ToDictionary(o => o.Key, o => o.Value);

        }

        public Dictionary<long, string> GetByGroupName(string searchString, string groupName)
        {
            Group group = CurrentContext.Group.Include("Users").Where(grp => grp.GroupName == groupName).FirstOrDefault();
            IList<User> usrList = group.Users.Where(u => u.IsActive == true).ToList();
            return usrList.Where(item => item.DisplayName.ToUpper().Contains(searchString.ToUpper()))
            .Select(x => new { Key = x.Id, Value = x.DisplayName })
           .Distinct().ToDictionary(o => o.Key, o => o.Value);
        }


        public Dictionary<string, string> GetDisticntSectionFromUsers(string searchString)
        {
            return CurrentContext.NOrganisations.Where(item => item.Name.Contains(searchString))
                        .Select(x => new { Key = x.Code, Value = x.Code + " - " + x.Name })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
        }

        public PresentationAnonymousUser GetUserByUserName(string userName)
        {
            return (from user in CurrentContext.User.Where(usr => usr.IsActive == true && usr.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase) && (usr.FullTimeEmployee == true || usr.EmployeeType == EmployeeType.TempStaff))
                    select new PresentationAnonymousUser
                    {
                        UserName = user.UserName,
                        Password = user.Password,
                        FullTimeEmployee = user.FullTimeEmployee,
                        AuthenticationMode = user.AuthenticationMode,
                        IsActive = user.IsActive,
                        UserEmployeeType = user.EmployeeType
                    }).FirstOrDefault();
        }

        public Dictionary<long, string> GetFullTimeUsers(string searchString)
        {

            return CurrentContext.User.Where(usr => !string.IsNullOrEmpty(usr.DisplayName) && usr.FullTimeEmployee == true &&
                                              //usr.JoinDate <= DateTime.Now && 
                                              (usr.DisplayName.Contains(searchString) || usr.UserName.Contains(searchString))).Take(100).
                                              Select(x => new { key = x.Id, value = x.DisplayName + " [" + x.UserName + "]" }).
                                              ToDictionary(o => o.key, o => o.value);

        }

        public Dictionary<long, string> GetVendorUsers(string searchString)
        {

            return CurrentContext.User.Where(usr => !string.IsNullOrEmpty(usr.DisplayName) && usr.FullTimeEmployee == false &&
                                              usr.JoinDate <= DateTime.Now && (usr.DisplayName.Contains(searchString) || usr.UserName.Contains(searchString))).Take(100).
                                              Select(x => new { key = x.Id, value = x.DisplayName + " [" + x.UserName + "]" }).
                                              ToDictionary(o => o.key, o => o.value);

        }


        public Dictionary<long, string> GetUsersForSupervisor(long managerId, string searchString)
        {

            return CurrentContext.User.Where(usr => !string.IsNullOrEmpty(usr.DisplayName) && usr.FullTimeEmployee == false &&
                                              usr.JoinDate <= DateTime.Now && (usr.DisplayName.Contains(searchString) || usr.UserName.Contains(searchString))).Take(100).
                                              Select(x => new { key = x.Id, value = x.DisplayName + " [" + x.UserName + "]" }).
                                              ToDictionary(o => o.key, o => o.value);

        }

        public Dictionary<long, string> GetUsersWithSameGradeOrAbove(long gradeId, string searchString, int sequenceNumber)
        {
            var userList = (from users in CurrentContext.User
                            where !string.IsNullOrEmpty(users.DisplayName) &&
                                    users.JoinDate <= DateTime.Now && (users.DisplayName.Contains(searchString) || users.UserName.Contains(searchString))
                            select users).ToList();

            return userList.Take(100).
                Select(x => new { key = x.Id, value = x.DisplayName + " [" + x.UserName + "]" }).ToDictionary(o => o.key, o => o.value);

        }

        public List<User> GetExclusionUsersByEventType(EmployeeEvents eventType)
        {
            return (from item in CurrentContext.User
                    join excludedUser in CurrentContext.UserExclusions
                    on item.Id equals excludedUser.UserId
                    where excludedUser.EventType == eventType && excludedUser.EffectiveDate >= DateTime.Now
                    select item).ToList();
        }

        public bool IsUserAccount(long userID, long endPointSystemID, long EndpointSystemTypeId, AccountType accountType)
        {
            var result = false;
            try
            {

                var Useraccounts = CurrentContext.AccountReconcilations.Where(account => account.UserId == userID && account.EndpointSystemId == endPointSystemID &&
                                                                   account.AccountType == accountType && !account.IsDeleted && !account.IsLocked).Select(account => account).ToList();

                result = Useraccounts.Count > 0 ? true : false;
            }
            catch
            {
                throw;
            }
            return result;
        }

        public void UpdateUserPAInfo()
        {
            string query1 = "UPDATE usr SET usr.DepartmentCode = dept.Code, usr.DepartmentName = dept.Text FROM DI_M_User usr JOIN DI_HRMS_PersonnelArea dept ON usr.DepartmentId = dept.Id";
            string query2 = "UPDATE usr SET usr.DivisionCode = div.Code, usr.DivisionName = div.Text FROM DI_M_User usr JOIN DI_HRMS_PersonnelSubArea div ON usr.DivisionId = div.Id";
            string query3 = "UPDATE usr SET usr.SectionCode = org.Code, usr.SectionName = org.Name FROM DI_M_User usr JOIN DI_M_NOrganisation org ON usr.NOrganisationId = org.Id";

            this.CurrentContext.Database.ExecuteSqlCommand(query1);
            this.CurrentContext.Database.ExecuteSqlCommand(query2);
            this.CurrentContext.Database.ExecuteSqlCommand(query3);
        }

        public void UpdateROCOForReferenceUsers(long referenceUserId, string ro, string co)
        {
            try
            {
                string query = string.Format("UPDATE DI_M_User SET ROEmployeeId = '{0}', COEmployeeId = '{1}' WHERE ReferenceUserId = {2}", ro, co, referenceUserId);

                this.CurrentContext.Database.ExecuteSqlCommand(query);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int DeriveCOEmployeeId(long DIUserId)
        {
            try
            {
                string query = string.Format(@"
                                                 MERGE INTO DI_M_User AS Main
                                                   USING (
                                                 SELECT Child.ID, Child.EmployeeId  ,Child.ROEmployeeId  , Child.COEmployeeId ,  
                                                 Parent.Id PID, Parent.EmployeeId PE,Parent.ROEmployeeId AS PRO  , Parent.COEmployeeId AS PCO
                                                 FROM  DI_M_User AS Child INNER JOIN DI_M_User AS Parent ON 
                                                 Child.ROEmployeeId = Parent.EmployeeId 
                                                   WHERE Child.Id in ({0})
                                                  ) S ON Main.ID = S.ID WHEN MATCHED THEN 
                                                  UPDATE SET  COEmployeeId = S.PRO ;", DIUserId);
                var result = this.CurrentContext.Database.ExecuteSqlCommand(query);

                var referenceUsr = CurrentContext.User.FirstOrDefault(i=> i.Id == DIUserId);
                if (referenceUsr != null && !string.IsNullOrWhiteSpace(referenceUsr.COEmployeeId))
                {
                    query = $"Update DI_M_User set COEmployeeId = '{referenceUsr.COEmployeeId}' WHERE ReferenceUserId = {DIUserId} and IsActive = 1";
                    result = this.CurrentContext.Database.ExecuteSqlCommand(query);

                    query = $"Update DI_M_User set COEmployeeId = '{referenceUsr.ROEmployeeId}' WHERE ROEmployeeId = '{referenceUsr.EmployeeId}' and IsActive = 1";
                    result = this.CurrentContext.Database.ExecuteSqlCommand(query);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("HRMSCOEmpUPError", ex);
            }
        }

        public List<long> GetReferenceUsersIds(long userId)
        {
            return CurrentContext.User.Where(usr => usr.ReferenceUserId == userId && usr.IsActive).Select(x => x.Id).ToList();
        }

        public List<string> GetReferenceUsers(long userId)
        {
            try
            {

                return CurrentContext.User.Where(usr => usr.ReferenceUserId == userId && usr.IsActive).Select(x => x.DisplayName + " [" + x.UserName + "]").ToList();
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<string> GetAllEmployeePerNo()
        {
            return CurrentContext.User.Where(usr => usr.FullTimeEmployee && usr.IsActive && usr.Id > 1).Select(x => x.EmployeeId).ToList();
        }
        public Dictionary<string, string> GetGroupsById(long Id, AdHocActionType actionType, string searchString)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            List<IACMGroupInfo> iacmGroups = CurrentContext.IACMGroupInfos.Where(g=> g.IsActive).Select(g=>g).ToList();

            if (Id > 0)
            {
                if (actionType == AdHocActionType.AmendmentAdd)
                {
                    var existingGroups = CurrentContext.EntitlementReconcilations
                                                           .Where(ent => ent.EndpointUserId == Id
                                                                           && ent.EntitlementType == (int)EntitlementType.Group
                                                                           && !ent.IsDeleted
                                                                           && ent.EndpointType == EndpointType.IDMS).Select(x => x.Entitlement).ToList();

                    iacmGroups = iacmGroups.Where(x => !existingGroups.Contains(x.Name)).ToList();

                    if (!string.IsNullOrWhiteSpace(searchString))
                    {
                        return iacmGroups.Where(x => x.Name.ToLower().Contains(searchString.ToLower())).ToDictionary(key => key.Id.ToString(), value => value.Name);
                    }
                    else
                    {
                        return iacmGroups.ToDictionary(key => key.Id.ToString(), value => value.Name);
                    }
                }
                else if (actionType == AdHocActionType.AmendmentRemove)
                {
                    var existingGroups = CurrentContext.EntitlementReconcilations
                                                           .Where(ent => ent.EndpointUserId == Id
                                                                           && ent.EntitlementType == (int)EntitlementType.Group
                                                                           && !ent.IsDeleted
                                                                           && ent.EndpointType == EndpointType.IDMS).Select(x => x.Entitlement).ToList();

                    iacmGroups = iacmGroups.Where(x => existingGroups.Contains(x.Name)).ToList();

                    if (!string.IsNullOrWhiteSpace(searchString))
                    {
                        return iacmGroups.Where(x => x.Name.ToLower().Contains(searchString.ToLower())).ToDictionary(key => key.Id.ToString(), value => value.Name);
                    }
                    else
                    {
                        return iacmGroups.ToDictionary(key => key.Id.ToString(), value => value.Name);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(searchString))
                    return this.CurrentContext.IACMGroupInfos.Where(x => x.Name.ToLower().Contains(searchString.ToLower())).ToDictionary(key => key.Id.ToString(), value => value.Name);
                else
                    return this.CurrentContext.IACMGroupInfos.ToDictionary(key => key.Id.ToString(), value => value.Name);
            }

            return keyValuePairs;
        }

        public Dictionary<string, string> GetRolesById(long Id, AdHocActionType actionType, string searchString)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            List<IACMRoleInfo> iacmRoles = CurrentContext.IACMRoleInfos.Where(g => g.IsActive && (g.CanRequest != null && g.CanRequest.Value)).Select(g => g).ToList();

            if (Id > 0)
            {
                if (actionType == AdHocActionType.AmendmentAdd)
                {
                    var existingRoles = CurrentContext.EntitlementReconcilations
                                                           .Where(ent => ent.EndpointUserId == Id
                                                                           && ent.EntitlementType == (int)EntitlementType.Role
                                                                           && !ent.IsDeleted
                                                                           && ent.EndpointType == EndpointType.IDMS).Select(x => x.Entitlement).ToList();

                    iacmRoles = iacmRoles.Where(x => !existingRoles.Contains(x.Name)).ToList();

                    if (!string.IsNullOrWhiteSpace(searchString))
                    {
                        return iacmRoles.Where(x => x.Name.ToLower().Contains(searchString.ToLower())).ToDictionary(key => key.Id.ToString(), value => value.Name);
                    }
                    else
                    {
                        return iacmRoles.ToDictionary(key => key.Id.ToString(), value => value.Name);
                    }
                }
                else if (actionType == AdHocActionType.AmendmentRemove)
                {
                    var existingRoles = CurrentContext.EntitlementReconcilations
                                                           .Where(ent => ent.EndpointUserId == Id
                                                                           && ent.EntitlementType == (int)EntitlementType.Role
                                                                           && !ent.IsDeleted
                                                                           && ent.EndpointType == EndpointType.IDMS).Select(x => x.Entitlement).ToList();

                    iacmRoles = iacmRoles.Where(x => existingRoles.Contains(x.Name)).ToList();

                    if (!string.IsNullOrWhiteSpace(searchString))
                    {
                        return iacmRoles.Where(x => x.Name.ToLower().Contains(searchString.ToLower())).ToDictionary(key => key.Id.ToString(), value => value.Name);
                    }
                    else
                    {
                        return iacmRoles.ToDictionary(key => key.Id.ToString(), value => value.Name);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(searchString))
                    return this.CurrentContext.IACMRoleInfos.Where(x => x.Name.ToLower().Contains(searchString.ToLower())).ToDictionary(key => key.Id.ToString(), value => value.Name);
                else
                    return this.CurrentContext.IACMRoleInfos.ToDictionary(key => key.Id.ToString(), value => value.Name);
            }

            return keyValuePairs;
        }

        public Dictionary<string, string> GetOrgCompanies(string searchString)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    return CurrentContext.User.Where(item => item.OrgCompany.ToLower().Contains(searchString.ToLower()))
                   .Select(x => new { key = x.OrgCompany, value = x.OrgCompany })
                   .Distinct().OrderBy(s => s.key).ToDictionary(o => o.key, o => o.value);
                }
                else
                {
                    return CurrentContext.User.Where(item => item.OrgCompany != null && item.OrgCompany != string.Empty)
                  .Select(x => new { key = x.OrgCompany, value = x.OrgCompany })
                  .Distinct().OrderBy(s => s.key).ToDictionary(o => o.key, o => o.value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<string, string> GetOrgDepartments(string searchString, List<string> CompanyNameFilter)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    return CurrentContext.User.Where(item => CompanyNameFilter.Contains(item.OrgCompany) && item.OrgDepartment != null && item.OrgDepartment.ToLower().Contains(searchString.ToLower()))
                        .Select(x => new { Key = x.OrgDepartment, Value = x.OrgDepartment })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
                else
                {
                    return CurrentContext.User.Where(item => CompanyNameFilter.Contains(item.OrgCompany) && item.OrgDepartment != null && item.OrgDepartment != string.Empty)
                       .Select(x => new { Key = x.OrgDepartment, Value = x.OrgDepartment })
                       .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<string, string> GetOrgDivisions(string searchString, List<string> CompanyNameFilter, List<string> DepartNameFilter)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    return CurrentContext.User.Where(item => CompanyNameFilter.Contains(item.OrgCompany) && DepartNameFilter.Contains(item.OrgDepartment) && item.OrgDivision != null && item.OrgDivision.ToLower().Contains(searchString.ToLower()))
                        .Select(x => new { Key = x.OrgDivision, Value = x.OrgDivision })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
                else
                {
                    return CurrentContext.User.Where(item => CompanyNameFilter.Contains(item.OrgCompany) && DepartNameFilter.Contains(item.OrgDepartment) && item.OrgDivision != null && item.OrgDivision != string.Empty)
                       .Select(x => new { Key = x.OrgDivision, Value = x.OrgDivision })
                       .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }


        public Dictionary<string, string> GetOrgBranches(string searchString, List<string> CompanyNameFilter, List<string> DepartNameFilter)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    return CurrentContext.User.Where(item => CompanyNameFilter.Contains(item.OrgCompany) && DepartNameFilter.Contains(item.OrgDepartment) //&& DivisionNameFilter.Contains(item.OrgDivision) 
                    && item.OrgBranch != null && item.OrgBranch.ToLower().Contains(searchString.ToLower()))
                        .Select(x => new { Key = x.OrgBranch, Value = x.OrgBranch })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
                else
                {
                    return CurrentContext.User.Where(item => CompanyNameFilter.Contains(item.OrgCompany) && DepartNameFilter.Contains(item.OrgDepartment) //&& DivisionNameFilter.Contains(item.OrgDivision)
                      && item.OrgBranch != null && item.OrgBranch != string.Empty)
                       .Select(x => new { Key = x.OrgBranch, Value = x.OrgBranch })
                       .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<string, string> GetOrgSections(string searchString, List<string> CompanyNameFilter, List<string> DepartNameFilter, List<string> BranchNameFilter)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    return CurrentContext.User.Where(item => CompanyNameFilter.Contains(item.OrgCompany) && DepartNameFilter.Contains(item.OrgDepartment)
                    && BranchNameFilter.Contains(item.OrgBranch) && item.OrgSection != null && item.OrgSection.ToLower().Contains(searchString.ToLower()))
                        .Select(x => new { Key = x.OrgSection, Value = x.OrgSection })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
                else
                {
                    return CurrentContext.User.Where(item => CompanyNameFilter.Contains(item.OrgCompany) && DepartNameFilter.Contains(item.OrgDepartment)
                      && BranchNameFilter.Contains(item.OrgBranch) && item.OrgSection != null && item.OrgSection != string.Empty)
                       .Select(x => new { Key = x.OrgSection, Value = x.OrgSection })
                       .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<string, string> GetOrgUnits(string searchString, List<string> CompanyNameFilter, List<string> DepartNameFilter, List<string> BranchNameFilter, List<string> SectioNameFilter)
        {
            try
            {
                Dictionary<string, string> ret = new Dictionary<string, string>();
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    ret = CurrentContext.User.Where(item => CompanyNameFilter.Contains(item.OrgCompany) && DepartNameFilter.Contains(item.OrgDepartment)
                      && BranchNameFilter.Contains(item.OrgBranch) && SectioNameFilter.Contains(item.OrgSection) && item.Reserved8 != null && item.Reserved8.ToLower().Contains(searchString.ToLower()))
                        .Select(x => new { Key = x.Reserved8, Value = x.Reserved8 })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                    return ret;
                }
                else
                {
                    ret = CurrentContext.User.Where(item => CompanyNameFilter.Contains(item.OrgCompany) && DepartNameFilter.Contains(item.OrgDepartment)
                      && BranchNameFilter.Contains(item.OrgBranch) && SectioNameFilter.Contains(item.OrgSection) && item.Reserved8 != null && item.Reserved8 != string.Empty)
                       .Select(x => new { Key = x.Reserved8, Value = x.Reserved8 })
                       .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                    return ret;
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetReportingofSubordinate(long UserID, string searchString, List<string> CompanyName, List<string> DepartmentName, List<string> BranchName, List<string> SectionName, List<string> UnitName, List<string> excludedUsers)
        {
            Dictionary<long, string> data = new Dictionary<long, string>();
            try
            {
                IList<User> User = new List<User>();
                bool IsAdmin = false;
                bool IsIACMAdmin = false;
                this.IsAdminUser(out IsAdmin, out IsIACMAdmin, UserID);
                if (IsAdmin || IsIACMAdmin)
                {
                    if (string.IsNullOrWhiteSpace(searchString))
                    {
                        data = CurrentContext.User.Where(item =>
                           (CompanyName.Count == 0 || (item.OrgCompany != null && item.OrgCompany != string.Empty && CompanyName.Contains(item.OrgCompany)))
                           && (DepartmentName.Count == 0 || (item.OrgDepartment != null && item.OrgDepartment != string.Empty && DepartmentName.Contains(item.OrgDepartment)))

                            //&& (DivisionName.Count == 0 || (item.OrgDivision != null && item.OrgDivision != string.Empty && DivisionName.Contains(item.OrgDivision)))

                            && (BranchName.Count == 0 || (item.OrgBranch != null && item.OrgBranch != string.Empty && BranchName.Contains(item.OrgBranch)))
                       && (excludedUsers.Count == 0 || (item.EmployeeId != null && item.EmployeeId != string.Empty && !excludedUsers.Contains(item.EmployeeId)))
                       && (UnitName.Count == 0 || (item.Reserved8 != null && item.Reserved8 != string.Empty && UnitName.Contains(item.Reserved8)))
                          && (SectionName.Count == 0 || (item.OrgSection != null && item.OrgSection != string.Empty && SectionName.Contains(item.OrgSection))))


                          .OrderBy(d => d.DisplayName).Select
                     (x => new { Key = x.Id, Value = x.DisplayName + " [" + x.UserName + "]" })
                       .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                    }
                    else
                    {
                        data = CurrentContext.User.Where(item => item.DisplayName.ToLower().Contains(searchString.ToLower())
                         && (CompanyName.Count == 0 || (item.OrgCompany != null && item.OrgCompany != string.Empty && CompanyName.Contains(item.OrgCompany)))
                          && (DepartmentName.Count == 0 || (item.OrgDepartment != null && item.OrgDepartment != string.Empty && DepartmentName.Contains(item.OrgDepartment)))

                           //&& (DivisionName.Count == 0 || (item.OrgDivision != null && item.OrgDivision != string.Empty && DivisionName.Contains(item.OrgDivision)))

                           && (BranchName.Count == 0 || (item.OrgBranch != null && item.OrgBranch != string.Empty && BranchName.Contains(item.OrgBranch)))
                       && (excludedUsers.Count == 0 || (item.EmployeeId != null && item.EmployeeId != string.Empty && !excludedUsers.Contains(item.EmployeeId)))
                        && (UnitName.Count == 0 || (item.Reserved8 != null && item.Reserved8 != string.Empty && UnitName.Contains(item.Reserved8)))
                         && (SectionName.Count == 0 || (item.OrgSection != null && item.OrgSection != string.Empty && SectionName.Contains(item.OrgSection)))).OrderBy(d => d.DisplayName).Select
                    (x => new { Key = x.Id, Value = x.DisplayName + " [" + x.UserName + "]" })
                      .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                    }
                }
                else
                {
                    //var users = GetAllSubordinateByUserID(UserID, searchString);

                    var users = GetAllSubordinatesByROandCO(UserID, searchString);
                    if (excludedUsers.Count > 0)
                        User = users.Where(item => !excludedUsers.Contains(item.EmployeeId)).OrderBy(d => d.DisplayName).ToList();
                    else
                        User = users.OrderBy(d => d.DisplayName).ToList();
                    if (User.Count > 0)
                    {
                        data = User.Select(x => new { Key = x.Id, Value = x.DisplayName + " [" + x.UserName + "]" })
                       .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                    }
                }
                return data;
            }
            catch
            {
                throw;
            }
        }

        public void IsAdminUser(out bool isAdminGroup, out bool isIACMAdminGroup, long userID)
        {
            isAdminGroup = false;
            isIACMAdminGroup = false;
            try
            {
                var itSecurityGroupId = this.CurrentContext.ConfigurationMastersIACM
                                                            .Where(conf => conf.ConfigKey == "ITSecurityGroupId")
                                                            .Select(conf => conf.ConfigValue).FirstOrDefault();

                User aUser = GetById(userID, true);
                if (aUser != null)
                {
                    foreach (Group grpItem in aUser.Groups)
                    {
                        if (grpItem.GroupName == "Administrator" || grpItem.Id.ToString() == itSecurityGroupId)
                        {
                            isAdminGroup = true;
                            isIACMAdminGroup = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<User> GetAllSubordinatesByROandCO(long UserID, string searchString)
        {
            IList<User> User = new List<User>();
            try
            {
                User user = CurrentContext.User.Where(item => item.Id == UserID).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(searchString))
                    return CurrentContext.User.Where(item => item.ROEmployeeId == user.EmployeeId).Distinct().ToList();
                //   List<User> COusers = _context.Users.Where(item => item.Reserved7 == user.EmployeeId).Distinct().ToList();
                return CurrentContext.User.Where(item => item.ROEmployeeId == user.EmployeeId && item.DisplayName.ToLower().Contains(searchString.ToLower())).Distinct().ToList(); ;
            }
            catch
            {
                throw;
            }

        }

        public List<UserAccessDetailMatrixGridData> GetUserAccessDetailMatrixGridData(List<long> userIds, List<string> excludedUsers)
        {
            var subQuery = from ER in CurrentContext.EntitlementReconcilations
                           where !ER.IsDeleted && ER.UserId.HasValue && userIds.Contains(ER.UserId.Value)
                           select ER;
            var query = from AR in CurrentContext.AccountReconcilations
                        join ER in subQuery on AR.Id equals ER.AccountReconcilationId into ARER
                        from ER in ARER.DefaultIfEmpty()
                        join UR in CurrentContext.User on AR.UserId equals UR.Id
                        join RO in CurrentContext.User on UR.ROEmployeeId equals RO.EmployeeId into URO
                        from RO in URO.DefaultIfEmpty()
                        join ES in CurrentContext.EndPoint on AR.EndpointSystemId equals ES.Id
                        where userIds.Contains(UR.Id) && !AR.IsDeleted && !AR.IsLocked
                        select new UserAccessDetailMatrixGridData
                        {
                            UserName = UR.UserName,
                            IsActive = UR.IsActive,
                            AccountType = AR.AccountType.ToString(),
                            ApplicationType = ES.ApplicationType.ToString(),
                            EndpointSystemName = ES.DisplayName,
                            EndpointHostName = ES.ApplicationCode,
                            LastLogonDate = AR.LastLogonDate,
                            Entitlement = ER.Entitlement,
                            EntitlementType = ((EntitlementType)ER.EntitlementType).ToString(),
                            Description = ER.EntitlementDescription,
                            FullName = UR.DisplayName,
                            Name = AR.AccountName,
                            EndpointSystemID = ES.Id,
                            EndpointSystemTypeId = ES.EndpointSystemTypeId,
                            AccountReconcilationId = AR.Id,
                            Company = UR.OrgCompany,
                            Division = UR.OrgDivision,
                            Department = UR.OrgDepartment,
                            Branch = UR.OrgBranch,
                            Section = UR.OrgSection,
                            Unit = UR.OrgUnit,
                            ReportingOfficer = RO.DisplayName
                        };

            var lstUserAccessDetailMatrixGridData = query.OrderBy(p => p.ApplicationType).ThenBy(p => p.EndpointSystemName).ThenBy(p => p.FullName).ThenBy(p => p.Entitlement).ToList();
            return lstUserAccessDetailMatrixGridData;
        }
        public PresentationGridPagedData<UserAccessDetailMatrixGridData> GetUserAccessDetailMatrixPagedData(List<long> userIds, List<string> excludedUsers, string sortExpression, string filterExpression, int pageSize, int startIndex)
        {
            this.CurrentContext.Database.CommandTimeout = 0;
            PresentationGridPagedData<UserAccessDetailMatrixGridData> dbResult = new PresentationGridPagedData<UserAccessDetailMatrixGridData>();


            var subQuery = from ER in CurrentContext.EntitlementReconcilations
                           where !ER.IsDeleted && ER.UserId.HasValue && userIds.Contains(ER.UserId.Value)
                           select new
                           {
                               ER.AccountReconcilationId,
                               ER.Entitlement,
                               ER.EntitlementType,
                               ER.EntitlementDescription,
                               ER.EndpointSystemId,
                               ER.Functions
                           };
            var accountRecons = from accountRecon in CurrentContext.AccountReconcilations
                                where accountRecon.UserId.HasValue && userIds.Contains(accountRecon.UserId.Value) && !accountRecon.IsDeleted && !accountRecon.IsLocked
                                select accountRecon;
            if (accountRecons.Count() == 0) return dbResult;
            var epIds = accountRecons.Select(i => i.EndpointSystemId).ToList();
            if (epIds.Count == 0) return dbResult;

            var inhouseFuncLevelEndpoints = (from ep in CurrentContext.EndPoint
                                             where ep.IsFunctionLevel
                                             select ep.Id).ToList();


            var eptypeId = (int)EndpointType.SAP;
            var type = (int)EntitlementType.Role;
            var sapEndpointSystemIds = CurrentContext.EndPoint.Where(i => i.EndpointSystemTypeId == eptypeId && epIds.Contains(i.Id)).Select(i => i.Id).ToList();

            var query = from AR in accountRecons
                        join ER in subQuery on AR.Id equals ER.AccountReconcilationId into ARER
                        from ER in ARER.DefaultIfEmpty()
                        join UR in CurrentContext.User on AR.UserId equals UR.Id
                        join RO in CurrentContext.User on UR.ROEmployeeId equals RO.EmployeeId into URO
                        from RO in URO.DefaultIfEmpty()
                        join ES in CurrentContext.EndPoint on AR.EndpointSystemId equals ES.Id
                        join tcodes in CurrentContext.SAPRoleToTCodes on ER.Entitlement equals tcodes.RoleCode into SAPTcode
                        from tcodes in SAPTcode.DefaultIfEmpty()
                        where userIds.Contains(UR.Id) && !AR.IsDeleted && !AR.IsLocked
                        select new UserAccessDetailMatrixGridData
                        {
                            Institution = (tcodes != null && (ER.EndpointSystemId == tcodes.EndPointSystemId && ER.Entitlement == tcodes.RoleCode && ER.EntitlementType == type && ES.EndpointSystemTypeId == eptypeId) ? tcodes.TCodes : ""),
                            UserName = UR.UserName,
                            IsActive = UR.IsActive,
                            AccountType = AR.AccountType.ToString(),
                            ApplicationType = ES.ApplicationType.ToString(),
                            EndpointSystemName = ES.DisplayName,
                            EndpointHostName = ES.ApplicationCode,
                            LastLogonDate = AR.LastLogonDate,
                            Entitlement = ER.Entitlement,
                            EntitlementType = ((EntitlementType)ER.EntitlementType).ToString(),
                            Description = ER.EntitlementDescription,
                            FullName = UR.DisplayName,
                            Name = AR.AccountName,
                            EndpointSystemID = ES.Id,
                            EndpointSystemTypeId = ES.EndpointSystemTypeId,
                            AccountReconcilationId = AR.Id,
                            Company = UR.OrgCompany,
                            Division = UR.OrgDivision,
                            Department = UR.OrgDepartment,
                            Branch = UR.OrgBranch,
                            Section = UR.OrgSection,
                            Unit = UR.OrgUnit,
                            ReportingOfficer = RO.DisplayName,
                            Functions = ER.Functions
                        };
            if (!string.IsNullOrWhiteSpace(filterExpression))
            {
                query = query.Where(filterExpression);
            }
            dbResult.TotalCount = query.Count();
            if (!string.IsNullOrWhiteSpace(sortExpression))
            {
                sortExpression = sortExpression.Trim(',');
                query = query.OrderBy(sortExpression);
            }
            else
            {
                query = query.OrderBy(p => p.ApplicationType).ThenBy(p => p.EndpointSystemName).ThenBy(p => p.FullName).ThenBy(p => p.UserName).ThenBy(i=>i.Entitlement);
            }

            if (pageSize == -1)
                dbResult.Data = query.ToList();
            else
                dbResult.Data = query.Skip(startIndex).Take(pageSize).ToList();

            foreach (var v in dbResult.Data)
            {
                if (inhouseFuncLevelEndpoints.Contains(v.EndpointSystemID))
                {
                    v.Entitlement = $"{v.Entitlement}=>{v.Functions}";
                }
            }

            return dbResult;
        }
        public void AssignCoordinatorRolesIfRequiredAndDefaultRolesIfNotExists()
        {
            string query = $@"SET NOCOUNT ON;
 
                                 
                                declare @tblCoordinateRolesContent table(id int identity, roleId int,   rolename nvarchar(max))
								declare @tblITSecurityRolesContent table(id int identity, roleId int,   rolename nvarchar(max))

                                --GET MANAGER ROLES--
                                INSERT INTO @tblCoordinateRolesContent 
                                SELECT C.Item , R.RoleName  FROM DI_M_Role R inner join (SELECT CAST(B.Item as bigint)  as Item   FROM [dbo].[DI_M_IACMConfigurationMaster]  A cross apply dbo.split(A.[ConfigValue],',') B  WHERE A.[ConfigKey] in ('IACMCoordinatorRoles') ) C
                                ON R.Id = C.Item 

								-- Step 1: Delete the Manager role for users who are not reporting managers
                                DELETE FROM UserRole
                                WHERE Role_RoleId IN(SELECT roleId from @tblCoordinateRolesContent)
                                AND User_UserId NOT IN (
                                    SELECT DISTINCT u2.Id
                                    FROM DI_M_User u1
                                    JOIN DI_M_User u2 ON u1.ROEmployeeId = u2.EmployeeId
	                                WHERE u1.ROEmployeeId IS NOT NULL AND U1.IsActive = 1
                                );

                                -- Step 2: Insert the Manager role for users who are reporting managers
                                INSERT INTO UserRole (User_UserId, Role_RoleId)
                                SELECT A.Id,B.roleId FROM 
                                (
                                SELECT DISTINCT u2.Id
                                FROM DI_M_User u1
                                JOIN DI_M_User u2 ON u1.ROEmployeeId = u2.EmployeeId
                                WHERE NOT EXISTS (
                                    SELECT 1
                                    FROM UserRole ur
                                    WHERE ur.User_UserId = u2.Id
                                    AND ur.Role_RoleId IN(SELECT roleId from @tblCoordinateRolesContent)
                                ) AND  u1.ROEmployeeId IS NOT NULL AND U1.IsActive = 1)AS A, @tblCoordinateRolesContent AS B; 

";



            //INSERT INTO @tblITSecurityRolesContent
            //                    SELECT R.Id,  R.RoleName FROM DI_M_Role R inner join(SELECT CAST(B.Item as bigint) as Item  FROM[dbo].[DI_M_IACMConfigurationMaster]  A cross apply dbo.split(A.[ConfigValue], ',') B WHERE A.[ConfigKey] in ('ITSecurityAdminRole') ) C
            //                ON R.Id = C.Item


            //DELETE FROM UserRole WHERE User_UserId in (select r.Id From DI_M_User r  where r.DepartmentCode in (SELECT B.Item FROM[dbo].[DI_M_IACMConfigurationMaster] A cross apply dbo.split(A.[ConfigValue],',') B WHERE A.[ConfigKey] = 'IT_DepartmentCode'))
            //                    AND Role_RoleId IN(SELECT ROLEID FROM @tblITSecurityRolesContent)



            // INSERT INTO UserRole
            //select t1.roleId, t2.Id from @tblITSecurityRolesContent t1, (select r.Id  From DI_M_User r  where r.DepartmentCode in (SELECT B.Item  FROM[dbo].[DI_M_IACMConfigurationMaster]  A cross apply dbo.split(A.[ConfigValue], ',') B  WHERE A.[ConfigKey] = 'IT_DepartmentCode')) t2
            this.CurrentContext.Database.CommandTimeout = 300;
            this.CurrentContext.Database.ExecuteSqlCommand(query);
        }
        public Dictionary<long, string> GetGroupMembersbyUserId(string searchString, long userId, List<long> rolesToExclude)
        {
            var results = new Dictionary<long, string>();
            try
            {
                var condition = "SELECT Role_RoleId FROM UserRole WHERE User_UserId = @UserId";
                if (rolesToExclude.Count > 0)
                {
                    condition = $"{condition} AND Role_RoleId NOT IN({string.Join(",", rolesToExclude)})";
                }

                var query = $@" SELECT U.* FROM DI_M_User U INNER JOIN UserRole UR on U.Id = UR.User_UserId
                                WHERE U.ISACTIVE=1 AND UR.Role_RoleId in({condition}) ";
                var groupMemberParameters = new List<System.Data.SqlClient.SqlParameter> { new System.Data.SqlClient.SqlParameter("@UserId", userId) };
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    query = query + " AND U.DisplayName like @SearchString";
                    groupMemberParameters.Add(new System.Data.SqlClient.SqlParameter("@SearchString", "%" + searchString + "%"));
                }
                var users = CurrentContext.Database.SqlQuery<User>(query, groupMemberParameters.ToArray()).ToList();
                List<User> usrList = new List<User>();
                if (users.Count > 0)
                {
                    results = users.Select(x => new { Key = x.Id, Value = x.DisplayName })
                                .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }

        public Dictionary<long, string> GetUsersByGroupId(string searchString, long groupId, string excludeRoleIds)
        {
            var results = new Dictionary<long, string>();

            try
            {
                var safeExcludeRoleIds = string.Join(",", (excludeRoleIds ?? string.Empty)
                    .Split(',')
                    .Select(id => id.Trim())
                    .Where(id => id.Length > 0)
                    .Select(id => { if (!long.TryParse(id, out var roleId)) throw new ArgumentException("excludeRoleIds must be a comma-separated list of integers."); return roleId; }));
                if (string.IsNullOrEmpty(safeExcludeRoleIds))
                {
                    safeExcludeRoleIds = "NULL";
                }

                var query = $"select * from DI_M_User usr inner join UserGroup ug on usr.id= ug.User_UserId where usr.IsActive = 1 and ug.Group_GroupId = @GroupId And usr.id not in (select usr.Id from di_m_user usr inner join UserRole ur on usr.Id = ur.User_UserId and ur.Role_RoleId in ({safeExcludeRoleIds}))";
                var groupUserParameters = new List<System.Data.SqlClient.SqlParameter> { new System.Data.SqlClient.SqlParameter("@GroupId", groupId) };

                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    query = query + " AND usr.DisplayName like @SearchString";
                    groupUserParameters.Add(new System.Data.SqlClient.SqlParameter("@SearchString", "%" + searchString + "%"));
                }

                var users = CurrentContext.Database.SqlQuery<User>(query, groupUserParameters.ToArray()).ToList();

                List<User> usrList = new List<User>();

                if (users.Count > 0)
                {
                    results = users.Select(x => new { Key = x.Id, Value = x.DisplayName })
                                .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }

        public void PerformLogoutCleanup(long userId)
        {
            string query1 = $"DELETE FROM DI_M_UserGridFilterInfo WHERE UserId={userId}";
            string query2 = $"DELETE FROM DI_M_UserGridInfo WHERE UserId={userId}";
            string query3 = $"DELETE FROM DI_M_UserGridColumnInfo WHERE UserId={userId}";

            this.CurrentContext.Database.ExecuteSqlCommand(query1);
            this.CurrentContext.Database.ExecuteSqlCommand(query2);
            this.CurrentContext.Database.ExecuteSqlCommand(query3);
        }
        public List<User> GetUserByCompanyCode(string code)
        {
            return (from usr in CurrentContext.User
                    join cmp in CurrentContext.Companies on usr.CompanyCode equals cmp.Code
                    join org in CurrentContext.NOrganisations on cmp.Text equals org.Name
                    where org.IsActive && org.Code == code && usr.IsActive && org.ScopeId == 1
                    select usr
                   ).ToList();
            //return this.CurrentContext.Database.SqlQuery<User>("select * from DI_M_User where CompanyCode in( select Code from DI_HRMS_Company where Text in(select Name from DI_M_NOrganisation where ScopeId=1 AND IsActive=1 and  Code=@Code)) and IsActive = 1", new SqlParameter("@Code", code)).ToList();
        }
        public Dictionary<long,string> GetAllReportingOfficer(string searchString)
        {
            Dictionary<long,string>results = new Dictionary<long,string>();
            var roUsr = CurrentContext.User.Join(CurrentContext.User,
                                            roUser => roUser.EmployeeId,
                                            userRO => userRO.ROEmployeeId,
                                            (roUser, userRO) => new { ReportingOfficerUser = roUser, UserEmployee = userRO })
                                            .Where(both => both.UserEmployee.IsActive && both.ReportingOfficerUser.IsActive)
                                            .Select(both => both.ReportingOfficerUser).Distinct().ToList();

            if (roUsr != null && roUsr.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(searchString))
                    roUsr = roUsr.Where(i => i.DisplayName.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)).ToList().ToList();

                results = roUsr.Distinct().ToDictionary(key => key.Id, name => $"{name.DisplayName}[{name.UserName}]");
            }
            return results;
        }
        public List<User> GetAllUserUnderReportingOfficer(Nullable<long> roUserId)
        {
            var result = CurrentContext.User.Join(CurrentContext.User,
                                            roUser => roUser.EmployeeId,
                                            userRO => userRO.ROEmployeeId,
                                            (roUser, userRO) => new { ReportingOfficerUser = roUser, UserEmployee = userRO })
                                            .Where(both => both.UserEmployee.IsActive && both.ReportingOfficerUser.IsActive
                                            );
            if(roUserId!=null && roUserId.Value > 0)
            {
                result = result.Where(i=> i.ReportingOfficerUser.Id == roUserId);
            }
            return result.Select(i => i.UserEmployee).Distinct().ToList();
        }
        public Dictionary<long,string> GetCustomReportUserBySearchString(string searchString)
        {
            var user = from usr in CurrentContext.User
                       select new
                       {
                           usr.Id,
                           usr.DisplayName,
                           usr.UserName
                       };
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                user = user.Where(i=> i.DisplayName.Contains(searchString));
            }
            return user.ToDictionary(key => key.Id, value => value.DisplayName + " [" + value.UserName + "]");
        }

        public List<User> GetAllRO()
        {
            return (from usr in CurrentContext.User
                    join ro in CurrentContext.User on usr.ROEmployeeId equals ro.EmployeeId
                    where (usr.ROEmployeeId != "" || usr.ROEmployeeId != null)
                    select new
                    {
                        ro.UserName,
                        ro.EmployeeId,
                        ro.DisplayName,
                        ro.IsActive
                    }).Distinct().ToList().Select(i => new User {
                        UserName = i.UserName,
                        EmployeeId = i.EmployeeId,
                        DisplayName = i.DisplayName,
                        IsActive = i.IsActive
                    }).ToList();
        }
        public Dictionary<long,string> GetAllActiveRO(string searchString)
        {
            var temp = (from usr in CurrentContext.User
                        join ro in CurrentContext.User on usr.ROEmployeeId equals ro.EmployeeId
                        where ro.IsActive && (usr.ROEmployeeId != "" || usr.ROEmployeeId != null)
                        select new { Id = ro.Id, DisplayName = ro.DisplayName , ro.UserName});
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                temp = temp.Where(i => i.DisplayName.Contains(searchString));
            }
            return (temp.Select(i=> new {Id =i.Id, DisplayName = i.DisplayName + " [" + i.UserName + "]" }).Distinct().
                    ToDictionary(i => i.Id, v => v.DisplayName));

        }
        public User GetReportingOfficerByUser(long userId)
        {
            var user = (from usr in CurrentContext.User
                        where usr.Id == userId
                        select usr).FirstOrDefault();
            if(user != null)
            {
                return CurrentContext.User.FirstOrDefault(i => i.EmployeeId == user.ROEmployeeId && i.IsActive);
            }

            return null;
        }

        public List<long> GetIACMUserForAttestation(List<string> IDMSAttestationRoleToExclude)
        {
            if(IDMSAttestationRoleToExclude != null && IDMSAttestationRoleToExclude.Count > 0)
            {
                return (from acc in CurrentContext.AccountReconcilations
                        join ent in CurrentContext.EntitlementReconcilations on acc.Id equals ent.AccountReconcilationId
                        join endpoint in CurrentContext.EndPoint on acc.EndpointSystemId equals endpoint.Id
                        where acc.UserId != null && endpoint.ApplicationType == ApplicationType.IDMS && ent.IsDeleted == false && acc.IsDeleted == false
                        && (ent.EntitlementType == (int)EntitlementType.Group || (ent.EntitlementType == (int)EntitlementType.Role && !IDMSAttestationRoleToExclude.Contains(ent.Entitlement)))
                        select acc.UserId.Value).Distinct().ToList();
            }
            else
            {
                return (from acc in CurrentContext.AccountReconcilations
                        join ent in CurrentContext.EntitlementReconcilations on acc.Id equals ent.AccountReconcilationId
                        join endpoint in CurrentContext.EndPoint on acc.EndpointSystemId equals endpoint.Id
                        where acc.UserId != null && endpoint.ApplicationType == ApplicationType.IDMS && ent.IsDeleted == false && acc.IsDeleted == false
                        && (ent.EntitlementType == (int)EntitlementType.Group || (ent.EntitlementType == (int)EntitlementType.Role && ent.Entitlement != "User" && ent.Entitlement != "Manager"))
                        select acc.UserId.Value).Distinct().ToList();
            }


        }
        public List<long> GetOneCardUserForAttestation(List<string> IDMSAttestationRoleToExclude)
        {

            if (IDMSAttestationRoleToExclude != null && IDMSAttestationRoleToExclude.Count > 0)
            {
                return (from acc in CurrentContext.AccountReconcilations
                        join ent in CurrentContext.EntitlementReconcilations on acc.Id equals ent.AccountReconcilationId
                        join endpoint in CurrentContext.EndPoint on acc.EndpointSystemId equals endpoint.Id
                        where acc.UserId != null && endpoint.ApplicationType == ApplicationType.OneCard && ent.IsDeleted == false && acc.IsDeleted == false
                        && (ent.EntitlementType == (int)EntitlementType.AccessLevel && !IDMSAttestationRoleToExclude.Contains(ent.Entitlement))
                        select acc.UserId.Value).Distinct().ToList();
            }
            else
            {
                return (from acc in CurrentContext.AccountReconcilations
                        join ent in CurrentContext.EntitlementReconcilations on acc.Id equals ent.AccountReconcilationId
                        join endpoint in CurrentContext.EndPoint on acc.EndpointSystemId equals endpoint.Id
                        where acc.UserId != null && endpoint.ApplicationType == ApplicationType.OneCard && ent.IsDeleted == false && acc.IsDeleted == false
                        && (ent.EntitlementType == (int)EntitlementType.AccessLevel)
                        select acc.UserId.Value).Distinct().ToList();
            }
        }
        /// <summary>
        /// Take only Fulltime Empoyee to update in HRMS system
        /// </summary>
        /// <param name="takeOnlyNeedTelephoneUpdateToHR"></param>
        /// <param name="takeOnlyNeedEmailUpdateToHR"></param>
        /// <param name="takeUpdateFLINSLoginIdInHR"></param>
        /// <returns></returns>
        public List<PresentationUserDetails> GetPresentationUsersToUpdateEmailIdInHR(bool takeOnlyNeedTelephoneUpdateToHR = false, bool takeOnlyNeedEmailUpdateToHR = false, bool takeUpdateFLINSLoginIdInHR = false)
        {

            var users = (from usr in CurrentContext.User
                         //where usr.FullTimeEmployee
                         select usr);
            if (takeOnlyNeedTelephoneUpdateToHR || takeOnlyNeedEmailUpdateToHR || takeUpdateFLINSLoginIdInHR)
            {

                if (takeOnlyNeedEmailUpdateToHR)
                {
                    users = users.Where(i => i.FullTimeEmployee && i.NeedEmailUpdateToHR );
                }
                else if (takeOnlyNeedTelephoneUpdateToHR)
                {
                    users = users.Where(i => i.NeedTelephoneUpdateToHR);
                }
                else if (takeUpdateFLINSLoginIdInHR)
                {
                    users = users.Where(i => i.FullTimeEmployee && i.NeedFLINSLoginIdUpdateToHR && i.JoinDate <= DateTime.Now);
                }
                return (from usr in users
                        select new PresentationUserDetails()
                        {
                            Id = usr.Id,
                            DisplayName = usr.DisplayName,
                            EmployeeId = usr.EmployeeId,
                            FullTimeEmployee = usr.FullTimeEmployee,
                            JoinDate = usr.JoinDate,
                            UserName = usr.UserName,
                            TelephoneNumber = usr.TelephoneNumber
                        }).ToList();
            }
            return new List<PresentationUserDetails>();
        }

        public string GetUserMobileNumber(string employeeId)
        {
            return CurrentContext.User.FirstOrDefault(i => i.EmployeeId == employeeId)?.MobileNumber;
        }
    }
}
