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

    public partial class AdhocSapGrcStep2UC_B : UIBaseStep2UC_B
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

        protected override void SetEntitlementDetail()
        {
            string module = "";

            if (ModuleDropDownList.SelectedIndex > 0)
                module = ModuleDropDownList.SelectedItem.Text;
            string submodule = "";
            if (SubModuleDropDownList.SelectedIndex > 0)
                submodule = SubModuleDropDownList.SelectedItem.Text;

            AdHocSAPGRCRequest sap = new AdHocSAPGRCRequest()
            {
                Module = module,
                SubModule = submodule,
                Entitlement = string.Join(",", EntitlementRequest),
                EntitlementType = EntitlementType.Role
            };

            AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(sap);

        }
        protected override void InitializeStep()
        {

            Session["Module"] = null;
            Session["SubModule"] = null;
            Session["EndpointUserId"] = null;
            Session["BulkAdhocrequestObj"] = AdhocRequest;
            BindEndpointUsers();
            EntitlementRequest.Clear();
            FillRequestGrid();

            if (AdhocRequest != null)
            {
                var request = AdhocRequest;
                
                if (request.AdhocActionType == AdHocActionType.NewAccount)
                {
                    Panel1.Visible = false;
                    LoadControl();
                }
                else
                {
                    Panel1.Visible = true;
                    LoadControl();
                }

                //AddButton.Visible = false;
                ModuleDropDownList.SelectedIndex = -1;
                SubModuleDropDownList.SelectedIndex = -1;

                BusinessRoleAutoCompleteBox.Enabled = true;
                BusinessRoleAutoCompleteBox.Entries.Clear();
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
        protected override void BindAdhocRequest()
        {
            var Entitlement = (AdHocSAPGRCRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (ModuleDropDownList.Items.FindByText(Entitlement.Module == "---Select---" ? "---Select---" : Convert.ToString(Entitlement.Module)) != null)
                    ModuleDropDownList.SelectedValue = Entitlement.Module == "---Select---" ? "---Select---" : Convert.ToString(Entitlement.Module);
                ddlModule_SelectedIndexChanged(null, null);
                if (SubModuleDropDownList.Items.FindByText(Entitlement.SubModule == "---Select---" ? "---Select---" : Convert.ToString(Entitlement.SubModule)) != null)
                    SubModuleDropDownList.SelectedValue = Entitlement.SubModule == "---Select---" ? "---Select---" : Convert.ToString(Entitlement.SubModule);
                ddlSubModule_SelectedIndexChanged(null, null);
                if (!string.IsNullOrEmpty(Entitlement.Entitlement))
                    EntitlementRequest = Entitlement.Entitlement.Split(',').Select(l => l).ToList();
                FillRequestGrid();
            }
        }

        #endregion


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
                    BusinessRoleAutoCompleteBox.Entries.Clear();

                }
                else
                {
                    ShowAlertMessage(AlertType.Error, "The business role is already in the request list.");
                }


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

            if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                ModuleDropDownList.SelectedIndex = -1;
                Session["Module"] = null;
                SubModuleDropDownList.Items.Clear();
                SubModuleDropDownList.Items.Insert(0, new ListItem("-- Select SubModule --", "-1"));
                BusinessRoleAutoCompleteBox.Entries.Clear();
                Session["SubModule"] = null;
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

            BusinessRoleAutoCompleteBox.Entries.Clear();
            //EntitlementRequest.Clear();
        }

        private void BindEndpointUsers()
        {
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                AccountRepeater.Visible = true;
                Panel1.Visible = true;
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

        }
        private bool VarianceValidation()
        {

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

        private void LoadControl()
        {
            BindModule();

            if (this.AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
            {
                GroupPanelFLINSBI.Visible = false;
                this.DeleteAccountAdd.Visible = false;
            }
            else
            {
                this.DeleteAccountAdd.Visible = true;
            }
        }

        #endregion



        protected void RequestGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                Label EntitlementLabel = (Label)item.FindControl("EntitlementLabel");
                Label TechnicalRoleLabel = (Label)item.FindControl("TechnicalRoleLabel");
                if (EntitlementLabel != null && TechnicalRoleLabel != null)
                {
                    var roles = DomainApiClient<Dto.SAPGRCRole>.GetByAction<List<string>>(DomainApiAction.Matrix.SAP_GRC.BindTechnicalFromSAPGRCRole
                                                            , new Dictionary<string, string> { { "BusinessRole", EntitlementLabel.Text } });
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

            if ((AdhocRequest.AdhocActionType == AdHocActionType.NewAccount || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove) && EntitlementRequest.Count <= 0)
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, Constants.addToGridWarning);
                return false;
            }

            return UpdateBulkReqAccountDetail();
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

        protected void RequestGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            //FillRequestGrid();
        }



    }

}