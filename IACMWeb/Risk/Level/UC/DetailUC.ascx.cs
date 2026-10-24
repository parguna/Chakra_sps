using System;

namespace DI.Chakra.Web.Risk.Level.UC
{
    public partial class DetailUC : UIBaseUC
    {
        #region Public Properties
        public string RiskLevelCode
        {
            get { return txtRiskLevelCode.Text; }
            set { txtRiskLevelCode.Text = value; }
        }
        public string RiskLevelDescription
        {
            get { return txtRiskDescription.Text; }
            set { txtRiskDescription.Text = value; }
        }
        #endregion

        #region Page Events
        /// <summary>
        /// Method to load page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        #endregion
        
        #region Helper
        /// <summary>
        /// Method to clear control values
        /// </summary>
        public void Clearvalue()
        {
            txtRiskLevelCode.Text = string.Empty;
            txtRiskDescription.Text = string.Empty;
        }
        #endregion
    }
}