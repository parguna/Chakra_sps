using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DI.Chakra.DomainModel
{
    public class AccountActionCategory : DomainModelBase<AccountActionCategory, EntityModel.AccountActionCategory> 
    {
        public string AccountActionTypeDesc { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static List<AccountActionCategory> GetByIsActive()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountActionCategory>(dataAccessTransaction)
                      .Get(accountaction => accountaction.IsActive).ToList());
            }
        }
    }
}
