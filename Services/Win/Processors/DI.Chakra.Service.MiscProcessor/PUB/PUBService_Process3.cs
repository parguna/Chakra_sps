using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DI.Chakra.Infrastructure.PUBConstants;
using static System.Net.WebRequestMethods;

namespace DI.Chakra.Service.MiscProcessor 
{
    public class PUBService_Process3
    {
        readonly string _masterConfigHelperServiceUser;
        readonly SchedulerLogger _logger = null;

        object lockerObj = new object();
        const int   minuteMultiplier = 60000;
        string process3_filePath;
        System.Timers.Timer _timerForEmployeeProcess;
        System.Timers.Timer _timerForReportGeneration;
        BackUpDeleteStrategy _fileBackUpDeleteStrategy;
        readonly ISchedulerFactory _schedulerFactory;
        readonly string _CronScheduleString;
        public PUBService_Process3()
        {
            _logger = new SchedulerLogger();
            _timerForEmployeeProcess = new System.Timers.Timer((long)MasterConfigHelper.PUBFile_Process3_Timer * minuteMultiplier );
            _timerForEmployeeProcess.Elapsed += _timer_Elapsed;
            _timerForReportGeneration = new System.Timers.Timer((long)MasterConfigHelper.PUBFile_Process3_Timer * minuteMultiplier);
            _timerForReportGeneration.Elapsed += _timerForReportGeneration_Elapsed;
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
            _fileBackUpDeleteStrategy = (BackUpDeleteStrategy)MasterConfigHelper.PUBFileBackupDeleteStrategy_Process3;

            _schedulerFactory = new StdSchedulerFactory();

            _CronScheduleString = "0 42 14 * * ?";

        }



        public void Start()
        {
            process3_filePath = MasterConfigHelper.IDMS_TO_PUB_PROCESSED_FILEPATH;
            _timerForEmployeeProcess.Enabled = true;
            _timerForReportGeneration.Enabled = true;
            startBackUp();
        }

        async void startBackUp()
        {
            try
            {
                IScheduler scheduler = await _schedulerFactory.GetScheduler();

                await scheduler.Start();

                ITrigger trigger = TriggerBuilder.Create()
                     .WithIdentity(PUBConstants.PUBFileType.ProcessFile)
                     .WithCronSchedule(_CronScheduleString)   // "0 59 23 L * ?")
                     .StartAt(DateTime.UtcNow)
                     .WithPriority(1)
                     .Build();

                IJobDetail job = JobBuilder.Create<BackUpExistingProcessedFile>().WithIdentity(PUBConstants.PUBFileType.ProcessFile).Build();


                await scheduler.ScheduleJob(job, trigger);
            }
            catch (Exception ee)
            {
                _logger.WriteLog(null, LogLevelType.Error, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process3_FileName}: Exception in Starting the Process3.", ee);
            }
        }

        public   async void Stop()
        {
            try
            {
                if (_timerForEmployeeProcess != null)
                _timerForEmployeeProcess.Enabled = false;

            if (_timerForReportGeneration != null)
                _timerForReportGeneration.Enabled = false; 
           
                ISchedulerFactory schedFact = new StdSchedulerFactory();

                // get a scheduler
                IScheduler sched = await schedFact.GetScheduler();
                //sched.Standby();
                await sched.UnscheduleJob(new TriggerKey(PUBConstants.PUBFileType.ProcessFile)); 

            }
            catch (Exception ee)
            {
                _logger.WriteLog(null, LogLevelType.Error, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process1_FileName}: Exception in stoping the Process1", ee);
            }
        }
        private void _timerForReportGeneration_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (sender)
            {
                var files = PUBFile.GetFilesBySourceAndStatus(PUBConstants.PUBFileSource.PUB, new[] { PUBConstants.PUBFileStatus.FileYetToBeReported });
                if (files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        DataTable dt = PUBEmployees.GetDataTableByQuery($@"select  e.EBSUserId as EBSUserId, 
                                    e.DisplayName as EBSUsername,
                                    e.CompanyName as CompanyName, 
                                    convert(varchar(23),e.NewTerminationDate,103)   as ValidTillDate,
                                    Status, e.ValidationMsg as Remarks
                                    from di_pub_employees e  
                                    where e.pubfileid = {file.Id} ");
                        if (dt!=null)
                        {
                            FileGenerate(dt, file);
                        } 
                    }
                }
            }
        }


