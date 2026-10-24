///<summary>
/// IACM v3.5 
/// 
/// Author        : I.Deepthi
/// Created Date  : 18/03/2011
/// Modified Date :
/// Description   :check  Version
///</summary>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Model;
using System.Data;
using System.Data.SqlClient;

using DeepIdentity.Infrastructure;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;

namespace DeepIdentity.DataAccess
{
    public class DiConfigurationDA// : IDiConfigurationDA
    {
        QueryExecutor executor;
        IACMDbOperator iacmOperator;

        private IACMDBContext oIACMDBContext = new IACMDBContext();
        
        #region Mail Templete

        public void AddMailTemplate(MailTemplate mailTemplate)
        {
            try
            {
                using (IACMDBContext Context = new IACMDBContext())
                {
                    if (mailTemplate.TemplateId == 0)
                    {
                        Context.MailTemplate.Attach(mailTemplate);
                        Context.Entry(mailTemplate).State = EntityState.Added;
                        Context.SaveChanges();
                    }
                    else
                    {
                        MailTemplate editMailTemplate = Context.MailTemplate.Where(Temp => Temp.TemplateId == mailTemplate.TemplateId).First();
                        if (editMailTemplate == null)
                            return;
                        Context.Entry(editMailTemplate).CurrentValues.SetValues(mailTemplate);
                        Context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Inserting the MailTemplete", ex);
                throw ex;
            }
        }

        public List<MailTemplate> GetMailTemplate()
        {
            try
            {
                using (IACMDBContext Context = new IACMDBContext())
                {
                    return Context.MailTemplate.ToList();
                }
                
            }

            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Fetching Get MailTemplete", ex);
                throw ex;
            }
        }

        public void DeleteMailTemplate(int TemplateId)
        {
            try
            {
                using (IACMDBContext Context = new IACMDBContext())
                {
                    MailTemplate delMailTemplate = oIACMDBContext.MailTemplate.Where(mt => mt.TemplateId == TemplateId).First();
                    if (delMailTemplate != null)
                    {
                        Context.MailTemplate.Attach(delMailTemplate);
                        Context.MailTemplate.Remove(delMailTemplate);
                        Context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Deleting  the Tenant", ex);
                throw ex;
            }
        }

        #endregion

        #region MailServerConfiguration

        public void SaveMailServerConfiguration(MailServerConfiguration mailServerConfig)
        {
            try
            {
                using (IACMDBContext Context = new IACMDBContext())
                {                    
                    if (mailServerConfig.MailServerId == 0)
                    {
                        Context.MailServerConfiguration.Attach(mailServerConfig);
                        Context.Entry(mailServerConfig).State = EntityState.Added;
                        Context.SaveChanges();
                    }
                    else
                    {
                        MailServerConfiguration editMail = Context.MailServerConfiguration.Where(ms => ms.MailServerId == mailServerConfig.MailServerId).First();
                        if (editMail == null)
                            return;
                        Context.Entry(editMail).CurrentValues.SetValues(mailServerConfig);
                        Context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Inserting the MailServerConfiguration", ex);
                throw ex;
            }
        }

        public List<MailServerConfiguration> GetMailServerConfiguration()
        {
            List<MailServerConfiguration> mailServerConfiglist = new List<MailServerConfiguration>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    mailServerConfiglist = oIACMDBContext.MailServerConfiguration.ToList();
                }
                return mailServerConfiglist;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Fetching Mail Configuration", ex);
                throw ex;
            }
        }

        public void DeleteMailServerConfig(int MailServerId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    MailServerConfiguration delMailServerConfig = oIACMDBContext.MailServerConfiguration.Where(msConfig => msConfig.MailServerId == MailServerId).First();
                    if (delMailServerConfig != null)
                    {
                        oIACMDBContext.MailServerConfiguration.Remove(delMailServerConfig);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Deleting the Mail Server Configuration", ex); //"Error Occured while Deleting  the Tenant", ex);
                throw ex;
            }
        }

        #endregion

       

        public void AddConfigTrustedSource(List<ConfigGeneralTrustedSource> configGeneralTrustedSource)
        {
            try
            {
                int noofAffeected = 0;
                DeleteConfigTrustedSource();


                foreach (ConfigGeneralTrustedSource item in configGeneralTrustedSource)
                {
                    item.CreatedDate = DateTime.Now;
                    item.LastModifiedDate = DateTime.Now;
                    oIACMDBContext.TrustedSources.Add(item);
                }
                oIACMDBContext.SaveChanges();
                IList<ConfigGeneralTrustedSource> TSList = oIACMDBContext.TrustedSources.ToList(); noofAffeected = TSList.Count();
                if (noofAffeected == configGeneralTrustedSource.Count())
                { }
                else
                {
                    throw new Exception("ConfigTrustedSource Details are not Inserted");
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Inserting the Config Trusted Source", ex);
                throw ex;
            }
        }
        public void AddConfigTrustedSource(ConfigGeneralTrustedSource configGeneralTrustedSource)
        {
            try
            {


                 using (IACMDBContext oIACMDBContext = new IACMDBContext())
                 {
                     configGeneralTrustedSource.CreatedDate = DateTime.Now;
                     configGeneralTrustedSource.LastModifiedDate = DateTime.Now;
                     oIACMDBContext.TrustedSources.Add(configGeneralTrustedSource);
                    oIACMDBContext.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Inserting the Config Trusted Source", ex);
                throw ex;
            }
        }
        public void DeleteConfigTrustedSource()
        {
            try
            {

                IList<ConfigGeneralTrustedSource> listTrustedSrcs = oIACMDBContext.TrustedSources.ToList();
                if (listTrustedSrcs.Count() > 0)
                {
                    foreach (ConfigGeneralTrustedSource TSItem in listTrustedSrcs)
                    {
                        oIACMDBContext.TrustedSources.Remove(TSItem);
                    }
                    oIACMDBContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Deleting  the DiConfigColumns", ex);
                throw ex;
            }
        }

        public IList<ConfigGeneralTrustedSource> GetAllConfigTrustedSource()
        {
            List<ConfigGeneralTrustedSource> list = new List<ConfigGeneralTrustedSource>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                    list = oIACMDBContext.TrustedSources.ToList();
                return list;
            }

            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Fetching Get ColumnsList By ModuleName", ex);
                throw ex;
            }
        }


        public void DeleteConfigTrustedSourceById(int configId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    ConfigGeneralTrustedSource config = oIACMDBContext.TrustedSources.FirstOrDefault(uTrsSrc => uTrsSrc.Id == configId);
                    if (config != null)
                    {
                        oIACMDBContext.TrustedSources.Remove(config);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }













        public void DeleteDiConfigColumns(string ModuleName)
        {
            try
            {
                string query = "DI_spDeleteConfigColumns";
                int recordsAffected;

                List<SqlParameter> parameters = new List<SqlParameter>();

                SqlParameter sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@ModuleName";
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = 50;
                sqlParam.Value = ModuleName;
                parameters.Add(sqlParam);

                iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

                executor = new QueryExecutor(iacmOperator);

                recordsAffected = executor.ExecuteQuery();
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Deleting  the DiConfigColumns", ex);
                throw ex;
            }
            finally
            {
                executor.IACMDbOperator.Close();
            }
        }

        private class entityDetails
        {
            public string entityName;
            public string tableName;
        }

        private class entityAttributeDetails
        {
            public string attributeName;
            public string attributeType;
            public string fullName;
        }

      


      

        public IList GetWorkflowColumnsList()
        {
            string query = "DI_spGetWorkflowColumnsList";
            try
            {

                iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, null);

                executor = new QueryExecutor(iacmOperator);

                DataSet ds = executor.ExecuteQueryDS();

                if (null == ds)
                    return null;

                if (0 == ds.Tables.Count)
                    return null;

                DataTable dt = ds.Tables[0];

                var list = (from myRow in dt.AsEnumerable()
                            select new
                            {
                                ColumnName =
                                    myRow.Field<string>("COLUMNNAME"),
                                FullName =
                                    myRow.Field<string>("FULLNAME")

                            }).ToList();


                return list;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while GetEndPointColumnsList", ex);
                throw ex;
            }
            finally
            {
                executor.IACMDbOperator.Close();
            }

        }

        public void TruncateHistoryTables()
        {
            //string query = "DI_spCleanHistory";

            try
            {
                //iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, null);
                //executor = new QueryExecutor(iacmOperator);
                //executor.ExecuteQuery();

                using (IACMDBContext _context = new IACMDBContext())
                {
                    //_context.AttestationReviewADSL2Histories.re
                    //_context.Database.ExecuteSqlCommand("TRUNCATE TABLE DI_ADS_AttestationReview_L2History");
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ShrinkDatabase(string dbName)
        {

            try
            {

                //string query = string.Empty;
                //List<SqlParameter> parameters = new List<SqlParameter>();
                //SqlParameter sqlParam = new SqlParameter();

                //query = "DI_spShrinkDBLog";

                //sqlParam = new SqlParameter();
                //sqlParam.ParameterName = "@DatabaseName";
                //sqlParam.SqlDbType = SqlDbType.NVarChar;
                //sqlParam.Size = 50;
                //sqlParam.Value = dbName;
                //parameters.Add(sqlParam);

                //iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);
                //executor = new QueryExecutor(iacmOperator);
                //executor.ExecuteQuery();

                using (IACMDBContext _context = new IACMDBContext())
                {

                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Shrinking  the Database", ex);
                throw ex;
            }

        }

        public MailTemplate GetMailTemplate(string TemplateName)
        {
            //    string query = "DI_spGetMailTemplate";
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.MailTemplate.Where(temp => temp.TemplateName == TemplateName).FirstOrDefault();
                }
            }

            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Fetching Get MailTemplete", ex);
                throw ex;
            }
        }

        public List<AttributeMappingColumns> GetAttributeMappingColumns(int endpointSystemId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {

                    return oIACMDBContext.AttributeMappingColumnss.Where(item => item.EndPoint.EndpointSystemId == endpointSystemId).ToList();
                }
            }
            catch
            {
                throw;
            }
        }


        public List<ConfigurationMaster> GetConfigurationMaster()
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {

                    return oIACMDBContext.ConfigurationMasters.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public ConfigurationMaster GetConfigurationMasterById(int configId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.ConfigurationMasters.FirstOrDefault(uconfmaster => uconfmaster.ConfigId == configId);
                }
            }
            catch
            {
                throw;
            }
        }
        public void DeleteConfigurationMasterById(int configId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    ConfigurationMaster config = oIACMDBContext.ConfigurationMasters.FirstOrDefault(uconfmaster => uconfmaster.ConfigId == configId);
                    if (config != null)
                    {
                        oIACMDBContext.ConfigurationMasters.Remove(config);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public ConfigurationMaster GetConfigurationMasterByConfigKey(string ConfigKey)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.ConfigurationMasters.FirstOrDefault(uconfmaster => uconfmaster.ConfigKey.ToUpper() == ConfigKey.ToUpper());
                }
            }
            catch
            {
                throw;
            }
        }

        public bool IsConfigAlreadyExists(string ConfigKey)
        {
            bool isExists = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    ConfigurationMaster config =  oIACMDBContext.ConfigurationMasters.FirstOrDefault(uconfmaster => uconfmaster.ConfigKey == ConfigKey);
                    if (config != null)
                    {
                        isExists = true;
                    }
                }
            }
            catch
            {
                throw;
            }
            return isExists;
        }
        public int updateConfigurationMaster(ConfigurationMaster configurationmaster)
        {
            ConfigurationMaster uconfigmaster;
            int retValue = 0;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var taskEntry = _context.Entry(configurationmaster);
                    if (taskEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<ConfigurationMaster>();
                        ConfigurationMaster attachedTask = set.Find(configurationmaster.ConfigId);

                        if (attachedTask != null)
                        {
                            var attachedEntry = _context.Entry(attachedTask);
                            attachedEntry.CurrentValues.SetValues(configurationmaster);
                        }
                    }
                    _context.SaveChanges();
                    retValue = -1;
                }

                return retValue;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int SaveConfigurationMaster(ConfigurationMaster configurationmaster)
        {
            int retValue = -1;
            try
            {

                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (configurationmaster.ConfigId == 0)
                    {
                        oIACMDBContext.ConfigurationMasters.Attach(configurationmaster);
                        oIACMDBContext.Entry(configurationmaster).State = EntityState.Added;
                        oIACMDBContext.SaveChanges();
                    }
                    else
                    {
                        ConfigurationMaster config = oIACMDBContext.ConfigurationMasters.Where(qns => qns.ConfigId == configurationmaster.ConfigId).First();
                        oIACMDBContext.Entry(config).CurrentValues.SetValues(configurationmaster);
                        oIACMDBContext.SaveChanges();
                    }
                }
                
                return configurationmaster.ConfigId;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
