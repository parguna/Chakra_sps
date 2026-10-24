#region NAMESPACE
using System;
using System.Collections.Generic;
using DI.Chakra.Dto;
using Application;
using DI.Chakra.Infrastructure;
#endregion

namespace DI.Chakra.Web
{
    public class PasswordPolicyValidator
    {
        SessionStatusImpl Session = new SessionStatusImpl();
        PasswordPolicy objPasswordPolicy = new PasswordPolicy();
        EndPoint endpoint = new EndPoint();
        IList<PasswordPolicy> listPasswordPolicy = new List<PasswordPolicy>();

        public long EndPointId { get; set; }

        public bool IsPasswordComplaint(string UserName, string Password, out string Message)
        {
            string errMsg = string.Empty;
            int isFalse = 0;
            if (this.EndPointId > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", EndPointId.ToString());
                endpoint = DomainApiClient<Dto.EndPoint>.GetByAction<EndPoint>
                                                            (DomainApiAction.Endpoint.EndPoint.GetByIdIncludingOwner, parameter);

                parameter = new Dictionary<string, string>();
                parameter.Add("endpointSystemId", this.EndPointId.ToString());
                objPasswordPolicy = DomainApiClient<PasswordPolicy>.GetByAction<PasswordPolicy> 
                                                  (DomainApiAction.Configuration.PasswordPolicy.GetByEndpointSystemId, parameter);
                if(objPasswordPolicy == null)
                {
                    objPasswordPolicy = DomainApiClient<Dto.PasswordPolicy>.GetById(1);
                }
            }
            else
            {
                objPasswordPolicy = DomainApiClient<Dto.PasswordPolicy>.GetById(1);
            }           

            //PASSWORD  LENGTH
            //if (objPasswordPolicy.PasswordLength != null && objPasswordPolicy.PasswordLength != 0)
            //{
            //    string pattern = @"^.{" + objPasswordPolicy.PasswordLength + "}$";
            //    System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Password, pattern);
            //    if (match.Success)
            //    {
            //    }
            //    else
            //    {
            //        errMsg = errMsg + "<br>" + string.Format("Password Should Contain " + objPasswordPolicy.PasswordLength + " Character(s)");
            //        isFalse++;
            //    }
            //}

            //MINIMUM AND MAXIMUM LENGTH OF PASSWORD.
            if (objPasswordPolicy.PasswordMinimumLength != null && objPasswordPolicy.PasswordMinimumLength != 0 &&
                objPasswordPolicy.PasswordMaximumLength != null && objPasswordPolicy.PasswordMaximumLength != 0)
            {
                string pattern = @"^.{" + objPasswordPolicy.PasswordMinimumLength + "," + objPasswordPolicy.PasswordMaximumLength + "}$";
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Password, pattern);
                if (match.Success)
                {
                }
                else
                {
                    errMsg = errMsg + "<br>" + string.Format("Password Should Contain Minimum " + objPasswordPolicy.PasswordMinimumLength + 
                                                            " Character(s) and Maximum " + objPasswordPolicy.PasswordMaximumLength + " Character(s)");
                    isFalse++;
                }
            }

            //MINIMUM  LENGTH OF PASSWORD.
            if (objPasswordPolicy.PasswordMinimumLength != null && objPasswordPolicy.PasswordMinimumLength != 0 && 
                (objPasswordPolicy.PasswordMaximumLength == null || objPasswordPolicy.PasswordMaximumLength == 0))
            {
                string pattern = @"^.{" + objPasswordPolicy.PasswordMinimumLength + ",}$";
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Password, pattern);
                if (match.Success)
                {
                }
                else
                {
                    errMsg = errMsg + "<br>" + string.Format("Password Should Contain Minimum " + objPasswordPolicy.PasswordMinimumLength + 
                                                               " Character(s)");
                    isFalse++;
                }
            }

