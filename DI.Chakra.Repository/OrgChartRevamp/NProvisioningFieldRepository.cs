using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Repository
{
    public class NProvisioningFieldRepository : RepositoryBase<NProvisioningField>
    {
        public NProvisioningFieldRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public List<NProvisioningField> GetAllProvisioningFieldForEndpointAndAccountType(long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction())
            {
                List<NProvisioningField> provisioningFieldsForEpAndAccountType = new List<NProvisioningField>();
                var provisioningFieldsForAccountType = CurrentContext.NProvisioningFields.Where(a => a.EndPointSystemId == endpointSystemId && a.AccountType == accountType && a.IsActive);

                if (provisioningFieldsForAccountType.Any())
                {
                    provisioningFieldsForEpAndAccountType.AddRange(provisioningFieldsForAccountType);
                    //provisioningFieldsForEpAndAccountType.AddRange((from b in provisioningFieldsForAccountType
                    //                                                select new
                    //                                      {
                    //                                          EventType = b.EventType,
                    //                                          SubEventType = b.SubEventType,
                    //                                          ProvisioningEndpointProcessName = b.ProvisioningEndpointProcessName,
                    //                                          ProvisioningCategoryType = b.ProvisioningCategoryType,
                    //                                          ProvisioningCategory = b.ProvisioningCategory,
                    //                                          ProvisioningAction = b.ProvisioningAction,
                    //                                          Order = b.Order,
                    //                                          BreakWhenFailed = b.BreakWhenFailed,
                    //                                          IsActive = b.IsActive,
                    //                                      }).ToList().Select(c => new NProvisioningEventProcess
                    //                                      {
                    //                                          EventType = c.EventType,
                    //                                          SubEventType = c.SubEventType,
                    //                                          ProvisioningEndpointProcessName = c.ProvisioningEndpointProcessName,
                    //                                          ProvisioningCategoryType = c.ProvisioningCategoryType,
                    //                                          ProvisioningCategory = c.ProvisioningCategory,
                    //                                          ProvisioningAction = c.ProvisioningAction,
                    //                                          Order = c.Order,
                    //                                          BreakWhenFailed = c.BreakWhenFailed,
                    //                                          IsActive = c.IsActive,
                    //                                      }).ToList());
                }

                provisioningFieldsForEpAndAccountType.AddRange(CurrentContext.NProvisioningFields.Where(a => a.EndPointSystemId == endpointSystemId && a.AccountType == AccountType.None && provisioningFieldsForAccountType.All(b => b.FieldName != a.FieldName) && a.IsActive));
                return provisioningFieldsForEpAndAccountType.ToList();
            }
        }

        public List<NProvisioningField> GetAllProvisioningFieldForEndpointTypeAndAccountType(long endpointSystemTypeId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction())
            {
                List<NProvisioningField> provisioningFieldsForEpAndAccountType = new List<NProvisioningField>();
                var provisioningFieldsForAccountType = CurrentContext.NProvisioningFields.Where(a => a.EndPointSystemTypeId == endpointSystemTypeId && a.AccountType == accountType && a.IsActive);

                if (provisioningFieldsForAccountType.Any())
                {
                    provisioningFieldsForEpAndAccountType.AddRange(provisioningFieldsForAccountType);
                    //provisioningFieldsForEpAndAccountType.AddRange((from b in provisioningFieldsForAccountType
                    //                                                select new
                    //                                      {
                    //                                          EventType = b.EventType,
                    //                                          SubEventType = b.SubEventType,
                    //                                          ProvisioningEndpointProcessName = b.ProvisioningEndpointProcessName,
                    //                                          ProvisioningCategoryType = b.ProvisioningCategoryType,
                    //                                          ProvisioningCategory = b.ProvisioningCategory,
                    //                                          ProvisioningAction = b.ProvisioningAction,
                    //                                          Order = b.Order,
                    //                                          BreakWhenFailed = b.BreakWhenFailed,
                    //                                          IsActive = b.IsActive,
                    //                                      }).ToList().Select(c => new NProvisioningEventProcess
                    //                                      {
                    //                                          EventType = c.EventType,
                    //                                          SubEventType = c.SubEventType,
                    //                                          ProvisioningEndpointProcessName = c.ProvisioningEndpointProcessName,
                    //                                          ProvisioningCategoryType = c.ProvisioningCategoryType,
                    //                                          ProvisioningCategory = c.ProvisioningCategory,
                    //                                          ProvisioningAction = c.ProvisioningAction,
                    //                                          Order = c.Order,
                    //                                          BreakWhenFailed = c.BreakWhenFailed,
                    //                                          IsActive = c.IsActive,
                    //                                      }).ToList());
                }

                provisioningFieldsForEpAndAccountType.AddRange(CurrentContext.NProvisioningFields.Where(a => a.EndPointSystemTypeId == endpointSystemTypeId && a.AccountType == AccountType.None && provisioningFieldsForAccountType.All(b => b.FieldName != a.FieldName) && a.IsActive));
                return provisioningFieldsForEpAndAccountType.ToList();
            }
        }
    }
}
