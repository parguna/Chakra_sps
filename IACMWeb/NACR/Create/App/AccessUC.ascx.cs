using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Web.NACR.Create.App.Step2UC;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace DI.Chakra.Web.NACR.Create.App
{
    public partial class AccessUC : DI.Chakra.Web.UIBaseUC
    {
        private const string V_TOTALITEMS = "V_TOTALITEMS";
        private const string V_REQUESTINFO = "V_REQUESTINFO";

        public delegate void RefreshCartHandler();
        public event RefreshCartHandler SaveInterimStore;

        public bool RefreshEndpointTree
        {
            set
            {
                //RequestItemList.RefreshEndpointTree = true;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (AccessRequest != null)
            {
                CurrentAccessUC.EntityId = AccessRequest.RequesteeUserId;
            }
        }

        public Dto.AdHocAccessRequest AccessRequest
        {
            get
            {
                if (ViewState[V_REQUESTINFO] == null)
                    return null;
                return (Dto.AdHocAccessRequest)ViewState[V_REQUESTINFO];
            }
            set
            {
                ViewState[V_REQUESTINFO] = value;
            }
        }

        void ShowWindow()
        {
            var key = "showWindoww";
            string script = "Sys.Application.remove_load(showWindow);Sys.Application.add_load(showWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        void CloseWindow()
        {
            var key = "closeWindoww";
            string script = "Sys.Application.remove_load(closeWindow);Sys.Application.add_load(closeWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        public void PersistEntitlementDetail()
        {
            var temp = this.getSecondStep();
            if (temp != null) 
               temp.PersistEntitlementDetail();
        }
        public virtual void SaveAccessRequest(IEntitlementDetail entitlementDetail, Dto.AdHocAccessRequest adHocAccessRequest)
        {
            //if(adHocAccessRequest.ApplicationType == ApplicationType.VPN)
            //{

            //    if (!ValidateRequest(adHocAccessRequest)) return;
            //    var adHocVPNentitlements = (AdHocVPNRequest)entitlementDetail;
            //    if (adHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount || 
            //        (adHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd && adHocVPNentitlements.UpdateOneKeyToken==false) || adHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove
            //        ) 
            //    {
            //        var vpnRequestDetailsProject = adHocVPNentitlements.AdHocVPNRequestDetails.FindAll(i => i.isOthers);
            //        var vpnRequestDetailsApplication = adHocVPNentitlements.AdHocVPNRequestDetails.FindAll(i => !i.isOthers);
            //        if (vpnRequestDetailsProject.Count > 0)
            //        {
            //            adHocVPNentitlements.AdHocVPNRequestDetails = vpnRequestDetailsProject;
            //            adHocAccessRequest.EntitlementDetail = adHocAccessRequest.GetSerializedString(adHocVPNentitlements);
            //            Dto.InterimStore interimstore = new Dto.InterimStore();
            //            interimstore.InterimStoreItemType = InterimStoreItemType.AccessRequest;
            //            interimstore.IsActive = true;
            //            interimstore.IsSaved = true;
            //            interimstore.UserId = new SessionStatusImpl().User.Id;
            //            interimstore.XmlData = UtilityMethods.Serialize<Dto.AdHocAccessRequest>(adHocAccessRequest);
            //            DomainApiClient<Dto.InterimStore>.Save(interimstore);
            //        }
            //        if (vpnRequestDetailsApplication.Count > 0)
            //        {
            //            adHocVPNentitlements.AdHocVPNRequestDetails = vpnRequestDetailsApplication;
            //            adHocAccessRequest.EntitlementDetail = adHocAccessRequest.GetSerializedString(adHocVPNentitlements);
            //            Dto.InterimStore interimstore = new Dto.InterimStore();
            //            interimstore.InterimStoreItemType = InterimStoreItemType.AccessRequest;
            //            interimstore.IsActive = true;
            //            interimstore.IsSaved = true;
            //            interimstore.UserId = new SessionStatusImpl().User.Id;
            //            interimstore.XmlData = UtilityMethods.Serialize<Dto.AdHocAccessRequest>(adHocAccessRequest);
            //            DomainApiClient<Dto.InterimStore>.Save(interimstore);
            //        }
            //    }
            //    else
            //    {
                    
            //        adHocAccessRequest.EntitlementDetail = adHocAccessRequest.GetSerializedString(entitlementDetail);
            //        if (!ValidateRequest(adHocAccessRequest)) return;
            //        Dto.InterimStore interimstore = new Dto.InterimStore();
            //        interimstore.InterimStoreItemType = InterimStoreItemType.AccessRequest;
            //        interimstore.IsActive = true;
            //        interimstore.IsSaved = true;
            //        interimstore.UserId = new SessionStatusImpl().User.Id;
            //        interimstore.XmlData = UtilityMethods.Serialize<Dto.AdHocAccessRequest>(adHocAccessRequest);
            //        DomainApiClient<Dto.InterimStore>.Save(interimstore);
            //    }
            //    SaveInterimStore();
            //}
            
            //else
            {
                adHocAccessRequest.EntitlementDetail = adHocAccessRequest.GetSerializedString(entitlementDetail);
                if (!ValidateRequest(adHocAccessRequest)) return;
                Dto.InterimStore interimstore = new Dto.InterimStore();
                interimstore.InterimStoreItemType = InterimStoreItemType.AccessRequest;
                interimstore.IsActive = true;
                interimstore.IsSaved = true;
                interimstore.UserId = new SessionStatusImpl().User.Id;
                interimstore.XmlData = UtilityMethods.Serialize<Dto.AdHocAccessRequest>(adHocAccessRequest);
                DomainApiClient<Dto.InterimStore>.Save(interimstore);
                SaveInterimStore();
            }
            
        }

        public bool AddToCart()
        {
            UIBaseStep2UC step2 = getSecondStep();
            if (step2 == null) return false;
            return step2.AddToCart();
        }
        protected virtual bool ValidateRequest(AdHocAccessRequest accessRequest)
        {
            //if (accessRequest == null)
            //{
            //    ShowAlertMessage(Infrastructure.AlertType.Error, "AdHocAccessRequest is null.");
            //    return false;
            //}

            //string validationMsg = string.Empty;
            //bool result = AdhocAccessRequestValidation.IsExists(accessRequest, out validationMsg);
            //if (result && !string.IsNullOrEmpty(validationMsg))
            //{
            //    ShowAlertMessage(Infrastructure.AlertType.Error, validationMsg);
            //    return false;
            //}
            return true;
        }
        public void Initialize()
        {
            EntitlementVisibility();
        }

        public void InitializeControlOnly()
        {
            //Session["AdhocrequestObj"] = AccessRequest;
            UIBaseStep2UC step2 = getSecondStep();
            if (step2 == null) return;
            step2.Visible = true;
        }
        void setInVisibility()
        {
            foreach (var control in pnlStep2Controls.Controls)
            {
                UIBaseStep2UC step2Control = control as UIBaseStep2UC;
                if (step2Control != null)
                    step2Control.Visible = false;
            }
        }

        void EntitlementVisibility()
        {
            Session["AdhocrequestObj"] = AccessRequest;
            UIBaseStep2UC step2 = getSecondStep();
            if (step2 == null) return;
            step2.Visible = true;
            step2.AdhocRequest = AccessRequest;
            step2.Initialize();
        }



        UIBaseStep2UC getSecondStep()
        {
            if (AccessRequest == null) return null;
            setInVisibility();
            switch (AccessRequest.ApplicationType)
            {
                case ApplicationType.ADSingaporePowerLocal:
                    return AdhocADStep2UC;
                case ApplicationType.HRMS_FLINS:
                    return AdhocSapFlinsStep2UC;
                case ApplicationType.SAPLocal:
                    return AdhocSapLocalStep2UC;
                case ApplicationType.FLINS_BI:
                    return AdhocSapFlinsBIStep2UC;
                case ApplicationType.SAP_PI:
                    return AdhocSapPiStep2UC;
                case ApplicationType.SAP_GRC:
                    return AdhocSapGrcStep2UC;
                case ApplicationType.MSSL_CRM:
                case ApplicationType.MSSL_ISU:
                    return AdhocSapMsslStep2UC;
                case ApplicationType.NPAC:
                    return AdhocNpacStep2UC;
                case ApplicationType.GEMS:
                    return AdhocGemsStep2UC;
                case ApplicationType.EBS_CRM:
                case ApplicationType.EBS_ISU:
                    return AdhocSapEbsStep2UC;
                case ApplicationType.Office365:
                    return AdhocOffice365Step2UC;
                case ApplicationType.InhouseSystems:
                    return AdhocInHouseStep2UC;
                case ApplicationType.SAP_EP:
                case ApplicationType.CE:
                    return AdhocSapEpCeStep2UC;
                case ApplicationType.OneCard:
                    return AdhocOneCardStep2UC;
                case ApplicationType.SRM:
                    return AdhocSapSrmStep2UC;
                case ApplicationType.SSIDS_GUI:
                case ApplicationType.SSIDS_SMS:
                    return AdhocSsidsStep2UC;
                case ApplicationType.IDMS:
                    return AdhocIdmsStep2UC;
                case ApplicationType.VPN:
                    return AdhocVPNStep2UC;
                case ApplicationType.MicrosoftTeams:
                    return AdhocSkypeForBusinessStep2UC;
                case ApplicationType.EAM:
                    return this.AdhocNovaStep2UC;
                case ApplicationType.EBS:
                case ApplicationType.MSSL:
                    return AdhocSAPHANAStep2UC;
            }
            return null;
        }

        protected void RequestItemList_ItemClicked(object sender, EventArgs e)
        {
            //CartInfo.Refresh();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            CloseWindow();
        }

        protected void ViewCurrentAccess_Click(object sender, EventArgs e)
        {
            CurrentAccess.Width = GetUnit(80);
            CurrentAccessUC.HideAccountDetail = true;
            CurrentAccessUC.Rebind();
            ShowWindow();
        }


    }
}