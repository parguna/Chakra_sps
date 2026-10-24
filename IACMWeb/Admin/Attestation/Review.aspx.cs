using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Collections.Generic;

namespace DI.Chakra.Web.Admin.Attestation
{
    public partial class Review : UIBasePage, IContextObjectRequired
    {
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
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
                if (sessionStatus.RedirectFrom != null && sessionStatus.RedirectFrom == "TaskDetails")
                {
                    trrback.Visible = true;
                    trrbtn.Visible = false;
                    tddbtn.Visible = false;
                }
                this.EnabledSubmitButtons();
            }
            this.AddCommentsUserControl.OnCommentsSave += CommentsSave_Click;
            this.AddCommentsUserControl.OnCommentsCancel += CommentsCancel_Click;
        }
        protected void AttestationReviewGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "ShowWarningInfo")
            {
                GridDataItem item = e.Item as GridDataItem;
                UserSODConflict.UserId = (long)item.GetDataKeyValue("AttestationReview.UserId");
                UserSODConflict.BindConflicts();
                SensitiveRoleDetails.VisibleOnPageLoad = true;
            }
        }
        protected void AttestationReviewGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (!e.IsFromDetailTable)
            {
                DI.Chakra.Dto.Task task;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", this.TaskId.ToString());

                if (e.RebindReason == GridRebindReason.InitialLoad)
                {
                    parameter.Add("isInitialLoad", "true");
                    task = DomainApiClient<Dto.Task>.GetByAction<Dto.Task>
                        (DomainApiAction.Tasks.Task.GetAttestationTaskForReview, parameter);
                    this.RevertTaskInInterimStore(true);
                }
                else
                {
                    parameter.Add("isInitialLoad", "false");
                    task = DomainApiClient<Dto.Task>.GetByAction<Dto.Task>
                        (DomainApiAction.Tasks.Task.GetAttestationTaskForReview, parameter);
                }
                this.BindHeaderInfo(task);

                if (task != null && task.IsCompleted)
                    DisableSaveAndSubmit();

                this.AttestationReviewGrid.DataSource = task.TaskReviews;
            }
        }

        protected void AttestationReviewGrid_DetailTableDataBind(object sender, Telerik.Web.UI.GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
            DI.Chakra.Dto.Task task = this.GetTaskFromInterimStore(false);
            string[] strTempArr = new string[0];
            switch (e.DetailTableView.Name)
            {
                case "EPSystem":
                    {
                        long reviewId = (long)dataItem.GetDataKeyValue("ReviewId");
                        var endpointSystems = from taskReview in task.TaskReviews
                                              where taskReview.ReviewId == reviewId
                                              from reviewEP in taskReview.AttestationReview.ReviewEndpointSystems
                                              select new
                                              {
                                                  Id = reviewEP.AttestationReviewEndpointSystem.Id,
                                                  Approve = reviewEP.AttestationReviewEndpointSystem.Approve,
                                                  Revoke = reviewEP.AttestationReviewEndpointSystem.Revoke,
                                                  EndpointUserName = reviewEP.AttestationReviewEndpointSystem.EndpointUserName,
                                                  HostName = reviewEP.AttestationReviewEndpointSystem.EndpointSystemTypeName + " - " + reviewEP.AttestationReviewEndpointSystem.HostName,
                                                  EndpointSystemTypeName = reviewEP.AttestationReviewEndpointSystem.EndpointSystemTypeName,
                                                  EndpointSystemTypeId = reviewEP.AttestationReviewEndpointSystem.EndpointSystemTypeId,
                                                  IsChanged = reviewEP.AttestationReviewEndpointSystem.IsChanged,
                                                  PreviousAccess = reviewEP.AttestationReviewEndpointSystem.PreviousAccess,
                                                  AccountType = reviewEP.AttestationReviewEndpointSystem.AccountType,
                                                  IsActive = reviewEP.AttestationReviewEndpointSystem.IsActive ? "Yes" : "No",
                                              };
                        e.DetailTableView.DataSource = endpointSystems;
                        break;
                    }

                case "EPSystemL2":
                    {
                        long reviewEndpointId = (long)dataItem.GetDataKeyValue("Id");
                        IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewL2Detail> l2EndpointSystems = (from taskReview in task.TaskReviews
                                                                                                                     from reviewEP in taskReview.AttestationReview.ReviewEndpointSystems
                                                                                                                     where reviewEP.ReviewEndpointId == reviewEndpointId
                                                                                                                     from reviewEPL2 in reviewEP.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details
                                                                                                                     select new DI.Chakra.Dto.Presentation.PresentationAttestationReviewL2Detail
                                                                                                                     {
                                                                                                                         EndpointSystemId = (int)reviewEP.AttestationReviewEndpointSystem.EndpointSystemTypeId,
                                                                                                                         Id = reviewEPL2.Id,
                                                                                                                         L2DetailId = reviewEPL2.ReviewEndpointId,
                                                                                                                         Approve = reviewEPL2.Approve,
                                                                                                                         Revoke = reviewEPL2.Revoke,
                                                                                                                         AccessName = reviewEPL2.AccessName,
                                                                                                                         AccessType = reviewEPL2.AccessType,
                                                                                                                         AccessDescription = reviewEPL2.AccessDescription,
                                                                                                                         ResourceType = reviewEPL2.ResourceType,
                                                                                                                         ResourceCompletePath = reviewEPL2.ResourceCompletePath,
                                                                                                                         PreviousAccess = reviewEPL2.PreviousAccess,
                                                                                                                         IsChanged = reviewEPL2.IsChanged,
                                                                                                                     }).ToList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewL2Detail>();

                        long endpointSystemTypeId = (from taskReview in task.TaskReviews
                                                     from reviewEP in taskReview.AttestationReview.ReviewEndpointSystems
                                                     where reviewEP.ReviewEndpointId == reviewEndpointId
                                                     select reviewEP).FirstOrDefault().AttestationReviewEndpointSystem.EndpointSystemTypeId;

                        if (endpointSystemTypeId == (byte)EndpointType.SAP)
                        {
                            ExpandAllUptoTCode.Visible = true;
                            //foreach (var l2Detail in l2EndpointSystems)
                            //{
                            //    l2Detail.AccessType = this.getRoleDesrcription(l2Detail.L2DetailId);
                            //}

                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessNameL2GridBoundColumn").HeaderText = "Role";

                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessTypeL2GridBoundColumn").Visible = true;

                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").Visible = true;
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").HeaderText = "Description";
                            e.DetailTableView.Columns.FindByUniqueNameSafe("ResourceCompletePathL2GridBoundColumn").Visible = false;

                        }
                        else if (endpointSystemTypeId == (byte)EndpointType.SAPEnterprisePortal)
                        {
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessNameL2GridBoundColumn").HeaderText = "Role";
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessTypeL2GridBoundColumn").Visible = true;
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").Visible = true;
                            e.DetailTableView.Columns.FindByUniqueNameSafe("ResourceCompletePathL2GridBoundColumn").Visible = false;
                            e.DetailTableView.ExpandCollapseColumn.Visible = false;

                        }
                        else if (endpointSystemTypeId == (byte)EndpointType.InhouseSystems)
                        {
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessNameL2GridBoundColumn").HeaderText = "Permission";
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessTypeL2GridBoundColumn").Visible = true;
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").Visible = true;
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").HeaderText = "Permission Description";
                            e.DetailTableView.Columns.FindByUniqueNameSafe("ResourceCompletePathL2GridBoundColumn").Visible = false;
                            e.DetailTableView.ExpandCollapseColumn.Visible = false;
                        }
                        else if (endpointSystemTypeId == (byte)EndpointType.ActiveDirectory)
                        {
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessNameL2GridBoundColumn").HeaderText = "Groups";
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessTypeL2GridBoundColumn").Visible = true;
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").Visible = true;
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").HeaderText = "Groups Description";
                            e.DetailTableView.Columns.FindByUniqueNameSafe("ResourceCompletePathL2GridBoundColumn").Visible = false;
                            e.DetailTableView.ExpandCollapseColumn.Visible = false;
                        }
                        else if (endpointSystemTypeId == (byte)EndpointType.LotusNotes)
                        {
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessNameL2GridBoundColumn").HeaderText = "Groups";
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessTypeL2GridBoundColumn").Visible = true;
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").Visible = true;
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").HeaderText = "Groups Description";
                            e.DetailTableView.Columns.FindByUniqueNameSafe("ResourceCompletePathL2GridBoundColumn").Visible = false;
                            e.DetailTableView.ExpandCollapseColumn.Visible = false;
                        }
                        else if (endpointSystemTypeId == (byte)EndpointType.Peoplesoft)
                        {
                            ExpandAllUptoTCode.Visible = true;
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessNameL2GridBoundColumn").HeaderText = "Roles";
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessTypeL2GridBoundColumn").Visible = true;
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").Visible = true;
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").HeaderText = "Roles Description";
                            e.DetailTableView.Columns.FindByUniqueNameSafe("ResourceCompletePathL2GridBoundColumn").Visible = false;
                           // e.DetailTableView.ExpandCollapseColumn.Visible = false;
                        }
                        else
                        {
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessNameL2GridBoundColumn").HeaderText = "Permission";
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessTypeL2GridBoundColumn").Visible = true;
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").Visible = true;
                            e.DetailTableView.Columns.FindByUniqueNameSafe("ResourceCompletePathL2GridBoundColumn").Visible = false;
                        }
                        e.DetailTableView.DataSource = l2EndpointSystems;
                        break;
                    }

                case "EPSystemL3":
                    {
                        var tableView = ((GridTableView)(dataItem as GridDataItem).ChildItem.NestedTableViews[0]);
                        
                        foreach (GridEditableItem edititem in (dataItem as GridDataItem).ChildItem.NestedTableViews[0].GetItems(GridItemType.EditItem))
                        {

                        }

                        
                        long l2DetailId = (long)dataItem.GetDataKeyValue("L2DetailId");
                        long Id = (long)dataItem.GetDataKeyValue("Id");
                        //long l2DetailId = Convert.ToInt64(dataItem.GetDataKeyValue("L2DetailId").ToString());
                        long endpointSystemTypeId = (from taskReview in task.TaskReviews
                                                     from reviewEP in taskReview.AttestationReview.ReviewEndpointSystems
                                                     from reviewEPL2 in reviewEP.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details
                                                     where reviewEPL2.ReviewEndpointId== l2DetailId
                                                     select reviewEP).FirstOrDefault().AttestationReviewEndpointSystem.EndpointSystemTypeId;

                        if (endpointSystemTypeId == (byte)EndpointType.SAP)
                        {
                            ExpandAllUptoTCode.Visible = true;
                            var value = (from taskReview in task.TaskReviews
                                         from reviewEP in taskReview.AttestationReview.ReviewEndpointSystems
                                         from reviewEPL2 in reviewEP.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details
                                         where reviewEPL2.ReviewEndpointId == l2DetailId
                                         select reviewEP);

                            long endPointSystemId = value.FirstOrDefault().AttestationReviewEndpointSystem.EndpointSystemId;
                            string roleName =  (dataItem.DataItem is Dto.Presentation.PresentationAttestationReviewL2Detail) ? ((Dto.Presentation.PresentationAttestationReviewL2Detail)dataItem.DataItem).AccessName : "";
                            IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPageSAP_Level1> sapLevel1Details = this.getLevel2DetailsForSAP("SINGLEROLE", roleName, (int)endPointSystemId);
                            var l3EndpointSystemsSAP = (from dtls in sapLevel1Details
                                                        select new
                                                        {
                                                            Id = endpointSystemTypeId,
                                                            L3DetailId = dtls.sapLevel1DetailId,
                                                            AccessName = dtls.sapLevel1Detail,
                                                            AccessType = dtls.sapLevel1DetailDesc,
                                                            Approve = false,
                                                            Revoke = false,
                                                            Hidden = false,
                                                            IsChanged = false,
                                                            PreviousAccess = "",
                                                        });

                            //sapLevel1Details.Clear();
                           var Item = e.DetailTableView.Columns.FindByDataField("AccessName");

                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessTypeL3GridBoundColumn").Visible = true;

                            e.DetailTableView.Columns.FindByUniqueNameSafe("ApproveL3GridTemplateColumn").Visible = false;

                            e.DetailTableView.Columns.FindByUniqueNameSafe("RevokeL3GridTemplateColumn").Visible = false;

                            e.DetailTableView.Columns.FindByUniqueNameSafe("HiddenL3GridTemplateColumn").Visible = false;

                            e.DetailTableView.Columns.FindByUniqueNameSafe("IsChangedL3GridTemplateColumn").Visible = false;

                            e.DetailTableView.Columns.FindByUniqueNameSafe("PreviousAccessL3GridBoundColumn").Visible = false;

                            e.DetailTableView.Columns.FindByUniqueNameSafe("CommentsL3GridBoundColumn").Visible = false;


                            if (sapLevel1Details.Count() > 0 && sapLevel1Details[0].sapLevel1DetailType.Equals("TCODE", StringComparison.InvariantCultureIgnoreCase) == true)
                            {
                                e.DetailTableView.EnableHierarchyExpandAll = true;
                                e.DetailTableView.ExpandCollapseColumn.Visible = true;
                                e.DetailTableView.Columns.FindByUniqueNameSafe("AccessNameL3GridBoundColumn").HeaderText = "TCodes";
                                e.DetailTableView.Columns.FindByUniqueNameSafe("AccessTypeL3GridBoundColumn").HeaderText = "Description";
                            }
                            else
                            {
                                e.DetailTableView.EnableHierarchyExpandAll = true;
                                e.DetailTableView.ExpandCollapseColumn.Visible = true;
                                e.DetailTableView.Columns.FindByUniqueNameSafe("AccessNameL3GridBoundColumn").HeaderText = "Authorization";
                                e.DetailTableView.Columns.FindByUniqueNameSafe("AccessTypeL3GridBoundColumn").HeaderText = "Description";
                            }
                            e.DetailTableView.DataSource = l3EndpointSystemsSAP;

                        }
                        else if (endpointSystemTypeId == (byte)EndpointType.Peoplesoft)
                        {
                            ExpandAllUptoTCode.Visible = true;
                            var value = (from taskReview in task.TaskReviews
                                         from reviewEP in taskReview.AttestationReview.ReviewEndpointSystems
                                         from reviewEPL2 in reviewEP.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details
                                         where reviewEPL2.Id == Id
                                         select reviewEP);

                            long endPointSystemId = value.FirstOrDefault().AttestationReviewEndpointSystem.EndpointSystemId;
                            string roleName = Convert.ToString(dataItem.GetDataKeyValue("AccessName"));  //(dataItem.DataItem is Dto.Presentation.PresentationAttestationReviewL2Detail) ? ((Dto.Presentation.PresentationAttestationReviewL2Detail)dataItem.DataItem).AccessName : "";
                            IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPagePeoplesoft_Level1> psftLevel1Details = this.getLevel1DetailsForPeoplesoft(roleName,(long)endPointSystemId);
                            var l3EndpointSystemsSAP = (from dtls in psftLevel1Details
                                                        select new
                                                        {
                                                            Id = endpointSystemTypeId,
                                                            L3DetailId = dtls.peopleSoftLevel1DetailId,
                                                            AccessName = dtls.peopleSoftLevel1Detail,
                                                            AccessType = dtls.peopleSoftLevel1DetailDesc,
                                                            Approve = false,
                                                            Revoke = false,
                                                            Hidden = false,
                                                            IsChanged = false,
                                                            PreviousAccess = "",
                                                        });

                            //sapLevel1Details.Clear();
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessTypeL3GridBoundColumn").Visible = true;

                            e.DetailTableView.Columns.FindByUniqueNameSafe("ApproveL3GridTemplateColumn").Visible = false;

                            e.DetailTableView.Columns.FindByUniqueNameSafe("RevokeL3GridTemplateColumn").Visible = false;

                            e.DetailTableView.Columns.FindByUniqueNameSafe("HiddenL3GridTemplateColumn").Visible = false;

                            e.DetailTableView.Columns.FindByUniqueNameSafe("IsChangedL3GridTemplateColumn").Visible = false;

                            e.DetailTableView.Columns.FindByUniqueNameSafe("PreviousAccessL3GridBoundColumn").Visible = false;

                            e.DetailTableView.Columns.FindByUniqueNameSafe("CommentsL3GridBoundColumn").Visible = false;


                            if (psftLevel1Details.Count() > 0 && psftLevel1Details[0].peopleSoftLevel1DetailType.Equals("Permission", StringComparison.InvariantCultureIgnoreCase) == true)
                            {
                                e.DetailTableView.EnableHierarchyExpandAll = true;
                                e.DetailTableView.ExpandCollapseColumn.Visible = true;
                                e.DetailTableView.Columns.FindByUniqueNameSafe("AccessNameL3GridBoundColumn").HeaderText = "Permission";
                                e.DetailTableView.Columns.FindByUniqueNameSafe("AccessTypeL3GridBoundColumn").HeaderText = "Description";
                            }
                            else
                            {
                                e.DetailTableView.EnableHierarchyExpandAll = true;
                                e.DetailTableView.ExpandCollapseColumn.Visible = true;
                                e.DetailTableView.Columns.FindByUniqueNameSafe("AccessNameL3GridBoundColumn").HeaderText = "Authorization";
                                e.DetailTableView.Columns.FindByUniqueNameSafe("AccessTypeL3GridBoundColumn").HeaderText = "Description";
                            }
                            e.DetailTableView.DataSource = l3EndpointSystemsSAP;

                        }
                        else
                        {
                            var l3EndpointSystems = from taskReview in task.TaskReviews
                                                    from reviewEP in taskReview.AttestationReview.ReviewEndpointSystems
                                                    from reviewEPL2 in reviewEP.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details
                                                    where reviewEPL2.Id == l2DetailId
                                                    from reviewEPL3 in reviewEPL2.AttestationReviewEndpointSystemL3Details
                                                    select new
                                                    {
                                                        Id = reviewEP.AttestationReviewEndpointSystem.EndpointSystemTypeId,
                                                        L3DetailId = reviewEPL3.Id,
                                                        Approve = reviewEPL3.Approve,
                                                        Revoke = reviewEPL3.Revoke,
                                                        Hidden = reviewEPL3.Hidden,
                                                        AccessName = reviewEPL3.AccessName,
                                                        AccessType = reviewEPL3.AccessType,
                                                        IsChanged = reviewEPL3.IsChanged,
                                                        PreviousAccess = reviewEPL3.PreviousAccess,
                                                    };


                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessNameL3GridBoundColumn").HeaderText = "Access Name";
                            e.DetailTableView.Columns.FindByUniqueNameSafe("AccessTypeL3GridBoundColumn").Visible = true;

                            e.DetailTableView.Columns.FindByUniqueNameSafe("ApproveL3GridTemplateColumn").HeaderText = "Approve";
                            e.DetailTableView.Columns.FindByUniqueNameSafe("ApproveL3GridTemplateColumn").HeaderStyle.Width = 50;
                            e.DetailTableView.Columns.FindByUniqueNameSafe("RevokeL3GridTemplateColumn").HeaderText = "Revoke";
                            e.DetailTableView.Columns.FindByUniqueNameSafe("HiddenL3GridTemplateColumn").Visible = false;

                            e.DetailTableView.DataSource = l3EndpointSystems;
                        }
                        break;
                    }

                case "SAPOnlyDetails":
                    string l2DetailId_1 = (string)dataItem.GetDataKeyValue("L3DetailId");
                    strTempArr = l2DetailId_1.Split(';');
                    if (strTempArr.Count() > 0 && l2DetailId_1.Contains("Authorization"))
                    {
                        IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPageSAP_Level1> sapLevel2Details = this.getLevel2DetailsForAuthorization(strTempArr[2], strTempArr[3], int.Parse(strTempArr[4]));
                        var l1EndpointSystemsSAP = (from dtls in sapLevel2Details
                                                    select new
                                                    {
                                                        sapLevel1DetailId = dtls.sapLevel1DetailId,
                                                        sapLevel1Detail = dtls.sapLevel1Detail,
                                                        sapLevel1DetailDesc = dtls.sapLevel1DetailDesc,
                                                        IsChanged = false
                                                    });

                        e.DetailTableView.DataSource = l1EndpointSystemsSAP;
                        e.DetailTableView.Columns.FindByUniqueNameSafe("DetailSapLevel1GridBoundColumn").HeaderText = "Authorization";
                        e.DetailTableView.Columns.FindByUniqueNameSafe("DetailTypeSapLevel1GridBoundColumn").HeaderText = "Description";
                    }
                    else
                    {
                        IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPageSAP_Level1> sapLevel2Details = new List<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPageSAP_Level1>();
                        var l1EndpointSystemsSAP = (from dtls in sapLevel2Details
                                                    select new
                                                    {
                                                        sapLevel1DetailId = dtls.sapLevel1DetailId,
                                                        sapLevel1Detail = dtls.sapLevel1Detail,
                                                        sapLevel1DetailDesc = dtls.sapLevel1DetailDesc,
                                                        IsChanged = false
                                                    });
                        e.DetailTableView.DataSource = l1EndpointSystemsSAP;
                        e.DetailTableView.Columns.FindByUniqueNameSafe("DetailSapLevel1GridBoundColumn").HeaderText = "";
                        e.DetailTableView.Columns.FindByUniqueNameSafe("DetailTypeSapLevel1GridBoundColumn").HeaderText = "";
                    }
                    break;

            }
        }

        private string getRoleDesrcription(long paramL2DetailId)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("paramL2DetailId", paramL2DetailId.ToString());
            return DomainApiClient<Dto.Attestation>.GetByAction<string>(DomainApiAction.Attestation.GetRoleDescription, parameter);
        }

        private IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPageSAP_Level1> getLevel1DetailsForSAP
            (long paramL2DetailId, long paramEndPointSystemId)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("paramL2DetailId", paramL2DetailId.ToString());
            parameter.Add("paramEndPointSystemId", paramEndPointSystemId.ToString());
            return DomainApiClient<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPageSAP_Level1>.
                GetByAction<IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPageSAP_Level1>>
                (DomainApiAction.Attestation.Presentation.PresentationAttestationReviewPageSAP_Level1.
                GetSAPLevel1InfosForAttestationReview, parameter);
        }

        private IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPagePeoplesoft_Level1> getLevel1DetailsForPeoplesoft
          (string paramRoleName, long paramEndPointSystemId)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("paramRoleName", paramRoleName.ToString());
            parameter.Add("paramEndPointSystemId", paramEndPointSystemId.ToString());
            return DomainApiClient<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPagePeoplesoft_Level1>.
                GetByAction<IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPagePeoplesoft_Level1>>
                (DomainApiAction.Attestation.Presentation.PresentationAttestationReviewPagePeoplesoft_Level1.
                GetPeoplesoftLevel1InfosForAttestationReview, parameter);
        }

        private IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPageSAP_Level1> getLevel2DetailsForSAP
            (string paramDetailType, string paramSingleRoleCode, long paramEndPointSystemId)
        {
            IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPageSAP_Level1> returnList = 
                new List<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPageSAP_Level1>();
            if (paramDetailType.Equals("SINGLEROLE", StringComparison.InvariantCultureIgnoreCase) == true)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("paramSingleRoleCode", paramSingleRoleCode.ToString());
                parameter.Add("paramEndPointSystemId", paramEndPointSystemId.ToString());
                // returnList = attestationFacade.getSAPLevel2InfosForAttestationReviewGrid(paramSingleRoleCode, paramEndPointSystemId);
                returnList = DomainApiClient<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPageSAP_Level1>.
                    GetByAction<IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPageSAP_Level1>>
                    (DomainApiAction.Attestation.Presentation.PresentationAttestationReviewPageSAP_Level1.
                    GetSAPLevel2InfosForAttestationReview, parameter);
            }
            return returnList;
        }

        private IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPagePeoplesoft_Level1> getLevel2DetailsForpeoplsoft
            (string paramDetailType, string paramSingleRoleCode, long paramEndPointSystemId)
        {
            IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPagePeoplesoft_Level1> returnList =
                new List<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPagePeoplesoft_Level1>();
            if (paramDetailType.Equals("SINGLEROLE", StringComparison.InvariantCultureIgnoreCase) == true)
            {
               Dictionary<string, string> parameter = new Dictionary<string, string>();
               parameter.Add("paramSingleRoleCode", paramSingleRoleCode.ToString());
                parameter.Add("paramEndPointSystemId", paramEndPointSystemId.ToString());
                // returnList = attestationFacade.getSAPLevel2InfosForAttestationReviewGrid(paramSingleRoleCode, paramEndPointSystemId);
                returnList = DomainApiClient<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPagePeoplesoft_Level1>.
                    GetByAction<IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPagePeoplesoft_Level1>>
                    (DomainApiAction.Attestation.Presentation.PresentationAttestationReviewPagePeoplesoft_Level1.
                    GetPeoplesoftLevel2InfosForAttestationReview, parameter);
            }
            return returnList;
        }

        private IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPageSAP_Level1> getLevel2DetailsForAuthorization
            (string paramDetailType, string paramSingleRoleCode, long paramEndPointSystemId)
        {

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("paramL2DetailId", paramSingleRoleCode.ToString());
            parameter.Add("paramEndPointSystemId", paramEndPointSystemId.ToString());
            return DomainApiClient<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPageSAP_Level1>.
                GetByAction<IList<DI.Chakra.Dto.Presentation.PresentationAttestationReviewPageSAP_Level1>>
                (DomainApiAction.Attestation.Presentation.PresentationAttestationReviewPageSAP_Level1.
                GetAuthorizationDataForAttestationReview, parameter);
        }

        protected void AttestationReviewGrid_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.AttestationReviewGrid.MasterTableView.Items[0].Expanded = true;
                this.AttestationReviewGrid.MasterTableView.Items[0].ChildItem.NestedTableViews[0].Items[0].Expanded = true;
                if (this.AttestationReviewGrid.MasterTableView.Items[0].ChildItem.NestedTableViews[0].Items[0].
                    ChildItem.NestedTableViews[0].Items.Count > 0)
                {
                    this.AttestationReviewGrid.MasterTableView.Items[0].ChildItem.NestedTableViews[0].Items[0].
                        ChildItem.NestedTableViews[0].Items[0].Expanded = true;
                }
            }
            else
            {
                foreach (GridDataItem dataItem in this.AttestationReviewGrid.MasterTableView.Items)
                {
                    foreach (GridTableView l1TableView in dataItem.ChildItem.NestedTableViews)
                    {
                        foreach (GridDataItem l1DataItem in l1TableView.Items)
                        {
                            foreach (GridTableView l2TableView in l1DataItem.ChildItem.NestedTableViews)
                            {
                                if (l2TableView.Items.Count > 0)
                                {
                                    if (l2TableView.Items[0].DataItem != null)
                                    {
                                        byte enpointSystemTypeId = Convert.ToByte(l2TableView.Items[0].DataItem.GetType().
                                            GetProperty("EndpointSystemId").GetValue(l2TableView.Items[0].DataItem));

                                        if (enpointSystemTypeId == (byte)EndpointType.SAP)
                                        {
                                            ExpandAllUptoTCode.Visible = true;
                                            l2TableView.Columns.FindByUniqueNameSafe("AccessNameL2GridBoundColumn").HeaderText = "Role";
                                            l2TableView.Columns.FindByUniqueNameSafe("AccessTypeL2GridBoundColumn").Visible = true;
                                            l2TableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").Visible = true;
                                            l2TableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").HeaderText = "Role Type";
                                            l2TableView.Columns.FindByUniqueNameSafe("ResourceCompletePathL2GridBoundColumn").Visible = false;
                                            l2TableView.AllowPaging = false;
                                        }
                                       else if (enpointSystemTypeId == (byte)EndpointType.Peoplesoft)
                                        {
                                            ExpandAllUptoTCode.Visible = true;
                                            l2TableView.Columns.FindByUniqueNameSafe("AccessNameL2GridBoundColumn").HeaderText = "Permission";
                                            l2TableView.Columns.FindByUniqueNameSafe("AccessTypeL2GridBoundColumn").Visible = true;
                                            l2TableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").Visible = true;
                                            l2TableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").HeaderText = "Permission Type";
                                            l2TableView.Columns.FindByUniqueNameSafe("ResourceCompletePathL2GridBoundColumn").Visible = false;
                                            l2TableView.AllowPaging = false;
                                        }
                                        else if (enpointSystemTypeId == (byte)EndpointType.InhouseSystems)
                                        {
                                            l2TableView.Columns.FindByUniqueNameSafe("AccessNameL2GridBoundColumn").HeaderText = "Permission";
                                            l2TableView.Columns.FindByUniqueNameSafe("AccessTypeL2GridBoundColumn").Visible = true;
                                            l2TableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").Visible = true;
                                            l2TableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").HeaderText = "Permission Description";
                                            l2TableView.ExpandCollapseColumn.Visible = false;
                                        }
                                        else
                                        {
                                            l2TableView.Columns.FindByUniqueNameSafe("AccessNameL2GridBoundColumn").HeaderText = "Permission";
                                            l2TableView.Columns.FindByUniqueNameSafe("AccessTypeL2GridBoundColumn").Visible = true;
                                            l2TableView.Columns.FindByUniqueNameSafe("AccessDescriptionL2GridBoundColumn").Visible = true;
                                            l2TableView.Columns.FindByUniqueNameSafe("ResourceCompletePathL2GridBoundColumn").Visible = false;
                                        }
                                    }
                                }
                                foreach (GridDataItem l2DataItem in l2TableView.Items)
                                {
                                    foreach (GridTableView l3TableView in l2DataItem.ChildItem.NestedTableViews)
                                    {
                                        if (l3TableView.Items.Count > 0)
                                        {
                                            if (l3TableView.Items[0].DataItem != null)
                                            {
                                                byte enpointSystemTypeId = Convert.ToByte(l3TableView.Items[0].DataItem.GetType().
                                                    GetProperty("Id").GetValue(l3TableView.Items[0].DataItem));

                                                if (enpointSystemTypeId == (byte)EndpointType.SAP)
                                                {

                                                    l3TableView.Columns.FindByUniqueNameSafe("AccessTypeL3GridBoundColumn").Visible = true;

                                                    l3TableView.Columns.FindByUniqueNameSafe("ApproveL3GridTemplateColumn").Visible = false;

                                                    l3TableView.Columns.FindByUniqueNameSafe("RevokeL3GridTemplateColumn").Visible = false;

                                                    l3TableView.Columns.FindByUniqueNameSafe("HiddenL3GridTemplateColumn").Visible = false;

                                                    l3TableView.Columns.FindByUniqueNameSafe("IsChangedL3GridTemplateColumn").Visible = false;

                                                    l3TableView.Columns.FindByUniqueNameSafe("PreviousAccessL3GridBoundColumn").Visible = false;

                                                    l3TableView.Columns.FindByUniqueNameSafe("CommentsL3GridBoundColumn").Visible = false;
                                                    l3TableView.AllowPaging = false;
                                                    string accessTypeID = Convert.ToString(l3TableView.Items[0].DataItem.GetType().
                                                        GetProperty("Id").GetValue(l3TableView.Items[0].DataItem));
                                                    if (accessTypeID.Contains("TCODE") == true)
                                                    {
                                                        l3TableView.EnableHierarchyExpandAll = true;
                                                        l3TableView.ExpandCollapseColumn.Visible = true;
                                                        l3TableView.Columns.FindByUniqueNameSafe("AccessNameL3GridBoundColumn").HeaderText = "TCodes";
                                                        l3TableView.Columns.FindByUniqueNameSafe("AccessTypeL3GridBoundColumn").HeaderText = "Description";
                                                        l3TableView.ExpandCollapseColumn.Visible = true;
                                                        foreach (GridDataItem l4TableItem in l3TableView.Items)
                                                        {
                                                            string l4AccessId = Convert.ToString(l4TableItem.DataItem.GetType().
                                                                GetProperty("Id").GetValue(l4TableItem.DataItem));
                                                            if (l4AccessId.Contains("Authorization"))
                                                            {
                                                                l4TableItem.ChildItem.NestedTableViews[0].ExpandCollapseColumn.Visible = true;
                                                                //l4TableItem.OwnerTableView.ExpandCollapseColumn.Visible = true;
                                                            }
                                                            else
                                                            {
                                                                l4TableItem.ChildItem.NestedTableViews[0].ExpandCollapseColumn.Visible = false;
                                                                //l4TableItem.OwnerTableView.ExpandCollapseColumn.Visible = false;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        l3TableView.EnableHierarchyExpandAll = true;
                                                        l3TableView.ExpandCollapseColumn.Visible = true;
                                                        l3TableView.Columns.FindByUniqueNameSafe("AccessNameL3GridBoundColumn").HeaderText = 
                                                            "SingleRoles";
                                                        l3TableView.Columns.FindByUniqueNameSafe("AccessTypeL3GridBoundColumn").HeaderText =
                                                            "Description";
                                                    }
                                                }
                                                else
                                                {
                                                    l3TableView.EnableHierarchyExpandAll = false;
                                                    l3TableView.ExpandCollapseColumn.Visible = false;
                                                    l3TableView.Columns.FindByUniqueNameSafe("AccessNameL3GridBoundColumn").HeaderText = 
                                                        "Access Name";
                                                    l3TableView.Columns.FindByUniqueNameSafe("AccessTypeL3GridBoundColumn").Visible = false;

                                                    l3TableView.Columns.FindByUniqueNameSafe("ApproveL3GridTemplateColumn").HeaderText = "Approve";
                                                    l3TableView.Columns.FindByUniqueNameSafe("ApproveL3GridTemplateColumn").HeaderStyle.Width = 50;
                                                    l3TableView.Columns.FindByUniqueNameSafe("RevokeL3GridTemplateColumn").HeaderText = "Revoke";
                                                    l3TableView.Columns.FindByUniqueNameSafe("HiddenL3GridTemplateColumn").Visible = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this.AttestationReviewGrid.RetainExpandStateOnRebind = true;
            this.AttestationReviewGrid.Rebind();
        }

        protected void AttestationReviewGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                bool isChanged;
                if (e.Item.DataItem.GetType() == typeof(TaskReview))
                {
                    isChanged = ((TaskReview)e.Item.DataItem).AttestationReview.IsChanged;
                }
                else
                {
                    isChanged = Convert.ToBoolean(e.Item.DataItem.GetType().GetProperty("IsChanged").GetValue(e.Item.DataItem, null));
                }
                if (isChanged)
                {
                    e.Item.BackColor = System.Drawing.Color.FromArgb(228, 236, 245);
                }
                if (e.Item.DataItem.GetType().GetProperty("L3DetailId") != null)
                {
                    string L3DetailId = e.Item.DataItem.GetType().GetProperty("L3DetailId").GetValue(e.Item.DataItem, null).ToString();
                    if (L3DetailId.Contains("TCODE") && !L3DetailId.Contains("Authorization"))
                    {
                        TableCell cell = (TableCell)(e.Item as GridDataItem)["ExpandColumn"];
                        cell.Controls[0].Visible = false;
                    }
                }
                if (sessionStatus.RedirectFrom != null && sessionStatus.RedirectFrom == "TaskDetails")
                {
                    RadioButton ApproveL3RadioButton = (RadioButton)e.Item.FindControl("ApproveL3RadioButton");
                    if (ApproveL3RadioButton != null)
                        ApproveL3RadioButton.Enabled = false;
                    RadioButton RevokeL3RadioButton = (RadioButton)e.Item.FindControl("RevokeL3RadioButton");
                    if (RevokeL3RadioButton != null)
                        RevokeL3RadioButton.Enabled = false;
                    RadioButton HiddenL3RadioButton = (RadioButton)e.Item.FindControl("HiddenL3RadioButton");
                    if (HiddenL3RadioButton != null)
                        HiddenL3RadioButton.Enabled = false;
                    ImageButton AddCommentLinkButton = (ImageButton)e.Item.FindControl("AddCommentLinkButton");
                    if (AddCommentLinkButton != null)
                        AddCommentLinkButton.Enabled = false;
                    RadioButton ApproveL2RadioButton = (RadioButton)e.Item.FindControl("ApproveL2RadioButton");
                    if (ApproveL2RadioButton != null)
                        ApproveL2RadioButton.Enabled = false;
                    RadioButton RevokeL2RadioButton = (RadioButton)e.Item.FindControl("RevokeL2RadioButton");
                    if (RevokeL2RadioButton != null)
                        RevokeL2RadioButton.Enabled = false;
                    RadioButton ApproveRadioButton = (RadioButton)e.Item.FindControl("ApproveRadioButton");
                    if (ApproveRadioButton != null)
                        ApproveRadioButton.Enabled = false;
                    RadioButton RevokeRadioButton = (RadioButton)e.Item.FindControl("RevokeRadioButton");
                    if (RevokeRadioButton != null)
                        RevokeRadioButton.Enabled = false;
                }
            }
        }

        protected void ApproveRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            long reviewEndpointId = this.GetKeyFieldValue(radioButton);
            bool value = radioButton.Checked;
            this.UpdateEndpointReview(reviewEndpointId, value, AccessType.Approve);
        }

        protected void RevokeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            long reviewEndpointId = this.GetKeyFieldValue(radioButton);
            bool value = radioButton.Checked;
            this.UpdateEndpointReview(reviewEndpointId, value, AccessType.Revoke);
        }

        protected void ApproveL2RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            long l2DetailId = this.GetIdValue(radioButton);
            bool value = radioButton.Checked;
            this.UpdateL2Detail(l2DetailId, value, AccessType.Approve);
        }

        protected void RevokeL2RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            long l2DetailId = this.GetIdValue(radioButton);
            bool value = radioButton.Checked;
            this.UpdateL2Detail(l2DetailId, value, AccessType.Revoke);
        }

        protected void ApproveL3RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            long l3DetailId = this.GetKeyFieldValue(radioButton);
            bool value = radioButton.Checked;
            this.UpdateL3Detail(l3DetailId, value, AccessType.Approve);
        }

        protected void RevokeL3RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            long l3DetailId = this.GetKeyFieldValue(radioButton);
            bool value = radioButton.Checked;
            this.UpdateL3Detail(l3DetailId, value, AccessType.Revoke);
        }

        protected void HiddenL3RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            long l3DetailId = this.GetKeyFieldValue(radioButton);
            bool value = radioButton.Checked;
            this.UpdateL3Detail(l3DetailId, value, AccessType.Hidden);
        }

        protected void RevertButton_Click(object sender, EventArgs e)
        {
            this.RevertTaskInInterimStore(false);
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            this.SaveTaskInInterimStore();
            this.IsSaved = true;
            this.EnabledSubmitButtons();
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            bool status;
            DI.Chakra.Dto.Task task = this.GetTaskFromInterimStore(true);
            if (task != null)
            {
                
                
                this.UpdateTempInterimStoreBeforeSubmit(task);
                Dto.Presentation.PresentationWorkflowCriteriaBase _criteria = new Dto.Presentation.ApprovalActionCriteria()
                {
                    RequestType = (RequestType)task.RequestTypeId,
                    TaskId = task.Id,
                    WorkflowInstanceId = task.WorkflowInstanceId
                };
                status = DomainApiClient<Object>.GetByAction<bool>("WorkflowController", DomainApiAction.WorkflowProcessor.SubmitWorkflowCriteria, null, _criteria);
            }
            else
            {
                status = false;
            }
            //throw new ApplicationException("Workflow Servicehelper ServiceAccessStrategy(WorkflowCriteriaBase- Dto to Domain mapping) Yet to be Implement ");

            //ServiceAccessStrategy _serviceAccessStrategy = new ServiceAccessStrategy(_criteria);
            //bool status = _serviceAccessStrategy.AccessWorkflowService();
            if (status)
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
                else
                    Response.Redirect("~//Task//List.aspx");
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Error, "Action not completed. Please contact administrator for more information.");
            }
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
            else
                Response.Redirect("~//Task//List.aspx");
        }

        protected void ExpandAllButton_Click(object sender, EventArgs e)
        {
            this.ExpandAll();
        }

        protected void ExpandAllUptoTCode_Click(object sender, EventArgs e)
        {
            this.ExpandUptoTCode();
        }

        protected void ExportExcelButton_Click(object sender, EventArgs e)
        {
            string ExportUrl = String.Format("~//Admin//Attestation//ExportToExcel.aspx?TaskId={0}", TaskId);
            Response.Redirect(ExportUrl);
        }

        protected void CollapseAllButton_Click(object sender, EventArgs e)
        {
            this.CollapseAll();
        }

        protected void AddCommentImageButton_Click(object sender, EventArgs e)
        {
            ImageButton commentLinkButton = (ImageButton)sender;
            this.AddCommentsUserControl.KeyId = Convert.ToInt64(commentLinkButton.CommandArgument);
            this.AddCommentsUserControl.CommandName = commentLinkButton.CommandName.ToString();
            this.CommentsWindow.VisibleOnPageLoad = true;
            string script = "function f(){$find(\"" + this.CommentsWindow.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
        }

        private void CommentsSave_Click(object sender, EventArgs e)
        {
            DI.Chakra.Dto.Task task = this.GetTaskFromInterimStore(false);

            if (task == null)
                return;

            string commandName = this.AddCommentsUserControl.CommandName;
            long keyID = this.AddCommentsUserControl.KeyId;
            string comments = this.AddCommentsUserControl.Comments;

            if (commandName.Equals("EndpointSystemComment", StringComparison.CurrentCultureIgnoreCase))
            {
                foreach (TaskReview taskReview in task.TaskReviews)
                {
                    foreach (ReviewEndpointSystem reviewEP in taskReview.AttestationReview.ReviewEndpointSystems)
                    {
                        if (reviewEP.AttestationReviewEndpointSystem.Id == keyID)
                        {
                            reviewEP.AttestationReviewEndpointSystem.Comments = comments;
                            reviewEP.AttestationReviewEndpointSystem.IsChanged = true;
                            this.UpdateL1ParentInfo(taskReview.AttestationReview, task);
                            this.UpdateTempInterimStore(task);
                        }
                    }
                }
            }

            else if (commandName.Equals("EndpointSystemL2Comment", StringComparison.CurrentCultureIgnoreCase))
            {
                foreach (TaskReview taskReview in task.TaskReviews)
                {
                    foreach (ReviewEndpointSystem reviewEP in taskReview.AttestationReview.ReviewEndpointSystems)
                    {
                        foreach (AttestationReviewEndpointSystemL2Detail l2Detail in reviewEP.AttestationReviewEndpointSystem.
                            AttestationReviewEndpointSystemL2Details)
                        {
                            if (l2Detail.Id == keyID)
                            {
                                l2Detail.Comments = comments;
                                l2Detail.IsChanged = true;
                                this.UpdateL2ParentInfo(reviewEP.AttestationReviewEndpointSystem, task);
                                this.UpdateTempInterimStore(task);
                            }
                        }
                    }
                }
            }
            else if (commandName.Equals("EndpointSystemL3Comment", StringComparison.CurrentCultureIgnoreCase))
            {
                foreach (TaskReview taskReview in task.TaskReviews)
                {
                    foreach (ReviewEndpointSystem reviewEP in taskReview.AttestationReview.ReviewEndpointSystems)
                    {
                        foreach (AttestationReviewEndpointSystemL2Detail l2Detail in reviewEP.AttestationReviewEndpointSystem.
                            AttestationReviewEndpointSystemL2Details)
                        {
                            foreach (AttestationReviewEndpointSystemL3Detail l3Detail in l2Detail.AttestationReviewEndpointSystemL3Details)
                            {
                                if (l3Detail.Id == keyID)
                                {
                                    l3Detail.Comments = comments;
                                    l3Detail.IsChanged = true;
                                    this.UpdateL3ParentInfo(l2Detail, task);
                                    this.UpdateTempInterimStore(task);
                                }
                            }
                        }
                    }
                }
            }
            this.CommentsWindow.VisibleOnPageLoad = false;
        }

        private void CommentsCancel_Click(object sender, EventArgs e)
        {
            this.CommentsWindow.VisibleOnPageLoad = false;
        }

        #region "Private Methods"
        private long GetKeyFieldValue(RadioButton radioButton)
        {
            long dataKeyValue = 0;
            foreach (Control control in radioButton.Parent.Parent.Controls)
            {
                foreach (Control childControl in control.Controls)
                {
                    if (childControl is HiddenField)
                    {
                        return Convert.ToInt64(((HiddenField)childControl).Value);
                    }
                }
              }
            return dataKeyValue;
        }

        private long GetIdValue(RadioButton radioButton) 
        {
            long dataKeyValue = 0;
            foreach (Control control in radioButton.Parent.Parent.Controls)
            {
                foreach (Control childControl in control.Controls)
                {
                    if (childControl is ImageButton)
                    {
                        return Convert.ToInt64(((ImageButton)childControl).CommandArgument);
                    }
                }
            }
            return dataKeyValue;
        }

        private void UpdateEndpointReview(long reviewEndpointId, bool value, AccessType accessType)
        {
            DI.Chakra.Dto.Task task = this.GetTaskFromInterimStore(false);
            if (task == null)
                return;

            foreach (TaskReview taskReview in task.TaskReviews)
            {
                foreach (ReviewEndpointSystem reviewEP in taskReview.AttestationReview.ReviewEndpointSystems)
                {
                    if (reviewEP.AttestationReviewEndpointSystem.Id == reviewEndpointId)
                    {
                        if (accessType == AccessType.Approve)
                        {
                            reviewEP.AttestationReviewEndpointSystem.Approve = value;
                            reviewEP.AttestationReviewEndpointSystem.Revoke = !value;
                        }
                        else if (accessType == AccessType.Revoke)
                        {
                            reviewEP.AttestationReviewEndpointSystem.Approve = !value;
                            reviewEP.AttestationReviewEndpointSystem.Revoke = value;
                        }
                        reviewEP.AttestationReviewEndpointSystem.IsChanged = true;
                        this.UpdateL1ChildInfo(reviewEP.AttestationReviewEndpointSystem, value, accessType);
                        this.UpdateL1ParentInfo(taskReview.AttestationReview, task);
                        this.UpdateTempInterimStore(task);
                        this.ReBindReviewInfo(task);
                        return;
                    }
                }
            }
        }

        private void UpdateL2Detail(long l2DetailId, bool value, AccessType accessType)
        {
            DI.Chakra.Dto.Task task = this.GetTaskFromInterimStore(false);
            if (task == null)
                return;

            foreach (TaskReview taskReview in task.TaskReviews)
            {
                foreach (ReviewEndpointSystem reviewEP in taskReview.AttestationReview.ReviewEndpointSystems)
                {
                    foreach (AttestationReviewEndpointSystemL2Detail l2Detail in reviewEP.AttestationReviewEndpointSystem.
                        AttestationReviewEndpointSystemL2Details)
                    {
                        if (l2Detail.Id == l2DetailId)
                        {
                            if (accessType == AccessType.Approve)
                            {
                                l2Detail.Approve = value;
                                l2Detail.Revoke = !value;
                            }
                            else if (accessType == AccessType.Revoke)
                            {
                                l2Detail.Approve = !value;
                                l2Detail.Revoke = value;
                            }
                            l2Detail.IsChanged = true;
                            this.UpdateL2ChildInfo(l2Detail, value, accessType);
                            this.UpdateL2ParentInfo(reviewEP.AttestationReviewEndpointSystem, task);
                            this.UpdateTempInterimStore(task);
                            this.ReBindReviewInfo(task);
                            return;
                        }
                    }
                }
            }
        }

        private void UpdateL3Detail(long l3DetailId, bool value, AccessType accessType)
        {
            DI.Chakra.Dto.Task task = this.GetTaskFromInterimStore(false);
            if (task == null)
                return;

            foreach (TaskReview taskReview in task.TaskReviews)
            {
                foreach (ReviewEndpointSystem reviewEP in taskReview.AttestationReview.ReviewEndpointSystems)
                {
                    long endpointSystem = reviewEP.AttestationReviewEndpointSystem.EndpointSystemTypeId;
                    foreach (AttestationReviewEndpointSystemL2Detail l2Detail in reviewEP.AttestationReviewEndpointSystem.
                        AttestationReviewEndpointSystemL2Details)
                    {
                        foreach (AttestationReviewEndpointSystemL3Detail l3Detail in l2Detail.AttestationReviewEndpointSystemL3Details)
                        {
                            if (l3Detail.Id == l3DetailId)
                            {
                                if (accessType == AccessType.Approve)
                                {
                                    l3Detail.Approve = value;
                                    l3Detail.Revoke = !value;
                                    l3Detail.Hidden = !value;
                                }
                                else if (accessType == AccessType.Revoke)
                                {
                                    l3Detail.Approve = !value;
                                    l3Detail.Revoke = value;
                                    l3Detail.Hidden = !value;
                                }
                                else if (accessType == AccessType.Hidden)
                                {
                                    l3Detail.Approve = !value;
                                    l3Detail.Revoke = !value;
                                    l3Detail.Hidden = value;
                                }
                                l3Detail.IsChanged = true;
                                this.UpdateL3ParentInfo(l2Detail, task);
                                this.UpdateTempInterimStore(task);
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void UpdateL1ParentInfo(AttestationReview review, DI.Chakra.Dto.Task task)
        {
            var selectedReview = (from taskReview in task.TaskReviews
                                  where taskReview.AttestationReview.Id == review.Id
                                  select taskReview.AttestationReview).FirstOrDefault();
            selectedReview.IsChanged = true;
        }

        private void UpdateL2ParentInfo(AttestationReviewEndpointSystem reviewEPS, DI.Chakra.Dto.Task task)
        {
            reviewEPS.IsChanged = true;
            var review = (from taskReview in task.TaskReviews
                          from reviewEP in taskReview.AttestationReview.ReviewEndpointSystems
                          where reviewEP.ReviewEndpointId == reviewEPS.Id
                          select taskReview.AttestationReview).FirstOrDefault();

            this.UpdateL1ParentInfo(review, task);
        }

        private void UpdateL3ParentInfo(AttestationReviewEndpointSystemL2Detail reviewEPSL2, DI.Chakra.Dto.Task task)
        {
            reviewEPSL2.IsChanged = true;
            var reviewEPS = (from taskReview in task.TaskReviews
                             from reviewEP in taskReview.AttestationReview.ReviewEndpointSystems
                             from reviewEPL2 in reviewEP.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details
                             where reviewEPL2.Id == reviewEPSL2.Id
                             select reviewEP.AttestationReviewEndpointSystem).FirstOrDefault();
            this.UpdateL2ParentInfo(reviewEPS, task);
        }

        private void UpdateL1ChildInfo(AttestationReviewEndpointSystem attestationReviewEndpointSystem, bool value, AccessType accessType)
        {
            foreach (AttestationReviewEndpointSystemL2Detail l2Detail in attestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details)
            {
                if (accessType == AccessType.Approve)
                {
                    l2Detail.Approve = value;
                    l2Detail.Revoke = !value;
                }
                else if (accessType == AccessType.Revoke)
                {
                    l2Detail.Approve = !value;
                    l2Detail.Revoke = value;
                }
                l2Detail.IsChanged = true;
                this.UpdateL2ChildInfo(l2Detail, value, accessType);
            }
        }

        private void UpdateL2ChildInfo(AttestationReviewEndpointSystemL2Detail l2Detail, bool value, AccessType accessType)
        {
            foreach (AttestationReviewEndpointSystemL3Detail l3Detail in l2Detail.AttestationReviewEndpointSystemL3Details)
            {
                if (accessType == AccessType.Approve)
                {
                    l3Detail.Approve = value;
                    l3Detail.Revoke = !value;
                }
                else if (accessType == AccessType.Revoke)
                {
                    l3Detail.Approve = !value;
                    l3Detail.Revoke = value;
                }
                l3Detail.IsChanged = true;
            }
        }

        private DI.Chakra.Dto.Task GetTaskFromInterimStore(bool isSaved)
        {
            InterimStore interimStore;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", TaskId.ToString());
            parameter.Add("isSaved", isSaved.ToString());
            parameter.Add("itemType", "1");
            interimStore = DomainApiClient<Dto.InterimStore>.GetByAction<Dto.InterimStore>
                (DomainApiAction.Tasks.InterimStore.GetByTaskId, parameter);
            //TransformXmlData transXmlData = new TransformXmlData();
            //transXmlData.XmlData = interimStore.XmlData;
            //return DomainApiClient<DI.Chakra.Dto.Task>.GetByAction<DI.Chakra.Dto.Task>(DomainApiAction.Tasks.Task.DeSerialize, null, transXmlData);
            if (interimStore != null)
            {
                return UtilityMethods.Deserialize<Dto.Task>(interimStore.XmlData);
            }
            else
            {
                return null;
            }
        }

        private void UpdateTempInterimStore(DI.Chakra.Dto.Task task)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", TaskId.ToString());
            parameter.Add("isSaved", "false");
            parameter.Add("itemType", "1");
            var interimStore = DomainApiClient<Dto.InterimStore>.GetByAction<Dto.InterimStore>
                (DomainApiAction.Tasks.InterimStore.GetByTaskId, parameter);
            if (interimStore != null)
            {
                //interimStore.XmlData = DomainApiClient<DI.Chakra.Dto.Task>.GetByAction<string>(DomainApiAction.Tasks.Task.Serialize, null, task);
                interimStore.XmlData = UtilityMethods.Serialize<Dto.Task>(task);
                DomainApiClient<Dto.InterimStore>.Save(interimStore);
            }
        }

        private void UpdateTempInterimStoreBeforeSubmit(DI.Chakra.Dto.Task task)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", TaskId.ToString());
            parameter.Add("isSaved", "true");
            parameter.Add("itemType", "1");
            var interimStore = DomainApiClient<Dto.InterimStore>.GetByAction<Dto.InterimStore>
                (DomainApiAction.Tasks.InterimStore.GetByTaskId, parameter);
            if (interimStore != null)
            {
                ////interimStore.XmlData = UtilityMethods.Serialize<DI.Chakra.Dto.Task>(task,true);
                //interimStore.XmlData = DomainApiClient<DI.Chakra.Dto.Task>.GetByAction<string>(DomainApiAction.Tasks.Task.Serialize, null, task);
                interimStore.XmlData = UtilityMethods.Serialize<Dto.Task>(task);
                DomainApiClient<Dto.InterimStore>.Save(interimStore);
            }
        }

        private void SaveTaskInInterimStore()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", TaskId.ToString());
            parameter.Add("isSaved", "false");
            parameter.Add("itemType", "1");
            var tempInterimStore = DomainApiClient<Dto.InterimStore>.GetByAction<Dto.InterimStore>
                (DomainApiAction.Tasks.InterimStore.GetByTaskId, parameter);
            parameter.Clear();
            parameter.Add("taskId", TaskId.ToString());
            parameter.Add("isSaved", "true");
            parameter.Add("itemType", "1");
            var interimStore = DomainApiClient<Dto.InterimStore>.GetByAction<Dto.InterimStore>
                (DomainApiAction.Tasks.InterimStore.GetByTaskId, parameter);
            interimStore.XmlData = tempInterimStore.XmlData;
            DomainApiClient<Dto.InterimStore>.Save(interimStore);
        }

        private void RevertTaskInInterimStore(bool onInitialLoad)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", TaskId.ToString());
            parameter.Add("isSaved", "true");
            parameter.Add("itemType", "1");
            var tempInterimStore = DomainApiClient<Dto.InterimStore>.GetByAction<Dto.InterimStore>
                (DomainApiAction.Tasks.InterimStore.GetByTaskId, parameter);
            parameter.Clear();
            parameter.Add("taskId", TaskId.ToString());
            parameter.Add("isSaved", "false");
            parameter.Add("itemType", "1");
            var interimStore = DomainApiClient<Dto.InterimStore>.GetByAction<Dto.InterimStore>
                (DomainApiAction.Tasks.InterimStore.GetByTaskId, parameter);
            interimStore.XmlData = tempInterimStore.XmlData;

            DomainApiClient<Dto.InterimStore>.Save(interimStore);
            if (!onInitialLoad)
            {
                var task = UtilityMethods.Deserialize<DI.Chakra.Dto.Task>(interimStore.XmlData);
                //TransformXmlData transXmlData = new TransformXmlData();
                //transXmlData.XmlData = interimStore.XmlData;
                //var task = DomainApiClient<DI.Chakra.Dto.Task>.GetByAction<DI.Chakra.Dto.Task>(DomainApiAction.Tasks.Task.DeSerialize, null, transXmlData);
                this.ReBindReviewInfo(task);
               
            }
        }

        private void BindHeaderInfo(DI.Chakra.Dto.Task task)
        {
            // DI.Chakra.Dto.Attestation attestation = DomainApiClient<Dto.Attestation>.GetById(task.AttestationId.Value, true);
            Dictionary<string, string> parameterId = new Dictionary<string, string>();
            parameterId.Add("id", Convert.ToString(task.AttestationId));
            Dto.Attestation attestation = DomainApiClient<Dto.Attestation>.GetByAction<Dto.Attestation>
                (DomainApiAction.Attestation.GetByIdIncludingAttestationProcesses, parameterId);
            var currentApprover = attestation.WorkflowConfiguration.WorkflowConfigurationDetails.
                FirstOrDefault(item => item.SequenceNumber == task.WorkflowStateId);
            this.lblAttName.Text = attestation.AttestationName;
            this.AttestationDescLabel.Text = attestation.AttestationDescription;
            this.lblReqDate.Text = task.CreatedDate.ToString();
            this.SubmitBeforeLabel.Text = task.CreatedDate.AddDays(currentApprover.ResponseTimeInDays).ToString();

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("workflowInstanceId", task.WorkflowInstanceId.ToString());
            parameter.Add("workflowStateId", task.WorkflowStateId.ToString());
            var previousApprovers = DomainApiClient<Dto.Presentation.PresentationPreviousApprovers>.
                GetByAction<List<Dto.Presentation.PresentationPreviousApprovers>>
                (DomainApiAction.Attestation.Presentation.PresentationPreviousApprovers.GetByWorkflowInstanceId, parameter);
            if (previousApprovers != null && previousApprovers.Count <= 0)
            {
                this.PreviousApproversDiv.Style["Height"] = "0px";
            }
            else if (previousApprovers.Count >= 3)
            {
                this.PreviousApproversDiv.Style["Height"] = "100px";
            }
            this.PreviousApproversRepeater.DataSource = previousApprovers;
            this.PreviousApproversRepeater.DataBind();
        }

        private void ReBindReviewInfo(DI.Chakra.Dto.Task task)
        {
            this.AttestationReviewGrid.DataSource = task.TaskReviews;
            this.AttestationReviewGrid.RetainExpandStateOnRebind = true;
            this.AttestationReviewGrid.Rebind();
            
        }
        private void DisableSaveAndSubmit()
        {
            this.SubmitButton.Enabled = false;
            this.SubmitButton1.Enabled = false;
            this.SaveButton.Enabled = this.SaveButton1.Enabled = false;
        }
        private void EnabledSubmitButtons()
        {
            this.SubmitButton.Enabled = this.SubmitButton1.Enabled = IsSaved;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", this.TaskId.ToString());
            parameter.Add("isInitialLoad", "true");
            DI.Chakra.Dto.Task task = DomainApiClient<Dto.Task>.GetByAction<Dto.Task>(DomainApiAction.Tasks.Task.
                GetAttestationTaskForReview, parameter);
            List<long> endPoints = (from taskReview in task.TaskReviews
                                    from ePoints in taskReview.AttestationReview.ReviewEndpointSystems
                                    select ePoints.AttestationReviewEndpointSystem.EndpointSystemTypeId
                             ).ToList();
            bool IsSapAvailableForReview = endPoints.Where(endpoint => endpoint == (int)EndpointType.SAP).ToList().Count > 0;
            //ExportExcelButton.Visible = IsSapAvailableForReview;
            ExpandAllUptoTCode.Visible = IsSapAvailableForReview;
            if(task.Action==StatusType.Approved)
            {
                RevertButton.Enabled = false;
                SaveButton.Enabled = false;
                SubmitButton.Enabled = false;
                Button1.Enabled = false;
                SubmitButton1.Enabled = false;
            }
        }

        private void ExpandAll()
        {
            foreach (GridDataItem dataItem in this.AttestationReviewGrid.MasterTableView.Items)
            {
                dataItem.Expanded = true;
                foreach (GridTableView l1TableView in dataItem.ChildItem.NestedTableViews)
                {
                    foreach (GridDataItem l1DataItem in l1TableView.Items)
                    {
                        l1DataItem.Expanded = true;
                        foreach (GridTableView l2TableView in l1DataItem.ChildItem.NestedTableViews)
                        {
                            foreach (GridDataItem l2DataItem in l2TableView.Items)
                            {
                                l2DataItem.Expanded = true;
                                foreach (GridTableView l3TableView in l2DataItem.ChildItem.NestedTableViews)
                                {
                                    foreach (GridDataItem l3DataItem in l3TableView.Items)
                                    {
                                        if (l3DataItem.DataItem.GetType().GetProperty("Id").GetValue(l3DataItem.DataItem, null) != null)
                                        {
                                            string L3DetailId = l3DataItem.DataItem.GetType().GetProperty("Id").
                                                GetValue(l3DataItem.DataItem, null).ToString();
                                            if (L3DetailId.Contains("Authorization"))
                                                l3DataItem.Expanded = true;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
            //this.AttestationReviewGrid.Rebind();
        }

        private void ExpandUptoTCode()
        {
            foreach (GridDataItem dataItem in this.AttestationReviewGrid.MasterTableView.Items)
            {
                dataItem.Expanded = true;
                foreach (GridTableView l1TableView in dataItem.ChildItem.NestedTableViews)
                {
                    foreach (GridDataItem l1DataItem in l1TableView.Items)
                    {
                        l1DataItem.Expanded = true;
                        foreach (GridTableView l2TableView in l1DataItem.ChildItem.NestedTableViews)
                        {
                            foreach (GridDataItem l2DataItem in l2TableView.Items)
                            {
                                l2DataItem.Expanded = true;
                            }
                        }
                    }
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

    }
}