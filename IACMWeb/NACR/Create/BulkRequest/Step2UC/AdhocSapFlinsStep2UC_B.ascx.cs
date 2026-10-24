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

namespace DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC
{    
    public partial class AdhocSapFlinsStep2UC_B : UIBaseStep2UC_B
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        

        #region Members 

        private const string V_REQUEST = "V_REQUEST";

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

        #endregion

        #region Events

        protected override void SetEntitlementDetail()
        {
           
            AdHocSAPHRFLINSRequest entitlementDetail = new AdHocSAPHRFLINSRequest();
            entitlementDetail = GenerateRequest(false);
            entitlementDetail.AdHocSAPHRFLINSRequestDetails.AddRange(EntitlementRequest);
            AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(entitlementDetail);
            EntitlementRequest.Clear();
            FillRequestGrid();
             
        }
        protected override void InitializeStep()
        {
          
            Session["Module"] = null;
            Session["SubModule"] = null;
            Session["EndpointUserId"] = null;
            // AddButton.Visible = true;
            EntitlementRequest.Clear();
            FillRequestGrid();
            if (AdhocRequest != null)
            {
                var request = AdhocRequest;
                Session["BulkAdhocrequestObj"] = AdhocRequest;
                BindEndpointUsers();
                BusinessRoleAsterix.Visible = true;
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    SetMandatory(BusinessRoleAsterix, false);
                }
                else
                {
                    SetMandatory(BusinessRoleAsterix, true);
                }
               // if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    //AddButton.Visible = false;
                    ModuleDropDownList.SelectedIndex = -1;
                SubModuleDropDownList.SelectedIndex = -1;
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
                LoadControl();
            } 
        }
        protected override void BindAdhocRequest()
        {
            var Entitlement = (AdHocSAPHRFLINSRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                
                if (Entitlement.AdHocSAPHRFLINSRequestDetails != null)
                    EntitlementRequest = Entitlement.AdHocSAPHRFLINSRequestDetails;
                FillRequestGrid(); 
            }
        }

        #endregion

        
       

        bool IsDeleteRequest()
        {
            return AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount;
        }
        private void FillRequestGrid()
        {
            if (this.EntitlementRequest == null)
            {
                EntitlementRequest = new List<AdHocSAPHRFLINSRequestDetail>();
            }
            RequestGrid.Visible = this.EntitlementRequest.Count > 0 ? true : false;
            RequestGrid.DataSource = this.EntitlementRequest;
            RequestGrid.DataBind();
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
                    if (CompanyAutoCompleteBox.Enabled == true)
                    {
                        int EndpointUserId = 0;
                        //if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                        //{
                        //    if (EndPointUserDropDownList.SelectedIndex > 0)
                        //    {
                        //        EndpointUserId = Convert.ToInt32(EndPointUserDropDownList.SelectedValue);
                        //    }
                        //}

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

                    bool isCompanyMandatory = false;
                    if (BusinessRoleAutoCompleteBox.Entries.Count > 0)
                    {
                        var parameter = new Dictionary<string, string> { { "BusinessRole", BusinessRoleAutoCompleteBox.Entries[0].Text } };
                        isCompanyMandatory = DomainApiClient<SAPRoleMandatoryField>.GetByAction<bool>(DomainApiAction.Matrix.SAP_FLINS.SAPRoleMandatoryFields.checkBusinessRoleCompanyOnlyMandatory, parameter);
                    }

                    if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove && CompanyAutoCompleteBox.CheckBoxes == false && !isCompanyMandatory)
                    {
                        RemoveCompanyCheckBox.Visible = true;
                        RemoveCompanyCheckBox.Style["width"] = "40%";
                        RemoveCompanyCheckBox.Style["float"] = "right";
                        CompanyAutoCompleteBox.Style["width"] = "60%!important";
                    }
                    else
                    {
                        RemoveCompanyCheckBox.Visible = false;
                        RemoveCompanyCheckBox.Style["width"] = "0%";
                        RemoveCompanyCheckBox.Style["float"] = "right";
                        CompanyAutoCompleteBox.Style["width"] = "100%!important";
                    }

                }

            }
            catch (Exception ex)
            {
            }

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
                EntitlementRequest.Clear();
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
          
            string Message = "Please select the";
            string ControlMessage = "";
            if (BusinessRoleAutoCompleteBox.Entries.Count <= 0)
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
                if (CostCenterAutoCompleteBox.CheckedItems.Count <= 0)
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Cost Center" : ControlMessage + " , Cost Center";
                }
            }
            if (PurchaseGroupAutoCompleteBox.Enabled == true)
            {
                if (PurchaseGroupAutoCompleteBox.CheckedItems.Count <= 0)
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
                ShowAlertMessage(AlertType.Error, Message);
                return false;
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
            return true;
        }
        
        private void ClearComboBox(RadComboBox Control)
        {
            foreach (RadComboBoxItem items in Control.Items)
            {
                items.Checked = false;
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
                    //if (EndPointUserDropDownList.SelectedItem != null && EndPointUserDropDownList.SelectedIndex > 0 && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    //{
                    //    EndpointUserId = Convert.ToInt32(EndPointUserDropDownList.SelectedValue);
                    //}

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
                                CostCenterAutoCompleteBox.Enabled = true;
                                CostCenterAutoCompleteBox.DataSource = dataSource;
                                CostCenterAutoCompleteBox.DataTextField = "Value";
                                CostCenterAutoCompleteBox.DataValueField = "Key";
                                CostCenterAutoCompleteBox.DataBind();
                                divCostCenter.Attributes.Add("class", "form-group required");
                                return;
                            }
                        case "PurchaseGroup":
                            {
                                PurchaseGroupAutoCompleteBox.Enabled = true;
                                PurchaseGroupAutoCompleteBox.DataSource = dataSource;
                                PurchaseGroupAutoCompleteBox.DataTextField = "Value";
                                PurchaseGroupAutoCompleteBox.DataValueField = "Key";
                                PurchaseGroupAutoCompleteBox.DataBind();
                                if (this.AdhocRequest != null && this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove && dataSource != null && dataSource.Count == 1)
                                {
                                    RemoveCompanyCheckBox.Checked = false;
                                    RemoveCompanyCheckBox.Enabled = false;
                                }
                                else
                                {
                                    RemoveCompanyCheckBox.Enabled = true;
                                }
                                divPgroup.Attributes.Add("class", "form-group required");
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
            catch
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
                //IList<string> data = DomainApiClient<Dto.SAPBIRoleCategory>.GetByAction<IList<string>>(DomainApiAction.Matrix.SAP_FLINS_BI.BindSubModuleFromFLINSBI, new Dictionary<string, string> { { "moduleName", module } });
                IList<SAPHRFLINSRoleMatrix> data = DomainApiClient<Dto.SAPHRFLINSRoleMatrix>.GetByAction<IList<Dto.SAPHRFLINSRoleMatrix>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.BindSubModule, new Dictionary<string, string> { { "moduleName", module }, { "UserID", AdhocRequest.BulkRequestDetails[0].UserId.ToString() } });
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
            return new AdHocSAPHRFLINSRequestDetail
            {
                Module = ModuleDropDownList.SelectedIndex > 0 ? ModuleDropDownList.SelectedItem.Text : null,
                SubModule = SubModuleDropDownList.SelectedIndex > 0 ? SubModuleDropDownList.SelectedItem.Text : null,
                EntitlementType = EntitlementType.Role,
                Entitlement = BusinessRoleAutoCompleteBox.Entries.Count > 0 ? BusinessRoleAutoCompleteBox.Entries[0].Text : "",
                Company = Companyname,
                RetainCompanyRoles = RemoveCompanyCheckBox.Checked,
                CostCenter = EntiltementString(CostCenterAutoCompleteBox),
                PurchaseOrg = EntiltementString(PurchaseOrgAutoCompleteBox),
                PurchaseGroup = EntiltementString(PurchaseGroupAutoCompleteBox),
                Plant = EntiltementString(PlanetAutoCompleteBox),
                StorageLocation = EntiltementString(StoreLocationsAutoCompleteBox),
                MRPController = EntiltementString(MRPControllerAutoCompleteBox),
            };
        }

        private AdHocSAPHRFLINSRequest GenerateRequest(bool CanAddRequest)
        {
            try
            {
                AdHocSAPHRFLINSRequest entitlementDetail = new AdHocSAPHRFLINSRequest();
                List<AdHocSAPHRFLINSRequestDetail> listofrequest = new List<AdHocSAPHRFLINSRequestDetail>();
                AdHocSAPHRFLINSRequestDetail mulitpleRequestDetails;
                AdHocSAPSharedAccount sharedDetails = new AdHocSAPSharedAccount();
                 
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
                    

                };
                if (CanAddRequest)
                {
                    EntitlementRequest = EntitlementRequest;
                    listofrequest.ForEach(lq =>
                    {
                        var exist = EntitlementRequest.Any(l => l == lq);
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
                        if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount && !exist)
                            EntitlementRequest.Add(lq);
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
                entiltment = entiltment == "" ? checkedItems.Text : entiltment + "," + checkedItems.Text;
            }
            return entiltment;
        }

        private void BindEndpointUsers()
        {
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                AccountRepeater.Visible = true;
                trEndPointUser.Visible = true;
                Dictionary<string, Dictionary<long, string>> repeaterData = new Dictionary<string, Dictionary<long, string>>();
                AdhocRequest.BulkRequestDetails.ForEach(item =>
            {
                var param = new Dictionary<string, string>
                    {
                        { "userId",item.UserId.ToString()},
                        { "endpointSystemId", AdhocRequest.EndpointSystemId.ToString()},
                        { "accountType", AccountType.User.ToString()}
                    };

                List<AccountReconcilation> userAccounts = DomainApiClient<Dto.AccountReconcilation>.
                                                                GetByAction<List<Dto.AccountReconcilation>>(
                                                                                DomainApiAction.Provisioning.AccountReconcilation.GetByAccountTypeAndEndpointSystemId, param);


                if (userAccounts != null && userAccounts.Count > 0)
                {
                    var userNames = new Dictionary<long, string>();

                    var items = userAccounts.Distinct();
                    if (items.Count() > 1)
                    {
                        userNames.Add(0, "--Please Select Account--");
                    }
                    foreach (var i in items)
                    {
                        userNames.Add(i.EndpointUserId, i.AccountName);
                    }
                    repeaterData.Add(item.Name, userNames);
                }
            });

                AccountRepeater.DataSource = repeaterData;
                AccountRepeater.DataBind();
            }
            else
            {
                AccountRepeater.Visible = false;
                trEndPointUser.Visible = false;
            }
        
        }

        
        private bool ValidationMessage()
        {
            try
            {
                if (AdhocRequest != null)
                {
                    return true;
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
            if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                ControlDiv.Visible = true;
                ControlDiv1.Visible = true;
                buttontr.Visible = false;
                if (ModuleDropDownList.SelectedIndex <= 0)
                {
                    BindModule(AdhocRequest.BulkRequestDetails[0].UserId);
                }
            }
            else if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
            {
                ControlDiv1.Visible = false;
                ControlDiv.Visible = false;
                buttontr.Visible = false;
            }
            else if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                ControlDiv.Visible = true;
                ControlDiv1.Visible = true;
                buttontr.Visible = false;
                if (ModuleDropDownList.SelectedIndex <= 0)
                {
                    BindModule(AdhocRequest.BulkRequestDetails[0].UserId);
                }
            }

        }


        #endregion

        

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
            }
            if (e.CommandName == "ViewTechicalRole")
            {
                GridDataItem item = e.Item as GridDataItem;
                string role = Convert.ToString(item.GetDataKeyValue("Entitlement"));
                string company = Convert.ToString(item.GetDataKeyValue("Company"));


                AdHocSAPHRFLINSRequest entiltement = new AdHocSAPHRFLINSRequest();
                entiltement.AdHocSAPHRFLINSRequestDetails.AddRange(EntitlementRequest);
                var detail = entiltement.AdHocSAPHRFLINSRequestDetails.FirstOrDefault(lq => lq.Entitlement == role && ((string.IsNullOrWhiteSpace(lq.Company) ? string.Empty: lq.Company) == (string.IsNullOrWhiteSpace(company)?string.Empty: company)));
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

            if ((AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove) && EntitlementRequest.Count <= 0)
            {   
                    ShowAlertMessage(AlertType.Error, "Please Select at least one Business Role.");
                    return false; 
            }

            return UpdateBulkReqAccountDetail(); 
        }


         
        protected void RequestGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            //FillRequestGrid();
        }

        private bool UpdateBulkReqAccountDetail()
        {
            if (AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                foreach (GridDataItem dataItem in AccountRepeater.Items)
                { 
                    if (dataItem.ItemType == GridItemType.AlternatingItem || dataItem.ItemType == GridItemType.Item)
                    {
                        var userName = ((Label)dataItem.FindControl("lblAcountName")).Text;
                        var SelectedAccount = ((RadComboBox)dataItem.FindControl("ddlAccounts")).SelectedItem.Text;
                        var endpointuserid = ((RadComboBox)dataItem.FindControl("ddlAccounts")).SelectedItem.Value;
                        if (Convert.ToInt32(endpointuserid) == 0)
                        {
                            ShowAlertMessage(AlertType.Error, "Please enter the Account Information."); 
                            return false;
                        }
                        AdhocRequest.BulkRequestDetails.FirstOrDefault(item => item.Name == userName).EndpointUserId = Convert.ToInt32(endpointuserid);
                        AdhocRequest.BulkRequestDetails.FirstOrDefault(item => item.Name == userName).AccountName = SelectedAccount;
                    }
                }
                return true;
            }
            else
            {
                return true;
            }
        }
         
    }
}