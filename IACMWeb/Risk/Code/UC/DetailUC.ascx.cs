using DI.Chakra.Dto;
using System;

namespace DI.Chakra.Web.Risk.Code.UC
{
    public partial class DetailUC : UIBaseUC,IContextObjectEnabled
    {
        #region Public Properties
        public string RiskCode
        {
            get { return txtRiskCode.Text; }
            set { txtRiskCode.Text = value; }
        }
        public string RiskCodeDescription
        {
            get { return txtRiskCodeDescription.Text; }
            set { txtRiskCodeDescription.Text = value; }
        }
        public SODRiskScore RiskCodeScore
        {
            get
            {
                return RiskScore.SelectedRiskScore;
            }
            set
            {
                RiskScore.SelectedRiskScore = value;
            }
        }
        public SODRiskLevel RiskCodeLevel
        {
            get
            {
                return Risksearch.SelectedRiskLevel;
            }
            set
            {
                Risksearch.SelectedRiskLevel = value;
            }
        }
        public User RiskCodeOwner
        {
            get
            {
                return UserSearch.SelectedUser;
            }
            set
            {
                UserSearch.Clear();
                UserSearch.SelectedUser = value;
            }
        }    
        public int RiskCodeStatus
        {
            get { return radActive.Checked == true ? 1 : 0; ; }
            set
            {
                if (value == 1)
                {
                    radActive.Checked = true;
                    radClosed.Checked = false;
                }
                else
                {
                    radActive.Checked = false;
                    radClosed.Checked = true;
                }
            }
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
            //if (!IsPostBack)
            //   // BindDDL();
        }

        /// <summary>
        /// Method to handle checkchange event for radActive
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void radActive_CheckedChanged(object sender, EventArgs e)
        {
            if (radActive.Checked) radClosed.Checked = false;
        }

        /// <summary>
        /// Method to handle checkchange event for radClosed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void radClosed_CheckedChanged(object sender, EventArgs e)
        {
            if (radClosed.Checked) radActive.Checked = false;
        }
        #endregion
  
        #region Helper     
        /// <summary>
        /// Method to clear control values
        /// </summary>
        public void Clearvalue()
        {
            txtRiskCode.Text = string.Empty;
            txtRiskCodeDescription.Text = string.Empty;
            Risksearch.Clear();
            UserSearch.Clear();
            RiskScore.Clear();
            radActive.Checked = true;
            radClosed.Checked = false;
        }
        #endregion
    }
}