            //MAXIMUM  LENGTH OF PASSWORD.
            if (objPasswordPolicy.PasswordMaximumLength != null && objPasswordPolicy.PasswordMaximumLength != 0 && 
                (objPasswordPolicy.PasswordMinimumLength == null || objPasswordPolicy.PasswordMinimumLength == 0))
            {
                string pattern = @"^.{0," + objPasswordPolicy.PasswordMaximumLength + "}$";
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Password, pattern);
                if (match.Success)
                {
                }
                else
                {
                    errMsg = errMsg + "<br>" + string.Format("Password Should Contain Maximum " + objPasswordPolicy.PasswordMaximumLength + 
                                                                " Character(s)");
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
                    errMsg = errMsg + "<br>" + string.Format("Password Should Contain Minimum " + objPasswordPolicy.NoOfSpecialCharacters + 
                                                                " Special Character(s)");
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
                    errMsg = errMsg + "<br>" + string.Format("Password Should Not Contain Special Character(s)");
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
                    errMsg = errMsg + "<br>" + string.Format("Password Should Not Contain Numeric(s)");
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
                    errMsg = errMsg + "<br>" + string.Format("Password Should Not Contain Alphabet(s)");
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
                    errMsg = errMsg + "<br>" + string.Format("Password Should Contain Minimum " + objPasswordPolicy.NoOfUpperCaseAlphabets + 
                                                                " Upper Case Alphabet(s)");
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
                    errMsg = errMsg + "<br>" + string.Format("Password Should Contain Minimum " + objPasswordPolicy.NoOfLowerCaseAlphabets + 
                                                                " Lower Case Alphabet(s)");
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
                    errMsg = errMsg + "<br>" + string.Format("Password Should Contain Minimum " + objPasswordPolicy.NoOfNumericCharacters + 
                                                                " Numeric(s)");
                    isFalse++;
                }
            }

            //CHECK NOT ALLOWED SPEACIAL CHARACTERS.
            if (objPasswordPolicy.NotAllowedCharacters != null && objPasswordPolicy.NotAllowedCharacters != String.Empty)
            {
                string pattern = @"^[^'" + objPasswordPolicy.NotAllowedCharacters + "']+$"; // return TRUE when No match found (Special Char Specified)
                string patternn = string.Format("^[^{0}]+$", objPasswordPolicy.NotAllowedCharacters);
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(Password, pattern);
                if (match.Success)
                {
                }
                else
                {
                    errMsg = errMsg + "<br>" + string.Format("Password Should Not Contain Special Character(s) : '" + objPasswordPolicy.NotAllowedCharacters + 
                                                                "'");
                    isFalse++;
                }
            }

            //CHECK PASSWORD CONTAINS USERNAME.
            if (Session.User.UserName != null && Session.User.UserName != String.Empty)
            {
                if (Password.ToUpper().Contains(Session.User.UserName.ToUpper()))
                {
                    errMsg = errMsg + "<br>" + string.Format("Password Should Not Contain User Name");
                    isFalse++;
                }
                else
                {
                }
            }

            //CHECK PASSWORD CONTAINS Endpoint Account Name.

            if (!String.IsNullOrWhiteSpace(UserName) && Password.ToUpper().Contains(UserName.ToUpper()))
            {
                errMsg = errMsg + "<br>" + string.Format("Password Should Not Contain Account Name");
                isFalse++;
            }

          

            //CHECK PASSWORD CONTAINS KEYWORDS.
            if (objPasswordPolicy.NotAllowedKeywords != null && objPasswordPolicy.NotAllowedKeywords != String.Empty)
            {
                string[] words = objPasswordPolicy.NotAllowedKeywords.Split(',');
                foreach (string word in words)
                {
                    if (Password.ToUpper().Contains(word.ToUpper()))
                    {
                        errMsg = errMsg + "<br>" + string.Format("Password Should Not Contain Specific KeyWords {0}", word);
                        isFalse++;
                    }
                    else
                    {
                    }
                }
            }

            //CHECK PASSWORD Change Number Of Days.
            if (this.EndPointId > 0 && objPasswordPolicy.NoOfDaysPasswordChangeAllowed != null)
            {
                //GET IACMLast Password change Date by Username
                string IACMLastPasswordChange = string.Empty;

                Dictionary<string, string> variables = new Dictionary<string, string>();
                variables.Add("endPointSystemId", endpoint.Id.ToString());
                variables.Add("userName", UserName);
                IACMLastPasswordChange = DomainApiClient<PasswordPolicy>.GetByAction<string>
                                    (DomainApiAction.Configuration.PasswordPolicy.GetIACMLastPasswordChangeByEndpointUser, variables);

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
                        errMsg = errMsg + "<br>" + string.Format("The previous {0} password reset was done on {1}. As per the policy, the next password reset is possible only after {2}. Please unselect the {0} to continue with the password sync.", endpoint.DisplayName, PasswordChangedDate, NextPwdChange);
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
    }
}