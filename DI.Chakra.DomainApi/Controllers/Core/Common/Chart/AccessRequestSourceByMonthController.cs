
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AccessRequestSourceByMonth")]
    public class AccessRequestSourceByMonthController : BaseController<DomainModel.AccessRequestSourceByMonth, EntityModel.AccessRequestSourceByMonth, Dto.AccessRequestSourceByMonth>
    {
        [HttpGet]
        [Route("GetAccessRequestSourceByYear")]
        public DataTable GetAccessRequestSourceByYear(int year)
        {
            return DomainModel.AccessRequestSourceByMonth.GetAccessRequestSourceByYear(year);
        }

        [HttpGet]
        [Route("GetAccessRequestSourceForUserByYear")]
        public DataTable GetAccessRequestSourceForUserByYear(int year, long userId)
        {
            return DomainModel.AccessRequestSourceByMonth.GetAccessRequestSourceForUserByYear(year, userId);
        }
    }
}
