using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.UsrControl
{
    public partial class RequestRiskUC : System.Web.UI.UserControl
    {
        private const string V_REQUESTID = "V_REQUESTID";
        private const string V_HASRISK = "V_HASRISK";
        private const string V_ISFROMACCESSREQUEST = "V_ISFROMACCESSREQUEST";
        private const string V_CANLOADACCESSREQITEMS = "V_CANLOADACCESSREQITEMS";
        private const string V_COMPLIANCEREPORT = "V_COMPLIANCEREPORT";
        private const string V_USERID = "V_USERID";
        private const string V_ENTITLEMENTCONFLICT = "V_ENTITLEMENTCONFLICT";
        public bool canLoadAccessReqItems
        {
            get
            {
                if (ViewState[V_CANLOADACCESSREQITEMS] == null)
                    return false;
                return (bool)ViewState[V_CANLOADACCESSREQITEMS];
            }
            set
            {
                ViewState[V_CANLOADACCESSREQITEMS] = value;

            }
        }

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
                if (UserId != value)
                ViewState[V_USERID] = value;
            }
        }

        public bool IsFromAccessRequest
        {
            get
            {
                if (ViewState[V_ISFROMACCESSREQUEST] == null)
                    return false;
                return (bool)ViewState[V_ISFROMACCESSREQUEST];
            }

            set
            {
                ViewState[V_ISFROMACCESSREQUEST] = value;
            }


        }

        public List<UserSODComplianceReport> complianceReport
        {
            get
            {
                if (ViewState[V_COMPLIANCEREPORT] == null)
                    return null;
                return (List<UserSODComplianceReport>)ViewState[V_COMPLIANCEREPORT];
            }
            set
            {
                ViewState[V_COMPLIANCEREPORT] = value;
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
        public long RequestId
        {
            get
            {
                if (ViewState[V_REQUESTID] == null)
                    return 0;
                return (long)ViewState[V_REQUESTID];

            }
            set
            {
                ViewState[V_REQUESTID] = value;
            }
        }

        public string EntitlementConflict
        {
            get
            {
                if (ViewState[V_ENTITLEMENTCONFLICT] == null)
                    return "";
                return (string)ViewState[V_ENTITLEMENTCONFLICT];

            }
            set
            {
                ViewState[V_ENTITLEMENTCONFLICT] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public void Initialize()
        {
            UserSODConflicts.RequestId = this.RequestId;
            UserSODConflicts.UserId = this.UserId;
            UserSODConflicts.canLoadAccessReqItems = canLoadAccessReqItems;
            UserSODConflicts.ShowNewConflictAlone = true;
            UserSODConflicts.EntitlementConflict = this.EntitlementConflict;
            var roleConflictDetails = new List<UserSODComplianceReport>();
            if (IsFromAccessRequest)
                UserSODConflicts.isFromAccessRequest = true;
            else
                UserSODConflicts.isFromAccessRequest = false;
            if (IsFromAccessRequest)
            {
                 roleConflictDetails = UserSODConflicts.UserSODAndSAConflicts;
                 
            }
            else
            {
              UserSODConflicts.LoadData();
                 roleConflictDetails = UserSODConflicts.report;
            }
            UserSODConflicts.BindConflicts();
            complianceReport = roleConflictDetails;

            lblHasConflicts.Text = "No";
            this.RiskInfo.Visible = false;


            bool isCurrentRequestHasConflict = IsUserHasSODConflictForCurrentRequest(roleConflictDetails);

            if (isCurrentRequestHasConflict)
            {
                this.HasRisk = true;
                lblHasConflicts.Text = "Yes";
                lblConflictsCount.Text = roleConflictDetails.Where(a => a.Type == "New" && !a.IsSensitiveRole).Count().ToString();
                lblSACount.Text = roleConflictDetails.Where(a => a.Type == "New" && a.IsSensitiveRole).Count().ToString();
                RiskInfo.Visible = true;
            }
           
        }
        bool IsUserHasSODConflictForCurrentRequest(List<UserSODComplianceReport> userSODComplianceReport)
        {
            if (userSODComplianceReport != null && userSODComplianceReport.Count > 0 && HasRisk)
            {
                var newConflicts = userSODComplianceReport.Where(a => a.Type == "New");
                if (newConflicts != null && newConflicts.Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }
        protected void MoreLinkButton_Click(object sender, EventArgs e)
        {

            SODConflictWindow.VisibleOnPageLoad = true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            SODConflictWindow.VisibleOnPageLoad = false;
        }
    }
}