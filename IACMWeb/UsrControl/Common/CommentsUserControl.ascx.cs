using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class CommentsUserControl : DI.Chakra.Web.UIBaseUC
    {
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
            set { ViewState["TaskId"] = value; }
        }

        public long KeyId
        {
            get
            {
                if (ViewState["KeyId"] != null)
                {
                    return Convert.ToInt64(ViewState["KeyId"]);
                }
                return 0;
            }
            set { ViewState["KeyId"] = value; }
        }

        public bool IsEditable
        {
            get
            {
                if (ViewState["DisableSaveButton"] != null)
                {
                    return Convert.ToBoolean(ViewState["DisableSaveButton"]);
                }
                return false;
            }
            set
            {
                ViewState["DisableSaveButton"] = value;
                SaveButton.Enabled = value;
                CommentsTextBox.Enabled = value;
                SaveButton.Attributes.Add("onfocus", "validateComments()");
                CommentsTextBox.Attributes.Add("onblur", "validateComments()");
                hdnIsEditable.Value = value ? "1" : "0";
            }
        }

        public string Category
        {
            get
            {
                if (ViewState["Category"] != null)
                {
                    return Convert.ToString(ViewState["Category"]);
                }
                return string.Empty;
            }
            set
            {
                ViewState["Category"] = value;
            }
        }
        public string ReviewComments
        {
            get
            {
                if (ViewState["ReviewComments"] != null)
                {
                    return Convert.ToString(ViewState["ReviewComments"]);
                }
                return string.Empty;
            }
            set
            {
                ViewState["ReviewComments"] = value;
                BindExistingComments();
            }
        }
        public string Comments
        {
            get { return this.CommentsTextBox.Text.Trim(); }
            set { this.CommentsTextBox.Text = value; }
        }

        public event EventHandler OnCommentsSave;
        public event EventHandler OnCommentsCancel;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (OnCommentsSave != null)

                this.OnCommentsSave(sender, e);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            OnCommentsCancel(sender, e);
        }

        #region "Private Methods"
        

        private void BindExistingComments()
        {
            IList comments = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("snapshotActionId", this.KeyId.ToString());
            parameter.Add("taskId", this.TaskId.ToString());

            comments = DomainApiClient<Dto.AttestationSnapshotActionHistory>.GetByAction<List<AttestationSnapshotActionHistory>>(DomainApiAction.Attestation.AttestationSnapshotActionHistory.GetWithValidComments, parameter);

            if (comments != null)
            {
                if (comments != null && comments.Count <= 0)
                {
                    divComments.Style["Height"] = "0px";
                }
                else if(comments.Count >= 1 && comments.Count < 3)
                {
                    divComments.Style["Height"] = "50px";
                }
                else if (comments.Count >= 3)
                {
                    divComments.Style["Height"] = "100px";
                }
            }

            rptComments.DataSource = comments;
            rptComments.DataBind();           
        }
        #endregion

    }
}