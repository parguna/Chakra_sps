using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Report.Provisioning
{
    public partial class AccessDetailMatrixUC : UIBaseUC
    {
        public void Rebind()
        {
            AccountDetail.Rebind();
        }

        #region Events
        protected void AccountDetail_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
           

            AccountDetail.DataSource = DomainApiClient<Dto.Presentation.PresentationUserAccount>
                .GetByAction<List<Dto.Presentation.PresentationUserAccount>>(DomainApiAction.Endpoint.Presentation.PresentationUserAccount.GetAllUserAccountDetail, null);

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
            }
        }
       
        #endregion

        //protected void AccountDetail_PreRender(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        // setting header text based on application level values
        //        foreach (GridColumn col in AccountDetail.Columns)
        //        {
        //            if (col.UniqueName == "EndpointSystemName")
        //                col.HeaderText = T("App.Common.Endpoint") + " Name";
        //        }
        //        AccountDetail.Rebind();
        //    }
        //}
    }
}