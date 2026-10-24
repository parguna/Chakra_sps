using Application;
using DI.Chakra.Infrastructure;
using System;
using System.Web.UI;
namespace DI.Chakra.Web.Task.UC
{
    public partial class DetailUC : UIBaseUC
    {
        public delegate void Add_SubmitButton(object sender, EventArgs e);
        public event Add_SubmitButton Add_SubmitButtonClicked;
        SessionStatusImpl session = new SessionStatusImpl();
        public delegate void Add_CancelButton(object sender, EventArgs e);
        public event Add_CancelButton Add_CancelButtonClicked;

   
        protected void Page_Load(object sender, EventArgs e)
        {
            
            this.delegateTo.DataSourceMethod = "GetActiveAndNonTerminatedUsers";
            if (!Page.IsPostBack)
            {
                btnSubmit.Attributes.Add("onfocus", "activateButton()");
                Comment.Attributes.Add("onblur", "activateButton()");
            }
            else
            {
                UpdateUC.Update();
                UpdateUsers.Update();
            }
        }
        
        public long userid
        {
            get
            {
                return delegateTo.SelectedUser.Id;
            }
            set
            {
                userid = value;
            }
        }

        public Dto.User SelectedUser
        {
            get { return delegateTo.SelectedUser; }

        }

        public string Comments
        {
            get { return Comment.Text; }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (Add_CancelButtonClicked != null)
            {
                clearPopup();
                Add_CancelButtonClicked(sender, e);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Comment.Text))
            {
                Add_CancelButtonClicked(sender, e);
                lblDelegationMessage.Text = "Please enter a comment";
                return;
            }
            if (Add_SubmitButtonClicked != null)
            {
                if (delegateTo.SelectedUser == null)
                {
                    clearPopup();
                    //Add_CancelButtonClicked(sender, e);
                    lblDelegationMessage.Text = "Delegated To cannot be empty";
                    return;
                }
                if (delegateTo.SelectedUser.Id != session.User.Id)
                {
                    Add_SubmitButtonClicked(sender, e);
                    clearPopup();
                }
                else
                {
                    clearPopup();
                    //Add_CancelButtonClicked(sender, e);
                    lblDelegationMessage.Text = "Self Delegation Not Allowed";
                }
            }
        }

        public void clearPopup()
        {
            delegateTo.SelectedUser = null;
            Comment.Text = "";
        }
    }
}