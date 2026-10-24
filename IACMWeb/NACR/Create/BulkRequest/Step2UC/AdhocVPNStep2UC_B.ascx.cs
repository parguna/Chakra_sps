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
    public partial class AdhocVPNStep2UC_B : UIBaseStep2UC_B
    {
        public bool ShowEnviornment
        {
            get
            {
                if (ViewState["ShowEnviornment"] != null)
                {
                    return (bool)ViewState["ShowEnviornment"];
                }
                return false;
            }
            set
            {
                ViewState["ShowEnviornment"] = value;
            }
        }
        
        public bool IsProd
        {
            get
            {
                if (ViewState["IsProd"] != null)
                {
                    return (bool)ViewState["IsProd"];
                }
                return false;
            }
            set
            {
                ViewState["IsProd"] = value;
            }
        }
        public bool IsQA
        {
            get
            {
                if (ViewState["IsQA"] != null)
                {
                    return (bool)ViewState["IsQA"];
                }
                return false;
            }
            set
            {
                ViewState["IsQA"] = value;
            }
        }
        public bool IsDev
        {
            get
            {
                if (ViewState["IsDev"] != null)
                {
                    return (bool)ViewState["IsDev"];
                }
                return false;
            }
            set
            {
                ViewState["IsDev"] = value;
            }
        }
        public bool IsProject
        {
            get
            {
                if (ViewState["IsProject"] != null)
                {
                    return (bool)ViewState["IsProject"];
                }
                return false;
            }
            set
            {
                ViewState["IsProject"] = value;
            }
        }
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
        public VPNType SelectedVPNType
        {
            get
            {
                if (ViewState["VPNType"] != null)
                {
                    return ((VPNType)Enum.Parse(typeof(VPNType), ViewState["VPNType"].ToString()));
                }
                return VPNType.VDI;
            }
            set
            {
                ViewState["VPNType"] = value;
            }
        }
        private List<AdHocVPNRequestDetail> VPNRequests
        {
            get
            {
                if (ViewState["VPNRequests"] != null)
                {
                    return (List<AdHocVPNRequestDetail>)ViewState["VPNRequests"];
                }
                return new List<AdHocVPNRequestDetail>();
            }
            set
            {
                ViewState["VPNRequests"] = value;
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
            BindVPNType();
            if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
            {
                pnlType.Visible = false;
                pnlGUI.Visible = false;
                btnAddRequest.Visible = false;
                ApplicationGrid.Visible = false;
                divVPNType.Visible = false;
                ddlVPNType.Items.Clear();
            }
            var applicationType = AdhocRequest.ApplicationType;
            if (string.IsNullOrEmpty(AdhocRequest.CanInvokeRequest))
                BindAdhocBulkRequest();
            else
            {
                AdhocRequest.CanInvokeRequest = string.Empty;
                VPNRequests.Clear();
                FillApplicationGrid();
            }
        }
        private void BindVPNType()
        {
            divVPNType.Visible = true;
            ddlVPNType.Items.Clear();
            bool isEmployee = true;
            this.AdhocRequest.BulkRequestDetails.ForEach(item =>
            {
                var user = DomainApiClient<Dto.User>.GetById(item.UserId);
                if (user != null && (user.EmployeeType != EmployeeType.TempStaff && user.EmployeeType != EmployeeType.Permanent))
                {
                    isEmployee = false;
                }
            });
            //Only Citrix
            if (isEmployee)
            {
                ddlVPNType.Items.Add(new ListItem(Enum.GetName(typeof(VPNType), VPNType.VDI), ((int)VPNType.VDI).ToString()));
                ddlVPNType.Items.Add(new ListItem(Enum.GetName(typeof(VPNType), VPNType.SSLVPNOnly), ((int)VPNType.SSLVPNOnly).ToString()));
                ddlVPNType.SelectedValue = ((int)VPNType.SSLVPNOnly).ToString();
                SelectedVPNType = VPNType.SSLVPNOnly;
            }
            else
            {
                ddlVPNType.Items.Add(new ListItem(Enum.GetName(typeof(VPNType), VPNType.VDI), ((int)VPNType.VDI).ToString()));
                ddlVPNType.SelectedValue = ((int)VPNType.VDI).ToString();
                SelectedVPNType = VPNType.VDI;
            }
        }
        protected void ddlVPNType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ApplicationAutoCompleteBox.Entries.Clear();
            HideEnviornmentType();
            this.SelectedVPNType =(VPNType)Convert.ToUInt32(ddlVPNType.SelectedValue);
            
        }
        private void BindAdhocBulkRequest()
        {
            var Entitlement = (AdHocVPNRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (Entitlement.AdHocVPNRequestDetails != null)
                    VPNRequests = Entitlement.AdHocVPNRequestDetails;
                FillApplicationGrid();
            }
        }
        void BindEndpointUsers()
        {
            var applicationType = AdhocRequest.ApplicationType;
            Dictionary<string, string> smsData = new Dictionary<string, string>();
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || this.AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
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

        #region Events
        protected void ApplicationGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var requests = this.VPNRequests;
                var identity = (int)((GridDataItem)e.Item).GetDataKeyValue("identity");
                requests.Remove(requests.FirstOrDefault(c => c.identity == identity));
                this.VPNRequests = requests;
                FillApplicationGrid();
            }
        }
        protected void ApplicationAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            if (ApplicationAutoCompleteBox.Entries.Count == 0)
            {
                HideEnviornmentType();
            }
        }
        protected void ApplicationAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {

            string group = string.Empty;

            var NewItem = ApplicationAutoCompleteBox.Entries[ApplicationAutoCompleteBox.Entries.Count - 1];
            ApplicationAutoCompleteBox.Entries.Clear();
            ApplicationAutoCompleteBox.Entries.Add(NewItem);
            try
            {
                var delimiter = new char[] { ',' };
                var adGroups = new List<VPNMatrixData>();
                this.HideEnviornmentType();
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                var matrixId = Convert.ToInt64(NewItem.Value);
                var matrix = DomainApiClient<Dto.VPNMatrix>.GetById(matrixId);
                lblProjectName.Text = matrix.ADGroups;
                lblProductionADNames.Text = matrix.ProductionADgroups;
                lblQAADNames.Text = matrix.QAADGroups;
                lblDevADNames.Text = matrix.DevelpomentADGroups;
                switch (this.AdhocRequest.AdhocActionType)
                {
                    case AdHocActionType.AmendmentAdd:
                    case AdHocActionType.AmendmentRemove:
                        ShowEnviornemntByMatrix(true, matrix);
                        break;
                    default:
                        break;
                }
                this.btnAddRequest.Visible = (this.IsProd || this.IsQA || this.IsDev || chkProjectEnv.Checked);
                ShowEnviornment = this.btnAddRequest.Visible;
            }
            catch
            {
                throw;
            }
        }
        protected void btnAddRequest_Click(object sender, EventArgs e)
        {

            if (this.SelectedVPNType == VPNType.SSLVPNOnly)
            {
                if (!(chkProduction.Checked || chkQA.Checked || chkDevelopment.Checked))
                {
                    ShowAlertMessage(Infrastructure.AlertType.Warning, "Select at least one environment(Production, QA, Development)");
                    return;
                }
            }
            else if (this.SelectedVPNType == VPNType.VDI)
            {
                if (!(chkProjectEnv.Checked))
                {
                    ShowAlertMessage(Infrastructure.AlertType.Warning, "Select the Project");
                    return;
                }
            }
            else
            {
                if (!(chkProduction.Checked || chkQA.Checked || chkDevelopment.Checked || chkProjectEnv.Checked))
                {
                    ShowAlertMessage(Infrastructure.AlertType.Warning, "Select at least one environment from Application(Production, QA, Development)/Project(ADGroups)");
                    return;
                }
            }
            var requests = this.VPNRequests;
            foreach (AutoCompleteBoxEntry Entries in ApplicationAutoCompleteBox.Entries)
            {
                var Id = Convert.ToInt64(Entries.Value);
                var vpnMatrix = DomainApiClient<Dto.VPNMatrix>.GetById(Id);
                string ApplicationName = Entries.Text;
                var dataAlreadyExists = requests.FirstOrDefault(i => i.VPNMatrixId == vpnMatrix.Id);
                if (dataAlreadyExists != null)
                {
                    requests.Remove(dataAlreadyExists);
                }
                AdHocVPNRequestDetail ovpnReq = new AdHocVPNRequestDetail();
                ovpnReq.VPNType = SelectedVPNType;
                ovpnReq.MatrixType = vpnMatrix.VPNMatrixType.ToString();
                ovpnReq.VPNMatrixId = vpnMatrix.Id;
                ovpnReq.ApplicationName = vpnMatrix.Name;
                ovpnReq.Production = chkProduction.Checked;
                ovpnReq.QA = chkQA.Checked;
                ovpnReq.Dev = chkDevelopment.Checked;
                ovpnReq.identity = (this.VPNRequests.Count > 0) ? (this.VPNRequests.Max(c => c.identity) + 1) : 1;
                ovpnReq.isOthers = chkProjectEnv.Checked;
                ovpnReq.ProductionADGroups = vpnMatrix.ProductionADgroups;
                ovpnReq.QAADgroups = vpnMatrix.QAADGroups;
                ovpnReq.DevelopmentADGroups = vpnMatrix.DevelpomentADGroups;
                ovpnReq.ProjectADGroups = vpnMatrix.ADGroups;
                if (requests.Count(c => c.ApplicationName == ApplicationName) == 0)
                {
                    requests.Add(ovpnReq);
                }
            }
            this.VPNRequests = requests;
            ApplicationAutoCompleteBox.Entries.Clear();
            HideEnviornmentType();
            FillApplicationGrid();
        }
        #endregion
        #region private method
        protected void FillApplicationGrid()
        {
            if (this.VPNRequests == null)
            {
                VPNRequests = new List<AdHocVPNRequestDetail>();
            }
            ApplicationGrid.Visible = true;

            ApplicationGrid.DataSource = this.VPNRequests.AsEnumerable().Select(c => c);
            ApplicationGrid.DataBind();
        }
        private void HideEnviornmentType()
        {
            ShowEnviornment = false;
            IsProd = false;
            IsQA = false;
            IsDev = false;
            IsProject = false;

            chkProduction.Checked = false;
            chkQA.Checked = false;
            chkDevelopment.Checked = false;
            chkProjectEnv.Checked = false;

            this.lblProductionADNames.Visible = false;
            this.lblQAADNames.Visible = false;
            this.lblDevADNames.Visible = false;
            this.lblProductionADNames.Visible = false;
            this.lblProjectName.Visible = false;
        }
        private void ShowEnviornemntByMatrix(bool excludeApplication, VPNMatrix matrix)
        {
            chkProjectEnv.Checked = false;
            if (matrix.VPNMatrixType == VPNMatrixType.Application)
            {
                var isProd = !string.IsNullOrWhiteSpace(matrix.ProductionADgroups);
                this.IsProd = isProd;
                //lblProductionADNames.Visible = isProd;
                var isQA = !string.IsNullOrWhiteSpace(matrix.QAADGroups);
                this.IsQA = isQA;
                //lblQAADNames.Visible = isQA;
                var isDev = !string.IsNullOrWhiteSpace(matrix.DevelpomentADGroups);
                this.IsDev = isDev;
                //lblDevADNames.Visible = isDev;
            }
            else
            {
                this.IsProject = false;
                chkProjectEnv.Checked = true;
                lblProjectName.Text = matrix.ADGroups;
            }
        }
        bool ValidateVPN()
        {
            var returnValue = true;
            if (AdhocRequest.ApplicationType == ApplicationType.VPN)
            {
                if (this.VPNRequests.Count == 0 && (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                {
                    returnValue = false;
                    ShowAlertMessage(Infrastructure.AlertType.Error, string.Format("Please select at least one Application/Project"));
                }
            }
            return returnValue;
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
        public override bool Validate()
        {
            return ValidateVPN() && UpdateBulkReqAccountDetail();
        }
        private AdHocVPNRequest GetAdhocVPNRequest()
        {
            var request = new AdHocVPNRequest();
            if (AdhocRequest != null)
            {
                request.EndpointUserId =  0;
                if (AdhocRequest.ApplicationType == ApplicationType.VPN)
                {
                    request.UpdateOneKeyToken = false;
                    request.AdHocVPNRequestDetails = this.VPNRequests;
                }
            }
            return request;
        }
        protected override void SetEntitlementDetail()
        {
            AdHocVPNRequest request = GetAdhocVPNRequest();
            if (AdhocRequest != null)
            {
                request.EndpointUserId = 0;
            }
            AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(request);
        }
        #endregion
    }
}