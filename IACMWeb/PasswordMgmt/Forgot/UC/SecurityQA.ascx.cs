
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Application;
using System.Collections;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Web.PasswordMgmt.Forgot.UC
{
    public partial class SecurityQA : UIBaseUC
    {
        #region ClassVariables
        private const string V_ENTITYID = "V_ENTITYID"; private const string V_ID = "ID";
        SessionStatusImpl session = new SessionStatusImpl();
        private List<PasswordResetQuestion> questions = new List<PasswordResetQuestion>();

        #endregion

        #region Properties

        public int EntityId
        {
            get
            {
                if (ViewState[V_ENTITYID] == null)
                    return -1;
                return (int)ViewState[V_ENTITYID];
            }
            set
            {
                ViewState[V_ENTITYID] = value;
                SetControl();
            }
        }

        #endregion

        #region Helpers
        /// <summary>
        /// Method responsible for Populate all security questions.
        /// </summary>
        private void SetControl()
        {
            LoadUserSecurityQuestions(); // It will load all Global and Individula Questions in Repeater control.

            foreach (RepeaterItem item in loadQuestion.Items)
            {
                TextBox txtQn = (TextBox)item.FindControl("txtQuestions");
                HiddenField QnId = (HiddenField)item.FindControl("Id");
                txtQn.Text = questions[item.ItemIndex].Question;
                QnId.Value = questions[item.ItemIndex].Id;
            }
        }

        /// <summary>
        /// Method Responsible for assigning questions datasource to repeater control.
        /// </summary>
        private void LoadUserSecurityQuestions()
        {
            // IList<DomainModel.GlobalAnswer> globalAnswers = DomainModel.GlobalAnswer.GetByUserId(session.User.Id);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", session.User.Id.ToString());
            List<Dto.GlobalAnswer> globalAnswers = DomainApiClient<Dto.GlobalAnswer>.GetByAction<List<Dto.GlobalAnswer>>(DomainApiAction.SecurityQA.GlobalAnswer.GetByUser, parameter);
            foreach (Dto.GlobalAnswer answer in globalAnswers)
            {
                PasswordResetQuestion resetQuestion = new PasswordResetQuestion();
                resetQuestion.Question = answer.Question;
                resetQuestion.Id = answer.Id.ToString();
                questions.Add(resetQuestion);
            }
            loadQuestion.DataSource = questions; // Collection contains both Individual and Global Questions.
            loadQuestion.DataBind();
        }
        #endregion

        #region Events
        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            foreach (RepeaterItem item in loadQuestion.Items)
            {
                var CurrentPanel = (Panel)item.FindControl("group");
                if (CurrentPanel != null && CurrentPanel.Visible)
                {
                    TextBox txtAnswer = (TextBox)item.FindControl("txtAnswer");
                    HiddenField QnId = (HiddenField)item.FindControl("Id");
                    string EnAnswer = string.Empty;
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("userId", Convert.ToInt64(QnId.Value).ToString());
                    EnAnswer = DomainApiClient<Dto.GlobalAnswer>.GetByAction<Dto.GlobalAnswer>(DomainApiAction.SecurityQA.GlobalAnswer.GetsecurityquestionById, parameter).Answer;
                    string DBAnswer = RSAHelper.DecryptData(EnAnswer);
                    if (txtAnswer.Text.Equals(DBAnswer)) // Checking User Entered Security Answer is same as answer stored in db.
                    {
                        CurrentPanel.Visible = false;
                        if ((item.ItemIndex + 1) < loadQuestion.Items.Count)
                        {
                            var NextPanel = (Panel)loadQuestion.Items[item.ItemIndex + 1].FindControl("group");
                            NextPanel.Visible = true;
                            continue;
                        }
                        else
                        {
                            Server.Transfer("Resetpassword.aspx");
                        }
                    }
                    else
                    {
                        if (!(string.IsNullOrEmpty(txtAnswer.Text)))
                        {
                            AlertMessage.AlertType = AlertType.Error;
                            AlertMessage.Message = "Answer is incorrect";
                            AlertMessage.Show();
                        }
                        break;
                    }
                }

            }
        }

        #endregion

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Login");
        }
    }


    public class PasswordResetQuestion
    {
        public string Question { get; set; }
        public string Id { get; set; }
    }
}