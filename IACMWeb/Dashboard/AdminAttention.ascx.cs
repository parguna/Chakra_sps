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
    public partial class AdminAttention : UIBaseUC
    {

        SessionStatusImpl m_session = null;
        public string rowcount = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            DashBoardHeaderUC.IsChartType = false;
            m_session = new SessionStatusImpl();

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", m_session.User.Id.ToString());
            parameter.Add("AdminMode", true.ToString());
            IList<Dto.Attention> taskDetailsList = DomainApiClient<Dto.Attention>.GetByAction<IList<Dto.Attention>>("GetAttention", parameter);


            IList<Dto.Attention> updatedDatasource = taskDetailsList.ToList();
            var split = updatedDatasource.Count / 2;
            if (updatedDatasource != null && updatedDatasource.Count > 0)
            {
                TaskRepeater.DataSource = updatedDatasource.Take(split).ToList();
                TaskRepeater.DataBind();
                Repeater1.DataSource = updatedDatasource.Skip(split).ToList();
                Repeater1.DataBind();
            }
            else
            {
                DashBoardHeaderUC.DashBoardRowCount = "0";
            }
        }

        protected void lnkTaskDetail_Click(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            var commandName = btn.CommandName;
            //this.ContextObjectId = (long)Convert.ToInt32(btn.CommandArgument);
            SessionStatusImpl _m_session = new SessionStatusImpl();
            _m_session.RedirectFrom = "Home";

            if(commandName == "MyTask")
                Response.Redirect("~/Task/List.aspx");
            else if (commandName == "MyRequest")
                Response.Redirect("~/Request/List.aspx");
            else if (commandName == "MyNotification")
                Response.Redirect("~/Notification/List.aspx");
            else if (commandName == "MyDelegation")
                Response.Redirect("~/AdvanceDelegation/List.aspx");
            else if (commandName == "Task")
                Response.Redirect("~/Prov/Task/List.aspx");
            else if (commandName == "Request")
                Response.Redirect("~/Prov/Request/List.aspx");
            else if (commandName == "JAC")
                Response.Redirect("~/JobAllocationCenter/List.aspx");
            else if(commandName == "AlertCenter")
                Response.Redirect("~/AlertCenter/List.aspx");
            else
                Response.Redirect("~/Home/Home.aspx");
        }
       
    }
}