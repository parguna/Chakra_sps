using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Job.Compliance.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        private static List<Dto.Presentation.PresentationComplianceProcess> list = new List<Dto.Presentation.PresentationComplianceProcess>();
        #region Class Member Variables

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            RadComplianceJobs.EnableLinqExpressions = false;
            if (!IsPostBack)
            {
                BindComplianceJobs(0, 10, RadComplianceJobs.MasterTableView.FilterExpression);
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
                RadComplianceJobs.Culture.DateTimeFormat.ShortDatePattern = usrCulPrefenece[0].DateFormat;
                RadComplianceJobs.datetime = usrCulPrefenece[0].TimeFormat;
            }
        }
        private void BindComplianceJobs(int startRow, int PageSize, string Filter)
        {          
            
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("startRow", startRow.ToString());
            parameter.Add("PageSize", PageSize.ToString());
            parameter.Add("filter", Filter);

            list = DomainApiClient<Dto.Presentation.PresentationComplianceProcess>.
                GetByAction<List<Dto.Presentation.PresentationComplianceProcess>>
                (DomainApiAction.Complaince.Presentation.PresentationComlainceProcess.GetComplianceProcessJobs, parameter);

            foreach (Dto.Presentation.PresentationComplianceProcess job in list)
            {
                job.StartDateTime = job.StartDateTime.Split('.').Count() > 0 ? job.StartDateTime.Split('.')[0] : job.StartDateTime;
            }
            this.RadComplianceJobs.DataSource = list;
        }

        protected void RadComplianceJobs_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindComplianceJobs(0, 10, "");
        }

        protected void RadComplianceJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = RadComplianceJobs.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("ComplianceProcessId");
        }

        protected void RadComplianceJobs_Init(object sender, EventArgs e)
        {
            GridFilterMenu menu = RadComplianceJobs.FilterMenu;
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

        protected void RadComplianceJobs_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
        }

        private void delete()
        {
            if (!ContextObjectId.HasValue)
                return;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("processId", this.ContextObjectId.Value.ToString());
            DomainApiClient<Dto.ComplianceProcess>.GetByAction<object>
                (DomainApiAction.Complaince.ComplianceProcess.DeleteComplianceProcess, parameter);
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "ComplianceJob Deleted Successfully");
            RadComplianceJobs.Rebind();
        }

        private void ScheduleForRunNow()
        {
            if (!ContextObjectId.HasValue)
                return;
            SessionStatusImpl session = new SessionStatusImpl();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("complianceProcessId", Convert.ToString(this.ContextObjectId.Value));
            Dto.Scheduler scheduler = DomainApiClient<Dto.Scheduler>.GetByAction<Dto.Scheduler>
                (DomainApiAction.Schedulers.Scheduler.GetComplianceScheduler, parameter);

            scheduler.Occurence = Occurence.RunNow;
            scheduler.ReccurEveryDays = null;
            scheduler.ReccurEveryMonths = null;
            scheduler.ReccurEveryWeeks = null;
            scheduler.StartDate = DateTime.Now.Date;
            scheduler.StartTime = new TimeSpan(DateTime.Now.Hour, (DateTime.Now.Minute + 1), DateTime.Now.Second);
            scheduler.WeekDay = string.Empty;

            Dictionary<string, string> parameterprocess = new Dictionary<string, string>();
            parameterprocess.Add("ProcessId", Convert.ToString(this.ContextObjectId.Value));
            parameterprocess.Add("processType", ProcessType.Compliance.ToString());
            Dto.Process process = DomainApiClient<Dto.Process>.GetByAction<Dto.Process>
                (DomainApiAction.Schedulers.Process.GetByProcessType, parameterprocess);

            if (process != null)
            {
                process.Scheduler = scheduler;
                process.ProcessStartAt = null;
                process.ProcessEndAt = null;
                process.Status = "Pending";
                DomainApiClient<Dto.Process>.Save(process);
                MasterPage.ShowAlert(AlertType.Success, "Compliance Job updated successfully");
                RadComplianceJobs.Rebind();
            }
        }

        #region Delegate
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Compliance Job - Delete");
                delete();
            }
            else if (e.ActionType == ACLAction.RunNow)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Compliance Job - Runno");
                ScheduleForRunNow();
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                RadComplianceJobs.ExportSettings.FileName = "ComplianceJob";
                RadComplianceJobs.Export(RadComplianceJobs, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                RadComplianceJobs.Export<List<Dto.Presentation.PresentationComplianceProcess>>(list, e.ActionType, "ComplianceJob");
            }

        }
        #endregion

        protected void Status_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBox combo = sender as RadComboBox;
            ViewState[combo.ID] = combo.SelectedValue;
            if (Convert.ToInt32(combo.SelectedValue.ToString()) > 0)
            {
                string ColumnName = combo.ID;
                string filterExpression = "([" + ColumnName + "] = '" + e.Text + "')";
                RadComplianceJobs.MasterTableView.FilterExpression = filterExpression;
            }
            else
            {
                RadComplianceJobs.MasterTableView.FilterExpression = string.Empty;
            }
            BindComplianceJobs(0, 10, "");
            RadComplianceJobs.DataBind();
        }

        protected void Status_PreRender(object sender, EventArgs e)
        {
            RadComboBox rcbFilter = sender as RadComboBox;
            if (ViewState[rcbFilter.ID] != null)
            {
                rcbFilter.SelectedValue = ViewState[rcbFilter.ID].ToString();
            }
        }

        protected void RadComplianceJobs_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridFilteringItem)
            {
                GridFilteringItem Item = (GridFilteringItem)e.Item;
                RadComboBox combo = (RadComboBox)Item["Occurence"].FindControl("Occurence");
                combo.DataSource = Enum.GetValues(typeof(Occurence));
                combo.DataBind();
            }
        }

        protected void Occurence_PreRender(object sender, EventArgs e)
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
                RadComplianceJobs.MasterTableView.FilterExpression = filterExpression;
            }
            else
            {
                RadComplianceJobs.MasterTableView.FilterExpression = string.Empty;
            }
            BindComplianceJobs(0, 10, RadComplianceJobs.MasterTableView.FilterExpression);
            RadComplianceJobs.DataBind();
        }

        protected void btncancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        public void Refresh()
        {
          RadComplianceJobs.Rebind();
        }



        #endregion
    }
}
