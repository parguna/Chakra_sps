using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPHANARoleInfo")]
    public class SAPHANARoleInfoController : BaseController<DomainModel.SAPHANARoleInfo, EntityModel.SAPHANARoleInfo, Dto.SAPHANARoleInfo>
    {
        [HttpGet]
        [Route("GetByRoleCode")]
        public SAPHANARoleInfo GetByRoleCode(string roleCode, long endPointSystemId)
        {
            return DtoMap(DomainModel.SAPHANARoleInfo.GetByRoleCode(roleCode, endPointSystemId));
        }

        [HttpGet]
        [Route("GetSAPRoles")]
        public IList<SAPHANARoleInfo> GetSAPRoles(long endpointId)
        {
            return DtoMapList(DomainModel.SAPHANARoleInfo.GetSAPRoles(endpointId));
        }

        [HttpGet]
        [Route("GetSAPRoleBySearchString")]
        public Dictionary<long, string> GetSAPRoleBySearchString(long endpointId, string searchString)
        {
            return DomainModel.SAPHANARoleInfo.GetSAPRoleBySearchString(endpointId, searchString);
        }


        [HttpGet]
        [Route("GetBusinessRoles")]
        public Dictionary<long, string> GetBusinessRoles(long endpointId, long endpointUserId, string module = null, string subModule = null)
        {
            return DomainModel.SAPHANARoleInfo.GetBusinessRoles(endpointId, endpointUserId,module,subModule);
        }

        [HttpGet]
        [Route("GetAuthorizationDataForAttestationReviewGrid")]
        public List<Dto.Presentation.AttestationReviewPageSAP_Level1GridData> GetAuthorizationDataForAttestationReviewGrid(string RoleCode, int EndpointSystemId)
        {
            var res = DomainModel.SAPHANARoleInfo.GetSapRolesToAuthorizationData(RoleCode, EndpointSystemId);

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
