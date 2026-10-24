using System.Web.Http;
using DI.Chakra.Dto;
using System.Collections.Generic;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PasswordPolicy")]
    public class PasswordPolicyController : BaseController<DomainModel.PasswordPolicy, EntityModel.PasswordPolicy, PasswordPolicy>
    {
        [HttpGet]
        [Route("GetByEndpointSystemId")]
        [AllowAnonymous]
        public PasswordPolicy GetByEndpointSystemId(long endpointSystemId)
        {
            return DtoMap(DomainModel.PasswordPolicy.GetByEndpointSystemId(endpointSystemId));
        }

        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string name)
        {
            return DomainModel.PasswordPolicy.DoesExists(name);
        }

        [HttpGet]
        [Route("GetIACMLastPasswordChangeByEndpointUser")]
        [AllowAnonymous]
        public  string GetIACMLastPasswordChangeByEndpointUser(long endPointSystemId, string userName)
        {
            return DomainModel.PasswordPolicy.GetIACMLastPasswordChangeByEndpointUser(endPointSystemId, userName);
        }

        [HttpGet]
        [Route("GetPasswordPolicyIdByName")]
        public long GetPasswordPolicyIdByName(string policyName)
        {
            return DomainModel.PasswordPolicy.GetPasswordPolicyIdByName(policyName);
        }

        [HttpGet]
        [Route("ConstructGeneralUserPassword")]
        public string ConstructGeneralUserPassword()
        {
            return DomainModel.PasswordPolicy.ConstructGeneralUserPassword();
        }

        [HttpGet]
        [Route("GetByName")]
        public PasswordPolicy GetByName(string policyName)
        {
            return DtoMap(DomainModel.PasswordPolicy.GetByName(policyName));
        }

        [HttpGet]
        [Route("GetPasswordPolicies")]
        public Dictionary<long, string> GetPasswordPolicies(string displayNameSearchString)
        {
            return DomainModel.PasswordPolicy.GetPasswordPolicies(displayNameSearchString);
        }

        [HttpGet]
        [Route("GetByPolicyId")]
        [AllowAnonymous]
        public PasswordPolicy GetByPolicyId(long policyId)
        {
            return DtoMap(DomainModel.PasswordPolicy.GetById(policyId));
        }
        [HttpGet]
        [Route("DeletePolicyById")]
        public bool DeletePolicyById(long id)
        {
            try
            {
                if (!DomainModel.PasswordPolicy.IsPolicyEndpointExists(id))
                {
                    DomainModel.PasswordPolicy.Delete(id);
                    return true;
                }
                return false;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
    }
}
