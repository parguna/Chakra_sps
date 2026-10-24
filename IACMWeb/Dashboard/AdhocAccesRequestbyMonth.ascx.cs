using Application;

using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using DI.Chakra.Dto;

namespace DI.Chakra.Web.Dashboard
{
    public partial class AdhocAccesRequestbyMonth : DI.Chakra.Web.UIBaseUC
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
                PopulateYearMonth();

            }
        }


        protected void drpYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            Year = Convert.ToInt32(drpYear.SelectedValue);
            PopulateAccessRequestSourceByMonth(Year, MonthNumber);
        }


        public void PopulateYearMonth()
        {
            IList<int> years =  DomainApiClient<Dto.AccessRequest>.GetByAction<IList<int>>(DomainApiAction.Provisioning.AccessRequest.GetYearsByActivationDate,null);
            IList<int> Months = DomainApiClient<Dto.AccessRequest>.GetByAction<IList<int>>(DomainApiAction.Provisioning.AccessRequest.GetMonthsByActivationDate,null);
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
            Year = Convert.ToInt32(drpYear.SelectedValue);

           
            Dictionary<int, string> dicMonth = new Dictionary<int, string>();
            dicMonth.Add(0, "-- Select Month --");
            dicMonth.Add(1, "January");
            dicMonth.Add(2, "February");
            dicMonth.Add(3, "March");
            dicMonth.Add(4, "April");
            dicMonth.Add(5, "May");
            dicMonth.Add(6, "June");
            dicMonth.Add(7, "July");
            dicMonth.Add(8, "August");
            dicMonth.Add(9, "September");
            dicMonth.Add(10, "October");
            dicMonth.Add(11, "November");
            dicMonth.Add(12, "December");

           
            drpMonth.DataTextField = "Value";
            drpMonth.DataValueField = "Key";
            drpMonth.DataSource = (object)dicMonth;
            drpMonth.DataBind();

            drpMonth.SelectedValue = DateTime.Now.Month.ToString();
            MonthNumber = Convert.ToInt32(drpMonth.SelectedValue);
            PopulateAccessRequestSourceByMonth(Year, MonthNumber);
        }

        protected void drpMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            MonthNumber = Convert.ToInt32(drpMonth.SelectedValue);
            PopulateAccessRequestSourceByMonth(Year, MonthNumber);
        }
        private void PopulateAccessRequestSourceByMonth(int Year, int Month)
        {
            if (drpYear.SelectedValue != "-1" && drpMonth.SelectedValue != "-1")
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("Year", Year.ToString());
                parameter.Add("Month", Month.ToString());
                parameter.Add("userId", Convert.ToString(sessionStatus.UserId));

                DataTable chartData = DomainApiClient<AdhocRequestAccountTyebyMonth>.GetByAction<DataTable>(DomainApiAction.Common.AdhocRequestAccountTyebyMonth.GetUserAccessRequestByAccountType,parameter);
                AdhocRequestColumnChart.DataSource = chartData;
                AdhocRequestColumnChart.DataBind();
            }

        }
    }
}