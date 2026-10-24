using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Web.Master;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;


namespace DI.Chakra.Web
{
    public class ContextObject
    {
        public int ObjectId { get; set; }
        public TabMenu Menu { get; set; }
    }


    public class UIBasePage : Page//, IAccessControl
    {
        AppTextSource appTextSource = null;
        SessionStatusImpl sessionStatus = new SessionStatusImpl();

        public UIBasePage()
        {
            //AppTextLocation.Path = Server.MapPath("~/assets/apptexts/AppTexts.xml");
            appTextSource = AppTextSource.Instance;
        }

        public string T(string key)
        {
            return appTextSource.GetString(key);
        }
        public long? ContextObjectId
        {
            get
            {
                if (ViewState["CONTEXTOBJECTID"] == null)
                    return null;

                return (long)ViewState["CONTEXTOBJECTID"];
            }
            set
            {
                ViewState["CONTEXTOBJECTID"] = value;
            }
        }

        public string TypeofRequest
        {
            get
            {
                if (ViewState["TypeofRequest"] == null)
                    return "";

                return (string)ViewState["TypeofRequest"];
            }
            set
            {
                ViewState["TypeofRequest"] = value;
            }
        }

        public int MenuId
        {
            get
            {
                if (ViewState["MenuIDD"] != null)
                {
                    return (int)ViewState["MenuIDD"];
                }
                return -1;
            }
            set
            {
                ViewState["MenuIDD"] = value;
            }
        }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);

            var npage = this.Page as IContextObjectEnabled;
            if (npage != null)
            {
                if (npage.ContextObjectId != null)
                {
                    this.MasterPage.ContextObjectId = npage.ContextObjectId;
                }
                if (npage.TypeofRequest != null)
                {
                    this.MasterPage.TypeofRequest = npage.TypeofRequest;
                }

            }

            var cpage = this.Page as IContextObjectRequired;
            if (cpage != null)
            {
                if (Request.QueryString["contextId"] != null)
                    cpage.ContextObjectId = Convert.ToInt64(Request.QueryString["contextId"].ToString());

                if (Request.QueryString["TypeofRequest"] != null)
                    cpage.TypeofRequest = Request.QueryString["TypeofRequest"].ToString();
            }

            //var prevPage = this.PreviousPage as IContextObjectEnabled;

            //if (prevPage != null)
            //{
            //    var page = this.Page as IContextObjectRequired;
            //    if (page != null)
            //    {
            //        if (prevPage.ContextObjectId == null)
            //        {
            //            sessionStatus.ReturnErrorMessage = "Please select an item.";
            //            Response.Redirect(this.PreviousPage.AppRelativeVirtualPath, true);
            //        }
            //        else
            //        {
            //            page.ContextObjectId = prevPage.ContextObjectId.Value;
            //        }

