using System;
using Application;
using System.Linq;
using Telerik.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Dto;
using System.Text.RegularExpressions;
using DI.Chakra.Dto.Core;
using System.Linq.Dynamic;

namespace DI.Chakra.Web.Prov.Request.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        private SessionStatusImpl SessionUser = new SessionStatusImpl();

        private const string V_REQUESTS = "V_REQUESTS";

        private List<PresentationAccessRequest> Requests
        {
            get
            {
                if (ViewState[V_REQUESTS] != null)
                {
                    return (List<PresentationAccessRequest>)ViewState[V_REQUESTS];
                }
                return new List<PresentationAccessRequest>();
            }
            set
            {
                ViewState[V_REQUESTS] = value;
            }
        }

        private List<PresentationAccessRequest> _requests = new List<PresentationAccessRequest>();

        private void BindProgressStatus()
        {
            this.MasterPage.DisableRedirect = false;

            ProgressStatus.Items.Clear();
            foreach (AccessRequestProgress item in Enum.GetValues(typeof(AccessRequestProgress)))
            {
                if (item == AccessRequestProgress.None)
                    continue;

                var description = UtilityMethods.GetEnumAttribute<Description>(item);
                ListItem listItem = new ListItem(description, ((byte)item).ToString());
                if (item == AccessRequestProgress.None)
                {
                    listItem.Text = "All";
                }
                ProgressStatus.Items.Add(listItem);
            }
            ProgressStatus.Items.Insert(0, "--- All ---");
            ProgressStatus.Items.FindByValue(Convert.ToInt16(AccessRequestProgress.WaitingforApproval).ToString()).Selected = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindProgressStatus();
                BindApplicationType();
                int progressStatus = !string.IsNullOrWhiteSpace(Request.QueryString["progressStatus"]) ? int.Parse(Request.QueryString["progressStatus"]) : 1;
                ProgressStatus.SelectedIndex = progressStatus;

                string fromDate = this.Request.QueryString["fromDate"] ?? "";
                string toDate = this.Request.QueryString["toDate"] ?? "";
                string requestFor = this.Request.QueryString["requestFor"] ?? "";
                string requestForName = this.Request.QueryString["requestForName"] ?? "";
                string epId = this.Request.QueryString["endpointsystemId"] ?? "";
                if (!string.IsNullOrWhiteSpace(requestFor) && !string.IsNullOrWhiteSpace(requestForName))
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
            }
            this.MasterPage.ActionClicked += ActionClicked;
            var _sessionStatus = new SessionStatusImpl();
            _sessionStatus.RedirectFrom = "AdminRequest";
        }

        protected void AccessRequest_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            Dictionary<string, string> parameter = GetKeyValuePairsWithDates();

            var requests = DomainApiClient<PresentationAccessRequest>.GetByAction<List<PresentationAccessRequest>>(DomainApiAction.Provisioning.AccessRequest.GetPagedAllRequestDataByProgressStatus, parameter);

            foreach (var request in requests)
            {
                request.EntitlementString = Utility.DeserializeEntitlement(request.Entitlement, request.ApplicationType);
            }
            Requests = requests;
            if (!string.IsNullOrWhiteSpace(AccessRequestGrid.MasterTableView.FilterExpression))
            {
                try
                {
                    Requests = requests.AsQueryable().Where(AccessRequestGrid.MasterTableView.FilterExpression).ToList();
                }
                catch
                {
                }
            }
            
            AccessRequestGrid.DataSource = requests;
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.ReSubmit)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Provisioning Request - Resubmit");
                bool itemSelected = false;
                string validationMessage = "";
                foreach (GridDataItem dataItem in this.AccessRequestGrid.MasterTableView.Items)
                {
                    var chkBox = dataItem.FindControl("ClientSelectColumn") as CheckBox;
                    if (chkBox != null && chkBox.Checked == true)
                    {
                        int AccessRequestID = Convert.ToInt32(dataItem.GetDataKeyValue("AccessRequestID"));
                        var Request = DomainApiClient<Dto.AccessRequest>.GetById(AccessRequestID);
                        if (Request.ProgressStatus == AccessRequestProgress.ValidationFailed || Request.ProgressStatus == AccessRequestProgress.ExecutionFailed)
                        {
                            itemSelected = true;
                            Request.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                            DomainApiClient<Dto.AccessRequest>.Save(Request);
                        }
                        else
                        {
                            validationMessage = string.IsNullOrEmpty(validationMessage) ? " " + Convert.ToString(AccessRequestID) : validationMessage + " ," + Convert.ToString(AccessRequestID);
                        }
                    }
                }
                if (itemSelected == false)
                {
                    MasterPage.ShowAlert(AlertType.Error, "No Request selected for Resubmission.");
                }

                AccessRequestGrid.Rebind();
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                if(Requests == null || Requests.Count == 0)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "No records found");
                    return;
                }
                GenericExport(Requests, e.ActionType, "Requests");
                
            }
        }

        protected void ProgressStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (ProgressStatus.SelectedValue == ((int)AccessRequestProgress.WaitingforApproval).ToString())
            {
                RefreshGrid();
            }
            //else
            //{
            //    var err = IsValidToFilter(false);
            //    if (!string.IsNullOrWhiteSpace(err))
            //    {
            //        AccessRequestGrid.DataSource = new List<PresentationAccessRequest>();
            //        AccessRequestGrid.DataBind();
            //    }
            //}
        }

        protected void AccessRequestGrid_PreRender(object sender, EventArgs e)
        {
            foreach (GridDataItem row in AccessRequestGrid.Items)
            {
                AccessRequestProgress Status = (AccessRequestProgress)(row.GetDataKeyValue("ProgressStatus"));
                if (Status != AccessRequestProgress.ValidationFailed && Status != AccessRequestProgress.Executing && Status != AccessRequestProgress.ExecutionFailed && Status != AccessRequestProgress.WaitingForSync)
                    AccessRequestGrid.Items[row.ItemIndex].SelectableMode = GridItemSelectableMode.None;
            }
        }

        void SetDisableRedirect()
        {
            if (this.ContextObjectId == null || this.ContextObjectId == 0)
            {
                MasterPage.ShowAlert(AlertType.Warning, "No detail available for this request.");
                this.MasterPage.DisableRedirect = true;
            }
            else
                this.MasterPage.DisableRedirect = false;
        }

        protected void AccessRequestGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem Grid = AccessRequestGrid.SelectedItems[0] as GridDataItem;
            this.TypeofRequest = (string)Grid.GetDataKeyValue("RequestSource");
            if (Grid.GetDataKeyValue("AdHocAccessRequestId") != null)
            {
                this.ContextObjectId = (long)Grid.GetDataKeyValue("AdHocAccessRequestId");
                SetDisableRedirect();
            }
            else
            {
                SetDisableRedirect();
                this.ContextObjectId = 0;
            }
        }

        protected void AccessRequestGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "ShowInfo")
            {
                GridDataItem item = e.Item as GridDataItem;
                // passing Access Request ID to InfoUC
                // If ad hoc access request, Info UC will retrieve the id itself
                if (item.GetDataKeyValue("Id") != null)
                {
                    InfoUC.AccessRequestId = Convert.ToInt32((long)item.GetDataKeyValue("Id"));
                    RequestInfo.VisibleOnPageLoad = true;
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RequestInfo.VisibleOnPageLoad = false;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            AccessRequestGrid.Rebind();
        }

        protected void lnkTaskId_Command(object sender, CommandEventArgs e)
        {
            bool isHRMSRequest = false;
            GridDataItem item = AccessRequestGrid.Items[Convert.ToInt32(e.CommandArgument)];
            if (item == null)
                return;

            this.TypeofRequest = (string)item.GetDataKeyValue("RequestSource");
            if (item.GetDataKeyValue("AdHocAccessRequestId") != null)
            {
                this.ContextObjectId = (long)item.GetDataKeyValue("AdHocAccessRequestId");
            }
            else
            {
                this.ContextObjectId = 0;

                if(this.TypeofRequest == AccessRequestSource.ReOrganisation.ToString() || this.TypeofRequest == AccessRequestSource.OUChange.ToString() || this.TypeofRequest == AccessRequestSource.OnBoarding.ToString()
                    || this.TypeofRequest == AccessRequestSource.UndoEvent.ToString() || this.TypeofRequest == AccessRequestSource.OffBoarding.ToString() || this.TypeofRequest == AccessRequestSource.Promotion.ToString() || this.TypeofRequest == AccessRequestSource.Propagation.ToString()
                    )
                {
                    isHRMSRequest = true;
                    var requestFor = RequestForUserName.Entries.Count > 0 ? RequestForUserName.Entries[0].Value : "";
                    var requestForName = RequestForUserName.Entries.Count > 0 ? RequestForUserName.Entries[0].Text : "";
                    var datefilter = string.Empty;

                    if (DatePickerValidFrom.SelectedDate.HasValue && DatePickerValidTo.SelectedDate.HasValue)
                    {
                        datefilter = $"&fromDate={DatePickerValidFrom.SelectedDate.Value}&toDate={DatePickerValidTo.SelectedDate.Value}";
                    }
                    this.ContextObjectId = (long)item.GetDataKeyValue("AccessRequestID");
                    Response.Redirect($"Detail.aspx?HRMSRequest=1&&contextId={this.ContextObjectId}&TypeOfRequest={this.TypeofRequest}&progressStatus={this.ProgressStatus.SelectedIndex}&requestFor={requestFor}&{datefilter}&requestForName={requestForName}&endpointsystemId={ddlEndpointSystemId.SelectedValue}");
                }
            }
            if (isHRMSRequest == false)
            {
                if (this.ContextObjectId == null || this.ContextObjectId == 0)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "No detail available for this request.");
                }
                else
                {
                    var requestFor = RequestForUserName.Entries.Count > 0 ? RequestForUserName.Entries[0].Value : "";
                    var requestForName = RequestForUserName.Entries.Count > 0 ? RequestForUserName.Entries[0].Text : "";
                    var datefilter = string.Empty;

                    if (DatePickerValidFrom.SelectedDate.HasValue && DatePickerValidTo.SelectedDate.HasValue)
                    {
                        datefilter = $"&fromDate={DatePickerValidFrom.SelectedDate.Value}&toDate={DatePickerValidTo.SelectedDate.Value}";
                    }

                    Response.Redirect($"Detail.aspx?contextId={this.ContextObjectId}&TypeOfRequest={this.TypeofRequest}&progressStatus={this.ProgressStatus.SelectedIndex}&requestFor={requestFor}&{datefilter}&requestForName={requestForName}&endpointsystemId={ddlEndpointSystemId.SelectedValue}");
                }
            }
        }

        protected void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            var x = sender as CheckBox;
            if (x != null)
            {
                bool checkStatus = x.Checked;
                foreach (GridDataItem item in AccessRequestGrid.MasterTableView.Items)
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

        protected void AccessRequestGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                var isExecutionFailed = item.GetDataKeyValue("ProgressStatus")?.ToString();
                var checkBox = item.FindControl("ClientSelectColumn") as CheckBox;
                if (checkBox != null)
                {
                    if (!string.IsNullOrEmpty(isExecutionFailed) && isExecutionFailed.Equals(AccessRequestProgress.ExecutionFailed.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        checkBox.Enabled = true;
                        checkBox.Visible = true;
                    }
                    else
                    {
                        checkBox.Enabled = false;
                        checkBox.Visible = false;
                    }
                }
            }
        }

        protected void DatePickerValidTo_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
        {
            DatePickerValidTo.Enabled = true;
        }

        protected void Filter_Click(object sender, EventArgs e)
        {
            //if (string.IsNullOrWhiteSpace(IsValidToFilter()))
            {
                RefreshGrid();
            }
        }

        protected void ClearFilter_Click(object sender, EventArgs e)
        {
            DatePickerValidFrom.SelectedDate = null;
            DatePickerValidTo.SelectedDate = null;
            ddlEndpointSystemId.SelectedIndex = 0;
            RequestForUserName.Entries.Clear();
            ProgressStatus.ClearSelection();
            ProgressStatus.Items.FindByValue(((int)AccessRequestProgress.WaitingforApproval).ToString()).Selected = true;
            AccessRequestGrid.CurrentPageIndex = 0;
            RefreshGrid();
            AccessRequestGrid.ClearFilter();
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

        Dictionary<string, string> GetKeyValuePairsWithDates()
        {
            var dic = new Dictionary<string, string>();

            if (RequestForUserName.Entries.Count > 0)
            {
                dic.Add("requestedFor", RequestForUserName.Entries[0].Value);
            }
            else
            {
                dic.Add("requestedFor", "0");
            }

            if (ddlEndpointSystemId.SelectedIndex > 0)
            {
                dic.Add("endpointSystemId", ddlEndpointSystemId.SelectedValue);
            }
            else
            {
                dic.Add("endpointSystemId", "0");
            }

            if (ProgressStatus.SelectedValue.ToLower().Contains("all"))
                dic.Add("progressStatus", "None");
            else
                dic.Add("progressStatus", ProgressStatus.SelectedValue);

            DateTime selectedDate;
            if (DatePickerValidFrom.SelectedDate.HasValue)
            {
                selectedDate = DatePickerValidFrom.SelectedDate.Value;
                var date = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 0, 0, 0);
                dic.Add("fromDate", date.ToString(Constants.DateFormat));
            }
            else
            {
                dic.Add("fromDate", null);
            }

            if (DatePickerValidTo.SelectedDate.HasValue)
            {
                selectedDate = DatePickerValidTo.SelectedDate.Value;
                var date = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 23, 59, 59);
                dic.Add("toDate", date.ToString(Constants.DateFormat));
            }
            else
            {
                dic.Add("toDate", null);
            }

            return dic;
        }

        public void RefreshGrid()
        {
            AccessRequestGrid.Rebind();
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

        public string IsValidToFilter(bool showpopup = true)
        {
            string errorMessage = string.Empty;
            if (RequestForUserName.Entries.Count == 0)
            {
                if (ProgressStatus.SelectedValue == ((int)AccessRequestProgress.Completed).ToString() && ddlEndpointSystemId.SelectedIndex == 0)
                {
                    errorMessage = "Please select Application or Request For";
                    if (showpopup)
                    {
                        ShowAlertMessage(AlertType.Error, errorMessage);
                        return errorMessage;
                    }
                }
                if (ProgressStatus.SelectedIndex == 0 || ProgressStatus.SelectedValue == ((int)AccessRequestProgress.Completed).ToString())
                {
                    var retMsg = isValidDateRange();
                    if (!string.IsNullOrWhiteSpace(retMsg))
                    {
                        errorMessage = retMsg;
                        if (showpopup)
                        {
                            ShowAlertMessage(AlertType.Error, retMsg);
                            return errorMessage;
                        }
                    }
                }
            }
            return errorMessage;
        }

        protected override IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request ID", "requestid", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Trigger ID", "triggerid", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Application Name", "applicationname", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request Type", "requesttype", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request For", "requestfor", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request By", "requestby", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Execution Time", "executiontime", "dd-MM-yyyy hh:mm tt"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Status", "progressstatus", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request Source", "requestsource", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Account Type", "accounttype", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Entitlement", "entitlementstring", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request Date", "requestdate", "dd-MM-yyyy"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Start Date", "startdate", "dd-MM-yyyy"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("End Date", "enddate", "dd-MM-yyyy"));
            return param;
        }
    }
}