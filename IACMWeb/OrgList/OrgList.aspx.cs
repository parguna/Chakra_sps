using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Master
{
    public partial class OrgList : DI.Chakra.Web.UIBasePage, IContextObjectRequired
    {

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindOrgStructure();
            }

            //   this.OrgChartUC.OnAccountInfoClick += OrgChartUC_OnAccountInfoClick;
        }

        private void OrgChartUC_OnAccountInfoClick(object sender, EventArgs e)
        {
            // if (this.OrgChartUC.FindControl("RadWinAccessDetails") != null)
            //   ((RadWindow)this.OrgChartUC.FindControl("RadWinAccessDetails")).VisibleOnPageLoad = true;
        }

        protected void OrgStructure_NodeClick(object sender, Telerik.Web.UI.RadTreeNodeEventArgs e)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("orgId", e.Node.Value);
            long headid = DomainApiClient<Dto.NOrganisationHead>.GetByAction<long>(DomainApiAction.Org.NOrganisationHead.GetOrganisationHead, parameter);

            if (headid > 0)
            {
                Message.Text = "";
                OrgChartUC.OrgHeadId = headid;
                OrgChartUC.ParentOrgId = Convert.ToInt64(e.Node.Value);
                OrgChartUC.BindOrgChart(headid,Convert.ToInt64(e.Node.Value));
            }
            else
                Message.Text = "No Employees found for the organization.";

            //Page.ClientScript.RegisterStartupScript(this.GetType(), "OrgChartScrollPosition", " ClientNodeClick();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OrgChartScrollPosition", "ClientNodeClick();", true);
        }

        protected void OrgStructure_NodeExpand(object sender, Telerik.Web.UI.RadTreeNodeEventArgs e)
        {
            if (e.Node.Nodes.Count == 0)
                PopulateNodeOnDemand(e);
        }

        protected void OrgStructure_NodeDataBound(object sender, RadTreeNodeEventArgs e)
        {

            Dto.NOrganisation org = (Dto.NOrganisation)e.Node.DataItem;

            e.Node.Value = org.Id.ToString();
            if (org.HasChildren)
                e.Node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;

        }

        #endregion

        #region Methods

        private void BindOrgStructure()
        {

            List<Dto.NOrganisation> parentOrgs = DomainApiClient<Dto.NOrganisation>.GetByAction<List<Dto.NOrganisation>>(DomainApiAction.Org.Organisation.GetParentOrgs, null);

            if (parentOrgs.Exists(o => o.Name == "ALL")) parentOrgs.RemoveAll(o => o.Name == "ALL");

            if (parentOrgs.Count > 0)
            {
                OrgStructure.DataSource = parentOrgs;
                OrgStructure.DataBind();
            }
        }

        private static void PopulateNodeOnDemand(RadTreeNodeEventArgs e)
        {

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("parentOrgId", e.Node.Value);
            List<Dto.NOrganisation> lstChildNodes = DomainApiClient<Dto.NOrganisation>.GetByAction<List<Dto.NOrganisation>>(DomainApiAction.Org.Organisation.GetByParentOrgId, parameter);

            if (lstChildNodes != null && lstChildNodes.Count > 0)
            {
                foreach (Dto.NOrganisation org in lstChildNodes)
                {
                    RadTreeNode node = new RadTreeNode();
                    node.Text = org.Name;
                    node.Value = org.Id.ToString();

                    if (org.HasChildren)
                        node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;

                    e.Node.Nodes.Add(node);
                }
                e.Node.Expanded = true;
            }

        }

        #endregion

    }
}