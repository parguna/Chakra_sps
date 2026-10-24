using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using Newtonsoft.Json;
namespace DI.Chakra.Web.UsrControl.Common
{
    public class AdditionalEntitlementDetail
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public partial class AdditionalEntitlementDetailUC : UIBaseUC
    {
        public event EventHandler ItemClicked;
        private const string V_REQUESTITEM = "V_REQUESTITEM";
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
            if (!string.IsNullOrEmpty(PresentationAccessRequestItem.AdditionalInfo))
            {

                this.lnkAdditionalInfo.Visible = true;
                this.lnkAdditionalInfo.Text = "Account Info";
                this.lblAdditionInfoHeader.Text = "Account Info";
                dynamic detail = JsonConvert.DeserializeObject(PresentationAccessRequestItem.AdditionalInfo);

                List<AdditionalEntitlementDetail> li = new List<AdditionalEntitlementDetail>();

                foreach (dynamic entry in detail)
                {
                    string name = entry.Name;
                    dynamic value = entry.Value;

                    li.Add(new AdditionalEntitlementDetail() { Name = name, Value = value });
                }


                AdditionalInfoRepeater.DataSource = li;
                AdditionalInfoRepeater.DataBind();
            }
            else if(PresentationAccessRequestItem.AccessItemType == Infrastructure.Enums.AccessItemType.Catalog)
            {
                if (!string.IsNullOrEmpty(PresentationAccessRequestItem.ItemDetail))
                {
                    this.lnkAdditionalInfo.Visible = true;
                    this.lnkAdditionalInfo.Text = "Catalog Info";
                    this.lblAdditionInfoHeader.Text = "Catalog Info";

                    List<AdditionalEntitlementDetail> li = new List<AdditionalEntitlementDetail>();
                    var details = PresentationAccessRequestItem.ItemDescription.Split(new string[] { ",<br />" }, StringSplitOptions.None);
                    foreach (string entry in details)
                    {
                        string name = "Entitlement";
                        dynamic value = entry;

                        li.Add(new AdditionalEntitlementDetail() { Name = name, Value = value });
                    }

                    AdditionalInfoRepeater.DataSource = li;
                    AdditionalInfoRepeater.DataBind();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void lnkAdditionalInfo_Click(object sender, EventArgs e)
        {
            AdditionalInfoWindow.VisibleOnPageLoad = true;
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            AdditionalInfoWindow.VisibleOnPageLoad = false;
        }
    }
}