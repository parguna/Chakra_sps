using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class Attachment : DomainModelBase<Attachment, EntityModel.Attachment>
    {
        public long RequestTypeId { get; set; }

        public Nullable<long> TaskId { get; set; }

        public long AccessRequestId { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }

        public byte[] AttachmentFile { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }
        public string AccessRequestSource { get; set; }
        public static List<Attachment> GetByRequestId(long requestId,string source)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.Attachment>(dataAccessTransaction)
                    .Get(a => a.AccessRequestId == requestId && a.FileName == source).ToList());
            }
        }

        public static long GetAttachmentCount(long taskId, string source)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return (new RepositoryBase<EntityModel.Attachment>(dataAccessTransaction)
                    .Count(a => a.TaskId == taskId && a.AccessRequestSource == source));
            }
        }
    }
}
