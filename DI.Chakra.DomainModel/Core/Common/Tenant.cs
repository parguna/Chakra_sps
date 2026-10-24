using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class Tenant : DomainModelBase<Tenant, EntityModel.Tenant>
    {
        public string TenantId { get; set; }

        public string TenantName { get; set; }

        public string TenantDescription { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> CreateDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static bool DoesExists(string tenantName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.Tenant>(dataAccessTransaction).Any(tenant => tenant.TenantName == tenantName);
            }
        }
        public static string Name()
        {

            using (var dataAccessTransaction = new DataAccessTransaction())
            {

                var list = new RepositoryBase<EntityModel.Tenant>(dataAccessTransaction).GetAll();

                if (list.Count() <= 0)
                    return string.Empty;

                return list.FirstOrDefault().TenantName;
            }

        }
    }
}
