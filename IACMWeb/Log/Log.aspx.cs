using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Log
{
    public partial class Log : UIBasePage
    {
        private static IList<ServiceLog> servicelogs = new List<ServiceLog>();
        private static IList<DiscoveryLog> discoverylogs = new List<DiscoveryLog>();
        private static IList<ProvisioningLog> provisioninglogs = new List<ProvisioningLog>();
        private static IList<AttestationLog> attestationlogs = new List<AttestationLog>();
        private static IList<EmailLog> emaillogs = new List<EmailLog>();
        private double reportStartDataDay = -15;
        private double reportEndDataDay = 14;
        SessionStatusImpl session = new SessionStatusImpl();
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                DatePickerValidFrom.MaxDate = DateTime.Now.Date;
                DatePickerValidTo.MaxDate = DateTime.Now.Date;
            }

        }

        public string logRefValue
        {
            get
            {
                if (ViewState["logRefValue"] != null)
                    return (string)ViewState["logRefValue"];

                return string.Empty;
            }
            set
            {
                ViewState["logRefValue"] = value;
            }
        }
        public LogLevelType loglevelSelectedValue
        {
            get
            {
                if (ViewState["loglevelSelectedValue"] != null)
                    return (LogLevelType)ViewState["loglevelSelectedValue"];

                return LogLevelType.All;
            }
            set
            {
                ViewState["loglevelSelectedValue"] = value;
            }
        }


        public long EndppointSystemId
        {
            get
            {
                if (ViewState["logEndppointSystemId"] != null)
                    return Convert.ToInt16(ViewState["logEndppointSystemId"]);

                return 0;
            }
            set
            {
                ViewState["logEndppointSystemId"] = value;
            }
        }


        public long EndppointSystemTypeId
        {
            get
            {
                if (ViewState["logEndppointSystemTypeId"] != null)
                    return Convert.ToInt16(ViewState["logEndppointSystemTypeId"]);

                return 0;
            }
            set
            {
                ViewState["logEndppointSystemTypeId"] = value;
            }
        }

        public int DiscoveryObject
        {
            get
            {
                if (ViewState["logDiscoveryObject"] != null)
                    return Convert.ToInt16(ViewState["logDiscoveryObject"]);

                return 0;
            }
            set
            {
                ViewState["logDiscoveryObject"] = value;
            }
        }

        public long AccessRequestId
        {
            get
            {
                if (ViewState["logAccessRequestId"] != null)
                    return Convert.ToInt64(ViewState["logAccessRequestId"]);

                return 0;
            }
            set
            {
                ViewState["logAccessRequestId"] = value;
            }
        }


        public string filterExpression
        {
            get
            {
                if (ViewState["filterExpression"] != null)
                    return ViewState["filterExpression"].ToString();

                return string.Empty;
            }
            set
            {
                ViewState["filterExpression"] = value;
            }
        }

        void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                if (IsValid())
                {
                    LoadLogGrid(DatePickerValidFrom.SelectedDate, DatePickerValidTo.SelectedDate, logRefValue, loglevelSelectedValue, false, false);
                }
                else
                {
                    return;
                }
            }

            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {

                //if (ddlLogRefType.SelectedItem.Text == LogReferenceType.Discovery.ToString())
                //{
                //    grdLogs.Export<IList<DiscoveryLog>>(discoverylogs, e.ActionType, "Log", false);
                //}
                //else if (ddlLogRefType.SelectedItem.Text == LogReferenceType.Provisioning.ToString())
                //{
                //    grdLogs.Export<IList<ProvisioningLog>>(provisioninglogs, e.ActionType, "Log", false);
                //}
                //else if (ddlLogRefType.SelectedItem.Text == LogReferenceType.Attestation.ToString())
                //{
                //    grdLogs.Export<IList<AttestationLog>>(attestationlogs, e.ActionType, "Log", false);
                //}
                if (ddlLogRefType.SelectedItem.Text == LogReferenceType.Email.ToString())
                {
                    grdEmailLog.Export<IList<EmailLog>>(emaillogs, e.ActionType, "Log", false);
                }
                else
                {
                    ExportNew(grdLogs, e.ActionType, "Log");
                }
                //else
                //{
                //    grdLogs.Export<IList<ServiceLog>>(servicelogs, e.ActionType, "Log", false);
                //}

            }
            //else if )
            //{
            //    ExportNew(grdLogs, e.ActionType, "Log");
            //    //if (ddlLogRefType.SelectedItem.Text == LogReferenceType.Discovery.ToString())
            //    //{
            //    //    grdLogs.Export<IList<DiscoveryLog>>(discoverylogs, e.ActionType, "Log");
            //    //}
            //    //else if (ddlLogRefType.SelectedItem.Text == LogReferenceType.Provisioning.ToString())
            //    //{
            //    //    grdLogs.Export<IList<ProvisioningLog>>(provisioninglogs, e.ActionType, "Log");
            //    //}
            //    //else if (ddlLogRefType.SelectedItem.Text == LogReferenceType.Attestation.ToString())
            //    //{
            //    //    grdLogs.Export<IList<AttestationLog>>(attestationlogs, e.ActionType, "Log");
            //    //}
            //    //else if (ddlLogRefType.SelectedItem.Text == LogReferenceType.Email.ToString())
            //    //{
            //    //    grdLogs.Export<IList<EmailLog>>(emaillogs, e.ActionType, "Log");
            //    //}
            //    //else
            //    //{
            //    //    grdLogs.Export<IList<ServiceLog>>(servicelogs, e.ActionType, "Log");
            //    //}
            //}
        }

        protected void DatePickerValidTo_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
        {
            DatePickerValidTo.Enabled = true;
        }

        public void LoadLogGrid(DateTime? dtFrom, DateTime? dtTo, string logReferenceType, LogLevelType logleveltype, bool isPagedRecords = true, bool bindGrid = true)
        {
            if (string.IsNullOrWhiteSpace(logReferenceType)) return;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            var logParameter = new LogParameter();

            parameter.Add("logReferenceType", logReferenceType);
            logParameter.logReferenceType = logReferenceType;
            DateTime defaultFrom = DateTime.Now.AddDays(reportStartDataDay + 1);
            DateTime defaultTo = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(txtRequestId.Text))
            {
                if (dtFrom.HasValue)
                {
                    logParameter.dtFrom = Convert.ToDateTime(dtFrom.Value.Date.ToString("dd-MMM-yyyy 00:00:00.001"));
                }
                if (dtTo.HasValue)
                {
                    logParameter.dtTo =Convert.ToDateTime(dtTo.Value.Date.ToString("dd-MMM-yyyy 23:59:59.999"));
                }
            }
            else
            {
                if (dtTo.HasValue)
                    defaultFrom = dtTo.Value.AddDays(reportStartDataDay);

                if (dtFrom.HasValue)
                    defaultTo = dtFrom.Value.AddDays(reportEndDataDay);

                parameter.Add("dtFrom", dtFrom.HasValue ? dtFrom.Value.ToString("dd-MMM-yyyy 00:00:00.001") : defaultFrom.ToString("dd-MMM-yyyy 00:00:00.001"));
                parameter.Add("dtTo", dtTo.HasValue ? dtTo.Value.ToString("dd-MMM-yyyy 23:59:59.999") : defaultTo.ToString("dd-MMM-yyyy 23:59:59.999"));
                logParameter.dtFrom = dtFrom.HasValue ? dtFrom.Value.Date : defaultFrom.Date;
                logParameter.dtTo = dtTo.HasValue ? dtTo.Value.Date.AddDays(1).AddTicks(-1) : defaultTo.Date.AddDays(1).AddTicks(-1);
            }

            parameter.Add("logLevelType", logleveltype.ToString());


            logParameter.logLevelType = logleveltype;
            logParameter.endpointSystemId = EndppointSystemId;
            logParameter.RequestId = AccessRequestId == 0 ? null : (long?)AccessRequestId;


            filterExpression = grdLogs.MasterTableView.FilterExpression;

            string sortingFieldName = "";
            string sortOrder = "";
            string sortExpressions = string.Empty;
            if (grdLogs.MasterTableView.SortExpressions.Count > 0)
            {
                sortingFieldName = grdLogs.MasterTableView.SortExpressions[0].FieldName;
                sortOrder = grdLogs.MasterTableView.SortExpressions[0].SortOrderAsString();
                foreach (GridSortExpression item in grdLogs.MasterTableView.SortExpressions)
                {
                    sortExpressions += string.Format("{0} {1},", item.FieldName, item.SortOrderAsString());
                }
                if (!string.IsNullOrWhiteSpace(sortExpressions))
                {
                    sortExpressions = sortExpressions.Substring(0, sortExpressions.Length - 1);
                }
            }

            filterExpression = grdLogs.MasterTableView.FilterExpression;

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

            parameter.Add("FilterExpression", filterExpression);
            parameter.Add("sortExpression", sortExpressions.Trim(',').Trim());

            logParameter.FilterExpression = filterExpression;
            logParameter.sortExpression = sortExpressions;
            //if (isPagedRecords)
            //{
            //    parameter.Add("pageSize", grdLogs.MasterTableView.PageSize.ToString());
            //    parameter.Add("pageNumber", (grdLogs.CurrentPageIndex + 1).ToString());
            //    logParameter.pageSize = grdLogs.MasterTableView.PageSize;
            //    logParameter.pageNumber = grdLogs.CurrentPageIndex + 1;
            //}
            parameter.Add("pageSize", null);
            parameter.Add("pageNumber", null);

            if (logReferenceType == LogReferenceType.Discovery.ToString())
            {

                if (logParameter.endpointSystemId < 0)
                {
                    MasterPage.ShowAlert(DI.Chakra.Infrastructure.AlertType.Error, "Please select the endpoint system.");
                    return;
                }
                grdLogs.AllowCustomPaging = false;
                grdLogs.VirtualItemCount = Convert.ToInt32(DomainApiClient<DiscoveryLog>.GetByAction<long>(DomainApiAction.Log.ServiceLog.GetCountByLogDate, null, logParameter));



                discoverylogs = DomainApiClient<DiscoveryLog>.GetByAction<IList<DiscoveryLog>>(DomainApiAction.Log.ServiceLog.GetByLogDate, null, logParameter);
                if (bindGrid)
                    grdLogs.DataSource = discoverylogs;

            }
            else if (logReferenceType == LogReferenceType.Provisioning.ToString())
            {
                grdLogs.AllowCustomPaging = false;
                grdLogs.VirtualItemCount = Convert.ToInt32(DomainApiClient<ProvisioningLog>.GetByAction<long>(DomainApiAction.Log.ServiceLog.GetCountByLogDate, null, logParameter));
                provisioninglogs = DomainApiClient<ProvisioningLog>.GetByAction<IList<ProvisioningLog>>(DomainApiAction.Log.ServiceLog.GetByLogDate, null, logParameter);
                if (bindGrid)
                    grdLogs.DataSource = provisioninglogs;
            }
            else if (logReferenceType == LogReferenceType.Attestation.ToString())
            {
                grdLogs.VirtualItemCount = Convert.ToInt32(DomainApiClient<AttestationLog>.GetByAction<long>(DomainApiAction.Log.ServiceLog.GetCountByLogDate, parameter));

                attestationlogs = DomainApiClient<AttestationLog>.GetByAction<IList<AttestationLog>>(DomainApiAction.Log.ServiceLog.GetByLogDate, parameter);
                if (bindGrid)
                    grdLogs.DataSource = attestationlogs;
            }
            else if (logReferenceType == LogReferenceType.Email.ToString())
            {
                grdEmailLog.VirtualItemCount = Convert.ToInt32(DomainApiClient<EmailLog>.GetByAction<long>(DomainApiAction.Log.ServiceLog.GetCountByLogDate, parameter));

                emaillogs = DomainApiClient<EmailLog>.GetByAction<IList<EmailLog>>(DomainApiAction.Log.ServiceLog.GetByLogDate, parameter);
                if (bindGrid)
                    grdEmailLog.DataSource = emaillogs;

                grdEmailLog.Visible = true;
            }
            else
            {
                grdLogs.VirtualItemCount = Convert.ToInt32(DomainApiClient<ServiceLog>.GetByAction<long>(DomainApiAction.Log.ServiceLog.GetCountByLogDate, parameter));


                //parameter.Add("pageSize", grdLogs.MasterTableView.PageSize.ToString());
                //parameter.Add("pageNumber", (grdLogs.CurrentPageIndex + 1).ToString());

                servicelogs = DomainApiClient<ServiceLog>.GetByAction<IList<ServiceLog>>(DomainApiAction.Log.ServiceLog.GetByLogDate, parameter);
                if (bindGrid)
                    grdLogs.DataSource = servicelogs;
            }
        }

        //void UpdateDiscoveryLoggerWithEndpointDetails(IList<DiscoveryLog> discoverylogs)
        //{
        //    var endpointTypes = DomainApiClient<Dto.EndPointSystemType>.GetAll();
        //    var endpoints = DomainApiClient<Dto.EndPoint>.GetAll();
        //    foreach (var log in discoverylogs)
        //    {
        //        if (log.EndpointSystemId > 0)
        //            log.EndpointSystemName = endpoints.FirstOrDefault(p => p.Id == log.EndpointSystemId)?.DisplayName;
        //        if (log.EndpointSystemTypeId.HasValue && log.EndpointSystemTypeId.Value > 0)
        //            log.EndpointSystemTypeName = endpointTypes.FirstOrDefault(p => p.Id == log.EndpointSystemTypeId.Value)?.DisplayName;
        //    }
        //}

        public void InitializeLogRefType()
        {
            if (!IsPostBack)
            {

                //var logref = Enum.GetValues(typeof(LogReferenceType)).Cast<LogReferenceType>().ToList();
                var logref = new List<LogReferenceType> { LogReferenceType.Attestation, LogReferenceType.Discovery, LogReferenceType.Provisioning, LogReferenceType.Email, LogReferenceType.Scheduler, LogReferenceType.PasswordReset,LogReferenceType.AccountUnlock };

                ddlLogRefType.Items.Clear();
                ddlEpType.Items.Clear();
                ddlEpName.Items.Clear();
                ddlLogLevel.Items.Clear();

                ddlLogRefType.Items.Add(new ListItem("--Select--", "0"));

                foreach (var lref in logref)
                    ddlLogRefType.Items.Add(new ListItem(lref.ToString(), ((int)lref).ToString()));


                ddlLogRefType.Items.FindByText("--Select--").Selected = true;


                var endpointTypes = Enum.GetValues(typeof(EndpointType)).Cast<EndpointType>();

                ddlEpType.Items.Clear();
                ddlEpType.Items.Add(new ListItem("--Select--", "0"));

                foreach (var lref in endpointTypes.OrderBy(i=> i.ToString()))
                    ddlEpType.Items.Add(new ListItem(lref.ToString(), ((int)lref).ToString()));


                ddlEpType.Items.FindByText("--Select--").Selected = true;


                var LogLevels = Enum.GetValues(typeof(LogLevelType)).Cast<LogLevelType>();

                ddlLogLevel.Items.Clear();
                ddlLogLevel.Items.Add(new ListItem("--Select--", "-1"));

                foreach (var lref in LogLevels)
                    ddlLogLevel.Items.Add(new ListItem(lref.ToString(), ((int)lref).ToString()));


                ddlLogLevel.Items.FindByText("--Select--").Selected = true;

            }

            logRefValue = ddlLogRefType.SelectedIndex > 0 ? ddlLogRefType.SelectedItem.Text : string.Empty;

            loglevelSelectedValue = ddlLogLevel.SelectedIndex > 0 ? (LogLevelType)Convert.ToInt32(ddlLogLevel.SelectedItem.Value) : LogLevelType.All;



            EndppointSystemId = ddlEpName.SelectedIndex > -1 ? Convert.ToInt64(ddlEpName.SelectedItem.Value) : 0;

            long reqId = 0;
            Int64.TryParse(txtRequestId.Text, out reqId);

            AccessRequestId = reqId;

        }


        protected void ddlLogRefType_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdEmailLog.DataSource = null;
            grdEmailLog.DataBind();
            grdEmailLog.Visible = false;
            grdLogs.DataSource = null;
            grdLogs.DataBind();
            grdLogs.Visible = false;
            grdEmailLog.ClearFilter();
            grdLogs.ClearFilter();
            if (ddlLogRefType.SelectedIndex > 0 || (ddlLogRefType.SelectedItem != null && !string.IsNullOrWhiteSpace(ddlLogRefType.SelectedItem.Text)))
            {
                discoverydiv.Visible = (ddlLogRefType.SelectedItem.Text == LogReferenceType.Discovery.ToString());
                //DiscoveryGridDiv.Visible = (ddlLogRefType.SelectedItem.Text == LogReferenceType.Discovery.ToString());
                if (discoverydiv.Visible)
                {
                    ddlEpType.ClearSelection();
                    ddlEpName.ClearSelection();
                }
                RequestIddiv.Visible = (ddlLogRefType.SelectedItem.Text == LogReferenceType.Provisioning.ToString());
                // ProvsioningGridDiv.Visible = (ddlLogRefType.SelectedItem.Text == LogReferenceType.Provisioning.ToString());
            }
        }

        protected void ddlEpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlEpName.Enabled = false;
            if (ddlEpType.SelectedIndex > 0)
            {
                ddlEpName.Enabled = true;
                ddlEpName.DataSource = DomainApiClient<Dto.EndPoint>.GetByAction<List<EndPoint>>(DomainApiAction.Endpoint.EndPoint.GetOrgEndpointByType,
                    new Dictionary<string, string> { { "id", Convert.ToInt32(ddlEpType.SelectedValue).ToString() } });
                ddlEpName.DataTextField = "DisplayName";
                ddlEpName.DataValueField = "ID";
                ddlEpName.DataBind();
            }
            else
            {
                ddlEpName.Items.Clear();
            }
        }

        protected void grdLogs_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            InitializeLogRefType();
            LoadLogGrid(DatePickerValidFrom.SelectedDate, DatePickerValidTo.SelectedDate, logRefValue, loglevelSelectedValue);
            grdLogs.Visible = true;

        }
        private bool IsValid()
        {
            if (ddlLogRefType.SelectedIndex == 0)
            {
                MasterPage.ShowAlert(Infrastructure.AlertType.Error, "Please select Log Type.");
                return false;
            }
            if ((DatePickerValidFrom.SelectedDate.HasValue && !DatePickerValidTo.SelectedDate.HasValue) || (!DatePickerValidFrom.SelectedDate.HasValue && DatePickerValidTo.SelectedDate.HasValue))
            {
                MasterPage.ShowAlert(Infrastructure.AlertType.Error, "Please select From and To.");
                return false;
            }
            if (DatePickerValidFrom.SelectedDate.HasValue && DatePickerValidTo.SelectedDate.HasValue)
            {
                if ((DatePickerValidTo.SelectedDate.Value - DatePickerValidFrom.SelectedDate.Value).TotalDays > 15)
                {
                    MasterPage.ShowAlert(Infrastructure.AlertType.Error, "Date Range (From and To) should be Maximum of 15 Days only allowed. Please try again");
                    return false;
                }
            }
            return true;
        }
        protected void Filter_Click(object sender, EventArgs e)
        {
            if (IsValid())
            {
                if (ddlLogRefType.SelectedItem.Text == LogReferenceType.Email.ToString())
                {
                    grdEmailLog.Rebind();
                }
                else
                {
                    grdLogs.Rebind();
                }
            }
        }

        protected void ClearFilter_Click(object sender, EventArgs e)
        {
            grdLogs.ClearFilter();
            grdEmailLog.ClearFilter();
            ddlLogRefType.ClearSelection();
            ddlLogRefType.SelectedIndex = 0;
            ddlLogRefType.Items.FindByText("--Select--").Selected = true;
            txtRequestId.Text = "";
            ddlEpName.ClearSelection();
            ddlEpName.Items.Clear();
            ddlEpType.ClearSelection();
            ddlLogLevel.ClearSelection();
            discoverydiv.Visible = false;
            RequestIddiv.Visible = false;
            ddlEpType.Items.FindByText("--Select--").Selected = true;
            ddlLogLevel.Items.FindByText("--Select--").Selected = true;
            grdEmailLog.Visible = false;
            grdEmailLog.DataBind();
            grdLogs.Visible = false;
            grdLogs.DataSource = null;
            grdLogs.DataSource = null;
            grdLogs.DataBind();
            DatePickerValidFrom.SelectedDate = null;
            DatePickerValidTo.SelectedDate = null;
            grdLogs.Rebind();
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            grdLogs.Rebind();
        }

        protected void LogLevel_PreRender(object sender, EventArgs e)
        {
            RadComboBox rcbFilter = sender as RadComboBox;
            if (!Page.IsPostBack)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", session.User.Id.ToString());
                parameter.Add("gridId", grdLogs.ID);
                parameter.Add("pagePath", System.Web.HttpContext.Current.Request.Url.AbsolutePath);
                string gridFilterValue = DomainApiClient<Dto.UserGridFilterInfo>.GetByAction<string>(DomainApiAction.Preference.UserGridFilterInfo.Getfiltervalue, parameter);
                if (gridFilterValue != null)
                    ViewState[rcbFilter.ID] = gridFilterValue;
            }
            if (ViewState[rcbFilter.ID] != null)
            {
                rcbFilter.SelectedValue = ViewState[rcbFilter.ID].ToString();
            }
        }

        protected void grdEmailLog_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            InitializeLogRefType();
            LoadLogGrid(DatePickerValidFrom.SelectedDate, DatePickerValidTo.SelectedDate, logRefValue, loglevelSelectedValue);
            grdEmailLog.Visible = true;
        }


        //public static string GetDescription(object enumValue, string defDesc)
        //{
        //    FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

        //    if (null != fi)
        //    {
        //        object[] attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
        //        if (attrs.Length > 0)
        //            return ((DescriptionAttribute)attrs[0]).Description;
        //    }
        //    return defDesc;
        //}
    }
}