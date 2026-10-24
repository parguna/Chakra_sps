using System;
using System.Collections.Generic;
using System.Data;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using DI.Chakra.Infrastructure;
using System.Linq;
using DI.Chakra.Repository;
using System.Text;
using DI.Chakra.Logger;
using System.Reflection;
using System.Collections;

namespace DI.Chakra.DomainModel
{
    public abstract class XLDataValidator
    {
        const string SQL_BACKUP_TABLE = @"IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'{0}_history'))
                                            BEGIN
	                                            DROP TABLE {0}_history
                                            END
                                            SELECT * INTO {0}_history FROM {0}
                                            TRUNCATE TABLE {0}";
        const string SQL_BACKUP_TABLE_UPDATE = @"IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'{0}_history'))
                                            
                                            insert into {0}_history select *,SYSDATETIME() as BackupDate from {0}
                                            TRUNCATE TABLE {0}";
        public const string NULL = "NULL";
        public string TableName { get; set; }
        public IXLData Data { get; set; }

        public DateTime UploadDateTime { get; set; }

        public IList<IXLData> InvalidList { get; set; }

        public IList<IXLData> ValidList { get; set; }

        public virtual IXLData ExistingData { get; set; }

        public bool canClearListobj { get; set; }
        public abstract string GetWhoUpdatedData();
        public abstract bool Validate(ref string validationMsg);

        public abstract bool IsExists(string currentUser);
        public abstract Type GetDomainModelType();

        public bool PartialUpload { get; set; }

