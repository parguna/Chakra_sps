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
    [RoutePrefix("PresentationADSFutureAccountExpiryAccount")]
    
    public class PresentationADSFutureAccountExpiryAccountController : ApiController
    {
        public PresentationADSFutureAccountExpiryAccount DtoMap(DomainModel.Presentation.PresentationADSFutureAccountExpiryAccount tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationADSFutureAccountExpiryAccount, PresentationADSFutureAccountExpiryAccount>(tModel);
        }

        public  List<PresentationADSFutureAccountExpiryAccount> DtoMapList(IList<DomainModel.Presentation.PresentationADSFutureAccountExpiryAccount> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetADSFutureAccountExpiryAccountReportData")]
        public IList<PresentationADSFutureAccountExpiryAccount> GetADSFutureAccountExpiryAccountReportData(long endpointsystemid, int accountExpiryDays)
        {
            return DtoMapList(DomainModel.Presentation.PresentationADSFutureAccountExpiryAccount.GetByEndpointSystemIdAndAccountExpiryDays(endpointsystemid, accountExpiryDays));
        }
    }
}
