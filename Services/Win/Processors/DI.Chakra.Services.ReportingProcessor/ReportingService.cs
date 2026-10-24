using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DBConfigHelper;
using System.Net.Http;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using DI.Chakra.Services.EmailingProcessor;
using System.Net;
using System.IO;
using Telerik.Reporting.Processing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Text.RegularExpressions;

namespace DI.Chakra.Services.ReportingProcessor
{
    public class ReportingService
    {

        private const string KEY_SERVICENAME = @"System";


        public ReportingService()
        {

        }

        private void ChangeReportingStatus(Process process, SchedulerStatus status)
        {
            var reportingProcess = DomainModel.Report.GetById(process.ProcessTypeKeyId);
            if (status == SchedulerStatus.InProgress)
            {
                process.ProcessStartAt = DateTime.Now;
                process.ProcessEndAt = null;
                process.Status = "InProgress";

            }
            else if (status == SchedulerStatus.Failed)
            {
                process.ProcessEndAt = DateTime.Now;
                process.Status = "Failed";
                //Need  to write logic to send email for failed Job
            }
            else if (status == SchedulerStatus.Pending)
            {
                process.LastModifiedBy = KEY_SERVICENAME;
                process.LastModifiedDate = DateTime.Now;
                process.Status = "Pending";

            }
            else
            {
                process.ProcessEndAt = DateTime.Now;
                process.LastRunOn = DateTime.Now;
                process.Status = "Completed";
            }
            process.Save(MasterConfigHelper.ServiceUser);

        }

        //public void Start(Process process)
        //{
        //    Report reportingProcess = null;
        //    string responseString = string.Empty;
        //    string result = string.Empty;
        //    try
        //    {
        //        reportingProcess = Report.GetById(process.ProcessTypeKeyId);

        //        User user = DomainModel.User.GetByUserName(DI.Chakra.Infrastructure.Constants.IACMADMIN);

        //        string loginUrl = DomainModel.ConfigurationMaster.GetConfigKey("IACMLoginURL", Product.IACM) + "?username=" + user.UserName + "&password=" + user.Password;

        //        if (reportingProcess == null)
        //        {
        //            DILog.ReportingLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Error, string.Format("Reporting Schedule process not found for ProcessId - {0}", Convert.ToString(process.ProcessTypeKeyId)));
        //            throw new EntityNotFoundException(string.Format("Reporting schedule process not found for ProcessId - {0}", Convert.ToString(process.ProcessTypeKeyId)));
        //        }
        //        DILog.ReportingLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Info, string.Format("Changing ProcessId:{0} to InProgress", Convert.ToString(reportingProcess.Id)));
        //        ChangeReportingStatus(process, SchedulerStatus.InProgress);
        //        DILog.ReportingLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Info, string.Format("Getdata for ProcessId:{0}", reportingProcess.Id.ToString()));
        //        switch (reportingProcess.Type)
        //        {
        //            case (ReportType.Attestation):
        //                break;
        //            case (ReportType.Audit):
        //                break;
        //            case (ReportType.Compliance):
        //                break;
        //            case (ReportType.EndpointSystem):

