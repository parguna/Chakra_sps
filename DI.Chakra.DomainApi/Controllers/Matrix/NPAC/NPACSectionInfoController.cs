using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NPACSectionInfo")]
    public class NPACSectionInfoController : BaseController<DomainModel.NPACSectionInfo, EntityModel.NPACSectionInfo, Dto.NPACSectionInfo>
    {
        [HttpGet]
        [Route("GetAllNPACSections")]
        public List<Dto.NPACSectionInfo> GetAllNPACSections(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.NPACSectionInfo.GetAllNPACSections(isValidRecords, isAllRecords));
        }

        [HttpGet]
        [Route("GetNPACSectionByName")]
        public Dto.NPACSectionInfo GetNPACSectionByName(string sectionName)
        {
            return DtoMap(DomainModel.NPACSectionInfo.GetNPACSectionByName(sectionName));
        }
        [HttpGet]
        [Route("GetNPACSectionByCode")]
        public Dto.NPACSectionInfo GetNPACSectionByCode(string sectionCode)
        {
            return DtoMap(DomainModel.NPACSectionInfo.GetNPACSectionByCode(sectionCode));
        }
    }
}