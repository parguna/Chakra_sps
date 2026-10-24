using DI.Chakra.Dto;
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
    public partial class AdhocSkypeForBusinessStep2UC_B : UIBaseStep2UC_B
    {
        public string EndpointSystemId
        {
            get
            {
                if (ViewState["EndpointId"] != null)
                {
                    return ViewState["EndpointId"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ViewState["EndpointId"] = value;
            }
        }
        public string AdhocActionType
        {
            get
            {
                if (ViewState["AdhocActionType"] != null)
                {
                    return ViewState["AdhocActionType"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ViewState["AdhocActionType"] = value;
            }
        }


        private List<ResponseGroup> ResponseGroups
        {
            get
            {
                if (ViewState["SFBResponseGroups"] != null)
                {
                    return (List<ResponseGroup>)ViewState["SFBResponseGroups"];
                }
                return new List<ResponseGroup>();
            }
            set
            {
                ViewState["SFBResponseGroups"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override void InitializeStep()
        {
            if (AdhocRequest == null) return;

            EndpointSystemId = AdhocRequest.EndpointSystemId.ToString();

            BindEndpointUsers();
            LoadEntitlementType();
            var applicationType = AdhocRequest.ApplicationType;
            var action = AdhocRequest.AdhocActionType;
            this.AdhocActionType = action.ToString();
            if (string.IsNullOrEmpty(AdhocRequest.CanInvokeRequest))
                BindAdhocBulkRequest();
            else
            {
                AdhocRequest.CanInvokeRequest = string.Empty;
                ResponseGroups.Clear();
                FillApplicationGrid();
            }
        }
        public override bool Validate()
        {
            return ValidateSkypeForBusiness() && UpdateBulkReqAccountDetail();
        }
        protected override void SetEntitlementDetail()
        {
            AdhocSkypeForBusinessRequest request = GetAdhocSkypeForBusinessRequest();
            if (AdhocRequest != null)
            {
                request.EndpointUserId = 0;
            }
            AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(request);
        }
        protected void ResponseGroupAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            string group = string.Empty;
            divMask.Visible = false;
            chkMask.Checked = false;
            btnAddRequest.Visible = true;
            var NewItem = ResponseGroupAutoCompleteBox.Entries[ResponseGroupAutoCompleteBox.Entries.Count - 1];
            ResponseGroupAutoCompleteBox.Entries.Clear();
            ResponseGroupAutoCompleteBox.Entries.Add(NewItem);
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                var entitlementType = (EntitlementType)(Convert.ToInt32(ddlEntitlementType.SelectedValue));
                if (entitlementType == EntitlementType.ResponseGroup)
                {
                    foreach (AutoCompleteBoxEntry Entries in ResponseGroupAutoCompleteBox.Entries)
                    {
                        var Id = Convert.ToInt64(Entries.Value);
                        var resGroup = DomainApiClient<Dto.SkypeForBusinessResponseGroupInfo>.GetById(Id);
                        if (resGroup != null)
                        {
                            var param = new Dictionary<string, string>();
                            param.Add("name", resGroup.Name);
                            param.Add("entitlementType", EntitlementType.ResponseGroup.ToString());
                            var resGroupMatrix = DomainApiClient<Dto.SkypeForBusinessProvisioningMatrix>.GetByAction<Dto.SkypeForBusinessProvisioningMatrix>(DomainApiAction.Matrix.SkypeForBusinessMatrix.GetSkypeForBusinessMatrixByName, param);
                            if (resGroupMatrix != null && !string.IsNullOrWhiteSpace(resGroupMatrix.ADID))
                            {
                                divMask.Visible = !string.IsNullOrWhiteSpace(resGroupMatrix.ADID);
                            }
                        }
                    }
                }
            }
            else if(this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
            {
                var entitlementType = (EntitlementType)(Convert.ToInt32(ddlEntitlementType.SelectedValue));
                if (entitlementType == EntitlementType.ResponseGroup)
                {
                    foreach (AutoCompleteBoxEntry Entries in ResponseGroupAutoCompleteBox.Entries)
                    {
                        var Id = Convert.ToInt64(Entries.Value);
                        var resGroup = DomainApiClient<Dto.SkypeForBusinessResponseGroupInfo>.GetById(Id);
                        if (resGroup != null)
                        {
                            var param = new Dictionary<string, string>();
                            param.Add("name", resGroup.Name);
                            param.Add("entitlementType", EntitlementType.ResponseGroup.ToString());
                            var resGroupMatrix = DomainApiClient<Dto.SkypeForBusinessProvisioningMatrix>.GetByAction<Dto.SkypeForBusinessProvisioningMatrix>(DomainApiAction.Matrix.SkypeForBusinessMatrix.GetSkypeForBusinessMatrixByName, param);
                            if (resGroupMatrix != null && !string.IsNullOrWhiteSpace(resGroupMatrix.ADID))
                            {
                                divMask.Visible = !string.IsNullOrWhiteSpace(resGroupMatrix.ADID);
                            }
                        }
                    }
                }
            }
            
        }
        protected void ResponseGroupAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            if (ResponseGroupAutoCompleteBox.Entries.Count == 0)
            {

            }
            this.divMask.Visible = false;
        }
        protected void btnAddRequest_Click(object sender, EventArgs e)
        {
            AddResponseGroup();
        }
        private void AddResponseGroup()
        {
            if (ResponseGroupAutoCompleteBox.Entries.Count == 0)
            {
                ShowAlertMessage(Infrastructure.AlertType.Warning, "Please select ResponseGroup");
                return;
            }

            var responses = this.ResponseGroups;
            foreach (AutoCompleteBoxEntry Entries in ResponseGroupAutoCompleteBox.Entries)
            {
                var Id = Convert.ToInt64(Entries.Value);
                var group = DomainApiClient<Dto.SkypeForBusinessResponseGroupInfo>.GetById(Id);
                if (responses != null && responses.Count > 0 && responses.Any(i => i.Id == Id))
                {
                    ShowAlertMessage(Infrastructure.AlertType.Warning, $"ResponseGroup:{group.Name} already added. Please add another ResponseGroup.");
                    return;
                }

                var responseGroup = new ResponseGroup();
                responseGroup.Id = group.Id;
                responseGroup.Identity = group.Identity;
                responseGroup.Name = group.Name;
                responseGroup.Mask = chkMask.Checked;
                responses.Add(responseGroup);
            }
            this.ResponseGroups = responses;
            ResponseGroupAutoCompleteBox.Entries.Clear();
            this.divMask.Visible = false;
            this.chkMask.Checked = false;
            FillApplicationGrid();
        }
        protected void ResponseGroupGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var requests = ResponseGroups;
                var identity = Convert.ToInt64(((GridDataItem)e.Item).GetDataKeyValue("Id"));
                requests.Remove(requests.FirstOrDefault(c => c.Id == identity));
                this.ResponseGroups = requests;
                FillApplicationGrid();
            }
        }
        protected void ddlEntitlementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEntitlementType.SelectedIndex == 0)
            {
                if (this.AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
                {
                    ShowAlertMessage(Infrastructure.AlertType.Warning, "Please select Type");
                    this.ResponseGroups = new List<ResponseGroup>();
                    return;
                }
            }
            EntitlementType type = (EntitlementType)Enum.Parse(typeof(EntitlementType), ddlEntitlementType.SelectedItem.Value, true);
            if(type == EntitlementType.ResponseGroup)
            {
                pnlResponseGroupSelection.Visible = true;
                pnlResponseGroup.Visible = true;
                btnAddRequest.Visible = true;
                divMask.Visible = false;
                chkMask.Checked = false;
            }
        }
        #region privatemethod
        private AdhocSkypeForBusinessRequest GetAdhocSkypeForBusinessRequest()
        {
            AdhocSkypeForBusinessRequest request = null;
            if (AdhocRequest != null)
            {
                request = new AdhocSkypeForBusinessRequest();
                request.EndpointUserId =  0;
                EntitlementType entitlementType = (EntitlementType)Enum.Parse(typeof(EntitlementType), ddlEntitlementType.SelectedItem.Value, true);
                request.EntitlementType = entitlementType;
                if (entitlementType == EntitlementType.ResponseGroup)
                {
                    request.SkypeForBusinessResponseGroups = this.ResponseGroups;
                }
            }
            return request;
        }
        public bool ValidateSkypeForBusiness()
        {
            if(this.ResponseGroups.Count == 0)
            {
                ShowAlertMessage(Infrastructure.AlertType.Warning, "Please add ResponseGroup(s)");
                return false;
            }
            return true;
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
        void LoadEntitlementType()
        {
            ddlEntitlementType.Items.Clear();
            ddlEntitlementType.Items.Add(new ListItem("-- Select --", ((int)EntitlementType.None).ToString()));
            ddlEntitlementType.Items.Add(new ListItem(EntitlementType.ResponseGroup.ToString(), ((int)EntitlementType.ResponseGroup).ToString()));
        }
        private void BindAdhocBulkRequest()
        {
            var Entitlement = (AdhocSkypeForBusinessRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (Entitlement.SkypeForBusinessResponseGroups != null)
                {
                    this.ResponseGroups = Entitlement.SkypeForBusinessResponseGroups;
                    ddlEntitlementType.SelectedValue = ((int)EntitlementType.ResponseGroup).ToString();
                    pnlResponseGroupSelection.Visible = true;
                    pnlResponseGroup.Visible = true;
                    divMask.Visible = false;
                }
                    
                FillApplicationGrid();
            }
        }
        protected void FillApplicationGrid()
        {
            if (this.ResponseGroups == null)
            {
                ResponseGroups = new List<ResponseGroup>();
            }
            var visible = (ResponseGroups.Count > 0);
            ResponseGroupGrid.DataSource = this.ResponseGroups.AsEnumerable().Select(c => c);
            ResponseGroupGrid.DataBind();
            
            ResponseGroupGrid.Visible = visible;
        }
        void ClearFields()
        {
            Session["AdHocSFBInfo"] = null;
            ResponseGroupGrid.Visible = false;
            pnlResponseGroup.Visible = false;
            ResponseGroupAutoCompleteBox.Entries.Clear();
        }
        void BindEndpointUsers()
        {
            var applicationType = AdhocRequest.ApplicationType;
            Dictionary<string, string> smsData = new Dictionary<string, string>();
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
            {
                AccountRepeater.Visible = true;
                pnlEndPointUserName.Visible = true;
                Dictionary<string, Dictionary<long, string>> repeaterData = new Dictionary<string, Dictionary<long, string>>();
                string endpointSystemId = AdhocRequest.EndpointSystemId.ToString();
                AdhocRequest.BulkRequestDetails.ForEach(item =>
                {
                    var param = new Dictionary<string, string>
                    {
                        { "userId",item.UserId.ToString()},
                        { "endpointSystemId", endpointSystemId},
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
                        string sms = string.Empty;
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
        #endregion
    }
}