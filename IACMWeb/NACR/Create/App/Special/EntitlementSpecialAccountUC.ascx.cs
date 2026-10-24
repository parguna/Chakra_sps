using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
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
    public partial class EntitlementSpecialAccountUC : UIBaseUC
    {
        private const string V_REQUESTITEM = "V_REQUESTITEM";
        private const string V_ADDTONEWACCOUNT = "V_ADDTONEWACCOUNT";

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
      
        private List<AdHocAccessRequest> GetFromInterim(long UserID)
        {
            AdHocAccessRequest adhocaccessrequest = new AdHocAccessRequest();
            List<AdHocAccessRequest> listadhoc = new List<AdHocAccessRequest>();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("itemType", InterimStoreItemType.AccessRequest.ToString());
            IList<InterimStore> listinterim = DomainApiClient<Dto.InterimStore>.GetByAction<List<Dto.InterimStore>>(DomainApiAction.Tasks.InterimStore.GetByType, parameter);
            if (listinterim != null)
            {
                for (int i = 0; i < listinterim.Count; i++)
                {
                    adhocaccessrequest = Infrastructure.UtilityMethods.Deserialize<AdHocAccessRequest>(listinterim[i].XmlData);
                    adhocaccessrequest.Id = listinterim[i].Id;
                    if (adhocaccessrequest.RequesteeUserId == UserID)
                        listadhoc.Add(adhocaccessrequest);
                }
            }
            return listadhoc;
        }
     
        public void Initialize()
        {
            this.EndpointUserUC.PresentationAccessRequestItem = this.PresentationAccessRequestItem;
            this.NewAccountUC.PresentationAccessRequestItem = this.PresentationAccessRequestItem;

            this.EndpointUserUC.Initialize();
            this.NewAccountUC.Initialize();
        }
        

        public string GetDetail()
        {
            if (newAc.Checked)
            {
                dynamic detail = NewAccountUC.GetDetailObject();
                return JsonConvert.SerializeObject(detail);
            }
            else if(existingAc.Checked)
            {
                this.PresentationAccessRequestItem.EndpointUserId = EndpointUserUC.EndpointUserID;

                dynamic detail = EndpointUserUC.GetDetailObject();

                return JsonConvert.SerializeObject(detail);
            }
            return string.Empty;
        }
        private void DisableAccountUserControl()
        {
            this.EndpointUserUC.Enabled = false;
            this.NewAccountUC.Enabled = false;

            this.EndpointUserUC.Initialize();
            this.NewAccountUC.Initialize();
        }

        protected void newAc_CheckedChanged(object sender, EventArgs e)
        {
            if (newAc.Checked)
            {
                DisableAccountUserControl();
                this.NewAccountUC.Enabled = true;
                this.NewAccountUC.PresentationAccessRequestItem = this.PresentationAccessRequestItem;

                this.NewAccountUC.Initialize();
            }
        }

        protected void existingAc_CheckedChanged(object sender, EventArgs e)
        {
            if (existingAc.Checked)
            {
                DisableAccountUserControl();
                this.EndpointUserUC.Enabled = true;
                this.EndpointUserUC.PresentationAccessRequestItem = this.PresentationAccessRequestItem;
                this.EndpointUserUC.Initialize();
            }
        }
    }
}