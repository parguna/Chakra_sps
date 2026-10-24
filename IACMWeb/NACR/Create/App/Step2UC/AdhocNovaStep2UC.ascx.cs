using Application;
using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Web.NACR.Create.App.Step2UC
{
    public partial class AdhocNovaStep2UC : UIBaseStep2UC
    {
        #region properties
        private List<Dto.AdHocNovaRequestDetails> NovaRequestDetails
        {
            get
            {
                if (ViewState["NovaRoles"] != null)
                {
                    return (List<Dto.AdHocNovaRequestDetails>)ViewState["NovaRoles"];
                }
                return new List<Dto.AdHocNovaRequestDetails>();
            }
            set
            {
                ViewState["NovaRoles"] = value;
            }
        }
        public string UserId
        {
            get
            {
                return AdhocRequest.RequesteeUserId.ToString();
            }
        }
        public string MatrixType
        {
            get
            {
                if (ViewState["MatrixType"] == null)
                    return "0";
                return ViewState["MatrixType"].ToString();
            }
            set
            {
                ViewState["MatrixType"] = value;
            }
        }
        public string EndpointUserId
        {
            get
            {
                if (ViewState["EndpointUserId"] == null)
                    return "0";
                return ViewState["EndpointUserId"].ToString();
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
                return this.AdhocRequest.EndpointSystemId.ToString();
            }
        }
        public string AdhocActionType
        {
            get
            {
                return ((int)this.AdhocRequest.AdhocActionType).ToString();
            }
            set
            {
                ViewState["AdhocActionType"] = value;
            }
        }
        public string DepartmentCode
        {
            get
            {
                if (ViewState["DepartmentCode"] != null)
                    return ViewState["DepartmentCode"].ToString();
                return null;
            }
            set
            {
                ViewState["DepartmentCode"] = value;
            }
        }
        #endregion
        protected override void afterSave()
        {
            base.afterSave();
            ClearFields();
            PositionAutoCompleteBox.Entries.Clear();
            ddlMatrixType.SelectedIndex = 0;
            NovaRequestDetails = new List<AdHocNovaRequestDetails>();
            FillApplicationGrid();

        }

        const string V_SAPSECTION = "V_SAPSECTION";
        public string SapSection
        {
            get
            {
                if (ViewState[V_SAPSECTION] != null)
                {
                    return ((string)ViewState[V_SAPSECTION]).Trim();
                }
                return string.Empty;
            }
            set
            { 
                ViewState[V_SAPSECTION] = value;
            }
        }

        private void ClearFields()
        {
            txtAcntName.Text = "";
            txtDiscription.Text = "";
            if (AdhocRequest.AccountType == AccountType.Secondary)
            {
                User usr = DomainApiClient<User>.GetById(this.AdhocRequest.RequesteeUserId);
                if (usr != null)
                {
                    txtDiscription.Text = usr.KnownAs;
                    txtDiscription.Enabled = false;
                }
            }
            else
            {
                txtDiscription.Enabled = true;
            }
        }
        protected override void InitializeStep()
        {
            if (AdhocRequest == null) return;
            SpecialAccountWarning.ShowWarningMessage(AdhocRequest);
            SharedPanelFLINS.Visible = false;

            ResetCategory();
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                LoadCategory();
            }
            BindEndpointUsers(AdhocRequest.RequesteeUserId, AdhocRequest.EndpointSystemId.Value, AdhocRequest.AccountType);
            this.EndpointUserId = ddlEndpointUser.SelectedValue;
            this.ddlEndpointUser.Enabled = true;
            var Entitlement = (AdHocNovaRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove
                || this.AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount
                )
                {
                    if (ddlEndpointUser.Items.FindByValue(Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId)) != null)
                        ddlEndpointUser.SelectedValue = Entitlement.EndpointUserId == 0 ? "-- Select EndPoint User --" : Convert.ToString(Entitlement.EndpointUserId);
                }
                if (Request.QueryString["AdhocRequestId"] != null && Convert.ToInt32(Request.QueryString["AdhocRequestId"]) > 0)
                {
                    if (Entitlement.RequestDetails != null)
                        this.NovaRequestDetails = Entitlement.RequestDetails;
                }
            }
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                divEndpointUser.Visible = true;
            }
            else
            {
                divEndpointUser.Visible = false;
            }
            FillApplicationGrid();
            trSecurityQstns.Visible = false;
            if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && AdhocRequest.AccountType != AccountType.User)
            {
                var accountType = AdhocRequest.AccountType;
                BindSAPUserType(accountType);
                if (accountType == AccountType.Service)
                {
                    trSecurityQstns.Visible = true;
                }
            }
            divSelectMatType.Visible = true;
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
            {
                divEndpointUser.Visible = true;
                divSelectMatType.Visible = false;
            }


        }

        List<AccountReconcilation> getAccountReconcilations(AccountType accountType, ApplicationType applicationType)
        {
            return DomainApiClient<Dto.AccountReconcilation>.GetByAction<List<Dto.AccountReconcilation>>(DomainApiAction.Provisioning.AccountReconcilation.GetByAccountTypeAndApplicationType,
                 new Dictionary<string, string> { { "userId", this.UserId.ToString() },
                                                   { "accountType", Convert.ToString((int)accountType) },
                                                   { "applicationType",Convert.ToString((int)applicationType) }
                                               });
        }



        void BindSAPUserType(AccountType accountType)
        {

            //var list = getAccountReconcilations(accountType, ApplicationType.HRMS_FLINS);
            //if (list.Any())
            //{
            //    divExistingFlinsAccount.Visible = true;
            //    SharedPanelFLINS.Visible = false;
            //    ddlFlinsAccount.Items.Clear();
            //    DropDownHelper.DropDownListBind(ddlFlinsAccount, list.ToDictionary(o => o.EndpointUserId, o => o.AccountName), "Value", "Key", null, true, "-- Select FLINS Account --", false);
            //}
            //else
            //{
            divExistingFlinsAccount.Visible = false;
            SharedPanelFLINS.Visible = true;
            txtAcntName.Text = "";
            txtDiscription.Text = "";
            txtDiscription.Enabled = true;
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
                    }
                }
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
            catch
            {
                throw;
            }
            //}

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCategory();
                FillApplicationGrid();
            }
        }
        protected void PositionAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            string group = string.Empty;

            var NewItem = PositionAutoCompleteBox.Entries[PositionAutoCompleteBox.Entries.Count - 1];
            PositionAutoCompleteBox.Entries.Clear();
            PositionAutoCompleteBox.Entries.Add(NewItem);
        }

        protected void PositionAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            if (PositionAutoCompleteBox.Entries.Count == 0)
            {

            }
        }
        protected void ddlMatrixType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetCategory();
            var selectedCategory = Convert.ToInt32(ddlMatrixType.SelectedValue);
            if (ddlMatrixType.SelectedIndex > 0 && selectedCategory > 0)
            {

                MatrixType = ddlMatrixType.SelectedValue;
                var user = DomainApiClient<Dto.SAPEamNovaUserInfo>.GetByAction<User>(DomainApiAction.Matrix.NOVA.GetUser, new Dictionary<string, string> { { "id", this.AdhocRequest.RequesteeUserId.ToString() } });
                var matrixType = ((NovaMatrixType)Enum.Parse(typeof(NovaMatrixType), (selectedCategory).ToString()));
                switch (matrixType)
                {
                    case NovaMatrixType.User:
                        if (this.NovaRequestDetails.Any(i => i.NovaMatrixType == NovaMatrixType.Management))
                        {
                            ShowAlertMessage(AlertType.Warning, "Please select either User or Management Matrixtype.");
                            ddlMatrixType.SelectedIndex = 0;
                            return;
                        }
                        
                        LoadSAPSections();
                        this.divUserCategory.Visible = true;
                        this.lblCompanyName.Text = $"{user?.CompanyName}({user?.CompanyCode})";
                        this.lblSectionName.Text = $"{getOrganisationCodeName(user)}({user?.OrganisationCode})"; 
                        this.btnAddRequest.Visible = true;
                        break;
                    case NovaMatrixType.Management:
                        if (this.NovaRequestDetails.Any(i => i.NovaMatrixType == NovaMatrixType.User))
                        {
                            ShowAlertMessage(AlertType.Warning, "Please select either User or Management Matrixtype.");
                            ddlMatrixType.SelectedIndex = 0;
                            return;
                        }
                        divRoleDescription.Visible = true;
                        this.btnAddRequest.Visible = true;
                        LoadDepartment();
                        this.divManagementDepartment.Visible = true;
                        break;
                    case NovaMatrixType.Exceptional:
                        divExceptionalRole.Visible = true;
                        LoadExceptionalCompanyCode();
                        this.btnAddRequest.Visible = true;
                        break;
                    default:
                        break;
                }
            }
        }

        string getOrganisationCodeName(User user)
        {
            if (user.UnitCode == user.OrganisationCode) return user.OrgUnit;
            else if (user.SectionCode == user.OrganisationCode) return user.OrgSection;
            else if (user.BranchCode == user.OrganisationCode) return user.OrgBranch;
            else if (user.DepartmentCode == user.OrganisationCode) return user.OrgDepartment;
            else return user.OrgCompany;
        }
        private void LoadDepartment()
        {
            var param = new Dictionary<string, string>();
            param.Add("endpointSystemId", this.EndpointSystemId);
            ddlDepartment.Items.Clear();
            ddlDepartment.Items.Add(new ListItem("---Select---", "0"));
            var departments = DomainApiClient<Dto.NOVAManagementRoleMatrix>.GetByAction<List<string>>(DomainApiAction.Matrix.NOVA.GetManagementDepartment, param);
            if (departments != null && departments.Count > 0)
            {
                foreach (var department in departments)
                {
                    ddlDepartment.Items.Add(new ListItem(department, department));
                }
            }
        }

        private void LoadSAPSections()
        {
            ddlSAPSection.Enabled = true;

            if (this.NovaRequestDetails != null && this.NovaRequestDetails.Any(a => a.NovaMatrixType == NovaMatrixType.User))
            {

                var detail = this.NovaRequestDetails.FirstOrDefault(a => a.NovaMatrixType == NovaMatrixType.User);
                if (detail != null)
                {  
                    ddlSAPSection.Items.Clear();
                    ddlSAPSection.Items.Add(new ListItem("---Select---", "0"));
                    ddlSAPSection.Items.Add(new ListItem(detail.SectionCode, detail.SectionCode)); 
                    ddlSAPSection.SelectedValue = detail.SectionCode;
                    ddlSAPSection_SelectedIndexChanged(null, null);
                    ddlSAPSection.Enabled = false;
                    return;
                }
                
            }





            var param = new Dictionary<string, string>();
            param.Add("DIUserId", this.AdhocRequest.RequesteeUserId.ToString());
            param.Add("adhocActionType", ((int)this.AdhocRequest.AdhocActionType).ToString());
            param.Add("endpointId", this.EndpointSystemId);
            ddlSAPSection.Items.Clear();
            ddlSAPSection.Items.Add(new ListItem("---Select---", "0"));
            var departments = DomainApiClient<Dto.NOVAUserRoleMatrix>.GetByAction<List<string>>(DomainApiAction.Matrix.NOVA.GetSAPSection, param);
            if (departments != null && departments.Count > 0)
            {
                foreach (var department in departments)
                {
                    ddlSAPSection.Items.Add(new ListItem(department, department));
                }

                if (departments.Count == 1)
                {                    
                    ddlSAPSection.SelectedValue = departments.FirstOrDefault(); 
                    ddlSAPSection_SelectedIndexChanged(null, null);
                    ddlSAPSection.Enabled = false;
                }
            }

            

        }

        protected void NovaRoleGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var requests = NovaRequestDetails;
                var identity = Convert.ToInt64(((GridDataItem)e.Item).GetDataKeyValue("Id"));
                requests.Remove(requests.FirstOrDefault(c => c.Id == identity));
                this.NovaRequestDetails = requests;
                FillApplicationGrid();
                LoadSAPSections();
            }
        }
        #region private method
        private void FillApplicationGrid()
        {
            if (this.NovaRequestDetails == null)
            {
                NovaRequestDetails = new List<Dto.AdHocNovaRequestDetails>();
            }
            var visible = (NovaRequestDetails.Count > 0);
            NovaRoleGrid.DataSource = this.NovaRequestDetails.AsEnumerable().Select(c => c);
            NovaRoleGrid.DataBind();
            NovaRoleGrid.Visible = visible;
            NovaRoleGrid.Visible = visible;
        }
        private void LoadCategory()
        {
            ddlMatrixType.Items.Clear();
            ddlMatrixType.Items.Add(new ListItem("---Select---", "0"));
            if (AdhocRequest != null && AdhocRequest.AccountType == AccountType.User)
            {
                ddlMatrixType.Items.Add(new ListItem(NovaMatrixType.User.ToString(), ((byte)NovaMatrixType.User).ToString()));
                ddlMatrixType.Items.Add(new ListItem(NovaMatrixType.Management.ToString(), ((byte)NovaMatrixType.Management).ToString()));
            }
            ddlMatrixType.Items.Add(new ListItem(NovaMatrixType.Exceptional.ToString(), ((byte)NovaMatrixType.Exceptional).ToString()));


        }
        private void LoadExceptionalCompanyCode()
        {
            try
            {
                ddlCompanyCode.Items.Clear();
                ExceptionalRoleComboBox.Items.Clear();
                ddlCompanyCode.Items.Add(new ListItem("---Select---", "0"));
                var param = new Dictionary<string, string>();
                param.Add("endpointSystemId", this.EndpointSystemId);
                param.Add("AccountType", Convert.ToString((int)this.AdhocRequest.AccountType));
                var companyCodes = DomainApiClient<Dto.NOVAExceptionalRoleMatrix>.GetByAction<Dictionary<string, string>>(DomainApiAction.Matrix.NOVA.GetExceptionalMatrixCompanyCode, param);
                if (companyCodes != null && companyCodes.Count > 0)
                {
                    foreach (var companyCode in companyCodes)
                    {
                        ddlCompanyCode.Items.Add(new ListItem(companyCode.Key, companyCode.Value));
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void ResetCategory()
        {
            //this.divEndpointUser.Visible = false;
            this.divExceptionalRole.Visible = false;
            this.divUserCategory.Visible = false;
            this.divExceptionalRole.Visible = false;
            this.btnAddRequest.Visible = false;
            this.divManagementDepartment.Visible = false;
            divRoleDescription.Visible = false;
            PositionAutoCompleteBox.Entries.Clear();
        }
        #endregion

        protected void ddlCompanyCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExceptionalRoleComboBox.Items.Clear();
            var param = new Dictionary<string, string>();
            param.Add("endpointSystemId", this.EndpointSystemId);
            param.Add("companyCode", ddlCompanyCode.SelectedValue);
            param.Add("adhocActionType", ((int)this.AdhocRequest.AdhocActionType).ToString());
            param.Add("endpointUserId", this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount ? ddlEndpointUser.SelectedValue : "0");
            param.Add("userId", this.AdhocRequest.RequesteeUserId.ToString());
            param.Add("AccountType", Convert.ToString((int)this.AdhocRequest.AccountType));
            var companyCodes = DomainApiClient<Dto.NOVAExceptionalRoleMatrix>.GetByAction<List<Dto.NOVAExceptionalRoleMatrix>>(DomainApiAction.Matrix.NOVA.GetByCompanyCode, param);
            if (companyCodes != null && companyCodes.Count > 0)
            {
                foreach (var companyCode in companyCodes)
                {
                    ExceptionalRoleComboBox.Items.Add(new RadComboBoxItem(companyCode.JobDescription, companyCode.FLINSRoles));
                }
            }
        }
        protected void btnAddRequest_Click(object sender, EventArgs e)
        {
            if (ddlMatrixType.SelectedIndex == 0)
            {
                ShowAlertMessage(AlertType.Warning, "Please select Matrix Type");
                return;
            }

            if (ddlMatrixType.SelectedIndex > 0 && ((NovaMatrixType)Enum.Parse(typeof(NovaMatrixType), ddlMatrixType.SelectedValue)) == NovaMatrixType.User && string.IsNullOrWhiteSpace(this.SapSection))
            {
                ShowAlertMessage(AlertType.Warning, "Please select the section");
                return;
            }

                NovaMatrixType matrixType = (NovaMatrixType)Enum.Parse(typeof(NovaMatrixType), ddlMatrixType.SelectedValue);
            switch (matrixType)
            {
                case NovaMatrixType.User:
                    if(!string.IsNullOrWhiteSpace(SapSection))
                    UserMatrix();
                    break;
                case NovaMatrixType.Management:
                    ManagementMatrix();
                    break;
                case NovaMatrixType.Exceptional:
                    ExceptionalMatrix();
                    break;
                default:
                    break;
            }
            PositionAutoCompleteBox.Entries.Clear();
        }
        private void UserMatrix()
        {
            if (PositionAutoCompleteBox.Entries.Count == 0)
            {
                ShowAlertMessage(AlertType.Warning, "Please select Role");
                return;
            }
            var user = DomainApiClient<Dto.User>.GetById(this.AdhocRequest.RequesteeUserId);
            var requests = this.NovaRequestDetails;
            foreach (AutoCompleteBoxEntry role in PositionAutoCompleteBox.Entries)
            {
                var id = Convert.ToInt64(role.Value);
                var matrix = DomainApiClient<Dto.NOVAUserRoleMatrix>.GetById(id);
                if (this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.NewAccount)
                {
                    ConstructNewAccountRequestDetails(user, matrix);
                }
                else if (this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.AmendmentAdd)
                {
                    ConstructAmendmentAddRequestDetails(user, matrix);
                }
                else if (this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.AmendmentRemove)
                {
                    if (!this.NovaRequestDetails.Exists(i => i.NovaMatrixType == NovaMatrixType.User && i.RoleName == matrix.Position))
                    {
                        requests.Add(new Dto.AdHocNovaRequestDetails()
                        {
                            NovaMatrixType = NovaMatrixType.User,
                            RoleName = matrix.Position,
                            CompanyCode = matrix.CompanyCode,
                            SectionCode = matrix.SectionCode,
                            CompanyName = user.CompanyName,
                            SectionName = user.SectionName,
                            JobDescription = matrix.JobDescription
                        });
                        this.NovaRequestDetails = requests;
                    }
                }
            }
            PositionAutoCompleteBox.Entries.Clear();
            FillApplicationGrid();
            LoadSAPSections();
        }

        private void ConstructAmendmentAddRequestDetails(Dto.User user, Dto.NOVAUserRoleMatrix matrix)
        { 
            var requests = this.NovaRequestDetails;
            var dataAlreadyExists = this.NovaRequestDetails.Exists(i => i.NovaMatrixType == NovaMatrixType.User);
            var selectedRole = new List<string>();
            var allowedRoles = new List<string>();
            var param = new Dictionary<string, string>();
            param.Add("endpointId", this.EndpointSystemId);
            param.Add("DIUserId", this.UserId);
            param.Add("adhocActionType", AdHocActionType.AmendmentAdd.ToString());
            param.Add("SAPSection", SapSection);
            var exisitingRoles = DomainApiClient<Dto.NOVAUserRoleMatrix>.GetByAction<List<Dto.NOVAUserRoleMatrix>>(DomainApiAction.Matrix.NOVA.GetExisitingRequestAndRole, param);
            if (exisitingRoles.Count > 0)
            {
                selectedRole.AddRange(exisitingRoles.Select(i => i.Position).Distinct());
            }
            if (dataAlreadyExists)
            {
                selectedRole.AddRange(this.NovaRequestDetails.Select(i => i.RoleName));
            }
            Dto.NOVASoDExceptionsMatrix anyOne = null;
            var sodMatrices = DomainApiClient<Dto.NOVASoDExceptionsMatrix>.GetByAction<List<Dto.NOVASoDExceptionsMatrix>>(DomainApiAction.Matrix.NOVA.GetSODMatricesBySectionCode, param);
            anyOne = sodMatrices.FirstOrDefault(i =>
            {
                var exce = i.Exceptions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (selectedRole.Count == 0)
                {
                    return exce.Exists(j => j.Equals(matrix.Position, StringComparison.InvariantCultureIgnoreCase));
                }
                else
                {
                    var temp = exce.Exists(j => j.Equals(matrix.Position, StringComparison.InvariantCultureIgnoreCase));
                    if (temp)
                    {
                        return !selectedRole.Except(exce).Any();
                    }
                    return false;
                }
            });


            {
                if (anyOne != null || selectedRole.Count==0)
                {
                    if (!this.NovaRequestDetails.Exists(i => i.NovaMatrixType == NovaMatrixType.User && i.RoleName == matrix.Position))
                    {
                        requests.Add(new Dto.AdHocNovaRequestDetails()
                        {
                            NovaMatrixType = NovaMatrixType.User,
                            RoleName = matrix.Position,
                            CompanyCode = matrix.CompanyCode,
                            SectionCode = matrix.SectionCode,
                            JobDescription = matrix.JobDescription,
                            CompanyName = user.CompanyName,
                            SectionName = user.SectionName
                        });
                        this.NovaRequestDetails = requests;
                    }

                }
                else
                {
                    ShowAlertMessage(AlertType.Warning, $"SOD conflicted. We cannot add this Role:{matrix.Position}");
                }
            }
        }

        private void ConstructNewAccountRequestDetails(Dto.User user, Dto.NOVAUserRoleMatrix matrix)
        {
            var requests = this.NovaRequestDetails;
            var dataAlreadyExists = this.NovaRequestDetails.Exists(i => i.NovaMatrixType == NovaMatrixType.User);
            var selectedRole = new List<string>();
            var allowedRoles = new List<string>();

            if (dataAlreadyExists)
            {
                selectedRole.AddRange(this.NovaRequestDetails.Select(i => i.RoleName));
            }

            var param = new Dictionary<string, string>();
            param.Add("endpointId", this.EndpointSystemId);
            param.Add("DIUserId", this.UserId);
            param.Add("sapSection", this.SapSection);
            Dto.NOVASoDExceptionsMatrix anyOne = null;
            var sodMatrices = DomainApiClient<Dto.NOVASoDExceptionsMatrix>.GetByAction<List<Dto.NOVASoDExceptionsMatrix>>(DomainApiAction.Matrix.NOVA.GetSODMatricesBySectionCode, param);
            anyOne = sodMatrices.FirstOrDefault(i =>
            {
                var exce = i.Exceptions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (selectedRole.Count == 0)
                {
                    return exce.Exists(j => j.Equals(matrix.Position, StringComparison.InvariantCultureIgnoreCase));
                }
                else
                {
                    var temp = exce.Exists(j => j.Equals(matrix.Position, StringComparison.InvariantCultureIgnoreCase));
                    if (temp)
                    {
                        return !selectedRole.Except(exce).Any();
                    }
                    return false;
                }
            });

            if (!dataAlreadyExists)//User Matrix data not exists
            {
                if (!this.NovaRequestDetails.Exists(i => i.NovaMatrixType == NovaMatrixType.User && i.RoleName == matrix.Position))
                {
                    requests.Add(new Dto.AdHocNovaRequestDetails()
                    {
                        NovaMatrixType = NovaMatrixType.User,
                        RoleName = matrix.Position,
                        CompanyCode = matrix.CompanyCode,
                        SectionCode = matrix.SectionCode,
                        CompanyName = user.CompanyName,
                        SectionName = user.SectionName,
                        JobDescription = matrix.JobDescription
                    });
                    this.NovaRequestDetails = requests;

                   

                }
                else
                {
                    ShowAlertMessage(AlertType.Warning, $"SOD conflicted. User cannot have more than one Role");
                }
            }
            else
            {
                if (anyOne != null)
                {
                    if (!this.NovaRequestDetails.Exists(i => i.NovaMatrixType == NovaMatrixType.User && i.RoleName == matrix.Position))
                    {
                        requests.Add(new Dto.AdHocNovaRequestDetails()
                        {
                            NovaMatrixType = NovaMatrixType.User,
                            RoleName = matrix.Position,
                            CompanyCode = matrix.CompanyCode,
                            SectionCode = matrix.SectionCode,
                            CompanyName = user.CompanyName,
                            SectionName = user.SectionName,
                            JobDescription = matrix.JobDescription
                        });
                        this.NovaRequestDetails = requests;
                    }

                }
                else
                {
                    ShowAlertMessage(AlertType.Warning, $"SOD conflicted. User cannot have more than one Role");
                }
            }

            var detail = this.NovaRequestDetails.FirstOrDefault(a => a.NovaMatrixType == NovaMatrixType.User);
            if (detail != null)
            {
                SapSection = detail.SectionCode;

                ddlSAPSection.Items.Clear();
                ddlSAPSection.Items.Add(new ListItem("---Select---", "0")); 
                ddlSAPSection.Items.Add(new ListItem(detail.SectionCode, detail.SectionCode)); 
                ddlSAPSection.SelectedValue = detail.SectionCode;
                ddlSAPSection_SelectedIndexChanged(null, null);
                ddlSAPSection.Enabled = false;
            }
        }

        private void ManagementMatrix()
        {
            if (ddlDepartment.SelectedIndex > 0)
            {
                if (PositionAutoCompleteBox.Entries.Count == 0)
                {
                    ShowAlertMessage(AlertType.Warning, "Please select Role");
                    return;
                }
                var user = DomainApiClient<Dto.User>.GetById(this.AdhocRequest.RequesteeUserId);
                var requests = this.NovaRequestDetails;
                foreach (AutoCompleteBoxEntry role in PositionAutoCompleteBox.Entries)
                {
                    var id = Convert.ToInt64(role.Value);
                    var matrix = DomainApiClient<Dto.NOVAManagementRoleMatrix>.GetById(id);
                    if (!this.NovaRequestDetails.Exists(i => i.NovaMatrixType == NovaMatrixType.Management && i.RoleName == matrix.Position && i.DepartmentCode == ddlDepartment.SelectedItem.Text))
                    {
                        requests.Add(new Dto.AdHocNovaRequestDetails()
                        {
                            NovaMatrixType = NovaMatrixType.Management,
                            RoleName = matrix.Position,
                            DepartmentCode = DepartmentCode,
                            JobDescription = matrix.JobDescription
                        });
                    }
                }
                this.NovaRequestDetails = requests;
                FillApplicationGrid();
            }
            else
            {
                ShowAlertMessage(AlertType.Warning, "Please select Department");
            }
        }
        private void ExceptionalMatrix()
        {
            if (ddlCompanyCode.SelectedIndex == 0)
            {
                ShowAlertMessage(AlertType.Warning, "Please select CompanyCode");
                return;
            }
            if (ExceptionalRoleComboBox.CheckedItems.Count == 0)
            {
                ShowAlertMessage(AlertType.Warning, "Please select Exceptional Role");
                return;
            }
            var requests = this.NovaRequestDetails;
            foreach (RadComboBoxItem role in ExceptionalRoleComboBox.CheckedItems)
            {
                if (!this.NovaRequestDetails.Exists(i => i.NovaMatrixType == NovaMatrixType.Exceptional && i.RoleName == role.Value && i.CompanyCode == ddlCompanyCode.SelectedValue))
                {
                    requests.Add(new Dto.AdHocNovaRequestDetails()
                    {
                        NovaMatrixType = NovaMatrixType.Exceptional,
                        RoleName = role.Value,
                        CompanyCode = ddlCompanyCode.SelectedValue,
                        JobDescription = role.Text

                    });
                }
            }
            foreach (RadComboBoxItem item in ExceptionalRoleComboBox.Items)
            {
                if (item.Checked)
                    item.Checked = false;
            }
            this.NovaRequestDetails = requests;
            FillApplicationGrid();
        }
        protected void ExceptionalRoleComboBox_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBox comboBox = (RadComboBox)sender;
            int checkedCount = comboBox.CheckedItems.Count();
            this.btnAddRequest.Visible = checkedCount > 0;
        }

        protected void ddlSAPSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            SapSection = string.Empty;

            if(ddlSAPSection.SelectedIndex>0)
              SapSection = ddlSAPSection.SelectedValue ; 

            if(!string.IsNullOrWhiteSpace(SapSection))
            {
                divRoleDescription.Visible = true;
            }
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlDepartment.SelectedIndex > 0)
            {
                this.DepartmentCode = ddlDepartment.SelectedValue;
            }
            else
            {
                this.DepartmentCode = string.Empty;
            }
        }
        public override bool Validate()
        {
            if (this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.AmendmentAdd ||
                this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.AmendmentRemove ||
                this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.DeleteAccount
                )
            {
                if (ddlEndpointUser.SelectedIndex == 0)
                {
                    this.ShowAlertMessage(AlertType.Warning, "Please select Endpoint User");
                    return false;
                }
            }
            if (this.AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
            {
                return true;
            }

            if (NovaRequestDetails.Count == 0)
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, Constants.addToGridWarning);
                return false;
            }

            if (AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.NewAccount && AdhocRequest.AccountType != AccountType.User)
            {
                if (divExistingFlinsAccount.Visible)
                {
                    if (ddlFlinsAccount.SelectedIndex <= 0)
                    {
                        ShowAlertMessage(AlertType.Error, "Please Select FLINS Account.");
                        return false;
                    }

                }
                else if (SharedPanelFLINS.Visible)
                {
                    if (SAPUserTypeDropDown.SelectedIndex <= 0 && string.IsNullOrWhiteSpace(txtAcntName.Text))
                    {
                        ShowAlertMessage(AlertType.Error, "Please Select SAP UserType and Account Name.");
                        return false;
                    }
                    else if (SAPUserTypeDropDown.SelectedIndex <= 0)
                    {
                        ShowAlertMessage(AlertType.Error, "Please Select SAP UserType.");
                        return false;
                    }
                    else if (string.IsNullOrWhiteSpace(txtAcntName.Text))
                    {
                        ShowAlertMessage(AlertType.Error, "Please Select Account Name.");
                        return false;
                    }

                    if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount && (AdhocRequest.AccountType == AccountType.Service || AdhocRequest.AccountType == AccountType.Shared || AdhocRequest.AccountType == AccountType.Secondary))
                    {
                        string msgValidation = string.Empty;
                        var flag = AdhocAccessRequestValidation.ValidateAccountName(txtAcntName.Text, AdhocRequest.EndpointSystemId.Value, out msgValidation, AdhocRequest.RequesteeUserId, this.AdhocRequest.AccountType);

                        if (!flag && !string.IsNullOrEmpty(msgValidation))
                        {
                            ShowAlertMessage(AlertType.Error, msgValidation);
                            return false;
                        }
                    }
                }

                if (AdhocRequest.AccountType == AccountType.Service && string.IsNullOrWhiteSpace(AdhocRequest.SecurityQuestions))
                {
                    ShowAlertMessage(AlertType.Error, "Please select service account compliance check.");
                    return false;
                }
            }

            return true;
        }
        protected override IEntitlementDetail GetEntitlementDetail()
        {
            var request = new AdHocNovaRequest();
            if (this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.NewAccount ||
                this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.AmendmentAdd ||
                this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.AmendmentRemove
                )
            {
                if (this.ddlEndpointUser.SelectedIndex > 0)
                    request.EndpointUserId = Convert.ToInt64(this.ddlEndpointUser.SelectedValue);
                var requestDetails = this.NovaRequestDetails;

                if (divExistingFlinsAccount.Visible && ddlFlinsAccount.SelectedIndex > 0)
                    request.FlinsAccountEndpointUserId = Convert.ToInt64(ddlFlinsAccount.SelectedValue);

                if (this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.NewAccount && this.AdhocRequest.AccountType != AccountType.User)
                {
                    //if (SharedPanelFLINS.Visible)
                    //{
                    var sharedDetails = new AdHocSAPSharedAccount();
                    if (SAPUserTypeDropDown.SelectedIndex > 0)
                    {
                        sharedDetails.SAPUserType = (SAPUserType)(Convert.ToByte(SAPUserTypeDropDown.SelectedValue));
                    }
                    sharedDetails.AccountName = txtAcntName.Text;
                    sharedDetails.Description = txtDiscription.Text;
                    request.AdHocSAPSharedAccountDetail = sharedDetails;
                    // }
                }

                if ((this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.AmendmentRemove ||
                this.AdhocRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.AmendmentAdd))
                {
                    if(this.ddlEndpointUser.SelectedItem != null)
                        request.DisplayAccountName = this.ddlEndpointUser.SelectedItem.Text;
                }
                

                request.RequestDetails = requestDetails;
            }
            else if (this.AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
            {
                if (divExistingFlinsAccount.Visible && ddlFlinsAccount.SelectedIndex > 0)
                    request.FlinsAccountEndpointUserId = Convert.ToInt64(ddlFlinsAccount.SelectedValue);

                if (this.ddlEndpointUser.SelectedIndex > 0)
                    request.EndpointUserId = Convert.ToInt64(this.ddlEndpointUser.SelectedValue);


                if (this.ddlEndpointUser.SelectedItem != null)
                    request.DisplayAccountName = this.ddlEndpointUser.SelectedItem.Text;


            }

            return request;
        }
        Dto.AccountReconDto BindEndpointUsers(long userId, long endpointSystemId, AccountType accountType)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("userid", userId.ToString());
            dict.Add("endpointsystemid", endpointSystemId.ToString());
            dict.Add("accountType", Convert.ToString((int)accountType));
            Dto.AccountReconDto list = DomainApiClient<Dto.ADSUserInfo>.GetByAction<Dto.AccountReconDto>("GetADUsersByDIUserId", dict);
            Session["EndpointUserId"] = null;
            if (list != null && list.AccountReconDictionary != null)
            {
                ddlEndpointUser.Items.Clear();
                DropDownHelper.DropDownListBind(ddlEndpointUser, list.AccountReconDictionary, "Value", "Key", null, true, "-- Select EndPoint User --", false);
                if (ddlEndpointUser.SelectedItem != null && ddlEndpointUser.SelectedIndex > 0 && AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    Session["EndpointUserId"] = Convert.ToInt32(ddlEndpointUser.SelectedValue);
                }
            }
            return list;
        }

        protected void ddlEndpointUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            afterSave();
            ddlMatrixType_SelectedIndexChanged(null, null);
        }
    }
}