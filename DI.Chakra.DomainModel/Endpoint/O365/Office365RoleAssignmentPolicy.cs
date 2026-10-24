using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using DI.Chakra.Infrastructure;
using DI.Chakra.Transaction;
using DI.Chakra.Repository;
using DI.Chakra.Logger;

namespace DI.Chakra.DomainModel
{
    public class Office365RoleAssignmentPolicy : DomainModelBase<Office365RoleAssignmentPolicy, EntityModel.Office365RoleAssignmentPolicy>
    {
        public long EndPointSystemId { get; set; }
        public string Name { get; set; }
        public string PolicyId { get; set; }
        public string DistinguishedName { get; set; }
        public bool? IsDefault { get; set; }
        public string Description { get; set; }
        public string Identity { get; set; }
        public DateTime? WhenCreated { get; set; }
        public bool? IsValid { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public static Office365RoleAssignmentPolicy GetRoleAssignementByPolicyId(string policyId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.Office365RoleAssignmentPolicy>(dataAccessTransaction)
                      .Get(t => t.PolicyId == policyId).FirstOrDefault());
            }
        }
        public static List<Office365RoleAssignmentPolicy> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.Office365RoleAssignmentPolicy>(dataAccessTransaction)
                      .Get(t => t.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static void SaveRolePolicy(IList<Office365RoleAssignmentPolicy> list, long endpointId, string currentUser)
        {
            try
            {
                var allExistingRolePolicies = Office365RoleAssignmentPolicy.GetAllByEndpointSystemId(endpointId);
                Office365RoleAssignmentPolicy existRolePolicies = null;

                foreach (Office365RoleAssignmentPolicy pol in list)
                {
                    existRolePolicies = allExistingRolePolicies.Where(ra => ra.PolicyId == pol.PolicyId).FirstOrDefault();
                    if (existRolePolicies == null)
                    {
                        
                        pol.CreatedBy = currentUser;
                        pol.CreatedDate = DateTime.Now;
                        pol.Save(currentUser);
                        allExistingRolePolicies.Add(pol);
                    }
                    else
                    {
                        pol.Id = existRolePolicies.Id;
                        pol.PolicyId = existRolePolicies.PolicyId;
                        pol.CreatedBy = existRolePolicies.CreatedBy;
                        pol.CreatedDate = existRolePolicies.CreatedDate;
                        pol.LastModifiedBy = currentUser;
                        pol.LastModifiedDate = DateTime.Now;
                        pol.Save(currentUser);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Exception Occured at Office 365 SaveRolePolicy", ex);
                throw;
            }
        }
    }
}
