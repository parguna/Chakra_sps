using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.PasswordMgmt.PrivateQuestion.UC
{
    public partial class AddUC : UIBaseUC
    {
        public delegate void Add_SubmitButton(object sender, EventArgs e);
        public event Add_SubmitButton Add_SubmitButtonClicked;
        public delegate void Add_CancelButton(object sender, EventArgs e);
        public event Add_CancelButton Add_CancelButtonClicked;

        public string Question
        {
            get
            {
                string strQuestion = string.Empty;
                if (rbPredefinedQues.Checked)
                {
                    if (ddlPredefinedQuestion.SelectedIndex != -1)
                        strQuestion = ddlPredefinedQuestion.SelectedItem.Text;
                }
                else if (rbCustomQues.Checked)
                    strQuestion = txtCustQues.Text;
                return strQuestion;
            }
            set 
            {
                if (rbPredefinedQues.Checked)
                {
                    ddlPredefinedQuestion.SelectedIndex = -1;
                }
                else if (rbCustomQues.Checked)
                    txtCustQues.Text = value;
            }
        }

        public string Answer
        {
            get { return txtAnswer.Text; }
            set { txtAnswer.Text = value; }
        }

        public string header
        {
            get { return hdrAdd.InnerText; }
            set { hdrAdd.InnerText = value; }
        }
        #region events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindGlobalQuestions();
            }
        }
        #endregion

        #region methods
        private void BindGlobalQuestions()
        {
            ddlPredefinedQuestion.DataSource = DomainApiClient<Dto.GlobalQuestion>.GetAll();
            ddlPredefinedQuestion.DataTextField = "Question";
            ddlPredefinedQuestion.DataValueField = "Id";
            ddlPredefinedQuestion.DataBind();
            ListItem item = new ListItem("-- Select Question --", "-1");
            ddlPredefinedQuestion.Items.Insert(0, item);
        }

        protected void rbQues_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCustomQues.Checked == true)
            {
                ddlPredefinedQuestion.Visible = false;
                txtCustQues.Visible = true;
            }
            else
            {
                ddlPredefinedQuestion.Visible = true;
                txtCustQues.Visible = false;
            }
        }
        #endregion

        protected void btnAddCancel_Click(object sender, EventArgs e)
        {
            if (Add_CancelButtonClicked != null)
            {
                Add_CancelButtonClicked(sender, e);
            }
       }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Add_SubmitButtonClicked != null)
            {
                Add_SubmitButtonClicked(sender, e);
            }
        }
    }
}