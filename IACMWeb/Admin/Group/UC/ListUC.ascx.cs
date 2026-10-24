using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Admin.Group.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        private static IList<Dto.Group> listGroup;
        #region Method
        private void BindGroups()
        {
            listGroup = DomainApiClient<Dto.Group>.GetAll();
            GroupRadGrid.DataSource = listGroup;
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;

            if (!IsPostBack)
            {
                GroupRadGrid.DataBind();
            }
        }


        protected void GroupRadGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindGroups();
        }

        #endregion

        protected void GroupRadGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = GroupRadGrid.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");


        }
        private void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            var groupId = (long)this.ContextObjectId;
            Dto.Group group = DomainApiClient<Dto.Group>.GetById(groupId,true);
            bool isExists = group != null ? true : false;
            if (isExists)
            {
                if (group.IsDefaultGroup)
                    MasterPage.ShowAlert(AlertType.Error, "Default Group cannot be deleted");
                else
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("id", Convert.ToString(groupId));

                    var msg = DomainApiClient<Dto.Group>.GetByAction<string>
                        (DomainApiAction.Group.DeleteGroupByID, parameter);
                    if (msg.Contains("Successfully"))
                    {
                        DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"Group:{group.GroupName}, IsDefaultGroup:{group.IsDefaultGroup}, Group Owner:{group.Owner?.DisplayName}, GroupDescription:{group.GroupDescription},EmailId :{group.EmailId}, Group Member:{group.Users?.Select(i=>i.DisplayName).ToList()} Deleted successfully");
                        MasterPage.ShowAlert(AlertType.Success, msg);
                    }
                    else
                        MasterPage.ShowAlert(AlertType.Warning, msg);
                }

                this.ContextObjectId = null;
            }
            //bool isExists = DomainApiClient<Dto.Group>.GetById((long)this.ContextObjectId) != null ? true : false;
            //if (isExists)
            //{
            //    DomainApiClient<Dto.Group>.DeleteById(this.ContextObjectId.Value);
            //    MasterPage.ShowAlert(AlertType.Success, "Group Deleted Successfully");
            //    this.ContextObjectId = null;
            //}
            GroupRadGrid.Rebind();
        }


        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "IDAS Group - Delete");
                delete();
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                GroupRadGrid.ExportSettings.FileName = "Group";
                GroupRadGrid.Export(GroupRadGrid,e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                GroupRadGrid.Export<IList<Dto.Group>>(listGroup, e.ActionType, "Group");
            }
        }

        protected void btncancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
    }
}