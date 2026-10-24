using System;
using Application;
using DI.Chakra.Infrastructure;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;

namespace DI.Chakra.Web.Admin.AppSetting
{
    public partial class ApplicationSetting : DI.Chakra.Web.UIBasePage, IAccessControl
    {
        #region Members

        List<Dto.ConfigurationMaster> listConfig = new List<Dto.ConfigurationMaster>();
        List<Dto.ConfigurationMasterPasswordManagement> listPMConfig = new List<Dto.ConfigurationMasterPasswordManagement>();

        private SessionStatusImpl SessionStatus = new SessionStatusImpl();

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            listConfig = DomainApiClient<Dto.ConfigurationMaster>.GetAll();
            if (!IsPostBack)
            {
                DateTime start = DateTime.Now;

                listPMConfig = DomainApiClient<Dto.ConfigurationMasterPasswordManagement>.GetAll();
                setAuthenticationMode();
                setAuthenticationDomain();
                setPasswordResetOption();
                setNoofSecurityQuestion();
                setCultures();
                setOtpOption();
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Admin/AppSetting - Save");
                Save();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AppSetting/ApplicationSetting.aspx");
        }

        protected void lblSetup_Click(object sender, EventArgs e)
        {
            setvalues();
            ProvisioningSetup.VisibleOnPageLoad = true;
        }

        protected void lblServiceMonitor_Click(object sender, EventArgs e)
        {
            ServiceMonitor.VisibleOnPageLoad = true;
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AppSetting/ApplicationSetting.aspx");
        }

        protected void btnSMCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AppSetting/ApplicationSetting.aspx");
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            //ServiceMonitorUC.GetServiceStatus();
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            Dto.IConfigurationMaster config = GetConfigurationMaster(ProvEngineSetupUC.RunOnBoardTriggerText, Product.IM);
            if (config != null)
            {
                config.ConfigValue = ProvEngineSetupUC.RunOnBoardTrigger ? "1" : "0";
                DomainApiClient<Dto.ConfigurationMasterIM>.Save((Dto.ConfigurationMasterIM)config);
            }
            config = GetConfigurationMaster(ProvEngineSetupUC.RunOffBoardTriggerText, Product.IM);
            if (config != null)
            {
                config.ConfigValue = ProvEngineSetupUC.RunOffBoardTrigger ? "1" : "0";
                DomainApiClient<Dto.ConfigurationMasterIM>.Save((Dto.ConfigurationMasterIM)config);
            }
            config = GetConfigurationMaster(ProvEngineSetupUC.RunTransferTriggerText, Product.IM);
            if (config != null)
            {
                config.ConfigValue = ProvEngineSetupUC.RunTransferTrigger ? "1" : "0";
                DomainApiClient<Dto.ConfigurationMasterIM>.Save((Dto.ConfigurationMasterIM)config);
            }
            config = GetConfigurationMaster(ProvEngineSetupUC.RunPropagationTriggerText, Product.IM);
            if (config != null)
            {
                config.ConfigValue = ProvEngineSetupUC.RunPropagationTrigger ? "1" : "0";
                DomainApiClient<Dto.ConfigurationMasterIM>.Save((Dto.ConfigurationMasterIM)config);
            }
            config = GetConfigurationMaster(ProvEngineSetupUC.RunReOrgTriggerText, Product.IM);
            if (config != null)
            {
                config.ConfigValue = ProvEngineSetupUC.RunReOrgTrigger ? "1" : "0";
                DomainApiClient<Dto.ConfigurationMasterIM>.Save((Dto.ConfigurationMasterIM)config);
            }
            config = GetConfigurationMaster(ProvEngineSetupUC.RunAttRemediationText, Product.IACM);
            if (config != null)
            {
                config.ConfigValue = ProvEngineSetupUC.RunAttRemediation ? "1" : "0";
                DomainApiClient<Dto.ConfigurationMaster>.Save((Dto.ConfigurationMaster)config);
            }


            config = GetConfigurationMaster(ProvEngineSetupUC.CanRunRehireTriggerText, Product.IM);
            if (config != null)
            {
                config.ConfigValue = ProvEngineSetupUC.RunRehireTrigger ? "1" : "0";
                DomainApiClient<Dto.ConfigurationMasterIM>.Save((Dto.ConfigurationMasterIM)config);
            }
            config = GetConfigurationMaster(ProvEngineSetupUC.RunPromotionTriggerText, Product.IM);
            if (config != null)
            {
                config.ConfigValue = ProvEngineSetupUC.RunPromotionTrigger ? "1" : "0";
                DomainApiClient<Dto.ConfigurationMasterIM>.Save((Dto.ConfigurationMasterIM)config);
            }

