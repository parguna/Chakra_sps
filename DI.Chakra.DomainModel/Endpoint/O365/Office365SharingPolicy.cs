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

    public class Office365SharingPolicy : DomainModelBase<Office365SharingPolicy, DI.Chakra.EntityModel.Office365SharingPolicy>
    {
        public long EndPointSystemId { get; set; }
        public string Name { get; set; }
        public string PolicyId { get; set; }
        public string DistinguishedName { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsEnabled { get; set; }
        public string Identity { get; set; }
        public DateTime? WhenCreated { get; set; }
        public bool? IsValid { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }

        public static Office365SharingPolicy GetSharingPolicyByPolicyId(string policyId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.Office365SharingPolicy>(dataAccessTransaction)
                      .Get(t => t.PolicyId == policyId).FirstOrDefault());
            }
        }
        public static List<Office365SharingPolicy> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.Office365SharingPolicy>(dataAccessTransaction)
                      .Get(t => t.EndPointSystemId == endpointSystemId).ToList());
            }
        }
        public static void SaveSharingPolicy(IList<Office365SharingPolicy> list, long endpointId, string currentUser)
        {
            try
            {
                var allExistingSharingPolcies = Office365SharingPolicy.GetAllByEndpointSystemId(endpointId);
                Office365SharingPolicy existinSharPolc = null;
                foreach (Office365SharingPolicy pol in list)
                {

                    existinSharPolc = allExistingSharingPolcies.Where(r => r.PolicyId == pol.PolicyId).FirstOrDefault();
                    if (existinSharPolc == null)
                    {
                        pol.Save(currentUser);
                        allExistingSharingPolcies.Add(pol);
                    }
                    else
                    {
                        pol.Id = existinSharPolc.Id;
                        pol.CreatedBy = existinSharPolc.CreatedBy;
                        pol.CreatedDate = existinSharPolc.CreatedDate;
                        pol.LastModifiedBy = currentUser;
                        pol.LastModifiedDate = DateTime.Now;
                        pol.Save(currentUser);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Exception Occured at Office 365 Saving Sharing Policy", ex);
                throw;
            }
        }
    }
}
