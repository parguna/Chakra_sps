using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationEndpointProvisioningField")] 
    public class PresentationEndpointProvisioningFieldController : ApiController
    {
        [HttpGet]
        [Route("GetEndpointProvisiongField")]
        public List<DI.Chakra.Dto.Presentation.PresentationEndpointProvisioningField> GetEndpointProvisiongField()
        {
            return DtoMapList(DomainModel.Presentation.PresentationEndpointProvisioningField.GetEndpointProvisiongField());
        }

        public DI.Chakra.Dto.Presentation.PresentationEndpointProvisioningField DtoMap(DomainModel.Presentation.PresentationEndpointProvisioningField tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationEndpointProvisioningField, DI.Chakra.Dto.Presentation.PresentationEndpointProvisioningField>(tModel);
        }

        public List<DI.Chakra.Dto.Presentation.PresentationEndpointProvisioningField> DtoMapList(IList<DomainModel.Presentation.PresentationEndpointProvisioningField> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}