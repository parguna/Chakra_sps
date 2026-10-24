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

namespace DI.Chakra.Web.NACR.Create.App.Step2UC
{
    public partial class AdhocOneCardStep2UC : UIBaseStep2UC
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
        private string onecardEntitlement = "OneCardEntitlement";
        public List<AdhocSmsGridInfo> EntitlementRequest
        {
            get
            {
                if (ViewState[onecardEntitlement] != null)
                {
                    return (List<AdhocSmsGridInfo>)ViewState[onecardEntitlement];
                }
                return new List<AdhocSmsGridInfo>();
            }
            set
            {
                ViewState[onecardEntitlement] = value;
            }
        }
        protected override void afterSave()
        {
            base.afterSave();
            EntitlementRequest = null;
            TechnicalRoleCombobox.ClearCheckedItems();
            TechnicalRoleCombobox.SelectedIndex = -1;
            ddlBuilding.SelectedIndex = -1;
            ddlSection.SelectedIndex = -1;
            BindGrid();
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

        protected override IEntitlementDetail GetEntitlementDetail()
        {
            AdHocSMSRequest smsRequest = new AdHocSMSRequest();

            if (AdhocRequest != null)
            {
                if (this.ddlEndpointUser.SelectedIndex > 0)
                {
                    smsRequest.EndpointUserId = ddlEndpointUser.SelectedIndex > 0 ? Convert.ToInt32(ddlEndpointUser.SelectedValue) : 0;

                    if (ddlEndpointUser.SelectedItem != null)
                    {
                        smsRequest.DisplayAccountName = ddlEndpointUser.SelectedItem.Text;
                    }
                }
                if (AdhocRequest.AccountType == AccountType.User)
                {
                    smsRequest.EntitlementType = EntitlementType.AccessLevel;
                    if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        var list = EntitlementRequest;
                        if (list != null)
                        {
                            IEnumerable<string> selectedRoles = list.Select(item => item.TechnicalRole).ToList();
                            smsRequest.Entitlement = string.Join(",", selectedRoles);
                            smsRequest.GridInfo = list;
                        }

                    }

                }
            }
            return smsRequest;
        }
        protected override void InitializeStep()
        {
            if (AdhocRequest != null)
            {
                LoadControl();
                if (AdhocRequest.GetEntitlementDetail() != null || (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0))
                {
                    BindAdhocRequest();
                }
            }
        }

        public override bool Validate()
        {
            return Validatation();
        }
        bool Validatation()
        {
            bool result = false;

            if (this.ddlEndpointUser.SelectedIndex <= 0 && this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                MasterPage.ShowAlert(Infrastructure.AlertType.Warning, "Please select EndPoint User Name.");
                return result;
            }

            if (EntitlementRequest.Count == 0 && this.AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                MasterPage.ShowAlert(Infrastructure.AlertType.Warning, Constants.addToGridWarning);
                return result;
            }

            return true;

        }
        void ClearFields()
        {
            EntitlementRequest = null;
            TechnicalRoleCombobox.Items.Clear();
            ddlBuilding.Items.Clear();
            ddlSection.Items.Clear();
            SMSObjGrid.Visible = false;
        }

