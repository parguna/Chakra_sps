using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
//using Telerik.Web.UI;
using Telerik.Web.UI.HtmlChart;
using System.Drawing;
using Telerik.Web.UI.HtmlChart.PlotArea;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Web.Dashboard;
using System.Linq;

namespace DI.Chakra.Web.Dashboard.Attestation
{
    public partial class TaskSummaryUC : DI.Chakra.Web.UIBaseUC
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAttestationType();
            }

        }

        private void PopulatePieChart(string attestationType)
        {
            string rowCount = "";
            TaskSummaryPie.DataSource = GetTaskSummary(out rowCount, attestationType);
            TaskSummaryPie.DataBind();
            DashBoardHeaderUC.DashBoardRowCount = rowCount;

        }

        private IList<PresentationAttestationStatusSummary> GetTaskSummary(out string rowCount, string attestationType)
        {
            rowCount = "";
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endPointTypeName", attestationType);
            var listTaskSummary = DomainApiClient< PresentationAttestationStatusSummary>.GetByAction<IList<PresentationAttestationStatusSummary>>(DomainApiAction.Attestation.Presentation.PresentationAttestationStatusSummary.GetTaskSummary,parameter);
            int tmpRowCount = 0;

            if (listTaskSummary != null && listTaskSummary.Count > 0)
            {
                foreach (PresentationAttestationStatusSummary taskSummary in listTaskSummary)
                {
                    tmpRowCount += taskSummary.StatusCount;
                }
            }
            rowCount = Convert.ToString(tmpRowCount);
            return listTaskSummary;


            //List<AttestationTaskSummary> listTaskSummary = new List<AttestationTaskSummary>();
            //listTaskSummary.Add(new AttestationTaskSummary(10, "Pending", true, "#ffdd00"));
            //listTaskSummary.Add(new AttestationTaskSummary(20, "Completed", false, "#279691"));
            //listTaskSummary.Add(new AttestationTaskSummary(30, "Delegated", false, "#59F0BD"));
        }

        protected void ddlAttestationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulatePieChart(ddlAttestationType.SelectedItem.Text.ToLower());
        }

        private void LoadAttestationType()
        {
            var attestationType = DomainApiClient<Dto.AttestationEndpoint>.GetByAction<List<string>>(DomainApiAction.Attestation.AttestationEndpoint.GetAttestationEndpoints,null);
            if (attestationType != null && attestationType.Count > 0)
            {
                ddlAttestationType.DataSource = attestationType;
                ddlAttestationType.DataBind();
                ddlAttestationType.Items.Insert(0, "Select");
                ddlAttestationType.Items.FindByValue(attestationType[0]).Selected = true;
                PopulatePieChart(attestationType[0]);
                HideNoData();
            }
            else
            {
                ShowNoData();
                //Response.Write("No Data Available !");
            }
        }

        private void ShowNoData()
        {
            divDdlType.Visible = false;
            divChartControl.Visible = false;
            divNoData.Visible = true;
        }
        private void HideNoData()
        {
            divDdlType.Visible = true;
            divChartControl.Visible = true;
            divNoData.Visible = false;
        }

    }
}
