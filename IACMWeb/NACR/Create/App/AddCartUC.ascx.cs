using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.NACR.Create.App
{
  
    public partial class AddCartUC :  UIBaseUC
    {
        private const string V_REQUESTITEM = "V_REQUESTITEM";
        public event EventHandler ItemClicked;

        

        public PresentationAccessRequestItem PresentationAccessRequestItem
        {
            get
            {
                if (ViewState[V_REQUESTITEM] == null)
                    return null;
                return (PresentationAccessRequestItem)ViewState[V_REQUESTITEM];
            }
            set
            {
                ViewState[V_REQUESTITEM] = value;
            }
        }


        public void Initialize()
        {
            if (this.PresentationAccessRequestItem.RemoveAccess)
            {
                lnkAddItem.Text = "Remove Item";

                dynamic detail = new ExpandoObject();

                detail = ExpandoHelper.AddProperty(detail, "AccountName", this.PresentationAccessRequestItem.EndpointAccountName);
                string detailString = JsonConvert.SerializeObject(detail);

                this.PresentationAccessRequestItem.AdditionalInfo = detailString;

                if (this.PresentationAccessRequestItem.IsMockEndpoint)
                {
                    detail = new ExpandoObject();
                    
                    ExpandoHelper.AddProperty(detail, "SequenceNumber", this.PresentationAccessRequestItem.ItemName);
                    ExpandoHelper.AddProperty(detail, "SubsystemName", this.PresentationAccessRequestItem.ItemDetail);
                    ExpandoHelper.AddProperty(detail, "Description", this.PresentationAccessRequestItem.ItemDescription);
                    ExpandoHelper.AddProperty(detail, "ReferenceAccessRequetId", this.PresentationAccessRequestItem.KeyId);

                    detailString = JsonConvert.SerializeObject(detail);
                    this.PresentationAccessRequestItem.AdditionalInfo = detailString;
                }
            }
            if (this.PresentationAccessRequestItem.Enabled && this.PresentationAccessRequestItem.AccessRequestItemType == AccessRequestItemType.Application)
            {
                lnkAddItem.Enabled = true; //!isOneWayTrust(this.PresentationAccessRequestItem.EndpointSystemId);
            }
            else
            {
                lnkAddItem.Enabled = this.PresentationAccessRequestItem.Enabled;
                if (!this.PresentationAccessRequestItem.Enabled)
                {
                    lnkAddItem.Text = "Existing";
                    if (this.PresentationAccessRequestItem.RemoveAccess)
                    {
                        lnkAddItem.Text = "Removed";
                    }
                    lnkAddItem.CssClass = "added-item-text";
                    //lnkAddItem.ForeColor = System.Drawing.ColorTranslator.FromHtml("#666");
                }
            }
        }

        public bool isOneWayTrust(long endpointSystemId)
        {
            bool result = false;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("key", "IsOneWayTrust");
            parameter.Add("value", Boolean.TrueString);
            List<Dto.EndPoint> isOneWayTrustEps = DomainApiClient<Dto.EndPoint>.GetByAction<List<Dto.EndPoint>>
                (DomainApiAction.Endpoint.EndPoint.GetByEndpointConfiguration, parameter);
            if (isOneWayTrustEps != null && isOneWayTrustEps.Count>0)
                if (isOneWayTrustEps.Any(x => x.Id == endpointSystemId))
                    result = true;
            return result;
        }

        protected void lnkAddItem_Click(object sender, EventArgs e)
        {
          
            if (this.PresentationAccessRequestItem.IsMockEndpoint && !this.PresentationAccessRequestItem.RemoveAccess)
            {
                StandingOrder.ShowEndpoint = true;
                StandingOrder.Initilize();
                titleID.InnerText = this.PresentationAccessRequestItem.EndpointName;
                SOFormWindow.VisibleOnPageLoad = true;
            }            
            else
            {
                if (ItemClicked != null)
                    ItemClicked(sender, e);
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            try
            {
                string detailString = string.Empty;
                string message = string.Empty;
                bool valid = StandingOrder.Validate(out message);

                if (!valid)
                {
                    MasterPage.ShowAlert(AlertType.Error, message);
                    return;
                }

                detailString = StandingOrder.GetDetail(this.PresentationAccessRequestItem.EndpointName, this.PresentationAccessRequestItem.EndpointSystemId,
                    this.PresentationAccessRequestItem.EndpointCode);

                this.PresentationAccessRequestItem.AdditionalInfo = detailString;
                SOFormWindow.VisibleOnPageLoad = false;

                dynamic detail = JsonConvert.DeserializeObject(detailString);

                if (detail.SubsystemName == null)
                    return;


                if (ItemClicked != null)
                    ItemClicked(sender, e);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Ok.Enabled = true;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            SOFormWindow.VisibleOnPageLoad = false;
        }
    }
}