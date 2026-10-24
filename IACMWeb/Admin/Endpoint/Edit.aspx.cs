using System;
using DI.Chakra.Dto;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Web.Admin.Endpoint
{
    public partial class Edit : DI.Chakra.Web.UIBasePage, IContextObjectRequired
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["GoBackTo"] = Request.UrlReferrer;
                int epId = Convert.ToInt32(this.ContextObjectId);
                if (Request.QueryString["EndpointId"] != null && !string.IsNullOrEmpty(Request.QueryString["EndpointId"]))
                {
                    epId = Convert.ToInt32(Convert.ToString(Request.QueryString["EndpointId"]));
                }
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointSystemId", epId.ToString());
                EndpointConfiguration epConfig = DomainApiClient<EndpointConfiguration>.GetByAction<EndpointConfiguration>(DomainApiAction.Endpoint.EndPoint.Configuration.EndpointConfiguration.GetByEndpointSystemId, parameter);
                WizardUC.EndpointType = epConfig.EndpointSystem.EndPointSystemType.EndpointSystemTypeName;
                this.WizardUC.EndpointConfiguration = epConfig;
            }
        }
    }
}