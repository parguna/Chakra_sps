using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto.Presentation;
using AutoMapper;
using DI.Chakra.Dto.Core;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationMail")] 
    public class PresentationMailController : EmailInfoController
    {
        [HttpGet]
        [Route("GetByActivityType")]
        public List<PresentationMail> GetByActivityType(string activityType, string status)
        {
            return DtoMapList(DomainModel.Presentation.PresentationMail.GetByActivityType(activityType,status));
        }
        [HttpPost]
        [Route("GetPagedDataByActivityType")]
        public Dto.Core.PresentationGridPagedData<PresentationMail> GetPagedDataByActivityType(string activityType, string status, PresentationGridPagedDataParameter param)
        {
            var response = new Dto.Core.PresentationGridPagedData<PresentationMail>();
            var dmRsult = DomainModel.Presentation.PresentationMail.GetByActivityType(activityType, status, param.StartRowIndex, param.MaximumRows, param.FilterExpression, param.SortExpression,param.FromDate,param.ToDate);
            response.TotalCount = dmRsult.TotalCount;
            response.Data = DtoMapList(dmRsult.Data);
            return response;
        }
        public  PresentationMail DtoMap(DomainModel.Presentation.PresentationMail tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationMail, Dto.Presentation.PresentationMail>(tModel);
        }

        public  List<Dto.Presentation.PresentationMail> DtoMapList(IList<DomainModel.Presentation.PresentationMail> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
        [HttpPost]
        [Route("DownloadEmail")]
        public byte[] DownloadEmail(string activityType, string status, string exportFileType, PresentationGridPagedDataParameter param)
        {
            return DomainModel.Presentation.PresentationMail.DownloadEmail(activityType, status, param.StartRowIndex, param.MaximumRows, param.FilterExpression, param.SortExpression, exportFileType, param.FromDate, param.ToDate);
        }
    }
}
