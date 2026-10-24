using DI.Chakra.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DBConfigHelper;

namespace DI.Chakra.Connector
{
    public class COPHRMSConnector : ConnectorBase
    {
        #region Members

        string serviceEndpoint = "";
        string WebApiURL = "api/services/app/";
        string ApiName = string.Empty;

        #endregion

        #region Core

        public COPHRMSConnector(EndPoint endpoint, AccessRequest currentRequest)
        {
            this.EndPoint = endpoint;
            this.CurrentRequest = currentRequest;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
        }

        #endregion

        #region Discovery

        public override bool Connect()
        {
            this.serviceEndpoint = this.Configuration(EndpointKey.WebAPI);

            ApiName = string.Format("{0}{1}", WebApiURL, "employee/Connect");


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(serviceEndpoint);

            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.PostAsync(ApiName,
                        new StringContent(JsonConvert.SerializeObject("").ToString(),
                        Encoding.UTF8, "application/json")).Result;

            if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, "Connection Failed");
                return false;
            }


            return true;
        }
        public override void Disconnect()
        {
            throw new NotImplementedException();
        }
        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            throw new NotImplementedException();
        }
        public override void GetData()
        {
            this.serviceEndpoint = this.Configuration(EndpointKey.WebAPI);

            getOrganisation(false);

            getDesignation();

            getUser();

            getOrganisation(true);

            getTransaction();

        }
        private void getUser()
        {
            ApiName = string.Format("{0}{1}", WebApiURL, "employee/GetAllEmployee");


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(serviceEndpoint);

            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.PostAsync(ApiName,
                        new StringContent(JsonConvert.SerializeObject("").ToString(),
                        Encoding.UTF8, "application/json")).Result;

            if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, "Get User Failed");
            }
            var content = response.Content.ReadAsStringAsync();
            var output = content.Result;

            dynamic jsonParserOutput = JObject.Parse(output.ToString());

            if (jsonParserOutput.result != null)
            {
                foreach (var resultItem in jsonParserOutput.result)
                {
                    try
                    {
                        var user = User.GetByEmployeeId((string)resultItem.SelectToken("employeeId"));
                        var isNewUser = false;

                        if (user == null)
                        {
                            user = new User();
                            isNewUser = true;
                            user.UserName = (string)resultItem.SelectToken("userName");
                        }
                        else
                        {
                            user = User.GetById(user.Id);
                        }

                        user.LastModifiedBy = "system connector";
                        user.LastModifiedDate = DateTime.Now;

                        //(string)resultItem.SelectToken("idType");                
                        user.Password = isNewUser ? RSAHelper.EncryptData(MasterConfigHelper.DefaultPassword) : user.Password;
                        user.IdentificationNo = (string)resultItem.SelectToken("identificationNo");
                        user.FirstName = (string)resultItem.SelectToken("firstName");
                        user.MiddleName = (string)resultItem.SelectToken("middleName");
                        user.LastName = (string)resultItem.SelectToken("lastName");
                        user.DisplayName = (string)resultItem.SelectToken("displayName");
                        user.KnownAs = (string)resultItem.SelectToken("knownAs");
                        user.EmployeeId = (string)resultItem.SelectToken("employeeId");

                        var orgCode = (string)resultItem.SelectToken("organisation.orgUnitPrefix");
                        var organisation = NOrganisation.GetByOrgCode(orgCode);
                        if (organisation != null)
                            user.NOrganisationId = (int)organisation.Id;

                        var businessCode = (string)resultItem.SelectToken("businessRole.designationCode");
                        var business = NDesignation.GetByCode(businessCode);
                        if (business != null)
                            user.NBusinessRoleId = (int)business.Id;

                        var managerId = resultItem.SelectToken("managerId");
                        if (managerId != null)
                            user.ManagerId = (int)managerId;
                        user.EmailId = (string)resultItem.SelectToken("emailId");
                        user.TelephoneNumber = (string)resultItem.SelectToken("telephoneNumber");
                        user.HomephoneNumber = (string)resultItem.SelectToken("homephoneNumber");
                        user.MobileNumber = (string)resultItem.SelectToken("mobileNumber");
                        user.Fax = (string)resultItem.SelectToken("fax");
                        user.PostalCode = (string)resultItem.SelectToken("postalCode");
                        user.PostalAddress = (string)resultItem.SelectToken("postalAddress");
                        user.Ethinicity = (string)resultItem.SelectToken("ethinicity");
                        user.Country = (string)resultItem.SelectToken("country");
                        user.EmployeeType = (EmployeeType)Enum.Parse(typeof(EmployeeType), (string)resultItem.SelectToken("employeeType"));
                        user.FullTimeEmployee = true;
                        user.JoinDate = (DateTime)resultItem.SelectToken("joinDate");
                        var terminationDate = resultItem.SelectToken("terminationDate");
                        if (terminationDate != null)
                            user.TerminationDate = (DateTime)terminationDate;
                        user.IsActive = (bool)resultItem.SelectToken("isActive");

                        user.Save(MasterConfigHelper.ServiceUser);

                        if (isNewUser)
                        {
                            long userId = user.Id;
                            int RoleId = MasterConfigHelper.RoleUser;
                            User.InsertUserRole(userId, RoleId);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, "Get User Failed : " + ex.Message);
                    }
                }
            }
        }
        private void getOrganisation(bool getHeadDetailFromUserTable)
        {
            ApiName = string.Format("{0}{1}", WebApiURL, "employee/GetOrganisationDetails");

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(serviceEndpoint);

            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.PostAsync(ApiName,
                        new StringContent(JsonConvert.SerializeObject("").ToString(),
                        Encoding.UTF8, "application/json")).Result;

            if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, "GetOrganisationDetails Failed");
            }
            var content = response.Content.ReadAsStringAsync();
            var output = content.Result;

            dynamic jsonParserOutput = JObject.Parse(output.ToString());

            if (jsonParserOutput.result != null)
            {
                foreach (var resultItem in jsonParserOutput.result)
                {
                    try
                    {
                        var organisation = NOrganisation.GetByOrgCode((string)resultItem.SelectToken("orgUnitPrefix"));

                        if (organisation == null)
                            organisation = new NOrganisation();

                        var code = (string)resultItem.SelectToken("orgUnitPrefix");
                        organisation.Code = code;
                        var name = (string)resultItem.SelectToken("organisationName");
                        organisation.Name = name;
                        var parentOrg = (string)resultItem.SelectToken("parentOrganisation");
                        organisation.ParentOrgCode = parentOrg;
                        var scope = (string)resultItem.SelectToken("scope");
                        if (!string.IsNullOrEmpty(scope))
                        {
                            var orgScope = NOrganisationScope.GetAll(true).Where(a => a.Scope == scope).FirstOrDefault();
                            if (orgScope != null)
                                organisation.ScopeId = orgScope.Id;
                        }

                        var isAcitve = (bool)resultItem.SelectToken("isActive");
                        organisation.IsActive = isAcitve;

                        organisation.Save(MasterConfigHelper.ServiceUser);

                        var head = resultItem.SelectToken("head");
                        if (head != null && organisation != null)
                        {
                            var org = NOrganisation.GetByOrgCode(organisation.Code);
                            var orgHead = NOrganisationHead.GetOrganisationHead(org.Id);
                            if (orgHead == 0)
                            {
                                NOrganisationHead orgHeadAdd = new NOrganisationHead();

                                orgHeadAdd.OrganisationId = org.Id;

                                if (getHeadDetailFromUserTable)
                                {
                                    if (head != null)
                                    {
                                        User headUser = User.GetByEmployeeId((string)head);
                                        if (headUser != null)
                                            orgHeadAdd.HeadId = headUser.Id;
                                    }

                                    orgHeadAdd.Save(MasterConfigHelper.ServiceUser);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, "Get Organisation Failed : " + ex.Message);
                    }
                }
            }
        }
        private void getDesignation()
        {
            ApiName = string.Format("{0}{1}", WebApiURL, "employee/GetDesignationDetails");

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(serviceEndpoint);

            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.PostAsync(ApiName,
                        new StringContent(JsonConvert.SerializeObject("").ToString(),
                        Encoding.UTF8, "application/json")).Result;

            if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, "GetDesignationDetails Failed");
            }
            var content = response.Content.ReadAsStringAsync();
            var output = content.Result;

            dynamic jsonParserOutput = JObject.Parse(output.ToString());

            if (jsonParserOutput.result != null)
            {
                try
                {
                    foreach (var resultItem in jsonParserOutput.result)
                    {
                        var designation = NDesignation.GetByCode((string)resultItem.SelectToken("designationCode"));

                        if (designation == null)
                            designation = new NDesignation();

                        designation.Code = (string)resultItem.SelectToken("designationCode");
                        designation.Description = (string)resultItem.SelectToken("designationDesc");
                        designation.IsActive = (bool)resultItem.SelectToken("isActive");

                        designation.Save(MasterConfigHelper.ServiceUser);
                    }
                }
                catch (Exception ex)
                {
                    WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, "Get Designation Failed : " + ex.Message);
                }
            }
        }
        private void getTransaction()
        {
            if (this.IsFirstTimeDiscovery)
            {
                ApiName = string.Format("{0}{1}", WebApiURL, "employee/GetAllTransactions");
            }
            else
            {
                var deltaDate = this.Process.LastRunOn.Value.ToString("yyyy-MM-ddTHH:MM:00");
                ApiName = string.Format("{0}{1}{2}", WebApiURL, "employee/GetAllTransactionsFromCreatedDate?createdDate=", deltaDate);
            }

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(serviceEndpoint);

            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.PostAsync(ApiName,
                        new StringContent(JsonConvert.SerializeObject("").ToString(),
                        Encoding.UTF8, "application/json")).Result;

            if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, "GetAllTransactions Failed");
            }
            var content = response.Content.ReadAsStringAsync();
            var output = content.Result;

            dynamic jsonParserOutput = JObject.Parse(output.ToString());

            if (jsonParserOutput.result != null)
            {
                foreach (var resultItem in jsonParserOutput.result)
                {
                    try
                    {
                        var requestType = (string)resultItem.SelectToken("requestType");
                        var userId = (string)resultItem.SelectToken("employeeId");
                        var effectiveDate = (DateTime)resultItem.SelectToken("effectiveDate");
                        var currentOrgPrefix = (string)resultItem.SelectToken("currentOrgPrefix");
                        var newOrgPrefix = (string)resultItem.SelectToken("newOrgPrefix");

                        if (requestType.ToUpper() == "HIRE")
                        {
                            CreateOnboardTriggers(userId, effectiveDate, false);
                        }
                        else if (requestType.ToUpper() == "RESIG")
                        {
                            CreateOffboardTriggers(userId, effectiveDate);
                        }
                        else if (requestType.ToUpper() == "TFR" || requestType.ToUpper() == "REORG")
                        {
                            CreateTransferTriggers(userId, newOrgPrefix, effectiveDate, currentOrgPrefix, requestType);
                        }
                        else if (requestType.ToUpper() == "REHIRE")
                        {
                            CreateOnboardTriggers(userId, effectiveDate, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, "Get Transaction Failed : " + ex.Message);
                    }
                }
            }
        }
        private void CreateOnboardTriggers(string userId, DateTime effectiveDate, bool isReJoin)
        {
            EmployeeEvents evnt = EmployeeEvents.None;
            evnt = EmployeeEvents.NewJoinee;

            if (isReJoin)
                evnt = EmployeeEvents.Rehire;

            var user = User.GetByEmployeeId(userId);

            if (user == null) return;

            var triggerExcuted = EmployeeEventTrigger.GetAll().Where(triggerExists => triggerExists.UserId == user.Id && triggerExists.EventType == evnt).FirstOrDefault();

            if (!isReJoin && triggerExcuted != null) return;

            triggerExcuted = EmployeeEventTrigger.GetByUserAndEvent(user.Id, evnt, this.Process.LastRunOn.Value);

            if (triggerExcuted != null && isReJoin)
            {
                if (triggerExcuted.IsRequestCreated)
                {
                    return;
                }

                triggerExcuted.EffectiveDate = effectiveDate;
                triggerExcuted.IsActive = true;
                triggerExcuted.UserDetails = isReJoin ? UtilityMethods.Serialize<User>(user) : string.Empty;

                triggerExcuted.Save(MasterConfigHelper.ServiceUser);
            }
            else
            {
                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    EventType = evnt,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true,
                    UserDetails = isReJoin ? UtilityMethods.Serialize<User>(user) : string.Empty,
                    EffectiveDate = effectiveDate,

                };

                newTrigger.Save(MasterConfigHelper.ServiceUser);
            }
        }
        private void CreateOffboardTriggers(string userId, DateTime effectiveDate)
        {
            var evnt = EmployeeEvents.Resignation;

            var user = User.GetByEmployeeId(userId);

            if (user == null) return;

            var triggerExcuted = EmployeeEventTrigger.GetAll().Where(triggerExists => triggerExists.UserId == user.Id && triggerExists.EventType == evnt).FirstOrDefault();

            if (triggerExcuted != null)
            {
                triggerExcuted.EffectiveDate = user.TerminationDate.Value;
                triggerExcuted.Save(MasterConfigHelper.ServiceUser);
            }
            else
            {
                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    EventType = evnt,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true,
                    UserDetails = string.Empty,
                    EffectiveDate = effectiveDate,

                };

                newTrigger.Save(MasterConfigHelper.ServiceUser);
            }
        }
        private void CreateTransferTriggers(string userId, string newOrgPrefix, DateTime effectiveDate, string oldOrgPrefix, string requestType)
        {
            EmployeeEvents evnt = EmployeeEvents.None;

            if (requestType.ToUpper() == "TFR")
                evnt = EmployeeEvents.OUChange;
            else if (requestType.ToUpper() == "REORG")
                evnt = EmployeeEvents.ReOrganisation;

            if (evnt == EmployeeEvents.None) return;

            var oldUser = User.GetByEmployeeId(userId);
            if (oldUser == null) return;
            var oldOrg = NOrganisation.GetByOrgCode(oldOrgPrefix);
            oldUser.NOrganisation = oldOrg;
            var oldUserDetail = UtilityMethods.Serialize<User>(oldUser);

            var user = User.GetByEmployeeId(userId);

            if (user == null) return;

            var trigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, evnt, this.Process.LastRunOn.Value.Date);

            if (trigger != null)
            {
                if (trigger.IsRequestCreated)
                {
                    return;
                }

                trigger.EffectiveDate = effectiveDate;
                trigger.LastModifiedBy = trigger.CreatedBy;
                trigger.LastModifiedDate = DateTime.Now;
                trigger.OldUserDetails = oldUserDetail;
                trigger.UserDetails = UtilityMethods.Serialize<User>(user);
                trigger.Save(MasterConfigHelper.ServiceUser);
            }
            else
            {

                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    EventType = evnt,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true,
                    EffectiveDate = effectiveDate,
                    UserDetails = UtilityMethods.Serialize<User>(user),
                    OldUserDetails = oldUserDetail

                };

                newTrigger.Save(MasterConfigHelper.ServiceUser);
            }
        }

        #endregion

    }
}
