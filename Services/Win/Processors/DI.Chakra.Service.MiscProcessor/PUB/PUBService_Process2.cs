using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using DI.Chakra.ProvisioningUtility;
using Microsoft.VisualBasic.FileIO;
using static DI.Chakra.Infrastructure.PUBConstants;

namespace DI.Chakra.Service.MiscProcessor 
{
    public class PUBService_Process2 : IDisposable
    {

        // Process1 : File sending from IDMS to PUB monthly once
        // Process2:  File receiving from PUB on daily basis
        // Process3:  Report File sending from IDMS to PUB against the process2

        #region Members

        bool _disposed = false;
        readonly BackUpDeleteStrategy _fileBackUpDeleteStrategy;
        readonly string _masterConfigHelperServiceUser ;
        readonly SchedulerLogger _logger = null;
        readonly string[] Process2ColumnNames;
        FileSystemWatcher _fileSystemWatcher;
        DataTable csvData;
        string process2_filePath;

       
        #endregion
        public PUBService_Process2()
        {
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser; 
            _logger = new SchedulerLogger();
            _fileBackUpDeleteStrategy =  (BackUpDeleteStrategy)MasterConfigHelper.PUBFileBackupDeleteStrategy_Process2;
            Process2ColumnNames = new[] { "EBSUserId", "EBSUsername", "CompanyName", "ValidTillDate" };
        }

      

         
        public void Start()
        {

            process2_filePath = MasterConfigHelper.PUB_TO_IDMS_FILEPATH;

            _fileSystemWatcher = new FileSystemWatcher();

           _fileSystemWatcher.Path = process2_filePath;

            _fileSystemWatcher.IncludeSubdirectories = true;
            _fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes |
            NotifyFilters.CreationTime |
            NotifyFilters.DirectoryName |
            NotifyFilters.FileName |
            NotifyFilters.LastAccess |
            NotifyFilters.LastWrite |
            NotifyFilters.Security |
            NotifyFilters.Size;
            // Watch all files.

            _fileSystemWatcher.EnableRaisingEvents = true;
            _fileSystemWatcher.Filter = "*.csv";

            _fileSystemWatcher.Created += OnChanged;

            _fileSystemWatcher.Deleted += OnChanged;

            _fileSystemWatcher.Changed += OnChanged;

            _fileSystemWatcher.Renamed += OnRenamed;


        }

        public void Stop()
        {
            if (_fileSystemWatcher != null)
                _fileSystemWatcher.EnableRaisingEvents = false;


        }

        long ReadAndSaveFile(string fullName, string filename)
        {
            Byte[] bytes = null;
            long fileId = 0;
            string backupName = fullName.Replace(process2_filePath, "");             

            try
            {
                if (!File.Exists(fullName))
                {
                    _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $" The File [{fullName}] is not exist", null);
                    return fileId;
                }


                FileInfo info = new FileInfo(fullName);
                backupName = info.Name;

                using (FileStream fs = new FileStream(fullName, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader br = new BinaryReader(fs);

                    bytes = br.ReadBytes((Int32)fs.Length);
                }

                try
                {
                    var file = new PUBFile()
                    {
                        FileDateTime = DateTime.Now,
                        PUBFileSource = PUBConstants.PUBFileSource.PUB,
                        PUBFileType = PUBConstants.PUBFileType.ProcessFile,
                        PUBFileStatus = PUBConstants.PUBFileStatus.FileReceived,
                        FileName = filename.Contains(".CSV", StringComparison.InvariantCultureIgnoreCase) ? filename.ToUpper().Replace(".CSV","") : filename
                    };

                    fileId = file.Save(_masterConfigHelperServiceUser);

                    _logger.WriteLog(fileId, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process2_FileName}:DI_PUB_File is created", null);

                    var attachment = new Attachment()
                    {
                        AccessRequestId = fileId,
                        AccessRequestSource = PUBConstants.FileNames.Process2_FileName,
                        AttachmentFile = bytes,
                        FileName = filename.Contains(".CSV", StringComparison.InvariantCultureIgnoreCase) ? filename.ToUpper().Replace(".CSV", "") : filename,
                        ContentType = "CSV"
                    };
                    attachment.Save(_masterConfigHelperServiceUser);

                    _logger.WriteLog(fileId, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process2_FileName}: File Attached.", null);

                }
                catch (Exception ee)
                {
                    _logger.WriteLog(null, LogLevelType.Error, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process2_FileName}: Exception in saving DI_PUB_File & Attachment", ee);
                }

                if (_fileBackUpDeleteStrategy == BackUpDeleteStrategy.BackUp)
                {
                    var bkupFilePath = $"{ MasterConfigHelper.PUB_TO_IDMS_BACKUP_FILEPATH}\\Daily\\";

                    if (!Directory.Exists(bkupFilePath))
                    {
                        Directory.CreateDirectory(bkupFilePath);
                    }
                    var directoryInfo = new DirectoryInfo(process2_filePath);
                    var fileNames = directoryInfo.GetFiles("*.*");

                    foreach (var fileinfo in fileNames)
                    {
                        var count = 0;
                        var f_fullName = $"{bkupFilePath}\\{fileinfo.Name}";
                        while (File.Exists(f_fullName))
                        {
                            f_fullName = $"{bkupFilePath}\\{fileinfo.Name}".Replace(fileinfo.Extension, $"_{count}{fileinfo.Extension}");
                            count++;
                        }
                         
                        File.Move(fileinfo.FullName, f_fullName);
                        _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process1_FileName}: File backup from: [{fileinfo.FullName}] to : [{f_fullName}] ", null);
                    }
                }
                else if (_fileBackUpDeleteStrategy == BackUpDeleteStrategy.Delete)
                {
                    File.Delete(fullName);
                }

            }
            catch (Exception eee)
            {
                _logger.WriteLog(null, LogLevelType.Error, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process2_FileName}: Exception in reading the file.", eee);
            }
            return fileId;
        }

