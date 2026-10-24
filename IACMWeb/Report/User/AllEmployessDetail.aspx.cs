using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Report.MySchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Report.User
{
    public partial class AllEmployessDetail : ReportBasePage
    {


        private const string V_Employee = "Employee";
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        public string EmployeeType
        {
            get
            {
                if (ViewState[V_Employee] == null)
                    return "Employee";
                return (string)ViewState[V_Employee];
            }
            set
            {
                ViewState[V_Employee] = value;
            }
        }
        protected void EmployeesGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (EmployeeType.Equals("Employee"))
                EmployeeGrid.DataSource = DomainApiClient<Dto.Presentation.PresentationUser>
             .GetByAction<List<Dto.Presentation.PresentationUser>>(DomainApiAction.Endpoint.Presentation.PresentationUser.GetEmployees, null);
           
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                EmployeeGrid.ExportSettings.FileName = "AllEmployee";
                //Export(EmployeeGrid, e.ActionType);
                var list = DomainApiClient<Dto.Presentation.PresentationUser>
             .GetByAction<List<Dto.Presentation.PresentationUser>>(DomainApiAction.Endpoint.Presentation.PresentationUser.GetEmployees, null);
                ExportData(EmployeeGrid, list, false, e.ActionType == ACLAction.Excel);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                EmployeeGrid.ExportSettings.FileName = "AllEmployee";
                //Export(EmployeeGrid, e.ActionType == ACLAction.ExcelAll ? ACLAction.Excel : ACLAction.CSV);
                var list = DomainApiClient<Dto.Presentation.PresentationUser>
             .GetByAction<List<Dto.Presentation.PresentationUser>>(DomainApiAction.Endpoint.Presentation.PresentationUser.GetEmployees, null);
                ExportData(EmployeeGrid, list, true, e.ActionType == ACLAction.ExcelAll);
            }
            else if (e.ActionType == ACLAction.Schedule)
            {               

            }
        }

        void ExportData(DI.Chakra.Web.CustomControl.IACMRadGridControl grid, List<Dto.Presentation.PresentationUser> userList, bool showAllColumn, bool isExcel)
        {
            if (isExcel)
                ExportDataToExcel(grid, userList, showAllColumn);
            else
                ExportDataToCSV(grid, userList, showAllColumn);
        }
        void ExportDataToExcel(DI.Chakra.Web.CustomControl.IACMRadGridControl grid, List<Dto.Presentation.PresentationUser> userList, bool showAllColumn)
        {
            try
            {
                HttpContext.Current.Session["ExportStatus"] = "false";
                byte[] report = null;
                //collect grid columns
                Dictionary<string, string> columns = new Dictionary<string, string>();
                foreach (GridHeaderItem itm in grid.MasterTableView.GetItems(GridItemType.Header))
                {
                    foreach (GridColumn col in grid.MasterTableView.Columns)
                    {
                        if (col.GetType().Name != "GridTemplateColumn" && col.UniqueName != string.Empty && col.UniqueName != " ")
                        {
                            if (showAllColumn)
                                columns.Add(col.UniqueName, col.HeaderText);
                            else if (!showAllColumn && col.Display)
                                columns.Add(col.UniqueName, col.HeaderText);
                        }
                    }
                }

                OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();

                var workSheet = excel.Workbook.Worksheets.Add("AllEmployee");

                //Header
                // loop top level users
                // add workcell entry for top level
                // TOP User header
                int currentRow = 1;
                int colStartPosition = 0;
                int colNum = colStartPosition;

                foreach (var column in columns)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = column.Value;
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                }

                //move the row index next to header
                currentRow++;
                foreach (var usr in userList)
                {
                    //usr.GetType().GetProperty(propertyName).GetValue(usr, null);
                    colStartPosition = 0;
                    colNum = colStartPosition;
                    foreach (var column in columns)
                    {
                        workSheet.Cells[currentRow, ++colNum].Value = Convert.ToString(usr.GetType().GetProperty(column.Key).GetValue(usr, null));
                    }
                    currentRow++;
                }

                report = excel.GetAsByteArray();

                // Fit the columns according to its content
                for (int i = 1; i < colNum; i++)
                {
                    workSheet.Column(i).AutoFit();
                }

                if (report != null)
                {
                    HttpResponse response = HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition", "attachment;filename=" + string.Format("AllEmployee.xlsx"));
                    response.BinaryWrite(report);
                    response.End();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                HttpContext.Current.Session["ExportStatus"] = "true";
            }
        }

        void ExportDataToCSV(DI.Chakra.Web.CustomControl.IACMRadGridControl grid, List<Dto.Presentation.PresentationUser> userList, bool showAllColumn)
        {
            try
            {
                HttpContext.Current.Session["ExportStatus"] = "false";
                StringBuilder objWriter = new StringBuilder();
                byte[] report = null;
                //collect grid columns
                Dictionary<string, string> columns = new Dictionary<string, string>();
                foreach (GridHeaderItem itm in grid.MasterTableView.GetItems(GridItemType.Header))
                {
                    foreach (GridColumn col in grid.MasterTableView.Columns)
                    {
                        if (col.GetType().Name != "GridTemplateColumn" && col.UniqueName != string.Empty && col.UniqueName != " ")
                        {
                            if (showAllColumn)
                                columns.Add(col.UniqueName, col.HeaderText);
                            else if (!showAllColumn && col.Display)
                                columns.Add(col.UniqueName, col.HeaderText);
                        }
                    }
                }

                //Header
                string header = string.Join(",", columns.Keys);
                objWriter.AppendLine(header);

                var previousParentEndsorer = "";
                foreach (var usr in userList)
                {
                    List<string> towrite = new List<string>();

                    foreach (var column in columns)
                    {
                        towrite.Add(Convert.ToString(usr.GetType().GetProperty(column.Key).GetValue(usr, null)));
                    }

                    objWriter.AppendLine(string.Join(",", towrite));
                }
                report = Encoding.ASCII.GetBytes(objWriter.ToString());

                if (report != null)
                {
                    HttpResponse response = HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition", "attachment;filename=" + string.Format("Employee.csv"));
                    response.BinaryWrite(report);
                    response.End();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                HttpContext.Current.Session["ExportStatus"] = "true";
            }
        }

    }
}