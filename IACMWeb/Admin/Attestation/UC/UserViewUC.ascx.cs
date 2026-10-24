using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Web;
using System.Globalization;
using AjaxControlToolkit;
using DI.Chakra.DBConfigHelper;

namespace DI.Chakra.Web.Admin.Attestation.UC
{
    public partial class UserViewUC : UIBaseUC
    {
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
        bool isActionable = false;
        bool isActionExtendable = false;
        bool isNextLevelAllowed = true;

        bool showReserved1 = false; bool showReserved2 = false; bool showReserved3 = false; bool showReserved4 = false; bool showReserved5 = false;
        bool showReserved6 = false; bool showReserved7 = false; bool showReserved8 = false; bool showReserved9 = false; bool showReserved10 = false;
        bool showActionReserved1 = false; bool showActionReserved2 = false; bool showActionReserved3 = false;
        public event EventHandler SaveButtonClick;
        public event EventHandler SaveAndSubmitButtonClick;
        protected void OnSaveAndSubmitButtonClick()
        {
            if (SaveAndSubmitButtonClick != null)
            {
                SaveAndSubmitButtonClick(this, new EventArgs());
            }
        }
        protected void OnSaveButtonClick()
        {
            if (SaveButtonClick != null)
            {
                SaveButtonClick(this, new EventArgs());
            }
        }
        private long TaskId
        {
            get
            {
                if (ViewState["TaskId"] != null)
                {
                    return Convert.ToInt64(ViewState["TaskId"]);
                }
                return 0;
            }
            set
            {
                ViewState["TaskId"] = value;
            }
        }
        private long Level
        {
            get
            {
                if (ViewState["Level"] != null)
                {
                    return Convert.ToInt64(ViewState["Level"]);
                }
                return 0;
            }
            set
            {
                ViewState["Level"] = value;
            }
        }
        public AttestationType CurrentAttestationType
        {
            get
            {
                if (ViewState["CurrentAttestationType"] != null)
                {
                    return (AttestationType)ViewState["CurrentAttestationType"];
                }
                return AttestationType.User;
            }
            set
            {
                ViewState["CurrentAttestationType"] = value;
            }
        }
        private long campaignId
        {
            get
            {
                if (ViewState["campaignId"] != null)
                {
                    return Convert.ToInt64(ViewState["campaignId"]);
                }
                return 0;
            }
            set
            {
                ViewState["campaignId"] = value;
            }
        }

        private Dto.Task Task
        {
            get
            {
                if (ViewState["Task"] != null)
                {
                    return (Dto.Task)ViewState["Task"];
                }
                return getCurrentTask(TaskId);
            }
            set
            {
                ViewState["Task"] = value;
                campaignId = value.AttestationCampaignId.HasValue ? value.AttestationCampaignId.Value : 0;
            }
        }

        private bool InitialLoad
        {
            get
            {
                if (ViewState["InitialLoad"] != null)
                {
                    return Convert.ToBoolean(ViewState["InitialLoad"]);
                }
                return true;
            }
            set
            {
                ViewState["InitialLoad"] = value;
            }
        }

        private bool IsSaved
        {
            get
            {
                if (ViewState["IsSaved"] != null)
                {
                    return Convert.ToBoolean(ViewState["IsSaved"]);
                }
                return false;
            }
            set
            {
                ViewState["IsSaved"] = value;
            }
        }

        private bool IsEditable
        {
            get
            {
                if (ViewState["IsEditable"] != null)
                {
                    return Convert.ToBoolean(ViewState["IsEditable"]);
                }
                return false;
            }
            set
            {
                ViewState["IsEditable"] = value;
            }
        }

