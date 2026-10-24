using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class ApprovalUserTypes : DomainModelBase<ApprovalUserTypes, EntityModel.ApprovalUserTypes> 
    {
        public string ApprovalUserTypeDesc { get; set; }

        public string ApplicableWorkFlowTypes { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static List<ApprovalUserTypes> GetByIsActive()    
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ApprovalUserTypes>(dataAccessTransaction)
                    .Get(accountcategory => accountcategory.IsActive).ToList());
            }
        }
    }
}
