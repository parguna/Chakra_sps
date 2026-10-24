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
    [RoutePrefix("AdhocRequestAccountTyebyMonth")]
    public class AdhocRequestAccountTyebyMonthController : BaseController<DomainModel.AdhocRequestAccountTyebyMonth, EntityModel.AdhocRequestAccountTyebyMonth, Dto.AdhocRequestAccountTyebyMonth>
    {
        [HttpGet]
        [Route("GetAccessRequestAccountType")]
        public DataTable GetAccessRequestAccountType(int Year, int Month)
        {
            return DomainModel.AdhocRequestAccountTyebyMonth.GetAccessRequestAccountType(Year, Month);
        }

        [HttpGet]
        [Route("GetUserAccessRequestByAccountType")]
        public DataTable GetUserAccessRequestByAccountType(int Year, int Month, long UserId)
        {
            return DomainModel.AdhocRequestAccountTyebyMonth.GetUserAccessRequestByAccountType(Year, Month, UserId);
        }
    }
}
