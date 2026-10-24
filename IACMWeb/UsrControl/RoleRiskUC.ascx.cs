using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.UsrControl
{
    public partial class RoleRiskUC : System.Web.UI.UserControl
    {
        private const string V_USERID = "V_USERID";
        private const string V_ENTITLEMENT = "V_ENTITLEMENT";
        private const string V_ROLEID = "V_ROLEID";
        private const string V_HASRISK = "V_HASRISK";
        private const string V_SHOWLINK = "V_SHOWLINK";

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
        public string Role
        {
            get
            {
                if (ViewState[V_ROLEID] == null)
                    return null;
                return (string)ViewState[V_ROLEID];

            }
            set
            {
                ViewState[V_ROLEID] = value;
            }
        }
        public PresentationUserEntitlement PresentationUserEntitlement
        {
            get
            {
                if (ViewState[V_ENTITLEMENT] == null)
                    return null;
                return (PresentationUserEntitlement)ViewState[V_ENTITLEMENT];

            }
            set
            {
                ViewState[V_ENTITLEMENT] = value;
            }
        }
        public bool HasRisk
        {
            get
            {
                if (ViewState[V_HASRISK] == null)
                    return false;
                return (bool)ViewState[V_HASRISK];

            }
            set
            {
                ViewState[V_HASRISK] = value;
            }
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
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public void Initialize()
        {
            UserSODConflict.UserId = UserId;
            UserSODConflict.EntitlementConflict = Role;
            UserSODConflict.LoadData();
            ShowWarningIcon.Visible = UserSODConflict.UserSODAndSAConflicts.Count > 0;
            HasRisk = UserSODConflict.UserSODAndSAConflicts.Count > 0;
            ShowWarningIcon.Visible = ShowLink;
        }
        protected void btnSensitiveRoleCancel_Click(object sender, EventArgs e)
        {
            SensitiveRoleDetails.VisibleOnPageLoad = false;
        }

        protected void ShowWarningIcon_Click(object sender, ImageClickEventArgs e)
        {
            UserSODConflict.UserId = UserId;
            UserSODConflict.EntitlementConflict = Role;
            UserSODConflict.BindConflicts();
            SensitiveRoleDetails.VisibleOnPageLoad = true;

        }
    }
}