        public bool IsUnSavedChangesAvailable
        {
            get
            {
                if (ViewState["IsUnSavedChangesAvailable"] != null)
                {
                    return Convert.ToBoolean(ViewState["IsUnSavedChangesAvailable"]);
                }
                return false;
            }
            set
            {
                ViewState["IsUnSavedChangesAvailable"] = value;
            }
        }
        private bool IsEBSORMSSL
        {
            get
            {
                if (ViewState["IsEBSORMSSL"] != null)
                {
                    return Convert.ToBoolean(ViewState["IsEBSORMSSL"]);
                }
                return true;
            }
            set
            {
                ViewState["IsEBSORMSSL"] = value;
            }
        }
        private Dictionary<string,Tuple<int,int>> PageIndex
        {
            get
            {
                if (ViewState["PageIndex"] != null)
                {
                    return ViewState["PageIndex"] as Dictionary<string, Tuple<int, int>>;
                }
                return default(Dictionary<string, Tuple<int, int>>);
            }
            set
            {
                ViewState["PageIndex"] = value;
            }
        }
        /// <summary>
        /// page load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                ViewState["PageIndexClientId"] = null;
                var msg = MasterConfigHelper.AttestationServiceAccountWarningMessage;
                if(!string.IsNullOrWhiteSpace(msg))
                lblConfirm.Text= msg;
                if (Request.QueryString["Id"] != null)
                {
                    this.TaskId = Convert.ToInt64(Request.QueryString["Id"]);
                    this.AddCommentsUserControl.TaskId = this.TaskId;
                }
            }
            this.AddCommentsUserControl.OnCommentsSave += CommentsSave_Click;
            this.AddCommentsUserControl.OnCommentsCancel += CommentsCancel_Click;
        }
        
        protected void AttestationReviewGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "ShowWarningInfo")
            {
                GridDataItem item = e.Item as GridDataItem;
                var userId = item["KeyId"].Text;
                UserSODConflict.UserId = Convert.ToInt64(userId);
                UserSODConflict.BindConflicts();
                SensitiveRoleDetails.VisibleOnPageLoad = true;
                
            }
            else if (e.CommandName == "ShowReviewHistory")
            {
                GridDataItem item = e.Item as GridDataItem;
                var snapshotActionId = item.GetDataKeyValue("SnapshotActionId");
                AttestationReviewHistoryUC.AttestationActionId = Convert.ToInt64(snapshotActionId);
                AttestationReviewHistoryUC.TaskId = Convert.ToInt64(TaskId);
                AttestationReviewHistoryUC.LoadForm();
                ReviewHistory.VisibleOnPageLoad = true;
            }
            try
            {
                GridDataItem dataItem = e.Item as GridDataItem;
                if (dataItem != null)
                {

                    long level = (long)dataItem.GetDataKeyValue("Level");
                    if (level > 1)
                        e.Item.OwnerTableView.Columns[27].Visible = false;
                }
            }
            catch
            {
            }
            /*Uncomment below code if only one item is to be expanded at a time*/
            //if (e.CommandName == RadGrid.ExpandCollapseCommandName)
            //{
            //    foreach (GridItem item in e.Item.OwnerTableView.Items)
            //    {
            //        if (item.Expanded && item != e.Item)
            //        {
            //            item.Expanded = false;
            //        }
            //    }
            //}
        }

        protected void AttestationReviewGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (!e.IsFromDetailTable)
            {
                DI.Chakra.Dto.Task task;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("taskId", this.TaskId.ToString());
                parameter.Add("currentUser", sessionStatus.User.UserName);
                IsUnSavedChangesAvailable = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
                        (DomainApiAction.Tasks.Task.GetTaskHasUnsavedChanges, parameter);
                parameter.Clear();

                parameter.Add("taskId", this.TaskId.ToString());
                CurrentAttestationType = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<AttestationType>
                        (DomainApiAction.Tasks.Task.GetAttestationTypeForTask, parameter);
                parameter.Clear();
                parameter.Add("taskId", this.TaskId.ToString());

                if (e.RebindReason == GridRebindReason.InitialLoad)
                {
                    if(IsUnSavedChangesAvailable)
                        InitialLoad = false;
                    else
                    InitialLoad = true;
                }
                else
                {
                    InitialLoad = false;
                }
                
                parameter.Add("currentUser", sessionStatus.User.UserName);
                parameter.Add("level", "0");
                parameter.Add("isInitialLoad", InitialLoad.ToString());
                List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail> attestationUserDetails = new List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>();
                attestationUserDetails = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>>
                        (DomainApiAction.Tasks.Task.GetUserForAttestationTaskReview, parameter);
                task = getCurrentTask(this.TaskId);

                if (attestationUserDetails != null && attestationUserDetails.Count > 0)
                {

                    var firstRow = attestationUserDetails.FirstOrDefault(i => i.IsActionable);
                    if (firstRow == null)
                    {
                        firstRow = attestationUserDetails.First();
                    }
                    isActionable = firstRow.IsActionable;
                    isNextLevelAllowed = firstRow.IsNextLevelAvailable;
                    isActionExtendable = firstRow.IsActionExtendable;
                    
                    showReserved1 = attestationUserDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved1));
                    //showReserved2 = attestationUserDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved2));
                    //showReserved3 = attestationUserDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved3));
                    showReserved4 = attestationUserDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved4));
                    showReserved5 = attestationUserDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved5));
                    showReserved6 = attestationUserDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved6));
                    showReserved7 = attestationUserDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved7));
                    showReserved8 = attestationUserDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved8));
                    showReserved9 = attestationUserDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved9));
                    showReserved10 = attestationUserDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved10));

                    showActionReserved1 = attestationUserDetails.Any(a => !string.IsNullOrWhiteSpace(a.ActionReserved1));
                    showActionReserved2 = attestationUserDetails.Any(a => !string.IsNullOrWhiteSpace(a.ActionReserved2));
                    showActionReserved3 = attestationUserDetails.Any(a => !string.IsNullOrWhiteSpace(a.ActionReserved3));

                    AttestationReviewGrid.MasterTableView.ExpandCollapseColumn.Display = isNextLevelAllowed;
                    AttestationReviewGrid.MasterTableView.EnableHierarchyExpandAll = isNextLevelAllowed;

                    //just to hide the approve revoke and comments from top/master grid
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ApproveGridTemplateColumn").Visible = isActionable;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("RevokeGridTemplateColumn").Visible = isActionable;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ReviewCommentsBoundColumn").Visible = isActionable;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("EffectiveDateTemplateColumn").Visible = isActionExtendable;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("RequestId").Visible = false;

                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("LastLogon").Display = false;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ReviewHistory").Display = false;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("TemplateEditColumn").Display = false;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Delta").Display = false;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("AccountType").Display = false;

                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved1").Visible = showReserved1;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved2").Visible = showReserved2;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved3").Visible = showReserved3;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved4").Visible = showReserved4;
                    //SRM POrg
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved5").Visible = false;
                    //SRM PGrp
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved6").Visible = false;
                    
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved7").Visible = showReserved7;
                    //Do not show this field
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved8").Visible = false;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved9").Visible = showReserved9;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved10").Visible = showReserved10;

                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ActionReserved1").Visible = showActionReserved1;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ActionReserved2").Visible = showActionReserved2;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ActionReserved3").Visible = showActionReserved3;

                    //if (CurrentAttestationType == AttestationType.Privileged)
                    //{
                    //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ApproveGridTemplateColumn").Visible = false;
                    //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("RevokeGridTemplateColumn").Visible = false;

                    //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved1").HeaderText = "Operator Id";
                    //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved2").HeaderText = "Activity DateTime";
                    //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ActionReserved1").HeaderText = "Request Id";
                    //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("DisplayName").HeaderText = "Activity";
                    //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ActionReserved1").Visible = false;
                    //}
                    //else if (CurrentAttestationType == AttestationType.Role)
                    //{
                    //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Name").HeaderText = "Name";
                    //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("DisplayName").HeaderText = "Description";
                    //}

                    //FirstLevel Dont have any history
                    //if (Task.WorkflowStateId == 1)
                    //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ReviewHistory").Display = false;
                    //else
                    //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ReviewHistory").Display = true;
                }               

                this.AttestationReviewGrid.DataSource = attestationUserDetails;

                UnSavedReviews.Visible = IsUnSavedChangesAvailable;
            }
        }

        protected void AttestationReviewGrid_DetailTableDataBind(object sender, Telerik.Web.UI.GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
            var clientid = e?.DetailTableView?.ClientID;

            if (!string.IsNullOrWhiteSpace(clientid) && PageIndex != null && PageIndex.ContainsKey(clientid))
            {
                e.DetailTableView.CurrentPageIndex = PageIndex[e.DetailTableView.ClientID].Item1;
                e.DetailTableView.PageSize = PageIndex[e.DetailTableView.ClientID].Item2;
                PageIndex.Remove(clientid);
            }
            //grid functionality like allow paging, filtering, expand were handled here for detailed grid
            e.DetailTableView.IsFilterItemExpanded = false;
            e.DetailTableView.AllowPaging = false;
            e.DetailTableView.EnableHierarchyExpandAll = true;
            e.DetailTableView.AllowSorting = false;

            List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails = new List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>();
            long endpointSystemTypeId = (long)dataItem.GetDataKeyValue("EndpointSystemType");
            string entity = dataItem.GetDataKeyValue("Entity").ToString();
            string endpointSystemId = dataItem.GetDataKeyValue("EndpointSystemId").ToString();
            long level = (long)dataItem.GetDataKeyValue("Level");
            this.Level = level;
            long bindingLevel = (long)dataItem.GetDataKeyValue("BindingLevel");
            string category = dataItem.GetDataKeyValue("ReviewCategory").ToString();
            var accountName = dataItem.GetDataKeyValue("AccountName")?.ToString();
            Dictionary <string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", this.TaskId.ToString());
            parameter.Add("currentUser", sessionStatus.User.UserName);
            parameter.Add("level", level.ToString());
            parameter.Add("bindingLevel", bindingLevel.ToString());
            parameter.Add("entity", entity);
            parameter.Add("endpointSystemId", endpointSystemId);
            parameter.Add("accountName", accountName);
            parameter.Add("isInitialLoad", InitialLoad.ToString());

            gridNextLevelDetails = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>>
                            (DomainApiAction.Tasks.Task.GetDetailsForAttestationTaskReviewByUser, parameter);
            if (level == 0)
            {
                dataItem.CssClass = "employeenamehighlight";
            }
            if (level == 1)
            {
                dataItem.CssClass = "account-highlight";
            }
            bool hasDelta = false;
            if (gridNextLevelDetails != null && gridNextLevelDetails.Count > 0)
            {
                var firstRow = gridNextLevelDetails.FirstOrDefault(i=> i.IsActionable);
                if(firstRow == null)
                {
                    firstRow = gridNextLevelDetails.First();
                }
                isActionable = firstRow.IsActionable;
                isNextLevelAllowed = firstRow.IsNextLevelAvailable;
                isActionExtendable = firstRow.IsActionExtendable;
                //if(firstRow.Type == EntitlementType.Position.ToString())
                //{
                //    isNextLevelAllowed = false;
                //}
                //enable paging for the inside grid, only if the count greater than 10.
                e.DetailTableView.AllowPaging = gridNextLevelDetails.Count > 10;

                showReserved1 = gridNextLevelDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved1));
                showReserved2 = gridNextLevelDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved2));
                showReserved3 = true;// gridNextLevelDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved3));
                showReserved4 = gridNextLevelDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved4));
                showReserved5 = gridNextLevelDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved5));
                showReserved6 = gridNextLevelDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved6));
                showReserved7 = gridNextLevelDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved7));
                showReserved8 = gridNextLevelDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved8));
                showReserved9 = gridNextLevelDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved9));
                showReserved10 = gridNextLevelDetails.Any(a => !string.IsNullOrWhiteSpace(a.Reserved10));

                showActionReserved1 = gridNextLevelDetails.Any(a => !string.IsNullOrWhiteSpace(a.ActionReserved1));
                showActionReserved2 = gridNextLevelDetails.Any(a => !string.IsNullOrWhiteSpace(a.ActionReserved2));
                showActionReserved3 = gridNextLevelDetails.Any(a => !string.IsNullOrWhiteSpace(a.ActionReserved3));
                hasDelta = gridNextLevelDetails.Any(a => a.IsDelta);
            }
            
			//want to show risk info in details level then remove the below line
            e.DetailTableView.Columns.FindByUniqueName("TemplateEditColumn").Visible = false;
            e.DetailTableView.Columns.FindByUniqueName("LastLogon").Display = false;
            e.DetailTableView.Columns.FindByUniqueName("Delta").Display = hasDelta;
            e.DetailTableView.Columns.FindByUniqueName("AccountType").Display = false;
            e.DetailTableView.Columns.FindByUniqueName("Reserved3").HeaderText = "Valid To";
            if (level == 0)
            {
                e.DetailTableView.Columns.FindByUniqueName("Delta").Display = true;
                e.DetailTableView.Columns.FindByUniqueName("Name").HeaderText = "Account Name";
                e.DetailTableView.Columns.FindByUniqueName("DisplayName").HeaderText = "Application Name";
            }

            if (level == 1)
            {
                e.DetailTableView.Columns.FindByUniqueName("Delta").Display = false;
                e.DetailTableView.Columns.FindByUniqueName("Name").HeaderText = "Name";
                e.DetailTableView.Columns.FindByUniqueName("DisplayName").HeaderText = "Description";
                e.DetailTableView.Columns.FindByUniqueName("Type").HeaderText = "Entitlement Type";
                //e.DetailTableView.Columns.FindByUniqueName("Reserved3").Display = false;
            }
            if (level > 1)
            {
                e.DetailTableView.Columns.FindByUniqueName("Delta").Display = false;
                e.DetailTableView.Columns.FindByUniqueName("Name").HeaderText = "Name";
                e.DetailTableView.Columns.FindByUniqueName("DisplayName").HeaderText = "Description";
                e.DetailTableView.Columns.FindByUniqueName("Type").HeaderText = "Entitlement Type";
                e.DetailTableView.Columns.FindByUniqueName("Reserved3").Visible = false;
                e.DetailTableView.Columns.FindByUniqueName("Reserved3").Display = false;
            }
            //just to hide the approve revoke and comments based on the configuration            
            e.DetailTableView.Columns.FindByUniqueName("ApproveGridTemplateColumn").Visible = isActionable;
            e.DetailTableView.Columns.FindByUniqueName("RevokeGridTemplateColumn").Visible = isActionable;
            e.DetailTableView.Columns.FindByUniqueName("ReviewCommentsBoundColumn").Visible = isActionable;
            e.DetailTableView.Columns.FindByUniqueName("EffectiveDateTemplateColumn").Visible = isActionExtendable;
            e.DetailTableView.Columns.FindByUniqueName("ReviewHistory").Visible = isActionable;
            e.DetailTableView.ExpandCollapseColumn.Display = isNextLevelAllowed;
            e.DetailTableView.EnableHierarchyExpandAll = isNextLevelAllowed;
            e.DetailTableView.Columns.FindByUniqueName("RequestId").Visible = false;

            //if you dont want to enable all levels in single click, comment the below row
            e.DetailTableView.HierarchyDefaultExpanded = isNextLevelAllowed;

            //reserved column visible
            e.DetailTableView.Columns.FindByUniqueName("Reserved1").Visible = showReserved1;
            e.DetailTableView.Columns.FindByUniqueName("Reserved2").Visible = showReserved2;
            e.DetailTableView.Columns.FindByUniqueName("Reserved3").Visible = showReserved3;
            e.DetailTableView.Columns.FindByUniqueName("Reserved4").Visible = showReserved4;
            //SRM POrg
            e.DetailTableView.Columns.FindByUniqueName("Reserved5").Visible = false;
            //SRM PGrp
            e.DetailTableView.Columns.FindByUniqueName("Reserved6").Visible = false;

            e.DetailTableView.Columns.FindByUniqueName("Reserved7").Visible = showReserved7;
            //Dont show this field.This is mainly for EBS MSSL
            e.DetailTableView.Columns.FindByUniqueName("Reserved8").Visible = false;
            e.DetailTableView.Columns.FindByUniqueName("Reserved9").Visible = showReserved9;
            e.DetailTableView.Columns.FindByUniqueName("Reserved10").Visible = showReserved10;

            e.DetailTableView.Columns.FindByUniqueName("ActionReserved1").Visible = showActionReserved1;
            e.DetailTableView.Columns.FindByUniqueName("ActionReserved2").Visible = showActionReserved2;
            e.DetailTableView.Columns.FindByUniqueName("ActionReserved3").Visible = showActionReserved3;

            //FirstLevel Dont have any history
            if (Task.WorkflowStateId == 1)
            {
                e.DetailTableView.Columns.FindByUniqueName("ReviewHistory").Visible = false;
                e.DetailTableView.Columns.FindByUniqueName("ReviewHistory").Display = false;
            }
            else
            {
                e.DetailTableView.Columns.FindByUniqueName("ReviewHistory").Visible = true;
                e.DetailTableView.Columns.FindByUniqueName("ReviewHistory").Display = true;
            }

            //want to show risk info in details level then remove the below line
            e.DetailTableView.Columns.FindByUniqueName("TemplateEditColumn").Visible = false;
            e.DetailTableView.DataSource = gridNextLevelDetails;
            if(gridNextLevelDetails != null && gridNextLevelDetails.Count == 0)
            {
                e.DetailTableView.Visible = false;
            }
            else
            {
                e.DetailTableView.Visible = true;
            }
        }

        protected void AttestationReviewGrid_PreRender(object sender, EventArgs e)
        {
            //if (CurrentAttestationType == AttestationType.Privileged)
            //{
            //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved1").HeaderText = "Operator Id";
            //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved2").HeaderText = "Activity DateTime";
            //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ActionReserved1").HeaderText = "Request Id";
            //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("DisplayName").HeaderText = "Activity";
            //}
            //else if (CurrentAttestationType == AttestationType.Role)
            //{
            //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Name").HeaderText = "Name";
            //    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("DisplayName").HeaderText = "Description";
            //}
        }

        protected void AttestationReviewGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                bool isChanged = Convert.ToBoolean(e.Item.DataItem.GetType().GetProperty("IsChanged").GetValue(e.Item.DataItem, null));
                var level = Convert.ToInt64(e.Item.DataItem.GetType().GetProperty("Level").GetValue(e.Item.DataItem, null));
                if (level == 1)
                {
                    e.Item.Style.Add("font-weight", "bold");
                    var lblDisplyaname = (e.Item.FindControl("lblDisplayName") as Label);
                    if(lblDisplyaname != null)
                    {
                        lblDisplyaname.CssClass = "applicationName";
                    }
                }
                //if (isChanged)
                //{
                //    e.Item.BackColor = System.Drawing.Color.FromArgb(228, 236, 245);
                //}
                var eType = e.Item.DataItem.GetType().GetProperty("Type").GetValue(e.Item.DataItem, null)?.ToString();
                //if(eType == EntitlementType.Position.ToString())
                //{
                //    //(e.Item as GridDataItem).["ExpandColumn"].Enabled = false;
                //    (e.Item as GridDataItem)["ExpandColumn"].Controls[0].Visible = false;
                //}
                bool isEditable = true;
                var actionUserId = Task.ActionUserId.HasValue ? Task.ActionUserId.Value : 0;
                if (Task.IsGroupTask && !Task.IsDelegated && Task.AssignedTo.HasValue)
                    actionUserId = (long)Task.AssignedTo;
                if (actionUserId != sessionStatus.UserId || Task.IsCompleted || Task.Action == StatusType.Approved || Task.Action == StatusType.AutoCompleted)
                    isEditable = false;

                IsEditable = isEditable;

                var lblDisplayName = e.Item.FindControl("lblDisplayName") as Label;
                var hoverDisplayName = e.Item.FindControl("hoverDisplayName") as HoverMenuExtender;
                bool isMaxChar = false;
                if (level > 0 && lblDisplayName != null && !string.IsNullOrWhiteSpace(lblDisplayName.Text) && lblDisplayName.Text.Length > 160)
                {
                    isMaxChar = true;
                    lblDisplayName.Text = lblDisplayName.Text.Substring(0, 160) +"...";
                }
                if (hoverDisplayName != null)
                {
                    hoverDisplayName.Enabled = isMaxChar;
                }

                //if (sessionStatus.RedirectFrom != null && sessionStatus.RedirectFrom == "TaskDetails")
                {
                    ImageButton AddCommentLinkButton = (ImageButton)e.Item.FindControl("AddCommentLinkButton");
                    if (AddCommentLinkButton != null)
                        AddCommentLinkButton.Enabled = true; //if you want to disable the clicking of comments button add this flag isEditable
                    RadioButton ApproveRadioButton = (RadioButton)e.Item.FindControl("ApproveRadioButton");
                    if (ApproveRadioButton != null)
                        ApproveRadioButton.Enabled = isEditable;
                    RadioButton RevokeRadioButton = (RadioButton)e.Item.FindControl("RevokeRadioButton");
                    if (RevokeRadioButton != null)
                        RevokeRadioButton.Enabled = isEditable;
                    RadDatePicker EffectiveDateButton = (RadDatePicker)e.Item.FindControl("ValidFromPicker");
                    if (EffectiveDateButton != null)
                        EffectiveDateButton.Enabled = isEditable && RevokeRadioButton.Checked;
                }

            }
            
            //if (isExport)
            //{
            //    GridDataItem item = e.Item as GridDataItem;

            //    if (item != null)
            //    {
            //        item["ApproveGridTemplateColumn"].Text = "Y";
            //        item["RevokeGridTemplateColumn"].Text = "Y";
            //    }

            //    if (e.Item is GridCommandItem)
            //    {
            //        e.Item.Visible = false;
            //    }
            //}
        }



        protected void ApproveRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            //IsEBSORMSSL = false;
            RadioButton radioButton = (RadioButton)sender;
            var selectedRadioButtonRow = (((radioButton).Parent.Parent as GridItem) as GridEditableItem);
            long snapshotActionId = Convert.ToInt64(selectedRadioButtonRow.GetDataKeyValue("SnapshotActionId"));
            //var isSAP = (((int)DI.Chakra.Infrastructure.Enums.EndpointType.SAP).ToString()) == selectedRadioButtonRow.GetDataKeyValue("EndpointSystemType").ToString();
            //if (isSAP)
            //{
            //    var endpointSystemId = Convert.ToInt64(selectedRadioButtonRow.GetDataKeyValue("EndpointSystemId"));
            //    var ep = DomainApiClient<Dto.EndPoint>.GetById(endpointSystemId);
            //    if (ep != null && (ep.ApplicationType == ApplicationType.EBS_CRM || ep.ApplicationType == ApplicationType.EBS_ISU || ep.ApplicationType == ApplicationType.MSSL_ISU || ep.ApplicationType == ApplicationType.MSSL_CRM))
            //    {
            //        IsEBSORMSSL = true;
            //    }
            //}
            //try
            //{
            //    PageIndex = ((Telerik.Web.UI.GridTableView)(radioButton).BindingContainer.BindingContainer).CurrentPageIndex;
            //    ViewState["PageIndexClientId"] = ((Telerik.Web.UI.GridTableView)(radioButton).BindingContainer.BindingContainer).ClientID;
            //}
            //catch
            //{
            //}
            try
            {
                var index = ((GridTableView)(radioButton).BindingContainer.BindingContainer).CurrentPageIndex;
                var size = ((GridTableView)(radioButton).BindingContainer.BindingContainer).PageSize;
                var clientID = ((Telerik.Web.UI.GridTableView)(radioButton).BindingContainer.BindingContainer).ClientID;
                if (PageIndex == null)
                {
                    PageIndex = new Dictionary<string, Tuple<int, int>>();
                    PageIndex.Add(clientID, Tuple.Create<int, int>(index, size));
                }
                else
                {
                    if (PageIndex.ContainsKey(clientID))
                        PageIndex[clientID] = Tuple.Create<int, int>(index, size);
                    else
                        PageIndex.Add(clientID, Tuple.Create<int, int>(index, size));
                }

            }
            catch
            {
            }
            this.UpdateEntity(snapshotActionId, radioButton.Checked, AccessType.Approve);
        }

        protected void RevokeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            var selectedRadioButtonRow = (((radioButton).Parent.Parent as GridItem) as GridEditableItem);
            long snapshotActionId = Convert.ToInt64(selectedRadioButtonRow.GetDataKeyValue("SnapshotActionId"));
            long level = Convert.ToInt64(selectedRadioButtonRow.GetDataKeyValue("Level"));
            var accountType = ((AccountType)selectedRadioButtonRow.GetDataKeyValue("AccountType"));
            //IsEBSORMSSL = false;
            var isSAP = (((int)DI.Chakra.Infrastructure.Enums.EndpointType.SAP).ToString()) == selectedRadioButtonRow.GetDataKeyValue("EndpointSystemType").ToString();
            if(isSAP)
            {
                var endpointSystemId = Convert.ToInt64(selectedRadioButtonRow.GetDataKeyValue("EndpointSystemId"));
                var ep = DomainApiClient<Dto.EndPoint>.GetById(endpointSystemId);
                if(ep != null && (ep.ApplicationType == ApplicationType.EBS_CRM || ep.ApplicationType == ApplicationType.EBS_ISU || ep.ApplicationType == ApplicationType.MSSL_ISU || ep.ApplicationType == ApplicationType.MSSL_CRM))
                {
                    //IsEBSORMSSL = true;
                    var snpaShotAction = DomainApiClient<Dto.AttestationSnapshotAction>.GetById(snapshotActionId,true);
                    if(snpaShotAction != null)
                    {
                        //For Account Removal we should check
                        if(snpaShotAction.AttestationSnapshot.EntitlementType == EntitlementType.Account)
                        {

                        }else if (snpaShotAction.AttestationSnapshot.EntitlementType == EntitlementType.Position)
                        {
                            var accountName = selectedRadioButtonRow.GetDataKeyValue("AccountName").ToString();
                            var param = new Dictionary<string, string>();
                            param.Add("snapShotActionId", snapshotActionId.ToString());
                            param.Add("accountName", accountName);
                            param.Add("endpointSystemId", endpointSystemId.ToString());
                            param.Add("campaignId", this.campaignId.ToString());
                            var isNotAllowed = DomainApiClient<Dto.AttestationSnapshotAction>.GetByAction<bool>("CheckAllPositionSelected", param);
                            if (isNotAllowed)
                            {
                                radioButton.Checked = false;
                                (selectedRadioButtonRow.FindControl("ApproveRadioButton") as RadioButton).Checked = true;
                                MasterPage.ShowAlert(AlertType.Error, "You are not allowed to remove all the position");
                                return;
                            }
                        }
                    }
                }
            }
            try
            {
                var index = ((GridTableView)(radioButton).BindingContainer.BindingContainer).CurrentPageIndex;
                var size = ((GridTableView)(radioButton).BindingContainer.BindingContainer).PageSize;
                var clientID = ((Telerik.Web.UI.GridTableView)(radioButton).BindingContainer.BindingContainer).ClientID;
                if (PageIndex == null)
                {
                    PageIndex = new Dictionary<string, Tuple<int, int>>();
                    PageIndex.Add(clientID, Tuple.Create<int, int>(index, size));
                }
                else
                {
                    if (PageIndex.ContainsKey(clientID))
                        PageIndex[clientID] = Tuple.Create<int, int>(index, size);
                    else
                        PageIndex.Add(clientID, Tuple.Create<int, int>(index, size));
                }
            }
            catch
            {
            }
            if ((accountType == AccountType.Service || accountType == AccountType.Shared))
            {
                this.hdnRevokeItem.Value = snapshotActionId.ToString();
                ConfirmServiceAccount.VisibleOnPageLoad = true;
            }
            else
            {
                this.UpdateEntity(snapshotActionId, radioButton.Checked, AccessType.Revoke);
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            SessionStatusImpl _sessionStatus = new SessionStatusImpl();

            if (_sessionStatus.RedirectFrom == "Tasks")
            {
                string source = this.Request.QueryString["source"] ?? "";
                string taskStatus = this.Request.QueryString["ddltaskstatus"] ?? "";
                string showAllTasks = this.Request.QueryString["showalltask"] ?? "";
                string fromDate = this.Request.QueryString["fromDate"] ?? "";
                string toDate = this.Request.QueryString["toDate"] ?? "";
                string requestFor = this.Request.QueryString["requestFor"] ?? "";
                string requestForName = this.Request.QueryString["requestForName"] ?? "";
                string epId = this.Request.QueryString["endpointsystemId"] ?? "";

                Response.Redirect($"~//Prov//Task//List.aspx?ddltaskstatus={taskStatus}&showalltask={showAllTasks}&toDate={toDate}&fromDate={fromDate}&requestFor={requestFor}&requestForName={requestForName}&endpointsystemId={epId}");
                //Response.Redirect("~//Prov//Task//List.aspx");
            }
            else if (_sessionStatus.RedirectFrom == "Home")
            {
                Response.Redirect("~//Home//Home.aspx");
            }
            else if (_sessionStatus.RedirectFrom == "GroupTask")
                Response.Redirect("~//Task//GroupTasks.aspx");
            else if (_sessionStatus.RedirectFrom == "AttestationCampaigntaskDetail")
                Response.Redirect("~//Report//Attestation//AttestationCampaigntaskDetail.aspx?ContextId=" + campaignId.ToString());
            else if (_sessionStatus.RedirectFrom == "AttestationCampaignStatus")
                Response.Redirect("~/Report/Attestation/AttestationCampaignStatus.aspx?ContextId=" + campaignId.ToString());
            else if (_sessionStatus.RedirectFrom == "AttestationOverallStatus")
                Response.Redirect("~/Report/Attestation/AttestationOverallStatus.aspx");
            else if (_sessionStatus.RedirectFrom == "AttestationCampaignReport")
            {
                Response.Redirect("~/Report/Attestation/AttestationCampaignReport.aspx");
            }
            else
            {
                string source = this.Request.QueryString["source"] ?? "";
                string taskStatus = this.Request.QueryString["ddltaskstatus"] ?? "";
                string showAllTasks = this.Request.QueryString["showalltask"] ?? "";
                string fromDate = this.Request.QueryString["fromDate"] ?? "";
                string toDate = this.Request.QueryString["toDate"] ?? "";
                string requestFor = this.Request.QueryString["requestFor"] ?? "";
                string requestForName = this.Request.QueryString["requestForName"] ?? "";
                string epId = this.Request.QueryString["endpointsystemId"] ?? "";

                Response.Redirect($"~//Task//List.aspx?ddltaskstatus={taskStatus}&showalltask={showAllTasks}&toDate={toDate}&fromDate={fromDate}&requestFor={requestFor}&requestForName={requestForName}&endpointsystemId={epId}");
                //Response.Redirect("~//Task//List.aspx");
            }
                
        }

        protected void ExpandAllButton_Click(object sender, EventArgs e)
        {
            this.ExpandAll();
        }

        protected void CollapseAllButton_Click(object sender, EventArgs e)
        {
            this.CollapseAll();
        }
        protected void btnHistoryCancel_Click(object sender, EventArgs e)
        {
            ReviewHistory.VisibleOnPageLoad = false;
        }

        protected void AddCommentImageButton_Click(object sender, EventArgs e)
        {
            ImageButton commentLinkButton = (ImageButton)sender;
            var commentLinkButtonRow = (((commentLinkButton).Parent.Parent as GridItem) as GridEditableItem);

            try
            {
                var index = ((GridTableView)(commentLinkButton).BindingContainer.BindingContainer).CurrentPageIndex;
                var clientID = ((Telerik.Web.UI.GridTableView)(commentLinkButton).BindingContainer.BindingContainer).ClientID;
                var size = ((GridTableView)(commentLinkButton).BindingContainer.BindingContainer).PageSize;
                if (PageIndex == null)
                {
                    PageIndex = new Dictionary<string, Tuple<int, int>>();
                    PageIndex.Add(clientID, Tuple.Create<int, int>(index, size));
                }
                else
                {
                    if (PageIndex.ContainsKey(clientID))
                    {
                        PageIndex[clientID] = Tuple.Create<int, int>(index, size);
                    }
                    else
                    {
                        PageIndex.Add(clientID, Tuple.Create<int, int>(index, size));
                    }
                }
            }
            catch
            {
            }

            long snapshotActionId = Convert.ToInt64(commentLinkButtonRow.GetDataKeyValue("SnapshotActionId"));
            string category = commentLinkButtonRow.GetDataKeyValue("ReviewCategory").ToString();
            var comments = commentLinkButtonRow.GetDataKeyValue("Comments");
            string comment = comments == null ? "" : comments.ToString();
            this.AddCommentsUserControl.KeyId = snapshotActionId;
            this.AddCommentsUserControl.Category = category;
            this.AddCommentsUserControl.Comments = comment;
            this.AddCommentsUserControl.IsEditable = IsEditable;
            this.AddCommentsUserControl.ReviewComments = "";
            this.CommentsWindow.VisibleOnPageLoad = true;
            string script = "function f(){$find(\"" + this.CommentsWindow.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
        }

        private void CommentsSave_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("snapshotActionId", this.AddCommentsUserControl.KeyId.ToString());
            parameter.Add("comments", this.AddCommentsUserControl.Comments);
            parameter.Add("category", this.AddCommentsUserControl.Category);
            parameter.Add("taskId", this.TaskId.ToString());
            parameter.Add("currentUser", sessionStatus.User.UserName);
            var actionUserId = Task.ActionUserId.HasValue ? Task.ActionUserId.Value : 0;
            if (Task.IsGroupTask && !Task.IsDelegated && Task.AssignedTo.HasValue)
                actionUserId = (long)Task.AssignedTo;
            parameter.Add("taskActionUserId", actionUserId.ToString());
            var temp = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
                        (DomainApiAction.Tasks.Task.UpdateAttestationReviewComments, parameter);
            this.AttestationReviewGrid.RetainExpandStateOnRebind = true;
            this.AttestationReviewGrid.Rebind();
            this.CommentsWindow.VisibleOnPageLoad = false;
            this.IsUnSavedChangesAvailable = true;
            UnSavedReviews.Visible = true;
        }

        private void CommentsCancel_Click(object sender, EventArgs e)
        {
            this.CommentsWindow.VisibleOnPageLoad = false;
        }

        #region "Private Methods"

        Dto.Task getCurrentTask(long taskId)
        {
            Task = DomainApiClient<Dto.Task>.GetById(this.TaskId);
            return Task;
        }

        private void UpdateEntity(long snapshotActionId, bool value, AccessType accessType)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("snapshotActionId", snapshotActionId.ToString());
            parameter.Add("value", value.ToString());
            parameter.Add("accessType", accessType.ToString());
            parameter.Add("taskId", this.TaskId.ToString());
            parameter.Add("currentUser", sessionStatus.User.UserName);
            var actionUserId = Task.ActionUserId.HasValue ? Task.ActionUserId.Value : 0;
            if (Task.IsGroupTask && !Task.IsDelegated && Task.AssignedTo.HasValue)
                actionUserId = (long)Task.AssignedTo;
            parameter.Add("taskActionUserId", actionUserId.ToString());
            var temp = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
                        (DomainApiAction.Tasks.Task.UpdateAttestationReviewAction, parameter);
            this.AttestationReviewGrid.RetainExpandStateOnRebind = true;

            // disable Submit button again whenver a change is made.
            // This will enforce that the user has to click save before submitting to avoid losing any changes.
            this.IsSaved = false;
            this.IsUnSavedChangesAvailable = true;
            //if (IsEBSORMSSL)
            {
                this.AttestationReviewGrid.Rebind();
            }
            UnSavedReviews.Visible = true;
        }

        private void UpdateEntityWithReserveValue(long snapshotActionId, string value, AccessType accessType)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("snapshotActionId", snapshotActionId.ToString());
            parameter.Add("value", value);
            parameter.Add("accessType", accessType.ToString());
            parameter.Add("taskId", this.TaskId.ToString());
            parameter.Add("currentUser", sessionStatus.User.UserName);
            var actionUserId = Task.ActionUserId.HasValue ? Task.ActionUserId.Value : 0;
            if (Task.IsGroupTask && !Task.IsDelegated && Task.AssignedTo.HasValue)
                actionUserId = (long)Task.AssignedTo;
            parameter.Add("taskActionUserId", actionUserId.ToString());
            var temp = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
                        (DomainApiAction.Tasks.Task.UpdateAttestationReviewReservedColumn, parameter);
            this.AttestationReviewGrid.RetainExpandStateOnRebind = true;

            // disable Submit button again whenver a change is made.
            // This will enforce that the user has to click save before submitting to avoid losing any changes.
            this.IsSaved = false;
            this.IsUnSavedChangesAvailable = true;
            this.AttestationReviewGrid.Rebind();
            UnSavedReviews.Visible = true;
        }

        public void Rebind()
        {
            //this.AttestationReviewGrid.RetainExpandStateOnRebind = true;
            foreach (GridItem item in AttestationReviewGrid.Items)
            {
                if (item.Expanded)
                {
                    item.Expanded = false;
                }
            }
            
            this.IsUnSavedChangesAvailable = false;
            AttestationReviewGrid.DataSource = null;
            AttestationReviewGrid.Rebind();
        }

        private void ExpandAll()
        {
            foreach (GridDataItem dataItem in this.AttestationReviewGrid.MasterTableView.Items)
            {
                dataItem.Expanded = true;
                ExpandChildGrid(dataItem);
            }
            this.AttestationReviewGrid.Rebind();
        }

        void ExpandChildGrid(GridDataItem dataItem)
        {
            foreach (GridTableView childDataItem in dataItem.ChildItem.NestedTableViews)
            {
                foreach (GridDataItem childItem in childDataItem.Items)
                {
                    childItem.Expanded = true;
                    ExpandChildGrid(childItem);
                }
            }
        }

        private void CollapseAll()
        {
            foreach (GridDataItem dataItem in this.AttestationReviewGrid.MasterTableView.Items)
            {
                dataItem.Expanded = false;
            }
            this.AttestationReviewGrid.Rebind();
        }
        #endregion

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~//Provisioning//TaskDetailsByActionUser.aspx");
        }

        protected void btnSensitiveRoleCancel_Click(object sender, EventArgs e)
        {
            SensitiveRoleDetails.VisibleOnPageLoad = false;
        }


        protected void AttestationReviewGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridHeaderItem && e.Item.OwnerTableView != AttestationReviewGrid.MasterTableView)
            {
                GridHeaderItem header = (GridHeaderItem)e.Item;
                //header["Name"].Text = "Name";
                if(this.Level == 0)
                {
                    header["Name"].Text = "Account Name";
                }
                if(this.Level >= 1)
                {
                    header["Name"].Text = "Name";
                    header["DisplayName"].Text = "Description";
                }
                else
                {
                    header["DisplayName"].Text = "Application Name";
                }
                header["Type"].Text = "Entitlement Type";
            }
            else if(e.Item is GridHeaderItem && e.Item.OwnerTableView != null)
            {
                GridHeaderItem header = (GridHeaderItem)e.Item;
                if (this.Level == 0)
                {
                    header["Name"].Text = "User Name";
                }
            }
            else if (e.Item is GridDataItem && e.Item.OwnerTableView == AttestationReviewGrid.MasterTableView )
            {
                var dataItem = e.Item as GridDataItem;
                if (Level == 0)
                {
                    dataItem.CssClass = "employeenamehighlight";
                }
            }
        }

        protected void ApprovePanel_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void RevokePanel_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
        .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void txtRequestId_TextChanged(object sender, EventArgs e)
        {
            TextBox txtBox = (TextBox)sender;
            var selectedRadioButtonRow = (((txtBox).Parent.Parent as GridItem) as GridEditableItem);
            long snapshotActionId = Convert.ToInt64(selectedRadioButtonRow.GetDataKeyValue("SnapshotActionId"));
            this.UpdateEntityWithReserveValue(snapshotActionId, txtBox.Text, AccessType.Hidden);
        }
        protected void ValidFromPicker_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
        {
            RadDatePicker datepickerButton = (RadDatePicker)sender;
            var datepickerButtonRow = (((datepickerButton).Parent.Parent as GridItem) as GridEditableItem);
            long snapshotActionId = Convert.ToInt64(datepickerButtonRow.GetDataKeyValue("SnapshotActionId"));
            string entity = Convert.ToString(datepickerButtonRow.GetDataKeyValue("Entity"));
            string endpointsystemId = Convert.ToString(datepickerButtonRow.GetDataKeyValue("EndpointSystemId"));

            //if (datepickerButtonRow.Parent != null && datepickerButtonRow.Parent is Telerik.Web.UI.GridTableView)
            //{
            //    CheckPagingForDetailView(datepickerButtonRow.Parent as Telerik.Web.UI.GridTableView, entity, this.TaskId, endpointsystemId);
            //}
            //else if (datepickerButtonRow.Parent != null && datepickerButtonRow.Parent.Parent != null && datepickerButtonRow.Parent.Parent is Telerik.Web.UI.GridTableView)
            //{
            //    CheckPagingForDetailView(datepickerButtonRow.Parent.Parent as Telerik.Web.UI.GridTableView, entity, this.TaskId, endpointsystemId);
            //}
            DateTime? efectivedate = e.NewDate.Value;

            if (efectivedate.HasValue)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("snapshotActionId", snapshotActionId.ToString());
                parameter.Add("effectivedate", efectivedate.Value.ToString("dd-MMM-yyyy"));
                parameter.Add("category", this.AddCommentsUserControl.Category);
                parameter.Add("taskId", this.TaskId.ToString());
                parameter.Add("currentUser", sessionStatus.User.UserName);
                var actionUserId = Task.ActionUserId.HasValue ? Task.ActionUserId.Value : 0;
                if (Task.IsGroupTask && !Task.IsDelegated && Task.AssignedTo.HasValue)
                    actionUserId = (long)Task.AssignedTo;
                parameter.Add("taskActionUserId", actionUserId.ToString());
                var temp = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<string>
                            (DomainApiAction.Tasks.Task.UpdateAttestationReviewEffectiveDate, parameter);
                if (!string.IsNullOrWhiteSpace(temp))
                {
                    MasterPage.ShowAlert(AlertType.Error, temp);
                }
                this.AttestationReviewGrid.RetainExpandStateOnRebind = true;
                this.AttestationReviewGrid.Rebind();
                this.CommentsWindow.VisibleOnPageLoad = false;
                this.IsUnSavedChangesAvailable = true;
                UnSavedReviews.Visible = true;
            }

        }

        protected void chkRevokeItem_CheckedChanged(object sender, EventArgs e)
        {
            AttestationReviewGrid.Rebind();
        }

        protected void btnConfirmSrvSubmit_Click(object sender, EventArgs e)
        {

            var snapShotId = hdnRevokeItem.Value;
            if (!string.IsNullOrWhiteSpace(snapShotId))
            {
                long snapshotActionId = Convert.ToInt64(snapShotId);
                this.UpdateEntity(snapshotActionId, true, AccessType.Revoke);
                ConfirmServiceAccount.VisibleOnPageLoad = false;
            }
        }

        protected void btnCancelSrvSubmit_Click(object sender, EventArgs e)
        {
            var snapShotId = hdnRevokeItem.Value;
            if (!string.IsNullOrWhiteSpace(snapShotId))
            {
                long snapshotActionId = Convert.ToInt64(snapShotId);
                this.UpdateEntity(snapshotActionId, true, AccessType.Approve);
            }
                
            ConfirmServiceAccount.VisibleOnPageLoad = false;
        }

        protected void btnIntrimSave_Click(object sender, EventArgs e)
        {
            OnSaveButtonClick();
        }

        protected void btnSaveAndSubmit_Click(object sender, EventArgs e)
        {
            OnSaveAndSubmitButtonClick();
        }
        public bool EnableAction
        {
            set
            {
                this.btnIntrimSave.Enabled = value;
                this.btnSaveAndSubmit.Enabled = value;
            }
        }
    }
}