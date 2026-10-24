using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using DI.Chakra.Logger;

namespace DI.Chakra.Repository 
{
   public class PUBFileRepository : RepositoryBase<PUBFile>
    {
        public PUBFileRepository(DataAccessTransaction transaction) : base(transaction) { }


        public  List<Attachment> GetAttachmentsBySourceAndStatus(string source, string[] pendingstatus)
        { 
            return (from attachment in this.CurrentContext.Attachments join
            pubFile in this.CurrentContext.PUBFile on attachment.AccessRequestId equals pubFile.Id
                    where pendingstatus.Contains(pubFile.PUBFileStatus) select attachment ).ToList(); 

        }


        public PresentationGridPagedData<PUBFile> GetLazyLoadPUBFiles(int startRowIndex, int maximumRows, string filterExpression, string sortExpression)
        {
            var result = new PresentationGridPagedData<PUBFile>();

            IQueryable<PUBFile> query = this.CurrentContext.PUBFile.Select(p=>p);

            if (!string.IsNullOrWhiteSpace(filterExpression))
            {
                query = query.Where(filterExpression);
            }

            result.TotalCount = query.Count();

            if (!string.IsNullOrWhiteSpace(sortExpression))
            {
                query = query.OrderBy(sortExpression);
            }
            else
            {
                query = query.OrderByDescending(p => p.FileDateTime);
            }


            if (maximumRows == -1)
                result.Data = query.ToList();
            else
            {
                query = query.Skip(startRowIndex).Take(maximumRows);
                result.Data = query.ToList(); }

            return result;
        }

        public int NoOfFilesFortheDay(string filesource, string fileType)
        {
            var query = $@" select  count(1)  from DI_PUB_File where PUBFileSource = '{filesource}'
                                            and PUBFileType = '{fileType}' and CAST(FileDateTime AS DATE)= CAST(GETDATE() as DATE) ";

            return this.CurrentContext.Database.SqlQuery<int>(query).FirstOrDefault();

        }



    }
}