        void ProcessSavedFile()
        {
            csvData = new DataTable(PUBConstants.FileNames.Process2_FileName);
            lock (csvData)
            {
                List<EntityModel.Attachment> attachements = PUBFile.GetAttachmentsBySourceAndStatus(PUBConstants.FileNames.Process2_FileName, new[] { PUBConstants.PUBFileStatus.FileProcessing, PUBConstants.PUBFileStatus.FileReceived });

                if (attachements == null || attachements.Count() == 0)
                {
                    _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process2_FileName}: No file found for processing... ", null);
                    return;
                }

                try
                {
                    foreach (var attachment in attachements)
                    {
                        var file = PUBFile.GetById(attachment.AccessRequestId);
                        if (file == null) continue;

                        file.PUBFileStatus = PUBConstants.PUBFileStatus.FileProcessing;
                        file.Save(_masterConfigHelperServiceUser);
                        try
                        {

                            PUBFile.ExecuteQueryWithCommandTimeout($" DELETE FROM DI_PUB_EMPLOYEES WHERE PUBFILEID = {file.Id} ", 0);
                        }
                        catch (Exception f)
                        {
                            _logger.WriteLog(file.Id, LogLevelType.Error, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process2_FileName}: Exception in Deletion of existing employees for the file ID - {file.Id}", null);
                        }

                        _logger.WriteLog(file.Id, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process2_FileName}: Start processing the file ID - {file.Id}", null);

                        try
                        {
                            using (Stream stream = new MemoryStream(attachment.AttachmentFile))
                            using (TextFieldParser parser = new TextFieldParser(stream))
                            {
                                parser.SetDelimiters(",");
                                parser.TrimWhiteSpace = true;
                                parser.HasFieldsEnclosedInQuotes = true;
                                string[] colFields = parser.ReadFields();

                                foreach (string column in colFields)
                                {
                                    if (csvData.Columns.Contains(column)) continue;
                                    DataColumn datecolumn = new DataColumn(column);
                                    datecolumn.AllowDBNull = true;
                                    csvData.Columns.Add(datecolumn);
                                }
                                 
                                while (!parser.EndOfData)
                                {
                                    string[] fieldData = parser.ReadFields();
                                    for (int i = 0; i < fieldData.Length; i++)
                                    {
                                        if (fieldData[i] == "")
                                        {
                                            fieldData[i] = null;
                                        }
                                    }
                                    csvData.Rows.Add(fieldData);
                                }

                                if (csvData.Columns.Cast<DataColumn>().Any(c => !Process2ColumnNames.Contains(c.ColumnName.Trim(), StringComparer.InvariantCultureIgnoreCase)) || csvData.Rows.Count == 0)
                                {
                                    file = PUBFile.GetById(file.Id);
                                    file.PUBFileStatus = PUBConstants.PUBFileStatus.FileError;
                                    if (csvData.Rows.Count == 0)
                                    {
                                        file.PUBFileStatus = PUBFileStatus.FileYetToBeReported;
                                        file.ValidationMsg = $"No Rows Found";
                                        file.Save(_masterConfigHelperServiceUser);
                                        ProvisioningEmailHelper.SendFileReceivedNotification(file, attachment);
                                    }
                                    else
                                    {
                                        file.ValidationMsg = $"FileError- [Message: Column mismatch ]";
                                        file.PUBFileStatus = PUBConstants.PUBFileStatus.FileError;
                                        file.Save(_masterConfigHelperServiceUser);
                                        ProvisioningEmailHelper.SendFileReceivedNotification(file, attachment);
                                    }

                                    continue;
                                }

                                process(csvData, file.Id);
                                ProvisioningEmailHelper.SendFileReceivedNotification(file, attachment);
                                csvData.Clear();
                            }
                        }
                        catch (Exception ee)
                        {
                            _logger.WriteLog(file.Id, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process2_FileName}: Exception in converting the file into DataTable.", ee);


                            file = PUBFile.GetById(file.Id);
                            file.PUBFileStatus = PUBConstants.PUBFileStatus.FileError;
                            file.ValidationMsg = $"FileError- [Message:{ee.Message}]; [StackTrace: {ee.StackTrace}]";
                            file.Save(_masterConfigHelperServiceUser);
                        }
                        file.PUBFileStatus = PUBConstants.PUBFileStatus.FileSaved;
                        file.Save(_masterConfigHelperServiceUser);
                         
                    }
                }
                catch (Exception ee)
                {
                    _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{PUBConstants.FileNames.Process2_FileName}: Exception in ProcessSavedFile.", ee);



                }
            }
        }

        void process(DataTable dt, long fileID)
        {
            string[] dateFormats = new[] { PUBConstants.PUBDateFormat, "dd/M/yyyy", "d/MM/yyyy", "d/M/yyyy" };
            StringBuilder stringBuilder = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {
                stringBuilder.Length = 0;
                stringBuilder.Clear();
                var pubemployee = new PUBEmployees()
                {
                    PUBFileId = fileID
                };

                if (dr["EBSUserId"] == DBNull.Value)
                {
                    AddValidation(stringBuilder,  PUB_ValidationMsg.InvalidEBSUserId);
                }
                else
                {

                    pubemployee.EBSUserId = dr["EBSUserId"].ToString();
                }


                if (dr["CompanyName"] == DBNull.Value)
                {
                    AddValidation(stringBuilder, PUB_ValidationMsg.Company_Name_is_not_PUB);
                }
                else
                { 
                    pubemployee.CompanyName = dr["CompanyName"].ToString();
                }

                if (dr["ValidTillDate"] == DBNull.Value)
                {
                    AddValidation(stringBuilder, PUB_ValidationMsg.InvalidDate );
                }
                else
                {


                    if (DateTime.TryParseExact(dr["ValidTillDate"].ToString(), dateFormats, null, System.Globalization.DateTimeStyles.None,
                            out DateTime parseDateTime))
                    {
                        pubemployee.NewTerminationDate = parseDateTime;
                    }
                    else
                    {
                        AddValidation(stringBuilder, PUB_ValidationMsg.InvalidDate);
                    }
                }

                if (dr["EBSUsername"] != DBNull.Value)
                {
                    pubemployee.DisplayName = dr["EBSUsername"].ToString();
                }



                if (stringBuilder.Length > 0)
                {

                    pubemployee.ValidationMsg =  stringBuilder.ToString();
                    pubemployee.Status = PUBConstants.EmployeeStatus.ValidationFailed;
                }
                else
                {
                    pubemployee.Status =  PUBConstants.EmployeeStatus.Received;
                }

                pubemployee.Save(_masterConfigHelperServiceUser); 

            }
        }


        static void AddValidation(StringBuilder stringBuilder, string msg)
        {
            if (stringBuilder.Length == 0) { stringBuilder.Append(msg); return; }

            if (stringBuilder.ToString().Contains(msg)) return;

            stringBuilder.Append(msg);

        }

        #region File Listener

        void OnChanged(object source, FileSystemEventArgs e)
        {
            _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{e.Name}, with path {e.FullPath} has been {e.ChangeType}", null);
            if (e.Name.Equals( $"{PUBConstants.FileNames.Process2_FileName}_{DateTime.Now.ToString("yyyyMMdd")}.csv", StringComparison.InvariantCultureIgnoreCase)
                && e.ChangeType == WatcherChangeTypes.Created)
            {
                _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"Receiving the file :[{e.Name}]", null);
                ReadAndSaveFile(e.FullPath, e.Name);
                ProcessSavedFile();
                _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"Saved the file :[{e.Name}] for processing.", null);
            }
        }
        void OnRenamed(object source, RenamedEventArgs e)
        {
            _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"{e.Name}, with path {e.FullPath} has been {e.ChangeType}", null);

            if (e.Name.Equals($"{PUBConstants.FileNames.Process2_FileName}_{DateTime.Now.ToString("yyyyMMdd")}.csv", StringComparison.InvariantCultureIgnoreCase)
                )
            {
                _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"Receiving the file :[{e.Name}]", null);
                ReadAndSaveFile(e.FullPath, e.Name);
                ProcessSavedFile();
                _logger.WriteLog(null, LogLevelType.Info, Infrastructure.Enums.LogReferenceType.PUBProcess, $"Saved the file :[{e.Name}] for processing.", null);
            }
        }

        #endregion

        








        #region IDisposable Implementation
        public void Dispose() => Dispose(true);


        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                  _fileSystemWatcher=null;
                  csvData=null;
            }

            _disposed = true;
        }
        #endregion

    }
}
