using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel.Core;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;

namespace DI.Chakra.DomainModel
{ 
    public class PUBFile : DomainModelBase<DomainModel.PUBFile, EntityModel.PUBFile>
    {        
        public Nullable<DateTime> FileDateTime { get; set; }
        public string PUBFileStatus { get; set; }
        public string PUBFileSource { get; set; }
        public string PUBFileType { get; set; }
        public string ValidationMsg { get; set; }
        public string Remarks { get; set; }
        public string AdditionalInfo { get; set; }

        public Nullable<DateTime> ReportedDateTime { get; set; }
        public string ReportedStatus { get; set; }
        public string FileName { get; set; }
        public string ReportFileName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }


        public static List<PUBFile> GetFilesBySourceAndStatus(string source,string[] status)
        {
            return PUBFile.Get(t => t.PUBFileSource.Trim().Equals(source, StringComparison.InvariantCultureIgnoreCase) 
                                     && status.Contains(t.PUBFileStatus) ,false,0,0);
        }

        public static List<EntityModel.Attachment> GetAttachmentsBySourceAndStatus(string source, string[] status)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new PUBFileRepository(dataAccessTransaction).GetAttachmentsBySourceAndStatus(source, status);
            }
        }

        public static PresentationGridPagedData<PUBFile> GetLazyLoadPUBFiles(int startRowIndex, int maximumRows, string filterExpression, string sortExpression)
        {
            var result = new PresentationGridPagedData<PUBFile>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var dbResult = new PUBFileRepository(dataAccessTransaction).GetLazyLoadPUBFiles(startRowIndex, maximumRows, filterExpression, sortExpression);
                result.Data = ORMapList(dbResult.Data);
                result.TotalCount = dbResult.TotalCount;
                return result;
            }
        }


        public static int NoOfFilesFortheDay(string filesource, string  fileType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new PUBFileRepository(dataAccessTransaction).NoOfFilesFortheDay(filesource, fileType);
            }
        }

    }
}
