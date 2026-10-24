using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("GlobalAnswer")]
    public class GlobalAnswerController : BaseController<DomainModel.GlobalAnswer, EntityModel.GlobalAnswer, Dto.GlobalAnswer>
    {
        [HttpGet]
        [Route("GetByUserId")]
        public List<GlobalAnswer> GetByUserId(long userId)
        {
           return DtoMapList(DomainModel.GlobalAnswer.GetByUserId(userId));
        }

        [HttpGet]
        [Route("HasAnyQuestionByUserId")]
        public bool HasAnyQuestionByUserId(long userId, string question)
        {
            return DomainModel.GlobalAnswer.HasAnyQuestionByUserId(userId, question);
        }


        [HttpGet]
        [Route("GetByUser")]
        [AllowAnonymous]
        public List<GlobalAnswer> GetByUser(long userId)
        {
            return DtoMapList(DomainModel.GlobalAnswer.GetByUserId(userId));
        }

        [HttpGet]
        [Route("GetsecurityquestionById")]
        [AllowAnonymous]
        public GlobalAnswer GetsecurityquestionById(long userId)
        {
            return DtoMap(DomainModel.GlobalAnswer.GetsecurityquestionById(userId));
        }
    }
}
