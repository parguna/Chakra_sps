using DI.Chakra.DomainApi.Controllers;
using System.Web.Http;

namespace DI.Chakra.DomainApi
{
    [RoutePrefix("Office365MassLicense")]
    public class Office365MassLicenseController : BaseController<DomainModel.Office365MassLicense, EntityModel.Office365MassLicense, Dto.Office365MassLicense>
    {

    }
}