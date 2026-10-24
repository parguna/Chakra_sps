using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Request.BulkRequest.UC
{
    public partial class BulkDetail : UIBaseUC
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && CanLoad)
            {
                InitialPageLoad();
                MasterPage.PreferredPageTitle = "Bulk Request Detail";
            }
        }
        private void InitialPageLoad()
        {
            InitializeForm();
            if (Session["RedirectFrom"] != null && Session["RedirectFrom"].ToString() == "RequestDetails")
            {
                tdback.Visible = true;
                tdbtn.Visible = false;
            }
            SetApproverForm();
        }
        public bool CanLoad
        {
            get
            {
                if (ViewState["BulkDetail_CanLoad"] == null)
                    return true;
                return (bool)ViewState["BulkDetail_CanLoad"];

            }
            set
            { ViewState["BulkDetail_CanLoad"] = value; }
        }

        protected void grdApprover_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
            byte state = (byte)dataItem.GetDataKeyValue("ApprovalState");

            var approver = this.TaskApprovers.FirstOrDefault(item => item.ApprovalState == state);

            e.DetailTableView.DataSource = approver.ApproverDetails;
        }

        public Dto.Task Task
        {
            get
            {
                if (ViewState["Task"] != null)
                {
                    return (Dto.Task)ViewState["Task"];
                }
                return null;
            }
            set
            {
                ViewState["Task"] = value;
            }
        }

        public List<Dto.Presentation.PresentationApprovalChain> TaskApprovers
        {
            get
            {
                if (ViewState["TaskApprovers"] != null)
                {
                    return (List<Dto.Presentation.PresentationApprovalChain>)ViewState["TaskApprovers"];
                }
                else
                {
                    return new List<Dto.Presentation.PresentationApprovalChain>();
                }
            }
            set
            {
                ViewState["TaskApprovers"] = value;
            }
        }

        private void SetApproverForm()
        {

            var approvers = this.TaskApprovers;

            if (approvers.Count == 0)
            {
                approvers = this.GetApprovers();
                this.TaskApprovers = approvers;
            }

            grdApproverList.DataSource = approvers.ToList();
            grdApproverList.DataBind();

            //if (approvers.Any(a => a.IsActionOnlyRequired))
            //{
            //    divExecutionerGrid.Visible = true;
            //    divExecutionerHeader.Visible = true;
            //    grdExecutioner.DataSource = approvers.Where(app => app.IsActionOnlyRequired).Select(app => app).ToList();
            //    grdExecutioner.DataBind();
            //}
            //else
            //{
            //    divExecutionerGrid.Visible = false;
            //    divExecutionerHeader.Visible = false;
            //}
        }

        List<Dto.Presentation.PresentationApprovalChain> GetApprovers()
        {

            if (Task != null && Task.AccessRequestId.HasValue)
            {
                if (!Task.AccessRequest.BulkRequestId.HasValue)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("accessRequestId", Task.AccessRequestId.ToString());
                    return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByRequestId, parameter);

                }
                else
                {
                    if (Task.AccessRequest.AdHocAccessRequest.ApplicationType == ApplicationType.InhouseSystems)
                    {
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("accessRequestId", Task.AccessRequestId.ToString());
                        return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByRequestId, parameter);
                    }
                    else
                    {
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("bulkRequestId", Task.AccessRequest.BulkRequestId.ToString());
                        return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByBulkRequestId, parameter);
                    }
                }
            }
            else if (Task != null && Task.BulkRequestId != 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("bulkRequestId", Task.BulkRequestId.ToString());
                return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByBulkRequestId, parameter);
            }

            else
            {
                int requestId = 0;
                if (Request.QueryString["Id"] != null)
                {
                    requestId = Convert.ToInt32(Request.QueryString["Id"]);
                }
                if (requestId > 0)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("bulkRequestId", requestId.ToString());
                    return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByBulkRequestId, parameter);
                }
            }

            return new List<Dto.Presentation.PresentationApprovalChain>();
        }

        private void InitializeForm()
        {
            int requestId = 0;
            Dto.BulkRequest request = null;
            if (Request.QueryString["Id"] != null)
            {
                requestId = Convert.ToInt32(Request.QueryString["Id"]);
            }
            if (requestId > 0)
            {
                Task = DomainApiClient<Dto.Task>.GetByAction<Dto.Task>(DomainApiAction.Tasks.Task.GetTaskByBulkRequestId,
                                       new Dictionary<string, string> { { "bulkRequestId", requestId.ToString() } });

                if (Task != null && Task.BulkRequest != null)
                    request = Task.BulkRequest;
                else
                    request = DomainApiClient<Dto.BulkRequest>.GetById(requestId, true);

                withdrawtd.Visible = ((Request.QueryString["prev"] != null) || request.ProgressStatus == AccessRequestProgress.Completed
                                                                                        || request.ProgressStatus == AccessRequestProgress.ExecutionFailed
                                                                                        || request.ProgressStatus == AccessRequestProgress.Executing
                                                                                        || request.ProgressStatus == AccessRequestProgress.Withdrawn
                                                                                        || request.ProgressStatus == AccessRequestProgress.RequestRejected
                                                                                        || ((request.ProgressStatus == AccessRequestProgress.ReadytoExecute || request.ProgressStatus == AccessRequestProgress.WaitingForSync) && request.ExecutionTime.HasValue && request.ExecutionTime.Value.Date <= DateTime.Now.Date)) ? false : true;
                if (!withdrawtd.Visible)
                {
                    tdbtn.Attributes.Add("align", "right");
                    tdback.Attributes.Add("align", "right");
                }
                string hstName = string.Empty;

                if (request.EndpointSystemId > 0)
                {
                    hstName = request.EndPoint.DisplayName;
                }
                else
                {
                    hstName = "";
                }

                string requestName = hstName + " - " + UtilityMethods.GetEnumAttribute<Description>(request.AdhocActionType);
                lblRequestFor.Text = request.IsEmployee ? "Employee" : "Non-Employee";
                lblRequestName.Text = requestName;
                //lblRequestType.Text = UtilityMethods.GetEnumAttribute<Description>(request.AdhocActionType);
                lblRequestedDate.Text = request.CreatedDate.ToString();
                if (request.ApplicationType == (ApplicationType.SAP_EP) || request.ApplicationType == (ApplicationType.CE))// || request.ApplicationType == (ApplicationType.InternetPortal))
                {
                    //trentitle.Visible = true;
                    AdhocSAPEPRequest adHocSAPEPRequest = (AdhocSAPEPRequest)request.GetEntitlementDetail();
                    if (adHocSAPEPRequest != null)
                    {
                        if (!string.IsNullOrEmpty(adHocSAPEPRequest.Groups))
                            lblgroupinfo.Text = adHocSAPEPRequest.Groups;
                        if (!string.IsNullOrEmpty(adHocSAPEPRequest.Roles))
                            lbroleinfo.Text = adHocSAPEPRequest.Roles;
                    }
                }
                string status = UtilityMethods.GetEnumAttribute<Description>(request.ProgressStatus);
                string approver = string.Empty;

                if (Task != null && Task.ActionUser != null)
                {
                    User actionUser = Task.ActionUser;
                    //approver = string.Format(" ({0})", (task.IsGroupTask) ? Convert.ToString(task.GroupName) : actionUser.DisplayName + " - " + task.Approver);
                    approver = string.Format(" ({0})", Task.Approver);
                    if (!Task.IsGroupTask)
                    {
                        approver = string.Format(" ({0})", actionUser.DisplayName);
                    }
                    //var accessRequestDetail = task.AccessRequest;
                    //if (accessRequestDetail != null && accessRequestDetails.AccessRequest != null)
                    //{
                    //    BulkGenericRequestDetails.ConflictRemark = accessRequestDetails.AccessRequest.AdditionalInfo;
                    //}
                }
                if (request.RequestSource == AccessRequestSource.AdhocRequest)
                    lblRequestBy.Text = DomainApiClient<Dto.User>.GetById(request.RequestedBy).DisplayName;
                else
                    lblRequestBy.Text = "System";

                if (request.ProgressStatus == AccessRequestProgress.WaitingforApproval)
                {

                    bool hasPendingChildTasks = DomainApiClient<Dto.AccessRequest>.GetByAction<bool>(DomainApiAction.Provisioning.AccessRequest.IsBulkRequestHasPendingChildRequests, new Dictionary<string, string> { { "bulkRequestId", requestId.ToString() } });
                    if (hasPendingChildTasks)
                    {
                        var group = DomainApiClient<Dto.Group>.GetById(DBConfigHelper.MasterConfigHelper.ITSecurityGroupId);
                        lblStatus.Text = string.Format("Waiting For Approval ({0}) (Child Request)", group.GroupName);
                    }
                    else
                    {
                        lblStatus.Text = status + approver;
                    }
                }
                else
                {
                    lblStatus.Text = status;
                }

                BulkGenericRequestDetails.BulkRequestID = request.Id;
                BulkGenericRequestDetails.EntitlementDetailType = request.ApplicationType;



                if (request.ApplicationType == ApplicationType.HRMS_FLINS && request.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    AdHocSAPHRFLINSRequest flinsRequest = (AdHocSAPHRFLINSRequest)request.GetEntitlementDetail();
                    if (flinsRequest != null)
                    {
                        Dictionary<string, string> ListofEntilementWithCurrent = new Dictionary<string, string>();
                        foreach (AdHocSAPHRFLINSRequestDetail detail in flinsRequest.AdHocSAPHRFLINSRequestDetails)
                        {
                            detail.Entitlement.Split(',').ToList().ForEach(lq =>
                            {
                                if (!ListofEntilementWithCurrent.ContainsKey(lq))
                                {
                                    ListofEntilementWithCurrent.Add(lq, "Current Queue");
                                }
                            });

                        }
                        //string Remark = "";

                        //request.BulkRequestDetails.ForEach(lq =>
                        //{
                        //        //var EndpointUser = sapFacade.GetPrimarySAPUserByDIUserId(lq.UserId, request.EndpointSystemId).FirstOrDefault();
                        //        //if (EndpointUser != null)
                        //        //{
                        //        //    var conflicts = BulkGenericRequestDetails.ConflictBusinessRoles(ListofEntilementWithCurrent, EndpointUser.UserId, lq.UserId, lq.Name);
                        //        //    Remark = string.IsNullOrEmpty(Remark) ? conflicts : Remark + "<br>" + conflicts;
                        //        //}
                        //    });
                        //BulkGenericRequestDetails.ConflictRemark = Remark;
                        //accessrequest.AdditionalInfo = Message + ".";
                    }
                }
            }
            else
                WithDrawnButton.Visible = false;
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            BulkWithDrawnModalPopupExtender.VisibleOnPageLoad = false;

            if (Session["RedirectFrom"] != null && Session["RedirectFrom"].ToString() == "AdminRequest")
            {
                int progressStatus = !string.IsNullOrWhiteSpace(Request.QueryString["progressStatus"]) ? int.Parse(Request.QueryString["progressStatus"]) : 0;
                string requestFor = Request.QueryString["requestFor"];
                string requestForName = Request.QueryString["requestForName"];
                string endpointsystemId = Request.QueryString["endpointsystemId"];
                string fromDate = Request.QueryString["fromDate"];
                string toDate = Request.QueryString["toDate"];

                Response.Redirect($"~//Prov//Request//List.aspx?progressStatus={progressStatus}&requestFor={requestFor}&fromDate={fromDate}&toDate={toDate}&requestForName={requestForName}&endpointsystemId={endpointsystemId}");
            }
            else
                Response.Redirect("~//Request//List.aspx");
        }
        protected void btnback_Click(object sender, EventArgs e)
        {
            Response.Redirect("~//Request//List.aspx");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid)
                return;

            int requestId = 0;
            if (Request.QueryString["Id"] != null)
            {
                requestId = Convert.ToInt32(Request.QueryString["Id"]);
            }

            Dto.BulkRequest request = null;

            if (Task != null && Task.BulkRequest != null)
                request = Task.BulkRequest;
            else
                request = DomainApiClient<Dto.BulkRequest>.GetById(requestId, true);

            if (Task != null)
            {
                request.RequestStatus = AccessRequestStatus.Withdrawn;
                request.ProgressStatus = AccessRequestProgress.Withdrawn;
                request.Comments = ReasonField.Text.Trim();
                ReasonField.Text = "";
                request.LastModifiedBy = "System";
                request.LastModifiedDate = DateTime.Now;

                DomainApiClient<Dto.BulkRequest>.Save(request);

                var _criteria = new Dto.Presentation.WithdrawActionCriteria()
                {
                    RequestType = RequestType.Provisioning,
                    WorkflowInstanceId = Task.WorkflowInstanceId
                };

                bool status = DomainApiClient<Object>.GetByAction<bool>("WorkflowController", DomainApiAction.WorkflowProcessor.WithDrawnRequest, null, _criteria);

            }
            else
            {
                request.RequestStatus = AccessRequestStatus.Withdrawn;
                request.ProgressStatus = AccessRequestProgress.Withdrawn;
                request.Comments = ReasonField.Text.Trim();
                ReasonField.Text = "";
                request.LastModifiedBy = "System";
                request.LastModifiedDate = DateTime.Now;

                DomainApiClient<Dto.BulkRequest>.Save(request);
            }

            BackButton_Click(sender, e);
        }

        protected void WithDrawnButton_Click(object sender, EventArgs e)
        {
            ShowWindow();
        }

        protected void cancelpopup_Click(object sender, EventArgs e)
        {
            CloseWindow();
        }

        void ShowWindow()
        {
            var key = "showWindoww";
            string script = "Sys.Application.remove_load(showWindowWithdraw);Sys.Application.add_load(showWindowWithdraw);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        void CloseWindow()
        {
            var key = "closeWindoww";
            string script = "Sys.Application.remove_load(closeWindowWithdraw);Sys.Application.add_load(closeWindowWithdraw);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }
        protected void lnkGroupInfo_Command(object sender, CommandEventArgs e)
        {
            try
            {
                if (e.CommandArgument == null)
                    return;
                var lnkbtn = sender as LinkButton;
                var row = lnkbtn.NamingContainer as GridDataItem;
                var isGroupApproval = Convert.ToBoolean(row.GetDataKeyValue("IsGroupApproval").ToString());
                if (isGroupApproval)
                {
                    //var res = e.CommandArgument.ToString().Split('|');
                    //var isGroup = Convert.ToBoolean(res[0]);
                    //if (!isGroup || res.Length <= 1)
                    //    return;
                    var groupName = row.GetDataKeyValue("ApproverName").ToString();
                    //var groupName = res[1].Split('-')[1].Trim();
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("searchString", string.Empty);
                    parameter.Add("groupName", groupName);

                    Dictionary<long, string> users = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>
                        (DomainApiAction.User.GetUsersByGroup, parameter);
                    if (users != null && users.Count > 0)
                    {
                        grdUsers.DataSource = users;
                        grdUsers.DataBind();
                        ShowWindowUsers();
                    }
                    else
                    {
                        ShowAlertMessage(AlertType.Error, "Group:{groupName} No members found");
                    }

                }
                //var res = e.CommandArgument.ToString().Split('|');
                //var isGroup = Convert.ToBoolean(res[0]);
                //if (!isGroup || res.Length <= 1)
                //    return;

                //var groupName = res[1].Split('-')[1].Trim();
                //Dictionary<string, string> parameter = new Dictionary<string, string>();
                //parameter.Add("searchString", string.Empty);
                //parameter.Add("groupName", groupName);

                //Dictionary<long, string> users = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>
                //    (DomainApiAction.User.GetUsersByGroup, parameter);
                //grdUsers.DataSource = users;
                //grdUsers.DataBind();
                //ShowWindowUsers();
            }
            catch (Exception)
            {
            }
        }
        void ShowWindowUsers()
        {
            var key = "ShowWindowUsers";
            string script = "Sys.Application.remove_load(showWindowUsers);Sys.Application.add_load(showWindowUsers);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        void CloseWindowUsers()
        {
            var key = "CloseWindowUsers";
            string script = "Sys.Application.remove_load(closeWindowUsers);Sys.Application.add_load(closeWindowUsers);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }
        protected void btnCloseUserInfo_Click(object sender, EventArgs e)
        {
            CloseWindowUsers();
        }

        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            InitialPageLoad();
            var approvers = this.GetApprovers();
            this.TaskApprovers = approvers;

            grdApproverList.DataSource = approvers.ToList();
            grdApproverList.DataBind();
        }

        protected void grdApproverList_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                foreach (GridDataItem dataitem in grdApproverList.MasterTableView.Items)
                {
                    if (dataitem != null)
                    {
                        var status = (StatusType)dataitem.GetDataKeyValue("Action");
                        if (status == StatusType.Approved || status == StatusType.AutoCompleted || status == StatusType.Rejected || status == StatusType.Pending || status == StatusType.Withdrawn)
                        {
                            dataitem.Expanded = true;
                        }
                    }
                }
            }
        }

        protected void grdApproverList_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == RadGrid.ExpandCollapseCommandName)
            {
                foreach (GridItem item in e.Item.OwnerTableView.Items)
                {
                    if (item.Expanded && item != e.Item)
                    {
                        item.Expanded = false;
                    }
                }
            }
        }
    }
}

