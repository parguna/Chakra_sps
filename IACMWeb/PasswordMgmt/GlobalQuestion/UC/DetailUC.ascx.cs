using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.PasswordMgmt.Global_Question.UC
{
    public partial class DetailUC : UIBaseUC
    {
        private const string V_ISEDIT = "ISEDIT";
        private const string V_VIEWONLY = "VIEWONLY";
        public delegate void Add_SubmitButton(object sender, EventArgs e);
        public event Add_SubmitButton Add_SubmitButtonClicked;
        public delegate void Add_CancelButton(object sender, EventArgs e);
        public event Add_CancelButton Add_CancelButtonClicked;

        #region Page Events
        protected void Page_Load(object sender, EventArgs e)
        {
           // this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                if (ViewOnly || IsEdit)
                {
                    SetPage();
                }
                //if (ViewOnly)
                //    DisplayMode();
            }
        }
        #endregion

        #region Properties
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
            get { return txtQuestion.Text; }
            set { txtQuestion.Text = value; }
        }
        #endregion

        #region Delegate
        //public void ActionClicked(object sender, AccessActionEventArgs e)
        //{
        //    if (e.ActionType == ACLAction.Save)
        //    {
        //        Save();
        //    }
        //}
        #endregion

        #region Helpers
        private void SetPage()
        {
            var globalQuestion = DomainApiClient<Dto.GlobalQuestion>.GetById(this.ContextObjectId.Value);

            if (null == globalQuestion)
                return;
            txtQuestion.Text = globalQuestion.Question;
        }
        //private void Save()
        //{
        //    if ((!IsEdit) && IsGlobalQuestionAlreadyExists(txtQuestion.Text.Trim()))
        //    {
        //        MasterPage.ShowAlert(AlertType.Warning, "Global Question Already Exists");
        //        return;
        //    }
        //    DI.Chakra.Model.GlobalQuestion _GlobalQuestion = new DI.Chakra.Model.GlobalQuestion();
        //    _GlobalQuestion.GlobalQuestionId = this.ContextObjectId.HasValue ? this.ContextObjectId.Value : 0;
        //    _GlobalQuestion.Question = txtQuestion.Text.Trim();
        //    _GlobalQuestion.CreatedDate = DateTime.Now;
        //    _GlobalQuestion.LastModifiedDate = DateTime.Now;
        //    pmFacade.SaveQuestion(_GlobalQuestion);

        //    Response.Redirect("~/PasswordMgmt/GlobalQuestion/List.aspx");

        //}
        //private void DisplayMode()
        //{
        //    contentWrapper.Disabled = true;
        //}
        private bool IsGlobalQuestionAlreadyExists(string Question)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("question", Question);            
            return DomainApiClient<Dto.GlobalQuestion>.GetByAction<bool>(DomainApiAction.SecurityQA.GlobalQuestion.DoesExists, parameter);
        }
        #endregion
      
        public void Clearvalue()
        {
            txtQuestion.Text = string.Empty;
        }

        public void SetAddPopupTexts()
        {
            H1.InnerText = "Add Question";
            btnSubmit.Text = "Add";
            txtQuestion.Text = string.Empty;
        }

        public void SetEditPopupTexts()
        {
            H1.InnerText = "Edit Question";
            btnSubmit.Text = "Update";
            txtQuestion.Text = string.Empty;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (Add_CancelButtonClicked != null)
            {
                Add_CancelButtonClicked(sender, e);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Add_SubmitButtonClicked != null)
            {
                Add_SubmitButtonClicked(sender, e);
            }
        }
    }
}