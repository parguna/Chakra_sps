using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.NACR.Create.App.Special
{
    public partial class SpecialAccountWarning : System.Web.UI.UserControl
    {
        public AdHocAccessRequest AccessRequest
        {
            set
            {
                ViewState["SAPWarningMsgAdhocAccessRequest"] = value;
            }
            get
            {
                if(ViewState["SAPWarningMsgAdhocAccessRequest"] != null)
                    return ViewState["SAPWarningMsgAdhocAccessRequest"] as AdHocAccessRequest;
                return null;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        public void ShowWarningMessage(AdHocAccessRequest AccessRequest)
        {
            lblWarningMessage.Text = "";
            this.AccessRequest = AccessRequest;
            if (AccessRequest != null && AccessRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.NewAccount && (AccessRequest.AccountType == Infrastructure.Enums.AccountType.Service || AccessRequest.AccountType == Infrastructure.Enums.AccountType.Shared))
            {
                lblWarningMessage.Text = MasterConfigHelper.SAPSpecialAccountNameWarningMessage;
            }
            lblWarningMessage.Visible = !string.IsNullOrWhiteSpace(lblWarningMessage.Text);
        }
    }
}