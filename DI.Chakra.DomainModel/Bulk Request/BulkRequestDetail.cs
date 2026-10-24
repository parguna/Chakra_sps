using DI.Chakra.Infrastructure;
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
   [Serializable]
    public partial class BulkRequestDetail : DomainModelBase<BulkRequestDetail, EntityModel.BulkRequestDetail>
    {

        public long BulkRequestId { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string EmployeeId { get; set; }
        public string EmailId { get; set; }
        public string Designation { get; set; }
        public string OrgCompany { get; set; }
        public string OrgDepartment { get; set; }
        public string OrgDivision { get; set; }
        public string OrgBranch { get; set; }
        public string OrgSection { get; set; }
        public string OrgUnit { get; set; }
        public string OneKey { get; set; }

        public string AccountName { get; set; }
        public long? EndpointUserId { get; set; }

        public string HandPhone { get; set; }
        public override long Save(string currentUser)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                if (this.Id == 0)
                {
                    this.Id = new RepositoryBase<EntityModel.BulkRequestDetail>(transaction).Insert(ORUnMap());
                }
                else
                {
                    new RepositoryBase<EntityModel.BulkRequestDetail>(transaction).Update(ORUnMap());
                }

                transaction.Commit();
            }
            return 0;
        }
    }
}
