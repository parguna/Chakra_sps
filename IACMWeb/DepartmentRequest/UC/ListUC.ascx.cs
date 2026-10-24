using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Web.UI;

namespace DI.Chakra.Web.DepartmentRequest.UC
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
            _sessionStatus.RedirectFrom = "MyDepartmentRequest";

            if (!IsPostBack)
            {

                ValidToPicker.SelectedDate = DateTime.Today;
            }
        }

        protected void grdRequest_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            Dictionary<string, string> paramtr = new Dictionary<string, string>();
            paramtr.Add("actionUserId", _m_session.User.Id.ToString());
            requestsList =
                DomainApiClient<Dto.Presentation.PresentationMyRequestData>.
                GetByAction<IList<Dto.Presentation.PresentationMyRequestData>>
                (DomainApiAction.Attestation.Presentation.PresentationMyRequest.GetByActionUserDepartment, paramtr);

            RequestsData = requestsList;
            if (RequestsData != null)
            {
                grdRequest.DataSource = RequestsData.OrderByDescending(l => l.AccessRequestID).ToList();
            }
        }

        protected void grdRequest_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            GridDataItem g = grdRequest.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("AdHocAccessRequestId");
            this.accessRequestId = (long)g.GetDataKeyValue("AccessRequestId");
            this.ItemProgressStatus = (AccessRequestProgress)g.GetDataKeyValue("ProgressStatus");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdRequest.ExportSettings.FileName = "MyRequests";
                grdRequest.Export(grdRequest, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdRequest.Export<IList<Dto.Presentation.PresentationMyRequestData>>(requestsList, e.ActionType, "MyRequests");
            }
            else if (e.ActionType == ACLAction.RequestExtend)
            {

                bool loadExtendPopup = false;
                Dto.AccessRequest accessRequest = null;
                if (this.ItemProgressStatus == AccessRequestProgress.Completed)
                {
                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters.Add("accessrequestId", Convert.ToString(this.accessRequestId));
                    accessRequest = DomainApiClient<Dto.AccessRequest>.GetByAction<Dto.AccessRequest>(DomainApiAction.Provisioning.AccessRequest.GetAccessRequestById, parameters);

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

        protected void grdRequest_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "ShowInfo")
            {
                GridDataItem item = e.Item as GridDataItem;
                // pass the access request id only. Retrieving ad hoc access request id from the access request if applicable.
                InfoUC.AccessRequestId = (long)item.GetDataKeyValue("AccessRequestId");
                RequestInfo.VisibleOnPageLoad = true;
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
    }
}