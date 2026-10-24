using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Web.Report.User.UC
{
    public partial class ActiveEmployeesListUC : UIBaseUC, IAccessControl
    {
        private const string V_EmployeeType = "EmployeeType";
        private static IList<Dto.Presentation.PresentationUser> listActiveEmployees = new List<Dto.Presentation.PresentationUser>();
        public string EmployeeType
        {
            get
            {
                if (ViewState[V_EmployeeType] == null)
                    return "All";
                return (string)ViewState[V_EmployeeType];
            }
            set
            {
                ViewState[V_EmployeeType] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                BindUserType();
            }
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                IACMGridUserList.ExportSettings.FileName = "Active Employees";
                IACMGridUserList.Export(IACMGridUserList, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                IACMGridUserList.Export<IList<Dto.Presentation.PresentationUser>>(listActiveEmployees, e.ActionType, "Active Employees");
            }
        }

        private void BindUserType()
        {
            ddlUserType.Items.Clear();
            ddlUserType.Items.Insert(0, "All");
            ddlUserType.Items.Insert(1, "Employee");
            ddlUserType.Items.Insert(2, "Non Employee");
            ddlUserType.Items.FindByValue("All").Selected = true;
        }

        protected void ddlUserType_SelectedIndexChanged(object sender, EventArgs e)
        {
            EmployeeType = ddlUserType.SelectedValue;
            LoadGrid();
            IACMGridUserList.DataBind();
        }

        protected void IACMGridUserList_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            LoadGrid();
        }

        public void LoadGrid()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("employeeType", EmployeeType);
            listActiveEmployees = DomainApiClient<Dto.Presentation.PresentationUser>
             .GetByAction<List<Dto.Presentation.PresentationUser>>(DomainApiAction.Endpoint.Presentation.PresentationUser.GetActiveEmployees, parameter);
            IACMGridUserList.DataSource = listActiveEmployees;
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {

        }
    }
}

