using AutoMapper;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationADSUsers")]
    
    public class PresentationADSUsersController : ApiController
    {
        public  PresentationADSUsers DtoMap(DomainModel.Presentation.PresentationADSUsers tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationADSUsers, PresentationADSUsers>(tModel);
        }

        public  List<PresentationADSUsers> DtoMapList(IList<DomainModel.Presentation.PresentationADSUsers> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        public PagedResults<PresentationADSUsers> DtoMapListPaging(PagedResults<DomainModel.Presentation.PresentationADSUsers> tModelList)
        {
            var results = tModelList.Results;
            var mappedResults = results.Select(DtoMap).ToList();

            return new PagedResults<PresentationADSUsers>()
            {
                PageNumber = tModelList.PageNumber,
                PageSize = tModelList.PageSize,
                TotalNumberOfPages = tModelList.TotalNumberOfPages,
                TotalNumberOfRecords = tModelList.TotalNumberOfRecords,
                Results = mappedResults
            };
        }

        [HttpGet]
        [Route("GetByEndPointSystemId")]
        public  List<PresentationADSUsers> GetByEndPointSystemId(long endpointSystemid) 
        {
            return DtoMapList(DomainModel.Presentation.PresentationADSUsers.GetByEndpointSystemId(endpointSystemid));
        }
    }
}
