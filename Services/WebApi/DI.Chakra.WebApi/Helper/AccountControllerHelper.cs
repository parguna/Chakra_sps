using ChakraService.Models;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Facade;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using DI.Chakra.ServiceHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChakraService
{
    public class AccountControllerHelper
    {
        #region Members
        private UserAccountFacade UAFacade = new UserAccountFacade();
        DiConfigurationFacade diConfigFacade = new DiConfigurationFacade();
        EndPointFacade endPointFacade = new EndPointFacade();
        PasswordManagementFacade pmFacade = new PasswordManagementFacade();
        PasswordPolicy objPasswordPolicy = new PasswordPolicy();
        UserFacade userFacade = new UserFacade();
        #endregion

        public ProcessResponse getSuccessResponse()
        {
            return new ProcessResponse()
            {
                Status = "Success",
                Message = "Success"
            };
        }
        public ProcessResponse getFailureResponse(string msg)
        {
            return new ProcessResponse()
            {
                Status = "Failure",
                Message = msg
            };
        }

        public ProcessResponse getSuccessResponse(string msg)
        {
            return new ProcessResponse()
            {
                Status = "Success",
                Message = msg
            };
        }

        public IList<EndpointDTO> getEndpointList(string UserName)
        {
            User mUser = null; 
            string mMessage="";
            bool bReturn=false;
            IList<EndpointDTO> endPointDTO = new List<EndpointDTO>();
            try
            {
                mUser = ValidUser(out mMessage, out bReturn, UserName);
                if (bReturn)
                {

                    var endPointsList = endPointFacade.GetEndpointsByDiUserid(mUser.Id);
                    foreach (EndpointInformation endPoint in endPointsList)
                    {
                        EndpointDTO endpointdto = new EndpointDTO();
                        endpointdto.Id = (int)endPoint.Endpointsystemid;
                        endpointdto.Name = endPoint.EndpointsystemType;
                        endpointdto.Description = endPoint.Endpointsystemdescription;
                        endPointDTO.Add(endpointdto);
                    }
                }
               
                    return endPointDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }

          
        }
        public EndpointListResponse getFailureEndpointListResponse(string msg)
        {
            return new EndpointListResponse()
            {
                ProcessResponse = getFailureResponse(msg),
                EndpointList = null
            };
        }
        public List<ChangePasswordResponse> getSuccessChangePasswordResponse()
        {
            return new List<ChangePasswordResponse>()
            {
                new ChangePasswordResponse() { EndpointId = 1, ProcessResponse = getSuccessResponse() },
                new ChangePasswordResponse() { EndpointId = 2, ProcessResponse = getSuccessResponse() },
                new ChangePasswordResponse() { EndpointId = 3, ProcessResponse = getSuccessResponse() }
            };
        }
        public List<ChangePasswordResponse> getFailureChangePasswordResponse(string msg)
        {
            return new List<ChangePasswordResponse>()
            {
                new ChangePasswordResponse() { EndpointId = 1, ProcessResponse = getFailureResponse("Password change is failed") },
                new ChangePasswordResponse() { EndpointId = 2, ProcessResponse = getFailureResponse("Password history violation") },
                new ChangePasswordResponse() { EndpointId = 3, ProcessResponse = getFailureResponse("Endpoint is not connected") }
            };
        }


        public List<ChangePasswordResponse> ChangePasword(ChangePasswordRequest request)
        {
            bool bReturn = false;
            string mMessage=String.Empty;
            User mUser = null;
            List<ChangePasswordResponse> chngePwdRpnse = new List<ChangePasswordResponse>();
            try
            {
                mUser = ValidUser(out mMessage, out bReturn, request.UserName);
                
                if (bReturn)
                {
                    foreach (int ePid in request.EndpointIds)
                    {
                        UserAccount response = InitiatePasswordChange(request.UserName, request.NewPassword, request.OldPassword, ePid, mUser.Id);
                        //chngePwdRpnse.Add(new ChangePasswordResponse { EndpointId = ePid, ProcessResponse = new ProcessResponse{ }); });
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return chngePwdRpnse;
        }


        public IList<EntitlementResponse> GetEntitlementResponse(int UserId)
        {
            List<EntitlementResponse> Returnvalue = new List<EntitlementResponse>();
            var UserAccount = UAFacade.GetUserAccountDetail(UserId);
            UserAccount.ToList().ForEach(lq =>
                    {
                        Returnvalue.Add(new EntitlementResponse() {EndpointSystem = lq.EndpointSystemName, EndpointUserID = lq.EndpointUserId, Name = lq.UserName, Type = "Account" });
                        var Account = UAFacade.GetEntitlementDetail(lq.EndpointUserId, lq.EndPointSystemType);
                        Account.ToList().ForEach(l =>
                            {
                                Returnvalue.Add(new EntitlementResponse() {EndpointSystem = lq.EndpointSystemName, EndpointUserID = lq.EndpointUserId, Name = l.Name, Type = l.Category });
                            });
                    }
                );
            return Returnvalue;
        }


        public PasswordPolicyDTO getPasswordPolicy(int EndPointId)
        {
            PasswordManagementFacade passwordFac = new PasswordManagementFacade();
            PasswordPolicy passwordPolicy = passwordFac.FindAllPoliciesByEndpointSystemId(EndPointId);
            PasswordPolicyDTO passwordPolicyDTO = new PasswordPolicyDTO();

            passwordPolicyDTO.Name = passwordPolicy!=null?passwordPolicy.Name:"";
            passwordPolicyDTO.Value = passwordPolicy!=null?passwordPolicy.Description:"";
            return passwordPolicyDTO;
        }
        
        public UserDTO getUserInfo(string userName)
        {
            bool rReturn = false;
            string rMessage = string.Empty;
            User mUser = null;
            mUser = ValidUser(out rMessage, out rReturn, userName);
            if (rReturn)
                return new UserDTO() { Name = mUser.UserName, DisplayName = mUser.DisplayName, Id = mUser.Id, MobileNumber = mUser.MobileNumber };
            else
                return null;
        }

        public bool IsAuthenticated(string userName,string Password,out string AuthMessage)
        {
            if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(Password))
            {
                string mMessage;
                bool IsAuth = false;
                User muser=null;
                IsAuth = IsValid(out mMessage, userName, Password, out muser);
                AuthMessage = mMessage;
                return IsAuth;
            }
            else
            {
                AuthMessage = "Credentials should not be empty";
                return false;
            }
        }

        public bool ValidateNumber(out string mMessage, string UserName, string mobileNumber)
        {
            bool bReturn = false;
            User mUser = null;
            string OTP=string.Empty;
            try
            {
               mUser=ValidUser(out mMessage,out bReturn,UserName);
                if(bReturn)
                {
                    if (mUser.MobileNumber == mobileNumber)
                    {
                      
                        OTP = GenerateRandomNumber();
                        mUser.Reserved1 = OTP;
                        mUser.LastModifiedBy = "Validate Mobille number";
                        mUser.LastModifiedDate = DateTime.Now;
                        userFacade.UpdateUser(mUser);
                        SMSService.Send(mUser.MobileNumber, string.Format("Please enter the following OTP to reset your Active Directory Password {0}", OTP));
                        mMessage = "Mobile Number verified. Sent OTP";
                    }

                    else
                    {
                        bReturn = false;
                        mMessage = "Mobile Number mismatch or not exist";
                    }
                }

                return bReturn;
            }
            catch (Exception ex)
            {
                // DILog.SchedulerLogger.WriteLog(LogLevelType.Error, "Error Occured while Getting the Get User By Name", ex);

                throw ex;
            }

        }

        public bool ValidateOTPPasswordChange(ValidateOTPRequest request)
        {
            bool bReturn = false;
            User mUser = null;
            string mMessage = string.Empty;
           
            try
            {
                mUser = ValidUser(out mMessage, out bReturn, request.UserName);
                if (bReturn)
                {
                    if (mUser.Reserved1 == request.OTP)
                    {
                        mMessage = "OTP validated";
                        ResetPassword(out mMessage,request.UserName,request.Password);
                    }
                  
                }
                

                

            }
            catch (Exception ex)
            {

            }

            return bReturn;
        }
             
        public bool IsValid(out string mMessage, string UserName, string Password,out User mUser)
        {
           bool bReturn = true;
           mUser = null;
            try
            {

                mMessage = string.Empty;

                mUser = userFacade.GetUserByName(UserName);

                if (mUser == null)
                {
                    mMessage = "User Name not found";
                    bReturn = false;
                    return bReturn;
                }

                if (mUser.IsActive.HasValue && mUser.IsActive.Value == false)
                {
                    mMessage = "User is NOT active";
                    bReturn = false;
                    return bReturn;
                }
                if (string.Compare(mUser.UserName, Constants.IACMADMIN, true) == 0)
                {
                    if (string.Compare(mUser.Password, Password, false) == -1)
                    {
                        mMessage = "User Name or Password is wrong";
                        bReturn = false;
                        return bReturn;
                    }

                }
                else
                {
                    AuthenticationMode authenticationMode = (AuthenticationMode)Enum.Parse(typeof(AuthenticationMode), MasterConfigHelper.AuthenticationMode);

                    if (authenticationMode == AuthenticationMode.IACM)
                    {

                        if (string.Compare(mUser.Password, Password, false) == -1)
                        {
                            mMessage = "User Name or Password is wrong";
                            bReturn = false;
                            return bReturn;
                        }

                    }

                    if (authenticationMode == AuthenticationMode.AD)
                    {

                        var serviceHelper= ServiceHelperFactory.GetPasswordResetServiceHelper;

                        bReturn = serviceHelper.AuthenticateADUser(UserName, Password);


                        if (!serviceHelper.AuthenticateADUser(UserName, Password))
                        {
                            mMessage = "Authentication failed.  Please check user name or password.";
                            bReturn = false;
                            return bReturn;
                        }

                    }

                    if (authenticationMode == AuthenticationMode.MixedMode)
                    {
                        if (string.Compare(mUser.AuthenticationMode, "AD", true) == 0)
                        {
                            var serviceHelper = ServiceHelperFactory.GetPasswordResetServiceHelper;
                            if (!serviceHelper.AuthenticateADUser(UserName, Password))
                            {
                                mMessage = "Authentication failed.  Please check user name or password.";
                                bReturn = false;
                                return bReturn;
                            }
                        }
                        else
                        {

                            if (string.Compare(mUser.Password, Password, false) == -1)
                            {
                                mMessage = "User Name or Password is wrong";
                                bReturn = false;
                                return bReturn;
                            }
                        }
                    }
                }
              

                //SessionStatusImpl session = new SessionStatusImpl();
               // session.User = mUser;
                return bReturn;
            }
            catch (Exception ex)
            {
               // DILog.SchedulerLogger.WriteLog(LogLevelType.Error, "Error Occured while Getting the Get User By Name", ex);

                throw ex;
            }
        }
        
        public User ValidUser(out string rMessage,out bool rReturn,string UserName)
        {
            rReturn = true;
            User mUser = null;

            rMessage = string.Empty;

            mUser = userFacade.GetUserByName(UserName);

            if (mUser == null)
            {
                rMessage = "User Name not found";
                rReturn = false;
                
            }

            if (mUser.IsActive.HasValue && mUser.IsActive.Value == false)
            {
                rMessage = "User is NOT active";
                rReturn = false;
               
            }

            return mUser;
        }

        public void UpdateUser(UserDTO User)
        {
            try
            {
                userFacade.UpdateUser(new User { Id = User.Id, UserName = User.Name, MobileNumber = User.MobileNumber, DisplayName = User.DisplayName });
            }
            catch (Exception ex)
            {

            }
        }

        public List<UserConfigResponse> getUserConfigResponse()
        {
            List<UserConfigResponse> configResponse = new List<UserConfigResponse>();
            List<DI.Chakra.DomainModel.ConfigGeneralTrustedSource> trustedSource = DI.Chakra.DomainModel.ConfigGeneralTrustedSource.GetAll();
               
            foreach (DI.Chakra.DomainModel.ConfigGeneralTrustedSource trustSource in trustedSource)
            {
                UserConfigResponse response = new UserConfigResponse();
                response.FieldName = trustSource.AttributeName;
                response.SelfUpdate = trustSource.SelfUpdate;
                configResponse.Add(response);
            }

            return configResponse;

        }




        //UserID= AccountReconcilation UserID ,epId= Accounreconcilation EndpointSystemId,
        #region Password change     
        public UserAccount InitiatePasswordChange(string accountName, string newPassword, string oldPassword, int epId, long userId)// , int epUserId,
        {
            string message = string.Empty;

            var serviceHelper = ServiceHelperFactory.GetPasswordResetServiceHelper;

            //UserAccount userAc = UAFacade.GetUserAccountDetail(userId).Where(t => t.EndpointSystemId == epId &&
            //    t.EndpointUserId == epUserId).FirstOrDefault();
            //&&
            //t.EndpointUserId == epUserId).FirstOrDefault();
            UserAccount userAc = UAFacade.GetUserAccountDetail(userId).Where(t => t.EndpointSystemId == epId).FirstOrDefault();
            
            bool bRet = IsPasswordComplaint(accountName, newPassword, out message, epId,userId);

            if (bRet == false)
            {
                userAc.Reserved1 = "Failed";
                userAc.Reserved2 = message;
                return userAc;
            }

            bRet = serviceHelper.ChangePassword(accountName, newPassword, oldPassword, userId, epId);

            if (bRet == false)
            {
                userAc.Reserved1 = "Failed";
                userAc.Reserved2 = "Password reset failed";
                return userAc;
            }

            userAc.Reserved1 = "Success";
            userAc.Reserved2 = "Password reset completed";
            return userAc;

        }


        /// <summary>
        /// Rewritten big block of code
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        /// 

        public bool IsPasswordComplaint(string UserName, string Password, out string Message, long EndPointId, long userId)
        {
            EndPoint Endpoint = endPointFacade.GetEndPointSystemByKey(EndPointId);

            string errMsg = string.Empty;
            int isFalse = 0;

            objPasswordPolicy = pmFacade.FindAllPoliciesByEndpointSystemId(EndPointId);

            //PASSWORD  LENGTH

            if (objPasswordPolicy.PasswordLength != null && objPasswordPolicy.PasswordLength != 0)
            {
                string pattern = @"^.{" + objPasswordPolicy.PasswordLength + "}$";
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Password, pattern);
                if (match.Success)
                {
                }
                else
                {
                    errMsg = errMsg + "<br>" + string.Format("Password Should Contain " + objPasswordPolicy.PasswordLength + " characters");
                    isFalse++;
                }
            }


            //MINIMUM AND MAXIMUM LENGTH OF PASSWORD.

            if (objPasswordPolicy.PasswordMinimumLength != null && objPasswordPolicy.PasswordMinimumLength != 0 && objPasswordPolicy.PasswordMaximumLength != null && objPasswordPolicy.PasswordMaximumLength != 0)
            {
                string pattern = @"^.{" + objPasswordPolicy.PasswordMinimumLength + "," + objPasswordPolicy.PasswordMaximumLength + "}$";
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Password, pattern);
                if (match.Success)
                {

                }
                else
                {
                    errMsg = errMsg + "<br>" + string.Format("Password should contain minimum " + objPasswordPolicy.PasswordMinimumLength + " characters and Maximum " + objPasswordPolicy.PasswordMaximumLength + " Characters. ");
                    isFalse++;

                }
            }

            //MINIMUM  LENGTH OF PASSWORD.

            if (objPasswordPolicy.PasswordMinimumLength != null && objPasswordPolicy.PasswordMinimumLength != 0 && (objPasswordPolicy.PasswordMaximumLength == null || objPasswordPolicy.PasswordMaximumLength == 0))
            {
                string pattern = @"^.{" + objPasswordPolicy.PasswordMinimumLength + ",}$";
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Password, pattern);
                if (match.Success)
                {

                }
                else
                {
                    errMsg = errMsg + "<br>" + string.Format("Password should contain minimum " + objPasswordPolicy.PasswordMinimumLength + " characters. ");
                    isFalse++;

                }
            }

            //MAXIMUM  LENGTH OF PASSWORD.

            if (objPasswordPolicy.PasswordMaximumLength != null && objPasswordPolicy.PasswordMaximumLength != 0 && (objPasswordPolicy.PasswordMinimumLength == null || objPasswordPolicy.PasswordMinimumLength == 0))
            {
                string pattern = @"^.{0," + objPasswordPolicy.PasswordMaximumLength + "}$";
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Password, pattern);
                if (match.Success)
                {

                }
                else
                {
                    errMsg = errMsg + "<br>" + string.Format("Password should contain maximum " + objPasswordPolicy.PasswordMaximumLength + " characters. ");
                    isFalse++;

                }
            }

            //CHECK  NUMBER OF  SPECIAL CHARACTERS.

            if (objPasswordPolicy.NoOfSpecialCharacters != null)
            {
                int count = 0;
                foreach (char c in Password)
                {
                    if (!char.IsLetterOrDigit(c.ToString(), 0))
                    {
                        count++;

                    }
                }

                if (count >= objPasswordPolicy.NoOfSpecialCharacters)
                {

                }
                else if (count < objPasswordPolicy.NoOfSpecialCharacters)
                {
                    errMsg = errMsg + "<br>" + string.Format("Must  contain  atleast " + objPasswordPolicy.NoOfSpecialCharacters + " Special Characters.");
                    isFalse++;
                }
            }

            //CHECK  IF  SPECIAL CHARACTERS ARE NOT ALLOWED.

            if (objPasswordPolicy.SpecialCharNotAllowed == true)
            {
                string pattern = @"^[a-zA-Z0-9]*$"; //return false when special characters are found.
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Password, pattern);
                if (match.Success)
                {

                }
                else
                {
                    errMsg = errMsg + "<br>" + string.Format("Special Characters Are Not Allowed.");
                    isFalse++;
                }

            }

            //CHECK  IF NUMERICS ARE NOT ALLOWED.

            if (objPasswordPolicy.NumericCharacterNotAllowed == true)
            {
                string pattern = @"^[^..0-9]+$";//return true when NO numeric is found in the entire string
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Password, pattern);
                if (match.Success)
                {

                }
                else
                {
                    errMsg = errMsg + "<br>" + string.Format("Numerics Are Not Allowed.");
                    isFalse++;

                }

            }

            //CHECK  IF ALPHABETS  ARE NOT ALLOWED.

            if (objPasswordPolicy.AlphabetsNotAllowed == true)
            {
                string pattern = @"^[^..a-zA-Z]+$"; // return true when NO alphabet found in entire string
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Password, pattern);
                if (match.Success)
                {

                }
                else
                {
                    errMsg = errMsg + "<br>" + string.Format("Alphabets Are Not Allowed.");
                    isFalse++;

                }

            }

            //CHECK NUMBER OF UC ALPHABETS
            if (objPasswordPolicy.NoOfUpperCaseAlphabets != null && objPasswordPolicy.NoOfUpperCaseAlphabets != 0)
            {
                string pattern = @"(?=(.*[A-Z]){" + objPasswordPolicy.NoOfUpperCaseAlphabets + "})"; //return true when number of Alphabets  found.
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Password, pattern);
                if (match.Success)
                {

                }
                else
                {
                    errMsg = errMsg + "<br>" + string.Format("Must Containt atleast " + objPasswordPolicy.NoOfUpperCaseAlphabets + " upper case Alphabets.");
                    isFalse++;

                }
            }

            //CHECK NUMBER OF LC ALPHABETS
            if (objPasswordPolicy.NoOfLowerCaseAlphabets != null && objPasswordPolicy.NoOfLowerCaseAlphabets != 0)
            {
                string pattern = @"(?=(.*[a-z]){" + objPasswordPolicy.NoOfLowerCaseAlphabets + "})"; //return true when number of Alphabets  found.
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Password, pattern);
                if (match.Success)
                {

                }
                else
                {
                    errMsg = errMsg + "<br>" + string.Format("Must Containt atleast " + objPasswordPolicy.NoOfLowerCaseAlphabets + "lower case Alphabets.");
                    isFalse++;

                }
            }

            //CHECK NUMBER OF NUMERIC
            if (objPasswordPolicy.NoOfNumericCharacters != null && objPasswordPolicy.NoOfNumericCharacters != 0)
            {
                string pattern = @"(?=(.*[0-9]){" + objPasswordPolicy.NoOfNumericCharacters + "})"; //return true when number of NUMERICS  found.
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Password, pattern);
                if (match.Success)
                {

                }
                else
                {
                    errMsg = errMsg + "<br>" + string.Format("Must Containt atleast " + objPasswordPolicy.NoOfNumericCharacters + "  Numerics.");
                    isFalse++;

                }

            }

            //CHECK NOT ALLOWED SPEACIAL CHARACTERS.
            if (objPasswordPolicy.NotAllowedCharacters != null && objPasswordPolicy.NotAllowedCharacters != String.Empty)
            {
                string pattern = @"^[^'" + objPasswordPolicy.NotAllowedCharacters + "']+$"; // return TRUE when No match found (Special Char Specified)
                string patternn = string.Format("^[^{0}]+$", objPasswordPolicy.NotAllowedCharacters);
                //
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Password, pattern);
                if (match.Success)
                {

                }
                else
                {
                    errMsg = errMsg + "<br>" + string.Format("Special Characters '" + objPasswordPolicy.NotAllowedCharacters + "' Are Not Allowed in Password.");
                    isFalse++;

                }
            }

           // CHECK PASSWORD CONTAINS USERNAME.

            if (UserName != null && UserName != String.Empty)
            {
                if (Password.ToUpper().Contains(UserName.ToUpper()))
                {
                    errMsg = errMsg + "<br>" + string.Format("Password should not have User Name.");
                    isFalse++;
                }
                else
                {


                }
            }

            //CHECK PASSWORD CONTAINS KEYWORDS.

            if (objPasswordPolicy.NotAllowedKeywords != null && objPasswordPolicy.NotAllowedKeywords != String.Empty)
            {
                string[] words = objPasswordPolicy.NotAllowedKeywords.Split(',');
                foreach (string word in words)
                {
                    if (Password.ToUpper().Contains(word.ToUpper()))
                    {
                        errMsg = errMsg + "<br>" + string.Format("Password should not have Specific KeyWords {0}", word);
                        isFalse++;
                    }
                    else
                    {

                    }
                }


            }

            //CHECK PASSWORD Change Number Of Days.

            if (objPasswordPolicy.NoOfDaysPasswordChangeAllowed != null)
            {
                //GET IACMLast Password change Date by Username
                PasswordManagementFacade passwordManagementFacade = new PasswordManagementFacade();
                string IACMLastPasswordChange = string.Empty;
                IACMLastPasswordChange = passwordManagementFacade.GetIACMLastPasswordChangeByEndpointUser(Endpoint.Id, UserName); // endPointFacade.

                if (IACMLastPasswordChange != null && IACMLastPasswordChange != string.Empty)
                {
                    DateTime PasswordChangedDate = Convert.ToDateTime(IACMLastPasswordChange);
                    DateTime currentDate = DateTime.Now;
                    TimeSpan span = currentDate.Subtract(PasswordChangedDate);
                    int days = span.Days;

                    DateTime NextPwdChange = PasswordChangedDate.AddDays((double)objPasswordPolicy.NoOfDaysPasswordChangeAllowed);
                    if (days < objPasswordPolicy.NoOfDaysPasswordChangeAllowed)
                    {
                        //“The previous WINDOWS DOMAIN SYSTEM password reset was done on 10/07/2013.  As per the policy the next password reset is possible only on 11/07/2013”.  Please unselect the WINDOWS DOMAIN SYSTEM to continue with the password sync.

                        //errMsg = errMsg + "<br>" + string.Format("*.Password Change not Allowed in {0}  Days.", objPasswordPolicy.NoOfDaysPasswordChangeAllowed);
                        errMsg = errMsg + "<br>" + string.Format("The previous {0}  password reset was done on  {1}.As per the policy the next password reset is possible only after {2}.Please unselect the {0} to continue with the password sync.", Endpoint.DisplayName, PasswordChangedDate, NextPwdChange);
                        isFalse++;
                    }
                    else
                    {

                    }
                }
            }

            Message = errMsg;
            if (isFalse > 0)
            {
                return false;
            }
            else
                return true;
        }

        public bool ResetPassword(out string mMessage,string userName,string password)
        {
            bool bReturn = false;
            mMessage = string.Empty;
            User mUser=null;
            try
            {
                bReturn = IsValid(out mMessage, userName, password, out mUser);

                if (!bReturn)
                {
                    mMessage = "User Name not found";
                    return false;
                }

                if (mUser.IsActive.HasValue && mUser.IsActive.Value == false)
                {
                    mMessage = "User is NOT active";
                    return false;
                }

                if (string.Compare(mUser.Password , Constants.IACMADMIN, true) == 0)
                {
                    bReturn = localAuth(mUser.Password,password);

                    if (bReturn == false)
                        mMessage = "User Name or Password is wrong";

                }

                AuthenticationMode authenticationMode = (AuthenticationMode)Enum.Parse(typeof(AuthenticationMode), MasterConfigHelper.AuthenticationMode);

                if (authenticationMode == AuthenticationMode.IACM)
                {
                    bReturn = ResetLocalPassword(mUser,password);

                    if (bReturn == false)
                        mMessage = "Error occured while doing password reset";

                }

                if (authenticationMode == AuthenticationMode.AD)
                {

                    bReturn = ResetADPassword(out mMessage, userName, password, mUser);

                }

                if (authenticationMode == AuthenticationMode.MixedMode)
                {
                    if (string.Compare(mUser.AuthenticationMode, "AD", true) == 0)
                    {
                        bReturn = ResetADPassword(out mMessage,userName,password,mUser);
                    }
                    else
                    {
                        bReturn = ResetLocalPassword(mUser, password);

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


        private bool ResetLocalPassword(User user, string Password)
        {
            try
            {
                UserFacade userFacade = new UserFacade();

                user.Password = RSAHelper.EncryptData(Password);

                userFacade.UpdateUser(user);
            }
            catch (Exception)
            {
                return false;
            }

            return true;

        }

        private bool ResetADPassword(out string outMessage,string userName,string password, User user)
        {
            outMessage = string.Empty;

            try
            {
                ADSUserInfo adsUser = new ActiveDirectoryFacade().GetADSUserInfoByName(userName);
                if (adsUser != null)
                {
                    bool Success = IsPasswordComplaint(userName, password, out outMessage,adsUser.EndPointSystemId, user.Id);
                    if (Success)
                    {
                        var serviceHelper = ServiceHelperFactory.GetPasswordResetServiceHelper;
                        return serviceHelper.ResetPassword(userName, password, user.Id, 0);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool localAuth(string userPasswod,string password)
        {
            return string.Compare(userPasswod, password, false) == 0;
        }

        #endregion

        private string GenerateRandomNumber()
        {
            Random rnd = new Random();
            int myRandomNo = rnd.Next(100000, 999999);
            return Convert.ToString(myRandomNo);
        }



      

        public IList<PasswordExpireResponse> GetPwdExpiryUsers()
        {
            List<PasswordExpireResponse> Returnvalue = new List<PasswordExpireResponse>();
            var PasswordExpiredUser = UAFacade.GetPwdExpiryUsers();
            PasswordExpiredUser.ToList().ForEach(lq =>
            {
                Returnvalue.Add(new PasswordExpireResponse() { DisplayName = lq.DisplayName, MobileNumber = lq.PhoneNo, Accounts = new AccountInfo() { AccountName = lq.LoginName, EndpointSystemType = lq.ApplicationType, Endpoint = lq.Application } });
            });
            return Returnvalue;
        }

    }




}