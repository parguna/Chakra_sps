using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Admin.Attestation.UC
{
    public partial class AttestationUploadUC : UIBaseUC
    {
        #region Memebers
        AlertType alertType = AlertType.Success;
        string failureMessage = "Uploaded file has error, check the Validation Message column";
        #endregion

        public long TaskId
        {
            get
            {
                if (ViewState["TaskId"] != null)
                {
                    return Convert.ToInt64(ViewState["TaskId"]);
                }
                return 0;
            }
            set
            {
                ViewState["TaskId"] = value;
            }
        }
        public bool IsTaskCompleted
        {
            get
            {
                if (ViewState["TaskId"] != null)
                {
                    return Convert.ToBoolean(ViewState["TaskId"]);
                }
                return false;
            }
            set
            {
                CommentsTextBox.Enabled = !value;

                ViewState["TaskId"] = value;
            }
        }
        public long CampaignId
        {
            get
            {
                if (ViewState["CampaignId"] != null)
                {
                    return Convert.ToInt64(ViewState["CampaignId"]);
                }
                return 0;
            }
            set
            {
                ViewState["CampaignId"] = value;
            }
        }

        public bool AllowAddComments
        {
            get
            {
                if (ViewState["AllowAddComments"] != null)
                {
                    return Convert.ToBoolean(ViewState["AllowAddComments"]);
                }
                return false;
            }
            set
            {
                ViewState["AllowAddComments"] = value;
            }
        }

        public string CurrentUser
        {
            get
            {
                if (ViewState["CurrentUser"] != null)
                {
                    return Convert.ToString(ViewState["CurrentUser"]);
                }
                return "";
            }
            set
            {
                ViewState["CurrentUser"] = value;
            }
        }
        public string CurrentUserName
        {
            get
            {
                if (ViewState["CurrentUserName"] != null)
                {
                    return Convert.ToString(ViewState["CurrentUserName"]);
                }
                return "";
            }
            set
            {
                ViewState["CurrentUserName"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void SaveData()
        {
            if (AllowAddComments)
            {
                List<AttestationCampaignAttachment> uploadedfiles = GetUploadedFiles();
                if (uploadedfiles != null)
                {
                    DomainApiClient<Dto.AttestationCampaignAttachment>.GetByAction<bool>
                                (DomainApiAction.Attestation.SaveAttachementList, null, uploadedfiles);

                    // DomainApiClient<Dto.AttestationCampaignAttachment>.SaveAttachementList(uploadedfiles);
                }
                if (!String.IsNullOrWhiteSpace(CommentsTextBox.Text))
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    long campaignId = 0;
                    if (this.CampaignId > 0)
                    {
                        campaignId = this.CampaignId;
                    }
                    else if (this.TaskId > 0)
                    {
                        var task = DomainApiClient<Dto.Task>.GetById(this.TaskId);
                        campaignId = task.AttestationCampaignId.Value;
                    }
                    parameter.Add("campaignId", campaignId.ToString());
                    parameter.Add("currentUser", CurrentUserName);
                    parameter.Add("comment", CommentsTextBox.Text);

                    DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
                                (DomainApiAction.Tasks.Task.UpdateAttestationComment, parameter);
                    CommentsTextBox.Text = "";
                }
            }
        }

        Dto.Task getCurrentTask(long taskId)
        {
            return DomainApiClient<Dto.Task>.GetById(this.TaskId);
        }

        public List<Dto.AttestationCampaignAttachment> GetUploadedFiles()
        {
            List<Dto.AttestationCampaignAttachment> attachments = new List<Dto.AttestationCampaignAttachment>();
            Dto.Task task = getCurrentTask(this.TaskId);
            if (task != null || CampaignId > 0)
            {

                if ((task != null && task.AttestationCampaignId.HasValue) || CampaignId > 0)
                {
                    long campaignId = 0;
                    if (task != null && task.AttestationCampaignId.HasValue)
                    {
                        campaignId = task.AttestationCampaignId.Value;
                    }
                    else
                    {
                        campaignId = CampaignId;
                    }
                    foreach (Telerik.Web.UI.UploadedFile file in AsyncUploadAttach.UploadedFiles)
                    {
                        Dto.AttestationCampaignAttachment na = new Dto.AttestationCampaignAttachment();
                        na.AttestationCampaignId = campaignId; // set campaign id
                        na.ContentType = file.ContentType;
                        na.FileName = file.FileName;
                        byte[] fileData = new byte[file.ContentLength];
                        file.InputStream.Read(fileData, 0, Convert.ToInt32(file.ContentLength));
                        na.AttachmentFile = fileData;
                        na.CreatedBy = CurrentUserName;
                        attachments.Add(na);
                    }
                }
                return attachments;
            }
            return null;
        }

        protected void requestList_NeedDataSource(object sender, RadListViewNeedDataSourceEventArgs e)
        {
            this.Initialize();
        }

        public void Initialize()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("attestationCampaignId", Convert.ToString(CampaignId));
            List<Dto.AttestationCampaignAttachment> requestAttachments = DomainApiClient<Dto.AttestationCampaignAttachment>.GetByAction<List<Dto.AttestationCampaignAttachment>>(DomainApiAction.Attestation.GetByAttestationCampaignId, parameter);
            requestList.DataSource = requestAttachments;
            lblAttachmentCount.Text = requestAttachments.Count.ToString();
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(requestList);
        }

        public void BindComments()
        {
            if (CampaignId == 0) return ;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("campaignId", Convert.ToString(CampaignId));
            Dto.AttestationCampaign requestComments = DomainApiClient<Dto.AttestationCampaign>.GetByAction<Dto.AttestationCampaign>("GetByCampaignId", parameter);
            List<PresentationAttestationCampaignComments> comments = new List<PresentationAttestationCampaignComments>();
            if (requestComments != null && requestComments.Comments.Length > 0)
            {
                var campaignComments = requestComments.Comments.Split(new string[] { "!~" }, StringSplitOptions.RemoveEmptyEntries);
                lblCommentsCount.Text = campaignComments.Length.ToString();
                foreach (var campaignComment in campaignComments)
                {
                    try
                    {
                        var commentsWithDate = campaignComment.Split(new string[] { "Update By:[" }, StringSplitOptions.RemoveEmptyEntries);
                        var finalComments = string.Empty;
                        string[] subComments = new string[] { };
                        if (commentsWithDate.Length > 1)
                        {
                            var tComments = commentsWithDate[1].Split(new string[] { "PM]", "AM]" }, StringSplitOptions.RemoveEmptyEntries);
                            if(tComments.Length > 1)
                            {
                                finalComments = tComments[1];
                            }
                            else
                            {
                                finalComments = tComments[0];
                            }
                        }
                        else
                        {
                            var tComments1 = commentsWithDate[0].Split(new string[] { "PM]", "AM]" }, StringSplitOptions.RemoveEmptyEntries);
                            if (tComments1.Length > 1)
                            {
                                finalComments = tComments1[1];
                            }
                            else
                            {
                                finalComments = tComments1[0];
                            }
                        }
                        //var cmt = commentsWithDate[1].Split(new string[] { "PM]", "AM]" },StringSplitOptions.RemoveEmptyEntries)[1];
                        var date = ""; var by = "";
                        if (commentsWithDate.Length > 1)
                        {
                            var remaining = commentsWithDate[1].Split(']');
                            by = remaining[0];
                            date = remaining[1].Split('[')[1];
                        }
                        comments.Add(new PresentationAttestationCampaignComments
                        {
                            Comments = finalComments,
                            CreatedOn = date,
                            CreatedBy = by,
                        });
                    }
                    catch (System.Exception ex)
                    {

                    }
                }
            }
            else
                lblCommentsCount.Text = "0";
            commentsList.DataSource = comments;
        }
        protected void commentsList_NeedDataSource(object sender, RadListViewNeedDataSourceEventArgs e)
        {
            BindComments();
        }
    }
}