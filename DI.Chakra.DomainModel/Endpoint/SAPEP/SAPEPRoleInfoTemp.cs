using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPEPRoleInfoTemp : DomainModelBase<SAPEPRoleInfoTemp, EntityModel.SAPEPRoleInfoTemp>
    {
        public string RoleUniqueName { get; set; }

        public string RoleFullName { get; set; }

        public int? Version { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


        public static Dictionary<string, string> GetSAPEPRoleUniqueNamesDictionary(long endPointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {

                return new SAPEPRoleInfoTempRepository(dataAccessTransaction).GetSAPEPRoleUniqueNamesDictionary(endPointId);

            }
        }
    }
}
