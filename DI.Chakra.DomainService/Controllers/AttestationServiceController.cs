using DI.Chakra.Connector;
using DI.Chakra.Connector.ActiveDirectory;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainService
{
   
    using DI.Chakra.DomainModel;
    using Infrastructure.Enums;
    using Services.AttestationProcessor;

    [RoutePrefix("AttestationService")]
    public class AttestationServiceController : ApiController
    {
        [HttpPost]
        [Route("Attestation")]

        public bool Attestation(List<ServiceParameter> fields)
        {
            Process process = null;
            try
            {
                var attestationrocessId = Convert.ToInt64(fields.First().Value);
                process = Process.GetByProcessType(attestationrocessId, ProcessType.Attestation);
                AttestationService _service = new AttestationService();
                _service.StartWorkFlow(process);
            }
            catch (Exception ex)
            {
            }

            return true;
        }
    }
}