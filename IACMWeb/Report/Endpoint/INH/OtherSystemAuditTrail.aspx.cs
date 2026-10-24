using Application;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Core;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Report.MySchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Report.Endpoint.INH
{
    public partial class OtherSystemAuditTrail : ReportBasePage
    {
        private static List<Dto.PresentationAccountEntitlementAuditTrial> list = new List<Dto.PresentationAccountEntitlementAuditTrial>();
        private static int totalCount = 0;
        private SessionStatusImpl SessionUser = new SessionStatusImpl();
        
        private long EndPointSystemID
        {
            get
            {
                if (ViewState["EndPointSystemID"] != null)
                {
                    return (long)ViewState["EndPointSystemID"];
                }
                return 0;
            }
            set
            {
                ViewState["EndPointSystemID"] = value;
            }
        }
        protected new void Page_Load(object sender, EventArgs e)
        {
            grdUser.ExportSettings.Pdf.PageHeader.MiddleCell.Text = "Other System Audit Trial";
            grdUser.ExportSettings.Pdf.PageHeader.MiddleCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
            grdUser.ExportSettings.Pdf.PageHeader.LeftCell.Text = PDFFileLeftCell;
            grdUser.ExportSettings.Pdf.PageHeader.LeftCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                LoadInhouseApplications();
            }
        }
        protected void Users_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            LoadResult();
        }

        private void LoadResult()
        {
            int PageIndex = grdUser.CurrentPageIndex;
            int PageSize = grdUser.MasterTableView.PageSize;
            string sortingFieldName = "";
            string sortOrder = "";
            string sortExpressions = string.Empty;
            if (grdUser.MasterTableView.SortExpressions.Count > 0)
            {
                sortingFieldName = grdUser.MasterTableView.SortExpressions[0].FieldName;
                sortOrder = grdUser.MasterTableView.SortExpressions[0].SortOrderAsString();
                foreach (GridSortExpression item in grdUser.MasterTableView.SortExpressions)
                {
                    sortExpressions += string.Format("{0} {1},", item.FieldName, item.SortOrderAsString());
                }
                sortExpressions = sortExpressions.Trim(',').Trim();
            }

            var filterExpression = grdUser.MasterTableView.FilterExpression;

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

            SessionStatusImpl session = new SessionStatusImpl();
            int startRowIndex = PageIndex * PageSize;
            List<long> ids = new List<long>();
            if(ddlInhouseSystems.SelectedValue != "ALL")
            {
                ids.Add(Convert.ToInt64(ddlInhouseSystems.SelectedValue));
            }

            PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter
            {
                FullTimeEmp = false,
                StartRowIndex = startRowIndex,
                MaximumRows = PageSize,
                FilterExpression = filterExpression,
                SortExpression = sortExpressions.Trim(',').Trim(),
                Ids = ids
            };
            if (DatePickerValidFrom.SelectedDate.HasValue)
            {
                param.FromDate = Convert.ToDateTime(DatePickerValidFrom.SelectedDate.Value.ToString("dd-MMM-yyyy 00:00:00"));
            }

            if (DatePickerValidTo.SelectedDate.HasValue)
            {
                param.ToDate = Convert.ToDateTime(DatePickerValidTo.SelectedDate.Value.ToString("dd-MMM-yyyy 23:59:59"));
            }
            var usrList = DomainApiClient<Dto.InhouseUserInfo>
             .GetByAction<Dto.Core.PresentationGridPagedData<Dto.PresentationAccountEntitlementAuditTrial>>(DomainApiAction.Endpoint.Presentation.PresentationUser.GetLazyLoadAuditTrial, null, param);
            grdUser.VirtualItemCount = (int)usrList.TotalCount;
            totalCount = grdUser.VirtualItemCount;
            grdUser.DataSource = usrList.Data;
        }

        private void LoadInhouseApplications()
        {
            var dictEndpoints = new Dictionary<string, string>();
            dictEndpoints.Add("ALL", "ALL");
            var endpoints = DomainApiClient<Dto.EndPoint>.GetByAction<Dictionary<long, string>>
                   ("GetInhouseEndPointsAutomateSystem", null);
            foreach (var ep in endpoints)
            {
                dictEndpoints.Add(ep.Key.ToString(), ep.Value);
            }
            ddlInhouseSystems.DataSource = dictEndpoints;
            ddlInhouseSystems.DataTextField = "Value";
            ddlInhouseSystems.DataValueField = "Key";
            ddlInhouseSystems.DataBind();
        }
        protected override Dto.IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("EndpointName", "EndpointName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("DisplayName", "DisplayName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("UserName", "UserName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("AccountType", "AccountType", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Permission", "Permission", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Remarks", "Remarks", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("CreatedDate", "CreatedDate", "dd-MM-yyyy hh:mm:ss"));
            return param;
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var fileType = (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.ExcelAll) ? "xlsx" : "csv";

                string sortingFieldName = "";
                string sortOrder = "";
                string sortExpressions = string.Empty;
                if (grdUser.MasterTableView.SortExpressions.Count > 0)
                {
                    sortingFieldName = grdUser.MasterTableView.SortExpressions[0].FieldName;
                    sortOrder = grdUser.MasterTableView.SortExpressions[0].SortOrderAsString();
                    foreach (GridSortExpression item in grdUser.MasterTableView.SortExpressions)
                    {
                        sortExpressions += string.Format("{0} {1},", item.FieldName, item.SortOrderAsString());
                    }
                    sortExpressions = sortExpressions.Trim(',').Trim();
                }

                var filterExpression = grdUser.MasterTableView.FilterExpression;

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

                SessionStatusImpl session = new SessionStatusImpl();
                List<long> ids = new List<long>();
                if (ddlInhouseSystems.SelectedValue != "ALL")
                {
                    ids.Add(Convert.ToInt64(ddlInhouseSystems.SelectedValue));
                }

                PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter
                {
                    FullTimeEmp = false,
                    StartRowIndex = 0,
                    MaximumRows = -1,
                    FilterExpression = filterExpression,
                    SortExpression = sortExpressions.Trim(',').Trim(),
                    Ids = ids
                };

                if (DatePickerValidFrom.SelectedDate.HasValue)
                {
                    param.FromDate = Convert.ToDateTime(DatePickerValidFrom.SelectedDate.Value.ToString("dd-MMM-yyyy 00:00:00"));
                }

                if (DatePickerValidTo.SelectedDate.HasValue)
                {
                    param.ToDate = Convert.ToDateTime(DatePickerValidTo.SelectedDate.Value.ToString("dd-MMM-yyyy 23:59:59"));
                }

                var parameter = new Dictionary<string, string>();
                parameter.Add("exportFileType", fileType);
                var data = DomainApiClient<Dto.InhouseUserInfo>
                 .GetByAction<byte[]>(DomainApiAction.Endpoint.Presentation.PresentationUser.DownloadOtherSystemAuditTrial, parameter, param);

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
                    response.AddHeader("Content-Disposition", "attachment;filename=" + string.Format("OtherSystemAuditTrial.{0}", fileType));
                    response.BinaryWrite(data);
                    response.End();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
        }
        protected string GetFilterExpression(string filterExpression)
        {
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
                return filterExpression;
            }
            return filterExpression;
        }
        protected void ddlInhouseSystems_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdUser.Rebind();
        }

        protected void DatePickerValidFrom_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
        {
            DatePickerValidTo.Enabled = true;
        }

        protected void Filter_Click(object sender, EventArgs e)
        {
            grdUser.Rebind();
        }

        protected void ClearFilter_Click(object sender, EventArgs e)
        {
            DatePickerValidTo.SelectedDate = null;
            DatePickerValidFrom.SelectedDate = null;
        }
    }
}