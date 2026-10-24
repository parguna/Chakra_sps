using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using Quartz;
using Quartz.Impl;
using static DI.Chakra.Infrastructure.PUBConstants;

namespace DI.Chakra.Service.MiscProcessor
{    
    public class PUBService_Process1
    { 

      
        readonly SchedulerLogger _logger;
        readonly string _masterConfigHelperServiceUser;

        readonly ISchedulerFactory _schedulerFactory;

        readonly string _CronScheduleString;

        readonly ISchedulerFactory _schedulerFactory_NoFileNotification;

        readonly string _cronScheduleExpressionForNoFileReceivedNotification;

            
        public PUBService_Process1()
        {
            _logger = new SchedulerLogger();
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;

            _schedulerFactory = new StdSchedulerFactory();

            _schedulerFactory_NoFileNotification = new StdSchedulerFactory();

            _CronScheduleString = MasterConfigHelper.CRON_SCHEDULE_EXPRESSION;

            _cronScheduleExpressionForNoFileReceivedNotification = MasterConfigHelper.NOFILE_RECEIVED_CRON_SCHEDULE_EXPRESSION;
        }

        public async void Start()
        {
            try
            {
                IScheduler scheduler = await _schedulerFactory.GetScheduler();

                await scheduler.Start();

                ITrigger trigger = TriggerBuilder.Create()
                     .WithIdentity(PUBConstants.PUBFileType.CompleteFile)
                     .WithCronSchedule(_CronScheduleString)   // "0 59 23 L * ?")
                     .StartAt(DateTime.UtcNow)
                     .WithPriority(1)
                     .Build();

                IJobDetail job = JobBuilder.Create<CreateCompleteFile>().WithIdentity(PUBConstants.PUBFileType.CompleteFile).Build();


                await scheduler.ScheduleJob(job, trigger);



                IScheduler scheduler_NoFileReceivedNotification = await _schedulerFactory_NoFileNotification.GetScheduler();

                await scheduler_NoFileReceivedNotification.Start();

                ITrigger trigger_NoFileReceivedNotification  = TriggerBuilder.Create()
                     .WithIdentity(ConfigConstants.NOFILE_RECEIVED_CRON_SCHEDULE_EXPRESSION)
                     .WithCronSchedule(_cronScheduleExpressionForNoFileReceivedNotification)   // "0 59 23 L * ?")
                     .StartAt(DateTime.UtcNow)
                     .WithPriority(1)
                     .Build();

                IJobDetail job_NoFileReceivedNotification = JobBuilder.Create<NoFileReceivedMailNotificationJob>().WithIdentity(ConfigConstants.NOFILE_RECEIVED_CRON_SCHEDULE_EXPRESSION).Build();


                await scheduler_NoFileReceivedNotification.ScheduleJob(job_NoFileReceivedNotification, trigger_NoFileReceivedNotification);




            }
            catch (Exception ee)
            {
                _logger.WriteLog(null, LogLevelType.Error, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process1_FileName}: Exception in Starting the Process1.", ee);
            }
        }

        public async void Stop()
        {
            try
            {
                ISchedulerFactory schedFact = new StdSchedulerFactory();

                // get a scheduler
                IScheduler sched = await schedFact.GetScheduler();
                //sched.Standby();
                await sched.UnscheduleJob(new TriggerKey(PUBConstants.PUBFileType.CompleteFile));

               

                // get a scheduler
                IScheduler sched_ = await schedFact.GetScheduler();
                //sched.Standby();
                await sched_.UnscheduleJob(new TriggerKey(ConfigConstants.NOFILE_RECEIVED_CRON_SCHEDULE_EXPRESSION));


            }
            catch (Exception ee)
            {
                _logger.WriteLog(null, LogLevelType.Error, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process1_FileName}: Exception in stoping the Process1", ee);
            }
        }

        

    }

    public class CreateCompleteFile : IJob {