        void LoadControl()
        {
            if (AdhocRequest != null)
            {
                if (AdhocRequest.AccountType == AccountType.User)
                {
                    btnAdd.Visible = true;
                    if (AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                    {
                        trEndPointUser.Visible = true;
                        BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
                    }
                    else
                    {
                        ddlEndpointUser.Items.Clear();
                        trEndPointUser.Visible = false;
                        BindBuilding();                         
                    }

                    if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                    {
                        TRSection.Visible = true;
                        TRBuilding.Visible = true;
                        BindBuilding();
                    }
                    else if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        TRSection.Visible = false;
                        TRBuilding.Visible = false;
                        BindTechRoles();
                    }
                    else if (this.AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        btnAdd.Visible = false;
                        TRSection.Visible = false;
                        TRBuilding.Visible = false;
                        TRRoles.Visible = false;
                    }
                }

            }

            if (AdhocRequest != null && AdhocRequest.Id != 0)
            {

                var Entitlement = (AdHocSMSRequest)AdhocRequest.GetEntitlementDetail();
                if (ddlEndpointUser.Items.FindByValue(Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId)) != null)
                    ddlEndpointUser.SelectedValue = Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId);
                ddlEndpointUser.Items.FindByValue(Entitlement.EndpointUserId.ToString()).Selected = true;
                ddlEndpointUser_SelectedIndexChanged(null, null);
                SMSObjGrid.DataSource = Entitlement.GridInfo;
                SMSObjGrid.Visible = true;
                EntitlementRequest = Entitlement.GridInfo;
            }
        }
        void BindAdhocRequest()
        {
            var Entitlement = (AdHocSMSRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null && Entitlement.GridInfo != null && Entitlement.GridInfo.Count > 0)
            {
                EntitlementRequest = Entitlement.GridInfo;
            }
            BindGrid();
        }
        private void BindGrid()
        {
            if (EntitlementRequest != null && EntitlementRequest.Count > 0)
            {
                SMSObjGrid.Visible = true;
                SMSObjGrid.DataSource = EntitlementRequest;
                SMSObjGrid.DataBind();
            }
            else
            {
                SMSObjGrid.Visible = false;
                SMSObjGrid.DataSource = new List<AdhocSmsGridInfo>();
                SMSObjGrid.DataBind();
            }
        }
        void BindEndpointUsers(long userId, long endpointSystemId, AccountType accountType)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("userid", userId.ToString());
            dict.Add("endpointsystemid", endpointSystemId.ToString());
            dict.Add("accountType", Convert.ToString((int)accountType));
            Dto.AccountReconDto list = DomainApiClient<ADSUserInfo>.GetByAction<AccountReconDto>("GetADUsersByDIUserId", dict);

            if (list != null && list.AccountReconDictionary != null)
            {
                ddlEndpointUser.Items.Clear();
                DropDownHelper.DropDownListBind(ddlEndpointUser, list.AccountReconDictionary, "Value", "Key", null, true, "-- Select EndPoint User --", false);
            }
        }

        private void BindBuilding()
        {
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                if (ddlEndpointUser.SelectedIndex != 0)
                {
                    ddlSection.Items.Clear();
                    TechnicalRoleCombobox.Items.Clear();

                    Dictionary<string, string> list = DomainApiClient<Dto.SMSAccessLevel>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.OneCard.GetAllBuildings, null);
                    DropDownHelper.DropDownListBind(ddlBuilding, list, "Value", "Key", null, true, "-- Select Building --", false);
                    ddlBuilding.SelectedIndex = 0;
                }
            }
        }
        private void BindSection()
        {
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                TechnicalRoleCombobox.Items.Clear();

                Dictionary<string, string> list = DomainApiClient<Dto.SMSAccessLevel>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.OneCard.GetSectionByBuilding,
                    new Dictionary<string, string> { { "Building", SelectedBuilding } });
                DropDownHelper.DropDownListBind(ddlSection, list, "Value", "Key", null, true, "-- Select Section --", false);
                ddlSection.SelectedIndex = 0;
            }
        }

        void BindTechRoles()
        {
            var actionType = this.AdhocRequest.AdhocActionType;
            if (actionType == AdHocActionType.AmendmentRemove || actionType == AdHocActionType.AmendmentAdd)
            {
                long endpointUserId = 0;
                if (ddlEndpointUser.SelectedIndex > 0)
                {
                    endpointUserId = Convert.ToInt32(this.ddlEndpointUser.SelectedValue);
                }
                if (endpointUserId == 0) return;

                Dictionary<string, string> existlist = DomainApiClient<Dto.SMSAccessLevel>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.OneCard.GetExistingRolesByEndpointUserId,
                     new Dictionary<string, string> { {"endpointUserID",endpointUserId.ToString()},
                     {"endpointsystemId", Convert.ToString( (int)this.AdhocRequest.EndpointSystemId) } });

                if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    existlist = DomainApiClient<Dto.SMSAccessLevel>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.OneCard.GetExistingTechRoles,
                         new Dictionary<string, string> { { "UserId", this.AdhocRequest.RequesteeUserId.ToString() }, { "endpointUserID", endpointUserId.ToString() }, { "endpointsystemId", Convert.ToString((int)this.AdhocRequest.EndpointSystemId) } });
                    doBindTechRoles(existlist);
                }

                if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    Dictionary<string, string> list = DomainApiClient<Dto.SMSAccessLevel>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.OneCard.GetTechRolesByBuildingandSection,
                          new Dictionary<string, string> { { "Building", SelectedBuilding }, { "section", this.SelectedSection } });

                    list = list.Except(existlist).ToDictionary(k => k.Key, v => v.Value);
                    doBindTechRoles(list);
                }
            }
            else if (this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
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
        protected void ddlEndpointUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            afterSave();
            if (ddlEndpointUser.SelectedIndex > 0)
            {
                if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                {
                    BindBuilding();
                    ddlSection.Items.Clear();
                    TechnicalRoleCombobox.Items.Clear();
                }
                else
                {
                    BindTechRoles();
                }
            }
            else
            {

                ddlBuilding.Items.Clear();
                ddlSection.Items.Clear();
                TechnicalRoleCombobox.Items.Clear();
            }

            EntitlementRequest = null;
            SMSObjGrid.Visible = false;

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
            if (EntitlementRequest != null)
            {
                var list = EntitlementRequest;
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
                    if (EntitlementRequest != null)
                    {
                        seleteditems = EntitlementRequest;
                    }

                    seleteditems = seleteditems.Count > 0 ? seleteditems.Union(_gridObjlst).ToList() : _gridObjlst;
                    EntitlementRequest = seleteditems;
                }

                if (EntitlementRequest != null)
                {
                    SMSObjGrid.Visible = true;
                    SMSObjGrid.DataSource = EntitlementRequest;
                    SMSObjGrid.DataBind();
                    ddlBuilding.SelectedIndex = 0;
                    ddlSection.SelectedIndex = 0;
                }
                
                TechnicalRoleCombobox.Items.Clear();
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
                    if (EntitlementRequest != null)
                    {
                        seleteditems = EntitlementRequest;
                    }

                    seleteditems = seleteditems.Count > 0 ? seleteditems.Union(_gridObjlst).ToList() : _gridObjlst;
                    EntitlementRequest = seleteditems;
                }

                if (EntitlementRequest != null)
                {
                    SMSObjGrid.Visible = true;
                    SMSObjGrid.DataSource = EntitlementRequest;
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
                List<AdhocSmsGridInfo> _sessionObj = EntitlementRequest;
                _sessionObj.RemoveAll(item => item.TechnicalRole == TechRole);
                EntitlementRequest = _sessionObj;
                if (_sessionObj.Count > 0)
                {
                    SMSObjGrid.DataSource = EntitlementRequest;
                    SMSObjGrid.DataBind();
                }
                else
                {
                    EntitlementRequest = null;
                    SMSObjGrid.Visible = false;
                    
                    if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        TechnicalRoleCombobox.Items.Clear();
                        ddlBuilding.SelectedIndex = 0;
                        ddlSection.SelectedIndex = 0;

                    }
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