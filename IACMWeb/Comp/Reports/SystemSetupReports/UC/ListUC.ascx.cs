using System;
using System.Collections.Generic;
using DI.Chakra.Dto;
using System.Web.UI;
using Telerik.Web.UI;
using Newtonsoft.Json;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Web.Comp.Reports.SystemSetupReports.UC
{
    public partial class ListUC : UIBaseUC
    {
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

                Export(ComplianceReportGrid, e.ActionType);
            }
        }

        private void BindReportType()
        {
            var _comp = DomainApiClient<ComplianceReportModules>.GetAll();
            ReportId.DataTextField = "ReportName";
            ReportId.DataValueField = "Id";
            ReportId.DataSource = _comp;
            ReportId.DataBind();
            ReportId.Items.Insert(0, "--Select--");
        } 

        protected void ReportId_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComplianceReportGrid.Rebind();
        }

        protected void ComplianceReportGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindGrid();
        }

        protected void ComplianceReportGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            //BindGrid();
            //ComplianceReportGrid.Rebind();
        }

        protected void BindGrid()
        {
            if (ReportId.SelectedValue != "--Select--")
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("ReportId", ReportId.SelectedItem.Text);
                IList<object> objList = DomainApiClient<Dto.SystemSetupReports>.GetByAction<List<object>>
                                        (DomainApiAction.SystemSetupReports.GetComplianceReportResult, parameter);
                switch (ReportId.SelectedItem.Text)
                {
                    case "Functions":
                        List<PresentationSODFunction> functions = new List<PresentationSODFunction>();
                        foreach (Newtonsoft.Json.Linq.JObject obj in objList)
                        {
                            PresentationSODFunction function = JsonConvert.DeserializeObject<PresentationSODFunction>(obj.ToString());
                            functions.Add(function);
                        }
                        ComplianceReportGrid.DataSource = functions;
                        ComplianceReportGrid.ExportSettings.FileName = "SystemSetupReport_Functions";
                        break;

                    case "RiskCode":
                        List<PresentationSODRiskCode> riskCodes = new List<PresentationSODRiskCode>();
                        foreach (Newtonsoft.Json.Linq.JObject obj in objList)
                        {
                            PresentationSODRiskCode riskCode = JsonConvert.DeserializeObject<PresentationSODRiskCode>(obj.ToString());
                            riskCodes.Add(riskCode);
                        }
                        ComplianceReportGrid.DataSource = riskCodes;
                        ComplianceReportGrid.ExportSettings.FileName = "SystemSetupReport_RiskCodes";
                        break;

                    case "PolicyExecution":
                        List<PresentationSODPolicyExecutionDefinition> policyExecutions = new List<PresentationSODPolicyExecutionDefinition>();
                        foreach (Newtonsoft.Json.Linq.JObject obj in objList)
                        {
                            PresentationSODPolicyExecutionDefinition policyExecution = JsonConvert.DeserializeObject<PresentationSODPolicyExecutionDefinition>(obj.ToString());
                            policyExecutions.Add(policyExecution);
                        }
                        ComplianceReportGrid.DataSource = policyExecutions;
                        ComplianceReportGrid.ExportSettings.FileName = "SystemSetupReport_PolicyExecution";
                        break;

                    case "Conflicts":
                        List<PresentationSODConflicts> conflicts = new List<PresentationSODConflicts>();
                        foreach (Newtonsoft.Json.Linq.JObject obj in objList)
                        {
                            PresentationSODConflicts conflict = JsonConvert.DeserializeObject<PresentationSODConflicts>(obj.ToString());
                            conflicts.Add(conflict);
                        }
                        ComplianceReportGrid.DataSource = conflicts;
                        ComplianceReportGrid.ExportSettings.FileName = "SystemSetupReport_Conflicts";
                        break;

                    case "Exceptions":
                        List<PresentationSODException> exceptions = new List<PresentationSODException>();
                        foreach (Newtonsoft.Json.Linq.JObject obj in objList)
                        {
                            PresentationSODException exception = JsonConvert.DeserializeObject<PresentationSODException>(obj.ToString());
                            exceptions.Add(exception);
                        }
                        ComplianceReportGrid.DataSource = exceptions;
                        ComplianceReportGrid.ExportSettings.FileName = "SystemSetupReport_Exceptions";
                        break;

                    case "Mitigations":
                        List<PresentationSODMitigation> mitigations = new List<PresentationSODMitigation>();
                        foreach (Newtonsoft.Json.Linq.JObject obj in objList)
                        {
                            PresentationSODMitigation mitigation = JsonConvert.DeserializeObject<PresentationSODMitigation>(obj.ToString());
                            mitigations.Add(mitigation);
                        }
                        ComplianceReportGrid.DataSource = mitigations;
                        ComplianceReportGrid.ExportSettings.FileName = "SystemSetupReport_Mitigations";
                        break;

                    case "Policy":
                        List<PresentationCompliancePolicy> policies = new List<PresentationCompliancePolicy>();
                        foreach (Newtonsoft.Json.Linq.JObject obj in objList)
                        {
                            PresentationCompliancePolicy policy = JsonConvert.DeserializeObject<PresentationCompliancePolicy>(obj.ToString());
                            policies.Add(policy);
                        }
                        ComplianceReportGrid.DataSource = policies;
                        ComplianceReportGrid.ExportSettings.FileName = "SystemSetupReport_Policy";
                        break;
                }                    
            }
        }
    }
}
