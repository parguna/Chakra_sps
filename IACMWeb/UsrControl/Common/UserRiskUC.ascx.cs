using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class UserRiskUC : System.Web.UI.UserControl
    {
        private const string V_USERID = "V_USERID";
        private const string V_SHOWLINK = "V_SHOWLINK";
        private const string V_ISFROMACCESSREQUEST = "V_ISFROMACCESSREQUEST";
        public long UserId
        {
            get
            {
                if (ViewState[V_USERID] == null)
                    return 0;
                return (long)ViewState[V_USERID];

            }
            set
            {
                ViewState[V_USERID] = value;
            }
        }

        public bool isFromAccessRequest
        {
            get
            {
                if (ViewState[V_ISFROMACCESSREQUEST] == null)
                    return false;
                return (bool)ViewState[V_ISFROMACCESSREQUEST];
            }
            set {
                ViewState[V_ISFROMACCESSREQUEST] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public void Initialize()
        {
            UserSODConflict.UserId = UserId;
            UserSODConflict.EntitlementConflict = string.Empty;
            
            if (isFromAccessRequest)
            {
                UserSODConflict.BindConflicts();
                lnkShowRisk.Visible = UserSODConflict.UserSODAndSAConflicts.Count > 0;
                lnkShowRisk.Visible = ShowLink;
            }
            else
            {
                UserSODConflict.isFromAccessRequest = false;
                UserSODConflict.LoadData();
                lnkShowRisk.Visible = UserSODConflict.canShowLink;
            }
        }
        protected void btnSensitiveRoleCancel_Click(object sender, EventArgs e)
        {
            SensitiveRoleDetails.VisibleOnPageLoad = false;
        }

        public bool ShowLink
        {
            get
            {
                if (ViewState[V_SHOWLINK] == null)
                    return true;
                return (bool)ViewState[V_SHOWLINK];
            }
            set
            {
                ViewState[V_SHOWLINK] = value;
            }
        }

        protected void lnkShowRisk_Click(object sender, EventArgs e)
        {
            UserSODConflict.UserId = UserId;
            UserSODConflict.EntitlementConflict = string.Empty;
            UserSODConflict.isFromAccessRequest = isFromAccessRequest;
            UserSODConflict.BindConflicts();
            SensitiveRoleDetails.VisibleOnPageLoad = true;
        }
    }
}