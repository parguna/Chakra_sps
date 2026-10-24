using System;

namespace DI.Chakra.Web.Risk.Score.UC
{
    public partial class DetailUC : UIBaseUC
    {
        #region Public Properties
        public string RiskScore
        {
            get { return txtRiskScore.Text; }
            set { txtRiskScore.Text = value; }
        }
        public string RiskScoreDescription
        {
            get { return txtRiskScoreDescription.Text; }
            set { txtRiskScoreDescription.Text = value; }
        }
        #endregion
        
        #region Page Event
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
            txtRiskScore.Text = string.Empty;
            txtRiskScoreDescription.Text = string.Empty;
        }
        #endregion
    }
}