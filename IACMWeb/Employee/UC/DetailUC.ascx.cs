using System;
using DI.Chakra.Dto;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Web.Employee.UC
{
    public partial class DetailUC : UIBaseUC
    {
        #region Members
        private const string V_EntityID = "V_EntityID";
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
        #endregion

        #region Methods
        private void SetPage()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", EntityId.ToString());
            User User = DomainApiClient<Dto.User>.GetByAction<User>
                (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);

            if (User == null)
                return;
            lblUserName.Text = User.UserName;
            lblDisplayName.Text = User.DisplayName;
            lblEmployeeId.Text = User.EmployeeId;
            lblEmailId.Text = User.EmailId;
            lblTelephone.Text = User.TelephoneNumber;
            lblPosition.Text = User.PositionName;
            lblSupervisor.Text = GetSupervisor();
            lblCountersigningOfficer.Text = GetCO();
            lblJoinDate.Text = User.JoinDate?.ToShortDateString();
            lblTerminationDate.Text = User.TerminationDate?.ToShortDateString();
            lblOneKeyId.Text = User.OneKeyTokenId;
            lblFirstName.Text = User.FirstName;
            lblLastName.Text = User.LastName;
            lblOrganisation.Text = User.NOrganisation?.Name;
            lblCostCenter.Text = User.CostCenter;
            lblCompany.Text = User.OrgCompany;
            lblDepartment.Text = User.OrgDepartment;
            //lblDivision.Text = User.OrgDivision;
            lblBranch.Text = User.OrgBranch;
            lblSection.Text = User.OrgSection;
            lblUnit.Text = User.OrgUnit;
        }

        private string GetCO()
        {
            var param = new Dictionary<string, string>();
            param.Add("id", EntityId.ToString());
            var co = DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.GetCOByUser, param);
            return co?.DisplayName;
        }

        private string GetSupervisor()
        {
            var param = new Dictionary<string, string>();
            param.Add("id", EntityId.ToString());
            var supervisor = DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.GetROByUser, param);
            return supervisor?.DisplayName;
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && ContextObjectId != null)
            {
                EntityId = Convert.ToInt64(ContextObjectId);
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Employee/List.aspx");
        }

        private string IdentificationNumberConsrtuct(string identificationNumber)
        {
            var length = identificationNumber.Length;
            var result = new String('X', length - 5) + identificationNumber.Substring(length - 5);
            return result;
        }
        //protected void Refresh_Click(object sender, EventArgs e)
        //{
        //    this.SetPage();
        //}
        #endregion
    }
}