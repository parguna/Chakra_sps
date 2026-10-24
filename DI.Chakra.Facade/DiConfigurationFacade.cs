using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DI.Chakra.Model;
using DI.Chakra.Infrastructure;

using System.Collections;
using DI.Chakra.DataAccess;

using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Facade
{
    public class DiConfigurationFacade// : DiConfigurationFacade
    {
        public DiConfigurationDA DIConfigurationDA { get; set; }

        public DiConfigurationFacade()
        {
            DIConfigurationDA = new DiConfigurationDA();
        }

        #region DiConfigurationFacade Members

        
        //public void AddMailTemplate(MailTemplate mailTemplate)
        //{
        //    try
        //    {
        //        DIConfigurationDA.AddMailTemplate(mailTemplate);

        //    }
        //    catch (DomainException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
        //public List<MailTemplate> GetMailTemplate()
        //{
        //    try
        //    {
        //        return DIConfigurationDA.GetMailTemplate();

        //    }
        //    catch (DomainException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
        //public MailTemplate GetMailTemplateById(int TemplateId)
        //{
        //    try
        //    {
        //        return DIConfigurationDA.GetMailTemplate().Where(t => t.Id == TemplateId).FirstOrDefault();
        //    }
        //    catch (DomainException)
        //    {
        //        throw;
        //    }
        //}
        //public void DeleteMailTemplate(int TemplateId)
        //{
        //    try
        //    {
        //        DIConfigurationDA.DeleteMailTemplate(TemplateId);

        //    }
        //    catch (DomainException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //public void AddConfigTrustedSource(List<ConfigGeneralTrustedSource> configGeneralTrustedSource)
        //{
        //    try
        //    {
        //        DIConfigurationDA.AddConfigTrustedSource(configGeneralTrustedSource);

        //    }
        //    catch (DomainException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
        //public void AddConfigTrustedSource(ConfigGeneralTrustedSource configGeneralTrustedSource)
        //{
        //    try
        //    {
        //        DIConfigurationDA.AddConfigTrustedSource(configGeneralTrustedSource);

        //    }
        //    catch (DomainException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
        //public List<MailServerConfiguration> GetMailServerConfiguration()
        //{
        //    try
        //    {
        //        return DIConfigurationDA.GetMailServerConfiguration();

        //    }
        //    catch (DomainException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
        //public MailServerConfiguration GetMailServerConfigurationById(int MailServerId)
        //{
        //    try
        //    {
        //        return DIConfigurationDA.GetMailServerConfiguration().Where(ms => ms.Id == MailServerId).FirstOrDefault();

        //    }
        //    catch (DomainException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //public void DeleteMailServerConfig(int MailServerId)
        //{
        //    try
        //    {
        //        DIConfigurationDA.DeleteMailServerConfig(MailServerId);

        //    }
        //    catch (DomainException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //public IList<ConfigGeneralTrustedSource> GetAllConfigTrustedSource()
        //{
        //    try
        //    {
        //        return DIConfigurationDA.GetAllConfigTrustedSource();

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //public void DeleteConfigTrustedSourceById(int configid)
        //{
        //    try
        //    {
        //        DIConfigurationDA.DeleteConfigTrustedSourceById(configid);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}


        //#region MailServerConfiguration

        //public void SaveMailServerConfiguration(MailServerConfiguration mailServerConfiguration)
        //{
        //    try
        //    {
        //        DIConfigurationDA.SaveMailServerConfiguration(mailServerConfiguration);

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        //#endregion


        # region WORKFLOW


        public IList GetWorkflowColumnsList()
        {
            try
            {
                return DIConfigurationDA.GetWorkflowColumnsList();

            }
            catch (Exception)
            {
                
                throw;
            }

        }
        #endregion

        public void TruncateHistoryTables()
        {
            try
            {
                DIConfigurationDA.TruncateHistoryTables();
            }
            catch (Exception)
            {
                
                throw;
            }

        }
        public void ShrinkDatabase(string dbName)
        {
            try
            {
                DIConfigurationDA.ShrinkDatabase(dbName);
            }
            catch (Exception)
            {
                
                throw;
            }
        }




        #endregion


        //public MailTemplate GetMailTemplate(string templatename)
        //{
        //    try
        //    {
        //        return DIConfigurationDA.GetMailTemplate(templatename);
        //    }
        //    catch (DomainException)
        //    {

        //        throw;
        //    }

        //}

        //Passsword Sync Config

        public void CreatePasswordSyncConfiguration(PasswordSyncConfiguration pwdSyncConfig)
        {
            try
            {
                PasswordSyncConfigurationDA pwdSyncConfigDA = new PasswordSyncConfigurationDA();
                pwdSyncConfigDA.CreatePasswordSyncConfiguration(pwdSyncConfig);
            }
            catch
            {
                throw;
            }
        }

        public void UpdatePasswordSyncConfiguration(PasswordSyncConfiguration pwdSyncConfig)
        {
            try
            {
                PasswordSyncConfigurationDA pwdSyncConfigDA = new PasswordSyncConfigurationDA();
                pwdSyncConfigDA.UpdatePasswordSyncConfiguration(pwdSyncConfig);

                foreach (PasswordSyncConfigurationDetail configDetail in pwdSyncConfig.PasswordSyncConfigurationDetails)
                {
                    if (configDetail.Id == 0)
                    {
                        configDetail.ConfigId = pwdSyncConfig.Id;
                        pwdSyncConfigDA.CreatePasswordSyncConfigurationDetail(configDetail);
                    }
                    else
                    {
                        pwdSyncConfigDA.UpdatePasswordSyncConfigurationDetail(configDetail);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public PasswordSyncConfiguration GetPasswordSyncConfiguration(bool deepLoad)
        {
            try
            {
                PasswordSyncConfigurationDA pwdSyncConfigDA = new PasswordSyncConfigurationDA();
                return pwdSyncConfigDA.GetPasswordSyncConfiguration(deepLoad);
            }
            catch
            {
                throw;
            }
        }

        //public List<PasswordSyncConfigGridData> GetPasswordSyncConfigDetails()
        //{
        //    try
        //    {
        //        PasswordSyncConfigurationDA pwdSyncConfigDA = new PasswordSyncConfigurationDA();
        //        return pwdSyncConfigDA.GetPasswordSyncConfigDetails();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public List<AttributeMappingColumns> GetAttributeMappingColumns(long endpointSystemId)
        {
            try
            {
                return DIConfigurationDA.GetAttributeMappingColumns(endpointSystemId);
            }
            catch
            {
                throw;
            }
        }

        //public List<ConfigurationMaster> GetConfigurationMaster()
        //{
        //    try
        //    {
        //        return DIConfigurationDA.GetConfigurationMaster();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        //public ConfigurationMaster GetConfigurationMasterById(int configId)
        //{
        //    try
        //    {
        //        return DIConfigurationDA.GetConfigurationMasterById(configId);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        //public void DeleteConfigurationMasterById(int configid)
        //{
        //    try
        //    {
        //        DIConfigurationDA.DeleteConfigurationMasterById(configid);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        //public bool IsConfigAlreadyExists(string ConfigKey)
        //{
        //    return DIConfigurationDA.IsConfigAlreadyExists(ConfigKey);
        //}
        //public ConfigurationMaster GetConfigurationMasterByConfigKey(string ConfigKey)
        //{
        //    try
        //    {
        //        return DIConfigurationDA.GetConfigurationMasterByConfigKey(ConfigKey);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public int updateConfigurationMaster(ConfigurationMaster configurationmaster)
        //{
        //    try
        //    {
        //        return DIConfigurationDA.updateConfigurationMaster(configurationmaster);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        //public long SaveConfigurationMaster(ConfigurationMaster configurationmaster)
        //{
        //    try
        //    {
        //        return DIConfigurationDA.SaveConfigurationMaster(configurationmaster);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
    }
}
