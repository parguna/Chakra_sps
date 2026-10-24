using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Report.MySchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DI.Chakra.Web.Report
{
    public partial class ReconcilationReport : UIBasePage
    {
        private static List<Dto.Presentation.PresentationAccountReconcilationReport> ReconList;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                BindStatusType();
            }
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                ReconcilationReportGrid.ExportSettings.FileName = "Reconcilation";
                ReconcilationReportGrid.Export(ReconcilationReportGrid, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                ReconcilationReportGrid.Export<List<Dto.Presentation.PresentationAccountReconcilationReport>>(ReconList, e.ActionType, "Reconcilation");
            }

        }
        protected void search_Click(object sender, EventArgs e)
        {
            //if (DatePickerFromDate.SelectedDate == null && DatePickerToDate.SelectedDate == null)
            //{
            //    MasterPage.ShowAlert(AlertType.Warning, "Valid Date must be required");
            //    return;
            //}
            ReconcilationReportGrid.Rebind();
        }

        protected void btncancel_Click(object sender, EventArgs e)
        {

        }

        protected void ReconcilationReportGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (DatePickerFromDate.SelectedDate != null)
                parameters.Add("FromDate", DatePickerFromDate.SelectedDate.HasValue ? DatePickerFromDate.SelectedDate.Value.ToString("dd-MM-yyyy 00:00:00.001") : string.Empty);
            else
                parameters.Add("FromDate", string.Empty);

            if (DatePickerToDate.SelectedDate != null)
                parameters.Add("ToDate", DatePickerToDate.SelectedDate.HasValue ? DatePickerToDate.SelectedDate.Value.ToString("dd-MM-yyyy 23:59:59.999") : string.Empty);
            else
                parameters.Add("ToDate", string.Empty);

            //if (rbManual.Checked)
            //    parameters.Add("status", ResolvedStatus.Manual.ToString());
            //if (rbAuto.Checked)
            //    parameters.Add("status", ResolvedStatus.Automatic.ToString());
            var statusType = (ResolvedStatus)(Convert.ToInt32(TypeOfStatus.SelectedValue));

            parameters.Add("status", statusType.ToString());
            parameters.Add("IsResolved", Resolved.Checked.ToString());
            ReconList = DomainApiClient<Dto.Presentation.PresentationAccountReconcilationReport>
              .GetByAction<List<Dto.Presentation.PresentationAccountReconcilationReport>>(DomainApiAction.Endpoint.Presentation.PresentationUser.GetResolvedReconcilations, parameters).Take(50000).ToList();
            ReconcilationReportGrid.DataSource = ReconList;
        }

        private void BindStatusType()
        {
            TypeOfStatus.Items.Clear();
            foreach (ResolvedStatus item in Enum.GetValues(typeof(ResolvedStatus)))
            {
                var description = UtilityMethods.GetEnumAttribute<Description>(item);
                ListItem listItem = new ListItem(description, ((byte)item).ToString());
                TypeOfStatus.Items.Add(listItem);
            }
        }

        protected void Resolved_CheckedChanged(object sender, EventArgs e)
        {
            if(Resolved.Checked)
            {
                TypeOfStatus.Enabled = true;
            }
            else
            {
                TypeOfStatus.SelectedIndex = 1;
                TypeOfStatus.SelectedValue = "0";
                TypeOfStatus.Enabled = false;
            }
        }
    }
}