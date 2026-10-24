using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class CurrentAccessInactiveUC : UIBaseUC
    {
        #region Members
        private const string V_ID = "ENTITY_ID";
        private const string V_ISSHOWICON = "ISSHOWICON";
        private const string V_SENSITIVEROLE = "V_SENSITIVEROLE";
        private const string V_USERSODREPORT = "V_USERSODREPORT";
        private const string V_ISFROMACCESSREQUEST = "V_ISFROMACCESSREQUEST";
        private const string V_ISVISIBLE = "V_ISVISIBLE";
        #endregion

        #region Properties


        public long EntityId
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return -1;
                return (long)ViewState[V_ID];
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }

        public bool isFromAccessRequest
        {
            get
            {
                if (ViewState[V_ISFROMACCESSREQUEST] == null)
                    return false;
                return (bool)ViewState[V_ISFROMACCESSREQUEST];
            }
            set
            {
                ViewState[V_ISFROMACCESSREQUEST] = value;
            }
        }
        public bool IsVisible
        {
            get
            {
                if (ViewState[V_ISVISIBLE] == null)
                    return false;
                return (bool)ViewState[V_ISVISIBLE];
            }
            set
            {
                ViewState[V_ISVISIBLE] = value;
            }
        }
        #endregion

        #region Method
        public void Rebind()
        {
            AccountDetail.Rebind();
        }
        #endregion

        #region Events
        protected void AccountDetail_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {

            Dictionary<String, string> parameter = new Dictionary<String, string>();
            parameter.Add("userID", EntityId.ToString());
            AccountDetail.DataSource = DomainApiClient<Dto.Presentation.PresentationUserEntitlement>
                  .GetByAction<List<Dto.Presentation.PresentationUserEntitlement>>(DomainApiAction.Endpoint.Presentation.PresentationUserEntitlement.GetUserAccountDetailInactiveAccounts, parameter);
                                
            if (isFromAccessRequest)
            {
                UserRiskUC.isFromAccessRequest = true;
                UserSODConflict.isFromAccessRequest = true;
            }
            else
            {
                UserRiskUC.isFromAccessRequest = false;
                UserSODConflict.isFromAccessRequest = false;
            }
            UserSODConflict.UserId = EntityId;
            UserRiskUC.UserId = EntityId;
            UserRiskUC.Initialize();

            //lnkShowRisk.Visible = UserSODConflict.UserSODAndSAConflicts.Count > 0;
            //lnkShowRisk.Visible = IsShowIcon;
        }
        protected void AccountDetail_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
        {
            try
            {
                GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
                var category = dataItem.GetDataKeyValue("Category").ToString();
                EndpointType ApplicationType = (EndpointType)Convert.ToInt32(dataItem.GetDataKeyValue("EndPointSystemType"));
                if (!string.IsNullOrEmpty(category))
                {
                    //e.DetailTableView.Columns.FindByUniqueName("EndpointSystemName").Visible = false;
                    //e.DetailTableView.Columns.FindByUniqueName("TemplateEditColumn").Visible = false;
                    e.DetailTableView.EnableHierarchyExpandAll = false;

                    if (category == "Account")
                    {
                        long EndpointUserId = Convert.ToInt64(dataItem.GetDataKeyValue("EntitlementId"));
                        long EndpointSystemId = Convert.ToInt64(dataItem.GetDataKeyValue("EndpointSystemId"));

                        Dictionary<String, string> parameter = new Dictionary<String, string>();
                        parameter.Add("userId", EntityId.ToString());
                        parameter.Add("endpointUserId", EndpointUserId.ToString());
                        parameter.Add("endpointSytemId", EndpointSystemId.ToString());
                        e.DetailTableView.DataSource = DomainApiClient<Dto.Presentation.PresentationUserEntitlement>.GetByAction<List<Dto.Presentation.PresentationUserEntitlement>>(DomainApiAction.Endpoint.Presentation.PresentationUserEntitlement.GetUserAccountEntitlment, parameter);
                        e.DetailTableView.ExpandCollapseColumn.Display = false;
                    }
                    else
                    {
                        GridDataItem parentItem = e.DetailTableView.ParentItem;
                        int EndpointUserId = Convert.ToInt32(parentItem.GetDataKeyValue("EntitlementId"));
                        Dictionary<String, string> parameter = new Dictionary<String, string>();
                        parameter.Add("endpointUserId", EndpointUserId.ToString());
                        parameter.Add("applicationType", ApplicationType.ToString());
                        e.DetailTableView.DataSource = DomainApiClient<Dto.Presentation.PresentationUserEntitlement>.GetByAction<List<Dto.Presentation.PresentationUserEntitlement>>(DomainApiAction.Endpoint.Presentation.PresentationUserEntitlement.GetEntitlementHierachyDetail, parameter);
                        e.DetailTableView.ExpandCollapseColumn.Display = false;
                        e.DetailTableView.Columns.FindByUniqueName("ReviewDate").Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void AccountDetail_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "ShowInfo")
            {
                GridDataItem item = e.Item as GridDataItem;
                long AccountReconId = (long)item.GetDataKeyValue("EntitlementId");
                AccountDetailsUC.EntityId = Convert.ToInt64(AccountReconId);
                AccountDetailsUC.Initialize();
                UserAccountDetail.VisibleOnPageLoad = true;
            }
            else if (e.CommandName == "ShowWarningInfo")
            {
                GridDataItem item = e.Item as GridDataItem;
                string roleCode = item["Name"].Text.ToString();
                UserSODConflict.EntitlementConflict = roleCode;
                UserSODConflict.BindConflicts();
                SensitiveRoleDetails.VisibleOnPageLoad = true;
            }
            //else if(e.CommandName == "ExpandCollapse" && !e.Item.Expanded && e.Item.OwnerTableView != AccountDetail.MasterTableView)
            //{
            //    //((GridTableView)e.Item.OwnerTableView.ch) .NestedTableViews[0].Rebind();
            //    GridDataItem item = e.Item as GridDataItem;
            //    item.OwnerTableView.Rebind();
            //    ((GridTableView)item.ChildItem.NestedTableViews[0]).Rebind();
            //}      
        }

        protected void AccountDetail_ItemDataBound(object sender, GridItemEventArgs e)
        {
            try
            {
                //Get the instance of the right type
                GridDataItem dataBoundItem = e.Item as GridDataItem;
                if (dataBoundItem != null)
                {
                    //dataBoundItem[""]
                    //if (dataBoundItem["TemplateRiskColumn"] != null && dataBoundItem["TemplateRiskColumn"].Controls.Count > 1)
                    //{
                    //    dataBoundItem["TemplateRiskColumn"].Controls[1].Visible = false;
                    //}

                    var isSensitiveRole = dataBoundItem["IsSensitiveAccess"].Text;

                    if (!string.IsNullOrEmpty(isSensitiveRole) && isSensitiveRole == "1")
                    {
                        //if (dataBoundItem["TemplateRiskColumn"].Controls.Count > 1)
                        //{
                        //    dataBoundItem["TemplateRiskColumn"].Controls[1].Visible = true;
                        //}
                    }

                    var isSODConflict = dataBoundItem["IsSODConflict"].Text;

                    if (!string.IsNullOrEmpty(isSODConflict) && isSODConflict == "1")
                    {
                        //if (dataBoundItem["TemplateRiskColumn"].Controls.Count > 1)
                        //{
                        //    dataBoundItem["TemplateRiskColumn"].Controls[1].Visible = true;
                        //}
                    }

                    dataBoundItem["TemplateEditColumn"].Controls[1].Visible = false;
                    var endpointType = dataBoundItem.GetDataKeyValue("EndPointSystemType");
                    if (endpointType != null)
                    {
                        var endpointSystemType = (EndpointType)Convert.ToInt32(endpointType);
                        if (endpointSystemType == EndpointType.ActiveDirectory)
                        {
                            dataBoundItem["TemplateEditColumn"].Controls[1].Visible = true;
                        }
                    }

                }

                if (e.Item is GridDataItem && e.Item.OwnerTableView != AccountDetail.MasterTableView)
                {
                    GridTableView detailTable = (GridTableView)e.Item.OwnerTableView;
                    detailTable.GetColumn("EndpointSystemName").Visible = false;
                    detailTable.GetColumn("AccountType").Visible = false;
                    detailTable.GetColumn("TemplateEditColumn").Visible = false;
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            UserAccountDetail.VisibleOnPageLoad = false;
        }

        protected void btnSensitiveRoleCancel_Click(object sender, EventArgs e)
        {
            SensitiveRoleDetails.VisibleOnPageLoad = false;
        }
        #endregion

        protected void AccountDetail_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // setting header text based on application level values
                foreach (GridColumn col in AccountDetail.Columns)
                {
                    if (col.UniqueName == "EndpointSystemName")
                        col.HeaderText = T("App.Common.Endpoint") + " Name";
                }
                AccountDetail.Rebind();
            }
            //AccountDetail.MasterTableView.Columns.FindByUniqueName("TemplateRiskColumn").Visible = false;
            AccountDetail.RetainExpandStateOnRebind = true;
            //uncomment below line will render properly but it has other side effects.   
            //AccountDetail.Rebind();
        }

        protected void AccountDetail_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridHeaderItem && e.Item.OwnerTableView != AccountDetail.MasterTableView)
            {
                //e.Item.Style["display"] = "none";
                GridHeaderItem header = (GridHeaderItem)e.Item;
                header["Description"].Text = "Description";
                header["Name"].Text = "Access";
            }
        }

        //protected void ExclusionGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        //{
        //    Dictionary<String, string> parameter = new Dictionary<String, string>();
        //    parameter.Add("userID", EntityId.ToString());
        //    ExclusionGrid.DataSource = DomainApiClient<Dto.UserExclusion>
        //        .GetByAction<List<Dto.UserExclusion>>(DomainApiAction.UserExclusion.GetAllUserExclusionByUserID, parameter);
        //}

        //protected void lnkShowRisk_Click(object sender, EventArgs e)
        //{
        //    UserSODConflict.EntitlementConflict = string.Empty;
        //    UserSODConflict.BindConflicts();
        //    SensitiveRoleDetails.VisibleOnPageLoad = true;
        //}
    }
}