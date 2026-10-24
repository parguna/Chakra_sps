using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AccountCategory")]
    public class AccountCategoryController : BaseController<DomainModel.AccountCategory, EntityModel.AccountCategory, AccountCategory>
    {
        [HttpGet]
        [Route("GetByIsActive")]
        public  List<AccountCategory> GetByIsActive()
        {
            return DtoMapList(DomainModel.AccountCategory.GetByIsActive());
        }
    }
}
