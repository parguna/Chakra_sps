using System;
using System.Collections.Generic;
using System.Web;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
namespace Application
{
    [Serializable]
    public class SessionStatusImpl
    {
        private const string KEY_USER_ID = @"UserId"; 
        private const string KEY_USER = @"User";
        private const string KEY_LogedInUserABACInfo = @"LogedInUserABACInfo";
        private const string KEY_LogedInUserAccessRightInfo = @"LogedInUserAccessRightInfo";
        private const string KEY_ISADMINGROUP = @"isAdminGroup";
        private const string KEY_ISIACMADMINGROUP = @"isIACMAdminGroup";

        private const string KEY_NOOFATTEMPTS = @"NoofAttempts";
        private const string KEY_ISHISTORY = @"isHistory";
        private const string KEY_ISMANAGER = @"isManager";
        private const string KEY_ENDPOINTTYPE = @"EndpointType";
        private const string KEY_PROVFORMENDPOINTTYPEID = @"ProvFormEndpointTypeId";
        private const string KEY_PROVFORMENDPOINTID = @"ProvFormEndpointId";
        private const string KEY_INHSYSTEMENDPOINTID = @"InhSysEndpointId";
        private const string KEY_ORGANISATIONSCOPEID = @"ORGANISATIONSCOPEID";
        private const string KEY_ATTESTATIONENDPOINTTYPEIDS = @"AttestationEndpointTypeIds";
        private const string KEY_REDIRECTFROM = @"RedirectFrom";
        private const string KEY_REQUESTFORMEPID = @"RequestFormEPId";

        private const string KEY_ENDPOINTSYSTEMID = @"KEY_ENDPOINTSYSTEMID";
        private const string KEY_ENDPOINTUSERID = @"KEY_ENDPOINTUSERID";

        private const string KEY_RETURN_ERROR_MESSAGE = @"KEY_RETURN_ERROR_MESSAGE";

        private const string KEY_DATEFORMAT = @"KEY_DATEFORMAT";
        private const string KEY_TIMEFORMAT = @"KEY_TIMEFORMAT";
        private const string KEY_FORGOTPWDTYPE = @"KEY_FORGOTPWDTYPE";

        const string KEY_ACCOUNTNAVIGATORHELPER = @"KEY_ACCOUNTNAVIGATORHELPER";
        const string KEY_ENTITLEMENTMANAGEMENTNAVIGATORHELPER = @"KEY_ENTITLEMENTMANAGEMENTNAVIGATORHELPER";
        const string KEY_ENDPOINTNEEDSPLACCOUNT = @"KEY_ENDPOINTNEEDSPLACCOUNT";
        #region SessionStatusImpl Members



        public long UserId
        {
            get
            {
                if (User == null)
                    return -1;

                return User.Id;
            }
            //set
            //{
            //    HttpContext.Current.Session[KEY_USER_ID] = value;
            //}
        }

        public int NoofAttempts
        {
            get
            {
                if (HttpContext.Current.Session[KEY_NOOFATTEMPTS] == null)
                    return -1;
                return Convert.ToInt32(HttpContext.Current.Session[KEY_NOOFATTEMPTS]);
            }
            set
            {
                HttpContext.Current.Session[KEY_NOOFATTEMPTS] = value;
            }
        }

        public long? RequestFormEPId
        {
            get
            {
                if (HttpContext.Current.Session[KEY_REQUESTFORMEPID] == null)
                    return null;
                return Convert.ToInt64(HttpContext.Current.Session[KEY_REQUESTFORMEPID]);
            }
            set
            {
                HttpContext.Current.Session[KEY_REQUESTFORMEPID] = value;
            }
        }

        public AccountNavigatorHelper AccountNavigatorHelper
        {
            get
            {
                if (HttpContext.Current.Session[KEY_ACCOUNTNAVIGATORHELPER] == null)
                    return null;
                return (AccountNavigatorHelper)HttpContext.Current.Session[KEY_ACCOUNTNAVIGATORHELPER];
            }
            set
            {
                HttpContext.Current.Session[KEY_ACCOUNTNAVIGATORHELPER] = value;
            }
        }

