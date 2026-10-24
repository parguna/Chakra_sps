using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class RequestedEntitlementUC : UIBaseUC
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
            RequestRepeater.DataSource = this.AdhocAccessRequest.RequestedItems;
            RequestRepeater.DataBind();
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RequestRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            AdditionalEntitlementDetailUC AdditionalInfoUC = (AdditionalEntitlementDetailUC)e.Item.FindControl("AdditionalEntitlementDetailUC");
            AdditionalInfoUC.PresentationAccessRequestItem = (PresentationAccessRequestItem)e.Item.DataItem;
            AdditionalInfoUC.Initialize();
        }
    }
}