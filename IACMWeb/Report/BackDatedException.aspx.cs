using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Report
{
    public partial class BackDatedException : UIBasePage
    {
        private static List<Dto.User> userList;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }
        void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdBackDatedException.ExportSettings.FileName = "Backdated Exceptions";
                grdBackDatedException.Export(grdBackDatedException, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdBackDatedException.Export<List<Dto.User>>(userList, e.ActionType, "Backdated Exceptions");
            }
        }
        protected void grdBackDatedException_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {           
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("onboardBackdatedWindowperiod", "7");// MasterConfigHelper.OnboardBackDatedWindowPeriodDays.ToString());
            parameters.Add("fromDate", DatePickerFromDate.SelectedDate.HasValue ? DatePickerFromDate.SelectedDate.Value.ToString("dd-MM-yyyy 00:00:00.001") : string.Empty);
            parameters.Add("toDate", DatePickerToDate.SelectedDate.HasValue ? DatePickerToDate.SelectedDate.Value.ToString("dd-MM-yyyy 23:59:59.999") : string.Empty);
            userList = DomainApiClient<Dto.User>.GetByAction<List<Dto.User>>("GetBackkDatedExceptions", parameters);
            grdBackDatedException.DataSource = userList;
        }

        protected void search_Click(object sender, EventArgs e)
        {
            grdBackDatedException.Rebind();
        }
    }
}