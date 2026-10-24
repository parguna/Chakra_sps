
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
    [RoutePrefix("PresentationADSLockedDisabledAccount")]
    
    public class PresentationADSLockedDisabledAccountController : ApiController
    {
        public PresentationADSLockedDisabledAccount DtoMap(DomainModel.Presentation.PresentationADSLockedDisabledAccount tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationADSLockedDisabledAccount, PresentationADSLockedDisabledAccount>(tModel);
        }

        public  List<PresentationADSLockedDisabledAccount> DtoMapList(IList<DomainModel.Presentation.PresentationADSLockedDisabledAccount> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetADSUserByLockedDisabledReportData")]
        public IList<PresentationADSLockedDisabledAccount> GetADSUserByLockedDisabledReportData(long endpointsystemid)
        {
            return DtoMapList(DomainModel.Presentation.PresentationADSLockedDisabledAccount.GetByEndpointIdIncludeLockedDisabledAccounts(endpointsystemid));
        }
    }
}
