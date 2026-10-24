using System;

namespace DI.Chakra.Web.Admin.AppText.UC
{
    public partial class DetailUC : UIBaseUC
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public string AppTextKey
        {
            get { return lblKeyVal.Text; }
            set { lblKeyVal.Text = value; }
        }

        public string AppTexValue
        {
            get { return txtTextValue.Text; }
            set { txtTextValue.Text = value; }
        }

        public void Clearvalue()
        {
            lblKeyVal.Text = string.Empty;
            txtTextValue.Text = string.Empty;
        }
    }
}