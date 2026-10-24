using System;
using System.Web.UI;
using Application;
using DI.Chakra.Dto;

namespace DI.Chakra.Web.PasswordMgmt.PrivateQuestion.UC
{
    public partial class DetailUC : UIBaseUC
    {
        private const string V_ENTITYID = "V_ENTITYID";
        private const string V_ISEDIT = "ISEDIT";
        private SessionStatusImpl session = new SessionStatusImpl();
        private const string V_VIEWONLY = "VIEWONLY";
        public delegate void CancelButton(object sender, EventArgs e);
        public event CancelButton CancelButtonClicked;
        public delegate void EditButton(object sender, EventArgs e);
        public event EditButton EditButtonClicked;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (ViewOnly || IsEdit)
                {
                    this.EntityId = (long)this.ContextObjectId.Value;
                    SetPage();
                }
            }
        }

        #region Properties
        public long EntityId
        {
            get
            {
                if (ViewState[V_ENTITYID] == null)
                    return 0;
                return (long)ViewState[V_ENTITYID];
            }
            set
            {
                ViewState[V_ENTITYID] = value;
            }
        }

        public bool IsEdit
        {
            get
            {
                if (ViewState[V_ISEDIT] == null)
                    return false;
                return (bool)ViewState[V_ISEDIT];
            }
            set
            {
                ViewState[V_ISEDIT] = value;
            }
        }

        public bool ViewOnly
        {
            get
            {
                if (ViewState[V_VIEWONLY] == null)
                    return false;
                return (bool)ViewState[V_VIEWONLY];
            }
            set
            {
                ViewState[V_VIEWONLY] = value;
            }
        }

        public string Question
        {
            get
            { return txtQuestion.Text; }
            set { txtQuestion.Text = value; }
        }

        public string Answer
        {
            get
            {
                return txtAnswer.Text;
            }
            set
            {
                txtAnswer.Text = value;
            }
        }

        public string header_edit
        {
            get
            {
                return hdrEdit.InnerText;
            }
            set
            {
                hdrEdit.InnerText = value;
            }
        }
        #endregion

        #region Helpers
        private void SetPage()
        {
            GlobalAnswer QuestionAns = DomainApiClient<Dto.GlobalAnswer>.GetById(this.EntityId);
            if (null == QuestionAns)
                return;
            txtQuestion.Text = QuestionAns.Question;
            txtAnswer.Text = QuestionAns.Answer;
        }

        public void ClearValues()
        {
            txtQuestion.Text = string.Empty;
            txtAnswer.Text = string.Empty;
        }
        #endregion

        protected void btnEditCancel_Click(object sender, EventArgs e)
        {
            if (CancelButtonClicked != null)
            {
                CancelButtonClicked(sender, e);
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            if (EditButtonClicked != null)
            {
                EditButtonClicked(sender, e);
            }
        }
    }
}
