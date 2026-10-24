using Application;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Web.NACR.Create.App
{
    public partial class CartInfo : UIBaseUC
    {
        private const string V_TOTALITEMS = "V_TOTALITEMS";
        private SessionStatusImpl Session = new SessionStatusImpl();
        public event EventHandler EditCartItemClicked;
        public Wizard.WizardCartUC wizardUC;
        public int TotalItems
        {
            get
            {
                if (ViewState[V_TOTALITEMS] == null)
                    return 0;
                return (int)ViewState[V_TOTALITEMS];
            }
            set
            {
                ViewState[V_TOTALITEMS] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                UpdateCart.Update();
            }
           Refresh();
        }

        private void OnUserControlButtonClick(object sender, EventArgs e)
        {
            EditCartItemClicked?.Invoke(this, e);
        }

        public void Refresh()
        {
            this.BindTotalRequests();
        }

        private void BindTotalRequests()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId",Convert.ToString(Session.User.Id));
            parameter.Add("isSaved", true.ToString());
            parameter.Add("itemType", InterimStoreItemType.AccessRequest.ToString());

            IList<Dto.InterimStore> interimList = DomainApiClient<Dto.InterimStore>.GetByAction<List<Dto.InterimStore>>
                (DomainApiAction.Tasks.InterimStore.GetByUserId, parameter);
            
            this.TotalItems = interimList.Count;
            lnkEditCart.Visible = interimList.Count > 0;
            lblTotalReq.Text = this.TotalItems.ToString();
        }

        protected void OnServerClick(object sender, EventArgs e)
        {
            if (TotalItems != 0)
            {
                CartItems.Show();
            }
            else
            {
                base.MasterPage.ShowAlert(AlertType.Warning, "No Items in Cart");
            }
        }

        protected void lnkEditCart_Click(object sender, EventArgs e)
        {
            OnUserControlButtonClick(sender, e);
        }
    }
}