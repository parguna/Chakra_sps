using Application;
using System;
using System.Collections.Generic;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Web;

namespace DI.Chakra.Web.Notification.UC
{
    public partial class DetailUC : UIBaseUC, IAccessControl
    {
        SessionStatusImpl _sessionStatus = new SessionStatusImpl();
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack && ContextObjectId != null)
            {
                SetPage();
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.MarkAsRead)
            {
                MarkAsRead();
            }
            else if (e.ActionType == ACLAction.Reply)
            {
                Dto.Notification parentNotification = DomainApiClient<Dto.Notification>.GetById(ContextObjectId.Value, true);
                UCReplyNotification.BindNotifiableUsers(parentNotification.RequestId);
                UCReplyNotification.SetControls(parentNotification.Subject);
                RadWinReplyNotification.VisibleOnPageLoad = true;
            }
        }

        private void SetPage()
        {
            if (ContextObjectId > 0)
            {
                Dto.Notification notification = DomainApiClient<Dto.Notification>.GetById(ContextObjectId.Value);
                Request.Value = notification.RequestId.ToString();

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("notificationId", Convert.ToString(ContextObjectId.Value.ToString()));
                List<Dto.Presentation.PresentationNotification> taskNotifications =
                    DomainApiClient<Dto.Presentation.PresentationNotification>.GetByAction<List<Dto.Presentation.PresentationNotification>>
                   (DomainApiAction.Notification.PresentationNotification.GetReplyChainById, parameter);

                if (taskNotifications != null && taskNotifications.Count > 0)
                {
                    radgridNotificationDetail.DataSource = taskNotifications;
                    radgridNotificationDetail.DataBind();

                    SetRadGridDefaultSelection();
                }
            }
        }

        protected void radgridNotificationDetail_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radgridNotificationDetail.SelectedItems.Count > 0)
            {
                DisplayConent();
            }
        }

        public void MarkAsRead()
        {
            Dto.Notification notification = DomainApiClient<Dto.Notification>.GetById(ContextObjectId.Value);
            if (!notification.IsRead)
            {
                notification.IsRead = true;
                DomainApiClient<Dto.Notification>.Save(notification);
                MasterPage.ShowAlert(AlertType.Success, "Notification marked as read successfully.");
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Warning, "Notification already read.");
            }
        }

        protected void btnReply_Click(object sender, EventArgs e)
        {
            string validationMessage = UCReplyNotification.ValidateNotification();
            if (!string.IsNullOrEmpty(validationMessage))
            {
                RadWinReplyNotification.VisibleOnPageLoad = true;
                MasterPage.ShowAlert(AlertType.Warning, validationMessage);
                return;
            }
            Dto.Notification parentNotification = DomainApiClient<Dto.Notification>.GetById(ContextObjectId.Value);
            parentNotification.IsRead = true;
            parentNotification.IsReplied = true;
            Dto.Notification notification = UCReplyNotification.GetNotificationDetails();
            notification.RequestId = parentNotification.RequestId;
            notification.ParentId = parentNotification.Id;
            DomainApiClient<Dto.Notification>.Save(notification);
            DomainApiClient<Dto.Notification>.Save(parentNotification);
            SetReplyNotificationDisplay(false);
            MasterPage.ShowAlert(AlertType.Success, "Notification Sent Successfully.");
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Notification/List.aspx");
        }

        public void SetRadGridDefaultSelection()
        {
            (radgridNotificationDetail.MasterTableView.Items[0] as GridDataItem).FireCommandEvent("Select", String.Empty);
            if (radgridNotificationDetail.Items[0]["SenderName"].Text == _sessionStatus.User.DisplayName)
                TaskId.Value = radgridNotificationDetail.Items[0]["SenderTaskId"].Text;
            else
                TaskId.Value = radgridNotificationDetail.Items[0]["ReceiverTaskId"].Text;

            DisplayConent();
        }

        public void DisplayConent()
        {
            GridDataItem g = radgridNotificationDetail.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
            txtContent.Text = DomainApiClient<Dto.Notification>.GetById(this.ContextObjectId.Value).Content;
        }

        protected void btnCancelReply_Click(object sender, EventArgs e)
        {
            SetReplyNotificationDisplay(false);
        }

        private void SetReplyNotificationDisplay(bool IsDisplay)
        {
            UCReplyNotification.ClearControls();
            RadWinReplyNotification.VisibleOnPageLoad = IsDisplay;
        }

        protected void radgridNotificationDetail_ItemCommand(object sender, GridCommandEventArgs e)
        {

            GridDataItem ditem = (GridDataItem)e.Item;
            this.ContextObjectId = (long)ditem.GetDataKeyValue("Id");

            if (e.CommandName == "file")
            {
                Dto.Notification parentNotification = DomainApiClient<Dto.Notification>.GetById(ContextObjectId.Value, true);

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

        protected void radgridNotificationDetail_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                ImageButton image = (ImageButton)item.FindControl("AttachmentImage");
                if (item["HasAttachments"].Text == Boolean.TrueString)
                {
                    image.Visible = true;
                }
                else
                    image.Visible = false;
            }
        }

        protected void Task_Click(object sender, EventArgs e)
        {

            if (this.ContextObjectId != null)
            {
                var accessRequest = DomainApiClient<Dto.AccessRequest>.GetById(Convert.ToInt64(Request.Value));
                if (accessRequest != null && accessRequest.AdHocAccessRequestId > 0)
                    Request.Value = accessRequest.AdHocAccessRequestId.ToString();
                if (_sessionStatus.UserId.ToString() == DomainApiClient<Dto.AccessRequest>.GetById(Convert.ToInt64(Request.Value)).RequesterId.ToString())
                    Response.Redirect(String.Format("~/Request/Detail.aspx?Id={0}&NotifyId={1}", Request.Value, ContextObjectId.Value));
                else
                    Response.Redirect(String.Format("~/Task/Detail.aspx?Id={0}&NotifyId={1}", TaskId.Value, ContextObjectId.Value));
            }
        }
    }
}