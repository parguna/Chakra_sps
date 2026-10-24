using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure 
{
    public class PUBConstants
    {
        public const string PUBDateFormat = "dd/MM/yyyy";
        public class EmployeeStatus
        {
            public const string Received = "Received";
            public const string ProcessedCompleted = "Processed";
            public const string YetToBeProcessed = "YetToBeProcessed";
            public const string PendingInProcess  = "PendingInProcess";
            public const string ValidationFailed = "Failed";
            public const string FuturedatedTermination = "FuturedatedTermination";
            public const string Terminated = "Terminated";
            //public const string TerminationFailed = "TerminationFailed";
            //public const string TerminationValidationFailed = "ValidationFailedInTermination";
        }

        public class PUBFileSource
        {
            public const string IDMS = "IDMS";
            public const string PUB = "PUB";
        }

        public class PUBFileStatus
        {
            public const string FileReceived = "FileReceived";
            public const string FileSent = "FileSent";
            public const string FileProcessing = "FileProcessing";
            public const string FileError = "FileError";
            public const string FileSaved = "FileSaved";
            public const string FileProcessCompleted = "FileProcessCompleted";
            public const string TerminationInProgress = "TerminationInProgress";
            public const string FileYetToBeReported = "FileYetToBeReported";
            public const string FileReported = "FileReported";
        }

        public class PUBFileType
        {
            public const string CompleteFile = "Monthly";
            public const string ProcessFile = "Daily";
            public const string ReportFile = "ReportFile";
        }

        public class FileNames
        {
            public const string Process1_FileName = "IDMS_TO_PUB";
            public const string Process2_FileName = "PUB_TO_IDMS";
            public const string Process3_FileName = "IDMS_TO_PUB_Processed";
        }

        public enum BackUpDeleteStrategy : byte
        {
            Delete=0,
            BackUp ,
        }

        public class PUB_ValidationMsg
        {
            public const string InvalidEBSUsername = "Invalid EBS Username. ";
            public const string InvalidEBSUserId = "Invalid EBSUserId. ";
            public const string Company_Name_is_not_PUB = "Company Name is not PUB. ";
            public const string InvalidDate = "Invalid Date. ";
            public const string InactiveEBSUserID = "Inactive EBS User ID. ";
            public const string FutureTerminationDateMsg = "Access will be terminated on ValidTillDate. Proposed TerminationDate:[{0}]. ";
            public const string TerminationDateIsPastDate = "Termination Date is Past Date. Proposed TerminationDate:[{0}]. ";



        }
    }
}