        //                ReportHistory reportHistory = new ReportHistory();
        //                try
        //                {
        //                    reportHistory.ReportId = reportingProcess.Id;
        //                    reportHistory.Status = ReportStatus.Initiated;
        //                    reportHistory.Save(MasterConfigHelper.ServiceUser);
        //                    string url = reportingProcess.ReportUrl + "?reportHistoryId=" + reportHistory.Id;
        //                    HttpClient client = new HttpClient();
        //                    HttpResponseMessage response = client.GetAsync(loginUrl).Result;
        //                    HttpContent content = response.Content;
        //                    responseString = content.ToString();
        //                    if (responseString != null)
        //                    {
        //                        response = client.GetAsync(url).Result;
        //                        content = response.Content;
        //                        responseString = content.ToString();
        //                    }
        //                  byte[] report= (ReportHistory.GetById(reportHistory.Id).Report);
        //                    /*Sending Email directly from the service in order to avoid saving (byte array)attachment in DI_M_Email, DI_M_EmailHistory 
        //                       */
        //                    if (report.Length > 0)
        //                    {
        //                        if (SendEmail(EmailTemplate.ReportSchedule, reportingProcess.UserId, report, reportingProcess))
        //                        {
        //                            reportHistory = DomainModel.ReportHistory.GetById(reportHistory.Id);
        //                            reportHistory.Status = ReportStatus.MailSent;
        //                            reportHistory.Save(MasterConfigHelper.ServiceUser);
        //                            DILog.ReportingLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Info, string.Format("Mail Sent for Report of ReportId:{0}", reportingProcess.Id.ToString()));
        //                        }
        //                        else
        //                        {
        //                            reportHistory = DomainModel.ReportHistory.GetById(reportHistory.Id);
        //                            reportHistory.Status = ReportStatus.MailFailed;
        //                            reportHistory.Save(MasterConfigHelper.ServiceUser);
        //                            DILog.ReportingLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Info, string.Format("Problem in sending mail for Report of ReportId:{0}", reportingProcess.Id.ToString()));
        //                        }
        //                    }

        //                    ChangeReportingStatus(process, SchedulerStatus.Completed);
        //                    ProcessHistory history = GetHistory(process.Id, "Success", "");
        //                    history.Save(MasterConfigHelper.ServiceUser);
        //                    DILog.ReportingLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Info, string.Format("Completed ProcessId:{0}", reportingProcess.Id.ToString()));

        //                }
        //                catch (Exception ex)
        //                {
        //                    process.ErrorMessage = ex.Message;
        //                    ChangeReportingStatus(process, SchedulerStatus.Failed);
        //                    // I need to write a code for saving error message and status in to ProcessHistroy table.
        //                    //throw ex;
        //                    DILog.ReportingLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Error, string.Format("Error while process the report found for ProcessId - {0}", Convert.ToString(process.ProcessTypeKeyId)));
        //                    reportHistory.Delete();
        //                }
        //                break;
        //            case (ReportType.Provisioning):
        //                break;
        //            case (ReportType.Security):
        //                break;
        //            case (ReportType.User):
        //                break;

        //        }
        //    }


        //    catch (Exception ex)
        //    {

        //    }

        //}
        public void Start(Process process)
        {
            DomainModel.Report reportingProcess = null;
            string responseString = string.Empty;
            string result = string.Empty;
            try
            {
                reportingProcess = DomainModel.Report.GetById(process.ProcessTypeKeyId);

                User user = DomainModel.User.GetByUserName(DI.Chakra.Infrastructure.Constants.IACMADMIN);

                string loginUrl = DomainModel.ConfigurationMaster.GetConfigKey("IACMLoginURL", Product.IACM) + "?username=" + user.UserName + "&password=" + user.Password;

                if (reportingProcess == null)
                {
                    DILog.ReportingLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Error, string.Format("Reporting Schedule process not found for ProcessId - {0}", Convert.ToString(process.ProcessTypeKeyId)));
                    throw new EntityNotFoundException(string.Format("Reporting schedule process not found for ProcessId - {0}", Convert.ToString(process.ProcessTypeKeyId)));
                }
                DILog.ReportingLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Info, string.Format("Changing ProcessId:{0} to InProgress", Convert.ToString(reportingProcess.Id)));
                ChangeReportingStatus(process, SchedulerStatus.InProgress);
                DILog.ReportingLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Info, string.Format("Getdata for ProcessId:{0}", reportingProcess.Id.ToString()));
                switch (reportingProcess.Type)
                {
                    case (ReportType.Attestation):
                        break;
                    case (ReportType.Audit):
                        break;
                    case (ReportType.Compliance):
                        break;
                    case (ReportType.EndpointSystem):

                        ReportHistory reportHistory = new ReportHistory();
                        try
                        {
                            reportHistory.ReportId = reportingProcess.Id;
                            reportHistory.Status = ReportStatus.Initiated;
                            reportHistory.Save(MasterConfigHelper.ServiceUser);
                            string url = reportingProcess.ReportUrl + "?reportHistoryId=" + reportHistory.Id;
                            HttpClient client = new HttpClient();
                            HttpResponseMessage response = client.GetAsync(loginUrl).Result;
                            HttpContent content = response.Content;
                            responseString = content.ToString();
                            if (responseString != null)
                            {
                                response = client.GetAsync(url).Result;
                                content = response.Content;
                                responseString = content.ToString();
                            }
                            byte[] report = (ReportHistory.GetById(reportHistory.Id).Report);
                            /*Sending Email directly from the service in order to avoid saving (byte array)attachment in DI_M_Email, DI_M_EmailHistory 
                               */
                            if (report.Length > 0)
                            {
                                if (SendEmail(EmailTemplate.ReportSchedule, reportingProcess.UserId, report, reportingProcess))
                                {
                                    reportHistory = DomainModel.ReportHistory.GetById(reportHistory.Id);
                                    reportHistory.Status = ReportStatus.MailSent;
                                    reportHistory.Save(MasterConfigHelper.ServiceUser);
                                    DILog.ReportingLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Info, string.Format("Mail Sent for Report of ReportId:{0}", reportingProcess.Id.ToString()));
                                }
                                else
                                {
                                    reportHistory = DomainModel.ReportHistory.GetById(reportHistory.Id);
                                    reportHistory.Status = ReportStatus.MailFailed;
                                    reportHistory.Save(MasterConfigHelper.ServiceUser);
                                    DILog.ReportingLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Info, string.Format("Problem in sending mail for Report of ReportId:{0}", reportingProcess.Id.ToString()));
                                }
                            }

