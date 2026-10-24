using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Admin.AdminUI
{
    public partial class List : DI.Chakra.Web.UIBasePage
    {
        const string ServiceName = "EndPointService";
        private XLDataConverterHelper xlHelper;
        static GridEditableItem selectedDataItem = null;
        private SessionStatusImpl SessionUser = new SessionStatusImpl();
        public XLDataConverterHelper XlHelper
        {
            get
            {
                if (xlHelper == null)
                {
                    xlHelper = new XLDataConverterHelper();
                }
                return xlHelper;
            }
            set { xlHelper = value; }
        }
        private SessionStatusImpl sessionStatus;

        public string Comments
        {
            get { return txtComment.Text; }
            set { txtComment.Text = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                sessionStatus = new SessionStatusImpl();
                if (!IsPostBack)
                {
                    Panel1.Visible = false;
                    Panel3.Visible = false;
                    Panel2.Visible = false;
                    btnDeactivateExpiredDelegation.Visible = false;
                    if (SessionUser.User.UserName.ToUpper() == "IACMADMIN" || SessionUser.User.Id == 1)
                    {
                        //Show decomminssion option to IACMAdmin only
                        Panel1.Visible = true;
                        //withdraw
                        Panel3.Visible = true;
                        //Resubmit
                        Panel2.Visible = true;
                        //Delegation
                        btnDeactivateExpiredDelegation.Visible = true;

                        pnlServiceAccount.Visible = true;
                        btnMassPropagateO365.Visible = true;
                    }
                    else
                    {
                        pnlServiceAccount.Visible = false;
                        btnMassPropagateO365.Visible = false;
                    }
                    

                    UserSearchUC.AllUsers = true;
                    BindServiceAccountQuestions();
                    ddlRequestType.Items.Clear();
                    ddlRequestType.Items.Add(new ListItem("[---Select---]", "0"));
                    ddlRequestType.Items.Add(new ListItem("AccessRequest", "1"));
                    ddlRequestType.Items.Add(new ListItem("BulkRequest", "2"));
                }
            }
            catch (System.Exception ex)
            {

            }
        }

        private void BindServiceAccountQuestions()
        {
            var data = DomainApiClient<Dto.ServiceAccountQuestion>.GetAll();
            serviceAccountQuestion.DataSource = data;
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {

            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {

            }
        }

        DataTable GetDataTableByQuery()
        {
            var parameters = new Dictionary<string, string>();
            //parameters.Add("query", QueryTextBox.Text.Trim());
            var data = DomainApiClient<Dto.ADSXLProvisioningMatrix>.GetByAction<DataTable>(DomainApiAction.ReportDefinitions.GetDataTableExecuteQuery, parameters);
            return data;
        }

        void ExecuteQuery()
        {
            var parameters = new Dictionary<string, string>();
            //parameters.Add("query", QueryTextBox.Text.Trim());
            DomainApiClient<Dto.ADSXLProvisioningMatrix>.GetByAction<Dto.ADSXLProvisioningMatrix>(DomainApiAction.ReportDefinitions.ExecuteQuery, parameters);
        }

        protected void btnOpenDataViewer_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/DataManagement/DataViewer/List.aspx");
        }

        protected void btnMassPropagateO365_Click(object sender, EventArgs e)
        {
            Response.Redirect("MassPropagateO365License.aspx");
        }
        protected void btnDownloadAllFromHRMS_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Convert.ToString(DatePickerFromDate.SelectedDate)))
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please enter the scan date.");
                    return;
                }

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("scanDate", DatePickerFromDate.SelectedDate.ToString());

                var result =
                DomainServiceClient.GetByAction<int>
                   (ServiceName, "DownloadAllUsersFromHR", parameter);

                if (result > 0)
                    MasterPage.ShowAlert(AlertType.Success, $"Downloaded {result} users from HRMS system successfully.");
                else
                    MasterPage.ShowAlert(AlertType.Error, "No users downloaded.");
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, ex.Message);
            }
        }

        protected void btnDownloadUserFromHRMS_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtPernrDownload.Text))
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please enter the pernr of the user.");
                    return;
                }

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("pernr", txtPernrDownload.Text);

                var result =
                DomainServiceClient.GetByAction<int>
                   (ServiceName, "DownloadSpecificUserFromHR", parameter);

                if (result > 0)
                    MasterPage.ShowAlert(AlertType.Success, $"Downloaded {result} user from HRMS system successfully.");
                else
                    MasterPage.ShowAlert(AlertType.Error, "No users downloaded.");
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, ex.Message);
            }
        }

        protected void btnDiscoverUserFromHRMS_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtPernrDiscover.Text))
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please enter the pernr of the user.");
                    return;
                }

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("pernr", txtPernrDiscover.Text);
                if (DatePickerFromDate.SelectedDate != null && DatePickerFromDate.SelectedDate.HasValue)
                {
                    if (DatePickerFromDate.SelectedDate.Value > DateTime.Now)
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Date cannot be futureDate");
                        return;
                    }
                    parameter.Add("fromDate", DatePickerFromDate.SelectedDate.Value.ToString("dd-MMM-yyyy"));
                }

                var result = DomainServiceClient.GetByAction<int>(ServiceName, "DiscoverSpecificUserFromHR", parameter);

                if (result > 0)
                    MasterPage.ShowAlert(AlertType.Success, $"Discovered {result} user from HRMS system successfully.");
                else
                    MasterPage.ShowAlert(AlertType.Error, "No users discovered.");
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, ex.Message);
            }
        }

        protected void btnAdminConsole_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AppSetting/ApplicationSetting.aspx");
        }

        protected void btnHRMS_Details_Click(object sender, EventArgs e)
        {
            Response.Redirect("HRMSDetails.aspx");
        }

        protected void UserSearchUC_UserChanged(object sender)
        {
            try
            {
                lblExistingToken.Text = System.Web.HttpUtility.HtmlEncode(UserSearchUC.SelectedUser?.OneKeyTokenId);
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, $"{ex.Message}");
            }
        }

        protected void btnUpdateOneKeyToken_Click(object sender, EventArgs e)
        {
            try
            {
                var oneKeyToken = txtOneKeyToken.Text;
                if (UserSearchUC.SelectedUser == null)
                {
                    MasterPage.ShowAlert(AlertType.Error, $"Please select user");
                    return;
                }
                if (string.IsNullOrWhiteSpace(oneKeyToken))
                {
                    oneKeyToken = null;
                }

                var userid = UserSearchUC.SelectedUser.Id;
                var updateBy = new SessionStatusImpl().User;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("applicationType", ((int)ApplicationType.VPN).ToString());
                var ep = DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>("GetEndPointByApplicationType", parameter);

                if (oneKeyToken != null)
                {
                    parameter = new Dictionary<string, string>();
                    parameter.Add("userid", userid.ToString());
                    parameter.Add("accountName", oneKeyToken);
                    parameter.Add("EndpointSystemId", ep.Id.ToString());
                    var accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>
                            (DomainApiAction.Provisioning.AccountReconcilation.DoesAccountExistInAccountRecon, parameter);
                    if (accountRecon != null && accountRecon.IsDeleted == false)
                    {
                        var errormsg = $"Token Number:{oneKeyToken} already exists. Please enter valid Token Number.";
                        MasterPage.ShowAlert(Infrastructure.AlertType.Error, errormsg);
                        return;
                    }
                }
                var usr = DomainApiClient<Dto.User>.GetById(userid);
                var msg = $"Updating token: User:{usr.DisplayName}({usr.UserName}) Old Token: {usr.OneKeyTokenId} New Token:{oneKeyToken}";
                DILog.UILogger.WriteLog(updateBy.Id, LogLevelType.Info, msg);
                parameter = new Dictionary<string, string>();
                parameter.Add("userid", userid.ToString());
                parameter.Add("oldToken", UserSearchUC.SelectedUser.OneKeyTokenId);
                parameter.Add("newToken", oneKeyToken);
                parameter.Add("EndpointSystemId", ep.Id.ToString());
                parameter.Add("updatedBy", updateBy.UserName.ToString());
                var isUpdated = DomainApiClient<Dto.AccountReconcilation>.GetByAction<bool>("UpdateOneKeyTokenForUserThroughAdminUI", parameter);
                if (isUpdated)
                {
                    MasterPage.ShowAlert(AlertType.Success, $"Token:{oneKeyToken} Updated successfully for User:{usr.DisplayName}");
                    lblExistingToken.Text = System.Web.HttpUtility.HtmlEncode(oneKeyToken);
                    txtOneKeyToken.Text = "";
                }
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, $"{ex.Message}");
            }
        }



        protected void btnNewRecord_Click(object sender, EventArgs e)
        {
            serviceAccountQuestion.MasterTableView.InsertItem();
            serviceAccountQuestion.Rebind();
            selectedDataItem = serviceAccountQuestion.MasterTableView.GetInsertItem();
        }

        protected void btnEditRecord_Click(object sender, EventArgs e)
        {
            if (serviceAccountQuestion.SelectedItems.Count > 0)
            {
                selectedDataItem = serviceAccountQuestion.SelectedItems[0] as GridEditableItem;
                selectedDataItem.Edit = true;
                //selectedDataItem["Edit"].Controls[0].Visible = true;
                //ChangeEditButton(grdInvalidEntries.SelectedItems[0] as GridDataItem, true);
                serviceAccountQuestion.Rebind();
            }
        }

        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, $"{ex.Message}");
            }
        }
        public string GetSerializeString<T>(long id)
        {
            var serializeString = string.Empty;
            try
            {
                var data = DomainApiClient<T>.GetById(id);
                if (data != null)
                {
                    serializeString = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                }
            }
            catch
            {
            }
            return serializeString;
        }
        Dto.ServiceAccountQuestion PopulateData(Hashtable values, long id, Dto.User usr)
        {

            var existingData = DomainApiClient<Dto.ServiceAccountQuestion>.GetById(id);
            if (existingData != null && existingData.Id > 0)
            {
                existingData.Description = (string)values["Description"];
                existingData.IsActive = Convert.ToBoolean(values["IsActive"]);
                existingData.LastModifiedBy = usr.UserName;
                return existingData;
            }
            else
            {
                existingData = new Dto.ServiceAccountQuestion();
                existingData.Description = (string)values["Description"];
                existingData.IsActive = Convert.ToBoolean(values["IsActive"]);
                existingData.CreatedDate = DateTime.Now;
                existingData.CreatedBy = usr.UserName;
                return existingData;
            }
        }

        protected void serviceAccountQuestion_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindServiceAccountQuestions();
        }



        protected void serviceAccountQuestion_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode)
            {
                try
                {
                    GridEditableItem item = e.Item as GridEditableItem;
                    GridEditManager manager = item.EditManager;
                    GridTextBoxColumnEditor editor = manager.GetColumnEditor("Id") as GridTextBoxColumnEditor;
                    editor.TextBoxControl.Enabled = false;
                }
                catch
                {
                }
            }
        }


        protected void serviceAccountQuestion_InsertCommand(object sender, GridCommandEventArgs e)
        {
            UpdateOrInsert(sender, e);
        }
        private void UpdateOrInsert(object sender, GridCommandEventArgs e)
        {
            try
            {
                if (e.Item is GridEditableItem)
                {
                    GridEditableItem editedItem = e.Item as GridEditableItem;
                    Hashtable values = new Hashtable();
                    e.Item.OwnerTableView.ExtractValuesFromItem(values, editedItem);
                    var boolIsActive = values["IsActive"]?.ToString();
                    if (string.IsNullOrWhiteSpace(boolIsActive))
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Please enter IsActive");
                        return;
                    }
                    var isValid = false;
                    if (bool.TryParse(boolIsActive, out isValid) == false)
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Please enter valid IsActive value:  True / False");
                        return;
                    }
                    long id = 0;
                    try
                    {
                        id = Convert.ToInt64(editedItem.GetDataKeyValue("Id"));
                    }
                    catch
                    {
                    }
                    var usr = new SessionStatusImpl().User;

                    string successMessage = string.Empty;
                    bool editMode = false;
                    Dto.ServiceAccountQuestion xldata = null;
                    string oldData = null;

                    if (id > 0)
                    {
                        editMode = true;
                        xldata = PopulateData(values, id, usr);
                        successMessage = "Record updated successfully.";
                        oldData = GetSerializeString<Dto.ServiceAccountQuestion>(id);
                    }
                    else
                    {
                        editMode = false;
                        xldata = PopulateData(values, id, usr);
                        successMessage = "Record inserted successfully.";
                    }
                    if (string.IsNullOrWhiteSpace(xldata.Description))
                    {
                        MasterPage.ShowAlert(AlertType.Error, $"Please enter Description");
                        return;
                    }
                    if (id > 0)
                    {
                        DILog.UILogger.WriteLog(usr.Id, LogLevelType.Info, $" OldValue: {oldData} NewValue:{xldata.Description}");
                    }
                    else
                    {
                        DILog.UILogger.WriteLog(usr.Id, LogLevelType.Info, $"New Service Account question. {xldata.Description}");
                    }

                    DomainApiClient<Dto.ServiceAccountQuestion>.Save(xldata);
                    MasterPage.ShowAlert(AlertType.Success, successMessage);
                    BindServiceAccountQuestions();
                    serviceAccountQuestion.DataBind();
                }
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, $"{ex.Message}");
            }
        }
        protected void serviceAccountQuestion_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            UpdateOrInsert(sender, e);
        }
        protected void btnADPropagation_Click(object sender, EventArgs e)
        {
            try
            {
                long userId = 0;
                if (UserSearchUC.SelectedUser == null)
                {
                    userId = 0;
                }
                else
                {
                    userId = UserSearchUC.SelectedUser.Id;
                }
                var param = new Dictionary<string, string>();
                param.Add("UserId", userId.ToString());
                var msg = DomainApiClient<Dto.AccessRequest>.GetByAction<string>("CreateADPropagationForEmployee", param);
                MasterPage.ShowAlert(AlertType.Success, msg);
            }
            catch (System.Exception)
            {
            }
        }
        protected void btnUpdateTelephone_Click(object sender, EventArgs e)
        {
            long userId = 0;
            if (UserSearchUC.SelectedUser == null)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select user");
                return;
            }
            else
            {
                if (UserSearchUC.SelectedUser.IsActive)
                {
                    userId = UserSearchUC.SelectedUser.Id;
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "You cannot update inactive user");
                    return;
                }
            }
            if (userId > 0)
            {
                var param = new Dictionary<string, string>();
                param.Add("UserId", userId.ToString());
                var msg = DomainApiClient<Dto.AccessRequest>.GetByAction<bool>("UpdateUserTelephonenumberInHRMS", param);
                if (msg)
                {
                    var usr = $"Successfully initiated HR update in HRMS for user:{UserSearchUC.SelectedUser.DisplayName}";
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, usr);
                    MasterPage.ShowAlert(AlertType.Success, usr);
                }
            }
        }


        #region Decommission


        protected void Decommission_Click(object sender, EventArgs e)
        {
            if (EndpointAutoCompleteBox.Entries.Count == 0)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select the Application");
                return;
            }
            else
                Decommision.VisibleOnPageLoad = true;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (EndpointAutoCompleteBox.Entries.Count == 0)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select the Application");
                return;
            }
            else if (txtComment.Text == string.Empty)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please enter the Comment");
                return;
            }

            else
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Initialize to Decommission");
                Dto.PresentationSystemDecommission decommission = new Dto.PresentationSystemDecommission();

                decommission.Comments = Comments;
                decommission.EndpointSystemId = Convert.ToInt64(EndpointAutoCompleteBox.Entries[0].Value);
                decommission.Status = true;
                decommission.DecommissionedBy = sessionStatus.User.UserName;
                decommission.DecommissionedDate = DateTime.Now;
                var status = DomainApiClient<Dto.PresentationSystemDecommission>.GetByAction<string>("InsertDecommission", null, decommission);
                if (status == "1")
                {
                    MasterPage.ShowAlert(AlertType.Success, $"{EndpointAutoCompleteBox.Entries[0].Text}: Decommission is saved successfully");
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Warning, $"{EndpointAutoCompleteBox.Entries[0].Text} {status}");
                }
            }
            Decommision.VisibleOnPageLoad = false;
            ClearData();
        }

        protected void CancelradWindow_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AdminUI/List.aspx");
        }

        private void ClearData()
        {
            EndpointAutoCompleteBox.Entries.Clear();
            txtComment.Text = string.Empty;
        }

        #endregion

        protected void btnTaskWithDraw_Click(object sender, EventArgs e)
        {
            try
            {
                long taskId = 0;
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"Trying to withdra/reject the task:{txtTaskId.Text}");
                if (long.TryParse(txtTaskId.Text,out taskId) == false || taskId == 0)
                {
                    MasterPage.ShowAlert(AlertType.Error, $"Please enter valid taskId:{txtTaskId.Text}");
                    txtTaskId.Text = "";
                    return;
                }
                Dto.Presentation.PresentationTask ptask = new Dto.Presentation.PresentationTask();
                ptask.TaskId = taskId;
                ptask.DelegatedComment = txtComments.Text;

                var result = DomainApiClient<Dto.Presentation.PresentationTask>.GetByAction<string>("WitHDrawHangingtask",null, ptask); 
                if(!string.IsNullOrWhiteSpace(result) && result.StartsWith(Constants.taskStatusMessageWithdraw))
                {
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"{result} with comments:{ptask.DelegatedComment}");
                    MasterPage.ShowAlert(AlertType.Success, result);
                }
                else
                {
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, result);
                    MasterPage.ShowAlert(AlertType.Error, result);
                }
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, "Error occured. Please try again");
            }
        }
        protected void btnRequestResubmit_Click(object sender, EventArgs e)
        {
            try
            {
                long requestId = 0;
                if (long.TryParse(txtRequestId.Text, out requestId) == false || requestId == 0)
                {
                    MasterPage.ShowAlert(AlertType.Error, $"Please enter valid RequestId: :{txtRequestId.Text}");
                    txtRequestId.Text = "";
                }
                if (ddlRequestType.SelectedIndex > 0)
                {
                    RequestType requestType = RequestType.None;
                    if (ddlRequestType.SelectedValue == "1")
                    {
                        requestType = RequestType.Provisioning;
                    }
                    else if (ddlRequestType.SelectedValue == "2")
                    {
                        requestType = RequestType.BulkRequest;
                    }

                    if (long.TryParse(txtRequestId.Text, out requestId) == false)
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Please enter valid RequestId");
                        txtRequestId.Text = "";
                        return;
                    }

                    var param = new Dictionary<string, string>();
                    param.Add("requestType", ((int)requestType).ToString());
                    param.Add("requestId", requestId.ToString());
                    var result = DomainApiClient<Dto.Presentation.PresentationTask>.GetByAction<string>("ReinitiateRequest", param, null);
                    if(result == "SUCCESS")
                    {
                        var msg = $"Request:{requestId} reinitiated successfully";
                        DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, msg);
                        MasterPage.ShowAlert(AlertType.Success, msg);
                    }
                    else
                    {
                        MasterPage.ShowAlert(AlertType.Error, result??"");
                    }
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please select RequestType");
                }
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, "Error occured. Please try again");
            }
        }
        protected void btnDeactivateExpiredDelegation_Click(object sender, EventArgs e)
        {
            try
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("modifiedBy", SessionUser.User.UserName);
                var msg = DomainApiClient<Dto.TaskDelegation>.GetByAction<string>("DeactivateExpiredAdvanceDelegation", parameters);
                if (msg == Constants.DelegationExpiredMessage || msg == Constants.DelegationNoRecords)
                {
                    MasterPage.ShowAlert(AlertType.Success, msg);
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, msg);
                }
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, "Error occured. Please try again");
            }
        }

    }
}