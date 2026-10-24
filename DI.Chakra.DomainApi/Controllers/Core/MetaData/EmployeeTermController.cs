using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("EmployeeTerm")]
    public class EmployeeTermController : BaseController<DomainModel.EmployeeTerm, EntityModel.EmployeeTerm, EmployeeTerm>
    {
        [HttpGet]
        [Route("GetByIsActive")]
        public  List<EmployeeTerm> GetByIsActive()
        {
            return DtoMapList(DomainModel.EmployeeTerm.GetByIsActive());
        }
    }
}
