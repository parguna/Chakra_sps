using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DI.Chakra.DomainModel
{
    public class EmployeeTerm : DomainModelBase<EmployeeTerm, EntityModel.EmployeeTerm> 
    {
        public string EmployeeTermDesc { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static List<EmployeeTerm> GetByIsActive()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.EmployeeTerm>(dataAccessTransaction)
                       .Get(roleTypes => roleTypes.IsActive).ToList());
            }
        }
    }
}
