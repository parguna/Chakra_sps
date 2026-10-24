using System;
using System.Collections.Generic;
using DI.Chakra.Dto;
using System.Web.UI;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Web.Comp.Reports.OperationDetailedReports.UC
{
    public partial class ListUC : DI.Chakra.Web.UIBaseUC
    {
        private static List<SODReport> list;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                BindReportType();
            }
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                ComplianceReportGrid.ExportSettings.FileName = "OperationDetailedReports";
                ComplianceReportGrid.Export(ComplianceReportGrid, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                ComplianceReportGrid.Export<List<SODReport>>(list, e.ActionType, "OperationDetailedReports");
            }
        }

        private void BindReportType()
        {
            var _reports = DomainApiClient<SODPolicyReport>.GetAll();
            ReportIdCom.DataTextField = "UniqueRunCode";
            ReportIdCom.DataValueField = "Id";
            ReportIdCom.DataSource = _reports;
            ReportIdCom.DataBind();
            ReportIdCom.Items.Insert(0, "--Select--");
        }

        protected void ComplianceReportGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindGrid();
        }

        protected void ComplianceReportGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            BindGrid();
            ComplianceReportGrid.Rebind();
        }

        protected void BindGrid()
        {
            if (ReportIdCom.SelectedValue != "--Select--")
            {
                long ReportId = Convert.ToInt64(ReportIdCom.SelectedValue);
                SODReport report = new SODReport();
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("Sodrunpolicyid", report.SODRunPolicyId.Value.ToString());
                list = DomainApiClient<SODReport>.GetByAction<List<SODReport>>
                    (DomainApiAction.SODReport.GetBySODRunPolicyIdIncludeSodFunctionItem, parameter);
                ComplianceReportGrid.DataSource = list;
            }
        }
    }
}