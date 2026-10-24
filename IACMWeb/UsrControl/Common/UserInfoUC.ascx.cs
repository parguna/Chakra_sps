
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace DI.Chakra.Web.AccessRequest.Common
{
    public partial class UserInfoUC : DI.Chakra.Web.UIBaseUC
    {
        public long UserId
        {
            get
            {
                if(ViewState["VWUserId"] != null)
                {
                    return Convert.ToInt64(ViewState["VWUserId"].ToString());
                }
                return 0;
            }
            set
            {
                ViewState["VWUserId"] = value;
            }
        }
        private Dto.User _mUser = null;
        public Dto.User User
        {
            set
            {
                _mUser = value;
                SetValues(_mUser);
            }
        }

        

        private void SetValues(Dto.User User)
        {
            if (User == null)
                return;

            LinkButtonRequestRequestorName.Text = User.DisplayName;

            lblUserName.InnerText = User.DisplayName;
            UserId = User.Id;
            UserDetailUC.User = User;

        }

        protected void LinkButtonRequestRequestorName_Click(object sender, EventArgs e)
        {
            if(UserId > 0)
            {
                UserDetailUC.User = DomainApiClient<Dto.User>.GetById(UserId);
               
            }

            string script = "function f(){$find(\"" + UserInfo.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
        }
    }
}