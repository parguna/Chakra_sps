using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NPACDesignationInfo")]
    public class NPACDesignationInfoController : BaseController<DomainModel.NPACDesignationInfo, EntityModel.NPACDesignationInfo, Dto.NPACDesignationInfo>
    {
        [HttpGet]
        [Route("GetAllNPACDesignations")]
        public List<Dto.NPACDesignationInfo> GetAllNPACDesignations(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.NPACDesignationInfo.GetAllNPACDesignations(isValidRecords, isAllRecords));
        }
    }
}