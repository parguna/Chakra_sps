using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Dto;
using Application;
using DI.Chakra.Logger;

namespace DI.Chakra.Web.Mail.Template.UC
{
    public partial class ListUC : UIBaseUC
    {
        private static IList<MailTemplate> Maillist;
        #region Method
        private void BindMailList()
        {
            Maillist = DomainApiClient<MailTemplate>.GetAll();
            MailRadGrid.DataSource = Maillist;
        }
        private void delete()
        {
            if (!ContextObjectId.HasValue)
                return;
            DomainApiClient<MailTemplate>.DeleteById(this.ContextObjectId.Value);
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "MailTemplate Deleted Successfully");
            MailRadGrid.Rebind();
        } 
        #endregion

        #region Event

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }
        protected void MailRadGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindMailList();
        }
        #endregion

        protected void MailRadGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = MailRadGrid.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }
        protected void MailCancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Mail Template - Delete");
                delete();
            }

            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                MailRadGrid.ExportSettings.FileName = "Mail-Template";
                MailRadGrid.Export(MailRadGrid,e.ActionType);
            }

            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                MailRadGrid.Export<IList<MailTemplate>>(Maillist, e.ActionType, "Task");
            }

        }

        protected void btncancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
    }
}