            //        if (prevPage.TypeofRequest == null)
            //        {
            //            sessionStatus.ReturnErrorMessage = "Please select an item.";
            //            Response.Redirect(this.PreviousPage.AppRelativeVirtualPath, true);
            //        }
            //        else
            //        {
            //            page.TypeofRequest = prevPage.TypeofRequest;
            //        }
            //    }
            //    page = null;
            //}
            //prevPage = null;
        }

        public ProtoMast4 MasterPage
        {

            get
            {
                return (ProtoMast4)Page.Master;
            }
        }

        public string PDFFileLeftCell
        {

            get { return string.Empty; }

        }

        [WebMethod]
        public static void RemoveUserFromCacheOnExit()
        {
            SessionStatusImpl session = new SessionStatusImpl();
            if (session != null && session.User != null)
            {
                string usrName = string.IsNullOrEmpty(session.User.UserName) ? string.Empty : session.User.UserName.Trim().ToUpper();
                string key = usrName;
                if (HttpRuntime.Cache[key] != null)
                {
                    HttpRuntime.Cache.Remove(key);
                }
            }
        }




        #region private Methods

        private static string getIP4Address()
        {
            string IP4Address = String.Empty;

            if (!(HttpContext.Current != null && HttpContext.Current.Request != null))
            {
                return IP4Address;
            }

            foreach (IPAddress IPA in Dns.GetHostAddresses(HttpContext.Current.Request.UserHostAddress))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            if (IP4Address != String.Empty)
            {
                return IP4Address;
            }

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }
        #endregion

        public EndpointType EndpointType
        {
            get
            {
                if (Session["CONTEXTOBJECTENDPOINTTYPE"] == null)
                    return EndpointType.None;

                return (EndpointType)Session["CONTEXTOBJECTENDPOINTTYPE"];
            }
            set
            {
                Session["CONTEXTOBJECTENDPOINTTYPE"] = value;
            }
        }

        public string EntitlementTypeValue
        {
            get
            {
                if (Session["CONTEXTOBJECTENTITLEMENTTYPE"] == null)
                    return "None";

                return (string)Session["CONTEXTOBJECTENTITLEMENTTYPE"];
            }
            set
            {
                Session["CONTEXTOBJECTENTITLEMENTTYPE"] = value;
            }
        }
        public void ExportNew(RadGrid gridControlNew, ACLAction actionType, string fileName)
        {
            try
            {
                HttpContext.Current.Session["ExportStatus"] = "false";
                if (gridControlNew.Items.Count > 0)
                {
                    if (actionType == ACLAction.ExcelAll || actionType == ACLAction.CSVAll)
                    {
                        foreach (GridColumn column in gridControlNew.MasterTableView.Columns)
                        {
                            column.Display = true;
                        }
                    }

                    foreach (GridColumn column in gridControlNew.MasterTableView.Columns)
                    {
                        if (column is GridBoundColumn && column.UniqueName.ToLower().Contains("date"))
                        {
                            ((GridBoundColumn)column).DataType = Type.GetType("System.DateTime");
                            ((GridBoundColumn)column).DataFormatString = "{0:dd/MM/yyyy}";
                        }
                        if (column is GridDateTimeColumn && column.UniqueName.Equals("LogDate",StringComparison.InvariantCultureIgnoreCase))
                        {
                            ((GridDateTimeColumn)column).DataType = Type.GetType("System.DateTime");
                            ((GridDateTimeColumn)column).DataFormatString = "{0:dd-MM-yyyy hh:mm:ss tt}";
                        }
                        if (column is GridBoundColumn && column.UniqueName.ToLower().Equals("LastLogonDate".ToLower()))
                        {
                            ((GridBoundColumn)column).DataType = Type.GetType("System.DateTime");
                            ((GridBoundColumn)column).DataFormatString = "{0:dd-MM-yyyy hh:mm:ss tt}";
                        }
                    }

                    //gridControlNew.ExportCellFormatting += new EventHandler<ExportCellFormattingEventArgs>(gridControlNew_ExportCellFormatting);
                    //gridControlNew.ExportSettings.SuppressColumnDataFormatStrings = true;
                    gridControlNew.ExportSettings.HideStructureColumns = true;
                    gridControlNew.ExportSettings.FileName = fileName;
                    gridControlNew.ExportSettings.OpenInNewWindow = true;
                    gridControlNew.ExportSettings.Excel.Format = GridExcelExportFormat.ExcelML;
                    gridControlNew.ExportSettings.IgnorePaging = true;
                    gridControlNew.ExportSettings.ExportOnlyData = true;
                    gridControlNew.GridLines = GridLines.Both;
                    gridControlNew.AllowFilteringByColumn = true;
                    //gridControlNew.InfrastructureExporting += GridControlNew_InfrastructureExporting;
                    if (actionType == ACLAction.CSV || actionType == ACLAction.CSVAll)
                    {
                        gridControlNew.ExportSettings.ExportOnlyData = false;
                        gridControlNew.MasterTableView.ExportToCSV();
                    }
                    else if (actionType == ACLAction.Excel || actionType == ACLAction.ExcelAll)
                    {
                        gridControlNew.ExportSettings.ExportOnlyData = true;
                        gridControlNew.MasterTableView.ExportToExcel();
                    }
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "No data to export");
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                HttpContext.Current.Session["ExportStatus"] = "true";
            }
        }
        public void Export(RadGrid gridControl, ACLAction actionType)
        {
            try
            {
                HttpContext.Current.Session["ExportStatus"] = "false";
                if (gridControl.Items.Count > 0)
                {
                    gridControl.ExportSettings.IgnorePaging = true;
                    gridControl.GridLines = GridLines.Both;
                    gridControl.AllowFilteringByColumn = false;
                    gridControl.ExportCellFormatting += new EventHandler<ExportCellFormattingEventArgs>(gridControl_ExportCellFormatting);
                    if (actionType == ACLAction.CSV || actionType == ACLAction.CSVAll)
                    {
                        gridControl.ExportSettings.ExportOnlyData = false;
                        gridControl.MasterTableView.ExportToCSV();
                    }
                    else if (actionType == ACLAction.Excel || actionType == ACLAction.ExcelAll)
                    {
                        gridControl.ExportSettings.ExportOnlyData = true;
                        gridControl.MasterTableView.ExportToExcel();
                    }
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "No data to export");
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                HttpContext.Current.Session["ExportStatus"] = "true";
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

        protected void WriteInfoLog(string message)
        {
            DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, message);
        }
        protected virtual IDMSReportParam GetReportHeaderColumnMapping()
        {
            return new IDMSReportParam();
        }
        public void GenericExport(dynamic reportData, ACLAction actionType, string reportName = "Report")
        {
            if (string.IsNullOrWhiteSpace(reportName)) reportName = "Report";
            Session["ExportStatus"] = "false";
            var exportFileType = (actionType == ACLAction.Excel || actionType == ACLAction.ExcelAll) ? "xlsx" : "csv";
            byte[] streamData = null;
            IDMSReportParam param = GetReportHeaderColumnMapping();
            if (param != null && param.ReportDataColumns != null && param.ReportDataColumns.Count > 0)
            {
                var reportSheetName = reportName;
                var temp = HttpContext.Current.Request.PhysicalApplicationPath + "\\temp\\";
                string filePath = string.Empty;
                try
                {
                    if (!System.IO.Directory.Exists(temp)) System.IO.Directory.CreateDirectory(temp);
                    filePath = string.Format("{0}\\{1}_{2}.{3}", temp, reportSheetName, DateTime.Now.ToString(Constants.Report_Date_Format), exportFileType);
                    if (exportFileType.ToUpper() == "XLSX" || exportFileType.ToUpper() == "XLS")
                    {

                        using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        {
                            using (var objExcelPackage = new ExcelPackage(stream))
                            {
                                int currentRow = 1;
                                int colStartPosition = 1;
                                int colNum = colStartPosition;
                                ExcelWorksheet workSheet = objExcelPackage.Workbook.Worksheets.Add(reportSheetName);
                                workSheet.Row(1).Height = 20;
                                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                workSheet.Row(1).Style.Font.Bold = true;
                                foreach (var item in param.ReportDataColumns)
                                {
                                    workSheet.Cells[currentRow, colNum].Value = item.Header;
                                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                    colNum++;
                                }
                                currentRow++;

                                foreach (var data in reportData)
                                {

                                    colStartPosition = 1;
                                    colNum = colStartPosition;
                                    foreach (var item in param.ReportDataColumns)
                                    {
                                        workSheet.Cells[currentRow, colNum].Value = data.GetReportPropertyValue(item.DataFieldName, item.Format);
                                        colNum++;
                                    }
                                    currentRow++;
                                }
                                workSheet.Cells.AutoFitColumns();
                                objExcelPackage.Workbook.Properties.Title = reportName;
                                streamData = objExcelPackage.GetAsByteArray();
                            }
                        }
                    }
                    else if (exportFileType.ToUpper() == "CSV")
                    {
                        using (StreamWriter stream = new StreamWriter(filePath))
                        {
                            StringBuilder objWriter = new StringBuilder();
                            StringBuilder objColumn = new StringBuilder();
                            stream.WriteLine(string.Join(",", param.ReportDataColumns.Select(i => string.Format("\"{0}\"", i.Header))));
                            foreach (var data in reportData)
                            {
                                objColumn = new StringBuilder();
                                stream.WriteLine(string.Join(",", param.ReportDataColumns.Select(item => string.Format("\"\t{0}\"", data.GetReportPropertyValue(item.DataFieldName, item.Format)))));
                            }
                        }
                        streamData = File.ReadAllBytes(filePath);
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            if (streamData != null)
            {
                Session["ExportStatus"] = "true";
                HttpResponse response = Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                response.AddHeader("Content-Length", streamData.Length.ToString());
                response.AddHeader("Content-Disposition", "attachment;filename=" + string.Format("{0}{2}.{1}", reportName, exportFileType, DateTime.Now.ToString(Constants.Report_Date_Format)));
                response.BinaryWrite(streamData);
                response.End();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }
        protected string GetSortExpression(GridSortExpressionCollection sortExpressions)
        {
            var resultSortExpress = string.Empty;
            if (sortExpressions.Count == 0)
                return resultSortExpress;
            var sortingFieldName = sortExpressions[0].FieldName;
            var sortOrder = sortExpressions[0].SortOrderAsString();
            foreach (GridSortExpression item in sortExpressions)
            {
                resultSortExpress += string.Format("{0} {1},", item.FieldName, item.SortOrderAsString());
            }
            return resultSortExpress.Trim(',').Trim();
        }
    }
}