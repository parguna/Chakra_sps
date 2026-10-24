using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class EndpointHierarchy : DomainModelBase<EndpointHierarchy, EntityModel.EndpointHierarchy>
    {        
        public EndpointType EndpointType { get; set; }

        public Nullable<long> EndpointId { get; set; }

        public EntitlementType EntitlementType { get; set; }

        public long EndpointObjectId { get; set; }
        public bool IsActionable { get; set; }
        public bool IsAssociatedWithUser { get; set; }
        public Nullable<long> ParentId { get; set; }
        public long Level { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static List<EndpointHierarchy> GetEndpointHierarchy(long endpointId, AttestationType attestationType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<EndpointHierarchy> endpointHierarchy = new List<EndpointHierarchy>();
                endpointHierarchy = ORMapList(new EndpointHierarchyRepository(dataAccessTransaction).Get(a => a.EndpointId == endpointId && a.IsActive, true).ToList());
                if(endpointHierarchy == null || endpointHierarchy.Count() == 0)
                {
                    var endpoint = EndPoint.GetById(endpointId, true);
                    if(endpoint != null)
                        endpointHierarchy = getEndpointTypeHierarchy(endpoint.EndPointSystemType.EndpointSystemTypeName, endpointId);
                }

                endpointHierarchy = endpointHierarchy.OrderBy(a => a.Level).ToList();

                return endpointHierarchy;
            }
        }

        public static List<EndpointHierarchy> GetEndpointHierarchyByLevel(long endpointId, long level)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<EndpointHierarchy> endpointHierarchy = new List<EndpointHierarchy>();
                endpointHierarchy = ORMapList(new EndpointHierarchyRepository(dataAccessTransaction).Get(a => a.EndpointId == endpointId && a.Level == level && a.IsActive, true).ToList());                
                return endpointHierarchy;
            }
        }

        public static EndpointHierarchy GetEndpointSystemIdHierarchyByLevel(long endpointSystemId, long level)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                EndpointHierarchy endpointHierarchy = new EndpointHierarchy();
                endpointHierarchy = ORMap(new EndpointHierarchyRepository(dataAccessTransaction).Get(a => a.EndpointType == (EndpointType)endpointSystemId && a.Level == level && a.IsActive, true).FirstOrDefault());
                return endpointHierarchy;
            }
        }

        public static List<EndpointHierarchy> GetEndpointHierarchyByEntitlementType(long endpointId, List<EntitlementType> entitlementType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<EndpointHierarchy> endpointHierarchy = new List<EndpointHierarchy>();
                endpointHierarchy = ORMapList(new EndpointHierarchyRepository(dataAccessTransaction).Get(a => a.EndpointId == endpointId 
                && entitlementType.Contains(a.EntitlementType)
                && a.IsActive, true).ToList());                
                return endpointHierarchy;
            }
        }

        private static List<EndpointHierarchy> getEndpointTypeHierarchy(EndpointType endpointType, long endpointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<EndpointHierarchy> endpointTypeHierarchy = new List<EndpointHierarchy>();
                endpointTypeHierarchy = ORMapList(new EndpointHierarchyRepository(dataAccessTransaction).Get(a => a.EndpointType == endpointType && a.IsActive, true).ToList());
                if (endpointTypeHierarchy == null || endpointTypeHierarchy.Count() == 0)
                {
                    DILog.AttestationLogger.WriteLog(LogLevelType.Error, string.Format("Hierarchy configuration not found for Endpoint Id {0} and EndpointType {1}", endpointId, endpointType.ToString()));
                    var parentId = 0; var level = 1;
                    //manually create the first 2 level of hierarchy
                    endpointTypeHierarchy.Add(new EndpointHierarchy {
                        EndpointType = endpointType,
                        EndpointId = endpointId,
                        EntitlementType = EntitlementType.Account,
                        IsActive = true,
                        IsActionable = true,
                        Id = parentId,
                        EndpointObjectId = 0,
                        Level = level,
                    });

                    var entitlementType = EndPointFactory.GetEndPoint(endpointType).GetEntitlementList().Distinct().ToList();
                    level = level+1;
                    foreach (var entitlement in entitlementType)
                    {
                        endpointTypeHierarchy.Add(new EndpointHierarchy
                        {
                            EndpointType = endpointType,
                            EndpointId = endpointId,
                            EntitlementType = entitlement,
                            IsActive = true,
                            IsActionable = true,
                            ParentId = parentId,
                            EndpointObjectId = 0,
                            Level = level,
                        });
                    }
                }
                return endpointTypeHierarchy;
            }
        }
    }
}
