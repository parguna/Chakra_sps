using System;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using DI.Chakra.Logger;
using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq.Dynamic;
using System.Linq;
using DI.Chakra.Dto.Core;
using System.Text.RegularExpressions;

namespace DI.Chakra.Web.Account.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        #region Members
        private const string V_ID = "ENTITY_ID";
        private SessionStatusImpl Session = new SessionStatusImpl();
        private static List<Dto.Presentation.PresentationAccountReconcilationReport> list;
        #endregion

        #region Properties
        public long EntityId
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return -1;
                return (long)ViewState[V_ID];
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }
        public Int32 UserAccountType
        {
            get
            {
                if (ViewState["AccountType"] == null)
                    return -1;
                return Convert.ToInt32(ViewState["AccountType"].ToString());
            }
            set
            {
                ViewState["AccountType"] = value;
            }
        }

        #endregion

        #region Methods
        private void BindExtraCriteria()
        {
            MappingTypesField.Items.Clear();
            MappingTypesField.Items.Add(new ListItem() { Text = "All", Value = "3" });
            MappingTypesField.Items.Add(new ListItem() { Text = "Automatically Mapped", Value = "1" });
            MappingTypesField.Items.Add(new ListItem() { Text = "Manually Mapped", Value = "2" });
            MappingTypesField.Items.Add(new ListItem() { Text = "Un-Mapped", Value = "0" });
        }
        protected void AttributeGridDataBind()
        {
            if (Session.AccountNavigatorHelper != null && Session.AccountNavigatorHelper.ApplicationId > 0)
            {

                var endpointId = Session.AccountNavigatorHelper.ApplicationId;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointSystemId", endpointId.ToString());
                EndpointConfiguration epConfig = DomainApiClient<EndpointConfiguration>.GetByAction<EndpointConfiguration>(DomainApiAction.Endpoint.EndPoint.Configuration.EndpointConfiguration.GetByEndpointSystemId, parameter);
                MappingGrid.DataSource = epConfig.EndpointSystem.ListAttributeMappingColumns;
                lblMapType.Text = epConfig.MapAll ? "Map All" : "Map Any One";
            }
            else
                return;
        }
        protected void GridDataBind()
        {
            Account.DataSource = GetAccountList();
        }
        private List<Dto.Presentation.PresentationAccountReconcilationReport> GetAccountList(bool isForExport = false)
        {
            int PageIndex = isForExport ? 0 : (Account.CurrentPageIndex * Account.MasterTableView.PageSize);
            int PageSize = isForExport ? -1 : Account.MasterTableView.PageSize;
            string sortingFieldName = "";
            string sortOrder = "";
            string sortExpressions = string.Empty;

            var accountReport = new List<Dto.Presentation.PresentationAccountReconcilationReport>();
            if (EntityId < 0)
            {
                Account.VirtualItemCount = 0;
                return accountReport;
            }

            Dictionary<string, string> paramter = new Dictionary<string, string>();
            paramter.Add("systemId", EntityId.ToString());
            paramter.Add("mappingTypes", MappingTypesField.SelectedValue);

            if (UserAccountType > -1)
            {
                paramter.Add("accType", UserAccountType.ToString());
            }
            else
            {
                paramter.Add("accType", null);
            }

            PresentationGridPagedDataParameter pagedParam = new PresentationGridPagedDataParameter();
            pagedParam.StartRowIndex = PageIndex;
            pagedParam.MaximumRows = PageSize;

            if (Account.MasterTableView.SortExpressions.Count > 0)
            {
                sortingFieldName = Account.MasterTableView.SortExpressions[0].FieldName;
                sortOrder = Account.MasterTableView.SortExpressions[0].SortOrderAsString();
                foreach (GridSortExpression item in Account.MasterTableView.SortExpressions)
                {
                    sortExpressions += string.Format("{0} {1},", item.FieldName, item.SortOrderAsString());
                }
                sortExpressions = sortExpressions.Trim(',').Trim();
            }

            var filterExpression = Account.MasterTableView.FilterExpression;

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
            pagedParam.FilterExpression = filterExpression;
            pagedParam.SortExpression = sortExpressions.Trim(',').Trim();

            var report = DomainApiClient<Dto.Presentation.PresentationAccountReconcilationReport>.GetByAction<PresentationGridPagedData<Dto.Presentation.PresentationAccountReconcilationReport>>
                ("GetLazyLoadAllSystem", paramter, pagedParam);
            if (isForExport == false)
            {
                Account.VirtualItemCount = report.TotalCount;
            }
            return report.Data;
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                LoadAccountType();
                BindExtraCriteria();
                EndpointUC.Initialize();
                try
                {
                    //if we have the data in the session, then in the postback we will try to save those values back to the grid.
                    if (Session.AccountNavigatorHelper != null && Session.AccountNavigatorHelper.ApplicationId > 0 && Session.AccountNavigatorHelper.ApplicationTypeId > 0)
                    {
                        if (EndpointUC.SelectedApplication == null && Session.AccountNavigatorHelper.ApplicationTypeId > 0)
                            EndpointUC.SelectedApplication = DomainApiClient<Dto.EndPointSystemType>.GetById(Session.AccountNavigatorHelper.ApplicationTypeId);

                        if (EndpointUC.SelectedEndpoint == null && Session.AccountNavigatorHelper.ApplicationId > 0)
                        {
                            EndpointUC.SelectedEndpoint = DomainApiClient<Dto.EndPoint>.GetById(Session.AccountNavigatorHelper.ApplicationId);
                            EntityId = Session.AccountNavigatorHelper.ApplicationId;
                            Account.CurrentPageIndex = Session.AccountNavigatorHelper.PageNumber;
                            Account.PageSize = Session.AccountNavigatorHelper.PageSize;

                            //Filter Formation
                            if (!string.IsNullOrWhiteSpace(Session.AccountNavigatorHelper.FilterExpression))
                            {
                                //if (Session.AccountNavigatorHelper.FilterFieldAndValue.Count > 0)
                                //{
                                //    foreach (KeyValuePair<string, string> filterCondition in Session.AccountNavigatorHelper.FilterFieldAndValue)
                                //    {
                                //        var filterFieldAndValues = filterCondition.Value.Split(';');
                                //        if (filterFieldAndValues.Length == 3)
                                //        {
                                //            GridColumn column = Account.MasterTableView.GetColumnSafe(filterFieldAndValues[0]);
                                //            column.CurrentFilterFunction = (GridKnownFunction)Enum.Parse(typeof(GridKnownFunction), filterFieldAndValues[1]);
                                //            column.CurrentFilterValue = filterFieldAndValues[2];

                                //        }
                                //    }
                                //}                                

                                //Account.MasterTableView.FilterExpression = Session.AccountNavigatorHelper.FilterExpression;

                                if (Session != null && Session.AccountNavigatorHelper != null && Session.AccountNavigatorHelper.FilterExpression.ToUpper() != "All".ToUpper())
                                {
                                    var eValue = (byte)Enum.Parse(typeof(AccountType), Session.AccountNavigatorHelper.FilterExpression);
                                    UserAccountType = eValue;
                                }
                                else
                                {
                                    UserAccountType = -1;
                                }
                            }

                            MappingTypesField.SelectedValue = Session.AccountNavigatorHelper.MappingType;

                            Account.Rebind();
                            if (Account.MasterTableView.Items.Count > 0)
                            {
                                Account.MasterTableView.Items[Session.AccountNavigatorHelper.SelectedRowNumber].Selected = true;
                                GridDataItem Grid = Account.SelectedItems[0] as GridDataItem;
                                this.ContextObjectId = (long)Grid.GetDataKeyValue("ReconciliationId");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        private void LoadAccountType()
        {
            try
            {
                //need a re-visit
                if (Session != null && Session.AccountNavigatorHelper != null)
                {
                    Session.AccountNavigatorHelper.PageNumber = Account.CurrentPageIndex;
                    Session.AccountNavigatorHelper.PageSize = Account.PageSize;
                }
                cmbAccountType.DataSource = Enum.GetValues(typeof(AccountType));
                cmbAccountType.DataBind();
                if (Session.AccountNavigatorHelper != null && !string.IsNullOrWhiteSpace(Session.AccountNavigatorHelper.FilterExpression))
                {
                    cmbAccountType.SelectedValue = Session.AccountNavigatorHelper.FilterExpression;
                }
            }
            catch (Exception ex)
            {

            }
        }
        protected void EndpointUC_ApplicationChanged(EndPointSystemType EndPointSystemType)
        {
            if (Session.AccountNavigatorHelper != null)
            {
                if (EndPointSystemType != null)
                    Session.AccountNavigatorHelper.ApplicationTypeId = EndPointSystemType.Id;
            }
            else
            {
                if (EndPointSystemType != null)
                {
                    AccountNavigatorHelper accountNavigatorHelper = new AccountNavigatorHelper();
                    accountNavigatorHelper.ApplicationTypeId = EndPointSystemType.Id;

                    Session.AccountNavigatorHelper = accountNavigatorHelper;
                }
            }
            EntityId = -1;
            Account.Rebind();
            MappingGrid.Rebind();
        }

        protected void EndpointUC_EndpointChanged(EndPoint EndPoint)
        {
            if (EndPoint != null)
            {
                EntityId = Convert.ToInt64(EndPoint.Id);
                if (Session.AccountNavigatorHelper != null)
                    Session.AccountNavigatorHelper.ApplicationId = EndPoint.Id;
            }
            Account.Rebind();
            MappingGrid.Rebind();
        }

        protected void Account_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        protected void TriggerBind(object sender, EventArgs e)
        {
            Account.Rebind();
            MappingGrid.Rebind();
        }

        protected void Account_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem Grid = Account.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)Grid.GetDataKeyValue("ReconciliationId");
            //on selecting a row we need to store the row which got selected, the page size and the current page number.
            Session.AccountNavigatorHelper.PageNumber = Account.CurrentPageIndex;
            Session.AccountNavigatorHelper.SelectedRowNumber = Grid.ItemIndex;
            Session.AccountNavigatorHelper.PageSize = Account.PageSize;
            //this.EndpointType = (EndpointType)(int)Grid.GetDataKeyValue("EndPointSystemType");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        //RadCombobox filter template
        protected void AccountType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            try
            {
                RadComboBox combo = sender as RadComboBox;
                ViewState[combo.ID] = combo.SelectedValue;

                if (combo.SelectedValue.ToString() != "All")
                {
                    var eValue = (byte)Enum.Parse(typeof(AccountType), e.Value);
                    UserAccountType = eValue;
                    Session.AccountNavigatorHelper.FilterExpression = e.Value;
                }
                else
                {
                    UserAccountType = -1;
                    Account.MasterTableView.FilterExpression = string.Empty;
                    Account.CurrentPageIndex = 0;
                    Session.AccountNavigatorHelper.FilterExpression = e.Value;

                }
                GridDataBind();
                AttributeGridDataBind();
                Account.DataBind();
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Methid will remove the existing filter expression in the column and add the new filter expression.
        /// </summary>
        /// <param name="existingFilter"></param>
        /// <param name="currentFilter"></param>
        /// <param name="currentColumnName"></param>
        /// <returns></returns>
        string getCorrectFilterExpression(string existingFilter, string currentFilter, string currentColumnName)
        {
            try
            {
                if (existingFilter.Trim() == string.Empty) return currentFilter;
                string newfilter = string.Empty;
                var existingFilterSPlit = existingFilter.Split(new string[] { "AND" }, StringSplitOptions.None);

                if (existingFilter.Contains(currentColumnName))
                {
                    if (existingFilterSPlit.Length == 1)
                        return currentFilter;

                    foreach (string splitFilter in existingFilterSPlit)
                    {
                        if (splitFilter.Contains(currentColumnName))
                            newfilter += string.Format("{0} AND ", currentFilter);
                        else
                            newfilter += string.Format("{0} AND ", splitFilter);
                    }
                }
                else
                {
                    newfilter += string.Format("{0} AND {1}", existingFilter, currentFilter);
                }

                newfilter = newfilter.Trim();
                if (newfilter.EndsWith("AND")) newfilter = newfilter.Substring(0, newfilter.Length - 3);
                return newfilter.Trim();
            }
            catch (Exception ex)
            {
                return currentFilter;
            }
        }

        protected void AccountType_PreRender(object sender, EventArgs e)
        {
            RadComboBox rcbFilter = sender as RadComboBox;

            if (Session.AccountNavigatorHelper != null && !string.IsNullOrWhiteSpace(Session.AccountNavigatorHelper.FilterExpression))
            {
                if (rcbFilter != null)
                {
                    rcbFilter.SelectedValue = Session.AccountNavigatorHelper.FilterExpression;
                }
                //foreach (string filterCondition in Session.AccountNavigatorHelper.ComboFilterExpression.Split('~'))
                //{
                //    var filterFieldAndValues = filterCondition.Split(';');
                //    if (filterFieldAndValues.Length == 3)
                //    {
                //        if(rcbFilter.ID == filterFieldAndValues[0])
                //        {
                //            rcbFilter.SelectedValue = filterFieldAndValues[2];
                //        }
                //    }
                //}
            }
        }

        string getCustomFilter(string customFilterExpression)
        {
            var customFilterSet = customFilterExpression.Split(';');
            return string.Format("([{0}] = '{1}')", customFilterSet[0], customFilterSet[2]);
        }

        protected void Account_ItemDataBound(object sender, GridItemEventArgs e)
        {

        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var data = GetAccountList(true);
                if (data != null && data.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(Account.MasterTableView.FilterExpression))
                        data = data.AsEnumerable().AsQueryable().Where(GetFilterExpression(Account.MasterTableView.FilterExpression)).ToList();

                    GenericExport(data, e.ActionType, "Account");
                }
            }
        }

        protected void Account_ItemCommand(object sender, GridCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "ShowInfo")
                {
                    GridDataItem item = e.Item as GridDataItem;
                    long AccountReconId = (long)item.GetDataKeyValue("ReconciliationId");
                    AccountDetailsUC.EntityId = Convert.ToInt64(AccountReconId);
                    AccountDetailsUC.Initialize();
                    UserAccountDetail.VisibleOnPageLoad = true;
                }
                else if (e.CommandName == RadGrid.FilterCommandName)
                {
                    Pair filterPair = (Pair)e.CommandArgument;
                    TextBox filterBox = (e.Item as GridFilteringItem)[filterPair.Second.ToString()].Controls[0] as TextBox;
                    var columnName = filterPair.Second.ToString();
                    if (filterBox != null)
                    {
                        if (Session.AccountNavigatorHelper.FilterFieldAndValue.ContainsKey(columnName))
                        {
                            Session.AccountNavigatorHelper.FilterFieldAndValue[columnName] = string.Format("{0};{1};{2}", columnName, filterPair.First, filterBox.Text);
                        }
                        else
                        {
                            Session.AccountNavigatorHelper.FilterFieldAndValue.Add(columnName, string.Format("{0};{1};{2}", columnName, filterPair.First, filterBox.Text));
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            UserAccountDetail.VisibleOnPageLoad = false;
        }
        #endregion

        protected void MappingGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            AttributeGridDataBind();
        }
        protected override IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Account Name", "AccountName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("EndPointName", "EndPointName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Display Name", "DisplayName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Owner Display Name", "UserDisplayName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Offboarding Action", "OffboardingAction", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Locked", "IsLocked", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Deleted", "IsDeletedBoolean", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Account Type", "AccountType", null));
            return param;
        }
    }
}