using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NDesignation")]
    public class NDesignationController : BaseController<DomainModel.NDesignation, EntityModel.NDesignation, Dto.NDesignation>
    {
        [HttpGet]
        [Route("GetNDesignations")]
        public  Dictionary<long, string> GetNDesignations(string searchString)
        {
            return DomainModel.NDesignation.GetNDesignations(searchString);
        }

        [HttpGet]
        [Route("GetById")]
        public Dto.NDesignation GetById(long Id)
        {
            return DtoMap(DomainModel.NDesignation.GetById(Id));
        }
    }
}
