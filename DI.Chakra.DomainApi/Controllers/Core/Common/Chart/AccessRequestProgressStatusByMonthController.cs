using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AccessRequestProgressStatusByMonth")]
    public class AccessRequestProgressStatusByMonthController : BaseController<DomainModel.AccessRequestProgressStatusByMonth, EntityModel.AccessRequestProgressStatusByMonth, Dto.AccessRequestProgressStatusByMonth>
    {
        [HttpGet]
        [Route("GetAccessRequestProgressStatusByUser")]
        public DataTable GetAccessRequestProgressStatusByUser(int Year, int Month, long UserId)
        {
            return DomainModel.AccessRequestProgressStatusByMonth.GetAccessRequestProgressStatusByUser(Year, Month, UserId);
        }

        [HttpGet]
        [Route("GetAccessRequestProgressStatusByAdmin")]
        public DataTable GetAccessRequestProgressStatusByAdmin(int Year, int Month)
        {
            return DomainModel.AccessRequestProgressStatusByMonth.GetAccessRequestProgressStatusByAdmin(Year, Month);
        }
    }
}
