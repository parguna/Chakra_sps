using System;
using System.Collections.Generic;
using DI.Chakra.Dto;
using System.Web.UI;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Web.Comp.Reports.ManagementReports.UC
{
    public partial class ListUC : DI.Chakra.Web.UIBaseUC
    {
        private static List<PresentationReport> reports = new List<PresentationReport>();
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
                ComplianceReportGrid.ExportSettings.FileName = "ComplianceManagementReports";
                ComplianceReportGrid.Export(ComplianceReportGrid, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                ComplianceReportGrid.Export<List<PresentationReport>>(reports, e.ActionType, "ComplianceManagementReports");
            }
        }

        private void BindReportType()
        {
            var _reports = DomainApiClient<SODPolicyReport>.GetByAction<List<string>>(DomainApiAction.SODPolicyReport.GetUniqueRunCodes, null);
            //ReportIdCom.DataTextField = "Value";
            //ReportIdCom.DataValueField = "Key";
            ReportIdCom.DataSource = _reports;
            ReportIdCom.DataBind();
            ReportIdCom.Items.Insert(0, "--Select--");
        }

        protected void ComplianceReportGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindGrid();
        }

        protected void ReportIdCom_SelectedIndexChanged(object sender, EventArgs e)
        {
            //BindGrid();
            ComplianceReportGrid.Rebind();
        }

        protected void BindGrid()
        {
            if (ReportIdCom.SelectedValue != "--Select--")
            {
              //  long ReportId = Convert.ToInt64(ReportIdCom.SelectedValue);
              //  SODReport report = new SODReport();
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("UniqueRunCode", ReportIdCom.SelectedItem.Text);
                List<SODPolicyReport> policyReports= DomainApiClient<SODPolicyReport>.GetByAction<List<SODPolicyReport>>
                    (DomainApiAction.SODPolicyReport.GetPolicyReport,parameter);
                
                foreach (SODPolicyReport report in policyReports)
                {
                    PresentationReport rep = new PresentationReport();
                    rep.Id = report.Id;
                    rep.ConflictCode = report.ConflictCode;
                    rep.UserName = report.UserName;
                    rep.UniqueRunCode = report.PolicyDefinition.UniqueRunCode;
                    rep.RunDate = report.RunDate.ToShortDateString();
                    rep.CompliancePolicyCode = report.PolicyDefinition.CompliancePolicy != null ? report.PolicyDefinition.CompliancePolicy.CompliancePolicyCode : "";
                    rep.SODConflictCode = report.Conflict != null ? report.Conflict.SodConflictCode : "";
                    if (report.Conflict != null && report.Conflict.RiskCodeId > 0)
                    {
                        SODRiskCode code = DomainApiClient<SODRiskCode>.GetById(report.Conflict.RiskCodeId);
                        rep.RiskCode = code != null ? code.RiskCode : "";
                    }
                    rep.MitigationCode = report.MitigationCode;
                    rep.SodFunctionCode1 = report.Function1 != null ? report.Function1.SodFunctionCode : "";
                    rep.SodFunctionCode2 = report.Function2 != null ? report.Function2.SodFunctionCode : "";
                    rep.Role1 = report.Role1;
                    rep.Role2 = report.Role2;
                    rep.ExceptionCode = report.exception != null ? report.exception.ExceptionCode : "";
                    reports.Add(rep);
                }
                ComplianceReportGrid.DataSource = reports;
                //ComplianceReportGrid.DataBind();
            }
        }

    }
    public class PresentationReport
    {
        public long Id { get; set; }
        public string ConflictCode { get; set; }
        public string UserName { get; set; }
        public string UniqueRunCode { get; set; }
        public string RunDate { get; set; }
        public string CompliancePolicyCode { get; set; }
        public string SODConflictCode { get; set; }
        public string RiskCode { get; set; }
        public string MitigationCode { get; set; }

        public string SodFunctionCode1 { get; set; }

        public string Role1 { get; set; }
        public string SodFunctionCode2 { get; set; }
        public string Role2 { get; set; }

        public string ExceptionCode { get; set; }
    }
}