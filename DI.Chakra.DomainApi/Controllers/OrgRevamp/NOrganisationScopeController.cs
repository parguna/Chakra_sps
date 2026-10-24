using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NOrganisationScope")]
    public class NOrganisationScopeController : BaseController<DomainModel.NOrganisationScope, EntityModel.NOrganisationScope, Dto.NOrganisationScope>
    {
        [HttpGet]
        [Route("GetAllOrganisationScope")]
        public List<Dto.NOrganisationScope> GetAllOrganisationScope()
        {
            return DtoMapList(DomainModel.NOrganisationScope.GetAllOrganisationScope());
        }

        [HttpGet]
        [Route("GetOrganisationScope")]
        public NOrganisationScope GetOrganisationScope(string Scope)
        {
            return DtoMap(DomainModel.NOrganisationScope.GetOrganisationScope(Scope));
        }
    }
}
