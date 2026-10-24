using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DI.Chakra.Connector
{
    public class ExProperties
    {

        public const string cn = "cn";
        public const string sn = "sn";
        public const string c = "c";
        public const string l = "l";
        public const string st = "st";
        public const string title = "title";
        public const string description = "description";
        public const string postalAddress = "postalAddress";
        public const string postalCode = "postalCode";
        public const string postOfficeBox = "postOfficeBox";
        public const string physicalDeliveryOfficeName = "physicalDeliveryOfficeName";
        public const string telephoneNumber = "telephoneNumber";
        public const string facsimileTelephoneNumber = "facsimileTelephoneNumber";
        public const string givenName = "givenName";
        public const string initials = "initials";
        public const string distinguishedName = "distinguishedName";
        public const string instanceType = "instanceType";
        public const string whenCreated = "whenCreated";
        public const string whenChanged = "whenChanged";
        public const string displayName = "displayName";
        public const string uSNCreated = "uSNCreated";
        public const string info = "info";
        public const string uSNChanged = "uSNChanged";
        public const string co = "co";
        public const string department = "department";
        public const string company = "company";
        public const string manager = "manager";
        public const string streetAddress = "streetAddress";
        public const string wWWHomePage = "wWWHomePage";
        public const string name = "name";
        public const string objectGUID = "objectGUID";
        public const string userAccountControl = "userAccountControl";
        public const string badPwdCount = "badPwdCount";
        public const string codePage = "codePage";
        public const string countryCode = "countryCode";
        public const string badPasswordTime = "badPasswordTime";
        public const string lastLogoff = "lastLogoff";
        public const string lastLogon = "lastLogon";
        public const string pwdLastSet = "pwdLastSet";
        public const string primaryGroupID = "primaryGroupID";
        public const string objectSid = "objectSid";
        public const string accountExpires = "accountExpires";
        public const string logonCount = "logonCount";
        public const string sAMAccountName = "sAMAccountName";
        public const string sAMAccountType = "sAMAccountType";
        public const string managedObjects = "managedObjects";
        public const string ipPhone = "ipPhone";
        public const string objectCategory = "objectCategory";
        public const string mail = "mail";
        public const string homePhone = "homePhone";
        public const string mobile = "mobile";
        public const string pager = "pager";
        public const string nTSecurityDescriptor = "nTSecurityDescriptor";
        //Exchange Attributes
        public const string MaxStorage = "maxStorage";
        public const string msExchHSName = "msExchHomeServerName";
        public const string msExchMailboxSecurity = "msExchMailboxSecurityDescriptor";
        public const string networkAddress = "networkAddress";
        public const string msExchMailboxGuid = "msExchMailboxGuid";
        public const string msExchMobileMailboxFlags = "msExchMobileMailboxFlags";
        public const string msExchPoliciesIncluded = "msExchPoliciesIncluded";
        public const string msExchRecipientDisplayType = "msExchRecipientDisplayType";
        public const string msExchRecipientTypeDetails = "msExchRecipientTypeDetails";
        public const string msExchUserAccountControl = "msExchUserAccountControl";
        public const string msExchUserCulture = "msExchUserCulture";
        public const string msExchVersion = "msExchVersion";
        public const string forwardingAddress = "forwardingAddress";
        //Completed Exchange Attributes
        //Group
        public const string member = "member";

        public const string groupType = "groupType";


        //OU
        public const string ou = "ou";
        public const string showInAdvancedViewOnly = "showInAdvancedViewOnly";
        public const string systemFlags = "systemFlags";
        public const string gPLink = "gPLink";
        public const string isCriticalSystemObject = "isCriticalSystemObject";

        //Computer
        public const string operatingSystem = "operatingSystem";
        public const string operatingSystemVersion = "operatingSystemVersion";
        public const string operatingSystemServicePack = "operatingSystemServicePack";
        public const string serverReferenceBL = "serverReferenceBL";
        public const string dNSHostName = "dNSHostName";
        public const string rIDSetReferences = "rIDSetReferences";
        public const string servicePrincipalName = "servicePrincipalName";
        public const string frsComputerReferenceBL = "frsComputerReferenceBL";
        public const string localPolicyFlags = "localPolicyFlags";

        //shared
        public const string uNCName = "uNCName";
        public const string managedBy = "managedBy";

        //Printer

        public const string versionNumber = "versionNumber";
        public const string location = "location";
        public const string serverName = "serverName";
        public const string portName = "portName";
        public const string driverName = "driverName";
        public const string priority = "priority";
        public const string printStartTime = "printStartTime";
        public const string printEndTime = "printEndTime";
        public const string printBinNames = "printBinNames";
        public const string printMaxResolutionSupported = "printMaxResolutionSupported";
        public const string printOrientationsSupported = "printOrientationsSupported";
        
        public const string printCollate = "printCollate";
        public const string printColor = "printColor";
        public const string printShareName = "printShareName";
        public const string printSpooling = "printSpooling";
        public const string printKeepPrintedJobs = "printKeepPrintedJobs";
        public const string driverVersion = "driverVersion	";
        public const string printMaxXExtent = "printMaxXExtent";
        public const string printMaxYExtent = "printMaxYExtent";
        public const string printMinXExtent = "printMinXExtent";
        public const string printMinYExtent = "printMinYExtent";
        public const string printStaplingSupported = "printStaplingSupported";
        public const string printMemory = "printMemory";
        public const string printRate = "printRate";
        public const string printerName = "printerName";
        public const string url = "url";
        public const string shortServerName = "shortServerName";
        public const string printDuplexSupported = "printDuplexSupported";


        #region New Attributes- Powershell

        public const string IsSecurityPrincipal = "IsSecurityPrincipal";
        public const string Sid = "Sid";
        public const string UserPrincipalName="UserPrincipalName";
        public const string ResetPasswordOnNextLogon="ResetPasswordOnNextLogon";
        public const string RemotePowerShellEnabled="RemotePowerShellEnabled";
        public const string WindowsLiveID="WindowsLiveID";
        public const string NetID="NetID";
        public const string UserAccountControl="UserAccountControl";
        public const string OrganizationalUnit="OrganizationalUnit";
        public const string IsLinked="IsLinked";
        public const string LinkedMasterAccount="LinkedMasterAccount";
        public const string CountryOrRegion="CountryOrRegion"; 
        public const string FirstName="FirstName";
        public const string Initials="Initials";
        public const string LastName="LastName";
        public const string WhenChanged = "WhenChanged";
        public const string WhenCreated = "WhenCreated";

        #endregion 


    }

}
