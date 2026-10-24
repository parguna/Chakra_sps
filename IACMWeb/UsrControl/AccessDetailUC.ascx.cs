using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.OrgChart.UC
{
    public partial class AccessDetailUC : UIBaseUC
    {
        private const string V_ID = "ENTITY_ID";

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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }

        protected void AccountDetail_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            LoadGrid();
        }

        public void BindGrid()
        {
            LoadGrid();
            AccountDetail.DataBind();
        }

        public void LoadGrid()
        {
            Dictionary<String, string> parameter = new Dictionary<String, string>();
            parameter.Add("userID", Convert.ToString(this.EntityId));
            parameter.Add("pwdReset", Boolean.FalseString);
            AccountDetail.DataSource = DomainApiClient<Dto.Presentation.PresentationUserAccount>
                .GetByAction<List<Dto.Presentation.PresentationUserAccount>>
                (DomainApiAction.Endpoint.Presentation.PresentationUserAccount.GetUserAccountDetail, parameter);
        }

        protected void AccountDetail_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
        {
            switch (e.DetailTableView.Name)
            {
                case "Entitlements":
                    {
                        GridDataItem parentItem = e.DetailTableView.ParentItem;
                        int EndpointUserId = Convert.ToInt32(parentItem.GetDataKeyValue("EndpointUserId"));
                        EndpointType ApplicationType = (EndpointType)Convert.ToInt32(parentItem.GetDataKeyValue("EndPointSystemType"));

                        Dictionary<String, string> parameter = new Dictionary<String, string>();
                        parameter.Add("endpointUserId", EndpointUserId.ToString());
                        parameter.Add("applicationType", ApplicationType.ToString());
                        e.DetailTableView.DataSource = DomainApiClient<Dto.Presentation.PresentationUserEntitlement>.GetByAction<List<Dto.Presentation.PresentationUserEntitlement>>(DomainApiAction.Endpoint.Presentation.PresentationUserEntitlement.GetEntitlementDetail, parameter);
                        break;
                    }
                case "Permissions":
                    {
                        GridDataItem parentItem = e.DetailTableView.ParentItem;
                        int EndpointUserId = Convert.ToInt32(parentItem.GetDataKeyValue("EndpointUserId"));
                        string EntitlementName = Convert.ToString(parentItem.GetDataKeyValue("Name"));
                        EndpointType ApplicationType = (EndpointType)Convert.ToInt32(parentItem.GetDataKeyValue("EndPointSystemType"));

                        Dictionary<String, string> parameter = new Dictionary<String, string>();
                        parameter.Add("endpointUserId", EndpointUserId.ToString());
                        parameter.Add("applicationType", ApplicationType.ToString());
                        parameter.Add("EntitlementName", EntitlementName.ToString());
                        e.DetailTableView.DataSource = DomainApiClient<Dto.Presentation.PresentationUserEntitlement>.GetByAction<List<Dto.Presentation.PresentationUserEntitlement>>(DomainApiAction.Endpoint.Presentation.PresentationUserEntitlement.GetPermissionDetail, parameter);
                        break;
                    }
            }
        }

        //protected void AccountDetail_PreRender(object sender, EventArgs e)
        //{
        //    if (IsPostBack)
        //    {
        //        AccountDetail.Rebind();
        //    }
        //}
    }
}