        void FileGenerate(DataTable dt, PUBFile file)
        {
            long fileId = file.Id;
            StringBuilder sb = new StringBuilder();
            _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process3_FileName}: Start File Generate ", null);

            string fullName = string.Empty;

            string backupName = string.Empty;

            string fileName = string.Empty;

            try
            {
                fileName = $"{PUBConstants.FileNames.Process3_FileName}_{DateTime.Now.ToString("yyyyMMdd")}";
                backupName = $@"{process3_filePath}\\{fileName}";
                fullName = $"{process3_filePath}\\{fileName}.CSV";
                

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
                    var bkupFilePath = $"{ MasterConfigHelper.PUB_TO_IDMS_BACKUP_FILEPATH}\\Processed\\";

                    if (!Directory.Exists(bkupFilePath))
                    {
                        Directory.CreateDirectory(bkupFilePath);
                    }
                    var directoryInfo = new DirectoryInfo(process3_filePath);
                    var fileNames = directoryInfo.GetFiles("*.*");

                    foreach (var fileinfo in fileNames)
                    {
                        var count = 0;
                        var f_fullName = $"{bkupFilePath}\\{fileinfo.Name}";
                        while (System.IO.File.Exists(f_fullName))
                        {
                            f_fullName = $"{bkupFilePath}\\{fileinfo.Name}".Replace(fileinfo.Extension, $"_{count}{fileinfo.Extension}");
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
                _logger.WriteLog(null, LogLevelType.Error, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process3_FileName}: Exception in creating & sending Report File.", ee);

                return;
                     
            }

            _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process3_FileName}: Report File Generated & sent sucessfully", null);

            // Creating & sending Report file

            if (!System.IO.File.Exists(fullName))
            {
                _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process3_FileName}: The File [{fullName}] is not exist", null);
                return ;
            }
            Byte[] bytes = System.Text.Encoding.ASCII.GetBytes(sb.ToString());
            //using (System.IO.FileStream fs = new System.IO.FileStream(fullName, FileMode.Open, FileAccess.Read))
            //{
            //    BinaryReader br = new BinaryReader(fs);

            //    bytes = br.ReadBytes((Int32)fs.Length);
            //}

           

