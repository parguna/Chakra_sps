
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Services.EmailingProcessor;
using System;
using System.Linq;
using System.Text;

namespace DI.Chakra.ProvisioningUtility
{
    public class ProvisioningValidator
    {
        public static string IsProvisioningDefaultExists(User user, EmployeeEvents employeeEvent)
        {
            string message = string.Empty;

            message += IsADDefaultsExists(user);

            if (employeeEvent != EmployeeEvents.ReOrganisation)
            {
                message += IsFLINSDefaultsExists(user);
            }

            if (employeeEvent == EmployeeEvents.OUChange)
            {
                message += IsSMSDefaultsExists(user);
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, $"Trigger {employeeEvent.ToString()} Validation Failed for User -{user.KnownAs}. Message - {message}", null);
                if (CanAlertAdmin(user.Id, employeeEvent))
                {
                    AlertITAdmin(user, message, employeeEvent);
                }
            }

            return message;
        }

        public static string IsADDefaultsExists(User user)
        {
            string message = string.Empty;
            var adConfig = ADSXLProvisioningMatrix.GetByUser(user);

            if (adConfig == null)
            {
                adConfig = PopulateADProvisioningMatrix(user);

                adConfig.Save(MasterConfigHelper.ServiceUser);
            }

            if (string.IsNullOrEmpty(adConfig.OU))
            {
                message += $"Default OU for AD is missing for Company - {adConfig.CompanyCode}, Department - {adConfig.DepartmentCode}, Branch - {adConfig.DivisionCode} and Organistion Code - {adConfig.OrganisationId}-{adConfig.OrganisationName}." + Environment.NewLine;
            }
            return message;
        }

        public static string IsSMSDefaultsExists(User user)
        {
            string message = string.Empty;
            if (MasterConfigHelper.CanIncludeOneCardMatrix)
            {
                var smsConfig = SMSXLProvisioningMatrix.GetByUser(user);

                if (smsConfig == null)
                {
                    smsConfig = PopulateSMSProvisioningMatrix(user);

                    smsConfig.Save(MasterConfigHelper.ServiceUser);
                }

                if (string.IsNullOrEmpty(smsConfig.DefaultGroups))
                {
                    message += "Default Access levels for OneCard is missing." + Environment.NewLine;
                }
            }

            return message;
        }

        public static string IsFLINSDefaultsExists(User user)
        {
            string message = string.Empty;

            var flinsConfig = ProvisioningConfigMatrixSAPOrg.GetSAPRolesMatrixesBasedOnUserOrg(user);
            var sapRoles = string.Join(",", flinsConfig.Select(item => item.DefaultHRMSGeneralRoles));

            if (flinsConfig.Count == 0)
            {
                var flinsmatrix = PopulateProvisioningConfigMatrixSAPOrg(user);
                flinsmatrix.Save("System");
                message += "FLINS default provisioning info is missing." + Environment.NewLine;
            }
            else if(string.IsNullOrWhiteSpace(sapRoles))
            {
                message += "FLINS default provisioning info is incomplete." + Environment.NewLine;
            }

            return message;
        }


        static ProvisioningConfigMatrixSAPOrg PopulateProvisioningConfigMatrixSAPOrg(User user)
        {
            return new ProvisioningConfigMatrixSAPOrg()
            {
                CompanyCode = user.CompanyCode,
                CostCenterCode = user.CostCenter,
                DepartmentCode = user.DepartmentCode,
                DivisionCode = user.BranchCode,
                IsValid = true,
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.MaxValue
            };
        }

        public static bool AlertITAdmin(User user, string message, EmployeeEvents employeeEvent)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = MailTemplate.GetByTemplateName(Constants.AlertNotificationMailTemplate);
                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@pernr@@", user.EmployeeId);

