using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto.Core;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationEmployeeEventTrigger")]
    public class PresentationEmployeeEventTriggerController : EmployeeEventTriggerController
    {
        [HttpGet]
        [Route("GetById")]
        public PresentationEmployeeEventTrigger GetById(long id)
        {
            return DtoMap(DomainModel.Presentation.PresentationEmployeeEventTrigger.GetById(id));
        }
        [HttpGet]
        [Route("GetAll")]
        public List<PresentationEmployeeEventTrigger> GetAll()
        {
            return DtoMapList(DomainModel.Presentation.PresentationEmployeeEventTrigger.GetAll());
        }
        [HttpPost]
        [Route("GetLazyLoadEmployeeEventTrigger")]
        public Dto.Core.PresentationGridPagedData<Dto.Presentation.PresentationEmployeeEventTrigger> GetLazyLoadEmployeeEventTrigger(EmployeeEvents eventType,PresentationGridPagedDataParameter param,bool includeProcessed)
        {
            var responseData = new Dto.Core.PresentationGridPagedData<Dto.Presentation.PresentationEmployeeEventTrigger>();
            var result = DomainModel.Presentation.PresentationEmployeeEventTrigger.GetLazyLoadEmployeeEventTrigger(eventType, param.StartRowIndex, param.MaximumRows, param.FilterExpression, param.SortExpression, includeProcessed);
            responseData.Data = DtoMapList(result.Data);
            responseData.TotalCount = result.TotalCount;
            return responseData;
        }
        [HttpGet]
        [Route("GetByEventType")]
        public List<PresentationEmployeeEventTrigger> GetByEventType(EmployeeEvents evnt)
        {
            return DtoMapList(DomainModel.Presentation.PresentationEmployeeEventTrigger.GetByEventType(evnt));
        }
        public  PresentationEmployeeEventTrigger DtoMap(DomainModel.Presentation.PresentationEmployeeEventTrigger tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationEmployeeEventTrigger, PresentationEmployeeEventTrigger>(tModel);
        }
        public  List<PresentationEmployeeEventTrigger> DtoMapList(IList<DomainModel.Presentation.PresentationEmployeeEventTrigger> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
