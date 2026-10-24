using Application;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Web.WorkFlow.UC
{
    public partial class ApproversUC : UIBaseUC
    {
        SessionStatusImpl sessionStatus = new SessionStatusImpl();
        const string V_WFCONFIGDETAILID = "WorkflowConfigurationDetail";
        const string V_DETAILOPERATIONMODE = "DetailOperationMode";
        const string V_SEQUENCENUMBER = "SequenceNumber";
        const string V_REQUESTTYPE = "RequestType";

        public event EventHandler OnApproverSave;
        public event EventHandler OnApproverCancel;

        #region Navigation related Properties
        public Dto.WorkflowConfigurationDetail WorkflowConfigurationDetail
        {
            get
            {
                if (ViewState[V_WFCONFIGDETAILID] == null)
                    return null;
                return (WorkflowConfigurationDetail)ViewState[V_WFCONFIGDETAILID];
            }
            set
            {
                ViewState[V_WFCONFIGDETAILID] = value;
            }
        }

        public UIMode DetailOperationMode
        {
            get
            {
                if (ViewState[V_DETAILOPERATIONMODE] == null)
                    return UIMode.View;
                return (UIMode)ViewState[V_DETAILOPERATIONMODE];
            }
            set
            {
                ViewState[V_DETAILOPERATIONMODE] = value;
            }
        }

        public byte SequenceNumber
        {
            get
            {
                if (ViewState[V_SEQUENCENUMBER] == null)
                    return 0;
                return Convert.ToByte(ViewState[V_SEQUENCENUMBER]);
            }
            set
            {
                ViewState[V_SEQUENCENUMBER] = value;
            }
        }

        public byte RequestType
        {
            get
            {
                if (ViewState[V_REQUESTTYPE] == null)
                    return 0;
                return Convert.ToByte(ViewState[V_REQUESTTYPE]);
            }
            set
            {
                ViewState[V_REQUESTTYPE] = value;
            }
        }

        public bool Enable
        {
            set
            {
                MainDiv.Enabled = SaveApproverButton.Visible = value;
            }
        }
        #endregion

        #region View Properties
        ApprovalUserType ApproverType
        {
            get
            {
                return (ApprovalUserType)Convert.ToByte(ddlApprovalUserType.SelectedValue);
            }

            set
            {
                ddlApprovalUserType.SelectedValue = ((byte)value).ToString();
            }
        }

        long Approver
        {
            get
            {
                if (this.ApproverType == ApprovalUserType.User)
                {
                    return Convert.ToInt64(this.UserNameAutoCompleteBox.Entries[0].Value);
                }
                else
                {
                    long approver = string.IsNullOrEmpty(ddlApprover.SelectedValue) ? Convert.ToInt64(0) : Convert.ToInt64(ddlApprover.SelectedValue);
                    return approver;
                }
            }
            set
            {
                if (this.ApproverType == ApprovalUserType.User)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("id", value.ToString());
                    var user = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                        (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);
                    this.UserNameAutoCompleteBox.Entries.Clear();
                    this.UserNameAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(user.KnownAs, user.Id.ToString()));
                }
                else
                {
                    ddlApprover.SelectedValue = value.ToString();
                }
            }
        }

        int ResponseTime
        {
            get
            {
                if (string.IsNullOrEmpty(txtResponseTime.Text))
                    return 0;
                else
                    return Convert.ToInt32(txtResponseTime.Text);
            }
            set
            {
                txtResponseTime.Text = value.ToString();
            }
        }

        bool IsRemainderRequired
        {
            get
            {
                return rbYes.Checked;
            }
            set
            {
                rbYes.Checked = value;
                rbNo.Checked = !value;
            }
        }

        int RemainderInterval
        {
            get
            {
                if (string.IsNullOrEmpty(txtReminderInterval.Text))
                    return 0;
                else
                    return Convert.ToInt32(txtReminderInterval.Text);
            }
            set
            {
                txtReminderInterval.Text = value.ToString();
            }
        }

        bool IsEscalate
        {
            get
            {
                return rbEscalate.Checked;
            }
            set
            {
                rbEscalate.Checked = value;
                rbTerminate.Checked = !value;
            }
        }

        int EscalationDays
        {
            get
            {
                if (string.IsNullOrEmpty(txtEscalationDays.Text))
                    return 0;
                else
                    return Convert.ToInt32(txtEscalationDays.Text);
            }
            set
            {
                txtEscalationDays.Text = value.ToString();
            }
        }

        bool IsActionOnlyRequired
        {
            get
            {
                return this.rbActionYes.Checked;
            }
            set
            {
                this.rbActionYes.Checked = value;
                this.rbActionNo.Checked = !value;
            }
        }

        bool IsMockGroup
        {
            get
            {
                return this.rbMockGrpYes.Checked;
            }
            set
            {
                this.rbMockGrpYes.Checked = value;
                this.rbMockGrpNo.Checked = !value;
            }
        }

        bool RequiresApprovalFromAll
        {
            get
            {
                return this.rbRequireAllYes.Checked;
            }
            set
            {
                this.rbRequireAllYes.Checked = value;
                this.rbRequireAllNo.Checked = !value;
            }
        }
        #endregion

        #region Page & Control Events
        protected void rbYes_CheckedChanged(object sender, EventArgs e)
        {
            if (rbYes.Checked)
            {
                txtReminderInterval.Enabled = true;
                RemainderInterval = 1;
            }
            else
            {
                txtReminderInterval.Enabled = false;
                RemainderInterval = 0;
            }
        }

        protected void rbNo_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNo.Checked)
            {
                txtReminderInterval.Enabled = false;
                RemainderInterval = 0;
            }
            else
            {
                txtReminderInterval.Enabled = true;
                RemainderInterval = 1;
            }
        }

        protected void rbEscalate_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEscalate.Checked)
            {
                txtEscalationDays.Enabled = true;
            }
            else
            {
                txtEscalationDays.Enabled = false;
            }
        }

        protected void rbTerminate_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTerminate.Checked)
            {
                txtEscalationDays.Enabled = false;
            }
            else
            {
                txtEscalationDays.Enabled = true;
            }
        }

        protected void ddlApprovalUserType_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            if (!string.IsNullOrEmpty(ddlApprovalUserType.SelectedValue))
                BindApprover(Convert.ToByte(ddlApprovalUserType.SelectedValue));
        }

        protected void ddlApprover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ApproverType == ApprovalUserType.Group && this.RequestType != (byte)Infrastructure.Enums.RequestType.Attestation)
            {
                divRequiresApprovalFromAll.Visible = true;
                divIsMockGroup.Visible = false;
                IsMockGroup = false;
                RequiresApprovalFromAll = false;
            }
            else if (this.ApproverType == ApprovalUserType.UseApplicationConfiguration && this.Approver == (byte)RoleType.TCodeOwner)
            {
                divRequiresApprovalFromAll.Visible = true;
                RequiresApprovalFromAll = false;
                divIsMockGroup.Visible = false;
            }
            else if (this.ApproverType == ApprovalUserType.UseApplicationConfiguration &&
                this.Approver == (byte)RoleType.Level1Approver || this.Approver == (byte)RoleType.Level2Approver ||
                this.Approver == (byte)RoleType.Level3Approver)
            {
                divRequiresApprovalFromAll.Visible = true;
                RequiresApprovalFromAll = false;
                divIsMockGroup.Visible = false;
            }
            else
            {
                divRequiresApprovalFromAll.Visible = false;
                divIsMockGroup.Visible = false;
            }
        }

        protected void SaveApproverButton_Click(object sender, EventArgs e)
        {
            lblWarningMessage.Text = "";
            if (IsValidApprover() && CheckValidResponseTime() && CheckValidReminder() && IsResponseGreater())
            {
                this.WorkflowConfigurationDetail = this.GetDetailsFromView();
                OnApproverSave(sender, e);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            OnApproverCancel(sender, e);
        }

        protected void txtResponseTime_TextChanged(object sender, EventArgs e)
        {
            ResponseTime = txtResponseTime.Text != "" ? Convert.ToInt32(txtResponseTime.Text) : 0;
            CheckValidResponseTime();
        }

        protected void txtReminderInterval_TextChanged(object sender, EventArgs e)
        {
            RemainderInterval = txtReminderInterval.Text != "" ? Convert.ToInt32(txtReminderInterval.Text) : 0;
            CheckValidReminder();
        }
        #endregion

        #region Public Methods
        public void InitializeControl()
        {
            lblWarningMessage.Text = "";
            BindApprovalUserType();
            this.BindDetailsToView();
        }

        public void BindApprovalUserType()
        {
            try
            {
                ddlApprovalUserType.Items.Clear();
                var list = DomainApiClient<Dto.ApprovalUserTypes>.GetByAction<List<Dto.ApprovalUserTypes>>
                    (DomainApiAction.MetaData.ApprovalUserTypes.GetByIsActive, null);
                List<Dto.ApprovalUserTypes> filteredApprovalTypes = new List<Dto.ApprovalUserTypes>();
                if (list.Count > 0)
                {
                    list.ForEach(approverType =>
                    {
                        var requestTypes = approverType.ApplicableWorkFlowTypes.Split(',');
                        if (requestTypes.Contains(this.RequestType.ToString()))
                        {
                            filteredApprovalTypes.Add(approverType);
                        }
                    });
                    this.ddlApprovalUserType.DataValueField = "Id";
                    this.ddlApprovalUserType.DataTextField = "ApprovalUserTypeDesc";
                    this.ddlApprovalUserType.DataSource = filteredApprovalTypes;
                    this.ddlApprovalUserType.DataBind();

                    if (filteredApprovalTypes != null && filteredApprovalTypes.Count > 0)
                        ddlApprovalUserType.SelectedIndex = 0;
                }
                if (!string.IsNullOrEmpty(ddlApprovalUserType.SelectedValue))
                    BindApprover(Convert.ToByte(ddlApprovalUserType.SelectedValue));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Private Methods
        private void BindApprover(byte approvalUserTypeId)
        {
            try
            {
                ddlApprover.Items.Clear();
                ddlApprover.Visible = true;
                this.UserNameAutoCompleteBox.Visible = false;
                divRequiresApprovalFromAll.Visible = false;
                divIsMockGroup.Visible = false;
                if (approvalUserTypeId == (byte)ApprovalUserType.User)
                {
                    ddlApprover.Visible = false;
                    this.UserNameAutoCompleteBox.Visible = true;
                }
                else if (approvalUserTypeId == (byte)ApprovalUserType.Role || approvalUserTypeId ==
                    (byte)ApprovalUserType.UseApplicationConfiguration)
                {
                    var roleTypesList = DomainApiClient<Dto.RoleTypes>.GetByAction<List<Dto.RoleTypes>>
                        (DomainApiAction.MetaData.RoleTypes.GetByIsActive, null);
                    List<Dto.RoleTypes> filteredRoles = new List<Dto.RoleTypes>();

                    List<Dto.NOrganisationScope> lstScope = DomainApiClient<Dto.NOrganisationScope>.GetAll();

                    // Temp Fix : Featute we have to impliment the Organization unit
                    var roleTypes = from item1 in roleTypesList
                                    where !(lstScope.Any(item2 => item2.Scope == item1.DisplayName))
                                    select item1;

                    
                     foreach (Dto.RoleTypes item in roleTypes)
                    {
                        bool blnApprovalUserType = false;
                        bool blnRequestType = false;
                        if (item.ApplicableApprovalUserTypes != null)
                        {
                            var userTypes = item.ApplicableApprovalUserTypes.Split(',');
                            blnApprovalUserType = userTypes.Contains(approvalUserTypeId.ToString());
                        }
                        if (item.ApplicableRequestTypes != null)
                        {
                            var requestTypes = item.ApplicableRequestTypes.Split(',');
                            blnRequestType = requestTypes.Contains(this.RequestType.ToString());
                        }
                        if (blnApprovalUserType && blnRequestType)
                        {
                            
                            filteredRoles.Add(item);
                        }
                        //var usrname = item.DisplayName;
                        if (item.RoleTypeDesc.Contains("T-Code"))
                        {
                            divRequiresApprovalFromAll.Visible = true;
                        }
                    }
                    this.ddlApprover.DataValueField = "Id";
                    this.ddlApprover.DataTextField = "DisplayName";
                    this.ddlApprover.DataSource = filteredRoles;
                    this.ddlApprover.DataBind();
                }
                else if (approvalUserTypeId == (byte)ApprovalUserType.Group)
                {
                    IOrderedEnumerable<Group> groupList = DomainApiClient<Dto.Group>.GetAll().OrderBy(group => group.GroupName);
                    this.ddlApprover.DataValueField = "Id";
                    this.ddlApprover.DataTextField = "GroupName";
                    this.ddlApprover.DataSource = groupList;
                    this.ddlApprover.DataBind();
                    divRequiresApprovalFromAll.Visible = true;
                    // RequiresApprovalFromAll = false;
                    divIsMockGroup.Visible = false;
                    IsMockGroup = false;
                    if (this.RequestType == (byte)Infrastructure.Enums.RequestType.Attestation)
                    {
                        divRequiresApprovalFromAll.Visible = false;
                        divIsMockGroup.Visible = false;
                    }
                }
                if (ddlApprover.Items.Count != 0)
                {
                    ddlApprover.SelectedIndex = 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ClearControls()
        {
            lblWarningMessage.Text = "";
            rbNo.Checked = true;
            txtReminderInterval.Enabled = false;
            rbActionNo.Checked = true;
            rbTerminate.Checked = true;
            txtEscalationDays.Enabled = false;

            divRequiresApprovalFromAll.Visible = false;
            divIsMockGroup.Visible = false;
            rbRequireAllNo.Checked = true;
            rbMockGrpNo.Checked = true;

            this.UserNameAutoCompleteBox.Entries.Clear();
            rbYes.Checked = false;
            rbNo.Checked = true;
            txtResponseTime.Text = "1";
            txtReminderInterval.Text = "0";
            rbEscalate.Checked = false;
            rbTerminate.Checked = true;
            txtEscalationDays.Text = "1";
        }

        private WorkflowConfigurationDetail GetDetailsFromView()
        {
            WorkflowConfigurationDetail wfConfigDetail = new WorkflowConfigurationDetail()
            {
                ApprovalUserType = this.ApproverType,
                Approver = this.Approver,
                DaysToWaitAfterEscalation = this.EscalationDays,
                IsActionRequired = this.IsActionOnlyRequired,
                IsEscalationRequired = this.IsEscalate,
                IsMockGroup = false,//this.IsMockGroup,
                IsReminderRequired = this.IsRemainderRequired,
                ReminderIntervalInDays = this.RemainderInterval,
                RequiresApprovalFromAll = this.RequiresApprovalFromAll,
                ResponseTimeInDays = this.ResponseTime,
                SequenceNumber = this.SequenceNumber,
                CreatedDate = DateTime.Now
            };
            var existingDetail = this.WorkflowConfigurationDetail;
            if (existingDetail != null)
            {
                wfConfigDetail.WorkflowConfigurationId = existingDetail.WorkflowConfigurationId;
                wfConfigDetail.Id = existingDetail.Id;
            }
            else
            {
                wfConfigDetail.WorkflowConfigurationId = 0;
                wfConfigDetail.Id = 0;
            }
            return wfConfigDetail;
        }

        private void BindDetailsToView()
        {
            WorkflowConfigurationDetail wfConfigDetail;
            if (this.DetailOperationMode == UIMode.Edit)
            {
                wfConfigDetail = this.WorkflowConfigurationDetail; ;
                this.ApproverType = wfConfigDetail.ApprovalUserType;
                this.ddlApprovalUserType_SelectedIndexChanged(this, EventArgs.Empty);
                this.Approver = wfConfigDetail.Approver;
                this.EscalationDays = wfConfigDetail.DaysToWaitAfterEscalation.Value;
                this.IsActionOnlyRequired = wfConfigDetail.IsActionRequired;
                this.IsEscalate = wfConfigDetail.IsEscalationRequired;
                this.IsMockGroup = false;//wfConfigDetail.IsMockGroup;
                this.IsRemainderRequired = wfConfigDetail.IsReminderRequired;
                this.RemainderInterval = wfConfigDetail.ReminderIntervalInDays.Value;
                this.RequiresApprovalFromAll = wfConfigDetail.RequiresApprovalFromAll;
                this.ResponseTime = wfConfigDetail.ResponseTimeInDays;
                if (this.ApproverType == ApprovalUserType.UseApplicationConfiguration &&
                this.Approver == (byte)RoleType.Level1Approver || this.Approver == (byte)RoleType.Level2Approver ||
                this.Approver == (byte)RoleType.Level3Approver)
                {
                    divRequiresApprovalFromAll.Visible = true;
                    divIsMockGroup.Visible = false;
                }
            }
            else if (this.DetailOperationMode == UIMode.Add)
            {
                wfConfigDetail = null;
                this.ClearControls();
                this.ApproverType = ApprovalUserType.Role;
                this.ddlApprovalUserType_SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        private bool IsValidApprover()
        {
            lblMessage.Visible = false;
            if (Convert.ToByte(ddlApprovalUserType.SelectedValue) == (byte)ApprovalUserType.User && UserNameAutoCompleteBox.Entries.Count <= 0)
            {
                lblMessage.Visible = true;
                lblMessage.Text = "Approver is required.";
                return false;
            }
            return true;
        }

        private bool CheckValidResponseTime()
        {
            lblWarningMessage.Text = "";
            if (ResponseTime <= 0)
            {
                txtResponseTime.Text = "";
                lblWarningMessage.Text = "Please enter valid Response Time(Days)";
                return false;
            }
            if (ResponseTime == 1 && IsRemainderRequired)
            {
                SetDefaultReminderInterval();
                return false;
            }
            return true;
        }

        private bool CheckValidReminder()
        {
            if (RemainderInterval <= 0 && IsRemainderRequired)
            {
                txtReminderInterval.Text = "";
                lblWarningMessage.Text = "Please enter valid Reminder Interval(Days)";
                return false;
            }
            if (ResponseTime == 1 && IsRemainderRequired)
            {
                SetDefaultReminderInterval();
                return false;
            }
            return true;
        }

        private void SetDefaultReminderInterval()
        {
            rbYes.Checked = false;
            rbNo.Checked = true;
            txtReminderInterval.Text = "0";
            txtReminderInterval.Enabled = false;
            lblWarningMessage.Text = "Reminder is not applicable for the selected Response Time (Days)";
        }

        private bool IsResponseGreater()
        {
            if (IsRemainderRequired && RemainderInterval >= ResponseTime)
            {
                RemainderInterval = ResponseTime - 1;
                txtReminderInterval.Text = RemainderInterval.ToString();
                lblWarningMessage.Text = "Reminder Interval (Days) cannot be greater than Response Time (Days)";
                return false;
            }
            return true;
        }
        #endregion
    }
}