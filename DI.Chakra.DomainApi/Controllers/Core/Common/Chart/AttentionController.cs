using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("Attention")]
    public class AttentionController : BaseController<DomainModel.Attention, EntityModel.Attention, Dto.Attention>
    {
        [HttpGet]
        [Route("GetAttention")]
        public List<Attention> GetAttention(long userId, bool AdminMode)
        {
            return DtoMapList(DomainModel.Attention.GetAttention(userId, AdminMode));
        }
    }
}