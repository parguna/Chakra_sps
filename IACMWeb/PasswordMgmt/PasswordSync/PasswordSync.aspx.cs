using Application;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.PasswordMgmt.PasswordSync
{
    public partial class PasswordSync : DI.Chakra.Web.UIBasePage
    {
        #region Member

        private SessionStatusImpl session = new SessionStatusImpl();
        PasswordPolicyValidator passwordPolicyValidator = new PasswordPolicyValidator();
        PasswordPolicy passwordPolicy = null;

        #endregion

        #region Properties
        public AuthenticationMode AuthenticationMode
        {
            get
            {
                return (AuthenticationMode)Enum.Parse(typeof(AuthenticationMode), MasterConfigHelper.AuthenticationMode);
            }
        }
        public IList<PresentationUserAccount> AccntDetail
        {
            get
            {
                if (ViewState["AccountDetail"] != null)
                {
                    return (IList<PresentationUserAccount>)ViewState["AccountDetail"];
                }
                return new List<PresentationUserAccount>();
            }
            set
            {
                ViewState["AccountDetail"] = value;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsPostBack)
                {
                    this.txtNewResetPassword.Attributes.Add("value", this.txtNewResetPassword.Text);
                    this.txtConfirmPassword.Attributes.Add("value", this.txtConfirmPassword.Text);
                    //AuthenticationMode = (AuthenticationMode)Enum.Parse(typeof(AuthenticationMode), MasterConfigHelper.AuthenticationMode);
                }
                else
                {
                    EnableValidator(false);
                    btnSubmit.Enabled = false;
                    btnUnlockAccount.Enabled = false;
                    GetData();
                }
            }
            catch { }
        }
        private void EnableValidator(bool enable)
        {
            Requiredfieldvalidator1.Enabled = enable;
            Requiredfieldvalidator2.Enabled = enable;
            cvPasswordCheck.Enabled = enable;
        }
        private void GetData()
        {

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userID", session.UserId.ToString());
            parameter.Add("selfServiceType", PasswordActionType.ResetPassword.ToString());
            AccountDetail.DataSource = DomainApiClient<PresentationUserAccount>.GetByAction<IList<PresentationUserAccount>>
                (DomainApiAction.Endpoint.Presentation.PresentationUserAccount.GetUserSelfServiceAccountDetail, parameter);
            btnSubmit.Enabled = false;
            btnUnlockAccount.Enabled = false;
            // AccountDetail.DataBind();
        }
        protected void AccountDetail_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GetData();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var dataItem = AccountDetail.SelectedItems[0] as GridDataItem;
            ApplicationType applicationType;
            if (dataItem != null)
            {
                var EndPointSystemID = (long)dataItem.GetDataKeyValue("EndpointSystemId");
                BindPolicy(EndPointSystemID);
                var userName = (string)dataItem.GetDataKeyValue("UserName");
                lblAccountName.Text = userName;
                var accType = Convert.ToInt64(dataItem.GetDataKeyValue("AccountType"));
                lblAccountType.Text = getAccTypeDescription(accType);
                lblendpointname.Text = (string)dataItem.GetDataKeyValue("EndpointSystemName");
            }
            txtNewResetPassword.Text = "";
            txtConfirmPassword.Text = "";
            EnableValidator(true);
            RadResetPwdWindow.VisibleOnPageLoad = true;

            
        }
        protected void AccountDetail_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                string status = Convert.ToString(item.GetDataKeyValue("Reserved1"));
                ImageButton _imgStatus = (ImageButton)item.FindControl("btnMessage");

                if (status == "Success")
                {
                    _imgStatus.Enabled = true;
                    _imgStatus.ImageUrl = "~/Images/status/success.png";
                }
                else if (status == "Failed")
                {
                    _imgStatus.Enabled = true;
                    _imgStatus.ImageUrl = "~/Images/status/fail.png";
                }
                var chk = item.FindControl("ClientSelectColumn") as CheckBox;
                if(chk != null)
                {
                    
                }
            }
        }

        protected void DisplayPolicies_Click(object sender, EventArgs e)
        {
            Button obj = sender as Button;
            GridDataItem item = (GridDataItem)obj.Parent.Parent;
            EnterprisePasswordPolicy1.EndPointSystemID = (long)item.GetDataKeyValue("EndpointSystemId");
            EnterprisePasswordPolicy1.BindPolicy();
            DisplayPolicy.VisibleOnPageLoad = true;
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            //Button obj = sender as Button;
            ////GridDataItem item = (GridDataItem)obj.Parent.Parent;
            //EnterprisePasswordPolicy1.EndPointSystemID = 1;
            //EnterprisePasswordPolicy1.BindPolicy();
           // DisplayPolicy.VisibleOnPageLoad = true;
            DisplayPolicy.VisibleOnPageLoad = false;
        }

        protected void AccountDetail_PreRender(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    // setting header text based on application level values
            //    foreach (GridColumn col in AccountDetail.Columns)
            //    {
            //        if (col.UniqueName == "EndpointSystemName")
            //            col.HeaderText = T("App.Common.Endpoint");
            //    }
            //    AccountDetail.Rebind();
            //}

            

        }

        protected void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            var x = sender as CheckBox;
            if (x != null)
            {
                bool checkStatus = x.Checked;
                foreach (GridDataItem item in AccountDetail.MasterTableView.Items)
                {
                    CheckBox chkbx = (CheckBox)item["CheckTemp"].FindControl("ClientSelectColumn");
                    chkbx.Checked = checkStatus;
                    item.Selected = checkStatus;
                }
            }
        }

        protected void ClientSelectColumn_CheckedChanged(object sender, EventArgs e)
        {
            EnableValidator(false);
            var selectedCheckBox = sender as CheckBox;
            var grdItem = selectedCheckBox.Parent.Parent as GridDataItem;
            bool chkchecked = false;
            bool chkStatus = false;
            bool chkunlockStatus = false;
           
            if (selectedCheckBox != null)
            {
                bool isResetAllowed = false;
                bool isUnLockedAllowed = false;

                bool checkStatus = selectedCheckBox.Checked;
                foreach (GridDataItem item in AccountDetail.MasterTableView.Items)
                {
                    
                    CheckBox chkbx = (CheckBox)item["CheckTemp"].FindControl("ClientSelectColumn");
                    if(chkbx == selectedCheckBox && chkbx.Checked)
                    {
                        chkchecked = chkbx.Checked;
                        isResetAllowed = Convert.ToBoolean(item.GetDataKeyValue("IsAllowPasswordReset"));
                        isUnLockedAllowed = Convert.ToBoolean(item.GetDataKeyValue("IsAllowAccountUnlock"));
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = chkbx.Checked = false;
                        item.BackColor = System.Drawing.Color.White;
                    }
                }
                btnSubmit.Enabled = chkchecked && isResetAllowed;
                btnUnlockAccount.Enabled = chkchecked && isUnLockedAllowed;
            }
            RadResetPwdWindow.VisibleOnPageLoad = false;

        }

        protected void ok_Click(object sender, EventArgs e)
        {
            DisplayPolicy.VisibleOnPageLoad = false;
        }

        /*protected void btnrefresh_Click(object sender, EventArgs e)
        {
            AccountDetail.Rebind();
        } */

        protected void Cancel_Submit_Click(object sender, EventArgs e)
        {
            this.txtNewResetPassword.Attributes.Add("value", string.Empty);
            this.txtConfirmPassword.Attributes.Add("value", string.Empty);
            Confirm.VisibleOnPageLoad = false;
        }

        #endregion
        
        #region Methods

        private void ShowValidationMessage(string message)
        {
            Confirm.VisibleOnPageLoad = true;
            lblConfirm.Text = message;
        }

        private void ResponseMessage(PresentationUserAccount preUserAccount)
        {
            if (preUserAccount.Reserved1 == "Success")
                lblConfirm.Text = "<span style='color:green;font-family:sans-serif'>" + preUserAccount.Reserved2 + "</span>";
            else
                lblConfirm.Text = "<span style='color:red;font-family:sans-serif'>" + preUserAccount.Reserved2 + "</span>";
            Confirm.VisibleOnPageLoad = true;
            AccountDetail.SelectedIndexes.Clear();
        }

        private void BindGrid(bool Skip)
        {
            if (Skip)
                AccountDetail.DataBind();
        }

        private PresentationUserAccount initiatePasswordReset(string accountName, string password, long epId, long epUserId, string endpointSystemType)
        {
            string message = string.Empty;
            var dataItem = AccountDetail.SelectedItems[0] as GridDataItem;
            var systemName = string.Empty;
            if(dataItem != null)
            {
                systemName = Convert.ToString(dataItem.GetDataKeyValue("EndpointSystemName"));
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userID", session.UserId.ToString());
            parameter.Add("endPointSystemId", epId.ToString());
            parameter.Add("endPointUserId", epUserId.ToString());

            PresentationUserAccount userAc = DomainApiClient<PresentationUserAccount>.GetByAction<PresentationUserAccount>
                (DomainApiAction.Endpoint.Presentation.PresentationUserAccount.GetUserAccountDetailByEndPointSystemId, parameter);
            passwordPolicyValidator.EndPointId = epId;
            bool bRet = passwordPolicyValidator.IsPasswordComplaint(accountName, password, out message);
            
            if (bRet == false)
            {
                userAc.Reserved1 = "PolicyFailed";
                userAc.Reserved2 = message;
                return userAc;
            }
            parameter.Clear();
            parameter.Add("userName", accountName);
            parameter.Add("password", password);
            parameter.Add("userId", session.UserId.ToString());
            parameter.Add("endPointUserId", epUserId.ToString());
            parameter.Add("endpointSystemId", epId.ToString());
            DILog.UILogger.WriteLog(session.UserId, LogLevelType.Info, $"EndpointSystem: {systemName} - {session.User.Reserved7} has initiated Password Reset for {accountName}");
            string retVal = DomainServiceClient.GetByAction<string>
                (DomainServiceAction.EndPointService.RootName, DomainServiceAction.EndPointService.ChangePassword, parameter);

            if (String.Compare(retVal, bool.TrueString, true) != 0)
            {
                // The raw backend/connector response can contain internal diagnostic detail and must
                // not be shown to the user; log it server-side and surface a generic message instead.
                DILog.UILogger.WriteLog(session.UserId, LogLevelType.Error, $"EndpointSystem: {systemName} - Password Reset for {accountName} failed: {retVal}");
                userAc.Reserved1 = "Failed";
                userAc.Reserved2 = "Password reset failed. Please contact your administrator if the problem persists.";
                return userAc;
            }
            DILog.UILogger.WriteLog(session.UserId, LogLevelType.Success, $"EndpointSystem: {systemName} - Password Reset for {accountName} is successful");
            userAc.Reserved1 = "Success";
            userAc.Reserved2 = $"Password reset completed for the account : {accountName}";
            return userAc;
        }

        protected void btnUnlockAccount_Click(object sender, EventArgs e)
        {
            RadResetPwdWindow.VisibleOnPageLoad = false;
            if (AccountDetail.SelectedItems.Count == 0)
            {
                ShowValidationMessage("Please select an item");
                return;
            }
            RadUnlockWindow.VisibleOnPageLoad = true;

        }


        private PresentationUserAccount initiateAccountUnlock(string accountName, string endpointSystemType, long endpointSystemId, long epUserId)
        {
            string message = string.Empty;
            var dataItem = AccountDetail.SelectedItems[0] as GridDataItem;
            var systemName = string.Empty;
            if (dataItem != null)
            {
                systemName = Convert.ToString(dataItem.GetDataKeyValue("EndpointSystemName"));
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userID", session.UserId.ToString());
            parameter.Add("endPointSystemId", endpointSystemId.ToString());
            parameter.Add("endPointUserId", epUserId.ToString());

            PresentationUserAccount userAc = DomainApiClient<PresentationUserAccount>.GetByAction<PresentationUserAccount>
                (DomainApiAction.Endpoint.Presentation.PresentationUserAccount.GetUserAccountDetailByEndPointSystemId, parameter);

            parameter.Clear();
            parameter.Add("userName", accountName);
            parameter.Add("endpointSystemId", endpointSystemId.ToString());
            parameter.Add("endPointUserId", epUserId.ToString());

            string retVal = String.Empty;

            DILog.UILogger.WriteLog(session.UserId, LogLevelType.Info, $"EndpointSystem: {systemName} - {session.User.Reserved7} has initiated Account unlock for {accountName}");
            retVal = DomainServiceClient.GetByAction<string>
            (DomainServiceAction.EndPointService.RootName, "UnlockAccount", parameter);

            if (String.Compare(retVal, bool.TrueString, true) != 0)
            {
                // The raw backend/connector response can contain internal diagnostic detail and must
                // not be shown to the user; log it server-side and surface a generic message instead.
                DILog.UILogger.WriteLog(session.UserId, LogLevelType.Error, $"EndpointSystem: {systemName} - Account Unlock for {accountName} is failed: {retVal}");
                userAc.Reserved1 = "Failed";
                userAc.Reserved2 = "Account unlock failed. Please contact your administrator if the problem persists.";
                return userAc;
            }
            DILog.UILogger.WriteLog(session.UserId, LogLevelType.Success, $"EndpointSystem: {systemName} - Account Unlock for {accountName} is successful");
            userAc.Reserved1 = "Success";
            userAc.Reserved2 = $"Account unlock successful for the account : {accountName} in {userAc?.EndpointSystemName}" ;
            return userAc;
        }

        protected bool IsUnlockButtonNeed(string AccountStatus)
        {
            bool accountStatus = AccountStatus.ToLower() == "locked";
            return accountStatus;
        }



        #endregion

        protected void btnConfirmSubmit_Click(object sender, EventArgs e)
        {

            RadConfirmWindow.VisibleOnPageLoad = false;
            var dataItem = AccountDetail.SelectedItems[0] as GridDataItem;

            DisplayPolicy.VisibleOnPageLoad = false;
            PresentationUserAccount uAc = null;

            if (dataItem != null)
            {
                long epId = Convert.ToInt64(dataItem.GetDataKeyValue("EndpointSystemId"));
                long epUserId = Convert.ToInt64(dataItem.GetDataKeyValue("EndpointUserId"));
                string endpointSystemType = dataItem.GetDataKeyValue("EndPointSystemType").ToString();
                bool isLocked = Convert.ToBoolean(dataItem.GetDataKeyValue("AccountLocked"));
                var applicationType = (ApplicationType)Enum.Parse(typeof(ApplicationType), dataItem.GetDataKeyValue("ApplicationType")?.ToString());
                var accountType = (AccountType)Enum.Parse(typeof(AccountType), dataItem.GetDataKeyValue("AccountType")?.ToString());
                
                lblPasswordPolicyMsg.Text = "";
                
                uAc = initiatePasswordReset(dataItem["AccountName"].Text.ToString(), txtNewResetPassword.Text.Trim(), epId, epUserId, endpointSystemType);
                if (uAc.Reserved1 == "PolicyFailed" || uAc.Reserved1 == "Failed")
                {
                    lblPasswordPolicyMsg.Text = uAc.Reserved2;
                    RadResetPwdWindow.VisibleOnPageLoad = true;
                }
                else
                {
                    MasterPage.ShowAlert(Infrastructure.AlertType.Success, $"{uAc.Reserved2}");
                    
                    if (AuthenticationMode == AuthenticationMode.AD && applicationType == ApplicationType.ADSingaporePowerLocal && accountType == AccountType.User)
                    {
                       Response.Redirect("~/login.aspx");
                    }
                    txtConfirmPassword.Text = "";
                    txtNewResetPassword.Text = "";
                    
                    UpdatePanel4.Update();
                    AccountDetail.Rebind();
                }
            }
        }
       
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            RadConfirmWindow.VisibleOnPageLoad = false;
        }

        protected void btnUnlockSubmit_Click(object sender, EventArgs e)
        {
            
            RadUnlockWindow.VisibleOnPageLoad = false;
            accountUnlock();
            
            AccountDetail.Rebind();
        }

        protected void CancelUnlockButton_Click(object sender, EventArgs e)
        {
            RadUnlockWindow.VisibleOnPageLoad = false;
        }

        protected void btnResetConfirm_Click(object sender, EventArgs e)
        {
            lblPasswordPolicyMsg.Text = "";
            var dataItem = AccountDetail.SelectedItems[0] as GridDataItem;
            long epId = Convert.ToInt64(dataItem.GetDataKeyValue("EndpointSystemId"));
            long epUserId = Convert.ToInt64(dataItem.GetDataKeyValue("EndpointUserId"));
            string endpointSystemType = dataItem.GetDataKeyValue("EndPointSystemType").ToString();
            bool isLocked = Convert.ToBoolean(dataItem.GetDataKeyValue("AccountLocked"));
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userID", session.UserId.ToString());
            parameter.Add("endPointSystemId", epId.ToString());
            parameter.Add("endPointUserId", epUserId.ToString());

            PresentationUserAccount userAc = DomainApiClient<PresentationUserAccount>.GetByAction<PresentationUserAccount>
                (DomainApiAction.Endpoint.Presentation.PresentationUserAccount.GetUserAccountDetailByEndPointSystemId, parameter);
            passwordPolicyValidator.EndPointId = epId;
            string message = string.Empty;
            var accountName = dataItem["AccountName"].Text;
            bool bRet = passwordPolicyValidator.IsPasswordComplaint(accountName, txtNewResetPassword.Text, out message);

            if (bRet == false)
            {
                userAc.Reserved1 = "PolicyFailed";
                userAc.Reserved2 = message;
                lblPasswordPolicyMsg.Text = message;
                //ShowValidationMessage(message);

            }
            else
            {
                RadConfirmWindow.VisibleOnPageLoad = true;
                RadResetPwdWindow.VisibleOnPageLoad = false;
            }
            //long epId = Convert.ToInt64(dataItem.GetDataKeyValue("EndpointSystemId"));
            //long epUserId = Convert.ToInt64(dataItem.GetDataKeyValue("EndpointUserId"));
            //string endpointSystemType = dataItem.GetDataKeyValue("EndPointSystemType").ToString();
            //bool isLocked = Convert.ToBoolean(dataItem.GetDataKeyValue("AccountLocked"));


                ////DILog.UILogger.WriteLog(session.UserId, LogLevelType.Info, string.Format("{0} has initiated Password Reset for {1}", session.User.Reserved7, dataItem["AccountName"].Text.ToString()));
                //var uAc = initiatePasswordReset(dataItem["AccountName"].Text.ToString(), txtNewResetPassword.Text.Trim(), epId, epUserId, endpointSystemType);
                //if (uAc.Reserved1 == "PolicyFailed")
                //{
                //    ShowValidationMessage(uAc.Reserved2);
                //    RadResetPwdWindow.VisibleOnPageLoad = true;
                //}
            
        }

        protected void btnResetCancel_Click(object sender, EventArgs e)
        {
            txtNewResetPassword.Text = "";
            txtConfirmPassword.Text = "";
            lblPasswordPolicyMsg.Text = "";
            Requiredfieldvalidator1.Enabled = false;
            Requiredfieldvalidator2.Enabled = false;
            UpdatePanel4.Update();
            RadResetPwdWindow.VisibleOnPageLoad = false;

        }

        private static string getAccTypeDescription( Int64 accountType)
        {
            var accType = string.Empty;
            switch(accountType)
            {
                case 0:
                    accType = "None";
                    break;

                case 1:
                     accType= "User Account";
                    break;
                        
                case 2:
                    accType = "Service Account";
                    break;

                case 3:
                    accType = "Shared Account";
                    break;

                case 4:
                    accType = "Secondary Account";
                    break;

                default:
                    accType = "none";
                    break;

            }
            return accType;
        }

        private void accountUnlock()
        {
            try
            {
                DisplayPolicy.VisibleOnPageLoad = false;
                PresentationUserAccount uAc = null;

                var dataItem = AccountDetail.SelectedItems[0] as GridDataItem;
                if (dataItem != null)
                {
                    long epId = Convert.ToInt64(dataItem.GetDataKeyValue("EndpointSystemId"));
                    long epUserId = Convert.ToInt64(dataItem.GetDataKeyValue("EndpointUserId"));
                    string endpointSystemType = dataItem.GetDataKeyValue("EndPointSystemType").ToString();
                    var accountName = dataItem["AccountName"].Text;
                    uAc = initiateAccountUnlock(accountName, endpointSystemType, epId, epUserId);
                    if (uAc != null && uAc.Reserved1 == "Failed")
                        MasterPage.ShowAlert(Infrastructure.AlertType.Warning, uAc.Reserved2);
                    else
                    {
                        MasterPage.ShowAlert(Infrastructure.AlertType.Success, $"Account:{accountName} unlocked successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(Infrastructure.AlertType.Error,$"{ex.Message}");
            }
        }

        public void BindPolicy(long EndPointSystemID)
        {
            long ctrlID = 1;
            if (EndPointSystemID > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointSystemId", EndPointSystemID.ToString());
                passwordPolicy = DomainApiClient<Dto.PasswordPolicy>.GetByAction<PasswordPolicy>(DomainApiAction.Configuration.PasswordPolicy.GetByEndpointSystemId, parameter);
            }
            else if (EndPointSystemID == 0)
            {
                passwordPolicy = DomainApiClient<Dto.PasswordPolicy>.GetById(1);
            }
            if (passwordPolicy == null)
                return;
            GetPolicy(passwordPolicy.Id);

        }
        public class bclass
        {
            public string name { get; set; }
            public string value { get; set; }
        }
        public void GetPolicy(long policyId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("policyId", policyId.ToString());
            var policy = DomainApiClient<Dto.PasswordPolicy>.GetByAction<Dto.PasswordPolicy>(DomainApiAction.Configuration.PasswordPolicy.GetByPolicyId, para);
            IList<bclass> blist = new List<bclass>();
            bclass b = new bclass();

            /*if (policy.Name != null && policy.Name != string.Empty)
            {
                b = new bclass();
                b.name = "Policy Name";
                b.value = policy.Name.ToString();
                blist.Add(b);
            }*/


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