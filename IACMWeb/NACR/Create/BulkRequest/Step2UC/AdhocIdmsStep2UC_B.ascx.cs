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
    public partial class AdhocIdmsStep2UC_B :  UIBaseStep2UC_B
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
            btnRoleAdd.Enabled = false;
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
            Session["EndpointUserId"] = 0;
            trGrdid.Visible = false;

            BindRepeater();
            BindEntitlementType();
            if (this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.NewAccount)
            {
                SetMandatory(divRole, true);
                divRole.Visible = true;
            }
            if (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0)
            {
                BindAdhocRequest();
            }
            Session["Action"] = (AdHocActionType)AdHocActionType.NewAccount;
        }

         
        protected override void SetEntitlementDetail()
        {
            AdHocIACMRequest request = new AdHocIACMRequest();
            request.EndpointUserId =  0;
            request.AdHocIACMRequestDetails = this.AdhocRequestDetails;
            AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(request);
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
        void BindRepeater()
        {
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                pnlEndPointUserName.Visible = true;

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
                pnlEndPointUserName.Visible = false;
            }
        }

        void BindEntitlementType()
        {
            ddlEntitlementType.Items.Clear();
            ddlEntitlementType.Items.Add(new ListItem() { Text = "--Select--", Value = Convert.ToString((int)EntitlementType.None) });
            ddlEntitlementType.Items.Add(new ListItem() { Text = "Role", Value = Convert.ToString((int)EntitlementType.Role) });
            ddlEntitlementType.Items.Add(new ListItem() { Text = "Group", Value = Convert.ToString((int)EntitlementType.Group) });
            ddlEntitlementType.SelectedIndex = 2;
            divRole.Visible = false;
            divGroup.Visible = true;
            btnGroupAdd.Enabled = false;
            btnRoleAdd.Enabled = false;
            if (this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.AmendmentRemove)
            {
                btnGroupAdd.Text = "Remove Group";
                btnRoleAdd.Text = "Remove Role";
            }
        }

        public override bool Validate()
        {           
             if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount && !this.AdhocRequestDetails.Any(item => item.EntitlementType == EntitlementType.Group || item.EntitlementType==EntitlementType.Role))
            {
                ShowAlertMessage(AlertType.Error, Constants.addToGridWarning);
                return false; 
            }

            return UpdateBulkReqAccountDetail();
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
                            ShowAlertMessage(Infrastructure.AlertType.Error, "Please enter the Account Information.");
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