using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NProvisioningField")]
    public class NProvisioningFieldController : BaseController<DomainModel.NProvisioningField, EntityModel.NProvisioningField, Dto.NProvisioningField>
    {
        [HttpGet]
        [Route("GetByEndpointSystemId")]
        public List<NProvisioningField> GetByEndpointSystemId(long endpointSystemId)
        {
            return DtoMapList(DomainModel.NProvisioningField.GetByEndpointSystemId(endpointSystemId));
        }

        [HttpGet]
        [Route("GetByEndPointSystemIncludingValueDisplayName")]
        public List<NProvisioningField> GetByEndPointSystemIncludingValueDisplayName(long endPointSystemTypeId, long? endPointSystemId)
        {
            return DtoMapList(DomainModel.NProvisioningField.GetByEndPointSystemIncludingValueDisplayName(endPointSystemTypeId, endPointSystemId));
        }

        
        [HttpGet]
        [Route("GetByEndpointSystemIdAndName")]
        public NProvisioningField GetByEndpointSystemIdAndName(long endpointSystemId,string fieldName)
        {
            return DtoMap(DomainModel.NProvisioningField.GetByEndpointSystemIdAndName(endpointSystemId, fieldName));
        }
        [HttpGet]
        [Route("HasAnyByEndPointsystemId")]  
        public bool HasAnyByEndpointsystemId(long endPointSystemId)  
        {
            return DomainModel.NProvisioningField.HasAnyByEndPointsystemId(endPointSystemId);
        }

        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string fieldName, Nullable<AccountType> AccountType, long endPointSystemId) 
        {
            return DomainModel.NProvisioningField.DoesExists(fieldName,AccountType,  endPointSystemId);
        }

        [HttpGet]
        [Route("DeleteByEndPointSystemId")] 
        public void DeleteByEndPointSystemId(long endPointSystemId)  
        {
            DomainModel.NProvisioningField.DeleteByEndPointSystemId(endPointSystemId);
        }
    }
}
