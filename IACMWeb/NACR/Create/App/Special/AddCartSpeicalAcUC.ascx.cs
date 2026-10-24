using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.NACR.Create.App
{
    public class AddOptions
    {
        public string DisplayName { get; set; }
        public string CommandName { get; set; }
    }
    public partial class AddCartSpeicalAcUC : UIBaseUC
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
            List<AddOptions> liOptions = new List<AddOptions>();
            if (this.PresentationAccessRequestItem.RemoveAccess == false)
            {
                liOptions = new List<AddOptions> {
                new AddOptions() { DisplayName="Add as Service", CommandName ="AddService" },
                new AddOptions() { DisplayName="Add as Admin", CommandName ="AddAdmin" },
                new AddOptions() { DisplayName="Add as Shared", CommandName ="AddShared" },
                //new AddOptions() { DisplayName="Add as Secondary", CommandName ="AddSecondary"}
                };
            }

            if (liOptions.Count <= 0)
            {
                liDropDown.Visible = false;
            }
            RequestRepeater.DataSource = liOptions;
            RequestRepeater.DataBind();

            //TODO: to be moved 
            //EntitlementSpecialAccountUC.PresentationAccessRequestItem = this.PresentationAccessRequestItem;
            //EntitlementSpecialAccountUC.Initialize();
        }

        public void DisableAll()
        {
            ADAccountInfoUC.Visible = false;
            EntitlementSpecialAccountUC.Visible = false;
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            AccountFormWindow.VisibleOnPageLoad = false;
        }

        protected void lnkAddItem_Click(object sender, EventArgs e)
        {

            DisableAll();

            Button lnkBtn = sender as Button;

            this.PresentationAccessRequestItem.ACRCategory = ACRCategory.SpecialAccessRequest;

            if (lnkBtn.CommandName == "AddService")
                this.PresentationAccessRequestItem.AccountType = Infrastructure.Enums.AccountType.Service;
            else if (lnkBtn.CommandName == "AddShared")
                this.PresentationAccessRequestItem.AccountType = Infrastructure.Enums.AccountType.Shared;
            else if (lnkBtn.CommandName == "AddAdmin")
                this.PresentationAccessRequestItem.AccountType = Infrastructure.Enums.AccountType.Secondary;

            AccountFormWindow.VisibleOnPageLoad = true;

            if (this.PresentationAccessRequestItem.AccessRequestItemType == Infrastructure.AccessRequestItemType.Entitlement)
            {
                EntitlementSpecialAccountUC.PresentationAccessRequestItem = this.PresentationAccessRequestItem;
                EntitlementSpecialAccountUC.Initialize();
                EntitlementSpecialAccountUC.Visible = true;

            }
            else
            {
                headerText.InnerText = string.Format("Account Detail - {0}", PresentationAccessRequestItem.AccountType.ToString());
                ADAccountInfoUC.EndPointID = this.PresentationAccessRequestItem.EndpointSystemId;
                ADAccountInfoUC.RequestedAccountType = this.PresentationAccessRequestItem.AccountType;
                ADAccountInfoUC.Initialize();
                ADAccountInfoUC.Visible = true;
            }

        }

        protected void Ok_Click(object sender, EventArgs e)
        {

            string detailString = string.Empty;

            if (this.PresentationAccessRequestItem.AccessRequestItemType == Infrastructure.AccessRequestItemType.Entitlement)
            {
                detailString = EntitlementSpecialAccountUC.GetDetail();

                this.PresentationAccessRequestItem = EntitlementSpecialAccountUC.PresentationAccessRequestItem;

                this.PresentationAccessRequestItem.AdditionalInfo = detailString;

            }
            else
            {
                string errorMessage = "";
                var canSave = ADAccountInfoUC.CheckValidation(out errorMessage, this.PresentationAccessRequestItem.EndpointSystemTypeId, this.PresentationAccessRequestItem.AccountType);
                if (!canSave)
                {
                    if (string.IsNullOrEmpty(errorMessage))
                        MasterPage.ShowAlert(AlertType.Warning, "Please Enter all values");
                    else
                        MasterPage.ShowAlert(AlertType.Warning, errorMessage);

                    return;
                }
                detailString = ADAccountInfoUC.GetDetail();

                this.PresentationAccessRequestItem.AdditionalInfo = detailString;
            }
            AccountFormWindow.VisibleOnPageLoad = false;

            dynamic detail = JsonConvert.DeserializeObject(detailString);

            if (detail.AccountName == null)
                return;


            if (ItemClicked != null)
                ItemClicked(sender, e);

        }


        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}