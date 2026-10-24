using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
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
    public class AttestationHierarchy : DomainModelBase<AttestationHierarchy, EntityModel.AttestationHierarchy>
    {
        public Nullable<long> AttestationId { get; set; }
        public Nullable<long> EndpointHierarchyId { get; set; }
        public long EndpointId { get; set; }
        
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }
        public bool IsActionAllowed { get; set; }
        public AttestationDefaultReviewStatus DefaultAttestationAction { get; set; }
        public bool IsActionExtendable { get; set; }
        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public EndpointHierarchy EndpointHierarchy { get; set; }

        public static List<AttestationHierarchy> GetAttestationHierarchy(long attestationId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                var attestation = Attestation.GetById(attestationId);
                var attestationEndpoints = AttestationEndpoint.GetByAttestationId(attestationId);
                List<AttestationHierarchy> attestationHierarchy = new List<AttestationHierarchy>();
                var attestationAllEndpointHierarchy = ORMapList(new AttestationHierarchyRepository(dataAccessTransaction).Get(a => a.AttestationId == attestationId && a.IsActive, true).ToList());

                foreach (var attestationEndpoint in attestationEndpoints)
                {
                    List<AttestationHierarchy> attestationEndpointHierarchy = new List<AttestationHierarchy>();
                    attestationEndpointHierarchy = attestationAllEndpointHierarchy.Where(a => a.EndpointId == attestationEndpoint.EndpointSystemId).ToList();
                    if (attestationEndpointHierarchy.Count() == 0)
                        attestationEndpointHierarchy.AddRange(attestationAllEndpointHierarchy.Where(a => a.EndpointHierarchy.EndpointType == attestationEndpoint.EndpointSystemType).ToList());
                    if (attestationEndpointHierarchy.Count() == 0)
                    {
                        var endpointHierarchy = EndpointHierarchy.GetEndpointHierarchy(attestationEndpoint.EndpointSystemId,
                            attestation.AttestationType).ToList();
                        attestationEndpointHierarchy.AddRange((from epHierarchy in endpointHierarchy
                                                               select new AttestationHierarchy
                                                               {
                                                                   AttestationId = attestationId,
                                                                   EndpointHierarchyId = 0,
                                                                   EndpointHierarchy = epHierarchy,
                                                                   IsActive = epHierarchy.IsActive,
                                                                   IsVisible = true,
                                                                   IsActionAllowed = epHierarchy.IsActionable,
                                                                   IsActionExtendable = false,
                                                               }).ToList());
                    }

                    attestationEndpointHierarchy.ForEach(a =>
                    {
                        a.EndpointHierarchy.EndpointId = attestationEndpoint.EndpointSystemId;
                        a.EndpointId = attestationEndpoint.EndpointSystemId;
                    });
                    attestationHierarchy.AddRange(attestationEndpointHierarchy);
                }

                attestationHierarchy = attestationHierarchy.OrderBy(a => a.EndpointHierarchy.Level).ToList();
                return attestationHierarchy;
            }
        }

        public static List<AttestationHierarchy> GetAttestationHierarchy(long attestationId, long endpointId)
        {
                var attestation = Attestation.GetById(attestationId);
                var attestationEndpoints = AttestationEndpoint.GetByAttestationId(attestationId).Where(a => a.EndpointSystemId == endpointId).ToList();
                List<AttestationHierarchy> attestationHierarchy = new List<AttestationHierarchy>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                var attestationAllEndpointHierarchy = ORMapList(new AttestationHierarchyRepository(dataAccessTransaction).Get(a => a.AttestationId == attestationId && a.IsActive, true).ToList());

                foreach (var attestationEndpoint in attestationEndpoints)
                {
                    List<AttestationHierarchy> attestationEndpointHierarchy = new List<AttestationHierarchy>();
                    attestationEndpointHierarchy = attestationAllEndpointHierarchy.Where(a => a.EndpointHierarchy.EndpointId == attestationEndpoint.EndpointSystemId).ToList();
                    if (attestationEndpointHierarchy.Count() == 0)
                        attestationEndpointHierarchy.AddRange(attestationAllEndpointHierarchy.Where(a => a.EndpointHierarchy.EndpointType == attestationEndpoint.EndpointSystemType).ToList());
                    if (attestationEndpointHierarchy.Count() == 0)
                    {
                        var endpointHierarchy = EndpointHierarchy.GetEndpointHierarchy(attestationEndpoint.EndpointSystemId, attestation.AttestationType).ToList();
                        attestationEndpointHierarchy.AddRange((from epHierarchy in endpointHierarchy
                                                               select new AttestationHierarchy
                                                               {
                                                                   AttestationId = attestationId,
                                                                   EndpointHierarchyId = 0,
                                                                   EndpointHierarchy = epHierarchy,
                                                                   IsActive = epHierarchy.IsActive,
                                                                   IsVisible = true,
                                                                   IsActionAllowed = epHierarchy.IsActionable,
                                                                   IsActionExtendable = false,
                                                               }).ToList());
                    }

                    attestationEndpointHierarchy.ForEach(a => a.EndpointHierarchy.EndpointId = attestationEndpoint.EndpointSystemId);
                    attestationHierarchy.AddRange(attestationEndpointHierarchy);
                }

                attestationHierarchy = attestationHierarchy.OrderBy(a => a.EndpointHierarchy.Level).ToList();
                return attestationHierarchy;
            }
        }

        public static IList<AttestationHierarchy> GetByAttestationId(long attestationId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AttestationHierarchy>(dataAccessTransaction)
                    .Get(attatn => attatn.AttestationId == attestationId).ToList());
            }
        }
    }
}
