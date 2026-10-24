using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class NUserDetailUC : DI.Chakra.Web.UIBaseUC
    {
        const string NullOrEmptyCode = "00000000";
    
        private User _mUser = null;
        public User User
        {
            set
            {
                _mUser = value;
                SetValues(_mUser);
            }
        }

        public bool IsFromTriggerPage
        {
            get
            {
                if (ViewState["IsFromTriggerPage"] == null)
                    return false;
                return (bool)ViewState["IsFromTriggerPage"];
            }
            set
            {
                ViewState["IsFromTriggerPage"] = value;
            }
        }

        private void SetValues(User User)
        {
            if (IsFromTriggerPage)
            {
                EmployeeDetails.Visible = false;
            }
            if (User == null)
                return;
            if (!User.JoinDate.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Warning, "This user does not have join date. So Please select other users.");
                return;
            }
            LabelEmployeeID.Text = User.EmployeeId;
            LabelUserName.Text = User.UserName;
            lblDisplayName.Text = User.DisplayName;

            if (User.NOrganisation != null)
                lblDepartment.Text = User.NOrganisation.Code + " - " + User.NOrganisation.Name;


            if (User.PositionName != null)
                lblPosition.Text = User.PositionName;

            LabelCompany.Text = User.OrgCompany;

            LabelEmployeeType.Text = User.EmployeeType.ToString();

            lblWorkPhNo.Text = User.TelephoneNumber;
            LabelJoiningDate.Text = User.JoinDate.Value.ToString();
            LabelEmailID.Text = User.EmailId;
            lblCostCenter.Text = User.CostCenter;

            if(!string.IsNullOrWhiteSpace(User.OrgDepartment) && !string.IsNullOrWhiteSpace(User.DepartmentCode) && User.DepartmentCode != NullOrEmptyCode)
                lblDepartment.Text = $"{User.OrgDepartment} ({User.DepartmentCode})";
            if (!string.IsNullOrWhiteSpace(User.OrgSection) && !string.IsNullOrWhiteSpace(User.SectionCode) && User.SectionCode != NullOrEmptyCode)
                lblSection.Text = $"{User.OrgSection} ({User.SectionCode})";
            if (!string.IsNullOrWhiteSpace(User.OrgBranch) && !string.IsNullOrWhiteSpace(User.BranchCode) && User.BranchCode != NullOrEmptyCode)
                lblBranch.Text = $"{User.OrgBranch} ({User.BranchCode})";
            
            lblCostCenter.Text = User.CostCenter;

            if (!string.IsNullOrWhiteSpace(User.OrgUnit) && !string.IsNullOrWhiteSpace(User.UnitCode) && User.UnitCode != NullOrEmptyCode)
                lblUnit.Text = $"{User.OrgUnit} ({User.UnitCode})";
            //lblPACompany.Text = User.CompanyCode + " - " + User.CompanyName;
            //lblPersonnelArea.Text = User.DepartmentCode + " - " + User.DepartmentName;
            //lblPersonnelSubArea.Text = User.DivisionCode + " - " + User.DivisionName;
            //lblOU.Text = User.SectionCode + " - " + User.SectionName;

            var ro = GetUser(User.ROEmployeeId);
            if (ro != null)
            {
                LabelManagerName.Text = ro.DisplayName;
                lblManagerPhNo.Text = ro.TelephoneNumber;
            }

            if (!IsFromTriggerPage)
            {
                DIVEMPTYPE.Visible = false;
                DIVCMPNY.Visible = false;
            }
        }

        Dto.User GetUser(string employeeId)
        {
            Dto.User user = null;
            if (string.IsNullOrWhiteSpace(employeeId))
                return user;

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("employeeId", employeeId);
            user = DomainApiClient<Dto.User>.GetByAction<User>(DomainApiAction.User.GetByEmployeeId, param);
            return user;
        }
    }
}