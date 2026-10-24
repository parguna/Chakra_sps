using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Web.Master;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.CustomControl
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:IACMRadGridControl runat=server></{0}:IACMRadGridControl>")]
    public class IACMRadGridControl : RadGrid
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]

        private const string V_StrDeclartionHiddenColumns = "V_StrDeclartionHiddenColumns";
        private const string V_UserGridAllInfo = "V_UserGridAllInfo";
        private const string V_SHOW_HIDDEN_COL = "V_SHOW_HIDDEN_COL";
        private const string V_SHOWGRIDFILTER = "V_SHOWGRIDFILTER";
        private const string V_HYPERLINKCOLUMNINDEX = "V_HYPERLINKCOLUMNINDEX";
        private const string V_HYPERLINKBEHAVIOR = "V_HYPERLINKBEHAVIOR";
        private const string V_HYPERLINKURL = "V_HYPERLINKURL";
        private const string V_ROWSELECT = "V_ROWSELECT";
        private const string V_ENTERTOFILTER = "V_ENTERTOFILTER";
        private const string V_ENABLE_ROW_ON_DEMAND = "V_ENABLE_ROW_ON_DEMAND";
        private const string V_ASSIGN_NULL_BEFORE_REBIND = "V_ASSIGN_NULL_BEFORE_REBIND";
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Text"] = value;
            }
        }

        public string datetime
        {
            set
            {
                this.Culture.DateTimeFormat.LongTimePattern = value;
            }
        }

        public bool ShowHiddenColumnInContext
        {
            get
            {
                if (ViewState["V_SHOW_HIDDEN_COL"] != null)
                {
                    return (bool)ViewState["V_SHOW_HIDDEN_COL"];
                }
                else
                {
                    return false;
                }
            }
            set
            {
                ViewState["V_SHOW_HIDDEN_COL"] = value;
            }
        }

        public bool ShowGridFilter
        {
            get
            {
                if (ViewState[V_SHOWGRIDFILTER] == null)
                    return true;
                return (bool)ViewState[V_SHOWGRIDFILTER];
            }
            set
            {
                ViewState[V_SHOWGRIDFILTER] = value;
            }
        }

        public bool EnableRowOnDemandVS
        {
            get
            {
                if (ViewState[V_ENABLE_ROW_ON_DEMAND] == null)
                    return true;
                return (bool)ViewState[V_ENABLE_ROW_ON_DEMAND];
            }
            set
            {
                ViewState[V_ENABLE_ROW_ON_DEMAND] = value;
            }
        }

        public bool EnterToFilterEnable
        {
            get
            {
                if (ViewState[V_ENTERTOFILTER] == null)
                    return false;
                return (bool)ViewState[V_ENTERTOFILTER];
            }
            set
            {
                ViewState[V_ENTERTOFILTER] = value;
            }
        }

        public int HyperLinkColumnIndex
        {
            get
            {
                if (ViewState[V_HYPERLINKCOLUMNINDEX] == null)
                    return -1;
                return (int)ViewState[V_HYPERLINKCOLUMNINDEX];
            }
            set
            {
                ViewState[V_HYPERLINKCOLUMNINDEX] = value;
            }
        }

        public string HyperLinkBehavior
        {
            get
            {
                if (ViewState[V_HYPERLINKBEHAVIOR] == null)
                    return "Detail";
                return Convert.ToString(ViewState[V_HYPERLINKBEHAVIOR]);
            }
            set
            {
                ViewState[V_HYPERLINKBEHAVIOR] = value;
            }
        }

        public string HyperLinkUrl
        {
            get
            {
                if (ViewState[V_HYPERLINKURL] == null)
                {
                    ViewState[V_HYPERLINKURL] = GetUrl();
                }
                return Convert.ToString(ViewState[V_HYPERLINKURL]);
            }
            set
            {
                ViewState[V_HYPERLINKURL] = value;
            }
        }

        public bool DisableRowSelect
        {
            get
            {
                if (ViewState[V_ROWSELECT] == null)
                {
                    return false;
                }
                return Convert.ToBoolean(ViewState[V_ROWSELECT]);
            }
            set
            {
                ViewState[V_ROWSELECT] = value;
            }
        }

        public IACMRadGridControl() : base() { }
        SessionStatusImpl session = new SessionStatusImpl();
        public RadGrid gridcustom { get; set; }

        private Hashtable HideDeclarationColumns
        {
            get
            {
                if (ViewState["V_StrDeclartionHiddenColumns"] != null)
                {
                    return (Hashtable)ViewState["V_StrDeclartionHiddenColumns"];
                }
                else
                {
                    return new Hashtable();
                }
            }
            set
            {
                ViewState["V_StrDeclartionHiddenColumns"] = value;
            }
        }

        private Dto.Presentation.UserGridAllInfo userGridAllInfo
        {
            get
            {
                if (ViewState["V_UserGridAllInfo"] != null)
                {
                    return (Dto.Presentation.UserGridAllInfo)ViewState["V_UserGridAllInfo"];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                ViewState["V_UserGridAllInfo"] = value;
            }
        }

        public bool AssignNullBeforeRebind
        {
            get
            {
                if (ViewState[V_ASSIGN_NULL_BEFORE_REBIND] == null)
                {
                    ViewState[V_ASSIGN_NULL_BEFORE_REBIND] = "false";
                }
                return Convert.ToBoolean(ViewState[V_ASSIGN_NULL_BEFORE_REBIND]);
            }
            set
            {
                ViewState[V_ASSIGN_NULL_BEFORE_REBIND] = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            EnableRowOnDemandVS = true;
            base.OnInit(e);
            gridcustom = this;
            gridcustom.PreRender += gridControl_PreRender;
            gridcustom.ItemCreated += Gridcustom_ItemCreated;
            HttpContext.Current.Session["gridcustom"] = gridcustom.ID;
            HttpContext.Current.Session["gridcustomId"] = gridcustom.ClientID;
            GridLines = GridLines.None;
            AutoGenerateColumns = false;
            EnableHeaderContextMenu = true;
            EnableLinqExpressions = true;
            AllowMultiRowSelection = false;
            AllowFilteringByColumn = true;
            this.Culture.DateTimeFormat.LongTimePattern = string.Empty;
            MasterTableView.AllowFilteringByColumn = true;
            AllowSorting = true;
            AllowPaging = true;
            gridcustom.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top;
            //PageSize = 10;
            gridcustom.PagerStyle.PageSizeControlType = PagerDropDownControlType.RadDropDownList;
            PagerStyle.Mode = GridPagerMode.NextPrevAndNumeric;
            PagerStyle.AlwaysVisible = true;
            GroupingSettings.CaseSensitive = false;
            MasterTableView.AutoGenerateColumns = false;
            //Temporary fix for page size override issue//
            MasterTableView.PageSize = gridcustom.PageSize;

            if (HyperLinkColumnIndex > -1 || DisableRowSelect)
            {
                ClientSettings.Selecting.AllowRowSelect = false;
                ClientSettings.EnablePostBackOnRowClick = false;
            }
            else
            {
                ClientSettings.Selecting.AllowRowSelect = true;
                ClientSettings.EnablePostBackOnRowClick = true;
            }

            ClientSettings.Scrolling.UseStaticHeaders = true;
            ClientSettings.Scrolling.AllowScroll = true;
            ClientSettings.Scrolling.EnableVirtualScrollPaging = false;
            ClientSettings.Scrolling.SaveScrollPosition = true;
            ClientSettings.AllowColumnsReorder = true;
            ClientSettings.ClientEvents.OnColumnHidden = "ColumnHidden";
            ClientSettings.ClientEvents.OnColumnShown = "ColumnShown";
            ClientSettings.ClientEvents.OnHeaderMenuShowing = "OnHeaderMenuShowing";
            ClientSettings.ClientEvents.OnGridCreating = "applyHeaderStyle";
            ClientSettings.ClientEvents.OnScroll = "Scroll";
            ClientSettings.ClientEvents.OnFilterMenuShowing = "filterMenuShowing";
            this.FilterMenu.OnClientShowing = "OnFliterMenuShowing";
            MasterTableView.CommandItemSettings.ShowAddNewRecordButton = false;
            MasterTableView.CommandItemSettings.ShowExportToCsvButton = false;
            MasterTableView.CommandItemSettings.ShowExportToExcelButton = false;
            MasterTableView.CommandItemSettings.ShowExportToPdfButton = false;
            MasterTableView.CommandItemSettings.ShowRefreshButton = false;
            MasterTableView.ShowHeadersWhenNoRecords = true;
            ClientSettings.ReorderColumnsOnClient = false;
            ClientSettings.Resizing.AllowColumnResize = false;
            ClientSettings.Resizing.AllowRowResize = false;
            ClientSettings.AllowColumnsReorder = false;

            gridcustom.PagerStyle.Mode = GridPagerMode.NextPrevAndNumeric;
            gridcustom.PagerStyle.AlwaysVisible = true;
            gridcustom.HeaderContextMenu.ItemCreated += this.HeaderContextMenu_ItemCreated;
            gridcustom.HeaderContextMenu.PreRender += new EventHandler(HeaderContextMenu_PreRender);
            gridcustom.ColumnsReorder += this.Grid_ColumnsReorder;
            gridcustom.MasterTableView.CommandItemTemplate = new IACMGridCommandItemTemplate();

            //GridFilterMenu menu = this.FilterMenu;
            //int i = 0;
            //while (i < menu.Items.Count)
            //{
            //    if (menu.Items[i].Text == "NoFilter" || menu.Items[i].Text == "Contains" || menu.Items[i].Text == "EqualTo" ||
            //        menu.Items[i].Text == "GreaterThan" || menu.Items[i].Text == "LessThan" || menu.Items[i].Text == "StartsWith" || menu.Items[i].Text == "EndsWith")
            //    {
            //        i++;
            //    }
            //    else
            //    {
            //        menu.Items.RemoveAt(i);
            //    }
            //}
        }


        private void Gridcustom_ItemCreated(object sender, GridItemEventArgs e)
        {
            var editMode = e.Item.IsInEditMode;
            var newItem = e.Item is GridEditFormInsertItem;

            if (editMode || newItem)
            {
                EnableRowOnDemandVS = false;
            }

            if (e.Item is GridFilteringItem)
            {
                GridFilteringItem filterItem = e.Item as GridFilteringItem;
                RadDatePicker picker;
                foreach (GridColumn column in Columns)
                {
                    try
                    {
                        if (filterItem[column.UniqueName].Controls.Count > 0 && filterItem[column.UniqueName].Controls[0] is RadDatePicker)
                        {
                            picker = filterItem[column.UniqueName].Controls[0] as RadDatePicker;
                            picker.SharedCalendar.ShowRowHeaders = false;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        public static void RemoveHeaderContextMenuColumnsByUniqueName(Hashtable uniqueNames, IList<RadMenuItem> menuItems)
        {
            for (int topItemIndex = 0; topItemIndex <= menuItems.Count - 1; topItemIndex++)
            {
                if (menuItems[topItemIndex].Text == "Columns")
                {
                    var lowerMenuItemsCount = menuItems[topItemIndex].Items.Count;
                    for (int lowerItemIndex = lowerMenuItemsCount - 1; lowerItemIndex >= 0; lowerItemIndex--)
                    {
                        string strName = menuItems[topItemIndex].Items[lowerItemIndex].Value;
                        string strColName = strName.Split('|')[1];
                        if (uniqueNames[strColName] != null)
                        {
                            menuItems[topItemIndex].Items.RemoveAt(lowerItemIndex);
                        }
                    }
                }
            }
        }
        public void HeaderContextMenu_PreRender(object sender, EventArgs e)
        {
            if (ShowHiddenColumnInContext) return;
            RemoveHeaderContextMenuColumnsByUniqueName(HideDeclarationColumns, gridcustom.HeaderContextMenu.GetAllItems());
        }
        public override GridTableView MasterTableView
        {
            get
            {
                return base.MasterTableView;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            gridcustom = this;
            gridcustom.AllowFilteringByColumn = ShowGridFilter;
            gridcustom.MasterTableView.AllowFilteringByColumn = ShowGridFilter;
            if (HideDeclarationColumns.Count < 1)
            {
                Hashtable _columnsToHide = new Hashtable();
                foreach (GridColumn gridColumn in gridcustom.MasterTableView.RenderColumns)
                {
                    if (!gridColumn.Display && !_columnsToHide.ContainsKey(gridColumn.UniqueName))
                        _columnsToHide.Add(gridColumn.UniqueName, gridColumn.UniqueName);

                    if (EnterToFilterEnable)
                    {
                        gridColumn.CurrentFilterFunction = GridKnownFunction.Contains;
                        gridColumn.AutoPostBackOnFilter = true;
                    }
                }
                if (_columnsToHide.Count == 0)
                    _columnsToHide.Add("No Declaration Columns To hide", "No Declaration Columns To hide");
                HideDeclarationColumns = _columnsToHide;

            }

            //bool editMode = false;
            //foreach (var item in gridcustom.Items)
            //{
            //    if (item is GridEditFormInsertItem || item is GridDataInsertItem)
            //    {
            //        editMode = true;
            //    }
            //    if (editMode)
            //        break;
            //}

            //if (editMode)
            //    EnableRowOnDemand(false);

            if (!Page.IsPostBack)
            {

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", session.User.Id.ToString());
                parameter.Add("gridId", gridcustom.ID);
                parameter.Add("pagePath", System.Web.HttpContext.Current.Request.Url.AbsolutePath);
                Dto.Presentation.UserGridAllInfo gridAllInfo = DomainApiClient<Dto.Presentation.UserGridAllInfo>.GetByAction<Dto.Presentation.UserGridAllInfo>(DomainApiAction.Preference.UserGridAllInfo.GetByUserId, parameter);
                userGridAllInfo = gridAllInfo;
                Dto.UserGridInfo gridInfo = gridAllInfo.UserGridInfo;  // DomainApiClient<Dto.UserGridInfo>.GetByAction<Dto.UserGridInfo>(DomainApiAction.Preference.UserGridInfo.GetByUserId, parameter);

                if (gridInfo != null)
                {
                    GridSortExpression expression = new GridSortExpression();
                    expression.FieldName = gridInfo.SortColumnName;
                    expression.SortOrder = gridInfo.SortDirection == "Ascending" ? GridSortOrder.Ascending : GridSortOrder.Descending;
                    gridcustom.MasterTableView.SortExpressions.AddSortExpression(expression);
                    gridcustom.MasterTableView.PageSize = gridInfo.PageSize;
                    gridcustom.MasterTableView.CurrentPageIndex = gridInfo.PageIndex;
                }

                #region filter
                List<Dto.UserGridFilterInfo> filters = gridAllInfo.UserGridFilterInfo;  // DomainApiClient<Dto.UserGridFilterInfo>.GetByAction<List<Dto.UserGridFilterInfo>>(DomainApiAction.Preference.UserFilterInfo.GetFilterByUserId, parameter);
                string expression1 = "";
                string ANDOperator = "";
                if (filters != null && filters.Count > 0)
                {
                    foreach (Dto.UserGridFilterInfo filter in filters)
                    {
                        Telerik.Web.UI.GridColumn column = gridcustom.MasterTableView.GetColumnSafe(filter.ColumnName);
                        if (column == null)
                            return;
                        column.CurrentFilterFunction = getOperandFunction(filter.Operand);
                        column.CurrentFilterValue = filter.FilterValue;
                        string currentFilterExpression = "";

                        if (filter.Operand == "Contains")
                        {
                            if (gridcustom.EnableLinqExpressions)
                                currentFilterExpression = String.Format(@"(iif({0} == null, """", {1}).ToString().ToUpper().Contains(""{2}"".ToUpper()))", filter.ColumnName, filter.ColumnName, filter.FilterValue);
                            else
                                currentFilterExpression = String.Format(@"([{0}] LIKE '%{2}%')", filter.ColumnName, filter.ColumnName, filter.FilterValue);
                        }
                        else if (filter.Operand == "EqualTo")
                        {
                            if (gridcustom.EnableLinqExpressions)
                            {
                                if (filter.FilterValue.ToLower() == "true" || filter.FilterValue.ToLower() == "false")
                                    currentFilterExpression = String.Format(@"({1} = {2})", filter.ColumnName, filter.ColumnName, filter.FilterValue);
                                else
                                    currentFilterExpression = String.Format(@"(iif({0} == null, """", {1}).ToString().ToUpper() = (""{2}"".ToUpper()))", filter.ColumnName, filter.ColumnName, filter.FilterValue);

                                if (column.UniqueName.ToLower().Contains("id"))
                                {
                                    currentFilterExpression = String.Format(@"({1} = {2})", filter.ColumnName, filter.ColumnName, filter.FilterValue);
                                }

                                if (column.ColumnType == "GridDateTimeColumn")
                                {
                                    DateTime date = Convert.ToDateTime(filter.FilterValue);
                                    DateTime startDate = Convert.ToDateTime(date);
                                    DateTime endDate = Convert.ToDateTime(date).AddDays(1);
                                    DateTime now = DateTime.Now.Date;

                                    currentFilterExpression = string.Format(
                                        "({2} >= DateTime.Parse(\"{0}\")) AND ({2} < DateTime.Parse(\"{1}\"))",
                                        startDate.ToString("G"), endDate.ToString("G"), filter.ColumnName);
                                }
                            }
                            else
                            {
                                currentFilterExpression = String.Format(@"([{0}] = '{2}')", filter.ColumnName, filter.ColumnName, filter.FilterValue);

                                if (column.ColumnType == "GridDateTimeColumn")
                                {
                                    DateTime date = Convert.ToDateTime(filter.FilterValue);
                                    DateTime startDate = Convert.ToDateTime(date);
                                    DateTime endDate = Convert.ToDateTime(date).AddDays(1);
                                    DateTime now = DateTime.Now.Date;

                                    currentFilterExpression = String.Format(@"([{0}] >= '{2}') AND ([{0}] < '{3}')", filter.ColumnName, filter.ColumnName, startDate.ToString("G"), endDate.ToString("G"));
                                }

                                if (column.UniqueName.ToLower().Contains("id"))
                                {
                                    currentFilterExpression = String.Format(@"({1} = {2})", filter.ColumnName, filter.ColumnName, filter.FilterValue);
                                }
                            }
                        }
                        else if (filter.Operand == "EndsWith")
                        {
                            if (gridcustom.EnableLinqExpressions)
                                currentFilterExpression = String.Format(@"(iif({0} == null, """", {1}).ToString().ToUpper().EndsWith(""{2}"".ToUpper()))", filter.ColumnName, filter.ColumnName, filter.FilterValue);
                            else
                                currentFilterExpression = string.Format(@"([{0}] LIKE '%{2}')", filter.ColumnName, filter.ColumnName, filter.FilterValue);
                        }
                        else if (filter.Operand == "StartsWith")
                        {
                            if (gridcustom.EnableLinqExpressions)
                                currentFilterExpression = string.Format(@"(iif({0} == null, """", {1}).ToString().ToUpper().StartsWith(""{2}"".ToUpper()))", filter.ColumnName, filter.ColumnName, filter.FilterValue);
                            else
                                currentFilterExpression = string.Format(@"([{0}] LIKE '{2}%')", filter.ColumnName, filter.ColumnName, filter.FilterValue);
                        }
                        else if (filter.Operand == "GreaterThan")
                        {
                            if (gridcustom.EnableLinqExpressions)
                            {
                                currentFilterExpression = string.Format(@"(iif({0} == null, """", {1}).ToString().ToUpper() >= (""{2}"".ToUpper()))", filter.ColumnName, filter.ColumnName, filter.FilterValue);

                                if (column.UniqueName.ToLower().Contains("id"))
                                {
                                    currentFilterExpression = String.Format(@"({1} > {2})", filter.ColumnName, filter.ColumnName, filter.FilterValue);
                                }

                                if (column.ColumnType == "GridDateTimeColumn")
                                {
                                    DateTime date = Convert.ToDateTime(filter.FilterValue);

                                    currentFilterExpression = string.Format(
                                        "({0} > DateTime.Parse(\"{1}\"))", filter.ColumnName, date.ToString("G"));
                                }
                            }
                            else
                            {
                                currentFilterExpression = string.Format(@"([{0}] > ('{2}'))", filter.ColumnName, filter.ColumnName, filter.FilterValue);

                                if (column.UniqueName.ToLower().Contains("id"))
                                {
                                    currentFilterExpression = String.Format(@"({1} > {2})", filter.ColumnName, filter.ColumnName, filter.FilterValue);
                                }
                            }
                        }
                        else if (filter.Operand == "LessThan")
                        {
                            if (gridcustom.EnableLinqExpressions)
                            {
                                currentFilterExpression = String.Format(@"(iif({0} == null, """", {1}).ToString().ToUpper() <= (""{2}"".ToUpper()))", filter.ColumnName, filter.ColumnName, filter.FilterValue);

                                if (column.UniqueName.ToLower().Contains("id"))
                                {
                                    currentFilterExpression = String.Format(@"({1} < {2})", filter.ColumnName, filter.ColumnName, filter.FilterValue);
                                }

                                if (column.ColumnType == "GridDateTimeColumn")
                                {
                                    DateTime date = Convert.ToDateTime(filter.FilterValue);

                                    currentFilterExpression = string.Format(
                                        "({0} < DateTime.Parse(\"{1}\"))", filter.ColumnName, date.ToString("G"));
                                }
                            }
                            else
                            {
                                currentFilterExpression = String.Format(@"([{0}] < ('{2}'))", filter.ColumnName, filter.ColumnName, filter.FilterValue);
                                if (column.UniqueName.ToLower().Contains("id"))
                                {
                                    currentFilterExpression = String.Format(@"({1} < {2})", filter.ColumnName, filter.ColumnName, filter.FilterValue);
                                }
                            }
                        }
                        else if (filter.Operand == "Custom")
                        {
                            if (gridcustom.EnableLinqExpressions)
                                currentFilterExpression = String.Format(@"(iif({0} == null, """", {1}).ToString() = (""{2}""))", filter.ColumnName, filter.ColumnName, filter.FilterValue);
                            else
                                currentFilterExpression = String.Format(@"([{0}] = '{2}')", filter.ColumnName, filter.ColumnName, filter.FilterValue);
                        }

                        expression1 = expression1 + ANDOperator + currentFilterExpression;
                        ANDOperator = " AND ";
                    }
                }

                if (!String.IsNullOrEmpty(expression1))
                    gridcustom.MasterTableView.FilterExpression = expression1;
                #endregion

                try
                {
                    // GridDateTime Column Filter having issue. 
                    //This is temporary fix.
                    //Setting datasource to null will ensure that need data source method will be called - Suresh
                    if (AssignNullBeforeRebind)
                        gridcustom.DataSource = null;
                    gridcustom.Rebind();
                }
                catch (Exception ex)
                {
                    gridcustom.MasterTableView.FilterExpression = string.Empty;
                    //Setting datasource to null will ensure that need data source method will be called - Suresh
                    if (AssignNullBeforeRebind)
                        gridcustom.DataSource = null;
                    gridcustom.Rebind();
                }
            }
        }

        protected override bool OnNeedDataSource(GridNeedDataSourceEventArgs e)
        {
            base.OnNeedDataSource(e);
            gridcustom = this;
            Hashtable colIndex = new Hashtable();

            List<Dto.UserGridColumnInfo> gridcolumninfo;
            if (userGridAllInfo != null && userGridAllInfo.UserGridColumnInfo != null)
            {
                gridcolumninfo = userGridAllInfo.UserGridColumnInfo;
            }
            else
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", session.User.Id.ToString());
                parameter.Add("gridId", gridcustom.ID);
                parameter.Add("pagePath", System.Web.HttpContext.Current.Request.Url.AbsolutePath);
                gridcolumninfo = DomainApiClient<Dto.UserGridColumnInfo>.GetByAction<List<Dto.UserGridColumnInfo>>(DomainApiAction.Preference.UserColumnInfo.GetcolumnsByUserId, parameter);
            }
            List<string> cols = gridcolumninfo.Select(c => c.ColumnName).ToList<string>();
            if (Page.IsPostBack)
            {
                foreach (GridColumn gridColumn in gridcustom.MasterTableView.RenderColumns)
                {
                    if (gridColumn.Display && !cols.Contains(gridColumn.UniqueName))
                    {
                        cols.Add(gridColumn.UniqueName);
                        colIndex.Add(gridColumn.UniqueName, gridColumn.OrderIndex);
                    }
                    else if (!gridColumn.Display && cols.Contains(gridColumn.UniqueName))
                    {
                        if (ShowHiddenColumnInContext)
                        {
                            cols.Add(gridColumn.UniqueName);
                            colIndex.Add(gridColumn.UniqueName, gridColumn.OrderIndex);
                        }
                        else
                        {
                            cols.Remove(gridColumn.UniqueName);
                            colIndex.Remove(gridColumn.UniqueName);
                        }
                    }
                }
            }
            if (gridcolumninfo.Count > 0)
            {
                int i = 0;
                int columnorder;
                foreach (GridColumn column in gridcustom.Columns)
                {

                    if (column is GridColumn)
                    {
                        GridColumn col = column as GridColumn;
                        string uniqueName = col.UniqueName;
                        if (!cols.Contains(uniqueName))
                        {
                            col.Display = false;
                        }
                        if (column.Display == true || (!column.Display && cols.Contains(uniqueName)))
                        {
                            if ((!column.Display && cols.Contains(uniqueName)))
                            {
                                column.Display = true;
                            }

                            if (colIndex.Contains(uniqueName))
                            {
                                columnorder = Convert.ToInt32(colIndex[uniqueName]);
                                column.OrderIndex = columnorder;
                            }
                            else
                            {
                                columnorder = gridcolumninfo.Where(x => x.ColumnName == column.UniqueName).FirstOrDefault().OrderIndex;
                                column.OrderIndex = columnorder;
                            }
                            i++;
                        }
                    }
                }
            }

            int Pageindex = gridcustom.MasterTableView.CurrentPageIndex;
            string FilterExpression = gridcustom.MasterTableView.FilterExpression;
            string Filterexp = FilterExpression.Replace(".ToString()", "");
            GridSortExpressionCollection expressionCollections = gridcustom.MasterTableView.SortExpressions;
            int PageSize = gridcustom.MasterTableView.PageSize;

            if (expressionCollections.Count > 0)
            {
                foreach (GridSortExpression expression in expressionCollections)
                {
                    SaveUserSortAndPageInformation(gridcustom.ID, session.User, expression.FieldName, expression.SortOrder.ToString(), Pageindex, PageSize, Filterexp);
                }
            }
            else
            {
                if (Page.IsPostBack)
                {
                    SaveUserSortAndPageInformation(gridcustom.ID, session.User, "", "", Pageindex, PageSize, Filterexp);
                }
            }
            return true;
        }
        string GetUrl()
        {
            var sbUrl = new StringBuilder();
            for (int i = 0; i < Page.Request.Url.Segments.Length; i++)
            {
                if (i == 0)
                    sbUrl.Append($"{Page.Request.Url.Scheme}://{Page.Request.Url.Authority}/");
                else if (i == Page.Request.Url.Segments.Length - 1)
                {
                    sbUrl.Append(HyperLinkBehavior + ".aspx?contextId={0}");
                }
                else
                    sbUrl.Append($"{Page.Request.Url.Segments[i]}");
            }
            return sbUrl.ToString();
        }
        protected override void OnItemDataBound(GridItemEventArgs e)
        {
            try
            {
                if (HyperLinkColumnIndex == -1)
                    return;

                if (e.Item is GridDataItem)
                {
                    var column = Columns[HyperLinkColumnIndex];
                    var item = e.Item as GridDataItem;
                    TableCell cell = item[column];
                    HyperLink h1 = new HyperLink();
                    var contextId = Convert.ToInt64(item.GetDataKeyValue("Id"));
                    h1.NavigateUrl = string.Format(HyperLinkUrl, contextId);

                    h1.Text = item[column].Text;
                    cell.Controls.Add(h1);
                }

                //if (!EnableRowOnDemandVS)
                //{
                //    if (e.Item is GridEditFormInsertItem || e.Item is GridDataInsertItem)
                //        EnableRowOnDemandVS = false;
                //}
                //if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
                //{
                //    if (e.Item.DataSetIndex == e.Item.OwnerTableView.DataSourceCount - 1)
                //    {
                //        EnableRowOnDemand();
                //    }
                //}
            }
            catch (Exception ex)
            {

            }
            finally
            {
                base.OnItemDataBound(e);
            }
        }
        protected override void OnItemCommand(GridCommandEventArgs e)
        {
            gridcustom = this;
            var gridFilter = gridcustom.MasterTableView.FilterExpression;
            if (e.CommandName == RadGrid.FilterCommandName)
            {
                try
                {


                    Pair filterPair = (Pair)e.CommandArgument;
                    string operand = filterPair.First.ToString();
                    string columnName = filterPair.Second.ToString();
                    if (operand != null && columnName != null && operand.Equals("EqualTo"))
                    {
                        Telerik.Web.UI.GridColumn column = gridcustom.MasterTableView.GetColumnSafe(columnName);
                        if (column != null && column.ColumnType == "GridDateTimeColumn")
                        {
                            e.Canceled = true;
                            string currentFilterExpression = "";
                            string filterValue = gridcustom.MasterTableView.GetColumn(filterPair.Second.ToString()).CurrentFilterValue;
                            if (gridcustom.EnableLinqExpressions)
                            {
                                DateTime date = Convert.ToDateTime(filterValue);
                                DateTime startDate = Convert.ToDateTime(date);
                                DateTime endDate = Convert.ToDateTime(date).AddDays(1);
                                DateTime now = DateTime.Now.Date;

                                currentFilterExpression = string.Format(
                                    "({2} >= DateTime.Parse(\"{0}\")) AND ({2} < DateTime.Parse(\"{1}\"))",
                                    startDate.ToString("G"), endDate.ToString("G"), columnName);
                            }
                            else
                            {
                                currentFilterExpression = String.Format(@"([{0}] = '{2}')", columnName, columnName, filterValue);
                                if (column.ColumnType == "GridDateTimeColumn")
                                {
                                    DateTime date = Convert.ToDateTime(filterValue);
                                    DateTime startDate = Convert.ToDateTime(date);
                                    DateTime endDate = Convert.ToDateTime(date).AddDays(1);
                                    DateTime now = DateTime.Now.Date;

                                    currentFilterExpression = String.Format(@"([{0}] >= '{2}') AND ([{0}] < '{3}')", columnName, columnName, startDate.ToString("G"), endDate.ToString("G"));
                                }
                            }

                            if (string.IsNullOrEmpty(gridFilter))
                                gridcustom.MasterTableView.FilterExpression = currentFilterExpression;
                            else if (!Regex.Match(gridFilter, @"\b" + columnName + @"\b", RegexOptions.Singleline | RegexOptions.IgnoreCase).Success && !string.IsNullOrEmpty(currentFilterExpression))
                                gridcustom.MasterTableView.FilterExpression += " AND " + currentFilterExpression;
                            else
                            {
                                List<string> finalCondition = new List<string>();
                                var conditionSplit = gridFilter.Split(new string[] { " AND " }, StringSplitOptions.None);
                                foreach (var condition in conditionSplit)
                                {
                                    var matched = Regex.Match(condition, @"\b" + columnName + @"\b", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                                    if (matched != null && matched.Success)
                                        finalCondition.Add(currentFilterExpression);
                                    else
                                        finalCondition.Add(condition);
                                }
                                finalCondition = finalCondition.Where(a => !string.IsNullOrEmpty(a)).Distinct().ToList();
                                gridcustom.MasterTableView.FilterExpression = string.Join(" AND ", finalCondition);
                            }

                            column.CurrentFilterValue = Convert.ToDateTime(filterValue).ToString("dd/MM/yyyy");
                            column.CurrentFilterFunction = GridKnownFunction.EqualTo;
                            gridcustom.Rebind();
                        }
                    }
                    else if (!string.IsNullOrEmpty(gridFilter) && gridFilter.ToLower().Contains("datetime.parse"))
                    {
                        string currentFilterExpression = "";
                        Telerik.Web.UI.GridColumn column = gridcustom.MasterTableView.GetColumnSafe(columnName);
                        string filterValue = gridcustom.MasterTableView.GetColumn(filterPair.Second.ToString()).CurrentFilterValue;
                        if (operand != null && operand == "Contains")
                        {
                            if (gridcustom.EnableLinqExpressions)
                                currentFilterExpression = String.Format(@"(iif({0} == null, """", {1}).ToString().ToUpper().Contains(""{2}"".ToUpper()))", columnName, columnName, filterValue);
                            else
                                currentFilterExpression = String.Format(@"([{0}] LIKE '%{2}%')", columnName, columnName, filterValue);
                        }
                        else if (operand != null && operand == "EqualTo")
                        {
                            if (gridcustom.EnableLinqExpressions)
                            {
                                currentFilterExpression = String.Format(@"(iif({0} == null, """", {1}).ToString().ToUpper() = (""{2}"".ToUpper()))", columnName, columnName, filterValue);

                                if (column.UniqueName.ToLower().Contains("id"))
                                {
                                    currentFilterExpression = String.Format(@"({1} = {2})", columnName, columnName, filterValue);
                                }
                            }
                            else
                            {
                                currentFilterExpression = String.Format(@"([{0}] = '{2}')", columnName, columnName, filterValue);

                                if (column.UniqueName.ToLower().Contains("id"))
                                {
                                    currentFilterExpression = String.Format(@"({1} = {2})", columnName, columnName, filterValue);
                                }
                            }
                        }
                        else if (operand != null && operand == "EndsWith")
                        {
                            if (gridcustom.EnableLinqExpressions)
                                currentFilterExpression = String.Format(@"(iif({0} == null, """", {1}).ToString().ToUpper().EndsWith(""{2}"".ToUpper()))", columnName, columnName, filterValue);
                            else
                                currentFilterExpression = string.Format(@"([{0}] LIKE '%{2}')", columnName, columnName, filterValue);
                        }
                        else if (operand != null && operand == "StartsWith")
                        {
                            if (gridcustom.EnableLinqExpressions)
                                currentFilterExpression = string.Format(@"(iif({0} == null, """", {1}).ToString().ToUpper().StartsWith(""{2}"".ToUpper()))", columnName, columnName, filterValue);
                            else
                                currentFilterExpression = string.Format(@"([{0}] LIKE '{2}%')", columnName, columnName, filterValue);
                        }
                        else if (operand != null && operand == "GreaterThan")
                        {
                            if (gridcustom.EnableLinqExpressions)
                            {
                                currentFilterExpression = string.Format(@"(iif({0} == null, """", {1}).ToString().ToUpper() >= (""{2}"".ToUpper()))", columnName, columnName, filterValue);

                                if (column.ColumnType == "GridDateTimeColumn")
                                {
                                    DateTime date = Convert.ToDateTime(filterValue);

                                    currentFilterExpression = string.Format(
                                        "({0} > DateTime.Parse(\"{1}\"))", columnName, date.ToString("G"));
                                }

                                if (column.UniqueName.ToLower().Contains("id"))
                                {
                                    currentFilterExpression = String.Format(@"({1} > {2})", columnName, columnName, filterValue);
                                }
                            }
                            else
                            {
                                currentFilterExpression = string.Format(@"([{0}] > ('{2}'))", columnName, columnName, filterValue);

                                if (column.UniqueName.ToLower().Contains("id"))
                                {
                                    currentFilterExpression = String.Format(@"({1} > {2})", columnName, columnName, filterValue);
                                }
                            }
                        }
                        else if (operand != null && operand == "LessThan")
                        {
                            if (gridcustom.EnableLinqExpressions)
                            {
                                currentFilterExpression = String.Format(@"(iif({0} == null, """", {1}).ToString().ToUpper() <= (""{2}"".ToUpper()))", columnName, columnName, filterValue);

                                if (column.ColumnType == "GridDateTimeColumn")
                                {
                                    DateTime date = Convert.ToDateTime(filterValue);

                                    currentFilterExpression = string.Format(
                                        "({0} < DateTime.Parse(\"{1}\"))", columnName, date.ToString("G"));
                                }

                                if (column.UniqueName.ToLower().Contains("id"))
                                {
                                    currentFilterExpression = String.Format(@"({1} < {2})", columnName, columnName, filterValue);
                                }
                            }
                            else
                            {
                                currentFilterExpression = String.Format(@"([{0}] < ('{2}'))", columnName, columnName, filterValue);

                                if (column.UniqueName.ToLower().Contains("id"))
                                {
                                    currentFilterExpression = String.Format(@"({1} < {2})", columnName, columnName, filterValue);
                                }
                            }
                        }
                        else if (operand != null && operand == "Custom")
                        {
                            if (gridcustom.EnableLinqExpressions)
                                currentFilterExpression = String.Format(@"(iif({0} == null, """", {1}).ToString() = (""{2}""))", columnName, columnName, filterValue);
                            else
                                currentFilterExpression = String.Format(@"([{0}] = '{2}')", columnName, columnName, filterValue);
                        }

                        if (!Regex.Match(gridFilter, @"\b" + columnName + @"\b", RegexOptions.Singleline | RegexOptions.IgnoreCase).Success && !string.IsNullOrEmpty(currentFilterExpression))
                            gridcustom.MasterTableView.FilterExpression += " AND " + currentFilterExpression;
                        else
                        {
                            List<string> finalCondition = new List<string>();
                            var conditionSplit = gridFilter.Split(new string[] { " AND " }, StringSplitOptions.None);
                            foreach (var condition in conditionSplit)
                            {
                                var matched = Regex.Match(condition, @"\b" + columnName + @"\b", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                                if (matched != null && matched.Success)
                                    finalCondition.Add(currentFilterExpression);
                                else
                                    finalCondition.Add(condition);
                            }
                            finalCondition = finalCondition.Where(a => !string.IsNullOrEmpty(a)).Distinct().ToList();
                            gridcustom.MasterTableView.FilterExpression = string.Join(" AND ", finalCondition);
                        }

                        if (operand != null && operand == "NoFilter")
                            column.CurrentFilterValue = "";
                        else
                            column.CurrentFilterValue = filterValue;

                        column.CurrentFilterFunction = getOperandFunction(operand);

                        e.Canceled = true;
                        gridcustom.Rebind();
                    }
                }
                catch (Exception)
                { }
            }
            base.OnItemCommand(e);
            if (e.CommandName == RadGrid.FilterCommandName)
            {
                Pair filterPair = (Pair)e.CommandArgument;

                string Operand = filterPair.First.ToString();//accessing function name
                string ColumnName = filterPair.Second.ToString();
                string filterValue = string.Empty;
                if (!Operand.Equals("NoFilter"))
                {
                    filterValue = gridcustom.MasterTableView.GetColumn(filterPair.Second.ToString()).CurrentFilterValue;
                    SaveFilterChanges(gridcustom.ID, session.User, Operand, ColumnName, filterValue);
                }
                else
                {
                    DeleteGridFilterInformation(gridcustom.ID, session.User.Id, ColumnName);
                }
            }
            if (e.CommandName == "gridFilterClear")
            {
                ClearFilters();
                gridcustom.MasterTableView.FilterExpression = string.Empty;
                gridcustom.Rebind();
            }
            if (e.CommandName == "Cancel")
            {
                EnableRowOnDemandVS = true;
            }
        }

        protected void Grid_ColumnsReorder(object sender, GridColumnsReorderEventArgs e)
        {

            DeleteGridColumnInformation(gridcustom.ID, session.User.Id);

            var sourcePosition = e.Source.OrderIndex;
            string sourceColumnName = e.Source.UniqueName;

            var targetPosition = e.Target.OrderIndex;
            string targetColumnName = e.Target.UniqueName;

            Hashtable columns = new Hashtable();
            columns.Add(targetColumnName, sourcePosition.ToString());
            columns.Add(sourceColumnName, targetPosition.ToString());

            if (gridcustom.ClientSettings.AllowColumnsReorder)
            {

                foreach (GridColumn gridColumn in MasterTableView.RenderColumns)
                {
                    GridColumnCollection columnscount = MasterTableView.Columns;
                    GridColumn c = columnscount.FindByUniqueNameSafe(gridColumn.UniqueName);
                    if (c != null)
                    {
                        int start = c.OrderIndex;
                        if (!columns.ContainsKey(gridColumn.UniqueName))
                        {
                            columns.Add(gridColumn.UniqueName, start.ToString());
                        }
                        else
                        {
                            if (c.UniqueName == targetColumnName)
                                gridColumn.OrderIndex = sourcePosition;
                            if (c.UniqueName == sourceColumnName)
                                gridColumn.OrderIndex = targetPosition;
                        }

                    }
                    SaveColumnChanges(gridcustom.ID, session.User.Id, gridColumn.UniqueName, gridColumn.OrderIndex);
                    gridcustom.MasterTableView.SwapColumns(sourceColumnName, targetColumnName);
                }
            }
            gridcustom.Rebind();
        }

        private void SaveUserSortAndPageInformation(string GridID, Dto.User user, string ColumnName, string SortDirection,
            int PageIndex, int PageSize, string FilterExpression)
        {
            Dto.UserGridInfo settings;
            if (userGridAllInfo != null && userGridAllInfo.UserGridInfo != null)
            {
                settings = userGridAllInfo.UserGridInfo;
            }
            else
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", user.Id.ToString());
                parameter.Add("gridId", GridID);
                parameter.Add("pagePath", System.Web.HttpContext.Current.Request.Url.AbsolutePath);
                settings = DomainApiClient<Dto.UserGridInfo>.GetByAction<Dto.UserGridInfo>(DomainApiAction.Preference.UserGridInfo.GetByUserId, parameter);
            }

            if (settings == null)
                settings = new Dto.UserGridInfo();
            settings.PagePath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            settings.UserId = user.Id;
            settings.GridId = GridID;
            settings.SortColumnName = ColumnName;
            settings.SortDirection = SortDirection;
            settings.PageIndex = PageIndex;
            settings.PageSize = PageSize;
            DomainApiClient<Dto.UserGridInfo>.Save(settings);
        }

        public void SaveFilterChanges(string GridID, Dto.User user, string Operand, string ColumnName, string FilterValue)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", user.Id.ToString());
            parameter.Add("gridId", GridID);
            parameter.Add("pagePath", System.Web.HttpContext.Current.Request.Url.AbsolutePath);
            parameter.Add("columnName", ColumnName);
            Dto.UserGridFilterInfo filter = DomainApiClient<Dto.UserGridFilterInfo>.GetByAction<Dto.UserGridFilterInfo>(DomainApiAction.Preference.UserFilterInfo.GetFilterByUserId, parameter);
            if (filter == null)
                filter = new Dto.UserGridFilterInfo();
            filter.UserId = user.Id;
            filter.GridId = GridID;
            filter.Operand = Operand;
            filter.ColumnName = ColumnName;
            filter.FilterValue = FilterValue;
            filter.PagePath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            DomainApiClient<Dto.UserGridFilterInfo>.Save(filter);
        }

        public void DeleteGridColumnInformation(string GridID, long UserID)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", Convert.ToString(UserID));
            parameter.Add("pagePath", System.Web.HttpContext.Current.Request.Url.AbsolutePath);
            parameter.Add("gridId", GridID);
            DomainApiClient<Dto.UserGridColumnInfo>.GetByAction<Dto.UserGridColumnInfo>
                (DomainApiAction.Preference.UserColumnInfo.Delete, parameter);
        }

        public void SaveColumnChanges(string GridID, long userId, string ColumnName, int columnOrder)
        {
            if (columnOrder > -1)
            {
                Dto.UserGridColumnInfo columns = new Dto.UserGridColumnInfo();
                columns.UserId = session.User.Id;
                columns.GridId = GridID;
                columns.ColumnName = ColumnName;
                columns.OrderIndex = columnOrder;
                columns.PagePath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
                DomainApiClient<Dto.UserGridColumnInfo>.Save(columns);
            }
        }

        public void DeleteGridFilterInformation(string GridID, long UserID, string ColumnName)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", Convert.ToString(UserID));
            parameter.Add("pagePath", System.Web.HttpContext.Current.Request.Url.AbsolutePath);
            parameter.Add("gridId", GridID);
            parameter.Add("columnName", ColumnName);
            DomainApiClient<Dto.UserGridFilterInfo>.GetByAction<Dto.UserGridFilterInfo>
                (DomainApiAction.Preference.UserFilterInfo.Delete, parameter);
        }


        private void ClearFilters()
        {
            foreach (object gridColumn in gridcustom.Columns)
            {
                if (gridColumn is GridBoundColumn)
                {
                    GridBoundColumn gridboundColumn = (GridBoundColumn)gridColumn;
                    bool hasValue = gridboundColumn.CurrentFilterValue == "" ? false : true;
                    gridboundColumn.CurrentFilterFunction = GridKnownFunction.NoFilter;
                    gridboundColumn.CurrentFilterValue = "";
                    if (hasValue)
                        DeleteGridFilterInformation(gridcustom.ID, session.User.Id, gridboundColumn.DataField);
                }
                try
                {
                    GridColumn column = gridColumn as GridColumn;
                    if (column != null)
                    {
                        column.CurrentFilterFunction = GridKnownFunction.NoFilter;
                        column.CurrentFilterValue = String.Empty;
                    }
                }
                catch
                {
                }
            }
        }

        private void HeaderContextMenu_ItemCreated(object sender, RadMenuEventArgs e)
        {
            switch ((e.Item.Text))
            {
                case "Sort Ascending":
                    e.Item.Visible = false;
                    break;
                case "Sort Descending":
                    e.Item.Visible = false;
                    break;
                case "Clear Sorting":
                    e.Item.Visible = false;
                    break;
                case "Group By":
                    e.Item.Visible = false;
                    break;
                case "Ungroup":
                    e.Item.Visible = false;
                    break;
            }
        }

        private GridKnownFunction getOperandFunction(string OperandName)
        {
            GridKnownFunction function = GridKnownFunction.NoFilter;
            switch (OperandName)
            {
                case "EqualTo":
                    function = GridKnownFunction.EqualTo;
                    break;
                case "Contains":
                    function = GridKnownFunction.Contains;
                    break;
                case "NotEqualTo":
                    function = GridKnownFunction.NotEqualTo;
                    break;
                case "StartsWith":
                    function = GridKnownFunction.StartsWith;
                    break;
                case "EndsWith":
                    function = GridKnownFunction.EndsWith;
                    break;
                case "GreaterThan":
                    function = GridKnownFunction.GreaterThan;
                    break;
                case "LessThan":
                    function = GridKnownFunction.LessThan;
                    break;
                case "Custom":
                    function = GridKnownFunction.Custom;
                    break;
            }
            return function;
        }

        public void Export<T>(T Datasource, ACLAction actionType, string fileName, bool exportAllColumns = true)
        {
            RadGrid exportGrid = new RadGrid();
            exportGrid.AllowCustomPaging = false;
            exportGrid.AutoGenerateColumns = true;
            if (!exportAllColumns)
            {
                exportGrid = gridcustom;
            }
            exportGrid.ExportSettings.FileName = fileName;
            exportGrid.DataSource = Datasource;
            exportGrid.DataBind();
            GridColumn[] gridColumns = (gridcustom.MasterTableView.RenderColumns.Where(x => !exportGrid.MasterTableView.RenderColumns.Any(y => y.UniqueName == x.UniqueName)).GroupBy(x => x.UniqueName).Select(x => x.First()).ToArray());
            gridColumns.All(x => { exportGrid.MasterTableView.Columns.Add(x); return true; });
            List<string> List = gridcustom.MasterTableView.RenderColumns.Select(x => x.UniqueName).ToList();

            if (!exportAllColumns)
            {
                List = gridcustom.MasterTableView.RenderColumns.Where(a => a.Display == true).Select(x => x.UniqueName).ToList();
            }


            foreach (string lst in List)
            {
                GridColumn c = exportGrid.MasterTableView.RenderColumns.Where(x => x.UniqueName == lst).FirstOrDefault();
                if (c != null)
                {
                    c.OrderIndex = c.OrderIndex;
                    c.HeaderText = c.HeaderText;
                }
            }


            if (gridcustom.MasterTableView.EnableHeaderContextMenu)
            {
                List<string> colNames = new List<string>();
                foreach (GridColumn col in exportGrid.MasterTableView.RenderColumns)
                {
                    GridColumnCollection columnscount = MasterTableView.Columns;
                    GridColumn c = columnscount.FindByUniqueNameSafe(col.UniqueName);
                    if (c != null && !HideDeclarationColumns.Contains(col.UniqueName) && (colNames == null || !colNames.Contains(col.UniqueName.ToUpper())))
                    {
                        if (col is GridBoundColumn && col.UniqueName.ToLower().Contains("date"))
                        {
                            ((GridBoundColumn)col).DataType = Type.GetType("System.DateTime");
                            ((GridBoundColumn)col).DataFormatString = "{0:dd/MM/yyyy hh:mm:ss tt}";
                        }
                        colNames.Add(col.UniqueName.ToUpper());
                        Page.Controls.Add(exportGrid);
                    }
                    else
                        col.Display = false;
                }
            }
            Export(exportGrid, actionType);
        }

        public void Export(RadGrid gridControl, ACLAction actionType)
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
                            gridControl.ExportSettings.ExportOnlyData = false;
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

        public void DownloadTemplate(RadGrid gridControl, ACLAction actionType)
        {
            try
            {
                foreach (GridColumn item in gridControl.MasterTableView.Columns)
                {
                    if (item.HeaderText == string.Empty)
                    {
                        item.Visible = false;
                    }
                }
                gridControl.ExportSettings.IgnorePaging = true;
                gridControl.GridLines = GridLines.Both;
                gridControl.AllowFilteringByColumn = true;
                gridControl.ExportSettings.ExportOnlyData = true;
                gridControl.ExportSettings.HideStructureColumns = true;
                gridControl.ExportCellFormatting += new EventHandler<ExportCellFormattingEventArgs>(gridControl_ExportCellFormatting);
                gridControl.MasterTableView.ExportToCSV();
            }
            catch
            {
                throw;
            }
        }

        void gridControl_ExportCellFormatting(object sender, ExportCellFormattingEventArgs e)
        {
            if ((e.FormattedColumn.UniqueName).ToLower().Contains("date"))
            {
                e.Cell.Style["mso-number-format"] = @"dd/MM/yyyy";
            }
            GridDataItem item = e.Cell.Parent as GridDataItem;
            e.Cell.HorizontalAlign = HorizontalAlign.Left;
        }

        public ProtoMast4 MasterPage
        {
            get
            {
                return (ProtoMast4)Page.Master;
            }
        }

        protected void gridControl_PreRender(object sender, EventArgs e)
        {
            EnableRowOnDemand();
        }

        void EnableRowOnDemand()
        {
            if (!EnableRowOnDemandVS || HyperLinkColumnIndex > -1 || DisableRowSelect)
            {
                ClientSettings.Selecting.AllowRowSelect = false;
                ClientSettings.EnablePostBackOnRowClick = false;
            }
            else
            {
                ClientSettings.Selecting.AllowRowSelect = true;
                ClientSettings.EnablePostBackOnRowClick = true;
            }
        }
        public void ClearFilter()
        {
            foreach (object gridColumn in this.Columns)
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
    }

}