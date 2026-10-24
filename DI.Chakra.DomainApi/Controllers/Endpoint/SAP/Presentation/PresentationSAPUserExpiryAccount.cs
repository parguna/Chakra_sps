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
    [RoutePrefix("PresentationSAPUserExpiryAccount")]

    public class PresentationSAPUserExpiryAccountController : ApiController
    {
        public  PresentationSAPUserExpiryAccount DtoMap(DomainModel.Presentation.PresentationSAPUserExpiryAccount tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPUserExpiryAccount, PresentationSAPUserExpiryAccount>(tModel);
        }

        public  List<PresentationSAPUserExpiryAccount> DtoMapList(IList<DomainModel.Presentation.PresentationSAPUserExpiryAccount> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetSAPUserExpiryAccountReportData")]
        public IList<PresentationSAPUserExpiryAccount> GetSAPUserExpiryAccountReportData(long EndpointSystemId, int accountExpiryDays)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPUserExpiryAccount.GetSAPUserExpiryAccountReportData(EndpointSystemId, accountExpiryDays));
        }
    }
}
