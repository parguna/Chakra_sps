using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC
{
    public class UIBaseStep2UC_B : UIBaseUC
    {
        public UIBaseStep2UC_B() : base()
        {

        }
        const string V_ADHOCREQUEST = "V_ADHOCREQUEST";
        const string V_EntitlementDetail = "V_EntitlementDetail";
        public Dto.BulkRequest AdhocRequest
        {
            get
            {
                if (ViewState[V_ADHOCREQUEST] != null)
                {
                    return (Dto.BulkRequest)ViewState[V_ADHOCREQUEST];
                }
                return null;
            }
            set
            {
                ViewState[V_ADHOCREQUEST] = value;
            }
        }

        protected virtual void BindAdhocRequest()
        {
        }
        public IEntitlementDetail EntitlementDetail
        {
            get
            {
                if (ViewState[V_ADHOCREQUEST] != null)
                {
                    return (IEntitlementDetail)ViewState[V_EntitlementDetail];
                }
                return null;
            }
            set
            {
                ViewState[V_EntitlementDetail] = value;
            }
        }

        public void Initialize()
        {
            InitializeStep();
            if (AdhocRequest != null)
            {
                BindAdhocRequest();
            }
            else
            {
                afterSave();
            }
            //checkForValueChange();
        }

        void checkForValueChange()
        {
            var valueNamesList = new List<string>() { "EndPoint", "EndpointSystemType", "RequesterUserId", "SelectedSupervisor", "AdhocActionType", "AccountType" };
            List<string> valueNames = (List<string>)Session["CHANGEDPROPERTY"];
            bool isvalueChanged = (bool)Session["AccessRequestChanged"];
            if (isvalueChanged && valueNames.Any(name => valueNamesList.Contains(name)))
            {
                //ClearValuesOnStep1ValuesChanged(valueNames.ToArray());
            }
            Session["AccessRequestChanged"] = false;
            Session["CHANGEDPROPERTY"] = new List<string>();
        }

        public virtual bool SaveBulkRequest()
        {
            if (Validate())
            {
                beforeSave();
                if (Save())
                {
                    afterSave();
                    Response.Redirect("~/Request/List.aspx");
                }

                return true;
            }

            return false;
        }

        bool Save()
        {
            if (AdhocRequest.ApplicationType == Infrastructure.Enums.ApplicationType.VPN)
            {
                var entitlements = (AdHocVPNRequest)AdhocRequest.GetEntitlementDetail();
                AdhocRequest.ProgressStatus = Infrastructure.Enums.AccessRequestProgress.Submitted;
                AdhocRequest.RequestStatus = Infrastructure.Enums.AccessRequestStatus.Submitted;
                if (entitlements != null)
                {
                    var applnEntitlements = entitlements.AdHocVPNRequestDetails.Where(i => i.VPNType == Infrastructure.Enums.VPNType.SSLVPNOnly).ToList();
                    var prjEntitlements = entitlements.AdHocVPNRequestDetails.Where(i => i.VPNType == Infrastructure.Enums.VPNType.VDI).ToList();
                    AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(entitlements);
                    DomainApiClient<Dto.BulkRequest>.Save(AdhocRequest);

                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                AdhocRequest.ProgressStatus = Infrastructure.Enums.AccessRequestProgress.Submitted;
                AdhocRequest.RequestStatus = Infrastructure.Enums.AccessRequestStatus.Submitted;
                DomainApiClient<Dto.BulkRequest>.Save(AdhocRequest);
            }

            return true;
        }
        protected virtual void beforeSave()
        {
            SetEntitlementDetail();
        }

        protected virtual void afterSave()
        {
        }

        public virtual bool Validate()
        {
            throw new NotImplementedException();
        }


        protected virtual void SetEntitlementDetail()
        {
            throw new NotImplementedException();
        }

        public virtual bool NeedToBeSavedOnGoBack()
        {
            return false;
        }

        protected virtual void InitializeStep()
        {
            throw new NotImplementedException();
        }

        protected virtual void ClearValuesOnStep1ValuesChanged(string[] propertyNamesValueChanged)
        {
            afterSave();
        }

        protected virtual void Save_Click(object sender, EventArgs e)
        {
            SaveBulkRequest();
        }

    }
}