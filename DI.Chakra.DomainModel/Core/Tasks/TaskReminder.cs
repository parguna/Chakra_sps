using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class TaskReminder : DomainModelBase<TaskReminder, EntityModel.TaskReminder>
    {
        public long TaskId { get; set; }
        public DI.Chakra.Infrastructure.Enums.RequestType RequestType { get; set; }
        public int ReminderCount { get; set; }
        public bool IsEsclation { get; set; } = false;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Task Task { get; set; }

        public static TaskReminder GetByTaskId(long taskId)
        {
            using(var tran = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.TaskReminder>(tran).Get(i => i.TaskId == taskId).FirstOrDefault());
            }
        }
    }
}
