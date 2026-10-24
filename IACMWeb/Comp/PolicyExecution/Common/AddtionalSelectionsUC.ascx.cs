using DI.Chakra.Infrastructure;
using System;
using System.Web.UI;

namespace DI.Chakra.Web.Comp.PolicyExecution.Common
{
    public partial class AddtionalSelectionsUC : DI.Chakra.Web.UIBaseUC
    {
        #region Public Methods
        /// <summary>
        /// Method to Initialise Page
        /// </summary>
        public void IntializePage()
        {
        }

        /// <summary>
        /// Method to set the display mode
        /// </summary>
        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);
        }
        #endregion
        
        #region Page and Control Events
        /// <summary>
        /// Method to load page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                GroupSection.Visible = false;
                RiskSection.Visible = false;
                UserSection.Visible = true;
                rblUsers.Checked = true;
            }
        }

        /// <summary>
        /// Method to handle checked event of radio button rblUsers 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rblUsers_CheckedChanged(object sender, EventArgs e)
        {
            GroupSection.Visible = false;
            RiskSection.Visible = false;
            UserSection.Visible = true;
            Clear();
        }

        /// <summary>
        /// Method to handle checked event of radio button rblGroups 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rblGroups_CheckedChanged(object sender, EventArgs e)
        {
            GroupSection.Visible = true;
            RiskSection.Visible = false;
            UserSection.Visible = false;
            Clear();
        }

        /// <summary>
        /// Method to handle checked event of radio button rblRisks 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rblRisks_CheckedChanged(object sender, EventArgs e)
        {
            GroupSection.Visible = false;
            RiskSection.Visible = true;
            UserSection.Visible = false;
            Clear();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Method to clear the controls
        /// </summary>
        private void Clear()
        {
            RiskLevelSearch.Clear();
            GroupSearch.Clear();
            UserSearch.Clear();
        }
        #endregion
    }
}