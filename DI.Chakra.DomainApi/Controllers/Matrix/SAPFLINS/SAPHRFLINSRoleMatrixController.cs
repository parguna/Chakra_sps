using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPHRFLINSRoleMatrix")]
    public class SAPHRFLINSRoleMatrixController : BaseController<DomainModel.SAPHRFLINSRoleMatrix, EntityModel.SAPHRFLINSRoleMatrix, Dto.SAPHRFLINSRoleMatrix>
    {
        [HttpGet]
        [Route("GetUserType")]
        public string GetUserType(string businessRole)
        {
            return DomainModel.SAPHRFLINSRoleMatrix.GetUserType(businessRole);
        }

        [HttpGet]
        [Route("GetCompany")]
        public Dictionary<long, string> GetCompany(string searchString, string businessRoleID, int EndpointUserId)
        {
            return DomainModel.SAPHRFLINSRoleMatrix.GetCompany(searchString, businessRoleID, EndpointUserId);
        }

        [HttpGet]
        [Route("GetMandatory2DataSource")]
        public Dictionary<long, string> GetMandatory2DataSource(string mandatory1string, string mandatory2string, string CostCenter, string businessRoleID, string Company, string PurchaseOrg, string PurchaseGroup, string Planet, string StorageLocation, string MRPControl, long EndpointUserId)
        {
            return DomainModel.SAPHRFLINSRoleMatrix.GetMandatory2DataSource(mandatory1string, mandatory2string, CostCenter, businessRoleID, Company, PurchaseOrg, PurchaseGroup, Planet, StorageLocation, MRPControl, EndpointUserId);
        }

        [HttpGet]
        [Route("BindModule")]
        public IList<Dto.SAPHRFLINSRoleMatrix> BindModule(long userID)
        {
            return DtoMapList(DomainModel.SAPHRFLINSRoleMatrix.BindModule(userID));
        }

        [HttpGet]
        [Route("BindSubModule")]
        public IList<Dto.SAPHRFLINSRoleMatrix> BindSubModule(string moduleName, long UserID)
        {
            return DtoMapList(DomainModel.SAPHRFLINSRoleMatrix.BindSubModule(moduleName, UserID));
        }

        [HttpPost]
        [Route("GetHrFlinsTechnicalRolesByAdhocRequestObject")]
        public IList<string> GetHrFlinsTechnicalRolesByAdhocRequestObject([FromBody] Dto.AdHocSAPHRFLINSRequestDetail detail)
        {
            return DomainModel.SAPHRFLINSRoleMatrix.GetHrFlinsTechnicalRolesByAdhocRequestObject(detail.CostCenter, detail.Company, detail.Entitlement, detail.PurchaseGroup, detail.Plant, detail.MRPController, detail.StorageLocation, detail.RetainCompanyRoles);
        }

       
        [HttpGet]
        [Route("GetBusinessRoles")]
        public Dictionary<long, string> GetBusinessRoles(string searchString, string module, string subModuleID, long UserID)
        {
            return DomainModel.SAPHRFLINSRoleMatrix.GetBusinessRoles(searchString, module, subModuleID, UserID);
        }
        [HttpGet]
        [Route("GetEndpointUserBusinessRoles")]
        public List<Dto.SAPHRFLINSRoleMatrix> GetEndpointUserBusinessRoles(long EndpointUserId, string CostCenterPurchaseGroup)
        {
            return DtoMapList(DomainModel.SAPHRFLINSRoleMatrix.GetAllExistingBusinessRoles(EndpointUserId, CostCenterPurchaseGroup));
        }

        [HttpGet]
        [Route("GetEndpointUserBusinessRolesIncudeSupplementary")]
        public List<Dto.SAPHRFLINSRoleMatrix> GetEndpointUserBusinessRolesIncudeSupplementary(long EndpointUserId,  string CostCenterPurchaseGroup)
        {
            return DtoMapList(DomainModel.SAPHRFLINSRoleMatrix.GetEndpointUserBusinessRolesIncudeSupplementary(EndpointUserId, CostCenterPurchaseGroup));
        }

        [HttpGet]
        [Route("GetAllSAPHRFLINSRoleMatrices")]
        public List<Dto.SAPHRFLINSRoleMatrix> GetAllSAPHRFLINSRoleMatrices(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPHRFLINSRoleMatrix.GetAllSAPHRFLINSRoleMatrices(isValidRecords, isAllRecords));
        }
    }
}