        public virtual bool TruncateTable()
        {
            try
            {
                var query = string.Format(SQL_BACKUP_TABLE, TableName);
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.EntityBase>(dataAccessTransaction).ExecuteQuery(query);
                    dataAccessTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public abstract void Add(string currentUser);

        public virtual void Update(string currentUser)
        {
        }

        public virtual void PostProcess(string currentUser)
        {
        }

        public virtual IXLData GetObjectFromData(DataRow dRow)
        {
            return null;
        }

        public virtual IXLData GetObjectFromData(DataRow dRow, long endpointSystemId)
        {
            return null;
        }

        public virtual bool IsEmptyRow(IXLData data)
        {
            return false;
        }

        public virtual bool IsEmployeeConfiguredAsApprover(string employeeId)
        {
            return false;
        }

        public virtual bool ValidateTechnicalRole(IXLData data, ref string validationMsg)
        {
            return true;
        }

        public static List<EmailInfo> PostDiscoveryValidation(EndPoint ep, IdmsLogger logger, DiscoveryProcess process)
        {
            List<EmailInfo> emailInfos = new List<EmailInfo>();

            try
            {
                List<XLTemplates> templates = new List<XLTemplates>();
                switch (ep.ApplicationType)
                {
                    case ApplicationType.HRMS_FLINS:
                        templates.AddRange(new List<XLTemplates> { XLTemplates.SAP_HRFLINSConflictedBusinessRoles, XLTemplates.SAP_HRFLINSRoleMatrix, XLTemplates.SAP_HRFLINSSuppRoleMatrix, XLTemplates.SAP_RoleBasedOnEEGroup, XLTemplates.SAP_RoleBasedOnOrgMgt, XLTemplates.SAP_RoleBasedOnPosition });
                        break;
                    case ApplicationType.FLINS_BI:
                        templates.Add(XLTemplates.SAP_BIRoleCategories);
                        break;
                    case ApplicationType.SAP_GRC:
                        templates.Add(XLTemplates.SAP_PGR_GRC_Role);
                        break;
                    case ApplicationType.EBS_CRM:
                    case ApplicationType.EBS_ISU:
                        templates.AddRange(new List<XLTemplates> { XLTemplates.SAP_EBSRoleMatrix, XLTemplates.SAP_EBSCriticalTCodesMatrix });
                        break;
                    case ApplicationType.MSSL_CRM:
                    case ApplicationType.MSSL_ISU:
                        templates.Add(XLTemplates.SAP_MSSLRoleMatrix);
                        break;
                    case ApplicationType.SRM:
                        templates.AddRange(new List<XLTemplates> { XLTemplates.SAPSRMEnablerRoleMatrix, XLTemplates.SAPSRMMainRoleMatrix });
                        break;
                    case ApplicationType.EAM:
                        templates.AddRange(new List<XLTemplates> { XLTemplates.NOVAUserRoleMatrix, XLTemplates.NOVAManagementRoleMatrix, XLTemplates.NOVAExceptionalRoleMatrix });
                        break;
                    case ApplicationType.OneCard:
                        templates.Add(XLTemplates.SMSRoleCatalogue);
                        break;
                    case ApplicationType.EBS:
                    case ApplicationType.MSSL:
                        templates.AddRange(new List<XLTemplates> { XLTemplates.SAPHANA_RoleMatrix, XLTemplates.SAPHANA_CriticalTCodesMatrix });
                        break;
                    default: break;

                }

                StringBuilder stringBuilder = new StringBuilder();

                foreach (var template in templates)
                {
                    XLDataValidator xLDataValidator = XLDataValidator.GetXLDataValidator(template);
                    stringBuilder.Clear();
                    Type domainModelType = xLDataValidator.GetDomainModelType();

                    object domainModelObject = domainModelType.GetConstructor(new Type[] { }).Invoke(new object[] { });

                    MethodInfo method = domainModelObject.GetType().BaseType.GetMethods().FirstOrDefault(m => m.Name == "GetAll" && m.GetParameters().Length == 0);

                    var items = (ICollection)method.Invoke(null, new object[] { });

                    string validationMessage = string.Empty;

                    foreach (var data in items)
                    {
                        xLDataValidator.ValidateTechnicalRole(data as IXLData, ref validationMessage);
                        if (!string.IsNullOrWhiteSpace(validationMessage))
                        {
                            stringBuilder.AppendLine(validationMessage);
                            validationMessage = string.Empty;
                        }
                    }

                    if (stringBuilder.Length > 0)
                    {
                        var emailInfo = SendEmail(process, ep, logger, stringBuilder.ToString(), template.ToString(), "Matrix Name");
                        if (emailInfo != null)
                            emailInfos.Add(emailInfo);
                    }

                }

                emailInfos.AddRange(defaultEntitlementValidation(ep, logger, process));

            }
            catch (Exception ee)
            {
                logger.WriteLog(LogLevelType.Exception, "Exception occured in PostDiscoveryValidation", ee);
            }
            return emailInfos;
        }

        static List<EmailInfo> defaultEntitlementValidation(EndPoint ep, IdmsLogger logger, DiscoveryProcess process)
        {
            List<EmailInfo> emailInfos = new List<EmailInfo>();
            try
            {
                var accountTypes = new[] { AccountType.User, AccountType.Shared, AccountType.Secondary, AccountType.Service };
                foreach (var accountType in accountTypes)
                {
                    var nProvisioningFields = NProvisioningField.GetProvisioningDefaults(ep.Id, accountType);

                    if (ep.EndPointSystemType != null)
                    {
                        switch (ep.EndPointSystemType.EndpointSystemTypeName)
                        {
                            case EndpointType.SAP:
                                if (nProvisioningFields != null && nProvisioningFields["Roles"] != null && !string.IsNullOrWhiteSpace(nProvisioningFields["Roles"].Value))
                                {
                                    if (!isSAPRoleValid(ep.Id, nProvisioningFields["Roles"].Value))
                                    {
                                        var msg = $"Found Invalid Technical Role in Default Provisioning for the endpoint {ep.DisplayName} and AccountType : {accountType.ToString()}";

                                        var emailInfo = SendEmail(process, ep, logger, msg, $"Default Provisioning for the endpoint {ep.DisplayName} and AccountType : {accountType.ToString()}", "Provisioning Form");

                                        if (emailInfo != null)
                                        {
                                            emailInfos.Add(emailInfo);
                                        }
                                    }
                                }
                                break;
                            case EndpointType.OneCard:
                                if (nProvisioningFields != null && nProvisioningFields["DefaultAccessLevels"] != null && !string.IsNullOrWhiteSpace(nProvisioningFields["DefaultAccessLevels"].Value))
                                {
                                    if (!isSMSRoleValid(ep.Id, nProvisioningFields["DefaultAccessLevels"].Value))
                                    {
                                        var msg = $"Found Invalid Access Level in Default Provisioning for the endpoint {ep.DisplayName} and AccountType : {accountType.ToString()}";

                                        var emailInfo = SendEmail(process, ep, logger, msg, $"Default Provisioning for the endpoint {ep.DisplayName} and AccountType : {accountType.ToString()}", "Provisioning Form");

                                        if (emailInfo != null)
                                        {
                                            emailInfos.Add(emailInfo);
                                        }
                                    }
                                }
                                break;
                            default: break;
                        }

                    }

                }
            }
            catch (Exception ee)
            {
                logger.WriteLog(LogLevelType.Exception, "Exception occured in defaultEntitlementValidation", ee);

            }
            return emailInfos;
        }

        static EmailInfo SendEmail(DiscoveryProcess process, EndPoint m_endpoint, IdmsLogger logger, string str, string place, string placeLabel)
        {
            string strMessage = string.Empty;
            StringBuilder strBldr = new StringBuilder();
            DiscoveryProcess discoveryProcess = process;
            DomainModel.MailTemplate template = null;
            StringBuilder subject = null;
            try
            {
                EmailInfo emailInfo = new EmailInfo();
                template = MailTemplate.GetByTemplateName(Constants.PostDiscoveryJobValidation);
                subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@ENDPOINTNAME@@", m_endpoint.DisplayName);
                subject.Replace("@@PROCESSNAME@@", place);
                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@ERRORMESSAGE@@", str);
                body.Replace("@@ENDPOINTTYPE@@", m_endpoint.EndPointSystemType.DisplayName);
                body.Replace("@@ENDPOINTNAME@@", m_endpoint.DisplayName);
                body.Replace("@@PROCESSNAME@@", place);
                body.Replace("@@PROCESSlabel@@", placeLabel);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = string.Join(",", MasterConfigHelper.AlertNotificationReceivers);
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Discovery.ToString();
                return emailInfo;
            }
            catch (Exception ex)
            {
                logger.WriteLog(LogLevelType.Exception, "XLValidator.SEndMail : Failed in composing mail from template", ex);
                return null;
            }
        }

        public static XLDataValidator GetXLDataValidator(XLTemplates templateName)
        {
            switch (templateName)
            {
                case XLTemplates.Inhouse_UserInfo:
                    return new InhouseUserXLValidator();
                case XLTemplates.Inhouse_Permissions:
                    return new InhousePermissionXLValidator();
                case XLTemplates.Inhouse_Functions:
                    return new InhouseFunctionXLValidator();
                case XLTemplates.Inhouse_UserPermissionFunctionMatrix:
                    return new InhouseUserPermissionFunctionXLValidator();
                case XLTemplates.ADS_PROVISIONINGMATRIX:
                    return new ADSProvMatrixDataValidator();
                case XLTemplates.SAP_RoleBasedOnOrgMgt:
                    return new SAPProvMatrixByOrgDataValidator();
                case XLTemplates.SAP_BIRoleCategories:
                    return new SAPBIRoleCategoriesXLValidator();
                case XLTemplates.SAP_PIP_SAP_PI_Role:
                    return new PIPSAPPIRoleMatrixDataValidator();
                case XLTemplates.SAP_RoleBasedOnEEGroup:
                    return new SAPRoleBasedEEGroupXLValidator();
                case XLTemplates.SAP_RoleBasedOnPosition:
                    return new SapRoleBasedPositionXLValidator();
                case XLTemplates.SAP_HRFLINSRoleMatrix:
                    return new SAPFLINSRoleMatrixDataValidator();
                case XLTemplates.SAP_HRFLINSSuppRoleMatrix:
                    return new SAPFLINSRoleSuppMatrixDataValidator();
                case XLTemplates.SAP_RoleMandatoryFields:
                    return new SAPRoleMandatoryDataValidator();
                case XLTemplates.SAP_RoleMatrix:
                    return new SAPRoleEligibilityMatrixDataValidator();
                case XLTemplates.SAP_HRFLINSConflictedBusinessRoles:
                    return new SAPFLINSConflictedRoleMatrixDataValidator();
                case XLTemplates.SAP_PGR_GRC_Role:
                    return new PGRGRCRoleMatrixDataValidator();
                case XLTemplates.SAP_MSSLCriticalTCodesMatrix:
                    return new SAPMSSLCriticalTCodeMatrixDataValidator();
                case XLTemplates.SAP_MSSLPositionMatrix:
                    return new SAPMSSLPositionMatrixDataValidator();
                case XLTemplates.SAP_MSSLRoleMatrix:
                    return new SAPMSSLRoleMatrixDataValidator();
                case XLTemplates.SAP_EBSCriticalTCodesMatrix:
                    return new SAPEBSCriticalTCodeMatrixDataValidator();
                case XLTemplates.SAP_EBSPositionMatrix:
                    return new SAPEBSPositionMatrixDataValidator();
                case XLTemplates.SAP_EBSRoleMatrix:
                    return new SAPEBSRoleMatrixDataValidator();
                case XLTemplates.SAP_EPRoleInfo:
                    return new SAPEPRoleInfoXLValidator();
                case XLTemplates.SAP_EPGroupInfo:
                    return new SAPEPGroupInfoValidator();
                case XLTemplates.CE_RoleInfo:
                    return new CERoleInfoXLValidator();
                case XLTemplates.CE_GroupInfo:
                    return new CEGroupInfoXLValidator();
                case XLTemplates.NPACConfigurations:
                    return new NPACSectionProvMatrixDataValidator();
                case XLTemplates.NPAC_Designation:
                    return new NPACDesignationProvMatrixDataValidator();
                case XLTemplates.SAPSRMMainRoleMatrix:
                    return new SAPSRMMainRoleMatrixDataValidator();
                case XLTemplates.SAPSRMEnablerRoleMatrix:
                    return new SAPSRMEnablerRoleMatrixDataValidator();
                case XLTemplates.SAPSRMBusinessRoleMapping:
                    return new SAPSRMBusinessRoleDataValidator();
                case XLTemplates.GEMSRoleMatrix:
                    return new GemsRoleMatrixDataValidator();
                case XLTemplates.Office365License:
                    return new Office365LicenseValidator();
                case XLTemplates.SMSProvisioningMatrix:
                    return new SMSProvisioningMatrixDataValidator();
                case XLTemplates.SMSRoleCatalogue:
                    return new SMSRoleCatalogueValidator();
                case XLTemplates.NOVAUserRoleMatrix:
                    return new NOVAUserRoleMatrixXLValidator();
                case XLTemplates.NOVAManagementRoleMatrix:
                    return new NOVAManagementRoleMatrixXLValidator();
                case XLTemplates.NOVAExceptionalRoleMatrix:
                    return new NOVAExceptionalRoleMatrixXLValidator();
                case XLTemplates.NOVASoDExceptionsMatrix:
                    return new NOVASoDExceptionsMatrixXLValidator();
                case XLTemplates.NOVACompositeRoleMatrix:
                    return new NOVACompositeRoleMatrixXLValidator();
                case XLTemplates.ApprovalException:
                    return new ApprovalExceptionInfoXLValidator();
                case XLTemplates.VPNConfigurations:
                    return new VPNMatrixDataValidator();
                case XLTemplates.SkypeForBusinessConfigurations:
                    return new SkypeForBusinessProvisioningMatrixDataValidator();
                case XLTemplates.SAP_SODConflictedBusinessRoles:
                    return new SAPSODConflictedRoleMatrixDataValidator();
                case XLTemplates.SAPHANA_PositionMatrix:
                    return new SAPHANAPositionMatrixDataValidator();
                case XLTemplates.SAPHANA_RoleMatrix:
                    return new SAPHANARoleMatrixDataValidator();
                case XLTemplates.SAPHANA_CriticalTCodesMatrix:
                    return new SAPHANACriticalTCodeMatrixDataValidator();
                default:
                    break;
            }
            return null;
        }

        public static IList<string> GetFields(XLTemplates templateName)
        {
            return ConfigurationMaster.GetByConfigKey(UtilityMethods.GetEnumAttribute<XLTemplateName>(templateName), Product.IACM).ConfigValue.Split(',').ToList();
        }

        protected string CheckNullAndGetValueFromCell(string cellText)
        {
            cellText = cellText.Trim();
            if (string.Compare(cellText, NULL, true) != 0)
                return cellText;
            return string.Empty;
        }

        protected string CheckCreatedAndGetDefault(string cellText)
        {
            cellText = cellText.Trim();
            if (string.IsNullOrEmpty(cellText))
                return "System";
            return cellText;
        }

        public bool IsUserValid(string value, ApprovalUserType Type)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return User.IsValid(value, Type);
            }
        }

