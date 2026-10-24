using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Dto;
using Telerik.Web.UI;
using Application;
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Web.Prov.UC
{
    public partial class SubmitRequest : UIBaseUC, IAccessControl
    {


        #region Members

        private const string KEY_CrntUserList = @"APAC_KEY_CrntUserList";
        private const string KEY_InlcudeADhocRequest = @"KEY_InlcudeADhocRequest";
        SessionStatusImpl sessionStatus;
        private List<FailedRequestGridData> m_reqList;
        private List<MyRequestsGridData> m_reqList1;
        bool _includeAdhocAccessRequest = false;

        public List<FailedRequestGridData> RequestList
        {
            get
            {
                if (m_reqList == null)
                    m_reqList = Session[KEY_CrntUserList] as List<FailedRequestGridData>;
                return m_reqList;
            }
            set
            {
                m_reqList = value;
                Session[KEY_CrntUserList] = m_reqList;
            }
        }

        public List<MyRequestsGridData> MyRequestList
        {
            get
            {
                if (m_reqList1 == null)
                    m_reqList1 = Session[KEY_CrntUserList] as List<MyRequestsGridData>;
                return m_reqList1;
            }
            set
            {
                m_reqList1 = value;
                Session[KEY_CrntUserList] = m_reqList1;
            }
        }

        public bool IncludeAdhocAccessRequest
        {
            get
            {
                if (ViewState[KEY_InlcudeADhocRequest] == null)
                    return false;
                return (bool)ViewState[KEY_InlcudeADhocRequest];
            }
            set
            {
                ViewState[KEY_InlcudeADhocRequest] = value;
            }
        }


        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["Prev"] == "reqDet")
                {
                    ExecutionFailedRadioButton.Checked = true;
                    grdNewUsers.ExportSettings.Pdf.PageHeader.MiddleCell.Text = "Execution failed requests.";
                }
                else
                {
                    grdNewUsers.ExportSettings.Pdf.PageHeader.MiddleCell.Text = "Validation failed requests.";
                }
                grdNewUsers.ExportSettings.Pdf.PageHeader.MiddleCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
                grdNewUsers.ExportSettings.Pdf.PageHeader.LeftCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
                
            }

            sessionStatus = new SessionStatusImpl();

            InvisibleBetween(grdNewUsers.FilterMenu);

            if (IncludeAdhocAccessRequest)
            {
                ValidationFailedRadioButton.Visible = true;
                ValidationFailedRadioButton.Text = "Validation Failed Requests";
                ExecutingRadioOption.Text = "Executing Access Requests";
                ExecutionFailedRadioButton.Text = "Execution Failed Requests";
                WorkflowNotFoundRadioButton.Text = "Workflow Not Found Requests";
                grdNewUsers.Visible = false;
                grdRequest.Visible = true;
            }
            else
            {
                ValidationFailedRadioButton.Visible = false;
                ValidationFailedRadioButton.Text = "";
                ExecutingRadioOption.Text = "Executing Access Requests";
                ExecutionFailedRadioButton.Text = "Execution Failed Requests";
                grdNewUsers.Visible = true;
                grdRequest.Visible = false;
            }
            if (!IsPostBack)
            {
                if (Request.QueryString["IncludeAdhocAccessRequest"] == "1")
                {
                    var failedReqType = Request.QueryString["failedRequestType"];
                    if (failedReqType == "1")
                    {
                        ExecutingRadioOption.Checked = true;
                        ValidationFailedRadioButton.Checked = false;
                        ExecutionFailedRadioButton.Checked = false;
                        WorkflowNotFoundRadioButton.Checked = false;
                    }
                    else if (failedReqType == "2")
                    {
                        ExecutingRadioOption.Checked = false;
                        ValidationFailedRadioButton.Checked = true;
                        ExecutionFailedRadioButton.Checked = false;
                        WorkflowNotFoundRadioButton.Checked = false;
                    }
                    else if (failedReqType == "3")
                    {
                        ExecutingRadioOption.Checked = false;
                        ValidationFailedRadioButton.Checked = false;
                        ExecutionFailedRadioButton.Checked = true;
                        WorkflowNotFoundRadioButton.Checked = false;
                    }
                    else if (failedReqType == "4")
                    {
                        ExecutingRadioOption.Checked = false;
                        ValidationFailedRadioButton.Checked = false;
                        ExecutionFailedRadioButton.Checked = false;
                        WorkflowNotFoundRadioButton.Checked = true;
                    }
                }
            }

        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (IncludeAdhocAccessRequest)
            {
                if (e.ActionType == ACLAction.Submit)
                {
                    AccessRequestProgress accessRequestProgress = AccessRequestProgress.ReadytoExecute;
                    if (WorkflowNotFoundRadioButton.Checked)
                    {
                        accessRequestProgress = AccessRequestProgress.Submitted;
                        DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "WorkflowNotFoundRequest - Submit");
                    }
                    else
                    {
                        DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "SubmitFailedRequest - Submit");
                    }
                    UpdateProvisioningEntries(accessRequestProgress);
                }
                else if (e.ActionType == ACLAction.ManuallyResolved)
                {
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "SubmitFailedRequest - ResolvedManually");
                    UpdateProvisioningEntries(AccessRequestProgress.ManuallyResolved);
                }
            }
            else
            {
                if (e.ActionType == ACLAction.Submit)
                {
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "SubmitFailedRequest - Submit");
                    UpdateEntries(AccessRequestProgress.ReadytoExecute);
                }
                else if (e.ActionType == ACLAction.ManuallyResolved)
                {
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "SubmitFailedRequest - ResolvedManually");
                    UpdateEntries(AccessRequestProgress.ManuallyResolved);
                }
            }
        }
        void InvisibleBetween(GridFilterMenu menu)
        {
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "Between")
                {
                    menu.Items.RemoveAt(i);
                }
                if (menu.Items[i].Text == "NotBetween")
                {
                    menu.Items.RemoveAt(i);
                }

                i++;
            }
        }
        public void UpdateEntries(AccessRequestProgress requstProgress)
        {

            int accessRequestId;
            int requestCount = 0;
            User user;
            bool isIntValue;
            List<string> invalidMatrix = new List<string>();

            foreach (GridDataItem grow in grdNewUsers.Items) // (GridViewRow grow in grdNewUsers.Rows)
            {
                CheckBox checkbox = (CheckBox)grow.FindControl("chkUser");
                if (checkbox.Checked)
                {
                    HiddenField hdUsedId = (HiddenField)grow.FindControl("hdnUserid");
                    isIntValue = int.TryParse(hdUsedId.Value, out accessRequestId);
                    if (isIntValue && accessRequestId > 0)
                    {

                        var request = RequestList.FirstOrDefault(item => item.AccessRequestId == accessRequestId);
                        user = DomainApiClient<Dto.User>.GetById(request.UserId);

                        if (user != null)
                        {
                            requestCount += 1;
                            var accessRequest = DomainApiClient<Dto.AccessRequest>.GetById(request.AccessRequestId);

                            accessRequest.ProgressStatus = requstProgress;
                            accessRequest.LastModifiedBy = sessionStatus.User.UserName;
                            accessRequest.LastModifiedDate = DateTime.Now;
                            DomainApiClient<Dto.AccessRequest>.Save(accessRequest);

                            UILog log = new UILog();
                            if (requstProgress == AccessRequestProgress.ManuallyResolved)
                            {
                                log.Message = $"Failed Access Rquest of Id is {accessRequest.Id} is manually Resovled By {new SessionStatusImpl().User.UserName}";
                            }
                            else
                            {
                                log.Message = $"Failed Access Rquest of Id is {accessRequest.Id} is Re-Submited By {new SessionStatusImpl().User.UserName}";

                            }
                            log.LogDate = DateTime.Now;
                            log.LogLevel = "Info";
                            log.UserName = "System";
                            DomainApiClient<UILog>.GetByAction<object>(DomainApiAction.Log.UILog.SaveUILog, null, log);

                        }
                    }
                }
            }
            if (invalidMatrix.Count != 0)
            {
                string errorMessage = string.Empty;
                if (invalidMatrix.Count != 0)
                {
                    errorMessage += string.Format("Provisioning default info is missing for the following user(s) {0}. ", String.Join(",", invalidMatrix));
                }
                ShowAlertMessage(Infrastructure.AlertType.Error, errorMessage);
            }
            else if (requestCount == 0)
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "No User selected for Provisioning.");
            }
            else
            {
                ShowAlertMessage(Infrastructure.AlertType.Success, "Data Saved Successfully!");
            }

            BindPageGridData();
        }


        public void UpdateProvisioningEntries(AccessRequestProgress requestProgress)
        {
            int accessRequestId;
            int requestCount = 0;
            User user;
            bool isIntValue;
            List<string> invalidMatrix = new List<string>();
            var actualStatus = requestProgress;
            foreach (GridDataItem grow in grdRequest.Items) // (GridViewRow grow in grdNewUsers.Rows)
            {
                requestProgress = actualStatus;
                CheckBox checkbox = (CheckBox)grow.FindControl("chkUser");
                if (checkbox.Checked)
                {
                    HiddenField hdUsedId = (HiddenField)grow.FindControl("hdnUserid");
                    isIntValue = int.TryParse(hdUsedId.Value, out accessRequestId);
                    if (isIntValue && accessRequestId > 0)
                    {

                        var request = MyRequestList.FirstOrDefault(item => item.AccessRequestID == accessRequestId);
                        //user = DomainApiClient<Dto.User>.GetById(request.UserID);
                        string message = string.Empty;
                        requestCount += 1;
                        if (requestProgress != AccessRequestProgress.ManuallyResolved)
                        {
                            if (request.EntitlementDetailType == ApplicationType.ADSDCLocal || request.EntitlementDetailType == ApplicationType.ADSingaporePowerLocal)
                            {
                                message = DomainApiClient<Dto.User>.GetByAction<string>(DomainApiAction.User.IsADDefaultsExists, new Dictionary<string, string> { { "events", 0.ToString()},{ "userId", request.UserID.ToString() } });
                            }
                            else if (request.EntitlementDetailType == ApplicationType.HRMS_FLINS)
                            {
                                message = DomainApiClient<Dto.User>.GetByAction<string>(DomainApiAction.User.IsFLINSDefaultsExists, new Dictionary<string, string> { { "events", 0.ToString() }, { "userId", request.UserID.ToString() } });
                            }
                            else if (request.EntitlementDetailType == ApplicationType.OneCard)
                            {
                                message = DomainApiClient<Dto.User>.GetByAction<string>(DomainApiAction.User.IsSMSDefaultsExists, new Dictionary<string, string> { { "events", 0.ToString() }, { "userId", request.UserID.ToString() } } );
                            }

                        }
                        if (string.IsNullOrEmpty(message))
                        {
                            var accessRequest = DomainApiClient<Dto.AccessRequest>.GetById(request.AccessRequestID);
                            if(requestProgress ==  AccessRequestProgress.ReadytoExecute && accessRequest.ApplicationType == ApplicationType.Office365 
                                && !string.IsNullOrWhiteSpace(accessRequest.AdditionalInfo))
                            {
                                if(accessRequest.AdditionalInfo == Constants.RequestAdditionalInfoLicense)
                                {
                                    requestProgress = AccessRequestProgress.WaitingForLicenseAssignment;
                                }
                                else if(accessRequest.AdditionalInfo == Constants.RequestAdditionalInfoMailBox)
                                {
                                    requestProgress = AccessRequestProgress.WaitingForMailBoxCreation;
                                }
                            }

                            accessRequest.ProgressStatus = requestProgress;
                            accessRequest.LastModifiedBy = sessionStatus.User.UserName;
                            accessRequest.LastModifiedDate = DateTime.Now;

                            DomainApiClient<Dto.AccessRequest>.Save(accessRequest);

                            if (accessRequest.BulkRequestId.HasValue && requestProgress == AccessRequestProgress.ManuallyResolved)
                            {
                                Dictionary<string, string> parameters = new Dictionary<string, string>();
                                parameters.Add("bulkRequestId", accessRequest.BulkRequestId.Value.ToString());
                                parameters.Add("currentUser", sessionStatus.User.UserName);
                                bool response = DomainApiClient<Dto.BulkRequest>.GetByAction<bool>("UpdateStatusWhenResolved", parameters);
                                if (response)
                                    DILog.UILogger.WriteLog(sessionStatus.User.Id, sessionStatus.User.UserName, LogLevelType.Info, string.Format("Updated bulk request {0} status as completed", accessRequest.BulkRequestId.Value), null);
                            }

                            UILog log = new UILog();
                            if (requestProgress == AccessRequestProgress.ManuallyResolved)
                            {
                                log.Message = string.Format("Failed Adhoc Access Rquest of Id is {0} is manually Resovled By {1}", accessRequest.Id, new SessionStatusImpl().User.UserName);
                            }
                            else
                            {
                                if (WorkflowNotFoundRadioButton.Checked == true)
                                {
                                    log.Message = string.Format("Workflow not found request is changed to Submited - By {1}", accessRequest.Id, new SessionStatusImpl().User.UserName);
                                }
                                else
                                {
                                    log.Message = string.Format("Failed Adhoc Access Rquest of Id is {0} is Re-Submited By {1}", accessRequest.Id, new SessionStatusImpl().User.UserName);
                                }
                            }

                            log.LogDate = DateTime.Now;
                            log.LogLevel = "Info";
                            log.UserName = "System";
                            DomainApiClient<UILog>.GetByAction<object>(DomainApiAction.Log.UILog.SaveUILog, null, log);

                        }
                        else
                        {
                            user = DomainApiClient<Dto.User>.GetById(request.UserID);
                            invalidMatrix.Add(user.UserName);
                            continue;
                        }
                    }
                }
            }

            if (invalidMatrix.Count != 0)
            {
                string errorMessage = string.Empty;
                if (invalidMatrix.Count != 0)
                {
                    errorMessage += string.Format("Provisioning default info is missing for the following user(s) {0}. ", String.Join(",", invalidMatrix));
                }
                ShowAlertMessage(Infrastructure.AlertType.Error, errorMessage);
            }
            else if (requestCount == 0)
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "No User selected for Provisioning.");
            }
            else
            {
                ShowAlertMessage(Infrastructure.AlertType.Success, "Data Saved Successfully!");
            }



            BindFailedRequestGridData();
            grdRequest.DataBind();
        }

        protected void grdRequest_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                GridDataItem g = grdRequest.SelectedItems[0] as GridDataItem;
                this.ContextObjectId = Convert.ToInt64(g.GetDataKeyValue("AdHocAccessRequestId"));//==0 && (long)g.GetDataKeyValue(";
                this.TypeofRequest = Constants.AccessRequest;
            }
            catch
            {
            }
        }

        private void BindPageGridData()
        {
            BindFailedRequestGridData();
            grdRequest.DataBind();
            grdNewUsers.DataBind();
        }

        protected void grdNewUsers_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "RequestDetails")
            {
                Response.Redirect("~/Home/MyRequestDetails.aspx?Id=" + e.CommandArgument + "&prev=failed");
            }
            else if (e.CommandName == "ErrorDetails")
            {
                txtError.Text = e.CommandArgument.ToString();
                UserOldNewDetailsModalPopupExtender.VisibleOnPageLoad = true;
            }


        }


        protected void grdNewUsers_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (!IncludeAdhocAccessRequest)
            {
                BindFailedRequestGridData();
                 
            }

        }

        protected void grdRequest_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (IncludeAdhocAccessRequest)
            {
                BindFailedRequestGridData();
                 
            }
        }


        protected void btnOk_Close(object sender, EventArgs e)
        {
            UserOldNewDetailsModalPopupExtender.VisibleOnPageLoad = false;
        }
        void BindFailedRequestGridData()
        {
            if (IncludeAdhocAccessRequest)
            {
                if (ExecutingRadioOption.Checked == true)
                {
                    grdRequest.DataSource = GetMyRequests(AccessRequestProgress.Executing);
                }
                else if (ExecutionFailedRadioButton.Checked == true)
                {
                    grdRequest.DataSource = this.GetMyRequests(AccessRequestProgress.ExecutionFailed);
                }
                else if (ValidationFailedRadioButton.Checked == true)
                {
                    grdRequest.DataSource = this.GetMyRequests(AccessRequestProgress.ValidationFailed);
                }
                else if (WorkflowNotFoundRadioButton.Checked == true)
                {
                    grdRequest.DataSource = this.GetMyRequests(AccessRequestProgress.WorkflowNotFound);
                }
                //grdRequest.DataBind();
            }
            else
            {
                if (ValidationFailedRadioButton.Checked == true)
                {
                    grdNewUsers.DataSource = this.GetFailedRequestGridData(AccessRequestProgress.ValidationFailed);
                    //grdNewUsers.MasterTableView.Columns.FindByUniqueName("ErrorMessage").Display = true;
                }
                else if (ExecutingRadioOption.Checked == true)
                {
                    grdNewUsers.DataSource = GetFailedRequestGridData(AccessRequestProgress.Executing);
                    //grdNewUsers.MasterTableView.Columns.FindByUniqueName("ErrorMessage").Display = false;
                }
                else if (WorkflowNotFoundRadioButton.Checked == true)
                {
                    grdNewUsers.DataSource = GetFailedRequestGridData(AccessRequestProgress.WorkflowNotFound);
                    //grdNewUsers.MasterTableView.Columns.FindByUniqueName("ErrorMessage").Display = false;
                }
                else
                {
                    //grdNewUsers.DataSource = null; 
                    grdNewUsers.DataSource = this.GetFailedRequestGridData(AccessRequestProgress.ExecutionFailed);
                    //grdNewUsers.MasterTableView.Columns.FindByUniqueName("ErrorMessage").Display = false;
                }
            }
        }



        protected void FailedSelectionChanged(object sender, EventArgs e)
        {
            BindPageGridData();
        }

        List<MyRequestsGridData> GetMyRequests(AccessRequestProgress progressStatus)
        {
            List<MyRequestsGridData> Requests = new List<MyRequestsGridData>();

            Requests = DomainApiClient<Dto.AccessRequest>.GetByAction<List<Dto.MyRequestsGridData>>(DomainApiAction.Provisioning.AccessRequest.GetAdhocRequestsByProgressStatus,
                new Dictionary<string, string> { { "progressStatus", Convert.ToString((long)progressStatus) } });
            this.MyRequestList = Requests;
            return Requests;
        }

        private List<FailedRequestGridData> GetFailedRequestGridData(AccessRequestProgress progressStatus)
        {
            try
            {
                List<FailedRequestGridData> failedRequests = new List<FailedRequestGridData>();

                failedRequests = DomainApiClient<Dto.AccessRequest>.GetByAction<List<FailedRequestGridData>>(DomainApiAction.Provisioning.AccessRequest.GetFailedSystemRequests,
                   new Dictionary<string, string> { { "progressStatus", Convert.ToString((long)progressStatus) } });
                this.RequestList = failedRequests;
                return failedRequests;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected void btnAPACCancel_Click(object sender, EventArgs e)
        {

            Response.Redirect("~/home/Home.aspx");
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            if (!IncludeAdhocAccessRequest)
            {
                grdNewUsers.Rebind();
            }
            else
            {
                grdRequest.Rebind();
            }
        }
        protected void lnkTaskId_Command(object sender, CommandEventArgs e)
        {
            GridDataItem item = grdNewUsers.Items[Convert.ToInt32(e.CommandArgument)];
            if (item == null)
                return;

            this.TypeofRequest = item.GetDataKeyValue("RequestSource").ToString();
            if (item.GetDataKeyValue("AccessRequestId") != null)
            {
                this.ContextObjectId = (long)item.GetDataKeyValue("AccessRequestId");
            }
            else
            {
                this.ContextObjectId = 0;
            }
            if (this.ContextObjectId == null || this.ContextObjectId == 0)
            {
                MasterPage.ShowAlert(AlertType.Warning, "No detail available for this request.");
            }
            else
            {
                Response.Redirect($"/Prov/Request/Detail.aspx?contextId={this.ContextObjectId}&TypeofRequest={TypeofRequest}");
            }
        }
        protected void lnkAdhocTaskId_Command(object sender, CommandEventArgs e)
        {
            GridDataItem item = grdRequest.Items[Convert.ToInt32(e.CommandArgument)];
            if (item == null)
                return;

            this.TypeofRequest = (string)item.GetDataKeyValue("RequestSource");
            if (item.GetDataKeyValue("AdHocAccessRequestId") != null)
            {
                this.ContextObjectId = (long)item.GetDataKeyValue("AdHocAccessRequestId");

                var AccessRequestId = (long)item.GetDataKeyValue("AccessRequestID");
                if (AccessRequestId > 0)
                {
                    var accessRequest = DomainApiClient<Dto.AccessRequest>.GetById(AccessRequestId);
                    if(accessRequest !=null && accessRequest.AdHocAccessRequestId.HasValue && accessRequest.BulkRequestId.HasValue)
                    {
                        //this.ContextObjectId = accessRequest.Id;
                        this.TypeofRequest = "Access Request";
                    }
                }
            }
            else
            {
                this.ContextObjectId = 0;
            }
            if (this.ContextObjectId == null || this.ContextObjectId == 0)
            {
                MasterPage.ShowAlert(AlertType.Warning, "No detail available for this request.");
            }
            else
            {
                var failedRequestType = 1;
                if (ExecutingRadioOption.Checked) failedRequestType = 1;
                if (ValidationFailedRadioButton.Checked) failedRequestType = 2;
                if (ExecutionFailedRadioButton.Checked) failedRequestType = 3;
                if (WorkflowNotFoundRadioButton.Checked) failedRequestType = 4;
                Response.Redirect($"/Prov/Request/Detail.aspx?contextId={this.ContextObjectId}&TypeofRequest={TypeofRequest}&IncludeAdhocAccessRequest={(IncludeAdhocAccessRequest?1:0)}&failedRequestType={failedRequestType}");
            }
        }
    }
}