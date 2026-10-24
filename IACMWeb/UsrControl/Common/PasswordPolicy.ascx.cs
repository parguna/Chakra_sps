

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class PasswordPolicy : UIBaseUC
    {
        #region Private Members
        private bool IsDisplayed = false;
    
        #endregion

        public class bclass
        {
            public string name { get; set; }
            public string value { get; set; }
        }
        public void GetPolicy(long policyId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("policyId", policyId.ToString());
            var policy = DomainApiClient<Dto.PasswordPolicy>.GetByAction< Dto.PasswordPolicy>(DomainApiAction.Configuration.PasswordPolicy.GetByPolicyId,para);
            IList<bclass> blist = new List<bclass>();
            bclass b = new bclass();

            if (policy.Name != null && policy.Name != string.Empty)
            {
                b = new bclass();
                b.name = "Policy Name";
                b.value = policy.Name.ToString();
                blist.Add(b);
            }


            if (policy.PasswordLength != null)
            {
                b = new bclass();
                b.name = "Password Length";
                b.value = policy.PasswordLength.ToString();
                blist.Add(b);
            }

            if (policy.PasswordMaximumLength != null)
            {
                b = new bclass();
                b.name = "Password Maximum Length";
                b.value = policy.PasswordMaximumLength.ToString();
                blist.Add(b);
            }

            if (policy.PasswordMinimumLength != null)
            {
                b = new bclass();
                b.name = "Password Minimum Length";
                b.value = policy.PasswordMinimumLength.ToString();
                blist.Add(b);
            }
            if (policy.NoOfSpecialCharacters != null)
            {
                b = new bclass();
                b.name = "Minimum Number Of Special Characters";
                b.value = policy.NoOfSpecialCharacters.ToString();
                blist.Add(b);
            }
            if (policy.NoOfUpperCaseAlphabets != null)
            {
                b = new bclass();
                b.name = "Minimum Number Of Upper Case Alphabets";
                b.value = policy.NoOfUpperCaseAlphabets.ToString();
                blist.Add(b);
            }

            if (policy.NoOfLowerCaseAlphabets != null)
            {
                b = new bclass();
                b.name = "Minimum Number Of Lower Case Alphabets";
                b.value = policy.NoOfLowerCaseAlphabets.ToString();
                blist.Add(b);
            }

            if (policy.NoOfNumericCharacters != null)
            {
                b = new bclass();
                b.name = "Minimum Number Of Numeric";
                b.value = policy.NoOfNumericCharacters.ToString();
                blist.Add(b);
            }
            if (policy.NotAllowedCharacters != null && policy.NotAllowedCharacters != string.Empty)
            {
                b = new bclass();
                b.name = "Not Allowed Special Characters";
                b.value = policy.NotAllowedCharacters.ToString();
                blist.Add(b);
            }

            if (policy.NoOfDaysPasswordChangeAllowed != null)
            {
                b = new bclass();
                b.name = "Minimum Password Age ";
                b.value = policy.NoOfDaysPasswordChangeAllowed.ToString();
                blist.Add(b);
            }

            if (policy.NotAllowedKeywords != null && policy.NotAllowedKeywords != string.Empty)
            {
                b = new bclass();
                b.name = "Not Allowed Keywords";
                b.value = policy.NotAllowedKeywords.ToString();
                blist.Add(b);
            }
            if (policy.UserNameNotAllowedInPassword != null && Convert.ToBoolean(policy.UserNameNotAllowedInPassword.ToString()).ToString() == "True")
            {
                b = new bclass();
                b.name = "User Name Not Allowed In Password";
                b.value = "Yes"; // string.Empty;
                blist.Add(b);
            }
            if (policy.SpecialCharNotAllowed != null && Convert.ToBoolean(policy.SpecialCharNotAllowed.ToString()).ToString() == "True")
            {
                b = new bclass();
                b.name = "Special Characters are  Not Allowed In Password";
                b.value = "Yes"; // string.Empty;
                blist.Add(b);
            }
            if (policy.AlphabetsNotAllowed != null && Convert.ToBoolean(policy.AlphabetsNotAllowed.ToString()).ToString() == "True")
            {
                b = new bclass();
                b.name = "Alphabets are Not Allowed In Password ";
                b.value = "Yes"; // string.Empty;
                blist.Add(b);
            }
            if (policy.NumericCharacterNotAllowed != null && Convert.ToBoolean(policy.NumericCharacterNotAllowed.ToString()).ToString() == "True")
            {
                b = new bclass();
                b.name = "Numeric are Not Allowed In Password  ";
                b.value = "Yes"; // string.Empty;
                blist.Add(b);
            }

            Repeater1.DataSource = blist;
            Repeater1.DataBind();


        }
    }
}