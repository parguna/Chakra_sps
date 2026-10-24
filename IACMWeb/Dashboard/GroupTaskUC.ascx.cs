using Application;
using DI.Chakra.Infrastructure.Enums;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;


namespace DI.Chakra.Web.Dashboard
{
    public partial class GroupTaskUC : UIBaseUC
    {
        SessionStatusImpl m_session = null;
        public string rowcount = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            m_session = new SessionStatusImpl();
            DashBoardHeaderUC.IsChartType = false;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("actionUserId", m_session.User.Id.ToString());
            IList<Dto.Presentation.PresentationTask> GrouptaskDetailsList = DomainApiClient<Dto.Presentation.PresentationTask>.GetByAction<IList<Dto.Presentation.PresentationTask>>(DomainApiAction.Tasks.Presentation.PresentationTask.GetAllGroupTask, parameter);

            if (GrouptaskDetailsList != null && GrouptaskDetailsList.Count > 0)
            {
                GroupTaskrepeater.DataSource = GrouptaskDetailsList.OrderByDescending(c => c.CreatedDate);
                rowcount = GrouptaskDetailsList.Count().ToString();
                if (rowcount.Length > -1)
                {
                    DashBoardHeaderUC.DashBoardRowCount = "0" + rowcount;
                }

                GroupTaskrepeater.DataBind();
            }
            else
            {
                DashBoardHeaderUC.DashBoardRowCount = "0";
            };
        }

        protected void lnkgroupTaskDetail_Click(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            this.TypeofRequest = btn.CommandName;
            //this.ContextObjectId = (long)Convert.ToInt32(btn.CommandArgument);
            SessionStatusImpl _m_session = new SessionStatusImpl();
            _m_session.RedirectFrom = "Home";
            Response.Redirect("~/Task/DetailType.aspx?contextId=" + btn.CommandArgument + "&&TypeofRequest=" + btn.CommandName + "&ddltaskstatus=0&source=home&showalltask=0");
        }

        protected void GroupTaskrepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = (Dto.Presentation.PresentationTask)e.Item.DataItem;
                Label accessRequestIdLabel = e.Item.FindControl("lblRequestID") as Label;
                Label lblRequestedFor = e.Item.FindControl("lblRequestFor") as Label;

                if (item != null && item.AccessRequestID != null)
                {
                    accessRequestIdLabel.Visible = true;
                }
                else
                {
                    accessRequestIdLabel.Visible = false;
                }

                if (item != null && !string.IsNullOrWhiteSpace(item.TaskSource) && item.TaskSource.ToLower().Contains("bulk"))
                {
                    lblRequestedFor.Visible = false;
                }
                else
                {
                    lblRequestedFor.Visible = true;
                }
            }
        }
    }

}