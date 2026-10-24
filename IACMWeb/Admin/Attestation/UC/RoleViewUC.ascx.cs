using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Web.Admin.Attestation.UC
{
    public partial class RoleViewUC : UIBaseUC
    {
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
        bool isActionable = false;
        bool isActionExtendable = false;
        bool isNextLevelAllowed = true;
        bool showReserved1 = false; bool showReserved2 = false; bool showReserved3 = false; bool showReserved4 = false; bool showReserved5 = false;
        bool showReserved6 = false; bool showReserved7 = false; bool showReserved8 = false; bool showReserved9 = false; bool showReserved10 = false;
        bool showActionReserved1 = false; bool showActionReserved2 = false; bool showActionReserved3 = false;
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

        /// <summary>
        /// page load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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
                var snapshotActionId = item["SnapshotActionId"].Text;
                AttestationReviewHistoryUC.AttestationActionId = Convert.ToInt64(snapshotActionId);
                AttestationReviewHistoryUC.TaskId = Convert.ToInt64(TaskId);
                AttestationReviewHistoryUC.LoadForm();
                ReviewHistory.VisibleOnPageLoad = true;
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
                
                if (e.RebindReason == GridRebindReason.InitialLoad)
                {
                    if (IsUnSavedChangesAvailable)
                        InitialLoad = false;
                    else
                        InitialLoad = true;
                }
                else
                {
                    InitialLoad = false;
                }

                parameter.Add("taskId", this.TaskId.ToString());
                parameter.Add("currentUser", sessionStatus.User.UserName);                
                parameter.Add("level", "0");
                parameter.Add("isInitialLoad", InitialLoad.ToString());
                
				List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail> attestationUserDetails = new List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>();
                attestationUserDetails = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>>
                        (DomainApiAction.Tasks.Task.GetEndpointsForAttestationTaskReview, parameter);

                task = getCurrentTask(this.TaskId);               
                
                if (attestationUserDetails != null && attestationUserDetails.Count > 0)
                {
                    var firstRow = attestationUserDetails.First();
                    isActionable = firstRow.IsActionable;
                    isNextLevelAllowed = firstRow.IsNextLevelAvailable;
                    isActionExtendable = firstRow.IsActionExtendable;

                    showReserved1 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved1));
                    showReserved2 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved2));
                    showReserved3 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved3));
                    showReserved4 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved4));
                    showReserved5 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved5));
                    showReserved6 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved6));
                    showReserved7 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved7));
                    showReserved8 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved8));
                    showReserved9 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved9));
                    showReserved10 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved10));

                    showActionReserved1 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved1));
                    showActionReserved2 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved2));
                    showActionReserved3 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved3));

                    AttestationReviewGrid.MasterTableView.ExpandCollapseColumn.Display = isNextLevelAllowed;
                    AttestationReviewGrid.MasterTableView.EnableHierarchyExpandAll = isNextLevelAllowed;

                    //just to hide the approve revoke and comments from top/master grid
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ApproveGridTemplateColumn").Visible = isActionable;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("RevokeGridTemplateColumn").Visible = isActionable;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ReviewCommentsBoundColumn").Visible = isActionable;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("EffectiveDateTemplateColumn").Visible = isActionExtendable;
                    
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("LastLogon").Display = false;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ReviewHistory").Display = false;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("TemplateEditColumn").Visible = false;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Delta").Display = false;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("AccountType").Display = false;

                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved1").Visible = showReserved1;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved2").Visible = showReserved2;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved3").Visible = showReserved3;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved4").Visible = showReserved4;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved5").Visible = showReserved5;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved6").Visible = showReserved6;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved7").Visible = showReserved7;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved8").Visible = showReserved8;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved9").Visible = showReserved9;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved10").Visible = showReserved10;

                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ActionReserved1").Display = showActionReserved1;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ActionReserved2").Display = showActionReserved2;
                    AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ActionReserved3").Display = showActionReserved3;

                    //if (CurrentAttestationType == AttestationType.Privileged)
                    //{
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

                    //    var endpointSystemTypeId = firstRow.EndpointSystemType;
                    //    if (endpointSystemTypeId == (long)Infrastructure.Enums.EndpointType.CasinoManagementSystem)
                    //    {
                    //        AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved1").HeaderText = "View";
                    //        AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved2").HeaderText = "Create";
                    //        AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved3").HeaderText = "Modify";
                    //        AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved4").HeaderText = "Delete";
                    //        AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved5").HeaderText = "Print";
                    //        AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved6").HeaderText = "Export";
                    //    }
                    //    else if (endpointSystemTypeId == (long)Infrastructure.Enums.EndpointType.PSFinance)
                    //    {
                    //        AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved1").HeaderText = "MenuName";
                    //        AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved2").HeaderText = "BarName";
                    //        AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved3").HeaderText = "ContentReference";
                    //        AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Reserved4").HeaderText = "IsDisplay";
                    //    }
                    //}

                    //FirstLevel Dont have any history
                    if (Task.WorkflowStateId == 1)
                        AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ReviewHistory").Display = false;
                    else
                        AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("ReviewHistory").Display = true;
                }               
                this.AttestationReviewGrid.DataSource = attestationUserDetails;
                
                UnSavedReviews.Visible = IsUnSavedChangesAvailable;
            }
        }

        protected void AttestationReviewGrid_DetailTableDataBind(object sender, Telerik.Web.UI.GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;

            //grid functionality like allow paging, filtering, expand were handled here for detailed grid
            e.DetailTableView.IsFilterItemExpanded = false;
            e.DetailTableView.AllowPaging = false;
            e.DetailTableView.EnableHierarchyExpandAll = true;
            e.DetailTableView.AllowSorting = false;

            List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails = new List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>();
            long level = (long)dataItem.GetDataKeyValue("Level");
            long endpointSystemTypeId = (long)dataItem.GetDataKeyValue("EndpointSystemType");
            string entity = dataItem.GetDataKeyValue("Entity").ToString();
            if(level > 0)
                entity = dataItem.GetDataKeyValue("Entity").ToString();
            string endpointSystemId = dataItem.GetDataKeyValue("EndpointSystemId").ToString();            
            long bindingLevel = (long)dataItem.GetDataKeyValue("BindingLevel");
            long keyId = (long)dataItem.GetDataKeyValue("KeyId");
            var endpointSystemType = dataItem.GetDataKeyValue("EndpointSystemType");
            string category = dataItem.GetDataKeyValue("ReviewCategory").ToString();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", this.TaskId.ToString());
            parameter.Add("keyId", keyId.ToString());
            parameter.Add("currentUser", sessionStatus.User.UserName);
            parameter.Add("level", level.ToString());
            parameter.Add("bindingLevel", bindingLevel.ToString());
            parameter.Add("entity", entity);
            parameter.Add("endpointSystemId", endpointSystemId);
            //need to be configurable
            parameter.Add("displayHierarchyStartLevel", "2");
            parameter.Add("isInitialLoad", InitialLoad.ToString());


            gridNextLevelDetails = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>>
                            (DomainApiAction.Tasks.Task.GetDetailsForAttestationTaskReviewByRole, parameter);



            if (gridNextLevelDetails != null && gridNextLevelDetails.Count > 0)
            {
                var firstRow = gridNextLevelDetails.First();
                isActionable = firstRow.IsActionable;
                isNextLevelAllowed = firstRow.IsNextLevelAvailable;
                isActionExtendable = firstRow.IsActionExtendable;

                //enable paging for the inside grid, only if the count greater than 10.
                e.DetailTableView.AllowPaging = gridNextLevelDetails.Count > 10;

                showReserved1 = gridNextLevelDetails.Any(a => !string.IsNullOrEmpty(a.Reserved1));
                showReserved2 = gridNextLevelDetails.Any(a => !string.IsNullOrEmpty(a.Reserved2));
                showReserved3 = gridNextLevelDetails.Any(a => !string.IsNullOrEmpty(a.Reserved3));
                showReserved4 = gridNextLevelDetails.Any(a => !string.IsNullOrEmpty(a.Reserved4));
                showReserved5 = gridNextLevelDetails.Any(a => !string.IsNullOrEmpty(a.Reserved5));
                showReserved6 = gridNextLevelDetails.Any(a => !string.IsNullOrEmpty(a.Reserved6));
                showReserved7 = gridNextLevelDetails.Any(a => !string.IsNullOrEmpty(a.Reserved7));
                showReserved8 = gridNextLevelDetails.Any(a => !string.IsNullOrEmpty(a.Reserved8));
                showReserved9 = gridNextLevelDetails.Any(a => !string.IsNullOrEmpty(a.Reserved9));
                showReserved10 = gridNextLevelDetails.Any(a => !string.IsNullOrEmpty(a.Reserved10));

                showActionReserved1 = gridNextLevelDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved1));
                showActionReserved2 = gridNextLevelDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved2));
                showActionReserved3 = gridNextLevelDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved3));
            }

            //want to show risk info in details level then remove the below line
            e.DetailTableView.Columns.FindByUniqueName("TemplateEditColumn").Visible = false;

            e.DetailTableView.Columns.FindByUniqueName("LastLogon").Display = false;
            e.DetailTableView.Columns.FindByUniqueName("Delta").Display = false;
            e.DetailTableView.Columns.FindByUniqueName("AccountType").Display = false;

            e.DetailTableView.Columns.FindByUniqueName("Name").HeaderText = "Name";
            e.DetailTableView.Columns.FindByUniqueName("DisplayName").HeaderText = "Description";
            e.DetailTableView.Columns.FindByUniqueName("Type").HeaderText = "Entitlement Type";
            
            if (level-1 == 1)
            {
                e.DetailTableView.Columns.FindByUniqueName("Name").HeaderText = "User Name";
                e.DetailTableView.Columns.FindByUniqueName("DisplayName").HeaderText = "Display Name";
            }

            //just to hide the approve revoke and comments based on the configuration            
            e.DetailTableView.Columns.FindByUniqueName("ApproveGridTemplateColumn").Visible = isActionable;
            e.DetailTableView.Columns.FindByUniqueName("RevokeGridTemplateColumn").Visible = isActionable;
            e.DetailTableView.Columns.FindByUniqueName("ReviewCommentsBoundColumn").Visible = isActionable;
            e.DetailTableView.Columns.FindByUniqueName("EffectiveDateTemplateColumn").Visible = isActionExtendable;
            e.DetailTableView.Columns.FindByUniqueName("ReviewHistory").Visible = isActionable;
            e.DetailTableView.ExpandCollapseColumn.Display = isNextLevelAllowed;
            e.DetailTableView.EnableHierarchyExpandAll = isNextLevelAllowed;

            //if you want to enable all levels in single click, uncomment the below row
            //e.DetailTableView.HierarchyDefaultExpanded = isNextLevelAllowed;

            //reserved column visible
            e.DetailTableView.Columns.FindByUniqueName("Reserved1").Display = showReserved1;
            e.DetailTableView.Columns.FindByUniqueName("Reserved2").Display = showReserved2;
            e.DetailTableView.Columns.FindByUniqueName("Reserved3").Display = showReserved3;
            e.DetailTableView.Columns.FindByUniqueName("Reserved4").Display = showReserved4;
            e.DetailTableView.Columns.FindByUniqueName("Reserved5").Display = showReserved5;
            e.DetailTableView.Columns.FindByUniqueName("Reserved6").Display = showReserved6;
            e.DetailTableView.Columns.FindByUniqueName("Reserved7").Display = showReserved7;
            e.DetailTableView.Columns.FindByUniqueName("Reserved8").Display = showReserved8;
            e.DetailTableView.Columns.FindByUniqueName("Reserved9").Display = showReserved9;
            e.DetailTableView.Columns.FindByUniqueName("Reserved10").Display = showReserved10;

            e.DetailTableView.Columns.FindByUniqueName("ActionReserved1").Display = showActionReserved1;
            e.DetailTableView.Columns.FindByUniqueName("ActionReserved2").Display = showActionReserved2;
            e.DetailTableView.Columns.FindByUniqueName("ActionReserved3").Display = showActionReserved3;
            //FirstLevel Dont have any history
            if (Task.WorkflowStateId == 1)
                e.DetailTableView.Columns.FindByUniqueName("ReviewHistory").Visible = false;
            else
                e.DetailTableView.Columns.FindByUniqueName("ReviewHistory").Visible = true;

			//want to show risk info in details level then remove the below line
            e.DetailTableView.Columns.FindByUniqueName("TemplateEditColumn").Visible = false;
            e.DetailTableView.DataSource = gridNextLevelDetails;
        }

        protected void AttestationReviewGrid_PreRender(object sender, EventArgs e)
        {
            AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Name").HeaderText = "Endpoint Name";
            AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("DisplayName").HeaderText = "Endpoint Type";
            AttestationReviewGrid.MasterTableView.Columns.FindByUniqueName("Type").HeaderText = "Type";

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

                if (isChanged)
                {
                    e.Item.BackColor = System.Drawing.Color.FromArgb(228, 236, 245);
                }

                bool isEditable = true;
                var actionUserId = Task.ActionUserId.HasValue ? Task.ActionUserId.Value : 0;
                if (Task.IsGroupTask && !Task.IsDelegated && Task.AssignedTo.HasValue)
                    actionUserId = (long)Task.AssignedTo;
                if (actionUserId != sessionStatus.UserId || Task.IsCompleted || Task.Action == StatusType.Approved || Task.Action == StatusType.AutoCompleted)
                    isEditable = false;

                IsEditable = isEditable;

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
                        EffectiveDateButton.Enabled = isEditable;
                }
            }
        }

        protected void ApproveRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            var selectedRadioButtonRow = (((radioButton).Parent.Parent as GridItem) as GridEditableItem);
            long snapshotActionId = Convert.ToInt64(selectedRadioButtonRow.GetDataKeyValue("SnapshotActionId"));
            this.UpdateEntity(snapshotActionId, radioButton.Checked, AccessType.Approve);
        }

        protected void RevokeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            var selectedRadioButtonRow = (((radioButton).Parent.Parent as GridItem) as GridEditableItem);
            long snapshotActionId = Convert.ToInt64(selectedRadioButtonRow.GetDataKeyValue("SnapshotActionId"));
            this.UpdateEntity(snapshotActionId, radioButton.Checked, AccessType.Revoke);
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            SessionStatusImpl _sessionStatus = new SessionStatusImpl();

            if (_sessionStatus.RedirectFrom == "Tasks")
                Response.Redirect("~//Prov//Task//List.aspx");
            else if (_sessionStatus.RedirectFrom == "Home")
            {
                Response.Redirect("~//Home//Home.aspx");
            }
            else if (_sessionStatus.RedirectFrom == "GroupTask")
                Response.Redirect("~//Task//GroupTasks.aspx");
            else if (_sessionStatus.RedirectFrom == "AttestationCampaigntaskDetail")
                Response.Redirect("~//Report//Attestation//AttestationCampaigntaskDetail.aspx?ContextId="+ campaignId.ToString());
            else if (_sessionStatus.RedirectFrom == "AttestationCampaignStatus")
                Response.Redirect("~/Report/Attestation/AttestationCampaignStatus.aspx?ContextId=" + campaignId.ToString());
            else if (_sessionStatus.RedirectFrom == "AttestationOverallStatus")
                Response.Redirect("~/Report/Attestation/AttestationOverallStatus.aspx");
            else if (_sessionStatus.RedirectFrom == "AttestationCampaignReport")
            {
                Response.Redirect("~/Report/Attestation/AttestationCampaignReport.aspx");
            }
            else
                Response.Redirect("~//Task//List.aspx");
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
            long snapshotActionId = Convert.ToInt64(commentLinkButtonRow.GetDataKeyValue("SnapshotActionId"));
            string category = commentLinkButtonRow.GetDataKeyValue("ReviewCategory").ToString();
            var comments = commentLinkButtonRow.GetDataKeyValue("Comments");
            string comment = comments == null ? "" : commentLinkButtonRow.GetDataKeyValue("Comments").ToString();
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
        }
    }
}