using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SODEndPointPolicySelection : DomainModelBase<SODEndPointPolicySelection, EntityModel.SODEndPointPolicySelection>
    {
        public long SODPolicyExceutionId { get; set; }

        public string EndpointTypeName { get; set; }

        public string LevelName { get; set; }

        public string LevelValue { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public  SODPolicyExecutionDefinition SODPolicyExecutionDefinition { get; set; }

        public static void DeleteSODEndPointPolicySelection(long sodPolicyExecId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                SODEndPointPolicySelection sodEndPointPolicySelection = SODEndPointPolicySelection.GetSODEndpointPolicySelection(sodPolicyExecId);
                try
                {
                    sodEndPointPolicySelection.Delete(dataAccessTransaction);
                }
                catch (System.Exception ex)
                {
                   
                }

                dataAccessTransaction.Commit();
            }
        }

        public static SODEndPointPolicySelection GetSODEndpointPolicySelection(long SODPolicyExecutionId)
        {
            using (DataAccessTransaction dataAccessTransation = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SODEndPointPolicySelection>(dataAccessTransation).Get(x => x.SODPolicyExceutionId == SODPolicyExecutionId).FirstOrDefault());
            }
        }
    }
}
