using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("OfficeLicenseConfig")]
    public class OfficeLicenseConfigController: BaseController<DomainModel.Office365LicenseConfig, EntityModel.Office365LicenseConfig, Dto.Office365LicenseConfig>
    {
        [HttpGet]
        [Route("GetAllOfficeLicenseConfig")]
        public List<Dto.Office365LicenseConfig> GetAllOffice365LicenseConfig(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.Office365LicenseConfig.GetAllOffice365LicenseConfig(isValidRecords, isAllRecords));
        }
    }
}