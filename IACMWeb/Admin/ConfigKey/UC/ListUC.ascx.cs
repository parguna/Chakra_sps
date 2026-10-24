using System;
using Application;
using DI.Chakra.Dto;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;

namespace DI.Chakra.Web.ConfigKey.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        #region Members
        private const string V_PRODUCT = "V_PRODUCT";
        private const string V_CONFIG = "V_CONFIG";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private SessionStatusImpl Session = new SessionStatusImpl();
        private static List<Dto.IConfigurationMaster> list;
        #endregion

        #region Properties
        private UIMode UIMode
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
        private Dto.IConfigurationMaster Configuration
        {
            get
            {
                if (ViewState[V_CONFIG] == null)
                    if (Product == Product.IACM)
                        return new ConfigurationMaster();
                    else if (Product == Product.IM)
                        return new ConfigurationMasterIM();
                    else if (Product == Product.PWDManagement)
                        return new ConfigurationMasterPasswordManagement();
                    else if (Product == Product.Compliance)
                        return new ConfigurationMasterCompliance();
                    else if (Product == Product.PIM)
                        return new ConfigurationMasterPIM();
                    else
                        return new ConfigurationMasterDGM();;
                return (Dto.IConfigurationMaster)ViewState[V_CONFIG];
            }
            set
            {
                ViewState[V_CONFIG] = value;
            }
        }
        public Product Product
        {
            get
            {
                if (ViewState[V_PRODUCT] == null)
                    return Product.IACM;
                return (Product)ViewState[V_PRODUCT];
            }
            set
            {
                ViewState[V_PRODUCT] = value;
            }
        }

        #endregion

        #region Methods
        private bool DoesExists(string ConfigName)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("configKey", ConfigName);
            parameter.Add("product", Convert.ToString(Product));
            return DomainApiClient<Dto.ConfigurationMaster>.GetByAction<bool>
                (DomainApiAction.Configuration.ConfigurationMaster.DoesExists, parameter);
        }
        
        private void ClearModalPopupValuesToDefault()
        {
            modalPopup.VisibleOnPageLoad = false;
            ConfigDetailUC.ConfigKey = string.Empty;
            ConfigDetailUC.ConfigValue = string.Empty;
            ConfigDetailUC.Description = string.Empty;
            ConfigDetailUC.UpdateButton = "Add";
            ConfigDetailUC.Header = "Add Configuration";
        }
        private void Delete()
        {
            if (!ContextObjectId.HasValue || ContextObjectId == 0)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            var configuration = DomainApiClient<Dto.ConfigurationMaster>.GetById(Convert.ToInt64(ContextObjectId));
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", Convert.ToString(ContextObjectId));
            parameter.Add("product", Convert.ToString(Product));
            DomainApiClient<Dto.ConfigurationMaster>.GetByAction<object>
            (DomainApiAction.Configuration.ConfigurationMaster.DeleteConfiguration, parameter);
            
            var log = $"{Session?.User?.UserName} deleted the configuration with Key:{configuration?.ConfigKey} and Value: {configuration?.ConfigValue}";
            DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, log);
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "Configuration deleted successfully");
        }
        private void SetPage()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", Convert.ToString(ContextObjectId));
            parameter.Add("product", Convert.ToString(Product));
            Configuration = DomainApiClient<Dto.ConfigurationMaster>.GetByAction<IConfigurationMaster>
                (DomainApiAction.Configuration.ConfigurationMaster.GetByProduct, parameter);

            if (null == Configuration)
                return;
            ConfigDetailUC.ConfigKey = Configuration.ConfigKey;
            ConfigDetailUC.ConfigValue = Configuration.ConfigValue;
            ConfigDetailUC.Description = Configuration.Description;
            var log = $"{Session.User.UserName} edited the configuration with Key:{Configuration.ConfigKey} and Value: {Configuration.ConfigValue}";
            DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, log);
        }
        private bool Save()
        {
            string ConfigKey = ConfigDetailUC.ConfigKey.Trim();
            string ConfigValue = ConfigDetailUC.ConfigValue.Trim();
            string Description = ConfigDetailUC.Description.Trim();
            if (!string.IsNullOrEmpty(ConfigKey) && !string.IsNullOrEmpty(ConfigValue))
            {
                if ((UIMode == UIMode.Add) && DoesExists(ConfigKey))
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Configuration Already Exists");
                    return false;
                }
                Configuration = Configuration;
                Configuration.ConfigKey = ConfigKey;
                Configuration.ConfigValue = ConfigValue;
                Configuration.Description = Description;
                if (UIMode == UIMode.Add)
                {
                    Configuration.Id = 0;
                }
                
                Configuration.IsUiDisplay = true;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("product", Convert.ToString(Product));
                DomainApiClient<Dto.ConfigurationMaster>.GetByAction<object>
                (DomainApiAction.Configuration.ConfigurationMaster.SaveConfiguration, parameter, Configuration);
                var usr = new SessionStatusImpl().User;
                var log = $"{Session.User.UserName} updated the configuration with Key:{ConfigKey} and Value: {ConfigValue}";
                DILog.UILogger.WriteLog(usr.Id,usr.DisplayName, LogLevelType.Info, log,null);
                ClearModalPopupValuesToDefault();
                return true;
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Error, "Config Key and Value should not be empty");
                return false;
            }

        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        protected void ConfigurationMaster_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("product", Convert.ToString(Product));
             list = DomainApiClient<Dto.ConfigurationMaster>.GetByAction<List<Dto.IConfigurationMaster>>(DomainApiAction.Configuration.ConfigurationMaster.GetConfigurationWithUIDisplayByProduct, parameter);
            ConfigurationMaster.DataSource = list;
        }

        protected void ConfigurationMaster_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                TableCell cell = (TableCell)item["ConfigValue"];
                Dto.IConfigurationMaster cMaster = (Dto.IConfigurationMaster)item.DataItem;
                cell.ToolTip = cMaster.ConfigValue;
            }
        }

        protected void ConfigurationMaster_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = ConfigurationMaster.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)(g.GetDataKeyValue("Id"));
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {                
                if (Save())
                {
                    ConfigurationMaster.Rebind();                    
                    ClearModalPopupValuesToDefault();
                    if (UIMode == UIMode.Edit)
                    {
                        
                        MasterPage.ShowAlert(AlertType.Success, "Configuration updated successfully");
                    }
                    else
                        MasterPage.ShowAlert(AlertType.Success, "Configuration added successfully");
                }
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, "Configuration not added successfully. Error Occurred");
                throw;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearModalPopupValuesToDefault();
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            var usr = new SessionStatusImpl().User;
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, LogLevelType.Info, "IDMS Config Key - Delete",null);
                Delete();
                ConfigurationMaster.Rebind();
            }
            else if (e.ActionType == ACLAction.Add)
            {
                DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, LogLevelType.Info, "IDMS Config Key - Add",null);
                UIMode = UIMode.Add;
                ConfigDetailUC.DisableConfigKey = true;
                ConfigurationMaster.SelectedIndexes.Clear();
                modalPopup.VisibleOnPageLoad = true;
            }
            else if (e.ActionType == ACLAction.Edit)
            {
                ConfigDetailUC.UpdateButton = "Update";
                ConfigDetailUC.Header = "Edit Configuration";
                ConfigDetailUC.DisableConfigKey = false;
                UIMode = UIMode.Edit;

                if (!ContextObjectId.HasValue)
                {
                    ClearModalPopupValuesToDefault();
                    UIMode = UIMode.Add;
                    MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                    return;
                }
                DILog.UILogger.WriteLog(usr.Id, usr.DisplayName, LogLevelType.Info, "IDMS Config Key - Edit",null);
                SetPage();
                modalPopup.VisibleOnPageLoad = true;
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                if (ConfigurationMaster.Items.Count == 0)
                {
                    MasterPage.ShowAlert(AlertType.Error, "No item found to export.");
                    return;
                }
                else
                {
                    ConfigurationMaster.ExportSettings.FileName = Page.Title;
                    ConfigurationMaster.Export(ConfigurationMaster, e.ActionType);
                }
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                ConfigurationMaster.Export<List<Dto.IConfigurationMaster>>(list, e.ActionType, Page.Title);
            }
        }
        #endregion
    }
}