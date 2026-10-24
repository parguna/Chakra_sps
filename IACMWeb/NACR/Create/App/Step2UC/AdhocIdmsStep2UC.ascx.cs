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
    public partial class AdhocIdmsStep2UC : UIBaseStep2UC
    {

        public List<AdHocIACMRequestDetail> AdhocRequestDetails
        {
            get
            {
                if (ViewState["AdHocIACMRequestDetail"] != null)
                {
                    return (List<AdHocIACMRequestDetail>)ViewState["AdHocIACMRequestDetail"];
                }
                return new List<AdHocIACMRequestDetail>();
            }
            set
            {
                ViewState["AdHocIACMRequestDetail"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected override void afterSave()
        {
            base.afterSave();
            RolesAutoCompleteBox.Entries.Clear();
            AdhocRequestDetails = new List<AdHocIACMRequestDetail>();
            GroupAutoCompleteBox.Entries.Clear();
            RequestGrid.DataSource = new List<AdHocIACMRequestDetail>();
            RequestGrid.DataBind();
        }
        protected void btnRoleAdd_Click(object sender, EventArgs e)
        {
            if (divGroup.Visible)
            {
            }
        }

        protected void ddlEntitlementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEntitlementType.SelectedItem != null && ddlEntitlementType.SelectedIndex > 0)
            {
                divRole.Visible = Convert.ToInt32(ddlEntitlementType.SelectedValue) == (int)EntitlementType.Role;
                divGroup.Visible = Convert.ToInt32(ddlEntitlementType.SelectedValue) == (int)EntitlementType.Group;
            }
        }

        protected void AutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            RadAutoCompleteBox box = (RadAutoCompleteBox)sender;
            if (box.Entries.Count == 2)
            {
                box.Entries.Remove(box.Entries[1]);
            }
            if (box.Entries.Count == 1)
            {
                btnRoleAdd.Enabled = Convert.ToInt32(ddlEntitlementType.SelectedValue) == (int)EntitlementType.Role ? true : false;
                btnGroupAdd.Enabled = Convert.ToInt32(ddlEntitlementType.SelectedValue) == (int)EntitlementType.Group ? true : false;
            }
        }

        protected void AutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            btnRoleAdd.Enabled =  false;
            btnGroupAdd.Enabled = false;
            RadAutoCompleteBox box = (RadAutoCompleteBox)sender;
            if (box.Entries.Count > 0)
            {
                btnRoleAdd.Enabled = Convert.ToInt32(ddlEntitlementType.SelectedValue) == (int)EntitlementType.Role ? true : false;
                btnGroupAdd.Enabled = Convert.ToInt32(ddlEntitlementType.SelectedValue) == (int)EntitlementType.Group ? true : false;
            }
        }

        protected void RequestGrid_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var requests = this.AdhocRequestDetails;
                requests.RemoveAt(e.Item.ItemIndex);
                this.AdhocRequestDetails = requests;
                FillRequestGrid();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            EntitlementType entitlementType = (EntitlementType)Convert.ToInt32(ddlEntitlementType.SelectedValue);
            if (divGroup.Visible && GroupAutoCompleteBox.Entries.Count > 0 && entitlementType == EntitlementType.Group && btn.ID == "btnGroupAdd")
            { 
                string entitlement = GroupAutoCompleteBox.Entries[0].Text;
                if (!AdhocRequestDetails.Any(item => item.EntitlementType == entitlementType && item.Entitlement == entitlement))
                {
                    List<AdHocIACMRequestDetail> list = AdhocRequestDetails;
                    list.Add(new AdHocIACMRequestDetail { Entitlement = entitlement, EntitlementType = entitlementType });
                    AdhocRequestDetails = list;
                    FillRequestGrid();
                     
                }
                else
                { 
                    ShowAlertMessage(AlertType.Error, $"The IDMS Group - {entitlement} is exists in the request list.");  
                }
                GroupAutoCompleteBox.Entries.Clear();
            }
            else if (divRole.Visible && RolesAutoCompleteBox.Entries.Count > 0 && entitlementType == EntitlementType.Role && btn.ID == "btnRoleAdd")
            {
                string entitlement = RolesAutoCompleteBox.Entries[0].Text;
                if (!AdhocRequestDetails.Any(item => item.EntitlementType == entitlementType && item.Entitlement == entitlement))
                {
                    List<AdHocIACMRequestDetail> list = AdhocRequestDetails;
                    list.Add(new AdHocIACMRequestDetail { Entitlement = entitlement, EntitlementType = entitlementType });
                    AdhocRequestDetails = list;
                    FillRequestGrid(); 
                }
                else
                { 
                    ShowAlertMessage(AlertType.Error, $"The IDMS Role - {entitlement} is exists in the request list.");  
                }
                RolesAutoCompleteBox.Entries.Clear();
            }
            btn.Enabled = false;
        }

        protected override void InitializeStep()
        {
            Session["EndpointUserId"] = null;
            trGrdid.Visible = false;
            if (this.AdhocRequest.AdhocActionType != Infrastructure.Enums.AdHocActionType.NewAccount)
            {
                pnlEndPointUserName.Visible = true;
                BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
            }
            BindEntitlementType();
            if (this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.NewAccount)
            {
                SetMandatory(divRole, true);
                divRole.Visible = true;
            }
            if(AdhocRequest.GetEntitlementDetail() != null || (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0))
            {
                BindAdhocRequest();
            }
            Session["Action"] = (AdHocActionType)AdhocRequest.AdhocActionType;
        }

        protected override IEntitlementDetail GetEntitlementDetail()
        {
            AdHocIACMRequest request = new AdHocIACMRequest();
            if (EndPointUserDropDownList != null && EndPointUserDropDownList.SelectedItem != null)
                request.DisplayAccountName = EndPointUserDropDownList.SelectedItem.Text;

            request.EndpointUserId = (this.AdhocRequest.AdhocActionType != Infrastructure.Enums.AdHocActionType.NewAccount && EndPointUserDropDownList.SelectedIndex > 0) ? Convert.ToInt32(EndPointUserDropDownList.SelectedItem.Value) : 0;
            request.AdHocIACMRequestDetails = this.AdhocRequestDetails;
            return request;
        }

        void BindAdhocRequest()
        {
            var Entitlement = (AdHocIACMRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                this.AdhocRequestDetails = Entitlement.AdHocIACMRequestDetails;
                FillRequestGrid();
            }
        }

        void FillRequestGrid()
        {
            var list = (List<AdHocIACMRequestDetail>)this.AdhocRequestDetails;
            RequestGrid.DataSource = list;
            trGrdid.Visible = list.Count == 0 ? false : true;
            RequestGrid.DataBind();
        }
        void BindEndpointUsers(long userId, long endpointSystemId, AccountType accountType)
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
                Session["EndpointUserId"] = null;
                if (EndPointUserDropDownList.SelectedItem != null && EndPointUserDropDownList.SelectedIndex > 0 && (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd))
                {
                    Session["EndpointUserId"] = Convert.ToInt32(EndPointUserDropDownList.SelectedValue);
                }
            }
        }

        void BindEntitlementType()
        {
            ddlEntitlementType.Items.Clear();
            ddlEntitlementType.Items.Add(new ListItem() { Text = "--Select--", Value = Convert.ToString((int)EntitlementType.None) });
            var roleItem = new ListItem() { Text = "Role", Value = Convert.ToString((int)EntitlementType.Role) };
            ddlEntitlementType.Items.Add(roleItem);
            var groupItem = new ListItem() { Text = "Group", Value = Convert.ToString((int)EntitlementType.Group) };
            ddlEntitlementType.Items.Add(groupItem);
            ddlEntitlementType.SelectedIndex = 1;
            divRole.Visible = false;
            divGroup.Visible = false;
            btnGroupAdd.Enabled = false;
            btnRoleAdd.Enabled = false;
            //if (this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.AmendmentRemove)
            //{
            //    btnRoleAdd.Text = "Remove Role";
            //}
            //else
            //{
            //    btnRoleAdd.Text = "Add Role";
            //}
            ddlEntitlementType_SelectedIndexChanged(ddlEntitlementType, null);
        }

        public override bool Validate()
        {
            if (pnlEndPointUserName.Visible && EndPointUserDropDownList.SelectedIndex <= 0 && AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                ShowAlertMessage(AlertType.Error, "Please select EndPoint User");
            }
            if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount && !this.AdhocRequestDetails.Any())
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, Constants.addToGridWarning);
                return false;
            } 

            return true;
        }

        protected void EndPointUserDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            afterSave();
        }
    }
}