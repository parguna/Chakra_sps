using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Dashboard
{
    public partial class AccessRequestColumnChartByUser : DI.Chakra.Web.UIBaseUC
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
            parameter.Add("year",Convert.ToString(Year));
            parameter.Add("userId", Convert.ToString(sessionStatus.UserId));

            DataTable chartData = DomainApiClient<Dto.AccessRequestSourceByMonth>.GetByAction<DataTable>
            (DomainApiAction.Common.AccessRequestSourceByMonth.GetAccessRequestSourceForUserByYear, parameter);

            AccessRequestSourceColumnChart.DataSource = chartData;
            AccessRequestSourceColumnChart.DataBind();

        }
       
        protected void drpYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            Year = Convert.ToInt32(drpYear.SelectedValue);
            PopulateAccessRequestSourceByYear(Year);
        }

        
    }
}