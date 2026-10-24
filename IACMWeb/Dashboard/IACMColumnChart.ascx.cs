using Application;

using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Dashboard
{
    public partial class IACMColumnChart : DI.Chakra.Web.UIBaseUC
    {
        #region Class MemberVariable
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
        #endregion
        public int Year
        {

            get
            {
                if (ViewState["Year"] == null)
                {
                    return -1;
                }
                return (int)ViewState["Year"];
            }

            set
            {

                ViewState["Year"] = value;

            }

        }
        public int MonthNumber
        {

            get
            {
                if (ViewState["Month"] == null)
                {
                    return -1;
                }
                return (int)ViewState["Month"];
            }

            set
            {

                ViewState["Month"] = value;

            }

        }
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
                DashBoardHeaderUC.IsChartType = true;
                IList<int> years = DomainApiClient<Dto.AccessRequest>.GetByAction<IList<int>>(DomainApiAction.Provisioning.AccessRequest.GetYearsByActivationDate, null);
                if (years.Count > 0)
                {
                    drpYear.DataSource = years;
                    drpYear.DataBind();
                    drpYear.SelectedValue = DateTime.Now.Year.ToString();
                    Year = Convert.ToInt32(drpYear.SelectedValue);
                }
                else
                {
                    drpYear.Items.Clear();
                    drpYear.Items.Add(new ListItem("-- Select Year --", "-1"));
                    drpYear.Items.Add(new ListItem(DateTime.Now.Year.ToString(), DateTime.Now.Year.ToString()));
                    drpYear.SelectedValue = DateTime.Now.Year.ToString();
                    Year = Convert.ToInt32(drpYear.SelectedValue);
                }
                ViewState["Year"] = Convert.ToInt32(drpYear.SelectedValue);
                PopulateAccessRequestSourceByYear(Convert.ToInt32(drpYear.SelectedValue));
            }
        }
        private void PopulateAccessRequestSourceByYear(int Year)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("year", Year.ToString());
            DataTable chartData = DomainApiClient<AccessRequestSourceByMonth>.GetByAction<DataTable>(DomainApiAction.Common.AccessRequestSourceByMonth.GetAccessRequestSourceByYear, parameter);
            AccessRequestSourceColumnChart.DataSource = chartData;
            AccessRequestSourceColumnChart.DataBind();
        }
        public void BindData()
        {
            int year = (int)ViewState["Year"];
            int month = (int)ViewState["Month"];
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("year", ((int)ViewState["Year"]).ToString());
            parameter.Add("month", ((int)ViewState["Month"]).ToString());
            parameter.Add("RequestSource", RequestSource.ToString());
            IList<AccessRequestSourceChartData> chartData = DomainApiClient<AccessRequestSourceChartData>.GetByAction<IList<AccessRequestSourceChartData>>(DomainApiAction.Common.AccessRequestSourceChartData.GetAccessRequestSourceByMonth, parameter);
            if (chartData.Count > 0)
            {
                //RadAccessRequestSourceMonthDetails.DataSource = chartData;
                //RadAccessRequestSourceMonthDetails.DataBind();
            }
        }

        protected void drpYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            int year = Convert.ToInt32(drpYear.SelectedValue);
            ViewState["Year"] = year;
            PopulateAccessRequestSourceByYear(year);
        }

        protected void RadAccessRequestSourceMonthDetails_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (ViewState["Month"] != null)
            {
                int year = (int)ViewState["Year"];
                int month = (int)ViewState["Month"];
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("year", ((int)ViewState["Year"]).ToString());
                parameter.Add("month", ((int)ViewState["Month"]).ToString());
                parameter.Add("RequestSource", RequestSource.ToString());
                IList<AccessRequestSourceChartData> chartData = DomainApiClient<AccessRequestSourceChartData>.GetByAction<IList<AccessRequestSourceChartData>>(DomainApiAction.Common.AccessRequestSourceChartData.GetAccessRequestSourceByMonth, parameter);
                if (chartData.Count > 0)
                {
                    //RadAccessRequestSourceMonthDetails.DataSource = chartData;
                }
            }
        }
    }
}