        public bool IsEndpointAvailableForApplication(string application)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var endpoint = EndPoint.GetByApplicationCode(application.Trim());
                if (endpoint != null && endpoint.Id > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static DateTime ConvertStringToDate(string inputDate)
        {
            DateTime outputDate;
            outputDate = DateTime.Now;
            try
            {
                if (string.IsNullOrWhiteSpace(inputDate))
                {
                    outputDate = DateTime.Now;
                }
                else
                {
                    string[] dateParts = inputDate.Split('/');
                    outputDate = new DateTime(Convert.ToInt32(dateParts[2]), Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[0]));
                }
            }
            catch (Exception)
            {
            }
            return outputDate;
        }
        public static DateTime? ConvertStringToDate(string inputDate, bool considerNullIfItsEmpty = false)
        {
            DateTime? outputDate;
            outputDate = DateTime.Now;
            try
            {
                if (string.IsNullOrWhiteSpace(inputDate))
                {
                    if (considerNullIfItsEmpty)
                    {
                        outputDate = null;
                    }
                    else
                    {
                        outputDate = DateTime.Now;

                    }
                }
                else
                {
                    string[] dateParts = inputDate.Split('/');
                    outputDate = new DateTime(Convert.ToInt32(dateParts[2]), Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[0]));
                }
            }
            catch (Exception)
            {
            }
            return outputDate;
        }
        public static bool ConvertStringToBool(string inputString)
        {
            if(!string.IsNullOrWhiteSpace(inputString))
            {
                inputString = inputString.Trim();
            }
            if (inputString.Equals("yes", StringComparison.InvariantCultureIgnoreCase) || inputString.Equals("1", StringComparison.InvariantCultureIgnoreCase) || inputString.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            else if (inputString.Equals("no", StringComparison.InvariantCultureIgnoreCase) || inputString.Equals("0", StringComparison.InvariantCultureIgnoreCase) || inputString.Equals("false", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            else
            {
                return false;
            }
        }
        public bool IsADGroupValid(string value)
        {
            var entitlements = value.Split(',');
            var isExists = false;
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                foreach (var entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement.Trim()))
                    {
                        isExists = ADSGroupInfo.DoesExist(entitlement.Trim());
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
            }
            return isExists;
        }

        //TODO: Include endpointsystem id in where clause
        public bool IsSAPEPGroupValid(string value)
        {
            var entitlements = value.Split(',');
            var isExists = false;
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                foreach (var entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement))
                    {
                        isExists = SAPEPGroupInfo.DoesExist(entitlement.Trim());
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
            }
            return isExists;
        }
        public bool IsSAPEPRoleValid(string value)
        {
            var entitlements = value.Split(',');
            var isExists = false;
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                foreach (var entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement))
                    {
                        isExists = SAPEPRoleInfo.DoesExist(entitlement.Trim());
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
            }
            return isExists;
        }
        public bool IsSAPEPGroupValid(ApplicationType applicationType, string value)
        {
            try
            {
                string[] entitlements = value.Split(',');
                bool isExists = false;

                long endpointSystemId = this.GetEndpointSystemID(applicationType);

                foreach (string entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement))
                    {
                        isExists = SAPEPGroupInfo.IsSAPEPGroupValid(endpointSystemId, entitlement.Trim());
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
                return isExists;
            }
            catch
            {
                return false;
            }
        }

