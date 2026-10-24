using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ProcessHistory :DomainModelBase<ProcessHistory,EntityModel.ProcessHistory>
    {
        public ProcessType ProcessType { get; set; }

        public long ProcessId { get; set; }

        public Nullable<DateTime> ProcessStartAt { get; set; }

        public Nullable<DateTime> ProcessEndAt { get; set; }

        public string Status { get; set; }

        public string ErrorMessage { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public Process Process { get; set; }

        public static ProcessHistory GetByProcessType(long ProcessId,ProcessType processType,string status)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.ProcessHistory>(dataAccessTransaction)
                    .Get(item => item.ProcessId==ProcessId&&item.ProcessType==processType&&item.Status==status).FirstOrDefault());
            }
        }

        public static List<ProcessHistory> GetAllProcessHistory(long processId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.ProcessHistory>(dataAccessTransaction).Get(item => item.ProcessId == processId).ToList());
            }
        }

    }
}
