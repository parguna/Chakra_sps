using AutoMapper;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Core;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.ProvisioningUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("User")]
    public class UserController : BaseController<DomainModel.User, EntityModel.User, Dto.User>
    {
        [HttpGet]
        [Route("GetUsersForBulkRequest")]
        public Dictionary<long, string> GetUsersForBulkRequest(string searchString, bool IsFulltimeEmployee, string RosEmpId)
        {
            return DomainModel.User.GetUsersForBulkRequest(searchString, IsFulltimeEmployee, RosEmpId);
        }


        [HttpGet]
        [Route("GetSameOUUserIDs")]
        public List<long> GetSameOUUserIDs(long DiUserId)
        {
            return DomainModel.User.GetSameOUUserIDs(DiUserId);
        }

        [HttpGet]
        [Route("GetOUChangeUsersForAdminCheck")]
        public List<DeptChangedUserGridData> GetOUChangeUsersForAdminCheck()
        {
            return DomainModel.EmployeeEventTrigger.GetOUChangeUsersForAdminCheck().Select(d => Mapper.Map<DeptChangedUserGridData>(d)).ToList();
        }

        [HttpGet]
        [Route("GetReOrgUsersForAdminCheck")]
        public List<DeptChangedUserGridData> GetReOrgUsersForAdminCheck()
        {
            return DomainModel.EmployeeEventTrigger.GetReOrgUsersForAdminCheck().Select(d => Mapper.Map<DeptChangedUserGridData>(d)).ToList();
        }

        [HttpGet]
        [Route("GetOnBoardUsersForAdminCheck")]
        public List<DeptChangedUserGridData> GetOnBoardUsersForAdminCheck()
        {
            return DomainModel.EmployeeEventTrigger.GetOnBoardUsersForAdminCheck().Select(d => Mapper.Map<DeptChangedUserGridData>(d)).ToList();
        }

        [HttpGet]
        [Route("GetReHireUsersForAdminCheck")]
        public List<DeptChangedUserGridData> GetReHireUsersForAdminCheck()
        {
            return DomainModel.EmployeeEventTrigger.GetReHireUsersForAdminCheck().Select(d => Mapper.Map<DeptChangedUserGridData>(d)).ToList();
        }

        [HttpPost]
        [Route("IsProvisioningDefaultExists")]
        public string IsProvisioningDefaultExists(EmployeeEvents events, [FromBody] Dto.User _user)
        {
            DomainModel.User user = DtoUnMap(_user); //DomainModel.User.GetById(userId, false);
            return ProvisioningValidator.IsProvisioningDefaultExists(user, events);
        }

        [HttpGet]
        [Route("IsADDefaultsExists")]
        public string IsADDefaultsExists(EmployeeEvents events, long userId)
        {
            DomainModel.User user = DomainModel.User.GetById(userId, false);
            return ProvisioningValidator.IsADDefaultsExists(user);
        }

        [HttpGet]
        [Route("IsSMSDefaultsExists")]
        public string IsSMSDefaultsExists(EmployeeEvents events, long userId)
        {
            DomainModel.User user = DomainModel.User.GetById(userId, false);
            return ProvisioningValidator.IsSMSDefaultsExists(user);
        }

        [HttpGet]
        [Route("IsFLINSDefaultsExists")]
        public string IsFLINSDefaultsExists(EmployeeEvents events, long userId)
        {
            DomainModel.User user = DomainModel.User.GetById(userId, false);
            return ProvisioningValidator.IsFLINSDefaultsExists(user);
        }



        [HttpGet]
        [Route("GetROByUser")]
        public User GetROByUser(long id)
        {
            return DtoMap(DomainModel.User.GetROByUser(id));
        }
        [HttpGet]
        [Route("GetCOByUser")]
        public User GetCOByUser(long id)
        {
            return DtoMap(DomainModel.User.GetCOByUser(id));
        }

        [HttpGet]
        [Route("GetByIdIncludeBusinessRoleAndOrganisation")]
        public User GetByIdIncludeBusinessRoleAndOrganisation(long id)
        {
            return DtoMap(DomainModel.User.GetByIdIncludeBusinessRoleAndOrganisation(id));
        }

        [HttpGet]
        [Route("GetByUserName")]
        public User GetByUserName(string name)
        {
            return DtoMap(DomainModel.User.GetByUserName(name));
        }
        [HttpGet]
        [Route("GetActiveByUserName")]
        public User GetActiveByUserName(string name)
        {
            return DtoMap(DomainModel.User.GetActiveByUserName(name));
        }
        [HttpGet]
        [Route("Authenticate")]
        [AllowAnonymous]
        public Boolean Authenticate(string userName, string password)
        {
            return DomainModel.User.Authenticate(userName, password);
        }



        [HttpGet]
        [Route("GetByNRIC")]
        public User GetByNRIC(string nric, int employeeType)
        {
            return DtoMap(DomainModel.User.GetByNRICByEmployeeType(nric, employeeType));
        }

        [HttpGet]
        [Route("GetByEmployeeId")]
        public User GetByEmployeeId(string employeeId)
        {
            return DtoMap(DomainModel.User.GetByEmployeeId(employeeId));
        }
        [HttpGet]
        [Route("GetIdByUserName")]
        public long GetIdByUserName(string userName)
        {
            return DomainModel.User.GetIdByUserName(userName);
        }

        [HttpPost]
        [Route("UpdateManagerForUsers")]
        public void UpdateManagerForUsers()
        {
            DomainModel.User.UpdateManagerForUsers();
        }

        [HttpGet]
        [Route("DoesEmployeeExists")]
        public bool DoesEmployeeExists(string username)
        {
            return DomainModel.User.DoesEmployeeExists(username);
        }

        [HttpGet]
        [Route("DoesIdentificationNumberExists")]
        public bool DoesIdentificationNumberExists(string identificationNumber, int employeeType)
        {
            return DomainModel.User.DoesIdentificationNumberExists(identificationNumber, employeeType);
        }

        [HttpGet]
        [Route("GetActiveEmployees")]
        public List<User> GetActiveEmployees()
        {
            return DtoMapList(DomainModel.User.GetActiveEmployees());
        }

        [HttpGet]
        [Route("GetEmployeeId")]
        public string GetEmployeeId(string searchString)
        {
            return DomainModel.User.GetEmployeeId(searchString);
        }

        [HttpGet]
        [Route("GetBySearchString")]
        public Dictionary<long, string> GetBySearchString(string searchString, bool hidden)
        {
            return DomainModel.User.GetBySearchString(searchString, hidden);
        }

        [HttpGet]
        [Route("GetROUsersBySearchString")]
        public Dictionary<long, string> GetROUsersBySearchString(string searchString)
        {
            return DomainModel.User.GetROUsersBySearchString(searchString);
        }

        [HttpGet]
        [Route("GetBySearchStringAndConsiderJoinDate")]
        public Dictionary<long, string> GetBySearchStringAndConsiderJoinDate(string searchString, bool hidden, bool considerJoinDate)
        {
            return DomainModel.User.GetBySearchStringAndConsiderJoinDate(searchString, hidden, considerJoinDate);
        }

        [HttpGet]
        [Route("GetCompanyDetails")]
        public User GetCompanyDetails(string prefix)
        {
            return DtoMap(DomainModel.User.GetCompanyDetails(prefix));
        }

        [HttpGet]
        [Route("GetFullTimeEmployees")]
        public List<User> GetFullTimeEmployees(bool fullTimeEmp, int startRowIndex, int maximumRows, string FilterExpression, string sortExpression)
        {
            return DtoMapList(DomainModel.User.GetFullTimeEmployees(fullTimeEmp, startRowIndex, maximumRows, FilterExpression, sortExpression));
        }

        [HttpGet]
        [Route("GetFullTimeEmployeesCount")]
        public long GetFullTimeEmployeesCount(bool fullTimeEmp)
        {
            return DomainModel.User.GetFullTimeEmployeesCount(fullTimeEmp);
        }

        [HttpPost]
        [Route("UpdateLastLogonInformation")]
        public void UpdateLastLogonInformation([FromBody]User user)
        {
            DomainModel.User.UpdateLastLogonInformation(DtoUnMap(user));
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("WriteLoginResult")]
        public bool WriteLoginResult(string userName, string message)
        {
            DomainModel.User.WriteLoginResult(userName, message);
            return true;
        }


        [HttpGet]
        [Route("GetOnBoardingUsers")]
        public List<User> GetOnBoardingUsers(bool forAdminCheck)
        {
            return DtoMapList(DomainModel.User.GetOnBoardingUsers(forAdminCheck));
        }

        [HttpGet]
        [Route("GetBackkDatedExceptions")]
        public List<User> GetBackkDatedExceptions(int onboardBackdatedWindowperiod, DateTime? fromDate, DateTime? toDate)
        {
            return DtoMapList(DomainModel.User.GetBackkDatedExceptions(onboardBackdatedWindowperiod, fromDate, toDate));
        }


        [HttpGet]
        [Route("GetOffBoardingUsers")]
        public List<User> GetOffBoardingUsers()
        {
            return DtoMapList(DomainModel.User.GetOffBoardingUsers());
        }

        [HttpGet]
        [Route("GetNotifiableUsers")]
        public List<User> GetNotifiableUsers(long taskId, long requestId)
        {
            return DtoMapList(DomainModel.User.GetNotifiableUsers(taskId, requestId));
        }

        [HttpGet]
        [Route("GetOUChangeUsers")]
        public List<User> GetOUChangeUsers()
        {
            return DtoMapList(DomainModel.User.GetOUChangeUsers());
        }

        [HttpGet]
        [Route("GetReOrgUsers")]
        public List<User> GetReOrgUsers()
        {
            return DtoMapList(DomainModel.User.GetReOrgUsers());
        }

        [HttpPost]
        [Route("UpdateUserFromDiscovery")]
        public void UpdateUserFromDiscovery(User user, List<string> propagationFields, bool canRunPropagationTrigger)
        {
            DomainModel.User.UpdateUserFromDiscovery(DtoUnMap(user), propagationFields, canRunPropagationTrigger, User.Identity.Name);
        }


        [HttpPost]
        [Route("GetManagerFromOrg")]
        public long GetManagerFromOrg([FromBody]User user)
        {
            return DomainModel.User.GetNManagerFromOrg(DtoUnMap(user));
        }
        [HttpPost]
        [Route("GetNManagerFromOrg")]
        public long GetNManagerFromOrg([FromBody]User user)
        {
            return DomainModel.User.GetNManagerFromOrg(DtoUnMap(user));
        }
        [HttpGet]
        [Route("GetSectionHeadByUser")]
        public long GetSectionHeadByUser([FromBody]User user)
        {
            return 0;
        }

        [HttpGet]
        [Route("GetDivisionHeadByUser")]
        public long GetDivisionHeadByUser([FromBody]User user)
        {
            return 0;
        }

        [HttpGet]
        [Route("GetDepartmentHeadByUser")]
        public long GetDepartmentHeadByUser([FromBody]User user)
        {
            return 0;
        }


        [HttpGet]
        [Route("InsertUserRole")]
        public void InsertUserRole(long userID, long roleID)
        {
            DomainModel.User.InsertUserRole(userID, roleID);
        }


        [HttpGet]
        [Route("CheckUserGroup")]
        public bool CheckUserGroup(long userID, long groupID)
        {
            return DomainModel.User.CheckUserGroup(userID, groupID);
        }

        [HttpGet]
        [Route("GetEndPointDetailByID")]
        public string GetEndPointDetailByID(long endpointUserID, string endpointSystemName)
        {
            return DomainModel.User.GetEndPointDetailByID(endpointUserID, endpointSystemName);
        }

        [HttpGet]
        [Route("GetAutoGeneratedNRICCount")]
        public long GetAutoGeneratedNRICCount()
        {
            return DomainModel.User.GetAutoGeneratedNRICCount();
        }

        [HttpGet]
        [Route("GetAssigneeInfo")]
        public List<User> GetAssigneeInfo(long? userId)
        {
            return DtoMapList(DomainModel.User.GetAssigneeInfo(userId));
        }

        [HttpPost]
        [Route("GetReportingEmployees")]
        public Dictionary<long, string> GetReportingEmployees([FromBody]Dto.Presentation.PresentationReportingEmployees reportingEmployees)
        {
            return DomainModel.User.GetReportingEmployees(AutoMapperHelper.DtoUnMap<Dto.Presentation.PresentationReportingEmployees, DomainModel.Presentation.PresentationReportingEmployees>(reportingEmployees));
        }

        [HttpGet]
        [Route("GetActiveAndNonTerminaters")]
        public Dictionary<long, string> GetActiveAndNonTerminaters(string searchString)
        {
            return DomainModel.User.GetActiveAndNonTerminaters(searchString);
        }

        [HttpGet]
        [Route("ConstructUserName")]
        public string ConstructUserName(string preferredName, string firstName, string lastName, string ethinicity, string employeeType)
        {
            return DomainModel.User.ConstructUserName(preferredName, firstName, lastName, employeeType);
        }

        [HttpGet]
        [Route("GetReferenceUser")]
        public Dictionary<string, string> GetReferenceUser(string prefix)
        {
            return DomainModel.User.GetReferenceUser(prefix);
        }
        [HttpGet]
        [Route("GetReferenceUserOnlyPermanent")]
        public Dictionary<string, string> GetReferenceUserOnlyPermanent(string prefix)
        {
            return DomainModel.User.GetReferencePermanentUser(prefix);
        }
        [HttpGet]
        [Route("GetReportingManager")]
        public Dictionary<long, string> GetReportingManager(string searchString)
        {
            return DomainModel.User.GetReportingManager(searchString);
        }

        [HttpGet]
        [Route("GetPosition")]
        public Dictionary<string, string> GetPosition(string prefix)
        {
            return DomainModel.User.GetPosition(prefix);
        }

        [HttpGet]
        [Route("GetCostCenters")]
        public Dictionary<long, string> GetCostCenters(string searchString, long companyId)
        {
            return DomainModel.User.GetCostCenters(searchString, companyId);
        }

        [HttpGet]
        [Route("GetOrganisationsWithPrefix")]
        public Dictionary<string, string> GetOrganisationsWithPrefix(string searchString)
        {
            return DomainModel.User.GetOrganisationsWithPrefix(searchString);
        }

        [HttpGet]
        [Route("GetNonReportingUsersOfManagerID")]
        public Dictionary<long, string> GetNonReportingUsersOfManagerID(long managerId, string searchString)
        {
            return DomainModel.User.GetNonReportingUsersOfManagerID(managerId, searchString);
        }


        [HttpGet]
        [Route("GetUsersByGroup")]
        public Dictionary<long, string> GetUsersByGroup(string searchString, string groupName)
        {
            return DomainModel.User.GetUsersByGroup(searchString, groupName);
        }

        [HttpGet]
        [Route("GetDisticntSectionFromUsers")]
        public Dictionary<string, string> GetDisticntSectionFromUsers(string searchString)
        {
            return DomainModel.User.GetDisticntSectionFromUsers(searchString);
        }

        [HttpGet]
        [Route("GetByIdReferenceUser")]
        public User GetByIdReferenceUser(long id)
        {
            return DtoMap(DomainModel.User.GetByIdReferenceUser(id));
        }

        [HttpGet]
        [Route("ForgotUser")]
        [AllowAnonymous]
        public User ForgotUser(string name)
        {
            return DtoMap(DomainModel.User.GetByUserName(name));
        }


        [HttpGet]
        [Route("ForgotPassword")]
        [AllowAnonymous]
        public User ForgotPassword(string userName, string nric, DateTime dob)
        {
            return DtoMap(DomainModel.User.GetByUserNric(userName, nric, dob));
        }

        [HttpPost]
        [Route("SaveUser")]
        [AllowAnonymous]
        public void SaveUser(User usr)
        {
            DomainModel.User.SaveUser(DtoUnMap(usr), User.Identity.Name);
        }

        [HttpPost]
        [Route("DoAfterSaveUser")]
        [AllowAnonymous]
        public void DoAfterSaveUser([FromBody]User usr)
        {
            DomainModel.User.DoAfterSaveUser(DtoUnMap(usr), User.Identity.Name);
        }

        [HttpGet]
        [Route("GetByUserId")]
        [AllowAnonymous]
        public User GetByUserId(long userId)
        {
            return DtoMap(DomainModel.User.GetByUserId(userId));
        }

        [HttpGet]
        [Route("GetGroupIds")]
        public List<long> GetGroupIds(long actionUserId)
        {
            return DomainModel.User.GetGroupId(actionUserId);
        }

        [HttpGet]
        [Route("GetByUsrId")]
        [AllowAnonymous]
        public User GetByUsrId(long userId)
        {
            return DtoMap(DomainModel.User.GetById(userId));
        }

        [HttpGet]
        [Route("UpdateUserADAccount")]
        public void UpdateUserADAccount(long userId, string sAMAccountName, long endpointsystemId)
        {
            DomainModel.User.UpdateUserADAccount(userId, sAMAccountName, endpointsystemId);
        }

        //[HttpGet]
        //[Route("ComposeNewUserCredentialMail")]
        //public bool ComposeNewUserCredentialMail(long userId)
        //{
        //    new EmailComposer().Compose(EmailTemplate.IACMNewUserNotification, userId);
        //    return true;
        //}

        [HttpGet]
        [Route("GetFullTimeUsers")]
        public Dictionary<long, string> GetFullTimeUsers(string searchString)
        {
            return DomainModel.User.GetFullTimeUsers(searchString);
        }

        [HttpGet]
        [Route("GetVendorUsers")]
        public Dictionary<long, string> GetVendorUsers(string searchString)
        {
            return DomainModel.User.GetVendorUsers(searchString);
        }

        [HttpGet]
        [Route("GetUsersForSupervisor")]
        public Dictionary<long, string> GetUsersForSupervisor(long managerId, string searchString)
        {
            return DomainModel.User.GetUsersForSupervisor(managerId, searchString);
        }

        [HttpGet]
        [Route("GetRequesteeInfo")]
        public List<string> GetRequesteeInfo(long userId, long endpointId)
        {
            return DomainModel.User.GetRequesteeInfo(userId, endpointId);
        }
        [HttpGet]
        [Route("IsUserAccount")]
        public bool IsUserAccount(long userID, long endPointSystemID, long EndpointSystemType)
        {
            return DomainModel.User.IsUserAccount(userID, endPointSystemID, EndpointSystemType, AccountType.User);

        }
        [HttpGet]
        [Route("GetPersonnelAreaById")]
        public Dto.PersonnelArea GetPersonnelAreaById(long id)
        {
            return Mapper.Map<Dto.PersonnelArea>(DomainModel.User.GetPersonnelAreaById(id));
        }
        [HttpGet]
        [Route("GetCompanyById")]
        public Dto.Company GetCompanyById(long id)
        {
            return Mapper.Map<Dto.Company>(DomainModel.User.GetCompanyById(id));
        }

        [HttpGet]
        [Route("GetCompanyByCode")]
        public Dto.Company GetCompanyByCode(string code)
        {
            return Mapper.Map<Dto.Company>(DomainModel.Company.GetByCode(code));
        }


        [HttpGet]
        [Route("GetCompanies")]
        public Dictionary<long, string> GetCompanies(string searchString)
        {
            var companies = DomainModel.User.GetCompanies(searchString);
            return companies;
        }
        [HttpGet]
        [Route("GetPersonnelSubAreaById")]
        public Dto.PersonnelSubArea GetPersonnelSubAreaById(long id)
        {
            return Mapper.Map<Dto.PersonnelSubArea>(DomainModel.User.GetPersonnelSubAreaById(id));
        }
        [HttpGet]
        [Route("GetOrganisationByCode")]
        public Dto.NOrganisation GetOrganisationByCode(string code)
        {
            return Mapper.Map<Dto.NOrganisation>(DomainModel.User.GetOrganisationByCode(code));
        }
        [HttpGet]
        [Route("GetAllOrganisations")]
        public List<Dto.NOrganisation> GetAllOrganisations()
        {
            var orgs = DomainModel.User.GetAllOrganisations();
            if (orgs != null)
                return orgs.Select(p => Mapper.Map<Dto.NOrganisation>(p)).ToList();
            return null;
        }

        [HttpGet]
        [Route("GetGroupsById")]
        public Dictionary<string, string> GetGroupsById(long Id, AdHocActionType actionType, string searchString)
        {
            return DomainModel.User.GetGroupsById(Id, actionType, DomainModel.CheckNull.AsteriskToString(searchString));
        }
        [HttpGet]
        [Route("GetRolesById")]
        public Dictionary<string, string> GetRolesById(long Id, AdHocActionType actionType, string searchString)
        {
            return DomainModel.User.GetRolesById(Id, actionType, DomainModel.CheckNull.AsteriskToString(searchString));
        }

        [HttpGet]
        [Route("IsAdminUser")]
        public bool IsAdminUser(long userId)
        {
            return DomainModel.User.IsAdminUser(userId);
        }

        [HttpGet]
        [Route("GetOrgCompanies")]
        public Dictionary<string, string> GetOrgCompanies(string searchString)
        {
            return DomainModel.User.GetOrgCompanies(searchString);
        }

        [HttpGet]
        [Route("GetOrgDepartments")]
        public Dictionary<string, string> GetOrgDepartments(string searchString, string CompanyNameFilter)
        {
            var companyFilter = CompanyNameFilter?.Split(',').ToList();
            companyFilter = companyFilter ?? new List<string>();
            return DomainModel.User.GetOrgDepartments(searchString, companyFilter);
        }

        [HttpGet]
        [Route("GetOrgDivisions")]
        public Dictionary<string, string> GetOrgDivisions(string searchString, string CompanyNameFilter, string DepartNameFilter)
        {
            var companyFilter = CompanyNameFilter?.Split(',').ToList();
            var departementFilter = DepartNameFilter?.Split(',').ToList();

            if (companyFilter == null) companyFilter = new List<string>();
            if (departementFilter == null) departementFilter = new List<string>();
            return DomainModel.User.GetOrgDivisions(searchString, companyFilter, departementFilter);
        }

        [HttpGet]
        [Route("GetOrgBranches")]
        public Dictionary<string, string> GetOrgBranches(string searchString, string CompanyNameFilter, string DepartNameFilter)
        {
            var companyFilter = CompanyNameFilter?.Split(',').ToList();
            var departementFilter = DepartNameFilter?.Split(',').ToList();


            if (companyFilter == null) companyFilter = new List<string>();
            if (departementFilter == null) departementFilter = new List<string>();

            return DomainModel.User.GetOrgBranches(searchString, companyFilter, departementFilter);
        }

        [HttpGet]
        [Route("GetOrgSections")]
        public Dictionary<string, string> GetOrgSections(string searchString, string CompanyNameFilter, string DepartNameFilter, string BranchNameFilter)
        {
            var companyFilter = CompanyNameFilter?.Split(',').ToList();
            var departementFilter = DepartNameFilter?.Split(',').ToList();

            var branchFilter = BranchNameFilter?.Split(',').ToList();

            if (companyFilter == null) companyFilter = new List<string>();
            if (departementFilter == null) departementFilter = new List<string>();

            if (branchFilter == null) branchFilter = new List<string>();
            return DomainModel.User.GetOrgSections(searchString, companyFilter, departementFilter, branchFilter);
        }

        [HttpGet]
        [Route("GetOrgUnits")]
        public Dictionary<string, string> GetOrgUnits(string searchString, string CompanyNameFilter, string DepartNameFilter, string BranchNameFilter, string SectioNameFilter)
        {
            var companyFilter = CompanyNameFilter?.Split(',').ToList();
            var departementFilter = DepartNameFilter?.Split(',').ToList();
            var branchFilter = BranchNameFilter?.Split(',').ToList();
            var sectionFilter = SectioNameFilter?.Split(',').ToList();

            if (companyFilter == null) companyFilter = new List<string>();
            if (departementFilter == null) departementFilter = new List<string>();
            if (branchFilter == null) branchFilter = new List<string>();
            if (sectionFilter == null) sectionFilter = new List<string>();
            var ret = DomainModel.User.GetOrgUnits(searchString, companyFilter, departementFilter, branchFilter, sectionFilter);
            return ret;
        }

        [HttpGet]
        [Route("GetReportingUsersOfThisManagerIDandAdmin_New")]
        public Dictionary<long, string> GetReportingUsersOfThisManagerIDandAdmin_New(long userId, string searchString, string CompanyNameFilter, string DepartNameFilter, string BranchNameFilter, string SectioNameFilter, string UnitNameFilter)
        {
            var companyFilter = CompanyNameFilter?.Split(',').ToList();
            var departementFilter = DepartNameFilter?.Split(',').ToList();

            var branchFilter = BranchNameFilter?.Split(',').ToList();
            var sectionFilter = SectioNameFilter?.Split(',').ToList();
            var unitNameFilter = UnitNameFilter?.Split(',').ToList();

            if (companyFilter == null) companyFilter = new List<string>();
            if (departementFilter == null) departementFilter = new List<string>();

            if (branchFilter == null) branchFilter = new List<string>();
            if (sectionFilter == null) sectionFilter = new List<string>();
            if (unitNameFilter == null) unitNameFilter = new List<string>();
            return DomainModel.User.GetReportingofSubordinate(userId, searchString, companyFilter, departementFilter, branchFilter, sectionFilter, unitNameFilter);
        }

        [HttpGet]
        [Route("GetAllEmployeesUnderRO")]
        public Dictionary<long, string> GetAllEmployeesUnderRO(string roEmployeeId, string searchString)
        {
            return DomainModel.User.GetAllEmployeesUnderRO(roEmployeeId, searchString);
        }

        [HttpPost]
        [Route("GetUserAccessDetailMatrixGridData")]
        public List<Dto.Presentation.UserAccessDetailMatrixGridData> GetUserAccessDetailMatrixGridData(UserAccessDetailMatrixInput userInput)
        {
            var res = DomainModel.Presentation.UserAccessDetailMatrixGridData.GetUserAccessDetailMatrixGridData(userInput.UserIds);
            if (res != null)
                return res.Select(p => Mapper.Map<Dto.Presentation.UserAccessDetailMatrixGridData>(p)).ToList();
            return null;
        }
        [HttpPost]
        [Route("GetUserAccessDetailMatrixPagedData")]
        public Dto.Core.PresentationGridPagedData<Dto.Presentation.UserAccessDetailMatrixGridData> GetUserAccessDetailMatrixPagedData(PresentationGridPagedDataParameter param)
        {
            var response = new Dto.Core.PresentationGridPagedData<Dto.Presentation.UserAccessDetailMatrixGridData>();
            var dmRsult = DomainModel.Presentation.UserAccessDetailMatrixGridData.GetUserAccessDetailMatrixPagedData(param.Ids, param.StartRowIndex, param.MaximumRows, param.FilterExpression, param.SortExpression);
            response.TotalCount = dmRsult.TotalCount;
            if (dmRsult.Data != null && dmRsult.Data.Count > 0)
                response.Data = ConverToDTo(dmRsult.Data);
            return response;
        }
        [HttpGet]
        [Route("GetGroupMembersbyUserId")]
        public Dictionary<long, string> GetGroupMembersbyUserId(string searchString, long userId)
        {
            return DomainModel.User.GetGroupMembersbyUserId(searchString, userId);
        }

        [HttpGet]
        [Route("GetUsersByGroupId")]
        public Dictionary<long, string> GetUsersByGroupId(string searchString, long groupId)
        {
            return DomainModel.User.GetUsersByGroupId(searchString, groupId);
        }
        public List<Dto.Presentation.UserAccessDetailMatrixGridData> ConverToDTo(List<DomainModel.Presentation.UserAccessDetailMatrixGridData> data)
        {
            List<Dto.Presentation.UserAccessDetailMatrixGridData> lst = new List<UserAccessDetailMatrixGridData>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    lst.Add(new UserAccessDetailMatrixGridData()
                    {
                        AccountReconcilationId = item.AccountReconcilationId,
                        AccountType = item.AccountType,
                        ApplicationType = item.ApplicationType,
                        Branch = item.Branch,
                        Company = item.Company,
                        Department = item.Department,
                        Description = item.Description,
                        Division = item.Division,
                        EndpointHostName = item.EndpointHostName,
                        EndpointSystemID = item.EndpointSystemID,
                        EndpointSystemName = item.EndpointSystemName,
                        EndpointSystemTypeId = item.EndpointSystemTypeId,
                        Entitlement = item.Entitlement,
                        EntitlementType = item.EntitlementType,
                        FullName = item.FullName,
                        Institution = item.Institution,
                        IsActive = item.IsActive,
                        LastLogonDate = item.LastLogonDate,
                        Name = item.Name,
                        ParentId = item.ParentId,
                        ReportingOfficer = item.ReportingOfficer,
                        Section = item.Section,
                        Type = item.Type,
                        Unit = item.Unit,
                        UserName = item.UserName
                    });
                }
            }
            return lst;
        }
        [HttpPost]
        [Route("DownloadUserAccessDetail")]
        public byte[] DownloadUserAccessDetail(PresentationGridPagedDataParameter param, string exportFileType)
        {
            return DomainModel.Presentation.UserAccessDetailMatrixGridData.DownloadUserAccessDetail(param.Ids, param.StartRowIndex, param.MaximumRows, param.FilterExpression, param.SortExpression, exportFileType);
        }
        [HttpGet]
        [Route("CheckDisplayNameDuplicate")]
        public User CheckDisplayNameDuplicate(long userId, string displayName)
        {
            return DtoMap(DomainModel.User.CheckDisplayNameDuplicate(userId, displayName));
        }

        [HttpGet]
        [Route("PerformLogoutCleanup")]
        public bool PerformLogoutCleanup(long userId)
        {
            DomainModel.User.PerformLogoutCleanup(userId);
            return true;
        }

        [HttpGet]
        [Route("SendSMS")]
        public string SendSMSNotification(long userId)
        {
            return DomainModel.User.SendOTP(userId);
        }

        [HttpGet]
        [Route("GetCustomReportUserBySearchString")]
        public Dictionary<long, string> GetCustomReportUserBySearchString(string searchString)
        {
            return DomainModel.User.GetCustomReportUserBySearchString(searchString);
        }

        [HttpGet]
        [Route("GetUserIsActive")]
        [AllowAnonymous]

        public string GetUserIsActive(string username)
        {
            return DomainModel.User.GetByUserActive(username);
        }
        [HttpGet]
        [Route("GetUserVPNToken")]
        public string CheckUserVPNToken(long userId)
        {
            return DomainModel.User.CheckUserVPNToken(userId);
        }
        [HttpGet]
        [Route("IsSameTokenExists")]
        public string IsSameTokenExists(string tokenId)
        {
            return DomainModel.User.IsSameTokenExists(tokenId);
        }
        [HttpGet]
        [Route("IsEmailIdAlreadyExist")]
        public User IsEmailIdAlreadyExist(long userId, string emailId)
        {
            return DtoMap(DomainModel.User.IsEmailIdAlreadyExist(userId, emailId));
        }
    }
}