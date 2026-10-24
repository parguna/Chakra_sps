using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("Attestation")]
    public class AttestationController : BaseController<DomainModel.Attestation, EntityModel.Attestation, Dto.Attestation>
    {
        [HttpGet]
        [Route("GetByIdIncludingAttestationProcesses")]
        public DI.Chakra.Dto.Attestation GetByIdIncludingAttestationProcesses(long id)
        {
            return DtoMap(DomainModel.Attestation.GetByIdIncludingAttestationProcesses(id));
        }

        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string attestationName, long attestationid)
        {
            return DomainModel.Attestation.DoesExists(attestationName, attestationid);
        }


        [HttpGet]
        [Route("DeleteAllNavigation")]
        public bool DeleteAllNavigation(long attesationId)
        {
            return DomainModel.Attestation.DeleteById(attesationId);
        }

        [HttpGet]
        [Route("IsLinkedWithAtestation")]
        public bool IsLinkedWithAtestation(long workflowid)
        {
            return DomainModel.Attestation.IsLinkedWithAtestation(workflowid);
        }

        [HttpGet]
        [Route("GetByAttestationName")]
        public DI.Chakra.Dto.Attestation GetByAttestationName(string attestationName)
        {
            return DtoMap(DomainModel.Attestation.GetByAttestationName(attestationName));
        }
        [HttpGet]
        [Route("GetFlinsAttestationMessage")]
        public string GetFlinsAttestationMessage(long id)
        {
            return DomainModel.Attestation.GetFlinsAttestationMessage(id);
        }
        [HttpGet]
        [Route("GetSAPApplicationTypeFromAttestation")]
        public Dictionary<int, string> GetSAPApplicationTypeFromAttestation(long id)
        {
            return DomainModel.Attestation.GetSAPApplicationTypeFromAttestation(id);
        }

        [HttpGet]
        [Route("DownloadTCodeReport")]
        public IDMSHttpResponse<byte[]> DownloadTCodeReport(ApplicationType applicationType)
        {
            var obj = DomainModel.Attestation.DownloadTCodeReport(applicationType);
            IDMSHttpResponse<byte[]> data = new IDMSHttpResponse<byte[]>();
            data.Data = obj.Data;
            data.Message = obj.Message;
            return data;
        }
        [HttpGet]
        [Route("DoesExistsOtherSystemEndpoint")]
        public bool DoesExistsOtherSystemEndpoint(long taskId)
        {
            return DomainModel.Attestation.DoesExistsOtherSystemEndpoint(taskId);
        }
    }
}
