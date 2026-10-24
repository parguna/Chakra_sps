using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class NOrgDetail : System.Web.UI.UserControl
    {
        private const string V_ORGID = "V_ORGID";

        public long OrganisationId
        {
            get
            {
                if (ViewState[V_ORGID] != null)
                {
                    return (long)ViewState[V_ORGID];
                }
                return long.MinValue;
            }
            set
            {
                ViewState[V_ORGID] = value;
                SetDetail();
            }
        }
        private void SetDetail()
        {
            rdPnlOrgDetail.CollapseAllItems();
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("OrgId", OrganisationId.ToString());
            var orgChart = DomainApiClient<Dto.NOrganisationPresentation>.GetByAction<List<NOrganisationPresentation>>(DomainApiAction.Org.NOrganisation.GetOrgChartForDisplay, param);

            rdPnlOrgDetail.Items.Clear();
            RadPanelItem orgItems = new RadPanelItem();
            var orgText = orgChart.First().OrganisationScope;

            orgItems.Text = string.Format("{0} : {1}", orgText == null ? string.Empty : orgText.Scope, orgChart.First().Name.ToString());
            orgItems.Expanded = false;

            for (int i = 1; i < orgChart.Count; i++)
            {
                orgText = orgChart[i].OrganisationScope;

                RadPanelItem orgItem = new RadPanelItem(string.Format("{0} : {1}", orgText == null ? string.Empty : orgText.Scope, orgChart[i].Name.ToString()));
                orgItem.Expanded = false;
                orgItems.Items.Add(orgItem);
                orgItems.Expanded = false;
            }
            orgItems.Expanded = false;
            rdPnlOrgDetail.Items.Add(orgItems);
            rdPnlOrgDetail.CollapseAllItems();
            //rdPnlOrgDetail.DataSource = orgChart;
            rdPnlOrgDetail.DataBind();
        }

        protected void rdPnlOrgDetail_PreRender(object sender, EventArgs e)
        {
            if (rdPnlOrgDetail.Items.Count > 0)
                rdPnlOrgDetail.Items[0].Expanded = false;
        }

        protected void rdPnlOrgDetail_DataBound(object sender, EventArgs e)
        {
            if (rdPnlOrgDetail.Items.Count > 0)
                rdPnlOrgDetail.Items[0].Expanded = false;
        }

        //protected void rdPnlOrgDetail_ItemDataBound(object sender, Telerik.Web.UI.RadPanelBarEventArgs e)
        //{
        //    NOrganisationPresentation row = (NOrganisationPresentation)e.Item.DataItem;
        //    e.Item.Text = string.Format("{0} : {1}", row.OrganisationScope.Scope, row.Name.ToString());            
        //    e.Item.ToolTip = e.Item.Text;
        //}
    }
}