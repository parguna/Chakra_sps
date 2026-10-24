using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Web.CustomControl;
using DI.Chakra.Web.Master.UC;
using DI.Chakra.Web.Report.MySchedule;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Report.CustomReport
{
    public partial class CustomReportViewer : ReportBasePage
    {
        private static DataTable list = new DataTable();
        private static int totalCount = 0;
        static string pattern = @"\[(.*?)\]";
        static Dictionary<string, string> Operators = new Dictionary<string, string>()
        {
            {"EqualTo","= {0}" },
            {"NotEqual", "<> {0}" },
            {"GreaterThan",">  {0}" },
            {"LessThan","<  {0}"},
            {"In","in ({0})"},
            {"Like","like '%{0}%' " }
        };
        const string V_CURRENTREPORT = "V_CURRENTREPORT";
        const string V_CUSTOMREPORT = "V_CUSTOMREPORT";
        const string V_UPDATEQUERY = "V_UPDATEQUERY";
        bool _filterControlsAdded = false;
        private long ReportId
        {
            get
            {
                if (ViewState["ReportId"] != null)
                {
                    return Convert.ToInt64(ViewState["ReportId"]);
                }
                return 0;
            }
            set
            {
                ViewState["ReportId"] = value;
            }
        }
        private string MultiEndpoint
        {
            get
            {
                if (ViewState["MultiEndpoint"] != null)
                {
                    return Convert.ToString(ViewState["MultiEndpoint"]);
                }
                return "";
            }
            set
            {
                ViewState["MultiEndpoint"] = value;
            }
        }
        private bool FilterControlsAdded
        {
            get
            {
                return _filterControlsAdded;
            }
            set
            {
                _filterControlsAdded = value;
            }
        }

        private string UpdateQuery
        {
            get
            {
                if (ViewState[V_UPDATEQUERY] != null)
                {
                    return ViewState[V_UPDATEQUERY].ToString();
                }
                return "";
            }
            set
            {
                ViewState[V_UPDATEQUERY] = value;
            }
        }

        static bool IsQueryParameterized
        {
            get;
            set;
        }

        private Dto.CustomReport CurrentReport
        {
            get
            {
                if (ViewState[V_CUSTOMREPORT] != null)
                {
                    return (Dto.CustomReport)ViewState[V_CUSTOMREPORT];
                }
                return new Dto.CustomReport();
            }
            set
            {
                ViewState[V_CUSTOMREPORT] = value;
            }
        }

        static bool WindowVisible { get; set; }
        protected void Page_PreInit(object sender, EventArgs e)
        {

            if (Request.QueryString["reportId"] != null)
            {
                ReportId = Convert.ToInt64(Request.QueryString["reportId"]);
            }
            if(ReportId == 0)
            {
                string reportId = HttpUtility.ParseQueryString(HttpUtility.UrlDecode(QueryStringEncyptorModule.GetActualUrl(Request.UrlReferrer.Query)))[0];
                ReportId = Convert.ToInt64(reportId);
            }
            if (Request.QueryString["MEndpoint"] != null)
            {
                MultiEndpoint = Request.QueryString["MEndpoint"];
            }
            reportHistoryId = Request["reportHistoryId"];
            CurrentReport = getCurrentReport(ReportId);
            var customReport = CurrentReport;
            //baseCustomReport = CurrentReport;
            bool hasAccess = false;
            MasterPage.PreferredPageTitle = CurrentReport.Name;
            if (customReport != null && ReportId > 0 && customReport.IsLinkedToMenu && customReport.PageId.HasValue)
            {

                List<Dto.PagePermission> pagePermissions = (List<Dto.PagePermission>)Session["PagePermissions"];
                hasAccess = pagePermissions.Any(x => x.PageId == customReport.PageId.Value);

                if (hasAccess)
                {
                    var permission = pagePermissions.Any(x => x.PageId == customReport.PageId.Value && x.PermissionId == 12);

                    if (!permission)
                    {
                        AccessPanel control = Page.Master.FindControl("AccessPanel") as AccessPanel;
                        if (control != null)
                        {
                            control.Visible = false;
                        }

                    }

                    if (reportHistoryId != null)
                    {
                        try
                        {
                            base.Page_Load(sender, e);

                            Dictionary<string, string> parameter = new Dictionary<string, string>();
                            parameter.Add("reportId", ReportId.ToString());
                            parameter.Add("startRowIndex", 0.ToString());
                            parameter.Add("maximumRows", 0.ToString());
                            parameter.Add("FilterExpression", "");
                            parameter.Add("sortExpression", "");
                            //TODO: Need clarification
                            list = DomainApiClient<Dto.CustomReport>.
                                   GetByAction<DataTable>(DomainApiAction.CustomReport.GetReportViewerData, parameter, "");

                            RadReportJobs.DataSource = list;
                            ServiceExport(RadReportJobs, fileformat);
                        }
                        catch (Exception ex)
                        { }
                    }

                    if (customReport.CustomReportParameters != null && customReport.CustomReportParameters.Count > 0
                        && customReport.CustomReportParameters.Any(a => !a.IsDeleted))
                    {
                        divSearch.Visible = true;
                        ShowFilter();
                        FilterControlsAdded = true;
                    }
                    else
                    {
                        divSearch.Visible = false;
                        //imgShowFilter.Visible = false;
                    }
                }
            }

            if (!hasAccess)
            {
                Response.Redirect("~/AccessDenied.aspx");
            }
        }
        protected new void Page_Load(object sender, EventArgs e)
        {
            MasterPage.ActionClicked += ActionClicked;

            
            if (!IsPostBack)
            {

                
            }
        }


        void setControl(CustomReportParameter parameter)//, bool isLastParameter)
        {
            if (parameter == null) return;
            tblParameter.ClientIDMode = ClientIDMode.Static;

            var id = parameter.ParameterName.Replace(" ", "").ToLower();
            id = id.Replace(".", "_");
            HtmlGenericControl tr = new HtmlGenericControl("tr");

            HtmlGenericControl td0 = new HtmlGenericControl("td");
            td0.InnerText = id;
            td0.Style.Add("display", "none");

            tr.Controls.Add(td0);
            HtmlGenericControl td01 = new HtmlGenericControl("td");
            td01.InnerText = parameter.Description;
            tr.Controls.Add(td01);
            HtmlGenericControl td1 = new HtmlGenericControl("td");


            RadComboBox cmbBox = new RadComboBox() { ID = $"op_{id}", ClientIDMode = ClientIDMode.Static, CssClass = "operand" };
            cmbBox.Width = 75;

            if (parameter.ParameterVariant == CustomReportParameterVariant.Value)
            {
                Operators.Keys.ToList().ForEach(key =>
                {
                    if (key == "EqualTo")
                        cmbBox.Items.Add(new RadComboBoxItem(key, Operators[key]));
                });
            }
            else
            {
                Operators.Keys.ToList().ForEach(key =>
                {
                    cmbBox.Items.Add(new RadComboBoxItem(key, Operators[key]));
                });
            }
            td1.Controls.Add(cmbBox);
            tr.Controls.Add(td1);

            HtmlGenericControl td2 = new HtmlGenericControl("td");

            switch (parameter.ParameterType)
            {
                case CustomReportParameterTypes.EndpointSystem:
                    {
                        var cmbBoxValue = new RadComboBox() { ID = id, ClientIDMode = ClientIDMode.Static, CssClass = "endpointSystem" };
                        cmbBoxValue.Width = 250;
                        var endPoints = new List<Dto.EndPoint>();

                        if (!string.IsNullOrWhiteSpace(parameter.DataFormatString))
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            dic.Add("id", parameter.DataFormatString);
                            endPoints = DomainApiClient<Dto.EndPoint>.GetByAction<List<Dto.EndPoint>>(DomainApiAction.Endpoint.EndPoint.GetByEndpointSystemTypeId, dic).OrderBy(x => x.DisplayName).ToList();
                            if (!string.IsNullOrWhiteSpace(MultiEndpoint))
                            {
                                endPoints = endPoints.Where(i=> i.ApplicationType == ApplicationType.MSSL || i.ApplicationType == ApplicationType.EBS || i.ApplicationType == ApplicationType.EBS_CRM || i.ApplicationType == ApplicationType.EBS_ISU || i.ApplicationType == ApplicationType.MSSL_CRM || i.ApplicationType == ApplicationType.MSSL_ISU).ToList();
                            }
                        }
                        else
                        {
                            endPoints = DomainApiClient<Dto.EndPoint>.GetAll().OrderBy(x => x.DisplayName).ToList();
                            if (!string.IsNullOrWhiteSpace(MultiEndpoint))
                            {
                                endPoints = endPoints.Where(i => i.ApplicationType == ApplicationType.MSSL || i.ApplicationType == ApplicationType.EBS || i.ApplicationType == ApplicationType.EBS_CRM || i.ApplicationType == ApplicationType.EBS_ISU || i.ApplicationType == ApplicationType.MSSL_CRM || i.ApplicationType == ApplicationType.MSSL_ISU).ToList();
                            }
                        }
                        if (endPoints != null && endPoints.Count > 0)
                        {
                            cmbBoxValue.Items.Add(new RadComboBoxItem("[---Select---]", "-1"));
                        }
                        endPoints.ForEach(key =>
                        {
                            cmbBoxValue.Items.Add(new RadComboBoxItem(key.DisplayName, key.Id.ToString()));
                        });

                        td2.Controls.Add(cmbBoxValue);

                        break;
                    }
                case CustomReportParameterTypes.UserName:
                case CustomReportParameterTypes.EmployeeId:
                    {
                        var txtBox = new RadAutoCompleteBox() { ID = id, ClientIDMode = ClientIDMode.Static, CssClass = "RadAutoCompleteBox", RenderMode = RenderMode.Lightweight };
                        txtBox.Style.Add(HtmlTextWriterStyle.Width, "100%");

                        txtBox.WebServiceSettings.Path = "../../Services/AutoCompleteDataService.svc";
                        if (parameter.ParameterType == CustomReportParameterTypes.UserName)
                        {

                            txtBox.WebServiceSettings.Method = "GetUsers";

                        }
                        if (parameter.ParameterType == CustomReportParameterTypes.EmployeeId)
                        {

                            txtBox.WebServiceSettings.Method = "GetUsers";

                        }
                        td2.Controls.Add(txtBox);
                        break;
                    }
                case CustomReportParameterTypes.varchar:
                    {
                        var txtbox = new RadTextBox() { ID = id, ClientIDMode = ClientIDMode.Static, CssClass = "txtUsername" };
                        txtbox.Style.Add(HtmlTextWriterStyle.Width, "100%");
                        if (!string.IsNullOrEmpty(parameter.DefaultValue))
                            txtbox.Text = parameter.DefaultValue;
                        td2.Controls.Add(txtbox);
                        break;
                    }
                case CustomReportParameterTypes.Long:
                    {
                        var txtbox = new RadNumericTextBox() { ID = id, ClientIDMode = ClientIDMode.Static, CssClass = "txtUsername" };
                        txtbox.Style.Add(HtmlTextWriterStyle.Width, "100%");
                        txtbox.NumberFormat.DecimalDigits = 0;
                        txtbox.NumberFormat.GroupSeparator = "";
                        if (!string.IsNullOrEmpty(parameter.DefaultValue))
                            txtbox.Text = parameter.DefaultValue;
                        td2.Controls.Add(txtbox);
                        break;
                    }
                case CustomReportParameterTypes.Bit:
                    {
                        var txtbox = new RadNumericTextBox() { ID = id, ClientIDMode = ClientIDMode.Static, CssClass = "txtUsername" };
                        txtbox.Style.Add(HtmlTextWriterStyle.Width, "100%");
                        txtbox.NumberFormat.DecimalDigits = 0;
                        txtbox.NumberFormat.GroupSeparator = "";
                        txtbox.MinValue = 0;
                        txtbox.MaxValue = 1;
                        if (!string.IsNullOrEmpty(parameter.DefaultValue))
                            txtbox.Text = parameter.DefaultValue;
                        td2.Controls.Add(txtbox);
                        break;
                    }
                case CustomReportParameterTypes.DateTime:
                    {
                        RadDatePicker radDatePicker1 = new RadDatePicker() { ID = id, ClientIDMode = ClientIDMode.Static };
                        radDatePicker1.Skin = "IACMSkin";

                        RadCalendar calendar = new RadCalendar();
                        calendar.ID = "RadCalendar_" + id;
                        calendar.ClientIDMode = ClientIDMode.Static;
                        calendar.UseColumnHeadersAsSelectors = false;
                        calendar.UseRowHeadersAsSelectors = false;
                        calendar.FastNavigationStep = 12;
                        calendar.EnableEmbeddedSkins = false;
                        calendar.EnableMultiSelect = false;

                        RadCalendarDay Day = new RadCalendarDay();

                        Day.Repeatable = Telerik.Web.UI.Calendar.RecurringEvents.Today;
                        calendar.SpecialDays.Add(Day);
                        radDatePicker1.Culture = new System.Globalization.CultureInfo("en-US");
                        radDatePicker1.Culture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
                        radDatePicker1.DateInput.DateFormat = "dd-MM-yyyy";
                        radDatePicker1.DateInput.DisplayDateFormat = "dd-MM-yyyy";
                        radDatePicker1.Controls.Add(calendar);


                        RadDateInput dateInput = radDatePicker1.DateInput;
                        dateInput.ID = "di_" + id;

                        if (!string.IsNullOrEmpty(parameter.DefaultValue))
                        {
                            radDatePicker1.SelectedDate = Convert.ToDateTime(parameter.DefaultValue);
                        }

                        radDatePicker1.Controls.Add(dateInput);
                        dateInput.ClientIDMode = ClientIDMode.Static;

                        radDatePicker1.DatePopupButton.Visible = true;
                        radDatePicker1.Calendar.RangeMinDate = System.DateTime.Now.Date.AddDays(-1000);
                        radDatePicker1.ViewStateMode = ViewStateMode.Enabled;
                        td2.Controls.Add(radDatePicker1);
                        break;
                    }

            }
            td2.Style.Add(HtmlTextWriterStyle.Width, "300px");
            tr.Controls.Add(td2);

            //if (!isLastParameter)
            //{
            //    HtmlGenericControl td3 = new HtmlGenericControl("td");
            //    RadComboBox cmbANDORBox = new RadComboBox() { ID = $"andOr_{id}", ClientIDMode = ClientIDMode.Static };
            //    cmbANDORBox.Width = 30;
            //    cmbANDORBox.Items.Add(new RadComboBoxItem("AND", "AND"));
            //    cmbANDORBox.Items.Add(new RadComboBoxItem("OR", "OR"));
            //    td3.Controls.Add(cmbANDORBox);
            //    tr.Controls.Add(td3);
            //}

            PlaceHolder1.Controls.Add(tr);
        }
        Dto.CustomReport getCurrentReport(long ReportId)
        {
            return DomainApiClient<Dto.CustomReport>.GetById(ReportId, true);
        }

        protected void RadReportJobs_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (WindowVisible)
            {
                return;
            }
            string updatedQuery = getUpdatedQueryWithOutArgument();

            if (IsQueryParameterized)
            {
                updatedQuery = getUpdatedQuery();
            }
            UpdateQuery = updatedQuery;
            LoadData(updatedQuery);
            SetGroupBy();

            ShowFilter();

        }

        private void SetGroupBy()
        {
            if (CurrentReport == null || !CurrentReport.IsGroupByEnabled || string.IsNullOrWhiteSpace(CurrentReport.GroupByColumn))
                return;

            GridGroupByExpression ge = new GridGroupByExpression();
            GridGroupByField gf = new GridGroupByField();
            gf.FieldName = CurrentReport.GroupByColumn;
            ge.SelectFields.Add(gf);
            ge.GroupByFields.Add(gf);
            RadReportJobs.MasterTableView.GroupByExpressions.Add(ge);
            RadReportJobs.ClientSettings.AllowGroupExpandCollapse = true;
        }

        void LoadData(string updatedQuery = null)
        {
            if (ReportId > 0)
            {
                int PageIndex = RadReportJobs.CurrentPageIndex;
                int PageSize = RadReportJobs.MasterTableView.PageSize;
                string sortingFieldName = "";
                string sortOrder = "";
                string sortExpressions = string.Empty;

                if (RadReportJobs.MasterTableView.SortExpressions.Count > 0)
                {
                    sortingFieldName = RadReportJobs.MasterTableView.SortExpressions[0].FieldName;
                    sortOrder = RadReportJobs.MasterTableView.SortExpressions[0].SortOrderAsString();
                    foreach (GridSortExpression item in RadReportJobs.MasterTableView.SortExpressions)
                    {
                        sortExpressions += string.Format("{0} {1},", item.FieldName, item.SortOrderAsString());
                    }
                }

                var filterExpression = RadReportJobs.MasterTableView.FilterExpression;

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
                else if (filterExpression.ToLower().Contains("date"))
                {
                    List<string> finalOutput = new List<string>();
                    foreach (string condition in filterExpression.Split(new string[] { " AND " }, StringSplitOptions.None))
                    {
                        string rawExpression = condition;
                        //if (rawExpression.ToLower().Contains("date") && rawExpression.ToLower().Contains("="))
                        //{
                        //    var filterOps = rawExpression.Split('=');
                        //    var field = filterOps[0].Split('(')[1];
                        //    var dateValues = rawExpression.Split('\'');
                        //    if (dateValues.Length > 2)
                        //    {
                        //        var fromDate = DateTime.ParseExact(dateValues[1], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                        //        var toDate = fromDate.AddDays(1);
                        //        finalOutput.Add(string.Format("({0} > '#{1}#' AND {0} < '#{2}#')", field, fromDate, toDate));
                        //    }
                        //}
                        //else 
                        if (rawExpression.ToLower().Contains("date"))
                        {
                            var dateValues = rawExpression.Split('\'');
                            if (dateValues.Length > 2)
                                finalOutput.Add(string.Format("{0}'#{1}#'{2}", dateValues[0], dateValues[1], dateValues[2]));
                        }
                        else
                            finalOutput.Add(rawExpression);
                    }

                    filterExpression = string.Join(" AND ", finalOutput);
                }

                //Dictionary<string, string> parameter1 = new Dictionary<string, string>();
                //parameter1.Add("reportId", ReportId.ToString());
                //parameter1.Add("FilterExpression", filterExpression);
                //var count = DomainApiClient<Dto.CustomReport>.
                //       GetByAction<long>(DomainApiAction.CustomReport.GetReportViewerDataCount, parameter1, updatedQuery);


                int startRowIndex = PageIndex * PageSize;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("reportId", ReportId.ToString());
                parameter.Add("startRowIndex", startRowIndex.ToString());
                parameter.Add("maximumRows", PageSize.ToString());
                parameter.Add("FilterExpression", filterExpression);
                parameter.Add("sortExpression", sortExpressions.Trim(',').Trim());

                Dto.PresentationCustomReportData reportData = DomainApiClient<Dto.CustomReport>.
                       GetByAction<PresentationCustomReportData>(DomainApiAction.CustomReport.GetReportViewerData, parameter, updatedQuery);
                list = reportData.ReportData;

                RadReportJobs.VirtualItemCount = (int)reportData.Count;
                totalCount = RadReportJobs.VirtualItemCount;

                RadReportJobs.Visible = true;

                if (list != null && list.Rows.Count > 0)
                {
                    RadReportJobs.DataSource = null;
                    RadReportJobs.DataSource = list;
                }
                else
                {
                    DataTable dt = new DataTable();
                    if (reportData.ReportColumns.Count > 0)
                    {
                        foreach (var v in reportData.ReportColumns)
                        {
                            dt.Columns.Add(v);
                        }
                        RadReportJobs.DataSource = dt;
                    }
                    else
                    {
                        RadReportJobs.DataSource = new List<string>();
                        RadReportJobs.Visible = false;
                    }
                }
            }
            else
            {
                RadReportJobs.DataSource = new List<string>();
            }
        }



        void ShowFilter()
        {
            //WindowVisible = true;
            //ParmeterDetails.VisibleOnPageLoad = true;
            if (FilterControlsAdded) return;
            IsQueryParameterized = true;
            for (int count = 0; count < CurrentReport.CustomReportParameters.Count; count++)// var parameter in customReport.CustomReportParameters)
            {
                if (!CurrentReport.CustomReportParameters[count].IsDeleted)
                    setControl(CurrentReport.CustomReportParameters[count]);//, count == customReport.CustomReportParameters.Count-1 );
            }
            getParameter.ClientIDMode = ClientIDMode.Static;
        }

        void CollapseFilter()
        {
            //WindowVisible = false;
            //ParmeterDetails.VisibleOnPageLoad = false;
        }
        string getUpdatedQueryWithOutArgument()
        {
            if (CurrentReport == null) return null;
            var query = CurrentReport.ReportQuery;
            if (CurrentReport.CustomReportParameters == null || CurrentReport.CustomReportParameters.Count == 0) return query;

            CurrentReport.CustomReportParameters.ForEach(reportParameter =>
            {
                if (!reportParameter.IsDeleted)
                {
                    if (reportParameter.ParameterVariant == CustomReportParameterVariant.Value)
                    {
                        query = query.Replace($"@{reportParameter.ParameterName}", reportParameter.DefaultValue);
                    }
                    else
                    {
                        query = query.Replace($":{reportParameter.ParameterName}", " 1=1 ");
                    }
                }
            });
            return query;
        }
        string getUpdatedQuery()
        {

            if (CurrentReport == null) return null;
            var query = CurrentReport.ReportQuery;
            try
            {
                var jsonObj = string.IsNullOrEmpty(getParameter.Value) ? null : (Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject(getParameter.Value);
                if(jsonObj != null)
                CurrentReport.CustomReportParameters.ForEach(reportParameter =>
                {
                    if (!reportParameter.IsDeleted)
                    {
                        var jobject = jsonObj.Where(jobj => jobj["Parameter"].ToString() == reportParameter.ParameterName.Replace(" ", "").Replace(".", "_").ToLower()).Select(jobj => jobj).FirstOrDefault();

                        if (string.IsNullOrEmpty(jobject["Value"].ToString()))
                        {
                            //if (!string.IsNullOrEmpty(reportParameter.DefaultValue))
                            //{
                            //    var operat = "EqualTo";
                            //    var v=   canConcatenateApostrophe(reportParameter.ParameterType, operat) ? concatenateApostrophe(reportParameter.DefaultValue, operat) : reportParameter.DefaultValue;
                            //    query = query.Replace($":{reportParameter.ParameterName}", $"{reportParameter.ParameterName}={v}");
                            //}
                            query = query.Replace($":{reportParameter.ParameterName}", $" 1=1 ");

                        }
                        else
                        {
                            var formatstring = reportParameter.DataFormatString;
                            var op = jobject["Operator"].ToString();
                            var value = jobject["Value"].ToString();

                            var valuestring = canConcatenateApostrophe(reportParameter.ParameterType, op) ? concatenateApostrophe(value, op) : value;
                            var parameter = jobject["Parameter"].ToString().Replace("_", ".");

                            if (!string.IsNullOrEmpty(formatstring))
                            {
                                valuestring = $@"CONVERT({reportParameter.ParameterType.ToString()},{valuestring},{formatstring})";
                                parameter = $@"CONVERT({reportParameter.ParameterType.ToString()},{parameter},{formatstring})";
                            }

                            var val = string.Format(Operators[op], valuestring);


                            if (reportParameter.ParameterType == CustomReportParameterTypes.DateTime && Operators[op] == "= {0}")
                            {
                                try
                                {
                                    valuestring = valuestring.Replace("'", "");
                                    val = string.Format(" >= '{0}' AND {1} < '{2}'", DateTime.ParseExact(valuestring, "yyyy-MM-dd", CultureInfo.InvariantCulture), parameter, DateTime.ParseExact(valuestring, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddHours(23).AddMinutes(59));
                                }
                                catch (Exception ex)
                                {

                                }
                            }



                            if (reportParameter.ParameterVariant == CustomReportParameterVariant.Value)
                                val = valuestring;

                            if (reportParameter.ParameterType == CustomReportParameterTypes.UserName)
                            {
                                try
                                {
                                    val = "'" + valuestring.Substring(valuestring.LastIndexOf("[") + 1, (valuestring.LastIndexOf("]") - valuestring.LastIndexOf("[")) - 1) + "'";
                                    //Regex.Match(valuestring, pattern).Groups[0].ToString();
                                }
                                catch (Exception ex)
                                {


                                }
                            }

                            if (reportParameter.ParameterType == CustomReportParameterTypes.EndpointSystem)
                            {
                                val = "'" + value + "'";
                            }

                            if (reportParameter.ParameterVariant == CustomReportParameterVariant.Value)
                                query = query.Replace($"@{reportParameter.ParameterName}", $"{val}");// {jobject["AndOr"].ToString()} ");
                            else
                                query = query.Replace($":{reportParameter.ParameterName}", $"{parameter}  {val}");// {jobject["AndOr"].ToString()} ");
                        }
                    }
                });
            }
            catch (Exception ee)
            {
                return query;
            }
            return query;
        }

        bool canConcatenateApostrophe(CustomReportParameterTypes type, string Operator)
        {
            return type == CustomReportParameterTypes.DateTime ||
                type == CustomReportParameterTypes.varchar && Operator != "Like";
        }

        string concatenateApostrophe(string value, string Operator)
        {
            if (Operator == "In")
            {
                string returnValue = string.Empty;
                var stringArray = value.Split(',');
                foreach (var val in stringArray)
                {
                    returnValue += $"'{val.Trim()}',";
                }
                returnValue = returnValue.Remove(returnValue.Length - 1, 1);
                return returnValue;
            }
            else
            {
                return $@"'{value}'";
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            IsQueryParameterized = true;
            //CollapseFilter();
            RadReportJobs.Rebind();
            //ShowFilter();
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            IsQueryParameterized = false;
            //CollapseFilter();
            RadReportJobs.Rebind();
            //ShowFilter();
        }

        //protected void imageClick_Event(object sender, EventArgs e)
        //{
        //    ShowFilter();
        //}
        protected void RadReportJobs_Init(object sender, EventArgs e)
        {
            RadReportJobs.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top;
            RadReportJobs.MasterTableView.CommandItemSettings.ShowAddNewRecordButton = false;
            RadReportJobs.MasterTableView.CommandItemSettings.ShowExportToCsvButton = false;
            RadReportJobs.MasterTableView.CommandItemSettings.ShowExportToExcelButton = false;
            RadReportJobs.MasterTableView.CommandItemSettings.ShowExportToPdfButton = false;
            RadReportJobs.MasterTableView.CommandItemSettings.ShowRefreshButton = false;
            RadReportJobs.MasterTableView.ShowHeadersWhenNoRecords = true;

            RadReportJobs.MasterTableView.CommandItemTemplate = new radGridCommandItemTemplate();
            GridFilterMenu menu = RadReportJobs.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" || menu.Items[i].Text == "Contains" || menu.Items[i].Text == "EqualTo" ||
                    menu.Items[i].Text == "GreaterThan" || menu.Items[i].Text == "LessThan" || menu.Items[i].Text == "StartsWith" || menu.Items[i].Text == "EndsWith"
                    || menu.Items[i].Text == "Between")
                {
                    i++;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }

        private void ShowDocument(string fileName, byte[] fileContent, string extension)
        {
            try
            {
                HttpResponse Response = HttpContext.Current.Response;
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName + extension);
                Response.Buffer = true;
                Response.AddHeader("Content-Length", fileContent.Length.ToString());
                Response.BinaryWrite(fileContent);
                Response.Flush();
                Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                Response.End();
            }
            catch (Exception ex)
            {

            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            try
            {
                HttpContext.Current.Session["ExportStatus"] = "false";

                if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
                {
                    if (CurrentReport.IsGroupByEnabled)
                    {
                        ExportWithGroupBy(e.ActionType);
                        return;
                    }
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("reportId", ReportId.ToString());
                    parameter.Add("isExcelReport", Convert.ToString(e.ActionType == ACLAction.Excel));
                    parameter.Add("RequestedUser", new SessionStatusImpl().User.DisplayName);
                    parameter.Add("filterExpression", RadReportJobs.MasterTableView.FilterExpression);

                    var report = DomainApiClient<Dto.CustomReport>.
                           GetByAction<byte[]>(DomainApiAction.CustomReport.GetReportViewerDataForExport, parameter, UpdateQuery);

                    if (report != null)
                    {
                        Dto.CustomReport custumReport = DomainApiClient<Dto.CustomReport>.GetById(ReportId);

                        if (custumReport != null)
                            ShowDocument(custumReport.FileName, report, e.ActionType == ACLAction.Excel ? ".xlsx" : ".csv");
                    }
                }
                else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
                {
                    if (CurrentReport.IsGroupByEnabled)
                    {
                        ExportWithGroupBy(e.ActionType);
                        return;
                    }
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("reportId", ReportId.ToString());
                    parameter.Add("isExcelReport", Convert.ToString(e.ActionType == ACLAction.ExcelAll));
                    parameter.Add("RequestedUser", new SessionStatusImpl().User.DisplayName);
                    parameter.Add("filterExpression", RadReportJobs.MasterTableView.FilterExpression);

                    var report = DomainApiClient<Dto.CustomReport>.
                           GetByAction<byte[]>(DomainApiAction.CustomReport.GetReportViewerDataForExport, parameter, UpdateQuery);

                    if (report != null)
                    {
                        Dto.CustomReport custumReport = DomainApiClient<Dto.CustomReport>.GetById(ReportId);

                        if (custumReport != null)
                            ShowDocument(custumReport.FileName, report, e.ActionType == ACLAction.ExcelAll ? ".xlsx" : ".csv");
                    }
                }
                //else if (e.ActionType == ACLAction.Schedule)
                //{

                //    usrModal.VisibleOnPageLoad = true;
                //    RptSchd.InitializeSched();
                //    RptSchd.RptType = ReportType.CustomReport.ToString(); //EndpointUC.SelectedApplication.DisplayName.ToString() + "-AccountWithNoGroups";
                //}
            }
            catch (Exception ex)
            {

            }
            finally
            {
                HttpContext.Current.Session["ExportStatus"] = "true";
            }
        }

        public void ExportExcelPDF(RadGrid reportData, ACLAction actionType)
        {
            foreach (GridDataItem item in reportData.MasterTableView.Items)
            {
                item.Expanded = true;
            }
            reportData.ExportSettings.ExportOnlyData = true;
            reportData.ExportSettings.IgnorePaging = true;
            reportData.ExportSettings.OpenInNewWindow = true;

            reportData.ExportSettings.Excel.Format = GridExcelExportFormat.Html;
            reportData.ExportSettings.HideStructureColumns = true;
            reportData.MasterTableView.HierarchyDefaultExpanded = true;
            reportData.MasterTableView.DetailTables[0].HierarchyDefaultExpanded = true;
            reportData.MasterTableView.HierarchyLoadMode = GridChildLoadMode.Client;
            reportData.MasterTableView.DetailTables[0].HierarchyLoadMode = GridChildLoadMode.Client;
            if (actionType == ACLAction.CSV || actionType == ACLAction.CSVAll)
            {
                reportData.ExportSettings.FileName = "file.csv";
                reportData.MasterTableView.ExportToCSV();
            }
            else
            {
                reportData.ExportSettings.FileName = "file.xls";
                reportData.MasterTableView.ExportToCSV();
            }
        }

        public void ExportWithGroupBy(ACLAction actionType)
        {
            try
            {
                HttpContext.Current.Session["ExportStatus"] = "false";
                if (RadReportJobs.Items.Count > 0)
                {
                    RadReportJobs.ExportSettings.HideStructureColumns = true;
                    RadReportJobs.ExportSettings.IgnorePaging = true;
                    RadReportJobs.ExportSettings.ExportOnlyData = true;
                    RadReportJobs.ExportSettings.OpenInNewWindow = true;
                    RadReportJobs.ExportSettings.Excel.Format = GridExcelExportFormat.ExcelML;
                    if (actionType == ACLAction.CSV || actionType == ACLAction.CSVAll)
                    {
                        RadReportJobs.MasterTableView.ExportToCSV();
                    }
                    else
                    {
                        RadReportJobs.MasterTableView.ExportToExcel();
                    }
                }
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, "Export report failed. Please contact Administrator.");
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Exception, "Export Report - " + RadReportJobs.ExportSettings.FileName, ex);
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
                e.Cell.Style["mso-number-format"] = "Date and Time";
            }
            GridDataItem item = e.Cell.Parent as GridDataItem;
            e.Cell.HorizontalAlign = HorizontalAlign.Center;
        }

        protected void RadReportJobs_PreRender(object sender, EventArgs e)
        {
            foreach (GridColumn col in RadReportJobs.MasterTableView.RenderColumns)
            {
                if (col is GridBoundColumn)
                {
                    ((GridBoundColumn)col).DataFormatString = "<nobr>{0}</nobr>";
                }

                if (col is GridDateTimeColumn)
                {
                    ((GridDateTimeColumn)col).EnableTimeIndependentFiltering = true;
                    ((GridDateTimeColumn)col).EnableRangeFiltering = true;
                    ((GridDateTimeColumn)col).PickerType = GridDateTimeColumnPickerType.DatePicker;
                    ((GridDateTimeColumn)col).HeaderStyle.CssClass = "ImpDateRangeFilter";
                }
            }
            RadReportJobs.Rebind();
        }

        protected void RadReportJobs_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "gridFilterClear")
            {
                ClearFilters();
                RadReportJobs.MasterTableView.FilterExpression = string.Empty;
                string redirectTarget = Request.Url.PathAndQuery;
                if (DI.Chakra.Infrastructure.UtilityMethods.IsLocalUrl(redirectTarget))
                    Response.Redirect(redirectTarget);
                else
                    Response.Redirect("~/Default.aspx");
            }
        }

        private void ClearFilters()
        {
            foreach (object gridColumn in RadReportJobs.Columns)
            {
                if (gridColumn is GridBoundColumn)
                {
                    GridBoundColumn gridboundColumn = (GridBoundColumn)gridColumn;
                    bool hasValue = gridboundColumn.CurrentFilterValue == "" ? false : true;
                    gridboundColumn.CurrentFilterFunction = GridKnownFunction.NoFilter;
                    gridboundColumn.CurrentFilterValue = "";
                }
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("reportId", ReportId.ToString());
            parameter.Add("isExcelReport", Convert.ToString(false));
            parameter.Add("requestedUser", new SessionStatusImpl().User.DisplayName);

            var report = DomainApiClient<Dto.CustomReport>.
                   GetByAction<bool>(DomainApiAction.CustomReport.GetReportViewerDataForExport, parameter);

            Dto.CustomReport custumReport = DomainApiClient<Dto.CustomReport>.GetById(ReportId);

            if (custumReport != null)
                ShowDocument(custumReport.FileName, custumReport.Attachment, ".csv");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //report = RptSchd.GetReportingSchedule();
                //scheduler = RptSchd.Scheduler;
                ////attributeParameter.Add("EndPointSystemTypeId", ((long)EndpointUC.SelectedEndpoint.EndpointSystemTypeId).ToString());
                ////attributeParameter.Add("EndPointSystemId", EndpointUC.SelectedEndpoint.Id.ToString());
                ////attributeParameter.Add("EndPointReport", "AccountWithNoGroups");
                //if (SaveCustomReport())
                //{
                //    RptSchd.ClearpopupData();
                //    usrModal.VisibleOnPageLoad = false;
                //}
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, "Schduler is not filled with schedule values");
            }
        }

        protected void btnSchCancel_Click(object sender, EventArgs e)
        {
            //usrModal.VisibleOnPageLoad = false;
            //RptSchd.InitializeSched();
        }

        protected void RadReportJobs_DataBinding(object sender, EventArgs e)
        {
            foreach (GridColumn col in RadReportJobs.MasterTableView.RenderColumns)
            {
                if (col is GridDateTimeColumn)
                {
                    ((GridDateTimeColumn)col).DataFormatString = "{0:dd-MM-yyyy hh:mm:ss tt}";
                }
            }
        }
    }
}