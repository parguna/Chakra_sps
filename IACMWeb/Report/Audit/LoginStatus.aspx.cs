using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Report.MySchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Report.Audit
{
    public partial class LoginStatus : ReportBasePage
    {
        private static IList<ServiceLog> ADUserList = new List<ServiceLog>();
        private const string V_login = "loginlog";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        public string TransferType
        {
            get
            {
                if (ViewState[V_login] == null)
                    return "loginlog";
                return (string)ViewState[V_login];
            }
            set
            {
                ViewState[V_login] = value;
            }
        }

        protected void EmployeesGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (TransferType.Equals("loginlog"))

                ADUserList = DomainApiClient<Dto.ServiceLog>
                    .GetByAction<List<Dto.ServiceLog>>(DomainApiAction.Log.ServiceLog.GetLoginDetail, null);
            EmployeeGrid.DataSource = ADUserList;
        }


        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                EmployeeGrid.ExportSettings.FileName = "Login Status";
                Export(EmployeeGrid, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                EmployeeGrid.Export<IList<ServiceLog>>(ADUserList, e.ActionType, "Login Status");
            }
        }
    }
}