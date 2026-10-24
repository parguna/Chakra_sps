using Application;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class SendNotification : UIBaseUC
    {
        #region Members
        SessionStatusImpl _sessionStatus = new SessionStatusImpl();
        private Dto.Task task = null;
        private long? _requestId;
        private long _taskId;
        static List<Dto.Presentation.PresentationRequestActionUsers> NotifiableUsers = 
                                            new List<Dto.Presentation.PresentationRequestActionUsers>();
        #endregion

        #region Public Properties
        public DI.Chakra.Dto.Task Task
        {
            set
            {
                task = value;
            }
        }

        public long? RequestId
        {
            get { return _requestId; }
            set { _requestId = value; }
        }

        public long TaskId
        {
            get { return _taskId; }
            set { _taskId = value; }
        }
        #endregion

        #region Page & Control Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                txtFrom.Text = _sessionStatus.User.DisplayName;
                BindNotifiableUsers(RequestId);
            }
        }
        #endregion

        #region Methods
        public void BindNotifiableUsers(long? requestId, string RequestSource = null)
        {
            ddlTo.Items.Clear();
            if (requestId != null)
            {
                string source = DI.Chakra.Infrastructure.Constants.AccessRequest;
                if (!string.IsNullOrEmpty(RequestSource))
                {
                    source = RequestSource;
                }
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("requestId", requestId.ToString());
                param.Add("source", source);
                NotifiableUsers = DomainApiClient<Dto.Presentation.PresentationRequestActionUsers>.GetByAction<List<Dto.Presentation.PresentationRequestActionUsers>>(DomainApiAction.Notification.PresentationRequestActionUsers.GetActionUsersByRequestId, param);
                long loginUserId = _sessionStatus.User.Id;
                foreach (Dto.Presentation.PresentationRequestActionUsers usr in NotifiableUsers.Where(u=> u.ActionUserId != loginUserId))
                { 
                        ListItem item = new ListItem(usr.ActionUser, usr.ActionUserId.ToString());
                        if ((ddlTo.Items.FindByValue(usr.ActionUserId.ToString()) == null) && usr.ActionUserId!=null)
                            ddlTo.Items.Add(item); 
                }
                ddlTo.Items.Insert(0, "---Select---");
                ddlTo.Items.FindByValue("---Select---").Selected = true;
            }
        }

        public bool IsSendNotificationRequired()
        {
            if (ddlTo.Items.Count > 1)
                return true;
            return false;
        }

        public Dto.Notification GetNotificationDetails()
        {
            Dto.Notification notification = new Dto.Notification();
            notification.Subject = txtSubject.Text;
            notification.Content = txtContent.Text;
            notification.SenderId = _sessionStatus.User.Id;
            notification.ReceiverId = Convert.ToInt64(ddlTo.SelectedItem.Value);
            notification.SenderTaskId = GetActionUserTaskId(_sessionStatus.User.Id);
            notification.ReceiverTaskId = GetActionUserTaskId(Convert.ToInt64(ddlTo.SelectedItem.Value));

            List<NotificationAttachment> attachments = new List<NotificationAttachment>();
            if (AsyncUploadAttach.UploadedFiles.Count > 0)
            {
                foreach (Telerik.Web.UI.UploadedFile file in AsyncUploadAttach.UploadedFiles)
                {
                    NotificationAttachment na = new NotificationAttachment();
                    na.ContentType = file.ContentType;
                    na.FileName = file.FileName;
                    byte[] fileData = new byte[file.ContentLength];
                    file.InputStream.Read(fileData, 0, Convert.ToInt32(file.ContentLength));
                    na.Attachment = fileData;
                    attachments.Add(na);
                }
                notification.NotificationAttachment = attachments;
            }
            return notification;
        }

        private long? GetActionUserTaskId(long userID)
        {
            return NotifiableUsers.Find(n => n.ActionUserId == userID).TaskId;
        }

        public void SetControls(string parentSubject)
        {
            txtSubject.Text = "Re ::" + parentSubject;
        }

        public void ClearControls()
        {
            ddlTo.ClearSelection();
            txtSubject.Text = "";
            txtContent.Text = "";
            AsyncUploadAttach.UploadedFiles.Clear();
        }

        public string ValidateNotification()
        {
            string errorMessage = string.Empty;
            string obj = (string.IsNullOrEmpty(txtSubject.Text) && (string.IsNullOrEmpty(txtContent.Text) && (string.IsNullOrEmpty(ddlTo.SelectedItem.Value) || ddlTo.SelectedValue == "---Select---")) ? "All" :
                ((string.IsNullOrEmpty(txtSubject.Text) && (string.IsNullOrEmpty(txtContent.Text))) ? "Subject&Content" : ((string.IsNullOrEmpty(txtSubject.Text) && (string.IsNullOrEmpty(ddlTo.SelectedItem.Value) || ddlTo.SelectedValue == "---Select---")) ? "To&Subject" :
                ((string.IsNullOrEmpty(txtContent.Text) && (string.IsNullOrEmpty(ddlTo.SelectedItem.Value) || ddlTo.SelectedValue == "---Select---"))) ? "To&Content" :
                (string.IsNullOrEmpty(txtSubject.Text) ? "Subject" : (string.IsNullOrEmpty(txtContent.Text) ? "Content" : (string.IsNullOrEmpty(ddlTo.SelectedItem.Value) || ddlTo.SelectedValue == "---Select---") ? "To" : "")))));
            switch (obj)
            {
                case "All":
                    errorMessage = "To,Subject & Content should not be empty";
                    break;
                case "Subject&Content":
                    errorMessage = "Subject & Content should not be Empty";
                    break;
                case "To&Subject":
                    errorMessage = "To & Subject should not be Empty";
                    break;
                case "To&Content":
                    errorMessage = "To & Content should not be Empty";
                    break;
                case "Subject":
                    errorMessage = "Subject should not be Empty";
                    break;
                case "Content":
                    errorMessage = "Content should not be Empty";
                    break;
                case "To":
                    errorMessage = "To should not be Empty";
                    break;
            }
            return errorMessage;
        }
        #endregion
    }
}