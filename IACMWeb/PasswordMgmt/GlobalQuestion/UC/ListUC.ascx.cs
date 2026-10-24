using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web.UI;
using Telerik.Web.UI;
using Application;
using DI.Chakra.Logger;

namespace DI.Chakra.Web.PasswordMgmt.Global_Question.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        private const string V_ISEDIT = "ISEDIT";
        private const string V_VIEWONLY = "VIEWONLY";
        SessionStatusImpl sessionStatus;

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

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            sessionStatus = new SessionStatusImpl();

            if (!Page.IsPostBack)
            {
                gvQuestions.DataBind();
            }
        }

        protected void gvQuestions_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            bindGlobalQns();
        }

        protected void bindGlobalQns()
        {
            IList<GlobalQuestion> globalQuestionList = DomainApiClient<GlobalQuestion>.GetAll();
            gvQuestions.DataSource = globalQuestionList;
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Password Management/Global Questions - Delete");
                delete();
            }
            else if (e.ActionType == ACLAction.Add)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Password Management/Global Questions - Add");
                DetailUC.Clearvalue();
                gvQuestions.SelectedIndexes.Clear();
                DetailUC.SetAddPopupTexts();
                modalPopup.VisibleOnPageLoad = true;
            }
            else if (e.ActionType == ACLAction.Edit)
            {
                if (!ContextObjectId.HasValue || ContextObjectId == 0)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please select an item");
                    return;
                }
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Password/Global Questions - Edit");
                DetailUC.SetEditPopupTexts();
                SetPage();
                modalPopup.VisibleOnPageLoad = true;
            }
        }

        private void delete()
        {
            if (!ContextObjectId.HasValue || ContextObjectId == 0)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item");
                return;
            }
            bool isExists = DomainApiClient<Dto.GlobalQuestion>.GetById((long)this.ContextObjectId) != null ? true : false;
            if (isExists)
            {
                DomainApiClient<Dto.GlobalQuestion>.DeleteById(this.ContextObjectId.Value);
                this.ContextObjectId = null;
                MasterPage.ShowAlert(AlertType.Success, "Global Question Deleted Successfully");
            }
            bindGlobalQns();
            gvQuestions.DataBind();
        }

        private void SetPage()
        {
            var globalquestion = DomainApiClient<Dto.GlobalQuestion>.GetById(this.ContextObjectId.Value);

            if (null == globalquestion)
                return;
            DetailUC.Question = globalquestion.Question;
        }

        private bool Save()
        {
            string question = DetailUC.Question.Trim();
            if (!string.IsNullOrEmpty(question))
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("question", question);

                if ((!IsEdit) && DomainApiClient<Dto.GlobalQuestion>.GetByAction<bool>
                    (DomainApiAction.SecurityQA.GlobalQuestion.DoesExists, parameter))
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Global Question Already Exists");
                    return false;
                }
                GlobalQuestion globalQuestion = new GlobalQuestion();
                globalQuestion.Id = this.ContextObjectId.HasValue ? (long)this.ContextObjectId.Value : 0;
                globalQuestion.Question = question;

                DomainApiClient<Dto.GlobalQuestion>.Save(globalQuestion);
                this.ContextObjectId = null;
                modalPopup.VisibleOnPageLoad = false;
                return true;
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Error, "Global Question should not be empty");
                return false;
            }
        }

        protected void gvQuestions_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = gvQuestions.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (Save())
                {
                    bindGlobalQns();
                    gvQuestions.DataBind();
                    this.ContextObjectId = null;
                    modalPopup.VisibleOnPageLoad = false;
                    MasterPage.ShowAlert(AlertType.Success, "Global Question saved successfully");
                }
            }
            catch (Exception)
            {
                this.ContextObjectId = null;
                modalPopup.VisibleOnPageLoad = false;
                MasterPage.ShowAlert(AlertType.Success, "Global Question not saved successfully");
                throw;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            modalPopup.VisibleOnPageLoad = false;
        }     
    }
}