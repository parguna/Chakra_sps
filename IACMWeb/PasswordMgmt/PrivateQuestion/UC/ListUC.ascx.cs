using System;
using System.Collections.Generic;
using System.Web.UI;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using Telerik.Web.UI;
using Application;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;

namespace DI.Chakra.Web.PasswordMgmt.PrivateQuestion.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        #region Memebers
        private const string V_ISEDIT = "ISEDIT";
        private const string V_ENTITYID = "V_ENTITYID";
        private static int intNoOfQues;
        private SessionStatusImpl session = new SessionStatusImpl();
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
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("configKey", ConfigConstants.NOOFSECURITYQUESTIONS);
                parameter.Add("product", Product.PWDManagement.ToString());
                var config = DomainApiClient<Dto.ConfigurationMaster>.GetByAction<Dto.IConfigurationMaster>
                                (DomainApiAction.Configuration.ConfigurationMaster.GetByConfigKey, parameter);
                intNoOfQues = Convert.ToInt32(config.ConfigValue);
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        protected void gvQuestions_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            bindPrivateQns();
        }

        protected void gvQuestions_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = gvQuestions.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string Message = "Please select the";
                string ControlMessage = "";
                if (AddUC.Question == "-- Select Question --" || string.IsNullOrEmpty(AddUC.Question))
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Question" : ControlMessage + " , Question";
                }
                if (string.IsNullOrEmpty(AddUC.Answer))
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Answer" : ControlMessage + " , Answer";
                }
                if (ControlMessage != "")
                {
                    int LastIndex = ControlMessage.LastIndexOf(",");
                    Message += LastIndex > 0 ? ControlMessage.Substring(0, LastIndex) + " and" + ControlMessage.Substring(LastIndex + 1) : ControlMessage;
                    Message += ".";
                    ShowAlertMessage(AlertType.Error, Message);
                }
                else
                {
                    Save();
                    gvQuestions.Rebind();
                }
            }
            catch (Exception)
            {
                MasterPage.ShowAlert(AlertType.Error, "Question not saved successfully");
                throw;
            }
        }

        protected void btnEditCancel_Click(object sender, EventArgs e)
        {
            modalPopupEdit.VisibleOnPageLoad = false;
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            string Message = "Please select the";
            string ControlMessage = "";
            if (string.IsNullOrEmpty(DetailUC.Question))
            {
                ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Question" : ControlMessage + " , Question";
            }
            if (string.IsNullOrEmpty(DetailUC.Answer))
            {
                ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Answer" : ControlMessage + " , Answer";
            }
            if (ControlMessage != "")
            {
                int LastIndex = ControlMessage.LastIndexOf(",");
                Message += LastIndex > 0 ? ControlMessage.Substring(0, LastIndex) + " and" + ControlMessage.Substring(LastIndex + 1) : ControlMessage;
                Message += ".";
                ShowAlertMessage(AlertType.Error, Message);
            }
            else
            {
                Edit();
                gvQuestions.Rebind();
            }
        }

        protected void btnAddCancel_Click(object sender, EventArgs e)
        {
            modalPopupAdd.VisibleOnPageLoad = false;
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                delete();
            }
            else if (e.ActionType == ACLAction.Add)
            {
                List<GlobalAnswer> globalAnswerList = GetglobalAnswerList();
                if (globalAnswerList.Count >= intNoOfQues)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Reached Adding Maximum no of Security questions");
                    return;
                }
                AddUC.header = "Add Question";
                AddUC.Question = "";
                AddUC.Answer = "";
                gvQuestions.SelectedIndexes.Clear();
                modalPopupAdd.VisibleOnPageLoad = true;
            }
            else if (e.ActionType == ACLAction.Edit)
            {
                if (!this.ContextObjectId.HasValue)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please select an item");
                    return;
                }
                else
                {
                    SetPage();
                    DetailUC.header_edit = "Edit Question and Answer";
                    modalPopupEdit.VisibleOnPageLoad = true;
                }
            }
        }
        #endregion

        #region Method
        private List<GlobalAnswer> GetglobalAnswerList()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", session.UserId.ToString());
            return DomainApiClient<Dto.GlobalAnswer>.GetByAction<List<GlobalAnswer>>
                                                        (DomainApiAction.SecurityQA.GlobalAnswer.GetByUserId, parameter);
        }

        protected void bindPrivateQns()
        {
            gvQuestions.DataSource = GetglobalAnswerList();
        }

        private void delete()
        {
            if (this.ContextObjectId == null)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item");
                return;
            }
            DomainApiClient<Dto.GlobalAnswer>.DeleteById(this.ContextObjectId.Value);
            gvQuestions.Rebind();
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "Security Question and Answer Deleted Successfully");
        }

        private void SetPage()
        {
            var PrivateQuestion = DomainApiClient<Dto.GlobalAnswer>.GetById(this.ContextObjectId.Value);
            if (null == PrivateQuestion)
                return;
            DetailUC.Question = PrivateQuestion.Question;
            DetailUC.Answer = RSAHelper.DecryptData(PrivateQuestion.Answer);
        }

        private void Save()
        {
            try
            {
                string question = AddUC.Question.Trim();
                if (string.IsNullOrEmpty(question))
                {
                    MasterPage.ShowAlert(AlertType.Error, "Question should not be empty");
                    return;
                }
                if (string.IsNullOrEmpty(AddUC.Answer.Trim()))
                {
                    MasterPage.ShowAlert(AlertType.Error, "Answer should not be empty");
                    return;
                }
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", session.UserId.ToString());
                parameter.Add("question", question);
                bool HasQuestion = DomainApiClient<Dto.GlobalAnswer>.GetByAction<bool>
                                                        (DomainApiAction.SecurityQA.GlobalAnswer.HasAnyQuestionByUserId, parameter);
                if (HasQuestion)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Question Already Exists");
                    return;
                }
                UpdateQuestion(AddUC.Question, AddUC.Answer);
                modalPopupAdd.VisibleOnPageLoad = false;
                MasterPage.ShowAlert(AlertType.Success, "Security Question and Answer Added Successfully");
            }
            catch
            {
                MasterPage.ShowAlert(AlertType.Error, "Security Question and Answer Not Added Successfully");
            }
        }

        private void Edit()
        {
            try
            {
                UpdateQuestion(DetailUC.Question, DetailUC.Answer);
                modalPopupEdit.VisibleOnPageLoad = false;
                this.ContextObjectId = null;
                MasterPage.ShowAlert(AlertType.Success, "Security Question and Answer updated Successfully");
            }
            catch
            {
                MasterPage.ShowAlert(AlertType.Error, "Security Question and Answer Not updated Successfully");
            }
        }

        private void UpdateQuestion(string strIndiv_Qn, string strAnswer)
        {
            GlobalAnswer globalAnswer = new GlobalAnswer();
            globalAnswer.Id = this.ContextObjectId.HasValue ? (int)this.ContextObjectId.Value : 0;
            globalAnswer.Question = strIndiv_Qn.Trim();
            globalAnswer.Answer = strAnswer.Trim();
            globalAnswer.Answer = RSAHelper.EncryptData(globalAnswer.Answer);
            globalAnswer.UserId = session.User.Id;
            DomainApiClient<Dto.GlobalAnswer>.Save(globalAnswer);
        }
        #endregion
    }
}