        readonly BackUpDeleteStrategy _fileBackUpDeleteStrategy;
        readonly string _masterConfigHelperServiceUser;
        public CreateCompleteFile()
        {
            _fileBackUpDeleteStrategy =  (BackUpDeleteStrategy)MasterConfigHelper.PUBFileBackupDeleteStrategy_Process1;
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
        }
       public System.Threading.Tasks.Task  Execute(IJobExecutionContext context)
        {
            Action action = () =>
            {

                DI.Chakra.Logger.SchedulerLogger _logger = new SchedulerLogger();
                string backupName = string.Empty, fullName = string.Empty, fileName = string.Empty;

                try
                {

                    string process1_filePath = MasterConfigHelper.IDMS_TO_PUB_FILEPATH;//.IDMS_TO_PUB_PROCESSED_FILEPATH;

                    var codes = MasterConfigHelper.PUB_Company;
                    string company = string.IsNullOrWhiteSpace(codes) ? "''" : codes;
                    codes = MasterConfigHelper.PUB_CostCenter;
                    string costcenter = string.IsNullOrWhiteSpace(codes) ? "''" : codes;

                    string query = $@" 
	                                    SELECT T.EBSUserId	AS EBSUserId, 
			                                    u.DisplayName AS EBSUsername,  
			                                    U.NCompany AS CompanyName,
			                                    CASE WHEN ISNULL(U.TerminationDate,'')!='' 
			                                         THEN FORMAT(U.TerminationDate,'dd/MM/yyyy') 
				                                     ELSE '' 
				                                     END 
			                                    AS ValidTillDate 
	                                    FROM DI_M_User U JOIN 
		                                     (select   a.userid , AccountName AS EBSUserId 
			                                    FROM DI_M_AccountReconcilation a 
			                                    WHERE a.EndpointSystemId in (SELECT ID FROM DI_M_Endpointsystem where ApplicationType in  ({ (byte)ApplicationType.EBS},{ (byte)ApplicationType.EBS_ISU},{ (byte)ApplicationType.EBS_CRM}))
				                                      AND a.IsDeleted=0 AND a.AccountType=1
				                                      AND ISNULL(a.USERID,'')!=''  
			                                    GROUP BY  a.userid , a.AccountName 
		                                      ) as T 
		                                      ON U.Id = T.UserId
	                                     WHERE  U.FullTimeEmployee =0 and U.IsActive=1 and  LTRIM(RTRIM(U.CostCenter))  IN ({costcenter}) and     LTRIM(RTRIM(U.NCompany)) IN ({company}) ";

                    _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process1_FileName}: Query to get PUB users : {query} ", null);

                    DataTable dt = PUBEmployees.GetDataTableByQuery(query);

                    _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process1_FileName}: Start Complete File Generate ", null);

                    fileName = $"{PUBConstants.FileNames.Process1_FileName}_{DateTime.Now.ToString("yyyyMMdd")}";
                    backupName = fullName = $@"{process1_filePath}\\{fileName}";
                    fullName = fullName + ".csv";
                    StringBuilder sb = new StringBuilder();

                    IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                                      Select(column => column.ColumnName);
                    sb.AppendLine(string.Join(",", columnNames));

                    foreach (DataRow row in dt.Rows)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                        sb.AppendLine(string.Join(",", fields));
                    }

