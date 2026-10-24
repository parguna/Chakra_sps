using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Linq.Dynamic;
using DI.Chakra.Dto.Core;
using System.Text.RegularExpressions;

namespace DI.Chakra.Web.Task
{

    public partial class GroupTasks : DI.Chakra.Web.UIBasePage, IContextObjectEnabled
    {
        SessionStatusImpl m_session = new SessionStatusImpl();

        private const string V_EntityId = "V_EntityId";
        public long? EntityId
        {
            get
            {
                if (Session[V_EntityId] == null)
                    return 0;
                return (long)Session[V_EntityId];
            }
            set
            {
                Session[V_EntityId] = value;
            }
        }
        public bool IsAdminMode
        {
            get
            {
                return m_session.isAdminGroup;
            }
        }
        StatusType TaskStatus
        {
            get
            {
                return (StatusType)Convert.ToByte(this.ddlTaskStatus.SelectedValue);
            }
        }
        bool ShowAllTasks
        {
            get
            {
                return true;//this.ShowAllCheckBox.Checked;
            }
        }

        IList<Dto.Presentation.PresentationTask> TaskPresentation
        {
            get
            {
                if (ViewState["TaskGridData"] != null)
                {
                    return (IList<Dto.Presentation.PresentationTask>)ViewState["TaskGridData"];
                }
                return new List<Dto.Presentation.PresentationTask>();
            }
            set
            {
                ViewState["TaskGridData"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (m_session.User == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            if (!IsPostBack)
            {
                BindApplicationType();
                divRequestFor.Visible = m_session.isAdminGroup || m_session.isIACMAdminGroup;
                BindTaskStatus();
                m_session.RedirectFrom = "GroupTask";
                string fromDate = this.Request.QueryString["fromDate"] ?? "";
                string toDate = this.Request.QueryString["toDate"] ?? "";
                string requestFor = this.Request.QueryString["requestFor"] ?? "";
                string requestForName = this.Request.QueryString["requestForName"] ?? "";
                string epId = this.Request.QueryString["endpointsystemId"] ?? "";
                if (!string.IsNullOrWhiteSpace(this.Request.QueryString["ddltaskstatus"]))
                {
                    ddlTaskStatus.SelectedIndex = int.Parse(this.Request.QueryString["ddltaskstatus"]);
                    //ShowAllCheckBox.Checked = bool.Parse(this.Request.QueryString["showalltask"]);
                }
                else
                {
                    ddlTaskStatus.Items.FindByValue(Convert.ToInt16(StatusType.Pending).ToString()).Selected = true;
                }
                if (!string.IsNullOrWhiteSpace(requestFor) && !string.IsNullOrWhiteSpace(requestForName) && divRequestFor.Visible)
                {
                    RequestForUserName.Entries.Add(new AutoCompleteBoxEntry(requestForName, requestFor));
                }
                if (!string.IsNullOrWhiteSpace(epId))
                    ddlEndpointSystemId.Items.FindByValue(epId).Selected = true;
                if (!string.IsNullOrWhiteSpace(toDate) && !string.IsNullOrWhiteSpace(fromDate))
                {
                    DatePickerValidFrom.SelectedDate = Convert.ToDateTime(fromDate);
                    DatePickerValidTo.SelectedDate = Convert.ToDateTime(toDate);
                }
                if (!string.IsNullOrWhiteSpace(this.Request.QueryString["ddltaskstatus"]) && !string.IsNullOrWhiteSpace(this.Request.QueryString["showalltask"]))
                {
                    ddlTaskStatus.SelectedIndex = int.Parse(this.Request.QueryString["ddltaskstatus"]);
                    //ShowAllCheckBox.Checked = bool.Parse(this.Request.QueryString["showalltask"]);
                }
            }
        }

        protected override IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Task ID", "taskid", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request ID", "accessrequestid", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Application Name", "eventname", null));
            
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Task", "type", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Task Source", "tasksource", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Account Type", "accounttype", null));

            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Assigned By", "assignedbyname", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Assigned To", "assignedtoname", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Task Type", "requesttype", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request For", "requestedfor", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request By", "requestedby", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Completion Date", "completiondate", "dd-MM-yyyy"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Is Delegated", "isdelegate", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Delegated By", "delegatedby", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Entitlement", "entitlement", null));
            
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Requested Date", "requesteddate", "dd-MM-yyyy hh:mm tt"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Created Date", "createddate", "dd-MM-yyyy hh:mm tt"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Start Date", "startdatetoshort", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("End Date", "enddateshort", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Status", "statustype", null));
            return param;
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (this.ContextObjectId == null)
                this.ContextObjectId = EntityId;

            if(e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var data = GetTaskList(true);
                if(data != null && data.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(grdTask.MasterTableView.FilterExpression))
                        data = data.AsEnumerable().AsQueryable().Where(grdTask.MasterTableView.FilterExpression).ToList();
                    if (grdTask.MasterTableView.SortExpressions != null && grdTask.MasterTableView.SortExpressions.Count > 0)
                    {
                        data = data.AsEnumerable().AsQueryable().OrderBy(GetSortExpression(grdTask.MasterTableView.SortExpressions)).ToList();
                    }

                    GenericExport(data, e.ActionType, $"GroupTask_{DateTime.Now.ToString("ddmmyyyyhhmmss")}");
                }
            }
        }

