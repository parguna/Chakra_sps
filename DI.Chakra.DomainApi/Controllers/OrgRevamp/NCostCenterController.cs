using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NCostCenter")]
    public class NCostCenterController : BaseController<DomainModel.NCostCenter, EntityModel.NCostCenter, Dto.NCostCenter>
    {
        [HttpGet]
        [Route("GetCostCenters")]
        public Dictionary<long, string> GetNCostCenters(string searchString)
        {
            return DomainModel.NCostCenter.GetNCostCenters(searchString);
        }

        [HttpGet]
        [Route("GetById")]
        public Dto.NCostCenter GetById(long Id)
        {
            return DtoMap(DomainModel.NCostCenter.GetById(Id));
        }
    }
}
