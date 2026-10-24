using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Infrastructure;
namespace DI.Chakra.Web.Prov.Task
{
    public partial class DetailType : UIBasePage, IContextObjectRequired
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (TypeofRequest == Convert.ToString(DI.Chakra.Infrastructure.Enums.RequestType.Attestation) || TypeofRequest == "Access Review")
                {
                    string adminMode = "Yes";
                    Response.Redirect("~/Admin/Attestation/NReview.aspx?"+ ToQueryString(Request.QueryString)+"&Id=" + ContextObjectId + "&IsAdminMode=" + adminMode);
                }else if (TypeofRequest == DI.Chakra.Infrastructure.Enums.RequestType.BulkRequest.GetDescription())
                {
                    Response.Redirect("~/Task/BulkTask/BulkTaskDetail.aspx" + ToQueryString(Request.QueryString)+"&Id="+ ContextObjectId);
                }
                else
                {
                    Response.Redirect("Detail.aspx" + ToQueryString(Request.QueryString));
                }
            }
        }

        public string ToQueryString(NameValueCollection nvc)
        {
            StringBuilder sb = new StringBuilder("?");
            bool first = true;
            foreach (string key in nvc.AllKeys)
            {
                if (key == null) continue;
                foreach (string value in nvc.GetValues(key))
                {
                    if (!first)
                    {
                        sb.Append("&");
                    }
                    sb.AppendFormat("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value));
                    first = false;
                }
            }

            return sb.ToString();
        }
    }
}