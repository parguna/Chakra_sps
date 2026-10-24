using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure;
using DeepIdentity.Logger;
using DeepIdentity.Model;
using DeepIdentity.Model.CustomEntities;
using DeepIdentity.Model.SAP;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using DeepIdentity.Model.ReportData;
using DeepIdentity.Infrastructure.Enums;


namespace DeepIdentity.Facade
{
    public class UserFacade// : UserFacade
    {

        public UserDA UserDA { get; set; }


        public UserFacade()
        {
            UserDA = new UserDA();


        }


        #region UserFacade Members

       

   
      

       


      
      
        
        public IList<TabMenu> GetMenusByUserRole(int RoleId)
        {
            try
            {

                return UserDA.GetMenusByUserRole(RoleId);
            }
            catch (Exception)
            {

                throw;
            }
        }
       
       
       
       

      
        public IList<User> GetAllManagers(string _tenantId)
        {
            return UserDA.GetAllManagers(_tenantId);
        }

        
       
       
    
        public User GetUser(int _id)
        {
            try
            {
                return UserDA.GetUser(_id); ;
            }
            catch (DomainException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }
        
      

        public User GetUserByName(string name)
        {
            try
            {

                User user = UserDA.GetUserByName(name);
                if (user != null)
                {
                    user.Password = RSAHelper.DecryptData(user.Password);
                    return user;
                }
                else
                    return null;
            }
            catch (DomainException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }

        public User GetUserByNRIC(string nric)
        {
            try
            {

                return UserDA.GetUserByNRIC(nric);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

       

        public User GetUserByEmployeeId(string employeeId)
        {
            try
            {

                return UserDA.GetUserByEmployeeId(employeeId);
            }
            catch (DomainException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }


      

       
        public void UpdateManagerForUsers()
        {
            try
            {
                UserDA.UpdateManagerForUsers();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int InsertUser(User user)
        {
            try
            {
               return UserDA.InsertNewUser(user);
            }
            catch (Exception)
            {
                    
                throw;
            }
        }

        public void UpdateUser(User user)
        {
            try
            {
                UserDA.UpdateUser(user);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool IsEmployeeIdAlreadyExists(string _employeeno)
        {
            var getUser = this.GetAllUsers().Where(ge => ge.EmployeeId == _employeeno);

            if (getUser == null)
                return true;

            return false;
        }

        public void DeleteUser(User user)
        {
            try
            {
                UserDA.DeleteUser(user);
            }
            catch (Exception)
            {
                throw;
            }
        }   

        public IList<User> GetAllUserOfOrganisation(UserType _userType, IList<Organisation> _list)
        {

            IList<User> retList = null;

            try
            {

                if (null == _list)
                    return null;


                retList = new List<User>();

                IList<User> allList = new List<User>();

                foreach (Organisation organisation in _list)
                {

                    retList = UserDA.GetAllUserOfOrganisation((object)organisation.OrgUnitPrefix, _userType);



                    if (retList.Count > 0)
                        allList = (IList<User>)allList.Union(retList).ToList();
                }


                return allList;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<User> GetAllUsersOfCompany(string CompanyCode)
        {
            try
            {
                return UserDA.GetAllUsersOfCompany(CompanyCode);
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
                return UserDA.GetAllUsersOfSection(SectionCode);
            }
            catch (Exception)
            {
                throw;
            }
        }


      
        public IList<User> GetAllUserOfManager(int _managerId)
        {

            try
            {
                if (_managerId == -1)
                    return null;


                return UserDA.GetAllUserOfManager(_managerId);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<User> GetAllUsers()
        {
            return UserDA.GetAllUsers();
        }

        public IList<User> GetAllUser(UserType _userType, IList<Group> _list)
        {

            IList<User> retList = new List<User>();

            try
            {

                if (null == _list)
                    return null;

                IList<User> allList = new List<User>();

                foreach (Group grp in _list)
                {
                    retList = UserDA.GetAllUserOfGroup(grp.GroupId, _userType);



                    if (retList.Count > 0)
                        allList = (IList<User>)allList.Union(retList).ToList();

                }






                return allList;

            }
            catch (Exception)
            {

                throw;
            }

        }
        public User GetManager(int _userId)
        {
            try
            {
                User user = this.GetUser(_userId);

                if (user == null)
                    return null;

                if (!user.ManagerId.HasValue)
                    return null;

                return this.GetUser(Convert.ToInt32(user.ManagerId));
            }
            catch (Exception)
            {

                throw;
            }
        }

    

        

        public void HandleServiceAccount(User _user)
        {
            ActiveDirectoryFacade adFacade = new ActiveDirectoryFacade();
            LotusNotesFacade lnFacade = new LotusNotesFacade();
            SAPFacade sapFacade = new SAPFacade();
            SAPEPFacade sapepFacade = new SAPEPFacade();

            int sectionHead = this.GetSectionHeadByUser(_user);

            try
            {
                IList<ADSUserInfo> adUsers = adFacade.GetAdsUsers(_user.UserId);

                if (adUsers.Count > 0)
                {
                    foreach (ADSUserInfo adUser in adUsers)
                    {
                        if (adUser.AccountType == AccountType.Service || adUser.AccountType == AccountType.Shared)
                        {
                            if (adUser.IfUnmappedMapTo == "HOD")
                            {
                                adUser.DIUserId = sectionHead;
                                DILog.SchedulerLogger.WriteLog(LogLevelType.Warning, string.Format("AD Account {0}  reassigned to Manager of {1}", adUser.sAMAccountName, _user.DisplayName), null, "Scheduler", 0, adUsers.Count, "Scheduler Service");
                            }
                            else if (string.IsNullOrEmpty(adUser.IfUnmappedMapTo))
                            {
                                adUser.DIUserId = null;
                                DILog.SchedulerLogger.WriteLog(LogLevelType.Warning, string.Format("AD Account {0} mapping is removed because the owner {1} is not found", adUser.sAMAccountName, _user.DisplayName), null, "Scheduler", 0, adUsers.Count, "Scheduler Service");
                            }
                            adFacade.UpdateUser(adUser);
                        }
                    }
                }

                IList<LotusUserInfo> lnUsers = lnFacade.GetLNSUserInfoByDIUserId(_user.UserId);

                if (lnUsers.Count > 0)
                {
                    foreach (LotusUserInfo lnUser in lnUsers)
                    {
                        if (lnUser.AccountType == AccountType.Service || lnUser.AccountType == AccountType.Shared)
                        {
                            if (lnUser.IfUnmappedMapTo == "HOD")
                            {
                                lnUser.DIUserId = sectionHead;
                                DILog.SchedulerLogger.WriteLog(LogLevelType.Warning, string.Format("Lotus Notes Account {0}  reassigned to Manager of {1}", lnUser.InternetAddress, _user.DisplayName), null, "Scheduler", 0, lnUsers.Count, "Scheduler Service");
                            }
                            else if (string.IsNullOrEmpty(lnUser.IfUnmappedMapTo))
                            {
                                lnUser.DIUserId = null;
                                DILog.SchedulerLogger.WriteLog(LogLevelType.Warning, string.Format("Lotus Notes Account {0} mapping is removed because the owner {1} is not found", lnUser.InternetAddress, _user.DisplayName), null, "Scheduler", 0, lnUsers.Count, "Scheduler Service");
                            }
                            lnFacade.UpdateUser(lnUser);
                        }
                    }
                }

                IList<SAPUserinfo> sapUsers = sapFacade.GetSAPUserInfoByDIUserId(_user.UserId);

                if (sapUsers.Count > 0)
                {
                    foreach (SAPUserinfo sapUser in sapUsers)
                    {
                        if (sapUser.AccountType == AccountType.Service || sapUser.AccountType == AccountType.Shared)
                        {
                            if (sapUser.IfUnmappedMapTo == "HOD")
                            {
                                sapUser.DIUserId = sectionHead;
                                DILog.SchedulerLogger.WriteLog(LogLevelType.Warning, string.Format("SAP Account {0}  reassigned to Manager of {1}", sapUser.AccountName, _user.DisplayName), null, "Scheduler", 0, sapUsers.Count, "Scheduler Service");
                            }
                            else if (string.IsNullOrEmpty(sapUser.IfUnmappedMapTo))
                            {
                                sapUser.DIUserId = null;
                                DILog.SchedulerLogger.WriteLog(LogLevelType.Warning, string.Format("SAP Account {0} mapping is removed because the owner {1} is not found", sapUser.AccountName, _user.DisplayName), null, "Scheduler", 0, sapUsers.Count, "Scheduler Service");
                            }
                            sapFacade.UpdateSAPUser(sapUser);
                        }
                    }
                }

                IList<SAPEPUserInfo> sapepUsers = sapepFacade.GetSAPEPUserInfoByDIUserId(_user.UserId);

                if (sapepUsers.Count > 0)
                {
                    foreach (SAPEPUserInfo sapepUser in sapepUsers)
                    {
                        if (sapepUser.AccountType == AccountType.Service || sapepUser.AccountType == AccountType.Shared)
                        {
                            if (sapepUser.IfUnmappedMapTo == "HOD")
                            {
                                sapepUser.DIUserId = sectionHead;
                                DILog.SchedulerLogger.WriteLog(LogLevelType.Warning, string.Format("SAPEP Account {0}  reassigned to Manager of {1}", sapepUser.UserName, _user.DisplayName), null, "Scheduler", 0, sapepUsers.Count, "Scheduler Service");
                            }
                            else if (string.IsNullOrEmpty(sapepUser.IfUnmappedMapTo))
                            {
                                sapepUser.DIUserId = null;
                                DILog.SchedulerLogger.WriteLog(LogLevelType.Warning, string.Format("SAPEP Account {0} mapping is removed because the owner {1} is not found", sapepUser.UserName, _user.DisplayName), null, "Scheduler", 0, sapepUsers.Count, "Scheduler Service");
                            }
                            sapepFacade.UpdateUser(sapepUser);
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
       
       
       

        public IList<User> GetAllActiveEmployees()
        {
            try
            {
                return UserDA.GetAllActiveEmployees();

            }
            catch (Exception)
            {
                throw;
            }
        }

        
        public string GetEmployeeId(string DefaultEmployeeKey)
        {
            try
            {
                return UserDA.GetEmployeeId(DefaultEmployeeKey);
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
                return UserDA.GetCompanyDetails(prefix);
            }
            catch (Exception)
            {
                throw;
            }
        }
       
      
       
       
   
       
        #endregion

 

       

     
      
       
      
       
        public IList<User> GetFullTimeEmployees()
        {
            return UserDA.GetFullTimeEmployees();
        }

        

       
    

       
      

        public void UpdateLastLogonInformation(User User)
        {
            UserDA.UpdateLastLogonInformation(User);
        }

     

        #region SECOND LEVEL ATTESTATION

       

        public IList<SAPRoleInfo> GetAllSAPSingleRolesByDIUserId(int userId)
        {
            try
            {
                return UserDA.GetAllSAPSingleRolesByDIUserId(userId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<SAPRoleInfo> GetAllSAPRoleCodesByDIUserIdForAttestation(int userId)
        {
            try
            {
                return UserDA.GetAllSAPRoleCodesByDIUserIdForAttestation(userId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

      

    

      
      

        public bool UpdateUserName(User usr)
        {
            return UserDA.UpdateUserName(usr);
        }

        public DataSet GetAllUsersByGroup(string groupName, int endpointId)
        {
            try
            {
                //return UserDA.GetAllUsersByGroup(groupName, endpointId);
                return null;
            }
            catch (Exception)
            {

                throw;
            }

        }





        public void GetAllMasterUsersSAP(IList<SAPUserinfo> listsapUser, bool mapAll)
        {
            try
            {
                DataRow[] dRowsMaster = null;

                Log.Discovery.WriteMessage(LogLevel.INFO, string.Format("Started processing Master user(s) objects"));



                IList<User> users = new List<User>();
                IList<ConfigGeneralTrustedSource> listTrustedConfig = null;
                //IList<ConfigGeneralTrustedSource> list = DiConfigurationDA.GetAllConfigTrustedSource();


                //listTrustedConfig = list.Where(li => li.TrustedSource == Constants.SAP).ToList();


                if (listTrustedConfig.Count <= 0)
                {
                    Log.Discovery.WriteMessage(LogLevel.INFO, string.Format("Populate User: No attribute is configured from SAP"));
                    return;
                }


                string tenantId = string.Empty;
                if (listsapUser.Count > 0)
                {
                    tenantId = listsapUser.First().TenantId;
                }

                IList<User> listUsers = new List<User>();

                if (tenantId != string.Empty)
                    listUsers = UserDA.GetAllUsers(tenantId);
                else
                    listUsers = UserDA.GetAllUsers();

                foreach (SAPUserinfo sapuser in listsapUser)
                {

                    IList<User> existingMasteruser = (from usr in listUsers where usr.UserId == sapuser.DIUserId select usr).ToList();
                    IList<SAPUserinfo> sapusr = (from sapu in listsapUser where sapu.UserLogonName == sapuser.UserLogonName select sapu).ToList();


                    if (existingMasteruser != null && existingMasteruser.Count() > 0)
                    {



                        DataTable dtMaster1 = CollectionHelper.ConvertToNormalTable<User>(existingMasteruser);
                        DataTable dtSapUser1 = CollectionHelper.ConvertToNormalTable<SAPUserinfo>(sapusr);


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

                            else //create new user
                            {



                            }
                        }


                        IList<User> masterusr = CollectionHelper.ConvertTo<User>(dtMaster1);

                        using (IACMDBContext oIACMDBContext = new IACMDBContext())
                        {

                            var original = oIACMDBContext.Users.Find(masterusr.FirstOrDefault().UserId);

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


       
     

        public List<User> GetOnBoardingUsersForAdminCheck()
        {
            try
            {
                return UserDA.GetOnBoardingUsers(true);
            }
            catch (Exception)
            {
                throw;
            }
        }
       
       
        public List<User> GetOnBoardingUsersForAutoProcess()
        {
            try
            {
                return UserDA.GetOnBoardingUsers(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<User> GetOffBoardingUsersForAutoProcess()
        {
            try
            {
                return UserDA.GetOffBoardingUsers();
            }
            catch (Exception)
            {
                throw;
            }
        }

       

        public List<User> GetOUChangeUsersForAdminCheck()
        {
            try
            {
                List<User> users = new List<User>();

                var triggers = new TriggerDA().GetOUChangeUsers(true);

                foreach (EmployeeEventTrigger trigger in triggers)
                {
                    users.Add(UtilityMethods.Deserialize<User>(trigger.UserDetails));
                }

                return users;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<User> GetOUChangeUsersForAutoProcess()
        {
            try
            {
                List<User> users = new List<User>();

                var triggers = new TriggerDA().GetOUChangeUsers(false);

                foreach (EmployeeEventTrigger trigger in triggers)
                {
                    users.Add(UtilityMethods.Deserialize<User>(trigger.UserDetails));
                }

                return users;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<User> GetReOrgUsersForAutoProcess()
        {
            try
            {
                List<User> users = new List<User>();

                var triggers = new TriggerDA().GetReOrgUsers();

                foreach (EmployeeEventTrigger trigger in triggers)
                {
                    users.Add(UtilityMethods.Deserialize<User>(trigger.UserDetails));
                }

                return users;

            }
            catch (Exception)
            {
                throw;
            }
        }

  


       
       

   

    

      


   

      

        public void UpdateUserFromDiscovery(User user, List<string> propagationFields, bool canRunPropagationTrigger)
        {
            try
            {
                //propagate trusted source changes
                this.InitiatePropagateTrigger(user, propagationFields, canRunPropagationTrigger);

                UserDA.UpdateUserFromDiscovery(user);
            }
            catch
            {
                throw;
            }
        }

        private void InitiatePropagateTrigger(User user, List<string> propagationFields, bool canRunPropagationTrigger)
        {
            List<string> listAttributesToBePropagated = new List<string>();

            foreach (string item in propagationFields)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    listAttributesToBePropagated.Add(item);
                }
            }

            var existingUser = UserDA.GetUser(user.UserId);

            bool isPropagationRequired = false;

            foreach (string attribute in listAttributesToBePropagated)
            {
                PropertyInfo trsuserpropertyInfo = user.GetType().GetProperty(attribute);
                string newValue = string.Empty;
                string dt = string.Empty;
                if (trsuserpropertyInfo.GetValue(user) != null)
                    newValue = trsuserpropertyInfo.GetValue(user).ToString();

                PropertyInfo userpropertyInfo = existingUser.GetType().GetProperty(attribute);
                string oldValue = string.Empty;

                if (userpropertyInfo.GetValue(existingUser) != null)
                    oldValue = userpropertyInfo.GetValue(existingUser).ToString();

                if (string.Compare(oldValue, newValue, true) != 0)
                {
                    Type userPropType = userpropertyInfo.PropertyType;

                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format("Changes found in Trusted source Attribute : {0} : For User {1} - Old Value : {2} ; New Value {3}", attribute, existingUser.DisplayName, oldValue, newValue), null, ProvisioningEventType.Others, "Provisioning Engine");

                    isPropagationRequired = true;
                }
            }

            if (isPropagationRequired && canRunPropagationTrigger)
            {
                this.CreatePropagationTrigger(user);
            }
        }

        public void CreatePropagationTrigger(User user)
        {
            EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
            {
                CreatedBy = user.CreatedBy,
                CreatedDate = DateTime.Today,
                EffectiveDate = DateTime.Today,
                EventType = EmployeeEvents.Propagation,
                IsRequestCreated = false,
                TenantId = user.TenantId,
                UserId = user.UserId,
                IsActive = true
            };

            new TriggerDA().CreateEmployeeEventTrigger(newTrigger);
        }

       

       

    
       
        public int GetManagerFromOrg(User user)
        {
            OrganisationFacade orgFacade = new OrganisationFacade();

            string approver = orgFacade.CheckForApproverInException(user.EmployeeId, 1);

            if (!string.IsNullOrEmpty(approver))
            {
                var appUser = new UserFacade().GetUserByEmployeeId(approver);
                if (appUser != null)
                {
                    return appUser.UserId;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                var section = orgFacade.GetOrganisationByCode(user.SectionCode);

                if (section != null)
                {
                    while (true)
                    {

                        if (section == null)
                        {
                            return 0;
                        }

                        if (section.Head.HasValue && section.Head.Value != user.UserId)
                        {
                            return section.Head.Value;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(section.ParentOrganisation))
                            {
                                section = orgFacade.GetOrganisationByCode(section.ParentOrganisation);
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        public int GetSectionHeadByUser(User user)
        {
            OrganisationFacade orgFacade = new OrganisationFacade();

            string approver = orgFacade.CheckForApproverInException(user.EmployeeId, 1);

            if (!string.IsNullOrEmpty(approver))
            {
                var appUser = new UserFacade().GetUserByEmployeeId(approver);
                if (appUser != null)
                {
                    return appUser.UserId;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                var section = orgFacade.GetOrganisationByCode(user.SectionCode);

                if (section != null)
                {
                    while (true)
                    {

                        if (section.Head.HasValue && section.Head.Value != user.UserId)
                        {
                            if (section.Scope.StartsWith("40") || section.Scope.StartsWith("30") || section.Scope.StartsWith("20") ||
                                section.Scope.StartsWith("10") || section.Scope.StartsWith("00"))
                            {
                                return section.Head.Value;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(section.ParentOrganisation))
                                {
                                    section = orgFacade.GetOrganisationByCode(section.ParentOrganisation);
                                }
                                else
                                {
                                    return 0;
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(section.ParentOrganisation))
                            {
                                section = orgFacade.GetOrganisationByCode(section.ParentOrganisation);
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        public int GetDivisionHeadByUser(User user)
        {
            OrganisationFacade orgFacade = new OrganisationFacade();

            string approver = orgFacade.CheckForApproverInException(user.EmployeeId, 2);

            if (!string.IsNullOrEmpty(approver))
            {
                var appUser = new UserFacade().GetUserByEmployeeId(approver);
                if (appUser != null)
                {
                    return appUser.UserId;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                var section = orgFacade.GetOrganisationByCode(user.SectionCode);

                if (section != null)
                {
                    while (true)
                    {
                        if (section.Head.HasValue && section.Head.Value != user.UserId)
                        {
                            if (section.Scope.StartsWith("20") || section.Scope.StartsWith("10") || section.Scope.StartsWith("00"))
                            {
                                return section.Head.Value;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(section.ParentOrganisation))
                                {
                                    section = orgFacade.GetOrganisationByCode(section.ParentOrganisation);
                                }
                                else
                                {
                                    return 0;
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(section.ParentOrganisation))
                            {
                                section = orgFacade.GetOrganisationByCode(section.ParentOrganisation);
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        public int GetDepartmentHeadByUser(User user)
        {
            OrganisationFacade orgFacade = new OrganisationFacade();

            var section = orgFacade.GetOrganisationByCode(user.SectionCode);
            if (section != null)
            {
                while (true)
                {
                    if (section.Head.HasValue && section.Head.Value != user.UserId)
                    {
                        if (section.Scope.StartsWith("10") || section.Scope.StartsWith("00"))
                        {
                            return section.Head.Value;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(section.ParentOrganisation))
                            {
                                section = orgFacade.GetOrganisationByCode(section.ParentOrganisation);
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(section.ParentOrganisation))
                        {
                            section = orgFacade.GetOrganisationByCode(section.ParentOrganisation);
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
            }
            return 0;
        }


        public void InsertUserRole(int UserID, int RoleId)
        {
            UserDA.InsetUserRole(UserID, RoleId);
        }
       
       

      

        public string GetOrganisationUnitPrefixByOrgId(int? OrgID)
        {
            return UserDA.GetOrganisationUnitPrefixByOrgId(OrgID);
        }

       

        public bool CheckUserGroup(int UserID, int GroupID)
        {
            try
            {
                return UserDA.CheckUserGroup(UserID, GroupID);
            }
            catch
            {
                throw;
            }
        }
        public string GetEndPointUserDetailByEndpointUserID(int EndpointUserID, string EndpointSystemName)
        {
            return UserDA.GetEndPointUserDetailByEndpointUserID(EndpointUserID, EndpointSystemName);
        }

       

       
      
       

        public List<User> GetUsersToUpdateEmailIdInHR()
        {
            try
            {
                return UserDA.GetUsersToUpdateEmailIdInHR();
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
                return UserDA.GetUsersToUpdateFLINSLoginIdInHR();
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
                return UserDA.GetDesignations();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CheckUserNameExist(string username)
        {
            try
            {
                return UserDA.CheckUserNameExist(username);
            }
            catch (Exception)
            {
                throw;
            }
        }

       
     
       

        public int GetAutoGeneratedNRICCount()
        {
            try
            {
                return UserDA.GetAutoGeneratedNRICCount();
            }
            catch
            {
                throw;
            }
        }


        public Dictionary<int, string> GetEndpointUser(EndpointType epType, int UserId, int EndPointId, AccountType accountType)
        {
            if (epType == EndpointType.ActiveDirectory)
            {
                return UserDA.GetADUsersByDIUserId(UserId, EndPointId, accountType);
            }
            else if (epType == EndpointType.LotusNotes)
            {
                return UserDA.GetLNSUsersByDIUserId(UserId, EndPointId, accountType);
            }
            else if (epType == EndpointType.SAP)
            {
                return UserDA.GetSAPUsersByDIUserId(UserId, EndPointId, accountType);
            }
            else if (epType == EndpointType.OracleEBS)
            {
                return UserDA.GetOEBSUsersByDIUserId(UserId, EndPointId, accountType);
            }
            else if (epType == EndpointType.Linux)
            {
                return UserDA.GetLinuxUsersByDIUserId(UserId, EndPointId, accountType);
            }
            else
            {
                return UserDA.GetADUsersByDIUserId(UserId, EndPointId, accountType);
            }
        }

        public bool CheckSelfUpdateField(string FieldName)
        {
            return UserDA.CheckSelfUpdateField(FieldName);
        }


        #region AddedByConnectorTeam

        public void ValidateAndCreateTriggers(List<EmployeeEventTrigger> triggers, DateTime lastRunDate)
        {
            if (triggers.Count > 0)
            {
                var onBoards = triggers.Where(item => item.EventType == EmployeeEvents.NewJoinee).ToList();
                var offBoards = triggers.Where(item => item.EventType == EmployeeEvents.Resignation).ToList();
                var ouChanges = triggers.Where(item => item.EventType == EmployeeEvents.OUChange).ToList();
                var reOrgs = triggers.Where(item => item.EventType == EmployeeEvents.ReOrganisation).ToList();

                foreach (EmployeeEventTrigger trigger in ouChanges)
                {
                    try
                    {
                        this.CreateEmployeeEventTrigger(trigger);
                    }
                    catch (Exception ex)
                    {
                        var user = UtilityMethods.Deserialize<User>(trigger.UserDetails);
                        DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Error, "Error while creating transfer trigger for user - " + user.DisplayName, ex, DiscoveryObjectType.None, 0);
                    }
                }

                foreach (EmployeeEventTrigger trigger in reOrgs)
                {
                    try
                    {
                        this.CreateEmployeeEventTrigger(trigger);
                    }
                    catch (Exception ex)
                    {
                        var user = UtilityMethods.Deserialize<User>(trigger.UserDetails);
                        DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Error, "Error while creating re-org trigger for user - " + user.DisplayName, ex, DiscoveryObjectType.None, 0);
                    }
                }

                foreach (EmployeeEventTrigger trigger in onBoards)
                {
                    try
                    {
                        this.CreateEmployeeEventTrigger(trigger);

                        //active offboarding event exists for same user then disable off board event.
                        var offBoardTrigger = this.GetEmployeeEventTrigger(trigger.UserId, EmployeeEvents.Resignation);
                        bool isOffBoardExists = offBoardTrigger != null;

                        if (isOffBoardExists)
                        {
                            offBoardTrigger.IsActive = false;
                            this.UpdateEmployeeEventTrigger(offBoardTrigger);
                        }

                        var postResignationTrigger = this.GetEmployeeEventTrigger(trigger.UserId, EmployeeEvents.PostResignation);
                        bool isPostResignationEventExists = postResignationTrigger != null;

                        if (isPostResignationEventExists)
                        {
                            postResignationTrigger.IsActive = false;
                            this.UpdateEmployeeEventTrigger(postResignationTrigger);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Error, "Error while creating on-board trigger", ex, DiscoveryObjectType.None, 0);
                    }
                }

                foreach (EmployeeEventTrigger trigger in offBoards)
                {
                    try
                    {
                        //onboarding event exists for same user then don't create off board event.
                        bool isOnBoardExists = this.GetEmployeeEventTrigger(trigger.UserId, EmployeeEvents.NewJoinee, lastRunDate) != null;

                        if (!isOnBoardExists)
                        {
                            this.CreateEmployeeEventTrigger(trigger);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Error, "Error while creating off-board trigger", ex, DiscoveryObjectType.None, 0);
                    }
                }
            }
        }


        public void ValidateAndCreateTriggers(List<EmployeeEventTrigger> triggers)
        {
            if (triggers.Count > 0)
            {
                var onBoards = triggers.Where(item => item.EventType == EmployeeEvents.NewJoinee).ToList();
                var offBoards = triggers.Where(item => item.EventType == EmployeeEvents.Resignation).ToList();
                var ouChanges = triggers.Where(item => item.EventType == EmployeeEvents.OUChange).ToList();
                var reOrgs = triggers.Where(item => item.EventType == EmployeeEvents.ReOrganisation).ToList();

                foreach (EmployeeEventTrigger trigger in ouChanges)
                {
                    try
                    {
                        this.CreateEmployeeEventTrigger(trigger);
                    }
                    catch (Exception ex)
                    {
                        var user = UtilityMethods.Deserialize<User>(trigger.UserDetails);
                        DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Error, "Error while creating transfer trigger for user - " + user.DisplayName, ex, DiscoveryObjectType.None, 0);
                    }
                }

                foreach (EmployeeEventTrigger trigger in reOrgs)
                {
                    try
                    {
                        this.CreateEmployeeEventTrigger(trigger);
                    }
                    catch (Exception ex)
                    {
                        var user = UtilityMethods.Deserialize<User>(trigger.UserDetails);
                        DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Error, "Error while creating re-org trigger for user - " + user.DisplayName, ex, DiscoveryObjectType.None, 0);
                    }
                }

                foreach (EmployeeEventTrigger trigger in onBoards)
                {
                    try
                    {
                        this.CreateEmployeeEventTrigger(trigger);

                        //active offboarding event exists for same user then disable off board event.
                        var offBoardTrigger = this.GetEmployeeEventTrigger(trigger.UserId, EmployeeEvents.Resignation);
                        bool isOffBoardExists = offBoardTrigger != null;

                        if (isOffBoardExists)
                        {
                            offBoardTrigger.IsActive = false;
                            this.UpdateEmployeeEventTrigger(offBoardTrigger);
                        }

                        var postResignationTrigger = this.GetEmployeeEventTrigger(trigger.UserId, EmployeeEvents.PostResignation);
                        bool isPostResignationEventExists = postResignationTrigger != null;

                        if (isPostResignationEventExists)
                        {
                            postResignationTrigger.IsActive = false;
                            this.UpdateEmployeeEventTrigger(postResignationTrigger);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Error, "Error while creating on-board trigger", ex, DiscoveryObjectType.None, 0);
                    }
                }

                foreach (EmployeeEventTrigger trigger in offBoards)
                {
                    try
                    {
                        //onboarding event exists for same user then don't create off board event.
                        bool isOnBoardExists = this.GetEmployeeEventTrigger(trigger.UserId, EmployeeEvents.NewJoinee) != null;

                        if (!isOnBoardExists)
                        {
                            this.CreateEmployeeEventTrigger(trigger);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Error, "Error while creating off-board trigger", ex, DiscoveryObjectType.None, 0);
                    }
                }
            }
        }



        public void CreateEmployeeEventTrigger(EmployeeEventTrigger trigger)
        {
            try
            {
                UserDA.CreateEmployeeEventTrigger(trigger);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public EmployeeEventTrigger GetEmployeeEventTrigger(int userId, EmployeeEvents evnt)
        {
            try
            {
                return UserDA.GetEmployeeEventTrigger(userId, evnt);
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
                UserDA.UpdateEmployeeEventTrigger(trigger);
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while updating  the User", ex);
                throw ex;
            }
        }

        public EmployeeEventTrigger GetEmployeeEventTriggerToExecute(int userId, EmployeeEvents evnt)
        {
            try
            {
                return UserDA.GetEmployeeEventTriggerToExecute(userId, evnt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeEventTrigger GetEmployeeEventTrigger(int userId, EmployeeEvents evnt, DateTime date)
        {
            try
            {
                return UserDA.GetEmployeeEventTrigger(userId, evnt, date);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        #endregion
    }
}