        public EntitlementmanagementNavigatorHelper EntitlementmanagementNavigatorHelper
        {
            get
            {
                if (HttpContext.Current.Session[KEY_ENTITLEMENTMANAGEMENTNAVIGATORHELPER] == null)
                    return null;
                return (EntitlementmanagementNavigatorHelper)HttpContext.Current.Session[KEY_ENTITLEMENTMANAGEMENTNAVIGATORHELPER];
            }
            set
            {
                HttpContext.Current.Session[KEY_ENTITLEMENTMANAGEMENTNAVIGATORHELPER] = value;
            }
        }

        public string EndpointNeedsSplAccount
        {
            get
            {
                if (HttpContext.Current.Session[KEY_ENDPOINTNEEDSPLACCOUNT] == null)
                    return string.Empty;
                return (string)HttpContext.Current.Session[KEY_ENDPOINTNEEDSPLACCOUNT];
            }
            set
            {
                HttpContext.Current.Session[KEY_ENDPOINTNEEDSPLACCOUNT] = value;
            }
        }

        public User User
        {
            get
            {
                if (HttpContext.Current == null || HttpContext.Current.Session == null || HttpContext.Current.Session[KEY_USER] == null)
                    return null;
                return (User)HttpContext.Current.Session[KEY_USER];
            }
            set
            {
                HttpContext.Current.Session[KEY_USER] = value;
            }
        }

        public string RedirectFrom
        {
            get
            {
                if (HttpContext.Current.Session[KEY_REDIRECTFROM] == null)
                    return "";
                return (string)HttpContext.Current.Session[KEY_REDIRECTFROM];
            }
            set
            {
                HttpContext.Current.Session[KEY_REDIRECTFROM] = value;
            }
        }

        

        public bool HasLogon()
        {
            return (UserId != -1) && (User != null);
        }

        public string EndpointType
        {
            get
            {
                if (HttpContext.Current.Session[KEY_ENDPOINTTYPE] == null)
                    return null;
                return HttpContext.Current.Session[KEY_ENDPOINTTYPE].ToString();
            }
            set
            {
                HttpContext.Current.Session[KEY_ENDPOINTTYPE] = value;
            }
        }

        public long ProvisioningFormEndPointTypeId
        {
            get
            {
                if (HttpContext.Current.Session[KEY_PROVFORMENDPOINTTYPEID] == null)
                    return -1;
                return Convert.ToInt64(HttpContext.Current.Session[KEY_PROVFORMENDPOINTTYPEID]);
            }
            set
            {
                HttpContext.Current.Session[KEY_PROVFORMENDPOINTTYPEID] = value;
            }
        }

        public long ProvisioningFormEndPointId
        {
            get
            {
                if (HttpContext.Current.Session[KEY_PROVFORMENDPOINTID] == null)
                    return -1;
                return Convert.ToInt64(HttpContext.Current.Session[KEY_PROVFORMENDPOINTID]);
            }
            set
            {
                HttpContext.Current.Session[KEY_PROVFORMENDPOINTID] = value;
            }
        }

        public long InHouseSystemEndPointId
        {
            get
            {
                if (HttpContext.Current.Session[KEY_INHSYSTEMENDPOINTID] == null)
                    return -1;
                return Convert.ToInt64(HttpContext.Current.Session[KEY_INHSYSTEMENDPOINTID]);
            }
            set
            {
                HttpContext.Current.Session[KEY_INHSYSTEMENDPOINTID] = value;
            }
        }

        public string OrganisationScopeId 
        {
            get
            {
                if (HttpContext.Current.Session[KEY_ORGANISATIONSCOPEID] == null)
                    return "0";
                return Convert.ToString(HttpContext.Current.Session[KEY_ORGANISATIONSCOPEID]);
            }
            set
            {
                HttpContext.Current.Session[KEY_ORGANISATIONSCOPEID] = value;
            }
        }

        public List<long> AttestationEndPointTypeIds
        {
            get
            {
                if (HttpContext.Current.Session[KEY_ATTESTATIONENDPOINTTYPEIDS] == null)
                    return new List<long>();//{ (int)DI.Chakra.Infrastructure.Enums.EndpointType.ActiveDirectory }; // { -1 };Remoing default value from Active Directory
                return (HttpContext.Current.Session[KEY_ATTESTATIONENDPOINTTYPEIDS]) as List<long>;
            }
            set
            {
                HttpContext.Current.Session[KEY_ATTESTATIONENDPOINTTYPEIDS] = value;
            }
        }

