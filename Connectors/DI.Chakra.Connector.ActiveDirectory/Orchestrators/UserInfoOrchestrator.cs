using DI.Chakra.DomainModel;
using DI.Chakra.Logger;
using System;
using System.DirectoryServices;

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
        public ADSUserInfo PopulateUser(DirectoryEntry directoryEntry, DirectorySearcher _dirSearcher)
        {
            if (null == directoryEntry)
                return null;

            ADSUserInfo adsUserInfo = new ADSUserInfo();

            try
            {
                adsUserInfo.EndPointSystemId = EndpointSystemId;
                adsUserInfo.cn = ADHelper.GetProperty(directoryEntry, ADProperties.cn);
                adsUserInfo.sn = ADHelper.GetProperty(directoryEntry, ADProperties.sn);
                adsUserInfo.st = ADHelper.GetProperty(directoryEntry, ADProperties.st);
                adsUserInfo.title = ADHelper.GetProperty(directoryEntry, ADProperties.title);
                adsUserInfo.description = ADHelper.GetProperty(directoryEntry, ADProperties.description);
                adsUserInfo.postalAddress = ADHelper.GetProperty(directoryEntry, ADProperties.postalAddress);
                adsUserInfo.postalCode = ADHelper.GetProperty(directoryEntry, ADProperties.postalCode);
                adsUserInfo.postOfficeBox = ADHelper.GetProperty(directoryEntry, ADProperties.postOfficeBox);
                adsUserInfo.physicalDeliveryOfficeName = ADHelper.GetProperty(directoryEntry, ADProperties.physicalDeliveryOfficeName);
                adsUserInfo.telephoneNumber = ADHelper.GetProperty(directoryEntry, ADProperties.telephoneNumber);
                adsUserInfo.facsimileTelephoneNumber = ADHelper.GetProperty(directoryEntry, ADProperties.facsimileTelephoneNumber);
                adsUserInfo.givenName = ADHelper.GetProperty(directoryEntry, ADProperties.givenName);
                adsUserInfo.initials = ADHelper.GetProperty(directoryEntry, ADProperties.initials);
                adsUserInfo.distinguishedName = ADHelper.GetProperty(directoryEntry, ADProperties.distinguishedName);
                adsUserInfo.instanceType = ADHelper.GetProperty(directoryEntry, ADProperties.instanceType);

                adsUserInfo.whenCreated = ADHelper.GetProperty_DateTime(directoryEntry, ADProperties.whenCreated);
                adsUserInfo.whenChanged = ADHelper.GetProperty_DateTime(directoryEntry, ADProperties.whenChanged);
                adsUserInfo.displayName = ADHelper.GetProperty(directoryEntry, ADProperties.displayName);
                adsUserInfo.uSNCreated = ADHelper.GetCOMProperty_Int64(directoryEntry, ADProperties.uSNCreated);

                adsUserInfo.info = ADHelper.GetProperty(directoryEntry, ADProperties.info);
                adsUserInfo.uSNChanged = ADHelper.GetCOMProperty_Int64(directoryEntry, ADProperties.uSNChanged);

                adsUserInfo.co = ADHelper.GetProperty(directoryEntry, ADProperties.co);
                adsUserInfo.department = ADHelper.GetProperty(directoryEntry, ADProperties.department);
                adsUserInfo.company = ADHelper.GetProperty(directoryEntry, ADProperties.company);
                adsUserInfo.manager = ADHelper.GetProperty(directoryEntry, ADProperties.manager);
                adsUserInfo.streetAddress = ADHelper.GetProperty(directoryEntry, ADProperties.streetAddress);
                adsUserInfo.wWWHomePage = ADHelper.GetProperty(directoryEntry, ADProperties.wWWHomePage);
                adsUserInfo.name = ADHelper.GetProperty(directoryEntry, ADProperties.name);
                adsUserInfo.objectGUID = ADHelper.GetProperty_GUID(directoryEntry, ADProperties.objectGUID);
                adsUserInfo.userAccountControl = ADHelper.GetProperty(directoryEntry, ADProperties.userAccountControl);

                adsUserInfo.IsAccountDisabled = ADAccount.IsAccountDisabled(directoryEntry);
                //adsUserInfo.IsLocked = ADAccount.IsAccountLocked(directoryEntry);
                adsUserInfo.IsPasswordRequired = ADAccount.IsPasswordRequired(directoryEntry);

                adsUserInfo.badPwdCount = ADHelper.GetProperty(directoryEntry, ADProperties.badPwdCount);
                adsUserInfo.codePage = ADHelper.GetProperty(directoryEntry, ADProperties.codePage);
                adsUserInfo.countryCode = ADHelper.GetProperty(directoryEntry, ADProperties.countryCode);
                adsUserInfo.badPasswordTime = ADHelper.GetCOMProperty_DateTime(directoryEntry, ADProperties.badPasswordTime);
                adsUserInfo.lastLogoff = ADHelper.GetCOMProperty_DateTime(directoryEntry, ADProperties.lastLogoff);
                adsUserInfo.lastLogon = ADHelper.GetCOMProperty_DateTime(directoryEntry, ADProperties.lastLogon);
                if (adsUserInfo.lastLogon == null || adsUserInfo.lastLogon.Value == DateTime.MinValue)
                {
                    adsUserInfo.lastLogon = ADHelper.GetCOMProperty_DateTime(directoryEntry, ADProperties.lastLogonTimestamp);
                }
                adsUserInfo.pwdLastSet = ADHelper.GetCOMProperty_DateTime(directoryEntry, ADProperties.pwdLastSet);
                adsUserInfo.primaryGroupID = ADHelper.GetProperty(directoryEntry, ADProperties.primaryGroupID);
                adsUserInfo.objectSid = ADHelper.GetProperty_SID(directoryEntry, ADProperties.objectSid);
                adsUserInfo.accountExpires = ADAccountHelper.AccountExpiresOn(directoryEntry);
                adsUserInfo.logonCount = ADHelper.GetProperty(directoryEntry, ADProperties.logonCount);
                adsUserInfo.sAMAccountName = ADHelper.GetProperty(directoryEntry, ADProperties.sAMAccountName);
                adsUserInfo.sAMAccountType = ADHelper.GetProperty(directoryEntry, ADProperties.sAMAccountType);
                adsUserInfo.managedObjects = ADHelper.GetProperty(directoryEntry, ADProperties.managedObjects);
                adsUserInfo.ipPhone = ADHelper.GetProperty(directoryEntry, ADProperties.ipPhone);
                adsUserInfo.objectCategory = ADHelper.GetProperty(directoryEntry, ADProperties.objectCategory);
                adsUserInfo.mail = ADHelper.GetProperty(directoryEntry, ADProperties.mail);
                adsUserInfo.homePhone = ADHelper.GetProperty(directoryEntry, ADProperties.homePhone);
                adsUserInfo.mobile = ADHelper.GetProperty(directoryEntry, ADProperties.mobile);
                adsUserInfo.pager = ADHelper.GetProperty(directoryEntry, ADProperties.pager);


                adsUserInfo.division = ADHelper.GetProperty(directoryEntry, ADProperties.division);
                adsUserInfo.businessCategory = ADHelper.GetProperty(directoryEntry, ADProperties.businessCategory);
                adsUserInfo.homeDrive = ADHelper.GetProperty(directoryEntry, ADProperties.homeDrive);
                adsUserInfo.homeDirectory = ADHelper.GetProperty(directoryEntry, ADProperties.homeDirectory);

                adsUserInfo.extensionAttribute1 = ADHelper.GetProperty(directoryEntry, ADProperties.extensionAttribute1);
                adsUserInfo.extensionAttribute2 = ADHelper.GetProperty(directoryEntry, ADProperties.extensionAttribute2);
                adsUserInfo.extensionAttribute3 = ADHelper.GetProperty(directoryEntry, ADProperties.extensionAttribute3);
                adsUserInfo.extensionAttribute4 = ADHelper.GetProperty(directoryEntry, ADProperties.extensionAttribute4);
                adsUserInfo.extensionAttribute5 = ADHelper.GetProperty(directoryEntry, ADProperties.extensionAttribute5);
                adsUserInfo.extensionAttribute6 = ADHelper.GetProperty(directoryEntry, ADProperties.extensionAttribute6);
                adsUserInfo.extensionAttribute7 = ADHelper.GetProperty(directoryEntry, ADProperties.extensionAttribute7);
                adsUserInfo.extensionAttribute8 = ADHelper.GetProperty(directoryEntry, ADProperties.extensionAttribute8);
                adsUserInfo.extensionAttribute9 = ADHelper.GetProperty(directoryEntry, ADProperties.extensionAttribute9);
                adsUserInfo.extensionAttribute10 = ADHelper.GetProperty(directoryEntry, ADProperties.extensionAttribute10);


                if (adsUserInfo.manager != string.Empty)
                {
                    _dirSearcher.Filter = String.Format("(distinguishedName={0})", adsUserInfo.manager);

                    _dirSearcher.PropertiesToLoad.Add("sAMAccountName");
                    SearchResult resultTemp = _dirSearcher.FindOne();

                    if (null != resultTemp)
                    {
                        string acManager = resultTemp.Properties["sAMAccountName"][0].ToString();

                        adsUserInfo.manager = acManager;
                    }
                }

            }
            catch (Exception ex)
            {
                //NOTE: Dont throw the exception. Keep running all objects
                DILog.DiscoveryLogger.WriteLog( LogLevelType.Exception,
                                 $"Error occured in populate User {ADHelper.GetProperty(directoryEntry, ADProperties.sn)}", ex);

                return null;

            }

            return adsUserInfo;

        }




    }
}
