using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DBConfigHelper;
using Newtonsoft.Json;
using Application;

namespace DI.Chakra.Web.NACR.Create.App
{
    public partial class RequestItemUC : UIBaseUC
    {
        public event EventHandler ItemClicked;
        private const string V_REQUESTITEM = "V_REQUESTITEM";
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();

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

        public string EndpointNeedsSplAccount
        {
            get
            {
                if (string.IsNullOrEmpty(sessionStatus.EndpointNeedsSplAccount))
                    sessionStatus.EndpointNeedsSplAccount = "";// MasterConfigHelper.EndpointNeedSplAccounts;
                return (string)sessionStatus.EndpointNeedsSplAccount;
            }
            set
            {
                sessionStatus.EndpointNeedsSplAccount = "";// MasterConfigHelper.EndpointNeedSplAccounts;
            }
        }

        public void Initialize()
        {
            AddCartUC.PresentationAccessRequestItem = PresentationAccessRequestItem;
            AddCartUC.Initialize();

            if (true)
            {
                if (PresentationAccessRequestItem.RemoveAccess)
                    AddCartSpeicalAcUC.Visible = false;
                //TODO: Need to implement for other system types.
                else
                {
                    if (!string.IsNullOrEmpty(EndpointNeedsSplAccount))
                    {
                        var endpointNeedsSplAccount = EndpointNeedsSplAccount.Split(',');
                        if (endpointNeedsSplAccount.Contains(((int)PresentationAccessRequestItem.EndpointType).ToString()))
                            AddCartSpeicalAcUC.Visible = true;
                        else
                            AddCartSpeicalAcUC.Visible = false;
                    }
                    else AddCartSpeicalAcUC.Visible = false;
                }

                AddCartSpeicalAcUC.PresentationAccessRequestItem = PresentationAccessRequestItem;
                AddCartSpeicalAcUC.Initialize();
            }
        }

        protected void AddCartUC_ItemClicked(object sender, EventArgs e)
        {
            PresentationAccessRequestItem requestedItem = AddCartUC.PresentationAccessRequestItem;

         
            this.PresentationAccessRequestItem = requestedItem;
            if (ItemClicked != null)
                ItemClicked(requestedItem, e);


        }

        protected void AddCartSpecialAcUC_ItemClicked(object sender, EventArgs e)
        {
            PresentationAccessRequestItem requestedItem = AddCartSpeicalAcUC.PresentationAccessRequestItem;
            this.PresentationAccessRequestItem = requestedItem;

            if (ItemClicked != null)
                ItemClicked(requestedItem, e);
        }
    }
}