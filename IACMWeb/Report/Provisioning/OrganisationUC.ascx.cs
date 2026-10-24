using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Report.Provisioning
{
    public partial class OrganisationUC : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void OrgDetail_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindOrgInfo();
        }

        private void BindOrgInfo()
        {

            OrgDetail.DataSource = DomainApiClient<Dto.Presentation.PresentationOrganisation>
                .GetByAction<List<Dto.Presentation.PresentationOrganisation>>(DomainApiAction.Provisioning.PresentationOrg.GetAllOrganizationInfo, null);

        }
    }
}