            Response.Redirect("~/Admin/AppSetting/ApplicationSetting.aspx");
        }

        protected void btnCancel1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AdminUI/List.aspx");
        }

        #endregion

        #region Methods

        private Dto.ConfigurationMaster getAuthenticationMode()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("configKey", ConfigConstants.AUTHENTICATIONMODE);
            parameter.Add("product", Product.IACM.ToString());
            Dto.IConfigurationMaster config = DomainApiClient<Dto.ConfigurationMaster>.GetByAction<Dto.IConfigurationMaster>
                (DomainApiAction.Configuration.ConfigurationMaster.GetByConfigKey, parameter);

            if (config == null)
                return null;

            config.ConfigKey = ConfigConstants.AUTHENTICATIONMODE;
            config.ConfigValue = AuthenticationModeField.SelectedItem.Value;
            return (Dto.ConfigurationMaster)config;
        }

        private Dto.ConfigurationMaster getAuthenticationDomain()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("configKey", ConfigConstants.AUTHENTICATIONDOMAIN);
            parameter.Add("product", Product.IACM.ToString());
            Dto.IConfigurationMaster config = DomainApiClient<Dto.ConfigurationMaster>.GetByAction<Dto.IConfigurationMaster>
                (DomainApiAction.Configuration.ConfigurationMaster.GetByConfigKey, parameter);

            if (config == null)
                return null;
            else
            {
                config.ConfigKey = ConfigConstants.AUTHENTICATIONDOMAIN;
                config.ConfigValue = (AuthenticationDomainField.SelectedItem.Text == "---Select---") ? String.Empty : Convert.ToString(AuthenticationDomainField.SelectedValue);
                return (Dto.ConfigurationMaster)config;
            }
        }

        private void setAuthenticationMode()
        {
            if (listConfig.Exists(cm => cm.ConfigKey == ConfigConstants.AUTHENTICATIONMODE))
            {
                var authenticationMode = (AuthenticationMode[])Enum.GetValues(typeof(AuthenticationMode));
                AuthenticationModeField.DataSource = authenticationMode;
                AuthenticationModeField.DataBind();
                AuthenticationModeField.SelectedValue = listConfig.Find(cm => cm.ConfigKey == ConfigConstants.AUTHENTICATIONMODE).ConfigValue;
            }
        }

        private void setAuthenticationDomain()
        {
            AuthenticationDomainField.Items.Clear();
            if (AuthenticationModeField.SelectedValue == AuthenticationMode.AD.ToString() ||
                AuthenticationModeField.SelectedValue == AuthenticationMode.MixedMode.ToString())
            {
                AuthenticationDomainField.Enabled = true;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", Convert.ToString((long)EndpointType.ActiveDirectory));
                List<Dto.EndPoint> adEndpoints = DomainApiClient<Dto.EndPoint>.GetByAction<List<Dto.EndPoint>>
                   (DomainApiAction.Endpoint.EndPoint.GetNonTrustedSource, parameter);

                foreach (Dto.EndPoint endpoint in adEndpoints)
                {
                    if (endpoint.IsActive)
                    {
                        ListItem item = new ListItem(endpoint.DisplayName, endpoint.Id.ToString());
                        AuthenticationDomainField.Items.Add(item);
                    }
                }

                string authDomainField = listConfig.Find(cm => cm.ConfigKey == ConfigConstants.AUTHENTICATIONDOMAIN).ConfigValue;
                if (!string.IsNullOrEmpty(authDomainField))
                {
                    if (AuthenticationDomainField.Items.FindByValue(authDomainField) != null)
                        AuthenticationDomainField.SelectedValue = authDomainField;
                }
                else
                {
                    AuthenticationDomainField.Items.Insert(0, "---Select---");
                    AuthenticationDomainField.Items.FindByValue("---Select---").Selected = true;
                }
            }
            else
            {
                AuthenticationDomainField.Items.Insert(0, "---Select---");
                AuthenticationDomainField.Items.FindByValue("---Select---").Selected = true;
                AuthenticationDomainField.Enabled = false;
            }
        }

        private Dto.ConfigurationMasterPasswordManagement getOtpOption()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("configKey", ConfigConstants.OTPOPTION);
            parameter.Add("product", Product.PWDManagement.ToString());
            Dto.IConfigurationMaster config = DomainApiClient<Dto.ConfigurationMaster>.GetByAction<Dto.IConfigurationMaster>
                (DomainApiAction.Configuration.ConfigurationMaster.GetByConfigKey, parameter);

            if (config == null)
                return null;

            config.ConfigKey = ConfigConstants.OTPOPTION;
            config.ConfigValue = OTPOptionfield.SelectedItem.Text;
            return (Dto.ConfigurationMasterPasswordManagement)config;
        }

        private void setOtpOption()
        {
            if (listPMConfig.Exists(cm => cm.ConfigKey == ConfigConstants.OTPOPTION))
            {
                var otpoption = (OTPOption[])Enum.GetValues(typeof(OTPOption));
                OTPOptionfield.DataSource = otpoption;
                OTPOptionfield.DataBind();
                OTPOptionfield.SelectedValue = listPMConfig.Find(cm => cm.ConfigKey == ConfigConstants.OTPOPTION).ConfigValue;
            }
        }

        private Dto.ConfigurationMasterPasswordManagement getPasswordResetOption()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("configKey", ConfigConstants.PASSWORDRESETOPTION);
            parameter.Add("product", Product.PWDManagement.ToString());
            Dto.IConfigurationMaster config = DomainApiClient<Dto.ConfigurationMaster>.GetByAction<Dto.IConfigurationMaster>
                (DomainApiAction.Configuration.ConfigurationMaster.GetByConfigKey, parameter);
            if (config == null)
                return null;

            config.ConfigKey = ConfigConstants.PASSWORDRESETOPTION;
            config.ConfigValue = PasswordResetOptionField.SelectedItem.Value;
            return (Dto.ConfigurationMasterPasswordManagement)config;
        }

        private void setPasswordResetOption()
        {
            if (listPMConfig.Exists(cm => cm.ConfigKey == ConfigConstants.PASSWORDRESETOPTION))
            {
                var authenticationMode = (PasswordResetOption[])Enum.GetValues(typeof(PasswordResetOption));
                PasswordResetOptionField.DataSource = authenticationMode;
                PasswordResetOptionField.DataBind();
                PasswordResetOptionField.SelectedValue = listPMConfig.Find(cm => cm.ConfigKey == ConfigConstants.PASSWORDRESETOPTION).ConfigValue;
            }
        }

        private Dto.ConfigurationMasterPasswordManagement getNoofSecurityQuestion()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("configKey", ConfigConstants.NOOFSECURITYQUESTIONS);
            parameter.Add("product", Product.PWDManagement.ToString());
            Dto.IConfigurationMaster config = DomainApiClient<Dto.ConfigurationMaster>.GetByAction<Dto.IConfigurationMaster>
                (DomainApiAction.Configuration.ConfigurationMaster.GetByConfigKey, parameter);

            if (config == null)
                return null;

            config.ConfigKey = ConfigConstants.NOOFSECURITYQUESTIONS;
            config.ConfigValue = NoofSecurityQuestion.Text;
            return (Dto.ConfigurationMasterPasswordManagement)config;
        }

        private void setNoofSecurityQuestion()
        {
            if (listPMConfig.Count > 0)
            {
                NoofSecurityQuestion.Text = listPMConfig.Find(cm => cm.ConfigKey == ConfigConstants.NOOFSECURITYQUESTIONS).ConfigValue;
            }
        }

        private void setCultures()
        {

            CultureInfo[] cultureInfoList = CultureInfo.GetCultures(CultureTypes.AllCultures);
            ddlCultures.DataSource = cultureInfoList;
            ddlCultures.DataTextField = "EnglishName";
            ddlCultures.DataValueField = "Name";
            ddlCultures.DataBind();

            if (listConfig.Count > 0)
            {
                ddlCultures.Items.FindByValue(listConfig.Find(cm => cm.ConfigKey == ConfigConstants.APPLICATIONCULTURE).ConfigValue).Selected = true;
            }
        }

        private Dto.ConfigurationMaster getCultures()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("configKey", ConfigConstants.APPLICATIONCULTURE);
            parameter.Add("product", Product.IACM.ToString());
            Dto.IConfigurationMaster config = DomainApiClient<Dto.ConfigurationMaster>.GetByAction<Dto.IConfigurationMaster>
                (DomainApiAction.Configuration.ConfigurationMaster.GetByConfigKey, parameter);

            if (config == null)
                return null;
            config.ConfigKey = ConfigConstants.APPLICATIONCULTURE;
            config.ConfigValue = ddlCultures.SelectedValue.ToString();
            return (Dto.ConfigurationMaster)config;
        }

        public void Save()
        {
            var authenticationMode = getAuthenticationMode();
            DomainApiClient<Dto.ConfigurationMaster>.Save(getAuthenticationMode());
            var authenticationDomain = getAuthenticationDomain();
            if (authenticationMode!=null && authenticationDomain != null)
            {
                if (authenticationMode.ConfigValue != Convert.ToString(AuthenticationMode.IACM) && string.IsNullOrEmpty(authenticationDomain.ConfigValue))
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please select Authentication Domain");
                    return;
                }
                DomainApiClient<Dto.ConfigurationMaster>.Save(authenticationDomain);
            }
            DomainApiClient<Dto.ConfigurationMasterPasswordManagement>.Save(getPasswordResetOption());
            DomainApiClient<Dto.ConfigurationMasterPasswordManagement>.Save(getNoofSecurityQuestion());
            DomainApiClient<Dto.ConfigurationMaster>.Save(getCultures());
            DomainApiClient<Dto.ConfigurationMasterPasswordManagement>.Save(getOtpOption());
            MasterPage.ShowAlert(AlertType.Success, "Settings Updated Successfully");
        }

        private Dto.IConfigurationMaster GetConfigurationMaster(string ConfigKey, Product product)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("configKey", ConfigKey);
            parameter.Add("product", product.ToString());
            Dto.IConfigurationMaster config = DomainApiClient<Dto.ConfigurationMaster>.GetByAction<Dto.IConfigurationMaster>
                (DomainApiAction.Configuration.ConfigurationMaster.GetByConfigKey, parameter);
            return config;
        }


        private void setvalues()
        {
            Dto.IConfigurationMaster config = GetConfigurationMaster(ProvEngineSetupUC.RunOnBoardTriggerText, Product.IM);
            ProvEngineSetupUC.RunOnBoardTrigger = config.ConfigValue != "0" ? true : false;
            ProvEngineSetupUC.RunOnBoardTriggerStr = config.Description;

            config = GetConfigurationMaster(ProvEngineSetupUC.RunOffBoardTriggerText, Product.IM);
            ProvEngineSetupUC.RunOffBoardTrigger = config.ConfigValue != "0" ? true : false;
            ProvEngineSetupUC.RunOffBoardTriggerStr = config.Description;

            config = GetConfigurationMaster(ProvEngineSetupUC.RunPropagationTriggerText, Product.IM);
            ProvEngineSetupUC.RunPropagationTrigger = config.ConfigValue != "0" ? true : false;
            ProvEngineSetupUC.RunPropagationTriggerStr = config.Description;

            config = GetConfigurationMaster(ProvEngineSetupUC.RunReOrgTriggerText, Product.IM);
            ProvEngineSetupUC.RunReOrgTrigger = config.ConfigValue != "0" ? true : false;
            ProvEngineSetupUC.RunReOrgTriggerStr = config.Description;

            config = GetConfigurationMaster(ProvEngineSetupUC.RunTransferTriggerText, Product.IM);
            ProvEngineSetupUC.RunTransferTrigger = config.ConfigValue != "0" ? true : false;
            ProvEngineSetupUC.RunTransferTriggerStr = config.Description;

            config = GetConfigurationMaster(ProvEngineSetupUC.RunAttRemediationText, Product.IACM);
            ProvEngineSetupUC.RunAttRemediation = config.ConfigValue != "0" ? true : false;
            ProvEngineSetupUC.RunAttRemediationStr = config.Description;

            config = GetConfigurationMaster(ProvEngineSetupUC.CanRunRehireTriggerText, Product.IM);
            ProvEngineSetupUC.RunRehireTrigger = config.ConfigValue != "0" ? true : false;
            ProvEngineSetupUC.RunRehireTriggerStr = config.Description;

            config = GetConfigurationMaster(ProvEngineSetupUC.RunPromotionTriggerText, Product.IM);
            ProvEngineSetupUC.RunPromotionTrigger = config.ConfigValue != "0" ? true : false;
            ProvEngineSetupUC.RunPromotionTriggerStr = config.Description;
        }

        #endregion

        protected void AuthenticationModeField_SelectedIndexChanged(object sender, EventArgs e)
        {
            setAuthenticationDomain();
        }
    }
}