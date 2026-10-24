using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class AccessRequestEntitlementDetails : System.Web.UI.UserControl
    {
        private const string V_REQUESTINFO = "V_REQUESTINFO";

        public Dto.AdHocAccessRequest AdhocAccessRequest
        {
            get
            {
                if (ViewState[V_REQUESTINFO] == null)
                    return null;
                return (Dto.AdHocAccessRequest)ViewState[V_REQUESTINFO];
            }
            set
            {
                ViewState[V_REQUESTINFO] = value;
            }
        }

        public void Initialize()
        {
            

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            EntitlementReportGrid.Rebind();
        }

        protected void lnkEntitlementDetails_Click(object sender, EventArgs e)
        {
            EntitlementInfoWindow.VisibleOnPageLoad = true;
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            EntitlementInfoWindow.VisibleOnPageLoad = false;
        }

        protected void EntitlementReportGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (this.AdhocAccessRequest == null) return;
            Dictionary<string, string> parameters1 = new Dictionary<string, string>();
            parameters1.Add("adhocAccessRequestId", this.AdhocAccessRequest.Id.ToString());

            var accessRequest = DomainApiClient<Dto.AccessRequest>.GetByAction<Dto.AccessRequest>(DomainApiAction.Provisioning.AccessRequest.GetByAdHocAccessRequestId, parameters1);
            if (accessRequest != null)
            {
                long accessRequestId = accessRequest.Id;
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("accessRequestId", accessRequestId.ToString());

                var ReconList = DomainApiClient<Dto.Presentation.PresentationAccountReconcilationReport>
                  .GetByAction<List<Dto.Presentation.PresentationAccountReconcilationReport>>(DomainApiAction.Endpoint.Presentation.PresentationUser.GetEntitlementDetailsForAccessRequest, parameters).ToList();
                EntitlementReportGrid.DataSource = ReconList;
                if(ReconList != null && ReconList.Count > 0){
                    EntitlementReportGrid.Visible = true;
                    noEntitlementIfo.Visible = false;
                }
                else
                {
                    noEntitlementIfo.Visible = true;
                    EntitlementReportGrid.Visible = false;
                }
               // EntitlementReportGrid.DataBind();
            }
        }
    }
}