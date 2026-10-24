using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.NACR.Create.Wizard
{
    public partial class WizardBulkAdhocAccessRequest : UIBaseUC
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BulkAdhocAccessRequestUC.Initialize();
                if (Request.QueryString["BulkRequestId"] != null)
                {
                    long requestId = !string.IsNullOrEmpty(Request.QueryString["BulkRequestId"]) ? Convert.ToInt32(Request.QueryString["BulkRequestId"]) : 0;
                    if (requestId > 0)
                    {
                        Dto.BulkRequest bulkRequest = DomainApiClient<Dto.BulkRequest>.GetById(requestId, true);
                        BulkAdhocAccessRequestUC.BindAdhocRequest(bulkRequest);
                        RadWizardStep2.Active = true;
                        ApplicationBind(RadWizard, new Telerik.Web.UI.WizardEventArgs(RadWizardStep1, RadWizardStep2));
                    }
                }

            }
        }


        protected void ApplicationBind(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            if (e.NextStepIndex == 1)
            {
                if (e.CurrentStepIndex < e.NextStepIndex)
                {
                    BulkAdhocAccessRequestUC.Initialize();
                    if (!BulkAdhocAccessRequestUC.Validate())
                    {
                        RadWizardStep1.Active = true;
                        return;
                    }
                    bool isvalid = true;
                    Dto.BulkRequest bulkRequest = this.BulkAdhocAccessRequestUC.ValidateBulkRequest(out isvalid);
                    if (!isvalid)
                    {
                        RadWizardStep1.Active = true;
                        return;
                    }

                    Session["endpointSystemId"] = bulkRequest.EndpointSystemId;
                    BulkAccessUC.BulkAccessRequest = bulkRequest;
                    BulkAccessUC.Initialize();

                }
            }
        }




        private bool IsValidToProcess()
        {
            //if (TotalItems == 0)
            //{
            //    RadWizardStep2.Active = true;
            //    ShowAlertMessage(AlertType.Warning, "Please Add at least one item into the cart");
            //    return false;
            //}
            //else if (this.AdhocBulkAccessUC.AdHocAccessRequest != null && this.AdhocBulkAccessUC.AdHocAccessRequest.ValidFrom != null && this.AdhocBulkAccessUC.AdHocAccessRequest.ValidTo != null)
            //{
            //    if (this.AdhocBulkAccessUC.AdHocAccessRequest.ValidTo < this.AdhocBulkAccessUC.AdHocAccessRequest.ValidFrom)
            //    {
            //        ShowAlertMessage(AlertType.Warning, "Valid To date should not be less than Valid From date");
            //        return false;
            //    }
            //}

            return true;
        }
        protected void RadWizard_FinishButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            if (IsValidToProcess() == false)
                return;

            if (!BulkAccessUC.SaveBulkRequest())
                ApplicationBind(RadWizard, new Telerik.Web.UI.WizardEventArgs(RadWizardStep1, RadWizardStep2));
        }

        protected void RadWizard_CancelButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }



    }
}