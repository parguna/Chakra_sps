using System.Data.SqlClient;
using System.Data;
using DI.Chakra.Infrastructure;
using System.Linq;
using DI.Chakra.Logger;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums; 

namespace DI.Chakra.Services.EmailingProcessor
{
    public class DBHelperEmail
    {
        #region Members and Constructor
        private SqlCommand m_Cmd;
        private static DBHelperEmail m_dbHelper;

        private DBHelperEmail()
        {
            m_Cmd = this.getSqlCommand();
        }
        public static DBHelperEmail CreateDBHelper()
        {
            m_dbHelper = m_dbHelper ?? new DBHelperEmail();
            return m_dbHelper;
        }
        #endregion

        #region Private Helper Methods
        private SqlCommand getSqlCommand()
        {
            string m_ConStr = string.Empty;
            SqlConnection crnt_Conn = null;
            SqlCommand crnt_Cmd = null;

            string connectionType = ConfigurationManager.AppSettings["Connection"];
            if (connectionType == "Development")
            {
                m_ConStr = ConfigurationManager.ConnectionStrings["Development"].ToString();
            }
            else if (connectionType == "DIServer")
            {
                m_ConStr = ConfigurationManager.ConnectionStrings["DIServer"].ToString();
            }
            else if (connectionType == "Integration")
            {
                m_ConStr = ConfigurationManager.ConnectionStrings["Integration"].ToString();
            }
            else if (connectionType == "Local")
            {
                m_ConStr = ConfigurationManager.ConnectionStrings["Local"].ToString();
            }
            else if (connectionType == "Production")
            {
                m_ConStr = ConfigurationManager.ConnectionStrings["Production"].ToString();
            }
            else if (connectionType == "UAT")
            {
                m_ConStr = ConfigurationManager.ConnectionStrings["UAT"].ToString();
            }

            if (!string.IsNullOrEmpty(m_ConStr))
            {
                crnt_Conn = new SqlConnection(m_ConStr);
                crnt_Conn.Open();

                crnt_Cmd = new SqlCommand();
                crnt_Cmd.CommandType = System.Data.CommandType.Text;
                crnt_Cmd.Connection = crnt_Conn;
            }
            else
            {
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, "Connection string is null in Log initialization.",0, "Connection string is null in Log initialization.");
            }


            return crnt_Cmd;
        }
        #endregion

        public List<EmailInfo> GetEmailInfo(EmailActivityType emailActivityType) 
        {
            List<EmailInfo> objEmailInfo = new List<EmailInfo>();
            IDataReader reader = null;
            long emailId = 0;
            try
            {
                if (m_Cmd.Connection.State != System.Data.ConnectionState.Open)
                {
                    m_Cmd.Connection.Open();
                }

                if (emailActivityType == EmailActivityType.OTP)
                { m_Cmd.CommandText = QueryHelperEmail.GetOTPEmailInfo_UIQuery; }
                else
                { m_Cmd.CommandText = QueryHelperEmail.GetAllEmailInfo_UIQuery; }

               

                reader = m_Cmd.ExecuteReader();
                EmailInfo objEmail;

                while (reader.Read())
                {
                    objEmail = new DomainModel.EmailInfo();
                    objEmail.Id = CheckDBNull.AgainstInt(reader["Id"]);
                    emailId = CheckDBNull.AgainstInt(reader["Id"]);
                    objEmail.ActivityType = CheckDBNull.AgainstString(reader["ActivityType"]);
                    objEmail.Priority = CheckDBNull.AgainstString(reader["Priority"]);
                    objEmail.From = CheckDBNull.AgainstString(reader["From"]);
                    objEmail.To = CheckDBNull.AgainstString(reader["To"]);
                    objEmail.Cc = CheckDBNull.AgainstString(reader["Cc"]);
                    objEmail.Bcc = CheckDBNull.AgainstString(reader["Bcc"]);
                    objEmail.Subject = CheckDBNull.AgainstString(reader["Subject"]);
                    objEmail.Body = CheckDBNull.AgainstString(reader["Body"]);
                    objEmail.IsHtml = CheckDBNull.AgainstBit(reader["IsHtml"]);
                    objEmail.Attachment = CheckDBNull.AgainstString(reader["Attachment"]);
                    objEmail.Status = CheckDBNull.AgainstString(reader["Status"]);
                    objEmail.CreatedDateTime = CheckDBNull.AgainstDatetime(reader["CreatedDateTime"]);
                    objEmail.SentDateTime = CheckDBNull.AgainstDatetime(reader["SentDateTime"]);
                    objEmail.CreatedBy = CheckDBNull.AgainstString(reader["CreatedBy"]);
                    objEmail.CreatedDate = CheckDBNull.AgainstDatetime(reader["CreatedDate"]);
                    objEmail.LastModifiedBy= CheckDBNull.AgainstString(reader["LastModifiedBy"]);
                    objEmail.LastModifiedDate = CheckDBNull.AgainstDatetime(reader["LastModifiedDate"]);
                    objEmailInfo.Add(objEmail);
                }

            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, "Exception Thrown in GetEmailInfo()", ex,emailId,"");

            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
            return objEmailInfo;
        }

