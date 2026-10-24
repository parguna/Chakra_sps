using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
namespace DI.Chakra.Web.Dashboard.Attestation
{
    public partial class AttestationcompletedTaskUC : DI.Chakra.Web.UIBaseUC
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

        public StatusType RequestSource
        {

            get
            {
                if (ViewState["AttestationcompletedChart"] == null)
                {
                    return StatusType.Approved;
                }
                return (StatusType)ViewState["AttestationcompletedChart"];
            }

            set
            {

                ViewState["AttestationcompletedChart"] = value;

            }

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                Default();

            //if (null != Session["AtnDashboardDefault"])
            //{
            //    GenerateSpeedoMeter(Convert.ToString(Session["AtnDashboardDefault"]));
            //}
            //else
            //{
            //    AttestationTypeUC.LoadAttestationType();
            //    if (null != Session["AtnDashboardDefault"])
            //    {
            //        GenerateSpeedoMeter(Convert.ToString(Session["AtnDashboardDefault"]));
            //    }
            //}

            DashBoardHeaderUC.HeaderImageUrl = "../Images/Attestation/task-completeness.jpg";

            AttestationTypeUC.SelectedIndexChanged += new EventHandler(ddlAttestation_SelectedIndexChanged);
           


        }
        protected void ddlAttestation_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlAttestation = (DropDownList)AttestationTypeUC.FindControl("ddlAttestation");
            GenerateSpeedoMeter(ddlAttestation.SelectedValue.Trim());

        }

        #region Methods
        private void GenerateSpeedoMeter(string attestationName)
        {
            try
            {     
                           
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("campaignName", attestationName);
                int totalTask = 0;//out parameter
                                  //parameter.Add("totaltask", totalTask.ToString());
                int completedTask = 0;//out perameter
                                      //parameter.Add("totaltask", completedTask.ToString());

                Infrastructure.CampaignTaskDetail taskCompleteness = DomainApiClient<Dto.Task>.GetByAction<Infrastructure.CampaignTaskDetail>(DomainApiAction.Tasks.Task.GetCampaignTaskDetail, parameter);

                totalTask = Convert.ToInt16(taskCompleteness.TotalTask);
                completedTask = Convert.ToInt16(taskCompleteness.CompletedTask);

                if (totalTask == 0 && completedTask == 0)
                {
                    Default();
                    return;
                }
                
                maxValue = totalTask;
                currentPointerValue = completedTask;
                rangeValue = maxValue / splitRatio;

                decimal tmpMinValue = minValue;

                var range1 = new GaugeRange();
                range1.From = 0;
                range1.To = completedTask;
                range1.Color = System.Drawing.Color.FromArgb(141, 203, 42);

                var range2 = new GaugeRange();
                range2.From = completedTask;
                range2.To = totalTask;
                range2.Color = Color.LightGray;

                ragTaskCompleteness.Scale.Ranges.Add(range1);
                ragTaskCompleteness.Scale.Ranges.Add(range2);               

                // Set Gauge control properties
                ragTaskCompleteness.Width = width;
                ragTaskCompleteness.Height = height;
                ragTaskCompleteness.Pointer.Value = currentPointerValue;
                ragTaskCompleteness.Pointer.Cap.Color = Color.FromArgb(254, 132, 66);
                ragTaskCompleteness.Scale.Min = minValue;
                ragTaskCompleteness.Scale.Max = maxValue;
                ragTaskCompleteness.Scale.MajorUnit = majorUnit;
                ragTaskCompleteness.Scale.Labels.Visible = true;
                ragTaskCompleteness.Scale.Labels.Format = labelFormat;
                ragTaskCompleteness.Scale.Labels.Font = "20px Helvetica,sans-serif,Calibri,Bold";                

                //date range control values.
                lblStartDate.Text = string.Format("Start Date : {0}", taskCompleteness.StartDate);
                lblEndDate.Text = string.Format("End Date : {0}", taskCompleteness.EndDate);

                RadLinearGauge1.Pointer.Shape = Telerik.Web.UI.Gauge.PointerShape.BarIndicator;
                RadLinearGauge1.Pointer.Color = System.Drawing.Color.FromArgb(42, 148, 203);
                RadLinearGauge1.Pointer.Size = 10.0f;                
                RadLinearGauge1.Pointer.Track.Opacity = 0.2f;
                RadLinearGauge1.Pointer.Value = (decimal)(DateTime.Now - taskCompleteness.StartDate).TotalDays;
                RadLinearGauge1.Scale.Min = 0;
                var maxLevel = (decimal)(taskCompleteness.EndDate - taskCompleteness.StartDate).TotalDays;
                RadLinearGauge1.Scale.Max = maxLevel;                
                RadLinearGauge1.Scale.MajorUnit = maxLevel / 10;
                RadLinearGauge1.Scale.Labels.Format = "{0} days";

                lblLevel.Text = string.Format("Level {0}/{1}", taskCompleteness.CurrentLevel, taskCompleteness.TotalLevel);

                //level
                var lvl1 = 0.4m;
                var lvl2 = 0.3m;
                var lvl3 = 0.2m;
                                
                //GaugeRange
                //Create new GaugeRange object
                GaugeRange gr1 = new GaugeRange();

                //Set the properties of the new object
                gr1.From = 0;
                gr1.To = (maxLevel*lvl1);
                gr1.Color = System.Drawing.Color.FromArgb(141, 203, 42);
                
                GaugeRange gr2 = new GaugeRange();
                gr2.From = gr1.To;
                gr2.To = gr2.From + (maxLevel * lvl2);
                gr2.Color = System.Drawing.Color.FromArgb(255, 199, 0);

                GaugeRange gr3 = new GaugeRange();
                gr3.From = gr2.To;
                gr3.To = gr3.From + (maxLevel * lvl3);
                gr3.Color = System.Drawing.Color.FromArgb(255, 122, 0);

                GaugeRange gr4 = new GaugeRange();
                gr4.From = gr3.To;
                gr4.To = maxLevel;
                gr4.Color = System.Drawing.Color.FromArgb(194, 0, 0);

                RadLinearGauge1.Scale.Ranges.Clear();
                RadLinearGauge1.Scale.Ranges.Add(gr1);
                RadLinearGauge1.Scale.Ranges.Add(gr2);
                RadLinearGauge1.Scale.Ranges.Add(gr3);
                RadLinearGauge1.Scale.Ranges.Add(gr4);                
            }
            catch (Exception ex)
            {
                
            }
        }


        private void Default()
        {
            try
            {                
                int totalTask = 0;//out parameter
                                  //parameter.Add("totaltask", totalTask.ToString());
                int completedTask = 0;//out perameter
                                      //parameter.Add("totaltask", completedTask.ToString());
              

                maxValue = totalTask;
                currentPointerValue = completedTask;
                rangeValue = maxValue / splitRatio;

                decimal tmpMinValue = minValue;

                var range1 = new GaugeRange();
                range1.From = 0;
                range1.To = completedTask;
                range1.Color = System.Drawing.Color.FromArgb(141, 203, 42);

                var range2 = new GaugeRange();
                range2.From = completedTask;
                range2.To = totalTask;
                range2.Color = Color.LightGray;
                
                //ragTaskCompleteness.Scale.Ranges.Add(range1);
                ragTaskCompleteness.Scale.Ranges.Add(range2);

                // Set Gauge control properties
                ragTaskCompleteness.Width = width;
                ragTaskCompleteness.Height = height;
                ragTaskCompleteness.Pointer.Value = currentPointerValue;
                
                ragTaskCompleteness.Scale.Labels.Visible = false;
                ragTaskCompleteness.Scale.Labels.Format = labelFormat;
                ragTaskCompleteness.Scale.Labels.Font = "20px Helvetica,sans-serif,Calibri,Bold";

                ragTaskCompleteness.Scale.Labels.Visible = false;
                ragTaskCompleteness.Pointer.Cap.Color = Color.FromArgb(254, 132, 66);

                //date range control values.
                lblStartDate.Text = string.Format("Start Date : {0}", "N.A.");
                lblEndDate.Text = string.Format("End Date : {0}", "N.A.");

                RadLinearGauge1.Pointer.Shape = Telerik.Web.UI.Gauge.PointerShape.BarIndicator;
                RadLinearGauge1.Pointer.Color = System.Drawing.Color.FromArgb(42, 148, 203);
                RadLinearGauge1.Pointer.Size = 10.0f;
                RadLinearGauge1.Pointer.Track.Opacity = 0.2f;
                RadLinearGauge1.Pointer.Value = 0;
                RadLinearGauge1.Scale.Min = 0;
                var maxLevel = 0;
                RadLinearGauge1.Scale.Max = maxLevel;
                RadLinearGauge1.Scale.MajorUnit = maxLevel / 10;
                RadLinearGauge1.Scale.Labels.Format = "{0} days";

                lblLevel.Text = string.Format("Level {0}/{1}", 0,0);

                //level
                var lvl1 = 0.4m;
                var lvl2 = 0.3m;
                var lvl3 = 0.2m;

                //GaugeRange
                //Create new GaugeRange object
                GaugeRange gr1 = new GaugeRange();

                //Set the properties of the new object
                gr1.From = 0;
                gr1.To = (maxLevel * lvl1);
                gr1.Color = System.Drawing.Color.FromArgb(141, 203, 42);

                GaugeRange gr2 = new GaugeRange();
                gr2.From = gr1.To;
                gr2.To = gr2.From + (maxLevel * lvl2);
                gr2.Color = System.Drawing.Color.FromArgb(255, 199, 0);

                GaugeRange gr3 = new GaugeRange();
                gr3.From = gr2.To;
                gr3.To = gr3.From + (maxLevel * lvl3);
                gr3.Color = System.Drawing.Color.FromArgb(255, 122, 0);

                GaugeRange gr4 = new GaugeRange();
                gr4.From = gr3.To;
                gr4.To = maxLevel;
                gr4.Color = System.Drawing.Color.FromArgb(194, 0, 0);

                RadLinearGauge1.Scale.Ranges.Clear();
                RadLinearGauge1.Scale.Ranges.Add(gr1);
                RadLinearGauge1.Scale.Ranges.Add(gr2);
                RadLinearGauge1.Scale.Ranges.Add(gr3);
                RadLinearGauge1.Scale.Ranges.Add(gr4);
            }
            catch (Exception ex)
            {

            }
        }



        #endregion
    }
}