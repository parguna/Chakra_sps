using Application;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Core;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Web.Report.MySchedule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Prov.UserAccessRequest
{
    public partial class UserAccessDetailMatrix : UIBasePage
    {
        private bool ExportMode;
        private bool ExportTCode;
        private static string selectedInstitute;
        private static int selectedDept;
        private bool ShowAll = false;

        private bool ShowDirectSubordinate
        {
            get
            {
                if (ViewState["ShowDirectSubordinate"] == null)
                {
                    ViewState["ShowDirectSubordinate"] = false;
                }
                return Convert.ToBoolean(ViewState["ShowDirectSubordinate"]);
            }
            set
            {
                ViewState["ShowDirectSubordinate"] = value;
            }
        }
        private bool HideRoColumn
        {
            get
            {
                if (ViewState["HideRoColumn"] == null)
                {
                    ViewState["HideRoColumn"] = true;
                }
                return Convert.ToBoolean(ViewState["HideRoColumn"]);
            }
            set
            {
                ViewState["HideRoColumn"] = value;
            }
        }
        /// <summary>
        /// 1. Submit 
        /// 2. All
        /// 3. Direct Subordinates
        /// 4. All Subordinates
        /// </summary>
        private int ReportType
        {
            get
            {
                if (ViewState["ReportType"] == null)
                {
                    ViewState["ReportType"] = 1;
                }
                return Convert.ToInt32(ViewState["ReportType"]);
            }
            set
            {
                ViewState["ReportType"] = value;
            }
        }
        public User SessionUser { get { return new SessionStatusImpl().User; } }
       
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                UserRadTreeList.PageSize = 50;
                UserRadTreeList.MasterTableView.PageSize = 50;
                Session["PresentationGridPagedData"] = null;
                btnShowALL.Visible = false;
                //btnExcelExport.Visible = false;
                //btnExcelExportALL.Visible = false;
                //btnExcelExport.Enabled = false;
                //btnExcelExportALL.Enabled = false;
                ExportMode = false;
                ExportTCode = false;
                UserRadTreeList.Visible = false;
                //usrList = new UserFacade().GetAllUsersIUFL() as List<User>;
                selectedInstitute = string.Empty;
                selectedDept = -1;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", SessionUser.Id.ToString());

                var isAdminUser = DomainApiClient<Dto.User>.GetByAction<bool>(DomainApiAction.User.IsAdminUser, parameter);
                UserAutoCompleteBox.WebServiceSettings.Method = !isAdminUser ? "GetAllEmployeesUnderRO" : "GetReportingUsersOfThisManagerIDandAdmin_New";
                if (isAdminUser)
                {
                    btnAllSubordinates.Visible = false;
                    btnDirectSubordinates.Visible = false;
                    autoCompleteDepartment.Enabled = false;
                   // autoCompleteDivision.Enabled = false;
                    autoCompleteBranch.Enabled = false;
                    autoCompleteSection.Enabled = false;
                    autoCompleteUnit.Enabled = false;
                    autocompany.Visible = true;
                   // autoDivision.Visible = true;
                    autoDept.Visible = true;
                    autoSection.Visible = true;
                    autobranch.Visible = true;
                    autoUnit.Visible = true;
                    btnShowALL.Visible = true;
                    divSpace.Visible = true;
                }
            }

            if (ViewState["ShowAll"] != null)
            {
                ShowAll = (bool)ViewState["ShowAll"];
            }

            UserRadTreeList.ExportSettings.Pdf.PageHeader.MiddleCell.Text = "User Access Details  ";
            UserRadTreeList.ExportSettings.Pdf.PageHeader.MiddleCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
            UserRadTreeList.ExportSettings.Pdf.PageHeader.LeftCell.Text = PDFFileLeftCell;
            UserRadTreeList.ExportSettings.Pdf.PageHeader.LeftCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
            GetDeptName();

            //invisibleBetween(UserRadTreeList.FilterMenu); 
        }

        protected void ddlUser_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private bool Flag = false;
        List<Dto.Presentation.UserAccessDetailMatrixGridData> GetUserAccessDetailMatrixGridData(long userId)
        {
            var pageResult = GetUserAccessDetailByPaging(new List<long>() { userId });
            Session["UserAccessDetails"] = pageResult.Data;
            return pageResult.Data;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            ReportType = 1;
            ShowDirectSubordinate = false;
            ShowAll = false;
            ViewState["ShowAll"] = ShowAll;
            // ViewState["UserAccessDetails"] = null;
            Session["UserAccessDetails"] = null;
            if (UserAutoCompleteBox.Entries.Count > 0)
            {
                int userId = 0;
                bool cndn = int.TryParse(UserAutoCompleteBox.Entries[0].Value, out userId);

                if (cndn == true && userId > 1)
                {
                    UserRadTreeList.CurrentPageIndex = 0;
                    UserRadTreeList.DataSource = GetUserAccessDetailMatrixGridData(userId);
                    UserRadTreeList.DataBind();

                    //btnExcelExport.Enabled = false;
                    //btnExcelExportALL.Enabled = false;
                    ExportMode = false;
                    ExportTCode = false;
                    string[] UserName = UserAutoCompleteBox.Entries[0].Text.Split('[');
                    string Usrname = UserName[1].Substring(0, UserName[1].Count() - 1);
                    string message = string.Format(Usrname + " " + "Access Details Viewed By" + " " + new SessionStatusImpl().User.UserName);

                    if (UserRadTreeList.Items.Count == 0)
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "No records found.");
                        UserRadTreeList.Visible = false;
                    }
                    else
                        UserRadTreeList.Visible = true;

                    WriteInfoLog(message);
                }
            }
            else if (UserAutoCompleteBox.Entries.Count == 0)
            {
                MasterPage.ShowAlert(AlertType.Error, "Select User Name");
                UserRadTreeList.DataSource = null;
                UserRadTreeList.DataBind();
                UserRadTreeList.Visible = false;
            }
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
           
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                PresentationGridPagedDataParameter param = Session["PresentationGridPagedData"] as PresentationGridPagedDataParameter;
                if(param != null)
                {
                    UpdateFilterAndSortingParameter(param);
                    param.MaximumRows = -1;
                    Session["ExportStatus"] = "false";
                    var fileType = (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.ExcelAll) ? "xlsx" : "csv";
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("exportFileType", fileType);
                    var data = DomainApiClient<Dto.User>.GetByAction<byte[]>("User", DomainApiAction.User.DownloadUserAccessDetail, parameter, param);
                    if (data != null)
                    {
                        Session["ExportStatus"] = "true";
                        HttpResponse response = Response;
                        response.Clear();
                        response.ClearContent();
                        response.ClearHeaders();
                        response.Buffer = true;
                        response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        response.AddHeader("Content-Length", data.Length.ToString());
                        response.AddHeader("Content-Disposition", "attachment;filename=" + string.Format("UserAccessDetails_{1}.{0}", fileType,DateTime.Now.ToString(Constants.Report_Date_Format)));
                        response.BinaryWrite(data);
                        response.End();
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
            }
        }
        void BindDatasource(bool allSubordinates = false)
        {
            Session["UserAccessDetails"] = null;
            List<Dto.Presentation.UserAccessDetailMatrixGridData> col = GetAccessDetailForAllUsersBasedOrgStructure(allSubordinates);
            UserRadTreeList.DataSource = col;
            UserRadTreeList.DataBind();
            //btnExcelExport.Enabled = false;
            //btnExcelExportALL.Enabled = false;
            ExportMode = false;
            ExportTCode = false;
            if (UserRadTreeList.Items.Count == 0)
            {
                MasterPage.ShowAlert(AlertType.Warning, "No records found.");
                UserRadTreeList.Visible = false;
            }
            else
            {
                Session["UserAccessDetails"] = UserRadTreeList.DataSource;
                UserRadTreeList.Visible = true;
            }
        }

        protected void btnShowALL_Click(object sender, EventArgs e)
        {
            ReportType = 2;
            GetCompanyName();
            GetDeptName();
            //GetDivisionName();
            GetBranchName();
            GetSectionName();
            ShowAll = true;
            //HideRoColumn = true;
            ViewState["ShowAll"] = ShowAll;
            Session["UserAccessDetails"] = null;
            ViewState["UserAccessDetails"] = null;
            ShowDirectSubordinate = false;
            BindDatasource();
        }

        public List<Dto.Presentation.UserAccessDetailMatrixGridData> GetAccessDetailForAllUsersBasedOrgStructure(bool allSubordinates = false)
        {
            Dictionary<long, string> employeeIds = null;
            if (allSubordinates)
            {
                var parameter = new Dictionary<string, string>();
                parameter.Add("roEmployeeId", SessionUser.EmployeeId);
                parameter.Add("searchString", string.Empty);
                employeeIds = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>(DomainApiAction.User.GetAllEmployeesUnderRO, parameter);
            }
            else
            {
                var CompanyNameFilter = string.Empty;
                var DepartmentNameFilter = string.Empty;
                //var DivisionNameFilter = string.Empty;
                var BranchNameFilter = string.Empty;
                var SectionNameFilter = string.Empty;
                var UnitNameFilter = string.Empty;

                if (HttpContext.Current.Session["CompanyName"] != null)
                {
                    CompanyNameFilter = Convert.ToString(HttpContext.Current.Session["CompanyName"]);
                }
                if (HttpContext.Current.Session["DepartmentName"] != null)
                {
                    DepartmentNameFilter = Convert.ToString(HttpContext.Current.Session["DepartmentName"]);
                }
                //if (HttpContext.Current.Session["DivisionName"] != null)
                //{
                //    DivisionNameFilter = Convert.ToString(HttpContext.Current.Session["DivisionName"]);
                //}
                if (HttpContext.Current.Session["BranchName"] != null)
                {
                    BranchNameFilter = Convert.ToString(HttpContext.Current.Session["BranchName"]);
                }
                if (HttpContext.Current.Session["SectionName"] != null)
                {
                    SectionNameFilter = Convert.ToString(HttpContext.Current.Session["SectionName"]);
                }
                if (HttpContext.Current.Session["UnitName"] != null)
                {
                    UnitNameFilter = Convert.ToString(HttpContext.Current.Session["UnitName"]);
                }

                var param = new Dictionary<string, string>();
                param.Add("searchString", string.Empty);
                param.Add("CompanyNameFilter", CompanyNameFilter);
                param.Add("DepartNameFilter", DepartmentNameFilter);
                //param.Add("DivisionNameFilter", DivisionNameFilter);
                param.Add("BranchNameFilter", BranchNameFilter);
                param.Add("GetOrgUnits", SectionNameFilter);
                param.Add("SectioNameFilter", SectionNameFilter);
                param.Add("UnitNameFilter", UnitNameFilter);
                param.Add("userId", SessionUser.Id.ToString());

                employeeIds = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>(DomainApiAction.User.GetReportingUsersOfThisManagerIDandAdmin_New, param);
            }

            if (employeeIds == null || employeeIds.Count == 0)
                return null;
            PresentationGridPagedData<UserAccessDetailMatrixGridData> pageResult = GetUserAccessDetailByPaging(employeeIds.Keys.ToList());
            //var UserAccessDetails = DomainApiClient<Dto.User>.GetByAction<List<Dto.Presentation.UserAccessDetailMatrixGridData>>("User", DomainApiAction.User.GetUserAccessDetailMatrixGridData, null, userInput);
            Session["UserAccessDetails"] = pageResult.Data;
            return pageResult.Data;
        }
        private void BindFilteredData(PresentationGridPagedDataParameter param)
        {
            UpdateFilterAndSortingParameter(param);
            var pageResult = DomainApiClient<Dto.User>.GetByAction<Dto.Core.PresentationGridPagedData<Dto.Presentation.UserAccessDetailMatrixGridData>>("User", DomainApiAction.User.GetUserAccessDetailMatrixPagedData, null, param);
            UserRadTreeList.VirtualItemCount = pageResult.TotalCount;
            Session["UserAccessDetails"] = pageResult.Data;
            UserRadTreeList.DataSource = pageResult.Data;
        }
        private void UpdateFilterAndSortingParameter(PresentationGridPagedDataParameter param)
        {
            int PageIndex = UserRadTreeList.MasterTableView.PageSize*UserRadTreeList.CurrentPageIndex;
            int PageSize = UserRadTreeList.MasterTableView.PageSize;
            string sortExpressions = string.Empty;
            string sortingFieldName = "";
            string sortOrder = "";
            if (UserRadTreeList.MasterTableView.SortExpressions.Count > 0)
            {
                sortingFieldName = UserRadTreeList.MasterTableView.SortExpressions[0].FieldName;
                sortOrder = UserRadTreeList.MasterTableView.SortExpressions[0].SortOrderAsString();
                foreach (GridSortExpression item in UserRadTreeList.MasterTableView.SortExpressions)
                {
                    sortExpressions += string.Format("{0} {1},", item.FieldName, item.SortOrderAsString());
                }
            }

            var filterExpression = UserRadTreeList.MasterTableView.FilterExpression;

            if (filterExpression.Contains("DateTime.Parse"))
            {
                List<string> finalOutput = new List<string>();
                foreach (string condition in filterExpression.Split(new string[] { " AND " }, StringSplitOptions.None))
                {
                    string rawExpression = condition;
                    var findMatch = Regex.Match(rawExpression, "DateTime.Parse(.*)", RegexOptions.IgnoreCase);
                    if (findMatch.Success)
                    {
                        var dtExtract = DateTime.Parse(findMatch.Value.Split('\"')[1]);
                        var conditionDate = string.Format("DateTime({0}, {1}, {2})", dtExtract.Year, dtExtract.Month, dtExtract.Day);
                        finalOutput.Add(Regex.Replace(rawExpression, "DateTime.Parse(.*)", string.Format("{0})", conditionDate), RegexOptions.IgnoreCase));
                    }
                    else
                        finalOutput.Add(rawExpression);
                }

                filterExpression = string.Join(" AND ", finalOutput);
            }
            param.SortExpression = sortExpressions;
            param.FilterExpression = filterExpression;
            param.StartRowIndex = PageIndex;
            param.MaximumRows = PageSize;
        }
        private PresentationGridPagedData<UserAccessDetailMatrixGridData> GetUserAccessDetailByPaging(List<long> employeeIds)
        {
            var onlyIds = employeeIds;
            int PageIndex = UserRadTreeList.CurrentPageIndex * UserRadTreeList.MasterTableView.PageSize;
            int PageSize = UserRadTreeList.MasterTableView.PageSize;
            string sortExpressions = string.Empty;
            string sortingFieldName = "";
            string sortOrder = "";
            if (UserRadTreeList.MasterTableView.SortExpressions.Count > 0)
            {
                sortingFieldName = UserRadTreeList.MasterTableView.SortExpressions[0].FieldName;
                sortOrder = UserRadTreeList.MasterTableView.SortExpressions[0].SortOrderAsString();
                foreach (GridSortExpression item in UserRadTreeList.MasterTableView.SortExpressions)
                {
                    sortExpressions += string.Format("{0} {1},", item.FieldName, item.SortOrderAsString());
                }
                sortExpressions = sortExpressions.Trim(',').Trim();
            }

            var filterExpression = UserRadTreeList.MasterTableView.FilterExpression;

            if (filterExpression.Contains("DateTime.Parse"))
            {
                List<string> finalOutput = new List<string>();
                foreach (string condition in filterExpression.Split(new string[] { " AND " }, StringSplitOptions.None))
                {
                    string rawExpression = condition;
                    var findMatch = Regex.Match(rawExpression, "DateTime.Parse(.*)", RegexOptions.IgnoreCase);
                    if (findMatch.Success)
                    {
                        var dtExtract = DateTime.Parse(findMatch.Value.Split('\"')[1]);
                        var conditionDate = string.Format("DateTime({0}, {1}, {2})", dtExtract.Year, dtExtract.Month, dtExtract.Day);
                        finalOutput.Add(Regex.Replace(rawExpression, "DateTime.Parse(.*)", string.Format("{0})", conditionDate), RegexOptions.IgnoreCase));
                    }
                    else
                        finalOutput.Add(rawExpression);
                }

                filterExpression = string.Join(" AND ", finalOutput);
            }

            PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter()
            {
                Ids = onlyIds,
                StartRowIndex = PageIndex,
                MaximumRows = PageSize,
                FilterExpression = filterExpression,
                SortExpression = sortExpressions.Trim(',').Trim(),
            };
            Session["PresentationGridPagedData"] = param;
            var pageResult = DomainApiClient<Dto.User>.GetByAction<Dto.Core.PresentationGridPagedData<Dto.Presentation.UserAccessDetailMatrixGridData>>("User", DomainApiAction.User.GetUserAccessDetailMatrixPagedData, null, param);
            UserRadTreeList.VirtualItemCount = pageResult.TotalCount;
            return pageResult;
        }

        /// <summary>
        /// Excel Export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExcelExport_Click(object sender, EventArgs e)
        {
            if (UserRadTreeList.Items.Count > 0)
            {
                ExportMode = true;
                ExportTCode = false;
                //foreach (GridDataItem item in UserRadTreeList.MasterTableView.Items)
                //{
                //    item.Expanded = true;
                //}
                UserRadTreeList.ExportSettings.FileName = "UserAccessDetailMatrix";
                UserRadTreeList.Export(UserRadTreeList, ACLAction.ExcelAll);
            }
        }
        void gridControl_ExportCellFormatting(object sender, ExportCellFormattingEventArgs e)
        {
            if ((e.FormattedColumn.UniqueName).ToLower().Contains("date"))
            {
                e.Cell.Style["mso-number-format"] = "Medium Date";
            }
            GridDataItem item = e.Cell.Parent as GridDataItem;
            e.Cell.HorizontalAlign = HorizontalAlign.Center;
        }
        public new void Export(RadGrid gridControl, ACLAction actionType)
        {
            try
            {
                HttpContext.Current.Session["ExportStatus"] = "false";
                //if (gridControl.Items.Count > 0)
                {
                    gridControl.ExportSettings.IgnorePaging = true;
                    gridControl.GridLines = GridLines.Both;
                    gridControl.AllowFilteringByColumn = false;
                    string filename = gridControl.ExportSettings.FileName;
                    gridControl.ExportCellFormatting += new EventHandler<ExportCellFormattingEventArgs>(gridControl_ExportCellFormatting);

                    if (actionType == ACLAction.CSV || actionType == ACLAction.CSVAll)
                    {
                        if (filename == "RiskScore" || filename == "RiskLevel" || filename == "RiskCode" || filename == "Mail-Server" || filename == "Provisioning Fields")
                        {
                            gridControl.ExportSettings.ExportOnlyData = false;
                        }
                        else
                        {
                            gridControl.ExportSettings.ExportOnlyData = true;
                        }
                        gridControl.MasterTableView.ExportToCSV();
                    }
                    else if (actionType == ACLAction.Excel || actionType == ACLAction.ExcelAll)
                    {
                        gridControl.ExportSettings.ExportOnlyData = true;
                        gridControl.MasterTableView.AllowFilteringByColumn = false;
                        gridControl.MasterTableView.ExportToExcel();
                    }
                }
                //else
                //{
                //    MasterPage.ShowAlert(AlertType.Error, "No data to export");
                //}
            }
            catch (Exception e)
            {
                MasterPage.ShowAlert(AlertType.Error, "Export report failed. Please contact Administrator.");
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Exception, "Export Report - " + gridControl.ExportSettings.FileName);
            }
            finally
            {
                HttpContext.Current.Session["ExportStatus"] = "true";
            }
        }
        public void ExportWithGroupBy(ACLAction actionType)
        {
            try
            {
                HttpContext.Current.Session["ExportStatus"] = "false";
                if (UserRadTreeList.Items.Count > 0)
                {
                    UserRadTreeList.ExportSettings.FileName = "UserAccessDetailMatrix";
                    UserRadTreeList.ExportSettings.HideStructureColumns = true;
                    UserRadTreeList.ExportSettings.IgnorePaging = true;
                    UserRadTreeList.ExportSettings.ExportOnlyData = true;
                    UserRadTreeList.ExportSettings.OpenInNewWindow = true;
                    UserRadTreeList.ExportSettings.Excel.Format = GridExcelExportFormat.ExcelML;
                    if (actionType == ACLAction.CSV || actionType == ACLAction.CSVAll)
                    {
                        UserRadTreeList.MasterTableView.ExportToCSV();
                    }
                    else
                    {
                        UserRadTreeList.MasterTableView.ExportToExcel();
                    }
                }
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, "Export report failed. Please contact Administrator.");
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Exception, "Export Report - " + UserRadTreeList.ExportSettings.FileName, ex);
            }
            finally
            {
                HttpContext.Current.Session["ExportStatus"] = "true";
            }
        }
        protected void btnExcelExportALL_Click(object sender, EventArgs e)
        {
            if (UserRadTreeList.Items.Count > 0)
            {
                foreach (GridDataItem dataItem in UserRadTreeList.MasterTableView.Items)
                {
                    dataItem.Expanded = false;
                }

                ExportMode = true;
                ExportTCode = true;
                foreach (GridDataItem item in UserRadTreeList.MasterTableView.Items)
                {
                    item.Expanded = true;
                    var EndpointSystemTypeId = Convert.ToInt64(item["EndpointSystemTypeId"].Text);
                    if (EndpointSystemTypeId == (long)EndpointType.SAP)
                    {
                        foreach (GridDataItem nestedItem in item.ChildItem.NestedTableViews[0].Items)
                        {
                            nestedItem.Expanded = true;
                        }
                    }
                }
                ExportWithGroupBy(ACLAction.ExcelAll);
            }
        }
        /// <summary>
        /// Export Excel
        /// </summary>
        /// <param name="RadGrid1"> RadGrid Name</param>
        public void ExportExcelPDF(RadGrid UserAccessDetailsGrid, string ExportFormatStyle)
        {
            UserAccessDetailsGrid.ExportSettings.ExportOnlyData = true;
            UserAccessDetailsGrid.ExportSettings.IgnorePaging = true;
            UserAccessDetailsGrid.ExportSettings.OpenInNewWindow = true;
            UserAccessDetailsGrid.ExportSettings.FileName = "file." + ExportFormatStyle;
            UserAccessDetailsGrid.ExportSettings.Excel.Format = GridExcelExportFormat.Html;
            UserAccessDetailsGrid.ExportSettings.HideStructureColumns = true;
            UserAccessDetailsGrid.MasterTableView.HierarchyDefaultExpanded = true;
            UserAccessDetailsGrid.MasterTableView.DetailTables[0].HierarchyDefaultExpanded = true;
            UserAccessDetailsGrid.MasterTableView.HierarchyLoadMode = GridChildLoadMode.Client;
            UserAccessDetailsGrid.MasterTableView.DetailTables[0].HierarchyLoadMode = GridChildLoadMode.Client;

            if (ExportFormatStyle == "xls") { UserAccessDetailsGrid.MasterTableView.ExportToExcel(); } else { UserAccessDetailsGrid.MasterTableView.ExportToPdf(); }
        }

        protected void UserRadTreeList_DetailTableDataBind(object sender, Telerik.Web.UI.GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = e.DetailTableView.ParentItem;
            long EndpointSystemID, EndpointSystemTypeId;
            var lstUserAccessDetails = Session["UserAccessDetails"] as List<UserAccessDetailMatrixGridData>;
            switch (e.DetailTableView.Name)
            {
                case "Roles":
                    long accountReconId = (long)dataItem.GetDataKeyValue("AccountReconcilationId");
                    var finallist = lstUserAccessDetails.Where(lq => lq.AccountReconcilationId == accountReconId).ToList();
                    e.DetailTableView.DataSource = finallist;

                    //btnExcelExport.Enabled = false;
                    //string rolesorgroup = finallist.Where(lq => lq.Type == "Role").Count() > 0 ? "Role" : "Group";
                    //if (finallist.Count == 0) rolesorgroup = "Role / Group";
                    //e.DetailTableView.Columns.FindByUniqueNameSafe("Name").HeaderText = "Entitlement";

                    break;
                case "TCode":

                    EndpointSystemID = (long)dataItem.GetDataKeyValue("EndpointSystemID");
                    EndpointSystemTypeId = (long)dataItem.GetDataKeyValue("EndpointSystemTypeId");
                    //endPoint = new EndPointFacade().GetEndPointSystemByKey(EndpointSystemID);
                    if (EndpointSystemTypeId == (long)EndpointType.SAP)
                    {
                        if (ExportMode && !ExportTCode)
                        {
                            e.DetailTableView.Visible = false;
                            List<SAPTCodeInfo> sapTCodes = new List<SAPTCodeInfo>();
                            //if (ExportMode == false)
                            sapTCodes.Add(new SAPTCodeInfo { TCODE_Code = "", EndPointSystemId = EndpointSystemID, Role = "" });
                            e.DetailTableView.DataSource = sapTCodes;
                            e.DetailTableView.Visible = false;
                        }
                        else
                        {
                            string rolecode = (string)dataItem.GetDataKeyValue("Entitlement");
                            string roletype = (string)dataItem.GetDataKeyValue("EntitlementType");
                            if (roletype == "Role")
                            {
                                var parameter = new Dictionary<string, string>();
                                parameter.Add("roleCode", rolecode);
                                parameter.Add("endPointSystemId", EndpointSystemID.ToString());
                                var sapTCodes = DomainApiClient<Dto.SAPTCodeInfo>.GetByAction<List<Dto.SAPTCodeInfo>>
                                    (DomainApiAction.Endpoint.SAP.SAPTCodeInfo.GetByRoleCode, parameter);
                                if (ExportMode == false)
                                    sapTCodes.Add(new SAPTCodeInfo { TCODE_Code = "Authorization", EndPointSystemId = EndpointSystemID, Role = rolecode });
                                else
                                    sapTCodes.Add(new SAPTCodeInfo { TCODE_Code = "", EndPointSystemId = EndpointSystemID, Role = rolecode });

                                e.DetailTableView.DataSource = sapTCodes;
                            }
                            else
                            {
                                List<SAPTCodeInfo> sapTCodes = new List<SAPTCodeInfo>();
                                sapTCodes.Add(new SAPTCodeInfo { TCODE_Code = "", EndPointSystemId = EndpointSystemID, Role = rolecode });
                                e.DetailTableView.DataSource = sapTCodes;
                                e.DetailTableView.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        List<SAPTCodeInfo> sapTCodes = new List<SAPTCodeInfo>();
                        //if (ExportMode == false)
                        sapTCodes.Add(new SAPTCodeInfo { TCODE_Code = "Authorization", EndPointSystemId = EndpointSystemID, Role = "" });
                        e.DetailTableView.DataSource = sapTCodes;
                        e.DetailTableView.Visible = false;
                    }

                    break;

                case "Authorization":
                    if (dataItem.GetDataKeyValue("EndpointSystemID") != null && ExportMode == false)
                    {
                        EndpointSystemID = (long)dataItem.GetDataKeyValue("EndpointSystemID");
                        EndpointSystemTypeId = (long)dataItem.GetDataKeyValue("EndpointSystemTypeId");
                        if (EndpointSystemTypeId == (long)EndpointType.SAP && ExportMode == false)
                        {
                            string rolecode = (string)dataItem.GetDataKeyValue("Entitlement");
                            string TCODE_Code = (string)dataItem.GetDataKeyValue("TCODE_Code");
                            if (TCODE_Code == "Authorization")
                            {
                                List<AttestationReviewPageSAP_Level1GridData> sapLevel2Details = this.getLevel2DetailsForAuthorization(rolecode, EndpointSystemID);
                                var l1EndpointSystemsSAP = (from dtls in sapLevel2Details
                                                            select new AttestationReviewPageSAP_Level1GridData
                                                            {
                                                                sapLevel1DetailId = dtls.sapLevel1DetailId,
                                                                sapLevel1Detail = dtls.sapLevel1Detail,
                                                                sapLevel1DetailDesc = dtls.sapLevel1DetailDesc,
                                                                //IsChanged = false
                                                            });

                                e.DetailTableView.DataSource = l1EndpointSystemsSAP;
                            }
                            else
                            {
                                IList<AttestationReviewPageSAP_Level1GridData> emptyDataSource = new List<AttestationReviewPageSAP_Level1GridData>();

                                e.DetailTableView.DataSource = emptyDataSource;
                                e.DetailTableView.Visible = false;
                            }
                        }
                        else
                        {
                            e.DetailTableView.Visible = false;
                        }
                    }

                    break;
            }

        }

        private List<AttestationReviewPageSAP_Level1GridData> getLevel2DetailsForAuthorization(string paramSingleRoleCode, long paramEndPointSystemId)
        {
            var parameter = new Dictionary<string, string>();
            parameter.Add("RoleCode", paramSingleRoleCode);
            parameter.Add("EndpointSystemId", paramEndPointSystemId.ToString());
            var result = DomainApiClient<Dto.SAPRoleInfo>.GetByAction<List<Dto.Presentation.AttestationReviewPageSAP_Level1GridData>>
                ("GetAuthorizationDataForAttestationReviewGrid", parameter);

            return result;
        }
        protected void UserRadTreeList_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            PresentationGridPagedDataParameter param = Session["PresentationGridPagedData"] as PresentationGridPagedDataParameter;
            if(param != null)
            {
                BindFilteredData(param);
            }
        }
        protected void AllUsers_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void autoCompleteCompany_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {

            if (autoCompleteCompany.Entries.Count > 0)
            {
                autoCompleteDepartment.Entries.Clear();
                autoCompleteDepartment.Enabled = true;
                //autoCompleteDivision.Entries.Clear();
                //autoCompleteDivision.Enabled = false;
                autoCompleteBranch.Entries.Clear();
                autoCompleteBranch.Enabled = false;
                autoCompleteSection.Entries.Clear();
                autoCompleteSection.Enabled = false;
                autoCompleteUnit.Entries.Clear();
                autoCompleteUnit.Enabled = false;
            }
            GetCompanyName();
            GetDeptName();
            GetDivisionName();
            GetBranchName();
            GetSectionName();
            GetUnitName();

        }
        protected void autoCompleteCompany_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {

            if (autoCompleteCompany.Entries.Count == 0)
            {
                autoCompleteDepartment.Entries.Clear();
                autoCompleteDepartment.Enabled = false;
                //autoCompleteDivision.Entries.Clear();
                //autoCompleteDivision.Enabled = false;
                autoCompleteBranch.Entries.Clear();
                autoCompleteBranch.Enabled = false;
                autoCompleteSection.Entries.Clear();
                autoCompleteSection.Enabled = false;
                autoCompleteUnit.Entries.Clear();
                autoCompleteUnit.Enabled = false;
            }
            GetCompanyName();
            GetDeptName();
            GetDivisionName();
            GetBranchName();
            GetSectionName();
            GetUnitName();

        }

        protected void autoCompleteDepartment_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {

            if (autoCompleteDepartment.Entries.Count > 0)
            {
                //autoCompleteDivision.Entries.Clear();
                //autoCompleteDivision.Enabled = true;
                autoCompleteBranch.Entries.Clear();
                autoCompleteBranch.Enabled = false;
                autoCompleteSection.Entries.Clear();
                autoCompleteSection.Enabled = false;
                autoCompleteUnit.Entries.Clear();
                autoCompleteUnit.Enabled = false;
            }
            GetCompanyName();
            GetDeptName();
            GetDivisionName();
            GetBranchName();
            GetSectionName();
            GetUnitName();
        }
        protected void autoCompleteDepartment_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {

            if (autoCompleteDepartment.Entries.Count == 0)
            {
                //autoCompleteDivision.Entries.Clear();
                //autoCompleteDivision.Enabled = false;
                autoCompleteBranch.Entries.Clear();
                autoCompleteBranch.Enabled = false;
                autoCompleteSection.Entries.Clear();
                autoCompleteSection.Enabled = false;
                autoCompleteUnit.Entries.Clear();
                autoCompleteUnit.Enabled = false;
            }
            GetCompanyName();
            GetDeptName();
            GetDivisionName();
            GetBranchName();
            GetSectionName();
            GetUnitName();
        }

        protected void autoCompleteDivision_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {

            //if (autoCompleteDivision.Entries.Count > 0)
            //{
            //    autoCompleteBranch.Entries.Clear();
            //    autoCompleteBranch.Enabled = true;
            //    autoCompleteSection.Entries.Clear();
            //    autoCompleteSection.Enabled = false;
            //    autoCompleteUnit.Entries.Clear();
            //    autoCompleteUnit.Enabled = false;
            //}
            GetCompanyName();
            GetDeptName();
            GetDivisionName();
            GetBranchName();
            GetSectionName();
            GetUnitName();
        }
        protected void autoCompleteDivision_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {

            //if (autoCompleteDivision.Entries.Count == 0)
            //{
            //    autoCompleteBranch.Entries.Clear();
            //    autoCompleteBranch.Enabled = false;
            //    autoCompleteSection.Entries.Clear();
            //    autoCompleteSection.Enabled = false;
            //    autoCompleteUnit.Entries.Clear();
            //    autoCompleteUnit.Enabled = false;
            //}
            GetCompanyName();
            GetDeptName();
            GetDivisionName();
            GetBranchName();
            GetSectionName();
            GetUnitName();
        }
        protected void autoCompleteBranch_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {

            if (autoCompleteBranch.Entries.Count > 0)
            {
                autoCompleteSection.Entries.Clear();
                autoCompleteSection.Enabled = true;
                autoCompleteUnit.Entries.Clear();
                autoCompleteUnit.Enabled = false;
            }
            GetCompanyName();
            GetDeptName();
            GetDivisionName();
            GetBranchName();
            GetSectionName();
            GetUnitName();
        }
        protected void autoCompleteBranch_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {

            if (autoCompleteBranch.Entries.Count == 0)
            {
                autoCompleteSection.Entries.Clear();
                autoCompleteSection.Enabled = false;
                autoCompleteUnit.Entries.Clear();
                autoCompleteUnit.Enabled = false;
            }
            GetCompanyName();
            GetDeptName();
            GetDivisionName();
            GetBranchName();
            GetSectionName();
            GetUnitName();

        }
        protected void autoCompleteSection_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            if (autoCompleteSection.Entries.Count > 0)
            {
                autoCompleteUnit.Entries.Clear();
                autoCompleteUnit.Enabled = true;
            }
            GetCompanyName();
            GetDeptName();
            GetDivisionName();
            GetBranchName();
            GetSectionName();
            GetUnitName();
        }
        protected void autoCompleteSection_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            if (autoCompleteSection.Entries.Count == 0)
            {
                autoCompleteUnit.Entries.Clear();
                autoCompleteUnit.Enabled = false;
            }
            GetCompanyName();
            GetDeptName();
            GetDivisionName();
            GetBranchName();
            GetSectionName();
            GetUnitName();
        }

        protected void autoCompleteUnit_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            GetCompanyName();
            GetDeptName();
            GetDivisionName();
            GetBranchName();
            GetSectionName();
            GetUnitName();
        }
        protected void autoCompleteUnit_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            GetCompanyName();
            GetDeptName();
            GetDivisionName();
            GetBranchName();
            GetSectionName();
            GetUnitName();
        }

        private void GetDeptName()
        {
            List<string> DepartmentName = new List<string>();
            if (autoCompleteDepartment.Entries.Count > 0)
            {
                foreach (AutoCompleteBoxEntry item in autoCompleteDepartment.Entries)
                {
                    DepartmentName.Add(item.Value);
                }
                Session["DepartmentName"] = string.Join(",", DepartmentName);
            }
            else
                Session["DepartmentName"] = null;
        }

        private void GetCompanyName()
        {
            List<string> CompanyNames = new List<string>();
            if (autoCompleteCompany.Entries.Count > 0)
            {
                foreach (AutoCompleteBoxEntry item in autoCompleteCompany.Entries)
                {
                    CompanyNames.Add(item.Value);
                }
                Session["CompanyName"] = string.Join(",", CompanyNames);
            }
            else
                Session["CompanyName"] = null;
        }

        private void GetBranchName()
        {
            List<string> BranchNames = new List<string>();
            if(autoCompleteDepartment.Entries.Count > 0)
            {
                autoCompleteBranch.Enabled = true;
            }
            else
            {
                autoCompleteBranch.Enabled = false;
            }
            if (autoCompleteBranch.Entries.Count > 0)
            {
                
                foreach (AutoCompleteBoxEntry item in autoCompleteBranch.Entries)
                {
                    BranchNames.Add(item.Value);
                }
                Session["BranchName"] = string.Join(",", BranchNames);
            }
            else
                Session["BranchName"] = null;
        }

        private void GetSectionName()
        {
            if(autoCompleteBranch.Entries.Count > 0)
            {
                autoCompleteSection.Enabled = true;
            }
            else
            {
                autoCompleteSection.Enabled = false;
            }
            List<string> SectionId = new List<string>();
            if(autoCompleteBranch.Entries.Count > 0)
            {
                autoCompleteSection.Enabled = true;
            }
            if (autoCompleteSection.Entries.Count > 0)
            {
                foreach (AutoCompleteBoxEntry item in autoCompleteSection.Entries)
                {
                    SectionId.Add(item.Value);
                }
                Session["SectionName"] = string.Join(",", SectionId);
            }
            else
                Session["SectionName"] = null;
        }

        private void GetUnitName()
        {
            if(autoCompleteSection.Entries.Count > 0)
            {
                autoCompleteUnit.Enabled = true;
            }
            else
            {
                autoCompleteUnit.Enabled = false;
            }
            List<string> UnitId = new List<string>();
            if (autoCompleteUnit.Entries.Count > 0)
            {
                foreach (AutoCompleteBoxEntry item in autoCompleteUnit.Entries)
                {
                    UnitId.Add(item.Value);
                }
                Session["Unitname"] = string.Join(",", UnitId);
            }
            else
                Session["Unitname"] = null;
        }

        private void GetDivisionName()
        {
            //List<string> DivisionName = new List<string>();
            //if (autoCompleteDivision.Entries.Count > 0)
            //{
            //    foreach (AutoCompleteBoxEntry item in autoCompleteDivision.Entries)
            //    {
            //        DivisionName.Add(item.Value);
            //    }
            //    Session["DivisionName"] = string.Join(",", DivisionName);
            //}
            //else
            //    Session["DivisionName"] = null;
        }

        protected void btnDirectSubordinates_Click(object sender, EventArgs e)
        {
            ReportType = 3;
            //HideRoColumn = true;
            ShowDirectSubordinate = true;
            BindDatasource();
        }

        protected void btnAllSubordinates_Click(object sender, EventArgs e)
        {
            ReportType = 4;
            //HideRoColumn = false;
            ShowDirectSubordinate = true;
            BindDatasource(true);
        }

        protected void UserRadTreeList_PreRender(object sender, EventArgs e)
        {
            //foreach (GridColumn col in UserRadTreeList.MasterTableView.RenderColumns)
            //{
            //    if (col is GridBoundColumn)
            //    {
            //        if (col != null && col.UniqueName.Equals("ReportingOfficer", StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            col.Visible = !HideRoColumn;
            //        }
            //    }
            //}
            UserRadTreeList.Rebind();
        }

       
    }
}