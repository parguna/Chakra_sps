using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ComplianceReportModules")]
    public class ComplianceReportModulesController : BaseController<DomainModel.ComplianceReportModules, EntityModel.ComplianceReportModules, Dto.ComplianceReportModules>
    {
    }
}
