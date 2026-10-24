using Application;
using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;


namespace DI.Chakra.Web.Job.CustomReport.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        private static List<Dto.CustomReport> list = new List<Dto.CustomReport>();

        private bool IsMenuLinked
        {
            get
            {
                return !string.IsNullOrEmpty(Convert.ToString(ViewState["IsMenuLinked"])) ? Convert.ToBoolean(ViewState["IsMenuLinked"]) : false;
            }
            set
            {
                ViewState["IsMenuLinked"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            RadReportJobs.EnableLinqExpressions = false;
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                BindReportJobs();
            }
            Datetime();
        }
        #region Actionclick
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                delete();
            }
            else if (e.ActionType == ACLAction.RunNow)
            {
                if (IsMenuLinked)
                {
                    
                    MasterPage.ShowAlert(AlertType.Warning, "Menu Linked Reports cannot be run as RUNNOW. Please select other Non-Menu Linked Job.");
                    return;
                }
               // DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, string.Format("Custom Report - Runnow for report Id {0} Name {1}", this.ContextObjectId.Value, ContextObjectName), "Report");
                //ScheduleForRunNow();
                var list = new List<ServiceParameter>
                {
                    new ServiceParameter{
                        Name = "customReportId",
                        Value = this.ContextObjectId.Value.ToString()
                                 }
                };

                list.Add(new ServiceParameter
                {
                    Name = "RequestedBy",
                    Value = new SessionStatusImpl().User.DisplayName
                }
                );
                var result = DomainServiceClient.GetByActionAsync<object>("ReportService/CustomReport", null, list);
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                RadReportJobs.ExportSettings.FileName = "CustomReports";
                RadReportJobs.Export(RadReportJobs, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                RadReportJobs.Export<List<Dto.CustomReport>>(list, e.ActionType, "CustomReports");
            }

        }
        #endregion

        private void Datetime()
        {
            SessionStatusImpl session = new SessionStatusImpl();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", session.UserId.ToString());

            List<DI.Chakra.Dto.UserCulturePreference> usrCulPrefenece = DomainApiClient<Dto.UserCulturePreference>.GetByAction<List<Dto.UserCulturePreference>>(DomainApiAction.Preference.UserCulturePreference.GetByUserId, parameter);
            if (usrCulPrefenece != null && usrCulPrefenece.Count > 0)
            {
                RadReportJobs.Culture.DateTimeFormat.ShortDatePattern = usrCulPrefenece[0].DateFormat;
                RadReportJobs.datetime = usrCulPrefenece[0].TimeFormat;
            }
            else
            {
                RadReportJobs.Culture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            }
        }
        private void BindReportJobs()
        {
            list = DomainApiClient<Dto.CustomReport>.GetAll().ToList();
            this.RadReportJobs.DataSource = list;
        }

        protected void RadReportJobs_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindReportJobs();
        }

        private void delete()
        {
            if (!ContextObjectId.HasValue)
                return;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("reportId", ContextObjectId.ToString());
            DomainApiClient<Dto.CustomReport>.GetByAction<int>("DeleteCustomReportByRuleId", parameters);
          //  DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, string.Format("Custom Report - Delete for {0} Name {1}", ContextObjectId.Value, ContextObjectName), "Report");

            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "Custom Report Deleted Successfully");
            RadReportJobs.Rebind();
        }
        protected void schd_ItemDataBound(object sender, GridItemEventArgs e)
        {
            try
            {
                if (e.Item is GridFilteringItem)
                {
                    GridFilteringItem Item = (GridFilteringItem)e.Item;
                    RadComboBox combo = (RadComboBox)Item["Occurence"].FindControl("Occurence");
                    combo.DataSource = Enum.GetValues(typeof(Occurence));
                    combo.DataBind();
                }

                if (e.Item is GridDataItem)
                {
                    GridDataItem item = e.Item as GridDataItem;
                    TableCell cell = item["AdhocProcessName"];
                    cell.Style.Add("word-break", "break-word");
                    cell = item["AdhocDescription"];
                    cell.Style.Add("word-break", "break-word");
                }
            }
            catch(Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
            }
        }
        protected void RadReportJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = RadReportJobs.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
           // this.ContextObjectName = Convert.ToString(g["Name"].Text);
            IsMenuLinked = Convert.ToBoolean(g["IsLinkedToMenu"].Text);

        }
        protected void Status_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBox combo = sender as RadComboBox;
            ViewState[combo.ID] = combo.SelectedValue;
            if (Convert.ToInt64(combo.SelectedValue.ToString()) > 0)
            {
                string ColumnName = combo.ID;
                string filterExpression = "([" + ColumnName + "] = '" + e.Text + "')";
                RadReportJobs.MasterTableView.FilterExpression = filterExpression;
            }
            else
            {
                RadReportJobs.MasterTableView.FilterExpression = string.Empty;
            }
            BindReportJobs();
            RadReportJobs.DataBind();

        }
        protected void Status_PreRender(object sender, EventArgs e)
        {
            RadComboBox rcbFilter = sender as RadComboBox;
            if (ViewState[rcbFilter.ID] != null)
            {
                rcbFilter.SelectedValue = ViewState[rcbFilter.ID].ToString();
            }
        }


        protected void RadReportJobs_Init(object sender, EventArgs e)
        {
            GridFilterMenu menu = RadReportJobs.FilterMenu;
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

        public void Refresh()
        {
            RadReportJobs.Rebind();
            Datetime();
        }
    }
}