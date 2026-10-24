using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Web;

namespace DI.Chakra.Web.Task
{
    public partial class DetailType : UIBasePage, IContextObjectRequired
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                if (TypeofRequest == Convert.ToString(DI.Chakra.Infrastructure.Enums.RequestType.Attestation) || TypeofRequest == "Access Review")
                {
                    var url = $"~/Admin/Attestation/AttestationUserComparision.aspx?SelectedtaskId={ContextObjectId}";
                    if (MasterConfigHelper.UseOldReviewPage)
                    {
                        string adminMode = "No";
                        NReviewUC.Visible = true;
                        url = $"~/Admin/Attestation/NReview.aspx?Id={ContextObjectId}&IsAdminMode={adminMode}";
                        if (Request.QueryString["CampaignId"] != null)
                        {
                            url = $"{url}&CampaignId={HttpUtility.UrlEncode(Request.QueryString["CampaignId"])}";
                        }
                        Response.Redirect(url);
                    }
                    else
                    {
                        Response.Redirect(url);
                    }
                }
                else if (TypeofRequest == DI.Chakra.Infrastructure.Constants.BulkRequest)
                {
                    BulkTaskDetail.CanLoad = true;
                    BulkTaskDetail.Visible = true;
                   // Response.Redirect("~/Task/BulkTask//BulkTaskDetail.aspx?Id=" + ContextObjectId);
                }
                else
                {
                    ActionTask.Visible = true;
                }
            }
        }
    }
}