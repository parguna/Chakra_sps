using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ExceptionAttribute : DomainModelBase<ExceptionAttribute, EntityModel.ExceptionAttribute>
    {
        public long EndpointSystemTypeId { get; set; }

        public string Attributes { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public static List<ExceptionAttribute> GetByEndpointTypeId(long endpointTypeId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ExceptionAttribute>(dataAccessTransaction).
                    Get(ea=>ea.EndpointSystemTypeId == endpointTypeId).ToList());
            }
        }
        public static ExceptionAttribute GetByEndpointTypeIdandAttribute(long endpointTypeId, string attribute)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ExceptionAttribute>(dataAccessTransaction).
                    Get(ea => ea.EndpointSystemTypeId == endpointTypeId && ea.Attributes == attribute).FirstOrDefault());
            }
        }
        public static void SaveExceptionAttribute(List<ExceptionAttribute> AttributeList, string currentUser)
        {
            using (var transaction = new DataAccessTransaction(currentUser))
            {
                long endpointTypeId = AttributeList.FirstOrDefault().EndpointSystemTypeId;
                List<ExceptionAttribute> dbExceptionAttributeList = ExceptionAttribute.GetByEndpointTypeId(endpointTypeId);
                //dbProvisioningRoleDetails.Except(this.ProvisioningRoleDetails, new IdBaseEqualityComparer<ProvisioningRoleDetail>()).ToList();
                if (dbExceptionAttributeList.Count > 0)
                {
                    List<ExceptionAttribute> removedAttributeList = dbExceptionAttributeList.Except(AttributeList, new IdBaseEqualityComparer<ExceptionAttribute>()).ToList();
                    ExceptionAttribute.DeleteList(transaction, removedAttributeList);
                }
                ExceptionAttribute.SaveList(transaction, AttributeList);
                transaction.Commit();
            }
        }
    }
}
