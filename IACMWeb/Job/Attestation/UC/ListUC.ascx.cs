using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Data;
using DI.Chakra.Dto;
using DI.Chakra.Logger;

namespace DI.Chakra.Web.Job.Attestation.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {

        #region Members

        private static List<Dto.Presentation.PresentationJobListAttestation> list;
        SessionStatusImpl session = new SessionStatusImpl();

        #endregion

        #region Properties

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
        public int OccurenceSelectedValue 
        {
            get
            {
                if (ViewState["OccurenceSelectedValue"] != null)
                    return Convert.ToInt16(ViewState["OccurenceSelectedValue"]);

                return 0;
            }
            set
            {
                ViewState["OccurenceSelectedValue"] = value;
            }
        }

        #endregion 

        #region Methods

        private void BindAttestationGrid(int startRow, int pageSize, string filter) 
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("pageSize", string.IsNullOrEmpty(filterExpression) ? RadAttestationJobs.PageSize.ToString() : string.Empty);
            parameter.Add("pageNumber", string.IsNullOrEmpty(filterExpression) ? (RadAttestationJobs.CurrentPageIndex + 1).ToString() : string.Empty);
            parameter.Add("filter", filter.ToString());

            list = DomainApiClient<Dto.Presentation.PresentationJobListAttestation>.GetByAction<List<Dto.Presentation.PresentationJobListAttestation>>(DomainApiAction.Attestation.Presentation.PresentationJobListAttestation.GetAttestationProcessJobs, parameter);
            this.RadAttestationJobs.DataSource = list;
            this.RadAttestationJobs.VirtualItemCount = list.Count;
        }

        private void Delete()
        {
            bool canThrowError;
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("processId", ContextObjectId.Value.ToString());
            canThrowError = DomainApiClient<Dto.AttestationProcess>.GetByAction<bool>(DomainApiAction.Attestation.AttestationProcess.
                            DeleteAttestationProcess, parameter);
            if (canThrowError)
                MasterPage.ShowAlert(AlertType.Error, "This Job cannot be deleted as it is already being used");
            else
                MasterPage.ShowAlert(AlertType.Success, "Attestation Job Deleted Successfully");
            RadAttestationJobs.Rebind();
            this.ContextObjectId = null;
        }

        private void ScheduleForRunNow()
        {
            if (!ContextObjectId.HasValue)
                return;
            SessionStatusImpl session = new SessionStatusImpl();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("attestationProcessId", Convert.ToString(this.ContextObjectId.Value));

            Dto.Scheduler schedluer = DomainApiClient<Dto.Scheduler>.GetByAction<Dto.Scheduler>(DomainApiAction.Schedulers.Scheduler.GetAttestationScheduler, parameter);

            schedluer.Occurence = Occurence.RunNow;
            schedluer.ReccurEveryDays = null;
            schedluer.ReccurEveryMonths = null;
            schedluer.ReccurEveryWeeks = null;
            schedluer.StartDate = DateTime.Now.Date;
            schedluer.StartTime = new TimeSpan(DateTime.Now.Hour, (DateTime.Now.Minute + 1), DateTime.Now.Second);
            schedluer.WeekDay = string.Empty;

            Dictionary<string, string> parameterprocess = new Dictionary<string, string>();
            parameterprocess.Add("ProcessId", Convert.ToString(this.ContextObjectId.Value));
            parameterprocess.Add("processType", Convert.ToString(ProcessType.Attestation.ToString()));

            Dto.Process process = DomainApiClient<Dto.Process>.GetByAction<Dto.Process>(DomainApiAction.Schedulers.Process.GetByProcessType, parameterprocess);
            if (process != null)
            {
                process.Scheduler = schedluer;
                process.ErrorMessage = "";
                process.ProcessStartAt = null;
                process.ProcessEndAt = null;
                process.Status = Enum.GetName(typeof(SchedulerStatus), SchedulerStatus.Pending);
                DomainApiClient<Dto.Process>.Save(process);
                MasterPage.ShowAlert(AlertType.Success, "Attestation Job updated successfully");
                RadAttestationJobs.Rebind();
            }
        }
        public void Refresh()
        {
            RadAttestationJobs.Rebind();
        }

        #endregion

        #region PageEvents

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            RadAttestationJobs.EnableLinqExpressions = false;
            if (!IsPostBack)
            {
                //BindAttestationGrid(0, 10, (OccurenceSelectedValue >= 0 ? (Occurence)OccurenceSelectedValue : Occurence.All).ToString());
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("filter", (OccurenceSelectedValue >= 0 ? (Occurence)OccurenceSelectedValue : Occurence.All).ToString());

                list = DomainApiClient<Dto.Presentation.PresentationJobListAttestation>.GetByAction<List<Dto.Presentation.PresentationJobListAttestation>>(DomainApiAction.Attestation.Presentation.PresentationJobListAttestation.GetAttestationJobs, parameter);
                this.RadAttestationJobs.DataSource = list;
                this.RadAttestationJobs.VirtualItemCount = list.Count;
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
                RadAttestationJobs.Culture.DateTimeFormat.ShortDatePattern = usrCulPrefenece[0].DateFormat;
                RadAttestationJobs.datetime = usrCulPrefenece[0].TimeFormat;
            }
        }
        protected void RadAttestationJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = RadAttestationJobs.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("AttestationProcessId");
        }

        protected void RadAttestationJobs_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            //BindAttestationGrid(0, 10, (OccurenceSelectedValue >= 0 ? (Occurence)OccurenceSelectedValue : Occurence.All).ToString());
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("filter", (OccurenceSelectedValue >= 0 ? (Occurence)OccurenceSelectedValue : Occurence.All).ToString());

            list = DomainApiClient<Dto.Presentation.PresentationJobListAttestation>.GetByAction<List<Dto.Presentation.PresentationJobListAttestation>>(DomainApiAction.Attestation.Presentation.PresentationJobListAttestation.GetAttestationJobs, parameter);
            this.RadAttestationJobs.DataSource = list;
            this.RadAttestationJobs.VirtualItemCount = list.Count;
        }

        #endregion

        #region Delegate
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Attestation Job - Delete");
                Delete();
            }
            else if (e.ActionType == ACLAction.RunNow)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Attestation Job - Runnow");
                ScheduleForRunNow();
            //    var list = new List<ServiceParameter>
            //{
            //    new ServiceParameter{ Name = "attestationProcessId", Value = this.ContextObjectId.Value.ToString() }
            //};
            //    var result = DomainServiceClient.GetByActionAsync<object>("AttestationService/Attestation", null, list);

            }

            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                RadAttestationJobs.ExportSettings.FileName = "AttestationJob";
                RadAttestationJobs.Export(RadAttestationJobs, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                RadAttestationJobs.Export<List<Dto.Presentation.PresentationJobListAttestation>>(list, e.ActionType, "AttestationJob");
            }

        }
        #endregion

        #region Gridview Events

        protected void RadAttestationJobs_Init(object sender, EventArgs e)
        {
            GridFilterMenu menu = RadAttestationJobs.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == Convert.ToString(Operand.NoFilter) || menu.Items[i].Text == Convert.ToString(Operand.Contains) || menu.Items[i].Text == Convert.ToString(Operand.EqualTo) ||
                    menu.Items[i].Text == Convert.ToString(Operand.GreaterThan) || menu.Items[i].Text == Convert.ToString(Operand.LessThan) || menu.Items[i].Text == Convert.ToString(Operand.StartsWith) || menu.Items[i].Text == Convert.ToString(Operand.EndsWith))
                {
                    i++;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        private enum Operand
        {
            [Description("NoFilter")]
            NoFilter,
            [Description("Contains")]
            Contains,
            [Description("EqualTo")]
            EqualTo,
            [Description("GreaterThan")]
            GreaterThan,
            [Description("LessThan")]
            LessThan,
            [Description("StartsWith")]
            StartsWith,
            [Description("EndsWith")]
            EndsWith,
        }
        protected void Occurence_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            //RadComboBox combo = sender as RadComboBox;
            //ViewState[combo.ID] = combo.SelectedValue;
            //if (combo.SelectedValue.ToString() != "All")
            //{
            //    string ColumnName = combo.ID;
            //    string filterExpression = "([" + ColumnName + "] = '" + e.Text + "')";
            //    RadAttestationJobs.MasterTableView.FilterExpression = filterExpression;
            //}
            //else
            //{
            //    RadAttestationJobs.MasterTableView.FilterExpression = string.Empty;
            //}
            //BindGrid(0, 10, "");
            //RadAttestationJobs.DataBind();
            RadComboBox combo = sender as RadComboBox;
            OccurenceSelectedValue = Convert.ToInt16(combo.SelectedIndex);

            string selectedlevel = e.Value;
            string ColumnName = combo.ID;
            string Operand = "Custom";
            ViewState[combo.ID] = combo.SelectedValue;
            if (combo.SelectedValue.ToString() != "All")
            {
                string filterExpression = String.Format(@"(iif({0} == null, """", {1}).ToString() = (""{2}""))", ColumnName, ColumnName, combo.SelectedValue);
                RadAttestationJobs.MasterTableView.FilterExpression = filterExpression;
                if (!Operand.Equals("NoFilter"))
                {
                    RadAttestationJobs.SaveFilterChanges(RadAttestationJobs.ID, session.User, Operand, ColumnName, combo.SelectedValue);
                }
                else
                {
                    RadAttestationJobs.DeleteGridFilterInformation(RadAttestationJobs.ID, session.User.Id, ColumnName);
                }
            }
            else
            {
                RadAttestationJobs.MasterTableView.FilterExpression = string.Empty;
                RadAttestationJobs.DeleteGridFilterInformation(RadAttestationJobs.ID, session.User.Id, ColumnName);
            }
            //BindAttestationGrid(0, 10, (OccurenceSelectedValue >= 0 ? (Occurence)OccurenceSelectedValue : Occurence.All).ToString());
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("filter", (OccurenceSelectedValue >= 0 ? (Occurence)OccurenceSelectedValue : Occurence.All).ToString());

            list = DomainApiClient<Dto.Presentation.PresentationJobListAttestation>.GetByAction<List<Dto.Presentation.PresentationJobListAttestation>>(DomainApiAction.Attestation.Presentation.PresentationJobListAttestation.GetAttestationJobs, parameter);
            this.RadAttestationJobs.DataSource = list;
            this.RadAttestationJobs.VirtualItemCount = list.Count;
            RadAttestationJobs.DataBind();
        }
        protected void Occurence_PreRender(object sender, EventArgs e)
        {
            RadComboBox rcbFilter = sender as RadComboBox;
            if (!Page.IsPostBack)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", session.User.Id.ToString());
                parameter.Add("gridId", RadAttestationJobs.ID);
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
        protected void RadAttestationJobs_ItemDataBound(object sender, GridItemEventArgs e)
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

        #endregion

       
    }
}