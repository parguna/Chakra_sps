using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Report.Attestation
{
    public partial class AttestationsUC : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void AttesationDetail_DetailTableDataBind(object sender, Telerik.Web.UI.GridDetailTableDataBindEventArgs e)
        {
            switch (e.DetailTableView.Name)
            {
                case "AttestationEndPoints":
                    {
                        GridDataItem parentItem = e.DetailTableView.ParentItem;
                        long AttestationId = Convert.ToInt64(parentItem.GetDataKeyValue("Id"));
                      
                        Dictionary<String, string> parameter = new Dictionary<String, string>();
                        parameter.Add("AttestationId", AttestationId.ToString());
                        e.DetailTableView.DataSource = DomainApiClient<Dto.Presentation.PresentationAttestationEndPoint>.GetByAction<List<Dto.Presentation.PresentationAttestationEndPoint>>(DomainApiAction.Endpoint.Presentation.PresentationAttestationEndPoint.GetAttestationEndPoints, parameter);
                        break;
                    }
            }
        }

        protected void AttesationDetail_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindAttestation();
        }

        private void BindAttestation()
        {
            AttesationDetail.DataSource = DomainApiClient<Dto.Presentation.PresentationAttestation>
                .GetByAction<List<Dto.Presentation.PresentationAttestation>>(DomainApiAction.Endpoint.Presentation.PresentationAttestation.GetAttestations, null);
        }
    }
}