            if (bytes != null)
            {
                try
                {
                     

                    _logger.WriteLog(fileId, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process3_FileName}: DI_PUB_File report file adding in the attachment", null);

                    var attachment = new Attachment()
                    {
                        AccessRequestId = fileId,
                        AccessRequestSource = PUBConstants.FileNames.Process3_FileName,
                        AttachmentFile = bytes,
                        FileName = fileName,
                        ContentType = "CSV"
                    };
                    attachment.Save(_masterConfigHelperServiceUser);

                    file.ReportedDateTime = DateTime.Now;
                    file.ReportedStatus = PUBConstants.PUBFileStatus.FileReported;
                    file.PUBFileStatus = PUBConstants.PUBFileStatus.FileProcessCompleted;
                    file.ReportFileName = fileName;
                    file.Save(_masterConfigHelperServiceUser);
                    ProvisioningEmailHelper.SendFileProcessedNotification(file, attachment);
                    _logger.WriteLog(fileId, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process3_FileName}: Report File Attached", null);

                }
                catch (Exception ee)
                {
                    _logger.WriteLog(null, LogLevelType.Error, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process3_FileName}: Exception in saving DI_PUB_File & Attachment", ee);

                    return;

                }
            }
            else
            {
                _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process3_FileName}: Could not read the report file from the path : {fullName}", null);
                return;
            }


           
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (lockerObj)
            {
                var files = PUBFile.GetFilesBySourceAndStatus(PUBConstants.PUBFileSource.PUB , new[] { PUBConstants.PUBFileStatus.FileSaved });
                if (files.Count > 0)
                {
                    foreach (var file in files)
                    {                         
                        PUBEmployees.ValidateAndSave(file.Id, _masterConfigHelperServiceUser,_logger);
                        PUBEmployees.Process(file.Id, _masterConfigHelperServiceUser,_logger);

                        file.PUBFileStatus = PUBConstants.PUBFileStatus.TerminationInProgress;
                        file.Save(_masterConfigHelperServiceUser);
                        _logger.WriteLog(file.Id, LogLevelType.Info, $"{PUBConstants.FileNames.Process3_FileName}: File  -[{file.Id}] is processed", null);
                    }
                }


                var inProgressFiles = PUBFile.GetFilesBySourceAndStatus(PUBConstants.PUBFileSource.PUB, new[] { PUBConstants.PUBFileStatus.TerminationInProgress });
                if (inProgressFiles.Count > 0)
                {
                    foreach (var file in inProgressFiles)
                    {
                        var query = getPUBEmployeeStatusUpdateQuery(file.Id);
                        PUBEmployees.ExecuteQueryWithCommandTimeout(query, 0);
                        _logger.WriteLog(file.Id, LogLevelType.Info, $"{PUBConstants.FileNames.Process3_FileName}: Employee Status Update Query Executed :[{query}]", null);


                        query = getPUBFileStatusUpdateQuery(file.Id);
                        PUBEmployees.ExecuteQueryWithCommandTimeout(query, 0);
                        _logger.WriteLog(file.Id, LogLevelType.Info, $"{PUBConstants.FileNames.Process3_FileName}: File Status Update Query Executed :[{query}]", null);
                    }
                }

                //var terminationInProgressFiles = PUBFile.GetFilesBySourceAndStatus(PUBConstants.PUBFileSource.PUB, new[] { PUBConstants.PUBFileStatus.TerminationInProgress });
                //if (terminationInProgressFiles.Count > 0)
                //{
                //    foreach (var file in terminationInProgressFiles)
                //    {
                       
                //    }
                //}

            }

            string getPUBEmployeeStatusUpdateQuery(long fileID)
            { 
                return  $@" UPDATE  PE1  Set  PE1.[Status]  =  
                                   CASE WHEN TT.TotalCount=TT.ProcessedCount AND TT.TotalCount=TT.ExecutionCompleted THEN 'Processed'
                                        WHEN TT.TotalCount=TT.ProcessedCount AND TT.TotalCount=TT.TerminationFailed THEN 'Failed'
										WHEN TT.TotalCount=TT.ProcessedCount AND TT.TotalCount=TT.ValidationFailed THEN 'ValidationFailedInTermination'
										ELSE  PE1.[Status]
                                   END ,

                            PE1.[ValidationMsg]  = CASE WHEN TT.TotalCount=TT.ProcessedCount AND TT.TotalCount=TT.TerminationFailed THEN 'Exceptional Error: Network/Execution' ELSE '' END

                            FROM DI_PUB_Employees as PE1 JOIN 
                            (
                                SELECT PE.ID, count(1) as TotalCount ,SUM( CASE WHEN ProgressStatus between 5 and 7 THEN 1 else 0 End) as  ProcessedCount,
                                SUM( CASE WHEN ProgressStatus=5 THEN 1 else 0 End) as  ExecutionCompleted,
                                SUM( CASE WHEN ProgressStatus=6 THEN 1 else 0 End) as  TerminationFailed,
                                SUM( CASE WHEN ProgressStatus=7 THEN 1 else 0 End) as  ValidationFailed 
                                From DI_M_AccessRequest AR join DI_M_EmployeeEventTrigger ET on AR.EmployeeEventTriggerId =  ET.Id 
                                join  DI_PUB_Employees PE on ET.UserId=PE.DIUserId  
                                where  AR.EmployeeEventTriggerId IS NOT NULL AND
                                AR.EndpointSystemId in (SELECT ID FROM DI_M_EndpointSystem WHERE ApplicationType in
                                ({ (byte)ApplicationType.EBS_ISU},{ (byte)ApplicationType.EBS_CRM},{ (byte)ApplicationType.EBS})
                            ) AND
                            PE.[Status] = 'PendingInProcess' and
                            PE.[PUBFileId]={fileID} GROUP BY PE.ID ) as TT on PE1.Id = TT.Id";


            }

            string getPUBFileStatusUpdateQuery(long fileID)
            {
                return $@"  UPDATE f SET f.PUBFileStatus= 
                              CASE WHEN TEMP.TerminationInProgress = 0 THEN 'FileYetToBeReported' ELSE f.PUBFileStatus END
                           FROM DI_PUB_File f JOIN 
                                            (SELECT 
                                                     SUM(CASE WHEN e.[Status] = 'PendingInProcess' THEN 1 ELSE 0 END) as TerminationInProgress,
                                                     e.PUBFileId FROM  DI_PUB_Employees e
                          WHERE  e.PUBFileId= {fileID}  GROUP BY e.PUBFileId) as TEMP on f.Id=TEMP.PUBFileId";
            }



        }


    }

    public class BackUpExistingProcessedFile : IJob
    {

        readonly BackUpDeleteStrategy _fileBackUpDeleteStrategy;
        readonly string _masterConfigHelperServiceUser;
        public BackUpExistingProcessedFile()
        {
            _fileBackUpDeleteStrategy = (BackUpDeleteStrategy)MasterConfigHelper.PUBFileBackupDeleteStrategy_Process3; 
        }
        public System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            Action action = () =>
            {

                DI.Chakra.Logger.SchedulerLogger _logger = new SchedulerLogger();
                string backupName = string.Empty, fullName = string.Empty, fileName = string.Empty;

                try
                {
                    string process1_filePath = MasterConfigHelper.IDMS_TO_PUB_PROCESSED_FILEPATH;
                    
                    var bkupFilePath = $"{ MasterConfigHelper.PUB_TO_IDMS_BACKUP_FILEPATH}\\Processed\\";

                    if (!Directory.Exists(bkupFilePath))
                    {
                        Directory.CreateDirectory(bkupFilePath);
                    }
                    var directoryInfo = new DirectoryInfo(process1_filePath);
                    var fileNames = directoryInfo.GetFiles("*.*");

                    foreach (var fileinfo in fileNames)
                    {
                        var count = 0;
                        var f_fullName = $"{bkupFilePath}\\{fileinfo.Name}";
                        while (System.IO.File.Exists(f_fullName))
                        {
                            f_fullName = $"{bkupFilePath}\\{fileinfo.Name}".Replace(fileinfo.Extension, $"_{count}{fileinfo.Extension}");
                            count++;
                        }
                        if (_fileBackUpDeleteStrategy == BackUpDeleteStrategy.BackUp)
                        {
                            System.IO.File.Move(fileinfo.FullName, f_fullName);
                            _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process3_FileName}: File backup from: [{fileinfo.FullName}] to : [{f_fullName}] ", null);
                        }
                        else if (_fileBackUpDeleteStrategy == BackUpDeleteStrategy.Delete && System.IO.File.Exists(fullName))
                        {
                            System.IO.File.Delete(fullName);
                        }
                    }
                }
                catch (Exception ee)
                {
                    _logger.WriteLog(null, LogLevelType.Error, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process3_FileName}: Exception in creating & sending Complete File.", ee);

                    return;
                } 
            };

            return System.Threading.Tasks.Task.Run(action);
        }
    } 

}