                            ChangeReportingStatus(process, SchedulerStatus.Completed);
                            ProcessHistory history = GetHistory(process.Id, "Success", "");
                            history.Save(MasterConfigHelper.ServiceUser);
                            DILog.ReportingLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Info, string.Format("Completed ProcessId:{0}", reportingProcess.Id.ToString()));

                        }
                        catch (Exception ex)
                        {
                            process.ErrorMessage = ex.Message;
                            ChangeReportingStatus(process, SchedulerStatus.Failed);
                            // I need to write a code for saving error message and status in to ProcessHistroy table.
                            //throw ex;
                            DILog.ReportingLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Error, string.Format("Error while process the report found for ProcessId - {0}", Convert.ToString(process.ProcessTypeKeyId)));
                            reportHistory.Delete();
                        }
                        break;
                    case (ReportType.Provisioning):
                        break;
                    case (ReportType.Security):
                        break;
                    case (ReportType.User):
                        break;

                }
            }


            catch (Exception ex)
            {

            }

        }

        private ProcessHistory GetHistory(long processId, string status, string errMessage)
        {
            ProcessHistory history = new ProcessHistory();
            history.ProcessType = ProcessType.Reporting;
            history.ProcessId = processId;
            history.Status = status;
            history.ErrorMessage = errMessage;
            return history;
        }
        public bool SendEmail(EmailTemplate template, long userId, byte[] attachment, DomainModel.Report report)
        {
            MailTemplate objTemplate = null;
            objTemplate = MailTemplate.GetByTemplateId((int)template);

            EmailInfo emailInfo = TokenReplacer.Execute(objTemplate, userId);

            if (null == emailInfo)
                return false;
            else
            {
                emailInfo.From = objTemplate.MailFrom;
                emailInfo.Attachment = string.Empty;
                emailInfo.ActivityType = objTemplate.ActivityType.ToString();
                //if (string.IsNullOrEmpty(emailInfo.To))
                //    emailInfo.To = ConfigurationMaster.GetByConfigKey("AccountCreatedNotificationTo", Product.IACM).ConfigValue.ToString();
                emailInfo.To = report.MailTo;
                if (!string.IsNullOrEmpty(report.MailCC))
                {
                    emailInfo.Cc = report.MailCC;
                }
                IACMEmailProvider provider = new IACMEmailProvider();
                System.Net.Mail.Attachment _attachment = new System.Net.Mail.Attachment(new MemoryStream(attachment), string.Empty);
                _attachment.Name = report.Name + report.Format;
                provider.Send(emailInfo, _attachment);
                return true;
            }
        }

        public void StartAdHocReport(Process process, DateTime runDate)
        {
            ReportProcess reportingProcess = null;
            string responseString = string.Empty;
            string result = string.Empty;
            try
            {
                reportingProcess = ReportProcess.GetById(process.ProcessTypeKeyId, true);
                if (reportingProcess == null)
                {
                    DILog.ReportingLogger.WriteLog((int)process.Id, LogLevelType.Error, string.Format("Reporting process not found for ProcessId - {0}", Convert.ToString(process.ProcessTypeKeyId)));
                    throw new EntityNotFoundException(string.Format("Reporting process not found for ProcessId - {0}", Convert.ToString(process.ProcessTypeKeyId)));
                }
                if (reportingProcess.ReportDefinition == null)
                {
                    DILog.ReportingLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Error, string.Format("Reporting definition not found for ReportProcessId - {0}", Convert.ToString(reportingProcess.Id)));
                    throw new EntityNotFoundException(string.Format("Reporting definition not found for ReportProcessId - {0}", Convert.ToString(reportingProcess.Id)));
                }
                DILog.ReportingLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Info, string.Format("Changing ProcessId:{0} to InProgress", Convert.ToString(reportingProcess.Id)));
                ChangeReportingStatus(process, SchedulerStatus.InProgress);

                var reportDefinition = reportingProcess.ReportDefinition;
                var content = reportDefinition.ReportContent;
                string xml = System.Text.Encoding.UTF8.GetString(content);
                string query = reportDefinition.ReportQuery;
                var reportType = (ManagementReportType)reportDefinition.Id;
                AdhocReportBase reportBase = AdhocReportFactory.GetReportJob(reportType);
                var reportResult = reportBase.GetData(runDate);

                string connectionString = AppSettingsHelper.DatabaseConnectionString;
                //  Telerik.Reporting.SqlDataSource sqlDataSource = new Telerik.Reporting.SqlDataSource(connectionString, query);
                // List<User> users = User.GetAll().Take(20).ToList();
                Telerik.Reporting.Report report = new Telerik.Reporting.Report();

                report = report.CreateFromXML(xml);
                foreach (Telerik.Reporting.ReportItemBase item in report.Items)
                {
                    if (item is Telerik.Reporting.DetailSection)
                    {
                        Telerik.Reporting.DetailSection detail = (Telerik.Reporting.DetailSection)item;
                        if (detail != null && detail.Items.Count > 0)
                        {
                            foreach (Telerik.Reporting.ReportItemBase detailItem in detail.Items)
                            {
                                if (detailItem is Telerik.Reporting.Table)
                                {
                                    Telerik.Reporting.Table table = (Telerik.Reporting.Table)detailItem;
                                    if (table != null)
                                    {
                                        table.DataSource = reportResult;
                                    }
                                }
                            }
                        }
                    }
                };

                // report.Items[0]
                if (report != null)
                {
                    ReportProcessor reportProcessor = new ReportProcessor();
                    Telerik.Reporting.InstanceReportSource instanceReportSource = new Telerik.Reporting.InstanceReportSource();
                    instanceReportSource.ReportDocument = report;

                    //RenderingResult reportresult = reportProcessor.RenderReport("PDF", instanceReportSource, null);
                    RenderingResult reportresult = reportProcessor.RenderReport("PDF", instanceReportSource, null);
                    ReportAttachment existingReport = ReportAttachment.GetByName(reportDefinition.ReportName, runDate);
                    if (existingReport != null)
                    {
                        existingReport.FileName = reportDefinition.ReportName + runDate.ToShortDateString();
                        existingReport.ReportDefinitionId = reportDefinition.Id;
                        existingReport.Attachment = reportresult.DocumentBytes;
                        existingReport.ContentType = "PDF";
                        existingReport.LastModifiedBy = "System";
                        existingReport.LastModifiedDate = DateTime.Now;
                        existingReport.CreatedDate = runDate;
                        existingReport.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        ReportAttachment attachment = new ReportAttachment();
                        attachment.FileName = reportDefinition.ReportName + runDate.ToShortDateString();
                        attachment.ReportDefinitionId = reportDefinition.Id;
                        attachment.Attachment = reportresult.DocumentBytes;
                        attachment.ContentType = "PDF";
                        attachment.CreatedDate = runDate;
                        attachment.Save(MasterConfigHelper.ServiceUser);
                    }
                }
                ChangeReportingStatus(process, SchedulerStatus.Completed);
                ProcessHistory history = GetHistory(process.Id, "Success", "");
                history.Save(MasterConfigHelper.ServiceUser);
                DILog.DiscoveryLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Info, string.Format("Completed ProcessId:{0}", reportingProcess.Id.ToString()));

            }
            catch (Exception ex)
            {
                process.ErrorMessage = ex.Message;
                ChangeReportingStatus(process, SchedulerStatus.Failed);
                // I need to write a code for saving error message and status in to ProcessHistroy table.
                DILog.DiscoveryLogger.WriteLog((int)reportingProcess.Id, LogLevelType.Error, "ErrorTraced", ex);
                //throw ex;
            }

        }

        public void StartCustomReport(long reportId)
        {
            CustomReport report = CustomReport.GetById(reportId);
            if (report != null && !report.IsLinkedToMenu)
            {
                try
                {
                    var query = report.ReportQuery;
                    string matchWords = @"\b(delete|insert|update|create|with|drop|alter|--|cursor|exec|execute|commit|waitfor|kill)\w*\b";
                    Regex reg = new Regex(matchWords, RegexOptions.IgnoreCase);
                    bool isMatch = reg.IsMatch(query);
                    if (isMatch || !query.ToLower().StartsWith("select"))
                    {
                        report.Status = "Failed";
                        report.LastRunOn = DateTime.Now;
                        report.Save(MasterConfigHelper.ServiceUser);
                        DILog.ReportingLogger.WriteLog((int)report.Id, LogLevelType.Error, "Entered query is not valid for report, please enter SELECT query only.");
                        return;
                    }
                    
                    var resultSet = CustomReport.GetDataTableByQuery(report.ReportQuery);
                    if (resultSet != null)
                    {
                        string pattern = "[\\~#%&*{}/:<>?|\"-]";
                        string replacement = "";

                        Regex regEx = new Regex(pattern);
                        string fileName = Regex.Replace(regEx.Replace(report.Name, replacement), @"\s+", "");                        
                        var content = DataTableToExcel(resultSet, fileName);
                        report.FileName = fileName;
                        report.Attachment = content;
                        report.ContentType = "Excel";
                        report.Status = "Completed";
                        report.LastRunOn = DateTime.Now;
                        report.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        report.Status = "Failed";
                        report.LastRunOn = DateTime.Now;
                        report.Save(MasterConfigHelper.ServiceUser);
                    }
                }
                catch (Exception ex)
                {
                    report.Status = "Failed";
                    report.LastRunOn = DateTime.Now;
                    report.Save(MasterConfigHelper.ServiceUser);

                    DILog.ReportingLogger.WriteLog((int)report.Id, LogLevelType.Error, "ErrorTraced while generating custom report", ex);
                }
            }
            else if(report != null && report.IsLinkedToMenu)
            {
                report.Status = "Failed";
                report.LastRunOn = DateTime.Now;
                report.Save(MasterConfigHelper.ServiceUser);

                DILog.ReportingLogger.WriteLog((int)report.Id, LogLevelType.Error, "Custom Report won't be generated as the report is attached to a Menu.");
            }
        }


        private byte[] DataTableToExcel(DataTable dataTable, string fileName)
        {
            byte[] result;
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(fileName.Length > 20 ? "Report" : fileName);
                ws.Cells["A1"].LoadFromDataTable(dataTable, true);
                result = pck.GetAsByteArray();
            }
            return result;
        }
    }

}
