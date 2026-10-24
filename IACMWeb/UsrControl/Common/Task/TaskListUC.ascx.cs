using Application;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Core;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common.Task
{
    public partial class TaskListUC : UIBaseUC, IAccessControl
    {
        #region Members
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private const string V_SHOWGRIDFILTER = "V_SHOWGRIDFILTER";
        private const string S_LstRequestedFor = "LstRequestedFor";
        private static IList<PresentationTask> taskDetailsList;
        private SessionStatusImpl SessionUser = new SessionStatusImpl();
        #endregion

        #region Properties
        public IsAdminMode IsAdminMode
        {
            get
            {
                if (ViewState[V_DISPLAYMODE] == null)
                    return IsAdminMode.No;
                return (IsAdminMode)ViewState[V_DISPLAYMODE];
            }
            set
            {
                ViewState[V_DISPLAYMODE] = value;
            }
        }
       
        public bool ShowGridFilter
        {
            get
            {
                if (ViewState[V_SHOWGRIDFILTER] == null)
                    return true;
                return (bool)ViewState[V_SHOWGRIDFILTER];
            }
            set
            {
                ViewState[V_SHOWGRIDFILTER] = value;
                TaskGrid.ShowGridFilter = true;
            }
        }

        StatusType TaskStatusType
        {
            get
            {
                return (StatusType)ViewState["TaskStatusType"];
            }
            set
            {
                ViewState["TaskStatusType"] = value;
            }
        }

        IList<PresentationTask> TaskGridData
        {
            get
            {
                if (ViewState["TaskGridData"] != null)
                {
                    return (IList<PresentationTask>)ViewState["TaskGridData"];
                }
                return new List<PresentationTask>();
            }
            set
            {
                ViewState["TaskGridData"] = value;
            }
        }
        List<string> LstRequestedFor
        {
            get
            {
                if (Session[S_LstRequestedFor] != null)
                {
                    return (List<string>)Session[S_LstRequestedFor];
                }
                return new List<string>();
            }
            set
            {
                Session[S_LstRequestedFor] = value;
            }
        }

        StatusType TaskStatus
        {
            get
            {
                return (StatusType)Convert.ToByte(this.ddlTaskStatus.SelectedValue);
            }
        }

        //bool ShowAllTasks
        //{
        //    get
        //    {
        //        return this.ShowAllCheckBox.Checked;
        //    }
        //}
        #endregion

        public void Refresh()
        {
            this.TaskGrid.Rebind();
        }
        #region Methods
        private void ShowOrHideGroupTaskLink()
        {
            if (IsAdminMode == IsAdminMode.Yes)
            {
                GroupTasksLinkButton.Visible = false;
            }
            else
            {
                //Dictionary<string, string> parameter = new Dictionary<string, string>();
                //parameter.Add("userId", SessionUser.User.Id.ToString());

                //GroupTasksLinkButton.Visible = DomainApiClient<Dto.Task>.GetByAction<bool>
                //    (DomainApiAction.Tasks.Task.DoesGroupTaskExistsForUser, parameter);
                GroupTasksLinkButton.Visible = true;
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
            
        }

        private void LoadGrid()
        {
            //taskDetailsList = this.GetTasks();
            
            var result = GetPagedTasks();

            if (result != null)
            {
                TaskGrid.VirtualItemCount = result.TotalCount;
                taskDetailsList = result.Data;
                //foreach (var task in taskDetailsList)
                //{
                //    adhocRequest.ApplicationType = task.ApplicationType;
                //    adhocRequest.EntitlementDetail = task.EntitlementXML;
                //    task.Entitlement = adhocRequest.DeserializeEntitlement();
                //}
                TaskGridData = taskDetailsList;
                TaskGrid.DataSource = TaskGridData.OrderByDescending(c => c.CreatedDate).ToList();
                if (IsPostBack)
                {
                    if (taskDetailsList.Count == 0)
                    {
                        ShowAlertMessage(AlertType.Success, "No Records found");
                    }
                }
            }
            else
            {
                TaskGrid.VirtualItemCount = 0;
                TaskGrid.DataSource = null;
            }
        }

        Dictionary<string, string> GetKeyValuePairsWithDates()
        {
            var dic = new Dictionary<string, string>();

            if (RequestForUserName.Entries.Count > 0)
            {
                dic.Add("RequestFor", RequestForUserName.Entries[0].Value);
            }
            if (ddlEndpointSystemId.SelectedIndex > 0)
            {
                dic.Add("endpointSystemId", ddlEndpointSystemId.SelectedValue);
            }
            dic.Add("statusType", ddlTaskStatus.SelectedValue);
            DateTime selectedDate;
            if (DatePickerValidFrom.SelectedDate.HasValue)
            {
                selectedDate = DatePickerValidFrom.SelectedDate.Value;
                var date = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 0, 0, 0);
                dic.Add("FromDate", date.ToString(Constants.DateFormat));
            }
            else
            {
                dic.Add("FromDate", null);
            }

            if (DatePickerValidTo.SelectedDate.HasValue)
            {
                selectedDate = DatePickerValidTo.SelectedDate.Value;
                var date = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 23, 59, 59);
                dic.Add("ToDate", date.ToString(Constants.DateFormat));
            }
            else
            { dic.Add("ToDate", null); }

            return dic;

        }
        Dictionary<string, string> GetQueryParam()
        {
            var dic = new Dictionary<string, string>();

            if (RequestForUserName.Entries.Count > 0)
            {
                dic.Add("RequestFor", RequestForUserName.Entries[0].Value);
            }
            if (ddlEndpointSystemId.SelectedIndex > 0)
            {
                dic.Add("endpointSystemId", ddlEndpointSystemId.SelectedValue);
            }
            dic.Add("statusType", ddlTaskStatus.SelectedValue);
            

            return dic;

        }
        private Dto.Core.PresentationGridPagedData<Dto.Presentation.PresentationTask> GetPagedTasks(bool forReport=false)
        {
            var dic = GetQueryParam();
            PresentationGridPagedDataParameter presentationGridPagedDataParameter = new PresentationGridPagedDataParameter();
            DateTime selectedDate;
            if (DatePickerValidFrom.SelectedDate.HasValue)
            {
                selectedDate = DatePickerValidFrom.SelectedDate.Value;
                presentationGridPagedDataParameter.FromDate = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 0, 0, 0);
            }
            else
            {
                presentationGridPagedDataParameter.FromDate = null;
            }

            if (DatePickerValidTo.SelectedDate.HasValue)
            {
                selectedDate = DatePickerValidTo.SelectedDate.Value;
                presentationGridPagedDataParameter.ToDate = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 23, 59, 59);
            }
            else
            {
                presentationGridPagedDataParameter.ToDate = null;
            }
            string webMethod = string.Empty;
            webMethod = "GetTaskPagedResult";

            if (IsAdminMode == IsAdminMode.Yes)
            {
            }
            else
            {
                if (this.ddlTaskStatus.SelectedIndex == 0)
                {
                    dic.Add("actionUserId", SessionUser.User.Id.ToString());
                }
                else
                {
                    dic.Add("actionUserId", SessionUser.User.Id.ToString());
                    dic.Add("taskStatus", TaskStatus.ToString());
                }
            }
            int PageIndex = TaskGrid.CurrentPageIndex * TaskGrid.MasterTableView.PageSize;
            int PageSize = TaskGrid.MasterTableView.PageSize;
            string sortExpressions = string.Empty;
            string sortingFieldName = "";
            string sortOrder = "";
            if (TaskGrid.MasterTableView.SortExpressions.Count > 0)
            {
                sortingFieldName = TaskGrid.MasterTableView.SortExpressions[0].FieldName;
                sortOrder = TaskGrid.MasterTableView.SortExpressions[0].SortOrderAsString();
                foreach (GridSortExpression item in TaskGrid.MasterTableView.SortExpressions)
                {
                    sortExpressions += string.Format("{0} {1},", item.FieldName, item.SortOrderAsString());
                }
                sortExpressions = sortExpressions.Trim(',').Trim();
            }

            var filterExpression = TaskGrid.MasterTableView.FilterExpression;

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

            presentationGridPagedDataParameter.StartRowIndex = PageIndex;
            presentationGridPagedDataParameter.MaximumRows = forReport ? -1 : PageSize;
            presentationGridPagedDataParameter.FilterExpression = filterExpression;
            presentationGridPagedDataParameter.SortExpression = sortExpressions.Trim(',').Trim();
            
            var result = DomainApiClient<PresentationTask>.GetByAction<Dto.Core.PresentationGridPagedData<Dto.Presentation.PresentationTask>>
                        (webMethod, dic, presentationGridPagedDataParameter);
            var adhocRequest = new AdHocAccessRequest();
            foreach (var task in result.Data)
            {
                adhocRequest.ApplicationType = task.ApplicationType;
                adhocRequest.EntitlementDetail = task.EntitlementXML;
                task.Entitlement = adhocRequest.DeserializeEntitlement();
            }
            return result;
        }
        private IList<PresentationTask> GetTasks()
        {

            if (IsAdminMode == IsAdminMode.Yes)
            {
                return DomainApiClient<PresentationTask>.GetByAction<IList<PresentationTask>>
                        (DomainApiAction.Tasks.Presentation.PresentationTask.GetAll, GetKeyValuePairsWithDates());
            }
            else
            {
                if (this.ddlTaskStatus.SelectedIndex == 0)
                {
                    Dictionary<string, string> parameter = GetKeyValuePairsWithDates();
                    parameter.Add("actionUserId", SessionUser.User.Id.ToString());
                    return DomainApiClient<PresentationTask>.GetByAction<IList<PresentationTask>>
                        (DomainApiAction.Tasks.Presentation.PresentationTask.GetByActionUserId, parameter);
                }
                else
                {
                    Dictionary<string, string> parameter = GetKeyValuePairsWithDates();
                    parameter.Add("actionUserId", SessionUser.User.Id.ToString());
                    parameter.Add("taskStatus", TaskStatus.ToString());

                    var tasks = DomainApiClient<PresentationTask>.GetByAction<IList<PresentationTask>>
                        (DomainApiAction.Tasks.Presentation.PresentationTask.GetTaskDetailsByTaskStatus, parameter);
                    if (tasks != null)
                        return tasks.Where(p => p.TaskAccessRequest != AccessRequestStatus.Withdrawn).ToList();
                    return null;
                }
            }

        }

        private TaskDelegation GetDelegationInfoFromUI()
        {
            TaskDelegation delegation = new TaskDelegation();
            User deletgateTo = UserDetail.SelectedUser;
            delegation.ActionUser = SessionUser.User.Id;
            delegation.DelegatedTo = deletgateTo.Id;
            delegation.DelegationComments = UserDetail.Comments;
            delegation.IsActive = true;
            delegation.CreatedBy = SessionUser.User.DisplayName;
            return delegation;
        }
        bool CanAllowDelegateForGroupTask()
        {
            return Request.Url.AbsolutePath == "/Prov/Task/List.aspx";
        }
        public string Save()
        {
            string validationMessage = "";
            bool skipConcMsg = false;
            Page.Validate();
            if (Page.IsValid)
            {
                TaskDelegation delegation = this.GetDelegationInfoFromUI();
                long selectedTaskId = -1;
                List<long> selectedTaskIdList = new List<long>();
                var param = new Dictionary<string, string>();
                param.Add("requestType", RequestType.Provisioning.ToString());
                param.Add("actionUser", delegation.DelegatedTo.ToString());
                var taskDelegation = DomainApiClient<Dto.TaskDelegation>.GetByAction<Dto.TaskDelegation>(DomainApiAction.Tasks.TaskDelegation.GetAdvanceDelegationByActionUser, param);
                if (taskDelegation != null)
                {
                    if (UserDetail.SelectedUser != null)
                    {
                        validationMessage = $"{UserDetail.SelectedUser.DisplayName}({UserDetail.SelectedUser.UserName}) has advance delegation enabled(Delegated To:{taskDelegation.DelegatedToUser.DisplayName}). Please assign to other(s)";
                        return validationMessage;
                    }
                }

                foreach (GridDataItem item in TaskGrid.MasterTableView.Items)
                {
                    CheckBox chk = (CheckBox)item.FindControl("ClientSelectColumn");
                    if (chk.Checked)
                    {
                        selectedTaskId = Convert.ToInt64(Convert.ToString(item.GetDataKeyValue("TaskId")));
                        long AccessRequestID = item.GetDataKeyValue("AccessRequestID") != null ? Convert.ToInt64(Convert.ToString(item.GetDataKeyValue("AccessRequestID"))) : -1;
                        bool canAdd = true;
                        if (Convert.ToString(item.GetDataKeyValue("RequestType")) == UtilityMethods.GetEnumAttribute<Description>(RequestType.Provisioning))
                        {
                            var request = DomainApiClient<Dto.AccessRequest>.GetById(AccessRequestID);
                            if (request != null && request.UserId == delegation.DelegatedTo)
                            {
                                validationMessage = string.IsNullOrEmpty(validationMessage) ? " " + Convert.ToString(AccessRequestID) : validationMessage + " ," + Convert.ToString(AccessRequestID);
                            }

                            Dto.Task taskObj = DomainApiClient<Dto.Task>.GetById(selectedTaskId);
                            if (taskObj != null)
                            {
                                Dto.User userApproverObj = null;
                                var actionUser = taskObj.ActionUserId;
                                if (taskObj.IsGroupTask && !taskObj.IsDelegated && taskObj.AssignedTo.HasValue)
                                    actionUser = taskObj.AssignedTo.Value;

                                Dto.User userDelegateObj = DomainApiClient<Dto.User>.GetById(delegation.DelegatedTo, true);

                                if (taskObj.IsGroupTask && !taskObj.IsDelegated)
                                {
                                    if (CanAllowDelegateForGroupTask())
                                    {
                                        selectedTaskIdList.Add(selectedTaskId);
                                        continue;
                                    }

                                    if (!taskObj.AssignedTo.HasValue)
                                    {
                                        canAdd = false;
                                        validationMessage = string.Format("The Request/s {0} is/are not delegated. This request needs to assign first.", Convert.ToString(AccessRequestID));
                                        skipConcMsg = true;
                                    }
                                }
                            }

                            if (canAdd)
                            {
                                selectedTaskIdList.Add(selectedTaskId);
                            }
                        }
                        else if (Convert.ToString(item.GetDataKeyValue("RequestType")) == UtilityMethods.GetEnumAttribute<Description>(RequestType.BulkRequest))
                        {
                            var request = DomainApiClient<Dto.BulkRequest>.GetById(AccessRequestID, true);
                            if (request != null && request.BulkRequestDetails.Any(i => i.UserId == delegation.DelegatedTo))
                            {
                                canAdd = false;
                                validationMessage += $"RequestId: {AccessRequestID} - DelegatedTo should not be part of BulkRequest User list.{Environment.NewLine}";
                            }
                            Dto.Task taskObj = DomainApiClient<Dto.Task>.GetById(selectedTaskId);
                            if (taskObj != null)
                            {
                                var actionUser = taskObj.ActionUserId;
                                if (taskObj.IsGroupTask && !taskObj.IsDelegated && taskObj.AssignedTo.HasValue)
                                    actionUser = taskObj.AssignedTo.Value;

                                Dto.User userDelegateObj = DomainApiClient<Dto.User>.GetById(delegation.DelegatedTo, true);
                                if (taskObj.IsGroupTask && !taskObj.IsDelegated)
                                {
                                    if (CanAllowDelegateForGroupTask())
                                    {
                                        selectedTaskIdList.Add(selectedTaskId);
                                        continue;
                                    }

                                    if (!taskObj.AssignedTo.HasValue)
                                    {
                                        canAdd = false;
                                        validationMessage = string.Format("The Request/s {0} is/are not delegated. This request needs to assign first.", Convert.ToString(AccessRequestID));
                                        skipConcMsg = true;
                                    }
                                }
                            }
                            if (canAdd)
                            {
                                selectedTaskIdList.Add(selectedTaskId);
                            }
                        }
                        else
                        {
                            selectedTaskId = Convert.ToInt64(Convert.ToString(item.GetDataKeyValue("TaskId")));
                            selectedTaskIdList.Add(selectedTaskId);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(validationMessage))
                {
                    string Message = string.Empty;

                    if (!skipConcMsg)
                    {
                        Message = "The request ID (";
                        int LastIndex = validationMessage.LastIndexOf(",");
                        Message += LastIndex > 0 ? validationMessage.Substring(0, LastIndex) + " and" + validationMessage.Substring(LastIndex + 1) : validationMessage;
                        Message += ") was not delegated. Task cannot be delegated to Requestee";
                    }
                    else
                    {
                        Message = validationMessage;
                    }

                    validationMessage = Message;
                    return validationMessage;
                }
                else
                {
                    Dto.TaskDelegationWrapper wrapper = new Dto.TaskDelegationWrapper();
                    wrapper.Delegation = delegation;
                    wrapper.SelectedtaskList = selectedTaskIdList;
                    DomainApiClient<Dto.TaskDelegation>.GetByAction<Dto.TaskDelegation>
                        (DomainApiAction.Tasks.TaskDelegation.CreateSelectedTasksDelegation, null, wrapper);

                    MasterPage.ShowAlert(AlertType.Success, "Delegated Successfully");
                }
                return validationMessage;
            }
            else
            {
                return validationMessage;
            }
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["TaskRequestedBy"] = null;
                BindTaskStatus();
                ShowOrHideGroupTaskLink();
                BindApplicationType();
                DatePickerValidTo.MaxDate = DateTime.Now;
                divRequestFor.Visible = this.IsAdminMode == IsAdminMode.Yes;
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
                if(!string.IsNullOrWhiteSpace(epId))
                    ddlEndpointSystemId.Items.FindByValue(epId).Selected = true;
                if (!string.IsNullOrWhiteSpace(toDate) && !string.IsNullOrWhiteSpace(fromDate))
                {
                    DatePickerValidFrom.SelectedDate = Convert.ToDateTime(fromDate);
                    DatePickerValidTo.SelectedDate = Convert.ToDateTime(toDate);
                }
                //ChangeGrid(this.ddlTaskStatus, null);
            }
            else
            {
                UpdateModalPopup.Update();
            }
            this.MasterPage.ActionClicked += ActionClicked;
        }

        void ShowWindow()
        {

            var key = "showWindoww";
            string script = "Sys.Application.remove_load(showWindow);Sys.Application.add_load(showWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }
        private void SetDelegationValidationMessage(string message)
        {
            var lbl = (this.UserDetail.FindControl("lblDelegationMessage") as Label);
            lbl.Text = message;
        }
        void CloseWindow()
        {
            var key = "closeWindoww";
            string script = "Sys.Application.remove_load(closeWindow);Sys.Application.add_load(closeWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delegate)
            {
                if (ddlTaskStatus.SelectedItem.Text == StatusType.Approved.ToString())
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Approved task cannot be delegated");
                    return;
                }
                else
                {
                    GridDataItem[] items = TaskGrid.MasterTableView.GetSelectedItems();
                    int selectedcount = 0;
                    foreach (GridDataItem item in TaskGrid.MasterTableView.Items)
                    {
                        CheckBox chkbx = (CheckBox)item["CheckTemp"].FindControl("ClientSelectColumn");
                        if (chkbx.Checked)
                            selectedcount++;
                    }
                    if (selectedcount > 0 || items.Count() > 0)
                    {
                        if (items.Count() == 1 && TaskStatusType != StatusType.Pending)
                        {
                            MasterPage.ShowAlert(AlertType.Warning, TaskStatusType + " task cannot be delegated.");
                            return;
                        }
                        var lbl = (this.UserDetail.FindControl("lblDelegationMessage") as Label);
                        lbl.Text = "";
                        ShowWindow();
                    }
                    else
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Please select at least one task to delegate");
                    }
                }
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var rep = GetPagedTasks(true);
                GenericExport(rep.Data, e.ActionType, "Tasks");
            }
        }

        protected void TaskGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            //if (ddlTaskStatus.SelectedItem.Text == "---All---" && !ShowAllCheckBox.Checked)
            //{
            //    MasterPage.ShowAlert(AlertType.Warning, "Please select Task Status");
            //    return;
            //}
            var errMsg = IsValidForSearch();
            if (string.IsNullOrWhiteSpace(errMsg))
            {
                LoadGrid();
            }
            else
            {
                TaskGrid.DataSource = new List<Dto.Presentation.PresentationTask>();
                ShowAlertMessage(AlertType.Error, errMsg);
            }
        }

        protected void TaskGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "MoreDetails")
            {
                Tasklist.VisibleOnPageLoad = true;


                if (TaskGridData != null && TaskGridData.Count > 0)
                {
                    var task = (e.Item as GridDataItem).GetDataKeyValue("TaskId");
                    int taskid = task != null ? Convert.ToInt32((e.Item as GridDataItem).GetDataKeyValue("TaskId")) : 0;
                    IList<PresentationTask> taskDetails = TaskGridData.Where(lq => lq.TaskId == taskid).ToList();
                    if (taskDetails != null && taskDetails.Count() > 0)
                    {
                        LabelAccountType.Text = UtilityMethods.GetEnumAttribute<Description>((AccountType)Enum.Parse(typeof(AccountType), taskDetails[0].AccountType, true));
                        LabelTaskType.Text = taskDetails[0].RequestType;
                        LabelRequestFor.Text = taskDetails[0].RequestedFor;
                        LabelRequestBy.Text = taskDetails[0].RequestedBy;
                        LabelIsDelegated.Text = Convert.ToString(taskDetails[0].IsDelegate);
                        RequestReason.Text = taskDetails[0].RequestReason;
                        //LabelEntitlement.Text = taskDetails[0].Entitlement;
                        DelegatedBy.Text = taskDetails[0].DelegatedBy;
                        DelegatedComment.Text = taskDetails[0].DelegatedComment;
                        LabelStartDate.Text = taskDetails[0].StartDate != null ? String.Format("{0:g}", taskDetails[0].StartDate) : string.Empty;
                        LabelEndDate.Text = taskDetails[0].EndDate != null ? String.Format("{0:g}", taskDetails[0].EndDate) : string.Empty;
                        LBLstatus.Text = UtilityMethods.GetEnumAttribute<Description>((StatusType)Enum.Parse(typeof(StatusType), taskDetails[0].StatusType, true));
                    }
                }
            }
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
                if ((DatePickerValidTo.SelectedDate.Value - DatePickerValidFrom.SelectedDate.Value).TotalDays > 180 && this.IsAdminMode == IsAdminMode.Yes)
                {
                    retMsg = "Date Range (From and To) should be Maximum of 180 Days only allowed. Please try again";
                }
            }

            return retMsg;
        }

        protected void ChangeGrid(object sender, EventArgs e)
        {
            //TaskGrid.DataSource = new List<PresentationTask>();
            if (sender is DropDownList)
            {
                DropDownList ddsendr = (DropDownList)sender;
                //if (ShowAllCheckBox.Checked) return;
                //if (ddsendr.SelectedItem.Text == "---Select---" || ddsendr.SelectedItem.Text != "Pending")
                //    ShowAllCheckBox.Checked = true;
                //else
                //    ShowAllCheckBox.Checked = false;

                //if (!dateFilter.Visible && ddsendr.SelectedItem.Text != UtilityMethods.GetEnumAttribute<Description>(StatusType.Pending))
                //{
                //    dateFilter.Visible = true;
                //    TaskGrid.DataSource = new List<PresentationTask>();
                //    TaskGrid.DataBind();
                //    return;
                //}
                //else
                //{
                //    //if(ShowAllCheckBox.Checked == false)
                //    {
                //        dateFilter.Visible = false;
                //        RequestForUserName.Entries.Clear();
                //        if (ddlEndpointSystemId.Items.Count > 0) ddlEndpointSystemId.SelectedIndex = 0;
                //        DatePickerValidFrom.Clear();
                //        DatePickerValidTo.Clear();
                //    }
                //}

            }
            else if (sender is CheckBox)
            {
                CheckBox chsendr = (CheckBox)sender;
                if (!chsendr.Checked)
                {
                    if (dateFilter.Visible)
                    {
                        dateFilter.Visible = false;
                        ddlTaskStatus.SelectedIndex = 1;
                    }
                }
                else if (!chsendr.Checked && ddlTaskStatus.Items.Count > 1)
                {
                    ddlTaskStatus.SelectedValue = "1";
                    ddlTaskStatus.SelectedIndex = 1;
                }
                else if (chsendr.Checked)
                {
                    ddlTaskStatus.SelectedIndex = 0;
                    dateFilter.Visible = true;
                    TaskGrid.DataSource = new List<PresentationTask>();
                    TaskGrid.DataBind();
                    return;
                }
            }
            TaskGrid.Rebind();
        }
        protected void TaskGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TaskGrid.SelectedItems.Count > 0)
            {
                Session["TaskRequestedBy"] = null;
                GridDataItem g = TaskGrid.SelectedItems[0] as GridDataItem;
                this.ContextObjectId = (long)g.GetDataKeyValue("TaskId");
                this.TypeofRequest = Convert.ToString(g.GetDataKeyValue("RequestType"));
                TaskStatusType = (StatusType)Enum.Parse(typeof(StatusType), g["StatusType"].Text);
                Session["TaskRequestedBy"] = g["RequestedBy"].Text;
                g.Selected = true;
            }
        }

        protected void TaskGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                StatusType StatusType = (StatusType)Enum.Parse(typeof(StatusType), Convert.ToString(item.GetDataKeyValue("StatusType")), true); // (StatusType)item.GetDataKeyValue("StatusType");
                if (StatusType != DI.Chakra.Infrastructure.Enums.StatusType.Pending)
                {
                    CheckBox chkbx = (CheckBox)item["CheckTemp"].FindControl("ClientSelectColumn");
                    chkbx.Visible = false;
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.ContextObjectId = null;
            Session["TaskRequestedBy"] = null;
            CloseWindow();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SetDelegationValidationMessage("");
            var message = Save();
            if (string.IsNullOrWhiteSpace(message))
            {
                this.ContextObjectId = null;
                TaskGrid.Rebind();
                Session["TaskRequestedBy"] = null;
                CloseWindow();
            }
            else
            {
                SetDelegationValidationMessage(message);
                //ShowWindow();
            }
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            Tasklist.VisibleOnPageLoad = false;
        }
        protected void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            var x = sender as CheckBox;
            if (x != null)
            {
                bool checkStatus = x.Checked;
                foreach (GridDataItem item in TaskGrid.MasterTableView.Items)
                {
                    CheckBox chkbx = (CheckBox)item["CheckTemp"].FindControl("ClientSelectColumn");
                    if (chkbx.Visible)
                    {
                        chkbx.Checked = checkStatus;
                        item.Selected = chkbx.Checked;
                    }
                }

            }
            if (TaskGrid.SelectedItems != null && TaskGrid.SelectedItems.Count > 0)
            {
                GridDataItem g = TaskGrid.SelectedItems[0] as GridDataItem;
                this.ContextObjectId = (long)g.GetDataKeyValue("TaskId");
                this.TypeofRequest = Convert.ToString(g.GetDataKeyValue("RequestType"));
                TaskStatusType = (StatusType)Enum.Parse(typeof(StatusType), g["StatusType"].Text);
                Session["TaskRequestedBy"] = g["RequestedBy"].Text;
            }
        }

        protected void ClientSelectColumn_CheckedChanged(object sender, EventArgs e)
        {
            int gridcount = TaskGrid.MasterTableView.Items.Count;
            int selectedcount = 0;
            foreach (GridDataItem item in TaskGrid.MasterTableView.Items)
            {
                CheckBox chkbx = (CheckBox)item["CheckTemp"].FindControl("ClientSelectColumn");
                if (chkbx.Checked)
                    selectedcount++;
                item.Selected = chkbx.Checked;
            }

            if (TaskGrid.SelectedItems.Count > 0)
            {
                GridDataItem g = TaskGrid.SelectedItems[0] as GridDataItem;
                this.ContextObjectId = (long)g.GetDataKeyValue("TaskId");
                this.TypeofRequest = Convert.ToString(g.GetDataKeyValue("RequestType"));
            }
            foreach (GridItem item in TaskGrid.MasterTableView.GetItems(GridItemType.Header))
            {
                CheckBox chkbx = (CheckBox)item.FindControl("CheckBox2");
                if (selectedcount == gridcount)
                {
                    chkbx.Checked = true;
                }
                else
                {
                    chkbx.Checked = false;
                }
            }
            if (selectedcount > 0)
            {
                Session["TaskRequestedBy"] = null;
                var tmpLstRequestFor = new List<string>();
                foreach (GridDataItem row in TaskGrid.MasterTableView.Items)
                {
                    CheckBox chk = (CheckBox)row.FindControl("ClientSelectColumn");
                    if (chk.Checked)
                    {
                        tmpLstRequestFor.AddRange(row["RequestedFor"].Text.Split(','));
                    }
                }
                LstRequestedFor = tmpLstRequestFor;
                foreach (GridDataItem row in TaskGrid.MasterTableView.Items)
                {
                    CheckBox chk = (CheckBox)row.FindControl("ClientSelectColumn");
                    if (chk.Checked == true)
                    {
                        this.ContextObjectId = (long)row.GetDataKeyValue("TaskId");
                        this.TypeofRequest = Convert.ToString(row.GetDataKeyValue("RequestType"));
                        TaskStatusType = (StatusType)Enum.Parse(typeof(StatusType), row["StatusType"].Text);
                        Session["TaskRequestedBy"] = row["RequestedBy"].Text;
                        return;
                    }
                    long AccessRequestID = Convert.ToInt64(row.GetDataKeyValue("AccessRequestID"));
                }
            }
            // GridDataItem selectedItem = (GridDataItem)chk.Parent.Parent;
        }

        #endregion

        protected void DatePickerValidTo_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
        {
            DatePickerValidTo.Enabled = true;
        }

        protected void Filter_Click(object sender, EventArgs e)
        {
            TaskGrid.ClearFilter();
            TaskGrid.CurrentPageIndex = 0;
            //if (IsAdminMode == IsAdminMode.Yes)
            //{
            //    if (RequestForUserName.Entries.Count == 0)
            //    {
            //        //if (ddlTaskStatus.SelectedValue != ((int)StatusType.Pending).ToString() && ddlEndpointSystemId.SelectedIndex == 0)
            //        //{
            //        //    ShowAlertMessage(AlertType.Error, "Please select Application or Request For");
            //        //    return;
            //        //}
            //        //if (ddlTaskStatus.SelectedIndex == 0 || ddlTaskStatus.SelectedValue == ((int)StatusType.Approved).ToString())
            //        //{
            //        //    var retMsg = isValidDateRange();
            //        //    if (!string.IsNullOrWhiteSpace(retMsg))
            //        //    {
            //        //        ShowAlertMessage(AlertType.Error, retMsg);
            //        //        return;
            //        //    }
            //        //}
            //    }
            //}
            //else
            //{
            //    //if (ddlTaskStatus.SelectedIndex == 0 || ddlTaskStatus.SelectedValue == ((int)StatusType.Approved).ToString())
            //    //{
            //    //    var retMsg = isValidDateRange();
            //    //    if (!string.IsNullOrWhiteSpace(retMsg))
            //    //    {
            //    //        ShowAlertMessage(AlertType.Error, retMsg);
            //    //        return;
            //    //    }
            //    //}
            //}
            Refresh();
        }

        protected void ClearFilter_Click(object sender, EventArgs e)
        {
            DatePickerValidFrom.SelectedDate = null;
            DatePickerValidTo.SelectedDate = null;
            ddlEndpointSystemId.SelectedIndex = 0;
            RequestForUserName.Entries.Clear();
            ddlTaskStatus.ClearSelection();
            ddlTaskStatus.Items.FindByValue(((int)StatusType.Pending).ToString()).Selected = true;
            TaskGrid.CurrentPageIndex = 0;
            Refresh();
        }
        protected void lnkTaskId_Command(object sender, CommandEventArgs e)
        {
            GridDataItem item = TaskGrid.Items[Convert.ToInt32(e.CommandArgument)];
            if (item == null)
                return;

            Session["TaskRequestedBy"] = null;
            this.ContextObjectId = (long)item.GetDataKeyValue("TaskId");
            this.TypeofRequest = Convert.ToString(item.GetDataKeyValue("RequestType"));
            TaskStatusType = (StatusType)Enum.Parse(typeof(StatusType), item["StatusType"].Text);
            Session["TaskRequestedBy"] = item["RequestedBy"].Text;
            var requestFor = RequestForUserName.Entries.Count > 0 ? RequestForUserName.Entries[0].Value : "";
            var requestForName = RequestForUserName.Entries.Count > 0 ? RequestForUserName.Entries[0].Text : "";
            var datefilter = string.Empty;
            if(DatePickerValidFrom.SelectedDate.HasValue && DatePickerValidTo.SelectedDate.HasValue)
            {
                datefilter = $"&fromDate={DatePickerValidFrom.SelectedDate.Value}&toDate={DatePickerValidTo.SelectedDate.Value}";
            }

            Response.Redirect($"DetailType.aspx?contextId={this.ContextObjectId}&TypeOfRequest={this.TypeofRequest}&source=mytask&ddltaskstatus={ddlTaskStatus.SelectedIndex}&TaskId={this.ContextObjectId}&requestFor={requestFor}&{datefilter}&requestForName={requestForName}&endpointsystemId={ddlEndpointSystemId.SelectedValue}");
        }
        
        protected override IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request ID", "accessrequestid", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Task ID", "taskid", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Application Name", "eventname", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Task Source", "tasksource", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Account Type", "accounttype", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request Type", "type", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Task Type", "requesttypename", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request For", "requestedfor", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request By", "requestedby", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Completion Date", "completiondate", "dd-MM-yyyy"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Is Delegated", "isdelegate", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Entitlement", "entitlement", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Requested Date", "requesteddate", "dd-MM-yyyy hh:mm tt"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Created Date", "createddate", "dd-MM-yyyy hh:mm tt"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Start Date", "startdatetoshort", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("End Date", "enddateshort", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Status", "statustype", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Action User", "actionuser", null));
            return param;
        }

        protected void ddlEndPointSystemType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        void BindApplicationType()
        {
            ddlEndpointSystemId.Items.Clear();
            ddlEndpointSystemId.Items.Insert(0, new ListItem("---- All ----", "0"));
            var ep = DomainApiClient<Dto.EndPoint>.GetAll().Where(i=>i.IsTrustedSource == false).OrderBy(i=>i.DisplayName);

            foreach (var item in ep)
            {
                var listItem = new ListItem(item.DisplayName, item.Id.ToString());
                ddlEndpointSystemId.Items.Add(listItem);
            }
        }

        protected void RequestForUserName_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {

        }

        protected void RequestForUserName_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {

        }

        protected void ddlTaskStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (ddlTaskStatus.SelectedValue == ((int)StatusType.Pending).ToString() )
            {
                Refresh();
            }
            //else
            //{
            //    var err = IsValidForSearch();
            //    if (!string.IsNullOrWhiteSpace(err))
            //    {
            //        TaskGrid.DataSource = new List<Dto.Presentation.PresentationTask>();
            //        TaskGrid.DataBind();
            //    }
            //}
        }
        private string IsValidForSearch()
        {
            string isValidSearch = string.Empty;
            //if (divRequestFor.Visible)
            //{
            //    //if (RequestForUserName.Entries.Count == 0)
            //    //{
            //    //    if (ddlTaskStatus.SelectedValue != ((int)StatusType.Pending).ToString() && ddlEndpointSystemId.SelectedIndex == 0)
            //    //    {
            //    //        isValidSearch = "Please select Application or Request For";
            //    //        return isValidSearch;
            //    //    }
            //    //    if (ddlTaskStatus.SelectedIndex == 0 || ddlTaskStatus.SelectedValue == ((int)StatusType.Approved).ToString())
            //    //    {
            //    //        isValidSearch = isValidDateRange();
            //    //        return isValidSearch;
            //    //    }
            //    //}
            //}
            //else
            //{
            //    //if (ddlTaskStatus.SelectedIndex == 0 || ddlTaskStatus.SelectedValue == ((int)StatusType.Approved).ToString())
            //    //{
            //    //    isValidSearch = isValidDateRange();
            //    //}
            //}
            return isValidSearch;
        }
    }
}