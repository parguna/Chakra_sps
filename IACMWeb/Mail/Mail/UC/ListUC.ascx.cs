using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using Application;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using DI.Chakra.Dto.Core;
using DI.Chakra.DBConfigHelper;

namespace DI.Chakra.Web.Mail.Mail.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        #region Members
        private string strActivityType, strStatus = string.Empty;
        private const string V_Status = "RecordStatus";
        private static List<Dto.Presentation.PresentationMail> list;
        #endregion

        public string RecordStatus
        {
            get
            {
                if (ViewState[V_Status] == null)
                    return string.Empty;
                return (string)ViewState[V_Status];
            }
            set
            {
                ViewState[V_Status] = value;

            }
        }

        #region Methods
        public void BindData()
        {
            strActivityType = ddlActivityType.SelectedItem.Text;
            strStatus = ddlStatus.SelectedItem.Text;

            int PageIndex = gvEmailViewer.CurrentPageIndex;
            int PageSize = gvEmailViewer.MasterTableView.PageSize;
            string sortingFieldName = "";
            string sortOrder = "";
            string sortExpressions = string.Empty;
            if (gvEmailViewer.MasterTableView.SortExpressions.Count > 0)
            {
                sortingFieldName = gvEmailViewer.MasterTableView.SortExpressions[0].FieldName;
                sortOrder = gvEmailViewer.MasterTableView.SortExpressions[0].SortOrderAsString();
                foreach (GridSortExpression item in gvEmailViewer.MasterTableView.SortExpressions)
                {
                    sortExpressions += string.Format("{0} {1},", item.FieldName, item.SortOrderAsString());
                }
                sortExpressions = sortExpressions.Trim(',').Trim();
            }

            var filterExpression = gvEmailViewer.MasterTableView.FilterExpression;

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
            DateTime? fromDate=null,toDate = null;
            if (DatePickerValidFrom.SelectedDate.HasValue)
            {
                fromDate = Convert.ToDateTime($"{DatePickerValidFrom.SelectedDate.Value} 00:00:00");
            }
            if (DatePickerValidTo.SelectedDate.HasValue)
            {
                toDate = Convert.ToDateTime($"{DatePickerValidTo.SelectedDate.Value} 23:59:59");
            }
            int startRowIndex = PageIndex * PageSize;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("activityType", strActivityType);
            parameter.Add("status", strStatus);
            PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter
            {
                FullTimeEmp = true,
                StartRowIndex = startRowIndex,
                MaximumRows = PageSize,
                FilterExpression = filterExpression,
                SortExpression = sortExpressions.Trim(',').Trim(),
                FromDate = fromDate,
                ToDate = toDate
            };
            var response = DomainApiClient<Dto.Presentation.PresentationMail>.GetByAction<Dto.Core.PresentationGridPagedData<Dto.Presentation.PresentationMail>>
            (DomainApiAction.Email.PresentationMail.GetPagedDataByActivityType, parameter, param);
            gvEmailViewer.VirtualItemCount = response.TotalCount;
            gvEmailViewer.DataSource = response.Data;
            var queryString = string.Empty;
            if (!string.IsNullOrWhiteSpace(strActivityType))
            {
                queryString = $"&activityType={strActivityType}";
            }
            else
            {
                queryString = $"&activityType=All";
            }

            if (!string.IsNullOrWhiteSpace(strStatus))
            {
                queryString += $"&Status={strStatus}";
            }
            else
            {
                queryString += $"&Status=All";
            }
            queryString += $"&fromDate={HttpUtility.HtmlEncode(DatePickerValidFrom.SelectedDate.Value.ToString("dd-MMM-yyyy"))}&toDate={HttpUtility.HtmlEncode(DatePickerValidTo.SelectedDate.Value.ToString("dd-MMM-yyyy"))}";
            this.MasterPage.AdditionalQueryString = queryString;
        }

        private void delete()
        {
            if (!ContextObjectId.HasValue)
                return;

            if(RecordStatus.ToUpper()!= "SENT")
            {
                DomainApiClient<Dto.EmailInfo>.DeleteById(this.ContextObjectId.Value);
            }
            else
            {
                DomainApiClient<Dto.EmailInfoHistory>.DeleteById(this.ContextObjectId.Value);
            }
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "Email deleted successfully");
            gvEmailViewer.Rebind();           
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                DatePickerValidTo.MaxDate = DateTime.Now.Date;
                DatePickerValidTo.SelectedDate = DateTime.Now.Date;
                DatePickerValidFrom.SelectedDate = DateTime.Now.Date.AddDays(-30);
                DatePickerValidFrom.MinDate = DateTime.Now.Date.AddDays((-MasterConfigHelper.DateFilter));
                if (!string.IsNullOrWhiteSpace(this.Request.QueryString["Status"]) && !string.IsNullOrWhiteSpace(this.Request.QueryString["activityType"]) && !string.IsNullOrWhiteSpace(this.Request.QueryString["fromDate"]) && !string.IsNullOrWhiteSpace(this.Request.QueryString["toDate"]))
                {
                    foreach (ListItem item in ddlActivityType.Items)
                    {
                        if (item.Value.Equals(this.Request.QueryString["activityType"]))
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                    foreach (ListItem item in ddlStatus.Items)
                    {
                        if (item.Value.Equals(this.Request.QueryString["Status"]))
                        {
                            item.Selected = true;
                            break;
                        }
                    }

                    DatePickerValidFrom.SelectedDate = DateTime.Parse(this.Request.QueryString["fromDate"]);
                    DatePickerValidTo.SelectedDate= DateTime.Parse(this.Request.QueryString["toDate"]);
                }
            }
            this.MasterPage.ActionClicked += ActionClicked;
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if(!DatePickerValidFrom.SelectedDate.HasValue || !DatePickerValidTo.SelectedDate.HasValue)
            {
                ShowAlertMessage(AlertType.Error, "Please select From and To Date");
                return;
            }
            gvEmailViewer.Rebind();
        }
        protected void gvEmailViewer_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindData();
        }
        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Mail - Delete");
                delete();
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll || e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                ExportAll(e.ActionType);
            }
        }

        private void ExportAll(ACLAction actionType)
        {
            if (actionType == ACLAction.Excel || actionType == ACLAction.CSV || actionType == ACLAction.ExcelAll || actionType == ACLAction.CSVAll)
            {
                PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter
                {
                    FullTimeEmp = true,
                    StartRowIndex = 0,
                    MaximumRows = -1,
                    SortExpression = null,
                    FilterExpression = null,
                    FromDate = null,
                    ToDate = null
                };
                var sActivityType = "All";
                var sStatus = "All";

                if (chkExportAll.Checked == false)
                {
                    DateTime? fromDate = null, toDate = null;
                    if (DatePickerValidFrom.SelectedDate.HasValue)
                    {
                        fromDate = Convert.ToDateTime($"{DatePickerValidFrom.SelectedDate.Value} 00:00:00");
                    }
                    if (DatePickerValidTo.SelectedDate.HasValue)
                    {
                        toDate = Convert.ToDateTime($"{DatePickerValidTo.SelectedDate.Value} 23:59:59");
                    }
                    sActivityType = ddlActivityType.SelectedItem.Text;
                    sStatus = ddlStatus.SelectedItem.Text;

                    //param.StartRowIndex = gvEmailViewer.CurrentPageIndex;
                    //param.MaximumRows = gvEmailViewer.PageSize;

                    param.FilterExpression = GetFilterExpression(gvEmailViewer.MasterTableView.FilterExpression);
                    param.SortExpression = GetSortExpression(gvEmailViewer.MasterTableView.SortExpressions);
                    param.FromDate = fromDate;
                    param.ToDate = toDate;
                }

                Session["ExportStatus"] = "false";
                var fileType = (actionType == ACLAction.Excel || actionType == ACLAction.ExcelAll) ? "xlsx" : "csv";
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("activityType", sActivityType);
                parameter.Add("status", sStatus);
                parameter.Add("exportFileType", fileType);

                var data = DomainApiClient<Dto.Presentation.PresentationMail>.GetByAction<byte[]>(DomainApiAction.Email.PresentationMail.DownloadEmail, parameter, param);
                if (data != null)
                {
                    Session["ExportStatus"] = "true";
                    HttpResponse response = Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.ContentType = (actionType == ACLAction.CSV || actionType == ACLAction.CSVAll )?  "text/csv":"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    response.AddHeader("Content-Length", data.Length.ToString());
                    response.AddHeader("Content-Disposition", "attachment;filename=" + string.Format("Email.{0}", fileType));
                    response.BinaryWrite(data);
                    response.End();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
        }

        protected void gvEmailViewer_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = gvEmailViewer.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
            ViewState[V_Status] = g["Status"].Text;
            if (this.RecordStatus != null && this.RecordStatus.ToUpper() == "SENT")
                Session["MAIL_STATUS"] = this.RecordStatus;
            else
                Session["MAIL_STATUS"] = string.Empty;
        }
        #endregion

        protected void btncancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
    }
}
