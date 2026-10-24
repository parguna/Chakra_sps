using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPRoleInfo")]
    public class SAPRoleInfoController : BaseController<DomainModel.SAPRoleInfo, EntityModel.SAPRoleInfo, Dto.SAPRoleInfo>
    {
        [HttpGet]
        [Route("GetByRoleCode")]
        public SAPRoleInfo GetByRoleCode(string roleCode, long endPointSystemId)
        {
            return DtoMap(DomainModel.SAPRoleInfo.GetByRoleCode(roleCode, endPointSystemId));
        }

        [HttpGet]
        [Route("GetSAPRoles")]
        public IList<SAPRoleInfo> GetSAPRoles(long endpointId)
        {
            return DtoMapList(DomainModel.SAPRoleInfo.GetSAPRoles(endpointId));
        }

        [HttpGet]
        [Route("GetSAPRoleBySearchString")]
        public Dictionary<long, string> GetSAPRoleBySearchString(long endpointId, string searchString)
        {
            return DomainModel.SAPRoleInfo.GetSAPRoleBySearchString(endpointId, searchString);
        }


        [HttpGet]
        [Route("GetBusinessRoles")]
        public Dictionary<long, string> GetBusinessRoles(long endpointId, int _applicationType, string module = null, string subModule = null)
        {
            return DomainModel.SAPRoleInfo.GetBusinessRoles(endpointId, (ApplicationType)_applicationType,module,subModule);
        }



        [HttpGet]
        [Route("GetAuthorizationDataForAttestationReviewGrid")]
        public List<Dto.Presentation.AttestationReviewPageSAP_Level1GridData> GetAuthorizationDataForAttestationReviewGrid(string RoleCode, int EndpointSystemId)
        {
            var res = DomainModel.SAPRoleInfo.GetSapRolesToAuthorizationData(RoleCode, EndpointSystemId);

            if (res.Count > 0)
            {

                return (from Authorization in res
                               select new Dto.Presentation.AttestationReviewPageSAP_Level1GridData
                               {
                                   sapLevel1DetailId = "Authorization;" + Authorization.AuthCode + ";" + EndpointSystemId,
                                   sapLevel1Detail = string.Format("Authorization Code: {0} Object: {1}", Authorization.AuthCode, Authorization.ObjectCode),
                                   sapLevel1DetailDesc = string.Format("Field: {0} From Value: {1}", Authorization.FieldCode, Authorization.FromValue),
                                   sapLevel1DetailType = "Auhtorization"
                               }).ToList();
            }

            return new List<Dto.Presentation.AttestationReviewPageSAP_Level1GridData>();
        }
    }
}
