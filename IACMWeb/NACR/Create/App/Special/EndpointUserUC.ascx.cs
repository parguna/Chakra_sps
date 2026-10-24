using System;
using System.Linq;

using DI.Chakra.Infrastructure.Enums;

using System.Collections.Generic;
using DI.Chakra.Dto.Presentation;
using System.Dynamic;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Web.NACR.Create.App.Special
{
    public partial class EndpointUserUC : UIBaseUC
    {
        #region Memebers
        private const string V_REQUESTITEM = "V_REQUESTITEM";
        private const string V_ENDPOINTUSERID = "V_ENDPOINTUSERID";
        private const string V_EPUSERENABLED = "V_EPUSERENABLED";
        private const string V_ACCOUNTNAME = "V_ACCOUNTNAME";

        #endregion

        #region Properties
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

        public long EndpointUserID
        {
            get
            {
                if (ViewState[V_ENDPOINTUSERID] == null)
                    return -1;


                return (long)ViewState[V_ENDPOINTUSERID];
            }
            set
            {
                ViewState[V_ENDPOINTUSERID] = value;
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

        #endregion


        #region Methods
        public void Initialize()
        {
            ddlEndpointUser.Enabled = this.Enabled;

            BindEndpointUserName();
        }
        public dynamic GetDetailObject()
        {
            dynamic detail = new ExpandoObject();

            ExpandoHelper.AddProperty(detail, "AccountName", this.AccountName);

            return detail;

        }
        public void BindEndpointUserName()
        {
            if (this.PresentationAccessRequestItem == null)
                return;



            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", this.PresentationAccessRequestItem.RequestFor.ToString());
            parameter.Add("accountType", this.PresentationAccessRequestItem.AccountType.ToString());
            ddlEndpointUser.Items.Clear();
            var list = DomainApiClient<Dto.Presentation.PresentationUserAccount>.GetByAction<List<Dto.Presentation.PresentationUserAccount>>(DomainApiAction.Endpoint.Presentation.PresentationUserAccount.GetAccountDetailByAccountType, parameter);
            ddlEndpointUser.DataSource = list;
            ddlEndpointUser.DataBind();
            ddlEndpointUser.Items.Insert(0, "--Select--");
        }

        protected void ddlEndpointUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.EndpointUserID = Convert.ToInt64(ddlEndpointUser.SelectedItem.Value);
            this.AccountName = ddlEndpointUser.SelectedItem.Text;
        }
        #endregion
    }
}