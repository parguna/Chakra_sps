using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.App.Step2UC
{
    public partial class AdhocVPNStep2UC : UIBaseStep2UC
    {
        private const string V_ADHOCREQUEST = "V_ADHOCREQUEST";
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
        //public VPNType SelectedVPNType
        //{
        //    get
        //    {
        //        if(string.IsNullOrEmpty(hiddenVPNType.Value))

        //        if (ViewState["VPNType"] != null)
        //        {
        //            return ((VPNType)Enum.Parse(typeof(VPNType), ViewState["VPNType"].ToString()));
        //        }
        //        return VPNType.CITRIX;
        //    }
        //    set
        //    {
        //        ViewState["VPNType"] = value;
        //    }
        //}
        public bool RequsteeIsFullTimeEmployee {
            get
            {
                if (ViewState["IsFullTimeEmployee"] != null)
                {
                    return (bool)ViewState["IsFullTimeEmployee"];
                }
                return true;
            }
            set
            {
                ViewState["IsFullTimeEmployee"] = value;
            }
        }
        public long SelectedVPNApplicationEnviornment
        {
            get
            {
                if (ViewState["VPNApplicationEnviornmentId"] != null)
                {
                    return (long)ViewState["VPNApplicationEnviornmentId"];
                }
                return 0;
            }
            set
            {
                ViewState["VPNApplicationEnviornmentId"] = value;
            }
        }
        public Dictionary<string, string> CurrentEntitlement
        {
            get
            {
                if (ViewState["V_CurrentEntitlement"] != null)
                {
                    return (Dictionary<string, string>)ViewState["V_CurrentEntitlement"];
                }
                return null;
            }
            set
            {
                ViewState["V_CurrentEntitlement"] = value;
            }
        }
        #region Properties
        public AdHocAccessRequest AdhocRequest
        {
            get
            {
                if (ViewState[V_ADHOCREQUEST] != null)
                {
                    return (AdHocAccessRequest)ViewState[V_ADHOCREQUEST];
                }
                return null;
            }
            set
            {
                ViewState[V_ADHOCREQUEST] = value;
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
        private List<VPNMatrix> VPNMatrixData
        {
            get
            {
                if(ViewState["VPNMatrix"] != null)
                    return ViewState["VPNMatrix"] as List<VPNMatrix>;
                return new List<VPNMatrix>();
            }
            set
            {
                ViewState["VPNMatrix"] = value;
            }
        }
        private bool IsAmendmentRemove
        {
            get
            {
                if (ViewState["IsAmendmentRemove"] != null)
                {
                    return (bool)ViewState["IsAmendmentRemove"];
                }
                return false;
            }
            set
            {
                ViewState["IsAmendmentRemove"] = value;
            }
        }
        private bool IsDeleteRequest
        {
            get
            {
                if (ViewState["IsDeleteRequest"] != null)
                {
                    return (bool)ViewState["IsDeleteRequest"];
                }
                return false;
            }
            set
            {
                ViewState["IsDeleteRequest"] = value;
            }
        }
        public string VPNAdhocActionType
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
        public string EndpointUserId
        {
            get
            {
                if (ViewState["EndpointUserId"] != null)
                {
                    return ViewState["EndpointUserId"].ToString();
                }
                return "0";
            }
            set
            {
                ViewState["EndpointUserId"] = value;
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
        public string LabelText
        {
            get
            {
                return  "Application/Project Name";
            }
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override void InitializeStep()
        {
            if (AdhocRequest == null) return;
           
            SetEmployeeType();
            BindVPNType();
            this.btnAddRequest.Visible = false;
            var action = AdhocRequest.AdhocActionType;
            this.VPNAdhocActionType = action.ToString();
            this.EndpointSystemId = AdhocRequest.EndpointSystemId.ToString();
            this.HideEnviornmentType();
            var actions = new[] { AdHocActionType.DeleteAccount, AdHocActionType.AmendmentAdd, AdHocActionType.AmendmentRemove };
            if (actions.Contains(action))
            {
                BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
                if (ddlEndpointUser.SelectedIndex > 0)
                {
                    this.txtOneKeyToken.Text = ddlEndpointUser.SelectedItem.Text;
                }
                this.EndpointUserId = ddlEndpointUser.SelectedValue;
                this.ddlEndpointUser.Enabled = false;
            }
            else
            {
                ddlEndpointUser.Items.Clear();
            }
            ShowControl();
            VPNRequests = new List<AdHocVPNRequestDetail>();
            FillApplicationGrid();
            if (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0)
            {
                BindAdhocRequest();
            }
        }

        private void ResetControl(AdHocActionType action)
        {
            if (action == AdHocActionType.NewAccount)
            {
                pnlEndPointUserName.Visible = false;
                chkUpdateOneKeyToken.Checked = false;
                chkUpdateOneKeyToken.Visible = false;
                txtOneKeyToken.Visible = false;
                txtOneKeyToken.Text = "";
            }
            if (action == AdHocActionType.AmendmentAdd)
            {
                this.txtOneKeyToken.Visible = false;
                txtOneKeyToken.ReadOnly = true;
                chkUpdateOneKeyToken.Visible = true;
                chkUpdateOneKeyToken.Checked = false;
                pnlEndPointUserName.Visible = true;
            }
            if (action == AdHocActionType.AmendmentRemove)
            {
                this.txtOneKeyToken.Visible = false;
                txtOneKeyToken.ReadOnly = true;
                chkUpdateOneKeyToken.Visible = false;
                pnlEndPointUserName.Visible = true;

            }
            if (action == AdHocActionType.DeleteAccount)
            {
                pnlEndPointUserName.Visible = true;
                txtOneKeyToken.ReadOnly = true;
                this.txtOneKeyToken.Visible = false;
                chkUpdateOneKeyToken.Visible = false;
                chkUpdateOneKeyToken.Checked = false;
                pnlGUI.Visible = false;
                btnAddRequest.Visible = false;
                ApplicationGrid.Visible = false;
            }
        }

        private void BindAdhocRequest()
        {
            var Entitlement = (AdHocVPNRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (ddlEndpointUser.Items.FindByValue(Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId)) != null)
                    ddlEndpointUser.SelectedValue = Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId);

                if (Entitlement.AdHocVPNRequestDetails != null)
                    VPNRequests = Entitlement.AdHocVPNRequestDetails;
                FillApplicationGrid();
            }
        }
        private void SetEmployeeType()
        {
            var user = DomainApiClient<Dto.User>.GetById(AdhocRequest.RequesteeUserId);
            if (user != null && user.EmployeeType == EmployeeType.Permanent || user.EmployeeType == EmployeeType.TempStaff)
            {
                RequsteeIsFullTimeEmployee = true;
            }
            else
            {
                RequsteeIsFullTimeEmployee = false;
            }
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
                ddlEndpointUser.Items.Clear();
                DropDownHelper.DropDownListBind(ddlEndpointUser, list.AccountReconDictionary, "Value", "Key", null, true, "-- Select EndPoint User --", false);

                CurrentEntitlement = list.EntitlementReconcilations.ToDictionary(item => item.Entitlement, item => item.Entitlement);
            }
        }
        public override bool Validate()
        {
            var returnValue = true;
            if (AdhocRequest.ApplicationType == ApplicationType.VPN)
            {
                if ((AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd) && this.ddlEndpointUser.SelectedIndex <= 0)
                {
                    ShowAlertMessage(Infrastructure.AlertType.Error, "Please select Endpoint User.");
                    return false;
                }
                var txtOneKey = txtOneKeyToken.Text.Trim();
                if (!string.IsNullOrWhiteSpace(txtOneKey) && ((AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && this.chkUpdateOneKeyToken.Checked) ||
                    AdhocRequest.AdhocActionType == AdHocActionType.NewAccount))
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("accountName", txtOneKey);
                    if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && this.chkUpdateOneKeyToken.Checked)
                    {
                        if (txtOneKey.Equals(ddlEndpointUser.SelectedItem.Text,StringComparison.InvariantCultureIgnoreCase))
                        {
                            ShowAlertMessage(Infrastructure.AlertType.Error, $"Token Number:{txtOneKey} should not be same as existing Token Number");
                            return false;
                        }
                        parameter.Add("userid", AdhocRequest.RequesteeUserId.ToString());
                        
                    }
                    
                    parameter.Add("EndpointSystemId", AdhocRequest.EndpointSystemId.ToString());
                    var  accountRecon = DomainApiClient<Dto.AccountReconcilation>.GetByAction<Dto.AccountReconcilation>
                        (DomainApiAction.Provisioning.AccountReconcilation.DoesAccountExistInAccountRecon, parameter);
                    if (accountRecon !=null && accountRecon.IsDeleted ==false)
                    {
                        ShowAlertMessage(Infrastructure.AlertType.Error, $"Token Number:{txtOneKey} already exists. Please enter valid Token Number.");
                        return false;
                    }
                }
                if (this.VPNRequests.Count == 0 && ((AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && chkUpdateOneKeyToken.Checked == false) || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                {
                    returnValue = false;
                    ShowAlertMessage(Infrastructure.AlertType.Error, string.Format("Please select at least one Application/Project"));
                    return returnValue;
                }
            }
            returnValue = ValidateRequest(this.AdhocRequest);
            return returnValue;
        }
        protected bool ValidateRequest(AdHocAccessRequest accessRequest)
        {
            if (accessRequest == null)
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "AdHocAccessRequest is null.");
                return false;
            }

            string validationMsg = string.Empty;
            bool result = AdhocAccessRequestValidation.IsExists(accessRequest, out validationMsg);
            if (result && !string.IsNullOrEmpty(validationMsg))
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, validationMsg);
                return false;
            }
            return true;
        }
        protected override IEntitlementDetail GetEntitlementDetail()
        {
            var request = new AdHocVPNRequest();
            request = GetAdhocVPNRequest();
            return request;
        }
        private AdHocVPNRequest GetAdhocVPNRequest()
        {
            var request = new AdHocVPNRequest();
            if (AdhocRequest != null)
            {
                request.EndpointUserId = (ddlEndpointUser.SelectedIndex > 0) ? Convert.ToInt32(ddlEndpointUser.SelectedItem.Value) : 0;

                if (ddlEndpointUser.SelectedItem != null)
                {
                    request.DisplayAccountName = ddlEndpointUser.SelectedItem.Text;
                }

                if (AdhocRequest.ApplicationType == ApplicationType.VPN)
                {
                    request.UpdateOneKeyToken = chkUpdateOneKeyToken.Checked;
                    if(AdhocRequest.AdhocActionType == AdHocActionType.NewAccount || (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && chkUpdateOneKeyToken.Checked)) 
                    {

                        request.OneKey = txtOneKeyToken.Text;
                    }
                    else
                    {
                        request.OneKey = ddlEndpointUser.SelectedItem.Text;
                    }
                    

                    if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount || (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd ) || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        request.AdHocVPNRequestDetails = this.VPNRequests;
                    }
                }
            }
            return request;
        }
        protected void ApplicationAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            if (ApplicationAutoCompleteBox.Entries.Count == 0)
            {
                btnAddRequest.Visible = false;
                HideEnviornmentType();
            }
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
        protected void btnAddRequest_Click(object sender, EventArgs e)
        {

            var selectedVPN = Convert.ToInt32(this.ddlVPNType.SelectedItem.Value);
            if (selectedVPN == (int)VPNType.SSLVPNOnly)
            {
                if (!(chkProduction.Checked || chkQA.Checked || chkDevelopment.Checked))
                {
                    ShowAlertMessage(Infrastructure.AlertType.Warning, "Select at least one environment(Production, QA, Development)");
                    return;
                }
            }else if (selectedVPN == (int)VPNType.VDI)
            {
                if (!(chkProjectEnv.Checked))
                {
                    ShowAlertMessage(Infrastructure.AlertType.Warning, "Please select the Project");
                    return;
                }
            }
            else
            {
                if (!(chkProduction.Checked || chkQA.Checked || chkDevelopment.Checked))
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
                if (vpnMatrix != null)
                {
                    var dataAlreadyExists = requests.FirstOrDefault(i => i.VPNMatrixId == vpnMatrix.Id);
                    if (dataAlreadyExists != null)
                    {
                        requests.Remove(dataAlreadyExists);
                    }
                    AdHocVPNRequestDetail ovpnReq = new AdHocVPNRequestDetail();
                    ovpnReq.VPNType = (VPNType)(Convert.ToInt32(ddlVPNType.SelectedItem.Value));
                    if (vpnMatrix.VPNMatrixType == VPNMatrixType.Application)
                    {
                        ovpnReq.MatrixType = "Application";
                    }
                    else
                    {
                        ovpnReq.MatrixType = "Project";
                    }
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
            }
            this.VPNRequests = requests;
            ApplicationAutoCompleteBox.Entries.Clear();
            HideEnviornmentType();
            FillApplicationGrid();
        }

        protected void FillApplicationGrid()
        {
            if (this.VPNRequests == null)
            {
                VPNRequests = new List<AdHocVPNRequestDetail>();
            }
            ApplicationGrid.DataSource = this.VPNRequests.AsEnumerable().Select(c => c);
            ApplicationGrid.DataBind();
        }
        private void ShowEnviornemntByMatrix(bool excludeApplication, VPNMatrix matrix)
        {
            //Requestee,exlcude,MatrixId
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("excludeApplication", excludeApplication.ToString());
            parameter.Add("endPointUserId",this.EndpointUserId);
            parameter.Add("matrixId", matrix.Id.ToString());
            var data = DomainApiClient<Dto.VPNMatrix>.
                GetByAction<Dictionary<VPNMatrixEnviornmentType, bool>>(DomainApiAction.Matrix.VPNMatrix.GetApplicationEnviornment, parameter);
            foreach (var item in data)
            {
                if (item.Key == VPNMatrixEnviornmentType.Production)
                {
                    IsProd = item.Value;
                    lblProductionADNames.Visible = item.Value;
                }
                
                if (item.Key == VPNMatrixEnviornmentType.QA)
                {
                    IsQA = item.Value;
                    lblQAADNames.Visible = item.Value;
                }
                if (item.Key == VPNMatrixEnviornmentType.Development)
                {
                    IsDev = item.Value;
                    lblDevADNames.Visible = item.Value;
                }
                if (item.Key == VPNMatrixEnviornmentType.ADGroup)
                {
                    IsProject = false;
                    chkProjectEnv.Checked = true;
                    //lblProjectName.Visible = item.Value;
                }
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
                //lblProjectName.Text = matrix.ADGroups;
                //lblProductionADNames.Text = matrix.ProductionADgroups;
                //lblQAADNames.Text = matrix.QAADGroups;
                //lblDevADNames.Text = matrix.DevelpomentADGroups;

                switch (this.AdhocRequest.AdhocActionType)
                {
                    case AdHocActionType.NewAccount:
                        if(matrix.VPNMatrixType == VPNMatrixType.Application)
                        {
                            var isProd = !string.IsNullOrWhiteSpace(matrix.ProductionADgroups);
                            this.IsProd = isProd;
                            var isQA = !string.IsNullOrWhiteSpace(matrix.QAADGroups);
                            this.IsQA = isQA;
                            var isDev = !string.IsNullOrWhiteSpace(matrix.DevelpomentADGroups);
                            this.IsDev = isDev;
                        }
                        else
                        {
                            var isProject = !string.IsNullOrWhiteSpace(matrix.ADGroups);
                            this.IsProject = false;
                            chkProjectEnv.Checked = true;
                            //lblProjectName.Visible = isProject;
                            if (this.IsProject)
                            {
                                chkProjectEnv.Text = matrix.BusinessRole;
                            }
                        }
                        
                        break;
                    case AdHocActionType.AmendmentAdd:
                        ShowEnviornemntByMatrix(true, matrix);
                        break;
                    case AdHocActionType.AmendmentRemove:
                        ShowEnviornemntByMatrix(false, matrix);
                        break;
                    default:
                        break;
                }
                this.btnAddRequest.Visible = (this.IsProd || this.IsQA || this.IsDev || !this.IsProject);
                ShowEnviornment = this.btnAddRequest.Visible;
            }
            catch
            {
                throw;
            }
        }
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

        protected void ApplicationGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            
        }

        protected void chkUpdateOneKeyToken_CheckedChanged(object sender, EventArgs e)
        {
            UpdateOneKeyTokenEnabled();
        }
        private void UpdateOneKeyTokenEnabled()
        {
            var show = !chkUpdateOneKeyToken.Checked;
            divVPNType.Visible = show;
            this.pnlGUI.Visible = show;
            this.btnAddRequest.Visible = show;
            ApplicationGrid.Visible = show;
            if(chkUpdateOneKeyToken.Checked) lblOneKeyTokenLabel.InnerText = "Token Number";
            else lblOneKeyTokenLabel.InnerText = "";
            if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd && chkUpdateOneKeyToken.Checked)
            {
                requiredFieldAppln.Visible = false;
                requiredFieldType.Visible = false;
                divVPNType.Visible = true;
                txtOneKeyToken.Text = "";
                txtOneKeyToken.ReadOnly = false;
                txtOneKeyToken.Visible = false;
                this.pnlGUI.Visible = true;
                this.btnAddRequest.Visible = true;
                ApplicationGrid.Visible = true;
                HideEnviornmentType();
                FillApplicationGrid();
            }
            else
            {
                requiredFieldAppln.Visible = true;
                requiredFieldType.Visible = true;
                txtOneKeyToken.Visible = false;
                txtOneKeyToken.ReadOnly = true;
            }
        }
        protected override void afterSave()
        {
            base.afterSave();
            this.txtOneKeyToken.Text = "";
            VPNRequests = new List<AdHocVPNRequestDetail>();
            ShowControl();
            HideEnviornmentType();
            FillApplicationGrid();
        }
        private void ShowControl()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userID", this.AdhocRequest.RequesteeUserId.ToString());
            var adUserInfo = DomainApiClient<Dto.ADSUserInfo>.GetByAction<Dto.ADSUserInfo>("GetActiveADAccountForVPN", parameter);

            if (adUserInfo != null)
            {
                pnlADEndpoint.Visible = true;
                lblADAccountName.Text = adUserInfo?.sAMAccountName;
            }
            else
            {
                pnlADEndpoint.Visible = false;
                lblADAccountName.Text = string.Empty;
            }
            this.ApplicationAutoCompleteBox.Entries.Clear();
            this.pnlOneKeyToken.Visible = false;
            this.btnSave.Visible = false;
            this.btnAddRequest.Visible = false;
            this.txtOneKeyToken.Text = "";
            this.txtOneKeyToken.ReadOnly = true;
            this.txtOneKeyToken.Visible = false;
            this.chkUpdateOneKeyToken.Visible = false;
            this.chkUpdateOneKeyToken.Checked = false;
            this.pnlGUI.Visible = false;
            AdHocVPNRequest Entitlement = null;
            if (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0)
            {
                Entitlement = (AdHocVPNRequest)AdhocRequest.GetEntitlementDetail();
            }
            if(this.AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
            {
                this.lblOneKeyTokenLabel.InnerText = "Token Number";
            }
            else
            {
                this.lblOneKeyTokenLabel.InnerText = "";
                if (chkUpdateOneKeyToken.Visible)
                {
                    if (chkUpdateOneKeyToken.Checked)
                    {
                        this.lblOneKeyTokenLabel.InnerText = "Token Number";
                    }
                }
            }
            switch (this.AdhocRequest.AdhocActionType)
            {
                case AdHocActionType.NewAccount:
                    NewAccount(Entitlement);
                    break;
                case AdHocActionType.AmendmentAdd:
                    AmendmentAdd(Entitlement);
                    break;
                case AdHocActionType.AmendmentRemove:
                    AmendmentRemove(Entitlement);
                    break;
                case AdHocActionType.DeleteAccount:
                    DeleteAccountPanel(Entitlement);
                    break;
                default:
                    break;
            }
        }
        private void NewAccount(AdHocVPNRequest request)
        {
            lblOneKeyTokenLabel.InnerText = "Token Number";
            this.pnlEndPointUserName.Visible = false;
            this.chkUpdateOneKeyToken.Visible = false;
            this.chkUpdateOneKeyToken.Checked = false;
            this.txtOneKeyToken.Visible = false;
            this.pnlGUI.Visible = true;
            this.pnlOneKeyToken.Visible = true;
            this.txtOneKeyToken.ReadOnly = false;
            requiredFieldAppln.Visible = false;
            requiredFieldType.Visible = false;
            if (request != null)
            {
                this.txtOneKeyToken.Text = request.OneKey;
            }
            var defaultAppText = MasterConfigHelper.VPNDefaultApplicationText;
            if (!string.IsNullOrWhiteSpace(defaultAppText))
            {
                divDefaultAppln.Visible = true;
                lblDefaultApplicationText.Text = defaultAppText;
            }
        }
        private void AmendmentAdd(AdHocVPNRequest request)
        {
            requiredFieldAppln.Visible = true;
            requiredFieldType.Visible = true;
            this.chkUpdateOneKeyToken.Visible = true;
            this.chkUpdateOneKeyToken.Checked = false;
            this.pnlEndPointUserName.Visible = true;
            this.txtOneKeyToken.ReadOnly = false;
            this.txtOneKeyToken.Visible = false;
            this.pnlOneKeyToken.Visible = true;
            this.pnlGUI.Visible = true;
            if (request != null)
            {
                if (request.UpdateOneKeyToken)
                {
                    requiredFieldAppln.Visible = false;
                    requiredFieldType.Visible = false;
                    divVPNType.Visible = true;
                    this.chkUpdateOneKeyToken.Checked = true;
                    this.txtOneKeyToken.Text = request.OneKey;
                    this.txtOneKeyToken.Visible = false;
                    this.txtOneKeyToken.ReadOnly = false;
                    if(request !=null && request.AdHocVPNRequestDetails.Count > 0)
                    {
                        this.VPNRequests = request.AdHocVPNRequestDetails;
                    }
                    this.ApplicationGrid.Visible = true;
                }
                else
                {
                    this.pnlGUI.Visible = true;
                    ApplicationGrid.Visible = true;
                }
            }
        }
        private void AmendmentRemove(AdHocVPNRequest request)
        {
            requiredFieldAppln.Visible = true;
            requiredFieldType.Visible = true;
            this.pnlEndPointUserName.Visible = true;
            this.pnlOneKeyToken.Visible = false;
            this.txtOneKeyToken.ReadOnly = false;
            this.txtOneKeyToken.Visible = false;
            this.pnlGUI.Visible = true;
            this.pnlOneKeyToken.Visible = false;
        }
        private void BindVPNType()
        {
            var user = DomainApiClient<Dto.User>.GetById(this.AdhocRequest.RequesteeUserId);
            divVPNType.Visible = true;
            ddlVPNType.Items.Clear();
            //Only Citrix
            if (user.EmployeeType == EmployeeType.Contractor || user.EmployeeType == EmployeeType.Vendor)
            {
                ddlVPNType.Items.Add(new ListItem(Enum.GetName(typeof(VPNType), VPNType.VDI), ((int)VPNType.VDI).ToString()));
            }
            else
            {
                ddlVPNType.Items.Add(new ListItem(Enum.GetName(typeof(VPNType), VPNType.VDI), ((int)VPNType.VDI).ToString()));
                ddlVPNType.Items.Add(new ListItem(Enum.GetName(typeof(VPNType), VPNType.SSLVPNOnly), ((int)VPNType.SSLVPNOnly).ToString()));
            }
            if(user.EmployeeType == EmployeeType.Contractor || user.EmployeeType == EmployeeType.Vendor)
            {
                ddlVPNType.SelectedValue = ((int)VPNType.VDI).ToString();
            }
            else
            {
                ddlVPNType.SelectedValue = ((int)VPNType.SSLVPNOnly).ToString();
            }
        }
        protected void ddlVPNType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplicationAutoCompleteBox.Entries.Clear();
            HideEnviornmentType();
        }
        private void DeleteAccountPanel(AdHocVPNRequest request)
        {
            requiredFieldAppln.Visible = false;
            requiredFieldType.Visible = false;
            this.pnlOneKeyToken.Visible = false;
            this.pnlEndPointUserName.Visible = true;
            this.pnlOneKeyToken.Visible = false;
            this.pnlGUI.Visible = false;
            this.ApplicationGrid.Visible = false;
            divVPNType.Visible = false;
        }

        protected void ddlEndpointUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            afterSave();
        }
    }
}