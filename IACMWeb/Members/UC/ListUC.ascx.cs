using Telerik.Web.UI;
using System;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System.Collections.Generic;
using Application;

namespace DI.Chakra.Web.Members.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        private static List<Dto.Presentation.PresentationUser> list = new List<Dto.Presentation.PresentationUser>();
        #region Events
        protected void Users_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", (new SessionStatusImpl().UserId).ToString());
            list = DomainApiClient<Dto.Presentation.PresentationUser>
             .GetByAction<List<Dto.Presentation.PresentationUser>>(DomainApiAction.Endpoint.Presentation.PresentationUser.GetMembers, parameter);
            Users.DataSource = list;
        }      
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
           
        }

       
        protected void Users_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            GridDataItem g = Users.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        protected void Button1_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                Users.ExportSettings.FileName = "IDAS User";
                Users.Export(Users, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                Users.Export<List<Dto.Presentation.PresentationUser>>(list, e.ActionType, "IDAS User");
            }
        }

        protected void Users_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "ShowInfo")
            {
                GridDataItem item = e.Item as GridDataItem;
                long UserId = (long)item.GetDataKeyValue("Id");
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", UserId.ToString());
                User User = DomainApiClient<Dto.User>.GetByAction<User>
                    (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);
                if (User == null)
                    return;
                NUserDetailUC.User = User;
                UserDetail.VisibleOnPageLoad = true;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            UserDetail.VisibleOnPageLoad = false;
        }
        #endregion
    }
}