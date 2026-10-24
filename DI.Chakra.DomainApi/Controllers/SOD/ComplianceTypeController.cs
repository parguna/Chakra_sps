using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ComplianceType")]
    public class ComplianceTypeController : BaseController<DomainModel.ComplianceType, EntityModel.ComplianceType, Dto.ComplianceType>
    {
    }
}
