using System.Web;
using System;
using System.Collections.Generic;
using System.Web.UI;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using Application;
namespace DI.Chakra.Web.Comp.SODPolicyConfig.UC
{
    public partial class DetailUC : UIBaseUC, IContextObjectEnabled
    {
        #region Members
        private const string V_ENTITYID = "V_ENTITYID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private SessionStatusImpl sessionstatus = new SessionStatusImpl();
        #endregion

        #region Properties
        public long EntityId
        {
            get
            {
                if (ViewState[V_ENTITYID] == null)
                    return 0;
                return (long)ViewState[V_ENTITYID];
            }
            set
            {
                ViewState[V_ENTITYID] = value;
            }
        }

        public UIMode UIMode
        {
            get
            {
                if (ViewState[V_DISPLAYMODE] == null)
                    return UIMode.View;

                return (UIMode)ViewState[V_DISPLAYMODE];
            }
            set
            {
                ViewState[V_DISPLAYMODE] = value;
            }
        }

        public void BindRiskLevel()
        {
            drprisklevel.DataTextField = "RiskLevelCode";
            drprisklevel.DataValueField = "Id";
            drprisklevel.DataSource = DomainApiClient<Dto.SODRiskLevel>.GetAll();
            drprisklevel.DataBind();
            drprisklevel.Items.Insert(0, "--Select--");
            drprisklevel.SelectedIndex = 0;
        }

        public void BindRiskOperator()
        {
            drpriskoperator.DataTextField = "Value";
            drpriskoperator.DataValueField = "Key";
            drpriskoperator.DataSource = DI.Chakra.Infrastructure.UtilityMethods.GetAll<Operator>();
            drpriskoperator.DataBind();
            drpriskoperator.Items.Insert(0, "--Select--");
            drpriskoperator.SelectedIndex = 0;
        }

        public void BindRiskScoreOperator()
        {
            drpriskScoreOperator.DataTextField = "Value";
            drpriskScoreOperator.DataValueField = "Key";
            drpriskScoreOperator.DataSource = DI.Chakra.Infrastructure.UtilityMethods.GetAll<RiskScoreOperator>();
            drpriskScoreOperator.DataBind();
            drpriskScoreOperator.Items.Insert(0, "--Select--");
            drpriskScoreOperator.SelectedIndex = 0;
        }

        private void SetPage()
        {
            var sodpolicyconfig = DomainApiClient<Dto.SODPolicyConfiguration>.GetById(EntityId);

            if (null == sodpolicyconfig)
                return;
            drprisklevel.SelectedValue = sodpolicyconfig.RiskLevelId.ToString();
            drpriskScoreOperator.SelectedValue = sodpolicyconfig.RiskScoreOperator.ToString();
            txtriskscore.Text = sodpolicyconfig.RiskScore.ToString();
            drpriskoperator.SelectedValue = sodpolicyconfig.Operator.ToString();
        }

        private bool DoesExists(long riskrevel)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("risklevel", riskrevel.ToString());
            return DomainApiClient<Dto.SODPolicyConfiguration>.GetByAction<bool>(DomainApiAction.SODFunction.SODConfiguration.DoesExists, parameter);
        }


        private void ViewOnlyMode()
        {
            Mainpanel.Enabled = false;
        }


