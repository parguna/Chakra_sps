using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("EndPoint")]
    public class EndPointController : BaseController<DomainModel.EndPoint, EntityModel.EndPoint, Dto.EndPoint>
    {

        [HttpGet]
        [Route("GetEndPointByApplicationType")]
        public EndPoint GetEndPointByApplicationType(ApplicationType applicationType)
        {
            return DtoMap(DomainModel.EndPoint.GetEndPointByApplicationType(applicationType)); 
        }

        [HttpGet]
        [Route("GetByEndpointTypeAndTrustedSource")]
        [AllowAnonymous]
        public EndPoint GetByEndpointTypeAndTrustedSource(long endpointTypeID, bool isTrustedSource)
        {
            return DtoMap(DomainModel.EndPoint.GetByEndpointType(endpointTypeID, isTrustedSource));
        }


        [HttpGet]
        [Route("GetEndPointByApplicationTypeNonTrusted")]
        public EndPoint GetEndPointByApplicationTypeNonTrusted(ApplicationType applicationType)
        {
            return DtoMap(DomainModel.EndPoint.GetEndPointByApplicationTypeNonTrusted(applicationType));
        }

        [HttpGet]
        [Route("GetByEndpointType")]
        public List<EndPoint> GetByEndpointType(EndpointType endpointType, string hostName)
        {
            return DtoMapList(DomainModel.EndPoint.GetByEndpointType(endpointType,hostName));
        }

        [HttpGet]
        [Route("GetAccessRequestConfigurationByEndPointSystemId")]
        public List<AccessRequestConfiguration> GetAccessRequestConfigurationByEndPointSystemId(long EndPointSystemId, long AccoutTypeId)
        {
           var configList = DomainModel.AccessRequestConfiguration.GetAccessRequestConfigurationByEndPointSystemId(EndPointSystemId, AccoutTypeId) ;
            if (configList == null) return new List<AccessRequestConfiguration>();
            
            return configList.Select(config=>  Mapper.Map<DomainModel.AccessRequestConfiguration, Dto.AccessRequestConfiguration>(config)).ToList();
        }

        [HttpGet]
        [Route("GetByIdIncludingEndPointSystemType")]
        public EndPoint GetByIdIncludingEndPointSystemType(long id)
        {
            return DtoMap(DomainModel.EndPoint.GetByIdIncludingEndPointSystemType(id));
        }

        [HttpGet]
        [Route("GetPasswordPolicyApplications")]
        public Dictionary<long, string> GetPasswordPolicyApplications(string displayNameSearchString)
        {
            return DomainModel.EndPoint.GetPasswordPolicyApplications(displayNameSearchString);
        }

        [HttpGet]
        [Route("GetByEndpointSystemTypeId")]
        public List<EndPoint> GetByEndpointSystemTypeId(long id)
        {
            return DtoMapList(DomainModel.EndPoint.GetByEndpointSystemTypeId(id));
        }

        [HttpGet]
        [Route("GetOrgEndpointByType")]
        public List<EndPoint> GetOrgEndpointByType(long id)
        {
            return DtoMapList(DomainModel.EndPoint.GetOrgEndpointByType(id));
        }


        [HttpGet]
        [Route("GetActiveNonTrustedSrcNonInhouseEndpoints")]
        public List<EndPoint> GetActiveNonTrustedSrcNonInhouseEndpoints(bool includeInhouse)
        {
            return DtoMapList(DomainModel.EndPoint.GetActiveNonTrustedSrcNonInhouseEndpoints(includeInhouse));
        }

        [HttpGet]
        [Route("GetActiveEndpoints")]
        public List<EndPoint> GetActiveEndpoints()
        {
            return DtoMapList(DomainModel.EndPoint.GetActiveEndpoints());
        }

        [HttpGet]
        [Route("GetByEndpointSystemTypeIdIncludingEndPointSystemType")]
        public List<EndPoint> GetByEndpointSystemTypeIdIncludingEndPointSystemType(long endpointSystemTypeId)
        {
            return DtoMapList(DomainModel.EndPoint.GetByEndpointSystemTypeIdIncludingEndPointSystemType(endpointSystemTypeId));
        }

        [HttpGet]
        [Route("GetByIdIncludingOwner")]
        [AllowAnonymous]
        public EndPoint GetByIdIncludingOwner(long id)
        {
            return DtoMap(DomainModel.EndPoint.GetByIdIncludingOwner(id));
        }

        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(long id, string name, long endpointSystemTypeId)
        {
            return DomainModel.EndPoint.DoesExists(id, name, endpointSystemTypeId);
        }
        [HttpGet]
        [Route("IsActiveEndpointsystem")]
        public bool IsActiveEndpointsystem(long endpointSystemId)
        {
            return DomainModel.EndPoint.IsActiveEndpointsystem(endpointSystemId);
        }
        [HttpGet]
        [Route("DoesExistsbyApplicationCode")]
        public int DoesExistsbyApplicationCode(string applicationCode, long id)
        {
            return DomainModel.EndPoint.DoesExistsbyApplicationCode(applicationCode, id);
        }

        [HttpGet]
        [Route("GetNonTrustedSource")]
        public List<EndPoint> GetNonTrustedSource(long id)
        {
            return DtoMapList(DomainModel.EndPoint.GetNonTrustedSource(id));
        }

        [HttpPost]
        [Route("SaveConfiguration")]
        public void SaveConfiguration([FromBody]EndpointConfiguration endPointConfig)
        {
            DomainModel.EndPoint.SaveConfiguration(AutoMapper.Mapper.Map<Dto.EndpointConfiguration, DomainModel.EndpointConfiguration>(endPointConfig), User.Identity.Name);
        }

        [HttpGet]
        [Route("GetByEndpointSystemTypeIdTrustedSource")]
        public Dictionary<long, string> GetByEndpointSystemTypeIdTrustedSource(long endpointSystemTypeId, bool ignoreTrustedSource = true)
        {
            return DomainModel.EndPoint.GetByEndpointSystemTypeIdTrustedSource(endpointSystemTypeId, ignoreTrustedSource);
        }

        [HttpGet]
        [Route("GetByTrustedSource")]
        public Dictionary<long, string> GetByTrustedSource(bool ignoreTrustedSource)
        {
            return DomainModel.EndPoint.GetByTrustedSource(ignoreTrustedSource);
        }

        [HttpGet]
        [Route("GetInhouseEndpoints")]
        public Dictionary<long, string> GetInhouseEndpoints(bool ignoreTrustedSource)
        {
            return DomainModel.EndPoint.GetInhouseEndpoints();
        }
        [HttpGet]
        [Route("GetInhouseEndPointsWithCode")]
        public Dictionary<string, string> GetInhouseEndPointsWithCode()
        {
            return DomainModel.EndPoint.GetInhouseEndPointsWithCode();
        }

        [HttpGet]
        [Route("GetByEndpointConfiguration")]
        public List<EndPoint> GetByEndpointConfiguration(string key, string value)
        {
            return DtoMapList(DomainModel.EndPoint.GetByEndpointConfiguration(key,value));
        }

        [HttpGet]
        [Route("GetAllIncludingEndPointSystemType")]
        public List<EndPoint> GetAllIncludingEndPointSystemType()
        {
            return DtoMapList(DomainModel.EndPoint.GetAllIncludingEndPointSystemType());
        }

        [HttpGet]
        [Route("GetByEndpointTypeIncludingOwner")]
        public List<EndPoint> GetByEndpointTypeIncludingOwner(long endPointSystemTypeId, string hostName)
        {
            return DtoMapList(DomainModel.EndPoint.GetByEndpointTypeIncludingOwner(endPointSystemTypeId, hostName));
        }

        [HttpGet]
        [Route("GetByDisplayNameIncludingEndPointSystemType")]
        public EndPoint GetByDisplayNameIncludingEndPointSystemType(string displayName)
        {
            return DtoMap(DomainModel.EndPoint.GetByDisplayNameIncludingEndPointSystemType(displayName));
        }

        [HttpGet]
        [Route("GetByApplicationCode")]
        public EndPoint GetByApplicationCode(string applicationCode)
        {
            return DtoMap(DomainModel.EndPoint.GetByApplicationCode(applicationCode));
        }

        [HttpGet]
        [Route("GetByDescriptionSearchString")]
        public Dictionary<long, string> GetByDescriptionSearchString(string descriptionSearchString)
        {
            return DomainModel.EndPoint.GetByDescriptionSearchString(descriptionSearchString);
        }

        [HttpGet]
        [Route("GetByEndpointDescSearchString")]
        public Dictionary<long, string> GetByEndpointDescSearchString(string descriptionSearchString)
        {
            return DomainModel.EndPoint.GetByEndpointDescSearchString(descriptionSearchString);
        }

        [HttpGet]
        [Route("GetApplicationCodeById")] 
        public Dictionary<long, string> GetApplicationCodeById(long id, string applicationCode)
        {
            return DomainModel.EndPoint.GetApplicationCodeById(id,applicationCode);
        }

        [HttpGet]
        [Route("GetApplicableApplications")]
        public Dictionary<long, string> GetApplicableApplications(string displayNameSearchString)
        {
            return DomainModel.EndPoint.GetApplicableApplications(displayNameSearchString);
        }

        [HttpGet]
        [Route("Delete")]
        public new bool Delete(long id)
        {
            return DomainModel.EndPoint.Delete(id);
        }

        [HttpPost]
        [Route("GetAttestationByEndpointSystemTypeIds")]
        public Dictionary<long, string> GetAttestationByEndpointSystemTypeIds(string displayNameSearchString, [FromBody] List<long> endpointSystemTypeIdList)
        {
            displayNameSearchString = string.IsNullOrEmpty(displayNameSearchString) ? "" : displayNameSearchString;
            return DomainModel.EndPoint.GetAttestationByEndpointSystemTypeIds(endpointSystemTypeIdList, displayNameSearchString);
        }

        [HttpPost]
        [Route("GetEndpointSystemByIds")]
        public List<EndPoint> GetEndpointSystemByIds([FromBody] List<long> endpointSystemIdList)
        {
            return DtoMapList(DomainModel.EndPoint.GetByEndpointSystemByIds(endpointSystemIdList));
        }

        [HttpGet]
        [Route("GetByEndpointSystemTypeIds")]
        public Dictionary<long, string> GetByEndpointSystemTypeIds(List<long> endpointSystemTypeIdList, string displayNameSearchString)
        {
            return DomainModel.EndPoint.GetByEndpointSystemTypeIds(endpointSystemTypeIdList, displayNameSearchString);
        }

        [HttpGet]
        [Route("GetSingleEntitlements")]
        public Dictionary<long, string> GetSingleEntitlements(string searchString, long EndpointSystemId, long? EndpointUserId, bool existingEntitlements)
        {
            return DomainModel.EndPoint.GetSingleEntitlements(searchString, EndpointSystemId, EndpointUserId, existingEntitlements);
        }

        [HttpGet]
        [Route("GetByEndpointSystemTypeIdIsActive")]
        [AllowAnonymous]
        public EndPoint GetByEndpointSystemTypeIdIsActive(long endpointSystemTypeId, bool isActive)
        {
            return DtoMap(DomainModel.EndPoint.GetByEndpointSystemTypeIdIsActive(endpointSystemTypeId, isActive));
        }
        [HttpGet]
        [Route("GetInhouseEndPointsAutomateSystem")]
        public Dictionary<long, string> GetInhouseEndPointsAutomateSystem()
        {
            return DomainModel.EndPoint.GetInhouseEndPointsAutomateSystem();
        }
    }
}
