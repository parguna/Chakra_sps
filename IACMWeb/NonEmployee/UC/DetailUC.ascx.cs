using System;
using DI.Chakra.Dto;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Web.Non_Employee.UC
{
    public partial class DetailUC : UIBaseUC
    {
        #region Members
        private const string V_EntityID = "V_EntityID";
        private const string V_ShowAccess = "V_ShowAccess";
        #endregion

        #region Properties
        public long EntityId
        {
            get
            {
                if (ViewState[V_EntityID] == null)
                    return -1;
                return (long)ViewState[V_EntityID];
            }
            set
            {
                ViewState[V_EntityID] = value;
                SetPage();
            }
        }
        public bool ShowAccess
        {
            get
            {
                if (ViewState[V_ShowAccess] == null)
                    return false;
                return (bool)ViewState[V_ShowAccess];
            }
            set
            {
                ViewState[V_ShowAccess] = value;
                SetPage();
            }
        }
        #endregion

        #region Methods
        private void SetPage()
        {
            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", EntityId.ToString());
                User User = DomainApiClient<Dto.User>.GetByAction<User>
                    (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);
                if (User == null)
                    return;

                if (User.FullTimeEmployee)
                {
                    nonEmployeeDiv.Visible = false;
                }
                UserName.Text = User.UserName;
                EmailID.Text = User.EmailId;
                EmployeeID.Text = User.EmployeeId;
                //lblStatus.Text = User.IsActive == true ? "Active" : "Inactive";
                lblDisplayName.Text = User.DisplayName;
                FName.Text = User.FirstName;
                LName.Text = User.LastName;
                lblCompanyOfNonEmployee.Text = User.NCompany;
                DateOnBoard.Text = User.JoinDate.HasValue ? User.JoinDate.Value.ToShortDateString() : "";
                DateOffBoard.Text = User.TerminationDate.HasValue ? User.TerminationDate.Value.ToShortDateString() : "";
                TelNumber.Text = User.TelephoneNumber;
                MbNumber.Text = User.MobileNumber;
                LabelCompany.Text = User.CompanyName;
                lblCostCenter.Text = User.CostCenterText;
                lblRO.Text = GetUsername(User.ROEmployeeId);
                lblCO.Text = GetUsername(User.COEmployeeId);
                //lblDivision.Text = User.DivisionName;
                lblOneKeyId.Text = User.OneKeyTokenId;

                lblOrganisation.Text = User?.NOrganisation?.Name;
                lblBranch.Text = User.OrgBranch;
                lblSection.Text = User.OrgSection;
                lblUnit.Text = User.OrgUnit;

                if (User.NOrganisation != null)
                {
                    lblOrgName.Text = User.NOrganisation.Code + " - " + User.NOrganisation.Name;
                }

                if (User.NBusinessRole != null)
                    lblPosition.Text = User.NBusinessRole.Code + " - " + User.NBusinessRole.Description;

                lblOrganisation.Text = User.NOrganisation?.Name;
                lblCostCenter.Text = User.CostCenter;
               // lblDivision.Text = User.OrgDivision;
                lblBranch.Text = User.OrgBranch;
                lblSection.Text = User.OrgSection;
                lblUnit.Text = User.OrgUnit;
                LabelCompany.Text = User.OrgCompany;
                lblOrgName.Text = User.OrgDepartment;
                lblPosition.Text = User.PositionName;
                UserAccountsListUC.Visible = false;
                Cancel.Visible = false;
                if (ShowAccess)
                {
                    UserAccountsListUC.EntityId = EntityId;
                    UserAccountsListUC.Visible = true;
                    Cancel.Visible = true;
                }
            }
            catch (Exception ex)
            {

            }
        }

        string GetUsername(string employeeId)
        {
            var userDisplayName = string.Empty;
            if (string.IsNullOrWhiteSpace(employeeId))
                return userDisplayName;

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("employeeId", employeeId);
            var user = DomainApiClient<Dto.User>.GetByAction<User>(DomainApiAction.User.GetByEmployeeId, param);
            if (user != null)
                return user.DisplayName;
            return userDisplayName;
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && ContextObjectId != null)
            {
                EntityId = UserAccountsListUC.EntityId = Convert.ToInt64(ContextObjectId);
            }
        }
        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/NonEmployee/List.aspx");
        }
        #endregion
    }
}