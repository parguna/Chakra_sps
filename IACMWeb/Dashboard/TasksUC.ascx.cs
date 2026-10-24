using Application;

using DI.Chakra.Infrastructure.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Dashboard
{
    public partial class TasksUC : UIBaseUC
    {
        SessionStatusImpl m_session = null;
        private static StatusType taskStatus = StatusType.Pending;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                DashBoardHeaderUC.IsChartType = false;
                m_session = new SessionStatusImpl();

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("FromDate", null);
                parameter.Add("ToDate", null);
                parameter.Add("actionUserId", m_session.User.Id.ToString());
                parameter.Add("taskStatus", StatusType.Pending.ToString());
                IList<Dto.Presentation.PresentationTask> taskDetailsList = DomainApiClient<Dto.Presentation.PresentationTask>.GetByAction<IList<Dto.Presentation.PresentationTask>>(DomainApiAction.Tasks.Presentation.PresentationTask.GetTaskDetailsByTaskStatus, parameter);


                IList<Dto.Presentation.PresentationTask> updatedDatasource = new List<Dto.Presentation.PresentationTask>();

                foreach (Dto.Presentation.PresentationTask items in taskDetailsList)
                {
                    if ((items.IsGroup && items.AssignedTo == m_session.User.Id) || (items.IsGroup == false && items.AssignedTo == null))
                    {
                        updatedDatasource.Add(items);
                    }
                }
                if (updatedDatasource != null && updatedDatasource.Count > 0)
                {
                    TaskRepeater.DataSource = updatedDatasource.OrderByDescending(c => c.CreatedDate);
                    if (updatedDatasource.Count < 10)
                        DashBoardHeaderUC.DashBoardRowCount = "0" + updatedDatasource.Count;
                    else
                        DashBoardHeaderUC.DashBoardRowCount = updatedDatasource.Count.ToString();
                    ViewState["DataSource"] = updatedDatasource.OrderByDescending(c => c.CreatedDate).ToList();
                    TaskRepeater.DataBind();
                }
                else
                {
                    DashBoardHeaderUC.DashBoardRowCount = "0";
                }
            }
            else
            {
                var dsResult = ViewState["DataSource"] as List<Dto.Presentation.PresentationTask>;
                if (dsResult != null && dsResult.Count < 10)
                    DashBoardHeaderUC.DashBoardRowCount = "0" + dsResult.Count;
                else
                {
                    if(dsResult != null)
                        DashBoardHeaderUC.DashBoardRowCount = dsResult.Count.ToString();
                    else
                    {
                        DashBoardHeaderUC.DashBoardRowCount = "0";
                    }
                }
                TaskRepeater.DataSource = dsResult;
                TaskRepeater.DataBind();
            }
        }

        protected void lnkTaskDetail_Click(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            this.TypeofRequest = btn.CommandName;
            //this.ContextObjectId = (long)Convert.ToInt32(btn.CommandArgument);
            SessionStatusImpl _m_session = new SessionStatusImpl();
            _m_session.RedirectFrom = "Home";
            Response.Redirect("~/Task/DetailType.aspx?contextId=" + btn.CommandArgument + "&TypeofRequest=" + btn.CommandName + "&ddltaskstatus=0&source=home&showalltask=0");
        }

        protected void TaskRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
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

        protected void TaskRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if(e.CommandName == "PendingTask")
            {
                var label = TaskRepeater.Items[e.Item.ItemIndex].FindControl("lblRequestType") as Label;
                this.TypeofRequest = label.Text;
                //this.ContextObjectId = (long)Convert.ToInt32(btn.CommandArgument);
                SessionStatusImpl _m_session = new SessionStatusImpl();
                _m_session.RedirectFrom = "Home";
                Response.Redirect("~/Task/DetailType.aspx?contextId=" + e.CommandArgument + "&TypeofRequest=" + this.TypeofRequest + "&ddltaskstatus=0&source=home&showalltask=0");
            }
        }
    }
}