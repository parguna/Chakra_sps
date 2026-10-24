using System;
using DI.Chakra.Dto;
using System.Collections.Generic;

namespace DI.Chakra.Web.Admin.Endpoint
{
    public partial class Detail : DI.Chakra.Web.UIBasePage, IContextObjectRequired
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int epId = Convert.ToInt32(this.ContextObjectId);
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointSystemId", epId.ToString());
                EndpointConfiguration epConfig = DomainApiClient<EndpointConfiguration>.GetByAction<EndpointConfiguration>(DomainApiAction.Endpoint.EndPoint.Configuration.EndpointConfiguration.GetByEndpointSystemId, parameter);
                WizardUC.EndpointType = epConfig.EndpointSystem.EndPointSystemType.EndpointSystemTypeName;
                this.WizardUC.EndpointConfiguration = epConfig;
            }
        }
    }
}