        //TODO: Include endpointsystem id in where clause
        public bool IsSAPRoleValid(string value)
        {
            var entitlements = value.Split(',');
            var isExists = false;

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                foreach (var entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement.Trim()))
                    {
                        isExists = SAPRoleInfo.DoesExist(entitlement.Trim());
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
            }
            return isExists;
        }
        public bool IsSAPRoleValid(ApplicationType applicationType, string value)
        {
            try
            {
                string[] entitlements = value.Split(',');
                bool isExists = false;

                long endpointSystemId = GetEndpointSystemID(applicationType);

                foreach (string entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement))
                    {
                        if(applicationType == ApplicationType.EBS || applicationType == ApplicationType.MSSL)
                        {
                            isExists = SAPHANARoleInfo.IsSAPRoleValid(endpointSystemId, entitlement.Trim());
                        }
                        else
                        {
                            isExists = SAPRoleInfo.IsSAPRoleValid(endpointSystemId, entitlement.Trim());
                        }
                        
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
                return isExists;
            }
            catch
            {
                return false;
            }
        }

        //TODO: Include endpointsystem id in where clause
        public bool IsSAPTCodeValid(string value)
        {
            var entitlements = value.Split(',');
            var isExists = false;

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                foreach (var entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement))
                    {
                        isExists = SAPTCodeInfo.isExists(entitlement.Trim());
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
            }
            return isExists;
        }

        //Added
        public bool IsSAPHANATCodeValid(string value)
        {
            var entitlements = value.Split(',');
            var isExists = false;

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                foreach (var entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement))
                    {
                        isExists = SAPHANATCodeInfo.isExists(entitlement.Trim());
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
            }
            return isExists;
        }

        //TODO: Include endpointsystem id in where clause
        public bool IsADOUValid(ApplicationType applicationType, string value)
        {
            try
            {
                long endpointSystemId = this.GetEndpointSystemID(applicationType);
                return ADSOuInfo.IsADOUValid(endpointSystemId, value);
            }
            catch
            {
                return false;
            }
        }
        public long GetEndpointSystemID(ApplicationType applicationtype)
        {
            try
            {
                return EndPoint.GetByApplicationType(applicationtype, false).Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public ApprovalUserType GetApprovalUserTypesByDescription(string description)
        {
            try
            {
                return (ApprovalUserType)Enum.Parse(typeof(ApprovalUserType), description, true);
            }
            catch
            {
                return ApprovalUserType.None;
            }
        }
        public EntitlementType GetRequestTypesByDescription(string description)
        {
            try
            {
                return (EntitlementType)Enum.Parse(typeof(EntitlementType), description, true);
            }
            catch
            {
                return EntitlementType.None;
            }
        }
        public string FormatLeadingZeros(string Value, int Padinglength)
        {
            try
            {
                if (!string.IsNullOrEmpty(Value))
                {
                    if (Value.Length < Padinglength)
                    {
                        char pad = '0';
                        string output = Value.PadLeft(Padinglength, pad);
                        return output;
                    }
                    else
                    {
                        return Value;
                    }
                }
                else return string.Empty;
            }
            catch (Exception ex)
            {

                return Value;
            }
        }

        public bool IsTCodeValid(EndPoint ep, string value)
        {
            try
            {
                string[] entitlements = value.Split(',');
                bool isExists = false;

                long endpointSystemId = ep.Id;

                foreach (string entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement))
                    {
                        isExists = SAPRoleInfo.IsSAPRoleValid(endpointSystemId, entitlement.Trim());
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
                return isExists;
            }
            catch
            {
                return false;
            }
        }

        public bool IsADGroupValid(ApplicationType applicationType, string value)
        {
            try
            {
                string[] entitlements = value.Split(',');
                bool isExists = false;

                long endpointSystemId = GetEndpointSystemID(applicationType);

                foreach (string entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement))
                    {
                        isExists = ADSGroupInfo.IsADGroupValid(endpointSystemId, entitlement.Trim());
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
                return isExists;
            }
            catch
            {
                return false;
            }
        }
        public bool IsCompanyCodeValid(string value)
        {
            try
            {
                return ADSXLProvisioningMatrix.IsCompanyCodeValid(value);
            }
            catch
            {
                return false;
            }
        }
        public bool IsPersonnelAreaValid(string value)
        {
            try
            {
                return ADSXLProvisioningMatrix.IsPersonnelAreaValid(value);
            }
            catch
            {
                return false;
            }
        }

        public bool IsPersonnelSubAreaValid(string value)
        {
            try
            {
                return ADSXLProvisioningMatrix.IsPersonnelSubAreaValid(value);
            }
            catch
            {
                return false;
            }
        }

        public bool IsCostCenterValid(string value)
        {
            try
            {
                return ADSXLProvisioningMatrix.IsCostCenterValid(value);
            }
            catch
            {
                return false;
            }
        }

        public bool IsOrgUnitValid(string value)
        {
            try
            {
                return ADSXLProvisioningMatrix.IsOrgUnitValid(value);
            }
            catch
            {
                return false;
            }
        }

        public bool IsSectionCodeValid(string value)
        {
            try
            {
                return NOrganisation.GetByOrgCode(value) != null;
            }
            catch
            {
                return false;
            }
        }

        public bool IsSAPSectionCodeValid(string value)
        {
            try
            {
                return SAPSectionMappingInfo.GetBySAPSectionCode(value) != null;
            }
            catch
            {
                return false;
            }
        }

        public bool IsPositionValid(string value)
        {
            try
            {
                return NDesignation.GetByCode(value) != null;
            }
            catch
            {
                return false;
            }
        }

        //public new bool IsUserValid(string Value, ApprovalUserType Type)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrWhiteSpace(Value))
        //        {
        //            return ADSXLProvisioningMatrix.IsUserValid(Value, Type);
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        public bool IsValidApproverLevel(int value)
        {
            try
            {
                if (value == 1 || value == 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {

                return false;
            }
        }
        public string FormatEmployeeID(string employeeid)
        {
            try
            {
                if (!string.IsNullOrEmpty(employeeid))
                {
                    if (employeeid.Length < 8)
                    {
                        char pad = '0';
                        string empId = employeeid.PadLeft(8, pad);
                        return empId;
                    }
                    else
                    {
                        return employeeid;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {

                return employeeid;
            }
        }
        public bool IsSRMAcountnameValid(string accountName, AccountType accountType)
        {
            try
            {
                var endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SRP);
                var sapUsr = SAPUserinfo.GetByUserLogonName(accountName, endpoint.Id);
                if (sapUsr == null)
                {
                    endpoint = EndPoint.GetEndPointByApplicationType(ApplicationType.SAP_SUS);
                    sapUsr = SAPUserinfo.GetByUserLogonName(accountName, endpoint.Id);
                    if (sapUsr == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {

                return false;
            }
        }
        public bool IsSRMBussinessRoleValid(string businessRole)
        {
            try
            {
                return SAPSRMEnablerRoleMatrix.IsSAPSRMMainRoleExist(businessRole);

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IsGEMSRoleValid(string value)
        {
            try
            {
                string[] entitlements = value.Split(',');
                bool isExists = false;

                foreach (string entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement))
                    {
                        isExists = GEMSRoleMatrix.IsGEMSRoleValid(entitlement.Trim());
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
                return isExists;
            }
            catch
            {
                return false;
            }
        }
        public bool IsSAPRoleValid(long endpointSystemId, string value)
        {
            return isSAPRoleValid(endpointSystemId, value);
        }
        public bool IsSAPHanaRoleValid(long endpointSystemId, string value)
        {
            return isSAPHanaRoleValid(endpointSystemId, value);
        }

        static bool isSAPRoleValid(long endpointSystemId, string value)
        {
            try
            {
                string[] entitlements = value.Split(',');
                bool isExists = false;

                foreach (string entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement))
                    {
                        isExists = SAPRoleInfo.IsSAPRoleValid(endpointSystemId, entitlement.Trim());
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
                return isExists;
            }
            catch
            {
                return false;
            }
        }

        static bool isSAPHanaRoleValid(long endpointSystemId, string value)
        {
            try
            {
                string[] entitlements = value.Split(',');
                bool isExists = false;

                foreach (string entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement))
                    {
                        isExists = SAPHANARoleInfo.IsSAPRoleValid(endpointSystemId, entitlement.Trim());
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
                return isExists;
            }
            catch
            {
                return false;
            }
        }

        public bool IsSMSRoleValid(long endpointSystemId, string value)
        {
            return isSMSRoleValid(endpointSystemId, value);
        }

        static bool isSMSRoleValid(long endpointSystemId, string value)
        {
            try
            {
                string[] entitlements = value.Split(',');
                bool isExists = false;

                foreach (string entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement))
                    {
                        isExists = SMSXLProvisioningMatrix.IsSMSRoleValid(entitlement.Trim(), endpointSystemId);
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
                return isExists;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsUserValidCheck(string value, ApprovalUserType Type)
        {
            return User.IsValid(value, Type);
        }

        public virtual bool TruncateTableWithBackup()
        {
            try
            {
                var query = string.Format(SQL_BACKUP_TABLE_UPDATE, TableName);
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.EntityBase>(dataAccessTransaction).ExecuteQuery(query);
                    dataAccessTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
