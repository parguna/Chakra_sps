using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.WorkFlow.UC
{
    public partial class WorkflowDetailUC : UIBaseUC
    {
        SessionStatusImpl sessionStatus = new SessionStatusImpl();
        const string V_OPERATIONMODE = "OperationMode";
        const string V_APPROVERS = "Approvers";
        const string V_SHOW = "V_SHOW";
        const string V_CLOSE = "V_CLOSE";

        #region Navigation related Properties
        public long WorkflowConfigurationId
        {
            get
            {
                if (this.ContextObjectId == null)
                    return 0;
                return (long)this.ContextObjectId.Value;
            }
        }

        public UIMode OperationMode
        {
            get
            {
                if (ViewState[V_OPERATIONMODE] == null)
                    return UIMode.View;
                return (UIMode)ViewState[V_OPERATIONMODE];
            }
            set
            {
                ViewState[V_OPERATIONMODE] = value;
            }
        }
        #endregion

        #region View Properties
        string WorkflowName
        {
            get
            {
                return txtWorkflowName.Text.Trim();
            }
            set
            {
                txtWorkflowName.Text = value;
            }
        }

        string WorkflowDescription
        {
            get
            {
                return txtWorkflowDescription.Text;
            }
            set
            {
                txtWorkflowDescription.Text = value;
            }
        }

        byte RequestTypeId
        {
            get
            {
                return Convert.ToByte(ddlRequestType.SelectedValue);
            }
            set
            {
                ddlRequestType.SelectedValue = value.ToString();
            }
        }
        string RoleType
        {
            get
            {
                return ddlEntitlementType.SelectedValue;
            }
            set
            {
                ddlEntitlementType.SelectedValue = value;
            }
        }
        long? EndpointSystemId
        {
            get
            {
                if (this.RequestTypeId == (byte)RequestType.Provisioning)
                {
                    return Convert.ToInt64(this.ApplicationTypeDropDownList.SelectedValue);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != null)
                    this.ApplicationTypeDropDownList.SelectedValue = ((long)value).ToString();
            }
        }

        List<byte> UserType
        {
            get
            {
                return this.UserTypeComboBox.CheckedItems.Select(item => Convert.ToByte(item.Value)).ToList();
            }
            set
            {
                this.UserTypeComboBox.ClearCheckedItems();
                foreach (RadComboBoxItem item in this.UserTypeComboBox.Items)
                {
                    if (value.Contains(Convert.ToByte(item.Value)))
                        item.Checked = true;
                }
            }
        }

        List<byte> AccountType
        {
            get
            {
                return this.AccountTypeComboBox.CheckedItems.Select(item => Convert.ToByte(item.Value)).ToList();
            }
            set
            {
                this.AccountTypeComboBox.ClearCheckedItems();
                foreach (RadComboBoxItem item in this.AccountTypeComboBox.Items)
                {
                    if (value.Contains(Convert.ToByte(item.Value)))
                        item.Checked = true;
                }
            }
        }

        List<byte> ActionType
        {
            get
            {
                return this.ActionTypeComboBox.CheckedItems.Select(item => Convert.ToByte(item.Value)).ToList();
            }
            set
            {
                this.ActionTypeComboBox.ClearCheckedItems();
                foreach (RadComboBoxItem item in this.ActionTypeComboBox.Items)
                {
                    if (value.Contains(Convert.ToByte(item.Value)))
                        item.Checked = true;
                }
            }
        }

        DateTime? ValidTo
        {
            get
            {
                return dtValidTo.SelectedDate;
            }
            set
            {
                //if (value == null) { dtValidTo.Clear(); return; }
                if (value <= dtValidTo.MaxDate && value >= dtValidTo.MinDate)
                    dtValidTo.SelectedDate = value;
            }
        }

        DateTime? ValidFrom
        {
            get
            {
                return dtValidFrom.SelectedDate;
            }
            set
            {
                if (value == null)
                {
                    dtValidTo.Clear();
                    return;
                }
                dtValidFrom.SelectedDate = value;
            }
        }

        bool IsActive
        {
            get
            {
                return this.IsActiveCheckBox.Checked;
            }
            set
            {
                this.IsActiveCheckBox.Checked = value;
            }
        }

        List<Dto.WorkflowConfigurationDetail> Approvers
        {
            get
            {
                if (ViewState[V_APPROVERS] == null)
                    return new List<Dto.WorkflowConfigurationDetail>();
                return (List<Dto.WorkflowConfigurationDetail>)ViewState[V_APPROVERS];
            }
            set
            {
                ViewState[V_APPROVERS] = value;
            }
        }

        List<Dto.WorkflowConfigurationDetail> DeletedApprovers
        {
            get
            {
                if (ViewState["DeletedApprovers"] == null)
                    return new List<Dto.WorkflowConfigurationDetail>(); ;
                return (List<Dto.WorkflowConfigurationDetail>)ViewState["DeletedApprovers"];
            }
            set
            {
                ViewState["DeletedApprovers"] = value;
            }
        }


        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (this.OperationMode == UIMode.View || this.OperationMode == UIMode.Edit || this.OperationMode == UIMode.Clone)
                {
                    if (this.ContextObjectId == null)
                        return;
                }
                BindRequestType();
                this.BindDetailsToView();
                if (this.OperationMode == UIMode.View)
                {
                    this.MainDiv.Enabled = this.ApproverDetailControl.Enable = false;
                    this.AddApproverLinkButton.Visible = false;
                }
            }
            this.ApproverDetailControl.RequestType = this.RequestTypeId;  // this line should be removed
            this.ApproverDetailControl.OnApproverSave += ApproverSave_Click;
            this.ApproverDetailControl.OnApproverCancel += ApproverCancel_Click;
            this.MasterPage.ActionClicked += ActionClicked;
        }

        protected void ApproversGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                GridColumn DeleteColumn = ApproversGrid.MasterTableView.GetColumn("DeleteColumn");
                if (DeleteColumn != null && this.OperationMode == UIMode.View)
                    DeleteColumn.Visible = false;
            }
        }


        protected void ApproversGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "EditApprover")
            {
                long detailId = Convert.ToInt64(e.CommandArgument);
                var detail = this.Approvers.FirstOrDefault(item => item.Id == detailId);
                this.ApproverDetailControl.DetailOperationMode = UIMode.Edit;
                this.ApproverDetailControl.WorkflowConfigurationDetail = detail;
                this.ApproverDetailControl.RequestType = this.RequestTypeId;
                this.ApproverDetailControl.SequenceNumber = detail.SequenceNumber;
                this.ApproverDetailControl.InitializeControl();
                ShowWindow();
            }
            else if (e.CommandName == "Delete")
            {
                GridDataItem row = e.Item as GridDataItem;
                int sequenceNumber = (int)Convert.ChangeType(row.GetDataKeyValue("SequenceNumber"), typeof(int));

                var existingDetails = this.Approvers;

                var itemToRemove = existingDetails.FirstOrDefault(item => item.SequenceNumber == sequenceNumber);

                if (itemToRemove != null)
                {
                    existingDetails.Remove(itemToRemove);
                    if (this.OperationMode == UIMode.Edit)
                    {
                        var deletedApprovers = this.DeletedApprovers;
                        deletedApprovers.Add(itemToRemove);
                        this.DeletedApprovers = deletedApprovers;
                    }
                }
                // if (this.OperationMode == UIMode.Add)
                existingDetails = GetFinalApproversWithSequence(existingDetails);
                this.Approvers = existingDetails;
                this.ApproversGrid.Rebind();
                ShowAlertMessage(AlertType.Success, "Approver removed successfully");
            }
        }

        protected void AddApproverLinkButton_Click(object sender, EventArgs e)
        {
            this.ApproverDetailControl.DetailOperationMode = UIMode.Add;
            this.ApproverDetailControl.WorkflowConfigurationDetail = null;
            this.ApproverDetailControl.RequestType = this.RequestTypeId;

            List<WorkflowConfigurationDetail> existingDetails = this.Approvers;
            int seq = 0;
            if (existingDetails.Any<WorkflowConfigurationDetail>())
                seq = existingDetails.Max<WorkflowConfigurationDetail>(s => s.SequenceNumber);

            this.ApproverDetailControl.SequenceNumber = (byte)(seq + 1);
            this.ApproverDetailControl.InitializeControl();
            ShowWindow();
        }

        void ShowWindow()
        {
            var key = "keyshowApproverWindow";
            string script = "Sys.Application.remove_load(showApproverWindow);Sys.Application.add_load(showApproverWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        void CloseWindow()
        {
            var key = "keycloseApproverWindow";
            string script = "Sys.Application.remove_load(closeApproverWindow);Sys.Application.add_load(closeApproverWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        protected void ddlRequestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            byte requestType = Convert.ToByte(this.ddlRequestType.SelectedValue);
            SetProvisioningDropDownsVisibility(requestType);
        }

        #region DropDown Population

        private void BindRequestType()
        {
            try
            {
                ddlRequestType.Items.Clear();
                var rqstTypes = DomainApiClient<Dto.RequestTypes>.GetByAction<List<Dto.RequestTypes>>
                    (DomainApiAction.MetaData.RequestTypes.GetByIsActive, null);
                this.ddlRequestType.DataValueField = "Id";
                this.ddlRequestType.DataTextField = "RequestTypeDesc";
                this.ddlRequestType.DataSource = rqstTypes;
                this.ddlRequestType.DataBind();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetProvisioningDropDownsVisibility(byte requestType)
        {
            if (requestType == (byte)RequestType.Provisioning)
            {
                this.ApplicationTypeDropDownList.Enabled = true;
                this.UserTypeComboBox.Enabled = true;
                this.AccountTypeComboBox.Enabled = true;
                this.ActionTypeComboBox.Enabled = true;
                this.PopulateProvisioningDropDowns();
            }
            else
            {
                this.ApplicationTypeDropDownList.Enabled = false;
                this.UserTypeComboBox.Enabled = false;
                this.AccountTypeComboBox.Enabled = false;
                this.ActionTypeComboBox.Enabled = false;
                this.ApplicationTypeDropDownList.Items.Clear();
            }
        }

        void populateApplicationTypeDropDown()
        {
            try
            {
                this.ApplicationTypeDropDownList.Items.Clear();

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("ignoreTrustedSource", Boolean.TrueString);
                Dictionary<long, string> endpoints = new Dictionary<long, string>();
                endpoints = DomainApiClient<Dto.EndPoint>.GetByAction<Dictionary<long, string>>
                    (DomainApiAction.Endpoint.EndPoint.GetByTrustedSource, parameter);

                byte requestType = Convert.ToByte(this.ddlRequestType.SelectedValue);

                endpoints = DomainApiClient<Dto.EndPoint>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Endpoint.EndPoint.GetByTrustedSource, parameter);

                this.ApplicationTypeDropDownList.DataValueField = "Key";
                this.ApplicationTypeDropDownList.DataTextField = "Value";
                this.ApplicationTypeDropDownList.DataSource = endpoints;
                this.ApplicationTypeDropDownList.DataBind();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void PopulateProvisioningDropDowns()
        {
            try
            {
                this.ApplicationTypeDropDownList.Items.Clear();
                this.UserTypeComboBox.Items.Clear();
                this.AccountTypeComboBox.Items.Clear();
                this.ActionTypeComboBox.Items.Clear();

                this.populateApplicationTypeDropDown();

                this.UserTypeComboBox.DataValueField = "Id";
                this.UserTypeComboBox.DataTextField = "EmployeeTermDesc";
                this.UserTypeComboBox.DataSource = DomainApiClient<Dto.EmployeeTerm>.GetByAction<List<EmployeeTerm>>
                    (DomainApiAction.MetaData.EmployeeTerm.GetByIsActive, null);
                this.UserTypeComboBox.DataBind();
                this.UserTypeComboBox.Items.Insert(0, new RadComboBoxItem() { Text = "All", Value = "0" });
                this.UserTypeComboBox.Items.FindItemByText("All").Checked = true;

                this.AccountTypeComboBox.DataValueField = "Id";
                this.AccountTypeComboBox.DataTextField = "AccountTypeDesc";
                this.AccountTypeComboBox.DataSource = DomainApiClient<Dto.AccountCategory>.GetByAction<List<AccountCategory>>
                    (DomainApiAction.MetaData.AccountCategory.GetByIsActive, null);
                this.AccountTypeComboBox.DataBind();
                this.AccountTypeComboBox.Items.Insert(0, new RadComboBoxItem() { Text = "All", Value = "0" });
                this.AccountTypeComboBox.Items.FindItemByText("All").Checked = true;

                this.ActionTypeComboBox.DataValueField = "Id";
                this.ActionTypeComboBox.DataTextField = "AccountActionTypeDesc";
                this.ActionTypeComboBox.DataSource = DomainApiClient<Dto.AccountActionCategory>.GetByAction<List<AccountActionCategory>>
                    (DomainApiAction.MetaData.AccountActionCategory.GetByIsActive, null);
                this.ActionTypeComboBox.DataBind();
                this.ActionTypeComboBox.Items.Insert(0, new RadComboBoxItem() { Text = "All", Value = "0" });
                this.ActionTypeComboBox.Items.FindItemByText("All").Checked = true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        private Dto.WorkflowConfiguration GetDetailsFromView()
        {
            Dto.WorkflowConfiguration wfConfig = new Dto.WorkflowConfiguration()
            {
                ApplicableUserTypes = this.GetFormattedStringFromList(this.UserType),
                ApplicableAccountTypes = this.GetFormattedStringFromList(this.AccountType),
                ApplicableActionTypes = this.GetFormattedStringFromList(this.ActionType),
                EndpointSystemId = this.EndpointSystemId,
                Description = this.WorkflowDescription,
                IsActive = this.IsActive,
                RequestTypeId = this.RequestTypeId,
                Id = this.WorkflowConfigurationId,
                WorkflowName = this.WorkflowName,
                ValidFrom = this.ValidFrom.HasValue ? this.ValidFrom.Value : DateTime.Now,
                ValidTo = this.ValidTo.HasValue ? this.ValidTo.Value : DateTime.Now
            };
            if (divRoleType.Visible)
            {
                if (ddlEntitlementType.SelectedIndex > 0)
                {
                    wfConfig.EntitlementType = this.RoleType;
                }
            }
            if (this.OperationMode == UIMode.Clone)
            {
                wfConfig.Id = 0;
            }
            return wfConfig;
        }

        private void BindDetailsToView()
        {
            Dto.WorkflowConfiguration wfConfig;
            parentDelimitedDiv.Visible = false;
            if (this.OperationMode == UIMode.Edit || this.OperationMode == UIMode.View || OperationMode == UIMode.Clone)
            {
                Dictionary<String, string> parameter = new Dictionary<String, string>();
                parameter.Add("id", this.WorkflowConfigurationId.ToString());
                wfConfig = DomainApiClient<Dto.WorkflowConfiguration>.GetByAction<Dto.WorkflowConfiguration>
                    (DomainApiAction.Workflow.WorkflowConfiguration.GetWorkflowConfigurationDetailById, parameter);

                this.Approvers = wfConfig.WorkflowConfigurationDetails;
                this.WorkflowName = wfConfig.WorkflowName;
                this.WorkflowDescription = wfConfig.Description;
                this.IsActive = wfConfig.IsActive;
                this.RequestTypeId = (byte)wfConfig.RequestTypeId;
                this.ddlRequestType.Enabled = false;
                if (this.OperationMode == UIMode.Edit || this.OperationMode == UIMode.Clone)
                    SetProvisioningDropDownsVisibility(Convert.ToByte(ddlRequestType.SelectedValue));
                else
                    PopulateProvisioningDropDowns();
                if (wfConfig.Endpoint != null && OperationMode != UIMode.Clone)
                    this.EndpointSystemId = (long)wfConfig.Endpoint.Id;
                else
                {
                    if (Convert.ToByte(ddlRequestType.SelectedValue) == (byte)RequestType.Attestation)
                        this.ApplicationTypeDropDownList.Items.Clear();
                }
                this.UserType = this.GetListFromFormattedString(wfConfig.ApplicableUserTypes);
                this.AccountType = this.GetListFromFormattedString(wfConfig.ApplicableAccountTypes);
                this.ActionType = this.GetListFromFormattedString(wfConfig.ApplicableActionTypes);
                this.ValidFrom = wfConfig.ValidFrom;
                this.ValidTo = wfConfig.ValidTo;
                if (this.EndpointSystemId > 0)
                {
                    var endPoint = DomainApiClient<Dto.EndPoint>.GetById(this.EndpointSystemId.Value);
                    if (endPoint != null && endPoint.ApplicationType == ApplicationType.MicrosoftTeams)
                    {
                        LoadRoleType();
                        DisplayEntitlementType(true);
                        if (!string.IsNullOrWhiteSpace(wfConfig.EntitlementType))
                        {
                            this.RoleType = wfConfig.EntitlementType;
                        }
                        else
                        {
                            this.RoleType = ((int)EntitlementType.Default).ToString();
                        }
                    }
                }
                if (OperationMode == UIMode.Clone)
                {
                    if (this.Approvers != null)
                    {
                        this.Approvers.ForEach(approver =>
                        {
                            approver.Id = 0;
                            approver.WorkflowConfigurations = null;
                            approver.WorkflowConfigurationId = 0;
                        });
                    }

                    this.WorkflowName = string.Empty;
                    this.WorkflowDescription = string.Empty;
                    this.ValidFrom = DateTime.Today;
                    this.ValidTo = dtValidTo.MaxDate;
                    parentDelimitedDiv.Visible = true;
                }

                this.ApproversGrid.Rebind();
            }
            else if (this.OperationMode == UIMode.Add)
            {
                wfConfig = null;
                this.Approvers = new List<Dto.WorkflowConfigurationDetail>();
                this.RequestTypeId = (byte)RequestType.Attestation;
                this.ddlRequestType_SelectedIndexChanged(this, EventArgs.Empty);
                this.IsActiveCheckBox.Checked = true;
                this.ApproversGrid.Rebind();
                this.ValidFrom = DateTime.Now.AddDays(1);
                this.ValidTo = dtValidTo.MaxDate;
            }
            // if (this.OperationMode == UIMode.Edit)
            //   SetProvisioningDropDownsVisibility(Convert.ToByte(ddlRequestType.SelectedValue));
        }

        private string GetFormattedStringFromList(List<byte> list)
        {
            string returnValue = string.Empty;
            foreach (byte item in list)
            {
                returnValue += item.ToString().PadLeft(3, '0') + ",";
            }
            return returnValue.TrimEnd(',');
        }

        private List<byte> GetListFromFormattedString(string formattedString)
        {
            List<byte> returnValue = new List<byte>();
            if (!string.IsNullOrEmpty(formattedString))
            {
                var stringList = formattedString.Split(',').ToList();

                foreach (string item in stringList)
                {
                    returnValue.Add(Convert.ToByte(item));
                }
            }
            return returnValue;
        }

        private bool SaveWorkflowConfiguration()
        {
            bool bResult = true;
            try
            {
                var usr = new SessionStatusImpl().User;
                var action = $"{this.OperationMode.ToString()} WorkFlow ;";
                var wfConfig = this.GetDetailsFromView();


                if (this.IsValid(wfConfig))
                {
                    if (this.OperationMode == UIMode.Add || this.OperationMode == UIMode.Clone)
                    {
                        wfConfig.WorkflowConfigurationDetails = this.Approvers;
                        if (this.OperationMode == UIMode.Clone && ParentDelimitedCheckBox.Checked)
                        {
                            wfConfig.ValidFrom = DateTime.Today.AddDays(1);
                            DomainApiClient<Dto.WorkflowConfiguration>.Save(wfConfig);

                            Dictionary<String, string> parameter = new Dictionary<String, string>();
                            parameter.Add("id", this.WorkflowConfigurationId.ToString());
                            Dto.WorkflowConfiguration parentwfConfig = DomainApiClient<Dto.WorkflowConfiguration>.GetById(this.WorkflowConfigurationId);
                            parentwfConfig.ValidTo = wfConfig.ValidFrom.AddDays(-1);
                            DomainApiClient<Dto.WorkflowConfiguration>.Save(parentwfConfig);
                        }
                        else
                        {
                            DomainApiClient<Dto.WorkflowConfiguration>.Save(wfConfig);
                        }
                        DILog.UILogger.WriteLog(usr.Id,usr.DisplayName, LogLevelType.Info, $"{action} WorkflowConfiguration Name : {wfConfig.WorkflowName}  - Saved",  null);
                    }
                    else if (this.OperationMode == UIMode.Edit)
                    {
                        Dictionary<String, string> parameter = new Dictionary<String, string>();
                        parameter.Add("id", this.WorkflowConfigurationId.ToString());
                        var existingConfig = DomainApiClient<Dto.WorkflowConfiguration>.GetByAction<Dto.WorkflowConfiguration>
                            (DomainApiAction.Workflow.WorkflowConfiguration.GetWorkflowConfigurationDetailById, parameter);

                        var details = this.Approvers;

                        if (existingConfig.IsActive == true && wfConfig.IsActive == false)
                        {
                            Dictionary<String, string> aparameter = new Dictionary<String, string>();
                            aparameter.Add("workflowid", this.WorkflowConfigurationId.ToString());
                            bool isExists = DomainApiClient<Dto.Attestation>.GetByAction<bool>
                                (DomainApiAction.Attestation.IsLinkedWithAtestation, aparameter);
                            if (isExists)
                            {
                                MasterPage.ShowAlert(AlertType.Warning, "Workflow is linked with Attestion, Can't make inactive. ");

                            }
                        }
                        parameter = new Dictionary<String, string>();
                        parameter.Add("id", this.WorkflowConfigurationId.ToString());
                        var exisitingDetails = DomainApiClient<Dto.WorkflowConfiguration>.GetByAction<WorkflowConfiguration>
                            (DomainApiAction.Workflow.WorkflowConfiguration.GetWorkflowConfigurationDetailById, parameter);
                        if(exisitingDetails != null && exisitingDetails.WorkflowConfigurationDetails != null)
                        {
                            foreach (Dto.WorkflowConfigurationDetail item in exisitingDetails.WorkflowConfigurationDetails)
                            {
                                var approverName = item.Approver.ToString();
                                if (item.ApprovalUserType == ApprovalUserType.User)
                                {
                                    approverName = DomainApiClient<Dto.User>.GetById(item.Approver)?.DisplayName;
                                }
                                else if (item.ApprovalUserType == ApprovalUserType.Group)
                                {
                                    approverName = DomainApiClient<Dto.Group>.GetById(item.Approver)?.GroupName;
                                }
                                else if (item.ApprovalUserType == ApprovalUserType.Role || item.ApprovalUserType == ApprovalUserType.UseApplicationConfiguration)
                                {
                                    approverName = ((RoleType)item.Approver).ToString();
                                }
                                DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, LogLevelType.Info, $"Old Values: {action} WorkflowConfiguration Name : {exisitingDetails.WorkflowName}, Description:{exisitingDetails.Description} ; WorkflowConfiguration Detail - Approver Type :{item.ApprovalUserType.ToString()}, Approver :{approverName}, Sequence :{item.SequenceNumber}, IsReminderRequired: { item.IsReminderRequired}, ReminderIntervalInDays: { item.ReminderIntervalInDays}, RequiresApprovalFromAll: { item.RequiresApprovalFromAll}, ResponseTimeInDays: { item.ResponseTimeInDays}, IsEscalationRequired: { item.IsEscalationRequired}, IsActionRequired: {item.IsActionRequired}, DaysToWaitAfterEscalation:{item.DaysToWaitAfterEscalation}.", null);
                            }
                        }

                        DomainApiClient<Dto.WorkflowConfiguration>.Save(wfConfig);

                        
                        var deletedApprovers = this.DeletedApprovers;

                        foreach (Dto.WorkflowConfigurationDetail item in deletedApprovers)
                        {
                            var approverName = item.Approver.ToString();
                            if(item.ApprovalUserType == ApprovalUserType.User)
                            {
                                approverName = DomainApiClient<Dto.User>.GetById(item.Approver)?.DisplayName;
                            }
                            else if (item.ApprovalUserType == ApprovalUserType.Group)
                            {
                                approverName = DomainApiClient<Dto.Group>.GetById(item.Approver)?.GroupName;
                            }
                            else if (item.ApprovalUserType == ApprovalUserType.Role || item.ApprovalUserType == ApprovalUserType.UseApplicationConfiguration)
                            {
                                approverName = ((RoleType)item.Approver).ToString();
                            }
                            
                            DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, LogLevelType.Info, $"{action} WorkflowConfiguration Name : {wfConfig.WorkflowName} ; WorkflowConfiguration Detail - Approver Type :{item.ApprovalUserType.ToString()}, Approver :{approverName}, Sequence :{item.SequenceNumber} is deleted. "
                                , null);
                            if (item.Id != 0)
                            {
                                DomainApiClient<Dto.WorkflowConfigurationDetail>.DeleteById(item.Id);
                            }
                        }

                        foreach (Dto.WorkflowConfigurationDetail item in details)
                        {
                            var approverName = item.Approver.ToString();
                            if (item.ApprovalUserType == ApprovalUserType.User)
                            {
                                approverName = DomainApiClient<Dto.User>.GetById(item.Approver)?.DisplayName;
                            }
                            else if (item.ApprovalUserType == ApprovalUserType.Group)
                            {
                                approverName = DomainApiClient<Dto.Group>.GetById(item.Approver)?.GroupName;
                            }
                            else if (item.ApprovalUserType == ApprovalUserType.Role || item.ApprovalUserType == ApprovalUserType.UseApplicationConfiguration)
                            {
                                approverName = ((RoleType)item.Approver).ToString();
                            }
                            DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, LogLevelType.Info, $"New Values: {action} WorkflowConfiguration Name : {wfConfig.WorkflowName} ; WorkflowConfiguration Detail - Approver Type :{item.ApprovalUserType.ToString()}, Approver :{approverName}, Sequence :{item.SequenceNumber}, IsReminderRequired: { item.IsReminderRequired}, ReminderIntervalInDays: { item.ReminderIntervalInDays}, RequiresApprovalFromAll: { item.RequiresApprovalFromAll}, ResponseTimeInDays: { item.ResponseTimeInDays}, IsEscalationRequired: { item.IsEscalationRequired}, IsActionRequired: {item.IsActionRequired}, DaysToWaitAfterEscalation:{item.DaysToWaitAfterEscalation} is saved.", null);
                            item.WorkflowConfigurationId = wfConfig.Id;
                            DomainApiClient<Dto.WorkflowConfigurationDetail>.Save(item);
                        }
                        DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, LogLevelType.Info, $"{action} WorkflowConfiguration Name : {wfConfig.WorkflowName}  - Saved", null);

                    }
                    return bResult;
                }
                else
                {
                    return !bResult;
                }
            }
            catch (Exception ex)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Exception, "WorkflowConfiguration - Save", ex);
                return !bResult;
            }
        }

        private bool IsValid(WorkflowConfiguration wfConfig)
        {
            if (string.IsNullOrEmpty(wfConfig.WorkflowName))
            {
                ShowAlertMessage(AlertType.Error, "Please enter Workflow Name.");
                return false;
            }
            if (wfConfig.RequestTypeId == (byte)RequestType.Provisioning)
            {
                if (string.IsNullOrEmpty(wfConfig.ApplicableUserTypes) || string.IsNullOrEmpty(wfConfig.ApplicableAccountTypes)
                              || string.IsNullOrEmpty(wfConfig.ApplicableActionTypes))
                {
                    ShowAlertMessage(AlertType.Error, "Please select applicable User / Account / Action Type .");
                    return false;
                }

                if (this.divRoleType.Visible && string.IsNullOrWhiteSpace(wfConfig.EntitlementType))
                {
                    ShowAlertMessage(AlertType.Error, "Please select Entitlement Type.");
                    return false;
                }
                bool isExists = DomainApiClient<Dto.WorkflowConfiguration>.GetByAction<bool>(DomainApiAction.Workflow.WorkflowConfiguration.DoesExists, null, wfConfig);
                if (isExists)
                {
                    ShowAlertMessage(AlertType.Error, "Workflow configuration already exists.");
                    return false;
                }
            }

            if (this.Approvers.Count == 0)
            {
                ShowAlertMessage(AlertType.Warning, "Please add an approver before saving.");
                return false;
            }

            if (!ValidFrom.HasValue || !ValidTo.HasValue)
            {
                ShowAlertMessage(AlertType.Error, "Valid From/To should not be empty");
                return false;
            }

            if (ValidTo < DateTime.Today || ValidFrom > ValidTo)
            {
                ShowAlertMessage(AlertType.Error, "Valid To should not be earlier than Valid From");
                return false;
            }


            Dictionary<String, string> parameter = new Dictionary<String, string>();
            parameter.Add("workflowName", this.WorkflowName);
            var wfConfigs = DomainApiClient<Dto.WorkflowConfiguration>.GetByAction<List<Dto.WorkflowConfiguration>>
                (DomainApiAction.Workflow.WorkflowConfiguration.GetByWorkflowName, parameter);

            foreach (Dto.WorkflowConfiguration item in wfConfigs)
            {
                if (item.Id != this.WorkflowConfigurationId)
                {
                    ShowAlertMessage(AlertType.Error, string.Format("Workflow Name ({0}) already exists.", this.WorkflowName));
                    return false;
                }
            }

            return true;
        }

        private void BindApproverGrid()
        {
            Dictionary<String, string> parameter = new Dictionary<String, string>();
            //parameter.Add("workflowConfigurationDetailList", this.Approvers.ToString());
            //parameter.Add("id", this.WorkflowConfigurationId.ToString());
            this.ApproversGrid.DataSource = DomainApiClient<Dto.Presentation.PresentationWorkflowApproverGridData>.
                GetByAction<List<Dto.Presentation.PresentationWorkflowApproverGridData>>
                (DomainApiAction.Workflow.Presentation.PresentationWorkflowApproverGridData.GetApproversList, null, this.Approvers);
        }

        private void ApproverSave_Click(object sender, EventArgs e)
        {
            CloseWindow();
            var detail = this.ApproverDetailControl.WorkflowConfigurationDetail;
            var existingDetails = this.Approvers;
            if (existingDetails.Any(item => item.Approver == detail.Approver && item.ApprovalUserType == detail.ApprovalUserType && (this.ApproverDetailControl.DetailOperationMode == UIMode.Edit ? item.SequenceNumber != detail.SequenceNumber : 1 == 1)))
            {
                MasterPage.ShowAlert(AlertType.Warning, "Same Approver Already Exists");
                return;
            }
            if (Convert.ToInt32(detail.DaysToWaitAfterEscalation) <= 0)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Zero is not a valid escalation period");
                return;
            }
            if (existingDetails.Any(item => item.SequenceNumber == detail.SequenceNumber))
            {
                var itemToRemove = existingDetails.FirstOrDefault(item => item.SequenceNumber == detail.SequenceNumber);
                existingDetails.Remove(itemToRemove);
                existingDetails.Add(detail);
            }
            else
            {
                existingDetails.Add(detail);
                MasterPage.ShowAlert(AlertType.Success, "Approver Added Successfully");
            }

            this.Approvers = existingDetails;
            this.ApproversGrid.Rebind();
        }

        /// <summary>
        ///  Method to generate the sequence of approver's List
        /// </summary>
        /// <param name="existingDetails"> Approver's List</param>
        /// <returns>Approver's List</returns>
        private List<WorkflowConfigurationDetail> GetFinalApproversWithSequence(List<WorkflowConfigurationDetail> existingDetails)
        {
            if (existingDetails != null)
            {
                int seq = 1;
                List<WorkflowConfigurationDetail> finalApprovers = new List<WorkflowConfigurationDetail>();
                foreach (WorkflowConfigurationDetail detail in existingDetails)
                {
                    detail.SequenceNumber = (byte)seq;
                    finalApprovers.Add(detail);
                    seq = seq + 1;
                }
                return finalApprovers;
            }
            return null;

        }

        private void ApproverCancel_Click(object sender, EventArgs e)
        {
            CloseWindow();
        }

        private void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                if (SaveWorkflowConfiguration())
                {
                    this.ContextObjectId = null;
                    Response.Redirect("~/Workflow/List.aspx");
                }
            }
        }

        protected void ApproversGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindApproverGrid();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Workflow/List.aspx");
        }

        protected void ComboBox_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBox comboBox = (RadComboBox)sender;
            string id = comboBox.ID;
            int checkedCount = comboBox.CheckedItems.Count();
            if (checkedCount == comboBox.Items.Count || (comboBox.Items.FindItemByText("All").Checked == false && (checkedCount == comboBox.Items.Count - 1)))
            {
                comboBox.ClearCheckedItems();
                comboBox.Items.FindItemByText("All").Checked = true;
            }
            else if (checkedCount > 1 && comboBox.Items.FindItemByText("All").Checked == true)
            {
                comboBox.Items.FindItemByText("All").Checked = false;
            }
        }

        protected void ddlEntitlementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.RoleType = ddlEntitlementType.SelectedValue;
        }

        protected void ApplicationTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayEntitlementType();
            if (ApplicationTypeDropDownList.SelectedIndex > 0 && this.RequestTypeId == (int)RequestType.Provisioning)
            {
                var endpointId = Convert.ToInt32(ApplicationTypeDropDownList.SelectedValue);
                var endpoint = DomainApiClient<Dto.EndPoint>.GetById(endpointId);
                if (endpoint != null && endpoint.ApplicationType == ApplicationType.MicrosoftTeams)
                {
                    DisplayEntitlementType(true);
                    LoadRoleType();
                }
            }
        }
        private void LoadRoleType()
        {
            ddlEntitlementType.Items.Clear();
            ddlEntitlementType.Items.Add(new ListItem(EntitlementType.None.ToString(), ((int)EntitlementType.None).ToString()));
            ddlEntitlementType.Items.Add(new ListItem(EntitlementType.Default.ToString(), ((int)EntitlementType.Default).ToString()));
            ddlEntitlementType.Items.Add(new ListItem(EntitlementType.DCC.ToString(), ((int)EntitlementType.DCC).ToString()));
            ddlEntitlementType.Items.Add(new ListItem(EntitlementType.DCCS.ToString(), ((int)EntitlementType.DCCS).ToString()));
            ddlEntitlementType.Items.Add(new ListItem(EntitlementType.IDD.ToString(), ((int)EntitlementType.IDD).ToString()));
            ddlEntitlementType.Items.Add(new ListItem(EntitlementType.Move.ToString(), ((int)EntitlementType.Move).ToString()));
            ddlEntitlementType.Items.Add(new ListItem(EntitlementType.ResponseGroup.ToString(), ((int)EntitlementType.ResponseGroup).ToString()));
            ddlEntitlementType.SelectedIndex = 1;
        }
        private void DisplayEntitlementType(bool show = false)
        {
            if (show)
            {
                divRoleType.Visible = true;
            }
            else
            {
                divRoleType.Visible = false;
            }
        }
    }
}
