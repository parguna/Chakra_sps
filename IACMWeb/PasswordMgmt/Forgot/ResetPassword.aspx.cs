using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Dto;
using System.Configuration;
using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DBConfigHelper;
using Telerik.Web.UI;
using System.Security.Claims;

namespace DI.Chakra.Web.PasswordMgmt.Forgot
{
    public partial class ResetPassword : DI.Chakra.Web.FrontUIPage
    {
        SessionStatusImpl session = new SessionStatusImpl();
        string username;

        protected void Page_Load(object sender, EventArgs e)
        {
            //var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
            //if(claimsIdentity != null && claimsIdentity.IsAuthenticated)
            //{
            //    //claimsIdentity.Claims.FirstOrDefault(i => i.Type == "IDMSUserName")
            //}
            if (session.User != null && session.User.AuthenticationMode == "AD")
                DisplayPolicies.Visible = true;
            else
                DisplayPolicies.Visible = false;

            //if (PreviousPage == null && !IsPostBack)
            //{
                Response.Redirect("~/Login.aspx");
            //}
            if (session.User != null)
                username = session.User.UserName;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string message = string.Empty;

            LoginHelper loginHelper = new LoginHelper(username, txtPassword.Text);

            if (txtPassword.Text == txtConfirmPassword.Text)
            {
                bool bRet = loginHelper.ResetPassword(out message);

                if (bRet == false)
                {
                    MasterPage.ShowAlert(AlertType.Error, message);
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Success, "Password Reset Successfully Done");

                    Response.Redirect("~/Login.aspx");
                }
            }
            else
            {
                    MasterPage.ShowAlert(AlertType.Error, "Password and ConfirmPassword Do Not Match - Please re enter.");
            }

        }

        protected void DisplayPolicies_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("endpointSystemTypeId", 1.ToString());
            para.Add("isActive", true.ToString());
            var ep = DomainApiClient<EndPoint>.GetByAction<EndPoint>(DomainApiAction.Endpoint.EndPoint.GetByEndpointSystemTypeIdIsActive, para);
            EnterprisePasswordPolicy.EndPointSystemID = ep.Id;
            EnterprisePasswordPolicy.BindPolicy();
            DisplayPolicy.VisibleOnPageLoad = true;

        }

        protected void ok_Click(object sender, EventArgs e)
        {
            DisplayPolicy.VisibleOnPageLoad = false;
        }
    }
}