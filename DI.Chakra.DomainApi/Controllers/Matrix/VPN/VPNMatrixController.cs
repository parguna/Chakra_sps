using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("VPNMatrix")]
    public class VPNMatrixController : BaseController<DomainModel.VPNMatrix, EntityModel.VPNMatrix, Dto.VPNMatrix>
    {
        [HttpGet]
        [Route("GetApplicationProjectMatrix")]
        public Dictionary<long, string> GetVPNMatrix(long endPointUserId, long endpointId, bool isFullTimeEmployee, string searchString,AdHocActionType adHocActionType,VPNType vpnType)
        {
            return DomainModel.VPNApplicationEnvironment.GetActiveVPNMatrix(endPointUserId, endpointId,isFullTimeEmployee, searchString, adHocActionType, vpnType);
        }
        [HttpGet]
        [Route("GetApplicationEnviornment")]
        public Dictionary<VPNMatrixEnviornmentType, bool> GetApplicationEnviornmentType(long endPointUserId, bool excludeApplication,long matrixId)
        {
            return DomainModel.VPNApplicationEnvironment.GetApplicationEnviornment(endPointUserId, excludeApplication, matrixId);
        }
        [HttpGet]
        [Route("GetAllVPNMatrix")]
        public List<Dto.VPNMatrix> GetAllVPNMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.VPNMatrix.GetAllVPNMatrix(isValidRecords, isAllRecords));
        }
        [HttpGet]
        [Route("GetApplicationForBulkRequest")]
        public Dictionary<long, string> GetApplicationForBulkRequest(string searchString, VPNType vpnType)
        {
            return DomainModel.VPNApplicationEnvironment.GetApplicationForBulkRequest( searchString, vpnType);
        }
        [HttpGet]
        [Route("DeleteVPNMatrixById")]
        public string DeleteVPNMatrixById(long id)
        {
            return DomainModel.VPNMatrix.DeleteByMatrixId(id);
        }
        
    }
}