                if (employeeEvent == EmployeeEvents.NewJoinee)
                {
                    subject.Replace("@@event@@", "On-Boarding");
                }
                else if (employeeEvent == EmployeeEvents.OUChange)
                {
                    subject.Replace("@@event@@", "OU Change");
                }
                else if (employeeEvent == EmployeeEvents.ReOrganisation)
                {
                    subject.Replace("@@event@@", "Re-Org");
                }
                else if (employeeEvent == EmployeeEvents.AdhocRequest)
                {
                    subject.Replace("@@event@@", "Adhoc Request");
                }
                else
                {
                    subject.Replace("@@event@@", string.Empty);
                }

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@USER@@", user.DisplayName);
                body.Replace("@@pernrNum@@", user.EmployeeId);
                body.Replace("@@coyCode@@", user.CompanyCode);
                body.Replace("@@coyText@@", user.CompanyName);
                body.Replace("@@deptCode@@", user.DepartmentCode);
                body.Replace("@@deptText@@", user.DepartmentName);
                body.Replace("@@branchcode@@", user.BranchCode);
                body.Replace("@@branchCode@@", user.BranchCode);

                body.Replace("@@branchtext@@", user.OrgBranch);
                body.Replace("@@branchText@@", user.OrgBranch);

                body.Replace("@@divCode@@", user.DivisionCode);
                body.Replace("@@divText@@", user.DivisionName);

                body.Replace("@@orgCode@@", user.SectionCode);
                body.Replace("@@orgText@@", user.SectionName);
                body.Replace("@@sectionCode@@", user.SectionCode);
                body.Replace("@@sectionText@@", user.SectionName);

                body.Replace("@@cctrCode@@", user.CostCenter);
                body.Replace("@@unitCode@@", user.UnitCode);
                body.Replace("@@unitText@@", user.OrgUnit);
                body.Replace("@@orgCode@@", user.OrganisationCode);
                body.Replace("@@orgText@@", getOrganisationText(user.OrganisationCode));

                body.Replace("@@cctrText@@", user.CostCenterText);
                body.Replace("@@MESSAGE@@", message);
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = string.Join(",", MasterConfigHelper.AlertNotificationReceivers);
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();

                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
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

        static bool CanAlertAdmin(long userId, EmployeeEvents employeeEvent)
        {
            var trigger = EmployeeEventTrigger.GetByUserIdAndEvent(userId, employeeEvent);

            if (trigger != null && !trigger.IsAdminCheckRequired)
            {
                trigger.IsAdminCheckRequired = true;
                trigger.LastModifiedBy = "System";
                trigger.LastModifiedDate = DateTime.Now;

                trigger.Save(MasterConfigHelper.ServiceUser);

                return true;
            }
            else
            {
                return false;
            }
        }

        static SMSXLProvisioningMatrix PopulateSMSProvisioningMatrix(User user)
        {
            return new SMSXLProvisioningMatrix()
            {
                DepartmentCode = user.DepartmentCode,
                DepartmentName = user.DepartmentName,
                DivisionCode = user.DivisionCode,
                DivisionName = user.DivisionName,
                PostalCode = user.PostalCode,
                IsValid = true,
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.MaxValue,

                DefaultGroups = string.Empty,
            };
        }

        static ADSXLProvisioningMatrix PopulateADProvisioningMatrix(User user)
        {
            return new ADSXLProvisioningMatrix()
            {
                CompanyCode = user.CompanyCode,
                CostCenterCode = user.CostCenter,
                CostDistributionCode = user.CostDistribution,
                DepartmentCode = user.DepartmentCode,
                DivisionCode =   string.IsNullOrWhiteSpace(user.BranchCode)?  "".PadLeft(8,'0') : user.BranchCode.PadLeft(8, '0'),
                IsValid = true,
                OrganisationId = user.OrganisationCode,
                OrganisationName = getOrganisationCodeName(user),
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.MaxValue,

                DefaultGroups = string.Empty,
                OU = string.Empty,
            };
        }

        static string getOrganisationCodeName(User user)
        {
            if (user.UnitCode == user.OrganisationCode) return user.OrgUnit;
            else if (user.SectionCode == user.OrganisationCode) return user.OrgSection;
            else if (user.BranchCode == user.OrganisationCode) return user.OrgBranch;
            else if (user.DepartmentCode == user.OrganisationCode) return user.OrgDepartment;
            else return user.OrgCompany;

        }

    }
}
