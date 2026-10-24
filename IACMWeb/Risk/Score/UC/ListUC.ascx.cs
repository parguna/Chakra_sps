using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web.UI;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Risk.Score.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        private SessionStatusImpl session = new SessionStatusImpl();

        #region Page and Control Events
        /// <summary>
        /// Method to load page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                gvRiskScore.DataBind();
            }
        }

        /// <summary>
        /// Method to handle selected index change for gvRiskScore 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvRiskScore_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem gdi = gvRiskScore.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)gdi.GetDataKeyValue("Id");
        }

        /// <summary>
        /// Method to handle click event for btn_Cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        /// <summary>
        /// Method to initialise grid gvRiskScore
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvRiskScore_Init(object sender, EventArgs e)
        {
            GridFilterMenu menu = gvRiskScore.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" || menu.Items[i].Text == "Contains" || menu.Items[i].Text == "EqualTo" ||
                    menu.Items[i].Text == "GreaterThan" || menu.Items[i].Text == "LessThan" || menu.Items[i].Text == "StartsWith" || 
                    menu.Items[i].Text == "EndsWith")
                {
                    i++;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Method to handle click event for btnCancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            modalPopup.VisibleOnPageLoad = false;
        }

        /// <summary>
        /// Method to handle click event for btnSubmit 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (Save())
                {
                    bindSODRiskScore();
                    gvRiskScore.DataBind();
                    //MasterPage.ShowAlert(AlertType.Success, "Risk Score saved successfully");
                }
            }
            catch (Exception)
            {
                //MasterPage.ShowAlert(AlertType.Success, "Risk Score not saved successfully");
                throw;
            }
        }

        /// <summary>
        /// Method to provide data source for grid gvRiskScore
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvRiskScore_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            bindSODRiskScore();
        }
        #endregion

        #region Delegate
        /// <summary>
        /// Method to handle any click event in the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                delete();
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                gvRiskScore.ExportSettings.FileName = "RiskScore";
                gvRiskScore.Export(gvRiskScore, e.ActionType);
            }
            else if (e.ActionType == ACLAction.Add)
            {
                DetailUC.Clearvalue();
                H1.InnerText = "Add Risk Score";
                btnSubmit.Text = "Add";
                modalPopup.VisibleOnPageLoad = true;
            }
            else if (e.ActionType == ACLAction.Edit)
            {
                if (!ContextObjectId.HasValue || ContextObjectId == 0)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please select an item");
                    return;
                }
                SetPage();
                H1.InnerText = "Edit Risk Score";
                btnSubmit.Text = "Update";
                modalPopup.VisibleOnPageLoad = true;
            }
        }
        #endregion

        #region Helper
        /// <summary>
        /// Method to delete risk score
        /// </summary>
        private void delete()
        {
            if (!ContextObjectId.HasValue || ContextObjectId == 0)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select an item");
                return;
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("riskScoreId", (this.ContextObjectId).ToString());
            bool isUsed = DomainApiClient<SODRiskCode>.GetByAction<bool>(DomainApiAction.Risks.SODRiskCode.HasRiskScore, parameter);
            if (isUsed)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Risk Score could not be deleted because it is being referenced in RiskCode.");
                return;
            }
            DomainApiClient<SODRiskScore>.DeleteById((long)this.ContextObjectId);
            bindSODRiskScore();
            gvRiskScore.DataBind();
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "Risk Score Deleted Successfully");
        }

        /// <summary>
        /// Method to set page
        /// </summary>
        private void SetPage()
        {
            var RiskScore = DomainApiClient<SODRiskScore>.GetById((long)this.ContextObjectId);
            if (null == RiskScore)
                return;
            DetailUC.RiskScore = Convert.ToString(RiskScore.RiskScore);
            DetailUC.RiskScoreDescription = RiskScore.RiskScoreDescription;
        }

        /// <summary>
        /// Method to save risk score
        /// </summary>
        /// <returns></returns>
        private bool Save()
        {
            if (string.IsNullOrEmpty(DetailUC.RiskScore))
            {
                MasterPage.ShowAlert(AlertType.Warning, "Risk Score should not be empty");
                return false;
            }
            int RiskScore ;
            bool isNumeric = int.TryParse(DetailUC.RiskScore, out RiskScore);
            //if (!isNumeric)
            //{
            //    MasterPage.ShowAlert(AlertType.Warning, "Score should be a number");
            //    return false;
            //}
            string RiskScoreDescription = DetailUC.RiskScoreDescription;
            SODRiskScore riskScore = new SODRiskScore();     
            if (btnSubmit.Text == "Add")
            {
                if (IsRiskScoreAlreadyExists(RiskScore))
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Risk Score Already Exists");
                    return false;
                }
            }
            riskScore.Id = this.ContextObjectId.HasValue ? (long)this.ContextObjectId.Value : 0;
            riskScore.RiskScore = Convert.ToInt32(RiskScore);
            riskScore.RiskScoreDescription = RiskScoreDescription.Trim();
            DomainApiClient<SODRiskScore>.Save(riskScore);
            modalPopup.VisibleOnPageLoad = false;
            this.ContextObjectId = null;
            return true;
        }

        /// <summary>
        /// Method to check if risk score already exists
        /// </summary>
        /// <param name="RiskScore"></param>
        /// <returns></returns>
        private bool IsRiskScoreAlreadyExists(long RiskScore)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("RiskScore", RiskScore.ToString());
            return DomainApiClient<SODRiskScore>.GetByAction<bool>(DomainApiAction.Risks.SODRiskScore.DoesExists, parameter);
        }

        /// <summary>
        /// Method to bind data to grid gvRiskScore
        /// </summary>
        protected void bindSODRiskScore()
        {
            gvRiskScore.DataSource = DomainApiClient<SODRiskScore>.GetAll();
        }
        #endregion
    }
}