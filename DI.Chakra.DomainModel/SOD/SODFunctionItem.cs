using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SODFunctionItem : DomainModelBase<SODFunctionItem, EntityModel.SODFunctionItem>
    {
        public Nullable<long> SodFunctionId { get; set; }

        public string SodFunctionType { get; set; }

        public string SodRoleId { get; set; }

        public string SodRoleDesc { get; set; }

        public string ObjectCode { get; set; }

        public string FieldCode { get; set; }

        public string FromValue { get; set; }

        public Nullable<long> EndpointSystemTypeId { get; set; }

        public Nullable<long> Endpointsystemid { get; set; }

        public EndPointSystemType EndpointType { get; set; }

        public EndPoint endpoint { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static Dictionary<long, string> GetBySearchString(string inputParams, string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SODFunctionItemRepository(dataAccessTransaction).
                    GetBySearchString(inputParams, CheckNull.AsteriskToString(searchString));
            }
        }

        public static SODFunctionItem GetByIdWithNavigation(long functionItemValue)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SODFunctionItem>(transaction)
                   .Get(functionitem => functionitem.Id == functionItemValue,
                   functionItem => functionItem.EndpointType,
                   functionItem => functionItem.endpoint).FirstOrDefault());
            }
        }
    }
}
