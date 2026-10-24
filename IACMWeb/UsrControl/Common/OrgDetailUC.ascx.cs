using DI.Chakra.Dto;
using System.Collections.Generic;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class OrgDetailUC : System.Web.UI.UserControl
    {

        private const string V_USERORGINFO = "V_USERORGINFO";

        public User UserDetail
        {
            get
            {
                if (ViewState[V_USERORGINFO] != null)
                {
                    return (User)ViewState[V_USERORGINFO];
                }
                return null;
            }
            set
            {
                ViewState[V_USERORGINFO] = value;
                SetDetail();
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

        private void SetDetail()
        {
            if (UserDetail != null)
            {
                lblCompany.Text = UserDetail.CompanyName;

                if (!string.IsNullOrWhiteSpace(UserDetail.DepartmentName) && !string.IsNullOrWhiteSpace(UserDetail.DepartmentCode))
                    lblDept.Text = $"{UserDetail.DepartmentName}({UserDetail.DepartmentCode})";

                if (!string.IsNullOrWhiteSpace(UserDetail.OrgBranch) && !string.IsNullOrWhiteSpace(UserDetail.BranchCode))
                    lblBranch.Text = $"{UserDetail.OrgBranch}({UserDetail.BranchCode})";

                lblCostCtr.Text = UserDetail.CostCenterText;

                if (!string.IsNullOrWhiteSpace(UserDetail.OrgSection) && !string.IsNullOrWhiteSpace(UserDetail.SectionCode))
                    lblSection.Text = $"{UserDetail.OrgSection}({UserDetail.SectionCode})";

                if (!string.IsNullOrWhiteSpace(UserDetail.OrgUnit) && !string.IsNullOrWhiteSpace(UserDetail.UnitCode))
                    lblUnit.Text = $"{UserDetail.OrgUnit}({UserDetail.UnitCode})";

                lableEmployeeType.Text = this.UserDetail.EmployeeType.ToString();
                var supervisor = GetUser(UserDetail.ROEmployeeId);
                lableSupervisor.Text = supervisor?.DisplayName;
            }
        }
    }
}