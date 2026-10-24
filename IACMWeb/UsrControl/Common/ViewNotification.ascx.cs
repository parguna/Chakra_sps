using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class ViewNotification : System.Web.UI.UserControl
    {
        List<Dto.Presentation.PresentationNotification> notifications = new List<Dto.Presentation.PresentationNotification>();

        private const string V_ID = "REQUEST_ID";
        bool isSort = false;
        public long? RequestId
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return -1;
                return (long)ViewState[V_ID];
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void radgridViewNotifications_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            LoadNotifications();
            LoadContent();
        }

        public void BindNotifications()
        {
            if (this.RequestId != null)
            {
                LoadNotifications();
                radgridViewNotifications.DataBind();
                LoadContent();
            }
        }

        public void LoadContent()
        {
            txtContent.Text = "";
            if (notifications != null && notifications.Count > 0)
            {
                lblContent.Visible = true;
                txtContent.Visible = true;
                SetRadGridDefaultSelection(radgridViewNotifications);
            }
            else
            {
                lblContent.Visible = false;
                txtContent.Visible = false;
            }
        }

        public void LoadNotifications()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("requestId", Convert.ToString(this.RequestId));
            notifications =
                 DomainApiClient<Dto.Presentation.PresentationNotification>.GetByAction<List<Dto.Presentation.PresentationNotification>>
                (DomainApiAction.Notification.PresentationNotification.GetByRequestId, parameter);
            radgridViewNotifications.DataSource = notifications;
        }

        protected void radgridViewNotifications_DetailTableDataBind(object sender, Telerik.Web.UI.GridDetailTableDataBindEventArgs e)
        {
            GridDataItem parentItem = e.DetailTableView.ParentItem;
            long notificationId = Convert.ToInt32(parentItem.GetDataKeyValue("Id"));

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("notificationId", Convert.ToString(notificationId));
            List<Dto.Presentation.PresentationNotification> taskNotifications =
                 DomainApiClient<Dto.Presentation.PresentationNotification>.GetByAction<List<Dto.Presentation.PresentationNotification>>
                (DomainApiAction.Notification.PresentationNotification.GetNotificationsById, parameter);

            e.DetailTableView.DataSource = taskNotifications.Where(p => p.ParentId != null);
        }

        public void SetRadGridDefaultSelection(RadGrid radgridNotifications)
        {
            if (!isSort)
            {
                radgridNotifications.Items[0].Selected = true;
                DisplayConent(radgridNotifications);
            }
        }

        public void DisplayConent(RadGrid radgridNotifications)
        {
            GridDataItem g = radgridNotifications.SelectedItems[0] as GridDataItem;
            long id = (long)g.GetDataKeyValue("Id");
            txtContent.Text = DomainApiClient<Dto.Notification>.GetById(id).Content;
        }

        protected void radgridViewNotifications_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayConent(radgridViewNotifications);
        }

        protected void radgridViewNotifications_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "file")
            {
                GridDataItem ditem = (GridDataItem)e.Item;
                Dto.Notification parentNotification = DomainApiClient<Dto.Notification>.GetById((long)ditem.GetDataKeyValue("Id"), true);
                if (parentNotification.NotificationAttachment != null && parentNotification.NotificationAttachment.Count > 0)
                    ShowDocument(parentNotification.NotificationAttachment[0].FileName, parentNotification.NotificationAttachment[0].Attachment, parentNotification.NotificationAttachment[0].ContentType);
            }
        }

        private void ShowDocument(string fileName, byte[] fileContent, string contenttype)
        {
            HttpResponse response = HttpContext.Current.Response;
            response.Clear();
            response.ClearContent();
            response.ClearHeaders();
            response.Buffer = true;
            response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
            response.BinaryWrite(fileContent);
            response.End();
        }

        protected void radgridViewNotifications_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                ImageButton mimage = (ImageButton)item.FindControl("MAttachmentImage");
                ImageButton image = (ImageButton)item.FindControl("AttachmentImage");
                if (item["HasAttachments"].Text == Boolean.TrueString)
                {
                    if (item.OwnerTableView.Name == "Master")
                        mimage.Visible = true;
                    else
                        image.Visible = true;
                }
                else
                {
                    if (item.OwnerTableView.Name == "Master")
                        mimage.Visible = false;
                    else
                        image.Visible = false;
                }
            }
        }

        protected void UpdatePanelDownload_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                                    .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void radgridViewNotifications_SortCommand(object sender, GridSortCommandEventArgs e)
        {
            isSort = true;
        }
    }
}