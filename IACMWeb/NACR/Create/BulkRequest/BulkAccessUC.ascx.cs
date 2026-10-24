using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.NACR.Create.App.Step2UC;
using DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.NACR.Create.BulkRequest
{
    public partial class BulkAccessUC : UIBaseUC
    {
        private const string V_REQUESTINFO = "V_REQUESTINFO";
        public delegate void RefreshCartHandler();
        public event RefreshCartHandler SaveBulkStore;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public Dto.BulkRequest BulkAccessRequest
        {
            get
            {
                if (ViewState[V_REQUESTINFO] == null)
                    return null;
                return (Dto.BulkRequest)ViewState[V_REQUESTINFO];
            }
            set
            {
                ViewState[V_REQUESTINFO] = value;
            }
        }

        public void Initialize()
        {
            EntitlementVisibility();
        }

        void setInVisibility()
        {
            foreach (var control in pnlStep2Controls.Controls)
            {
                UIBaseStep2UC_B step2Control = control as UIBaseStep2UC_B;
                if (step2Control != null)
                    step2Control.Visible = false;
            }
        }

        void EntitlementVisibility()
        {
            Session["AdhocrequestObj"] = BulkAccessRequest;
            UIBaseStep2UC_B step2 = getSecondStep();
            if (step2 == null) return;
            step2.Visible = true;
            step2.AdhocRequest = BulkAccessRequest;
            step2.Initialize();
        }

        UIBaseStep2UC_B getSecondStep()
        {
            if (BulkAccessRequest == null) return null;
            setInVisibility();
            switch (BulkAccessRequest.ApplicationType)
            {
                case ApplicationType.ADSingaporePowerLocal:
                    return AdhocADStep2UC_B;
                case ApplicationType.HRMS_FLINS:
                    return AdhocSapFlinsStep2UC_B;
                case ApplicationType.FLINS_BI:
                    return AdhocSapFlinsBIStep2UC_B;
                case ApplicationType.SAP_PI:
                    return AdhocSapPiStep2UC_B;
                case ApplicationType.SAP_GRC:
                    return AdhocSapGrcStep2UC_B;
                case ApplicationType.MSSL_CRM:
                case ApplicationType.MSSL_ISU:
                    return AdhocSapMsslStep2UC_B;
                case ApplicationType.NPAC:
                    return AdhocNpacStep2UC_B;
                case ApplicationType.GEMS:
                    return AdhocGemsStep2UC_B;
                case ApplicationType.EBS_CRM:
                case ApplicationType.EBS_ISU:
                    return AdhocSapEbsStep2UC_B;
                case ApplicationType.Office365:
                    return AdhocOffice365Step2UC_B;
                case ApplicationType.SAP_EP:
                case ApplicationType.CE:
                    return AdhocSapEpCeStep2UC_B;
                case ApplicationType.OneCard:
                    return AdhocOneCardStep2UC_B;
                case ApplicationType.InhouseSystems:
                    return AdhocInHouseStep2UC_B;
                case ApplicationType.SRM:
                    return AdhocSapSrmStep2UC_B;
                case ApplicationType.SSIDS_GUI:
                case ApplicationType.SSIDS_SMS:
                    return AdhocSsidsStep2UC_B;
                case ApplicationType.IDMS:
                    return AdhocIdmsStep2UC_B;
                case ApplicationType.VPN:
                    return AdhocVPNStep2UC_B;
                case ApplicationType.MicrosoftTeams:
                    return AdhocSkypeForBusinessStep2UC_B;
                case ApplicationType.EBS:
                case ApplicationType.MSSL:
                    return AdhocSAPHANAStep2UC_B;
            }
            return null;
        }

        public bool SaveBulkRequest()
        {
            UIBaseStep2UC_B step2 = getSecondStep();
            if (step2 == null)
            {
                Response.Redirect("~/Request/List.aspx");
                return true;
            }
            return step2.SaveBulkRequest();
        }

    }
}