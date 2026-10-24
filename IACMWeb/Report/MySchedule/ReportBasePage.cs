using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Report.MySchedule
{
    static class EnumerableExtenstion
    {
        public static int Count(this IEnumerable data)
        {
            int total = 0;
            var e = data.GetEnumerator();
                while (e.MoveNext())
                    total++;
            return total;
        }
    }
    public class ReportBasePage : DI.Chakra.Web.UIBasePage
    {
        byte[] reportObj;
        long reportHistory_Id;
        long report_Id;
        public  string reportHistoryId = string.Empty;
        public string fileformat = string.Empty;
        public Dictionary<string, string> parameter = new Dictionary<string, string>();
        public Dictionary<string, string> attributeParameter = new Dictionary<string, string>();
        public Dto.Report report;
        public Dto.Report Reportitem;
        public Scheduler scheduler;
       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (reportHistoryId != null)
                {
                    long.TryParse(reportHistoryId, out reportHistory_Id);
                    report_Id = DomainApiClient<ReportHistory>.GetById(reportHistory_Id).ReportId;
                    fileformat = DomainApiClient<Dto.Report>.GetById(report_Id).Format;
                    parameter.Add("reportId", report_Id.ToString());
                    List<ReportAttributes> reportAttributes = DomainApiClient<ReportAttributes>.GetByAction<List<ReportAttributes>>(DomainApiAction.Reporting.GetReportAttributes, parameter);
                    parameter.Clear();
                    foreach (ReportAttributes attributes in reportAttributes)
                    {
                        parameter.Add(attributes.ParamKey, attributes.ParamValue);
                    }
                }
            }
        }



        public void ServiceExport(RadGrid gridControl,string fileType=".xls")
        {
            try
            {
                HttpContext.Current.Session["ExportStatus"] = "false";
                RadGrid grid = new RadGrid();
                grid = gridControl;
                int pageSize = 0;
                object obj = gridControl.DataSource;
                if (obj is IEnumerable)
                    pageSize = EnumerableExtenstion.Count((IEnumerable)obj);
                gridControl.PageSize = pageSize;
                gridControl.DataBind();
                if (gridControl.Items.Count > 0)
                {
                    gridControl.ExportSettings.IgnorePaging = true;
                    gridControl.AllowPaging = false;
                    gridControl.GridLines = GridLines.Both;
                    gridControl.AllowFilteringByColumn = false;
                    gridControl.ExportSettings.ExportOnlyData = true;
                    gridControl.ExportCellFormatting += new EventHandler<ExportCellFormattingEventArgs>(gridControl_ExportCellFormatting);
                    gridControl.GridExporting += GridControl_GridExporting;
                    
                    this.Page.Controls.Add(gridControl);
                    switch(fileType)
                    {
                        case ".xls":
                            gridControl.MasterTableView.ExportToExcel();
                            break;
                        case ".csv":
                            gridControl.MasterTableView.ExportToCSV();
                            break;
                        case ".doc":
                            gridControl.MasterTableView.ExportToWord();
                            break;
                        case ".pdf":
                            gridControl.MasterTableView.ExportToPdf();
                            break;
                        default:
                            gridControl.MasterTableView.ExportToExcel();
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                HttpContext.Current.Session["ExportStatus"] = "true";

            }
        }

        private void GridControl_GridExporting(object sender, GridExportingArgs e)
        {
            reportObj = Encoding.ASCII.GetBytes(e.ExportOutput);
            ReportHistory history = DomainApiClient<ReportHistory>.GetById(reportHistory_Id);
            if (!string.IsNullOrEmpty(reportHistoryId))
            {
                history.ReportId = report_Id;
                history.Id = reportHistory_Id;
                history.Report = reportObj;
                history.Status = ReportStatus.Created;
            }
            DomainApiClient<ReportHistory>.Save(history);
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

        public bool Save()
        {
            Application.SessionStatusImpl session = new Application.SessionStatusImpl();
            if (report.Format != null && scheduler != null )
            {
                report.Type = Infrastructure.Enums.ReportType.EndpointSystem;
                report.ReportUrl = Request.Url.AbsoluteUri;
                report.UserId = session.User.Id;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("reportName", report.Name);
                Reportitem = DomainApiClient<Dto.Report>.GetByAction<Dto.Report>
                    (DomainApiAction.Reporting.GetByReportName, parameter);
                if (Reportitem == null && report.Name!=string.Empty && report.MailTo != string.Empty)
                {
                    DomainApiClient<Dto.Report>.Save(report);
                    long ReportId = DomainApiClient<Dto.Report>.GetByAction<long>
                   (DomainApiAction.Reporting.GetReportId, parameter);
                    report.Process = new Process();
                    Process process = new Process();
                    process.ProcessTypeKeyId = ReportId;
                    process.Scheduler = scheduler;
                    process.Status = "Pending";
                    process.ProcessType = ProcessType.Reporting;
                    report.Process = process;
                    DomainApiClient<Process>.Save(process);

                    List<ReportAttributes> reportAttributes = new List<ReportAttributes>();
                    foreach(KeyValuePair<string,string> key in attributeParameter)
                    {
                        reportAttributes.Add(new ReportAttributes() {ParamKey=key.Key,ParamValue=key.Value,ReportId=ReportId });
                    }
                    DomainApiClient<ReportAttributes>.GetByAction<object>(DomainApiAction.Reporting.SaveReportAttributes, null, reportAttributes);
                    MasterPage.ShowAlert(AlertType.Success, "Report Scheduled");
                    return true;
                }
                else
                {
                    if (report.Name == string.Empty || report.MailTo == string.Empty)
                        MasterPage.ShowAlert(AlertType.Error, "Report Name and Mail To shouldn't be empty");
                    else
                    MasterPage.ShowAlert(AlertType.Error, "Report name already available");
                    return false;
                }
            }
            else
            {
                if (report.Format == null)
                    MasterPage.ShowAlert(AlertType.Error, "Please choose any format");
                else if (scheduler == null)
                    MasterPage.ShowAlert(AlertType.Error, "Please select Scheduler");
                else if(scheduler.ReccurEveryDays==null || scheduler.ReccurEveryMonths!=null || scheduler.ReccurEveryWeeks!=null)
                    MasterPage.ShowAlert(AlertType.Error, "Please select Scheduler occurence");
                return false;
            }
        }
    }
}