using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.NACR.Create.App
{
    public class NewAccount
    {
        public string DisplayName { get; set; }
        public string AccountName { get; set; }
    }

    public partial class NewAccountUC : UIBaseUC
    {
        #region Memebers
        private const string V_REQUESTITEM = "V_REQUESTITEM";
        private const string V_ACCOUNTNAME = "V_ACCOUNTNAME";
        private const string V_DISPLAYNAME = "V_DISPLAYNAME";
        private const string V_EPUSERENABLED = "V_EPUSERENABLED";

        #endregion

        #region Properties
        public PresentationAccessRequestItem PresentationAccessRequestItem
        {
            get
            {
                if (ViewState[V_REQUESTITEM] == null)
                    return null;

                if(string.IsNullOrEmpty(AccountName) == false)
                {

                }

                return (PresentationAccessRequestItem)ViewState[V_REQUESTITEM];
            }
            set
            {
                ViewState[V_REQUESTITEM] = value;
            }
        }

        public string AccountName
        {
            get
            {
                if (ViewState[V_ACCOUNTNAME] == null)
                    return null;
                

                return (string)ViewState[V_ACCOUNTNAME];
            }
            set
            {
                ViewState[V_ACCOUNTNAME] = value;
            }
        }
        public string DisplayName
        {
            get
            {
                if (ViewState[V_DISPLAYNAME] == null)
                    return null;


                return (string)ViewState[V_DISPLAYNAME];
            }
            set
            {
                ViewState[V_DISPLAYNAME] = value;
            }
        }
        public bool Enabled
        {
            get
            {
                if (ViewState[V_EPUSERENABLED] == null)
                    return true;


                return (bool)ViewState[V_EPUSERENABLED];
            }
            set
            {
                ViewState[V_EPUSERENABLED] = value;
            }
        }
        public dynamic GetDetailObject()
        {
            dynamic detail = new ExpandoObject();

            ExpandoHelper.AddProperty(detail, "AccountName", this.AccountName);
            ExpandoHelper.AddProperty(detail, "DisplayName", this.DisplayName);

            return detail;

        }

        #endregion
       
        #region Methods
        public void Initialize()
        {
            if (this.PresentationAccessRequestItem == null)
                return;

            ddlEndpointUser.Enabled = this.Enabled;


            BindEndpointUserName();
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
        public void BindEndpointUserName()
        {
            ddlEndpointUser.Items.Clear();

            var interimlist = GetFromInterim(this.PresentationAccessRequestItem.RequestFor);

            var acListFromInterim = interimlist.Where(a => a.AccountType == this.PresentationAccessRequestItem.AccountType 
            && a.AdhocActionType == AdHocActionType.NewAccount).ToList();
            
            var ddlList = acListFromInterim.Select(a => new NewAccount
            {
                AccountName = a.RequestedItems.FirstOrDefault().AdditionalInfoObj.AccountName,
                DisplayName = a.RequestedItems.FirstOrDefault().AdditionalInfoObj.DisplayName
            });
            //TODO: need to bring account names from Access Request pending list.

            ddlEndpointUser.DataSource = ddlList;
            ddlEndpointUser.DataBind();
            ddlEndpointUser.Items.Insert(0, "--Select--");

           
        }
        #endregion

        protected void ddlEndpointUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.AccountName = ddlEndpointUser.SelectedItem.Value;
            this.DisplayName = ddlEndpointUser.SelectedItem.Text;
        }
    }
}