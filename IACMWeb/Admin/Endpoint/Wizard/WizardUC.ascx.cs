using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Web.EndPoints.EndpointSystem.Common;
using DI.Chakra.Web.UsrControl.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace DI.Chakra.Web.EndPoints.EndpointSystem.Wizard
{
    public partial class WizardUC : UIBaseUC
    {
        #region Members
        SessionStatusImpl _sessionStatus = new SessionStatusImpl();
        private EndpointConfiguration mConfiguration;
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private const string V_ENDPOINTTYPE = "V_ENDPOINTTYPE";

        #endregion

        #region Properties
        public EndpointConfiguration EndpointConfiguration
        {
            set
            {
                mConfiguration = value;
                SetPage();
            }
        }

        public EndpointType EndpointType
        {
            get
            {
                if (ViewState[V_ENDPOINTTYPE] == null)
                    return EndpointType.ActiveDirectory;
                return (EndpointType)ViewState[V_ENDPOINTTYPE];
            }
            set
            {
                ViewState[V_ENDPOINTTYPE] = value;
                InitilizePage();
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

        #endregion

        #region Methods

        private void ViewOnlyMode()
        {
            EndpointInfoUC.DisplayViewMode();
            EndpointConfigurationUC.DisplayViewMode();
            MappingDetailUC.DisplayViewMode();
        }

        private void SetPage()
        {
            EndpointInfoUC.EndpointSystem = mConfiguration.EndpointSystem;
            MappingDetailUC.EndpointTypeId = (long)mConfiguration.EndpointSystem.EndpointSystemTypeId.Value;
            MappingDetailUC.EndpointSystemId = (long)mConfiguration.EndpointSystemId.Value;
            if (mConfiguration.MapAll)
            {
                MappingDetailUC.MapAll = true;
                MappingDetailUC.MapAnyOne = false;
            }
            else
            {
                MappingDetailUC.MapAll = false;
                MappingDetailUC.MapAnyOne = true;
            }
            MappingDetailUC.UserObjectName = mConfiguration.EndpointSystem.UserObjectName;
            MappingDetailUC.ListAttributeMappingColumns = mConfiguration.EndpointSystem.ListAttributeMappingColumns;
        }

        public void InitilizePage()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endpointSystemTypeName", EndpointType.ToString());
            var Type = DomainApiClient<Dto.EndPointSystemType>.GetByAction<EndPointSystemType>
                (DomainApiAction.Endpoint.EndPointSystemType.GetByEndpointSystemTypeName, parameter);
            long typeId = Type == null ? 0 : Type.Id;
            if (Type != null)
                EndpointInfoUC.MakeTSourceVisible = Type.CanBeTrustedSource;
            EndpointInfoUC.EndpointTypeId = typeId;
            MappingDetailUC.EndpointTypeId = typeId;
            EndpointInfoUC.InitializePage();
            MappingDetailUC.InitializePage();
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RadWizardStep1.Title = T("App.Common.Endpoint") + " Information";
            }
            //else
            //{
                if (EndpointInfoUC != null  && EndpointInfoUC.IsTrustedSourceSelected)
                    Wizard.WizardSteps.Remove(RadWizardStep3);
            //}
        } 

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            EndpointConfigurationUC.EndpointSystemTypeId = Convert.ToInt64(EndpointType);
            EndpointConfigurationUC.Initialize();
            if (this.UIMode == DI.Chakra.Infrastructure.Enums.UIMode.View)
                ViewOnlyMode();
            if (this.UIMode == Infrastructure.Enums.UIMode.Edit || this.UIMode == DI.Chakra.Infrastructure.Enums.UIMode.View && mConfiguration != null)
                EndpointConfigurationUC.EndpointConfiguration = mConfiguration;
            if (!IsPostBack && this.UIMode == Infrastructure.Enums.UIMode.Add)
                EndpointConfigurationUC.ResetPassword();
        }

        protected void Wizard_FinishButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            try
            {
                EndPoint ep = EndpointInfoUC.EndpointSystem;
                if ((EndpointInfoUC.isEpNameExists(this.UIMode)) && (this.UIMode != UIMode.View))
                {
                    MasterPage.ShowAlert(AlertType.Warning, T("App.Common.Endpoint") + " Name already exists. Please select a different name.");
                    return;
                }
                if ((EndpointInfoUC.isExistApplicationCode()) > 0)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Application Code already exists. Please enter a different Application Code.");
                    return;
                }
                if(ep.ApplicationType == ApplicationType.None)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please select ApplicationType");
                    return;
                }
                ep.UserObjectName = MappingDetailUC.UserObjectName;
                ep.ListAttributeMappingColumns = MappingDetailUC.ListAttributeMappingColumns;
                Dto.EndpointConfiguration config = new EndpointConfiguration();
                config = EndpointConfigurationUC.EndpointConfiguration;
                config.MapAll = MappingDetailUC.MapAll;
                config.EndpointSystemId = ep.Id;
                config.EndpointSystem = ep;
                //ADSConfiguration adsConfig = ADSConfigurationUC.adsConfiguration;

                if (this.UIMode == UIMode.View)
                {
                    Response.Redirect("~/Admin//Endpoint/List.aspx", false);
                }

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("key", "IsWOG");
                parameter.Add("value", Boolean.TrueString);
                EndPoint wogEp = DomainApiClient<Dto.EndPoint>.GetByAction<List<EndPoint>>(DomainApiAction.Endpoint.EndPoint.GetByEndpointConfiguration, parameter).FirstOrDefault();
                parameter.Clear();
                parameter.Add("key", "IsOneWayTrust");
                parameter.Add("value", Boolean.TrueString);
                List<EndPoint> oneWayTrustEps = DomainApiClient<Dto.EndPoint>.GetByAction<List<EndPoint>>(DomainApiAction.Endpoint.EndPoint.GetByEndpointConfiguration, parameter);

                if (wogEp != null)
                {
                    if (EndpointConfigurationUC.IsWoGSelected && (config.EndpointSystem.Id <= 0 || config.EndpointSystem.Id != wogEp.Id))
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "WOG AD already configured,Only one WOG AD can exist");
                        return;
                    }
                    else if (!EndpointConfigurationUC.IsWoGSelected && oneWayTrustEps != null)
                    {
                        if (config.EndpointSystem.Id == wogEp.Id && oneWayTrustEps.Count > 0)
                        {
                            MasterPage.ShowAlert(AlertType.Warning, "Cannot uncheck Is WOG due to dependencies");
                            return;
                        }
                    }
                }
                if (EndpointInfoUC.IsAttestationSelected && EndpointConfigurationUC.IsWoGSelected)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "PLease Dissable Attestation,WOG AD cannot be Attested");
                    return;
                }

                if (this.UIMode == UIMode.Edit || this.UIMode == UIMode.Add)
                {
                    
                    var usr = new SessionStatusImpl().User;
                    if (this.UIMode == UIMode.Edit)
                    {
                        var endpoint = DomainApiClient<Dto.EndPoint>.GetById(config.EndpointSystemId.Value, true);
                        var endpointConfig = DomainApiClient<Dto.EndpointConfiguration>.GetById(config.Id, true);

                        DomainApiClient<Dto.EndPoint>.GetByAction<object>(DomainApiAction.Endpoint.EndPoint.SaveConfiguration, null, config);
                        var newOwnerDisplayName = ep.OwnerName;
                        var oldUser = DomainApiClient<Dto.User>.GetById(endpoint.OwnerUserId.Value);
                        string oldOwnerDisplayName = string.Empty;
                        if (oldUser != null) {
                            oldOwnerDisplayName = $"{oldUser.DisplayName}({oldUser.UserName})";
                        }
                        var changes = string.Empty;
                        string[] excludes = new string[] { "LastModifiedDate", "LastModifiedBy", "CreatedDate", "CreatedBy","Id","EndpointSystemId","EndpointConfigurationId","KeyId","ConfigurationDetails","ListAttributeMappingColumns","EndpointSystem" };
                        changes = $"EndPoint:{endpoint.DisplayName} Edited: {UtilityMethods.PublicInstancePropertiesEqualWithAdditionalValue<EndPoint>(endpoint, config.EndpointSystem, oldOwnerDisplayName, newOwnerDisplayName, excludes)}";
                        changes += $"{Environment.NewLine} {UtilityMethods.PublicInstancePropertiesEqual<EndpointConfiguration>(config, endpointConfig, excludes)}";
                        
                        StringBuilder sbConfig = new StringBuilder();
                        if (endpointConfig?.ConfigurationDetails != null && endpointConfig?.ConfigurationDetails.Count > 0)
                        {
                            sbConfig.Append("<br/>");
                            sbConfig.Append("[ConfigurationDetails  Before Save]: ");
                            foreach (var item in config.ConfigurationDetails)
                            {
                                if (item.KeyId > 0 && EndpointConfigurationUC.EndpointConfiguration != null && EndpointConfigurationUC.EndpointConfiguration.ConfigurationDetails != null && EndpointConfigurationUC.EndpointConfiguration.ConfigurationDetails.Count > 0)
                                {
                                    var data = EndpointConfigurationUC.EndpointConfiguration.ConfigurationDetails.FirstOrDefault(i => i.KeyId == item.KeyId);
                                    if (data != null && data.EndpointConfigurationKey?.KeyName != null)
                                    {

                                        sbConfig.AppendFormat("{0} : {1} {2}", data.EndpointConfigurationKey?.KeyName, item.Value, Environment.NewLine);
                                    }
                                }
                            }
                        }
                            
                        if(endpointConfig?.ConfigurationDetails != null && endpointConfig?.ConfigurationDetails.Count > 0)
                        {
                            sbConfig.Append("<br/>");
                            sbConfig.Append("[ConfigurationDetails  After Save]: ");
                            foreach (var item in endpointConfig.ConfigurationDetails)
                            {
                                if (item.KeyId > 0 && EndpointConfigurationUC.EndpointConfiguration != null && EndpointConfigurationUC.EndpointConfiguration.ConfigurationDetails != null && EndpointConfigurationUC.EndpointConfiguration.ConfigurationDetails.Count > 0)
                                {
                                    var data = EndpointConfigurationUC.EndpointConfiguration.ConfigurationDetails.FirstOrDefault(i => i.KeyId == item.KeyId);
                                    if (data != null && data.EndpointConfigurationKey?.KeyName != null)
                                    {

                                        sbConfig.AppendFormat("{0} : {1} {2}", data.EndpointConfigurationKey?.KeyName, item.Value, Environment.NewLine);
                                    }
                                }
                            }
                        }
                       
                        
                        changes += $"{sbConfig.ToString()}";
                        sbConfig.Clear();
                        sbConfig.Append("<br/>");
                        if (config.EndpointSystem?.ListAttributeMappingColumns != null)
                        {
                            sbConfig.AppendLine("[AttributeMapping Before Save]:");
                            foreach (var item in config.EndpointSystem.ListAttributeMappingColumns)
                            {
                                sbConfig.AppendLine($"DiColumn:{ item.DiColumn} TargetColumn:{item.Tgcolumn} {Environment.NewLine}");
                            }
                           
                        }
                        if (endpoint.ListAttributeMappingColumns != null)
                        {
                            sbConfig.AppendLine("[AttributeMapping After Save]:");
                            foreach (var item in endpoint.ListAttributeMappingColumns)
                            {
                                sbConfig.AppendLine($"DiColumn:{ item.DiColumn} TargetColumn:{item.Tgcolumn} {Environment.NewLine}");
                            }
                        }
                            
                        changes += $"{sbConfig.ToString()}";
                        //changes += $"{Environment.NewLine} {UtilityMethods.PublicInstancePropertiesEqual<List<AttributeMappingColumns>>(endpoint.ListAttributeMappingColumns, config.EndpointSystem?.ListAttributeMappingColumns, excludes)}";
                        DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, LogLevelType.Info, changes, null);
                    }
                    else
                    {
                        DomainApiClient<Dto.EndPoint>.GetByAction<object>(DomainApiAction.Endpoint.EndPoint.SaveConfiguration, null, config);

                        var mode = this.UIMode == UIMode.Edit ? "Endpoint Edited:" : "Endpoint Created:";

                        string str = $"{mode} Endpoint Name - {config.EndpointSystem.DisplayName} ";
                        var empty = "Empty;";
                        var keyValues = string.Join(";", config.ConfigurationDetails.Select(s =>
                        {
                            var value = string.IsNullOrWhiteSpace(s.Value) ? empty : s.Value;

                            return $"{s.EndpointConfigurationKey.KeyName}:{value}";
                        }));

                        var strConfig = $"{mode} Endpoint Configuration - {keyValues} ";

                        DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, LogLevelType.Info, str, null);

                        DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, LogLevelType.Info, strConfig, null);
                    }

                    


                }
            }
            catch (Exception ex)

            {
                base.ShowAlertMessage(AlertType.Error, "Error Occured while Saving Endpoint");
            }
            if (ViewState["GoBackTo"] != null)
            {
                Response.Redirect(ViewState["GoBackTo"].ToString());
            }
            else
                Response.Redirect("~/Admin//Endpoint/List.aspx");
        }
        protected void Wizard_CancelButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            _sessionStatus = new SessionStatusImpl();
            if (_sessionStatus.RedirectFrom == "AlertCenter")
            {
                Response.Redirect("~/AlertCenter/List.aspx");
            }
            else if (ViewState["GoBackTo"] != null)
            {
                Response.Redirect(ViewState["GoBackTo"].ToString());
            }
            else
                Response.Redirect("~/Admin/Endpoint/List.aspx");

            _sessionStatus.RedirectFrom = "";
        }
        protected void Navigation(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            EndPoint ep = EndpointInfoUC.EndpointSystem;
            int EndPointSystemType = (int)ep.EndpointSystemTypeId;
            if (ep.OwnerUserId == 0)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select the Owner field");
                e.CurrentStep.Active = true;
            }

            Wizard.Attributes.Add("OnClientButtonClicking", "OnClientButtonClicking");
        }
        #endregion
    }
}