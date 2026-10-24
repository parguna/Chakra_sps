using System;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using Telerik.Web.UI;
using System.Globalization;

namespace DI.Chakra.Web.NACR.Create.App.Special
{
    public partial class DefaultAccountInfoUC : UIBaseUC
    {
        private const string V_ENDPOINTID = "V_ENDPOINTID";
        private const string V_ACCOUNTTYPE = "V_ACCOUNTTYPE";

        #region Members
        #endregion

        #region Property
        public long EndPointID
        {
            get
            {
                if (ViewState[V_ENDPOINTID] == null)
                    return 0;
                return (long)ViewState[V_ENDPOINTID];
            }
            set
            {
                ViewState[V_ENDPOINTID] = value;
            }
        }

        public AccountType RequestedAccountType
        {
            get
            {
                if (ViewState[V_ACCOUNTTYPE] == null)
                    return AccountType.None;
                return (AccountType)ViewState[V_ACCOUNTTYPE];
            }
            set
            {
                ViewState[V_ACCOUNTTYPE] = value;
            }
        }



        #endregion

        public void Initilize()
        {
            AccountNameTextBox.Text = "";
            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            DisplayNameTextBox.Text = "";
            DescriptionTextBox.Text = "";
            DepartmentAutoCompleteBox.Clear();

            if (RequestedAccountType == AccountType.Secondary)
            {
                divDepartment.Visible = false;
            }
            else if (RequestedAccountType == AccountType.Service)
            {
                divLastName.Visible = false;
                divDepartment.Visible = false;
            }
            else if (RequestedAccountType == AccountType.Shared)
            {

            }
            else
            {

            }
        }

        public dynamic GetDetailObject()
        {
            dynamic detail = new ExpandoObject();

            ExpandoHelper.AddProperty(detail, "AccountName", AccountNameTextBox.Text.Trim().ToLower());
            ExpandoHelper.AddProperty(detail, "DisplayName", DisplayNameTextBox.Text.Trim());
            ExpandoHelper.AddProperty(detail, "givenname", FirstNameTextBox.Text.Trim());
            ExpandoHelper.AddProperty(detail, "sn", LastNameTextBox.Text.Trim());
            ExpandoHelper.AddProperty(detail, "Description", DescriptionTextBox.Text.Trim());

            AutoCompleteBoxEntry selectedDepartment = DepartmentAutoCompleteBox.GetValue();
            var DepartmentText = "";
            if (selectedDepartment != null)
            {
                DepartmentText = selectedDepartment.Text;
            }
            ExpandoHelper.AddProperty(detail, "Department", DepartmentText);
            return detail;

        }

        public bool CheckValidation(out string errorMessage, long endpointTypeId, AccountType accountType)
        {
            errorMessage = "";
            if (string.IsNullOrEmpty(FirstNameTextBox.Text.Trim()))
            {
                errorMessage += "Please enter First Name.";
                return false;
            }
            else if (string.IsNullOrEmpty(DisplayNameTextBox.Text.Trim()))
            {
                errorMessage += "Please enter Display Name.";
                return false;
            }
            else if (string.IsNullOrEmpty(DescriptionTextBox.Text.Trim()))
            {
                errorMessage += "Please enter Description.";
                return false;
            }
            else if (string.IsNullOrEmpty(AccountNameTextBox.Text.Trim()))
            {
                errorMessage += "Please enter Accout Name.";
                return false;
            }
            else if ((!string.IsNullOrEmpty(AccountNameTextBox.Text.Trim()) && AccountNameTextBox.Text.Trim().Contains(" ")))
            {
                errorMessage += "Account Name Should not contains empty spaces.";
                return false;
            }

            if (RequestedAccountType == AccountType.Shared && DepartmentAutoCompleteBox.GetValue() == null)
            {
                errorMessage += "Please select Department.";
                return false;
            }

            if (RequestedAccountType != AccountType.Service && string.IsNullOrEmpty(LastNameTextBox.Text.Trim()))
            {
                errorMessage += "Please enter Last Name.";
                return false;
            }

            if (!string.IsNullOrEmpty(AccountNameTextBox.Text.Trim()))
            {
                if ((long)EndpointType.ActiveDirectory == endpointTypeId)
                {
                    var accountNameSpl = AccountNameTextBox.Text.Trim().ToLower();
                    if (RequestedAccountType != AccountType.Shared && accountNameSpl.Length > 15)
                    {
                        errorMessage += "Account name length should not be greater than 15 characters.";
                        return false;
                    }

                    if (accountType == AccountType.Secondary && !accountNameSpl.EndsWith(".adm"))
                    {
                        errorMessage += "Active Directory Admin account should ends with .adm ";
                        return false;
                    }

                    if (accountType == AccountType.Secondary && accountNameSpl.StartsWith(".adm"))
                    {
                        errorMessage += "please enter Active Directory account name";
                        return false;
                    }

                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("name", Convert.ToString(accountNameSpl));

                    var tmpUsr = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                        (DomainApiAction.User.GetByUserName, parameter);

                    if (tmpUsr != null && tmpUsr.Id > 0)
                    {
                        errorMessage += "user name already available in Active Directory, please try with other options.";
                        return false;
                    }
                }
            }

            return true;
        }

        protected void FirstNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (RequestedAccountType == AccountType.Service)
            {
                this.FirstNameTextBox.Text = this.FirstNameTextBox.Text.Trim().ToUpper();
                this.AccountNameTextBox.Text = this.FirstNameTextBox.Text.ToLower();
            }
            else
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                this.FirstNameTextBox.Text = textInfo.ToTitleCase(FirstNameTextBox.Text);
            }

            if (RequestedAccountType == AccountType.Secondary)
                this.AccountNameTextBox.Text = string.Format("{0}.adm", this.FirstNameTextBox.Text.ToLower());

            this.AccountNameTextBox.Text = this.AccountNameTextBox.Text.Replace(" ", "");
            if (RequestedAccountType == AccountType.Secondary)
                this.DisplayNameTextBox.Text = this.FirstNameTextBox.Text.Trim().ToUpper();
            else
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                this.DisplayNameTextBox.Text = string.Format("{0} {1}", textInfo.ToTitleCase(FirstNameTextBox.Text), textInfo.ToTitleCase(LastNameTextBox.Text));
            }
        }

        protected void DisplayNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (RequestedAccountType == AccountType.Service)
                this.DisplayNameTextBox.Text = this.DisplayNameTextBox.Text.Trim().ToUpper();
            else
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                this.DisplayNameTextBox.Text = textInfo.ToTitleCase(DisplayNameTextBox.Text);
            }
        }

        protected void LastNameTextBox_TextChanged(object sender, EventArgs e)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            this.LastNameTextBox.Text = textInfo.ToTitleCase(LastNameTextBox.Text);

            if (RequestedAccountType != AccountType.Service)
            {
                this.DisplayNameTextBox.Text = string.Format("{0} {1}", textInfo.ToTitleCase(FirstNameTextBox.Text), textInfo.ToTitleCase(LastNameTextBox.Text));
            }
        }
    }
}