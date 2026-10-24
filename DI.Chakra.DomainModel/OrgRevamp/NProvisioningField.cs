using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public partial class NProvisioningField : DomainModelBase<NProvisioningField, EntityModel.NProvisioningField>
    {

        public long EndPointSystemTypeId { get; set; }

        public long EndPointSystemId { get; set; }

        public AccountType AccountType { get; set; }

        public string FieldName { get; set; }

        public string Description { get; set; }

        public string Value { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public EndPoint EndPoint { get; set; }
        public EndPointSystemType EndpointSystemType { get; set; }

        public override string ToString()
        {
            if (FieldName != null && !string.IsNullOrEmpty(Value))
                return string.Format("{0}-{1}", FieldName, Value);

            return Value;
        }
        public string ValueDisplayName { get; set; }

        public static bool DoesExists(string fieldName, Nullable<AccountType> accountType, long endPointSystemId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.NProvisioningField>(tranaction).
                    Any(provField => provField.FieldName == fieldName && provField.AccountType == accountType
                        && provField.EndPointSystemId == endPointSystemId);
            }
        }

        public static NProvisioningField GetByFieldName(long endPointSystemId, string fieldName)
        {
            NProvisioningField provisioningField = null;

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                provisioningField = ORMap(new NProvisioningFieldRepository(dataAccessTransaction).
                    Get(provField => provField.EndPointSystemId == endPointSystemId && provField.FieldName == fieldName, true).FirstOrDefault());

                return provisioningField;

            }
        }

        public static NProvisioningField GetByFieldNameAndAccountType(long endPointSystemId, string fieldName, AccountType accountType)
        {
            NProvisioningField provisioningField = null;

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                provisioningField = ORMap(new NProvisioningFieldRepository(dataAccessTransaction).
                    Get(provField => provField.EndPointSystemId == endPointSystemId && provField.FieldName == fieldName && provField.AccountType == accountType).FirstOrDefault());

                return provisioningField;

            }
        }
        public static List<NProvisioningField> GetByFieldNamesAndAccountType(long endPointSystemId, string[] fieldNames, AccountType accountType)
        {
            List<NProvisioningField> provisioningField = null;
            using(DataAccessTransaction tran = new DataAccessTransaction(false))
            {
                provisioningField = ORMapList(new NProvisioningFieldRepository(tran).
                Get(provField => provField.EndPointSystemId == endPointSystemId && fieldNames.Contains(provField.FieldName) && provField.AccountType == accountType).ToList());
            }
            return provisioningField;
        }
        public static bool HasAnyByEndPointsystemId(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.NProvisioningField>(dataAccessTransaction).
                       Any(provField => provField.EndPointSystemId == endPointSystemId);
            }
        }

        public static void DeleteByEndPointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<NProvisioningField> provFields = new List<NProvisioningField>();
                provFields = ORMapList(new NProvisioningFieldRepository(dataAccessTransaction).
                                            Get(provField => provField.EndPointSystemId == endPointSystemId, true).ToList());

                NProvisioningField.DeleteList(dataAccessTransaction, provFields);
                dataAccessTransaction.Commit();
            }
        }

        public static List<NProvisioningField> GetByEndPointSystemIncludingValueDisplayName(long endPointSystemTypeId, long? endPointSystemId)
        {
            List<NProvisioningField> result = new List<NProvisioningField>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<NProvisioningField> list = ORMapList(new NProvisioningFieldRepository(dataAccessTransaction)
                                                            .Get(provField =>
                                                                provField.EndPointSystemTypeId == endPointSystemTypeId &&
                                                                   provField.EndPointSystemId == endPointSystemId, true).ToList());
                foreach (NProvisioningField provField in list)
                {
                    //int intvalue;
                    //if (provField.ResolverType == DI.Chakra.Infrastructure.Enums.ResolverType.PasswordResolver.ToString())
                    //{
                    //    Dictionary<long, string> policyList = PasswordPolicy.GetPasswordPolicies("*");
                    //    if (int.TryParse(provField.Value, out intvalue))
                    //    {
                    //        var policyName = policyList.FirstOrDefault(x => x.Key == Convert.ToInt64(provField.Value)).Value;
                    //        if (policyName != null)
                    //            provField.ValueDisplayName = policyName;
                    //    }
                    //}
                    //else if (provField.ResolverType == DI.Chakra.Infrastructure.Enums.ResolverType.RuleSetResolver.ToString())
                    //{
                    //    //Dictionary<long, string> nameConstructorList = ProvisioningNameConstructor.GetAllProvisioningFieldBySearchString("*");
                    //    //if (int.TryParse(provField.Value, out intvalue))
                    //    //{
                    //    //    var constructorName = nameConstructorList.FirstOrDefault(x => x.Key == Convert.ToInt64(provField.Value)).Value;
                    //    //    if (constructorName != null)
                    //    //        provField.ValueDisplayName = constructorName;
                    //    //}
                    //}
                    //else
                    //{
                        provField.ValueDisplayName = provField.Value;
                    //}
                    result.Add(provField);
                }
            }
            return result;
        }

        public static List<NProvisioningField> GetByEndpointSystemId(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<NProvisioningField> provFields = new List<NProvisioningField>();
                provFields = ORMapList(new NProvisioningFieldRepository(dataAccessTransaction).
                                            Get(provField => provField.EndPointSystemId == endpointSystemId, true).ToList());

                return provFields;
            }
        }
        public static NProvisioningField GetByEndpointSystemIdAndName(long endpointSystemId,string name)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var provField = ORMap(new RepositoryBase<EntityModel.NProvisioningField>(dataAccessTransaction).Get(x => x.EndPointSystemId == endpointSystemId && x.FieldName == name).FirstOrDefault());
                return provField;
            }
        }
        
        public static List<NProvisioningField> GetByEndpointSystemId(long endpointSystemId, AccountType accountType)
        {
            List<NProvisioningField> provisioningFieldList = GetByEndpointSystemId(endpointSystemId);

            List<NProvisioningField> filteredProvisioningField = provisioningFieldList.Where(pf => pf.AccountType == accountType).ToList();

            filteredProvisioningField
                .AddRange(provisioningFieldList.Where(pf => !filteredProvisioningField.Any(pf1 => pf1.FieldName == pf.FieldName)));

            filteredProvisioningField = filteredProvisioningField.Where(pf => pf.AccountType == accountType).ToList();

            filteredProvisioningField = filteredProvisioningField.Where(pf => pf.IsActive).ToList();

            return filteredProvisioningField;
        }

        public static ProvisioningParamCollection GetProvisioningDefaults(long endpointSystemId, AccountType accountType)
        {
            ProvisioningParamCollection provCol = new ProvisioningParamCollection();
            var provFields = GetByEndpointSystemId(endpointSystemId, accountType);

            var provs = provFields.Select(t => new ProvisioningParam
            {
                Name = t.FieldName,
                Value = t.Value
            }).ToList();

            provCol.AddAll(provs);

            return provCol;
        }
    }
}
