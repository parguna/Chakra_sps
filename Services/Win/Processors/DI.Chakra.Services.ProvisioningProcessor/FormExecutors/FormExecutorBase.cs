using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DI.Chakra.Services.ProvisioningProcessor
{
    public abstract class FormExecutorBase
    {
        protected readonly IdmsLogger _idmsLogger;
        readonly string MasterConfigHelperServiceUser;
        public FormExecutorBase(IdmsLogger idmsLogger)
        {
            _idmsLogger = idmsLogger;
            if (_idmsLogger == null && CurrentRequest!=null)
            {
                var ep = EndPoint.GetById(CurrentRequest.EndpointSystemId.Value);
                _idmsLogger = new ProvisioningLogger(CurrentRequest?.Id, CurrentRequest.EndpointSystemId, ep!=null ? ep.EndpointSystemTypeId:0, ep.DisplayName);

            }
            MasterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
        }

        protected string sapUserDefaultValidTo = DateTime.MaxValue.ToString("yyyy.MM.dd"); //"9999.12.31";

        ConnectorBase _connector = null;

        public ConnectorBase EndpointConnector
        {
            get
            {
                if (_connector == null)
                {
                    _connector = ConnectorFactory.GetConnector(Endpoint, CurrentRequest);
                }
                return _connector;
            }

        }

        protected virtual string GetSAPUserType(SAPUserType sAPUserType)
        {

            if (sAPUserType == SAPUserType.Communication)
            {
                return "C";
            }
            else if (sAPUserType == SAPUserType.Service)
            {
                return "S";
            }
            else if (sAPUserType == SAPUserType.System)
            {
                return "B";
            }
            else if (sAPUserType == SAPUserType.Reference)
            {
                return "L";
            }
            else if (sAPUserType == SAPUserType.Dialog)
            {
                return "A";
            }
            else
                return "";
        }

        protected virtual SAPUserType GetUserType(string sAPUserType)
        {

            if (sAPUserType.Equals("C", StringComparison.InvariantCultureIgnoreCase))
            {
                return SAPUserType.Communication;
            }
            else if (sAPUserType.Equals("S", StringComparison.InvariantCultureIgnoreCase))
            {
                return SAPUserType.Service;
            }
            else if (sAPUserType.Equals("B", StringComparison.InvariantCultureIgnoreCase))
            {
                return SAPUserType.Service;// "B";
            }
            else if (sAPUserType.Equals("L", StringComparison.InvariantCultureIgnoreCase))
            {
                return SAPUserType.Reference;//   "L";
            }
            else if (sAPUserType.Equals("A", StringComparison.InvariantCultureIgnoreCase))
            {
                return SAPUserType.Dialog;// "A";
            }
            else
                return SAPUserType.Dialog;
        }
        protected DateTime? GetValidTo(Nullable<long> userId)
        {
            var validTo = DateTime.MaxValue;
            if ((CurrentRequest.AccountType == AccountType.Service || CurrentRequest.AccountType == AccountType.Shared) && !CurrentRequest.DeactivationDate.HasValue) return validTo;

            if (CurrentRequest != null && CurrentRequest.DeactivationDate.HasValue)
            {
                if (userId.HasValue && userId.Value > 0)
                {
                    var user = DomainModel.User.GetById(userId.Value);
                    if (user.FullTimeEmployee == false)
                    {
                        return CurrentRequest.DeactivationDate.Value.Date.AddHours(Constants.MaxHour);
                    }
                    else
                    {
                        return CurrentRequest.DeactivationDate.Value.Date.AddHours(Constants.MaxHour).AddMinutes(Constants.MaxMinutes).AddSeconds(Constants.MaxSeconds);
                    }
                }
                else
                {
                    return CurrentRequest.DeactivationDate.Value.Date.AddHours(Constants.MaxHour).AddMinutes(Constants.MaxMinutes).AddSeconds(Constants.MaxSeconds);
                }
            }
            else
            {
                if (userId.HasValue && userId.Value > 0)
                {
                    if (userId.HasValue && userId.Value > 0)
                    {
                        var user = DomainModel.User.GetById(userId.Value);
                        if (user != null && user.FullTimeEmployee == false && user.TerminationDate.HasValue)
                        {
                            validTo = user.TerminationDate.Value.Date.AddHours(Constants.MaxHour);
                        }
                    }
                }
            }
            return validTo;
        }

        protected bool DeleteFLINSDependentAccount(DomainModel.User user, string accountName)
        {
            bool success = true;
            try
            {
                EndPoint ep = EndPoint.GetByApplicationType(ApplicationType.SAP_EP);
                EndPoint ce = EndPoint.GetByApplicationType(ApplicationType.CE);

                List<SAPEPUserInfo> accounts = SAPEPUserInfo.GetActiveByUserName(accountName, new List<long> { ep.Id, ce.Id });

                DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common.SAPCEBase sAPCEBase = null;
                foreach (var account in accounts)
                {
                    if (account.EndPointSystemId == ep.Id && ep.IsActive)
                    {
                        sAPCEBase = new DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common.SAPCEBase(ep, this.CurrentRequest, _idmsLogger);
                        sAPCEBase.DeleteAccount(user, account.Id);
                    }
                    if (account.EndPointSystemId == ce.Id && ce.IsActive)
                    {
                        sAPCEBase = new DI.Chakra.Services.ProvisioningProcessor.FormExecutors.Common.SAPCEBase(ce, this.CurrentRequest, _idmsLogger);
                        sAPCEBase.DeleteAccount(user, account.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error while deleting FLINS Dependent account for the account name {0}. {0}", accountName, ex.Message), ex);

            }

            return success;
        }

        protected virtual bool AreEntitlementsValid(string[] entitlements)
        {
            return true;
        }
        protected EndPoint Endpoint { get; set; }

        public AccessRequest CurrentRequest { get; set; }

        public abstract bool Execute(ProvisioningParamCollection fields, User user);

        public abstract bool OffBoard(User user, ProvisioningParamCollection fields);

        public abstract bool Transfer(User user, ProvisioningParamCollection fields);

        public abstract bool ReOrg(User user, ProvisioningParamCollection fields);

        public abstract bool Retain(User user, ProvisioningParamCollection fields);

        public abstract bool Propagate(User user, ProvisioningParamCollection fields);

        public abstract bool RemediateAttestation(User user, ProvisioningParamCollection fields);

        public virtual bool Extension(User user, ProvisioningParamCollection fields, AccountType accountType, long accountReconId = 0)
        {
            var executed = false;

            try
            {
                AccountReconcilation accountRecon;

                if (accountReconId == 0)
                    accountRecon = AccountReconcilation.GetByUserId(user.Id, accountType, Endpoint.Id);
                else
                    accountRecon = AccountReconcilation.GetById(accountReconId);

                if (accountRecon != null)
                {
                    var currentUser = MasterConfigHelperServiceUser;
                    var sapUserDefaultValidTo = user.TerminationDate.HasValue && accountType != AccountType.Service ? user.TerminationDate.Value : DateTime.MaxValue;
                    accountRecon.ValidToDate = sapUserDefaultValidTo;
                    accountRecon.Save(currentUser);

                    // IDMS0448
                    //var entitlementRecons = EntitlementReconcilation.GetAllActiveByAccountReconcilationId(accountRecon.Id);
                    //if (entitlementRecons != null && entitlementRecons.Count > 0)
                    //{
                    //    entitlementRecons.ForEach(i =>
                    //    {
                    //        i.ValidToDate = sapUserDefaultValidTo;
                    //        i.Save(currentUser);
                    //    });
                    //}
                    executed = true;
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
            }
            return executed;
        }

        public virtual bool UndoEvent(User user, ProvisioningParamCollection fields) { return true; }

        public virtual bool AssignO365License(User user, ProvisioningParamCollection fields) { return true; }

        public virtual bool UpdateO365MailBoxSettings(DomainModel.User user, ProvisioningParamCollection fields) { return true; }
        public string ConstructUserNameForMSSLAndEBS(User user)
        {
            string userName;

            // Assign the same username as in the parent request
            if (this.CurrentRequest.AccountType == AccountType.User
                && this.CurrentRequest.AdHocAccessRequest != null
                && this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount
                && this.CurrentRequest.ParentRequestId != null)
            {
                var dependentApplicationType = getDependentApplicationType(this.CurrentRequest.AdHocAccessRequest.ApplicationType);

                var dependentApplicationAccounts = AccountReconcilation.GetByAccountTypeAndApplicationType(user.Id, AccountType.User, dependentApplicationType);

                if (dependentApplicationAccounts != null && dependentApplicationAccounts.Count > 0)
                {
                    userName = dependentApplicationAccounts.Where(a => a.IsDeleted == false && a.IsLocked == false && a.AccountType == AccountType.User).Select(a => a.AccountName.ToUpper()).FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(userName))
                    {
                        var available_userName = this.CheckUserNameExistsForMSSLAndEBS(userName, user.Id);

                        if (userName.Equals(available_userName, StringComparison.OrdinalIgnoreCase))
                        {
                            return userName;
                        }
                        else
                            throw new Exception($"Acount Name :{userName} is not available in the dependent System : {UtilityMethods.GetEnumAttribute<Description>(dependentApplicationType)}");
                    }
                }
            }


            StringBuilder userNameSB = new StringBuilder();
            userName = removeSpecialCharacters(user.DisplayName, false, false, user.Ethinicity);

            //append position prefix from reserved10 field
            userNameSB.Append(user.Reserved10);
            user.Reserved10 = string.Empty;

            if (!string.IsNullOrEmpty(userName)) // && userName.Length >= 3)
            {
                string[] nameAry = userName.Split(' ');
                if (nameAry.Length == 1)
                {
                    if (nameAry[0].Length >= 3)
                        userNameSB.Append(nameAry[0].Substring(0, 3));
                    else
                        userNameSB.Append(nameAry[0]);
                }
                else if (nameAry.Length == 2)
                {
                    if (nameAry[0].Length >= 2)
                    {
                        userNameSB.Append(nameAry[0].Substring(0, 2));
                        userNameSB.Append(nameAry[1].Substring(0, 1));
                    }
                    else if (nameAry[1].Length >= 2)
                    {
                        userNameSB.Append(nameAry[0].Substring(0, 1));
                        userNameSB.Append(nameAry[1].Substring(0, 2));
                    }
                    else
                    {
                        userNameSB.Append(nameAry[0]);
                        userNameSB.Append(nameAry[1]);
                    }
                }
                else if (nameAry.Length >= 3)
                {
                    userNameSB.Append(nameAry[0].Substring(0, 1));
                    userNameSB.Append(nameAry[1].Substring(0, 1));
                    userNameSB.Append(nameAry[2].Substring(0, 1));
                }
            }

            userName = userNameSB.ToString().ToUpper();
            userName = this.CheckUserNameExistsForMSSLAndEBS(userName, user.Id);
            return userName;
        }




        ApplicationType getDependentApplicationType(ApplicationType applicationType)
        {
            switch (applicationType)
            {
                case ApplicationType.EBS_CRM: return ApplicationType.EBS_ISU;
                case ApplicationType.EBS_ISU: return ApplicationType.EBS_CRM;
                case ApplicationType.MSSL_CRM: return ApplicationType.MSSL_ISU;
                case ApplicationType.MSSL_ISU: return ApplicationType.MSSL_CRM;
                default: return applicationType;
            }
        }

        protected string CheckUserNameExistsForMSSLAndEBS(string userName, long DiUserID)
        {
            LogDebugStep("CheckUserNameExistsForMSSLAndEBS:" + userName);
            int no = 1;
            if (userName.Length > 8)
                userName = userName.Substring(0, 8);

            var dependentEndpoint = EndPoint.GetByApplicationType(getDependentApplicationType(this.Endpoint.ApplicationType));

            var endpoints = new List<long>() { this.Endpoint.Id };

            if (dependentEndpoint != null)
            {
                endpoints.Add(dependentEndpoint.Id);
            }

            var acctRecons = AccountReconcilation.GetByAccountNames(userName, endpoints);
            int increment = 1;
            string initialUsername = userName;
            while (acctRecons != null && acctRecons.Count>0)
            {
                if (acctRecons.All(a=> a.UserId == DiUserID))
                    break;
                var inc = System.Text.RegularExpressions.Regex.Match(userName, @"\d+$").Value;
                if (!string.IsNullOrWhiteSpace(inc) && int.TryParse(inc,out increment))
                {
                    increment = increment + 1;
                    userName = $"{System.Text.RegularExpressions.Regex.Replace(userName, @"\d+$", "")}{increment}";
                    //userName = $"{initialUsername}{(increment)}";
                }
                else
                {
                    userName = userName.Substring(0, userName.Length - 1) + no;
                }
                

                acctRecons = AccountReconcilation.GetByAccountNames(userName, endpoints);
                no++;
            }

            return userName;
        }
        protected virtual string ConstructUserName(User user)
        {
            LogDebugStep("ConstructUserName for the user Id:" + user.Id);
            string userName = string.Empty;
            StringBuilder userNameSB = new StringBuilder();

            try
            {
                userName = removeSpecialCharacters(user.DisplayName, false, false, user.Ethinicity)?.Trim();
                
                var firstLetter = Enum.GetName(typeof(EmployeeType), user.EmployeeType)[0];

                userNameSB.Append(firstLetter);

                if (!string.IsNullOrEmpty(userName)) // && userName.Length >= 3)
                {
                    string[] nameAry = userName.Split(' ');
                    if (nameAry.Length == 1)
                    {
                        if (nameAry[0].Length >= 3)
                            userNameSB.Append(nameAry[0].Substring(0, 3));
                        else
                            userNameSB.Append(nameAry[0]);
                    }
                    else if (nameAry.Length == 2)
                    {
                        if (nameAry[0].Length >= 2)
                        {
                            userNameSB.Append(nameAry[0].Substring(0, 2));
                            userNameSB.Append(nameAry[1].Substring(0, 1));
                        }
                        else if (nameAry[1].Length >= 2)
                        {
                            userNameSB.Append(nameAry[0].Substring(0, 1));
                            userNameSB.Append(nameAry[1].Substring(0, 2));
                        }
                        else
                        {
                            userNameSB.Append(nameAry[0]);
                            userNameSB.Append(nameAry[1]);
                        }
                    }
                    else if (nameAry.Length >= 3)
                    {
                        userNameSB.Append(nameAry[0].Substring(0, 1));
                        userNameSB.Append(nameAry[1].Substring(0, 1));
                        userNameSB.Append(nameAry[2].Substring(0, 1));
                    }
                }

                if (!string.IsNullOrEmpty(user.EmployeeId) && user.EmployeeId.Length >= 5)
                {
                    userNameSB.Append(user.EmployeeId.Remove(0, user.EmployeeId.Length - 4));
                }
                userName = userNameSB.ToString().ToUpper();
                userName = CheckUserNameExists(userName, 8);
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Error while Constructing User Name", ex);
                throw ex;
            }

            LogDebugStep($"Constructed UserName for the user Id:{user.Id} is {userName}");

            return userName;
        }

        string CheckUserNameExists(string userName, int maxLength)
        {
            if (userName.Length > maxLength)
                userName = userName.Substring(0, maxLength);
            return CheckUserNameExists(userName);
        }

        protected virtual string CheckUserNameExists(string userName)
        {
            int no = 1;
            bool checkForLocked = false;

            var ep = DomainModel.EndPoint.GetById(this.Endpoint.Id, true);
            var endpointType = ep.EndPointSystemType;

            if (endpointType != null && endpointType.Id != (long)EndpointType.SAP)
                checkForLocked = true;

            var acctRecon = AccountReconcilation.GetByAccountName(userName, Endpoint.Id);
            var canloop = acctRecon != null && !acctRecon.IsDeleted;

            if (checkForLocked)
                canloop = canloop && acctRecon.IsLocked;

            while (canloop)
            {
                userName = userName.Substring(0, userName.Length - 1) + no;
                acctRecon = AccountReconcilation.GetByAccountName(userName, Endpoint.Id);

                canloop = acctRecon != null && !acctRecon.IsDeleted;
                if (checkForLocked)
                    canloop = canloop && !acctRecon.IsLocked;

                no++;
            }
            return userName;
        }

        protected virtual void LogMessage(string message)
        {
            _idmsLogger.WriteLog(LogLevelType.Info, message, null);
        }

        protected virtual void LogDebugStep(string message)
        {
            _idmsLogger.WriteLog(LogLevelType.DebugStep, message, null);
        }

        protected virtual void LogError(string message, Exception ex)
        {
            _idmsLogger.WriteLog(LogLevelType.Error, message, ex);
        }

        protected virtual Object setUserInfoByAtrbValue(Object endPointUsr, User usr, ProvisioningParam field)
        {
            if (field == null || usr == null)
                return endPointUsr;

            object atrbValue = null;
            Type atrbType = null;

            List<PropertyInfo> pInfoList = (from pInfo in usr.GetType().GetProperties()
                                            where pInfo.Name == field.Value
                                            select pInfo).ToList<PropertyInfo>();

            if (pInfoList != null && pInfoList.Count == 1)
            {
                atrbValue = pInfoList[0].GetValue(usr, null);
                atrbType = pInfoList[0].PropertyType;

            }

            List<PropertyInfo> endPtpInfoList = (from pInfo in endPointUsr.GetType().GetProperties()
                                                 where pInfo.Name == field.Name && pInfo.PropertyType == atrbType
                                                 select pInfo).ToList<PropertyInfo>();

            if (endPtpInfoList != null && endPtpInfoList.Count == 1 && atrbValue != null)
            {
                endPtpInfoList[0].SetValue(endPointUsr, atrbValue, null);
            }

            return endPointUsr;
        }

        protected string removeSpecialCharacters(string inPutStr, bool isRemoveSpace, bool isFirstName, string ethinicity)
        {
            StringBuilder outPutSB = new StringBuilder();
            char[] spChars = new char[] { '\'', '~', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '{', '}', '[', ']', '|', '\\', '<', '>', '?', ',', '.', '/', '\'' };

            inPutStr = inPutStr != null ? inPutStr.Trim() : string.Empty;
            if (inPutStr.Length > 4 && inPutStr.StartsWith("Bin ", StringComparison.CurrentCultureIgnoreCase))
            {
                inPutStr = inPutStr.Substring(4, inPutStr.Length - 4);
            }
            if (inPutStr.Length > 4 && inPutStr.StartsWith("bte ", StringComparison.CurrentCultureIgnoreCase))
            {
                inPutStr = inPutStr.Substring(4, inPutStr.Length - 4);
            }
            if (inPutStr.Length > 6 && inPutStr.StartsWith("binte ", StringComparison.CurrentCultureIgnoreCase))
            {
                inPutStr = inPutStr.Substring(6, inPutStr.Length - 6);
            }

            outPutSB.Append(inPutStr);

            outPutSB.Replace("S/O", string.Empty);
            outPutSB.Replace("S/o", string.Empty);
            outPutSB.Replace("s/O", string.Empty);
            outPutSB.Replace("s/o", string.Empty);
            outPutSB.Replace("s o", string.Empty);
            outPutSB.Replace("S O", string.Empty);

            outPutSB.Replace("D/O", string.Empty);
            outPutSB.Replace("D/o", string.Empty);
            outPutSB.Replace("d/O", string.Empty);
            outPutSB.Replace("d/o", string.Empty);
            outPutSB.Replace("d o", string.Empty);
            outPutSB.Replace("D O", string.Empty);

            foreach (char spChar in spChars)
            {
                outPutSB.Replace(spChar, ' ');
            }

            if (isRemoveSpace)
            {
                outPutSB.Replace(" ", string.Empty);
            }
            else
            {
                outPutSB.Replace("   ", string.Empty);
                outPutSB.Replace("  ", string.Empty);
            }

            return outPutSB.ToString();
        }

        protected List<string> ConstructSAPUserGroup(User user)
        {
            List<string> groups = new List<string>();

            string groupCode = string.Format("{0}{1}{2}", user.DepartmentCode, user.DivisionCode, user.CostCenter);

            bool isGroupExists = IsSAPUserGroupExists(groupCode);

            if (isGroupExists)
            {
                groups.Add(groupCode);
            }

            return groups;
        }

        protected bool IsSAPUserGroupExists(string groupCode)
        {

            var group = SAPGroupInfo.GetByEndPointSystemId(Endpoint.Id, groupCode);

            if (group != null)
            {
                return true;
            }

            return false;
        }

        protected string getSAPDateFormat(DateTime tmpDate)
        {
            string dateStr = string.Empty;
            try
            {
                string yearStr = tmpDate.Year.ToString();
                string monthStr = tmpDate.Month < 10 ? "0" + tmpDate.Month.ToString() : tmpDate.Month.ToString();
                string dayStr = tmpDate.Day < 10 ? "0" + tmpDate.Day.ToString() : tmpDate.Day.ToString();

                dateStr = string.Format("{0}.{1}.{2}", yearStr, monthStr, dayStr);
            }
            catch
            { }

            return dateStr;
        }

        protected List<string> getCompositeAndChildRoleListFromRoleStr(string roleStr)
        {
            List<string> sapRoles = new List<string>();
            List<string> childRoles;
            string[] rolesAry = !string.IsNullOrEmpty(roleStr) ? roleStr.Split(',') : null;
            if (rolesAry != null)
            {
                foreach (string compositeRole in rolesAry)
                {
                    if (!string.IsNullOrEmpty(compositeRole))
                    {
                        sapRoles.Add(compositeRole);
                    }
                }

                childRoles = SAPCompositeRoleInfo.GetChildRolesByCompositeRoleCode(sapRoles);
                if (childRoles != null)
                {
                    foreach (string childRole in childRoles)
                    {
                        sapRoles.Add(childRole);
                    }
                }
            }

            return sapRoles;
        }

        protected List<string> RetainSupplementaryAndGeneralRoles(List<string> roles, AdHocSAPHRFLINSRequest flinsRequest, int endpointUserId)
        {
            List<string> rolesToRemove = new List<string>();
            List<string> rolesToRetain = new List<string>();

            var existingRoles = SAPUserinfo.GetByIdIncludeSAPRoleInfo(endpointUserId).SAPRoleInfo.Select(item => item.ROLE_CODE);

            //find all business roles
            List<string> businessRolesToRetain = new List<string>();

            foreach (string role in existingRoles)
            {
                var matrix = SAPHRFLINSRoleMatrix.GetByRoleCode(role);
                if (matrix != null)
                {
                    bool isMainRole = matrix.MainSupp.Equals("main", StringComparison.CurrentCultureIgnoreCase);

                    if (isMainRole)
                    {
                        if (!flinsRequest.AdHocSAPHRFLINSRequestDetails.Any(item => item.Entitlement.Equals(matrix.BusinessRole, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            businessRolesToRetain.Add(matrix.Company + "," + matrix.BusinessRole);
                        }
                    }
                }
            }

            foreach (string bussinessRole in businessRolesToRetain)
            {
                var bussinessRoleInfo = bussinessRole.Split(',');
                var matrices = SAPHRFLINSRoleMatrix.GetByBussinessRoleAndCompany(bussinessRoleInfo[1], bussinessRoleInfo[0]);
                if (matrices != null)
                {
                    foreach (SAPHRFLINSRoleMatrix matrix in matrices)
                    {
                        rolesToRetain.Add(matrix.TechnicalRule);
                    }
                }
            }

            foreach (AdHocSAPHRFLINSRequestDetail sapDetail in flinsRequest.AdHocSAPHRFLINSRequestDetails)
            {
                //if company roles need to be retained then exclude from the remove list
                if (sapDetail.RetainCompanyRoles)
                {
                    var matrices = SAPHRFLINSRoleMatrix.GetByBussinessRoleAndCompany(sapDetail.Entitlement, sapDetail.Company);
                    if (matrices != null)
                    {
                        foreach (SAPHRFLINSRoleMatrix matrix in matrices)
                        {
                            rolesToRetain.Add(matrix.TechnicalRule);
                        }
                    }
                }
            }

            foreach (string role in roles)
            {
                if (!rolesToRetain.Contains(role))
                {
                    rolesToRemove.Add(role);
                }
            }

            return rolesToRemove;
        }

        protected string GetBusinessRolesFromFLINSRequest(AdHocSAPHRFLINSRequest flinsRequest)
        {
            //find all business roles
            List<string> businessRoles = new List<string>();

            foreach (AdHocSAPHRFLINSRequestDetail sapDetail in flinsRequest.AdHocSAPHRFLINSRequestDetails)
            {
                if (!businessRoles.Contains(sapDetail.Entitlement))
                {
                    businessRoles.Add(sapDetail.Entitlement);
                }
            }

            return string.Join(",", businessRoles);
        }

        protected void TakeOUSnapshot(long userId, IEndpointUserInfo endpointUserInfo, EmployeeEvents Event, EndpointType endpointType, EntitlementType entitlementType,string batchId="")
        {
            string entitlements = string.Empty;

            try
            {
                switch (endpointType)
                {
                    case EndpointType.ActiveDirectory:
                        var oUser = ADSUserInfo.GetByIdIncludeADSGroups(endpointUserInfo.Id);
                        entitlements = string.Join(",", oUser.ADSGroups.Select(item => item.name).ToList());
                        break;
                    case EndpointType.GEMS:
                        var gemsUser = GEMSUserInfo.GetByIdIncludeRoles(endpointUserInfo.Id);
                        entitlements = string.Join(",", gemsUser.GEMSRoles.Select(item => item.Name).ToList());
                        break;
                    case EndpointType.NPAC:
                        var npacUser = NPACUserInfo.GetByUserIdentityIncludeLevels(endpointUserInfo.AccountName);
                        entitlements = $"SECTION:{npacUser.Section?.Trim()}|LEVELS:{string.Join(",", npacUser.NPACLevelInfos.Select(item => item.Level).ToList())}";
                        break;
                    case EndpointType.Office365:
                        var o365User = Office365UserInfo.GetByIdIncludeGroups(endpointUserInfo.Id);
                        entitlements = string.Join(",", o365User.O365Groups.Select(item => item.sAMAccountName).ToList());
                        break;
                    case EndpointType.OneCard:
                        var smsUser = SMSUserInfo.GetByIdIncludeAccesslevels(endpointUserInfo.Id);
                        entitlements = string.Join(",", smsUser.SMSAccessLevels.Select(item => item.Name).ToList());
                        break;
                    case EndpointType.SSIDS:
                        if (entitlementType == EntitlementType.Role)
                        {
                            var ssidUser = SSIDSUserInfo.GetByIdIncludeRoles(endpointUserInfo.Id);
                            entitlements = string.Join(",", ssidUser.SSIDSRolelnfos.Select(item => item.RoleName).ToList());
                        }
                        else if (entitlementType == EntitlementType.HandPhone)
                        {
                            var ssidUser = SSIDSUserInfo.GetById(endpointUserInfo.Id);
                            entitlements = ssidUser.HandPhone;
                        }
                        break;
                    case EndpointType.InhouseSystems:
                        var inhUser = InhouseUserInfo.GetById(endpointUserInfo.Id);
                        var inhUserEntitlements = EntitlementReconcilation.GetByEndpointUser(inhUser.Id, inhUser.EndPointSystemId);
                        _idmsLogger.WriteLog(LogLevelType.Info, $"Inhouse Snapshots: inhUserEntitlements == null: {inhUserEntitlements == null}");
                        var endpoint = DomainModel.EndPoint.GetById(inhUser.EndPointSystemId);
                        if (inhUserEntitlements != null && inhUserEntitlements.Count > 0)
                        {
                            if (endpoint.IsFunctionLevel)
                                entitlements = string.Join("|", inhUserEntitlements.Select(item => item.Entitlement + Constants.SPLITTER_FUCNTIONS + item.Functions).ToList());
                            else
                                entitlements = string.Join(",", inhUserEntitlements.Select(item => item.Entitlement));

                            _idmsLogger.WriteLog(LogLevelType.Info, $"Inhouse Snapshots:{entitlements} RequestId:{this.CurrentRequest?.Id}");
                        }
                        break;

                    case EndpointType.IDMS:
                        var snapShotIDMSUserDetails = new SnapshotUserDetails<SnapshotUser>();
                        var iacmUser = IACMUserInfo.GetById(endpointUserInfo.Id);
                        var diUser = User.GetByEmployeeId(iacmUser.EmployeeId);
                        string terminationDate = string.Empty;
                        if (diUser != null)
                        {
                            terminationDate = diUser.TerminationDate.HasValue ? diUser.TerminationDate.Value.ToString() : string.Empty;
                        }

                        snapShotIDMSUserDetails.UserInfo = new SnapshotUser() { UserName = iacmUser.EmployeeId, Details = terminationDate };
                        var iacmUserEntitlements = EntitlementReconcilation.GetActiveEntitlementByEndpointUserId(iacmUser.Id, iacmUser.EndPointSystemId);
                        foreach (var iacmUserEntitlement in iacmUserEntitlements)
                        {
                            snapShotIDMSUserDetails.Entitlements.Add(new SnapshotEntitlement()
                            {
                                Entitlement = iacmUserEntitlement.Entitlement,
                                EntitlementType = (EntitlementType)iacmUserEntitlement.EntitlementType,
                                Function = iacmUserEntitlement.Functions
                            });
                        }
                        entitlements = JsonConvert.SerializeObject(snapShotIDMSUserDetails);
                        //entitlements = string.Join("|", iacmUserEntitlements.Select(item => item.EntitlementType + ":-" + item.Entitlement + "-" + item.Functions).ToList());
                        break;
                    case EndpointType.VPN:
                        var user = User.GetById(userId);
                        
                        if (user != null && !string.IsNullOrWhiteSpace(user.OneKeyTokenId))
                        {
                            var snapShotVPNUserDetails = new SnapshotUserDetails<SnapshotUser>();
                            snapShotVPNUserDetails.UserInfo = new SnapshotUser() { UserName = user.OneKeyTokenId, Details = string.Empty };
                            VPNUserInfo vpnUser = null;
                            if (endpointUserInfo != null && endpointUserInfo.Id > 0)
                            {
                                vpnUser = VPNUserInfo.GetById(endpointUserInfo.Id);
                            }
                            if (endpointUserInfo != null && vpnUser != null)
                            {
                                var vpnUserEntitlements = EntitlementReconcilation.GetActiveEntitlementByEndpointUserId(vpnUser.Id, vpnUser.EndPointSystemId);
                                if (vpnUserEntitlements != null && vpnUserEntitlements.Count > 0)
                                {
                                    foreach (var vpnUserEntitlement in vpnUserEntitlements)
                                    {
                                        snapShotVPNUserDetails.Entitlements.Add(new SnapshotEntitlement() { Entitlement = vpnUserEntitlement.Entitlement, EntitlementType = (EntitlementType)vpnUserEntitlement.EntitlementType, Function = vpnUserEntitlement.Functions });
                                    }
                                }
                            }
                            entitlements = JsonConvert.SerializeObject(snapShotVPNUserDetails);
                        }
                        break;
                    case EndpointType.MicrosoftTeams:
                        entitlements = string.Empty;
                        var sfbUser = SkypeForBusinessUserInfo.GetById(endpointUserInfo.Id);
                        var sfbUserEntitlements = EntitlementReconcilation.GetActiveEntitlementByEndpointUserId(sfbUser.Id, sfbUser.EndPointSystemId);
                        bool hasIDD = false;
                        if (sfbUser.UserType == EntitlementType.DCCS)
                        {
                            var iddEntitlement = sfbUserEntitlements.FirstOrDefault(i => i.EntitlementType == (int)EntitlementType.IDD);
                            if (iddEntitlement != null)
                            {
                                hasIDD = true;
                            }
                        }
                        SnapshotUserDetails<SnapshotUser> snapShot = new SnapshotUserDetails<SnapshotUser>();
                        snapShot.UserInfo = new SnapshotUser() { UserName = sfbUser.UserPrincipalName, Details = $"{sfbUser.LineURI}|{sfbUser.LineURIExtension}", UserType = sfbUser.UserType.ToString(), HasIDD = hasIDD };

                        if (sfbUserEntitlements.Count > 0)
                        {
                            foreach (var sfbUserEntitlement in sfbUserEntitlements)
                            {
                                snapShot.Entitlements.Add(new SnapshotEntitlement() { Entitlement = sfbUserEntitlement.Entitlement, EntitlementType = (EntitlementType)sfbUserEntitlement.EntitlementType, Function = sfbUserEntitlement.Functions });
                            }
                        }
                        entitlements = JsonConvert.SerializeObject(snapShot);
                        break;
                    case EndpointType.EAM_NOVA:
                        var novaUserInfo = SAPEamNovaUserInfo.GetById(endpointUserInfo.Id);
                        if (novaUserInfo != null)
                        {
                            SnapshotUserDetails<SnapshotUser> snapShot_EAM = new SnapshotUserDetails<SnapshotUser>();
                            snapShot_EAM.UserInfo = new SnapshotUser() { UserName = novaUserInfo.AccountName };

                            var novaUserEntitlements = EntitlementReconcilation.GetByEndpointUserId(novaUserInfo.Id, novaUserInfo.EndPointSystemId);
                            novaUserEntitlements.ForEach(item =>
                            {
                                snapShot_EAM.Entitlements.Add(new SnapshotEntitlement() { Entitlement = item.Functions, EntitlementType = EntitlementType.Role });
                            });
                            entitlements = JsonConvert.SerializeObject(snapShot_EAM);
                        }
                        break;
                }

                LogDebugStep("Inhouse saving snapshot 1");
                OUChangeSnapshot snapshot = new OUChangeSnapshot()
                {
                    BatchId = !string.IsNullOrWhiteSpace(batchId)? batchId : DateTime.Today.ToString("yyyyMMdd"),
                    EnteredDate = DateTime.Now,
                    EndpointSystemId = endpointUserInfo.EndPointSystemId,
                    EndpointUserId = endpointUserInfo.Id,
                    Entitlement = entitlements,
                    EntitlementType = entitlementType,
                    UserId = userId,
                    EmployeeeEventType = Event,
                    AccountName = endpointUserInfo.AccountName,
                    AccountType = CurrentRequest.AccountType,
                    RequestId = this.CurrentRequest.Id,
                };

                LogDebugStep("Inhouse saving snapshot 2");
                snapshot.Save(MasterConfigHelperServiceUser);
                LogDebugStep("Inhouse snapshot saved");
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Exception at TakeOUSnapshot1()", ex);
                throw ex;
            }
        }

        OUChangeSnapshot getOUChangeSnapshot(User user, SAPUserinfo endpointUserInfo, EmployeeEvents employeeEvent)
        {
            return new OUChangeSnapshot()
            {
                BatchId = DateTime.Today.ToString("yyyyMMdd"),
                EnteredDate = DateTime.Now,
                EndpointSystemId = endpointUserInfo.EndPointSystemId,
                EndpointUserId = endpointUserInfo.Id,
                UserId = user.Id,
                EmployeeeEventType = employeeEvent,
                AccountName = endpointUserInfo.AccountName,
                AccountType = CurrentRequest.AccountType,
                RequestId = CurrentRequest.Id,
                AdditionalInfo = JsonConvert.SerializeObject(new SnapshotUser() { UserName = endpointUserInfo.AccountName, ValidFrom = endpointUserInfo.validFromDate, ValidTo = endpointUserInfo.validToDate })
            };
        }

        OUChangeSnapshot getOUChangeSnapshot(User user, SAPEPUserInfo endpointUserInfo, EmployeeEvents employeeEvent)
        {
            return new OUChangeSnapshot()
            {
                BatchId = DateTime.Today.ToString("yyyyMMdd"),
                EnteredDate = DateTime.Now,
                EndpointSystemId = endpointUserInfo.EndPointSystemId,
                EndpointUserId = endpointUserInfo.Id,
                UserId = user.Id,
                EmployeeeEventType = employeeEvent,
                AccountName = endpointUserInfo.AccountName,
                AccountType = CurrentRequest.AccountType,
                RequestId = CurrentRequest.Id
            };
        }
        protected void TakeOUSnapshot(User user, SAPUserinfo endpointUserInfo, EmployeeEvents employeeEvent)
        {
            try
            {
                var usergroups = SAPUserinfo.GetByIdIncludeSAPGroupInfo(endpointUserInfo.Id);
                var userroles = SAPUserinfo.GetByIdIncludeSAPRoleInfo(endpointUserInfo.Id);
                var userprofiles = SAPUserinfo.GetByIdIncludeSAPProfileInfo(endpointUserInfo.Id);
                var userpositions = SAPUserinfo.GetPositionTextByUser(endpointUserInfo.UserLogonName, endpointUserInfo.EndPointSystemId);

                string group = string.Join(",", usergroups.SAPGroupInfos.Select(item => item.GROUP_CODE).ToList());
                string role = string.Join(",", userroles.SAPRoleInfo.Select(item => item.ROLE_CODE).ToList());
                string profile = string.Join(",", userprofiles.SAPProfileInfos.Select(item => item.PROFILE_CODE).ToList());
                string position = string.Join(",", userpositions);



                if (string.IsNullOrEmpty(group) && string.IsNullOrEmpty(role) && string.IsNullOrEmpty(profile) && string.IsNullOrEmpty(position))
                {
                    OUChangeSnapshot snapshot = getOUChangeSnapshot(user, endpointUserInfo, employeeEvent);
                    snapshot.Save("System");
                }

                if (!string.IsNullOrEmpty(group))
                {
                    OUChangeSnapshot snapshot = getOUChangeSnapshot(user, endpointUserInfo, employeeEvent);
                    snapshot.Entitlement = group;
                    snapshot.EntitlementType = EntitlementType.Group;
                    snapshot.Save("System");
                }

                if (!string.IsNullOrEmpty(role))
                {
                    OUChangeSnapshot snapshot = getOUChangeSnapshot(user, endpointUserInfo, employeeEvent);
                    snapshot.Entitlement = role;
                    snapshot.EntitlementType = EntitlementType.Role;
                    snapshot.Save("System");
                }

                if (!string.IsNullOrEmpty(profile))
                {
                    OUChangeSnapshot snapshot = getOUChangeSnapshot(user, endpointUserInfo, employeeEvent);
                    snapshot.Entitlement = profile;
                    snapshot.EntitlementType = EntitlementType.Profile;
                    snapshot.Save("System");
                }

                if (!string.IsNullOrEmpty(position))
                {
                    OUChangeSnapshot snapshot = getOUChangeSnapshot(user, endpointUserInfo, employeeEvent);
                    snapshot.Entitlement = position;
                    snapshot.EntitlementType = EntitlementType.Position;
                    snapshot.Save("System");
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Exception at TakeOUSnapshot2()", ex);
                throw ex;
            }
        }

        protected void TakeOUSnapshot(User user, SAPUserinfo endpointUserInfo, EmployeeEvents employeeEvent, List<SRMUserRoleMappingData> srmMappingData, ApplicationType applicationType)
        {
            try
            {
                var srmSnapShot = new SRMSnapShot();
                srmSnapShot.SRMUserRoles = srmMappingData;
                string mappingData = JsonConvert.SerializeObject(srmSnapShot);
                OUChangeSnapshot snapshot = new OUChangeSnapshot()
                {
                    BatchId = DateTime.Today.ToString("yyyyMMdd"),
                    EnteredDate = DateTime.Now,
                    EndpointSystemId = endpointUserInfo.EndPointSystemId,
                    EndpointUserId = endpointUserInfo.Id,
                    UserId = user.Id,
                    EmployeeeEventType = employeeEvent,
                    AccountName = endpointUserInfo.AccountName,
                    AccountType = CurrentRequest.AccountType,
                    AdditionalInfo = null,
                    RequestId = this.CurrentRequest.Id
                };
                snapshot.Entitlement = mappingData;
                snapshot.EntitlementType = EntitlementType.Role;
                snapshot.Save("System");
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"Exception at TakeOUSnapshot3.{ex.StackTrace}", ex);
                throw ex;
            }
        }

        protected void TakeOUSnapshot(User user, SAPEPUserInfo endpointUserInfo, EmployeeEvents employeeEvent)
        {

            var sapepceUser = DomainModel.SAPEPUserInfo.GetById(endpointUserInfo.Id, true);
            var groups = sapepceUser.SAPEPGroups;
            var roles = sapepceUser.SAPEPRoles;

            string group = string.Join(",", groups.Select(item => item.GroupUniqueName).ToList());
            string role = string.Join(",", roles.Select(item => item.UniqueRoleName).ToList());



            if (string.IsNullOrEmpty(group) && string.IsNullOrEmpty(role))
            {
                OUChangeSnapshot snapshot = getOUChangeSnapshot(user, endpointUserInfo, employeeEvent);
                snapshot.Save(MasterConfigHelperServiceUser);
            }

            if (!string.IsNullOrEmpty(group))
            {
                OUChangeSnapshot snapshot = getOUChangeSnapshot(user, endpointUserInfo, employeeEvent);
                snapshot.Entitlement = group;
                snapshot.EntitlementType = EntitlementType.Group;
                snapshot.Save(MasterConfigHelperServiceUser);
            }

            if (!string.IsNullOrEmpty(role))
            {
                OUChangeSnapshot snapshot = getOUChangeSnapshot(user, endpointUserInfo, employeeEvent);
                snapshot.Entitlement = role;
                snapshot.EntitlementType = EntitlementType.Role;
                snapshot.Save(MasterConfigHelperServiceUser);
            }
        }

        

        protected void CreateDependentSystemEntriesInIDMS(SAPUserinfo sapUser, User user)
        {
            try
            {
                var sapepEndpoint = EndPoint.GetByApplicationType(ApplicationType.SAP_EP);
                var sapceEndpoint = EndPoint.GetByApplicationType(ApplicationType.CE);

                SAPEPUserInfo sapepUser = SAPEPUserInfo.GetByUserName(sapUser.UserLogonName, sapepEndpoint.Id);
                SAPEPUserInfo sapceUser = SAPEPUserInfo.GetByUserName(sapUser.UserLogonName, sapceEndpoint.Id);


                if (sapepUser == null && sapepEndpoint.IsActive)
                {
                    sapepUser = GetSAPEPUserByFLINSId(sapUser, sapepEndpoint, user);
                    List<SAPEPUserInfo> sapepUsers = new List<SAPEPUserInfo>();
                    sapepUsers.Add(sapepUser);

                    SAPEPUserInfo.SaveUsers(sapepUsers, sapepEndpoint.Id, MasterConfigHelperServiceUser, (long)sapepEndpoint.EndpointSystemTypeId, this.CurrentRequest.UserId, this.CurrentRequest.AccountType);
                }

                if (sapceUser == null && sapceEndpoint.IsActive)
                {
                    sapceUser = GetSAPEPUserByFLINSId(sapUser, sapceEndpoint, user);
                    List<SAPEPUserInfo> sapepUsers = new List<SAPEPUserInfo>();
                    sapepUsers.Add(sapceUser);

                    SAPEPUserInfo.SaveUsers(sapepUsers, sapceEndpoint.Id, MasterConfigHelperServiceUser, (long)sapceEndpoint.EndpointSystemTypeId, this.CurrentRequest.UserId, this.CurrentRequest.AccountType);
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Exception at CreateDependentSystemEntriesInIDMS()", ex);
            }
        }

        SAPEPUserInfo GetSAPEPUserByFLINSId(SAPUserinfo sapUser, EndPoint endpoint, User user)
        {
            SAPEPUserInfo sapEPUserValue = new SAPEPUserInfo();

            sapEPUserValue.UserName = sapUser.UserLogonName;
            sapEPUserValue.UniqueId = sapUser.UserLogonName;
            sapEPUserValue.DisplayName = sapUser.Lastname;
            sapEPUserValue.UniqueName = sapUser.UserLogonName;

            sapEPUserValue.JobTitle = sapUser.Function0;
            sapEPUserValue.LastName = sapUser.Lastname;
            sapEPUserValue.Email = sapUser.E_Mail;
            sapEPUserValue.Department = sapUser.Department;

            sapEPUserValue.EndPointSystemId = endpoint.Id;

            sapEPUserValue.StartDate = CurrentRequest.ActivationDate;
            sapEPUserValue.EndDate = GetValidTo(user.Id);

            sapEPUserValue.CreatedBy = ProvisioningConstants.ProcessName;
            sapEPUserValue.CreatedDate = DateTime.Now;


            return sapEPUserValue;
        }

        protected string ConstructPassword()
        {

            var endpointPolicy = PasswordPolicyEndpoint.GetEndpointPasswordPolicy(Endpoint.Id);

            var policy = endpointPolicy.PasswordPolicy;

            string password = new PasswordResolver().ConstructGeneralUserPassword
                (
                    policy.PasswordLength.HasValue ? policy.PasswordLength.Value : 8,
                    policy.NoOfSpecialCharacters.HasValue ? policy.NoOfSpecialCharacters.Value : 0,
                    policy.NoOfLowerCaseAlphabets.HasValue ? policy.NoOfLowerCaseAlphabets.Value : 0,
                    policy.NoOfUpperCaseAlphabets.HasValue ? policy.NoOfUpperCaseAlphabets.Value : 0,
                    policy.NoOfNumericCharacters.HasValue ? policy.NoOfNumericCharacters.Value : 0
                );

            return password;
        }
        protected string ConstructPassword(long endpointId)
        {

            var endpointPolicy = PasswordPolicyEndpoint.GetEndpointPasswordPolicy(endpointId);

            var policy = endpointPolicy.PasswordPolicy;

            string password = new PasswordResolver().ConstructGeneralUserPassword
                (
                    policy.PasswordLength.HasValue ? policy.PasswordLength.Value : 8,
                    policy.NoOfSpecialCharacters.HasValue ? policy.NoOfSpecialCharacters.Value : 0,
                    policy.NoOfLowerCaseAlphabets.HasValue ? policy.NoOfLowerCaseAlphabets.Value : 0,
                    policy.NoOfUpperCaseAlphabets.HasValue ? policy.NoOfUpperCaseAlphabets.Value : 0,
                    policy.NoOfNumericCharacters.HasValue ? policy.NoOfNumericCharacters.Value : 0
                );

            return password;
        }
        public virtual bool CreateUserInReconTable(IEndpointUserInfo epUser, string currentUser)
        {
            EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(epUser.EndPointSystemId);
            if (ep != null)
            {
                var existingAcctRecon = AccountReconcilation.GetByAccountName(epUser.AccountName, ep.Id);
                if (existingAcctRecon != null)
                {
                    existingAcctRecon.AccessRequestId = this.CurrentRequest.Id;
                    existingAcctRecon.ValidFromDate = CurrentRequest.ActivationDate;
                    if (CurrentRequest.AdHocAccessRequestId.HasValue && CurrentRequest.AdHocAccessRequest != null && CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        existingAcctRecon.ValidToDate = GetValidTo(CurrentRequest.UserId);
                        existingAcctRecon.UserId = CurrentRequest.UserId;
                    }
                    existingAcctRecon.AccountCreatedDate = DateTime.Now;
                    if (existingAcctRecon.IsDeleted)
                    {
                        existingAcctRecon.IsLocked = false;
                        existingAcctRecon.IsDeleted = false;
                        existingAcctRecon.AccountCreatedDate = DateTime.Now;
                        existingAcctRecon.DeletedDate = null;
                        existingAcctRecon.LastAttestedOn = null;
                        existingAcctRecon.LastLogonDate = null;
                        existingAcctRecon.AccountType = CurrentRequest.AccountType != AccountType.None ? CurrentRequest.AccountType : AccountType.User;
                        
                        existingAcctRecon.EndpointUserId = epUser.Id;
                        
                    }
                    // set mapping for global user
                    existingAcctRecon.FullName = !string.IsNullOrWhiteSpace(epUser.DisplayName) ? epUser.DisplayName : epUser.AccountName;

                    existingAcctRecon.MappingType = existingAcctRecon.AccountType == AccountType.User ? DI.Chakra.Infrastructure.Enums.MappingType.Auto : DI.Chakra.Infrastructure.Enums.MappingType.Manual;
                    existingAcctRecon.OffboardingAccountAction = (existingAcctRecon.AccountType == AccountType.User || existingAcctRecon.AccountType == AccountType.Secondary) ? OffboardingAccountActionType.Delete : OffboardingAccountActionType.Supervisor;
                    
                    //existingAcctRecon.UserId = CurrentRequest.UserId;
                    existingAcctRecon.Save(currentUser);
                }
                else
                {
                    AccountReconcilation recon = new AccountReconcilation();
                    recon.AccessRequestId = this.CurrentRequest.Id;
                    recon.UserId = CurrentRequest.UserId;
                    recon.AccountName = epUser.AccountName;
                    recon.FullName = !string.IsNullOrWhiteSpace(epUser.DisplayName) ? epUser.DisplayName : epUser.AccountName;
                    recon.EndpointSystemId = epUser.EndPointSystemId;
                    recon.AccountType = CurrentRequest.AccountType != AccountType.None ? CurrentRequest.AccountType : AccountType.User;
                    recon.MappingType = recon.AccountType == AccountType.User ? DI.Chakra.Infrastructure.Enums.MappingType.Auto : DI.Chakra.Infrastructure.Enums.MappingType.Manual;
                    recon.OffboardingAccountAction = (recon.AccountType == AccountType.User || recon.AccountType == AccountType.Secondary) ? OffboardingAccountActionType.Delete : OffboardingAccountActionType.Supervisor;
                    recon.EndpointUserId = epUser.Id;
                    recon.ValidFromDate = CurrentRequest.ActivationDate;
                    recon.ValidToDate = GetValidTo(CurrentRequest.UserId);
                    recon.AccountCreatedDate = DateTime.Now;
                    recon.Save(currentUser);

                }

            }
            return true;
        }

        protected virtual string getGroupListAsString(IList<string> grpList)
        {
            StringBuilder grpSB = new StringBuilder();
            string grpStr = string.Empty;
            string StringFormatter = ", {0}";
            if (grpList == null)
                return grpStr;

            foreach (string tmpGrpStr in grpList)
            {
                if (!string.IsNullOrWhiteSpace(tmpGrpStr))
                {
                    grpSB.Append(string.Format(StringFormatter, tmpGrpStr));
                }
            }
            grpStr = grpSB.Length > 1 ? grpSB.ToString().Substring(2, grpSB.Length - 2) : string.Empty;
            return grpStr;
        }

        protected void RemoveUserBusinessRoleMapping(IEndpointUserInfo _userInfo, string businessRole)
        {
            var roles = businessRole.Split(',').ToList();
            foreach (string role in roles)
            {
                if (string.IsNullOrWhiteSpace(role)) continue;
                var existbusinessroles = UserBusinessRoleMapping.GetBussRoleMappingbyRoleName(_userInfo.Id, _userInfo.EndPointSystemId, role);
                if (existbusinessroles != null)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Removal of Business Role from businessRoleMapping. EndpointUserId :{_userInfo.Id},EndpointSystemId:{_userInfo.EndPointSystemId} and BusinessRole :{existbusinessroles.BusinessRole} ");
                    UserBusinessRoleMapping.RemoveUserBussRoleMappingRecord(existbusinessroles);
                }
            }
        }

        protected void CreateUserBusinessRoleMapping(IEndpointUserInfo _userInfo, string businessRole)
        {

            var roles = businessRole.Split(',').ToList();
            foreach (string role in roles)
            {
                if (string.IsNullOrWhiteSpace(role)) continue;
                var roleMapping = this.CreateUserBusinessRoleMappingObjectShell(_userInfo, role);
                if (roleMapping != null)
                {
                    roleMapping.Save("System");
                }
            }
        }

        private UserBusinessRoleMapping CreateUserBusinessRoleMappingObjectShell(IEndpointUserInfo _userInfo, string businessRole)
        {
            var existroles = UserBusinessRoleMapping.GetAllBussRoleMappingRecordsForUser(_userInfo.Id, _userInfo.EndPointSystemId);
            UserBusinessRoleMapping roleMapping = null;
            if (!existroles.Contains(businessRole))
            {
                roleMapping = new UserBusinessRoleMapping()
                {
                    BusinessRole = businessRole,
                    UserId = CurrentRequest.UserId,
                    EndpointUserId = _userInfo.Id,
                    EndpointSystemId = _userInfo.EndPointSystemId,
                    CreatedBy = "System",
                    CreatedDate = DateTime.Now,
                };

            }
            return roleMapping;
        }

        protected void WriteProvisioningLog(LogLevelType loglevel, string messageText, Exception ex = null)
        {
            _idmsLogger.WriteLog(loglevel, messageText, ex);
        }

        protected void WriteProvisioningLog(long endpointId, LogLevelType loglevel, string messageText, Exception ex = null)
        {
            _idmsLogger.WriteLog(loglevel, messageText, ex);
        }
        public virtual bool ExecuteEnableCommand(ProvisioningParamCollection fields, DomainModel.User user, bool fromAccessRequest = false)
        {
            return true;
        }
        public virtual bool ExecuteUserAccountPolicy(ProvisioningParamCollection fields, DomainModel.User user, bool accountExists)
        {
            return true;
        }
        public virtual bool UpdateAttributes(DomainModel.User user,long endpointUserId)
        {
            return true;
        }
        protected void TakeOUSnapshotForSRPSUP(User user, SAPUserinfo endpointUserInfo, EmployeeEvents employeeEvent)
        {
            try
            {
                var accountRecon = AccountReconcilation.GetByAccountName(endpointUserInfo.UserLogonName, endpointUserInfo.EndPointSystemId);
                if(accountRecon != null && (accountRecon.IsDeleted == false || accountRecon.IsLocked == false))
                {
                    var entitlements = EntitlementReconcilation.GetAllActiveByAccountReconcilationId(accountRecon.Id);
                    if (entitlements != null && entitlements.Count > 0)
                    {
                        string role = string.Join(",", entitlements.Select(i => i.Entitlement));
                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $"TakeOUSnapshotForSRPSUP for User:{endpointUserInfo.UserLogonName} and Entitlements:{role}");
                            OUChangeSnapshot snapshot = getOUChangeSnapshot(user, endpointUserInfo, employeeEvent);
                            snapshot.Entitlement = role;
                            snapshot.EntitlementType = EntitlementType.Role;
                            snapshot.Save("System");
                            _idmsLogger.WriteLog(LogLevelType.Info, $"TakeOUSnapshotForSRPSUP for User:{endpointUserInfo.UserLogonName} and Entitlements:{role} completed");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Exception at TakeOUSnapshotForSRPSUP failed", ex);
            }
        }

        public virtual bool IsUserExistsInOnline(string accountName)
        {
            return true;
        }
        public virtual bool IsMailBoxExists(string accountName)
        {
            return true;
        }
    }
}
