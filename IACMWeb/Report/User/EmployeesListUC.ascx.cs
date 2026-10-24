using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Report.User
{
    public partial class EmployeesListUC : UIBaseUC, IAccessControl
    {
       
        private static List<Dto.Presentation.PresentationUser> userList;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;

        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                EmployeeGrid.ExportSettings.FileName = "User";
                EmployeeGrid.Export(EmployeeGrid, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                EmployeeGrid.Export<List<Dto.Presentation.PresentationUser>>(userList, e.ActionType, "User");
            }

        }

        protected void EmployeesGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {

          
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                if (DatePickerFromDate.SelectedDate != null)
                    parameters.Add("FromDate", DatePickerFromDate.SelectedDate.ToString());
                else
                    parameters.Add("FromDate", string.Empty);

                if (DatePickerToDate.SelectedDate != null)
                    parameters.Add("ToDate", DatePickerToDate.SelectedDate.ToString());
                else
                    parameters.Add("ToDate", string.Empty);
                bool? isActive = null;
                if (rbInActive.Checked)
                    isActive = rbInActive.Checked;
                if (rbActive.Checked)
                    isActive = rbActive.Checked;
                if (isActive.HasValue)
                    parameters.Add("IsActive", isActive.ToString());
                else
                    parameters.Add("IsActive", string.Empty);
            userList = DomainApiClient<Dto.Presentation.PresentationUser>
              .GetByAction<List<Dto.Presentation.PresentationUser>>(DomainApiAction.Endpoint.Presentation.PresentationUser.GetEmployees, parameters);

            EmployeeGrid.DataSource = userList;
        }

        protected void search_Click(object sender, EventArgs e)
        {
            if (DatePickerFromDate.SelectedDate == null && DatePickerToDate.SelectedDate == null)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Valid Date must be required");
                return;
            }
            EmployeeGrid.Rebind();
        }
    }
}