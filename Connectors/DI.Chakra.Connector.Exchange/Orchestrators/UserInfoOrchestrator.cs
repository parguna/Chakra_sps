using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.DomainModel;
using System.DirectoryServices;
using DI.Chakra.Infrastructure;
using System.Management.Automation;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Connector
{
    public class UserInfoOrchestrator
    {


        private const string PROCESSNAME = "DiscoveryProcess";

        public UserInfoOrchestrator()
        {
        }

        public long EndpointSystemId { get; set; }

        public int HistoryId { get; set; }
        //public ExUserInfo PopulateUser(PSObject directoryEntry, DirectorySearcher _dirSearcher)
        //{
        //    if (null == directoryEntry)
        //        return null;


        //    IList<ExUserInfo> list = new List<ExUserInfo>();

        //    ExUserInfo excUserInfo = new ExUserInfo();

        //    try
        //    {

        //        excUserInfo.TenantId = TenantId;
        //        excUserInfo.EndPointSystemId = EndpointSystemId;
        //        excUserInfo.cn = ExHelper.GetProperty(directoryEntry, ExProperties.cn);
        //        excUserInfo.sn = ExHelper.GetProperty(directoryEntry, ExProperties.sn);               
        //        excUserInfo.st = ExHelper.GetProperty(directoryEntry, ExProperties.st);
        //        excUserInfo.title = ExHelper.GetProperty(directoryEntry, ExProperties.title);
        //        excUserInfo.description = ExHelper.GetProperty(directoryEntry, ExProperties.description);
        //        excUserInfo.postalAddress = ExHelper.GetProperty(directoryEntry, ExProperties.postalAddress);
        //        excUserInfo.postalCode = ExHelper.GetProperty(directoryEntry, ExProperties.postalCode);
        //        excUserInfo.postOfficeBox = ExHelper.GetProperty(directoryEntry, ExProperties.postOfficeBox);
        //        excUserInfo.physicalDeliveryOfficeName = ExHelper.GetProperty(directoryEntry, ExProperties.physicalDeliveryOfficeName);
        //        excUserInfo.telephoneNumber = ExHelper.GetProperty(directoryEntry, ExProperties.telephoneNumber);
        //        excUserInfo.facsimileTelephoneNumber = ExHelper.GetProperty(directoryEntry, ExProperties.facsimileTelephoneNumber);
        //        excUserInfo.givenName = ExHelper.GetProperty(directoryEntry, ExProperties.givenName);
        //        excUserInfo.initials = ExHelper.GetProperty(directoryEntry, ExProperties.initials);
        //        excUserInfo.distinguishedName = ExHelper.GetProperty(directoryEntry, ExProperties.distinguishedName);
        //        excUserInfo.instanceType = ExHelper.GetProperty(directoryEntry, ExProperties.instanceType);
        //        excUserInfo.whenCreated = ExHelper.GetProperty_DateTime(directoryEntry, ExProperties.whenCreated);
        //        excUserInfo.whenChanged = ExHelper.GetProperty_DateTime(directoryEntry, ExProperties.whenChanged);
        //        excUserInfo.displayName = ExHelper.GetProperty(directoryEntry, ExProperties.displayName);
        //        excUserInfo.uSNCreated = ExHelper.GetCOMProperty_Int64(directoryEntry, ExProperties.uSNCreated);
        //        excUserInfo.info = ExHelper.GetProperty(directoryEntry, ExProperties.info);
        //        excUserInfo.uSNChanged = ExHelper.GetCOMProperty_Int64(directoryEntry, ExProperties.uSNChanged);
        //        excUserInfo.co = ExHelper.GetProperty(directoryEntry, ExProperties.co);
        //        excUserInfo.department = ExHelper.GetProperty(directoryEntry, ExProperties.department);
        //        excUserInfo.company = ExHelper.GetProperty(directoryEntry, ExProperties.company);
        //        excUserInfo.manager = ExHelper.GetProperty(directoryEntry, ExProperties.manager);                
        //        excUserInfo.streetAddress = ExHelper.GetProperty(directoryEntry, ExProperties.streetAddress);
        //        excUserInfo.wWWHomePage = ExHelper.GetProperty(directoryEntry, ExProperties.wWWHomePage);
        //        excUserInfo.name = ExHelper.GetProperty(directoryEntry, ExProperties.name);
        //        excUserInfo.objectGUID = ExHelper.GetProperty_GUID(directoryEntry, ExProperties.objectGUID);
        //        excUserInfo.userAccountControl = ExHelper.GetProperty(directoryEntry, ExProperties.userAccountControl);//gk
        //        excUserInfo.IsAccountDisabled = ExAccount.IsAccountDisabled(directoryEntry);
        //        excUserInfo.IsAccountLocked =  ExAccount.IsAccountLocked(directoryEntry);
        //        excUserInfo.IsPasswordRequired = ExAccount.IsPasswordRequired(directoryEntry);
        //        excUserInfo.PasswordExpirationDate = ExHelper.GetCOMProperty_DateTime(directoryEntry, ExProperties.badPasswordTime); 
        //        excUserInfo.badPwdCount = ExHelper.GetProperty(directoryEntry, ExProperties.badPwdCount);
        //        excUserInfo.codePage = ExHelper.GetProperty(directoryEntry, ExProperties.codePage);//gk
        //        excUserInfo.countryCode = ExHelper.GetProperty(directoryEntry, ExProperties.countryCode);
        //        excUserInfo.badPasswordTime = ExHelper.GetCOMProperty_DateTime(directoryEntry, ExProperties.badPasswordTime);                
        //        excUserInfo.lastLogoff =ExHelper.GetCOMProperty_DateTime(directoryEntry, ExProperties.lastLogoff);
        //        excUserInfo.lastLogon = ExHelper.GetCOMProperty_DateTime(directoryEntry, ExProperties.lastLogon);
        //        excUserInfo.pwdLastSet = ExHelper.GetCOMProperty_DateTime(directoryEntry, ExProperties.pwdLastSet);                
        //        excUserInfo.primaryGroupID = ExHelper.GetProperty(directoryEntry, ExProperties.primaryGroupID);//gk
        //        excUserInfo.objectSid = ExHelper.GetProperty_SID(directoryEntry, ExProperties.objectSid);
        //        excUserInfo.accountExpires = ExAccountHelper.AccountExpiresOn(directoryEntry);                
        //        excUserInfo.logonCount = ExHelper.GetProperty(directoryEntry, ExProperties.logonCount);
        //        excUserInfo.sAMAccountName = ExHelper.GetProperty(directoryEntry, ExProperties.sAMAccountName);
        //        excUserInfo.sAMAccountType = ExHelper.GetProperty(directoryEntry, ExProperties.sAMAccountType);
        //        excUserInfo.managedObjects = ExHelper.GetProperty(directoryEntry, ExProperties.managedObjects);
        //        excUserInfo.ipPhone = ExHelper.GetProperty(directoryEntry, ExProperties.ipPhone);
        //        excUserInfo.objectCategory = ExHelper.GetProperty(directoryEntry, ExProperties.objectCategory);
        //        excUserInfo.mail = ExHelper.GetProperty(directoryEntry, ExProperties.mail);
        //        excUserInfo.homePhone = ExHelper.GetProperty(directoryEntry, ExProperties.homePhone);
        //        excUserInfo.mobile = ExHelper.GetProperty(directoryEntry, ExProperties.mobile);
        //        excUserInfo.pager = ExHelper.GetProperty(directoryEntry, ExProperties.pager);
        //       // excUserInfo.nTSecurityDescriptor = ExHelper.GetProperty(directoryEntry, ExProperties.nTSecurityDescriptor);
        //        excUserInfo.MaxStorage = ExHelper.GetProperty(directoryEntry, ExProperties.MaxStorage);
        //       // excUserInfo.MaxStorage = ExHelper.GetProperty(directoryEntry, ExProperties.MaxStorage);

        //        excUserInfo.msExchHSName = ExHelper.GetProperty(directoryEntry, ExProperties.msExchHSName);
        //       // excUserInfo.msExchMailboxSecurity = ExHelper.GetProperty(directoryEntry, ExProperties.msExchMailboxSecurity);
        //        excUserInfo.networkAddress = ExHelper.GetProperty(directoryEntry, ExProperties.networkAddress);
        //        excUserInfo.msExchHomeServerName = ExHelper.GetProperty(directoryEntry, ExProperties.msExchHSName);
        //        //excUserInfo.msExchMailboxSecurityDescriptor = ExHelper.GetProperty(directoryEntry, ExProperties.msExchMailboxSecurity);
        //        excUserInfo.msExchMailboxGuid = ExHelper.GetProperty(directoryEntry, ExProperties.msExchMailboxGuid);
        //        excUserInfo.msExchMobileMailboxFlags = ExHelper.GetProperty(directoryEntry, ExProperties.msExchMobileMailboxFlags);
        //        excUserInfo.msExchPoliciesIncluded = ExHelper.GetProperty(directoryEntry, ExProperties.msExchPoliciesIncluded);
        //        excUserInfo.msExchRecipientDisplayType = ExHelper.GetProperty(directoryEntry, ExProperties.msExchRecipientDisplayType);
        //        excUserInfo.msExchRecipientTypeDetails = ExHelper.GetProperty(directoryEntry, ExProperties.msExchRecipientTypeDetails);
        //        excUserInfo.msExchUserAccountControl = ExHelper.GetProperty(directoryEntry, ExProperties.msExchUserAccountControl);
        //        excUserInfo.msExchUserCulture = ExHelper.GetProperty(directoryEntry, ExProperties.msExchUserCulture);
        //        excUserInfo.msExchVersion = ExHelper.GetProperty(directoryEntry, ExProperties.msExchVersion);
        //       // excUserInfo.RequireSenderAuthen = ExHelper.GetProperty(directoryEntry, ExProperties.RequireSenderAuthen);

        //        excUserInfo.CreatedBy = PROCESSNAME;
        //        excUserInfo.CreatedDate = DateTime.Now;
        //        excUserInfo.LastModifiedBy = PROCESSNAME;
        //        excUserInfo.LastModifiedDate = DateTime.Now;


        //    }
        //    catch (Exception ex)
        //    {

        //        //NOTE: Dont throw the exception. Keep running all objects
        //        Log.General.WriteMessage(LogLevel.ERROR,
        //                       string.Format("Error occured in accessing the path {0}",
        //                       directoryEntry.Path), ex);


        //        return null;

        //    }



        //    return excUserInfo;

        //}

        public ExUserInfo PopulateExchangeUser(PSObject psMemberInfo)
        {
            if (null == psMemberInfo)
                return null;


            IList<ExUserInfo> list = new List<ExUserInfo>();

            ExUserInfo excUserInfo = new ExUserInfo();

            try
            {

                excUserInfo.EndPointSystemId = EndpointSystemId;
                excUserInfo.cn = ExHelper.GetProperty(psMemberInfo, ExProperties.name);
                excUserInfo.sn = ExHelper.GetProperty(psMemberInfo, ExProperties.name);
                excUserInfo.st = ExHelper.GetProperty(psMemberInfo, ExProperties.st);
                excUserInfo.title = ExHelper.GetProperty(psMemberInfo, ExProperties.title);
                excUserInfo.description = ExHelper.GetProperty(psMemberInfo, ExProperties.description);
                excUserInfo.postalAddress = ExHelper.GetProperty(psMemberInfo, ExProperties.postalAddress);
                excUserInfo.postalCode = ExHelper.GetProperty(psMemberInfo, ExProperties.postalCode);
                excUserInfo.postOfficeBox = ExHelper.GetProperty(psMemberInfo, ExProperties.postOfficeBox);
                excUserInfo.physicalDeliveryOfficeName = ExHelper.GetProperty(psMemberInfo, ExProperties.physicalDeliveryOfficeName);
                excUserInfo.telephoneNumber = ExHelper.GetProperty(psMemberInfo, ExProperties.telephoneNumber);
                excUserInfo.facsimileTelephoneNumber = ExHelper.GetProperty(psMemberInfo, ExProperties.facsimileTelephoneNumber);
                excUserInfo.givenName = ExHelper.GetProperty(psMemberInfo, ExProperties.name);
                excUserInfo.initials = ExHelper.GetProperty(psMemberInfo, ExProperties.initials);
                excUserInfo.distinguishedName = ExHelper.GetProperty(psMemberInfo, ExProperties.distinguishedName);
                excUserInfo.instanceType = ExHelper.GetProperty(psMemberInfo, ExProperties.instanceType);
                excUserInfo.whenCreated = ExHelper.GetProperty_DateTime(psMemberInfo, ExProperties.whenCreated);
                excUserInfo.whenChanged = ExHelper.GetProperty_DateTime(psMemberInfo, ExProperties.whenChanged);
                // excUserInfo.displayName = ExHelper.GetProperty(psMemberInfo, ExProperties.displayName);
                //excUserInfo.uSNCreated = ExHelper.GetCOMProperty_Int64(psMemberInfo, ExProperties.uSNCreated);
                excUserInfo.info = ExHelper.GetProperty(psMemberInfo, ExProperties.info);
                //excUserInfo.uSNChanged = ExHelper.GetCOMProperty_Int64(psMemberInfo, ExProperties.uSNChanged);
                excUserInfo.co = ExHelper.GetProperty(psMemberInfo, ExProperties.co);
                //excUserInfo.department = ExHelper.GetProperty(psMemberInfo, ExProperties.department);
                // excUserInfo.company = ExHelper.GetProperty(psMemberInfo, ExProperties.company);
                //// excUserInfo.manager = ExHelper.GetProperty(psMemberInfo, ExProperties.manager);
                excUserInfo.streetAddress = ExHelper.GetProperty(psMemberInfo, ExProperties.streetAddress);
                excUserInfo.wWWHomePage = ExHelper.GetProperty(psMemberInfo, ExProperties.wWWHomePage);
                excUserInfo.name = ExHelper.GetProperty(psMemberInfo, ExProperties.name);
                excUserInfo.objectGUID = ExHelper.GetProperty_GUID(psMemberInfo, ExProperties.objectGUID);
                excUserInfo.userAccountControl = ExHelper.GetProperty(psMemberInfo, ExProperties.userAccountControl);
                excUserInfo.sAMAccountName = ExHelper.GetProperty(psMemberInfo, ExProperties.sAMAccountName);

                //gk
                //excUserInfo.IsAccountDisabled = ExAccount.IsAccountDisabled(psMemberInfo);
                //excUserInfo.IsAccountLocked = ExAccount.IsAccountLocked(psMemberInfo);
                // excUserInfo.IsPasswordRequired = ExAccount.IsPasswordRequired(psMemberInfo);
                //excUserInfo.PasswordExpirationDate = ExHelper.GetCOMProperty_DateTime(psMemberInfo, ExProperties.badPasswordTime);
                //excUserInfo.badPwdCount = ExHelper.GetProperty(psMemberInfo, ExProperties.badPwdCount);
                //excUserInfo.codePage = ExHelper.GetProperty(psMemberInfo, ExProperties.codePage);//gk
                //excUserInfo.countryCode = ExHelper.GetProperty(psMemberInfo, ExProperties.countryCode);
                //excUserInfo.badPasswordTime = ExHelper.GetCOMProperty_DateTime(psMemberInfo, ExProperties.badPasswordTime);
                //excUserInfo.lastLogoff = ExHelper.GetCOMProperty_DateTime(psMemberInfo, ExProperties.lastLogoff);
                //excUserInfo.lastLogon = ExHelper.GetCOMProperty_DateTime(psMemberInfo, ExProperties.lastLogon);
                //excUserInfo.pwdLastSet = ExHelper.GetCOMProperty_DateTime(psMemberInfo, ExProperties.pwdLastSet);
                //excUserInfo.primaryGroupID = ExHelper.GetProperty(psMemberInfo, ExProperties.primaryGroupID);//gk
                //excUserInfo.objectSid = ExHelper.GetProperty_SID(psMemberInfo, ExProperties.objectSid);
                //excUserInfo.accountExpires = ExAccountHelper.AccountExpiresOn(psMemberInfo);
                //excUserInfo.logonCount = ExHelper.GetProperty(psMemberInfo, ExProperties.logonCount);

                //excUserInfo.sAMAccountType = ExHelper.GetProperty(psMemberInfo, ExProperties.sAMAccountType);
                //excUserInfo.managedObjects = ExHelper.GetProperty(psMemberInfo, ExProperties.managedObjects);
                //excUserInfo.ipPhone = ExHelper.GetProperty(psMemberInfo, ExProperties.ipPhone);
                //excUserInfo.objectCategory = ExHelper.GetProperty(psMemberInfo, ExProperties.objectCategory);
                //excUserInfo.mail = ExHelper.GetProperty(psMemberInfo, ExProperties.mail);
                //excUserInfo.homePhone = ExHelper.GetProperty(psMemberInfo, ExProperties.homePhone);
                //excUserInfo.mobile = ExHelper.GetProperty(psMemberInfo, ExProperties.mobile);
                //excUserInfo.pager = ExHelper.GetProperty(psMemberInfo, ExProperties.pager);
                //// excUserInfo.nTSecurityDescriptor = ExHelper.GetProperty(psMemberInfo, ExProperties.nTSecurityDescriptor);
                //excUserInfo.MaxStorage = ExHelper.GetProperty(psMemberInfo, ExProperties.MaxStorage);
                //// excUserInfo.MaxStorage = ExHelper.GetProperty(psMemberInfo, ExProperties.MaxStorage);

                //excUserInfo.msExchHSName = ExHelper.GetProperty(psMemberInfo, ExProperties.msExchHSName);
                //// excUserInfo.msExchMailboxSecurity = ExHelper.GetProperty(psMemberInfo, ExProperties.msExchMailboxSecurity);
                //excUserInfo.networkAddress = ExHelper.GetProperty(psMemberInfo, ExProperties.networkAddress);
                //excUserInfo.msExchHomeServerName = ExHelper.GetProperty(psMemberInfo, ExProperties.msExchHSName);
                ////excUserInfo.msExchMailboxSecurityDescriptor = ExHelper.GetProperty(psMemberInfo, ExProperties.msExchMailboxSecurity);
                //excUserInfo.msExchMailboxGuid = ExHelper.GetProperty(psMemberInfo, ExProperties.msExchMailboxGuid);
                //excUserInfo.msExchMobileMailboxFlags = ExHelper.GetProperty(psMemberInfo, ExProperties.msExchMobileMailboxFlags);
                //excUserInfo.msExchPoliciesIncluded = ExHelper.GetProperty(psMemberInfo, ExProperties.msExchPoliciesIncluded);
                //excUserInfo.msExchRecipientDisplayType = ExHelper.GetProperty(psMemberInfo, ExProperties.msExchRecipientDisplayType);
                //excUserInfo.msExchRecipientTypeDetails = ExHelper.GetProperty(psMemberInfo, ExProperties.msExchRecipientTypeDetails);
                //excUserInfo.msExchUserAccountControl = ExHelper.GetProperty(psMemberInfo, ExProperties.msExchUserAccountControl);
                //excUserInfo.msExchUserCulture = ExHelper.GetProperty(psMemberInfo, ExProperties.msExchUserCulture);
                //excUserInfo.msExchVersion = ExHelper.GetProperty(psMemberInfo, ExProperties.msExchVersion);
                //// excUserInfo.RequireSenderAuthen = ExHelper.GetProperty(psMemberInfo, ExProperties.RequireSenderAuthen);

                #region Newly Added Attributes powershell

                excUserInfo.IsSecurityPrincipal = Convert.ToBoolean(ExHelper.GetProperty(psMemberInfo, ExProperties.IsSecurityPrincipal));
                excUserInfo.Sid = ExHelper.GetProperty(psMemberInfo, ExProperties.Sid);
                excUserInfo.UserPrincipalName = ExHelper.GetProperty(psMemberInfo, ExProperties.UserPrincipalName);
                excUserInfo.ResetPasswordOnNextLogon = Convert.ToBoolean(ExHelper.GetProperty(psMemberInfo, ExProperties.ResetPasswordOnNextLogon));
                excUserInfo.RemotePowerShellEnabled = Convert.ToBoolean(ExHelper.GetProperty(psMemberInfo, ExProperties.RemotePowerShellEnabled));
                excUserInfo.WindowsLiveID = ExHelper.GetProperty(psMemberInfo, ExProperties.WindowsLiveID);
                //   excUserInfo.NetID = ExHelper.GetProperty(psMemberInfo, ExProperties.NetID);
                //  excUserInfo.UserAccountControl = ExHelper.GetProperty(psMemberInfo, ExProperties.UserAccountControl);
                excUserInfo.OrganizationalUnit = ExHelper.GetProperty(psMemberInfo, ExProperties.OrganizationalUnit);
                excUserInfo.IsLinked = Convert.ToBoolean(ExHelper.GetProperty(psMemberInfo, ExProperties.IsLinked));
                excUserInfo.LinkedMasterAccount = ExHelper.GetProperty(psMemberInfo, ExProperties.LinkedMasterAccount);
                excUserInfo.company = ExHelper.GetProperty(psMemberInfo, ExProperties.company);
                //excUserInfo.CountryOrRegion
                excUserInfo.department = ExHelper.GetProperty(psMemberInfo, ExProperties.department);
                excUserInfo.displayName = ExHelper.GetProperty(psMemberInfo, ExProperties.name);

                excUserInfo.FirstName = ExHelper.GetProperty(psMemberInfo, ExProperties.name);

                excUserInfo.LastName = ExHelper.GetProperty(psMemberInfo, ExProperties.name);
                //excUserInfo.manager = ExHelper.GetProperty(psMemberInfo, ExProperties.manager);

                #endregion

            }
            catch (Exception ex)
            {               
                //NOTE: Dont throw the exception. Keep running all objects
                Logger.DILog.DiscoveryLogger.WriteLog(null, Logger.LogLevelType.Exception,
                    string.Format("Error occured in accessing the path {0}", ExHelper.GetProperty(psMemberInfo, ExProperties.sAMAccountName))
                               , ex);
                return null;
            }

            return excUserInfo;

        }

    }
}
