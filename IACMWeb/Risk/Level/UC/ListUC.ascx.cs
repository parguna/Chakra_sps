using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web.UI;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Risk.Level.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        private SessionStatusImpl session = new SessionStatusImpl();
        private static IList<SODRiskLevel> SODRiskLevels;

        #region Page and Control Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                gvRiskLevel.DataBind();
            }
        }

        protected void gvRiskLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem gdi = gvRiskLevel.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)gdi.GetDataKeyValue("Id");
        }

        protected void gvRiskLevel_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            bindSODRiskLevel();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            modalPopup.VisibleOnPageLoad = false;
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (Save())
                {
                    bindSODRiskLevel();
                    gvRiskLevel.DataBind();
                    //MasterPage.ShowAlert(AlertType.Success, "Risk Level saved successfully");
                }
            }
            catch (Exception)
            {
                //MasterPage.ShowAlert(AlertType.Success, "Risk Level not saved successfully");
                throw;
            }
        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        protected void gvRiskLevel_Init(object sender, EventArgs e)
        {

            GridFilterMenu menu = gvRiskLevel.FilterMenu;
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
        #endregion

        #region Delegate
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                delete();
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                gvRiskLevel.ExportSettings.FileName = "RiskLevel";
                gvRiskLevel.Export(gvRiskLevel, e.ActionType);
            }

            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                gvRiskLevel.Export<IList<SODRiskLevel>>(SODRiskLevels, e.ActionType, "RiskLevel");
            }

            else if (e.ActionType == ACLAction.Add)
            {
                DetailUC.Clearvalue();
                H1.InnerText = "Add Risk Level";
                btnSubmit.Text = "Add";
                modalPopup.VisibleOnPageLoad = true;
            }
            else if (e.ActionType == ACLAction.Edit)
            {
                if (!ContextObjectId.HasValue)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please select an item");
                    return;
                }
                SetPage();
                H1.InnerText = "Edit Risk Level";
                btnSubmit.Text = "Update";
                modalPopup.VisibleOnPageLoad = true;
            }
        }
        #endregion

        #region Helper
        private void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select an item");
                return;
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("riskLevelId", (this.ContextObjectId).ToString());
            bool isUsed = DomainApiClient<SODRiskCode>.GetByAction<bool>(DomainApiAction.Risks.SODRiskCode.HasRiskLevel, parameter);
            if (isUsed)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Risk Level could not be deleted because it is being referenced in RiskCode.");
                return;
            }
            DomainApiClient<SODRiskLevel>.DeleteById((long)this.ContextObjectId);
            bindSODRiskLevel();
            gvRiskLevel.DataBind();
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "Risk Level Deleted Successfully");
        }

        private void SetPage()
        {
            var RiskLevel = DomainApiClient<SODRiskLevel>.GetById((long)this.ContextObjectId);
            if (null == RiskLevel)
                return;
            DetailUC.RiskLevelCode = RiskLevel.RiskLevelCode;
            DetailUC.RiskLevelDescription = RiskLevel.RiskLevelDescription;
        }

        private bool Save()
        {
            string RiskLevelCode = DetailUC.RiskLevelCode;
            string RiskLevelDescription = DetailUC.RiskLevelDescription;
            SODRiskLevel riskLevel = new SODRiskLevel();
            if (btnSubmit.Text == "Add")
            {
                if (!string.IsNullOrEmpty(RiskLevelCode))
                {
                    if (IsRiskLevelAlreadyExists(RiskLevelCode.Trim()))
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Risk Level Already Exists");
                        return false;
                    }
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Risk Level Code should not be empty");
                    return false;
                }
            }
            riskLevel.Id = this.ContextObjectId.HasValue ? (long)this.ContextObjectId.Value : 0;
            riskLevel.RiskLevelCode = RiskLevelCode;
            riskLevel.RiskLevelDescription = RiskLevelDescription.Trim();
            DomainApiClient<SODRiskLevel>.Save(riskLevel);
            modalPopup.VisibleOnPageLoad = false;
            this.ContextObjectId = null;
            return true;
        }

        /// <summary>
        /// Method to check if risk level already exists
        /// </summary>
        /// <param name="RiskLevel"></param>
        /// <returns></returns>
        private bool IsRiskLevelAlreadyExists(string RiskLevel)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("riskLevelCode", RiskLevel);
            return DomainApiClient<SODRiskLevel>.GetByAction<bool>(DomainApiAction.Risks.SODRiskLevel.DoesExists, parameter);
        }

        /// <summary>
        /// Method to bind data to grid gvRiskLevel
        /// </summary>
        protected void bindSODRiskLevel()
        {
            SODRiskLevels = DomainApiClient<SODRiskLevel>.GetAll();
            gvRiskLevel.DataSource = SODRiskLevels;
        }
        #endregion
    }
}