        internal List<EmailInfo> GetEmailInfo(string Status)
        {
            List<EmailInfo> objEmailInfo = new List<EmailInfo>();
            IDataReader reader = null;
            long emailid = 0;
            try
            {
                if (m_Cmd.Connection.State != System.Data.ConnectionState.Open)
                {
                    m_Cmd.Connection.Open();
                }

                m_Cmd.CommandText = QueryHelperEmail.GetAllEmailInfoByStatus_UIQuery;
                m_Cmd.Parameters.Clear();

                SqlParameter sqlParam;
                m_Cmd.Parameters.Clear();
                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Status";
                sqlParam.SqlDbType = SqlDbType.VarChar;
                sqlParam.Size = 50;
                sqlParam.Value = Status;
                m_Cmd.Parameters.Add(sqlParam);

                reader = m_Cmd.ExecuteReader();
                EmailInfo objEmail;

                while (reader.Read())
                {
                    objEmail = new EmailInfo();
                    objEmail.Id = CheckDBNull.AgainstInt(reader["Id"]);
                    emailid = objEmail.Id;
                    objEmail.ActivityType = CheckDBNull.AgainstString(reader["ActivityType"]);
                    objEmail.Priority = CheckDBNull.AgainstString(reader["Priority"]);
                    objEmail.From = CheckDBNull.AgainstString(reader["From"]);
                    objEmail.To = CheckDBNull.AgainstString(reader["To"]);
                    objEmail.Cc = CheckDBNull.AgainstString(reader["Cc"]);
                    objEmail.Bcc = CheckDBNull.AgainstString(reader["Bcc"]);
                    objEmail.Subject = CheckDBNull.AgainstString(reader["Subject"]);
                    objEmail.Body = CheckDBNull.AgainstString(reader["Body"]);
                    objEmail.IsHtml = CheckDBNull.AgainstBit(reader["IsHtml"]);
                    objEmail.Attachment = CheckDBNull.AgainstString(reader["Attachment"]);
                    objEmail.Status = CheckDBNull.AgainstString(reader["Status"]);
                    objEmail.CreatedDateTime = CheckDBNull.AgainstDatetime(reader["CreatedDateTime"]);
                    objEmail.SentDateTime = CheckDBNull.AgainstDatetime(reader["SentDateTime"]);
                    objEmailInfo.Add(objEmail);
                }

            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null, LogLevelType.Exception, "Error at method getEmailInfo, Error : {0}", ex, emailid, "");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
            return objEmailInfo;
        }
        public List<EmailInfoHistory> GetEmailInfo(string ActivityType, string Status)
        {
            List<EmailInfoHistory> objEmailInfo = new List<EmailInfoHistory>();
            try
            {
                objEmailInfo = EmailInfoHistory.GetByActivityType(ActivityType, Status);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null, LogLevelType.Exception, "Exception Thrown in List<EmailInfoHistory> GetEmailInfo(string ActivityType, string Status)", ex, 0,string.Empty);
            }
            return objEmailInfo;
        }

        public List<DomainModel.Presentation.PresentationMail> GetEmailInfoMain(string ActivityType, string Status)
        {
            try
            {
                List<DomainModel.Presentation.PresentationMail> emailInfoList = new List<DomainModel.Presentation.PresentationMail>();
                emailInfoList = DomainModel.Presentation.PresentationMail.GetByActivityType(ActivityType, Status).ToList();           
                return emailInfoList;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, string.Empty, ex,0, string.Empty);
                return null;
            }
        }
        
        public EmailInfo GetEmailInfoById(int MailId)
        {
            try
            {
              return EmailInfo.GetById(MailId);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, string.Empty, ex, MailId, string.Empty);
                return null;
            }
        }

        public List<EmailHistory> GetEmailInfoHistory(DateTime sentDateTime)
        {
            List<EmailHistory> objEmailInfoHistory = new List<EmailHistory>();
            IDataReader reader = null;
            long emailId = 0;
            try
            {
                if (m_Cmd.Connection.State != System.Data.ConnectionState.Open)
                {
                    m_Cmd.Connection.Open();
                }

                m_Cmd.CommandText = QueryHelperEmail.GetAllEmailInfoHistory_UIQuery;
                m_Cmd.Parameters.Clear();

                SqlParameter sqlParam;
                m_Cmd.Parameters.Clear();
                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@SentDateTime";
                sqlParam.SqlDbType = SqlDbType.DateTime;
                sqlParam.Value = sentDateTime;
                m_Cmd.Parameters.Add(sqlParam);

                reader = m_Cmd.ExecuteReader();
                EmailHistory objEmailHistory;

                while (reader.Read())
                {
                    objEmailHistory = new EmailHistory();
                    objEmailHistory.Id = CheckDBNull.AgainstInt(reader["Id"]);
                    emailId = objEmailHistory.Id;
                    objEmailHistory.HistoryId = CheckDBNull.AgainstInt(reader["Id"]);
                    objEmailHistory.ActivityType = CheckDBNull.AgainstString(reader["ActivityType"]);
                    objEmailHistory.Priority = CheckDBNull.AgainstString(reader["Priority"]);
                    objEmailHistory.From = CheckDBNull.AgainstString(reader["From"]);
                    objEmailHistory.To = CheckDBNull.AgainstString(reader["To"]);
                    objEmailHistory.Cc = CheckDBNull.AgainstString(reader["Cc"]);
                    objEmailHistory.Bcc = CheckDBNull.AgainstString(reader["Bcc"]);
                    objEmailHistory.Subject = CheckDBNull.AgainstString(reader["Subject"]);
                    objEmailHistory.Body = CheckDBNull.AgainstString(reader["Body"]);
                    objEmailHistory.IsHtml = CheckDBNull.AgainstBit(reader["IsHtml"]);
                    objEmailHistory.Attachment = CheckDBNull.AgainstString(reader["Attachment"]);
                    objEmailHistory.Status = CheckDBNull.AgainstString(reader["Status"]);
                    objEmailHistory.CreatedDateTime = CheckDBNull.AgainstDatetime(reader["CreatedDateTime"]);
                    objEmailHistory.SentDateTime = CheckDBNull.AgainstDatetime(reader["SentDateTime"]);
                    objEmailInfoHistory.Add(objEmailHistory);
                }

            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, "Exception thrown in List<EmailHistory> GetEmailInfoHistory(DateTime sentDateTime)", ex, emailId,string.Empty);

            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }

            return objEmailInfoHistory;
        }

        internal void SaveEmailtoDB(EmailInfo emailInfo)
        {
            try
            {
                emailInfo.ActivityType = emailInfo.ActivityType ?? string.Empty;
                emailInfo.Priority = emailInfo.Priority ?? string.Empty;
                emailInfo.From = GetUniqueEmailAddress(emailInfo.From) ?? string.Empty;
                emailInfo.To = GetUniqueEmailAddress(emailInfo.To) ?? string.Empty;
                emailInfo.Cc = GetUniqueEmailAddress(emailInfo.Cc) ?? string.Empty;
                emailInfo.Bcc = GetUniqueEmailAddress(emailInfo.Bcc) ?? string.Empty;
                emailInfo.Subject = emailInfo.Subject ?? string.Empty;
                emailInfo.Body = emailInfo.Body ?? string.Empty;
                emailInfo.Attachment = emailInfo.Attachment ?? string.Empty;
                emailInfo.Status = emailInfo.Status ?? string.Empty;
                emailInfo.CreatedDateTime = (emailInfo.CreatedDateTime == DateTime.MinValue) ? (DateTime?)null : emailInfo.CreatedDateTime; 
                emailInfo.SentDateTime = (emailInfo.SentDateTime == DateTime.MinValue) ? (DateTime?)null : emailInfo.SentDateTime;
                emailInfo.Save(DomainModel.MasterConfigHelper.ServiceUser);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, "internal void SaveEmailtoDB(EmailInfo emailInfo)", ex,null,string.Empty);

            }
        }
        internal string GetUniqueEmailAddress(string email)
        {
            return string.Join(",", UtilityMethods.GetListFromString(email).Distinct());
        }
        internal void UpdateEmailtoDB(EmailInfo emailInfo)
        {
            try
            {
                if (m_Cmd.Connection.State != System.Data.ConnectionState.Open)
                {
                    m_Cmd.Connection.Open();
                }
                if (emailInfo.Status == EmailStatus.SENT.ToString())
                    m_Cmd.CommandText = QueryHelperEmail.UpdateEmail_UIQuery;
                else
                    m_Cmd.CommandText = QueryHelperEmail.UpdateFailedEmail_UIQuery;

                #region Sql Parameters
                SqlParameter sqlParam;

                m_Cmd.Parameters.Clear();

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Id";
                sqlParam.SqlDbType = SqlDbType.Int;
                sqlParam.Value = emailInfo.Id;
                m_Cmd.Parameters.Add(sqlParam);

                sqlParam = new SqlParameter();
                sqlParam.ParameterName = "@Status";
                sqlParam.SqlDbType = SqlDbType.VarChar;
                sqlParam.Size = 50;
                sqlParam.Value = emailInfo.Status ?? string.Empty;
                m_Cmd.Parameters.Add(sqlParam);

                if (emailInfo.Status == EmailStatus.SENT.ToString())
                {
                    sqlParam = new SqlParameter();
                    sqlParam.ParameterName = "@SentDateTime";
                    sqlParam.SqlDbType = SqlDbType.DateTime;
                    sqlParam.Value = emailInfo.SentDateTime == DateTime.MinValue ? null : emailInfo.SentDateTime;
                    m_Cmd.Parameters.Add(sqlParam);
                }
                #endregion

                m_Cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, string.Empty, ex, emailInfo.Id,emailInfo.Subject);

            }
        }

        //public void UpdateEmailInfotoDB(EmailInfo emailInfo)
        //{
        //    try
        //    {
        //        emailInfo.Save();
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Saving the Mail", ex);
        //        throw ex;
        //    }
        //}
        internal void DeleteSentMails()
        {
            try
            {
                if (m_Cmd.Connection.State != System.Data.ConnectionState.Open)
                {
                    m_Cmd.Connection.Open();
                }
                m_Cmd.CommandText = QueryHelperEmail.DeleteSentEmail_UIQuery;
                m_Cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null, LogLevelType.Exception, "Exception Thrown from DeleteSentMails()", ex,null,string.Empty);
            }
        }

        public void DeleteEmailInfo(int ItemId,string Status)
        {
            EmailInfo delEmailInfo = null;
            try
            {
                if (Status.ToUpper() != "SENT")
                {
                      delEmailInfo =EmailInfo.GetById(ItemId);
                    if (delEmailInfo != null)
                    {
                        delEmailInfo.Delete();
                    }
                }
                else
                {
                    EmailInfoHistory histroy = EmailInfoHistory.GetById(ItemId);
                    if (histroy != null)
                    {
                        histroy.Delete();
                    }
                }  
            }
            catch (Exception ex)
            {
                long emailid = 0;
                if (delEmailInfo != null)
                    emailid = delEmailInfo.Id;
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, "Error at method deleteEmailInfo, Error", ex, emailid,string.Empty);
            }
        }

        internal List<MailServerConfigInfo> GetServerInfo()
        {
            List<MailServerConfigInfo> objServerInfo = new List<MailServerConfigInfo>();
            IDataReader reader = null;
            try
            {
                if (m_Cmd.Connection.State != System.Data.ConnectionState.Open)
                {
                    m_Cmd.Connection.Open();
                }

                m_Cmd.CommandText = QueryHelperEmail.GetAllServerInfo_UIQuery;

                reader = m_Cmd.ExecuteReader();
                MailServerConfigInfo objServer;

                while (reader.Read())
                {
                    objServer = new MailServerConfigInfo();
                    objServer.MailServerId = CheckDBNull.AgainstInt(reader["Id"]);
                    objServer.EmailID = CheckDBNull.AgainstString(reader["EmailID"]);
                    objServer.UserName = CheckDBNull.AgainstString(reader["UserName"]);
                    objServer.Password = CheckDBNull.AgainstString(reader["Password"]);
                    objServer.Password = RSAHelper.DecryptData(objServer.Password);
                    objServer.HostName = CheckDBNull.AgainstString(reader["HostName"]);
                    objServer.PortNumber = CheckDBNull.AgainstString(reader["PortNumber"]);
                    objServer.EnableSSL = CheckDBNull.AgainstBit(reader["EnableSSL"]);
                    objServerInfo.Add(objServer);
                }
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, "Exception Thrown in List<MailServerConfigInfo> GetServerInfo()", ex,null,string.Empty);

            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
            return objServerInfo;
        }

        internal void SqlBulkInsert(string TableName, IList<EmailInfo> objEmailInfo)
        {
            DataTable dataTable = ToDataTable<DomainModel.EmailInfo>(objEmailInfo);
            SqlBulkCopy bulkCopy = null;

            try
            {
                if (m_Cmd.Connection.State != System.Data.ConnectionState.Open)
                {
                    m_Cmd.Connection.Open();
                }
                    bulkCopy = new SqlBulkCopy
                    (
                    m_Cmd.Connection,
                    SqlBulkCopyOptions.TableLock |
                    SqlBulkCopyOptions.FireTriggers |
                    SqlBulkCopyOptions.UseInternalTransaction,
                    null
                    );

                bulkCopy.BatchSize = 5000;
                bulkCopy.DestinationTableName = TableName;
                foreach (DataColumn dcName in dataTable.Columns)
                {
                    if (dcName.ColumnName != "IsNew")
                    {
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(dcName.ColumnName, dcName.ColumnName));
                    }
                }
                bulkCopy.WriteToServer(dataTable);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, "Exception in SqlBulkInsert", ex,null,string.Empty);

                throw;
            }
            dataTable.Clear();
        }

        internal DataTable ToDataTable<EmailInfo>(IList<EmailInfo> data)
        {
            System.ComponentModel.PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(DomainModel.EmailInfo));

            DataTable dttable = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {

                PropertyDescriptor prop = props[i];

                if (prop.PropertyType.Namespace == "System")
                {
                    dttable.Columns.Add(string.Format("{0}", prop.Name), Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }

                else
                    dttable.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (EmailInfo item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                dttable.Rows.Add(values);
            }
            return dttable;
        }
    }
}

