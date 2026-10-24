using System;
using System.Collections.Generic;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;

namespace DI.Chakra.Web.Dashboard.Attestation
{
    public partial class AttestationRemediateStatusUC : DI.Chakra.Web.UIBaseUC
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
            TaskSummaryPie.DataSource = GetRevokeStatus(out rowCount, attestationType);
            TaskSummaryPie.DataBind();
            DashBoardHeaderUC.DashBoardRowCount = rowCount;

        }

        private IList<PresentationAttestationStatusSummary> GetRevokeStatus(out string rowCount, string attestationType)
        {
            rowCount = "";
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endPointTypeName" , attestationType.ToString());
            var listTaskSummary = DomainApiClient<PresentationAttestationStatusSummary>.GetByAction<IList<PresentationAttestationStatusSummary>>(DomainApiAction.Attestation.Presentation.PresentationAttestationStatusSummary.GetSummaryByEndPointTypeName,parameter);
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
            
        }

        protected void ddlAttestationType_SelectedIndexChanged(object sender, EventArgs e)
        {

            PopulatePieChart(ddlAttestationType.SelectedItem.Text.ToLower());
        }

        private void LoadAttestationType()
        {
            var attestationType = DomainApiClient<AttestationEndpoint>.GetByAction<List<string>>(DomainApiAction.Attestation.AttestationEndpoint.GetAttestationEndpoints,null);
            TaskSummaryPie.Visible = true;
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
