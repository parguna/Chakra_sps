
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Services.EmailingProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DI.Chakra.ProvisioningUtility
{
    public class ProvisioningEmailHelper
    {
        const string DATETIMEFORMAT = "dd/MM/yyyy hh:mm tt";

        public static bool SendEmailForNPACSectionNOTFound(User user)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();
                MailTemplate template = GetMailTemplate(Constants.NPACSectionNOTFound);
                StringBuilder subject = new StringBuilder(template.MailSubject);
                //subject.Replace("@@Section@@", user.SectionName);
                subject.Replace("@@UserName@@", user.DisplayName);

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@Section@@", user.SectionName);
                body.Replace("@@UserName@@", user.DisplayName);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = template.MailTo;
                emailInfo.From = template.MailFrom;
                emailInfo.Priority = "High";
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Exception thrown from SendEmailForNPACSectionNOTFound(User user) : User - {user.Id}", ex, null, string.Empty);
                return false;
            }
        }
        public static bool SendEmailForAccountProvisioning(AccessRequest accessRequest, IEndpointUserInfo _userInfo, User user, ApplicationType applicationType,string passwordForNPAC=null, bool isForSAPSSO = false)
        {
            string division = string.Empty;
            string section = string.Empty;

            MailTemplate template = null;
            StringBuilder subject = null;

            try
            {
                var appDisplayName = string.Empty;

                var endpoint = ApplicationNameWithEndpoint(accessRequest.EndpointSystemId.Value);
                EmailInfo emailInfo = new EmailInfo();

                appDisplayName = ApplicationName(accessRequest.EndpointSystemId.Value);

                var fullDisplayName = string.Empty;

                if (endpoint != null && endpoint.ApplicationType == ApplicationType.EAM)
                {
                    var endp = EndPoint.GetById(_userInfo.EndPointSystemId);
                    if (endp != null && endp.ApplicationType != ApplicationType.EAM)
                    {

                        if (endp.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                        {
                            fullDisplayName = $"AD Account";
                        }

                        if (endp.ApplicationType == ApplicationType.HRMS_FLINS)
                        {
                            fullDisplayName = $"HRMS -FLINS Account";
                        }

                        if (endp.ApplicationType == ApplicationType.FLINS_BI)
                        {
                            fullDisplayName = $"FLINS BI Account";
                        }
                    }
                }
                if (endpoint != null && endpoint.ApplicationType == ApplicationType.SRM)
                {
                    if (applicationType == ApplicationType.SAP_SRP)
                    {
                        fullDisplayName = $"SRP Account";
                    }

                    if (applicationType == ApplicationType.SAP_SUS)
                    {
                        fullDisplayName = $"SUS Account";
                    }

                    if (applicationType == ApplicationType.FLINS_BI)
                    {
                        fullDisplayName = $"FLINS BI Account";
                    }
                }
                var emailSubject = appDisplayName;
                if (!string.IsNullOrWhiteSpace(fullDisplayName))
                {
                    emailSubject = $"{appDisplayName} - {fullDisplayName}";
                }
                else
                {
                    fullDisplayName = appDisplayName;
                }
                if (accessRequest.RequestSource == AccessRequestSource.AdhocRequest)
                {
                    template = GetMailTemplate(Constants.CREATE_ACCOUNT_NOTIFICATION_COMMON_TEMPLATE);

                    subject = new StringBuilder(template.MailSubject);

                    subject.Replace("@@APPLICATIONNAME@@", emailSubject);
                    subject.Replace("@@REQUESTTYPE@@", accessRequest.AdHocAccessRequest.AdhocActionType.ToString());
                    subject.Replace("@@REQUESTID@@", accessRequest.Id.ToString());
                    subject.Replace("@@USERDISPLAYNAME@@", user.DisplayName);
                    if (!string.IsNullOrEmpty(user.EmployeeId))
                        subject.Replace("@@EMPLOYEEID@@", Convert.ToString(user.EmployeeId));

                    subject.Replace("@@USERNAME@@", _userInfo.AccountName);
                }
                else
                {

                    template = GetMailTemplate(Constants.OnboardingUserEmail);

                    subject = new StringBuilder(template.MailSubject);

                    subject.Replace("@@APPLICATIONNAME@@", emailSubject);
                    if (!string.IsNullOrEmpty(user.EmployeeId))
                        subject.Replace("@@EMPLOYEEID@@", Convert.ToString(user.EmployeeId));

                    subject.Replace("@@USERNAME@@", _userInfo.AccountName);
                    fullDisplayName = string.Empty;
                }
                //template.MailContent.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                string body = GetNewAccountTemplate(template.MailContent, accessRequest, _userInfo, user, applicationType, appDisplayName, fullDisplayName, endpoint, passwordForNPAC, isForSAPSSO);
                //body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Body = body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Subject = subject.ToString();
                emailInfo.From = !string.IsNullOrWhiteSpace(template.MailFrom) ? template.MailFrom : MasterConfigHelper.AccountCreatedNotificationCC;

                if (accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.IsSelfRequest == false)
                {
                    emailInfo.To = $"{user.EmailId},{template.MailTo}";
                    SendEmailToRequestBy(user, accessRequest, _userInfo.AccountName);
                }
                else
                {
                    emailInfo.To = $"{user.EmailId},{template.MailTo}";
                }
                emailInfo.Cc = $"{MasterConfigHelper.AccountCreatedNotificationCC},{template.MailCC}";
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailForAccountProvisioning : AccessRequest Id - {accessRequest.Id}", ex, null, string.Empty);
                return false;
            }

            return true;
        }
        /// <summary>
        /// SP asked to revert the changes. No more email separation
        /// </summary>
        /// <param name="accessRequest"></param>
        /// <param name="_userInfo"></param>
        /// <param name="user"></param>
        /// <param name="applicationType"></param>
        /// <param name="passwordForNPAC"></param>
        /// <param name="isForSAPSSO"></param>
        /// <returns></returns>
        [Obsolete]
        public static bool SendEmailForAccountProvisioningOnlyAccountName(AccessRequest accessRequest, IEndpointUserInfo _userInfo, User user, ApplicationType applicationType, string passwordForNPAC = null, bool isForSAPSSO = false)
        {
            //string division = string.Empty;
            //string section = string.Empty;

            //MailTemplate template = null;
            //StringBuilder subject = null;

            //try
            //{
            //    var appDisplayName = string.Empty;

            //    var endpoint = ApplicationNameWithEndpoint(accessRequest.EndpointSystemId.Value);
            //    EmailInfo emailInfo = new EmailInfo();

            //    appDisplayName = ApplicationName(accessRequest.EndpointSystemId.Value);

            //    var fullDisplayName = string.Empty;

            //    if (endpoint != null && endpoint.ApplicationType == ApplicationType.EAM)
            //    {
            //        var endp = EndPoint.GetById(_userInfo.EndPointSystemId);
            //        if (endp != null && endp.ApplicationType != ApplicationType.EAM)
            //        {

            //            if (endp.ApplicationType == ApplicationType.ADSingaporePowerLocal)
            //            {
            //                fullDisplayName = $"AD Account";
            //            }

            //            if (endp.ApplicationType == ApplicationType.HRMS_FLINS)
            //            {
            //                fullDisplayName = $"HRMS -FLINS Account";
            //            }

            //            if (endp.ApplicationType == ApplicationType.FLINS_BI)
            //            {
            //                fullDisplayName = $"FLINS BI Account";
            //            }
            //        }
            //    }
            //    if (endpoint != null && endpoint.ApplicationType == ApplicationType.SRM)
            //    {
            //        if (applicationType == ApplicationType.SAP_SRP)
            //        {
            //            fullDisplayName = $"SRP Account";
            //        }

            //        if (applicationType == ApplicationType.SAP_SUS)
            //        {
            //            fullDisplayName = $"SUS Account";
            //        }

            //        if (applicationType == ApplicationType.FLINS_BI)
            //        {
            //            fullDisplayName = $"FLINS BI Account";
            //        }
            //    }
            //    var emailSubject = appDisplayName;
            //    if (!string.IsNullOrWhiteSpace(fullDisplayName))
            //    {
            //        emailSubject = $"{appDisplayName} - {fullDisplayName}";
            //    }
            //    else
            //    {
            //        fullDisplayName = appDisplayName;
            //    }
            //    if (accessRequest.RequestSource == AccessRequestSource.AdhocRequest)
            //    {
            //        template = GetMailTemplate(Constants.CREATE_ACCOUNT_NOTIFICATION_COMMON_TEMPLATE);

            //        subject = new StringBuilder(template.MailSubject);

            //        subject.Replace("@@APPLICATIONNAME@@", emailSubject);
            //        subject.Replace("@@REQUESTTYPE@@", accessRequest.AdHocAccessRequest.AdhocActionType.ToString());
            //        subject.Replace("@@REQUESTID@@", accessRequest.Id.ToString());
            //        subject.Replace("@@USERDISPLAYNAME@@", user.DisplayName);
            //        if (!string.IsNullOrEmpty(user.EmployeeId))
            //            subject.Replace("@@EMPLOYEEID@@", Convert.ToString(user.EmployeeId));

            //        subject.Replace("@@USERNAME@@", _userInfo.AccountName);
            //    }
            //    else
            //    {

            //        template = GetMailTemplate(Constants.OnboardingUserEmail);

            //        subject = new StringBuilder(template.MailSubject);

            //        subject.Replace("@@APPLICATIONNAME@@", emailSubject);
            //        if (!string.IsNullOrEmpty(user.EmployeeId))
            //            subject.Replace("@@EMPLOYEEID@@", Convert.ToString(user.EmployeeId));

            //        subject.Replace("@@USERNAME@@", _userInfo.AccountName);
            //        fullDisplayName = string.Empty;
            //    }
            //    //template.MailContent.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
            //    string body = GetNewAccountTemplate(template.MailContent, accessRequest, _userInfo, user, applicationType, appDisplayName, fullDisplayName, endpoint, passwordForNPAC, isForSAPSSO);
            //    //body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
            //    emailInfo.Body = body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
            //    emailInfo.Subject = subject.ToString();
            //    emailInfo.From = !string.IsNullOrWhiteSpace(template.MailFrom) ? template.MailFrom : MasterConfigHelper.AccountCreatedNotificationCC;

            //    if (accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.IsSelfRequest == false)
            //    {
            //        emailInfo.To = $"{user.EmailId},{template.MailTo}";
            //        SendEmailToRequestBy(user, accessRequest, _userInfo.AccountName);
            //    }
            //    else
            //    {
            //        emailInfo.To = $"{user.EmailId},{template.MailTo}";
            //    }
            //    emailInfo.Cc = $"{MasterConfigHelper.AccountCreatedNotificationCC},{template.MailCC}";
            //    emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
            //    emailInfo.CreatedBy = "System";
            //    emailInfo.CreatedDate = DateTime.Now;
            //    emailInfo.CreatedDateTime = DateTime.Now;
            //    EmailHandler.Send(emailInfo);
            //}
            //catch (Exception ex)
            //{
            //    DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailForAccountProvisioning : AccessRequest Id - {accessRequest.Id}", ex, null, string.Empty);
            //    return false;
            //}

            return true;
        }
        /// <summary>
        /// Not required
        /// </summary>
        /// <param name="accessRequest"></param>
        /// <param name="_userInfo"></param>
        /// <param name="user"></param>
        /// <param name="applicationType"></param>
        /// <param name="passwordForNPAC"></param>
        /// <param name="isForSAPSSO"></param>
        /// <returns></returns>
        [Obsolete]
        public static bool SendEmailForAccountProvisioningOnlyPassword(AccessRequest accessRequest, IEndpointUserInfo _userInfo, User user, ApplicationType applicationType, string passwordForNPAC = null, bool isForSAPSSO = false)
        {
            //string division = string.Empty;
            //string section = string.Empty;

            //MailTemplate template = null;
            //StringBuilder subject = null;
            //try
            //{
            //    var appDisplayName = string.Empty;

            //    var endpoint = ApplicationNameWithEndpoint(accessRequest.EndpointSystemId.Value);
            //    EmailInfo emailInfo = new EmailInfo();

            //    appDisplayName = ApplicationName(accessRequest.EndpointSystemId.Value);

            //    var fullDisplayName = string.Empty;

            //    if (endpoint != null && endpoint.ApplicationType == ApplicationType.EAM)
            //    {
            //        var endp = EndPoint.GetById(_userInfo.EndPointSystemId);
            //        if (endp != null && endp.ApplicationType != ApplicationType.EAM)
            //        {

            //            if (endp.ApplicationType == ApplicationType.ADSingaporePowerLocal)
            //            {
            //                fullDisplayName = $"AD Account";
            //            }

            //            if (endp.ApplicationType == ApplicationType.HRMS_FLINS)
            //            {
            //                fullDisplayName = $"HRMS -FLINS Account";
            //            }

            //            if (endp.ApplicationType == ApplicationType.FLINS_BI)
            //            {
            //                fullDisplayName = $"FLINS BI Account";
            //            }
            //        }
            //    }
            //    if (endpoint != null && endpoint.ApplicationType == ApplicationType.SRM)
            //    {
            //        if (applicationType == ApplicationType.SAP_SRP)
            //        {
            //            fullDisplayName = $"SRP Account";
            //        }

            //        if (applicationType == ApplicationType.SAP_SUS)
            //        {
            //            fullDisplayName = $"SUS Account";
            //        }

            //        if (applicationType == ApplicationType.FLINS_BI)
            //        {
            //            fullDisplayName = $"FLINS BI Account";
            //        }
            //    }
            //    var emailSubject = appDisplayName;
            //    if (!string.IsNullOrWhiteSpace(fullDisplayName))
            //    {
            //        emailSubject = $"{appDisplayName} - {fullDisplayName}";
            //    }
            //    else
            //    {
            //        fullDisplayName = appDisplayName;
            //    }
            //    if (accessRequest.RequestSource == AccessRequestSource.AdhocRequest)
            //    {
            //        template = GetMailTemplate(Constants.CREATE_ACCOUNT_PASSWORD_NOTIFICATION_COMMON_TEMPLATE);

            //        subject = new StringBuilder(template.MailSubject);

            //        subject.Replace("@@APPLICATIONNAME@@", emailSubject);
            //        subject.Replace("@@REQUESTTYPE@@", accessRequest.AdHocAccessRequest.AdhocActionType.ToString());
            //        subject.Replace("@@REQUESTID@@", accessRequest.Id.ToString());
            //        subject.Replace("@@USERDISPLAYNAME@@", user.DisplayName);
            //        if (!string.IsNullOrEmpty(user.EmployeeId))
            //            subject.Replace("@@EMPLOYEEID@@", Convert.ToString(user.EmployeeId));

            //        subject.Replace("@@USERNAME@@", _userInfo.AccountName);
            //    }
            //    else
            //    {

            //        template = GetMailTemplate(Constants.OnboardingUserEmailPassword);

            //        subject = new StringBuilder(template.MailSubject);

            //        subject.Replace("@@APPLICATIONNAME@@", emailSubject);
            //        if (!string.IsNullOrEmpty(user.EmployeeId))
            //            subject.Replace("@@EMPLOYEEID@@", Convert.ToString(user.EmployeeId));

            //        subject.Replace("@@USERNAME@@", _userInfo.AccountName);
            //        fullDisplayName = string.Empty;
            //    }
            //    //template.MailContent.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
            //    string body = GetNewAccountTemplate(template.MailContent, accessRequest, _userInfo, user, applicationType, appDisplayName, fullDisplayName, endpoint, passwordForNPAC, isForSAPSSO);
            //    //body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
            //    emailInfo.Body = body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
            //    emailInfo.Subject = subject.ToString();
            //    emailInfo.From = !string.IsNullOrWhiteSpace(template.MailFrom) ? template.MailFrom : MasterConfigHelper.AccountCreatedNotificationCC;

            //    if (accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.IsSelfRequest == false)
            //    {
            //        emailInfo.To = $"{user.EmailId},{template.MailTo}";
            //        SendEmailToRequestBy(user, accessRequest, _userInfo.AccountName);
            //    }
            //    else
            //    {
            //        emailInfo.To = $"{user.EmailId},{template.MailTo}";
            //    }
            //    emailInfo.Cc = $"{MasterConfigHelper.AccountCreatedNotificationCC},{template.MailCC}";
            //    emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
            //    emailInfo.CreatedBy = "System";
            //    emailInfo.CreatedDate = DateTime.Now;
            //    emailInfo.CreatedDateTime = DateTime.Now;
            //    EmailHandler.Send(emailInfo);
            //}
            //catch (Exception ex)
            //{
            //    DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailForAccountProvisioning : AccessRequest Id - {accessRequest.Id}", ex, null, string.Empty);
            //    return false;
            //}

            return true;
        }
        static void SendEmailToRequestBy(User user, AccessRequest accessRequest, string userLogonName)
        {
            try
            {
                if (accessRequest.AdHocAccessRequest == null) return;

                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate(Constants.NewAccountSendEmailToRequestBy);
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);

                AdHocAccessRequest adHocAccessRequest = AdHocAccessRequest.GetById(accessRequest.AdHocAccessRequestId.Value, false);

                var appDisplayName = ApplicationName(accessRequest.EndpointSystemId.Value);

                subject.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                subject.Replace("@@applicationName@@", appDisplayName);
                subject.Replace("@@USER@@", user.DisplayName);
                subject.Replace("@@EMPLOYEEID@@", user.EmployeeId.ToString());
                subject.Replace("@@REQUESTTYPE@@", adHocAccessRequest.AdhocActionType.ToString());
                // Body
                body.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                body.Replace("@@applicationName@@", appDisplayName);
                body.Replace("@@REQUESTTYPE@@", adHocAccessRequest.AdhocActionType.ToString());
                body.Replace("@@USER@@", user.DisplayName);
                body.Replace("@@EMPLOYEEID@@", user.EmployeeId);
                body.Replace("@@userLogonName@@", userLogonName);
                body.Replace("@@RequestBy@@", GetUser(adHocAccessRequest.RequesterUserId).DisplayName);
                body.Replace("@@AccountType@@", accessRequest.AccountType.GetDescription());
                body.Replace("@@EmployeeType@@", user.EmployeeType.GetDescription());
                body.Replace("@@Comments@@", Convert.ToString(accessRequest.Comments ?? ""));
                if(accessRequest.AdHocAccessRequest != null)
                {
                    body.Replace("@@accessName@@", accessRequest.AdHocAccessRequest.Entitlement ?? "");
                }
                else
                {
                    body.Replace("@@accessName@@", "");
                }
                
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();

                User userRequester = User.GetById(adHocAccessRequest.RequesterUserId);
                emailInfo.To = userRequester.EmailId;

                emailInfo.Cc = MasterConfigHelper.AccountCreatedNotificationTO;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailToRequestBy  RequestBy: AccessRequest Id - {accessRequest.Id}", ex, null, string.Empty);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="accessRequest"></param>
        /// <param name="_userInfo"></param>
        /// <param name="user"></param>
        /// <param name="applicationType"></param>
        /// <param name="appDisplayName"></param>
        /// <param name="fullDisplayname"></param>
        /// <param name="endpoint"></param>
        /// <param name="passwordForNpac"></param>
        /// <param name="isForSAPSSO">Will be used for SSO enabled SAP system only</param>
        /// <returns></returns>
        static string GetNewAccountTemplate(string template, AccessRequest accessRequest, IEndpointUserInfo _userInfo, User user, ApplicationType applicationType, string appDisplayName, string fullDisplayname, EndPoint endpoint,string passwordForNpac = null, bool isForSAPSSO = false )
        {
            StringBuilder body = new StringBuilder(template);
            var passwordMsg = MasterConfigHelper.PasswordMessage;
            if(passwordMsg != null && !passwordMsg.Contains("{0}"))
            {
                passwordMsg= "Please use your {0} password to login and ignore the change password instructions below.";
            }
            //var appDisplayName = ApplicationName(accessRequest.EndpointSystemId.Value);

            body.Replace("@@USERDISPLAYNAME@@", user.DisplayName);

            body.Replace("@@COMPANY@@", !string.IsNullOrWhiteSpace(user.OrgCompany) ? user.OrgCompany : user.CompanyName);
            body.Replace("@@DEPT@@", !string.IsNullOrWhiteSpace(user.OrgDepartment) ? user.OrgDepartment : user.DepartmentName);
            body.Replace("@@DIVISION@@", string.Format("/{0}", !string.IsNullOrWhiteSpace(user.OrgDivision) ? user.OrgDivision : user.DivisionName));
            body.Replace("@@BRANCH@@", !string.IsNullOrWhiteSpace(user.OrgBranch) ? user.OrgBranch : string.Empty);
            body.Replace("@@SECTION@@", string.Format("/{0}", !string.IsNullOrWhiteSpace(user.OrgSection) ? user.OrgSection : user.SectionName));
            body.Replace("@@UNIT@@", string.Format("/{0}", !string.IsNullOrWhiteSpace(user.OrgUnit) ? user.OrgUnit : user.Reserved8 ?? string.Empty));
            body.Replace("@@EMPLOYEEID@@", !string.IsNullOrEmpty(user.EmployeeId) ? user.EmployeeId : "");
            body.Replace("@@EMPLOYEETYPE@@", user.EmployeeType.ToString());
            body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

            body.Replace("@@APPLICATIONNAMEFULL@@", fullDisplayname);

            body.Replace("@@APPLICATIONNAME@@", appDisplayName);

            body.Replace("@@USERNAME@@", _userInfo.AccountName);

            if (applicationType == ApplicationType.SSIDS_SMS && accessRequest.AdHocAccessRequest?.AdhocActionType == AdHocActionType.NewAccount)
            {
                body.Replace("Password : @@PASSWORD@@", "Handphone Activated");
            }
            else if (accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.AccountType == AccountType.Service)
            {
                body.Replace("@@PASSWORD@@", string.Empty);
            }
            else if (applicationType == ApplicationType.NPAC)
            {
                if(user != null && user.EmployeeType != EmployeeType.Contractor)
                {
                    body.Replace("@@PASSWORD@@", string.Format(passwordMsg, "AD/Windows"));
                }
                else
                {
                    body.Replace("@@PASSWORD@@", passwordForNpac);
                }
            }
            else
            {
                if (accessRequest.ApplicationType == ApplicationType.GEMS)
                {
                    body.Replace("@@PASSWORD@@", string.Format(passwordMsg, "AD/Windows"));
                }
                if (accessRequest.ApplicationType == ApplicationType.InhouseSystems && endpoint != null && endpoint.CanAutomate)
                {
                    var matrixData = DomainModel.InhousePermission.GetInhousePermissionGridData(endpoint.ApplicationCode);
                    if (matrixData != null && matrixData.Any(x => !string.IsNullOrWhiteSpace(x.ADGroups)))
                    {
                        body.Replace("@@PASSWORD@@", string.Format(passwordMsg, "AD/Windows"));
                    }
                    else if (matrixData != null &&
                            (matrixData.Any(x => !string.IsNullOrWhiteSpace(x.CEGroups)) || matrixData.Any(x => !string.IsNullOrWhiteSpace(x.CERoles)) ||
                            matrixData.Any(x => !string.IsNullOrWhiteSpace(x.EPGroups)) || matrixData.Any(x => !string.IsNullOrWhiteSpace(x.EPRoles))))
                    {
                        body.Replace("@@PASSWORD@@", string.Format(passwordMsg, "FLINS"));
                    }
                    else
                    {
                        body.Replace("@@PASSWORD@@", string.Empty);
                    }
                }
                
                if (accessRequest.ApplicationType != ApplicationType.OneCard && accessRequest.ApplicationType != ApplicationType.IDMS)
                {
                    if (applicationType == ApplicationType.MicrosoftTeams)
                    {
                        body.Replace("@@PASSWORD@@", string.Format(passwordMsg, "AD/Windows"));
                    }

                    if (applicationType == ApplicationType.EAM && string.IsNullOrWhiteSpace(_userInfo.AccountPassword))
                    {
                        if (isForSAPSSO)
                        {
                            body.Replace("@@PASSWORD@@", string.Format(passwordMsg, "AD/Windows"));
                        }
                        else
                        {
                            body.Replace("@@PASSWORD@@", string.Format(passwordMsg, "FLINS"));
                        }
                    }

                    else
                    {
                        if(isForSAPSSO)
                        {
                            body.Replace("@@PASSWORD@@", string.Format(passwordMsg, "AD/Windows"));
                        }
                        else
                        {
                            body.Replace("@@PASSWORD@@", _userInfo.AccountPassword);
                        }
                    }

                }
                else if (accessRequest.ApplicationType == ApplicationType.IDMS)
                {
                    body.Replace("@@PASSWORD@@", string.Format(passwordMsg, "AD/Windows"));
                }
                else
                {
                    body.Replace("@@PASSWORD@@", _userInfo.AccountPassword + "#");
                }
            }

            if (accessRequest.RequestSource == AccessRequestSource.AdhocRequest)
                body.Replace("@@ACCOUNTTYPE@@", accessRequest.AdHocAccessRequest.AccountType.ToString());
            else
                body.Replace("@@ACCOUNTTYPE@@", "User");

            body.Replace("@@RES1@@", string.Empty);
            body.Replace("@@RES2@@", string.Empty);

            return body.ToString();
        }

        public static bool SendEmailAfterOffBoardingTheUser(User usr)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate(Constants.AdhocUserOffBoardingMailTemplate);
                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@userDisplayName@@", usr.DisplayName);
                subject.Replace("@@pernrNum@@", usr.EmployeeId);

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@terminationDate@@", DateTime.Now.MailDateFormat());
                body.Replace("@@userDisplayName@@", usr.DisplayName);
                body.Replace("@@coyCode@@", usr.CompanyCode);
                body.Replace("@@coyText@@", usr.CompanyName);
                body.Replace("@@deptCode@@", usr.DepartmentCode);
                body.Replace("@@deptText@@", usr.DepartmentName);
                body.Replace("@@divCode@@", usr.DivisionCode);
                body.Replace("@@divText@@", usr.DivisionName);
                body.Replace("@@orgCode@@", usr.SectionCode);
                body.Replace("@@orgText@@", usr.SectionName);
                body.Replace("@@cctrCode@@", usr.CostCenter);
                body.Replace("@@pernrNum@@", usr.EmployeeId);
                body.Replace("@@cctrText@@", usr.CostCenterText);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

                body.Replace("@@coyCode@@", usr.CompanyCode);
                body.Replace("@@coyText@@", usr.CompanyName);
                body.Replace("@@deptCode@@", usr.DepartmentCode);
                body.Replace("@@deptText@@", usr.DepartmentName);
                body.Replace("@@branchCode@@", usr.BranchCode);
                body.Replace("@@branchText@@", usr.OrgBranch);

                body.Replace("@@sectionCode@@", usr.SectionCode);
                body.Replace("@@sectionText@@", usr.OrgBranch);

                body.Replace("@@unitCode@@", usr.UnitCode);
                body.Replace("@@unitText@@", usr.OrgUnit);

                //body.Replace("@@divCode@@", user.DivisionCode);
                //body.Replace("@@divText@@", user.DivisionName);
                body.Replace("@@orgCode@@", usr.OrganisationCode);
                body.Replace("@@orgText@@", getOrganisationText(usr.OrganisationCode));

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = MasterConfigHelper.AccountCreatedNotificationTO;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);

                return false;
            }
        }

        static string getOrganisationText(string orgCode)
        {
            if (string.IsNullOrWhiteSpace(orgCode)) return string.Empty;

            var org = NOrganisation.GetByOrgCode(orgCode);

            if (org == null) return string.Empty;
            else
                return org.Description;
        }
        public static bool SendEmailAfterOffBoardingTheApprover(User usr, IList<string> approverConfiguration, IList<Group> ApproverGroups)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate(Constants.AdhocApproverOffBoardingMailTemplate);
                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@userDisplayName@@", usr.DisplayName);
                subject.Replace("@@pernrNum@@", usr.EmployeeId);

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@terminationDate@@", DateTime.Now.MailDateFormat());
                body.Replace("@@userDisplayName@@", usr.DisplayName);
                body.Replace("@@pernrNum@@", usr.EmployeeId);

                string ApplicationString = "<br>";
                if (approverConfiguration != null)
                {
                    foreach (string aConfig in approverConfiguration)
                    {
                        ApplicationString += aConfig + "<br>";
                    }
                }
                ApplicationString += "<br>";
                body.Replace("@@applicationstring@@", ApplicationString);

                string GroupString = "<br>";
                if (ApproverGroups != null)
                {
                    foreach (Group group in ApproverGroups)
                    {
                        GroupString += group.GroupName + "<br>";
                    }
                }
                body.Replace("@@GROUPNAME@@", GroupString);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = MasterConfigHelper.AccountCreatedNotificationTO;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);

                return false;
            }
        }
        public static bool SendEmailAfterExecutingOUChangeRequest(User usr)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                if (usr == null)
                {
                    DILog.EmailLogger.WriteLog(LogLevelType.Warning, $"SendEmailAfterExecutingOUChangeRequest(User usr) - user Object is NULL", null);
                    return true;
                }
                MailTemplate template = GetMailTemplate(Constants.OUChangeRequestMailTemplate);
                if (template != null)
                {

                    StringBuilder subject = new StringBuilder(template.MailSubject);
                    subject.Replace("@@userDisplayName@@", usr.DisplayName);
                    subject.Replace("@@pernrNum@@", usr.EmployeeId);

                    StringBuilder body = new StringBuilder(template.MailContent);
                    body.Replace("@@userDisplayName@@", usr.DisplayName);
                    body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                    body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

                    emailInfo.Body = body.ToString();
                    emailInfo.Subject = subject.ToString();
                    emailInfo.To = usr.EmailId;
                    emailInfo.Cc = MasterConfigHelper.AccountCreatedNotificationTO;
                    emailInfo.From = template.MailFrom;
                    emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                    emailInfo.CreatedBy = "System";
                    emailInfo.CreatedDate = DateTime.Now;
                    emailInfo.CreatedDateTime = DateTime.Now;
                    EmailHandler.Send(emailInfo);
                }
                else
                {
                    DILog.EmailLogger.WriteLog(LogLevelType.Warning, $"{Constants.OUChangeRequestMailTemplate} - not found", null);
                }
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);

                return false;
            }
        }
        public static bool SendEmailAfterAdhocUserRemoval(User user, EndPoint endPoint, AccessRequest accessRequest, string accountName)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate(Constants.AdhocUserRemovalMailTemplate);
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);
                AdHocAccessRequest adHocAccessRequest = new AdHocAccessRequest();

                if (accessRequest.AdHocAccessRequestId.HasValue)
                    adHocAccessRequest = AdHocAccessRequest.GetById(accessRequest.AdHocAccessRequestId.Value, false);

                var appDisplayName = ApplicationName(accessRequest.EndpointSystemId.Value);
                var requestType = $"{(accessRequest.AttestationCampaignId.HasValue ? "Attestation Remediation - " : "")}{adHocAccessRequest.AdhocActionType.ToString()}";

                subject.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                subject.Replace("@@applicationName@@", appDisplayName);
                subject.Replace("@@USER@@", user.DisplayName);
                subject.Replace("@@EMPLOYEEID@@", user.EmployeeId.ToString());

                // Body
                body.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                body.Replace("@@applicationName@@", appDisplayName);
                body.Replace("@@REQUESTTYPE@@", requestType);
                body.Replace("@@USER@@", user.DisplayName);
                body.Replace("@@EMPLOYEEID@@", user.EmployeeId.ToString());
                body.Replace("@@userLogonName@@", accountName);

                body.Replace("@@RequestBy@@", GetUser(adHocAccessRequest.RequesterUserId).DisplayName);
                body.Replace("@@AccountType@@", accessRequest.AccountType.GetDescription());
                body.Replace("@@EmployeeType@@", user.EmployeeType.GetDescription());
                body.Replace("@@Comments@@", accessRequest.Comments?.ToString());

                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();

                if (accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.IsSelfRequest == false)
                {
                    User userRequester = User.GetById(accessRequest.AdHocAccessRequest.RequesterUserId);
                    emailInfo.To = string.Format("{0},{1}", user.EmailId, userRequester.EmailId);
                }
                else
                {
                    emailInfo.To = user.EmailId;
                }

                emailInfo.Cc = MasterConfigHelper.AccountCreatedNotificationTO;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);
                return false;
            }

            #region old code
            //try
            //{
            //    EmailInfo emailInfo = new EmailInfo();

            //    MailTemplate template = GetMailTemplate(Constants.AdhocUserRemovalMailTemplate);
            //    StringBuilder body = new StringBuilder(template.MailContent);
            //    StringBuilder subject = new StringBuilder(template.MailSubject);
            //    AdHocAccessRequest adHocAccessRequest = new AdHocAccessRequest();

            //    if (accessRequest.AdHocAccessRequestId.HasValue)
            //        adHocAccessRequest = AdHocAccessRequest.GetById(accessRequest.AdHocAccessRequestId.Value, false);


            //    //Subject 
            //    subject.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
            //    subject.Replace("@@SYSTEM@@", adHocAccessRequest.EndpointSystemType.ToString());
            //    subject.Replace("@@USER@@", user.DisplayName);
            //    subject.Replace("@@USERID@@", user.EmployeeId.ToString());

            //    subject.Replace("@@applicationName@@", Convert.ToString(adHocAccessRequest.ApplicationType));
            //    subject.Replace("@@REQUESTTYPE@@", adHocAccessRequest.AdhocActionType.ToString());
            //    subject.Replace("@@REQUESTID@@", accessRequest.Id.ToString());
            //    subject.Replace("@@USER@@", user.DisplayName);
            //    subject.Replace("@@EMPLOYEEID@@", user.EmployeeId.ToString());
            //    subject.Replace("@@userLogonName@@", accountName);

            //    // Body
            //    body.Replace("@@USER@@", user.DisplayName);
            //    body.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
            //    body.Replace("@@REQUESTID@@", accessRequest.Id.ToString());
            //    body.Replace("@@applicationName@@", Convert.ToString(adHocAccessRequest.ApplicationType));
            //    body.Replace("@@REQUESTTYPE@@", adHocAccessRequest.AdhocActionType.ToString());
            //    body.Replace("@@EMPLOYEEID@@", user.EmployeeId.ToString());
            //    body.Replace("@@userLogonName@@", accountName);

            //    body.Replace("@@DATE@@", DateTime.Now.ToString());
            //    body.Replace("@@REQUESTNAME@@", adHocAccessRequest.EndpointSystemType.ToString() + "-" + adHocAccessRequest.AdhocActionType.ToString());
            //    body.Replace("@@REQUESTFOR@@", user.DisplayName);
            //    body.Replace("@@REQUESTDATE@@", accessRequest.CreatedDate.ToString());
            //    body.Replace("@@ENDPOINT@@", endPoint.DisplayName);


            //    emailInfo.Body = body.ToString();
            //    emailInfo.Subject = subject.ToString();

            //    if (accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.IsSelfRequest == false)
            //    {
            //        User userRequester = User.GetById(accessRequest.AdHocAccessRequest.RequesteeUserId);
            //        emailInfo.To = string.Format("{0},{1}", user.EmailId, userRequester.EmailId);
            //    }
            //    else
            //    {
            //        emailInfo.To = user.EmailId;
            //    }

            //    emailInfo.Cc = MasterConfigHelper.AccountCreatedNotificationTO;
            //    emailInfo.From = template.MailFrom;
            //    emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
            //    emailInfo.CreatedBy = "System";
            //    emailInfo.CreatedDate = DateTime.Now;
            //    emailInfo.CreatedDateTime = DateTime.Now;
            //    EmailHandler.Send(emailInfo);
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    DILog.EmailLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);

            //    return false;
            //}
            #endregion
        }

        public static bool SendEmailAfterAdhocAmendmentAddExternalEmail(User user, AccessRequest accessRequest, string entitlement, string userLogonName)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate(Constants.AdhocAmendment);
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);
                AdHocAccessRequest adHocAccessRequest = new AdHocAccessRequest();

                if (accessRequest.AdHocAccessRequestId.HasValue)
                    adHocAccessRequest = AdHocAccessRequest.GetById(accessRequest.AdHocAccessRequestId.Value, false);

                if (entitlement == null)
                {
                    entitlement = "";
                }
                entitlement = entitlement.Replace(",", "<br/>");

                subject.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                subject.Replace("@@applicationName@@", "External Email");
                subject.Replace("@@userLogonName@@", userLogonName);
                subject.Replace("@@SYSTEM@@", adHocAccessRequest.EndpointSystemType.ToString());
                subject.Replace("@@REQUESTTYPE@@", adHocAccessRequest.AdhocActionType.ToString());
                subject.Replace("@@USER@@", user.DisplayName);
                subject.Replace("@@USERID@@", user.EmployeeId.ToString());

                // Body
                body.Replace("@@USER@@", user.DisplayName);
                body.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                body.Replace("@@applicationName@@", "External Email");
                body.Replace("@@REQUESTTYPE@@", adHocAccessRequest.AdhocActionType.ToString());
                body.Replace("@@EMPLOYEEID@@", user.EmployeeId.ToString());
                body.Replace("@@userLogonName@@", userLogonName);
                //string requestDetail = string.Format("{0} - {1}", UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.AdhocActionType), UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.AccountType));
                body.Replace("@@accessName@@", "External Email");

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();

                if (accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.IsSelfRequest == false)
                {
                    User userRequester = User.GetById(accessRequest.AdHocAccessRequest.RequesterUserId);
                    emailInfo.To = string.Format("{0},{1}", user.EmailId, userRequester.EmailId);
                }
                else
                {
                    emailInfo.To = user.EmailId;
                }

                emailInfo.Cc = MasterConfigHelper.AccountCreatedNotificationTO;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;

            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);

                return false;
            }
        }

        public static bool SendEmailAfterAdhocAmendmentRemoveExternalEmail(User user, AccessRequest accessRequest, string entitlement, string userLogonName)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate(Constants.AdhocAmendment);
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);
                AdHocAccessRequest adHocAccessRequest = new AdHocAccessRequest();

                if (accessRequest.AdHocAccessRequestId.HasValue)
                    adHocAccessRequest = AdHocAccessRequest.GetById(accessRequest.AdHocAccessRequestId.Value, false);

                if (entitlement == null)
                {
                    entitlement = "";
                }
                entitlement = entitlement.Replace(",", "<br/>");
                subject.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                subject.Replace("@@applicationName@@", "External Email");
                subject.Replace("@@userLogonName@@", userLogonName);
                subject.Replace("@@SYSTEM@@", adHocAccessRequest.EndpointSystemType.ToString());
                subject.Replace("@@REQUESTTYPE@@", adHocAccessRequest.AdhocActionType.ToString());
                subject.Replace("@@USER@@", user.DisplayName);
                subject.Replace("@@USERID@@", user.EmployeeId.ToString());

                // Body
                body.Replace("@@USER@@", user.DisplayName);
                body.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                body.Replace("@@applicationName@@", "External Email");
                body.Replace("@@REQUESTTYPE@@", adHocAccessRequest.AdhocActionType.ToString());
                body.Replace("@@EMPLOYEEID@@", user.EmployeeId.ToString());
                body.Replace("@@userLogonName@@", userLogonName);
                //string requestDetail = string.Format("{0} - {1}", UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.AdhocActionType), UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.AccountType));
                body.Replace("@@accessName@@", "External Email");


                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();

                if (accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.IsSelfRequest == false)
                {
                    User userRequester = User.GetById(accessRequest.AdHocAccessRequest.RequesterUserId);
                    emailInfo.To = string.Format("{0},{1}", user.EmailId, userRequester.EmailId);
                }
                else
                {
                    emailInfo.To = user.EmailId;
                }

                emailInfo.Cc = MasterConfigHelper.AccountCreatedNotificationTO;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString(); ;
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);

                return false;
            }
        }

        public static bool SendEmailAfterVPNCreateAccount(User user, AccessRequest accessRequest, string entitlement, string userLogonName)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate(Constants.AdhocAmendment);
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);
                AdHocAccessRequest adHocAccessRequest = new AdHocAccessRequest();

                if (accessRequest.AdHocAccessRequestId.HasValue)
                    adHocAccessRequest = AdHocAccessRequest.GetById(accessRequest.AdHocAccessRequestId.Value, false);

                if (entitlement == null)
                {
                    entitlement = "";
                }
                var requestBy = GetUser(adHocAccessRequest.RequesterUserId);
                entitlement = entitlement.Replace(",", "<br/>");
                var appDisplayName = ApplicationName(accessRequest.EndpointSystemId.Value);



                //subject.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                //subject.Replace("@@applicationName@@", appDisplayName);
                //subject.Replace("@@userLogonName@@", userLogonName);
                //subject.Replace("@@SYSTEM@@", adHocAccessRequest.EndpointSystemType.ToString());
                //subject.Replace("@@REQUESTTYPE@@", adHocAccessRequest.AdhocActionType.ToString());
                //subject.Replace("@@USER@@", user.DisplayName);
                //subject.Replace("@@USERID@@", user.EmployeeId.ToString());

                subject.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                subject.Replace("@@applicationName@@", appDisplayName);
                subject.Replace("@@userLogonName@@", userLogonName);
                subject.Replace("@@SYSTEM@@", adHocAccessRequest.EndpointSystemType.ToString());
                subject.Replace("@@REQUESTTYPE@@", adHocAccessRequest.AdhocActionType.ToString());
                subject.Replace("@@USER@@", user.DisplayName);
                subject.Replace("@@USERID@@", user.EmployeeId.ToString());
                subject.Replace("@@EMPLOYEEID@@", user.EmployeeId.ToString());

                // Body
                //body.Replace("@@USER@@", user.DisplayName);
                //body.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                //body.Replace("@@applicationName@@", appDisplayName);
                //body.Replace("@@REQUESTTYPE@@", adHocAccessRequest.AdhocActionType.ToString());
                //body.Replace("@@EMPLOYEEID@@", user.EmployeeId.ToString());
                //body.Replace("@@userLogonName@@", userLogonName);
                ////string requestDetail = string.Format("{0} - {1}", UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.AdhocActionType), UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.AccountType));
                //body.Replace("@@accessName@@", entitlement);
                body.Replace("@@USER@@", user.DisplayName);
                body.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                body.Replace("@@applicationName@@", appDisplayName);
                body.Replace("@@REQUESTTYPE@@", adHocAccessRequest.AdhocActionType.ToString());
                body.Replace("@@EMPLOYEEID@@", user.EmployeeId.ToString());
                body.Replace("@@userLogonName@@", userLogonName);
                body.Replace("@@accessName@@", entitlement);

                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                body.Replace("@@regards@@", MasterConfigHelper.EmailRegards);
                body.Replace("@@RequestBy@@", requestBy?.DisplayName);
                body.Replace("@@AccountType@@", adHocAccessRequest.AccountType.ToString());
                body.Replace("@@ACCOUNTTYPE@@", adHocAccessRequest.AccountType.ToString());
                body.Replace("@@EmployeeType@@", user.EmployeeType.ToString());
                body.Replace("@@EMPLOYEETYPE@@", user.EmployeeType.ToString());
                body.Replace("@@REQUESTBY@@", requestBy.DisplayName);
                body.Replace("@@Comments@@", accessRequest.Comments?.ToString());

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();

                if (accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.IsSelfRequest == false)
                {
                    User userRequester = User.GetById(accessRequest.AdHocAccessRequest.RequesterUserId);
                    emailInfo.To = string.Format("{0},{1}", user.EmailId, userRequester.EmailId);
                }
                else
                {
                    emailInfo.To = user.EmailId;
                }

                emailInfo.Cc = MasterConfigHelper.AccountCreatedNotificationTO;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);

                return false;
            }
        }
        static User GetUser(long userId)
        {
            return User.GetById(userId);
        }

        public static bool SendEmailAfterAdhocAmendmentAdd(User user, AccessRequest accessRequest, string entitlement, string userLogonName)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate(Constants.AdhocAmendment);
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);
                AdHocAccessRequest adHocAccessRequest = new AdHocAccessRequest();

                DILog.EmailLogger.WriteLog(LogLevelType.DebugStep, "SendEmailAfterAdhocAmendmentAdd 1");

                if (accessRequest.AdHocAccessRequestId.HasValue)
                    adHocAccessRequest = AdHocAccessRequest.GetById(accessRequest.AdHocAccessRequestId.Value, false);

                if (entitlement == null)
                {
                    entitlement = "";
                }

                DILog.EmailLogger.WriteLog(LogLevelType.DebugStep, "SendEmailAfterAdhocAmendmentAdd 2");
                if (accessRequest.ApplicationType != ApplicationType.SRM)
                    entitlement = entitlement.Replace(",", "<br/>");

                var appDisplayName = ApplicationName(accessRequest.EndpointSystemId.Value);

                DILog.EmailLogger.WriteLog(LogLevelType.DebugStep, "SendEmailAfterAdhocAmendmentAdd 3");
                subject.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                subject.Replace("@@applicationName@@", appDisplayName);
                subject.Replace("@@USER@@", user.DisplayName);
                subject.Replace("@@EMPLOYEEID@@", user.EmployeeId.ToString());
                DILog.EmailLogger.WriteLog(LogLevelType.DebugStep, "SendEmailAfterAdhocAmendmentAdd 4");
                subject.Replace("@@REQUESTTYPE@@", adHocAccessRequest.AdhocActionType.ToString());
                DILog.EmailLogger.WriteLog(LogLevelType.DebugStep, "SendEmailAfterAdhocAmendmentAdd 5");
                // Body
                body.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                body.Replace("@@applicationName@@", appDisplayName);
                body.Replace("@@REQUESTTYPE@@", adHocAccessRequest.AdhocActionType.ToString());
                body.Replace("@@USER@@", user.DisplayName);
                body.Replace("@@EMPLOYEEID@@", user.EmployeeId.ToString());
                body.Replace("@@userLogonName@@", userLogonName);
                DILog.EmailLogger.WriteLog(LogLevelType.DebugStep, "SendEmailAfterAdhocAmendmentAdd 6");
                body.Replace("@@RequestBy@@", GetUser(adHocAccessRequest.RequesterUserId).DisplayName);
                body.Replace("@@AccountType@@", accessRequest.AccountType.GetDescription());
                body.Replace("@@EmployeeType@@", user.EmployeeType.GetDescription());
                body.Replace("@@Comments@@", Convert.ToString(accessRequest.Comments ?? ""));

                body.Replace("@@accessName@@", entitlement);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                DILog.EmailLogger.WriteLog(LogLevelType.DebugStep, "SendEmailAfterAdhocAmendmentAdd 7");
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();

                if (adHocAccessRequest != null && adHocAccessRequest.IsSelfRequest == false)
                {
                    User userRequester = User.GetById(adHocAccessRequest.RequesterUserId);
                    emailInfo.To = string.Format("{0},{1}", user.EmailId, userRequester.EmailId);
                }
                else
                {
                    emailInfo.To = user.EmailId;
                }


                DILog.EmailLogger.WriteLog(LogLevelType.DebugStep, "SendEmailAfterAdhocAmendmentAdd 8");
                emailInfo.Cc = MasterConfigHelper.AccountCreatedNotificationTO;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                DILog.EmailLogger.WriteLog(LogLevelType.DebugStep, "SendEmailAfterAdhocAmendmentAdd 9");
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);

                return false;
            }
        }

        public static bool SendEmailAfterAdhocAmendmentRemove(User user, AccessRequest accessRequest, string entitlement, string userLogonName)
        {

            try
            {

                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate(Constants.AdhocAmendment);
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);
                AdHocAccessRequest adHocAccessRequest = new AdHocAccessRequest();
                

                if (accessRequest.AdHocAccessRequestId.HasValue)
                    adHocAccessRequest = AdHocAccessRequest.GetById(accessRequest.AdHocAccessRequestId.Value, false);

                var requestType = $"{(accessRequest.AttestationCampaignId.HasValue ? "Attestation Remediation - " : "")}{adHocAccessRequest.AdhocActionType.ToString()}";

                if (entitlement == null)
                {
                    entitlement = "";
                }
                var requestBy = GetUser(adHocAccessRequest.RequesterUserId);
                entitlement = entitlement.Replace(",", "<br/>");
                var appDisplayName = ApplicationName(accessRequest.EndpointSystemId.Value);

                subject.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                subject.Replace("@@applicationName@@", appDisplayName);
                subject.Replace("@@userLogonName@@", userLogonName);
                subject.Replace("@@SYSTEM@@", adHocAccessRequest.EndpointSystemType.ToString());
                subject.Replace("@@REQUESTTYPE@@", requestType);
                subject.Replace("@@USER@@", user.DisplayName);
                subject.Replace("@@USERID@@", user.EmployeeId.ToString());
                subject.Replace("@@EMPLOYEEID@@", user.EmployeeId.ToString());
                // Body
                body.Replace("@@USER@@", user.DisplayName);
                body.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                body.Replace("@@applicationName@@", appDisplayName);
                body.Replace("@@REQUESTTYPE@@", requestType);
                body.Replace("@@EMPLOYEEID@@", user.EmployeeId.ToString());
                body.Replace("@@userLogonName@@", userLogonName);
                body.Replace("@@accessName@@", entitlement);

                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                body.Replace("@@regards@@", MasterConfigHelper.EmailRegards);
                body.Replace("@@RequestBy@@", requestBy?.DisplayName);
                body.Replace("@@AccountType@@", adHocAccessRequest.AccountType.ToString());
                body.Replace("@@ACCOUNTTYPE@@", adHocAccessRequest.AccountType.ToString());
                body.Replace("@@EmployeeType@@", user.EmployeeType.ToString());
                body.Replace("@@EMPLOYEETYPE@@", user.EmployeeType.ToString());
                body.Replace("@@REQUESTBY@@", requestBy.DisplayName);
                body.Replace("@@Comments@@", accessRequest.Comments?.ToString());

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();

                if (accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.IsSelfRequest == false)
                {
                    User userRequester = User.GetById(accessRequest.AdHocAccessRequest.RequesterUserId);
                    emailInfo.To = string.Format("{0},{1}", user.EmailId, userRequester.EmailId);
                }
                else
                {
                    emailInfo.To = user.EmailId;
                }

                emailInfo.Cc = MasterConfigHelper.AccountCreatedNotificationTO;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);

                return false;
            }
        }

        public static bool SendEmailForDeletingNonConnectedAccount(User usr, List<string> EndpointNames, EmployeeEvents eventType)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();
                string eventInfo = string.Empty;
                string subject = string.Empty;

                MailTemplate template = GetMailTemplate(Constants.NotifyDeleteNonConnectAcc);
                subject.Replace("@@userDisplayName@@", usr.DisplayName);
                subject.Replace("@@pernrNum@@", usr.EmployeeId);

                if (eventType == EmployeeEvents.OUChange)
                {
                    eventInfo = "The following user has transferred to new OU. Please deactivate the user's accounts in the systems listed below.";

                    subject = "IDMS: OU Change Request - @@userDisplayName@@  (@@pernrNum@@)";
                    subject = subject.Replace("@@userDisplayName@@", usr.DisplayName);
                    subject = subject.Replace("@@pernrNum@@", usr.EmployeeId);
                }
                else if (eventType == EmployeeEvents.Resignation)
                {
                    eventInfo = "The following user has left service on @@terminationDate@@. Please deactivate the user's accounts in the systems listed below.";
                    eventInfo = eventInfo.Replace("@@terminationDate@@", DateTime.Now.ToString(DATETIMEFORMAT));

                    subject = "IDMS: Offboarding Request - @@userDisplayName@@  (@@pernrNum@@)";
                    subject = subject.Replace("@@userDisplayName@@", usr.DisplayName);
                    subject = subject.Replace("@@pernrNum@@", usr.EmployeeId);
                }

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@eventinfo@@", eventInfo);
                body.Replace("@@userDisplayName@@", usr.DisplayName);
                body.Replace("@@coyCode@@", usr.CompanyCode);
                body.Replace("@@coyText@@", usr.CompanyName);
                body.Replace("@@deptCode@@", usr.DepartmentCode);
                body.Replace("@@deptText@@", usr.DepartmentName);
                body.Replace("@@divCode@@", usr.DivisionCode);
                body.Replace("@@divText@@", usr.DivisionName);
                body.Replace("@@orgCode@@", usr.SectionCode);
                body.Replace("@@orgText@@", usr.SectionName);
                body.Replace("@@cctrCode@@", usr.CostCenter);
                body.Replace("@@pernrNum@@", usr.EmployeeId);
                body.Replace("@@cctrText@@", usr.CostCenterText);

                body.Replace("@@coyCode@@", usr.CompanyCode);
                body.Replace("@@coyText@@", usr.CompanyName);
                body.Replace("@@deptCode@@", usr.DepartmentCode);
                body.Replace("@@deptText@@", usr.DepartmentName);
                body.Replace("@@branchcode@@", usr.BranchCode);
                body.Replace("@@branchtext@@", usr.OrgBranch);

                body.Replace("@@sectioncode@@", usr.SectionCode);
                body.Replace("@@sectiontext@@", usr.OrgBranch);

                body.Replace("@@unitcode@@", usr.UnitCode);
                body.Replace("@@unittext@@", usr.OrgUnit);

                //body.Replace("@@divCode@@", user.DivisionCode);
                //body.Replace("@@divText@@", user.DivisionName);
                body.Replace("@@orgCode@@", usr.OrganisationCode);
                body.Replace("@@orgText@@", getOrganisationText(usr.OrganisationCode));

                string applicationstring = "";
                foreach (string application in EndpointNames)
                {
                    var applicationInfo = application.Split(',');
                    applicationstring = applicationstring + applicationInfo[0] + " - " + applicationInfo[1] + "<br />";
                }

                body.Replace("@@applicationstring@@", applicationstring);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject;
                emailInfo.To = MasterConfigHelper.AccountCreatedNotificationTO;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);

                return false;
            }
        }

        public static bool SendEmailToNotifyReferenceUserChange(User usr, List<string> nonEmployees, EmployeeEvents eventType)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();
                string eventInfo = "@@TEXT1@@ @@terminationDate@@";

                MailTemplate template = GetMailTemplate(Constants.NotifyReferenceUserChange);

                string roEmail = string.Empty;

                if (!string.IsNullOrEmpty(usr.ROEmployeeId))
                {
                    var roUser = User.GetByEmployeeId(usr.ROEmployeeId.Trim());

                    if (roUser != null)
                    {
                        roEmail = roUser.EmailId != null ? roUser.EmailId : string.Empty;
                    }
                }

                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@userDisplayName@@", usr.DisplayName);
                subject.Replace("@@pernrNum@@", usr.EmployeeId);
                subject.Replace("@@eventtype@@", UtilityMethods.GetEnumAttribute<Description>(eventType));

                if (eventType == EmployeeEvents.OUChange)
                {
                    eventInfo = eventInfo.Replace("@@TEXT1@@", "The following user has transferred to new OU.");
                    eventInfo = eventInfo.Replace("@@terminationDate@@", "");
                }
                else if (eventType == EmployeeEvents.Resignation)
                {
                    eventInfo = eventInfo.Replace("@@TEXT1@@", "The following user has left service on");
                    eventInfo = eventInfo.Replace("@@terminationDate@@", DateTime.Now.ToString(DATETIMEFORMAT));
                }


                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@eventinfo@@", eventInfo);
                body.Replace("@@EMPTYPE@@", usr.EmployeeType.ToString());
                body.Replace("@@userDisplayName@@", usr.DisplayName);
                body.Replace("@@coyCode@@", usr.CompanyCode);
                body.Replace("@@coyText@@", usr.CompanyName);
                body.Replace("@@deptCode@@", usr.DepartmentCode);
                body.Replace("@@deptText@@", usr.DepartmentName);
                body.Replace("@@divCode@@", usr.DivisionCode);
                body.Replace("@@divText@@", usr.DivisionName);
                body.Replace("@@orgCode@@", usr.SectionCode);
                body.Replace("@@orgText@@", usr.SectionName);
                body.Replace("@@cctrCode@@", usr.CostCenter);
                body.Replace("@@pernrNum@@", usr.EmployeeId);
                body.Replace("@@cctrText@@", usr.CostCenterText);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

                body.Replace("@@coyCode@@", usr.CompanyCode);
                body.Replace("@@coyText@@", usr.CompanyName);
                body.Replace("@@deptCode@@", usr.DepartmentCode);
                body.Replace("@@deptText@@", usr.DepartmentName);
                body.Replace("@@branchCode@@", usr.BranchCode);
                body.Replace("@@branchText@@", usr.OrgBranch);

                body.Replace("@@sectionCode@@", usr.SectionCode);
                body.Replace("@@sectionText@@", usr.OrgBranch);

                body.Replace("@@unitCode@@", usr.UnitCode);
                body.Replace("@@unitText@@", usr.OrgUnit);

                //body.Replace("@@divCode@@", user.DivisionCode);
                //body.Replace("@@divText@@", user.DivisionName);
                body.Replace("@@orgCode@@", usr.OrganisationCode);
                body.Replace("@@orgText@@", getOrganisationText(usr.OrganisationCode));

                string applicationstring = "";
                foreach (string application in nonEmployees)
                {
                    applicationstring = applicationstring + application + "<br />";
                }

                body.Replace("@@applicationstring@@", applicationstring);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = $"{MasterConfigHelper.AccountCreatedNotificationTO}";
                if (!string.IsNullOrWhiteSpace(roEmail))
                {
                    emailInfo.To = $"{emailInfo.To},{roEmail}";
                }
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);

                return false;
            }
        }

        public static bool SendEmailAfterAttestationRemediation(User user, AccessRequest accessRequest, string entitlement, string userLogonName)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate(Constants.RemediationComplete);
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);
                AdHocAccessRequest adHocAccessRequest = new AdHocAccessRequest();

                if (accessRequest.AdHocAccessRequestId.HasValue)
                    adHocAccessRequest = AdHocAccessRequest.GetById(accessRequest.AdHocAccessRequestId.Value, false);

                if (entitlement == null)
                {
                    entitlement = "";
                }
                entitlement = entitlement.Replace(",", "<br/>");
                var appDisplayName = ApplicationName(accessRequest.EndpointSystemId.Value);

                subject.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                subject.Replace("@@applicationName@@", appDisplayName);
                subject.Replace("@@userLogonName@@", userLogonName);
                subject.Replace("@@SYSTEM@@", adHocAccessRequest.EndpointSystemType.ToString());
                subject.Replace("@@REQUESTTYPE@@", adHocAccessRequest.AdhocActionType.ToString());
                subject.Replace("@@USER@@", user.DisplayName);
                subject.Replace("@@USERID@@", user.EmployeeId.ToString());

                // Body
                body.Replace("@@USER@@", user.DisplayName);
                body.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                body.Replace("@@applicationName@@", appDisplayName);
                body.Replace("@@REQUESTTYPE@@", adHocAccessRequest.AdhocActionType.ToString());
                body.Replace("@@EMPLOYEEID@@", user.EmployeeId.ToString());
                body.Replace("@@userLogonName@@", userLogonName);
                body.Replace("@@accessName@@", entitlement);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = user.EmailId;
                //emailInfo.Cc = MasterConfigHelper.AccountCreatedNotificationTO;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);

                return false;
            }
        }

        public static bool NotifyDeletedTrigger(User user, EmployeeEvents employeeEvent, EmployeeEventTrigger trigger)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate(Constants.TriggerDeletedNotificationTemplate);

                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@pernr@@", user.EmployeeId);
                subject.Replace("@@USER@@", user.DisplayName);
                //if (employeeEvent == EmployeeEvents.NewJoinee)
                //{
                //    subject.Replace("@@event@@", "On-Boarding");
                //}
                //else if (employeeEvent == EmployeeEvents.Resignation)
                //{
                //    subject.Replace("@@event@@", "Off-Boarding");
                //}
                //else if (employeeEvent == EmployeeEvents.OUChange)
                //{
                //    subject.Replace("@@event@@", "Off-Boarding");
                //}
                //else if (employeeEvent == EmployeeEvents.ReOrganisation)
                //{
                //    subject.Replace("@@event@@", "Off-Boarding");
                //}
                //else
                //{
                //subject.Replace("@@event@@", string.Empty);
                //}

                subject.Replace("@@event@@", employeeEvent.GetDescription());
                subject.Replace("@@effectivedate@@", trigger.EffectiveDate.ToString("dd/MM/yyyy"));

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@USER@@", user.DisplayName);
                body.Replace("@@pernrNum@@", user.EmployeeId);
                body.Replace("@@coyCode@@", user.CompanyCode);
                body.Replace("@@coyText@@", user.CompanyName);
                body.Replace("@@deptCode@@", user.DepartmentCode);
                body.Replace("@@deptText@@", user.DepartmentName);
                body.Replace("@@divCode@@", user.DivisionCode);
                body.Replace("@@divText@@", user.DivisionName);
                body.Replace("@@orgCode@@", user.SectionCode);
                body.Replace("@@orgText@@", user.SectionName);
                body.Replace("@@cctrCode@@", user.CostCenter);
                body.Replace("@@cctrText@@", user.CostCenterText);
                body.Replace("@@EMPTYPE@@", user.EmployeeType.ToString());
                body.Replace("@@effectivedate@@", trigger.EffectiveDate.ToString(DATETIMEFORMAT));
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

                body.Replace("@@coyCode@@", user.CompanyCode);
                body.Replace("@@coyText@@", user.CompanyName);
                body.Replace("@@deptCode@@", user.DepartmentCode);
                body.Replace("@@deptText@@", user.DepartmentName);
                body.Replace("@@branchCode@@", user.BranchCode);
                body.Replace("@@branchText@@", user.OrgBranch);

                body.Replace("@@sectionCode@@", user.SectionCode);
                body.Replace("@@sectionText@@", user.OrgBranch);

                body.Replace("@@unitCode@@", user.UnitCode);
                body.Replace("@@unitText@@", user.OrgUnit);

                //body.Replace("@@divCode@@", user.DivisionCode);
                //body.Replace("@@divText@@", user.DivisionName);
                body.Replace("@@orgCode@@", user.OrganisationCode);
                body.Replace("@@orgText@@", getOrganisationText(user.OrganisationCode));

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = string.Join(",", MasterConfigHelper.AlertNotificationReceivers);
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"NotifyDeletedTrigger:{ex.Message}", ex);
                return false;
            }
        }
        public static bool AccessRequestFailureNotification(AccessRequest accessRequest, User user)
        {

            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate(Constants.AccessRequestFailureNotification);
                StringBuilder subject = new StringBuilder(template.MailSubject);
                StringBuilder body = new StringBuilder(template.MailContent);
                string applicationName = string.Empty;
                if (accessRequest.AdHocAccessRequestId.HasValue)
                {
                    applicationName = EndPoint.GetById(accessRequest.EndpointSystemId.Value)?.DisplayName;
                    subject.Replace("@@APPLICATION@@", applicationName);
                    subject.Replace("@@RequestType@@", Convert.ToString(accessRequest.AdHocAccessRequest.AdhocActionType));
                    if (accessRequest.AdHocAccessRequest != null)
                    {
                        body.Replace("@@accessName@@", accessRequest.AdHocAccessRequest.Entitlement ?? "");
                    }
                    else
                    {
                        body.Replace("@@accessName@@", string.Empty);
                    }
                }
                else
                {
                    body.Replace("@@accessName@@", string.Empty);
                    subject.Replace("@@RequestType@@", Convert.ToString(accessRequest.RequestSource));

                    if (accessRequest.EndpointSystemId.HasValue)
                    {
                        var endpoint = EndPoint.GetById(accessRequest.EndpointSystemId.Value);
                        applicationName = endpoint?.DisplayName;
                        subject.Replace("@@APPLICATION@@", applicationName);
                    }
                    else
                    {
                        subject.Replace("@@APPLICATION@@", string.Empty);
                    }
                }

                subject.Replace("@@RequestNum@@", Convert.ToString(accessRequest.Id));
                subject.Replace("@@pernr@@", user.EmployeeId);


                body.Replace("@@RequestNum@@", Convert.ToString(accessRequest.Id));
                if (accessRequest.AdHocAccessRequestId.HasValue)
                {
                    body.Replace("@@APPLICATION@@", applicationName);
                    body.Replace("@@RequestType@@", Convert.ToString(accessRequest.AdHocAccessRequest.AdhocActionType));
                }
                else
                {
                    body.Replace("@@RequestType@@", Convert.ToString(accessRequest.RequestSource));

                    if (accessRequest.EndpointSystemId.HasValue)
                    {
                        var endpoint = EndPoint.GetById(accessRequest.EndpointSystemId.Value);
                        applicationName = endpoint?.DisplayName;
                        body.Replace("@@APPLICATION@@", applicationName);
                    }
                    else
                    {
                        body.Replace("@@APPLICATION@@", string.Empty);
                    }
                }
                body.Replace("@@USERDISPLAYNAME@@", Convert.ToString(user.DisplayName));
                body.Replace("@@PERNRNO@@", Convert.ToString(user.EmployeeId));
                body.Replace("@@DATE@@", DateTime.Now.MailDateTimeFormat());

                var requester = User.GetById(Convert.ToInt64(accessRequest.RequesterId));
                body.Replace("@@RequestBy@@", requester?.DisplayName);
                body.Replace("@@AccountType@@", accessRequest.AccountType.GetDescription());
                body.Replace("@@EmployeeType@@", user?.EmployeeType.GetDescription());
                body.Replace("@@Comments@@", accessRequest.Comments?.ToString());

                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = string.Join(",", MasterConfigHelper.FailureNotificationReceivers);
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString(); ;
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error while sending email for Access request:{accessRequest?.Id} execution failed notification", ex);
                return false;
            }
        }
        static MailTemplate GetMailTemplate(string templateName)
        {
            return MailTemplate.GetByTemplateName(templateName);
        }
        static string ApplicationName(long endpointId)
        {
            var endpoint = EndPoint.GetById(endpointId);

            if (endpoint == null)
                return string.Empty;

            return endpoint.DisplayName;

        }

        static EndPoint ApplicationNameWithEndpoint(long endpointId)
        {
            var endpoint = EndPoint.GetById(endpointId);

            return endpoint;

        }
        public static void SendMailOnBulkRequestRejection(long taskId)
        {
            DomainModel.Task task = null;

            task = DomainModel.Task.GetBulkRequestTask(taskId);

            SendEmailForAccessRequestRejected(task);

            var accessRequestId = task.BulkRequestId.Value;
            var workflowId = task.WorkflowStateId;

            var tasks = DomainModel.Task.Get(t => t.BulkRequestId == accessRequestId && t.WorkflowStateId == workflowId, false, 0, 0);


            foreach (DomainModel.Task item in tasks)
            {
                if (!item.IsActive && !item.IsCompleted && !item.IsGroupTask && item.ActionUserId.HasValue)
                {
                    NotifyApproverAboutRequestRejectedBulkRequest(item.ActionUserId.Value, task.ActionUserId.Value, task.BulkRequest, task.Comments);
                }
                else if (!item.IsActive && !item.IsCompleted && item.IsGroupTask)
                {
                    try
                    {
                        var group = Group.GetByGroupName(item.GroupName);

                        bool isGroupTaskActioned = tasks.Any(tsk => tsk.GroupName == item.GroupName && tsk.IsActive && tsk.IsCompleted);
                        if (!isGroupTaskActioned)
                        {
                            NotifyApproverAboutRequestRejectedBulkRequest(item.ActionUserId.Value, task.ActionUserId.Value, task.BulkRequest, task.Comments);
                        }

                        foreach (User usr in group.Users)
                        {
                            NotifyApproverAboutRequestRejectedBulkRequest(usr, task.ActionUserId.Value, task.BulkRequest, task.Comments);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while sending rejection message to Group.", ex);
                    }
                }
            }
        }


        public static void SendMailOnAccessRequestRejection(long taskId)
        {
            DomainModel.Task task = null;

            task = DomainModel.Task.GetProvisioningTask(taskId);

            SendEmailForAccessRequestRejected(task);
            try
            {
                var accessRequestId = task.AccessRequestId.Value;
                var workflowId = task.WorkflowStateId;
                var tasks = DomainModel.Task.Get(t => t.AccessRequestId == accessRequestId && t.WorkflowStateId == workflowId, false, 0, 0);

                foreach (DomainModel.Task item in tasks)
                {
                    if (!item.IsActive && !item.IsCompleted && !item.IsGroupTask && item.ActionUserId.HasValue)
                    {
                        NotifyApproverAboutRequestRejectedAccessRequest(item.ActionUserId.Value, task.ActionUserId.Value, task.AccessRequest);
                    }
                    else if (item.IsActive && !item.IsCompleted && item.IsGroupTask)
                    {
                        try
                        {
                            var group = Group.GetByGroupName(item.GroupName);

                            bool isGroupTaskActioned = tasks.Any(tsk => tsk.GroupName == item.GroupName && tsk.IsActive && tsk.IsCompleted);
                            if (!isGroupTaskActioned)
                            {
                                NotifyApproverAboutRequestRejectedAccessRequest(item.ActionUserId.Value, task.ActionUserId.Value, task.AccessRequest);
                            }

                            foreach (User usr in group.Users)
                            {
                                NotifyApproverAboutRequestRejectedAccessRequest(usr, task.ActionUserId.Value, task.AccessRequest);
                            }

                        }
                        catch (Exception ex)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while sending rejection message to Group.", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while sending rejection message to Group.", ex);
            }
        }
        public static void SendEmailForAccessRequestRejected(DomainModel.Task task)
        {
            try
            {
                if (task.EmployeeEventTriggerId.HasValue)
                {
                    var trigger = EmployeeEventTrigger.GetById(task.EmployeeEventTriggerId.Value);
                    if (trigger != null)
                    {
                        EmailInfo emailInfo = new EmailInfo();
                        MailTemplate template = GetMailTemplate("NotifyEmployeeEventTaskRejected");
                        var requester = User.GetByUserName(trigger.CreatedBy);
                        var RequesteeUser = User.GetById(trigger.UserId);
                        var rejectedBy = User.GetById(task.ActionUserId.Value);
                        StringBuilder subject = new StringBuilder(template.MailSubject);

                        string requestType = string.Empty;

                        if (trigger.EventType == EmployeeEvents.NonEmployeeCreation)
                        {
                            requestType = UtilityMethods.GetEnumAttribute<Description>(AccessRequestSource.EmployeeCreation);
                        }
                        else if (trigger.EventType == EmployeeEvents.NonEmployeeExtension)
                        {
                            requestType = UtilityMethods.GetEnumAttribute<Description>(AccessRequestSource.EmployeeExtension);
                        }

                        subject.Replace("@@requestType@@", requestType);
                        subject.Replace("@@REQUESTNUM@@", trigger.Id.ToString());
                        subject.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                        subject.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);

                        StringBuilder body = new StringBuilder(template.MailContent);
                        body.Replace("@@requestType@@", requestType);
                        body.Replace("@@REQUESTNUM@@", trigger.Id.ToString());
                        body.Replace("@@requesterDisplayName@@", requester.DisplayName);
                        body.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                        body.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);
                        body.Replace("@@REQUESTER@@", trigger.CreatedBy);
                        body.Replace("@@REJECTEDBY@@", rejectedBy.DisplayName);
                        body.Replace("@@REASON@@", task.Comments);
                        body.Replace("@@EMPTYPE@@", RequesteeUser.EmployeeType.ToString());
                        body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                        emailInfo.Body = body.ToString();
                        emailInfo.Subject = subject.ToString();
                        emailInfo.To = requester.EmailId;
                        emailInfo.From = template.MailFrom;
                        emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();

                        EmailHandler.Send(emailInfo);
                    }
                }
                else
                {
                    StringBuilder body = new StringBuilder();
                    StringBuilder mailSubject = new StringBuilder();
                    EmailInfo emailInfo = new EmailInfo();


                    string reqToMailid = string.Empty;
                    string reqByMailid = string.Empty;

                    if (task.BulkRequest != null)
                    {
                        MailTemplate mailTemplate = GetMailTemplate(Constants.BulkRequestRejected);
                        if (mailTemplate == null) return;

                        body = new StringBuilder(mailTemplate.MailContent);
                        mailSubject = new StringBuilder(mailTemplate.MailSubject);


                        var appDisplayName = ApplicationName(task.BulkRequest.EndpointSystemId);
                        mailSubject.Replace("@@ApplicationName@@", appDisplayName);
                        mailSubject.Replace("@@REQUESTTYPE@@", task.BulkRequest.AdhocActionType.ToString());
                        mailSubject.Replace("@@REQUESTID@@", task.BulkRequestId.ToString());

                        string displayName = string.Join(",", task.BulkRequest.BulkRequestDetails.Select(item => $"{item.Name}({item.EmployeeId})").ToList());
                        string employeeId = string.Join(",", task.BulkRequest.BulkRequestDetails.Select(item => item.EmployeeId).ToList());


                        User requester = User.GetById(task.BulkRequest.RequestedBy);
                        User approver = User.GetById(task.ActionUserId.Value);
                        body.Replace("@@USER@@", displayName);
                        body.Replace("@@APPROVER@@", approver.DisplayName);
                        body.Replace("@@DATE@@", DateTime.Now.MailDateTimeFormat());
                        body.Replace("@@REQUESTID@@", task.BulkRequest.Id.ToString());
                        body.Replace("@@ApplicationName@@", Convert.ToString(task.BulkRequest.ApplicationType));
                        body.Replace("@@REQUESTTYPE@@", task.BulkRequest.AdhocActionType.ToString());
                        body.Replace("@@approverComments@@", task.BulkRequest.Comments);
                        body.Replace("@@pernrNum@@", employeeId);

                        body.Replace("@@RequestBy@@", User.GetById(Convert.ToInt64(task.BulkRequest.RequestedBy))?.DisplayName);
                        body.Replace("@@AccountType@@", AccountType.User.ToString());
                        body.Replace("@@Comments@@", task.Comments.ToString());
                        body.Replace("@@EMPTYPE@@", task.BulkRequest.IsEmployee ? EmployeeType.Permanent.ToString() : "Non-Employee");
                        body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);


                        task.BulkRequest.BulkRequestDetails.ForEach(lq =>
                        {
                            reqToMailid = string.IsNullOrEmpty(reqToMailid) ? ((string.IsNullOrWhiteSpace(lq.EmailId)) ? string.Empty : Convert.ToString(lq.EmailId)) : reqToMailid + ", " + ((string.IsNullOrWhiteSpace(lq.EmailId)) ? string.Empty : Convert.ToString(lq.EmailId));
                        });

                        if (!task.BulkRequest.BulkRequestDetails.Select(lq => lq.UserId).Contains(requester.Id))
                        {
                            reqByMailid = (requester == null || string.IsNullOrWhiteSpace(requester.EmailId)) ? string.Empty : Convert.ToString(requester.EmailId);
                        }
                        emailInfo.From = mailTemplate.MailFrom;
                    }
                    else if (task.AccessRequest != null)
                    {
                        MailTemplate mailTemplate = GetMailTemplate(Constants.AccessRequestRejected);
                        if (mailTemplate == null) return;

                        body = new StringBuilder(mailTemplate.MailContent);
                        mailSubject = new StringBuilder(mailTemplate.MailSubject);
                        User requester = User.GetById(task.AccessRequest.AdHocAccessRequest.RequesterUserId);
                        User approver = User.GetById(task.ActionUserId.Value);

                        var usr = DomainModel.User.GetById(task.AccessRequest.AdHocAccessRequest.RequesteeUserId);

                        string displayName = usr.DisplayName;
                        string employeeId = usr.EmployeeId;

                        var appDisplayName = ApplicationName(task.AccessRequest.EndpointSystemId.Value);

                        mailSubject.Replace("@@ApplicationName@@", appDisplayName);
                        mailSubject.Replace("@@USER@@", displayName);
                        mailSubject.Replace("@@pernrNum@@", employeeId);
                        mailSubject.Replace("@@REQUESTID@@", task.AccessRequestId.ToString());


                        body.Replace("@@USER@@", displayName);
                        body.Replace("@@APPROVER@@", approver.DisplayName);
                        body.Replace("@@DATE@@", DateTime.Now.MailDateTimeFormat());
                        body.Replace("@@REQUESTID@@", task.AccessRequest.Id.ToString());
                        body.Replace("@@ApplicationName@@", Convert.ToString(task.AccessRequest.AdHocAccessRequest.ApplicationType));
                        body.Replace("@@REQUESTTYPE@@", task.AccessRequest.AdHocAccessRequest.AdhocActionType.ToString());
                        body.Replace("@@approverComments@@", task.AccessRequest.AdHocAccessRequest.Comments);
                        body.Replace("@@pernrNum@@", employeeId);

                        body.Replace("@@RequestBy@@", requester?.DisplayName);
                        body.Replace("@@AccountType@@", task.AccessRequest.AccountType.GetDescription());
                        body.Replace("@@EmployeeType@@", usr?.EmployeeType.GetDescription());
                        body.Replace("@@REASON@@", task.Comments.ToString());

                        body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);


                        reqToMailid = usr.EmailId;


                        if (task.AccessRequest.AdHocAccessRequest.RequesterUserId != task.AccessRequest.AdHocAccessRequest.RequesteeUserId)
                        {
                            reqByMailid = requester.EmailId;
                        }
                        emailInfo.From = mailTemplate.MailFrom;
                    }

                    emailInfo.Body = body.ToString();
                    emailInfo.Subject = mailSubject.ToString();
                    emailInfo.To = string.Format("{0},{1}", reqToMailid, reqByMailid);

                    emailInfo.Cc = string.Format("{0}", MasterConfigHelper.AccountCreatedNotificationTO);


                    emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                    emailInfo.CreatedBy = "System";
                    emailInfo.CreatedDate = DateTime.Now;
                    emailInfo.CreatedDateTime = DateTime.Now;
                    EmailHandler.Send(emailInfo);
                    return;
                }

            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);
                return;
            }
        }
        public static void NotifyApproverAboutRequestRejectedBulkRequest(long approverUserId, long rejectedApproverId, BulkRequest bulkRequest, string rejectionReason)
        {
            try
            {
                var approver = User.GetById(approverUserId);
                NotifyApproverAboutRequestRejectedBulkRequest(approver, rejectedApproverId, bulkRequest, rejectionReason);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"NotifyApproverAboutRequestRejectedBulkRequest:{ex.Message}", ex);
            }
        }

        static void NotifyApproverAboutRequestRejectedBulkRequest(User approver, long rejectedApproverId, BulkRequest bulkRequest, string rejectionReason)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate mailTemplate = MailTemplate.GetByTemplateName(Constants.BulkRequestRejectAlertToApprovers);

                if (mailTemplate == null) return;

                var requester = User.GetById(bulkRequest.RequestedBy);

                var rejectedApprover = User.GetById(rejectedApproverId);

                string appName = string.Empty;
                if (bulkRequest.ApplicationType == ApplicationType.MSSL_CRM ||
                      bulkRequest.ApplicationType == ApplicationType.MSSL_ISU)
                {
                    appName = Constants.MSSL_ISU_CREM;
                }
                else if (bulkRequest.ApplicationType == ApplicationType.EBS_CRM ||
                    bulkRequest.ApplicationType == ApplicationType.EBS_ISU)
                {
                    appName = Constants.EBS_ISU_CREM;
                }
                else
                {
                    appName = UtilityMethods.GetEnumAttribute<Description>(bulkRequest.ApplicationType);
                }

                StringBuilder body = new StringBuilder(mailTemplate.MailContent);
                StringBuilder mailSubject = new StringBuilder(mailTemplate.MailSubject);

                string toMailid = string.Empty;

                mailSubject.Replace("@@ApplicationName@@", appName);
                mailSubject.Replace("@@REQUESTTYPE@@", bulkRequest.AdhocActionType.ToString());
                mailSubject.Replace("@@REQUESTID@@", bulkRequest.Id.ToString());

                string displayName = string.Join(",", bulkRequest.BulkRequestDetails.Select(item => $"{item.Name}({item.EmployeeId})").ToList());

                body.Replace("@@REJECTEDUSER@@", rejectedApprover.DisplayName);

                body.Replace("@@APPROVER@@", approver.DisplayName);
                body.Replace("@@USER@@", displayName);
                body.Replace("@@DATE@@", DateTime.Now.MailDateTimeFormat());
                body.Replace("@@REQUESTID@@", bulkRequest.Id.ToString());
                body.Replace("@@ApplicationName@@", appName);
                body.Replace("@@REQUESTTYPE@@", bulkRequest.AdhocActionType.ToString());

                body.Replace("@@REQUESTBY@@", GetUser(bulkRequest.RequestedBy)?.DisplayName ?? "");
                body.Replace("@@ACCOUNTTYPE@@", AccountType.User.ToString());
                body.Replace("@@EMPLOYEETYPE@@", bulkRequest.IsEmployee ? EmployeeType.Permanent.ToString() : "Non-Employee");
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                body.Replace("@@REJECTIONREASON@@", rejectionReason);
                toMailid = (approver == null || string.IsNullOrWhiteSpace(approver.EmailId)) ? string.Empty : Convert.ToString(approver.EmailId);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = mailSubject.ToString();
                emailInfo.To = string.Format("{0}", toMailid);

                emailInfo.From = mailTemplate.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"NotifyApproverAboutRequestRejectedBulkRequest:{ex.Message}", ex);
            }
        }

        static void NotifyApproverAboutRequestRejectedAccessRequest(long approverUserId, long rejectedApproverId, AccessRequest accessRequest)
        {
            try
            {
                var approver = User.GetById(approverUserId);
                NotifyApproverAboutRequestRejectedAccessRequest(approver, rejectedApproverId, accessRequest);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"NotifyApproverAboutRequestRejectedAccessRequest:{ex.Message}", ex);
            }
        }

        public static void NotifyApproverAboutRequestRejectedAccessRequest(User approver, long rejectedApproverId, AccessRequest accessRequest)
        {
            try
            {

                EmailInfo emailInfo = new EmailInfo();


                MailTemplate mailTemplate = MailTemplate.GetByTemplateName(Constants.RequestRejectAlertToApprovers);

                if (mailTemplate == null) return;

                var requester = User.GetById(accessRequest.AdHocAccessRequest.RequesterUserId);
                var requestee = User.GetById(accessRequest.AdHocAccessRequest.RequesteeUserId);
                var rejectedApprover = User.GetById(rejectedApproverId);

                string appName = string.Empty;
                if (accessRequest.AdHocAccessRequest.ApplicationType == ApplicationType.MSSL_CRM ||
                      accessRequest.AdHocAccessRequest.ApplicationType == ApplicationType.MSSL_ISU)
                {
                    appName = Constants.MSSL_ISU_CREM;
                }
                else if (accessRequest.AdHocAccessRequest.ApplicationType == ApplicationType.EBS_CRM ||
                    accessRequest.AdHocAccessRequest.ApplicationType == ApplicationType.EBS_ISU)
                {
                    appName = Constants.EBS_ISU_CREM;
                }
                else
                {
                    appName = UtilityMethods.GetEnumAttribute<Description>(accessRequest.AdHocAccessRequest.ApplicationType);
                }

                StringBuilder body = new StringBuilder(mailTemplate.MailContent);
                StringBuilder mailSubject = new StringBuilder(mailTemplate.MailSubject);

                string toMailid = string.Empty;

                mailSubject.Replace("@@ApplicationName@@", appName);
                mailSubject.Replace("@@REQUESTTYPE@@", accessRequest.AdHocAccessRequest.AdhocActionType.ToString());
                mailSubject.Replace("@@REQUESTID@@", accessRequest.Id.ToString());
                mailSubject.Replace("@@USER@@", requestee.DisplayName);
                mailSubject.Replace("@@pernrNum@@", requestee.EmployeeId);


                string displayName = requestee.DisplayName;
                string employeeId = requestee.EmployeeId;

                body.Replace("@@REJECTEDUSER@@", rejectedApprover.DisplayName);

                body.Replace("@@APPROVER@@", approver.DisplayName);
                body.Replace("@@USER@@", displayName);
                body.Replace("@@DATE@@", DateTime.Now.MailDateTimeFormat());
                body.Replace("@@REQUESTID@@", accessRequest.Id.ToString());
                body.Replace("@@ApplicationName@@", appName);
                body.Replace("@@REQUESTTYPE@@", accessRequest.AdHocAccessRequest.AdhocActionType.ToString());
                body.Replace("@@pernrNum@@", employeeId);

                body.Replace("@@REQUESTBY@@", requester.DisplayName);
                body.Replace("@@REASON@@", accessRequest.Comments);
                body.Replace("@@ACCOUNTTYPE@@", accessRequest.AccountType.ToString());
                body.Replace("@@EMPTYPE@@", requestee.EmployeeType.ToString());
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                toMailid = (approver == null || string.IsNullOrWhiteSpace(approver.EmailId)) ? string.Empty : approver.EmailId;

                emailInfo.Body = body.ToString();
                emailInfo.Subject = mailSubject.ToString();
                emailInfo.To = string.Format("{0}", toMailid);

                emailInfo.From = mailTemplate.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"NotifyApproverAboutRequestRejectedAccessRequest:{ex.Message}", ex);


            }
        }
        public static bool SendEmailForSectionOrDesignationEmpty(string UserName, string Section, string Designation)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate(Constants.NotificationNPACIncomplete);

                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@UserName@@", UserName);

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@UserName@@", UserName);
                body.Replace("@@Section@@", Section);
                body.Replace("@@Designation@@", Designation);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = string.Format("{0}", MasterConfigHelper.AccountCreatedNotificationTO);
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);
                return false;
            }

        }
        public static bool SendEmailForAccountProvisioningForOffice365(string accountName, User user, string appName, string appFullName)
        {
            string division = string.Empty;
            string section = string.Empty;

            MailTemplate template = null;
            StringBuilder subject = null;
            User uFacade = new User();

            try
            {
                EmailInfo emailInfo = new EmailInfo();

                template = GetMailTemplate(Constants.OnboardingUserEmail);

                subject = new StringBuilder(template.MailSubject);

                subject.Replace("@@APPLICATIONNAME@@", appName);
                if (!string.IsNullOrEmpty(user.EmployeeId))
                    subject.Replace("@@EMPLOYEEID@@", Convert.ToString(user.EmployeeId));

                subject.Replace("@@USERNAME@@", accountName);

                string body = GetNewAccountTemplate(template.MailContent, accountName, user, appName, appFullName);
                body = body.Replace("@@REGARDS@@", "IAM Administrator");
                emailInfo.Body = body;
                emailInfo.Subject = subject.ToString();
                emailInfo.From = template.MailFrom;

                emailInfo.To = user.EmailId;

                emailInfo.Cc = string.Format("{0}", MasterConfigHelper.AccountCreatedNotificationCC);
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);

            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailForAccountProvisioningForOffice365:{ex.Message}", ex);
                return false;
            }

            return true;
        }
        public static string GetNewAccountTemplate(string template, string accountName, User user, string appName, string appFullName)
        {
            StringBuilder body = new StringBuilder(template);


            body.Replace("@@USERDISPLAYNAME@@", user.DisplayName);

            body.Replace("@@COMPANY@@", !string.IsNullOrWhiteSpace(user.OrgCompany) ? user.OrgCompany : user.CompanyName);
            body.Replace("@@DEPT@@", !string.IsNullOrWhiteSpace(user.OrgDepartment) ? user.OrgDepartment : user.DepartmentName);
            body.Replace("@@DIVISION@@", string.Format("/{0}", !string.IsNullOrWhiteSpace(user.OrgDivision) ? user.OrgDivision : user.DivisionName));
            body.Replace("@@BRANCH@@", !string.IsNullOrWhiteSpace(user.OrgBranch) ? user.OrgBranch : string.Empty);
            body.Replace("@@SECTION@@", string.Format("/{0}", !string.IsNullOrWhiteSpace(user.OrgSection) ? user.OrgSection : user.SectionName));
            body.Replace("@@UNIT@@", string.Format("/{0}", !string.IsNullOrWhiteSpace(user.OrgUnit) ? user.OrgUnit : user.Reserved8 ?? string.Empty));
            body.Replace("@@coyCode@@", user.CompanyCode);
            body.Replace("@@coyText@@", user.CompanyName);
            body.Replace("@@deptCode@@", user.DepartmentCode);
            body.Replace("@@deptText@@", user.DepartmentName);
            body.Replace("@@branchcode@@", user.BranchCode);
            body.Replace("@@branchtext@@", user.OrgBranch);

            body.Replace("@@sectioncode@@", user.SectionCode);
            body.Replace("@@sectiontext@@", user.OrgBranch);

            body.Replace("@@unitcode@@", user.UnitCode);
            body.Replace("@@unittext@@", user.Reserved8);

            //body.Replace("@@divCode@@", user.DivisionCode);
            //body.Replace("@@divText@@", user.DivisionName);
            body.Replace("@@orgCode@@", user.OrganisationCode);
            body.Replace("@@orgText@@", getOrganisationText(user.OrganisationCode));

            if (appFullName == string.Empty)
                body.Replace("@@APPLICATIONNAMEFULL@@", string.Empty);
            else
                body.Replace("@@APPLICATIONNAMEFULL@@", string.Format("({0})", appFullName));

            body.Replace("@@APPLICATIONNAME@@", appName);

            body.Replace("@@USERNAME@@", accountName);
            body.Replace("@@PASSWORD@@", string.Format(ProvisioningConstants.PasswordMessage, "AD/Windows"));

            body.Replace("@@ACCOUNTTYPE@@", "User");

            body.Replace("@@RES1@@", string.Empty);
            body.Replace("@@RES2@@", string.Empty);

            return body.ToString();
        }
        public static void NotifyAdminOffice365Failed(string upn, AccessRequest accessRequest)
        {
            IList<EmailInfo> listMails = new List<EmailInfo>();
            EmailInfo mail = null;


            string strMailTo = string.Empty;
            try
            {
                mail = new EmailInfo();

                string mailContent = string.Empty;
                string mailSubject = string.Empty;
                var requestId = accessRequest?.Id.ToString();
                MailTemplate mailTemplate = GetMailTemplate(Constants.NotifyOffice365Failed);
                if (mailTemplate == null) return;

                mailContent = mailTemplate.MailContent;
                mailSubject = mailTemplate.MailSubject;


                if (string.IsNullOrEmpty(strMailTo))
                {
                    strMailTo = string.Join(",", MasterConfigHelper.AlertNotificationReceivers);
                }

                StringBuilder subject = new StringBuilder(mailSubject);
                subject.Replace("@@UPN@@", upn);
                subject.Replace("@@REQUESTID@@", requestId);
                StringBuilder bldrAD = new StringBuilder(mailContent);
                bldrAD.Replace("@@REQUESTID@@", requestId);
                if (accessRequest != null && accessRequest.RequesterId.HasValue)
                {
                    bldrAD.Replace("@@REQUESTBy@@", GetUser(accessRequest.RequesterId.Value)?.DisplayName);
                }
                else
                {
                    bldrAD.Replace("@@REQUESTBy@@", "");
                }
                if (accessRequest != null)
                {
                    var requestFor = GetUser(accessRequest.UserId);
                    if (requestFor != null)
                    {
                        bldrAD.Replace("@@REQUESTFOR@@", requestFor.DisplayName);
                    }
                    else
                    {
                        bldrAD.Replace("@@REQUESTFOR@@", "");
                    }
                }
                else
                {
                    bldrAD.Replace("@@REQUESTFOR@@", "");
                }

                bldrAD.Replace("@@UPN@@", upn);
                bldrAD.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                mail.From = mailTemplate.MailFrom;
                mail.To = strMailTo;
                mail.Subject = subject.ToString();
                mail.Body = bldrAD.ToString();
                mail.IsHtml = true;
                mail.CreatedBy = "System";
                mail.CreatedDate = DateTime.Now;
                mail.CreatedDateTime = DateTime.Now;
                listMails.Add(mail);

                EmailHandler.Send(listMails);
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, string.Format("Error while sending email to IT Security about license assignment failure for user {0}.", upn), ex);
            }
        }

        public static bool NotifyPassOfficeAboutOnBoard(DomainModel.User user)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();


                MailTemplate template = GetMailTemplate(Constants.PassOfficeNotification);

                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@EMPLOYEEID@@", user.EmployeeId);
                subject.Replace("@@DISPLAYNAME@@", user.DisplayName);

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@USER@@", user.DisplayName);
                body.Replace("@@EMPTYPE@@", user.EmployeeType.ToString());
                body.Replace("@@pernrNum@@", user.EmployeeId);
                body.Replace("@@coyCode@@", user.CompanyCode);
                body.Replace("@@coyText@@", user.CompanyName);
                body.Replace("@@deptCode@@", user.DepartmentCode);
                body.Replace("@@deptText@@", user.DepartmentName);
                body.Replace("@@divCode@@", user.DivisionCode);
                body.Replace("@@divText@@", user.DivisionName);
                body.Replace("@@orgCode@@", user.SectionCode);
                body.Replace("@@orgText@@", user.SectionName);
                body.Replace("@@cctrCode@@", user.CostCenter);
                body.Replace("@@cctrText@@", user.CostCenterText);
                body.Replace("@@JOINDATE@@", user.JoinDate.Value.ToString("dd/MM/yyyy"));
                body.Replace("@@coyCode@@", user.CompanyCode);
                body.Replace("@@coyText@@", user.CompanyName);
                body.Replace("@@deptCode@@", user.DepartmentCode);
                body.Replace("@@deptText@@", user.DepartmentName);
                body.Replace("@@branchCode@@", user.BranchCode);
                body.Replace("@@branchText@@", user.OrgBranch);

                body.Replace("@@sectionCode@@", user.SectionCode);
                body.Replace("@@sectionText@@", user.OrgBranch);

                body.Replace("@@unitCode@@", user.UnitCode);
                body.Replace("@@unitText@@", user.Reserved8);

                //body.Replace("@@divCode@@", user.DivisionCode);
                //body.Replace("@@divText@@", user.DivisionName);
                body.Replace("@@orgCode@@", user.OrganisationCode);
                body.Replace("@@orgText@@", getOrganisationText(user.OrganisationCode));
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = string.Join(",", MasterConfigHelper.SMSPassOfficeEmail);
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"NotifyPassOfficeAboutOnBoard:{ex.Message}", ex);
                return false;
            }
        }

        public static bool NotifyPassOfficeAboutOffboard(User user)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate(Constants.PassOfficeOffBoardNotification);

                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@EMPLOYEEID@@", user.EmployeeId);
                subject.Replace("@@DISPLAYNAME@@", user.DisplayName);


                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@USER@@", user.DisplayName);
                body.Replace("@@EMPTYPE@@", user.EmployeeType.ToString());
                body.Replace("@@pernrNum@@", user.EmployeeId);
                body.Replace("@@coyCode@@", user.CompanyCode);
                body.Replace("@@coyText@@", user.CompanyName);
                //body.Replace("@@deptCode@@", user.DepartmentCode);
                //body.Replace("@@deptText@@", user.DepartmentName);
                //body.Replace("@@divCode@@", user.DivisionCode);
                //body.Replace("@@divText@@", user.DivisionName);
                //body.Replace("@@orgCode@@", user.SectionCode);
                //body.Replace("@@orgText@@", user.SectionName);
                body.Replace("@@cctrCode@@", user.CostCenter);
                body.Replace("@@cctrText@@", user.CostCenterText);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

                body.Replace("@@deptCode@@", user.DepartmentCode);
                body.Replace("@@deptText@@", user.DepartmentName);

                body.Replace("@@branchcode@@", user.BranchCode);
                body.Replace("@@branchtext@@", user.OrgBranch);

                body.Replace("@@sectioncode@@", user.SectionCode);
                body.Replace("@@sectiontext@@", user.SectionName);

                body.Replace("@@unitCode@@", user.UnitCode);
                body.Replace("@@unitText@@", user.Reserved8);

                //body.Replace("@@divCode@@", user.DivisionCode);
                //body.Replace("@@divText@@", user.DivisionName);
                body.Replace("@@orgCode@@", user.OrganisationCode);
                body.Replace("@@orgText@@", getOrganisationText(user.OrganisationCode));
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = string.Join(",", MasterConfigHelper.SMSPassOfficeEmail);
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;

                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"NotifyPassOfficeAboutOffboard:{ex.Message}", ex);
                return false;
            }
        }

        public static bool NotifySRMSupportTeam(AccessRequest accessRequest, string entitlement, string accountName, string requestType)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate("SRMSupportTeamNotification");
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);
                DomainModel.User user = DomainModel.User.GetById(accessRequest.UserId);

                if (entitlement == null)
                {
                    entitlement = "";
                }

                subject.Replace("@@USER@@", user.DisplayName);
                subject.Replace("@@USERID@@", user.EmployeeId.ToString());
                if (accessRequest.BulkRequestId.HasValue)
                {
                    subject.Replace("@@REQUESTID@@", accessRequest.BulkRequestId.ToString());
                }
                else
                {
                    subject.Replace("@@REQUESTID@@", accessRequest.Id.ToString());
                }
                subject.Replace("@@REQUESTTYPE@@", requestType);
                // Body
                body.Replace("@@USER@@", user.DisplayName);
                body.Replace("@@REQUEST_TYPE@@", requestType);
                body.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                body.Replace("@@userLogonName@@", accountName);
                body.Replace("@@accessName@@", entitlement);
                if (accessRequest.AdHocAccessRequest != null)
                    body.Replace("@@REQUESTBY@@", GetUser(accessRequest.AdHocAccessRequest.RequesterUserId).DisplayName);
                else
                {
                    body.Replace("@@REQUESTBY@@", "");
                }
                body.Replace("@@ACCOUNTTYPE@@", accessRequest.AccountType.ToString());
                body.Replace("@@EMPLOYEETYPE@@", user.EmployeeType.ToString());
                body.Replace("@@COMMENTS@@", accessRequest.Comments);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();

                emailInfo.To = template.MailTo;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);

                return false;
            }
        }

        public static bool NotifyOffice365SharedAccessRemovalToHelpDeskTeam(DomainModel.User user, AccessRequest accessRequest, string accountName, string requestType)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();


                MailTemplate template = GetMailTemplate(Constants.Office365LicenseRemovalNotification);

                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@SharedAccountEmail@@", accountName);

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@USER@@", user.DisplayName);
                body.Replace("@@pernrNum@@", user.EmployeeId);
                body.Replace("@@cctrCode@@", user.CostCenter);
                body.Replace("@@cctrText@@", user.CostCenterText);
                body.Replace("@@SharedAccountEmail@@", accountName);
                body.Replace("@@RequestType@@", requestType);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = string.Join(",", MasterConfigHelper.HelpDeskEmail);
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);
                return false;
            }
        }
        public static bool SendEmailUserExistingInApproversOrOwners(string employeeId, string matrix)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate(Constants.NotificationNPACIncomplete);

                //StringBuilder subject = new StringBuilder(template.MailSubject);
                //subject.Replace("@@UserName@@", UserName);

                //StringBuilder body = new StringBuilder(template.MailContent);
                //body.Replace("@@UserName@@", UserName);
                //body.Replace("@@Section@@", Section);
                //body.Replace("@@Designation@@", Designation);

                emailInfo.Body = $"The user ({employeeId}) is an approver/owner in {matrix}";
                emailInfo.Subject = "User's existence in matrix";
                emailInfo.To = string.Format("{0}", MasterConfigHelper.AccountCreatedNotificationTO);
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;

                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);
                return false;
            }

        }
        public static bool NotifySkypeForBusinessSupportTeam(AccessRequest accessRequest, string entitlement, string accountName, string requestType)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate("SRMSupportTeamNotification");
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);
                DomainModel.User user = DomainModel.User.GetById(accessRequest.UserId);

                if (entitlement == null)
                {
                    entitlement = "";
                }
                entitlement = entitlement.Replace(",", "<br/>");

                subject.Replace("@@USER@@", user.DisplayName);
                subject.Replace("@@USERID@@", user.EmployeeId.ToString());

                // Body
                body.Replace("@@USER@@", user.DisplayName);
                body.Replace("@@REQUEST_TYPE@@", requestType);
                body.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                body.Replace("@@userLogonName@@", accountName);
                body.Replace("@@accessName@@", entitlement);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();

                emailInfo.To = template.MailTo;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Exception thrown in NotifySkypeForBusinessSupportTeam - AccessRequest Id : {accessRequest.Id}", ex, null, string.Empty);

                return false;
            }
        }
        public static bool GetEmailInfoForSendingEmailForBulkRequest(EmailInfo emailInfo)
        {
            bool mailSent = false;
            try
            {
                EmailHandler.Send(emailInfo);
                mailSent = true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, "GetEmailInfoForSendingEmailForBulkRequest", ex, emailInfo.Id, emailInfo.Subject);
                mailSent = false;
            }
            return mailSent;
        }
        public static bool GetEmailInfoForSendingEmailForProvisioningRequest(EmailInfo emailInfo)
        {
            bool mailSent = false;
            try
            {
                if (emailInfo != null)
                {

                    EmailHandler.Send(emailInfo);
                    mailSent = true;
                }
            }
            catch (Exception ex)
            {
                mailSent = false;
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex, emailInfo.Id, emailInfo.Subject);
            }
            return mailSent;
        }
        public static bool GetEmailInfoForSendEmailForEmployeeEventApproved(EmailInfo emailInfo)
        {
            bool mailSent = false;
            try
            {

                EmailHandler.Send(emailInfo);
                mailSent = true;

            }
            catch (Exception ex)
            {
                mailSent = false;
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"GetEmailInfoForSendEmailForEmployeeEventApproved {ex.Message}", ex, emailInfo.Id, emailInfo.Subject);
            }
            return mailSent;
        }
        public static bool SendEmailForAdvanceDelegation(EmailInfo emailInfo)
        {
            bool mailSent = false;
            try
            {
                EmailHandler.Send(emailInfo);
                mailSent = true;
            }
            catch (Exception ex)
            {
                mailSent = false;
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailForAdvanceDelegation {ex.Message}", ex, emailInfo.Id, emailInfo.Subject);
            }
            return mailSent;
        }

        public static void SendFileReceivedNotification(PUBFile file, EntityModel.Attachment attachment)
        {
            if (file == null || attachment == null)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Info, "SendFileReceivedNotification: Either file or attachment is null");
                return;
            }
            EmailInfo emailInfo = new EmailInfo();
            try
            {
                MailTemplate template = GetMailTemplate("PubFileRecievedNotification");
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);

                if (string.IsNullOrWhiteSpace(file.ValidationMsg))
                {
                    subject.Replace("@@Status@@", "Successfully");
                    body.Replace("@@ValidationMessage@@", "Received Successfully");
                }
                else
                {
                    subject.Replace("@@Status@@", $"with Error {file.ValidationMsg}");
                    body.Replace("@@ValidationMessage@@", file.ValidationMsg);
                }

                body.Replace("@@FileName@@", attachment.AccessRequestSource);
                body.Replace("@@CreatedDate@@", file.CreatedDate.ToString(DATETIMEFORMAT));

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = $"{template.MailTo},{GetEmailIDsFromConfig(MasterConfigHelper.PUBProcess2NotificationTo)}";
                emailInfo.Cc = $"{template.MailCC},{GetEmailIDsFromConfig(MasterConfigHelper.PUBProcess2NotificationCC)}";
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.None.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                emailInfo.Attachment = attachment.Id.ToString();
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Exception during SendFileReceivedNotification. Exception Message: {ex.Message}", ex, emailInfo.Id, emailInfo.Subject);
            }
        }

        public static void SendFileProcessedNotification(PUBFile file, DomainModel.Attachment attachment)
        {
            if (file == null || attachment == null)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Info, "SendFileProcessedNotification: Either file or attachment is null");
                return;
            }

            EmailInfo emailInfo = new EmailInfo();
            try
            {
                MailTemplate template = GetMailTemplate("PubFileProcessedNotification");
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);

                body.Replace("@@FileName@@", attachment.AccessRequestSource);
                body.Replace("@@CreatedDate@@", file.CreatedDate.ToString(DATETIMEFORMAT));
                body.Replace("@@ReportedDate@@", file.ReportedDateTime?.ToString(DATETIMEFORMAT));

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = $"{template.MailTo},{GetEmailIDsFromConfig(MasterConfigHelper.PUBProcess3NotificationTo)}";
                emailInfo.Cc = $"{template.MailCC},{GetEmailIDsFromConfig(MasterConfigHelper.PUBProcess3NotificationCC)}";
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.None.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                emailInfo.Attachment = attachment.Id.ToString();
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Exception during SendFileProcessedNotification. Exception Message: {ex.Message}", ex, emailInfo.Id, emailInfo.Subject);
            }
        }

        public static void SendMonthlyFileGeneratedNotification(DomainModel.Attachment attachment)
        {
            if (attachment == null)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Info, "SendMonthlyFileGeneratedNotification: Either file or attachment is null");
                return;
            }

            EmailInfo emailInfo = new EmailInfo();
            try
            {
                MailTemplate template = GetMailTemplate("PubMonthlyFileGeneratedNotification");
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);

                body.Replace("@@FileName@@", attachment.AccessRequestSource);
                body.Replace("@@CreatedDate@@", DateTime.Now.ToString(DATETIMEFORMAT));

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = $"{template.MailTo},{GetEmailIDsFromConfig(MasterConfigHelper.PUBProcess1NotificationTo)}";
                emailInfo.Cc = $"{template.MailCC},{GetEmailIDsFromConfig(MasterConfigHelper.PUBProcess1NotificationCC)}";
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.None.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                emailInfo.Attachment = attachment.Id.ToString();
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Exception during SendMonthlyFileGeneratedNotification. Exception Message: {ex.Message}", ex, emailInfo.Id, emailInfo.Subject);
            }
        }


        public static void SendNoFileReceivedNotification()
        {
             
            EmailInfo emailInfo = new EmailInfo();
            try
            {
                MailTemplate template = GetMailTemplate("PubNoFileReceivedNotification");
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);

              
                body.Replace("@@CurrentDate@@", DateTime.Now.ToString(DATETIMEFORMAT));

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = $"{template.MailTo},{GetEmailIDsFromConfig(MasterConfigHelper.PUBProcess4NotificationTo)}";
                emailInfo.Cc = $"{template.MailCC},{GetEmailIDsFromConfig(MasterConfigHelper.PUBProcess4NotificationCC)}";
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.None.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                 
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Exception during SendNoFileReceivedNotification. Exception Message: {ex.Message}", ex, emailInfo.Id, emailInfo.Subject);
            }
        }
        private static string GetEmailIDsFromConfig(string toFormat)
        {
            var values = toFormat.Split('|');
            if (values.Count() != 2)
                return string.Empty;
            if (values[0].ToLower() == "emailid")
                return values[1];
            else if (values[0].ToLower() == "group")
                return User.GetEmailIdsByGroupName(values[1]);
            return string.Empty;
        }

        public static bool SendReminderEmailForAttestation(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration, WorkflowConfigurationDetail wfDetail)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();
                var enddate = currentTask.CreatedDate.AddDays(wfDetail.ResponseTimeInDays).MailDateFormat();

                MailTemplate template = MailTemplate.GetByTemplateName("RemindAttestationApproval");
                if(template == null)
                {
                    DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendReminderEmailForAttestation: missing Email template");
                    return false;
                }
                User user = null;
                var receiverName = string.Empty;
                string emailIds = string.Empty;
                if (currentTask.ActionUserId.HasValue)
                {
                    user = User.GetById(currentTask.ActionUserId.Value);
                    receiverName = user?.DisplayName;
                    if (user != null)
                    {
                        emailIds = user?.EmailId;
                    }
                    else
                        DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendReminderEmailForAttestation:ActionUser does not exists for Attestation:{currentTask?.AttestationCampaignId} ");
                }
                else
                {
                    if (currentTask.ActionGroupId.HasValue)
                    {
                        var group = Group.GetById(currentTask.ActionGroupId.Value);
                        receiverName = group?.GroupName;
                        if (group != null)
                        {
                            emailIds = User.GetEmailIdsByGroupName(group.GroupName);
                        }
                        else
                        {
                            DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendReminderEmailForAttestation:ActionGroup does not exists for Attestation:{currentTask?.AttestationCampaignId} ");
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(emailIds))
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format("Reminder email is not sent to {0} since he has no valid email id", user.UserName), null);
                    return false;
                }

                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@ATTESTATION_ENDPOINTNAME@@", currentTask.Attestation?.AttestationName);
                subject.Replace("@@ATTE_TYPE@@", $"{currentTask.Attestation?.AttestationType}");

                subject.Replace("@@CAMPAIGN_NAME@@", currentTask.AttestationCampaign?.CampaignName);

                StringBuilder body = new StringBuilder(template.MailContent);

                body.Replace("@@REVIEWER_NAME@@", receiverName);
                body.Replace("@@ATTESTATION_NAME@@", currentTask.Attestation?.AttestationName);
                body.Replace("@@ATTE_TYPE@@", $"{currentTask.Attestation?.AttestationType}");
                body.Replace("@@ATTESTATION_DESC@@", $"{currentTask.Attestation?.AttestationDescription}");
                body.Replace("@@CAMPAIGN_NAME@@", currentTask.AttestationCampaign?.CampaignName);
                body.Replace("@@APPROVAL_LEVEL@@", $"{currentTask.WorkflowStateId}");
                body.Replace("@@CAMPIGN_START_DATE@@", currentTask.AttestationCampaign?.CreatedDate.ToString("dd/MM/yyyy"));
                body.Replace("@@REVIEWLASTDATE@@", enddate);
                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Body = body.ToString();

                emailInfo.Subject = subject.ToString();
                if (!string.IsNullOrWhiteSpace(template.MailCC))
                    emailInfo.Cc = template.MailCC;

                if (!string.IsNullOrWhiteSpace(template.MailBcc))
                    emailInfo.Bcc = template.MailBcc;

                emailInfo.To = $"{emailIds},{template.MailTo}";
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Attestation.ToString();
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendReminderEmailForAttestation:{ex.Message}", ex);
                return false;
            }
        }
        /// <summary>
        /// Esclation Email
        /// </summary>
        /// <param name="currentTask"></param>
        /// <param name="currentWorkflowConfiguration"></param>
        /// <param name="attestation"></param>
        /// <returns></returns>
        public static bool SendEsclationEmailForAttestation(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration, Attestation attestation)
        {
            bool sent = false;
            try
            {

                EmailInfo emailInfo = new EmailInfo();
                var emailIds = string.Empty;
                MailTemplate template = MailTemplate.GetByTemplateName("EscalateAttestationApproval");
                User manager = null;
                User user = null;
                var epName = string.Empty;
                var iamTeam = string.Join(",",MasterConfigHelper.AlertNotificationReceivers);
                epName = string.Join(",", attestation.AttestationEndpoint.Select(i => i.EndPoint?.DisplayName));
                var reviewerName = string.Empty;
                var receiver = string.Empty;
                var campaign = AttestationCampaign.GetById(currentTask.AttestationCampaignId.Value);
                if (currentTask.ActionUserId.HasValue)
                {
                    user = User.GetById(currentTask.ActionUserId.Value);
                    reviewerName = user.DisplayName;
                    if (user != null && !string.IsNullOrWhiteSpace(user.ROEmployeeId))
                    {
                        manager = User.GetByEmployeeId(user.ROEmployeeId);
                    }

                    if (manager != null && manager.IsActive == false)
                    {
                        manager = User.GetByEmployeeId(manager.ROEmployeeId);
                    }
                }
                else if (currentTask.IsGroupTask && currentTask.ActionGroupId.HasValue)
                {
                    manager = null;
                    emailIds = string.Join(",", User.GetEmailIdsByGroupId(currentTask.ActionGroupId.Value));
                    var grp = Group.GetById(currentTask.ActionGroupId.Value)?.GroupName;
                    reviewerName = $"Group({grp}) Reviewer";
                    receiver = $"Group({grp}) Member";
                    
                }

                if (manager == null)
                {
                    if (string.IsNullOrWhiteSpace(emailIds))
                        emailIds = iamTeam;
                    receiver = "Receiver";
                }
                else
                {
                    emailIds = manager.EmailId;
                    receiver = manager.DisplayName;
                }
                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@ATTTSK_ENDPOINTNAME_DISP@@", epName);
                subject.Replace("@@ATTESTATION_NAME@@", attestation.AttestationName);
                subject.Replace("@@CAMPAIGN_NAME@@", campaign.CampaignName);
                subject.Replace("@@APPROVALLEVEL@@", currentTask.WorkflowStateId.ToString());
                subject.Replace("@@RECEIVER@@", receiver);
                subject.Replace("@@REVIEWER_NAME@@", reviewerName);


                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@RECEIVER@@", receiver);
                body.Replace("@@REVIEWER_NAME@@", reviewerName);
                body.Replace("@@ATTTSK_ENDPOINTNAME_DISP@@", epName);
                body.Replace("@@ATTESTATION_DESC@@", attestation.AttestationDescription);
                body.Replace("@@ATTESTATION_NAME@@", attestation.AttestationName);
                body.Replace("@@APPROVALLEVEL@@", currentTask.WorkflowStateId.ToString());
                body.Replace("@@CAMPAIGN_NAME@@", campaign.CampaignName);
                body.Replace("@@CAMPIGN_START_DATE@@", campaign.CreatedDate.ToString("dd/MM/yyyy"));
                body.Replace("@@REVIEWLASTDATE@@", currentTask.CreatedDate.AddDays(currentWorkflowConfiguration.WorkflowConfigurationDetails[0].ResponseTimeInDays).ToString("dd/MM/yyyy"));
                body.Replace("@@APPROVALLEVEL@@", currentTask.WorkflowStateId.ToString());
                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                if (currentTask.IsGroupTask)
                {
                    emailInfo.Cc = $"{template?.MailCC},{iamTeam}";
                }
                else
                {
                    emailInfo.Cc = $"{template?.MailCC},{user?.EmailId},{iamTeam}";
                }
                if (!string.IsNullOrWhiteSpace(template.MailCC))
                    emailInfo.Cc = $"{ template.MailCC},{ (currentTask.IsGroupTask ? iamTeam : (user?.EmailId))}";

                if (!string.IsNullOrWhiteSpace(template.MailBcc))
                    emailInfo.Bcc = template.MailBcc;

                emailInfo.To = $"{template.MailTo},{emailIds}";
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Attestation.ToString();
                EmailHandler.Send(emailInfo);
                sent = true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailForAttestation:{ex.Message}", ex);

            }
            return sent;
        }
        /// <summary>
        /// Send an email for Add delegation/Remove delegation notification to Email Team
        /// </summary>
        /// <param name="user"></param>
        /// <param name="accessRequest"></param>
        /// <param name="entitlement"></param>
        /// <param name="upn"></param>
        /// <param name="actionName">Add Delegation/Remove Delegation</param>
        /// <param name="appDisplayName">MSTeams</param>
        /// <returns></returns>
        public static bool NotifyMSTeamsDelegation(User user, AccessRequest accessRequest, string entitlement, string upn, string actionName, string appDisplayName,string lineUri,string source)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate("NotifyMSTeamsDelegation");
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);

                if (entitlement == null)
                {
                    entitlement = "";
                }
               
                if (accessRequest.ApplicationType != ApplicationType.SRM)
                    entitlement = entitlement.Replace(",", "<br/>");

                subject.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                subject.Replace("@@APPLICATIONNAME@@", appDisplayName);
                subject.Replace("@@USER@@", user.DisplayName);
                subject.Replace("@@ACTIONNAME@@", actionName);
                subject.Replace("@@REQUESTSOURCE@@", source);
                // Body
                body.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                body.Replace("@@APPLICATIONNAME@@", appDisplayName);
                body.Replace("@@USER@@", user.DisplayName);
                body.Replace("@@UPN@@", upn);
                if (accessRequest.RequesterId.HasValue)
                {
                    body.Replace("@@REQUESTBY@@", GetUser(accessRequest.RequesterId.Value).DisplayName);
                }
                else
                {
                    body.Replace("@@REQUESTBY@@", "System");
                }
                body.Replace("@@REQUESTFOR@@", GetUser(accessRequest.UserId).DisplayName);
                body.Replace("@@RESPONSEGROUPNAMES@@", entitlement);
                body.Replace("@@REQUESTSOURCE@@", source);
                body.Replace("@@ACTIONNAME@@", actionName);
                body.Replace("@@LINEURI@@", lineUri);
                body.Replace("@@EMAIL@@", user.EmailId);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();

                var emailTeam = DI.Chakra.DomainModel.MasterConfigHelper.EmailTeam;
                var group = Group.GetByGroupName(emailTeam);
                emailInfo.To = string.Format("{0}", string.Join(",", group.Users.Where(i => !string.IsNullOrEmpty(i.EmailId)).Select(i => i.EmailId)));

                emailInfo.Cc = template.MailCC;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);
                return false;
            }
        }

        public static bool NotifyMSTeamIDDRemoval(AccessRequest accessRequest, User user,string upn,string lineUri, string appDisplayName, string action)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate("NotifyMSTeamIDDRemovalOnHREvent");
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);

                subject.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                subject.Replace("@@APPLICATIONNAME@@", appDisplayName);
                subject.Replace("@@USER@@", user.DisplayName);
                subject.Replace("@@REQUESTSOURCE@@", action);
                
                // Body
                body.Replace("@@REQUESTNUMBER@@", accessRequest.Id.ToString());
                body.Replace("@@USER@@", user.DisplayName);
                if (accessRequest.RequesterId.HasValue)
                {
                    body.Replace("@@REQUESTBY@@", GetUser(accessRequest.RequesterId.Value)?.DisplayName);
                }
                else
                {
                    body.Replace("@@REQUESTBY@@", "System");
                }
                
                body.Replace("@@EMAIL@@", user.EmailId);
                body.Replace("@@APPLICATIONNAME@@", appDisplayName);
                body.Replace("@@UPN@@", upn);
                body.Replace("@@REQUESTFOR@@", GetUser(accessRequest.UserId).DisplayName);
                body.Replace("@@REQUESTSOURCE@@", action);
                body.Replace("@@LINEURI@@", lineUri);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();

                var emailTeam = MasterConfigHelper.EmailTeam;
                var group = Group.GetByGroupName(emailTeam);
                emailInfo.To = string.Format("{0}", string.Join(",", group.Users.Where(i => !string.IsNullOrEmpty(i.EmailId)).Select(i => i.EmailId)));

                emailInfo.Cc = template.MailCC;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"NotifyMSTeamIDDRemoval Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);
                return false;
            }
        }

        public static bool NotifyInactiveEmployeeEvent(string employeeId, string eventName, string date, string existingEventId)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate("NotifyInactiveEmployeeEvent");
                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder subject = new StringBuilder(template.MailSubject);

                subject.Replace("@@EMPLOYEEID@@", employeeId);
                subject.Replace("@@EVENTNAME@@", eventName);
                subject.Replace("@@EFFECTIVEDATE@@", date);
                subject.Replace("@@EXISTING_EVENTID@@", existingEventId);
                // Body
                body.Replace("@@EMPLOYEEID@@", employeeId);
                body.Replace("@@EVENTNAME@@", eventName);
                body.Replace("@@EFFECTIVEDATE@@", date);
                body.Replace("@@EXISTING_EVENTID@@", existingEventId);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();

                var emailTeam = MasterConfigHelper.ITSecurityGroupId;
                var group = Group.GetByIdIncludingUsers(emailTeam);
                emailInfo.To = string.Format("{0}", string.Join(",", group.Users.Where(i => !string.IsNullOrWhiteSpace(i.EmailId)).Select(i => i.EmailId)));

                emailInfo.Cc = template.MailCC;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Discovery.ToString();
                emailInfo.CreatedBy = "System";
                emailInfo.CreatedDate = DateTime.Now;
                emailInfo.CreatedDateTime = DateTime.Now;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"NotifyInactiveEmployeeEvent Error:{ex.Message} StackTrace:{ex.StackTrace}", ex);
                return false;
            }
        }

        public static bool SendEmailForAdhocProvisioning(Task currentTask)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate("RemindAdhocProvisioningApproval");
                string emailIds, approverName;
                GetApproverInformation(currentTask, out emailIds, out approverName);
                var requester = User.GetById(currentTask.AccessRequest.AdHocAccessRequest.RequesterUserId);
                var RequesteeUser = User.GetById(currentTask.AccessRequest.AdHocAccessRequest.RequesteeUserId);

                string appName = string.Empty;
                try
                {
                    appName = ApplicationName(currentTask.AccessRequest.EndpointSystemId.Value);
                }
                catch (Exception)
                {
                }

                if (string.IsNullOrWhiteSpace(appName))
                {
                    if (currentTask.AccessRequest.ApplicationType == ApplicationType.MSSL_CRM ||
                            currentTask.AccessRequest.ApplicationType == ApplicationType.MSSL_ISU)
                    {
                        appName = Constants.MSSL_ISU_CREM;
                    }
                    else if (currentTask.AccessRequest.ApplicationType == ApplicationType.EBS_CRM ||
                        currentTask.AccessRequest.ApplicationType == ApplicationType.EBS_ISU)
                    {
                        appName = Constants.EBS_ISU_CREM;
                    }
                    else
                    {
                        appName = UtilityMethods.GetEnumAttribute<Description>(currentTask.AccessRequest.ApplicationType);
                    }
                }
                string requestDetail = string.Format("{0} - {1}", UtilityMethods.GetEnumAttribute<Description>(currentTask.AccessRequest.AdHocAccessRequest.AdhocActionType), UtilityMethods.GetEnumAttribute<Description>(currentTask.AccessRequest.AdHocAccessRequest.AccountType));

                if (string.IsNullOrEmpty(emailIds))
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, "Reminder email is not sent to  since he has no valid email id", null);
                    return false;
                }

                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@applicationName@@", appName);
                subject.Replace("@@requestType@@", currentTask.AccessRequest.AdHocAccessRequest.AdhocActionType.ToString());
                subject.Replace("@@requestNum@@", currentTask.AccessRequestId.ToString());

                subject.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                subject.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);

                subject.Replace("@@RECEIVERID@@", requester.EmployeeId.ToString());


                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@approverName@@", approverName);
                body.Replace("@@requestNum@@", currentTask.AccessRequestId.ToString());
                body.Replace("@@applicationName@@", appName);
                body.Replace("@@requestType@@", currentTask.AccessRequest.AdHocAccessRequest.AdhocActionType.ToString());
                body.Replace("@@REQUESTBY@@", requester.DisplayName);
                body.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                body.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);
                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                body.Replace("@@EMPTYPE@@", RequesteeUser.EmployeeType.ToString());

                body.Replace("@@AccountType@@", UtilityMethods.GetEnumAttribute<Description>(currentTask.AccessRequest.AdHocAccessRequest.AccountType));

                if (string.IsNullOrEmpty(currentTask.AccessRequest.AdHocAccessRequest.Comments))
                    body.Replace("@@COMMENTS@@", string.Empty);
                else
                    body.Replace("@@COMMENTS@@", currentTask.AccessRequest.AdHocAccessRequest.Comments?.Replace("\n", "<br>"));

                emailInfo.Body = body.ToString();
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Subject = subject.ToString();
                emailInfo.To = emailIds;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailForAdhocProvisioning:{ex.Message}", ex);
                return false;
            }
        }
        private static void GetApproverInformation(Task currentTask, out string emailIds, out string approverName)
        {
            emailIds = string.Empty;
            approverName = string.Empty;
            try
            {
                if (currentTask.IsGroupTask == false)
                {
                    if (currentTask.ActionUserId.HasValue)
                    {
                        var usr = User.GetByUserId(currentTask.ActionUserId.Value);
                        emailIds = usr?.EmailId;
                        approverName = usr?.DisplayName;
                    }

                }
                else
                {
                    if (currentTask.ActionGroupId.HasValue)
                    {
                        approverName = currentTask.GroupName;
                        emailIds = $"{string.Join(",", User.GetUsetEmailIdsByGroupId(currentTask.ActionGroupId.Value))}";
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"GetApproverInformation:{ex.Message}", ex);
            }
        }
        public static bool SendEmailForBulkRequest(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration)
        {
            try
            {

                if (currentTask == null || currentTask.BulkRequest == null) return false;

                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate("RemindBulkRequestApproval");
                //var user = utility.GetUserByUserId(currentTask.ActionUserId.Value);
                string emailIds, approverName;
                GetApproverInformation(currentTask, out emailIds, out approverName);
                User requester = null;
                if (currentTask.BulkRequest != null)
                    requester = User.GetByUserId(currentTask.BulkRequest.RequestedBy);

                string appName = string.Empty;

                string accounttype = string.Format("{0}", UtilityMethods.GetEnumAttribute<Description>(AccountType.User));

                if (string.IsNullOrEmpty(emailIds))
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format("Notification email is not sent to {0} since he has no valid email id", currentTask.Approver), null);
                    return false;
                }
                try
                {
                    appName = ApplicationName(currentTask.BulkRequest.EndpointSystemId);
                }
                catch (Exception)
                {
                }
                if (string.IsNullOrWhiteSpace(appName))
                {

                    // TODO:
                    // Need to change this code
                    if (currentTask.BulkRequest != null && (currentTask.BulkRequest.ApplicationType == ApplicationType.MSSL_CRM ||
                           currentTask.BulkRequest.ApplicationType == ApplicationType.MSSL_ISU))
                    {
                        appName = Constants.MSSL_ISU_CREM;
                    }
                    else if (currentTask.BulkRequest != null && (currentTask.BulkRequest.ApplicationType == ApplicationType.EBS_CRM ||
                        currentTask.BulkRequest.ApplicationType == ApplicationType.EBS_ISU))
                    {
                        appName = Constants.EBS_ISU_CREM;
                    }
                    else
                    {
                        appName = UtilityMethods.GetEnumAttribute<Description>(currentTask.BulkRequest.ApplicationType);
                    }
                }
                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@applicationName@@", appName);
                subject.Replace("@@requestType@@", currentTask.BulkRequest.AdhocActionType.ToString());
                subject.Replace("@@requestNum@@", currentTask.BulkRequestId.ToString());

                subject.Replace("@@RECEIVERID@@", requester?.EmployeeId?.ToString());

                string displayName = string.Join(",", currentTask.BulkRequest.BulkRequestDetails.Select(item => item.Name).ToList());
                string employeeId = string.Join(",", currentTask.BulkRequest.BulkRequestDetails.Select(item => item.EmployeeId).ToList());

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@approverName@@", approverName);
                body.Replace("@@requestNum@@", currentTask.BulkRequestId.ToString());
                body.Replace("@@applicationName@@", appName);
                body.Replace("@@requestType@@", currentTask.BulkRequest.AdhocActionType.ToString());

                body.Replace("@@userDisplayName@@", displayName);
                body.Replace("@@pernrNum@@", employeeId);
                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);

                body.Replace("@@AccountType@@", accounttype);

                if (string.IsNullOrEmpty(currentTask.BulkRequest.Description))
                    body.Replace("@@COMMENTS@@", string.Empty);
                else
                    body.Replace("@@COMMENTS@@", currentTask.BulkRequest.Description?.Replace("\n", "<br>"));
                body.Replace("@@REQUESTBY@@", requester?.DisplayName);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                body.Replace("@@EMPTYPE@@", currentTask.BulkRequest.IsEmployee ? EmployeeType.Permanent.ToString() : "Non-Employee");

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString(); //string.Format(template.MailSubject, appName);
                emailInfo.To = emailIds;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();

                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailForBulkRequest:{ex.Message}", ex);
                return false;
            }
        }
        public static bool SendEmailForEmployeeEvent(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration, EmployeeEventTrigger trigger)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = GetMailTemplate("RemindEmployeeEventApproval");
                var user = User.GetById(currentTask.ActionUserId.Value);
                var requester = User.GetByName(trigger.CreatedBy);
                var RequesteeUser = User.GetById(trigger.UserId);

                if (string.IsNullOrEmpty(user.EmailId))
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format("Reminder email is not sent to {0} since he has no valid email id", user.UserName), null);
                    return false;
                }

                StringBuilder subject = new StringBuilder(template.MailSubject);

                string requestType = string.Empty;

                if (trigger.EventType == EmployeeEvents.NonEmployeeCreation)
                {
                    requestType = UtilityMethods.GetEnumAttribute<Description>(AccessRequestSource.EmployeeCreation);
                }
                else if (trigger.EventType == EmployeeEvents.NonEmployeeExtension)
                {
                    requestType = UtilityMethods.GetEnumAttribute<Description>(AccessRequestSource.EmployeeExtension);
                }

                subject.Replace("@@requestType@@", requestType);

                subject.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                subject.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@RECEIVER@@", user.DisplayName);
                body.Replace("@@requestType@@", requestType);

                body.Replace("@@requesterDisplayName@@", requester.DisplayName);
                body.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                body.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);
                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                body.Replace("@@EMPTYPE@@", RequesteeUser.EmployeeType.ToString());
                body.Replace("@@REVIEWLASTDATE@@", GetTaskCompletionDate(currentWorkflowConfiguration.WorkflowConfigurationDetails[0].ResponseTimeInDays, currentTask.CreatedDate).MailDateFormat());
                body.Replace("@@REQUESTEDDATE@@", trigger.CreatedDate.MailDateTimeFormat());

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = user.EmailId;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailForEmployeeEvent:{ex.Message}", ex);
                return false;
            }
        }
        public static DateTime GetTaskCompletionDate(int responseTimeInDays, DateTime taskCreatedDate)
        {
            return taskCreatedDate.AddDays(responseTimeInDays);
        }

        public static void NotifyITAdminForReverseEngineering(Dictionary<string,List<string>> audits, SchedulerLogger logger)
        {

            string strMessage = string.Empty;
            StringBuilder strBldr = new StringBuilder();
            DomainModel.MailTemplate template = null;
            StringBuilder subject = null;
            try
            {

                EmailInfo emailInfo = new EmailInfo();
                template = MailTemplate.GetByTemplateName(Constants.ADReconcilationNotification);
                subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@ENDPOINTNAME@@", "Other System(s)");

                StringBuilder body = new StringBuilder(template.MailContent);
                StringBuilder actionData = new StringBuilder();
                if (audits != null && audits.Count > 0)
                {
                    actionData.Append("<table class='table'><tr><th>EndpointSystem Name</th><th>Action</th></tr>");
                    foreach (var systemData in audits)
                    {
                        actionData.Append($"<tr><td>{systemData.Key}</td><td><ul>");
                        foreach (var action in systemData.Value)
                        {
                            actionData.Append($"<li>{action}</li>");
                        }
                        actionData.Append($"</ul></td></tr>");
                    }
                    actionData.Append("</table>");
                }
                body.Replace("@@REVIEWER_NAME@@", "");
                
                body.Replace("@@ACTIONLIST@@", actionData.ToString());
                body.Replace("@@EMPLOYEEDATA@@", "");
                body.Replace("@@ENDPOINTNAME@@", "Other System(s)");
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = string.Join(",", MasterConfigHelper.AlertNotificationReceivers);
                emailInfo.From = template.MailFrom;
                if (!string.IsNullOrWhiteSpace(template.MailTo))
                    emailInfo.To = emailInfo.To + "," + template.MailTo;

                if (!string.IsNullOrWhiteSpace(template.MailCC))
                    emailInfo.Cc = template.MailCC;

                if (!string.IsNullOrWhiteSpace(template.MailBcc))
                    emailInfo.Bcc = template.MailBcc;

                emailInfo.ActivityType = EmailActivityType.Discovery.ToString();
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                logger.WriteLog(LogLevelType.Exception, "Failed in composing mail from template:NotifySystemOwnerForNewAccount", ex);
            }
        }
    }
}
