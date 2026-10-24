using Application;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Web;
using DI.Chakra;

namespace DI.Chakra.Web
{
    public class LoginHelper
    {
        private readonly string mUserName = string.Empty;
        private readonly string mPassword = string.Empty;
        private User mUser = null;
        public LoginHelper()
        { }
        public LoginHelper(string userName, string password)
        {
            mUserName = userName;
            mPassword = password;
        }
        public bool IsValid(bool createNewSessionAndAuthCookie, out string mMessage)
        {
            bool bReturn = false;

            try
            {

                mMessage = string.Empty;

                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("name", mUserName);

                Dto.Presentation.PresentationAnonymousUser anonymoususer = DomainApiClient<Dto.Presentation.PresentationAnonymousUser>.GetByAction<Dto.Presentation.PresentationAnonymousUser>(DomainApiAction.Endpoint.Presentation.PresentationAnonymousUser.GetUserByUserName, param);

                if (anonymoususer == null)
                {
                    mMessage = "Username or Password is wrong";
                    return false;
                }

                bool superUser = string.Compare(mUserName, Constants.IACMADMIN, true) == 0;

                if (!anonymoususer.IsActive.HasValue || anonymoususer.IsActive.Value == false)
                {
                    mMessage = "User is NOT active";
                    return false;
                }

                //if ((!superUser) && (!anonymoususer.FullTimeEmployee.HasValue || anonymoususer.FullTimeEmployee.Value == false))
                //{
                //    mMessage = "You are not authorised to access IACM!";
                //    return false;

                //}

                if (superUser && anonymoususer.AuthenticationMode != "AD")
                {
                    bReturn = localAuth();

                    if (bReturn == false)
                        mMessage = "User Name or Password is wrong";

                }
                else
                {
                    AuthenticationMode authenticationMode = (AuthenticationMode)Enum.Parse(typeof(AuthenticationMode), MasterConfigHelper.AuthenticationMode);

                    if (authenticationMode == AuthenticationMode.IACM)
                    {
                        bReturn = localAuth();

                        if (bReturn == false)
                            mMessage = "User Name or Password is wrong";

                    }
                    else if (authenticationMode == AuthenticationMode.AD)
                    {

                        LoginResult result = adAuth();

                        if (result != null)
                        {
                            if (result.Success == false)
                            {
                                mMessage = result.Message;
                            }
                            bReturn = result.Success;
                        }

                    }
                    else if (authenticationMode == AuthenticationMode.MixedMode)
                    {
                        if (string.Compare(anonymoususer.AuthenticationMode, "AD", true) == 0)
                        {
                            LoginResult result = adAuth();

                            if (result != null)
                            {
                                if (result.Success == false)
                                {
                                    mMessage = result.Message;
                                }
                                bReturn = result.Success;
                            }
                        }
                        else
                        {
                            bReturn = localAuth();

                            if (bReturn == false)
                                mMessage = "User Name or Password is wrong";
                        }
                    }
                }

                if (bReturn && createNewSessionAndAuthCookie)
                {
                    CreateUserSessionAndAuthendicaitonCookie();
                    //HttpContext.Current.Session["LoginUserName"] = mUserName;
                    //HttpContext.Current.Session["LoginPassword"] = mPassword;

                    //Dictionary<string, string> parameter = new Dictionary<string, string>();
                    //parameter.Add("name", mUserName);

                    //mUser = DomainApiClient<Dto.User>.GetByAction<User>(DomainApiAction.User.GetByUserName, parameter);

                    //SessionStatusImpl session = new SessionStatusImpl();
                    //session.User = mUser;
                }
                return bReturn;
            }
            catch (Exception ex)
            {
                // NeedToConfirm
                //DILog.SchedulerLogger.WriteLog(LogLevelType.Error, "Error Occured while Getting the Get User By Name", ex.Message, string.Empty, 0, 0, string.Empty);

                throw ex;
            }
            return true;
        }
        public bool ResetPassword(out string mMessage)
        {
            bool bReturn = false;
            mMessage = string.Empty;

            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("name", mUserName);

                mUser = DomainApiClient<Dto.User>.GetByAction<User>(DomainApiAction.User.ForgotUser, parameter);

                if (mUser == null)
                {
                    mMessage = "User Name not found";
                    return false;
                }

                if (!mUser.IsActive)
                {
                    mMessage = "User is NOT active";
                    return false;
                }

                if (string.Compare(mUser.UserName, Constants.IACMADMIN, true) == 0)
                {
                    bReturn = localAuth();

                    if (bReturn == false)
                        mMessage = "User Name or Password is wrong";

                }

                AuthenticationMode authenticationMode = (AuthenticationMode)Enum.Parse(typeof(AuthenticationMode), MasterConfigHelper.AuthenticationMode);

                if (authenticationMode == AuthenticationMode.IACM)
                {
                    bReturn = ResetLocalPassword();

                    if (bReturn == false)
                        mMessage = "Error occured while doing password reset";

                }

                if (authenticationMode == AuthenticationMode.AD)
                {

                    bReturn = ResetADPassword(out mMessage);

                    //if (bReturn == false)
                    //mMessage = "Error occured while doing password reset";

                }

                if (authenticationMode == AuthenticationMode.MixedMode)
                {
                    if (string.Compare(mUser.AuthenticationMode, "AD", true) == 0)
                    {
                        bReturn = ResetADPassword(out mMessage);

                        //if (bReturn == false)
                        // mMessage = "Error occured while doing password reset";
                    }
                    else
                    {
                        bReturn = ResetLocalPassword();

                        if (bReturn == false)
                            mMessage = "Error occured while doing password reset";
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return bReturn;
        }
        public bool ChangePassword(string newPassword, out string mMessage)
        {
            bool bReturn = false;
            mMessage = string.Empty;
            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("name", mUserName);
                mUser = DomainApiClient<Dto.User>.GetByAction<User>(DomainApiAction.User.ForgotUser, parameter);
                if (mUser == null)
                {
                    mMessage = "User Name not found";
                    return false;
                }
                if (!mUser.IsActive)
                {
                    mMessage = "User is NOT active";
                    return false;
                }
                AuthenticationMode authenticationMode = (AuthenticationMode)Enum.Parse(typeof(AuthenticationMode), MasterConfigHelper.AuthenticationMode);
                if (authenticationMode == AuthenticationMode.IACM)
                {
                    bReturn = localAuth();
                    if (bReturn == false)
                        mMessage = "User Name or Password is wrong";
                    bReturn = ResetLocalPassword();
                    if (bReturn == false)
                        mMessage = "Error occured while doing password reset";
                }
                if (authenticationMode == AuthenticationMode.AD)
                {
                    bReturn = ResetADPassword(out mMessage);
                }
                if (authenticationMode == AuthenticationMode.MixedMode)
                {
                    if (string.Compare(mUser.AuthenticationMode, "AD", true) == 0)
                    {
                        bReturn = ResetADPassword(out mMessage);
                    }
                    else
                    {
                        bReturn = localAuth();
                        if (bReturn == false)
                        {
                            mMessage = "Current Password is Wrong";
                        }
                        else
                        {
                            bReturn = ChangeLocalPassword(newPassword);
                            if (bReturn == false)
                                mMessage = "Error occured during Password Change";
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return bReturn;
        }
        private bool localAuth()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userName", mUserName);
            parameter.Add("password", mPassword);

            bool isValidUer = DomainApiClient<Dto.User>.GetByAction<Boolean>(DomainApiAction.User.Authenticate, parameter);

            if (isValidUer == true)
            {
                //
            }

            return isValidUer;
        }
        private LoginResult adAuth()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("configKey", ConfigConstants.AUTHENTICATIONDOMAIN);
            param.Add("product", Product.IACM.ToString());
            Dto.IConfigurationMaster config = DomainApiClient<Dto.ConfigurationMaster>.GetByAction<Dto.IConfigurationMaster>
                (DomainApiAction.Configuration.ConfigurationMaster.GetByConfigKey, param);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userName", mUserName);
            parameter.Add("password", mPassword);
            parameter.Add("domainId", config.ConfigValue);
            return DomainServiceClient.GetByAction<LoginResult>
                (DomainServiceAction.ActiveDirectoryService.RootName, DomainServiceAction.ActiveDirectoryService.AuthenticateADUser, parameter);
        }
        private bool ResetLocalPassword()
        {
            try
            {

                mUser.Password = RSAHelper.EncryptData(mPassword);
                DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.SaveUser, null, mUser);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private bool ChangeLocalPassword(string newPassword)
        {
            try
            {
                mUser.Password = RSAHelper.EncryptData(newPassword);
                DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.SaveUser, null, mUser);

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;

        }
        private bool ResetADPassword(out string outMessage)
        {
            outMessage = string.Empty;

            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("sAMAccountName", mUserName);
                ADSUserInfo adsUser = DomainApiClient<Dto.ADSUserInfo>.GetByAction<Dto.ADSUserInfo>
                    (DomainApiAction.Endpoint.ActiveDirectory.ADSUserInfo.GetBysAMAccountName, parameter);

                if (adsUser != null)
                {
                    PasswordPolicyValidator PasswordPolicyValidator = new PasswordPolicyValidator();
                    PasswordPolicyValidator.EndPointId = adsUser.EndPointSystemId;
                    bool Success = PasswordPolicyValidator.IsPasswordComplaint(mUserName, mPassword, out outMessage);

                    if (Success)
                    {
                        Dictionary<string, string> param = new Dictionary<string, string>();
                        param.Add("userName", mUserName);
                        param.Add("password", mPassword);
                        param.Add("userId", mUser.Id.ToString());
                        param.Add("endpointSystemId", adsUser.EndPointSystemId.ToString());
                        return DomainServiceClient.GetByAction<bool>
                            (DomainServiceAction.EndPointService.RootName, DomainServiceAction.EndPointService.ResetPassword, param);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public System.Globalization.CultureInfo setCulture(out string landingPageName)
        {
            landingPageName = "";
            SessionStatusImpl session = new SessionStatusImpl();

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", session.UserId.ToString());

            List<DI.Chakra.Dto.UserCulturePreference> usrCulPrefenece = DomainApiClient<Dto.UserCulturePreference>.GetByAction<List<Dto.UserCulturePreference>>(DomainApiAction.Preference.UserCulturePreference.GetByUserId, parameter);

            if (usrCulPrefenece.Count != 0)
            {
                if (usrCulPrefenece[0].Id != 0)
                {
                    if (!string.IsNullOrEmpty(usrCulPrefenece[0].LandingPageMenuName))
                    {
                        landingPageName = usrCulPrefenece[0].LandingPageMenuName;
                    }

                    System.Globalization.CultureInfo newCulture = (System.Globalization.CultureInfo)new System.Globalization.CultureInfo(usrCulPrefenece[0].CultureName).Clone();
                    newCulture.DateTimeFormat.ShortDatePattern = usrCulPrefenece[0].DateFormat;


                    newCulture.DateTimeFormat.LongTimePattern = string.Empty;
                    if (!string.IsNullOrEmpty(usrCulPrefenece[0].TimeFormat))
                    {
                        newCulture.DateTimeFormat.ShortTimePattern = usrCulPrefenece[0].TimeFormat;
                    }

                    else
                    {
                        newCulture.DateTimeFormat.ShortTimePattern = string.Empty;
                        newCulture.DateTimeFormat.LongTimePattern = string.Empty;
                    }
                    System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture;
                    return newCulture;
                }
            }
            return null;
        }

        public static void ClearUserSessionAndAuthendicationCookie()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Session.RemoveAll();

            if (HttpContext.Current.Request.Cookies["ASP.NET_SessionId"] != null)
            {
                HttpContext.Current.Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                HttpContext.Current.Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (HttpContext.Current.Request.Cookies["AuthToken"] != null)
            {
                HttpContext.Current.Response.Cookies["AuthToken"].Value = string.Empty;
                HttpContext.Current.Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }

        private void CreateUserSessionAndAuthendicaitonCookie()
        {
            HttpContext.Current.Session["LoginUserName"] = mUserName;
            HttpContext.Current.Session["LoginPassword"] = mPassword;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("name", mUserName);

            mUser = DomainApiClient<Dto.User>.GetByAction<User>(DomainApiAction.User.GetActiveByUserName, parameter);

            SessionStatusImpl session = new SessionStatusImpl();
            session.User = mUser;
            
            DomainApiClient<DI.Chakra.Dto.User>.GetByAction<object>(DomainApiAction.User.UpdateLastLogonInformation, null, mUser);

            parameter = new Dictionary<string, string>();
            parameter.Add("userId", mUser.Id.ToString());
            var isAdminUser = DomainApiClient<Dto.User>.GetByAction<bool>(DomainApiAction.User.IsAdminUser, parameter);
            session.isAdminGroup = isAdminUser;

            // createa a new GUID and save into the session
            string guid = Guid.NewGuid().ToString();
            HttpContext.Current.Session["AuthToken"] = guid;
            // now create a new cookie with this guid value
            HttpContext.Current.Response.Cookies.Add(new HttpCookie("AuthToken", guid));
        }

        public static void ValidateSessionAndAuthendicationCookie()
        {
            if (new SessionStatusImpl().User == null ||
                HttpContext.Current.Session["AuthToken"] == null || HttpContext.Current.Request.Cookies["AuthToken"] == null)
            {
                HttpContext.Current.Response.Redirect("~/Login.aspx");
                return;
            }

            if (HttpContext.Current.Session["AuthToken"].ToString() != HttpContext.Current.Request.Cookies["AuthToken"].Value)
            {
                HttpContext.Current.Response.Redirect("~/Login.aspx");
                return;
            }

        }

        public string IsADActiveAccount(string userName,string responsemessage)
        {
            string mMessage = string.Empty;
            if (responsemessage == "")
            {
                try
                {
                    Dictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("name", userName);

                    Dto.Presentation.PresentationAnonymousUser anonymoususer = DomainApiClient<Dto.Presentation.PresentationAnonymousUser>.GetByAction<Dto.Presentation.PresentationAnonymousUser>(DomainApiAction.Endpoint.Presentation.PresentationAnonymousUser.GetUserByUserName, param);

                    if (anonymoususer == null)
                    {
                        return "Username is wrong.";

                    }

                    bool superUser = string.Compare(userName, Constants.IACMADMIN, true) == 0;

                    if (!anonymoususer.IsActive.HasValue || anonymoususer.IsActive.Value == false)
                    {
                        return "User is NOT active";

                    }

                    //if ((!superUser) && (!anonymoususer.FullTimeEmployee.HasValue || anonymoususer.FullTimeEmployee.Value == false))
                    //{
                    //    mMessage = "You are not authorised to access IACM!";
                    //    return false;

                    //}

                    if (superUser && anonymoususer.AuthenticationMode != "AD")
                    {
                        string message = "Forgot Password is not applicable for AuthenticationMode" + anonymoususer.AuthenticationMode;
                        return message;
                            
                    }
                    else
                    {
                        AuthenticationMode authenticationMode = (AuthenticationMode)Enum.Parse(typeof(AuthenticationMode), MasterConfigHelper.AuthenticationMode);

                        if (authenticationMode == AuthenticationMode.AD)
                        {
                            Dictionary<string, string> parameter = new Dictionary<string, string>();
                            parameter.Add("userName", userName);
                            var result = DomainServiceClient.GetByAction<bool>
                                (DomainServiceAction.ActiveDirectoryService.RootName, "PasswordResetAccountVerification", parameter);

                            if (result == false)
                                return "AccountName does not exist or disabled in AD";

                            
                        }
                        else if (authenticationMode == AuthenticationMode.MixedMode)
                        {

                        }
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
            return mMessage;
        }
    }
}