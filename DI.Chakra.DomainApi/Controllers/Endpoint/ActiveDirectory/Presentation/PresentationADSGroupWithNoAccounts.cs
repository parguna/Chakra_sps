
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
    [RoutePrefix("PresentationADSGroupWithNoAccounts")]
    
    public class PresentationADSGroupWithNoAccountsController : ApiController
    {
        public PresentationADSGroupWithNoAccounts DtoMap(DomainModel.Presentation.PresentationADSGroupWithNoAccounts tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationADSGroupWithNoAccounts, PresentationADSGroupWithNoAccounts>(tModel);
        }

        public  List<PresentationADSGroupWithNoAccounts> DtoMapList(IList<DomainModel.Presentation.PresentationADSGroupWithNoAccounts> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetADSGroupWithNoAccountsReportData")]
        public IList<PresentationADSGroupWithNoAccounts> GetADSGroupWithNoAccountsReportData(long endpointsystemid)
        {
            return DtoMapList(DomainModel.Presentation.PresentationADSGroupWithNoAccounts.GetByEndpointIdIncludeGroupWithNoAccounts(endpointsystemid));
        }
    }
}
