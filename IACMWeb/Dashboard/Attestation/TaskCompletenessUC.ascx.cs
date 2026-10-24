using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Dashboard.Attestation
{
    public partial class TaskCompletenessUC : DI.Chakra.Web.UIBaseUC
    {
        #region Variable declaration
        private decimal minValue = 0;
        private decimal maxValue = 10;
        private decimal majorUnit = 1;
        private decimal currentPointerValue = 0;
        private int splitRatio = 4;
        private decimal rangeValue = 0;
        private string labelFormat = "";
        private Unit width = 350;
        private Unit height = 350;
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            ragTaskCompleteness.Visible = true;
            if (null != Session["AtnDashboardDefault"])
            {
                GenerateSpeedoMeter(Convert.ToString(Session["AtnDashboardDefault"]));                
            }
            else
            {
                AttestationTypeUC.LoadAttestationType();
                if (null != Session["AtnDashboardDefault"])
                {
                    GenerateSpeedoMeter(Convert.ToString(Session["AtnDashboardDefault"]));                    
                }
            }

            DashBoardHeaderUC.HeaderImageUrl = "../Images/Attestation/task-completeness.jpg";

            AttestationTypeUC.SelectedIndexChanged += new EventHandler(ddlAttestation_SelectedIndexChanged);
        }
        protected void ddlAttestation_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlAttestation = (DropDownList)AttestationTypeUC.FindControl("ddlAttestation");
            GenerateSpeedoMeter(ddlAttestation.SelectedItem.Text.Trim());

        }
        #endregion

        #region Methods
        private void GenerateSpeedoMeter(string attestationName)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("attestationName", attestationName);
            int totalTask = 0;//out parameter
            //parameter.Add("totaltask", totalTask.ToString());
            int completedTask = 0;//out perameter
                                  //parameter.Add("totaltask", completedTask.ToString());

            Infrastructure.TaskCompleteness taskCompleteness = DomainApiClient<Dto.Task>.GetByAction<Infrastructure.TaskCompleteness>(DomainApiAction.Tasks.Task.GetTaskCompleteness, parameter);

            totalTask = Convert.ToInt16(taskCompleteness.TotalTask);
            completedTask = Convert.ToInt16(taskCompleteness.CompletedTask);

            maxValue = totalTask;
            currentPointerValue = completedTask;
            rangeValue = maxValue / splitRatio;

            decimal tmpMinValue = minValue;

            var range = new GaugeRange();
            range.From = 0;
            range.To = totalTask;
            range.Color = Color.LightGray;
            ragTaskCompleteness.Scale.Ranges.Add(range);

            // Set Gauge control properties
            ragTaskCompleteness.Width = width;
            ragTaskCompleteness.Height = height;
            ragTaskCompleteness.Pointer.Value = currentPointerValue;
            ragTaskCompleteness.Scale.Min = minValue;
            ragTaskCompleteness.Scale.Max = maxValue;
            ragTaskCompleteness.Scale.MajorUnit = majorUnit;
            ragTaskCompleteness.Scale.Labels.Visible = true;
            ragTaskCompleteness.Scale.Labels.Format = labelFormat;
            ragTaskCompleteness.Scale.Labels.Font = "20px Calibri,Helvetica,sans-serif,Bold";
        }
        #endregion

    }
}