        #region LogedIn User ABAC




        #endregion

        #endregion

        #region SessionStatusImpl Members

        public bool isAdminGroup
        {
            get
            {
                if (HttpContext.Current.Session[KEY_ISADMINGROUP] == null)
                    return false;
                return (bool)HttpContext.Current.Session[KEY_ISADMINGROUP];
            }
            set
            {
                HttpContext.Current.Session[KEY_ISADMINGROUP] = value;
            }
        }

        public bool isIACMAdminGroup
        {
            get
            {
                if (HttpContext.Current.Session[KEY_ISIACMADMINGROUP] == null)
                    return false;
                return (bool)HttpContext.Current.Session[KEY_ISIACMADMINGROUP];
            }
            set
            {
                HttpContext.Current.Session[KEY_ISIACMADMINGROUP] = value;
            }
        }

        public bool IsHistory
        {
            get
            {
                if (HttpContext.Current.Session[KEY_ISHISTORY] == null)
                    return false;
                return (bool)HttpContext.Current.Session[KEY_ISHISTORY];
            }
            set
            {
                HttpContext.Current.Session[KEY_ISHISTORY] = value;
            }
        }

        public bool IsManager
        {
            get
            {
                if (HttpContext.Current.Session[KEY_ISMANAGER] == null)
                    return false;
                return (bool)HttpContext.Current.Session[KEY_ISMANAGER];
            }
            set
            {
                HttpContext.Current.Session[KEY_ISMANAGER] = value;
            }
        }

        #endregion

        public long EndpointSystemId
        {
            get
            {
                if (HttpContext.Current.Session[KEY_ENDPOINTSYSTEMID] == null)
                    return -1;
                return (long)HttpContext.Current.Session[KEY_ENDPOINTSYSTEMID];
            }
            set
            {
                HttpContext.Current.Session[KEY_ENDPOINTSYSTEMID] = value;
            }
        }

        public long EndpointUserId
        {
            get
            {
                if (HttpContext.Current.Session[KEY_ENDPOINTUSERID] == null)
                    return -1;
                return (long)HttpContext.Current.Session[KEY_ENDPOINTUSERID];
            }
            set
            {
                HttpContext.Current.Session[KEY_ENDPOINTUSERID] = value;
            }
        }

        public string ReturnErrorMessage
        {
            get
            {
                if (HttpContext.Current.Session[KEY_RETURN_ERROR_MESSAGE] == null)
                    return string.Empty;
                return (string)HttpContext.Current.Session[KEY_RETURN_ERROR_MESSAGE];
            }
            set
            {
                HttpContext.Current.Session[KEY_RETURN_ERROR_MESSAGE] = value;
            }
        }

        public string DateFormat
        {
            get
            {
                if (HttpContext.Current.Session[KEY_DATEFORMAT] == null)
                    return null;
                return HttpContext.Current.Session[KEY_DATEFORMAT].ToString();
            }
            set
            {
                HttpContext.Current.Session[KEY_DATEFORMAT] = value;
            }
        }

        public string TimeFormat
        {
            get
            {
                if (HttpContext.Current.Session[KEY_TIMEFORMAT] == null)
                    return null;
                return HttpContext.Current.Session[KEY_TIMEFORMAT].ToString();
            }
            set
            {
                HttpContext.Current.Session[KEY_TIMEFORMAT] = value;
            }
        }
        public ForgotPwdType ForgotPwdType
        {
            get
            {
                if (HttpContext.Current.Session[KEY_FORGOTPWDTYPE] == null)
                    return ForgotPwdType.ForgotPassword;

                return (ForgotPwdType)Enum.Parse(typeof(ForgotPwdType), HttpContext.Current.Session[KEY_FORGOTPWDTYPE].ToString());
            }
            set
            {
                HttpContext.Current.Session[KEY_FORGOTPWDTYPE] = value;
            }
        }
    }
}
    
