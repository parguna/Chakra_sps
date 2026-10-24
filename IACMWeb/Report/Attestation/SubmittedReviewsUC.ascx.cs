using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Report.Attestation
{
    public partial class SubmittedReviewsUC : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SubmittedReviews_DetailTableDataBind(object sender, Telerik.Web.UI.GridDetailTableDataBindEventArgs e)
        {
            switch (e.DetailTableView.Name)
            {
                case "ReviewerDetails":
                    {
                        GridDataItem parentItem = e.DetailTableView.ParentItem;
                        long AttestationId = Convert.ToInt64(parentItem.GetDataKeyValue("Id"));

                        Dictionary<String, string> parameter = new Dictionary<String, string>();
                        parameter.Add("AttestationId", AttestationId.ToString());
                        e.DetailTableView.DataSource = DomainApiClient<Dto.Presentation.PresentationReviewer>
                .GetByAction<List<Dto.Presentation.PresentationReviewer>>(DomainApiAction.Endpoint.Presentation.PresentationAttestation.GetAttestationReviewers, parameter);
                      //  e.DetailTableView.DataSource = DomainApiClient<Dto.Presentation.PresentationReviewer>
                      //.GetByAction<List<Dto.Presentation.PresentationReviewer>>("GetAttestationRejectedReviewers", parameter);
                        break;
                    }
                case "AttestationReviewDetails":
                    {
                        GridDataItem parentItem = e.DetailTableView.ParentItem;
                        long TaskId = Convert.ToInt64(parentItem.GetDataKeyValue("TaskId"));

                        Dictionary<String, string> parameter = new Dictionary<String, string>();
                        parameter.Add("TaskId", TaskId.ToString());
                        e.DetailTableView.DataSource = DomainApiClient<Dto.Presentation.PresentationAttestationReview>
                .GetByAction<List<Dto.Presentation.PresentationAttestationReview>>(DomainApiAction.Endpoint.Presentation.PresentationAttestation.GetAttestationReviews, parameter);
                        // e.DetailTableView.DataSource = DomainApiClient<Dto.Presentation.PresentationAttestationReview>
                        //.GetByAction<List<Dto.Presentation.PresentationAttestationReview>>("GetRejectedAttestationReviews", parameter);


                        break;
                    }
                case "UserEndPointSystemDetails":
                    {
                        GridDataItem parentItem = e.DetailTableView.ParentItem;
                        long ReviewId = Convert.ToInt64(parentItem.GetDataKeyValue("ReviewId"));

                        Dictionary<String, string> parameter = new Dictionary<String, string>();
                        parameter.Add("ReviewId", ReviewId.ToString());
                        e.DetailTableView.DataSource = DomainApiClient<Dto.Presentation.PresentationAttestationReviewEndpointSystem>
                .GetByAction<List<Dto.Presentation.PresentationAttestationReviewEndpointSystem>>(DomainApiAction.Endpoint.Presentation.PresentationAttestation.GetAttestationReviewEndpointSystems, parameter);
                       // e.DetailTableView.DataSource = DomainApiClient<Dto.Presentation.PresentationAttestationReviewEndpointSystem>
                       //.GetByAction<List<Dto.Presentation.PresentationAttestationReviewEndpointSystem>>("GetRejectedAttestationReviewEndpointSystems", parameter);


                        break;
                    }
                case "UserEndPointAccessDetails":
                    {
                        GridDataItem parentItem = e.DetailTableView.ParentItem;
                        long ReviewEndpointId = Convert.ToInt64(parentItem.GetDataKeyValue("ReviewEndpointId"));

                        Dictionary<String, string> parameter = new Dictionary<String, string>();
                        parameter.Add("ReviewEndpointId", ReviewEndpointId.ToString());
                        e.DetailTableView.DataSource = DomainApiClient<Dto.Presentation.PresentationAttestationReviewEndpointSystemL2Detail>
                .GetByAction<List<Dto.Presentation.PresentationAttestationReviewEndpointSystemL2Detail>>(DomainApiAction.Endpoint.Presentation.PresentationAttestation.GetAttestationReviewEndpointAccessDetails, parameter);
                        break;
                    }
            }
        }

        protected void SubmittedReviews_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            SubmittedReviews.DataSource = DomainApiClient<Dto.Presentation.PresentationAttestation>
                .GetByAction<List<Dto.Presentation.PresentationAttestation>>(DomainApiAction.Endpoint.Presentation.PresentationAttestation.GetAttestations, null);
        }
    }
}