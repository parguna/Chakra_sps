using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class PasswordPolicyRepository : RepositoryBase<PasswordPolicy>
    {
        public PasswordPolicyRepository(DataAccessTransaction transaction) : base(transaction) { }

        public PasswordPolicy GetByEndpointSystemTypeId(long endpointSystemTypeId)
        {
            return (from policy in CurrentContext.PasswordPolicy
                    join policyEp in CurrentContext.PasswordPolicyEndpoint
                                on policy.Id equals policyEp.PolicyId
                    join endPoint in CurrentContext.EndPoint
                                on policyEp.EndpointSystemId equals endPoint.Id
                    where endPoint.Id == endpointSystemTypeId
                    select policy).FirstOrDefault();
        }

        public PasswordPolicy GetByEndpointSystemId(long endpointSystemId)
        {
            return (from policy in CurrentContext.PasswordPolicy
                    join policyEp in CurrentContext.PasswordPolicyEndpoint
                            on policy.Id equals policyEp.PolicyId
                    join endPoint in CurrentContext.EndPoint
                            on policyEp.EndpointSystemId equals endPoint.Id
                    where endPoint.Id == endpointSystemId
                    select policy).FirstOrDefault();
        }

        public PasswordPolicy GetByEndpointSystemTypeId(long endpointSystemTypeId, long endpointSystemId)
        {
            return (from policy in CurrentContext.PasswordPolicy
                    join policyEp in CurrentContext.PasswordPolicyEndpoint
                                on policy.Id equals policyEp.PolicyId
                    join endPoint in CurrentContext.EndPoint
                                on policyEp.EndpointSystemId equals endPoint.Id
                    where endPoint.EndpointSystemTypeId == endpointSystemTypeId && endPoint.Id == endpointSystemId
                    select policy).FirstOrDefault();
        }

        public Dictionary<long, string> GetPasswordPolicies(string displayNameSearchString)
        {
            return (from PP in CurrentContext.PasswordPolicy
                    where PP.Name.Contains(displayNameSearchString)
                    select new { PP.Name, PP.Id })
                    .Distinct().ToDictionary(c => c.Id, c => c.Name);
        }
        public bool IsPolicyEndpointExists(long policyId)
        {
            return (from policy in CurrentContext.PasswordPolicy
                    join policyEp in CurrentContext.PasswordPolicyEndpoint
                                on policy.Id equals policyEp.PolicyId
                    join endPoint in CurrentContext.EndPoint
                                on policyEp.EndpointSystemId equals endPoint.Id
                    where policy.Id == policyId
                    select policy).Any();
        }
    }
}
