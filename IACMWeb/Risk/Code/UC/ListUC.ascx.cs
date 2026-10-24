using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web.UI;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Risk.Code.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        private SessionStatusImpl session = new SessionStatusImpl();
        private static IList<SODRiskCode> SODRiskCodes;
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
                gvRiskCode.DataBind();
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
                    bindSODRiskCode();
                    gvRiskCode.DataBind();
                    //MasterPage.ShowAlert(AlertType.Success, "Risk Code saved successfully");
                }
            }
            catch (Exception)
            {
                //MasterPage.ShowAlert(AlertType.Success, "Risk Code not saved successfully");
                throw;
            }
        }

        /// <summary>
        /// Method to assign data source for gvRiskCode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvRiskCode_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            bindSODRiskCode();
        }

        /// <summary>
        /// Method to handle selected index change for gvRiskCode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvRiskCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem gdi = gvRiskCode.SelectedItems[0] as GridDataItem;
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
        /// Method to initialise grid gvRiskCode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvRiskCode_Init(object sender, EventArgs e)
        {
            GridFilterMenu menu = gvRiskCode.FilterMenu;
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
                gvRiskCode.ExportSettings.FileName = "RiskCode";
                gvRiskCode.Export(gvRiskCode, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                gvRiskCode.Export<IList<SODRiskCode>>(SODRiskCodes, e.ActionType, "RiskCode");
            }

            else if (e.ActionType == ACLAction.Add)
            {
                DetailUC.Clearvalue();
                H1.InnerText = "Add Risk Code";
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
                H1.InnerText = "Edit Risk Code";
                btnSubmit.Text = "Update";
                SetPage();
                modalPopup.VisibleOnPageLoad = true;
            }
        }
        #endregion

        #region Helper
        /// <summary>
        /// Method to delete risk code
        /// </summary>
        private void delete()
        {
            try
            {
                if (!ContextObjectId.HasValue || ContextObjectId == 0)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please select an item");
                    return;
                }
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("riskCodeId", (this.ContextObjectId).ToString());
                bool isUsedInConflict = DomainApiClient<SODConflictFunctions>.GetByAction<bool>
                                (DomainApiAction.SODFunction.SODConflictFunction.HasRiskCode, parameter);
                bool isUsedInMitigation = DomainApiClient<SODMitigation>.GetByAction<bool>
                                (DomainApiAction.SODMitigation.HasRiskCode, parameter);
                if (isUsedInConflict || isUsedInMitigation)
                {
                    MasterPage.ShowAlert(AlertType.Warning,
                        "Risk Code could not be deleted because it is being referenced in Conflict Functions and/or Mitigation.");
                    return;
                }
                DomainApiClient<SODRiskCode>.DeleteById((long)this.ContextObjectId);
                bindSODRiskCode();
                gvRiskCode.DataBind();
                this.ContextObjectId = null;
                MasterPage.ShowAlert(AlertType.Success, "Risk Code Deleted Successfully");
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Warning, ex.Message);
            }
        }

        /// <summary>
        /// Method to set page values
        /// </summary>
        private void SetPage()
        {
            var RiskCode = DomainApiClient<SODRiskCode>.GetById((long)this.ContextObjectId, true);
            if (null == RiskCode)
                return;

            DetailUC.RiskCode = RiskCode.RiskCode;
            DetailUC.RiskCodeDescription = RiskCode.RiskDescription;
            DetailUC.RiskCodeLevel = RiskCode.sodrisklevel;
            DetailUC.RiskCodeScore = RiskCode.sodriskscore;
            DetailUC.RiskCodeOwner = RiskCode.user;
            DetailUC.RiskCodeStatus = RiskCode.Status;
        }

        /// <summary>
        /// Method to save risk code
        /// </summary>
        /// <returns></returns>
        private bool Save()
        {
            bool toSave = false;
            string ErrorMessage = "";
            int RiskCodeStatus = DetailUC.RiskCodeStatus;
            SODRiskCode riskCode = new SODRiskCode();
            if (btnSubmit.Text == "Add")
            {
                if (IsRiskCodeAlreadyExists(DetailUC.RiskCode))
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Risk Code Already Exists.");
                    return false;
                }
            }
            string obj = (string.IsNullOrEmpty(DetailUC.RiskCode) && DetailUC.RiskCodeLevel == null && DetailUC.RiskCodeScore == null && DetailUC.RiskCodeOwner == null) ?
                          "ALL" : (DetailUC.RiskCode == null ? "CODE" : (DetailUC.RiskCodeLevel == null ? "LEVEL" : (DetailUC.RiskCodeScore == null ? "SCORE" :
                          (DetailUC.RiskCodeOwner == null ? "OWNER" : ""))));
            switch (obj)
            {
                case "CODE":
                    ErrorMessage = "Risk Code should not be Empty";
                    toSave = false;
                    break;
                case "LEVEL":
                    ErrorMessage = "Risk Level should not be Empty";
                    toSave = false;
                    break;
                case "SCORE":
                    ErrorMessage = "Risk Score should not be Empty";
                    toSave = false;
                    break;
                case "OWNER":
                    ErrorMessage = "Risk Owner should not be Empty";
                    toSave = false;
                    break;
                case "ALL":
                    ErrorMessage = "Risk Code,Risk Level, Risk Score & Risk Owner should not be empty";
                    break;
                default:
                    toSave = true;
                    break;
            }
            if (toSave)
            {
                riskCode.Id = this.ContextObjectId.HasValue ? (long)this.ContextObjectId.Value : 0;
                riskCode.RiskCode = DetailUC.RiskCode;
                riskCode.RiskDescription = DetailUC.RiskCodeDescription;
                riskCode.UserId = DetailUC.RiskCodeOwner.Id;
                riskCode.RiskLevelId = DetailUC.RiskCodeLevel.Id;
                riskCode.RiskScoreId = DetailUC.RiskCodeScore.Id;
                riskCode.Status = DetailUC.RiskCodeStatus;

                bool isExists = DomainApiClient<Dto.SODRiskCode>.GetByAction<bool>(DomainApiAction.SODFunction.SODPolicyConfig.isExists, null, riskCode);
                if(isExists)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Requested combination already exists");
                    return false; 
                }

                DomainApiClient<SODRiskCode>.Save(riskCode);
                modalPopup.VisibleOnPageLoad = false;
                this.ContextObjectId = null;
                return true;
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
                return false;
            }
        }

        /// <summary>
        /// Method to check if risk code already exists
        /// </summary>
        /// <param name="RiskCode"></param>
        /// <returns></returns>
        private bool IsRiskCodeAlreadyExists(string RiskCode)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("RiskCode", RiskCode);
            return DomainApiClient<SODRiskCode>.GetByAction<bool>(DomainApiAction.Risks.SODRiskCode.DoesExists, parameter);
        }

        /// <summary>
        /// Method to bind data to grid gvRiskCode
        /// </summary>
        protected void bindSODRiskCode()
        {
            SODRiskCodes = DomainApiClient<SODRiskCode>.GetByAction<IList<SODRiskCode>>
                                                (DomainApiAction.Risks.SODRiskCode.GetAllByIncludeUser, null);
            gvRiskCode.DataSource = SODRiskCodes;
        }
        #endregion
    }
}