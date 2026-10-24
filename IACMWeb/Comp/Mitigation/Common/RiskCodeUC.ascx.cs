using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Comp.Mitigation.Common
{
    public partial class RiskCodeUC : UIBaseUC
    {
        #region Class Member Variables
        SessionStatusImpl session = new SessionStatusImpl();
        private bool isEdit = false;
        private bool isView = false;
        SODMitigation mitigaiton;
        List<SODRiskCode> rskCode;
        private const string V_ID = "ENTITY_ID";
        private const string V_SELECTEDRISK = "SELECTEDRISK";
        private UIMode uiMode;
        #endregion

        #region Public Properties
        public List<SODRiskCode> SelectedRisks
        {
            get
            {
                if (ViewState[V_SELECTEDRISK] != null)
                {
                    return (List<SODRiskCode>)ViewState[V_SELECTEDRISK];
                }
                return new List<SODRiskCode>();
            }
            set
            {
                ViewState[V_SELECTEDRISK] = value;
            }
        }

        public long EntityId
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return -1;
                return (long)ViewState[V_ID];
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }

        public bool IsEdit
        {
            set { isEdit = value; }
            get { return isEdit; }
        }

        public bool IsView
        {
            set { isView = value; }
            get { return isView; }
        }

        public SODRiskCode riskCode
        {
            get { return CodeRisks.SelectedRiskCode; }
            set { CodeRisks.SelectedRiskCode = value; }
        }

        public SODMitigation Mitigation
        {
            get { return mitigaiton; }
            set { mitigaiton = value; }
        }

        public List<SODRiskCode> RiskCode
        {
            get { return rskCode; }
            set { rskCode = value; }
        }

        public UIMode UIMode
        {
            get { return uiMode; }
            set { uiMode = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to Fill the Risk codes
        /// </summary>
        /// <param name="SODMitigation"></param>
        public void GetFilledRiskCodes(ref SODMitigation SODMitigation)
        {
            SODMitigation.RiskCodes = new List<SODRiskCode>();
            SODMitigation.RiskCodes = SelectedRisks;
        }

        /// <summary>
        /// Method to show the screen in the View Mode
        /// </summary>
        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Method to bind the Selected Risk items in the risk grid
        /// </summary>
        private void BindGrid()
        {
            this.grdMititaionRisk.DataSource = this.SelectedRisks;
            this.grdMititaionRisk.DataBind();
        }
        #endregion

        #region Page and Control Events
        /// <summary>
        /// Method to load the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Add Risk Items in the Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnMitigationFunction_Click(object sender, EventArgs e)
        {
            this.RiskCode = (this.RiskCode == null) ? new List<SODRiskCode>() : this.RiskCode;
            this.RiskCode = SelectedRisks;
            if (this.RiskCode.Where(x => x.RiskCode.Equals(this.riskCode.RiskCode)).Count() == 0)
            {
                this.RiskCode.Add(new SODRiskCode
                {
                    RiskCode = this.riskCode.RiskCode,
                    RiskDescription = this.riskCode.RiskDescription,
                    Mitigations = this.riskCode.Mitigations,
                    Id = this.riskCode.Id,
                    RiskLevelId = this.riskCode.RiskLevelId,
                    RiskScoreId = this.riskCode.RiskScoreId,
                    sodrisklevel = this.riskCode.sodrisklevel,
                    sodriskscore = this.riskCode.sodriskscore,
                    Status = this.riskCode.Status,
                    user = this.riskCode.user,
                    UserId = this.riskCode.UserId
                })
                ;
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Error, "Selected Risk Code is already added");
                return;
            }
            SelectedRisks=this.RiskCode;
            this.CodeRisks.Clear();
            BindGrid();
        }

        /// <summary>
        /// Delete the Existing Risk Items from the Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdMititaionRisk_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = e.Item as GridDataItem;
                long key = (long)Convert.ChangeType(item.GetDataKeyValue("Id"), typeof(long));
                SelectedRisks.RemoveAll(x => x.Id == key);
                BindGrid();
            }
        }
        #endregion 

        #region Initialise Section
        /// <summary>
        /// Mehtod to Intialize the page 
        /// </summary>
        public void IntializePage()
        {
            if (uiMode == UIMode.Edit || uiMode == UIMode.View)
            {
                if (!IsPostBack)
                    this.RiskCode = Mitigation.RiskCodes;
                SelectedRisks = this.RiskCode;
                BindGrid();
            }
            else
            {
                this.RiskCode = SelectedRisks;
                BindGrid();
            }
        }
        #endregion
    }
}