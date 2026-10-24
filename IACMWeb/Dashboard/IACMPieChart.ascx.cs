using Application;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Dashboard
{
    public partial class IACMPieChart : DI.Chakra.Web.UIBaseUC
    {
        #region Class MemberVariable
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
        #endregion
        public AccessRequestSource RequestSource
        {

            get
            {
                if (ViewState["AccessRequestSource"] == null)
                {
                    return AccessRequestSource.None;
                }
                return (AccessRequestSource)ViewState["AccessRequestSource"];
            }

            set
            {

                ViewState["AccessRequestSource"] = value;

            }

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DashBoardHeaderUCUC.IsChartType = true;
                PopulateAccessRequestSourceChart();
            }
        }
        private void PopulateAccessRequestSourceChart()
        {

            IList<Dto.AccessRequestSourcePieSeries> chartData = DomainApiClient<Dto.AccessRequestSourcePieSeries>.GetByAction<IList<Dto.AccessRequestSourcePieSeries>>("GetPieChartData", null);
            PieSeries resourceTypes = AccessRequestSourceChart.PlotArea.Series[0] as PieSeries;
            resourceTypes.Items.Clear();
            foreach (Dto.AccessRequestSourcePieSeries model in chartData)
            {
                System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml("#AEB536");
                if(model.Name.ToString() == "AdhocRequest")
                {
                    color = System.Drawing.ColorTranslator.FromHtml("#26A9E0");
                }
                else if (model.Name.ToString() == "OnBoarding")
                {
                    color = System.Drawing.ColorTranslator.FromHtml("#AEB536");
                }
                else if (model.Name.ToString() == "OffBoarding")
                {
                    color = System.Drawing.ColorTranslator.FromHtml("#FE8442");
                }
                else if (model.Name.ToString() == "OUChange")
                {
                    color = System.Drawing.ColorTranslator.FromHtml("#8BC53F");
                }
                else if (model.Name.ToString() == "ReOrganisation")
                {
                    color = System.Drawing.ColorTranslator.FromHtml("#61B045");
                }
                else if (model.Name.ToString() == "EmployeeExtension")
                {
                    color = System.Drawing.ColorTranslator.FromHtml("#0E6251");
                }
                else if (model.Name.ToString() == "AttestationRemediation")
                {
                    color = System.Drawing.ColorTranslator.FromHtml("#1A5276");
                }
                else if (model.Name.ToString() == "EmployeeCreation")
                {
                    color = System.Drawing.ColorTranslator.FromHtml("#F4D03F");
                }
                else if (model.Name.ToString() == "Rehire")
                {
                    color = System.Drawing.ColorTranslator.FromHtml("#F39C12");
                }
                else if (model.Name.ToString() == "Promotion")
                {
                    color = System.Drawing.ColorTranslator.FromHtml("#C39BD3");
                }
                else if (model.Name.ToString() == "ExtendedRequest")
                {
                    color = System.Drawing.ColorTranslator.FromHtml("#0E6251");
                }
                else if (model.Name.ToString() == "GamingApplicationRemoval")
                {
                    color = System.Drawing.ColorTranslator.FromHtml("#922B21");
                }
                else if (model.Name.ToString() == "OffBoardingDay2")
                {
                    color = System.Drawing.ColorTranslator.FromHtml("#EDBB99");
                }
                else if (model.Name.ToString() == "OffBoardingDay3")
                {
                    color = System.Drawing.ColorTranslator.FromHtml("#F6DDCC");
                }
                else if (model.Name.ToString() == "Demotion")
                {
                    color = System.Drawing.ColorTranslator.FromHtml("#7F8C8D");
                }

                resourceTypes.Items.Add(new SeriesItem() { Name = model.Name.ToString(), YValue = model.Y, BackgroundColor = color, });
                
            }
        }
    }
}