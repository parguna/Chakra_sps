using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class RoleTypes : DomainModelBase<RoleTypes, EntityModel.RoleTypes> 
    {
        public string DisplayName { get; set; }
        public string RoleTypeDesc { get; set; }

        public string ApplicableApprovalUserTypes { get; set; }

        public string ApplicableRequestTypes { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static List<RoleTypes> GetByIsActive()   
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.RoleTypes>(dataAccessTransaction)
                    .Get(roleTypes => roleTypes.IsActive).ToList());
            }
        }
    }
}
