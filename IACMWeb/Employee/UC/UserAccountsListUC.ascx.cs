using System;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;

namespace DI.Chakra.Web.Employee.UC
{
    public partial class UserAccountsListUC : UIBaseUC
    {
        #region Members
        private const string V_ID = "ENTITY_ID";
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
        #endregion

        #region Events
        protected void AccountDetail_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userID", EntityId.ToString());
            parameter.Add("pwdReset", Boolean.FalseString);
            AccountDetail.DataSource = DomainApiClient<Dto.Presentation.PresentationUserAccount>.
                GetByAction<List<Dto.Presentation.PresentationUserAccount>>
                (DomainApiAction.Endpoint.Presentation.PresentationUserAccount.GetUserAccountDetail, parameter);
        }

        protected void AccountDetail_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
        {
            switch (e.DetailTableView.Name)
            {
                case "Entitlements":
                    {
                        GridDataItem parentItem = e.DetailTableView.ParentItem;
                        long EndpointUserId = Convert.ToInt64(parentItem.GetDataKeyValue("EndpointUserId"));
                        EndpointType ApplicationType = (EndpointType)Convert.ToInt32(parentItem.GetDataKeyValue("EndPointSystemType"));
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("endpointUserId", EndpointUserId.ToString());
                        parameter.Add("applicationType", ApplicationType.ToString());
                        e.DetailTableView.DataSource = DomainApiClient<Dto.Presentation.PresentationUserEntitlement>.
                            GetByAction<List<Dto.Presentation.PresentationUserEntitlement>>
                            (DomainApiAction.Endpoint.Presentation.PresentationUserEntitlement.GetEntitlementHierachyDetail, parameter);
                        break;
                    }
            }
        }

        protected void AccountDetail_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "ShowInfo")
            {
                GridDataItem item = e.Item as GridDataItem;
                long AccountReconId = (long)item.GetDataKeyValue("AcountReconcilationId");
                //AccountDetailUC.EntityId = AccountReconId;
                UserAccountDetail.VisibleOnPageLoad = true;
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            UserAccountDetail.VisibleOnPageLoad = false;
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
                        col.HeaderText = T("App.Common.Endpoint");
                }
                AccountDetail.Rebind();
            }
        }
    }
}