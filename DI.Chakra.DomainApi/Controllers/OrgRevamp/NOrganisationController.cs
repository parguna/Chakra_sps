using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NOrganisation")]
    public class NOrganisationController : BaseController<DomainModel.NOrganisation, EntityModel.NOrganisation, Dto.NOrganisation>
    {
        [HttpGet]
        [Route("GetByOrgCode")]
        public NOrganisation GetByOrgCode(string code)
        {
            return DtoMap(DomainModel.NOrganisation.GetByOrgCode(code));
        }

        [HttpGet]
        [Route("GetById")]
        public Dto.NOrganisation GetById(long Id)
        {
            return DtoMap(DomainModel.NOrganisation.GetById(Id));
        }

        [HttpGet]
        [Route("GetNOrganisations")]
        public Dictionary<long, string> GetNOrganisations(string searchString)
        {
            return DomainModel.NOrganisation.GetOrganisations(searchString);
        }

        [HttpGet]
        [Route("GetNOrganisationByScope")]
        public Dictionary<long, string> GetNOrganisationByScope(string searchString, long scopeId)
        {
            return DomainModel.NOrganisation.GetNOrganisationByScope(searchString, scopeId);
        }

        [HttpGet]
        [Route("GetBusinessUnits")]
        public Dictionary<long, string> GetBusinessUnits(string searchString)
        {
            return DomainModel.NOrganisation.GetBusinessUnits(searchString);
        }

        [HttpGet]
        [Route("GetOrganisations")]
        public   Dictionary<string, string> GetOrganisations(string searchString, string scopeID, string ParentCode)
        {
            return DomainModel.NOrganisation.GetOrganisations(searchString, scopeID, ParentCode);
        }


        [HttpGet]
        [Route("GetOrganisationsByName")]
        public Dictionary<string, string> GetOrganisationsByName(string name )
        {
            return DomainModel.NOrganisation.GetOrganisationsByName(name);
        }

        [HttpGet]
        [Route("GetDepartMents")]
        public Dictionary<long, string> GetDepartMents(string searchString)
        {
            return DomainModel.NOrganisation.GetDepartMents(searchString);
        }

        [HttpGet]
        [Route("GetCompanies")]
        public Dictionary<long, string> GetCompanies(string searchString)
        {
            return DomainModel.NOrganisation.GetCompanies(searchString);
        }
        [HttpGet]
        [Route("GetDivisionsByDeptID")]
        public Dictionary<long, string> GetDivisionsByDeptID(string searchString, string departmentId)
        {
            return DomainModel.NOrganisation.GetDivisionsByDeptID(searchString, departmentId);
        }
        [HttpGet]
        [Route("GetDisticntSectionFromUsers")]
        public Dictionary<string, string> GetDisticntSectionFromUsers(string searchString)
        {
            return DomainModel.NOrganisation.GetDisticntSectionFromUsers(searchString);
        }
        [HttpGet]
        [Route("GetCostCenterCodeAndTextByCompanyID")]
        public Dictionary<string, string> GetCostCenterCodeAndTextByCompanyID(string searchString, string companyId)
        {
            return DomainModel.NOrganisation.GetCostCenterCodeAndTextByCompanyID(searchString, companyId);
        }
        [HttpGet]
        [Route("GetInternalCompanies")]
        public Dictionary<long, string> GetInternalCompanies(string searchString)
        {
            return DomainModel.NOrganisation.GetInternalCompanies(searchString);
        }

        [HttpGet]
        [Route("GetExternalCompanies")]
        public Dictionary<long, string> GetExternalCompanies(string searchString)
        {
            return DomainModel.NOrganisation.GetExternalCompanies(searchString);
        }


        [HttpGet]
        [Route("GetLocations")]
        public Dictionary<long, string> GetLocations(string searchString)
        {
            return DomainModel.NOrganisation.GetLocations(searchString);
        }

        [HttpGet]
        [Route("GetCostCenters")]
        public Dictionary<long, string> GetCostCenters(string searchString)
        {
            return DomainModel.NOrganisation.GetCostCenters(searchString);
        }

        [HttpGet]
        [Route("GetOrganisationScopeUsers")]
        public Dictionary<long, string> GetOrganisationScopeUsers(string searchString, string OrganisationScopeId)
        {
            return DomainModel.NOrganisation.GetOrganisationScopeUsers(searchString, OrganisationScopeId);
        }


        [HttpGet]
        [Route("GetByParentOrgId")]
        public List<NOrganisation> GetByParentOrgId(long? parentOrgId)
        {
            return DtoMapList(DomainModel.NOrganisation.GetByParentOrgId(parentOrgId));
        }


        [HttpGet]
        [Route("GetCompaniesByFilter")]
        public List<NOrganisation> GetCompaniesByFilter(DateTime? FromDate, DateTime? ToDate, bool? Internal)
        {
            return DtoMapList(DomainModel.NOrganisation.GetCompaniesByFilter(FromDate, ToDate, Internal));
        }

        [HttpGet]
        [Route("GetOrgCompanies")]
        public List<NOrganisation> GetOrgCompanies()
        {
            return DtoMapList(DomainModel.NOrganisation.GetOrgCompanies());
        }

        [HttpGet]
        [Route("GetByScopeId")]
        public List<NOrganisation> GetByScopeId(long scopeId)
        {
            return DtoMapList(DomainModel.NOrganisation.GetByScopeId(scopeId));
        }

        [HttpGet]
        [Route("GetParentOrgs")]
        public List<NOrganisation> GetParentOrgs()
        {
            return DtoMapList(DomainModel.NOrganisation.GetParentOrgs());
        }

        [HttpGet]
        [Route("GetTopmostByHeadId")]
        public long GetTopmostByHeadId(long userId)
        {
            return DomainModel.NOrganisation.GetTopmostByHeadId(userId);
        }

        [HttpGet]
        [Route("isExists")]
        public bool isExists(string companyCode,long scopeId)
        {
            return DomainModel.NOrganisation.isExists(companyCode, scopeId);
        }

        [HttpGet]
        [Route("GetNOrganisationByScopeId")]
        public NOrganisation GetNOrganisationByScopeId(string Code, long scopeId)
        {
            return DtoMap(DomainModel.NOrganisation.GetNOrganisationByScopeId(Code, scopeId));
        }

        [HttpGet]
        [Route("GetNOrganisationIdByScopeId")]
        public NOrganisation GetNOrganisationIdByScopeId(long Id, long scopeId)
        {
            return DtoMap(DomainModel.NOrganisation.GetNOrganisationIdByScopeId(Id, scopeId));
        }
    }
}