        private void BindTaskStatus()
        {
            ddlTaskStatus.Items.Clear();
            foreach (StatusType status in Enum.GetValues(typeof(StatusType)))
            {
                if (status == StatusType.None)
                    continue;

                var description = UtilityMethods.GetEnumAttribute<Description>(status);
                ListItem item = new ListItem(description, Convert.ToInt16(status).ToString());
                ddlTaskStatus.Items.Add(item);
            }
            ddlTaskStatus.Items.Insert(0, "---All---");
            ddlTaskStatus.Items.FindByValue(Convert.ToInt16(StatusType.Pending).ToString()).Selected = true;
        }
        private string IsValidForSearch()
        {
            string isValidSearch = string.Empty;
            if (divRequestFor.Visible)
            {
                if (RequestForUserName.Entries.Count == 0)
                {
                    if (ddlTaskStatus.SelectedValue != ((int)StatusType.Pending).ToString() && ddlEndpointSystemId.SelectedIndex == 0)
                    {
                        isValidSearch = "Please select Application or Request For";
                        return isValidSearch;
                    }
                    if (ddlTaskStatus.SelectedIndex == 0 || ddlTaskStatus.SelectedValue == ((int)StatusType.Approved).ToString())
                    {
                        isValidSearch = isValidDateRange();
                        return isValidSearch;
                    }
                }
            }
            else
            {
                if (ddlTaskStatus.SelectedIndex == 0 || ddlTaskStatus.SelectedValue == ((int)StatusType.Approved).ToString())
                {
                    isValidSearch = isValidDateRange();
                }
            }
            return isValidSearch;
        }
        protected void grdTask_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            var errMsg = IsValidForSearch();
            if (string.IsNullOrWhiteSpace(errMsg))
            {
                TaskPresentation = GetTaskList();
                grdTask.DataSource = TaskPresentation;
            }
            else
            {
                grdTask.DataSource = new List<Dto.Presentation.PresentationTask >();
                MasterPage.ShowAlert(AlertType.Error, errMsg);
            }
            
        }

        private List<Dto.Presentation.PresentationTask> GetTaskList(bool isExport=false)
        {
            IList<Dto.Presentation.PresentationTask> taskDetailsList = this.GetTasks(isExport);
            var adhocRequest = new AdHocAccessRequest();
            foreach (var task in taskDetailsList)
            {
                adhocRequest.ApplicationType = task.ApplicationType;
                adhocRequest.EntitlementDetail = task.EntitlementXML;
                task.Entitlement = adhocRequest.DeserializeEntitlement();
            }
            return taskDetailsList.ToList();
        }

        protected void grdTask_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "ActionEdit")
            {
                GridDataItem item = e.Item as GridDataItem;

                string taskSource = item.GetDataKeyValue("TaskSource").ToString();

                long taskId = (long)Convert.ChangeType(item.GetDataKeyValue("TaskId"), typeof(long));
                long AccessRequestID = 0;
                if(taskSource == "Attestation")
                {
                    AccessRequestID = (long)Convert.ChangeType(item.GetDataKeyValue("AttestationCampaignId"), typeof(long));
                }
                else
                {
                    AccessRequestID = (long)Convert.ChangeType(item.GetDataKeyValue("AccessRequestID"), typeof(long));
                }
                var tsk = DomainApiClient<Dto.Task>.GetById(taskId);
                var request = string.Empty;
                Dto.AccessRequest adhocRequest = null;
                Dto.BulkRequest blkRequest = null;
                if (tsk.AttestationId.HasValue)
                {

                }
                else
                {
                    AccessRequestID = (long)Convert.ChangeType(item.GetDataKeyValue("AccessRequestID"), typeof(long));

                    if (tsk.BulkRequestId.HasValue)
                    {
                        blkRequest = DomainApiClient<Dto.BulkRequest>.GetById(AccessRequestID, true);
                        request = string.Join(",", blkRequest.BulkRequestDetails.Select(i => i.UserId));
                    }
                    else
                    {
                        adhocRequest = DomainApiClient<Dto.AccessRequest>.GetById(AccessRequestID);
                        request = adhocRequest.RequesterId.ToString();
                    }
                }
                
                Session["GroupName"] = tsk.GroupName;
                if (request != null)
                    Session["Requestfor"] = request;
                SelfRadioButton.Checked = true;
                OthersRadioButton.Checked = false;
                UserNameAutoCompleteBox.Enabled = false;
                UserNameAutoCompleteBox.Entries.Clear();

                this.TaskIdHiddenField.Value = taskId.ToString();
                if (tsk.AssignedTo.HasValue)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("id", tsk.AssignedTo.Value.ToString());
                    User usr = DomainApiClient<Dto.User>.GetByAction<User>
                        (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);

                    if (tsk.AssignedTo.Value != m_session.User.Id)
                    {
                        SelfRadioButton.Checked = false;
                        OthersRadioButton.Checked = true;
                        UserNameAutoCompleteBox.Enabled = true;
                    }
                    this.UserNameAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(usr.DisplayName, usr.Id.ToString()));
                }
                else
                {
                    UserNameAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry
                        (m_session.User.DisplayName, m_session.User.Id.ToString()));
                }
                if (tsk.BulkRequestId.HasValue == false)
                {
                    if (!string.IsNullOrWhiteSpace(request) && (adhocRequest.RequesterId.Equals(m_session.User.Id) || adhocRequest.UserId.Equals(m_session.User.Id)))
                    {
                        selfAssignDiv.Visible = false;
                        UserNameAutoCompleteBox.Enabled = true;
                        SelfRadioButton.Checked = false;
                        OthersRadioButton.Checked = true;
                        UserNameAutoCompleteBox.Entries.Clear();
                    }
                    else
                    {
                        selfAssignDiv.Visible = true;
                    }
                }
                else
                {
                    if (blkRequest != null && (blkRequest.BulkRequestDetails.Exists(i => i.UserId == m_session.User.Id)
                        || blkRequest.RequestedBy == m_session.User.Id))
                    {
                        selfAssignDiv.Visible = false;
                        UserNameAutoCompleteBox.Enabled = true;
                        SelfRadioButton.Checked = false;
                        OthersRadioButton.Checked = true;
                        UserNameAutoCompleteBox.Entries.Clear();
                    }
                    else
                    {
                        selfAssignDiv.Visible = true;
                    }
                }

                ShowWindow();
            }
            else if (e.CommandName == "MoreDetails")
            {
                MoreDetailModalPopupExtender.Show();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModalMore();", true);
                if (TaskPresentation != null && TaskPresentation.Count > 0)
                {
                    var task = (e.Item as GridDataItem).GetDataKeyValue("TaskId");
                    int taskid = task != null ? Convert.ToInt32((e.Item as GridDataItem).GetDataKeyValue("TaskId")) : 0;
                    IList<Dto.Presentation.PresentationTask> taskDetails = TaskPresentation.
                        Where(lq => lq.TaskId == taskid).ToList();
                    if (taskDetails != null && taskDetails.Count() > 0)
                    {
                        string entitlement = string.Empty;
                        if (taskDetails[0].AdHocAccessRequestId.HasValue)
                        {
                            Dictionary<string, string> parameter = new Dictionary<string, string>();
                            parameter.Add("adhocAccessRequestId", taskDetails[0].AdHocAccessRequestId.Value.ToString());
                            entitlement = DomainApiClient<Dto.AdHocAccessRequest>.GetByAction<string>
                                (DomainApiAction.AdhocRequest.GetEntitlementDetail, parameter);
                        }
                        LabelAccountType.Text = UtilityMethods.GetEnumAttribute<Description>((AccountType)Enum.Parse(typeof(AccountType), taskDetails[0].AccountType, true));
                        LabelAssignedBy.Text = taskDetails[0].AssignedByName;
                        LabelAssignedTo.Text = taskDetails[0].AssignedToName;
                        LabelTaskType.Text = taskDetails[0].RequestType;
                        LabelRequestFor.Text = taskDetails[0].RequestedFor;
                        LabelRequestBy.Text = taskDetails[0].RequestedBy;
                        LabelIsDelegated.Text = Convert.ToString(taskDetails[0].IsDelegate);
                        LabelEntitlement.Text = entitlement;
                        LabelStartDate.Text = taskDetails[0].StartDate != null ? String.Format("{0:g}", taskDetails[0].StartDate) : string.Empty;
                        LabelEndDate.Text = taskDetails[0].EndDate != null ? String.Format("{0:g}", taskDetails[0].EndDate) : string.Empty;
                        DelegatedComment.Text = taskDetails[0].DelegatedComment != null ? taskDetails[0].DelegatedComment : String.Empty;
                    }
                }
            }
            else if (e.CommandName == "ViewTask")
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

        protected void ChangeGrid(object sender, EventArgs e)
        {
            if (ddlTaskStatus.SelectedValue == ((int)StatusType.Pending).ToString())
            {
                grdTask.Rebind();
            }
            else
            {
                var err = IsValidForSearch();
                if (!string.IsNullOrWhiteSpace(err))
                {
                    grdTask.DataSource = new List<Dto.Presentation.PresentationTask>();
                    grdTask.DataBind();
                }
            }
        }

        protected void grdTask_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                Label lblTask = (Label)item.FindControl("Label1");
                LinkButton lnkTask = (LinkButton)item.FindControl("ViewTaskButton");
                LinkButton lnkTaskId = (LinkButton)item.FindControl("lnkTaskId");
                HiddenField hdnassignedTo = (HiddenField)item.FindControl("hdnAssignedTo");
                LinkButton btnAssignTask = (LinkButton)item["AssignTask"].Controls[0];
                btnAssignTask.Visible = (Convert.ToBoolean(item.GetDataKeyValue("IsCompleted")) == false && Convert.ToString(item.GetDataKeyValue("StatusType")) == "Pending") ? true : false;
                if (lblTask != null && lnkTask != null && hdnassignedTo != null)
                {
                    int hdnassignedToId = 0;
                    bool cndn = int.TryParse(hdnassignedTo.Value, out hdnassignedToId);
                    if (lnkTask.Visible == true)
                    {
                        if (cndn == true)
                        {
                            if (hdnassignedToId == m_session.User.Id)
                            {
                                lnkTask.Visible = true;
                                lblTask.Visible = false;
                            }
                            else
                            {
                                lnkTask.Visible = false;
                                lblTask.Visible = true;
                            }
                        }
                        else
                        {
                            lnkTask.Visible = true;
                            lblTask.Visible = false;
                        }
                    }
                }
                if(lnkTaskId != null)
                {
                    PostBackTrigger trigger = new PostBackTrigger();
                    trigger.ControlID = lnkTaskId.UniqueID;
                    UpdatePanel1.Triggers.Add(trigger);

                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            CloseWindow();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                {
                    ShowWindow();
                    return;
                }

                if (UserNameAutoCompleteBox.Entries.Count > 0)
                {
                    if (!string.IsNullOrEmpty(UserNameAutoCompleteBox.Entries[0].Text))
                    {
                        long usrId = Convert.ToInt64(UserNameAutoCompleteBox.Entries[0].Value);
                        long taskId = Convert.ToInt64(this.TaskIdHiddenField.Value);
                        if (taskId > 0)
                        {
                            var tsk = DomainApiClient<Dto.Task>.GetById(taskId);

                            if (tsk != null)
                            {
                                var userId = SelfRadioButton.Checked ? m_session.User.Id : usrId;
                                if (tsk.BulkRequestId.HasValue && tsk.BulkRequestId > 0)
                                {
                                    var blkRequest = DomainApiClient<Dto.BulkRequest>.GetById(tsk.BulkRequestId.Value, true);
                                    if (blkRequest != null && blkRequest.BulkRequestDetails != null && blkRequest.BulkRequestDetails.Count > 0)
                                    {
                                        if (blkRequest.BulkRequestDetails.Exists(i => i.UserId == userId))
                                        {
                                            MasterPage.ShowAlert(AlertType.Warning, "The selected User Name is part of BulkRequest User list. Please select the different user.");
                                            return;
                                        }
                                    }
                                }
                                tsk.IsDelegated = false;
                                if (SelfRadioButton.Checked == true)
                                {
                                    tsk.AssignedTo = userId;
                                    tsk.AssignedBy = m_session.User.Id;
                                }
                                else
                                {
                                    tsk.AssignedTo = userId;
                                    tsk.AssignedBy = m_session.User.Id;
                                }
                                DomainApiClient<Dto.Task>.Save(tsk);
                                MasterPage.ShowAlert(AlertType.Success, "Task Assigned Successfully");

                                //update the action user details in the snapshot
                                if (tsk.AttestationId.HasValue && tsk.AttestationCampaignId.HasValue && tsk.AssignedTo.HasValue)
                                {
                                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                                    parameter.Add("attestationId", tsk.AttestationId.ToString());
                                    parameter.Add("attestationCampaignId", tsk.AttestationCampaignId.ToString());
                                    parameter.Add("taskId", tsk.Id.ToString());
                                    parameter.Add("assignedTo", tsk.AssignedTo.ToString());
                                    parameter.Add("currentUser", m_session.User.UserName.ToString());

                                    var updateReviewerInfoInSnapshot = DomainApiClient<Dto.AttestationSnapshotAction>.GetByAction<System.Object>
                                        (DomainApiAction.Attestation.UpdateReviewerDetail, parameter);
                                }
                                CloseWindow();
                                grdTask.Rebind();
                            }
                        }
                    }
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please select the username");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void UserSelectionChanged(object sender, EventArgs e)
        {
            UserNameAutoCompleteBox.Entries.Clear();
            if (SelfRadioButton.Checked == true)
            {
                UserNameAutoCompleteBox.Enabled = false;
                UserNameAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(m_session.User.DisplayName, m_session.User.Id.ToString()));
            }
            else
                UserNameAutoCompleteBox.Enabled = true;
            ShowWindow();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);

        }

        protected void UserNameAutoCompleteValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (this.UserNameAutoCompleteBox.Entries.Count == 0)
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        private IList<Dto.Presentation.PresentationTask> GetTasks(bool isExport=false)
        {
            //if (this.ShowAllTasks)
            //{
            //    Dictionary<string, string> parameter = new Dictionary<string, string>();
            //    parameter.Add("actionUserId", m_session.User.Id.ToString());
            //    return DomainApiClient<Dto.Presentation.PresentationTask>.GetByAction<List<Dto.Presentation.PresentationTask>>
            //        (DomainApiAction.Tasks.Presentation.PresentationTask.GetGroupTaskDetailsByActionUserId, parameter);
            //}
            //else
            //{
            //    Dictionary<string, string> parameter = new Dictionary<string, string>();
            //    parameter.Add("actionUserId", m_session.User.Id.ToString());
            //    parameter.Add("taskStatus", TaskStatus.ToString());
            //    return DomainApiClient<Dto.Presentation.PresentationTask>.GetByAction<List<Dto.Presentation.PresentationTask>>
            //        (DomainApiAction.Tasks.Presentation.PresentationTask.GetGroupTaskDetailsByTaskStatus, parameter);
            //}
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            long actionUserId = 0;
            actionUserId = m_session.User.Id;
            parameter.Add("actionUserId", actionUserId.ToString());
            int PageIndex = grdTask.CurrentPageIndex * grdTask.MasterTableView.PageSize;
            int PageSize = grdTask.MasterTableView.PageSize;
            if (isExport)
            {
                PageIndex = 0;
                PageSize = -1;
            }
            var filterExpression = grdTask.MasterTableView.FilterExpression;

            if (filterExpression.Contains("DateTime.Parse"))
            {
                List<string> finalOutput = new List<string>();
                foreach (string condition in filterExpression.Split(new string[] { " AND " }, StringSplitOptions.None))
                {
                    string rawExpression = condition;
                    var findMatch = Regex.Match(rawExpression, "DateTime.Parse(.*)", RegexOptions.IgnoreCase);
                    if (findMatch.Success)
                    {
                        var dtExtract = DateTime.Parse(findMatch.Value.Split('\"')[1]);
                        var conditionDate = string.Format("DateTime({0}, {1}, {2})", dtExtract.Year, dtExtract.Month, dtExtract.Day);
                        finalOutput.Add(Regex.Replace(rawExpression, "DateTime.Parse(.*)", string.Format("{0})", conditionDate), RegexOptions.IgnoreCase));
                    }
                    else
                        finalOutput.Add(rawExpression);
                }

                filterExpression = string.Join(" AND ", finalOutput);
            }
            PresentationGridPagedDataParameter paramObj = new PresentationGridPagedDataParameter
            {
                StartRowIndex = PageIndex,
                MaximumRows = PageSize,
                FilterExpression = filterExpression,
                SortExpression = GetSortExpression(grdTask.MasterTableView.SortExpressions),
            };
            if (ddlEndpointSystemId.SelectedIndex == -1 || ddlEndpointSystemId.SelectedValue=="0")
            {
                parameter.Add("endpointId", "0");
            }
            else
            {
                parameter.Add("endpointId", ddlEndpointSystemId.SelectedValue);
            }

            if (ddlTaskStatus.SelectedIndex == 0)
            {
                parameter.Add("taskStatus", null);
            }
            else
            {
                parameter.Add("taskStatus", ((byte)TaskStatus).ToString());
            }
            if (RequestForUserName.Entries.Count > 0)
            {
                parameter.Add("requestFor", RequestForUserName.Entries[0].Value);
            }
            if(DatePickerValidFrom.SelectedDate.HasValue && DatePickerValidTo.SelectedDate.HasValue)
            {
                var dt = DatePickerValidTo.SelectedDate.Value;
                paramObj.FromDate = DatePickerValidFrom.SelectedDate;
                paramObj.ToDate = new DateTime(dt.Year, dt.Month,dt.Day,23,59,59);
            }
            var result = DomainApiClient<Dto.Presentation.PresentationTask>.GetByAction<PresentationGridPagedData<Dto.Presentation.PresentationTask>>
                (DomainApiAction.Tasks.Presentation.PresentationTask.GetGroupTaskDetailsByActionUserId, parameter, paramObj);
            grdTask.VirtualItemCount = result.TotalCount;
            return result.Data;
        }
        protected void grdTask_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = grdTask.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("TaskId");
            this.TypeofRequest = Convert.ToString(g.GetDataKeyValue("RequestType"));
            EntityId = this.ContextObjectId;
        }

        protected void UserNameAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            if (UserNameAutoCompleteBox.Entries.Count > 1)
                UserNameAutoCompleteBox.Entries.RemoveAt(1);
        }

        protected void lnkTaskId_Command(object sender, CommandEventArgs e)
        {
            GridDataItem item = grdTask.Items[Convert.ToInt32(e.CommandArgument)];
            if (item == null)
                return;
            Session["TaskRequestedBy"] = null;
            this.ContextObjectId = (long)item.GetDataKeyValue("TaskId");
            this.TypeofRequest = Convert.ToString(item.GetDataKeyValue("RequestType"));
            Session["TaskRequestedBy"] = item["RequestedBy"].Text;

            var requestFor = RequestForUserName.Entries.Count > 0 ? RequestForUserName.Entries[0].Value : "";
            var requestForName = RequestForUserName.Entries.Count > 0 ? RequestForUserName.Entries[0].Text : "";
            var datefilter = string.Empty;
            if (DatePickerValidFrom.SelectedDate.HasValue && DatePickerValidTo.SelectedDate.HasValue)
            {
                datefilter = $"&fromDate={DatePickerValidFrom.SelectedDate.Value}&toDate={DatePickerValidTo.SelectedDate.Value}";
            }

            Response.Redirect($"DetailType.aspx?contextId={this.ContextObjectId}&TypeOfRequest={this.TypeofRequest}&ddltaskstatus={ddlTaskStatus.SelectedIndex}&source=grouptask&showalltask=true&TaskId={ContextObjectId}&requestFor={requestFor}&{datefilter}&requestForName={requestForName}&endpointsystemId={ddlEndpointSystemId.SelectedValue}");
        }
        void ShowWindow()
        {
            var key = "showWindoww";
            string script = "Sys.Application.remove_load(showWindow);Sys.Application.add_load(showWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        void CloseWindow()
        {
            var key = "closeWindoww";
            string script = "Sys.Application.remove_load(closeWindow);Sys.Application.add_load(closeWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }
        protected void RequestForUserName_ClientEntryAdded(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            //if (RequestForUserName.Entries.Count > 0)
            //{
            //    grdTask.Rebind();
            //}
        }
        protected void RequestForUserName_ClientEntryRemoved(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            
        }

        protected void ClearFilter_Click(object sender, EventArgs e)
        {
            DatePickerValidFrom.SelectedDate = null;
            DatePickerValidTo.SelectedDate = null;
            ddlEndpointSystemId.SelectedIndex = 0;
            RequestForUserName.Entries.Clear();
            ddlTaskStatus.ClearSelection();
            ddlTaskStatus.Items.FindByValue(((int)StatusType.Pending).ToString()).Selected = true;
            grdTask.CurrentPageIndex = 0;
            Refresh();
        }

        protected void Filter_Click(object sender, EventArgs e)
        {
            BindGroupTask();
        }
        private void BindGroupTask()
        {
            grdTask.ClearFilter();
            grdTask.CurrentPageIndex = 0;
            if (divRequestFor.Visible)
            {
                if (RequestForUserName.Entries.Count == 0)
                {
                    if (ddlTaskStatus.SelectedValue != ((int)StatusType.Pending).ToString() && ddlEndpointSystemId.SelectedIndex == 0)
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Please select Application or Request For");
                        return;
                    }
                    if (ddlTaskStatus.SelectedIndex == 0 || ddlTaskStatus.SelectedValue == ((int)StatusType.Approved).ToString())
                    {
                        var retMsg = isValidDateRange();
                        if (!string.IsNullOrWhiteSpace(retMsg))
                        {
                            MasterPage.ShowAlert(AlertType.Error, retMsg);
                            return;
                        }
                    }
                }
            }
            else
            {
                if (ddlTaskStatus.SelectedIndex == 0 || ddlTaskStatus.SelectedValue == ((int)StatusType.Approved).ToString())
                {
                    var retMsg = isValidDateRange();
                    if (!string.IsNullOrWhiteSpace(retMsg))
                    {
                        MasterPage.ShowAlert(AlertType.Error, retMsg);
                        return;
                    }
                }
            }
            Refresh();
        }
        public void Refresh()
        {
            this.grdTask.Rebind();
        }
        string isValidDateRange()
        {
            var retMsg = string.Empty;
            if (!DatePickerValidFrom.SelectedDate.HasValue || !DatePickerValidTo.SelectedDate.HasValue)
            {
                if (IsPostBack)
                    retMsg = "Please select From and To.";
            }
            if (DatePickerValidFrom.SelectedDate.HasValue && DatePickerValidTo.SelectedDate.HasValue)
            {
                if ((DatePickerValidTo.SelectedDate.Value - DatePickerValidFrom.SelectedDate.Value).TotalDays > 180)
                {
                    retMsg = "Date Range (From and To) should be Maximum of 180 Days only allowed. Please try again";
                }
            }

            return retMsg;
        }
        void BindApplicationType()
        {
            ddlEndpointSystemId.Items.Clear();
            ddlEndpointSystemId.Items.Insert(0, new ListItem("---- All ----", "0"));
            var ep = DomainApiClient<Dto.EndPoint>.GetAll().Where(i => i.IsTrustedSource == false).OrderBy(i => i.DisplayName);

            foreach (var item in ep)
            {
                var listItem = new ListItem(item.DisplayName, item.Id.ToString());
                ddlEndpointSystemId.Items.Add(listItem);
            }
        }
    }
}

