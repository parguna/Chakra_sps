using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NOVASoDExceptionsMatrix")]
    public class NOVASoDExceptionsMatrixController: BaseController<DomainModel.NOVASoDExceptionsMatrix, EntityModel.NOVASoDExceptionsMatrix, Dto.NOVASoDExceptionsMatrix>
    {
        [HttpGet]
        [Route("GetNOVASoDExceptionsMatrices")]
        public List<Dto.NOVASoDExceptionsMatrix> GetNOVASoDExceptionsMatrices(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.NOVASoDExceptionsMatrix.GetNOVASoDExceptionsMatrices(isValidRecords, isAllRecords));
        }
        [HttpGet]
        [Route("GetSODMatricesBySectionCode")]
        public List<Dto.NOVASoDExceptionsMatrix> GetSODMatricesBySectionCode(long endpointId, long DIUserId,string sapSection)
        {
            return DtoMapList(DomainModel.NOVASoDExceptionsMatrix.GetSODMatricesBySectionCode(endpointId,DIUserId,sapSection));
        }
    }
}