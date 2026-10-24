using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("GlobalQuestion")]
    public class GlobalQuestionController : BaseController<DomainModel.GlobalQuestion, EntityModel.GlobalQuestion, GlobalQuestion>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string question)
        {
            return DomainModel.GlobalQuestion.DoesExists(question);
        }
    }
}
