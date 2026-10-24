
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
    public partial class AdhocSapGrcStep2UC : UIBaseStep2UC
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }



        #region Members 

        private const string V_REQUEST = "V_REQUEST";

        #endregion

        #region Properties


        #endregion

        #region Events

        protected override IEntitlementDetail GetEntitlementDetail()
        {
            string module = "";

            if (ModuleDropDownList.SelectedIndex > 0)
                module = ModuleDropDownList.SelectedItem.Text;
            string submodule = "";
            if (SubModuleDropDownList.SelectedIndex > 0)
                submodule = SubModuleDropDownList.SelectedItem.Text;

            AdHocSAPGRCRequest sap = new AdHocSAPGRCRequest()
            {
                EndpointUserId = (EndPointUserDropDownList.SelectedIndex > 0) ? Convert.ToInt32(EndPointUserDropDownList.SelectedItem.Value) : 0,
                Module = module,
                SubModule = submodule,
                Entitlement = string.Join(",", EntitlementRequest),
                EntitlementType = EntitlementType.Role
            };

            if (EndPointUserDropDownList.SelectedItem != null)
            {
                sap.DisplayAccountName = EndPointUserDropDownList.SelectedItem.Text;
            }

            AdHocSAPSharedAccount sharedDetails = new AdHocSAPSharedAccount();
            if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && (AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Secondary))
            {
                if (SAPUserTypeDropDown.SelectedIndex > 0)
                {
                    sharedDetails.SAPUserType = (SAPUserType)(Convert.ToByte(SAPUserTypeDropDown.SelectedValue));
                }
                sharedDetails.AccountName = txtAcntName.Text;
                sharedDetails.Description = txtDiscription.Text;
            }
            sap.AdHocSAPGRCSharedAccount = (AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Secondary) && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount ? sharedDetails : null;
             
            return sap;
        }
         
        protected override void InitializeStep()
        {
            trSecurityQstns.Visible = false;
            Session["Module"] = null;
            Session["SubModule"] = null;
            Session["EndpointUserId"] = null;
            Session["AdhocrequestObj"] = AdhocRequest;
            // AddButton.Visible = true;
            EntitlementRequest.Clear();
            FillRequestGrid();
            txtDiscription.Attributes.Add("maxlength", "40");
            txtDiscription.Enabled = true;
            txtAcntName.Text = "";
            txtDiscription.Text = "";
            BindSAPUserType(AdhocRequest.AccountType);

            if (AdhocRequest.AccountType == AccountType.Secondary)
            {
                User usr = DomainApiClient<User>.GetById(this.AdhocRequest.RequesteeUserId);
                if (usr != null)
                {
                    txtDiscription.Text = usr.KnownAs;
                    txtDiscription.Enabled = false;
                } 
            }

            Session["AdhocActionType"] = Convert.ToInt32(AdhocRequest.AdhocActionType);

            if (AdhocRequest != null)
            {

                SpecialAccountWarning.ShowWarningMessage(AdhocRequest);
                LoadControl();

                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && AdhocRequest.AccountType == AccountType.User)
                {
                    SharedPanelFLINSBI.Visible = false;
                    SetMandatory(BusinessRoleAsterix, false);
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    //AddButton.Visible = false;
                    ModuleDropDownList.SelectedIndex = -1;
                    SubModuleDropDownList.SelectedIndex = -1;
                    DeleteAccountAdd.Visible = false;
                    BusinessRoleAutoCompleteBox.Enabled = true;
                    BusinessRoleAutoCompleteBox.Entries.Clear();
                }

                if (AdhocRequest.GetEntitlementDetail() != null || (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0))
                {
                    BindAdhocRequest();
                }
            }

        }

        protected void ddlModule_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ModuleDropDownList.SelectedIndex > 0)
            {
                Session["Module"] = ModuleDropDownList.SelectedValue;
                Session["SubModule"] = null;
                BindSubModule();
            }
            else
            {
                Session["Module"] = null;
                Session["SubModule"] = null;
            }
        }

        protected void ddlSubModule_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (SubModuleDropDownList.SelectedIndex > 0)
            {
                Session["SubModule"] = SubModuleDropDownList.SelectedItem.Text;
            }
            else
            {
                Session["SubModule"] = null;
            }
        }
        private void BindAdhocRequest()
        {
            var Entitlement = (AdHocSAPGRCRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (EndPointUserDropDownList.Items.FindByValue(Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId)) != null)
                    EndPointUserDropDownList.SelectedValue = Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId);
                EndPointUserDropDownList_SelectedIndexChanged(null, null);
                if (ModuleDropDownList.Items.FindByText(Entitlement.Module == "---Select---" ? "---Select---" : Convert.ToString(Entitlement.Module)) != null)
                    ModuleDropDownList.SelectedValue = Entitlement.Module == "---Select---" ? "---Select---" : Convert.ToString(Entitlement.Module);
                ddlModule_SelectedIndexChanged(null, null);
                if (SubModuleDropDownList.Items.FindByText(Entitlement.SubModule == "---Select---" ? "---Select---" : Convert.ToString(Entitlement.SubModule)) != null)
                    SubModuleDropDownList.SelectedValue = Entitlement.SubModule == "---Select---" ? "---Select---" : Convert.ToString(Entitlement.SubModule);
                ddlSubModule_SelectedIndexChanged(null, null);
                if (!string.IsNullOrEmpty(Entitlement.Entitlement))
                    EntitlementRequest = Entitlement.Entitlement.Split(',').Select(l => l).ToList();
                FillRequestGrid();
                if (Entitlement.AdHocSAPGRCSharedAccount != null)
                {
                    if (SAPUserTypeDropDown.Items.FindByText(Convert.ToString(Entitlement.AdHocSAPGRCSharedAccount.SAPUserType)) != null)
                    {
                        SAPUserTypeDropDown.Items.FindByText(Convert.ToString(Entitlement.AdHocSAPGRCSharedAccount.SAPUserType)).Selected = true;
                    }
                    txtAcntName.Text = Entitlement.AdHocSAPGRCSharedAccount.AccountName;
                    txtDiscription.Text = Entitlement.AdHocSAPGRCSharedAccount.Description;

                    if (AdhocRequest.AccountType == AccountType.Secondary)
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
            if (this.EntitlementRequest == null)
            {
                EntitlementRequest = new List<string>();
            }
            RequestGrid.Visible = this.EntitlementRequest.Count > 0 ? true : false;
            RequestGrid.DataSource = this.EntitlementRequest;
            RequestGrid.DataBind();
        }



        protected void AddButton_Click(object sender, EventArgs e)
        {

            if (VarianceValidation())
            {
                List<string> requests = new List<string>();
                string newRequest = BusinessRoleAutoCompleteBox.Text.Replace(";", "").Trim();
                requests = this.EntitlementRequest;
                List<string> listofOldEntiltement = this.EntitlementRequest.Select(entitlement => entitlement).ToList();
                if (!EntitlementRequest.Contains(newRequest) && !string.IsNullOrEmpty(newRequest))
                {
                    requests.Add(newRequest);
                    this.EntitlementRequest = requests;
                    FillRequestGrid();
                    ModuleDropDownList.SelectedIndex = -1;
                    Session["Module"] = null;
                    SubModuleDropDownList.Items.Clear();
                    SubModuleDropDownList.Items.Insert(0, new ListItem("-- Select SubModule --", "-1"));
                    BusinessRoleAutoCompleteBox.Entries.Clear();
                    Session["SubModule"] = null;
                }
                else
                {
                    ShowAlertMessage(AlertType.Error, "The business role is already in the request list.");
                }


                FillRequestGrid();
               
            }
        }

        protected override void afterSave()
        {
            base.afterSave();
            RefreshControl();
        }

        private void RefreshControl()
        {
            ClearControls();
            if (EntitlementRequest != null)
            {
                EntitlementRequest = null;
            }
            FillRequestGrid();
        }

        private void ClearControls()
        {

            if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                ModuleDropDownList.SelectedIndex = -1;
                SubModuleDropDownList.Items.Clear();
                SubModuleDropDownList.Items.Insert(0, new ListItem("-- Select SubModule --", "-1"));
                BusinessRoleAutoCompleteBox.Entries.Clear();
                txtAcntName.Text = "";
                txtDiscription.Text = "";
            }

            if (OtherControls.Visible)
            {
                ModuleDropDownList.SelectedIndex = 0;
                if (SubModuleDropDownList.Items.Count > 0)
                {
                    SubModuleDropDownList.SelectedIndex = 0;
                }
                BusinessRoleAutoCompleteBox.Entries.Clear();
            }
            if (AdhocRequest.AccountType!=
                 AccountType.User)
            {
                txtDiscription.Enabled = true;
                txtAcntName.Text = "";
             
                txtDiscription.Text = "";
                 
                BindSAPUserType(this.AdhocRequest.AccountType);
                if (AdhocRequest.AccountType == AccountType.Secondary)
                {
                    User usr = DomainApiClient<User>.GetById(this.AdhocRequest.RequesteeUserId);
                    if (usr != null)
                    {
                        txtDiscription.Text = usr.KnownAs;
                        txtDiscription.Enabled = false;
                    }
                }
            }
            BusinessRoleAutoCompleteBox.Entries.Clear();
            //EntitlementRequest.Clear();
        }
        private bool VarianceValidation()
        {
            if ((AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && isServiceOrSharedAccountRequest()) && (SAPUserTypeDropDown.SelectedIndex <= 0 || string.IsNullOrEmpty(txtAcntName.Text)))
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
            }
            if (AdhocRequest.AccountType == AccountType.Service && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (string.IsNullOrEmpty(AdhocRequest.SecurityQuestions))
                {
                    ShowAlertMessage(AlertType.Error, "Please select service account compliance check.");
                    return false;
                }
            }
            string Message = "Please select the";
            string ControlMessage = "";
            if (BusinessRoleAutoCompleteBox.Entries.Count <= 0)
            {
                ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Business Role" : ControlMessage + " , Business Role";
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
            else
            {
                return true;
            }
        }

        private List<string> EntitlementRequest
        {
            get
            {
                if (ViewState[V_REQUEST] != null)
                {
                    return (List<string>)ViewState[V_REQUEST];
                }
                return new List<string>();
            }
            set
            {
                ViewState[V_REQUEST] = value;
            }
        }
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
            if (EndPointUserDropDownList.SelectedItem != null && EndPointUserDropDownList.SelectedIndex > 0 )
            {
                Session["EndpointUserId"] = Convert.ToInt32(EndPointUserDropDownList.SelectedValue);
            }
        }


        #region Methods
        private void BindModule()
        {
            try
            {
                IList<string> data = DomainApiClient<Dto.SAPGRCRole>.GetByAction<IList<string>>(DomainApiAction.Matrix.SAP_GRC.BindModuleFromGRCRole, null);
                ModuleDropDownList.Items.Clear();
                ModuleDropDownList.DataSource = data;
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
                IList<string> data = DomainApiClient<Dto.SAPGRCRole>.GetByAction<IList<string>>(DomainApiAction.Matrix.SAP_GRC.BindSubModuleFromGRCRole, new Dictionary<string, string> { { "Module", module } });
                SubModuleDropDownList.Items.Clear();
                SubModuleDropDownList.DataSource = data;
                SubModuleDropDownList.DataBind();
                SubModuleDropDownList.Items.Insert(0, new ListItem("-- Select SubModule --", "-1"));
            }
            catch (Exception)
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

        private void BindEndpointUsers(long userId, long endpointSystemId, AccountType accountType)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("userid", userId.ToString());
            dict.Add("endpointsystemid", endpointSystemId.ToString());
            dict.Add("accountType", Convert.ToString((int)accountType));
            AccountReconDto list = DomainApiClient<Dto.ADSUserInfo>.GetByAction<AccountReconDto>("GetADUsersByDIUserId", dict);

            if (list != null && list.AccountReconDictionary!=null)
            {
                EndPointUserDropDownList.Items.Clear();
                DropDownHelper.DropDownListBind(EndPointUserDropDownList, list.AccountReconDictionary, "Value", "Key", null, true, "-- Select EndPoint User --", false);


                if (list.AccountReconDictionary.Count() == 1)
                {
                    EndPointUserDropDownList.SelectedIndex = 1;
                }

                Session["EndpointUserId"] = null;
                if (EndPointUserDropDownList.SelectedItem != null && EndPointUserDropDownList.SelectedIndex > 0)
                {
                    Session["EndpointUserId"] = Convert.ToInt32(EndPointUserDropDownList.SelectedValue);
                }
            }
        }

        private void BindSAPUserType(AccountType accountType)
        {
            try
            {
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
                        //SAPUserTypeDropDown.Enabled = false;
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
            if (AdhocRequest != null)
            {
                var request = AdhocRequest;
                var actionType = request.AdhocActionType;
                var accountType = request.AccountType;

                trSecurityQstns.Visible = accountType == AccountType.Service && actionType == AdHocActionType.NewAccount;
                SharedPanelFLINSBI.Visible = accountType != AccountType.User && actionType == AdHocActionType.NewAccount;
                if (actionType == AdHocActionType.AmendmentAdd || actionType == AdHocActionType.AmendmentRemove || actionType == AdHocActionType.DeleteAccount)
                {
                    trEndPointUser.Visible = true;
                    BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
                }
                else
                {
                    trEndPointUser.Visible = false;
                  
                }
                
                OtherControls.Visible = actionType != AdHocActionType.DeleteAccount;
            }
            BindModule();

        }

        #endregion

        protected void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidationMessage())
                {

                    //GroupPanelHRMSFLINS.Visible = true;
                    //SharedPanelFLINSBI.Visible = false;
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

        protected void RequestGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                Label EntitlementLabel = (Label)item.FindControl("EntitlementLabel");
                Label TechnicalRoleLabel = (Label)item.FindControl("TechnicalRoleLabel");
                if (EntitlementLabel != null && TechnicalRoleLabel != null)
                {
                    var roles = DomainApiClient<Dto.SAPGRCRole>.GetByAction<List<string>>(DomainApiAction.Matrix.SAP_GRC.BindTechnicalFromSAPGRCRole, new Dictionary<string, string> { { "BusinessRole", EntitlementLabel.Text } });
                    TechnicalRoleLabel.Text = string.Join(",", roles);
                }
            }
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
            //if (e.CommandName == "ViewTechicalRole")
            //{
            //    GridDataItem item = e.Item as GridDataItem;
            //    string role = Convert.ToString(item.GetDataKeyValue("Entitlement"));
            //    string company = Convert.ToString(item.GetDataKeyValue("Company"));


            //    AdHocSAPHRFLINSRequest entiltement = new AdHocSAPHRFLINSRequest();
            //    entiltement.AdHocSAPHRFLINSRequestDetails.AddRange(EntitlementRequest);
            //    var detail = entiltement.AdHocSAPHRFLINSRequestDetails.FirstOrDefault(lq => lq.Entitlement == role && lq.Company == company);
            //    List<string> list = DomainApiClient<Dto.SAPHRFLINSRoleMatrix>.GetByAction<List<string>>(DomainApiAction.Matrix.SAP_FLINS.SAPHRFLINSRoleMatrix.GetHrFlinsTechnicalRolesByAdhocRequestObject, null, detail);
            //    ViewTechnicalSource.DataSource = list;
            //    ViewTechnicalSource.DataBind();
            //    ViewTechnicalSourceWindow.VisibleOnPageLoad = true;
            //}
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ViewTechnicalSourceWindow.VisibleOnPageLoad = false;
        }



        public override bool Validate()
        {
            if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                if (EntitlementRequest.Count == 0)
                {
                    ShowAlertMessage(Infrastructure.AlertType.Error, Constants.addToGridWarning);
                    return false;
                }
            }
            if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                 
                string Message = "Please select the ";
                string ControlMessage = "";
                if (SharedPanelFLINSBI.Visible && SAPUserTypeDropDown.SelectedIndex <= 0)
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " SAP UserType" : ControlMessage + " , SAP UserType";
                }
                if (SharedPanelFLINSBI.Visible && string.IsNullOrEmpty(txtAcntName.Text))
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Account Name" : ControlMessage + " , Account Name";
                }
                if (trEndPointUser.Visible && EndPointUserDropDownList.SelectedIndex <= 0 && AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " EndPoint User" : ControlMessage + " , EndPoint User";
                }
                if (OtherControls.Visible && EntitlementRequest.Count == 0)
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Business Role" : ControlMessage + " , Business Role";
                }
                if (AdhocRequest.AccountType == AccountType.Service && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    if (string.IsNullOrEmpty(AdhocRequest.SecurityQuestions))
                    {
                        ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Service account compliance check" : ControlMessage + ", Service account compliance check";
                    }
                }
                if (ControlMessage != "")
                {
                    int lastIndex = ControlMessage.LastIndexOf(",");
                    Message += lastIndex > 0 ? ControlMessage.Substring(0, lastIndex) + " and" + ControlMessage.Substring(lastIndex + 1) : ControlMessage;
                    ShowAlertMessage(AlertType.Warning, Message + ".");
                    return false;
                }
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
                else if (EntitlementRequest.Count <= 0)
                {
                    ShowAlertMessage(AlertType.Error, "Please Select at least one Business Role.");
                    return false;
                }
            }
            if ((AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount && EndPointUserDropDownList.SelectedIndex <= 0))
            {
                ShowAlertMessage(AlertType.Error, "Please Select EndPoint User.");
                return false;
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

            //        if (SharedPanelFLINSBI.Visible)
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