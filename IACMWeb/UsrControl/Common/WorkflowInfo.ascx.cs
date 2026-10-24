

using DI.Chakra.Infrastructure.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class WorkflowInfo : DI.Chakra.Web.UIBaseUC
    {
        private const string V_REQUESTTYPE = "REQUESTTYPE";
        private Dto.WorkflowConfiguration mWorkflowConfiguration;


        public RequestType Requesttype
        {
            get
            {
                if (ViewState[V_REQUESTTYPE] == null)
                    return RequestType.Provisioning;

                return (RequestType)ViewState[V_REQUESTTYPE];
            }
            set
            {
                ViewState[V_REQUESTTYPE] = value;
            }
        }

        public Dto.WorkflowConfiguration WorkflowConfiguration
        {
            get
            {
                mWorkflowConfiguration = GetWorkflow();
                return mWorkflowConfiguration;
            }

            set
            {
                mWorkflowConfiguration = value;
                SetWorkflow();
            }
        }

        public Dto.Attestation Attestation { get; set; }
        
        private void SetWorkflow()
        {           
            BindWorkflow();
            BindApprovalUserType();
            DV_CreateTask.Visible = false;
            if (mWorkflowConfiguration == null)
                return;

            txtWorkflowDesc.Text = mWorkflowConfiguration.Description;
            ddlWorkflow.ClearSelection();

            var selectedWorkflow = ddlWorkflow.Items.FindByValue(mWorkflowConfiguration.Id.ToString());
            // in case the workflow has been deleted or set as inactive
            if (selectedWorkflow != null)
            {
                selectedWorkflow.Selected = true;
            }
            
            if (Attestation != null)
            {
                chkCreateTask.Checked = Attestation.CreateTaskOnNoReviewer;
                chkSendNotification.Checked = Attestation.SendEmailOnNoReviewer;

                if(chkCreateTask.Checked)
                {
                    DV_CreateTask.Visible = true;
                    if (!Attestation.IsGroupTask)
                    {
                        ddlApprovalUserType.SelectedIndex = 0;
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("id", Attestation.TaskActionUserId.ToString());
                        var user = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                            (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);
                        this.UserNameAutoCompleteBox.Entries.Clear();
                        this.UserNameAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(user.DisplayName + " [" + user.UserName + "]", user.Id.ToString()));
                    }
                    else
                    {
                        ddlApprovalUserType.SelectedIndex = 1;
                        var group = DomainApiClient<Dto.Group>.GetById(Convert.ToInt32(Attestation.TaskActionUserId));
                        ddlApprover.SelectedValue = Attestation.TaskActionUserId.ToString();
                    }
                }
            }
        }

        private Dto.WorkflowConfiguration GetWorkflow()
        {
            if (ddlWorkflow.SelectedValue != "-1")
            {
                return new Dto.WorkflowConfiguration
                {
                    Description = ddlWorkflow.SelectedItem.Text,
                    Id = Convert.ToInt16(ddlWorkflow.SelectedValue)
                };
            }
            else
                return null;
        }

        public void GetExceptionConfiguration(Dto.Attestation attestation)
        {
            attestation.CreateTaskOnNoReviewer = chkCreateTask.Checked;
            attestation.SendEmailOnNoReviewer = chkSendNotification.Checked;

            if (chkCreateTask.Checked)
            {
                if (this.ApproverType == ApprovalUserType.User)
                {
                    attestation.IsGroupTask = false;
                    attestation.TaskActionUserId = null;
                    if(this.UserNameAutoCompleteBox.Entries.Count > 0)
                        attestation.TaskActionUserId = Convert.ToInt64(this.UserNameAutoCompleteBox.Entries[0].Value);
                }
                else
                {
                    attestation.IsGroupTask = true;
                    attestation.TaskActionUserId = string.IsNullOrEmpty(ddlApprover.SelectedValue) ? Convert.ToInt64(0) : Convert.ToInt64(ddlApprover.SelectedValue);
                }
            }
        }

        public bool ValidateExceptionConfiguration(out string errorMessage)
        {
            errorMessage = "";
            if (chkCreateTask.Checked)
            {
                if (this.ApproverType == ApprovalUserType.User)
                {
                    if (this.UserNameAutoCompleteBox.Entries == null || this.UserNameAutoCompleteBox.Entries.Count == 0)
                    {
                        errorMessage = "please select the user for exception task."; 
                        return false;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(ddlApprover.SelectedValue))
                    {
                        errorMessage = "please select the group for exception task.";
                        return false;
                    }
                }
            }

            return true;
        }

        public void InitializeControl()
        {
            BindWorkflow();
            BindApprovalUserType();
            DV_CreateTask.Visible = false;
        }

        private void BindWorkflow()
        {
            Dictionary<String, string> parameter = new Dictionary<String, string>();
            parameter.Add("requestTypeId", (this.Requesttype.ToString()));

            var workflowConfigList = DomainApiClient<Dto.WorkflowConfiguration>.GetByAction<List<Dto.WorkflowConfiguration>>(DomainApiAction.Workflow.WorkflowConfiguration.GetByRequestTypeId, parameter);
            ddlWorkflow.Items.Clear();
            ddlWorkflow.DataSource = workflowConfigList;
            ddlWorkflow.DataBind();
            ddlWorkflow.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select", "-1"));
        }

        protected void ddlWorkflow_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlWorkflow.SelectedItem.Value == "-1")
                    txtWorkflowDesc.Text = string.Empty;
                else
                {
                    long id = Convert.ToInt16(ddlWorkflow.SelectedItem.Value.ToString());
                    Dictionary<String, string> parameter = new Dictionary<String, string>();
                    parameter.Add("id", id.ToString());
                    var workflowConfig = DomainApiClient<Dto.WorkflowConfiguration>.GetByAction<Dto.WorkflowConfiguration>(DomainApiAction.Workflow.WorkflowConfiguration.GetWorkflowConfigurationDetailById, parameter);
                    if (workflowConfig != null)
                        txtWorkflowDesc.Text = workflowConfig.Description;
                }
            }
            catch (Exception ex)
            {
                //NEEDTOCONFIRM
                //UIBasePage.SaveLogInfo(DI.Chakra.Logger.UIAction.Error, ex.Message);
                throw;
            }
        }        
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

        protected void ddlApprovalUserType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlApprovalUserType.SelectedValue))
                BindApprover(Convert.ToByte(ddlApprovalUserType.SelectedValue));
        }
        protected void ddlApprover_SelectedIndexChanged(object sender, EventArgs e)
        {
            
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
                        //if (requestTypes.Contains(this.RequestType.ToString()))
                        if(approverType.ApprovalUserTypeDesc == "User" || approverType.ApprovalUserTypeDesc == "Group")
                        {
                            filteredApprovalTypes.Add(approverType);
                        }
                    });
                    this.ddlApprovalUserType.DataValueField = "Id";
                    this.ddlApprovalUserType.DataTextField = "ApprovalUserTypeDesc";
                    this.ddlApprovalUserType.DataSource = filteredApprovalTypes;
                    this.ddlApprovalUserType.DataBind();
                    ddlApprovalUserType.SelectedIndex = 0;

                    if (Attestation != null && filteredApprovalTypes.Count >= 2)
                    {
                        if (Attestation.IsGroupTask)
                        {
                            ddlApprovalUserType.SelectedIndex = 1;
                        }
                        else
                        {
                    ddlApprovalUserType.SelectedIndex = 0;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(ddlApprovalUserType.SelectedValue))
                    BindApprover(Convert.ToByte(ddlApprovalUserType.SelectedValue));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void BindApprover(byte approvalUserTypeId)
        {
            try
            {
                ddlApprover.Items.Clear();
                ddlApprover.Visible = true;
                this.UserNameAutoCompleteBox.Visible = false;
                if (approvalUserTypeId == (byte)ApprovalUserType.User)
                {
                    ddlApprover.Visible = false;
                    this.UserNameAutoCompleteBox.Visible = true;
                }                
                else if (approvalUserTypeId == (byte)ApprovalUserType.Group)
                {
                    IList<Dto.Group> groupList = DomainApiClient<Dto.Group>.GetAll();
                    this.ddlApprover.DataValueField = "Id";
                    this.ddlApprover.DataTextField = "GroupName";
                    this.ddlApprover.DataSource = groupList;
                    this.ddlApprover.DataBind();                    
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

        protected void chkCreateTask_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCreateTask.Checked == true)
                DV_CreateTask.Visible = true;
            else
                DV_CreateTask.Visible = false;
        }
    }
}