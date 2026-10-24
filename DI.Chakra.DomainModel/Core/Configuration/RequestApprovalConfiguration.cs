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
    public class RequestApprovalConfiguration : DomainModelBase<RequestApprovalConfiguration, EntityModel.RequestApprovalConfiguration>
    {
        public AccessRequestSource RequestSource { get; set; }

        public bool IsApprovalRequired { get; set; }

        public Nullable<long> WorkflowConfigurationId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public WorkflowConfiguration WorkflowConfiguration { get; set; }

        public static RequestApprovalConfiguration GetByRequestSource(DI.Chakra.Infrastructure.Enums.AccessRequestSource requestSource)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.RequestApprovalConfiguration>(dataAccessTransaction).
                    Get(reqAppConfig => reqAppConfig.RequestSource == requestSource).FirstOrDefault());
            }
        }


        public static bool DoesApprovalRequired(AccessRequestSource requestSource)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return (new RepositoryBase<EntityModel.RequestApprovalConfiguration>(dataAccessTransaction).
                    Get(reqAppConfig => reqAppConfig.RequestSource == requestSource).Any());
            }
        }
    }
}