                    if (_fileBackUpDeleteStrategy == BackUpDeleteStrategy.BackUp)
                    {
                        var bkupFilePath = $"{ MasterConfigHelper.PUB_TO_IDMS_BACKUP_FILEPATH}\\Monthly\\";

                        if (!Directory.Exists(bkupFilePath))
                        {
                            Directory.CreateDirectory(bkupFilePath);
                        }
                        var directoryInfo = new DirectoryInfo(process1_filePath);
                        var fileNames = directoryInfo.GetFiles("*.*");

                        

                        //backupName = $"{bkupFilePath}\\{backupName.Replace(".csv", "")}";
                        foreach (var fileinfo in fileNames)
                        {
                            var count = 0;
                            var f_fullName = $"{bkupFilePath}\\{fileinfo.Name}";
                            while (System.IO.File.Exists(f_fullName))
                            {                                
                                f_fullName = $"{bkupFilePath}\\{fileinfo.Name}".Replace(fileinfo.Extension,$"_{count}{fileinfo.Extension}");
                                count++;
                            }
                            System.IO.File.Move(fileinfo.FullName, f_fullName);
                            _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process1_FileName}: File backup from: [{fileinfo.FullName}] to : [{f_fullName}] ", null);
                        }
                         
                    }
                    else if (_fileBackUpDeleteStrategy == BackUpDeleteStrategy.Delete && System.IO.File.Exists(fullName))
                    {
                        System.IO.File.Delete(fullName);
                    }

                    System.IO.File.WriteAllText(fullName, sb.ToString());


                }
                catch (Exception ee)
                {
                    _logger.WriteLog(null, LogLevelType.Error, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process1_FileName}: Exception in creating & sending Complete File.", ee);

                    return; 
                }

                _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process1_FileName}: Complete File Generated & sent sucessfully", null);

                // Creating & sending Report file

                if (!System.IO.File.Exists(fullName))
                {
                    _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process1_FileName}: The File [{fullName}] is not exist", null);
                    return;
                }
                Byte[] bytes = null;
                using (System.IO.FileStream fs = new System.IO.FileStream(fullName, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader br = new BinaryReader(fs);

                    bytes = br.ReadBytes((Int32)fs.Length);
                }

                if (bytes != null)
                {
                    try
                    {


                        _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process1_FileName}: DI_PUB_File Complete file adding in the attachment", null);

                        var file = new PUBFile() {
                            FileDateTime = DateTime.Now,
                            PUBFileSource = PUBFileSource.IDMS,
                            PUBFileStatus = PUBFileStatus.FileSent,
                            PUBFileType = PUBFileType.CompleteFile,
                            FileName = fileName
                        };

                        long fileId = file.Save(_masterConfigHelperServiceUser);


                        var attachment = new Attachment()
                        {
                            AccessRequestId = fileId,
                            AccessRequestSource = PUBConstants.FileNames.Process1_FileName,
                            AttachmentFile = bytes,
                            FileName = fileName,
                            ContentType = "CSV"
                        };
                        attachment.Save(_masterConfigHelperServiceUser);

                        ProvisioningEmailHelper.SendMonthlyFileGeneratedNotification(attachment);

                        _logger.WriteLog(fileId, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process1_FileName}: Complete File Attached", null);

                    }
                    catch (Exception ee)
                    {
                        _logger.WriteLog(null, LogLevelType.Error, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process1_FileName}: Exception in saving DI_PUB_File & Attachment", ee);

                        return;

                    }
                }
                else
                {
                    _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process1_FileName}: Could not read the report file from the path : {fullName}", null);
                    return;
                }


            };

            return  System.Threading.Tasks.Task.Run(action);
        }
    }


    public class NoFileReceivedMailNotificationJob: IJob
    {
 
        public System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            Action action = () =>
            {

                DI.Chakra.Logger.SchedulerLogger _logger = new SchedulerLogger();
                 
                try
                {



                    int noOfFilesFortheDay = PUBFile.NoOfFilesFortheDay(PUBFileSource.PUB, PUBFileType.ProcessFile);

                    if (noOfFilesFortheDay > 0) return; 

                    _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process1_FileName}: Start sending No File Received Notification", null);

                    ProvisioningEmailHelper.SendNoFileReceivedNotification();

                    return;

                }
                catch (Exception ee)
                {
                    _logger.WriteLog(null, LogLevelType.Error, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process1_FileName}: Exception in sending No File Received Notification", ee);

                    return;
                } 
            };

            return System.Threading.Tasks.Task.Run(action);
        }
    }



}
