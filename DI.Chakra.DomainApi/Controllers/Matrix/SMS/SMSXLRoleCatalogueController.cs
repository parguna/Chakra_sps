using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SMSXLRoleCatalogue")]
    public class SMSXLRoleCatalogueController : BaseController<DomainModel.SMSXLRoleCatalogue, EntityModel.SMSXLRoleCatalogue, Dto.SMSXLRoleCatalogue>
    {
        [HttpGet]
        [Route("GetAllSMSRoleCatalogues")]
        public List<Dto.SMSXLRoleCatalogue> GetAllSMSRoleCatalogues(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SMSXLRoleCatalogue.GetAllSMSRoleCatalogues(isValidRecords, isAllRecords));
        }
    }
}