using AutoMapper;
using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationADSFuturePasswordExpiryAccount")]
    
    public class PresentationADSFuturePasswordExpiryAccountController : ApiController
    {
        public PresentationADSFuturePasswordExpiryAccount DtoMap(DomainModel.Presentation.PresentationADSFuturePasswordExpiryAccount tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationADSFuturePasswordExpiryAccount, PresentationADSFuturePasswordExpiryAccount>(tModel);
        }

        public  List<PresentationADSFuturePasswordExpiryAccount> DtoMapList(IList<DomainModel.Presentation.PresentationADSFuturePasswordExpiryAccount> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetADSFuturePasswordExpiryAccountReportData")]
        public IList<PresentationADSFuturePasswordExpiryAccount> GetADSFuturePasswordExpiryAccountReportData(long endpointsystemid, int passwordExpiryDays)
        {
            return DtoMapList(DomainModel.Presentation.PresentationADSFuturePasswordExpiryAccount.GetByEndpointSystemIdAndPasswordExpiryDays(endpointsystemid, passwordExpiryDays));
        }
    }
}
