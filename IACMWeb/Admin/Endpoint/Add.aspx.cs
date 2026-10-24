using System;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Web.Admin.Endpoint
{
    public partial class Add : DI.Chakra.Web.UIBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["Type"] != null)
                {
                    WizardUC.EndpointType = ((EndpointType)Enum.Parse(typeof(EndpointType), Request.QueryString["Type"]));
                }
                else
                {
                    WizardUC.EndpointType = (EndpointType.ActiveDirectory);
                }
            }
        }
    }
}