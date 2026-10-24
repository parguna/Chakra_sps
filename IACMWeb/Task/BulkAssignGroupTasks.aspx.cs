using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Task
{
    public partial class BulkAssignGroupTasks : DI.Chakra.Web.UIBasePage, IContextObjectEnabled
    {
        private const string V_GROUPID = "V_GROUPID";

        SessionStatusImpl m_session = new SessionStatusImpl();
        string TaskStatus
        {
            get
            {
                return StatusType.Pending.ToString();
            }
        }

        public long GroupId
        {
            get
            {
                if (ViewState[V_GROUPID] == null)
                    return 0;
                return (long)ViewState[V_GROUPID];
            }
            set
            {
                ViewState[V_GROUPID] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (m_session.User == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            if (!IsPostBack)
            {
                UserNameAutoCompleteBox.Enabled = false;
                m_session.RedirectFrom = "GroupTask";
                //BindGroupTasks();
                BindGroups();
                var grpId = Request.QueryString["groupId"];
                if (!string.IsNullOrWhiteSpace(grpId))
                {
                    try
                    {
                        ddlGroups.Items.FindByValue(grpId).Selected = true;
                        ddlGroupsIndexChanged(ddlGroups, null);
                    }
                    catch
                    {
                    }
                }
            }
        }
        protected void grdTask_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "ViewTask")
            {
                long taskId = Convert.ToInt64(e.CommandArgument);
                var task = DomainApiClient<Dto.Task>.GetById(taskId);

                if (task.RequestTypeId == (byte)RequestType.Attestation)
                {
                    Response.Redirect("~/Attestation/AttestationReviewPage.aspx?Id=" + e.CommandArgument);
                }
                else if (task.RequestTypeId == (byte)RequestType.Provisioning)
                {
                    if (task.AccessRequestId.HasValue)
                    {
                        Response.Redirect("~/AccessRequest/TaskDetail.aspx?Id=" + e.CommandArgument);
                    }
                }
            }

            if (e?.Item != null)
            {
                GridDataItem g = e.Item as GridDataItem;
                if (g != null)
                {
                    this.TypeofRequest = Convert.ToString(g.GetDataKeyValue("RequestType"));
                }
            }
        }
        protected void grdTask_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindGroupTasks();
        }
        private void BindGroupTasks()
        {
            if (GroupId == 0)
            {
                grdTask.DataSource = new List<Dto.Presentation.PresentationTask>();
                return;
            }

            btnSubmit.Visible = false;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("actionUserId", m_session.User.Id.ToString());
            parameter.Add("taskStatus", TaskStatus);
            parameter.Add("groupId", GroupId.ToString());
            IList<Dto.Presentation.PresentationTask> taskDetailsList = DomainApiClient<Dto.Presentation.PresentationTask>.GetByAction<List<Dto.Presentation.PresentationTask>>
                (DomainApiAction.Tasks.Presentation.PresentationTask.GetTaskDetailsByActionUserIdAndTaskStatusForBulkAssign, parameter);
            var adhocRequest = new AdHocAccessRequest();
            foreach (var task in taskDetailsList)
            {
                adhocRequest.ApplicationType = task.ApplicationType;
                adhocRequest.EntitlementDetail = task.EntitlementXML;
                task.Entitlement = adhocRequest.DeserializeEntitlement();
            }
            //taskFacade.GetTaskDetailsByActionUserIdAndTaskStatusForBulkAssign(m_session.User.UserId, StatusType.Pending, true);
            if (taskDetailsList != null && taskDetailsList.Count > 0)
            {
                grdTask.DataSource = taskDetailsList;
                //grdTask.DataBind();
                //if (taskDetailsList[0].AdHocAccessRequestId.HasValue && (!string.IsNullOrEmpty(taskDetailsList[0].RequestedBy)))
                //    grdTask.MasterTableView.GetColumn("RequestedBy").Display = true;
                //else
                //    grdTask.MasterTableView.GetColumn("RequestedBy").Display = false;
            }
            else
            {
                grdTask.DataSource = new string[] { };
                //grdTask.DataBind();
            }
        }

        protected void UserNameAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            btnSubmit.Visible = false;
        }

        protected void UserNameAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            if (UserNameAutoCompleteBox.Entries.Count > 1)
                UserNameAutoCompleteBox.Entries.RemoveAt(1);
            btnSubmit.Visible = true;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                bool isItemSelected = false;
                List<long> selectedTaskIds = new List<long>();
                if (UserNameAutoCompleteBox.Entries.Count == 0)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please select Assign To.");
                    return;
                }
                if (grdTask.MasterTableView.Items.Count == 0)
                {
                    MasterPage.ShowAlert(AlertType.Error, "No task to be assigned.");
                    return;
                }
                var usrId = Convert.ToInt64(UserNameAutoCompleteBox.Entries[0].Value);
                var usr = DomainApiClient<Dto.User>.GetById(usrId);
                foreach (GridDataItem item in grdTask.MasterTableView.Items)
                {
                    CheckBox chkbx = (CheckBox)item["CheckTemp"].FindControl("ClientSelectColumn");
                    if (chkbx.Checked)
                    {
                        isItemSelected = true;
                        var task = item.GetDataKeyValue("TaskId");
                        if (task != null)
                        {
                            var taskId = Convert.ToInt64(task);
                            if (taskId > 0)
                            {
                                selectedTaskIds.Add(taskId);
                            }
                        }
                    }
                }
                if (isItemSelected == false || selectedTaskIds.Count == 0)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please select a task to assign.");
                    return;
                }
                List<Dto.Task> tasks = new List<Dto.Task>();
                bool isSuccessful = false;
                foreach (var selectedTaskId in selectedTaskIds)
                {
                    var task = DomainApiClient<Dto.Task>.GetById(selectedTaskId);
                    if (task != null)
                    {
                        task.AssignedTo = usrId;
                        task.AssignedBy = m_session.User.Id;
                        task.LastModifiedBy = m_session.User.DisplayName;
                        task.LastModifiedDate = DateTime.Now;
                        DomainApiClient<Dto.Task>.Save(task);
                        isSuccessful = true;
                    }
                }
                if (isSuccessful)
                {
                    MasterPage.ShowAlert(AlertType.Success, $"Selected tasks assigned to {usr?.DisplayName} successfully.");
                }
                UserNameAutoCompleteBox.Entries.Clear();
                grdTask.Rebind();

            }
            catch (Exception ex)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Exception, "BulkAssignGroupTask", ex);
                MasterPage.ShowAlert(AlertType.Error, "Task Assigned failed");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Task/GroupTasks.aspx");
        }
        protected void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            var x = sender as CheckBox;
            if (x != null)
            {
                bool checkStatus = x.Checked;
                foreach (GridDataItem item in grdTask.MasterTableView.Items)
                {
                    CheckBox chkbx = (CheckBox)item["CheckTemp"].FindControl("ClientSelectColumn");
                    if (chkbx.Visible)
                    {
                        chkbx.Checked = checkStatus;
                        item.Selected = chkbx.Checked;
                    }
                }
            }
        }
        protected void ClientSelectColumn_CheckedChanged(object sender, EventArgs e)
        {

        }
        protected void lnkTaskId_Command(object sender, CommandEventArgs e)
        {
            GridDataItem item = grdTask.Items[Convert.ToInt32(e.CommandArgument)];
            if (item == null)
                return;

            this.ContextObjectId = (long)item.GetDataKeyValue("TaskId");
            this.TypeofRequest = Convert.ToString(item.GetDataKeyValue("RequestType"));
            Response.Redirect($"DetailType.aspx?contextId={this.ContextObjectId}&TypeOfRequest={this.TypeofRequest}&source=bulkassigntask&groupId={ddlGroups.SelectedValue}");
        }
        private void BindGroups()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", m_session.User.Id.ToString());
            var result = DomainApiClient<Dto.Group>.GetByAction<Dictionary<long, string>>
                ("GetGroupsByUserId", parameter);
            ddlGroups.DataSource = result;
            ddlGroups.DataValueField = "Key";
            ddlGroups.DataTextField = "Value";
            ddlGroups.DataBind();
            ddlGroups.Items.Insert(0, new ListItem("--Select--", "0"));
            if (ddlGroups.Items.Count == 2)
            {
                ddlGroups.SelectedIndex = 1;
                GroupId = Convert.ToInt64(ddlGroups.SelectedValue);
                hdGroupId.Value = GroupId.ToString();
                UserNameAutoCompleteBox.Enabled = true;
            }
        }
        protected void ddlGroupsIndexChanged(object sender, EventArgs e)
        {
            UserNameAutoCompleteBox.Entries.Clear();
            if (ddlGroups.SelectedIndex != 0)
            {
                GroupId = Convert.ToInt64(ddlGroups.SelectedValue);
                hdGroupId.Value = GroupId.ToString();
                UserNameAutoCompleteBox.Enabled = true;
                BindGroupTasks();
                grdTask.DataBind();
            }
            else
            {
                GroupId = 0;
                hdGroupId.Value = GroupId.ToString();
                UserNameAutoCompleteBox.Enabled = false;
                BindGroupTasks();
                grdTask.DataBind();
            }
        }
    }
}