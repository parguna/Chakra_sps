using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Web.Dashboard.Attestation
{
    public partial class AttestationOverallStatus : DI.Chakra.Web.UIBaseUC
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DashBoardHeaderUC.HeaderImageUrl = "../Images/Attestation/task-completeness.jpg";

            AttestationTypeUC.SelectedIndexChanged += new EventHandler(ddlAttestation_SelectedIndexChanged);
            //PopulateAccessRequestSourceChart();
        }

        protected void ddlAttestation_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlAttestation = (DropDownList)AttestationTypeUC.FindControl("ddlAttestation");
            PopulateAccessRequestSourceChart(ddlAttestation.SelectedValue.Trim());
        }

        private void PopulateAccessRequestSourceChart(string campaignName)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("campaignName", campaignName);

            IList<Dto.AttestationcompletedSourcePieSeries> chartData = DomainApiClient<Dto.AttestationcompletedSourcePieSeries>.GetByAction<IList<Dto.AttestationcompletedSourcePieSeries>>("GetPieChartData", parameter);
            PieSeries resourceTypes = AttestationcompletedChart.PlotArea.Series[0] as PieSeries;
            resourceTypes.Items.Clear();
            foreach (Dto.AttestationcompletedSourcePieSeries model in chartData)
            {
                System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml("#AEB536");
                if (model.Name.ToString() == "Pending")
                {
                    color = Color.LightGray;
                }
                else if (model.Name.ToString() == "Approved")
                {
                    color = System.Drawing.Color.FromArgb(141, 203, 42);
                }
                else if (model.Name.ToString() == "Rejected")
                {
                    color = System.Drawing.ColorTranslator.FromHtml("#FE8442");
                }

                resourceTypes.Items.Add(new SeriesItem() { Name = model.Name.ToString(), YValue = model.Y, BackgroundColor = color, });

            }
        }
    }
}