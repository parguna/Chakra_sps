using Application;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.OrgChart.UC
{
    public partial class OrgChartUC : UIBaseUC
    {

        #region Members

        private SessionStatusImpl SessionUser = new SessionStatusImpl();
        private List<PresentationOrgChart> OrgChartNodes = new List<PresentationOrgChart>();
        private const string V_ISFROMORGLIST = "V_ISFROMORGLIST";
        private const string V_ORGHEADID = "V_ORGHEADID";
        private const string V_PARENTORGID = "V_PARENTORGID";

        #endregion

        #region Properties
        public bool isFromOrgList
        {
            get
            {
                if (ViewState[V_ISFROMORGLIST] == null)
                    return false;
                return (bool)ViewState[V_ISFROMORGLIST];
            }
            set
            {
                ViewState[V_ISFROMORGLIST] = value;
            }
        }

        public long OrgHeadId
        {
            get
            {
                if (ViewState[V_ORGHEADID] == null)
                    return 0;
                return (long)ViewState[V_ORGHEADID];
            }
            set
            {
                ViewState[V_ORGHEADID] = value;
            }
        }

        public long ParentOrgId
        {
            get
            {
                if (ViewState[V_PARENTORGID] == null)
                    return 0;
                return (long)ViewState[V_PARENTORGID];
            }
            set
            {
                ViewState[V_PARENTORGID] = value;
            }
        }

        #endregion

        // public event EventHandler OnAccountInfoClick;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!isFromOrgList)
            {
                GetTopmostOrgByHeadId(SessionUser.User.Id);
                BindOrgChart(SessionUser.User.Id, ParentOrgId);
            }
            else
            {
                btnCancel.Visible = false;
                if (OrgHeadId > 0 && ParentOrgId > 0)
                {
                    BindOrgChart(OrgHeadId, ParentOrgId);
                }
            }         
        }

        public void GetTopmostOrgByHeadId(long userId)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", userId.ToString());
            ParentOrgId = DomainApiClient<NOrganisation>.GetByAction<long>
                            (DomainApiAction.Org.Organisation.GetTopmostByHeadId, parameter);
        }

        public void BindOrgChart(long userID, long parentOrgId)
        {
            RadOrgChart1.Nodes.Clear();
            RadOrgChart1.RenderedFields.NodeFields.Clear();

            OrgChartNodes = GetOrgChart(userID, parentOrgId);

            if (OrgChartNodes.Count > 0)
            {
                RadOrgChart1.GroupEnabledBinding.NodeBindingSettings.DataFieldID = "Id";
                RadOrgChart1.GroupEnabledBinding.NodeBindingSettings.DataFieldParentID = "ParentId";
                RadOrgChart1.RenderedFields.NodeFields.Add(new Telerik.Web.UI.OrgChartRenderedField() { DataField = "OrgHeadCaption" });
                RadOrgChart1.GroupEnabledBinding.NodeBindingSettings.DataSource = OrgChartNodes;

                RadOrgChart1.GroupEnabledBinding.GroupItemBindingSettings.DataFieldID = "Id";
                RadOrgChart1.GroupEnabledBinding.GroupItemBindingSettings.DataFieldNodeID = "Id";
                RadOrgChart1.GroupEnabledBinding.GroupItemBindingSettings.DataSource = OrgChartNodes;
                RadOrgChart1.DataBind();
                RadOrgChart1.CollapseAllGroups();
                RadOrgChart1.CollapseAllNodes();
                RadOrgChart1.PersistExpandCollapseState = false;
            }
        }

        private List<PresentationOrgChart> GetOrgChart(long userId, long parentOrgId)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("usrId", userId.ToString());
            parameter.Add("parentOrgId", parentOrgId.ToString());
            return DomainApiClient<PresentationOrgChart>.GetByAction<List<PresentationOrgChart>>
                (DomainApiAction.Org.PresentationOrgChart.GetOrgChartNodes, parameter);
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            RadWinAccessDetails.VisibleOnPageLoad = false;
        }

        protected void lnkBtnAccInfo_Click(object sender, EventArgs e)
        {
            AccessDetailUC.EntityId = Convert.ToInt64(((LinkButton)sender).CommandArgument);
            AccessDetailUC.BindGrid();
            RadWinAccessDetails.VisibleOnPageLoad = true;
        }

        protected void RadOrgChart1_GroupItemDataBound(object sender, Telerik.Web.UI.OrgChartGroupItemDataBoundEventArguments e)
        {
            LinkButton AccInfolnk = (LinkButton)e.Item.FindControl("lnkBtnAccInfo");
            //AccInfolnk.Click += new System.EventHandler(this.lnkBtnAccInfo_Click);


            Image Img = (Image)e.Item.FindControl("OrgItemImg");
            if (Convert.ToInt64(AccInfolnk.CommandArgument) == 0)
            {
                AccInfolnk.Visible = false;
                Img.Visible = false;
            }
            else
            {
                AccInfolnk.Visible = true;
                Img.Visible = false;
            }
        }

        public string GetImage(object img)
        {
            if (((byte[])img).LongLength != 0)
            {
                return "data:image/png;base64," + Convert.ToBase64String((byte[])img);
            }
            else
                return "~/images/profile-placeholder.png";
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "My Profile/My Organisation Chart - Cancel");
            Response.Redirect("~/Profile/Profile.aspx");
        }
    }
}