
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DI.Chakra.DomainModel
{

    public class Office365RetentionPolicy : DomainModelBase<Office365RetentionPolicy, EntityModel.Office365RetentionPolicy>
    {
        public long EndPointSystemId { get; set; }
        public string Name { get; set; }
        public string PolicyId { get; set; }
        public string DistinguishedName { get; set; }
        public bool? IsDefault { get; set; }
        public string Identity { get; set; }
        public DateTime? WhenCreated { get; set; }
        public bool? IsValid { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }

        public static Office365RetentionPolicy GetRetentionPolicyByPolicyId(string policyId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.Office365RetentionPolicy>(dataAccessTransaction)
                      .Get(t => t.PolicyId == policyId).FirstOrDefault());
            }
        }
        public static List<Office365RetentionPolicy> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.Office365RetentionPolicy>(dataAccessTransaction)
                      .Get(t => t.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static void SaveRetentionPolicy(IList<Office365RetentionPolicy> list, long endpointId, string currentUser)
        {
            try
            {
                var allexistingRetentionPolcs = Office365RetentionPolicy.GetAllByEndpointSystemId(endpointId);
                Office365RetentionPolicy existingRetnPlcy = null;
                foreach (Office365RetentionPolicy pol in list)
                {
                    existingRetnPlcy = allexistingRetentionPolcs.Where(rp => rp.PolicyId == pol.PolicyId).FirstOrDefault();
                    if (existingRetnPlcy == null)
                    {
                        pol.CreatedBy = currentUser;
                        pol.CreatedDate = DateTime.Now;
                        pol.Save(currentUser);
                        allexistingRetentionPolcs.Add(pol);
                    }
                    else
                    {
                        pol.Id = existingRetnPlcy.Id;
                        pol.CreatedDate = existingRetnPlcy.CreatedDate;
                        pol.CreatedBy = existingRetnPlcy.CreatedBy;
                        pol.LastModifiedBy = currentUser;
                        pol.LastModifiedDate = DateTime.Now;
                        pol.Save(currentUser);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Exception Occured at Office 365 SaveRetentionPolicy", ex);
                throw;
            }
        }

    }
}
