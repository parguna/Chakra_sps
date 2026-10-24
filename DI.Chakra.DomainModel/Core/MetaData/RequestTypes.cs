using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class RequestTypes : DomainModelBase<RequestTypes, EntityModel.RequestTypes> 
    {
        public string RequestTypeDesc { get; set; }

        public bool IsActive { get; set; }

        public List<WorkflowConfiguration> WorkflowConfigurations { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static List<RequestTypes> GetByIsActive() 
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.RequestTypes>(dataAccessTransaction)
                    .Get(roleTypes => roleTypes.IsActive).ToList());
            }
        }
    }
}
