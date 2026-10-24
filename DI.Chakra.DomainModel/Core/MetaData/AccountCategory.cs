using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class AccountCategory : DomainModelBase<AccountCategory, EntityModel.AccountCategory> 
    {
        public string AccountTypeDesc { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static List<AccountCategory> GetByIsActive()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountCategory>(dataAccessTransaction)
                       .Get(accountcategory => accountcategory.IsActive).ToList());
            }
        }
    }
}
