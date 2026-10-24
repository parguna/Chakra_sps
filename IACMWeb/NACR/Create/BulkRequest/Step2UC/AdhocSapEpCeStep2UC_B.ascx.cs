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
    public partial class AdhocSapEpCeStep2UC_B : UIBaseStep2UC_B
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void InitializeStep()
        {
            if (AdhocRequest != null)
            {
                txtRequestDetails.Text = "";
                GroupPanelPLRELearn.Visible = true;
                LoadControl();
                BindEndpointUsers();
                BindEntitlementType();
                Session["Action"] = AdHocActionType.NewAccount;
                Session["EndPointSystemId"] = AdhocRequest.EndpointSystemId;
                Session["EndpointUserId"] = 0;
            }
        }

        void BindEntitlementType()
        {
            ddlEntitlementType.Items.Clear();
            ddlEntitlementType.Items.Add(new ListItem() { Text = "--Select--", Value = Convert.ToString((int)EntitlementType.None) });
            ddlEntitlementType.Items.Add(new ListItem() { Text = "Role", Value = Convert.ToString((int)EntitlementType.Role) });
            ddlEntitlementType.Items.Add(new ListItem() { Text = "Group", Value = Convert.ToString((int)EntitlementType.Group) });
            divRole.Visible = false;
            divGroup.Visible = false;
            btnGroupAdd.Enabled = false;
            btnRoleAdd.Enabled = false;
            if (this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.AmendmentRemove)
            {
                btnGroupAdd.Text = "Remove Group";
                btnRoleAdd.Text = "Remove Role";
            }
        }

        void LoadControl()
        {
            if (AdhocRequest != null)
            {
                if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    OtherControls.Visible = false;
                }
                else
                {
                    OtherControls.Visible = true;
                }
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
                    ShowAlertMessage(AlertType.Error, $"The Group - {entitlement} is exists in the request list.");
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
                    ShowAlertMessage(AlertType.Error, $"The Role - {entitlement} is exists in the request list.");
                }
                RolesAutoCompleteBox.Entries.Clear();
            }
            btn.Enabled = false;
        }

        void FillRequestGrid()
        {
            var list = (List<AdHocIACMRequestDetail>)this.AdhocRequestDetails;
            RequestGrid.DataSource = list;
            trGrdid.Visible = list.Count == 0 ? false : true;
            RequestGrid.DataBind();
        }
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

        protected override void BindAdhocRequest()
        {
            var Entitlement = (AdhocSAPEPRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                txtRequestDetails.Text = Entitlement.RequestDetail;
            }
        }
        void BindEndpointUsers()
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
            else
            {
                AccountRepeater.Visible = false;
                Panel1.Visible = false;
            }
        }

        protected override void SetEntitlementDetail()
        {
            AdhocSAPEPRequest sap = new AdhocSAPEPRequest()
            {
                EndpointUserId = 0,
                RequestDetail = txtRequestDetails.Text,
                Roles = string.Join(",", AdhocRequestDetails.Where(r => r.EntitlementType == EntitlementType.Role).Select(r => r.Entitlement)),
                Groups = string.Join(",", AdhocRequestDetails.Where(r => r.EntitlementType == EntitlementType.Group).Select(r => r.Entitlement)),
            };

            sap.AdHocSAPEPSharedAccount = null;
            AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(sap);
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
            return validate() && UpdateBulkReqAccountDetail();
        }
        bool validate()
        {
            bool result = false;
            string Message = "Please fill the ";
            string ControlMessage = "";

            if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount && string.IsNullOrWhiteSpace(txtRequestDetails.Text))
            {
                ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Request Info" : ControlMessage + " , Request Info";
            }
            if (ControlMessage != "")
            {
                int lastIndex = ControlMessage.LastIndexOf(",");
                Message += lastIndex > 0 ? ControlMessage.Substring(0, lastIndex) + " and" + ControlMessage.Substring(lastIndex + 1) : ControlMessage;
                ShowAlertMessage(AlertType.Error, Message + ".");
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }
    }
}