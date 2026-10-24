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
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Request.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        #region Members
        private SessionStatusImpl _m_session = new SessionStatusImpl();
        private static IList<Dto.Presentation.PresentationMyRequestData> requestsList;

        private AccessRequestProgress ItemProgressStatus
        {
            get
            {
                if (ViewState["RequestProgress"] != null)
                    return (AccessRequestProgress)ViewState["RequestProgress"];
                else
                    return AccessRequestProgress.None;
            }
            set
            {
                ViewState["RequestProgress"] = value;
            }

        }

        private long accessRequestId
        {
            get
            {
                if (ViewState["accessRequestId"] != null)
                    return (long)ViewState["accessRequestId"];
                else
                    return 0;
            }
            set
            {
                ViewState["accessRequestId"] = value;
            }

        }
        #endregion

        #region Properties
        private IList<Dto.Presentation.PresentationMyRequestData> RequestsData
        {
            get
            {
                if (ViewState["RequestsData"] != null)
                {
                    return (IList<Dto.Presentation.PresentationMyRequestData>)ViewState["RequestsData"];
                }
                return new List<Dto.Presentation.PresentationMyRequestData>();
            }
            set
            {
                ViewState["RequestsData"] = value;
            }
        }

        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            var _sessionStatus = new SessionStatusImpl();
            _sessionStatus.RedirectFrom = "MyRequest";
            divEmpEventChkBox.Visible = false;
            //divEmpEventChkBox.Visible = _m_session.User != null && _m_session.User.Id == 1;

            if (!IsPostBack)
            {
                ValidToPicker.SelectedDate = DateTime.Today;
            }
        }

        void SetDataSource()
        {
            if (_m_session.User != null && _m_session.User.Id == 1)
            {
                if (chkEmpEvent.Checked)
                {
                    requestsList =
                        DomainApiClient<Dto.Presentation.PresentationMyRequestData>.
                        GetByAction<IList<Dto.Presentation.PresentationMyRequestData>>
                        (DomainApiAction.Attestation.Presentation.PresentationMyRequest.GetOnlyEventTriggedAccessRequests, null);

                    RequestsData = requestsList;
                    SortBindDataSource();
                }
                else
                    SetAllData();
            }
            else
                SetAllData();
        }

        void SortBindDataSource()
        {
            if (RequestsData != null)
            {
                foreach (var requestData in RequestsData)
                {
                    requestData.EntitlementString = Utility.DeserializeEntitlement(requestData.Entitlement, requestData.ApplicationType);
                    requestData.Status = UtilityMethods.GetEnumAttribute<DisplayAs>(requestData.RequestStatus);
                }
                grdRequest.DataSource = RequestsData;
            }
        }

        void SetAllData()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("actionUserId", _m_session.User.Id.ToString());

            PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter
            {
                MaximumRows = -1,
            };

            var myRequests =
                DomainApiClient<Dto.Presentation.PresentationMyRequestData>.
                GetByAction<Dto.Core.PresentationGridPagedData<PresentationMyRequestData>>
                (DomainApiAction.Attestation.Presentation.PresentationMyRequest.GetPagedByActionUserId, parameter, param);

            foreach (var request in myRequests.Data)
            {
                request.EntitlementString = Utility.DeserializeEntitlement(request.Entitlement, request.ApplicationType);
                request.Status = UtilityMethods.GetEnumAttribute<DisplayAs>(request.RequestStatus);
            }

            grdRequest.DataSource = myRequests.Data;
        }

        protected void grdRequest_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            SetDataSource();
        }

        protected void grdRequest_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            GridDataItem g = grdRequest.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (string)g.GetDataKeyValue("RequestSource") == Constants.BulkRequest ? (long)g.GetDataKeyValue("BulkRequestId") : (long)g.GetDataKeyValue("AdHocAccessRequestId");//==0 && (long)g.GetDataKeyValue(";
            this.TypeofRequest = (string)g.GetDataKeyValue("RequestSource");
            this.accessRequestId = (long)g.GetDataKeyValue("AccessRequestId");
            this.ItemProgressStatus = (AccessRequestProgress)g.GetDataKeyValue("ProgressStatus");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var filterExpression = grdRequest.MasterTableView.FilterExpression;

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

                Dictionary<string, string> paramtr = new Dictionary<string, string>();
                paramtr.Add("actionUserId", _m_session.User.Id.ToString());

                PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter
                {
                    MaximumRows = -1,
                    FilterExpression = filterExpression,
                };

                var myRequests =
                    DomainApiClient<Dto.Presentation.PresentationMyRequestData>.
                    GetByAction<Dto.Core.PresentationGridPagedData<PresentationMyRequestData>>
                    (DomainApiAction.Attestation.Presentation.PresentationMyRequest.GetPagedByActionUserId, paramtr, param);

                GenericExport(myRequests.Data as List<PresentationMyRequestData>, e.ActionType, "MyRequests");

            }
            else if (e.ActionType == ACLAction.RequestExtend)
            {

                bool loadExtendPopup = false;
                Dto.AccessRequest accessRequest = null;
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("accessrequestId", Convert.ToString(this.accessRequestId));
                accessRequest = DomainApiClient<Dto.AccessRequest>.GetByAction<Dto.AccessRequest>(DomainApiAction.Provisioning.AccessRequest.GetAccessRequestById, parameters);

                if (this.ItemProgressStatus == AccessRequestProgress.Completed)
                {
                    if (accessRequest != null)
                    {
                        this.ValidToPicker.MinDate = accessRequest != null && accessRequest.DeactivationDate != null ? (DateTime)accessRequest.DeactivationDate : DateTime.Today;
                        this.ValidToPicker.SelectedDate = accessRequest != null && accessRequest.DeactivationDate != null ? accessRequest.DeactivationDate : DateTime.Today;
                        AdHocActionType actionType = accessRequest.AdHocAccessRequest.AdhocActionType;
                        if (accessRequest.ProgressStatus == AccessRequestProgress.Completed && accessRequest.AdHocAccessRequest != null && !accessRequest.AdHocAccessRequest.IsÉxpiryHandled && (actionType == AdHocActionType.NewAccount || actionType == AdHocActionType.AmendmentAdd) && accessRequest.DeactivationDate.HasValue)
                        {
                            loadExtendPopup = true;
                        }
                    }
                }

                if (loadExtendPopup)
                {
                    ExtendedAction.VisibleOnPageLoad = true;
                }
                else
                {
                    if (accessRequest != null)
                    {
                        AdHocActionType actionType = accessRequest.AdHocAccessRequest.AdhocActionType;
                        if (accessRequest.ProgressStatus != AccessRequestProgress.Completed)
                            ShowAlertMessage(AlertType.Warning, string.Format("Selected AccessRequest -{0} does not support extend request, as the request is not in completed status.", Convert.ToString(this.accessRequestId)));
                        else if (accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.IsÉxpiryHandled)
                            ShowAlertMessage(AlertType.Warning, string.Format("Selected AccessRequest -{0} does not support extend request, as the request already extended.", Convert.ToString(this.accessRequestId)));
                        else if (actionType != AdHocActionType.NewAccount && actionType != AdHocActionType.AmendmentAdd)
                            ShowAlertMessage(AlertType.Warning, string.Format("Selected AccessRequest -{0} does not support extend request, only New Account and AmendmentAdd request can be extended.", Convert.ToString(this.accessRequestId)));
                        else if (!accessRequest.DeactivationDate.HasValue)
                            ShowAlertMessage(AlertType.Warning, string.Format("Selected AccessRequest -{0} does not support extend request, as the request doesnt have any end date.", Convert.ToString(this.accessRequestId)));
                        else
                            ShowAlertMessage(AlertType.Warning, string.Format("Selected AccessRequest -{0} does not support extend request.", Convert.ToString(this.accessRequestId)));
                    }
                }
            }
        }

        protected void grdRequest_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName != "Sort" && e.CommandName != "Page" && e.CommandName != "ChangePageSize" && e.CommandName != "RebindGrid" && e.CommandArgument != "" && e.CommandName != "Filter")
            {
                GridDataItem item = e.Item as GridDataItem;

                string RequestSource = Convert.ToString(item.GetDataKeyValue("RequestSource"));

                if (e.CommandName == "Resubmit" && RequestSource == Constants.AccessRequest && Convert.ToString(item.GetDataKeyValue("AdHocAccessRequestId")) != "0")
                    Response.Redirect("~/NACR/CreateCart.aspx?AdhocRequestId=" + e.CommandArgument);
                if (e.CommandName == "Resubmit" && RequestSource == Constants.BulkRequest && Convert.ToString(item.GetDataKeyValue("AccessRequestId")) != "0")
                    Response.Redirect("~/NACR/Create/BulkRequest/BulkAdhocAccessRequest.aspx?BulkRequestId=" + Convert.ToString(item.GetDataKeyValue("AccessRequestId")));
                if (Convert.ToBoolean(e.CommandName) == false && Convert.ToString(item.GetDataKeyValue("AdHocAccessRequestId")) == "0" && Convert.ToString(item.GetDataKeyValue("RequestSource")) == Constants.BulkRequest)
                    Response.Redirect("~/Home/MyBulkRequestDetails.aspx?Id=" + Convert.ToInt32(item.GetDataKeyValue("AccessRequestId")));
                if (Convert.ToBoolean(e.CommandName) == true && Convert.ToString(item.GetDataKeyValue("RequestSource")) == Constants.AccessRequest)
                    Response.Redirect("~/Home/MyRequestDetails.aspx?Id=" + e.CommandArgument);
                else
                    Response.Redirect($"~/Request/Detail.aspx?contextId={e.CommandArgument}&TypeofRequest=");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RequestInfo.VisibleOnPageLoad = false;
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            ExtendedAction.VisibleOnPageLoad = false;
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            if (txtComments.Text == "" || ValidToPicker.SelectedDate == null)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Mandatory fields should be required");
            }
            else
            {
                if (CheckforAlreadyExtendedRequestActive())
                {
                    return;
                }
                else
                {
                    try
                    {
                        SaveExtendedRequest();
                        MasterPage.ShowAlert(AlertType.Success, string.Format("Access Request - {0} sent for approval to extend.", Convert.ToString(this.accessRequestId)));
                    }
                    catch (Exception ex)
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Error in update record.");
                    }
                }

                ExtendedAction.VisibleOnPageLoad = false;
                this.grdRequest.Rebind();
            }

        }
        #endregion

        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            this.grdRequest.Rebind();
        }

        private bool CheckforAlreadyExtendedRequestActive()
        {
            Dictionary<string, string> paramtr = new Dictionary<string, string>();
            paramtr.Add("extendedRequestId", Convert.ToString(this.accessRequestId));
            IList<Dto.AccessRequest> checkforRequests = DomainApiClient<Dto.AccessRequest>.GetByAction<IList<Dto.AccessRequest>>(DomainApiAction.Provisioning.AccessRequest.GetExtendedRequestById, paramtr);

            if (checkforRequests.Count > 0)
            {
                MasterPage.ShowAlert(AlertType.Warning, string.Format("Extended Request already in process. Reference Id - {0}", checkforRequests[0].Id.ToString()));
            }

            return checkforRequests.Count > 0 ? true : false;
        }

        private void SaveExtendedRequest()
        {
            Dto.AccessRequest request = new Dto.AccessRequest();
            request.Id = (long)this.accessRequestId;
            request.DeactivationDate = (DateTime)this.ValidToPicker.SelectedDate;
            request.Comments = Convert.ToString(this.txtComments.Text);
            request.CreatedBy = _m_session.User.DisplayName;
            request.CreatedDate = DateTime.Now;

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("endpointId", Convert.ToString(this.accessRequestId));
            DomainApiClient<Dto.AccessRequest>.GetByAction<System.Object>(DomainApiAction.Provisioning.AccessRequest.SaveExtendedRequest, parameters, request); ;

            SaveAttachment();
        }

        private void SaveAttachment()
        {
            Dictionary<string, string> paramtr = new Dictionary<string, string>();
            paramtr.Add("extendedRequestId", Convert.ToString(this.accessRequestId));
            IList<Dto.AccessRequest> checkforRequests = DomainApiClient<Dto.AccessRequest>.GetByAction<IList<Dto.AccessRequest>>(DomainApiAction.Provisioning.AccessRequest.GetExtendedRequestById, paramtr);
            if (checkforRequests.Count > 0)
            {
                Dto.Attachment attach = new Dto.Attachment();
                if (AsyncUploadAttach.UploadedFiles.Count > 0)
                {
                    Telerik.Web.UI.UploadedFile file = AsyncUploadAttach.UploadedFiles[0];
                    byte[] fileData = new byte[file.ContentLength];
                    file.InputStream.Read(fileData, 0, Convert.ToInt32(file.ContentLength));
                    attach.FileName = file.FileName;
                    attach.AttachmentFile = fileData;
                    attach.ContentType = file.ContentType;
                }
                attach.AccessRequestId = accessRequestId;
                attach.RequestTypeId = (long)RequestType.Provisioning;
                attach.TaskId = null;
                attach.AccessRequestId = checkforRequests[0].Id;
                DomainApiClient<Dto.Attachment>.Save(attach);
            }
        }

        protected void chkEmpEvent_CheckedChanged(object sender, EventArgs e)
        {
            SetDataSource();
            grdRequest.DataBind();
        }

        protected void lnkTaskId_Command(object sender, CommandEventArgs e)
        {
            GridDataItem g = grdRequest.Items[Convert.ToInt32(e.CommandArgument)];
            if (g == null)
                return;

            this.ContextObjectId = (string)g.GetDataKeyValue("RequestSource") == Constants.BulkRequest ? (long)g.GetDataKeyValue("BulkRequestId") : (long)g.GetDataKeyValue("AdHocAccessRequestId");//==0 && (long)g.GetDataKeyValue(";
            this.TypeofRequest = (string)g.GetDataKeyValue("RequestSource");
            this.accessRequestId = (long)g.GetDataKeyValue("AccessRequestId");
            this.ItemProgressStatus = (AccessRequestProgress)g.GetDataKeyValue("ProgressStatus");
            Response.Redirect($"Detail.aspx?contextId={this.ContextObjectId}&TypeOfRequest={this.TypeofRequest}&&TaskId=0");
        }

        protected override IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request ID", "requestid", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Progress Status", "progressstatus", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Application Name", "applicationname", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request Source", "requestsource", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Account Type", "accounttype", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request Type", "requesttype", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request For", "requestfor", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request By", "requestby", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Entitlement", "entitlementstring", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Request Date", "requestdate", "dd-MM-yyyy"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Start Date", "startdate", "dd-MM-yyyy"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("End Date", "enddate", "dd-MM-yyyy"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Execution Time", "executiontime", "dd-MM-yyyy hh:mm tt"));
            return param;
        }
    }
}