        private void Save()
        {
            string validationMessage = ValidateThreshold();
            if (!string.IsNullOrEmpty(validationMessage))
            {
                MasterPage.ShowAlert(AlertType.Error, validationMessage);
                return;
            }

            SODPolicyConfiguration sodpolicyconfig = new SODPolicyConfiguration();
            sodpolicyconfig.RiskLevelId = Convert.ToInt64(drprisklevel.SelectedValue.ToString());
            sodpolicyconfig.Operator = drpriskoperator.SelectedValue;
            sodpolicyconfig.RiskScoreOperator = drpriskScoreOperator.SelectedValue;
            sodpolicyconfig.RiskScore = Convert.ToInt64(txtriskscore.Text);
            if (UIMode == UIMode.Edit)
            {
                sodpolicyconfig.Id = EntityId;
            }
            else if(UIMode==UIMode.Add)
            {
                //Dictionary<string, string> parameter = new Dictionary<string, string>();
                //parameter.Add("risklevel", sodpolicyconfig.RiskLevelId.ToString());
                bool isExists = DomainApiClient<Dto.SODPolicyConfiguration>.GetByAction<bool>(DomainApiAction.SODFunction.SODPolicyConfig.isExists,null,sodpolicyconfig);
                if (isExists)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "SOD Policy already exists");
                    return;
                }
            }
            
            DomainApiClient<Dto.SODPolicyConfiguration>.Save(sodpolicyconfig);
            MasterPage.ShowAlert(AlertType.Success, "SOD Policy Configuration Saved Successfully.");
            Response.Redirect("~/Comp/SODPolicyConfig/List.aspx");
        }

        public string ValidateThreshold()
        {
            string errorMessage = string.Empty;
            string obj = ((((string.IsNullOrEmpty(drprisklevel.SelectedItem.Value) || drprisklevel.SelectedValue == "--Select--")) && ((string.IsNullOrEmpty(drpriskoperator.SelectedItem.Value) || drpriskoperator.SelectedValue == "--Select--")) && ((string.IsNullOrEmpty(drpriskScoreOperator.SelectedItem.Value) || drpriskScoreOperator.SelectedValue == "--Select--")) && String.IsNullOrEmpty(txtriskscore.Text)) ? "All" :
                ((((string.IsNullOrEmpty(drpriskoperator.SelectedItem.Value) || drpriskoperator.SelectedValue == "--Select--")) && ((string.IsNullOrEmpty(drpriskScoreOperator.SelectedItem.Value) || drpriskScoreOperator.SelectedValue == "--Select--")) && String.IsNullOrEmpty(txtriskscore.Text)) ? "OperatorScoreOperatorScore" :
                ((((string.IsNullOrEmpty(drprisklevel.SelectedItem.Value) || drprisklevel.SelectedValue == "--Select--")) && ((string.IsNullOrEmpty(drpriskScoreOperator.SelectedItem.Value) || drpriskScoreOperator.SelectedValue == "--Select--")) && String.IsNullOrEmpty(txtriskscore.Text)) ? "LevelScoreOperatorScore" :
                ((((string.IsNullOrEmpty(drpriskoperator.SelectedItem.Value) || drpriskoperator.SelectedValue == "--Select--")) && ((string.IsNullOrEmpty(drprisklevel.SelectedItem.Value) || drprisklevel.SelectedValue == "--Select--")) && String.IsNullOrEmpty(txtriskscore.Text)) ? "LevelOperatorScore" :
                ((((string.IsNullOrEmpty(drpriskoperator.SelectedItem.Value) || drpriskoperator.SelectedValue == "--Select--")) && ((string.IsNullOrEmpty(drpriskScoreOperator.SelectedItem.Value) || drpriskScoreOperator.SelectedValue == "--Select--")) && ((string.IsNullOrEmpty(drprisklevel.SelectedItem.Value) || drprisklevel.SelectedValue == "--Select--"))) ? "LevelOperatorScoreOperator" :
                ((((string.IsNullOrEmpty(drprisklevel.SelectedItem.Value) || drprisklevel.SelectedValue == "--Select--")) && ((string.IsNullOrEmpty(drpriskoperator.SelectedItem.Value) || drpriskoperator.SelectedValue == "--Select--"))) ? "RiskLevels" :
                ((((string.IsNullOrEmpty(drprisklevel.SelectedItem.Value) || drprisklevel.SelectedValue == "--Select--")) && ((string.IsNullOrEmpty(drpriskScoreOperator.SelectedItem.Value) || drpriskScoreOperator.SelectedValue == "--Select--"))) ? "LevelScoreOperator" :
                ((((string.IsNullOrEmpty(drpriskoperator.SelectedItem.Value) || drpriskoperator.SelectedValue == "--Select--")) && ((string.IsNullOrEmpty(drpriskScoreOperator.SelectedItem.Value) || drpriskScoreOperator.SelectedValue == "--Select--"))) ? "OperatorScoreOperator" :
                ((((string.IsNullOrEmpty(drprisklevel.SelectedItem.Value) || drprisklevel.SelectedValue == "--Select--")) && String.IsNullOrEmpty(txtriskscore.Text)) ? "LevelScore" :
                ((((string.IsNullOrEmpty(drpriskScoreOperator.SelectedItem.Value) || drpriskScoreOperator.SelectedValue == "--Select--")) && String.IsNullOrEmpty(txtriskscore.Text)) ? "RiskScores" :
                (((string.IsNullOrEmpty(drprisklevel.SelectedItem.Value) || drprisklevel.SelectedValue == "--Select--")) ? "Level" :
                (((string.IsNullOrEmpty(drpriskoperator.SelectedItem.Value) || drpriskoperator.SelectedValue == "--Select--")) ? "LevelOperator" :
                (((string.IsNullOrEmpty(drpriskScoreOperator.SelectedItem.Value) || drpriskScoreOperator.SelectedValue == "--Select--")) ? "ScoreOperator" :
                ((String.IsNullOrEmpty(txtriskscore.Text)) ? "Score" : ""))))))))))))));
            switch (obj)
            {
                case "All":
                    errorMessage = "Risk Level,Risk Operator,Risk Score Operator & Risk Score should not be empty";
                    break;
                case "OperatorScoreOperatorScore":
                    errorMessage = "Risk Operator,Risk Score Operator & Risk Score should not be empty";
                    break;
                case "LevelScoreOperatorScore":
                    errorMessage = "Risk Level,Risk Score Operator & Risk Score should not be empty";
                    break;
                case "LevelOperatorScore":
                    errorMessage = "Risk Level,Risk Operator & Risk Score should not be empty";
                    break;
                case "LevelOperatorScoreOperator":
                    errorMessage = "Risk Level,Risk Operator & Risk Score Operator should not be empty";
                    break;
                case "RiskLevels":
                    errorMessage = "Risk Level & Risk Operator should not be Empty";
                    break;
                case "RiskScores":
                    errorMessage = "Risk Score Operator & Risk Score should not be Empty";
                    break;
                case "LevelScoreOperator":
                    errorMessage = "Risk Level & Risk Score Operator should not be Empty";
                    break;
                case "OperatorScoreOperator":
                    errorMessage = "Risk Operator & Risk Score Operator should not be Empty";
                    break;
                case "LevelScore":
                    errorMessage = "Risk Level & Risk Score should not be Empty";
                    break;
                case "Level":
                    errorMessage = "Risk Level should not be Empty";
                    break;
                case "LevelOperator":
                    errorMessage = "Risk Operator should not be Empty";
                    break;
                case "ScoreOperator":
                    errorMessage = "Risk Score Operator should not be Empty";
                    break;
                case "Score":
                    errorMessage = "Risk Score should not be Empty";
                    break;
            }
            return errorMessage;
        }


        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                BindRiskLevel();
                BindRiskOperator();
                BindRiskScoreOperator();

                if (UIMode == UIMode.Edit || UIMode == UIMode.View)
                {
                    if (this.ContextObjectId == null)
                        return;
                    this.EntityId = (long)this.ContextObjectId.Value;
                    SetPage();
                }
                if (UIMode == UIMode.View)
                    ViewOnlyMode();
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                Save();
            }
        }

    }
}