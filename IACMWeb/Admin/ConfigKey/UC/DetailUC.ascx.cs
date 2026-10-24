using System;
namespace DI.Chakra.Web.ConfigKey.UC
{
    public partial class DetailUC : UIBaseUC
    {
        #region Memebers
        public delegate void Add_SubmitButton(object sender, EventArgs e);
        public event Add_SubmitButton Add_SubmitButtonClicked;
        public delegate void Add_CancelButton(object sender, EventArgs e);
        public event Add_CancelButton Add_CancelButtonClicked;
        #endregion

        #region Properties
        public string ConfigKey
        {
            get { return txtConfigKey.Text; }
            set { txtConfigKey.Text = value; }
        }
        public string ConfigValue
        {
            get { return txtConfigValue.Text; }
            set { txtConfigValue.Text = value; }
        }
        public string Header
        {
            get { return H1.InnerText; }
            set { H1.InnerText = value; }
        }
        public bool DisableConfigKey
        {
           set { txtConfigKey.Enabled=value; }
        }
        public string UpdateButton
        {
            get { return btnSubmit.Text; }
            set { btnSubmit.Text = value; }
        }
        public string Description
        {
            get { return txtDESC.Text; }
            set { txtDESC.Text = value; }
        }
        #endregion

        #region Methods
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
        #endregion
    }
}