using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC
{
    public partial class AdhocOneCardStep2UC_B : UIBaseStep2UC_B
    {
        public string SelectedBuilding
        {
            get
            {
                if (ddlBuilding.SelectedIndex > 0)
                {
                    return this.ddlBuilding.SelectedItem.Text;
                }
                return string.Empty;

            }
        }

        public string SelectedSection
        {
            get
            {
                if (ddlSection.SelectedIndex > 0)
                {
                    return this.ddlSection.SelectedItem.Text;
                }
                return string.Empty;
            }
        }

        public List<string> SelectedTechRoles
        {
            get
            {
                List<string> selecteditems = new List<string>();

                foreach (var checkeditem in TechnicalRoleCombobox.CheckedItems)
                {
                    selecteditems.Add(checkeditem.Text);
                }
                return selecteditems;
            }

        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void SetEntitlementDetail()
        {
            AdHocSMSRequest smsRequest = new AdHocSMSRequest();
            if (AdhocRequest != null)
            {
                smsRequest.EntitlementType = EntitlementType.AccessLevel;
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    if(Session["gridObjlst"] != null)
                    {
                        var list = (List<AdhocSmsGridInfo>)Session["gridObjlst"];
                        IEnumerable<string> selectedRoles = list.Select(item => item.TechnicalRole).ToList();
                        smsRequest.Entitlement = string.Join(",", selectedRoles);
                        smsRequest.GridInfo = list;
                    }
                    else
                    {
                        smsRequest.GridInfo = new List<AdhocSmsGridInfo>();
                    }
                }
                AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(smsRequest);
            }

        }

        void BindRepeater()
        {
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                Accountpanel.Visible = true;

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
                Accountpanel.Visible = false;
            }
        }
        protected override void InitializeStep()
        {
            ClearFields();
            if (AdhocRequest != null)
            {
                BindRepeater();
                LoadControl();
            }
        }

        bool UpdateBulkReqAccountDetail()
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
        public override bool Validate()
        {
            return Validatation() && UpdateBulkReqAccountDetail();
        }
        bool Validatation()
        {
            bool result = false;

            //if (Session["gridObjlst"] == null)
            //{
            //    ShowAlertMessage(AlertType.Warning, Constants.addToGridWarning);
            //    return result;
            //}
            if (Session["gridObjlst"] == null && AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                MasterPage.ShowAlert(Infrastructure.AlertType.Warning, Constants.addToGridWarning);
                return result;
            }

            return true;

        }
        void ClearFields()
        {
            Session["gridObjlst"] = null;
            TechnicalRoleCombobox.Items.Clear();
            ddlBuilding.Items.Clear();
            ddlSection.Items.Clear();
            SMSObjGrid.Visible = false;
        }

        void LoadControl()
        {
            if (AdhocRequest != null)
            {
                TRRoles.Visible = false;
                TRSection.Visible = false;
                TRBuilding.Visible = false;
                if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    TRRoles.Visible = true;
                    TRSection.Visible = true;
                    TRBuilding.Visible = true;
                    BindBuilding();
                }
                else if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    TRRoles.Visible = true;
                    TRSection.Visible = false;
                    TRBuilding.Visible = false;
                    BindTechRoles();
                }else if((this.AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount))
                {
                    btnAdd.Visible = false;
                }
            }

        }
        protected override void BindAdhocRequest()
        {
            var Entitlement = (AdHocSMSRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement == null) return;
            SMSObjGrid.DataSource = Entitlement.GridInfo;
            SMSObjGrid.Visible = true;
            Session["gridObjlst"] = Entitlement.GridInfo;
        }
        private void BindBuilding()
        {
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                ddlSection.Items.Clear();
                TechnicalRoleCombobox.Items.Clear();

                Dictionary<string, string> list = DomainApiClient<Dto.SMSAccessLevel>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.OneCard.GetAllBuildings, null);
                App.Step2UC.DropDownHelper.DropDownListBind(ddlBuilding, list, "Value", "Key", null, true, "-- Select Building --", false);
                ddlBuilding.SelectedIndex = 0;
            }
        }
        void BindSection()
        {
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                TechnicalRoleCombobox.Items.Clear();

                Dictionary<string, string> list = DomainApiClient<Dto.SMSAccessLevel>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.OneCard.GetSectionByBuilding,
                    new Dictionary<string, string> { { "Building", SelectedBuilding } });
                App.Step2UC.DropDownHelper.DropDownListBind(ddlSection, list, "Value", "Key", null, true, "-- Select Section --", false);
                ddlSection.SelectedIndex = 0;
            }
        }

        void BindTechRoles()
        {
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                Dictionary<string, string> distinctRoles = DomainApiClient<Dto.SMSAccessLevel>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.OneCard.GetDistinctTechRoles,
                null);

                doBindTechRoles(distinctRoles);
            }

            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                Dictionary<string, string> list = DomainApiClient<Dto.SMSAccessLevel>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.OneCard.GetTechRolesByBuildingandSection,
                      new Dictionary<string, string> { { "Building", SelectedBuilding }, { "section", this.SelectedSection } });
                doBindTechRoles(list);
            }
        }

        void doBindTechRoles(Dictionary<string, string> list)
        {
            TechnicalRoleCombobox.DataSource = list;
            TechnicalRoleCombobox.DataValueField = "key";
            TechnicalRoleCombobox.DataTextField = "Value";
            TechnicalRoleCombobox.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {

            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (this.SelectedTechRoles.Count == 0)
                {
                    ShowAlertMessage(Infrastructure.AlertType.Error, "Please select at least one techinical role");
                    return;
                }
            }
            var selectedRoles = new List<string>();
            if (Session["gridObjlst"] != null)
            {
                var list = ((List<AdhocSmsGridInfo>)Session["gridObjlst"]);
                selectedRoles = list.Select(item => item.TechnicalRole).ToList();
            }


            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {

                List<AdhocSmsGridInfo> _gridObjlst = new List<AdhocSmsGridInfo>();

                if (this.SelectedTechRoles.Count > 0)
                {
                    foreach (string techRole in this.SelectedTechRoles)
                    {
                        if (selectedRoles.Contains(techRole))
                        {

                            ShowAlertMessage(Infrastructure.AlertType.Error, techRole + " role has been already selected in the list, please remove");
                            return;
                        }
                        AdhocSmsGridInfo obj = new AdhocSmsGridInfo();
                        obj.Building = this.SelectedBuilding;
                        obj.Section = this.SelectedSection;
                        obj.Description = getDescription(techRole); ;
                        obj.TechnicalRole = techRole;
                        _gridObjlst.Add(obj);
                    }
                }
                if (_gridObjlst.Count > 0)
                {
                    var seleteditems = new List<AdhocSmsGridInfo>();
                    if (Session["gridObjlst"] != null)
                    {
                        seleteditems = (List<AdhocSmsGridInfo>)Session["gridObjlst"];
                    }

                    seleteditems = seleteditems.Count > 0 ? seleteditems.Union(_gridObjlst).ToList() : _gridObjlst;
                    Session["gridObjlst"] = seleteditems;
                }

                if (Session["gridObjlst"] != null)
                {
                    SMSObjGrid.Visible = true;
                    SMSObjGrid.DataSource = (List<AdhocSmsGridInfo>)Session["gridObjlst"];
                    SMSObjGrid.DataBind();
                    ddlBuilding.SelectedIndex = 0;
                    ddlSection.SelectedIndex = 0;
                    TechnicalRoleCombobox.Items.Clear();
                }
            }
            else if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                List<AdhocSmsGridInfo> _gridObjlst = new List<AdhocSmsGridInfo>();

                if (SelectedTechRoles.Count > 0)
                {
                    foreach (string techRole in SelectedTechRoles)
                    {
                        if (selectedRoles.Contains(techRole))
                        {
                            ShowAlertMessage(Infrastructure.AlertType.Error, techRole + " role has been already selected in the list, please remove.");
                            return;
                        }
                        AdhocSmsGridInfo obj = new AdhocSmsGridInfo();


                        SMSXLRoleCatalogue roleCatlouge = DomainApiClient<Dto.SMSAccessLevel>.GetByAction<SMSXLRoleCatalogue>(DomainApiAction.Endpoint.OneCard.GetBuildingandSectionByTechRoles,
                            new Dictionary<string, string> { { "techRole", techRole } });
                        if (roleCatlouge != null)
                        {
                            obj.TechnicalRole = techRole;
                            obj.Section = roleCatlouge.Section;
                            obj.Building = roleCatlouge.Building;
                            obj.Description = roleCatlouge.Description;
                            _gridObjlst.Add(obj);
                        }
                    }


                }
                if (_gridObjlst.Count > 0)
                {
                    var seleteditems = new List<AdhocSmsGridInfo>();
                    if (Session["gridObjlst"] != null)
                    {
                        seleteditems = (List<AdhocSmsGridInfo>)Session["gridObjlst"];
                    }

                    seleteditems = seleteditems.Count > 0 ? seleteditems.Union(_gridObjlst).ToList() : _gridObjlst;
                    Session["gridObjlst"] = seleteditems;
                }
                if (Session["gridObjlst"] != null)
                {
                    SMSObjGrid.Visible = true;
                    SMSObjGrid.DataSource = (List<AdhocSmsGridInfo>)Session["gridObjlst"];
                    SMSObjGrid.DataBind();
                }
            }

        }


        string getDescription(string TechRole)
        {
            return DomainApiClient<Dto.SMSAccessLevel>.GetByAction<string>(DomainApiAction.Endpoint.OneCard.GetDescription,
                new Dictionary<string, string> { { "techRole", TechRole } });
        }

        protected void ddlBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlBuilding.SelectedIndex == 0)
            {
                ddlSection.SelectedIndex = 0;
                TechnicalRoleCombobox.Items.Clear();
            }
            else
            {
                BindSection();
            }
        }
        protected void ddlSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSection.SelectedIndex == 0)
            {
                TechnicalRoleCombobox.Items.Clear();
            }
            else
            {
                BindTechRoles();
            }
        }
        protected void SMSObjGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var TechRole = (string)((GridDataItem)e.Item).GetDataKeyValue("TechnicalRole");
                List<AdhocSmsGridInfo> _sessionObj = (List<AdhocSmsGridInfo>)Session["gridObjlst"];
                _sessionObj.RemoveAll(item => item.TechnicalRole == TechRole);
                Session["gridObjlst"] = _sessionObj;
                if (_sessionObj.Count > 0)
                {
                    SMSObjGrid.DataSource = (List<AdhocSmsGridInfo>)Session["gridObjlst"];
                    SMSObjGrid.DataBind();
                }
                else
                {
                    Session["gridObjlst"] = null;
                    SMSObjGrid.Visible = false;
                }
            }
        }
        protected void TechnicalRoleCombobox_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (TechnicalRoleCombobox.CheckedItems.Count > 0)
            {
                btnAdd.Enabled = true;
            }
            else
            {
                btnAdd.Enabled = false;
            }
        }
        protected void SMSObjGrid_DataBound(object sender, EventArgs e)
        {
            RadGrid grid = sender as RadGrid;
            int gridItems = grid.MasterTableView.Items.Count;
            if (gridItems <= 3)
            {
                grid.ClientSettings.Scrolling.AllowScroll = false;
            }
            else
            {
                grid.ClientSettings.Scrolling.ScrollHeight = System.Web.UI.WebControls.Unit.Pixel(150);
                grid.ClientSettings.Scrolling.AllowScroll = true;
            }
        }

    }
}
