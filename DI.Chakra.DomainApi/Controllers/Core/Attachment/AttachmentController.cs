using AutoMapper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("Attachment")]
    public class AttachmentController : BaseController<DomainModel.Attachment, EntityModel.Attachment, Attachment>
    {
        [Route("GetByRequestId")]
        [HttpGet]
        public IList<Dto.Attachment> GetByRequestId(long requestId, string source)
        {
            return DtoMapList(DomainModel.Attachment.GetByRequestId(requestId, source));
        }

        [Route("GetAttachmentCount")]
        [HttpGet]
        public long GetAttachmentCount(long taskId, string source)
        {
            return DomainModel.Attachment.GetAttachmentCount(taskId,source);
        }

        public Attachment DtoMap(DomainModel.Attachment tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Attachment, Attachment>(tModel);
        }

        public List<Attachment> DtoMapList(IList<DomainModel.Attachment> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
