using AutoMapper;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationADSDormantAccount")]
    
    public class PresentationADSDormantAccountController : ApiController
    {
        public PresentationADSDormantAccount DtoMap(DomainModel.Presentation.PresentationADSDormantAccount tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationADSDormantAccount, PresentationADSDormantAccount>(tModel);
        }

        public  List<PresentationADSDormantAccount> DtoMapList(IList<DomainModel.Presentation.PresentationADSDormantAccount> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        public PagedResults<PresentationADSDormantAccount> DtoMapListPaging(PagedResults<DomainModel.Presentation.PresentationADSDormantAccount> tModelList)
        {
            var results = tModelList.Results;
            var mappedResults = results.Select(DtoMap).ToList();

            return new PagedResults<PresentationADSDormantAccount>()
            {
                PageNumber = tModelList.PageNumber,
                PageSize = tModelList.PageSize,
                TotalNumberOfPages = tModelList.TotalNumberOfPages,
                TotalNumberOfRecords = tModelList.TotalNumberOfRecords,
                Results = mappedResults
            };
        }

        [HttpGet]
        [Route("GetADSUserByDormantDaysReportData")]
        public IList<PresentationADSDormantAccount> GetADSUserByDormantDaysReportData(long endpointsystemid, int dormantDays)
        {
            return DtoMapList(DomainModel.Presentation.PresentationADSDormantAccount.GetByEndpointSystemIdAndDormantDays(endpointsystemid, dormantDays));
        }
        [HttpGet]
        [Route("GetADSUserByDormantDaysReportData")]
        public IList<PresentationADSDormantAccount> GetADSUserToGroupsReportData(long endpointsystemid, int dormantDays)
        {
            return DtoMapList(DomainModel.Presentation.PresentationADSDormantAccount.GetByEndpointSystemIdAndDormantDays(endpointsystemid, dormantDays));
        }
    }
}
