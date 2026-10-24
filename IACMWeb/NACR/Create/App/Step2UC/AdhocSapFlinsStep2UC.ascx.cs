using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.App.Step2UC
{
    public partial class AdhocSapFlinsStep2UC : UIBaseStep2UC
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override bool NeedToBeSavedOnGoBack()
        {
            if (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0)
            {
                Save_Click(null, null);
                Session["AdhocrequestObj"] = AdhocRequest;
            }
            return AdhocRequest != null && AdhocRequest.GetEntitlementDetail() != null;
        }

        #region Members 

        private const string V_REQUEST = "V_REQUEST";
        const string V_isCompanyMandatory = "V_isCompanyMandatory";
        const string V_canClearAllPurchaseGroup = "V_canClearAllPurchaseGroup";
        #endregion

        #region Properties

        public List<AdHocSAPHRFLINSRequestDetail> EntitlementRequest
        {
            get
            {
                if (ViewState[V_REQUEST] != null)
                {
                    return (List<AdHocSAPHRFLINSRequestDetail>)ViewState[V_REQUEST];
                }
                return new List<AdHocSAPHRFLINSRequestDetail>();
            }
            set
            {
                ViewState[V_REQUEST] = value;
            }
        }


        public bool isCompanyMandatory
        {
            get
            {
                if (ViewState[V_isCompanyMandatory] != null)
                {
                    return (bool)ViewState[V_isCompanyMandatory];
                }
                return false;
            }
            set
            {
                ViewState[V_isCompanyMandatory] = value;
            }
        }


        public Dto.SAPFlinsMatrixBusinessRules SAPFlinsMatrixBusinessRules
        {
            get
            {
                if (ViewState[V_canClearAllPurchaseGroup] != null)
                {
                    return (Dto.SAPFlinsMatrixBusinessRules)ViewState[V_canClearAllPurchaseGroup];
                }
                return null;
            }
            set
            {
                ViewState[V_canClearAllPurchaseGroup] = value;
            }
        }

        public string EndpointUserId
        {
            get
            {
                return EndPointUserDropDownList.SelectedValue;
            }
        }



        #endregion

        #region Events

        protected override IEntitlementDetail GetEntitlementDetail()
        {
            AdHocSAPHRFLINSRequest entitlementDetail = new AdHocSAPHRFLINSRequest();
            entitlementDetail = GenerateRequest(false);
            entitlementDetail.AdHocSAPHRFLINSRequestDetails.AddRange(EntitlementRequest);
            if (EntitlementRequest.Count <= 0 && (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && AdhocRequest.AccountType == AccountType.User)) return null;

            if (SharedPanelHRMSFLINS.Visible)
            {
                SAPUserTypeDropDown.SelectedIndex = 0;
                txtAcntName.Text = "";
                if (AdhocRequest.AccountType != AccountType.Secondary)
                    txtDiscription.Text = "";
            }

            FillRequestGrid();
            return entitlementDetail;
        }
        protected override void InitializeStep()
        {
            txtDiscription.Enabled = true;
            trSecurityQstns.Visible = false;
            Session["Module"] = null;
            Session["SubModule"] = null;
            Session["EndpointUserId"] = null;
            Session["AdhocrequestObj"] = AdhocRequest;
            // AddButton.Visible = true;
            EntitlementRequest.Clear();
            FillRequestGrid();
            txtDiscription.Attributes.Add("maxlength", "40");
            if (AdhocRequest != null)
            {
                SpecialAccountWarning.ShowWarningMessage(AdhocRequest);
                var request = AdhocRequest;
                OtherDetailPanel.Visible = true;
                DeleteAccountAdd.Visible = true;
                if (request.AdhocActionType == AdHocActionType.NewAccount && isServiceOrSharedAccountRequest())
                {
                    SharedPanelHRMSFLINS.Visible = true;
                    BindSAPUserType(AdhocRequest.AccountType);
                    txtAcntName.Text = "";
                    if (AdhocRequest.AccountType != AccountType.Secondary)
                        txtDiscription.Text = "";
                    LoadControl();
                    if (request.AccountType == AccountType.Service)
                    {
                        trSecurityQstns.Visible = true;
                    }

                }

                else
                {
                    //  AddButton.Visible = true;

                    SetMandatory(BusinessRoletr, true);
                    SharedPanelHRMSFLINS.Visible = true;

                    LoadControl();
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && AdhocRequest.AccountType == AccountType.User)
                {
                    SharedPanelHRMSFLINS.Visible = false;
                    SetMandatory(BusinessRoletr, false);
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    DeleteAccountAdd.Visible = false;
                ModuleDropDownList.SelectedIndex = -1;
                SubModuleDropDownList.SelectedIndex = -1;
                Session["Module"] = null;
                Session["SubModule"] = null;
                CostCenterAutoCompleteBox.Enabled = false;
                CompanyAutoCompleteBox.Enabled = false;
                PurchaseOrgAutoCompleteBox.Enabled = false;
                PurchaseGroupAutoCompleteBox.Enabled = false;
                PlanetAutoCompleteBox.Enabled = false;
                StoreLocationsAutoCompleteBox.Enabled = false;
                MRPControllerAutoCompleteBox.Enabled = false;
                BusinessRoleAutoCompleteBox.Enabled = true;
                BusinessRoleAutoCompleteBox.Entries.Clear();
                if (CompanyAutoCompleteBox.CheckBoxes == true)
                {
                    ClearComboBox(CompanyAutoCompleteBox);
                }
                else
                {
                    CompanyAutoCompleteBox.Items.Clear();
                }
                ClearComboBox(CostCenterAutoCompleteBox);
                ClearComboBox(PurchaseOrgAutoCompleteBox);
                ClearComboBox(PurchaseGroupAutoCompleteBox);
                ClearComboBox(PlanetAutoCompleteBox);
                ClearComboBox(StoreLocationsAutoCompleteBox);
                ClearComboBox(MRPControllerAutoCompleteBox);
                RemarksLabel.Text = "";
                if (AdhocRequest.GetEntitlementDetail() != null || (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0))
                {
                    BindAdhocRequest();
                }
            }

        }
        private void BindAdhocRequest()
        {
            var Entitlement = (AdHocSAPHRFLINSRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (EndPointUserDropDownList.Items.FindByValue(Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId)) != null)
                    EndPointUserDropDownList.SelectedValue = Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId);
                EndPointUserDropDownList_SelectedIndexChanged(null, null);
                if (Entitlement.AdHocSAPHRFLINSRequestDetails != null)
                    EntitlementRequest = Entitlement.AdHocSAPHRFLINSRequestDetails;
                FillRequestGrid();

                if (Entitlement.AdHocSAPHRFLINSSharedAccount != null)
                {
                    if (SAPUserTypeDropDown.Items.FindByText(Convert.ToString(Entitlement.AdHocSAPHRFLINSSharedAccount.SAPUserType)) != null)
                    {
                        SAPUserTypeDropDown.Items.FindByText(Convert.ToString(Entitlement.AdHocSAPHRFLINSSharedAccount.SAPUserType)).Selected = true;
                    }
                    txtAcntName.Text = Entitlement.AdHocSAPHRFLINSSharedAccount.AccountName;
                    txtDiscription.Text = Entitlement.AdHocSAPHRFLINSSharedAccount.Description;
                }

            }
        }

        #endregion

        bool isServiceOrSharedAccountRequest()
        {
            return AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Secondary;
        }

        bool IsAmendRequest()
        {
            return AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove;
        }

        bool IsDeleteRequest()
        {
            return AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount;
        }
        private void FillRequestGrid()
        {
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                RequestGrid.Columns.FindByUniqueName("RetainCompanyRoles").Display = true;
            }
            else
            {
                RequestGrid.Columns.FindByUniqueName("RetainCompanyRoles").Display = false;
            }
            if (this.EntitlementRequest == null)
            {
                EntitlementRequest = new List<AdHocSAPHRFLINSRequestDetail>();
            }
            RequestGrid.Visible = this.EntitlementRequest.Count > 0 ? true : false;
            RequestGrid.DataSource = this.EntitlementRequest;
            RequestGrid.DataBind();
        }
        private void ClearMandatorField(bool required = false)
        {
            divCompany.Attributes.Add("class", $"form-group");
            divPurchasingOrg.Attributes.Add("class", $"form-group");
            divCostCenter.Attributes.Add("class", $"form-group");
            divPgroup.Attributes.Add("class", $"form-group");
            divPlant.Attributes.Add("class", $"form-group");
            divStoreLocation.Attributes.Add("class", $"form-group");
            divMRPController.Attributes.Add("class", $"form-group");
        }
        private void SetMandatorField()
        {
            divCompany.Attributes.Add("class", string.Format("form-group {0}", CompanyAutoCompleteBox.Enabled ? "required" : ""));
            divPurchasingOrg.Attributes.Add("class", string.Format("form-group {0}", PurchaseOrgAutoCompleteBox.Enabled ? "required" : ""));
            divCostCenter.Attributes.Add("class", string.Format("form-group {0}", CostCenterAutoCompleteBox.Enabled ? "required" : ""));
            divPgroup.Attributes.Add("class", string.Format("form-group {0}", PurchaseGroupAutoCompleteBox.Enabled ? "required" : ""));
            divPlant.Attributes.Add("class", string.Format("form-group {0}", PlanetAutoCompleteBox.Enabled ? "required" : ""));
            divStoreLocation.Attributes.Add("class", string.Format("form-group {0}", StoreLocationsAutoCompleteBox.Enabled ? "required" : ""));
            divMRPController.Attributes.Add("class", string.Format("form-group {0}", MRPControllerAutoCompleteBox.Enabled ? "required" : ""));
        }
        protected void BusinessRoleAutoCompleteBox_EntryAdded(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            try
            {

                if (BusinessRoleAutoCompleteBox.Entries.Count == 2)
                {
                    BusinessRoleAutoCompleteBox.Entries.Remove(BusinessRoleAutoCompleteBox.Entries[1]);
                }
                else
                {
                    string businessRole = BusinessRoleAutoCompleteBox.Text.Replace(";", "").Trim().Trim();
                    RemarksLabel.Text = DomainApiClient<Dto.SAPHRFLINSRoleMatrix>.GetByAction<string>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetUserType, new Dictionary<string, string>() { { "businessRole", businessRole } });
                    Dictionary<string, string> listofcontrols = DomainApiClient<Dto.SAPRoleMandatoryField>.GetByAction<Dictionary<string, string>>(DomainApiAction.Matrix.SAP_FLINS.SAPRoleMandatoryFields.MandatoryFieldIdentification, new Dictionary<string, string>() { { "businessRole", businessRole } });
                    if (listofcontrols.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> pair in listofcontrols)
                        {
                            CompanyAutoCompleteBox.Enabled = (pair.Key != null && Convert.ToString(pair.Key).Contains("Company")) ? true : false;
                            CompanyAutoCompleteBox.CheckBoxes = (pair.Key != null && Convert.ToString(pair.Key).Contains("Company")) && string.IsNullOrEmpty(pair.Value) ? true : false;
                            PurchaseOrgAutoCompleteBox.Enabled = (pair.Key != null && Convert.ToString(pair.Key).Contains("Org")) ? true : false;
                            CostCenterAutoCompleteBox.Enabled = (pair.Key != null && Convert.ToString(pair.Key).Contains("CostCenter")) ? true : false;
                            PurchaseGroupAutoCompleteBox.Enabled = (pair.Key != null && Convert.ToString(pair.Key).Contains("PurchaseGroup")) ? true : false;
                            PlanetAutoCompleteBox.Enabled = (pair.Key != null && Convert.ToString(pair.Key).Contains("Plant")) ? true : false;
                            StoreLocationsAutoCompleteBox.Enabled = (pair.Key != null && Convert.ToString(pair.Key).Contains("StoreLocations")) ? true : false;
                            MRPControllerAutoCompleteBox.Enabled = (pair.Key != null && Convert.ToString(pair.Key).Contains("MRPController")) ? true : false;
                            Mandatory1HiddenField.Value = pair.Key;
                            Mandatory2HiddenField.Value = pair.Value;
                        }
                    }
                    SetMandatorField();
                    int EndpointUserId = 0;
                    if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        if (EndPointUserDropDownList.SelectedIndex > 0)
                        {
                            EndpointUserId = Convert.ToInt32(EndPointUserDropDownList.SelectedValue);
                        }
                    }

                    if (CompanyAutoCompleteBox.Enabled == true)
                    {


                        var parameter = new Dictionary<string, string> { { "searchString", "" }, { "businessRoleID", BusinessRoleAutoCompleteBox.Entries[0].Text }, { "EndpointUserId", EndpointUserId.ToString() } };


                        Dictionary<long, string> dataSource = DomainApiClient<SAPHRFLINSRoleMatrix>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetCompany, parameter);
                        CompanyAutoCompleteBox.DataSource = dataSource;
                        CompanyAutoCompleteBox.DataTextField = "Value";
                        CompanyAutoCompleteBox.DataValueField = "Key";
                        CompanyAutoCompleteBox.DataBind();
                        if (CompanyAutoCompleteBox.CheckBoxes == false)
                        {
                            CompanyAutoCompleteBox.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("-- Select Company --", string.Empty));
                        }
                    }

                    //bool 
                    isCompanyMandatory = false;


                    if (BusinessRoleAutoCompleteBox.Entries.Count > 0)
                    {
                        //List<string> addedBusinessRole = new List<string>() { BusinessRoleAutoCompleteBox.Entries[0].Text };
                        //addedBusinessRole.AddRange(EntitlementRequest.Select(b => b.Entitlement));

                        loadValidationRules();

                        isCompanyMandatory = SAPFlinsMatrixBusinessRules.IsCompanyMandatory;

                    }

                    if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove && SAPFlinsMatrixBusinessRules.HasSupplimentaryRole)
                    {
                        RemoveCompanyCheckBox.Visible = true;
                    }
                    else
                    {
                        RemoveCompanyCheckBox.Visible = false;
                    }

                }

            }
            catch (Exception ex)
            {
            }

        }


        void loadValidationRules(bool considerCompany = false)
        {
            //if (this.AdhocRequest.AdhocActionType != AdHocActionType.AmendmentRemove && string.IsNullOrWhiteSpace(EndpointUserId)) return;

            var canConsiderCompany = considerCompany && CompanyAutoCompleteBox.CheckBoxes == false && !string.IsNullOrWhiteSpace(CompanyAutoCompleteBox.Text);

            var parameter = new Dictionary<string, string> {
                                        { "EndpointUserId",String.IsNullOrWhiteSpace(EndpointUserId.ToString())? "0": EndpointUserId.ToString()  },
                                        { "BusinessRole", BusinessRoleAutoCompleteBox.Entries[0].Text.Trim() },
                                        { "Company",
                                            canConsiderCompany ? CompanyAutoCompleteBox.Text.Replace(",", "").Trim() : null }
                                                           };


            SAPFlinsMatrixBusinessRules = DomainApiClient<SAPRoleMandatoryField>
                .GetByAction<Dto.SAPFlinsMatrixBusinessRules>(DomainApiAction.Matrix.SAP_FLINS.SAPRoleMandatoryFields.checkBusinessRule, parameter, EntitlementRequest);
        }
        protected void BusinessRoleAutoCompleteBox_EntryRemoved(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            try
            {
                RemarksLabel.Text = "";
                CompanyAutoCompleteBox.Enabled = false;
                CostCenterAutoCompleteBox.Enabled = false;
                PurchaseOrgAutoCompleteBox.Enabled = false;
                PurchaseGroupAutoCompleteBox.Enabled = false;
                PlanetAutoCompleteBox.Enabled = false;
                StoreLocationsAutoCompleteBox.Enabled = false;
                MRPControllerAutoCompleteBox.Enabled = false;
                if (CompanyAutoCompleteBox.CheckBoxes == true)
                {
                    ClearComboBox(CompanyAutoCompleteBox);
                }
                else
                {
                    CompanyAutoCompleteBox.Items.Clear();
                }
                ClearComboBox(CostCenterAutoCompleteBox);
                ClearComboBox(PurchaseOrgAutoCompleteBox);
                ClearComboBox(PurchaseGroupAutoCompleteBox);
                ClearComboBox(PlanetAutoCompleteBox);
                ClearComboBox(StoreLocationsAutoCompleteBox);
                ClearComboBox(MRPControllerAutoCompleteBox);
                ClearMandatorField();
            }
            catch (Exception)
            {
            }
        }
        protected void AddButton_Click(object sender, EventArgs e)
        {

            if (VarianceValidation())
            {

                GenerateRequest(true);
                FillRequestGrid();
                ClearControls();
            }
        }

        protected override void afterSave()
        {
            base.afterSave();
            ClearControls();
            if (EntitlementRequest != null)
            {
                EntitlementRequest = null;
            }
            FillRequestGrid();
        }
        private void ClearControls()
        {
            RemoveCompanyCheckBox.Visible = false;
            RemoveCompanyCheckBox.Checked = false;
            if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                ModuleDropDownList.SelectedIndex = -1;
                Session["Module"] = null;
                Session["SubModule"] = null;
                SubModuleDropDownList.Items.Clear();
                SubModuleDropDownList.Items.Insert(0, new ListItem("-- Select SubModule --", "-1"));
                CostCenterAutoCompleteBox.Enabled = false;
                CompanyAutoCompleteBox.Enabled = false;
                PurchaseOrgAutoCompleteBox.Enabled = false;
                PurchaseGroupAutoCompleteBox.Enabled = false;
                PlanetAutoCompleteBox.Enabled = false;
                StoreLocationsAutoCompleteBox.Enabled = false;
                MRPControllerAutoCompleteBox.Enabled = false;
                BusinessRoleAutoCompleteBox.Entries.Clear();
                if (CompanyAutoCompleteBox.CheckBoxes == true)
                {
                    ClearComboBox(CompanyAutoCompleteBox);
                }
                else
                {
                    CompanyAutoCompleteBox.Items.Clear();
                }
                ClearComboBox(CostCenterAutoCompleteBox);
                ClearComboBox(PurchaseOrgAutoCompleteBox);
                ClearComboBox(PurchaseGroupAutoCompleteBox);
                ClearComboBox(PlanetAutoCompleteBox);
                ClearComboBox(StoreLocationsAutoCompleteBox);
                ClearComboBox(MRPControllerAutoCompleteBox);
                RemarksLabel.Text = "";
                //if (trEndPointUser.Visible)
                //{
                //    EndPointUserDropDownList.SelectedIndex = 0;
                //};
            }
            ClearMandatorField();
        }
        private bool VarianceValidation()
        {
            /*Note: High Important
                * If you want Remove all the costcenter/Purchasing Group,then use must select only business role and company without cost center/Purchasing Group then user select business role, company and Cost center/Purchasing Group.
                * This is workaround to solve this issue.
                * */
            if ((AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
            {
                if (EndPointUserDropDownList.SelectedIndex <= 0)
                {
                    ShowAlertMessage(AlertType.Error, "Please Select EndPoint User.");
                    ClearControls();
                    return false;
                }
            }

            string Message = "Please select the";
            string ControlMessage = "";
            if (BusinessRoleAutoCompleteBox.Entries.Count <= 0 || BusinessRoleAutoCompleteBox.Entries[0].Text == "No Records")
            {
                ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Business Role" : ControlMessage + " , Business Role";
            }
            if (CompanyAutoCompleteBox.Enabled == true)
            {
                if (CompanyAutoCompleteBox.CheckBoxes == true)
                {
                    if (CompanyAutoCompleteBox.CheckedItems.Count <= 0)
                    {
                        ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Company" : ControlMessage + " , Company";
                    }
                }
                else
                {
                    if (CompanyAutoCompleteBox.SelectedIndex <= 0)
                    {
                        ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Company" : ControlMessage + " , Company";
                    }
                }
            }
            if (PurchaseOrgAutoCompleteBox.Enabled == true)
            {
                if (PurchaseOrgAutoCompleteBox.CheckedItems.Count <= 0)
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Purchase Org" : ControlMessage + " , Purchase Org";
                }
            }
            if (CostCenterAutoCompleteBox.Enabled == true)
            {
                if (!(SAPFlinsMatrixBusinessRules.CanClearAllCostCenter == false && CostCenterAutoCompleteBox.Items.Count == 1) && CostCenterAutoCompleteBox.CheckedItems.Count <= 0)
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Cost Center" : ControlMessage + " , Cost Center";
                }                 
            }
            if (PurchaseGroupAutoCompleteBox.Enabled == true )
            {
                if (PurchaseGroupAutoCompleteBox.CheckedItems.Count <= 0 && PurchaseGroupAutoCompleteBox.Items.Count() > 1)
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Purchase Group" : ControlMessage + " , Purchase Group";
                }
            }
            if (PlanetAutoCompleteBox.Enabled == true)
            {
                if (PlanetAutoCompleteBox.CheckedItems.Count <= 0)
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Plant" : ControlMessage + " , Plant";
                }
            }
            if (StoreLocationsAutoCompleteBox.Enabled == true)
            {
                if (StoreLocationsAutoCompleteBox.CheckedItems.Count <= 0)
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Store Location" : ControlMessage + " , Store Location";
                }
            }
            if (MRPControllerAutoCompleteBox.Enabled == true)
            {
                if (MRPControllerAutoCompleteBox.CheckedItems.Count <= 0)
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " MRP Controller" : ControlMessage + " , MRP Controller";
                }
            }

            if (ControlMessage != "")
            {
                int LastIndex = ControlMessage.LastIndexOf(",");
                Message += LastIndex > 0 ? ControlMessage.Substring(0, LastIndex) + " and" + ControlMessage.Substring(LastIndex + 1) : ControlMessage;
                Message += ".";
                // ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert", @"alert('" + Message + "');", true);

                ShowAlertMessage(AlertType.Error, Message);
                return false;
            }

            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                var totalCompany = CompanyAutoCompleteBox.Items.Count;
                var isRequiredForCompanyCheck = (MRPControllerAutoCompleteBox.Enabled == true || PlanetAutoCompleteBox.Enabled == true || CostCenterAutoCompleteBox.Enabled == true || StoreLocationsAutoCompleteBox.Enabled == true || PurchaseOrgAutoCompleteBox.Enabled == true || PurchaseGroupAutoCompleteBox.Enabled == true);
                if (EntitlementRequest != null && EntitlementRequest.Count > 0 && CompanyAutoCompleteBox.Enabled == true)
                {
                    var bRole = BusinessRoleAutoCompleteBox.Entries[0].Text;
                    var bRoles = EntitlementRequest.Where(i => i.Entitlement.Equals(bRole, StringComparison.InvariantCultureIgnoreCase)).ToList();
                    if (CompanyAutoCompleteBox.CheckBoxes == true)
                    {
                        if (bRoles != null && bRoles.Count > 0)
                        {
                            ShowAlertMessage(AlertType.Error, $"BusinessRole:{bRole} already exists. Please remove the role from Grid and try again.");
                            return false;
                        }
                        if (CompanyAutoCompleteBox.Items.All(i => i.Checked == true) && RemoveCompanyCheckBox.Checked)
                        {
                            ShowAlertMessage(AlertType.Error, "At least one Company is required for your existing business role(s). Please deselect any one of them.");
                            return false;
                        }

                    }
                    else
                    {
                        var cmpny = CompanyAutoCompleteBox.Text;
                        if (bRoles != null && bRoles.Count > 0)
                        {
                            if (bRoles.Exists(i => i.Company.Equals(cmpny, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                ShowAlertMessage(AlertType.Error, $"BusinessRole:{bRole} and Company:{cmpny} already exists in the below Grid. Please remove the role from Grid and try again.");
                                return false;
                            }
                            var companies = bRoles.Select(i => i.Company).ToList();
                            companies.Add(cmpny);
                            companies = companies.Distinct(StringComparer.InvariantCultureIgnoreCase).ToList();
                            if (RemoveCompanyCheckBox.Checked && totalCompany > 0 && companies.Count > 0 && (companies.Count == totalCompany))
                            {
                                ShowAlertMessage(AlertType.Error, "At least one Company is required for your existing business role(s).");
                                return false;
                            }
                            if (RemoveCompanyCheckBox.Checked == false && totalCompany > 0 && companies.Count > 0 && (companies.Count != totalCompany) && !isRequiredForCompanyCheck)
                            {
                                ShowAlertMessage(AlertType.Error, "Please click retain role.");
                                return false;
                            }
                        }
                        if (RemoveCompanyCheckBox.Checked && totalCompany == 1)
                        {
                            ShowAlertMessage(AlertType.Error, "At least one Company is required for your existing business role(s). Please deselect any one of them.");
                            return false;
                        }
                        if (RemoveCompanyCheckBox.Checked == false && totalCompany > 1 && !isRequiredForCompanyCheck)
                        {
                            ShowAlertMessage(AlertType.Error, "Please click retain role.");
                            return false;
                        }
                    }
                }
                else
                {
                    if (CompanyAutoCompleteBox.Enabled == true && CompanyAutoCompleteBox.CheckBoxes == true && RemoveCompanyCheckBox.Checked == false && CompanyAutoCompleteBox.Items.Any(i => i.Checked == false) && SAPFlinsMatrixBusinessRules.HasSupplimentaryRole==true)
                    {
                        ShowAlertMessage(AlertType.Error, "Please click retain role.");
                        return false;
                    }
                    if (CompanyAutoCompleteBox.Enabled == true && CompanyAutoCompleteBox.CheckBoxes == false && RemoveCompanyCheckBox.Checked == false && totalCompany > 1 && !isRequiredForCompanyCheck 
                        && SAPFlinsMatrixBusinessRules.HasSupplimentaryRole == true)
                    {
                        ShowAlertMessage(AlertType.Error, "Please click retain role.");
                        return false;
                    }
                }
            }
            IList<RadComboBoxItem> companyText = CompanyAutoCompleteBox.CheckedItems;
            List<AdHocSAPHRFLINSRequestDetail> listofrequest = new List<AdHocSAPHRFLINSRequestDetail>();
            AdHocSAPHRFLINSRequestDetail mulitpleRequestDetails = null;
            if (CompanyAutoCompleteBox.Enabled == true)
            {
                if (CompanyAutoCompleteBox.CheckBoxes == true)
                {
                    foreach (RadComboBoxItem checkeditems in companyText)
                    {
                        mulitpleRequestDetails = getAdHocSAPHRFLINSRequestDetail(companyText.Count() == 0 ? null : checkeditems.Text);

                        listofrequest.Add(mulitpleRequestDetails);
                    }
                }
                else
                {
                    mulitpleRequestDetails = getAdHocSAPHRFLINSRequestDetail(CompanyAutoCompleteBox.Text);
                    listofrequest.Add(mulitpleRequestDetails);
                }
            }
            else
            {
                mulitpleRequestDetails = new AdHocSAPHRFLINSRequestDetail
                {
                    Module = ModuleDropDownList.SelectedIndex > 0 ? ModuleDropDownList.SelectedItem.Text : null,
                    SubModule = SubModuleDropDownList.SelectedIndex > 0 ? SubModuleDropDownList.SelectedItem.Text : null,
                    EntitlementType = EntitlementType.Role,
                    Entitlement = BusinessRoleAutoCompleteBox.Entries.Count > 0 ? BusinessRoleAutoCompleteBox.Entries[0].Text : "",
                };
                listofrequest.Add(mulitpleRequestDetails);
            }
            if (listofrequest != null && listofrequest.Count > 0)
            {

                var flag = true;
                var entitlementList = EntitlementRequest;
                listofrequest.ForEach(
                    d =>
                    {

                        var exist = entitlementList.Any(e =>
                     {

                         if (!string.IsNullOrWhiteSpace(d.Company))
                         {
                             return e.Entitlement == d.Entitlement && (!string.IsNullOrWhiteSpace(e.Company) && !string.IsNullOrWhiteSpace(d.Company) && e.Company == d.Company) &&
                            (e.Company == d.Company && e.Plant == d.Plant && e.CostCenter == d.CostCenter && e.MRPController == d.MRPController && e.PurchaseOrg == d.PurchaseOrg && e.PurchaseGroup == d.PurchaseGroup);

                         }
                         else
                         {
                             return e.Entitlement == d.Entitlement && (
                            (e.Company == d.Company && e.Plant == d.Plant && e.CostCenter == d.CostCenter && e.MRPController == d.MRPController && e.PurchaseOrg == d.PurchaseOrg && e.PurchaseGroup == d.PurchaseGroup)
                            );
                         }

                     });

                        if (exist)
                        {
                            ShowAlertMessage(AlertType.Error, "The business role is already in the request list.");
                            flag = false;
                        }

                    });

                if (!flag)
                {
                    return flag;
                }


            }


            if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove &&  SAPFlinsMatrixBusinessRules.HasSupplimentaryRole == true)
            {

                if (CompanyAutoCompleteBox.CheckBoxes == true && CompanyAutoCompleteBox.Enabled == true)
                {
                    var msg = string.Empty;
                    var allChecked = CompanyAutoCompleteBox.Items.All(i => i.Checked == true);
                    if (RemoveCompanyCheckBox.Checked == true && allChecked)
                    {
                        msg = "At least one Company is required for your existing business role(s). Please deselect any one of them.";
                        ShowAlertMessage(AlertType.Error, msg);
                        return false;
                    }
                    if (RemoveCompanyCheckBox.Checked == false && CompanyAutoCompleteBox.Items.Any(i => i.Checked == false))
                    {
                        ShowAlertMessage(AlertType.Error, "Please click retain role.");
                        return false;
                    }
                }

                if (PurchaseGroupAutoCompleteBox.Enabled == true)
                {
                    if (PurchaseGroupAutoCompleteBox.Items.Count() == 1 && SAPFlinsMatrixBusinessRules.ShouldClearAllPurchaseGroup)
                    {
                        if (RemoveCompanyCheckBox.Checked == false && PurchaseGroupAutoCompleteBox.CheckedItems.Count() == 0)
                        {
                            ShowAlertMessage(AlertType.Error, "Please select all of the purchase groups. No more business role required purchase group.");
                            return false;
                        }
                    }
                    else
                    {
                        //if (RemoveCompanyCheckBox.Checked == true && PurchaseGroupAutoCompleteBox.Items.Count() == PurchaseGroupAutoCompleteBox.CheckedItems.Count())
                        //{
                        //    var msg = "At least one Purchase Group is required for your existing business role(s). Please deselect any one of them.";
                        //    if (RemoveCompanyCheckBox.Checked == true)
                        //    {
                        //        msg = "At least one Purchase Group is required to retain the business role. Please deselect any one of them.";
                        //    }
                        //    ShowAlertMessage(AlertType.Error, msg);
                        //    return false;
                        //}

                        //if (PurchaseGroupAutoCompleteBox.Items.Count() != PurchaseGroupAutoCompleteBox.CheckedItems.Count() && RemoveCompanyCheckBox.Checked == false)
                        //{
                        //    ShowAlertMessage(AlertType.Error, "Please select all of the purchase groups. No more business role required purchase group.");
                        //    return false;
                        //}
                        //if (PurchaseGroupAutoCompleteBox.Items.Count() == PurchaseGroupAutoCompleteBox.CheckedItems.Count() && RemoveCompanyCheckBox.Checked == false)
                        //{
                        //    var bRole = BusinessRoleAutoCompleteBox.Entries.Count > 0 ? BusinessRoleAutoCompleteBox.Entries[0].Text : "";
                        //    var EndpointUserId = Convert.ToInt32(EndPointUserDropDownList.SelectedValue);
                        //    var parameter = new Dictionary<string, string> { { "EndpointUserId", EndpointUserId.ToString() } };
                        //    var sapFlinsMatrixBusinessRules = DomainApiClient<SAPRoleMandatoryField>
                        //        .GetByAction<List<Dto.SAPHRFLINSRoleMatrix>>(DomainApiAction.Matrix.SAP_FLINS.SAPRoleMandatoryFields.GetAllExistingBusinessRolesOnlyPurchaseGroup, parameter);
                        //    var allPurchaseGroup = sapFlinsMatrixBusinessRules.Where(i => i.BusinessRole.Equals(bRole, StringComparison.InvariantCultureIgnoreCase)).ToList();
                        //    var pGroups = allPurchaseGroup.Select(i => UtilityMethods.GetListFromString(i.PurchasingGrp));
                        //    if (EntitlementRequest != null && EntitlementRequest.Count > 0)
                        //    {
                        //        var exisitingPurchaseGroup = EntitlementRequest.Where(i => !string.IsNullOrWhiteSpace(i.PurchaseGroup));

                        //    }
                        //}
                        if (PurchaseGroupAutoCompleteBox.Enabled == true && (SAPFlinsMatrixBusinessRules != null || RemoveCompanyCheckBox.Checked))
                        {
                            if (PurchaseGroupAutoCompleteBox.Items.Count() == PurchaseGroupAutoCompleteBox.CheckedItems.Count() && (RemoveCompanyCheckBox.Checked == true || !SAPFlinsMatrixBusinessRules.CanClearAllPurchaseGrp))
                            {
                                var msg = "At least one PurchaseGroup is required for your existing business role(s). Please deselect any one of them.";
                                if (RemoveCompanyCheckBox.Checked == true)
                                {
                                    msg = "At least one PurchaseGroup is required to retain the business role. Please deselect any one of them.";
                                }
                                ShowAlertMessage(AlertType.Error, msg);
                                return false;
                            }

                            if (PurchaseGroupAutoCompleteBox.Items.Count() != PurchaseGroupAutoCompleteBox.CheckedItems.Count() && SAPFlinsMatrixBusinessRules != null && SAPFlinsMatrixBusinessRules.ShouldClearAllPurchaseGroup && RemoveCompanyCheckBox.Checked == false)
                            {
                                ShowAlertMessage(AlertType.Error, "Please select all of the PurchaseGroup. No other business role required.");
                                return false;
                            }
                        }

                    }

                }

                if (CostCenterAutoCompleteBox.Enabled == true)
                {
                    if (//CostCenterAutoCompleteBox.Items.Count()> 1 && 
                        CostCenterAutoCompleteBox.Items.Count() == CostCenterAutoCompleteBox.CheckedItems.Count() && (RemoveCompanyCheckBox.Checked == true || !SAPFlinsMatrixBusinessRules.CanClearAllCostCenter))
                    {
                        //var msg = "At least one Cost Center is required to retain the business role. Please deselect any one of them.";
                        //ShowAlertMessage(AlertType.Error, msg);
                        //return false;

                        var msg = "At least one Cost Center is required for your existing business role(s). Please deselect any one of them.";
                        if (RemoveCompanyCheckBox.Checked == true)
                        {
                            msg = "At least one Cost Center is required to retain the business role. Please deselect any one of them.";
                        }
                        ShowAlertMessage(AlertType.Error, msg);
                        return false;
                    }

                    if (CostCenterAutoCompleteBox.Items.Count() != CostCenterAutoCompleteBox.CheckedItems.Count() && SAPFlinsMatrixBusinessRules.ShouldClearAllCostcenter && RemoveCompanyCheckBox.Checked == false)
                    {
                        ShowAlertMessage(AlertType.Error, "Please select all of the Cost Centers. No other business role required.");
                        return false;
                    }

                }

                if (PlanetAutoCompleteBox.Enabled == true)
                {
                    if (PlanetAutoCompleteBox.Items.Count() == PlanetAutoCompleteBox.CheckedItems.Count() && RemoveCompanyCheckBox.Checked)
                    {
                        ShowAlertMessage(AlertType.Error, "At least one Plant is required to retain the business role. Please deselect any one of them.");
                        return false;
                    }

                    if (PlanetAutoCompleteBox.Items.Count() != PlanetAutoCompleteBox.CheckedItems.Count() && RemoveCompanyCheckBox.Checked == false)
                    {
                        ShowAlertMessage(AlertType.Error, "Please select all of the Plants. No other business role required.");
                        return false;
                    }
                }

                if (MRPControllerAutoCompleteBox.Enabled == true)
                {
                    if (MRPControllerAutoCompleteBox.Items.Count() == MRPControllerAutoCompleteBox.CheckedItems.Count() && RemoveCompanyCheckBox.Checked)
                    {
                        ShowAlertMessage(AlertType.Error, "At least one MRPControl is required to retain the business role. Please deselect any one of them.");
                        return false;
                    }

                    if (MRPControllerAutoCompleteBox.Items.Count() != MRPControllerAutoCompleteBox.CheckedItems.Count() && RemoveCompanyCheckBox.Checked == false)
                    {
                        ShowAlertMessage(AlertType.Error, "Please select all of the MRPControls. No other business role required.");
                        return false;
                    }
                }

                if (StoreLocationsAutoCompleteBox.Enabled == true)
                {
                    if (StoreLocationsAutoCompleteBox.Items.Count() == StoreLocationsAutoCompleteBox.CheckedItems.Count() && RemoveCompanyCheckBox.Checked)
                    {
                        ShowAlertMessage(AlertType.Error, "At least one StoreLocation is required to retain the business role. Please deselect any one of them.");
                        return false;
                    }

                    if (StoreLocationsAutoCompleteBox.Items.Count() != StoreLocationsAutoCompleteBox.CheckedItems.Count() && RemoveCompanyCheckBox.Checked == false)
                    {
                        ShowAlertMessage(AlertType.Error, "Please select all of the StoreLocations. No other business role required.");
                        return false;
                    }
                }

                if (PurchaseOrgAutoCompleteBox.Enabled == true)
                {
                    if (PurchaseOrgAutoCompleteBox.Items.Count() == PurchaseOrgAutoCompleteBox.CheckedItems.Count() && RemoveCompanyCheckBox.Checked)
                    {
                        ShowAlertMessage(AlertType.Error, "At least one StoreLocation is required to retain the business role. Please deselect any one of them.");
                        return false;
                    }

                    if (PurchaseOrgAutoCompleteBox.Items.Count() != PurchaseOrgAutoCompleteBox.CheckedItems.Count() && RemoveCompanyCheckBox.Checked == false)
                    {
                        ShowAlertMessage(AlertType.Error, "Please select all of the PurchaseOrgs. No other business role required.");
                        return false;
                    }
                }
            }
            return true;

        }
        //protected void BackButton_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (null != BackClicked)
        //        {
        //            if (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0)
        //            {
        //                Save_Click(null, null);
        //                Session["AdhocrequestObj"] = AdhocRequest;
        //            }
        //            BackClicked();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }

        //}
        private void ClearComboBox(RadComboBox Control)
        {
            foreach (RadComboBoxItem items in Control.Items)
            {
                items.Checked = false;
            }
        }

        protected void EndPointUserDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            afterSave();
            Session["EndpointUserId"] = null;
            if (EndPointUserDropDownList.SelectedItem != null && EndPointUserDropDownList.SelectedIndex > 0 && (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
            {
                Session["EndpointUserId"] = Convert.ToInt32(EndPointUserDropDownList.SelectedValue);
            }
        }

        protected void ModuleDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CompanyAutoCompleteBox.Enabled = false;
                PurchaseOrgAutoCompleteBox.Enabled = false;
                PurchaseGroupAutoCompleteBox.Enabled = false;
                PlanetAutoCompleteBox.Enabled = false;
                StoreLocationsAutoCompleteBox.Enabled = false;
                MRPControllerAutoCompleteBox.Enabled = false;
                BusinessRoleAutoCompleteBox.Entries.Clear();
                CompanyAutoCompleteBox.Items.Clear();
                PurchaseOrgAutoCompleteBox.Items.Clear();
                PurchaseGroupAutoCompleteBox.Items.Clear();
                PlanetAutoCompleteBox.Items.Clear();
                StoreLocationsAutoCompleteBox.Items.Clear();
                MRPControllerAutoCompleteBox.Items.Clear();
                RemarksLabel.Text = "";
                if (ModuleDropDownList.SelectedIndex <= 0)
                {
                    Session["Module"] = null;
                    Session["SubModule"] = null;
                    SubModuleDropDownList.Items.Clear();
                    SubModuleDropDownList.Items.Insert(0, new ListItem("-- Select SubModule --", "-1"));
                }
                else
                {
                    Session["Module"] = ModuleDropDownList.SelectedItem.Text;
                    Session["SubModule"] = null;
                    BindSubModule();
                }
            }
            catch (Exception)
            {
            }
        }
        protected void CompanyAutoCompleteBox_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            try
            {
                if (CompanyAutoCompleteBox.CheckBoxes == false)
                {

                    Dictionary<long, string> dataSource = new Dictionary<long, string>();
                    int EndpointUserId = 0;
                    if (EndPointUserDropDownList.SelectedItem != null && EndPointUserDropDownList.SelectedIndex > 0 && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        EndpointUserId = Convert.ToInt32(EndPointUserDropDownList.SelectedValue);
                    }

                    var param = new Dictionary<string, string>{
                         { "mandatory1string", Mandatory1HiddenField.Value },
                         { "mandatory2string",Mandatory2HiddenField.Value },
                        {"CostCenter","" }, {"businessRoleID",BusinessRoleAutoCompleteBox.Text.Replace(";", "").Trim().Trim()},
                        {"Company", CompanyAutoCompleteBox.Text.Replace(",", "").Trim()} ,
                        { "PurchaseOrg",""},

                        { "PurchaseGroup","" },{"Planet","" },{"StorageLocation","" },{"MRPControl","" },{"EndpointUserId",EndpointUserId.ToString() } };

                    dataSource = DomainApiClient<Dto.SAPHRFLINSRoleMatrix>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetMandatory2DataSource, param);

                    switch (Mandatory2HiddenField.Value)
                    {
                        case "Org":
                            {
                                PurchaseOrgAutoCompleteBox.Enabled = true;
                                PurchaseOrgAutoCompleteBox.DataSource = dataSource;
                                PurchaseOrgAutoCompleteBox.DataTextField = "Value";
                                PurchaseOrgAutoCompleteBox.DataValueField = "Key";
                                PurchaseOrgAutoCompleteBox.DataBind();
                                divPurchasingOrg.Attributes.Add("class", "form-group required");
                                return;
                            }
                        case "CostCenter":
                            {
                                if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                                    loadValidationRules(true);                                 
                                CostCenterAutoCompleteBox.Enabled = true;
                                CostCenterAutoCompleteBox.DataSource = dataSource;
                                CostCenterAutoCompleteBox.DataTextField = "Value";
                                CostCenterAutoCompleteBox.DataValueField = "Key";
                                CostCenterAutoCompleteBox.DataBind();
                                if (!(SAPFlinsMatrixBusinessRules.CanClearAllCostCenter == false && CostCenterAutoCompleteBox.Items.Count == 1))
                                {
                                    divCostCenter.Attributes.Add("class", "form-group required");
                                }
                                else
                                {
                                    divCostCenter.Attributes.Add("class", "form-group");
                                }
                                return;
                            }
                        case "PurchaseGroup":
                            {
                                if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                                    loadValidationRules(false);
                                PurchaseGroupAutoCompleteBox.Enabled = true;
                                PurchaseGroupAutoCompleteBox.DataSource = dataSource.OrderBy(d => d.Value).Select(d => d).ToDictionary(d => d.Key, d => d.Value);
                                PurchaseGroupAutoCompleteBox.DataTextField = "Value";
                                PurchaseGroupAutoCompleteBox.DataValueField = "Key";
                                PurchaseGroupAutoCompleteBox.DataBind();
                                divPgroup.Attributes.Add("class", "form-group required");
                                if (this.AdhocRequest != null && this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove && dataSource != null && dataSource.Count == 1)
                                {
                                    RemoveCompanyCheckBox.Checked = false;
                                    RemoveCompanyCheckBox.Enabled = false;

                                    if(SAPFlinsMatrixBusinessRules!=null  && SAPFlinsMatrixBusinessRules.ShouldClearAllPurchaseGroup==false)
                                       divPgroup.Attributes.Add("class", "form-group");
                                }
                                else
                                {
                                    RemoveCompanyCheckBox.Enabled = true;
                                    
                                }
                                return;
                            }
                        case "Plant":
                            {
                                PlanetAutoCompleteBox.Enabled = true;
                                PlanetAutoCompleteBox.DataSource = dataSource;
                                PlanetAutoCompleteBox.DataTextField = "Value";
                                PlanetAutoCompleteBox.DataValueField = "Key";
                                PlanetAutoCompleteBox.DataBind();
                                divPlant.Attributes.Add("class", "form-group required");
                                return;
                            }
                        case "StorageLocation":
                            {
                                StoreLocationsAutoCompleteBox.Enabled = true;
                                StoreLocationsAutoCompleteBox.DataSource = dataSource;
                                StoreLocationsAutoCompleteBox.DataTextField = "Value";
                                StoreLocationsAutoCompleteBox.DataValueField = "Key";
                                StoreLocationsAutoCompleteBox.DataBind();
                                divStoreLocation.Attributes.Add("class", "form-group required");
                                return;
                            }
                        case "MRPController":
                            {
                                MRPControllerAutoCompleteBox.Enabled = true;
                                MRPControllerAutoCompleteBox.DataSource = dataSource;
                                MRPControllerAutoCompleteBox.DataTextField = "Value";
                                MRPControllerAutoCompleteBox.DataValueField = "Key";
                                MRPControllerAutoCompleteBox.DataBind();
                                divMRPController.Attributes.Add("class", "form-group required");
                                return;
                            }
                    }
                }
            }
            catch (Exception ee)
            {
            }
        }

        protected void SubModuleDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            BusinessRoleAutoCompleteBox.Entries.Clear();
            CompanyAutoCompleteBox.Enabled = false;
            PurchaseOrgAutoCompleteBox.Enabled = false;
            PurchaseGroupAutoCompleteBox.Enabled = false;
            PlanetAutoCompleteBox.Enabled = false;
            StoreLocationsAutoCompleteBox.Enabled = false;
            MRPControllerAutoCompleteBox.Enabled = false;
            BusinessRoleAutoCompleteBox.Entries.Clear();
            CompanyAutoCompleteBox.Items.Clear();
            PurchaseOrgAutoCompleteBox.Items.Clear();
            PurchaseGroupAutoCompleteBox.Items.Clear();
            PlanetAutoCompleteBox.Items.Clear();
            StoreLocationsAutoCompleteBox.Items.Clear();
            MRPControllerAutoCompleteBox.Items.Clear();
            RemarksLabel.Text = "";
            ClearMandatorField();
            if (SubModuleDropDownList.SelectedIndex > 0)
            {
                Session["SubModule"] = SubModuleDropDownList.SelectedItem.Text;
            }
            else
            {
                Session["SubModule"] = null;
            }
        }



        #region Methods
        private void BindModule(long UserID)
        {
            try
            {
                IList<SAPHRFLINSRoleMatrix> data = DomainApiClient<Dto.SAPHRFLINSRoleMatrix>.GetByAction<IList<Dto.SAPHRFLINSRoleMatrix>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.BindModule, new Dictionary<string, string> { { "userID", UserID.ToString() } });
                ModuleDropDownList.Items.Clear();
                ModuleDropDownList.DataSource = data;
                ModuleDropDownList.DataTextField = "Module";
                ModuleDropDownList.DataValueField = "Id";
                ModuleDropDownList.DataBind();
                ModuleDropDownList.Items.Insert(0, new ListItem("-- Select Module --", "-1"));
                SubModuleDropDownList.Items.Clear();
                SubModuleDropDownList.Items.Insert(0, new ListItem("-- Select SubModule --", "-1"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindSubModule()
        {
            try
            {

                var module = ModuleDropDownList.SelectedItem.Text;
                var userId = AdhocRequest.RequesteeUserId.ToString();
                IList<SAPHRFLINSRoleMatrix> data = DomainApiClient<Dto.SAPHRFLINSRoleMatrix>.GetByAction<IList<Dto.SAPHRFLINSRoleMatrix>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.BindSubModule, new Dictionary<string, string> { { "moduleName", module }, { "UserID", userId } });
                SubModuleDropDownList.Items.Clear();
                SubModuleDropDownList.DataSource = data;
                SubModuleDropDownList.DataTextField = "SubModule";
                SubModuleDropDownList.DataValueField = "Id";
                SubModuleDropDownList.DataBind();
                SubModuleDropDownList.Items.Insert(0, new ListItem("-- Select SubModule --", "-1"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        AdHocSAPHRFLINSRequestDetail getAdHocSAPHRFLINSRequestDetail(string Companyname)
        {
            var requestDetails = new AdHocSAPHRFLINSRequestDetail();
            requestDetails.Module = ModuleDropDownList.SelectedIndex > 0 ? ModuleDropDownList.SelectedItem.Text : null;
            requestDetails.SubModule = SubModuleDropDownList.SelectedIndex > 0 ? SubModuleDropDownList.SelectedItem.Text : null;
            requestDetails.EntitlementType = EntitlementType.Role;
            requestDetails.Entitlement = BusinessRoleAutoCompleteBox.Entries.Count > 0 ? BusinessRoleAutoCompleteBox.Entries[0].Text : "";
            requestDetails.Company = Companyname;
            requestDetails.RetainCompanyRoles = RemoveCompanyCheckBox.Checked;
            requestDetails.CostCenter = EntiltementString(CostCenterAutoCompleteBox);
            requestDetails.PurchaseOrg = EntiltementString(PurchaseOrgAutoCompleteBox);
            requestDetails.PurchaseGroup = EntiltementString(PurchaseGroupAutoCompleteBox);
            requestDetails.Plant = EntiltementString(PlanetAutoCompleteBox);
            requestDetails.StorageLocation = EntiltementString(StoreLocationsAutoCompleteBox);
            requestDetails.MRPController = EntiltementString(MRPControllerAutoCompleteBox);
            return requestDetails;
        }

        private AdHocSAPHRFLINSRequest GenerateRequest(bool CanAddRequest)
        {
            try
            {
                AdHocSAPHRFLINSRequest entitlementDetail = new AdHocSAPHRFLINSRequest();
                List<AdHocSAPHRFLINSRequestDetail> listofrequest = new List<AdHocSAPHRFLINSRequestDetail>();
                AdHocSAPHRFLINSRequestDetail mulitpleRequestDetails;
                AdHocSAPSharedAccount sharedDetails = new AdHocSAPSharedAccount();
                if (isServiceOrSharedAccountRequest() && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    if (SAPUserTypeDropDown.SelectedIndex > 0)
                    {
                        sharedDetails.SAPUserType = (SAPUserType)(Convert.ToByte(SAPUserTypeDropDown.SelectedValue));
                    }
                    sharedDetails.AccountName = txtAcntName.Text;
                    sharedDetails.Description = txtDiscription.Text;
                }
                IList<RadComboBoxItem> companyText = CompanyAutoCompleteBox.CheckedItems;
                if (IsDeleteRequest())
                {
                    mulitpleRequestDetails = new AdHocSAPHRFLINSRequestDetail();
                    listofrequest.Add(mulitpleRequestDetails);
                }
                else
                {
                    if (CompanyAutoCompleteBox.Enabled == true)
                    {
                        if (CompanyAutoCompleteBox.CheckBoxes == true)
                        {
                            foreach (RadComboBoxItem checkeditems in companyText)
                            {
                                mulitpleRequestDetails = getAdHocSAPHRFLINSRequestDetail(companyText.Count() == 0 ? null : checkeditems.Text);
                                listofrequest.Add(mulitpleRequestDetails);
                            }
                        }
                        else
                        {
                            mulitpleRequestDetails = getAdHocSAPHRFLINSRequestDetail(CompanyAutoCompleteBox.Text);
                            listofrequest.Add(mulitpleRequestDetails);
                        }
                    }
                    else
                    {
                        mulitpleRequestDetails = new AdHocSAPHRFLINSRequestDetail
                        {
                            Module = ModuleDropDownList.SelectedIndex > 0 ? ModuleDropDownList.SelectedItem.Text : null,
                            SubModule = SubModuleDropDownList.SelectedIndex > 0 ? SubModuleDropDownList.SelectedItem.Text : null,
                            EntitlementType = EntitlementType.Role,
                            Entitlement = BusinessRoleAutoCompleteBox.Entries.Count > 0 ? BusinessRoleAutoCompleteBox.Entries[0].Text : "",
                        };
                        listofrequest.Add(mulitpleRequestDetails);
                    }
                }
                entitlementDetail = new AdHocSAPHRFLINSRequest
                {
                    EndpointUserId = (EndPointUserDropDownList.SelectedIndex > 0) ? Convert.ToInt32(EndPointUserDropDownList.SelectedItem.Value) : 0,
                    AdHocSAPHRFLINSSharedAccount = isServiceOrSharedAccountRequest() && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount ? sharedDetails : null
                    //FLINSMatrixId = 

                };

                if (EndPointUserDropDownList.SelectedItem != null)
                {
                    entitlementDetail.DisplayAccountName = EndPointUserDropDownList.SelectedItem.Text;
                }


                if (CanAddRequest)
                {
                    EntitlementRequest = EntitlementRequest;
                    listofrequest.ForEach(lq =>
                    {
                        var exist = EntitlementRequest.Any(l => l == lq);
                        if (isServiceOrSharedAccountRequest() && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                        {
                            if (!exist && !string.IsNullOrEmpty(lq.Entitlement))
                            {
                                if (!string.IsNullOrWhiteSpace(lq.Company))
                                {
                                    var matchedRequest = EntitlementRequest.FirstOrDefault(EntitlementReq => EntitlementReq.Entitlement.Equals(lq.Entitlement, StringComparison.InvariantCultureIgnoreCase) && (!string.IsNullOrWhiteSpace(EntitlementReq.Company) && EntitlementReq.Company.Equals(lq.Company, StringComparison.InvariantCultureIgnoreCase)));
                                    if (matchedRequest != null)
                                    {
                                        if (MRPControllerAutoCompleteBox.Enabled)
                                        {
                                            if (!string.IsNullOrWhiteSpace(lq.MRPController) && !string.IsNullOrWhiteSpace(matchedRequest.MRPController))
                                            {
                                                var existing = UtilityMethods.GetListFromString(matchedRequest.MRPController);
                                                var newList = UtilityMethods.GetListFromString(lq.MRPController);
                                                existing.AddRange(newList);
                                                existing = existing.Distinct().ToList();
                                                matchedRequest.MRPController = UtilityMethods.GetStringFromList(existing);
                                            }
                                        }
                                        if (PurchaseOrgAutoCompleteBox.Enabled)
                                        {
                                            if (!string.IsNullOrWhiteSpace(lq.PurchaseOrg) && !string.IsNullOrWhiteSpace(matchedRequest.PurchaseOrg))
                                            {
                                                var existing = UtilityMethods.GetListFromString(matchedRequest.PurchaseOrg);
                                                var newList = UtilityMethods.GetListFromString(lq.PurchaseOrg);
                                                existing.AddRange(newList);
                                                existing = existing.Distinct().ToList();
                                                matchedRequest.PurchaseOrg = UtilityMethods.GetStringFromList(existing);
                                            }
                                        }
                                        if (PurchaseGroupAutoCompleteBox.Enabled)
                                        {
                                            if (!string.IsNullOrWhiteSpace(lq.PurchaseGroup) && !string.IsNullOrWhiteSpace(matchedRequest.PurchaseGroup))
                                            {
                                                var existing = UtilityMethods.GetListFromString(matchedRequest.PurchaseGroup);
                                                var newList = UtilityMethods.GetListFromString(lq.PurchaseGroup);
                                                existing.AddRange(newList);
                                                existing = existing.Distinct().ToList();
                                                matchedRequest.PurchaseGroup = UtilityMethods.GetStringFromList(existing);
                                            }
                                        }
                                        if (StoreLocationsAutoCompleteBox.Enabled)
                                        {
                                            if (!string.IsNullOrWhiteSpace(lq.StorageLocation) && !string.IsNullOrWhiteSpace(matchedRequest.StorageLocation))
                                            {
                                                var existing = UtilityMethods.GetListFromString(matchedRequest.StorageLocation);
                                                var newList = UtilityMethods.GetListFromString(lq.StorageLocation);
                                                existing.AddRange(newList);
                                                existing = existing.Distinct().ToList();
                                                matchedRequest.StorageLocation = UtilityMethods.GetStringFromList(existing);
                                            }
                                        }
                                        if (PlanetAutoCompleteBox.Enabled)
                                        {
                                            if (!string.IsNullOrWhiteSpace(lq.Plant) && !string.IsNullOrWhiteSpace(matchedRequest.Plant))
                                            {
                                                var existing = UtilityMethods.GetListFromString(matchedRequest.Plant);
                                                var newList = UtilityMethods.GetListFromString(lq.Plant);
                                                existing.AddRange(newList);
                                                existing = existing.Distinct().ToList();
                                                matchedRequest.Plant = UtilityMethods.GetStringFromList(existing);
                                            }
                                        }
                                        if (CostCenterAutoCompleteBox.Enabled)
                                        {
                                            if (!string.IsNullOrWhiteSpace(lq.CostCenter) && !string.IsNullOrWhiteSpace(matchedRequest.CostCenter))
                                            {
                                                var existing = UtilityMethods.GetListFromString(matchedRequest.CostCenter);
                                                var newList = UtilityMethods.GetListFromString(lq.CostCenter);
                                                existing.AddRange(newList);
                                                existing = existing.Distinct().ToList();
                                                matchedRequest.CostCenter = UtilityMethods.GetStringFromList(existing);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        EntitlementRequest.Add(lq);
                                    }
                                }
                                else
                                {
                                    EntitlementRequest.Add(lq);
                                }
                            }
                        }
                        else
                        {
                            if (!exist && (!string.IsNullOrEmpty(lq.Entitlement)))
                            {
                                if (!string.IsNullOrWhiteSpace(lq.Company))
                                {
                                    var matchedRequest = EntitlementRequest.FirstOrDefault(EntitlementReq => EntitlementReq.Entitlement.Equals(lq.Entitlement, StringComparison.InvariantCultureIgnoreCase) && (!string.IsNullOrWhiteSpace(EntitlementReq.Company) && EntitlementReq.Company.Equals(lq.Company, StringComparison.InvariantCultureIgnoreCase)));
                                    if (matchedRequest != null)
                                    {
                                        if (MRPControllerAutoCompleteBox.Enabled)
                                        {
                                            if (!string.IsNullOrWhiteSpace(lq.MRPController) && !string.IsNullOrWhiteSpace(matchedRequest.MRPController))
                                            {
                                                var existing = UtilityMethods.GetListFromString(matchedRequest.MRPController);
                                                var newList = UtilityMethods.GetListFromString(lq.MRPController);
                                                existing.AddRange(newList);
                                                existing = existing.Distinct().ToList();
                                                matchedRequest.MRPController = UtilityMethods.GetStringFromList(existing);
                                            }
                                        }
                                        if (PurchaseOrgAutoCompleteBox.Enabled)
                                        {
                                            if (!string.IsNullOrWhiteSpace(lq.PurchaseOrg) && !string.IsNullOrWhiteSpace(matchedRequest.PurchaseOrg))
                                            {
                                                var existing = UtilityMethods.GetListFromString(matchedRequest.PurchaseOrg);
                                                var newList = UtilityMethods.GetListFromString(lq.PurchaseOrg);
                                                existing.AddRange(newList);
                                                existing = existing.Distinct().ToList();
                                                matchedRequest.PurchaseOrg = UtilityMethods.GetStringFromList(existing);
                                            }
                                        }
                                        if (PurchaseGroupAutoCompleteBox.Enabled)
                                        {
                                            if (!string.IsNullOrWhiteSpace(lq.PurchaseGroup) && !string.IsNullOrWhiteSpace(matchedRequest.PurchaseGroup))
                                            {
                                                var existing = UtilityMethods.GetListFromString(matchedRequest.PurchaseGroup);
                                                var newList = UtilityMethods.GetListFromString(lq.PurchaseGroup);
                                                existing.AddRange(newList);
                                                existing = existing.Distinct().ToList();
                                                matchedRequest.PurchaseGroup = UtilityMethods.GetStringFromList(existing);
                                            }
                                        }
                                        if (StoreLocationsAutoCompleteBox.Enabled)
                                        {
                                            if (!string.IsNullOrWhiteSpace(lq.StorageLocation) && !string.IsNullOrWhiteSpace(matchedRequest.StorageLocation))
                                            {
                                                var existing = UtilityMethods.GetListFromString(matchedRequest.StorageLocation);
                                                var newList = UtilityMethods.GetListFromString(lq.StorageLocation);
                                                existing.AddRange(newList);
                                                existing = existing.Distinct().ToList();
                                                matchedRequest.StorageLocation = UtilityMethods.GetStringFromList(existing);
                                            }
                                        }
                                        if (PlanetAutoCompleteBox.Enabled)
                                        {
                                            if (!string.IsNullOrWhiteSpace(lq.Plant) && !string.IsNullOrWhiteSpace(matchedRequest.Plant))
                                            {
                                                var existing = UtilityMethods.GetListFromString(matchedRequest.Plant);
                                                var newList = UtilityMethods.GetListFromString(lq.Plant);
                                                existing.AddRange(newList);
                                                existing = existing.Distinct().ToList();
                                                matchedRequest.Plant = UtilityMethods.GetStringFromList(existing);
                                            }
                                        }
                                        if (CostCenterAutoCompleteBox.Enabled)
                                        {
                                            if (!string.IsNullOrWhiteSpace(lq.CostCenter) && !string.IsNullOrWhiteSpace(matchedRequest.CostCenter))
                                            {
                                                var existing = UtilityMethods.GetListFromString(matchedRequest.CostCenter);
                                                var newList = UtilityMethods.GetListFromString(lq.CostCenter);
                                                existing.AddRange(newList);
                                                existing = existing.Distinct().ToList();
                                                matchedRequest.CostCenter = UtilityMethods.GetStringFromList(existing);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        EntitlementRequest.Add(lq);
                                    }
                                }
                                else
                                {
                                    EntitlementRequest.Add(lq);
                                }
                            }
                            if (IsDeleteRequest() && !exist)
                                EntitlementRequest.Add(lq);
                        }
                    });
                }
                return entitlementDetail;
            }
            catch
            {
                throw;
            }
        }
        private string EntiltementString(object Control)
        {
            string entiltment = "";
            RadComboBox control = (RadComboBox)Control;
            foreach (RadComboBoxItem checkedItems in control.CheckedItems)
            {
                entiltment = entiltment == "" ? checkedItems.Text.Trim() : entiltment + "," + checkedItems.Text.Trim();
            }
            return entiltment;
        }

        private void BindEndpointUsers(long userId, long endpointSystemId, AccountType accountType)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("userid", userId.ToString());
            dict.Add("endpointsystemid", endpointSystemId.ToString());
            dict.Add("accountType", Convert.ToString((int)accountType));
            AccountReconDto list = DomainApiClient<Dto.ADSUserInfo>.GetByAction<AccountReconDto>("GetADUsersByDIUserId", dict);

            if (list != null && list.AccountReconDictionary != null)
            {
                EndPointUserDropDownList.Items.Clear();
                DropDownHelper.DropDownListBind(EndPointUserDropDownList, list.AccountReconDictionary, "Value", "Key", null, true, "-- Select EndPoint User --", false);

                if (list.AccountReconDictionary.Count() == 1)
                {
                    EndPointUserDropDownList.SelectedIndex = 1;
                }

                Session["EndpointUserId"] = null;



                if (EndPointUserDropDownList.SelectedItem != null && EndPointUserDropDownList.SelectedIndex > 0 && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    Session["EndpointUserId"] = Convert.ToInt32(EndPointUserDropDownList.SelectedValue);
                }
            }
        }

        private void BindSAPUserType(AccountType accountType)
        {
            try
            {
                txtDiscription.Enabled = true;
                SAPUserTypeDropDown.Items.Clear();
                SAPUserTypeDropDown.Items.Insert(0, new ListItem("-- Select SAP UserType --", "-1"));

                foreach (SAPUserType item in Enum.GetValues(typeof(SAPUserType)))
                {
                    if (item == SAPUserType.Reference || (accountType == AccountType.Shared && item == SAPUserType.Communication))
                        continue;

                    var description = UtilityMethods.GetEnumAttribute<Description>(item);
                    ListItem listItem = new ListItem(description, ((byte)item).ToString());
                    if (accountType == AccountType.Service && (((SAPUserType)item) == SAPUserType.Service || ((SAPUserType)item) == SAPUserType.System))
                    {
                        SAPUserTypeDropDown.Items.Add(listItem);
                    }
                    if ((accountType == AccountType.Shared || accountType == AccountType.Secondary) && item == SAPUserType.Dialog)
                    {
                        SAPUserTypeDropDown.Items.Add(listItem);

                        SAPUserTypeDropDown.SelectedIndex = 1;
                    }

                    if (accountType == AccountType.Secondary)
                    {
                        User usr = DomainApiClient<User>.GetById(this.AdhocRequest.RequesteeUserId);
                        if (usr != null)
                        {
                            txtDiscription.Text = usr.KnownAs;
                            txtDiscription.Enabled = false;
                        }

                    }
                }
            }
            catch
            {
                throw;
            }
        }
        private bool ValidationMessage()
        {
            try
            {
                if (AdhocRequest != null)
                {
                    if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && isServiceOrSharedAccountRequest())
                    {
                        var endpointSystemId = AdhocRequest.EndpointSystemId.HasValue ? AdhocRequest.EndpointSystemId.Value.ToString() : "0";
                        bool isExists = DomainApiClient<Dto.SAPUserinfo>.GetByAction<bool>(DomainApiAction.Endpoint.SAP.SAPUserinfo.DoesExists, new Dictionary<string, string> { { "userName", txtAcntName.Text.Trim() }, { "endpointsystemId", endpointSystemId } });
                        string specialCharacter = "!@#$%^&*()+=[]\\\';,./{}|\":<>?";

                        if (isExists)
                        {
                            ShowAlertMessage(AlertType.Error, "Account Name already exist, please try with another name.");
                            return false;
                        }
                        else
                        {
                            for (int i = 0; i < specialCharacter.Length; i++)
                            {
                                if (txtAcntName.Text.Contains(specialCharacter[i]))
                                {
                                    ShowAlertMessage(AlertType.Error, "Special characters not allowed in Account Name.");
                                    return false;
                                }
                            }
                        }
                        if (txtAcntName.Text.Any(x => Char.IsWhiteSpace(x)))
                        {
                            ShowAlertMessage(AlertType.Error, "Space is not allowed in Account Name.");
                            return false;
                        }
                        if (txtAcntName.Text.Length < MasterConfigHelper.AccountNameMinimumLength)
                        {
                            ShowAlertMessage(AlertType.Error, "Account Name is too Short.");
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }
        private void LoadControl()
        {
            AdHocAccessRequest request = AdhocRequest;
            if (IsAmendRequest())
            {
                trEndPointUser.Visible = true;
                SharedPanelHRMSFLINS.Visible = false;
                BindEndpointUsers(request.RequesteeUserId, request.EndpointSystemId.Value, request.AccountType);
                if (ModuleDropDownList.SelectedIndex <= 0)
                {
                    BindModule(request.RequesteeUserId);
                }
            }
            else if (IsDeleteRequest())
            {
                OtherDetailPanel.Visible = false;
                trEndPointUser.Visible = true;
                DeleteAccountAdd.Visible = false;
                SharedPanelHRMSFLINS.Visible = false;

                BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
            }
            else if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                OtherDetailPanel.Visible = true;
                trEndPointUser.Visible = false;
                DeleteAccountAdd.Text = "Add";
                if (ModuleDropDownList.SelectedIndex <= 0)
                {
                    BindModule(AdhocRequest.RequesteeUserId);
                }
            }

        }


        #endregion

        protected void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidationMessage())
                {

                    //GroupPanelHRMSFLINS.Visible = true;
                    //SharedPanelHRMSFLINS.Visible = false;
                    LoadControl();
                }
            }
            catch
            {
            }
        }

        protected void BusinessRoleAutoCompleteBox__ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (this.BusinessRoleAutoCompleteBox.Entries.Count == 0)
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }
        //protected void btnBack_Click(object sender, EventArgs e)
        //{
        //    BackClicked();
        //}

        protected void sapuserselectionchanged(object sender, EventArgs e)
        {

        }
        protected void txtAcntName_TextChanged(object sender, EventArgs e)
        {

        }
        protected void RequestGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var requests = this.EntitlementRequest;
                requests.RemoveAt(e.Item.ItemIndex);
                this.EntitlementRequest = requests;
                FillRequestGrid();
                ClearControls();
            }
            if (e.CommandName == "ViewTechicalRole")
            {
                GridDataItem item = e.Item as GridDataItem;
                string role = Convert.ToString(item.GetDataKeyValue("Entitlement"));
                string company = Convert.ToString(item.GetDataKeyValue("Company"));
                if (string.IsNullOrWhiteSpace(company)) company = null;

                AdHocSAPHRFLINSRequest entiltement = new AdHocSAPHRFLINSRequest();
                entiltement.AdHocSAPHRFLINSRequestDetails.AddRange(EntitlementRequest);
                var detail = entiltement.AdHocSAPHRFLINSRequestDetails.FirstOrDefault(lq => lq.Entitlement == role && lq.Company == company);
                List<string> list = DomainApiClient<Dto.SAPHRFLINSRoleMatrix>.GetByAction<List<string>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetHrFlinsTechnicalRolesByAdhocRequestObject, null, detail);
                ViewTechnicalSource.DataSource = list;
                ViewTechnicalSource.DataBind();
                ViewTechnicalSourceWindow.VisibleOnPageLoad = true;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ViewTechnicalSourceWindow.VisibleOnPageLoad = false;
        }


        public override bool Validate()
        {

            if (EntitlementRequest.Count <= 0 && (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
            {
                ShowAlertMessage(AlertType.Error, Constants.addToGridWarning);
                return false;
            }

            if ((AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
            {
                if (EndPointUserDropDownList.SelectedIndex <= 0 && EntitlementRequest.Count <= 0)
                {
                    ShowAlertMessage(AlertType.Error, "Please Select EndPoint User and Business Role.");
                    return false;
                }
                else if (EndPointUserDropDownList.SelectedIndex <= 0)
                {
                    ShowAlertMessage(AlertType.Error, "Please Select EndPoint User.");
                    return false;
                }

            }
            if ((AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount && EndPointUserDropDownList.SelectedIndex <= 0))
            {
                ShowAlertMessage(AlertType.Error, "Please Select EndPoint User.");
                return false;
            }

            if ((AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && isServiceOrSharedAccountRequest()) && (SAPUserTypeDropDown.SelectedIndex <= 0 || string.IsNullOrEmpty(txtAcntName.Text) || EntitlementRequest.Count <= 0))
            {
                if (SAPUserTypeDropDown.SelectedIndex <= 0 && string.IsNullOrEmpty(txtAcntName.Text))
                {
                    ShowAlertMessage(AlertType.Error, "Please Select SAP UserType and Account Name.");
                    return false;
                }
                else if (SAPUserTypeDropDown.SelectedIndex <= 0)
                {
                    ShowAlertMessage(AlertType.Error, "Please Select SAP UserType.");
                    return false;
                }
                else if (string.IsNullOrEmpty(txtAcntName.Text))
                {
                    ShowAlertMessage(AlertType.Error, "Please Select Account Name.");
                    return false;
                }
                else if (EntitlementRequest.Count <= 0 && (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                {
                    ShowAlertMessage(AlertType.Error, Constants.addToGridWarning);
                    return false;
                }
            }

            if (AdhocRequest.AccountType == AccountType.Service && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (string.IsNullOrEmpty(AdhocRequest.SecurityQuestions))
                {
                    ShowAlertMessage(AlertType.Error, "Please select service account compliance check.");
                    return false;
                }
            }


            if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                var purchagseGrouponly = EntitlementRequest.Where(i => !string.IsNullOrWhiteSpace(i.PurchaseGroup)).ToList();
                if (purchagseGrouponly != null && purchagseGrouponly.Count > 0)
                {
                    var allPurchaseGroups = new List<string>();
                    foreach (var item in purchagseGrouponly)
                    {
                        allPurchaseGroups.AddRange(UtilityMethods.GetListFromString(item.PurchaseGroup));
                    }
                    allPurchaseGroups = allPurchaseGroups.Distinct(StringComparer.InvariantCultureIgnoreCase).ToList();

                    //Take existing purchase group
                    var param = new Dictionary<string, string>{
                         { "mandatory1string", string.Empty },
                         { "mandatory2string","PurchaseGroup" },
                        {"CostCenter","" }, {"businessRoleID",purchagseGrouponly.FirstOrDefault().Entitlement},
                        {"Company", string.Empty} ,
                        { "PurchaseOrg",""},
                        { "PurchaseGroup","" },{"Planet","" },{"StorageLocation","" },{"MRPControl","" },{"EndpointUserId",EndpointUserId} };

                    var dataSource = DomainApiClient<Dto.SAPHRFLINSRoleMatrix>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetMandatory2DataSource, param);
                    if (allPurchaseGroups.Count > 0 && dataSource != null && dataSource.Count > 0)
                    {

                        var tempList = dataSource.ToList().Select(i => i.Value).ToList();
                        var retainedRoles = purchagseGrouponly.Where(i => i.RetainCompanyRoles).ToList();
                        var isAllSelected = allPurchaseGroups.All(tempList.Contains) && allPurchaseGroups.Count == tempList.Count;
                        //All purchase group(s) are selected and business role were selected as retain.
                        if (isAllSelected && retainedRoles.Count > 0)
                        {
                            ShowAlertMessage(AlertType.Error, $"BusinessRole:{string.Join(",", retainedRoles.Select(i => i.Entitlement))} cannot be retained. Because all the purchasing groups are selected.");
                            return false;
                        }
                        //Check whether all purchase groups selected then check the business roles count match.
                        if (isAllSelected)
                        {
                            var parameter = new Dictionary<string, string>();
                            parameter.Add("EndpointUserId", EndpointUserId);
                            parameter.Add("CostCenterPurchaseGroup", "PurchaseGroup");
                            var businessRoles = DomainApiClient<Dto.SAPHRFLINSRoleMatrix>.GetByAction<List<Dto.SAPHRFLINSRoleMatrix>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetEndpointUserBusinessRoles, parameter);
                            var existingRoles = businessRoles.Select(i => new { BusinessRole = i.BusinessRole, Company = i.Company }).Distinct().ToList();
                            var addedRoles = purchagseGrouponly.Select(i=> new { BusinessRole = i.Entitlement, Company = i.Company }).Distinct().ToList();
                            //if(addedRoles.Count == 1 && existingRoles.Count >1 && allPurchaseGroups.Count == 1)
                            //{
                            //    return true;
                            //}
                            //var existingRoles = businessRoles.Select(i => $"{i.Company}-{i.BusinessRole}").Distinct().ToList();
                            //var addedRoles = purchagseGrouponly.Select(i => $"{i.Company}-{i.Entitlement}").Distinct().ToList();
                            //var anyRoleExists = existingRoles.Where(i => !addedRoles.Contains(i, StringComparer.InvariantCultureIgnoreCase)).ToList();
                            //if (anyRoleExists.Count > 0 && allPurchaseGroups.Count == 1)
                            //{
                            //    ShowAlertMessage(AlertType.Error, $"At least one PurchaseGroup is required for your existing business role(s). Please deselect any one of them.");
                            //    return false;
                            //}
                            if (!(existingRoles.All(b => EntitlementRequest.Any(e => e.Entitlement == b.BusinessRole && e.Company == b.Company) ) ))
                            {
                                ShowAlertMessage(AlertType.Error, $"At least one PurchaseGroup is required for your existing business role(s). Please deselect any one of them.");
                                return false;
                            }
                        }
                    }
                }


                
                var CostCenteronly = EntitlementRequest.Where(i => !string.IsNullOrWhiteSpace(i.CostCenter)).ToList();
                if (CostCenteronly != null && CostCenteronly.Count > 0)
                {
                     

                    foreach (var costcenterBusinessRole in CostCenteronly)
                    {

                        var selectedCostCenters = new List<string>();
                        selectedCostCenters.AddRange(UtilityMethods.GetListFromString(costcenterBusinessRole.CostCenter));

                        //Take existing Cost Center
                        var param = new Dictionary<string, string>{
                         { "mandatory1string", string.Empty },
                         { "mandatory2string","CostCenter" },
                        {"CostCenter","" }, {"businessRoleID", costcenterBusinessRole.Entitlement},
                        {"Company", string.Empty} ,
                        { "PurchaseOrg",""},
                        { "PurchaseGroup","" },{"Planet","" },{"StorageLocation","" },{"MRPControl","" },{"EndpointUserId",EndpointUserId} };

                        var dataSource = DomainApiClient<Dto.SAPHRFLINSRoleMatrix>.GetByAction<Dictionary<long, string>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetMandatory2DataSource, param);
                        if (selectedCostCenters.Count > 0 && dataSource != null && dataSource.Count > 0)
                        {

                            var tempList = dataSource.ToList().Select(i => i.Value).ToList();
                            // var retainedRoles = CostCenteronly.Where(i => i.RetainCompanyRoles).ToList();
                            var isAllSelected = tempList.All( selectedCostCenters.Contains);// && allCostCenters.Count == tempList.Count;
                             
                            if (isAllSelected)
                            {
                                var parameter = new Dictionary<string, string>();
                                parameter.Add("EndpointUserId", EndpointUserId);
                                parameter.Add("CostCenterPurchaseGroup", "CostCenter");
                                var businessRoles = DomainApiClient<Dto.SAPHRFLINSRoleMatrix>.GetByAction<List<Dto.SAPHRFLINSRoleMatrix>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetEndpointUserBusinessRolesIncudeSupplementary, parameter);
                                var selectedTechnicalRule = string.Empty;
                                var selectedEntitlement = businessRoles.FirstOrDefault(b => b.MainSupp== "supplement" && b.BusinessRole == costcenterBusinessRole.Entitlement);
                                if (selectedEntitlement != null)
                                    selectedTechnicalRule = selectedEntitlement.TechnicalRule;

                                var existingRoles = businessRoles.Where(i => i.TechnicalRule== selectedTechnicalRule && i.CostCenter =="X").Select(i => new { Rule = i.TechnicalRule, Company = i.Company, BusinessRole = i.BusinessRole }).Distinct().ToList();
                                var addedRoles = CostCenteronly.Select(i => new { BusinessRole = i.Entitlement, Company = i.Company }).Distinct().ToList();
                                

                                if (!(existingRoles.All(b => EntitlementRequest.Any(e => e.Entitlement == b.BusinessRole && e.Company == b.Company  ))))
                                {
                                    ShowAlertMessage(AlertType.Error, $"At least one Cost Center is required for your existing business role(s). Please deselect any one of them.");
                                    return false;
                                }

                                if ((existingRoles.Any(b => EntitlementRequest.Any(e => e.Entitlement == b.BusinessRole && e.Company == b.Company && e.RetainCompanyRoles))))
                                {
                                    ShowAlertMessage(AlertType.Error, $"BusinessRole:{string.Join(",", EntitlementRequest.Where(i=> i.RetainCompanyRoles && existingRoles.Any(b => i.Entitlement == b.BusinessRole && i.Company == b.Company)).Select(i => i.Entitlement))} cannot be retained. Because all the cost centers are selected to remove.");
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }
        protected void Save_Click(object sender, EventArgs e)
        {
            //string msg = string.Empty;
            //if (!Validate(out msg))
            //{
            //    ShowAlertMessage(AlertType.Error, msg);
            //    return;
            //}
            //if (((IsAmendRequest() && (EndPointUserDropDownList.SelectedIndex > 0 && EntitlementRequest.Count > 0))
            //    || (IsDeleteRequest() && EndPointUserDropDownList.SelectedIndex > 0)
            //    || ((AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && isServiceOrSharedAccountRequest() && SAPUserTypeDropDown.SelectedIndex > 0 && !string.IsNullOrEmpty(txtAcntName.Text)))
            //    || (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && AdhocRequest.AccountType == AccountType.User)
            //    ) || sender == null)
            //{

            //    AdHocSAPHRFLINSRequest entitlementDetail = new AdHocSAPHRFLINSRequest();
            //    entitlementDetail = GenerateRequest(false);
            //    entitlementDetail.AdHocSAPHRFLINSRequestDetails.AddRange(EntitlementRequest);
            //    if (sender == null)
            //    {
            //        if (EntitlementRequest.Count <= 0 && (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && AdhocRequest.AccountType == AccountType.User))
            //            AdhocRequest.EntitlementDetail = null;
            //        else
            //            AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(entitlementDetail);
            //    }
            //    else
            //    {
            //        if (EntitlementRequest.Count <= 0 && (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && AdhocRequest.AccountType == AccountType.User))
            //        {

            //        }
            //        else
            //        {

            //        }

            //        if (SharedPanelHRMSFLINS.Visible)
            //        {
            //            SAPUserTypeDropDown.SelectedIndex = 0;
            //            txtAcntName.Text = "";
            //            txtDiscription.Text = "";
            //        }
            //        EntitlementRequest.Clear();
            //        FillRequestGrid();
            //        ClearControls();
            //    }
            //}
            //else
            //{

            //}
        }
        protected void RequestGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            FillRequestGrid();
        }
        protected void securityQstnsBtn_Click(object sender, EventArgs e)
        {

            if (this.AdhocRequest.GetSecurityQuestions != null)
            {
                SecurityQuestionUC1.SecurityResponseDetail = this.AdhocRequest.GetSecurityQuestions.SecurityResponseDetails;
            }
            SecurityQuestionUC1.Initialize();
        }

        protected void SecurityQuestion_SecurityResponseSaveClicked(List<SecurityResponseDetail> detail)
        {
            SecurityResponse response = new SecurityResponse();
            response.SecurityResponseDetails = detail;
            this.AdhocRequest.SetSecurityQuestions = response;
        }


    }
}