using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.NACR.Create.App.Step2UC
{
    public class UIBaseStep2UC : UIBaseUC
    {
        private const string V_ADHOCREQUEST = "V_ADHOCREQUEST";
        private const string V_ENTITLEMENT = "V_ENTITLEMENT";
        const string V_ReachSecondStep = "ReachSecondStep";
        public delegate void SaveAccessRequestHandler(IEntitlementDetail ientitlement, AdHocAccessRequest adhocRequest);
        public event SaveAccessRequestHandler ToSaveAccessRequest;

        public AdHocAccessRequest AdhocRequest
        {
            get
            {
                if (ViewState[V_ADHOCREQUEST] != null)
                {
                    return (AdHocAccessRequest)ViewState[V_ADHOCREQUEST];
                }
                return null;
            }
            set
            {

                if (value != null && value.EntitlementDetail == null && EntitlementDetail != null)
                {
                    value.EntitlementDetail = AdhocRequest.GetSerializedString(EntitlementDetail);
                }
                ViewState[V_ADHOCREQUEST] = value;
            }
        }

        public IEntitlementDetail EntitlementDetail
        {
            get
            {
                if (ViewState[V_ENTITLEMENT] != null)
                {
                    return (IEntitlementDetail)ViewState[V_ENTITLEMENT];
                }
                return null;
            }
            set
            {
                ViewState[V_ENTITLEMENT] = value;
            }
        }

        public bool ReachSecondStep
        {
            get
            {
                if (ViewState[V_ReachSecondStep] != null)
                {
                    return (bool)ViewState[V_ReachSecondStep];
                }
                return false;
            }
            set
            {
                ViewState[V_ReachSecondStep] = value;
            }
        }
        public UIBaseStep2UC() : base()
        {

        }

        public void Initialize()
        {

            checkForValueChange();
            InitializeStep();
        }


        void checkForValueChange()
        {
            if (ReachSecondStep)
            {
                var valueNamesList = new List<string>() { "EndPoint", "EndpointSystemType", "RequesterUserId", "SelectedSupervisor", "AdhocActionType", "AccountType" };
                List<string> valueNames = (List<string>)Session["CHANGEDPROPERTY"];
                bool isvalueChanged = (bool)Session["AccessRequestChanged"];
                if (isvalueChanged && valueNames.Any(name => valueNamesList.Contains(name)) &&
                    !(Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0)
                    )
                {
                    this.AdhocRequest.EntitlementDetail = null;
                    ClearValuesOnStep1ValuesChanged(valueNames.ToArray());
                }
            }
            Session["AccessRequestChanged"] = false;
            Session["CHANGEDPROPERTY"] = new List<string>();
            ReachSecondStep = true;
        }

        public virtual void AddToCart(object sender, EventArgs e)
        {
            AddToCart();
        }

        public bool canSave(AdHocAccessRequest adHocAccessRequest)
        {
            return !IsAccountExist(adHocAccessRequest);
        }

        bool IsAccountExist(AdHocAccessRequest adHocAccessRequest)
        {
            if (AdhocRequest == null)
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "AdHocAccessRequest is null.");
                return true;
            }

            string validationMsg = string.Empty;
            bool result = AdhocAccessRequestValidation.IsExists(AdhocRequest, out validationMsg);
            if (result && !string.IsNullOrEmpty(validationMsg))
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, validationMsg);
                return true;
            }

            return false;
        }

        public bool AddToCart()
        {
            if (Validate())
            {
                beforeSave();
                if (!SaveAccessRequest()) return false;
                doAfterSave();
                if (AdhocRequest != null)
                    AdhocRequest.SecurityQuestions = string.Empty;
                return true;
            }
            else
                return false;

        }

        protected void doAfterSave()
        {
            clearEntitlement();
            afterSave();
            this.AdhocRequest.EntitlementDetail = null;
        }

        void clearEntitlement()
        {
            EntitlementDetail = null;
        }

        protected virtual void afterSave()
        {
            if (AdhocRequest != null)
                AdhocRequest.SecurityQuestions = string.Empty;
        }

        protected virtual void beforeSave()
        {
        }

        public virtual bool Validate()
        {
            throw new NotImplementedException();
        }

        public void PersistEntitlementDetail()
        {
            EntitlementDetail = GetEntitlementDetail();
        }

        protected virtual IEntitlementDetail GetEntitlementDetail()
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

        protected virtual bool SaveAccessRequest()
        {
            IEntitlementDetail detail = GetEntitlementDetail();
            AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(detail);
            if (!ServiceSharedAccountNameValidation()) return false;
            if (IsAccountExist(AdhocRequest))
                return false;

            if (AdhocRequest.ValidTo.HasValue && (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove) && AdhocRequest.AccountType != AccountType.Service)
            {
                var canproceed = ValidateValidTo(AdhocRequest);
                if (!canproceed) return false;
            }

            ToSaveAccessRequest(detail, AdhocRequest);
            return true;
        }


        bool ValidateValidTo(AdHocAccessRequest adHocAccessRequest)
        {
            var entitlement = adHocAccessRequest.GetEntitlementDetail();
            if (entitlement.EndpointUserId == 0)
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "Please select the account");
                return false;
            }


            var accountRecon = DomainApiClient<AccountReconcilation>.GetByAction<AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser,
                new Dictionary<string, string> { {"endpointSystemId", adHocAccessRequest.EndpointSystemId.ToString() },
                                                 { "endpointUserId", entitlement.EndpointUserId.ToString() }
                                               }
                );

            if (accountRecon == null)
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "Please select the account");
                return false;
            }

            if (accountRecon.ValidToDate.HasValue && adHocAccessRequest.ValidTo.HasValue && accountRecon.ValidToDate < adHocAccessRequest.ValidTo)
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, $"In Step 1, Please select the ValidTo less than the account expiry date");
                return false;
            }

            return true;
        }

        bool ServiceSharedAccountNameValidation()
        {
            var adhocRequest = AdhocRequest;
            var excludeSAPapplicationTypes = new[] { ApplicationType.MSSL_CRM, ApplicationType.MSSL_ISU, ApplicationType.EBS_CRM, ApplicationType.EBS_ISU,ApplicationType.EBS, ApplicationType.MSSL };
            var conditionToCheckSAP =
                (adhocRequest.AdhocActionType == AdHocActionType.NewAccount
                && (adhocRequest.EndpointSystemType == EndpointType.SAP || adhocRequest.EndpointSystemType == EndpointType.EAM_NOVA)
                && !excludeSAPapplicationTypes.Contains(adhocRequest.ApplicationType)
                && (adhocRequest.AccountType == AccountType.Service || adhocRequest.AccountType == AccountType.Shared));

            if (conditionToCheckSAP)
            {
                var entitlementDetail = adhocRequest.GetEntitlementDetail();
                if (entitlementDetail != null && entitlementDetail.SharedAccountDetail != null && !string.IsNullOrWhiteSpace(entitlementDetail.SharedAccountDetail.AccountName))
                {
                    var validateAccountName = MasterConfigHelper.ValidateSAPSpecialAccountName;
                    if (validateAccountName)
                    {
                        if (adhocRequest.AccountType == AccountType.Shared && !entitlementDetail.SharedAccountDetail.AccountName.StartsWith("SPECIAL", StringComparison.OrdinalIgnoreCase))
                        {
                            ShowAlertMessage(Infrastructure.AlertType.Warning, $"{adhocRequest.EndpointSystemType.ToString()} Shared Account name should be prefixed with 'SPECIAL'. Please re-enter the account name with correct prefix.");
                            return false;
                        }

                        if (adhocRequest.AccountType == AccountType.Service && !entitlementDetail.SharedAccountDetail.AccountName.StartsWith("RFC", StringComparison.OrdinalIgnoreCase))
                        {
                            ShowAlertMessage(Infrastructure.AlertType.Warning, $"{adhocRequest.EndpointSystemType.ToString()} Service Account name should be prefixed with 'RFC'. Please re-enter the account name with correct prefix.");
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        protected virtual void SecurityQuestion_SecurityResponseSaveClicked(List<SecurityResponseDetail> detail)
        {
            if (this.AdhocRequest == null) return;
            SecurityResponse response = new SecurityResponse();
            response.SecurityResponseDetails = detail;
            this.AdhocRequest.SetSecurityQuestions = response;
        }

        protected virtual void SecurityQuestionUC_SecurityQuestionPopUp(object sender, EventArgs e)
        {
            SecurityQuestionUC control = sender as SecurityQuestionUC;
            if (control == null || AdhocRequest == null || AdhocRequest.GetSecurityQuestions == null) return;
            control.SecurityResponseDetail = AdhocRequest.GetSecurityQuestions.SecurityResponseDetails;
        }

    }

    public static class DropDownHelper
    {
        public static void DropDownListBind(DropDownList ddlControl, object dataSource, string dataTextField, string dataValueField, string selectedText, bool blankRow, bool selectAll)
        {
            ddlControl.DataTextField = dataTextField;
            ddlControl.DataValueField = dataValueField;
            ddlControl.DataSource = dataSource;
            ddlControl.DataBind();
            if (blankRow)
            {
                if (selectAll)
                {
                    ddlControl.Items.Insert(0, "ALL");
                }
                else
                {
                    ddlControl.Items.Insert(0, "--Select--");
                    //ddlControl.Items.Add(new ListItem("--Select--", "-1"));
                    //ddlControl.Text = "--Select--";
                }
                ddlControl.Items[0].Selected = true;
            }
            if (!string.IsNullOrEmpty(selectedText))
            {
                ddlControl.Items[0].Selected = false;
                if (ddlControl.Items.FindByText(selectedText) != null)
                {
                    ddlControl.Items.FindByText(selectedText).Selected = true;
                }
            }
            if (ddlControl.Items.Count > 0 && selectedText == null)
            {
                ddlControl.Items[0].Selected = true;
            }
        }

        public static void DropDownListBind(DropDownList ddlControl, object dataSource, string dataTextField, string dataValueField, string selectedText, bool blankRow, string defaultString, bool selectAll)
        {
            ddlControl.DataTextField = dataTextField;
            ddlControl.DataValueField = dataValueField;
            ddlControl.DataSource = dataSource;
            ddlControl.DataBind();
            ddlControl.ClearSelection();
            if (blankRow)
            {
                if (selectAll)
                {
                    ddlControl.Items.Insert(0, "ALL");
                }
                else
                {
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        ddlControl.Items.Insert(0, "--Select--");
                    }
                    else
                    {
                        ddlControl.Items.Insert(0, defaultString);
                    }
                }
                if (ddlControl.Items.Count == 2)
                {
                    ddlControl.Items[1].Selected = true;
                }
                else if (ddlControl.Items.Count == 1)
                {
                    ddlControl.Items[0].Selected = true;
                }
                else if (ddlControl.Items.Count > 0)
                {
                    ddlControl.Items[0].Selected = true;
                }
            }
            if (!string.IsNullOrEmpty(selectedText))
            {
                if (ddlControl.Items.FindByText(selectedText) != null)
                {
                    ddlControl.Items.FindByText(selectedText).Selected = true;
                }
            }
        }

    }
}