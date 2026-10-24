
using DI.Chakra.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector
{
    public class UserInfoOrchestrator
    {
        public Office365UserInfo PopulateUser(PSObject psMemberInfo, long endpointId)
        {
            Office365UserInfo office365UserInfo = new Office365UserInfo();

            try
            {
                office365UserInfo.EndPointSystemId = endpointId;
                office365UserInfo.DisplayName = Office365Helper.GetProperty(psMemberInfo, Office365Properties.DisplayName);
                office365UserInfo.City = Office365Helper.GetProperty(psMemberInfo, Office365Properties.City);
                office365UserInfo.Company = Office365Helper.GetProperty(psMemberInfo, Office365Properties.Company);
                office365UserInfo.ConsumerNetID = Office365Helper.GetProperty(psMemberInfo, Office365Properties.ConsumerNetID);
                office365UserInfo.CountryOrRegion = Office365Helper.GetProperty(psMemberInfo, Office365Properties.CountryOrRegion);
                office365UserInfo.Department = Office365Helper.GetProperty(psMemberInfo, Office365Properties.Department);
                office365UserInfo.DisplayName = Office365Helper.GetProperty(psMemberInfo, Office365Properties.DisplayName);
                office365UserInfo.DistinguishedName = Office365Helper.GetProperty(psMemberInfo, Office365Properties.DistinguishedName);
                office365UserInfo.ExchangeVersion = Office365Helper.GetProperty(psMemberInfo, Office365Properties.ExchangeVersion);
                office365UserInfo.Fax = Office365Helper.GetProperty(psMemberInfo, Office365Properties.Fax);
                office365UserInfo.FirstName = Office365Helper.GetProperty(psMemberInfo, Office365Properties.FirstName);
                office365UserInfo.HomePhone = Office365Helper.GetProperty(psMemberInfo, Office365Properties.HomePhone);
                office365UserInfo.Identity = Office365Helper.GetProperty(psMemberInfo, Office365Properties.Identity);
                office365UserInfo.IsLinked = Office365Helper.GetProperty_Boolean(psMemberInfo, "IsLinked");
                office365UserInfo.IsSecurityPrincipal = Office365Helper.GetProperty_Boolean(psMemberInfo, "IsSecurityPrincipal"); 
                office365UserInfo.IsValid = Office365Helper.GetProperty_Boolean(psMemberInfo, "IsValid");
                office365UserInfo.LastName = Office365Helper.GetProperty(psMemberInfo, Office365Properties.LastName);
                office365UserInfo.LinkedMasterAccount = Office365Helper.GetProperty(psMemberInfo, Office365Properties.LinkedMasterAccount);
                office365UserInfo.Manager = Office365Helper.GetProperty(psMemberInfo, Office365Properties.Manager);
                office365UserInfo.MicrosoftOnlineServicesID = Office365Helper.GetProperty(psMemberInfo, Office365Properties.MicrosoftOnlineServicesID);
                office365UserInfo.MobilePhone = Office365Helper.GetProperty(psMemberInfo, Office365Properties.MobilePhone);
                office365UserInfo.Name = Office365Helper.GetProperty(psMemberInfo, Office365Properties.Name);
                office365UserInfo.NetID = Office365Helper.GetProperty(psMemberInfo, Office365Properties.NetID);
                office365UserInfo.Office = Office365Helper.GetProperty(psMemberInfo, Office365Properties.Office);
                office365UserInfo.OrganizationalUnit = Office365Helper.GetProperty(psMemberInfo, Office365Properties.OrganizationalUnit);
                office365UserInfo.OrganizationId = Office365Helper.GetProperty(psMemberInfo, Office365Properties.OrganizationId);
                office365UserInfo.Phone = Office365Helper.GetProperty(psMemberInfo, Office365Properties.Phone);
                office365UserInfo.SamAccountName = Office365Helper.GetProperty(psMemberInfo, Office365Properties.SamAccountName);
                office365UserInfo.Sid = Office365Helper.GetProperty(psMemberInfo, Office365Properties.Sid);
                office365UserInfo.SimpleDisplayName = Office365Helper.GetProperty(psMemberInfo, Office365Properties.SimpleDisplayName);
                office365UserInfo.SKUAssigned = Office365Helper.GetProperty(psMemberInfo, Office365Properties.SKUAssigned);
                office365UserInfo.Title = Office365Helper.GetProperty(psMemberInfo, Office365Properties.Title);
                office365UserInfo.UserAccountControl = Office365Helper.GetProperty(psMemberInfo, Office365Properties.UserAccountControl);
                office365UserInfo.UserPrincipalName = Office365Helper.GetProperty(psMemberInfo, Office365Properties.UserPrincipalName);
                office365UserInfo.WhenCreated = Office365Helper.GetProperty_DateTime(psMemberInfo, Office365Properties.WhenCreated);
                office365UserInfo.WindowsEmailAddress = Office365Helper.GetProperty(psMemberInfo, Office365Properties.WindowsEmailAddress);
                office365UserInfo.WindowsLiveID = Office365Helper.GetProperty(psMemberInfo, Office365Properties.WindowsLiveID);

            }
            catch (Exception)
            {
                //Dont Throw ,keep running the objects.
            }

            return office365UserInfo;

        }
    }



    public class NPACQueryHelper
    {
        internal const string ConnectionStringFormatter = "Data Source={0}; Initial Catalog={1}; Persist Security Info= True; {2} Connection Timeout=30";
        //internal const string ConnectionStringFormatter = "Data Source={0}; Initial Catalog={1}; Persist Security Info= True; Integrated Security=SSPI; Connection Timeout=30";
        internal const string SelectUserQuery = "SELECT * FROM USERS";

        internal const string SelectUserByUserLoginQuery = @"SELECT * FROM USERS WHERE userid = @username";
        internal const string updateUserQuery = @"UPDATE USERS SET phone = @phone, username = @username WHERE userid = @userid";
        internal const string updateUserIdentityQuery = @"UPDATE USERS SET userid = @userid WHERE userid = @olduserid";
        internal const string ReOrgUserQuery = @"UPDATE USERS SET section = @section,designation=@designation WHERE userid = @userid";

        internal const string GetEncryptPasswordQuery = @"SELECT [dbo].[f_encrypt] (@encrypt_pwd)";

        internal const string InsertUserQuery = @"INSERT INTO USERS ([emp_id], [userid],[username],[passwds],[designation],[phone],[pager],[fax],[groups],[section],[grade],[zone],[create_date],[encrypt_pwd],[invalid_date])
                                                        VALUES (@emp_id, @userid, @username, @passwds, @designation, @phone, @pager, @fax, @groups, @section, @grade, @zone, @create_date, @encrypt_pwd, @invalid_date)";
        internal const string InsertUserLevelQuery = @"UPDATE USERS SET levels = @levels WHERE userid = @username";

        internal const string DeleteUserLevelQuery = @"UPDATE USERS SET levels = @levels WHERE userid = @username AND levels = @currentlevel";
        internal const string RemoveUserLevelQuery = @"UPDATE USERS SET levels = @levels WHERE userid = @username";
        internal const string DeleteUserQuery = @"DELETE USERS WHERE userid = @username";

        internal const string LockUserQuery = @"UPDATE USERS SET invalid_date = @invalid_date, levels = @levels, section = @section WHERE userid = @username";
        internal const string UnLockUserQuery = @"UPDATE USERS SET invalid_date = @invalid_date WHERE userid = @username";

        internal const string DisableUserQuery = @"UPDATE USERS SET levels = @levels, section = @section WHERE userid = @username";
        internal const string EnableUserQuery = @"UPDATE USERS SET invalid_date = @invalid_date WHERE userid = @username";

        //internal const string SelectMaxUserIdQuery = "SELECT MAX(userid) FROM USERS";

        internal const string CheckUserLoginExistQuery = @"SELECT * FROM USERS WHERE userid = @username";
        internal const string ChangePasswordQuery = @"UPDATE USERS SET [encrypt_pwd] = @encrypt_pwd WHERE userid = @username";

        internal const string ExtendUserQuery = @"UPDATE USERS SET invalid_date = @invalid_date WHERE userid = @username";

    }
}

