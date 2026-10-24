using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Web.UI;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Job.Discovery.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        private static List<Dto.Presentation.PresentationDiscoveryProcess> list = new List<Dto.Presentation.PresentationDiscoveryProcess>();
        #region Helpers
        private void BindEndPointSystems(int startRow, int PageSize, string Filter)
        {
            int count;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("startRow", startRow.ToString());
            parameter.Add("PageSize", PageSize.ToString());
            parameter.Add("Filter", Filter.ToString());

            if (EndpointUC.SelectedApplication == null)
            {
                list = DomainApiClient<Dto.Presentation.PresentationDiscoveryProcess>.
                  GetByAction<List<Dto.Presentation.PresentationDiscoveryProcess>>
                  (DomainApiAction.Discovery.PresentationDiscoveryProcess.GetDiscoveryProcessJobs, parameter);
            }
            else
            {
                long endPointSystemTypeId = Convert.ToInt64(EndpointUC.SelectedApplication.Id);
                parameter.Add("endPointSystemId", endPointSystemTypeId.ToString());
                list = DomainApiClient<Dto.Presentation.PresentationDiscoveryProcess>.
                    GetByAction<List<Dto.Presentation.PresentationDiscoveryProcess>>
                    (DomainApiAction.Discovery.PresentationDiscoveryProcess.GetDiscoveryProcessJobsByEndPointSystemId, parameter);

            }

            this.RadDiscoveryJobs.DataSource = list;
        }

        private void delete()
        {
            var usr = new SessionStatusImpl().User;
            var disovery = DomainApiClient<Dto.DiscoveryProcess>.GetById(this.ContextObjectId.Value);
            bool canThrowError;
            if (!ContextObjectId.HasValue)
                return;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("processId", this.ContextObjectId.Value.ToString());
            canThrowError = DomainApiClient<Dto.DiscoveryProcess>.GetByAction<bool>(DomainApiAction.Discovery.DiscoveryProcess.
                DeleteDiscoveryProcess, parameter);



            if (canThrowError)
                MasterPage.ShowAlert(AlertType.Error, "This Job cant be deleted as it is already being used");
            else
            {
                DI.Chakra.Logger.DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, Logger.LogLevelType.Info, $"Discovery Job Delete:: Job Name:{disovery.ProcessName}", null);

                MasterPage.ShowAlert(AlertType.Success, "DiscoveryJob Deleted Successfully");
            }

            RadDiscoveryJobs.Rebind();
            this.ContextObjectId = null;
        }


        private void ScheduleForRunNow()
        {
            var usr = new SessionStatusImpl().User;

            if (!ContextObjectId.HasValue)
                return;
            SessionStatusImpl session = new SessionStatusImpl();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("discoveryProcessId", Convert.ToString(this.ContextObjectId.Value));

            Dto.Scheduler schedluer = DomainApiClient<Dto.Scheduler>.GetByAction<Dto.Scheduler>
                (DomainApiAction.Schedulers.Scheduler.GetDiscoveryScheduler, parameter);


            schedluer.Occurence = Occurence.RunNow;
            schedluer.ReccurEveryDays = null;
            schedluer.ReccurEveryMonths = null;
            schedluer.ReccurEveryWeeks = null;
            schedluer.StartDate = DateTime.Now.Date;
            schedluer.StartTime = new TimeSpan(DateTime.Now.Hour, (DateTime.Now.Minute + 1), DateTime.Now.Second);
            schedluer.WeekDay = string.Empty;

            Dictionary<string, string> parameterprocess = new Dictionary<string, string>();
            parameterprocess.Add("ProcessId", Convert.ToString(this.ContextObjectId.Value));
            parameterprocess.Add("processType", ProcessType.Discovery.ToString());
            Dto.Process process = DomainApiClient<Dto.Process>.GetByAction<Dto.Process>
                (DomainApiAction.Schedulers.Process.GetByProcessType, parameterprocess);

            if (process != null)
            {
                process.Scheduler = schedluer;
                process.ErrorMessage = "";
                process.ProcessStartAt = null;
                process.ProcessEndAt = null;
                process.Status = "Pending";
                DomainApiClient<Dto.Process>.Save(process);

                DI.Chakra.Logger.DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, Logger.LogLevelType.Info, $"Discovery Job Run Nov:: Job Name:{DiscoveryJobName}", null);
                 

                MasterPage.ShowAlert(AlertType.Success, "Discovery Job updated successfully");
                RadDiscoveryJobs.Rebind();
            }
        }


        private void SaveUserSortAndPageInformation(string GridID, Dto.User user, string ColumnName, string SortDirection,
            int PageIndex, int PageSize, string FilterExpression)
        {
            Dto.UserGridInfo settings = new Dto.UserGridInfo();
            settings.PagePath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            settings.UserId = user.Id;
            settings.GridId = GridID;
            settings.SortColumnName = ColumnName;
            settings.SortDirection = SortDirection;
            settings.PageIndex = PageIndex;
            settings.PageSize = PageSize;
            DomainApiClient<Dto.UserGridInfo>.Save(settings);
        }

        private void SaveFilterChanges(string GridID, Dto.User user, string Operand, string ColumnName, string FilterValue)
        {
            Dto.UserGridFilterInfo filter = new Dto.UserGridFilterInfo();
            filter.UserId = user.Id;
            filter.GridId = GridID;
            filter.Operand = Operand;
            filter.ColumnName = ColumnName;
            filter.FilterValue = FilterValue;
            filter.PagePath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            DomainApiClient<Dto.UserGridFilterInfo>.Save(filter);
        }
        private void DeleteGridFilterInformation(string GridID, long UserID, string ColumnName)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", Convert.ToString(UserID));
            parameter.Add("pagePath", System.Web.HttpContext.Current.Request.Url.AbsolutePath);
            parameter.Add("gridId", GridID);
            parameter.Add("columnName", ColumnName);
            DomainApiClient<Dto.UserGridFilterInfo>.GetByAction<Dto.UserGridFilterInfo>
                (DomainApiAction.Preference.UserFilterInfo.Delete, parameter);
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
            }
            return function;
        }

        private void ClearFilters()
        {
            SessionStatusImpl session = new SessionStatusImpl();
            foreach (object gridColumn in RadDiscoveryJobs.Columns)
            {
                if (gridColumn is GridBoundColumn)
                {
                    GridBoundColumn gridboundColumn = (GridBoundColumn)gridColumn;
                    bool hasValue = gridboundColumn.CurrentFilterValue == "" ? false : true;
                    gridboundColumn.CurrentFilterFunction = GridKnownFunction.NoFilter;
                    gridboundColumn.CurrentFilterValue = "";
                    if (hasValue)
                        DeleteGridFilterInformation(RadDiscoveryJobs.ID, session.User.Id, gridboundColumn.DataField);
                }
            }
        }

        #endregion

        #region PageEvents
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            RadDiscoveryJobs.EnableLinqExpressions = false;
            if (!IsPostBack)
            {
                EndpointUC.Initialize();
                BindEndPointSystems(0, 10, RadDiscoveryJobs.MasterTableView.FilterExpression);
            }
            Datetime();
        }

        private void Datetime()
        {
            SessionStatusImpl session = new SessionStatusImpl();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", session.UserId.ToString());

            List<DI.Chakra.Dto.UserCulturePreference> usrCulPrefenece = DomainApiClient<Dto.UserCulturePreference>.GetByAction<List<Dto.UserCulturePreference>>(DomainApiAction.Preference.UserCulturePreference.GetByUserId, parameter);
            if (usrCulPrefenece.Count > 0)
            {
                RadDiscoveryJobs.Culture.DateTimeFormat.ShortDatePattern = usrCulPrefenece[0].DateFormat;
                RadDiscoveryJobs.datetime = usrCulPrefenece[0].TimeFormat;
            }
        }

        protected void EndpointTypeUC_OnEndpointTypeChanged(Dto.EndPointSystemType EndPointSystemType)
        {
            int startRow = 0;
            ClearFilters();
            RadDiscoveryJobs.MasterTableView.FilterExpression = "";
            string FilterExpression = RadDiscoveryJobs.MasterTableView.FilterExpression;
            string Filter = FilterExpression.Replace(".ToString()", "");
            RadDiscoveryJobs.MasterTableView.CurrentPageIndex = 0;
            BindEndPointSystems(startRow, 10, "");
            RadDiscoveryJobs.Rebind();

        }

        protected void RadDiscoveryJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = RadDiscoveryJobs.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("DiscoveryProcessId");
            DiscoveryJobName = (string)g.GetDataKeyValue("ProcessName");
        }

        public string DiscoveryJobName
        {
            get
            {
                if (ViewState["DiscoveryJobName"] == null)
                    return null;

                return (string)ViewState["DiscoveryJobName"];
            }
            set
            {
                ViewState["DiscoveryJobName"] = value;
            }
        }

        protected void RadDiscoveryJobs_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindEndPointSystems(0, 10, "");

        }

        protected void RadDiscoveryJobs_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == RadGrid.FilterCommandName)
            {
                SessionStatusImpl session = new SessionStatusImpl();
                Pair filterPair = (Pair)e.CommandArgument;

                string Operand = filterPair.First.ToString();//accessing function name
                string ColumnName = filterPair.Second.ToString();
                string filterValue = string.Empty;
                if (!Operand.Equals("NoFilter"))
                {
                    filterValue = RadDiscoveryJobs.MasterTableView.GetColumn(filterPair.Second.ToString()).CurrentFilterValue;
                    SaveFilterChanges(RadDiscoveryJobs.ID, session.User, Operand, ColumnName, filterValue);
                }
                else
                {
                    DeleteGridFilterInformation(RadDiscoveryJobs.ID, session.User.Id, ColumnName);
                }
            }
        }
        #endregion

        #region Delegate
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Discovery Job - Delete");
                delete();
            }
            else if (e.ActionType == ACLAction.RunNow)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Discovery Job - Runnow");
                ScheduleForRunNow();

            //    var list = new List<ServiceParameter>
            //{
            //    new ServiceParameter{ Name = "discoveryProcessId", Value = this.ContextObjectId.Value.ToString() }
            //};
            //    var result = DomainServiceClient.GetByActionAsync<bool>("DiscoveryService/Discover", null, list);
                //var result1 = DomainServiceClient.GetByActionAsync<bool>("DiscoveryService/TestDiscover", null, null);



            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                RadDiscoveryJobs.ExportSettings.FileName = "DiscoveryJob";
                RadDiscoveryJobs.Export(RadDiscoveryJobs, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                RadDiscoveryJobs.Export<List<Dto.Presentation.PresentationDiscoveryProcess>>(list, e.ActionType, "DiscoveryJob");
            }

        }
        #endregion

        protected void RadDiscoveryJobs_Init(object sender, EventArgs e)
        {
            GridFilterMenu menu = RadDiscoveryJobs.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" || menu.Items[i].Text == "Contains" || menu.Items[i].Text == "EqualTo" ||
                    menu.Items[i].Text == "GreaterThan" || menu.Items[i].Text == "LessThan" || menu.Items[i].Text == "StartsWith" ||
                    menu.Items[i].Text == "EndsWith")
                {
                    i++;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }

        protected void Status_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBox combo = sender as RadComboBox;
            ViewState[combo.ID] = combo.SelectedValue;
            if (Convert.ToInt64(combo.SelectedValue.ToString()) > 0)
            {
                string ColumnName = combo.ID;
                string filterExpression = "([" + ColumnName + "] = '" + e.Text + "')";
                RadDiscoveryJobs.MasterTableView.FilterExpression = filterExpression;
            }
            else
            {
                RadDiscoveryJobs.MasterTableView.FilterExpression = string.Empty;
            }
            BindEndPointSystems(0, 10, "");
            RadDiscoveryJobs.Rebind();

        }

        protected void Status_PreRender(object sender, EventArgs e)
        {
            RadComboBox rcbFilter = sender as RadComboBox;
            if (ViewState[rcbFilter.ID] != null)
            {
                rcbFilter.SelectedValue = ViewState[rcbFilter.ID].ToString();
            }
        }

        protected void Occurence_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBox combo = sender as RadComboBox;
            ViewState[combo.ID] = combo.SelectedValue;
            if (combo.SelectedValue.ToString() != "All")
            {
                string ColumnName = combo.ID;
                string filterExpression = "([" + ColumnName + "] = '" + e.Text + "')";
                RadDiscoveryJobs.MasterTableView.FilterExpression = filterExpression;
            }
            else
            {
                RadDiscoveryJobs.MasterTableView.FilterExpression = string.Empty;
            }
            BindEndPointSystems(0, 10, "");
            RadDiscoveryJobs.Rebind();
        }

        protected void Occurence_PreRender(object sender, EventArgs e)
        {
            RadComboBox rcbFilter = sender as RadComboBox;
            if (ViewState[rcbFilter.ID] != null)
            {
                rcbFilter.SelectedValue = ViewState[rcbFilter.ID].ToString();
            }
        }

        protected void RadDiscoveryJobs_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridFilteringItem)
            {
                GridFilteringItem Item = (GridFilteringItem)e.Item;
                RadComboBox combo = (RadComboBox)Item["Occurence"].FindControl("Occurence");
                combo.DataSource = Enum.GetValues(typeof(Occurence));
                combo.DataBind();
            }
        }

        protected void btncancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        public void Refresh()
        {
            RadDiscoveryJobs.Rebind();
            Datetime();
        }

    }
}